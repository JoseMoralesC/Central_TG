# Informe 7 de avance - Estado actual contra informe 6

Fecha de analisis: 2026-08-18  
Rama revisada: arbol local actual del repositorio `Central_TG`  
Documentos base: `docs/auditoria/informe 6_avance_alcance_final_dev.md`, codigo fuente actual, verificaciones locales de compilacion y diagnostico tecnico posterior al informe 6

## 1. Resumen ejecutivo

El proyecto avanzo de forma importante frente al informe 6. La arquitectura
principal sigue viva y ahora hay mejoras funcionales concretas en la Web
Administrativa, en el simulador telefonico, en el proveedor Java y en la
facturacion postpago.

El cambio mas relevante es que ADM3-ADM6 dejaron de estar solamente
"parcial alto pendiente de prueba" y pasaron a tener reglas de negocio mas
cerradas:

- Las pantallas administrativas ya muestran identificadores visibles cuando el
  dato puede descifrarse o ya viene en formato `ENC_SIM_` / `ENC_IMEI_`.
- El registro de nuevas lineas genera SIM/IMEI automaticamente, con formato
  correcto y validacion de no repeticion.
- La asignacion no debe volver a listar numeros ya asignados y el estado pasa a
  `ACTIVO`.
- La eliminacion de lineas disponibles elimina dependencias relacionadas.
- El simulador ya registra llamadas postpago como consumo facturable.
- La facturacion ahora es una consulta individual por linea postpago, repetible
  y por periodo.
- El encabezado fue renombrado a `Ultimo calculo` y se ordena por
  `fecha_registro`, no por `fecha_calculo`.

La deuda principal ya no es ausencia de funcionalidad, sino cierre de detalles
de consistencia y evidencia. Hay que aplicar las migraciones SQL actualizadas en
el ambiente real, reiniciar procesos para tomar DLL/clases nuevas y capturar
evidencia final por historia.

La revision contra `estrategia_3.md` mantiene el reparto:

| Companero | Nombre | Historias asignadas |
|---|---|---|
| Companero 1 | Charlie | ADM1, ADM2, ADM3, ADM4, ADM5 |
| Companero 2 | Jose | ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3 |
| Companero 3 | Gabriel | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |

Estado global estimado:

| Area | Estado | Comentario |
|---|---|---|
| Simulador C# | Cumple como base alta | Compila y ya muestra consumo postpago facturable junto al saldo. |
| Identificador Python | Parcial alto | Compila, consulta catalogo real, valida SIM/IMEI compatibles y envia registro de movimiento al proveedor. |
| Proveedor Java | Parcial alto | Compila, registra llamadas prepago/postpago, mantiene consumo postpago y ejecuta facturacion individual. |
| WS Autenticacion | Cumple funcionalmente | Login, registro, CRUD, cambios de estado y MongoDB por WS. |
| WS Proveedor WCF | Parcial alto/casi completo para ADM | Compila; cubre nuevas lineas, asignacion, devolucion, facturacion individual, listados y datos visibles. |
| WS ProveedorCliente WCF | Parcial alto | Compila; consulta lineas, recarga y pago. Mantiene riesgo en criterio de ultima factura para PortalCliente. |
| Web Administrativo principal | Parcial alto/casi defendible | ADM1-ADM7 estan en la ruta oficial; la parte visual y logica administrativa mejoro sustancialmente. |
| Web Cliente principal | Parcial alto | CLIENTE1-CLIENTE3 compilan y redirigen al PortalCliente para transacciones. |
| PortalCliente ASP.NET Core | Parcial alto | Implementa CLIENTE4-CLIENTE7; falta evidencia integral y SMTP real. |
| WebAdministrativa MVC | Parcial bajo | Sigue fallando por paquete NuGet faltante y no debe usarse en demo. |
| Documentacion y evidencias | Parcial alto | Hay documentos base; falta actualizar ruta oficial con los cambios recientes. |

Resultado general: el proyecto se encuentra en mejor posicion que en el informe
6. Charlie sube porque ADM3-ADM5 ya no solo existen, sino que fueron corregidos
en datos, duplicados, formatos y estados. Jose sube ligeramente por ADM6, ya que
la facturacion paso a ser individual, consultable sin limite y alimentada por
consumo postpago real. Gabriel se mantiene alto, pero su mayor riesgo sigue en
SMTP, evidencia de demo y consistencia de lectura de facturas desde
`WS_ProveedorCliente`.

## 2. Requerimientos fuente revisados

Se mantienen las catorce historias principales del alcance final:

| Bloque segun estrategia 3 | Historias |
|---|---|
| Companero 1 - Charlie | ADM1, ADM2, ADM3, ADM4, ADM5 |
| Companero 2 - Jose | ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3 |
| Companero 3 - Gabriel | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |

Reglas tecnicas obligatorias consideradas:

- La aplicacion Web Administrativa debe estar desarrollada en C#.
- La aplicacion Web Cliente puede estar en C# u otro lenguaje.
- Las aplicaciones Web no deben acceder directo a bases de datos ni sockets.
- Toda operacion Web debe pasar por Web Services.
- Los Web Services pueden ampliarse o crearse segun necesidad.
- Deben mantenerse actualizados Proveedor, Identificador, Simulador, WS
  Proveedor y WS Autenticacion.
- Debe existir documentacion completa de analisis, diseno, diagramas y
  evidencias.

Comparacion contra informe 6:

- No cambia el alcance.
- Si cambia el nivel de cumplimiento operativo de ADM3-ADM6.
- La facturacion ya no se interpreta como proceso global unico ni como pago,
  sino como consulta individual de consumo por linea postpago.

## 3. Verificaciones ejecutadas

| Verificacion | Resultado |
|---|---|
| Lectura completa de `docs/auditoria/informe 6_avance_alcance_final_dev.md` | Correcta; se tomo como estructura base. |
| Inventario de archivos con `rg --files` | Correcto; se confirmaron WebApps, servicios WCF, PortalCliente, Java, Python, simulador y scripts. |
| `MSBuild dotnet_webapps/CentralTelefonica.WebApps.sln` | Correcto; WebAdministrativo y WebCliente compilan con 0 errores. |
| `MSBuild dotnet_webservices/WS_Autenticacion/WS_Autenticacion.csproj` | Correcto; 0 errores. |
| `MSBuild dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj` | Correcto; 0 errores. |
| `MSBuild dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/WS_ProveedorCliente.sln` | Correcto; 0 errores. |
| `dotnet build dotnet_webservices/PortalCliente/PortalCliente.csproj --no-restore` | Correcto; 0 errores. |
| `dotnet build csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj --no-restore` | Correcto; 0 errores. |
| `javac` sobre `java_proveedor` | Correcto; 0 errores. |
| `python -m py_compile` sobre archivos Python del Identificador | Correcto despues de pasar lista real de archivos; 0 errores. |
| Build de `dotnet_webservices/WebAdministrativa/WebAdministrativa.sln` | Sigue fallando por paquete faltante `Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1`. |
| Revision de `scripts/persona2-preparar.ps1` | Correcta; compila proyectos y puede aplicar migraciones 010/013 con `-ApplySqlMigrations`. |
| Revision de `scripts/persona2-levantar.ps1` | Correcta; levanta Java, Python, simulador, WCF, WebApps y PortalCliente; MongoDB queda manual. |

Observacion: no se hizo prueba de bases vivas desde este analisis. Las
verificaciones son de codigo/compilacion y lectura estatica. Las capturas
recientes del usuario si muestran ejecucion real de WebAdministrativo,
simulador y consumo postpago.

## 4. Hallazgos transversales

### 4.1 La arquitectura base esta viva

Se mantiene la arquitectura del informe 6:

```text
WebApps / PortalCliente / Simulador C# -> WS -> Python Identificador / Java Proveedor -> Bases de datos
```

Puertos relevantes:

- Identificador Python: `127.0.0.1:5000`.
- Proveedor Java: `127.0.0.1:6000`.
- WS Autenticacion: `http://localhost:59113/Service1.svc`.
- WS Proveedor: `http://localhost:55254/ProveedorService.svc`.
- WS ProveedorCliente: `http://localhost:55260/ProveedorClienteService.svc`.
- WebAdministrativo: `http://localhost:56121/Login.aspx`.
- WebCliente: `http://localhost:56122/Login.aspx`.
- PortalCliente: `http://localhost:56123/Cliente/Index`.

Comparacion contra informe 6:

- La ruta oficial sigue siendo la misma.
- La diferencia es que el flujo administrativo tiene menos deuda funcional.
- El simulador ahora entrega mas informacion util para validar facturacion.

### 4.2 Hay aplicaciones duplicadas para cubrir el alcance

Siguen existiendo cuatro superficies Web:

- Web Administrativo principal: `dotnet_webapps/WebAdministrativo`.
- Web Cliente principal: `dotnet_webapps/WebCliente`.
- WebAdministrativa MVC: `dotnet_webservices/WebAdministrativa`.
- PortalCliente ASP.NET Core: `dotnet_webservices/PortalCliente`.

La recomendacion del informe 6 se mantiene: no usar la MVC antigua en demo.
Esa solucion sigue sin compilar por paquete NuGet faltante y no representa la
ruta funcional actual.

### 4.3 El backend ya registra consumo postpago

