package java_proveedor.src.services;

import java_proveedor.src.config.Config_Loader;

import java.nio.charset.StandardCharsets;
import java.util.Base64;
import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

public class CryptoService {

    private static final String DEFAULT_AES_KEY = "ClaveSecreta1234";
    private static final String DEFAULT_AES_IV = "VectorInicio1234";

    public String desencriptarAes(String textoCifradoBase64) {
        if (textoCifradoBase64 == null || textoCifradoBase64.isBlank()) {
            return "";
        }

        try {
            byte[] key = obtenerKey();
            byte[] iv = obtenerIv();
            byte[] datosCifrados = Base64.getDecoder().decode(textoCifradoBase64.trim());

            Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
            cipher.init(
                Cipher.DECRYPT_MODE,
                new SecretKeySpec(key, "AES"),
                new IvParameterSpec(iv)
            );

            byte[] plano = cipher.doFinal(datosCifrados);
            return new String(plano, StandardCharsets.UTF_8);
        } catch (Exception e) {
            return "";
        }
    }

    public String desencriptarAesOOriginal(String valor) {
        String plano = desencriptarAes(valor);
        return plano.isBlank() ? valor : plano;
    }

    private byte[] obtenerKey() {
        String valor = valorConfig("aes.key", DEFAULT_AES_KEY);
        byte[] bytes = valor.getBytes(StandardCharsets.UTF_8);

        if (bytes.length == 16 || bytes.length == 24 || bytes.length == 32) {
            return bytes;
        }

        byte[] ajustada = new byte[32];
        System.arraycopy(bytes, 0, ajustada, 0, Math.min(bytes.length, ajustada.length));
        return ajustada;
    }

    private byte[] obtenerIv() {
        String valor = valorConfig("aes.iv", DEFAULT_AES_IV);
        byte[] bytes = valor.getBytes(StandardCharsets.UTF_8);

        if (bytes.length != 16) {
            throw new IllegalStateException("aes.iv debe tener exactamente 16 bytes.");
        }

        return bytes;
    }

    private String valorConfig(String llave, String defecto) {
        String valor = Config_Loader.get(llave);
        return valor == null || valor.isBlank() ? defecto : valor.trim();
    }
}
