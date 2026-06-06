# test_suite_cliente.py
import socket
import json
import time
import sys
import os

# Permitir la importación de módulos del proyecto (Base de datos y Criptografía)
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from app.database.conection import obtener_conexion, cerrar_conexion
from app.utils.crypto import desencriptar_aes  # Usamos desencriptación pasiva

HOST = '127.0.0.1'
PORT = 5000

def obtener_datos_reales_bd():
    """Consulta la BD de forma pasiva para extraer componentes válidos en texto plano."""
    print("🔍 [Cliente] Extrayendo credenciales reales y activas de MySQL...")
    conexion = obtener_conexion()
    if not conexion:
        print("❌ No se pudo conectar a la BD para extraer datos.")
        return None

    datos = {}
    try:
        cursor = conexion.cursor(dictionary=True)

        # 1. Buscar un teléfono origen activo que tenga SIM y dispositivo asociados
        query_origen = """
            SELECT t.telefono_id, t.numero_cifrado, ts.identificador_tarjeta_cifrado, d.identificador_dispositivo_cifrado
            FROM telefonos t
            JOIN tarjetas_telefonicas ts ON t.telefono_id = ts.telefono_id
            JOIN dispositivos d ON t.telefono_id = d.telefono_id
            WHERE t.activo = 1 AND ts.activa = 1 AND d.activo = 1
            LIMIT 1;
        """
        cursor.execute(query_origen)
        res_origen = cursor.fetchone()

        if not res_origen:
            print("⚠️ No se encontró ningún teléfono completamente configurado y activo en la BD.")
            return None

        # 2. Buscar un teléfono destino diferente y activo (Para el éxito nacional)
        query_destino = """
            SELECT numero_cifrado FROM telefonos 
            WHERE activo = 1 AND telefono_id != %s 
            LIMIT 1;
        """
        cursor.execute(query_destino, (res_origen["telefono_id"],))
        res_destino = cursor.fetchone()

        if not res_destino:
            print("⚠️ No se encontró un segundo teléfono activo para usar como destino.")
            return None

        # 3. Desencriptar pasivamente para obtener los textos planos originales
        datos["tel_origen"] = desencriptar_aes(res_origen["numero_cifrado"])
        datos["tarjeta"] = desencriptar_aes(res_origen["identificador_tarjeta_cifrado"])
        datos["dispositivo"] = desencriptar_aes(res_origen["identificador_dispositivo_cifrado"])
        datos["tel_destino"] = desencriptar_aes(res_destino["numero_cifrado"])

        print("✅ Datos recuperados y descifrados con éxito desde tu entorno.")
        return datos

    except Exception as e:
        print(f"❌ Error al consultar la base de datos: {e}")
        return None
    finally:
        cursor.close()
        cerrar_conexion(conexion)

def enviar_trama_socket(nombre_caso, payload):
    """Establece conexión por socket, envía el JSON y despliega la respuesta."""
    print(f"==================================================")
    print(f"▶️ Ejecutando Caso: {nombre_caso}")
    print(f"==================================================")
    
    try:
        cliente = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        cliente.connect((HOST, PORT))
        
        trama_json = json.dumps(payload)
        cliente.sendall(trama_json.encode('utf-8'))
        
        bytes_recibidos = cliente.recv(4096)
        cliente.close()
        
        if bytes_recibidos:
            respuesta = json.loads(bytes_recibidos.decode('utf-8'))
            print("📥 [Respuesta del Servidor]:")
            print(json.dumps(respuesta, indent=4, ensure_ascii=False))
        else:
            print("⚠️ El servidor cerró la conexión sin retornar datos.")
            
    except ConnectionRefusedError:
        print("❌ Error: Servidor apagado (main_test.py).")
    except Exception as e:
        print(f"❌ Error inesperado: {e}")
    
    print("-" * 50 + "\n")
    time.sleep(0.4)

def ejecutar_suite_automatica():
    # EJECUTAR ETAPA 1: Extracción pasiva
    datos_bd = obtener_datos_reales_bd()
    if not datos_bd:
        print("🛑 Abortando suite. Es necesario que existan registros válidos en la BD.")
        return

    print("\n" + "=" * 60)
    print("🧪 SUITE CLIENTE: PROBANDO CON DATOS REALES DE TU BD")
    print("=" * 60 + "\n")

    # Valores dinámicos recuperados de la BD
    tel_origen = datos_bd["tel_origen"]
    tel_destino = datos_bd["tel_destino"]
    dispositivo = datos_bd["dispositivo"]
    tarjeta = datos_bd["tarjeta"]

    # -------------------------------------------------------------------------
    # CASO 1: LLAMADA_AUTORIZADA (Caso Exitoso con correspondencia exacta)
    # -------------------------------------------------------------------------
    caso_exitoso = {
        "tipo_transaccion": "SOLICITUD_LLAMADA",
        "telefono_origen": tel_origen,
        "telefono_destino": tel_destino,
        "identificador_dispositivo": dispositivo,
        "identificador_tarjeta": tarjeta,
        "ubicacion": {"pais": "Costa Rica", "provincia": "San Jose", "latitud": 9.9333, "longitud": -84.0833},
        "tipo_llamada": "NACIONAL",
        "fecha_hora": "2026-06-05T10:35:00"
    }
    enviar_trama_socket("LLAMADA_AUTORIZADA (Éxito con datos de la BD)", caso_exitoso)

    # -------------------------------------------------------------------------
    # CASO 2: UBICACION_INVALIDA (Mismos datos correctos pero desde Panamá)
    # -------------------------------------------------------------------------
    caso_ubicacion = caso_exitoso.copy()
    caso_ubicacion["ubicacion"] = {
        "pais": "Panama",
        "provincia": "Ciudad de Panama",
        "latitud": 8.9824, 
        "longitud": -79.5199
    }
    enviar_trama_socket("UBICACION_INVALIDA", caso_ubicacion)

    # -------------------------------------------------------------------------
    # CASO 3: TELEFONO_INACTIVO / INEXISTENTE
    # -------------------------------------------------------------------------
    caso_inactivo = caso_exitoso.copy()
    caso_inactivo["telefono_origen"] = "00000000"  # Forzar número inexistente
    enviar_trama_socket("TELEFONO_INACTIVO", caso_inactivo)

    # -------------------------------------------------------------------------
    # CASO 4: SIM_INVALIDA (Cambiamos el ID de la tarjeta para romper el amarre)
    # -------------------------------------------------------------------------
    caso_sim = caso_exitoso.copy()
    caso_sim["identificador_tarjeta"] = "9999999999999999999"  # SIM errónea
    enviar_trama_socket("SIM_INVALIDA", caso_sim)

    # -------------------------------------------------------------------------
    # CASO 5: SALDO (Formato fijo de 19 dígitos)
    # -------------------------------------------------------------------------
    caso_saldo = caso_exitoso.copy()
    caso_saldo["tipo_transaccion"] = "SALDO"
    enviar_trama_socket("CONSULTA DE SALDO (19 Dígitos)", caso_saldo)

if __name__ == "__main__":
    ejecutar_suite_automatica()