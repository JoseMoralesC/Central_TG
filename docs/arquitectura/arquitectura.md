# File Tree: Central_TG

**Generated:** 19/8/2026, 11:59:34
**Root Path:** `c:\Users\Personal\Desktop\Central_TG`

```
├── 📁 .agents
├── 📁 .github
│   └── 📁 workflows
├── 📁 artifacts
├── 📁 csharp_simulador
│   ├── 📁 SimuladorTelefonico
│   │   ├── 📁 Config
│   │   │   └── 📄 AppConfig.cs
│   │   ├── 📁 Models
│   │   │   ├── 📄 ConsultaSaldo.cs
│   │   │   ├── 📄 FinalizarLlamada.cs
│   │   │   ├── 📄 InicioLlamada.cs
│   │   │   ├── 📄 RespuestaLlamada.cs
│   │   │   ├── 📄 RespuestaSaldo.cs
│   │   │   ├── 📄 SolicitudLlamada.cs
│   │   │   ├── 📄 TelefonoVirtual.cs
│   │   │   └── 📄 UsuarioMongoResumen.cs
│   │   ├── 📁 Services
│   │   │   ├── 📄 AdministracionTelefonicaService.cs
│   │   │   ├── 📄 AutenticacionCryptoService.cs
│   │   │   ├── 📄 BitacoraService.cs
│   │   │   ├── 📄 CryptoService.cs
│   │   │   ├── 📄 MongoUsuariosConsultaService.cs
│   │   │   ├── 📄 PoliticaTarifaService.cs
│   │   │   ├── 📄 RespuestaService.cs
│   │   │   ├── 📄 TelefonoCatalogoService.cs
│   │   │   └── 📄 TramaService.cs
│   │   ├── 📁 Socket
│   │   │   └── 📄 TcpSocketClient.cs
│   │   ├── 📁 UI
│   │   │   ├── 📄 AdministracionTelefonicaForm.cs
│   │   │   ├── 📄 BitacoraForm.cs
│   │   │   ├── 📄 ConsultaSaldoForm.cs
│   │   │   ├── 📄 LlamadaActivaForm.cs
│   │   │   ├── 📄 MarcarNumeroForm.cs
│   │   │   ├── 📄 SeleccionTelefonoForm.cs
│   │   │   └── 📄 UiTheme.cs
│   │   ├── 📁 Utils
│   │   │   └── 📄 ValidacionTelefono.cs
│   │   ├── 📄 Form1.Designer.cs
│   │   ├── 📄 Form1.cs
│   │   ├── 📄 Program.cs
│   │   └── 📄 SimuladorTelefonico.csproj
│   ├── 📝 README.md
│   └── 📄 SimuladorTelefonico.slnx
├── 📁 database
│   ├── 📁 mongodb
│   │   ├── 📝 README.md
│   │   ├── 📄 actualizar_indices_usuarios_por_tipo.js
│   │   ├── 📄 crear_coleccion_usuarios.js
│   │   ├── 📄 datos_semilla_persona_2.js
│   │   ├── 📄 datos_semilla_usuarios.example.js
│   │   └── 📄 indices_usuarios.js
│   ├── 📁 mysql_identificador
│   │   ├── 📁 backups
│   │   │   └── ⚙️ .gitkeep
│   │   ├── 📁 migrations
│   │   ├── 📁 schema
│   │   └── 📁 seed
│   │       └── ⚙️ .gitkeep
│   ├── 📁 sqlserver_proveedor
│   │   ├── 📁 backups
│   │   │   └── ⚙️ .gitkeep
│   │   ├── 📁 migrations
│   │   ├── 📁 schema
│   │   └── 📁 seed
│   │       └── ⚙️ .gitkeep
│   └── ⚙️ .gitkeep
├── 📁 docs
│   ├── 📁 Proyecto
│   │   ├── 📕 Proyecto - Alcance 1.pdf
│   │   ├── 📕 Proyecto - Alcance 2.pdf
│   │   └── 📕 Proyecto final.pdf
│   ├── 📁 arquitectura
│   │   ├── 📝 contrato_json_integrado.md
│   │   └── 📝 protocolo_comunicacion.md
│   ├── 📁 auditoria
│   │   ├── 📝 Informe_de_analisis_1.md
│   │   ├── 📝 Informe_de_analisis_3.md
│   │   ├── 📝 informe 1_avance_alcance_2_dev.md
│   │   ├── 📝 informe 2 avance alcance 2 dev .md
│   │   ├── 📝 informe 3 avance alcance 2 dev .md
│   │   ├── 📝 informe 6_avance_alcance_final_dev.md
│   │   ├── 📝 informe 7_estado_actual_vs_informe_6_dev.md
│   │   └── 📝 informe 8_estado_actual_vs_informe_7_y_proyecto_final.md
│   ├── 📁 contratos
│   │   └── 📝 jose_activacion_desactivacion.md
│   ├── 📁 entrega_final
│   │   ├── 📝 Arquitectura.md
│   │   ├── 📝 Flujo_general.md
│   │   ├── 📝 Historias_completadas.md
│   │   ├── 📝 Instalacion.md
│   │   ├── 📝 Pruebas.md
│   │   └── 📝 Ruta_demo_alcance_final.md
│   ├── 📁 estrategia
│   │   ├── 📝 estrategia_1.md
│   │   ├── 📝 estrategia_2.md
│   │   ├── 📝 estrategia_3.md
│   │   └── 📝 orden.md
│   ├── 📁 evidencias
│   │   ├── 📁 charlie
│   │   │   ├── 🖼️ ReAutenticarUsuarioActivo.PNG
│   │   │   ├── 🖼️ autenticarusuario.png
│   │   │   ├── 🖼️ cambiarestadousuario_inactivar.PNG
│   │   │   ├── 🖼️ crearusuario.PNG
│   │   │   ├── 🖼️ facturacion_sql.png
│   │   │   ├── 🖼️ modificarusuario.png
│   │   │   ├── 🖼️ pruebaautenticarusuarioinactivo.PNG
│   │   │   └── 🖼️ reactivarusuario.PNG
│   │   ├── 📁 gabriel
│   │   │   ├── 📁 CLIENTE7
│   │   │   │   └── 📝 CLIENTE7_devolucion_linea.md
│   │   │   ├── 🖼️ ConsultaSaldoCorrectaCompleta.PNG
│   │   │   ├── 🖼️ PruebaRegistroInvalidoDatosIncorrectos.PNG
│   │   │   ├── 🖼️ PruebaRegistroInvalidoDatosIncorrectos2.PNG
│   │   │   ├── 🖼️ RegistrarLinea.PNG
│   │   │   └── 🖼️ consultarSaldo.PNG
│   │   └── 📁 jose
│   │       ├── 📁 IDENTIFICADOR6
│   │       │   ├── ⚙️ .gitkeep
│   │       │   └── 📝 prueba_identificador6.md
│   │       ├── 📁 PROVEEDOR5
│   │       │   ├── ⚙️ .gitkeep
│   │       │   └── 📝 prueba_proveedor5.md
│   │       ├── 📁 WS_PROVEEDOR2
│   │       │   ├── ⚙️ .gitkeep
│   │       │   └── 📝 prueba_ws_proveedor2.md
│   │       ├── 📁 integracion
│   │       │   └── ⚙️ .gitkeep
│   │       ├── 📁 mysql
│   │       │   └── ⚙️ .gitkeep
│   │       ├── 📁 sqlserver
│   │       │   └── ⚙️ .gitkeep
│   │       ├── 🖼️ Activarinea.PNG
│   │       └── 🖼️ InactivarLinea.PNG
│   ├── 📁 roadmaps
│   │   ├── 📝 analisis_tecnico_integral.md
│   │   ├── 📝 guia_ejecucion_persona_2_web.md
│   │   ├── 📝 guia_levantamiento_terminales.md
│   │   ├── 📝 mapeo_herramientas_fase0_persona_2.md
│   │   ├── 📝 roadmap_2.md
│   │   └── 📝 roadmap_persona_2_alcance_final.md
│   ├── 📁 testing
│   │   ├── 📝 casos_prueba.md
│   │   ├── 📝 pruebas_integradas_oficiales.md
│   │   └── 📝 pruebas_persona_2_web.md
│   ├── 📝 Requisitos_ejecucion.md
│   └── 📝 SETUP.md
├── 📁 dotnet_webapps
│   ├── 📁 WebAdministrativo
│   │   ├── 📁 Assets
│   │   │   └── 🖼️ Logo.png
│   │   ├── 📁 Services
│   │   │   ├── 📄 AutenticacionSoapClient.cs
│   │   │   ├── 📄 CryptoHelper.cs
│   │   │   ├── 📄 EmailService.cs
│   │   │   ├── 📄 ProveedorCryptoHelper.cs
│   │   │   └── 📄 ProveedorSoapClient.cs
│   │   ├── 📁 Styles
│   │   │   └── 🎨 site.css
│   │   ├── 📄 Administradores.aspx
│   │   ├── 📄 Administradores.aspx.cs
│   │   ├── 📄 Administradores.aspx.designer.cs
│   │   ├── 📄 Default.aspx
│   │   ├── 📄 Default.aspx.cs
│   │   ├── 📄 Facturacion.aspx
│   │   ├── 📄 Facturacion.aspx.cs
│   │   ├── 📄 Facturacion.aspx.designer.cs
│   │   ├── 📄 Global.asax
│   │   ├── 📄 Global.asax.cs
│   │   ├── 📄 LineasActivar.aspx
│   │   ├── 📄 LineasActivar.aspx.cs
│   │   ├── 📄 LineasActivar.aspx.designer.cs
│   │   ├── 📄 LineasDevolucion.aspx
│   │   ├── 📄 LineasDevolucion.aspx.cs
│   │   ├── 📄 LineasDevolucion.aspx.designer.cs
│   │   ├── 📄 LineasNuevas.aspx
│   │   ├── 📄 LineasNuevas.aspx.cs
│   │   ├── 📄 LineasNuevas.aspx.designer.cs
│   │   ├── 📄 Login.aspx
│   │   ├── 📄 Login.aspx.cs
│   │   ├── 📄 Login.aspx.designer.cs
│   │   ├── 📄 Site.Master
│   │   ├── 📄 Site.Master.cs
│   │   ├── 📄 Site.Master.designer.cs
│   │   ├── 📄 SolicitudesLineas.aspx
│   │   ├── 📄 SolicitudesLineas.aspx.cs
│   │   ├── 📄 SolicitudesLineas.aspx.designer.cs
│   │   ├── ⚙️ Web.config
│   │   └── 📄 WebAdministrativo.csproj
│   ├── 📁 WebCliente
│   │   ├── 📁 Assets
│   │   │   └── 🖼️ Logo.png
│   │   ├── 📁 Services
│   │   │   ├── 📄 AutenticacionSoapClient.cs
│   │   │   └── 📄 CryptoHelper.cs
│   │   ├── 📁 Styles
│   │   │   └── 🎨 site.css
│   │   ├── 📄 Default.aspx
│   │   ├── 📄 Default.aspx.cs
│   │   ├── 📄 Global.asax
│   │   ├── 📄 Global.asax.cs
│   │   ├── 📄 Lineas.aspx
│   │   ├── 📄 Lineas.aspx.cs
│   │   ├── 📄 Lineas.aspx.designer.cs
│   │   ├── 📄 Login.aspx
│   │   ├── 📄 Login.aspx.cs
│   │   ├── 📄 Login.aspx.designer.cs
│   │   ├── 📄 Portal.aspx
│   │   ├── 📄 Portal.aspx.cs
│   │   ├── 📄 Portal.aspx.designer.cs
│   │   ├── 📄 Registro.aspx
│   │   ├── 📄 Registro.aspx.cs
│   │   ├── 📄 Registro.aspx.designer.cs
│   │   ├── 📄 Site.Master
│   │   ├── 📄 Site.Master.cs
│   │   ├── 📄 Site.Master.designer.cs
│   │   ├── ⚙️ Web.config
│   │   └── 📄 WebCliente.csproj
│   └── 📄 CentralTelefonica.WebApps.sln
├── 📁 dotnet_webservices
│   ├── 📁 CentralTelefonica.WebServices
│   │   ├── 📁 WS_Proveedor
│   │   │   ├── 📁 App_Data
│   │   │   ├── 📁 Contracts
│   │   │   │   ├── 📄 IProveedorService.cs
│   │   │   │   ├── 📄 TramaProveedor5.cs
│   │   │   │   └── 📄 TramaProveedor6.cs
│   │   │   ├── 📁 Infrastructure
│   │   │   │   ├── 📄 ProveedorClientOptions.cs
│   │   │   │   └── 📄 ProveedorTcpClient.cs
│   │   │   ├── 📁 Models
│   │   │   │   ├── 📄 ActivarDesactivarLineaRequest.cs
│   │   │   │   ├── 📄 ActualizarCorreoClienteRequest.cs
│   │   │   │   ├── 📄 CalcularFacturacionRequest.cs
│   │   │   │   ├── 📄 FacturacionConsultaResponse.cs
│   │   │   │   ├── 📄 FacturacionResponse.cs
│   │   │   │   ├── 📄 LineaAdministrativaDto.cs
│   │   │   │   ├── 📄 ListadoLineasResponse.cs
│   │   │   │   ├── 📄 ListadoSolicitudesLineaResponse.cs
│   │   │   │   ├── 📄 RegistrarLineaAdministrativaRequest.cs
│   │   │   │   ├── 📄 RegistrarLineaRequest.cs
│   │   │   │   ├── 📄 RespuestaServicio.cs
│   │   │   │   ├── 📄 SolicitarLineaClienteRequest.cs
│   │   │   │   ├── 📄 SolicitudLineaDto.cs
│   │   │   │   └── 📄 UltimaFacturacionResponse.cs
│   │   │   ├── 📁 Properties
│   │   │   │   └── 📄 AssemblyInfo.cs
│   │   │   ├── 📁 Services
│   │   │   │   ├── 📄 ProveedorCryptoService.cs
│   │   │   │   ├── 📄 ProveedorService.cs
│   │   │   │   ├── 📄 TramaProveedor6Service.cs
│   │   │   │   └── 📄 TramaProveedorService.cs
│   │   │   ├── 📁 Validators
│   │   │   │   ├── 📄 ActivarDesactivarLineaValidator.cs
│   │   │   │   └── 📄 CalcularFacturacionValidator.cs
│   │   │   ├── 📁 test-client
│   │   │   │   └── 📄 ProveedorClient.cs
│   │   │   ├── 📄 Default.aspx
│   │   │   ├── 📄 Default.aspx.cs
│   │   │   ├── 📄 IProveedorService.cs
│   │   │   ├── 📄 ProveedorService.svc
│   │   │   ├── 📄 ProveedorService.svc.cs
│   │   │   ├── 📄 WS_Proveedor.csproj
│   │   │   ├── ⚙️ Web.Debug.config
│   │   │   ├── ⚙️ Web.Release.config
│   │   │   └── ⚙️ Web.config
│   │   ├── 📁 WS_ProveedorCliente
│   │   │   ├── 📁 Models
│   │   │   │   ├── 📄 ConsultarLineasClienteResponse.cs
│   │   │   │   ├── 📄 LineaClienteDto.cs
│   │   │   │   ├── 📄 PagarFacturaResponse.cs
│   │   │   │   └── 📄 RecargarSaldoResponse.cs
│   │   │   ├── 📁 Properties
│   │   │   │   └── 📄 AssemblyInfo.cs
│   │   │   ├── 📁 Services
│   │   │   │   ├── 📄 CryptoAes.cs
│   │   │   │   └── 📄 LineaClienteService.cs
│   │   │   ├── 📄 IProveedorClienteService.cs
│   │   │   ├── 📄 ProveedorClienteService.svc
│   │   │   ├── 📄 ProveedorClienteService.svc.cs
│   │   │   ├── 📄 WS_ProveedorCliente.csproj
│   │   │   ├── 📄 WS_ProveedorCliente.sln
│   │   │   └── ⚙️ Web.config
│   │   ├── 📁 WS_Proveedor_1
│   │   │   ├── 📁 Models
│   │   │   │   ├── 📄 ConsultarSaldoRequest.cs
│   │   │   │   ├── 📄 ConsultarSaldoResponse.cs
│   │   │   │   ├── 📄 Proovedor_telefono.cs
│   │   │   │   ├── 📄 Registrar_linea.cs
│   │   │   │   ├── 📄 RespuestaProveedor.cs
│   │   │   │   └── 📄 ResultadoProveedor.cs
│   │   │   ├── 📁 Properties
│   │   │   │   └── 📄 AssemblyInfo.cs
│   │   │   ├── 📁 Services
│   │   │   │   ├── 📄 CryptoAES.cs
│   │   │   │   └── 📄 ProveedorTcpCliente.cs
│   │   │   ├── 📁 packages
│   │   │   │   └── 📁 Newtonsoft.Json.13.0.4
│   │   │   │       ├── 📁 lib
│   │   │   │       │   ├── 📁 net20
│   │   │   │       │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │       │   ├── 📁 net35
│   │   │   │       │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │       │   ├── 📁 net40
│   │   │   │       │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │       │   ├── 📁 net45
│   │   │   │       │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │       │   ├── 📁 net6.0
│   │   │   │       │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │       │   ├── 📁 netstandard1.0
│   │   │   │       │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │       │   ├── 📁 netstandard1.3
│   │   │   │       │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │       │   └── 📁 netstandard2.0
│   │   │   │       │       └── ⚙️ Newtonsoft.Json.xml
│   │   │   │       ├── ⚙️ .signature.p7s
│   │   │   │       ├── 📝 LICENSE.md
│   │   │   │       ├── 📝 README.md
│   │   │   │       └── 🖼️ packageIcon.png
│   │   │   ├── 📄 IService1.cs
│   │   │   ├── 📄 Requerimentos.txt
│   │   │   ├── 📄 Service1.svc
│   │   │   ├── 📄 Service1.svc.cs
│   │   │   ├── 📄 WS_Proveedor_1.csproj
│   │   │   ├── 📄 WS_Proveedor_1.sln
│   │   │   ├── ⚙️ Web.Debug.config
│   │   │   ├── ⚙️ Web.Release.config
│   │   │   ├── ⚙️ Web.config
│   │   │   └── ⚙️ packages.config
│   │   ├── 📄 CentralTelefonica.WebServices.csproj
│   │   ├── 📄 CentralTelefonica.WebServices.slnx
│   │   ├── 📄 MinimalHost.cs
│   │   ├── 📄 Program.cs
│   │   └── ⚙️ appsettings.json
│   ├── 📁 PortalCliente
│   │   ├── 📁 Controllers
│   │   │   ├── 📄 ClienteController.cs
│   │   │   └── 📄 HomeController.cs
│   │   ├── 📁 Models
│   │   │   ├── 📄 AccionLineaViewModel.cs
│   │   │   ├── 📄 ActivarDesactivarLineaPortal.cs
│   │   │   ├── 📄 CargarSaldoViewModel.cs
│   │   │   ├── 📄 ConsultarLineasClienteResult.cs
│   │   │   ├── 📄 ConsultarSaldoPortalResult.cs
│   │   │   ├── 📄 DevolverLineaViewModel.cs
│   │   │   ├── 📄 ErrorViewModel.cs
│   │   │   ├── 📄 IndexViewModel.cs
│   │   │   ├── 📄 LineaCliente.cs
│   │   │   ├── 📄 LineaDisponible.cs
│   │   │   ├── 📄 ListarLineasDisponiblesResult.cs
│   │   │   ├── 📄 MetodoPagoCliente.cs
│   │   │   ├── 📄 PagarFacturaResult.cs
│   │   │   ├── 📄 PagarFacturaViewModel.cs
│   │   │   ├── 📄 PerfilClienteViewModel.cs
│   │   │   ├── 📄 RecargarSaldoResult.cs
│   │   │   └── 📄 SolicitarLineaViewModel.cs
│   │   ├── 📁 Properties
│   │   │   └── ⚙️ launchSettings.json
│   │   ├── 📁 Services
│   │   │   ├── 📄 AutenticacionPortalSoapClient.cs
│   │   │   ├── 📄 CryptoHelper.cs
│   │   │   ├── 📄 EmailService.cs
│   │   │   ├── 📄 IAutenticacionPortalService.cs
│   │   │   ├── 📄 IProveedor2Service.cs
│   │   │   ├── 📄 IProveedorClienteService.cs
│   │   │   ├── 📄 IProveedorPortalService.cs
│   │   │   ├── 📄 Proveedor2SoapClient.cs
│   │   │   ├── 📄 ProveedorClienteSoapClient.cs
│   │   │   └── 📄 ProveedorPortalSoapClient.cs
│   │   ├── 📁 Views
│   │   │   ├── 📁 Cliente
│   │   │   │   ├── 📄 CargarSaldo.cshtml
│   │   │   │   ├── 📄 DevolverLinea.cshtml
│   │   │   │   ├── 📄 Index.cshtml
│   │   │   │   ├── 📄 PagarFactura.cshtml
│   │   │   │   ├── 📄 Perfil.cshtml
│   │   │   │   └── 📄 SolicitarLinea.cshtml
│   │   │   ├── 📁 Home
│   │   │   │   ├── 📄 Index.cshtml
│   │   │   │   └── 📄 Privacy.cshtml
│   │   │   ├── 📁 Shared
│   │   │   │   ├── 📄 Error.cshtml
│   │   │   │   ├── 📄 _Layout.cshtml
│   │   │   │   ├── 🎨 _Layout.cshtml.css
│   │   │   │   └── 📄 _ValidationScriptsPartial.cshtml
│   │   │   ├── 📄 _ViewImports.cshtml
│   │   │   └── 📄 _ViewStart.cshtml
│   │   ├── 📁 wwwroot
│   │   │   ├── 📁 css
│   │   │   │   └── 🎨 site.css
│   │   │   ├── 📁 img
│   │   │   │   └── 🖼️ Logo.png
│   │   │   ├── 📁 js
│   │   │   │   └── 📄 site.js
│   │   │   ├── 📁 lib
│   │   │   │   ├── 📁 bootstrap
│   │   │   │   │   └── 📄 LICENSE
│   │   │   │   ├── 📁 jquery
│   │   │   │   │   └── 📄 LICENSE.txt
│   │   │   │   ├── 📁 jquery-validation
│   │   │   │   │   └── 📝 LICENSE.md
│   │   │   │   └── 📁 jquery-validation-unobtrusive
│   │   │   │       ├── 📄 LICENSE.txt
│   │   │   │       └── 📄 jquery.validate.unobtrusive.js
│   │   │   └── 📄 favicon.ico
│   │   ├── 📄 PortalCliente.csproj
│   │   ├── 📄 PortalCliente.slnx
│   │   ├── 📄 Program.cs
│   │   └── ⚙️ appsettings.json
│   ├── 📁 WS_Autenticacion
│   │   ├── 📁 Data
│   │   │   └── 📄 Usuariorepository.cs
│   │   ├── 📁 Models
│   │   │   ├── 📄 AutenticacionRequest.cs
│   │   │   ├── 📄 AutenticacionResponse.cs
│   │   │   └── 📄 Usuario.cs
│   │   ├── 📁 Properties
│   │   │   └── 📄 AssemblyInfo.cs
│   │   ├── 📁 Security
│   │   │   └── 📄 Cryptohelper.cs
│   │   ├── 📁 Services
│   │   │   └── 📄 IAutenticacionService1.cs
│   │   ├── 📁 Validators
│   │   │   └── 📄 UsuarioValidator.cs
│   │   ├── 📁 packages
│   │   │   ├── 📁 DnsClient.1.6.1
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   └── ⚙️ DnsClient.xml
│   │   │   │   │   ├── 📁 net471
│   │   │   │   │   │   └── ⚙️ DnsClient.xml
│   │   │   │   │   ├── 📁 net5.0
│   │   │   │   │   │   └── ⚙️ DnsClient.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   └── ⚙️ DnsClient.xml
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ DnsClient.xml
│   │   │   │   │   └── 📁 netstandard2.1
│   │   │   │   │       └── ⚙️ DnsClient.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   └── 🖼️ icon.png
│   │   │   ├── 📁 Microsoft.Bcl.AsyncInterfaces.8.0.0
│   │   │   │   ├── 📁 buildTransitive
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── 📄 Microsoft.Bcl.AsyncInterfaces.targets
│   │   │   │   │   └── 📁 net462
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   │   └── ⚙️ Microsoft.Bcl.AsyncInterfaces.xml
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ Microsoft.Bcl.AsyncInterfaces.xml
│   │   │   │   │   └── 📁 netstandard2.1
│   │   │   │   │       └── ⚙️ Microsoft.Bcl.AsyncInterfaces.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   ├── 📄 LICENSE.TXT
│   │   │   │   ├── 📝 PACKAGE.md
│   │   │   │   ├── 📄 THIRD-PARTY-NOTICES.TXT
│   │   │   │   └── 📄 useSharedDesignerContext.txt
│   │   │   ├── 📁 Microsoft.Extensions.Logging.Abstractions.2.0.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 netstandard2.0
│   │   │   │   │       └── ⚙️ Microsoft.Extensions.Logging.Abstractions.xml
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   ├── 📁 Microsoft.Win32.Registry.5.0.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   └── 📁 netstandard2.0
│   │   │   │   │       └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │   └── 📁 netstandard2.0
│   │   │   │   │       └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net46
│   │   │   │   │           ├── 📁 net461
│   │   │   │   │           │   └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   │           ├── 📁 netstandard1.3
│   │   │   │   │           └── 📁 netstandard2.0
│   │   │   │   │               └── ⚙️ Microsoft.Win32.Registry.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   ├── 📄 LICENSE.TXT
│   │   │   │   ├── 📄 THIRD-PARTY-NOTICES.TXT
│   │   │   │   ├── 📄 useSharedDesignerContext.txt
│   │   │   │   └── 📄 version.txt
│   │   │   ├── 📁 MongoDB.Bson.3.10.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net472
│   │   │   │   │   │   └── ⚙️ MongoDB.Bson.xml
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   │   └── ⚙️ MongoDB.Bson.xml
│   │   │   │   │   └── 📁 netstandard2.1
│   │   │   │   │       └── ⚙️ MongoDB.Bson.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📝 README.md
│   │   │   │   └── 🖼️ packageIcon.png
│   │   │   ├── 📁 MongoDB.Driver.3.10.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net472
│   │   │   │   │   │   └── ⚙️ MongoDB.Driver.xml
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   │   └── ⚙️ MongoDB.Driver.xml
│   │   │   │   │   └── 📁 netstandard2.1
│   │   │   │   │       └── ⚙️ MongoDB.Driver.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📝 README.md
│   │   │   │   └── 🖼️ packageIcon.png
│   │   │   ├── 📁 SharpCompress.0.48.1
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net10.0
│   │   │   │   │   ├── 📁 net48
│   │   │   │   │   ├── 📁 net5.0
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   ├── 📁 net7.0
│   │   │   │   │   ├── 📁 net8.0
│   │   │   │   │   ├── 📁 net9.0
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   └── 📁 netstandard2.1
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   └── 📝 README.md
│   │   │   ├── 📁 Snappier.1.3.1
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net472
│   │   │   │   │   │   └── ⚙️ Snappier.xml
│   │   │   │   │   ├── 📁 net8.0
│   │   │   │   │   │   └── ⚙️ Snappier.xml
│   │   │   │   │   └── 📁 netstandard2.0
│   │   │   │   │       └── ⚙️ Snappier.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📝 README.md
│   │   │   │   └── 🖼️ icon.png
│   │   │   ├── 📁 System.Buffers.4.6.1
│   │   │   │   ├── 📁 buildTransitive
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── 📄 System.Buffers.targets
│   │   │   │   │   └── 📁 net462
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   │   └── ⚙️ System.Buffers.xml
│   │   │   │   │   ├── 📁 netcoreapp2.0
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Buffers.xml
│   │   │   │   │   └── 📁 netstandard2.1
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   └── 📝 PACKAGE.md
│   │   │   ├── 📁 System.Diagnostics.DiagnosticSource.6.0.1
│   │   │   │   ├── 📁 buildTransitive
│   │   │   │   │   ├── 📁 netcoreapp2.0
│   │   │   │   │   │   └── 📄 System.Diagnostics.DiagnosticSource.targets
│   │   │   │   │   └── 📁 netcoreapp3.1
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── ⚙️ System.Diagnostics.DiagnosticSource.xml
│   │   │   │   │   ├── 📁 net5.0
│   │   │   │   │   │   └── ⚙️ System.Diagnostics.DiagnosticSource.xml
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   │   └── ⚙️ System.Diagnostics.DiagnosticSource.xml
│   │   │   │   │   └── 📁 netstandard2.0
│   │   │   │   │       └── ⚙️ System.Diagnostics.DiagnosticSource.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   ├── 📄 LICENSE.TXT
│   │   │   │   ├── 📄 THIRD-PARTY-NOTICES.TXT
│   │   │   │   └── 📄 useSharedDesignerContext.txt
│   │   │   ├── 📁 System.IO.4.3.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   ├── 📁 portable-net45+win8+wp8+wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 win8
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wp80
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   ├── 📁 netcore50
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   ├── 📁 netstandard1.0
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   ├── 📁 netstandard1.5
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   │   └── ⚙️ System.IO.xml
│   │   │   │   │   ├── 📁 portable-net45+win8+wp8+wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 win8
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wp80
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 ThirdPartyNotices.txt
│   │   │   │   └── 📄 dotnet_library_license.txt
│   │   │   ├── 📁 System.Memory.4.6.3
│   │   │   │   ├── 📁 buildTransitive
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── 📄 System.Memory.targets
│   │   │   │   │   └── 📁 net462
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   │   └── ⚙️ System.Memory.xml
│   │   │   │   │   ├── 📁 netcoreapp2.1
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Memory.xml
│   │   │   │   │   └── 📁 netstandard2.1
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   └── 📝 PACKAGE.md
│   │   │   ├── 📁 System.Net.Http.4.3.4
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 Xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 monoandroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 monotouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 portable-net45+win8+wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 win8
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 Xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 monoandroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 monotouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 netcore50
│   │   │   │   │   ├── 📁 netstandard1.1
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   ├── 📁 portable-net45+win8+wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 win8
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   ├── 📁 unix
│   │   │   │   │   │   └── 📁 lib
│   │   │   │   │   │       └── 📁 netstandard1.6
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net46
│   │   │   │   │           ├── 📁 netcore50
│   │   │   │   │           └── 📁 netstandard1.3
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 ThirdPartyNotices.txt
│   │   │   │   └── 📄 dotnet_library_license.txt
│   │   │   ├── 📁 System.Numerics.Vectors.4.6.1
│   │   │   │   ├── 📁 buildTransitive
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── 📄 System.Numerics.Vectors.targets
│   │   │   │   │   └── 📁 net462
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   │   └── ⚙️ System.Numerics.Vectors.xml
│   │   │   │   │   ├── 📁 netcoreapp2.0
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Numerics.Vectors.xml
│   │   │   │   │   └── 📁 netstandard2.1
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   └── 📝 PACKAGE.md
│   │   │   ├── 📁 System.Runtime.4.3.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   ├── 📁 portable-net45+win8+wp80+wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 win8
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wp80
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   ├── 📁 netcore50
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   ├── 📁 netstandard1.0
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   ├── 📁 netstandard1.2
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   ├── 📁 netstandard1.5
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   │   └── ⚙️ System.Runtime.xml
│   │   │   │   │   ├── 📁 portable-net45+win8+wp80+wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 win8
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wp80
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 wpa81
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 ThirdPartyNotices.txt
│   │   │   │   └── 📄 dotnet_library_license.txt
│   │   │   ├── 📁 System.Runtime.CompilerServices.Unsafe.6.1.2
│   │   │   │   ├── 📁 buildTransitive
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── 📄 System.Runtime.CompilerServices.Unsafe.targets
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 netcoreapp2.0
│   │   │   │   │       └── 📄 System.Runtime.CompilerServices.Unsafe.targets
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   │   └── ⚙️ System.Runtime.CompilerServices.Unsafe.xml
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   │   └── ⚙️ System.Runtime.CompilerServices.Unsafe.xml
│   │   │   │   │   ├── 📁 net7.0
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 netstandard2.0
│   │   │   │   │       └── ⚙️ System.Runtime.CompilerServices.Unsafe.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   └── 📝 PACKAGE.md
│   │   │   ├── 📁 System.Runtime.InteropServices.RuntimeInformation.4.3.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   ├── 📁 netstandard1.1
│   │   │   │   │   ├── 📁 win8
│   │   │   │   │   ├── 📁 wpa81
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 netstandard1.1
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   ├── 📁 aot
│   │   │   │   │   │   └── 📁 lib
│   │   │   │   │   │       └── 📁 netcore50
│   │   │   │   │   ├── 📁 unix
│   │   │   │   │   │   └── 📁 lib
│   │   │   │   │   │       └── 📁 netstandard1.1
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net45
│   │   │   │   │           ├── 📁 netcore50
│   │   │   │   │           └── 📁 netstandard1.1
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 ThirdPartyNotices.txt
│   │   │   │   └── 📄 dotnet_library_license.txt
│   │   │   ├── 📁 System.Security.AccessControl.5.0.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   └── 📁 uap10.0.16299
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │   └── 📁 uap10.0.16299
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net46
│   │   │   │   │           ├── 📁 net461
│   │   │   │   │           │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │           ├── 📁 netcoreapp2.0
│   │   │   │   │           │   └── ⚙️ System.Security.AccessControl.xml
│   │   │   │   │           ├── 📁 netstandard1.3
│   │   │   │   │           └── 📁 uap10.0.16299
│   │   │   │   │               └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   ├── 📄 LICENSE.TXT
│   │   │   │   ├── 📄 THIRD-PARTY-NOTICES.TXT
│   │   │   │   ├── 📄 useSharedDesignerContext.txt
│   │   │   │   └── 📄 version.txt
│   │   │   ├── 📁 System.Security.Cryptography.Algorithms.4.3.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   ├── 📁 net463
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   ├── 📁 net463
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   ├── 📁 netstandard1.4
│   │   │   │   │   ├── 📁 netstandard1.6
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   ├── 📁 osx
│   │   │   │   │   │   └── 📁 lib
│   │   │   │   │   │       └── 📁 netstandard1.6
│   │   │   │   │   ├── 📁 unix
│   │   │   │   │   │   └── 📁 lib
│   │   │   │   │   │       └── 📁 netstandard1.6
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net46
│   │   │   │   │           ├── 📁 net461
│   │   │   │   │           ├── 📁 net463
│   │   │   │   │           ├── 📁 netcore50
│   │   │   │   │           └── 📁 netstandard1.6
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 ThirdPartyNotices.txt
│   │   │   │   └── 📄 dotnet_library_license.txt
│   │   │   ├── 📁 System.Security.Cryptography.Encoding.4.3.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.Encoding.xml
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   ├── 📁 unix
│   │   │   │   │   │   └── 📁 lib
│   │   │   │   │   │       └── 📁 netstandard1.3
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net46
│   │   │   │   │           └── 📁 netstandard1.3
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 ThirdPartyNotices.txt
│   │   │   │   └── 📄 dotnet_library_license.txt
│   │   │   ├── 📁 System.Security.Cryptography.Primitives.4.3.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 ThirdPartyNotices.txt
│   │   │   │   └── 📄 dotnet_library_license.txt
│   │   │   ├── 📁 System.Security.Cryptography.X509Certificates.4.3.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   ├── 📁 netstandard1.4
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   │   └── ⚙️ System.Security.Cryptography.X509Certificates.xml
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   ├── 📁 unix
│   │   │   │   │   │   └── 📁 lib
│   │   │   │   │   │       └── 📁 netstandard1.6
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net46
│   │   │   │   │           ├── 📁 net461
│   │   │   │   │           ├── 📁 netcore50
│   │   │   │   │           └── 📁 netstandard1.6
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 ThirdPartyNotices.txt
│   │   │   │   └── 📄 dotnet_library_license.txt
│   │   │   ├── 📁 System.Security.Principal.Windows.5.0.0
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   └── 📁 uap10.0.16299
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 net46
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   ├── 📁 netcoreapp3.0
│   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   ├── 📁 de
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   ├── 📁 es
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   ├── 📁 fr
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   ├── 📁 it
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   ├── 📁 ja
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   ├── 📁 ko
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   ├── 📁 ru
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   ├── 📁 zh-hans
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   ├── 📁 zh-hant
│   │   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   └── 📁 uap10.0.16299
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   ├── 📁 unix
│   │   │   │   │   │   └── 📁 lib
│   │   │   │   │   │       ├── 📁 netcoreapp2.0
│   │   │   │   │   │       │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   │       └── 📁 netcoreapp2.1
│   │   │   │   │   │           └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net46
│   │   │   │   │           ├── 📁 net461
│   │   │   │   │           │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │           ├── 📁 netcoreapp2.0
│   │   │   │   │           │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │           ├── 📁 netcoreapp2.1
│   │   │   │   │           │   └── ⚙️ System.Security.Principal.Windows.xml
│   │   │   │   │           ├── 📁 netstandard1.3
│   │   │   │   │           └── 📁 uap10.0.16299
│   │   │   │   │               └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   ├── 📄 LICENSE.TXT
│   │   │   │   ├── 📄 THIRD-PARTY-NOTICES.TXT
│   │   │   │   ├── 📄 useSharedDesignerContext.txt
│   │   │   │   └── 📄 version.txt
│   │   │   ├── 📁 System.Text.Encoding.CodePages.8.0.0
│   │   │   │   ├── 📁 buildTransitive
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── 📄 System.Text.Encoding.CodePages.targets
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 netcoreapp2.0
│   │   │   │   │       └── 📄 System.Text.Encoding.CodePages.targets
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net462
│   │   │   │   │   │   └── ⚙️ System.Text.Encoding.CodePages.xml
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   │   └── ⚙️ System.Text.Encoding.CodePages.xml
│   │   │   │   │   ├── 📁 net7.0
│   │   │   │   │   │   └── ⚙️ System.Text.Encoding.CodePages.xml
│   │   │   │   │   ├── 📁 net8.0
│   │   │   │   │   │   └── ⚙️ System.Text.Encoding.CodePages.xml
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Text.Encoding.CodePages.xml
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 runtimes
│   │   │   │   │   └── 📁 win
│   │   │   │   │       └── 📁 lib
│   │   │   │   │           ├── 📁 net6.0
│   │   │   │   │           │   └── ⚙️ System.Text.Encoding.CodePages.xml
│   │   │   │   │           ├── 📁 net7.0
│   │   │   │   │           │   └── ⚙️ System.Text.Encoding.CodePages.xml
│   │   │   │   │           └── 📁 net8.0
│   │   │   │   │               └── ⚙️ System.Text.Encoding.CodePages.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ Icon.png
│   │   │   │   ├── 📄 LICENSE.TXT
│   │   │   │   ├── 📝 PACKAGE.md
│   │   │   │   ├── 📄 THIRD-PARTY-NOTICES.TXT
│   │   │   │   └── 📄 useSharedDesignerContext.txt
│   │   │   ├── 📁 System.Threading.Tasks.Extensions.4.5.4
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 net461
│   │   │   │   │   │   └── ⚙️ System.Threading.Tasks.Extensions.xml
│   │   │   │   │   ├── 📁 netcoreapp2.1
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 netstandard1.0
│   │   │   │   │   │   └── ⚙️ System.Threading.Tasks.Extensions.xml
│   │   │   │   │   ├── 📁 netstandard2.0
│   │   │   │   │   │   └── ⚙️ System.Threading.Tasks.Extensions.xml
│   │   │   │   │   ├── 📁 portable-net45+win8+wp8+wpa81
│   │   │   │   │   │   └── ⚙️ System.Threading.Tasks.Extensions.xml
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── 📁 ref
│   │   │   │   │   ├── 📁 MonoAndroid10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 MonoTouch10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 netcoreapp2.1
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinios10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarinmac20
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   ├── 📁 xamarintvos10
│   │   │   │   │   │   └── 📄 _._
│   │   │   │   │   └── 📁 xamarinwatchos10
│   │   │   │   │       └── 📄 _._
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📄 LICENSE.TXT
│   │   │   │   ├── 📄 THIRD-PARTY-NOTICES.TXT
│   │   │   │   ├── 📄 useSharedDesignerContext.txt
│   │   │   │   └── 📄 version.txt
│   │   │   └── 📁 ZstdSharp.Port.0.7.3
│   │   │       ├── 📁 lib
│   │   │       │   ├── 📁 net461
│   │   │       │   ├── 📁 net5.0
│   │   │       │   ├── 📁 net6.0
│   │   │       │   ├── 📁 net7.0
│   │   │       │   ├── 📁 netcoreapp3.1
│   │   │       │   ├── 📁 netstandard2.0
│   │   │       │   └── 📁 netstandard2.1
│   │   │       └── ⚙️ .signature.p7s
│   │   ├── 📄 Default.aspx
│   │   ├── 📄 Default.aspx.cs
│   │   ├── 📄 IAutenticacionService.cs
│   │   ├── 📄 Service1.svc
│   │   ├── 📄 Service1.svc.cs
│   │   ├── 📄 WS_Autenticacion.csproj
│   │   ├── 📄 WS_Autenticacion.sln
│   │   ├── ⚙️ Web.Debug.config
│   │   ├── ⚙️ Web.Release.config
│   │   ├── ⚙️ Web.config
│   │   └── ⚙️ packages.config
│   ├── 📁 WebAdministrativa
│   │   ├── 📁 .dotnet-cli
│   │   │   └── 📁 .dotnet
│   │   │       ├── ⚙️ .workloadAdvertisingManifestSentinel10.0.300
│   │   │       ├── 📄 10.0.303.aspNetCertificateSentinel
│   │   │       ├── 📄 10.0.303.dotnetFirstUseSentinel
│   │   │       └── 📄 10.0.303.toolpath.sentinel
│   │   ├── 📁 App_Start
│   │   │   ├── 📄 BundleConfig.cs
│   │   │   ├── 📄 FilterConfig.cs
│   │   │   └── 📄 RouteConfig.cs
│   │   ├── 📁 Content
│   │   │   ├── 🎨 Site.css
│   │   │   ├── 🎨 bootstrap-grid.css
│   │   │   ├── 🎨 bootstrap-grid.rtl.css
│   │   │   ├── 🎨 bootstrap-reboot.css
│   │   │   ├── 🎨 bootstrap-reboot.rtl.css
│   │   │   ├── 🎨 bootstrap-utilities.css
│   │   │   ├── 🎨 bootstrap-utilities.rtl.css
│   │   │   ├── 🎨 bootstrap.css
│   │   │   └── 🎨 bootstrap.rtl.css
│   │   ├── 📁 Controllers
│   │   │   ├── 📄 AccountController.cs
│   │   │   ├── 📄 HomeController.cs
│   │   │   └── 📄 LineasController.cs
│   │   ├── 📁 Models
│   │   │   ├── 📄 ActivarLineaViewModel.cs
│   │   │   ├── 📄 Linea.cs
│   │   │   ├── 📄 NuevaLineaViewModel.cs
│   │   │   └── 📄 Usuario.cs
│   │   ├── 📁 Properties
│   │   │   └── 📄 AssemblyInfo.cs
│   │   ├── 📁 Scripts
│   │   │   ├── 📄 bootstrap.esm.js
│   │   │   ├── 📄 bootstrap.js
│   │   │   ├── 📄 jquery-3.7.0.intellisense.js
│   │   │   ├── 📄 jquery-3.7.0.js
│   │   │   ├── 📄 jquery-3.7.0.slim.js
│   │   │   ├── 📄 jquery.validate-vsdoc.js
│   │   │   ├── 📄 jquery.validate.js
│   │   │   ├── 📄 jquery.validate.unobtrusive.js
│   │   │   └── 📄 modernizr-2.8.3.js
│   │   ├── 📁 Services
│   │   │   ├── 📄 AutenticacionClient.cs
│   │   │   ├── 📄 AutenticacionClient.cs.cs
│   │   │   ├── 📄 ProveedorEstadoClient.cs
│   │   │   └── 📄 ProveedorRegistroClient.cs
│   │   ├── 📁 Views
│   │   │   ├── 📁 Account
│   │   │   │   └── 📄 Login.cshtml
│   │   │   ├── 📁 Home
│   │   │   │   ├── 📄 About.cshtml
│   │   │   │   ├── 📄 Contact.cshtml
│   │   │   │   └── 📄 Index.cshtml
│   │   │   ├── 📁 Lineas
│   │   │   │   ├── 📄 Activar.cshtml
│   │   │   │   ├── 📄 ConfirmarActivacion.cshtml
│   │   │   │   ├── 📄 Crear.cshtml
│   │   │   │   ├── 📄 Desactivar.cshtml
│   │   │   │   └── 📄 Nuevas.cshtml
│   │   │   ├── 📁 Shared
│   │   │   │   ├── 📄 Error.cshtml
│   │   │   │   └── 📄 _Layout.cshtml
│   │   │   ├── ⚙️ Web.config
│   │   │   └── 📄 _ViewStart.cshtml
│   │   ├── 📁 packages
│   │   │   ├── 📁 Antlr.3.5.0.2
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   ├── 📁 Microsoft.AspNet.Mvc.5.2.9
│   │   │   │   ├── 📁 Content
│   │   │   │   │   ├── 📄 Web.config.install.xdt
│   │   │   │   │   └── 📄 Web.config.uninstall.xdt
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 net45
│   │   │   │   │       ├── 📁 es
│   │   │   │   │       │   └── ⚙️ System.Web.Mvc.xml
│   │   │   │   │       └── ⚙️ System.Web.Mvc.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ NET.icon.png
│   │   │   │   └── 📄 NET_Library_EULA_ENU.txt
│   │   │   ├── 📁 Microsoft.AspNet.Mvc.es.5.2.9
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 net45
│   │   │   │   │       └── 📁 es
│   │   │   │   │           └── ⚙️ System.Web.Mvc.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ NET.icon.png
│   │   │   │   └── 📄 NET_Library_EULA_ESN.txt
│   │   │   ├── 📁 Microsoft.AspNet.Razor.3.2.9
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 net45
│   │   │   │   │       ├── 📁 es
│   │   │   │   │       │   └── ⚙️ system.web.razor.xml
│   │   │   │   │       └── ⚙️ System.Web.Razor.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ NET.icon.png
│   │   │   │   └── 📄 NET_Library_EULA_ENU.txt
│   │   │   ├── 📁 Microsoft.AspNet.Razor.es.3.2.9
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 net45
│   │   │   │   │       └── 📁 es
│   │   │   │   │           └── ⚙️ system.web.razor.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ NET.icon.png
│   │   │   │   └── 📄 NET_Library_EULA_ESN.txt
│   │   │   ├── 📁 Microsoft.AspNet.Web.Optimization.1.1.3
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 net40
│   │   │   │   │       └── ⚙️ system.web.optimization.xml
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   ├── 📁 Microsoft.AspNet.Web.Optimization.es.1.1.3
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   ├── 📁 Microsoft.AspNet.WebPages.3.2.9
│   │   │   │   ├── 📁 Content
│   │   │   │   │   ├── 📄 Web.config.install.xdt
│   │   │   │   │   └── 📄 Web.config.uninstall.xdt
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 net45
│   │   │   │   │       ├── 📁 es
│   │   │   │   │       │   ├── ⚙️ system.web.helpers.xml
│   │   │   │   │       │   ├── ⚙️ system.web.webpages.deployment.xml
│   │   │   │   │       │   ├── ⚙️ system.web.webpages.razor.xml
│   │   │   │   │       │   └── ⚙️ system.web.webpages.xml
│   │   │   │   │       ├── ⚙️ System.Web.Helpers.xml
│   │   │   │   │       ├── ⚙️ System.Web.WebPages.Deployment.xml
│   │   │   │   │       ├── ⚙️ System.Web.WebPages.Razor.xml
│   │   │   │   │       └── ⚙️ System.Web.WebPages.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ NET.icon.png
│   │   │   │   └── 📄 NET_Library_EULA_ENU.txt
│   │   │   ├── 📁 Microsoft.AspNet.WebPages.es.3.2.9
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 net45
│   │   │   │   │       └── 📁 es
│   │   │   │   │           ├── ⚙️ system.web.helpers.xml
│   │   │   │   │           ├── ⚙️ system.web.webpages.deployment.xml
│   │   │   │   │           ├── ⚙️ system.web.webpages.razor.xml
│   │   │   │   │           └── ⚙️ system.web.webpages.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ NET.icon.png
│   │   │   │   └── 📄 NET_Library_EULA_ESN.txt
│   │   │   ├── 📁 Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1
│   │   │   │   ├── 📁 content
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   ├── 📄 app.config.install.xdt
│   │   │   │   │   │   ├── 📄 app.config.uninstall.xdt
│   │   │   │   │   │   ├── 📄 web.config.install.xdt
│   │   │   │   │   │   └── 📄 web.config.uninstall.xdt
│   │   │   │   │   └── 📁 net46
│   │   │   │   │       ├── 📄 app.config.install.xdt
│   │   │   │   │       ├── 📄 app.config.uninstall.xdt
│   │   │   │   │       ├── 📄 web.config.install.xdt
│   │   │   │   │       └── 📄 web.config.uninstall.xdt
│   │   │   │   ├── 📁 lib
│   │   │   │   │   └── 📁 net45
│   │   │   │   │       └── ⚙️ Microsoft.CodeDom.Providers.DotNetCompilerPlatform.xml
│   │   │   │   ├── 📁 tools
│   │   │   │   │   ├── 📁 Roslyn45
│   │   │   │   │   │   ├── 📄 Microsoft.CSharp.Core.targets
│   │   │   │   │   │   ├── 📄 Microsoft.VisualBasic.Core.targets
│   │   │   │   │   │   ├── ⚙️ VBCSCompiler.exe.config
│   │   │   │   │   │   ├── ⚙️ csc.exe.config
│   │   │   │   │   │   ├── 📄 csc.rsp
│   │   │   │   │   │   ├── 📄 csi.rsp
│   │   │   │   │   │   ├── ⚙️ vbc.exe.config
│   │   │   │   │   │   └── 📄 vbc.rsp
│   │   │   │   │   ├── 📁 RoslynLatest
│   │   │   │   │   │   ├── 📄 Microsoft.CSharp.Core.targets
│   │   │   │   │   │   ├── 📄 Microsoft.Managed.Core.targets
│   │   │   │   │   │   ├── 📄 Microsoft.VisualBasic.Core.targets
│   │   │   │   │   │   ├── ⚙️ VBCSCompiler.exe.config
│   │   │   │   │   │   ├── ⚙️ csc.exe.config
│   │   │   │   │   │   ├── 📄 csc.rsp
│   │   │   │   │   │   ├── ⚙️ csi.exe.config
│   │   │   │   │   │   ├── 📄 csi.rsp
│   │   │   │   │   │   ├── ⚙️ vbc.exe.config
│   │   │   │   │   │   └── 📄 vbc.rsp
│   │   │   │   │   └── 📁 net45
│   │   │   │   │       ├── 📄 install.ps1
│   │   │   │   │       └── 📄 uninstall.ps1
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   ├── 📁 Microsoft.Web.Infrastructure.2.0.0
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 🖼️ NET.icon.png
│   │   │   │   └── 📄 NET_Library_EULA_ENU.txt
│   │   │   ├── 📁 Microsoft.jQuery.Unobtrusive.Validation.3.2.11
│   │   │   │   ├── 📁 Content
│   │   │   │   │   └── 📁 Scripts
│   │   │   │   │       └── 📄 jquery.validate.unobtrusive.js
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   ├── 📁 Modernizr.2.8.3
│   │   │   │   ├── 📁 Content
│   │   │   │   │   └── 📁 Scripts
│   │   │   │   │       └── 📄 modernizr-2.8.3.js
│   │   │   │   ├── 📁 Tools
│   │   │   │   │   ├── 📄 common.ps1
│   │   │   │   │   ├── 📄 install.ps1
│   │   │   │   │   └── 📄 uninstall.ps1
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   ├── 📁 Newtonsoft.Json.13.0.3
│   │   │   │   ├── 📁 lib
│   │   │   │   │   ├── 📁 net20
│   │   │   │   │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │   │   ├── 📁 net35
│   │   │   │   │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │   │   ├── 📁 net40
│   │   │   │   │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │   │   ├── 📁 net45
│   │   │   │   │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │   │   ├── 📁 net6.0
│   │   │   │   │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │   │   ├── 📁 netstandard1.0
│   │   │   │   │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │   │   ├── 📁 netstandard1.3
│   │   │   │   │   │   └── ⚙️ Newtonsoft.Json.xml
│   │   │   │   │   └── 📁 netstandard2.0
│   │   │   │   │       └── ⚙️ Newtonsoft.Json.xml
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   ├── 📝 LICENSE.md
│   │   │   │   ├── 📝 README.md
│   │   │   │   └── 🖼️ packageIcon.png
│   │   │   ├── 📁 WebGrease.1.6.0
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   ├── 📁 bootstrap.5.2.3
│   │   │   │   ├── 📁 content
│   │   │   │   │   ├── 📁 Content
│   │   │   │   │   │   ├── 🎨 bootstrap-grid.css
│   │   │   │   │   │   ├── 🎨 bootstrap-grid.rtl.css
│   │   │   │   │   │   ├── 🎨 bootstrap-reboot.css
│   │   │   │   │   │   ├── 🎨 bootstrap-reboot.rtl.css
│   │   │   │   │   │   ├── 🎨 bootstrap-utilities.css
│   │   │   │   │   │   ├── 🎨 bootstrap-utilities.rtl.css
│   │   │   │   │   │   ├── 🎨 bootstrap.css
│   │   │   │   │   │   └── 🎨 bootstrap.rtl.css
│   │   │   │   │   └── 📁 Scripts
│   │   │   │   │       ├── 📄 bootstrap.esm.js
│   │   │   │   │       └── 📄 bootstrap.js
│   │   │   │   ├── 📁 contentFiles
│   │   │   │   │   └── 📁 any
│   │   │   │   │       └── 📁 any
│   │   │   │   │           └── 📁 wwwroot
│   │   │   │   │               ├── 📁 css
│   │   │   │   │               │   ├── 🎨 bootstrap-grid.css
│   │   │   │   │               │   ├── 🎨 bootstrap-grid.rtl.css
│   │   │   │   │               │   ├── 🎨 bootstrap-reboot.css
│   │   │   │   │               │   ├── 🎨 bootstrap-reboot.rtl.css
│   │   │   │   │               │   ├── 🎨 bootstrap-utilities.css
│   │   │   │   │               │   ├── 🎨 bootstrap-utilities.rtl.css
│   │   │   │   │               │   ├── 🎨 bootstrap.css
│   │   │   │   │               │   └── 🎨 bootstrap.rtl.css
│   │   │   │   │               └── 📁 js
│   │   │   │   │                   ├── 📄 bootstrap.esm.js
│   │   │   │   │                   └── 📄 bootstrap.js
│   │   │   │   ├── ⚙️ .signature.p7s
│   │   │   │   └── 🖼️ bootstrap.png
│   │   │   ├── 📁 jQuery.3.7.0
│   │   │   │   ├── 📁 Content
│   │   │   │   │   └── 📁 Scripts
│   │   │   │   │       ├── 📄 jquery-3.7.0-vsdoc.js
│   │   │   │   │       ├── 📄 jquery-3.7.0.js
│   │   │   │   │       └── 📄 jquery-3.7.0.slim.js
│   │   │   │   ├── 📁 Tools
│   │   │   │   │   ├── 📄 common.ps1
│   │   │   │   │   ├── 📄 install.ps1
│   │   │   │   │   ├── 📄 jquery-3.7.0.intellisense.js
│   │   │   │   │   └── 📄 uninstall.ps1
│   │   │   │   └── ⚙️ .signature.p7s
│   │   │   └── 📁 jQuery.Validation.1.19.5
│   │   │       ├── 📁 Content
│   │   │       │   └── 📁 Scripts
│   │   │       │       ├── 📄 jquery.validate-vsdoc.js
│   │   │       │       └── 📄 jquery.validate.js
│   │   │       └── ⚙️ .signature.p7s
│   │   ├── 📄 Global.asax
│   │   ├── 📄 Global.asax.cs
│   │   ├── ⚙️ Web.Debug.config
│   │   ├── ⚙️ Web.Release.config
│   │   ├── ⚙️ Web.config
│   │   ├── 📄 WebAdministrativa.csproj
│   │   ├── 📄 WebAdministrativa.sln
│   │   ├── 📄 favicon.ico
│   │   └── ⚙️ packages.config
│   └── 📝 estructura_guia.md
├── 📁 java_proveedor
│   ├── 📁 lib
│   ├── 📁 src
│   │   ├── 📁 config
│   │   │   ├── ☕ Config_Loader.java
│   │   │   └── 📄 config.properties
│   │   ├── 📁 database
│   │   │   ├── ☕ ClienteDAO.java
│   │   │   ├── ☕ ConexionSQL.java
│   │   │   ├── ☕ FacturacionDAO.java
│   │   │   ├── ☕ LlamadaProveedorDAO.java
│   │   │   ├── ☕ ServicioDAO.java
│   │   │   └── ☕ TarifaDAO.java
│   │   ├── 📁 models
│   │   │   ├── ☕ Cliente.java
│   │   │   ├── ☕ LlamadaProveedor.java
│   │   │   ├── ☕ Servicio.java
│   │   │   └── ☕ Tarifa.java
│   │   ├── 📁 services
│   │   │   ├── ☕ AdministracionTelefonica.java
│   │   │   ├── ☕ BitacoraService.java
│   │   │   ├── ☕ CalculoTarifa.java
│   │   │   ├── ☕ ConsultaSaldo.java
│   │   │   ├── ☕ CryptoService.java
│   │   │   ├── ☕ Identificador6Client.java
│   │   │   ├── ☕ Proveedor5Service.java
│   │   │   ├── ☕ Proveedor6Service.java
│   │   │   ├── ☕ RegistrarMovimiento.java
│   │   │   └── ☕ VerificarSaldo.java
│   │   └── 📁 sockets
│   │       ├── ☕ ManejoCliente.java
│   │       └── ☕ SocketTCP.java
│   ├── ☕ Main.java
│   └── 📝 README.md
├── 📁 python_identificador
│   ├── 📁 app
│   │   ├── 📁 config
│   │   │   ├── ⚙️ .gitkeep
│   │   │   └── 🐍 config.py
│   │   ├── 📁 database
│   │   │   ├── ⚙️ .gitkeep
│   │   │   ├── 🐍 conection.py
│   │   │   └── 🐍 repositorio.py
│   │   ├── 📁 models
│   │   │   └── ⚙️ .gitkeep
│   │   ├── 📁 services
│   │   │   ├── ⚙️ .gitkeep
│   │   │   ├── 🐍 administracion_telefonica.py
│   │   │   ├── 🐍 autorizacion_llamada.py
│   │   │   ├── 🐍 consulta.py
│   │   │   ├── 🐍 identificador6.py
│   │   │   ├── 🐍 iniciar_llamada.py
│   │   │   ├── 🐍 proveedor4_handler.py
│   │   │   ├── 🐍 proveedor_cliente.py
│   │   │   └── 🐍 termina_llamada.py
│   │   ├── 📁 sockets
│   │   │   ├── ⚙️ .gitkeep
│   │   │   ├── 🐍 handler.py
│   │   │   └── 🐍 servidor.py
│   │   └── 📁 utils
│   │       ├── ⚙️ .gitkeep
│   │       └── 🐍 crypto.py
│   ├── 📝 README.md
│   ├── 📄 bitacora_identificador.txt
│   ├── 🐍 main.py
│   └── 📄 requirements.txt
├── 📁 resources
│   ├── 📁 icono
│   │   └── 🖼️ Logo.png
│   └── ⚙️ .gitkeep
├── 📁 scripts
│   ├── ⚙️ .gitkeep
│   ├── 📄 persona2-levantar.ps1
│   ├── 📄 persona2-preparar.ps1
│   └── 📄 sincronizar-inventario-mysql-a-sqlserver.ps1
├── 📁 shared
│   ├── 📁 config
│   │   ├── ⚙️ database.json
│   │   └── ⚙️ puertos.json
│   ├── 📁 contracts
│   │   ├── ⚙️ activar_desactivar_linea_proveedor5.json
│   │   ├── ⚙️ cambiar_estado_telefono.json
│   │   ├── ⚙️ consulta_catalogo_telefonos.json
│   │   ├── ⚙️ consulta_proveedor.json
│   │   ├── ⚙️ consulta_saldo.json
│   │   ├── ⚙️ finalizar_llamada.json
│   │   ├── ⚙️ identificador6.json
│   │   ├── ⚙️ inicio_llamada.json
│   │   ├── ⚙️ proveedor6_facturacion.json
│   │   ├── ⚙️ recargar_saldo.json
│   │   ├── ⚙️ registrar_telefono.json
│   │   ├── ⚙️ registro_movimiento_proveedor.json
│   │   ├── ⚙️ respuesta_cambio_estado_telefono.json
│   │   ├── ⚙️ respuesta_catalogo_telefonos.json
│   │   ├── ⚙️ respuesta_identificador6.json
│   │   ├── ⚙️ respuesta_llamada.json
│   │   ├── ⚙️ respuesta_proveedor.json
│   │   ├── ⚙️ respuesta_proveedor5.json
│   │   ├── ⚙️ respuesta_recarga_saldo.json
│   │   ├── ⚙️ respuesta_registro_telefono.json
│   │   ├── ⚙️ respuesta_saldo.json
│   │   ├── ⚙️ respuestas_proveedor6.json
│   │   └── ⚙️ solicitud_llamada.json
│   ├── 📁 examples
│   │   ├── ⚙️ consulta_saldo_exitosa.json
│   │   ├── ⚙️ finalizar_llamada_exitosa.json
│   │   ├── ⚙️ llamada_exitosa.json
│   │   ├── ⚙️ saldo_insuficiente.json
│   │   ├── ⚙️ sim_invalida.json
│   │   ├── ⚙️ telefono_inactivo.json
│   │   └── ⚙️ ubicacion_invalida.json
│   └── 📁 schemas
│       └── ⚙️ .gitkeep
├── 📁 test
│   └── ⚙️ .gitkeep
├── 📁 tools
│   └── 🐍 generar_seed_aes.py
├── ⚙️ .gitignore
├── 📝 README.md
├── 📄 bitacora_identificador.txt
└── 📄 sources.txt
```

---
*Generated by FileTree Pro Extension*