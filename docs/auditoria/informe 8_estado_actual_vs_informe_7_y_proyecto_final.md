# Informe 8 de avance - Estado actual contra informe 7 y Proyecto final

Fecha de analisis: 2026-08-19  
Rama revisada: arbol local actual del repositorio `Central_TG`  
Documentos base: `docs/auditoria/informe 7_estado_actual_vs_informe_6_dev.md`, `docs/Proyecto/Proyecto final.pdf`, codigo fuente actual, compilaciones locales y validaciones funcionales reportadas por el usuario

## 1. Resumen ejecutivo

El proyecto avanzo de forma muy importante frente al informe 7. El informe 7
todavia marcaba como pendientes varias pruebas integrales, migraciones,
reinicios, evidencias y validaciones de ADM3-ADM6 y CLIENTE6. Segun la
confirmacion del usuario, esas pruebas ya fueron ejecutadas, documentadas y
validadas correctamente fuera de este repositorio, por lo que este informe no
las conserva como pendientes.

El estado actual queda mas cerca de cierre funcional que de desarrollo activo.
Los cambios mas relevantes posteriores al informe 7 son:

- La marca visual del sistema ya usa el logo real desde `resources/icono/Logo.png`
  copiado a las aplicaciones Web.
- WebAdministrativo y WebCliente tienen icono, logo visible y fondo con marca
  semitransparente.
- ADM6 fue separado correctamente entre consulta y generacion de factura.
- ADM6 bloquea la generacion cuando la consulta devuelve total `0` o no tiene
  llamadas facturables.
- CLIENTE6 paga la factura pendiente y deja el consumo del periodo en estado
  `PAGADA`, evitando que vuelva a facturarse.
- `WS_ProveedorCliente` ya lee la factura pendiente por `fecha_registro DESC`,
  alineado con el ultimo calculo generado.
- CLIENTE3 ahora registra tambien metodo de pago del cliente.
- CLIENTE5 y CLIENTE6 precargan datos bancarios registrados y los muestran como
  datos de solo lectura en las vistas.
- El portal cliente ya elimina la cedula editable en pantalla y conserva la
  identidad desde sesion.
- El portal cliente tiene logout hacia `WebCliente/Login.aspx`.
- El cliente puede solicitar lineas disponibles sin autoasignacion.
- WebAdministrativo tiene pantalla de solicitudes de linea para aprobar o
  rechazar solicitudes.
- Los usuarios admin y cliente estan separados por tipo en MongoDB y permiten
  una misma identificacion con roles distintos.
- La regla de contrasena fue ajustada a minimo 7 caracteres con mayuscula,
  minuscula, numero y caracter especial.
- El simulador tiene polling en selector y administracion telefonica.
- La politica de tarifas fue actualizada: nacional mismo tipo 10 CRC/min,
  nacional PREPAGO/POSTPAGO mixto 30 CRC/min y extranjero 60 CRC/min sin
  importar direccion.

El pendiente funcional reconocido por el usuario es SMTP para el envio real de
correo en CLIENTE6. El flujo de pago existe, registra la cancelacion y prepara
el intento de correo, pero la configuracion SMTP real queda para el siguiente
dia de trabajo.

La revision contra `Proyecto final.pdf` confirma que la ruta oficial sigue
siendo:

```text
WebAdministrativo C# -> WS -> Java/Python/SQL/Mongo
WebCliente C# -> PortalCliente ASP.NET Core -> WS -> SQL/Mongo
Simulador C# -> Python Identificador / Java Proveedor
```

Estado global estimado:

| Area | Estado | Comentario |
|---|---|---|
| Simulador C# | Cumple alto | Compila; incorpora polling y reglas de tarifa nuevas. |
| Identificador Python | Cumple alto | Sintaxis valida; mantiene contratos de llamadas, consultas y catalogo. |
| Proveedor Java | Cumple alto | Compila; registra llamadas, aplica consumo postpago y soporta tarifa recibida. |
| WS Autenticacion | Cumple alto | Login por tipo, CRUD, separacion admin/cliente, metodo de pago y contrasena minimo 7. |
| WS Proveedor WCF | Cumple alto | Nuevas lineas, asignacion, devolucion, facturacion, solicitudes y listados. |
| WS ProveedorCliente WCF | Cumple alto | Consulta lineas, recarga, pago, reinicio de factura y consumo pagado. |
| Web Administrativo principal | Cumple alto/casi cierre | ADM1-ADM7 completos y solicitudes de linea agregadas. |
| Web Cliente principal | Cumple alto | Login, registro con metodo de pago, logo y redireccion a PortalCliente. |
| PortalCliente ASP.NET Core | Cumple alto con pendiente SMTP | CLIENTE4-CLIENTE7 integrados; pago funciona, correo real pendiente. |
| WebAdministrativa MVC | Parcial bajo/no oficial | Sigue fallando por paquete NuGet faltante; no debe usarse en demo. |
| Documentacion y evidencias | Parcial alto | Evidencias funcionales reportadas como listas fuera del repo; SMTP queda por evidenciar. |

