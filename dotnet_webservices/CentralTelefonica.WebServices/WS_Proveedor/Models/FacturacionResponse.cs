using System.Runtime.Serialization;

namespace CentralTelefonica.WebServices.WS_Proveedor.Models;

[DataContract]
public class FacturacionResponse
{
    [DataMember]
    public bool Exito { get; set; }

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;

    [DataMember]
    public decimal TotalFacturar { get; set; }
}
