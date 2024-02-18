Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.Documenti
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms

Partial Public Class WUC_FatturazioneDDT
    Inherits System.Web.UI.UserControl

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = ""
    'giu150312 per il ricalcolo scadenze
    'DATE DI SCADENZE RATE
    Dim lblDataScad1 As String = ""
    Dim lblDataScad2 As String = ""
    Dim lblDataScad3 As String = ""
    Dim lblDataScad4 As String = ""
    Dim lblDataScad5 As String = ""
    'IMPORTO RATE DI SCADENZE RATE
    Dim lblImpRata1 As String = ""
    Dim lblImpRata2 As String = ""
    Dim lblImpRata3 As String = ""
    Dim lblImpRata4 As String = ""
    Dim lblImpRata5 As String = ""
    Dim LblTotaleRate As String = ""
    Const DefTipoFatt As String = "TipoFatturazioneDEFAULT"
    Const AvvisoTF As String = "AvvisoTFDiv" 'alb060213

    Private Enum CellIdxT
        TipoDC = 1
        Stato = 2
        NumDoc = 3
        DataDoc = 4
        DataCons = 5
        CodCliForProvv = 6
        RagSoc = 7
        Denom = 8
        Loc = 9
        Pr = 10
        CAP = 11
        PI = 12
        CF = 13
        DataVal = 14
        Riferimento = 15
        DesCausale = 16
        TotaleDoc = 17
        TotNettoPagare = 18
        TotaleImp = 19
        TotaleIVA = 20
        FatturaPA = 21
        SplitIVA = 22
        RitAcconto = 23
        Cod_Valuta = 24
        Cod_Pagamento = 25
        ABI = 26
        CAB = 27
        IDDoc = 28
    End Enum
    Private Enum CellIdxD
        CodArt = 0
        DesArt = 1
        UM = 2
        QtaOr = 3
        QtaEv = 4
        QtaRe = 5
        QtaAl = 6
        IVA = 7
        Prz = 8
        ScV = 9
        Sc1 = 10
        Importo = 11
        ScR = 12
    End Enum
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_FatturazioneDDT.aspx?labelForm=Fatturazione documenti emessi"
        ModalPopup.WucElement = Me
        Attesa.WucElement = Me

        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If

        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            chkTipoFT.AutoPostBack = False : ddlTipoFattur.AutoPostBack = False 'giu161219
            chkTipoFT.Enabled = False : chkTipoFT.Checked = False : ddlTipoFattur.Enabled = False
            btnCreaFattura.Visible = False
            btnCreaTutte.Visible = False
        End If
        '----------------------------
        ' ''If (sTipoUtente.Equals(CSTVENDITE)) Then
        ' ''    chkTipoFT.Enabled = False : chkTipoFT.Checked = False : ddlTipoFattur.Enabled = False
        ' ''    btnCreaFattura.Visible = False
        ' ''    btnCreaTutte.Visible = False
        ' ''End If
        '----------------------------
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

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSTipoFatt.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        If (Not IsPostBack) Then
            Try
                'giu160312 richiesta di Cinzia DATA del giorno
                ' ''txtDataA.Text = Format(ControllaDataDoc, FormatoData)
                ' ''If CDate(txtDataA.Text.Trim) = DATANULL Then
                ' ''    txtDataA.Text = Format(Now, FormatoData)
                ' ''    txtDataFattura.Text = Format(Now, FormatoData)
                ' ''Else
                ' ''    txtDataFattura.Text = txtDataA.Text
                ' ''End If
                If CDate(Now).Year = Val(Session(ESERCIZIO)) Then
                    txtDataA.Text = Format(Now, FormatoData)
                    txtDataFattura.Text = Format(Now, FormatoData)
                Else
                    txtDataA.Text = Format(CDate("31/12/" & Session(ESERCIZIO).ToString.Trim), FormatoData)
                    txtDataFattura.Text = Format(CDate("31/12/" & Session(ESERCIZIO).ToString.Trim), FormatoData)
                End If

                Dim NDoc As Long = 0 : Dim StrErrore As String = ""
                If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                    If chkFatturaPA.Checked = False Then
                        NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                    Else
                        NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                    End If
                Else
                    Chiudi("Errore Caricamento parametri generali.: " & StrErrore)
                    'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    'ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                '-
                txtPrimoNFattura.Text = FormattaNumero(NDoc.ToString.Trim)
                '---
                btnCreaFattura.Text = "Crea singola Fattura"
                btnCreaTutte.Text = "Crea tutte le Fatture"
                '-----
                Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)

                ddlRicerca.Items.Add("Numero documento")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("Data documento")
                ddlRicerca.Items(1).Value = "D"
                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(2).Value = "R"
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(3).Value = "S"
                ddlRicerca.Items.Add("Partita IVA")
                ddlRicerca.Items(4).Value = "P"
                ddlRicerca.Items.Add("Codice Fiscale")
                ddlRicerca.Items(5).Value = "F"
                ddlRicerca.Items.Add("Località")
                ddlRicerca.Items(6).Value = "L"
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(7).Value = "M"
                ddlRicerca.Items.Add("Data consegna")
                ddlRicerca.Items(8).Value = "C"
                ddlRicerca.Items.Add("Data validità")
                ddlRicerca.Items(9).Value = "V"
                ddlRicerca.Items.Add("Riferimento")
                ddlRicerca.Items(10).Value = "NR"
                ddlRicerca.Items.Add("Codice CoGe")
                ddlRicerca.Items(11).Value = "CG"
                'giu161219 giu130312
                If App.GetDatiAbilitazioni(CSTABILAZI, "TipoFattIDEF", Session(DefTipoFatt), StrErrore) = True Then
                    If StrErrore.Trim <> "" Then
                        Session(DefTipoFatt) = ""
                    End If
                End If
                ddlTipoFattur.AutoPostBack = False
                ddlTipoFattur.DataBind()
                chkTipoFT.AutoPostBack = False
                chkTipoFT.Checked = True
                Call PosizionaItemDDL(Session(DefTipoFatt), ddlTipoFattur) 'chkTipoFT_CheckedChanged(Nothing, Nothing)
                ddlTipoFattur.AutoPostBack = True
                chkTipoFT.AutoPostBack = True
                'giu161219
                If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                    chkTipoFT.AutoPostBack = False : ddlTipoFattur.AutoPostBack = False 'giu161219
                    chkTipoFT.Enabled = False : chkTipoFT.Checked = False : ddlTipoFattur.Enabled = False
                    btnCreaFattura.Visible = False
                    btnCreaTutte.Visible = False
                End If
                '-------------------
                BuidDett()
                Session(SWOP) = SWOPNESSUNA
                'giu210512 controllo numerazione fattura se corretta
                '--------------------------------------------
                Dim CkNumDoc As Long = CheckNumDoc(StrErrore)
                If CkNumDoc = -1 Then
                    Chiudi("Errore: Verifica N° Documento da impegnare. " & StrErrore)
                    Exit Sub
                End If
                If NDoc <> CkNumDoc Then
                    txtPrimoNFattura.BackColor = SEGNALA_KO
                    txtPrimoNFattura.ToolTip = "Attenzione, ci sono dei numeri documento da recuperare!!!"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = "Attenzione, ci sono dei numeri documento da recuperare!!!"
                    ModalPopup.Show("N° Fattura fuori sequenza", "<strong><span> FATTURA COMMERCIALE" _
                                & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                & FormattaNumero(NDoc) & " <br>" _
                                & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                                & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                                & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_ALERT)
                End If

            Catch Ex As Exception
                Chiudi("Errore caricamento Elenco Documenti: " & Ex.Message)
                Exit Sub
            End Try
            Session(AvvisoTF) = True 'alb060213
        End If

    End Sub
    'giu161219
    Private Sub ImpostaFiltro()
        SqlDSPrevTElenco.FilterExpression = ""
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or _
               ddlRicerca.SelectedValue = "V" Or _
               ddlRicerca.SelectedValue = "C" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        If txtRicerca.Text.Trim = "" Then
            'GIU171219
            If chkTipoFT.Checked Then
                If ddlTipoFattur.SelectedValue.ToString.Trim = "" Then
                    'nulla per TIPO FATTURAZIONE
                Else
                    If SqlDSPrevTElenco.FilterExpression <> "" Then
                        SqlDSPrevTElenco.FilterExpression += " AND "
                    End If
                    SqlDSPrevTElenco.FilterExpression += "Tipo_Fatturazione = '" & ddlTipoFattur.SelectedValue.ToString.Trim & "'"
                End If
            End If
            '----------
            If Not IsNumeric(txtDalNDDT.Text.Trim) Then txtDalNDDT.Text = "1"
            If Not IsNumeric(txtAlNDDT.Text.Trim) Then txtAlNDDT.Text = "999999999"
            If Val(txtDalNDDT.Text.Trim) > Val(txtAlNDDT.Text.Trim) Then
                txtDalNDDT.Text = "1"
                txtAlNDDT.Text = "999999999"
            End If
            'giu080512
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Numero >= " & txtDalNDDT.Text.Trim & " AND "
            SqlDSPrevTElenco.FilterExpression += "Numero <= " & txtAlNDDT.Text.Trim
            'giu140312
            If IsDate(txtDataA.Text.Trim) Then
                If SqlDSPrevTElenco.FilterExpression <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                SqlDSPrevTElenco.FilterExpression += "Data_Doc <= '" & CDate(txtDataA.Text.Trim) & "'"
            End If
            'GIU110814
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            If chkFatturaPA.Checked = False Then
                SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)=0"
            Else
                SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)<>0"
            End If

        Else
            If ddlRicerca.SelectedValue = "N" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtRicerca.Text.Trim
                Else
                    SqlDSPrevTElenco.FilterExpression = "Numero = " & txtRicerca.Text.Trim
                End If
            ElseIf ddlRicerca.SelectedValue = "D" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "Data_Doc >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "Data_Doc = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "CG" Then 'GIU090212
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "R" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "C" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataOraConsegna >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataOraConsegna = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "V" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "Data_Validita >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "Data_Validita = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "L" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Localita = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "M" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "CAP = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "S" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Denominazione = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "P" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "F" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "NR" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Riferimento = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            End If
            'GIU171219
            If chkTipoFT.Checked Then
                If ddlTipoFattur.SelectedValue.ToString.Trim = "" Then
                    'nulla per TIPO FATTURAZIONE
                Else
                    If SqlDSPrevTElenco.FilterExpression <> "" Then
                        SqlDSPrevTElenco.FilterExpression += " AND "
                    End If
                    SqlDSPrevTElenco.FilterExpression += "Tipo_Fatturazione = '" & ddlTipoFattur.SelectedValue.ToString.Trim & "'"
                End If
            End If
            '----------
            If Not IsNumeric(txtDalNDDT.Text.Trim) Then txtDalNDDT.Text = "1"
            If Not IsNumeric(txtAlNDDT.Text.Trim) Then txtAlNDDT.Text = "999999999"
            If Val(txtDalNDDT.Text.Trim) > Val(txtAlNDDT.Text.Trim) Then
                txtDalNDDT.Text = "1"
                txtAlNDDT.Text = "999999999"
            End If
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Numero >= " & txtDalNDDT.Text.Trim & " AND "
            SqlDSPrevTElenco.FilterExpression += "Numero <= " & txtAlNDDT.Text.Trim
            'giu140312
            If IsDate(txtDataA.Text.Trim) Then
                If SqlDSPrevTElenco.FilterExpression <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                SqlDSPrevTElenco.FilterExpression += "Data_Doc <= '" & CDate(txtDataA.Text.Trim) & "'"
            End If
            '-
            'GIU110814
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            If chkFatturaPA.Checked = False Then
                SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)=0"
            Else
                SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)<>0"
            End If
            '---------
        End If
    End Sub
    'giu210512
    'giu230312 giu260312 Recupero Numeri non impegnati
    'giu260312 verifica la sequenza se è completa
    'giu110814 FatturaPA
    Private Function CheckNumDoc(ByRef strErrore As String) As Long
        'giu110814
        ' ''Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE "
        ' ''strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' OR "
        ' ''strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
        ' ''If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
        ' ''    strSQL += "OR Tipo_Doc = '" & SWTD(TD.NotaCredito) & "'"
        ' ''End If
        '-
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero "
        strSQL += "From DocumentiT WHERE "
        If chkFatturaPA.Checked = False Then
            strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
            If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                strSQL += "OR Tipo_Doc = '" & SWTD(TD.NotaCredito) & "'"
            End If
            strSQL += ") AND ISNULL(FatturaPA,0)=0"
        Else
            strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' "
            If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                strSQL += "OR Tipo_Doc = '" & SWTD(TD.NotaCredito) & "'"
            End If
            strSQL += ") AND ISNULL(FatturaPA,0)<>0"
        End If
        '---------

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        CheckNumDoc = ds.Tables(0).Rows(0).Item("Numero") + 1
                        'giu260819 non va bene, es. i preventivi ci sono le REVISION quindi sicuramente il numero è superiore
                        ' ''If (ds.Tables(0).Rows(0).Item("TotDoc") + 1) <> (ds.Tables(0).Rows(0).Item("Numero") + 1) Then
                        ' ''    CheckNumDoc = (ds.Tables(0).Rows(0).Item("TotDoc") + 1)
                        ' ''End If
                    Else
                        CheckNumDoc = 1
                    End If
                    Exit Function
                Else
                    CheckNumDoc = 1
                    Exit Function
                End If
            Else
                CheckNumDoc = 1
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = Ex.Message
            CheckNumDoc = -1
            Exit Function
        End Try

    End Function
    'giu080212
    Private Sub BtnSetByStato(ByVal myStato As String)
        BtnSetEnabledTo(True)
        If myStato.Trim = "NON Fatturabile" Then
            btnCreaFattura.Visible = False
            btnCreaTutte.Visible = False
        End If
        If myStato.Trim = "Fatturato" Then
            btnCreaFattura.Visible = False
            btnCreaTutte.Visible = False
            btnModifica.Visible = False
        End If
        If myStato.Trim = "OK trasf.in CoGe" Then
            btnCreaFattura.Visible = False
            btnCreaTutte.Visible = False
            btnModifica.Visible = False
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Then
            btnCreaFattura.Visible = False
            btnCreaTutte.Visible = False
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito) Then
            btnCreaFattura.Visible = False
            btnCreaTutte.Visible = False
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori) Then
            btnCreaFattura.Visible = False
            btnCreaTutte.Visible = False
        End If

    End Sub

