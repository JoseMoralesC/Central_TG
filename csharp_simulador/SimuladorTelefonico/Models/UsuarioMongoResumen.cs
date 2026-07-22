namespace SimuladorTelefonico.Models
{
    public class UsuarioMongoResumen
    {
        public string Identificacion { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string PrimerApellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public string UsuarioCifrado { get; set; } = string.Empty;
    }
}
