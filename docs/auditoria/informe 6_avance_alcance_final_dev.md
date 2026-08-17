# Informe 6 de avance - Alcance final contra proyecto actual

Fecha de analisis: 2026-08-17  
Rama revisada: arbol local actual del repositorio `Central_TG`  
Documentos base: PDF `docs/Proyecto/Proyecto final.pdf`, `docs/estrategia/estrategia_3.md`, diagnostico tecnico actual del proyecto

## 1. Resumen ejecutivo

El proyecto se encuentra en un estado funcional parcial avanzado. La integracion
base del sistema ya existe y varios componentes principales compilan: simulador
C#, identificador Python, proveedor Java, WS Autenticacion, WS Proveedor WCF y
las WebApps principales de persona 2. Sin embargo, comparado contra el alcance
final del PDF, el proyecto todavia no esta listo como entrega completa porque
faltan pantallas y endpoints Web para una parte importante de las historias.

La revision contra `estrategia_3.md` muestra que el reparto acordado para el
alcance final es:

| Companero | Nombre | Historias asignadas |
|---|---|---|
| Companero 1 | Charlie | ADM1, ADM2, ADM3, ADM4, ADM5 |
| Companero 2 | Jose | ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3 |
| Companero 3 | Gabriel | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |

Estado global estimado:

| Area | Estado | Comentario |
|---|---|---|
| Simulador C# | Cumple como base | Compila y forma parte del flujo integrado C# -> Python -> Java. |
| Identificador Python | Parcial alto | Atiende llamadas, saldo, bitacora, catalogo y sincronizacion de lineas. |
| Proveedor Java | Parcial alto | Tiene verificacion, movimientos, bitacora, recarga, registro/cambio de estado y facturacion. |
| WS Autenticacion | Cumple funcionalmente para WebApps | Login, creacion, modificacion, cambio de estado, listado y eliminacion de usuarios en MongoDB. |
| WS Proveedor WCF | Parcial | Expone activacion/desactivacion, calculo de facturacion y ultima facturacion; faltan consultas/recarga/pago/devolucion para cliente. |
| Web Administrativo principal | Parcial medio | Tiene ADM1, parte de ADM2, ADM6 y ADM7; faltan ADM3-ADM5 en la Web final integrada. |
| Web Cliente principal | Parcial medio-bajo | Tiene CLIENTE1, CLIENTE2 basico y CLIENTE3; CLIENTE4-CLIENTE7 estan en placeholder. |
| WebAdministrativa MVC integrada | Parcial bajo | Existe para ADM3-ADM5, pero no compila por paquete NuGet faltante y sus acciones no consumen WS real. |
| Documentacion y scripts | Parcial alto | Hay guias, scripts de preparacion/arranque y pruebas de persona 2; falta consolidar evidencia final por historia. |

Resultado general: el bloque de Jose es el mas defendible actualmente. Charlie
tiene parte de su alcance final cubierto por login/plantilla, pero ADM3-ADM5
requieren integracion real. Gabriel tiene el bloque con mayor deuda actual:
CLIENTE4-CLIENTE7 aun no estan implementadas en la Web Cliente.

## 2. Requerimientos fuente revisados

El PDF de alcance final exige catorce historias principales:

| Bloque segun estrategia 3 | Historias |
|---|---|
| Companero 1 - Charlie | ADM1, ADM2, ADM3, ADM4, ADM5 |
| Companero 2 - Jose | ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3 |
| Companero 3 - Gabriel | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |

Reglas tecnicas obligatorias del PDF:

- La aplicacion Web Administrativa debe estar desarrollada en C#.
- La aplicacion Web Cliente puede estar en C# u otro lenguaje; el proyecto actual la tiene en C# Web Forms.
- Las aplicaciones Web no deben acceder directo a base de datos ni a sockets.
- Toda operacion Web debe pasar por Web Services.
- Los Web Services pueden ampliarse o crearse segun necesidad.
- Deben mantenerse actualizados Proveedor, Identificador, Simulador, WS Proveedor y WS Autenticacion.
- Debe existir documentacion completa de analisis, diseno, diagramas y evidencias.

## 3. Verificaciones ejecutadas

