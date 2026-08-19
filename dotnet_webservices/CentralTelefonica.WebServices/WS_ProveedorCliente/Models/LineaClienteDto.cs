using System;

namespace WS_ProveedorCliente.Models
{
    public class LineaClienteDto
    {
        public string NumeroTelefono { get; set; }

        public string TipoServicio { get; set; }

        public decimal Saldo { get; set; }

        public decimal FacturaPendiente { get; set; }

        public DateTime? FacturaFechaMaximaPago { get; set; }

        public bool Activo { get; set; }

        public string IdentificadorTelefono { get; set; }

        public string IdentificadorTarjeta { get; set; }

        public string IdentificacionDuenoCifrada { get; set; }
    }
}