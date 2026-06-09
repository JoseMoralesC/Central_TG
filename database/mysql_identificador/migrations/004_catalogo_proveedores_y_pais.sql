USE central_identificador;

DELIMITER $$

DROP PROCEDURE IF EXISTS add_column_if_missing $$
CREATE PROCEDURE add_column_if_missing(
    IN p_table_name VARCHAR(64),
    IN p_column_name VARCHAR(64),
    IN p_column_definition TEXT
)
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = DATABASE()
          AND table_name = p_table_name
          AND column_name = p_column_name
    ) THEN
        SET @ddl = CONCAT(
            'ALTER TABLE ',
            p_table_name,
            ' ADD COLUMN ',
            p_column_name,
            ' ',
            p_column_definition
        );
        PREPARE stmt FROM @ddl;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END $$

DELIMITER ;

INSERT INTO proveedores (nombre, codigo, activo)
VALUES
    ('Kolbi', 'KOLBI', TRUE),
    ('Claro', 'CLARO', TRUE),
    ('Liberty', 'LIBERTY', TRUE),
    ('Movistar', 'MOVISTAR', TRUE)
ON DUPLICATE KEY UPDATE
    nombre = VALUES(nombre),
    activo = VALUES(activo);

CALL add_column_if_missing('telefonos', 'pais', 'VARCHAR(80) NOT NULL DEFAULT ''Costa Rica''');

UPDATE telefonos t
JOIN proveedores p ON p.codigo = CASE t.telefono_id
    WHEN 1 THEN 'KOLBI'
    WHEN 2 THEN 'CLARO'
    WHEN 3 THEN 'LIBERTY'
    WHEN 4 THEN 'KOLBI'
    ELSE 'KOLBI'
END
SET t.proveedor_id = p.proveedor_id,
    t.pais = CASE t.telefono_id
        WHEN 3 THEN 'Panama'
        ELSE 'Costa Rica'
    END
WHERE t.telefono_id IN (1, 2, 3, 4);

INSERT INTO tipos_transaccion_identificador
    (tipo_transaccion, descripcion, activo)
VALUES
    ('CONSULTA_CATALOGO_TELEFONOS', 'Consulta administrativa de catalogo real de telefonos', TRUE),
    ('RESPUESTA_CATALOGO_TELEFONOS', 'Respuesta de catalogo real hacia C#', TRUE),
    ('RECARGAR_SALDO', 'Solicitud administrativa de recarga de saldo', TRUE),
    ('RESPUESTA_RECARGA_SALDO', 'Respuesta administrativa de recarga de saldo', TRUE),
    ('REGISTRAR_TELEFONO', 'Solicitud administrativa de registro de telefono', TRUE),
    ('RESPUESTA_REGISTRO_TELEFONO', 'Respuesta administrativa de registro de telefono', TRUE),
    ('CAMBIAR_ESTADO_TELEFONO', 'Solicitud administrativa de activacion o desactivacion de telefono', TRUE),
    ('RESPUESTA_CAMBIO_ESTADO_TELEFONO', 'Respuesta administrativa de cambio de estado de telefono', TRUE)
ON DUPLICATE KEY UPDATE
    descripcion = VALUES(descripcion),
    activo = VALUES(activo);

DROP PROCEDURE IF EXISTS add_column_if_missing;
