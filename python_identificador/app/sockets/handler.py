import json
import logging
from app.services.autorizacion_llamada import procesar_autorizacion_llamada
from app.services.iniciar_llamada import procesar_inicio_llamada
from app.services.termina_llamada import procesar_finalizacion_llamada
from app.services.consulta import procesar_consulta_saldo

# Cola de bitácora (inyectada desde servidor.py)
cola_bitacora = None

def inicializar_handler(cola):
    """Recibe la cola de bitácora desde servidor.py"""
    global cola_bitacora
    cola_bitacora = cola

def registrar_en_bitacora(trama, tipo="ENTRADA"):
    """Registra una trama en la bitácora (entrada o salida)"""
    if cola_bitacora is None:
        return
    try:
        registro = {
            "tipo": tipo,
            "trama": trama
        }
        cola_bitacora.put(registro)
    except Exception as e:
        print(f"[Bitácora] Error encolando registro: {e}")

def manejar_cliente(conexion_cliente, direccion_cliente):
    """
    Despachador central: Valida, Rutea y Responde.
    Soporta los tipos de transacción del protocolo:
    - SOLICITUD_LLAMADA
    - INICIO_LLAMADA
    - FINALIZAR_LLAMADA
    - CONSULTA_SALDO
    """
    try:
        # Leer trama completa (incluyendo \n final)
        data = b""
        while True:
            chunk = conexion_cliente.recv(4096)
            if not chunk:
                break
            data += chunk
            if b"\n" in chunk:
                break
        
        trama_str = data.decode('utf-8').strip()
        if not trama_str:
            return

        trama = json.loads(trama_str)
        
        # Registrar trama de entrada en bitácora
        registrar_en_bitacora(trama, "ENTRADA")
        
        # Router de transacciones
        router = {
            "SOLICITUD_LLAMADA": procesar_autorizacion_llamada,
            "INICIO_LLAMADA": procesar_inicio_llamada,
            "FINALIZAR_LLAMADA": procesar_finalizacion_llamada,
            "CONSULTA_SALDO": procesar_consulta_saldo
        }

        # Validar tipo de transacción
        tipo_tx = trama.get("tipo_transaccion")
        
        if tipo_tx in router:
            resultado = router[tipo_tx](trama)
        else:
            resultado = {
                "tipo_transaccion": "RESPUESTA_ERROR",
                "resultado": {
                    "codigo": "ERROR",
                    "estado": "RECHAZADA",
                    "mensaje": f"Tipo de transacción no soportado: {tipo_tx}"
                }
            }

        # Registrar trama de salida en bitácora
        registrar_en_bitacora(resultado, "SALIDA")
        
        # Enviar respuesta con salto de línea (\n) como requiere el protocolo
        respuesta_json = json.dumps(resultado) + "\n"
        conexion_cliente.sendall(respuesta_json.encode('utf-8'))

    except json.JSONDecodeError:
        error_resp = {
            "tipo_transaccion": "RESPUESTA_ERROR",
            "resultado": {
                "codigo": "ERROR",
                "estado": "RECHAZADA",
                "mensaje": "JSON mal formado"
            }
        }
        conexion_cliente.sendall((json.dumps(error_resp) + "\n").encode('utf-8'))
    except Exception as e:
        print(f"[CRÍTICO] Error en el handler: {e}")
        error_resp = {
            "tipo_transaccion": "RESPUESTA_ERROR",
            "resultado": {
                "codigo": "ERROR",
                "estado": "RECHAZADA",
                "mensaje": "Error interno del servidor"
            }
        }
        conexion_cliente.sendall((json.dumps(error_resp) + "\n").encode('utf-8'))
    finally:
        conexion_cliente.close()