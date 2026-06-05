# app/utils/crypto.py
import base64
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
from app.config.config import settings  

def encriptar_aes(texto_plano: str) -> str:
    if not texto_plano:
        return ""
    try:
        # Usamos settings.AES_KEY y settings.AES_IV mapeados desde el .env
        cipher = AES.new(settings.AES_KEY, AES.MODE_CBC, settings.AES_IV)
        datos_padded = pad(texto_plano.encode('utf-8'), AES.block_size)
        bytes_encriptados = cipher.encrypt(datos_padded)
        return base64.b64encode(bytes_encriptados).decode('utf-8')
    except Exception as e:
        print(f"Error al encriptar con AES: {e}")
        return ""

def desencriptar_aes(texto_encriptado_b64: str) -> str:
    if not texto_encriptado_b64:
        return ""
    try:
        bytes_encriptados = base64.b64decode(texto_encriptado_b64.encode('utf-8'))
        # Usamos settings.AES_KEY y settings.AES_IV mapeados desde el .env
        cipher = AES.new(settings.AES_KEY, AES.MODE_CBC, settings.AES_IV)
        datos_decifrados = cipher.decrypt(bytes_encriptados)
        return unpad(datos_decifrados, AES.block_size).decode('utf-8')
    except Exception as e:
        print(f"Error al desencriptar con AES: {e}")
        return ""