# app/database/repositorio.py
"""
Repositorio de datos: consultas reales a MySQL usando la conexión de conection.py.
Reemplaza las simulaciones anteriores.
"""
from typing import Optional
from app.database.conection import obtener_conexion, cerrar_conexion

def listar_telefonos_catalogo() -> list[dict]:
    """
    Lista telefonos con proveedor, SIM e IMEI desde MySQL.
    Los campos sensibles se devuelven cifrados; el servicio decide si los descifra.
    """
    conn = obtener_conexion()
    if not conn:
        return []

    try:
        cursor = conn.cursor(dictionary=True, buffered=True)
        query = """
            SELECT
                t.telefono_id,
                t.numero_cifrado,
                t.proveedor_id,
                t.tipo_servicio,
                t.activo,
                COALESCE(t.pais, 'Costa Rica') AS pais,
                p.nombre AS proveedor_nombre,
                p.codigo AS proveedor_codigo,
                tt.identificador_tarjeta_cifrado,
                tt.activa AS sim_activa,
                d.identificador_dispositivo_cifrado,
                d.activo AS dispositivo_activo
            FROM telefonos t
            JOIN proveedores p ON t.proveedor_id = p.proveedor_id
            LEFT JOIN tarjetas_telefonicas tt ON tt.telefono_id = t.telefono_id
            LEFT JOIN dispositivos d ON d.telefono_id = t.telefono_id
            ORDER BY t.telefono_id
        """
        cursor.execute(query)
        resultado = cursor.fetchall()
        cursor.close()
        return resultado
    except Exception as e:
        print(f"[DB Error] listar_telefonos_catalogo: {e}")
        return []
    finally:
        cerrar_conexion(conn)

def buscar_proveedor_por_codigo(codigo: str) -> Optional[dict]:
    conn = obtener_conexion()
    if not conn:
        return None

    try:
        cursor = conn.cursor(dictionary=True, buffered=True)
        cursor.execute(
            "SELECT proveedor_id, nombre, codigo FROM proveedores WHERE codigo = %s LIMIT 1",
            (codigo,)
        )
        resultado = cursor.fetchone()
        cursor.close()
        return resultado
    except Exception as e:
        print(f"[DB Error] buscar_proveedor_por_codigo: {e}")
        return None
    finally:
        cerrar_conexion(conn)

def insertar_telefono_catalogo(
    numero_cifrado: str,
    proveedor_codigo: str,
    tipo_servicio: str,
    pais: str,
    sim_cifrado: str,
    imei_cifrado: str,
    activo: bool = True
) -> bool:
    conn = obtener_conexion()
    if not conn:
        return False

    try:
        proveedor = buscar_proveedor_por_codigo(proveedor_codigo)
        if not proveedor:
            return False

        cursor = conn.cursor()
        cursor.execute(
            """
            INSERT INTO telefonos (numero_cifrado, proveedor_id, tipo_servicio, activo, pais)
            VALUES (%s, %s, %s, %s, %s)
            """,
            (numero_cifrado, proveedor["proveedor_id"], tipo_servicio, activo, pais)
        )
        telefono_id = cursor.lastrowid
        cursor.execute(
            """
            INSERT INTO tarjetas_telefonicas (telefono_id, identificador_tarjeta_cifrado, activa)
            VALUES (%s, %s, TRUE)
            """,
            (telefono_id, sim_cifrado)
        )
        cursor.execute(
            """
            INSERT INTO dispositivos (telefono_id, identificador_dispositivo_cifrado, activo)
            VALUES (%s, %s, TRUE)
            """,
            (telefono_id, imei_cifrado)
        )
        conn.commit()
        cursor.close()
        return True
    except Exception as e:
        print(f"[DB Error] insertar_telefono_catalogo: {e}")
        try:
            conn.rollback()
        except Exception:
            pass
        return False
    finally:
        cerrar_conexion(conn)

def actualizar_estado_telefono_catalogo(numero_cifrado: str, activo: bool) -> bool:
    conn = obtener_conexion()
    if conn is None:
        return False

    try:
        cursor = conn.cursor()
        cursor.execute(
            "UPDATE telefonos SET activo = %s WHERE numero_cifrado = %s",
            (activo, numero_cifrado)
        )
        actualizado = cursor.rowcount > 0
        conn.commit()
        cursor.close()
        return actualizado
    except Exception as e:
        print(f"[DB Error] actualizar_estado_telefono_catalogo: {e}")
        try:
            conn.rollback()
        except Exception:
            pass
        return False
    finally:
        cerrar_conexion(conn)

