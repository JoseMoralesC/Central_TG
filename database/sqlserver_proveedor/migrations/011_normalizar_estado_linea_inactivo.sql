USE CentralProveedor;
GO

IF COL_LENGTH('dbo.servicios', 'estado_linea') IS NOT NULL
BEGIN
    UPDATE dbo.servicios
    SET estado_linea = 'INACTIVO'
    WHERE UPPER(LTRIM(RTRIM(estado_linea))) = 'DISPONIBLE';

    UPDATE dbo.servicios
    SET estado_linea = CASE WHEN activo = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END
    WHERE estado_linea IS NULL OR LTRIM(RTRIM(estado_linea)) = '';
END;
GO
