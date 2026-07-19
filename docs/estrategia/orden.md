# Roadmap completo — Segundo alcance

## Sistema de Control de Llamadas Telefónicas

Este roadmap mantiene la distribución planteada anteriormente, sustituyendo los nombres genéricos por los integrantes reales:

| **Integrante** | **Rol en la distribución** | **Historias asignadas**                                         |
|----------------|----------------------------|-----------------------------------------------------------------|
| **Gabriel**    | Compañero 1                | PROVEEDOR4, WS_PROVEEDOR1, WS_IDENTIFICADOR1                    |
| **José**       | Compañero 2                | PROVEEDOR5, IDENTIFICADOR6, WS_PROVEEDOR2                       |
| **Charlie**    | Compañero 3                | PROVEEDOR6, WS_PROVEEDOR3, WS_AUTENTICACION1, WS_AUTENTICACION2 |

La distribución cumple con la condición de que cada historia tenga un único responsable y que cada estudiante desarrolle al menos tres historias.

## 1. Organización general del equipo

Aunque cada integrante desarrollará historias diferentes, el proyecto no puede funcionar como tres trabajos separados.

La profesora exige que los componentes se prueben de manera integrada. Si se usan varias computadoras, todas deben tener acceso a los proyectos y bases de datos necesarios. La evaluación aislada de un componente puede recibir una reducción del 20 %.

Por tanto, el equipo trabajará bajo esta estructura:

Gabriel

└── Alta de líneas y consulta de saldo

José

└── Activación/desactivación e integración Proveedor–Identificador

Charlie

└── Facturación y autenticación con MongoDB

Los tres flujos se conectan:

WS Proveedor

↓

Proveedor telefónico

↓

Identificador

↓

MySQL

Y el bloque de seguridad funciona así:

WS Autenticación

↓

MongoDB

## 2. Infraestructura central del proyecto

### 2.1 Máquina principal de José

La computadora de José funcionará inicialmente como servidor central de desarrollo.

En esta computadora estarán:

- SQL Server.
- SQL Server Management Studio.
- MySQL Server.
- Herramienta de administración de MySQL.
- MongoDB Community Server.
- MongoDB Compass.
- Tailscale.
- Repositorio principal del proyecto.
- Bases de datos compartidas.
- Servicios que deban probarse de manera centralizada.

### 2.2 Acceso remoto

Gabriel y Charlie se conectarán desde sus casas mediante Tailscale.

Cada servicio deberá ser accesible mediante la dirección IP de Tailscale de José:

| **Componente**   | **Puerto habitual** |
|------------------|---------------------|
| SQL Server       | 1433                |
| MySQL            | 3306                |
| MongoDB          | 27017               |
| WS Proveedor     | Por definir         |
| WS Autenticación | Por definir         |
| WS Identificador | Por definir         |

Los puertos definitivos de los Web Services deberán acordarse antes de programar.

Ejemplo:

- **WS Proveedor:** `http://IP-TAILSCALE-JOSE:5001`
- **WS Autenticación:** `http://IP-TAILSCALE-JOSE:5002`
- **WS Identificador:** `http://IP-TAILSCALE-JOSE:5003`

Esas direcciones son ilustrativas. El equipo debe definir y documentar las reales.

## 3. Instalaciones requeridas por integrante

### 3.1 Gabriel

Gabriel trabajará principalmente con:

- Proveedor telefónico.
- WS Proveedor.
- WS Identificador.
- SQL Server.
- Consulta de saldo.
- Tramas SOAP/XML.

#### Gabriel debe instalar

- Visual Studio Community.
- Carga de trabajo de desarrollo web y ASP.NET.
- SDK de .NET compatible con el proyecto.
- Git.
- VS Code.
- Tailscale.
- SoapUI o herramienta equivalente.
- SQL Server Management Studio, aunque SQL Server esté en la computadora de José.
- Cliente de MySQL, si necesita revisar resultados del Identificador.
- Extensión o herramienta para visualizar XML.
- Dependencias de C# utilizadas por el proyecto.

Gabriel no necesita instalar obligatoriamente los servidores completos de SQL Server o MySQL si trabajará contra las bases alojadas en la computadora de José. Sin embargo, sí debe contar con clientes que le permitan consultar y comprobar los datos.

### 3.2 José

José tendrá dos responsabilidades: desarrollar sus historias y mantener la infraestructura central.

