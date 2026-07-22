# Informe 2 de avance - Alcance 2 contra proyecto actual

Fecha de analisis: 2026-07-22  
Rama revisada: arbol local actual del repositorio `Central_TG` despues de actualizar `dev`  
Documentos base: PDF `Proyecto - Alcance 2`, `docs/estrategia/estrategia_2.md`, `docs/estrategia/orden.md`  
Documento historico usado como referencia de formato: `docs/auditoria/informe_avance_alcance_2_dev.md`

## 1. Resumen ejecutivo

El proyecto muestra un avance real frente al informe historico. El cambio mas
importante es que el flujo de Gabriel ya no esta solo como codigo aislado:
`RegistrarLinea` puede ejecutarse desde WCF Test Client, llega a Python, sincroniza
datos con el proveedor Java/SQL Server y permite que luego `ConsultarSaldo`
responda correctamente. Esto corrige los dos fallos iniciales principales:

- Python rechazaba `REGISTRAR_LINEA` porque no estaba registrado en el router.
- La consulta de saldo fallaba porque la linea existia en MySQL, pero no en SQL
  Server, que es donde el proveedor Java consulta saldos.

Tambien se limpio el ruido de consola del servidor Python para que las pruebas
sean mas legibles y no impriman tramas completas ni catalogos extensos.

Aunque el avance general subio, el proyecto todavia no esta completamente cerrado
contra el PDF. La razon principal es que varias historias funcionan a traves de
JSON interno y sockets, pero el PDF exige que los Web Services externos sean SOAP.
Ademas, aun hay mezcla de proyectos WCF clasicos con un host .NET moderno, y el
build principal de `CentralTelefonica.WebServices` continua fallando porque intenta
compilar codigo WCF clasico dentro de `net8.0`.

Estado global estimado:

| Area | Estado | Comentario |
|---|---|---|
| Distribucion de responsabilidades | Cumple | `orden.md` separa Gabriel, Jose y Charlie con minimo tres historias cada uno. |
| Simulador C# | Cumple | Compila correctamente y sirve como soporte para pruebas integradas. |
| Proveedor Java | Parcial alto | Compila; contiene PROVEEDOR5, PROVEEDOR6 y acciones administrativas usadas por PROVEEDOR4. |
| Identificador Python | Parcial alto | Compila; enruta IDENTIFICADOR6, REGISTRAR_LINEA y CONSULTA_SALDO. |
| WS Proveedor 1 / saldo | Parcial alto | `WS_Proveedor_1` compila y se probo con WCF Test Client para registrar linea y consultar saldo. |
| WS Proveedor 2 | Parcial alto | Codigo WCF existe para activar/desactivar; desde CLI falla por targets WebApplication no disponibles. |
| WS Proveedor 3 | Parcial bajo | PROVEEDOR6 existe en Java, pero la operacion SOAP de facturacion no esta integrada al WCF real. |
| WS Autenticacion | Parcial medio | Ya existen `.svc`, servicio, AES y MongoDB, pero el `.csproj` no incluye todos los archivos necesarios y requiere prueba en Visual Studio. |
| MongoDB | Parcial medio | Hay scripts, modelo y repositorio; falta evidencia funcional completa del WS SOAP. |
| Documentacion/evidencias | Parcial | Jose tiene evidencias; Gabriel tiene prueba manual funcional reciente; Charlie sigue requiriendo evidencias. |

## 2. Requerimientos fuente revisados

El PDF de alcance 2 exige diez historias principales:

| Integrante | Historias asignadas segun `orden.md` |
|---|---|
| Gabriel | PROVEEDOR4, WS_PROVEEDOR1, WS_IDENTIFICADOR1 |
| Jose | PROVEEDOR5, IDENTIFICADOR6, WS_PROVEEDOR2 |
| Charlie | PROVEEDOR6, WS_PROVEEDOR3, WS_AUTENTICACION1, WS_AUTENTICACION2 |

Reglas tecnicas obligatorias del PDF:

- WS Proveedor y WS Identificador deben desarrollarse en C#.
- Todos los Web Services deben usar SOAP.
- WS Autenticacion puede usar otra herramienta, pero debe ser XML/SOAP.
- Proveedor persiste en SQL Server.
- Identificador persiste en MySQL.
- Autenticacion usa MongoDB.
- Usuario, contrasena y datos telefonicos sensibles deben viajar y almacenarse cifrados.
- Si un componente no esta listo, debe existir stub o simulador para probar lo desarrollado.
- La revision aislada de componentes puede tener sancion del 20%.

## 3. Verificaciones ejecutadas

