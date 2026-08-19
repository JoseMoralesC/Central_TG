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

La deuda principal detectada era fragmentacion e integracion. En esta revision
se comenzo a cerrar: `WebCliente` redirige al `PortalCliente` oficial despues
del login, y la Web administrativa principal incorporo ADM3-ADM5 consumiendo
operaciones nuevas del WS Proveedor. La WebAdministrativa MVC separada sigue
siendo no oficial y no debe usarse como ruta de demo.

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
| Identificador Python | Parcial alto | Atiende llamadas, saldo, bitacora, catalogo y sincronizacion; el catalogo consulta MySQL real y ya no queda limitado a datos locales del simulador. |
| Proveedor Java | Parcial alto | Tiene verificacion, movimientos, bitacora, recarga, registro/cambio de estado y facturacion; el arranque local quedo estabilizado en puerto 6000. |
| WS Autenticacion | Cumple funcionalmente | Login, creacion, modificacion, cambio de estado, listado y eliminacion de usuarios en MongoDB. |
| WS Proveedor WCF | Parcial alto | Expone activar/desactivar, calcular facturacion, ultima facturacion, listados administrativos, registro y eliminacion de lineas disponibles. |
| WS ProveedorCliente WCF | Parcial alto | Compila y expone consulta de lineas, recarga y pago; no expone devolucion propia ni correo directo. |
| Web Administrativo principal | Parcial alto | Tiene ADM1-ADM7 en la Web final integrada; ADM3-ADM5 dependen de prueba con datos reales. |
| Web Cliente principal | Parcial alto | Tiene CLIENTE1-CLIENTE3 y redirige a PortalCliente para CLIENTE4-CLIENTE7. |
| PortalCliente ASP.NET Core | Parcial alto para Gabriel | Implementa CLIENTE4-CLIENTE7 con WS y acepta identificacion enviada por WebCliente. |
| WebAdministrativa MVC | Parcial bajo | Existe para ADM3-ADM5, pero no compila y no consume WS real. |
| Documentacion y evidencias | Parcial alto | Hay guias y evidencias; falta consolidar una ruta oficial de demo por historia. |

Resultado general: Jose sigue siendo el bloque mas defendible. Gabriel queda con
ruta oficial mas clara gracias al puente `WebCliente -> PortalCliente`. Charlie
sube de riesgo alto a medio porque ADM3-ADM5 ya existen en la Web principal,
aunque deben validarse con datos reales y procesos levantados. El cierre ahora
depende menos de construir piezas nuevas y mas de estabilizar la corrida
integrada, usar MongoDB real levantado manualmente y documentar evidencia.

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
| Revision de `dotnet_webapps/WebCliente` | CLIENTE1-CLIENTE3 compilan y el login redirige a `PortalCliente` para CLIENTE4-CLIENTE7. |
| Revision de `dotnet_webservices/PortalCliente` | CLIENTE4-CLIENTE7 tienen implementacion funcional parcial/alta. |
| Revision de `dotnet_webservices/WebAdministrativa` | Acciones de ADM3-ADM5 son placeholders y no consumen WS real. |
| `python -m py_compile` sobre configuracion/repositorio/servicio del Identificador | Correcto; no hay errores de sintaxis en los cambios de catalogo. |
| Prueba directa de catalogo Python contra MySQL `central_identificador` | Correcta; devuelve 20 telefonos reales y no se cae si Java esta temporalmente no disponible. |
| Prueba de proveedor Java en primer plano | Correcta; conecta a SQL Server y escucha en `127.0.0.1:6000`. |
| Build del simulador C# con `--no-restore` | Correcto; la lectura de socket queda preparada para respuestas completas con salto de linea. |
| Revision de `scripts/persona2-levantar.ps1` | Correcta; MongoDB ya no se levanta ni aplica datos semilla desde el script, las ventanas usan el nombre real del proceso e IIS Express evita duplicar puertos activos. |

Observacion: la corrida oficial debe usar bases reales ya preparadas. MongoDB se
mantiene fuera del script de levantamiento y debe estar iniciado manualmente en
`localhost:27017`; el script solo valida si el puerto responde.

## 4. Hallazgos transversales

### 4.1 La arquitectura base esta viva

El sistema ya tiene varios flujos operativos:

```text
WebApps / PortalCliente / Simulador C# -> WS -> Python Identificador / Java Proveedor -> Bases de datos
```