Este punto mejora frente al informe 6. El proveedor Java ya registra el consumo
de llamadas postpago en `llamadas_proveedor.costo` y tambien genera movimiento
`COBRO_POSTPAGO`. En el simulador administrativo se agrego la columna de
consumo postpago, lo cual permite validar visualmente que una linea postpago
tiene saldo `0,00` pero consumo acumulado facturable.

Evidencia tecnica:

- `java_proveedor/src/services/RegistrarMovimiento.java`
- `java_proveedor/src/database/LlamadaProveedorDAO.java`
- `java_proveedor/src/database/ServicioDAO.java`
- `csharp_simulador/SimuladorTelefonico/Models/TelefonoVirtual.cs`
- `csharp_simulador/SimuladorTelefonico/UI/AdministracionTelefonicaForm.cs`

### 4.4 ADM3-ADM5 ya estan mas cerca de cierre defendible

La WebAdministrativo principal ahora maneja:

- Datos visibles de SIM/IMEI sin mostrar basura cifrada.
- Alta de linea con generacion aleatoria de identificadores.
- Validacion de duplicados de numero e identificadores.
- Eliminacion de lineas disponibles con dependencias.
- Asignacion solo de lineas disponibles no activas.
- Cambio de estado a `ACTIVO` al asignar.
- Devolucion a `DISPONIBLE`.

Esto corrige varios riesgos observados despues del informe 6.

### 4.5 Facturacion cambio de global a individual

ADM6 cambio de forma significativa:

- Antes: calculo global con continuidad contra ultima fecha.
- Ahora: consulta individual por linea postpago activa.
- Antes: podia interpretarse como una factura cerrada.
- Ahora: se trata como consulta/recalculo; puede ejecutarse varias veces.
- Antes: el SP sumaba llamadas `fecha_llamada <= fecha_calculo`.
- Ahora: suma por periodo `fecha_calculo <= fecha_llamada <= fecha_maxima_pago`.

Riesgo residual:

- `ObtenerUltimaFacturacion` ya ordena por `fecha_registro`, pero agrupa por
  `fecha_calculo` y `fecha_maxima_pago`. Si varias lineas comparten el mismo
  periodo, el encabezado puede mostrar `Lineas: 2` aunque la consulta ejecutada
  haya sido individual. Es un problema de presentacion/resumen, no del calculo
  individual mostrado debajo.

### 4.6 Datos reales y arranque local

Se mantiene la recomendacion del informe 6:

- MongoDB debe estar levantado manualmente en `localhost:27017`.
- SQL Server debe tener migraciones actualizadas.
- MySQL debe estar disponible para el Identificador.
- Java y Python deben estar arriba antes de probar simulador y WebAdmin.

Nueva observacion:

- Para ADM6 es obligatorio reaplicar la migracion 010 o el script de esquema
  porque el procedimiento `sp_CalcularFacturacionPostpago` ahora recibe
  `@numero_telefono`.

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
- Consumo del WS Autenticacion.
- Uso de tipo administrador.
- Manejo de credenciales cifradas.
- Mensajes de error en credenciales invalidas.
- Redireccion al sitio administrativo.

Cambios frente al informe 6:

- No se identifican cambios funcionales fuertes en ADM1.
- La mejora visual global del sitio beneficia la pantalla.

Pendientes o riesgos:

- Falta evidencia formal de login exitoso y fallido.
- Depende de MongoDB real iniciado manualmente.

Nivel de cumplimiento estimado: 86%.

### 5.2 ADM2 - Plantilla y administracion de clientes

Estado encontrado: Parcial alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Site.Master`
- `dotnet_webapps/WebAdministrativo/Site.Master.cs`
- `dotnet_webapps/WebAdministrativo/Styles/site.css`

Lo que ya esta:

- Plantilla comun con barra superior.
- Menu administrativo completo.
- Opcion de salir del sitio.
- Footer.
- Indicador visual de opcion activa.
- Mejoras recientes de estilo, orden y espaciado.

Cambios frente al informe 6:

- La presentacion visual paso de estatica/basica a mas ordenada.
- El menu se ve mas defendible en capturas de demo.

Pendientes o riesgos:

- Falta logo/icono corporativo real; se usa marca `CT`/texto.
- Falta evidencia formal de navegacion completa.

Nivel de cumplimiento estimado: 78%.

### 5.3 ADM3 - Poner nuevas lineas a disposicion

Estado encontrado: Funcional alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasNuevas.aspx`
- `dotnet_webapps/WebAdministrativo/LineasNuevas.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/ProveedorSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/AdministracionTelefonica.java`
- `java_proveedor/src/database/ServicioDAO.java`

Lo que ya esta:

- Pantalla oficial en WebAdministrativo.
- Lista lineas disponibles desde WS Proveedor.
- Muestra identificadores visibles.
- Boton `Nuevo`.
- Formulario de nueva linea.
- Generacion automatica de `ENC_SIM_` con 19 digitos.
- Generacion automatica de `ENC_IMEI_` con 16 digitos.
- Validacion contra disponibles y activas para evitar repetir
  identificadores.
- Registro de linea disponible.
- Eliminacion de linea disponible.
- El servicio valida numero duplicado e identificadores duplicados.
- El servicio conserva formato `ENC_SIM_` / `ENC_IMEI_` sin cifrarlo otra vez.

Cambios frente al informe 6:

- El estado sube de parcial medio/alto a funcional alto.
- Se corrigio el problema de datos cifrados visibles.
- Se incorporo logica aleatoria de identificadores del simulador.
- Se corrigio la eliminacion que fallaba por dependencias.

Pendientes o riesgos:

- Falta evidencia formal de alta y eliminacion.
- Si la base tiene datos historicos con formatos antiguos, conviene limpiar o
  normalizar antes de demo.

Nivel de cumplimiento estimado: 88%.

### 5.4 ADM4 - Activar linea vendida

Estado encontrado: Funcional alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasActivar.aspx`
- `dotnet_webapps/WebAdministrativo/LineasActivar.aspx.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Validators/ActivarDesactivarLineaValidator.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- Lista lineas disponibles desde WS.
- Filtra para no mostrar numeros ya activos.
- Permite seleccionar cliente desde MongoDB via WS Autenticacion.
- Valida que la linea tenga SIM/IMEI.
- Acepta identificadores `ENC_IMEI_` y `ENC_SIM_`.
- Permite PREPAGO y POSTPAGO.
- Evita asignar el mismo numero a dos clientes activos.
- Envia estado `activo`.
- Java sincroniza con proveedor/identificador.

Cambios frente al informe 6:

- Se corrigieron fallos con formato `ENC_*`.
- Se corrigio que PREPAGO no pudiera asignarse.
- Se corrigio duplicidad de numeros asignados.
- El estado ya debe pasar a `ACTIVO`.

Pendientes o riesgos:

- Falta evidencia integral Web -> WS -> Java -> Python -> SQL/MySQL.
- El listado depende de que las lineas esten normalizadas como `DISPONIBLE`.

Nivel de cumplimiento estimado: 88%.

### 5.5 ADM5 - Devolucion/desactivacion administrativa de linea

Estado encontrado: Funcional alto.

Evidencia:

- `dotnet_webapps/WebAdministrativo/LineasDevolucion.aspx`
- `dotnet_webapps/WebAdministrativo/LineasDevolucion.aspx.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`
- `database/sqlserver_proveedor/migrations/013_normalizar_estado_linea_disponible.sql`

Lo que ya esta:

- Lista lineas activas desde WS.
- Muestra numero, identificadores visibles, identificacion visible y cliente.
- Confirma antes de desactivar.
- Envia estado `disponible`.
- Actualiza estado de inventario.

Cambios frente al informe 6:

- La pantalla ya se ve integrada y ordenada.
- Los datos cifrados fueron resueltos en la vista.
- La devolucion forma parte de la ruta oficial.

Pendientes o riesgos:

- El PDF habla de estado inactivo; el sistema usa `DISPONIBLE`.
- Falta evidencia integral con datos reales.

Nivel de cumplimiento estimado: 86%.

### 5.6 Resultado Charlie

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM1 | Funcional | 86% | Evidencia final y MongoDB real levantado. |
| ADM2 | Parcial alto | 78% | Logo/icono real y evidencia de navegacion. |
| ADM3 | Funcional alto | 88% | Evidencia de alta/eliminacion y base limpia. |
| ADM4 | Funcional alto | 88% | Evidencia integral y validacion con datos reales. |
| ADM5 | Funcional alto | 86% | Evidencia integral y explicar estado `DISPONIBLE`. |

Riesgo principal: bajo/medio. Frente al informe 6, Charlie mejora
notablemente. Las historias de lineas ya tienen logica defendible; falta
principalmente evidencia y preparacion de datos.

## 6. Jose - ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3

Responsabilidad segun estrategia 3: facturacion, mantenimiento de usuarios,
login cliente, plantilla cliente y registro cliente.

### 6.1 ADM6 - Calcular facturacion

