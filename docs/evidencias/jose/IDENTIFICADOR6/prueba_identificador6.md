# Evidencia IDENTIFICADOR6

Fecha de preparacion: 2026-07-16

## Objetivo

Validar que `IDENTIFICADOR6` recibe una trama JSON interna desde PROVEEDOR5, valida datos cifrados AES, sincroniza MySQL y responde con el formato oficial.

## Ambiente

- Base MySQL: `central_identificador`
- Tabla principal: `telefonos`
- Proveedor usado: `XYZ`
- Transporte esperado en integracion: JSON UTF-8 por socket TCP

## Migracion aplicada

Se agrego la columna:

```sql
ALTER TABLE telefonos
ADD COLUMN identificacion_cliente_cifrada VARCHAR(255) NULL;
```

La columna permite asociar la linea con la identificacion cifrada del cliente, como solicita IDENTIFICADOR6.

## Caso 1 - Activacion valida

Datos planos usados antes de cifrar:

```text
telefono: 70001122
identificador_dispositivo: 1234567890123456
identificador_tarjeta: 1234567890123456789
tipo_servicio: PREPAGO
identificacion_cliente: 118880999
proveedor_codigo: XYZ
accion: ACTIVAR
```

Respuesta obtenida:

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

Verificacion en MySQL:

```json
{
  "telefono": "70001122",
  "tipo_servicio": "PREPAGO",
  "activo": 1,
  "cliente": "118880999"
}
```

## Caso 2 - Desactivacion valida

Respuesta obtenida:

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

Verificacion en MySQL:

```json
{
  "telefono": "70001122",
  "tipo_servicio": "PREPAGO",
  "activo": 0,
  "cliente": null
}
```

## Caso 3 - Datos sensibles sin AES valido

Respuesta obtenida:

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

## Resultado

IDENTIFICADOR6 queda probado de forma directa contra MySQL para activacion, desactivacion y rechazo por datos cifrados invalidos.
