db = db.getSiblingDB("central_tg_mongo");

const ahora = new Date();

db.usuarios.updateOne(
  { identificacion: "100200300" },
  {
    $set: {
      identificacion: "100200300",
      nombre: "Admin",
      primerApellido: "Persona",
      segundoApellido: "Dos",
      correo: "admin.persona2@example.com",
      usuario: "E+WcGVUGNMlMO+PeM6iXYQ==",
      contrasena: "oceRdJB5JPsjvicMW0R5Zw==",
      estado: "activo",
      tipo: NumberInt(1),
      fechaActualizacion: ahora
    },
    $setOnInsert: {
      fechaCreacion: ahora
    }
  },
  { upsert: true }
);

db.usuarios.updateOne(
  { identificacion: "200300400" },
  {
    $set: {
      identificacion: "200300400",
      nombre: "Cliente",
      primerApellido: "Persona",
      segundoApellido: "Dos",
      correo: "cliente.persona2@example.com",
      usuario: "FecrMXBny0NFu+4+h48AyQ==",
      contrasena: "1uCkdLoBqiWfdpysMDFcBA==",
      estado: "activo",
      tipo: NumberInt(2),
      fechaActualizacion: ahora
    },
    $setOnInsert: {
      fechaCreacion: ahora
    }
  },
  { upsert: true }
);