Puerto operativo recomendado para `PortalCliente`: `http://localhost:56123`. El
puerto `5000` queda reservado para el socket TCP del Identificador Python.
Por eso `http://localhost:5000/Cliente/Index` no es una URL valida del portal y
puede mostrar `ERR_INVALID_HTTP_RESPONSE` en navegador.

Ruta cliente recomendada para demo:

```text
WebCliente Login: http://localhost:56122/Login.aspx
Destino posterior: http://localhost:56123/Cliente/Index
```

Tambien existen scripts y guias de apoyo:

- `scripts/persona2-preparar.ps1`
- `scripts/persona2-levantar.ps1`
- `docs/roadmaps/guia_ejecucion_persona_2_web.md`
- `docs/entrega_final/Ruta_demo_alcance_final.md`
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
capacidades que ahora llegan a las rutas oficiales principales. El caso cliente
se resolvio mediante redireccion desde `WebCliente` hacia `PortalCliente`.

### 4.4 ADM3-ADM5 ya estan en la Web oficial, pero requieren evidencia

La WebAdministrativa MVC tiene pantallas visuales para nuevas lineas, activar y
desactivar, pero no compila y sus acciones dicen que la integracion con Web
Service sera en una siguiente etapa. Esa aplicacion no debe usarse como ruta de
demo.

La ruta oficial actual es `dotnet_webapps/WebAdministrativo`, donde ya existen
las paginas `LineasNuevas.aspx`, `LineasActivar.aspx` y
`LineasDevolucion.aspx`, conectadas al `WS_Proveedor`. El riesgo que queda no es
de ausencia de pantalla, sino de prueba integral con SQL Server, Java, Python y
datos reales.

### 4.5 Datos reales y arranque local

La configuracion de demo quedo mas cercana a datos reales:

- Python Identificador apunta por defecto a MySQL `central_identificador`.
- El catalogo telefonico usa `LEFT JOIN` con proveedores para no perder
  telefonos cuando falte detalle de proveedor.
- El simulador C# lee respuestas completas del socket; esto evita mostrar solo
  los 4 telefonos locales cuando Python devuelve un catalogo mas grande.
- El proveedor Java queda en `127.0.0.1:6000` y el script lo espera antes de la
  prueba.
- MongoDB debe levantarse manualmente con la base real. No se debe depender de
  seeds automaticos para defensa.
- IIS Express ya no intenta abrir otra instancia cuando el puerto esta ocupado;
  reporta el proceso como activo.

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
  sitio y ya incluye las opciones administrativas principales.
- Falta evidencia final formal de login exitoso y fallido.

Nivel de cumplimiento estimado: 85%.

### 5.2 ADM2 - Plantilla y administracion de clientes

Estado encontrado: Parcial alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Site.Master`
- `dotnet_webapps/WebAdministrativo/Site.Master.cs`

Lo que ya esta:

- Existe plantilla con barra superior.
- Existe opcion de salir del sitio.
- Existe footer.
- El menu esta disponible en paginas administrativas actuales.
- El menu oficial ya incluye nuevas lineas, activar linea, devolucion de linea,
  calcular facturacion y administradores.
- ADM3-ADM7 quedan accesibles desde la Web Administrativa principal.

Pendientes o riesgos:

- No se observa icono/logo de empresa como imagen; solo marca textual.
- Falta evidencia final de navegacion completa por el menu.

Nivel de cumplimiento estimado: 72%.

### 5.3 ADM3 - Poner nuevas lineas a disposicion

Estado encontrado: Parcial medio/alto.

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
- La WebAdministrativo principal ahora incluye `LineasNuevas.aspx`.
- `WS_Proveedor` expone `ListarLineasDisponibles`, `RegistrarLinea` y
  `EliminarLineaDisponible`.

Pendientes o riesgos:

- La MVC separada no compila y queda fuera de la ruta oficial.
- Falta prueba integral con SQL Server real y datos de defensa.
- Falta evidencia visual de alta y eliminacion.

Nivel de cumplimiento estimado: 70%.

### 5.4 ADM4 - Activar linea vendida

Estado encontrado: Parcial medio/alto.

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
- La WebAdministrativo principal ahora incluye `LineasActivar.aspx`.
- La pantalla lista disponibles desde WS y permite ingresar cedula del cliente.

Pendientes o riesgos:

- Falta prueba integral desde Web -> WS -> Java -> Python -> SQL/MySQL.
- Falta evidencia visual de activacion exitosa y error.

Nivel de cumplimiento estimado: 72%.

### 5.5 ADM5 - Devolucion/desactivacion administrativa de linea

Estado encontrado: Parcial medio/alto.

Evidencia:

- `dotnet_webservices/WebAdministrativa/Controllers/LineasController.cs`
- `dotnet_webservices/WebAdministrativa/Views/Lineas/Desactivar.cshtml`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`
- `database/sqlserver_proveedor/migrations/013_normalizar_estado_linea_disponible.sql`

