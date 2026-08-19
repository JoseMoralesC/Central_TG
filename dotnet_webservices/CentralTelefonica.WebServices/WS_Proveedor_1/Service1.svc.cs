using Newtonsoft.Json;
using System;
using System.Globalization;
using WS_Proveedor_1.Models;
using WS_Proveedor_1.Services;
using static WS_Proveedor_1.Models.Proovedor_telefono;

namespace WS_Proveedor_1
{
    public class Service1 : IService1
    {
        public Registrar_linea RegistrarLinea(AgregarTelefono request)
        {
            {
                try
                {
                    var trama = new
                    {
                        tipo_transaccion = "REGISTRAR_LINEA",
                        telefono = CryptoAES.Encriptar(request.NumeroTelefono),
                        identificador_dispositivo = CryptoAES.Encriptar(request.IdentificadorTelefono),
                        identificador_tarjeta = CryptoAES.Encriptar(request.IdentificadorTarjeta),
                        tipo = request.Tipo,
                        estado = request.Estado
                    };

                    string json = JsonConvert.SerializeObject(trama);

                    ProveedorTcpCliente cliente = new ProveedorTcpCliente();

                    string respuestaJson = cliente.Enviar(json);
                    Console.WriteLine(respuestaJson);

                    RespuestaProveedor respuesta =
                        JsonConvert.DeserializeObject<RespuestaProveedor>(respuestaJson);

                    if (string.Equals(respuesta?.resultado?.codigo, "OK", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Registrar_linea
                        {
                            Resultado = true,
                            Mensaje = respuesta.resultado.mensaje ?? "Exitoso"
                        };
                    }

                    return new Registrar_linea
                    {
                        Resultado = false,
                        Mensaje = respuesta?.resultado?.mensaje ?? "Problemas al incluir la información."
                    };
                }
                catch (Exception ex)
                {
                    return new Registrar_linea
                    {
                        Resultado = false,
                        Mensaje = ex.Message
                    };
                }
            }
        }


        public ConsultarSaldoResponse ConsultarSaldo(ConsultarSaldoRequest request)
        {
            try
            {
                string origen = string.IsNullOrWhiteSpace(request.Origen)
                    ? "WEB"
                    : request.Origen.Trim().ToUpperInvariant();

                string tipoTransaccion = string.IsNullOrWhiteSpace(request.TipoTransaccion)
                    ? "CONSULTA_SALDO"
                    : request.TipoTransaccion.Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(request.NumeroTelefono))
                {
                    return new ConsultarSaldoResponse
                    {
                        Resultado = false,
                        Mensaje = "Datos incompletos",
                        Saldo = 0
                    };
                }

                if (tipoTransaccion != "CONSULTA_SALDO")
                {
                    return new ConsultarSaldoResponse
                    {
                        Resultado = false,
                        Mensaje = "Tipo de transaccion invalido",
                        Saldo = 0
                    };
                }

                if (origen != "WEB" && origen != "TELEFONO")
                {
                    return new ConsultarSaldoResponse
                    {
                        Resultado = false,
                        Mensaje = "Origen invalido",
                        Saldo = 0
                    };
                }

                object trama;

                if (origen == "WEB")
                {
                    trama = new
                    {
                        tipo_transaccion = tipoTransaccion,
                        origen,
                        telefono_origen = CryptoAES.Encriptar(request.NumeroTelefono),
                        fecha_hora = DateTime.Now.ToString("s")
                    };
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(request.IdentificadorTelefono) ||
                        string.IsNullOrWhiteSpace(request.IdentificadorTarjeta))
                    {
                        return new ConsultarSaldoResponse
                        {
                            Resultado = false,
                            Mensaje = "Datos incompletos",
                            Saldo = 0
                        };
                    }

                    trama = new
                    {
                        tipo_transaccion = tipoTransaccion,
                        origen,
                        telefono_origen = CryptoAES.Encriptar(request.NumeroTelefono),
                        identificador_dispositivo = CryptoAES.Encriptar(request.IdentificadorTelefono),
                        identificador_tarjeta = CryptoAES.Encriptar(request.IdentificadorTarjeta),
                        ubicacion = new
                        {
                            pais = string.IsNullOrWhiteSpace(request.Pais) ? "Costa Rica" : request.Pais.Trim(),
                            provincia = string.IsNullOrWhiteSpace(request.Provincia) ? "San Jose" : request.Provincia.Trim(),
                            latitud = ResolverDecimalUbicacion(request.Latitud, 9.9281m),
                            longitud = ResolverDecimalUbicacion(request.Longitud, -84.0907m)
                        },
                        fecha_hora = DateTime.Now.ToString("s")
                    };
                }

                string json = JsonConvert.SerializeObject(trama);

                ProveedorTcpCliente cliente = new ProveedorTcpCliente();

                string respuestaJson = cliente.Enviar(json);

                dynamic respuesta = JsonConvert.DeserializeObject(respuestaJson);

                if ((string)respuesta.resultado.codigo == "OK")
                {
                    return new ConsultarSaldoResponse
                    {
                        Resultado = true,
                        Mensaje = "OK",
                        Saldo = (decimal)respuesta.datos_saldo.saldo_disponible
                    };
                }

                return new ConsultarSaldoResponse
                {
                    Resultado = false,
                    Mensaje = (string)respuesta.resultado.mensaje,
                    Saldo = 0
                };
            }
            catch (Exception ex)
            {
                return new ConsultarSaldoResponse
                {
                    Resultado = false,
                    Mensaje = ex.Message,
                    Saldo = 0
                };
            }
        }

        private static decimal ResolverDecimalUbicacion(string valor, decimal predeterminado)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return predeterminado;
            }

            string normalizado = valor.Trim().Replace(',', '.');

            if (decimal.TryParse(
                normalizado,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal resultado))
            {
                return resultado;
            }

            return predeterminado;
        }

    }
}
