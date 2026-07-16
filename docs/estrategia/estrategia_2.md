**PLAN DE DISTRIBUCIÓN Y ROADMAP**

**Segundo alcance - Sistema base**

Sistema de Control de Llamadas Telefónicas

| **Curso**                     | Programación IV                                       |
|-------------------------------|-------------------------------------------------------|
| **Proyecto**                  | Segundo Proyecto Programado - Alcance 2               |
| **Equipo**                    | Compañero 1, Compañero 2 y Compañero 3                |
| **Fecha de entrega indicada** | Miércoles 22 de julio de 2026, antes de las 6:00 p.m. |

*Documento elaborado a partir del requerimiento “Proyecto - Alcance 2”.*

# 1. Propósito del documento

Este documento analiza el alcance funcional y técnico solicitado para el segundo avance del Sistema de Control de Llamadas Telefónicas. Su objetivo es convertir los requerimientos en un plan de trabajo claro, balanceado y verificable para tres integrantes del equipo, sin dividir una misma historia entre varias personas.

La propuesta respeta la regla del enunciado: cada estudiante debe desarrollar al menos tres historias y cada historia tiene una persona responsable. Además, incorpora tareas de integración, pruebas y documentación para evitar que los componentes se evalúen de manera aislada.

# 2. Lectura ejecutiva del alcance

El sistema está compuesto por cuatro bloques conectados: Identificador con MySQL, Proveedor con SQL Server, WS Proveedor, y WS de Autenticación conectado a MongoDB. El documento también solicita mantener el Simulador de Llamadas disponible para probar los flujos integrados.

## 2.1 Historias funcionales identificadas

| **Historia**          | **Resultado esperado**                                         | **Bloque principal**       |
|-----------------------|----------------------------------------------------------------|----------------------------|
| **PROVEEDOR4**        | Registrar una línea/tarjeta telefónica disponible.             | Proveedor / SQL Server     |
| **PROVEEDOR5**        | Activar o desactivar una línea y notificar al Identificador.   | Proveedor + Identificador  |
| **PROVEEDOR6**        | Ejecutar cálculo de facturación para servicios postpago.       | Proveedor / SQL Server     |
| **IDENTIFICADOR6**    | Registrar en el Identificador la línea asociada al Proveedor.  | Identificador / MySQL      |
| **WS_PROVEEDOR1**     | Exponer la operación SOAP para registrar una línea disponible. | WS Proveedor               |
| **WS_PROVEEDOR2**     | Exponer la operación SOAP para activar/desactivar una línea.   | WS Proveedor               |
| **WS_PROVEEDOR3**     | Exponer la operación SOAP para cálculo de facturas.            | WS Proveedor               |
| **WS_AUTENTICACION1** | Autenticar usuarios mediante servicio SOAP XML.                | WS Autenticación / MongoDB |
| **WS_AUTENTICACION2** | Crear, modificar y activar/inactivar usuarios.                 | WS Autenticación / MongoDB |
| **WS_IDENTIFICADOR1** | Consultar saldo por medio de una operación SOAP XML.           | WS Identificador           |

Total: 10 historias. La distribución propuesta asigna 3, 3 y 4 historias respectivamente.

## 2.2 Reglas técnicas que condicionan el desarrollo

- El WS Proveedor, el WS Identificador y sus operaciones deben implementarse en C# con comunicación SOAP.

- El WS de autenticación debe utilizar SOAP/XML y puede desarrollarse con la herramienta elegida por el equipo.

- Usuario y contraseña deben viajar cifrados y almacenarse cifrados. Los datos telefónicos sensibles también deben enviarse y almacenarse cifrados.

- La autenticación debe usar MongoDB como base documental; Proveedor trabaja con SQL Server e Identificador con MySQL.

- Los componentes deben probarse integrados. Si algún componente aún no está listo, debe existir un simulador o stub que permita demostrar la historia.

- La documentación debe actualizar diagramas de base de datos, casos de uso y clases, además de incluir introducción, conclusiones, recomendaciones y bibliografía.

# 3. Análisis de dependencias y riesgo

