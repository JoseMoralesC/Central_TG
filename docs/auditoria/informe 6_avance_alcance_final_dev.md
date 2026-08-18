# Informe 6 de avance - Alcance final contra proyecto actual

Fecha de analisis: 2026-08-18  
Rama revisada: arbol local actual del repositorio `Central_TG`  
Documentos base: PDF `docs/Proyecto/Proyecto final.pdf`, `docs/estrategia/estrategia_3.md`, diagnostico tecnico actual del proyecto

## 1. Resumen ejecutivo

El proyecto se encuentra en un estado funcional parcial avanzado. La base tecnica
principal existe y compila en varios frentes: WebApps Web Forms, WS
Autenticacion, WS Proveedor WCF, WS ProveedorCliente WCF, PortalCliente ASP.NET
Core, simulador C#, identificador Python y proveedor Java. Frente al informe
anterior, el cambio mas importante es que ya existe un `PortalCliente`
independiente para CLIENTE4-CLIENTE7 y un `WS_ProveedorCliente` que compila y
expone consulta de lineas, recarga y pago de factura.

La deuda principal ya no es solamente falta de codigo, sino fragmentacion e
integracion: la Web Cliente principal en `dotnet_webapps/WebCliente` todavia
tiene CLIENTE4-CLIENTE7 como pantallas vacias/reservadas, mientras que la
implementacion mas completa vive en `dotnet_webservices/PortalCliente` y no esta
conectada al login/registro de la Web Cliente principal. En administrativo,
ADM3-ADM5 siguen en una Web MVC separada que no compila por paquete NuGet
faltante y cuyos controladores aun son placeholders.

La revision contra `estrategia_3.md` mantiene el reparto:

| Companero | Nombre | Historias asignadas |
|---|---|---|
| Companero 1 | Charlie | ADM1, ADM2, ADM3, ADM4, ADM5 |
| Companero 2 | Jose | ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3 |
| Companero 3 | Gabriel | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |

Estado global estimado:

| Area | Estado | Comentario |
|---|---|---|
| Simulador C# | Cumple como base | Compila y forma parte del flujo integrado. |
| Identificador Python | Parcial alto | Atiende llamadas, saldo, bitacora, catalogo y sincronizacion. |
| Proveedor Java | Parcial alto | Tiene verificacion, movimientos, bitacora, recarga, registro/cambio de estado y facturacion. |
| WS Autenticacion | Cumple funcionalmente | Login, creacion, modificacion, cambio de estado, listado y eliminacion de usuarios en MongoDB. |
| WS Proveedor WCF | Parcial funcional | Expone activar/desactivar, calcular facturacion y ultima facturacion; no expone todos los listados administrativos. |
| WS ProveedorCliente WCF | Parcial alto | Compila y expone consulta de lineas, recarga y pago; no expone devolucion propia ni correo directo. |
| Web Administrativo principal | Parcial medio | Tiene ADM1, parte de ADM2, ADM6 y ADM7; faltan ADM3-ADM5 en la Web final integrada. |
| Web Cliente principal | Parcial medio-bajo | Tiene CLIENTE1, CLIENTE2 basico y CLIENTE3; CLIENTE4-CLIENTE7 siguen en placeholder. |
| PortalCliente ASP.NET Core | Parcial alto para Gabriel | Implementa CLIENTE4-CLIENTE7 con WS, pero sin login/registro integrado de cliente. |
| WebAdministrativa MVC | Parcial bajo | Existe para ADM3-ADM5, pero no compila y no consume WS real. |
| Documentacion y evidencias | Parcial alto | Hay guias y evidencias; falta consolidar una ruta oficial de demo por historia. |

Resultado general: Jose sigue siendo el bloque mas defendible. Gabriel mejora de
forma relevante gracias a `PortalCliente`, pero debe integrarse o presentarse
claramente como portal oficial para no chocar con `WebCliente`. Charlie mantiene
el mayor riesgo administrativo: ADM3-ADM5 no estan cerradas.

## 2. Requerimientos fuente revisados

El PDF de alcance final exige catorce historias principales:

