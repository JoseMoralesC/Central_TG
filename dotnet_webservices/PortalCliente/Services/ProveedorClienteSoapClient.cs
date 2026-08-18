using System.Net.Http;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using PortalCliente.Models;

namespace PortalCliente.Services;

public class ProveedorClienteSoapClient : IProveedorClienteService
{
    private const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
    private const string ServiceNamespace = "http://tempuri.org/";
    private const string ConsultarAction = "http://tempuri.org/IProveedorClienteService/ConsultarLineasCliente";
    private const string RecargarAction = "http://tempuri.org/IProveedorClienteService/RecargarSaldo";
    private const string PagarAction = "http://tempuri.org/IProveedorClienteService/PagarFactura";

    private readonly HttpClient _httpClient;
    private readonly string _url;

    public ProveedorClienteSoapClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("ProveedorCliente");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _url = configuration["ProveedorCliente:Url"]
            ?? "http://localhost:55260/ProveedorClienteService.svc";
    }

    public async Task<ConsultarLineasClienteResult> ConsultarLineasClienteAsync(
        string identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return new ConsultarLineasClienteResult
            {
                Resultado = false,
                Mensaje = "Ingrese una identificacion."
            };
        }

        string cuerpo = $"<ConsultarLineasCliente xmlns=\"{ServiceNamespace}\">"
            + $"<identificacion>{EscapeXml(identificacion.Trim())}</identificacion>"
            + "</ConsultarLineasCliente>";

        string sobre = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
            + $"<soap:Envelope xmlns:soap=\"{SoapEnvelopeNamespace}\">"
            + "<soap:Body>"
            + cuerpo
            + "</soap:Body>"
            + "</soap:Envelope>";

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + ConsultarAction + "\"");
        request.Content = new StringContent(sobre, Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return new ConsultarLineasClienteResult
                {
                    Resultado = false,
                    Mensaje = "El servicio respondio con error HTTP: "
                        + (int)respuesta.StatusCode
                };
            }

            return ParsearRespuesta(xml);
        }
        catch (TaskCanceledException)
        {
            return Error("El servicio WS_ProveedorCliente no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return Error("No se pudo conectar con WS_ProveedorCliente: " + ex.Message);
        }
    }

    public async Task<RecargarSaldoResult> RecargarSaldoAsync(
        string numeroTelefono,
        decimal monto)
    {
        if (string.IsNullOrWhiteSpace(numeroTelefono))
        {
            return ErrorRecarga("El numero de telefono es obligatorio.");
        }

        string cifraMonto = monto.ToString("0", CultureInfo.InvariantCulture);

        string cuerpo = $"<RecargarSaldo xmlns=\"{ServiceNamespace}\">"
            + $"<numeroTelefono>{EscapeXml(numeroTelefono.Trim())}</numeroTelefono>"
            + $"<monto>{cifraMonto}</monto>"
            + "</RecargarSaldo>";

        string sobre = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
            + $"<soap:Envelope xmlns:soap=\"{SoapEnvelopeNamespace}\">"
            + "<soap:Body>"
            + cuerpo
            + "</soap:Body>"
            + "</soap:Envelope>";

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + RecargarAction + "\"");
        request.Content = new StringContent(sobre, Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return ErrorRecarga("El servicio respondio con error HTTP: "
                    + (int)respuesta.StatusCode);
            }

            return ParsearRespuestaRecarga(xml);
        }
        catch (TaskCanceledException)
        {
            return ErrorRecarga("El servicio WS_ProveedorCliente no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return ErrorRecarga("No se pudo conectar con WS_ProveedorCliente: " + ex.Message);
        }
    }

    private static RecargarSaldoResult ParsearRespuestaRecarga(string xml)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);

            XElement? resultadoElemento = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "RecargarSaldoResult");

            if (resultadoElemento is null)
            {
                return ErrorRecarga("Respuesta del servicio en formato inesperado.");
            }

            return new RecargarSaldoResult
            {
                Resultado = LeerBooleano(resultadoElemento, "Resultado"),
                Mensaje = LeerTexto(resultadoElemento, "Mensaje"),
                NuevoSaldo = LeerDecimal(resultadoElemento, "NuevoSaldo")
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return ErrorRecarga("No se pudo interpretar la respuesta del servicio: " + ex.Message);
        }
    }

    private static RecargarSaldoResult ErrorRecarga(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje };

    public async Task<PagarFacturaResult> PagarFacturaAsync(
        string numeroTelefono,
        decimal monto,
        string identificacion)
    {
        if (string.IsNullOrWhiteSpace(numeroTelefono))
        {
            return ErrorPago("El numero de telefono es obligatorio.");
        }

        string cifraMonto = monto.ToString("0.00", CultureInfo.InvariantCulture);

        string cuerpo = $"<PagarFactura xmlns=\"{ServiceNamespace}\">"
            + $"<numeroTelefono>{EscapeXml(numeroTelefono.Trim())}</numeroTelefono>"
            + $"<monto>{cifraMonto}</monto>"
            + $"<identificacion>{EscapeXml(identificacion.Trim())}</identificacion>"
            + "</PagarFactura>";

        string sobre = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
            + $"<soap:Envelope xmlns:soap=\"{SoapEnvelopeNamespace}\">"
            + "<soap:Body>"
            + cuerpo
            + "</soap:Body>"
            + "</soap:Envelope>";

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + PagarAction + "\"");
        request.Content = new StringContent(sobre, Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return ErrorPago("El servicio respondio con error HTTP: "
                    + (int)respuesta.StatusCode);
            }

            return ParsearRespuestaPago(xml);
        }
        catch (TaskCanceledException)
        {
            return ErrorPago("El servicio WS_ProveedorCliente no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return ErrorPago("No se pudo conectar con WS_ProveedorCliente: " + ex.Message);
        }
    }

    private static PagarFacturaResult ParsearRespuestaPago(string xml)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);

            XElement? resultadoElemento = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "PagarFacturaResult");

            if (resultadoElemento is null)
            {
                return ErrorPago("Respuesta del servicio en formato inesperado.");
            }

            return new PagarFacturaResult
            {
                Resultado = LeerBooleano(resultadoElemento, "Resultado"),
                Mensaje = LeerTexto(resultadoElemento, "Mensaje"),
                CorreoCliente = LeerTexto(resultadoElemento, "CorreoCliente"),
                FacturacionId = LeerEntero(resultadoElemento, "FacturacionId"),
                MontoCancelado = LeerDecimal(resultadoElemento, "MontoCancelado"),
                FechaPago = LeerFecha(resultadoElemento, "FechaPago") ?? DateTime.Now
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return ErrorPago("No se pudo interpretar la respuesta del servicio: " + ex.Message);
        }
    }

    private static PagarFacturaResult ErrorPago(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje };

    private static int LeerEntero(XElement elemento, string nombre)
    {
        string valor = LeerTexto(elemento, nombre);
        return int.TryParse(valor, out int resultado) ? resultado : 0;
    }

    private static ConsultarLineasClienteResult ParsearRespuesta(string xml)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);

            XElement? resultadoElemento = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "ConsultarLineasClienteResult");

            if (resultadoElemento is null)
            {
                return new ConsultarLineasClienteResult
                {
                    Resultado = false,
                    Mensaje = "Respuesta del servicio en formato inesperado."
                };
            }

            var lineas = new List<LineaCliente>();

            XElement? items = resultadoElemento.Elements()
                .FirstOrDefault(e => e.Name.LocalName == "Lineas");

            if (items is not null)
            {
                foreach (XElement item in items.Elements())
                {
                    if (item.Name.LocalName != "LineaClienteDto")
                    {
                        continue;
                    }

                    lineas.Add(new LineaCliente
                    {
                        NumeroTelefono = LeerTexto(item, "NumeroTelefono"),
                        TipoServicio = LeerTexto(item, "TipoServicio"),
                        Saldo = LeerDecimal(item, "Saldo"),
                        FacturaPendiente = LeerDecimal(item, "FacturaPendiente"),
                        FacturaFechaMaximaPago = LeerFecha(item, "FacturaFechaMaximaPago"),
                        Activo = LeerBooleano(item, "Activo"),
                        IdentificadorTelefono = LeerTexto(item, "IdentificadorTelefono"),
                        IdentificadorTarjeta = LeerTexto(item, "IdentificadorTarjeta"),
                        IdentificacionDuenoCifrada = LeerTexto(item, "IdentificacionDuenoCifrada")
                    });
                }
            }

            return new ConsultarLineasClienteResult
            {
                Resultado = LeerBooleano(resultadoElemento, "Resultado"),
                Mensaje = LeerTexto(resultadoElemento, "Mensaje"),
                Lineas = lineas
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return Error("No se pudo interpretar la respuesta del servicio: " + ex.Message);
        }
    }

    private static ConsultarLineasClienteResult Error(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje };

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

    private static decimal LeerDecimal(XElement elemento, string nombre)
    {
        string valor = LeerTexto(elemento, nombre);
        if (decimal.TryParse(
            valor,
            System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal resultado))
        {
            return resultado;
        }

        return 0m;
    }

    private static DateTime? LeerFecha(XElement elemento, string nombre)
    {
        string valor = LeerTexto(elemento, nombre);
        return DateTime.TryParse(valor, out DateTime resultado) ? resultado : null;
    }

    private static string EscapeXml(string valor) =>
        valor
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
}