Estado encontrado: Funcional alto con riesgo residual de resumen.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Facturacion.aspx`
- `dotnet_webapps/WebAdministrativo/Facturacion.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/ProveedorSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/Contracts/TramaProveedor6.cs`
- `java_proveedor/src/services/Proveedor6Service.java`
- `java_proveedor/src/database/FacturacionDAO.java`
- `database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql`
- `database/sqlserver_proveedor/schema/002_pa_database_facturacion_postpago.sql`

Lo que ya esta:

- Pantalla Web de consulta de facturacion.
- Selector de linea postpago activa.
- Consulta individual por numero.
- Permite consultar todas las veces que se requiera.
- No exige continuidad contra calculos anteriores.
- Valida fechas y que fecha maxima no sea menor a fecha de calculo.
- Envia `telefono_origen` en PROVEEDOR6.
- Java recibe `telefono_origen` y llama SP con tercer parametro.
- El SP filtra por numero exacto o ultimos 8 digitos.
- El SP calcula llamadas finalizadas dentro del periodo.
- El resultado muestra llamadas y total individual consultado.
- El encabezado dice `Ultimo calculo` y ordena por `fecha_registro`.

Cambios frente al informe 6:

- Se elimina la restriccion de continuidad.
- Se deja de calcular como proceso global obligatorio.
- Se alinea con la idea de consulta, no cancelacion/pago.
- Se corrige el rango de fechas para incluir llamadas entre inicio y maximo
  pago.
- Se corrige el encabezado para que tome el ultimo registro generado.

Pendientes o riesgos:

- Hay que aplicar la migracion SQL actualizada; si no, Java llamara el SP con
  tres parametros y el runtime fallara.
- `Ultimo calculo` todavia resume por periodo agrupado. Si varias lineas
  comparten `fecha_calculo` y `fecha_maxima_pago`, puede mostrar mas de una
  linea aunque el ultimo calculo haya sido individual.
- El PortalCliente/WS_ProveedorCliente todavia lee factura pendiente con orden
  por `fecha_calculo DESC, facturacion_id DESC`; con consultas arbitrarias por
  periodo, podria no coincidir siempre con el ultimo calculo por
  `fecha_registro`.

Nivel de cumplimiento estimado: 93%.

### 6.2 ADM7 - Mantenimiento usuario administrador

Estado encontrado: Cumple funcionalmente.

Evidencia:

- `dotnet_webapps/WebAdministrativo/Administradores.aspx`
- `dotnet_webapps/WebAdministrativo/Administradores.aspx.cs`
- `dotnet_webapps/WebAdministrativo/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`

Lo que ya esta:

- Lista administradores.
- Crear administrador.
- Editar datos.
- Mantener usuario sin cambios en edicion.
- Cambiar estado activo/inactivo.
- Eliminar usuario.
- Validar correo.
- Validar contrasena de 14 caracteres con mayuscula, minuscula, numero y
  especial.
- Mostrar usuario visible y contrasena enmascarada.

Cambios frente al informe 6:

- Se corrigio el riesgo visual de mostrar usuario/contrasena cifrados.
- La contrasena ya no se expone; aparece como `********`.

Pendientes o riesgos:

- Falta evidencia formal CRUD completo.
- Depende de MongoDB real.

Nivel de cumplimiento estimado: 93%.

### 6.3 CLIENTE1 - Login cliente

Estado encontrado: Cumple funcionalmente en WebCliente.

Evidencia:

- `dotnet_webapps/WebCliente/Login.aspx`
- `dotnet_webapps/WebCliente/Login.aspx.cs`
- `dotnet_webapps/WebCliente/Services/AutenticacionSoapClient.cs`
- `dotnet_webservices/WS_Autenticacion/Service1.svc.cs`

Lo que ya esta:

- Pantalla de login cliente.
- Enlace a registro.
- Uso de tipo cliente.
- Contrasena cifrada hacia WS.
- Redireccion al PortalCliente con identificacion autenticada.
- Mensaje esperado para credenciales incorrectas.

Cambios frente al informe 6:

- No se observan cambios funcionales relevantes.

Pendientes o riesgos:

- Falta evidencia formal de exito y fallo.
- Depende de MongoDB real.

Nivel de cumplimiento estimado: 90%.

### 6.4 CLIENTE2 - Plantilla portal cliente

Estado encontrado: Parcial alto.

Evidencia:

- `dotnet_webapps/WebCliente/Site.Master`
- `dotnet_webapps/WebCliente/Lineas.aspx`
- `dotnet_webapps/WebCliente/Portal.aspx`
- `dotnet_webservices/PortalCliente/Views/Shared/_Layout.cshtml`

Lo que ya esta:

- WebCliente funciona como entrada oficial.
- PortalCliente funciona como destino transaccional.
- Hay menu para lineas, cargar saldo, pagar factura y devolver linea.
- Hay footer y navegacion persistente.
- Se conserva identificacion del cliente por sesion.

Cambios frente al informe 6:

- No hay cambio funcional mayor detectado.

Pendientes o riesgos:

- Falta logo real.
- Falta evidencia de navegacion WebCliente -> PortalCliente.

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
- Crea usuario tipo cliente activo por WS.
- Valida correo y contrasena.
- Permite volver al login.

Cambios frente al informe 6:

- No se observan cambios funcionales relevantes.

Pendientes o riesgos:

- Falta evidencia final.
- Segundo apellido sigue siendo opcional en implementacion.

Nivel de cumplimiento estimado: 86%.

### 6.6 Resultado Jose

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| ADM6 | Funcional alto | 93% | Aplicar migracion SQL y pulir resumen por linea si se desea. |
| ADM7 | Funcional | 93% | Evidencia CRUD y MongoDB real. |
| CLIENTE1 | Funcional | 90% | Evidencia login exitoso/fallido. |
| CLIENTE2 | Parcial alto | 82% | Logo real y evidencia de navegacion. |
| CLIENTE3 | Funcional | 86% | Evidencia final y confirmar segundo apellido. |

Riesgo principal: bajo/medio. Jose mantiene el bloque mas defendible. La mayor
mejora frente al informe 6 es ADM6, aunque conviene alinear tambien el criterio
de factura pendiente que consume PortalCliente.

## 7. Gabriel - CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7

Responsabilidad segun estrategia 3: autogestion transaccional del cliente.

### 7.1 CLIENTE4 - Mostrar lineas asociadas al cliente

Estado encontrado: Parcial alto en PortalCliente, integrado desde WebCliente.

Evidencia:

- `dotnet_webapps/WebCliente/Login.aspx.cs`
- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/Index.cshtml`
- `dotnet_webservices/PortalCliente/Services/ProveedorClienteSoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- WebCliente redirige al PortalCliente con identificacion.
- PortalCliente consulta lineas por identificacion.
- Separa prepago y postpago.
- Muestra saldo y factura pendiente.
- Usa WS_ProveedorCliente como intermediario.

Cambios frente al informe 6:

- No hay cambio funcional fuerte detectado en CLIENTE4.
- Indirectamente mejora porque ADM4/ADM5 normalizan mejor estados y asignacion.

Pendientes o riesgos:

- La consulta depende de que `identificacion_dueno_cifrada` coincida con el
  cifrado del servicio.
- Falta evidencia con datos reales.

Nivel de cumplimiento estimado: 84%.

### 7.2 CLIENTE5 - Cargar saldo a linea prepago

Estado encontrado: Parcial alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/CargarSaldo.cshtml`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- Lista lineas prepago del cliente.
- Valida pertenencia al cliente.
- Valida datos de tarjeta.
- Valida monto positivo.
- Recarga saldo via WS_ProveedorCliente.

