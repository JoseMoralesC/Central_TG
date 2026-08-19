# Informe 3 de avance - Alcance 2 contra proyecto actual

Fecha de analisis: 2026-07-22  
Rama revisada: arbol local actual del repositorio `Central_TG` despues de integrar y probar los cambios finales  
Documento base unico de requerimientos: PDF `docs/Proyecto/Proyecto - Alcance 2.pdf`  
Documento usado solo como referencia de formato: `docs/auditoria/informe 2 avance alcance 2 dev .md`

## 1. Resumen ejecutivo

El proyecto se encuentra en un estado alto de cumplimiento frente al alcance 2.
La revision se hizo usando unicamente el PDF del alcance numero 2 como fuente de
requerimientos. El informe 2 se uso solamente como plantilla de estructura para
mantener continuidad historica.

Desde la revision anterior se confirmo funcionalmente el flujo completo de los
tres bloques de trabajo:

- Gabriel: registrar linea y consultar saldo desde WCF, pasando por Python,
  Java, MySQL y SQL Server.
- Jose: activar/desactivar linea con sincronizacion hacia Identificador.
- Charlie: facturacion postpago, autenticacion, creacion, modificacion y cambio
  de estado de usuarios con MongoDB.

Tambien se confirmo que las pruebas manuales fueron ejecutadas exitosamente y
que existe evidencia documental/capturas para los casos revisados. Por tanto, el
riesgo principal ya no esta en la funcionalidad base, sino en detalles de cierre:
consistencia de nombres de estado, mensajes exactos frente al PDF, aislamiento de
proyectos WCF clasicos y cuidado de configuraciones sensibles.

Estado global estimado:

| Area | Estado | Comentario |
|---|---|---|
| Gabriel | Cumple alto | Registro de linea y consulta de saldo funcionan de extremo a extremo. |
| Jose | Cumple alto | Activacion/desactivacion queda funcional y sincronizada con Identificador. |
| Charlie | Cumple alto | Facturacion y WS de autenticacion funcionan con SQL Server y MongoDB. |
| SOAP/WCF | Cumple alto | Los servicios principales existen como WCF/SOAP y compilan por proyecto. |
| Bases de datos | Cumple alto | SQL Server, MySQL y MongoDB estan integradas en los flujos requeridos. |
| Documentacion/evidencias | Cumple | El usuario confirma documentacion y capturas completas para los casos probados. |
| Salud de solucion .NET global | Riesgo tecnico | Los proyectos WCF clasicos deben ejecutarse/compilarse como proyectos separados; el host net8 mezclado sigue siendo un riesgo de organizacion. |

## 2. Requerimientos fuente revisados

El PDF del alcance 2 define los siguientes bloques obligatorios:

| Integrante | Historias revisadas |
|---|---|
| Gabriel | PROVEEDOR4, WS_PROVEEDOR1, WS_IDENTIFICADOR1 |
| Jose | PROVEEDOR5, IDENTIFICADOR6, WS_PROVEEDOR2 |
| Charlie | PROVEEDOR6, WS_PROVEEDOR3, WS_AUTENTICACION1, WS_AUTENTICACION2 |

Reglas tecnicas obligatorias del PDF:

- Completar funcionalidad de Proveedor.
- Completar funcionalidad de Identificador.
- Usar MongoDB para validacion/autenticacion.
- Crear WS del Proveedor.
- Crear WS de logueo/autenticacion.
- WS Proveedor y WS Identificador deben estar desarrollados en C#.
- Todos los Web Services deben usar SOAP/XML.
- Proveedor persiste en SQL Server.
- Identificador persiste en MySQL.
- Autenticacion persiste usuarios en MongoDB.
- Datos sensibles de telefonos, identificadores, usuario y contrasena deben viajar
  y almacenarse cifrados.
- Si un componente no esta listo, debe existir simulador o stub para probar el
  componente desarrollado.

## 3. Verificaciones ejecutadas

