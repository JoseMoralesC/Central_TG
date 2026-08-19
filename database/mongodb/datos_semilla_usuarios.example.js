db = db.getSiblingDB("central_tg_mongo");

db.usuarios.insertMany([
  {
    identificacion: "118880999",
    nombre: "Jose",
    primerApellido: "Morales",
    segundoApellido: "Calderon",
    correo: "jose@example.com",
    usuario: "USUARIO_CIFRADO_EJEMPLO",
    contrasena: "CONTRASENA_CIFRADA_EJEMPLO",
    estado: "activo",
    tipo: 1,
    fechaCreacion: new Date(),
    fechaActualizacion: new Date()
  },
  {
    identificacion: "209990888",
    nombre: "Cliente",
    primerApellido: "Prueba",
    correo: "cliente@example.com",
    usuario: "CLIENTE_CIFRADO_EJEMPLO",
    contrasena: "CONTRASENA_CLIENTE_CIFRADA_EJEMPLO",
    estado: "activo",
    tipo: 2,
    fechaCreacion: new Date(),
    fechaActualizacion: new Date()
  }
]);