| Bloque segun estrategia 3 | Historias |
|---|---|
| Companero 1 - Charlie | ADM1, ADM2, ADM3, ADM4, ADM5 |
| Companero 2 - Jose | ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3 |
| Companero 3 - Gabriel | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |

Reglas tecnicas obligatorias del PDF:

- La aplicacion Web Administrativa debe estar desarrollada en C#.
- La aplicacion Web Cliente puede estar en C# u otro lenguaje; el proyecto tiene
  una Web Cliente Web Forms y un PortalCliente ASP.NET Core.
- Las aplicaciones Web no deben acceder directo a base de datos ni a sockets.
- Toda operacion Web debe pasar por Web Services.
- Los Web Services pueden ampliarse o crearse segun necesidad.
- Deben mantenerse actualizados Proveedor, Identificador, Simulador, WS
  Proveedor y WS Autenticacion.
- Debe existir documentacion completa de analisis, diseno, diagramas y
  evidencias.

## 3. Verificaciones ejecutadas

| Verificacion | Resultado |
|---|---|
| Extraccion del PDF `Proyecto final.pdf` con `pypdf` | Correcta; 19 paginas, se confirmaron ADM1-ADM7 y CLIENTE1-CLIENTE7. |
| Lectura de `docs/estrategia/estrategia_3.md` | Correcta; se confirmo reparto Companero 1/2/3. |
| `powershell -File scripts/persona2-preparar.ps1` | Correcto para WebApps, WS Autenticacion, WS Proveedor WCF, simulador C# y Java. |
| Build de `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/WS_ProveedorCliente.sln` | Correcto, 0 errores. |
| Build de `dotnet_webservices/PortalCliente/PortalCliente.csproj` | Correcto, 0 errores. |
| Build de `dotnet_webservices/CentralTelefonica.WebServices/CentralTelefonica.WebServices.csproj` | Correcto despues de restaurar NuGet con acceso de red. |
| Build de `dotnet_webservices/WebAdministrativa/WebAdministrativa.sln` | Falla por paquete faltante `Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1`. |
| Revision de `dotnet_webapps/WebCliente` | CLIENTE4-CLIENTE7 siguen como listas vacias o pantalla reservada. |
| Revision de `dotnet_webservices/PortalCliente` | CLIENTE4-CLIENTE7 tienen implementacion funcional parcial/alta. |
| Revision de `dotnet_webservices/WebAdministrativa` | Acciones de ADM3-ADM5 son placeholders y no consumen WS real. |

Observacion: `scripts/persona2-preparar.ps1` no aplico seeds ni migraciones de
base de datos porque no se ejecuto con `-ApplyMongoSeed` ni
`-ApplySqlMigrations`.

## 4. Hallazgos transversales

### 4.1 La arquitectura base esta viva

El sistema ya tiene varios flujos operativos:

```text
WebApps / PortalCliente / Simulador C# -> WS -> Python Identificador / Java Proveedor -> Bases de datos
```

Tambien existen scripts y guias de apoyo:

- `scripts/persona2-preparar.ps1`
- `scripts/persona2-levantar.ps1`
- `docs/roadmaps/guia_ejecucion_persona_2_web.md`
- `docs/evidencias/gabriel/CLIENTE7/CLIENTE7_devolucion_linea.md`

### 4.2 Hay aplicaciones duplicadas para cubrir el alcance

Actualmente hay:

- Web Administrativo principal: `dotnet_webapps/WebAdministrativo`.
- Web Cliente principal: `dotnet_webapps/WebCliente`.
- WebAdministrativa MVC: `dotnet_webservices/WebAdministrativa`.
- PortalCliente ASP.NET Core: `dotnet_webservices/PortalCliente`.

Esto no es necesariamente invalido, porque el PDF permite ampliar o crear
servicios y aplicaciones Web. El riesgo es de presentacion: si el equipo no
declara cual es el sitio oficial para cada bloque, la evaluacion puede ver
historias duplicadas, incompletas o desconectadas.

### 4.3 El backend tiene mas capacidad que algunas pantallas finales

