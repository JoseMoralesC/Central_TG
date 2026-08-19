USE CentralProveedor;
GO


SET NOCOUNT ON;

BEGIN TRANSACTION;

DECLARE @lineas_afectadas TABLE (
    servicio_id INT PRIMARY KEY
);

INSERT INTO @lineas_afectadas (servicio_id)
SELECT servicio_id
FROM dbo.servicios;

DECLARE @total_afectadas INT = (
    SELECT COUNT(1)
    FROM @lineas_afectadas
);

DECLARE @cliente_inventario_id INT;

SELECT @cliente_inventario_id = cliente_id
FROM dbo.clientes
WHERE identificacion = 'INV-DISPONIBLE';

IF @cliente_inventario_id IS NULL
BEGIN
    INSERT INTO dbo.clientes (nombre, identificacion, correo, activo)
    VALUES ('Inventario Disponible', 'INV-DISPONIBLE', 'inventario@central.test', 1);

    SET @cliente_inventario_id = CAST(SCOPE_IDENTITY() AS INT);
END;

UPDATE s
SET s.cliente_id = @cliente_inventario_id,
    s.activo = 0,
    s.estado_linea = 'DISPONIBLE',
    s.identificacion_dueno_cifrada = NULL
FROM dbo.servicios s
INNER JOIN @lineas_afectadas a
    ON a.servicio_id = s.servicio_id;

UPDATE sd
SET sd.saldo_disponible = 0,
    sd.fecha_actualizacion = GETDATE()
FROM dbo.saldos sd
INNER JOIN @lineas_afectadas a
    ON a.servicio_id = sd.servicio_id;

IF OBJECT_ID('dbo.facturacion_postpago', 'U') IS NOT NULL
BEGIN
    UPDATE f
    SET f.total_facturar = 0
    FROM dbo.facturacion_postpago f
    INNER JOIN @lineas_afectadas a
        ON a.servicio_id = f.servicio_id
    WHERE f.total_facturar > 0;
END;

SELECT
    @total_afectadas AS lineas_convertidas_a_disponible,
    SUM(CASE WHEN activo = 0 AND UPPER(ISNULL(estado_linea, '')) = 'DISPONIBLE' THEN 1 ELSE 0 END) AS total_disponibles,
    SUM(CASE WHEN activo = 1 OR UPPER(ISNULL(estado_linea, '')) = 'ACTIVO' THEN 1 ELSE 0 END) AS total_activas_restantes
FROM dbo.servicios;

COMMIT TRANSACTION;
GO
