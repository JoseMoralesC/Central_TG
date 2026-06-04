import os
from dotenv import load_dotenv

load_dotenv()

class Settings:
    
    DB_HOST: str = os.getenv("MYSQL_HOST","localhost")
    DB_USER: str = os.getenv("MYSQL_USER","root")
    DB_PASSWORD: str = os.getenv("MYSQL_PASSWORD","llave-falsa")
    DB_NAME: str = os.getenv("MYSQL_DATABASE","central")

    DB_PORT: int = int(os.getenv("MYSQL_PORT", 3306))


    SOCKET_HOST: str = os.getenv("IDENTIFICADOR_HOST", "0.0.0.0")
    SOCKET_PORT: int = int(os.getenv("IDENTIFICADOR_PORT", 5000))
    
    
    ENV: str = os.getenv("APP_ENV", "development")
settings = Settings()