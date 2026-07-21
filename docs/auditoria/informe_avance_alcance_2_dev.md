# Informe de avance - Alcance 2 contra rama dev

Fecha de analisis: 2026-07-20  
Rama revisada: `dev` sincronizada con `origin/dev`  
Repositorio: `Central_TG`

## 1. Resumen ejecutivo

El proyecto tiene avances reales, especialmente en el flujo asignado a Jose:
`WS_PROVEEDOR2 -> PROVEEDOR5 -> IDENTIFICADOR6 -> MySQL/SQL Server`.
Ese bloque cuenta con codigo, contratos, migraciones y evidencias especificas.

Sin embargo, el alcance 2 no esta completo como entrega integral. Hay historias
que existen solo parcialmente, estan ubicadas en un componente distinto al
solicitado por el PDF, o estan escritas pero no integradas al ejecutable principal.

Aclaracion importante sobre contratos: desde el alcance 1 el equipo dejo
establecidos contratos JSON para la comunicacion interna entre componentes. Para
el alcance 2 esa decision se mantiene. XML/SOAP aplica en la frontera de los Web
Services cuando el requerimiento lo exige, pero la comunicacion interna puede
seguir usando JSON siempre que el WS traduzca correctamente entre XML/SOAP y el
contrato JSON existente. Por tanto, el uso de JSON no se considera por si mismo
un incumplimiento; el punto a verificar es que los servicios SOAP existan,
reciban/entreguen XML correctamente y respeten los contratos internos acordados.

Estado global estimado:

| Area | Estado | Comentario |
|---|---|---|
| Distribucion de responsabilidades | Cumple | `orden.md` asigna Gabriel, Jose y Charlie con minimo 3 historias. |
| Simulador C# | Cumple como base | Compila correctamente; corresponde mas al alcance anterior y apoyo de pruebas. |
| Proveedor Java | Parcial | Tiene PROVEEDOR5 y funciones previas; falta PROVEEDOR4/6 conforme PDF. |
| Identificador Python | Parcial | Tiene IDENTIFICADOR6; tambien contiene un handler llamado PROVEEDOR4, pero no corresponde al bloque Proveedor/SQL Server. |
| WS Proveedor | Parcial | Existe WCF para WS_PROVEEDOR2; no se evidencian WS_PROVEEDOR1/3 completos en C# SOAP. |
| WS Autenticacion | Parcial bajo | Hay clases de servicio/validacion MongoDB, pero no estan compiladas por el proyecto principal y no hay proyecto SOAP completo. |
| WS Identificador | Pendiente | No se encontro implementacion C# SOAP de WS_IDENTIFICADOR1. |
| MongoDB | Parcial | Hay scripts de coleccion, indices y README; falta confirmar servicio SOAP funcional y cifrado requerido. |
| Documentacion final | Parcial | Hay estrategia, orden, contrato de Jose y evidencias de Jose; faltan evidencias completas de Gabriel y Charlie. |

## 2. Requerimientos base revisados

Documentos fuente:

- `docs/Proyecto/Proyecto - Alcance 2.pdf`
- `docs/estrategia/estrategia_2.md`
- `docs/estrategia/orden.md`
- `docs/roadmaps/roadmap_2.md`

Historias y responsables acordados:

| Integrante | Rol | Historias |
|---|---|---|
| Gabriel | Companero 1 | PROVEEDOR4, WS_PROVEEDOR1, WS_IDENTIFICADOR1 |
| Jose | Companero 2 | PROVEEDOR5, IDENTIFICADOR6, WS_PROVEEDOR2 |
| Charlie | Companero 3 | PROVEEDOR6, WS_PROVEEDOR3, WS_AUTENTICACION1, WS_AUTENTICACION2 |

Reglas tecnicas relevantes del PDF:

- WS Proveedor y WS Identificador deben estar en C#.
- Todos los web services deben usar SOAP.
- WS Autenticacion puede usar otra herramienta, pero debe usar SOAP/XML.
- Proveedor trabaja con SQL Server.
- Identificador trabaja con MySQL.
- Autenticacion trabaja con MongoDB.
- Usuario, contrasena y datos telefonicos sensibles deben viajar y almacenarse cifrados.
- Los componentes deben poder probarse integrados; si algo falta, debe existir stub o simulador.

## 3. Verificaciones ejecutadas

