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
    proveedor_codigo,
    activo
)
VALUES
(1,'61925420','PREPAGO','KOLBI',1),
(2,'83447172','PREPAGO','CLARO',1),
(3,'60302145','POSTPAGO','LIBERTY',1),
(4,'71268439','PREPAGO','KOLBI',0);
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