def actualizar_estado_telefono_catalogo_por_id(telefono_id: int, activo: bool) -> bool:
    conn = obtener_conexion()
    if conn is None:
        return False

    try:
        cursor = conn.cursor()
        cursor.execute(
            "UPDATE telefonos SET activo = %s WHERE telefono_id = %s",
            (activo, telefono_id)
        )
        cursor.execute(
            "UPDATE tarjetas_telefonicas SET activa = %s WHERE telefono_id = %s",
            (activo, telefono_id)
        )
        cursor.execute(
            "UPDATE dispositivos SET activo = %s WHERE telefono_id = %s",
            (activo, telefono_id)
        )
        conn.commit()
        cursor.close()
        return True
    except Exception as e:
        print(f"[DB Error] actualizar_estado_telefono_catalogo_por_id: {e}")
        try:
            conn.rollback()
        except Exception:
            pass
        return False
    finally:
        cerrar_conexion(conn)

def existe_telefono_catalogo(numero_plano: str) -> bool:
    from app.utils.crypto import desencriptar_aes

    for item in listar_telefonos_catalogo():
        numero_catalogo = desencriptar_aes(item.get("numero_cifrado", ""))
        if _normalizar_numero(numero_catalogo) == _normalizar_numero(numero_plano):
            return True

    return False

def buscar_telefono_por_numero_cifrado(numero_cifrado: str) -> Optional[dict]:
    """
    Busca un teléfono por su número cifrado (AES).
    Retorna dict con datos del teléfono, proveedor, tarjeta y dispositivo,
    o None si no existe.
    """
    conn = obtener_conexion()
    if not conn:
        return None
    
    try:
        cursor = conn.cursor(dictionary=True, buffered=True)
        
        query = """
            SELECT 
                t.telefono_id,
                t.numero_cifrado,
                t.proveedor_id,
                t.tipo_servicio,
                t.activo,
                p.nombre AS proveedor_nombre,
                p.codigo AS proveedor_codigo
            FROM telefonos t
            JOIN proveedores p ON t.proveedor_id = p.proveedor_id
            WHERE t.numero_cifrado = %s
            LIMIT 1
        """
        cursor.execute(query, (numero_cifrado,))
        resultado = cursor.fetchone()
        cursor.close()
        return resultado or buscar_telefono_por_numero_limpio(numero_cifrado)
    except Exception as e:
        print(f"[DB Error] buscar_telefono_por_numero_cifrado: {e}")
        return buscar_telefono_por_numero_limpio(numero_cifrado)
    finally:
        cerrar_conexion(conn)

def buscar_telefono_por_numero_limpio(numero_cifrado_o_plano: str) -> Optional[dict]:
    """
    Resuelve telefonos por numero visible limpio. Esto cubre filas historicas
    que hayan quedado cifradas con codigo de area, sin exigir ese prefijo en UI.
    """
    from app.utils.crypto import desencriptar_aes

    numero_plano = desencriptar_aes(numero_cifrado_o_plano) or numero_cifrado_o_plano
    numero_limpio = _normalizar_numero(numero_plano)

    if not numero_limpio:
        return None

    for item in listar_telefonos_catalogo():
        numero_catalogo = desencriptar_aes(item.get("numero_cifrado", ""))

        if _normalizar_numero(numero_catalogo) == numero_limpio:
            return item

    return None

def _normalizar_numero(numero: str) -> str:
    solo_digitos = "".join(ch for ch in str(numero or "") if ch.isdigit())
    return solo_digitos[-8:] if len(solo_digitos) > 8 else solo_digitos

def buscar_tarjeta_por_telefono_id(telefono_id: int, identificador_cifrado: str) -> Optional[dict]:
    """
    Verifica que la tarjeta SIM cifrada corresponda al teléfono.
    """
    conn = obtener_conexion()
    if not conn:
        return None
    
    try:
        cursor = conn.cursor(dictionary=True, buffered=True)
        query = """
            SELECT tarjeta_id, activa
            FROM tarjetas_telefonicas
            WHERE telefono_id = %s AND identificador_tarjeta_cifrado = %s
            LIMIT 1
        """
        cursor.execute(query, (telefono_id, identificador_cifrado))
        resultado = cursor.fetchone()
        cursor.close()
        return resultado
    except Exception as e:
        print(f"[DB Error] buscar_tarjeta_por_telefono_id: {e}")
        return None
    finally:
        cerrar_conexion(conn)

def buscar_dispositivo_por_telefono_id(telefono_id: int, dispositivo_cifrado: str) -> Optional[dict]:
    """
    Verifica que el dispositivo cifrado corresponda al teléfono.
    """
    conn = obtener_conexion()
    if not conn:
        return None
    
    try:
        cursor = conn.cursor(dictionary=True, buffered=True)
        query = """
            SELECT dispositivo_id, activo
            FROM dispositivos
            WHERE telefono_id = %s AND identificador_dispositivo_cifrado = %s
            LIMIT 1
        """
        cursor.execute(query, (telefono_id, dispositivo_cifrado))
        resultado = cursor.fetchone()
        cursor.close()
        return resultado
    except Exception as e:
        print(f"[DB Error] buscar_dispositivo_por_telefono_id: {e}")
        return None
    finally:
        cerrar_conexion(conn)

