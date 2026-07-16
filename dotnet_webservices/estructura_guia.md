# Estructura del proyecto — Alcance 2

La estructura propuesta mantiene los componentes desarrollados durante el primer alcance y agrega los nuevos servicios web, la base de datos MongoDB y los recursos necesarios para las pruebas de integración.

```text
Central_TG/
│
├── csharp_simulador/              # José - actualizar si corresponde
│
├── python_identificador/          # Gabriel - IDENTIFICADOR6 y ajustes
│
├── java_proveedor/                # Charlie - PROVEEDOR4, 5 y 6
│
├── dotnet_webservices/            # Nuevos proyectos C# en Visual Studio
│   │
│   ├── CentralTelefonica.WebServices.sln
│   │
│   ├── WS_Proveedor/
│   │   ├── Contracts/
│   │   ├── Services/
│   │   ├── Validators/
│   │   ├── Infrastructure/
│   │   └── WS_Proveedor.csproj
│   │
│   ├── WS_Identificador/
│   │   ├── Contracts/
│   │   ├── Services/
│   │   ├── Validators/
│   │   ├── Infrastructure/
│   │   └── WS_Identificador.csproj
│   │
│   └── WS_Autenticacion/
│       ├── Contracts/
│       ├── Services/
│       ├── Validators/
│       ├── Infrastructure/
│       └── WS_Autenticacion.csproj
│
├── database/
│   │
│   ├── mysql_identificador/
│   │
│   ├── sqlserver_proveedor/
│   │
│   └── mongodb_autenticacion/     # Nueva base de datos MongoDB
│       ├── collections/
│       ├── indexes/
│       ├── seed/
│       └── scripts/
│
├── shared/
│   ├── config/
│   ├── contracts/
│   ├── soap/                      # Ejemplos de mensajes XML/SOAP
│   └── encryption/
│
├── docs/
│   ├── Proyecto/
│   ├── alcance_1/
│   │
│   └── alcance_2/
│       ├── arquitectura/
│       ├── casos_uso/
│       ├── clases/
│       └── pruebas/
│
└── test/
    ├── integracion/
    └── webservices/