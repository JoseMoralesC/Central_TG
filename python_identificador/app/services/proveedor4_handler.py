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
  "estado": "inactivo"
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
    existe_telefono_catalogo,
    insertar_linea_proveedor4
)
from app.services.proveedor_cliente import enviar_al_proveedor
from app.utils.crypto import desencriptar_aes, encriptar_aes


def _obtener_valor_plano(valor: str) -> str:
    """
    Devuelve el valor descifrado cuando viene cifrado; si ya viene plano,
    conserva el valor original para soportar pruebas manuales.
    """
    texto = str(valor).strip()
    return desencriptar_aes(texto) or texto


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

    telefono = _obtener_valor_plano(trama["telefono"])
    id_dispositivo = _obtener_valor_plano(trama["identificador_dispositivo"])
    id_tarjeta = _obtener_valor_plano(trama["identificador_tarjeta"])

    if not telefono.isdigit():
        return False, "Telefono debe contener solo digitos"

    if not id_dispositivo.isdigit() or len(id_dispositivo) != 16:
        return False, "Identificador de telefono debe tener 16 digitos"

    if not id_tarjeta.isdigit() or len(id_tarjeta) != 19:
        return False, "Identificador de tarjeta debe tener 19 digitos"

    # Validar que tipo sea PREPAGO o POSTPAGO
    tipo = str(trama["tipo"]).strip().upper()
    if tipo not in ("PREPAGO", "POSTPAGO"):
        return False, "Tipo debe ser PREPAGO o POSTPAGO"

    estado = str(trama["estado"]).strip().lower()
    if estado not in ("activo", "inactivo", "disponible", "false", "0", "no"):
        return False, "Estado no es valido"

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
    estado = str(trama.get("estado", "activo")).strip().lower()
    activo = estado not in ("inactivo", "disponible", "false", "0", "no")
    existe_en_mysql = False

    # 2. Verificar duplicados (descifrando los números en MySQL)
    try:
        telefono_plano = desencriptar_aes(telefono) or telefono
        existe_en_mysql = existe_telefono_catalogo(telefono_plano)
    except Exception as e:
        print(f"[PROVEEDOR4] Error verificando duplicado: {e}")
        return _respuesta("ERROR", "ERROR")

    # 3. Cifrar con AES
    try:
        telefono_cifrado = _asegurar_cifrado(telefono)
        sim_cifrada = _asegurar_cifrado(id_tarjeta)
        imei_cifrado = _asegurar_cifrado(id_dispositivo)

        if not telefono_cifrado or not sim_cifrada or not imei_cifrado:
            return _respuesta("ERROR", "ERROR")

    except Exception as e:
        print(f"[PROVEEDOR4] Error en cifrado AES: {e}")
        return _respuesta("ERROR", "ERROR")

    respuesta_proveedor = _registrar_en_proveedor(
        telefono_plano=telefono_plano,
        tipo=tipo,
        activo=activo
    )
    codigo_proveedor = respuesta_proveedor.get("resultado", {}).get(
        "codigo",
        respuesta_proveedor.get("status", "ERROR")
    )
    mensaje_proveedor = respuesta_proveedor.get("resultado", {}).get(
        "mensaje",
        respuesta_proveedor.get("mensaje", "No fue posible registrar en SQL Server")
    )
    proveedor_duplicado = "existe en SQL Server" in mensaje_proveedor

    if codigo_proveedor != "OK" and not proveedor_duplicado:
        return _respuesta("ERROR", mensaje_proveedor)

    if existe_en_mysql:
        return _respuesta(
            "TEL_DUPLICADO" if proveedor_duplicado else "OK",
            "Telefono en uso" if proveedor_duplicado else "OK"
        )

    # 4. Insertar en MySQL usando el nuevo método específico de PROVEEDOR4
    #    que NO depende de la tabla proveedores
    try:
        ok = insertar_linea_proveedor4(
            numero_cifrado=telefono_cifrado,
            tipo_servicio=tipo,
            pais="Costa Rica",
            sim_cifrado=sim_cifrada,
            imei_cifrado=imei_cifrado,
            activo=activo
        )

        if ok:
            return _respuesta("OK", "OK")
        else:
            return _respuesta("ERROR", "ERROR")

    except Exception as e:
        print(f"[PROVEEDOR4] Error insertando en MySQL: {e}")
        return _respuesta("ERROR", "ERROR")


def _asegurar_cifrado(valor: str) -> str:
    return valor if desencriptar_aes(valor) else encriptar_aes(valor)


def _registrar_en_proveedor(telefono_plano: str, tipo: str, activo: bool) -> dict:
    saldo_inicial = 1000.00 if tipo == "PREPAGO" else 0

    return enviar_al_proveedor({
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "REGISTRAR_TELEFONO",
        "telefono": telefono_plano,
        "tipo_servicio": tipo,
        "proveedor_codigo": "KOLBI",
        "saldo_inicial": saldo_inicial,
        "activo": activo
    })