Java, Python, `WS_Proveedor`, `WS_Proveedor_1` y `WS_ProveedorCliente` contienen
capacidades que no siempre llegan a la Web principal. El caso mas claro es
Gabriel: el backend y el portal separado avanzaron, pero `WebCliente` sigue
mostrando placeholders para esas historias.

### 4.4 El punto debil administrativo sigue siendo ADM3-ADM5

La WebAdministrativa MVC tiene pantallas visuales para nuevas lineas, activar y
desactivar, pero no compila y sus acciones dicen que la integracion con Web
Service sera en una siguiente etapa. En la WebAdministrativo principal no existen
esas opciones en el menu ni pantallas funcionales equivalentes.

## 5. Charlie - ADM1, ADM2, ADM3, ADM4, ADM5

Responsabilidad segun estrategia 3: acceso administrativo y ciclo de vida de
lineas desde la Web Administrativa.

### 5.1 ADM1 - Login administrativo

Estado encontrado: Cumple funcionalmente.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Login.aspx`
- `dotnet_webapps/WebAdministrativo/Login.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`

Lo que ya esta:

- Pantalla de login administrativo.
- Envia usuario y contrasena al WS Autenticacion.
- Usa tipo administrador de forma interna.
- Contrasena viaja cifrada desde el cliente SOAP.
- Si credenciales son incorrectas, muestra mensaje equivalente a lo solicitado.
- Al autenticar redirige al area administrativa.

Pendientes o riesgos:

- El PDF indica que despues del login debe ir a ADM2, pantalla de administracion
  de clientes. Actualmente el flujo real se apoya en las paginas disponibles del
  sitio, pero ADM2 no esta completa.
- Falta evidencia final formal de login exitoso y fallido.

Nivel de cumplimiento estimado: 85%.

### 5.2 ADM2 - Plantilla y administracion de clientes

Estado encontrado: Parcial.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Site.Master`
- `dotnet_webapps/WebAdministrativo/Site.Master.cs`

Lo que ya esta:

- Existe plantilla con barra superior.
- Existe opcion de salir del sitio.
- Existe footer.
- El menu esta disponible en paginas administrativas actuales.

Pendientes o riesgos:

- El menu oficial debe incluir ADM3, ADM4, ADM5, ADM6 y ADM7.
- Actualmente la Web principal solo muestra `Calcular facturacion`,
  `Administradores` y `Salir del sitio`.
- No se observa icono/logo de empresa como imagen; solo marca textual.
- No existe pantalla administrativa principal de clientes/lineas dentro de esta
  Web.

Nivel de cumplimiento estimado: 45%.

### 5.3 ADM3 - Poner nuevas lineas a disposicion

Estado encontrado: Parcial bajo.

Evidencia:

- `dotnet_webservices/WebAdministrativa/Controllers/LineasController.cs`
- `dotnet_webservices/WebAdministrativa/Views/Lineas/Nuevas.cshtml`
- `dotnet_webservices/WebAdministrativa/Views/Lineas/Crear.cshtml`
- `java_proveedor/src/services/AdministracionTelefonica.java`
- `java_proveedor/src/database/ServicioDAO.java`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Services/ProveedorService.cs`

Lo que ya esta:

- Existe una WebAdministrativa MVC con rutas visuales para nuevas lineas.
- Existen clases Java para registrar telefonos.
- Existe un servicio .NET alterno con metodo `RegistrarLinea` en el host
  `CentralTelefonica.WebServices`.

Pendientes o riesgos:

- La WebAdministrativa MVC no compila por paquete NuGet faltante.
- La accion `Crear` solo muestra mensaje de que se conectara al Web Service en
  una siguiente etapa.
- No se consume WS_PROVEEDOR1 desde la pantalla MVC.
- No se lista realmente lineas disponibles desde WS.
- No se elimina linea con confirmacion usando WS.
- Esta funcionalidad no esta en la WebAdministrativo principal.

Nivel de cumplimiento estimado: 25%.

### 5.4 ADM4 - Activar linea vendida

Estado encontrado: Parcial bajo/medio.

Evidencia:

- `dotnet_webservices/WebAdministrativa/Controllers/LineasController.cs`
- `dotnet_webservices/WebAdministrativa/Views/Lineas/Activar.cshtml`
- `dotnet_webservices/WebAdministrativa/Views/Lineas/ConfirmarActivacion.cshtml`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- Backend WCF `ActivarDesactivarLinea` existe.
- Java `Proveedor5Service` soporta activacion y sincroniza con Identificador.
- MVC tiene pantallas visuales iniciales.

Pendientes o riesgos:

- La pantalla MVC no consume realmente el WS.
- La Web principal no contiene ADM4.
- Falta listado real de lineas disponibles.
- Falta asociar cedula de cliente desde pantalla final funcional.
- Falta prueba integral desde Web -> WS -> Java -> Python -> SQL/MySQL.

Nivel de cumplimiento estimado: 35%.

### 5.5 ADM5 - Devolucion/desactivacion administrativa de linea

Estado encontrado: Parcial bajo/medio.

Evidencia:

- `dotnet_webservices/WebAdministrativa/Controllers/LineasController.cs`
- `dotnet_webservices/WebAdministrativa/Views/Lineas/Desactivar.cshtml`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`
- `database/sqlserver_proveedor/migrations/013_normalizar_estado_linea_disponible.sql`