Lo que ya esta:

- Backend soporta desactivacion.
- Se normalizo el estado de linea hacia `ACTIVO` y `DISPONIBLE`.
- La WebAdministrativo principal ahora incluye `LineasDevolucion.aspx`.
- La pantalla lista lineas activas desde WS y confirma antes de devolver.

Pendientes o riesgos:

- El PDF habla de estado inactivo; el proyecto usa `DISPONIBLE` para devolver
  lineas. Esto debe explicarse en defensa.
- Falta prueba integral con Java/Python levantados.

Nivel de cumplimiento estimado: 72%.

### 5.6 Resultado Charlie

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM1 | Funcional | 85% | Evidencia final y ajustar destino/documentacion del flujo. |
| ADM2 | Parcial alto | 72% | Logo/icono real y evidencia de navegacion completa por el menu. |
| ADM3 | Parcial alto | 70% | Prueba integral con datos reales y evidencia final. |
| ADM4 | Parcial alto | 72% | Prueba integral Web -> WS -> Java/Python y evidencia final. |
| ADM5 | Parcial alto | 72% | Prueba integral Web -> WS -> Java/Python y explicar estado DISPONIBLE. |

Riesgo principal: medio. El bloque de administracion de lineas ya esta en la Web
final, pero requiere validacion integral con servicios y bases levantadas.

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
- MongoDB real en `localhost:27017`

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
- Si credenciales son correctas redirige al `PortalCliente` con la
  identificacion autenticada.
- Si son incorrectas muestra mensaje esperado.

Pendientes o riesgos:

- Falta evidencia final de caso exitoso y fallido.

Nivel de cumplimiento estimado: 90%.

### 6.4 CLIENTE2 - Plantilla portal cliente

Estado encontrado: Parcial alto.

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
- El login de `WebCliente` funciona como puerta de entrada oficial.
- `PortalCliente` funciona como pantalla transaccional posterior al login, no
  como duplicado de `WebCliente`.

Pendientes o riesgos:

- `WebCliente` tiene la plantilla conectada al login y envia al portal oficial.
- `PortalCliente` tiene las historias transaccionales y acepta identificacion
  recibida desde `WebCliente`.
- Falta icono/logo real de empresa; se usa marca textual.

Nivel de cumplimiento estimado: 82%.

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
- El registro queda enlazado indirectamente: despues de login, `WebCliente`
  redirige a `PortalCliente` con la identificacion autenticada.

Nivel de cumplimiento estimado: 86%.

### 6.6 Resultado Jose

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM6 | Funcional | 90% | Evidencia final y asegurar migraciones aplicadas. |
| ADM7 | Funcional | 90% | Evidencia final CRUD completo. |
| CLIENTE1 | Funcional | 90% | Evidencia login exitoso/fallido. |
| CLIENTE2 | Parcial alto | 82% | Logo real y evidencia de navegacion WebCliente -> PortalCliente. |
| CLIENTE3 | Funcional | 86% | Evidencia final y confirmar segundo apellido. |

Riesgo principal: bajo/medio. El bloque de Jose funciona y el salto hacia
`PortalCliente` ya esta definido; falta evidencia formal y confirmar MongoDB
real levantado con las credenciales esperadas por el WS.

## 7. Gabriel - CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7

Responsabilidad segun estrategia 3: autogestion transaccional del cliente.

### 7.1 CLIENTE4 - Mostrar lineas asociadas al cliente

Estado encontrado: Parcial alto en `PortalCliente`, integrado como ruta oficial desde `WebCliente`.

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

- `WebCliente` redirige a `PortalCliente` con la identificacion autenticada.
- `PortalCliente` aun conserva formulario manual como respaldo si entra directo.
- Se debe confirmar en demo que el servicio tiene datos reales y cifrado
  compatible para `identificacion_dueno_cifrada`.

Nivel de cumplimiento estimado: 82%.

### 7.2 CLIENTE5 - Cargar saldo a linea prepago

