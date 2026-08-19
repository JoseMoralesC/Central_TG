using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WebAdministrativo.Services
{
    public static class ProveedorCryptoHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ClaveSecreta1234");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("VectorInicio1234");
        private static readonly Encoding Utf8SinBom = new UTF8Encoding(false);

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return string.Empty;
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
                        sw.Write(plainText.Trim());
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherTextBase64)
        {
            if (string.IsNullOrWhiteSpace(cipherTextBase64))
            {
                return string.Empty;
            }

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64.Trim());

                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var ms = new MemoryStream(cipherBytes))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs, Utf8SinBom))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DecryptOrSummary(string cipherTextBase64)
        {
            string plainText = Decrypt(cipherTextBase64);

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                return plainText;
            }

            if (string.IsNullOrWhiteSpace(cipherTextBase64))
            {
                return string.Empty;
            }

            string value = cipherTextBase64.Trim();
            if (value.StartsWith("ENC_IMEI_", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("ENC_SIM_", StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            return value.Length <= 12
                ? value
                : value.Substring(0, 8) + "...";
        }
    }
}
