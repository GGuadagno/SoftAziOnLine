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

Partial Public Class WUC_FatturazioneRiepDDT
    Inherits System.Web.UI.UserControl

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = ""
    Const DefTipoFatt As String = "TipoFatturazioneDEFAULT"
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

    Const AvvisoTF As String = "AvvisoTFDiv" 'alb060213

    Dim arrMesi() As String = {"Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno", "Luglio", "Agosto", "Settembre", "Ottobre", "Novembre", "Dicembre"}
#Region "Def per gestire il GRID dettagli"
    Private aDataViewDettFCR As DataView
    Private SqlAdapDocDettFCR As SqlDataAdapter
    Private SqlConnDocDettFCR As SqlConnection
    Private SqlDbSelectCmdDettFCR As SqlCommand
    Private DsDocumentiDettFCR As New DSDocumenti

    Private Enum CellIdxT
        checkSel = 1
        NumDoc = 2
        DataDoc = 3
        Riferimento = 4
        DesCausale = 5
        DesTipoFatt = 6
        TotaleDoc = 7
        TotNettoPagare = 8
        Cod_Valuta = 9
        Cod_Pagamento = 10
        DesPagamento = 11
        ABI = 12
        CAB = 13
        Localita = 14
        Pr = 15
        CAP = 16
        PI = 17
        CF = 18
        IDDoc = 19
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
    Private Enum CellIdxDFC
        Riga = 0
        NDoc = 1
        CodArt = 2
        DesArt = 3
        UM = 4
        QtaOr = 5
        QtaEv = 6
        QtaRe = 7
        QtaAl = 8
        IVA = 9
        Prz = 10
        ScV = 11
        Sc1 = 12
        Importo = 13
        ScR = 14
    End Enum
#End Region
#Region "Imposta Command e DataAdapter"
    Private Function SetCdmDAdp() As Boolean

        SqlConnDocDettFCR = New SqlConnection
        SqlAdapDocDettFCR = New SqlDataAdapter
        SqlDbSelectCmdDettFCR = New SqlCommand

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDocDettFCR.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmdDettFCR.CommandText = "get_DocDByIDDocumenti"
        SqlDbSelectCmdDettFCR.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmdDettFCR.Connection = Me.SqlConnDocDettFCR
        SqlDbSelectCmdDettFCR.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
        SqlDbSelectCmdDettFCR.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        
        SqlAdapDocDettFCR.SelectCommand = SqlDbSelectCmdDettFCR
        Session("aSqlAdapFCR") = SqlAdapDocDettFCR

    End Function
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_FatturazioneRiepDDT.aspx?labelForm=Fatturazione riepilogativa per cliente"
        SetCdmDAdp()
        '---
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
            btnSelTutte.Visible = False
        End If
        '----------------------------
        ' ''If (sTipoUtente.Equals(CSTVENDITE)) Then
        ' ''    chkTipoFT.Enabled = False : chkTipoFT.Checked = False : ddlTipoFattur.Enabled = False
        ' ''    btnCreaFattura.Visible = False
        ' ''    btnSelTutte.Visible = False
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
        SqlDSCliFor.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSTipoFatt.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        If (Not IsPostBack) Then
            LblTotDocumento.Text = "0,00"
            DsDocumentiDettFCR.Clear()
            DsDocumentiDettFCR.AcceptChanges()
            '-
            Session("aDataViewFCR") = aDataViewDettFCR
            Session("aSqlAdapFCR") = SqlAdapDocDettFCR
            Session("aDsDettFCR") = DsDocumentiDettFCR
            aDataViewDettFCR = New DataView(DsDocumentiDettFCR.DocumentiDForFCRiep)
            If aDataViewDettFCR.Count > 0 Then aDataViewDettFCR.Sort = "Numero,Riga"
            GridViewPrevDNEWFatt.DataSource = aDataViewDettFCR
            Session("aDataViewFCR") = aDataViewDettFCR
            'SetBtnFCR(False)
            GridViewPrevDNEWFatt.DataBind()
            If GridViewPrevDNEWFatt.Rows.Count = 0 Then
                SetBtnFCR(False)
                btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
            Else
                SetBtnFCR(True)
            End If
            '------------------------------------------------------------------------------------
            Try
                'giu160312 richiesta di Cinzia DATA del giorno
                ' ''txtDataA.Text = Format(ControllaDataDoc, FormatoData)
                ' ''If CDate(txtDataA.Text.Trim) = DATANULL Then
                ' ''    txtDataA.Text = Format(Now, FormatoData)
                ' ''    txtDataFattura.Text = Format(Now, FormatoData)
                ' ''Else
                ' ''    txtDataFattura.Text = txtDataA.Text
                ' ''End If
                'giu160118
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
                txtPrimoNFattura.Text = FormattaNumero(NDoc.ToString.Trim)
                '---
                btnDeSelTutte.Text = "Deseleziona tutto"
                btnDeSelRiga.Text = "Deseleziona riga"
                btnModifica.Text = "Modifica DDT"
                btnCreaFattura.Text = "Crea Fattura riepilogativa"
                '-----
                Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                'GIU171219
                If App.GetDatiAbilitazioni(CSTABILAZI, "TipoFattRDEF", Session(DefTipoFatt), StrErrore) = True Then
                    If StrErrore.Trim <> "" Then
                        Session(DefTipoFatt) = ""
                    End If
                End If
                ddlTipoFattur.AutoPostBack = False : chkTipoFT.AutoPostBack = False
                ddlTipoFattur.DataBind()
                chkTipoFT.Checked = True
                Call PosizionaItemDDL(Session(DefTipoFatt), ddlTipoFattur)
                ddlTipoFattur.AutoPostBack = True : chkTipoFT.AutoPostBack = True
                If ddlTipoFattur.SelectedValue.ToString.Trim = "" Then
                    SqlDSCliFor.SelectParameters("TipoFatt").DefaultValue = String.Empty
                Else
                    SqlDSCliFor.SelectParameters("TipoFatt").DefaultValue = ddlTipoFattur.SelectedValue
                End If
                ddlRicerca.DataBind()
                '--
                If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
                    chkTipoFT.AutoPostBack = False : ddlTipoFattur.AutoPostBack = False 'giu161219
                    chkTipoFT.Enabled = False : chkTipoFT.Checked = False : ddlTipoFattur.Enabled = False
                    btnCreaFattura.Visible = False
                    btnSelTutte.Visible = False
                End If
                BuidDett()

                btnModifica.Enabled = False

                'fatto sopra SetBtnFCR(False)
                btnRicerca.Text = "Visualizza DDT"
                Session(SWOP) = SWOPNESSUNA

                'giu210512 controllo numerazione fattura se corretta
                '--------------------------------------------
                Dim CkNumDoc As Long = CheckNumDoc(StrErrore)
                If CkNumDoc = -1 Then
                    Chiudi("Errore: Verifica N° Documento da impegnare. " & StrErrore)
                    Exit Sub
                End If
                If NDoc <> CkNumDoc Then
                    txtPrimoNFattura.Text = FormattaNumero(NDoc.ToString.Trim)
                    txtPrimoNFattura.BackColor = SEGNALA_KO
                    txtPrimoNFattura.ToolTip = "Attenzione, ci sono dei numeri documento da recuperare!!!"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("N° Fattura fuori sequenza", "<strong><span> FATTURA COMMERCIALE (Riepilogativa)" _
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
            Session(AvvisoTF) = True 'alb060213 gestione avviso cambio tipo fatturazione 
        End If
    End Sub
    'giu210512
    'giu230312 giu260312 Recupero Numeri non impegnati
    'giu260312 verifica la sequenza se è completa
    Private Function CheckNumDoc(ByRef strErrore As String) As Long
        'GIU110814
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
            btnSelTutte.Visible = False
        End If
        If myStato.Trim = "Fatturato" Then
            btnCreaFattura.Visible = False
            btnSelTutte.Visible = False
            btnModifica.Visible = False
        End If
        If myStato.Trim = "OK trasf.in CoGe" Then
            btnCreaFattura.Visible = False
            btnSelTutte.Visible = False
            btnModifica.Visible = False
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Then
            btnCreaFattura.Visible = False
            btnSelTutte.Visible = False
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito) Then
            btnCreaFattura.Visible = False
            btnSelTutte.Visible = False
        End If
        If Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori) Then
            btnCreaFattura.Visible = False
            btnSelTutte.Visible = False
        End If

    End Sub

