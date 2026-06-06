# app/sockets/server.py
import socket
import threading
import queue
import time
import json
from app.config.config import settings
from app.sockets import handler  # Importación absoluta del handler vecino

# Cola global "Thread-Safe" para la Bitácora (HU5)
cola_bitacora = queue.Queue()

def trabajador_bitacora():
    """
    HU Identificador 5: Consumidor en segundo plano (hilo demonio).
    Saca las tramas de la cola una por una y las escribe de forma ordenada.
    """
    print("[Bitácora] Hilo de auditoría asíncrona listo y escuchando la cola...")
    while True:
        try:
            evento = cola_bitacora.get()
            if evento is None:  # Señal de parada
                break
            
            # Formato exacto exigido por el alcance: Fecha: JSON
            fecha_actual = time.strftime("%d/%m/%Y")
            linea = f"{fecha_actual}: {json.dumps(evento, ensure_ascii=False)}\n"
            
            # Escritura segura en el archivo de texto plano
            with open("bitacora_identificador.txt", "a", encoding="utf-8") as archivo:
                archivo.write(linea)
                
            cola_bitacora.task_done()
        except Exception as e:
            print(f"[Bitácora] Error crítico escribiendo en archivo: {e}")

def iniciar_servidor_socket():
    """
    Inicializa el socket principal y distribuye las conexiones entrantes
    en hilos independientes de forma asíncrona.
    """
    servidor = socket.socket()
    # Permite reutilizar el puerto de inmediato si el servidor se reinicia
    servidor.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    
    host = settings.SOCKET_HOST
    port = settings.SOCKET_PORT
    
    try:
        servidor.bind((host, port))
        servidor.listen(100)
        print(f"[Socket Servidor] Escuchando activamente en {host}:{port}")
        
        # 1. Iniciar el hilo de la bitácora (HU5) antes de recibir clientes
        hilo_bitacora = threading.Thread(target=trabajador_bitacora, daemon=True)
        hilo_bitacora.start()
        
        # 2. Inyectar la cola dentro del handler para que se puedan comunicar
        handler.inicializar_handler(cola_bitacora)
        
        while True:
            # El hilo principal se bloquea aquí esperando tramas de C#
            conexion_cliente, direccion_cliente = servidor.accept()
            print(f"\n[Socket] Nueva conexión entrante desde: {direccion_cliente}")
            
            # Delegamos la atención del cliente al método manejar_cliente de handler.py
            hilo = threading.Thread(
                target=handler.manejar_cliente, 
                args=(conexion_cliente, direccion_cliente)
            )
            hilo.start()
            
    except Exception as e:
        print(f"[Socket] Error crítico en el bucle principal: {e}")
    finally:
        servidor.close()
        print("[Socket] Servidor detenido.")