La principal dificultad no está en crear operaciones aisladas, sino en asegurar el encadenamiento de las tramas: WS Proveedor -\> Proveedor -\> Identificador, y WS Autenticación -\> MongoDB. Por ello, antes de programar cada módulo deben acordarse contratos de entrada/salida, formato de trama, respuestas permitidas y reglas de cifrado.

| **Flujo**     | **Dependencia**                                       | **Riesgo si no se coordina**                                  | **Control**                                          |
|---------------|-------------------------------------------------------|---------------------------------------------------------------|------------------------------------------------------|
| Alta de línea | WS_PROVEEDOR1 -\> PROVEEDOR4                          | La trama o la respuesta no coinciden.                         | Contrato común + prueba SOAP                         |
| Activación    | WS_PROVEEDOR2 -\> PROVEEDOR5 -\> IDENTIFICADOR6       | La línea queda activa en un sistema y no en otro.             | Prueba extremo a extremo + rollback lógico           |
| Facturación   | WS_PROVEEDOR3 -\> PROVEEDOR6 -\> SQL                  | Procedimiento almacenado no calcula o no guarda fecha máxima. | Datos semilla y caso postpago controlado             |
| Saldo         | WS_IDENTIFICADOR1 -\> Proveedor/PROVEEDOR4 modificado | Campos obligatorios difieren según origen Web/teléfono.       | Matriz de validaciones por origen                    |
| Autenticación | WS_AUTENTICACION1/2 -\> MongoDB                       | Credenciales o estados no se cifran/validan correctamente.    | Pruebas de contraseña, duplicados y usuario inactivo |

# 4. Distribución propuesta entre tres compañeros

Se agrupan las historias por flujo funcional para que cada persona tenga ownership real sobre una parte del sistema. Aunque hay puntos de integración, la codificación y aceptación de cada historia queda a cargo de una sola persona.

| **Responsable** | **Historias asignadas** | **Motivo de agrupación** | **Carga estimada** |
|---|---|---|---|
| **Compañero 1** | PROVEEDOR4<br>WS_PROVEEDOR1<br>WS_IDENTIFICADOR1 | Controla el ciclo de alta y consulta. PROVEEDOR4 se amplía para contemplar el origen Web requerido por WS_IDENTIFICADOR1; por eso conviene una sola persona responsable. | 3 historias<br>Alta-media |
| **Compañero 2** | PROVEEDOR5<br>IDENTIFICADOR6<br>WS_PROVEEDOR2 | Controla el flujo completo de activación/desactivación: entrada SOAP, validación, cambio de estado y sincronización con el Identificador. | 3 historias<br>Alta |
| **Compañero 3** | PROVEEDOR6<br>WS_PROVEEDOR3<br>WS_AUTENTICACION1<br>WS_AUTENTICACION2 | Responsable de facturación postpago y del bloque de seguridad/usuarios. Tiene cuatro historias porque WS_AUTENTICACION1 y PROVEEDOR6 son más acotadas que WS_AUTENTICACION2. | 4 historias<br>Alta |

## 4.1 Responsabilidades transversales sin romper la regla de historias

- Compañero 1: custodiar el contrato de tramas de alta/consulta y mantener el proyecto del Simulador de Llamadas utilizable para pruebas de su flujo.

- Compañero 2: liderar la prueba integrada de activación/desactivación y documentar los casos de error entre Proveedor e Identificador.

- Compañero 3: custodiar la configuración de MongoDB, datos semilla de usuarios y el catálogo de pruebas de autenticación/facturación.

- Los tres: revisar pull requests, ejecutar pruebas conjuntas y actualizar el documento final; la responsabilidad de cada historia no se transfiere ni se comparte.

# 5. Detalle de trabajo por responsable

## Compañero 1

Dueño del flujo de registro inicial de líneas y de la consulta de saldo desde el Web Service del Identificador.

### Entregables de código

- PROVEEDOR4: recepción de trama plana, validación de datos, verificación de teléfono no existente, cifrado y persistencia de número/identificadores; respuestas exactas OK, Datos Incompletos, Teléfono en uso o ERROR.