#### José debe tener instalado

- Visual Studio Community.
- Carga de trabajo de ASP.NET y desarrollo web.
- SDK de .NET correspondiente.
- Git.
- VS Code.
- Tailscale.
- SoapUI.
- SQL Server.
- SQL Server Management Studio.
- MySQL Server.
- Herramienta administrativa de MySQL.
- MongoDB Community Server.
- MongoDB Compass.
- Herramientas de firewall y red de Windows.
- Controladores de conexión necesarios:
  - SQL Server.
  - MySQL.
  - MongoDB, cuando corresponda.

José también debe mantener disponible el Simulador de Llamadas para las pruebas integradas.

### 3.3 Charlie

Charlie trabajará principalmente con:

- Facturación postpago.
- Procedimiento almacenado de SQL Server.
- WS Proveedor para facturación.
- WS Autenticación.
- MongoDB.
- Seguridad de usuarios.

#### Charlie debe instalar

- Visual Studio Community o la tecnología escogida para el WS de autenticación.
- SDK de .NET compatible con el proyecto.
- Git.
- VS Code.
- Tailscale.
- SoapUI.
- MongoDB Compass.
- SQL Server Management Studio.
- Cliente o extensión de MongoDB.
- Paquete o driver oficial para MongoDB.
- Herramienta para probar expresiones regulares y validaciones.
- Herramienta para inspeccionar solicitudes SOAP/XML.

El WS de autenticación puede desarrollarse con la herramienta que el equipo seleccione, pero debe usar SOAP/XML. El WS Proveedor y el WS Identificador deben desarrollarse en C#.

## 4. Decisiones obligatorias antes de programar

Antes de que cada integrante comience sus historias, los tres deben reunirse y dejar documentadas las decisiones siguientes.

### 4.1 Versión de .NET

Deben acordar:

- Versión de .NET.
- Tipo de proyecto.
- Tecnología SOAP.
- Versión de Visual Studio.
- Arquitectura de compilación.

Ejemplo de decisión:

Lenguaje: C#

Framework: .NET Framework o .NET moderno

SOAP: WCF, CoreWCF o ASMX

Arquitectura: x64

No debe ocurrir que Gabriel cree un servicio en una tecnología y José intente consumirlo desde otra incompatible.

### 4.2 Formato de las tramas

Deben definir:

- Separador.
- Orden de los campos.
- Codificación.
- Campos obligatorios.
- Campos opcionales.
- Cifrado.
- Respuestas exactas.
- Manejo de datos nulos.
- Manejo de caracteres especiales.

Documento recomendado:

`/docs/contratos-integracion.md`

Este archivo deberá ser aprobado por Gabriel, José y Charlie antes de iniciar la programación completa.

### 4.3 Estructura de respuestas

Para los Web Services se utilizará una estructura uniforme:

Resultado = true/false

Mensaje = texto correspondiente

Ejemplo conceptual en XML:

\<Respuesta\>

\<Resultado\>true\</Resultado\>

\<Mensaje\>Exitoso\</Mensaje\>

\</Respuesta\>

Los mensajes deben respetar los textos exigidos por la profesora.

### 4.4 Cifrado

Deben decidir conjuntamente:

- Algoritmo.
- Tamaño de la clave.
- Vector de inicialización.
- Conversión a Base64.
- Qué campos se cifran.
- En qué punto se cifran.
- En qué punto se descifran.
- Dónde se almacenan las claves.

Se deben cifrar, según la historia correspondiente:

- Número telefónico.
- Identificador del teléfono.
- Identificador de tarjeta.
- Usuario.
- Contraseña.

La clave real no debe publicarse en GitHub.

Documento recomendado:

`/docs/estandar-cifrado.md`

### 4.5 Formato de fechas

Charlie y el equipo deben decidir un formato uniforme para:

- Fecha de cálculo.
- Fecha máxima de pago.
- Fechas almacenadas en SQL Server.
- Fechas enviadas mediante SOAP.

Ejemplo:

YYYY-MM-DD

El formato escogido debe documentarse y utilizarse en todos los componentes.

## 5. Responsabilidades de Gabriel

### Gabriel — Alta de líneas y consulta de saldo

#### Historias

PROVEEDOR4

WS_PROVEEDOR1

WS_IDENTIFICADOR1

#### Objetivo general

