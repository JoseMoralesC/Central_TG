USE central_identificador;

SET @telefono_22223333 = 'l7kyqYzTB0DRFHGSZ9TFdQ==';
SET @telefono_66774422 = 'o+zuVoRkAiJRQRwByI/u3Q==';

DELETE b
FROM bitacora_identificador b
JOIN telefonos t ON t.telefono_id = b.telefono_id
WHERE t.numero_cifrado IN (@telefono_22223333, @telefono_66774422);

DELETE h
FROM historial_llamadas_identificador h
JOIN telefonos t ON t.telefono_id = h.telefono_id
WHERE t.numero_cifrado IN (@telefono_22223333, @telefono_66774422);

DELETE la
FROM llamadas_activas la
JOIN telefonos t ON t.telefono_id = la.telefono_id
WHERE t.numero_cifrado IN (@telefono_22223333, @telefono_66774422);

DELETE d
FROM dispositivos d
JOIN telefonos t ON t.telefono_id = d.telefono_id
WHERE t.numero_cifrado IN (@telefono_22223333, @telefono_66774422);

DELETE tt
FROM tarjetas_telefonicas tt
JOIN telefonos t ON t.telefono_id = tt.telefono_id
WHERE t.numero_cifrado IN (@telefono_22223333, @telefono_66774422);

DELETE FROM telefonos
WHERE numero_cifrado IN (@telefono_22223333, @telefono_66774422);
