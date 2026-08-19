using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using WS_Proveedor_1;
using WS_Proveedor_1.Models;
using static WS_Proveedor_1.Models.Proovedor_telefono;

namespace WS_Proveedor_1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        
        [OperationContract]
        Registrar_linea RegistrarLinea(AgregarTelefono request);

        [OperationContract]
        ConsultarSaldoResponse ConsultarSaldo(ConsultarSaldoRequest request);

    }
     


}
