# app/sockets/server.py
import socket
import threading
from app.config.config import settings
from app.sockets.handler import manejar_cliente  # <--- Importamos el hilo

def iniciar_servidor_socket():
    """
    Inicializa el socket principal y distribuye las conexiones entrantes
    en hilos independientes de forma asíncrona.
    """
    servidor = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    # Permite reutilizar el puerto inmediatamente si el servidor se reinicia
    servidor.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    
    host = settings.SOCKET_HOST
    port = settings.SOCKET_PORT
    
    try:
        servidor.bind((host, port))
        servidor.listen(100)  # Capacidad de respuesta ante ráfagas concurrentes
        print(f"📡 [Socket Servidor] Escuchando activamente en {host}:{port}")
        
        while True:
            # El hilo principal se queda esperando conexiones de C# aquí
            conexion_cliente, direccion_cliente = servidor.accept()
            print(f"\n⚡ [Socket] Nueva conexión entrante desde: {direccion_cliente}")
            
            # ¡La magia del split! Creamos el hilo llamando al handler asignado
            hilo = threading.Thread(
                target=manejar_cliente, 
                args=(conexion_cliente, direccion_cliente)
            )
            hilo.start()
            
    except Exception as e:
        print(f"❌ [Socket] Error crítico en el bucle principal: {e}")
    finally:
        servidor.close()
        print("🛑 [Socket] Servidor detenido.")