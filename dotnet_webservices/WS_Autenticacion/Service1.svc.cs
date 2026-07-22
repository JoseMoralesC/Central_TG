using System;
using CentralTelefonica.WS_Autenticacion.Data;
using CentralTelefonica.WS_Autenticacion.Security;
using CentralTelefonica.WS_Autenticacion.Validators;
using WS_Autenticacion.Models;

namespace CentralTelefonica.WS_Autenticacion
{
    public class Service1 : IAutenticacionService
    {
        private const string MensajeCredencialesIncorrectas = "Usuario y/o contrasena incorrectos.";
        private const string MensajeUsuarioExisteOIncorrecto = "Usuario ya existe o datos incorrectos o incompletos.";
        private const string MensajeUsuarioNoExisteOIncorrecto = "Usuario no existe o datos incorrectos o incompletos.";
        private const string MensajeUsuarioNoExisteOEstadoIncorrecto = "Usuario no existe o datos incorrectos.";

        private readonly UsuarioRepository _repositorio;

        public Service1() : this(new UsuarioRepository()) { }

        public Service1(UsuarioRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public ResultadoOperacion AutenticarUsuario(string usuarioEncriptado, string contrasenaEncriptada, int tipo)
        {
            if (string.IsNullOrEmpty(usuarioEncriptado) || string.IsNullOrEmpty(contrasenaEncriptada))
                return ResultadoOperacion.Fallo(MensajeCredencialesIncorrectas);

            if (!CryptoHelper.TryDecrypt(contrasenaEncriptada, out var contrasenaPlana))
                return ResultadoOperacion.Fallo(MensajeCredencialesIncorrectas);

            var usuario = _repositorio.ObtenerPorUsuarioCifrado(usuarioEncriptado);

            if (usuario == null)
                return ResultadoOperacion.Fallo(MensajeCredencialesIncorrectas);

            if (usuario.Estado != "activo")
                return ResultadoOperacion.Fallo(MensajeCredencialesIncorrectas);

            if (usuario.Tipo != tipo)
                return ResultadoOperacion.Fallo(MensajeCredencialesIncorrectas);

            if (!CryptoHelper.TryDecrypt(usuario.ContrasenaCifrada, out var contrasenaBd) ||
                contrasenaBd != contrasenaPlana)
            {
                return ResultadoOperacion.Fallo(MensajeCredencialesIncorrectas);
            }

            return ResultadoOperacion.Ok();
        }

        public ResultadoOperacion CrearUsuario(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correoElectronico,
            string usuarioEncriptado,
            string contrasenaEncriptada,
            string estado,
            int tipo)
        {
            if (!UsuarioValidator.EsIdentificacionValida(identificacion) ||
                !UsuarioValidator.EsNombreValido(nombre) ||
                !UsuarioValidator.EsNombreValido(primerApellido) ||
                !UsuarioValidator.EsCorreoValido(correoElectronico) ||
                !UsuarioValidator.EsTipoValido(tipo))
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);
            }

            if (!string.IsNullOrWhiteSpace(segundoApellido) &&
                !UsuarioValidator.EsNombreValido(segundoApellido))
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);
            }

            if (estado != "activo")
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (string.IsNullOrEmpty(usuarioEncriptado))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (!CryptoHelper.TryDecrypt(contrasenaEncriptada, out var contrasenaPlana))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (!UsuarioValidator.EsContrasenaValida(contrasenaPlana))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (_repositorio.ExisteIdentificacion(identificacion))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (_repositorio.ExisteCorreo(correoElectronico.Trim()))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (_repositorio.ExisteUsuarioCifrado(usuarioEncriptado))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            var ahora = DateTime.UtcNow;
            var nuevoUsuario = new Usuario
            {
                Identificacion = identificacion,
                Nombre = nombre.Trim(),
                PrimerApellido = primerApellido.Trim(),
                SegundoApellido = string.IsNullOrWhiteSpace(segundoApellido) ? null : segundoApellido.Trim(),
                Correo = correoElectronico.Trim(),
                UsuarioCifrado = usuarioEncriptado,
                ContrasenaCifrada = CryptoHelper.Encrypt(contrasenaPlana),
                Estado = "activo",
                Tipo = tipo,
                FechaCreacion = ahora,
                FechaActualizacion = ahora
            };

            try
            {
                _repositorio.Insertar(nuevoUsuario);
            }
            catch (MongoDB.Driver.MongoWriteException)
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);
            }

            return ResultadoOperacion.Ok();
        }

        public ResultadoOperacion ModificarUsuario(
            string identificacion,
            string nombre,
            string primerApellido,
            string segundoApellido,
            string correoElectronico,
            string usuarioEncriptado,
            string contrasenaEncriptada)
        {
            if (!UsuarioValidator.EsIdentificacionValida(identificacion))
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);

            var usuarioExistente = _repositorio.ObtenerPorIdentificacion(identificacion);
            if (usuarioExistente == null)
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                if (!UsuarioValidator.EsNombreValido(nombre))
                    return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
                usuarioExistente.Nombre = nombre.Trim();
            }

            if (!string.IsNullOrWhiteSpace(primerApellido))
            {
                if (!UsuarioValidator.EsNombreValido(primerApellido))
                    return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
                usuarioExistente.PrimerApellido = primerApellido.Trim();
            }

            if (!string.IsNullOrWhiteSpace(segundoApellido))
            {
                if (!UsuarioValidator.EsNombreValido(segundoApellido))
                    return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
                usuarioExistente.SegundoApellido = segundoApellido.Trim();
            }

            if (!string.IsNullOrWhiteSpace(correoElectronico))
            {
                if (!UsuarioValidator.EsCorreoValido(correoElectronico))
                    return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);

                var correoNuevo = correoElectronico.Trim();
                if (correoNuevo != usuarioExistente.Correo && _repositorio.ExisteCorreo(correoNuevo))
                    return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);

                usuarioExistente.Correo = correoNuevo;
            }

            if (!string.IsNullOrWhiteSpace(usuarioEncriptado))
            {
                if (usuarioEncriptado != usuarioExistente.UsuarioCifrado &&
                    _repositorio.ExisteUsuarioCifrado(usuarioEncriptado))
                {
                    return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
                }
                usuarioExistente.UsuarioCifrado = usuarioEncriptado;
            }

            if (!string.IsNullOrWhiteSpace(contrasenaEncriptada))
            {
                if (!CryptoHelper.TryDecrypt(contrasenaEncriptada, out var contrasenaPlana) ||
                    !UsuarioValidator.EsContrasenaValida(contrasenaPlana))
                {
                    return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
                }
                usuarioExistente.ContrasenaCifrada = CryptoHelper.Encrypt(contrasenaPlana);
            }

            usuarioExistente.FechaActualizacion = DateTime.UtcNow;

            try
            {
                _repositorio.Actualizar(usuarioExistente);
            }
            catch (MongoDB.Driver.MongoWriteException)
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
            }

            return ResultadoOperacion.Ok();
        }

        public ResultadoOperacion CambiarEstadoUsuario(string identificacion, string estado)
        {
            if (!UsuarioValidator.EsIdentificacionValida(identificacion) ||
                !UsuarioValidator.EsEstadoValido(estado))
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOEstadoIncorrecto);
            }

            var usuarioExistente = _repositorio.ObtenerPorIdentificacion(identificacion);
            if (usuarioExistente == null)
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOEstadoIncorrecto);

            usuarioExistente.Estado = estado;
            usuarioExistente.FechaActualizacion = DateTime.UtcNow;
            _repositorio.Actualizar(usuarioExistente);
            return ResultadoOperacion.Ok();
        }
    }
}