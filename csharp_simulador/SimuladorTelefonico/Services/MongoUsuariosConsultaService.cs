using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimuladorTelefonico.Models;

namespace SimuladorTelefonico.Services
{
    public class MongoUsuariosConsultaService
    {
        public async Task<(bool Exitoso, string Mensaje, List<UsuarioMongoResumen> Usuarios)> ConsultarUsuariosAsync()
        {
            string? webConfig = AutenticacionCryptoService.BuscarWebConfigAutenticacion();
            if (string.IsNullOrWhiteSpace(webConfig))
            {
                return (false, "No se encontro Web.config de WS_Autenticacion.", new());
            }

            XDocument documento = XDocument.Load(webConfig);
            string connectionString = LeerConnectionString(documento);
            string databaseName = LeerAppSetting(documento, "MongoDatabaseName", "central_tg_mongo");

            string eval =
                "const docs = db.getSiblingDB('" + databaseName + "').usuarios.find({}, " +
                "{_id:0, identificacion:1, nombre:1, primerApellido:1, correo:1, estado:1, tipo:1, usuario:1})" +
                ".sort({identificacion:1}).limit(100).toArray(); print(JSON.stringify(docs));";

            ProcessStartInfo startInfo = new()
            {
                FileName = "mongosh",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add(connectionString);
            startInfo.ArgumentList.Add("--quiet");
            startInfo.ArgumentList.Add("--eval");
            startInfo.ArgumentList.Add(eval);

            try
            {
                using Process proceso = new() { StartInfo = startInfo };
                proceso.Start();
                string salida = await proceso.StandardOutput.ReadToEndAsync();
                string error = await proceso.StandardError.ReadToEndAsync();
                await proceso.WaitForExitAsync();

                if (proceso.ExitCode != 0)
                {
                    return (false, string.IsNullOrWhiteSpace(error)
                        ? "No se pudo consultar MongoDB."
                        : error.Trim(), new());
                }

                return (true, "Usuarios consultados desde MongoDB.", ParsearUsuarios(salida));
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception || ex is InvalidOperationException)
            {
                return (false, "No se encontro mongosh en PATH; se generan datos WCF, pero no se puede listar MongoDB desde esta pantalla.", new());
            }
        }

        private static List<UsuarioMongoResumen> ParsearUsuarios(string salida)
        {
            List<UsuarioMongoResumen> usuarios = new();
            string json = salida.Trim();
            if (string.IsNullOrWhiteSpace(json))
            {
                return usuarios;
            }

            using JsonDocument documento = JsonDocument.Parse(json);
            foreach (JsonElement item in documento.RootElement.EnumerateArray())
            {
                usuarios.Add(new UsuarioMongoResumen
                {
                    Identificacion = LeerTexto(item, "identificacion"),
                    Nombre = LeerTexto(item, "nombre"),
                    PrimerApellido = LeerTexto(item, "primerApellido"),
                    Correo = LeerTexto(item, "correo"),
                    Estado = LeerTexto(item, "estado"),
                    Tipo = item.TryGetProperty("tipo", out JsonElement tipo) && tipo.TryGetInt32(out int valor)
                        ? valor
                        : 0,
                    UsuarioCifrado = LeerTexto(item, "usuario")
                });
            }

            return usuarios;
        }

        private static string LeerTexto(JsonElement item, string propiedad)
        {
            return item.TryGetProperty(propiedad, out JsonElement valor)
                ? valor.ToString()
                : string.Empty;
        }

        private static string LeerConnectionString(XDocument documento)
        {
            string? valor = documento
                .Descendants("add")
                .FirstOrDefault(e => (string?)e.Attribute("name") == "MongoUsuarios")
                ?.Attribute("connectionString")
                ?.Value;

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new InvalidOperationException("Falta connectionString MongoUsuarios.");
            }

            return valor.Trim();
        }

        private static string LeerAppSetting(XDocument documento, string llave, string valorPorDefecto)
        {
            return documento
                .Descendants("add")
                .FirstOrDefault(e => (string?)e.Attribute("key") == llave)
                ?.Attribute("value")
                ?.Value
                ?.Trim()
                ?? valorPorDefecto;
        }
    }
}
