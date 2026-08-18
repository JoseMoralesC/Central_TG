# Mapeo de herramientas y confirmacion Fase 0 - Persona 2

## Objetivo

Este documento complementa `docs/roadmaps/roadmap_persona_2_alcance_final.md` con dos decisiones practicas:

- Que partes del proyecto conviene programar en VS Code.
- Que partes conviene programar en Visual Studio.

Tambien registra el escaneo general realizado para confirmar la Fase 0 del roadmap de persona 2.

## Resumen ejecutivo

El repositorio ya contiene los componentes base del proyecto anterior:

- Simulador C# WinForms.
- Identificador Python.
- Proveedor Java.
- Servicios Web WCF/.NET Framework.
- Un host .NET 8 minimo de prueba.
- Scripts SQL para MySQL, SQL Server y referencias a MongoDB.
- Contratos JSON compartidos.

No se encontro un proyecto Web final ya creado para el alcance 3. El equipo ya confirmo que la Web Cliente tambien se trabajara en C#, por lo que antes de programar pantallas `ADM6`, `ADM7`, `CLIENTE1`, `CLIENTE2` y `CLIENTE3`, se debe decidir si se va a crear:

1. Dos proyectos Web C# separados: `WebAdmin` y `WebCliente`.
2. Un solo proyecto Web C# con areas/rutas separadas para administracion y cliente.

Decision tomada: usar C# para ambas Web y crearlas/trabajarlas en Visual Studio, porque el alcance exige Web Administrativa en C# y los servicios actuales son WCF clasicos que se consumen mas naturalmente desde Visual Studio.

## Mapeo VS Code vs Visual Studio

| Area | Carpeta/archivo | Herramienta recomendada | Motivo |
| --- | --- | --- | --- |
| Documentacion y roadmaps | `docs/` | VS Code | Edicion Markdown rapida, busqueda global y control de cambios simple. |
| Contratos JSON | `shared/contracts/`, `shared/config/` | VS Code | Son archivos planos JSON; conviene revisarlos junto a Python/Java. |
| Identificador | `python_identificador/` | VS Code | Proyecto Python, ejecucion por terminal, lectura rapida de servicios y sockets. |
| Proveedor Java | `java_proveedor/` | VS Code | Java sin proyecto Maven/Gradle detectado; se compila por `javac` desde terminal. |
| Scripts SQL | `database/mysql_identificador/`, `database/sqlserver_proveedor/` | VS Code para editar; SSMS/MySQL Workbench para ejecutar | VS Code sirve para versionar scripts; la ejecucion real debe ir contra el motor correspondiente. |
| Simulador C# WinForms | `csharp_simulador/SimuladorTelefonico.slnx` | Visual Studio | Usa `net10.0-windows` y `UseWindowsForms`; el designer/depuracion visual vive mejor en Visual Studio. |
| WS Autenticacion | `dotnet_webservices/WS_Autenticacion/WS_Autenticacion.sln` | Visual Studio | Es WCF/.NET Framework 4.8 con IIS Express y `Service1.svc`. |
| WS Proveedor | `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/WS_Proveedor.csproj` | Visual Studio | Es WCF/.NET Framework 4.7.2 con IIS Express y `ProveedorService.svc`. |
| WS Proveedor 1 | `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor_1/WS_Proveedor_1.sln` | Visual Studio | Es WCF/.NET Framework 4.7.2 con IIS Express. |
| Host .NET 8 minimo | `dotnet_webservices/CentralTelefonica.WebServices/Program.cs` | VS Code o Visual Studio | Es SDK-style `Microsoft.NET.Sdk.Web`; se puede correr con CLI, pero parece host de prueba. |
| Web Administrativo alcance final | `dotnet_webapps/WebAdministrativo/WebAdministrativo.csproj` | Visual Studio | Proyecto C# Web Forms .NET Framework creado para `ADM6` y `ADM7`. |
| Web Cliente alcance final | `dotnet_webapps/WebCliente/WebCliente.csproj` | Visual Studio | Proyecto C# Web Forms .NET Framework creado para `CLIENTE1`, `CLIENTE2`, `CLIENTE3` y base de `CLIENTE4` a `CLIENTE7`. |

## Mapa por historia de persona 2

| Historia | Que se programa | Herramienta principal | Apoyo en VS Code |
| --- | --- | --- | --- |
| ADM6 | Pantalla `Facturacion.aspx` y consumo de `WS_PROVEEDOR3`. | Visual Studio | Revisar contratos, scripts SQL y Java proveedor. |
| ADM7 | Pantalla `Administradores.aspx` y consumo de `WS_AUTENTICACION2`. | Visual Studio | Revisar Mongo, validadores y documentacion. |
| CLIENTE1 | `Login.aspx` cliente con tipo oculto `2`. | Visual Studio | Revisar contrato de autenticacion y cifrado. |
| CLIENTE2 | `Site.Master`, sesion y menu del portal cliente. | Visual Studio | Coordinar rutas con docs y persona 3. |
| CLIENTE3 | `Registro.aspx` cliente con tipo `2` y estado `activo`. | Visual Studio | Revisar validaciones y evidencias. |