Estado encontrado: Parcial alto en `PortalCliente`, integrado como ruta oficial desde `WebCliente`.

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

- El PDF pide que el proceso se realice por WS; esto se cumple en
  `PortalCliente`, pero debe declararse como ruta oficial.
- La validacion de monto dice "sin decimales", pero en backend solo valida
  `monto > 0`; la restriccion de entero depende del input HTML `step=1`.

Nivel de cumplimiento estimado: 82%.

### 7.3 CLIENTE6 - Pagar factura postpago

Estado encontrado: Parcial alto en `PortalCliente`, integrado como ruta oficial desde `WebCliente`.

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

- SMTP esta sin credenciales en `appsettings.json`, por lo que el correo no
  saldra en demo a menos que se configure.
- El pago cancela la deuda en SQL Server, pero no se observo una tabla historica
  de pagos separada.

Nivel de cumplimiento estimado: 78%.

### 7.4 CLIENTE7 - Devolucion de linea por cliente

Estado encontrado: Parcial alto en `PortalCliente`, integrado como ruta oficial desde `WebCliente`.

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

- El PDF habla de desactivar/inactivo; el proyecto devuelve a `disponible`. Debe
  explicarse como regla interna de inventario.
- La confirmacion se hace con `confirm()` del navegador; funcionalmente cumple,
  pero es basica.

Nivel de cumplimiento estimado: 82%.

### 7.5 Resultado Gabriel

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| CLIENTE4 | Parcial alto integrado | 82% | Evidencia con datos reales. |
| CLIENTE5 | Parcial alto integrado | 82% | Configurar demo real y reforzar validacion de monto entero en backend si se desea. |
| CLIENTE6 | Parcial alto integrado | 78% | Configurar SMTP/evidencia de correo. |
| CLIENTE7 | Parcial alto integrado | 82% | Evidencia integral y explicar estado `disponible` frente al PDF. |

Riesgo principal: medio. Gabriel ya tiene implementacion defendible en
`PortalCliente` y `WebCliente` redirige hacia esa ruta; falta evidencia integral
con datos reales.

## 8. Estado de Web Services requeridos por estrategia 3

| Servicio / necesidad | Estado | Historias afectadas |
|---|---|---|
| WS_AUTENTICACION1 - login por tipo | Funcional | ADM1, CLIENTE1 |
| WS_AUTENTICACION2 - CRUD/cambio estado usuarios | Funcional | ADM7, CLIENTE3 |
| WS_PROVEEDOR1 - registrar nueva linea | Integrado en WS Proveedor/WebAdministrativo | ADM3 |
| WS_PROVEEDOR2 - activar/desactivar linea | Funcional en backend; usado por WebAdministrativo y PortalCliente | ADM4, ADM5, CLIENTE7 |
| WS_PROVEEDOR3 - calcular facturacion | Funcional para ADM6 | ADM6 |
| WS_ProveedorCliente - consultar lineas | Funcional parcial y compila | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |
| WS_ProveedorCliente - recargar saldo | Funcional parcial y compila | CLIENTE5 |
| WS_ProveedorCliente - pagar factura | Funcional parcial y compila | CLIENTE6 |
| Enviar correo de factura | Parcial | CLIENTE6 |
| Eliminar linea disponible | Integrado en WS Proveedor/WebAdministrativo | ADM3 |
| Listados administrativos de lineas disponibles/en uso | Integrado en WS Proveedor/WebAdministrativo | ADM3, ADM4, ADM5 |

## 9. Lo que ya esta defendible

- Compilacion principal de WebApps y servicios del alcance final.
- Compilacion de `WS_ProveedorCliente` y `PortalCliente`.
- Login administrativo y cliente con MongoDB por WS Autenticacion.
- Registro de cliente.
- CRUD de administradores.
- Calculo de facturacion postpago.
- Consulta, recarga, pago y devolucion de cliente en `PortalCliente`.
- Proveedor Java con movimientos, bitacora y facturacion.
- Identificador Python con bitacora, consulta de saldo y sincronizacion.
- Scripts de preparacion y levantamiento de procesos.
- Guia de ejecucion y ruta oficial de demo documentadas.
- MongoDB separado del script de levantamiento para trabajar con datos reales.
- IIS Express protegido contra puertos ya ocupados.
- Catalogo telefonico desde MySQL real, sin fallback visible a 4 telefonos del
  simulador cuando Python devuelve mas registros.

## 10. Lo que falta para cierre defendible

Prioridad alta:

