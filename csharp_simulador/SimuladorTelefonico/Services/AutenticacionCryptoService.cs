using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace SimuladorTelefonico.Services
{
    public class AutenticacionCryptoService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AutenticacionCryptoService()
        {
            string webConfig = BuscarWebConfigAutenticacion()
                ?? throw new InvalidOperationException(
                    "No se encontro Web.config de WS_Autenticacion.");

            XDocument documento = XDocument.Load(webConfig);
            _key = Convert.FromBase64String(LeerAppSetting(documento, "AesKeyBase64"));
            _iv = Convert.FromBase64String(LeerAppSetting(documento, "AesIvBase64"));
        }

        public string Cifrar(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
            {
                return textoPlano;
            }

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] plano = Encoding.UTF8.GetBytes(textoPlano);
            byte[] cifrado = encryptor.TransformFinalBlock(plano, 0, plano.Length);

            return Convert.ToBase64String(cifrado);
        }

        private static string LeerAppSetting(XDocument documento, string llave)
        {
            string? valor = documento
                .Descendants("add")
                .FirstOrDefault(e => (string?)e.Attribute("key") == llave)
                ?.Attribute("value")
                ?.Value;

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new InvalidOperationException($"Falta {llave} en Web.config.");
            }

            return valor.Trim();
        }

        internal static string? BuscarWebConfigAutenticacion()
        {
            DirectoryInfo? directorio = new(AppDomain.CurrentDomain.BaseDirectory);

            while (directorio != null)
            {
                string candidato = Path.Combine(
                    directorio.FullName,
                    "dotnet_webservices",
                    "WS_Autenticacion",
                    "Web.config");

                if (File.Exists(candidato))
                {
                    return candidato;
                }

                directorio = directorio.Parent;
            }

            return null;
        }
    }
}
