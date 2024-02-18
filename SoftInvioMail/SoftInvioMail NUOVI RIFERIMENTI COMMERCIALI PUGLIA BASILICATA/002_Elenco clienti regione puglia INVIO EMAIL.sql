USE [05Scadenze]
GO
CREATE VIEW [dbo].[Categorie] AS SELECT * FROM [052023CoGe].dbo.Categorie
GO
CREATE VIEW [dbo].[Agenti] AS SELECT * FROM [052023CoGe].dbo.Agenti
GO
CREATE VIEW [dbo].[Regioni] AS SELECT * FROM [052023CoGe].dbo.Regioni
GO
DELETE FROM EMAILINVIATETOGGETTO
GO
INSERT INTO EmailInviateTOggetto
                         (Stato, DataInvio, Note, OLDStato, Destinatario, Codice_CoGe, Rag_Soc, Denominazione, Riferimento, Email, EmailInvioScad, EmailInvioFatt, PECEmail, Indirizzo, CAP, Localita, Provincia, Codice_Fiscale, Partita_IVA, 
                         Telefono1, Telefono2, Categorie_desc, Agenti_desc)
SELECT        0 AS Expr1, NULL AS Expr2, '' AS Expr3, 0 AS Expr4, '' AS Expr5, Clienti.Codice_CoGe, Clienti.Rag_Soc, Clienti.Denominazione, Clienti.Riferimento, Clienti.Email, Clienti.EmailInvioScad, Clienti.EmailInvioFatt, Clienti.PECEmail, 
                         Clienti.Indirizzo, Clienti.CAP, Clienti.Localita, Clienti.Provincia, Clienti.Codice_Fiscale, Clienti.Partita_IVA, Clienti.Telefono1, Clienti.Telefono2, Categorie.Descrizione AS Categorie_desc, Agenti.Descrizione AS Agenti_desc
FROM            Clienti LEFT OUTER JOIN
                         Province ON Clienti.Provincia = Province.Codice LEFT OUTER JOIN
                         Regioni ON Province.Regione = Regioni.Codice LEFT OUTER JOIN
                         Categorie ON Clienti.Categoria = Categorie.Codice LEFT OUTER JOIN
                         Agenti ON Clienti.Agente_N = Agenti.Codice
WHERE        (Regioni.Codice = 17)
ORDER BY Clienti.Rag_Soc                   
GO

DBCC CHECKIDENT ('dbo.EmailInviateTOggetto', RESEED, 0);  
GO
elenco clienti PUGLIA BASILICATA
SELECT        Clienti.Codice_CoGe, Clienti.Rag_Soc, ISNULL(Clienti.Denominazione,'') as Denominazione, ISNULL(Clienti.Riferimento,'') AS Riferimento, 
ISNULL(Clienti.Email,'') as Email, ISNULL(Clienti.EmailInvioScad,'') as EmailInvioScad, ISNULL(Clienti.EmailInvioFatt,'') as EmailInvioFatt, 
ISNULL(Clienti.PECEmail,'') as PECEmail, ISNULL(Clienti.Indirizzo,'') as Indirizzo, ISNULL(Clienti.CAP,'') as CAP, ISNULL(Clienti.Localita,'')as Localita, 
ISNULL(Clienti.Provincia,'')as Provincia, ISNULL(Clienti.Codice_Fiscale,'')as Codice_Fiscale, ISNULL(Clienti.Partita_IVA,'') as Partita_IVA, 
ISNULL(Clienti.Telefono1,'')as Telefono1, ISNULL(Clienti.Telefono2,'')as Telefono2, 
ISNULL(Categorie.Descrizione,'') AS Categorie_desc, ISNULL(Agenti.Descrizione,'') AS Agenti_desc
FROM            Clienti LEFT OUTER JOIN
                         Province ON Clienti.Provincia = Province.Codice LEFT OUTER JOIN
                         Regioni ON Province.Regione = Regioni.Codice LEFT OUTER JOIN
                         Categorie ON Clienti.Categoria = Categorie.Codice LEFT OUTER JOIN
                         Agenti ON Clienti.Agente_N = Agenti.Codice
WHERE        (Regioni.Codice = 17) or (Regioni.Codice = 18)
ORDER BY Clienti.Rag_Soc 
