# app/sockets/handler.py
import json
from app.services.autorizacion_llamada import procesar_autorizacion_llamada

def manejar_cliente(conexion_cliente, direccion_cliente):
    """
    Atiende la solicitud de un cliente específico de forma síncrona 
    dentro de su propio hilo, sin bloquear el resto de conexiones.
    """
    print(f"🧵 [Hilo] Asignado para atender a: {direccion_cliente}")
    
    try:
        # Recibir la trama de datos (Buffer de 4096 bytes)
        bytes_recibidos = conexion_cliente.recv(4096)
        if not bytes_recibidos:
            return

        texto_recibido = bytes_recibidos.decode('utf-8')
        trama = json.loads(texto_recibido)
        
        tipo_transaccion = trama.get("tipo_transaccion")
        print(f"[Socket] Tipo de transacción recibida: '{tipo_transaccion}'")

        # -----------------------------------------------------------------
        # ENRUTADOR DE HISTORIAS DE USUARIO (HU)
        # -----------------------------------------------------------------
        if tipo_transaccion == "SOLICITUD_LLAMADA":
            # HU Identificador1: Autorizar llamada (Validaciones, AES y MySQL)
            respuesta = procesar_autorizacion_llamada(trama)
            
        elif tipo_transaccion == "INICIAR_LLAMADA":
            # HU Identificador2: Registrar en la lista de llamadas activas
            respuesta = procesar_llamada_pendiente(trama)
            
        elif tipo_transaccion == "TERMINAR_LLAMADA":
            # HU Identificador3: Sacar de la lista y procesar fin
            respuesta = procesar_finalizacion_pendiente(trama)
            
        elif tipo_transaccion == "CONSULTA_SALDO":
            # HU Identificador4: Consulta rápida de saldo
            respuesta = procesar_saldo_pendiente(trama)

        else:
            print(f"[Socket] Transacción desconocida: {tipo_transaccion}")
            respuesta = {"status": "error", "motivo": 4, "message": "Acción inválida"}

        # Enviar la respuesta final en bytes al simulador de C#
        conexion_cliente.sendall(json.dumps(respuesta).encode('utf-8'))

    except json.JSONDecodeError:
        print(f"[Socket] Trama JSON malformada desde {direccion_cliente}")
        respuesta = {"status": "error", "motivo": 5, "message": "Formato JSON inválido"}
        conexion_cliente.sendall(json.dumps(respuesta).encode('utf-8'))
        
    except Exception as e:
        print(f"[Socket] Error inesperado en el hilo de {direccion_cliente}: {e}")
        respuesta = {"status": "error", "motivo": 5, "message": "Error no controlado"}
        conexion_cliente.sendall(json.dumps(respuesta).encode('utf-8'))
        
    finally:
        conexion_cliente.close()
        print(f"[Socket] Conexión con {direccion_cliente} finalizada de forma limpia.\n")


# =====================================================================
# TEMPLATES DE PROCESAMIENTO (PENDIENTES DE CONFIGURAR EN PASOS SIGUIENTES)
# =====================================================================

def procesar_llamada_pendiente(trama):
    print("[Servicio] Procesando HU2: Registrar inicio de llamada...")
    return {"status": "OK", "message": "Llamada registrada en memoria"}

def procesar_finalizacion_pendiente(trama):
    print("[Servicio] Procesando HU3: Terminación de llamada...")
    return {"status": "ok"}

def procesar_saldo_pendiente(trama):
    print("[Servicio] Procesando HU4: Consulta de saldo...")
    return {"status": "OK", "saldo": "0000000012345678900"}