Gabriel será responsable de la creación inicial de líneas telefónicas disponibles y de la operación SOAP/XML que permitirá consultar el saldo.

### 5.1 Historia PROVEEDOR4

Gabriel debe implementar la funcionalidad del Proveedor que recibe una trama proveniente del WS Proveedor.

La trama debe contener:

- Número de teléfono.
- Identificador del teléfono de 16 dígitos.
- Identificador de tarjeta de 19 dígitos.
- Tipo prepago o postpago.
- Estado disponible.

Los datos sensibles deben enviarse y almacenarse cifrados. El Proveedor debe comprobar que todos los datos estén completos y que el número telefónico no esté en uso.

#### Gabriel debe programar

- Recepción de la trama.
- Separación de campos.
- Validación de cantidad de campos.
- Validación de valores vacíos.
- Validación de 16 dígitos.
- Validación de 19 dígitos.
- Validación del tipo de línea.
- Validación del estado.
- Consulta de teléfono duplicado.
- Cifrado o validación del cifrado.
- Inserción en SQL Server.
- Manejo controlado de excepciones.

#### Respuestas requeridas

OK

Datos Incompletos

Teléfono en uso

ERROR

#### Pruebas mínimas de Gabriel

1.  Línea prepago válida.
2.  Línea postpago válida.
3.  Número duplicado.
4.  Identificador de teléfono incompleto.
5.  Identificador de tarjeta incompleto.
6.  Tipo inválido.
7.  Estado inválido.
8.  Datos vacíos.
9.  Falla de conexión con SQL Server.

10. Datos cifrados incorrectamente.

### 5.2 Historia WS_PROVEEDOR1

Gabriel debe crear la operación SOAP para registrar una nueva línea disponible.

#### Responsabilidades

- Recibir los campos del servicio.
- Recibir los datos cifrados.
- Validar que la solicitud tenga estructura correcta.
- Construir la trama para PROVEEDOR4.
- Enviar la trama.
- Interpretar la respuesta.
- Convertirla en una respuesta SOAP.

#### Respuesta exitosa

Resultado = true

Mensaje = Exitoso

#### Respuesta no exitosa

Resultado = false

Mensaje = Problemas al incluir la información.

#### Evidencia requerida

- Solicitud SOAP.
- Respuesta SOAP.
- Registro insertado en SQL Server.
- Captura o archivo de SoapUI.
- Evidencia del dato cifrado en la base.

### 5.3 Historia WS_IDENTIFICADOR1

Gabriel debe desarrollar la operación SOAP/XML para consultar saldo.

El servicio debe distinguir el origen:

Origen Web

Origen teléfono

Cuando el origen sea Web, se aplicarán las reglas especiales indicadas en el documento. Cuando sea teléfono, deberán validarse todos los campos del flujo existente.

#### Responsabilidades

- Recibir XML.
- Validar estructura.
- Validar número telefónico.
- Validar origen.
- Validar tipo de transacción saldo.
- Construir la trama hacia el Proveedor.
- Recibir el saldo.
- Devolver una respuesta XML.
- Manejar errores.

#### Respuesta esperada

Resultado: ok

Saldo: 0000000000000125000

La historia también requiere modificar la funcionalidad correspondiente del Proveedor para incorporar el nuevo campo de origen y aplicar validaciones según ese origen.

### 5.4 Entregables de Gabriel

Código PROVEEDOR4

Código WS_PROVEEDOR1

Código WS_IDENTIFICADOR1

Scripts SQL asociados

Solicitudes SOAP de prueba

Documento de tramas de alta

Documento de consulta de saldo

Casos de prueba

Evidencias

### 5.5 Dependencias de Gabriel

Gabriel depende de:

- SQL Server disponible en la computadora de José.
- Contrato de cifrado acordado.
- Formato de respuesta común.
- Datos del Proveedor existentes.
- Acceso al código del primer alcance.

José depende de Gabriel para conocer:

- Cómo quedan almacenadas las líneas.
- Qué campos se utilizan.
- Cómo se cifran.
- Qué estado representa una línea disponible.

## 6. Responsabilidades de José

### José — Activación, desactivación e integración

#### Historias

PROVEEDOR5

IDENTIFICADOR6

WS_PROVEEDOR2

#### Objetivo general

José será responsable del flujo completo de activación y desactivación de una línea telefónica.

