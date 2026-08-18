USE CentralProveedor;
GO

/*
   014_seed_portal_cliente_test.sql
   Datos de prueba para el Portal del Cliente (CLIENTE4 - CLIENTE7).

   Cliente de prueba: 118880999
   identificacion_dueno_cifrada = AES-CBC('118880999') con la clave del WS_ProveedorCliente:
   'ClaveSecreta1234' / 'VectorInicio1234' => LCcw+3gIJ1EomYmFZmFclQ==

   Idempotente: puede ejecutarse varias veces.
*/

DECLARE @cliente_id INT;
DECLARE @cifrado NVARCHAR(255) = N'LCcw+3gIJ1EomYmFZmFclQ==';

-- 1) Asegurar el cliente de prueba
IF NOT EXISTS (SELECT 1 FROM dbo.clientes WHERE identificacion = '118880999')
BEGIN
    INSERT INTO dbo.clientes (nombre, identificacion, correo, activo)
    VALUES (N'Cliente Portal Test', '118880999', 'cliente7@test.com', 1);
END;

SELECT @cliente_id = cliente_id
FROM dbo.clientes
WHERE identificacion = '118880999';

-- 2) Asegurar lineas prepago con saldo
IF NOT EXISTS (SELECT 1 FROM dbo.servicios WHERE numero_telefono = '88112233')
BEGIN
    INSERT INTO dbo.servicios (cliente_id, numero_telefono, tipo_servicio, proveedor_codigo, activo, estado_linea, identificacion_dueno_cifrada)
    VALUES (@cliente_id, '88112233', 'PREPAGO', 'KOLBI', 1, 'ACTIVO', @cifrado);
END
ELSE
BEGIN
    UPDATE dbo.servicios
    SET cliente_id = @cliente_id,
        activo = 1,
        estado_linea = 'ACTIVO',
        identificacion_dueno_cifrada = @cifrado
    WHERE numero_telefono = '88112233';
END;

IF NOT EXISTS (SELECT 1 FROM dbo.servicios WHERE numero_telefono = '88334455')
BEGIN
    INSERT INTO dbo.servicios (cliente_id, numero_telefono, tipo_servicio, proveedor_codigo, activo, estado_linea, identificacion_dueno_cifrada)
    VALUES (@cliente_id, '88334455', 'PREPAGO', 'KOLBI', 1, 'ACTIVO', @cifrado);
END
ELSE
BEGIN
    UPDATE dbo.servicios
    SET cliente_id = @cliente_id,
        activo = 1,
        estado_linea = 'ACTIVO',
        identificacion_dueno_cifrada = @cifrado
    WHERE numero_telefono = '88334455';
END;

-- 3) Asegurar lineas postpago (una con factura pendiente y otra sin)
IF NOT EXISTS (SELECT 1 FROM dbo.servicios WHERE numero_telefono = '88776655')
BEGIN
    INSERT INTO dbo.servicios (cliente_id, numero_telefono, tipo_servicio, proveedor_codigo, activo, estado_linea, identificacion_dueno_cifrada)
    VALUES (@cliente_id, '88776655', 'POSTPAGO', 'LIBERTY', 1, 'ACTIVO', @cifrado);
END
ELSE
BEGIN
    UPDATE dbo.servicios
    SET cliente_id = @cliente_id,
        activo = 1,
        estado_linea = 'ACTIVO',
        identificacion_dueno_cifrada = @cifrado
    WHERE numero_telefono = '88776655';
END;

IF NOT EXISTS (SELECT 1 FROM dbo.servicios WHERE numero_telefono = '88990011')
BEGIN
    INSERT INTO dbo.servicios (cliente_id, numero_telefono, tipo_servicio, proveedor_codigo, activo, estado_linea, identificacion_dueno_cifrada)
    VALUES (@cliente_id, '88990011', 'POSTPAGO', 'KOLBI', 1, 'ACTIVO', @cifrado);
END
ELSE
BEGIN
    UPDATE dbo.servicios
    SET cliente_id = @cliente_id,
        activo = 1,
        estado_linea = 'ACTIVO',
        identificacion_dueno_cifrada = @cifrado
    WHERE numero_telefono = '88990011';
END;

-- 4) Saldos prepago
IF EXISTS (SELECT 1 FROM dbo.saldos s INNER JOIN dbo.servicios sv ON sv.servicio_id = s.servicio_id WHERE sv.numero_telefono = '88112233')
BEGIN
    UPDATE s
    SET s.saldo_disponible = 5000.00
    FROM dbo.saldos s
    INNER JOIN dbo.servicios sv ON sv.servicio_id = s.servicio_id
    WHERE sv.numero_telefono = '88112233';
END
ELSE
BEGIN
    INSERT INTO dbo.saldos (servicio_id, saldo_disponible)
    SELECT servicio_id, 5000.00 FROM dbo.servicios WHERE numero_telefono = '88112233';
END;

IF EXISTS (SELECT 1 FROM dbo.saldos s INNER JOIN dbo.servicios sv ON sv.servicio_id = s.servicio_id WHERE sv.numero_telefono = '88334455')
BEGIN
    UPDATE s
    SET s.saldo_disponible = 0.00
    FROM dbo.saldos s
    INNER JOIN dbo.servicios sv ON sv.servicio_id = s.servicio_id
    WHERE sv.numero_telefono = '88334455';
END
ELSE
BEGIN
    INSERT INTO dbo.saldos (servicio_id, saldo_disponible)
    SELECT servicio_id, 0.00 FROM dbo.servicios WHERE numero_telefono = '88334455';
END;

-- 5) Factura pendiente postpago (70.00 en la linea 88776655)
DELETE f
FROM dbo.facturacion_postpago f
INNER JOIN dbo.servicios sv ON sv.servicio_id = f.servicio_id
WHERE sv.numero_telefono = '88776655';

INSERT INTO dbo.facturacion_postpago (servicio_id, fecha_calculo, fecha_maxima_pago, total_llamadas, total_facturar)
SELECT servicio_id, '2026-08-01', '2026-08-31', 0, 70.00
FROM dbo.servicios
WHERE numero_telefono = '88776655';

-- La linea 88990011 (postpago) queda sin factura pendiente (consultara 0).
GO