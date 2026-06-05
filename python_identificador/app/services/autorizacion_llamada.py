# app/services/auth_service.py
from app.database.conection import obtener_conexion, cerrar_conexion
from app.utils.crypto import encriptar_aes

TABLA_TELEFONOS = "telefonos"
TABLA_DISPOSITIVOS = "dispositivos"
TABLA_TARJETAS = "tarjetas_telefonicas"  

def validar_coordenadas_costa_rica(latitud, longitud):
    if latitud is None or longitud is None:
        return False
    try:
        lat = float(latitud)
        lon = float(longitud)
        return (8.0 <= lat <= 11.5) and (-86.0 <= lon <= -82.5)
    except (ValueError, TypeError):
        return False

def procesar_autorizacion_llamada(trama):
    # --- CRITERIO 2.a: Todos los datos son obligatorios ---
    # Nota: Si es consulta de saldo, algunos campos como telefono_destino o tarjetas podrían variar, 
    # pero el criterio 1 dice que la trama debe incorporar toda la información. Validamos la estructura base.
    tipo_transaccion = trama.get("tipo_transaccion")
    
    # --- CRITERIO 2.e: El tipo de transacción solo puede ser SOLICITUD o SALDO (según Criterio 4) ---
    if tipo_transaccion not in ["SOLICITUD_LLAMADA", "SALDO"]:
        return {"status": "ERROR", "motivo": 4, "mensaje": "Acción inválida"}

    ubicacion = trama.get("ubicacion", {})
    latitud = ubicacion.get("latitud")
    longitud = ubicacion.get("longitud")

    # --- CRITERIO 2.d: Validación de Territorio Nacional (Código de motivo: 3) ---
    if not validar_coordenadas_costa_rica(latitud, longitud):
        return {"status": "ERROR", "motivo": 3, "mensaje": "Llamada no permitida (Fuera del área del país)"}

    # Cifrado AES reglamentario para búsquedas de seguridad
    tel_origen_cifrado = encriptar_aes(str(trama.get("telefono_origen")))
    tarjeta_cifrado = encriptar_aes(str(trama.get("identificador_tarjeta")))
    dispositivo_cifrado = encriptar_aes(str(trama.get("identificador_dispositivo")))

    conexion = obtener_conexion()
    if not conexion:
        return {"status": "ERROR", "motivo": 5, "mensaje": "Error no controlado (BD)"}
    
    try:
        cursor = conexion.cursor(dictionary=True)
        
        # --- CRITERIO 2.b: El número de teléfono debe existir, estar activo y tener proveedor ---
        query_tel = f"SELECT * FROM {TABLA_TELEFONOS} WHERE numero_cifrado = %s"
        cursor.execute(query_tel, (tel_origen_cifrado,))
        db_telefono = cursor.fetchone()
        
        if not db_telefono or db_telefono["activo"] != 1 or not db_telefono.get("proveedor_id"):
            # El documento mapea teléfono origen inactivo/inexistente como error no controlado o rechazo directo
            return {"status": "ERROR", "motivo": 5, "mensaje": "Error no controlado (Origen inválido o inactivo)"}

        # --- CRITERIO 2.c: Datos de la tarjeta telefónica deben coincidir (Código de motivo: 2) ---
        query_chip = f"SELECT * FROM {TABLA_TARJETAS} WHERE identificador_tarjeta_cifrado = %s"
        cursor.execute(query_chip, (tarjeta_cifrado,))
        db_chip = cursor.fetchone()
        
        if not db_chip or db_chip["telefono_id"] != db_telefono["telefono_id"] or db_chip["activa"] != 1:
            return {"status": "ERROR", "motivo": 2, "mensaje": "Datos de tarjeta telefónica no coinciden"}

        # Validación del dispositivo asociado
        query_disp = f"SELECT * FROM {TABLA_DISPOSITIVOS} WHERE identificador_dispositivo_cifrado = %s AND activo = 1"
        cursor.execute(query_disp, (dispositivo_cifrado,))
        db_dispositivo = cursor.fetchone()
        
        if not db_dispositivo or db_dispositivo["telefono_id"] != db_telefono["telefono_id"]:
            return {"status": "ERROR", "motivo": 5, "mensaje": "Dispositivo no coincide"}

        # =====================================================================
        # FLUJO B: SI LA TRANSACCIÓN ES DE TIPO SALDO (CRITERIO 4)
        # =====================================================================
        if tipo_transaccion == "SALDO":
            # Simulación de consulta HU PROVEEDOR1 (Ejemplo: devuelve 1234.56 colones)
            saldo_proveedor = 1234.56  
            
            # Formatear a un entero eliminando el punto decimal (1234.56 -> 123456)
            saldo_entero = int(saldo_proveedor * 100)
            # Rellenar con ceros a la izquierda hasta cumplir estrictamente los 19 espacios
            saldo_formateado = f"{saldo_entero:019d}"
            
            return {"status": "OK", "saldo": saldo_formateado}

        # =====================================================================
        # FLUJO A: SI LA TRANSACCIÓN ES DE TIPO LLAMADA (CRITERIO 3)
        # =====================================================================
        elif tipo_transaccion == "SOLICITUD_LLAMADA":
            # --- CRITERIO 2.f: Validación de Teléfono Destino (Nacional o Internacional) ---
            tipo_llamada = trama.get("tipo_llamada")
            tel_destino = str(trama.get("telefono_destino"))
            
            if tipo_llamada == "NACIONAL":
                tel_destino_cifrado = encriptar_aes(tel_destino)
                query_destino = f"SELECT * FROM {TABLA_TELEFONOS} WHERE numero_cifrado = %s AND activo = 1"
                cursor.execute(query_destino, (tel_destino_cifrado,))
                if not cursor.fetchone():
                    return {"status": "ERROR", "motivo": 1, "mensaje": "Teléfono destino inválido (inactivo o que no existe)"}
                    
            elif tipo_llamada == "INTERNACIONAL":
                # Prefijos válidos exigidos por la cátedra
                if not tel_destino.startswith(("506", "1", "34")): 
                    return {"status": "ERROR", "motivo": 5, "mensaje": "Código de país inválido"}
            else:
                return {"status": "ERROR", "motivo": 4, "mensaje": "Acción inválida"}

            # --- CRITERIO 3.a.i: Verificación de saldo con HU PROVEEDOR1 ---
            # Simulación: El proveedor responde que el cliente tiene fondos y calcula el tiempo disponible
            proveedor_permite_llamada = True
            
            if proveedor_permite_llamada:
                # Ejemplo de tiempo asignado: 1 hora, 23 minutos, 10 segundos
                # Formato estricto de 6 dígitos sin puntos exigido: "012310"
                tiempo_maximo_permitido = "012310" 
                return {"status": "OK", "tiempo": tiempo_maximo_permitido}
            else:
                return {"status": "ERROR", "motivo": 5, "mensaje": "Saldo insuficiente para el primer minuto"}
            
    except Exception as e:
        print(f"Error interno: {e}")
        return {"status": "ERROR", "motivo": 5, "mensaje": "Error no controlado"}
        
    finally:
        cursor.close()
        cerrar_conexion(conexion)