# Roadmap persona 2 - Alcance final Web

## Proposito

Este documento traduce el alcance oficial del PDF `docs/Proyecto/Proyecto final.pdf` y la estrategia del equipo `docs/estrategia/estrategia_3.md` en un plan de desarrollo concreto para la persona 2.

La persona 2 queda responsable de cinco historias:

| Historia | Aplicacion | Enfoque |
| --- | --- | --- |
| ADM6 | Web Administrativo C# | Calcular facturacion |
| ADM7 | Web Administrativo C# | Mantenimiento de usuarios administradores |
| CLIENTE1 | Web Cliente | Login de clientes |
| CLIENTE2 | Web Cliente | Plantilla del portal cliente |
| CLIENTE3 | Web Cliente | Registro de clientes |

## Reglas oficiales que condicionan el desarrollo

- La Web Administrativa debe desarrollarse en C#.
- La Web Cliente puede desarrollarse en C# u otro lenguaje, pero se recomienda C# para reducir riesgos de integracion.
- Las aplicaciones Web no pueden conectarse directamente a bases de datos ni a sockets.
- Toda operacion desde las Web debe pasar por servicios Web.
- Si falta una consulta u operacion en los WS actuales, se puede ampliar el servicio existente o crear un WS nuevo.
- Cada historia debe quedar desarrollada por un unico estudiante.
- La evaluacion puede ser por historias, por lo que cada HU debe tener evidencia propia.
- El PDF indica pruebas en clase para el 19 de agosto durante semana de examenes finales. Tambien contiene una fecha administrativa inconsistente: 19/agosto/2025, aunque el documento pertenece al II Cuatrimestre 2026. Conviene confirmar la fecha final oficial con el profesor.

## Mapeo general del desarrollo

El alcance final agrega dos capas Web encima del sistema ya construido:

```txt
Web Administrativo C#
Web Cliente C# recomendado
        |
        v
Servicios Web
        |
        +-- WS_AUTENTICACION: login, crear/modificar usuarios, estado, eliminacion si aplica
        |
        +-- WS_PROVEEDOR: facturacion y consultas/operaciones de lineas
                |
                v
        Java Proveedor + SQL Server
```

Para persona 2, el trabajo se divide en tres bloques:

1. Facturacion administrativa: `ADM6`.
2. Gestion de usuarios administrativos: `ADM7`.
3. Base del portal cliente: `CLIENTE1`, `CLIENTE2`, `CLIENTE3`.

La ruta critica esta en los servicios Web. Antes de construir pantallas definitivas hay que verificar si las operaciones necesarias estan expuestas para consumo Web. Si existen internamente en Java, MongoDB o SQL Server pero no estan publicadas como WS, siguen contando como faltantes para el alcance final.

## Inventario inicial detectado en el repositorio

| Area | Estado observado | Implicacion para persona 2 |
| --- | --- | --- |
| Estrategia de reparto | `docs/estrategia/estrategia_3.md` asigna a persona 2 `ADM6`, `ADM7`, `CLIENTE1`, `CLIENTE2`, `CLIENTE3`. | Usar esta division como base oficial interna. |
| Contrato de comunicacion interno | `docs/arquitectura/contrato_json_integrado.md` define JSON sobre sockets para C#, Python y Java. | No cambiar contratos internos sin avisar al equipo. |
| Facturacion Java | Existen `java_proveedor/src/services/Proveedor6Service.java`, `FacturacionDAO.java` y `shared/contracts/proveedor6_facturacion.json`. | `ADM6` puede apoyarse en esta funcionalidad, pero necesita exposicion via WS para la Web. |
| Stored procedure facturacion | Existe `database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql`. | Validar que este aplicada en SQL Server y que genera facturas correctamente. |
| WS Autenticacion | Existe `dotnet_webservices/WS_Autenticacion` con login, crear, modificar y cambiar estado. | `ADM7`, `CLIENTE1` y `CLIENTE3` deben consumirlo o ajustar lo faltante. |
| Eliminacion de usuario | En la interfaz `IAutenticacionService.cs` no se observa una operacion explicita de borrar usuario. | `ADM7` requiere eliminar; hay que agregarla o acordar si "eliminar" sera baja logica. |
| Listado de administradores | El repositorio tiene repositorio Mongo con `ObtenerTodos`, pero la interfaz WS revisada no expone listado. | `ADM7` requiere listar administradores; hay que exponer operacion filtrada por tipo 1. |
| Plantillas Web | No se observa una aplicacion Web final administrativa/cliente claramente creada para el alcance final. | Persona 2 debe acordar estructura con persona 1 y 3 para no duplicar layouts. |

## Roadmap recomendado

### Fase 0 - Alinear decisiones base

Objetivo: evitar que persona 2 programe contra contratos equivocados.

Tareas:

- Confirmar con el equipo si la Web Cliente tambien se hara en C#.
- Definir si seran dos proyectos Web separados o un solo proyecto con areas/rutas para admin y cliente.
- Definir URL base y tecnologia de consumo de WS: SOAP/WCF, Minimal API puente, REST wrapper o cliente generado.
- Confirmar el cifrado usado por las Web para contrasenas: mismo AES requerido por `WS_AUTENTICACION`.
- Definir mensajes comunes para exito/error segun el PDF.

Salida esperada:

- Decision escrita en un documento o README tecnico.
- Endpoints/servicios identificados para cada historia.
- Credenciales y configuracion de entorno listas sin subir `.env`.

### Fase 1 - Completar contratos WS necesarios para persona 2

Objetivo: dejar disponibles las operaciones que las pantallas Web consumiran.

#### ADM6 - Facturacion

Operaciones requeridas:

- Consultar ultima facturacion generada exitosamente.
- Ejecutar nuevo calculo de facturacion usando `WS_PROVEEDOR3`.

Trabajo tecnico:

- Verificar si `WS_PROVEEDOR3` ya expone `CalcularFacturacion`.
- Exponer una operacion tipo `ObtenerUltimaFacturacion`.
- Validar que el WS no dependa de acceso directo desde la Web a SQL Server.
- Confirmar formato de fechas esperado: recomendado `yyyy-MM-dd`.
- Confirmar que la fecha maxima de pago no sea anterior a la fecha de calculo.

#### ADM7 - Administradores

Operaciones requeridas:

- Listar usuarios administradores tipo `1`.
- Crear administrador con estado `activo`.
- Modificar datos personales y credenciales.
- Activar/inactivar administrador.
- Eliminar administrador definitivamente o por baja logica, segun decida el equipo.

Trabajo tecnico:

- Agregar o exponer `ListarUsuariosPorTipo(1)`.
- Agregar o exponer `EliminarUsuario(identificacion)` si no existe.
- Verificar que `CrearUsuario`, `ModificarUsuario` y `CambiarEstadoUsuario` respeten los datos del PDF.
- Asegurar que la contrasena viaje cifrada desde la Web.
- Evitar mostrar campos internos como `tipo` y `estado` en formularios cuando el PDF indica que deben ser ocultos.

#### CLIENTE1 y CLIENTE3 - Autenticacion cliente

Operaciones requeridas:

- Login con tipo usuario `2`.
- Crear cliente con tipo `2` y estado `activo`.

Trabajo tecnico:

- Reutilizar `WS_AUTENTICACION1` para login.
- Reutilizar `WS_AUTENTICACION2` para registro.
- Confirmar que el login retorna suficientes datos del cliente para `CLIENTE2`: al menos nombre e identificacion.
- Si el WS actual solo retorna `ResultadoOperacion`, ampliar respuesta o agregar consulta de perfil por usuario autenticado.

Salida esperada de la fase:

- Documento corto con endpoints finales.
- Pruebas manuales de cada operacion WS antes de conectarla a pantalla.
- Evidencia de respuestas exitosas y fallidas.

### Fase 2 - Implementar ADM6: pantalla de calculo de facturacion

Objetivo: cumplir la HU ADM6 de punta a punta.

Pantalla requerida:

- Mostrar al cargar la ultima facturacion generada exitosamente.
- Permitir ingresar nueva informacion para ejecutar el proceso.
- Validar continuidad de fechas.
- Ejecutar calculo mediante `WS_PROVEEDOR3`.
- Mostrar `Proceso finalizado de forma exitosa` o `Error al realizar el proceso`.

Validaciones clave:

- La fecha inicial del nuevo calculo no puede iniciar antes de la fecha de la ultima ejecucion.
- No puede dejar dias sin calcular entre la ultima ejecucion y el nuevo inicio.
- La fecha maxima de pago debe ser valida y coherente.
- No permitir ejecucion si hay datos vacios o formato incorrecto.

Riesgo principal:

- El PDF pide consultar ultima facturacion con `WS_PROVEEDOR` nuevo; si solo existe calculo y no consulta, ADM6 queda incompleta aunque el stored procedure funcione.

Evidencias:

- Carga inicial con ultima facturacion.
- Validacion por fecha anterior.
- Validacion por brecha de dias.
- Calculo exitoso.
- Error controlado del WS.

### Fase 3 - Implementar ADM7: mantenimiento de administradores

Objetivo: cumplir CRUD administrativo completo.

Pantalla principal:

- Lista de administradores con identificacion, nombre, primer apellido, segundo apellido, correo, usuario, contrasena y estado.
- Boton `Nuevo`.
- Boton por fila para activar/inactivar.
- Boton por fila para eliminar.

Pantalla/formulario de nuevo:

- Identificacion.
- Nombre.
- Primer apellido.
- Segundo apellido.
- Correo electronico.
- Usuario.
- Contrasena.
- Estado oculto: `activo`.
- Tipo oculto: `1`.

Pantalla/formulario de edicion:

- Cargar datos seleccionados.
- Validar campos.
- Enviar modificacion a `WS_AUTENTICACION2`.
- Volver a lista si es exitoso.
- Mantener formulario y mostrar error si falla.

Validaciones:

- Ningun campo obligatorio puede quedar nulo.
- Correo con formato valido.
- Contrasena exactamente de 14 caracteres.
- Contrasena con al menos un caracter especial, una mayuscula, una minuscula y un numero.
- Usuario no vacio.
- Identificacion valida segun reglas del servicio.

Acciones por fila:

- Si usuario esta activo, mostrar `inactivar`.
- Si usuario esta inactivo, mostrar `activar`.
- Al cambiar estado, refrescar lista y etiqueta del boton.
- Para borrar, confirmar con: `Esta seguro de eliminar el registro del usuario de forma definitiva?`
- Si borra correctamente, mostrar `Borrado exitoso`.

Riesgos principales:

- No existe operacion publica de listado en la interfaz WS revisada.
- No existe operacion publica de eliminacion en la interfaz WS revisada.
- El PDF pide mostrar contrasena en lista, pero por seguridad puede estar cifrada o no ser recuperable en claro. Acordar con el profesor/equipo si se muestra el valor cifrado, un marcador, o se omite por seguridad.

Evidencias:

- Lista inicial de administradores.
- Registro exitoso.
- Validaciones de correo y contrasena.
- Edicion exitosa.
- Activar e inactivar.
- Eliminacion con confirmacion.

### Fase 4 - Implementar CLIENTE1: login cliente

Objetivo: permitir ingreso al portal de clientes.

Pantalla requerida:

- Usuario.
- Contrasena.
- Boton de ingreso.
- Opcion para registrarse que navega a `CLIENTE3`.

Flujo:

- Cifrar contrasena antes de enviarla.
- Enviar tipo usuario oculto `2`.
- Consumir `WS_AUTENTICACION1`.
- Si credenciales son correctas, navegar a la plantilla cliente `CLIENTE2`.
- Si son incorrectas, mostrar `Usuario y/o contrasena incorrectos`.

Validaciones:

- Usuario requerido.
- Contrasena requerida.
- Manejo de error de servicio no disponible.

Evidencias:

- Login exitoso de cliente activo.
- Login fallido por credenciales incorrectas.
- Login fallido si el usuario existe pero no es cliente.
- Navegacion a registro.

