# Evidencia PROVEEDOR5

Fecha de preparacion: 2026-07-16

## Objetivo

Validar que PROVEEDOR5 recibe una trama JSON desde WS_PROVEEDOR2, consulta SQL Server, sincroniza IDENTIFICADOR6 y confirma el cambio manteniendo consistencia con MySQL.

## Ambiente

- Proveedor Java: socket TCP `127.0.0.1:6000`
- Identificador Python: socket TCP `127.0.0.1:5000`
- SQL Server: `CentralProveedor`
- MySQL: `central_identificador`

## Migracion aplicada en SQL Server

Se ejecuto:

```text
database/sqlserver_proveedor/migrations/009_proveedor5_linea.sql
```

Campos agregados a `servicios`:

- `identificador_telefono_cifrado`
- `identificador_tarjeta_cifrado`
- `identificacion_dueno_cifrada`
- `estado_linea`

## Datos de prueba

Datos planos antes de cifrar:

```text
telefono: 70002233
identificador_dispositivo: 1234567890123456
identificador_tarjeta: 1234567890123456789
tipo_servicio: PREPAGO
identificacion_dueno: 118880999
accion: ACTIVAR / DESACTIVAR
```

La linea fue preparada en SQL Server como:

```text
estado_linea = DISPONIBLE
activo = 0
proveedor_codigo = XYZ
tipo_servicio = PREPAGO
```

## Caso 1 - Activacion valida

Respuesta de PROVEEDOR5:

```text
OK
```

Verificacion SQL Server:

```text
tipo=PREPAGO activo=True estado=ACTIVO tiene_dueno=1 saldo=1000.00
```

Verificacion MySQL:

```json
{
  "telefono": "70002233",
  "tipo_servicio": "PREPAGO",
  "activo": 1,
  "cliente": "118880999"
}
```

## Caso 2 - Desactivacion valida

Respuesta de PROVEEDOR5:

```text
OK
```

Verificacion SQL Server:

```text
tipo=PREPAGO activo=False estado=DISPONIBLE tiene_dueno=0 saldo=1000.00
```

Verificacion MySQL:

```json
{
  "telefono": "70002233",
  "tipo_servicio": "PREPAGO",
  "activo": 0,
  "cliente": null
}
```

## Caso 3 - Datos incompletos

Trama enviada solo con telefono y accion.

Respuesta de PROVEEDOR5:

```text
Datos Incompletos
```

## Resultado

PROVEEDOR5 queda probado para activacion, desactivacion, saldo inicial prepago, sincronizacion con IDENTIFICADOR6 y validacion de campos obligatorios.
