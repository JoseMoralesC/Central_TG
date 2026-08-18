using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WS_Proveedor.Services
{
    public static class ProveedorCryptoService
    {
        private static readonly Encoding Utf8SinBom = new UTF8Encoding(false);

        public static string Encriptar(string textoPlano)
        {
            if (string.IsNullOrWhiteSpace(textoPlano))
            {
                return string.Empty;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = ObtenerBytes("ProveedorAesKey", "ClaveSecreta1234", 16);
                aes.IV = ObtenerBytes("ProveedorAesIv", "VectorInicio1234", 16);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var memoria = new MemoryStream())
                {
                    using (var crypto = new CryptoStream(memoria, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(crypto, Utf8SinBom))
                    {
                        writer.Write(textoPlano.Trim());
                    }

                    return Convert.ToBase64String(memoria.ToArray());
                }
            }
        }

        public static string Desencriptar(string textoBase64)
        {
            if (string.IsNullOrWhiteSpace(textoBase64))
            {
                return string.Empty;
            }

            try
            {
                byte[] datosCifrados = Convert.FromBase64String(textoBase64.Trim());

                using (var aes = Aes.Create())
                {
                    aes.Key = ObtenerBytes("ProveedorAesKey", "ClaveSecreta1234", 16);
                    aes.IV = ObtenerBytes("ProveedorAesIv", "VectorInicio1234", 16);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var memoria = new MemoryStream(datosCifrados))
                    using (var crypto = new CryptoStream(memoria, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(crypto, Utf8SinBom))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static byte[] ObtenerBytes(string llave, string defecto, int longitud)
        {
            string valor = ConfigurationManager.AppSettings[llave];
            byte[] bytes = Encoding.UTF8.GetBytes(
                string.IsNullOrWhiteSpace(valor) ? defecto : valor.Trim());

            if (bytes.Length == longitud)
            {
                return bytes;
            }

            var ajustado = new byte[longitud];
            Array.Copy(bytes, ajustado, Math.Min(bytes.Length, ajustado.Length));
            return ajustado;
        }
    }
}
