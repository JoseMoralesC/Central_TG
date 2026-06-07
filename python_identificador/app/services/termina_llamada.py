# app/services/termina_llamada.py
import threading
import time
from datetime import datetime
from app.config.config import settings
from app.services.proveedor_cliente import enviar_al_proveedor
from app.services.iniciar_llamada import (
    llamadas_activas, lock_llamadas, eliminar_llamada_activa
)

def enviar_procesar_cobro(telefono_origen: str, telefono_destino: str,
                           duracion_segundos: int, monto_total: float,
                           tipo_servicio: str, tipo_llamada: str,
                           motivo_finalizacion: str) -> dict:
    """
    Envía solicitud de cobro al Proveedor Java usando el contrato consulta_proveedor.json
    con acción "PROCESAR_COBRO".
    
    Envía:
    {
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "PROCESAR_COBRO",
        "telefono_origen": "...",
        "telefono_destino": "...",
        "datos_llamada": { "duracion_segundos": 600, "motivo_finalizacion": "..." },
        "datos_cobro": { "tipo_servicio": "PREPAGO", "tipo_llamada": "NACIONAL",
                         "monto_total": 100.00, "moneda": "CRC" },
        "fecha_hora": "..."
    }
    
    Retorna el resultado de enviar_al_proveedor().
    """
    trama_cobro = {
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "REBAJAR_SALDO",
        "telefono_origen": telefono_origen,
        "telefono_destino": telefono_destino,
        "datos_llamada": {
            "duracion_segundos": duracion_segundos,
            "motivo_finalizacion": motivo_finalizacion
        },
        "datos_cobro": {
            "tipo_servicio": tipo_servicio,
            "tipo_llamada": tipo_llamada,
            "monto_total": monto_total,
            "moneda": "CRC"
        },
        "fecha_hora": datetime.now().isoformat()
    }
    
    respuesta = enviar_al_proveedor(trama_cobro)
    return respuesta

def verificar_llamadas_vencidas():
    """
    Hilo que revisa periódicamente las llamadas activas para detectar
    si alguna ha excedido su tiempo máximo (saldo agotado).
    """
    while True:
        try:
            with lock_llamadas:
                ahora = datetime.now()
                vencidas = []
                
                for llamada in llamadas_activas:
                    if llamada["hora_fin_maxima"] <= ahora:
                        vencidas.append(llamada)
                
                for llamada in vencidas:
                    llamadas_activas.remove(llamada)
                    print(f"[!] Llamada {llamada['id_llamada']} vencida por saldo agotado")
                    
                    # Enviar rebajo al proveedor si es prepago
                    if llamada["tipo_servicio"] == "PREPAGO":
                        duracion = (llamada["hora_fin_maxima"] - datetime.fromisoformat(llamada["fecha_inicio"])).total_seconds()
                        
                        # Calcular monto estimado basado en duración y tarifa base
                        # (costo_por_minuto se obtendría del proveedor idealmente)
                        costo_por_minuto = 10.00
                        monto_estimado = (duracion / 60) * costo_por_minuto
                        
                        enviar_procesar_cobro(
                            telefono_origen=llamada["telefono_origen"],
                            telefono_destino=llamada["telefono_destino"],
                            duracion_segundos=int(duracion),
                            monto_total=round(monto_estimado, 2),
                            tipo_servicio="PREPAGO",
                            tipo_llamada="NACIONAL",
                            motivo_finalizacion="SALDO_AGOTADO"
                        )
            
            # Dormir 5 segundos antes de la próxima verificación
            time.sleep(5)
        except Exception as e:
            print(f"[-] Error en verificador de llamadas vencidas: {e}")
            time.sleep(5)

def iniciar_verificador_llamadas():
    """Inicia el hilo que verifica llamadas vencidas en segundo plano"""
    hilo = threading.Thread(target=verificar_llamadas_vencidas, daemon=True)
    hilo.start()
    print("[Verificador] Hilo de monitoreo de llamadas activas iniciado")

def procesar_finalizacion_llamada(trama_json: dict) -> dict:
    """
    HU Identificador3: Terminación de llamadas.
    Recibe la solicitud de finalización y procesa el rebajo con el proveedor.
    
    Campos esperados del contrato finalizar_llamada.json:
    - tipo_transaccion: "FINALIZAR_LLAMADA"
    - datos_llamada: { id_llamada, telefono_origen, telefono_destino, 
                       fecha_inicio, fecha_fin, duracion_segundos, 
                       duracion_minutos, motivo_finalizacion }
    - datos_cobro: { tipo_servicio, tipo_llamada, costo_por_minuto, 
                     monto_total, moneda }
    """
    try:
        datos_llamada = trama_json.get("datos_llamada", {})
        datos_cobro = trama_json.get("datos_cobro", {})
        
        id_llamada = datos_llamada.get("id_llamada", "")
        telefono_origen = datos_llamada.get("telefono_origen", "")
        telefono_destino = datos_llamada.get("telefono_destino", "")
        duracion_segundos = datos_llamada.get("duracion_segundos", 0)
        motivo_finalizacion = datos_llamada.get("motivo_finalizacion", "FINALIZACION_MANUAL")
        
        tipo_servicio = datos_cobro.get("tipo_servicio", "PREPAGO")
        tipo_llamada = datos_cobro.get("tipo_llamada", "NACIONAL")
        monto_total = datos_cobro.get("monto_total", 0)
        
        # Buscar y eliminar la llamada de la lista de activas
        llamada_activa = eliminar_llamada_activa(id_llamada)
        
        if not llamada_activa:
            print(f"[-] Llamada {id_llamada} no encontrada en lista de activas")
        
        # Enviar al proveedor para procesar el cobro usando el contrato estándar
        respuesta_proveedor = enviar_procesar_cobro(
            telefono_origen=telefono_origen,
            telefono_destino=telefono_destino,
            duracion_segundos=duracion_segundos,
            monto_total=float(monto_total),
            tipo_servicio=tipo_servicio,
            tipo_llamada=tipo_llamada,
            motivo_finalizacion=motivo_finalizacion
        )
        
        # Verificar respuesta del proveedor
        # Java responde con {"status":"OK",...} o {"resultado":{"codigo":"OK",...}}
        resultado_codigo = (
            respuesta_proveedor.get("resultado", {}).get("codigo") or
            respuesta_proveedor.get("status") or
            "ERROR"
        )
        
        if resultado_codigo == "OK":
            return {
                "tipo_transaccion": "RESPUESTA_FINALIZACION",
                "resultado": {
                    "codigo": "OK",
                    "estado": "LLAMADA_FINALIZADA",
                    "mensaje": "La llamada fue finalizada y registrada correctamente"
                },
                "movimiento": {
                    "saldo_anterior": respuesta_proveedor.get("datos_autorizacion", {}).get("saldo_anterior", 0),
                    "monto_rebajado": float(monto_total),
                    "saldo_actual": respuesta_proveedor.get("datos_autorizacion", {}).get("saldo_actual", 0)
                }
            }
        else:
            return {
                "tipo_transaccion": "RESPUESTA_FINALIZACION",
                "resultado": {
                    "codigo": "ERROR",
                    "estado": "FALLIDO",
                    "mensaje": respuesta_proveedor.get("resultado", {}).get("mensaje", "Error al registrar el movimiento en el proveedor")
                }
            }
            
    except Exception as e:
        print(f"[-] Error en finalización de llamada: {e}")
        return {
            "tipo_transaccion": "RESPUESTA_FINALIZACION",
            "resultado": {
                "codigo": "ERROR",
                "estado": "FALLIDO",
                "mensaje": "Error al finalizar la llamada"
            }
        }