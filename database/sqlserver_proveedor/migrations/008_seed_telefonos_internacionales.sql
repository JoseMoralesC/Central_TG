USE CentralProveedor;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.clientes WHERE identificacion = 'PA-61234567'
)
BEGIN
    INSERT INTO dbo.clientes (nombre, identificacion, correo, activo)
    VALUES ('Cliente Panama Prueba', 'PA-61234567', 'panama@test.com', 1);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.servicios WHERE numero_telefono = '+50761234567'
)
BEGIN
    INSERT INTO dbo.servicios (
        cliente_id,
        numero_telefono,
        tipo_servicio,
        proveedor_codigo,
        pais_id,
        activo
    )
    SELECT c.cliente_id, '+50761234567', 'PREPAGO', 'TIGO_PA', p.pais_id, 1
    FROM dbo.clientes c
    JOIN dbo.paises p ON p.nombre = 'Panama'
    WHERE c.identificacion = 'PA-61234567';

    INSERT INTO dbo.saldos (servicio_id, saldo_disponible)
    SELECT servicio_id, 1500.00
    FROM dbo.servicios
    WHERE numero_telefono = '+50761234567';
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.clientes WHERE identificacion = 'MX-5512345678'
)
BEGIN
    INSERT INTO dbo.clientes (nombre, identificacion, correo, activo)
    VALUES ('Cliente Mexico Prueba', 'MX-5512345678', 'mexico@test.com', 1);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.servicios WHERE numero_telefono = '+525512345678'
)
BEGIN
    INSERT INTO dbo.servicios (
        cliente_id,
        numero_telefono,
        tipo_servicio,
        proveedor_codigo,
        pais_id,
        activo
    )
    SELECT c.cliente_id, '+525512345678', 'PREPAGO', 'TELCEL_MX', p.pais_id, 1
    FROM dbo.clientes c
    JOIN dbo.paises p ON p.nombre = 'Mexico'
    WHERE c.identificacion = 'MX-5512345678';

    INSERT INTO dbo.saldos (servicio_id, saldo_disponible)
    SELECT servicio_id, 2500.00
    FROM dbo.servicios
    WHERE numero_telefono = '+525512345678';
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.clientes WHERE identificacion = 'FR-123456789'
)
BEGIN
    INSERT INTO dbo.clientes (nombre, identificacion, correo, activo)
    VALUES ('Cliente Francia Prueba', 'FR-123456789', 'francia@test.com', 1);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.servicios WHERE numero_telefono = '+33123456789'
)
BEGIN
    INSERT INTO dbo.servicios (
        cliente_id,
        numero_telefono,
        tipo_servicio,
        proveedor_codigo,
        pais_id,
        activo
    )
    SELECT c.cliente_id, '+33123456789', 'PREPAGO', 'ORANGE_FR', p.pais_id, 1
    FROM dbo.clientes c
    JOIN dbo.paises p ON p.nombre = 'Francia'
    WHERE c.identificacion = 'FR-123456789';

    INSERT INTO dbo.saldos (servicio_id, saldo_disponible)
    SELECT servicio_id, 6000.00
    FROM dbo.servicios
    WHERE numero_telefono = '+33123456789';
END;
GO
