using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebCliente.Services
{
    public static class CryptoHelper
    {
        private static readonly Encoding Utf8SinBom = new UTF8Encoding(false);

        private static readonly byte[] Key = Convert.FromBase64String(
            ConfigurationManager.AppSettings["AesKeyBase64"]
            ?? throw new ConfigurationErrorsException("Falta AesKeyBase64 en Web.config"));

        private static readonly byte[] IV = Convert.FromBase64String(
            ConfigurationManager.AppSettings["AesIvBase64"]
            ?? throw new ConfigurationErrorsException("Falta AesIvBase64 en Web.config"));

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs, Utf8SinBom))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }
}