| Verificacion | Resultado |
|---|---|
| `git status --short --branch` | Rama `dev...origin/dev`, sin cambios locales antes del reporte. |
| `dotnet build csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj --no-restore` | Correcto, 0 errores. |
| `dotnet build dotnet_webservices/CentralTelefonica.WebServices/CentralTelefonica.WebServices.csproj --no-restore` | Correcto, 0 errores, pero compila el host minimo REST, no las carpetas WCF excluidas. |
| `python -m compileall python_identificador` | Correcto. |
| `javac -d build ...` en `java_proveedor` | Correcto sin incluir el jar de SQL Server. |
| `javac -cp lib/mssql-jdbc.jar ...` | Falla por `AccessDeniedException` sobre el jar, no por error de sintaxis. |
| `msbuild WS_Proveedor.csproj` | No ejecutable: `msbuild` no esta en PATH. Requiere Visual Studio/MSBuild clasico. |

## 4. Hallazgos transversales

### 4.1 Coexistencia JSON y XML/SOAP

El equipo mantiene los contratos JSON definidos desde el alcance 1 para la
comunicacion interna entre C#, Python, Java y bases de datos. En el alcance 2,
los servicios web deben actuar como frontera SOAP/XML cuando corresponda, pero
pueden transformar la solicitud SOAP a JSON interno para reutilizar contratos ya
estables.

Ejemplos de contratos internos JSON vigentes:

- `docs/contratos/jose_activacion_desactivacion.md`
- `shared/contracts/activar_desactivar_linea_proveedor5.json`
- `java_proveedor/src/services/Proveedor5Service.java`
- `python_identificador/app/sockets/handler.py`

Esto debe quedar explicado en la documentacion final como una decision de
arquitectura: XML/SOAP en la capa de Web Services; JSON en la capa interna de
integracion. Con esa aclaracion, el riesgo no es usar JSON, sino no tener el WS
SOAP/XML funcional que haga la traduccion.

### 4.2 El proyecto .NET principal no compila los WS reales

`dotnet_webservices/CentralTelefonica.WebServices/CentralTelefonica.WebServices.csproj`
excluye:

- `WS_Autenticacion/**/*.cs`
- `WS_Proveedor/**/*.cs`
- `MinimalHost.cs`

Por eso el build verde del proyecto principal solo valida `Program.cs`, que expone:

- `/health`
- `/autenticacion/login`
- `/proveedor/facturacion`

Estos endpoints son REST/minimal API y responden "Servicio listo"; no son SOAP ni
ejecutan las clases WCF/MongoDB reales.

### 4.3 Evidencias desbalanceadas

Hay evidencias detalladas para Jose:

- `docs/evidencias/jose/PROVEEDOR5/prueba_proveedor5.md`
- `docs/evidencias/jose/IDENTIFICADOR6/prueba_identificador6.md`
- `docs/evidencias/jose/WS_PROVEEDOR2/prueba_ws_proveedor2.md`

No se encontro un equivalente de evidencias completas para Gabriel ni Charlie.

## 5. Gabriel - Companero 1

Responsabilidad: alta de lineas y consulta de saldo.

### 5.1 PROVEEDOR4

Requerido por PDF:

- Recibir trama de texto plano desde WS Proveedor.
- Registrar nueva linea disponible en Proveedor/SQL Server.
- Validar datos completos.
- Validar numero no usado.
- Cifrar y almacenar numero, identificador de telefono e identificador de tarjeta.
- Responder exactamente: `OK`, `Datos Incompletos`, `Telefono en uso`, `ERROR`.

Estado encontrado: Parcial / no ubicado en el componente correcto.

Evidencia:

- Existe `python_identificador/app/services/proveedor4_handler.py`.
- El handler valida campos, cifra con AES, verifica duplicado e inserta catalogo.
- Pero el propio archivo indica que almacena en MySQL.
- La historia PROVEEDOR4 pertenece al Proveedor y debe persistir en SQL Server.
- Usa JSON (`REGISTRAR_LINEA`) como contrato interno, consistente con la decision
  del equipo desde el alcance 1.
- No valida longitud de identificador de telefono de 16 digitos ni tarjeta de 19
  digitos en la funcion `validar_campos`; solo valida existencia, tipo y estado.
- Inserta con `proveedor_codigo="SISTEMA"`, pero el repositorio busca proveedor
  por codigo activo; si `SISTEMA` no existe, la insercion falla.
- Inserta `activo=True`, aunque el PDF pide estado `disponible`.

Conclusion:

Hay una implementacion parecida al alta, pero no cumple completamente PROVEEDOR4
porque esta en Python/Identificador/MySQL y no en Proveedor/SQL Server. Debe
reubicarse o replicarse en el Proveedor Java/SQL Server con el contrato oficial.

