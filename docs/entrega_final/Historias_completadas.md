# Historias completadas

## PROVEEDOR1 - Verificar saldo

Estado: Cumple funcionalmente.

Evidencia:

- `java_proveedor/src/services/VerificarSaldo.java`
- `java_proveedor/src/services/ConsultaSaldo.java`
- `java_proveedor/src/database/ServicioDAO.java`

## PROVEEDOR2 - Registrar movimientos

Estado: Cumple funcionalmente.

Evidencia:

- `java_proveedor/src/services/RegistrarMovimiento.java`
- `java_proveedor/src/database/LlamadaProveedorDAO.java`
- `java_proveedor/src/database/TarifaDAO.java`
- `java_proveedor/src/sockets/ManejoCliente.java`

## PROVEEDOR3 - Bitacora del proveedor

Estado: Cumple funcionalmente.

Evidencia:

- `java_proveedor/src/services/BitacoraService.java`
- `logs/proveedor_bitacora.txt`

## IDENTIFICADOR1 - Autorizar llamada

Estado: Cumple funcionalmente.

Evidencia:

- `python_identificador/app/services/autorizacion_llamada.py`
- `python_identificador/app/database/repositorio.py`

## IDENTIFICADOR2 - Iniciar llamada

Estado: Cumple funcionalmente.

Evidencia:

- `python_identificador/app/services/iniciar_llamada.py`

## IDENTIFICADOR3 - Terminacion de llamada

Estado: Cumple funcionalmente.

Evidencia:

- `python_identificador/app/services/termina_llamada.py`
- `java_proveedor/src/services/RegistrarMovimiento.java`

## IDENTIFICADOR4 - Consulta de saldo

Estado: Cumple funcionalmente.

Evidencia:

- `python_identificador/app/services/consulta.py`
- `csharp_simulador/SimuladorTelefonico/UI/ConsultaSaldoForm.cs`

## IDENTIFICADOR5 - Bitacora del identificador

Estado: Cumple funcionalmente.

Evidencia:

- `python_identificador/app/sockets/servidor.py`
- `python_identificador/app/sockets/handler.py`

## SIM1 - Simulador de llamadas

Estado: Cumple funcionalmente.

Evidencia:

- `csharp_simulador/SimuladorTelefonico/Form1.cs`
- `csharp_simulador/SimuladorTelefonico/UI/MarcarNumeroForm.cs`
- `csharp_simulador/SimuladorTelefonico/UI/LlamadaActivaForm.cs`
- `csharp_simulador/SimuladorTelefonico/UI/ConsultaSaldoForm.cs`
- `csharp_simulador/SimuladorTelefonico/Services/CryptoService.cs`

## Nota de validacion

El estado funcional queda sujeto a ejecutar la matriz de pruebas integradas con MySQL y SQL Server disponibles.