Resultado general: el proyecto esta en estado defendible alto. El riesgo ya no
esta en las historias principales, sino en configuracion final de SMTP,
documentacion formal consolidada y disciplina de demo usando solo las rutas
oficiales.

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

Comparacion contra informe 7:

- No cambia el alcance oficial.
- Si cambia el nivel de cumplimiento: ADM2, ADM6, CLIENTE2, CLIENTE3,
  CLIENTE5, CLIENTE6 y CLIENTE7 mejoran.
- El riesgo de factura pendiente en cliente fue resuelto al ordenar por
  `fecha_registro DESC`.
- El pendiente SMTP se mantiene.
- La lista de prioridad alta del informe 7 se omite porque el usuario confirma
  que ya fue probada, validada y documentada en evidencias externas.

## 3. Verificaciones ejecutadas

| Verificacion | Resultado |
|---|---|
| Lectura completa de `informe 7_estado_actual_vs_informe_6_dev.md` | Correcta; se tomo como estructura base. |
| Extraccion y lectura de `docs/Proyecto/Proyecto final.pdf` con `pypdf` | Correcta; 19 paginas leidas. |
| Inventario de archivos con `rg --files` | Correcto; se confirmaron WebApps, servicios WCF, PortalCliente, Java, Python, simulador, scripts y migraciones. |
| Busqueda dirigida de facturacion, solicitudes, metodo de pago, logout, logo, polling y tarifas | Correcta; se localizaron implementaciones actuales. |
| `MSBuild dotnet_webapps/CentralTelefonica.WebApps.sln` | Correcto; WebAdministrativo y WebCliente compilan con 0 errores. |
| `MSBuild dotnet_webservices/WS_Autenticacion/WS_Autenticacion.csproj` | Correcto; 0 errores. |
| `MSBuild dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj` | Correcto; 0 errores. |
| `MSBuild dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/WS_ProveedorCliente.sln` | Correcto; 0 errores. |
| `dotnet build dotnet_webservices/PortalCliente/PortalCliente.csproj --no-restore -o .tmp/portalcliente-build` | Correcto; 0 errores. |
| `dotnet build csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj --no-restore` | Correcto; 0 errores. |
| `javac` sobre `java_proveedor` | Correcto; 0 errores. |
| `python -m py_compile` sobre archivos Python del Identificador | Correcto; 0 errores. |
| Build de `dotnet_webservices/WebAdministrativa/WebAdministrativa.sln` | Sigue fallando por paquete faltante `Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1`. |
| Revision de estado git | Hay cambios locales no confirmados, incluyendo las mejoras recientes. |
| Revision de script temporal de limpieza Mongo | El archivo temporal ya no existe. |

Observacion: este informe no repite pruebas funcionales ya realizadas por el
usuario ni solicita nuevas capturas de evidencia sobre los puntos cerrados del
informe 7. Las evidencias finales viven fuera del repositorio segun indicacion
del usuario.

## 4. Hallazgos transversales

### 4.1 La arquitectura oficial se respeta

El PDF exige que las Web no consulten bases de datos ni sockets directamente.
La revision actual confirma que las pantallas Web principales pasan por
servicios:

- WebAdministrativo consume `AutenticacionSoapClient` y `ProveedorSoapClient`.
- WebCliente consume `AutenticacionSoapClient`.
- PortalCliente consume clientes de servicio para autenticacion, proveedor,
  proveedor cliente y proveedor2.
- Los accesos SQL aparecen en servicios WCF o backend, no en las paginas Web.
- El socket hacia Java queda encapsulado en WS Proveedor y en componentes de
  backend.

Esto deja la arquitectura alineada con los aspectos tecnicos generales del PDF.

### 4.2 La ruta oficial de demo queda clara

Superficies recomendadas:

- Administrativo: `dotnet_webapps/WebAdministrativo`.
- Login/registro cliente: `dotnet_webapps/WebCliente`.
- Autogestion transaccional: `dotnet_webservices/PortalCliente`.
- Simulador: `csharp_simulador/SimuladorTelefonico`.
- Backend: `WS_Autenticacion`, `WS_Proveedor`, `WS_ProveedorCliente`, Java
  proveedor y Python identificador.

Superficie no recomendada:

- `dotnet_webservices/WebAdministrativa`, porque sigue sin compilar por paquete
  NuGet faltante y no representa la ruta funcional actual.

### 4.3 La marca visual ya cumple mejor el PDF

El PDF pide icono de empresa proveedora y una interfaz seria/profesional. El
estado actual mejora sustancialmente:

