param(
    [switch]$Apply,
    [string]$SqlServer,
    [string]$SqlDatabase,
    [string]$SqlUser,
    [string]$SqlPassword,
    [switch]$UseIntegratedSql,
    [switch]$TrustServerCertificate
)

$ErrorActionPreference = "Stop"
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $RepoRoot

function Require-Command($name) {
    if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
        throw "No se encontro '$name' en PATH."
    }
}

function Read-EnvFile {
    $values = @{}
    $envPath = Join-Path $RepoRoot ".env"

    if (-not (Test-Path $envPath)) {
        return $values
    }

    foreach ($line in Get-Content -Path $envPath) {
        $trimmed = $line.Trim()
        if ($trimmed -eq "" -or $trimmed.StartsWith("#") -or -not $trimmed.Contains("=")) {
            continue
        }

        $parts = $trimmed.Split("=", 2)
        $values[$parts[0].Trim()] = $parts[1].Trim()
    }

    return $values
}

function Read-JavaConfig {
    $values = @{}
    $configPath = Join-Path $RepoRoot "java_proveedor\src\config\config.properties"

    if (-not (Test-Path $configPath)) {
        return $values
    }

    foreach ($line in Get-Content -Path $configPath) {
        $trimmed = $line.Trim()
        if ($trimmed -eq "" -or $trimmed.StartsWith("#") -or -not $trimmed.Contains("=")) {
            continue
        }

        $parts = $trimmed.Split("=", 2)
        $values[$parts[0].Trim()] = $parts[1].Trim()
    }

    return $values
}

function Read-JdbcSqlServer {
    param([string]$JdbcUrl)

    $result = @{
        Server = ""
        Database = ""
    }

    if ([string]::IsNullOrWhiteSpace($JdbcUrl)) {
        return $result
    }

    if ($JdbcUrl -match "jdbc:sqlserver://([^;]+)") {
        $result.Server = $Matches[1] -replace ":", ","
    }

    if ($JdbcUrl -match "databaseName=([^;]+)") {
        $result.Database = $Matches[1]
    }

    return $result
}

function Sql-Escape($value) {
    if ($null -eq $value) {
        return ""
    }

    return [string]$value -replace "'", "''"
}

function Numero-Normalizado($value) {
    $numero = [string]$value
    $digitos = $numero -replace "\D", ""

    if ($digitos.Length -gt 8) {
        return $digitos.Substring($digitos.Length - 8)
    }

    return $digitos
}

$envValues = Read-EnvFile
$javaConfig = Read-JavaConfig
$jdbcSql = Read-JdbcSqlServer $javaConfig["db.url"]

if ([string]::IsNullOrWhiteSpace($SqlServer)) {
    $SqlServer = $jdbcSql.Server

    if ([string]::IsNullOrWhiteSpace($SqlServer)) {
        $hostName = $envValues["SQLSERVER_HOST"]
        $port = $envValues["SQLSERVER_PORT"]
        $SqlServer = if ([string]::IsNullOrWhiteSpace($port)) { $hostName } else { "$hostName,$port" }
    }
}

if ([string]::IsNullOrWhiteSpace($SqlDatabase)) {
    $SqlDatabase = $jdbcSql.Database

    if ([string]::IsNullOrWhiteSpace($SqlDatabase)) {
        $SqlDatabase = $envValues["SQLSERVER_DATABASE"]
    }
}

if ([string]::IsNullOrWhiteSpace($SqlUser)) {
    $SqlUser = $javaConfig["db.usuario"]

    if ([string]::IsNullOrWhiteSpace($SqlUser)) {
        $SqlUser = $envValues["SQLSERVER_USER"]
    }
}

if ([string]::IsNullOrWhiteSpace($SqlPassword)) {
    $SqlPassword = $javaConfig["db.contrasena"]

    if ([string]::IsNullOrWhiteSpace($SqlPassword)) {
        $SqlPassword = $envValues["SQLSERVER_PASSWORD"]
    }
}

Require-Command python
$pythonScript = @'
import json
import sys

sys.path.insert(0, "python_identificador")

from app.database.repositorio import listar_telefonos_catalogo
from app.utils.crypto import desencriptar_aes

def normalizar_numero(numero):
    digitos = "".join(ch for ch in str(numero or "") if ch.isdigit())
    return digitos[-8:] if len(digitos) > 8 else digitos

items = []

for row in listar_telefonos_catalogo():
    numero_plano = desencriptar_aes(row.get("numero_cifrado", "")) or row.get("numero_cifrado", "")
    numero = normalizar_numero(numero_plano)

    if not numero:
        continue

    items.append({
        "numero": numero,
        "tipo_servicio": str(row.get("tipo_servicio") or "PREPAGO").strip().upper(),
        "proveedor_codigo": str(row.get("proveedor_codigo") or "KOLBI").strip().upper(),
        "pais": str(row.get("pais") or "Costa Rica").strip(),
        "identificador_tarjeta": row.get("identificador_tarjeta_cifrado") or "",
        "identificador_telefono": row.get("identificador_dispositivo_cifrado") or ""
    })

