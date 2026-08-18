# Ruta de demo - alcance final

Fecha de actualizacion: 2026-08-18

## Ruta oficial por bloque

| Bloque | Ruta oficial | Comentario |
|---|---|---|
| ADM1-ADM7 | `dotnet_webapps/WebAdministrativo` | Web administrativa principal. Incluye ADM3-ADM5, ADM6 y ADM7. |
| CLIENTE1-CLIENTE3 | `dotnet_webapps/WebCliente` | Login, plantilla base y registro de cliente. |
| CLIENTE4-CLIENTE7 | `dotnet_webservices/PortalCliente` | Portal transaccional requerido despues del login. Para demo se entra por `WebCliente`; `WebCliente` redirige aqui. |

## Preparacion

Desde la raiz del repositorio:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\scripts\persona2-preparar.ps1
.\scripts\persona2-levantar.ps1
```

El arranque levanta:

```txt
WS_Autenticacion:    http://localhost:59113/Service1.svc?wsdl
WS_Proveedor:        http://localhost:55254/ProveedorService.svc?wsdl
WS_ProveedorCliente: http://localhost:55260/ProveedorClienteService.svc?wsdl
WebAdministrativo:   http://localhost:56121/Login.aspx
WebCliente:          http://localhost:56122/Login.aspx
PortalCliente:       http://localhost:56123/Cliente/Index (destino despues del login)
```

## Flujo administrativo

1. Entrar a `http://localhost:56121/Login.aspx`.
2. Iniciar sesion con `adminp2 / AdminPersona2!`.
3. ADM3: abrir `Nuevas lineas`, crear una linea y validar que aparece como disponible.
4. ADM3: eliminar una linea disponible y confirmar el mensaje.
5. ADM4: abrir `Activar linea`, seleccionar una disponible, ingresar cedula y activar.
6. ADM5: abrir `Devolucion linea`, seleccionar una activa y desactivarla/devolverla.
7. ADM6: abrir `Calcular facturacion`, validar ultima facturacion y ejecutar el calculo.
8. ADM7: abrir `Administradores`, probar crear, editar, activar/inactivar y eliminar.

## Flujo cliente

1. Entrar a `http://localhost:56122/Login.aspx`.
2. Iniciar sesion con `clientep2 / ClienteUser2!!`.
3. Confirmar redireccion a `PortalCliente` en `http://localhost:56123` con la identificacion en sesion.
4. CLIENTE4: revisar `Mis lineas`, separadas en prepago y postpago.
5. CLIENTE5: cargar saldo a una linea prepago.
6. CLIENTE6: pagar una factura postpago pendiente.
7. CLIENTE7: devolver una linea prepago o una postpago sin deuda.

## SMTP para CLIENTE6

`PortalCliente` intenta enviar correo usando la seccion `Smtp` de:

```txt
dotnet_webservices/PortalCliente/appsettings.json
```

Para evidencia real de correo se deben configurar:

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "From": "cuenta@dominio.com",
    "User": "cuenta@dominio.com",
    "Password": "clave-o-app-password",
    "EnableSsl": true
  }
}
```

Si no se configuran credenciales, el pago queda registrado y la pantalla muestra
que no se pudo enviar el correo por falta de configuracion SMTP.

## Diagnostico rapido

- `http://localhost:5000` pertenece al Identificador Python por socket TCP. No
  abrirlo en navegador como si fuera una pagina Web.
- `ERR_INVALID_HTTP_RESPONSE` en `localhost:5000` indica que el navegador esta
  intentando hablar HTTP contra el socket del Identificador.
- Si el Identificador imprime muchas lineas de `Conexion rechazada por el
  proveedor Java`, revisar la ventana de Java y confirmar que escucha en
  `127.0.0.1:6000`.
- El proveedor Java usa `java_proveedor/src/config/config.properties`; para la
  demo local debe apuntar a SQL Server en `localhost:49172` y mantener
  `socket.puerto=6000`.
- Si el simulador muestra solo 4 telefonos, esta usando el catalogo seed local;
  reiniciar Identificador y Simulador para forzar la consulta real a MySQL.
- Si `Consultar Mongo` muestra `ECONNREFUSED 127.0.0.1:27017`, MongoDB no esta
  escuchando. Levantar MongoDB manualmente como administrador antes de ejecutar
  la ruta; `persona2-levantar.ps1` no crea ni siembra una instancia temporal.

## Evidencias minimas

| Historia | Evidencia |
|---|---|
| ADM1 | Login exitoso y login fallido. |
| ADM2 | Menu completo ADM3-ADM7, footer y salida del sitio. |
| ADM3 | Lista de disponibles, alta de linea, eliminacion con confirmacion. |
| ADM4 | Lista de disponibles, cedula del cliente, activacion exitosa. |
| ADM5 | Lista de activas, confirmacion y devolucion/desactivacion exitosa. |
| ADM6 | Ultima facturacion, caso exitoso y caso con error de fechas. |
| ADM7 | Crear, editar, activar/inactivar y eliminar administrador. |
| CLIENTE1 | Login cliente exitoso y fallido. |
| CLIENTE2 | Menu cliente, saludo/sesion y salida. |
| CLIENTE3 | Registro exitoso y validacion de contrasena/correo. |
| CLIENTE4 | Listado prepago/postpago con saldo/factura. |
| CLIENTE5 | Recarga exitosa y validaciones de tarjeta/monto. |
| CLIENTE6 | Pago exitoso, monto no editable y evidencia de correo o mensaje SMTP. |
| CLIENTE7 | Bloqueo por deuda y devolucion exitosa sin deuda. |