Lo que ya esta:

- Backend soporta desactivacion.
- Se normalizo el estado de linea hacia `ACTIVO` y `DISPONIBLE`.

Pendientes o riesgos:

- La pantalla administrativa final no esta conectada.
- Falta listado real de lineas en uso.
- Falta confirmacion real desde Web administrativa.
- El PDF habla de estado inactivo; el proyecto usa `DISPONIBLE` para devolver
  lineas. Esto debe explicarse en defensa.

Nivel de cumplimiento estimado: 35%.

### 5.6 Resultado Charlie

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM1 | Funcional | 85% | Evidencia final y ajustar destino/documentacion del flujo. |
| ADM2 | Parcial | 45% | Menu completo ADM3-ADM7, logo/icono y pantalla base administrativa. |
| ADM3 | Parcial bajo | 25% | Conectar pantalla con WS_PROVEEDOR1/listados/eliminacion o integrarla en Web principal. |
| ADM4 | Parcial | 35% | Listado disponible, formulario cedula y consumo real de WS_PROVEEDOR2. |
| ADM5 | Parcial | 35% | Listado en uso, confirmacion y desactivacion/devolucion via WS. |

Riesgo principal: alto. El login existe, pero el bloque de administracion de
lineas no esta cerrado en la Web final.

## 6. Jose - ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3

Responsabilidad segun estrategia 3: facturacion, mantenimiento de usuarios,
login cliente, plantilla cliente y registro cliente.

### 6.1 ADM6 - Calcular facturacion

Estado encontrado: Cumple funcionalmente.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Facturacion.aspx`
- `dotnet_webapps/WebAdministrativo/Facturacion.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/ProveedorSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor6Service.java`
- `database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql`
- `database/sqlserver_proveedor/migrations/012_seed_facturacion_persona2.sql`

Lo que ya esta:

- Pantalla Web de calculo de facturacion.
- Consulta ultima facturacion desde WS.
- Valida fecha maxima de pago contra fecha de calculo.
- Valida continuidad contra la ultima fecha calculada.
- Consume WS Proveedor para calcular facturacion.
- Java ejecuta procedimiento almacenado SQL Server.

Pendientes o riesgos:

- Falta evidencia formal final de pantalla, SQL Server y respuesta del WS.
- El calculo depende de que las migraciones 010/012/013 esten aplicadas en el
  ambiente de defensa.

Nivel de cumplimiento estimado: 90%.

### 6.2 ADM7 - Mantenimiento usuario administrador

Estado encontrado: Cumple funcionalmente.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Administradores.aspx`
- `dotnet_webapps/WebAdministrativo/Administradores.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`
- `database/mongodb/datos_semilla_persona_2.js`

Lo que ya esta:

- Lista administradores.
- Crear administrador tipo 1 activo.
- Editar datos sin forzar cambio de contrasena.
- Activar/inactivar usuario.
- Eliminar usuario.
- Valida correo y contrasena de 14 caracteres con mayuscula, minuscula, numero
  y caracter especial.
