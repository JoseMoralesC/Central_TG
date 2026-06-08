using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.Services
{
    public static class TelefonoCatalogoService
    {
        private const string OrigenSeeds =
            "Datos de prueba (scripts SQL de database)";

        private const string OrigenMock =
            "Mock local pendiente de conexion real";

        public static string FuenteDatos { get; private set; } = OrigenSeeds;

        public static List<TelefonoVirtual> CargarTelefonos()
        {
            try
            {
                string? raizRepositorio = BuscarRaizRepositorio();

                if (raizRepositorio == null)
                {
                    FuenteDatos = OrigenMock;
                    return CrearCatalogoTemporal();
                }

                string seedProveedor = Path.Combine(
                    raizRepositorio,
                    "database",
                    "sqlserver_proveedor",
                    "seed",
                    "001_seed_proveedor.sql"
                );

                string seedIdentificador = Path.Combine(
                    raizRepositorio,
                    "database",
                    "mysql_identificador",
                    "seed",
                    "001_seed_identificador.sql"
                );

                if (!File.Exists(seedProveedor) || !File.Exists(seedIdentificador))
                {
                    FuenteDatos = OrigenMock;
                    return CrearCatalogoTemporal();
                }

                List<ClienteSeed> clientes = LeerClientes(File.ReadAllText(seedProveedor));
                List<ServicioSeed> servicios = LeerServicios(File.ReadAllText(seedProveedor));
                List<SaldoSeed> saldos = LeerSaldos(File.ReadAllText(seedProveedor));

                string contenidoIdentificador = File.ReadAllText(seedIdentificador);
                string proveedor = LeerProveedor(contenidoIdentificador);
                List<TelefonoIdentificadorSeed> telefonosIdentificador =
                    LeerTelefonosIdentificador(contenidoIdentificador);
                List<IdentificadorSeed> sims = LeerIdentificadores(
                    contenidoIdentificador,
                    "tarjetas_telefonicas",
                    "identificador_tarjeta_cifrado"
                );
                List<IdentificadorSeed> imeis = LeerIdentificadores(
                    contenidoIdentificador,
                    "dispositivos",
                    "identificador_dispositivo_cifrado"
                );

                FuenteDatos = OrigenSeeds;

                return servicios.Select((servicio, indice) =>
                {
                    ClienteSeed? cliente = clientes.FirstOrDefault(c =>
                        c.Id == servicio.ClienteId);
                    SaldoSeed? saldo = saldos.FirstOrDefault(s =>
                        s.ServicioId == servicio.Id);
                    TelefonoIdentificadorSeed? telefonoIdentificador =
                        telefonosIdentificador.FirstOrDefault(t =>
                            t.Id == servicio.Id);
                    IdentificadorSeed? sim = sims.FirstOrDefault(s =>
                        s.TelefonoId == servicio.Id);
                    IdentificadorSeed? imei = imeis.FirstOrDefault(i =>
                        i.TelefonoId == servicio.Id);

                    return new TelefonoVirtual
                    {
                        Id = $"TEL-{servicio.Id:000}",
                        Nombre = cliente?.Nombre ?? $"Telefono {servicio.Id}",
                        Cliente = cliente?.Nombre ?? "Cliente no disponible",
                        Numero = servicio.Numero,
                        Maquina = "Seed SQL",
                        Proveedor = proveedor,
                        IdentificadorTarjeta = LimpiarValorCifrado(sim?.Valor ?? string.Empty),
                        IdentificadorDispositivo = LimpiarValorCifrado(imei?.Valor ?? string.Empty),
                        TipoServicio = servicio.TipoServicio,
                        TipoLlamada = "NACIONAL",
                        SaldoDisponible = saldo?.SaldoDisponible ?? 0m,
                        Activo = servicio.Activo
                            && (cliente?.Activo ?? true)
                            && (telefonoIdentificador?.Activo ?? true),
                        OrigenDatos = FuenteDatos
                    };
                }).ToList();
            }
            catch
            {
                FuenteDatos = OrigenMock;
                return CrearCatalogoTemporal();
            }
        }

        private static string? BuscarRaizRepositorio()
        {
            DirectoryInfo? directorio = new(AppDomain.CurrentDomain.BaseDirectory);

            while (directorio != null)
            {
                string rutaDatabase = Path.Combine(directorio.FullName, "database");
                string rutaCsharp = Path.Combine(directorio.FullName, "csharp_simulador");

                if (Directory.Exists(rutaDatabase) && Directory.Exists(rutaCsharp))
                {
                    return directorio.FullName;
                }

                directorio = directorio.Parent;
            }

            return null;
        }

        private static List<ClienteSeed> LeerClientes(string sql)
        {
            return LeerTuplas(sql, "clientes")
                .Select((valores, indice) => new ClienteSeed(
                    indice + 1,
                    valores.ElementAtOrDefault(0) ?? "Cliente no disponible",
                    EsVerdadero(valores.ElementAtOrDefault(3))
                ))
                .ToList();
        }

        private static List<ServicioSeed> LeerServicios(string sql)
        {
            return LeerTuplas(sql, "servicios")
                .Select((valores, indice) => new ServicioSeed(
                    indice + 1,
                    LeerEntero(valores.ElementAtOrDefault(0), indice + 1),
                    valores.ElementAtOrDefault(1) ?? string.Empty,
                    valores.ElementAtOrDefault(2) ?? string.Empty,
                    EsVerdadero(valores.ElementAtOrDefault(3))
                ))
                .ToList();
        }

        private static List<SaldoSeed> LeerSaldos(string sql)
        {
            return LeerTuplas(sql, "saldos")
                .Select((valores, indice) => new SaldoSeed(
                    LeerEntero(valores.ElementAtOrDefault(0), indice + 1),
                    LeerDecimal(valores.ElementAtOrDefault(1))
                ))
                .ToList();
        }

        private static string LeerProveedor(string sql)
        {
            return LeerTuplas(sql, "proveedores")
                .FirstOrDefault()?
                .ElementAtOrDefault(0)?
                .Replace("TelefÃ³nico", "Telefonico", StringComparison.Ordinal)
                ?? "Proveedor no disponible";
        }

        private static List<TelefonoIdentificadorSeed> LeerTelefonosIdentificador(string sql)
        {
            return LeerTuplas(sql, "telefonos")
                .Select((valores, indice) => new TelefonoIdentificadorSeed(
                    indice + 1,
                    LimpiarValorCifrado(valores.ElementAtOrDefault(0) ?? string.Empty),
                    EsVerdadero(valores.ElementAtOrDefault(3))
                ))
                .ToList();
        }

        private static List<IdentificadorSeed> LeerIdentificadores(
            string sql,
            string tabla,
            string columna
        )
        {
            _ = columna;

            return LeerTuplas(sql, tabla)
                .Select(valores => new IdentificadorSeed(
                    LeerEntero(valores.ElementAtOrDefault(0), 0),
                    valores.ElementAtOrDefault(1) ?? string.Empty
                ))
                .ToList();
        }

        private static List<List<string>> LeerTuplas(string sql, string tabla)
        {
            Match bloque = Regex.Match(
                sql,
                @$"INSERT\s+INTO\s+{tabla}\s*\([\s\S]*?\)\s*VALUES\s*(?<values>[\s\S]*?);",
                RegexOptions.IgnoreCase
            );

            if (!bloque.Success)
            {
                return new List<List<string>>();
            }

            return Regex.Matches(bloque.Groups["values"].Value, @"\((?<tuple>[^()]*)\)")
                .Select(match => SepararValores(match.Groups["tuple"].Value))
                .ToList();
        }

        private static List<string> SepararValores(string tupla)
        {
            List<string> valores = new();
            bool dentroTexto = false;
            int inicio = 0;

            for (int i = 0; i < tupla.Length; i++)
            {
                if (tupla[i] == '\'')
                {
                    dentroTexto = !dentroTexto;
                }

                if (tupla[i] == ',' && !dentroTexto)
                {
                    valores.Add(LimpiarToken(tupla[inicio..i]));
                    inicio = i + 1;
                }
            }

            valores.Add(LimpiarToken(tupla[inicio..]));

            return valores;
        }

        private static string LimpiarToken(string token)
        {
            string valor = token.Trim();

            if (valor.StartsWith("'") && valor.EndsWith("'") && valor.Length >= 2)
            {
                valor = valor[1..^1];
            }

            return valor.Trim();
        }

        private static string LimpiarValorCifrado(string valor)
        {
            return valor
                .Replace("ENC_SIM_", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("ENC_IMEI_", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("ENC_", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsVerdadero(string? valor)
        {
            return valor?.Trim().Equals("1", StringComparison.OrdinalIgnoreCase) == true
                || valor?.Trim().Equals("TRUE", StringComparison.OrdinalIgnoreCase) == true;
        }

        private static int LeerEntero(string? valor, int valorPorDefecto)
        {
            return int.TryParse(valor, out int numero)
                ? numero
                : valorPorDefecto;
        }

        private static decimal LeerDecimal(string? valor)
        {
            return decimal.TryParse(
                valor,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal numero
            )
                ? numero
                : 0m;
        }

        private static List<TelefonoVirtual> CrearCatalogoTemporal()
        {
            return new List<TelefonoVirtual>
            {
                new()
                {
                    Id = "MOCK-001",
                    Nombre = "Telefono de prueba",
                    Cliente = "Datos de prueba",
                    Numero = "00000000",
                    Proveedor = "Mock",
                    IdentificadorTarjeta = "SIM-MOCK",
                    IdentificadorDispositivo = "IMEI-MOCK",
                    TipoServicio = "PREPAGO",
                    TipoLlamada = "NACIONAL",
                    Activo = true,
                    OrigenDatos = OrigenMock
                }
            };
        }

        private record ClienteSeed(int Id, string Nombre, bool Activo);

        private record ServicioSeed(
            int Id,
            int ClienteId,
            string Numero,
            string TipoServicio,
            bool Activo
        );

        private record SaldoSeed(int ServicioId, decimal SaldoDisponible);

        private record TelefonoIdentificadorSeed(int Id, string Numero, bool Activo);

        private record IdentificadorSeed(int TelefonoId, string Valor);
    }
}
