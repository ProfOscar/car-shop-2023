CREATE TABLE [dbo].[Veicoli]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [IdTipo] INT NOT NULL,
    [Marca] VARCHAR(30) NOT NULL, 
    [Modello] VARCHAR(50) NOT NULL, 
    [IdAlimentazione] INT NOT NULL, 
    [Colore] VARCHAR(10) NULL, 
    [Lunghezza] SMALLINT NULL, 
    [Larghezza] SMALLINT NULL, 
    [Altezza] SMALLINT NULL, 
    [VIN] VARCHAR(50) NULL, 
    [KM] INT NULL, 
    [VelocitaMassima] SMALLINT NULL, 
    [Potenza] SMALLINT NULL, 
    [DataImmatricolazione] DATE NULL, 
    [Prezzo] INT NOT NULL DEFAULT 0, 
    [Immagine] TEXT NULL
)
