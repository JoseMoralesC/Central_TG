import os
from dotenv import load_dotenv

load_dotenv()

class Settings:
    
    # MySQL - Base de datos del Identificador
    DB_HOST: str = os.getenv("MYSQL_HOST","localhost")
    DB_USER: str = os.getenv("MYSQL_USER","root")
    DB_PASSWORD: str = os.getenv("MYSQL_PASSWORD","llave-falsa")
    DB_NAME: str = os.getenv("MYSQL_DATABASE","central")
    DB_PORT: int = int(os.getenv("MYSQL_PORT", 3306))

    # Socket del Identificador (recibe de C#)
    SOCKET_HOST: str = os.getenv("IDENTIFICADOR_HOST", "0.0.0.0")
    SOCKET_PORT: int = int(os.getenv("IDENTIFICADOR_PORT", 5000))
    
    # Socket del Proveedor Java (envía a Java)
    PROVEEDOR_HOST: str = os.getenv("PROVEEDOR_HOST", "127.0.0.1")
    PROVEEDOR_PORT: int = int(os.getenv("PROVEEDOR_PORT", 6000))
    
    # Entorno
    ENV: str = os.getenv("APP_ENV", "development")
    
    # Cifrado AES
    AES_KEY: bytes = os.getenv("AES_KEY", "ClaveSecreta12345").encode('utf-8')
    AES_IV: bytes = os.getenv("AES_IV", "VectorInicio1234").encode('utf-8')
    
settings = Settings()