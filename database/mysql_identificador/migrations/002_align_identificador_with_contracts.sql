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

DROP PROCEDURE IF EXISTS add_index_if_missing $$
CREATE PROCEDURE add_index_if_missing(
    IN p_table_name VARCHAR(64),
    IN p_index_name VARCHAR(64),
    IN p_index_definition TEXT
)
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.statistics
        WHERE table_schema = DATABASE()
          AND table_name = p_table_name
          AND index_name = p_index_name
    ) THEN
        SET @ddl = CONCAT(
            'ALTER TABLE ',
            p_table_name,
            ' ADD ',
            p_index_definition
        );
        PREPARE stmt FROM @ddl;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
    END IF;
END $$

DELIMITER ;

CALL add_column_if_missing('proveedores', 'host_socket', 'VARCHAR(120) NULL');
CALL add_column_if_missing('proveedores', 'puerto_socket', 'INT NULL');
CALL add_column_if_missing('proveedores', 'protocolo', 'VARCHAR(20) NOT NULL DEFAULT ''TCP''');
CALL add_column_if_missing('proveedores', 'descripcion', 'VARCHAR(200) NULL');

UPDATE proveedores
SET host_socket = COALESCE(host_socket, '127.0.0.1'),
    puerto_socket = COALESCE(puerto_socket, 6000),
    protocolo = COALESCE(protocolo, 'TCP')
WHERE codigo = 'XYZ';

CALL add_column_if_missing('llamadas_activas', 'id_llamada', 'VARCHAR(80) NULL');
CALL add_column_if_missing('llamadas_activas', 'telefono_origen_cifrado', 'VARCHAR(255) NULL');
CALL add_column_if_missing('llamadas_activas', 'tipo_servicio', 'VARCHAR(20) NULL');
CALL add_column_if_missing('llamadas_activas', 'tipo_llamada', 'VARCHAR(30) NULL');
CALL add_column_if_missing('llamadas_activas', 'tiempo_maximo_segundos', 'INT NULL');
CALL add_column_if_missing('llamadas_activas', 'fecha_actualizacion', 'DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP');

CALL add_index_if_missing(
    'llamadas_activas',
    'ux_llamadas_activas_id_llamada',
    'UNIQUE INDEX ux_llamadas_activas_id_llamada (id_llamada)'
);

CALL add_index_if_missing(
    'llamadas_activas',
    'ix_llamadas_activas_estado_fin',
    'INDEX ix_llamadas_activas_estado_fin (estado, fecha_fin_maxima)'
);

CALL add_column_if_missing('historial_llamadas_identificador', 'id_llamada', 'VARCHAR(80) NULL');
CALL add_column_if_missing('historial_llamadas_identificador', 'telefono_origen_cifrado', 'VARCHAR(255) NULL');
CALL add_column_if_missing('historial_llamadas_identificador', 'duracion_segundos', 'INT NULL');
CALL add_column_if_missing('historial_llamadas_identificador', 'duracion_minutos', 'INT NULL');
CALL add_column_if_missing('historial_llamadas_identificador', 'tipo_servicio', 'VARCHAR(20) NULL');
CALL add_column_if_missing('historial_llamadas_identificador', 'tipo_llamada', 'VARCHAR(30) NULL');
CALL add_column_if_missing('historial_llamadas_identificador', 'costo_por_minuto', 'DECIMAL(10,2) NULL');
CALL add_column_if_missing('historial_llamadas_identificador', 'monto_total', 'DECIMAL(10,2) NULL');
CALL add_column_if_missing('historial_llamadas_identificador', 'moneda', 'VARCHAR(10) NOT NULL DEFAULT ''CRC''');
CALL add_column_if_missing('historial_llamadas_identificador', 'estado', 'VARCHAR(30) NOT NULL DEFAULT ''FINALIZADA''');

CALL add_index_if_missing(
    'historial_llamadas_identificador',
    'ix_historial_id_llamada',
    'INDEX ix_historial_id_llamada (id_llamada)'
);

CALL add_index_if_missing(
    'historial_llamadas_identificador',
    'ix_historial_fecha_inicio',
    'INDEX ix_historial_fecha_inicio (fecha_inicio)'
);

CALL add_column_if_missing('bitacora_identificador', 'direccion', 'VARCHAR(20) NULL');
CALL add_column_if_missing('bitacora_identificador', 'resultado_codigo', 'VARCHAR(30) NULL');
CALL add_column_if_missing('bitacora_identificador', 'id_llamada', 'VARCHAR(80) NULL');
CALL add_column_if_missing('bitacora_identificador', 'telefono_origen_cifrado', 'VARCHAR(255) NULL');
CALL add_column_if_missing('bitacora_identificador', 'telefono_destino', 'VARCHAR(30) NULL');
CALL add_column_if_missing('bitacora_identificador', 'proveedor_codigo', 'VARCHAR(20) NULL');
CALL add_column_if_missing('bitacora_identificador', 'correlacion_id', 'VARCHAR(80) NULL');

CALL add_index_if_missing(
    'bitacora_identificador',
    'ix_bitacora_identificador_fecha',
    'INDEX ix_bitacora_identificador_fecha (fecha_registro)'
);

CALL add_index_if_missing(
    'bitacora_identificador',
    'ix_bitacora_identificador_correlacion',
    'INDEX ix_bitacora_identificador_correlacion (correlacion_id)'
);

CREATE TABLE IF NOT EXISTS tipos_transaccion_identificador (
    tipo_transaccion VARCHAR(50) PRIMARY KEY,
    descripcion VARCHAR(200) NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

INSERT INTO tipos_transaccion_identificador
    (tipo_transaccion, descripcion, activo)
VALUES
    ('SOLICITUD_LLAMADA', 'Solicitud de autorizacion de llamada desde C#', TRUE),
    ('RESPUESTA_LLAMADA', 'Respuesta de autorizacion hacia C#', TRUE),
    ('INICIO_LLAMADA', 'Registro de inicio de llamada activa', TRUE),
    ('FINALIZAR_LLAMADA', 'Solicitud de finalizacion de llamada', TRUE),
    ('RESPUESTA_FINALIZACION', 'Respuesta de finalizacion de llamada', TRUE),
    ('CONSULTA_SALDO', 'Consulta de saldo desde C#', TRUE),
    ('RESPUESTA_SALDO', 'Respuesta de consulta de saldo hacia C#', TRUE),
    ('CONSULTA_PROVEEDOR', 'Consulta enviada desde Python hacia proveedor', TRUE),
    ('RESPUESTA_PROVEEDOR', 'Respuesta recibida desde proveedor', TRUE)
ON DUPLICATE KEY UPDATE
    descripcion = VALUES(descripcion),
    activo = VALUES(activo);

DROP PROCEDURE IF EXISTS add_column_if_missing;
DROP PROCEDURE IF EXISTS add_index_if_missing;
