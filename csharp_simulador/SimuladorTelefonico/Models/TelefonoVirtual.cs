namespace SimuladorTelefonico.Models
{
    public class TelefonoVirtual
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Maquina { get; set; } = string.Empty;
        public string IdentificadorDispositivo { get; set; } = string.Empty;
        public string IdentificadorTarjeta { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public string TipoLlamada { get; set; } = string.Empty;
    }
}