- `dotnet_webapps/WebAdministrativo/Assets/Logo.png`
- `dotnet_webapps/WebCliente/Assets/Logo.png`
- `dotnet_webservices/PortalCliente/wwwroot/img/Logo.png`
- `site.css` de WebAdmin, WebCliente y PortalCliente usan marca visual.
- Login y registro cliente tienen favicon y logo grande.
- Las pantallas administrativas principales ya muestran logo grande como fondo
  de baja opacidad.

Riesgo residual: no se hizo verificacion automatizada visual con navegador en
este informe, pero las capturas compartidas por el usuario confirmaron mejora
visual.

### 4.4 La facturacion quedo conceptualmente bien separada

ADM6 ya no mezcla consulta con pago:

- `ConsultarFacturacion` calcula una vista previa sin generar factura.
- `GenerarFacturaButton_Click` genera la factura solo despues de consultar.
- Si el resultado consultado es `0`, se deshabilita el boton y tambien se
  bloquea del lado servidor.
- El mensaje de exito indica factura generada, no cancelada.
- CLIENTE6 es el responsable de pagar/cancelar la factura.

Este flujo coincide con la interpretacion funcional definida por el usuario:
el administrador genera la factura; el cliente la consulta y la paga.

### 4.5 El pago ya limpia factura y consumo

En `WS_ProveedorCliente`, el pago:

- Busca la factura pendiente mas reciente por `fecha_registro DESC`.
- Valida que el monto pagado coincida con el pendiente.
- Pone `total_facturar = 0.00`.
- Pone `total_llamadas = 0`.
- Actualiza `fecha_registro`.
- Marca llamadas del periodo como `PAGADA`.

Esto corrige el riesgo anterior de que un consumo ya pagado volviera a entrar
en una nueva facturacion.

### 4.6 Usuarios admin y cliente tienen separacion real

MongoDB y WS Autenticacion trabajan con `tipo`:

- Tipo `1`: administrador.
- Tipo `2`: cliente.

Los indices por tipo permiten que una misma identificacion exista como admin y
cliente. La pantalla de administradores lista por tipo admin y los flujos de
cliente autentican por tipo cliente. Esto responde al detalle detectado por el
usuario: una persona puede tener ambos roles, pero las pantallas deben filtrar
por rol.

### 4.7 Solicitudes de linea agregan un flujo no oficial pero util

El PDF no exige "solicitar linea" desde el cliente, pero la funcionalidad es
coherente con el sistema:

- El cliente ve lineas disponibles.
- El cliente solicita una linea.
- La solicitud no autoasigna.
- El administrador ve solicitudes pendientes.
- El administrador puede aprobar, asignando la linea por el flujo oficial
  WS_PROVEEDOR2.
- El administrador puede rechazar.

Esto fortalece la demo porque permite iniciar desde un cliente sin lineas y
mostrar un flujo realista de aprobacion administrativa.

### 4.8 Tarifas y polling del simulador estan actualizados

El simulador ahora refresca dinamicamente:

- Selector de telefonos: polling cada 6 segundos.
- Administracion telefonica: polling cada 7 segundos.

Tarifas:

- Nacional a nacional mismo tipo: 10 CRC/min.
- Nacional PREPAGO/POSTPAGO mixto: 30 CRC/min.
- Cualquier llamada con extranjero, sin importar direccion: 60 CRC/min.
- Extranjero a extranjero: 60 CRC/min.

El codigo relevante esta en:

- `csharp_simulador/SimuladorTelefonico/Services/PoliticaTarifaService.cs`
- `csharp_simulador/SimuladorTelefonico/UI/SeleccionTelefonoForm.cs`
- `csharp_simulador/SimuladorTelefonico/UI/AdministracionTelefonicaForm.cs`
- `java_proveedor/src/services/RegistrarMovimiento.java`
- `java_proveedor/src/services/VerificarSaldo.java`
- `database/sqlserver_proveedor/migrations/017_normalizar_tarifas_extranjeras.sql`

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

- Login administrativo en C#.
- Validacion contra WS Autenticacion.
- Tipo administrador oculto.
- Contrasena cifrada.
- Redireccion a WebAdministrativo.
- Mensaje para credenciales incorrectas.
- Logo e icono aplicado.

Cambios frente al informe 7:

- Mejora visual por logo y fondo con marca.
- Mantiene separacion real por tipo de usuario.

Pendientes o riesgos:

- Riesgo bajo: depende de MongoDB levantado manualmente.

Nivel de cumplimiento estimado: 92%.

