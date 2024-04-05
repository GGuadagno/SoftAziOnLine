Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
'Imports Microsoft.Reporting.WebForms
Imports System.IO 'giu150615
Partial Public Class WUC_ElencoPreventivi
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private CodiceDitta As String = "" 'giu180120
    Private InizialiUT As String = "" 'GIU210120

    Private Enum CellIdxT
        Stato = 1
        NumDoc = 2
        RevN = 3
        DataDoc = 4
        DataCons = 5
        CodCliForProvv = 6
        RagSoc = 7
        DataVal = 13
        Riferimento = 14
        DataRif = 15
    End Enum
    Private Enum CellIdxD
        CodArt = 0
        DesArt = 1
        UM = 2
        Qta = 3
        IVA = 4
        Prz = 5
        ScV = 6
        Sc1 = 7
        Importo = 8
        ScR = 9
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_ElencoPreventivi.aspx?labelForm=Gestione preventivi/offerte"
        LnkStampa.Visible = False
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
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        'giu15112011 da ATTIVARE
        'giu150320 btnCambiaStato.Visible = False
        '-----------------------
        If (Not IsPostBack) Then
            Try
                ddlRicerca.Items.Add("Numero preventivo/offerta")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("Data preventivo/offerta")
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
                ddlRicerca.Items.Add("Codice CIG/CUP")
                ddlRicerca.Items(12).Value = "CC"
                ddlRicerca.Items.Add("Destinazione1")
                ddlRicerca.Items(13).Value = "D1"
                ddlRicerca.Items.Add("Destinazione2")
                ddlRicerca.Items(14).Value = "D2"
                ddlRicerca.Items.Add("Destinazione3")
                ddlRicerca.Items(15).Value = "D3"
                'giu110319
                Dim SWRbtnTD As String = Session(CSTSWRbtnTD)
                If IsNothing(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                If String.IsNullOrEmpty(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                '-
                If SWRbtnTD.Trim = "" Or SWRbtnTD.Trim <> SWTD(TD.Preventivi) Then
                    Session(CSTSTATODOCSEL) = "0"
                    Session(CSTSWRbtnTD) = SWTD(TD.Preventivi)
                End If
                '-----------
                Session(CSTTIPODOC) = SWTD(TD.Preventivi)
                Session(CSTSORTPREVTEL) = "*" 'giu201021 PRIMA VOLTA NEL CASO SI VOGLIA SELEZIONARE CON TOP COSI E' PIU VELOCE L'APERTURA
                Session(SWOP) = SWOPNESSUNA
                'GIU080319
                Dim strStatoDoc As String = Session(CSTSTATODOCSEL)
                If IsNothing(strStatoDoc) Then
                    strStatoDoc = ""
                End If
                If String.IsNullOrEmpty(strStatoDoc) Then
                    strStatoDoc = ""
                End If
                '-----------
                If strStatoDoc = "" Then
                    Session(CSTSTATODOC) = "0"
                    rbtnDaConfermare.Checked = True
                ElseIf strStatoDoc = "1" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnConfermati.Checked = True
                ElseIf strStatoDoc = "3" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnChiusoNoConf.Checked = True
                ElseIf strStatoDoc = "4" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnNonConferm.Checked = True
                ElseIf strStatoDoc = "999" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnTutti.Checked = True
                Else
                    Session(CSTSTATODOC) = "0"
                    rbtnDaConfermare.Checked = True
                End If
                'giu201021 troppo lento esegue piu vole le letture cosi va bene 
                '' ''giu090319 lo esegue il Checked=true  BuidDett()
                ' ''Try
                ' ''    If GridViewPrevT.Rows.Count > 0 Then
                ' ''        Dim savePI = Session("PageIndex")
                ' ''        Dim saveSI = Session("SelIndex")
                ' ''        GridViewPrevT.Sort(Session("SortExp"), Session("SortDir"))
                ' ''        GridViewPrevT.PageIndex = savePI 'Session("PageIndex")
                ' ''        GridViewPrevT.SelectedIndex = saveSI 'Session("SelIndex")
                ' ''        ImpostaFiltro()
                ' ''        GridViewPrevT.DataBind()
                ' ''        GridViewPrevD.DataBind()
                ' ''    End If

                ' ''Catch ex As Exception

                ' ''End Try
                '-----------
                If GridViewPrevT.Rows.Count > 0 Then
                    BtnSetEnabledTo(True)
                    Try
                        'giu021123 
                        ' ''GridViewPrevT.SelectedIndex = 0 
                        Dim savePI = Session("PageIndex")
                        Dim saveSI = Session("SelIndex")
                        If String.IsNullOrEmpty(Session("SortExp")) Then
                            Session("SortExp") = "Numero"
                        End If
                        If String.IsNullOrEmpty(Session("SortDir")) Then
                            Session("SortDir") = 1
                        End If
                        GridViewPrevT.Sort(Session("SortExp"), Session("SortDir"))
                        GridViewPrevT.PageIndex = savePI
                        GridViewPrevT.SelectedIndex = saveSI
                        '---------
                        Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                        Dim row As GridViewRow = GridViewPrevT.SelectedRow
                        Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                        btnCreaOrdine.Enabled = True
                        BtnSetByStato(Stato) 'giu010612
                        GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
                        GridViewPrevD.DataBind() 'giu201021
                    Catch ex As Exception
                        Session(IDDOCUMENTI) = ""
                    End Try
                Else
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    Session(IDDOCUMENTI) = ""
                End If
                '-----------------------------------------------
                ' ''giu090319 viene eseguito dal rbtn....check=true BuidDett()
            Catch ex As Exception
                Chiudi("Errore: Caricamento Elenco preventivi: " & ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
        'Simone 290317
        WFPDocCollegati.WucElement = Me
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
        '-
        WFPCambiaStatoPR.WucElement = Me
        If Session(F_CAMBIOSTATO_APERTA) Then
            WFPCambiaStatoPR.Show()
        End If
    End Sub
    'giu010612
    Private Sub BtnSetByStato(ByVal myStato As String)
        If myStato.Trim = "Confermato" Then
            btnModifica.Enabled = False
            btnCreaOrdine.Enabled = False
            btnNuovaRev.Enabled = False
        End If
        If myStato.Trim = "Chiuso non confermato" Then
            btnModifica.Enabled = False
            btnCreaOrdine.Enabled = False
            btnNuovaRev.Enabled = False
        End If
        If myStato.Trim = "Non confermabile" Then
            btnModifica.Enabled = False
            btnCreaOrdine.Enabled = False
            btnNuovaRev.Enabled = False
        End If
    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnCambiaStato.Enabled = Valore
        btnVisualizza.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        btnCreaOrdine.Enabled = Valore
        btnStampa.Enabled = Valore
        btnCopia.Enabled = Valore
        btnNuovaRev.Enabled = Valore
        btnDocCollegati.Enabled = Valore 'Simone290317
    End Sub

#Region " Ordinamento e ricerca"
    Private Sub rbtnDaConfermare_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDaConfermare.CheckedChanged
        LnkStampa.Visible = False
        btnCreaOrdine.Enabled = True
        Session(CSTSTATODOC) = "0"
        Session(CSTSTATODOCSEL) = "0"
        BuidDett()
    End Sub
    Private Sub rbtnConfermati_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnConfermati.CheckedChanged
        LnkStampa.Visible = False
        btnCreaOrdine.Enabled = False
        Session(CSTSTATODOC) = "1"
        Session(CSTSTATODOCSEL) = "1"
        BuidDett()
    End Sub
    Private Sub rbtnChiusoNoConf_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnChiusoNoConf.CheckedChanged
        LnkStampa.Visible = False
        btnCreaOrdine.Enabled = False
        Session(CSTSTATODOC) = "3"
        Session(CSTSTATODOCSEL) = "3"
        BuidDett()
    End Sub
    Private Sub rbtnNonConferm_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnNonConferm.CheckedChanged
        LnkStampa.Visible = False
        btnCreaOrdine.Enabled = False
        Session(CSTSTATODOC) = "4"
        Session(CSTSTATODOCSEL) = "4"
        BuidDett()
    End Sub
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTutti.CheckedChanged
        LnkStampa.Visible = False
        btnCreaOrdine.Enabled = True
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        BuidDett()
    End Sub
    Private Sub BuidDett()
        'GIU22062017
        ImpostaFiltro()
        SqlDSPrevTElenco.DataBind()
        '---------
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind() 'giu080212
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                btnCreaOrdine.Enabled = True
                BtnSetByStato(Stato) 'giu010612
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                btnCreaOrdine.Enabled = True
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
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
        If txtRicerca.Text.Trim <> "" Then
            BuidDett()
        End If
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        LnkStampa.Visible = False
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
        'GIU090418 CHIAMO SEMPRE BUILDDETT ALTRIMENTI QUANDO AZZERO TXTRICERCA NON RIESEGUE LA RICERCA SU TUTTI
        'If txtRicerca.Text.Trim <> "" Then
        BuidDett()
        'End If
    End Sub

    'giu220617
    Private Sub ImpostaFiltro()
        SqlDSPrevTElenco.FilterExpression = ""
        If ddlRicerca.SelectedValue <> "CG" Then 'CODICE COGE
            Session(CSTSORTPREVTEL) = Left(ddlRicerca.SelectedValue.Trim, 1)
        Else
            Session(CSTSORTPREVTEL) = "R" 'PER RAGIONE SOCIALE
        End If
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
        If txtRicerca.Text.Trim <> "" Then
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
            ElseIf ddlRicerca.SelectedValue = "CC" Then 'GIU090212
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "CIG like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%' OR CUP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "CIG = '" & Controlla_Apice(txtRicerca.Text.Trim) & "' OR CUP = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "R" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
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
                    SqlDSPrevTElenco.FilterExpression = "Localita >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "M" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "CAP >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "S" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Denominazione >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "P" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "F" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "NR" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Riferimento >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "D1" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Destinazione1 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Destinazione1 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "D2" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Destinazione2 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Destinazione2 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "D3" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Destinazione3 like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Destinazione3 >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            End If
        End If
        If rbtnDaConfermare.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "(StatoDoc = 0)"
        End If
        If rbtnConfermati.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "(StatoDoc = 1)"
        End If
        If rbtnChiusoNoConf.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "(StatoDoc = 3)"
        End If
        If rbtnNonConferm.Checked Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "(StatoDoc = 4)"
        End If
    End Sub
#End Region

    Private Sub btnVisualizza_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVisualizza.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione preventivi/offerte")
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione preventivi/offerte")
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewPR"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo Preventivo", "Confermi la creazione del nuovo Preventivo ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewPR()
        Session(SWOP) = SWOPNUOVO
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione preventivi/offerte")
    End Sub

    Private Sub btnCreaOrdine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaOrdine.Click
        'EVASIONE CLASSICA Response.Redirect("WF_EvadiDocumenti.aspx?labelForm=Crea Ordine")
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu230822
        'GIU170422 BLOCCO ALLESTIMENTO 
        Dim strBloccoCliente As String = "" : Dim strErrore As String = ""
        Dim objControllo As New Controllo
        Dim SWNoFatt As Boolean = False
        Dim SWNoPIVACF As Boolean = False
        Dim SWNoCodIPA As Boolean = False
        Dim swNoDestM As Boolean = False : Dim swNODatiCorr As Boolean = False : Dim swNoCVett As Boolean = False
        SWNoFatt = objControllo.CKCliNoFattByIDDoc(myID, SWNoPIVACF, SWNoCodIPA, swNoDestM, swNODatiCorr, swNoCVett, strErrore)
        objControllo = Nothing
        If strErrore.Trim = "" Then
            Dim swOK As Boolean = False
            strBloccoCliente = "<strong><span>Attenzione!!! (Non bloccante), Cliente:<br>"
            If SWNoPIVACF Then
                strBloccoCliente += "SENZA P.IVA/C.F.<br>"
                swOK = True
            End If
            If SWNoCodIPA Then
                strBloccoCliente += "SENZA Codice IPA<br>"
                swOK = True
            End If
            If SWNoFatt Then
                strBloccoCliente += "NON Fatturabile<br>"
                swOK = True
            End If
            If swNoCVett Then
                strBloccoCliente += "SENZA Vettore<br>"
                swOK = True
            End If
            If swNoDestM Then
                strBloccoCliente += "SENZA Destinazione Merce<br>"
                swOK = True
            ElseIf swNODatiCorr Then
                strBloccoCliente += "SENZA Dati corriere<br>"
                swOK = True
            End If
            strBloccoCliente += "</span></strong>"
            If swOK = False Then
                strBloccoCliente = ""
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore verifica Cliente", strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '--------
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaOrdine"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea Ordine (Conferma preventivo)", "Confermi la creazione dell'Ordine da preventivo ?<br>" & strBloccoCliente, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaOrdine()
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
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
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineCliente + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        If AggiornaNumDoc(SWTD(TD.OrdClienti), NDoc, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVO ORDINE
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocOC]"
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
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.OrdClienti)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Dim myIDNew As String = "" 'giu200423
        Try
            myIDNew = ""
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myIDNew = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Ordine da Preventivo", "Verificare i dati dell'Ordine appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Ordine da Preventivo", "Verificare i dati dell'Ordine appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore SQL: " + Ex.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try

        Try
            myID = myIDNew
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuovo Ordine da Preventivo", "Verificare i dati dell'Ordine appena creato <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br></span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Ordine da Preventivo", "Verificare i dati dell'Ordine appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini")
    End Sub
    Public Sub AvviaRicerca()
        Call btnRicerca_Click(Nothing, Nothing)
    End Sub
    Private Sub btnCopia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopia.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaCopia"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo Preventivo (Copia Preventivo)", "Confermi la copia del Preventivo selezionato ? <br><strong><span> " _
                        & "Attenzione, Il tipo Pagamento sarà aggiornato a quello in essere.<br>Si prega di verificare i dati Pagamento.</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaCopia()

        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
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
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPreventivo + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        If AggiornaNumDoc(SWTD(TD.Preventivi), NDoc, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVO ORDINE
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDoc]"
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
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.Preventivi)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Dim myIDNew As String = "" 'giu200423
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myIDNew = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea copia Preventivo", "Verificare i dati del Preventivo appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea copia Preventivo", "Verificare i dati del Preventivo appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore SQL: " + Ex.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try

        Try
            myID = myIDNew
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea copia Preventivo", "Verificare i dati del Preventivo appena creato <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br></span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea copia Preventivo", "Verificare i dati del Preventivo appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'GIU181120 GIU191120 IMPOSTO A 'Non evadibile' cosi impongo l'aggiornamento dell'ordine appena copiato cosi se il tipo pagamento è stato cambiato aggiorna tutto
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "DECLARE @CodiceCG NVARCHAR(16)=NULL, @TipoPag INT=NULL"
            SQLStr += " SELECT @CodiceCG=ISNULL(Cod_Cliente,Cod_Fornitore) FROM DocumentiT WHERE IDDocumenti=" & myID.Trim
            SQLStr += " SELECT @TipoPag=Pagamento_N FROM CliFor WHERE Codice_Coge=@CodiceCG"
            SQLStr += " IF NOT @TipoPag IS NULL"
            SQLStr += " UPDATE DocumentiT SET Cod_Pagamento=@TipoPag, Totale=0,TotNettoPagare=0 WHERE IDDocumenti=" & myID.Trim
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Aggiornamento TIPO PAGAMENTO N° Doc.: (" & NDoc.ToString.Trim & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione Preventivi/Offerte")
    End Sub
    Private Sub btnNuovaRev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovaRev.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "NuovaRev"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuova Rev.Preventivo", "Confermi la creazione nuova Rev. del Preventivo selezionato ? <br> Attenzione, la Rev.Preventivo precedente <br> sarà ANNULLATO.", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub NuovaRev()

        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
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
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0

        Dim row As GridViewRow = GridViewPrevT.SelectedRow
        Dim strNumero As String = row.Cells(CellIdxT.NumDoc).Text.Trim
        If strNumero.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Numero documento non valido.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        ElseIf Not IsNumeric(strNumero.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Numero documento non valido.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        If GetNewRevN(SWTD(TD.Preventivi), strNumero.Trim, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        NDoc = CLng(strNumero.Trim)
        'OK CREAZIONE NUOVO 
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDoc]"
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
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.Preventivi)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Dim myIDNew As String = "" 'giu200423
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            myIDNew = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Rev. Preventivo", "Verificare i dati del Preventivo appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Rev. Preventivo", "Verificare i dati del Preventivo appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore: " + Ex.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try

        Try
            myID = myIDNew
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Crea nuova Rev. Preventivo", "Verificare i dati del Preventivo appena creato <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br></span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Rev. Preventivo", "Verificare i dati del Preventivo appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br></span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'AGGIORNO LE REVISIONI PRECEDENTI DA STATO 0 A 3 CHIUSO NON EVASO
        Dim SWOk As Boolean = True : Dim SWOkT As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=3 WHERE IDDocumenti <> " & myID.Trim
            SQLStr += " AND Tipo_Doc = '" & SWTD(TD.Preventivi) & "'"
            SQLStr += " AND Numero = '" & strNumero.Trim & "'"
            SQLStr += " AND StatoDoc = 0"
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore", "Errore durante la cancellazione movimenti N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            ' ''Exit Sub
        End Try
        ObjDB = Nothing
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione Preventivi/Offerte")
    End Sub

    Private Function GetNewRevN(ByVal TDoc As String, ByVal NDoc As String, ByRef NRev As Integer) As Boolean
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select MAX(RevisioneNDoc) AS RevisioneNDoc From DocumentiT WHERE Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        strSQL += " AND Numero = '" & NDoc.Trim & "'"
        ' ''strSQL += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim rowTes() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    rowTes = ds.Tables(0).Select()
                    NRev = IIf(IsDBNull(rowTes(0).Item("RevisioneNDoc")), 0, rowTes(0).Item("RevisioneNDoc"))
                    NRev += 1
                    GetNewRevN = True
                    Exit Function
                Else
                    NRev = 0
                    GetNewRevN = True
                    Exit Function
                End If
            Else
                NRev = 0
                GetNewRevN = True
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica N.Documento/Rev.N° da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function

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

        SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = TDoc
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

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPELIMINA
        Response.Redirect("WF_Documenti.aspx?labelForm=Elimina Preventivo/Offerta")
    End Sub
    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        LnkStampa.Visible = False
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        'giu210617
        ImpostaFiltro()
        '---------
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                btnCreaOrdine.Enabled = True
                BtnSetByStato(Stato) 'giu010612
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        'giu210617
        ImpostaFiltro()
        '---------
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                LnkStampa.Visible = False
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                btnCreaOrdine.Enabled = True
                BtnSetByStato(Stato) 'giu010612
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewPrevT.Sorting
        LnkStampa.Visible = False
        Session("SortExp") = e.SortExpression
        Session("SortDir") = e.SortDirection
    End Sub
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        Try
            LnkStampa.Visible = False
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            btnCreaOrdine.Enabled = True
            BtnSetByStato(Stato) 'giu010612
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
            btnCreaOrdine.Enabled = True
        End Try
    End Sub

    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
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
            If IsDate(e.Row.Cells(CellIdxT.DataRif).Text) Then
                e.Row.Cells(CellIdxT.DataRif).Text = Format(CDate(e.Row.Cells(CellIdxT.DataRif).Text), FormatoData).ToString
            End If
        End If
    End Sub

    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Qta).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxD.Qta).Text = ""
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
                    e.Row.Cells(CellIdxD.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScR).Text = ""
                End If
            End If
            'If IsDate(e.Row.Cells(6).Text) Then
            '    e.Row.Cells(6).Text = Format(CDate(e.Row.Cells(6).Text), FormatoData).ToString
            'End If
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "TIPO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                ' ''Session(CSTObjReport) = ObjReport
                ' ''Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                Session(CSTNOBACK) = 0 
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub

    Private Sub OKApriStampa(ByRef DsPrinWebDoc As DSPrintWeb_Documenti)
        'GIU31082023 stampa lotti in documento senza il SUBReport
        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim SWStampaDocLotti As Boolean = False
        If App.GetDatiAbilitazioni(CSTABILAZI, "SWSTDOCLT", strValore, strErrore) = True Then
            SWStampaDocLotti = True
        Else
            SWStampaDocLotti = False
        End If
        '---------
        Dim SWSconti As Integer = 1
        If Not String.IsNullOrEmpty(Session(CSTSWScontiDoc)) Then
            If IsNumeric(Session(CSTSWScontiDoc)) Then
                SWSconti = Session(CSTSWScontiDoc)
            End If
        End If
        '-
        Dim SWConfermaDoc As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTSWConfermaDoc)) Then
            If IsNumeric(Session(CSTSWConfermaDoc)) Then
                SWConfermaDoc = Session(CSTSWConfermaDoc)
            End If
        End If
        'giu110319
        Dim SWRitAcc As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTSWRitAcc)) Then
            If IsNumeric(Session(CSTSWRitAcc)) Then
                SWRitAcc = Session(CSTSWRitAcc)
            End If
        End If
        '---------
        Dim Rpt As Object = Nothing
        'giu170320
        ' ''Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        ' ''DsPrinWebDoc = Session(CSTDsPrinWebDoc)
        Dim SWTabCliFor As String = ""
        'GIU 160312
        Try
            If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                'giu110319
                Session(CSTTIPODOC) = DsPrinWebDoc.Tables("DocumentiT").Rows.Item(0).Item("Tipo_Doc").ToString.Trim
                '---------
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica tipo documento.: " + ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        'giu120319
        Try
            If (DsPrinWebDoc.Tables("DocumentiT").Select("RitAcconto=true").Count > 0) Then
                Session(CSTSWRitAcc) = 1
                SWRitAcc = 1
            Else
                Session(CSTSWRitAcc) = 0
                SWRitAcc = 0
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica Ritenuta d'acconto.: " + ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        '-
        Try
            If (DsPrinWebDoc.Tables("DocumentiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
                SWSconti = 1
                Session(CSTSWScontiDoc) = 1
            Else
                SWSconti = 0
                Session(CSTSWScontiDoc) = 0
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica presenza Sconti.: " + ex.Message.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        'GIU END 160312
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDocST() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica Tipo documento.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------------------
        ' ''CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        'GIU160415 documenti con intestazione vecchia sono identificati 05(ditta) 01(versione vecchia) 
        'per poter stampare la versione vecchia nella tabella operatori al campo
        'codiceditta impostarlo 0501
        Try 'giu080324 giu281112 errore che il file Ã¨ gia aperto
            If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
                If SWSconti = 1 Then
                    Rpt = New Preventivo
                    If CodiceDitta = "01" Then
                        Rpt = New Preventivo01
                    ElseIf CodiceDitta = "05" Then
                        Rpt = New Preventivo05
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New Preventivo0501
                    End If
                Else
                    Rpt = New PreventivoNOSconti
                    If CodiceDitta = "01" Then
                        Rpt = New PreventivoNOSconti01
                    ElseIf CodiceDitta = "05" Then
                        Rpt = New PreventivoNOSconti05
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New PreventivoNOSconti0501
                    End If
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdClienti) Then
                If SWSconti = 1 Then
                    Rpt = New Ordine
                    If CodiceDitta = "01" Then
                        Rpt = New Ordine01
                    ElseIf CodiceDitta = "05" Then
                        Rpt = New Ordine05
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New Ordine0501
                    End If
                    '''If SWConfermaDoc = 0 Then
                    '''    Rpt = New Ordine
                    '''    If CodiceDitta = "01" Then
                    '''        Rpt = New Ordine01
                    '''    ElseIf CodiceDitta = "05" Then
                    '''        Rpt = New Ordine05
                    '''    ElseIf CodiceDitta = "0501" Then
                    '''        Rpt = New Ordine0501
                    '''    End If
                    '''Else
                    '''    Rpt = New ConfermaOrdine
                    '''    If CodiceDitta = "01" Then
                    '''        Rpt = New ConfermaOrdine01
                    '''    ElseIf CodiceDitta = "05" Then
                    '''        Rpt = New ConfermaOrdine05
                    '''    ElseIf CodiceDitta = "0501" Then
                    '''        Rpt = New ConfermaOrdine0501
                    '''    End If
                    '''End If
                Else
                    Rpt = New OrdineNoSconti
                    If CodiceDitta = "01" Then
                        Rpt = New OrdineNoSconti01
                    ElseIf CodiceDitta = "05" Then
                        Rpt = New OrdineNoSconti05
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New OrdineNoSconti0501
                    End If
                    '''If SWConfermaDoc = 0 Then
                    '''    Rpt = New OrdineNoSconti
                    '''    If CodiceDitta = "01" Then
                    '''        Rpt = New OrdineNoSconti01
                    '''    ElseIf CodiceDitta = "05" Then
                    '''        Rpt = New OrdineNoSconti05
                    '''    ElseIf CodiceDitta = "0501" Then
                    '''        Rpt = New OrdineNoSconti0501
                    '''    End If
                    '''Else
                    '''    Rpt = New ConfermaOrdineNoSconti
                    '''    If CodiceDitta = "01" Then
                    '''        Rpt = New ConfermaOrdineNoSconti01
                    '''    ElseIf CodiceDitta = "05" Then
                    '''        Rpt = New ConfermaOrdineNoSconti05
                    '''    ElseIf CodiceDitta = "0501" Then
                    '''        Rpt = New ConfermaOrdineNoSconti0501
                    '''    End If
                    '''End If
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
                Rpt = New DDTNoPrezzi
                If CodiceDitta = "01" Then
                    Rpt = New DDTNoPrezzi01
                ElseIf CodiceDitta = "05" Then
                    If SWStampaDocLotti = False Then
                        Rpt = New DDTNoPrezzi05
                    Else
                        Rpt = New DDTNoPrezzi05LT
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New DDTNoPrezzi0501
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.DocTrasportoFornitori) Then
                Rpt = New DDTNoPrezzi
                If CodiceDitta = "01" Then
                    Rpt = New DDTNoPrezzi01
                ElseIf CodiceDitta = "05" Then
                    If SWStampaDocLotti = False Then
                        Rpt = New DDTNoPrezzi05
                    Else
                        Rpt = New DDTNoPrezzi05LT
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New DDTNoPrezzi0501
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale) Then
                'giu251211
                If SWSconti = 1 Then
                    Rpt = New Fattura
                    If CodiceDitta = "01" Then
                        Rpt = New Fattura01
                    ElseIf CodiceDitta = "05" Then
                        If SWStampaDocLotti = False Then
                            Rpt = New Fattura05
                        Else
                            Rpt = New Fattura05LT
                        End If
                        '-
                        If SWRitAcc <> 0 Then
                            If SWStampaDocLotti = False Then
                                Rpt = New Fattura05RA
                            Else
                                Rpt = New Fattura05RALT
                            End If
                        End If
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New Fattura0501
                        If SWRitAcc <> 0 Then
                            If SWStampaDocLotti = False Then
                                Rpt = New Fattura05RA
                            Else
                                Rpt = New Fattura05RALT
                            End If
                        End If
                    End If
                Else
                    Rpt = New FatturaNoSconti
                    If CodiceDitta = "01" Then
                        Rpt = New FatturaNoSconti01
                    ElseIf CodiceDitta = "05" Then
                        If SWStampaDocLotti = False Then
                            Rpt = New FatturaNoSconti05
                        Else
                            Rpt = New FatturaNoSconti05LT
                        End If
                        '-
                        If SWRitAcc <> 0 Then
                            If SWStampaDocLotti = False Then
                                Rpt = New Fattura05RA
                            Else
                                Rpt = New Fattura05RALT
                            End If
                        End If
                    ElseIf CodiceDitta = "0501" Then
                        Rpt = New FatturaNoSconti0501
                        If SWRitAcc = True <> 0 Then
                            If SWStampaDocLotti = False Then
                                Rpt = New Fattura05RA
                            Else
                                Rpt = New Fattura05RALT
                            End If
                        End If
                    End If
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.NotaCredito) Then
                Rpt = New NotaCredito
                If CodiceDitta = "01" Then
                    Rpt = New NotaCredito01
                ElseIf CodiceDitta = "05" Then
                    If SWStampaDocLotti = False Then
                        Rpt = New NotaCredito05
                    Else
                        Rpt = New NotaCredito05LT
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New NotaCredito0501
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.OrdFornitori) Then
                Rpt = New OrdineFornitore
                If CodiceDitta = "01" Then
                    Rpt = New OrdineFornitore01
                ElseIf CodiceDitta = "05" Then
                    Rpt = New OrdineFornitore05
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New OrdineFornitore0501
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or
                Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
                Rpt = New MMNoPrezzi
                If CodiceDitta = "01" Then
                    Rpt = New MMNoPrezzi01
                ElseIf CodiceDitta = "05" Then
                    If SWStampaDocLotti = False Then
                        Rpt = New MMNoPrezzi05
                    Else
                        Rpt = New MMNoPrezzi05LT
                    End If
                ElseIf CodiceDitta = "0501" Then
                    Rpt = New MMNoPrezzi0501
                End If
            ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or
            Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or
            Session(CSTTIPODOC) = SWTD(TD.NotaCorrispondenza) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Stampa tipodocumento non prevista.: " + Session(CSTTIPODOC), WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Stampa tipodocumento non prevista.: " + Session(CSTTIPODOC), WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'ok
            '-----------------------------------
            Rpt.SetDataSource(DsPrinWebDoc)
            Session(CSTNOMEPDF) = InizialiUT.Trim & "PREVOFF.pdf"
            '---------
            ''''Session(CSTESPORTAPDF) = True
            ''''Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & "Preventivi\"
            ''''Dim stPathReport As String = Session(CSTPATHPDF)

            ''''Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            '''''giu140124
            ''''Rpt.Close()
            ''''Rpt.Dispose()
            ''''Rpt = Nothing
            '''''-
            ''''GC.WaitForPendingFinalizers()
            ''''GC.Collect()
            '-------------
        Catch ex As Exception
            Rpt = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        getOutputRPT(Rpt, "PDF")
        LnkStampa.Visible = True
        '''Dim LnkName As String = "~/Documenti/Preventivi/" & Session(CSTNOMEPDF)
        '''LnkStampa.HRef = LnkName
    End Sub
    '@@@@@
    Private Function getOutputRPT(ByVal _Rpt As Object, ByVal _Formato As String) As Boolean
        '_Rpt.Refresh()
        Dim myStream As Stream
        Try
            If _Formato = "PDF" Then
                myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            Else
                myStream = _Rpt.ExportToStream(ExportFormatType.ExcelRecord)
            End If
            Dim byteReport() As Byte = GetStreamAsByteArray(myStream)
            Session("WebFormStampe") = byteReport
        Catch ex As Exception
            Return False
        End Try

        Try
            _Rpt.Close()
            _Rpt.Dispose()
            _Rpt = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        Catch
        End Try
        getOutputRPT = True
    End Function
    Private Shared Function GetStreamAsByteArray(ByVal stream As System.IO.Stream) As Byte()

        Dim streamLength As Integer = Convert.ToInt32(stream.Length)

        Dim fileData As Byte() = New Byte(streamLength) {}

        ' Read the file into a byte array
        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function
    '@@@@@
    Public Function CKCSTTipoDocST(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDocST = True
        TipoDoc = Session(CSTTIPODOC)
        If IsNothing(TipoDoc) Then
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
        'giu270412 per testare i vari moduli di stampa personalizzati
        Dim sTipoUtente As String = ""
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Function
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        InizialiUT = Utente.Codice.Trim 'GIU210120
        'GIU040213 SE VIENE VAORIZZATO IL CODICE DITTA ESEGUE LA STAMPA SU QUEL CODICE 
        'SE NON ESISTE IL REPORT PERSONALIZZATO CON CODICE DITTA METTE QUELLO DI DEMO SENZA CODICE DITTA
        Try
            Dim OpSys As New Operatori
            Dim myOp As OperatoriEntity
            Dim arrOperatori As ArrayList = Nothing
            arrOperatori = OpSys.getOperatoriByName(Utente.NomeOperatore)
            If Not IsNothing(arrOperatori) Then
                If arrOperatori.Count > 0 Then
                    myOp = CType(arrOperatori(0), OperatoriEntity)
                    If myOp.CodiceDitta.Trim <> "" Then
                        CodiceDitta = myOp.CodiceDitta.Trim
                        Return True
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
        '------------------------------------------------------------
        'giu310112 codice ditta per la gestione delle stampe personalizzate
        CodiceDitta = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            Return False
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            Return False
        End If
        If CodiceDitta = "" Then
            Return False
        End If
        '-------------------------------------------------------------------
    End Function
    '----
    Public Sub Chiudi(ByVal strErrore As String)
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale " & strErrore)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDoc = True
        TipoDoc = Session(CSTTIPODOC)
        If IsNothing(TipoDoc) Then
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
    End Function

    'Simone290317
#Region "Creazione DOCUMENTI COLLEGATI"
    Public Sub CancBackWFPDocCollegati()
        'GIU191219
        Session(CSTTIPODOC) = SWTD(TD.Preventivi)
        Session(CSTSORTPREVTEL) = "N"
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Public Sub CallBackWFPDocCollegati()
        'Dim myID As String = Session(IDDOCUMENTI)
        'If IsNothing(myID) Then
        '    myID = ""
        'End If
        'If String.IsNullOrEmpty(myID) Then
        '    myID = ""
        'End If
        'If Not IsNumeric(myID) Then
        '    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        '    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        '    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
        '    Exit Sub
        'End If
        'GIU191219
        Session(CSTTIPODOC) = SWTD(TD.Preventivi)
        Session(CSTSORTPREVTEL) = "N"
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Private Sub btnDocCollegati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocCollegati.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDOCUMCOLLCALL) = Session(IDDOCUMENTI) 'giu201221
        WFPDocCollegati.PopolaGrigliaWUCDocCollegati()
        ' ''WFPDocCollegati.WucElement = Me
        Session(F_DOCCOLL_APERTA) = True
        WFPDocCollegati.Show()
    End Sub
#End Region

    'giu260612 CAMBIO STATO DOCUMENTO
    Private Sub btnCambiaStato_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCambiaStato.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu080312
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
        If (sTipoUtente.Equals(CSTTECNICO)) Or (sTipoUtente.Equals(CSTAMMINISTRATORE)) Then
            'ok PROCEDO
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        WFPCambiaStatoPR.WucElement = Me
        Session(IDDOCCAMBIAST) = myID 'giu181221
        WFPCambiaStatoPR.SetLblMessUtente("Seleziona/modifica dati")
        Session(F_CAMBIOSTATO_APERTA) = True
        WFPCambiaStatoPR.Show(True)
    End Sub
    Public Sub CancBackWFPCambiaStatoPR()
        'nulla
    End Sub
    Public Sub CallBackWFPCambiaStatoPR()
        BuidDett()
    End Sub
End Class