1. Levantar MongoDB manualmente como administrador con datos reales y confirmar
   que `localhost:27017` responde antes de probar login/usuarios.
2. Ejecutar la demo cliente desde `http://localhost:56122/Login.aspx`; el
   `PortalCliente` en `http://localhost:56123/Cliente/Index` es el destino
   transaccional posterior al login.
3. Confirmar en cada corrida que Java proveedor escuche en `127.0.0.1:6000` y
   que Python Identificador escuche en `127.0.0.1:5000`.
4. Ejecutar prueba integral de ADM3-ADM5 con SQL Server, Java proveedor, Python
   identificador y MySQL levantados.
5. Ejecutar prueba integral de CLIENTE4-CLIENTE7 desde login de WebCliente hasta
   PortalCliente con datos reales.
6. Configurar SMTP real para CLIENTE6 o usar la evidencia controlada del mensaje
   de falta de configuracion SMTP.
7. Capturar evidencias finales por historia y por responsable.

Prioridad media:

1. Agregar logo/icono real a plantillas.
2. Homologar textos visibles con los mensajes exactos del PDF.
3. Documentar que las WebApps no acceden directo a BD/socket.
4. Explicar que las lineas devueltas pasan a `DISPONIBLE` aunque el PDF use el
   termino inactivo.
5. Mantener una guia unica de demo para evitar abrir aplicaciones incompletas.

## 11. Recomendacion por responsable

### Charlie

Ruta corta para cierre:

1. Usar `dotnet_webapps/WebAdministrativo` como Web Administrativa oficial.
2. Validar ADM3, ADM4 y ADM5 contra `WS_Proveedor`, Java, Python y bases reales.
3. Verificar que ADM2 muestre todas las opciones esperadas del menu.
4. Ajustar mensajes visibles que no coincidan con el PDF.
5. Tomar evidencia de login, plantilla, nueva linea, activacion y devolucion.

### Jose

Ruta corta para cierre:

1. Mantener estables ADM6, ADM7, CLIENTE1, CLIENTE2 y CLIENTE3.
2. Completar evidencias de pruebas exitosas y fallidas.
3. Validar que `WS_Autenticacion` conecte contra MongoDB real levantado
   manualmente.
4. Documentar que `WebCliente` es entrada/login y `PortalCliente` es la pantalla
   transaccional posterior.

### Gabriel

Ruta corta para cierre:

1. Usar `PortalCliente` como implementacion oficial de CLIENTE4-CLIENTE7.
2. Entrar desde `WebCliente/Login.aspx` para demostrar la integracion.
3. Configurar SMTP y generar evidencia del correo de CLIENTE6.
4. Ejecutar prueba integral con `WS_ProveedorCliente`, `WS_PROVEEDOR2` y datos
   reales.
5. Mantener `PortalCliente` como destino de navegacion, evitando demostrar rutas
   placeholder antiguas de `WebCliente`.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra historias asignadas en `estrategia_3.md`:

```text
Charlie : [########--] 76% de 100
Jose    : [#########-] 89% de 100
Gabriel : [########--] 84% de 100
```

Lectura rapida:

- Charlie: ya tiene ADM3-ADM5 en la Web final y ADM2 enlaza las opciones clave;
  falta prueba integral y evidencia.
- Jose: tiene el bloque mas completo; su mayor riesgo es MongoDB real y la
  evidencia formal de flujo cliente.
- Gabriel: tiene ruta oficial desde `WebCliente` hacia `PortalCliente`; falta
  evidencia de correo/datos reales.

## 13. Conclusion

El proyecto esta mas avanzado de lo que indicaba el informe anterior porque
`PortalCliente` y `WS_ProveedorCliente` agregan una implementacion real para gran
parte de CLIENTE4-CLIENTE7. En esta revision tambien se cerro la fragmentacion
principal: `WebCliente` redirige al portal transaccional y `WebAdministrativo`
incluye ADM3-ADM5 contra WS Proveedor.

La prioridad real para el cierre ya no es rehacer el backend. La prioridad es
ejecutar pruebas integrales con datos reales y capturar evidencias finales. El
proyecto ya tiene una ruta defendible: WebAdministrativo para ADM1-ADM7,
WebCliente para login/registro cliente y PortalCliente para transacciones. Si el
equipo enfoca el esfuerzo en validar ADM3-ADM5, CLIENTE4-CLIENTE7, MongoDB real
y SMTP, el estado puede pasar de parcial avanzado a defendible integral.
