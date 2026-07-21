using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using WS_Proveedor.Contracts;
using WS_Proveedor.Models;

namespace WS_Proveedor.Services
{
 public static class TramaProveedor6Service
 {
    public static string ConstruirTrama(CalcularFacturacionRequest solicitud)
    {
        var trama = new TramaProveedor6
        {
            TipoTransaccion = "PROVEEDOR6",
            Accion = "CALCULAR_FACTURACION",
            FechaCalculo = solicitud.FechaCalculo,
            FechaMaximaPago = solicitud.FechaMaximaPago
        };

        var serializer = new DataContractJsonSerializer(typeof(TramaProveedor6));
        using (var ms = new MemoryStream())
        {
            serializer.WriteObject(ms, trama);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
 }
}