print(json.dumps(items, ensure_ascii=False))
'@

$tempPy = Join-Path $env:TEMP "central_tg_exportar_inventario_mysql.py"
Set-Content -Path $tempPy -Value $pythonScript -Encoding UTF8

$json = python $tempPy
$telefonos = $json | ConvertFrom-Json

if ($null -eq $telefonos -or $telefonos.Count -eq 0) {
    throw "No se obtuvieron telefonos desde MySQL. Revise conexion y datos reales."
}

$sqlLines = New-Object System.Collections.Generic.List[string]
$sqlLines.Add("USE [$SqlDatabase];")
$sqlLines.Add("SET NOCOUNT ON;")
$sqlLines.Add("BEGIN TRANSACTION;")
$sqlLines.Add("IF COL_LENGTH('dbo.servicios', 'identificador_telefono_cifrado') IS NULL ALTER TABLE dbo.servicios ADD identificador_telefono_cifrado NVARCHAR(255) NULL;")
$sqlLines.Add("IF COL_LENGTH('dbo.servicios', 'identificador_tarjeta_cifrado') IS NULL ALTER TABLE dbo.servicios ADD identificador_tarjeta_cifrado NVARCHAR(255) NULL;")
$sqlLines.Add("IF COL_LENGTH('dbo.servicios', 'identificacion_dueno_cifrada') IS NULL ALTER TABLE dbo.servicios ADD identificacion_dueno_cifrada NVARCHAR(255) NULL;")
$sqlLines.Add("IF COL_LENGTH('dbo.servicios', 'estado_linea') IS NULL ALTER TABLE dbo.servicios ADD estado_linea NVARCHAR(30) NOT NULL CONSTRAINT df_servicios_estado_linea DEFAULT 'DISPONIBLE';")
$sqlLines.Add("IF COL_LENGTH('dbo.servicios', 'pais_id') IS NULL ALTER TABLE dbo.servicios ADD pais_id INT NULL;")
$sqlLines.Add("IF OBJECT_ID('dbo.paises', 'U') IS NULL CREATE TABLE dbo.paises (pais_id INT IDENTITY(1,1) PRIMARY KEY, nombre NVARCHAR(100) NOT NULL UNIQUE, codigo_area NVARCHAR(10) NULL, clasificacion NVARCHAR(30) NULL, activo BIT NOT NULL DEFAULT 1);")
$sqlLines.Add("DECLARE @clienteInventarioId INT;")
$sqlLines.Add("SELECT @clienteInventarioId = cliente_id FROM dbo.clientes WHERE identificacion = 'INV-DISPONIBLE';")
$sqlLines.Add("IF @clienteInventarioId IS NULL BEGIN INSERT INTO dbo.clientes (nombre, identificacion, correo, activo) VALUES ('Inventario Disponible', 'INV-DISPONIBLE', 'inventario@central.test', 1); SET @clienteInventarioId = CAST(SCOPE_IDENTITY() AS INT); END;")
$sqlLines.Add("DECLARE @catalogo TABLE (numero_telefono NVARCHAR(30) PRIMARY KEY, tipo_servicio NVARCHAR(20), proveedor_codigo NVARCHAR(20), pais NVARCHAR(80), identificador_tarjeta_cifrado NVARCHAR(255), identificador_telefono_cifrado NVARCHAR(255));")

foreach ($telefono in $telefonos) {
    $numero = Sql-Escape (Numero-Normalizado $telefono.numero)
    $tipo = Sql-Escape $telefono.tipo_servicio
    $proveedor = Sql-Escape $telefono.proveedor_codigo
    $pais = Sql-Escape $telefono.pais
    $sim = Sql-Escape $telefono.identificador_tarjeta
    $imei = Sql-Escape $telefono.identificador_telefono

    $sqlLines.Add("INSERT INTO @catalogo (numero_telefono, tipo_servicio, proveedor_codigo, pais, identificador_tarjeta_cifrado, identificador_telefono_cifrado) VALUES ('$numero', '$tipo', '$proveedor', '$pais', '$sim', '$imei');")
}

