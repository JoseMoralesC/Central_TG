# Requisitos para ejecutar el proyecto en otra maquina

## Proyecto desarrollado por el equipo Central TG

Conformado por:

- Carlos Alfredo Mata Fallas / 304830029
- Gabriel Navarro Aguirre / 304960432
- Jose Rodolfo Morales Calderon / 305000192

## Resumen rapido

Para correr `Central_TG` fuera de la computadora actual se deben instalar y
configurar herramientas externas. El proyecto ya trae codigo fuente, scripts de
base de datos, contratos, documentacion y el driver JDBC de SQL Server.

Se necesita:

- Windows.
- Visual Studio 2022 o Build Tools con MSBuild.
- IIS Express.
- .NET SDK compatible con `net8.0` y `net10.0-windows`.
- Python 3 con `pip`.
- Java JDK.
- MySQL.
- SQL Server.
- MongoDB.
- PowerShell.
- Acceso SMTP para correos reales.

## Requisitos por componente

### Web Administrativo

Ruta:

```txt
dotnet_webapps/WebAdministrativo
```

Requiere:

- ASP.NET WebForms.
- .NET Framework 4.7.2.
- IIS Express.
- MSBuild de Visual Studio.
- Acceso a `WS_Autenticacion` y `WS_Proveedor`.

URL esperada:

```txt
http://localhost:56121/Login.aspx
```

### Web Cliente

Ruta:

```txt
dotnet_webapps/WebCliente
```

Requiere:

- ASP.NET WebForms.
- .NET Framework 4.7.2.
- IIS Express.
- MSBuild de Visual Studio.
- Acceso a `WS_Autenticacion`.

URL esperada:

```txt
http://localhost:56122/Login.aspx
```

### PortalCliente

Ruta:

```txt
dotnet_webservices/PortalCliente
```

Requiere:

- .NET SDK para ASP.NET Core `net8.0`.
- Acceso a `WS_Autenticacion`, `WS_Proveedor` y `WS_ProveedorCliente`.

Comando:

```powershell
dotnet run --project dotnet_webservices\PortalCliente\PortalCliente.csproj --urls http://localhost:56123
```

### Servicios WCF

Rutas:

```txt
dotnet_webservices/WS_Autenticacion
dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor
dotnet_webservices/CentralTelefonica.WebServices/WS_ProveedorCliente
```

Requieren:

- .NET Framework 4.7.2.
- IIS Express.
- MSBuild de Visual Studio.
- MongoDB para autenticacion.
- SQL Server para proveedor.

URLs esperadas:

```txt
http://localhost:59113/Service1.svc?wsdl
http://localhost:55254/ProveedorService.svc?wsdl
http://localhost:55260/ProveedorClienteService.svc?wsdl
```

### Simulador C#

Ruta:

```txt
csharp_simulador/SimuladorTelefonico
```

Requiere:

- Windows.
- .NET SDK compatible con `net10.0-windows`.

Comandos:

```powershell
dotnet build csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj
dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj
```

### Identificador Python

Ruta:

```txt
python_identificador
```

Requiere:

- Python 3.
- Dependencias de `python_identificador/requirements.txt`.
- MySQL.

Instalacion:

```powershell
cd python_identificador
pip install -r requirements.txt
python main.py
```

### Proveedor Java

Ruta:

```txt
java_proveedor
```

Requiere:

- Java JDK.
- SQL Server.
- `java_proveedor/lib/mssql-jdbc.jar`.

Configuracion:

```txt
java_proveedor/src/config/config.properties
```

Compilar/ejecutar:

```powershell
javac -encoding UTF-8 -d .tmp\java_proveedor_classes (Get-ChildItem -Recurse -Filter *.java java_proveedor | ForEach-Object { $_.FullName })
java -cp ".tmp\java_proveedor_classes;java_proveedor\lib\mssql-jdbc.jar" java_proveedor.Main
```

## Bases de datos

| Base | Motor | Uso |
|---|---|---|
| `central_identificador` | MySQL | Telefonos, SIM, dispositivos, llamadas activas, bitacora identificador |
| `CentralProveedor` | SQL Server | Clientes, servicios, saldos, llamadas, facturacion, solicitudes |
| `central_tg_mongo` | MongoDB | Usuarios admin/cliente y metodo de pago |

Scripts:

```txt
database/mysql_identificador/
database/sqlserver_proveedor/
database/mongodb/
```

## Variables `.env`

El archivo `.env` vive en la raiz y no debe subirse a Git.

Variables principales:

```env
IDENTIFICADOR_HOST=127.0.0.1
IDENTIFICADOR_PORT=5000
PROVEEDOR_HOST=127.0.0.1
PROVEEDOR_PORT=6000

MYSQL_HOST=127.0.0.1
MYSQL_PORT=3306
MYSQL_DATABASE=central_identificador
MYSQL_USER=usuario
MYSQL_PASSWORD=password

SQLSERVER_HOST=127.0.0.1
SQLSERVER_PORT=49172
SQLSERVER_DATABASE=CentralProveedor
SQLSERVER_USER=usuario
SQLSERVER_PASSWORD=password

AES_KEY=ClaveSecreta1234
AES_IV=VectorInicio1234
CSHARP_AES_ACTIVO=true

SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_FROM=correo@dominio.com
SMTP_USER=correo@dominio.com
SMTP_PASSWORD=app-password
SMTP_ENABLE_SSL=true
```

## Puertos

| Servicio | Puerto |
|---|---:|
| Identificador Python | 5000 |
| Proveedor Java | 6000 |
| WS Autenticacion | 59113 |
| WS Proveedor | 55254 |
| WS ProveedorCliente | 55260 |
| Web Administrativo | 56121 |
| Web Cliente | 56122 |
| PortalCliente | 56123 |
| MongoDB | 27017 |
| MySQL | 3306 |
| SQL Server | 49172 en ambiente local documentado |

## Ejecucion recomendada

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\scripts\persona2-preparar.ps1
.\scripts\persona2-levantar.ps1
```

Si se quiere aplicar migraciones SQL desde el script:

```powershell
.\scripts\persona2-preparar.ps1 -ApplySqlMigrations
```

MongoDB debe estar iniciado manualmente antes de usar login, usuarios o metodo
de pago.