| Verificacion | Resultado |
|---|---|
| Extraccion del PDF `Proyecto final.pdf` con `pypdf` | Correcta; se confirmaron ADM1-ADM7 y CLIENTE1-CLIENTE7. |
| Lectura de `docs/estrategia/estrategia_3.md` | Correcta; se confirmo reparto Companero 1/2/3. |
| `powershell -File scripts/persona2-preparar.ps1` | Correcto para WebApps, WS Autenticacion, WS Proveedor WCF, simulador C# y Java. |
| Build de `dotnet_webservices/WebAdministrativa/WebAdministrativa.sln` | Falla por paquete NuGet faltante `Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1`. |
| Revision de WebCliente | CLIENTE4-CLIENTE7 estan como listas vacias o pantalla reservada. |
| Revision de WebAdministrativa MVC | Acciones de ADM3-ADM5 son placeholders y no consumen Web Service real. |

Observacion: la ejecucion de `scripts/persona2-preparar.ps1` no aplico seeds ni
migraciones, de acuerdo con la configuracion actual del script.

## 4. Hallazgos transversales

### 4.1 La arquitectura base esta viva

El sistema ya tiene flujo operativo entre componentes:

```text
WebApps / Simulador C# -> WS / Python Identificador -> Java Proveedor -> Bases de datos
```

Tambien existen scripts de apoyo:

- `scripts/persona2-preparar.ps1`
- `scripts/persona2-levantar.ps1`
- `docs/roadmaps/guia_ejecucion_persona_2_web.md`

Esto facilita defensa y pruebas, siempre que los procesos se levanten en orden.

### 4.2 Hay dos frentes Web administrativos

Actualmente hay:

- Web final principal: `dotnet_webapps/WebAdministrativo`.
- Web MVC integrada: `dotnet_webservices/WebAdministrativa`.

La Web final principal compila y ya se usa para ADM6/ADM7. La MVC parece apuntar
a ADM3-ADM5, pero no compila sin restaurar NuGet y sus controladores aun dicen
que la integracion con Web Service sera en una siguiente etapa.

Riesgo: si el equipo presenta ambas sin aclarar cual es la oficial, puede generar
confusion. Conviene elegir una ruta: integrar ADM3-ADM5 en `dotnet_webapps` o
terminar la MVC y conectarla realmente.

### 4.3 El mayor hueco del alcance final esta en Web Cliente

La Web Cliente cumple login, plantilla basica y registro. Pero las operaciones
transaccionales del cliente no estan listas:

- CLIENTE4: listas reales de lineas prepago/postpago.
- CLIENTE5: recarga con tarjeta.
- CLIENTE6: pago de factura y correo.
- CLIENTE7: devolucion de linea con validacion de deuda.

Estas historias dependen de ampliar `WS_Proveedor` con operaciones que todavia no
estan expuestas para las WebApps.

### 4.4 El backend tiene mas capacidad que la capa Web

Java y Python ya tienen soporte para varias operaciones que no estan visibles en
las aplicaciones Web finales: catalogo de telefonos, recarga, registro, cambio
de estado, consulta de saldo y movimientos. El trabajo pendiente no es partir de
cero, sino exponer y consumir esas funciones desde Web Services correctos.

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
- Contraseña viaja cifrada desde el cliente SOAP.
- Si credenciales son incorrectas, muestra mensaje equivalente a lo solicitado.
- Al autenticar redirige al area administrativa.

Pendientes o riesgos:

- El PDF indica que despues del login debe ir a ADM2, pantalla de administracion
  de clientes. Actualmente redirige a `Facturacion.aspx`, no a una pagina base
  con ADM3-ADM5.
- Falta evidencia final capturada de login exitoso y fallido.

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
- No se observa icono/logo de empresa como imagen; solo texto de marca.
- No existe pantalla administrativa principal de clientes/lineas dentro de esta Web.

Nivel de cumplimiento estimado: 45%.

### 5.3 ADM3 - Poner nuevas lineas a disposicion

Estado encontrado: Parcial bajo.

Evidencia:

- `dotnet_webservices/WebAdministrativa/Controllers/LineasController.cs`
- `dotnet_webservices/WebAdministrativa/Views/Lineas/Nuevas.cshtml`
- `dotnet_webservices/WebAdministrativa/Views/Lineas/Crear.cshtml`
- `java_proveedor/src/services/AdministracionTelefonica.java`
- `java_proveedor/src/database/ServicioDAO.java`

