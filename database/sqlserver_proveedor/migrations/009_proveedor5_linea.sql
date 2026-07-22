USE CentralProveedor;
GO

IF COL_LENGTH('dbo.servicios', 'identificador_telefono_cifrado') IS NULL
BEGIN
    ALTER TABLE dbo.servicios ADD identificador_telefono_cifrado NVARCHAR(255) NULL;
END;
GO

IF COL_LENGTH('dbo.servicios', 'identificador_tarjeta_cifrado') IS NULL
BEGIN
    ALTER TABLE dbo.servicios ADD identificador_tarjeta_cifrado NVARCHAR(255) NULL;
END;
GO

IF COL_LENGTH('dbo.servicios', 'identificacion_dueno_cifrada') IS NULL
BEGIN
    ALTER TABLE dbo.servicios ADD identificacion_dueno_cifrada NVARCHAR(255) NULL;
END;
GO

IF COL_LENGTH('dbo.servicios', 'estado_linea') IS NULL
BEGIN
    ALTER TABLE dbo.servicios ADD estado_linea NVARCHAR(30) NOT NULL
        CONSTRAINT df_servicios_estado_linea DEFAULT 'ACTIVO';
END;
GO

UPDATE dbo.servicios
SET estado_linea = CASE WHEN activo = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END
WHERE estado_linea IS NULL OR estado_linea = '';
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'ix_servicios_estado_linea'
      AND object_id = OBJECT_ID('dbo.servicios')
)
BEGIN
    CREATE INDEX ix_servicios_estado_linea
    ON dbo.servicios(estado_linea);
END;
GO

INSERT INTO dbo.tipos_transaccion_proveedor (tipo_transaccion, descripcion, activo)
SELECT tipo_transaccion, descripcion, activo
FROM (
    VALUES
        ('PROVEEDOR5', 'Activacion o desactivacion de linea vendida desde WS Proveedor', 1)
) AS source (tipo_transaccion, descripcion, activo)
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.tipos_transaccion_proveedor target
    WHERE target.tipo_transaccion = source.tipo_transaccion
);
GO
