using System.Runtime.Serialization;
using WS_Autenticacion.Models;

[DataContract]
public class AutenticacionResponse
{
    [DataMember]
    public bool Exito { get; set; }

    [DataMember]
    public string Mensaje { get; set; } = string.Empty;

    [DataMember]
    public Usuario Usuario { get; set; }
}
