db = db.getSiblingDB("central_tg_mongo");

db.createCollection("usuarios", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: [
        "identificacion",
        "nombre",
        "primerApellido",
        "segundoApellido",
        "correo",
        "usuario",
        "contrasena",
        "estado",
        "tipo"
      ],
      properties: {
        identificacion: { bsonType: "string" },
        nombre: { bsonType: "string" },
        primerApellido: { bsonType: "string" },
        segundoApellido: { bsonType: "string" },
        correo: { bsonType: "string" },
        usuario: { bsonType: "string" },
        contrasena: { bsonType: "string" },
        estado: { enum: ["activo", "inactivo"] },
        tipo: { enum: [1, 2] },
        fechaCreacion: { bsonType: "date" },
        fechaActualizacion: { bsonType: "date" }
      }
    }
  }
});
