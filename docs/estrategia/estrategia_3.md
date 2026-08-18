# CENTRAL TELEFÓNICA

## ALCANCE 3 – DISTRIBUCIÓN EQUITATIVA DE HISTORIAS
Propuesta de separación lógica para Compañero 1, Compañero 2 y Compañero 3

Proyecto Final Programado – Sistema de Control de Llamadas Telefónicas
Programación IV · II Cuatrimestre 2026

| Objetivo | Separar las historias del alcance final de forma lógica, equilibrada y técnicamente coherente. |
| --- | --- |
| Criterio de reparto | Equilibrio por cantidad de historias, complejidad, dependencias e integración. |
| Asignación nominal | Pendiente. La propuesta usa Compañero 1, Compañero 2 y Compañero 3 para que el equipo decida después quién asume cada bloque. |

**Base:** documento “Proyecto Final Programado – Alcance Final - Sistemas Web”, 19 páginas.

## 1. Resumen del alcance final
El alcance final introduce dos aplicaciones Web: una administrativa y otra para clientes, ambas conectadas a los servicios Web construidos en las etapas anteriores. Además, el proyecto debe completar o ampliar funcionalidades del Proveedor, del Identificador y de los Web Services cuando las operaciones requeridas aún no existan.
- Sitio Web Administrativo: obligatorio en C#.
- Sitio Web Cliente: puede desarrollarse en C# o en otro lenguaje; para reducir riesgos de integración se recomienda mantenerlo también en C#/.NET.
- Las aplicaciones Web no deben conectarse directamente a bases de datos ni a sockets. Toda operación debe pasar por servicios Web.
- Los servicios Web nuevos o ampliados pueden implementarse en la tecnología que el equipo elija y pueden acceder internamente por sockets, ADO.NET u ORM.
- Cada historia debe quedar desarrollada por un único estudiante; no se pueden compartir historias.
- El documento exige una cantidad de historias equitativa entre los integrantes.

## 2. Criterio utilizado para dividir el trabajo
La división propuesta no se limita a contar historias. También considera el peso técnico de cada una, la cantidad de validaciones, el número de servicios que consume, la necesidad de operaciones nuevas en WS_PROVEEDOR/WS_AUTENTICACIÓN, el manejo de pagos y correo, y la conveniencia de agrupar historias que comparten pantallas, modelos y flujo funcional.

## 3. Distribución propuesta

| Bloque | Historias | Cantidad | Enfoque principal | Carga estimada |
| --- | --- | --- | --- | --- |
| Compañero 1 | ADM1, ADM2, ADM3, ADM4, ADM5 | 5 | Administración de acceso y ciclo de vida de líneas | Media-Alta |
| Compañero 2 | ADM6, ADM7, CLIENTE1, CLIENTE2, CLIENTE3 | 5 | Facturación, usuarios, autenticación y base del portal cliente | Alta |
| Compañero 3 | CLIENTE4, CLIENTE5, CLIENTE6, CLIENTE7 | 4 | Autogestión transaccional del cliente | Alta |

Aunque el Compañero 3 tiene cuatro historias, su bloque concentra las operaciones transaccionales más pesadas (consulta consolidada de líneas, recargas, pago de facturas con correo y devolución de líneas). Por complejidad, la distribución queda más equilibrada que una separación basada únicamente en número de historias.

## 4. Base tecnológica recomendada

| Componente | Tecnología / herramienta | Observación |
| --- | --- | --- |
| Web Administrativo | C# + .NET en Visual Studio | C# es obligatorio. Usar un framework Web .NET compatible con los WS existentes. |
| Web Cliente | Recomendado: C# + .NET en Visual Studio | El documento permite otro lenguaje, pero C# reduce duplicidad de modelos, cifrado y consumo de WS. |
| WS Proveedor | .NET/WCF existente o ampliación compatible | Agregar consultas y operaciones nuevas que requieran ADM3-ADM6 y CLIENTE4-CLIENTE7. |
| WS Autenticación | .NET/WCF existente o ampliación compatible | Login, alta/modificación, activar/inactivar y eliminar usuarios. |
| Proveedor / Identificador | Java / Python según componentes existentes | Actualizar solamente lo necesario para soportar los nuevos servicios y mantener la arquitectura previa. |
| Bases de datos | MySQL / SQL Server / MongoDB según diseño existente | Solo accesibles desde servicios/componentes de backend; nunca desde las aplicaciones Web. |
| Integración | JSON + servicios Web | Mantener contratos claros entre interfaz y servicios; cifrar contraseña antes del envío. |

## 5. Detalle del bloque – Compañero 1
Este bloque contiene 5 historias y se recomienda que una misma persona sea responsable de su diseño, implementación, pruebas unitarias y evidencia de funcionamiento, respetando que cada historia pertenece a un solo estudiante.

