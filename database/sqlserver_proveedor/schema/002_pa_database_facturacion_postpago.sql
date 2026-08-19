CREATE OR ALTER PROCEDURE dbo.sp_CalcularFacturacionPostpago
    @fecha_calculo DATE,
    @fecha_maxima_pago DATE,
    @numero_telefono NVARCHAR(30) = NULL
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
        WHERE fecha_calculo = @fecha_calculo
          AND (
              NULLIF(LTRIM(RTRIM(@numero_telefono)), '') IS NULL
              OR servicio_id IN (
                  SELECT servicio_id
                  FROM dbo.servicios
                  WHERE numero_telefono = @numero_telefono
                     OR RIGHT(REPLACE(numero_telefono, '+', ''), 8) =
                        RIGHT(REPLACE(@numero_telefono, '+', ''), 8)
              )
          );

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
            AND lp.fecha_llamada >= @fecha_calculo
            AND lp.fecha_llamada <= @fecha_maxima_pago
            AND COALESCE(lp.estado, 'FINALIZADA') = 'FINALIZADA'
        WHERE UPPER(s.tipo_servicio) = 'POSTPAGO'
            AND s.activo = 1
            AND (
                NULLIF(LTRIM(RTRIM(@numero_telefono)), '') IS NULL
                OR s.numero_telefono = @numero_telefono
                OR RIGHT(REPLACE(s.numero_telefono, '+', ''), 8) =
                    RIGHT(REPLACE(@numero_telefono, '+', ''), 8)
            )
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
