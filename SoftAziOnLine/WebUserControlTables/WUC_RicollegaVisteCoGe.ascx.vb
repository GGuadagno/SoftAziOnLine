Imports It.SoftAzi.Model.Facade
Imports System.Data.SqlClient
Imports SoftAziOnLine.Def
'Imports It.SoftAzi.Model.Entity
'Imports It.SoftAzi.SystemFramework
'Imports SoftAziOnLine.WebFormUtility
'Imports SoftAziOnLine.Formatta
Partial Public Class WUC_RicollegaVisteCoGe
    Inherits System.Web.UI.UserControl
    Dim SWErrore As Boolean = False
    Dim myErrore As String = ""
    Dim NomeFunz As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strDitta As String = Session(CSTCODDITTA)
        If IsNothing(strDitta) Then
            strDitta = ""
        End If
        If String.IsNullOrEmpty(strDitta) Then
            strDitta = ""
        End If
        If strDitta = "" Or Not IsNumeric(strDitta) Then
            Chiudi("Errore: CODICE DITTA SCONOSCIUTO")
            Exit Sub
        End If
        '-
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Sub
        End If
        ModalPopup1.WucElement = Me
        If (Not IsPostBack) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CollegaTabelleAZISCAD"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "GoMenu"
            ModalPopup1.Show("Collegamento tabelle esterne (CoGe/GesAzi/Scadenzario)", "Confermi il collegamento tabelle esterne (CoGe/GesAzi/Scadenzario) ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
        End If
    End Sub
    'GIU250418 GIU280618
    Public Function CollegaTabelleAZISCAD() As Boolean
        Dim strDitta As String = Session(CSTCODDITTA)
        If IsNothing(strDitta) Then
            strDitta = ""
        End If
        If String.IsNullOrEmpty(strDitta) Then
            strDitta = ""
        End If
        If strDitta = "" Or Not IsNumeric(strDitta) Then
            Chiudi("Errore: CODICE DITTA SCONOSCIUTO")
            Exit Function
        End If
        '-
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Function
        End If
        Dim dbcon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SQLConn As New SqlConnection
        Dim SQLCmd As New SqlCommand
        Dim strSQL As String
        Dim strSP As String

        'giu040918 DA QUI PARTE I COLLEGAMENTI DI SOFTAZI
        SQLConn.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi)
        SQLCmd.CommandType = CommandType.Text
        SQLCmd.Connection = SQLConn
        SWErrore = False
        myErrore = "" 'giu230113
        Try
            If SQLConn.State <> ConnectionState.Open Then
                SQLConn.Open()
            End If
            'AGENTI aggiorno la nuova vista
            NomeFunz = "Agenti"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW Agenti AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Agenti"
            SQLCmd.ExecuteNonQuery()
            'ArticoliInst_ContrattiAss aggiorno la nuova vista
            NomeFunz = "ArticoliInst_ContrattiAss"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW ArticoliInst_ContrattiAss AS SELECT * FROM [" & Session(CSTCODDITTA) & "Scadenze].dbo.ArticoliInst_ContrattiAss"
            SQLCmd.ExecuteNonQuery()
            'Categorie AGGIORNO la vista
            NomeFunz = "Categorie"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW Categorie AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Categorie"
            SQLCmd.ExecuteNonQuery()
            'CLIENTI AGGIORNO la vista
            NomeFunz = "Clienti"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW Clienti AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Clienti"
            SQLCmd.ExecuteNonQuery()
            'CLIFOR AGGIORNO la vista
            NomeFunz = "CliFor"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSQL = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW CliFor AS SELECT Codice_CoGe, Rag_Soc, Indirizzo, Localita, CAP, Provincia, Nazione, Telefono1, Telefono2, Fax, Societa, Codice_Fiscale, Partita_IVA, Zona, Categoria, " _
                & "ABI_N, CAB_N, Regime_IVA, Agente_N, Vettore_N, Pagamento_N, Ragg_P, Bilancio_SN, Allegato_IVA, Apertura, DA_Apertura, Saldo_Dare, " _
                & "Saldo_Avere, Data_Agg_Saldi, Saldo_Prec, DA_Saldo_Prec, Dare_Chiusura, Avere_Chiusura, Riferimento, Denominazione, Titolare, Email, " _
                & "Saldo_Dare_2, Saldo_Avere_2, Dare_Chiusura_2, Avere_Chiusura_2, Apertura_2, DA_Apertura_2, DEM, Contenitori, Inviato, Listino, Conto_Corrente, " _
                & "Credito_1, Credito_2, Provincia_Estera, IndirizzoSenzaNumero, NumeroCivico, GiornoChiusura_1, GiornoChiusura_2, ChiusuraMattino_1, " _
                & "ChiusuraPomeriggio_1, ChiusuraMattino_2, ChiusuraPomeriggio_2, Note, CodProvinciaAgente, Agente_N_Prec, Codice_SEDE, Codice_Ricavo, " _
                & "0 AS FuoriZona, 0 AS Modalita_Invio_Ordine, NULL AS Data_Inizio_Chiusura, NULL AS Data_Fine_Chiusura, IVASosp, NoFatt, 0 AS Rit_Acconto, " _
                & "Data_Nascita, 0 AS StRit_Acconto, 0 AS CodTributo, CIN, NazIBAN, CINEUIBAN, SWIFT, CSAggrAllIVA, CIG, CUP, InseritoDa, ModificatoDa, IBAN_Ditta, " _
                & "CodiceMABELL, EsportatoMABELL, IDTipoFatt, IPA, SplitIVA, EmailInvioFatt, EmailInvioScad, InvioMailScad, PECEmail " _
                & "FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Clienti UNION ALL " _
                & "SELECT Codice_CoGe, Rag_Soc, Indirizzo, Localita, CAP, Provincia, Nazione, Telefono1, Telefono2, Fax, Societa, Codice_Fiscale, Partita_IVA, Zona, Categoria, " _
                & "ABI_N, CAB_N, Regime_IVA, 0 AS Agente_N, 0 AS Vettore_N, Pagamento_N, Ragg_P, Bilancio_SN, Allegato_IVA, Apertura, DA_Apertura, Saldo_Dare, " _
                & "Saldo_Avere, Data_Agg_Saldi, Saldo_Prec, DA_Saldo_Prec, Dare_Chiusura, Avere_Chiusura, Riferimento, Denominazione, Titolare, Email, " _
                & "Saldo_Dare_2, Saldo_Avere_2, Dare_Chiusura_2, Avere_Chiusura_2, Apertura_2, DA_Apertura_2, 0 AS DEM, 0 AS Contenitori, 0 AS Inviato, Listino, Conto_Corrente, " _
                & "0 AS Credito_1, 0 AS Credito_2, Provincia_Estera, IndirizzoSenzaNumero, NumeroCivico, 0 AS GiornoChiusura_1, " _
                & "0 AS GiornoChiusura_2, 0 AS ChiusuraMattino_1, 0 AS ChiusuraPomeriggio_1, 0 AS ChiusuraMattino_2, 0 AS ChiusuraPomeriggio_2, '' AS Note, " _
                & "'' AS CodProvinciaAgente, 0 AS Agente_N_Prec, Codice_SEDE, Codice_Costo, FuoriZona, Modalita_Invio_Ordine, Data_Inizio_Chiusura, " _
                & "Data_Fine_Chiusura, 0 AS IVASosp, NoFatt, Rit_Acconto, Data_Nascita, StRit_Acconto, CodTributo, CIN, NazIBAN, CINEUIBAN, SWIFT, CSAggrAllIVA, " _
                & "'' AS CIG, '' AS CUP, InseritoDa, ModificatoDa, IBAN_Ditta, " _
                & "CodiceMABELL, EsportatoMABELL, '' AS IDTipoFatt, '' AS IPA, 0 AS SplitIVA, '' AS EmailInvioFatt, '' AS EmailInvioScad, 0 AS InvioMailScad, PECEmail  " _
                & "FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Fornitori"
            SQLCmd.CommandText = strSQL
            SQLCmd.ExecuteNonQuery()
            'DESTCLIENTI AGGIORNO la vista
            NomeFunz = "DestClienti"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW DestClienti AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.DestClienti"
            SQLCmd.ExecuteNonQuery()

            'DocumentiD_ALLANNI
            NomeFunz = "DocumentiD_ALLANNI"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            'giu240418 tutti gli esercizi crescente
            strSQL = "Select Ditta, Esercizio FROM Esercizi "
            strSQL += "WHERE Ditta = '" & strDitta.Trim & "' "
            strSQL += "ORDER BY Esercizio"
            '----------
            Dim ObjDB As New DataBaseUtility
            Dim dsEser As New DataSet
            Dim rsDett As DataRow
            Dim EserPrec As String = ""
            Try
                'giu291118 Aggiunto la Descrizione per ovviare al problema Stesso Codice Articolo ma Descrizione diversa
                'GIU090320 QTA_EVASA PER LE NC GLI CAMBIO DI SEGNO ALTRIMENTI MI SEGNALA CHE PER ES. HO FATTURATO PIU DI QUELLO ORDINATO
                'XKE' NON TIENE CONTO DELLA NC (RISCONTRATO CON ELENA)
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, dsEser)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA giu270722 */ " & _
                "CREATE VIEW DocumentiD_ALLANNI AS SELECT IDDocumenti, ISNULL(Cod_Articolo,'') AS Cod_Articolo, ISNULL(Descrizione,'') AS Descrizione, RefInt, SUM(Qta_Evasa) AS Qta_Evasa FROM ( "
                'giu040519 prendo anche le righe senza codice articolo
                If (dsEser.Tables.Count > 0) Then
                    If (dsEser.Tables(0).Rows.Count > 0) Then
                        For Each rsDett In dsEser.Tables(0).Select("")
                            If EserPrec <> "" Then
                                strSP += " UNION ALL "
                            End If
                            EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                            If EserPrec.Trim <> "" Then
                                strSP += " SELECT [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti, " & _
                                         "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, " & _
                                         "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, " & _
                                         "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.RefInt, "
                                strSP += "SUM(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc <> 'NC' THEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa * -1 END) AS Qta_Evasa "
                                strSP += "FROM [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD INNER JOIN "
                                strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti = [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.IDDocumenti "
                                'giu040519 strSP += "WHERE (NOT ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo IS NULL)) AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo <> N'') AND "
                                strSP += "WHERE (NOT ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.RefInt IS NULL)) AND ( [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa > 0) "
                                'GIU270722 
                                strSP += "AND (CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc='FC' THEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Acconto,0) ELSE 0 END =0) "
                                strSP += "AND (CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc='FC' THEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.DedPerAcconto,0) ELSE 0 END =0) "
                                strSP += "GROUP BY [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti, [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.RefInt "
                            End If
                        Next
                    Else
                        strSP += " SELECT [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti, " & _
                                 "[" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo, " & _
                                 "[" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Descrizione, " & _
                                 "[" & strDitta & strEser & "GestAzi].dbo.DocumentiT.RefInt, "
                        strSP += "SUM(CASE WHEN [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Tipo_Doc <> 'NC' THEN [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Qta_Evasa ELSE [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Qta_Evasa * -1 END) AS Qta_Evasa "
                        strSP += "FROM [" & strDitta & strEser & "GestAzi].dbo.DocumentiD INNER JOIN "
                        strSP += "[" & strDitta & strEser & "GestAzi].dbo.DocumentiT ON [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti = [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.IDDocumenti "
                        'giu040519 strSP += "WHERE (NOT ([" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo IS NULL)) AND ([" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo <> N'') AND "
                        strSP += "WHERE (NOT ([" & strDitta & strEser & "GestAzi].dbo.DocumentiT.RefInt IS NULL)) AND ( [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Qta_Evasa > 0) "
                        'GIU270722 strSP += "AND (ISNULL([" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Acconto,0) =0) AND (ISNULL([" & strDitta & strEser & "GestAzi].dbo.DocumentiD.DedPerAcconto,0) =0) "
                        strSP += "AND (CASE WHEN [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Tipo_Doc='FC' THEN ISNULL([" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Acconto,0) ELSE 0 END =0) "
                        strSP += "AND (CASE WHEN [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Tipo_Doc='FC' THEN ISNULL([" & strDitta & strEser & "GestAzi].dbo.DocumentiD.DedPerAcconto,0) ELSE 0 END =0) "
                        strSP += "GROUP BY [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti, [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo, [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Descrizione, [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.RefInt "
                    End If
                Else
                    strSP += " SELECT [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti, " & _
                                 "[" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo, " & _
                                 "[" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Descrizione, " & _
                                 "[" & strDitta & strEser & "GestAzi].dbo.DocumentiT.RefInt, "
                    strSP += "SUM(CASE WHEN [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Tipo_Doc <> 'NC' THEN [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Qta_Evasa ELSE [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Qta_Evasa * -1 END) AS Qta_Evasa "
                    strSP += "FROM [" & strDitta & strEser & "GestAzi].dbo.DocumentiD INNER JOIN "
                    strSP += "[" & strDitta & strEser & "GestAzi].dbo.DocumentiT ON [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti = [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.IDDocumenti "
                    'giu040519 strSP += "WHERE (NOT ([" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo IS NULL)) AND ([" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo <> N'') AND "
                    strSP += "WHERE (NOT ([" & strDitta & strEser & "GestAzi].dbo.DocumentiT.RefInt IS NULL)) AND ( [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Qta_Evasa > 0) "
                    'GIU270722 strSP += "AND (ISNULL([" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Acconto,0) =0) AND (ISNULL([" & strDitta & strEser & "GestAzi].dbo.DocumentiD.DedPerAcconto,0) =0) "
                    strSP += "AND (CASE WHEN [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Tipo_Doc='FC' THEN ISNULL([" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Acconto,0) ELSE 0 END =0) "
                    strSP += "AND (CASE WHEN [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.Tipo_Doc='FC' THEN ISNULL([" & strDitta & strEser & "GestAzi].dbo.DocumentiD.DedPerAcconto,0) ELSE 0 END =0) "
                    strSP += "GROUP BY [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti, [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo, [" & strDitta & strEser & "GestAzi].dbo.DocumentiD.Descrizione, [" & strDitta & strEser & "GestAzi].dbo.DocumentiT.RefInt "
                End If
                '--------------------------------------
                strSP += ") AS Expr1 GROUP BY IDDocumenti, ISNULL(Cod_Articolo,''), ISNULL(Descrizione,''), RefInt"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '------------------
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = "GoMenu"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup1.Show("Errore Ricollega tabelle", "Errore: <BR>" & Ex.Message.Trim & " <BR>" & NomeFunz, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Function
            End Try
            '- NomeFunz = "DocumentiD_AP"(AAAA)"
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    For Each rsDett In dsEser.Tables(0).Select("")
                        EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        If EserPrec.Trim <> "" Then
                            NomeFunz = "DocumentiD_AP" & EserPrec
                            strSP = "DROP VIEW " & NomeFunz
                            Call ExecDelete(SQLCmd, strSP)
                            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                            "CREATE VIEW DocumentiD_AP" & EserPrec & " AS  SELECT * FROM [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD"
                            SQLCmd.CommandText = strSP
                            SQLCmd.ExecuteNonQuery()
                            '------------------
                        End If
                    Next
                End If
            End If
            '-
            'giu230113 ricreare la vista di tutti i documenti anno precedente per l'evasione in questo
            'attenzione se è il primo anno (test su esercizi) select su se stesso ma con id= -1 (nessuno)
            NomeFunz = "DocumentiD_AP"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            EserPrec = CKAnnoPrec()
            If SWErrore Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "GoMenu"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup1.Show("Errore Ricollega tabelle", "Errore: <BR>" & myErrore & " <BR>" & NomeFunz, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Function
            End If
            If EserPrec.Trim <> "" Then
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW DocumentiD_AP AS " & _
                "SELECT [" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti, " & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, " & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiT.RefInt, " & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa " & _
                "FROM [" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiD INNER JOIN " & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiT ON " & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti = " & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiT.IDDocumenti " & _
                "WHERE (NOT ([" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo IS NULL)) AND (" & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo <> N'') AND (NOT ( " & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiT.RefInt IS NULL)) AND ( " & _
                "[" & Session(CSTCODDITTA) & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa > 0)"
            Else 'VUOTA
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW DocumentiD_AP AS " & _
                "SELECT [" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti, " & _
                "[" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiD.Cod_Articolo, " & _
                "[" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiT.RefInt, " & _
                "[" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiD.Qta_Evasa " & _
                "FROM [" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiD INNER JOIN " & _
                "[" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiT ON " & _
                "[" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti = " & _
                "[" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiT.IDDocumenti " & _
                "WHERE ([" & Session(CSTCODDITTA) & strEser & "GestAzi].dbo.DocumentiD.IDDocumenti = -1)"
            End If
            SQLCmd.ExecuteNonQuery()
            '-
            '- NomeFunz = [DocumentiT_ALLANNI]
            NomeFunz = "DocumentiT_ALLANNI"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            EserPrec = ""
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW DocumentiT_ALLANNI AS SELECT Cod_Cliente, SUM(TotaleDoc) AS TotaleDoc FROM ("
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    For Each rsDett In dsEser.Tables(0).Select("")
                        If EserPrec <> "" Then
                            strSP += " UNION ALL "
                        End If
                        EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        If EserPrec.Trim <> "" Then
                            strSP += "SELECT Cod_Cliente, COUNT(IDDocumenti) AS TotaleDoc "
                            strSP += "FROM [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT "
                            strSP += "WHERE ISNULL(Cod_Cliente,'')<>'' GROUP BY Cod_Cliente "
                        End If
                    Next
                End If
            End If
            strSP += ") AS Expr1 GROUP BY Cod_Cliente"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '---------------------------@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'giu300618 tutte le testate documentiT per documenti collegati e altro PS ULTIMO ANNO COLLEGO ANCHE OC PR
            NomeFunz = "DocumentiTALL_ALLANNI"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            EserPrec = ""
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW DocumentiTALL_ALLANNI AS SELECT * FROM ("
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    Dim UltEser As String = ""
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio DESC")
                        UltEser = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        Exit For
                    Next
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio")
                        If EserPrec <> "" Then
                            strSP += " UNION ALL "
                        End If
                        EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        If EserPrec.Trim <> "" Then
                            If EserPrec.Trim <> UltEser.Trim Then
                                strSP += "SELECT * FROM [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT "
                                strSP += "WHERE LEFT(Tipo_Doc,1) <> 'O' AND LEFT(Tipo_Doc,1) <> 'P' "
                            Else
                                strSP += "SELECT * FROM [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT "
                            End If
                        End If
                    Next
                End If
            End If
            strSP += ") AS Expr1"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '---------------------------@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'giu040619 modifica statistiche venduto su più anni e venduto per i clienti a cui abbiamo inviata ALERT consumabili
            'GIU060619 LE INNER JOIN CON TROPPE TABELLA FA SI CHE ESCLUDINO DEI CLIENTI - FATTO IN MODO DA NON FARLE MA SOLO DOPO LA PRIMA ESTRAZIONE
            NomeFunz = "VendutoClientiArticolo"
            strSP = "DROP FUNCTION " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            EserPrec = ""
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE FUNCTION VendutoClientiArticolo " & vbCr & _
            "(@DataDa nvarchar(10)=NULL,@DataA nvarchar(10)=NULL,@DataNC nvarchar(10)=NULL,@Cod1 nvarchar(20)=NULL,@Cod2 nvarchar(20)=NULL,@CodCliente nvarchar(16)=NULL,@CodAgente INT=NULL,@CodCatCli int=NULL)" & vbCr & _
            "RETURNS TABLE" & vbCr & _
            "AS" & vbCr & _
            "RETURN (" & vbCr & _
            "SELECT Codice_CoGe, Rag_Soc, Cod_Articolo, Descrizione, Sum(TotQta) As TotQta, PrezzoListino, PrezzoVendita, SUM(Imponibile) AS Imponibile, MAX(Data_Ultimo) AS Data_Ultimo, Segno,Agente, Cod_Categoria, Provincia " & vbCr & _
            "FROM ("
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    Dim UltEser As String = ""
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio DESC")
                        UltEser = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        Exit For
                    Next
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio")
                        If EserPrec <> "" Then
                            strSP += " UNION ALL " & vbCr
                        End If
                        EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        If EserPrec.Trim <> "" Then
                            strSP += "      SELECT  ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente, '') AS Codice_CoGe,ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Rag_Soc, '') AS Rag_Soc, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga) " & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo END AS Cod_Articolo, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione END AS Descrizione, " & vbCr
                            strSP += "SUM(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' " & vbCr
                            strSP += "THEN ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa)*-1 " & vbCr
                            strSP += "ELSE ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa) END) AS TotQta, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo AS PrezzoListino,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo_Netto AS PrezzoVendita, " & vbCr
                            strSP += "SUM(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' " & vbCr
                            strSP += "THEN ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Importo)*-1 " & vbCr
                            strSP += "ELSE ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Importo)END) AS Imponibile," & vbCr
                            strSP += "MAX([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc) AS Data_Ultimo, " & vbCr
                            strSP += "CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN ('-') ELSE ('+') END AS Segno," & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0) AS Agente, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0) AS Cod_Categoria, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Provincia, '') AS Provincia" & vbCr
                            strSP += "      FROM  [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT INNER JOIN" & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.IDDocumenti = [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Clienti ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente = [" & strDitta & UltEser & "GestAzi].dbo.Clienti.Codice_CoGe LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Agenti ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente = [" & strDitta & UltEser & "GestAzi].dbo.Agenti.Codice LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Categorie ON [" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria = [" & strDitta & UltEser & "GestAzi].dbo.Categorie.Codice LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.CausMag ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Causale = [" & strDitta & UltEser & "GestAzi].dbo.CausMag.Codice" & vbCr
                            strSP += "      WHERE ((([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'DT') AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 0 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 2)) OR "
                            strSP += "(([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'FC') AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 0 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 1 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 2)) OR " & vbCr
                            strSP += "(([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC') AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 0 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 1 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 2))) AND " & vbCr
                            strSP += "([" & strDitta & UltEser & "GestAzi].dbo.CausMag.Fatturabile <> 0) AND ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa, 0) <> 0 AND " & vbCr
                            strSP += "([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc >= CONVERT(datetime,@DataDa,103)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc <= " & vbCr
                            strSP += "(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN CONVERT(datetime,@DataNC,103) ELSE CONVERT(datetime,@DataA,103) END))" & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo >= COALESCE(@Cod1,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo <= COALESCE(@Cod2,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente = COALESCE(@CodCliente,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente))" & vbCr
                            strSP += "AND (ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0) = ISNULL(@CodAgente, ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0)))" & vbCr
                            strSP += "AND (ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0) = ISNULL(@CodCatCli, ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0)))" & vbCr
                            strSP += "      GROUP BY ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente, ''),ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Rag_Soc, '')," & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo END, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione END," & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo ,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo_Netto, " & vbCr
                            strSP += "CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN ('-') ELSE ('+') END," & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0), " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0), " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Provincia, '')" & vbCr
                        End If
                    Next
                End If
            End If
            strSP += ") AS UnionTabella" & vbCr
            strSP += "GROUP BY Codice_CoGe, Rag_Soc, Cod_Articolo, Descrizione, PrezzoListino, PrezzoVendita, Segno, Agente, Cod_Categoria,Provincia " & vbCr
            strSP += ")"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            'FATTURATO
            NomeFunz = "FatturatoClientiArticolo"
            strSP = "DROP FUNCTION " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            EserPrec = ""
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE FUNCTION FatturatoClientiArticolo " & vbCr & _
            "(@DataDa nvarchar(10)=NULL,@DataA nvarchar(10)=NULL,@DataNC nvarchar(10)=NULL,@Cod1 nvarchar(20)=NULL,@Cod2 nvarchar(20)=NULL,@CodCliente nvarchar(16)=NULL,@CodAgente INT=NULL,@CodCatCli int=NULL)" & vbCr & _
            "RETURNS TABLE" & vbCr & _
            "AS" & vbCr & _
            "RETURN (" & vbCr & _
            "SELECT Codice_CoGe, Rag_Soc, Cod_Articolo, Descrizione, Sum(TotQta) As TotQta, PrezzoListino, PrezzoVendita, SUM(Imponibile) AS Imponibile, MAX(Data_Ultimo) AS Data_Ultimo, Segno,Agente, Cod_Categoria,Provincia " & vbCr & _
            "FROM ("
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    Dim UltEser As String = ""
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio DESC")
                        UltEser = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        Exit For
                    Next
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio")
                        If EserPrec <> "" Then
                            strSP += " UNION ALL " & vbCr
                        End If
                        EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        If EserPrec.Trim <> "" Then
                            strSP += "      SELECT  ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente, '') AS Codice_CoGe,ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Rag_Soc, '') AS Rag_Soc, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga) " & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo END AS Cod_Articolo, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione END AS Descrizione, " & vbCr
                            strSP += "SUM(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' " & vbCr
                            strSP += "THEN ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa)*-1 " & vbCr
                            strSP += "ELSE ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa) END) AS TotQta, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo AS PrezzoListino,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo_Netto AS PrezzoVendita, " & vbCr
                            strSP += "SUM(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' " & vbCr
                            strSP += "THEN ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Importo)*-1 " & vbCr
                            strSP += "ELSE ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Importo)END) AS Imponibile," & vbCr
                            strSP += "MAX([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc) AS Data_Ultimo, " & vbCr
                            strSP += "CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN ('-') ELSE ('+') END AS Segno," & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0) AS Agente, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0) AS Cod_Categoria, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Provincia, '') AS Provincia" & vbCr
                            strSP += "      FROM  [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT INNER JOIN" & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.IDDocumenti = [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Clienti ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente = [" & strDitta & UltEser & "GestAzi].dbo.Clienti.Codice_CoGe LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Agenti ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente = [" & strDitta & UltEser & "GestAzi].dbo.Agenti.Codice LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Categorie ON [" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria = [" & strDitta & UltEser & "GestAzi].dbo.Categorie.Codice LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.CausMag ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Causale = [" & strDitta & UltEser & "GestAzi].dbo.CausMag.Codice" & vbCr
                            strSP += "      WHERE  ((([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'FC') AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 0 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 1 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 2)) OR " & vbCr
                            strSP += "(([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC') AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 0 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 1 OR [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 2))) AND " & vbCr
                            strSP += "([" & strDitta & UltEser & "GestAzi].dbo.CausMag.Fatturabile <> 0) AND ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa, 0) <> 0 AND " & vbCr
                            strSP += "([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc >= CONVERT(datetime,@DataDa,103)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc <= " & vbCr
                            strSP += "(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN CONVERT(datetime,@DataNC,103) ELSE CONVERT(datetime,@DataA,103) END))" & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo >= COALESCE(@Cod1,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo <= COALESCE(@Cod2,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente = COALESCE(@CodCliente,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente))" & vbCr
                            strSP += "AND (ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0) = ISNULL(@CodAgente, ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0)))" & vbCr
                            strSP += "AND (ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0) = ISNULL(@CodCatCli, ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0)))" & vbCr
                            strSP += "      GROUP BY ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente, ''),ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Rag_Soc, '')," & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo END, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione END," & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo ,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo_Netto, " & vbCr
                            strSP += "CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN ('-') ELSE ('+') END," & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0), " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0), " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Provincia, '')" & vbCr
                        End If
                    Next
                End If
            End If
            strSP += ") AS UnionTabella" & vbCr
            strSP += "GROUP BY Codice_CoGe, Rag_Soc, Cod_Articolo, Descrizione, PrezzoListino, PrezzoVendita, Segno, Agente, Cod_Categoria, Provincia " & vbCr
            strSP += ")"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            'DA FATTURARE 
            NomeFunz = "DaFatturareClientiArticolo"
            strSP = "DROP FUNCTION " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            EserPrec = ""
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE FUNCTION DaFatturareClientiArticolo " & vbCr & _
            "(@DataDa nvarchar(10)=NULL,@DataA nvarchar(10)=NULL,@DataNC nvarchar(10)=NULL,@Cod1 nvarchar(20)=NULL,@Cod2 nvarchar(20)=NULL,@CodCliente nvarchar(16)=NULL,@CodAgente INT=NULL,@CodCatCli int=NULL)" & vbCr & _
            "RETURNS TABLE" & vbCr & _
            "AS" & vbCr & _
            "RETURN (" & vbCr & _
            "SELECT Codice_CoGe, Rag_Soc, Cod_Articolo, Descrizione, Sum(TotQta) As TotQta, PrezzoListino, PrezzoVendita, SUM(Imponibile) AS Imponibile, MAX(Data_Ultimo) AS Data_Ultimo, Segno,Agente, Cod_Categoria,Provincia " & vbCr & _
            "FROM ("
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    Dim UltEser As String = ""
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio DESC")
                        UltEser = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        Exit For
                    Next
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio")
                        If EserPrec <> "" Then
                            strSP += " UNION ALL " & vbCr
                        End If
                        EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        If EserPrec.Trim <> "" Then
                            strSP += "      SELECT  ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente, '') AS Codice_CoGe,ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Rag_Soc, '') AS Rag_Soc, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga) " & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo END AS Cod_Articolo, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione END AS Descrizione, " & vbCr
                            strSP += "SUM(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' " & vbCr
                            strSP += "THEN ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa)*-1 " & vbCr
                            strSP += "ELSE ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa) END) AS TotQta, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo AS PrezzoListino,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo_Netto AS PrezzoVendita, " & vbCr
                            strSP += "SUM(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' " & vbCr
                            strSP += "THEN ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Importo)*-1 " & vbCr
                            strSP += "ELSE ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Importo)END) AS Imponibile," & vbCr
                            strSP += "MAX([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc) AS Data_Ultimo, " & vbCr
                            strSP += "CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN ('-') ELSE ('+') END AS Segno," & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0) AS Agente, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0) AS Cod_Categoria, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Provincia, '') AS Provincia" & vbCr
                            strSP += "      FROM  [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT INNER JOIN" & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.IDDocumenti = [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Clienti ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente = [" & strDitta & UltEser & "GestAzi].dbo.Clienti.Codice_CoGe LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Agenti ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente = [" & strDitta & UltEser & "GestAzi].dbo.Agenti.Codice LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Categorie ON [" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria = [" & strDitta & UltEser & "GestAzi].dbo.Categorie.Codice LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.CausMag ON [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Causale = [" & strDitta & UltEser & "GestAzi].dbo.CausMag.Codice" & vbCr
                            strSP += "      WHERE  ((([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'DT') AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.StatoDoc = 0))" & vbCr
                            strSP += ") AND " & vbCr
                            strSP += "([" & strDitta & UltEser & "GestAzi].dbo.CausMag.Fatturabile <> 0) AND ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa, 0) <> 0 AND " & vbCr
                            strSP += "([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc >= CONVERT(datetime,@DataDa,103)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc <= " & vbCr
                            strSP += "(CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN CONVERT(datetime,@DataNC,103) ELSE CONVERT(datetime,@DataA,103) END))" & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo >= COALESCE(@Cod1,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo <= COALESCE(@Cod2,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente = COALESCE(@CodCliente,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente))" & vbCr
                            strSP += "AND (ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0) = ISNULL(@CodAgente, ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0)))" & vbCr
                            strSP += "AND (ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0) = ISNULL(@CodCatCli, ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0)))" & vbCr
                            strSP += "      GROUP BY ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente, ''),ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Rag_Soc, '')," & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo END, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione END," & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo ,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Prezzo_Netto, " & vbCr
                            strSP += "CASE WHEN [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'NC' THEN ('-') ELSE ('+') END," & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0), " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0), " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Provincia, '')" & vbCr
                        End If
                    Next
                End If
            End If
            strSP += ") AS UnionTabella" & vbCr
            strSP += "GROUP BY Codice_CoGe, Rag_Soc, Cod_Articolo, Descrizione, PrezzoListino, PrezzoVendita, Segno, Agente, Cod_Categoria, Provincia " & vbCr
            strSP += ")"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-----------------------------------------------------------------------
            'GIU010923 ESTRAZIONE INSTALLATO PER CLIENTE
            'GIU080923 INCLUSO ANCHE LE FATTURE ACCOMPAGNATORIE
            NomeFunz = "InstallatoClientiArticolo"
            strSP = "DROP FUNCTION " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            EserPrec = ""
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE FUNCTION InstallatoClientiArticolo " & vbCr & _
            "(@DataDa nvarchar(10)=NULL,@DataA nvarchar(10)=NULL,@Cod1 nvarchar(20)=NULL,@Cod2 nvarchar(20)=NULL,@CodCliente nvarchar(16)=NULL,@CodFornitore nvarchar(16)=NULL,@CodAgente INT=NULL,@CodCatCli int=NULL)" & vbCr & _
            "RETURNS TABLE" & vbCr & _
            "AS" & vbCr & _
            "RETURN (" & vbCr & _
            "SELECT Codice_CoGe,Rag_Soc,Denominazione,Indirizzo,CAP,Localita,Provincia,Email,Telefono1,Telefono2," & _
            "Cod_Articolo,Descrizione,Tipo_Doc,Numero,Riga,Cod_Causale,Data_Doc,DataOraRitiro,Segno_Giacenza,Agente," & _
            "Cod_Categoria,Lotto,NSerie,CodiceFornitore " & vbCr & _
            "FROM ("
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    Dim UltEser As String = ""
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio DESC")
                        UltEser = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        Exit For
                    Next
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio")
                        If EserPrec <> "" Then
                            strSP += " UNION ALL " & vbCr
                        End If
                        EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        If EserPrec.Trim <> "" Then
                            strSP += "      SELECT  ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente, '') AS Codice_CoGe,ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Rag_Soc, '') AS Rag_Soc, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Denominazione, '') AS Denominazione, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Indirizzo, '') AS Indirizzo, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.CAP, '') AS CAP, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Localita, '') AS Localita, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Provincia, '') AS Provincia, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Email, '') AS Email, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Telefono1, '') AS Telefono1, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Telefono2, '') AS Telefono2, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM(" & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga) " & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo END AS Cod_Articolo, " & vbCr
                            strSP += "CASE WHEN ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione, '') = '' " & vbCr
                            strSP += "THEN '_' + [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc + '_' + RTRIM(LTRIM(" & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero)) + '_' + CONVERT(NVARCHAR(5), " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga)" & vbCr
                            strSP += "ELSE [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Descrizione END AS Descrizione, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Numero, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Causale, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc, " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.DataOraRitiro, " & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.CausMag.Segno_Giacenza, " & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0) AS Agente, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0) AS Cod_Categoria, " & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiDLotti.Lotto, '') AS Lotto," & vbCr
                            strSP += "ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiDLotti.NSerie, '') AS NSerie, " & vbCr
                            strSP += "ISNULL([" & strDitta & UltEser & "GestAzi].dbo.AnaMag.CodiceFornitore, '') AS CodiceFornitore " & vbCr
                            strSP += "      FROM  [" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT INNER JOIN " & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD ON " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.IDDocumenti = " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti LEFT OUTER JOIN " & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.AnaMag ON " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo = " & _
                            "[" & strDitta & UltEser & "GestAzi].dbo.AnaMag.Cod_Articolo LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiDLotti ON " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Riga = " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiDLotti.Riga AND " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.IDDocumenti = " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiDLotti.IDDocumenti LEFT OUTER JOIN" & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Clienti ON " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente = " & _
                            "[" & strDitta & UltEser & "GestAzi].dbo.Clienti.Codice_CoGe LEFT OUTER JOIN " & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Agenti ON " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente = " & _
                            "[" & strDitta & UltEser & "GestAzi].dbo.Agenti.Codice LEFT OUTER JOIN " & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.Categorie ON " & _
                            "[" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria = " & _
                            "[" & strDitta & UltEser & "GestAzi].dbo.Categorie.Codice LEFT OUTER JOIN " & vbCr
                            strSP += "[" & strDitta & UltEser & "GestAzi].dbo.CausMag ON " & _
                            "[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Causale = " & _
                            "[" & strDitta & UltEser & "GestAzi].dbo.CausMag.Codice" & vbCr
                            strSP += "      WHERE (  ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'DT')" & vbCr
                            strSP += "  OR  ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Tipo_Doc = 'FC' AND  ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.FatturaAC,0)<>0 )  AND (ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.ScGiacenza,0)<>0 )  )" & vbCr
                            strSP += " AND " & vbCr
                            strSP += "([" & strDitta & UltEser & "GestAzi].dbo.CausMag.Fatturabile <> 0) " & _
                            " AND ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Qta_Evasa, 0) <> 0 AND " & vbCr
                            strSP += "([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc >= CONVERT(datetime,@DataDa,103)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Data_Doc <= CONVERT(datetime,@DataA,103)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo >= " & _
                            "COALESCE(@Cod1,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo <= " & _
                            "COALESCE(@Cod2,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiD.Cod_Articolo)) " & vbCr
                            strSP += "AND ([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente = " & _
                            "COALESCE(@CodCliente,[" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Cliente))" & vbCr
                            strSP += "AND (ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0) = " & _
                            "ISNULL(@CodAgente, ISNULL([" & strDitta & EserPrec & "GestAzi].dbo.DocumentiT.Cod_Agente, 0)))" & vbCr
                            strSP += "AND (ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0) = " & _
                            "ISNULL(@CodCatCli, ISNULL([" & strDitta & UltEser & "GestAzi].dbo.Clienti.Categoria, 0)))" & vbCr
                        End If
                    Next
                End If
            End If
            strSP += ") AS UnionTabella" & vbCr
            strSP += ")"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-----------------------------------------------------------------------
            'giu290618
            NomeFunz = "EmailInviateT"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW EmailInviateT AS SELECT * FROM [" & strDitta & "Scadenze].dbo.EmailInviateT"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "EmailInviateDett"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW EmailInviateDett AS SELECT * FROM [" & strDitta & "Scadenze].dbo.EmailInviateDett"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '---------
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-----------------------------------------------------------------------
            'giu011020 GESTIONE CONTRATTI
            NomeFunz = "ContrattiT"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW ContrattiT AS SELECT * FROM [" & strDitta & "Scadenze].dbo.ContrattiT"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "ContrattiD"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW ContrattiD AS SELECT * FROM [" & strDitta & "Scadenze].dbo.ContrattiD"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "ContrattiDLotti"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW ContrattiDLotti AS SELECT * FROM [" & strDitta & "Scadenze].dbo.ContrattiDLotti"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '---------
            'FORNITORI AGGIORNO la vista
            NomeFunz = "Fornitori"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW Fornitori AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Fornitori"
            SQLCmd.ExecuteNonQuery()
            '--------
            'PAGAMENTI AGGIORNO la vista
            NomeFunz = "Pagamenti"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW Pagamenti AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Pagamenti"
            SQLCmd.ExecuteNonQuery()
            'PROVINCE AGGIORNO la vista
            NomeFunz = "Province"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW Province AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Province"
            SQLCmd.ExecuteNonQuery()
            'REGIONI AGGIORNO la vista
            NomeFunz = "Regioni"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW Regioni AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Regioni"
            SQLCmd.ExecuteNonQuery()
            'VETTORI AGGIORNO la vista
            NomeFunz = "Vettori"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
            "CREATE VIEW Vettori AS SELECT * FROM [" & Session(CSTCODDITTA) & Session(ESERCIZIO) & "CoGe].dbo.Vettori"
            SQLCmd.ExecuteNonQuery()
            '-@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'GIU070618 ATTENZIONE CAMBIO CONNESSIONE SCADENZARIO
            '-@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            If SQLConn.State = ConnectionState.Open Then
                SQLConn.Close()
            End If
            '- NomeFunz = "AnaMag"(AAAA)" VISTA IN SCADENZE PER LA GESTIONE ArticoliInst_ContrattiAss
            SQLConn.ConnectionString = dbcon.getConnectionString(TipoDB.dbScadenzario)
            SQLCmd.CommandType = CommandType.Text
            SQLCmd.Connection = SQLConn
            If SQLConn.State <> ConnectionState.Open Then
                SQLConn.Open()
            End If
            For myanno = 2000 To Year(Now)
                NomeFunz = "AnaMag" & Trim(Str(myanno))
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
            Next
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    For Each rsDett In dsEser.Tables(0).Select("", "Esercizio")
                        EserPrec = IIf(IsDBNull(rsDett!Esercizio), "", rsDett!Esercizio)
                        If EserPrec.Trim <> "" Then
                            NomeFunz = "AnaMag" & EserPrec
                            strSP = "DROP VIEW " & NomeFunz
                            Call ExecDelete(SQLCmd, strSP)
                            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                            "CREATE VIEW AnaMag" & EserPrec & " AS  SELECT * FROM [" & strDitta & EserPrec & "GestAzi].dbo.AnaMag"
                            SQLCmd.CommandText = strSP
                            SQLCmd.ExecuteNonQuery()
                            '------------------
                        End If
                    Next
                End If
            End If
            'giu111219 SEMPRE ULTIMO ANNO SEMPRE giu050619 mancava 
            '- cancello tutti i riferimenti e li ricreo - SE AGGIUNGO ALTRO AGGIUNGERE LA CANCELLAZIONE QUI DI SEGUITO
            NomeFunz = "Clienti"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "Fornitori"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "AnagrProvv"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "DestClienti"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "Province"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "CausMag"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "CausNonEvasione"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            'giu020320
            NomeFunz = "DocTUltimo"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "DocDUltimo"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "Update_EmailInviateT_IdMod"
            strSP = "DROP PROCEDURE " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '- ok cancellati 
            If Not String.IsNullOrEmpty(EserPrec) Then 'giu111219
                NomeFunz = "Clienti"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW Clienti AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "CoGe].dbo.Clienti"
                SQLCmd.ExecuteNonQuery()
                'GIU271119
                NomeFunz = "Fornitori"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW Fornitori AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "CoGe].dbo.Fornitori"
                SQLCmd.ExecuteNonQuery()
                'GIU281119
                NomeFunz = "AnagrProvv"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW AnagrProvv AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "GestAzi].dbo.AnagrProvv"
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "DestClienti"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW DestClienti AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "CoGe].dbo.DestClienti"
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Province"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW Province AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "CoGe].dbo.Province"
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "CausMag"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW CausMag AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "GestAzi].dbo.CausMag"
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "CausNonEvasione"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW CausNonEvasione AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "GestAzi].dbo.CausNonEvasione"
                SQLCmd.ExecuteNonQuery()
                '---------
                'giu020320
                NomeFunz = "DocTUltimo"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW DocTUltimo AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "GestAzi].dbo.DocumentiT"
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "DocDUltimo"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                SQLCmd.CommandText = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                "CREATE VIEW DocdUltimo AS SELECT * FROM [" & Session(CSTCODDITTA) & EserPrec.Trim & "GestAzi].dbo.DocumentiD"
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Update_EmailInviateT_IdMod"
                strSP = "DROP PROCEDURE " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                '- giu300818 Tolto l'aggiornamento , NReInvio= ISNULL(NReInvio,0) + 1 VERRA' FATTO DA INVIO E-MAIL ALTRIMENTI RADDOPPIA QUANDO LE SCADENZE SONO 2 SUL SINGOLO ARTICOLO
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */" & vbCr & _
                    "CREATE PROCEDURE [dbo].[Update_EmailInviateT_IdMod]" & vbCr & _
                    "(@ID int,@Anno int,@Stato int,@Email nvarchar(100),@Note ntext," & vbCr & _
                    "@IdModulo1 int,@IdModulo2 int,@IdModulo3 int,@IdModulo4 int,@Mittente nvarchar(100),@RetVal int = 0 output)" & vbCr & _
                    "AS SET NOCOUNT ON;" & vbCr & "DECLARE @Cod_Articolo NVarChar(20)" & vbCr & "SET @Cod_Articolo=NULL" & vbCr & _
                    "DECLARE @IdArticoliInst INT, @FlagGa bit, @FlagEl bit, @FlagBa bit" & vbCr & _
                    "DECLARE @Data1InvioScadGa datetime, @Data1InvioScadEl datetime, @Data1InvioScadBa datetime" & vbCr & _
                    "DECLARE dsArticoli SCROLL CURSOR FOR" & vbCr & _
                    "SELECT ArticoliInst_ContrattiAss.Cod_Articolo," & vbCr & _
                    "AnaMag" & EserPrec & ".IdModulo1, AnaMag" & EserPrec & ".IdModulo2, AnaMag" & EserPrec & ".IdModulo3, AnaMag" & EserPrec & ".IdModulo4" & vbCr & _
                    "FROM ArticoliInst_ContrattiAss INNER JOIN" & vbCr & _
                    "EmailInviateDett ON ArticoliInst_ContrattiAss.ID = EmailInviateDett.IdArticoliInst_ContrattiAss INNER JOIN" & vbCr & _
                    "AnaMag" & EserPrec & " ON ArticoliInst_ContrattiAss.Cod_Articolo = AnaMag" & EserPrec & ".Cod_Articolo" & vbCr & _
                    "WHERE @ID = EmailInviateDett.IdEmailInviateT" & vbCr & _
                    "GROUP BY ArticoliInst_ContrattiAss.Cod_Articolo," & vbCr & _
                    "AnaMag" & EserPrec & ".IdModulo1, AnaMag" & EserPrec & ".IdModulo2, AnaMag" & EserPrec & ".IdModulo3, AnaMag" & EserPrec & ".IdModulo4" & vbCr & _
                    "OPEN dsArticoli" & vbCr & _
                    "FETCH NEXT FROM dsArticoli INTO @Cod_Articolo, @IdModulo1, @IdModulo2, @IdModulo3, @IdModulo4" & vbCr & _
                    "WHILE @@FETCH_STATUS = 0 " & vbCr & "BEGIN" & vbCr & _
                    "IF ISNULL(@IdModulo1,0)=0 SET @IdModulo1=NULL" & vbCr & _
                    "IF ISNULL(@IdModulo2,0)=0 SET @IdModulo2=NULL" & vbCr & _
                    "IF ISNULL(@IdModulo3,0)=0 SET @IdModulo3=NULL" & vbCr & _
                    "IF ISNULL(@IdModulo4,0)=0 SET @IdModulo4=NULL" & vbCr & _
                    "UPDATE EmailInviateT SET IdModulo1=ISNULL(@IdModulo1,IdModulo1), IdModulo2=ISNULL(@IdModulo2,IdModulo2), " & _
                    "IdModulo3=ISNULL(@IdModulo3,IdModulo3), IdModulo4=ISNULL(@IdModulo4,IdModulo4),Mittente=@Mittente " & vbCr & _
                    "WHERE ID=@ID" & vbCr & _
                    "IF (@@ERROR <> 0)" & vbCr & "BEGIN" & vbCr & "SET @RetVal=-1" & vbCr & "RETURN @RetVal" & vbCr & "END" & vbCr & _
                    "FETCH NEXT FROM dsArticoli INTO @Cod_Articolo, @IdModulo1, @IdModulo2, @IdModulo3, @IdModulo4" & vbCr & "END" & vbCr & _
                    "CLOSE dsArticoli" & vbCr & "DEALLOCATE dsArticoli" & vbCr & _
                    "/* AGGIORNO LA TABELLA ArticoliInst_ContrattiAss.Data1InvioScad(Ga) DIPENDE DAL FLAG IN EmailInviateT E DALLA DATA DI SELEZIONE (31/08/2018) */" & vbCr & _
                    "DECLARE dsArticoliInst SCROLL CURSOR FOR" & vbCr & _
                    "SELECT IdArticoliInst_ContrattiAss, Data1InvioScadGa, FlagGa, Data1InvioScadEl, FlagEl, Data1InvioScadBa, FlagBa" & vbCr & _
                    "FROM EmailInviateDett INNER JOIN ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID" & vbCr & _
                    "WHERE IdEmailInviateT=@ID" & vbCr & _
                    "OPEN dsArticoliInst" & vbCr & _
                    "FETCH NEXT FROM dsArticoliInst INTO @IdArticoliInst, @Data1InvioScadGa, @FlagGa, @Data1InvioScadEl, @FlagEl, @Data1InvioScadBa, @FlagBa" & vbCr & _
                    "WHILE @@FETCH_STATUS = 0 " & vbCr & "BEGIN" & vbCr & _
                    "IF ISNULL(@FlagGa,0)<>0 AND NOT @Data1InvioScadGa IS NULL" & vbCr & "UPDATE ArticoliInst_ContrattiAss SET Data2InvioScadGa=GETDATE()" & vbCr & "WHERE ID=@IdArticoliInst AND NOT DataScadGaranzia IS NULL" & vbCr & _
                    "IF ISNULL(@FlagGa,0)<>0 AND @Data1InvioScadGa IS NULL    " & vbCr & "UPDATE ArticoliInst_ContrattiAss SET Data1InvioScadGa=GETDATE()" & vbCr & "WHERE ID=@IdArticoliInst AND NOT DataScadGaranzia IS NULL" & vbCr & _
                    "IF ISNULL(@FlagEl,0)<>0 AND NOT @Data1InvioScadEl IS NULL" & vbCr & "UPDATE ArticoliInst_ContrattiAss SET Data2InvioScadEl=GETDATE()" & vbCr & "WHERE ID=@IdArticoliInst AND NOT DataScadElettrodi IS NULL" & vbCr & _
                    "IF ISNULL(@FlagEl,0)<>0 AND @Data1InvioScadEl IS NULL    " & vbCr & "UPDATE ArticoliInst_ContrattiAss SET Data1InvioScadEl=GETDATE()" & vbCr & "WHERE ID=@IdArticoliInst AND NOT DataScadElettrodi IS NULL" & vbCr & _
                    "IF ISNULL(@FlagBa,0)<>0 AND NOT @Data1InvioScadBa IS NULL" & vbCr & "UPDATE ArticoliInst_ContrattiAss SET Data2InvioScadBa=GETDATE()" & vbCr & "WHERE ID=@IdArticoliInst AND NOT DataScadBatterie IS NULL" & vbCr & _
                    "IF ISNULL(@FlagBa,0)<>0 AND @Data1InvioScadBa IS NULL    " & vbCr & "UPDATE ArticoliInst_ContrattiAss SET Data1InvioScadBa=GETDATE()" & vbCr & "WHERE ID=@IdArticoliInst AND NOT DataScadBatterie IS NULL" & vbCr & _
                    "IF (@@ERROR <> 0)" & vbCr & "BEGIN" & vbCr & "SET @RetVal=-1" & vbCr & "RETURN @RetVal" & vbCr & "END" & vbCr & _
                    "FETCH NEXT FROM dsArticoliInst INTO @IdArticoliInst, @Data1InvioScadGa, @FlagGa, @Data1InvioScadEl, @FlagEl, @Data1InvioScadBa, @FlagBa" & vbCr & "END" & vbCr & _
                    "CLOSE dsArticoliInst" & vbCr & "DEALLOCATE dsArticoliInst" & vbCr & _
                    "SELECT * FROM EmailInviateT WHERE ID = @ID"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '------------------
            Else
                SWErrore = True
            End If
            '---------
            If SQLConn.State = ConnectionState.Open Then
                SQLConn.Close()
            End If
            ObjDB = Nothing
        Catch ex As Exception
            SWErrore = True
            myErrore = ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = "GoMenu"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup1.Show("Errore Ricollega tabelle (CoGe/GesAzi/Scadenzario)", "Errore (CoGe/GesAzi/Scadenzario): <BR>" & myErrore.Trim & " <BR>" & NomeFunz, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Function
        End Try
        If SWErrore = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CollegaTabelleCOGE"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup1.Show("Operazione completata (CoGe/GesAzi/Scadenzario)", "Ricollegamento viste completato correttamente (CoGe/GesAzi/Scadenzario).<BR>" & _
                             "Proseguo con il collegamento viste per (SoftCoGe).", WUC_ModalPopup.TYPE_CONFIRM_Y)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "GoMenu"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup1.Show("Errore Ricollega tabelle (CoGe/GesAzi/Scadenzario)", "Errore (CoGe/GesAzi/Scadenzario): <BR>" & myErrore.Trim & " <BR>" & NomeFunz, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Function
        End If
    End Function
    'GIU040918 QUI COLLEGO I DATI DI COGE ATTENZIONE SE SI MODIFICA QUI MODIFICARE ANCHE SOFTCOGESQL
    Public Function CollegaTabelleCOGE() As Boolean

        'GIU040918 QUI COLLEGO I DATI DI COGE ATTENZIONE SE SI MODIFICA QUI MODIFICARE ANCHE SOFTCOGESQL
        'BasMain.CollegaTabelle @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim strDitta As String = Session(CSTCODDITTA)
        If IsNothing(strDitta) Then
            strDitta = ""
        End If
        If String.IsNullOrEmpty(strDitta) Then
            strDitta = ""
        End If
        If strDitta = "" Or Not IsNumeric(strDitta) Then
            Chiudi("Errore: CODICE DITTA SCONOSCIUTO")
            Exit Function
        End If
        '-
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Function
        End If
        Dim dbcon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SQLConn As New SqlConnection
        Dim SQLCmd As New SqlCommand
        Dim strSQL As String
        Dim strSP As String

        'giu040918 DA QUI PARTE I COLLEGAMENTI DI COGE
        SQLConn.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftCoge)
        SQLCmd.CommandType = CommandType.Text
        SQLCmd.Connection = SQLConn
        SWErrore = False
        myErrore = "" 'giu230113
        Try
            If SQLConn.State <> ConnectionState.Open Then
                SQLConn.Open()
            End If
            'INIZIO Lettura Esercizi Azienda: strDitta
            NomeFunz = "Lettura Esercizi Azienda: (" & strDitta.Trim & ")"
            'giu240418 tutti gli esercizi crescente
            strSQL = "Select Ditta, Esercizio FROM Esercizi "
            strSQL += "WHERE Ditta = '" & strDitta.Trim & "' "
            strSQL += "ORDER BY Esercizio"
            '----------
            Dim ObjDB As New DataBaseUtility
            Dim dsEser As New DataSet
            Dim rsDett As DataRow = Nothing
            Dim EserPrec As String = ""
            Dim EserSucc As String = ""
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, dsEser)
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = "GoMenu"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup1.Show("Errore Ricollega tabelle (COGE)", "Errore (COGE): <BR>" & Ex.Message.Trim & " <BR>" & NomeFunz, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Function
            End Try
            '------------------------------------------
            NomeFunz = "Scadenze"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW Scadenze AS SELECT * FROM [" & strDitta & "Scadenze].dbo.Scadenze"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "Banche"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW Banche AS SELECT * FROM [" & strDitta & "Scadenze].dbo.Banche"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "Filiali"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW Filiali AS SELECT * FROM [" & strDitta & "Scadenze].dbo.Filiali"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "BudgetDef"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW BudgetDef AS SELECT * FROM [" & strDitta & "Scadenze].dbo.BudgetDef"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "BudgetDefC"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW BudgetDefC AS SELECT * FROM [" & strDitta & "Scadenze].dbo.BudgetDefC"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "BudgetPdC"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW BudgetPdC AS SELECT * FROM [" & strDitta & "Scadenze].dbo.BudgetPdC"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "UffIVA"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW UffIVA AS SELECT * FROM [" & strDitta & "Scadenze].dbo.UffIVA"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            NomeFunz = "MovContRA"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW MovContRA AS SELECT * FROM [" & strDitta & "Scadenze].dbo.MovContRA"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '-
            EserPrec = CKAnnoPrec()
            If EserPrec.Trim <> "" Then
                NomeFunz = "CeeD_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW CeeD_AP AS SELECT * FROM [" & strDitta & EserPrec & "CoGe].dbo.CeeD"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "PianoDeiConti_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW PianoDeiConti_AP AS SELECT * FROM [" & strDitta & EserPrec & "CoGe].dbo.PianoDeiConti"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Clienti_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW Clienti_AP AS SELECT * FROM [" & strDitta & EserPrec & "CoGe].dbo.Clienti"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Fornitori_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW Fornitori_AP AS SELECT * FROM [" & strDitta & EserPrec & "CoGe].dbo.Fornitori"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Cee_ArchBas_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSQL = "SELECT Codice_CoGe, Ragg_E_P AS Ragg, E_P, Descrizione, Saldo_Dare, Saldo_Avere, Dare_Chiusura, Avere_Chiusura, Saldo_Dare_2, Saldo_Avere_2, Dare_Chiusura_2 , Avere_Chiusura_2 " _
                & "From dbo.PianoDeiConti_AP Union ALL " _
                & "SELECT Codice_CoGe, Ragg_P AS Ragg, 'P' AS Expr1, Rag_Soc AS Descrizione, Saldo_Dare, Saldo_Avere, Dare_Chiusura, Avere_Chiusura, Saldo_Dare_2, Saldo_Avere_2 , Dare_Chiusura_2, Avere_Chiusura_2 " _
                & "From dbo.Clienti_AP Union ALL " _
                & "SELECT Codice_CoGe, Ragg_P AS Ragg, 'P' AS Expr1, Rag_Soc AS Descrizione, Saldo_Dare, Saldo_Avere, Dare_Chiusura, Avere_Chiusura, Saldo_Dare_2, Saldo_Avere_2 , Dare_Chiusura_2, Avere_Chiusura_2 " _
                & "From dbo.Fornitori_AP"
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW Cee_ArchBas_AP AS " & strSQL & ""
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
            Else
                NomeFunz = "CeeD_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW CeeD_AP AS SELECT * FROM [" & strDitta & strEser & "CoGe].dbo.CeeD WHERE (Codice IS NULL)"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "PianoDeiConti_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW PianoDeiConti_AP AS SELECT * FROM [" & strDitta & strEser & "CoGe].dbo.PianoDeiConti WHERE (Codice_CoGe IS NULL)"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Clienti_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW Clienti_AP AS SELECT * FROM [" & strDitta & strEser & "CoGe].dbo.Clienti WHERE (Codice_CoGe IS NULL)"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Fornitori_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW Fornitori_AP AS SELECT * FROM [" & strDitta & strEser & "CoGe].dbo.Fornitori WHERE (Codice_CoGe IS NULL)"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Cee_ArchBas_AP"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSQL = "SELECT Codice_CoGe, Ragg_E_P AS Ragg, E_P, Descrizione, Saldo_Dare, Saldo_Avere, Dare_Chiusura, Avere_Chiusura, Saldo_Dare_2, Saldo_Avere_2, Dare_Chiusura_2 , Avere_Chiusura_2 " _
                & "From dbo.PianoDeiConti_AP Union ALL " _
                & "SELECT Codice_CoGe, Ragg_P AS Ragg, 'P' AS Expr1, Rag_Soc AS Descrizione, Saldo_Dare, Saldo_Avere, Dare_Chiusura, Avere_Chiusura, Saldo_Dare_2, Saldo_Avere_2 , Dare_Chiusura_2, Avere_Chiusura_2 " _
                & "From dbo.Clienti_AP Union ALL " _
                & "SELECT Codice_CoGe, Ragg_P AS Ragg, 'P' AS Expr1, Rag_Soc AS Descrizione, Saldo_Dare, Saldo_Avere, Dare_Chiusura, Avere_Chiusura, Saldo_Dare_2, Saldo_Avere_2 , Dare_Chiusura_2, Avere_Chiusura_2 " _
                & "From dbo.Fornitori_AP"
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW Cee_ArchBas_AP AS " & strSQL & ""
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
            End If
            'ANNO SUCCESSIVO COGE
            EserSucc = CKAnnoSucc()
            If EserSucc.Trim <> "" Then
                NomeFunz = "PianoDeiConti_AS"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW PianoDeiConti_AS AS SELECT * FROM [" & strDitta & EserSucc & "CoGe].dbo.PianoDeiConti"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Clienti_AS"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW Clienti_AS AS SELECT * FROM [" & strDitta & EserSucc & "CoGe].dbo.Clienti"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
                NomeFunz = "Fornitori_AS"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                               "CREATE VIEW Fornitori_AS AS SELECT * FROM [" & strDitta & EserSucc & "CoGe].dbo.Fornitori"
                SQLCmd.CommandText = strSP
                SQLCmd.ExecuteNonQuery()
                '-
            Else
                NomeFunz = "PianoDeiConti_AS"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                '-
                NomeFunz = "Clienti_AS"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
                '-
                NomeFunz = "Fornitori_AS"
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
            End If
            '-
            NomeFunz = "AnagrProvv"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            strSP = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                           "CREATE VIEW AnagrProvv AS SELECT * FROM [" & strDitta & strEser & "GestAzi].dbo.AnagrProvv"
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
            '- MovContAAAA
            For i = 2000 To Year(Now)
                NomeFunz = "MovCont" & CStr(i)
                strSP = "DROP VIEW " & NomeFunz
                Call ExecDelete(SQLCmd, strSP)
            Next
            '-
            Dim Union As Boolean = False
            Dim SQLStrMovCont As String = ""
            Dim SQLStrMovimenti As String = ""
            Dim SQLStrMovimentiSC As String = ""
            Dim SQLStrClienti As String = ""
            NomeFunz = "DocumentiT_TuttiEser"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "Movimenti"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "MovimentiSC"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            NomeFunz = "Clienti_UltimoEser"
            strSP = "DROP VIEW " & NomeFunz
            Call ExecDelete(SQLCmd, strSP)
            '-
            If (dsEser.Tables.Count > 0) Then
                If dsEser.Tables(0).Select("").Length = 0 Then
                    myErrore = "Errore: Nessun esercizio è presente per la società: '" & strDitta & "' ."
                    SWErrore = False
                    GoTo Prosegui01
                End If
            Else
                myErrore = "Errore: Nessun esercizio è presente per la società: '" & strDitta & "' ."
                SWErrore = False
                GoTo Prosegui01
            End If
            
            Union = False
            strSQL = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                     "CREATE VIEW dbo.DocumentiT_TuttiEser AS "
            SQLStrMovimenti = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                     "CREATE VIEW dbo.Movimenti AS "
            SQLStrMovimentiSC = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                     "CREATE VIEW MovimentiSC AS "
            SQLStrClienti = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                     "CREATE VIEW dbo.Clienti_UltimoEser AS "
            'giu030220
            Dim EserMax As String = GetEserMax()
            If EserMax.Trim = "" Then
                myErrore = "Errore (GetEserMax): Nessun esercizio è presente per la società: '" & strDitta & "' ."
                SWErrore = False
                GoTo Prosegui01
            End If
            For Each rsDett In dsEser.Tables(0).Select("")
                If Union Then
                    strSQL = strSQL & "UNION ALL "
                    SQLStrMovimenti = SQLStrMovimenti & "UNION ALL "
                    SQLStrMovimentiSC = SQLStrMovimentiSC & "UNION ALL "
                End If
                'GIU030220 strSQL = strSQL & "SELECT * FROM [" & strDitta & rsDett!Esercizio & "GestAzi].dbo.DocumentiT "
                If rsDett!Esercizio.Trim <> EserMax.Trim Then
                    strSQL += "SELECT * FROM [" & strDitta & rsDett!Esercizio & "GestAzi].dbo.DocumentiT "
                    strSQL += "WHERE LEFT(Tipo_Doc,1) <> 'O' AND LEFT(Tipo_Doc,1) <> 'P' "
                Else
                    strSQL += "SELECT * FROM [" & strDitta & rsDett!Esercizio & "GestAzi].dbo.DocumentiT "
                End If
                '---------
                SQLStrMovCont = "/* ATTENZIONE GENERATA DA UTILITY RICOLLEGA */ " & _
                                "CREATE VIEW MovCont" & rsDett!Esercizio & " AS SELECT * FROM [" & strDitta & rsDett!Esercizio & "CoGe].dbo.MovCont"
                SQLCmd.CommandText = SQLStrMovCont
                SQLCmd.ExecuteNonQuery()

                SQLStrMovimenti = SQLStrMovimenti & "SELECT * FROM MovCont" & rsDett!Esercizio & " "
                SQLStrMovimentiSC = SQLStrMovimentiSC & "SELECT Data_Reg, Sezione, Importo, ImportoVal1, Codice_Coge FROM MovCont" & rsDett!Esercizio & " "

                Union = True
                'GIU030220 EserMax = rsDett!Esercizio
            Next
            '-
            SQLCmd.CommandText = strSQL
            SQLCmd.ExecuteNonQuery()
            '-
            SQLCmd.CommandText = SQLStrMovimenti
            SQLCmd.ExecuteNonQuery()
            '-
            SQLCmd.CommandText = SQLStrMovimentiSC
            SQLCmd.ExecuteNonQuery()
            '-
            SQLStrClienti = SQLStrClienti & "SELECT * FROM [" & strDitta & EserMax.Trim & "CoGe].dbo.Clienti "
            SQLCmd.CommandText = SQLStrClienti
            SQLCmd.ExecuteNonQuery()
            '-
            'FINE CICLO SU ESERCIZI
Prosegui01:

Termina:
            'OK TERMINATO @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            If SQLConn.State = ConnectionState.Open Then
                SQLConn.Close()
            End If
            ObjDB = Nothing
        Catch ex As Exception
            SWErrore = True
            myErrore = ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = "GoMenu"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup1.Show("Errore Ricollega tabelle (COGE)", "Errore (COGE): <BR>" & myErrore.Trim & " <BR>" & NomeFunz, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Function
        End Try
        If SWErrore = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "GoMenu"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup1.Show("Operazione completata (COGE)", "Ricollegamento viste (COGE) completato correttamente.", WUC_ModalPopup.TYPE_CONFIRM_Y)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "GoMenu"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup1.Show("Errore Ricollega tabelle (COGE)", "Errore (COGE): <BR>" & myErrore.Trim & " <BR>" & NomeFunz, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Function
        End If

    End Function

    'giu240418
    Public Sub GoMenu()
        Try
            Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale")
            Exit Sub
        Catch ex As Exception
            Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale")
            Exit Sub
        End Try
    End Sub
    'giu230113
    Private Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        Else
            Try
                Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale")
                Exit Sub
            End Try
            Exit Sub
        End If
    End Sub
    Private Function CKAnnoPrec() As String
        CKAnnoPrec = ""
        myErrore = ""
        '-
        Dim strDitta As String = Session(CSTCODDITTA)
        If IsNothing(strDitta) Then
            strDitta = ""
        End If
        If String.IsNullOrEmpty(strDitta) Then
            strDitta = ""
        End If
        If strDitta = "" Or Not IsNumeric(strDitta) Then
            Chiudi("Errore: CODICE DITTA SCONOSCIUTO")
            Exit Function
        End If
        '-
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Function
        End If
        Dim EserPrec As String = Format(Val(strEser) - 1, "0000")
        '---
        Dim strSQL As String = ""
        strSQL = "Select Ditta, Esercizio FROM Esercizi "
        strSQL += "WHERE Ditta = '" & strDitta.Trim & "'"
        strSQL += " AND Esercizio = '" & EserPrec.Trim & "'"
        '----------
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CKAnnoPrec = EserPrec
                    Exit Function
                Else
                    CKAnnoPrec = ""
                    Exit Function
                End If
            Else
                CKAnnoPrec = ""
                Exit Function
            End If
        Catch Ex As Exception
            CKAnnoPrec = ""
            SWErrore = True
            myErrore = Ex.Message
            Exit Function
        End Try

    End Function
    'giu050918
    Private Function CKAnnoSucc() As String
        CKAnnoSucc = ""
        myErrore = ""
        '-
        Dim strDitta As String = Session(CSTCODDITTA)
        If IsNothing(strDitta) Then
            strDitta = ""
        End If
        If String.IsNullOrEmpty(strDitta) Then
            strDitta = ""
        End If
        If strDitta = "" Or Not IsNumeric(strDitta) Then
            Chiudi("Errore: CODICE DITTA SCONOSCIUTO")
            Exit Function
        End If
        '-
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Function
        End If
        Dim EserSucc As String = Format(Val(strEser) + 1, "0000")
        '---
        Dim strSQL As String = ""
        strSQL = "Select Ditta, Esercizio FROM Esercizi "
        strSQL += "WHERE Ditta = '" & strDitta.Trim & "'"
        strSQL += " AND Esercizio = '" & EserSucc.Trim & "'"
        '----------
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CKAnnoSucc = EserSucc
                    Exit Function
                Else
                    CKAnnoSucc = ""
                    Exit Function
                End If
            Else
                CKAnnoSucc = ""
                Exit Function
            End If
        Catch Ex As Exception
            CKAnnoSucc = ""
            SWErrore = True
            myErrore = Ex.Message
            Exit Function
        End Try

    End Function
    'GIU030220
    Private Function GetEserMax() As String
        GetEserMax = ""
        myErrore = ""
        '-
        Dim strDitta As String = Session(CSTCODDITTA)
        If IsNothing(strDitta) Then
            strDitta = ""
        End If
        If String.IsNullOrEmpty(strDitta) Then
            strDitta = ""
        End If
        If strDitta = "" Or Not IsNumeric(strDitta) Then
            Chiudi("Errore: CODICE DITTA SCONOSCIUTO")
            Exit Function
        End If
        '-
        Dim strSQL As String = ""
        strSQL = "Select MAX(Esercizio) AS Esercizio FROM Esercizi "
        strSQL += "WHERE Ditta = '" & strDitta.Trim & "'"
        '----------
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    GetEserMax = ds.Tables(0).Rows(0)("Esercizio")
                    Exit Function
                Else
                    GetEserMax = ""
                    Exit Function
                End If
            Else
                GetEserMax = ""
                Exit Function
            End If
        Catch Ex As Exception
            GetEserMax = ""
            SWErrore = True
            myErrore = Ex.Message
            Exit Function
        End Try

    End Function
    'GIU250418
    Private Sub ExecDelete(ByVal SQLCmd As SqlCommand, ByVal strSP As String)
        Try
            SQLCmd.CommandText = strSP
            SQLCmd.ExecuteNonQuery()
        Catch ex As Exception
            'PROSEGUO XKè NON ESISTE
        End Try
    End Sub
End Class