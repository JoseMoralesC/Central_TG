IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_NAME = 'facturacion_postpago'
)
BEGIN
    CREATE TABLE facturacion_postpago (
        facturacion_id INT IDENTITY(1,1) NOT NULL,
        servicio_id INT NOT NULL,
        fecha_calculo DATE NOT NULL,
        fecha_maxima_pago DATE NOT NULL,
        total_llamadas INT NOT NULL DEFAULT 0,
        total_facturar DECIMAL(10,2) NOT NULL DEFAULT 0.00,
        fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT pk_facturacion_postpago
        PRIMARY KEY (facturacion_id),
        CONSTRAINT fk_facturacion_servicio
        FOREIGN KEY (servicio_id) REFERENCES servicios(servicio_id)
    );

    CREATE INDEX ix_facturacion_servicio_fecha
        ON facturacion_postpago(servicio_id, fecha_calculo);
 END
 GO

IF NOT EXISTS (
    SELECT 1 FROM tipos_transaccion_proveedor
    WHERE tipo_transaccion = 'PROVEEDOR6'
 )
 BEGIN
    INSERT INTO tipos_transaccion_proveedor (tipo_transaccion, descripcion)
    VALUES ('PROVEEDOR6', 'Calculo de facturacion postpago por periodo');
 END
 GO;

 CREATE OR ALTER PROCEDURE sp_CalcularFacturacionPostpago
    @fecha_calculo DATE,
    @fecha_maxima_pago DATE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM facturacion_postpago
        WHERE fecha_calculo = @fecha_calculo;

        INSERT INTO facturacion_postpago (servicio_id, fecha_calculo, fecha_maxima_pago, total_llamadas, total_facturar)
        SELECT 
            s.servicio_id,
            @fecha_calculo,
            @fecha_maxima_pago,
            COUNT(lp.llamada_id),
            ISNULL(SUM(lp.costo),0.00)
            FROM servicios s
            LEFT JOIN llamadas_proveedor lp
                ON lp.servicio_id = s.servicio_id
                AND lp.fecha_llamada >= @fecha_calculo
                AND lp.estado = 'FINALIZADA'
            WHERE s.tipo_servicio = 'POSTPAGO'
                AND s.activo = 1
            GROUP BY s.servicio_id;

            cOMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        THROW;
    END CATCH
END;
GO;