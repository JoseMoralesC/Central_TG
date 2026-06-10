USE CentralProveedor;
GO

MERGE dbo.paises AS target
USING (
    VALUES
        ('Costa Rica', '+506', 'NACIONAL', 1),
        ('Panama', '+507', 'EXTRANJERO', 1),
        ('Mexico', '+52', 'EXTRANJERO', 1),
        ('Francia', '+33', 'EXTRANJERO', 1)
) AS source (nombre, codigo_area, clasificacion, activo)
ON target.nombre = source.nombre
WHEN MATCHED THEN
    UPDATE SET
        codigo_area = source.codigo_area,
        clasificacion = source.clasificacion,
        activo = source.activo,
        updated_at = GETDATE()
WHEN NOT MATCHED THEN
    INSERT (nombre, codigo_area, clasificacion, activo)
    VALUES (source.nombre, source.codigo_area, source.clasificacion, source.activo);
GO

UPDATE s
SET pais_id = p.pais_id
FROM dbo.servicios s
JOIN dbo.paises p
    ON p.nombre = CASE
        WHEN s.numero_telefono LIKE '+507%' THEN 'Panama'
        WHEN s.numero_telefono LIKE '+52%' THEN 'Mexico'
        WHEN s.numero_telefono LIKE '+33%' THEN 'Francia'
        ELSE 'Costa Rica'
    END
WHERE s.pais_id IS NULL;
GO

UPDATE t
SET pais_id = p.pais_id,
    costo_por_minuto = 10.00,
    descripcion = 'Llamadas dentro del territorio nacional',
    activa = 1
FROM dbo.tarifas t
JOIN dbo.paises p ON p.nombre = 'Costa Rica'
WHERE t.tipo_llamada = 'NACIONAL';
GO

UPDATE dbo.tarifas
SET activa = 0,
    descripcion = 'Tarifa internacional generica reemplazada por tarifas por pais'
WHERE tipo_llamada = 'INTERNACIONAL';
GO

INSERT INTO dbo.tarifas (
    tipo_llamada,
    descripcion,
    costo_por_minuto,
    activa,
    pais_id
)
SELECT source.tipo_llamada, source.descripcion, source.costo_por_minuto, 1, p.pais_id
FROM (
    VALUES
        ('INTERNACIONAL_PA', 'Llamadas internacionales hacia Panama', 15.00, 'Panama'),
        ('INTERNACIONAL_MX', 'Llamadas internacionales hacia Mexico', 25.00, 'Mexico'),
        ('INTERNACIONAL_FR', 'Llamadas internacionales hacia Francia', 60.00, 'Francia')
) AS source (tipo_llamada, descripcion, costo_por_minuto, pais_nombre)
JOIN dbo.paises p ON p.nombre = source.pais_nombre
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.tarifas t
    WHERE t.tipo_llamada = source.tipo_llamada
);
GO

UPDATE t
SET t.descripcion = source.descripcion,
    t.costo_por_minuto = source.costo_por_minuto,
    t.activa = 1,
    t.pais_id = p.pais_id
FROM dbo.tarifas t
JOIN (
    VALUES
        ('INTERNACIONAL_PA', 'Llamadas internacionales hacia Panama', 15.00, 'Panama'),
        ('INTERNACIONAL_MX', 'Llamadas internacionales hacia Mexico', 25.00, 'Mexico'),
        ('INTERNACIONAL_FR', 'Llamadas internacionales hacia Francia', 60.00, 'Francia')
) AS source (tipo_llamada, descripcion, costo_por_minuto, pais_nombre)
    ON source.tipo_llamada = t.tipo_llamada
JOIN dbo.paises p ON p.nombre = source.pais_nombre;
GO
