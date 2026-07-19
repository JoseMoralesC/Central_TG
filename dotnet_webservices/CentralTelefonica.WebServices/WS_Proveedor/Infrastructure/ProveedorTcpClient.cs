using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace WS_Proveedor.Infrastructure
{
    public class ProveedorTcpClient
    {
        private readonly ProveedorClientOptions _opciones;

        public ProveedorTcpClient(
            ProveedorClientOptions opciones)
        {
            _opciones = opciones ??
                throw new ArgumentNullException(nameof(opciones));
        }

        public string EnviarTrama(string tramaJson)
        {
            if (string.IsNullOrWhiteSpace(tramaJson))
            {
                throw new ArgumentException(
                    "La trama JSON no puede estar vacía.",
                    nameof(tramaJson));
            }

            if (_opciones.ModoSimulado)
            {
                return EjecutarModoSimulado(tramaJson);
            }

            return EnviarAlProveedorReal(tramaJson);
        }

        private string EnviarAlProveedorReal(string tramaJson)
        {
            using (var cliente = new TcpClient())
            {
                ConectarConTimeout(cliente);

                cliente.SendTimeout = _opciones.TimeoutMs;
                cliente.ReceiveTimeout = _opciones.TimeoutMs;

                using (NetworkStream flujo = cliente.GetStream())
                {
                    EnviarMensaje(flujo, tramaJson);

                    return RecibirRespuesta(flujo);
                }
            }
        }

        private void ConectarConTimeout(TcpClient cliente)
        {
            IAsyncResult resultado = cliente.BeginConnect(
                _opciones.Host,
                _opciones.Puerto,
                null,
                null);

            bool conectado = resultado.AsyncWaitHandle.WaitOne(
                TimeSpan.FromMilliseconds(_opciones.TimeoutMs));

            try
            {
                if (!conectado)
                {
                    throw new TimeoutException(
                        "El Proveedor no respondió dentro del tiempo permitido.");
                }

                cliente.EndConnect(resultado);
            }
            finally
            {
                resultado.AsyncWaitHandle.Close();
            }
        }

        private static void EnviarMensaje(
            NetworkStream flujo,
            string tramaJson)
        {
            /*
             * El salto de línea se utiliza como delimitador del mensaje.
             * Java deberá leer hasta encontrar '\n'.
             */
            string mensaje = tramaJson + "\n";

            byte[] datos = Encoding.UTF8.GetBytes(mensaje);

            flujo.Write(datos, 0, datos.Length);
            flujo.Flush();
        }

        private static string RecibirRespuesta(
            NetworkStream flujo)
        {
            using (var lector = new StreamReader(
                flujo,
                Encoding.UTF8,
                false,
                1024,
                true))
            {
                string respuesta = lector.ReadLine();

                if (string.IsNullOrWhiteSpace(respuesta))
                {
                    throw new IOException(
                        "El Proveedor devolvió una respuesta vacía.");
                }

                return respuesta.Trim();
            }
        }

        private static string EjecutarModoSimulado(
            string tramaJson)
        {
            /*
             * Stub temporal.
             * Permite probar WS_PROVEEDOR2 sin tener PROVEEDOR5 listo.
             */

            if (tramaJson.Contains(
                "\"tipo_transaccion\":\"PROVEEDOR5\""))
            {
                return "OK";
            }

            return "ERROR";
        }
    }
}