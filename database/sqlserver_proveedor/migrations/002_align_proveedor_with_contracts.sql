USE CentralProveedor;
GO

IF COL_LENGTH('dbo.servicios', 'proveedor_codigo') IS NULL
BEGIN
    ALTER TABLE dbo.servicios ADD proveedor_codigo NVARCHAR(20) NULL;
END;
GO

UPDATE dbo.servicios
SET proveedor_codigo = COALESCE(proveedor_codigo, 'XYZ');
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'id_llamada') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD id_llamada NVARCHAR(80) NULL;
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'telefono_origen') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD telefono_origen NVARCHAR(30) NULL;
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'fecha_inicio') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD fecha_inicio DATETIME NULL;
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'fecha_fin') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD fecha_fin DATETIME NULL;
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'duracion_segundos') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD duracion_segundos INT NULL;
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'duracion_minutos') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD duracion_minutos INT NULL;
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'tipo_llamada') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD tipo_llamada NVARCHAR(30) NULL;
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'motivo_finalizacion') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD motivo_finalizacion NVARCHAR(100) NULL;
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'estado') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD estado NVARCHAR(30) NOT NULL
        CONSTRAINT df_llamadas_proveedor_estado DEFAULT 'FINALIZADA';
END;
GO

IF COL_LENGTH('dbo.llamadas_proveedor', 'moneda') IS NULL
BEGIN
    ALTER TABLE dbo.llamadas_proveedor ADD moneda NVARCHAR(10) NOT NULL
        CONSTRAINT df_llamadas_proveedor_moneda DEFAULT 'CRC';
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'ix_llamadas_proveedor_id_llamada'
      AND object_id = OBJECT_ID('dbo.llamadas_proveedor')
)
BEGIN
    CREATE INDEX ix_llamadas_proveedor_id_llamada
    ON dbo.llamadas_proveedor(id_llamada);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'ix_llamadas_proveedor_fecha'
      AND object_id = OBJECT_ID('dbo.llamadas_proveedor')
)
BEGIN
    CREATE INDEX ix_llamadas_proveedor_fecha
    ON dbo.llamadas_proveedor(fecha_llamada, hora_llamada);
END;
GO

IF COL_LENGTH('dbo.movimientos_saldo', 'saldo_anterior') IS NULL
BEGIN
    ALTER TABLE dbo.movimientos_saldo ADD saldo_anterior DECIMAL(10,2) NULL;
END;
GO

IF COL_LENGTH('dbo.movimientos_saldo', 'saldo_posterior') IS NULL
BEGIN
    ALTER TABLE dbo.movimientos_saldo ADD saldo_posterior DECIMAL(10,2) NULL;
END;
GO

IF COL_LENGTH('dbo.movimientos_saldo', 'moneda') IS NULL
BEGIN
    ALTER TABLE dbo.movimientos_saldo ADD moneda NVARCHAR(10) NOT NULL
        CONSTRAINT df_movimientos_saldo_moneda DEFAULT 'CRC';
END;
GO

IF COL_LENGTH('dbo.movimientos_saldo', 'estado') IS NULL
BEGIN
    ALTER TABLE dbo.movimientos_saldo ADD estado NVARCHAR(30) NOT NULL
        CONSTRAINT df_movimientos_saldo_estado DEFAULT 'APLICADO';
END;
GO

IF COL_LENGTH('dbo.movimientos_saldo', 'referencia_transaccion') IS NULL
BEGIN
    ALTER TABLE dbo.movimientos_saldo ADD referencia_transaccion NVARCHAR(80) NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'ix_movimientos_saldo_referencia'
      AND object_id = OBJECT_ID('dbo.movimientos_saldo')
)
BEGIN
    CREATE INDEX ix_movimientos_saldo_referencia
    ON dbo.movimientos_saldo(referencia_transaccion);
END;
GO

IF COL_LENGTH('dbo.bitacora_proveedor', 'direccion') IS NULL
BEGIN
    ALTER TABLE dbo.bitacora_proveedor ADD direccion NVARCHAR(20) NULL;
END;
GO

IF COL_LENGTH('dbo.bitacora_proveedor', 'resultado_codigo') IS NULL
BEGIN
    ALTER TABLE dbo.bitacora_proveedor ADD resultado_codigo NVARCHAR(30) NULL;
END;
GO

IF COL_LENGTH('dbo.bitacora_proveedor', 'id_llamada') IS NULL
BEGIN
    ALTER TABLE dbo.bitacora_proveedor ADD id_llamada NVARCHAR(80) NULL;
END;
GO

IF COL_LENGTH('dbo.bitacora_proveedor', 'telefono_origen') IS NULL
BEGIN
    ALTER TABLE dbo.bitacora_proveedor ADD telefono_origen NVARCHAR(30) NULL;
END;
GO

IF COL_LENGTH('dbo.bitacora_proveedor', 'telefono_destino') IS NULL
BEGIN
    ALTER TABLE dbo.bitacora_proveedor ADD telefono_destino NVARCHAR(30) NULL;
END;
GO

IF COL_LENGTH('dbo.bitacora_proveedor', 'correlacion_id') IS NULL
BEGIN
    ALTER TABLE dbo.bitacora_proveedor ADD correlacion_id NVARCHAR(80) NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'ix_bitacora_proveedor_fecha'
      AND object_id = OBJECT_ID('dbo.bitacora_proveedor')
)
BEGIN
    CREATE INDEX ix_bitacora_proveedor_fecha
    ON dbo.bitacora_proveedor(fecha_registro);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'ix_bitacora_proveedor_correlacion'
      AND object_id = OBJECT_ID('dbo.bitacora_proveedor')
)
BEGIN
    CREATE INDEX ix_bitacora_proveedor_correlacion
    ON dbo.bitacora_proveedor(correlacion_id);
END;
GO

IF OBJECT_ID('dbo.tipos_transaccion_proveedor', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tipos_transaccion_proveedor (
        tipo_transaccion NVARCHAR(50) NOT NULL PRIMARY KEY,
        descripcion NVARCHAR(200) NULL,
        activo BIT NOT NULL CONSTRAINT df_tipos_transaccion_proveedor_activo DEFAULT 1
    );
END;
GO

MERGE dbo.tipos_transaccion_proveedor AS target
USING (
    VALUES
        ('CONSULTA_PROVEEDOR', 'Solicitud recibida desde Python para validar proveedor', 1),
        ('RESPUESTA_PROVEEDOR', 'Respuesta emitida por Java hacia Python', 1),
        ('VERIFICAR_SALDO', 'Validacion de saldo y servicio para autorizar llamada', 1),
        ('CONSULTAR_SALDO', 'Consulta de saldo disponible', 1),
        ('REGISTRO_MOVIMIENTO', 'Registro de llamada y movimiento de saldo', 1),
        ('REBAJAR_SALDO', 'Rebajo de saldo prepago', 1),
        ('FINALIZAR_LLAMADA', 'Finalizacion y cobro de llamada', 1)
) AS source (tipo_transaccion, descripcion, activo)
ON target.tipo_transaccion = source.tipo_transaccion
WHEN MATCHED THEN
    UPDATE SET descripcion = source.descripcion, activo = source.activo
WHEN NOT MATCHED THEN
    INSERT (tipo_transaccion, descripcion, activo)
    VALUES (source.tipo_transaccion, source.descripcion, source.activo);
GO
