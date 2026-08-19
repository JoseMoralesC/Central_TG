namespace PortalCliente.Models;

public class PerfilClienteViewModel
{
    public string Identificacion { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string PrimerApellido { get; set; } = string.Empty;

    public string SegundoApellido { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public string CorreoElectronico { get; set; } = string.Empty;

    public string UsuarioActual { get; set; } = string.Empty;

    public string NuevoUsuario { get; set; } = string.Empty;

    public string NuevaContrasena { get; set; } = string.Empty;

    public bool Procesado { get; set; }

    public bool Exitoso { get; set; }

    public string MensajeResultado { get; set; } = string.Empty;
}

public class ClientePerfilResult
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public PerfilClienteViewModel? Perfil { get; set; }

    public string UsuarioEncriptado { get; set; } = string.Empty;
}

public class OperacionPerfilResult
{
    public bool Resultado { get; set; }

    public string Mensaje { get; set; } = string.Empty;
}