### Fase 5 - Implementar CLIENTE2: plantilla portal cliente

Objetivo: crear la base navegable del portal de autogestion.

Elementos requeridos:

- Menu siempre disponible.
- Opcion `Mostrar lineas activas a su nombre` hacia `CLIENTE4`.
- Opcion `Cargar saldo a linea prepago` hacia `CLIENTE5`.
- Opcion `Pagar facturas pendientes de pago` hacia `CLIENTE6`.
- Opcion `Devolver una linea` hacia `CLIENTE7`.
- Opcion `Salir del Sitio`.
- Saludo superior derecho: `Hola ` + nombre del cliente.
- Icono/logo de la empresa.
- Footer visible en todas las paginas.
- Al ingresar al sitio, mostrar `CLIENTE4`.

Trabajo coordinado:

- Persona 2 construye layout, sesion, menu y rutas base.
- Persona 3 conecta `CLIENTE4`, `CLIENTE5`, `CLIENTE6` y `CLIENTE7` dentro del layout.
- Definir un modelo de sesion compartido: identificacion, nombre, tipo usuario, usuario.

Riesgo principal:

- Si `CLIENTE1` no obtiene nombre/identificacion del cliente, `CLIENTE2` no puede mostrar saludo ni filtrar operaciones de persona 3. Esto debe resolverse en WS o mediante una consulta de perfil.

Evidencias:

- Menu persistente.
- Saludo con nombre real.
- Logout limpia sesion y vuelve al login.
- Entrada inicial redirige/muestra `CLIENTE4`.

### Fase 6 - Implementar CLIENTE3: registro de cliente

Objetivo: permitir alta de clientes para autogestion.

Pantalla requerida:

- Identificacion del cliente.
- Nombre.
- Primer apellido.
- Segundo apellido.
- Correo electronico.
- Usuario.
- Contrasena.
- Opcion para volver al login.

Valores ocultos:

- Estado: `activo`.
- Tipo usuario: `2`.

Validaciones:

- Ningun campo obligatorio puede quedar nulo.
- Correo valido.
- Contrasena exactamente de 14 caracteres.
- Contrasena con al menos un caracter especial, una mayuscula, una minuscula y un numero.
- Usuario requerido.
- Identificacion valida.

Flujo:

- Cifrar contrasena antes de enviarla.
- Consumir `WS_AUTENTICACION2`.
- Mostrar `Registro exitoso` si el WS responde bien.
- Mostrar el error recibido si falla.
- Permitir volver al login.

Evidencias:

- Registro exitoso de cliente.
- Registro fallido por correo invalido.
- Registro fallido por contrasena invalida.
- Registro duplicado.
- Navegacion al login.

## Orden practico de trabajo

1. Levantar y probar `WS_AUTENTICACION`.
2. Confirmar/agregar listado y eliminacion de administradores.
3. Confirmar/agregar respuesta de login con datos basicos del usuario.
4. Levantar y probar `WS_PROVEEDOR` para facturacion.
5. Confirmar/agregar consulta de ultima facturacion.
6. Crear estructura Web administrativa y conectar `ADM6`.
7. Crear estructura Web administrativa y conectar `ADM7`.
8. Crear estructura Web cliente y conectar `CLIENTE1`.
9. Crear layout/sesion/menu cliente de `CLIENTE2`.
10. Crear registro `CLIENTE3`.
11. Integrar con persona 1 y persona 3.
12. Preparar evidencias individuales por historia.

## Checklist de definicion de terminado

Una historia de persona 2 se considera terminada cuando cumple todo esto:

- La pantalla existe y es navegable desde el menu correspondiente.
- No usa base de datos ni sockets directamente.
- Consume el WS indicado o un WS nuevo documentado.
- Valida datos antes de enviar al servicio.
- Maneja respuesta exitosa y respuesta fallida.
- Muestra los mensajes solicitados por el PDF.
- Limpia o conserva datos segun corresponda al flujo.
- Tiene al menos una evidencia de exito y una de error.
- Se probo integrada con los servicios reales o con simulacion justificada si un componente externo no esta listo.

## Pruebas minimas recomendadas

