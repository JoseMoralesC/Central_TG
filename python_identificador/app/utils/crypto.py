# app/utils/crypto.py
import base64
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
from app.config.config import settings  

def obtener_llaves_bytes():
    """
    Garantiza que la llave y el IV estén en formato bytes y cumplan 
    con el tamaño requerido por AES (16 bytes para AES-128).
    """
    key = settings.AES_KEY
    iv = settings.AES_IV
    
    # Si vienen como string desde el .env, los convertimos a bytes
    if isinstance(key, str):
        key = key.encode('utf-8')
    if isinstance(iv, str):
        iv = iv.encode('utf-8')
        
    return key, iv

def encriptar_aes(texto_plano: str) -> str:
    if not texto_plano:
        return ""
    try:
        key_bytes, iv_bytes = obtener_llaves_bytes()
        
        # Inicializar el cifrador con bytes puros
        cipher = AES.new(key_bytes, AES.MODE_CBC, iv_bytes)
        datos_padded = pad(texto_plano.encode('utf-8'), AES.block_size)
        bytes_encriptados = cipher.encrypt(datos_padded)
        
        return base64.b64encode(bytes_encriptados).decode('utf-8')
    except Exception as e:
        print(f"[-] Error al encriptar con AES: {e}")
        return ""

def desencriptar_aes(texto_encriptado_b64: str) -> str:
    if not texto_encriptado_b64:
        return ""
    try:
        # Decodificar el contenedor Base64 a bytes cifrados
        bytes_encriptados = base64.b64decode(texto_encriptado_b64.encode('utf-8'))
        key_bytes, iv_bytes = obtener_llaves_bytes()
        
        # Inicializar el descifrador
        cipher = AES.new(key_bytes, AES.MODE_CBC, iv_bytes)
        datos_decifrados = cipher.decrypt(bytes_encriptados)
        
        return unpad(datos_decifrados, AES.block_size).decode('utf-8')
    except Exception as e:
        print(f"[-] Error al desencriptar con AES: {e}")
        return ""