def eliminar_llamada_activa_por_telefono_id(telefono_id: int) -> bool:
    """
    Elimina todas las llamadas activas de un teléfono específico.
    Útil al finalizar una llamada para limpiar la BD.
    """
    conn = obtener_conexion()
    if not conn:
        return False
    
    try:
        cursor = conn.cursor()
        query = "DELETE FROM llamadas_activas WHERE telefono_id = %s"
        cursor.execute(query, (telefono_id,))
        conn.commit()
        eliminado = cursor.rowcount > 0
        cursor.close()
        return eliminado
    except Exception as e:
        print(f"[DB Error] eliminar_llamada_activa_por_telefono_id: {e}")
        return False
    finally:
        cerrar_conexion(conn)

def insertar_bitacora(telefono_id: Optional[int], tipo_transaccion: str,
                      tipo_trama: str, contenido_json: str) -> Optional[int]:
    """
    Inserta un registro en la tabla bitacora_identificador.
    """
    conn = obtener_conexion()
    if not conn:
        return None
    
    try:
        cursor = conn.cursor()
        query = """
            INSERT INTO bitacora_identificador
                (telefono_id, tipo_transaccion, tipo_trama, contenido_json, fecha_registro)
            VALUES (%s, %s, %s, %s, NOW())
        """
        cursor.execute(query, (telefono_id, tipo_transaccion, tipo_trama, contenido_json))
        conn.commit()
        id_insertado = cursor.lastrowid
        cursor.close()
        return id_insertado
    except Exception as e:
        print(f"[DB Error] insertar_bitacora: {e}")
        return None
    finally:
        cerrar_conexion(conn)

def insertar_llamada_activa(telefono_id: int, telefono_destino: str,
                            fecha_inicio, fecha_fin_maxima,
                            tiempo_maximo: str, estado: str = "ACTIVA") -> Optional[int]:
    """
    Registra una llamada activa en la BD.
    Retorna el ID insertado o None si falla.
    """
    conn = obtener_conexion()
    if not conn:
        return None
    
    try:
        cursor = conn.cursor()
        query = """
            INSERT INTO llamadas_activas 
                (telefono_id, telefono_destino, fecha_inicio, fecha_fin_maxima, 
                 tiempo_maximo, estado)
            VALUES (%s, %s, %s, %s, %s, %s)
        """
        cursor.execute(query, (telefono_id, telefono_destino, fecha_inicio,
                               fecha_fin_maxima, tiempo_maximo, estado))
        conn.commit()
        id_insertado = cursor.lastrowid
        cursor.close()
        return id_insertado
    except Exception as e:
        print(f"[DB Error] insertar_llamada_activa: {e}")
        return None
    finally:
        cerrar_conexion(conn)

def eliminar_llamada_activa_por_id(llamada_id: int) -> bool:
    """
    Elimina una llamada activa por su ID.
    """
    conn = obtener_conexion()
    if not conn:
        return False
    
    try:
        cursor = conn.cursor()
        query = "DELETE FROM llamadas_activas WHERE llamada_activa_id = %s"
        cursor.execute(query, (llamada_id,))
        conn.commit()
        eliminado = cursor.rowcount > 0
        cursor.close()
        return eliminado
    except Exception as e:
        print(f"[DB Error] eliminar_llamada_activa_por_id: {e}")
        return False
    finally:
        cerrar_conexion(conn)

def insertar_historial_llamada(telefono_id: int, telefono_destino: str,
                                fecha_inicio, fecha_fin,
                                duracion: str, motivo: str) -> Optional[int]:
    """
    Inserta una llamada finalizada en el historial.
    """
    conn = obtener_conexion()
    if not conn:
        return None
    
    try:
        cursor = conn.cursor()
        query = """
            INSERT INTO historial_llamadas_identificador
                (telefono_id, telefono_destino, fecha_inicio, fecha_fin,
                 duracion, motivo_finalizacion)
            VALUES (%s, %s, %s, %s, %s, %s)
        """
        cursor.execute(query, (telefono_id, telefono_destino, fecha_inicio,
                               fecha_fin, duracion, motivo))
        conn.commit()
        id_insertado = cursor.lastrowid
        cursor.close()
        return id_insertado
    except Exception as e:
        print(f"[DB Error] insertar_historial_llamada: {e}")
        return None
    finally:
        cerrar_conexion(conn)
