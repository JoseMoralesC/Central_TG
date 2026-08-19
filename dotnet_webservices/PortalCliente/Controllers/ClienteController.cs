using Microsoft.AspNetCore.Mvc;
using PortalCliente.Models;
using PortalCliente.Services;

namespace PortalCliente.Controllers;

public class ClienteController : Controller
{
    private const string SessionIdentificacion = "IdentificacionCliente";
    private const string SessionNombreCliente = "NombreCliente";
    private const string LoginClienteUrl = "http://localhost:56122/Login.aspx";

    private readonly IProveedorClienteService _proveedorClienteService;
    private readonly IProveedorPortalService _proveedorPortalService;
    private readonly IProveedor2Service _proveedor2Service;
    private readonly IEmailService _emailService;
    private readonly IAutenticacionPortalService _autenticacionPortalService;
    private readonly ProveedorCryptoHelper _proveedorCryptoHelper;

    public ClienteController(
        IProveedorClienteService proveedorClienteService,
        IProveedorPortalService proveedorPortalService,
        IProveedor2Service proveedor2Service,
        IEmailService emailService,
        IAutenticacionPortalService autenticacionPortalService,
        ProveedorCryptoHelper proveedorCryptoHelper)
    {
        _proveedorClienteService = proveedorClienteService;
        _proveedorPortalService = proveedorPortalService;
        _proveedor2Service = proveedor2Service;
        _emailService = emailService;
        _autenticacionPortalService = autenticacionPortalService;
        _proveedorCryptoHelper = proveedorCryptoHelper;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string? identificacion = Request.Query["identificacion"].FirstOrDefault();
        string? nombre = Request.Query["nombre"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(identificacion))
        {
            HttpContext.Session.SetString(
                SessionIdentificacion,
                identificacion.Trim());
        }

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            HttpContext.Session.SetString(
                SessionNombreCliente,
                nombre.Trim());
        }

        string? identificacionSesion = HttpContext.Session.GetString(SessionIdentificacion);
        string? nombreSesion = HttpContext.Session.GetString(SessionNombreCliente);

        if (string.IsNullOrWhiteSpace(identificacionSesion))
        {
            return View(IndexViewModel.SinIdentificacion());
        }

        ConsultarLineasClienteResult resultado =
            await _proveedorClienteService.ConsultarLineasClienteAsync(identificacionSesion);

        var modelo = new IndexViewModel
        {
            Identificacion = identificacionSesion,
            NombreCliente = nombreSesion,
            Resultado = resultado.Resultado,
            Mensaje = resultado.Mensaje,
            Lineas = resultado.Lineas
        };

