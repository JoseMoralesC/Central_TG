# Instalacion

## Requisitos

- .NET SDK compatible con `net10.0-windows`.
- Python 3.
- Java JDK.
- MySQL.
- SQL Server.
- Driver JDBC SQL Server en el classpath cuando se ejecute Java.

## Configuracion

Crear o revisar `.env` en la raiz del proyecto:

```env
IDENTIFICADOR_HOST=127.0.0.1
IDENTIFICADOR_PORT=5000
PROVEEDOR_HOST=127.0.0.1
PROVEEDOR_PORT=6000
AES_KEY=ClaveSecreta1234
AES_IV=VectorInicio1234
```

## Base MySQL

Ejecutar:

```txt
database/mysql_identificador/schema/001_create_database_identificador.sql
database/mysql_identificador/seed/001_seed_identificador.sql
database/mysql_identificador/migrations/002_align_identificador_with_contracts.sql
database/mysql_identificador/migrations/003_update_identificador_seed_values_to_aes.sql
```

La migracion 003 es necesaria para que los datos cifrados coincidan con C# y Python.

## Base SQL Server

Ejecutar:

```txt
database/sqlserver_proveedor/schema/001_create_database_proveedor.sql
database/sqlserver_proveedor/seed/001_seed_proveedor.sql
database/sqlserver_proveedor/migrations/002_align_proveedor_with_contracts.sql
```

## Levantar Java Proveedor

Desde la raiz:

```powershell
javac java_proveedor\Main.java java_proveedor\src\sockets\SocketTCP.java java_proveedor\src\sockets\ManejoCliente.java java_proveedor\src\services\*.java java_proveedor\src\database\*.java java_proveedor\src\models\*.java java_proveedor\src\config\*.java
java -cp ".;java_proveedor\lib\mssql-jdbc.jar" java_proveedor.Main
```

## Levantar Python Identificador

```powershell
cd python_identificador
python main.py
```

## Levantar C# Simulador

Desde la raiz:

```powershell
dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj
```

## Orden recomendado

1. SQL Server y MySQL disponibles.
2. Java Proveedor.
3. Python Identificador.
4. C# Simulador.
