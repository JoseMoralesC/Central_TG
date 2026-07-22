IF OBJECT_ID('dbo.facturacion_postpago', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.facturacion_postpago (
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
        ON dbo.facturacion_postpago(servicio_id, fecha_calculo);
END
GO

IF OBJECT_ID('dbo.tipos_transaccion_proveedor', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tipos_transaccion_proveedor (
        tipo_transaccion NVARCHAR(50) NOT NULL PRIMARY KEY,
        descripcion NVARCHAR(200) NULL,
        activo BIT NOT NULL CONSTRAINT df_tipos_transaccion_proveedor_activo DEFAULT 1
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.tipos_transaccion_proveedor
    WHERE tipo_transaccion = 'PROVEEDOR6'
)
BEGIN
    INSERT INTO dbo.tipos_transaccion_proveedor (tipo_transaccion, descripcion)
    VALUES ('PROVEEDOR6', 'Calculo de facturacion postpago por periodo');
END
GO

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
