USE central_identificador;

INSERT INTO proveedores (nombre, codigo, activo)
VALUES
    ('Tigo Panama', 'TIGO_PA', TRUE),
    ('Telcel Mexico', 'TELCEL_MX', TRUE),
    ('Orange France', 'ORANGE_FR', TRUE)
ON DUPLICATE KEY UPDATE
    nombre = VALUES(nombre),
    activo = VALUES(activo);

INSERT INTO telefonos (
    numero_cifrado,
    proveedor_id,
    tipo_servicio,
    pais,
    pais_id,
    activo
)
SELECT 'Q0+yfKTkvTWLk9PcpJcmVQ==', pr.proveedor_id, 'PREPAGO', 'Panama', pa.pais_id, TRUE
FROM proveedores pr
JOIN paises pa ON pa.nombre = 'Panama'
WHERE pr.codigo = 'TIGO_PA'
  AND NOT EXISTS (
      SELECT 1 FROM telefonos WHERE numero_cifrado = 'Q0+yfKTkvTWLk9PcpJcmVQ=='
  );

INSERT INTO telefonos (
    numero_cifrado,
    proveedor_id,
    tipo_servicio,
    pais,
    pais_id,
    activo
)
SELECT 'LTjpQoNP4qIlK9O678WK4g==', pr.proveedor_id, 'PREPAGO', 'Mexico', pa.pais_id, TRUE
FROM proveedores pr
JOIN paises pa ON pa.nombre = 'Mexico'
WHERE pr.codigo = 'TELCEL_MX'
  AND NOT EXISTS (
      SELECT 1 FROM telefonos WHERE numero_cifrado = 'LTjpQoNP4qIlK9O678WK4g=='
  );

INSERT INTO telefonos (
    numero_cifrado,
    proveedor_id,
    tipo_servicio,
    pais,
    pais_id,
    activo
)
SELECT '6R6uwBU4TajeQomds27EZQ==', pr.proveedor_id, 'PREPAGO', 'Francia', pa.pais_id, TRUE
FROM proveedores pr
JOIN paises pa ON pa.nombre = 'Francia'
WHERE pr.codigo = 'ORANGE_FR'
  AND NOT EXISTS (
      SELECT 1 FROM telefonos WHERE numero_cifrado = '6R6uwBU4TajeQomds27EZQ=='
  );

INSERT INTO tarjetas_telefonicas (
    telefono_id,
    identificador_tarjeta_cifrado,
    activa
)
SELECT t.telefono_id, 's58dutXAjgnkMvc2/k7WXROMVFCcCQPKruhf7tkKaVw=', TRUE
FROM telefonos t
WHERE t.numero_cifrado = 'Q0+yfKTkvTWLk9PcpJcmVQ=='
  AND NOT EXISTS (
      SELECT 1 FROM tarjetas_telefonicas WHERE telefono_id = t.telefono_id
  );

INSERT INTO tarjetas_telefonicas (
    telefono_id,
    identificador_tarjeta_cifrado,
    activa
)
SELECT t.telefono_id, 'bkDdb8Dx5ErnxtvR48b9/V4CWy+h5AHWeA0EbnWf/GQ=', TRUE
FROM telefonos t
WHERE t.numero_cifrado = 'LTjpQoNP4qIlK9O678WK4g=='
  AND NOT EXISTS (
      SELECT 1 FROM tarjetas_telefonicas WHERE telefono_id = t.telefono_id
  );

INSERT INTO tarjetas_telefonicas (
    telefono_id,
    identificador_tarjeta_cifrado,
    activa
)
SELECT t.telefono_id, '/WDBP85W2lFufm8OV/3GFLhFnzs6B9j1qVeQ8K/vLXw=', TRUE
FROM telefonos t
WHERE t.numero_cifrado = '6R6uwBU4TajeQomds27EZQ=='
  AND NOT EXISTS (
      SELECT 1 FROM tarjetas_telefonicas WHERE telefono_id = t.telefono_id
  );

INSERT INTO dispositivos (
    telefono_id,
    identificador_dispositivo_cifrado,
    activo
)
SELECT t.telefono_id, 'dGCRAbeaXZiOPU/nxnOQ0vJ6O12zKz+04L5MN7EyqBQ=', TRUE
FROM telefonos t
WHERE t.numero_cifrado = 'Q0+yfKTkvTWLk9PcpJcmVQ=='
  AND NOT EXISTS (
      SELECT 1 FROM dispositivos WHERE telefono_id = t.telefono_id
  );

INSERT INTO dispositivos (
    telefono_id,
    identificador_dispositivo_cifrado,
    activo
)
SELECT t.telefono_id, 'PjNUL7WrkyYIZBGHiZXBIdeEI16YaoL337/6yh0UwS0=', TRUE
FROM telefonos t
WHERE t.numero_cifrado = 'LTjpQoNP4qIlK9O678WK4g=='
  AND NOT EXISTS (
      SELECT 1 FROM dispositivos WHERE telefono_id = t.telefono_id
  );

INSERT INTO dispositivos (
    telefono_id,
    identificador_dispositivo_cifrado,
    activo
)
SELECT t.telefono_id, 'iRQZAFi0saURt08rj+5I43QDXQlejCxyzJ//Zkg1IgU=', TRUE
FROM telefonos t
WHERE t.numero_cifrado = '6R6uwBU4TajeQomds27EZQ=='
  AND NOT EXISTS (
      SELECT 1 FROM dispositivos WHERE telefono_id = t.telefono_id
  );
