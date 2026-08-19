# Informe de avance - Alcance 2 contra proyecto actual

Fecha de analisis: 2026-07-21  
Rama revisada: arbol local actual del repositorio `Central_TG`  
Documentos base: PDF `Proyecto - Alcance 2`, `docs/estrategia/estrategia_2.md`, `docs/estrategia/orden.md`

## 1. Resumen ejecutivo

El proyecto tiene avances reales en los tres bloques asignados, pero no esta listo
como entrega integral del alcance 2. La situacion cambio respecto al informe
anterior: ahora hay intentos visibles de `WS_PROVEEDOR1`, `WS_IDENTIFICADOR1`,
`PROVEEDOR6` y `WS_AUTENTICACION1/2`. Sin embargo, varios de esos avances estan
incompletos, no compilan dentro del proyecto que los contiene, no estan conectados
al componente correcto o no cumplen los textos y reglas exactas del PDF.

Aclaracion importante sobre formatos: desde el alcance 1 el equipo dejo definidos
contratos internos JSON. Esa decision se mantiene. En el alcance 2, SOAP/XML aplica
en la frontera de los Web Services, especialmente en WS Proveedor, WS Identificador
y WS Autenticacion. Por tanto, JSON interno no se considera incumplimiento si el WS
recibe/entrega SOAP/XML correctamente y traduce hacia las tramas JSON heredadas.
El riesgo actual no es usar JSON internamente, sino que algunos servicios SOAP no
compilan, no estan completos o no tienen evidencia de ejecucion.

Estado global estimado:

| Area | Estado | Comentario |
|---|---|---|
| Distribucion de responsabilidades | Cumple | `orden.md` asigna Gabriel, Jose y Charlie, cada uno con al menos tres historias. |
| Simulador C# | Cumple como base | Compila correctamente; sirve como soporte del alcance anterior y pruebas. |
| Proveedor Java | Parcial alto | Compila e incluye PROVEEDOR5 y PROVEEDOR6; falta PROVEEDOR4 real en SQL Server. |
| Identificador Python | Parcial alto | Compila e incluye IDENTIFICADOR6 y consulta de saldo; tambien contiene un handler PROVEEDOR4 en MySQL que no corresponde al bloque Proveedor. |
| WS Proveedor | Parcial | Hay WCF para WS_PROVEEDOR2 y otro proyecto para WS_PROVEEDOR1/saldo, pero hay problemas de compilacion/integracion. |
| WS Autenticacion | Parcial bajo | Existe proyecto WCF separado con MongoDB, pero faltan archivos incluidos por el `.csproj` y hay errores de modelo/codigo. |
| WS Identificador | Parcial bajo | Hay una operacion `ConsultarSaldo` dentro de `WS_Proveedor_1`, pero no un WS Identificador C# SOAP separado como pide el PDF. |
| MongoDB | Parcial | Existen scripts y clases, pero el servicio SOAP funcional no queda comprobado. |
| Documentacion/evidencias | Parcial | Jose tiene evidencias; Gabriel y Charlie siguen sin evidencias equivalentes completas. |

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
| Extraccion de PDF con `pypdf` | Correcta; se confirmaron historias y criterios de aceptacion del alcance 2. |
| `dotnet build csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj --no-restore` | Correcto, 0 errores. |
| `python -m compileall python_identificador` | Correcto. |
| `javac -d java_proveedor/build ...` | Correcto; el proveedor Java compila con PROVEEDOR5 y PROVEEDOR6. |
| `dotnet build dotnet_webservices/CentralTelefonica.WebServices/CentralTelefonica.WebServices.csproj --no-restore` | Falla. El proyecto net8 arrastra `WS_Proveedor_1` y genera errores de WCF/Newtonsoft/atributos duplicados. |
| `msbuild` | No esta en PATH; no se pudo compilar desde terminal los proyectos WCF clasicos. |

Observacion de build: el proyecto `CentralTelefonica.WebServices.csproj` excluye
`WS_Autenticacion/**/*.cs` y `WS_Proveedor/**/*.cs`, pero no excluye
`WS_Proveedor_1/**/*.cs`. Por eso el build net8 intenta compilar codigo WCF clasico
.NET Framework y falla. Esto debe corregirse o separarse antes de defender la
entrega.