| Verificacion | Resultado |
|---|---|
| Extraccion de PDF `Proyecto - Alcance 2.pdf` con `pypdf` | Correcta; se confirmaron historias, criterios de aceptacion y entregables. |
| Lectura de `docs/estrategia/estrategia_2.md` | Correcta; confirma bloques, reglas tecnicas y roadmap. |
| Lectura de `docs/estrategia/orden.md` | Correcta; confirma responsables reales: Gabriel, Jose y Charlie. |
| `dotnet build csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj --no-restore` | Correcto, 0 errores. |
| `python -m compileall -q python_identificador` | Correcto. |
| `javac -encoding UTF-8 -d java_proveedor/build ...` | Correcto; Java Proveedor compila. |
| `dotnet build dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/WS_Proveedor_1.csproj --no-restore` | Correcto; WCF de Gabriel compila desde CLI. |
| `dotnet build dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj --no-restore` | Falla por falta de `Microsoft.WebApplication.targets` en el entorno CLI. Requiere Visual Studio/MSBuild clasico. |
| `dotnet build dotnet_webservices/WS_Autenticacion/WS_Autenticacion.csproj --no-restore` | Falla por falta de `Microsoft.WebApplication.targets` en el entorno CLI. Ademas el `.csproj` no incluye todos los archivos usados por el servicio. |
| `dotnet build dotnet_webservices/CentralTelefonica.WebServices/CentralTelefonica.WebServices.csproj --no-restore` | Falla; el host net8 arrastra `WS_Proveedor_1` y genera errores de WCF/Newtonsoft/atributos duplicados. |

Observacion de entorno: los proyectos WCF clasicos pueden requerir Visual Studio
con los targets de WebApplication. Por eso la falla de `WS_Proveedor` y
`WS_Autenticacion` desde CLI no prueba por si sola que no funcionen en Visual
Studio. Sin embargo, si es un riesgo de reproducibilidad para la entrega.

Observacion de integracion: se confirmo por prueba manual reciente que
`WS_Proveedor_1` permite registrar `70003344` y consultar saldo `1000.00` usando
WCF Test Client, Python Identificador, Java Proveedor, MySQL y SQL Server.

## 4. Hallazgos transversales

### 4.1 JSON interno y SOAP/XML externo

El repositorio conserva JSON como formato interno entre servicios WCF, Python y
Java. Esto puede defenderse si el equipo explica que:

- SOAP/XML es la frontera externa de los Web Services.
- JSON es la trama interna entre componentes heredados del alcance 1.
- Los datos sensibles se cifran antes de cruzar entre componentes.

El riesgo no es usar JSON internamente. El riesgo es que algunas historias todavia
no tienen una operacion SOAP real demostrable.

### 4.2 Avance real despues de actualizar dev

Se encontraron mejoras respecto al informe historico:

- `WS_Proveedor_1` ahora compila.
- `WS_Proveedor_1` incluye `Services/CryptoAES.cs` en el `.csproj`.
- Python acepta `REGISTRAR_LINEA`.
- `proveedor4_handler.py` evita doble cifrado.
- El registro de linea sincroniza con Java/SQL Server mediante `REGISTRAR_TELEFONO`.
- `ConsultarSaldo` desde WCF devuelve saldo real del proveedor.
- `WS_Autenticacion` ya tiene `Service1.svc`, `Service1.svc.cs`, AES y repositorio MongoDB.
- La consola Python fue limpiada para mostrar resumenes y no datos completos en cada consulta.

### 4.3 Build .NET principal actualmente roto

El proyecto `dotnet_webservices/CentralTelefonica.WebServices/CentralTelefonica.WebServices.csproj`
sigue fallando porque compila archivos de `WS_Proveedor_1`, que son WCF clasico
.NET Framework, dentro de un proyecto `net8.0`.

Errores observados:

- Atributos de ensamblado duplicados.
- `System.ServiceModel.Web` no disponible en el contexto net8.
- `Newtonsoft.Json` no referenciado por el host principal.
- `ServiceContract` y `OperationContract` no resueltos dentro del proyecto net8.

Este punto no bloquea la prueba manual de `WS_Proveedor_1`, pero si afecta la
salud general del repositorio y la reproducibilidad.

## 5. Gabriel - PROVEEDOR4, WS_PROVEEDOR1, WS_IDENTIFICADOR1

Responsabilidad: alta de lineas disponibles y consulta de saldo.

### 5.1 PROVEEDOR4

Requerido por PDF:

- Recibir trama de texto plano desde WS Proveedor.
- Registrar nueva linea disponible en Proveedor/SQL Server.
- Validar datos completos.
- Validar telefono no usado.
- Validar identificador de telefono de 16 digitos e identificador de tarjeta de 19.
- Cifrar y almacenar telefono, identificador de telefono e identificador de tarjeta.
- Responder `OK`, `Datos Incompletos`, `Telefono en uso` o `ERROR`.

Estado encontrado: Parcial alto.

Evidencia:

- `python_identificador/app/services/proveedor4_handler.py`
- `python_identificador/app/sockets/handler.py`
- `python_identificador/app/database/repositorio.py`
- `java_proveedor/src/services/AdministracionTelefonica.java`
- `java_proveedor/src/database/ServicioDAO.java`

Lo que ya esta:

- Python ya enruta `REGISTRAR_LINEA`.
- El handler valida campos obligatorios y tipo `PREPAGO`/`POSTPAGO`.
- Evita doble cifrado: si el dato ya viene cifrado, lo conserva; si viene plano, lo cifra.
- Revisa duplicados contra MySQL usando el numero descifrado cuando es posible.
- Inserta telefono, tarjeta y dispositivo en MySQL.
- Sincroniza la linea con Java/SQL Server usando la accion interna `REGISTRAR_TELEFONO`.
- Para prepago crea saldo inicial en SQL Server a traves del flujo Java.
- Se probo funcionalmente registrar una linea y luego consultar su saldo.

Pendientes o riesgos:

- La implementacion PROVEEDOR4 oficial del PDF deberia vivir en Proveedor/SQL Server; aqui el orquestador de la historia vive en Python y reutiliza una accion administrativa de Java.
- No hay validacion formal de 16 digitos para identificador de telefono ni 19 digitos para identificador de tarjeta en `proveedor4_handler.py`.
- El PDF indica `Estado: disponible`; el flujo actual acepta `activo` y lo transforma a booleano.
- Si MySQL ya tiene la linea y SQL Server tambien, responde `TEL_DUPLICADO`, pero el mensaje interno usa `Telefono en uso` sin tilde. Funcionalmente sirve, pero no coincide exactamente con todos los textos del PDF.
- Falta evidencia escrita en `docs/evidencias/gabriel`.

Nivel de cumplimiento estimado: 70%.

### 5.2 WS_PROVEEDOR1

Requerido por PDF:

- Servicio Web SOAP en C#.
- Recibe telefono, identificador telefono, identificador tarjeta, tipo y estado.
- Todos los datos se reciben encriptados.
- Prepara trama hacia PROVEEDOR4.
- Si PROVEEDOR4 responde OK: `Resultado=true`, `Mensaje=Exitoso`.
- Si no: `Resultado=false`, `Mensaje=Problemas al incluir la informacion.`

Estado encontrado: Parcial alto.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/IService1.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/Service1.svc.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/WS_Proveedor_1.csproj`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/Services/CryptoAES.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/Services/ProveedorTcpCliente.cs`

Lo que ya esta:

- Existe proyecto WCF clasico .NET Framework 4.7.2.
- Existe operacion `RegistrarLinea`.
- El `.csproj` incluye `CryptoAES.cs`, `ProveedorTcpCliente.cs` y los modelos usados.
- Compila desde CLI con `dotnet build ...WS_Proveedor_1.csproj --no-restore`.
- Arma JSON interno `REGISTRAR_LINEA`.
- Envia la trama a Python por TCP.
- Devuelve `Resultado=true` si Python responde `codigo=OK`.
- La prueba manual con WCF Test Client ya confirmo registro exitoso.

Pendientes o riesgos:

- El PDF dice que el WS recibe datos encriptados; el flujo de prueba actual recibe datos planos desde WCF Test Client y el WCF los cifra. Esto debe documentarse como decision de prueba o ajustarse al contrato exacto.
- En caso de error, ahora devuelve el mensaje real de Python. Eso ayuda a depurar, pero difiere del texto fijo del PDF para WS_PROVEEDOR1.
- El cliente TCP apunta a Python Identificador en `127.0.0.1:5000`, no directamente al proveedor Java.
- Falta evidencia formal guardada en carpeta de Gabriel.

Nivel de cumplimiento estimado: 75%.

### 5.3 WS_IDENTIFICADOR1

Requerido por PDF:

- Servicio Web SOAP/XML en C# para consulta de saldo.
- Recibe XML con telefono cifrado, origen Web/telefono y tipo transaccion saldo.
- Si origen es Web, solo esos datos son obligatorios.
- Si origen es telefono, valida el flujo completo existente.
- Envia trama al Proveedor y responde XML con `Resultado: ok` y saldo.
- Debe modificar PROVEEDOR4 para soportar origen y validaciones por origen.

