# Informe de analisis tecnico del proyecto

## 1. Resumen ejecutivo

El proyecto esta parcialmente funcional como integracion demo. Actualmente puede ejecutar el flujo C# -> Python -> Java para autorizacion, inicio, consulta de saldo y finalizacion simulada, pero no cumple completamente el alcance tecnico porque faltan piezas criticas en Java: PROVEEDOR2 registrar movimientos reales y PROVEEDOR3 bitacora del proveedor.

Estado general: funciona parcialmente.

Compilacion verificada:

- C# WinForms: compila correctamente.
- Python Identificador: compila correctamente.
- Java Proveedor: compila correctamente.

Hallazgo principal: la integracion ya responde, pero todavia hay respuestas simuladas y contratos inconsistentes, especialmente entre Python y Java.

## 2. Estado global del sistema

El sistema si puede funcionar como demo integrada basica:

```txt
C# Simulador -> Python Identificador -> Java Proveedor -> SQL Server
```

Componentes que se comunican:

- C# conecta al socket Python en `127.0.0.1:5000`.
- Python recibe JSON, valida en MySQL y consulta Java.
- Java recibe solicitudes en `127.0.0.1:6000`.
- Java consulta SQL Server para saldo, servicio y tarifa.

Componentes incompletos:

- Java no registra movimientos reales.
- Java no tiene bitacora del proveedor.
- Python no persiste historial final de llamadas.
- Scripts MySQL versionados todavia tienen datos `ENC_*`, no AES real.
- Comunicacion Python -> Java usa JSON, aunque el requisito indica texto plano para proveedor.

Incumplimientos tecnologicos:

- Python -> Java no usa texto plano; usa JSON.
- AES existe, pero Python permite fallback a datos planos si falla el descifrado.
- Bitacora del proveedor no esta implementada.
- Registro de movimientos en SQL Server no esta implementado.

## 3. Analisis por historias de usuario

### PROVEEDOR1

Estado actual: funciona parcialmente.

Evidencia:

- `java_proveedor/src/services/VerificarSaldo.java`
- `java_proveedor/src/services/ConsultaSaldo.java`
- `java_proveedor/src/database/ServicioDAO.java`
- `database/sqlserver_proveedor/schema/001_create_database_proveedor.sql`

Flujo detectado:

- Java recibe accion `VERIFICAR_SALDO`.
- Busca servicio en SQL Server.
- Valida linea activa.
- Si es postpago autoriza.
- Si es prepago valida saldo y calcula tiempo maximo.

Criterios cumplidos:

- Consulta SQL Server.
- Valida servicio activo.
- Diferencia prepago y postpago.
- Responde `OK`, `INSUF` o `ERROR`.

Criterios faltantes:

- Respuesta formal alineada al contrato final.
- Comunicacion en texto plano, si ese es el requisito obligatorio.
- Validacion robusta de tarifas inactivas.

Errores, riesgos o inconsistencias:

- Java parsea JSON con busqueda manual de strings, no con parser real.
- El proveedor depende de `mssql-jdbc.jar` en classpath.

Que falta desarrollar:

- Normalizar contrato de entrada y salida.
- Reemplazar parseo manual por parser o formato de texto plano claramente definido.
- Validar tarifa activa y errores de SQL Server con respuestas consistentes.

Prioridad recomendada: alta.

### PROVEEDOR2

Estado actual: no implementado / simulado.

Evidencia:

- `java_proveedor/src/services/RegistrarMovimiento.java` esta vacio.
- `java_proveedor/src/database/LlamadaProveedorDAO.java` esta vacio.
- `java_proveedor/src/sockets/ManejoCliente.java` responde `OK` para movimientos sin registrar nada.

Flujo detectado:

- Python envia finalizacion o registro de movimiento al proveedor.
- Java acepta `FINALIZAR_LLAMADA`, `REGISTRO_MOVIMIENTO` y `REBAJAR_SALDO`.
- Java devuelve `OK`, pero no escribe en SQL Server.