#Region " Funzioni e Procedure"
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnCreaTutte.Enabled = Valore
        btnCreaFattura.Enabled = Valore
        btnModifica.Enabled = Valore
    End Sub
#End Region

#Region "Ordinamento e ricerca"

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        Session(CSTTIPODOCSEL) = ""
        ImpostaFiltro()
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU220617
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        Session(CSTTIPODOCSEL) = ""
        Call ImpostaFiltro()
        SqlDSPrevTElenco.DataBind()
        'giu110412
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged

        Try
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            BtnSetByStato(Stato)
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End Try
    End Sub
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
                e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataVal).Text) Then
                e.Row.Cells(CellIdxT.DataVal).Text = Format(CDate(e.Row.Cells(CellIdxT.DataVal).Text), FormatoData).ToString
            End If
            'TotaleDoc
            If IsNumeric(e.Row.Cells(CellIdxT.TotaleDoc).Text) Then
                If CDec(e.Row.Cells(CellIdxT.TotaleDoc).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.TotaleDoc).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.TotaleDoc).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.TotaleDoc).Text = ""
                End If
            End If
            'Tot.Netto a pagare
            If IsNumeric(e.Row.Cells(CellIdxT.TotNettoPagare).Text) Then
                If CDec(e.Row.Cells(CellIdxT.TotNettoPagare).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.TotNettoPagare).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.TotNettoPagare).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.TotNettoPagare).Text = ""
                End If
            End If
            'TotaleImp
            If IsNumeric(e.Row.Cells(CellIdxT.TotaleImp).Text) Then
                If CDec(e.Row.Cells(CellIdxT.TotaleImp).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.TotaleImp).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.TotaleImp).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.TotaleImp).Text = ""
                End If
            End If
            'TotaleIVA
            If IsNumeric(e.Row.Cells(CellIdxT.TotaleIVA).Text) Then
                If CDec(e.Row.Cells(CellIdxT.TotaleIVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.TotaleIVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.TotaleIVA).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.TotaleIVA).Text = ""
                End If
            End If
        End If
    End Sub
    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.QtaOr).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaOr).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaOr).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaOr).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaOr).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaAl).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaAl).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaAl).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaAl).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaAl).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxD.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxD.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxD.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScR).Text), 4).ToString
                Else
                    e.Row.Cells(CellIdxD.ScR).Text = ""
                End If
            End If
        End If
    End Sub
    Private Sub BuidDett()
        Call ImpostaFiltro()
        '---------
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If

    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        Call BuidDett()
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        BuidDett()
    End Sub
#End Region

#Region "Scelta tipo documenti"
    Private Sub chkTipoFT_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        chkTipoFT.CheckedChanged
        If chkTipoFT.Checked Then
            ddlTipoFattur.Enabled = True
            If ddlTipoFattur.SelectedValue.ToString.Trim = "" Then
                If String.IsNullOrEmpty(Session(DefTipoFatt)) Then
                    Session(DefTipoFatt) = ""
                End If
                ddlTipoFattur.AutoPostBack = False
                Call PosizionaItemDDL(Session(DefTipoFatt), ddlTipoFattur)
                ddlTipoFattur.AutoPostBack = True
            End If
        Else
            ddlTipoFattur.AutoPostBack = False
            ddlTipoFattur.Enabled = False
            ddlTipoFattur.SelectedIndex = -1
            ddlTipoFattur.AutoPostBack = True
            'alb060213
            If Session(AvvisoTF) Then
                Session(AvvisoTF) = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Tipo fatturazione selezionato diverso da quello predefinito.", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
            'alb060213 END
        End If
        BuidDett()
    End Sub

