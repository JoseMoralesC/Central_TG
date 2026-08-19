using System;
using CentralTelefonica.WS_Autenticacion.Data;
using CentralTelefonica.WS_Autenticacion.Security;
using CentralTelefonica.WS_Autenticacion.Validators;
using WS_Autenticacion.Models;

namespace CentralTelefonica.WS_Autenticacion
{
    public class Service1 : IAutenticacionService
    {
        private const int TipoAdministrador = 1;
        private const int TipoCliente = 2;
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

            var usuario = _repositorio.ObtenerPorUsuarioCifradoYTipo(usuarioEncriptado, tipo);

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

        public ResultadoAutenticacion AutenticarUsuarioDetalle(string usuarioEncriptado, string contrasenaEncriptada, int tipo)
        {
            if (string.IsNullOrEmpty(usuarioEncriptado) || string.IsNullOrEmpty(contrasenaEncriptada))
                return ResultadoAutenticacion.Fallo(MensajeCredencialesIncorrectas);

            if (!CryptoHelper.TryDecrypt(contrasenaEncriptada, out var contrasenaPlana))
                return ResultadoAutenticacion.Fallo(MensajeCredencialesIncorrectas);

            var usuario = _repositorio.ObtenerPorUsuarioCifradoYTipo(usuarioEncriptado, tipo);

            if (usuario == null)
                return ResultadoAutenticacion.Fallo(MensajeCredencialesIncorrectas);

            if (usuario.Estado != "activo")
                return ResultadoAutenticacion.Fallo(MensajeCredencialesIncorrectas);

            if (usuario.Tipo != tipo)
                return ResultadoAutenticacion.Fallo(MensajeCredencialesIncorrectas);

            if (!CryptoHelper.TryDecrypt(usuario.ContrasenaCifrada, out var contrasenaBd) ||
                contrasenaBd != contrasenaPlana)
            {
                return ResultadoAutenticacion.Fallo(MensajeCredencialesIncorrectas);
            }

            return ResultadoAutenticacion.Ok(MapearUsuario(usuario));
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

            var estadoNormalizado = estado?.Trim().ToLowerInvariant();
            if (estadoNormalizado != "activo")
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (string.IsNullOrEmpty(usuarioEncriptado))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (!CryptoHelper.TryDecrypt(contrasenaEncriptada, out var contrasenaPlana))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (!UsuarioValidator.EsContrasenaValida(contrasenaPlana))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (_repositorio.ExisteIdentificacion(identificacion, tipo))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (_repositorio.ExisteCorreo(correoElectronico.Trim(), tipo))
                return ResultadoOperacion.Fallo(MensajeUsuarioExisteOIncorrecto);

            if (_repositorio.ExisteUsuarioCifrado(usuarioEncriptado, tipo))
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
                Estado = estadoNormalizado,
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
            if (!UsuarioValidator.EsIdentificacionValida(identificacion) ||
                !UsuarioValidator.EsNombreValido(nombre) ||
                !UsuarioValidator.EsNombreValido(primerApellido) ||
                !UsuarioValidator.EsCorreoValido(correoElectronico) ||
                string.IsNullOrWhiteSpace(usuarioEncriptado))
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
            }

            var usuarioExistente = _repositorio.ObtenerPorIdentificacionYTipo(identificacion, TipoAdministrador);
            if (usuarioExistente == null)
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);

