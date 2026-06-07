using System;
using System.Globalization;
using System.Text.Json;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.Services
{
    public class RespuestaService
    {
        private readonly JsonSerializerOptions _opcionesJson;

        public RespuestaService()
        {
            _opcionesJson = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public bool EsRespuestaExitosa(string respuesta)
        {
            string codigo = ObtenerCodigo(respuesta);

            return codigo.Equals("OK", StringComparison.OrdinalIgnoreCase)
                || codigo.Equals("APROBADO", StringComparison.OrdinalIgnoreCase)
                || codigo.Equals("AUTORIZADA", StringComparison.OrdinalIgnoreCase)
                || codigo.Equals("ACTIVA", StringComparison.OrdinalIgnoreCase);
        }

        public bool EsErrorConexion(string respuesta)
        {
            return !string.IsNullOrWhiteSpace(respuesta)
                && respuesta.TrimStart().StartsWith("ERROR:", StringComparison.OrdinalIgnoreCase);
        }

        public string ObtenerCodigo(string respuesta)
        {
            if (string.IsNullOrWhiteSpace(respuesta))
            {
                return "VACIA";
            }

            if (EsErrorConexion(respuesta))
            {
                return "ERROR";
            }

            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuesta);
                JsonElement raiz = documento.RootElement;

                if (raiz.TryGetProperty("status", out JsonElement status))
                {
                    return status.GetString() ?? "ERROR";
                }

                if (raiz.TryGetProperty("codigo", out JsonElement codigoRaiz))
                {
                    return codigoRaiz.GetString() ?? "ERROR";
                }

                if (raiz.TryGetProperty("resultado", out JsonElement resultado))
                {
                    if (resultado.TryGetProperty("codigo", out JsonElement codigo))
                    {
                        return codigo.GetString() ?? "ERROR";
                    }

                    if (resultado.TryGetProperty("estado", out JsonElement estado))
                    {
                        return estado.GetString() ?? "ERROR";
                    }
                }
            }
            catch
            {
                return respuesta.Contains("OK", StringComparison.OrdinalIgnoreCase)
                    ? "OK"
                    : "FORMATO_INVALIDO";
            }

            return "DESCONOCIDO";
        }

        public string ObtenerMensaje(string respuesta)
        {
            if (string.IsNullOrWhiteSpace(respuesta))
            {
                return "No se recibio respuesta del Identificador.";
            }

            if (EsErrorConexion(respuesta))
            {
                return respuesta;
            }

            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuesta);
                JsonElement raiz = documento.RootElement;

                if (raiz.TryGetProperty("message", out JsonElement message))
                {
                    return message.GetString() ?? respuesta;
                }

                if (raiz.TryGetProperty("mensaje", out JsonElement mensajeRaiz))
                {
                    return mensajeRaiz.GetString() ?? respuesta;
                }

                if (raiz.TryGetProperty("motivo", out JsonElement motivo))
                {
                    return $"Respuesta con motivo {motivo}";
                }

                if (raiz.TryGetProperty("resultado", out JsonElement resultado))
                {
                    if (resultado.TryGetProperty("mensaje", out JsonElement mensaje))
                    {
                        return mensaje.GetString() ?? respuesta;
                    }

                    if (resultado.TryGetProperty("estado", out JsonElement estado))
                    {
                        return estado.GetString() ?? respuesta;
                    }
                }

                return $"Codigo: {ObtenerCodigo(respuesta)}";
            }
            catch
            {
                return respuesta;
            }
        }

        public int ObtenerTiempoMaximoSegundos(string respuesta, int valorPorDefecto)
        {
            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuesta);
                JsonElement raiz = documento.RootElement;

                if (raiz.TryGetProperty("tiempo", out JsonElement tiempoSimple))
                {
                    return ConvertirTiempoAsegundos(tiempoSimple.GetString(), valorPorDefecto);
                }

                if (raiz.TryGetProperty("datos_llamada", out JsonElement datos)
                    && datos.TryGetProperty("tiempo_maximo_segundos", out JsonElement segundos)
                    && segundos.TryGetInt32(out int valor))
                {
                    return valor;
                }
            }
            catch
            {
                return valorPorDefecto;
            }

            return valorPorDefecto;
        }

        public string ObtenerIdLlamada(string respuesta, string valorPorDefecto)
        {
            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuesta);
                JsonElement raiz = documento.RootElement;

                if (raiz.TryGetProperty("datos_llamada", out JsonElement datos)
                    && datos.TryGetProperty("id_llamada", out JsonElement id))
                {
                    return id.GetString() ?? valorPorDefecto;
                }
            }
            catch
            {
                return valorPorDefecto;
            }

            return valorPorDefecto;
        }

        public string ObtenerSaldoTexto(string respuesta)
        {
            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuesta);
                JsonElement raiz = documento.RootElement;

                if (raiz.TryGetProperty("datos_saldo", out JsonElement datosSaldo))
                {
                    string montoTexto = "No disponible";
                    string monedaTexto = "CRC";

                    if (datosSaldo.TryGetProperty("saldo_disponible", out JsonElement saldoDisponible))
                    {
                        montoTexto = FormatearMonto(saldoDisponible);
                    }

                    if (datosSaldo.TryGetProperty("moneda", out JsonElement moneda))
                    {
                        monedaTexto = moneda.GetString() ?? monedaTexto;
                    }

                    return $"{montoTexto} {monedaTexto}";
                }

                if (raiz.TryGetProperty("saldo", out JsonElement saldo))
                {
                    if (saldo.ValueKind == JsonValueKind.Object
                        && saldo.TryGetProperty("monto_disponible", out JsonElement monto))
                    {
                        return FormatearMonto(monto);
                    }

                    return saldo.ToString();
                }
            }
            catch
            {
                return "No disponible";
            }

            return "No disponible";
        }

        public RespuestaLlamada? ConvertirRespuestaLlamada(string respuestaJson)
        {
            try
            {
                return JsonSerializer.Deserialize<RespuestaLlamada>(respuestaJson, _opcionesJson);
            }
            catch
            {
                return null;
            }
        }

        public RespuestaSaldo? ConvertirRespuestaSaldo(string respuestaJson)
        {
            try
            {
                return JsonSerializer.Deserialize<RespuestaSaldo>(respuestaJson, _opcionesJson);
            }
            catch
            {
                return null;
            }
        }

        private static int ConvertirTiempoAsegundos(string? tiempo, int valorPorDefecto)
        {
            if (string.IsNullOrWhiteSpace(tiempo) || tiempo.Length != 6)
            {
                return valorPorDefecto;
            }

            if (!int.TryParse(tiempo[..2], out int horas)
                || !int.TryParse(tiempo.Substring(2, 2), out int minutos)
                || !int.TryParse(tiempo.Substring(4, 2), out int segundos))
            {
                return valorPorDefecto;
            }

            return (horas * 3600) + (minutos * 60) + segundos;
        }

        private static string FormatearMonto(JsonElement monto)
        {
            if (monto.TryGetDecimal(out decimal decimalMonto))
            {
                return decimalMonto.ToString("N2", CultureInfo.InvariantCulture);
            }

            return monto.ToString();
        }
    }
}
