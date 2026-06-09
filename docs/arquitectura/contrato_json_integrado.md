# Contrato JSON integrado

## Estandar oficial del equipo

La comunicacion oficial del proyecto es JSON sobre sockets TCP:

```txt
C# Simulador
  -> JSON UTF-8 + \n
Python Identificador
  -> JSON UTF-8 + \n
Java Proveedor
```

No se usa texto plano con separadores. El Proveedor Java recibe y responde JSON.

## Regla de transporte

- Encoding: UTF-8.
- Cierre de mensaje: salto de linea `\n`.
- Cada solicitud debe incluir `tipo_transaccion`.
- Las solicitudes al proveedor deben incluir `accion`.
- Cada respuesta debe incluir `status` y `resultado.codigo`.

## Acciones Python -> Java

| Accion | Uso | Contrato |
|---|---|---|
| `VERIFICAR_SALDO` | Autorizar llamada | `shared/contracts/consulta_proveedor.json` |
| `CONSULTAR_SALDO` | Consultar saldo disponible | `shared/contracts/consulta_proveedor.json` |
| `REBAJAR_SALDO` | Registrar llamada y movimiento | `shared/contracts/registro_movimiento_proveedor.json` |
| `REGISTRO_MOVIMIENTO` | Alias de registro de movimiento | `shared/contracts/registro_movimiento_proveedor.json` |
| `FINALIZAR_LLAMADA` | Alias de cierre/cobro | `shared/contracts/registro_movimiento_proveedor.json` |

## Forma estandar de respuesta

```json
{
  "tipo_transaccion": "RESPUESTA_PROVEEDOR",
  "accion": "VERIFICAR_SALDO",
  "status": "OK",
  "estado": "OK",
  "mensaje": "Operacion realizada correctamente",
  "resultado": {
    "codigo": "OK",
    "estado": "AUTORIZADO",
    "mensaje": "Operacion realizada correctamente"
  },
  "datos_autorizacion": {}
}
```

## Codigos permitidos

- `OK`: operacion correcta.
- `INSUF`: saldo insuficiente.
- `ERROR`: error de validacion, base de datos, formato o comunicacion.

## Compatibilidad

Para no romper consumidores existentes:

- Java mantiene `status` en el nivel superior.
- Python sigue leyendo `resultado.codigo` cuando existe.
- Python tambien acepta `status` como respaldo.
- Las respuestas de Java incluyen `datos_autorizacion` cuando hay datos de saldo, tiempo o movimiento.

## Bitacoras

- Python registra entrada/salida en `bitacora_identificador.txt`.
- Java registra entrada/salida/error en `logs/proveedor_bitacora.txt`.