Criterios cumplidos:

- El socket acepta acciones relacionadas con movimiento.
- Hay tablas de BD para llamadas y movimientos.

Criterios faltantes:

- Insertar en `llamadas_proveedor`.
- Insertar en `movimientos_saldo`.
- Rebajar saldo prepago.
- Registrar cobro postpago.
- Responder error real si falla SQL Server.

Errores, riesgos o inconsistencias:

- IDENTIFICADOR3 puede parecer finalizado, pero el proveedor no cobra ni registra.
- La demo puede pasar visualmente, pero no cumple base de datos.

Que falta desarrollar:

- Implementar `RegistrarMovimiento.java`.
- Implementar DAO para llamadas, movimientos y actualizacion de saldos.
- Integrar esa logica desde `ManejoCliente.java`.

Prioridad recomendada: alta.

### PROVEEDOR3

Estado actual: no implementado.

Evidencia:

- Existe tabla `bitacora_proveedor`.
- No existe servicio de bitacora Java.
- No hay cola ni hilo independiente en Java.
- No se encontro escritura real a `bitacora_proveedor`.

Criterios cumplidos:

- Solo existe estructura de tabla.

Criterios faltantes:

- Archivo de texto de bitacora.
- Formato JSON.
- Registro de entrada y salida.
- Cola.
- Hilo independiente.
- Persistencia o archivo segun requerimiento.

Errores, riesgos o inconsistencias:

- Historia completa sin implementacion funcional.
- No hay evidencia auditable de tramas del proveedor.

Que falta desarrollar:

- Crear servicio de bitacora Java.
- Encolar entrada/salida desde `ManejoCliente.java`.
- Crear hilo escritor y archivo `.txt`.
- Opcionalmente persistir tambien en `bitacora_proveedor`.

Prioridad recomendada: alta.

### IDENTIFICADOR1

Estado actual: funciona parcialmente.

Evidencia:

- `python_identificador/app/services/autorizacion_llamada.py`
- `python_identificador/app/database/repositorio.py`
- `python_identificador/app/utils/crypto.py`

Flujo detectado:

- Recibe `SOLICITUD_LLAMADA`.
- Valida campos, ubicacion, telefono, SIM y dispositivo.
- Consulta al proveedor Java.
- Responde autorizacion o rechazo.

Criterios cumplidos:

- Recibe JSON desde C#.
- Valida MySQL.
- Valida ubicacion en Costa Rica.
- Valida SIM y dispositivo.
- Consulta proveedor Java.
- Responde `OK`, `INSUF` o `ERROR`.

Criterios faltantes:

- No debe permitir fallback a datos planos si AES falla.
- Codigo de dispositivo invalido deberia usar `DISPOSITIVO_INVALIDO`, no `ERROR`.
- Contrato Python -> Java no cumple texto plano.

Errores, riesgos o inconsistencias:

- El fallback a texto plano debilita el requisito de AES.
- La respuesta del proveedor no esta totalmente normalizada.

Que falta desarrollar:

- Rechazar tramas que no puedan descifrarse.
- Ajustar codigos de error.
- Congelar contrato real con Java.

Prioridad recomendada: alta.

### IDENTIFICADOR2

Estado actual: funciona parcialmente.

Evidencia:

- `python_identificador/app/services/iniciar_llamada.py`
- Tabla `llamadas_activas`.

Flujo detectado:

- Recibe `INICIO_LLAMADA`.
- Mantiene lista en memoria con lock.
- Calcula hora maxima.
- Inserta llamada activa en MySQL si resuelve `telefono_id`.
- Inicia monitoreo de llamadas activas.

Criterios cumplidos:

- Registro de llamada activa en memoria.
- Uso de lock para acceso concurrente.
- Calculo de hora maxima.
- Persistencia parcial en MySQL.

Criterios faltantes:

- No persiste `id_llamada` en la tabla.
- La lista en memoria es la fuente primaria; si Python reinicia se pierde estado.
- No actualiza ni elimina llamada activa en BD al finalizar.

Errores, riesgos o inconsistencias:

- Estado duplicado entre memoria y base de datos.
- Persistencia incompleta para auditoria real.

Que falta desarrollar:

- Agregar campo o referencia de `id_llamada` en BD.
- Eliminar o cerrar llamada activa en MySQL al finalizar.
- Definir fuente de verdad: memoria, BD o ambas sincronizadas.

Prioridad recomendada: media.

### IDENTIFICADOR3

Estado actual: funciona parcialmente.

Evidencia:

- `python_identificador/app/services/termina_llamada.py`
- `python_identificador/app/services/proveedor_cliente.py`

Flujo detectado:

- Recibe `FINALIZAR_LLAMADA`.
- Elimina llamada de lista en memoria.
- Calcula/formatea datos para proveedor.
- Envia movimiento al proveedor.

Criterios cumplidos:

- Finalizacion manual desde C#.
- Finalizacion automatica por vencimiento prevista en hilo.
- Envio de datos al proveedor.

Criterios faltantes:

- Java no registra movimiento real.
- Python no inserta historial en `historial_llamadas_identificador`.
- No elimina registro de `llamadas_activas` en MySQL.
- Finalizacion automatica depende de movimiento simulado.

Errores, riesgos o inconsistencias:

- Historia depende directamente de PROVEEDOR2.
- Puede devolver OK aunque SQL Server no haya sido actualizado.

Que falta desarrollar:

- Completar PROVEEDOR2.
- Registrar historial en MySQL.
- Sincronizar cierre en `llamadas_activas`.

Prioridad recomendada: alta.

### IDENTIFICADOR4

Estado actual: funciona parcialmente.

Evidencia:

- `csharp_simulador/SimuladorTelefonico/UI/ConsultaSaldoForm.cs`
- `csharp_simulador/SimuladorTelefonico/Models/ConsultaSaldo.cs`
- `python_identificador/app/services/consulta.py`
- `java_proveedor/src/services/ConsultaSaldo.java`

Flujo detectado:

- C# detecta `#9090*`.
- C# arma `CONSULTA_SALDO`.
- Python valida datos contra MySQL.
- Python consulta Java.
- Java devuelve saldo.
- C# muestra saldo recibido.

Criterios cumplidos:

- Detecta `#9090*`.
- Incluye ubicacion.
- Cifra telefono, dispositivo y tarjeta.
- Envia al Identificador.
- Recibe y muestra saldo.

Criterios faltantes:

- Python no valida dispositivo en consulta de saldo, solo telefono y tarjeta.
- Tipo de servicio en respuesta se fija como `PREPAGO`.
- Python -> Java usa JSON.

Errores, riesgos o inconsistencias:

- Respuesta de saldo puede ser conceptualmente incorrecta para postpago.
- Falta validacion completa de dispositivo.

Que falta desarrollar:

- Validar dispositivo en `consulta.py`.
- Devolver tipo de servicio real.
- Normalizar contrato con Java.

Prioridad recomendada: alta.

### IDENTIFICADOR5

Estado actual: funciona parcialmente.

Evidencia:

- `python_identificador/app/sockets/servidor.py`
- `python_identificador/app/sockets/handler.py`
- `python_identificador/bitacora_identificador.txt`

Flujo detectado:

- Handler encola tramas de entrada y salida.
- Servidor inicia hilo de bitacora.
- Hilo escribe archivo `bitacora_identificador.txt`.

Criterios cumplidos:

- Usa cola `queue.Queue`.
- Usa hilo independiente.
- Registra entrada y salida.
- Escribe archivo `.txt`.
- Contenido en JSON por linea.

Criterios faltantes:

- No escribe en tabla `bitacora_identificador`.
- Para tramas anidadas como `FINALIZAR_LLAMADA`, varios campos quedan vacios porque busca solo en raiz.
- No hay rotacion ni manejo de tamano de archivo.