#Region " Funzioni e Procedure"
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnSelTutte.Enabled = Valore
        btnSelRiga.Enabled = Valore
        btnDeSelTutte.Enabled = Valore
        btnDeSelRiga.Enabled = Valore
        btnModifica.Enabled = Valore
    End Sub
    Private Sub SetBtnFCR(ByVal Valore As Boolean)
        btnCreaFattura.Enabled = Valore
        ' ''btnAnnullaFattura.Enabled = Valore
    End Sub
#End Region

#Region " Ordinamento e ricerca"

    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & ddlRicerca.SelectedValue & "'"

        'GIU171219
        If chkTipoFT.Checked = True Then
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
        SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtDalNDDT.Text.Trim & " AND "
        SqlDSPrevTElenco.FilterExpression += "Numero <= " & txtAlNDDT.Text.Trim
        'giu140312
        If IsDate(txtDataA.Text.Trim) Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Data_Doc <= '" & CDate(txtDataA.Text.Trim) & "'"
        End If
        '----------
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
        'GIU160118
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        If chkSpltIVA.Checked = False Then
            SqlDSPrevTElenco.FilterExpression += "ISNULL(SplitIVA,0)=0"
        Else
            SqlDSPrevTElenco.FilterExpression += "ISNULL(SplitIVA,0)<>0"
        End If
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        If chkRitAcconto.Checked = False Then
            SqlDSPrevTElenco.FilterExpression += "ISNULL(RitAcconto,0)=0"
        Else
            SqlDSPrevTElenco.FilterExpression += "ISNULL(RitAcconto,0)<>0"
        End If
        '---------
        SqlDSPrevTElenco.DataBind()
        'giu110412
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                'giu160512 giu170512
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
                If (checkSel.Checked) Then
                    btnSelRiga.Enabled = False
                    btnDeSelRiga.Enabled = True
                Else
                    btnSelRiga.Enabled = True
                    btnDeSelRiga.Enabled = False
                End If
                Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                Dim Stato As String = ""
                '---------
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
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            'TotaleDoc
            If IsNumeric(e.Row.Cells(CellIdxT.TotaleDoc).Text) Then
                If CDec(e.Row.Cells(CellIdxT.TotaleDoc).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.TotaleDoc).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.TotaleDoc).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.TotaleDoc).Text = ""
                End If
            End If
            'TotNettoPagare
            If IsNumeric(e.Row.Cells(CellIdxT.TotNettoPagare).Text) Then
                If CDec(e.Row.Cells(CellIdxT.TotNettoPagare).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.TotNettoPagare).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.TotNettoPagare).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.TotNettoPagare).Text = ""
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
    Private Sub GridViewPrevDNEWFatt_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevDNEWFatt.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxDFC.Riga).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.Riga).Text) = 0 Then
                    e.Row.Cells(CellIdxDFC.Riga).ForeColor = Drawing.Color.Blue
                    '-
                    e.Row.Cells(CellIdxDFC.NDoc).ForeColor = Drawing.Color.Blue
                    '-
                    e.Row.Cells(CellIdxDFC.DesArt).ForeColor = Drawing.Color.Blue
                End If
            End If
            '----
            If IsNumeric(e.Row.Cells(CellIdxDFC.QtaOr).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.QtaOr).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.QtaOr).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.QtaOr).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDFC.QtaOr).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDFC.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDFC.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.QtaAl).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.QtaAl).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.QtaAl).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.QtaAl).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDFC.QtaAl).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDFC.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxDFC.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDFC.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxDFC.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDFC.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDFC.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxDFC.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxDFC.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDFC.ScR).Text), 4).ToString
                Else
                    e.Row.Cells(CellIdxDFC.ScR).Text = ""
                End If
            End If
        End If
    End Sub

    Private Sub BuidDett()
        Session(IDDOCUMENTI) = ""
        SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & ddlRicerca.SelectedValue & "'"
        'GIU171219
        If chkTipoFT.Checked = True Then
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
        'GIU160118
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        If chkSpltIVA.Checked = False Then
            SqlDSPrevTElenco.FilterExpression += "ISNULL(SplitIVA,0)=0"
        Else
            SqlDSPrevTElenco.FilterExpression += "ISNULL(SplitIVA,0)<>0"
        End If
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        If chkRitAcconto.Checked = False Then
            SqlDSPrevTElenco.FilterExpression += "ISNULL(RitAcconto,0)=0"
        Else
            SqlDSPrevTElenco.FilterExpression += "ISNULL(RitAcconto,0)<>0"
        End If
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
                'giu160512 giu170512
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
                If (checkSel.Checked) Then
                    btnSelRiga.Enabled = False
                    btnDeSelRiga.Enabled = True
                Else
                    btnSelRiga.Enabled = True
                    btnDeSelRiga.Enabled = False
                End If
                Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
                Dim Stato As String = ""
                '---------
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
        OKAzzera()
        'BuidDett()
        'DisableAll()
        btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select().Length > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "OKRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Seleziona Cliente", "Attenzione, sono stati inclusi dei DDT. <br> " _
                            & "Siete sicuri di voler procedere ?", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        OKRicerca()
    End Sub
    Public Sub OKRicerca()
        OKAzzera()
        If UCase(btnRicerca.Text.Trim) = UCase("Visualizza DDT") Then
            BuidDett()
            DisableAll()
            StatoModSel(True)
        Else
            SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '00'"
            Session(IDDOCUMENTI) = ""
            EnableAll()
            StatoModSel(False)
        End If
        btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
    End Sub
    Public Sub OKAzzera()
        DsDocumentiDettFCR = Session("aDsDettFCR")
        DsDocumentiDettFCR.Clear()
        DsDocumentiDettFCR.AcceptChanges()
        LblTotDocumento.Text = "0,00"
        btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
        '-
        Session("aDataViewFCR") = aDataViewDettFCR
        Session("aSqlAdapFCR") = SqlAdapDocDettFCR
        Session("aDsDettFCR") = DsDocumentiDettFCR
        aDataViewDettFCR = New DataView(DsDocumentiDettFCR.DocumentiDForFCRiep)
        If aDataViewDettFCR.Count > 0 Then aDataViewDettFCR.Sort = "Numero,Riga"
        GridViewPrevDNEWFatt.DataSource = aDataViewDettFCR
        Session("aDataViewFCR") = aDataViewDettFCR
        'SetBtnFCR(False)
        GridViewPrevDNEWFatt.DataBind()
        If GridViewPrevDNEWFatt.Rows.Count = 0 Then
            SetBtnFCR(False)
        Else
            SetBtnFCR(True)
        End If
        '------------------------------------------------------------------------------------
        btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
    End Sub