- Usa WS Autenticacion y MongoDB.

Pendientes o riesgos:

- La grilla muestra usuario/contrasena cifrados segun contrato; puede ser raro
  visualmente, pero coincide con datos almacenados cifrados.
- Falta evidencia formal final de CRUD completo.

Nivel de cumplimiento estimado: 90%.

### 6.3 CLIENTE1 - Login cliente

Estado encontrado: Cumple funcionalmente en WebCliente.

Evidencia:

- `dotnet_webapps/WebCliente/Login.aspx`
- `dotnet_webapps/WebCliente/Login.aspx.cs`
- `dotnet_webapps/WebCliente/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`

Lo que ya esta:

- Pantalla login cliente.
- Enlace a registro.
- Usa tipo cliente internamente.
- Envia contrasena cifrada.
- Si credenciales son correctas entra a `Lineas.aspx`.
- Si son incorrectas muestra mensaje esperado.

Pendientes o riesgos:

- `PortalCliente`, donde viven CLIENTE4-CLIENTE7, no consume esta sesion ni este
  login; pide identificacion manual.
- Falta evidencia final de caso exitoso y fallido.

Nivel de cumplimiento estimado: 88%.

### 6.4 CLIENTE2 - Plantilla portal cliente

Estado encontrado: Parcial medio.

Evidencia:

- `dotnet_webapps/WebCliente/Site.Master`
- `dotnet_webapps/WebCliente/Site.Master.cs`
- `dotnet_webapps/WebCliente/Lineas.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx`
- `dotnet_webservices/PortalCliente/Views/Shared/_Layout.cshtml`

Lo que ya esta:

- `WebCliente` tiene menu con lineas activas, cargar saldo, pagar facturas,
  devolver linea y salir.
- `WebCliente` muestra saludo `Hola` + nombre del cliente.
- Existe footer y navegacion persistente.
- `PortalCliente` tambien tiene layout con menu CLIENTE4-CLIENTE7 y footer.

Pendientes o riesgos:

- `WebCliente` tiene la plantilla conectada al login, pero sus opciones
  transaccionales son placeholders.
- `PortalCliente` tiene las historias transaccionales, pero no tiene login real
  ni saludo del cliente autenticado desde WS Autenticacion.
- Falta icono/logo real de empresa; se usa marca textual.

Nivel de cumplimiento estimado: 65%.

### 6.5 CLIENTE3 - Registro cliente

Estado encontrado: Cumple funcionalmente en WebCliente.

Evidencia:

- `dotnet_webapps/WebCliente/Registro.aspx`
- `dotnet_webapps/WebCliente/Registro.aspx.cs`
- `dotnet_webapps/WebCliente/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`

Lo que ya esta:

- Pantalla de registro cliente.
- Solicita datos personales, usuario y contrasena.
- Crea usuario tipo 2 activo por medio del WS.
- Valida correo y contrasena de 14 caracteres.
- Tiene opcion para volver al login.

Pendientes o riesgos:

- Falta evidencia final.
- Segundo apellido es opcional en implementacion; validar si el profesor lo exige
  como campo obligatorio absoluto.
- El registro no queda enlazado con `PortalCliente` salvo por identificacion
  ingresada manualmente.

Nivel de cumplimiento estimado: 86%.

### 6.6 Resultado Jose

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM6 | Funcional | 90% | Evidencia final y asegurar migraciones aplicadas. |
| ADM7 | Funcional | 90% | Evidencia final CRUD completo. |
| CLIENTE1 | Funcional | 88% | Evidencia login exitoso/fallido e integracion con PortalCliente si se usa como portal oficial. |
| CLIENTE2 | Parcial medio/alto | 65% | Unificar plantilla/portal funcional, logo real y salida de sesion en la ruta oficial. |
| CLIENTE3 | Funcional | 86% | Evidencia final y confirmar segundo apellido. |

Riesgo principal: medio. El bloque de Jose funciona, pero CLIENTE1-CLIENTE3 y
CLIENTE4-CLIENTE7 estan partidos entre dos aplicaciones.

## 7. Gabriel - CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7

Responsabilidad segun estrategia 3: autogestion transaccional del cliente.