Nivel de cumplimiento: 35%.

Pendientes:

- Implementar PROVEEDOR4 en Proveedor/SQL Server.
- Documentar formalmente que el WS recibe SOAP/XML y traduce a JSON interno.
- Validar 16 y 19 digitos.
- Guardar como linea disponible, no activa.
- Responder con mensajes exactos.
- Agregar evidencias de pruebas positivas y negativas.

### 5.2 WS_PROVEEDOR1

Requerido por PDF:

- Servicio Web SOAP en C#.
- Recibe numero, identificador telefono, identificador tarjeta, tipo y estado.
- Todos los datos llegan encriptados.
- Construye trama hacia PROVEEDOR4.
- Si PROVEEDOR4 responde OK: `Resultado=true`, `Mensaje=Exitoso`.
- Si no: `Resultado=false`, `Mensaje=Problemas al incluir la informacion.`

Estado encontrado: Pendiente / no evidenciado.

Evidencia:

- No se encontro operacion WCF/SOAP de registro de linea en `WS_Proveedor`.
- `WS_Proveedor` contiene principalmente `ActivarDesactivarLinea`, asociado a
  WS_PROVEEDOR2.
- `Program.cs` tiene endpoints REST de prueba, no SOAP.

Conclusion:

WS_PROVEEDOR1 no esta listo como historia demostrable segun el PDF.

Nivel de cumplimiento: 10% por estructura parcial de WS Proveedor, sin operacion.

Pendientes:

- Agregar operacion SOAP `RegistrarLinea` o equivalente.
- Validar datos cifrados.
- Invocar PROVEEDOR4 real.
- Preparar prueba SOAP y evidencia en SQL Server.

### 5.3 WS_IDENTIFICADOR1

Requerido por PDF:

- Servicio Web SOAP/XML en C# para consulta de saldo.
- Recibe XML con telefono cifrado, origen Web/telefono y tipo transaccion saldo.
- Si origen es Web solo esos datos son obligatorios.
- Si origen es telefono debe validar el flujo completo existente.
- Envia trama al Proveedor y devuelve XML con resultado y saldo.
- Debe modificar Proveedor4 para aceptar nuevo campo/origen.

Estado encontrado: Pendiente / no evidenciado.

Evidencia:

- No se encontro carpeta ni proyecto `WS_Identificador` con archivos.
- Existe consulta de saldo en Python/Java/C# del alcance anterior:
  - `python_identificador/app/services/consulta.py`
  - `java_proveedor/src/services/ConsultaSaldo.java`
  - `csharp_simulador/SimuladorTelefonico/UI/ConsultaSaldoForm.cs`
- Esa consulta usa JSON por socket, no SOAP/XML C#.

Conclusion:

La funcionalidad de consulta de saldo existe parcialmente en el flujo previo, pero
no existe como WS_IDENTIFICADOR1 SOAP/XML en C#.

Nivel de cumplimiento: 20% funcional indirecto; 0% como WS solicitado.

Pendientes:

- Crear WS Identificador en C# con SOAP.
- Implementar contrato XML de entrada/salida.
- Diferenciar origen Web vs telefono.
- Conectar con Proveedor.
- Documentar pruebas XML/SOAP.

### 5.4 Resultado Gabriel

| Historia | Estado | Cumple PDF | Observacion | Que falta programar | Herramienta recomendada |
|---|---|---|---|---|---|
| PROVEEDOR4 | Parcial | No completo | Existe handler en Python/MySQL, no Proveedor/SQL Server. | Logica real de alta de linea disponible en Proveedor/SQL Server; validar 16/19 digitos, duplicados, estado disponible y cifrado. | VS Code o IDE Java para `java_proveedor`; SQL Server Management Studio para scripts y pruebas. |
| WS_PROVEEDOR1 | Pendiente | No | No se encontro operacion SOAP de alta. | Operacion SOAP `RegistrarLinea`, validacion de datos cifrados y traduccion SOAP/XML -> JSON interno para PROVEEDOR4. | Visual Studio Community; proyecto WCF/.NET Framework similar a `WS_Proveedor`. |
| WS_IDENTIFICADOR1 | Pendiente | No | Solo existe consulta de saldo del flujo anterior por socket JSON. | Servicio C# SOAP/XML para consulta de saldo, origen Web/telefono y traduccion hacia el contrato interno de consulta. | Visual Studio Community; WCF o servicio SOAP equivalente en C#. |

Riesgo principal: alto. Gabriel necesita completar o evidenciar tres historias
para poder defender evaluacion individual.