| HU | Responsabilidad | Tecnología principal | Trabajo esperado |
| --- | --- | --- | --- |
| ADM1 | Login administrativo | C#/.NET Web + WS_AUTENTICACION1 | Pantalla de acceso, validación de credenciales, tipo administrador oculto y contraseña cifrada. |
| ADM2 | Plantilla y navegación administrativa | C#/.NET Web | Layout general, menú ADM3-ADM7, logo, pie de página, navegación persistente y cierre de sesión. |
| ADM3 | Poner nuevas líneas a disposición | C#/.NET Web + WS_PROVEEDOR / WS_PROVEEDOR1 | Listar líneas sin vender, borrar con confirmación, formulario de nueva línea, validaciones y alta del servicio. |
| ADM4 | Activar línea vendida | C#/.NET Web + WS_PROVEEDOR / WS_PROVEEDOR2 | Listar líneas disponibles, asociar cédula de cliente, validar y activar con estado Activo. |
| ADM5 | Desactivar/devolver línea desde administración | C#/.NET Web + WS_PROVEEDOR / WS_PROVEEDOR2 | Listar líneas en uso, confirmar desactivación y enviar estado inactivo. |

## 6. Detalle del bloque – Compañero 2
Este bloque contiene 5 historias y se recomienda que una misma persona sea responsable de su diseño, implementación, pruebas unitarias y evidencia de funcionamiento, respetando que cada historia pertenece a un solo estudiante.

| HU | Responsabilidad | Tecnología principal | Trabajo esperado |
| --- | --- | --- | --- |
| ADM6 | Cálculo de facturación | C#/.NET Web + WS_PROVEEDOR / WS_PROVEEDOR3 | Consultar última facturación, validar continuidad de fechas y ejecutar nuevo cálculo. |
| ADM7 | Mantenimiento de administradores | C#/.NET Web + WS_AUTENTICACIÓN | Listar, crear, editar, activar/inactivar y eliminar administradores; validar correo y contraseña de 14 caracteres. |
| CLIENTE1 | Login de clientes | Recomendado: C#/.NET Web + WS_AUTENTICACION1 | Login de cliente, cifrado de contraseña, control de credenciales y enlace al registro. |
| CLIENTE2 | Plantilla del portal de autogestión | Recomendado: C#/.NET Web | Menú CLIENTE4-CLIENTE7, saludo al cliente autenticado, navegación persistente, logo, footer y logout. |
| CLIENTE3 | Registro de cliente | Recomendado: C#/.NET Web + WS_AUTENTICACION2 | Alta de cliente con tipo 2, estado activo, validaciones de campos, correo y contraseña; retorno al login. |

## 7. Detalle del bloque – Compañero 3
Este bloque contiene 4 historias y se recomienda que una misma persona sea responsable de su diseño, implementación, pruebas unitarias y evidencia de funcionamiento, respetando que cada historia pertenece a un solo estudiante.

| HU | Responsabilidad | Tecnología principal | Trabajo esperado |
| --- | --- | --- | --- |
| CLIENTE4 | Consulta de líneas del cliente | Recomendado: C#/.NET Web + WS_PROVEEDOR | Mostrar prepago con saldo y postpago con facturación pendiente; navegación a recarga o pago. |
| CLIENTE5 | Recarga de saldo prepago | Recomendado: C#/.NET Web + WS_PROVEEDOR | Selección de línea, formulario de tarjeta, validaciones de vencimiento/CVV/monto y aumento de saldo. |
| CLIENTE6 | Pago de factura postpago | Recomendado: C#/.NET Web + WS_PROVEEDOR + correo | Pago total de factura, validación de tarjeta, monto no editable, cancelación de pendiente y envío de correo con detalle. |
| CLIENTE7 | Devolución de línea por cliente | Recomendado: C#/.NET Web + WS_PROVEEDOR / WS_PROVEEDOR2 | Consultar líneas, validar deuda, impedir devolución con factura pendiente y desactivar línea cuando corresponda. |

## 8. Funcionalidades de Web Services que deben revisarse o ampliarse
El alcance aclara que deben completarse los servicios Web cuando una consulta u operación requerida por las interfaces no exista. Por eso, antes de programar cada pantalla conviene hacer un inventario de endpoints disponibles y marcar cuáles faltan.

| Servicio | Necesidad en este alcance | Historias que dependen | Sugerencia técnica |
| --- | --- | --- | --- |
| WS_AUTENTICACION1 | Validar credenciales por tipo de usuario. | ADM1, CLIENTE1 | Reutilizar endpoint existente; verificar cifrado y discriminador de rol. |
| WS_AUTENTICACION2 | Crear/modificar usuarios y soportar cambios de estado/eliminación. | ADM7, CLIENTE3 | Ampliar si faltan operaciones CRUD o activar/inactivar. |
| WS_PROVEEDOR1 | Registrar nueva línea/servicio. | ADM3 | Reutilizar o ajustar contrato de alta. |
| WS_PROVEEDOR2 | Activar/desactivar líneas y asociar cliente. | ADM4, ADM5, CLIENTE7 | Reutilizar endpoint existente, validando estados y datos requeridos. |
| WS_PROVEEDOR3 | Ejecutar cálculo de facturación. | ADM6 | Reutilizar si está completo; asegurar manejo de rangos de fechas. |
| WS_PROVEEDOR – consultas nuevas | Listados de líneas, saldos, deuda, última facturación, eliminación de línea, recarga y pago. | ADM3-ADM6, CLIENTE4-CLIENTE7 | Agregar endpoints al WS actual o crear un WS auxiliar; nunca consultar BD/socket directamente desde la Web. |

