using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;
using WS_Autenticacion.Models;

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

        public Usuario ObtenerPorIdentificacionYTipo(string identificacion, int tipo) =>
            _coleccion.Find(u => u.Identificacion == identificacion && u.Tipo == tipo).FirstOrDefault();

        public bool ExisteIdentificacion(string identificacion) =>
            ObtenerPorIdentificacion(identificacion) != null;

        public bool ExisteIdentificacion(string identificacion, int tipo) =>
            ObtenerPorIdentificacionYTipo(identificacion, tipo) != null;

        public bool ExisteCorreo(string correo) =>
            _coleccion.Find(u => u.Correo == correo).Any();

        public bool ExisteCorreo(string correo, int tipo) =>
            _coleccion.Find(u => u.Correo == correo && u.Tipo == tipo).Any();

        public Usuario ObtenerPorUsuarioCifrado(string usuarioCifrado) =>
            _coleccion.Find(u => u.UsuarioCifrado == usuarioCifrado).FirstOrDefault();

        public Usuario ObtenerPorUsuarioCifradoYTipo(string usuarioCifrado, int tipo) =>
            _coleccion.Find(u => u.UsuarioCifrado == usuarioCifrado && u.Tipo == tipo).FirstOrDefault();

        public bool ExisteUsuarioCifrado(string usuarioCifrado) =>
            ObtenerPorUsuarioCifrado(usuarioCifrado) != null;

        public bool ExisteUsuarioCifrado(string usuarioCifrado, int tipo) =>
            ObtenerPorUsuarioCifradoYTipo(usuarioCifrado, tipo) != null;

        public List<Usuario> ObtenerTodos() =>
            _coleccion.Find(FilterDefinition<Usuario>.Empty).ToList();

        public List<Usuario> ObtenerPorTipo(int tipo) =>
            _coleccion.Find(u => u.Tipo == tipo).ToList();

        public void Insertar(Usuario usuario) => _coleccion.InsertOne(usuario);

        public void Actualizar(Usuario usuario) =>
            _coleccion.ReplaceOne(
                u => u.Identificacion == usuario.Identificacion && u.Tipo == usuario.Tipo,
                usuario);

        public void ActualizarMetodoPago(
            string identificacion,
            string numeroTarjetaCifrado,
            string nombreTarjetaCifrado,
            string fechaVencimientoCifrada,
            string codigoSeguridadCifrado)
        {
            var update = Builders<Usuario>.Update
                .Set(u => u.MetodoPagoNumeroTarjetaCifrado, numeroTarjetaCifrado)
                .Set(u => u.MetodoPagoNombreTarjetaCifrado, nombreTarjetaCifrado)
                .Set(u => u.MetodoPagoFechaVencimientoCifrada, fechaVencimientoCifrada)
                .Set(u => u.MetodoPagoCodigoSeguridadCifrado, codigoSeguridadCifrado)
                .Set(u => u.FechaActualizacion, System.DateTime.UtcNow);

            _coleccion.UpdateOne(
                u => u.Identificacion == identificacion && u.Tipo == 2,
                update);
        }

        public bool EliminarPorIdentificacion(string identificacion)
        {
            var resultado = _coleccion.DeleteOne(u => u.Identificacion == identificacion);
            return resultado.DeletedCount > 0;
        }

        public bool EliminarPorIdentificacionYTipo(string identificacion, int tipo)
        {
            var resultado = _coleccion.DeleteOne(
                u => u.Identificacion == identificacion && u.Tipo == tipo);
            return resultado.DeletedCount > 0;
        }
    }
}
