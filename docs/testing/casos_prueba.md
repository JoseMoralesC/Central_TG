# Casos de Prueba

## Proyecto

Central Telefónica TG

## Objetivo

Documentar los escenarios mínimos que deben validarse durante el desarrollo e integración del sistema.

Estos casos permiten comprobar el correcto funcionamiento de:

- Simulador de llamadas en C#
- Identificador en Python
- Proveedor Telefónico en Java
- Bases de datos MySQL y SQL Server
- Comunicación mediante Sockets TCP

---

# Resumen de casos

| Código |    Caso                 | Resultado esperado                        |
|---     |---                      |---                                        |
| CP-001 | Llamada exitosa         | Llamada autorizada                        |
| CP-002 | Saldo insuficiente      | Llamada rechazada                         |
| CP-003 | Teléfono inactivo       | Llamada rechazada                         |
| CP-004 | SIM inválida            | Llamada rechazada                         |
| CP-005 | Ubicación inválida      | Llamada rechazada                         |
| CP-006 | Consulta de saldo       | Saldo retornado correctamente             |
| CP-007 | Finalización de llamada | Movimiento registrado y saldo actualizado |

---

# CP-001 — Llamada exitosa

## Archivo relacionado

```txt
shared/examples/llamada_exitosa.json



Entrada

Teléfono origen:

88889999

Teléfono destino:

22223333

Tipo de servicio:

PREPAGO

Condición:

Teléfono activo con saldo disponible
Resultado esperado
Código: OK
Estado: AUTORIZADA
Mensaje: Llamada autorizada correctamente
Componentes involucrados
C# Simulador -> Python Identificador -> Java Proveedor -> SQL Server
CP-002 — Saldo insuficiente
Archivo relacionado
shared/examples/saldo_insuficiente.json
Entrada

Teléfono origen:

88880000

Condición:

Teléfono prepago activo sin saldo disponible
Resultado esperado
Código: INSUF
Estado: RECHAZADA
Mensaje: Saldo insuficiente
Componentes involucrados
C# Simulador -> Python Identificador -> Java Proveedor -> SQL Server
CP-003 — Teléfono inactivo
Archivo relacionado
shared/examples/telefono_inactivo.json
Entrada

Teléfono origen:

77776666

Condición:

Teléfono registrado, pero inactivo
Resultado esperado
Código: TEL_INACTIVO
Estado: RECHAZADA
Mensaje: El teléfono origen se encuentra inactivo
Componentes involucrados
C# Simulador -> Python Identificador -> MySQL
CP-004 — SIM inválida
Archivo relacionado
shared/examples/sim_invalida.json
Condición
El teléfono existe, pero la SIM enviada no corresponde al teléfono registrado
Resultado esperado
Código: SIM_INVALIDA
Estado: RECHAZADA
Mensaje: La tarjeta SIM no corresponde al teléfono registrado
Componentes involucrados
C# Simulador -> Python Identificador -> MySQL
CP-005 — Ubicación inválida
Archivo relacionado
shared/examples/ubicacion_invalida.json
Condición
El teléfono intenta realizar la llamada desde fuera de Costa Rica
Resultado esperado
Código: UBICACION_INVALIDA
Estado: RECHAZADA
Mensaje: La ubicación enviada no pertenece al territorio nacional permitido
Componentes involucrados
C# Simulador -> Python Identificador
CP-006 — Consulta de saldo
Archivo relacionado
shared/examples/consulta_saldo_exitosa.json
Entrada

Código especial:

#9090*

Teléfono origen:

88889999
Resultado esperado
Código: OK
Estado: CONSULTA_EXITOSA
Saldo disponible: 5000.00 CRC
Componentes involucrados
C# Simulador -> Python Identificador -> Java Proveedor -> SQL Server
CP-007 — Finalización de llamada
Archivo relacionado
shared/examples/finalizar_llamada_exitosa.json
Condición
Existe una llamada activa que finaliza correctamente
Resultado esperado
Código: OK
Estado: LLAMADA_FINALIZADA
Movimiento registrado
Saldo actualizado
Componentes involucrados
C# Simulador -> Python Identificador -> Java Proveedor -> SQL Server
Consideraciones generales
Todos los mensajes deben enviarse en formato JSON.
La comunicación debe realizarse mediante Socket TCP.
Las tramas deben enviarse con codificación UTF-8.
Cada mensaje debe finalizar con salto de línea \n.
Los resultados deben coincidir con los contratos definidos en shared/contracts/.
Los datos de prueba deben coincidir con los scripts seed.
Los errores deben registrarse en la bitácora correspondiente.