### 5.2 ADM2 - Plantilla y administracion de clientes

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Site.Master`
- `dotnet_webapps/WebAdministrativo/Site.Master.cs`
- `dotnet_webapps/WebAdministrativo/Styles/site.css`
- `dotnet_webapps/WebAdministrativo/Assets/Logo.png`

Lo que ya esta:

- Plantilla general.
- Menu ADM3-ADM7.
- Opcion de salir del sitio.
- Footer.
- Icono/logo de empresa.
- Navegacion persistente.
- Estilo visual consistente.
- Menu extendido con solicitudes de linea.

Cambios frente al informe 7:

- Se agrego logo real.
- Se aplico marca visual en login y pantallas administrativas.
- La estetica paso de pendiente a bastante defendible.

Pendientes o riesgos:

- El menu tiene una opcion adicional no oficial (`Solicitudes de lineas`), pero
  es complementaria y no rompe el alcance.

Nivel de cumplimiento estimado: 94%.

### 5.3 ADM3 - Poner nuevas lineas a disposicion

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasNuevas.aspx`
- `dotnet_webapps/WebAdministrativo/LineasNuevas.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/ProveedorSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/AdministracionTelefonica.java`
- `java_proveedor/src/database/ServicioDAO.java`

Lo que ya esta:

- Lista lineas disponibles.
- Muestra numero, identificadores y tipo.
- Permite eliminar con confirmacion.
- Permite crear nuevas lineas.
- Genera SIM/IMEI con formato definido por el equipo.
- Valida duplicados de numero e identificadores.
- Registra estado `DISPONIBLE`.
- Usa WS Proveedor.

Cambios frente al informe 7:

- El usuario confirma que la prueba ADM3 con `ENC_SIM_` / `ENC_IMEI_` ya fue
  validada y evidenciada.
- Mejora visual aplicada a la pantalla.

Pendientes o riesgos:

- Riesgo bajo: mantener datos reales limpios para demo.

Nivel de cumplimiento estimado: 95%.

### 5.4 ADM4 - Activar linea vendida

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasActivar.aspx`
- `dotnet_webapps/WebAdministrativo/LineasActivar.aspx.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Validators/ActivarDesactivarLineaValidator.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- Lista lineas disponibles.
- No lista lineas ya asignadas como activas.
- Permite asociar cliente real desde MongoDB.
- Permite PREPAGO y POSTPAGO.
- Valida linea e identificadores.
- Cambia estado a activo mediante WS_PROVEEDOR2.
- Evita duplicidad de numero activo.

Cambios frente al informe 7:

- El usuario confirma que asignacion PREPAGO/POSTPAGO y desaparicion de
  disponibles ya fueron probadas.
- Se agrego flujo alterno desde solicitudes de cliente.

Pendientes o riesgos:

- Riesgo bajo: que MongoDB y SQL Server tengan datos consistentes antes de demo.

Nivel de cumplimiento estimado: 95%.

### 5.5 ADM5 - Devolucion/desactivacion administrativa de linea

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasDevolucion.aspx`
- `dotnet_webapps/WebAdministrativo/LineasDevolucion.aspx.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`
- `database/sqlserver_proveedor/migrations/013_normalizar_estado_linea_disponible.sql`

Lo que ya esta:

- Lista lineas activas.
- Muestra identificadores y cliente.
- Solicita confirmacion.
- Devuelve la linea a estado `DISPONIBLE`.
- Usa WS Proveedor.

Cambios frente al informe 7:

- El usuario confirma que la linea asignada aparece en devolucion y que el flujo
  fue validado.
- Mejora visual aplicada.

Pendientes o riesgos:

- Explicar en demo que `DISPONIBLE` es el equivalente operativo de linea
  devuelta para inventario.

Nivel de cumplimiento estimado: 94%.

### 5.6 Resultado Charlie

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM1 | Cumple | 92% | MongoDB levantado en demo. |
| ADM2 | Cumple alto | 94% | Mantener ruta oficial y explicar opcion extra de solicitudes. |
| ADM3 | Cumple alto | 95% | Mantener base limpia. |
| ADM4 | Cumple alto | 95% | Mantener clientes reales consistentes. |
| ADM5 | Cumple alto | 94% | Explicar `DISPONIBLE` como devuelta/inventario. |

Riesgo principal: bajo. Charlie queda muy cerca de cierre.

## 6. Jose - ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3

Responsabilidad segun estrategia 3: facturacion, mantenimiento de usuarios,
login cliente, plantilla cliente y registro cliente.

### 6.1 ADM6 - Calcular facturacion

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Facturacion.aspx`
- `dotnet_webapps/WebAdministrativo/Facturacion.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/ProveedorSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Models/FacturacionConsultaResponse.cs`
- `java_proveedor/src/services/Proveedor6Service.java`
- `java_proveedor/src/database/FacturacionDAO.java`
- `database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql`

Lo que ya esta:

