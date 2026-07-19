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

Los valores de `usuario` y `contrasena` deben ser cifrados por el servicio de autenticacion antes de guardarse en datos reales.
