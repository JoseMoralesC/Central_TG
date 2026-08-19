using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using PortalCliente.Models;

namespace PortalCliente.Services;

public class Proveedor2SoapClient : IProveedor2Service
{
    private const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
    private const string ServiceNamespace = "http://tempuri.org/";
    private const string ActivarDesactivarAction =
        "http://tempuri.org/IProveedorService/ActivarDesactivarLinea";
    private const string ListarLineasDisponiblesAction =
        "http://tempuri.org/IProveedorService/ListarLineasDisponibles";
    private const string SolicitarLineaClienteAction =
        "http://tempuri.org/IProveedorService/SolicitarLineaCliente";
    private const string ActualizarCorreoClienteAction =
        "http://tempuri.org/IProveedorService/ActualizarCorreoCliente";

    private readonly HttpClient _httpClient;
    private readonly string _url;

    public Proveedor2SoapClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("WS_PROVEEDOR2");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _url = configuration["Proveedor2:Url"]
            ?? "http://localhost:55254/ProveedorService.svc";
    }

    public async Task<ListarLineasDisponiblesResult> ListarLineasDisponiblesAsync()
    {
        string cuerpo = $"<ListarLineasDisponibles xmlns=\"{ServiceNamespace}\" />";
        string sobre = CrearSobre(cuerpo);

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + ListarLineasDisponiblesAction + "\"");
        request.Content = new StringContent(sobre, Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return ErrorListado("WS_PROVEEDOR2 respondio con error HTTP: "
                    + (int)respuesta.StatusCode);
            }

            return ParsearListadoLineas(xml);
        }
        catch (TaskCanceledException)
        {
            return ErrorListado("El servicio WS_PROVEEDOR2 no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return ErrorListado("No se pudo conectar con WS_PROVEEDOR2: " + ex.Message);
        }
    }

    public async Task<CambioEstadoLineaResult> SolicitarLineaClienteAsync(
        int servicioId,
        string numeroTelefono,
        string tipoServicio,
        string identificacionCliente,
        string nombreCliente)
    {
        if (servicioId <= 0 ||
            string.IsNullOrWhiteSpace(numeroTelefono) ||
            string.IsNullOrWhiteSpace(tipoServicio) ||
            string.IsNullOrWhiteSpace(identificacionCliente) ||
            string.IsNullOrWhiteSpace(nombreCliente))
        {
            return Error("Debe seleccionar una linea disponible.");
        }

        string cuerpo = $"<SolicitarLineaCliente xmlns=\"{ServiceNamespace}\">"
            + "<solicitud xmlns:a=\"http://schemas.datacontract.org/2004/07/WS_Proveedor.Models\">"
            + $"<a:ServicioId>{servicioId}</a:ServicioId>"
            + $"<a:NumeroTelefono>{EscapeXml(numeroTelefono.Trim())}</a:NumeroTelefono>"
            + $"<a:TipoServicio>{EscapeXml(tipoServicio.Trim())}</a:TipoServicio>"
            + $"<a:IdentificacionCliente>{EscapeXml(identificacionCliente.Trim())}</a:IdentificacionCliente>"
            + $"<a:NombreCliente>{EscapeXml(nombreCliente.Trim())}</a:NombreCliente>"
            + "</solicitud>"
            + "</SolicitarLineaCliente>";

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + SolicitarLineaClienteAction + "\"");
        request.Content = new StringContent(CrearSobre(cuerpo), Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return Error("WS_PROVEEDOR2 respondio con error HTTP: "
                    + (int)respuesta.StatusCode);
            }

            return ParsearRespuesta(xml, "SolicitarLineaClienteResult");
        }
        catch (TaskCanceledException)
        {
            return Error("El servicio WS_PROVEEDOR2 no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return Error("No se pudo conectar con WS_PROVEEDOR2: " + ex.Message);
        }
    }

    public async Task<CambioEstadoLineaResult> ActivarDesactivarLineaAsync(
        ActivarDesactivarLineaPortalRequest solicitud)
    {
        if (solicitud is null
            || string.IsNullOrWhiteSpace(solicitud.NumeroTelefono))
        {
            return Error("El numero de telefono es obligatorio.");
        }

        string cuerpo = $"<ActivarDesactivarLinea xmlns=\"{ServiceNamespace}\">"
            + "<solicitud xmlns:a=\"http://schemas.datacontract.org/2004/07/WS_Proveedor.Models\">"
            + $"<a:NumeroTelefono>{EscapeXml(solicitud.NumeroTelefono.Trim())}</a:NumeroTelefono>"
            + $"<a:IdentificadorTelefono>{EscapeXml(solicitud.IdentificadorTelefono)}</a:IdentificadorTelefono>"
            + $"<a:IdentificadorTarjeta>{EscapeXml(solicitud.IdentificadorTarjeta)}</a:IdentificadorTarjeta>"
            + $"<a:Tipo>{EscapeXml(solicitud.Tipo)}</a:Tipo>"
            + $"<a:IdentificacionCliente>{EscapeXml(solicitud.IdentificacionCliente)}</a:IdentificacionCliente>"
            + $"<a:Estado>{EscapeXml(solicitud.Estado)}</a:Estado>"
            + "</solicitud>"
            + "</ActivarDesactivarLinea>";

        string sobre = CrearSobre(cuerpo);

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + ActivarDesactivarAction + "\"");
        request.Content = new StringContent(sobre, Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return Error("WS_PROVEEDOR2 respondio con error HTTP: "
                    + (int)respuesta.StatusCode);
            }

            return ParsearRespuesta(xml, "ActivarDesactivarLineaResult");
        }
        catch (TaskCanceledException)
        {
            return Error("El servicio WS_PROVEEDOR2 no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return Error("No se pudo conectar con WS_PROVEEDOR2: " + ex.Message);
        }
    }

    public async Task<CambioEstadoLineaResult> ActualizarCorreoClienteAsync(
        string identificacionCliente,
        string correoCliente)
    {
        if (string.IsNullOrWhiteSpace(identificacionCliente) ||
            string.IsNullOrWhiteSpace(correoCliente))
        {
            return Error("Debe indicar identificacion y correo del cliente.");
        }

        string cuerpo = $"<ActualizarCorreoCliente xmlns=\"{ServiceNamespace}\">"
            + "<solicitud xmlns:a=\"http://schemas.datacontract.org/2004/07/WS_Proveedor.Models\">"
            + $"<a:IdentificacionCliente>{EscapeXml(identificacionCliente.Trim())}</a:IdentificacionCliente>"
            + $"<a:CorreoCliente>{EscapeXml(correoCliente.Trim())}</a:CorreoCliente>"
            + "</solicitud>"
            + "</ActualizarCorreoCliente>";

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + ActualizarCorreoClienteAction + "\"");
        request.Content = new StringContent(CrearSobre(cuerpo), Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return Error("WS_PROVEEDOR2 respondio con error HTTP: "
                    + (int)respuesta.StatusCode);
            }

            return ParsearRespuesta(xml, "ActualizarCorreoClienteResult");
        }
        catch (TaskCanceledException)
        {
            return Error("El servicio WS_PROVEEDOR2 no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return Error("No se pudo conectar con WS_PROVEEDOR2: " + ex.Message);
        }
    }

    private static CambioEstadoLineaResult ParsearRespuesta(
        string xml,
        string nombreResultado)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);

            XElement? resultadoElemento = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == nombreResultado);

            if (resultadoElemento is null)
            {
                return Error("Respuesta de WS_PROVEEDOR2 en formato inesperado.");
            }

            return new CambioEstadoLineaResult
            {
                Resultado = LeerBooleano(resultadoElemento, "Resultado"),
                Mensaje = LeerTexto(resultadoElemento, "Mensaje")
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return Error("No se pudo interpretar la respuesta de WS_PROVEEDOR2: " + ex.Message);
        }
    }

    private static CambioEstadoLineaResult Error(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje };

    private static ListarLineasDisponiblesResult ErrorListado(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje };

    private static ListarLineasDisponiblesResult ParsearListadoLineas(string xml)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);

            XElement? resultadoElemento = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "ListarLineasDisponiblesResult");

            if (resultadoElemento is null)
            {
                return ErrorListado("Respuesta de WS_PROVEEDOR2 en formato inesperado.");
            }

            var lineas = resultadoElemento.Descendants()
                .Where(e => e.Name.LocalName == "LineaAdministrativaDto")
                .Select(e => new LineaDisponible
                {
                    ServicioId = LeerEntero(e, "ServicioId"),
                    NumeroTelefono = LeerTexto(e, "NumeroTelefono"),
                    TipoServicio = LeerTexto(e, "TipoServicio")
                })
                .Where(l => l.ServicioId > 0 && !string.IsNullOrWhiteSpace(l.NumeroTelefono))
                .ToList();

            return new ListarLineasDisponiblesResult
            {
                Resultado = LeerBooleano(resultadoElemento, "Resultado"),
                Mensaje = LeerTexto(resultadoElemento, "Mensaje"),
                Lineas = lineas
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return ErrorListado("No se pudo interpretar la respuesta de WS_PROVEEDOR2: " + ex.Message);
        }
    }

    private static string LeerTexto(XElement elemento, string nombre)
    {
        XElement? hijo = elemento.Elements()
            .FirstOrDefault(e => e.Name.LocalName == nombre);
        return string.IsNullOrEmpty(hijo?.Value) ? string.Empty : hijo.Value;
    }

    private static bool LeerBooleano(XElement elemento, string nombre)
    {
        string valor = LeerTexto(elemento, nombre);
        return bool.TryParse(valor, out bool resultado) && resultado;
    }

    private static int LeerEntero(XElement elemento, string nombre)
    {
        string valor = LeerTexto(elemento, nombre);
        return int.TryParse(valor, out int resultado) ? resultado : 0;
    }

    private static string CrearSobre(string cuerpo) =>
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
        + $"<soap:Envelope xmlns:soap=\"{SoapEnvelopeNamespace}\">"
        + "<soap:Body>"
        + cuerpo
        + "</soap:Body>"
        + "</soap:Envelope>";

    private static string EscapeXml(string valor) =>
        (valor ?? string.Empty)
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
}