## 4. Hallazgos transversales

### 4.1 JSON interno y SOAP/XML externo

El repositorio mantiene contratos internos JSON en `shared/contracts` y en los
clientes TCP entre C#, Java y Python. Esto es coherente con el alcance 1. Para el
alcance 2 debe documentarse asi:

- SOAP/XML: contrato externo de Web Services.
- JSON: contrato interno entre WS y componentes heredados.
- Cifrado: los datos sensibles deben llegar cifrados al WS o ser cifrados antes de
  enviarse al componente interno, segun el contrato acordado.

### 4.2 Hay avance nuevo, pero sin evidencia completa

Se encontraron piezas nuevas o no contempladas por el informe anterior:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1`
- `java_proveedor/src/services/Proveedor6Service.java`
- `java_proveedor/src/database/FacturacionDAO.java`
- `database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql`
- `dotnet_webservices/WS_Autenticacion`
- `shared/contracts/proveedor6_facturacion.json`

Estas piezas suben el avance de Gabriel y Charlie, pero todavia no cierran sus
historias porque faltan compilacion WCF real, ubicacion correcta, validaciones,
mensajes exactos y evidencias SOAP.

### 4.3 Build .NET principal actualmente roto

Antes el host .NET principal compilaba como minimal API. En el estado actual falla
porque `WS_Proveedor_1` quedo dentro del arbol del proyecto net8 sin exclusion. Los
errores observados incluyen:

- atributos `AssemblyCompany`, `AssemblyConfiguration`, `AssemblyVersion`, etc.
  duplicados;
- referencias WCF clasicas no disponibles para net8;
- falta de referencia a `Newtonsoft.Json`;
- tipos `ServiceContract` y `OperationContract` no resueltos.

Esto no significa que los proyectos WCF clasicos no puedan compilar en Visual
Studio, pero si significa que el build principal del repositorio no esta limpio.

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

Estado encontrado: Parcial bajo.

Evidencia:

- `python_identificador/app/services/proveedor4_handler.py`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/Service1.svc.cs`

Lo que ya esta:

- Existe un handler llamado PROVEEDOR4 en Python.
- El handler valida campos basicos, cifra con AES, revisa duplicado e inserta datos.
- `WS_Proveedor_1` construye una trama `REGISTRAR_LINEA` y la envia por TCP.

Pendientes o riesgos:

- La historia PROVEEDOR4 pertenece al Proveedor y debe persistir en SQL Server; el
  handler encontrado esta en Identificador/Python/MySQL.
- No se encontro implementacion equivalente en `java_proveedor` para registrar la
  linea disponible en SQL Server.
- El handler no valida formalmente 16 y 19 digitos.
- Inserta como activo/disponible desde una logica que no coincide claramente con
  la tabla de Proveedor SQL Server.
- Falta evidencia de registro real en SQL Server.

Nivel de cumplimiento estimado: 35%.

### 5.2 WS_PROVEEDOR1

Requerido por PDF:

- Servicio Web SOAP en C#.
- Recibe telefono, identificador telefono, identificador tarjeta, tipo y estado.
- Todos los datos se reciben encriptados.
- Prepara trama hacia PROVEEDOR4.
- Si PROVEEDOR4 responde OK: `Resultado=true`, `Mensaje=Exitoso`.
- Si no: `Resultado=false`, `Mensaje=Problemas al incluir la informacion.`

