# Informe 9 de auditoria final - Estado actual contra Proyecto final

Fecha de analisis: 2026-08-19  
Rama revisada: arbol local actual del repositorio `Central_TG`  
Documentos base: `docs/auditoria/informe 8_estado_actual_vs_informe_7_y_proyecto_final.md`, `docs/Proyecto/Proyecto final.pdf`, codigo fuente actual, compilaciones locales y validaciones funcionales reportadas por el usuario

## 1. Resumen ejecutivo

Este informe cierra la revision tipo auditoria del proyecto completo contra el
PDF oficial `Proyecto final.pdf`, manteniendo la estructura del informe 8. La
revision se concentra en determinar si el proyecto cumple al 100% los criterios
del alcance final y en dejar visibles los puntos que todavia podrian generar
preguntas en una defensa estrictamente literal.

Frente al informe 8 hubo avances finales importantes:

- SMTP quedo configurado desde `.env` y cargado por `scripts/persona2-levantar.ps1`.
- WebAdministrativo envia correo cuando se genera una factura postpago.
- PortalCliente envia correo cuando el cliente paga/cancela la factura.
- Se corrigio la fuente del correo del cliente para facturacion, sincronizando
  el correo del perfil cliente con SQL Server proveedor.
- Se agrego `ModificarCliente` en `WS_Autenticacion` para que el portal no
  actualice accidentalmente el correo de un administrador con la misma
  identificacion.
- Se agrego `Mis datos` en PortalCliente para ver datos personales y editar
  solo correo, usuario y contrasena.
- Se corrigio CLIENTE7 en PortalCliente: al devolver linea ahora envia el
  numero cifrado a `WS_Proveedor`, igual que el flujo administrativo.
- PortalCliente, Simulador, Java proveedor y Python identificador fueron
  validados localmente desde terminal.

El estado general es de cierre funcional alto. Las historias oficiales estan
implementadas en la ruta de demo definida por el equipo:

```text
WebAdministrativo C# -> WS -> Java/Python/SQL/Mongo
WebCliente C# -> PortalCliente ASP.NET Core -> WS -> SQL/Mongo
Simulador C# -> Python Identificador / Java Proveedor
```

Resultado de cumplimiento contra el PDF:

| Area | Estado | Comentario |
|---|---|---|
| Simulador C# | Cumple alto | Compila; opera como superficie de llamadas y consulta. |
| Identificador Python | Cumple alto | Sintaxis valida; mantiene flujo por socket y bitacora. |
| Proveedor Java | Cumple alto | Compila; registra movimientos, llamadas, saldos y facturacion. |
| WS Autenticacion | Cumple alto | Login por tipo, CRUD, clientes/admin separados y metodo de pago. |
| WS Proveedor WCF | Cumple alto con verificacion pendiente de MSBuild local | Implementa ADM3-ADM6, solicitudes, sincronizacion de correo y devolucion. |
| WS ProveedorCliente WCF | Cumple alto | Consulta lineas, recarga, pago, factura y correo de recibo. |
| Web Administrativo principal | Cumple alto | ADM1-ADM7 completos, logo, menu, footer y correo de factura generada. |
| Web Cliente principal | Cumple alto | CLIENTE1-CLIENTE3 completos; redirige a PortalCliente. |
| PortalCliente ASP.NET Core | Cumple alto | CLIENTE4-CLIENTE7 completos, perfil cliente y correo de pago. |
| WebAdministrativa MVC | No oficial/no recomendada | No debe usarse en demo; no forma parte de la ruta defendible. |
| Documentacion | Parcial alto | Hay arquitectura, instalacion, pruebas y ruta de demo; faltan documentos formales completos del PDF. |

Conclusion ejecutiva: el proyecto esta funcionalmente muy cerca del 100%, pero
una auditoria literal contra el PDF no puede marcarlo como 100% absoluto por
cuatro diferencias:

1. El PDF exige contrasenas de 14 caracteres; el sistema quedo con minimo 7 por
   decision funcional del equipo.
2. ADM6 en el PDF pide validacion estricta de continuidad de fechas entre
   facturaciones; la implementacion actual trabaja una facturacion individual
   por linea y valida fechas basicas, no toda la continuidad global literal.
3. Los cambios WCF finales no pudieron compilarse desde esta terminal porque no
   existe `MSBuild.exe` disponible; requieren Visual Studio/MSBuild clasico.
4. La documentacion formal del PDF no esta completa como paquete final dentro
   del repositorio: faltan portada, introduccion formal, diagramas finales
   exportados, conclusiones, recomendaciones y bibliografia.