- Consulta individual por linea postpago.
- Separacion entre consultar y generar.
- Generacion de factura solo posterior a consulta.
- Bloqueo si la consulta retorna `0`.
- Proteccion server-side contra factura en cero.
- Fecha maxima no puede ser menor que fecha de calculo.
- Ultimo calculo usa `fecha_registro DESC`.
- Factura generada queda para que el cliente la pague.

Cambios frente al informe 7:

- Se resolvio el ajuste conceptual solicitado por el usuario: admin genera,
  cliente paga.
- Se agrego bloqueo de facturas en cero.
- Se elimina como pendiente la prueba de llamadas postpago/facturacion porque
  el usuario confirma validacion OK.

Pendientes o riesgos:

- Riesgo bajo: si se quiere apego literal al PDF, ADM6 oficial hablaba de
  continuidad de fechas global. La solucion actual fue adaptada al flujo real
  por linea y es mas coherente con el pago cliente.

Nivel de cumplimiento estimado: 96%.

### 6.2 ADM7 - Mantenimiento usuario administrador

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Administradores.aspx`
- `dotnet_webapps/WebAdministrativo/Administradores.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`
- `dotnet_webservices/WS_Autenticacion/Validators/UsuarioValidator.cs`
- `database/mongodb/actualizar_indices_usuarios_por_tipo.js`

Lo que ya esta:

- Lista solo administradores.
- Crea administradores.
- Edita administradores.
- Activa/inactiva.
- Elimina.
- Muestra datos legibles y contrasena enmascarada.
- Usa separacion por tipo.
- Permite misma identificacion como admin y cliente por indices compuestos.
- Regla de contrasena ajustada a minimo 7 caracteres.

Cambios frente al informe 7:

- Se corrigio la regla estricta de 14 caracteres por minimo 7, segun ajuste
  solicitado.
- Se confirmo separacion real admin/cliente.

Pendientes o riesgos:

- Riesgo bajo: el PDF oficial pedia 14 caracteres; el proyecto decidio cambiar
  la regla por usabilidad. Si el profesor exige literalidad, conviene explicar
  que fue decision de alcance interno.

Nivel de cumplimiento estimado: 91%.

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
- Link a registro.
- Redireccion a PortalCliente.
- Envio de identificacion y nombre hacia sesion del portal.
- Logo e icono en login.

Cambios frente al informe 7:

- El login separado de WebCliente recibio mejora visual.
- La redireccion ahora alimenta mejor el saludo y sesion del PortalCliente.

Pendientes o riesgos:

- Riesgo bajo: depende de MongoDB real.

Nivel de cumplimiento estimado: 94%.

### 6.4 CLIENTE2 - Plantilla portal cliente

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webapps/WebCliente/Site.Master`
- `dotnet_webapps/WebCliente/Styles/site.css`
- `dotnet_webservices/PortalCliente/Views/Shared/_Layout.cshtml`
- `dotnet_webservices/PortalCliente/wwwroot/css/site.css`
- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`

Lo que ya esta:

- Menu de Mis lineas, Cargar saldo, Pagar factura y Devolver linea.
- Opcion Solicitar linea agregada.
- Logout hacia login.
- Nombre del cliente por sesion.
- Logo e icono.
- Footer.
- Navegacion persistente.
- Pantalla inicial CLIENTE4.

Cambios frente al informe 7:

- Se elimino el campo editable de cedula en Mis lineas.
- Se muestra identidad desde sesion.
- Se agrego logout real.
- Se agrego solicitar linea.
- Se aplico identidad visual con logo.

Pendientes o riesgos:

- Riesgo bajo: la opcion Solicitar linea no esta en PDF, pero es adicional y
  consistente.

Nivel de cumplimiento estimado: 95%.

### 6.5 CLIENTE3 - Registro cliente

Estado encontrado: Cumple alto con extension funcional.

Evidencia:

- `dotnet_webapps/WebCliente/Registro.aspx`
- `dotnet_webapps/WebCliente/Registro.aspx.cs`
- `dotnet_webapps/WebCliente/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`
- `dotnet_webservices/WS_Autenticacion/Data/Usuariorepository.cs`
- `dotnet_webservices/WS_Autenticacion/Models/Usuario.cs`

Lo que ya esta:

- Registro cliente tipo 2.
- Estado activo.
- Validaciones de campos.
- Validacion de correo.
- Contrasena minimo 7 con complejidad.
- Registra metodo de pago.
- Tarjeta de 12 digitos.
- Nombre de titular.
- Vencimiento MM/AA vigente.
- CVV de 3 digitos.
- Formulario en dos columnas.
- Opcion de volver al login.

Cambios frente al informe 7:

- Se agrego metodo de pago en registro.
- El formulario fue reorganizado en datos personales y datos bancarios.
- El login/registro tiene logo y fondo visual.

Pendientes o riesgos:

- Riesgo medio-bajo: el PDF no pide metodo de pago en CLIENTE3, pero la
  extension reduce errores en CLIENTE5/CLIENTE6.
- Riesgo bajo: igual que ADM7, la regla de 14 caracteres fue relajada.

Nivel de cumplimiento estimado: 93%.

### 6.6 Resultado Jose

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM6 | Cumple alto | 96% | Defender adaptacion de facturacion individual si preguntan por continuidad global. |
| ADM7 | Cumple alto | 91% | Explicar regla de contrasena minimo 7 si se compara literal con PDF. |
| CLIENTE1 | Cumple alto | 94% | MongoDB levantado en demo. |
| CLIENTE2 | Cumple alto | 95% | Mantener sesion limpia y logout probado. |
| CLIENTE3 | Cumple alto | 93% | Defender metodo de pago como extension funcional. |

Riesgo principal: bajo. Jose queda casi cerrado.

## 7. Gabriel - CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7

Responsabilidad segun estrategia 3: autogestion transaccional del cliente.

### 7.1 CLIENTE4 - Mostrar lineas asociadas al cliente

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/Index.cshtml`
- `dotnet_webservices/PortalCliente/Models/IndexViewModel.cs`
- `dotnet_webservices/PortalCliente/Services/ProveedorClienteSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- Consulta lineas por identificacion de sesion.
- Muestra nombre del cliente.
- Lista prepago con saldo.
- Lista postpago con factura pendiente.
- Si no tiene lineas, permite solicitar una.
- No permite editar cedula en pantalla.
- Usa WS_ProveedorCliente.

Cambios frente al informe 7:

- Se reemplazo cedula editable por identidad de sesion.
- Se agrego solicitar linea cuando no hay lineas o cuando el cliente quiere una
  adicional.
- La factura pendiente se consulta por ultimo registro real.

Pendientes o riesgos:

- Riesgo bajo: requiere sesion valida desde WebCliente.

Nivel de cumplimiento estimado: 94%.

### 7.2 CLIENTE5 - Cargar saldo a linea prepago

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/CargarSaldo.cshtml`
- `dotnet_webservices/PortalCliente/Models/MetodoPagoCliente.cs`
- `dotnet_webservices/PortalCliente/Services/AutenticacionPortalSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- Lista lineas prepago del cliente.
- Valida que la linea pertenezca al cliente.
- Precarga metodo de pago registrado.
- Valida tarjeta, vencimiento y CVV.
- Valida monto positivo.
- Recarga saldo por WS.

Cambios frente al informe 7:

- Los datos de tarjeta ya no dependen de captura manual repetida.
- Se precargan desde MongoDB via WS Autenticacion.

Pendientes o riesgos:

- Riesgo bajo/medio: el backend valida positivo, pero no fuerza estrictamente
  entero sin decimales en todos los caminos. La UI puede controlar esto.

Nivel de cumplimiento estimado: 90%.

### 7.3 CLIENTE6 - Pagar factura postpago

Estado encontrado: Cumple alto con pendiente SMTP.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/PagarFactura.cshtml`
- `dotnet_webservices/PortalCliente/Services/EmailService.cs`
- `dotnet_webservices/PortalCliente/appsettings.json`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- Lista lineas postpago.
- Muestra factura pendiente o 0.
- Bloquea pago si no hay factura.
- Monto no editable desde el cliente.
- Precarga metodo de pago.
- Valida tarjeta.
- Paga por WS_ProveedorCliente.
- Cancela factura poniendo total en 0.
- Marca consumo del periodo como pagado.
- Intenta enviar correo con detalle.