Estado encontrado: Parcial.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/IService1.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/Service1.svc.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/WS_Proveedor_1.csproj`

Lo que ya esta:

- Existe proyecto WCF clasico .NET Framework 4.7.2.
- Existe operacion `RegistrarLinea`.
- La operacion arma JSON interno `REGISTRAR_LINEA`.
- Devuelve `Resultado=true`, `Mensaje=Exitoso` cuando recibe `OK`.

Pendientes o riesgos:

- El archivo `Services/CryptoAES.cs` existe, pero no esta incluido en
  `WS_Proveedor_1.csproj`; asi el proyecto WCF no queda completo para compilar.
- `RegistrarLinea` cifra los datos recibidos, pero el PDF indica que los datos se
  reciben encriptados. Debe definirse si el WS recibe plano y cifra, o si valida
  datos ya cifrados.
- El cliente TCP apunta por defecto a `127.0.0.1:5000`, que corresponde mas al
  Identificador Python que al Proveedor Java; PROVEEDOR4 deberia estar en Proveedor.
- No hay evidencia SoapUI/WCF Test Client.

Nivel de cumplimiento estimado: 45%.

### 5.3 WS_IDENTIFICADOR1

Requerido por PDF:

- Servicio Web SOAP/XML en C# para consulta de saldo.
- Recibe XML con telefono cifrado, origen Web/telefono y tipo transaccion saldo.
- Si origen es Web, solo esos datos son obligatorios.
- Si origen es telefono, valida el flujo completo existente.
- Envia trama al Proveedor y responde XML con `Resultado: ok` y saldo.
- Debe modificar PROVEEDOR4 para soportar origen y validaciones por origen.

Estado encontrado: Parcial bajo.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/IService1.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/Service1.svc.cs`
- `python_identificador/app/services/consulta.py`
- `java_proveedor/src/services/ConsultaSaldo.java`

Lo que ya esta:

- Hay una operacion WCF `ConsultarSaldo`.
- La consulta arma JSON interno `CONSULTA_SALDO`.
- El flujo de consulta de saldo existe desde el alcance anterior en Python/Java.

Pendientes o riesgos:

- La operacion esta dentro de `WS_Proveedor_1`, no en un WS Identificador C# separado.
- No se observa contrato XML completo con origen Web/telefono y tipo de transaccion.
- La implementacion fija `origen = "WEB"` y no valida el caso origen telefono.
- No se evidencia modificacion de PROVEEDOR4 para el nuevo campo origen.
- No hay evidencia SoapUI ni respuesta XML con el formato exacto solicitado.

Nivel de cumplimiento estimado: 30%.

### 5.4 Resultado Gabriel

| Historia | Estado | Cumple PDF | Que falta |
|---|---|---|---|
| PROVEEDOR4 | Parcial bajo | No completo | Implementarlo en Proveedor/SQL Server, validar 16/19 digitos, duplicados, estado disponible, cifrado y mensajes exactos. |
| WS_PROVEEDOR1 | Parcial | Parcialmente | Incluir `CryptoAES.cs` en el proyecto, apuntar al Proveedor correcto, confirmar recepcion de datos cifrados y generar evidencia SOAP. |
| WS_IDENTIFICADOR1 | Parcial bajo | No completo | Crear/ordenar WS Identificador C# SOAP/XML real, diferenciar origen Web/telefono y documentar XML de entrada/salida. |

Riesgo principal: alto. Gabriel ya tiene codigo visible para WS, pero todavia no
tiene las tres historias defendibles contra el PDF.

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
- Hay evidencia documentada.

Pendientes o riesgos:

- Usa lectura manual de JSON por busqueda de strings; funciona para el contrato
  actual, pero es fragil ante cambios de orden/escape.
- Si IDENTIFICADOR6 actualiza MySQL y luego falla SQL Server, falta compensacion
  automatica documentada.
- Faltan capturas completas dentro del repositorio para todos los escenarios
  negativos.

Nivel de cumplimiento estimado: 80%.

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

Pendientes o riesgos:

- El PDF habla de estado `activo`; la implementacion usa `accion` ACTIVAR/DESACTIVAR
  como extension interna.
- Falta evidencia automatizada o capturas completas en base de datos.

Nivel de cumplimiento estimado: 82%.

### 6.3 WS_PROVEEDOR2

Estado encontrado: Parcial alto / sujeto a prueba WCF.

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
- Valida campos obligatorios y Base64 en campos sensibles.
- Construye trama JSON interna para PROVEEDOR5.
- Cliente TCP configurable hacia Java.
- Traduce `OK` a `Resultado=true`, `Mensaje=Exitoso`.
- Traduce errores al mensaje requerido: `Problemas al activar/desactivar la linea.`

Pendientes o riesgos:

- No se pudo compilar desde terminal porque `msbuild` clasico no esta en PATH.
- La prueba final SOAP con WCF Test Client/SoapUI no esta documentada con captura.
- El build net8 principal no valida este WCF y actualmente falla por `WS_Proveedor_1`.