## 6. Jose - Companero 2

Responsabilidad: activacion/desactivacion e integracion Proveedor-Identificador.

### 6.1 PROVEEDOR5

Requerido por PDF:

- Recibir trama desde WS_PROVEEDOR2.
- Validar datos completos.
- Validar linea disponible para activar y activa para desactivar.
- Validar pertenencia al dueno en desactivacion.
- Activar asociando cliente.
- Si es prepago, saldo inicial 1000.
- Sincronizar con IDENTIFICADOR6.
- Responder OK solo si Identificador responde OK.
- Responder mensajes exactos de error.

Estado encontrado: Cumple parcialmente alto.

Evidencia:

- `java_proveedor/src/services/Proveedor5Service.java`
- `java_proveedor/src/database/ServicioDAO.java`
- `java_proveedor/src/services/Identificador6Client.java`
- `java_proveedor/src/sockets/ManejoCliente.java`
- `database/sqlserver_proveedor/migrations/009_proveedor5_linea.sql`
- `docs/evidencias/jose/PROVEEDOR5/prueba_proveedor5.md`

Lo que ya esta:

- Valida campos obligatorios.
- Valida tipo `PREPAGO`/`POSTPAGO`.
- Valida accion `ACTIVAR`/`DESACTIVAR`.
- Consulta linea en SQL Server.
- Detecta linea activa como `Telefono en uso`.
- Detecta desactivacion no valida como `Telefono no corresponde`.
- Llama a `Identificador6Client` antes de confirmar cambio local.
- Activa en SQL Server y asigna datos cifrados.
- Crea o actualiza saldo inicial prepago en 1000.
- Desactiva dejando `activo=0`, `estado_linea='DISPONIBLE'` y dueno nulo.
- Hay evidencia documentada de activacion, desactivacion y datos incompletos.

Pendientes o riesgos:

- Usa JSON interno, lo cual es consistente con los contratos del alcance 1.
- La lectura de JSON se hace manualmente con busqueda de strings; puede fallar con
  JSON valido pero distinto en orden/escape.
- El flujo sincroniza Identificador antes de actualizar SQL Server. Eso reduce el
  riesgo de SQL activo/MySQL fallido, pero puede dejar MySQL actualizado si luego
  falla SQL Server. Falta compensacion/rollback logico documentado y automatizado.
- `ManejoCliente` enruta PROVEEDOR5 por el campo `accion`, no por
  `tipo_transaccion`. Funciona para `ACTIVAR`/`DESACTIVAR`, pero es fragil.
- Las pruebas de todos los escenarios ya fueron realizadas por Jose fuera del
  proyecto. Lo que falta dentro del repositorio es documentarlas con capturas y
  evidencia formal del proceso.

Nivel de cumplimiento: 75%.

### 6.2 IDENTIFICADOR6

Requerido por PDF:

- Recibir trama del Proveedor.
- Validar datos completos.
- Validar datos cifrados.
- Incluir o actualizar datos en telefonos asociados al Proveedor en MySQL.
- Responder `OK` o `Activacion fallida`.

Estado encontrado: Cumple parcialmente alto.

Evidencia:

- `python_identificador/app/services/identificador6.py`
- `python_identificador/app/database/repositorio.py`
- `python_identificador/app/sockets/handler.py`
- `database/mysql_identificador/migrations/010_identificador6_cliente.sql`
- `docs/evidencias/jose/IDENTIFICADOR6/prueba_identificador6.md`

Lo que ya esta:

- Router de Python acepta `IDENTIFICADOR6`.
- Valida campos obligatorios.
- Valida tipo de servicio y accion.
- Descifra AES para comprobar telefono, dispositivo, tarjeta y cliente.
- Valida longitud de dispositivo 16 y tarjeta 19.
- Inserta o actualiza telefono, tarjeta y dispositivo en MySQL.
- Asocia/desasocia cliente cifrado segun accion.
- Responde con JSON que contiene `codigo: OK` y `mensaje: OK`.
- Evidencia documentada para activacion, desactivacion y AES invalido.

Pendientes o riesgos:

- Usa JSON por socket como contrato interno del alcance 1.
- Responde con JSON estructurado que contiene `codigo: OK`, equivalente interno
  al `OK` requerido por el flujo.
- El PDF indica que la trama debe incluir `Estado: activo`; la implementacion usa
  `accion: ACTIVAR/DESACTIVAR`, aceptable como extension pero no literal.
- Falta evidencia automatizada o scripts reproducibles de prueba.