Este es uno de los flujos con mayor nivel de integración porque conecta:

WS_PROVEEDOR2

↓

PROVEEDOR5

↓

IDENTIFICADOR6

↓

MySQL

### 6.1 Historia PROVEEDOR5

José debe implementar la funcionalidad que activa o desactiva una línea.

La trama debe incluir:

- Número telefónico.
- Identificador del teléfono.
- Identificador de tarjeta.
- Tipo.
- Identificación del dueño.
- Estado.

#### Para activar

José debe verificar:

- Datos completos.
- Línea en estado disponible.
- Número telefónico no utilizado.
- Identificadores correctos.
- Cliente indicado.
- Tipo prepago o postpago.

Si es prepago:

Saldo inicial = ₡1 000

Además deberá:

- Cambiar la línea a activa.
- Asociarla al cliente.
- Enviar los datos al Identificador.
- Esperar la respuesta del Identificador.
- Responder OK únicamente si el Identificador respondió OK.

#### Para desactivar

José deberá:

- Verificar que la línea esté activa.
- Verificar que pertenezca al dueño indicado.
- Desasociarla del cliente.
- Inactivar el servicio.
- Informar al Identificador.
- Confirmar el resultado.

### 6.2 Consistencia entre sistemas

José debe evitar que la línea quede activa en SQL Server, pero no registrada correctamente en MySQL.

La secuencia recomendada será:

1\. Validar solicitud.

2\. Consultar estado actual.

3\. Preparar cambio.

4\. Enviar solicitud al Identificador.

5\. Recibir confirmación.

6\. Confirmar cambio local.

7\. Responder al WS.

El equipo debe decidir si utilizará:

- Transacción SQL.
- Cambio provisional de estado.
- Rollback lógico.
- Compensación manual.

La opción escogida debe documentarse.

### 6.3 Respuestas de PROVEEDOR5

OK

Datos Incompletos

Teléfono en uso

Teléfono no corresponde

Activación fallida

ERROR

#### Pruebas mínimas

1.  Activar línea prepago válida.
2.  Activar línea postpago válida.
3.  Activar línea ya activa.
4.  Activar número en uso.
5.  Desactivar línea activa.
6.  Desactivar línea que no pertenece al cliente.
7.  Datos incompletos.
8.  Identificador fuera de servicio.
9.  Respuesta inválida del Identificador.

10. Error en SQL Server.

11. Error en MySQL.

12. Verificación del saldo inicial prepago.

### 6.4 Historia IDENTIFICADOR6

José debe implementar en el Identificador la recepción de la trama enviada por el Proveedor.

#### Responsabilidades

- Recibir la trama.
- Validar campos.
- Validar datos cifrados.
- Registrar o actualizar el teléfono.
- Asociar el número al Proveedor.
- Asociar la identificación del cliente.
- Cambiar el estado.
- Guardar en MySQL.
- Responder al Proveedor.

#### Respuestas

OK

Activación fallida

La historia establece que el Identificador debe recibir los datos de la línea, validar que estén completos e incluirlos entre los teléfonos asociados al Proveedor.

### 6.5 Historia WS_PROVEEDOR2

José debe desarrollar la operación SOAP para activar o desactivar la línea.

#### Flujo

Solicitud SOAP

↓

Validación

↓

Construcción de trama

↓

PROVEEDOR5

↓

IDENTIFICADOR6

↓

Respuesta SOAP

#### Respuesta exitosa

Resultado = true

Mensaje = Exitoso

#### Respuesta no exitosa

Resultado = false

Mensaje = Problemas al activar/desactivar la línea.

### 6.6 Responsabilidades adicionales de José como administrador del entorno

José deberá:

- Mantener encendida la máquina servidor durante sesiones acordadas.
- Confirmar conexión Tailscale.
- Habilitar puertos.
- Crear usuarios de base de datos.
- Mantener respaldos.
- Documentar direcciones y puertos.
- Evitar publicar credenciales.
- Mantener el Simulador de Llamadas.
- Coordinar pruebas integradas.

Esto no significa que José sea responsable del código de Gabriel o Charlie. Su responsabilidad adicional es únicamente mantener el entorno disponible.

### 6.7 Entregables de José

Código PROVEEDOR5

Código IDENTIFICADOR6

Código WS_PROVEEDOR2

Scripts de SQL Server

Scripts de MySQL

