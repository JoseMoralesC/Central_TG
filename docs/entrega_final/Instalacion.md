# Instalacion y ejecucion

## Proyecto desarrollado por el equipo Central TG

Conformado por:

- Carlos Alfredo Mata Fallas / 304830029
- Gabriel Navarro Aguirre / 304960432
- Jose Rodolfo Morales Calderon / 305000192

## 1. Requisitos

Para ejecutar el proyecto completo en otra maquina se necesita instalar:

- Windows.
- Visual Studio 2022 o Build Tools con MSBuild para compilar WebForms/WCF.
- IIS Express.
- .NET SDK compatible con `net8.0` y `net10.0-windows`.
- Python 3 con `pip`.
- Java JDK.
- MySQL.
- SQL Server.
- MongoDB.
- PowerShell.
- Driver JDBC SQL Server; el proyecto incluye `java_proveedor/lib/mssql-jdbc.jar`.

## 2. Configuracion del archivo `.env`

Crear o revisar `.env` en la raiz del proyecto. Este archivo contiene
credenciales y no debe subirse al repositorio.

Ejemplo base:

```env
IDENTIFICADOR_HOST=127.0.0.1
IDENTIFICADOR_PORT=5000
PROVEEDOR_HOST=127.0.0.1
PROVEEDOR_PORT=6000

MYSQL_HOST=127.0.0.1
MYSQL_PORT=3306
MYSQL_DATABASE=central_identificador
MYSQL_USER=usuario_mysql
MYSQL_PASSWORD=password_mysql

SQLSERVER_HOST=127.0.0.1
SQLSERVER_PORT=49172
SQLSERVER_DATABASE=CentralProveedor
SQLSERVER_USER=usuario_sql
SQLSERVER_PASSWORD=password_sql

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

SMTP es requerido para evidenciar el envio real de:

- factura postpago generada desde WebAdministrativo;
- recibo de pago de factura desde PortalCliente.

## 3. Base MySQL - Identificador

Base esperada:

```txt
central_identificador
```

Ejecutar scripts en orden:

```txt
database/mysql_identificador/schema/001_create_database_identificador.sql
database/mysql_identificador/seed/001_seed_identificador.sql
database/mysql_identificador/migrations/
```

La llave `AES_KEY` y `AES_IV` deben coincidir con C# y Python.

## 4. Base SQL Server - Proveedor

Base esperada:

```txt
CentralProveedor
```

Ejecutar scripts en orden:

```txt
database/sqlserver_proveedor/schema/001_create_database_proveedor.sql
database/sqlserver_proveedor/seed/001_seed_proveedor.sql
database/sqlserver_proveedor/migrations/
```

Tambien se puede aplicar el grupo principal de migraciones con:

```powershell
.\scripts\persona2-preparar.ps1 -ApplySqlMigrations
```

## 5. MongoDB - Usuarios

MongoDB debe estar disponible en:

```txt
localhost:27017
```

Se usa para:

- administradores;
- clientes;
- credenciales cifradas;
- metodo de pago del cliente.

Revisar scripts y notas en:

```txt
database/mongodb/
```

## 6. Preparar componentes

Desde la raiz del repositorio:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\scripts\persona2-preparar.ps1
```

Este script compila:

- WebAdministrativo y WebCliente;
- WS_Autenticacion;
- WS_Proveedor;
- WS_ProveedorCliente;
- PortalCliente;
- Simulador C#;
- Java proveedor.

Para los proyectos WCF/WebForms se requiere MSBuild de Visual Studio.

## 7. Levantar el sistema completo

Desde la raiz:

```powershell
.\scripts\persona2-levantar.ps1
```

El script:

- carga `.env`;
- configura SMTP para WebAdministrativo y PortalCliente;
- levanta Java proveedor;
- levanta Python identificador;
- levanta Simulador C#;
- levanta servicios WCF en IIS Express;
- levanta PortalCliente.

MongoDB no se levanta desde este script; debe estar iniciado manualmente.

## 8. URLs locales

| Componente | URL |
|---|---|
| WS Autenticacion | `http://localhost:59113/Service1.svc?wsdl` |
| WS Proveedor | `http://localhost:55254/ProveedorService.svc?wsdl` |
| WS ProveedorCliente | `http://localhost:55260/ProveedorClienteService.svc?wsdl` |
| Web Administrativo | `http://localhost:56121/Login.aspx` |
| Web Cliente | `http://localhost:56122/Login.aspx` |
| PortalCliente | `http://localhost:56123/Cliente/Index` |

## 9. Orden recomendado de prueba

1. Verificar SQL Server, MySQL y MongoDB.
2. Ejecutar `persona2-preparar.ps1`.
3. Ejecutar `persona2-levantar.ps1`.
4. Entrar a WebAdministrativo.
5. Crear/asignar linea.
6. Generar factura postpago.
7. Entrar a WebCliente.
8. Pagar factura desde PortalCliente.
9. Confirmar correo de factura y correo de recibo.
10. Devolver linea desde PortalCliente.

## 10. Diagnostico rapido

- Si falla login, revisar MongoDB en `localhost:27017`.
- Si falla facturacion, revisar Java proveedor en puerto `6000`.
- Si falla consulta/llamada del simulador, revisar Python identificador en
  puerto `5000`.
- Si no llegan correos, cerrar procesos y volver a ejecutar
  `persona2-levantar.ps1` para que cargue SMTP desde `.env`.
- Si falla compilacion WCF desde terminal, abrir Visual Studio o instalar Build
  Tools con MSBuild.
