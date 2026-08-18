using Microsoft.AspNetCore.Mvc;
using PortalCliente.Models;
using PortalCliente.Services;

namespace PortalCliente.Controllers;

public class ClienteController : Controller
{
    private const string SessionIdentificacion = "IdentificacionCliente";

    private readonly IProveedorClienteService _proveedorClienteService;
    private readonly IProveedorPortalService _proveedorPortalService;
    private readonly IProveedor2Service _proveedor2Service;
    private readonly IEmailService _emailService;

    public ClienteController(
        IProveedorClienteService proveedorClienteService,
        IProveedorPortalService proveedorPortalService,
        IProveedor2Service proveedor2Service,
        IEmailService emailService)
    {
        _proveedorClienteService = proveedorClienteService;
        _proveedorPortalService = proveedorPortalService;
        _proveedor2Service = proveedor2Service;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string? identificacion = Request.Query["identificacion"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(identificacion))
        {
            HttpContext.Session.SetString(
                SessionIdentificacion,
                identificacion.Trim());
        }

        string? identificacionSesion = HttpContext.Session.GetString(SessionIdentificacion);

        if (string.IsNullOrWhiteSpace(identificacionSesion))
        {
            return View(IndexViewModel.SinIdentificacion());
        }

        ConsultarLineasClienteResult resultado =
            await _proveedorClienteService.ConsultarLineasClienteAsync(identificacionSesion);

        var modelo = new IndexViewModel
        {
            Identificacion = identificacionSesion,
            Resultado = resultado.Resultado,
            Mensaje = resultado.Mensaje,
            Lineas = resultado.Lineas
        };

        return View(modelo);
    }

    [HttpPost]
    public IActionResult Index(string identificacion)
    {
        HttpContext.Session.SetString(
            SessionIdentificacion,
            identificacion?.Trim() ?? string.Empty);

        return RedirectToAction(nameof(Index));
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
            NumeroTelefono = linea.NumeroTelefono,
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
}
