using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CentralTelefonica.WS_Autenticacion.Models;
using MongoDB.Driver;

namespace CentralTelefonica.WS_Autenticacion.Data
{
    public class UsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _coleccion;

        public UsuarioRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoUsuarios"].ConnectionString;
            // Nombre real segun crear_coleccion_usuarios.js del equipo.
            var databaseName = ConfigurationManager.AppSettings["MongoDatabaseName"] ?? "central_tg_mongo";

            var cliente = new MongoClient(connectionString);
            var baseDatos = cliente.GetDatabase(databaseName);
            _coleccion = baseDatos.GetCollection<Usuario>("usuarios");
        }

        public Usuario ObtenerPorIdentificacion(string identificacion) =>
            _coleccion.Find(u => u.Identificacion == identificacion).FirstOrDefault();

        public bool ExisteIdentificacion(string identificacion) =>
            ObtenerPorIdentificacion(identificacion) != null;

        public bool ExisteCorreo(string correo) =>
            _coleccion.Find(u => u.Correo == correo).Any();

        public Usuario ObtenerPorUsuarioCifrado(string usuarioCifrado) =>
            _coleccion.Find(u => u.UsuarioCifrado == usuarioCifrado).FirstOrDefault();

        public bool ExisteUsuarioCifrado(string usuarioCifrado) =>
            ObtenerPorUsuarioCifrado(usuarioCifrado) != null;

        public List<Usuario> ObtenerTodos() =>
            _coleccion.Find(FilterDefinition<Usuario>.Empty).ToList();

        public void Insertar(Usuario usuario) => _coleccion.InsertOne(usuario);

        public void Actualizar(Usuario usuario) =>
            _coleccion.ReplaceOne(u => u.Identificacion == usuario.Identificacion, usuario);
    }
}