Cambios frente al informe 6:

- No hay cambio funcional fuerte detectado.

Pendientes o riesgos:

- El backend permite decimal positivo aunque el mensaje diga sin decimales; la
  restriccion fuerte queda en UI.
- Falta evidencia integral.

Nivel de cumplimiento estimado: 82%.

### 7.3 CLIENTE6 - Pagar factura postpago

Estado encontrado: Parcial alto, con riesgo de criterio de ultima factura.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/PagarFactura.cshtml`
- `dotnet_webservices/PortalCliente/Services/EmailService.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente/Services/LineaClienteService.cs`

Lo que ya esta:

- Lista lineas postpago.
- Permite pagar solo si hay factura pendiente.
- El monto viene del servicio y no se captura manualmente.
- Valida datos de tarjeta.
- Cancela la factura poniendo `total_facturar = 0.00`.
- Intenta enviar correo SMTP.

Cambios frente al informe 6:

- La facturacion previa que alimenta CLIENTE6 ahora puede ser individual y por
  periodo.
- Esto mejora la preparacion de una factura especifica antes del pago.

Pendientes o riesgos:

- SMTP sigue sin credenciales en `appsettings.json`.
- No hay tabla historica de pagos separada.
- `LineaClienteService` obtiene factura pendiente con orden por
  `fecha_calculo DESC, facturacion_id DESC`; despues del cambio de ADM6 podria
  convenir ordenar por `fecha_registro DESC` para mostrar la consulta mas
  reciente.

Nivel de cumplimiento estimado: 79%.

### 7.4 CLIENTE7 - Devolucion de linea por cliente

Estado encontrado: Parcial alto.

Evidencia:

- `dotnet_webservices/PortalCliente/Controllers/ClienteController.cs`
- `dotnet_webservices/PortalCliente/Views/Cliente/DevolverLinea.cshtml`
- `dotnet_webservices/PortalCliente/Services/Proveedor2SoapClient.cs`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc.cs`
- `java_proveedor/src/services/Proveedor5Service.java`