$sqlLines.Add(@"
MERGE dbo.paises AS target
USING (
    SELECT DISTINCT
        pais AS nombre,
        CASE pais
            WHEN 'Costa Rica' THEN '+506'
            WHEN 'Panama' THEN '+507'
            WHEN 'Mexico' THEN '+52'
            WHEN 'Francia' THEN '+33'
            ELSE ''
        END AS codigo_area,
        CASE WHEN pais = 'Costa Rica' THEN 'NACIONAL' ELSE 'INTERNACIONAL' END AS clasificacion
    FROM @catalogo
) AS source
ON target.nombre = source.nombre
WHEN MATCHED THEN
    UPDATE SET
        codigo_area = CASE WHEN source.codigo_area <> '' THEN source.codigo_area ELSE target.codigo_area END,
        clasificacion = source.clasificacion,
        activo = 1
WHEN NOT MATCHED THEN
    INSERT (nombre, codigo_area, clasificacion, activo)
    VALUES (source.nombre, source.codigo_area, source.clasificacion, 1);

MERGE dbo.servicios AS target
USING (
    SELECT
        c.numero_telefono,
        c.tipo_servicio,
        c.proveedor_codigo,
        p.pais_id,
        c.identificador_tarjeta_cifrado,
        c.identificador_telefono_cifrado
    FROM @catalogo c
    LEFT JOIN dbo.paises p ON p.nombre = c.pais
) AS source
ON target.numero_telefono = source.numero_telefono
WHEN MATCHED THEN
    UPDATE SET
        cliente_id = @clienteInventarioId,
        tipo_servicio = source.tipo_servicio,
        proveedor_codigo = source.proveedor_codigo,
        pais_id = source.pais_id,
        activo = 0,
        estado_linea = 'DISPONIBLE',
        identificacion_dueno_cifrada = NULL,
        identificador_tarjeta_cifrado = source.identificador_tarjeta_cifrado,
        identificador_telefono_cifrado = source.identificador_telefono_cifrado
WHEN NOT MATCHED THEN
    INSERT (
        cliente_id,
        numero_telefono,
        tipo_servicio,
        proveedor_codigo,
        pais_id,
        activo,
        estado_linea,
        identificacion_dueno_cifrada,
        identificador_tarjeta_cifrado,
        identificador_telefono_cifrado
    )
    VALUES (
        @clienteInventarioId,
        source.numero_telefono,
        source.tipo_servicio,
        source.proveedor_codigo,
        source.pais_id,
        0,
        'DISPONIBLE',
        NULL,
        source.identificador_tarjeta_cifrado,
        source.identificador_telefono_cifrado
    );

UPDATE s
SET s.activo = 0,
    s.estado_linea = 'FUERA_CATALOGO',
    s.identificacion_dueno_cifrada = NULL
FROM dbo.servicios s
WHERE NOT EXISTS (
    SELECT 1
    FROM @catalogo c
    WHERE c.numero_telefono = s.numero_telefono
);

MERGE dbo.saldos AS target
USING (
    SELECT servicio_id
    FROM dbo.servicios
    WHERE estado_linea = 'DISPONIBLE'
) AS source
ON target.servicio_id = source.servicio_id
WHEN MATCHED THEN
    UPDATE SET saldo_disponible = 0, fecha_actualizacion = GETDATE()
WHEN NOT MATCHED THEN
    INSERT (servicio_id, saldo_disponible)
    VALUES (source.servicio_id, 0);

IF OBJECT_ID('dbo.facturacion_postpago', 'U') IS NOT NULL
BEGIN
    UPDATE f
    SET f.total_facturar = 0
    FROM dbo.facturacion_postpago f
    INNER JOIN dbo.servicios s ON s.servicio_id = f.servicio_id
    WHERE s.estado_linea <> 'ACTIVO';
END;

SELECT
    (SELECT COUNT(1) FROM @catalogo) AS telefonos_mysql,
    SUM(CASE WHEN estado_linea = 'DISPONIBLE' THEN 1 ELSE 0 END) AS lineas_disponibles_sql,
    SUM(CASE WHEN estado_linea = 'FUERA_CATALOGO' THEN 1 ELSE 0 END) AS lineas_sql_fuera_catalogo,
    SUM(CASE WHEN activo = 1 OR estado_linea = 'ACTIVO' THEN 1 ELSE 0 END) AS lineas_activas_restantes
FROM dbo.servicios;

COMMIT TRANSACTION;
"@)

$tempSql = Join-Path $env:TEMP "central_tg_sincronizar_inventario_mysql_a_sqlserver.sql"
Set-Content -Path $tempSql -Value ($sqlLines -join [Environment]::NewLine) -Encoding UTF8

Write-Host "[Inventario] Telefonos leidos desde MySQL: $($telefonos.Count)"
Write-Host "[Inventario] SQL temporal generado: $tempSql"

if (-not $Apply) {
    Write-Host "[Inventario] Modo vista previa. Para aplicar cambios ejecute de nuevo con -Apply."
    Get-Content -Path $tempSql -TotalCount 40
    exit 0
}

if ($UseIntegratedSql) {
    $connectionString =
        "Server=$SqlServer;Database=$SqlDatabase;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;Connection Timeout=15"
}
else {
    $connectionString =
        "Server=$SqlServer;Database=$SqlDatabase;User ID=$SqlUser;Password=$SqlPassword;Encrypt=False;TrustServerCertificate=True;Connection Timeout=15"
}

Add-Type -AssemblyName System.Data
$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)

try {
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandTimeout = 120
    $command.CommandText = Get-Content -Raw -Path $tempSql

    $reader = $command.ExecuteReader()
    do {
        if ($reader.FieldCount -le 0) {
            continue
        }

        while ($reader.Read()) {
            $values = @()
            for ($i = 0; $i -lt $reader.FieldCount; $i++) {
                $values += ($reader.GetName($i) + "=" + $reader.GetValue($i))
            }

            Write-Host ($values -join "; ")
        }
    } while ($reader.NextResult())

    $reader.Close()
}
finally {
    if ($connection.State -ne "Closed") {
        $connection.Close()
    }
}
