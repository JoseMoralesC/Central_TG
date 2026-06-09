from app.database.repositorio import (
    listar_telefonos_catalogo,
    insertar_telefono_catalogo,
    actualizar_estado_telefono_catalogo_por_id,
    existe_telefono_catalogo
)
from app.services.proveedor_cliente import enviar_al_proveedor
from app.utils.crypto import desencriptar_aes, encriptar_aes

def procesar_catalogo_telefonos(_: dict) -> dict:
    telefonos = []

    for item in listar_telefonos_catalogo():
        numero = desencriptar_aes(item.get("numero_cifrado", ""))
        sim = desencriptar_aes(item.get("identificador_tarjeta_cifrado", ""))
        imei = desencriptar_aes(item.get("identificador_dispositivo_cifrado", ""))

        if not numero:
            continue

        detalle = enviar_al_proveedor({
            "tipo_transaccion": "CONSULTA_PROVEEDOR",
            "accion": "DETALLE_TELEFONO",
            "telefono_origen": numero
        })

        detalle_tel = detalle.get("telefono", {})
        pais = item.get("pais") or "Costa Rica"
        nacionalidad = "NACIONAL" if pais.lower() == "costa rica" else "EXTRANJERO"

        telefonos.append({
            "id": f"TEL-{item.get('telefono_id')}",
            "numero": numero,
            "cliente": detalle_tel.get("cliente", f"Telefono {numero}"),
            "proveedor": detalle_tel.get("proveedor", item.get("proveedor_nombre", "")),
            "proveedor_codigo": detalle_tel.get("proveedor_codigo", item.get("proveedor_codigo", "")),
            "pais": pais,
            "nacionalidad": nacionalidad,
            "tipo_servicio": detalle_tel.get("tipo_servicio", item.get("tipo_servicio", "PREPAGO")),
            "saldo": detalle_tel.get("saldo", 0),
            "sim": sim or item.get("identificador_tarjeta_cifrado", ""),
            "imei": imei or item.get("identificador_dispositivo_cifrado", ""),
            "activo": bool(item.get("activo")) and bool(item.get("sim_activa", True)) and bool(item.get("dispositivo_activo", True))
        })

    return {
        "tipo_transaccion": "RESPUESTA_CATALOGO_TELEFONOS",
        "resultado": {
            "codigo": "OK",
            "estado": "CONSULTA_EXITOSA",
            "mensaje": "Catalogo consultado correctamente"
        },
        "telefonos": telefonos
    }

def procesar_recarga_saldo(trama: dict) -> dict:
    telefono = str(trama.get("telefono", "")).strip()
    monto = trama.get("monto", 0)

    if not telefono or float(monto) <= 0:
        return _respuesta("RESPUESTA_RECARGA_SALDO", "ERROR", "Telefono y monto positivo son obligatorios")

    respuesta = enviar_al_proveedor({
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "RECARGAR_SALDO",
        "telefono_origen": telefono,
        "monto": monto,
        "moneda": trama.get("moneda", "CRC")
    })

    codigo = respuesta.get("resultado", {}).get("codigo", respuesta.get("status", "ERROR"))
    mensaje = respuesta.get("resultado", {}).get("mensaje", respuesta.get("mensaje", "Respuesta no disponible"))

    return _respuesta("RESPUESTA_RECARGA_SALDO", codigo, mensaje, respuesta.get("telefono", {}))

