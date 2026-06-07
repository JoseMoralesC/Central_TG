# app/main.py
import sys
import os

# Asegurar que el path incluya la raíz para los imports
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from app.sockets.servidor import iniciar_servidor_socket

if __name__ == "__main__":
    print("[🚀] Iniciando Servidor Central (Sistema Identificador)...")
    try:
        iniciar_servidor_socket()
    except KeyboardInterrupt:
        print("\n[🛑] Servidor detenido por el usuario.")