- WS_PROVEEDOR1: operación SOAP que recibe datos cifrados, arma la trama para PROVEEDOR4 y devuelve Resultado/Mensaje según corresponda.

- WS_IDENTIFICADOR1: operación SOAP XML de consulta de saldo, diferenciando origen Web y teléfono; respuesta XML con saldo o error.

- Ajuste asociado a PROVEEDOR4 para soportar el nuevo campo de origen y reglas de validación indicadas para la consulta.

### Criterios de terminado

- No permite teléfono repetido.

- La información sensible viaja y queda cifrada.

- La operación SOAP puede probarse con SoapUI, WCF Test Client u otra herramienta.

- Existe prueba positiva y pruebas negativas: datos incompletos, teléfono en uso y origen inválido.

### Punto de integración

Debe publicar el contrato de entrada/salida de PROVEEDOR4 antes de que se integren WS_PROVEEDOR1 y WS_IDENTIFICADOR1.

## Compañero 2

Dueño del flujo de activación/desactivación; es el flujo con mayor dependencia entre el WS Proveedor, Proveedor e Identificador.

### Entregables de código

- WS_PROVEEDOR2: operación SOAP para activar/desactivar con datos cifrados y respuesta estructurada.

- PROVEEDOR5: validaciones de línea disponible/activa, dueño correcto, asociación/desasociación, saldo inicial de ₡1 000 para prepago y manejo de mensajes definidos.

- IDENTIFICADOR6: recepción y validación de trama desde Proveedor; inclusión de datos en teléfonos disponibles asociados al proveedor; respuesta OK o Activación fallida.

- Pruebas del escenario activo, desactivado, dueño incorrecto, teléfono ya usado, datos incompletos y fallo del Identificador.

### Criterios de terminado

- Una activación solamente responde OK cuando el Identificador confirma OK.

- Una desactivación deja la línea sin dueño y el servicio inactivo.

- La activación prepago crea saldo inicial de 1000 colones.

- Los errores se retornan con el mensaje solicitado por el enunciado.

### Punto de integración

Requiere que Compañero 1 exponga los formatos de cifrado y los datos de línea creados por PROVEEDOR4. Debe entregar un script/dataset de prueba para ejecutar el flujo completo.

## Compañero 3

Dueño de facturación postpago y del bloque de autenticación/administración de usuarios en MongoDB.

### Entregables de código

- PROVEEDOR6: validación de fechas y ejecución del procedimiento almacenado para calcular facturación postpago; guardado de fecha máxima de pago.

- WS_PROVEEDOR3: operación SOAP para solicitar el cálculo y traducir la respuesta de PROVEEDOR6.

- WS_AUTENTICACION1: servicio SOAP/XML de inicio de sesión para empleados o clientes, con credenciales cifradas.

- WS_AUTENTICACION2: métodos SOAP/XML para crear, modificar y activar/inactivar usuarios; persistencia en MongoDB.

- Modelo documental, colección de usuarios y datos semilla iniciales.

### Criterios de terminado

- Contraseñas de exactamente 14 caracteres con mayúscula, minúscula, número y carácter especial.

- Correo válido; nombres y apellidos sin blancos, solo espacios o números.

- Usuarios nuevos se crean activos y con tipo 1 o 2.

- No permite usuarios duplicados; modificaciones respetan campos llave.

- Facturación se ejecuta mediante procedimiento almacenado y solamente para servicios postpago.

### Punto de integración

Debe compartir temprano el esquema MongoDB y el formato estándar Resultado/Mensaje. Para facturación necesita que existan líneas postpago y registros de llamadas de prueba.

# 6. Contratos de integración obligatorios

Antes de iniciar la implementación completa, el equipo debe congelar estos contratos en un archivo compartido (por ejemplo, /docs/contratos-integracion.md). Esto reduce retrabajo y permite probar componentes aunque otro módulo siga en desarrollo.

