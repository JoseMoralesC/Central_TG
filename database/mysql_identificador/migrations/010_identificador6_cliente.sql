USE central_identificador;

DROP PROCEDURE IF EXISTS add_column_if_missing;

DELIMITER //
CREATE PROCEDURE add_column_if_missing(
    IN table_name_in VARCHAR(64),
    IN column_name_in VARCHAR(64),
    IN column_definition_in TEXT
)
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = DATABASE()
          AND table_name = table_name_in
          AND column_name = column_name_in
    ) THEN
        SET @sql = CONCAT(
            'ALTER TABLE ',
            table_name_in,
            ' ADD COLUMN ',
            column_name_in,
            ' ',
            column_definition_in
        );
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END //
DELIMITER ;

CALL add_column_if_missing(
    'telefonos',
    'identificacion_cliente_cifrada',
    'VARCHAR(255) NULL'
);

DROP PROCEDURE IF EXISTS add_column_if_missing;