Nivel global estimado:

```text
Proyecto completo: [#########-] 96% de 100
```

Si se atienden los cuatro puntos anteriores, el proyecto podria defenderse como
100% contra el PDF.

## 2. Requerimientos fuente revisados

Se reviso `docs/Proyecto/Proyecto final.pdf`, 19 paginas, que define el
`Alcance Final - Sistemas Web`.

Requerimientos oficiales principales:

| Bloque | Historias |
|---|---|
| Administracion | ADM1, ADM2, ADM3, ADM4, ADM5, ADM6, ADM7 |
| Cliente | CLIENTE1, CLIENTE2, CLIENTE3, CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |
| Componentes base | Proveedor, Identificador, Simulador, WS Proveedor, WS Autenticacion |

Reglas tecnicas obligatorias del PDF:

- La aplicacion Web Administrativa debe estar desarrollada en C#.
- La aplicacion Web Cliente puede estar en C# u otro lenguaje.
- Las aplicaciones Web no deben acceder directamente a bases de datos ni a
  sockets.
- Toda operacion Web debe pasar por Web Services.
- Los servicios Web pueden ampliarse o crearse para cubrir consultas nuevas.
- Los componentes pueden estar en equipos distintos, pero deben funcionar
  integrados.
- Evaluar componentes aislados puede implicar sancion.
- Debe entregarse documentacion completa de analisis, diseno, diagramas,
  implementacion y codigo fuente actualizado.
- Cada historia debe pertenecer a un estudiante y la distribucion debe ser
  equitativa.

Comparacion contra informe 8:

- SMTP deja de ser pendiente de configuracion base: ya existe configuracion
  centralizada por `.env` y consumo en ambos lados del proceso de factura.
- CLIENTE6 mejora porque el recibo de pago puede enviarse por correo real.
- ADM6 mejora porque la factura generada tambien intenta notificar al cliente.
- El perfil cliente agrega una funcionalidad no exigida por el PDF, pero
  necesaria para mantener actualizado el correo operativo.
- CLIENTE7 mejora por correccion del telefono cifrado en la devolucion desde
  PortalCliente.
- Se mantiene como diferencia literal la regla de 14 caracteres de contrasena.
- Se mantiene como diferencia literal la continuidad global de fechas de ADM6.
- Se mantiene como pendiente documental el paquete formal completo de entrega.

## 3. Verificaciones ejecutadas

| Verificacion | Resultado |
|---|---|
| Lectura completa de `informe 8_estado_actual_vs_informe_7_y_proyecto_final.md` | Correcta; se tomo como estructura base. |
| Extraccion y lectura de `docs/Proyecto/Proyecto final.pdf` con `pypdf` | Correcta; 19 paginas leidas. |
| Inventario de archivos con `rg --files` | Correcto; se confirmaron WebApps, servicios WCF, PortalCliente, Java, Python, simulador, scripts y docs. |
| Busqueda dirigida de SMTP, facturacion, recibo, perfil cliente, sincronizacion de correo y devolucion | Correcta; se localizaron implementaciones actuales. |
| Busqueda de acceso directo a BD/socket en WebAdministrativo, WebCliente y PortalCliente | Correcta; no se encontraron accesos directos en las Web principales. |
| `dotnet build dotnet_webservices/PortalCliente/PortalCliente.csproj --no-restore /p:UseAppHost=false /p:OutputPath=$env:TEMP/PortalClienteBuildCheck` | Correcto; 0 errores. |
| `dotnet build csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj --no-restore` | Correcto; 0 errores. |
| `python -m py_compile` sobre archivos principales del Identificador | Correcto; 0 errores. |
| `javac -encoding UTF-8` sobre `java_proveedor/src` | Correcto; 0 errores. |
| `where.exe MSBuild.exe` | No disponible en esta terminal; no se pudo validar WCF clasico desde CLI. |
| Revision de estado git | Hay cambios locales no confirmados, incluyendo los arreglos finales. |

Observacion importante: la ausencia de `MSBuild.exe` en esta terminal no implica
que los proyectos WCF esten rotos; significa que la validacion debe hacerse con
Visual Studio/MSBuild clasico, como ya se habia documentado en informes
anteriores.

## 4. Hallazgos transversales

### 4.1 La arquitectura oficial se respeta

El PDF exige que las Web no consulten bases de datos ni sockets directamente.
La revision actual confirma que las pantallas Web principales pasan por
servicios:

- WebAdministrativo consume `AutenticacionSoapClient` y `ProveedorSoapClient`.
- WebCliente consume `AutenticacionSoapClient`.
- PortalCliente consume `ProveedorClienteSoapClient`, `ProveedorPortalSoapClient`,
  `Proveedor2SoapClient`, `AutenticacionPortalSoapClient` y `EmailService`.
- Los accesos SQL aparecen en servicios WCF o en Java proveedor.
- Los accesos Mongo aparecen en `WS_Autenticacion`.
- El socket hacia Java queda encapsulado en WS Proveedor o en componentes de
  backend.

Esto queda alineado con la regla del PDF: las Web no tienen acceso directo a
BD ni socket.

### 4.2 La ruta oficial de demo queda clara

Superficies recomendadas:

- Administrativo: `dotnet_webapps/WebAdministrativo`.
- Login/registro cliente: `dotnet_webapps/WebCliente`.
- Autogestion transaccional: `dotnet_webservices/PortalCliente`.
- Simulador: `csharp_simulador/SimuladorTelefonico`.
- Backend: `WS_Autenticacion`, `WS_Proveedor`, `WS_ProveedorCliente`, Java
  proveedor y Python identificador.

Superficie no recomendada:

- `dotnet_webservices/WebAdministrativa`, porque no representa la ruta funcional
  actual y no debe usarse en demo.

### 4.3 La marca visual cumple el requerimiento de icono

El PDF pide icono de empresa proveedora y una interfaz seria/profesional. El
estado actual incluye:

- `dotnet_webapps/WebAdministrativo/Assets/Logo.png`
- `dotnet_webapps/WebCliente/Assets/Logo.png`
- `dotnet_webservices/PortalCliente/wwwroot/img/Logo.png`
- Favicon y logo en pantallas de login.
- Fondo de marca semitransparente en pantallas principales.
- Menus persistentes y footer.

Nivel de cumplimiento visual: alto.

### 4.4 SMTP ya forma parte del flujo operativo

El PDF exige correo en CLIENTE6 cuando el pago de factura es exitoso. El flujo
actual cubre incluso mas:

- ADM6: WebAdministrativo envia correo al cliente al generar factura.
- CLIENTE6: PortalCliente envia recibo al cliente al pagar/cancelar factura.
- La configuracion SMTP vive en `.env` y se inyecta al levantar con
  `scripts/persona2-levantar.ps1`.
- `EmailService` en WebAdministrativo y PortalCliente valida que exista
  configuracion antes de intentar enviar.

Riesgo residual:

- Si el proceso estaba levantado antes de cargar `.env`, no hereda la
  configuracion SMTP. Se debe reiniciar con `persona2-levantar.ps1`.
- Si el correo del cliente esta desactualizado en SQL Server proveedor, el
  correo puede rebotar. Para eso se agrego sincronizacion desde `Mis datos`.

### 4.5 La fuente del correo de facturacion fue corregida

El problema detectado fue que el portal actualizaba el correo en MongoDB, pero
facturacion leia el correo desde SQL Server proveedor. Se corrigio con:

- `ActualizarCorreoClienteRequest`
- `IProveedorService.ActualizarCorreoCliente`
- `ProveedorService.svc.cs::ActualizarCorreoCliente`
- `Proveedor2SoapClient.ActualizarCorreoClienteAsync`
- llamada desde `ClienteController.Perfil`

Ademas, `WS_Autenticacion` ahora expone `ModificarCliente`, evitando que el
portal modifique por accidente el administrador si existe la misma
identificacion en ambos roles.

### 4.6 La facturacion quedo funcional, pero no 100% literal

ADM6 ya separa consulta y generacion:

- `ConsultarFacturacion` calcula vista previa.
- `CalcularFacturacion` genera la factura.
- WebAdministrativo bloquea facturas con total `0`.
- Se envia correo de factura generada.
- CLIENTE6 paga y deja la factura pendiente en cero.

Diferencia literal:

- El PDF solicita validar que la fecha de calculo no inicie antes de la ultima
  ejecucion y que no deje dias entre la ultima ejecucion y la nueva. La
  implementacion actual valida fechas basicas por linea, pero no implementa por
  completo la continuidad global entre procesos.

### 4.7 CLIENTE7 fue corregido en la ruta del cliente

El fallo detectado en devolucion desde PortalCliente era que `NumeroTelefono`
se enviaba sin cifrar a `WS_Proveedor`. El validador espera Base64/AES, igual
que el flujo administrativo.

