import json
import logging

# Importaciones de servicios (Logica de Negocio)
from app.services.autorizacion_llamada import procesar_autorizacion_llamada
#from app.services.iniciar_llamada import procesar_llamada_pendiente
#from app.services.termina_llamada import procesar_finalizacion_pendiente
#from app.services.consulta import procesar_saldo_pendiente

# Referencia global para la bitácora
bitacora_queue = None

def inicializar_handler(queue_compartida):
    global bitacora_queue
    bitacora_queue = queue_compartida

def enviar_a_bitacora(data):
    if bitacora_queue:
        bitacora_queue.put(data)

def manejar_cliente(conexion_cliente, direccion_cliente):
    """
    Handler defensivo: Valida estructura antes de ejecutar cualquier lógica.
    """
    try:
        # 1. Recepción
        data_raw = conexion_cliente.recv(4096).decode('utf-8')
        if not data_raw:
            return

        # 2. Deserialización segura
        try:
            trama = json.loads(data_raw)
        except json.JSONDecodeError:
            raise ValueError("Formato JSON inválido")

        # 3. Validación de estructura (Garantizar que sea un diccionario)
        if not isinstance(trama, dict):
            raise ValueError("La trama no es un objeto JSON (dict)")

        # Registrar entrada
        enviar_a_bitacora({"tipo": "ENTRADA", "data": trama})

        # 4. Enrutamiento controlado
        tipo_tx = str(trama.get("tipo_transaccion", "")).strip().lower()
        
        # Mapa de funciones para evitar un "if-else" gigante
        router = {
            "solicitud": procesar_autorizacion_llamada,
            "llamada": procesar_llamada_pendiente,
            "finalizacion": procesar_finalizacion_pendiente,
            "saldo": procesar_saldo_pendiente
        }

        if tipo_tx in router:
            respuesta = router[tipo_tx](trama)
        else:
            respuesta = {"status": "ERROR", "motivo": 4, "detalle": "Tipo de transacción desconocido"}

    except ValueError as ve:
        # Errores de formato (JSON mal hecho o tipo incorrecto)
        respuesta = {"status": "ERROR", "motivo": 4, "detalle": str(ve)}
    except Exception as e:
        # Errores críticos de sistema (NullPointer, Base de datos, etc)
        print(f"[!] Error crítico procesando cliente {direccion_cliente}: {e}")
        respuesta = {"status": "ERROR", "motivo": 5, "detalle": "Error interno del servidor"}
    finally:
        # 5. Registro y Envío
        enviar_a_bitacora({"tipo": "SALIDA", "data": respuesta})
        try:
            conexion_cliente.sendall(json.dumps(respuesta, ensure_ascii=False).encode('utf-8'))
        except:
            pass
        conexion_cliente.close()