Cambios frente al informe 7:

- Se corrigio criterio de ultima factura a `fecha_registro DESC`.
- Se corrigio reinicio de consumo/factura despues del pago.
- Se precargan datos bancarios.

Pendientes o riesgos:

- Pendiente real: configurar SMTP con credenciales/servidor validos.
- No hay tabla historica separada de pagos; el estado queda representado por
  factura en 0 y llamadas `PAGADA`.

Nivel de cumplimiento estimado: 88%.

### 7.4 CLIENTE7 - Devolucion de linea por cliente

Estado encontrado: Cumple alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/DevolverLinea.cshtml`
- `dotnet_webservices/PortalCliente/Services/Proveedor2SoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- Lista prepago y postpago del cliente.
- Consulta saldo para prepago.
- Muestra factura pendiente para postpago.
- Bloquea devolucion si postpago tiene deuda.
- Valida pertenencia de la linea.
- Envia cambio de estado via WS_PROVEEDOR2.
- Devuelve a `DISPONIBLE`.

Cambios frente al informe 7:

- Se beneficia de CLIENTE6, porque despues de pagar se limpia la factura y el
  cliente puede devolver la linea.
- Se beneficia del logout/sesion y mejora visual.

Pendientes o riesgos:

- Explicar que `DISPONIBLE` es el estado operativo usado por el proyecto.

