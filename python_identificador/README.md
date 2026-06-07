# Identificador - Python + MySQL

## Objetivo

Este componente recibe las solicitudes desde el simulador C# y valida la información del teléfono antes de consultar al proveedor telefónico Java.

## Responsabilidades

- Recibir tramas JSON por Socket TCP.
- Validar teléfono origen.
- Validar tarjeta SIM.
- Validar dispositivo.
- Validar ubicación nacional.
- Consultar al proveedor Java.
- Registrar llamadas activas.
- Registrar historial de llamadas.
- Registrar bitácoras.
- Cifrar datos sensibles con AES.

## Base de datos

Motor:

```txt
MySQL


Base:

central_identificador

Scripts:

database/mysql_identificador/schema/
database/mysql_identificador/seed/
Socket

El Identificador escucha solicitudes del simulador en:

Host: 127.0.0.1
Puerto: 5000
Comunicación

Recibe desde:

C# Simulador

Envía hacia:

Java Proveedor
Contratos utilizados
shared/contracts/solicitud_llamada.json
shared/contracts/inicio_llamada.json
shared/contracts/finalizar_llamada.json
shared/contracts/consulta_saldo.json
shared/contracts/respuesta_llamada.json
shared/contracts/respuesta_saldo.json
shared/contracts/consulta_proveedor.json
shared/contracts/respuesta_proveedor.json
Configuración local

Cada integrante debe copiar:

cp ../.env.example ../.env

Luego configurar sus datos reales:

MYSQL_HOST=100.114.84.5
MYSQL_PORT=3306
MYSQL_DATABASE=central_identificador
MYSQL_USER=usuario_mysql
MYSQL_PASSWORD=tu_password
Flujo principal
C# Simulador
    ↓
Python Identificador
    ↓
MySQL
    ↓
Java Proveedor
Casos de prueba relacionados
shared/examples/llamada_exitosa.json
shared/examples/saldo_insuficiente.json
shared/examples/telefono_inactivo.json
shared/examples/sim_invalida.json
shared/examples/ubicacion_invalida.json
shared/examples/consulta_saldo_exitosa.json
shared/examples/finalizar_llamada_exitosa.json