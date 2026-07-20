db = db.getSiblingDB("central_tg_mongo");

if (!db.getCollectionNames().includes("usuarios")) {
  db.createCollection("usuarios", {
    validator: {
      $jsonSchema: {
        bsonType: "object",
        required: [
          "identificacion",
          "nombre",
          "primerApellido",
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
          segundoApellido: { bsonType: ["string", "null"] },
          correo: { bsonType: "string" },
          usuario: { bsonType: "string" },
          contrasena: { bsonType: "string" },
          estado: { enum: ["activo", "inactivo"] },
          tipo: { bsonType: "int", minimum: 1, maximum: 2 },
          fechaCreacion: { bsonType: "date" },
          fechaActualizacion: { bsonType: "date" }
        }
      }
    }
  });
}

