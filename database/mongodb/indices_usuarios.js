db = db.getSiblingDB("central_tg_mongo");

db.usuarios.createIndex(
  { identificacion: 1 },
  { unique: true, name: "ux_usuarios_identificacion" }
);

db.usuarios.createIndex(
  { usuario: 1 },
  { unique: true, name: "ux_usuarios_usuario_cifrado" }
);

db.usuarios.createIndex(
  { correo: 1 },
  { unique: true, name: "ux_usuarios_correo" }
);
