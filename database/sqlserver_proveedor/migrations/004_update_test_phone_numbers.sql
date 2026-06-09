USE CentralProveedor;
GO

UPDATE dbo.servicios
SET numero_telefono = CASE servicio_id
    WHEN 1 THEN '61925420'
    WHEN 2 THEN '83447172'
    WHEN 3 THEN '60302145'
    WHEN 4 THEN '71268439'
    WHEN 5 THEN '64031857'
    WHEN 6 THEN '87029416'
    WHEN 7 THEN '66774422'
    ELSE numero_telefono
END
WHERE servicio_id IN (1, 2, 3, 4, 5, 6, 7);
GO