Errores, riesgos o inconsistencias:

- Datos auditables incompletos para tramas anidadas.
- La tabla de bitacora queda sin uso.

Que falta desarrollar:

- Extraer campos desde `datos_llamada` cuando aplique.
- Persistir tambien en MySQL si el criterio lo exige.
- Mejorar formato y manejo de errores.

Prioridad recomendada: media.

### SIM1

Estado actual: funciona parcialmente.

Evidencia:

- `csharp_simulador/SimuladorTelefonico/Form1.cs`
- `csharp_simulador/SimuladorTelefonico/UI/MarcarNumeroForm.cs`
- `csharp_simulador/SimuladorTelefonico/UI/LlamadaActivaForm.cs`
- `csharp_simulador/SimuladorTelefonico/Socket/TcpSocketClient.cs`
- `csharp_simulador/SimuladorTelefonico/Services/CryptoService.cs`

Flujo detectado:

- Pantalla principal muestra telefonos.
- Permite marcar numero.
- Permite consultar saldo.
- Envia JSON al socket Python.
- Muestra respuesta.
- Abre pantalla de llamada activa cuando hay autorizacion.
- Permite finalizar llamada.

Criterios cumplidos:

- Interfaz con varios telefonos.
- Pantalla de marcado.
- Pantalla de saldo.
- Pantalla de llamada activa.
- Finalizacion manual.
- JSON hacia Python.
- Socket TCP.
- AES-CBC/PKCS7/Base64.
- Manejo basico de errores.
- Bitacora local.

Criterios faltantes:

- `IdentificadorTelefono` y `IdentificadorDispositivo` usan el mismo valor en `AppConfig`.
- Algunos telefonos virtuales no coinciden con datos MySQL/SQL Server.
- Textos con codificacion rota en UI.
- No hay pruebas automatizadas.

Errores, riesgos o inconsistencias:

- Puede fallar si se seleccionan telefonos no existentes en BD.
- La bitacora local no sustituye la bitacora oficial del Identificador.

Que falta desarrollar:

- Separar identificador de telefono y dispositivo.
- Alinear todos los telefonos virtuales con seeds reales.
- Corregir textos con caracteres mal codificados.

Prioridad recomendada: media.

## 4. Matriz de cumplimiento

| Historia   | Estado | Cumple | Falta | Prioridad | Observaciones |
|---         |---|---|---|---|---|
| PROVEEDOR1 | Funciona parcialmente | Consulta servicio, saldo y tarifa | Texto plano, contrato robusto | Alta | Es la parte Java mas avanzada |
| PROVEEDOR2 | No implementado | Socket responde OK simulado | Registrar llamada, movimiento y rebajo | Alta | Bloquea cierre real de llamadas |
| PROVEEDOR3 | No implementado | Tabla existe | Bitacora archivo/JSON/cola/hilo | Alta | No hay servicio Java |
| IDENTIFICADOR1 | Funciona parcialmente | Valida MySQL, ubicacion, SIM, proveedor | No fallback plano, codigos finos | Alta | Flujo principal autoriza         |
| IDENTIFICADOR2 | Funciona parcialmente | Lista activa, hilo, BD parcial | Persistencia completa y limpieza BD | Media | Memoria sigue siendo fuente primaria  |
| IDENTIFICADOR3 | Funciona parcialmente | Envia finalizacion a proveedor | Depende de PROVEEDOR2 real | Alta | Actualmente cierre es simulado |
| IDENTIFICADOR4 | Funciona parcialmente | `#9090*`, saldo, socket, AES | Validar dispositivo, contrato Java | Alta | Demo funcional |
| IDENTIFICADOR5 | Funciona parcialmente | Archivo, JSON, cola, hilo | Campos anidados y tabla BD | Media | Cumple bastante, pero incompleto |
| SIM1 | Funciona parcialmente | UI, tramas, AES, sockets | Datos inconsistentes y pruebas | Media | Presentable como demo |

