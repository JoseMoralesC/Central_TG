Componente	Tecnología
Identificador	Python + MySQL
Proveedor Telefónico	Java + SQL Server
Simulador de Llamadas	C#
Comunicación	Sockets TCP
Integración	Los 3 componentes funcionando juntos

Compañero	Historias	Lenguaje	Branch
Carlos Alberto Mata Fallas	1, 2 y 3	Java	feature/java
Gabriel Navarro Aguirre	4, 5 y 6	Python	feature/python
José Rodolfo Morales Calderón	7, 8 y 9	C#	feature/csharp

GIT: comandos básicos a utilizar
Comandos	Descripción
git status	Muestra el estado y la rama actual
git add . 	Sube al stage los cambios
git commit –m “”	Le da un mensaje descriptivo al commit
git push origin feature/xxxx	Aplica los cambios al repo
git pull	Actualiza el proyecto con los cambios recientes
git checkout feature/xxxx	Nos posiciona en la rama de desarrollo
git checkout main	Nos posiciona en el main

Ciclo de actualización de datos con Git:
Comandos	Descripcion
git checkout main	Nos posiciona en el main
git pull origin main	Todo se descarga y queda actualizado.
git checkout feature/xxxx	Nos posiciona en la branch
git pull origin feature/xxxx	Trae desde GitHub los cambios que existan
git merge main	Quiero incorporar todo lo que existe en main dentro de feature/xxxx
git push origin feature/xxxx	Envía a GitHub la versión actual de tu rama

Repo de GitHub:
https://github.com/JoseMoralesC/Central_TG.git





Bases de datos con usuario remoto:
DB	Usuario	Password	Hostname	Port

Mysql	jose	jose1234	
100.114.84.5
	
3306
	gabriel	gabriel1234		
	charlie	charlie1234		

SSMS	jose_dev	jose1234	
100.88.25.17,49172	
N/A
	charlie_dev	charlie1234		
	gabriel_dev	gabriel1234		

Conexión servidor remoto con app:

https://tailscale.com/download/windows



Propuesta de distribución:
Gabriel Aguirre Navarro (Python)
Identificador1
Autorizar llamada
Identificador2
Iniciar llamada
Identificador3
Finalizar llamada

Estas tres historias forman un flujo completo:
Solicitud
     ↓
Autorizar
     ↓
Iniciar llamada
     ↓
Finalizar llamada
Todo ocurre dentro del Identificador.
Este compañero desarrolla:
Socket Python
MySQL
AES
Validaciones
Gestión de llamadas activas

Carlos Alberto Mata Fallas (Java)
Proveedor1
Verificar saldo
Proveedor2
Registrar movimientos
Proveedor3
Bitácora del proveedor
Las tres historias pertenecen al mismo sistema:
Proveedor Telefónico XYZ
Este compañero desarrolla:
Socket Java
SQL Server
Tarifas
Saldos
Cobros
Bitácoras
Todo queda encapsulado.

