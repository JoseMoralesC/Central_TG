# Pruebas integradas oficiales

## Objetivo

Validar el cierre funcional del sistema distribuido:

```txt
C# Simulador -> Python Identificador -> Java Proveedor -> SQL Server
```

## Precondiciones

- MySQL `central_identificador` disponible.
- SQL Server `CentralProveedor` disponible.
- Migracion AES aplicada:

```txt
database/mysql_identificador/migrations/003_update_identificador_seed_values_to_aes.sql
```

- Java Proveedor levantado en puerto `6000`.
- Python Identificador levantado en puerto `5000`.
- C# Simulador configurado con la misma llave AES.

## Caso 1 - Consulta saldo

### Pasos

1. Levantar Java.
2. Levantar Python.
3. Abrir C#.
4. Seleccionar telefono `88889999`.
5. Presionar `Saldo`.
6. Confirmar codigo `#9090*`.

### Resultado esperado

- C# muestra saldo.
- Python valida telefono, SIM, dispositivo y ubicacion.
- Java consulta SQL Server.
- Python escribe bitacora.
- Java escribe bitacora.

### Verificacion

```sql
SELECT * FROM servicios WHERE numero_telefono = '88889999';
SELECT * FROM saldos WHERE servicio_id = 1;
```

Archivos:

```txt
bitacora_identificador.txt
logs/proveedor_bitacora.txt
```

## Caso 2 - Llamada completa

### Pasos

1. Seleccionar telefono prepago activo `88889999`.
2. Marcar destino `22223333`.
3. Solicitar llamada.
4. Confirmar autorizacion `OK`.
5. Esperar algunos segundos.
6. Finalizar llamada.

### Resultado esperado

- C# abre llamada activa.
- Python registra llamada activa.
- Python finaliza llamada.
- Java registra llamada.
- Java registra movimiento.
- Java rebaja saldo prepago.
- C# muestra finalizacion correcta.

### Verificacion SQL Server

```sql
SELECT TOP 10 *
FROM llamadas_proveedor
ORDER BY llamada_id DESC;

SELECT TOP 10 *
FROM movimientos_saldo
ORDER BY movimiento_id DESC;

SELECT *
FROM saldos
WHERE servicio_id = 1;
```

### Verificacion MySQL

```sql
SELECT *
FROM llamadas_activas;

SELECT *
FROM historial_llamadas_identificador
ORDER BY historial_id DESC;
```

## Caso 3 - Saldo insuficiente

### Pasos

1. Seleccionar telefono `88880000`.
2. Marcar destino `22223333`.
3. Solicitar llamada.

### Resultado esperado

- Java responde `INSUF`.
- Python devuelve rechazo.
- C# no abre llamada activa.
- No se registra movimiento de cobro.

## Caso 4 - Telefono bloqueado o inactivo

### Pasos

1. Seleccionar o enviar telefono inactivo `77776666`.
2. Solicitar llamada o consulta saldo.

### Resultado esperado

- Python responde `TEL_INACTIVO`.
- No consulta proveedor para cobro.

## Caso 5 - SIM invalida

### Pasos

1. Enviar una trama con telefono correcto y SIM que no corresponde.

### Resultado esperado

- Python responde `SIM_INVALIDA`.
- No consulta proveedor.

## Caso 6 - Ubicacion incorrecta

### Pasos

1. Enviar una trama con coordenadas fuera de Costa Rica.

### Resultado esperado

- Python responde `UBICACION_INVALIDA`.
- No consulta proveedor.

## Caso 7 - Error AES

### Pasos

1. Enviar `telefono_origen`, `identificador_dispositivo` o `identificador_tarjeta` sin cifrado AES valido.

### Resultado esperado

- Python responde error de seguridad.
- No usa fallback a texto plano.
- No consulta proveedor.

## Validaciones locales realizadas

```powershell
dotnet build csharp_simulador\SimuladorTelefonico.slnx
python -m compileall python_identificador
javac java_proveedor\Main.java java_proveedor\src\sockets\SocketTCP.java java_proveedor\src\sockets\ManejoCliente.java java_proveedor\src\services\*.java java_proveedor\src\database\*.java java_proveedor\src\models\*.java java_proveedor\src\config\*.java
```

Resultado:

- C# compila correctamente.
- Python compila correctamente.
- Java compila correctamente.