Correccion:

- `PortalCliente/Services/ProveedorCryptoHelper.cs`
- registro en `Program.cs`
- `ClienteController.DevolverLinea` envia el numero cifrado.

Esto alinea CLIENTE7 con ADM5 y con `WS_PROVEEDOR2`.

### 4.8 Hay extensiones funcionales no exigidas por el PDF

Extensiones actuales:

- Solicitud de linea desde PortalCliente.
- Aprobacion/rechazo de solicitudes desde WebAdministrativo.
- Perfil cliente editable parcialmente.
- Metodo de pago registrado en CLIENTE3.
- Correo de factura generada desde ADM6.

Estas extensiones no contradicen el PDF, pero en demo conviene presentarlas como
mejoras adicionales, no como historias oficiales.

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

- Pantalla de login.
- Usuario y contrasena.
- Tipo administrador oculto.
- Validacion contra `WS_AUTENTICACION1`.
- Contrasena cifrada.
- Redireccion al sitio administrativo.
- Mensaje de credenciales incorrectas.

Pendientes o riesgos:

- Riesgo bajo: mensaje usa "contrasena" sin tilde por estilo ASCII, pero el
  contenido funcional coincide.

Nivel de cumplimiento estimado: 96%.

### 5.2 ADM2 - Plantilla y administracion de clientes

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Site.Master`
- `dotnet_webapps/WebAdministrativo/Site.Master.cs`
- `dotnet_webapps/WebAdministrativo/Styles/site.css`
- `dotnet_webapps/WebAdministrativo/Assets/Logo.png`

Lo que ya esta:

- Menu ADM3-ADM7.
- Salir del sitio.
- Icono/logo.
- Menu disponible en las operaciones.
- Footer visible.
- Pantallas administrativas con identidad visual.

Pendientes o riesgos:

- Ninguno relevante para el PDF.
- La opcion adicional `Solicitudes` no afecta el cumplimiento.

Nivel de cumplimiento estimado: 98%.

### 5.3 ADM3 - Poner nuevas lineas a disposicion

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasNuevas.aspx`
- `dotnet_webapps/WebAdministrativo/LineasNuevas.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/ProveedorSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/database/ServicioDAO.java`

Lo que ya esta:

- Lista lineas disponibles.
- Muestra numero, identificador de tarjeta y tipo.
- Permite eliminar una linea disponible con confirmacion.
- Permite crear nueva linea.
- Solicita numero, identificador del telefono, tarjeta y tipo.
- Deja la linea lista para venta como `DISPONIBLE`.
- Usa `WS_PROVEEDOR1` / WS Proveedor.

Pendientes o riesgos:

- Validar con Visual Studio/MSBuild clasico despues de los cambios finales WCF.

Nivel de cumplimiento estimado: 97%.

### 5.4 ADM4 - Activar linea vendida

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasActivar.aspx`
- `dotnet_webapps/WebAdministrativo/LineasActivar.aspx.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Validators/ActivarDesactivarLineaValidator.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- Lista lineas disponibles.
- Permite seleccionar linea.
- Registra cliente desde datos de MongoDB.
- Envia a `WS_PROVEEDOR2` con estado activo.
- Valida identificadores y duplicados.
- Espera respuesta y muestra mensaje.

Pendientes o riesgos:

- El PDF dice que se debe mostrar un campo para registrar cedula; la solucion
  actual usa seleccion/control desde clientes reales, lo cual es mas segura pero
  ligeramente distinta en UI.

Nivel de cumplimiento estimado: 96%.

### 5.5 ADM5 - Devolucion/desactivacion administrativa de linea

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasDevolucion.aspx`
- `dotnet_webapps/WebAdministrativo/LineasDevolucion.aspx.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- Lista lineas activas.
- Muestra telefono, tarjeta, telefono/IMEI, cliente, nombre y tipo.
- Confirmacion antes de devolver.
- Usa `WS_PROVEEDOR2`.
- Devuelve a estado `DISPONIBLE`.

Pendientes o riesgos:

- El PDF usa el texto "estado inactivo"; la implementacion usa
  `DISPONIBLE` como estado operativo de inventario. Conviene explicarlo.

Nivel de cumplimiento estimado: 96%.

### 5.6 Resultado Charlie

