﻿CREATE TABLE [dbo].[Veicoli] (
    [Id]                   INT          IDENTITY (1, 1) NOT NULL,
    [IdTipo]               INT          NOT NULL,
    [Marca]                VARCHAR (30) NOT NULL,
    [Modello]              VARCHAR (50) NOT NULL,
    [IdAlimentazione]      INT          NOT NULL,
    [Colore]               VARCHAR (10) NULL,
    [Lunghezza]            SMALLINT     NULL,
    [Larghezza]            SMALLINT     NULL,
    [Altezza]              SMALLINT     NULL,
    [VIN]                  VARCHAR (50) NULL,
    [KM]                   INT          NULL,
    [VelocitaMassima]      SMALLINT     NULL,
    [Potenza]              SMALLINT     NULL,
    [DataImmatricolazione] DATE         NULL,
    [Prezzo]               INT          DEFAULT ((0)) NOT NULL,
    [Immagine]             TEXT         NULL,
    [AutoIsIntegrale]      BIT          NULL, 
    [AutoNumPorte]         SMALLINT     NULL, 
    [AutoDimCerchi]        SMALLINT     NULL, 
    [MotoIdTipologia]      INT          NULL, 
    [MotoNumTempi]         SMALLINT     NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
