# Pruebas

## Documento principal

La matriz oficial esta en:

```txt
docs/testing/pruebas_integradas_oficiales.md
```

## Validaciones locales

```powershell
dotnet build csharp_simulador\SimuladorTelefonico.slnx
python -m compileall python_identificador
javac java_proveedor\Main.java java_proveedor\src\sockets\SocketTCP.java java_proveedor\src\sockets\ManejoCliente.java java_proveedor\src\services\*.java java_proveedor\src\database\*.java java_proveedor\src\models\*.java java_proveedor\src\config\*.java
```

## Pruebas funcionales minimas

### Consulta saldo

Flujo:

```txt
C# -> Python -> Java -> SQL Server -> Java -> Python -> C#
```

Resultado esperado:

- Saldo mostrado en C#.
- Bitacora en Python.
- Bitacora en Java.

### Llamada completa

Resultado esperado:

- Llamada autorizada.
- Llamada activa registrada.
- Finalizacion enviada.
- Llamada registrada en SQL Server.
- Movimiento creado.
- Saldo prepago actualizado.

### Errores

Validar:

- saldo insuficiente;
- telefono inactivo;
- SIM invalida;
- ubicacion invalida;
- AES invalido.

## Consultas de verificacion

SQL Server:

```sql
SELECT TOP 10 * FROM llamadas_proveedor ORDER BY llamada_id DESC;
SELECT TOP 10 * FROM movimientos_saldo ORDER BY movimiento_id DESC;
SELECT * FROM saldos WHERE servicio_id = 1;
```

MySQL:

```sql
SELECT * FROM llamadas_activas;
SELECT * FROM historial_llamadas_identificador ORDER BY historial_id DESC;
```
