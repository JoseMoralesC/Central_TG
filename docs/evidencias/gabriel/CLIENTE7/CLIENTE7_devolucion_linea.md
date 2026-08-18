# Evidencia CLIENTE7 - Devolucion de lineas

Fecha de validacion: 2026-08-17

## Objetivo

Cerrar CLIENTE7 (pantalla de devolucion de lineas) cumpliendo los criterios de aceptacion y dejando la integracion Java (PROVEEDOR5) preparada, sin ejecutarla (la parte de Java no es del dueño de este cliente; solo se dejo lista para funcionar).

## Criterios de aceptacion y estado

| Criterio | Estado |
|---|---|
| 1a. Lista prepago con numero y saldo usando WS_PROVEEDOR | Cumplido - `ClienteController.DevolverLinea` (GET) consulta WS_ProveedorCliente (55260) y por cada prepago llama `ConsultarSaldoAsync` (WS_PROVEEDOR 53885). Sin saldo muestra "No disponible". |
| 1b. Lista postpago con numero y facturacion pendiente (0 si no hay) | Cumplido - `FacturaPendiente` desde la consulta; la vista muestra el monto o 0.00. |
| 1c. Confirmacion "¿Está seguro de querer devolver la línea telefónica?" | Cumplido - `confirm()` con el texto exacto al hacer clic en la linea. |
| 1d. Envio automatico a WS_PROVEEDOR2 (numero, id telefono, id tarjeta, tipo, id cliente, estado inactivo) | Cumplido - POST arma la solicitud con `Estado="inactivo"` y todos los datos cargados de la linea (no visibles). Exito: "La linea telefonica fue devuelta correctamente."; fallo: muestra el detalle recibido. |
| 1e. Postpago con facturacion pendiente cancela el proceso | Cumplido - "La linea postpago posee facturacion pendiente. Primero debe cancelar la factura para poder devolver la linea." |

## Causa raiz corregida (Resultado=false en WS_PROVEEDOR2)

El WS recibia la solicitud pero **deserializaba el parametro como null**, por lo que siempre devolvia `Resultado=false` con mensaje generico.

Diagnostico:
- Con un cliente WCF real (`ChannelFactory`) el servicio devolvia `Resultado=true / Exitoso`.
- Comparando el envelope que genera WCF vs el que construia el portal, la diferencia estaba en el namespace del elemento `solicitud`.

Bug: los clientes SOAP del portal declaraban `solicitud`/`request` con el namespace del DataContract (`http://schemas.datacontract.org/2004/07/WS_Proveedor.Models`) cuando WCF espera ese elemento en el namespace de la operacion (`http://tempuri.org/`), dejando los campos hijos en el namespace Models.

Fixes aplicados (solo en el portal, los WS estan correctos):

- `PortalCliente/Services/Proveedor2SoapClient.cs` - `solicitud` en `tempuri.org`, campos con prefijo `a:` (WS_Proveedor.Models).
- `PortalCliente/Services/ProveedorPortalSoapClient.cs` - `request` en `tempuri.org`, campos con prefijo `a:` (WS_Proveedor_1.Models).
- `PortalCliente/Services/ProveedorClienteSoapClient.cs` - no requeria cambio (parametros primitivos en `tempuri.org`).

## Pruebas ejecutadas (modo simulado en WS_PROVEEDOR2)

Envases SOAP directos a `http://localhost:55254/ProveedorService.svc`:

| Caso | Numero | Resultado |
|---|---|---|
| Desactivacion prepago | `NzAwMDQ0NTU=` (70004455) | `Resultado=true, Mensaje=Exitoso` |
| Desactivacion | 25754044 | `Resultado=true, Mensaje=Exitoso` |

Flujo por el portal (`http://localhost:5031/Cliente/DevolverLinea`, cliente 118880999):

| Caso | Resultado |
|---|---|
| POST `70004455` (prepago) | "La linea telefonica fue devuelta correctamente." |
| POST `25754044` (postpago sin factura) | "La linea telefonica fue devuelta correctamente." |
| POST `22758463` (postpago con factura 70.00) | "La linea postpago posee facturacion pendiente. Primero debe cancelar la factura..." |

## Integracion real (preparada, no ejecutada)

La parte Java (PROVEEDOR5) pertenece a otro integrante. Se dejo lista para funcionar:

- `java_proveedor/lib/mssql-jdbc.jar` descargado (Maven Central, 12.4.2.jre11).
- `java_proveedor/src/config/config.properties` actualizado a SQL Server real:
  `db.url=jdbc:sqlserver://100.114.84.5:49172;databaseName=CentralProveedor;encrypt=true;trustServerCertificate=true`, `db.usuario=gabriel_dev`.
- Compilacion verificada (`javac` exitoso, 28 clases).

Comandos para la prueba real cuando el dueño de Java levante PROVEEDOR5 en 6000 (desde la raiz del repo):

```powershell
javac java_proveedor\Main.java java_proveedor\src\sockets\SocketTCP.java java_proveedor\src\sockets\ManejoCliente.java java_proveedor\src\services\*.java java_proveedor\src\database\*.java java_proveedor\src\models\*.java java_proveedor\src\config\*.java -cp "java_proveedor\lib\mssql-jdbc.jar"
java -cp ".;java_proveedor\lib\mssql-jdbc.jar" java_proveedor.Main

cd python_identificador; python main.py

dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj
```

Y en `WS_Proveedor/Web.config` cambiar `ProveedorModoSimulado` a `false` (queda en `true` para esta validacion porque Java no corrio).

## Remocion de datos de prueba

Se revirtio en SQL Server `CentralProveedor` (host 100.114.84.5:49172, usuario `gabriel_dev`):
- `servicios` 10 (25754044) y 15 (22758463): `identificacion_dueno_cifrada = NULL`.
- `servicios` 10: identificadores restaurados a los valores originales de la bitacora (`SiD34tkppNlwDR0djVZ1Y/2S3RnD0Y+RdOK722EqEnY=` y `SiD34tkppNlwDR0djVZ1Y2v/ZBQpheDwYhnO6nvHodE=`).
- Las lineas 14 y 17 (prepago) ya pertenecian legitimamente al cliente 118880999; no se modificaron.
- El registro `clientes` id=25 (`Cliente test CLIENTE7` / `cliente7@test.com`) quedo huerfano y disponible para eliminar si se desea.

## Recordatorios

- SMTP Gmail: las credenciales van en `PortalCliente/appsettings.Development.json` (gitignored); el de este repo esta con valores vacios.
- Para dejar el WS en modo real, flip `ProveedorModoSimulado=false` solo cuando PROVEEDOR5 (6000) este escuchando.

## Archivos relevantes

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs` (GET/POST DevolverLinea)
- `dotnet_webservices/PortalCliente/Views/Cliente/DevolverLinea.cshtml`
- `dotnet_webservices/PortalCliente/Services/Proveedor2SoapClient.cs` (fix)
- `dotnet_webservices/PortalCliente/Services/ProveedorPortalSoapClient.cs` (fix)
- `dotnet_webservices/PortalCliente/Services/ProveedorClienteSoapClient.cs` (sin cambio)
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Web.config` (`ProveedorModoSimulado=true`)
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs` (sin cambios respecto al estado original)
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs` (consulta de lineas en SQL Server)
- `java_proveedor/src/config/config.properties`, `java_proveedor/lib/mssql-jdbc.jar`
- `python_identificador/main.py` (identificador Python, puerto 5000)