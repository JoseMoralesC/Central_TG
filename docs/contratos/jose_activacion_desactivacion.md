# Contrato de Jose - Activacion y desactivacion

## Alcance

Este contrato cubre las historias asignadas a Jose en el segundo alcance:

- WS_PROVEEDOR2
- PROVEEDOR5
- IDENTIFICADOR6

El Web Service usa SOAP/XML en la frontera WCF. La comunicacion interna del proyecto se mantiene como JSON UTF-8 terminado en salto de linea, segun el contrato del primer alcance.

```text
Cliente SOAP
  -> WS_PROVEEDOR2
  -> JSON PROVEEDOR5
  -> JSON IDENTIFICADOR6
  -> MySQL
```

## Regla de formato

- Transporte interno: socket TCP.
- Codificacion: UTF-8.
- Delimitador de mensaje: `\n`.
- Cuerpo: JSON serializado como texto.
- Campos sensibles cifrados con AES y representados en Base64.

## WS_PROVEEDOR2 -> PROVEEDOR5

Archivo de referencia: `shared/contracts/activar_desactivar_linea_proveedor5.json`.

```json
{
  "tipo_transaccion": "PROVEEDOR5",
  "telefono": "dato_cifrado",
  "identificador_dispositivo": "dato_cifrado",
  "identificador_tarjeta": "dato_cifrado",
  "tipo_servicio": "PREPAGO",
  "identificacion_dueno": "dato_cifrado",
  "accion": "ACTIVAR",
  "fecha_hora": "2026-07-16T18:00:00"
}
```

Valores permitidos:

- `tipo_servicio`: `PREPAGO`, `POSTPAGO`.
- `accion`: `ACTIVAR`, `DESACTIVAR`.

Respuestas permitidas de PROVEEDOR5:

- `OK`
- `Datos Incompletos`
- `Telefono en uso`
- `Telefono no corresponde`
- `Activacion fallida`
- `ERROR`

## PROVEEDOR5 -> IDENTIFICADOR6

Archivo de referencia: `shared/contracts/identificador6.json`.

```json
{
  "tipo_transaccion": "IDENTIFICADOR6",
  "telefono": "dato_cifrado",
  "identificador_dispositivo": "dato_cifrado",
  "identificador_tarjeta": "dato_cifrado",
  "tipo_servicio": "PREPAGO",
  "identificacion_cliente": "dato_cifrado",
  "proveedor_codigo": "XYZ",
  "accion": "ACTIVAR",
  "fecha_hora": "2026-07-16T18:00:00"
}
```

Campos obligatorios:

- `tipo_transaccion`
- `telefono`
- `identificador_dispositivo`
- `identificador_tarjeta`
- `tipo_servicio`
- `identificacion_cliente`
- `proveedor_codigo`
- `accion`

Campos cifrados:

- `telefono`
- `identificador_dispositivo`
- `identificador_tarjeta`
- `identificacion_cliente`

Respuesta exitosa:

```json
{
  "tipo_transaccion": "RESPUESTA_IDENTIFICADOR6",
  "resultado": {
    "codigo": "OK",
    "estado": "LINEA_SINCRONIZADA",
    "mensaje": "OK"
  }
}
```

Respuesta fallida:

```json
{
  "tipo_transaccion": "RESPUESTA_IDENTIFICADOR6",
  "resultado": {
    "codigo": "ERROR",
    "estado": "ACTIVACION_FALLIDA",
    "mensaje": "Activacion fallida"
  }
}
```

## Equivalencia SOAP

La operacion SOAP `ActivarDesactivarLinea` recibe los datos en XML por WCF y construye el JSON interno para PROVEEDOR5. Si PROVEEDOR5 responde `OK`, el WS responde:

```text
Resultado = true
Mensaje = Exitoso
```

Para cualquier otra respuesta:

```text
Resultado = false
Mensaje = Problemas al activar/desactivar la linea.
```
