using System;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.Services
{
    public static class PoliticaTarifaService
    {
        public static decimal CalcularCostoPorMinuto(
            TelefonoVirtual origen,
            string tipoLlamadaDestino,
            string tipoServicioDestino,
            string nacionalidadDestino,
            string paisDestino)
        {
            if (EsExtranjero(origen.Nacionalidad, origen.Pais) ||
                EsExtranjero(nacionalidadDestino, paisDestino) ||
                EsInternacional(tipoLlamadaDestino))
            {
                return 60.00m;
            }

            if (ServiciosMixtos(origen.TipoServicio, tipoServicioDestino))
            {
                return 30.00m;
            }

            return 10.00m;
        }

        private static bool ServiciosMixtos(string origen, string destino)
        {
            if (string.IsNullOrWhiteSpace(origen) || string.IsNullOrWhiteSpace(destino))
            {
                return false;
            }

            return !origen.Trim().Equals(destino.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsInternacional(string tipoLlamada) =>
            !string.IsNullOrWhiteSpace(tipoLlamada) &&
            !tipoLlamada.Trim().Equals("NACIONAL", StringComparison.OrdinalIgnoreCase);

        private static bool EsExtranjero(string nacionalidad, string pais)
        {
            if (!string.IsNullOrWhiteSpace(nacionalidad))
            {
                return !nacionalidad.Trim().Equals("NACIONAL", StringComparison.OrdinalIgnoreCase);
            }

            return !string.IsNullOrWhiteSpace(pais) &&
                !pais.Trim().Equals("Costa Rica", StringComparison.OrdinalIgnoreCase);
        }
    }
}