Lo que ya esta:

- Lista lineas prepago y postpago.
- Para prepago consulta saldo.
- Para postpago bloquea devolucion si hay factura pendiente.
- Valida pertenencia de la linea al cliente.
- Envia solicitud de cambio de estado al WS Proveedor.
- Devuelve la linea a `DISPONIBLE`.

Cambios frente al informe 6:

- Mejora indirecta: WS Proveedor ya valida mejor formatos `ENC_*`, duplicados y
  estados.

Pendientes o riesgos:

- Debe explicarse que `DISPONIBLE` es el equivalente operativo de linea
  devuelta/inactiva en inventario.
- Falta evidencia integral final.

Nivel de cumplimiento estimado: 84%.

### 7.5 Resultado Gabriel

| Historia | Estado | Avance estimado | Que falta |
|---|---|---:|---|
| CLIENTE4 | Parcial alto integrado | 84% | Evidencia con datos reales y cifrado compatible. |
| CLIENTE5 | Parcial alto integrado | 82% | Evidencia integral y validacion de monto entero en backend si se exige. |
| CLIENTE6 | Parcial alto integrado | 79% | SMTP real, evidencia de correo y revisar criterio de ultima factura. |
| CLIENTE7 | Parcial alto integrado | 84% | Evidencia integral y explicar `DISPONIBLE`. |

Riesgo principal: medio. Gabriel se mantiene en buena posicion, pero su cierre
depende de datos reales, SMTP y coherencia de factura pendiente despues de los
cambios de ADM6.

## 8. Estado de Web Services requeridos por estrategia 3

| Servicio / necesidad | Estado actual | Historias afectadas |
|---|---|---|
| WS_AUTENTICACION1 - login por tipo | Funcional y compila | ADM1, CLIENTE1 |
| WS_AUTENTICACION2 - CRUD/cambio estado usuarios | Funcional y compila | ADM7, CLIENTE3 |
| WS_PROVEEDOR1 - registrar nueva linea | Integrado; genera/valida formatos `ENC_*` | ADM3 |
| WS_PROVEEDOR2 - activar/desactivar linea | Funcional; valida duplicados y formatos | ADM4, ADM5, CLIENTE7 |
| WS_PROVEEDOR3 / PROVEEDOR6 - calcular facturacion | Funcional individual por linea y periodo | ADM6 |
| WS_ProveedorCliente - consultar lineas | Funcional parcial y compila | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 |
| WS_ProveedorCliente - recargar saldo | Funcional parcial y compila | CLIENTE5 |
| WS_ProveedorCliente - pagar factura | Funcional parcial y compila | CLIENTE6 |
| Enviar correo de factura | Parcial | CLIENTE6 |
| Eliminar linea disponible | Integrado con borrado de dependencias | ADM3 |
| Listados administrativos de lineas disponibles/en uso | Integrado con datos visibles | ADM3, ADM4, ADM5 |
| Registro de llamadas postpago | Integrado Java/SQL y visible en simulador | ADM6, CLIENTE6 |

Comparacion contra informe 6:

- WS Proveedor sube por facturacion individual y validaciones de lineas.
- Java proveedor sube por registro de movimiento postpago.
- WS_ProveedorCliente mantiene el mismo nivel, pero ahora deberia alinearse al
  nuevo criterio de ultimo calculo.

## 9. Lo que ya esta defendible

- Compilacion principal de WebApps y servicios WCF.
- Compilacion de PortalCliente.
- Compilacion de simulador C#.
- Compilacion de Java proveedor.
- Chequeo de sintaxis Python del Identificador.
- Login administrativo y cliente por WS Autenticacion.
- Registro cliente.
- CRUD de administradores con datos visibles y contrasena enmascarada.
- ADM3 con alta, generacion de SIM/IMEI y eliminacion.
- ADM4 con asignacion, filtro de disponibles y prevencion de duplicados.
- ADM5 con devolucion a disponible.
- ADM6 con consulta individual, repetible, por linea postpago y periodo.
- Registro de consumo postpago desde llamadas del simulador.
- Simulador mostrando saldo y consumo separado.
- PortalCliente para CLIENTE4-CLIENTE7.
- Scripts de preparacion y levantamiento.
- MongoDB fuera del script de levantamiento para trabajar con datos reales.
- WebAdministrativa MVC identificada como no oficial/no defendible.

## 10. Lo que falta para cierre defendible

Prioridad alta:

1. Aplicar migraciones SQL actualizadas, especialmente
   `010_proveedor6_facturacion.sql`.
