# Arquitectura

## Vision general

El sistema implementa una central telefonica distribuida con tres componentes:

```txt
C# Simulador Telefonico
  -> JSON por socket TCP
Python Identificador + MySQL
  -> JSON por socket TCP
Java Proveedor Telefonico + SQL Server
```

## Componentes

### C# Simulador

Responsabilidades:

- Mostrar telefonos virtuales.
- Marcar numero destino.
- Consultar saldo con `#9090*`.
- Iniciar llamada.
- Finalizar llamada.
- Cifrar datos sensibles con AES.
- Enviar tramas JSON al Identificador.

### Python Identificador

Responsabilidades:

- Escuchar solicitudes desde C#.
- Validar telefono, SIM, dispositivo y ubicacion.
- Administrar llamadas activas.
- Finalizar llamadas.
- Consultar proveedor Java.
- Registrar bitacora del identificador.

### Java Proveedor

Responsabilidades:

- Escuchar solicitudes desde Python.
- Verificar saldo.
- Consultar saldo.
- Registrar llamadas.
- Registrar movimientos.
- Rebajar saldo prepago.
- Registrar bitacora del proveedor.

## Bases de datos

### MySQL

Base:

```txt
central_identificador
```

Tablas principales:

- `proveedores`
- `telefonos`
- `tarjetas_telefonicas`
- `dispositivos`
- `llamadas_activas`
- `historial_llamadas_identificador`
- `bitacora_identificador`

### SQL Server

Base:

```txt
CentralProveedor
```

Tablas principales:

- `clientes`
- `servicios`
- `tarifas`
- `saldos`
- `llamadas_proveedor`
- `movimientos_saldo`
- `bitacora_proveedor`

## Contrato de comunicacion

El contrato oficial del equipo es JSON:

```txt
docs/arquitectura/contrato_json_integrado.md
```

Reglas:

- UTF-8.
- Un JSON por mensaje.
- Cada mensaje termina en `\n`.
- Respuestas con `status` y `resultado.codigo`.

## Seguridad

Los datos sensibles viajan cifrados con AES:

- telefono origen;
- identificador del dispositivo;
- identificador de tarjeta SIM.

Configuracion esperada:

```env
AES_KEY=ClaveSecreta1234
AES_IV=VectorInicio1234
```