Nivel de cumplimiento: 80%.

### 6.3 WS_PROVEEDOR2

Requerido por PDF:

- Operacion SOAP en C#.
- Recibe datos cifrados.
- Prepara trama para PROVEEDOR5.
- Si PROVEEDOR5 responde OK: `Resultado=true`, `Mensaje=Exitoso`.
- Si no: `Resultado=false`, `Mensaje=Problemas al activar/desactivar la linea.`

Estado encontrado: Parcial alto, sujeto a Visual Studio/WCF.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/IProveedorService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Services/TramaProveedorService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Infrastructure/ProveedorTcpClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Web.config`
- `docs/evidencias/jose/WS_PROVEEDOR2/prueba_ws_proveedor2.md`

Lo que ya esta:

- Proyecto WCF clasico .NET Framework 4.7.2.
- Contrato SOAP `ActivarDesactivarLinea`.
- Validador de campos obligatorios.
- Validador de Base64 para campos sensibles.
- Construccion de trama PROVEEDOR5.
- Cliente TCP a Java configurable.
- `ProveedorModoSimulado=false` en Web.config.
- Traduce `OK` a `Resultado=true`, `Mensaje=Exitoso`.
- Traduce errores a mensaje requerido.

Pendientes o riesgos:

- No se pudo compilar desde terminal porque no hay `msbuild` clasico en PATH.
- El proyecto principal `CentralTelefonica.WebServices.csproj` excluye esta carpeta,
  asi que `dotnet build` no valida este WS.
- La trama enviada a PROVEEDOR5 es JSON interno, segun los contratos estables del
  equipo.
- No se encontro evidencia ejecutada directamente en SoapUI/WCF Test Client; el
  documento indica que la validacion final esta pendiente en Visual Studio.

Nivel de cumplimiento: 70%.

### 6.4 Resultado Jose

| Historia | Estado | Cumple PDF | Observacion | Que falta programar/documentar | Herramienta recomendada |
|---|---|---|---|---|---|
| PROVEEDOR5 | Parcial alto | Mayormente si | Funcional con JSON interno; falta subir evidencia/capturas de todos los escenarios. | No se identifica programacion critica pendiente; falta documentar capturas de activacion, desactivacion, errores, SQL Server y MySQL. | VS Code/terminal para Java; SQL Server Management Studio; carpeta `docs/evidencias/jose`. |
| IDENTIFICADOR6 | Parcial alto | Mayormente si | Funcional con respuesta JSON estructurada; falta evidencia visual completa. | No se identifica programacion critica pendiente; falta documentar capturas de MySQL, respuestas OK y fallos. | VS Code para Python; MySQL Workbench o cliente MySQL; carpeta `docs/evidencias/jose`. |
| WS_PROVEEDOR2 | Parcial alto | Si, sujeto a prueba WCF | Codigo SOAP existe; falta documentar prueba final en Visual Studio/SoapUI. | Falta capturar prueba SOAP real con `ActivarDesactivarLinea` y respuesta `Resultado=true`. | Visual Studio Community para WCF; SoapUI o WCF Test Client para evidencia. |

Riesgo principal: medio-bajo. Jose tiene las tres historias defendibles y las
pruebas ya fueron realizadas fuera del repositorio. El pendiente principal es
documentar en este proyecto las capturas y evidencias de todos los escenarios.

## 7. Charlie - Companero 3

Responsabilidad: facturacion postpago y autenticacion/usuarios con MongoDB.

### 7.1 PROVEEDOR6

Requerido por PDF:

- Recibir trama desde WS_PROVEEDOR3 con fecha de calculo y fecha maxima de pago.
- Validar datos completos y fechas validas.
- Ejecutar procedimiento almacenado SQL Server que calcule saldo en llamadas de
  todos los servicios postpago.
- Almacenar fecha maxima de pago.
- Responder `OK` o `ERROR`.

Estado encontrado: Parcial bajo.

Evidencia:

- `database/sqlserver_proveedor/schema/002_pa_database_facturacionPostpago.sql`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Services/ProveedorService.cs`

Lo que ya esta:

- Existe un procedimiento `sp_CalcularFacturacionPostpago`.
- Existe una clase C# que llama un SP y devuelve una respuesta de facturacion.

Pendientes o riesgos:

- El SP encontrado recibe `@identificacion_cliente`, `@fecha_inicio` y
  `@fecha_fin`; el PDF pide fecha de calculo y fecha maxima de pago para todos los
  servicios postpago.
