# Flujo general

## Consulta de saldo

1. Usuario selecciona telefono en C#.
2. Usuario presiona `Saldo`.
3. C# arma `CONSULTA_SALDO`.
4. C# cifra datos sensibles.
5. Python valida AES, telefono, SIM, dispositivo y ubicacion.
6. Python consulta Java con `CONSULTAR_SALDO`.
7. Java consulta SQL Server.
8. Java responde JSON.
9. Python responde a C#.
10. C# muestra saldo.

## Llamada autorizada

1. Usuario marca destino.
2. C# envia `SOLICITUD_LLAMADA`.
3. Python valida datos sensibles, origen, destino y ubicacion.
4. Python consulta Java con `VERIFICAR_SALDO`.
5. Java valida servicio y saldo.
6. Java responde `OK` o `INSUF`.
7. Python responde autorizacion.
8. C# abre pantalla de llamada activa.
9. C# envia `INICIO_LLAMADA`.
10. Python registra llamada activa.

## Finalizacion

1. Usuario finaliza llamada desde C#.
2. C# envia `FINALIZAR_LLAMADA`.
3. Python elimina llamada activa.
4. Python registra historial.
5. Python descifra telefono origen.
6. Python envia `REBAJAR_SALDO` a Java.
7. Java registra llamada en SQL Server.
8. Java registra movimiento.
9. Java actualiza saldo prepago.
10. Java responde JSON.
11. Python responde a C#.

## Bitacoras

Python:

```txt
bitacora_identificador.txt
```

Java:

```txt
logs/proveedor_bitacora.txt
```