Lo que ya esta:

- Existe una WebAdministrativa MVC con rutas visuales para nuevas lineas.
- Existen clases de apoyo para registrar telefonos en Java.
- Existen contratos y servicios internos para catalogo/registro desde el flujo
  C# -> Python -> Java.

Pendientes o riesgos:

- La WebAdministrativa MVC no compila por paquete NuGet faltante.
- La accion `Crear` solo muestra mensaje de que se conectara al Web Service en
  una siguiente etapa.
- No se consume WS_PROVEEDOR1 desde la pantalla.
- No se lista realmente lineas disponibles desde WS.
- No se elimina linea con confirmacion usando WS.
- Esta funcionalidad no esta en la WebAdministrativo principal que se esta
  levantando con IIS Express.

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
- Falta asociar cedula de cliente desde pantalla final.
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
- Se corrigio la regla de estado de linea a `ACTIVO` y `DISPONIBLE`.
- Existe normalizacion SQL para evitar `INACTIVO` como estado de linea.

Pendientes o riesgos:

- El PDF usa texto de desactivar/inactivo, pero por alcance 2 se definio que la
  linea debe quedar disponible; esto debe explicarse en defensa.
- Falta pantalla final conectada.
- Falta listado real de lineas en uso.
- Falta confirmacion real antes de desactivar desde Web.

Nivel de cumplimiento estimado: 35%.

### 5.6 Resultado Charlie

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM1 | Funcional | 85% | Ajustar redireccion/documentar destino y tomar evidencia final. |
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
- Se probo visualmente con ultima facturacion cargada.

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

- La grilla muestra usuario/contrasena cifrados segun contrato; puede ser
  visualmente raro, pero coincide con datos almacenados cifrados.
- Falta evidencia formal final de CRUD completo.

Nivel de cumplimiento estimado: 90%.

### 6.3 CLIENTE1 - Login cliente

Estado encontrado: Cumple funcionalmente.

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

- Falta evidencia final de caso exitoso y fallido.

Nivel de cumplimiento estimado: 90%.

### 6.4 CLIENTE2 - Plantilla portal cliente

Estado encontrado: Parcial medio/alto.

Evidencia:

- `dotnet_webapps/WebCliente/Site.Master`
- `dotnet_webapps/WebCliente/Site.Master.cs`
- `dotnet_webapps/WebCliente/Lineas.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx`

Lo que ya esta:

- Menu con lineas activas, cargar saldo, pagar facturas, devolver linea y salir.
- Saludo `Hola` + nombre del cliente.
- Footer visible.
- Al ingresar redirige a `Lineas.aspx`, que corresponde a CLIENTE4.
- Navegacion persistente por Master Page.

Pendientes o riesgos:

- Falta icono/logo real de empresa; actualmente se ve marca textual.
- Las opciones del menu apuntan a pantallas placeholder para CLIENTE5-CLIENTE7.
- CLIENTE4 carga grids vacios.

Nivel de cumplimiento estimado: 70%.

### 6.5 CLIENTE3 - Registro cliente

Estado encontrado: Cumple funcionalmente.

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

Nivel de cumplimiento estimado: 88%.

### 6.6 Resultado Jose

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM6 | Funcional | 90% | Evidencia final y asegurar migraciones aplicadas. |
| ADM7 | Funcional | 90% | Evidencia final CRUD completo. |
| CLIENTE1 | Funcional | 90% | Evidencia login exitoso/fallido. |
| CLIENTE2 | Parcial alto | 70% | Logo/icono real y que sus opciones apunten a pantallas funcionales. |
| CLIENTE3 | Funcional | 88% | Evidencia final y confirmar obligatoriedad de segundo apellido. |

Riesgo principal: medio-bajo. El bloque de Jose esta mayormente defendible; su
dependencia mas fuerte es que Gabriel cierre CLIENTE4-CLIENTE7 para que CLIENTE2
no sea solo plantilla.

## 7. Gabriel - CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7

Responsabilidad segun estrategia 3: autogestion transaccional del cliente.

### 7.1 CLIENTE4 - Mostrar lineas asociadas al cliente

Estado encontrado: No completo.