| **Contrato**   | **Datos mínimos**                                       | **Respuesta esperada**                                                        | **Responsable de publicar** |
|----------------|---------------------------------------------------------|-------------------------------------------------------------------------------|-----------------------------|
| Alta de línea  | teléfono, id teléfono(16), id tarjeta(19), tipo, estado | OK / Datos Incompletos / Teléfono en uso / ERROR                              | Compañero 1                 |
| Activación     | datos de línea, id cliente, estado activar/desactivar   | OK / Datos Incompletos / Teléfono no corresponde / Activación fallida / ERROR | Compañero 2                 |
| Facturación    | fecha cálculo, fecha máxima pago                        | OK / ERROR                                                                    | Compañero 3                 |
| Autenticación  | usuario, contraseña, tipo usuario                       | Resultado true/false + Mensaje                                                | Compañero 3                 |
| Consulta saldo | teléfono, origen, tipo transacción                      | XML: Resultado ok + saldo; o false + Mensaje                                  | Compañero 1                 |

# 7. Roadmap de desarrollo

El roadmap se organiza en fases y no depende de que todas las historias estén terminadas para comenzar. Las fechas exactas pueden moverse según el calendario del grupo; sin embargo, debe preservarse el orden de dependencias y dejar una semana de integración antes de la entrega.

| **Fase** | **Momento**      | **Responsable** | **Actividades principales**                                                                                                                                                     | **Salida verificable**                     |
|----------|------------------|-----------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------|
| **0**    | Día 1-2          | Los 3           | Revisar alcance; inventario del código previo; crear ramas; acordar naming, cifrado, formato de trama y contratos SOAP/XML.                                                     | Acta de acuerdos + contratos versión 1     |
| **1**    | Día 3-5          | C1 / C2 / C3    | Preparar ambientes: SQL Server, MySQL, MongoDB, proyectos de WS y datos semilla. C1 inicia PROVEEDOR4; C2 prepara entidades de activación; C3 crea MongoDB y SP de facturación. | Ambientes levantados + pruebas de conexión |
| **2**    | Día 6-9          | C1              | Completar PROVEEDOR4 y WS_PROVEEDOR1; publicar formatos y pruebas de alta.                                                                                                      | Alta de línea demostrable                  |
| **3**    | Día 6-10         | C2              | Completar IDENTIFICADOR6 y PROVEEDOR5 con mocks/stubs si hace falta.                                                                                                            | Lógica de activación local demostrable     |
| **4**    | Día 6-10         | C3              | Completar WS_AUTENTICACION1/2 y empezar PROVEEDOR6 + SP.                                                                                                                        | Login y CRUD de usuarios demostrables      |
| **5**    | Día 11-13        | C1 / C2 / C3    | C1 completa WS_IDENTIFICADOR1. C2 integra WS_PROVEEDOR2 con PROVEEDOR5 e IDENTIFICADOR6. C3 completa WS_PROVEEDOR3 y facturación.                                               | Todas las historias codificadas            |
| **6**    | Día 14-16        | Los 3           | Pruebas extremo a extremo: alta -\> activación -\> consulta saldo; usuario -\> autenticación; postpago -\> facturación. Corregir diferencias.                                   | Matriz de pruebas ejecutada                |
| **7**    | Día 17-18        | Los 3           | Actualizar diagramas, README, scripts de BD, datos de demostración y manual de pruebas. Empaquetar código.                                                                      | Documentación y repositorio listos         |
| **8**    | Antes de entrega | Los 3           | Ensayo de presentación desde una máquina limpia; validar rutas, servicios, bases de datos y respaldo en Teams.                                                                  | Entrega reproducible y publicada           |

## 7.1 Secuencia recomendada de trabajo por flujo

- Flujo 1 - Alta: WS_PROVEEDOR1 -\> PROVEEDOR4 -\> SQL Server.

- Flujo 2 - Activación/desactivación: WS_PROVEEDOR2 -\> PROVEEDOR5 -\> IDENTIFICADOR6 -\> MySQL.

- Flujo 3 - Facturación: WS_PROVEEDOR3 -\> PROVEEDOR6 -\> procedimiento almacenado en SQL Server.

- Flujo 4 - Autenticación: WS_AUTENTICACION1 y WS_AUTENTICACION2 -\> MongoDB.