Estado encontrado: Parcial medio.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/IService1.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/Service1.svc.cs`
- `python_identificador/app/services/consulta.py`
- `java_proveedor/src/services/ConsultaSaldo.java`
- `java_proveedor/src/sockets/ManejoCliente.java`

Lo que ya esta:

- Existe operacion WCF `ConsultarSaldo`.
- La operacion cifra el numero, arma `CONSULTA_SALDO` con `origen = "WEB"` y llama a Python.
- Python valida el numero contra MySQL.
- Python consulta Java/SQL Server.
- La prueba manual reciente devolvio saldo `1000.00` para `70003344`.

Pendientes o riesgos:

- La operacion esta dentro de `WS_Proveedor_1`, no en un proyecto/servicio separado llamado WS Identificador.
- El contrato externo no expone claramente `origen` ni `tipo_transaccion`; el WCF fija `origen = "WEB"`.
- No se valida desde WCF el caso origen telefono.
- La respuesta WCF usa `Mensaje = OK`, no literalmente `Resultado: ok, saldo: 000...` como ejemplo del PDF.
- Falta evidencia XML/SOAP guardada.

Nivel de cumplimiento estimado: 60%.

### 5.4 Resultado Gabriel

| Historia | Estado | Cumple PDF | Que falta |
|---|---|---|---|
| PROVEEDOR4 | Parcial alto | Parcialmente | Formalizar ubicacion en Proveedor/SQL Server, validar 16/19 digitos, alinear estado disponible y guardar evidencias. |
| WS_PROVEEDOR1 | Parcial alto | Mayormente | Decidir si recibe plano o cifrado, ajustar mensaje no exitoso al PDF y documentar prueba SOAP. |
| WS_IDENTIFICADOR1 | Parcial medio | Parcial | Separarlo o documentarlo como WS Identificador, exponer origen/tipo, validar origen telefono y evidenciar XML. |

Riesgo principal: medio. Gabriel ya puede demostrar registro y saldo integrados,
pero debe cerrar diferencias de contrato/formato contra el PDF.

## 6. Jose - PROVEEDOR5, IDENTIFICADOR6, WS_PROVEEDOR2

Responsabilidad: activacion/desactivacion e integracion Proveedor-Identificador.

### 6.1 PROVEEDOR5

Estado encontrado: Parcial alto / defendible.

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
- Llama a IDENTIFICADOR6 antes de confirmar cambio local.
- Activa en SQL Server, asocia datos cifrados y crea saldo inicial prepago de 1000.
- Desactiva dejando la linea disponible y sin dueno.
- Java compila correctamente.
- Hay evidencia documentada.

Pendientes o riesgos:

- Usa lectura manual de JSON por busqueda de strings, fragil ante cambios de formato.
- Falta documentar compensacion si IDENTIFICADOR6 actualiza MySQL pero luego falla SQL Server.
- Faltan evidencias visuales completas para todos los negativos.

Nivel de cumplimiento estimado: 82%.

### 6.2 IDENTIFICADOR6

Estado encontrado: Parcial alto / defendible.

Evidencia:

- `python_identificador/app/services/identificador6.py`
- `python_identificador/app/database/repositorio.py`
- `python_identificador/app/sockets/handler.py`
- `database/mysql_identificador/migrations/010_identificador6_cliente.sql`
- `docs/evidencias/jose/IDENTIFICADOR6/prueba_identificador6.md`

Lo que ya esta:

- Router Python acepta `IDENTIFICADOR6`.
- Valida campos obligatorios, tipo de servicio y accion.
- Descifra AES para comprobar telefono, dispositivo, tarjeta y cliente.
- Valida longitud de dispositivo 16 y tarjeta 19.
- Inserta o actualiza telefono, tarjeta y dispositivo en MySQL.
- Asocia/desasocia cliente cifrado segun accion.
- Devuelve `codigo: OK` en respuesta JSON interna.
- Python compila correctamente.

Pendientes o riesgos:

- El PDF habla de estado `activo`; la implementacion usa `accion` `ACTIVAR`/`DESACTIVAR` como extension interna.
- Faltan capturas o evidencias directas de base de datos despues de cada caso.

Nivel de cumplimiento estimado: 84%.

### 6.3 WS_PROVEEDOR2

Estado encontrado: Parcial alto / sujeto a prueba WCF en Visual Studio.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/IProveedorService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Services/TramaProveedorService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Infrastructure/ProveedorTcpClient.cs`
- `docs/evidencias/jose/WS_PROVEEDOR2/prueba_ws_proveedor2.md`