| Historia | Estado | Avance estimado | Que falta para 100 literal |
|---|---|---:|---|
| ADM1 | Cumple | 96% | Pulir texto exacto con tilde si se desea literalidad. |
| ADM2 | Cumple alto | 98% | Sin pendiente relevante. |
| ADM3 | Cumple alto | 97% | Compilar WCF con MSBuild clasico. |
| ADM4 | Cumple alto | 96% | Explicar seleccion de cliente vs campo libre de cedula. |
| ADM5 | Cumple alto | 96% | Explicar `DISPONIBLE` como equivalente operativo de inactivo/devuelta. |

Riesgo principal: bajo. Charlie queda defendible.

## 6. Jose - ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3

Responsabilidad segun estrategia 3: facturacion, mantenimiento de usuarios,
login cliente, plantilla cliente y registro cliente.

### 6.1 ADM6 - Calcular facturacion

Estado encontrado: Cumple alto, con diferencia literal.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Facturacion.aspx`
- `dotnet_webapps/WebAdministrativo/Facturacion.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/EmailService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor6Service.java`
- `java_proveedor/src/database/FacturacionDAO.java`

Lo que ya esta:

- Muestra ultima factura generada.
- Consulta individual por linea postpago.
- Separa consulta y generacion.
- Bloquea facturas en cero.
- Usa `WS_PROVEEDOR3`.
- Envia correo al cliente cuando se genera factura.
- Muestra mensajes de exito/error.

Diferencia contra el PDF:

- No se implementa completamente la validacion de continuidad global de fechas
  entre procesos de facturacion. El flujo actual fue adaptado a facturacion por
  linea y pago por cliente.

Nivel de cumplimiento estimado: 91%.

### 6.2 ADM7 - Mantenimiento usuario administrador

Estado encontrado: Cumple alto, con diferencia literal.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Administradores.aspx`
- `dotnet_webapps/WebAdministrativo/Administradores.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`
- `dotnet_webservices/WS_Autenticacion/Validators/UsuarioValidator.cs`

Lo que ya esta:

- Lista administradores.
- Nuevo administrador.
- Edicion.
- Activar/inactivar.
- Eliminar con confirmacion.
- Estado activo para nuevos.
- Tipo administrador oculto.
- Validaciones de correo y contrasena.
- Cifrado.

Diferencia contra el PDF:

- El PDF exige contrasenas de 14 caracteres. El sistema usa minimo 7 con
  mayuscula, minuscula, numero y especial.

Nivel de cumplimiento estimado: 90%.