- Flujo 5 - Saldo: WS_IDENTIFICADOR1 -\> consulta al Proveedor, considerando reglas especiales de origen Web/teléfono.

# 8. Plan mínimo de pruebas

| **Área**      | **Caso positivo**                    | **Casos negativos clave**                                    | **Evidencia**                     |
|---------------|--------------------------------------|--------------------------------------------------------------|-----------------------------------|
| Alta línea    | Agregar línea nueva disponible       | Datos incompletos, teléfono repetido, cifrado inválido       | Capturas SOAP + registro BD       |
| Activación    | Activar prepago de dueño válido      | Línea no disponible, dueño incorrecto, fallo Identificador   | Estado en ambas BDs + respuesta   |
| Desactivación | Desactivar línea activa del dueño    | Teléfono no corresponde, datos incompletos                   | Desasociación y servicio inactivo |
| Facturación   | Calcular postpago con fechas válidas | Fechas inválidas, falta de datos, error SP                   | Factura/monto y fecha máxima      |
| Autenticación | Login de usuario activo correcto     | Contraseña mala, usuario inexistente/inactivo, tipo inválido | Respuesta SOAP + datos cifrados   |
| Usuarios      | Crear/modificar/cambiar estado       | Duplicado, correo inválido, contraseña incumple regla        | Documento MongoDB antes/después   |
| Saldo         | Consulta Web exitosa                 | Origen inválido, teléfono inexistente, error proveedor       | XML de respuesta                  |

# 9. Entregables y carpeta final recomendada

Para que la entrega sea fácil de revisar, se recomienda publicar una estructura única y reproducible. Cada proyecto debe incluir instrucciones de ejecución, dependencias y datos de prueba.

- /docs - análisis y diseño: portada, introducción, diagramas actualizados, conclusiones, recomendaciones, bibliografía y matriz de pruebas.

- /database/sqlserver - scripts de tablas, procedimientos almacenados, datos semilla y guía de ejecución.

- /database/mysql - scripts actualizados para Identificador.

- /database/mongodb - definición de colección, datos iniciales y guía de importación.

- /src/proveedor - código actualizado del Proveedor Telefónico.

- /src/identificador - código actualizado del Identificador.

- /src/ws-proveedor - servicio SOAP del Proveedor.

- /src/ws-autenticacion - servicio SOAP/XML de autenticación.

- /src/ws-identificador - servicio SOAP/XML de consulta de saldo.

- /src/simulador-llamadas - código y configuración del simulador.

- /README.md - pasos para levantar todo el ecosistema y ejecutar las pruebas.

# 10. Acuerdos de control del equipo

- Crear una rama por historia: feature/PROVEEDOR4, feature/WS-PROVEEDOR1, etc. No hacer cambios directos a main.

- Abrir pull request por historia y solicitar al menos una revisión de otro compañero antes de fusionar.

- Versionar únicamente secretos de ejemplo; las claves reales y cadenas de conexión deben ir en archivos de configuración excluidos del repositorio.

- Registrar en un tablero simple el estado: Pendiente, En desarrollo, En prueba local, En integración, Aprobada.

- No cerrar una historia hasta contar con evidencia de prueba, respuesta esperada y actualización mínima de documentación.

- Realizar una revisión de integración conjunta al menos dos veces: al finalizar las historias y antes de publicar en Teams.

# 11. Conclusión

La distribución propuesta permite que cada integrante tenga responsabilidades claras, cumpla el mínimo de tres historias y trabaje sobre flujos completos en vez de tareas aisladas. Compañero 1 concentra alta y saldo; Compañero 2 concentra activación/desactivación e integración con Identificador; Compañero 3 concentra facturación y seguridad de usuarios. La ejecución disciplinada de los contratos, pruebas y fases de integración será indispensable para evitar la penalización por presentar componentes desconectados.

# 12. Referencia del requerimiento

Colegio Universitario de Cartago. (2026). Programación IV - Segundo Proyecto Programado: Sistema de Control de Llamadas Telefónicas, segundo alcance - Sistema base. Documento suministrado por el equipo docente.