José Rodolfo Morales Calderón (C#)
Identificador4
Consulta de saldo
Identificador5
Bitácora del identificador
SIM1
Simulador de llamadas

Aquí hay una pequeña excepción.
SIM1 obligatoriamente debe hacerse en C#.
Y las historias restantes son:
Identificador4
Identificador5
que son independientes del flujo principal de llamadas.

Ventajas de esta distribución
Gabriel Aguirre Navarro Python
Trabaja únicamente en:
Python
MySQL
Socket Identificador

Carlos Alberto Mata Fallas Java
Trabaja únicamente en:
Java
SQL Server
Socket Proveedor

José Rodolfo Morales Calderón C#
Trabaja únicamente en:
C#
Interfaces
Simulador
Consulta de saldo
Bitácoras

Resumen:
Gabriel Navarro Aguirre Python
Identificador1
Identificador2
Identificador3

Carlos Alberto Mata Fallas Java
Proveedor1
Proveedor2
Proveedor3
.

José Rodolfo Morales Calderón C#
Identificador4
Identificador5
SIM1












Yo considero que antes de escribir una sola línea de código realicemos un tipo RoadMap para ordenarnos y tener un norte claro ejemplo:
Semana 1
Todos juntos:
Modelo MySQL
Modelo SQL Server
Diagramas
Definir puertos
Definir formato JSON
Por ejemplo:
{
  "telefono": "25743715",
  "tipo": "solicitud",
  "destino": "88889999"
}
y dejarlo congelado.
Definir la estructura de carpetas del proyecto y las reglas que vamos a manejar para commits y mas.
Reglas:
Commits generales:
Avisar cuando hay commit 
Siempre avisar los commit y cambios 
Mensajes de commit bien definidos
Las pruebas siempre en main.
Regla como equipo
Antes de empezar a programar cada día:
git checkout main
git pull origin main

git checkout mi-rama
git pull origin mi-rama
git merge main

Así todos trabajamos siempre con la versión más reciente del proyecto y evitan muchísimos conflictos cuando llegue el momento de fusionar a main.

Semana 2
Cada uno desarrolla sus 3 historias.
Ya teniendo lo anterior definido, es más fácil desarrollar sin tener que ver para atrás.

Semana 3
Integración:
SIMULADOR (C#)
      ↓
IDENTIFICADOR (Python)
      ↓
PROVEEDOR (Java)

Como ya tenemos echo el entorno de usuario remoto, será súper fácil realizar pruebas de entorno desde la comodidad de nuestras casas sin necesidad de estarnos enviando códigos.

Esta es mi propuesta, desde luego está abierta a todo tipo de cambios.

Abajo mi recomendación de RoadMap para un desarrollo ordenado.
Podemos incluso crear un Excel tipo check-list para ver los avances grupales e indivduales.






Fase 0 — Organización inicial
Objetivo
Dejar el grupo ordenado antes de programar.
Actividades
Crear repositorio en GitHub.
Crear ramas por integrante.
Definir puertos de sockets.
Definir formato estándar de tramas JSON
Definir estructura de carpetas.
Definir scripts iniciales de BD.
Definir responsables por historia.
Resultado esperado
Proyecto organizado, tecnologías claras y responsabilidades asignadas.

Fase 1 — Diseño de arquitectura y bases de datos
Compañero Python — Identificador
Debe diseñar la base de datos MySQL para:
Teléfonos.
Proveedores.
Tarjetas telefónicas.
Dispositivos.
Llamadas activas.
Registro de llamadas del identificador.
Compañero Java — Proveedor
Debe diseñar la base de datos SQL Server para:
Clientes.
Servicios prepago/postpago.
Saldos.
Tarifas.
Movimientos.
Llamadas registradas.
Compañero C# — Simulador
Debe definir:
Pantallas necesarias.
Datos que enviará al identificador.
Formato de entrada/salida.
Flujo visual del simulador.





Fase 2 — Contratos de comunicación
Objetivo
Definir cómo se van a hablar los tres sistemas.
C# Simulador
    ↓ JSON/XML
Python Identificador
    ↓ texto plano
Java Proveedor
Se deben definir estas tramas
Simulador → Identificador
Solicitud de llamada.
Inicio de llamada.
Finalización de llamada.
Consulta de saldo.
Identificador → Proveedor
Verificación de saldo.
Consulta de saldo.
Registro de movimiento.
Proveedor → Identificador
OK.
INSUF.
ERROR.
Monto de saldo.
Tiempo máximo autorizado.






Fase 3 — Desarrollo individual por componente
Compañero 1 — Python + MySQL
Historia Identificador1
Autorizar llamada telefónica
Debe desarrollar:
Socket síncrono en Python.
Recepción de JSON/XML.
Validación de teléfono.
Validación de tarjeta.
Validación de ubicación nacional.
Validación de teléfono destino.
Cifrado AES para datos sensibles.
Comunicación con proveedor Java.
Respuesta OK, error o motivo de rechazo.

Historia Identificador2
Iniciar llamada
Debe desarrollar:
Recepción de trama de inicio.
Registro de llamada activa.
Control de llamadas prepago.
Cálculo de hora máxima de finalización.
Lista ordenada de llamadas activas.
Manejo seguro de hilos.

Historia Identificador3
Terminación de llamada
Debe desarrollar:
Proceso que revise llamadas activas vencidas.
Finalización automática por saldo agotado.
Finalización manual por solicitud del cliente.
Eliminación de llamada activa.
Cálculo de duración real.
Envío de datos al proveedor para rebajo/cobro.

Compañero 2 — Java + SQL Server
Historia Proveedor1
Verificar saldo telefónico
Debe desarrollar:
Socket del proveedor en Java.
Recepción de trama en texto plano.
Validación de tipo de servicio.
Si es postpago, autorizar.
Si es prepago, validar saldo.
Cálculo de tarifa por tipo de llamada.
Respuesta OK, INSUF o ERROR.

Historia Proveedor2
Registrar movimientos
Debe desarrollar:
Recepción de trama de cobro o rebajo.
Registro de llamada.
Registro de movimiento.
Rebajo de saldo si es prepago.
Registro de cobro si es postpago.
Respuesta OK o ERROR.

Historia Proveedor3
Bitácora del proveedor
Debe desarrollar:
Archivo de bitácora en texto.
Formato JSON.
Registro de tramas de entrada.
Registro de tramas de salida.
Cola de escritura.
Hilo independiente para no bloquear operaciones.

Compañero 3 — C# + Simulador
Historia SIM1
Simulador de llamadas
Debe desarrollar:
Interfaz en C#.
Pantalla para marcar número.
Pantalla para realizar llamada.
Pantalla para finalizar llamada.
Pantalla para consultar saldo con #9090*.
Armado de tramas JSON/XML.
Envío de datos al identificador Python.
Mostrar respuestas en pantalla.

Historia Identificador4
Consulta de saldo
Puede apoyar desarrollando la lógica funcional o documentada de:
Trama de consulta de saldo.
Validaciones requeridas.
Comunicación con proveedor.
Respuesta con saldo disponible.
Esta historia toca el Identificador, así que debe coordinarse muy bien con el compañero de Python.

Historia Identificador5
Bitácora del identificador
Puede encargarse de:
Definir formato JSON de bitácora.
Registrar entrada y salida de tramas.
Crear cola de bitácora.
Manejo en hilo independiente.
Integrarlo con el identificador Python.
Esta también requiere coordinación con Python.














Fase 4 — Integración parcial
Objetivo
Probar conexión entre componentes.

Pruebas mínimas
Prueba	Flujo
Consulta de saldo	C# → Python → Java → Python → C#
Autorizar llamada	C# → Python → Java → Python → C#
Iniciar llamada	C# → Python
Finalizar llamada	C# → Python → Java
Saldo insuficiente	C# → Python → Java → Python → C#




















Fase 5 — Integración completa
Objetivo
Que todo funcione como un solo sistema.
1. Usuario marca desde C#
2. C# envía solicitud al Identificador
3. Python valida datos
4. Python consulta saldo en Java
5. Java responde autorización
6. Python autoriza llamada
7. C# muestra llamada iniciada
8. Python controla llamada activa
9. Al finalizar, Python avisa a Java
10. Java registra movimiento y rebaja saldo


















Fase 6 — Documentación
Documentos obligatorios
Según el PDF, deben entregar:
Portada.
Introducción.
Diagrama de base de datos.
Diagramas de casos de uso.
Diagramas de clases.
Conclusiones.
Recomendaciones.
Bibliografía.
Código fuente.
Scripts de bases de datos.















Fase 7 — Pruebas finales
Casos de prueba recomendados
Caso	Resultado esperado
Teléfono prepago con saldo	Llamada autorizada
Teléfono prepago sin saldo	Respuesta INSUF
Teléfono postpago	Llamada autorizada
Consulta de saldo prepago	Retorna saldo
Consulta de saldo postpago	Retorna -1
Teléfono inactivo	Error
Tarjeta incorrecta	Error código 2
Ubicación fuera del país	Error código 3
Finalización normal	Movimiento registrado
Finalización por saldo agotado	Movimiento registrado y saldo rebajado