Evidencia:

- `dotnet_webapps/WebCliente/Lineas.aspx`
- `dotnet_webapps/WebCliente/Lineas.aspx.cs`

Lo que ya esta:

- Existe pantalla `Lineas.aspx`.
- Existe estructura visual con grids para prepago y postpago.
- La pagina protege sesion de cliente.

Pendientes o riesgos:

- Los grids se cargan con `new List<object>()`, es decir, sin datos reales.
- No consume WS Proveedor.
- No muestra saldo prepago real.
- No muestra monto pendiente postpago real.
- No implementa doble click/navegacion hacia recarga o pago.

Nivel de cumplimiento estimado: 20%.

### 7.2 CLIENTE5 - Cargar saldo a linea prepago

Estado encontrado: No completo.

Evidencia:

- `dotnet_webapps/WebCliente/Portal.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx.cs`
- `java_proveedor/src/database/ServicioDAO.java`
- `java_proveedor/src/services/AdministracionTelefonica.java`

Lo que ya esta:

- Existe ruta visual `Portal.aspx?op=recarga`.
- Java tiene capacidad de recargar saldo internamente.
- Python tambien enruta `RECARGAR_SALDO`.

Pendientes o riesgos:

- La Web solo muestra texto de pantalla reservada.
- No lista lineas prepago.
- No tiene formulario de tarjeta.
- No valida tarjeta, fecha, CVV ni monto.
- No consume WS Proveedor desde Web.
- El WS Proveedor WCF principal no expone una operacion de recarga para cliente.

Nivel de cumplimiento estimado: 15%.

### 7.3 CLIENTE6 - Pagar factura postpago

Estado encontrado: No completo.

Evidencia:

- `dotnet_webapps/WebCliente/Portal.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx.cs`
- `database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql`

Lo que ya esta:

- Existe ruta visual `Portal.aspx?op=pago`.
- Existe tabla/proceso de facturacion postpago.

Pendientes o riesgos:

- No hay pantalla funcional de pago.
- No lista facturas pendientes por cliente.
- No bloquea edicion del monto de factura.
- No valida datos de tarjeta.
- No existe endpoint WCF final para cancelar factura.
- No se observa envio de correo electronico con detalle de facturacion.

Nivel de cumplimiento estimado: 10%.

### 7.4 CLIENTE7 - Devolucion de linea por cliente

Estado encontrado: No completo.

Evidencia:

- `dotnet_webapps/WebCliente/Portal.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- Existe ruta visual `Portal.aspx?op=devolucion`.
- Backend puede desactivar linea mediante WS_PROVEEDOR2/Proveedor5.

Pendientes o riesgos:

- La pantalla no lista lineas del cliente.
- No valida si la linea postpago tiene factura pendiente.
- No muestra confirmacion real.
- No consume WS_PROVEEDOR2 desde WebCliente.
- Falta enviar datos ocultos requeridos por el PDF al WS.

Nivel de cumplimiento estimado: 15%.

### 7.5 Resultado Gabriel

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| CLIENTE4 | No completo | 20% | Listar lineas reales por cliente via WS Proveedor, separar prepago/postpago y enlazar acciones. |
| CLIENTE5 | No completo | 15% | Pantalla de recarga, validaciones de tarjeta/monto y endpoint WS de recarga. |
| CLIENTE6 | No completo | 10% | Pago de factura, consulta deuda, endpoint de cancelacion y correo. |
| CLIENTE7 | No completo | 15% | Devolucion con validacion de deuda y consumo de WS_PROVEEDOR2. |

Riesgo principal: muy alto. Este bloque es el mayor pendiente del alcance final.
Aunque existen capacidades en Java/Python, todavia no estan expuestas ni usadas
por WebCliente.

## 8. Estado de Web Services requeridos por estrategia 3

| Servicio / necesidad | Estado | Historias afectadas |
|---|---|---|
| WS_AUTENTICACION1 - login por tipo | Funcional | ADM1, CLIENTE1 |
| WS_AUTENTICACION2 - CRUD/cambio estado usuarios | Funcional | ADM7, CLIENTE3 |
| WS_PROVEEDOR1 - registrar nueva linea | Parcial/no integrado | ADM3 |
| WS_PROVEEDOR2 - activar/desactivar linea | Funcional en backend, falta uso Web ADM3-ADM5/CLIENTE7 | ADM4, ADM5, CLIENTE7 |
| WS_PROVEEDOR3 - calcular facturacion | Funcional para ADM6 | ADM6 |
| Consultar lineas por cliente | Faltante en WCF final | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |
| Recargar saldo | Backend interno existe, endpoint Web final faltante | CLIENTE5 |
| Cancelar factura | Faltante | CLIENTE6 |
| Enviar correo de factura | Faltante | CLIENTE6 |
| Eliminar linea disponible | Faltante/no integrado | ADM3 |

## 9. Lo que ya esta defendible

- Compilacion principal de WebApps y servicios usados por persona 2.
- Login administrativo y cliente con MongoDB por WS Autenticacion.
- Registro de cliente.
- CRUD de administradores.
- Calculo de facturacion postpago.
- Proveedor Java con movimientos, bitacora y facturacion.
- Identificador Python con bitacora, consulta de saldo y sincronizacion.
- Scripts de preparacion y levantamiento de procesos.
- Seed de persona 2 y guia de ejecucion documentada.

## 10. Lo que falta para cierre defendible

Prioridad alta:

1. Decidir si ADM3-ADM5 se terminan en `dotnet_webapps/WebAdministrativo` o en
   `dotnet_webservices/WebAdministrativa`.
2. Si se usa la MVC de Charlie/companero 1, restaurar NuGet y corregir build.
3. Conectar ADM3-ADM5 a WS Proveedor real.
4. Agregar en WS Proveedor operaciones de consulta de lineas disponibles/en uso.
5. Implementar CLIENTE4 con datos reales por identificacion del cliente.
6. Implementar CLIENTE5 con recarga y validaciones de tarjeta.
7. Implementar CLIENTE6 con pago de factura, cancelacion en backend y correo.
8. Implementar CLIENTE7 con validacion de deuda y devolucion por WS_PROVEEDOR2.
9. Capturar evidencias finales por historia y por responsable.

Prioridad media:

1. Agregar logo/icono real a ambas plantillas.
2. Homologar textos visibles con los mensajes exactos del PDF.
3. Documentar que las WebApps no acceden directo a BD/socket.
4. Revisar que los estados de linea se expliquen como `ACTIVO` y `DISPONIBLE`.
5. Preparar una guia unica de demo para el dia de defensa.

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
3. Apoyar la integracion de WS Autenticacion si Charlie/Gabriel requieren datos
   de cliente autenticado.
4. Ajustar CLIENTE2 si Gabriel agrega nuevas paginas para CLIENTE4-CLIENTE7.

### Gabriel

Ruta corta para cierre:

1. Implementar primero CLIENTE4 porque alimenta CLIENTE5, CLIENTE6 y CLIENTE7.
2. Pedir/agregar endpoints WCF para listar lineas y facturas por cliente.
3. Implementar recarga prepago y validaciones de tarjeta.
4. Implementar pago postpago con correo.
5. Implementar devolucion bloqueando lineas con deuda pendiente.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra historias asignadas en `estrategia_3.md`:

```text
Charlie : [#####-----] 45% de 100
Jose    : [########--] 86% de 100
Gabriel : [##--------] 15% de 100
```

Lectura rapida:

- Charlie: tiene login y parte de plantilla, pero debe cerrar ADM3-ADM5.
- Jose: tiene el bloque mas completo; faltan evidencias y pequenos ajustes.
- Gabriel: necesita desarrollar la autogestion cliente casi completa.

## 13. Conclusion

El proyecto avanzo mucho en infraestructura y en el bloque de persona 2, pero el
alcance final todavia depende de cerrar las operaciones Web faltantes. La
estrategia 3 sigue siendo valida: separar administrativo de lineas, bloque de
facturacion/autenticacion y autogestion cliente ayuda a ordenar el cierre.

La prioridad real no es rehacer el backend, porque Java/Python ya tienen mucho
del trabajo operativo. La prioridad es exponer esas capacidades mediante Web
Services y conectarlas a las pantallas finales. Si el equipo se enfoca en ADM3,
ADM4, ADM5 y CLIENTE4-CLIENTE7, el proyecto puede pasar de funcional parcial a
defendible integral.
