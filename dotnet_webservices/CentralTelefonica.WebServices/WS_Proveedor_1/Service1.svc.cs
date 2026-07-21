using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WS_Proveedor_1.Models;
using WS_Proveedor_1.Services;
using static WS_Proveedor_1.Models.Proovedor_telefono;

namespace WS_Proveedor_1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
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
                        telefono = request.NumeroTelefono,
                        identificador_dispositivo = request.IdentificadorTelefono,
                        identificador_tarjeta = request.IdentificadorTarjeta,
                        tipo = request.Tipo,
                        estado = request.Estado
                    };

                    string json = JsonConvert.SerializeObject(trama);

                    ProveedorTcpCliente cliente = new ProveedorTcpCliente();

                    string respuestaJson = cliente.Enviar(json);
                    Console.WriteLine(respuestaJson);

                    RespuestaProveedor respuesta =
                        JsonConvert.DeserializeObject<RespuestaProveedor>(respuestaJson);

                    if (respuesta?.resultado?.codigo == "OK")
                    {
                        return new Registrar_linea
                        {
                            Resultado = true,
                            Mensaje = "Exitoso"
                        };
                    }

                    return new Registrar_linea
                    {
                        Resultado = false,
                        Mensaje = "Problemas al incluir la información."
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

        public Registrar_linea RegistrarLinea(Registrar_linea request)
        {
            throw new NotImplementedException();
        }

        public ConsultarSaldoResponse ConsultarSaldo(ConsultarSaldoRequest request)
        {
            try
            {
                var trama = new
                {
                    tipo_transaccion = "CONSULTA_SALDO",
                    origen = "WEB",
                    telefono_origen = request.NumeroTelefono
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
