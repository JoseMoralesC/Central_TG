# main_test.py
import sys
import os

# Aseguramos que Python encuentre la carpeta 'app' desde la raíz
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from app.sockets.servidor import iniciar_servidor

def ejecutar_prueba_socket():
    print("🧪 [MODO PRUEBA] Inicializando entorno de red...")
    print("=========================================================")
    print("🚀 Levantando únicamente el servidor Socket para pruebas.")
    print("=========================================================")
    
    try:
        # Arrancamos el socket server directo de tu carpeta app/sockets/
        iniciar_servidor()
    except KeyboardInterrupt:
        print("\n🛑 Prueba finalizada por el usuario (Ctrl + C).")

if __name__ == "__main__":
    ejecutar_prueba_socket()