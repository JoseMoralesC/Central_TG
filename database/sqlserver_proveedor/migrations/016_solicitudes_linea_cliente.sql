IF OBJECT_ID('dbo.solicitudes_linea', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.solicitudes_linea
    (
        solicitud_id INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_solicitudes_linea PRIMARY KEY,
        servicio_id INT NOT NULL,
        numero_telefono VARCHAR(25) NOT NULL,
        tipo_servicio VARCHAR(20) NOT NULL,
        identificacion_cliente VARCHAR(50) NOT NULL,
        nombre_cliente NVARCHAR(160) NOT NULL,
        estado VARCHAR(20) NOT NULL
            CONSTRAINT DF_solicitudes_linea_estado DEFAULT ('PENDIENTE'),
        fecha_solicitud DATETIME2(0) NOT NULL
            CONSTRAINT DF_solicitudes_linea_fecha DEFAULT (SYSUTCDATETIME()),
        fecha_atencion DATETIME2(0) NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_solicitudes_linea_estado_fecha'
      AND object_id = OBJECT_ID('dbo.solicitudes_linea')
)
BEGIN
    CREATE INDEX IX_solicitudes_linea_estado_fecha
    ON dbo.solicitudes_linea (estado, fecha_solicitud DESC);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_solicitudes_linea_pendiente_cliente'
      AND object_id = OBJECT_ID('dbo.solicitudes_linea')
)
BEGIN
    CREATE UNIQUE INDEX UX_solicitudes_linea_pendiente_cliente
    ON dbo.solicitudes_linea (servicio_id, identificacion_cliente)
    WHERE estado = 'PENDIENTE';
END;
GO
