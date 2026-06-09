# app/services/proveedor_cliente.py
import socket
import json
from app.config.config import settings

def _respuesta_error(mensaje: str) -> dict:
    return {
        "tipo_transaccion": "RESPUESTA_PROVEEDOR",
        "status": "ERROR",
        "estado": "ERROR",
        "mensaje": mensaje,
        "resultado": {
            "codigo": "ERROR",
            "estado": "ERROR",
            "mensaje": mensaje
        }
    }

def enviar_al_proveedor(trama_json: dict) -> dict:
    """
    Envía una trama JSON al Proveedor Java y espera su respuesta JSON.
    
    Args:
        trama_json: Diccionario con la trama a enviar (según contracts)
    
    Returns:
        Diccionario con la respuesta del proveedor, o un dict de error si falla
    """
    try:
        con_proveedor = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        con_proveedor.settimeout(5.0)
        
        con_proveedor.connect((settings.PROVEEDOR_HOST, settings.PROVEEDOR_PORT))
        
        # Enviar JSON con salto de línea (\n) como requiere el protocolo
        trama_str = json.dumps(trama_json) + "\n"
        con_proveedor.sendall(trama_str.encode('utf-8'))
        
        # Recibir respuesta JSON
        respuesta = b""
        while True:
            chunk = con_proveedor.recv(4096)
            if not chunk:
                break
            respuesta += chunk
            if b"\n" in chunk:
                break
        
        con_proveedor.close()
        
        respuesta_str = respuesta.decode('utf-8').strip()
        if not respuesta_str:
            return _respuesta_error("Respuesta vacia del proveedor")
        
        return json.loads(respuesta_str)
        
    except socket.timeout:
        print("[-] Timeout conectando con el proveedor Java")
        return _respuesta_error("Timeout de conexion con el proveedor")
    except json.JSONDecodeError:
        print(f"[-] Respuesta JSON inválida del proveedor: {respuesta_str}")
        return _respuesta_error("Respuesta invalida del proveedor")
    except ConnectionRefusedError:
        print("[-] Conexión rechazada por el proveedor Java")
        return _respuesta_error("Proveedor no disponible")
    except Exception as e:
        print(f"[-] Error de conexión con el proveedor: {e}")
        return _respuesta_error(f"Error de conexion: {str(e)}")
