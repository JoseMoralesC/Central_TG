using System.Text;
using System.Xml.Linq;
using PortalCliente.Models;

namespace PortalCliente.Services;

public class AutenticacionPortalSoapClient : IAutenticacionPortalService
{
    private const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
    private const string ServiceNamespace = "http://centraltelefonica.cr/ws/autenticacion";
    private const string ObtenerMetodoAction = "http://centraltelefonica.cr/ws/autenticacion/IAutenticacionService/ObtenerMetodoPagoCliente";

    private readonly HttpClient _httpClient;
    private readonly string _url;

    public AutenticacionPortalSoapClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("AutenticacionPortal");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _url = configuration["Autenticacion:Url"]
            ?? "http://localhost:59113/Service1.svc";
    }

    public async Task<MetodoPagoCliente> ObtenerMetodoPagoClienteAsync(string identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return Error("Ingrese una identificacion.");
        }

        string cuerpo = $"<ObtenerMetodoPagoCliente xmlns=\"{ServiceNamespace}\">"
            + $"<identificacion>{EscapeXml(identificacion.Trim())}</identificacion>"
            + "</ObtenerMetodoPagoCliente>";

        string sobre = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
            + $"<soap:Envelope xmlns:soap=\"{SoapEnvelopeNamespace}\">"
            + "<soap:Body>"
            + cuerpo
            + "</soap:Body>"
            + "</soap:Envelope>";

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + ObtenerMetodoAction + "\"");
        request.Content = new StringContent(sobre, Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return Error("El servicio respondio con error HTTP: "
                    + (int)respuesta.StatusCode);
            }

            return ParsearRespuesta(xml);
        }
        catch (TaskCanceledException)
        {
            return Error("El servicio WS_Autenticacion no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return Error("No se pudo conectar con WS_Autenticacion: " + ex.Message);
        }
    }

    private static MetodoPagoCliente ParsearRespuesta(string xml)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);
            XElement? resultado = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "ObtenerMetodoPagoClienteResult");

            if (resultado is null)
            {
                return Error("Respuesta del servicio en formato inesperado.");
            }

            return new MetodoPagoCliente
            {
                Resultado = LeerBooleano(resultado, "Resultado"),
                Mensaje = LeerTexto(resultado, "Mensaje"),
                NumeroTarjeta = FormatearTarjeta(LeerTexto(resultado, "NumeroTarjeta")),
                NombreTarjeta = LeerTexto(resultado, "NombreTarjeta"),
                FechaVencimiento = LeerTexto(resultado, "FechaVencimiento"),
                CodigoSeguridad = LeerTexto(resultado, "CodigoSeguridad")
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return Error("No se pudo interpretar la respuesta del servicio: " + ex.Message);
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
        return bool.TryParse(LeerTexto(elemento, nombre), out bool resultado) && resultado;
    }

    private static string FormatearTarjeta(string valor)
    {
        string digitos = new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digitos.Length != 12)
        {
            return valor ?? string.Empty;
        }

        return string.Join("-", Enumerable.Range(0, 3)
            .Select(i => digitos.Substring(i * 4, 4)));
    }

    private static MetodoPagoCliente Error(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje };

    private static string EscapeXml(string valor) =>
        valor
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
}