### 7.1 CLIENTE4 - Mostrar lineas asociadas al cliente

Estado encontrado: Parcial alto en `PortalCliente`; no completo en `WebCliente`.

Evidencia:

- `dotnet_webapps/WebCliente/Lineas.aspx`
- `dotnet_webapps/WebCliente/Lineas.aspx.cs`
- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/Index.cshtml`
- `dotnet_webservices/PortalCliente/Services/ProveedorClienteSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/IProveedorClienteService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- `WebCliente` tiene pantalla `Lineas.aspx`, pero carga grids vacios.
- `PortalCliente` consulta lineas por identificacion usando
  `WS_ProveedorCliente`.
- Separa prepago y postpago.
- Muestra saldo prepago y factura pendiente postpago.
- Implementa doble clic hacia recarga o pago.
- `WS_ProveedorCliente` compila y consulta SQL Server desde el servicio.

Pendientes o riesgos:

- La implementacion completa no esta integrada a `WebCliente`.
- `PortalCliente` solicita identificacion manual, no reutiliza la sesion del
  login cliente.
- Se debe confirmar en demo que el servicio tiene datos reales y cifrado
  compatible para `identificacion_dueno_cifrada`.

Nivel de cumplimiento estimado: 70%.

### 7.2 CLIENTE5 - Cargar saldo a linea prepago

Estado encontrado: Parcial alto en `PortalCliente`; no completo en `WebCliente`.

Evidencia:

- `dotnet_webapps/WebCliente/Portal.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx.cs`
- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/CargarSaldo.cshtml`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`
- `java_proveedor/src/database/ServicioDAO.java`
- `python_identificador/app/sockets/handler.py`

Lo que ya esta:

- `PortalCliente` lista lineas prepago del cliente.
- Valida que la linea pertenezca al cliente.
- Tiene formulario de tarjeta.
- Valida numero de tarjeta de 12 digitos, nombre, vencimiento MM/AA no vencido,
  CVV de 3 digitos y monto positivo.
- Consume `WS_ProveedorCliente.RecargarSaldo`.
- `LineaClienteService.Recargar` actualiza saldo en SQL Server.

Pendientes o riesgos:

- `WebCliente` solo muestra pantalla reservada para esta historia.
- El PDF pide que el proceso se realice por WS; esto se cumple en
  `PortalCliente`, pero debe declararse como ruta oficial.
- La validacion de monto dice "sin decimales", pero en backend solo valida
  `monto > 0`; la restriccion de entero depende del input HTML `step=1`.

Nivel de cumplimiento estimado: 75%.

### 7.3 CLIENTE6 - Pagar factura postpago

Estado encontrado: Parcial alto en `PortalCliente`; no completo en `WebCliente`.

Evidencia:

- `dotnet_webapps/WebCliente/Portal.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx.cs`
- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/PagarFactura.cshtml`
- `dotnet_webservices/PortalCliente/Services/EmailService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- `PortalCliente` lista lineas postpago.
- Permite pagar solo si hay factura pendiente.
- El monto se toma del servicio y se muestra como no editable.
- Valida datos de tarjeta con la misma funcion de validacion usada para recarga.
- Consume `WS_ProveedorCliente.PagarFactura`.
- `LineaClienteService.PagarFactura` cancela la factura poniendo
  `total_facturar = 0.00`.
- `EmailService` intenta enviar correo SMTP con detalle de pago.

Pendientes o riesgos:

- `WebCliente` solo muestra pantalla reservada.
- SMTP esta sin credenciales en `appsettings.json`, por lo que el correo no
  saldra en demo a menos que se configure.
- El pago cancela la deuda en SQL Server, pero no se observo una tabla historica
  de pagos separada.

Nivel de cumplimiento estimado: 70%.

### 7.4 CLIENTE7 - Devolucion de linea por cliente

Estado encontrado: Parcial alto en `PortalCliente`; no completo en `WebCliente`.

Evidencia:

- `dotnet_webapps/WebCliente/Portal.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx.cs`
- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/DevolverLinea.cshtml`
- `dotnet_webservices/PortalCliente/Services/Proveedor2SoapClient.cs`
- `dotnet_webservices/PortalCliente/Services/ProveedorPortalSoapClient.cs`
- `docs/evidencias/gabriel/CLIENTE7/CLIENTE7_devolucion_linea.md`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- `PortalCliente` lista lineas prepago y postpago.
- Consulta saldo prepago por WS Proveedor.
- Para postpago valida factura pendiente antes de devolver.
- Si hay deuda bloquea la devolucion con mensaje.
- Construye solicitud con numero, identificadores, tipo, identificacion cifrada y
  estado `disponible`.
- Consume `WS_PROVEEDOR2` mediante `Proveedor2SoapClient`.
- Existe evidencia especifica de CLIENTE7.

Pendientes o riesgos:

- `WebCliente` solo muestra pantalla reservada.
- El PDF habla de desactivar/inactivo; el proyecto devuelve a `disponible`. Debe
  explicarse como regla interna de inventario.
- La confirmacion se hace con `confirm()` del navegador; funcionalmente cumple,
  pero es basica.

Nivel de cumplimiento estimado: 75%.

### 7.5 Resultado Gabriel

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| CLIENTE4 | Parcial alto en portal separado | 70% | Integrar con login/plantilla oficial o declarar `PortalCliente` como portal oficial. |
| CLIENTE5 | Parcial alto en portal separado | 75% | Configurar demo real, reforzar validacion de monto entero en backend e integrar ruta oficial. |
| CLIENTE6 | Parcial alto en portal separado | 70% | Configurar SMTP/evidencia de correo e integrar ruta oficial. |
| CLIENTE7 | Parcial alto en portal separado | 75% | Evidencia integral y explicar estado `disponible` frente al PDF. |

Riesgo principal: medio-alto. Gabriel ya tiene implementacion defendible en
`PortalCliente`, pero debe evitar que el evaluador abra `WebCliente` y encuentre
solo placeholders.

## 8. Estado de Web Services requeridos por estrategia 3

| Servicio / necesidad | Estado | Historias afectadas |
|---|---|---|
| WS_AUTENTICACION1 - login por tipo | Funcional | ADM1, CLIENTE1 |
| WS_AUTENTICACION2 - CRUD/cambio estado usuarios | Funcional | ADM7, CLIENTE3 |
| WS_PROVEEDOR1 - registrar nueva linea | Parcial/no integrado a ADM3 | ADM3 |
| WS_PROVEEDOR2 - activar/desactivar linea | Funcional en backend; usado por PortalCliente para CLIENTE7 | ADM4, ADM5, CLIENTE7 |
| WS_PROVEEDOR3 - calcular facturacion | Funcional para ADM6 | ADM6 |
| WS_ProveedorCliente - consultar lineas | Funcional parcial y compila | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |
| WS_ProveedorCliente - recargar saldo | Funcional parcial y compila | CLIENTE5 |
| WS_ProveedorCliente - pagar factura | Funcional parcial y compila | CLIENTE6 |
| Enviar correo de factura | Parcial | CLIENTE6 |
| Eliminar linea disponible | Faltante/no integrado | ADM3 |
| Listados administrativos de lineas disponibles/en uso | Faltante/no integrado en Web final | ADM3, ADM4, ADM5 |

## 9. Lo que ya esta defendible

- Compilacion principal de WebApps y servicios usados por persona 2.
- Compilacion de `WS_ProveedorCliente` y `PortalCliente`.
- Login administrativo y cliente con MongoDB por WS Autenticacion.
- Registro de cliente.
- CRUD de administradores.
- Calculo de facturacion postpago.
- Consulta, recarga, pago y devolucion de cliente en `PortalCliente`.
- Proveedor Java con movimientos, bitacora y facturacion.
- Identificador Python con bitacora, consulta de saldo y sincronizacion.
- Scripts de preparacion y levantamiento de procesos.
- Seed de persona 2 y guia de ejecucion documentada.

## 10. Lo que falta para cierre defendible

Prioridad alta:

1. Definir oficialmente si el portal cliente final sera `WebCliente` o
   `PortalCliente`.
2. Si se usa `PortalCliente`, enlazarlo desde el login de `WebCliente` o migrar
   CLIENTE1-CLIENTE3 al portal ASP.NET Core.
3. Si se mantiene `WebCliente`, reemplazar `Lineas.aspx` y `Portal.aspx` por
   consumo real de `WS_ProveedorCliente`.
4. Decidir si ADM3-ADM5 se terminan en `dotnet_webapps/WebAdministrativo` o en
   `dotnet_webservices/WebAdministrativa`.
5. Restaurar/corregir build de `WebAdministrativa` si sera la ruta oficial.
6. Conectar ADM3-ADM5 a WS Proveedor real.
7. Agregar o exponer listados administrativos de lineas disponibles/en uso.
8. Configurar SMTP real para CLIENTE6 o documentar evidencia controlada.
9. Capturar evidencias finales por historia y por responsable.

Prioridad media:

1. Agregar logo/icono real a plantillas.
2. Homologar textos visibles con los mensajes exactos del PDF.
3. Documentar que las WebApps no acceden directo a BD/socket.
4. Explicar que las lineas devueltas pasan a `DISPONIBLE` aunque el PDF use el
   termino inactivo.
5. Preparar una guia unica de demo para evitar abrir aplicaciones incompletas.

## 11. Recomendacion por responsable

### Charlie

Ruta corta para cierre:

1. Completar el menu ADM2 con ADM3, ADM4 y ADM5.
2. Elegir una sola Web Administrativa oficial.
3. Conectar nuevas lineas, activacion y devolucion administrativa contra WS
   Proveedor.
4. Validar que cada pantalla muestre los mensajes exactos del PDF.
5. Tomar evidencia de login, plantilla, nueva linea, activacion y devolucion.

### Jose

Ruta corta para cierre:

1. Mantener estables ADM6, ADM7, CLIENTE1, CLIENTE2 y CLIENTE3.
2. Completar evidencias de pruebas exitosas y fallidas.
3. Decidir junto con Gabriel como se hara el salto de CLIENTE1 a CLIENTE4.
4. Ajustar CLIENTE2 si `PortalCliente` pasa a ser el portal oficial.

### Gabriel

Ruta corta para cierre:

1. Declarar `PortalCliente` como implementacion oficial de CLIENTE4-CLIENTE7 o
   integrar su logica en `WebCliente`.
2. Hacer que el portal reciba/reuse la identificacion del cliente autenticado.
3. Configurar SMTP y generar evidencia del correo de CLIENTE6.
4. Ejecutar prueba integral con `WS_ProveedorCliente`, `WS_PROVEEDOR2` y datos
   reales.
5. Documentar claramente que `WebCliente/Portal.aspx` ya no es la ruta de demo si
   se mantiene como placeholder.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra historias asignadas en `estrategia_3.md`:

```text
Charlie : [#####-----] 45% de 100
Jose    : [########--] 84% de 100
Gabriel : [#######---] 73% de 100
```

Lectura rapida:

- Charlie: tiene login y parte de plantilla, pero debe cerrar ADM3-ADM5.
- Jose: tiene el bloque mas completo; su mayor riesgo es la integracion del
  portal cliente oficial.
- Gabriel: subio mucho por `PortalCliente`, pero debe unificar o declarar la
  ruta oficial y cerrar evidencia de correo/datos reales.

## 13. Conclusion

El proyecto esta mas avanzado de lo que indicaba el informe anterior porque
`PortalCliente` y `WS_ProveedorCliente` agregan una implementacion real para gran
parte de CLIENTE4-CLIENTE7. Aun asi, el proyecto no debe presentarse como
cerrado sin antes resolver la fragmentacion: la Web Cliente principal muestra
placeholders, la Web Administrativa MVC no compila y ADM3-ADM5 no consumen WS
real.

La prioridad real para el cierre ya no es rehacer el backend. La prioridad es
ordenar la ruta oficial de demo, integrar o retirar las pantallas incompletas y
cerrar el bloque administrativo de lineas. Si el equipo enfoca el esfuerzo en
ADM3-ADM5, integracion WebCliente/PortalCliente y evidencias finales, el estado
puede pasar de parcial avanzado a defendible integral.
