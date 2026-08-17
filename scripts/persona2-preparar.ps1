param(
    [switch]$SkipBuild,
    [switch]$ApplyMongoSeed,
    [switch]$ApplySqlMigrations,
    [switch]$UseIntegratedSql,
    [string]$SqlServer = "localhost,49172",
    [string]$SqlDatabase = "CentralProveedor",
    [string]$SqlUser = "charlie_dev",
    [string]$SqlPassword = "Charlie1234",
    [string]$MongoUser = "charlie",
    [string]$MongoPassword = "charlie1234"
)

$ErrorActionPreference = "Stop"
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $RepoRoot

function Require-Command($name) {
    if (-not (Get-Command $name -ErrorAction SilentlyContinue)) {
        throw "No se encontro '$name' en PATH."
    }
}

function Find-MSBuild {
    $candidates = @(
        "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    $fromPath = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
    if ($fromPath) {
        return $fromPath.Source
    }

    throw "No se encontro MSBuild de Visual Studio."
}

if ($ApplyMongoSeed) {
    Write-Host "[MongoDB] Iniciando servicio y aplicando seed persona 2..."
    Start-Service MongoDB -ErrorAction SilentlyContinue
    Require-Command mongosh
    mongosh --host localhost --port 27017 -u $MongoUser -p $MongoPassword --authenticationDatabase central_tg_mongo --eval "load('database/mongodb/crear_coleccion_usuarios.js'); load('database/mongodb/indices_usuarios.js'); load('database/mongodb/datos_semilla_persona_2.js');"
}

if ($ApplySqlMigrations) {
    Write-Host "[SQL Server] Aplicando migraciones persona 2..."
    Require-Command sqlcmd

    $sqlFiles = @(
        "database/sqlserver_proveedor/migrations/010_proveedor6_facturacion.sql",
        "database/sqlserver_proveedor/migrations/012_seed_facturacion_persona2.sql",
        "database/sqlserver_proveedor/migrations/013_normalizar_estado_linea_disponible.sql"
    )

    foreach ($file in $sqlFiles) {
        if ($UseIntegratedSql) {
            sqlcmd -S $SqlServer -d $SqlDatabase -E -i $file
        }
        else {
            sqlcmd -S $SqlServer -d $SqlDatabase -U $SqlUser -P $SqlPassword -i $file
        }
    }
}

if (-not $SkipBuild) {
    Write-Host "[Build] Compilando proyectos C# con MSBuild..."
    $msbuild = Find-MSBuild

    & $msbuild "dotnet_webapps/CentralTelefonica.WebApps.sln" /t:Build /p:Configuration=Debug /p:Platform="Any CPU" /m
    & $msbuild "dotnet_webservices/WS_Autenticacion/WS_Autenticacion.sln" /t:Build /p:Configuration=Debug /p:Platform="Any CPU" /m
    & $msbuild "dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj" /t:Build /p:Configuration=Debug /p:Platform="AnyCPU" /m

    Write-Host "[Build] Compilando simulador C#..."
    Require-Command dotnet
    dotnet build "csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj"

    Write-Host "[Build] Compilando Java proveedor..."
    Require-Command javac
    New-Item -ItemType Directory -Force -Path ".tmp/java_proveedor_classes" | Out-Null
    $javaFiles = Get-ChildItem -Recurse -Filter *.java "java_proveedor" | ForEach-Object { $_.FullName }
    javac -encoding UTF-8 -d ".tmp/java_proveedor_classes" $javaFiles
}

Write-Host ""
Write-Host "Preparacion persona 2 completada."
if (-not $ApplyMongoSeed -and -not $ApplySqlMigrations) {
    Write-Host "No se aplicaron seeds ni migraciones. Para aplicarlos use -ApplyMongoSeed y/o -ApplySqlMigrations."
}
Write-Host "Credenciales admin:   adminp2 / AdminPersona2!"
Write-Host "Credenciales cliente: clientep2 / ClienteUser2!!"