        return View(modelo);
    }

    [HttpPost]
    public IActionResult Index(string identificacion)
    {
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Redirect(LoginClienteUrl);
    }

    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return View(new PerfilClienteViewModel
            {
                Procesado = true,
                Exitoso = false,
                MensajeResultado = "Debe iniciar sesion para consultar sus datos."
            });
        }

        ClientePerfilResult resultado =
            await _autenticacionPortalService.ObtenerPerfilClienteAsync(identificacion);

        if (!resultado.Resultado || resultado.Perfil is null)
        {
            return View(new PerfilClienteViewModel
            {
                Identificacion = identificacion,
                Procesado = true,
                Exitoso = false,
                MensajeResultado = resultado.Mensaje
            });
        }

        return View(resultado.Perfil);
    }

    [HttpPost]
    public async Task<IActionResult> Perfil(PerfilClienteViewModel modelo)
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = "Debe iniciar sesion para actualizar sus datos.";
            return View(modelo);
        }

        modelo.Identificacion = identificacion;

        string? error = ValidarCambiosPerfil(modelo);
        if (error is not null)
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = error;
            return View(modelo);
        }

        OperacionPerfilResult resultado =
            await _autenticacionPortalService.ActualizarPerfilClienteAsync(modelo);

        modelo.Procesado = true;
        modelo.Exitoso = resultado.Resultado;
        modelo.MensajeResultado = resultado.Resultado
            ? "Datos actualizados correctamente."
            : resultado.Mensaje;

        if (resultado.Resultado)
        {
            CambioEstadoLineaResult sincronizacionCorreo =
                await _proveedor2Service.ActualizarCorreoClienteAsync(
                    identificacion,
                    modelo.CorreoElectronico);

            if (!sincronizacionCorreo.Resultado)
            {
                modelo.MensajeResultado += " Aviso: no se pudo sincronizar el correo para facturacion: "
                    + sincronizacionCorreo.Mensaje;
            }

            ClientePerfilResult perfilActualizado =
                await _autenticacionPortalService.ObtenerPerfilClienteAsync(identificacion);

            if (perfilActualizado.Perfil is not null)
            {
                modelo.Nombre = perfilActualizado.Perfil.Nombre;
                modelo.PrimerApellido = perfilActualizado.Perfil.PrimerApellido;
                modelo.SegundoApellido = perfilActualizado.Perfil.SegundoApellido;
                modelo.Estado = perfilActualizado.Perfil.Estado;
                modelo.UsuarioActual = perfilActualizado.Perfil.UsuarioActual;
                modelo.NuevoUsuario = string.Empty;
                modelo.NuevaContrasena = string.Empty;
            }
        }

        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> SolicitarLinea()
    {
        SolicitarLineaViewModel modelo = await CrearModeloSolicitudAsync();
        modelo.MensajeResultado = TempData["SolicitarLineaMensaje"] as string;
        modelo.Exitoso = TempData["SolicitarLineaExitoso"] as bool? ?? false;
        modelo.Procesado = modelo.MensajeResultado is not null;
        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> SolicitarLinea(
        int servicioId,
        string numeroTelefono,
        string tipoServicio)
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);
        string? nombre = HttpContext.Session.GetString(SessionNombreCliente);

        if (string.IsNullOrWhiteSpace(identificacion) ||
            string.IsNullOrWhiteSpace(nombre))
        {
            TempData["SolicitarLineaExitoso"] = false;
            TempData["SolicitarLineaMensaje"] = "Debe iniciar sesion para solicitar una linea.";
            return RedirectToAction(nameof(SolicitarLinea));
        }

        CambioEstadoLineaResult resultado =
            await _proveedor2Service.SolicitarLineaClienteAsync(
                servicioId,
                numeroTelefono,
                tipoServicio,
                identificacion,
                nombre);

        TempData["SolicitarLineaExitoso"] = resultado.Resultado;
        TempData["SolicitarLineaMensaje"] = resultado.Resultado
            ? "Solicitud registrada. Un administrador revisara la asignacion."
            : resultado.Mensaje;

        return RedirectToAction(nameof(SolicitarLinea));
    }

    private async Task<SolicitarLineaViewModel> CrearModeloSolicitudAsync()
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);
        string? nombre = HttpContext.Session.GetString(SessionNombreCliente);

        var modelo = new SolicitarLineaViewModel
        {
            Identificacion = identificacion,
            NombreCliente = nombre
        };

        if (string.IsNullOrWhiteSpace(identificacion))
        {
            modelo.ResultadoConsulta = false;
            modelo.MensajeConsulta = "Debe iniciar sesion para solicitar una linea.";
            return modelo;
        }

        ListarLineasDisponiblesResult resultado =
            await _proveedor2Service.ListarLineasDisponiblesAsync();

        modelo.ResultadoConsulta = resultado.Resultado;
        modelo.MensajeConsulta = resultado.Mensaje;
        modelo.Lineas = resultado.Lineas;
        return modelo;
    }

    [HttpGet]
    public async Task<IActionResult> CargarSaldo(string numero)
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);

        var modelo = new CargarSaldoViewModel
        {
            Identificacion = identificacion
        };

        if (!string.IsNullOrWhiteSpace(numero))
        {
            modelo.Numero = numero.Trim();
            await PrecargarMetodoPagoAsync(modelo, identificacion);
            return View(modelo);
        }

        modelo.ModoLista = true;

        if (string.IsNullOrWhiteSpace(identificacion))
        {
            modelo.ResultadoConsulta = false;
            modelo.MensajeConsulta = "Ingrese su identificacion para consultar sus lineas.";
            return View(modelo);
        }

        ConsultarLineasClienteResult resultado =
            await _proveedorClienteService.ConsultarLineasClienteAsync(identificacion);

        modelo.ResultadoConsulta = resultado.Resultado;
        modelo.MensajeConsulta = resultado.Mensaje;
        modelo.Lineas = resultado.Lineas
            .Where(l => string.Equals(
                l.TipoServicio,
                "PREPAGO",
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> CargarSaldo(CargarSaldoViewModel modelo)
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);

        if (string.IsNullOrWhiteSpace(identificacion))
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = "Ingrese su identificacion para continuar.";
            return View(modelo);
        }

        string? error = ValidarPagoTarjeta(modelo);
        if (error is not null)
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = error;
            return View(modelo);
        }

        ConsultarLineasClienteResult consulta =
            await _proveedorClienteService.ConsultarLineasClienteAsync(identificacion);

        bool esLineaDelCliente = consulta.Lineas.Any(l =>
            string.Equals(l.NumeroTelefono, modelo.Numero, StringComparison.Ordinal)
            && string.Equals(l.TipoServicio, "PREPAGO", StringComparison.OrdinalIgnoreCase));

        if (!esLineaDelCliente)
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = "La linea prepago seleccionada no pertenece al cliente.";
            return View(modelo);
        }

        RecargarSaldoResult resultado =
            await _proveedorClienteService.RecargarSaldoAsync(
                modelo.Numero ?? string.Empty,
                modelo.Monto ?? 0);

        modelo.Procesado = true;
        modelo.Exitoso = resultado.Resultado;
        modelo.MensajeResultado = resultado.Resultado
            ? "Registro exitoso"
            : resultado.Mensaje;
        modelo.NuevoSaldo = resultado.NuevoSaldo;

        return View(modelo);
    }

    private static string? ValidarPagoTarjeta(CargarSaldoViewModel modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.Numero))
        {
            return "Debe seleccionar la linea prepago a recargar.";
        }

        if (modelo.Monto is null || modelo.Monto <= 0)
        {
            return "Debe ingresar un monto valido, positivo y sin decimales.";
        }

        return ValidarDatosTarjeta(
            modelo.NumeroTarjeta,
            modelo.NombreTarjeta,
            modelo.FechaVencimiento,
            modelo.CodigoSeguridad);
    }

    private static string? ValidarDatosTarjeta(
        string? numeroTarjeta,
        string? nombreTarjeta,
        string? fechaVencimiento,
        string? codigoSeguridad)
    {
        string tarjeta = (numeroTarjeta ?? string.Empty)
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty);

        if (!System.Text.RegularExpressions.Regex.IsMatch(tarjeta, @"^\d{12}$"))
        {
            return "El numero de tarjeta debe tener doce digitos en grupos de 4.";
        }

        if (string.IsNullOrWhiteSpace(nombreTarjeta))
        {
            return "Debe ingresar el nombre del dueno de la tarjeta.";
        }

        string? errorVencimiento = ValidarVencimiento(fechaVencimiento);
        if (errorVencimiento is not null)
        {
            return errorVencimiento;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(
            codigoSeguridad ?? string.Empty,
            @"^\d{3}$"))
        {
            return "El codigo de seguridad debe tener tres digitos.";
        }

        return null;
    }

    private static string? ValidarVencimiento(string? fecha)
    {
        if (string.IsNullOrWhiteSpace(fecha))
        {
            return "Debe ingresar la fecha de vencimiento de la tarjeta.";
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(fecha.Trim(), @"^(0[1-9]|1[0-2])/\d{2}$"))
        {
            return "La fecha de vencimiento debe tener formato MM/AA.";
        }

        string[] partes = fecha.Trim().Split('/');
        int mes = int.Parse(partes[0]);
        int anio = 2000 + int.Parse(partes[1]);

        DateTime ahora = DateTime.Now;
        if (anio < ahora.Year
            || (anio == ahora.Year && mes < ahora.Month))
        {
            return "La fecha de vencimiento debe ser igual o superior al mes en curso.";
        }

        return null;
    }

    private async Task PrecargarMetodoPagoAsync(
        CargarSaldoViewModel modelo,
        string? identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return;
        }

        MetodoPagoCliente metodo =
            await _autenticacionPortalService.ObtenerMetodoPagoClienteAsync(identificacion);

        if (!metodo.Resultado)
        {
            return;
        }

        modelo.NumeroTarjeta = metodo.NumeroTarjeta;
        modelo.NombreTarjeta = metodo.NombreTarjeta;
        modelo.FechaVencimiento = metodo.FechaVencimiento;
        modelo.CodigoSeguridad = metodo.CodigoSeguridad;
    }

    private async Task PrecargarMetodoPagoAsync(
        PagarFacturaViewModel modelo,
        string? identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return;
        }

        MetodoPagoCliente metodo =
            await _autenticacionPortalService.ObtenerMetodoPagoClienteAsync(identificacion);

        if (!metodo.Resultado)
        {
            return;
        }

        modelo.NumeroTarjeta = metodo.NumeroTarjeta;
        modelo.NombreTarjeta = metodo.NombreTarjeta;
        modelo.FechaVencimiento = metodo.FechaVencimiento;
        modelo.CodigoSeguridad = metodo.CodigoSeguridad;
    }

    [HttpGet]
    public async Task<IActionResult> PagarFactura(string numero)
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);

        var modelo = new PagarFacturaViewModel
        {
            Identificacion = identificacion
        };

        if (string.IsNullOrWhiteSpace(identificacion))
        {
            modelo.ResultadoConsulta = false;
            modelo.MensajeConsulta = "Ingrese su identificacion para consultar sus lineas.";
            return View(modelo);
        }

        ConsultarLineasClienteResult resultado =
            await _proveedorClienteService.ConsultarLineasClienteAsync(identificacion);

        if (string.IsNullOrWhiteSpace(numero))
        {
            modelo.ModoLista = true;
            modelo.ResultadoConsulta = resultado.Resultado;
            modelo.MensajeConsulta = resultado.Mensaje;
            modelo.Lineas = resultado.Lineas
                .Where(l => string.Equals(
                    l.TipoServicio,
                    "POSTPAGO",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
            return View(modelo);
        }

        modelo.Numero = numero.Trim();
        LineaCliente? linea = resultado.Lineas.FirstOrDefault(l =>
            string.Equals(l.NumeroTelefono, modelo.Numero, StringComparison.Ordinal));

        if (linea is null || linea.FacturaPendiente <= 0)
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = "La linea postpago seleccionada no posee factura pendiente.";
            return View(modelo);
        }

        modelo.MontoFactura = linea.FacturaPendiente;
        await PrecargarMetodoPagoAsync(modelo, identificacion);
        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> PagarFactura(PagarFacturaViewModel modelo)
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);

        if (string.IsNullOrWhiteSpace(identificacion))
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = "Ingrese su identificacion para continuar.";
            return View(modelo);
        }

        ConsultarLineasClienteResult consulta =
            await _proveedorClienteService.ConsultarLineasClienteAsync(identificacion);

        LineaCliente? linea = consulta.Lineas.FirstOrDefault(l =>
            string.Equals(l.NumeroTelefono, modelo.Numero, StringComparison.Ordinal)
            && string.Equals(l.TipoServicio, "POSTPAGO", StringComparison.OrdinalIgnoreCase));

        if (linea is null || linea.FacturaPendiente <= 0)
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = "La linea postpago seleccionada no posee factura pendiente.";
            return View(modelo);
        }

        decimal monto = linea.FacturaPendiente;
        modelo.MontoFactura = monto;

        string? error = ValidarDatosTarjeta(
            modelo.NumeroTarjeta,
            modelo.NombreTarjeta,
            modelo.FechaVencimiento,
            modelo.CodigoSeguridad);
        if (error is not null)
        {
            modelo.Procesado = true;
            modelo.Exitoso = false;
            modelo.MensajeResultado = error;
            return View(modelo);
        }

        PagarFacturaResult resultado =
            await _proveedorClienteService.PagarFacturaAsync(
                modelo.Numero ?? string.Empty,
                monto,
                identificacion);

        modelo.Procesado = true;
        modelo.Exitoso = resultado.Resultado;
        modelo.MensajeResultado = resultado.Resultado
            ? "Registro exitoso"
            : resultado.Mensaje;

        if (resultado.Resultado && !string.IsNullOrWhiteSpace(resultado.CorreoCliente))
        {
            EmailEnvioResultado envio = await _emailService.EnviarFacturacionAsync(
                resultado.CorreoCliente,
                modelo.Numero ?? string.Empty,
                resultado);

            modelo.DetalleCorreo = envio.Mensaje
                + (envio.Enviado ? string.Empty : " (detalle: " + monto.ToString("0.00") + ").");
        }
        else if (resultado.Resultado)
        {
            modelo.DetalleCorreo =
                "Pago registrado. No se pudo enviar el correo: el cliente no tiene correo registrado.";
        }

        return View(modelo);
    }

    [HttpGet]
    public async Task<IActionResult> DevolverLinea()
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);

        var modelo = new DevolverLineaViewModel
        {
            Identificacion = identificacion
        };

        if (string.IsNullOrWhiteSpace(identificacion))
        {
            modelo.ResultadoConsulta = false;
            modelo.MensajeConsulta = "Ingrese su identificacion para consultar sus lineas.";
            return View(modelo);
        }

        ConsultarLineasClienteResult resultado =
            await _proveedorClienteService.ConsultarLineasClienteAsync(identificacion);

        modelo.ResultadoConsulta = resultado.Resultado;
        modelo.MensajeConsulta = resultado.Mensaje;

        if (resultado.Resultado)
        {
            foreach (LineaCliente linea in resultado.Lineas)
            {
                bool postpago = string.Equals(
                    linea.TipoServicio,
                    "POSTPAGO",
                    StringComparison.OrdinalIgnoreCase);

                var item = new LineaDevolucion
                {
                    NumeroTelefono = linea.NumeroTelefono,
                    TipoServicio = linea.TipoServicio,
                    FacturaPendiente = linea.FacturaPendiente
                };

                if (postpago)
                {
                    modelo.LineasPostpago.Add(item);
                    continue;
                }

                ConsultarSaldoPortalResult saldo =
                    await _proveedorPortalService.ConsultarSaldoAsync(
                        linea.NumeroTelefono);

                item.Saldo = saldo.Resultado ? saldo.Saldo : 0;
                item.SaldoDetalle = saldo.Resultado ? null : saldo.Mensaje;
                modelo.LineasPrepago.Add(item);
            }
        }

        modelo.MensajeResultado = TempData["DevolverMensaje"] as string;
        modelo.Exitoso = TempData["DevolverExitoso"] as bool? ?? false;
        modelo.Procesado = modelo.MensajeResultado is not null;

        return View(modelo);
    }

    [HttpPost]
    public async Task<IActionResult> DevolverLinea(string numero)
    {
        string? identificacion = HttpContext.Session.GetString(SessionIdentificacion);

        if (string.IsNullOrWhiteSpace(identificacion))
        {
            TempData["DevolverExitoso"] = false;
            TempData["DevolverMensaje"] = "Ingrese su identificacion para continuar.";
            return RedirectToAction(nameof(DevolverLinea));
        }

        if (string.IsNullOrWhiteSpace(numero))
        {
            TempData["DevolverExitoso"] = false;
            TempData["DevolverMensaje"] = "Debe seleccionar la linea a devolver.";
            return RedirectToAction(nameof(DevolverLinea));
        }

        ConsultarLineasClienteResult consulta =
            await _proveedorClienteService.ConsultarLineasClienteAsync(identificacion);

        LineaCliente? linea = consulta.Lineas.FirstOrDefault(l =>
            string.Equals(l.NumeroTelefono, numero.Trim(), StringComparison.Ordinal));

        if (linea is null)
        {
            TempData["DevolverExitoso"] = false;
            TempData["DevolverMensaje"] = "La linea seleccionada no pertenece al cliente.";
            return RedirectToAction(nameof(DevolverLinea));
        }

        bool postpago = string.Equals(
            linea.TipoServicio,
            "POSTPAGO",
            StringComparison.OrdinalIgnoreCase);

        if (postpago && linea.FacturaPendiente > 0)
        {
            TempData["DevolverExitoso"] = false;
            TempData["DevolverMensaje"] =
                "La linea postpago posee facturacion pendiente. Primero debe cancelar la factura para poder devolver la linea.";
            return RedirectToAction(nameof(DevolverLinea));
        }

        var solicitud = new ActivarDesactivarLineaPortalRequest
        {
            NumeroTelefono = _proveedorCryptoHelper.Encrypt(linea.NumeroTelefono),
            IdentificadorTelefono = linea.IdentificadorTelefono,
            IdentificadorTarjeta = linea.IdentificadorTarjeta,
            Tipo = linea.TipoServicio,
            IdentificacionCliente = linea.IdentificacionDuenoCifrada,
            Estado = "disponible"
        };

        CambioEstadoLineaResult resultado =
            await _proveedor2Service.ActivarDesactivarLineaAsync(solicitud);

        TempData["DevolverExitoso"] = resultado.Resultado;
        TempData["DevolverMensaje"] = resultado.Resultado
            ? "La linea telefonica fue devuelta correctamente."
            : (string.IsNullOrWhiteSpace(resultado.Mensaje)
                ? "No fue posible devolver la linea."
                : resultado.Mensaje);

        return RedirectToAction(nameof(DevolverLinea));
    }

    private static string? ValidarCambiosPerfil(PerfilClienteViewModel modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.CorreoElectronico) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                modelo.CorreoElectronico.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return "Ingrese un correo electronico valido.";
        }

        if (!string.IsNullOrWhiteSpace(modelo.NuevoUsuario) &&
            modelo.NuevoUsuario.Trim().Length < 4)
        {
            return "El usuario debe tener al menos 4 caracteres.";
        }

        if (!string.IsNullOrWhiteSpace(modelo.NuevaContrasena) &&
            !System.Text.RegularExpressions.Regex.IsMatch(
                modelo.NuevaContrasena,
                @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{7,}$"))
        {
            return "La contrasena debe tener minimo 7 caracteres, mayuscula, minuscula, numero y especial.";
        }

        return null;
    }
}
