param(
    [switch]$SkipJava,
    [switch]$SkipPython,
    [switch]$SkipSimuladorCSharp,
    [switch]$SkipAuth,
    [switch]$SkipProveedor,
    [switch]$SkipProveedorCliente,
    [switch]$SkipWebAdmin,
    [switch]$SkipWebCliente,
    [switch]$SkipPortalCliente
)

$ErrorActionPreference = "Stop"
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $RepoRoot
$started = New-Object System.Collections.Generic.List[string]

function Find-IisExpress {
    $candidates = @(
        "C:\Program Files\IIS Express\iisexpress.exe",
        "C:\Program Files (x86)\IIS Express\iisexpress.exe"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    throw "No se encontro IIS Express."
}

function Quote-PS($value) {
    return "'" + ($value -replace "'", "''") + "'"
}

function Start-PowerShellWindow($title, $command) {
    $root = Quote-PS $RepoRoot
    $windowTitle = Quote-PS $title
    $fullCommand = "`$Host.UI.RawUI.WindowTitle = $windowTitle; Set-Location -LiteralPath $root; $command"
    Start-Process -FilePath "powershell.exe" -ArgumentList @("-NoExit", "-ExecutionPolicy", "Bypass", "-Command", $fullCommand)
}

function Start-IisExpressWindow($title, $path, $port, $startedLabel) {
    if (Wait-TcpPort "127.0.0.1" $port 1) {
        Write-Host "[$title] Ya hay un proceso escuchando en el puerto $port; no se abre otra instancia de IIS Express."
        $started.Add("$startedLabel (ya activo)")
        return
    }

    Start-PowerShellWindow $title "& $(Quote-PS $iis) /path:$(Quote-PS $path) /port:$port"
    $started.Add($startedLabel)
}

function Wait-TcpPort($hostName, $port, $timeoutSeconds) {
    $deadline = (Get-Date).AddSeconds($timeoutSeconds)

    while ((Get-Date) -lt $deadline) {
        $client = New-Object System.Net.Sockets.TcpClient
        try {
            $result = $client.BeginConnect($hostName, $port, $null, $null)
            if ($result.AsyncWaitHandle.WaitOne(500)) {
                $client.EndConnect($result)
                return $true
            }
        }
        catch {
        }
        finally {
            $client.Close()
        }

        Start-Sleep -Milliseconds 500
    }

    return $false
}

Write-Host "[MongoDB] No se levanta desde este script. Debe estar iniciado manualmente en localhost:27017 con datos reales."
if (Wait-TcpPort "127.0.0.1" 27017 2) {
    $started.Add("MongoDB externo: puerto 27017")
}
else {
    Write-Host "[MongoDB] Aviso: el puerto 27017 no responde. Inicie MongoDB manualmente antes de usar login/usuarios."
}


if (-not $SkipJava) {
    if (-not (Test-Path ".tmp/java_proveedor_classes/java_proveedor/Main.class")) {
        Write-Host "[Java] No hay clases compiladas; compilando proveedor..."
        New-Item -ItemType Directory -Force -Path ".tmp/java_proveedor_classes" | Out-Null
        $javaFiles = Get-ChildItem -Recurse -Filter *.java "java_proveedor" | ForEach-Object { $_.FullName }
        javac -encoding UTF-8 -d ".tmp/java_proveedor_classes" $javaFiles
    }

    New-Item -ItemType Directory -Force -Path "logs" | Out-Null
    $javaLog = Join-Path $RepoRoot "logs\java_proveedor_6000.log"
    Remove-Item -LiteralPath $javaLog -ErrorAction SilentlyContinue

    Start-PowerShellWindow "Java Proveedor 6000" "& java -cp '.tmp\java_proveedor_classes;java_proveedor\lib\mssql-jdbc.jar' java_proveedor.Main 2>&1 | Tee-Object -FilePath 'logs\java_proveedor_6000.log'"
    $started.Add("Java Proveedor: puerto 6000")

    Write-Host "[Java] Esperando puerto 6000..."
    if (-not (Wait-TcpPort "127.0.0.1" 6000 60)) {
        Write-Host "[Java] Aviso: el puerto 6000 aun no responde. Revise la ventana de Java."
        if (Test-Path $javaLog) {
            Write-Host "[Java] Ultimas lineas del log:"
            Get-Content -Path $javaLog -Tail 20
        }
    }
}

if (-not $SkipPython) {
    Start-PowerShellWindow "Python Identificador 5000" 'Set-Location -LiteralPath "python_identificador"; python main.py'
    $started.Add("Python Identificador: python_identificador/main.py")

    Write-Host "[Python] Esperando puerto 5000..."
    if (-not (Wait-TcpPort "127.0.0.1" 5000 20)) {
        Write-Host "[Python] Aviso: el puerto 5000 aun no responde. Revise la ventana de Python."
    }
}

if (-not $SkipSimuladorCSharp) {
    Start-PowerShellWindow "CSharp Simulador Telefonico" 'dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj'
    $started.Add("Simulador C#: csharp_simulador/SimuladorTelefonico")
}

$iis = Find-IisExpress

if (-not $SkipAuth) {
    Start-IisExpressWindow "WS_Autenticacion 59113" (Join-Path $RepoRoot 'dotnet_webservices\WS_Autenticacion') 59113 "WSDL Auth: http://localhost:59113/Service1.svc?wsdl"
}

if (-not $SkipProveedor) {
    Start-IisExpressWindow "WS_Proveedor 55254" (Join-Path $RepoRoot 'dotnet_webservices\CentralTelefonica.WebServices\WS_Proveedor') 55254 "WSDL Proveedor: http://localhost:55254/ProveedorService.svc?wsdl"
}

if (-not $SkipProveedorCliente) {
    Start-IisExpressWindow "WS_ProveedorCliente 55260" (Join-Path $RepoRoot 'dotnet_webservices\CentralTelefonica.WebServices\WS_ProveedorCliente') 55260 "WSDL ProveedorCliente: http://localhost:55260/ProveedorClienteService.svc?wsdl"
}

if (-not $SkipWebAdmin) {
    Start-IisExpressWindow "WebAdministrativo 56121" (Join-Path $RepoRoot 'dotnet_webapps\WebAdministrativo') 56121 "Web Admin: http://localhost:56121/Login.aspx"
}

if (-not $SkipWebCliente) {
    Start-IisExpressWindow "WebCliente 56122" (Join-Path $RepoRoot 'dotnet_webapps\WebCliente') 56122 "Web Cliente: http://localhost:56122/Login.aspx"
}

if (-not $SkipPortalCliente) {
    Start-PowerShellWindow "PortalCliente Transaccional 56123" 'dotnet run --project dotnet_webservices\PortalCliente\PortalCliente.csproj --urls http://localhost:56123'
    $started.Add("Portal transaccional cliente: http://localhost:56123/Cliente/Index (destino despues del login)")
}

Write-Host ""
if ($started.Count -eq 0) {
    Write-Host "No se lanzo ningun proceso porque todos fueron omitidos."
} else {
    Write-Host "Procesos solicitados lanzados:"
    foreach ($item in $started) {
        Write-Host "- $item"
    }
}
