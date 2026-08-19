USE CentralProveedor;
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
    SELECT 1 FROM dbo.clientes WHERE identificacion = '200300400'
)
BEGIN
    INSERT INTO dbo.clientes (nombre, identificacion, correo, activo)
    VALUES ('Cliente Persona Dos', '200300400', 'cliente.persona2@example.com', 1);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.servicios WHERE numero_telefono = '60606060'
)
BEGIN
    INSERT INTO dbo.servicios (
        cliente_id,
        numero_telefono,
        tipo_servicio,
        proveedor_codigo,
        activo
    )
    SELECT
        cliente_id,
        '60606060',
        'POSTPAGO',
        'KOLBI',
        1
    FROM dbo.clientes
    WHERE identificacion = '200300400';
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.tarifas WHERE tipo_llamada = 'NACIONAL'
)
BEGIN
    INSERT INTO dbo.tarifas (
        tipo_llamada,
        descripcion,
        costo_por_minuto,
        activa
    )
    VALUES (
        'NACIONAL',
        'Llamadas dentro del territorio nacional',
        10.00,
        1
    );
END;
GO

DECLARE @servicioId INT;
DECLARE @tarifaId INT;

SELECT @servicioId = servicio_id
FROM dbo.servicios
WHERE numero_telefono = '60606060';

SELECT @tarifaId = tarifa_id
FROM dbo.tarifas
WHERE tipo_llamada = 'NACIONAL';

IF @servicioId IS NOT NULL
   AND @tarifaId IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM dbo.llamadas_proveedor
       WHERE id_llamada = 'P2-ADM6-001'
   )
BEGIN
    INSERT INTO dbo.llamadas_proveedor (
        servicio_id,
        tarifa_id,
        telefono_destino,
        fecha_llamada,
        hora_llamada,
        costo,
        duracion,
        id_llamada,
        telefono_origen,
        fecha_inicio,
        fecha_fin,
        duracion_segundos,
        duracion_minutos,
        tipo_llamada,
        motivo_finalizacion,
        estado,
        moneda
    )
    VALUES (
        @servicioId,
        @tarifaId,
        '70000001',
        '2026-08-15',
        '09:30:00',
        120.00,
        '12',
        'P2-ADM6-001',
        '60606060',
        '2026-08-15T09:30:00',
        '2026-08-15T09:42:00',
        720,
        12,
        'NACIONAL',
        'NORMAL',
        'FINALIZADA',
        'CRC'
    );
END;

IF @servicioId IS NOT NULL
   AND @tarifaId IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM dbo.llamadas_proveedor
       WHERE id_llamada = 'P2-ADM6-002'
   )
BEGIN
    INSERT INTO dbo.llamadas_proveedor (
        servicio_id,
        tarifa_id,
        telefono_destino,
        fecha_llamada,
        hora_llamada,
        costo,
        duracion,
        id_llamada,
        telefono_origen,
        fecha_inicio,
        fecha_fin,
        duracion_segundos,
        duracion_minutos,
        tipo_llamada,
        motivo_finalizacion,
        estado,
        moneda
    )
    VALUES (
        @servicioId,
        @tarifaId,
        '70000002',
        '2026-08-16',
        '14:15:00',
        80.00,
        '8',
        'P2-ADM6-002',
        '60606060',
        '2026-08-16T14:15:00',
        '2026-08-16T14:23:00',
        480,
        8,
        'NACIONAL',
        'NORMAL',
        'FINALIZADA',
        'CRC'
    );
END;
GO

EXEC dbo.sp_CalcularFacturacionPostpago
    @fecha_calculo = '2026-08-16',
    @fecha_maxima_pago = '2026-08-31';
GO
