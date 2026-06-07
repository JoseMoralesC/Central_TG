# app/services/termina_llamada.py
import threading
import time
from datetime import datetime
from app.config.config import settings
from app.services.proveedor_cliente import enviar_al_proveedor
from app.services.iniciar_llamada import (
    llamadas_activas, lock_llamadas, eliminar_llamada_activa
)

def enviar_registro_movimiento(telefono: str, fecha: str, hora: str,
                                telefono_destino: str, costo: float, 
                                duracion_segundos: int, tipo_servicio: str) -> bool:
    """
    Envía registro de movimiento al Proveedor (HU PROVEEDOR2).
    TODO: Pendiente de definir contrato JSON exacto con el equipo de Java.
    
    Por ahora envía estructura genérica que será ajustada cuando el compañero
    de Java defina el contrato específico para registro de movimientos.
    """
    horas = duracion_segundos // 3600
    minutos = (duracion_segundos % 3600) // 60
    segundos = duracion_segundos % 60
    duracion_formateada = f"{horas:02d}:{minutos:02d}:{segundos:02d}"
    
    trama_movimiento = {
        "tipo_transaccion": "REGISTRO_MOVIMIENTO",
        "accion": "REBAJAR_SALDO",
        "telefono_origen": telefono,
        "telefono_destino": telefono_destino,
        "tipo_servicio": tipo_servicio,
        "fecha": fecha,
        "hora": hora,
        "duracion": duracion_formateada,
        "monto": costo,
        "moneda": "CRC"
    }
    
    respuesta = enviar_al_proveedor(trama_movimiento)
    resultado_codigo = respuesta.get(
        "status",
        respuesta.get("resultado", {}).get("codigo", "ERROR")
    )
    return resultado_codigo == "OK"

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
                        fecha = llamada["fecha_inicio"][:10].replace("-", "")
                        hora_inicio = llamada["fecha_inicio"][11:19].replace(":", "")
                        duracion = (llamada["hora_fin_maxima"] - datetime.fromisoformat(llamada["fecha_inicio"])).total_seconds()
                        
                        enviar_registro_movimiento(
                            telefono=llamada["telefono_origen"],
                            fecha=fecha,
                            hora=hora_inicio,
                            telefono_destino=llamada["telefono_destino"],
                            costo=0.0,
                            duracion_segundos=int(duracion),
                            tipo_servicio="PREPAGO"
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
        fecha_inicio = datos_llamada.get("fecha_inicio", "")
        duracion_segundos = datos_llamada.get("duracion_segundos", 0)
        
        tipo_servicio = datos_cobro.get("tipo_servicio", "PREPAGO")
        monto_total = datos_cobro.get("monto_total", 0)
        
        # Buscar y eliminar la llamada de la lista de activas
        llamada_activa = eliminar_llamada_activa(id_llamada)
        
        if not llamada_activa:
            print(f"[-] Llamada {id_llamada} no encontrada en lista de activas")
        
        # Formatear datos para el proveedor
        fecha_formateada = fecha_inicio[:10].replace("-", "") if fecha_inicio else datetime.now().strftime("%Y%m%d")
        hora_formateada = fecha_inicio[11:19].replace(":", "") if len(fecha_inicio) > 10 else datetime.now().strftime("%H%M%S")
        
        # Enviar al proveedor para registrar el movimiento (HU PROVEEDOR2)
        exito = enviar_registro_movimiento(
            telefono=telefono_origen,
            fecha=fecha_formateada,
            hora=hora_formateada,
            telefono_destino=telefono_destino,
            costo=float(monto_total),
            duracion_segundos=duracion_segundos,
            tipo_servicio=tipo_servicio
        )
        
        if exito:
            return {
                "tipo_transaccion": "RESPUESTA_FINALIZACION",
                "resultado": {
                    "codigo": "OK",
                    "estado": "LLAMADA_FINALIZADA",
                    "mensaje": "La llamada fue finalizada y registrada correctamente"
                },
                "movimiento": {
                    "saldo_anterior": 5000.00,
                    "monto_rebajado": float(monto_total),
                    "saldo_actual": 5000.00 - float(monto_total)
                }
            }
        else:
            return {
                "tipo_transaccion": "RESPUESTA_FINALIZACION",
                "resultado": {
                    "codigo": "ERROR",
                    "estado": "FALLIDO",
                    "mensaje": "Error al registrar el movimiento en el proveedor"
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
