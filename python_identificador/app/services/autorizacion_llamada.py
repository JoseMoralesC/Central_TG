# app/services/autorizacion_llamada.py
import socket
import re
from app.utils.crypto import desencriptar_aes
from app.config.config import settings  # Para HOST/PORT del proveedor y credenciales
# Para las consultas a MySQL, puedes importar tu gestor de conexiones habitual, ej:
# from app.utils.db import get_db_connection 

def validar_coordenadas_costa_rica(coordenadas_str: str) -> bool:
    """
    Criterio 2.d: Valida si las coordenadas corresponden a Costa Rica.
    Caja de búsqueda aproximada para Costa Rica:
    Latitud: 8.0° N a 11.2° N | Longitud: -86.0° O a -82.5° O
    """
    try:
        # Ejemplo de formato esperado: "9.9281,-84.0907" o el enviado por el simulador
        # Extraemos los números decimales (soporta negativos)
        numeros = re.findall(r"[-+]?\d*\.\d+|\d+", coordenadas_str)
        if len(numeros) < 2:
            return False
        
        lat = float(numeros[0])
        lon = float(numeros[1])
        
        # Validar rangos geográficos del territorio nacional
        if (8.0 <= lat <= 11.2) and (-86.0 <= lon <= -82.5):
            return True
        return False
    except Exception:
        return False

def consultar_proveedor_saldo(telefono: str, tipo_llamada: str) -> dict:
    """
    Comunicación vía Sockets Síncronos con el Proveedor Telefónico (Java / SQL Server).
    Envía trama de texto plano según HU PROVEEDOR1.
    """
    # Formato trama: TipoTransaccion(1) + Telefono(8) + TipoLlamada(1)
    # Tipo Transacción para llamada = "1"
    trama_plana = f"1{telefono}{tipo_llamada}"
    
    try:
        # Crear socket síncrono TCP
        con_proveedor = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        con_proveedor.settimeout(5.0) # Timeout prudencial
        
        # Conectar al componente de Java
        con_proveedor.connect((settings.SOCKET_HOST, settings.SOCKET_PORT))
        
        # Enviar trama en texto plano
        con_proveedor.sendall(trama_plana.encode('utf-8'))
        
        # Recibir respuesta del proveedor
        respuesta = con_proveedor.recv(1024).decode('utf-8').strip()
        con_proveedor.close()
        
        # Procesar respuestas del Proveedor
        if respuesta.startswith("OK"):
            # El formato de éxito del proveedor típicamente incluye la tarifa(10) y tiempo(6)
            # Ejemplo: "OK9999999999245959" o "OK0000002513001025"
            # Extraemos los últimos 6 dígitos que representan el tiempo HHMMSS
            tiempo_autorizado = respuesta[-6:] 
            return {"status": "OK", "tiempo": tiempo_autorizado}
            
        elif respuesta == "INSUF":
            return {"status": "ERROR", "motivo": "INSUF"}
        else:
            return {"status": "ERROR", "motivo": 5} # Error no controlado / Respuesta inválida
            
    except Exception as e:
        print(f"[-] Error de conexión con el proveedor externo (Java): {e}")
        return {"status": "ERROR", "motivo": 5} # Código 5: Error no controlado

