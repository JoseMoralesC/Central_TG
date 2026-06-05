# main_test.py
import socket
import threading
import json
import sys
import os

# Asegurar que Python reconozca la estructura de la carpeta 'app'
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from app.services.autorizacion_llamada import procesar_autorizacion_llamada

HOST = '127.0.0.1'
PORT = 5000

def manejar_cliente(conexion_cliente, direccion):
    """Maneja la conexión de cada cliente en un hilo independiente."""
    print(f"📡 [Servidor] Nueva conexión entrante desde {direccion}")
    try:
        # Recibir la trama del socket (búfer de hasta 4096 bytes)
        datos = conexion_cliente.recv(4096)
        if not datos:
            return

        # Decodificar el texto plano recibido
        trama_texto = datos.decode('utf-8')
        print(f"📥 [Servidor] Trama JSON recibida:\n{trama_texto}")
        
        # Convertir a diccionario de Python
        trama_json = json.loads(trama_texto)

        # Ejecutar la validación transaccional en la Base de Datos
        respuesta = procesar_autorizacion_llamada(trama_json)

        # Serializar y enviar la respuesta oficial obligatoria
        respuesta_texto = json.dumps(respuesta)
        print(f"📤 [Servidor] Enviando respuesta a cliente: {respuesta_texto}")
        conexion_cliente.sendall(respuesta_texto.encode('utf-8'))

    except json.JSONDecodeError:
        print("❌ [Servidor] Error: La trama recibida no tiene un formato JSON válido.")
        error_resp = {"codigo": "ERROR", "estado": "ERROR"}
        conexion_cliente.sendall(json.dumps(error_resp).encode('utf-8'))
    except Exception as e:
        print(f"❌ [Servidor] Error crítico al procesar la solicitud: {e}")
        error_resp = {"codigo": "ERROR", "estado": "ERROR"}
        conexion_cliente.sendall(json.dumps(error_resp).encode('utf-8'))
    finally:
        conexion_cliente.close()
        print(f"🛑 [Servidor] Conexión cerrada con {direccion}\n" + "-" * 50)

def iniciar_servidor():
    """Inicializa el socket principal y escucha en modo multi-hilo."""
    servidor = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    
    # SO_REUSEADDR evita el bloqueo del puerto si reinicias el servidor rápido
    servidor.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    
    try:
        servidor.bind((HOST, PORT))
        servidor.listen()
        print("=" * 60)
        print(f"🚀 [Socket Servidor] Escuchando activamente en {HOST}:{PORT}")
        print("   Esperando solicitudes de autorización (HU1)...")
        print("=" * 60)

        while True:
            # Esperar conexiones entrantes
            conexion_cliente, direccion = servidor.accept()
            
            # Crear y lanzar el hilo asíncrono para no bloquear a otros usuarios
            hilo = threading.Thread(target=manejar_cliente, args=(conexion_cliente, direccion))
            hilo.daemon = True
            hilo.start()

    except KeyboardInterrupt:
        print("\n🛑 [Servidor] Apagando el servidor de sockets de forma ordenada...")
    except Exception as e:
        print(f"❌ [Servidor] Error fatal en el ciclo del socket: {e}")
    finally:
        servidor.close()

if __name__ == "__main__":
    iniciar_servidor()