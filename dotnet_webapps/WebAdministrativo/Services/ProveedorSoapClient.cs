using System;
using System.Configuration;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WebAdministrativo.Services
{
    [ServiceContract]
    public interface IProveedorService
    {
        [OperationContract]
        RespuestaServicio CalcularFacturacion(CalcularFacturacionRequest solicitud);

        [OperationContract]
        UltimaFacturacionResponse ObtenerUltimaFacturacion();
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/WS_Proveedor.Models")]
    public class CalcularFacturacionRequest
    {
        [DataMember]
        public string FechaCalculo { get; set; }

        [DataMember]
        public string FechaMaximaPago { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/WS_Proveedor.Models")]
    public class RespuestaServicio
    {
        [DataMember(Order = 1)]
        public bool Resultado { get; set; }

        [DataMember(Order = 2)]
        public string Mensaje { get; set; }
    }

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/WS_Proveedor.Models")]
    public class UltimaFacturacionResponse
    {
        [DataMember(Order = 1)]
        public bool Resultado { get; set; }

        [DataMember(Order = 2)]
        public string Mensaje { get; set; }

        [DataMember(Order = 3)]
        public bool HayFacturacion { get; set; }

        [DataMember(Order = 4)]
        public string FechaCalculo { get; set; }

        [DataMember(Order = 5)]
        public string FechaMaximaPago { get; set; }

        [DataMember(Order = 6)]
        public int TotalLineas { get; set; }

        [DataMember(Order = 7)]
        public int TotalLlamadas { get; set; }

        [DataMember(Order = 8)]
        public decimal TotalFacturar { get; set; }

        [DataMember(Order = 9)]
        public string FechaRegistro { get; set; }
    }

    public class ProveedorSoapClient
    {
        public UltimaFacturacionResponse ObtenerUltimaFacturacion()
        {
            return Ejecutar(servicio => servicio.ObtenerUltimaFacturacion());
        }

        public RespuestaServicio CalcularFacturacion(DateTime fechaCalculo, DateTime fechaMaximaPago)
        {
            var solicitud = new CalcularFacturacionRequest
            {
                FechaCalculo = fechaCalculo.ToString("yyyy-MM-dd"),
                FechaMaximaPago = fechaMaximaPago.ToString("yyyy-MM-dd")
            };

            return Ejecutar(servicio => servicio.CalcularFacturacion(solicitud));
        }

        private static T Ejecutar<T>(Func<IProveedorService, T> accion)
        {
            var url = ConfigurationManager.AppSettings["WsProveedorUrl"];

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ConfigurationErrorsException("Falta WsProveedorUrl en Web.config");
            }

            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 262144,
                SendTimeout = TimeSpan.FromSeconds(20),
                ReceiveTimeout = TimeSpan.FromSeconds(20),
                OpenTimeout = TimeSpan.FromSeconds(10),
                CloseTimeout = TimeSpan.FromSeconds(10)
            };

            var endpoint = new EndpointAddress(url);
            var factory = new ChannelFactory<IProveedorService>(binding, endpoint);
            IClientChannel channel = null;

            try
            {
                var proxy = factory.CreateChannel();
                channel = (IClientChannel)proxy;
                var resultado = accion(proxy);
                channel.Close();
                factory.Close();
                return resultado;
            }
            catch
            {
                channel?.Abort();
                factory.Abort();
                throw;
            }
        }
    }
}
