using System.Text;
using System.Xml.Linq;
using PortalCliente.Models;

namespace PortalCliente.Services;

public class AutenticacionPortalSoapClient : IAutenticacionPortalService
{
    private const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
    private const string ServiceNamespace = "http://centraltelefonica.cr/ws/autenticacion";
    private const string ObtenerMetodoAction = "http://centraltelefonica.cr/ws/autenticacion/IAutenticacionService/ObtenerMetodoPagoCliente";
    private const string ListarUsuariosAction = "http://centraltelefonica.cr/ws/autenticacion/IAutenticacionService/ListarUsuariosPorTipo";
    private const string ModificarClienteAction = "http://centraltelefonica.cr/ws/autenticacion/IAutenticacionService/ModificarCliente";
    private const int TipoCliente = 2;

    private readonly HttpClient _httpClient;
    private readonly string _url;
    private readonly CryptoHelper _cryptoHelper;

    public AutenticacionPortalSoapClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        CryptoHelper cryptoHelper)
    {
        _httpClient = httpClientFactory.CreateClient("AutenticacionPortal");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _url = configuration["Autenticacion:Url"]
            ?? "http://localhost:59113/Service1.svc";
        _cryptoHelper = cryptoHelper;
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

    public async Task<ClientePerfilResult> ObtenerPerfilClienteAsync(string identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return ErrorPerfil("Debe iniciar sesion para consultar sus datos.");
        }

        string cuerpo = $"<ListarUsuariosPorTipo xmlns=\"{ServiceNamespace}\">"
            + $"<tipo>{TipoCliente}</tipo>"
            + "</ListarUsuariosPorTipo>";

        string xml = await EnviarSoapAsync(cuerpo, ListarUsuariosAction).ConfigureAwait(false);
        if (xml.StartsWith("ERROR:", StringComparison.Ordinal))
        {
            return ErrorPerfil(xml.Substring("ERROR:".Length));
        }

        return ParsearPerfil(xml, identificacion.Trim());
    }

    public async Task<OperacionPerfilResult> ActualizarPerfilClienteAsync(PerfilClienteViewModel perfil)
    {
        if (perfil is null || string.IsNullOrWhiteSpace(perfil.Identificacion))
        {
            return ErrorOperacion("Debe iniciar sesion para actualizar sus datos.");
        }

        ClientePerfilResult actual = await ObtenerPerfilClienteAsync(perfil.Identificacion)
            .ConfigureAwait(false);
        if (!actual.Resultado || string.IsNullOrWhiteSpace(actual.UsuarioEncriptado))
        {
            return ErrorOperacion(actual.Mensaje);
        }

        PerfilClienteViewModel perfilActual = actual.Perfil!;
        string usuarioEncriptado = string.IsNullOrWhiteSpace(perfil.NuevoUsuario)
            ? actual.UsuarioEncriptado
            : _cryptoHelper.Encrypt(perfil.NuevoUsuario.Trim());
        string contrasenaEncriptada = string.IsNullOrWhiteSpace(perfil.NuevaContrasena)
            ? string.Empty
            : _cryptoHelper.Encrypt(perfil.NuevaContrasena);

        string cuerpo = $"<ModificarCliente xmlns=\"{ServiceNamespace}\">"
            + $"<identificacion>{EscapeXml(perfilActual.Identificacion.Trim())}</identificacion>"
            + $"<nombre>{EscapeXml(perfilActual.Nombre.Trim())}</nombre>"
            + $"<primerApellido>{EscapeXml(perfilActual.PrimerApellido.Trim())}</primerApellido>"
            + $"<segundoApellido>{EscapeXml(perfilActual.SegundoApellido?.Trim() ?? string.Empty)}</segundoApellido>"
            + $"<correoElectronico>{EscapeXml(perfil.CorreoElectronico.Trim())}</correoElectronico>"
            + $"<usuarioEncriptado>{EscapeXml(usuarioEncriptado)}</usuarioEncriptado>"
            + $"<contrasenaEncriptada>{EscapeXml(contrasenaEncriptada)}</contrasenaEncriptada>"
            + "</ModificarCliente>";

        string xml = await EnviarSoapAsync(cuerpo, ModificarClienteAction).ConfigureAwait(false);
        if (xml.StartsWith("ERROR:", StringComparison.Ordinal))
        {
            return ErrorOperacion(xml.Substring("ERROR:".Length));
        }

        return ParsearOperacion(xml, "ModificarClienteResult");
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

    private async Task<string> EnviarSoapAsync(string cuerpo, string soapAction)
    {
        string sobre = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
            + $"<soap:Envelope xmlns:soap=\"{SoapEnvelopeNamespace}\">"
            + "<soap:Body>"
            + cuerpo
            + "</soap:Body>"
            + "</soap:Envelope>";

        using var request = new HttpRequestMessage(HttpMethod.Post, _url);
        request.Headers.Add("SOAPAction", "\"" + soapAction + "\"");
        request.Content = new StringContent(sobre, Encoding.UTF8, "text/xml");

        try
        {
            using HttpResponseMessage respuesta =
                await _httpClient.SendAsync(request).ConfigureAwait(false);

            string xml = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!respuesta.IsSuccessStatusCode)
            {
                return "ERROR:El servicio respondio con error HTTP: "
                    + (int)respuesta.StatusCode;
            }

            return xml;
        }
        catch (TaskCanceledException)
        {
            return "ERROR:El servicio WS_Autenticacion no respondio a tiempo.";
        }
        catch (HttpRequestException ex)
        {
            return "ERROR:No se pudo conectar con WS_Autenticacion: " + ex.Message;
        }
    }

    private ClientePerfilResult ParsearPerfil(string xml, string identificacion)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);
            XElement? resultado = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == "ListarUsuariosPorTipoResult");

            if (resultado is null)
            {
                return ErrorPerfil("Respuesta del servicio en formato inesperado.");
            }

            bool correcto = LeerBooleano(resultado, "Resultado");
            if (!correcto)
            {
                return ErrorPerfil(LeerTexto(resultado, "Mensaje"));
            }

            XElement? usuario = resultado.Descendants()
                .FirstOrDefault(e =>
                    e.Name.LocalName == "UsuarioServicio"
                    && string.Equals(
                        LeerTexto(e, "Identificacion"),
                        identificacion,
                        StringComparison.OrdinalIgnoreCase));

            if (usuario is null)
            {
                return ErrorPerfil("No se encontraron datos del cliente en WS_Autenticacion.");
            }

            string usuarioEncriptado = LeerTexto(usuario, "UsuarioEncriptado");
            string usuarioActual = _cryptoHelper.TryDecrypt(usuarioEncriptado, out string usuarioPlano)
                ? usuarioPlano
                : string.Empty;

            return new ClientePerfilResult
            {
                Resultado = true,
                Mensaje = "Exitoso",
                UsuarioEncriptado = usuarioEncriptado,
                Perfil = new PerfilClienteViewModel
                {
                    Identificacion = LeerTexto(usuario, "Identificacion"),
                    Nombre = LeerTexto(usuario, "Nombre"),
                    PrimerApellido = LeerTexto(usuario, "PrimerApellido"),
                    SegundoApellido = LeerTexto(usuario, "SegundoApellido"),
                    CorreoElectronico = LeerTexto(usuario, "CorreoElectronico"),
                    Estado = LeerTexto(usuario, "Estado"),
                    UsuarioActual = usuarioActual
                }
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return ErrorPerfil("No se pudo interpretar la respuesta del servicio: " + ex.Message);
        }
    }

    private static OperacionPerfilResult ParsearOperacion(string xml, string nombreResultado)
    {
        try
        {
            XDocument documento = XDocument.Parse(xml);
            XElement? resultado = documento.Descendants()
                .FirstOrDefault(e => e.Name.LocalName == nombreResultado);

            if (resultado is null)
            {
                return ErrorOperacion("Respuesta del servicio en formato inesperado.");
            }

            return new OperacionPerfilResult
            {
                Resultado = LeerBooleano(resultado, "Resultado"),
                Mensaje = LeerTexto(resultado, "Mensaje")
            };
        }
        catch (System.Xml.XmlException ex)
        {
            return ErrorOperacion("No se pudo interpretar la respuesta del servicio: " + ex.Message);
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

    private static ClientePerfilResult ErrorPerfil(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje };

    private static OperacionPerfilResult ErrorOperacion(string mensaje) =>
        new() { Resultado = false, Mensaje = mensaje };

    private static string EscapeXml(string valor) =>
        valor
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
}