Nivel de cumplimiento estimado: 75%.

### 6.4 Resultado Jose

| Historia | Estado | Cumple PDF | Que falta |
|---|---|---|---|
| PROVEEDOR5 | Parcial alto | Mayormente si | Documentar mas evidencias visuales y compensacion si SQL falla despues de MySQL. |
| IDENTIFICADOR6 | Parcial alto | Mayormente si | Completar evidencias MySQL y respuestas de error. |
| WS_PROVEEDOR2 | Parcial alto | Si, sujeto a ejecucion | Capturar prueba SOAP real y confirmar build/ejecucion en Visual Studio. |

Riesgo principal: medio-bajo. Jose tiene el bloque mas defendible; su pendiente
principal es evidencia formal y prueba SOAP final.

## 7. Charlie - PROVEEDOR6, WS_PROVEEDOR3, WS_AUTENTICACION1, WS_AUTENTICACION2

Responsabilidad: facturacion postpago y autenticacion/usuarios con MongoDB.

### 7.1 PROVEEDOR6

Requerido por PDF:

- Recibir fecha de calculo y fecha maxima de pago desde WS Proveedor.
- Validar datos completos y fechas validas.
- Ejecutar procedimiento almacenado SQL Server para todos los servicios postpago.
- Guardar fecha maxima de pago.
- Responder `OK` o `ERROR`.

Estado encontrado: Parcial medio.

Evidencia:

- `java_proveedor/src/services/Proveedor6Service.java`
- `java_proveedor/src/database/FacturacionDAO.java`
- `java_proveedor/src/sockets/ManejoCliente.java`
- `database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql`
- `shared/contracts/proveedor6_facturacion.json`

Lo que ya esta:

- Java compila con `Proveedor6Service`.
- `ManejoCliente` enruta `accion = "CALCULAR_FACTURACION"` hacia PROVEEDOR6.
- Valida que las fechas existan y tengan formato `YYYY-MM-DD`.
- Ejecuta `sp_CalcularFacturacionPostpago(?, ?)`.
- La migracion `010_proveedor6_facturacion.sql` crea tabla
  `facturacion_postpago` y un SP con `@fecha_calculo` y `@fecha_maxima_pago`.
- El SP inserta facturacion para servicios `POSTPAGO` activos y guarda fecha
  maxima de pago.

Pendientes o riesgos:

- Si faltan fechas responde `Datos Incompletos`, pero el PDF para PROVEEDOR6 solo
  define `ERROR` como respuesta no exitosa.
- El contrato `shared/contracts/proveedor6_facturacion.json` usa
  `fecha_Maxima_pago` con `M` mayuscula, pero Java lee `fecha_maxima_pago`; esa
  diferencia rompe la trama de ejemplo.
- El SP filtra llamadas `lp.fecha_llamada >= @fecha_calculo`, lo cual podria
  calcular desde la fecha de calculo hacia adelante, no un periodo cerrado. Debe
  validarse con el criterio del equipo/profesora.
- Falta evidencia en SQL Server del resultado de `facturacion_postpago`.

Nivel de cumplimiento estimado: 55%.

### 7.2 WS_PROVEEDOR3

Requerido por PDF:

- Operacion SOAP en C#.
- Recibe fecha de calculo y fecha maxima de pago.
- Prepara trama hacia PROVEEDOR6.
- Si PROVEEDOR6 responde OK: `Resultado=true`, `Mensaje=Exitoso`.
- Si no: `Resultado=false`, `Mensaje=Problemas al realizar el calculo.`

Estado encontrado: Parcial bajo.

Evidencia:

- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Models/CalcularFacturacionRequest.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Services/TramaProveedor6Service.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Validators/CalcularFacturacionValidator.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/Program.cs`

Lo que ya esta:

- Hay modelos/servicios auxiliares para construir trama PROVEEDOR6.
- El host minimal tiene endpoint REST `/proveedor/facturacion`, pero solo responde
  `"Servicio listo"` y no es SOAP.

Pendientes o riesgos:

- `WS_Proveedor.csproj` no incluye los archivos de facturacion en su lista
  `Compile`, por lo que no forman parte del WCF real.
- `IProveedorService.cs` del WCF solo expone `ActivarDesactivarLinea`.
- No existe operacion SOAP `CalcularFacturacion` demostrable.
- No hay evidencia SoapUI/WCF Test Client.

Nivel de cumplimiento estimado: 25%.

### 7.3 WS_AUTENTICACION1

Requerido por PDF:

- Servicio Web XML/SOAP para autenticar usuarios.
- Recibe usuario, contrasena y tipo.
- Valida existencia, contrasena, estado activo y tipo.
- Usuario y contrasena viajan encriptados.
- Usuario y contrasena se almacenan en MongoDB de forma encriptada.
- Respuesta exitosa: `Resultado=true`, `Mensaje=Exitoso`.
- Respuesta fallida: `Resultado=false`, `Mensaje=Usuario y/o contrasena incorrectos.`

Estado encontrado: Parcial bajo.

Evidencia:

- `dotnet_webservices/WS_Autenticacion/WS_Autenticacion.csproj`
- `dotnet_webservices/WS_Autenticacion/Contracts/IAutenticacionService.cs`
- `dotnet_webservices/WS_Autenticacion/Services/IService1.cs`
- `database/mongodb/crear_coleccion_usuarios.js`
- `database/mongodb/indices_usuarios.js`

Lo que ya esta:

- Hay proyecto WCF separado .NET Framework 4.8.
- Hay contrato SOAP con `AutenticarUsuario`.
- Hay clase de servicio que consulta MongoDB.
- Valida usuario, contrasena hasheada, tipo y estado activo.

Pendientes o riesgos:

- `WS_Autenticacion.csproj` incluye `Service1.svc` y `Service1.svc.cs`, pero esos
  archivos no existen en la carpeta. Asi el proyecto no esta completo.
- El servicio usa constructor `AutenticacionService(IMongoDatabase database)`, lo
  cual no es directamente instanciable por WCF clasico sin configuracion/factory.
- `Services/IService1.cs` usa sintaxis nullable `Usuario?`, incompatible con C# 7.3
  si se compila como proyecto .NET Framework clasico sin version moderna de C#.
- Los mensajes no son los exactos del PDF.
- Se usa SHA-256 para contrasena, no cifrado reversible/encriptacion como indica el
  PDF para usuario y contrasena.
- No se ve cifrado del campo usuario; el modelo MongoDB tiene inconsistencias de
  propiedades (`UserUsuario`, `NombreUsuario`) y el servicio tambien referencia
  `usuario.Usuario`, propiedad que no existe.

Nivel de cumplimiento estimado: 25%.

### 7.4 WS_AUTENTICACION2

Requerido por PDF:

- Metodos SOAP/XML para crear, modificar y activar/inactivar usuarios.
- Validar campos completos, correo, nombres/apellidos, tipo 1/2.
- Usuario nuevo debe crearse activo.
- Contrasena de exactamente 14 caracteres con mayuscula, minuscula, numero y
  caracter especial.
- No permitir duplicados.
- No modificar campos llave.
- Cambiar estado por identificacion y estado.
- Usuario y contrasena viajan y se almacenan encriptados.
- Mensajes exactos segun PDF.

Estado encontrado: Parcial bajo.

Evidencia:

- `dotnet_webservices/WS_Autenticacion/Contracts/IAutenticacionService.cs`
- `dotnet_webservices/WS_Autenticacion/Services/IService1.cs`
- `dotnet_webservices/WS_Autenticacion/Validators/UsuarioValidator.cs`
- `dotnet_webservices/WS_Autenticacion/Models/Usuario.cs`

Lo que ya esta:

- Existen metodos `CrearUsuario`, `ModificarUsuario` y `CambiarEstadoUsuario`.
- El validador revisa correo, nombres, tipo y contrasena de 14 caracteres.
- Hay deteccion intencionada de duplicados.

Pendientes o riesgos:

- El proyecto no esta completo por falta de `Service1.svc` y `Service1.svc.cs`.
- El servicio referencia `u.Usuario` y `usuario.Usuario`, pero el modelo define
  `UserUsuario` y `NombreUsuario`; eso impide compilacion.
- `ModificarUsuario` permite cambiar identificacion y usuario, aunque son llaves.
- `CambiarEstadoUsuario` recibe nombre de usuario, pero el PDF pide identificacion.
- Crear usuario permite estado informado; el PDF exige estado activo para nuevos.
- Usuario no se guarda cifrado y contrasena se guarda como hash.
- Los mensajes de respuesta no coinciden con los textos exactos.

Nivel de cumplimiento estimado: 25%.

### 7.5 Resultado Charlie

| Historia | Estado | Cumple PDF | Que falta |
|---|---|---|---|
| PROVEEDOR6 | Parcial medio | Parcial | Alinear contrato `fecha_maxima_pago`, ajustar respuesta de error, probar SP y guardar evidencias SQL Server. |
| WS_PROVEEDOR3 | Parcial bajo | No completo | Agregar operacion SOAP real al WCF, incluir archivos en `.csproj`, conectar con PROVEEDOR6 y evidenciar SoapUI. |
| WS_AUTENTICACION1 | Parcial bajo | No completo | Completar proyecto WCF, corregir constructor/modelos, cifrar usuario/contrasena y mensajes exactos. |
| WS_AUTENTICACION2 | Parcial bajo | No completo | Corregir compilacion, reglas de llaves/estado, cambio por identificacion, cifrado y respuestas exactas. |

Riesgo principal: alto. Charlie tiene mas avance que antes en PROVEEDOR6, pero el
bloque SOAP de autenticacion y WS_PROVEEDOR3 todavia no es defendible.

## 8. Estado de entregables del PDF

| Entregable | Estado | Evidencia / comentario |
|---|---|---|
| Documentacion analisis/diseno | Parcial | Hay estrategia, orden, roadmaps y docs finales; faltan diagramas actualizados completos y evidencias por historia. |
| MongoDB | Parcial | Hay scripts e indices; falta servicio SOAP funcional y prueba real. |
| Codigo Identificador actualizado | Parcial alto | Python compila e incluye IDENTIFICADOR6 y consulta saldo previa. |
| Codigo Proveedor actualizado | Parcial alto | Java compila con PROVEEDOR5 y PROVEEDOR6; falta PROVEEDOR4 oficial en SQL Server. |
| Codigo Simulador | Cumple | C# compila. |
| WS Proveedor | Parcial | WS_PROVEEDOR2 existe; WS_PROVEEDOR1 parcial; WS_PROVEEDOR3 no integrado al WCF. |
| WS Autenticacion | Parcial bajo | Proyecto existe, pero no esta completo ni consistente para compilar/ejecutar. |
| WS Identificador | Parcial bajo | Hay consulta en `WS_Proveedor_1`, no WS Identificador separado. |

## 9. Lo que ya esta

- El PDF fue revisado y coincide con la separacion Gabriel/Jose/Charlie de
  `orden.md`.
- El simulador C# compila.
- Python Identificador compila.
- Java Proveedor compila e incluye PROVEEDOR5 y PROVEEDOR6.
- Jose tiene contrato, codigo y evidencias para sus tres historias.
- Gabriel tiene un proyecto WCF para registrar linea y consultar saldo, aunque no
  esta cerrado.
- Charlie tiene PROVEEDOR6 en Java y SP de facturacion mas un proyecto inicial de
  WS Autenticacion.
- MongoDB tiene scripts base.
- Hay contratos JSON internos para PROVEEDOR5, IDENTIFICADOR6 y PROVEEDOR6.

## 10. Lo que falta para cierre defendible

Prioridad alta:

1. Corregir el build .NET principal excluyendo o separando `WS_Proveedor_1` del
   proyecto net8, o eliminando la mezcla net8/WCF clasico.
2. Completar PROVEEDOR4 en Proveedor/SQL Server.
3. Hacer que `WS_Proveedor_1` compile como WCF: incluir `CryptoAES.cs`, revisar
   namespaces y apuntar al Proveedor correcto.
4. Crear o reubicar WS_IDENTIFICADOR1 como C# SOAP/XML real, no solo metodo dentro
   de WS Proveedor.
5. Integrar WS_PROVEEDOR3 al WCF real y conectarlo con PROVEEDOR6.
6. Completar `WS_Autenticacion`: agregar archivos `.svc`, corregir modelo
   `Usuario`, constructor WCF, propiedades y mensajes.
7. Corregir cifrado de usuario/contrasena segun PDF.
8. Ejecutar pruebas con SoapUI/WCF Test Client y guardar evidencias por historia.

Prioridad media:

1. Alinear `shared/contracts/proveedor6_facturacion.json` con el campo real
   `fecha_maxima_pago`.
2. Documentar formalmente SOAP/XML externo y JSON interno.
3. Agregar evidencias de Gabriel y Charlie con el mismo nivel que Jose.
4. Documentar compensacion del flujo PROVEEDOR5/IDENTIFICADOR6.
5. Actualizar diagramas de base de datos, casos de uso y clases.

## 11. Recomendacion por responsable

### Gabriel

Ruta corta para defensa:

1. Implementar PROVEEDOR4 en `java_proveedor` contra SQL Server.
2. Corregir `WS_Proveedor_1` para que compile en Visual Studio e incluir
   `Services/CryptoAES.cs`.
3. Configurar el cliente TCP hacia el Proveedor Java, no hacia Identificador.
4. Separar o crear WS Identificador C# SOAP/XML para consulta de saldo.
5. Subir evidencias SoapUI y SQL Server/MySQL de alta y saldo.

### Jose

Ruta corta para defensa:

1. Mantener PROVEEDOR5, IDENTIFICADOR6 y WS_PROVEEDOR2 como bloque integrado.
2. Tomar capturas de activacion, desactivacion, linea ya activa, dueno incorrecto,
   datos incompletos y fallo de Identificador.
3. Documentar que SOAP/XML vive en WS y JSON se mantiene como contrato interno.
4. Confirmar en Visual Studio que `WS_Proveedor` ejecuta `ActivarDesactivarLinea`.

### Charlie

Ruta corta para defensa:

1. Probar `PROVEEDOR6` con el contrato correcto `fecha_maxima_pago` y guardar
   evidencia en `facturacion_postpago`.
2. Agregar operacion SOAP `CalcularFacturacion` al WCF `WS_Proveedor`.
3. Reparar `WS_Autenticacion.csproj`: agregar `.svc`, corregir propiedades
   `Usuario/UserUsuario/NombreUsuario` y constructor del servicio.
4. Ajustar WS_AUTENTICACION2 a reglas exactas: usuario nuevo activo, llaves no
   modificables, cambio de estado por identificacion.
5. Corregir cifrado y mensajes exactos.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra historias asignadas y evidencias visibles en este
repositorio:

```text
Gabriel : [####------] 37% de 100
Jose    : [########--] 79% de 100
Charlie : [###-------] 33% de 100
```

Lectura rapida:

- Gabriel: subio avance real de WS, pero PROVEEDOR4 esta en el componente
  incorrecto y WS_IDENTIFICADOR1 no esta completo como servicio C# SOAP/XML.
- Jose: mantiene el bloque mas solido; falta evidencia formal y prueba SOAP final.
- Charlie: PROVEEDOR6 mejoro, pero WS_PROVEEDOR3 y WS_AUTENTICACION1/2 siguen con
  riesgo alto por compilacion, integracion, cifrado y mensajes.

## 13. Conclusion

El proyecto avanzo frente al informe anterior: ya no se puede decir que Gabriel y
Charlie no tengan nada en sus historias. Hay codigo nuevo y parte del flujo de
facturacion ya compila en Java. Aun asi, la entrega completa del alcance 2 sigue
en riesgo porque varias historias estan en estado parcial y el build principal de
Web Services esta roto.

La arquitectura JSON interna puede defenderse, siempre que quede claramente
documentado que SOAP/XML es la frontera externa requerida por el PDF. Para cerrar
el alcance, el equipo debe concentrarse en hacer ejecutables los servicios SOAP,
ubicar PROVEEDOR4 en Proveedor/SQL Server, completar WS_IDENTIFICADOR1, integrar
WS_PROVEEDOR3 y reparar WS_AUTENTICACION con MongoDB, cifrado y mensajes exactos.

Si se evalua por historias, Jose tiene el bloque mas defendible. Gabriel y Charlie
necesitan terminar o simular formalmente sus componentes y documentar evidencias
equivalentes antes de la entrega.
