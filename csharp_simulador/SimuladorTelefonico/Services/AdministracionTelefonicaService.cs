using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.Services
{
    public class AdministracionTelefonicaService
    {
        private readonly TramaService _tramaService = new();

        public async Task<List<TelefonoVirtual>> ConsultarCatalogoAsync()
        {
            var solicitud = new
            {
                tipo_transaccion = "CONSULTA_CATALOGO_TELEFONOS",
                origen = "CSHARP_SIMULADOR",
                fecha_hora = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            string respuesta = await _tramaService.EnviarTramaAsync(solicitud);
            return ParsearCatalogo(respuesta);
        }

        public async Task<(bool Exitoso, string Mensaje)> RecargarSaldoAsync(
            string numeroTelefono,
            decimal monto)
        {
            var solicitud = new
            {
                tipo_transaccion = "RECARGAR_SALDO",
                telefono = numeroTelefono,
                monto,
                moneda = "CRC",
                fecha_hora = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            string respuesta = await _tramaService.EnviarTramaAsync(solicitud);
            return LeerResultadoSimple(respuesta);
        }

        public async Task<(bool Exitoso, string Mensaje)> RegistrarTelefonoAsync(
            string numeroTelefono,
            string tipoServicio,
            string proveedorCodigo,
            string pais,
            decimal saldoInicial,
            string sim,
            string imei,
            bool activo)
        {
            var solicitud = new
            {
                tipo_transaccion = "REGISTRAR_TELEFONO",
                telefono = numeroTelefono,
                tipo_servicio = tipoServicio,
                proveedor_codigo = proveedorCodigo,
                pais,
                saldo_inicial = saldoInicial,
                sim,
                imei,
                activo,
                fecha_hora = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            string respuesta = await _tramaService.EnviarTramaAsync(solicitud);
            return LeerResultadoSimple(respuesta);
        }

        public async Task<(bool Exitoso, string Mensaje)> CambiarEstadoAsync(
            string telefonoId,
            string numeroTelefono,
            bool activo)
        {
            var solicitud = new
            {
                tipo_transaccion = "CAMBIAR_ESTADO_TELEFONO",
                telefono_id = telefonoId,
                telefono = numeroTelefono,
                activo,
                fecha_hora = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            string respuesta = await _tramaService.EnviarTramaAsync(solicitud);
            return LeerResultadoSimple(respuesta);
        }

        private static List<TelefonoVirtual> ParsearCatalogo(string respuesta)
        {
            List<TelefonoVirtual> telefonos = new();

            if (string.IsNullOrWhiteSpace(respuesta)
                || respuesta.TrimStart().StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase))
            {
                return telefonos;
            }

            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuesta);
                JsonElement raiz = documento.RootElement;

                if (!raiz.TryGetProperty("telefonos", out JsonElement items)
                    || items.ValueKind != JsonValueKind.Array)
                {
                    return telefonos;
                }

                foreach (JsonElement item in items.EnumerateArray())
                {
                    string numero = LeerTexto(item, "numero");
                    if (string.IsNullOrWhiteSpace(numero))
                    {
                        continue;
                    }

                    string pais = LeerTexto(item, "pais", "Costa Rica");
                    string nacionalidad = LeerTexto(
                        item,
                        "nacionalidad",
                        pais.Equals("Costa Rica", StringComparison.OrdinalIgnoreCase)
                            ? "NACIONAL"
                            : "EXTRANJERO");

                    telefonos.Add(new TelefonoVirtual
                    {
                        Id = LeerTexto(item, "id", $"TEL-{numero}"),
                        Numero = numero,
                        Nombre = LeerTexto(item, "nombre", $"Telefono {numero}"),
                        Cliente = LeerTexto(item, "cliente", $"Cliente {numero}"),
                        Proveedor = LeerTexto(item, "proveedor", "Proveedor no disponible"),
                        ProveedorCodigo = LeerTexto(item, "proveedor_codigo", ""),
                        Pais = pais,
                        Nacionalidad = nacionalidad,
                        IdentificadorTarjeta = LeerTexto(item, "sim", ""),
                        IdentificadorDispositivo = LeerTexto(item, "imei", ""),
                        TipoServicio = LeerTexto(item, "tipo_servicio", "PREPAGO"),
                        TipoLlamada = nacionalidad,
                        SaldoDisponible = LeerDecimal(item, "saldo"),
                        Activo = LeerBooleano(item, "activo", true),
                        OrigenDatos = "Socket C# -> Python -> Java"
                    });
                }
            }
            catch
            {
                return new List<TelefonoVirtual>();
            }

            return telefonos;
        }

        private static (bool Exitoso, string Mensaje) LeerResultadoSimple(string respuesta)
        {
            if (string.IsNullOrWhiteSpace(respuesta))
            {
                return (false, "No se recibio respuesta.");
            }

            if (respuesta.TrimStart().StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase))
            {
                return (false, respuesta);
            }

            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuesta);
                JsonElement raiz = documento.RootElement;
                JsonElement resultado = raiz.TryGetProperty("resultado", out JsonElement res)
                    ? res
                    : raiz;

                string codigo = LeerTexto(resultado, "codigo", LeerTexto(raiz, "status", "ERROR"));
                string mensaje = LeerTexto(resultado, "mensaje", LeerTexto(raiz, "mensaje", respuesta));

                return (codigo.Equals("OK", StringComparison.OrdinalIgnoreCase), mensaje);
            }
            catch
            {
                return (false, respuesta);
            }
        }

        private static string LeerTexto(JsonElement elemento, string propiedad, string valorPorDefecto = "")
        {
            if (elemento.TryGetProperty(propiedad, out JsonElement valor))
            {
                return valor.ValueKind == JsonValueKind.String
                    ? valor.GetString() ?? valorPorDefecto
                    : valor.ToString();
            }

            return valorPorDefecto;
        }

        private static decimal LeerDecimal(JsonElement elemento, string propiedad)
        {
            if (!elemento.TryGetProperty(propiedad, out JsonElement valor))
            {
                return 0m;
            }

            if (valor.ValueKind == JsonValueKind.Number && valor.TryGetDecimal(out decimal numero))
            {
                return numero;
            }

            return decimal.TryParse(
                valor.ToString(),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal texto)
                    ? texto
                    : 0m;
        }

        private static bool LeerBooleano(JsonElement elemento, string propiedad, bool valorPorDefecto)
        {
            if (!elemento.TryGetProperty(propiedad, out JsonElement valor))
            {
                return valorPorDefecto;
            }

            if (valor.ValueKind == JsonValueKind.True || valor.ValueKind == JsonValueKind.False)
            {
                return valor.GetBoolean();
            }

            return bool.TryParse(valor.ToString(), out bool resultado)
                ? resultado
                : valorPorDefecto;
        }
    }
}
