# Roadmap detallado de próximos pasos

---

# Paso 1: Definir contrato Python -> Java en texto plano

## Objetivo

Cumplir el PDF y eliminar la incompatibilidad actual entre Python y Java.

---

## Estado actual

Actualmente:

- Python envía JSON desde:

```txt
proveedor_cliente.py
```

- Java lee un string y busca campos tipo JSON manualmente.
- El PDF exige texto plano para el Proveedor.
- Python envía acciones como:

```txt
VERIFICAR_SALDO
REGISTRO_MOVIMIENTO
```

- Java espera otras acciones.

---

# Contratos sugeridos

## Contrato para verificar saldo

```txt
1|88889999|1
```

Donde:

```txt
1 = tipo transacción llamada
88889999 = teléfono origen
1 = mismo proveedor
```

---

## Contrato para consulta saldo

```txt
2|88889999|1
```

---

## Contrato para registrar movimiento

```txt
1|88889999|20250607|101025|22223333|00001000|000125
```

---

# Acciones concretas

## Python

Actualizar:

```txt
python_identificador/app/services/proveedor_cliente.py
```

Cambiar:

- Dejar de enviar JSON.
- Construir string plano.

---

## Java

Actualizar:

```txt
ManejoCliente.java
```

Implementar:

- Parsear por separador:

```txt
|
```

- Responder texto simple o JSON acordado.

Idealmente usar respuesta plana según PDF:

Respuesta correcta:

```txt
OK|0000001000|001025
```

Saldo insuficiente:

```txt
INSUF
```

Error:

```txt
ERROR
```

---

# Resultado esperado

Al finalizar este paso:

- Python y Java hablan el mismo protocolo.
- PROVEEDOR1 puede responder saldo real.
- Identificador1 e Identificador4 dejan de fallar por contrato.

---

## Prioridad

```txt
Alta
```

---

<br>

# Paso 2: Completar RegistrarMovimiento.java

## Objetivo

Cerrar PROVEEDOR2 y permitir que IDENTIFICADOR3 funcione.

---

# Estado actual

Actualmente:

- `RegistrarMovimiento.java` está vacío.
- Python intenta registrar movimientos al finalizar llamada.
- Java no guarda llamada.
- Java no rebaja saldo.

---

# Tareas concretas

Crear método en:

```java
RegistrarMovimiento.java
```

Método:

```java
public String registrarMovimiento(String tramaPlano)
```

---

# Parsear datos recibidos

Datos a obtener:

- Tipo transacción.
- Teléfono origen.
- Fecha llamada.
- Hora llamada.
- Teléfono destino.
- Costo.
- Duración.

---

# Lógica requerida

## Validación servicio

Buscar servicio por teléfono en:

```txt
SQL Server
```

---

## Registrar llamada

Insertar datos en:

```txt
llamadas_proveedor
```

---

## Registrar movimiento

Insertar datos en:

```txt
movimientos_saldo
```

---

## Actualizar saldo

Si el servicio es:

```txt
Prepago
```

Actualizar:

```txt
saldos
```

---

# Respuestas esperadas

Correcto:

```txt
OK
```

Error:

```txt
ERROR
```

---

# Archivos probables

```txt
java_proveedor/src/services/RegistrarMovimiento.java

java_proveedor/src/database/ServicioDAO.java

java_proveedor/src/database/LlamadaProveedorDAO.java

java_proveedor/src/database/TarifaDAO.java

java_proveedor/src/sockets/ManejoCliente.java
```

---

# Resultado esperado

Al finalizar este paso:

- Al finalizar llamada desde C#, Python avisa a Java.
- Java registra llamada.
- Java rebaja saldo si es prepago.
- Java responde:

```txt
OK
```

---

## Prioridad

```txt
Alta
```

---

<br>

# Paso 3: Regenerar datos MySQL cifrados con AES real

## Objetivo

Que C#, Python y MySQL usen exactamente el mismo cifrado.

---

# Problema actual

Actualmente:

- C# envía AES real en Base64.
- Python busca en MySQL usando el valor cifrado recibido.

El seed actual de MySQL tiene valores simulados:

```txt
ENC_88889999

ENC_SIM_1234567891234567891

ENC_IMEI_1234567891234567
```

Esto no coincide con AES real.

---

# Tareas concretas

## Definir llave única

Utilizar:

```env
AES_KEY=ClaveSecreta12345

AES_IV=VectorInicio1234
```

---

# Algoritmo estándar

Todos los sistemas deben usar:

```txt
AES-CBC

PKCS7

UTF-8

Base64
```

---

# Generar valores cifrados reales

Generar cifrado para:

- Teléfonos.
- Identificadores de dispositivo.
- Identificadores de tarjeta.

---

# Actualizar seed MySQL

Modificar:

```txt
database/mysql_identificador/seed/001_seed_identificador.sql
```

Agregar los valores cifrados reales.

---

# Actualizar C#

Actualizar los teléfonos del simulador C# para que coincidan con:

- Datos reales.
- Datos cifrados.
- Datos de prueba.

---

# Actualización de base de datos

Volver a ejecutar:

```txt
seed MySQL
```

---

# Resultado esperado

Al finalizar:

- C# envía teléfono cifrado.
- Python recibe el dato cifrado.
- Python consulta MySQL usando ese valor.
- MySQL encuentra el registro.

Empiezan a funcionar:

- Validaciones de teléfono.
- Validaciones SIM.
- Validaciones dispositivo.

---

## Prioridad

```txt
Alta
```

---

<br>

# Orden práctico recomendado de implementación

Ejecutar en este orden:

## 1. Corregir servidor Python

Corregir:

```txt
main.py
```

Objetivo:

- Levantar el servidor correcto.

---

## 2. Alinear contrato Python -> Java

Implementar:

```txt
Texto plano
```

Entre:

```txt
Python <-> Java
```

---

## 3. Completar RegistrarMovimiento.java

Finalizar registro de:

- Llamadas.
- Movimientos.
- Rebajo de saldo.

---

## 4. Regenerar seed MySQL con AES real

Actualizar:

```txt
001_seed_identificador.sql
```

---

## 5. Pruebas funcionales

Ejecutar:

### Consulta saldo

```txt
#9090*
```

---

### Llamada autorizada

Validar flujo completo:

```txt
C#
 ↓
Python
 ↓
Java
 ↓
SQL Server
```

---

### Finalizar llamada

Validar:

- Registro.
- Cobro.
- Movimiento.
- Respuesta final.

---

## 6. Revisar bitácoras

Validar:

- Logs.
- Errores.
- Transacciones.

---

## 7. Documentar pruebas

Actualizar:

```txt
docs/testing/casos_prueba.md
```

Registrar:

- Casos exitosos.
- Casos fallidos.
- Correcciones realizadas.

---

# Estado esperado al completar roadmap

Al finalizar todos los pasos:

- C# simula teléfonos correctamente.
- Python identifica dispositivos y servicios.
- Java administra proveedor correctamente.
- MySQL valida identificadores cifrados.
- SQL Server administra servicios, llamadas y saldos.
- Comunicación cumple contrato solicitado.
- Proyecto queda listo para pruebas finales.