Lo que ya esta:

- Proyecto WCF clasico .NET Framework 4.7.2.
- Contrato SOAP `ActivarDesactivarLinea`.
- Valida campos obligatorios y Base64 en datos sensibles.
- Construye trama JSON interna para PROVEEDOR5.
- Cliente TCP configurable hacia Java.
- Traduce `OK` a `Resultado=true`, `Mensaje=Exitoso`.
- Traduce error a `Problemas al activar/desactivar la linea.`
- Hay evidencia escrita de prueba.

Pendientes o riesgos:

- Desde CLI falla por falta de `Microsoft.WebApplication.targets`; debe validarse desde Visual Studio.
- `IProveedorService.cs` tiene `using System.ServiceModel;` duplicado, menor pero conviene limpiar.
- Falta evidencia final con WCF Test Client o SoapUI guardada como captura/archivo.

Nivel de cumplimiento estimado: 78%.

### 6.4 Resultado Jose

| Historia | Estado | Cumple PDF | Que falta |
|---|---|---|---|
| PROVEEDOR5 | Parcial alto | Mayormente si | Evidencias negativas completas y compensacion documentada. |
| IDENTIFICADOR6 | Parcial alto | Mayormente si | Evidencia MySQL y alinear lenguaje estado/accion. |
| WS_PROVEEDOR2 | Parcial alto | Mayormente si | Prueba WCF final en Visual Studio y evidencia SOAP. |

Riesgo principal: bajo-medio. Jose sigue teniendo el bloque mas defendible.

## 7. Charlie - PROVEEDOR6, WS_PROVEEDOR3, WS_AUTENTICACION1, WS_AUTENTICACION2

Responsabilidad: facturacion postpago y autenticacion/usuarios con MongoDB.

### 7.1 PROVEEDOR6

Requerido por PDF:

- Recibir fecha de calculo y fecha maxima de pago desde WS Proveedor.
- Validar datos completos y fechas validas.
- Ejecutar procedimiento almacenado SQL Server para todos los servicios postpago.
- Guardar fecha maxima de pago.
- Responder `OK` o `ERROR`.

Estado encontrado: Parcial medio-alto.

Evidencia:

- `java_proveedor/src/services/Proveedor6Service.java`
- `java_proveedor/src/database/FacturacionDAO.java`
- `java_proveedor/src/sockets/ManejoCliente.java`
- `database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql`
- `shared/contracts/proveedor6_facturacion.json`

Lo que ya esta:

- Java compila con `Proveedor6Service`.
- `ManejoCliente` enruta `accion = "CALCULAR_FACTURACION"`.
- Valida presencia de `fecha_calculo` y `fecha_maxima_pago`.
- Valida formato `YYYY-MM-DD` usando `java.sql.Date.valueOf`.
- Ejecuta `sp_CalcularFacturacionPostpago(?, ?)`.
- La migracion crea `facturacion_postpago`.
- El procedimiento guarda `fecha_maxima_pago` y calcula total de llamadas/costo para servicios postpago activos.
- El contrato JSON ya esta alineado con `fecha_maxima_pago`.

Pendientes o riesgos:

- El procedimiento contiene `cOMMIT TRANSACTION`; SQL Server no suele distinguir mayusculas, pero conviene corregir estilo.
- En el `CATCH` del SP aparece `IF @@TRANCOUNT > 0 THROW;`, pero no se observa `ROLLBACK TRANSACTION`; si falla dentro de la transaccion puede dejar estado abierto.
- El filtro `lp.fecha_llamada >= @fecha_calculo` calcula desde la fecha hacia adelante, no un periodo cerrado; debe validarse contra el criterio de facturacion esperado.
- Falta evidencia de ejecucion real del SP y tabla `facturacion_postpago`.

Nivel de cumplimiento estimado: 65%.

### 7.2 WS_PROVEEDOR3

Requerido por PDF:

- Operacion SOAP en C#.
- Recibe fecha de calculo y fecha maxima de pago.
- Prepara trama hacia PROVEEDOR6.
- Si PROVEEDOR6 responde OK: `Resultado=true`, `Mensaje=Exitoso`.
- Si no: `Resultado=false`, `Mensaje=Problemas al realizar el calculo.`