Datos de prueba

Pruebas SOAP

Pruebas de integración

Manejo de rollback o compensación

Documentación del entorno remoto

Evidencias

## 7. Responsabilidades de Charlie

### Charlie — Facturación y autenticación

#### Historias

PROVEEDOR6

WS_PROVEEDOR3

WS_AUTENTICACION1

WS_AUTENTICACION2

#### Objetivo general

Charlie será responsable de dos bloques:

1.  Facturación postpago.
2.  Seguridad y administración de usuarios.

### 7.1 Historia PROVEEDOR6

Charlie debe implementar la funcionalidad para calcular la facturación de servicios postpago.

La trama debe contener:

- Fecha de cálculo.
- Fecha máxima de pago.

#### Responsabilidades

- Validar que las fechas existan.
- Validar formato.
- Validar coherencia.
- Ejecutar procedimiento almacenado.
- Procesar todos los servicios postpago.
- Calcular montos.
- Guardar la fecha máxima de pago.
- Responder al WS.

El requerimiento exige crear un procedimiento almacenado en SQL Server para calcular el saldo en llamadas de todos los servicios postpago.

#### Respuestas

OK

ERROR

#### Pruebas mínimas

1.  Fecha válida.
2.  Fecha máxima válida.
3.  Fecha de pago anterior a la fecha de cálculo.
4.  Fecha vacía.
5.  Servicio prepago excluido.
6.  Servicio postpago sin llamadas.
7.  Servicio postpago con llamadas.
8.  Error de procedimiento almacenado.
9.  Error de conexión.

10. Ejecución repetida.

### 7.2 Procedimiento almacenado

Charlie será responsable del procedimiento almacenado.

Debe definir junto con el equipo:

- Fuente de registros de llamadas.
- Tarifa aplicable.
- Periodo de facturación.
- Campo del monto.
- Campo de fecha máxima.
- Tratamiento de llamadas ya facturadas.
- Tratamiento de líneas inactivas.
- Manejo de duplicados.

El procedimiento debe entregarse como script reproducible:

`/database/sqlserver/procedures/calcular_facturacion_postpago.sql`

### 7.3 Historia WS_PROVEEDOR3

Charlie debe desarrollar la operación SOAP que activa el cálculo de facturación.

#### Responsabilidades

- Recibir fecha de cálculo.
- Recibir fecha máxima.
- Validar estructura.
- Construir la trama.
- Invocar PROVEEDOR6.
- Interpretar respuesta.
- Generar respuesta SOAP.

#### Respuesta exitosa

Resultado = true

Mensaje = Exitoso

#### Respuesta no exitosa

Resultado = false

Mensaje = Problemas al realizar el cálculo.

### 7.4 Historia WS_AUTENTICACION1

Charlie deberá crear el servicio SOAP/XML para autenticar usuarios.

El servicio recibirá:

- Usuario.
- Contraseña.
- Tipo de usuario.

Debe verificar:

- Existencia del usuario.
- Contraseña correcta.
- Estado activo.
- Tipo correcto.
- Permiso de ingreso.

#### Respuesta correcta

Resultado = true

Mensaje = Exitoso

#### Respuesta incorrecta

Resultado = false

Mensaje = Usuario y/o contraseña incorrectos.

El usuario y la contraseña deben viajar cifrados. La información debe consultarse en MongoDB.

### 7.5 Historia WS_AUTENTICACION2

Charlie debe implementar tres métodos.

#### Método 1: crear usuario

Debe recibir:

- Identificación.
- Nombre.
- Primer apellido.
- Segundo apellido.
- Correo.
- Usuario.
- Contraseña.
- Estado.
- Tipo.

Debe validar:

- Campos completos.
- Nombres no vacíos.
- Nombres sin números.
- Correo válido.
- Usuario no duplicado.
- Identificación no duplicada.
- Estado activo para usuario nuevo.
- Tipo 1 o 2.
- Contraseña de exactamente 14 caracteres.
- Al menos una mayúscula.
- Al menos una minúscula.
- Al menos un número.
- Al menos un carácter especial.

#### Respuesta exitosa

Resultado = true

Mensaje = Exitoso

#### Respuesta no exitosa

Resultado = false

Mensaje = Usuario ya existe o datos incorrectos o incompletos.

#### Método 2: modificar usuario

Debe validar:

- Usuario existente.
- Identificación válida.
- Campos completos.
- Correo válido.
- Contraseña válida.
- Campos llave no modificables.
- Campos no presentes no modificables.

#### Respuesta no exitosa

Resultado = false

Mensaje = Usuario no existe o datos incorrectos o incompletos.

#### Método 3: activar o inactivar usuario

Debe recibir:

- Identificación.
- Estado.

Debe validar:

- Existencia del usuario.
- Estado permitido.

#### Respuesta no exitosa

Resultado = false

Mensaje = Usuario no existe o datos incorrectos.

Los datos de usuario y contraseña deben guardarse cifrados en MongoDB.

### 7.6 Modelo MongoDB

Charlie será responsable de proponer el esquema inicial.

Ejemplo conceptual:

{

"identificacion": "dato",

"nombre": "dato",

"primerApellido": "dato",

"segundoApellido": "dato",

"correo": "dato",

"usuario": "dato_cifrado",

"contrasena": "dato_cifrado",

"estado": "activo",

"tipo": 1

}

El modelo definitivo deberá acordarse antes de programar.

Charlie deberá preparar:

`/database/mongodb/crear_coleccion.js`

`/database/mongodb/indices.js`

`/database/mongodb/datos_semilla.js`

`/database/mongodb/README.md`

### 7.7 Entregables de Charlie

Código PROVEEDOR6

Código WS_PROVEEDOR3

Código WS_AUTENTICACION1

Código WS_AUTENTICACION2

Procedimiento almacenado

Modelo MongoDB

Índices MongoDB

Datos semilla

Pruebas SOAP

Pruebas de validación

Casos positivos y negativos

Evidencias

## 8. Roadmap por fases

### Fase 0 — Preparación y acuerdos

#### Duración sugerida

Día 1 y Día 2

#### Participantes

Gabriel + José + Charlie

#### Actividades

- Revisar completamente el PDF.
- Revisar el código del primer alcance.
- Identificar pendientes anteriores.
- Verificar compilación.
- Verificar bases de datos.
- Confirmar las historias asignadas.
- Publicar la distribución en Teams.
- Definir .NET.
- Definir SOAP.
- Definir cifrado.
- Definir formato de tramas.
- Definir formato XML.
- Definir puertos.
- Definir estructura Git.
- Definir convenciones de nombres.
- Definir manejo de errores.

#### Resultado verificable

Acta de acuerdos técnicos

Distribución de historias

Contratos de integración versión 1

Repositorio organizado

### Fase 1 — Preparación de entornos

#### Duración sugerida

Día 3 al Día 5

#### Gabriel

- Instalar herramientas.
- Verificar conexión a SQL Server.
- Verificar conexión a MySQL.
- Crear rama de PROVEEDOR4.
- Preparar solicitud de prueba de alta.
- Revisar el código actual del Proveedor.

#### José

- Instalar MongoDB.
- Configurar MongoDB.
- Configurar Tailscale.
- Configurar firewall.
- Crear usuarios de bases de datos.
- Probar acceso remoto.
- Verificar Simulador de Llamadas.
- Preparar datos iniciales.

#### Charlie

- Instalar MongoDB Compass.
- Conectarse remotamente.
- Diseñar colección de usuarios.
- Diseñar procedimiento almacenado.
- Revisar tablas de llamadas y servicios.
- Preparar ramas de autenticación y facturación.

#### Resultado verificable

- Tres compañeros conectados.
- Bases accesibles.
- Repositorios compilando.
- Datos iniciales disponibles.
- Conexiones probadas.

### Fase 2 — Desarrollo independiente inicial

#### Duración sugerida

Día 6 al Día 10

#### Gabriel

Desarrolla:

PROVEEDOR4

WS_PROVEEDOR1

Debe publicar temprano:

- Formato de alta.
- Campos.
- Respuestas.
- Cifrado.
- Datos insertados.

#### José

Desarrolla:

IDENTIFICADOR6

Base de PROVEEDOR5

Puede usar un stub mientras Gabriel finaliza el alta.

#### Charlie

Desarrolla:

WS_AUTENTICACION1

Base de WS_AUTENTICACION2

Procedimiento almacenado

#### Resultado verificable

- Alta de línea local.
- Activación local simulada.
- Autenticación funcionando.
- Procedimiento almacenado ejecutable.

### Fase 3 — Desarrollo funcional completo

