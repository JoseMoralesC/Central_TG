# Requisitos para ejecutar el proyecto en otra maquina

Este documento resume que se necesita instalar y configurar para correr el
proyecto `Central_TG` fuera de la computadora actual.

## Resumen rapido

Si se ejecuta en otra maquina, si se deben instalar herramientas externas:

- .NET SDK 10 para el simulador C# WinForms.
- Python 3 con `pip` para el Identificador.
- Java JDK para compilar y ejecutar el proveedor Java.
- Acceso a MySQL para la base del Identificador.
- Acceso a SQL Server para la base del Proveedor.
- Dependencias Python del archivo `python_identificador/requirements.txt`.

El proyecto ya trae en el repositorio:

- Codigo fuente C#, Python y Java.
- Scripts SQL de creacion, migracion y datos iniciales.
- Contratos JSON en `shared/contracts`.
- Driver JDBC de SQL Server en `java_proveedor/lib/mssql-jdbc.jar`.

## Lo instalado en esta maquina

En la computadora actual el proyecto corre con:

| Herramienta | Version detectada |
|---|---|
| .NET SDK | 10.0.300 |
| .NET Runtime | 10.0.8 y 8.0.27 |
| Python | 3.13.5 |
| pip | 25.1.1 |
| Java JDK | OpenJDK 21.0.6 |
| javac | 21.0.6 |

Librerias Python instaladas actualmente:

| Libreria | Version instalada | Version pedida en requirements |
|---|---:|---:|
| python-dotenv | 1.2.2 | 1.0.1 |
| pycryptodome | 3.23.0 | 3.20.0 |
| mysql-connector-python | 9.7.0 | 8.0.33 |

Nota: aunque en esta maquina hay versiones mas nuevas de las librerias Python, en
otra maquina se recomienda instalar exactamente lo indicado por
`python_identificador/requirements.txt` para evitar diferencias.

## Requisitos por componente

### Simulador C#

Requiere:

- Windows, porque el proyecto usa WinForms.
- .NET SDK compatible con `net10.0-windows`.

Archivo principal del proyecto:

```txt
csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj
```

Comandos desde la raiz del proyecto:

```powershell
dotnet build csharp_simulador\SimuladorTelefonico.slnx
dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj
```

Variables usadas por C#:

```env
IDENTIFICADOR_HOST=127.0.0.1
IDENTIFICADOR_PORT=5000
CSHARP_SOCKET_CONNECT_TIMEOUT_MS=5000
CSHARP_SOCKET_READ_TIMEOUT_MS=8000
CSHARP_SOCKET_BUFFER_BYTES=8192
SOCKET_ENCODING=UTF-8
AES_KEY=ClaveSecreta1234
AES_IV=VectorInicio1234
CSHARP_AES_ACTIVO=true
```

### Identificador Python

Requiere:

- Python 3.
- pip.
- MySQL accesible.

Instalar dependencias:

```powershell
cd python_identificador
pip install -r requirements.txt
```

Ejecutar:

```powershell
python main.py
```

Variables usadas por Python:

```env
MYSQL_HOST=localhost
MYSQL_PORT=3306
MYSQL_DATABASE=central_identificador
MYSQL_USER=root
MYSQL_PASSWORD=tu_password
IDENTIFICADOR_HOST=0.0.0.0
IDENTIFICADOR_PORT=5000
PROVEEDOR_HOST=127.0.0.1
PROVEEDOR_PORT=6000
APP_ENV=development
AES_KEY=ClaveSecreta1234
AES_IV=VectorInicio1234
```

### Proveedor Java

Requiere:

- Java JDK.
- SQL Server accesible.
- Driver JDBC de SQL Server.

El driver JDBC ya esta incluido en:

```txt
java_proveedor/lib/mssql-jdbc.jar
```

La configuracion del proveedor esta en:

```txt
java_proveedor/src/config/config.properties
```

Ese archivo debe tener los datos correctos para la maquina o base que se vaya a
usar:

```properties
socket.puerto=6000
db.url=jdbc:sqlserver://HOST:PUERTO;databaseName=CentralProveedor;encrypt=true;trustServerCertificate=true;
db.usuario=usuario_sqlserver
db.contrasena=password_sqlserver
```

Compilar y ejecutar desde la raiz:

```powershell
javac java_proveedor\Main.java java_proveedor\src\sockets\SocketTCP.java java_proveedor\src\sockets\ManejoCliente.java java_proveedor\src\services\*.java java_proveedor\src\database\*.java java_proveedor\src\models\*.java java_proveedor\src\config\*.java
java -cp ".;java_proveedor\lib\mssql-jdbc.jar" java_proveedor.Main
```

## Bases de datos

### MySQL Identificador

Base esperada:

```txt
central_identificador
```

Scripts disponibles:

```txt
database/mysql_identificador/schema/001_create_database_identificador.sql
database/mysql_identificador/seed/001_seed_identificador.sql
database/mysql_identificador/migrations/
```

### SQL Server Proveedor

Base esperada:

```txt
CentralProveedor
```

Scripts disponibles:

```txt
database/sqlserver_proveedor/schema/001_create_database_proveedor.sql
database/sqlserver_proveedor/seed/001_seed_proveedor.sql
database/sqlserver_proveedor/migrations/
```

## Puertos necesarios

| Servicio | Puerto |
|---|---:|
| Identificador Python | 5000 |
| Proveedor Java | 6000 |
| MySQL | 3306 |
| SQL Server | depende de la configuracion, en docs aparece 49172 |

En otra maquina se debe permitir comunicacion por esos puertos, especialmente si
los componentes no corren todos en el mismo equipo.

## Archivo .env

El codigo C# y Python puede leer variables desde un archivo `.env` en la raiz del
proyecto. Ese archivo no debe subirse al repositorio porque contiene credenciales.

Importante: la documentacion existente menciona `.env.example`, pero actualmente
no existe un `.env.example` en el repositorio.

Ejemplo base de `.env`:

```env
IDENTIFICADOR_HOST=127.0.0.1
IDENTIFICADOR_PORT=5000
PROVEEDOR_HOST=127.0.0.1
PROVEEDOR_PORT=6000

MYSQL_HOST=localhost
MYSQL_PORT=3306
MYSQL_DATABASE=central_identificador
MYSQL_USER=root
MYSQL_PASSWORD=tu_password

AES_KEY=ClaveSecreta1234
AES_IV=VectorInicio1234
CSHARP_AES_ACTIVO=true
```

## Orden recomendado para correr

1. Tener MySQL y SQL Server disponibles.
2. Verificar que las bases y datos iniciales existan.
3. Levantar el Proveedor Java en el puerto 6000.
4. Levantar el Identificador Python en el puerto 5000.
5. Levantar el Simulador C#.

## Observaciones importantes

- La llave `AES_KEY` y el vector `AES_IV` deben ser iguales en C# y Python.
- `AES_KEY` debe tener una longitud valida para AES: 16, 24 o 32 bytes.
- El simulador C# depende de Windows por WinForms.
- Si se cambia de computadora, revisar IPs de MySQL, SQL Server y sockets.
- El archivo `java_proveedor/src/config/config.properties` contiene la conexion
  usada por Java; se debe ajustar si cambia el servidor SQL Server.
