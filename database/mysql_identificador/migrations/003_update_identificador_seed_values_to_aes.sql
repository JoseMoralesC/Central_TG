USE central_identificador;

UPDATE proveedores
SET nombre = 'Proveedor Telefonico XYZ',
    codigo = 'XYZ',
    activo = TRUE
WHERE proveedor_id = 1;

UPDATE telefonos
SET numero_cifrado = CASE telefono_id
    WHEN 1 THEN 'zn6cw4R7L5kvgOADpNE+Cw=='
    WHEN 2 THEN 'MYRdFog+97grrtD/Vt+sig=='
    WHEN 3 THEN 'l7kyqYzTB0DRFHGSZ9TFdQ=='
    WHEN 4 THEN 'jXrauYSymvM+ZNDtqzqleQ=='
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