Estado encontrado: Parcial bajo.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Contracts/TramaProveedor6.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Services/TramaProveedor6Service.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Models/CalcularFacturacionRequest.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Validators/CalcularFacturacionValidator.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Services/ProveedorService.cs`

Lo que ya esta:

- Existen modelos/servicios auxiliares para facturacion.
- Hay un servicio de estilo .NET moderno con `ObtenerFacturaPostpago`.
- Existen archivos que construyen tramas para PROVEEDOR6.

Pendientes o riesgos:

- El WCF real `WS_Proveedor.csproj` no incluye los archivos de facturacion.
- `IProveedorService.cs` del WCF clasico solo expone `ActivarDesactivarLinea`.
- No se encontro operacion SOAP `CalcularFacturacion` en el WCF clasico.
- El servicio moderno usa otro contrato (`ObtenerFacturaPostpago` con identificacion/inicio/fin) que no coincide con el PDF de WS_PROVEEDOR3.
- No hay evidencia SoapUI/WCF Test Client.

Nivel de cumplimiento estimado: 30%.

### 7.3 WS_AUTENTICACION1

Requerido por PDF:

- Servicio Web XML/SOAP para autenticar usuarios.
- Recibe usuario, contrasena y tipo.
- Valida existencia, contrasena, estado activo y tipo.
- Usuario y contrasena viajan encriptados.
- Usuario y contrasena se almacenan en MongoDB de forma encriptada.
- Respuesta exitosa: `Resultado=true`, `Mensaje=Exitoso`.
- Respuesta fallida: `Resultado=false`, `Mensaje=Usuario y/o contrasena incorrectos.`

Estado encontrado: Parcial medio.

Evidencia:

- `dotnet_webservices/WS_Autenticacion/Service1.svc`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`
- `dotnet_webservices/WS_Autenticacion/IAutenticacionService.cs`
- `dotnet_webservices/WS_Autenticacion/Data/Usuariorepository.cs`
- `dotnet_webservices/WS_Autenticacion/Security/Cryptohelper.cs`
- `dotnet_webservices/WS_Autenticacion/Models/Usuario.cs`
- `database/mongodb/crear_coleccion_usuarios.js`
- `database/mongodb/indices_usuarios.js`

Lo que ya esta:

- Ya existe `.svc` y clase `Service1`.
- Existe contrato SOAP `AutenticarUsuario`.
- Usa MongoDB mediante `UsuarioRepository`.
- Usa AES para descifrar credenciales recibidas.
- Usuario y contrasena se modelan como campos cifrados en MongoDB.
- Valida usuario existente, contrasena, estado activo y tipo.
- Mensajes principales coinciden casi totalmente con el PDF, usando `contrasena` sin `ñ` por ASCII.

Pendientes o riesgos:

- `WS_Autenticacion.csproj` no incluye `Data/Usuariorepository.cs`, `Security/Cryptohelper.cs` ni `Validators/UsuarioValidator.cs`; con proyecto clasico esos archivos pueden no compilar.
- Hay namespace inconsistente: algunos archivos usan `WS_Autenticacion.Models` y otros `CentralTelefonica.WS_Autenticacion.Models`.
- No se pudo compilar desde CLI por falta de targets WebApplication; falta prueba real en Visual Studio.
- No hay evidencia SOAP/MongoDB guardada.
- Se debe confirmar que los datos semilla de MongoDB esten cifrados con la misma clave configurada en `Web.config`.

Nivel de cumplimiento estimado: 55%.

### 7.4 WS_AUTENTICACION2

Requerido por PDF:

- Metodos SOAP/XML para crear, modificar y activar/inactivar usuarios.
- Validar campos completos, correo, nombres/apellidos, tipo 1/2.
- Usuario nuevo debe crearse activo.
- Contrasena de exactamente 14 caracteres con mayuscula, minuscula, numero y caracter especial.
- No permitir duplicados.
- No modificar campos llave.
- Cambiar estado por identificacion y estado.
- Usuario y contrasena viajan y se almacenan encriptados.
- Mensajes exactos segun PDF.

Estado encontrado: Parcial medio.

Evidencia:

- `dotnet_webservices/WS_Autenticacion/IAutenticacionService.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`
- `dotnet_webservices/WS_Autenticacion/Validators/UsuarioValidator.cs`
- `dotnet_webservices/WS_Autenticacion/Models/Usuario.cs`
- `dotnet_webservices/WS_Autenticacion/Data/Usuariorepository.cs`

Lo que ya esta:

- Existen metodos `CrearUsuario`, `ModificarUsuario` y `CambiarEstadoUsuario`.
- `CrearUsuario` exige estado `activo` para usuarios nuevos.
- Valida tipo 1/2.
- Valida correo.
- Valida nombres sin numeros.
- Valida contrasena de 14 caracteres con mayuscula, minuscula, numero y caracter especial.
- Revisa duplicados por identificacion, correo y usuario cifrado.
- `CambiarEstadoUsuario` recibe identificacion y estado, como pide el PDF.
- Guarda usuario y contrasena cifrados.

