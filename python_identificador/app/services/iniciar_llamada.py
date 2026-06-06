# app/services/iniciar_llamada.py
import threading
from datetime import datetime, timedelta
from app.utils.crypto import desencriptar_aes

# Lista de llamadas activas (compartida entre hilos)
# Cada elemento: {"id_llamada": str, "telefono_origen": str, "telefono_destino": str,
#                 "fecha_inicio": str, "hora_fin_maxima": datetime, "tipo_servicio": str}
llamadas_activas = []
lock_llamadas = threading.Lock()

def agregar_llamada_activa(id_llamada, telefono_origen, telefono_destino, 
                           fecha_inicio, tiempo_maximo_segundos, tipo_servicio="PREPAGO"):
    """Agrega una llamada a la lista de llamadas activas (thread-safe)"""
    with lock_llamadas:
        # Calcular hora de finalización máxima
        fecha_inicio_dt = datetime.fromisoformat(fecha_inicio)
        hora_fin = fecha_inicio_dt + timedelta(seconds=tiempo_maximo_segundos)
        
        llamada = {
            "id_llamada": id_llamada,
            "telefono_origen": telefono_origen,
            "telefono_destino": telefono_destino,
            "fecha_inicio": fecha_inicio,
            "hora_fin_maxima": hora_fin,
            "tipo_servicio": tipo_servicio
        }
        llamadas_activas.append(llamada)
        # Ordenar por hora_fin_maxima (más cercanas primero)
        llamadas_activas.sort(key=lambda x: x["hora_fin_maxima"])
        return llamada

def obtener_llamada_activa(id_llamada):
    """Busca una llamada activa por su ID"""
    with lock_llamadas:
        for llamada in llamadas_activas:
            if llamada["id_llamada"] == id_llamada:
                return llamada
        return None

def eliminar_llamada_activa(id_llamada):
    """Elimina una llamada de la lista de activas y la retorna"""
    with lock_llamadas:
        for i, llamada in enumerate(llamadas_activas):
            if llamada["id_llamada"] == id_llamada:
                return llamadas_activas.pop(i)
        return None

def procesar_inicio_llamada(trama_json: dict) -> dict:
    """
    HU Identificador2: Iniciar llamada.
    Recibe el aviso de inicio de llamada y la registra en la lista de llamadas activas.
    
    Campos esperados del contrato inicio_llamada.json:
    - tipo_transaccion: "INICIO_LLAMADA"
    - datos_llamada: {
        id_llamada, telefono_origen, telefono_destino, fecha_inicio, estado
      }
    - control: {
        tipo_servicio, tiempo_maximo_segundos, validar_saldo, monitoreo_activo
      }
    """
    try:
        datos_llamada = trama_json.get("datos_llamada", {})
        control = trama_json.get("control", {})
        
        # Validar campos requeridos
        if not datos_llamada.get("id_llamada") or not datos_llamada.get("telefono_origen"):
            return {
                "tipo_transaccion": "RESPUESTA_ERROR",
                "resultado": {
                    "codigo": "ERROR",
                    "estado": "RECHAZADA",
                    "mensaje": "Datos de llamada incompletos"
                }
            }
        
        id_llamada = datos_llamada["id_llamada"]
        telefono_origen = datos_llamada["telefono_origen"]
        telefono_destino = datos_llamada.get("telefono_destino", "")
        fecha_inicio = datos_llamada.get("fecha_inicio", "")
        tipo_servicio = control.get("tipo_servicio", "PREPAGO")
        tiempo_maximo = control.get("tiempo_maximo_segundos", 1800)
        
        # Registrar la llamada como activa
        agregar_llamada_activa(
            id_llamada=id_llamada,
            telefono_origen=telefono_origen,
            telefono_destino=telefono_destino,
            fecha_inicio=fecha_inicio,
            tiempo_maximo_segundos=tiempo_maximo,
            tipo_servicio=tipo_servicio
        )
        
        print(f"[+] Llamada {id_llamada} registrada como activa. Origen: {telefono_origen}")
        
        return {
            "tipo_transaccion": "RESPUESTA_INICIO",
            "resultado": {
                "codigo": "OK",
                "estado": "LLAMADA_INICIADA",
                "mensaje": "Llamada iniciada correctamente"
            },
            "id_llamada": id_llamada
        }
        
    except Exception as e:
        print(f"[-] Error en inicio de llamada: {e}")
        return {
            "tipo_transaccion": "RESPUESTA_ERROR",
            "resultado": {
                "codigo": "ERROR",
                "estado": "RECHAZADA",
                "mensaje": "Error al iniciar la llamada"
            }
        }