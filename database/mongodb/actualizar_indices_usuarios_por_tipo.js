db = db.getSiblingDB("central_tg_mongo");

const indicesAnteriores = [
  "ux_usuarios_identificacion",
  "ux_usuarios_usuario_cifrado",
  "ux_usuarios_correo"
];

const indicesActuales = db.usuarios.getIndexes().map(function (indice) {
  return indice.name;
});

indicesAnteriores.forEach(function (nombreIndice) {
  if (indicesActuales.indexOf(nombreIndice) >= 0) {
    db.usuarios.dropIndex(nombreIndice);
  }
});

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