| Verificacion | Resultado |
|---|---|
| Extraccion y lectura del PDF `Proyecto - Alcance 2.pdf` | Correcta; se validaron historias, criterios y entregables. |
| `python -m compileall -q python_identificador` | Correcto. |
| Compilacion Java Proveedor con `javac -encoding UTF-8` | Correcta. |
| `dotnet build` de `WS_Proveedor_1.csproj` | Correcto, 0 errores. |
| MSBuild de `WS_Proveedor.csproj` | Correcto, 0 errores. |
| MSBuild de `WS_Autenticacion.csproj` | Correcto, 0 errores. |
| `dotnet build` del simulador C# | Correcto, 0 errores. |
| Revision de evidencias locales por companero | Existen carpetas y capturas para Charlie, Gabriel y Jose. |
| Pruebas manuales reportadas por el usuario | Confirmadas como exitosas para los flujos trabajados. |

Observacion: el proyecto principal `CentralTelefonica.WebServices.csproj` conserva
un riesgo de organizacion porque mezcla un host .NET moderno con proyectos WCF
clasicos. Esto no invalida las historias del PDF si los WCF se entregan y prueban
como proyectos separados, pero si conviene explicarlo si alguien intenta compilar
toda la carpeta como una unica solucion moderna.

## 4. Hallazgos transversales

### 4.1 SOAP externo y JSON interno

Los Web Services expuestos al cliente son SOAP/WCF. Internamente el sistema sigue
usando tramas JSON por sockets entre WCF, Python Identificador y Java Proveedor.
Esto es defendible porque el PDF exige SOAP para la frontera de Web Services, no
necesariamente para la mensajeria interna entre componentes.

### 4.2 Cifrado de datos sensibles

Los datos telefonicos sensibles y las credenciales se manejan cifrados en los
flujos revisados. En las pruebas WCF, algunos metodos permiten ingresar valores
planos y el servicio los cifra antes de enviarlos a los componentes internos. Esto
facilita las pruebas, pero debe explicarse como una decision de implementacion:
el dato sensible cruza hacia los componentes internos cifrado.

### 4.3 Estados de linea

El PDF usa principalmente `disponible` para lineas no activas y `activo` para
lineas activadas. El proyecto fue normalizado para trabajar de forma mas clara
con `activo` e `inactivo`, manteniendo compatibilidad con `disponible` donde
corresponde. Esta correccion mejora la comprension y reduce errores durante las
pruebas.

### 4.4 Evidencia funcional

El usuario confirmo que todas las pruebas fueron realizadas exitosamente y que la
documentacion/capturas se generaron para los tres companeros. La revision local
tambien encontro archivos de evidencia en `docs/evidencias`.

## 5. Gabriel - PROVEEDOR4, WS_PROVEEDOR1, WS_IDENTIFICADOR1

Responsabilidad: registro de lineas disponibles y consulta de saldo.

### 5.1 PROVEEDOR4

Requerido por PDF:

- Recibir trama desde WS Proveedor.
- Registrar una nueva linea disponible.
- Validar datos completos.
- Validar telefono no usado.
- Validar identificador de telefono de 16 digitos y tarjeta de 19 digitos.
- Cifrar y almacenar telefono e identificadores.
- Responder `OK`, `Datos Incompletos`, `Telefono en uso` o `ERROR`.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Python enruta `REGISTRAR_LINEA`.
- El handler valida campos obligatorios, tipo de servicio y datos cifrados.
- Evita doble cifrado.
- Valida duplicados antes de registrar.
- Registra en MySQL y sincroniza con Java/SQL Server.
- El flujo fue probado desde WCF Test Client y luego la linea pudo consultar saldo.

Observaciones:

- El flujo real usa Python como orquestador y Java/SQL Server como persistencia
  del proveedor. Funcionalmente cumple, aunque la arquitectura interna no es una
  traduccion literal de la historia.
- El estado fue normalizado hacia `inactivo`, con compatibilidad para
  `disponible`.

Nivel de cumplimiento estimado: 90%.

### 5.2 WS_PROVEEDOR1

Requerido por PDF:

- Servicio SOAP en C#.
- Recibe telefono, identificador telefono, identificador tarjeta, tipo y estado.
- Prepara trama hacia PROVEEDOR4.
- Devuelve `Resultado=true`, `Mensaje=Exitoso` ante OK.
- Devuelve `Resultado=false`, `Mensaje=Problemas al incluir la informacion` ante
  error.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Existe proyecto WCF `WS_Proveedor_1`.
- Expone `RegistrarLinea`.
- Compila correctamente.
- Cifra datos sensibles antes de enviarlos a Python.
- Retorna respuesta compatible al cliente WCF.
- Fue probado exitosamente desde WCF Test Client.

Observaciones:

- El flujo de prueba permite ingresar datos planos para que el WCF los cifre. Si
  se evalua estrictamente la frase "recibe datos encriptados", conviene aclarar
  esta decision durante la defensa.

Nivel de cumplimiento estimado: 88%.

### 5.3 WS_IDENTIFICADOR1

Requerido por PDF:

- Servicio SOAP en C# para consulta de saldo.
- Recibe telefono y origen.
- Si el origen es Web, solo valida los datos minimos.
- Si el origen es telefono, valida telefono, SIM, IMEI, pais, provincia, latitud,
  longitud y tipo de transaccion.
- Solicita saldo al proveedor.
- Responde resultado y saldo.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Existe operacion `ConsultarSaldo` en el WCF revisado.
- Soporta origen `WEB` y `TELEFONO`.
- Valida datos completos para consulta desde telefono.
- Valida ubicacion nacional.
- Consulta saldo real al proveedor Java/SQL Server.
- La prueba de consulta de saldo fue confirmada como exitosa.

Observaciones:

- La operacion esta integrada dentro del proyecto WCF usado para Gabriel, no como
  un proyecto independiente llamado literalmente `WS_IDENTIFICADOR`. Funcionalmente
  cubre la historia, pero el nombre/separacion puede requerir explicacion.

Nivel de cumplimiento estimado: 87%.

### 5.4 Resultado Gabriel

Gabriel queda en estado defendible. Las historias principales funcionan y fueron
probadas. Los pendientes son de presentacion tecnica: explicar la ubicacion real
de la consulta de saldo y la decision de cifrar dentro del WCF durante pruebas.

Cumplimiento Gabriel: 88% a 90%.

## 6. Jose - PROVEEDOR5, IDENTIFICADOR6, WS_PROVEEDOR2

Responsabilidad: activacion/desactivacion de lineas y sincronizacion con
Identificador.

### 6.1 PROVEEDOR5

Requerido por PDF:

- Recibir trama para activar/desactivar una linea vendida.
- Validar datos completos.
- Activar solo si la linea esta disponible.
- Desactivar solo si la linea esta activa y pertenece al cliente.
- En activacion prepago, asignar saldo inicial de 1000.
- En desactivacion, desasociar cliente e inactivar servicio.
- Enviar datos a IDENTIFICADOR6.
- Responder `OK`, `Datos Incompletos`, `Telefono en uso`,
  `Telefono no corresponde`, `Activacion fallida` o `ERROR`.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Java Proveedor implementa `Proveedor5Service`.
- Valida campos, tipo de servicio y accion.
- Verifica disponibilidad/actividad de la linea.
- Valida pertenencia del cliente en desactivacion.
- Llama a IDENTIFICADOR6 antes de confirmar cambios.
- Actualiza SQL Server.
- Maneja saldo inicial prepago de 1000.
- Se corrigio la comprension de estados para usar `activo` e `inactivo`.
- Activacion e inactivacion fueron probadas exitosamente.

Observaciones:

- El PDF usa el termino `disponible`; el proyecto ya admite el flujo mas claro
  `activo`/`inactivo`. Esto no rompe el objetivo funcional.

Nivel de cumplimiento estimado: 92%.

### 6.2 IDENTIFICADOR6

Requerido por PDF:

- Recibir datos desde Proveedor para activar linea.
- Validar datos completos.
- Incluir la informacion en telefonos disponibles/asociados del Identificador.
- Responder `OK` o `Activacion fallida`.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Python Identificador enruta `IDENTIFICADOR6`.
- Valida campos requeridos, tipo de servicio y accion.
- Valida datos sensibles cifrados y longitudes esperadas.
- Sincroniza datos en MySQL.
- Responde con estructura de exito o fallo.
- La sincronizacion quedo probada dentro del flujo de activacion/desactivacion.

Observaciones:

- La historia del PDF se enfoca en activacion; el proyecto tambien soporta
  desactivacion para mantener consistencia con PROVEEDOR5.

Nivel de cumplimiento estimado: 93%.

### 6.3 WS_PROVEEDOR2

Requerido por PDF:

- Operacion SOAP en C# para activar/desactivar linea.
- Recibe telefono, identificadores, tipo, identificacion de cliente y estado.
- Datos sensibles recibidos/enviados cifrados.
- Prepara trama hacia PROVEEDOR5.
- Devuelve `Exitoso` ante OK.
- Devuelve `Problemas al activar/desactivar la linea` ante error.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Existe proyecto WCF `WS_Proveedor`.
- Expone `ActivarDesactivarLinea`.
- Compila correctamente con MSBuild de Visual Studio.
- Valida datos requeridos.
- Mapea `activo` a activacion e `inactivo`/`disponible` a desactivacion.
- Envia trama a Java Proveedor.
- Fue probado exitosamente desde WCF Test Client.

Observaciones:

- En algunos errores devuelve el detalle real del proveedor. Esto ayuda a pruebas,
  pero puede diferir del mensaje generico exacto solicitado por el PDF.

Nivel de cumplimiento estimado: 90%.

### 6.4 Resultado Jose

Jose queda en estado defendible y funcional. La parte mas importante, la
activacion/desactivacion con sincronizacion hacia Identificador, esta resuelta.
Los riesgos restantes son de nomenclatura y mensajes, no de funcionamiento base.

Cumplimiento Jose: 91% a 93%.

## 7. Charlie - PROVEEDOR6, WS_PROVEEDOR3, WS_AUTENTICACION1, WS_AUTENTICACION2

Responsabilidad: facturacion postpago y autenticacion/gestion de usuarios.

### 7.1 PROVEEDOR6

Requerido por PDF:

- Recibir fecha de calculo y fecha maxima de pago.
- Validar datos completos y fechas validas.
- Ejecutar procedimiento almacenado en SQL Server.
- Calcular saldo de llamadas para servicios postpago.
- Guardar la fecha maxima de pago.
- Responder `OK` o `ERROR`.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Java Proveedor implementa `Proveedor6Service`.
- Valida fechas requeridas y formato `yyyy-MM-dd`.
- Rechaza fecha maxima de pago anterior a fecha de calculo.
- Ejecuta `sp_CalcularFacturacionPostpago`.
- La migracion SQL crea tabla, indice, tipo de transaccion y procedimiento.
- El procedimiento calcula facturacion de servicios postpago activos.
- La prueba SQL y la prueba desde WCF fueron confirmadas como exitosas.

Observaciones:

- El PDF menciona una respuesta OK del Identificador en esta historia, pero por
  contexto parece un error de redaccion del documento. La implementacion correcta
  resuelve la facturacion desde Proveedor/SQL Server.

Nivel de cumplimiento estimado: 94%.

### 7.2 WS_PROVEEDOR3

Requerido por PDF:

- Operacion SOAP en C# para solicitar calculo de facturacion.
- Recibe fecha de calculo y fecha maxima de pago.
- Prepara trama hacia PROVEEDOR6.
- Devuelve `Exitoso` ante OK.
- Devuelve `Problemas al realizar el calculo` ante error.

Estado encontrado: Cumple alto.

Lo que ya esta:

- `WS_Proveedor` expone `CalcularFacturacion`.
- Valida fechas antes de enviar la trama.
- Envia solicitud a Java Proveedor.
- Compila correctamente con MSBuild.
- Fue probado exitosamente desde WCF Test Client.
- La tabla `facturacion_postpago` queda poblada en SQL Server.

