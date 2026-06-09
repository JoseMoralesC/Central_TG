# Protocolo de Comunicación TCP

## Proyecto

Central Telefónica

## Objetivo

Definir el estándar de comunicación entre los tres componentes del sistema:

- Simulador de llamadas en C#
- Identificador en Python
- Proveedor Telefónico en Java

---

## Arquitectura general

```txt
Simulador C#
    |
    | JSON por Socket TCP
    v
Identificador Python
    |
    | JSON por Socket TCP
    v
Proveedor Java



## Componentes

### 1. Simulador de llamadas

Tecnología:

* C#

Responsabilidad:

* Capturar datos de llamada.
* Armar tramas JSON.
* Enviar solicitudes al Identificador.
* Mostrar respuestas al usuario.

### 2. Identificador

Tecnología:

* Python
* MySQL

Responsabilidad:

* Recibir solicitudes del simulador.
* Validar teléfono, tarjeta, dispositivo y ubicación.
* Consultar al proveedor.
* Registrar llamadas activas.
* Guardar bitácora.
* Responder al simulador.

### 3. Proveedor Telefónico

Tecnología:

* Java
* SQL Server

Responsabilidad:

* Validar saldo.
* Verificar servicio activo.
* Calcular tarifas.
* Registrar movimientos.
* Guardar bitácora.
* Responder al Identificador.

---

## Puertos definidos

| Componente    | Tecnología | Puerto | Función                         |
| ------------- | ---------: | -----: | ------------------------------- |
| Identificador |     Python |   5000 | Recibe solicitudes desde C#     |
| Proveedor     |       Java |   6000 | Recibe solicitudes desde Python |

---

## Direcciones de prueba

Para pruebas locales:

```txt
Identificador Python: 127.0.0.1:5000
Proveedor Java: 127.0.0.1:6000
```

Para pruebas remotas por Tailscale:

```txt
Identificador Python: IP_TAILSCALE_PYTHON:5000
Proveedor Java: IP_TAILSCALE_JAVA:6000
```

---

## Formato de codificación

Todas las tramas deben enviarse como texto JSON usando:

```txt
UTF-8
```

El contrato JSON integrado vigente del equipo se detalla en:

```txt
docs/arquitectura/contrato_json_integrado.md
```

---

## Regla de cierre de mensaje

Cada mensaje debe finalizar con salto de línea:

```txt
\n
```

Esto permite leer la trama completa desde el socket.

---

## Flujo 1: Solicitud de llamada

```txt
C# -> Python
```

Archivo de contrato:

```txt
shared/contracts/solicitud_llamada.json
```

Respuesta:

```txt
shared/contracts/respuesta_llamada.json
```

---

## Flujo 2: Consulta al proveedor

```txt
Python -> Java
```

Archivo de contrato:

```txt
shared/contracts/consulta_proveedor.json
```

Respuesta:

```txt
shared/contracts/respuesta_proveedor.json
```

---

## Flujo 3: Inicio de llamada

```txt
C# -> Python
```

Archivo de contrato:

```txt
shared/contracts/inicio_llamada.json
```

---

## Flujo 4: Finalización de llamada

```txt
C# -> Python -> Java
```

Archivo de contrato:

```txt
shared/contracts/finalizar_llamada.json
```

---

## Flujo 5: Consulta de saldo

```txt
C# -> Python -> Java -> Python -> C#
```

Solicitud:

```txt
shared/contracts/consulta_saldo.json
```

Respuesta:

```txt
shared/contracts/respuesta_saldo.json
```

---

## Códigos de respuesta

| Código               | Significado              |
| -------------------- | ------------------------ |
| OK                   | Operación correcta       |
| INSUF                | Saldo insuficiente       |
| ERROR                | Error general            |
| TEL_INACTIVO         | Teléfono inactivo        |
| SIM_INVALIDA         | Tarjeta SIM no válida    |
| DISPOSITIVO_INVALIDO | Dispositivo no válido    |
| UBICACION_INVALIDA   | Ubicación fuera del país |

---

## Reglas generales

1. Todo mensaje debe ser JSON válido.
2. Todo mensaje debe tener `tipo_transaccion`.
3. Todo mensaje debe enviarse en UTF-8.
4. Todo mensaje debe terminar con `\n`.
5. Ningún componente debe modificar los contratos sin avisar al equipo.
6. Los puertos definidos deben mantenerse durante toda la integración.
7. Los logs y bitácoras deben registrar tramas de entrada y salida.

---

## Ejemplo de envío

```json
{
  "tipo_transaccion": "SOLICITUD_LLAMADA",
  "telefono_origen": "88889999",
  "telefono_destino": "22223333",
  "tipo_llamada": "NACIONAL"
}
```

---

## Ejemplo de respuesta

```json
{
  "tipo_transaccion": "RESPUESTA_LLAMADA",
  "resultado": {
    "codigo": "OK",
    "estado": "AUTORIZADA",
    "mensaje": "Llamada autorizada correctamente"
  }
}
