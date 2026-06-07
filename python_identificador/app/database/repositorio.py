# app/database/repositorio.py
"""
Repositorio de datos: consultas reales a MySQL usando la conexión de conection.py.
Reemplaza las simulaciones anteriores.
"""
from typing import Optional
from app.database.conection import obtener_conexion, cerrar_conexion

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
        cursor = conn.cursor(dictionary=True)
        
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
        """
        cursor.execute(query, (numero_cifrado,))
        resultado = cursor.fetchone()
        cursor.close()
        return resultado
    except Exception as e:
        print(f"[DB Error] buscar_telefono_por_numero_cifrado: {e}")
        return None
    finally:
        cerrar_conexion(conn)

def buscar_tarjeta_por_telefono_id(telefono_id: int, identificador_cifrado: str) -> Optional[dict]:
    """
    Verifica que la tarjeta SIM cifrada corresponda al teléfono.
    """
    conn = obtener_conexion()
    if not conn:
        return None
    
    try:
        cursor = conn.cursor(dictionary=True)
        query = """
            SELECT tarjeta_id, activa
            FROM tarjetas_telefonicas
            WHERE telefono_id = %s AND identificador_tarjeta_cifrado = %s
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
        cursor = conn.cursor(dictionary=True)
        query = """
            SELECT dispositivo_id, activo
            FROM dispositivos
            WHERE telefono_id = %s AND identificador_dispositivo_cifrado = %s
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