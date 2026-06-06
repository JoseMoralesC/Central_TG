# app/services/consulta.py
from datetime import datetime
from app.utils.crypto import desencriptar_aes
from app.services.proveedor_cliente import enviar_al_proveedor

def consultar_saldo_proveedor(telefono_origen: str) -> dict:
    """
    Consulta el saldo al Proveedor Java usando JSON.
    Usa el mismo contrato consulta_proveedor.json pero con acción "CONSULTAR_SALDO".
    
    Envía:
    {
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "CONSULTAR_SALDO",
        "telefono_origen": "...",
        "datos_tarifa": { "moneda": "CRC" },
        "fecha_hora": "..."
    }
    
    Respuesta esperada (respuesta_proveedor.json):
    {
        "tipo_transaccion": "RESPUESTA_PROVEEDOR",
        "resultado": { "codigo": "OK", ... },
        "datos_autorizacion": { "saldo_disponible": ..., "moneda": "CRC" }
    }
    """
    trama_consulta = {
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "CONSULTAR_SALDO",
        "telefono_origen": telefono_origen,
        "datos_tarifa": {
            "moneda": "CRC"
        },
        "fecha_hora": datetime.now().isoformat()
    }
    
    respuesta = enviar_al_proveedor(trama_consulta)
    return respuesta

def procesar_consulta_saldo(trama_json: dict) -> dict:
    """
    HU Identificador4: Consulta de saldo.
    Recibe solicitud de consulta de saldo, valida datos y consulta al proveedor.
    
    Campos esperados del contrato consulta_saldo.json:
    - tipo_transaccion: "CONSULTA_SALDO"
    - telefono_origen: str (cifrado)
    - identificador_dispositivo: str (cifrado, 16 dígitos)
    - identificador_tarjeta: str (cifrado, 19 dígitos)
    - fecha_hora: str
    """
    # Validar campos requeridos
    campos_requeridos = [
        "telefono_origen", "identificador_dispositivo", 
        "identificador_tarjeta", "tipo_transaccion"
    ]
    if not all(k in trama_json and trama_json[k] for k in campos_requeridos):
        return {
            "tipo_transaccion": "RESPUESTA_SALDO",
            "resultado": {
                "codigo": "ERROR",
                "estado": "CONSULTA_FALLIDA",
                "mensaje": "Datos incompletos"
            }
        }

    # Desencriptar campos sensibles
    try:
        telefono_origen = desencriptar_aes(trama_json["telefono_origen"])
        id_dispositivo = desencriptar_aes(trama_json["identificador_dispositivo"])
        id_tarjeta = desencriptar_aes(trama_json["identificador_tarjeta"])
    except Exception:
        telefono_origen = trama_json["telefono_origen"]
        id_dispositivo = trama_json["identificador_dispositivo"]
        id_tarjeta = trama_json["identificador_tarjeta"]
        print("[!] Advertencia: Usando datos en plano (sin desencriptar)")

    # Validar tipo de transacción
    tipo_tx = trama_json.get("tipo_transaccion")
    if tipo_tx != "CONSULTA_SALDO":
        return {
            "tipo_transaccion": "RESPUESTA_SALDO",
            "resultado": {
                "codigo": "ERROR",
                "estado": "CONSULTA_FALLIDA",
                "mensaje": "Acción inválida"
            }
        }

    # Validaciones contra MySQL (simuladas - BD no disponible aún)
    try:
        # TODO: Implementar consultas reales a MySQL
        # Validar que el teléfono existe, está activo y la tarjeta coincide
        
        # Simulación para pruebas
        registro_origen = {
            "numero": telefono_origen,
            "activo": True,
            "id_tarjeta": id_tarjeta,
            "tipo_servicio": "PREPAGO"
        }
        
        if not registro_origen or not registro_origen.get("activo"):
            return {
                "tipo_transaccion": "RESPUESTA_SALDO",
                "resultado": {
                    "codigo": "TEL_INACTIVO",
                    "estado": "CONSULTA_FALLIDA",
                    "mensaje": "El teléfono se encuentra inactivo"
                }
            }
            
        if registro_origen["id_tarjeta"] != id_tarjeta:
            return {
                "tipo_transaccion": "RESPUESTA_SALDO",
                "resultado": {
                    "codigo": "SIM_INVALIDA",
                    "estado": "CONSULTA_FALLIDA",
                    "mensaje": "La tarjeta SIM no corresponde al teléfono"
                }
            }

    except Exception as db_err:
        print(f"[-] Error en consultas MySQL: {db_err}")
        return {
            "tipo_transaccion": "RESPUESTA_SALDO",
            "resultado": {
                "codigo": "ERROR",
                "estado": "CONSULTA_FALLIDA",
                "mensaje": "Error no controlado"
            }
        }

    # Consultar saldo al proveedor
    resultado = consultar_saldo_proveedor(telefono_origen)
    
    resultado_codigo = resultado.get("resultado", {}).get("codigo", "ERROR")
    datos_autorizacion = resultado.get("datos_autorizacion", {})
    
    if resultado_codigo == "OK":
        saldo_disponible = datos_autorizacion.get("saldo_disponible", 0)
        return {
            "tipo_transaccion": "RESPUESTA_SALDO",
            "telefono_origen": telefono_origen,
            "resultado": {
                "codigo": "OK",
                "estado": "CONSULTA_EXITOSA",
                "mensaje": "Consulta realizada correctamente"
            },
            "datos_saldo": {
                "tipo_servicio": "PREPAGO",
                "saldo_disponible": saldo_disponible,
                "moneda": datos_autorizacion.get("moneda", "CRC"),
                "fecha_consulta": trama_json.get("fecha_hora", datetime.now().isoformat())
            }
        }
    else:
        return {
            "tipo_transaccion": "RESPUESTA_SALDO",
            "resultado": {
                "codigo": "ERROR",
                "estado": "CONSULTA_FALLIDA",
                "mensaje": resultado.get("resultado", {}).get("mensaje", "Error no controlado")
            }
        }