Informe de análisis técnico del proyecto
Informe de análisis técnico del proyecto1. Resumen ejecutivoEl proyecto está en un estado parcialmente implementado, pero todavía no estable como sistema integrado.
Hay avances importantes respecto a la revisión anterior: el simulador C# compila, cifra datos sensibles con AES y arma tramas más completas; el Identificador Python ya tiene router, servicios separados, cola de bitácora e hilo de monitoreo; el Proveedor Java tiene socket, servicios de saldo y DAO hacia SQL Server.
Pero el sistema completo aún no puede considerarse funcional extremo a extremo porque hay incompatibilidades críticas:
Python envía JSON al proveedor, pero el PDF exige texto plano para el Proveedor.
Python envía acciones como VERIFICAR_SALDO y REGISTRO_MOVIMIENTO, pero Java espera otras acciones como INICIAR_LLAMADA, FINALIZAR_LLAMADA, CONSULTAR_SALDO.
La base MySQL seed usa valores ENC_..., no valores AES reales compatibles con C#/Python.
python_identificador/main.py está vacío, por lo que el punto de arranque principal no levanta el servidor.
PROVEEDOR2 y PROVEEDOR3 siguen sin implementación funcional real.
La respuesta de saldo Python no coincide completamente con el modelo C#.
Verificaciones ejecutadas:
dotnet build csharp_simulador\SimuladorTelefonico.slnx: correcto.
python -m py_compile ...: correcto.
javac ...: correcto.
PDF en docs/Proyecto/Proyecto - Alcance 1.pdf: leído.
2. Estado global del sistemaEstado global: funciona parcialmente por componentes, no como integración completa.
El sistema actualmente no está listo para funcionar como proyecto integrado sin ajustes. C# puede enviar tramas a Python; Python puede recibir, enrutar y responder; Java puede levantar un socket y consultar SQL Server en algunos servicios. Pero la comunicación Python -> Java no está alineada ni con el PDF ni con el handler Java.
Componentes que ya se comunican o están preparados:
C# -> Python por socket JSON en puerto 5000.
Python tiene cliente socket hacia Java en proveedor_cliente.py.
Java tiene socket en SocketTCP.java.
Componentes aislados o incompletos:
Java no procesa correctamente las acciones enviadas por Python.
Java no registra movimientos.
Java no tiene bitácora oficial en archivo con cola/hilo.
Python no tiene arranque principal en main.py.
MySQL seed no coincide con cifrado AES real.
Incumplimientos tecnológicos:
Python -> Java usa JSON; el PDF exige texto plano para comunicación con Proveedor.
Bitácoras del Proveedor no cumplen archivo JSON + cola + hilo.
Datos MySQL seed no están cifrados con AES real.
3. Análisis por historias de usuarioPROVEEDOR1Estado actual: Funciona parcialmente.
Evidencia:
Socket Java: SocketTCP.java.
Handler: ManejoCliente.java.
Servicio saldo llamada: VerificarSaldo.java.
Consulta saldo: ConsultaSaldo.java.
DAO SQL Server: ServicioDAO.java.
Tablas: servicios, saldos, tarifas en 001_create_database_proveedor.sql.
Criterios cumplidos:
Existe socket Java.
Existe lógica para consultar servicio, saldo y tarifa.
Postpago devuelve autorización sin saldo.
Prepago valida saldo mínimo.
SQL Server tiene tablas necesarias.
Criterios faltantes:
El PDF exige trama de texto plano; Java está leyendo JSON manualmente.
Python envía accion: VERIFICAR_SALDO; Java no maneja esa acción.
Formato de respuesta no cumple los campos exactos del PDF: tarifa en 10 espacios y tiempo en 6 dígitos.
Tipos de llamada PDF: mismo proveedor, otro proveedor, fuera del país; Java usa LOCAL/INTERNACIONAL.
Errores/riesgos:
Integrado con Python, PROVEEDOR1 no funciona para autorización porque las acciones no coinciden.
ServicioDAO busca tarifa por tipoDestino, pero el seed tiene NACIONAL/INTERNACIONAL, no LOCAL.
Config Java apunta a instancia local BIBILOTECABD, no al servidor remoto documentado.
Falta desarrollar:
Parser de trama plana.
Alinear acciones con Python.
Formato exacto de respuesta del PDF.
Homologar tipos de llamada.
Prioridad: Alta.
PROVEEDOR2Estado actual: No implementado.
Evidencia:
RegistrarMovimiento.java está vacío.
ManejoCliente.java para FINALIZAR_LLAMADA solo responde un JSON fijo.
Tablas existen: llamadas_proveedor, movimientos_saldo.
Criterios cumplidos:
Base de datos tiene estructura para llamadas y movimientos.
Criterios faltantes:
Recibir trama de rebajo/cobro.
Insertar llamada.
Insertar movimiento.
Rebajar saldo prepago.
Registrar cobro postpago.
Responder OK/ERROR real.
Errores/riesgos:
IDENTIFICADOR3 no puede completarse porque el proveedor no registra movimientos.
Python envía REGISTRO_MOVIMIENTO, pero Java no maneja esa acción.
Falta desarrollar:
Implementar RegistrarMovimiento.
Agregar DAO para llamada/movimiento/saldo.
Alinear contrato Python -> Java.
Prioridad: Alta.
PROVEEDOR3Estado actual: No implementado.
Evidencia:
No se encontró servicio de bitácora del proveedor.
No hay cola ni hilo de escritura.
Tabla bitacora_proveedor existe, pero el PDF exige archivo de texto.
Criterios cumplidos:
Ninguno funcional contra el PDF.
Criterios faltantes:
Archivo de texto en servidor.
Registro JSON por entrada/salida.
Cola.
Hilo independiente.
Bitácora para toda transacción.
Errores/riesgos:
La tabla SQL no sustituye la bitácora requerida.
Sin cola/hilo, no se cumple el criterio explícito.
Falta desarrollar:
BitacoraProveedorService.
BlockingQueue.
Worker thread.
Registro de tramas entrada/salida desde ManejoCliente.
Prioridad: Media.
IDENTIFICADOR1Estado actual: Funciona parcialmente.
Evidencia:
Router Python: handler.py.
Servicio autorización: autorizacion_llamada.py.
Repositorio MySQL: repositorio.py.
AES: crypto.py.
Cliente proveedor: proveedor_cliente.py.
Criterios cumplidos:
Recibe JSON.
Valida campos obligatorios.
Valida ubicación nacional.
Intenta desencriptar AES.
Valida teléfono, SIM y dispositivo contra MySQL.
Consulta proveedor.
Responde JSON.
Criterios faltantes:
No se valida teléfono destino nacional contra MySQL, solo formato de 8 dígitos.
Comunicación con proveedor incumple texto plano.
Acciones enviadas a Java no coinciden con Java.
Seed MySQL no coincide con valores AES reales.
main.py está vacío.
Errores/riesgos:
C# cifra con AES real; MySQL seed contiene ENC_88889999, por lo que las búsquedas fallarán salvo que la BD real tenga valores actualizados.
Si se ejecuta python main.py, no levanta nada.
La autorización depende de proveedor, pero el proveedor no procesa VERIFICAR_SALDO.
Falta desarrollar:
Actualizar seeds con AES real.
Corregir arranque principal.
Alinear contrato Python -> Java.
Validar destino nacional contra base de datos.
Prioridad: Alta.
IDENTIFICADOR2Estado actual: Funciona parcialmente.
Evidencia:
Servicio: iniciar_llamada.py.
Router acepta INICIO_LLAMADA.
Lista global llamadas_activas con Lock.
Tabla MySQL llamadas_activas.
Criterios cumplidos:
Recibe trama de inicio.
Registra llamada en lista en memoria.
Calcula hora máxima.
Ordena por hora máxima.
Usa bloqueo para concurrencia.
Criterios faltantes:
Persistencia en MySQL no es correcta: inserta telefono_id=0, lo cual puede fallar por FK.
No resuelve teléfono real asociado.
No valida ni desencripta datos de inicio.
No garantiza recuperación si el proceso se reinicia.
Errores/riesgos:
La lista en memoria funciona solo mientras Python esté vivo.
El insert a BD es “best effort” y puede fallar silenciosamente.
No se registra historial.
Falta desarrollar:
Resolver telefono_id.
Persistir correctamente.
Controlar errores de BD.
Decidir fuente oficial: memoria, BD o ambas.
Prioridad: Alta.
IDENTIFICADOR3Estado actual: Funciona parcialmente.
Evidencia:
Servicio: termina_llamada.py.
Hilo monitor: iniciar_verificador_llamadas.
Finalización manual: procesar_finalizacion_llamada.
Envía movimiento al proveedor con enviar_registro_movimiento.
Criterios cumplidos:
Hay proceso que revisa llamadas vencidas.
Elimina llamadas activas.
Recibe finalización desde C#.
Intenta enviar movimiento al proveedor.
Criterios faltantes:
No informa al cliente cuando termina por saldo agotado.
No registra historial real en MySQL.
Proveedor no implementa REGISTRO_MOVIMIENTO.
Formato enviado a proveedor no cumple texto plano del PDF.
Duración para proveedor usa HH:MM:SS, PDF pide 6 dígitos sin dos puntos.
Errores/riesgos:
Para llamadas vencidas envía costo 0.0.
Finalización depende de PROVEEDOR2, que no existe.
Si los datos llegan cifrados desde C#, se envían cifrados al proveedor.
Falta desarrollar:
Formato proveedor correcto.
Registro historial.
Notificación al simulador para saldo agotado.
Completar integración con PROVEEDOR2.
Prioridad: Alta.
IDENTIFICADOR4Estado actual: Funciona parcialmente.
Evidencia:
C# consulta: ConsultaSaldoForm.cs.
Modelo C#: ConsultaSaldo.cs.
Python: consulta.py.
Java: ConsultaSaldo.java.
Criterios cumplidos:
C# detecta #9090*.
C# envía teléfono, identificadores y ubicación.
C# cifra datos sensibles.
Python recibe CONSULTA_SALDO.
Python valida teléfono y SIM contra MySQL.
Python consulta proveedor.
Java tiene lógica de consulta de saldo.
Criterios faltantes:
Python no valida ubicación en consulta de saldo, aunque el PDF lo exige.
Python no valida dispositivo en consulta de saldo.
Python responde datos_saldo, pero C# RespuestaSaldo espera propiedad saldo; puede mostrar saldo 0 aunque haya saldo real.
Java espera campo numero; Python envía telefono_origen.
Java maneja CONSULTAR_SALDO, esta parte coincide en acción, pero el campo de teléfono no coincide.
Errores/riesgos:
Consulta #9090* no está garantizada extremo a extremo por incompatibilidad campo telefono_origen vs numero.
Seed MySQL no coincide con AES.
La respuesta puede mostrarse incorrectamente en C#.
Falta desarrollar:
Alinear respuesta RESPUESTA_SALDO.
Python debe validar ubicación y dispositivo.
Java debe leer telefono_origen o Python enviar numero.
Actualizar C# para leer datos_saldo.
Prioridad: Alta.
IDENTIFICADOR5Estado actual: Funciona parcialmente.
Evidencia:
Cola en servidor.py.
Worker trabajador_bitacora.
Handler registra entrada/salida en handler.py.
Archivo: bitacora_identificador.txt.
Criterios cumplidos:
Usa archivo de texto.
Escribe JSON por línea.
Usa cola queue.Queue.
Usa hilo independiente.
Registra entradas y salidas.
Criterios faltantes:
Para tramas anidadas como datos_llamada, no extrae bien telefono_origen, destino o tiempo.
No desencripta ni normaliza datos para auditoría.
No garantiza flush/cierre ordenado.
La bitácora solo inicia si se usa servidor.py; main.py vacío no la inicia.
Errores/riesgos:
Bitácora puede quedar incompleta para inicio/finalización.
Si se ejecuta el entry point incorrecto, no hay bitácora.
Falta desarrollar:
Extraer datos también desde datos_llamada.
Definir arranque único.
Mejorar formato para cumplir todos los campos del PDF.
Prioridad: Media.
SIM1Estado actual: Funciona parcialmente.
Evidencia:
Proyecto C# compila.
UI principal: Form1.cs.
Marcar: MarcarNumeroForm.cs.
Llamada activa: LlamadaActivaForm.cs.
Consulta saldo: ConsultaSaldoForm.cs.
Socket: TcpSocketClient.cs.
AES: CryptoService.cs.
Bitácora local: BitacoraService.cs.
Criterios cumplidos:
Simulador en C#.
UI con varios teléfonos.
Pantallas separadas.
Marcar número.
Iniciar y finalizar llamada.
Consulta #9090*.
Arma JSON.
Envía por socket TCP.
Muestra respuestas.
Maneja errores de conexión.
Cifra datos sensibles con AES.
Tiene bitácora local.
Criterios faltantes:
No probado integrado con Python/Java real.
Teléfonos configurados no coinciden claramente con MySQL AES.
Respuesta de saldo no coincide con el modelo C#.
No hay automatización de pruebas.
Errores/riesgos:
Puede verse funcional en UI, pero fallar por incompatibilidad con MySQL/Java.
Si Python usa otra llave/IV, no podrá descifrar.
En main, el C# está implementado, pero el proyecto global no está integrado.
Falta desarrollar:
Ajustar modelo C# a datos_saldo.
Coordinar AES y seeds.
Pruebas integradas.
Prioridad: Alta.
4. Matriz de cumplimientoHistoria	Estado	Cumple	Falta	Prioridad	Observaciones
PROVEEDOR1	Funciona parcialmente	Socket, DAO, saldo/tarifa parcial	Texto plano, acciones, formato PDF	Alta	No funciona integrado con Python
PROVEEDOR2	No implementado	Tablas SQL	Registro llamada/movimiento/rebajo	Alta	RegistrarMovimiento vacío
PROVEEDOR3	No implementado	Nada funcional	Archivo, JSON, cola, hilo	Media	Tabla SQL no basta
IDENTIFICADOR1	Funciona parcialmente	JSON, AES, MySQL, validaciones	Proveedor texto plano, destino BD, seeds AES	Alta	Depende de Java incompatible
IDENTIFICADOR2	Funciona parcialmente	Lista activa, lock, ordenamiento	Persistencia correcta, telefono_id real	Alta	Insert BD puede fallar
IDENTIFICADOR3	Funciona parcialmente	Monitor, finalización, envío proveedor	Movimiento real, historial, notificación saldo agotado	Alta	PROVEEDOR2 bloquea
IDENTIFICADOR4	Funciona parcialmente	C# + Python + Java parcial	Campos/respuesta/validaciones	Alta	datos_saldo vs saldo
IDENTIFICADOR5	Funciona parcialmente	Archivo, cola, hilo, entrada/salida	Extracción completa de tramas anidadas	Media	Entry point vacío
SIM1	Funciona parcialmente	UI, socket, AES, saldo, llamada	Prueba integrada y respuesta saldo	Alta	Es el componente más presentable
5. Riesgos técnicosContrato Python -> Java incompatible.
Incumplimiento del PDF: Proveedor debe recibir texto plano, no JSON.
main.py vacío impide arranque estándar.
Seeds MySQL no cifrados con AES real.
C# cifra correctamente, pero Python/BD deben usar misma llave/IV y valores.
PROVEEDOR2 no existe funcionalmente.
PROVEEDOR3 no existe.
Consulta saldo puede mostrar saldo incorrecto por mismatch datos_saldo.
Java parsea JSON manualmente, frágil ante espacios, números o estructuras anidadas.
No hay prueba integrada demostrada.
6. Riesgos para la entregaEl mayor riesgo es la presentación integrada. El PDF indica que los componentes deben funcionar como conjunto. Actualmente se puede demostrar C# y partes de Python, pero el flujo C# -> Python -> Java -> SQL Server no está estabilizado.
Riesgos concretos:
Autorización falla al consultar Java.
Consulta saldo falla o muestra saldo incorrecto.
Finalización no registra movimiento.
Bitácora del proveedor ausente.
Si se ejecuta python main.py, no inicia el Identificador.
La evaluación puede penalizar por incumplir tramas de texto plano al Proveedor.
7. Propuesta de desarrollo por fasesFASE 1: Diagnóstico y estabilizaciónObjetivo: lograr que cada componente arranque correctamente.
Tareas:
Definir entry point Python.
Confirmar rama y estado final del repo.
Verificar .env.
Confirmar puertos 5000 y 6000.
Crear prueba mínima C# -> Python.
Archivos:
python_identificador/main.py
python_identificador/app/sockets/servidor.py
docs/SETUP.md
Resultado esperado: C# conecta con Python de forma reproducible.
Prioridad: Alta.
FASE 2: Corrección de comunicación entre componentesObjetivo: alinear Python -> Java.
Tareas:
Cambiar Python -> Java a texto plano, o documentar excepción si el profesor la permite.
Alinear acciones: VERIFICAR_SALDO, CONSULTAR_SALDO, REGISTRO_MOVIMIENTO.
Alinear campos: telefono_origen vs numero.
Responder con formato esperado por Python.
Archivos:
proveedor_cliente.py
ManejoCliente.java
VerificarSaldo.java
ConsultaSaldo.java
Resultado esperado: Python obtiene respuesta real del proveedor.
Prioridad: Alta.
FASE 3: Cumplimiento de historias críticasObjetivo: cerrar flujo llamada/saldo/finalización.
Tareas:
Completar PROVEEDOR1.
Implementar PROVEEDOR2.
Ajustar IDENTIFICADOR1/3/4.
Corregir respuesta de saldo C#.
Archivos:
RegistrarMovimiento.java
DAOs Java
consulta.py
termina_llamada.py
RespuestaService.cs / RespuestaSaldo.cs
Resultado esperado: llamada autorizada, iniciada, finalizada y cobrada.
Prioridad: Alta.
FASE 4: Seguridad, cifrado AES y validacionesObjetivo: hacer consistente el cifrado.
Tareas:
Unificar AES_KEY y AES_IV.
Regenerar seeds MySQL con AES real.
Validar ubicación en consulta saldo.
Validar dispositivo en consulta saldo.
Archivos:
.env
001_seed_identificador.sql
consulta.py
crypto.py
CryptoService.cs
Resultado esperado: C# y Python cifran/descifran igual; MySQL coincide.
Prioridad: Alta.
FASE 5: Bitácoras e hilosObjetivo: cumplir bitácoras oficiales.
Tareas:
Mejorar bitácora Identificador para tramas anidadas.
Implementar bitácora Proveedor.
Cola e hilo en Java.
Registrar entrada/salida.
Archivos:
servidor.py
handler.py
nuevo servicio Java de bitácora
ManejoCliente.java
Resultado esperado: bitácoras completas y no bloqueantes.
Prioridad: Media.
FASE 6: Pruebas integradas y preparación para presentaciónObjetivo: tener demo confiable.
Tareas:
Probar llamada OK.
Probar saldo insuficiente.
Probar consulta #9090*.
Probar finalización.
Probar errores controlados.
Documentar resultados.
Archivos:
docs/testing/casos_prueba.md
README de cada componente
Resultado esperado: guion de demo con evidencia.
Prioridad: Alta.
FASE 7: Documentación finalObjetivo: alinear documentación con código real.
Tareas:
Corregir protocolo: JSON solo C# -> Python; texto plano Python -> Java.
Documentar contratos definitivos.
Documentar pendientes y limitaciones.
Agregar pasos exactos de ejecución.
Archivos:
docs/arquitectura/protocolo_comunicacion.md
docs/SETUP.md
README.md
Resultado esperado: documentación defendible.
Prioridad: Media.
8. Recomendaciones finalesLa prioridad no debe ser crear más pantallas ni más clases, sino alinear contratos e integración.
Orden recomendado:
Arreglar main.py para levantar el servidor Python.
Alinear Python -> Java.
Implementar PROVEEDOR2.
Corregir AES/seeds MySQL.
Ajustar respuesta de saldo C#.
Implementar bitácora Java.
Ejecutar pruebas integradas.
9. Próximos pasos sugeridosPrimero: definir contrato Python -> Java en texto plano y actualizar ambos lados.
Segundo: completar RegistrarMovimiento.java, porque sin PROVEEDOR2 no se puede cerrar IDENTIFICADOR3.
Tercero: regenerar datos MySQL cifrados con AES real usando la misma llave/IV que C# y Python.
