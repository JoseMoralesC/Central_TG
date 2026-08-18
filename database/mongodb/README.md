# MongoDB - Autenticacion

MongoDB se usa para las historias de Charlie:

- WS_AUTENTICACION1
- WS_AUTENTICACION2

La base acordada es `central_tg_mongo` y la coleccion inicial es `usuarios`.

## Conexion remota por Tailscale

Usar el usuario asignado por integrante y escribir la contrasena solo en la terminal cuando `mongosh` la solicite.

```powershell
mongosh --host 100.114.84.5 --port 27017 -u jose --authenticationDatabase central_tg_mongo
mongosh --host 100.114.84.5 --port 27017 -u gabriel --authenticationDatabase central_tg_mongo
mongosh --host 100.114.84.5 --port 27017 -u charlie --authenticationDatabase central_tg_mongo
```

No versionar contrasenas reales en scripts.

## Orden sugerido

```javascript
use central_tg_mongo
load("database/mongodb/crear_coleccion_usuarios.js")
load("database/mongodb/indices_usuarios.js")
load("database/mongodb/datos_semilla_usuarios.example.js")
```

Para las pruebas Web de persona 2, usar el seed con valores AES reales:

```javascript
use central_tg_mongo
load("database/mongodb/crear_coleccion_usuarios.js")
load("database/mongodb/indices_usuarios.js")
load("database/mongodb/datos_semilla_persona_2.js")
```

Credenciales de prueba:

| Tipo | Usuario | Contrasena |
| --- | --- | --- |
| Administrador | `adminp2` | `AdminPersona2!` |
| Cliente | `clientep2` | `ClienteUser2!!` |

Estos valores estan cifrados con AES-CBC, PKCS7 y UTF-8 sin BOM, igual que `WS_Autenticacion` y las Web.

## Estructura esperada

- Base: `central_tg_mongo`
- Colección: `usuarios`
- Índices:
  - `ux_usuarios_identificacion`
  - `ux_usuarios_usuario_cifrado`
  - `ux_usuarios_correo`

## Notas importantes

- Los valores de `usuario` y `contrasena` deben ser cifrados por el servicio de autenticación antes de guardarse en datos reales.
- Los documentos semilla son ejemplos; al conectar la base real, se pueden reemplazar por los usuarios reales del sistema.
- El script de creación es idempotente: si la colección ya existe, no intenta recrearla.