## 5. Riesgos tecnicos

- PROVEEDOR2 no registra ni rebaja saldo.
- PROVEEDOR3 no existe.
- Python -> Java usa JSON, no texto plano.
- AES tiene fallback inseguro a texto plano en Python.
- Seeds MySQL versionados no reconstruyen datos AES reales.
- Java parsea JSON manualmente con `indexOf`.
- Config Java contiene credenciales en `config.properties`.
- La tabla `bitacora_identificador` existe pero no se usa.
- Historial de llamadas del Identificador no se persiste al finalizar.
- Algunos datos C# no coinciden con la BD.

## 6. Riesgos para la entrega

- El profesor puede revisar SQL Server y ver que no hay movimientos.
- Puede pedir bitacora del proveedor y no existira.
- Puede exigir texto plano hacia proveedor y el proyecto usa JSON.
- Si se reconstruye la BD desde scripts, MySQL puede fallar por datos `ENC_*`.
- Si Tailscale o SQL Server no estan activos, PROVEEDOR1 falla.
- Si se valida seguridad, el fallback AES en Python es objetable.

## 7. Propuesta de desarrollo por fases

| Fase | Objetivo | Tareas | Archivos o modulos | Resultado | Prioridad |
|---|---|---|---|---|---|
| FASE 1 | Diagnostico y estabilizacion | Congelar puertos, revisar `.env`, validar seeds, compilar todo | `.env`, `docs/SETUP.md`, seeds | Ambiente repetible | Alta |
| FASE 2 | Comunicacion entre componentes | Definir si Python -> Java sera texto plano o JSON; ajustar ambos lados | `proveedor_cliente.py`, `ManejoCliente.java` | Contrato unico | Alta |
| FASE 3 | Historias criticas | Completar PROVEEDOR2 e IDENTIFICADOR3 | `RegistrarMovimiento.java`, DAO Java, `termina_llamada.py` | Finalizacion real | Alta |
| FASE 4 | Seguridad AES | Eliminar fallback plano, regenerar seeds AES | `crypto.py`, seeds MySQL, `AppConfig.cs` | Datos sensibles seguros | Alta |
| FASE 5 | Bitacoras e hilos | Implementar bitacora proveedor; mejorar bitacora identificador | Java service nuevo, `servidor.py` | Auditoria completa | Alta |
| FASE 6 | Pruebas integradas | Probar saldo, insuficiente, finalizacion, inactivo, SIM invalida | `docs/testing/casos_prueba.md` | Demo defendible | Alta |
| FASE 7 | Documentacion final | Actualizar informe, setup, contratos y pendientes | `docs`, README | Entrega clara | Media |

## 8. Recomendaciones finales

Primero completar PROVEEDOR2. Es el hueco mas importante: sin registro de movimientos no hay cierre real del ciclo telefonico.

Segundo definir oficialmente el contrato Python -> Java. Si el PDF exige texto plano, hay que cambiarlo ya o documentar la desviacion.

Tercero corregir los seeds MySQL con AES real y quitar los valores `ENC_*`.

Cuarto implementar PROVEEDOR3 antes de la presentacion. Es una historia completa y actualmente esta vacia.

Quinto endurecer AES en Python: si no puede descifrar, debe rechazar la trama, no continuar en plano.

## 9. Proximos pasos sugeridos

1. Implementar `RegistrarMovimiento.java` y DAO para insertar en `llamadas_proveedor`, `movimientos_saldo` y actualizar `saldos`.
2. Crear bitacora Java con archivo `.txt`, JSON por linea, cola e hilo independiente.
3. Normalizar contrato Python -> Java segun PDF: texto plano si se mantiene el requisito oficial.
4. Regenerar `database/mysql_identificador/seed/001_seed_identificador.sql` con AES real.
5. Ejecutar pruebas integradas y marcar resultados reales en `docs/testing/casos_prueba.md`.