### 6.3 CLIENTE1 - Login cliente

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebCliente/Login.aspx`
- `dotnet_webapps/WebCliente/Login.aspx.cs`
- `dotnet_webapps/WebCliente/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`

Lo que ya esta:

- Login cliente.
- Tipo cliente oculto.
- Contrasena cifrada.
- Mensaje de credenciales incorrectas.
- Link a registro.
- Redireccion a PortalCliente.

Pendientes o riesgos:

- Ninguno relevante para el PDF.

Nivel de cumplimiento estimado: 97%.

### 6.4 CLIENTE2 - Plantilla portal cliente

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebCliente/Site.Master`
- `dotnet_webservices/PortalCliente/Views/Shared/_Layout.cshtml`
- `dotnet_webservices/PortalCliente/wwwroot/css/site.css`
- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`

Lo que ya esta:

- Menu: Mis lineas, Cargar saldo, Pagar factura, Devolver linea.
- Salir del sitio.
- Saludo/nombre de cliente desde sesion.
- Icono/logo.
- Menu persistente.
- Footer.
- Al ingresar se muestra CLIENTE4.

Pendientes o riesgos:

- La opcion adicional `Mis datos` y `Solicitar linea` no estan en PDF, pero no
  rompen cumplimiento.

Nivel de cumplimiento estimado: 98%.

### 6.5 CLIENTE3 - Registro cliente

Estado encontrado: Cumple alto, con diferencia literal.

Evidencia:

- `dotnet_webapps/WebCliente/Registro.aspx`
- `dotnet_webapps/WebCliente/Registro.aspx.cs`
- `dotnet_webapps/WebCliente/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`

Lo que ya esta:

- Registro de cliente.
- Identificacion, nombre, apellidos, correo, usuario y contrasena.
- Tipo cliente oculto.
- Estado activo.
- Validaciones.
- Registro exitoso o error recibido.
- Opcion de volver al login.
- Metodo de pago agregado.

Diferencia contra el PDF:

- Contrasena minimo 7 en lugar de 14.

Nivel de cumplimiento estimado: 91%.

### 6.6 Resultado Jose

| Historia | Estado | Avance estimado | Que falta para 100 literal |
|---|---|---:|---|
| ADM6 | Cumple alto con diferencia | 91% | Implementar continuidad global de fechas o documentar formalmente la adaptacion. |
| ADM7 | Cumple alto con diferencia | 90% | Cambiar regla de contrasena a 14 caracteres o justificarla. |
| CLIENTE1 | Cumple alto | 97% | Sin pendiente relevante. |
| CLIENTE2 | Cumple alto | 98% | Sin pendiente relevante. |
| CLIENTE3 | Cumple alto con diferencia | 91% | Cambiar regla de contrasena a 14 caracteres o justificarla. |

Riesgo principal: medio-bajo por diferencias literales contra PDF.

## 7. Gabriel - CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7

Responsabilidad segun estrategia 3: autogestion transaccional del cliente.

### 7.1 CLIENTE4 - Mostrar lineas asociadas al cliente

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/Index.cshtml`
- `dotnet_webservices/PortalCliente/Services/ProveedorClienteSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- Lista lineas prepago con saldo.
- Lista lineas postpago con factura pendiente o cero.
- Consulta por identificacion de sesion.
- Usa Web Service.
- Desde filas permite navegar a recarga o pago.

Pendientes o riesgos:

- El PDF menciona doble click; la UI actual usa click. Funcionalmente cumple el
  acceso, pero no es literal al 100%.

Nivel de cumplimiento estimado: 95%.

### 7.2 CLIENTE5 - Cargar saldo a linea prepago

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/CargarSaldo.cshtml`
- `dotnet_webservices/PortalCliente/Models/CargarSaldoViewModel.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- Lista lineas prepago.
- Carga datos de tarjeta.
- Valida tarjeta de 12 digitos, nombre, vencimiento y CVV.
- Valida monto positivo y sin decimales.
- Usa WS para aumentar saldo.
- Muestra resultado.

Pendientes o riesgos:

- Riesgo bajo: el flujo precarga metodo de pago como mejora; en demo se debe
  mostrar que los datos estan disponibles y son validos.

Nivel de cumplimiento estimado: 96%.

### 7.3 CLIENTE6 - Pagar factura postpago

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/PagarFactura.cshtml`
- `dotnet_webservices/PortalCliente/Services/EmailService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`
- `scripts/persona2-levantar.ps1`

Lo que ya esta:

- Lista lineas postpago.
- Muestra factura pendiente o cero.
- Al seleccionar, muestra pago con tarjeta.
- Monto de factura no editable.
- Valida datos de tarjeta.
- Usa WS para pagar.
- Cancela la factura pendiente.
- Marca consumo como `PAGADA`.
- Envia correo de recibo al cliente.

Cambios finales:

- SMTP configurado por `.env`.
- Email de recibo usa `Smtp:*`.
- Se corrigio sincronizacion del correo del cliente hacia SQL proveedor.

Pendientes o riesgos:

- Necesita reiniciar procesos con `.env` cargado para que SMTP funcione.
- Se recomienda hacer una prueba final end-to-end despues de reiniciar todos los
  servicios WCF y PortalCliente.

Nivel de cumplimiento estimado: 97%.

### 7.4 CLIENTE7 - Devolucion de linea por cliente

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/DevolverLinea.cshtml`
- `dotnet_webservices/PortalCliente/Services/Proveedor2SoapClient.cs`
- `dotnet_webservices/PortalCliente/Services/ProveedorCryptoHelper.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`

Lo que ya esta:

- Lista prepago con saldo.
- Lista postpago con factura pendiente o cero.
- Confirma devolucion.
- Bloquea postpago con deuda.
- Envia datos ocultos al WS.
- Usa `WS_PROVEEDOR2`.
- Envia numero cifrado.

Pendientes o riesgos:

- Igual que ADM5, el estado final operativo es `DISPONIBLE`, no una etiqueta
  literal `INACTIVO`.
- Requiere prueba manual final despues del reinicio.

Nivel de cumplimiento estimado: 96%.

### 7.5 Solicitar linea y Mis datos - Extensiones

Estado encontrado: Funcional complementario.

Evidencia:

- `dotnet_webservices/PortalCliente/Views/Cliente/SolicitarLinea.cshtml`
- `dotnet_webservices/PortalCliente/Views/Cliente/Perfil.cshtml`
- `dotnet_webapps/WebAdministrativo/SolicitudesLineas.aspx`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`

Impacto:

- No son historias oficiales del PDF.
- Mejoran la coherencia de demo y el flujo de correo.
- No deben presentarse como reemplazo de ninguna historia oficial.

