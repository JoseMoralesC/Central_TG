import mysql.connector
from mysql.connector import Error
from app.config.config import settings  # Importamos tu configuración mapeada

def obtener_conexion():
    """
    Creamos la conexion a base de datos, con los datos provenientes den config.
    """
    try:
        # Abrimos el canal usando los datos que config.py extrajo de tu .env
        conexion = mysql.connector.connect(
            host=settings.DB_HOST,
            user=settings.DB_USER,
            password=settings.DB_PASSWORD,
            database=settings.DB_NAME,
            port=settings.DB_PORT
        )
        
        # Verificar si la conexion esta activa
        if conexion.is_connected():
            print("[Database] Conexión exitosa a la base de datos MySQL.")
            return conexion

    except Error as e:
        print(f"[Database] Error al intentar conectar a MySQL: {e}")
        return None

def cerrar_conexion(conexion):
    """
    Cerramos la conexion de forma segura, para que no quede activa
    """
    if conexion and conexion.is_connected():
        conexion.close()
        print("[Database] Conexion a MySQL cerrada correctamente.")