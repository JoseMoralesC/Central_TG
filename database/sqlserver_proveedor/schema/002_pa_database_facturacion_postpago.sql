CREATE OR ALTER PROCEDURE dbo.sp_CalcularFacturacionPostpago
    @fecha_calculo DATE,
    @fecha_maxima_pago DATE
AS
BEGIN
    SET NOCOUNT ON;

    IF @fecha_calculo IS NULL OR @fecha_maxima_pago IS NULL
        THROW 51000, 'Fechas obligatorias para calcular facturacion postpago.', 1;

    IF @fecha_maxima_pago < @fecha_calculo
        THROW 51001, 'La fecha maxima de pago no puede ser anterior a la fecha de calculo.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM dbo.facturacion_postpago
        WHERE fecha_calculo = @fecha_calculo;

        INSERT INTO dbo.facturacion_postpago (
            servicio_id,
            fecha_calculo,
            fecha_maxima_pago,
            total_llamadas,
            total_facturar
        )
        SELECT
            s.servicio_id,
            @fecha_calculo,
            @fecha_maxima_pago,
            COUNT(lp.llamada_id),
            ISNULL(SUM(lp.costo), 0.00)
        FROM dbo.servicios s
        LEFT JOIN dbo.llamadas_proveedor lp
            ON lp.servicio_id = s.servicio_id
            AND lp.fecha_llamada <= @fecha_calculo
            AND COALESCE(lp.estado, 'FINALIZADA') = 'FINALIZADA'
        WHERE UPPER(s.tipo_servicio) = 'POSTPAGO'
            AND s.activo = 1
        GROUP BY s.servicio_id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO
