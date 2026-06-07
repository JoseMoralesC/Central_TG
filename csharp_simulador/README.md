# Simulador Telefonico - C#

Componente desarrollado por Jose Rodolfo Morales Calderon para la rama
`feature/csharp`.

## Alcance

Este componente cubre la parte C# del proyecto:

- SIM1: simulador de llamadas telefonicas.
- Apoyo a Identificador4: pantalla y trama de consulta de saldo.
- Apoyo a Identificador5: envio de datos auditables y bitacora local del simulador.

La bitacora oficial del Identificador, su cola y su hilo independiente pertenecen al
componente Python. C# solo registra una bitacora local para depuracion y envia la
informacion que Python necesita auditar.

## Ejecucion

Desde la raiz del repositorio:

```powershell
dotnet build csharp_simulador\SimuladorTelefonico.slnx
dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj
```

El Identificador Python debe estar escuchando antes de probar integracion:

```txt
Host: 127.0.0.1
Puerto: 5000
```

## Configuracion

El simulador lee variables de entorno y, si existe, el archivo `.env` de la raiz.

| Variable | Uso | Valor por defecto |
|---|---|---|
| `IDENTIFICADOR_HOST` | Host del socket Python | `127.0.0.1` |
| `IDENTIFICADOR_PORT` | Puerto del socket Python | `5000` |
| `CSHARP_SOCKET_CONNECT_TIMEOUT_MS` | Timeout de conexion | `5000` |
| `CSHARP_SOCKET_READ_TIMEOUT_MS` | Timeout de lectura | `8000` |
| `CSHARP_SOCKET_BUFFER_BYTES` | Buffer de respuesta | `8192` |
| `SOCKET_ENCODING` | Encoding de sockets | `UTF-8` |
| `AES_KEY` | Llave AES compartida con Python | `ClaveSecreta1234` |
| `AES_IV` | Vector AES compartido con Python | `VectorInicio1234` |
| `CSHARP_AES_ACTIVO` | Activa cifrado de datos sensibles | `true` |

Pendiente critico de integracion: Python debe usar la misma llave, IV, modo
`AES-CBC`, padding `PKCS7` y salida `Base64`. El `.env` actual contiene
`AES_SECRET_KEY`, pero Python usa `AES_KEY`/`AES_IV`; se debe unificar antes de la
demo integrada.

## Flujo general

1. El usuario selecciona un telefono virtual.
2. Puede marcar un numero destino o consultar saldo con `#9090*`.
3. C# arma una trama JSON.
4. Los datos sensibles se cifran antes de serializar:
   - `telefono_origen`
   - `identificador_telefono`
   - `identificador_dispositivo`
   - `identificador_tarjeta`
5. La trama se envia por TCP al Identificador Python.
6. C# muestra la respuesta y la registra en bitacora local.
7. Si la llamada es autorizada, se abre la pantalla de llamada activa.
8. Al finalizar, C# envia la trama de finalizacion.

## Formularios principales

- `Form1.cs`: pantalla principal con telefonos virtuales.
- `MarcarNumeroForm.cs`: captura numero destino y solicita llamada.
- `ConsultaSaldoForm.cs`: valida `#9090*` y consulta saldo.
- `LlamadaActivaForm.cs`: muestra duracion, envia inicio y finalizacion.

## Servicios principales

- `TramaService.cs`: serializa JSON, registra bitacora local y envia al socket.
- `TcpSocketClient.cs`: comunicacion TCP con timeouts.
- `CryptoService.cs`: AES-CBC/PKCS7/Base64 para datos sensibles.
- `RespuestaService.cs`: interpreta respuestas `OK`, `INSUF`, `ERROR`, errores de
  conexion, respuestas simples y contratos estructurados.
- `BitacoraService.cs`: bitacora local del simulador en `bin/.../logs`.

## Contratos enviados a Python

Los contratos usan JSON UTF-8 terminado en salto de linea.

Tipos actuales:

- `SOLICITUD_LLAMADA`
- `INICIO_LLAMADA`
- `FINALIZAR_LLAMADA`
- `CONSULTA_SALDO` con `accion: SALDO`

Pendiente de coordinacion: confirmar con Python si la consulta debe llegar como
`CONSULTA_SALDO` o como `SALDO`. Se mantiene `CONSULTA_SALDO` porque existe en
`shared/contracts/consulta_saldo.json`.

## Bitacora local

La bitacora local guarda tramas enviadas, respuestas recibidas y errores de envio.
Ayuda a presentar y depurar, pero no sustituye la bitacora oficial del
Identificador en Python.

## Pendientes conocidos

- Confirmar contrato final de respuesta con Python.
- Confirmar nombres definitivos de `INICIO_LLAMADA` y `FINALIZAR_LLAMADA`.
- Alinear datos seed de MySQL con los valores cifrados por C#.
- Realizar prueba integrada C# -> Python -> Java.
