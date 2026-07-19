from app.database.repositorio import sincronizar_linea_identificador6
from app.utils.crypto import desencriptar_aes

TIPO_RESPUESTA = "RESPUESTA_IDENTIFICADOR6"
TIPOS_SERVICIO = {"PREPAGO", "POSTPAGO"}
ACCIONES = {"ACTIVAR", "DESACTIVAR"}

def procesar_identificador6(trama: dict) -> dict:
    """
    Procesa la sincronizacion de linea enviada por PROVEEDOR5.
    Devuelve OK o Activacion fallida segun el criterio oficial.
    """
    normalizada = _normalizar_trama(trama)
    error = _validar_trama(normalizada)

    if error:
        return _respuesta_fallida(error)

    activo = normalizada["accion"] == "ACTIVAR"
    guardado = sincronizar_linea_identificador6(
        normalizada["telefono"],
        normalizada["identificador_dispositivo"],
        normalizada["identificador_tarjeta"],
        normalizada["tipo_servicio"],
        normalizada["identificacion_cliente"],
        normalizada["proveedor_codigo"],
        activo
    )

    if not guardado:
        return _respuesta_fallida("Activacion fallida")

    return {
        "tipo_transaccion": TIPO_RESPUESTA,
        "resultado": {
            "codigo": "OK",
            "estado": "LINEA_SINCRONIZADA",
            "mensaje": "OK"
        }
    }

def _normalizar_trama(trama: dict) -> dict:
    return {
        "tipo_transaccion": str(trama.get("tipo_transaccion", "")).strip().upper(),
        "telefono": str(trama.get("telefono", "")).strip(),
        "identificador_dispositivo": str(
            trama.get("identificador_dispositivo", "")
        ).strip(),
        "identificador_tarjeta": str(
            trama.get("identificador_tarjeta", "")
        ).strip(),
        "tipo_servicio": str(trama.get("tipo_servicio", "")).strip().upper(),
        "identificacion_cliente": str(
            trama.get(
                "identificacion_cliente",
                trama.get("identificacion_dueno", "")
            )
        ).strip(),
        "proveedor_codigo": str(
            trama.get("proveedor_codigo", "XYZ")
        ).strip().upper(),
        "accion": str(trama.get("accion", "")).strip().upper(),
        "fecha_hora": str(trama.get("fecha_hora", "")).strip()
    }

def _validar_trama(trama: dict) -> str | None:
    campos_obligatorios = [
        "tipo_transaccion",
        "telefono",
        "identificador_dispositivo",
        "identificador_tarjeta",
        "tipo_servicio",
        "identificacion_cliente",
        "proveedor_codigo",
        "accion"
    ]

    if any(not trama[campo] for campo in campos_obligatorios):
        return "Activacion fallida"

    if trama["tipo_transaccion"] not in {"IDENTIFICADOR6", "PROVEEDOR5"}:
        return "Activacion fallida"

    if trama["tipo_servicio"] not in TIPOS_SERVICIO:
        return "Activacion fallida"

    if trama["accion"] not in ACCIONES:
        return "Activacion fallida"

    if not _datos_cifrados_validos(trama):
        return "Activacion fallida"

    return None

def _datos_cifrados_validos(trama: dict) -> bool:
    telefono = desencriptar_aes(trama["telefono"])
    identificador_dispositivo = desencriptar_aes(
        trama["identificador_dispositivo"]
    )
    identificador_tarjeta = desencriptar_aes(trama["identificador_tarjeta"])
    identificacion_cliente = desencriptar_aes(
        trama["identificacion_cliente"]
    )

    if not (
        telefono and
        identificador_dispositivo and
        identificador_tarjeta and
        identificacion_cliente
    ):
        return False

    return (
        telefono.isdigit() and
        identificador_dispositivo.isdigit() and
        len(identificador_dispositivo) == 16 and
        identificador_tarjeta.isdigit() and
        len(identificador_tarjeta) == 19 and
        identificacion_cliente.strip() != ""
    )

def _respuesta_fallida(mensaje: str) -> dict:
    return {
        "tipo_transaccion": TIPO_RESPUESTA,
        "resultado": {
            "codigo": "ERROR",
            "estado": "ACTIVACION_FALLIDA",
            "mensaje": mensaje or "Activacion fallida"
        }
    }
