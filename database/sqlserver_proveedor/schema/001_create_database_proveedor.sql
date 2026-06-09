CREATE DATABASE CentralProveedor;
GO

USE CentralProveedor;
GO

CREATE TABLE clientes (
    cliente_id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(120) NOT NULL,
    identificacion NVARCHAR(50) NOT NULL UNIQUE,
    correo NVARCHAR(120) NULL,
    activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE servicios (
    servicio_id INT IDENTITY(1,1) PRIMARY KEY,
    cliente_id INT NOT NULL,
    numero_telefono NVARCHAR(30) NOT NULL UNIQUE,
    tipo_servicio NVARCHAR(20) NOT NULL,
    proveedor_codigo NVARCHAR(20) NULL,
    activo BIT NOT NULL DEFAULT 1,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_servicios_clientes
        FOREIGN KEY (cliente_id)
        REFERENCES clientes(cliente_id)
);
GO

CREATE TABLE tarifas (
    tarifa_id INT IDENTITY(1,1) PRIMARY KEY,
    tipo_llamada NVARCHAR(30) NOT NULL UNIQUE,
    descripcion NVARCHAR(150) NULL,
    costo_por_minuto DECIMAL(10,2) NOT NULL,
    activa BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE saldos (
    saldo_id INT IDENTITY(1,1) PRIMARY KEY,
    servicio_id INT NOT NULL UNIQUE,
    saldo_disponible DECIMAL(10,2) NOT NULL DEFAULT 0,
    fecha_actualizacion DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_saldos_servicios
        FOREIGN KEY (servicio_id)
        REFERENCES servicios(servicio_id)
);
GO

CREATE TABLE llamadas_proveedor (
    llamada_id INT IDENTITY(1,1) PRIMARY KEY,
    servicio_id INT NOT NULL,
    tarifa_id INT NOT NULL,
    telefono_destino NVARCHAR(30) NOT NULL,
    fecha_llamada DATE NOT NULL,
    hora_llamada NVARCHAR(20) NOT NULL,
    costo DECIMAL(10,2) NOT NULL DEFAULT 0,
    duracion NVARCHAR(20) NULL,

    CONSTRAINT fk_llamadas_servicios
        FOREIGN KEY (servicio_id)
        REFERENCES servicios(servicio_id),

    CONSTRAINT fk_llamadas_tarifas
        FOREIGN KEY (tarifa_id)
        REFERENCES tarifas(tarifa_id)
);
GO

CREATE TABLE movimientos_saldo (
    movimiento_id INT IDENTITY(1,1) PRIMARY KEY,
    servicio_id INT NOT NULL,
    llamada_id INT NULL,
    tipo_movimiento NVARCHAR(30) NOT NULL,
    monto DECIMAL(10,2) NOT NULL,
    fecha_movimiento DATETIME NOT NULL DEFAULT GETDATE(),
    descripcion NVARCHAR(200) NULL,

    CONSTRAINT fk_movimientos_servicios
        FOREIGN KEY (servicio_id)
        REFERENCES servicios(servicio_id),

    CONSTRAINT fk_movimientos_llamadas
        FOREIGN KEY (llamada_id)
        REFERENCES llamadas_proveedor(llamada_id)
);
GO

CREATE TABLE bitacora_proveedor (
    bitacora_id INT IDENTITY(1,1) PRIMARY KEY,
    servicio_id INT NULL,
    tipo_transaccion NVARCHAR(50) NOT NULL,
    tipo_trama NVARCHAR(50) NOT NULL,
    contenido_json NVARCHAR(MAX) NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_bitacora_servicios
        FOREIGN KEY (servicio_id)
        REFERENCES servicios(servicio_id)
);
GO
