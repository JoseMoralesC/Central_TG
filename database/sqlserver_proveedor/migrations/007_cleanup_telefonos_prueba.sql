USE CentralProveedor;
GO

DECLARE @telefonos TABLE (numero_telefono NVARCHAR(30) PRIMARY KEY);

INSERT INTO @telefonos (numero_telefono)
VALUES ('22223333'), ('66774422');

DELETE ms
FROM dbo.movimientos_saldo ms
JOIN dbo.servicios s ON s.servicio_id = ms.servicio_id
JOIN @telefonos t ON t.numero_telefono = s.numero_telefono;

DELETE lp
FROM dbo.llamadas_proveedor lp
JOIN dbo.servicios s ON s.servicio_id = lp.servicio_id
JOIN @telefonos t ON t.numero_telefono = s.numero_telefono;

DELETE b
FROM dbo.bitacora_proveedor b
JOIN dbo.servicios s ON s.servicio_id = b.servicio_id
JOIN @telefonos t ON t.numero_telefono = s.numero_telefono;

DELETE sa
FROM dbo.saldos sa
JOIN dbo.servicios s ON s.servicio_id = sa.servicio_id
JOIN @telefonos t ON t.numero_telefono = s.numero_telefono;

DECLARE @clientes TABLE (cliente_id INT PRIMARY KEY);

INSERT INTO @clientes (cliente_id)
SELECT DISTINCT s.cliente_id
FROM dbo.servicios s
JOIN @telefonos t ON t.numero_telefono = s.numero_telefono;

DELETE s
FROM dbo.servicios s
JOIN @telefonos t ON t.numero_telefono = s.numero_telefono;

DELETE c
FROM dbo.clientes c
JOIN @clientes target ON target.cliente_id = c.cliente_id
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.servicios s
    WHERE s.cliente_id = c.cliente_id
);
GO