Pendientes o riesgos:

- Igual que WS_AUTENTICACION1, el `.csproj` no incluye todos los archivos usados.
- `ModificarUsuario` permite actualizar `UsuarioCifrado`; el PDF indica que los campos llave no deben modificarse. Si usuario es llave funcional, esto debe bloquearse.
- `ModificarUsuario` permite campos opcionales; el PDF indica que los datos recibidos deben estar completos para modificar.
- Falta evidencia SOAP y evidencia MongoDB antes/despues.
- Falta confirmar compilacion real con Visual Studio.

Nivel de cumplimiento estimado: 55%.

### 7.5 Resultado Charlie

| Historia | Estado | Cumple PDF | Que falta |
|---|---|---|---|
| PROVEEDOR6 | Parcial medio-alto | Parcial | Probar SP, corregir rollback, validar periodo de facturacion y evidenciar SQL Server. |
| WS_PROVEEDOR3 | Parcial bajo | No completo | Agregar operacion SOAP real al WCF clasico e incluir archivos en `.csproj`. |
| WS_AUTENTICACION1 | Parcial medio | Parcial | Incluir archivos en `.csproj`, alinear namespaces, probar SOAP/MongoDB y evidenciar. |
| WS_AUTENTICACION2 | Parcial medio | Parcial | Bloquear llaves, exigir campos completos, compilar/probar WCF y evidenciar. |

Riesgo principal: medio-alto. Charlie mejoro claramente en autenticacion y
facturacion, pero todavia debe cerrar integracion WCF y pruebas.

## 8. Estado de entregables del PDF

| Entregable | Estado | Evidencia / comentario |
|---|---|---|
| Documentacion analisis/diseno | Parcial | Hay estrategia, orden, docs finales y este informe; faltan evidencias y diagramas finales actualizados. |
| MongoDB | Parcial medio | Existen scripts, modelo y repositorio; falta prueba SOAP funcional documentada. |
| Codigo Identificador actualizado | Parcial alto | Python compila e incluye IDENTIFICADOR6, REGISTRAR_LINEA y CONSULTA_SALDO. |
| Codigo Proveedor actualizado | Parcial alto | Java compila e incluye PROVEEDOR5, PROVEEDOR6 y registro administrativo usado por alta. |
| Codigo Simulador | Cumple | Compila correctamente. |
| WS Proveedor | Parcial | WS_PROVEEDOR1 compila y funciona; WS_PROVEEDOR2 existe; WS_PROVEEDOR3 no esta integrado al WCF clasico. |
| WS Autenticacion | Parcial medio | Proyecto y codigo existen, pero requiere corregir `.csproj`/namespaces y evidenciar ejecucion. |
| WS Identificador | Parcial medio | Hay consulta de saldo funcional en `WS_Proveedor_1`, pero no como WS Identificador separado. |

## 9. Lo que ya esta

- El PDF fue revisado completo y coincide con la separacion Gabriel/Jose/Charlie.
- `orden.md` confirma la distribucion por integrante.
- El simulador C# compila.
- Python Identificador compila.
- Java Proveedor compila.
- `WS_Proveedor_1` compila.
- `RegistrarLinea` desde WCF Test Client ya registra una linea.
- `ConsultarSaldo` desde WCF Test Client ya consulta saldo real.
- PROVEEDOR4 ya no queda solo en MySQL: tambien sincroniza SQL Server por Java.
- Se corrigio el doble cifrado en Python.
- Se limpio la salida de terminal del servidor Python.
- PROVEEDOR5, IDENTIFICADOR6 y WS_PROVEEDOR2 tienen codigo y evidencias de Jose.
- PROVEEDOR6 tiene servicio Java, DAO y procedimiento almacenado.
- WS_AUTENTICACION1/2 tienen servicio WCF, contrato, modelo, AES y repositorio MongoDB.
- MongoDB tiene scripts base e indices.

## 10. Lo que falta para cierre defendible

Prioridad alta:

1. Definir formalmente si WS_PROVEEDOR1 recibe datos ya cifrados o datos planos y los cifra. El PDF dice que los recibe cifrados.
2. Agregar validaciones de longitud 16/19 en PROVEEDOR4.
3. Documentar evidencia de Gabriel: SOAP/WCF Test Client, MySQL, SQL Server y consulta de saldo.
4. Separar WS_IDENTIFICADOR1 o documentar claramente que la operacion `ConsultarSaldo` es el WS Identificador dentro del proyecto actual.
5. Integrar WS_PROVEEDOR3 al WCF clasico `WS_Proveedor`.
6. Corregir `WS_Autenticacion.csproj` para incluir `Data`, `Security` y `Validators`.
7. Alinear namespaces de `WS_Autenticacion`.
8. Probar WS_AUTENTICACION1/2 en Visual Studio con MongoDB y guardar evidencias.
9. Arreglar el build principal net8 para que no compile WCF clasico dentro del host moderno.