## 9. Dependencias y orden recomendado de desarrollo
1. Congelar y documentar los contratos JSON de autenticación, líneas, facturación, recarga y pago.
2. Revisar qué operaciones ya existen en WS_AUTENTICACIÓN y WS_PROVEEDOR y cuáles deben agregarse.
3. Construir primero las plantillas y autenticación: ADM1-ADM2 y CLIENTE1-CLIENTE3.
4. Completar los endpoints de consulta que alimentan listados administrativos y de clientes.
5. Implementar las operaciones de líneas administrativas: ADM3-ADM5.
6. Implementar facturación y mantenimiento administrativo: ADM6-ADM7.
7. Implementar autogestión transaccional: CLIENTE4-CLIENTE7.
8. Integrar los tres bloques, ejecutar pruebas extremo a extremo y preparar evidencias para la evaluación.

## 10. Trabajo común del equipo
Estas actividades no sustituyen la responsabilidad individual sobre cada historia, pero deben coordinarse como equipo para que el sistema completo funcione:
- Actualizar diagramas de base de datos, casos de uso y clases.
- Crear el nuevo diagrama de componentes.
- Completar portada, introducción, conclusiones, recomendaciones y bibliografía.
- Mantener actualizados el Identificador, Proveedor, Simulador, WS Proveedor y WS Autenticación.
- Definir configuración compartida de endpoints, cifrado, modelos DTO y manejo uniforme de errores.
- Realizar pruebas integrales con todos los componentes conectados; el documento advierte penalización si se evalúan componentes aislados.
- Preparar simulaciones/mocks únicamente para componentes que no estén listos, cuando sea necesario demostrar una historia.

## 11. Matriz rápida de responsabilidad

| Historia | Bloque | Aplicación | Servicio principal |
| --- | --- | --- | --- |
| ADM1 | Compañero 1 | Web Administrativo C# | WS_AUTENTICACION1 |
| ADM2 | Compañero 1 | Web Administrativo C# | — |
| ADM3 | Compañero 1 | Web Administrativo C# | WS_PROVEEDOR / WS_PROVEEDOR1 |
| ADM4 | Compañero 1 | Web Administrativo C# | WS_PROVEEDOR / WS_PROVEEDOR2 |
| ADM5 | Compañero 1 | Web Administrativo C# | WS_PROVEEDOR / WS_PROVEEDOR2 |
| ADM6 | Compañero 2 | Web Administrativo C# | WS_PROVEEDOR / WS_PROVEEDOR3 |
| ADM7 | Compañero 2 | Web Administrativo C# | WS_AUTENTICACIÓN |
| CLIENTE1 | Compañero 2 | Web Cliente (recomendado C#) | WS_AUTENTICACION1 |
| CLIENTE2 | Compañero 2 | Web Cliente (recomendado C#) | — |
| CLIENTE3 | Compañero 2 | Web Cliente (recomendado C#) | WS_AUTENTICACION2 |
| CLIENTE4 | Compañero 3 | Web Cliente (recomendado C#) | WS_PROVEEDOR |
| CLIENTE5 | Compañero 3 | Web Cliente (recomendado C#) | WS_PROVEEDOR |
| CLIENTE6 | Compañero 3 | Web Cliente (recomendado C#) | WS_PROVEEDOR + correo |
| CLIENTE7 | Compañero 3 | Web Cliente (recomendado C#) | WS_PROVEEDOR / WS_PROVEEDOR2 |

## 12. Observaciones importantes del documento fuente
- El alcance se identifica como “Alcance Final - Sistemas Web”.
- La aplicación administrativa Web debe desarrollarse en C#.
- La aplicación Web Cliente puede desarrollarse en el lenguaje de preferencia del equipo o en C#.
- Ninguna aplicación Web puede acceder directamente a bases de datos o sockets; toda comunicación debe realizarse mediante servicios Web.
- Las historias no pueden compartirse entre estudiantes y la distribución debe ser equitativa.
- El PDF presenta una inconsistencia de fecha: en aspectos técnicos indica pruebas en clase el 19 de agosto, mientras la sección administrativa menciona 19/agosto/2025, aunque el documento está rotulado II Cuatrimestre 2026. Conviene confirmar la fecha oficial con el profesor/Teams.

Propuesta preparada para discusión interna del equipo antes de asignar nombres.
