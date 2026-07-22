using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WS_Autenticacion.Models
{   
    [BsonIgnoreExtraElements]
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("identificacion")]
        public string Identificacion { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; }

        [BsonElement("primerApellido")]
        public string PrimerApellido { get; set; }

        [BsonElement("segundoApellido")]
        [BsonIgnoreIfNull]
        public string SegundoApellido { get; set; }

        [BsonElement("correo")]
        public string Correo { get; set; }

         [BsonElement("usuario")]
        public string UsuarioCifrado { get; set; }

        [BsonElement("contrasena")]
        public string ContrasenaCifrada { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; }

        [BsonElement("tipo")]
        public int Tipo { get; set; }

        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [BsonElement("fechaActualizacion")]
        public DateTime FechaActualizacion { get; set; }
    }
}