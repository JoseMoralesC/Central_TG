# Guia para levantar el proyecto por terminales

## Objetivo

Levantar el sistema integrado completo:

```txt
C# Simulador -> Python Identificador -> Java Proveedor -> SQL Server
```

Para pruebas integradas se recomienda usar **tres terminales separadas en VS Code**:

- Terminal 1: Proveedor Java.
- Terminal 2: Identificador Python.
- Terminal 3: Simulador C#.

El orden recomendado es:

1. Java Proveedor.
2. Python Identificador.
3. C# Simulador.

## 1. Requisitos previos

Antes de levantar el sistema, validar:

- Tailscale encendido si se usan bases remotas.
- SQL Server accesible.
- MySQL accesible.
- Archivo `.env` configurado.
- Driver JDBC de SQL Server disponible en:

```txt
java_proveedor/lib/mssql-jdbc.jar
```

Valores esperados principales:

```env
IDENTIFICADOR_HOST=127.0.0.1
IDENTIFICADOR_PORT=5000
PROVEEDOR_HOST=127.0.0.1
PROVEEDOR_PORT=6000

MYSQL_HOST=100.114.84.5
MYSQL_PORT=3306
MYSQL_DATABASE=central_identificador

SQLSERVER_HOST=100.114.84.5
SQLSERVER_PORT=49172

AES_KEY=ClaveSecreta1234
AES_IV=VectorInicio1234
```

## 2. Terminal 1 - Levantar Proveedor Java

Abrir una terminal nueva en VS Code.

Ir a la raiz del proyecto:

```powershell
cd C:\Users\Personal\Desktop\Central_TG
```

Compilar Java:

```powershell
javac java_proveedor\Main.java java_proveedor\src\sockets\SocketTCP.java java_proveedor\src\sockets\ManejoCliente.java java_proveedor\src\services\*.java java_proveedor\src\database\*.java java_proveedor\src\models\*.java java_proveedor\src\config\*.java
```

Ejecutar el socket del proveedor:

```powershell
java -cp ".;java_proveedor\lib\mssql-jdbc.jar" java_proveedor.src.sockets.SocketTCP
```

Resultado esperado:

```txt
[Proveedor] Escuchando en 127.0.0.1:6000
[Config] Configuracion cargada con exito desde Classpath.
```

Importante:

- Esta terminal debe quedar abierta.
- Si aparece `ClassNotFoundException: com.microsoft.sqlserver.jdbc.SQLServerDriver`, falta el `.jar` o no se ejecuto con `-cp`.
- Si aparece error de conexion TCP/IP a SQL Server, revisar Tailscale, host, puerto, usuario y password.

## 3. Terminal 2 - Levantar Identificador Python

Abrir una segunda terminal nueva en VS Code.

Entrar al componente Python:

```powershell
cd C:\Users\Personal\Desktop\Central_TG\python_identificador
```

Ejecutar el servidor:

```powershell
python main.py
```

Resultado esperado:

```txt
[Identificador] Iniciando servidor central...
[Socket Servidor] Escuchando activamente en 127.0.0.1:5000
[Bitacora] Hilo de auditoria asincrona listo y escuchando la cola...
[Verificador] Hilo de monitoreo de llamadas activas iniciado
```

Importante:

- Esta terminal debe quedar abierta.
- Python escucha las solicitudes del simulador C#.
- Python tambien se conecta al proveedor Java en el puerto `6000`.

## 4. Terminal 3 - Levantar Simulador C#

Abrir una tercera terminal nueva en VS Code.

Ir a la raiz del proyecto:

```powershell
cd C:\Users\Personal\Desktop\Central_TG
```

Compilar C#:

```powershell
dotnet build csharp_simulador\SimuladorTelefonico.slnx
```

Ejecutar simulador:

```powershell
dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj
```

Resultado esperado:

- Se abre la ventana del simulador.
- La pantalla principal muestra telefonos virtuales.
- El subtitulo indica el Identificador Python configurado.

Importante:

- Si el build falla porque `SimuladorTelefonico.exe` esta en uso, cerrar la ventana anterior del simulador y volver a ejecutar.
- El simulador debe levantarse despues de Python para poder conectarse.

## 5. Prueba rapida 1 - Consulta de saldo

Con las tres terminales activas:

1. En C#, seleccionar telefono `88889999`.
2. Presionar `Saldo`.
3. Confirmar el codigo:

```txt
#9090*
```

Resultado esperado en C#:

```txt
Estado: OK
Saldo: 5000.00 CRC
```

Resultado esperado en Python:

```txt
[Socket] Nueva conexion entrante desde: ...
[Database] Conexion exitosa a la base de datos MySQL.
```

Resultado esperado en Java:

- Java recibe la consulta del proveedor.
- Si SQL Server esta disponible, no debe mostrar error de conexion.

## 6. Prueba rapida 2 - Llamada autorizada

Con las tres terminales activas:

1. En C#, presionar `Marcar`.
2. Ingresar destino:

```txt
22223333
```

3. Presionar `Solicitar llamada`.

Resultado esperado:

- C# muestra respuesta `OK`.
- Se abre la pantalla `Llamada activa`.
- Python registra la llamada activa.
- Java autoriza segun saldo o tipo de servicio.

## 7. Prueba rapida 3 - Finalizar llamada

Con la llamada activa:

1. Esperar unos segundos.
2. Presionar `Finalizar llamada`.

Resultado esperado:

- C# envia `FINALIZAR_LLAMADA`.
- Python envia registro de movimiento al proveedor.
- Java responde `OK`.
- C# muestra estado de llamada finalizada.

Nota importante:

Actualmente Java puede responder `OK` para movimiento, pero PROVEEDOR2 todavia debe completarse para registrar realmente en SQL Server y rebajar saldo.

## 8. Errores comunes

### Error: Java se queda esperando

Esto es normal si muestra:

```txt
[Proveedor] Escuchando en 127.0.0.1:6000
```

El socket esta activo y esperando conexiones de Python.

### Error: No suitable driver found

Causa:

- Se ejecuto Java sin el driver JDBC en el classpath.

Solucion:

```powershell
java -cp ".;java_proveedor\lib\mssql-jdbc.jar" java_proveedor.src.sockets.SocketTCP
```

### Error: ClassNotFoundException SQLServerDriver

Causa:

- Falta `java_proveedor/lib/mssql-jdbc.jar`.

Solucion:

- Descargar o colocar el driver JDBC en esa ruta.
- Ejecutar usando `-cp`.

### Error: No se pudo realizar conexion TCP/IP a SQL Server

Causas posibles:

- Tailscale apagado.
- Host incorrecto.
- Puerto incorrecto.
- SQL Server no acepta conexiones remotas.
- Firewall bloqueando.

Verificacion:

```powershell
Test-NetConnection 100.114.84.5 -Port 49172
```

### Error: C# no conecta con Python

Causas posibles:

- Python no esta levantado.
- Puerto `5000` ocupado.
- `IDENTIFICADOR_HOST` incorrecto.

Verificacion:

```powershell
Test-NetConnection 127.0.0.1 -Port 5000
```

### Error: Python responde telefono inactivo o no registrado

Causas posibles:

- MySQL no tiene el telefono cifrado con AES real.
- Los seeds tienen datos `ENC_*`.
- La llave o IV de AES no coincide entre C# y Python.

Verificar:

```env
AES_KEY=ClaveSecreta1234
AES_IV=VectorInicio1234
```

## 9. Orden recomendado para demo

1. Abrir VS Code en `Central_TG`.
2. Terminal 1: levantar Java proveedor.
3. Terminal 2: levantar Python identificador.
4. Terminal 3: levantar C# simulador.
5. Probar consulta de saldo.
6. Probar llamada autorizada.
7. Probar llamada activa.
8. Probar finalizacion.
9. Mostrar bitacora del identificador.
10. Explicar pendiente: PROVEEDOR2 y PROVEEDOR3 requieren implementacion completa.

## 10. Comandos resumidos

### Terminal 1

```powershell
cd C:\Users\Personal\Desktop\Central_TG
javac java_proveedor\Main.java java_proveedor\src\sockets\SocketTCP.java java_proveedor\src\sockets\ManejoCliente.java java_proveedor\src\services\*.java java_proveedor\src\database\*.java java_proveedor\src\models\*.java java_proveedor\src\config\*.java
java -cp ".;java_proveedor\lib\mssql-jdbc.jar" java_proveedor.src.sockets.SocketTCP
```

### Terminal 2

```powershell
cd C:\Users\Personal\Desktop\Central_TG\python_identificador
python main.py
```

### Terminal 3

```powershell
cd C:\Users\Personal\Desktop\Central_TG
dotnet run --project csharp_simulador\SimuladorTelefonico\SimuladorTelefonico.csproj
```
