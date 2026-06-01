USE CentralProveedor;
GO


INSERT INTO clientes (
    nombre,
    identificacion,
    correo,
    activo
)
VALUES
(
    'Cliente Prepago Activo',
    '101110111',
    'prepago@test.com',
    1
),
(
    'Cliente Prepago Sin Saldo',
    '202220222',
    'sinsaldo@test.com',
    1
),
(
    'Cliente Postpago',
    '303330333',
    'postpago@test.com',
    1
),
(
    'Cliente Inactivo',
    '404440444',
    'inactivo@test.com',
    0
);
GO


INSERT INTO servicios (
    cliente_id,
    numero_telefono,
    tipo_servicio,
    activo
)
VALUES
(1,'88889999','PREPAGO',1),
(2,'88880000','PREPAGO',1),
(3,'22223333','POSTPAGO',1),
(4,'77776666','PREPAGO',0);
GO


INSERT INTO tarifas (
    tipo_llamada,
    descripcion,
    costo_por_minuto,
    activa
)
VALUES
(
    'NACIONAL',
    'Llamadas dentro del territorio nacional',
    10.00,
    1
),
(
    'INTERNACIONAL',
    'Llamadas fuera del territorio nacional',
    50.00,
    1
);
GO


INSERT INTO saldos (
    servicio_id,
    saldo_disponible
)
VALUES
(1,5000),
(2,0),
(3,0),
(4,1000);
GO