import os
import sys

sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from app.sockets.servidor import iniciar_servidor_socket


if __name__ == "__main__":
    print("[Identificador] Iniciando servidor central...")

    try:
        iniciar_servidor_socket()
    except KeyboardInterrupt:
        print("\n[Identificador] Servidor detenido por el usuario.")
