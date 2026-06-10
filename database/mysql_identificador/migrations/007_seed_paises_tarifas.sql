USE central_identificador;

INSERT INTO paises (
    nombre,
    codigo_area,
    clasificacion,
    tarifa_minuto,
    moneda,
    activo
) VALUES
    ('Costa Rica', '+506', 'NACIONAL', 10.00, 'CRC', TRUE),
    ('Panama', '+507', 'EXTRANJERO', 15.00, 'CRC', TRUE),
    ('Mexico', '+52', 'EXTRANJERO', 25.00, 'CRC', TRUE),
    ('Francia', '+33', 'EXTRANJERO', 60.00, 'CRC', TRUE)
ON DUPLICATE KEY UPDATE
    codigo_area = VALUES(codigo_area),
    clasificacion = VALUES(clasificacion),
    tarifa_minuto = VALUES(tarifa_minuto),
    moneda = VALUES(moneda),
    activo = VALUES(activo);

UPDATE telefonos t
JOIN paises p ON p.nombre = t.pais
SET t.pais_id = p.pais_id;
