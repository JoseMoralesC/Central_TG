# Casos de prueba

## Alcance

Este documento contiene los casos minimos para preparar la demo integrada del
proyecto y, en especial, validar el simulador C# de la rama `feature/csharp`.

Fecha de preparacion: 2026-06-07.

Configuracion esperada para C#:

```txt
Identificador Python: 127.0.0.1:5000
Encoding: UTF-8
Trama: JSON + salto de linea
AES: CBC / PKCS7 / Base64
```

## Matriz de casos

| Codigo | Caso | Flujo | Resultado esperado | Estado |
|---|---|---|---|---|
| CSHARP-001 | Error de conexion | C# -> Python apagado | Mensaje claro de conexion fallida | Preparado |
| CSHARP-002 | Consulta de saldo | C# -> Python -> Java -> Python -> C# | Se muestra saldo o error estructurado | Pendiente de Python/Java |
| CSHARP-003 | Llamada autorizada | C# -> Python -> Java -> Python -> C# | Se abre llamada activa | Pendiente de Python/Java |
| CSHARP-004 | Saldo insuficiente | C# -> Python -> Java -> Python -> C# | Se muestra rechazo `INSUF` | Pendiente de Python/Java |
| CSHARP-005 | Finalizacion | C# -> Python -> Java | Se muestra estado final y se registra movimiento | Pendiente de Python/Java |
| CSHARP-006 | Respuesta invalida | Python devuelve texto no JSON | C# muestra respuesta sin excepcion | Preparado |
| CSHARP-007 | SIM o telefono incorrecto | C# -> Python -> MySQL | C# muestra motivo/error recibido | Pendiente de Python |

## CSHARP-001 - Error de conexion

Pasos:

1. Mantener apagado el Identificador Python.
2. Ejecutar el simulador C#.
3. Seleccionar un telefono.
4. Presionar `Saldo` o `Marcar`.

Resultado esperado:

- C# no se congela.
- Muestra mensaje similar a:
  `ERROR: No fue posible conectar con el Identificador Python...`
- La bitacora local registra el intento.

## CSHARP-002 - Consulta de saldo

Pasos:

1. Levantar Python en `127.0.0.1:5000`.
2. Levantar Java si Python consulta al proveedor.
3. Ejecutar C#.
4. Seleccionar telefono prepago.
5. Presionar `Saldo`.
6. Confirmar codigo `#9090*`.

Resultado esperado:

- C# envia `CONSULTA_SALDO` con `accion: SALDO`.
- Incluye ubicacion.
- `telefono_origen`, `identificador_telefono`, `identificador_dispositivo` e
  `identificador_tarjeta` viajan cifrados.
- Se muestra saldo disponible o mensaje de error recibido.

## CSHARP-003 - Llamada autorizada

Pasos:

1. Levantar componentes Python y Java.
2. Ejecutar C#.
3. Seleccionar telefono activo.
4. Presionar `Marcar`.
5. Digitar numero destino activo.
6. Enviar solicitud.

Resultado esperado:

- C# envia `SOLICITUD_LLAMADA`.
- Se muestra respuesta del Identificador.
- Si la respuesta es `OK`, se abre `LlamadaActivaForm`.
- El tiempo maximo recibido se usa para la trama de inicio.

## CSHARP-004 - Saldo insuficiente

Pasos:

1. Seleccionar telefono prepago sin saldo segun datos de prueba.
2. Enviar solicitud de llamada.

Resultado esperado:

- C# muestra `INSUF`, `ERROR` o el mensaje que retorne Python.
- No debe abrir pantalla de llamada activa si no hay autorizacion.

## CSHARP-005 - Finalizacion de llamada

Pasos:

1. Ejecutar una llamada autorizada.
2. Esperar algunos segundos.
3. Presionar `Finalizar llamada`.

Resultado esperado:

- C# envia `FINALIZAR_LLAMADA`.
- La trama incluye duracion, destino, datos de cobro local estimado y datos
  auditables cifrados.
- C# muestra el estado final recibido.

## CSHARP-006 - Respuesta invalida

Pasos:

1. Probar contra un socket Python temporal o respuesta manual no JSON.
2. Enviar una trama desde C#.

Resultado esperado:

- C# no lanza excepcion visual.
- Muestra el contenido recibido o indica formato invalido.

## CSHARP-007 - Telefono o tarjeta incorrectos

Pasos:

1. Configurar un telefono virtual con datos no registrados o no coincidentes.
2. Enviar solicitud.

Resultado esperado:

- Python devuelve motivo de rechazo.
- C# muestra el motivo sin intentar iniciar llamada.

## Checklist de demo C#

- [ ] El proyecto C# compila sin errores.
- [ ] La pantalla principal muestra varios telefonos.
- [ ] Se puede marcar un numero.
- [ ] Se envia solicitud de llamada.
- [ ] Se muestra respuesta del Identificador.
- [ ] Se puede iniciar llamada si la respuesta es `OK`.
- [ ] Se puede finalizar llamada.
- [ ] Se muestra duracion.
- [ ] Se puede consultar saldo con `#9090*`.
- [ ] La consulta de saldo incluye ubicacion.
- [ ] Los datos sensibles viajan cifrados.
- [ ] Host y puerto son ajustables por `.env` o variables de entorno.
- [ ] Errores de conexion se muestran claramente.
- [ ] Existe bitacora local del simulador.
- [ ] Esta claro que Identificador5 oficial pertenece a Python.
- [ ] Los contratos estan listos para probar con Python.