def procesar_autorizacion_llamada(trama_json: dict) -> dict:
    """
    Lógica principal de la HU Identificador1: Recibe JSON, valida contra MySQL,
    consulta saldo al proveedor y dictamina la respuesta para el simulador C#.
    """
    # Criterio 2.a: Todos los datos son obligatorios
    campos_requeridos = [
        "telefono", "identificador_telefono", "identificador_tarjeta", 
        "ubicacion_geografica", "tipo_transaccion", "telefono_destino"
    ]
    if not all(k in trama_json and trama_json[k] for k in campos_requeridos):
        return {"status": "ERROR", "motivo": 5} # Error general de estructura

    # 1. Desencriptar campos sensibles de la trama
    telefono_origen = desencriptar_aes(trama_json["telefono"])
    id_telefono = desencriptar_aes(trama_json["identificador_telefono"])
    id_tarjeta = desencriptar_aes(trama_json["identificador_tarjeta"])
    
    ubicacion = trama_json["ubicacion_geografica"]
    tipo_tx = trama_json["tipo_transaccion"]
    telefono_destino = trama_json["telefono_destino"]

    # Criterio 2.e: El tipo de transacción debe ser solamente 'solicitud'
    if tipo_tx.lower() != "solicitud":
        return {"status": "ERROR", "motivo": 4} # 4: Acción inválida

    # Criterio 2.d: Validación de ubicación geográfica (Área nacional)
    if not validar_coordenadas_costa_rica(ubicacion):
        return {"status": "ERROR", "motivo": 3} # 3: Llamada no permitida (fuera del país)

    # 2. Conexión y Validaciones contra la Base de Datos MySQL
    # Nota: Adapta este bloque a tu arquitectura de persistencia (SQLAlchemy, Raw MySQL, etc.)
    try:
        # db = get_db_connection()
        # cursor = db.cursor(dictionary=True)
        
        # Criterio 2.b y 2.c: Validar existencia, estado del teléfono y coincidencia de tarjeta
        # query_origen = "SELECT * FROM telefonos WHERE numero = %s AND activo = 1"
        # cursor.execute(query_origen, (telefono_origen,))
        # registro_origen = cursor.fetchone()
        
        # Simulación de respuesta positiva de BD para el ejemplo estructural:
        registro_origen = {"numero": telefono_origen, "id_tarjeta": id_tarjeta, "id_proveedor": 1} 
        
        if not registro_origen:
            # Si no existe o está inactivo, el negocio usualmente retorna que los datos no coinciden o error controlado
            return {"status": "ERROR", "motivo": 5} 
            
        if registro_origen["id_tarjeta"] != id_tarjeta:
            return {"status": "ERROR", "motivo": 2} # 2: Datos de tarjeta telefónica no coinciden

        # Criterio 2.f: Determinar si la llamada es Nacional o Internacional
        # Suponiendo que si inicia con código país diferente a Costa Rica (ej: +506 o sin prefijo de 8 dígitos) es internacional
        es_internacional = telefono_destino.startswith("+") and not telefono_destino.startswith("+506")
        
        if not es_internacional:
            # Validar teléfono destino nacional en MySQL
            # query_destino = "SELECT * FROM telefonos WHERE numero = %s AND activo = 1"
            # cursor.execute(query_destino, (telefono_destino,))
            # if not cursor.fetchone():
            #     return {"status": "ERROR", "motivo": 1} # 1: Teléfono destino inválido
            tipo_llamada = "1" # Mismo proveedor (o "2" si mapeas otro proveedor en tu BD)
        else:
            # Validar si el código de país internacional es válido (puedes usar un regex o lista en BD)
            # Si es inválido: return {"status": "ERROR", "motivo": 5} (Código de país inválido es 5)
            tipo_llamada = "3" # Fuera del país

    except Exception as db_err:
        print(f"[-] Error en consultas MySQL: {db_err}")
        return {"status": "ERROR", "motivo": 5} # 5: Error no controlado
    # finally:
    #     cursor.close()
    #     db.close()

    # 3. Consultar los fondos/saldo en el Proveedor (Java)
    resultado_proveedor = consultar_proveedor_saldo(telefono_origen, tipo_llamada)
    
    if resultado_proveedor["status"] == "OK":
        # Formato de salida requerido por el simulador: {"status": "OK", "tiempo": "012310"}
        return {
            "status": "OK",
            "tiempo": resultado_proveedor["tiempo"]
        }
    elif resultado_proveedor["motivo"] == "INSUF":
        # Retornamos el estado de saldo insuficiente directamente
        return {"status": "INSUF"}
    else:
        return {"status": "ERROR", "motivo": resultado_proveedor["motivo"]}