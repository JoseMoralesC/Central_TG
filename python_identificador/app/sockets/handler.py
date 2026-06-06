# app/sockets/handler.py
import json

# =====================================================================
# IMPORTS ABSOLUTOS DE TUS SERVICIOS (LÓGICA DE NEGOCIO EN APP/SERVICES)
# =====================================================================
try:
    from app.services.autorizacion_llamada import procesar_autorizacion_llamada  # HU1
    from app.services.iniciar_llamada import procesar_llamada_pendiente          # HU2
    from app.services.termina_llamada import procesar_finalizacion_pendiente     # HU3
    from app.services.consulta import procesar_saldo_pendiente                  # HU4
except ImportError as e:
    print(f"[Handler] Alerta: No se pudieron importar todos los servicios ({e})")
    # Mocks temporales por si aún no has creado los archivos en app/services/
    procesar_autorizacion_llamada = lambda t: {"status": "ERROR", "motivo": 5}
    procesar_llamada_pendiente = lambda t: {"status": "ERROR", "motivo": 5}
    procesar_finalizacion_pendiente = lambda t: {"status": "ERROR", "motivo": 5}
    procesar_saldo_pendiente = lambda t: {"status": "ERROR", "motivo": 5}

# Variable global que almacena la cola compartida por server.py
bitacora_queue = None

def inicializar_handler(queue_compartida):
    """
    Guarda la referencia de la cola de la bitácora para usarla de forma asíncrona.
    """
    global bitacora_queue
    bitacora_queue = queue_compartida

def enviar_a_bitacora(trama):
    """
    Envía una trama de datos a la cola de auditoría (HU5) sin bloquear el hilo.
    """
    if bitacora_queue is not None:
        bitacora_queue.put(trama)

def manejar_cliente(conexion_cliente, direccion_cliente):
    """
    Atiende las solicitudes de red del simulador de C# de forma aislada.
    """
    print(f"🧵 [Hilo] Procesando cliente en red: {direccion_cliente}")
    respuesta = None
    
    try:
        # Recibir la trama del socket (Buffer de 4096 bytes)
        bytes_recibidos = conexion_cliente.recv(4096)
        if not bytes_recibidos:
            return

        texto_recibido = bytes_recibidos.decode('utf-8')
        trama = json.loads(texto_recibido)
        
        # ─── HU5: REGISTRAR TRAMA DE ENTRADA EN COLA ───
        enviar_a_bitacora(trama)

        # Limpiamos y normalizamos el texto de la transacción (Tolerante a fallos de C#)
        tipo_transaccion = str(trama.get("tipo_transaccion", "")).strip().lower()
        print(f"[Socket] Buscando enrutamiento para: '{tipo_transaccion}'")

        # =================================================================
        # ENRUTADOR SEGURO DE HISTORIAS DE USUARIO
        # =================================================================
        if tipo_transaccion in ["solicitud", "solicitud_llamada"]:
            # HU1: Validaciones MySQL + Tarifa/Saldo inicial consultado a Java
            respuesta = procesar_autorizacion_llamada(trama)
            
        elif tipo_transaccion in ["llamada", "iniciar_llamada"]:
            # HU2: Registrar la llamada autorizada en la lista de memoria activa
            respuesta = procesar_llamada_pendiente(trama)
            
        elif tipo_transaccion in ["finalizacion", "terminar_llamada", "finalizar_llamada"]:
            # HU3: Sacar de activas, calcular costo final y enviar cobro a Java
            respuesta = procesar_finalizacion_pendiente(trama)
            
        elif tipo_transaccion in ["saldo", "consulta_saldo"]:
            # HU4: Consulta rápida de saldo de tarjeta
            respuesta = procesar_saldo_pendiente(trama)

        else:
            print(f"[Socket] Transacción desconocida devuelta: {tipo_transaccion}")
            # Criterio de aceptación: Motivo 4 = Acción inválida / Estructura incorrecta
            respuesta = {"status": "ERROR", "motivo": 4, "message": "Acción inválida"}

    except json.JSONDecodeError:
        print(f"[Socket] Trama JSON malformada enviada por {direccion_cliente}")
        # Criterio de aceptación: Motivo 4 = Estructura de trama inválida
        respuesta = {"status": "ERROR", "motivo": 4, "message": "Formato JSON inválido"}
        
    except Exception as e:
        print(f"[Socket] Error no controlado en el hilo de {direccion_cliente}: {e}")
        # Criterio de aceptación: Motivo 5 = Error no controlado del sistema
        respuesta = {"status": "ERROR", "motivo": 5, "message": f"Error interno: {str(e)}"}
        
    finally:
        # Si logramos procesar u obtener un error controlado, respondemos por el socket
        if respuesta:
            # ─── HU5: REGISTRAR TRAMA DE SALIDA EN COLA ───
            enviar_a_bitacora(respuesta)
            
            # Enviar JSON final codificado en bytes al cliente
            conexion_cliente.sendall(json.dumps(respuesta, ensure_ascii=False).encode('utf-8'))
            
        conexion_cliente.close()
        print(f"[Socket] Conexión con {direccion_cliente} cerrada correctamente.\n")