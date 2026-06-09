USE central_identificador;

INSERT INTO proveedores (nombre, codigo, activo) VALUES
('Kolbi', 'KOLBI', TRUE),
('Claro', 'CLARO', TRUE),
('Liberty', 'LIBERTY', TRUE),
('Movistar', 'MOVISTAR', TRUE);

INSERT INTO telefonos (
    numero_cifrado,
    proveedor_id,
    tipo_servicio,
    pais,
    activo
) VALUES
('ENC_61925420', 1, 'PREPAGO', 'Costa Rica', TRUE),
('ENC_83447172', 2, 'PREPAGO', 'Costa Rica', TRUE),
('ENC_60302145', 3, 'POSTPAGO', 'Panama', TRUE),
('ENC_71268439', 1, 'PREPAGO', 'Costa Rica', FALSE);

INSERT INTO tarjetas_telefonicas (
    telefono_id,
    identificador_tarjeta_cifrado,
    activa
) VALUES
(1, 'ENC_SIM_1234567891234567891', TRUE),
(2, 'ENC_SIM_2222222222222222222', TRUE),
(3, 'ENC_SIM_3333333333333333333', TRUE),
(4, 'ENC_SIM_4444444444444444444', TRUE);

INSERT INTO dispositivos (
    telefono_id,
    identificador_dispositivo_cifrado,
    activo
) VALUES
(1, 'ENC_IMEI_1234567891234567', TRUE),
(2, 'ENC_IMEI_2222222222222222', TRUE),
(3, 'ENC_IMEI_3333333333333333', TRUE),
(4, 'ENC_IMEI_4444444444444444', TRUE);
