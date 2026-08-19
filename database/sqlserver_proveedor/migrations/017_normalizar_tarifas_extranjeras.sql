UPDATE dbo.tarifas
SET costo_por_minuto = 60.00,
    descripcion = CASE
        WHEN descripcion IS NULL OR LTRIM(RTRIM(descripcion)) = ''
            THEN 'Llamadas internacionales'
        ELSE descripcion
    END,
    activa = 1
WHERE UPPER(ISNULL(tipo_llamada, '')) <> 'NACIONAL'
  AND (
      pais_id IS NOT NULL
      OR UPPER(ISNULL(tipo_llamada, '')) LIKE 'INTERNACIONAL%'
  );
