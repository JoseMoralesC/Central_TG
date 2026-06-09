import argparse
import base64
import os
from dataclasses import dataclass

from Crypto.Cipher import AES
from Crypto.Util.Padding import pad


DEFAULT_AES_KEY = "ClaveSecreta1234"
DEFAULT_AES_IV = "VectorInicio1234"


@dataclass(frozen=True)
class RegistroIdentificador:
    telefono_id: int
    telefono: str
    sim: str
    dispositivo: str


REGISTROS = [
    RegistroIdentificador(1, "88889999", "1234567891234567891", "1234567891234567"),
    RegistroIdentificador(2, "88880000", "2222222222222222222", "2222222222222222"),
    RegistroIdentificador(3, "22223333", "3333333333333333333", "3333333333333333"),
    RegistroIdentificador(4, "77776666", "4444444444444444444", "4444444444444444"),
]


def _normalizar_key(valor: str) -> bytes:
    data = valor.encode("utf-8")

    if len(data) in (16, 24, 32):
        return data

    ajustada = bytearray(32)
    ajustada[: min(len(data), 32)] = data[:32]
    return bytes(ajustada)


def _normalizar_iv(valor: str) -> bytes:
    data = valor.encode("utf-8")

    if len(data) != 16:
        raise ValueError("AES_IV debe tener exactamente 16 bytes en UTF-8")

    return data


def cifrar(texto: str, key: bytes, iv: bytes) -> str:
    cipher = AES.new(key, AES.MODE_CBC, iv)
    cifrado = cipher.encrypt(pad(texto.encode("utf-8"), AES.block_size))
    return base64.b64encode(cifrado).decode("utf-8")


def generar_sql(key: bytes, iv: bytes) -> str:
    lineas = [
        "USE central_identificador;",
        "",
        "UPDATE telefonos",
        "SET numero_cifrado = CASE telefono_id",
    ]

    for registro in REGISTROS:
        lineas.append(
            f"    WHEN {registro.telefono_id} THEN '{cifrar(registro.telefono, key, iv)}'"
        )

    lineas.extend([
        "    ELSE numero_cifrado",
        "END",
        "WHERE telefono_id IN (1, 2, 3, 4);",
        "",
        "UPDATE tarjetas_telefonicas",
        "SET identificador_tarjeta_cifrado = CASE telefono_id",
    ])

    for registro in REGISTROS:
        lineas.append(
            f"    WHEN {registro.telefono_id} THEN '{cifrar(registro.sim, key, iv)}'"
        )

    lineas.extend([
        "    ELSE identificador_tarjeta_cifrado",
        "END",
        "WHERE telefono_id IN (1, 2, 3, 4);",
        "",
        "UPDATE dispositivos",
        "SET identificador_dispositivo_cifrado = CASE telefono_id",
    ])

    for registro in REGISTROS:
        lineas.append(
            f"    WHEN {registro.telefono_id} THEN '{cifrar(registro.dispositivo, key, iv)}'"
        )

    lineas.extend([
        "    ELSE identificador_dispositivo_cifrado",
        "END",
        "WHERE telefono_id IN (1, 2, 3, 4);",
        "",
    ])

    return "\n".join(lineas)


def main() -> None:
    parser = argparse.ArgumentParser(
        description="Genera SQL con valores AES compatibles entre C# y Python."
    )
    parser.add_argument(
        "--key",
        default=os.getenv("AES_KEY", DEFAULT_AES_KEY),
        help="Llave AES. Por defecto usa AES_KEY o ClaveSecreta1234.",
    )
    parser.add_argument(
        "--iv",
        default=os.getenv("AES_IV", DEFAULT_AES_IV),
        help="IV AES de 16 bytes. Por defecto usa AES_IV o VectorInicio1234.",
    )

    args = parser.parse_args()
    key = _normalizar_key(args.key)
    iv = _normalizar_iv(args.iv)

    print(generar_sql(key, iv))


if __name__ == "__main__":
    main()