#### Duración sugerida

Día 11 al Día 13

#### Gabriel

Completa:

WS_IDENTIFICADOR1

Ajuste de origen Web/teléfono

#### José

Completa:

PROVEEDOR5

WS_PROVEEDOR2

Integración con IDENTIFICADOR6

#### Charlie

Completa:

PROVEEDOR6

WS_PROVEEDOR3

WS_AUTENTICACION2

#### Resultado verificable

Las diez historias deben estar codificadas y funcionar individualmente.

### Fase 4 — Integración

#### Duración sugerida

Día 14 al Día 16

#### Flujo 1

WS_PROVEEDOR1

↓

PROVEEDOR4

↓

SQL Server

Responsable principal: **Gabriel**

Apoyo de ambiente: **José**

#### Flujo 2

WS_PROVEEDOR2

↓

PROVEEDOR5

↓

IDENTIFICADOR6

↓

MySQL

Responsable principal: **José**

Datos iniciales: **Gabriel**

#### Flujo 3

WS_PROVEEDOR3

↓

PROVEEDOR6

↓

Procedimiento almacenado

↓

SQL Server

Responsable principal: **Charlie**

Apoyo de datos: **José**

#### Flujo 4

WS_AUTENTICACION1/2

↓

MongoDB

Responsable principal: **Charlie**

Apoyo de infraestructura: **José**

#### Flujo 5

WS_IDENTIFICADOR1

↓

Proveedor

↓

Consulta de saldo

Responsable principal: **Gabriel**

Apoyo de integración: **José**

### Fase 5 — Pruebas completas

#### Duración sugerida

Día 17 y Día 18

Cada integrante debe probar sus historias, pero las pruebas finales deben ejecutarse conjuntamente.

#### Gabriel prueba

- Registro de línea.
- Duplicados.
- Datos incompletos.
- Consulta Web.
- Consulta desde teléfono.
- XML correcto.
- XML incorrecto.

#### José prueba

- Activación.
- Desactivación.
- Dueño incorrecto.
- Línea ya activa.
- Línea inexistente.
- Error del Identificador.
- Saldo inicial.
- Consistencia SQL Server–MySQL.

#### Charlie prueba

- Facturación postpago.
- Fechas inválidas.
- Usuario válido.
- Usuario inactivo.
- Contraseña incorrecta.
- Alta de usuario.
- Duplicado.
- Modificación.
- Cambio de estado.
- Validaciones de contraseña.

#### Resultado verificable

Matriz de pruebas completa

Capturas SOAP

Resultados en bases de datos

Errores documentados

Correcciones aplicadas

### Fase 6 — Documentación y cierre

#### Participantes

Gabriel + José + Charlie

Aunque cada uno debe documentar sus historias, los documentos finales deben quedar unificados.

#### Gabriel documenta

- Alta de líneas.
- Consulta de saldo.
- Contratos XML de sus historias.
- Casos de uso correspondientes.
- Clases relacionadas.

#### José documenta

- Activación/desactivación.
- Comunicación Proveedor–Identificador.
- Arquitectura de red.
- Conexiones remotas.
- Flujo integrado.

#### Charlie documenta

- MongoDB.
- Autenticación.
- Administración de usuarios.
- Facturación.
- Procedimiento almacenado.
- Reglas de validación.

#### Documentación final

El PDF exige:

- Portada.
- Introducción.
- Diagrama de base de datos actualizado.
- Casos de uso actualizados.
- Diagrama de clases actualizado.
- Conclusiones.
- Recomendaciones.
- Bibliografía.

### Fase 7 — Ensayo de entrega

Debe realizarse desde una computadora limpia o desde un entorno que simule la revisión de la profesora.

#### Deben comprobar

- Clonado del repositorio.
- Restauración de dependencias.
- Ejecución de scripts.
- Acceso SQL Server.
- Acceso MySQL.
- Acceso MongoDB.
- Inicio de servicios.
- URLs SOAP.
- Pruebas con SoapUI.
- Simulador funcionando.
- Datos cifrados.
- Documentación disponible.
- Código publicado en Teams.

Si algún componente todavía no está disponible, deben incluir un stub o simulador que permita demostrar la historia. Esto está expresamente autorizado y exigido por el documento.

## 9. Estrategia Git

#### Ramas generales

main

dev

#### Ramas de Gabriel

