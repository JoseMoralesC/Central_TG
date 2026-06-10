USE CentralProveedor;
GO

IF OBJECT_ID('dbo.paises', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.paises (
        pais_id INT IDENTITY(1,1) PRIMARY KEY,
        nombre NVARCHAR(80) NOT NULL UNIQUE,
        codigo_area NVARCHAR(10) NOT NULL UNIQUE,
        clasificacion NVARCHAR(20) NOT NULL,
        activo BIT NOT NULL CONSTRAINT df_paises_activo DEFAULT 1,
        created_at DATETIME NOT NULL CONSTRAINT df_paises_created_at DEFAULT GETDATE(),
        updated_at DATETIME NOT NULL CONSTRAINT df_paises_updated_at DEFAULT GETDATE()
    );
END;
GO

IF COL_LENGTH('dbo.servicios', 'pais_id') IS NULL
BEGIN
    ALTER TABLE dbo.servicios ADD pais_id INT NULL;

    ALTER TABLE dbo.servicios
    ADD CONSTRAINT fk_servicios_paises
        FOREIGN KEY (pais_id)
        REFERENCES dbo.paises(pais_id);
END;
GO

IF COL_LENGTH('dbo.tarifas', 'pais_id') IS NULL
BEGIN
    ALTER TABLE dbo.tarifas ADD pais_id INT NULL;

    ALTER TABLE dbo.tarifas
    ADD CONSTRAINT fk_tarifas_paises
        FOREIGN KEY (pais_id)
        REFERENCES dbo.paises(pais_id);
END;
GO