2. Reiniciar WS Proveedor, Java proveedor, Python identificador, WebAdmin,
   WebCliente y PortalCliente despues de compilar.
3. Probar ADM3 alta de linea con formato `ENC_SIM_` / `ENC_IMEI_`.
4. Probar ADM4 asignando PREPAGO y POSTPAGO a clientes reales.
5. Confirmar que una linea asignada desaparece de disponibles y aparece en
   devolucion.
6. Probar llamada postpago desde simulador y confirmar consumo en SQL y en
   Facturacion Web.
7. Probar ADM6 con el mismo periodo de la llamada; ejemplo:
   `01/08/2026` a `31/08/2026`.
8. Probar CLIENTE6 despues de ADM6 para confirmar que PortalCliente ve la
   factura pendiente correcta.
9. Capturar evidencias finales por historia.

Prioridad media:

1. Ajustar `WS_ProveedorCliente` para ordenar factura pendiente por
   `fecha_registro DESC` si se quiere coherencia total con `Ultimo calculo`.
2. Ajustar encabezado de ADM6 para mostrar numero de linea del ultimo calculo o
   evitar el agregado por periodo si confunde.
3. Configurar SMTP real para CLIENTE6.
4. Agregar logo/icono real en plantillas.
5. Documentar explicitamente que `DISPONIBLE` equivale a linea devuelta en el
   inventario.
6. Preparar una base limpia para demo sin telefonos antiguos mal formateados.

## 11. Recomendacion por responsable

### Charlie

Ruta corta para cierre:

1. Usar solo `dotnet_webapps/WebAdministrativo` para ADM1-ADM5.
2. Demostrar ADM3 con alta y eliminacion de linea disponible.
3. Demostrar ADM4 con una linea PREPAGO y una POSTPAGO.
4. Mostrar que una linea ya asignada no vuelve a salir en disponibles.
5. Demostrar ADM5 devolviendo una linea y explicar que vuelve a
   `DISPONIBLE`.

### Jose

Ruta corta para cierre:

1. Mantener ADM6 como consulta individual, no como pago.
2. Antes de la demo, aplicar migracion 010 y reiniciar servicios.
3. Hacer una llamada postpago desde simulador, luego consultar esa linea en
   Facturacion Web.
4. Demostrar ADM7 con crear/editar/activar/inactivar/eliminar.
5. Demostrar CLIENTE1-CLIENTE3 con MongoDB real.

### Gabriel

Ruta corta para cierre:

1. Entrar siempre desde `WebCliente/Login.aspx`.
2. Mostrar que el login redirige a `PortalCliente`.
3. Demostrar CLIENTE4 con lineas reales del cliente.
4. Demostrar CLIENTE5 con recarga prepago.
5. Demostrar CLIENTE6 despues de generar factura desde ADM6.
6. Configurar SMTP o preparar evidencia controlada del mensaje de no envio.
7. Demostrar CLIENTE7 devolviendo linea sin deuda.

## 12. Nivel de cumplimiento por integrante

Estimacion de avance contra historias asignadas en `estrategia_3.md`:

```text
Charlie : [#########-] 86% de 100
Jose    : [#########-] 89% de 100
Gabriel : [########--] 82% de 100
```

Lectura rapida:

- Charlie: sube con fuerza frente al informe 6 por las correcciones de ADM3,
  ADM4 y ADM5.
- Jose: se mantiene alto; ADM6 mejora, pero el promedio se modera por
  pendientes de evidencia, MongoDB y coherencia de lectura cliente.
- Gabriel: se mantiene alto, pero no sube mucho porque el riesgo principal esta
  en SMTP/evidencia y en confirmar factura pendiente con el nuevo flujo ADM6.

## 13. Conclusion

El sistema esta en mejor estado que en el informe 6. Las piezas principales
compilan y la ruta oficial de demo sigue clara: WebAdministrativo para ADM1-ADM7,
WebCliente para login/registro cliente y PortalCliente para CLIENTE4-CLIENTE7.

El avance mas importante esta en la integracion operativa de telefonos y
facturacion. Ahora las lineas disponibles tienen identificadores correctos, las
asignaciones respetan formato y estado, el simulador registra consumo postpago y
ADM6 permite consultar una linea concreta tantas veces como se requiera.

Para pasar de "defendible alto" a "cierre integral", el equipo debe concentrarse
en tres puntos: aplicar migraciones actualizadas, ejecutar pruebas integrales con
bases reales y capturar evidencia por historia. El unico ajuste tecnico
recomendado antes de cerrar es alinear la lectura de factura pendiente en
`WS_ProveedorCliente` con el nuevo criterio por `fecha_registro`, para que
CLIENTE6 refleje siempre la consulta mas reciente generada desde ADM6.
