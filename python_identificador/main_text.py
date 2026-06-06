# app/main.py
import socket
import threading
import json
from app.config.config import settings
from app.services.autorizacion_llamada import procesar_autorizacion_llamada


# app/main.py
import sys
import os

# Asegura que Python pueda encontrar la carpeta 'app' desde cualquier directorio
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from app.sockets.servidor import iniciar_servidor_socket

if __name__ == "__main__":
    print("[🚀] Iniciando el ecosistema del Sistema Identificador...")
    iniciar_servidor_socket()


def manejar_cliente(socket_cliente, direccion_cliente):
    """
    Atiende la conexión individual de un cliente en un hilo independiente.
    """
    print(f"[+] Nueva conexión establecida desde {direccion_cliente}")
    try:
        # Buffer de lectura para tramas JSON
        datos_recibidos = socket_cliente.recv(4096).decode('utf-8')
        if not datos_recibidos:
            return

        print(f"[*] Trama cruda recibida en el Socket: {datos_recibidos}")
        
        # 1. Parsear el JSON entrante
        trama_json = json.loads(datos_recibidos)
        
        # 2. Procesar la solicitud con las validaciones de negocio y proveedor
        respuesta_dict = procesar_autorizacion_llamada(trama_json)
        
        # 3. Serializar y enviar respuesta de vuelta al cliente
        trama_respuesta = json.dumps(respuesta_dict)
        print(f"[*] Enviando respuesta al cliente: {trama_respuesta}")
        socket_cliente.sendall(trama_respuesta.encode('utf-8'))
        
    except json.JSONDecodeError:
        print("[-] Error: La trama recibida no tiene un formato JSON válido.")
        error_resp = {"status": "ERROR", "motivo": 5}
        socket_cliente.sendall(json.dumps(error_resp).encode('utf-8'))
    except Exception as e:
        print(f"[-] Error gestionando la petición del cliente: {e}")
        error_resp = {"status": "ERROR", "motivo": 5}
        socket_cliente.sendall(json.dumps(error_resp).encode('utf-8'))
    finally:
        socket_cliente.close()
        print(f"[-] Conexión cerrada con {direccion_cliente}\n")

def iniciar_servidor():
    """
    Inicializa el socket del Servidor Identificador.
    """
    # Configuración por defecto si no están mapeados en settings
    host = getattr(settings, "IDENTIFICADOR_HOST", "127.0.0.1")
    port = getattr(settings, "IDENTIFICADOR_PORT", 5000)

    servidor = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    # Permite reutilizar el puerto inmediatamente si se reinicia el script
    servidor.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    
    try:
        servidor.bind((host, port))
        servidor.listen(10)  # Cola de espera para conexiones
        print(f"=====================================================")
        print(f"[🚀] Servidor Identificador iniciado en {host}:{port}")
        print(f"=====================================================")
        
        while True:
            # Bloquea el hilo principal esperando un cliente
            socket_cliente, direccion_cliente = servidor.accept()
            
            # Crear y arrancar un hilo exclusivo para atender al cliente de forma síncrona
            hilo = threading.Thread(
                target=manejar_cliente, 
                args=(socket_cliente, direccion_cliente)
            )
            hilo.daemon = True # El hilo muere si el proceso principal se detiene
            hilo.start()
            
    except KeyboardInterrupt:
        print("\n[🛑] Servidor detenido manualmente por el usuario.")
    except Exception as e:
        print(f"[-] Error crítico en el servidor: {e}")
    finally:
        servidor.close()

if __name__ == "__main__":
    iniciar_servidor()