## Escaneo tecnico realizado

### Componentes encontrados

| Componente | Evidencia | Estado para Fase 0 |
| --- | --- | --- |
| Simulador C# | `csharp_simulador/SimuladorTelefonico/SimuladorTelefonico.csproj` | WinForms, no es la Web final. |
| Identificador Python | `python_identificador/main.py` y `python_identificador/app/` | Backend previo por sockets TCP. |
| Proveedor Java | `java_proveedor/Main.java` y `java_proveedor/src/` | Backend proveedor por socket TCP en puerto 6000. |
| WS Autenticacion | `dotnet_webservices/WS_Autenticacion/Service1.svc` | Servicio WCF clasico disponible para login/usuarios. |
| WS Proveedor | `dotnet_webservices/CentralTelefonica.WebServices/WS_Proveedor/ProveedorService.svc` | Servicio WCF clasico disponible para operaciones proveedor. |
| Host .NET 8 | `dotnet_webservices/CentralTelefonica.WebServices/Program.cs` | Host minimo con endpoints de prueba; no sustituye aun los WCF oficiales. |
| Web alcance final | `dotnet_webapps/CentralTelefonica.WebApps.sln`. | Base creada y compilada correctamente. |
| Web Cliente autenticacion | `dotnet_webapps/WebCliente/Services/AutenticacionSoapClient.cs`. | `CLIENTE1` y `CLIENTE3` ya tienen cliente SOAP y cifrado AES configurado. |
| Web Administrativo autenticacion | `dotnet_webapps/WebAdministrativo/Services/AutenticacionSoapClient.cs`. | `ADM7` ya tiene cliente SOAP, cifrado AES y operaciones CRUD/estado configuradas. |
| Web Administrativo facturacion | `dotnet_webapps/WebAdministrativo/Services/ProveedorSoapClient.cs`. | `ADM6` ya consume `ObtenerUltimaFacturacion` y `CalcularFacturacion`. |

### Servicios relevantes para persona 2

| Servicio | Operaciones detectadas | Brecha para alcance final |
| --- | --- | --- |
| `WS_AUTENTICACION` | `AutenticarUsuario`, `AutenticarUsuarioDetalle`, `CrearUsuario`, `ModificarUsuario`, `CambiarEstadoUsuario`, `ListarUsuariosPorTipo`, `EliminarUsuario`. | Operaciones base de persona 2 implementadas; falta probar con IIS Express/Mongo real. |
| `WS_AUTENTICACION` repositorio | `ObtenerTodos`, `ObtenerPorTipo`, `ObtenerPorIdentificacion`, `ObtenerPorUsuarioCifrado`, `Insertar`, `Actualizar`, `EliminarPorIdentificacion`. | Base lista para `ADM7`, `CLIENTE1` y `CLIENTE3`. |
| `WS_PROVEEDOR` | `ActivarDesactivarLinea`, `CalcularFacturacion`, `ObtenerUltimaFacturacion`. | Operaciones base de `ADM6` implementadas; falta probar con IIS Express, Java y SQL Server real. |
| Java Proveedor | `Proveedor6Service` ejecuta `CALCULAR_FACTURACION`. | Se debe validar que responde bien desde WCF y que el SP esta aplicado. |
| SQL Server | `sp_CalcularFacturacionPostpago` en migracion `010_proveedor6_facturacion.sql`. | Falta confirmar datos reales y tabla/consulta de ultima ejecucion. |

## Confirmacion de Fase 0

### 1. Lenguaje de la Web Cliente

Estado: confirmado.

Decision: hacerla en C# igual que la Web Administrativa. Esto permite reutilizar clientes WCF, validaciones, modelos, cifrado y manejo de sesion.

### 2. Estructura de proyectos Web

Estado: confirmado.

Recomendacion practica:

- Crear un proyecto para Web Administrativa: hecho en `dotnet_webapps/WebAdministrativo`.
- Crear un proyecto separado para Web Cliente: hecho en `dotnet_webapps/WebCliente`.

Motivo: el PDF los trata como ambientes distintos y la separacion evita mezclar sesion, menu y autorizacion de administradores/clientes.

Alternativa aceptable: un solo proyecto Web con areas `Admin` y `Cliente`, pero requiere disciplina de rutas y autorizacion.

### 3. Tecnologia sugerida para las nuevas Web

Estado: confirmado.

Decision: ASP.NET Web Forms en C#/.NET Framework desde Visual Studio.

Motivo: es compatible con los servicios WCF/.NET Framework existentes, IIS Express y consumo SOAP directo.

### 4. Consumo de servicios

Estado: definido.

Recomendacion:

- Consumir WCF directamente desde las Web C# cuando sea posible.
- Si el equipo prefiere REST, crear un wrapper .NET 8 formal y documentado, no usar el `Program.cs` actual como si ya estuviera completo.
- No consumir sockets desde las Web.
- No consultar MongoDB, SQL Server o MySQL desde las Web.

### 5. Cifrado de contrasenas

Estado: definido a nivel de codigo.

Observacion:

- `WS_Autenticacion` espera usuario/contrasena cifrados.
- La contrasena se descifra y valida en el servicio.
- La Web debe cifrar antes de enviar.

Implementacion:

- `WebCliente` y `WebAdministrativo` usan `AesKeyBase64` y `AesIvBase64` equivalentes a `WS_Autenticacion`.
- Las Web cifran usuario y contrasena antes de llamar al WS.

### 6. Datos de sesion cliente

Estado: resuelto a nivel de codigo.

Problema:

- `CLIENTE2` necesita mostrar `Hola Nombre`.
- Persona 3 necesitara identificacion del cliente para `CLIENTE4` a `CLIENTE7`.
- La operacion `AutenticarUsuario` revisada retorna resultado/mensaje, pero no garantiza perfil completo en el contrato principal.

Implementacion:

- `WS_AUTENTICACION` expone `AutenticarUsuarioDetalle`.
- `WebCliente` guarda en sesion usuario, identificacion y nombre.

### 7. Configuracion de ambientes

Estado: inconsistente.

Hallazgos:

- `shared/config/puertos.json` define proveedor Java en `127.0.0.1:6000`.
- `WS_Proveedor/Web.config` usa `ProveedorHost=127.0.0.1` y `ProveedorPort=6000`.
- `docs/SETUP.md` menciona SQL Server en `100.88.25.17:49172`.
- `shared/config/database.json` menciona SQL Server en `localhost:49172`.

Accion pendiente:

- Definir un ambiente oficial para pruebas finales: local o Tailscale.
- Ajustar configuraciones antes de tomar evidencias.

## Decision recomendada para arrancar desarrollo

Para persona 2, la decision mas segura es:

1. Programar las nuevas Web en Visual Studio.
2. Crear dos proyectos Web C# separados: Administrativo y Cliente.
3. Consumir WCF para autenticacion y proveedor.
4. Completar primero las brechas de WS:
   - `ListarUsuariosPorTipo`.
   - `EliminarUsuario`.
   - Login con datos basicos de usuario o consulta de perfil.
   - `ObtenerUltimaFacturacion`.
5. Usar VS Code para documentacion, contratos, SQL, Python y Java.

## Checklist para cerrar Fase 0

- [x] Equipo confirma C# para Web Cliente.
- [x] Equipo decide dos proyectos Web o un proyecto con areas.
- [ ] Se confirma Visual Studio como herramienta principal para Web y WCF.
- [ ] Se confirma VS Code para docs, contratos, Python, Java y SQL.
- [ ] Se define si las Web consumen WCF directo o wrapper REST formal.
- [ ] Se congelan URLs/puertos de servicios para pruebas.
- [ ] Se confirma configuracion AES para contrasenas.
- [x] Se define respuesta de login con datos de usuario.
- [x] Se define operacion de listado de administradores.
- [x] Se define operacion de eliminacion de administradores.
- [x] Se define operacion de ultima facturacion.

## Estado actual de Fase 0

Fase 0 queda cerrada a nivel de decisiones y codigo base.

Confirmado:

- La Web Administrativa debe ser C#.
- La Web Cliente tambien sera C#.
- Visual Studio es la herramienta adecuada para Web C# y WCF.
- VS Code es adecuado para backend no visual, contratos y documentacion.
- Existen servicios base de autenticacion y facturacion.
- Ya se agregaron operaciones faltantes para persona 2 en `WS_AUTENTICACION`.
- Ya se agrego `ObtenerUltimaFacturacion` en `WS_PROVEEDOR`.
- Ya se crearon y compilaron las Web base en `dotnet_webapps/CentralTelefonica.WebApps.sln`.
- `WebCliente` ya cifra usuario/contrasena y llama a `WS_AUTENTICACION` para login y registro.
- `WebAdministrativo` ya cifra usuario/contrasena y llama a `WS_AUTENTICACION` para login, listar, crear, editar, activar/inactivar y eliminar administradores.
- `WebAdministrativo` ya llama a `WS_PROVEEDOR` para cargar ultima facturacion y ejecutar calculo.
- No debe haber acceso directo desde Web a bases de datos ni sockets.

Pendiente:

- Unificar configuracion de ambiente.
- Probar servicios desde IIS Express contra MongoDB, Java y SQL Server reales.
- Ejecutar pruebas funcionales reales de `ADM6`, `ADM7`, `CLIENTE1` y `CLIENTE3` con servicios levantados.
- Seguir la guia `docs/testing/pruebas_persona_2_web.md`.
