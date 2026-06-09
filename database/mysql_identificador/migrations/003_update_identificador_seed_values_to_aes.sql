USE central_identificador;

INSERT INTO proveedores (nombre, codigo, activo)
VALUES
    ('Kolbi', 'KOLBI', TRUE),
    ('Claro', 'CLARO', TRUE),
    ('Liberty', 'LIBERTY', TRUE),
    ('Movistar', 'MOVISTAR', TRUE)
ON DUPLICATE KEY UPDATE
    nombre = VALUES(nombre),
    activo = VALUES(activo);

UPDATE telefonos
SET numero_cifrado = CASE telefono_id
    WHEN 1 THEN 'Ssy0wADM0R8i4alx8T1aHg=='
    WHEN 2 THEN '8rdtdFHBFJKlmaAFgVLhZA=='
    WHEN 3 THEN 'hr6q8rF6+HGpYj9AW2+Y6g=='
    WHEN 4 THEN 'M2xnYxuwGO9DlT1MKVNhxA=='
    ELSE numero_cifrado
END
WHERE telefono_id IN (1, 2, 3, 4);

UPDATE tarjetas_telefonicas
SET identificador_tarjeta_cifrado = CASE telefono_id
    WHEN 1 THEN 'SiD34tkppNlwDR0djVZ1Y2v/ZBQpheDwYhnO6nvHodE='
    WHEN 2 THEN 'BVA25tyD4VO8W9cBEyG2V1xMXJqAkunRS60n53xEgNg='
    WHEN 3 THEN 'pq8eFOl7apsLP4xOD2T0JIyWJivZX8AEP2gCuROcRxU='
    WHEN 4 THEN 'QM4Qfj9dx6pPtNAFQQi1OLTvC1BztyAPCZDEggDjmKs='
    ELSE identificador_tarjeta_cifrado
END
WHERE telefono_id IN (1, 2, 3, 4);

UPDATE dispositivos
SET identificador_dispositivo_cifrado = CASE telefono_id
    WHEN 1 THEN 'SiD34tkppNlwDR0djVZ1Y/2S3RnD0Y+RdOK722EqEnY='
    WHEN 2 THEN 'BVA25tyD4VO8W9cBEyG2VzmWrKsCe3RYCam2Mw8d260='
    WHEN 3 THEN 'pq8eFOl7apsLP4xOD2T0JDHe64PqadLyonU5Y1HmQ80='
    WHEN 4 THEN 'QM4Qfj9dx6pPtNAFQQi1OApH41qMhXEwLiWJv9lD1qE='
    ELSE identificador_dispositivo_cifrado
END
WHERE telefono_id IN (1, 2, 3, 4);
