USE central_identificador;

START TRANSACTION;

CREATE TEMPORARY TABLE tmp_telefonos_a_disponible (
    telefono_id INT PRIMARY KEY
);

INSERT INTO tmp_telefonos_a_disponible (telefono_id)
SELECT telefono_id
FROM telefonos;

SET @total_afectados = (
    SELECT COUNT(1)
    FROM tmp_telefonos_a_disponible
);

UPDATE llamadas_activas la
INNER JOIN tmp_telefonos_a_disponible t
    ON t.telefono_id = la.telefono_id
SET la.estado = 'FINALIZADA',
    la.fecha_fin_maxima = COALESCE(la.fecha_fin_maxima, NOW())
WHERE UPPER(COALESCE(la.estado, 'ACTIVA')) = 'ACTIVA';

UPDATE tarjetas_telefonicas tt
INNER JOIN tmp_telefonos_a_disponible a
    ON a.telefono_id = tt.telefono_id
SET tt.activa = FALSE;

UPDATE dispositivos d
INNER JOIN tmp_telefonos_a_disponible a
    ON a.telefono_id = d.telefono_id
SET d.activo = FALSE;

SET @tiene_identificacion_cliente = (
    SELECT COUNT(1)
    FROM information_schema.columns
    WHERE table_schema = DATABASE()
      AND table_name = 'telefonos'
      AND column_name = 'identificacion_cliente_cifrada'
);

SET @reset_telefonos_sql = IF(
    @tiene_identificacion_cliente > 0,
    'UPDATE telefonos t INNER JOIN tmp_telefonos_a_disponible a ON a.telefono_id = t.telefono_id SET t.activo = FALSE, t.identificacion_cliente_cifrada = NULL',
    'UPDATE telefonos t INNER JOIN tmp_telefonos_a_disponible a ON a.telefono_id = t.telefono_id SET t.activo = FALSE'
);

PREPARE reset_telefonos_stmt FROM @reset_telefonos_sql;
EXECUTE reset_telefonos_stmt;
DEALLOCATE PREPARE reset_telefonos_stmt;

SELECT
    @total_afectados AS telefonos_convertidos_a_disponible,
    SUM(CASE WHEN activo = FALSE THEN 1 ELSE 0 END) AS total_disponibles,
    SUM(CASE WHEN activo = TRUE THEN 1 ELSE 0 END) AS total_activos_restantes
FROM telefonos;

SELECT
    SUM(CASE WHEN activa = TRUE THEN 1 ELSE 0 END) AS sims_activas_restantes
FROM tarjetas_telefonicas;

SELECT
    SUM(CASE WHEN activo = TRUE THEN 1 ELSE 0 END) AS dispositivos_activos_restantes
FROM dispositivos;

DROP TEMPORARY TABLE tmp_telefonos_a_disponible;

COMMIT;
