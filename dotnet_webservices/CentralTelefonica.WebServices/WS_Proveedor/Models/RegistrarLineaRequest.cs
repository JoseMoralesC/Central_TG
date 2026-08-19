using System.Runtime.Serialization;

namespace CentralTelefonica.WebServices.WS_Proveedor.Models;

[DataContract]
public class RegistrarLineaRequest
{
    [DataMember(Order = 1, IsRequired = true)]
    public string NumeroTelefono { get; set; } = string.Empty;

    [DataMember(Order = 2, IsRequired = true)]
    public string IdentificadorTelefono { get; set; } = string.Empty;

    [DataMember(Order = 3, IsRequired = true)]
    public string IdentificadorTarjeta { get; set; } = string.Empty;

    [DataMember(Order = 4, IsRequired = true)]
    public string Tipo { get; set; } = string.Empty;

    [DataMember(Order = 5, IsRequired = true)]
    public string Estado { get; set; } = string.Empty;
}
