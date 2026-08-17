# Guia de ejecucion persona 2 - Web final

## Objetivo

Levantar los componentes necesarios para probar:

- `ADM6`: calcular facturacion.
- `ADM7`: mantenimiento de administradores.
- `CLIENTE1`: login cliente.
- `CLIENTE2`: plantilla portal cliente.
- `CLIENTE3`: registro cliente.

## 1. Preparar MongoDB

MongoDB debe estar activo en:

```txt
localhost:27017
```

Aplicar coleccion, indices y datos de prueba:

```powershell
mongosh --host localhost --port 27017 -u charlie -p charlie1234 --authenticationDatabase central_tg_mongo --eval "load('database/mongodb/crear_coleccion_usuarios.js'); load('database/mongodb/indices_usuarios.js'); load('database/mongodb/datos_semilla_persona_2.js');"
```

Credenciales Web generadas por el seed:

| Tipo | Usuario | Contrasena |
| --- | --- | --- |
| Administrador | `adminp2` | `AdminPersona2!` |
| Cliente | `clientep2` | `ClienteUser2!!` |

## Ejecucion rapida con scripts

Desde PowerShell, en la raiz del repositorio:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\scripts\persona2-preparar.ps1
.\scripts\persona2-levantar.ps1
```

El primer script compila WebApps, servicios WCF, simulador C# y Java. Por defecto no aplica seeds ni migraciones.

El segundo script levanta MongoDB, Java proveedor, Python identificador, simulador C# y cuatro ventanas de IIS Express:

```txt
Java Proveedor:     puerto 6000
Python Identificador: python_identificador/main.py
Simulador C#:       csharp_simulador/SimuladorTelefonico
WS_Autenticacion:   http://localhost:59113/Service1.svc?wsdl
WS_Proveedor:       http://localhost:55254/ProveedorService.svc?wsdl
WebAdministrativo:  http://localhost:56121/Login.aspx
WebCliente:         http://localhost:56122/Login.aspx
```

El arranque espera a que Java responda en el puerto `6000` antes de iniciar Python, y espera a que Python responda en el puerto `5000` antes de iniciar el simulador C#.

Para omitir partes:

```powershell
.\scripts\persona2-preparar.ps1 -SkipBuild
.\scripts\persona2-levantar.ps1 -SkipJava
.\scripts\persona2-levantar.ps1 -SkipPython -SkipSimuladorCSharp
```

Para aplicar seeds o migraciones solo cuando haga falta:

```powershell
.\scripts\persona2-preparar.ps1 -ApplyMongoSeed
.\scripts\persona2-preparar.ps1 -ApplySqlMigrations
.\scripts\persona2-preparar.ps1 -ApplyMongoSeed -ApplySqlMigrations
```

## 2. Preparar SQL Server proveedor

SQL Server debe estar activo en:

```txt
localhost,49172
```

Base esperada:

```txt
CentralProveedor
```

Aplicar facturacion y datos de prueba:

```powershell
sqlcmd -S localhost,49172 -d CentralProveedor -E -i database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql
sqlcmd -S localhost,49172 -d CentralProveedor -E -i database/sqlserver_proveedor/migrations/012_seed_facturacion_persona2.sql
```

El ambiente del repo usa usuario SQL para Java y `WS_Proveedor`:

```powershell
sqlcmd -S localhost,49172 -d CentralProveedor -U charlie_dev -P Charlie1234 -i database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql
sqlcmd -S localhost,49172 -d CentralProveedor -U charlie_dev -P Charlie1234 -i database/sqlserver_proveedor/migrations/012_seed_facturacion_persona2.sql
sqlcmd -S localhost,49172 -d CentralProveedor -U charlie_dev -P Charlie1234 -i database/sqlserver_proveedor/migrations/013_normalizar_estado_linea_disponible.sql
```

La conexion configurada en `WS_Proveedor/Web.config` debe coincidir con:

```txt
Server=localhost,49172;Database=CentralProveedor;User Id=charlie_dev;Password=Charlie1234;TrustServerCertificate=True;
```

Regla oficial para lineas telefonicas:

```txt
ACTIVO
DISPONIBLE
```

No usar `INACTIVO` como estado de linea. Ese estado queda reservado solo para usuarios administrativos/clientes cuando aplique en autenticacion.

La ultima facturacion inicial queda en:

```txt
Fecha calculo: 2026-08-16
Fecha maxima pago: 2026-08-31
```

## 3. Compilar Java proveedor

Desde la raiz del repositorio:

```powershell
New-Item -ItemType Directory -Force -Path .tmp/java_proveedor_classes
javac -encoding UTF-8 -d .tmp/java_proveedor_classes (Get-ChildItem -Recurse -Filter *.java java_proveedor | ForEach-Object { $_.FullName })
```

El driver SQL Server se usa al ejecutar, no es necesario para compilar.

## 4. Ejecutar Java proveedor

Desde la raiz del repositorio:

```powershell
java -cp ".tmp/java_proveedor_classes;java_proveedor/lib/mssql-jdbc.jar" java_proveedor.Main
```

Debe mostrar:

```txt
[Server] OK - Escuchando en el puerto: 6000
```

## 5. Levantar WS_Autenticacion

Abrir en Visual Studio:

```txt
dotnet_webservices/WS_Autenticacion/WS_Autenticacion.sln
```

Ejecutar con IIS Express.

URL esperada:

```txt
http://localhost:59113/Service1.svc?wsdl
```

## 6. Levantar WS_Proveedor

Abrir en Visual Studio:

```txt
dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj
```

Ejecutar con IIS Express.

URL esperada:

```txt
http://localhost:55254/ProveedorService.svc?wsdl
```

## 7. Levantar Webs persona 2

Abrir en Visual Studio:

```txt
dotnet_webapps/CentralTelefonica.WebApps.sln
```

Si los proyectos aparecen descargados, borrar:

```txt
dotnet_webapps/.vs
```

Y volver a abrir el `.sln`.

Proyectos:

```txt
WebAdministrativo
WebCliente
```

## 8. Pruebas recomendadas

### CLIENTE1 y CLIENTE2

Entrar a:

```txt
WebCliente/Login.aspx
```

Credenciales:

```txt
Usuario: clientep2
Contrasena: ClienteUser2!!
```

Debe entrar a:

```txt
Lineas.aspx
```

Y mostrar:

```txt
Hola Cliente
```

### CLIENTE3

Entrar a:

```txt
WebCliente/Registro.aspx
```

Registrar un cliente nuevo con contrasena de 14 caracteres, por ejemplo:

```txt
ClienteNuevo2!
```

### ADM7

Entrar a:

```txt
WebAdministrativo/Login.aspx
```

Credenciales:

```txt
Usuario: adminp2
Contrasena: AdminPersona2!
```

Luego probar:

- Crear administrador.
- Editar administrador sin cambiar contrasena.
- Inactivar.
- Activar.
- Eliminar.

### ADM6

Entrar a:

```txt
WebAdministrativo/Facturacion.aspx
```

Caso exitoso:

```txt
Fecha de calculo: 2026-08-17
Fecha maxima de pago: 2026-09-01
```

Mensaje esperado:

```txt
Proceso finalizado de forma exitosa
```

Caso de error:

```txt
Fecha de calculo: 2026-08-18
Fecha maxima de pago: 2026-09-01
```

Debe mostrar:

```txt
Error al realizar el proceso
```

Porque deja una brecha desde la ultima facturacion del `2026-08-16`.

## Validacion de compilacion

Comandos usados desde la raiz del repositorio:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' dotnet_webapps/CentralTelefonica.WebApps.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU" /m
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' dotnet_webservices/WS_Autenticacion/WS_Autenticacion.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU" /m
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj /t:Build /p:Configuration=Debug /p:Platform="AnyCPU" /m
```

Resultado esperado:

```txt
Compilacion correcta.
0 Advertencia(s)
0 Errores
```
