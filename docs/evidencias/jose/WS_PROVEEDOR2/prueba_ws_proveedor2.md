# Evidencia WS_PROVEEDOR2

Fecha de preparacion: 2026-07-16

## Objetivo

Conectar la operacion SOAP `ActivarDesactivarLinea` del WCF con el PROVEEDOR5 real, eliminando el modo simulado usado durante la prueba inicial.

## Cambios aplicados

Archivo:

```text
dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Web.config
```

Configuracion actual:

```xml
<add key="ProveedorHost" value="127.0.0.1" />
<add key="ProveedorPort" value="6000" />
<add key="ProveedorTimeoutMs" value="5000" />
<add key="ProveedorModoSimulado" value="false" />
```

Esto hace que WS_PROVEEDOR2 envie la trama a Java PROVEEDOR5 en el puerto `6000`.

Archivo:

```text
dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Validators/ActivarDesactivarLineaValidator.cs
```

La validacion ahora comprueba que los campos sensibles tengan formato Base64:

- `NumeroTelefono`
- `IdentificadorTelefono`
- `IdentificadorTarjeta`
- `IdentificacionCliente`

## Flujo esperado

```text
Solicitud SOAP
  -> WS_PROVEEDOR2 / ActivarDesactivarLinea
  -> JSON PROVEEDOR5
  -> Java puerto 6000
  -> IDENTIFICADOR6 Python puerto 5000
  -> MySQL
  -> SQL Server
  -> Respuesta SOAP
```

## Solicitud de activacion para prueba SOAP

Datos planos antes de cifrar:

```text
telefono: 70002233
identificador telefono: 1234567890123456
identificador tarjeta: 1234567890123456789
tipo: PREPAGO
identificacion cliente: 118880999
estado: activo
```

Valores cifrados usados en la prueba integrada:

```text
NumeroTelefono: PXloB7zo4OHs+zSxgSykUQ==
IdentificadorTelefono: 501EBq6g61woKjf69HmoENmQ/3tpAErnngXocmUKiG8=
IdentificadorTarjeta: 501EBq6g61woKjf69HmoEJk4otscp6Zged72iAeU90I=
IdentificacionCliente: 7TZXP5ymkPeC2HIBhynPNA==
```

Ejemplo de cuerpo SOAP conceptual:

```xml
<ActivarDesactivarLinea>
  <solicitud>
    <NumeroTelefono>PXloB7zo4OHs+zSxgSykUQ==</NumeroTelefono>
    <IdentificadorTelefono>501EBq6g61woKjf69HmoENmQ/3tpAErnngXocmUKiG8=</IdentificadorTelefono>
    <IdentificadorTarjeta>501EBq6g61woKjf69HmoEJk4otscp6Zged72iAeU90I=</IdentificadorTarjeta>
    <Tipo>PREPAGO</Tipo>
    <IdentificacionCliente>7TZXP5ymkPeC2HIBhynPNA==</IdentificacionCliente>
    <Estado>activo</Estado>
  </solicitud>
</ActivarDesactivarLinea>
```

Respuesta esperada si PROVEEDOR5 responde `OK`:

```text
Resultado = true
Mensaje = Exitoso
```

## Solicitud de desactivacion

Usar los mismos datos cifrados y cambiar:

```xml
<Estado>disponible</Estado>
```

Respuesta esperada:

```text
Resultado = true
Mensaje = Exitoso
```

## Prueba previa de PROVEEDOR5

El proveedor real ya fue probado por socket con la misma trama que genera WCF.

Resultado de activacion:

```text
RESPUESTA_PROVEEDOR5=OK
```

Resultado de desactivacion:

```text
RESPUESTA_PROVEEDOR5=OK
```

Evidencia relacionada:

```text
docs/evidencias/jose/PROVEEDOR5/prueba_proveedor5.md
```

## Validacion pendiente en Visual Studio Community

Desde esta terminal no se pudo compilar el proyecto WCF con `dotnet build` porque es un proyecto Web/WCF clasico de .NET Framework 4.7.2 y requiere los targets de Visual Studio:

```text
Microsoft.WebApplication.targets
```

Por tanto, la prueba final debe ejecutarse en Visual Studio Community:

1. Levantar Python Identificador en puerto `5000`.
2. Levantar Java Proveedor en puerto `6000`.
3. Abrir `WS_Proveedor` en Visual Studio Community.
4. Ejecutar el servicio WCF.
5. Invocar `ActivarDesactivarLinea`.
6. Confirmar `Resultado=true` y `Mensaje=Exitoso`.
7. Verificar SQL Server y MySQL con las evidencias indicadas.