#End Region

    Private Sub Chiudi(ByVal strErrore As String)

        ' ''Try
        ' ''    Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale" & strErrore)
        ' ''Catch ex As Exception
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        ' ''End Try
        '-
        Dim strRitorno As String = ""
        strRitorno = ""
        If strErrore.Trim = "" Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        Else
            strRitorno = "WF_Menu.aspx?labelForm=" & strErrore
        End If
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try
    End Sub
    Private Function CKCSTTipoDocSel(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDocSel = True
        TipoDoc = Session(CSTTIPODOCSEL)
        If IsNothing(TipoDoc) Then
            TipoDoc = ""
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            TipoDoc = ""
            Return False
        End If
        strDesTipoDocumento = ""
        'DDT
        If TipoDoc = SWTD(TD.DocTrasportoClienti) Then
            strDesTipoDocumento = "DOCUMENTO DI TRASPORTO CLIENTI"
        End If
        If TipoDoc = SWTD(TD.DocTrasportoCLavoro) Then
            strDesTipoDocumento = "DOCUMENTO DI TRASPORTO C/LAVORO"
        End If
        If TipoDoc = SWTD(TD.DocTrasportoFornitori) Then
            strDesTipoDocumento = "DOCUMENTO DI TRASPORTO FORNITORI"
        End If
        'fatture, NC,
        If TipoDoc = SWTD(TD.FatturaCommerciale) Then
            strDesTipoDocumento = "FATTURA COMMERCIALE"
        End If
        If TipoDoc = SWTD(TD.FatturaAccompagnatoria) Then
            strDesTipoDocumento = "FATTURA ACCOMPAGNATORIA"
        End If
        If TipoDoc = SWTD(TD.FatturaScontrino) Then
            strDesTipoDocumento = "FATTURA CON SCONTRINO"
        End If
        If TipoDoc = SWTD(TD.NotaCredito) Then
            strDesTipoDocumento = "NOTA DI CREDITO"
        End If
        If TipoDoc = SWTD(TD.NotaCorrispondenza) Then
            strDesTipoDocumento = "NOTA CORRISPONDENZA"
        End If
        If TipoDoc = SWTD(TD.BuonoConsegna) Then
            strDesTipoDocumento = "BUONO CONSEGNA"
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
    End Function

    Private Sub ddlTipoFattur_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTipoFattur.SelectedIndexChanged
        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        Call BuidDett()
        'alb060213
        If Session(AvvisoTF) And ddlTipoFattur.SelectedValue <> Session(DefTipoFatt) Then
            Session(AvvisoTF) = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Tipo fatturazione selezionato diverso da quello predefinito.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        'alb060213 END
    End Sub

    Private Sub btnCreaFattura_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaFattura.Click
        If Not IsDate(txtDataFattura.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "DATA FATTURAZIONE ERRATA.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        If CDate(txtDataFattura.Text.Trim).Date < CDate(ControllaDataDoc()).Date Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "DATA FATTURAZIONE NON PUO' ESSERE INFERIORE ALL'ULTIMA DATA FATTURA INSERITA.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        'giu160118
        If CDate(txtDataFattura.Text.Trim).Date.Year <> Val(Session(ESERCIZIO).ToString.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "ANNO DOCUMENTO NON PUO' ESSERE DIVERSO DALL'ESERCIZIO IN CORSO: " & Session(ESERCIZIO).ToString.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        '---------
        Dim StrErrore As String = ""
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        'giu200323
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'GIU170422 BLOCCO ALLESTIMENTO 
        Dim strBloccoCliente As String = ""
        Dim objControllo As New Controllo
        Dim SWNoFatt As Boolean = False
        Dim SWNoPIVACF As Boolean = False
        Dim SWNoCodIPA As Boolean = False
        Dim swNoDestM As Boolean = False : Dim swNODatiCorr As Boolean = False : Dim swNoCVett As Boolean = False
        SWNoFatt = objControllo.CKCliNoFattByIDDoc(myID, SWNoPIVACF, SWNoCodIPA, swNoDestM, swNODatiCorr, swNoCVett, StrErrore)
        objControllo = Nothing
        If StrErrore.Trim = "" Then
            Dim swOK As Boolean = False
            strBloccoCliente = "<strong><span>Attenzione!!!, Cliente:<br>"
            If SWNoPIVACF Then
                strBloccoCliente += "SENZA P.IVA/C.F.<br>"
                swOK = True
            End If
            If SWNoCodIPA Then
                strBloccoCliente += "SENZA Codice IPA<br>"
                swOK = True
            End If
            If SWNoFatt Then
                strBloccoCliente += "NON Fatturabile (Non bloccante)<br>"
                swOK = True
            End If
            '''If swNoDestM Then
            '''    strBloccoCliente += "SENZA Destinazione Merce<br>"
            '''    swOK = True
            '''ElseIf swNODatiCorr Then
            '''    strBloccoCliente += "SENZA Dati corriere<br>"
            '''    swOK = True
            '''End If
            strBloccoCliente += "</span></strong>"
            If swOK = False Then
                strBloccoCliente = ""
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore verifica Cliente", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'GIU200323
        If SWNoPIVACF Or SWNoCodIPA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strBloccoCliente + "<strong><span><br>IMPOSSIBILE CREARE FATTURA<br>CONTATTARE l'ufficio Amministrativo</span></strong>", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '--------
        'GIU290519
        Dim row As GridViewRow = GridViewPrevT.SelectedRow
        Dim strTotaleImp As String = row.Cells(CellIdxT.TotaleImp).Text
        Dim SWTotaleZero As Boolean = False
        If String.IsNullOrEmpty(strTotaleImp) Then
            SWTotaleZero = True
        ElseIf Not IsNumeric(strTotaleImp.Trim) Then
            SWTotaleZero=True
        ElseIf CDec(strTotaleImp) = 0 Then
            SWTotaleZero=True
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "EnableAll"
        If SWTotaleZero = True Then
            ModalPopup.Show("Crea singola Fattura da DDT", "Confermi la creazione Fattura dal DDT selezionato ? <br> <strong><span>ATTENZIONE, Totale documento uguale a ZERO !!!</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        Else
            ModalPopup.Show("Crea singola Fattura da DDT", "Confermi la creazione Fattura dal DDT selezionato ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
        End If
    End Sub
    Public Sub CreaFattura() 'SINGOLA

        Dim StrErrore As String = ""
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu250118 SPlit
        Dim mySplitIVA As String = "" : Dim strNDoc As String = ""
        Try
            Dim rowCTR As GridViewRow = GridViewPrevT.SelectedRow
            strNDoc = rowCTR.Cells(CellIdxT.NumDoc).Text
            If IsNothing(strNDoc) Then
                strNDoc = ""
            End If
            If String.IsNullOrEmpty(strNDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUMERO DOCUMENTO ERRATO", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            mySplitIVA = rowCTR.Cells(CellIdxT.SplitIVA).Text
            If IsNothing(strNDoc) Then
                mySplitIVA = ""
            End If
            If CBool(mySplitIVA.Trim) = True Then
                'OK
            End If
            If String.IsNullOrEmpty(mySplitIVA) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "SPLIT PAYMENT IVA NON DEFINITO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "SPLIT PAYMENT IVA NON DEFINITO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '---------------
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            If chkFatturaPA.Checked = False Then 'giu110814
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        If AggiornaNumDoc(SWTD(TD.FatturaCommerciale), NDoc, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVA FATTURA
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFC]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FatturaPA", System.Data.SqlDbType.Bit, 1, "FatturaPA"))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SplitIVA", System.Data.SqlDbType.Bit, 1, "SplitIVA"))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Doc", System.Data.SqlDbType.DateTime, 8, "Data_Doc"))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        If chkFatturaPA.Checked = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        'GIU250117
        If CBool(mySplitIVA.Trim) = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
        SqlDbNewCmd.Parameters.Item("@Data_Doc").Value = CDate(txtDataFattura.Text).Date
        '---------
        Try
            myID = ""
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myID = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                        & "Errore: " + Ex.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try

        Try
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'OK FATTO
        Session(CSTFATTURAPA) = chkFatturaPA.Checked 'giu110814
        'GIU250117
        Session(CSTSPLITIVA) = CBool(mySplitIVA.Trim)
        '---------
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(IDDOCUMENTI) = myID.Trim
        Response.Redirect("WF_Documenti.aspx?labelForm=FATTURA COMMERCIALE")
    End Sub
    Public Sub AvviaRicerca()
        Call btnRicerca_Click(Nothing, Nothing)
    End Sub
    Private Function AggiornaNumDoc(ByVal TDoc As String, ByVal NDoc As Long, ByVal NRev As Integer) As Boolean
        AggiornaNumDoc = True
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Dim SqlDbUpdCmd As New SqlCommand

        SqlDbUpdCmd.CommandText = "[Update_NDocPargen]"
        SqlDbUpdCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdCmd.Connection = SqlConn
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWOp", System.Data.SqlDbType.VarChar, 1))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '--
        SqlDbUpdCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbUpdCmd.Parameters.Item("@SWOp").Value = "N"
        If chkFatturaPA.Checked = False Then 'giu110814
            SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = TDoc
        Else
            SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = "PA"
        End If
        '-
        SqlDbUpdCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbUpdCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        Try
            SqlConn.Open()
            SqlDbUpdCmd.CommandTimeout = myTimeOUT
            SqlDbUpdCmd.ExecuteNonQuery()
            SqlConn.Close()
            If SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -1 Then 'IMPEGNATO GIA ESISTE SOMMO 1 E RIPROVO
                AggiornaNumDoc = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiorna numero documento", "NUOVO IDENTIFICATIVO DOCUMENTO GIA' PRESENTE.", WUC_ModalPopup.TYPE_ERROR)
            ElseIf SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -2 Then 'ERRORE O PER SQL OPPURE TIPO DOC NON GESTITO
                AggiornaNumDoc = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiorna numero documento", "NUOVO TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ExSQL As SqlException
            AggiornaNumDoc = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
        Catch Ex As Exception
            AggiornaNumDoc = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Function

    Private Sub txtDataA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataA.TextChanged
        BuidDett()
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub

    Private Sub btnChiudi_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChiudi.Click
        Chiudi("")
    End Sub

    Private Sub btnCercaDDTNum_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaDDTNum.Click
        BuidDett()
    End Sub

    'giu140312
    Private Sub btnCreaTutte_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaTutte.Click
        BuidDett()
        If Not IsDate(txtDataFattura.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "DATA FATTURAZIONE ERRATA.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        If CDate(txtDataFattura.Text.Trim).Date < CDate(ControllaDataDoc()).Date Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "DATA FATTURAZIONE NON PUO' ESSERE INFERIORE ALL'ULTIMA DATA FATTURA INSERITA.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        'giu160118
        If CDate(txtDataFattura.Text.Trim).Date.Year <> Val(Session(ESERCIZIO).ToString.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "ANNO DOCUMENTO NON PUO' ESSERE DIVERSO DALL'ESERCIZIO IN CORSO: " & Session(ESERCIZIO).ToString.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        '---------
        Dim strNDoc As String = "" : Dim strDataDoc As String = ""
        'GIU290519
        Dim SWTotaleZero As Boolean = False
        Dim strTotNettoPagare As String = "" : Dim strTotaleImp As String = ""
        'giu200323
        Dim strBloccoCliente As String = "" : Dim strErrore As String = ""
        Dim SWNoFatt As Boolean = False
        Dim SWNoPIVACF As Boolean = False
        Dim SWNoCodIPA As Boolean = False
        Dim swNoDestM As Boolean = False : Dim swNODatiCorr As Boolean = False : Dim swNoCVett As Boolean = False
        Dim objControllo As New Controllo
        Dim myID As String = ""
        '---------
        If GridViewPrevT.Rows.Count > 0 Then
            'ok procedo
            For Each rowCTR As GridViewRow In GridViewPrevT.Rows
                'N°DOCUMENTO
                strNDoc = rowCTR.Cells(CellIdxT.NumDoc).Text
                If String.IsNullOrEmpty(strNDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUMERO DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
                'DATA DOCUMENTO
                strDataDoc = rowCTR.Cells(CellIdxT.DataDoc).Text
                If String.IsNullOrEmpty(strDataDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                ElseIf Not IsDate(CDate(strDataDoc)) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
                If CDate(strDataDoc).Date > CDate(txtDataFattura.Text.Trim) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "DATA DOCUMENTO DDT SUPERIORE ALLA DATA DI FATTURAZIONE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
                'giu230323
                myID = rowCTR.Cells(CellIdxT.IDDoc).Text.Trim
                strBloccoCliente = ""
                SWNoFatt = objControllo.CKCliNoFattByIDDoc(myID, SWNoPIVACF, SWNoCodIPA, swNoDestM, swNODatiCorr, swNoCVett, strErrore)
                
                If strErrore.Trim = "" Then
                    Dim swOK As Boolean = False
                    strBloccoCliente = "<strong><span>Attenzione!!!, Cliente:<br>"
                    If SWNoPIVACF Then
                        strBloccoCliente += "SENZA P.IVA/C.F.<br>"
                        swOK = True
                    End If
                    If SWNoCodIPA Then
                        strBloccoCliente += "SENZA Codice IPA<br>"
                        swOK = True
                    End If
                    If SWNoFatt Then
                        strBloccoCliente += "NON Fatturabile (Non bloccante)<br>"
                        swOK = True
                    End If
                    '''If swNoDestM Then
                    '''    strBloccoCliente += "SENZA Destinazione Merce<br>"
                    '''    swOK = True
                    '''ElseIf swNODatiCorr Then
                    '''    strBloccoCliente += "SENZA Dati corriere<br>"
                    '''    swOK = True
                    '''End If
                    strBloccoCliente += "</span></strong>"
                    If swOK = False Then
                        strBloccoCliente = ""
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore verifica Cliente. N° DDT: " & strNDoc, strErrore, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
                'GIU200323
                If SWNoPIVACF Or SWNoCodIPA Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", strBloccoCliente + "<strong><span><br>IMPOSSIBILE CREARE FATTURA per il . N° DDT: " & strNDoc & "<br>CONTATTARE l'ufficio Amministrativo</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
                '--------
                '--------
                'GIU290519 Exit For
                strTotNettoPagare = rowCTR.Cells(CellIdxT.TotNettoPagare).Text
                If String.IsNullOrEmpty(strTotNettoPagare) Then
                    SWTotaleZero = True
                ElseIf Not IsNumeric(strTotNettoPagare.Trim) Then
                    SWTotaleZero=True
                ElseIf CDec(strTotNettoPagare) = 0 Then
                    SWTotaleZero=True
                End If
                'giu110215
                strTotaleImp = rowCTR.Cells(CellIdxT.TotaleImp).Text
                If String.IsNullOrEmpty(strTotaleImp) Then
                    SWTotaleZero=True
                ElseIf Not IsNumeric(strTotaleImp.Trim) Then
                    SWTotaleZero=True
                ElseIf CDec(strTotaleImp) = 0 Then
                    SWTotaleZero=True
                End If
            Next
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun DDT selezionato per la fatturazione.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        'giu040512
        Session(EL_DOC_TOPRINT_SCY) = Nothing
        Session(EL_DOC_TOPRINT_SCN) = Nothing
        '---------
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattureTutte"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "EnableAll"
        If SWTotaleZero = True Then
            ModalPopup.Show("Crea TUTTE le Fatture da DDT selezionati", "Confermi la creazione di TUTTE le Fatture ? <br>" & _
                            "<strong><span>ATTENZIONE, Sono presenti dei documenti con Importo totale uguale a ZERO !!!</span></strong> <br>" & _
                            "Primo DDT selezionato per la fatturazione: N° " & strNDoc & " del " & strDataDoc, WUC_ModalPopup.TYPE_CONFIRM_YN)
        Else
            ModalPopup.Show("Crea TUTTE le Fatture da DDT selezionati", "Confermi la creazione di TUTTE le Fatture ? <br>" & _
                            "Primo DDT selezionato per la fatturazione: N° " & strNDoc & " del " & strDataDoc, WUC_ModalPopup.TYPE_CONFIRM_YN)
        End If
    End Sub
    'giu150312
    Private Function ControllaDataDoc() As DateTime
        Dim strSQL As String = ""
        strSQL = "Select MAX(Data_Doc) AS Data_Doc From DocumentiT WHERE Tipo_Doc = 'FC' "
        'GIU110814
        If chkFatturaPA.Checked = False Then
            strSQL += " AND ISNULL(FatturaPA,0) = 0"
        Else
            strSQL += " AND ISNULL(FatturaPA,0) <> 0"
        End If
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Data_Doc")) Then
                        ControllaDataDoc = ds.Tables(0).Rows(0).Item("Data_Doc")
                    Else
                        ControllaDataDoc = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu160118 DATANULL
                    End If
                    Exit Function
                Else
                    ControllaDataDoc = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu160118 DATANULL
                    Exit Function
                End If
            Else
                ControllaDataDoc = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu160118 DATANULL
                Exit Function
            End If
        Catch Ex As Exception
            ControllaDataDoc = CDate("01/01/" & Session(ESERCIZIO).ToString.Trim) 'giu160118 DATANULL
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
    Public Sub CreaFattureTutte() 'TUTTE
        If Not IsDate(txtDataFattura.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "DATA FATTURAZIONE ERRATA.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        If CDate(txtDataFattura.Text.Trim).Date < CDate(ControllaDataDoc()).Date Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "DATA FATTURAZIONE NON PUO' ESSERE INFERIORE ALL'ULTIMA DATA FATTURA INSERITA.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        'giu160118
        If CDate(txtDataFattura.Text.Trim).Date.Year <> Val(Session(ESERCIZIO).ToString.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "ANNO DOCUMENTO NON PUO' ESSERE DIVERSO DALL'ESERCIZIO IN CORSO: " & Session(ESERCIZIO).ToString.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        '---------
        Session(CSTDECIMALIVALUTADOC) = ""
        Dim StrErrore As String = ""
        Dim strNDoc As String = "" : Dim strDataDoc As String = "" : Dim strPr As String = "" : Dim strCausale As String = ""
        Dim myID As String = ""
        Dim NDoc As Long = 0 : Dim NDocOK As Long = 0 : Dim NRev As Integer = 0
        Dim strCPag As String = "" : Dim strTotaleDoc As String = "" : Dim Valuta As String = ""
        Dim strTotaleImp As String = "" : Dim strTotaleIVA As String = "" 'giu110215
        Dim strABI As String = "" : Dim strCAB As String = ""
        'giu160312 richiesta di Cinzia
        Dim strPI As String = "" : Dim strCF As String = ""
        Dim strSplitIVA As String = "" 'giu160215
        Dim strTotNettoPagare As String = ""
        'CONTROLLO
        For Each rowCTR As GridViewRow In GridViewPrevT.Rows
            'N°DOCUMENTO
            strNDoc = rowCTR.Cells(CellIdxT.NumDoc).Text
            If String.IsNullOrEmpty(strNDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUMERO DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'DATA DOCUMENTO
            strDataDoc = rowCTR.Cells(CellIdxT.DataDoc).Text
            If String.IsNullOrEmpty(strDataDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            ElseIf Not IsDate(CDate(strDataDoc)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            If CDate(strDataDoc).Date > CDate(txtDataFattura.Text.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO DDT SUPERIORE ALLA DATA DI FATTURAZIONE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'ID DOCUMENTI
            myID = rowCTR.Cells(CellIdxT.IDDoc).Text
            Session(IDDOCUMENTI) = myID
            myID = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                ' ''Chiudi("Errore: " & "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.")
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            
            'CODICE PAGAMENTO
            strCPag = rowCTR.Cells(CellIdxT.Cod_Pagamento).Text
            If String.IsNullOrEmpty(strCPag) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            ElseIf Not IsNumeric(strCPag) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            ElseIf CInt(strCPag) = 0 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'VALUTA
            Valuta = rowCTR.Cells(CellIdxT.Cod_Valuta).Text
            If String.IsNullOrEmpty(Valuta) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE VALUTA SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'TOTALE DOCUMENTO
            strTotaleDoc = rowCTR.Cells(CellIdxT.TotaleDoc).Text
            If String.IsNullOrEmpty(strTotaleDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            ElseIf Not IsNumeric(strTotaleDoc.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
                ' ''ElseIf CDec(strTotaleDoc) = 0 Then
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''    ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''    Exit Sub
            End If
            'TOT.NETTO A PAGARE
            strTotNettoPagare = rowCTR.Cells(CellIdxT.TotNettoPagare).Text
            If String.IsNullOrEmpty(strTotNettoPagare) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOT.NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            ElseIf Not IsNumeric(strTotNettoPagare.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOT.NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
                ' ''ElseIf CDec(strTotNettoPagare) = 0 Then
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''    ModalPopup.Show("Errore", "TOT.NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''    Exit Sub
            End If
            'giu110215
            strTotaleImp = rowCTR.Cells(CellIdxT.TotaleImp).Text
            If String.IsNullOrEmpty(strTotaleImp) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            ElseIf Not IsNumeric(strTotaleImp.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
                ' ''ElseIf CDec(strTotaleImp) = 0 Then
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''    ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''    Exit Sub
            End If
            strTotaleIVA = rowCTR.Cells(CellIdxT.TotaleIVA).Text
            'ABI
            strABI = rowCTR.Cells(CellIdxT.ABI).Text
            If String.IsNullOrEmpty(strABI) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE ABI. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'CAB
            strCAB = rowCTR.Cells(CellIdxT.CAB).Text
            If String.IsNullOrEmpty(strCAB) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE CAB. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'PI'CF
            strPI = rowCTR.Cells(CellIdxT.PI).Text
            strCF = rowCTR.Cells(CellIdxT.CF).Text
            If String.IsNullOrEmpty(strPI) And String.IsNullOrEmpty(strCF) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "P.IVA - Cod.Fiscale MANCANTI. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'PROVINCIA
            strPr = rowCTR.Cells(CellIdxT.Pr).Text
            If String.IsNullOrEmpty(strPr) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "PROVINCIA NON DEFINITA. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'CAUSALE
            strCausale = rowCTR.Cells(CellIdxT.DesCausale).Text
            If String.IsNullOrEmpty(strCausale) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CAUSALE NON DEFINITA. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'giu160215 Split Payment strSplitIVA
            strSplitIVA = rowCTR.Cells(CellIdxT.SplitIVA).Text
            If String.IsNullOrEmpty(strSplitIVA) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "EnableAll"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "SPLIT PAYMENT IVA NON DEFINITO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            '---------
        Next
        'FINE CONTROLLO
        For Each row As GridViewRow In GridViewPrevT.Rows
            'N°DOCUMENTO
            strNDoc = row.Cells(CellIdxT.NumDoc).Text
            If String.IsNullOrEmpty(strNDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUMERO DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'DATA DOCUMENTO
            strDataDoc = row.Cells(CellIdxT.DataDoc).Text
            If String.IsNullOrEmpty(strDataDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf Not IsDate(CDate(strDataDoc)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            If CDate(strDataDoc).Date > CDate(txtDataFattura.Text.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO DDT SUPERIORE ALLA DATA DI FATTURAZIONE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'ID DOCUMENTI
            myID = row.Cells(CellIdxT.IDDoc).Text
            'giu210423 non serve toccare la sessione 
            '''Session(IDDOCUMENTI) = myID
            '''myID = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                ' ''Chiudi("Errore: " & "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.")
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'CODICE PAGAMENTO
            strCPag = row.Cells(CellIdxT.Cod_Pagamento).Text
            If String.IsNullOrEmpty(strCPag) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf Not IsNumeric(strCPag) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf CInt(strCPag) = 0 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'VALUTA
            Valuta = row.Cells(CellIdxT.Cod_Valuta).Text
            If String.IsNullOrEmpty(Valuta) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE VALUTA SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'TOTALE DOCUMENTO
            strTotaleDoc = row.Cells(CellIdxT.TotaleDoc).Text
            If String.IsNullOrEmpty(strTotaleDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf Not IsNumeric(strTotaleDoc.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
                ' ''ElseIf CDec(strTotaleDoc) = 0 Then
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''    ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''    Exit Sub
            End If
            'TOT.NETTO A PAGARE
            strTotNettoPagare = row.Cells(CellIdxT.TotNettoPagare).Text
            If String.IsNullOrEmpty(strTotNettoPagare) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOT.NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf Not IsNumeric(strTotNettoPagare.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOT.NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
                ' ''ElseIf CDec(strTotNettoPagare) = 0 Then
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''    ModalPopup.Show("Errore", "TOT.NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''    Exit Sub
            End If
            'giu110215
            strTotaleImp = row.Cells(CellIdxT.TotaleImp).Text
            If String.IsNullOrEmpty(strTotaleImp) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf Not IsNumeric(strTotaleImp.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
                ' ''ElseIf CDec(strTotaleImp) = 0 Then
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''    ModalPopup.Show("Errore", "TOTALE IMP.DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''    Exit Sub
            End If
            '-
            strTotaleIVA = row.Cells(CellIdxT.TotaleIVA).Text
            'ABI
            strABI = row.Cells(CellIdxT.ABI).Text
            If String.IsNullOrEmpty(strABI) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE ABI. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'CAB
            strCAB = row.Cells(CellIdxT.CAB).Text
            If String.IsNullOrEmpty(strCAB) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE CAB. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'PI'CF
            strPI = row.Cells(CellIdxT.PI).Text
            strCF = row.Cells(CellIdxT.CF).Text
            If String.IsNullOrEmpty(strPI) And String.IsNullOrEmpty(strCF) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "P.IVA - Cod.Fiscale MANCANTI. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'PROVINCIA
            strPr = row.Cells(CellIdxT.Pr).Text
            If String.IsNullOrEmpty(strPr) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "PROVINCIA NON DEFINITA. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'CAUSALE
            strCausale = row.Cells(CellIdxT.DesCausale).Text
            If String.IsNullOrEmpty(strCausale) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CAUSALE NON DEFINITA. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'giu160215 Split Payment strSplitIVA
            strSplitIVA = row.Cells(CellIdxT.SplitIVA).Text
            If String.IsNullOrEmpty(strSplitIVA) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "SPLIT PAYMENT IVA NON DEFINITO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            '---------
            'RICALCOLO SCADENZE
            Try
                'giu110215 FatturePA Split Payment dal 2015 e solo per le PA
                'LA DIFFERENZA E' SOLO PER IL TOTALE DOCUMENTO DA MEMORIZZARE NELLO SCADENZARIO: IMP+IVA OPPURE SOLO IMP.
                'giu300315 lo split iva non è obbligatorio per le fatture PA (BARBARA)
                'GIU180118 ''ritacconto() E SPLIT QUINDI USO IL NETTO A PAGARE PER TUTTI I CASI
                ' ''If CBool(strSplitIVA.Trim) = True Then
                ' ''    If Calcola_Scadenze(CLng(myID), strNDoc, CDate(txtDataFattura.Text), CInt(strCPag), CDec(strTotaleImp), Valuta, StrErrore) = False Then
                ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''        ModalPopup.Show("Errore", "RICALCOLO SCADENZE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''        Exit Sub
                ' ''    End If
                ' ''Else
                ' ''    If Calcola_Scadenze(CLng(myID), strNDoc, CDate(txtDataFattura.Text), CInt(strCPag), CDec(strTotaleDoc), Valuta, StrErrore) = False Then
                ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''        ModalPopup.Show("Errore", "RICALCOLO SCADENZE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''        Exit Sub
                ' ''    End If
                ' ''End If
                '-------------------------------------------------------------------------------
                If Calcola_Scadenze(CLng(myID), strNDoc, CDate(txtDataFattura.Text), CInt(strCPag), CDec(strTotNettoPagare), Valuta, StrErrore) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "RICALCOLO SCADENZE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                '-------------------------------------------------------------------------------
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "RICALCOLO SCADENZE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            'OK POSSO CREARE LA FATTURA
            NDoc = 0 : NRev = 0
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                If chkFatturaPA.Checked = False Then 'GIU110814
                    NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
                Else
                    NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
                End If
            Else
                ' ''Chiudi("Errore: " & "Caricamento parametri generali.")
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            NDocOK = NDoc
            '-
            If AggiornaNumDoc(SWTD(TD.FatturaCommerciale), NDoc, NRev) = False Then
                Chiudi("Errore: " & "Aggiornamento numero documento")
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'OK CREAZIONE NUOVA FATTURA
            Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim SqlConn As New SqlConnection
            SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
            'giu200617
            Dim strValore As String = ""
            'Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            'esempio.CommandTimeout = myTimeOUT
            '---------------------------
            Dim SqlDbNewCmd As New SqlCommand

            SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFCTutte]"
            SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbNewCmd.Connection = SqlConn
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataFC", System.Data.SqlDbType.DateTime, 8))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_1", System.Data.SqlDbType.DateTime, 8))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_2", System.Data.SqlDbType.DateTime, 8))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_3", System.Data.SqlDbType.DateTime, 8))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_4", System.Data.SqlDbType.DateTime, 8))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza_5", System.Data.SqlDbType.DateTime, 8))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_1", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_2", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_3", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_4", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_5", System.Data.SqlDbType.Decimal, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
            '--
            SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
            SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
            SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
            SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
            SqlDbNewCmd.Parameters.Item("@DataFC").Value = CDate(txtDataFattura.Text)
            'Blocco Data Scadenza
            If IsNothing(lblDataScad1) Then lblDataScad1 = ""
            If String.IsNullOrEmpty(lblDataScad1) Then lblDataScad1 = ""
            If lblDataScad1.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_1").Value = CDate(lblDataScad1.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_1").Value = DBNull.Value
            End If
            If IsNothing(lblDataScad2) Then lblDataScad2 = ""
            If String.IsNullOrEmpty(lblDataScad2) Then lblDataScad2 = ""
            If lblDataScad2.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_2").Value = CDate(lblDataScad2.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_2").Value = DBNull.Value
            End If
            If IsNothing(lblDataScad3) Then lblDataScad3 = ""
            If String.IsNullOrEmpty(lblDataScad3) Then lblDataScad3 = ""
            If lblDataScad3.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_3").Value = CDate(lblDataScad3.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_3").Value = DBNull.Value
            End If
            If IsNothing(lblDataScad4) Then lblDataScad4 = ""
            If String.IsNullOrEmpty(lblDataScad4) Then lblDataScad4 = ""
            If lblDataScad4.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_4").Value = CDate(lblDataScad4.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_4").Value = DBNull.Value
            End If
            If IsNothing(lblDataScad5) Then lblDataScad5 = ""
            If String.IsNullOrEmpty(lblDataScad5) Then lblDataScad5 = ""
            If lblDataScad5.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_5").Value = CDate(lblDataScad5.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Data_Scadenza_5").Value = DBNull.Value
            End If
            'Blocco Scadenze RATE lblImpRata1
            If IsNothing(lblImpRata1) Then lblImpRata1 = ""
            If String.IsNullOrEmpty(lblImpRata1) Then lblImpRata1 = ""
            If lblImpRata1.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Rata_1").Value = CDec(lblImpRata1.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Rata_1").Value = 0
            End If
            If IsNothing(lblImpRata2) Then lblImpRata2 = ""
            If String.IsNullOrEmpty(lblImpRata2) Then lblImpRata2 = ""
            If lblImpRata2.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Rata_2").Value = CDec(lblImpRata2.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Rata_2").Value = 0
            End If
            If IsNothing(lblImpRata3) Then lblImpRata3 = ""
            If String.IsNullOrEmpty(lblImpRata3) Then lblImpRata3 = ""
            If lblImpRata3.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Rata_3").Value = CDec(lblImpRata3.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Rata_3").Value = 0
            End If
            If IsNothing(lblImpRata4) Then lblImpRata4 = ""
            If String.IsNullOrEmpty(lblImpRata4) Then lblImpRata4 = ""
            If lblImpRata4.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Rata_4").Value = CDec(lblImpRata4.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Rata_4").Value = 0
            End If
            If IsNothing(lblImpRata5) Then lblImpRata5 = ""
            If String.IsNullOrEmpty(lblImpRata5) Then lblImpRata5 = ""
            If lblImpRata5.Trim <> "" Then
                SqlDbNewCmd.Parameters.Item("@Rata_5").Value = CDec(lblImpRata5.Trim)
            Else
                SqlDbNewCmd.Parameters.Item("@Rata_5").Value = 0
            End If
            '---------------------------------------------------
            SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
            SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            Try
                myID = ""
                SqlConn.Open()
                SqlDbNewCmd.CommandTimeout = myTimeOUT
                SqlDbNewCmd.ExecuteNonQuery()
                SqlConn.Close()
                myID = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
            Catch ExSQL As SqlException
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                            & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            Catch Ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                            & "Errore SQL: " + Ex.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End Try
            Try
                If IsNothing(myID) Then
                    myID = ""
                End If
                If String.IsNullOrEmpty(myID) Then
                    myID = ""
                End If
                If Not IsNumeric(myID) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                                & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End Try
            '''Dim IDOKFC As Long = CLng(myID)
            '''txtPrimoNFattura.Text = NDoc + 1
            '''txtPrimoNFattura.Text = FormattaNumero(txtPrimoNFattura.Text.Trim)
            '''BuidDett()
            '''Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
            If CKScontiXStampa(myID, StrErrore) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Fattura da DDT", "Verificare i dati della fattura appena creata <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche il DDT che sia con lo stato Fatturato </span></strong><br>" _
                            & "Errore verifica Sconti per Stampa: " + StrErrore.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Next
        'ok terminato
        'OK FATTO
        Session(MODALPOPUP_CALLBACK_METHOD) = "StampaDocumentiAll1"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CallChiudi"
        ModalPopup.Show("Crea TUTTE le Fatture da DDT selezionati", _
            "Creazione fatture avvenuta con sussesso. <br> <strong><span> " & _
            "Vuole stampare tutte le fatture appena create ?</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        Exit Sub
    End Sub
    'giu150312
    Private Function Calcola_Scadenze(ByVal IdDocumento As Long, ByVal strNDoc As String, ByVal DataDoc As DateTime, ByVal CPag As Integer, ByVal TotaleDoc As Decimal, ByVal Valuta As String, ByRef strErrore As String) As Boolean

        '------------------------------------
        'Valuta per i decimali per il calcolo
        Dim DecimaliVal As String = Session(CSTDECIMALIVALUTADOC)
        If IsNothing(DecimaliVal) Then DecimaliVal = ""
        If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
        If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
            DecimaliVal = GetDatiValute(Valuta, strErrore).Decimali
            If IsNothing(DecimaliVal) Then DecimaliVal = ""
            If String.IsNullOrEmpty(DecimaliVal) Then DecimaliVal = ""
            If DecimaliVal = "" Or Not IsNumeric(DecimaliVal) Then
                DecimaliVal = "2" 'Euro
            End If
        End If
        '------------------------------------
        Dim DecimaliValuta As Integer = Int(DecimaliVal)

        Calcola_Scadenze = True
        '' '' ''    'giu200510 DA FARE AL MOMENTO è DISABILITATO
        '' '' ''    If SWModScad = True Then
        '' '' ''        If MyTipoDoc = "FC" Or MyTipoDoc = "FT" Or MyTipoDoc = "NC" Or MyTipoDoc = "NZ" Then
        '' '' ''            '''cmdAggiornaScad.Visible = True: cmdAggiornaScad.Enabled = False
        '' '' ''        Else
        '' '' ''            SWChangeTipoPag = True
        '' '' ''        End If
        '' '' ''    End If
        '' '' ''    '----------
        '' '' ''    '''giu120410 giu140410
        '' '' ''    Dim TotRate As Currency
        '' '' ''    If SWModScad = True Then
        '' '' ''        If UCase(lblOperazione.caption) <> "NUOVO" And SWChangeTipoPag = False Then
        '' '' ''            Dim i As Integer : Dim ImpScad As Currency : TotRate = 0
        '' '' ''            For i = 1 To 5
        '' '' ''                ImpScad = IIf(IsNull(rsordiniT("Rata_" + Trim(i))), 0, rsordiniT("Rata_" + Trim(i)))
        '' '' ''                If ImpScad = 0 Then
        '' '' ''                    lblDataScadenza(i - 1).caption = ""
        '' '' ''                    lblImportoScadenza(i - 1).caption = ""
        '' '' ''                Else
        '' '' ''                    lblDataScadenza(i - 1).caption = IIf(IsNull(rsordiniT("Data_Scadenza_" + Trim(i))), "", Format(rsordiniT("Data_Scadenza_" + Trim(i)), "dd/mm/yyyy"))
        '' '' ''                    lblImportoScadenza(i - 1).caption = Formatta_Valute(ImpScad, DecimaliValutaFinito)
        '' '' ''                End If
        '' '' ''                TotRate = TotRate + ImpScad
        '' '' ''            Next i
        '' '' ''            lblTotRate.caption = Formatta_Valute(TotRate, DecimaliValutaFinito)
        '' '' ''            Exit Sub
        '' '' ''        End If
        '' '' ''    End If
        '' '' ''    SWChangeTipoPag = False
        '' '' ''    '---------
        '' '' ''    'giu200510 DA FARE AL MOMENTO è DISABILITATO

        Dim Arr_Giorni(5) As String
        Dim Arr_Scad(5) As String
        Dim Arr_Impo(5) As Decimal
        'Dim NumDec As Integer adesso DecimaliValuta
        Dim NumRate As Integer = 0
        Dim Tot_Rate As Decimal = 0
        Dim ind As Integer = 0

        'Codice Pagamento (da TD0)
        If CPag = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End If
        
        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsPag As New DataSet
        Dim rowPag() As DataRow
        strSQL = "Select * From Pagamenti WHERE Codice = " & CPag.ToString.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, dsPag)
            If (dsPag.Tables.Count > 0) Then
                If (dsPag.Tables(0).Rows.Count > 0) Then
                    rowPag = dsPag.Tables(0).Select()
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Codice pagamento nella tabella Pagamenti: " & CPag.ToString.Trim & " N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Calcola_Scadenze = False
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Codice pagamento nella tabella Pagamenti: " & CPag.ToString.Trim & " N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Calcola_Scadenze = False
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura pagamenti: " & Ex.Message & " N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End Try
        Comodo = IIf(IsDBNull(rowPag(0).Item("Numero_Rate")), "0", rowPag(0).Item("Numero_Rate"))
        If String.IsNullOrEmpty(Comodo) Then Comodo = "0"
        NumRate = CLng(Comodo)

        If CompilaScadenze(rowPag, DataDoc, TotaleDoc, _
            DecimaliValuta, NumRate, Arr_Giorni, Arr_Scad, Arr_Impo, Tot_Rate, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nella compilazione Scadenze" & IIf(strErrore.Trim <> "", ": " & strErrore.Trim, "") & " N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End If
        lblDataScad1 = "" : lblDataScad2 = "" : lblDataScad3 = "" : lblDataScad4 = "" : lblDataScad1 = ""
        lblImpRata1 = "" : lblImpRata2 = "" : lblImpRata3 = "" : lblImpRata4 = "" : lblImpRata5 = ""
        Dim TotRate As Decimal = 0
        For ind = 0 To UBound(Arr_Scad) - 1
            Select Case ind
                Case 0
                    lblDataScad1 = Arr_Scad(ind)
                    lblImpRata1 = Arr_Impo(ind)
                    If lblImpRata1.Trim <> "" Then
                        If CDec(lblImpRata1.Trim) > 0 Then
                            lblImpRata1 = FormattaNumero(CDec(lblImpRata1.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata1.Trim)
                        Else
                            lblImpRata1 = ""
                        End If
                    End If
                Case 1
                    lblDataScad2 = Arr_Scad(ind)
                    lblImpRata2 = Arr_Impo(ind)
                    If lblImpRata2.Trim <> "" Then
                        If CDec(lblImpRata2.Trim) > 0 Then
                            lblImpRata2 = FormattaNumero(CDec(lblImpRata2.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata2.Trim)
                        Else
                            lblImpRata2 = ""
                        End If
                    End If
                Case 2
                    lblDataScad3 = Arr_Scad(ind)
                    lblImpRata3 = Arr_Impo(ind)
                    If lblImpRata3.Trim <> "" Then
                        If CDec(lblImpRata3.Trim) > 0 Then
                            lblImpRata3 = FormattaNumero(CDec(lblImpRata3.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata3.Trim)
                        Else
                            lblImpRata3 = ""
                        End If
                    End If
                Case 3
                    lblDataScad4 = Arr_Scad(ind)
                    lblImpRata4 = Arr_Impo(ind)
                    If lblImpRata4.Trim <> "" Then
                        If CDec(lblImpRata4.Trim) > 0 Then
                            lblImpRata4 = FormattaNumero(CDec(lblImpRata4.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata4.Trim)
                        Else
                            lblImpRata4 = ""
                        End If
                    End If
                Case 4
                    lblDataScad5 = Arr_Scad(ind)
                    lblImpRata5 = Arr_Impo(ind)
                    If lblImpRata5.Trim <> "" Then
                        If CDec(lblImpRata5.Trim) > 0 Then
                            lblImpRata5 = FormattaNumero(CDec(lblImpRata5.Trim), DecimaliValuta)
                            TotRate = TotRate + CDec(lblImpRata5.Trim)
                        Else
                            lblImpRata5 = ""
                        End If
                    End If
            End Select
        Next ind
        LblTotaleRate = FormattaNumero(TotRate, DecimaliValuta)
        If Tot_Rate <> TotaleDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Totale documento (" & FormattaNumero(TotaleDoc, DecimaliValuta) & ") diverso dal Totale Rate (" & FormattaNumero(TotRate, DecimaliValuta) & ") - N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            Calcola_Scadenze = False
            Exit Function
        End If
    End Function

    'giu210423
    Private Function CKScontiXStampa(ByVal IDDocFC As String, ByRef strErrore As String) As Boolean
        'giu210423 controllo sconti per la stampa
        CKScontiXStampa = False
        strErrore = ""
        Dim SWSconti As Boolean = False
        Try
            Session(IDDOCUMENTI) = IDDocFC.Trim
            Dim dvDettDoc As DataView
            dvDettDoc = SqlDSPrevDByIDDocumenti.Select(DataSourceSelectArguments.Empty)
            If (dvDettDoc Is Nothing) Then
                strErrore = "Errore Verifica Sconti: NON TROVATO IDDOCUMENTO"
                Exit Function
            ElseIf dvDettDoc.Count > 0 Then
                dvDettDoc.RowFilter = "(Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0)"
                If dvDettDoc.Count > 0 Then
                    SWSconti = True
                End If
            Else
                strErrore = "Errore Verifica Sconti: NESSUN DETTAGLIO PRESENTE"
                Exit Function
            End If
            'OK
            Dim ELDocToPrintSCY As New List(Of String)
            Dim ELDocToPrintSCN As New List(Of String)
            If SWSconti = True Then
                If Not IsNothing(Session(EL_DOC_TOPRINT_SCY)) Then
                    ELDocToPrintSCY = Session(EL_DOC_TOPRINT_SCY)
                End If
                ELDocToPrintSCY.Add(IDDocFC.ToString.Trim)
                Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
            Else
                If Not IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
                    ELDocToPrintSCN = Session(EL_DOC_TOPRINT_SCN)
                End If
                ELDocToPrintSCN.Add(IDDocFC.ToString.Trim)
                Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
            End If
            CKScontiXStampa = True
        Catch ex As Exception
            strErrore = "Errore Verifica Sconti: " + ex.Message.Trim
        End Try

    End Function
    'giu160312 giu210422 non mi serve stampare quindi non la uso
    'Private Function OKStampaDoc(ByVal IDDocFC As Long, ByVal NDoc As Long, ByRef strErrore As String) As Boolean
    '    OKStampaDoc = True
    '    Dim DsPrinWebDoc As New DSPrintWeb_Documenti
    '    Dim ObjReport As New Object
    '    Dim ClsPrint As New Documenti
    '    strErrore = ""
    '    Dim SWSconti As Boolean = False
    '    Dim ELDocToPrintSCY As New List(Of String)
    '    Dim ELDocToPrintSCN As New List(Of String)
    '    Try
    '        DsPrinWebDoc.Clear() 'GIU020512
    '        If ClsPrint.StampaDocumento(IDDocFC.ToString.Trim, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, "Cli", DsPrinWebDoc, ObjReport, SWSconti, strErrore) Then
    '            Session(CSTObjReport) = ObjReport
    '            Session(CSTDsPrinWebDoc) = DsPrinWebDoc
    '            If SWSconti = True Then
    '                Session(CSTSWScontiDoc) = 1
    '                'giu040512 giu070512
    '                If Not IsNothing(Session(EL_DOC_TOPRINT_SCY)) Then
    '                    ELDocToPrintSCY = Session(EL_DOC_TOPRINT_SCY)
    '                End If
    '                ELDocToPrintSCY.Add(IDDocFC.ToString.Trim)
    '                Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
    '            Else
    '                Session(CSTSWScontiDoc) = 0
    '                'giu040512 giu070512
    '                If Not IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
    '                    ELDocToPrintSCN = Session(EL_DOC_TOPRINT_SCN)
    '                End If
    '                ELDocToPrintSCN.Add(IDDocFC.ToString.Trim)
    '                Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
    '            End If
    '            Session(CSTSWConfermaDoc) = 0
    '            'GIU210423 NON VA BENE 
    '            '''If ChkStampaSingFC.Checked = True Then
    '            '''    Session(ATTESA_CALLBACK_METHOD) = "CreaFattureTutte"
    '            '''    Session(CSTNOBACK) = 1
    '            '''    Attesa.ShowStampa("Stampa Fattura N° : " & FormattaNumero(NDoc.ToString), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
    '            '''Else
    '            '''    CreaFattureTutte()
    '            '''End If
    '        Else
    '            'L'ERRORE è INIZIALIZZATO ALLA FUNZIONE STAMPADOCUMENTO
    '            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
    '            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    '            ' ''ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
    '            OKStampaDoc = False
    '            'giu040512
    '            ELDocToPrintSCN = Nothing
    '            Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
    '            ELDocToPrintSCY = Nothing
    '            Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
    '        End If
    '    Catch ex As Exception
    '        OKStampaDoc = False
    '        'giu040512
    '        ELDocToPrintSCN = Nothing
    '        Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
    '        ELDocToPrintSCY = Nothing
    '        Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
    '        ' ''Chiudi("Errore:" & ex.Message)
    '        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
    '        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    '        ' ''ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
    '        strErrore = "Errore:" & ex.Message
    '    End Try

    'End Function

    Public Sub StampaDocumentiAll1()
        If IsNothing(Session(EL_DOC_TOPRINT_SCY)) Then
            StampaDocumentiAll2()
            Exit Sub
        End If
        '-----------------------------------------------------------------------------
        'giu030512 ESEMPIO COME STAMPARE TUTTI I DOCUMENTI PERSONALIZZATI (DDT,FC,etc)
        '-----------------------------------------------------------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Dim SWSconti As Boolean = False
        Dim SWRitAcc As Boolean = False
        DsPrinWebDoc.Clear()

        '--------
        Dim SWOk As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ELDocToPrintSCY As List(Of String) = Session(EL_DOC_TOPRINT_SCY)
        Dim TotFatture As Integer = 0
        If (ELDocToPrintSCY.Count > 0) Then
            TotFatture = ELDocToPrintSCY.Count
            Try
                For Each Codice As String In ELDocToPrintSCY
                    NumInt = Codice
                    SWOk = ClsPrint.StampaDocumentiDalAl(NumInt, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, SWRitAcc, StrErrore)
                    If SWOk = False Then Exit For
                Next
                If SWOk = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante la stampa fatture con SCONTI. N° Interno (" & NumInt & ") Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTSWScontiDoc) = 1
                Session(CSTSWConfermaDoc) = 0
            Catch ex As Exception
                SWOk = False
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante la stampa fatture con SCONTI. N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else
            StampaDocumentiAll2()
            Exit Sub
        End If
        ObjDB = Nothing
        '---------
        Session(ATTESA_CALLBACK_METHOD) = "StampaDocumentiAll2"
        Session(CSTNOBACK) = 1
        Dim SWOKAll2 As Boolean = True
        If IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
            SWOKAll2 = False
        Else
            Dim ELDocToPrintSCN As List(Of String) = Session(EL_DOC_TOPRINT_SCN)
            If (ELDocToPrintSCN.Count > 0) Then
                'nulla
            Else
                SWOKAll2 = False
            End If
        End If
        If SWOKAll2 = True Then
            Attesa.ShowStampaAll1("Totale Fatture con SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Fatture con SCONTI")
        Else
            Attesa.ShowStampaAll2("Totale Fatture con SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Fatture con SCONTI")
        End If
    End Sub
    Public Sub StampaDocumentiAll2()
        If IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
            Exit Sub
        End If
        '-----------------------------------------------------------------------------
        'giu030512 ESEMPIO COME STAMPARE TUTTI I DOCUMENTI PERSONALIZZATI (DDT,FC,etc)
        '-----------------------------------------------------------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Dim SWSconti As Boolean = False
        Dim SWRitAcc As Boolean = False
        DsPrinWebDoc.Clear()

        '--------
        Dim SWOk As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ELDocToPrintSCN As List(Of String) = Session(EL_DOC_TOPRINT_SCN)
        Dim TotFatture As Integer = 0
        If (ELDocToPrintSCN.Count > 0) Then
            TotFatture = ELDocToPrintSCN.Count
            Try
                For Each Codice As String In ELDocToPrintSCN
                    NumInt = Codice
                    SWOk = ClsPrint.StampaDocumentiDalAl(NumInt, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, SWRitAcc, StrErrore)
                    If SWOk = False Then Exit For
                Next
                If SWOk = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante la stampa fatture NO SCONTI. N° Interno (" & NumInt & ") Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTSWScontiDoc) = 0
                Session(CSTSWRitAcc) = IIf(SWRitAcc = True, 1, 0)
                Session(CSTSWConfermaDoc) = 0
            Catch ex As Exception
                SWOk = False
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante la stampa fatture NO SCONTI. N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else
            Exit Sub
        End If
        ObjDB = Nothing
        '---------
        Session(ATTESA_CALLBACK_METHOD) = "CallChiudi"
        Session(CSTNOBACK) = 1
        Attesa.ShowStampaAll2("Totale Fatture NO SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Fatture NO SCONTI")
    End Sub
    Public Sub CallChiudi()
        Chiudi("")
    End Sub

    Public Sub DisableAll()
        ddlRicerca.Enabled = False : checkParoleContenute.Enabled = False : btnRicerca.Enabled = False
        txtDataA.Enabled = False : imgBtnShowCalendarA.Enabled = False : chkTipoFT.Enabled = False : ddlTipoFattur.Enabled = False
        txtDalNDDT.Enabled = False : txtAlNDDT.Enabled = False : btnCercaDDTNum.Enabled = False : txtPrimoNFattura.Enabled = False : txtDataFattura.Enabled = False : ImageButtonDF.Enabled = False
        chkFatturaPA.Enabled = False
        btnCreaFattura.Enabled = False
        btnCreaTutte.Enabled = False
        btnModifica.Enabled = False
    End Sub
    Public Sub EnableAll()
        ddlRicerca.Enabled = True : checkParoleContenute.Enabled = True : btnRicerca.Enabled = True
        txtDataA.Enabled = True : imgBtnShowCalendarA.Enabled = True : chkTipoFT.Enabled = True
        txtDalNDDT.Enabled = True : txtAlNDDT.Enabled = True : btnCercaDDTNum.Enabled = True : txtPrimoNFattura.Enabled = False : txtDataFattura.Enabled = True : ImageButtonDF.Enabled = True
        chkFatturaPA.Enabled = True
        If chkTipoFT.Checked = True Then ddlTipoFattur.Enabled = True
        btnCreaFattura.Enabled = True
        btnCreaTutte.Enabled = True
        btnModifica.Enabled = True
    End Sub

    Private Sub chkFatturaPA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFatturaPA.CheckedChanged
        Dim NDoc As Long = 0 : Dim StrErrore As String = ""
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            If chkFatturaPA.Checked = False Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            End If
        Else
            Chiudi("Errore Caricamento parametri generali.: " & StrErrore)
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim CkNumDoc As Long = CheckNumDoc(StrErrore)
        If CkNumDoc = -1 Then
            Chiudi("Errore: Verifica N° Documento da impegnare. " & StrErrore)
            Exit Sub
        End If
        If NDoc <> CkNumDoc Then
            txtPrimoNFattura.BackColor = SEGNALA_KO
            txtPrimoNFattura.ToolTip = "Attenzione, ci sono dei numeri documento da recuperare!!!"
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "Attenzione, ci sono dei numeri documento da recuperare!!!"
            ModalPopup.Show("N° Fattura fuori sequenza", "<strong><span> FATTURA COMMERCIALE" _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(NDoc) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_ALERT)
        Else
            txtPrimoNFattura.BackColor = SEGNALA_OK
            txtPrimoNFattura.ToolTip = ""
        End If
        '-
        txtPrimoNFattura.Text = FormattaNumero(NDoc.ToString.Trim)
        '---
        BuidDett()
    End Sub
End Class