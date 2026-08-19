using System.Security.Cryptography;
using System.Text;

namespace PortalCliente.Services;

public class ProveedorCryptoHelper
{
    private static readonly Encoding Utf8SinBom = new UTF8Encoding(false);
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public ProveedorCryptoHelper(IConfiguration configuration)
    {
        string key = configuration["ProveedorAes:Key"] ?? "ClaveSecreta1234";
        string iv = configuration["ProveedorAes:Iv"] ?? "VectorInicio1234";

        _key = AjustarBytes(key, 16);
        _iv = AjustarBytes(iv, 16);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return string.Empty;
        }

        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Utf8SinBom))
        {
            sw.Write(plainText.Trim());
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    private static byte[] AjustarBytes(string valor, int longitud)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(valor.Trim());
        if (bytes.Length == longitud)
        {
            return bytes;
        }

        var ajustado = new byte[longitud];
        Array.Copy(bytes, ajustado, Math.Min(bytes.Length, longitud));
        return ajustado;
    }
}
