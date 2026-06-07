using System;
using System.Security.Cryptography;
using System.Text;
using SimuladorTelefonico.Config;

namespace SimuladorTelefonico.Services
{
    public class CryptoService
    {
        public string CifrarDatoSensible(string textoPlano)
        {
            if (!AppConfig.CifradoSensiblesActivo || string.IsNullOrWhiteSpace(textoPlano))
            {
                return textoPlano;
            }

            byte[] key = ObtenerBytesAes(AppConfig.AesKey, 32, "AES_KEY");
            byte[] iv = ObtenerBytesAes(AppConfig.AesIv, 16, "AES_IV");

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] plano = Encoding.UTF8.GetBytes(textoPlano);
            byte[] cifrado = encryptor.TransformFinalBlock(plano, 0, plano.Length);

            return Convert.ToBase64String(cifrado);
        }

        private static byte[] ObtenerBytesAes(string valor, int longitudDestino, string nombre)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(valor);

            if (bytes.Length == 16 || bytes.Length == 24 || bytes.Length == 32)
            {
                return bytes;
            }

            if (nombre == "AES_IV")
            {
                throw new InvalidOperationException(
                    "AES_IV debe tener exactamente 16 bytes en UTF-8."
                );
            }

            byte[] ajustado = new byte[longitudDestino];
            int copiar = Math.Min(bytes.Length, ajustado.Length);
            Array.Copy(bytes, ajustado, copiar);

            return ajustado;
        }
    }
}