| Historia | Caso exitoso | Caso de error obligatorio |
| --- | --- | --- |
| ADM6 | Calcular facturacion con fechas continuas. | Rechazar fecha anterior o con brecha respecto a ultima facturacion. |
| ADM7 | Crear, editar, inactivar, activar y borrar administrador. | Rechazar correo invalido, contrasena invalida o usuario duplicado. |
| CLIENTE1 | Login de cliente activo tipo 2. | Rechazar credenciales incorrectas o tipo incorrecto. |
| CLIENTE2 | Navegar entre opciones y cerrar sesion. | Intentar acceder sin sesion y volver al login. |
| CLIENTE3 | Registrar cliente tipo 2 activo. | Rechazar datos vacios, correo invalido, contrasena invalida o duplicados. |

## Dependencias con los otros integrantes

| Integrante | Dependencia | Accion requerida |
| --- | --- | --- |
| Persona 1 | Plantilla administrativa general `ADM2`. | Acordar si persona 2 implementa solo vistas `ADM6`/`ADM7` dentro del layout existente o deja rutas preparadas. |
| Persona 1 | Operaciones administrativas de lineas. | Mantener estilo visual, mensajes y manejo de sesion coherente. |
| Persona 3 | Pantallas `CLIENTE4` a `CLIENTE7`. | Persona 2 debe dejar layout cliente, sesion y menu listos para conectar estas pantallas. |
| Equipo backend | WS de autenticacion y proveedor. | Publicar operaciones faltantes y congelar contratos antes de cerrar UI. |

## Riesgos y mitigaciones

| Riesgo | Impacto | Mitigacion |
| --- | --- | --- |
| Faltan operaciones Web para listar/eliminar usuarios. | `ADM7` no cumple criterios oficiales. | Agregar operaciones a `WS_AUTENTICACION2` o documentar baja logica aprobada. |
| Login solo retorna booleano. | `CLIENTE2` no puede mostrar `Hola Nombre` ni pasar identificacion a persona 3. | Ampliar respuesta de login con identificacion, nombre, correo, tipo y estado. |
| No existe consulta de ultima facturacion. | `ADM6` carga incompleta. | Agregar endpoint en `WS_PROVEEDOR` que lea ultima ejecucion exitosa. |
| Web accede directo a SQL Server/Mongo/socket por urgencia. | Incumple regla oficial y expone penalizacion. | Crear wrapper WS aunque sea minimo. |
| Diferencia entre contratos JSON internos y roadmap antiguo de texto plano. | Integracion rota entre componentes. | Mantener JSON como contrato vigente segun `docs/arquitectura/contrato_json_integrado.md`. |
| Fecha oficial del PDF inconsistente. | Riesgo de entrega tarde. | Confirmar con profesor/Teams y planificar contra el 19 de agosto del ciclo 2026. |

## Entregables de persona 2

- Pantalla `ADM6` funcional y evidencia.
- Pantalla `ADM7` funcional y evidencia.
- Login cliente `CLIENTE1` funcional y evidencia.
- Layout cliente `CLIENTE2` funcional y evidencia.
- Registro cliente `CLIENTE3` funcional y evidencia.
- Ajustes o contratos WS documentados para autenticacion y facturacion.
- Casos de prueba agregados o actualizados en `docs/testing/`.
- Capturas en `docs/evidencias/` con subcarpeta de la persona 2.

## Cierre recomendado

Antes de la presentacion, ejecutar una prueba integrada minima:

1. Iniciar servicios Web necesarios.
2. Registrar un cliente desde `CLIENTE3`.
3. Iniciar sesion con ese cliente desde `CLIENTE1`.
4. Confirmar que `CLIENTE2` muestra `Hola Nombre`.
5. Crear un administrador desde `ADM7`.
6. Inactivar y reactivar ese administrador.
7. Ejecutar una facturacion valida desde `ADM6`.
8. Verificar que no hubo acceso directo desde Web a bases de datos ni sockets.

Si esos ocho pasos pasan, el bloque de persona 2 queda bien parado para evaluacion individual o grupal.
