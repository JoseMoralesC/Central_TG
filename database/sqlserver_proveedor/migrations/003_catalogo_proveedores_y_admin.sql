USE CentralProveedor;
GO

IF COL_LENGTH('dbo.servicios', 'proveedor_codigo') IS NULL
BEGIN
    ALTER TABLE dbo.servicios ADD proveedor_codigo NVARCHAR(20) NULL;
END;
GO

UPDATE dbo.servicios
SET proveedor_codigo = CASE servicio_id
    WHEN 1 THEN 'KOLBI'
    WHEN 2 THEN 'CLARO'
    WHEN 3 THEN 'LIBERTY'
    WHEN 4 THEN 'KOLBI'
    ELSE COALESCE(proveedor_codigo, 'KOLBI')
END
WHERE servicio_id IN (1, 2, 3, 4);
GO

INSERT INTO dbo.tipos_transaccion_proveedor (tipo_transaccion, descripcion, activo)
SELECT tipo_transaccion, descripcion, activo
FROM (
    VALUES
        ('DETALLE_TELEFONO', 'Consulta administrativa de detalle de telefono', 1),
        ('RECARGAR_SALDO', 'Recarga administrativa de saldo prepago', 1),
        ('REGISTRAR_TELEFONO', 'Registro administrativo de telefono', 1),
        ('CAMBIAR_ESTADO_TELEFONO', 'Activacion o desactivacion administrativa de telefono', 1)
) AS source (tipo_transaccion, descripcion, activo)
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.tipos_transaccion_proveedor target
    WHERE target.tipo_transaccion = source.tipo_transaccion
);
GO
