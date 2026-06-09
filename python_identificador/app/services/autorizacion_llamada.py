# app/services/autorizacion_llamada.py
import json
import re
from datetime import datetime
from app.utils.crypto import desencriptar_aes, encriptar_aes
from app.services.proveedor_cliente import enviar_al_proveedor

def validar_coordenadas_costa_rica(ubicacion: dict) -> bool:
    """
    Valida si las coordenadas corresponden a Costa Rica.
    La ubicación viene como objeto: {"pais": "...", "provincia": "...", "latitud": ..., "longitud": ...}
    """
    try:
        lat = float(ubicacion.get("latitud", 0))
        lon = float(ubicacion.get("longitud", 0))
        
        # Rangos geográficos de Costa Rica
        # Latitud: 8.0° N a 11.2° N | Longitud: -86.0° O a -82.5° O
        if (8.0 <= lat <= 11.2) and (-86.0 <= lon <= -82.5):
            return True
        return False
    except Exception:
        return False

def consultar_proveedor_saldo(telefono_origen: str, telefono_destino: str, 
                               tipo_servicio: str, tipo_llamada: str) -> dict:
    """
    Consulta saldo al Proveedor Java usando JSON según contrato consulta_proveedor.json.
    
    Envía:
    {
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "VERIFICAR_SALDO",
        "telefono_origen": "...",
        "telefono_destino": "...",
        "tipo_servicio": "PREPAGO",
        "tipo_llamada": "NACIONAL",
        "datos_tarifa": { "moneda": "CRC", "tiempo_estimado_segundos": 1800 },
        "fecha_hora": "..."
    }
    
    Respuesta esperada (respuesta_proveedor.json):
    {
        "tipo_transaccion": "RESPUESTA_PROVEEDOR",
        "resultado": { "codigo": "OK", "estado": "AUTORIZADO", ... },
        "datos_autorizacion": { "saldo_disponible": ..., "costo_por_minuto": ...,
                                "tiempo_maximo_segundos": ..., "moneda": "CRC" }
    }
    """
    trama_consulta = {
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "VERIFICAR_SALDO",
        "telefono_origen": telefono_origen,
        "telefono_destino": telefono_destino,
        "tipo_servicio": tipo_servicio,
        "tipo_llamada": tipo_llamada,
        "datos_tarifa": {
            "moneda": "CRC",
            "tiempo_estimado_segundos": 1800
        },
        "fecha_hora": datetime.now().isoformat()
    }
    
    respuesta = enviar_al_proveedor(trama_consulta)
    return respuesta