### 7.6 Resultado Gabriel

| Historia | Estado | Avance estimado | Que falta para 100 literal |
|---|---|---:|---|
| CLIENTE4 | Cumple alto | 95% | Si se exige literalidad, ajustar click a doble click. |
| CLIENTE5 | Cumple alto | 96% | Prueba final integrada con saldo real. |
| CLIENTE6 | Cumple alto | 97% | Prueba final end-to-end de correo despues de reinicio. |
| CLIENTE7 | Cumple alto | 96% | Prueba final end-to-end despues de reinicio. |

Riesgo principal: bajo. Gabriel queda defendible.

## 8. Estado de Web Services requeridos por el PDF

| Servicio / necesidad | Estado actual | Historias afectadas |
|---|---|---|
| WS_AUTENTICACION1 - login por tipo | Funcional | ADM1, CLIENTE1 |
| WS_AUTENTICACION2 - CRUD/cambio estado usuarios | Funcional | ADM7, CLIENTE3 |
| WS_AUTENTICACION2 - `ModificarCliente` | Funcional agregado | Perfil cliente, correo correcto |
| WS_AUTENTICACION2 - metodo de pago cliente | Funcional agregado | CLIENTE3, CLIENTE5, CLIENTE6 |
| WS_PROVEEDOR1 - registrar nueva linea | Funcional | ADM3 |
| WS_PROVEEDOR2 - activar/desactivar lineas | Funcional | ADM4, ADM5, CLIENTE7 |
| WS_PROVEEDOR3 / PROVEEDOR6 - facturacion | Funcional con diferencia literal en continuidad | ADM6, CLIENTE6 |
| WS Proveedor - consultas administrativas | Funcional | ADM3-ADM6 |
| WS Proveedor - solicitudes de linea | Funcional agregado | CLIENTE4, ADM4 |
| WS Proveedor - sincronizar correo cliente | Funcional agregado | ADM6, CLIENTE6 |
| WS_ProveedorCliente - consultar lineas | Funcional | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |
| WS_ProveedorCliente - recargar saldo | Funcional | CLIENTE5 |
| WS_ProveedorCliente - pagar factura | Funcional | CLIENTE6 |
| Correo de factura generada | Funcional agregado | ADM6 |
| Correo de recibo de pago | Funcional requerido | CLIENTE6 |

Comparacion contra informe 8:

- SMTP pasa de pendiente a implementado/configurable.
- WS Autenticacion mejora por `ModificarCliente`.
- WS Proveedor mejora por `ActualizarCorreoCliente`.
- PortalCliente mejora por cifrado de telefono en devolucion.

## 9. Lo que ya esta defendible

- WebAdmin en C#.
- WebCliente en C#.
- PortalCliente funcional en ASP.NET Core.
- Web sin acceso directo a BD/socket.
- Operaciones principales por WS.
- Logo, favicon, menu y footer.
- Login admin por tipo.
- Login cliente por tipo.
- Registro cliente.
- Registro de metodo de pago.
- CRUD administradores.
- Separacion admin/cliente en MongoDB.
- ADM3 alta/eliminacion de lineas disponibles.
- ADM4 activacion.
- ADM5 devolucion administrativa.
- ADM6 consulta/generacion de factura.
- ADM6 correo de factura generada.
- CLIENTE4 lineas prepago/postpago.
- CLIENTE5 recarga prepago.
- CLIENTE6 pago postpago.
- CLIENTE6 correo de recibo.
- CLIENTE7 devolucion desde portal con telefono cifrado.
- Solicitudes de linea como extension.
- Perfil cliente como extension.
- Java proveedor compila.
- Python identificador compila sintaxis.
- Simulador compila.
- PortalCliente compila.

## 10. Lo que falta para cierre 100% literal

Prioridad alta:

1. Compilar los proyectos WCF clasicos con Visual Studio/MSBuild:
   `WS_Autenticacion`, `WS_Proveedor` y `WS_ProveedorCliente`.
2. Reiniciar todos los procesos con `scripts/persona2-levantar.ps1`.
3. Hacer prueba final end-to-end:
   generar factura -> recibir correo de factura -> pagar factura -> recibir
   correo de recibo -> devolver linea sin deuda.
4. Decidir si la regla de contrasena vuelve a 14 caracteres para coincidir
   exactamente con el PDF.
5. Decidir si ADM6 implementa validacion literal de continuidad de fechas o si
   se documenta formalmente como adaptacion por linea.

Prioridad media:

1. Completar paquete documental formal:
   portada, introduccion, diagramas de BD, casos de uso, clases, componentes,
   conclusiones, recomendaciones y bibliografia.
2. Actualizar `docs/entrega_final/Ruta_demo_alcance_final.md` para decir que
   SMTP ya usa `.env` y que hay dos correos: factura generada y recibo de pago.
3. Actualizar `docs/entrega_final/Pruebas.md` con las pruebas finales de correo,
   perfil cliente y devolucion desde portal.
4. Explicar `DISPONIBLE` como estado operativo de linea devuelta.
5. Si se exige literalidad de UI, cambiar los clicks de CLIENTE4 a doble click.

Prioridad baja:

1. Limpiar o confirmar cambios no relacionados en git antes de entrega.
2. No abrir `dotnet_webservices/WebAdministrativa` durante demo.
3. Pulir textos con tildes si se desea mayor coincidencia visual con el PDF.

## 11. Recomendacion por responsable

### Charlie

Ruta corta para defensa:

1. Entrar por WebAdministrativo.
2. Mostrar menu, logo, footer y salir del sitio.
3. Mostrar nuevas lineas disponibles.
4. Crear una linea.
5. Eliminar una linea disponible con confirmacion.
6. Activar una linea para un cliente real.
7. Devolver una linea y explicar que vuelve a `DISPONIBLE`.

Punto a cuidar:

- Explicar que `DISPONIBLE` es el estado usado por el sistema para inventario
  despues de una devolucion/desactivacion.

### Jose

Ruta corta para defensa:

1. Mostrar login cliente y login admin por tipo.
2. Mostrar registro cliente.
3. Mostrar administradores filtrados por tipo.
4. Crear/editar/activar/inactivar/eliminar administrador.
5. Mostrar ADM6: ultima factura, consulta, generacion y correo.
6. Explicar que el cliente es quien cancela la factura desde CLIENTE6.

Puntos a cuidar:

- Contrasena de 7 vs 14 caracteres.
- ADM6 por linea vs continuidad global del PDF.

### Gabriel

Ruta corta para defensa:

1. Entrar desde WebCliente/Login.
2. Confirmar redireccion a PortalCliente.
3. Mostrar Mis lineas.
4. Recargar prepago.
5. Generar/pagar factura postpago.
6. Confirmar correo de recibo.
7. Devolver linea sin deuda.
8. Mostrar bloqueo de devolucion cuando hay deuda.

Punto a cuidar:

- Reiniciar procesos antes de demo para que SMTP y contratos WCF nuevos esten
  activos.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra historias asignadas en `estrategia_3.md`:

```text
Charlie : [#########-] 97% de 100
Jose    : [#########-] 93% de 100
Gabriel : [#########-] 96% de 100
```

Lectura rapida:

- Charlie: muy alto; solo hay diferencias de estado operativo y validacion WCF
  pendiente desde MSBuild clasico.
- Jose: alto; baja por las dos diferencias mas literales del PDF: contrasena de
  14 y continuidad de fechas ADM6.
- Gabriel: muy alto; correo y devolucion fueron corregidos, queda prueba final
  integrada.

Promedio general estimado:

```text
Proyecto completo: [#########-] 96% de 100
```

Lectura de auditoria:

- Funcionalmente, el proyecto esta cerrado para demo.
- Literalmente contra PDF, no debe declararse 100% hasta resolver o justificar
  los puntos listados en seccion 10.

## 13. Conclusion

El proyecto esta en estado final funcional alto frente al PDF oficial. Las 14
historias principales tienen implementacion en la ruta oficial, las Web pasan
por servicios, los componentes backend existen, las validaciones locales
disponibles pasan y los ultimos problemas reportados por el usuario fueron
corregidos: SMTP, correo del cliente, separacion admin/cliente en modificacion
de perfil y devolucion de linea desde PortalCliente.

Para una defensa practica, el proyecto es defendible. Para una auditoria literal
de 100% contra `Proyecto final.pdf`, todavia se deben cerrar o justificar cuatro
puntos: contrasena de 14 caracteres, continuidad global de fechas en ADM6,
compilacion WCF clasica con MSBuild/Visual Studio y paquete documental formal
completo.

La recomendacion final es presentar la ruta oficial, no abrir proyectos
alternos, reiniciar todo desde `persona2-levantar.ps1`, probar el ciclo completo
de correo antes de la exposicion y tener preparada una explicacion breve de las
decisiones de diseno que difieren del texto literal del PDF.
