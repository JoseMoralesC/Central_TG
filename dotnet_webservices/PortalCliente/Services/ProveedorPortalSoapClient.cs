using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using PortalCliente.Models;

namespace PortalCliente.Services;

public class ProveedorPortalSoapClient : IProveedorPortalService
{
    private const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
    private const string ServiceNamespace = "http://tempuri.org/";
    private const string ConsultarSaldoAction = "http://tempuri.org/IService1/ConsultarSaldo";

    private readonly HttpClient _httpClient;
    private readonly string _url;

    public ProveedorPortalSoapClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("WS_PROVEEDOR");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _url = configuration["Proveedor:Url"]
            ?? "http://localhost:53885/Service1.svc";
    }

    public async Task<ConsultarSaldoPortalResult> ConsultarSaldoAsync(
        string numeroTelefono)
    {
        if (string.IsNullOrWhiteSpace(numeroTelefono))
        {
            return Error("El numero de telefono es obligatorio.");
        }

        string cuerpo = $"<ConsultarSaldo xmlns=\"{ServiceNamespace}\">"
            + "<request xmlns:a=\"http://schemas.datacontract.org/2004/07/WS_Proveedor_1.Models\">"
            + $"<a:NumeroTelefono>{EscapeXml(numeroTelefono.Trim())}</a:NumeroTelefono>"
            + "<a:Origen>WEB</a:Origen>"
            + "<a:TipoTransaccion>CONSULTA_SALDO</a:TipoTransaccion>"
            + "</request>"
            + "</ConsultarSaldo>";

        string sobre = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
            + $"<soap:Envelope xmlns:soap=\"{SoapEnvelopeNamespace}\">"
            + "<soap:Body>"
            + cuerpo
            + "</soap:Body>"
            + "</soap:Envelope>";

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + ConsultarSaldoAction + "\"");
        request.Content = new StringContent(sobre, Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return Error("WS_PROVEEDOR respondio con error HTTP: "
                    + (int)respuesta.StatusCode);
            }

            return ParsearRespuesta(xml);
        }
        catch (TaskCanceledException)
        {
            return Error("El servicio WS_PROVEEDOR no respondio a tiempo.");
        }
        catch (HttpRequestException ex)
        {
            return Error("No se pudo conectar con WS_PROVEEDOR: " + ex.Message);
        }
    }

    private static ConsultarSaldoPortalResult ParsearRespuesta(string xml)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);

            XElement? resultadoElemento = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "ConsultarSaldoResult");

            if (resultadoElemento is null)
            {
                return Error("Respuesta de WS_PROVEEDOR en formato inesperado.");
            }

            return new ConsultarSaldoPortalResult
            {
                Resultado = LeerBooleano(resultadoElemento, "Resultado"),
                Mensaje = LeerTexto(resultadoElemento, "Mensaje"),
                Saldo = LeerDecimal(resultadoElemento, "Saldo")
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return Error("No se pudo interpretar la respuesta de WS_PROVEEDOR: " + ex.Message);
        }
    }

    private static ConsultarSaldoPortalResult Error(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje, Saldo = 0 };

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

    private static string EscapeXml(string valor) =>
        valor
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
}