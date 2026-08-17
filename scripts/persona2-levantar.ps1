param(
    [switch]$SkipMongo,
    [switch]$SkipJava,
    [switch]$SkipPython,
    [switch]$SkipSimuladorCSharp,
    [switch]$SkipAuth,
    [switch]$SkipProveedor,
    [switch]$SkipWebAdmin,
    [switch]$SkipWebCliente
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

if (-not $SkipMongo) {
    Write-Host "[MongoDB] Iniciando servicio..."
    Start-Service MongoDB -ErrorAction SilentlyContinue
    $started.Add("MongoDB")
}

if (-not $SkipJava) {
    if (-not (Test-Path ".tmp/java_proveedor_classes/java_proveedor/Main.class")) {
        Write-Host "[Java] No hay clases compiladas; compilando proveedor..."
        New-Item -ItemType Directory -Force -Path ".tmp/java_proveedor_classes" | Out-Null
        $javaFiles = Get-ChildItem -Recurse -Filter *.java "java_proveedor" | ForEach-Object { $_.FullName }
        javac -encoding UTF-8 -d ".tmp/java_proveedor_classes" $javaFiles
    }

    Start-PowerShellWindow "Persona2 Java Proveedor 6000" 'java -cp ".tmp\java_proveedor_classes;java_proveedor\lib\mssql-jdbc.jar" java_proveedor.Main'
    $started.Add("Java Proveedor: puerto 6000")

    Write-Host "[Java] Esperando puerto 6000..."
    if (-not (Wait-TcpPort "127.0.0.1" 6000 20)) {
        Write-Host "[Java] Aviso: el puerto 6000 aun no responde. Revise la ventana de Java."
    }
}

if (-not $SkipPython) {
    Start-PowerShellWindow "Persona2 Python Identificador" 'Set-Location -LiteralPath "python_identificador"; python main.py'
    $started.Add("Python Identificador: python_identificador/main.py")

    Write-Host "[Python] Esperando puerto 5000..."
    if (-not (Wait-TcpPort "127.0.0.1" 5000 20)) {
        Write-Host "[Python] Aviso: el puerto 5000 aun no responde. Revise la ventana de Python."
    }
}

if (-not $SkipSimuladorCSharp) {
    Start-PowerShellWindow "Persona2 CSharp Simulador" 'dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj'
    $started.Add("Simulador C#: csharp_simulador/SimuladorTelefonico")
}

$iis = Find-IisExpress

if (-not $SkipAuth) {
    Start-PowerShellWindow "Persona2 WS_Autenticacion 59113" "& $(Quote-PS $iis) /path:$(Quote-PS (Join-Path $RepoRoot 'dotnet_webservices\WS_Autenticacion')) /port:59113"
    $started.Add("WSDL Auth: http://localhost:59113/Service1.svc?wsdl")
}

if (-not $SkipProveedor) {
    Start-PowerShellWindow "Persona2 WS_Proveedor 55254" "& $(Quote-PS $iis) /path:$(Quote-PS (Join-Path $RepoRoot 'dotnet_webservices\CentralTelefonica.WebServices\WS_Proveedor')) /port:55254"
    $started.Add("WSDL Proveedor: http://localhost:55254/ProveedorService.svc?wsdl")
}

if (-not $SkipWebAdmin) {
    Start-PowerShellWindow "Persona2 WebAdministrativo 56121" "& $(Quote-PS $iis) /path:$(Quote-PS (Join-Path $RepoRoot 'dotnet_webapps\WebAdministrativo')) /port:56121"
    $started.Add("Web Admin: http://localhost:56121/Login.aspx")
}

if (-not $SkipWebCliente) {
    Start-PowerShellWindow "Persona2 WebCliente 56122" "& $(Quote-PS $iis) /path:$(Quote-PS (Join-Path $RepoRoot 'dotnet_webapps\WebCliente')) /port:56122"
    $started.Add("Web Cliente: http://localhost:56122/Login.aspx")
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
