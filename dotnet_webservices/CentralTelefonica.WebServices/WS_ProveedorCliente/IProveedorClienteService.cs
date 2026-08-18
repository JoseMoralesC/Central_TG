using System.ServiceModel;
using WS_ProveedorCliente.Models;

namespace WS_ProveedorCliente
{
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface IProveedorClienteService
    {
        [OperationContract]
        ConsultarLineasClienteResponse ConsultarLineasCliente(
            string identificacion
        );

        [OperationContract]
        RecargarSaldoResponse RecargarSaldo(
            string numeroTelefono,
            decimal monto
        );

        [OperationContract]
        PagarFacturaResponse PagarFactura(
            string numeroTelefono,
            decimal monto,
            string identificacion
        );
    }
}