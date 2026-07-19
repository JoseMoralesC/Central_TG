"""
PROVEEDOR4 - Registro de líneas telefónicas disponibles.
Rol: Dueño del sistema
Objetivo: Colocar a disposición nuevas líneas telefónicas

Recibe trama JSON desde WS_PROVEEDOR1 (WCF), valida, cifra con AES y almacena en MySQL.

Contrato de entrada (trama JSON):
{
  "tipo_transaccion": "REGISTRAR_LINEA",
  "telefono": "88889999",
  "identificador_dispositivo": "1234567891234567",    // 16 dígitos
  "identificador_tarjeta": "1234567891234567891",     // 19 dígitos
  "tipo": "PREPAGO",                                   // PREPAGO | POSTPAGO
  "estado": "disponible"
}

Contrato de respuesta (JSON):
{
  "tipo_transaccion": "RESPUESTA_REGISTRO_LINEA",
  "resultado": {
    "codigo": "OK",
    "mensaje": "OK"
  }
}

Códigos de respuesta:
  "OK"                → Registro exitoso
  "DATOS_INCOMPLETOS" → Faltan campos obligatorios
  "TEL_DUPLICADO"     → El número ya existe en BD
  "ERROR"             → Cualquier otro error
"""

from app.database.repositorio import (
    insertar_telefono_catalogo,
    existe_telefono_catalogo
)
from app.utils.crypto import encriptar_aes


def _respuesta(codigo: str, mensaje: str) -> dict:
    """
    Construye la respuesta JSON siguiendo el formato estándar del proyecto.
    """
    return {
        "tipo_transaccion": "RESPUESTA_REGISTRO_LINEA",
        "resultado": {
            "codigo": codigo,
            "mensaje": mensaje
        }
    }


def validar_campos(trama: dict) -> tuple[bool, str]:
    """
    Valida que todos los campos requeridos estén presentes y sean válidos.
    Retorna (True, "") si ok, o (False, "mensaje error") si falla.
    """
    campos_requeridos = [
        "telefono", "identificador_dispositivo",
        "identificador_tarjeta", "tipo", "estado"
    ]

    for campo in campos_requeridos:
        valor = trama.get(campo)
        if not valor or not str(valor).strip():
            return False, f"Campo '{campo}' es obligatorio"

    # Validar que tipo sea PREPAGO o POSTPAGO
    tipo = str(trama["tipo"]).strip().upper()
    if tipo not in ("PREPAGO", "POSTPAGO"):
        return False, "Tipo debe ser PREPAGO o POSTPAGO"

    return True, ""


def procesar_registro_linea(trama: dict) -> dict:
    """
    Procesa una solicitud REGISTRAR_LINEA del WS_PROVEEDOR1.

    Flujo:
    1. Validar campos obligatorios
    2. Verificar que el teléfono no exista en BD
    3. Cifrar datos sensibles con AES
    4. Insertar en MySQL (telefonos, tarjetas_telefonicas, dispositivos)
    5. Retornar respuesta según resultado
    """
    # 1. Validar campos
    valido, mensaje = validar_campos(trama)
    if not valido:
        return _respuesta("DATOS_INCOMPLETOS", "Datos Incompletos")

    telefono = str(trama["telefono"]).strip()
    id_dispositivo = str(trama["identificador_dispositivo"]).strip()
    id_tarjeta = str(trama["identificador_tarjeta"]).strip()
    tipo = str(trama["tipo"]).strip().upper()

    # 2. Verificar duplicados (descifrando los números en MySQL)
    try:
        if existe_telefono_catalogo(telefono):
            return _respuesta("TEL_DUPLICADO", "Teléfono en uso")
    except Exception as e:
        print(f"[PROVEEDOR4] Error verificando duplicado: {e}")
        return _respuesta("ERROR", "ERROR")

    # 3. Cifrar con AES
    try:
        telefono_cifrado = encriptar_aes(telefono)
        sim_cifrada = encriptar_aes(id_tarjeta)
        imei_cifrado = encriptar_aes(id_dispositivo)

        if not telefono_cifrado or not sim_cifrada or not imei_cifrado:
            return _respuesta("ERROR", "ERROR")

    except Exception as e:
        print(f"[PROVEEDOR4] Error en cifrado AES: {e}")
        return _respuesta("ERROR", "ERROR")

    # 4. Insertar en MySQL
    try:
        ok = insertar_telefono_catalogo(
            numero_cifrado=telefono_cifrado,
            proveedor_codigo="SISTEMA",
            tipo_servicio=tipo,
            pais="Costa Rica",
            sim_cifrado=sim_cifrada,
            imei_cifrado=imei_cifrado,
            activo=True
        )

        if ok:
            return _respuesta("OK", "OK")
        else:
            return _respuesta("ERROR", "ERROR")

    except Exception as e:
        print(f"[PROVEEDOR4] Error insertando en MySQL: {e}")
        return _respuesta("ERROR", "ERROR")