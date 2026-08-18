using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WS_ProveedorCliente.Services
{
    public static class CryptoAes
    {
        private static readonly byte[] Key =
            Encoding.UTF8.GetBytes("ClaveSecreta1234");

        private static readonly byte[] IV =
            Encoding.UTF8.GetBytes("VectorInicio1234");

        public static string Encriptar(string textoPlano)
        {
            if (string.IsNullOrWhiteSpace(textoPlano))
            {
                return string.Empty;
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(
                        ms,
                        encryptor,
                        CryptoStreamMode.Write))
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(textoPlano);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Desencriptar(string textoBase64)
        {
            if (string.IsNullOrWhiteSpace(textoBase64))
            {
                return string.Empty;
            }

            byte[] cipherBytes = Convert.FromBase64String(textoBase64);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor();

                using (MemoryStream ms = new MemoryStream(cipherBytes))
                using (CryptoStream cs = new CryptoStream(
                    ms,
                    decryptor,
                    CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}