Prioridad media:

1. Ajustar mensajes exactos del PDF en los WS, especialmente errores fijos.
2. Documentar SOAP/XML externo y JSON interno.
3. Probar PROVEEDOR6 con datos postpago reales y evidenciar `facturacion_postpago`.
4. Corregir rollback del procedimiento almacenado PROVEEDOR6.
5. Completar capturas o archivos de prueba para Gabriel y Charlie.
6. Limpiar duplicados menores de `using` y codigo paralelo no usado.

## 11. Recomendacion por responsable

### Gabriel

Ruta corta para defensa:

1. Guardar evidencia de `RegistrarLinea` exitoso con WCF Test Client.
2. Guardar evidencia SQL Server de la linea y saldo inicial.
3. Guardar evidencia MySQL de telefono, SIM e IMEI cifrados.
4. Guardar evidencia de `ConsultarSaldo` exitoso.
5. Agregar validacion de 16/19 digitos en `proveedor4_handler.py`.
6. Ajustar o explicar contrato de cifrado: WCF recibe plano para pruebas y cifra antes del socket, o WCF recibe cifrado y no recifra.
7. Documentar que JSON es interno y SOAP es externo.

### Jose

Ruta corta para defensa:

1. Mantener PROVEEDOR5, IDENTIFICADOR6 y WS_PROVEEDOR2 como bloque integrado.
2. Probar WS_PROVEEDOR2 desde Visual Studio/WCF Test Client.
3. Agregar evidencia visual de activacion, desactivacion, linea ya activa, dueno incorrecto y datos incompletos.
4. Documentar la estrategia ante fallo parcial entre SQL Server y MySQL.
5. Confirmar que los mensajes de error visibles al WS coinciden con el PDF.

### Charlie

Ruta corta para defensa:

1. Corregir `WS_Autenticacion.csproj` para incluir todos los archivos necesarios.
2. Alinear namespaces de autenticacion.
3. Probar `AutenticarUsuario`, `CrearUsuario`, `ModificarUsuario` y `CambiarEstadoUsuario` contra MongoDB.
4. Bloquear modificacion de llaves en `ModificarUsuario`.
5. Integrar `CalcularFacturacion` como operacion SOAP real en `WS_Proveedor`.
6. Probar PROVEEDOR6 y guardar evidencia de `facturacion_postpago`.
7. Corregir rollback del procedimiento almacenado.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra historias asignadas, codigo visible, builds y pruebas
manuales reportadas en el repositorio actual:

```text
Gabriel : [#######---] 68% de 100
Jose    : [########--] 81% de 100
Charlie : [#####-----] 51% de 100
```

Lectura rapida:

- Gabriel: subio mucho. Registro de linea y consulta de saldo ya funcionan de punta a punta, aunque falta cerrar contrato exacto SOAP/cifrado y evidencias.
- Jose: sigue siendo el bloque mas solido, con Java/Python funcionales y evidencia escrita.
- Charlie: tiene mas codigo real que antes, especialmente autenticacion, pero necesita corregir integracion de proyecto WCF y demostrar ejecucion.

## 13. Conclusion

El estado actual de `dev` es mejor que el descrito en el informe historico. Ya hay
un flujo integrado demostrable para registrar un numero y consultar saldo, lo que
reduce bastante el riesgo de Gabriel. Tambien hay avance visible en autenticacion
de Charlie, con AES, MongoDB y metodos SOAP definidos.

El proyecto, sin embargo, todavia debe ordenarse para defensa. Lo mas urgente no
es escribir mucho codigo nuevo, sino cerrar contratos y evidencias: que cada WS
SOAP se pueda abrir/probar, que los mensajes coincidan con el PDF, que las bases
muestren los datos cifrados, y que los proyectos compilen desde el entorno que se
va a usar en la revision.

Si se evalua por historias, Jose mantiene el bloque mas defendible. Gabriel ya
tiene un flujo funcional importante, pero debe documentarlo y ajustar validaciones.
Charlie tiene avance medio, pero su riesgo principal sigue siendo que
WS_PROVEEDOR3 y WS_AUTENTICACION1/2 no queden demostrables como SOAP funcional
antes de la entrega.
