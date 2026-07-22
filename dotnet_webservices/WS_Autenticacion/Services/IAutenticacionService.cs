using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Security.Cryptography;
using System.Text;
using WS_Autenticacion.Contracts;
using WS_Autenticacion.Models;
using WS_Autenticacion.Validators;
using MongoDB.Driver;
using System.Runtime.Serialization;


namespace WS_Autenticacion
{
    public class AutenticacionService : IAutenticacionService
    {
        private readonly IMongoCollection<Usuario> _usuarios;

        public AutenticacionService(IMongoDatabase database)
        {
            _usuarios = database.GetCollection<Usuario>("usuarios");
        }

        public AutenticacionResponse AutenticarUsuario(AutenticacionRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Contrasena))
            {
                return CrearRespuesta(false, "Usuario y contraseña son obligatorios.");
            }

            var passwordHash = HashPassword(request.Contrasena);
            var usuarioAutenticado = _usuarios
                .Find(u => u.UserUsuario == request.Usuario.Trim() && u.Contrasena == passwordHash && u.Tipo == request.Tipo && u.Estado == "activo")
                .FirstOrDefault();

            return usuarioAutenticado is null
                ? CrearRespuesta(false, "Credenciales inválidas o usuario inactivo.")
                : CrearRespuesta(true, "Autenticación exitosa.", usuarioAutenticado);
        }

        public AutenticacionResponse CrearUsuario(Usuario usuario)
        {
            if (usuario is null)
            {
                return CrearRespuesta(false, "El usuario no puede ser nulo.");
            }

            var error = UsuarioValidator.Validar(usuario);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return CrearRespuesta(false, error);
            }

            var existeDuplicado = _usuarios.Find(u =>
                u.Usuario == usuario.Usuario ||
                u.Correo == usuario.Correo ||
                u.Identificacion == usuario.Identificacion).Any();

            if (existeDuplicado)
            {
                return CrearRespuesta(false, "Ya existe un usuario con ese nombre de usuario, correo o identificación.");
            }

            usuario.Contrasena = HashPassword(usuario.Contrasena);
            usuario.Estado = string.IsNullOrWhiteSpace(usuario.Estado) ? "activo" : usuario.Estado.ToLowerInvariant();
            usuario.Tipo = usuario.Tipo == 0 ? 1 : usuario.Tipo;
            usuario.FechaCreacion = DateTime.UtcNow;
            usuario.FechaActualizacion = DateTime.UtcNow;

            _usuarios.InsertOne(usuario);
            return CrearRespuesta(true, "Usuario creado correctamente.", usuario);
        }

        public AutenticacionResponse ModificarUsuario(Usuario usuario)
        {
            if (usuario is null || string.IsNullOrWhiteSpace(usuario.Id))
            {
                return CrearRespuesta(false, "Debe indicar el identificador del usuario.");
            }

            var error = UsuarioValidator.Validar(usuario);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return CrearRespuesta(false, error);
            }

            var existeDuplicado = _usuarios.Find(u =>
                u.Id != usuario.Id &&
                (u.Usuario == usuario.Usuario || u.Correo == usuario.Correo || u.Identificacion == usuario.Identificacion)).Any();

            if (existeDuplicado)
            {
                return CrearRespuesta(false, "Ya existe un usuario con ese nombre de usuario, correo o identificación.");
            }

            var update = Builders<Usuario>.Update
                .Set(u => u.Identificacion, usuario.Identificacion)
                .Set(u => u.Nombre, usuario.Nombre)
                .Set(u => u.PrimerApellido, usuario.PrimerApellido)
                .Set(u => u.SegundoApellido, usuario.SegundoApellido)
                .Set(u => u.Correo, usuario.Correo)
                .Set(u => u.Usuario, usuario.Usuario)
                .Set(u => u.Tipo, usuario.Tipo)
                .Set(u => u.Estado, usuario.Estado)
                .Set(u => u.FechaActualizacion, DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
            {
                update = update.Set(u => u.Contrasena, HashPassword(usuario.Contrasena));
            }

            var resultado = _usuarios.UpdateOne(u => u.Id == usuario.Id, update);
            return resultado.ModifiedCount > 0
                ? CrearRespuesta(true, "Usuario actualizado correctamente.", usuario)
                : CrearRespuesta(false, "No se encontró el usuario indicado.");
        }

        public AutenticacionResponse CambiarEstadoUsuario(string usuario, string estado)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                return CrearRespuesta(false, "Debe indicar el nombre de usuario.");
            }

            if (estado is not null && estado.ToLowerInvariant() != "activo" && estado.ToLowerInvariant() != "inactivo")
            {
                return CrearRespuesta(false, "El estado debe ser activo o inactivo.");
            }

            var resultado = _usuarios.UpdateOne(
                u => u.Usuario == usuario.Trim(),
                Builders<Usuario>.Update
                    .Set(u => u.Estado, estado.ToLowerInvariant())
                    .Set(u => u.FechaActualizacion, DateTime.UtcNow));

            return resultado.ModifiedCount > 0
                ? CrearRespuesta(true, "Estado actualizado correctamente.")
                : CrearRespuesta(false, "No se encontró el usuario indicado.");
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static AutenticacionResponse CrearRespuesta(bool exito, string mensaje, Usuario? usuario = null)
        {
            return new AutenticacionResponse
            {
                Exito = exito,
                Mensaje = mensaje,
                Usuario = usuario
            };
        }
    }
}
