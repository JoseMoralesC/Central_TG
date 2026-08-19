using System.Security.Cryptography;
using System.Text;

namespace PortalCliente.Services;

public class CryptoHelper
{
    private static readonly Encoding Utf8SinBom = new UTF8Encoding(false);
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public CryptoHelper(IConfiguration configuration)
    {
        string key = configuration["Aes:KeyBase64"]
            ?? "qk1U2sHck1c3G0lHY0uFVQxlhO8m0cRk8mJ4Z2m8m1E=";
        string iv = configuration["Aes:IvBase64"]
            ?? "MTIzNDU2Nzg5MDEyMzQ1Ng==";

        _key = Convert.FromBase64String(key);
        _iv = Convert.FromBase64String(iv);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return plainText;
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
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public bool TryDecrypt(string cipherTextBase64, out string plainText)
    {
        plainText = string.Empty;

        if (string.IsNullOrWhiteSpace(cipherTextBase64))
        {
            return false;
        }

        try
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64.Trim());

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Utf8SinBom);
            plainText = sr.ReadToEnd();
            return true;
        }
        catch
        {
            plainText = string.Empty;
            return false;
        }
    }
}