            if (!string.IsNullOrWhiteSpace(segundoApellido) &&
                !UsuarioValidator.EsNombreValido(segundoApellido))
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
            }

            if (usuarioEncriptado != usuarioExistente.UsuarioCifrado)
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
            }

            var correoNuevo = correoElectronico.Trim();
            if (correoNuevo != usuarioExistente.Correo && _repositorio.ExisteCorreo(correoNuevo, TipoAdministrador))
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);

            var debeActualizarContrasena = !string.IsNullOrWhiteSpace(contrasenaEncriptada);
            string contrasenaPlana = null;

            if (debeActualizarContrasena &&
                (!CryptoHelper.TryDecrypt(contrasenaEncriptada, out contrasenaPlana) ||
                !UsuarioValidator.EsContrasenaValida(contrasenaPlana)))
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);
            }

            usuarioExistente.Nombre = nombre.Trim();
            usuarioExistente.PrimerApellido = primerApellido.Trim();
            usuarioExistente.SegundoApellido = string.IsNullOrWhiteSpace(segundoApellido) ? null : segundoApellido.Trim();
            usuarioExistente.Correo = correoNuevo;
            if (debeActualizarContrasena)
            {
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
            var estadoNormalizado = estado?.Trim().ToLowerInvariant();

            if (!UsuarioValidator.EsIdentificacionValida(identificacion) ||
                !UsuarioValidator.EsEstadoValido(estadoNormalizado))
            {
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOEstadoIncorrecto);
            }

            var usuarioExistente = _repositorio.ObtenerPorIdentificacionYTipo(identificacion, TipoAdministrador);
            if (usuarioExistente == null)
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOEstadoIncorrecto);

            usuarioExistente.Estado = estadoNormalizado;
            usuarioExistente.FechaActualizacion = DateTime.UtcNow;
            _repositorio.Actualizar(usuarioExistente);
            return ResultadoOperacion.Ok();
        }

        public ResultadoListadoUsuarios ListarUsuariosPorTipo(int tipo)
        {
            if (!UsuarioValidator.EsTipoValido(tipo))
                return ResultadoListadoUsuarios.Fallo("Tipo de usuario incorrecto.");

            var usuarios = _repositorio.ObtenerPorTipo(tipo);
            var respuesta = new System.Collections.Generic.List<UsuarioServicio>();

            foreach (var usuario in usuarios)
            {
                respuesta.Add(MapearUsuario(usuario));
            }

            return ResultadoListadoUsuarios.Ok(respuesta);
        }

        public ResultadoOperacion EliminarUsuario(string identificacion)
        {
            if (!UsuarioValidator.EsIdentificacionValida(identificacion))
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOEstadoIncorrecto);

            var eliminado = _repositorio.EliminarPorIdentificacionYTipo(identificacion, TipoAdministrador);

            return eliminado
                ? ResultadoOperacion.Ok("Borrado exitoso")
                : ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOEstadoIncorrecto);
        }

        public ResultadoOperacion RegistrarMetodoPagoCliente(
            string identificacion,
            string numeroTarjetaEncriptado,
            string nombreTarjetaEncriptado,
            string fechaVencimientoEncriptada,
            string codigoSeguridadEncriptado)
        {
            if (!UsuarioValidator.EsIdentificacionValida(identificacion))
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);

            var usuario = _repositorio.ObtenerPorIdentificacionYTipo(identificacion.Trim(), TipoCliente);
            if (usuario == null || usuario.Tipo != TipoCliente)
                return ResultadoOperacion.Fallo(MensajeUsuarioNoExisteOIncorrecto);

            if (!CryptoHelper.TryDecrypt(numeroTarjetaEncriptado, out var numeroTarjeta) ||
                !CryptoHelper.TryDecrypt(nombreTarjetaEncriptado, out var nombreTarjeta) ||
                !CryptoHelper.TryDecrypt(fechaVencimientoEncriptada, out var fechaVencimiento) ||
                !CryptoHelper.TryDecrypt(codigoSeguridadEncriptado, out var codigoSeguridad))
            {
                return ResultadoOperacion.Fallo("Datos bancarios incorrectos.");
            }

            if (!UsuarioValidator.EsNumeroTarjetaValido(numeroTarjeta) ||
                !UsuarioValidator.EsNombreValido(nombreTarjeta) ||
                !UsuarioValidator.EsFechaVencimientoValida(fechaVencimiento) ||
                !UsuarioValidator.EsCodigoSeguridadValido(codigoSeguridad))
            {
                return ResultadoOperacion.Fallo("Datos bancarios incorrectos.");
            }

            _repositorio.ActualizarMetodoPago(
                usuario.Identificacion,
                numeroTarjetaEncriptado,
                nombreTarjetaEncriptado,
                fechaVencimientoEncriptada,
                codigoSeguridadEncriptado);

            return ResultadoOperacion.Ok("Metodo de pago registrado.");
        }

        public MetodoPagoClienteServicio ObtenerMetodoPagoCliente(string identificacion)
        {
            if (!UsuarioValidator.EsIdentificacionValida(identificacion))
                return MetodoPagoClienteServicio.Fallo("Ingrese una identificacion valida.");

            var usuario = _repositorio.ObtenerPorIdentificacionYTipo(identificacion.Trim(), TipoCliente);
            if (usuario == null || usuario.Tipo != TipoCliente)
                return MetodoPagoClienteServicio.Fallo("Cliente no encontrado.");

            if (string.IsNullOrWhiteSpace(usuario.MetodoPagoNumeroTarjetaCifrado) ||
                string.IsNullOrWhiteSpace(usuario.MetodoPagoNombreTarjetaCifrado) ||
                string.IsNullOrWhiteSpace(usuario.MetodoPagoFechaVencimientoCifrada) ||
                string.IsNullOrWhiteSpace(usuario.MetodoPagoCodigoSeguridadCifrado))
            {
                return MetodoPagoClienteServicio.Fallo("El cliente no tiene metodo de pago registrado.");
            }

            if (!CryptoHelper.TryDecrypt(usuario.MetodoPagoNumeroTarjetaCifrado, out var numeroTarjeta) ||
                !CryptoHelper.TryDecrypt(usuario.MetodoPagoNombreTarjetaCifrado, out var nombreTarjeta) ||
                !CryptoHelper.TryDecrypt(usuario.MetodoPagoFechaVencimientoCifrada, out var fechaVencimiento) ||
                !CryptoHelper.TryDecrypt(usuario.MetodoPagoCodigoSeguridadCifrado, out var codigoSeguridad))
            {
                return MetodoPagoClienteServicio.Fallo("No se pudo leer el metodo de pago.");
            }

            return new MetodoPagoClienteServicio
            {
                Resultado = true,
                Mensaje = "Exitoso",
                NumeroTarjeta = numeroTarjeta,
                NombreTarjeta = nombreTarjeta,
                FechaVencimiento = fechaVencimiento,
                CodigoSeguridad = codigoSeguridad
            };
        }

        private static UsuarioServicio MapearUsuario(Usuario usuario)
        {
            return new UsuarioServicio
            {
                Identificacion = usuario.Identificacion,
                Nombre = usuario.Nombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                CorreoElectronico = usuario.Correo,
                UsuarioEncriptado = usuario.UsuarioCifrado,
                ContrasenaEncriptada = usuario.ContrasenaCifrada,
                Estado = usuario.Estado,
                Tipo = usuario.Tipo
            };
        }
    }
}
