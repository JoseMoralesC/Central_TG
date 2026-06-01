# Proveedor Telefónico - Java + SQL Server

## Objetivo

Este componente representa el sistema interno del proveedor telefónico XYZ.

Recibe solicitudes desde el Identificador Python y determina si una operación telefónica puede realizarse.

---

# Responsabilidades

- Escuchar solicitudes mediante Socket TCP.
- Recibir tramas JSON.
- Validar servicios telefónicos.
- Consultar tipo de servicio.
- Validar saldo disponible.
- Calcular tarifas.
- Autorizar o rechazar llamadas.
- Registrar llamadas realizadas.
- Registrar movimientos de saldo.
- Registrar bitácoras del proveedor.

---

# Base de datos

Motor:

```txt
SQL Server
```

Base:

```txt
CentralProveedor
```

Scripts:

```txt
database/sqlserver_proveedor/schema/

database/sqlserver_proveedor/seed/
```

---

# Socket TCP

El proveedor escucha solicitudes desde Python en:

```txt
Host: 127.0.0.1

Puerto: 6000
```

---

# Comunicación

Recibe desde:

```txt
Python Identificador
```

Responde hacia:

```txt
Python Identificador
```

---

# Contratos utilizados

Entrada:

```txt
shared/contracts/consulta_proveedor.json

shared/contracts/finalizar_llamada.json
```

Salida:

```txt
shared/contracts/respuesta_proveedor.json
```

---

# Configuración local

Cada integrante debe crear su archivo:

```bash
cp ../.env.example ../.env
```

Configurar:

```env
SQLSERVER_HOST=100.88.25.17

SQLSERVER_PORT=49172

SQLSERVER_DATABASE=CentralProveedor

SQLSERVER_USER=usuario_sqlserver

SQLSERVER_PASSWORD=tu_password
```

---

# Tipos de respuesta

## Operación correcta

```json
{
  "codigo": "OK",
  "mensaje": "Servicio autorizado"
}
```

---

## Saldo insuficiente

```json
{
  "codigo": "INSUF",
  "mensaje": "Saldo insuficiente"
}
```

---

## Error

```json
{
  "codigo": "ERROR",
  "mensaje": "Servicio no disponible"
}
```

---

# Flujo verificar saldo

```txt
Python
 |
 | consulta_proveedor.json
 ↓

Java

 ↓

SQL Server

 ↓

Java

 |
 | respuesta_proveedor.json
 ↓

Python
```

---

# Servicios soportados

## Prepago

Debe validar:

- Servicio activo.
- Saldo disponible.
- Tarifa.
- Tiempo máximo permitido.

---

## Postpago

Debe validar:

- Servicio activo.
- Registrar consumo.

Retorna:

```txt
saldo = -1
```

---

# Casos de prueba relacionados

```txt
shared/examples/llamada_exitosa.json

shared/examples/saldo_insuficiente.json

shared/examples/finalizar_llamada_exitosa.json
```

---

# Estado inicial

Pendiente de implementación.

Antes de programar verificar:

- SQL Server accesible.
- Base creada.
- Seed ejecutado.
- `.env` configurado.
- Puerto 6000 disponible.
- Contratos revisados.