Nivel de cumplimiento estimado: 92%.

### 7.5 Solicitar linea - Extension cliente/admin

Estado encontrado: Funcional complementario.

Evidencia:

- `dotnet_webservices/PortalCliente/Views/Cliente/SolicitarLinea.cshtml`
- `dotnet_webservices/PortalCliente/Models/SolicitarLineaViewModel.cs`
- `dotnet_webservices/PortalCliente/Services/Proveedor2SoapClient.cs`
- `dotnet_webapps/WebAdministrativo/SolicitudesLineas.aspx`
- `dotnet_webapps/WebAdministrativo/SolicitudesLineas.aspx.cs`
- `database/sqlserver_proveedor/migrations/016_solicitudes_linea_cliente.sql`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`

Lo que ya esta:

- Cliente solicita linea disponible.
- Solicitud queda pendiente.
- No autoasigna.
- Admin puede aprobar y asignar.
- Admin puede rechazar.
- Se evita duplicar solicitud pendiente por linea/cliente.

Impacto:

- No suma como historia oficial del PDF.
- Mejora la coherencia funcional de CLIENTE4 y ADM4.

### 7.6 Resultado Gabriel

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| CLIENTE4 | Cumple alto | 94% | Sesion real desde login. |
| CLIENTE5 | Cumple alto | 90% | Reforzar entero sin decimales si se exige literalidad. |
| CLIENTE6 | Cumple alto con pendiente SMTP | 88% | Configurar SMTP real y evidenciar correo. |
| CLIENTE7 | Cumple alto | 92% | Explicar `DISPONIBLE` como linea devuelta. |

Riesgo principal: medio-bajo por SMTP. Funcionalmente, Gabriel queda alto.

## 8. Estado de Web Services requeridos por el PDF

| Servicio / necesidad | Estado actual | Historias afectadas |
|---|---|---|
| WS_AUTENTICACION1 - login por tipo | Funcional y compila | ADM1, CLIENTE1 |
| WS_AUTENTICACION2 - CRUD/cambio estado usuarios | Funcional y compila | ADM7, CLIENTE3 |
| WS_AUTENTICACION2 - metodo de pago cliente | Funcional agregado | CLIENTE3, CLIENTE5, CLIENTE6 |
| WS_PROVEEDOR1 - registrar nueva linea | Funcional | ADM3 |
| WS_PROVEEDOR2 - activar/desactivar lineas | Funcional | ADM4, ADM5, CLIENTE7 |
| WS_PROVEEDOR3 / PROVEEDOR6 - facturacion | Funcional individual por linea | ADM6, CLIENTE6 |
| WS Proveedor - consultas administrativas | Funcional | ADM3-ADM6, solicitudes |
| WS Proveedor - solicitudes de linea | Funcional agregado | CLIENTE4, ADM4 |
| WS_ProveedorCliente - consultar lineas | Funcional | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |
| WS_ProveedorCliente - recargar saldo | Funcional | CLIENTE5 |
| WS_ProveedorCliente - pagar factura | Funcional; limpia factura y consumo | CLIENTE6 |
| Correo de factura | Parcial por configuracion | CLIENTE6 |

Comparacion contra informe 7:

- WS Proveedor sube por solicitudes y bloqueo de factura en cero.
- WS ProveedorCliente sube por orden correcto de factura pendiente y consumo
  pagado.
- WS Autenticacion sube por metodo de pago y separacion por tipo.
- SMTP se mantiene como unico pendiente funcional visible.

## 9. Lo que ya esta defendible

- Compilacion principal de WebApps y servicios WCF.
- Compilacion de PortalCliente.
- Compilacion de simulador C#.
- Compilacion de Java proveedor.
- Chequeo de sintaxis Python del Identificador.
- Lectura del PDF oficial y cruce contra historias.
- Login administrativo por WS.
- Login cliente por WS.
- Registro cliente con metodo de pago.
- CRUD de administradores.
- Separacion admin/cliente por tipo en MongoDB.
- Misma identificacion con dos roles.
- Plantillas con logo, favicon, menu y footer.
- Logout administrativo y cliente.
- ADM3 alta/eliminacion de lineas disponibles.
- ADM4 asignacion de lineas PREPAGO/POSTPAGO.
- ADM5 devolucion administrativa.
- ADM6 consulta y generacion de factura separadas.
- ADM6 bloqueo de factura en cero.
- CLIENTE4 consulta de lineas por sesion.
- CLIENTE4 solicitud de linea como extension.
- CLIENTE5 recarga prepago con metodo de pago precargado.
- CLIENTE6 pago postpago con monto no editable.
- CLIENTE6 reinicio de factura y consumo pagado.
- CLIENTE7 devolucion cliente con bloqueo por deuda.
- Polling en simulador.
- Tarifas actualizadas segun nacional/extranjero y tipo de servicio.
- WebAdministrativa MVC identificada como no oficial/no defendible.

## 10. Lo que falta para cierre defendible

Prioridad alta:

1. Configurar SMTP real para CLIENTE6.
2. Probar envio real de correo con una factura pagada.
3. Documentar en la entrega final que ADM6 fue implementado como consulta y
   generacion individual por linea, y que el pago corresponde al cliente.

Prioridad media:

1. Si el profesor exige literalidad estricta, decidir si se vuelve a regla de
   contrasena de 14 caracteres o se justifica el minimo de 7.
2. Si el profesor exige monto entero sin decimales en CLIENTE5, reforzar la
   validacion backend ademas de UI.
3. Documentar explicitamente que `DISPONIBLE` es el estado usado para lineas
   devueltas/inactivas en inventario.
4. Mantener fuera de demo la MVC antigua `dotnet_webservices/WebAdministrativa`.
5. Consolidar en la documentacion final la nueva tabla
   `dbo.solicitudes_linea` y el flujo de aprobacion administrativa.

Prioridad baja:

1. Limpiar archivos generados de build si se quiere dejar el repo mas pulcro.
2. Revisar textos sin tilde/acentos si se desea pulido final visual.
3. Preparar un guion corto de demo con el orden oficial de pantallas.

## 11. Recomendacion por responsable

### Charlie

Ruta corta para defensa:

1. Entrar por WebAdministrativo.
2. Mostrar menu, logo, footer y salir del sitio.
3. Mostrar nuevas lineas disponibles.
4. Crear una linea.
5. Asignarla a cliente.
6. Mostrar que ya no aparece como disponible.
7. Devolverla y explicar que vuelve a `DISPONIBLE`.
8. Si se usa el flujo nuevo, mostrar solicitud de cliente y aprobacion admin.

### Jose

Ruta corta para defensa:

1. Mostrar ADM6 como consulta primero.
2. Consultar una linea con consumo.
3. Generar factura solo si el total es mayor a 0.
4. Consultar una linea sin consumo y mostrar que no permite factura en cero.
5. Mostrar administradores filtrados por tipo.
6. Crear/editar/cambiar estado/eliminar admin.
7. Mostrar login cliente y registro cliente con metodo de pago.

### Gabriel

Ruta corta para defensa:

1. Entrar desde WebCliente/Login.
2. Mostrar PortalCliente con nombre y sin cedula editable.
3. Mostrar Mis lineas.
4. Si no hay linea, solicitar una.
5. Recargar prepago con metodo de pago precargado.
6. Pagar factura postpago y confirmar que queda en 0.
7. Devolver linea sin deuda.
8. Para manana: completar SMTP y evidenciar correo.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra historias asignadas en `estrategia_3.md`:

```text
Charlie : [#########-] 94% de 100
Jose    : [#########-] 94% de 100
Gabriel : [#########-] 91% de 100
```

Lectura rapida:

- Charlie: sube por logo, validacion funcional confirmada y flujo de
  solicitudes conectado con ADM4.
- Jose: sube por cierre de ADM6, separacion real de usuarios y registro cliente
  con metodo de pago.
- Gabriel: sube por sesion, metodo de pago precargado, pago que limpia consumo
  y solicitud de linea; se mantiene algo menor por SMTP pendiente.

Promedio general estimado:

```text
Proyecto completo: [#########-] 93% de 100
```

## 13. Conclusion

El proyecto esta en estado de cierre alto frente al informe 7 y frente al PDF
oficial. La arquitectura requerida se respeta, las Web principales estan en C#,
las operaciones pasan por Web Services, los servicios principales compilan y el
flujo funcional de administracion/cliente ya esta integrado.

La diferencia clave frente al informe 7 es que muchas deudas dejaron de ser
tecnicas: migraciones, reinicios, pruebas de ADM3-ADM6, consumo postpago,
factura pendiente y evidencias ya fueron validadas por el usuario. Ademas, el
codigo actual corrige puntos que el informe 7 marcaba como riesgo, especialmente
la lectura de ultima factura y el pago que deja consumo en cero.

El unico pendiente funcional de peso es SMTP. Cuando se configure y se capture
el correo real de CLIENTE6, el proyecto quedaria en una posicion muy solida para
defensa. Para la demo, la recomendacion es no abrir la MVC antigua, mantener los
datos reales limpios y explicar con claridad tres decisiones del equipo:
facturacion individual por linea, estado `DISPONIBLE` como linea devuelta, y
contrasena minima de 7 caracteres en lugar de la regla literal de 14 del PDF.
