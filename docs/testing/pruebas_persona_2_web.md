# Pruebas persona 2 - Alcance final Web

## Objetivo

Validar las historias asignadas a persona 2:

- `ADM6`: Calcular facturacion.
- `ADM7`: Mantenimiento de administradores.
- `CLIENTE1`: Login cliente.
- `CLIENTE2`: Plantilla portal cliente.
- `CLIENTE3`: Registro cliente.

Guia paso a paso de ejecucion:

```txt
docs/roadmaps/guia_ejecucion_persona_2_web.md
```

## Estado del codigo

El codigo base ya existe y compila:

- `dotnet_webservices/WS_Autenticacion/WS_Autenticacion.sln`
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj`
- `dotnet_webapps/CentralTelefonica.WebApps.sln`

Verificacion local realizada el 2026-08-17:

- `dotnet_webapps/CentralTelefonica.WebApps.sln`: compilacion correcta con MSBuild de Visual Studio, 0 errores y 0 advertencias.
- `dotnet_webservices/WS_Autenticacion/WS_Autenticacion.sln`: compilacion correcta con MSBuild de Visual Studio, 0 errores y 0 advertencias.
- `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj`: compilacion correcta con MSBuild de Visual Studio, 0 errores y 0 advertencias.
- `dotnet build` no es suficiente para las Web Forms porque no encuentra `Microsoft.WebApplication.targets`; usar MSBuild de Visual Studio.

Comando usado:

```txt
C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe
```

Tambien se ajusto el flujo Web:

- Las paginas protegidas de cliente y administracion redirigen al login y cortan ejecucion si no hay sesion.
- Los login limpian sesion al entrar por primera vez.
- En `ADM7`, los mensajes de crear/editar/activar/inactivar/eliminar son visibles aunque el formulario este cerrado.
- En `ADM7`, editar un administrador permite conservar la contrasena actual si el campo se deja vacio.
- En `CLIENTE2`, las tablas de lineas muestran mensaje cuando aun no hay datos conectados por persona 3.

## Orden de arranque recomendado

1. Activar MongoDB para `WS_Autenticacion`.
2. Activar SQL Server para `WS_Proveedor`.
3. Levantar Java Proveedor en `127.0.0.1:6000`.
4. Ejecutar `WS_Autenticacion` en Visual Studio.
5. Ejecutar `WS_Proveedor` en Visual Studio.
6. Ejecutar `dotnet_webapps/CentralTelefonica.WebApps.sln`.

## Datos de prueba para autenticacion

Si la base MongoDB no tiene usuarios validos, ejecutar:

```javascript
use central_tg_mongo
load("database/mongodb/crear_coleccion_usuarios.js")
load("database/mongodb/indices_usuarios.js")
load("database/mongodb/datos_semilla_persona_2.js")
```

Credenciales:

| Flujo | Usuario | Contrasena |
| --- | --- | --- |
| Login administrativo | `adminp2` | `AdminPersona2!` |
| Login cliente | `clientep2` | `ClienteUser2!!` |

## Datos de prueba para ADM6

Para que `Facturacion.aspx` cargue una ultima facturacion y permita probar continuidad de fechas, aplicar despues del esquema base:

```txt
database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql
database/sqlserver_proveedor/migrations/012_seed_facturacion_persona2.sql
database/sqlserver_proveedor/migrations/013_normalizar_estado_linea_disponible.sql
```

El seed deja una ultima facturacion calculada para:

```txt
Fecha calculo: 2026-08-16
Fecha maxima pago: 2026-08-31
```

Caso exitoso sugerido desde la pantalla `ADM6`:

```txt
Fecha de calculo: 2026-08-17
Fecha maxima de pago: 2026-09-01
```

Caso de error sugerido:

```txt
Fecha de calculo: 2026-08-18
Fecha maxima de pago: 2026-09-01
```

Debe fallar porque deja una brecha respecto a la ultima facturacion del `2026-08-16`.

## URLs esperadas

Autenticacion:

```txt
http://localhost:59113/Service1.svc
http://localhost:59113/Service1.svc?wsdl
```

Proveedor:

```txt
http://localhost:55254/ProveedorService.svc
http://localhost:55254/ProveedorService.svc?wsdl
```

Webs:

```txt
WebAdministrativo/Login.aspx
WebCliente/Login.aspx
```

Si IIS Express asigna otros puertos, actualizar:

```txt
dotnet_webapps/WebAdministrativo/Web.config
dotnet_webapps/WebCliente/Web.config
```

## Casos de prueba minimos

### CLIENTE3 - Registro cliente

Resultado esperado:

```txt
Registro exitoso
```

Probar errores:

- Correo invalido.
- Contrasena distinta de 14 caracteres.
- Contrasena sin mayuscula, minuscula, numero o caracter especial.
- Usuario duplicado.

### CLIENTE1 - Login cliente

Resultado esperado:

- Redireccion a `Lineas.aspx`.
- Saludo disponible en el portal.

Error esperado:

```txt
Usuario y/o contrasena incorrectos
```

### CLIENTE2 - Plantilla cliente

Validar:

- Menu siempre visible.
- Saludo `Hola Nombre`.
- Footer visible.
- Logout vuelve al login.
- Acceso sin sesion vuelve al login.

### ADM7 - Mantenimiento administradores

Validar:

- Login administrativo.
- Listado de administradores tipo `1`.
- Crear administrador.
- Editar administrador.
- Activar/inactivar administrador.
- Eliminar administrador con confirmacion.

Mensajes esperados:

```txt
Registro exitoso
Borrado exitoso
Error al realizar el proceso
```

### ADM6 - Calculo facturacion

Validar:

- Carga de ultima facturacion.
- Rechazo de fecha maxima anterior a fecha calculo.
- Rechazo de fecha no continua respecto a la ultima facturacion.
- Ejecucion correcta de calculo.

Resultado esperado:

```txt
Proceso finalizado de forma exitosa
```

## Errores comunes

| Error | Causa probable | Solucion |
| --- | --- | --- |
| `403.14 Forbidden` en `localhost:59113` | Se abrio la raiz del WCF. | Abrir `http://localhost:59113/Service1.svc?wsdl`. |
| `EndpointNotFoundException` | Puerto/URL del WS no coincide. | Actualizar `WsAutenticacionUrl` o `WsProveedorUrl`. |
| Login falla con datos correctos | AES, tipo o estado incorrecto. | Revisar llaves AES, tipo `1`/`2` y estado `activo`. |
| Error al consultar ultima facturacion | SQL Server o tabla no disponible. | Revisar `SqlServerProveedor` y migracion `010_proveedor6_facturacion.sql`. |
| Error al calcular facturacion | Java proveedor no responde. | Levantar Java en `127.0.0.1:6000`. |

## Evidencias sugeridas

Guardar capturas en:

```txt
docs/evidencias/persona_2/
```

Capturas minimas:

- Registro cliente exitoso.
- Login cliente exitoso.
- Portal cliente con saludo.
- Registro administrador exitoso.
- Lista administradores.
- Activar/inactivar administrador.
- Eliminacion administrador.
- Carga de ultima facturacion.
- Calculo facturacion exitoso.
- Un error de validacion por historia.
