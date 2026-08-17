USE CentralProveedor;
GO

IF COL_LENGTH('dbo.servicios', 'estado_linea') IS NOT NULL
BEGIN
    UPDATE dbo.servicios
    SET estado_linea = 'DISPONIBLE'
    WHERE UPPER(LTRIM(RTRIM(estado_linea))) = 'INACTIVO';

    UPDATE dbo.servicios
    SET estado_linea = CASE WHEN activo = 1 THEN 'ACTIVO' ELSE 'DISPONIBLE' END
    WHERE estado_linea IS NULL OR LTRIM(RTRIM(estado_linea)) = '';
END;
GO
