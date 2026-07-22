using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CentralTelefonica.WS_Autenticacion.Security
{
    public static class CryptoHelper
    {
        private static readonly byte[] Key = Convert.FromBase64String(
            ConfigurationManager.AppSettings["AesKeyBase64"]
            ?? throw new ConfigurationErrorsException("Falta AesKeyBase64 en Web.config"));

        private static readonly byte[] IV = Convert.FromBase64String(
            ConfigurationManager.AppSettings["AesIvBase64"]
            ?? throw new ConfigurationErrorsException("Falta AesIvBase64 en Web.config"));

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

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
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static bool TryDecrypt(string cipherTextBase64, out string plainText)
        {
            plainText = null;
            if (string.IsNullOrEmpty(cipherTextBase64)) return false;

            try
            {
                var cipherBytes = Convert.FromBase64String(cipherTextBase64);

                using (var aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var ms = new MemoryStream(cipherBytes))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {
                        plainText = sr.ReadToEnd();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}