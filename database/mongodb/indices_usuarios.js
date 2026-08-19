db = db.getSiblingDB("central_tg_mongo");

db.usuarios.createIndex(
  { identificacion: 1, tipo: 1 },
  { unique: true, name: "ux_usuarios_identificacion_tipo" }
);

db.usuarios.createIndex(
  { usuario: 1, tipo: 1 },
  { unique: true, name: "ux_usuarios_usuario_tipo" }
);

db.usuarios.createIndex(
  { correo: 1, tipo: 1 },
  { unique: true, name: "ux_usuarios_correo_tipo" }
);
