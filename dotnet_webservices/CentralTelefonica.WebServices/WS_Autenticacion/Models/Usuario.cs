using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CentralTelefonica.WebServices.WS_Autenticacion.Models;

[DataContract]
public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [DataMember]
    public string? Id { get; set; }

    [BsonElement("identificacion")]
    [DataMember]
    public string Identificacion { get; set; } = string.Empty;

    [BsonElement("nombre")]
    [DataMember]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("primerApellido")]
    [DataMember]
    public string PrimerApellido { get; set; } = string.Empty;

    [BsonElement("segundoApellido")]
    [DataMember]
    public string SegundoApellido { get; set; } = string.Empty;

    [BsonElement("correo")]
    [DataMember]
    public string Correo { get; set; } = string.Empty;

    [BsonElement("usuario")]
    [DataMember]
    public string Usuario { get; set; } = string.Empty;

    [BsonElement("contrasena")]
    [DataMember]
    public string Contrasena { get; set; } = string.Empty;

    [BsonElement("estado")]
    [DataMember]
    public string Estado { get; set; } = "activo";

    [BsonElement("tipo")]
    [DataMember]
    public int Tipo { get; set; }

    [BsonElement("fechaCreacion")]
    [DataMember]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    [BsonElement("fechaActualizacion")]
    [DataMember]
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    public Usuario()
    {
        FechaCreacion = DateTime.UtcNow;
        FechaActualizacion = DateTime.UtcNow;
    }
}