feature/PROVEEDOR4

feature/WS-PROVEEDOR1

feature/WS-IDENTIFICADOR1

#### Ramas de José

feature/PROVEEDOR5

feature/IDENTIFICADOR6

feature/WS-PROVEEDOR2

#### Ramas de Charlie

feature/PROVEEDOR6

feature/WS-PROVEEDOR3

feature/WS-AUTENTICACION1

feature/WS-AUTENTICACION2

#### Flujo de integración

feature/\*

↓

dev

↓

pruebas integradas

↓

main

Nadie debe trabajar directamente en main.

## 10. Regla de revisión cruzada

Cada pull request debe ser revisado por otro integrante.

| **Autor** | **Revisor principal sugerido** |
|-----------|--------------------------------|
| Gabriel   | José                           |
| José      | Charlie                        |
| Charlie   | Gabriel                        |

Esto no cambia la propiedad de la historia. El autor sigue siendo responsable de implementar y defender sus criterios de aceptación.

## 11. Matriz de responsabilidad

| **Componente**        | **Gabriel** | **José**              | **Charlie**              |
|-----------------------|-------------|-----------------------|--------------------------|
| SQL Server central    | Consulta    | Administra            | Consulta y desarrolla SP |
| MySQL central         | Consulta    | Administra/desarrolla | Consulta                 |
| MongoDB               | Consulta    | Administra servidor   | Diseña y desarrolla      |
| PROVEEDOR4            | Responsable | Apoyo                 | No aplica                |
| PROVEEDOR5            | Apoyo       | Responsable           | No aplica                |
| PROVEEDOR6            | No aplica   | Apoyo                 | Responsable              |
| IDENTIFICADOR6        | Apoyo       | Responsable           | No aplica                |
| WS_PROVEEDOR1         | Responsable | Apoyo                 | No aplica                |
| WS_PROVEEDOR2         | Apoyo       | Responsable           | No aplica                |
| WS_PROVEEDOR3         | No aplica   | Apoyo                 | Responsable              |
| WS_AUTENTICACION1     | Revisión    | Infraestructura       | Responsable              |
| WS_AUTENTICACION2     | Revisión    | Infraestructura       | Responsable              |
| WS_IDENTIFICADOR1     | Responsable | Apoyo                 | No aplica                |
| Simulador de llamadas | Pruebas     | Mantiene disponible   | Pruebas                  |
| Integración final     | Participa   | Coordina              | Participa                |

## 12. Condición de terminado por historia

Una historia no debe considerarse terminada solo porque compila.

Debe cumplir:

- Código completo.
- Criterios de aceptación.
- Validaciones.
- Respuestas exactas.
- Prueba positiva.
- Pruebas negativas.
- Evidencia SOAP.
- Evidencia en base de datos.
- Datos sensibles cifrados.
- Manejo de errores.
- Pull request revisado.
- Integración con dev.
- Documentación actualizada.
- Capacidad de ejecutarse desde otro equipo.

## 13. Resultado esperado por integrante

#### Gabriel deberá poder demostrar

Registro una nueva línea mediante SOAP, la información llega al Proveedor, se valida, se almacena cifrada en SQL Server y luego puedo consultar el saldo mediante el WS Identificador.

#### José deberá poder demostrar

Recibo una solicitud SOAP para activar o desactivar una línea, valido la información, actualizo el Proveedor, sincronizo el Identificador y mantengo consistencia entre SQL Server y MySQL.

#### Charlie deberá poder demostrar

Calculo la facturación postpago mediante un procedimiento almacenado y administro la autenticación, creación, modificación y estado de usuarios mediante SOAP/XML y MongoDB.

## 14. Orden recomendado para comenzar

El equipo no debería iniciar directamente con las historias. El orden correcto sería:

1.  Confirmar que el alcance anterior compila.
2.  Corregir pendientes anteriores.
3.  Instalar MongoDB.
4.  Probar Tailscale desde las tres casas.
5.  Probar acceso remoto a las tres bases.
6.  Definir tecnología SOAP.
7.  Definir cifrado.
8.  Definir tramas.
9.  Definir respuestas.

10. Crear ramas.

11. Crear datos semilla.

12. Iniciar desarrollo independiente.

13. Integrar progresivamente.

14. Ejecutar pruebas completas.

15. Actualizar documentación.

16. Ensayar la entrega.