Observaciones:

- Debe ejecutarse como proyecto WCF clasico desde Visual Studio/MSBuild, no desde
  el host .NET moderno mezclado.

Nivel de cumplimiento estimado: 93%.

### 7.3 WS_AUTENTICACION1

Requerido por PDF:

- Servicio XML/SOAP para autenticacion.
- Recibe usuario, contrasena y tipo.
- Usuario y contrasena viajan cifrados.
- Valida existencia, contrasena, estado activo y acceso por tipo.
- Usa MongoDB como base de datos.
- Responde `Exitoso` o `Usuario y/o contrasena incorrectos`.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Existe proyecto WCF `WS_Autenticacion`.
- Expone `AutenticarUsuario`.
- Usa MongoDB mediante repositorio.
- Usuario y contrasena se comparan cifrados.
- Valida tipo y estado activo.
- Compila correctamente con MSBuild.
- Fue probado exitosamente con usuario activo e inactivo.

Observaciones:

- MongoDB debe estar iniciado manualmente antes de las pruebas si el servicio de
  Windows se mantiene en modo manual.

Nivel de cumplimiento estimado: 94%.

### 7.4 WS_AUTENTICACION2

Requerido por PDF:

- Crear usuarios.
- Modificar usuarios.
- Activar/inactivar usuarios.
- Validar identificacion, nombres, apellidos, correo, usuario, contrasena,
  estado y tipo.
- Contrasena exacta de 14 caracteres con mayuscula, minuscula, numero y caracter
  especial.
- Usuario y contrasena cifrados en transito y almacenamiento.
- Persistencia en MongoDB.

Estado encontrado: Cumple alto.

Lo que ya esta:

- Existen operaciones `CrearUsuario`, `ModificarUsuario` y `CambiarEstadoUsuario`.
- Se validan datos obligatorios y formato de correo.
- Se valida contrasena con las reglas del PDF.
- Se valida tipo 1/2 y estado activo/inactivo.
- Se evita duplicidad por identificacion, usuario y correo mediante codigo e
  indices MongoDB.
- Se impide cambiar identificacion y usuario cifrado durante modificacion.
- Los scripts MongoDB crean coleccion e indices unicos.
- Las pruebas de crear, autenticar, modificar, inactivar y reactivar fueron
  confirmadas como exitosas.

Observaciones:

- La configuracion local de MongoDB y cualquier secreto real debe mantenerse fuera
  de commits publicos o entregables compartidos.

Nivel de cumplimiento estimado: 94%.

### 7.5 Resultado Charlie

Charlie queda en estado defendible. La facturacion postpago y el modulo de
autenticacion cumplen el comportamiento exigido por el PDF y fueron probados en
el entorno local.

Cumplimiento Charlie: 93% a 94%.

## 8. Estado de entregables del PDF

| Entregable PDF | Estado actual | Comentario |
|---|---|---|
| Documentacion de analisis/diseno | Cumple | Usuario confirma documentacion general y detallada con capturas. |
| MongoDB | Cumple | Coleccion, indices y WS Autenticacion funcionando. |
| Fuente Identificador actualizado | Cumple alto | Python enruta REGISTRAR_LINEA, IDENTIFICADOR6 y CONSULTA_SALDO. |
| Fuente Proveedor actualizado | Cumple alto | Java implementa PROVEEDOR5, PROVEEDOR6 y soporte de registro/saldo. |
| Fuente Simulador de llamadas | Cumple | Compila y apoya pruebas de administracion telefonica. |
| WS Proveedor | Cumple alto | Operaciones WCF para registro, activacion/desactivacion y facturacion. |
| WS Autenticacion | Cumple alto | Autenticacion y administracion de usuarios en MongoDB. |
| WS Identificador | Cumple funcional | Consulta de saldo implementada y probada; conviene explicar su ubicacion real en el proyecto WCF. |

## 9. Lo que ya esta