- El SP consulta `historial_llamadas`, pero el esquema base del proveedor usa
  tablas como `llamadas_proveedor`; falta confirmar que la tabla exista y coincida.
- No se evidencia almacenamiento de la fecha maxima de pago.
- No se encontro servicio Java/Proveedor que reciba PROVEEDOR6 por socket.
- No se encontro respuesta `OK`/`ERROR` hacia WS_PROVEEDOR3.

Nivel de cumplimiento: 25%.

### 7.2 WS_PROVEEDOR3

Requerido por PDF:

- Operacion SOAP en C#.
- Recibe fecha de calculo y fecha maxima de pago.
- Prepara trama para PROVEEDOR6.
- Si PROVEEDOR6 responde OK: `Resultado=true`, `Mensaje=Exitoso`.
- Si no: `Resultado=false`, `Mensaje=Problemas al realizar el calculo.`

Estado encontrado: Pendiente / parcial minimo.

Evidencia:

- El host principal tiene endpoint REST `/proveedor/facturacion`, pero solo
  responde `"Servicio listo"` y devuelve el request.
- Existe una clase `ProveedorService.cs` con `ObtenerFacturaPostpago`, pero no
  corresponde al contrato WS_PROVEEDOR3 del PDF y no esta compilada por el
  proyecto principal.
- El `WS_Proveedor.csproj` clasico no incluye `Services/ProveedorService.cs` ni
  `Models/FacturacionResponse.cs` segun el listado de Compile; por tanto esa pieza
  no queda dentro del WCF revisado.

Conclusion:

WS_PROVEEDOR3 no esta listo como SOAP demostrable.

Nivel de cumplimiento: 15%.

### 7.3 WS_AUTENTICACION1

Requerido por PDF:

- Servicio Web XML/SOAP.
- Recibe usuario, contrasena y tipo.
- Valida existencia, contrasena, estado activo y tipo.
- Usuario y contrasena viajan encriptados.
- Usuario y contrasena se almacenan encriptados en MongoDB.
- Respuesta correcta: `Resultado=true`, `Mensaje=Exitoso`.
- Respuesta incorrecta: `Resultado=false`, `Mensaje=Usuario y/o contrasena incorrectos.`

