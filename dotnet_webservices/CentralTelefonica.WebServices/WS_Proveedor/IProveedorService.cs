using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel;
using WS_Proveedor.Models;

namespace WS_Proveedor
{
    [ServiceContract]
    public interface IProveedorService
    {
        [OperationContract]
        RespuestaServicio ActivarDesactivarLinea(
            ActivarDesactivarLineaRequest solicitud
        );
    }
}
