import socket
import json
from app.config.config import settings

servidor = socket.socket()
#defino puerto y host,
host = settings.SOCKET_HOST
port = settings.SOCKET_PORT


def iniciar_servidor():
    try:
        #asigno al servidor el host y puerto
        servidor.bind((host,port))
        #de momento que ponga en cola solo 5, esto puede cambiar
        servidor.listen(5)
        #prueba para ver si esta conectado correctamente
        print(f'servidor activo y escuchado en host: {host}, puerto: {port}')
        
        #mantiene el servidor vivo
        while True:
            #asigno el puerto y direccion IP del cliente a una variable
            conection_cliente, address_cliente = servidor.accept()
            #prueba de conexion
            print(f'Conexion de cliente entrando desde: {address_cliente}')
            
            #el numero de bytes puede cambiar dependiendo del tamanio del mensaje
            bytes_aceptados = conection_cliente.recv(1024)
            
            archivo_recibido = bytes_aceptados.decode('utf-8')
            print(f'Mensaje recibido: {archivo_recibido}')
            
            try:
                #cargo en el json los archivos recibidos // aqui faltaria la logica y metodos para decidir que hacer con el json
                datos_json = json.loads (archivo_recibido)
                print(f"JSON recibido con éxito: {datos_json}")

                #respuesta prueba
                respuesta = {}
                conection_cliente.sendall(json.dumps(respuesta).encode('utf-8'))
            except json.JSONDecodeError:
                        print("Error: Los datos recibidos no son un JSON válido.")
                        respuesta = {"status": "error", "message": "Formato JSON inválido"}
                        conection_cliente.sendall(json.dumps(respuesta).encode('utf-8'))
                        
    except Exception as e:
            print(f"[Socket] Error al interactuar con el cliente: {e}")
    finally:
            # Cerramos la sesión con este cliente para liberar el canal
            conection_cliente.close()
            print(f"[Socket] Conexión con {address_cliente} finalizada.\n")