#End Region

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
            '-
            If ddlTipoFattur.SelectedValue.ToString.Trim = "" Then
                SqlDSCliFor.SelectParameters("TipoFatt").DefaultValue = String.Empty
            Else
                SqlDSCliFor.SelectParameters("TipoFatt").DefaultValue = ddlTipoFattur.SelectedValue
            End If
            ddlRicerca.DataBind()
            ddlTipoFattur.Focus()
        Else
            ddlTipoFattur.Enabled = False
            SqlDSCliFor.SelectParameters("TipoFatt").DefaultValue = String.Empty
            ddlRicerca.DataBind()
            If Session(AvvisoTF) Then
                Session(AvvisoTF) = False
                Session(MODALPOPUP_CALLBACK_METHOD) = "OKAzzera"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Tipo fatturazione selezionato diverso da quello predefinito.", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
            End If
        End If
        OKAzzera()
        'BuidDett()
        'DisableAll()
    End Sub

    Private Sub Chiudi(ByVal strErrore As String)

        ' ''Try
        ' ''    'giu160512 ..\WebFormTables\
        ' ''    Response.Redirect("WF_Menu.aspx?labelForm=Menu principale" & strErrore)
        ' ''Catch ex As Exception
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        ' ''End Try
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
        If chkTipoFT.Checked = True Then
            If ddlTipoFattur.SelectedValue.Trim = "" Then
                'nulla per TIPO FATTURAZIONE
            Else
                SqlDSCliFor.SelectParameters("TipoFatt").DefaultValue = ddlTipoFattur.SelectedValue
                ddlRicerca.DataBind()
            End If
        End If
        
        If Not IsNumeric(txtDalNDDT.Text.Trim) Then txtDalNDDT.Text = "1"
        If Not IsNumeric(txtAlNDDT.Text.Trim) Then txtAlNDDT.Text = "999999999"
        If Val(txtDalNDDT.Text.Trim) > Val(txtAlNDDT.Text.Trim) Then
            txtDalNDDT.Text = "1"
            txtAlNDDT.Text = "999999999"
        End If
        
        BtnSetEnabledTo(False)
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        OKAzzera()
        BuidDett()
        DisableAll()

        If Session(AvvisoTF) And ddlTipoFattur.SelectedValue <> Session(DefTipoFatt) Then
            Session(AvvisoTF) = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Tipo fatturazione selezionato diverso da quello predefinito.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
    End Sub

    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged

        Try
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            'giu160512 giu170512
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            If (checkSel.Checked) Then
                btnSelRiga.Enabled = False
                btnDeSelRiga.Enabled = True
            Else
                btnSelRiga.Enabled = True
                btnDeSelRiga.Enabled = False
            End If
            ' ''Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            ' ''Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
            Dim Stato As String = ""
            '---------
            BtnSetByStato(Stato)
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End Try
    End Sub

    Private Sub btnCreaFattura_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaFattura.Click
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select().Length = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea Fattura riepilogativa", "Attenzione, nessun DDT è stato incluso.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsDate(txtDataFattura.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "DATA FATTURAZIONE ERRATA.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If CDate(txtDataFattura.Text.Trim).Date < CDate(ControllaDataDoc()).Date Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "DATA FATTURAZIONE NON PUO' ESSERE INFERIORE ALL'ULTIMA DATA FATTURA INSERITA.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu160118
        If CDate(txtDataFattura.Text.Trim).Date.Year <> Val(Session(ESERCIZIO).ToString.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "ANNO DOCUMENTO NON PUO' ESSERE DIVERSO DALL'ESERCIZIO IN CORSO: " & Session(ESERCIZIO).ToString.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Try
            If CDec(LblTotDocumento.Text.Trim) = 0 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "TOTALE FATTURA UGUALE A ZERO.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura totale fattura riepilogativa.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'CONTROLLO se tutti i DDT inclusi sono presenti in: DocumentiDForFCRiep 
        Session("PrimoIDDocumenti") = ""
        Dim SWOK As Boolean = True
        Dim strIDDoc As String = ""
        Dim strNDoc As String = "" : Dim strDataDoc As String = ""
        Dim myTotaleDDT As String = "" : Dim myTotaleFCRiep As Decimal = 0
        Dim myTotNettoPagare As String = ""
        'giu200323
        Dim strBloccoCliente As String = "" : Dim strErrore As String = ""
        Dim SWNoFatt As Boolean = False
        Dim SWNoPIVACF As Boolean = False
        Dim SWNoCodIPA As Boolean = False
        Dim swNoDestM As Boolean = False : Dim swNODatiCorr As Boolean = False : Dim swNoCVett As Boolean = False
        Dim objControllo As New Controllo
        Dim myID As String = ""
        '---------
        For Each rowCTR As GridViewRow In GridViewPrevT.Rows
            Dim checkSel As CheckBox = CType(rowCTR.FindControl("checkSel"), CheckBox)
            If checkSel.Checked = True Then
                strIDDoc = rowCTR.Cells(CellIdxT.IDDoc).Text
                If DsDocumentiDettFCR.DocumentiDForFCRiep.Select("IDDocumenti=" & strIDDoc.Trim).Length = 0 Then
                    SWOK = False
                    Exit For
                End If
                If String.IsNullOrEmpty(Session("PrimoIDDocumenti")) Then
                    Session("PrimoIDDocumenti") = strIDDoc.Trim
                End If
                'N°DOCUMENTO
                strNDoc = rowCTR.Cells(CellIdxT.NumDoc).Text
                If String.IsNullOrEmpty(strNDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUMERO DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'DATA DOCUMENTO
                strDataDoc = rowCTR.Cells(CellIdxT.DataDoc).Text
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
                'giu230323
                myID = rowCTR.Cells(CellIdxT.IDDoc).Text.Trim
                strBloccoCliente = ""
                SWNoFatt = objControllo.CKCliNoFattByIDDoc(myID, SWNoPIVACF, SWNoCodIPA, swNoDestM, swNODatiCorr, swNoCVett, strErrore)
                If strErrore.Trim = "" Then
                    Dim swOKK As Boolean = False
                    strBloccoCliente = "<strong><span>Attenzione!!!, Cliente:<br>"
                    If SWNoPIVACF Then
                        strBloccoCliente += "SENZA P.IVA/C.F.<br>"
                        swOKK = True
                    End If
                    If SWNoCodIPA Then
                        strBloccoCliente += "SENZA Codice IPA<br>"
                        swOKK = True
                    End If
                    If SWNoFatt Then
                        strBloccoCliente += "NON Fatturabile (non bloccante)<br>"
                        swOKK = True
                    End If
                    '''If swNoDestM Then
                    '''    strBloccoCliente += "SENZA Destinazione Merce<br>"
                    '''    swOKK = True
                    '''ElseIf swNODatiCorr Then
                    '''    strBloccoCliente += "SENZA Dati corriere<br>"
                    '''    swOKK = True
                    '''End If
                    strBloccoCliente += "</span></strong>"
                    If swOKK = False Then
                        strBloccoCliente = ""
                    End If
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore verifica Cliente", strErrore, WUC_ModalPopup.TYPE_ALERT)
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
                'TOTALE DOCUMENTO
                myTotaleDDT = rowCTR.Cells(CellIdxT.TotaleDoc).Text
                If myTotaleDDT = "" Then myTotaleDDT = "0"
                Try
                    'GIU180118 SOMMO IL NETTO DA PAGARE myTotaleFCRiep += CDec(myTotaleDDT)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CONTROLLO TOTALE DOCUMENTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
                'TOTALE NETTO PAGARE
                myTotNettoPagare = rowCTR.Cells(CellIdxT.TotNettoPagare).Text
                If myTotNettoPagare = "" Then myTotNettoPagare = "0"
                Try
                    myTotaleFCRiep += CDec(myTotNettoPagare)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CONTROLLO TOT.NETTO A PAGARE N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            End If
        Next
        If CDec(LblTotDocumento.Text.Trim) <> myTotaleFCRiep Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "CONTROLLO TOTALE DOCUMENTO. LA SOMMA DEI DDT DIFFERISCE DAL TOTALE FATTURA", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If SWOK = False Or Session("PrimoIDDocumenti") = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Session("PrimoIDDocumenti") = ""
            ModalPopup.Show("Errore", "Discrepanza dei DDT inclusi e selezionati. <br> " & _
                            "Annullare e ripetere l'inclusione dei DDT", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        objControllo = Nothing
        '----------------------------------------------------------------------------------------
        strErrore = ""
        Dim NDoc As Long = 0
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            If chkFatturaPA.Checked = False Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            End If
            txtPrimoNFattura.Text = FormattaNumero(NDoc)
        Else
            Session("PrimoIDDocumenti") = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattureTutte"
        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''ModalPopup.Show("Crea Fattura riepilogativa", "Confermi la creazione della Fattura N° : " & txtPrimoNFattura.Text & " ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
        Dim CkNumDoc As Long = CheckNumDoc(StrErrore)
        If CkNumDoc = -1 Then
            Chiudi("Errore: Verifica N° Documento da impegnare. " & StrErrore)
            Exit Sub
        End If
        If NDoc <> CkNumDoc Then
            txtPrimoNFattura.BackColor = SEGNALA_KO
            txtPrimoNFattura.ToolTip = "Attenzione, ci sono dei numeri documento da recuperare!!!"
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattureTutte"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea Fattura riepilogativa", "<strong><span> FATTURA COMMERCIALE (Riepilogativa)" _
                        & "</span></strong> <br> Confermi la creazione della Fattura N° : <strong><span>" _
                        & FormattaNumero(NDoc) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        Else
            txtPrimoNFattura.BackColor = SEGNALA_OK
            txtPrimoNFattura.ToolTip = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattureTutte"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea Fattura riepilogativa", "<strong><span> FATTURA COMMERCIALE (Riepilogativa)" _
                        & "</span></strong> <br> Confermi la creazione della Fattura N° : <strong><span>" _
                        & FormattaNumero(NDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        End If
        '-
        txtPrimoNFattura.Text = FormattaNumero(NDoc.ToString.Trim)
        '---
    End Sub
    Public Sub CreaFattureTutte() 'TUTTE
        Session(CSTDECIMALIVALUTADOC) = ""
        Dim StrErrore As String = ""
        Dim strNDoc As String = "" : Dim strDataDoc As String = "" : Dim strPr As String = "" : Dim strCausale As String = ""
        Dim myID As String = ""
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        Dim strCPag As String = "" : Dim strTotaleDoc As String = "" : Dim Valuta As String = ""
        Dim strABI As String = "" : Dim strCAB As String = ""
        'giu160312 richiesta di Cinzia
        Dim strPI As String = "" : Dim strCF As String = ""
        Dim myTotaleFCRiep As Decimal = 0
        Dim strTotNettoPagare As String = ""
        'CONTROLLO
        Dim strPeriodo As String = "" 'alb070213
        Dim MinData As DateTime = CDate("31/12/" & CStr(CInt(Session(ESERCIZIO)) + 1)) 'alb070213
        Dim MaxData As DateTime = CDate("01/01/1900") 'alb070213
        For Each rowCTR As GridViewRow In GridViewPrevT.Rows
            Dim checkSel As CheckBox = CType(rowCTR.FindControl("checkSel"), CheckBox)
            If checkSel.Checked = True Then
                'alb070213
                If chkTipoFT.Checked Then
                    If ddlTipoFattur.SelectedItem.Text Like "*mensile*" Then
                        If Not strPeriodo Like "*" & arrMesi(DatePart(DateInterval.Month, CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim)) - 1) & "*" Then
                            strPeriodo = strPeriodo & arrMesi(DatePart(DateInterval.Month, CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim)) - 1) & ", "
                        End If
                    Else
                        If CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim) < MinData Then
                            MinData = CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim)
                        End If
                        If CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim) > MaxData Then
                            MaxData = CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim)
                        End If
                    End If
                Else
                    If CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim) < MinData Then
                        MinData = CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim)
                    End If
                    If CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim) > MaxData Then
                        MaxData = CDate(rowCTR.Cells(CellIdxT.DataDoc).Text.Trim)
                    End If
                End If
                'alb070213 END

                'N°DOCUMENTO
                strNDoc = rowCTR.Cells(CellIdxT.NumDoc).Text
                If String.IsNullOrEmpty(strNDoc) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "NUMERO DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'DATA DOCUMENTO
                strDataDoc = rowCTR.Cells(CellIdxT.DataDoc).Text
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
                myID = rowCTR.Cells(CellIdxT.IDDoc).Text
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
                strCPag = rowCTR.Cells(CellIdxT.Cod_Pagamento).Text
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
                Valuta = rowCTR.Cells(CellIdxT.Cod_Valuta).Text
                If String.IsNullOrEmpty(Valuta) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE VALUTA SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'TOTALE DOCUMENTO
                strTotaleDoc = rowCTR.Cells(CellIdxT.TotaleDoc).Text
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
                ElseIf CDec(strTotaleDoc) = 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'TOTALE NETTO PAGARE
                strTotNettoPagare = rowCTR.Cells(CellIdxT.TotNettoPagare).Text
                If String.IsNullOrEmpty(strTotNettoPagare) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOT. NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf Not IsNumeric(strTotNettoPagare.Trim) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOT. NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf CDec(strTotNettoPagare) = 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOT. NETTO A PAGARE ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                myTotaleFCRiep += CDec(strTotNettoPagare) 'GIU250118
                'ABI
                strABI = rowCTR.Cells(CellIdxT.ABI).Text
                If String.IsNullOrEmpty(strABI) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE ABI. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'CAB
                strCAB = rowCTR.Cells(CellIdxT.CAB).Text
                If String.IsNullOrEmpty(strCAB) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CODICE CAB. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'giu160512 da controllare adesso commentati
                'PI'CF
                strPI = rowCTR.Cells(CellIdxT.PI).Text
                strCF = rowCTR.Cells(CellIdxT.CF).Text
                If String.IsNullOrEmpty(strPI) And String.IsNullOrEmpty(strCF) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "P.IVA - Cod.Fiscale MANCANTI. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'PROVINCIA
                strPr = rowCTR.Cells(CellIdxT.Pr).Text
                If String.IsNullOrEmpty(strPr) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "PROVINCIA NON DEFINITA. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'CAUSALE
                strCausale = rowCTR.Cells(CellIdxT.DesCausale).Text
                If String.IsNullOrEmpty(strCausale) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "CAUSALE NON DEFINITA. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            End If
        Next
        If CDec(LblTotDocumento.Text.Trim) <> myTotaleFCRiep Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "CONTROLLO TOTALE DOCUMENTO. LA SOMMA DEI DDT DIFFERISCE DAL TOTALE FATTURA", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
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
        'FINE CONTROLLO
        For Each row As GridViewRow In GridViewPrevT.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            If checkSel.Checked = True Then
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
                'ID DOCUMENTI
                myID = row.Cells(CellIdxT.IDDoc).Text
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
                    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                Dim IDDDTPRIMO As String = myID
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
                If CDec(LblTotDocumento.Text.Trim) = 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "TOTALE FATTURA NON PUO AVERE UN IMPORTO A ZERO.", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
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
                '' ''PI'CF
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
                'RICALCOLO SCADENZE
                Try
                    If Calcola_Scadenze(CLng(myID), strNDoc, CDate(txtDataFattura.Text), CInt(strCPag), CDec(LblTotDocumento.Text.Trim), Valuta, StrErrore) = False Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "RICALCOLO SCADENZE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "RICALCOLO SCADENZE. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
                'OK POSSO CREARE LA FATTURA
                NDoc = 0 : NRev = 0
                If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                    If chkFatturaPA.Checked = False Then
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
                
                Dim SqlDbNewCmd As New SqlCommand
                Dim RifTestata As String 'alb070213

                SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFCRiep]"
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
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_1", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_2", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_3", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_4", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Rata_5", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RifTestata", System.Data.SqlDbType.NVarChar, 150)) 'alb070213
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                '--
                SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
                SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
                SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
                SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
                SqlDbNewCmd.Parameters.Item("@DataFC").Value = CDate(txtDataFattura.Text)

                'alb070213
                If chkTipoFT.Checked Then
                    RifTestata = ddlTipoFattur.SelectedItem.Text
                    If strPeriodo <> "" Then
                        RifTestata = RifTestata & " periodo " & Left(strPeriodo, Len(strPeriodo) - 2)
                    Else
                        RifTestata = RifTestata & " dal " & Format(MinData, FormatoData) & " al " & Format(MaxData, FormatoData)
                    End If
                Else
                    RifTestata = "Fatturazione DDT dal " & Format(MinData, FormatoData) & " al " & Format(MaxData, FormatoData)
                End If
                SqlDbNewCmd.Parameters.Item("@RifTestata").Value = RifTestata
                'alb070213 END

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
                SqlDbNewCmd.Parameters.Item("@RetVal").Value = 0
                Try
                    myID = ""
                    SqlConn.Open()
                    SqlDbNewCmd.CommandTimeout = myTimeOUT
                    SqlDbNewCmd.ExecuteNonQuery()
                    SqlConn.Close()
                    myID = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
                    If SqlDbNewCmd.Parameters.Item("@RetVal").Value <> 0 Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                    & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                    & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
                                    & "", WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                Catch ExSQL As SqlException
                    Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
                                & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                Catch Ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
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
                        ModalPopup.Show("Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                    & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                    & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
                                    & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
                                & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End Try
                'OK IL PRIMO ADESSO I SUCCESSIVI ESCLUSO IL PRIMO
                Dim IDDDTNEXT As String = ""
                For Each rowNext As GridViewRow In GridViewPrevT.Rows
                    Dim checkSelNext As CheckBox = CType(rowNext.FindControl("checkSel"), CheckBox)
                    If checkSelNext.Checked = True Then
                        'N°DOCUMENTO
                        strNDoc = rowNext.Cells(CellIdxT.NumDoc).Text
                        If String.IsNullOrEmpty(strNDoc) Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", "NUMERO DOCUMENTO DDT ERRATO. N° DDT: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                        'DATA DOCUMENTO
                        strDataDoc = rowNext.Cells(CellIdxT.DataDoc).Text
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
                        'ID DOCUMENTI
                        IDDDTNEXT = rowNext.Cells(CellIdxT.IDDoc).Text
                        If IsNothing(IDDDTNEXT) Then
                            IDDDTNEXT = ""
                        End If
                        If String.IsNullOrEmpty(IDDDTNEXT) Then
                            IDDDTNEXT = ""
                        End If
                        If Not IsNumeric(IDDDTNEXT) Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                        & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
                                        & "Errore: IDENTIFICATIVO DDT SCONOSCIUTO: " & strNDoc, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                        If IDDDTPRIMO.Trim <> IDDDTNEXT.Trim Then
                            'OK INSERISCO GLI ALTRI DDT NELLA NUOVA FATTURA
                            SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
                            Dim SqlDbNewCmdNext As New SqlCommand

                            SqlDbNewCmdNext.CommandText = "[Insert_DocTCreateNewDocFCRiepNext]"
                            SqlDbNewCmdNext.CommandType = System.Data.CommandType.StoredProcedure
                            SqlDbNewCmdNext.Connection = SqlConn
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataFC", System.Data.SqlDbType.DateTime, 8))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
                            SqlDbNewCmdNext.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                            '--
                            SqlDbNewCmdNext.Parameters.Item("@IDDocIN").Value = CLng(IDDDTNEXT)
                            SqlDbNewCmdNext.Parameters.Item("@IDDocumenti").Value = CLng(myID)
                            SqlDbNewCmdNext.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
                            SqlDbNewCmdNext.Parameters.Item("@Numero").Value = NDoc
                            SqlDbNewCmdNext.Parameters.Item("@DataFC").Value = CDate(txtDataFattura.Text)
                            SqlDbNewCmdNext.Parameters.Item("@RevisioneNDoc").Value = NRev
                            SqlDbNewCmdNext.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                            SqlDbNewCmdNext.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                            SqlDbNewCmdNext.Parameters.Item("@RetVal").Value = 0
                            Try
                                SqlConn.Open()
                                SqlDbNewCmdNext.CommandTimeout = myTimeOUT
                                SqlDbNewCmdNext.ExecuteNonQuery()
                                SqlConn.Close()
                                If SqlDbNewCmdNext.Parameters.Item("@RetVal").Value <> 0 Then
                                    Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ModalPopup.Show("Errore Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                                & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                                & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
                                                & "", WUC_ModalPopup.TYPE_CONFIRM_Y)
                                    Exit Sub
                                End If
                            Catch ExSQL As SqlException
                                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Errore Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                            & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
                                            & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
                                Exit Sub
                            Catch Ex As Exception
                                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Errore Crea nuova Fattura riepilogativa", "Verificare i dati della fattura appena creata <br><strong><span> " _
                                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                                            & NDoc.ToString.Trim & "<br> Verificare anche i DDT che siano con lo stato Fatturato </span></strong><br>" _
                                            & "Errore: " + Ex.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
                                Exit Sub
                            End Try
                        End If
                    End If
                Next
                'FINITO ESCO DA CICLO FOR PRINCIPALE
                Exit For
            End If
        Next
        'OK FATTO
        Session(CSTFATTURAPA) = chkFatturaPA.Checked 'giu110814
        Session(CSTSPLITIVA) = chkSpltIVA.Checked 'giu160117
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(IDDOCUMENTI) = myID.Trim
        Response.Redirect("WF_Documenti.aspx?labelForm=FATTURA COMMERCIALE (Riepilogativa)")
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
        OKAzzera()
        BuidDett()
        DisableAll()
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
        'giu110814
        Dim StrErrore As String = "" : Dim SWSplitIVA As Boolean = False 'giu050118
        Dim SWFatturaPA As Boolean = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select("IDDocumenti=" & myID.Trim).Length > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("DDT incluso nella da Fattura riepilogativa", "Attenzione, impossibile la modifica.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select().Length > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "OKModificaDDT"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Modifica DDT selezionato", "Attenzione, sono stati inclusi dei DDT. <br> " _
                            & "La selezione dei documenti sarà perduta se procedete con la modifica. <br> " _
                            & "Siete sicuri di voler procedere ?", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        '-
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub
    Public Sub OKModificaDDT()
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub

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
                        ControllaDataDoc = Format(CDate("01/01/" & Session(ESERCIZIO).ToString.Trim), FormatoData) 'DATANULL
                    End If
                    Exit Function
                Else
                    ControllaDataDoc = Format(CDate("01/01/" & Session(ESERCIZIO).ToString.Trim), FormatoData) 'DATANULL
                    Exit Function
                End If
            Else
                ControllaDataDoc = Format(CDate("01/01/" & Session(ESERCIZIO).ToString.Trim), FormatoData) 'DATANULL
                Exit Function
            End If
        Catch Ex As Exception
            ControllaDataDoc = Format(CDate("01/01/" & Session(ESERCIZIO).ToString.Trim), FormatoData) 'DATANULL
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
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

    Private Function OKStampaDoc(ByVal IDDocFC As Long, ByVal NDoc As Long, ByRef strErrore As String) As Boolean
        OKStampaDoc = True
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        strErrore = ""
        Dim SWSconti As Boolean = False
        Dim ELDocToPrintSCY As New List(Of String)
        Dim ELDocToPrintSCN As New List(Of String)
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(IDDocFC.ToString.Trim, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, "Cli", DsPrinWebDoc, ObjReport, SWSconti, strErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                    'giu040512 giu070512
                    If Not IsNothing(Session(EL_DOC_TOPRINT_SCY)) Then
                        ELDocToPrintSCY = Session(EL_DOC_TOPRINT_SCY)
                    End If
                    ELDocToPrintSCY.Add(IDDocFC.ToString.Trim)
                    Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
                Else
                    Session(CSTSWScontiDoc) = 0
                    'giu040512 giu070512
                    If Not IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
                        ELDocToPrintSCN = Session(EL_DOC_TOPRINT_SCN)
                    End If
                    ELDocToPrintSCN.Add(IDDocFC.ToString.Trim)
                    Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
                End If
                Session(CSTSWConfermaDoc) = 0
                ' ''Response.Redirect("..\WebFormTables\WF_PrintWebCR.aspx?ESPORTA: " & Session(IDDOCUMENTI).ToString.Trim)
                'giu160512
                CreaFattureTutte()
                ' ''If ChkStampaSingFC.Checked = True Then
                ' ''    Session(ATTESA_CALLBACK_METHOD) = "CreaFattureTutte"
                ' ''    Session(CSTNOBACK) = 1
                ' ''    Attesa.ShowStampa("Stampa Fattura N° : " & FormattaNumero(NDoc.ToString), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
                ' ''Else
                ' ''    CreaFattureTutte()
                ' ''End If
            Else
                'L'ERRORE è INIZIALIZZATO ALLA FUNZIONE STAMPADOCUMENTO
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                OKStampaDoc = False
                'giu040512
                ELDocToPrintSCN = Nothing
                Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
                ELDocToPrintSCY = Nothing
                Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
            End If
        Catch ex As Exception
            OKStampaDoc = False
            'giu040512
            ELDocToPrintSCN = Nothing
            Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
            ELDocToPrintSCY = Nothing
            Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
            ' ''Chiudi("Errore:" & ex.Message)
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "Errore:" & ex.Message
        End Try

    End Function
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
                Session(CSTSWRitAcc) = IIf(SWRitAcc = True, 1, 0)
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
        ddlRicerca.Enabled = False
        btnRicerca.Text = "Seleziona Cliente"
        txtDataA.Enabled = False : imgBtnShowCalendarA.Enabled = False : chkTipoFT.Enabled = False : ddlTipoFattur.Enabled = False
        txtDalNDDT.Enabled = False : txtAlNDDT.Enabled = False : txtPrimoNFattura.Enabled = False
    End Sub

    Public Sub StatoModSel(ByVal Valore As Boolean)
        If GridViewPrevT.Rows.Count > 0 Then
            btnModifica.Enabled = Valore
            btnSelTutte.Enabled = Valore
            btnSelRiga.Enabled = Valore
        End If
    End Sub

    Public Sub EnableAll()
        ddlRicerca.Enabled = True
        btnRicerca.Text = "Visualizza DDT"
        txtDataA.Enabled = True : imgBtnShowCalendarA.Enabled = True
        chkTipoFT.Enabled = True
        If chkTipoFT.Checked = True Then ddlTipoFattur.Enabled = True
        txtDalNDDT.Enabled = True : txtAlNDDT.Enabled = True : txtPrimoNFattura.Enabled = False
    End Sub

    Private Sub btnSelRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSelRiga.Click

        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myNDoc As String = "" : Dim myDataDoc As String = "" : Dim myRiferimento As String = "" 'alb060213 aggiunto myRiferimento per inserire riga nel dettaglio
        Try
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            myNDoc = row.Cells(CellIdxT.NumDoc).Text.Trim
            myDataDoc = row.Cells(CellIdxT.DataDoc).Text.Trim
            myRiferimento = row.Cells(CellIdxT.Riferimento).Text.Trim 'alb060213
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura dati DDT da includere in fattura riepilogativa(1).: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        If Not IsDate(myDataDoc) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura dati DDT da includere in fattura riepilogativa.: Data documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If Not IsNumeric(CDec(myNDoc)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura dati DDT da includere in fattura riepilogativa.: N° documento", WUC_ModalPopup.TYPE_ERROR)
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
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select("IDDocumenti=" & myID.Trim).Length > 0 Then
            btnSelRiga.Enabled = False : btnDeSelRiga.Enabled = True
            Exit Sub
        End If
        'OK inserisco a RIGA 0 IL RIFERIMENTO DDT E IL RIFERIMENTO DEL DDT SELEZIONATO----------------------------
        'alb060213 aggiunto myRiferimento
        If OKSelDDT(myID, myNDoc, myDataDoc, myRiferimento) = False Then
            Exit Sub
        End If
        Dim myTotNettoPagare As String = ""
        Try
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = True
            myTotNettoPagare = row.Cells(CellIdxT.TotNettoPagare).Text.Trim
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura dati DDT da includere in fattura riepilogativa(2).: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        If myTotNettoPagare = "" Then myTotNettoPagare = "0"
        Try
            LblTotDocumento.Text = CDec(LblTotDocumento.Text.Trim) + CDec(myTotNettoPagare)
            LblTotDocumento.Text = Formatta.FormattaNumero(CDec(LblTotDocumento.Text.Trim), 2)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura totale DDT da sommare alla fattura riepilogativa.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        btnSelRiga.Enabled = False
        btnDeSelRiga.Enabled = True : btnDeSelTutte.Enabled = True
        '---------------------------------------------------------------------
        Session("aDataViewFCR") = aDataViewDettFCR
        Session("aSqlAdapFCR") = SqlAdapDocDettFCR
        Session("aDsDettFCR") = DsDocumentiDettFCR
        aDataViewDettFCR = New DataView(DsDocumentiDettFCR.DocumentiDForFCRiep)
        If aDataViewDettFCR.Count > 0 Then aDataViewDettFCR.Sort = "Numero,Riga"
        GridViewPrevDNEWFatt.DataSource = aDataViewDettFCR
        Session("aDataViewFCR") = aDataViewDettFCR
        GridViewPrevDNEWFatt.DataBind()
        If GridViewPrevDNEWFatt.Rows.Count = 0 Then
            SetBtnFCR(False)
            btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
        Else
            SetBtnFCR(True)
        End If
    End Sub
    Private Sub btnSelTutte_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSelTutte.Click
        btnDeSelRiga.Enabled = True : btnDeSelTutte.Enabled = True
        btnSelRiga.Enabled = False : btnSelTutte.Enabled = False
        Dim myID As String = "" : Dim myNDoc As String = "" : Dim myDataDoc As String = "" : Dim myRiferimento As String = "" 'alb060213 aggiunto myRiferimento per inserire riga nel dettaglio 
        Dim myTotNettoPagare As String = ""
        DsDocumentiDettFCR = Session("aDsDettFCR")
        
        For Each row As GridViewRow In GridViewPrevT.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = True
            '----
            myID = row.Cells(CellIdxT.IDDoc).Text.Trim
            myNDoc = row.Cells(CellIdxT.NumDoc).Text.Trim
            myDataDoc = row.Cells(CellIdxT.DataDoc).Text.Trim
            myTotNettoPagare = row.Cells(CellIdxT.TotNettoPagare).Text.Trim
            myRiferimento = row.Cells(CellIdxT.Riferimento).Text.Trim 'alb060213
            If DsDocumentiDettFCR.DocumentiDForFCRiep.Select("IDDocumenti=" & myID.Trim).Length > 0 Then
                'gia' presente
            Else
                'OK inserisco a RIGA 0 IL RIFERIMENTO DDT E IL RIFERIMENTO DEL DDT SELEZIONATO----------------------------
                'alb060213 aggiunto myRiferimento
                If OKSelDDT(myID, myNDoc, myDataDoc, myRiferimento) = False Then
                    Exit For
                End If
                '---------------------------------------------------------------------
                If myTotNettoPagare = "" Then myTotNettoPagare = "0"
                Try
                    LblTotDocumento.Text = CDec(LblTotDocumento.Text.Trim) + CDec(myTotNettoPagare)
                    LblTotDocumento.Text = Formatta.FormattaNumero(CDec(LblTotDocumento.Text.Trim), 2)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Lettura totale DDT da sommare alla fattura riepilogativa.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit For
                End Try
            End If
        Next
        Session("aDataViewFCR") = aDataViewDettFCR
        Session("aSqlAdapFCR") = SqlAdapDocDettFCR
        Session("aDsDettFCR") = DsDocumentiDettFCR
        aDataViewDettFCR = New DataView(DsDocumentiDettFCR.DocumentiDForFCRiep)
        If aDataViewDettFCR.Count > 0 Then aDataViewDettFCR.Sort = "Numero,Riga"
        GridViewPrevDNEWFatt.DataSource = aDataViewDettFCR
        Session("aDataViewFCR") = aDataViewDettFCR
        GridViewPrevDNEWFatt.DataBind()
        If GridViewPrevDNEWFatt.Rows.Count = 0 Then
            SetBtnFCR(False)
            btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
        Else
            SetBtnFCR(True)
        End If
    End Sub

    Private Function OKSelDDT(ByVal _IDDocumenti As String, ByVal _NDoc As String, ByVal _DataDoc As String, ByVal _Riferimento As String) As Boolean
        'OK inserisco a RIGA 0 IL RIFERIMENTO DDT E IL RIFERIMENTO DEL DDT SELEZIONATO----------------------------
        'alb060213 aggiunto myRiferimento
        OKSelDDT = True
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select("IDDocumenti=" & _IDDocumenti.Trim).Length > 0 Then
            'gia' presente
            Exit Function
        End If
        If Not IsDate(_DataDoc) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura dati DDT da includere in fattura riepilogativa.: Data documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        If Not IsNumeric(CDec(_NDoc)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura dati DDT da includere in fattura riepilogativa.: N° documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        '---
        Dim newRowDocD As DSDocumenti.DocumentiDForFCRiepRow = DsDocumentiDettFCR.DocumentiDForFCRiep.NewDocumentiDForFCRiepRow
        With newRowDocD
            .BeginEdit()
            .IDDocumenti = CLng(_IDDocumenti)
            .Numero = CLng(_NDoc)
            .Riga = 0
            .Descrizione = "Rif. Ns DDT nr. " & _NDoc.Trim & " del " & _DataDoc.Trim & IIf(_Riferimento.Trim <> "" And _Riferimento.Trim <> "&nbsp;", " - Riferimento: " & _Riferimento, "")
            .Prezzo = 0
            .Prezzo_Netto = 0
            .Qta_Allestita = 0
            .Qta_Evasa = 0
            .Qta_Impegnata = 0
            .Qta_Ordinata = 0
            .Qta_Prenotata = 0
            .Qta_Residua = 0
            .Importo = 0
            .PrezzoAcquisto = 0
            .PrezzoListino = 0
            .SWModAgenti = False
            .PrezzoCosto = 0
            .Qta_Inviata = 0
            .Qta_Fatturata = 0
            '---------
            .OmaggioImponibile = False
            .OmaggioImposta = False
            .EndEdit()
        End With
        Try
            DsDocumentiDettFCR.DocumentiDForFCRiep.AddDocumentiDForFCRiepRow(newRowDocD)
            newRowDocD = Nothing
        Catch Ex As Exception
            OKSelDDT = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Includi DDT in Fattura riepilogativa", "Errore inserimento Riga(0): " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        '---------------------------------------------------------------------
        SetCdmDAdp()
        SqlDbSelectCmdDettFCR.Parameters.Item("@IDDocumenti").Value = CLng(_IDDocumenti)
        SqlAdapDocDettFCR.Fill(DsDocumentiDettFCR.DocumentiDForFCRiep)

        'imposto numero doc
        For Each rowDett As DSDocumenti.DocumentiDForFCRiepRow In DsDocumentiDettFCR.DocumentiDForFCRiep.Select("IDDocumenti=" & _IDDocumenti.Trim, "Riga")
            With rowDett
                .BeginEdit()
                .Numero = CLng(_NDoc)
                '----------
                .EndEdit()
            End With
        Next
        DsDocumentiDettFCR.AcceptChanges()
        '-
        Session("aDsDettFCR") = DsDocumentiDettFCR
    End Function

    Private Sub btnDeSelRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeSelRiga.Click

        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myTotaleDDT As String = ""
        Try
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = False
            myTotaleDDT = row.Cells(CellIdxT.TotaleDoc).Text.Trim
            Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoClienti)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura dati DDT da escludere(1).: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        If myTotaleDDT = "" Then myTotaleDDT = "0"
        Try
            If CDec(LblTotDocumento.Text.Trim) > 0 Then
                LblTotDocumento.Text = CDec(LblTotDocumento.Text.Trim) - CDec(myTotaleDDT)
                LblTotDocumento.Text = Formatta.FormattaNumero(CDec(LblTotDocumento.Text.Trim), 2)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura totale DDT da sottrarre alla fattura riepilogativa.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
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
        btnSelRiga.Enabled = True : btnDeSelRiga.Enabled = False
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select("IDDocumenti=" & myID.Trim).Length = 0 Then
            Exit Sub
        End If
        'OK Escludo
        For Each rowDett As DSDocumenti.DocumentiDForFCRiepRow In DsDocumentiDettFCR.DocumentiDForFCRiep.Select("IDDocumenti=" & myID.Trim)
            With rowDett
                .Delete()
            End With
        Next
        DsDocumentiDettFCR.AcceptChanges()
        '-
        Session("aDataViewFCR") = aDataViewDettFCR
        Session("aSqlAdapFCR") = SqlAdapDocDettFCR
        Session("aDsDettFCR") = DsDocumentiDettFCR
        aDataViewDettFCR = New DataView(DsDocumentiDettFCR.DocumentiDForFCRiep)
        If aDataViewDettFCR.Count > 0 Then aDataViewDettFCR.Sort = "Numero,Riga"
        GridViewPrevDNEWFatt.DataSource = aDataViewDettFCR
        Session("aDataViewFCR") = aDataViewDettFCR
        GridViewPrevDNEWFatt.DataBind()
        If GridViewPrevDNEWFatt.Rows.Count = 0 Then
            SetBtnFCR(False)
            btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
        Else
            SetBtnFCR(True)
        End If
        btnSelRiga.Enabled = True : btnDeSelRiga.Enabled = False
    End Sub
    Private Sub btnDeSelTutte_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeSelTutte.Click

        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select().Length > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "DeSelTutte"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Deseleziona tutti i DDT inclusi", "Attenzione, sono stati inclusi dei DDT. <br> " _
                            & "Siete sicuri di voler procedere ?", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        DeSelTutte()
    End Sub
    Public Sub DeSelTutte()
        btnDeSelRiga.Enabled = False : btnDeSelTutte.Enabled = False
        btnSelRiga.Enabled = True : btnSelTutte.Enabled = True
        For Each row As GridViewRow In GridViewPrevT.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = False
        Next
        OKAzzera()
    End Sub
   
    Private Sub chkFatturaPA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFatturaPA.CheckedChanged
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select().Length > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "SWDDTPA"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Seleziona Cliente", "Attenzione, sono stati inclusi dei DDT. <br> " _
                            & "Siete sicuri di voler procedere ?", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        SWDDTPA()
    End Sub
    Public Sub SWDDTPA()
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
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("N° Fattura fuori sequenza", "<strong><span> FATTURA COMMERCIALE (Riepilogativa)" _
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
        OKRicerca()
    End Sub

    Private Sub chkSpltIVA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSpltIVA.CheckedChanged
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select().Length > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "SWDDTPA"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Seleziona Cliente", "Attenzione, sono stati inclusi dei DDT. <br> " _
                            & "Siete sicuri di voler procedere ?", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        SWDDTPA()
    End Sub

    Private Sub chkRitAcconto_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRitAcconto.CheckedChanged
        DsDocumentiDettFCR = Session("aDsDettFCR")
        If DsDocumentiDettFCR.DocumentiDForFCRiep.Select().Length > 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "SWDDTPA"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Seleziona Cliente", "Attenzione, sono stati inclusi dei DDT. <br> " _
                            & "Siete sicuri di voler procedere ?", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        SWDDTPA()
    End Sub
End Class