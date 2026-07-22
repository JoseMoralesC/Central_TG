using Newtonsoft.Json;
using System;
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
                var trama = new
                {
                    tipo_transaccion = "CONSULTA_SALDO",
                    origen = "WEB",
                    telefono_origen = CryptoAES.Encriptar(request.NumeroTelefono)
                };

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



    }
}