Estado encontrado: Parcial bajo.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Autenticacion/Contracts/IAutenticacionService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Autenticacion/Services/AutenticacionService.cs`
- `database/mongodb/crear_coleccion_usuarios.js`
- `database/mongodb/indices_usuarios.js`
- `database/mongodb/README.md`

Lo que ya esta:

- Existe interfaz con `[ServiceContract]` y metodo `AutenticarUsuario`.
- Existe clase que consulta MongoDB.
- Valida usuario, contrasena, tipo y estado activo.
- Hay scripts de coleccion e indices MongoDB.

Pendientes o riesgos:

- No hay `.svc`, `.csproj` o host SOAP completo para WS_Autenticacion.
- El proyecto principal excluye `WS_Autenticacion/**/*.cs`, por lo que estas clases
  no se compilan en el build actual.
- `Program.cs` expone `/autenticacion/login` REST y responde `"Servicio listo"`, sin
  ejecutar `AutenticacionService`.
- Usa SHA-256 para contrasena, no cifrado/encriptacion reversible como lo pide el
  PDF para usuario y contrasena.
- El usuario se almacena/compara en texto plano (`u.Usuario == usuario.Trim()`).
- Los mensajes no coinciden con los textos exactos requeridos.

Nivel de cumplimiento: 30%.

### 7.4 WS_AUTENTICACION2

Requerido por PDF:

- Servicio Web XML/SOAP con tres metodos:
  - crear usuario,
  - modificar usuario,
  - activar/inactivar usuario.
- Validar campos completos, correo, nombres/apellidos, tipo 1/2, estado activo en
  usuarios nuevos y contrasena exacta de 14 caracteres con reglas.
- No permitir duplicados.
- No permitir modificar campos llave.
- Usuario y contrasena viajan y se almacenan encriptados.
- Respuestas exactas segun PDF.

Estado encontrado: Parcial.

Evidencia:

- `AutenticacionService.cs` implementa `CrearUsuario`, `ModificarUsuario`,
  `CambiarEstadoUsuario`.
- `UsuarioValidator.cs` valida correo, nombres, tipo y contrasena de 14 caracteres.
- Scripts MongoDB crean coleccion e indices.

Lo que ya esta:

- Hay logica de CRUD parcial en MongoDB.
- Hay validaciones importantes de contrasena, correo y tipo.
- Hay deteccion de duplicados por usuario, correo e identificacion.

Pendientes o riesgos:

- No esta expuesto como SOAP ejecutable.
- No se compila dentro del proyecto principal.
- Crear usuario permite que el estado venga informado como `inactivo`; el PDF exige
  que usuarios nuevos se creen activos.
- Modificar usuario permite cambiar `Usuario` e `Identificacion`, aunque el PDF
  indica que campos llave no deben modificarse.
- Cambio de estado busca por `usuario`; el PDF pide recibir identificacion y estado.
- Usuario se guarda en texto plano.
- Contrasena se guarda como hash SHA-256, no como cifrado requerido.
- Mensajes de respuesta no son los exactos del PDF.

Nivel de cumplimiento: 35%.

### 7.5 Resultado Charlie

| Historia | Estado | Cumple PDF | Observacion | Que falta programar | Herramienta recomendada |
|---|---|---|---|---|---|
| PROVEEDOR6 | Parcial bajo | No completo | Hay SP, pero no cumple alcance global ni fecha maxima. | Servicio interno del Proveedor para recibir fecha calculo/fecha maxima, ejecutar SP para todos los postpago, guardar fecha maxima y devolver OK/ERROR. | VS Code o IDE Java para `java_proveedor`; SQL Server Management Studio para SP. |
| WS_PROVEEDOR3 | Pendiente/parcial minimo | No | No hay SOAP funcional demostrado. | Operacion SOAP `CalcularFacturacion` que traduzca SOAP/XML -> JSON interno PROVEEDOR6 y devuelva Resultado/Mensaje. | Visual Studio Community; WCF dentro de `WS_Proveedor` o proyecto SOAP C# equivalente. |
| WS_AUTENTICACION1 | Parcial bajo | No completo | Logica existe, pero no host SOAP ni cifrado requerido. | Host SOAP/XML real para login, conexion MongoDB, validacion de credenciales activas y mensajes exactos. | Visual Studio Community si se mantiene C#; WCF/CoreWCF o servicio SOAP equivalente. MongoDB Compass para datos. |
| WS_AUTENTICACION2 | Parcial | No completo | CRUD parcial, pero incumple SOAP, cifrado y reglas llave/estado. | Metodos SOAP para crear, modificar y activar/inactivar; corregir reglas de estado, campos llave, identificacion y cifrado. | Visual Studio Community para servicio; MongoDB Compass/mongosh para validar coleccion e indices. |

Riesgo principal: alto. Charlie necesita integrar el servicio SOAP real, corregir
cifrado/validaciones y completar facturacion postpago conforme al PDF.

## 8. Estado de entregables del PDF

| Entregable | Estado | Evidencia / comentario |
|---|---|---|
| Documentacion analisis/diseno | Parcial | Existen estrategia, orden, docs de entrega final y contrato de Jose. Faltan diagramas actualizados completos para alcance 2. |
| MongoDB | Parcial | Scripts en `database/mongodb`, pero falta validar servicio SOAP real. |
| Codigo Identificador actualizado | Parcial alto | Python compila e incluye IDENTIFICADOR6. |
| Codigo Proveedor actualizado | Parcial | Java compila y tiene PROVEEDOR5; faltan PROVEEDOR4/6 oficiales. |
| Codigo Simulador | Cumple | C# compila correctamente. |
| WS Proveedor | Parcial | WS_PROVEEDOR2 existe; WS_PROVEEDOR1/3 no estan completos. |
| WS Autenticacion | Parcial bajo | Clases existen, pero no host SOAP compilado. |
| WS Identificador | Pendiente | No se encontro implementacion C# SOAP. |

## 9. Lo que ya esta

- Rama `dev` limpia y sincronizada.
- Simulador C# compila.
- Host .NET principal compila, aunque es REST/minimal y no SOAP.
- Python Identificador compila.
- Java Proveedor compila sin el jar en classpath; el error con jar parece de acceso
  del sistema, no de codigo.
- Jose tiene:
  - contrato documentado,
  - PROVEEDOR5 funcional,
  - IDENTIFICADOR6 funcional,
  - WS_PROVEEDOR2 WCF implementado,
  - migraciones SQL/MySQL,
  - evidencias documentadas.
- MongoDB tiene scripts de coleccion e indices.
- Hay avance de logica de autenticacion y administracion de usuarios.
- Hay un SP inicial de facturacion.

## 10. Lo que falta para cierre defendible

Prioridad alta:

1. Completar WS_IDENTIFICADOR1 en C# SOAP.
2. Completar WS_PROVEEDOR1 en C# SOAP.
3. Mover o implementar PROVEEDOR4 en Proveedor/SQL Server.
4. Completar PROVEEDOR6 real en Proveedor/SQL Server.
5. Completar WS_PROVEEDOR3 SOAP y conectarlo a PROVEEDOR6.
6. Crear host/proyecto SOAP real para WS_AUTENTICACION1/2.
7. Corregir cifrado de usuario/contrasena segun requerimiento.
8. Ejecutar pruebas con SoapUI/WCF Test Client y guardar evidencias.

Prioridad media:

1. Documentar formalmente la convivencia XML/SOAP en Web Services y JSON en
   contratos internos.
2. Agregar compensacion si IDENTIFICADOR6 actualiza MySQL pero SQL Server falla.
3. Incorporar al repositorio las evidencias/capturas de las pruebas ya realizadas
   por Jose.
4. Unificar mensajes exactos requeridos por PDF.
5. Actualizar diagramas de base de datos, casos de uso y clases.

## 11. Recomendacion por responsable

### Gabriel

Debe enfocarse en cerrar las tres historias asignadas. Actualmente son el bloque
mas pendiente. La ruta mas corta es:

1. Implementar `PROVEEDOR4` en Java/SQL Server usando tablas `servicios`, saldos y
   campos cifrados de linea.
2. Agregar operacion WCF `RegistrarLinea` en `WS_Proveedor`.
3. Crear proyecto/carpeta `WS_Identificador` C# SOAP para consulta de saldo.
4. Preparar evidencias equivalentes a las de Jose.

### Jose

Debe preparar defensa y pruebas integradas. Su codigo es el mas cercano a
terminado, pero conviene reforzar:

1. Subir al proyecto las capturas de las pruebas ya realizadas.
2. Guardar evidencia real de SOAP para `WS_PROVEEDOR2`.
3. Documentar en el informe final que XML/SOAP vive en el WS y JSON queda como
   contrato interno heredado del alcance 1.
4. Dejar evidencia de fallo de Identificador y compensacion.
5. Dejar evidencia de dueno incorrecto y linea ya activa.

### Charlie

Debe convertir clases sueltas en servicios demostrables:

1. Crear o integrar un WS SOAP real para Autenticacion.
2. Hacer que las clases `WS_Autenticacion` se compilen y se ejecuten.
3. Corregir almacenamiento/cifrado de usuario y contrasena.
4. Ajustar crear/modificar/cambiar estado a las reglas exactas del PDF.
5. Rehacer PROVEEDOR6 para calcular todos los postpago y guardar fecha maxima.
6. Crear WS_PROVEEDOR3 SOAP conectado a PROVEEDOR6.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra las historias asignadas y evidencias visibles en este
repositorio. En el caso de Jose, se considera la aclaracion de que las pruebas
completas ya fueron realizadas fuera del proyecto y que falta incorporar capturas.

```text
Gabriel : [##--------] 20% de 100
Jose    : [########--] 75% de 100
Charlie : [###-------] 25% de 100
```

Lectura rapida:

- Gabriel: tiene avances indirectos en consulta/registro, pero faltan los WS SOAP
  y PROVEEDOR4 en el componente correcto.
- Jose: tiene las tres historias implementadas y probadas; falta subir evidencia
  formal/capturas al repositorio.
- Charlie: tiene bases de MongoDB, validadores y SP inicial, pero faltan servicios
  SOAP funcionales y facturacion completa.

## 13. Conclusion

El proyecto muestra una integracion importante del alcance anterior y un avance
solido en las historias de Jose. La arquitectura puede defender la coexistencia
de dos formatos: SOAP/XML en la frontera de Web Services y JSON como contrato
interno estable desde el alcance 1. Esa decision debe quedar explicada en la
documentacion final para evitar que se interprete como una contradiccion.

Frente al PDF de alcance 2, la entrega todavia no cumple como sistema completo.
La mayor brecha esta en Gabriel y Charlie: faltan servicios SOAP reales, ubicacion
correcta de PROVEEDOR4, WS Identificador, facturacion postpago completa y
autenticacion MongoDB expuesta como SOAP.

Si se evalua por historias, Jose tiene material defendible y pruebas realizadas;
su pendiente es documentarlas con capturas dentro del proyecto. Gabriel y Charlie
necesitan completar o al menos simular formalmente sus componentes con evidencias.
Si se evalua como grupo, el riesgo principal es presentar componentes aislados o
endpoints REST/stubs en lugar de SOAP integrado, lo que el PDF penaliza.
