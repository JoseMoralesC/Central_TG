using System.Data;
using System.Data.SqlClient;
using CentralTelefonica.WebServices.WS_Proveedor.Contracts;
using CentralTelefonica.WebServices.WS_Proveedor.Models;

public class ProveedorService : IProveedorService
{
    private readonly string _connectionString;

    public ProveedorService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServerConnection") ?? "Server=localhost;Database=proveedor;Integrated Security=True;TrustServerCertificate=True;";
    }

    public FacturacionResponse ObtenerFacturaPostpago(string identificacion, DateTime inicio, DateTime fin)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CalcularFacturacionPostpago", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@identificacion_cliente", identificacion);
            command.Parameters.AddWithValue("@fecha_inicio", inicio);
            command.Parameters.AddWithValue("@fecha_fin", fin);

            connection.Open();
            var result = command.ExecuteScalar();

            var total = result != DBNull.Value ? Convert.ToDecimal(result) : 0m;

            return new FacturacionResponse
            {
                Exito = true,
                Mensaje = "Facturación calculada correctamente.",
                TotalFacturar = total
            };
        }
        catch (Exception ex)
        {
            return new FacturacionResponse
            {
                Exito = false,
                Mensaje = $"Error al calcular la facturación: {ex.Message}",
                TotalFacturar = 0m
            };
        }
    }
}