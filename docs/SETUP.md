# Configuracion inicial del proyecto

## Proyecto desarrollado por el equipo Central TG

Conformado por:

- Carlos Alfredo Mata Fallas / 304830029
- Gabriel Navarro Aguirre / 304960432
- Jose Rodolfo Morales Calderon / 305000192

## 1. Clonar repositorio

```bash
git clone https://github.com/JoseMoralesC/Central_TG.git
cd Central_TG
```

## 2. Instalar requisitos

Instalar:

- Visual Studio 2022 o Build Tools con MSBuild.
- IIS Express.
- .NET SDK compatible con `net8.0` y `net10.0-windows`.
- Python 3.
- Java JDK.
- MySQL.
- SQL Server.
- MongoDB.
- PowerShell.

## 3. Crear `.env`

Crear un archivo `.env` en la raiz del repositorio.

Ejemplo:

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

SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_FROM=correo@dominio.com
SMTP_USER=correo@dominio.com
SMTP_PASSWORD=app-password
SMTP_ENABLE_SSL=true
```

No subir `.env` a Git.

## 4. Preparar bases de datos

Ejecutar los scripts correspondientes:

```txt
database/mysql_identificador/
database/sqlserver_proveedor/
database/mongodb/
```

Bases esperadas:

- MySQL: `central_identificador`
- SQL Server: `CentralProveedor`
- MongoDB: `central_tg_mongo`

## 5. Instalar dependencias Python

```powershell
cd python_identificador
pip install -r requirements.txt
cd ..
```

## 6. Compilar

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\scripts\persona2-preparar.ps1
```

## 7. Levantar sistema

```powershell
.\scripts\persona2-levantar.ps1
```

El script abre ventanas para Java, Python, Simulador, servicios WCF, WebAdmin,
WebCliente y PortalCliente.

## 8. URLs

```txt
WebAdministrativo:   http://localhost:56121/Login.aspx
WebCliente:          http://localhost:56122/Login.aspx
PortalCliente:       http://localhost:56123/Cliente/Index
WS_Autenticacion:    http://localhost:59113/Service1.svc?wsdl
WS_Proveedor:        http://localhost:55254/ProveedorService.svc?wsdl
WS_ProveedorCliente: http://localhost:55260/ProveedorClienteService.svc?wsdl
```

## 9. Ruta de demo

Ver:

```txt
docs/entrega_final/Ruta_demo_alcance_final.md
```

## 10. Diagnostico

- Si falla login: revisar MongoDB.
- Si falla facturacion o devolucion de linea: revisar Java proveedor y SQL
  Server.
- Si falla simulador: revisar Python identificador y Java proveedor.
- Si no llegan correos: revisar SMTP en `.env` y reiniciar con
  `persona2-levantar.ps1`.
