Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports SoftAziOnLine.Magazzino
'giu110320
Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO 'giu140615

Partial Public Class WUC_ValMagCostoVenduto
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblMess.Visible = False
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceCategoria.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceLinea.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'giu130112
        ' ''txtCodMagazzino.Enabled = False
        ' ''ddlMagazzino.Enabled = False
        LnkStampa.Visible = False 'giu110320
        If (Not IsPostBack) Then
            rbtnCodice.Checked = True
            txtCodMagazzino.Text = 0
            ddlMagazzino.SelectedValue = 1
            chkArtGiacNegativa.Checked = False
            If Now > CDate("31/12/" & Session(ESERCIZIO)) Then
                txtData.Text = "31/12/" & Session(ESERCIZIO)
            Else
                txtData.Text = Format(Now, FormatoData)
            End If
            btnStampa.Text = "Stampa analitica"
            btnStampaSint.Text = "Stampa sintetica"
        End If
        ModalPopup.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub

    Public Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        LnkStampa.Visible = False 'giu110320
        Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFO
        Call OKStampa()
    End Sub
    Public Sub btnStampaSint_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaSint.Click
        LnkStampa.Visible = False 'giu110320
        Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFOSint
        Call OKStampa()
    End Sub

    'giu101013
    Private Sub OKStampa()
        'giu210912
        ' ''Dim SWDebug As Boolean = False
        '-----
        Session(CSTNOMEPDF) = "" 'giu300315
        ' ''If App.GetDatiAbilitazioni(CSTABILAZI, "SWDebug", "", "") = True Then
        ' ''    SWDebug = True
        ' ''End If
        '----------------------------
        '@@@@@@@@@
        Dim DsMagazzino1 As New DsMagazzino
        Dim ObjReport As New Object
        Dim ClsPrint As New Magazzino
        Dim Errore As Boolean = False
        Dim Filtri As String = ""
        Dim Selezione As String = ""
        Dim StrErrore As String = ""
        Dim RagrForn As Boolean = False
        If ddlMagazzino.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlCatgoria.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlLinea.BackColor = SEGNALA_KO Then
            Errore = True
        End If

        If ddlMagazzino.Text = "" Then
            Errore = True
        End If
        If txtCod1.Text.Trim <> "" And txtCod2.Text.Trim <> "" Then
            If txtCod2.Text.Trim < txtCod1.Text.Trim Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati selezione", "Attenzione, Codici articoli incongruenti. <br> (Al codice < di Dal codice).", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If Errore Then
            ModalPopup.Show("Controllo dati stampa", "Attenzione, i campi segnalati in rosso non sono validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Dim strErroreGiac As String = ""
        Dim SWNegativi As Boolean = False
        If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then 'giu190613 aggiunto SWNegativi
            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf strErroreGiac.Trim <> "" Then
            'If Session("SWConferma") = SWNO Then 'giu300423
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricalcolo giacenze", strErroreGiac.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        'FILTRO PER MAGAZZINO
        'GIU210920 VIENE SELEZIONATO NELLA CHIAMATA A funDispMagazzino Selezione = "WHERE Codice_Magazzino = " & ddlMagazzino.SelectedValue
        Selezione = ""
        '-------------
        Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper

        'FILTRO PER CATEGORIA
        If txtCodCategoria.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "CodCategoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
            Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
        End If

        'FILTRO PER LINEA
        If txtCodLinea.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "CodLinea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
            Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
        End If

        'FILTRO DA ARTICOLO 
        If txtCod1.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
            Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
        End If

        'FILTRO A ARTICOLO
        If txtCod2.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
            Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
        End If
        Selezione = Selezione & " ORDER BY Cod_Articolo"
        '=======================================================
        '
        Try
            ' ''Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFO
            If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Costo del venduto (FIFO) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, False, False, SWNegativi, False, True, ddlMagazzino.SelectedValue) Then
                If DsMagazzino1.DispMagazzino.Count > 0 Then
                    'giu011020 GIU210920 OK ADESSO ANCHE PER ALTRI MAGAZZINI
                    If Val(ddlMagazzino.SelectedValue.Trim) <> 0 Then
                        Selezione = "WHERE CodiceMagazzino = " & ddlMagazzino.SelectedValue & " AND "
                    Else
                        Selezione = "WHERE "
                    End If
                    Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper
                    'FILTRO VALORIZZAZIONE ALLA DATA
                    If chkDebug.Checked = False Then
                        Selezione += " Data_Doc <= CONVERT(DATETIME, '" & Format(CDate(txtData.Text), FormatoData) & "', 103)"
                    Else
                        'giu190912 TEST per verificare un mese e/o periodo (discordanza dati tra CV e StatVend)
                        Selezione += " Data_Doc >= CONVERT(DATETIME, '01/" & Month(CDate(txtData.Text)) & "/" & Year(CDate(txtData.Text)) & "', 103)"
                        Selezione += " Data_Doc <= CONVERT(DATETIME, '" & Format(CDate(txtData.Text), FormatoData) & "', 103)"
                    End If
                    '--------------------------------------------------------------------------------------
                    'è NELL'INTESTAZIONE Filtri += " | Alla data - " & Format(CDate(txtData.Text), FormatoData)
                    'FILTRO PER CATEGORIA
                    If txtCodCategoria.Text <> "" Then
                        Selezione = Selezione & " AND Categoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
                        Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
                    End If
                    'FILTRO PER LINEA
                    If txtCodLinea.Text <> "" Then
                        Selezione = Selezione & " AND Linea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
                        Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
                    End If
                    'FILTRO DA ARTICOLO 
                    If txtCod1.Text <> "" Then
                        Selezione = Selezione & " AND view_MovMagValorizzazioneCS.Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
                        Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
                    End If
                    'FILTRO A ARTICOLO
                    If txtCod2.Text <> "" Then
                        Selezione = Selezione & " AND view_MovMagValorizzazioneCS.Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
                        Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
                    End If
                    Selezione += " ORDER BY view_MovMagValorizzazioneCS.Cod_Articolo, Data_Doc"
                    If ClsPrint.ValMagFIFO_CostoVendutoFIFO(Session(CSTAZIENDARPT), "Costo del venduto (FIFO) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, chkArtGiacNegativa.Checked, False, True, SWNegativi) Then
                        If DsMagazzino1.DispMagazzino.Count > 0 Then
                            ' ''Session(CSTDsPrinWebDoc) = DsMagazzino1
                            If SWNegativi = True Then
                                If chkArtGiacNegativa.Checked = False Then
                                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "OKStampaFIFO"
                                    ' ''ModalPopup.Show("Attenzione", "Sono presenti movimenti in negativo. <br> Confermi l'elaborazione della stampa ?", WUC_ModalPopup.TYPE_CONFIRM)
                                    ' ''Exit Sub
                                    lblMess.Text = "Attenzione, sono presenti movimenti in negativo."
                                    lblMess.Visible = True
                                End If
                            End If
                            ' ''Session(CSTNOBACK) = 0 'giu040512
                            ' ''Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
                            Call OKStampaFIFO(DsMagazzino1)
                        Else
                            ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                        End If
                    Else
                        ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                Else
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Public Sub OKStampaFIFO(ByRef DsMagazzino1 As DsMagazzino)
        'giu110320
        ' ''Session(CSTNOBACK) = 0 'giu040512
        ' ''Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim Rpt As ReportClass
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        Dim CodiceDitta As String = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            CodiceDitta = ""
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            CodiceDitta = ""
        End If
        '---------------------
        Session(CSTNOMEPDF) = ""
        If Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzino Then
            Session(CSTNOMEPDF) = "DisponibilitaMagazzino.pdf"
            Rpt = New DispMag
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzinoFornitori Then
            Session(CSTNOMEPDF) = "DisponibilitaMagazzinoFornitori.pdf"
            Rpt = New DispMagForn
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOAnalitico Then
            Session(CSTNOMEPDF) = "ValMagFIFOAnalitico.pdf"
            Rpt = New ValMagFIFO
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSintetico Then
            Session(CSTNOMEPDF) = "ValMagFIFOSintetico.pdf"
            Rpt = New ValMagFIFOS
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoFor Then
            Session(CSTNOMEPDF) = "ValMagFIFOSinteticoFor.pdf"
            Rpt = New ValMagFIFOSFor
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoForS Then
            Session(CSTNOMEPDF) = "ValMagFIFOSinteticoForSint.pdf"
            Rpt = New ValMagFIFOSForSint
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqAnalitico Then
            Session(CSTNOMEPDF) = "ValMagUltPrzAcqAnalitico.pdf"
            Rpt = New ValMagUltPrzAcq
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqSintetico Then
            Session(CSTNOMEPDF) = "ValMagUltPrzAcqSintetico.pdf"
            Rpt = New ValMagUltPrzAcqS
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondAnalitico Then
            Session(CSTNOMEPDF) = "ValMagMediaPondAnalitico.pdf"
            Rpt = New ValMagMediaPond
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondSintetico Then
            Session(CSTNOMEPDF) = "ValMagMediaPondSintetico.pdf"
            Rpt = New ValMagMediaPondS
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFO Then
            Session(CSTNOMEPDF) = "ValMagCostoVendFIFO.pdf"
            Rpt = New ValMagCostoVendutoFIFO
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFOSint Then
            Session(CSTNOMEPDF) = "ValMagCostoVendFIFOSint.pdf"
            Rpt = New ValMagCostoVendutoFIFOSint
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOAnalitico Then
            Session(CSTNOMEPDF) = "ValMagLIFOAnalitico.pdf"
            Rpt = New ValMagLIFO
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOSintetico Then
            Session(CSTNOMEPDF) = "ValMagLIFOSintetico.pdf"
            Rpt = New ValMagLIFOS
        Else
            Chiudi("Errore: TIPO STAMPA DI MAGAZZINO SCONOSCIUTA")
            Exit Sub
        End If
        '-----------------------------------
        ' ''Dim DsMovMag1 As New DsMagazzino
        ' ''DsMovMag1 = Session(CSTDsPrinWebDoc)
        Rpt.SetDataSource(DsMagazzino1)
        'Per evitare che solo un utente possa elaborare le stampe
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Sub
        End If
        Session(CSTNOMEPDF) = Format(Now, "yyyyMMddHHmmss") + Utente.Codice.Trim & Session(CSTNOMEPDF)
        '---------
        'giu110320 giu180320 TROPPO LENTO
        ' ''LnkStampa.HRef = "~/WebFormTables/Stampa.aspx"
        ' ''Dim myStream As Stream
        ' ''Dim ms As New MemoryStream
        ' ''Dim myOBJ() As Byte = Nothing
        ' ''Try
        ' ''    myStream = Rpt.ExportToStream(ExportFormatType.PortableDocFormat)

        ' ''    Dim Ret As Integer
        ' ''    Do
        ' ''        Ret = myStream.ReadByte() 'netstream.Read(Bytes, 0, Bytes.Length)
        ' ''        If Ret > 0 Then
        ' ''            ReDim Preserve myOBJ(myStream.Position - 1)
        ' ''            myOBJ(myStream.Position - 1) = Ret
        ' ''        End If
        ' ''    Loop Until Ret = -1

        ' ''Catch ex As Exception
        ' ''    Chiudi("Errore in elaborazione stampa: " & ex.Message)
        ' ''End Try
        ' ''Session("objReport") = myOBJ
        ' ''LnkStampa.Visible = True
        '---------
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & "StatMag\"
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            'giu140124
            Rpt.Close()
            Rpt.Dispose()
            Rpt = Nothing
            '-
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '-------------
        Catch ex As Exception
            Rpt = Nothing
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Stampa valorizzazione magazzino", "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
            Exit Sub
        End Try
        LnkStampa.Visible = True
        Dim LnkName As String = "~/Documenti/StatMag/" & Session(CSTNOMEPDF)
        LnkStampa.HRef = LnkName
    End Sub
    Private Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        Else
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        End If
    End Sub
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub txtCodCategoria_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCategoria.TextChanged
        PosizionaItemDDLTxt(txtCodCategoria, ddlCatgoria)
        txtCodLinea.Focus()
    End Sub

    Private Sub txtCodLinea_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodLinea.TextChanged
        PosizionaItemDDLTxt(txtCodLinea, ddlLinea)
        txtCod1.Focus()
    End Sub

    Private Sub ddlLinea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlLinea.SelectedIndexChanged
        txtCodLinea.Text = ddlLinea.SelectedValue
        txtCodLinea.BackColor = SEGNALA_OK
        ddlLinea.BackColor = SEGNALA_OK
    End Sub

    Private Sub txtCodMagazzino_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodMagazzino.TextChanged
        PosizionaItemDDLByTxt(txtCodMagazzino, ddlMagazzino, True)
        txtCodCategoria.Focus()
    End Sub

    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        txtCodMagazzino.Text = ddlMagazzino.SelectedValue
        txtCodMagazzino.BackColor = SEGNALA_OK
        ddlMagazzino.BackColor = SEGNALA_OK
    End Sub

    Private Sub ddlCatgoria_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCatgoria.SelectedIndexChanged
        txtCodCategoria.Text = ddlCatgoria.SelectedValue
        txtCodCategoria.BackColor = SEGNALA_OK
        ddlCatgoria.BackColor = SEGNALA_OK
    End Sub

#Region "Ricerca e scelta articoli"
    Private Sub BtnCod1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod1.Click
        Session(SWCOD1COD2) = 1
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub
    Private Sub BtnCod2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod2.Click
        Session(SWCOD1COD2) = 2
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub
    Public Sub CallBackWFPArticoloSelSing()
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        If String.IsNullOrEmpty(Session(SWCOD1COD2)) Then
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
        End If
        'TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        'TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub
#End Region

End Class