def procesar_autorizacion_llamada(trama_json: dict) -> dict:
    """
    HU Identificador1: Autorizar llamada telefónica.
    Recibe JSON del simulador C#, valida contra MySQL, consulta saldo al proveedor.
    
    Campos esperados del contrato solicitud_llamada.json:
    - tipo_transaccion: "SOLICITUD_LLAMADA"
    - telefono_origen: str (cifrado)
    - telefono_destino: str
    - identificador_dispositivo: str (cifrado, 16 dígitos)
    - identificador_tarjeta: str (cifrado, 19 dígitos)
    - ubicacion: {"pais": str, "provincia": str, "latitud": float, "longitud": float}
    - tipo_llamada: str ("NACIONAL" o "INTERNACIONAL")
    - fecha_hora: str
    """
    # Criterio 2.a: Todos los datos son obligatorios
    campos_requeridos = [
        "telefono_origen", "identificador_dispositivo", "identificador_tarjeta",
        "ubicacion", "tipo_transaccion", "telefono_destino", "tipo_llamada"
    ]
    if not all(k in trama_json and trama_json[k] for k in campos_requeridos):
        return {
            "tipo_transaccion": "RESPUESTA_LLAMADA",
            "resultado": {
                "codigo": "ERROR",
                "estado": "RECHAZADA",
                "mensaje": "Datos incompletos"
            }
        }

    # Desencriptar campos sensibles. Si AES falla, se rechaza la solicitud.
    telefono_origen = desencriptar_aes(trama_json["telefono_origen"])
    id_dispositivo = desencriptar_aes(trama_json["identificador_dispositivo"])
    id_tarjeta = desencriptar_aes(trama_json["identificador_tarjeta"])

    if not telefono_origen or not id_dispositivo or not id_tarjeta:
        return {
            "tipo_transaccion": "RESPUESTA_LLAMADA",
            "resultado": {
                "codigo": "ERROR",
                "estado": "RECHAZADA",
                "mensaje": "Error de seguridad: no fue posible descifrar los datos sensibles"
            }
        }

    ubicacion = trama_json["ubicacion"]
    tipo_tx = trama_json["tipo_transaccion"]
    telefono_destino = trama_json["telefono_destino"]
    tipo_llamada = trama_json.get("tipo_llamada", "NACIONAL")

    # Criterio 2.e: El tipo de transacción debe ser SOLICITUD_LLAMADA
    if tipo_tx != "SOLICITUD_LLAMADA":
        return {
            "tipo_transaccion": "RESPUESTA_LLAMADA",
            "resultado": {
                "codigo": "ERROR",
                "estado": "RECHAZADA",
                "mensaje": "Acción inválida"
            }
        }

    # Criterio 2.d: Validación de ubicación geográfica
    if not validar_coordenadas_costa_rica(ubicacion):
        return {
            "tipo_transaccion": "RESPUESTA_LLAMADA",
            "resultado": {
                "codigo": "UBICACION_INVALIDA",
                "estado": "RECHAZADA",
                "mensaje": "La ubicación enviada no pertenece al territorio nacional permitido"
            }
        }

    # Criterio 2.f: Validar teléfono destino según tipo de llamada
    if tipo_llamada == "INTERNACIONAL":
        # Validar que el código de país internacional sea válido
        # Formatos esperados: +506 (con código), 00506 (con prefijo internacional)
        if not re.match(r"^(\+|00)\d{1,3}", telefono_destino):
            return {
                "tipo_transaccion": "RESPUESTA_LLAMADA",
                "resultado": {
                    "codigo": "ERROR",
                    "estado": "RECHAZADA",
                    "mensaje": "Código de país inválido"
                }
            }
    else:
        # Llamada nacional: validar que el destino sea un número de 8 dígitos
        if not re.match(r"^\d{8}$", telefono_destino):
            return {
                "tipo_transaccion": "RESPUESTA_LLAMADA",
                "resultado": {
                    "codigo": "ERROR",
                    "estado": "RECHAZADA",
                    "mensaje": "Teléfono destino inválido (debe ser 8 dígitos para llamada nacional)"
                }
            }

    # Validaciones contra MySQL (base de datos real compartida)
    # NOTA: La BD almacena datos cifrados (AES). C# envía datos cifrados,
    #       por lo que se compara el valor cifrado directamente con la BD.
    try:
        from app.database.repositorio import (
            buscar_telefono_por_numero_cifrado,
            buscar_tarjeta_por_telefono_id,
            buscar_dispositivo_por_telefono_id
        )
        
        # Criterio 2.b: Buscar teléfono origen por su número cifrado
        # Se usa el valor cifrado original (no desencriptado) para buscar en BD
        telefono_cifrado = trama_json["telefono_origen"]
        registro_origen = buscar_telefono_por_numero_cifrado(telefono_cifrado)
        
        if not registro_origen:
            return {
                "tipo_transaccion": "RESPUESTA_LLAMADA",
                "resultado": {
                    "codigo": "TEL_INACTIVO",
                    "estado": "RECHAZADA",
                    "mensaje": "El teléfono origen no existe en la base de datos"
                }
            }
            
        if not registro_origen.get("activo"):
            return {
                "tipo_transaccion": "RESPUESTA_LLAMADA",
                "resultado": {
                    "codigo": "TEL_INACTIVO",
                    "estado": "RECHAZADA",
                    "mensaje": "El teléfono origen se encuentra inactivo"
                }
            }
        
        # Criterio 2.c: Validar que la tarjeta SIM corresponda al teléfono
        telefono_id = registro_origen["telefono_id"]
        tarjeta_cifrada = trama_json["identificador_tarjeta"]
        tarjeta = buscar_tarjeta_por_telefono_id(telefono_id, tarjeta_cifrada)
        
        if not tarjeta:
            return {
                "tipo_transaccion": "RESPUESTA_LLAMADA",
                "resultado": {
                    "codigo": "SIM_INVALIDA",
                    "estado": "RECHAZADA",
                    "mensaje": "La tarjeta SIM no corresponde al teléfono registrado"
                }
            }
            
        if not tarjeta.get("activa"):
            return {
                "tipo_transaccion": "RESPUESTA_LLAMADA",
                "resultado": {
                    "codigo": "SIM_INVALIDA",
                    "estado": "RECHAZADA",
                    "mensaje": "La tarjeta SIM se encuentra inactiva"
                }
            }
        
        # Validar que el dispositivo corresponda al teléfono
        dispositivo_cifrado = trama_json["identificador_dispositivo"]
        dispositivo = buscar_dispositivo_por_telefono_id(telefono_id, dispositivo_cifrado)
        
        if not dispositivo:
            return {
                "tipo_transaccion": "RESPUESTA_LLAMADA",
                "resultado": {
                    "codigo": "DISPOSITIVO_INVALIDO",
                    "estado": "RECHAZADA",
                    "mensaje": "El dispositivo no corresponde al teléfono registrado"
                }
            }

        if tipo_llamada != "INTERNACIONAL":
            destino_cifrado = encriptar_aes(telefono_destino)
            registro_destino = buscar_telefono_por_numero_cifrado(destino_cifrado)

            if not registro_destino or not registro_destino.get("activo"):
                return {
                    "tipo_transaccion": "RESPUESTA_LLAMADA",
                    "resultado": {
                        "codigo": "ERROR",
                        "estado": "RECHAZADA",
                        "mensaje": "Telefono destino invalido o inactivo"
                    }
                }

    except Exception as db_err:
        print(f"[-] Error en consultas MySQL: {db_err}")
        return {
            "tipo_transaccion": "RESPUESTA_LLAMADA",
            "resultado": {
                "codigo": "ERROR",
                "estado": "RECHAZADA",
                "mensaje": "Error no controlado"
            }
        }

    # Consultar saldo en el Proveedor (Java) usando JSON
    tipo_servicio = registro_origen.get("tipo_servicio", "PREPAGO")
    resultado_proveedor = consultar_proveedor_saldo(
        telefono_origen, telefono_destino, tipo_servicio, tipo_llamada
    )
    
    # Procesar respuesta del proveedor
    resultado_codigo = resultado_proveedor.get(
        "status",
        resultado_proveedor.get("resultado", {}).get("codigo", "ERROR")
    )
    datos_autorizacion = resultado_proveedor.get("datos_autorizacion", {})
    
    if resultado_codigo == "OK":
        tiempo_maximo = resultado_proveedor.get(
            "tiempo_maximo_segundos",
            datos_autorizacion.get("tiempo_maximo_segundos", 1800)
        )
        return {
            "tipo_transaccion": "RESPUESTA_LLAMADA",
            "resultado": {
                "codigo": "OK",
                "estado": "AUTORIZADA",
                "mensaje": "Llamada autorizada correctamente"
            },
            "datos_llamada": {
                "tiempo_maximo_segundos": tiempo_maximo,
                "costo_por_minuto": datos_autorizacion.get("costo_por_minuto", 0),
                "saldo_disponible": datos_autorizacion.get("saldo_disponible", 0)
            }
        }
    elif resultado_codigo == "INSUF" or resultado_proveedor.get("resultado", {}).get("estado") == "SALDO_INSUFICIENTE":
        return {
            "tipo_transaccion": "RESPUESTA_LLAMADA",
            "resultado": {
                "codigo": "INSUF",
                "estado": "RECHAZADA",
                "mensaje": "Saldo insuficiente"
            }
        }
    else:
        return {
            "tipo_transaccion": "RESPUESTA_LLAMADA",
            "resultado": {
                "codigo": "ERROR",
                "estado": "RECHAZADA",
                "mensaje": resultado_proveedor.get(
                    "mensaje",
                    resultado_proveedor.get("resultado", {}).get("mensaje", "Error no controlado")
                )
            }
        }
