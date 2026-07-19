using System.Runtime.Serialization;
using CentralTelefonica.WebServices.WS_Proveedor.Models;

[ServiceContract]
public interface IProveedorService
{
    [OperationContract]
    FacturacionResponse ObtenerFacturaPostpago(string identificacion, DateTime inicio, DateTime fin);
}
