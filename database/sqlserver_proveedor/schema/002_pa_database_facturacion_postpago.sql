CREATE PROCEDURE sp_CalcularFacturacionPostpago
    @identificacion_cliente VARCHAR(50),
    @fecha_inicio DATETIME,
    @fecha_fin DATETIME
AS
BEGIN
    SELECT 
        SUM(DATEDIFF(MINUTE, fecha_inicio, fecha_fin) * tarifa_por_minuto) AS TotalFacturar
    FROM historial_llamadas
    WHERE identificacion_cliente = @identificacion_cliente
      AND fecha_inicio >= @fecha_inicio
      AND fecha_fin <= @fecha_fin
      AND estado = 'FINALIZADA';
END;
GO