def procesar_registro_telefono(trama: dict) -> dict:
    telefono = str(trama.get("telefono", "")).strip()
    tipo_servicio = str(trama.get("tipo_servicio", "PREPAGO")).strip().upper()
    proveedor_codigo = str(trama.get("proveedor_codigo", "KOLBI")).strip().upper()
    pais = str(trama.get("pais", "Costa Rica")).strip()
    sim = str(trama.get("sim", "")).strip()
    imei = str(trama.get("imei", "")).strip()
    saldo_inicial = trama.get("saldo_inicial", 0)

    if not telefono or not sim or not imei:
        return _respuesta("RESPUESTA_REGISTRO_TELEFONO", "ERROR", "Telefono, SIM e IMEI son obligatorios")

    if existe_telefono_catalogo(telefono):
        return _respuesta("RESPUESTA_REGISTRO_TELEFONO", "ERROR", "El numero ya existe en el catalogo")

    respuesta_java = enviar_al_proveedor({
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "REGISTRAR_TELEFONO",
        "telefono": telefono,
        "tipo_servicio": tipo_servicio,
        "proveedor_codigo": proveedor_codigo,
        "pais": pais,
        "saldo_inicial": saldo_inicial,
        "activo": bool(trama.get("activo", True))
    })

    codigo = respuesta_java.get("resultado", {}).get("codigo", respuesta_java.get("status", "ERROR"))
    if codigo != "OK":
        return _respuesta(
            "RESPUESTA_REGISTRO_TELEFONO",
            codigo,
            respuesta_java.get("resultado", {}).get("mensaje", "No fue posible registrar en proveedor")
        )

    insertado = insertar_telefono_catalogo(
        encriptar_aes(telefono),
        proveedor_codigo,
        tipo_servicio,
        pais,
        encriptar_aes(sim),
        encriptar_aes(imei),
        bool(trama.get("activo", True))
    )

    if not insertado:
        return _respuesta("RESPUESTA_REGISTRO_TELEFONO", "ERROR", "Proveedor registrado, pero fallo MySQL")

    return _respuesta("RESPUESTA_REGISTRO_TELEFONO", "OK", "Telefono registrado correctamente")

def procesar_cambio_estado_telefono(trama: dict) -> dict:
    telefono = str(trama.get("telefono", "")).strip()
    telefono_id_texto = str(trama.get("telefono_id", "")).replace("TEL-", "").strip()
    activo = bool(trama.get("activo", True))

    if not telefono:
        return _respuesta("RESPUESTA_CAMBIO_ESTADO_TELEFONO", "ERROR", "Telefono obligatorio")

    respuesta_java = enviar_al_proveedor({
        "tipo_transaccion": "CONSULTA_PROVEEDOR",
        "accion": "CAMBIAR_ESTADO_TELEFONO",
        "telefono": telefono,
        "activo": activo
    })

    codigo = respuesta_java.get("resultado", {}).get("codigo", respuesta_java.get("status", "ERROR"))
    if codigo != "OK":
        return _respuesta(
            "RESPUESTA_CAMBIO_ESTADO_TELEFONO",
            codigo,
            respuesta_java.get("resultado", {}).get("mensaje", "No fue posible actualizar el proveedor")
        )

    telefono_id = int(telefono_id_texto) if telefono_id_texto.isdigit() else None

    if telefono_id is None:
        for item in listar_telefonos_catalogo():
            numero_catalogo = desencriptar_aes(item.get("numero_cifrado", ""))
            if numero_catalogo == telefono:
                telefono_id = item.get("telefono_id")
                break

    actualizado = (
        actualizar_estado_telefono_catalogo_por_id(int(telefono_id), activo)
        if telefono_id is not None
        else False
    )
    if not actualizado:
        return _respuesta("RESPUESTA_CAMBIO_ESTADO_TELEFONO", "ERROR", "Proveedor actualizado, pero fallo MySQL")

    estado = "activado" if activo else "desactivado"
    return _respuesta("RESPUESTA_CAMBIO_ESTADO_TELEFONO", "OK", f"Telefono {estado} correctamente")

def _respuesta(tipo: str, codigo: str, mensaje: str, datos: dict | None = None) -> dict:
    return {
        "tipo_transaccion": tipo,
        "resultado": {
            "codigo": codigo,
            "estado": "OK" if codigo == "OK" else "ERROR",
            "mensaje": mensaje
        },
        "datos": datos or {}
    }
