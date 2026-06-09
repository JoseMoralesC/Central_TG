CREATE DATABASE IF NOT EXISTS central_identificador
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE central_identificador;

CREATE TABLE proveedores (
    proveedor_id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    codigo VARCHAR(20) NOT NULL UNIQUE,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE telefonos (
    telefono_id INT AUTO_INCREMENT PRIMARY KEY,
    numero_cifrado VARCHAR(255) NOT NULL,
    proveedor_id INT NOT NULL,
    tipo_servicio VARCHAR(20) NOT NULL,
    pais VARCHAR(80) NOT NULL DEFAULT 'Costa Rica',
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_registro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_telefonos_proveedores
        FOREIGN KEY (proveedor_id)
        REFERENCES proveedores(proveedor_id)
);

CREATE TABLE tarjetas_telefonicas (
    tarjeta_id INT AUTO_INCREMENT PRIMARY KEY,
    telefono_id INT NOT NULL,
    identificador_tarjeta_cifrado VARCHAR(255) NOT NULL,
    activa BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_tarjetas_telefonos
        FOREIGN KEY (telefono_id)
        REFERENCES telefonos(telefono_id)
);

CREATE TABLE dispositivos (
    dispositivo_id INT AUTO_INCREMENT PRIMARY KEY,
    telefono_id INT NOT NULL,
    identificador_dispositivo_cifrado VARCHAR(255) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT fk_dispositivos_telefonos
        FOREIGN KEY (telefono_id)
        REFERENCES telefonos(telefono_id)
);

CREATE TABLE llamadas_activas (
    llamada_activa_id INT AUTO_INCREMENT PRIMARY KEY,
    telefono_id INT NOT NULL,
    telefono_destino VARCHAR(30) NOT NULL,
    fecha_inicio DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_fin_maxima DATETIME NULL,
    tiempo_maximo VARCHAR(20) NULL,
    estado VARCHAR(30) NOT NULL DEFAULT 'ACTIVA',

    CONSTRAINT fk_llamadas_activas_telefonos
        FOREIGN KEY (telefono_id)
        REFERENCES telefonos(telefono_id)
);

CREATE TABLE historial_llamadas_identificador (
    historial_id INT AUTO_INCREMENT PRIMARY KEY,
    telefono_id INT NOT NULL,
    telefono_destino VARCHAR(30) NOT NULL,
    fecha_inicio DATETIME NOT NULL,
    fecha_fin DATETIME NULL,
    duracion VARCHAR(20) NULL,
    motivo_finalizacion VARCHAR(100) NULL,

    CONSTRAINT fk_historial_identificador_telefonos
        FOREIGN KEY (telefono_id)
        REFERENCES telefonos(telefono_id)
);

CREATE TABLE bitacora_identificador (
    bitacora_id INT AUTO_INCREMENT PRIMARY KEY,
    telefono_id INT NULL,
    tipo_transaccion VARCHAR(50) NOT NULL,
    tipo_trama VARCHAR(50) NOT NULL,
    contenido_json TEXT NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_bitacora_identificador_telefonos
        FOREIGN KEY (telefono_id)
        REFERENCES telefonos(telefono_id)
);
