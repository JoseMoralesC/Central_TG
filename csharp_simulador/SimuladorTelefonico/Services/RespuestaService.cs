using System;
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

        public bool EsRespuestaExitosa(string respuestaJson)
        {
            if (string.IsNullOrWhiteSpace(respuestaJson))
            {
                return false;
            }

            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuestaJson);

                JsonElement raiz = documento.RootElement;

                if (raiz.TryGetProperty("resultado", out JsonElement resultado))
                {
                    if (resultado.TryGetProperty("codigo", out JsonElement codigo))
                    {
                        return codigo.GetString()?.Equals("OK", StringComparison.OrdinalIgnoreCase) == true;
                    }

                    if (resultado.TryGetProperty("estado", out JsonElement estado))
                    {
                        return estado.GetString()?.Equals("APROBADO", StringComparison.OrdinalIgnoreCase) == true;
                    }
                }

                return respuestaJson.Contains("OK", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return respuestaJson.Contains("OK", StringComparison.OrdinalIgnoreCase);
            }
        }

        public string ObtenerMensaje(string respuestaJson)
        {
            if (string.IsNullOrWhiteSpace(respuestaJson))
            {
                return "No se recibió respuesta del Identificador.";
            }

            try
            {
                using JsonDocument documento = JsonDocument.Parse(respuestaJson);

                JsonElement raiz = documento.RootElement;

                if (raiz.TryGetProperty("resultado", out JsonElement resultado))
                {
                    if (resultado.TryGetProperty("mensaje", out JsonElement mensaje))
                    {
                        return mensaje.GetString() ?? respuestaJson;
                    }

                    if (resultado.TryGetProperty("estado", out JsonElement estado))
                    {
                        return estado.GetString() ?? respuestaJson;
                    }
                }

                return respuestaJson;
            }
            catch
            {
                return respuestaJson;
            }
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
    }
}