- Registro de linea disponible desde WCF.
- Consulta de saldo desde WCF.
- Validacion de consulta por origen Web/Telefono.
- Activacion de linea.
- Desactivacion de linea.
- Sincronizacion con Identificador.
- Normalizacion de estado `activo`/`inactivo`.
- Facturacion postpago con procedimiento almacenado.
- Creacion de usuarios en MongoDB.
- Autenticacion de usuarios activos.
- Bloqueo funcional de usuarios inactivos.
- Modificacion de usuarios.
- Cambio de estado de usuarios.
- Compilacion individual exitosa de Python, Java, WCF Proveedor, WCF Autenticacion
  y simulador.
- Evidencias documentales/capturas generadas por el usuario.

## 10. Lo que falta para cierre defendible

No se identifica una funcionalidad critica pendiente frente al PDF, tomando en
cuenta que las pruebas fueron confirmadas como exitosas. Lo pendiente es de
preparacion de entrega:

- Ejecutar en cada ambiente las migraciones SQL necesarias, incluyendo la
  normalizacion de estado de linea.
- Iniciar MongoDB manualmente antes de probar autenticacion.
- Confirmar que las cadenas de conexion y credenciales reales no se suban.
- Explicar que algunos WS reciben datos planos en pruebas y cifran antes de
  enviarlos internamente.
- Explicar que `disponible` fue normalizado a `inactivo` para evitar confusion.
- Entregar/probar los WCF como proyectos clasicos separados, no como un unico host
  moderno `.NET`.
- Mantener a mano evidencia de WCF Test Client, consola Python/Java, SQL Server,
  MySQL y MongoDB.

## 11. Recomendacion por responsable

### Gabriel

Presentar primero el flujo feliz:

1. Registrar linea desde WCF.
2. Ver que llega a Python.
3. Ver que queda en MySQL y SQL Server.
4. Consultar saldo por Web.
5. Consultar saldo por Telefono con ubicacion valida.

Punto a explicar: el WCF cifra los datos sensibles antes de enviarlos al resto
del sistema.

### Jose

Presentar primero:

1. Linea inactiva/disponible.
2. Activacion con cliente.
3. Sincronizacion IDENTIFICADOR6.
4. Cambio en SQL Server/MySQL.
5. Desactivacion y validacion de propietario.

Punto a explicar: `inactivo` representa el estado no activo que el PDF llama
`disponible`.

### Charlie

Presentar primero:

1. MongoDB iniciado.
2. Crear usuario.
3. Autenticar usuario activo.
4. Modificar usuario.
5. Inactivar usuario y validar que no autentica.
6. Reactivar usuario.
7. Ejecutar facturacion postpago y revisar tabla SQL.

Punto a explicar: los proyectos WCF clasicos deben abrirse/ejecutarse desde
Visual Studio o MSBuild de Visual Studio.

## 12. Nivel de cumplimiento por integrante

| Integrante | Historias | Cumplimiento estimado | Estado |
|---|---|---:|---|
| Gabriel | PROVEEDOR4, WS_PROVEEDOR1, WS_IDENTIFICADOR1 | 88% - 90% | Cumple alto |
| Jose | PROVEEDOR5, IDENTIFICADOR6, WS_PROVEEDOR2 | 91% - 93% | Cumple alto |
| Charlie | PROVEEDOR6, WS_PROVEEDOR3, WS_AUTENTICACION1, WS_AUTENTICACION2 | 93% - 94% | Cumple alto |

Cumplimiento global estimado: 91% - 93%.

## 13. Conclusion

Con base exclusiva en el PDF del alcance 2, el proyecto esta en estado alto y
defendible. Las historias funcionales principales ya estan implementadas,
compilan por componente y fueron probadas exitosamente por el usuario.

El proyecto no presenta pendientes funcionales criticos para el alcance 2. Los
temas restantes son de orden de entrega, explicacion tecnica y limpieza de
configuracion: mantener secretos fuera del repositorio, ejecutar migraciones,
levantar MongoDB manualmente antes de probar, y aclarar la separacion entre SOAP
externo y JSON interno.

