Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Documenti
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Magazzino
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports System.IO 'giu150615
Partial Public Class WUC_ElencoOrdini
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    'GIU210120
    Private CodiceDitta As String = ""
    Private InizialiUT As String = ""
    '---------
    Private SWFatturaPA As Boolean = False 'GIU080814
    Private SWSplitIVA As Boolean = False 'giu05018
    ' ''Private CSTRICERCA As String = "Ricerca"
    Private Enum CellIdxT
        Stato = 1
        NumDoc = 2
        RevN = 3
        DataDoc = 4
        DataCons = 5
        PercImp = 6
        PercImPorto = 7
        CodCliForProvv = 8
        RagSoc = 9
        '------ ....
        DataVal = 15
        Riferimento = 16
        DataRif = 17
        NumDocCA = 21
        DataDocCA = 22
    End Enum
    Private Enum CellIdxD
        CodArt = 0
        DesArt = 1
        UM = 2
        Qta = 3
        QtaEv = 4
        QtaIm = 5
        QtaRe = 6
        QtaAl = 7 'Inviata
        IVA = 8
        Prz = 9
        ScV = 10
        Sc1 = 11
        Importo = 12
        ScR = 13
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTChiamatoDa) = "WF_ElencoOrdini.aspx?labelForm=Gestione ordini CLIENTI"
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
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            btnNuovo.Visible = False
            btnModifica.Visible = False
            btnElimina.Visible = False
            btnNuovaRev.Visible = False
            btnCopia.Visible = False
            btnCreaOF.Visible = False
            btnCreaDDT.Visible = False
            btnNuovoDaOC.Visible = False
            btnFatturaOC.Visible = False
            btnFatturaOCAC.Visible = False
            lblStampe.Visible = False
            btnConfOrdine.Visible = False
            btnStampa.Visible = False
        End If
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
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-
        If (Not IsPostBack) Then
            Try
                ddlRicerca.Items.Add("Numero ordine")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("Data ordine")
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
                '--
                btnSblocca.Text = "Sblocca documento"
                btnCreaOF.Text = "Crea ordine Fornitore"
                btnConfOrdine.Text = "Conferma Ordine"
                'giu110319
                Dim SWRbtnTD As String = Session(CSTSWRbtnTD)
                If IsNothing(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                If String.IsNullOrEmpty(SWRbtnTD) Then
                    SWRbtnTD = ""
                End If
                '-
                If SWRbtnTD.Trim = "" Or SWRbtnTD.Trim <> SWTD(TD.OrdClienti) Then
                    Session(CSTSTATODOCSEL) = "0"
                    Session(CSTSWRbtnTD) = SWTD(TD.OrdClienti)
                End If
                '-----------
                'GIU080319
                Dim strStatoDoc As String = Session(CSTSTATODOCSEL)
                If IsNothing(strStatoDoc) Then
                    strStatoDoc = ""
                End If
                If String.IsNullOrEmpty(strStatoDoc) Then
                    strStatoDoc = ""
                End If
                '-----------
                Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
                Session(CSTSORTPREVTEL) = "*" 'giu201021 PRIMA VOLTA NEL CASO SI VOGLIA SELEZIONARE CON TOP COSI E' PIU VELOCE L'APERTURA
                Session(SWOP) = SWOPNESSUNA
                If strStatoDoc = "" Then
                    Session(CSTSTATODOC) = "0"
                    rbtnDaEvadere.Checked = True
                ElseIf strStatoDoc = "1" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnEvaso.Checked = True
                ElseIf strStatoDoc = "2" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnParzEvaso.Checked = True
                ElseIf strStatoDoc = "3" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnChiusoNoEvaso.Checked = True
                ElseIf strStatoDoc = "4" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnNonEvadibile.Checked = True
                ElseIf strStatoDoc = "5" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbntInAllestimento.Checked = True
                ElseIf strStatoDoc = "999" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnTutti.Checked = True
                ElseIf strStatoDoc = "998" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnImpegnati.Checked = True
                ElseIf strStatoDoc = "997" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnImpegnati100.Checked = True
                ElseIf strStatoDoc = "996" Then
                    Session(CSTSTATODOC) = strStatoDoc
                    rbtnInContratto.Checked = True
                Else
                    Session(CSTSTATODOC) = "0"
                    rbtnDaEvadere.Checked = True
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
                        BtnSetByStato(Stato)
                        GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU220617
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
                Chiudi("Errore: Caricamento Elenco ordini: " & ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me

        WFPSceltaSpedizione.WucElement = Me
        If Session(F_SCELTASPED_APERTA) Then
            WFPSceltaSpedizione.Show()
        End If
        '-
        WFPCambiaStatoOC.WucElement = Me
        If Session(F_CAMBIOSTATO_APERTA) Then
            WFPCambiaStatoOC.Show()
        End If
        'giu090415
        WFPFatturaOC.WucElement = Me
        If Session(F_EVASIONEPARZ_APERTA) Then
            WFPFatturaOC.Show()
        End If
        '-
        'Simone 290317
        WFPDocCollegati.WucElement = Me
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
    End Sub
    'giu270112
    Private Sub BtnSetByStato(ByVal myStato As String)
        'giu270612
        btnSblocca.Visible = False
        Dim SWBloccoModifica As Boolean = False
        Dim SWBloccoElimina As Boolean = False
        Dim SWBloccoCambiaStato As Boolean = False
        '--
        BtnSetEnabledTo(True)
        btnAllestimento.Text = "Allestimento"
        If myStato.Trim = "Evaso" Or _
                        myStato.Trim = "Chiuso non evaso" Or _
                        myStato.Trim = "In Allestimento" Or _
                        myStato.Trim = "Non evadibile" Then
            btnCambiaStato.Enabled = False : SWBloccoCambiaStato = True
            btnModifica.Enabled = False : SWBloccoModifica = True
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnAllestimento.Text = "Allestimento"
            btnAllestimento.Enabled = False
            btnCreaDDT.Enabled = False
            If myStato.Trim = "Evaso" Or _
                       myStato.Trim = "In Allestimento" Then
                btnNuovoDaOC.Enabled = True
            Else
                btnNuovoDaOC.Enabled = False
            End If
            '-
            btnFatturaOC.Enabled = False
            btnFatturaOCAC.Enabled = False
            btnNuovaRev.Enabled = False
            btnCreaOF.Enabled = False
            btnConfOrdine.Enabled = False
            'btnStampa.Enabled = False
        End If
        If myStato.Trim = "In Allestimento" Then
            btnAllestimento.Text = "NO Allestimento"
            btnAllestimento.Enabled = True
        End If
        If myStato.Trim = "Parz. evaso" Then
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnNuovaRev.Enabled = False
            btnConfOrdine.Enabled = False
        End If
        If SWBloccoElimina Or SWBloccoModifica Or SWBloccoCambiaStato Then
            btnSblocca.Visible = True
        End If
    End Sub
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnCambiaStato.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        btnAllestimento.Enabled = Valore
        btnCreaDDT.Enabled = Valore
        btnNuovoDaOC.Enabled = Valore
        btnFatturaOC.Enabled = Valore
        btnFatturaOCAC.Enabled = Valore
        btnCreaOF.Enabled = Valore
        btnStampa.Enabled = Valore
        btnConfOrdine.Enabled = Valore
        btnCopia.Enabled = Valore
        btnNuovaRev.Enabled = Valore
        btnDocCollegati.Enabled = Valore 'Simone290317
    End Sub

#Region " Ordinamento e ricerca"
    Private Sub rbtnDaEvadere_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDaEvadere.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnCreaDDT.Enabled = True
        btnNuovoDaOC.Enabled = True
        btnFatturaOC.Enabled = True
        btnFatturaOCAC.Enabled = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = True
        btnCreaOF.Enabled = True
        Session(CSTSTATODOC) = "0"
        Session(CSTSTATODOCSEL) = "0"
        BuidDett()
    End Sub
    Private Sub rbtnEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnEvaso.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = False
        btnCreaDDT.Enabled = False
        btnNuovoDaOC.Enabled = True
        btnFatturaOC.Enabled = False
        btnFatturaOCAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        btnConfOrdine.Enabled = False
        btnStampa.Enabled = False
        Session(CSTSTATODOC) = "1"
        Session(CSTSTATODOCSEL) = "1"
        BuidDett()
    End Sub
    Private Sub rbtnParzEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnParzEvaso.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnCreaDDT.Enabled = True
        btnNuovoDaOC.Enabled = True
        btnFatturaOC.Enabled = True
        btnFatturaOCAC.Enabled = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = True
        btnCreaOF.Enabled = True
        Session(CSTSTATODOC) = "2"
        Session(CSTSTATODOCSEL) = "2"
        BuidDett()
    End Sub
    Private Sub rbtnChiusoNoEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnChiusoNoEvaso.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = False
        btnCreaDDT.Enabled = False
        btnNuovoDaOC.Enabled = False
        btnFatturaOC.Enabled = False
        btnFatturaOCAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        btnConfOrdine.Enabled = False
        btnStampa.Enabled = False
        Session(CSTSTATODOC) = "3"
        Session(CSTSTATODOCSEL) = "3"
        BuidDett()
    End Sub
    Private Sub rbtnNonEvadibile_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnNonEvadibile.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = False
        btnCreaDDT.Enabled = False
        btnNuovoDaOC.Enabled = False
        btnFatturaOC.Enabled = False
        btnFatturaOCAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        btnConfOrdine.Enabled = False
        btnStampa.Enabled = False
        Session(CSTSTATODOC) = "4"
        Session(CSTSTATODOCSEL) = "4"
        BuidDett()
    End Sub
    Private Sub rbtnInAllestimento_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbntInAllestimento.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnAllestimento.Text = "NO Allestimento"
        btnAllestimento.Enabled = True
        btnCreaDDT.Enabled = False
        btnNuovoDaOC.Enabled = True
        btnFatturaOC.Enabled = False
        btnFatturaOCAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        btnConfOrdine.Enabled = False
        btnStampa.Enabled = False
        Session(CSTSTATODOC) = "5"
        Session(CSTSTATODOCSEL) = "5"
        BuidDett()
    End Sub
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTutti.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnCreaDDT.Enabled = True
        btnNuovoDaOC.Enabled = True
        btnFatturaOC.Enabled = True
        btnFatturaOCAC.Enabled = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = False
        btnCreaOF.Enabled = True
        Session(CSTSTATODOC) = "999"
        Session(CSTSTATODOCSEL) = "999"
        BuidDett()
    End Sub
    Private Sub rbtnImpegnati_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnImpegnati.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnCreaDDT.Enabled = True
        btnNuovoDaOC.Enabled = True
        btnFatturaOC.Enabled = True
        btnFatturaOCAC.Enabled = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = False
        btnCreaOF.Enabled = True
        'giu260617 non più qui ma nella STORE PROCEDURE USANDO I VALORI 998 E 997 IN STATODOC
        Session(CSTSTATODOC) = "998"
        Session(CSTSTATODOCSEL) = "998"
        BuidDett()
    End Sub
    Private Sub rbtnImpegnati100_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnImpegnati100.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnCreaDDT.Enabled = True
        btnNuovoDaOC.Enabled = True
        btnFatturaOC.Enabled = True
        btnFatturaOCAC.Enabled = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = False
        btnCreaOF.Enabled = True
        'giu260617 non più qui ma nella STORE PROCEDURE USANDO I VALORI 998 E 997 IN STATODOC
        Session(CSTSTATODOC) = "997"
        Session(CSTSTATODOCSEL) = "997"
        BuidDett()
    End Sub
    Private Sub rbtnInContratto_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnInContratto.CheckedChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        btnCreaDDT.Enabled = True
        btnNuovoDaOC.Enabled = True
        btnFatturaOC.Enabled = True
        btnFatturaOCAC.Enabled = True
        btnAllestimento.Text = "Allestimento"
        btnAllestimento.Enabled = False
        btnCreaOF.Enabled = True
        'giu260617 non più qui ma nella STORE PROCEDURE USANDO I VALORI 998 E 997 IN STATODOC
        Session(CSTSTATODOC) = "996"
        Session(CSTSTATODOCSEL) = "996"
        BuidDett()
    End Sub
    Private Sub BuidDett()
        'GIU21062017
        ImpostaFiltro()
        SqlDSPrevTElenco.DataBind()
        '---------
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
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
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
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
        LnkStampa.Visible = False : lblStampe.Visible = True
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
        ' ''If txtRicerca.Text.Trim <> "" Then
        BuidDett()
        ' ''End If
    End Sub
    'giu200617
    Private Sub ImpostaFiltro()
        ' ''Session(CSTRICERCA) = ""
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
                ' ''Session(CSTRICERCA) = txtRicerca.Text.Trim
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
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini CLIENTI")
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
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini CLIENTI")
    End Sub
    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewOC"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo ordine Cliente", "Confermi la creazione dell'ordine Cliente ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewOC()
        Session(SWOP) = SWOPNUOVO
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini CLIENTI")
    End Sub
    'giu020320
    Private Sub btnNuovoDaOC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovoDaOC.Click
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
        Dim myMess As String = myID
        If CKContratti(myMess) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", myMess, WUC_ModalPopup.TYPE_ALERT)
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
            ModalPopup.Show("Errore verifica Cliente", strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '--------
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewCADaOC"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo CONTRATTO Cliente da Ordine", "Confermi la creazione CONTRATTO ?" _
                        & "<br><br><strong><span>Attenzione, l'Ordine verrà EVASO automaticamente" _
                        & "<br>se lo stato attuale è Da Evadere" & IIf(myMess.Trim <> "", "<br><br>" + myMess, "") & "</span></strong><br>" & strBloccoCliente, WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Private Function CKContratti(ByRef myMess As String) As Boolean
        CKContratti = True
        Dim myID As String = myMess
        myMess = ""
        If Not IsNumeric(myID) Then
            myMess = "Numero IDENTIFICATIVO ordine errato."
            CKContratti = False
            Exit Function
        End If
        '-ok leggo
        Dim strSQL As String = ""
        strSQL = "Select * From ContrattiT WHERE RefInt = -" & myID.ToString.Trim & ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    myMess = "Attenzione, l'Ordine risulta già collegato al Contratto N° "
                    For Each row In ds.Tables(0).Select()
                        myMess += IIf(IsDBNull(row.Item("Numero")), "", row.Item("Numero"))
                        Exit For
                    Next
                    Exit Function
                End If
            End If
        Catch Ex As Exception
            CKContratti = False
            myMess = "Errore: Lettura Contratti - " & Ex.Message
            Exit Function
        End Try
    End Function
    Public Sub CreaNewCADaOC()
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
        Session(SWOP) = SWOPNUOVO
        Session(CSTNUOVOCADAOC) = myID
        Session(CSTTIPODOCSEL) = SWTD(TD.ContrattoAssistenza)
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(IDDOCUMENTI) = ""
        Session(IDDURATANUM) = "0"
        Session(IDDURATANUMRIGA) = "0"
        Session(CSTNUOVOCADACA) = SWNO
        Response.Redirect("WF_Contratti.aspx?labelForm=Nuovo CONTRATTO da Ordine")
    End Sub
#Region " Crea Ordine Fornitore"
    Private Sub btnCreaOF_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaOF.Click
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewOF"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea Ordine a Fornitore da ordine Cliente", "Confermi la creazione dell'ordine a Fornitore ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewOF()
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        If VerificaQtaPrenotata() = 0 Then
            OkCreaNewOF()
            Exit Sub
        End If

        Session(MODALPOPUP_CALLBACK_METHOD) = "OkCreaNewOF"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Verifica quantità prenotata", "Attenzione, per quest'ordine è già stato creato un ordine a FORNITORE, <br><br> si vuole procedere comunque alla creazione dell'ordine ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub OkCreaNewOF()
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
            NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineFornitore + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        If AggiornaNumDoc(SWTD(TD.OrdFornitori), NDoc, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVO ORDINE A FORNITORE
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocOF]"
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
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.OrdFornitori)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        Try
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
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.OrdFornitori)
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini FORNITORI")
    End Sub
    Private Function VerificaQtaPrenotata() As Decimal
        VerificaQtaPrenotata = 0
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
        If myID = "" Or Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select SUM(Qta_Prenotata) AS Tot_Qta_Prenotata From DocumentiD WHERE IDDocumenti = " & myID
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim rowTes() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    rowTes = ds.Tables(0).Select()
                    VerificaQtaPrenotata = IIf(IsDBNull(rowTes(0).Item("Tot_Qta_Prenotata")), 0, rowTes(0).Item("Tot_Qta_Prenotata"))
                    Exit Function
                Else
                    VerificaQtaPrenotata = 0
                    Exit Function
                End If
            Else
                VerificaQtaPrenotata = 0
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica Quantità prenotata: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
#End Region

#Region " Crea DDT"
    Private Sub btnCreaDDT_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreaDDT.Click

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
        'giu070814
        Dim strErrore As String = ""
        Dim CkNumDoc As Long = CheckNumDoc(SWTD(TD.DocTrasportoClienti), strErrore)
        If CkNumDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica N° Documento da impegnare. " & strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim myNuovoNumero As Long = 0
        If CaricaParametri(Session(ESERCIZIO), strErrore) Then
            myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu010420
        Dim strMessSpese As String = ""
        If CKSpeseAdd(myID, strMessSpese) = False Then
            Exit Sub
        End If
        'giu230822
        'GIU170422 BLOCCO ALLESTIMENTO 
        Dim strBloccoCliente As String = ""
        Dim objControllo As New Controllo
        Dim SWNoFatt As Boolean = False
        Dim SWNoPIVACF As Boolean = False
        Dim SWNoCodIPA As Boolean = False
        Dim swNoDestM As Boolean = False : Dim swNODatiCorr As Boolean = False
        Dim SWNoCVett As Boolean = False
        SWNoFatt = objControllo.CKCliNoFattByIDDoc(myID, SWNoPIVACF, SWNoCodIPA, swNoDestM, swNODatiCorr, SWNoCVett, strErrore)
        objControllo = Nothing
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
            If SWNoCVett Then
                strBloccoCliente += "SENZA Vettore (Non bloccante)<br>"
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
        'GIU200323
        If SWNoPIVACF Or SWNoCodIPA Or swNoDestM Or swNODatiCorr Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", strBloccoCliente + "<strong><span><br>IMPOSSIBILE CREARE DDT<br>CONTATTARE l'ufficio Amministrativo</span></strong>", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '--------
        If myNuovoNumero <> CkNumDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaDDT"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaDDTRecuperaNum"
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & "DOCUMENTO DI TRASPORTO CLIENTI" & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>" & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaDDT"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & "DOCUMENTO DI TRASPORTO CLIENTI" & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & "</span></strong>" & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    'giu010420
    Private Function CKSpeseAdd(ByVal myID As String, ByRef strMess As String) As Boolean
        CKSpeseAdd = False
        If Not IsNumeric(myID.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "IDENTIFICATIVO Documento ERRATO.".Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If

        Dim strSQL As String = "" : Dim TotSpese = "" : Dim TotSpeseAdd = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsDocT As New DataSet
        Dim rowDocT() As DataRow
        strSQL = "Select (ISNULL(Spese_Incasso,0)+ISNULL(Spese_Trasporto,0)+ISNULL(Spese_Imballo,0)+ISNULL(SpeseVarie,0)) " & _
                        "AS TotSpese,TotSpeseAddebitate " & _
                        "From DocumentiT Where IDDocumenti=" & myID.Trim
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsDocT)
            If (dsDocT.Tables.Count > 0) Then
                If (dsDocT.Tables(0).Rows.Count > 0) Then
                    rowDocT = dsDocT.Tables(0).Select()
                    TotSpese = IIf(IsDBNull(rowDocT(0).Item("TotSpese")), "0", rowDocT(0).Item("TotSpese"))
                    If String.IsNullOrEmpty(TotSpese) Then TotSpese = "0"
                    '-
                    TotSpeseAdd = IIf(IsDBNull(rowDocT(0).Item("TotSpeseAddebitate")), "0", rowDocT(0).Item("TotSpeseAddebitate"))
                    If String.IsNullOrEmpty(TotSpeseAdd) Then TotSpeseAdd = "0"
                    'ok
                    If CDec(TotSpese) <> 0 And CDec(TotSpeseAdd) = 0 Then
                        strMess = "<br><br><strong><span>Attenzione, sono presenti delle spese non ancora addebitate,</span></strong><br>" & _
                                         "SARANNO ADDEBITATE LE SPESE in caso contrario TOGLIERLE manualmente<br>" & _
                                         "Alla successiva emissione non saranno più inserite."
                    ElseIf CDec(TotSpese) <> 0 And CDec(TotSpeseAdd) <> 0 Then
                        strMess = "<br><br><strong><span>Attenzione, sono presenti delle spese già addebitate,</span></strong><br>" & _
                                        "NON SARANNO ADDEBITATE LE SPESE in caso contrario INSERIRLE manualmente."
                    End If
                    CKSpeseAdd = True
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Non trovato Documento in archivio.".Trim, WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Non trovato Documento in archivio.".Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Lettura Documento (CKSpeseAdd): " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function
    'giu060814
    Private Function CheckNumDoc(ByVal myTipoDoc As String, ByRef strErrore As String) As Long
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE "
        If myTipoDoc = SWTD(TD.DocTrasportoClienti) Or _
            myTipoDoc = SWTD(TD.DocTrasportoFornitori) Or _
            myTipoDoc = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "'"
            'giu110814 qui non verranno mai creati FC,NC e simile quindi OK no FatturaPA
            ' ''ElseIf myTipoDoc = SWTD(TD.FatturaCommerciale) Or _
            ' ''    myTipoDoc = SWTD(TD.FatturaScontrino) Then
            ' ''    'GIU220714
            ' ''    If rbtnFCPA.Checked = True And myTipoDoc = SWTD(TD.FatturaCommerciale) Then
            ' ''        strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0) "
            ' ''        If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
            ' ''            strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0)"
            ' ''        End If
            ' ''    Else
            ' ''        strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
            ' ''        strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
            ' ''        If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
            ' ''            strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0)"
            ' ''        End If
            ' ''    End If
            ' ''ElseIf myTipoDoc = SWTD(TD.FatturaAccompagnatoria) Then
            ' ''    strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaAccompagnatoria) & "'"
            ' ''ElseIf myTipoDoc = SWTD(TD.NotaCredito) Then
            ' ''    'giu220714
            ' ''    If rbtnNCPA.Checked = True Then
            ' ''        strSQL += "(Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0) "
            ' ''        If GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
            ' ''            strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0)"
            ' ''        End If
            ' ''    Else
            ' ''        strSQL += "(Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0) "
            ' ''        If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
            ' ''            strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
            ' ''            strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
            ' ''        End If
            ' ''    End If
            ' ''ElseIf myTipoDoc = SWTD(TD.NotaCorrispondenza) Then
            ' ''    strSQL += "Tipo_Doc = '" & SWTD(TD.NotaCorrispondenza) & "'"
            ' ''ElseIf myTipoDoc = SWTD(TD.BuonoConsegna) Then
            ' ''    strSQL += "Tipo_Doc = '" & SWTD(TD.BuonoConsegna) & "'"
        Else 'GIU260312 PER TUTTI GLI ALTRI 
            strSQL += "Tipo_Doc = '" & myTipoDoc.ToString.Trim & "'"
        End If

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
                        ' ''    'GIU171012
                        ' ''    CheckNumDoc = IIf((ds.Tables(0).Rows(0).Item("TotDoc") + 1) < CheckNumDoc, CheckNumDoc, (ds.Tables(0).Rows(0).Item("TotDoc") + 1))
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
    'giu080814 giu090814
    Public Sub CreaDDT()
        Call CreaDDTOK(False)
    End Sub
    Public Sub CreaDDTRecuperaNum()
        Call CreaDDTOK(True)
    End Sub
    '---------
    Public Sub CreaDDTOK(Optional ByVal SWRecuperaNum As Boolean = False)
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
        'giu080814
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroDDT + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If SWRecuperaNum Then
            Dim CkNumDoc As Long = CheckNumDoc(SWTD(TD.DocTrasportoClienti), StrErrore)
            If CkNumDoc = -1 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Verifica N° Documento da impegnare. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            NDoc = CkNumDoc
        End If
        '-
        If AggiornaNumDoc(SWTD(TD.DocTrasportoClienti), NDoc, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVO DDT
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocDT]"
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
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.DocTrasportoClienti)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        'giu080814
        If SWFatturaPA = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        'giu050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
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
            ModalPopup.Show("Crea nuovo DDT", "Verificare i dati del DDT appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "</span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo DDT", "Verificare i dati del DDT appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "</span></strong><br>" _
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
                ModalPopup.Show("Crea nuovo DDT", "Verificare i dati del DDT appena creato <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "</span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo DDT", "Verificare i dati del DDT appena creato <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "</span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti)
        Session(IDDOCUMENTI) = myID
        Response.Redirect("WF_Documenti.aspx?labelForm=DOCUMENTO DI TRASPORTO CLIENTI")
    End Sub

#End Region

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
        ModalPopup.Show("Crea nuovo Ordine (Copia ordine)", "Confermi la copia dell'ordine selezionato ? <br><strong><span> " _
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
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.OrdClienti)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        Try
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
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
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
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini CLIENTI")
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
        ModalPopup.Show("Crea nuova Rev.Ordine", "Confermi la creazione nuova Rev. dell'ordine selezionato ? <br><strong><span> " _
                        & "Attenzione, la Rev.Ordine precedente <br> sarà ANNULLATO.</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
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

        If GetNewRevN(SWTD(TD.OrdClienti), strNumero.Trim, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        NDoc = CLng(strNumero.Trim)
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
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.OrdClienti)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        Try
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
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        'AGGIORNO LE REVISIONI PRECEDENTI DA STATO 0 A 3 CHIUSO NON EVASO
        Dim SWOk As Boolean = True : Dim SWOkT As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=3 WHERE IDDocumenti <> " & Session(IDDOCUMENTI).ToString.Trim
            SQLStr += " AND Tipo_Doc = '" & SWTD(TD.OrdClienti) & "'"
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
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini CLIENTI")
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
        Response.Redirect("WF_Documenti.aspx?labelForm=Elimina Ordine")
    End Sub

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        LnkStampa.Visible = False : lblStampe.Visible = True
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
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU220617
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
        LnkStampa.Visible = False : lblStampe.Visible = True
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
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing) 'GIU220617
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
        Session("SortExp") = e.SortExpression
        Session("SortDir") = e.SortDirection
    End Sub
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        LnkStampa.Visible = False : lblStampe.Visible = True
        Try
            Session("PageIndex") = GridViewPrevT.PageIndex
            Session("SelIndex") = GridViewPrevT.SelectedIndex

            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            BtnSetByStato(Stato)
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
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
            If IsNumeric(e.Row.Cells(CellIdxT.PercImp).Text) Then
                If CDec(e.Row.Cells(CellIdxT.PercImp).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.PercImp).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.PercImp).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.PercImp).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.PercImPorto).Text) Then
                If CDec(e.Row.Cells(CellIdxT.PercImPorto).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.PercImPorto).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.PercImPorto).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.PercImPorto).Text = ""
                End If
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataRif).Text) Then
                e.Row.Cells(CellIdxT.DataRif).Text = Format(CDate(e.Row.Cells(CellIdxT.DataRif).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.NumDocCA).Text.Trim) Then
                If CLng(e.Row.Cells(CellIdxT.NumDocCA).Text.Trim) = 0 Then
                    e.Row.Cells(CellIdxT.NumDocCA).Text = ""
                End If
            Else
                e.Row.Cells(CellIdxT.NumDocCA).Text = ""
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataDocCA).Text) Then
                e.Row.Cells(CellIdxT.DataDocCA).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDocCA).Text), FormatoData).ToString
            End If
        End If
    End Sub

    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaIm).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaIm).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaIm).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaIm).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaIm).Text = ""
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

    Private Sub btnConfOrdine_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfOrdine.Click
        LnkStampa.Visible = False : lblStampe.Visible = True
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
                'giu170320
                ' ''Session(CSTObjReport) = ObjReport
                ' ''Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 1
                Session(CSTNOBACK) = 0 
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Elaborazione StampaDOcumento: " + ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        btnStampa.Click
        LnkStampa.Visible = False : lblStampe.Visible = True
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
                'giu170320 passo l'oggetto
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Elaborazione stampaDocumento: " + ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKApriStampa(DsPrinWebDoc)
    End Sub

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

#Region " Allestimento"

    Private Sub btnAllestimento_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAllestimento.Click
        'GIU291211 DA ATTIVARE PER LE SPEDIZIONI 1 A MOLTI ADESSO 1 SPEDIZIONE 1 ORDINE
        ' ''Session(F_SCELTASPED_APERTA) = True
        ' ''WFPSceltaSpedizione.Show(True)

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
        Dim strMessSpese As String = ""
        If CKSpeseAdd(myID, strMessSpese) = False Then 'giu010420
            Exit Sub
        End If
        Dim strErrore As String = "" 'giu221021
        'giu230822
        'GIU170422 BLOCCO ALLESTIMENTO 
        Dim strBloccoCliente As String = ""
        Dim objControllo As New Controllo
        Dim SWNoFatt As Boolean = False
        Dim SWNoPIVACF As Boolean = False
        Dim SWNoCodIPA As Boolean = False
        Dim swNoDestM As Boolean = False : Dim swNODatiCorr As Boolean = False : Dim swNOCVett As Boolean = False
        SWNoFatt = objControllo.CKCliNoFattByIDDoc(myID, SWNoPIVACF, SWNoCodIPA, swNoDestM, swNODatiCorr, swNOCVett, strErrore)
        objControllo = Nothing
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
            If swNOCVett Then
                strBloccoCliente += "SENZA Vettore (Non bloccante)<br>"
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
        If btnAllestimento.Text = "Allestimento" Then
            'GIU200323
            If SWNoPIVACF Or SWNoCodIPA Or swNoDestM Or swNODatiCorr Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", strBloccoCliente + "<strong><span><br>IMPOSSIBILE ALLESTIRE<br>CONTATTARE l'ufficio Amministrativo</span></strong>", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            '-
            Session(MODALPOPUP_CALLBACK_METHOD) = "InsOrdineInSpedizione"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Inserimento Ordine in allestimento", "Confermi l'allestimento Ordine selezionato ?" & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
        ElseIf btnAllestimento.Text = "NO Allestimento" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CancOrdineInSpedizione"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Togli Ordine in allestimento", "Confermi la rimozione dell'Ordine selezionato dall'Allestimento ?" & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    '@@@@@ DA ATTIVARE PER LE SPEDIZIONI 1 A MOLTI ADESSO 1 SPEDIZIONE 1 ORDINE
    Public Sub CallBackWFPSceltaSped(ByVal IDSpedizione As String)
    End Sub
    Public Sub InsOrdineInSpedizione()
        Dim StrErrore As String = "" : Dim StrConferma As String = ""
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
        '-
        'giu090418 giu150418 RICALCOLO GIACENZE/IMPEGNATO DOCUMENTO/ARTICOLI
        If RicalcolaGicenzaDOC(myID, StrErrore, StrConferma) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ElencoOrdini.InsOrdineInSpedizione.RicalcolaGicenzaDOC", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf StrConferma.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "OkOrdineInSped"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nessun articolo presente per l'allestimento", "Confermi l'allestimento Ordine selezionato ? <br>" & _
                            "NOTA: Forzare manualmente la quantità d'allestire nel dettaglio ordini, in caso si proceda.", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        Call OkOrdineInSped()
    End Sub
    Public Sub OkOrdineInSped()
        Dim StrErrore As String = "" : Dim StrConferma As String = ""
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
        '-
        Dim NSped As Long = GetNewSpedizione()
        If NSped < 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in InsOrdineInSpedizione", "Verifica N.Spedizione da impegnare", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '---------------------------------------------------------
        'OK CREAZIONE NUOVA SPEDIZIONE
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

        SqlDbNewCmd.CommandText = "[Insert_DocINSpedizione]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NSped", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDSpedizioni", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.InputOutput, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@NSped").Value = NSped
        SqlDbNewCmd.Parameters.Item("@IDSpedizioni").Value = 0 'NUOVA SPEDIZIONE 
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDSPEDIZIONI) = SqlDbNewCmd.Parameters.Item("@IDSpedizioni").Value
            'SE MI RITORNA -1 ERRORE
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        Try
            myID = Session(IDSPEDIZIONI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO SPEDIZIONE SCONOSCIUTO.: (" & myID.ToString.Trim & ")", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf CLng(myID) < 1 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "INSERIMENTO NUOVO IDENTIFICATIVO SPEDIZIONE.: (" & myID.ToString.Trim & ")", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO SPEDIZIONE SCONOSCIUTO.: (" & myID.ToString.Trim & ")", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini CLIENTI (Allestimento)")
        'GIU171211 GIU271211 SE NON VOGLIO MODIFICARE NULLA: CARICO COMPLETO
        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''ModalPopup.Show("Inserimento Ordine in allestimento", "Operazione avvenuta con sussesso. <br> Numero Spedizione: " & myID.ToString.Trim, WUC_ModalPopup.TYPE_INFO)
    End Sub
    Private Function GetNewSpedizione() As Long
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        'giu280823 cancello SPEDIZIONI con IDDocumenti = NULL
        strSQL = "SELECT Spedizioni.ID, SpedizioniDett.IDDocumenti " & _
                 "FROM Spedizioni LEFT OUTER JOIN " & _
                 "SpedizioniDett ON Spedizioni.ID = SpedizioniDett.IDSpedizione " & _
                 "WHERE(SpedizioniDett.IDDocumenti Is NULL)"
        Dim dsSped As New DataSet
        Try
            Dim I As Integer
            Dim myIDSped As Long = 0
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsSped)
            If (dsSped.Tables.Count > 0) Then
                If (dsSped.Tables(0).Rows.Count > 0) Then
                    For I = 0 To dsSped.Tables(0).Rows.Count - 1
                        If IsDBNull(dsSped.Tables(0).Rows(I).Item("IDDocumenti")) Then
                            myIDSped = dsSped.Tables(0).Rows(I).Item("ID")
                            strSQL = "DELETE Spedizioni WHERE ID=" + myIDSped.ToString.Trim
                            If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL) = False Then
                                'PROSEGUO COMUNQUE Prenderà un numero MAX + 1
                            End If
                        Else
                            'OK PROSEGUO
                        End If
                    Next
                Else
                    'OK NESSUN ORDINE NON REFERENZIATO
                End If
            Else
                'OK NESSUN ORDINE NON REFERENZIATO
            End If
        Catch Ex As Exception
            GetNewSpedizione = -1
            Exit Function
        End Try
        '----------------------------------------------------
        'GIU210819 CANCELLO LE SPEDIZIONI CHE NON SONO REFERENZIATI SU DOCUMENTITestata
        strSQL = "SELECT SpedizioniDett.IDSpedizione, SpedizioniDett.IDDocumenti, DocumentiT.IDDocumenti AS IDDocT " & _
                 "FROM SpedizioniDett LEFT OUTER JOIN " & _
                 "DocumentiT ON SpedizioniDett.IDDocumenti = DocumentiT.IDDocumenti " & _
                 "WHERE(DocumentiT.IDDocumenti Is NULL)"
        Dim ds0 As New DataSet
        Try
            Dim I As Integer
            Dim myIDDoc As Long = 0
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds0)
            If (ds0.Tables.Count > 0) Then
                If (ds0.Tables(0).Rows.Count > 0) Then
                    For I = 0 To ds0.Tables(0).Rows.Count - 1
                        If Not IsDBNull(ds0.Tables(0).Rows(I).Item("IDDocumenti")) Then
                            myIDDoc = ds0.Tables(0).Rows(I).Item("IDDocumenti")
                            Call CancOrdineInSpedbyID(myIDDoc)
                        Else
                            'OK PROSEGUO
                        End If
                    Next
                Else
                    'OK NESSUN ORDINE NON REFERENZIATO
                End If
            Else
                'OK NESSUN ORDINE NON REFERENZIATO
            End If
        Catch Ex As Exception
            GetNewSpedizione = -1
            Exit Function
        End Try
        '------------------------------------------------------------------------------
        strSQL = "Select MAX(ISNULL(NumeroSpedizione,0)) AS Numero From Spedizioni"
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewSpedizione = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewSpedizione = 1
                    End If
                    Exit Function
                Else
                    GetNewSpedizione = 1
                    Exit Function
                End If
            Else
                GetNewSpedizione = 1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewSpedizione = -1
            Exit Function
        End Try

    End Function
    Private Sub CancOrdineInSpedbyID(ByVal IDDoc As Long) 'giu210819
        'OK CANCELLA ORDINE DALLE SPEDIZIONI
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        Dim strValore As String = "" : Dim strErrore As String = ""
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

        SqlDbNewCmd.CommandText = "[Delete_DocINSpedizione]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = IDDoc
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
    End Sub
    Public Sub CancOrdineInSpedizione()
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
        'OK CANCELLA ORDINE DALLE SPEDIZIONI
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

        SqlDbNewCmd.CommandText = "[Delete_DocINSpedizione]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
        btnAllestimento.Enabled = False
        btnCreaDDT.Enabled = False
        btnNuovoDaOC.Enabled = False
        btnFatturaOC.Enabled = False
        btnFatturaOCAC.Enabled = False
        btnNuovaRev.Enabled = False
        'btnCreaOF.Enabled = False
        btnConfOrdine.Enabled = False
        btnStampa.Enabled = False
        Session(CSTSTATODOC) = "5"
        BuidDett()
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Togli Ordine in allestimento", "Operazione avvenuta con sussesso.", WUC_ModalPopup.TYPE_INFO)
    End Sub
    
#End Region

    Private Sub btnSblocca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSblocca.Click
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

        If Not (sTipoUtente.Equals(CSTAMMINISTRATORE)) And _
            Not (sTipoUtente.Equals(CSTTECNICO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '----------------------------
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "SbloccaDoc"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Sblocca Documento", "Confermi lo sblocco del documento ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub SbloccaDoc()
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        btnSblocca.Visible = False
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
            'giu230412
            ' ''If TipoDoc <> SWTD(TD.FatturaCommerciale) And TipoDoc <> SWTD(TD.FatturaAccompagnatoria) Then
            ' ''    If btnCreaFattura.Enabled = False Then btnCreaFattura.Enabled = True
            ' ''End If
            If btnModifica.Enabled = False Then btnModifica.Enabled = True
            If btnElimina.Enabled = False Then btnElimina.Enabled = True
            If btnCambiaStato.Enabled = False Then btnCambiaStato.Enabled = True
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

    End Sub

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
        WFPCambiaStatoOC.WucElement = Me
        Session(IDDOCCAMBIAST) = myID 'giu181221
        WFPCambiaStatoOC.SetLblMessUtente("Seleziona/modifica dati")
        Session(F_CAMBIOSTATO_APERTA) = True
        WFPCambiaStatoOC.Show(True)
    End Sub
    Public Sub CancBackWFPCambiaStatoOC()
        'nulla
    End Sub
    Public Sub CallBackWFPCambiaStatoOC()
        btnSblocca.Visible = False
        BuidDett()
    End Sub

    'giu050415
#Region "Fatturazione Parziale ordine"
    Private Sub btnFatturaOC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFatturaOC.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        '.
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
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '------------------------------------------
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
        '-
        Dim strMessSpese As String = ""
        If CKSpeseAdd(myID, strMessSpese) = False Then 'giu010420
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "FatturaORParz"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Fatturazione ordine", "Confermi l'emissione della fattura dell'ordine selezionato ?<br>" & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub FatturaORParz()
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
        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI) 'PER L'EVASIONE PARZIALE

        ApriFattORParz()
    End Sub
    Private Sub ApriFattORParz()
        WFPFatturaOC.WucElement = Me
        WFPFatturaOC.SetLblMessUtente("Seleziona/modifica Quantità articoli da fatturare")
        Session(F_EVASIONEPARZ_APERTA) = True
        WFPFatturaOC.Show(True)
    End Sub
    Public Sub CallBackWFPFatturaParzOC()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
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

        ListaDocD = Session(L_EVASIONEPARZ_DA_CAR)
        If ListaDocD.Count > 0 Then
            'ok proseguo 'giu090415
        Else 'giu080415
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nessuna voce è stata selezionata per l'evasione.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu050415
        StrErrore = ""
        'giu040814 salto il test visto che il DDT ha il segnale se DEVe essere PA oppure no
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        'GIU230312 'giu120412 GESTIONE RECUPERO BUCHI NUMERAZIONE
        'giu260312 se si modifica qui ... ricordarsi modificare anche in GESTIONE DOCUMENTI ed elenchiDoc
        Dim myNuovoNumero As Long = 0
        If SWFatturaPA = False Then
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                Chiudi("Errore: Caricamento parametri generali. " & StrErrore)
                Exit Sub
            End If
        Else
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            Else
                Chiudi("Errore: Caricamento parametri generali. " & StrErrore)
                Exit Sub
            End If
        End If
        '--------------------------------------------
        Dim CkNumDocFC As Long = CheckNumDocFC(StrErrore)
        If CkNumDocFC = -1 Then
            Chiudi("Errore: Verifica N° Fattura da impegnare. " & StrErrore)
            Exit Sub
        End If
        Dim strDesTipoDocumento As String = "FATTURA COMMERCIALE"
        If SWFatturaPA = True Then
            strDesTipoDocumento += " (PA)"
        End If
        If myNuovoNumero <> CkNumDocFC Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaFatturaRecuperaNum"
            ModalPopup.Show("Crea nuova Fattura da Ordine", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDocFC) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaFattura"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da Ordine", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
#End Region

#Region " Crea Fattura"
    Private Function CheckNumDocFC(ByRef strErrore As String) As Long
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero "
        strSQL += "From DocumentiT WHERE "
        If SWFatturaPA = False Then
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

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        CheckNumDocFC = ds.Tables(0).Rows(0).Item("Numero") + 1
                        'giu260819 non va bene, es. i preventivi ci sono le REVISION quindi sicuramente il numero è superiore
                        ' ''If (ds.Tables(0).Rows(0).Item("TotDoc") + 1) <> (ds.Tables(0).Rows(0).Item("Numero") + 1) Then
                        ' ''    'CheckNumDocFC = (ds.Tables(0).Rows(0).Item("TotDoc") + 1)
                        ' ''    'GIU171012
                        ' ''    CheckNumDocFC = IIf((ds.Tables(0).Rows(0).Item("TotDoc") + 1) < CheckNumDocFC, CheckNumDocFC, (ds.Tables(0).Rows(0).Item("TotDoc") + 1))
                        ' ''End If
                    Else
                        CheckNumDocFC = 1
                    End If
                    Exit Function
                Else
                    CheckNumDocFC = 1
                    Exit Function
                End If
            Else
                CheckNumDocFC = 1
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = Ex.Message
            CheckNumDocFC = -1
            Exit Function
        End Try

    End Function
    '------------------------------
    Public Sub CreaFattura()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
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
        'giu090415
        ListaDocD = Session(L_EVASIONEPARZ_DA_CAR)
        If ListaDocD.Count > 0 Then
            Dim StrSql As String
            Try
                StrSql = "Update DocumentiD Set Qta_Fatturata = 0 Where IdDocumenti = " & Session(IDDOCUMENTI)
                ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Fatturata=0 Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            '---------------------
            Dim I As Integer
            Dim QtaDaEv As Decimal
            Dim NRiga As Integer
            Dim StrDato() As String
            '-
            For I = 0 To ListaDocD.Count - 1
                Try
                    StrDato = ListaDocD(I).Split("|")
                    QtaDaEv = StrDato(0)
                    NRiga = StrDato(1)
                    StrSql = "Update DocumentiD Set Qta_Fatturata = " & QtaDaEv.ToString.Replace(",", ".") & " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Fatturata=QtaDaEv Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next
        Else 'giu080415
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nessuna voce è stata selezionata per l'evasione.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu040814 giu050118
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        'giu080614
        If SWFatturaPA = False Then
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '--------------------------------------------
        '-
        ' ''If CKCSTTipoDocSel() = False Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
        ' ''    Exit Sub
        ' ''End If
        '' ''-
        'giu060814
        If SWFatturaPA = False Then
            If AggiornaNumDoc(SWTD(TD.FatturaCommerciale), NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If AggiornaNumDoc("PA", NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '---------
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
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        If SWFatturaPA = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        'giu050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        Try
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
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Response.Redirect("WF_Documenti.aspx?labelForm=FATTURA COMMERCIALE")
    End Sub
    'giu120412 GESTIONE RECUPERO NUMERI FATTURA NON USATI
    Public Sub CreaFatturaRecuperaNum()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
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
        'giu090415
        ListaDocD = Session(L_EVASIONEPARZ_DA_CAR)
        If ListaDocD.Count > 0 Then
            Dim StrSql As String
            Try
                StrSql = "Update DocumentiD Set Qta_Fatturata = 0 Where IdDocumenti = " & Session(IDDOCUMENTI)
                ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Fatturata = 0 Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
            '---------------------
            Dim I As Integer
            Dim QtaDaEv As Decimal
            Dim NRiga As Integer
            Dim StrDato() As String
            '-
            For I = 0 To ListaDocD.Count - 1
                Try
                    StrDato = ListaDocD(I).Split("|")
                    QtaDaEv = StrDato(0)
                    NRiga = StrDato(1)
                    StrSql = "Update DocumentiD Set Qta_Fatturata = " & QtaDaEv.ToString.Replace(",", ".") & " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Fatturata = QtaDaEv Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next
        Else 'giu080415
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nessuna voce è stata selezionata per l'evasione.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu040814 GIU050118
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        ' ''If CKCSTTipoDocSel() = False Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
        ' ''    Exit Sub
        ' ''End If
        '-
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        NDoc = CheckNumDocFC(StrErrore)
        If NDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "N° Fattura da impegnare (Recupero).", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        'giu060814
        If SWFatturaPA = False Then
            If AggiornaNumDoc(SWTD(TD.FatturaCommerciale), NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If AggiornaNumDoc("PA", NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '---------
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
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        If SWFatturaPA = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        'giu050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        Try
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
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
        'GIU151111 DA IMPLEMENTARE 
        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        'ModalPopup.Show("Crea DDT", "Creazione DDT avvenuta con sussesso. <br> Numero: " & NDoc.ToString.Trim, WUC_ModalPopup.TYPE_INFO)
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Response.Redirect("WF_Documenti.aspx?labelForm=FATTURA COMMERCIALE") 'giu080614 FATTURA COMMERCIALE")
    End Sub
#End Region

    'Simone290317
#Region "Creazione DOCUMENTI COLLEGATI"
    Public Sub CancBackWFPDocCollegati()
        'GIU191219
        Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
        Session(CSTSORTPREVTEL) = "N"
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Public Sub CallBackWFPDocCollegati()
        Session(CSTTIPODOC) = SWTD(TD.OrdClienti)
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

#Region "Emissione fattura per ACCONTO/SALDO FORNITURA"
    Private Sub btnFatturaOCAC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFatturaOCAC.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        '.
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
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
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
        '-
        Dim strMessSpese As String = ""
        If CKSpeseAdd(myID, strMessSpese) = False Then 'giu010420
            Exit Sub
        End If
        'giu220822
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
        Session(MODALPOPUP_CALLBACK_METHOD) = "FatturaOCAC"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "FatturaAC"
        ModalPopup.Show("Emissione Fattura d'Acconto/Saldo", "Confermi l'emissione della fattura d'Acconto/Saldo ?<BR>NOTA, Non sarà evasa alcuna voce dell'ordine." & strBloccoCliente & strMessSpese, WUC_ModalPopup.TYPE_CONFIRM_YNA)
    End Sub
    'NESSUN DOCUMENTO COLLEGATO
    Public Sub FatturaAC()
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
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
        '-----------
        SWFatturaPA = False
        SWSplitIVA = False
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTSPLITIVA) = SWSplitIVA
        'GIU230312 GESTIONE RECUPERO BUCHI NUMERAZIONE
        'giu260312 se si modifica qui ... ricordarsi modificare anche in GESTIONE DOCUMENTI ed elenchiDoc
        Dim strErrore As String = "" : Dim myNuovoNumero As Long = 0
        If CaricaParametri(Session(ESERCIZIO), strErrore) Then
            myNuovoNumero = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali. " & strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '--------------------------------------------
        Dim CkNumDoc As Long = CheckNumDocFC(strErrore)
        If CkNumDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali. " & strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If myNuovoNumero <> CkNumDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewFCAC"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewFCACRecuperaNum"
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewFCAC"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(myNuovoNumero) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    Public Sub CreaNewFCAC()
        '----------------------------
        Session(CSTFATTURAPA) = False
        SWFatturaPA = False
        'giu230714
        Session(CSTSPLITIVA) = False
        SWSplitIVA = False
        '---------
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNUOVO
        Session(SWOPNUOVONUMDOC) = SWSI
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub
    Public Sub CreaNewFCACRecuperaNum()
        '----------------------------
        Session(CSTFATTURAPA) = False
        SWFatturaPA = False
        'giu230714
        Session(CSTSPLITIVA) = False
        SWSplitIVA = False
        '---------
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPNUOVO
        Session(SWOPNUOVONUMDOC) = SWNO
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub

    'giu020519 CON DOCUMENTO COLLEGATO 
    Public Sub FatturaOCAC()
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        'giu080312
        Dim StrErrore As String = ""
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
        '------------------------------------------
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
        '@@@@@@@@@@@ DOCUMENTO IN 
        'giu040814 giu050118
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        'giu080614
        If SWFatturaPA = False Then
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '-
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        'GIU201221 LO ASSEGNO QUANDO PASSO ALLA MODIFICA DELLA FATTURA Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        If SWFatturaPA = True Then
            strDesTipoDocumento += " (PA)"
        End If
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTSPLITIVA) = SWSplitIVA
        '--------------------------------------------
        'GESTIONE RECUPERO BUCHI NUMERAZIONE
        '--------------------------------------------
        Dim CkNumDoc As Long = CheckNumDocFC(StrErrore)
        If CkNumDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Ultimo numero Fattura. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        If NDoc <> CkNumDoc Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewFCACOR"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewFCACORRecuperaNum"
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(NDoc) & " <br>" _
                        & "Attenzione, ci sono dei numeri documento da recuperare!!!</span></strong> <br> " _
                        & "(Ultimo acquisito+1) N° da recuperare: <strong><span>" _
                        & FormattaNumero(CkNumDoc) & " </span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewFCACOR"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuovo Documento", "Confermi la creazione del documento ? <br><strong><span> " _
                        & strDesTipoDocumento & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & FormattaNumero(NDoc) & "</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    Public Sub CreaNewFCACOR()
        Call CreaNewFCACOR_OK(False)
    End Sub
    Public Sub CreaNewFCACORRecuperaNum()
        Call CreaNewFCACOR_OK(True)
    End Sub
    Private Sub CreaNewFCACOR_OK(Optional ByVal SWRecuperaNum As Boolean = False)
        Dim StrErrore As String = ""
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
        '------------------------------------------
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
        '@@@@@@@@@@@ DOCUMENTO IN 
        'giu040814 giu050118
        SWFatturaPA = Documenti.CKClientiIPAByIDDocORCod(CLng(myID), "", SWSplitIVA, StrErrore)
        Session(CSTFATTURAPA) = SWFatturaPA 'GIU060814
        Session(CSTSPLITIVA) = SWSplitIVA
        If StrErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '---------
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        If SWFatturaPA = False Then
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroFattura + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroPA + 1
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento parametri generali. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '-
        Dim strDesTipoDocumento As String = DesTD(SWTD(TD.FatturaCommerciale))
        strDesTipoDocumento += " per ACCONTO/SALDO"
        'GIU201221 LO ASSEGNO DOPO Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        If SWFatturaPA = True Then
            strDesTipoDocumento += " (PA)"
        End If
        Session(CSTFATTURAPA) = SWFatturaPA
        Session(CSTSPLITIVA) = SWSplitIVA
        '--------------------------------------------
        'GESTIONE RECUPERO BUCHI NUMERAZIONE
        '--------------------------------------------
        Dim CkNumDoc As Long = CheckNumDocFC(StrErrore)
        If CkNumDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Ultimo numero Fattura. " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf SWRecuperaNum = True Then
            NDoc = CkNumDoc
        End If

        If SWFatturaPA = False Then
            If AggiornaNumDoc(SWTD(TD.FatturaCommerciale), NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Else
            If AggiornaNumDoc("PA", NDoc, NRev) = False Then
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '---------
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

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocFCAcconto]"
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
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.FatturaCommerciale)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        If SWFatturaPA = True Then
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@FatturaPA").Value = 0
        End If
        'giu050118
        If SWSplitIVA = True Then
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 1
        Else
            SqlDbNewCmd.Parameters.Item("@SplitIVA").Value = 0
        End If
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
            ModalPopup.Show("Crea nuova Fattura da Ordine", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche l'Ordine che sia con lo stato Evaso</span></strong><br>" _
                        & "Errore SQL: " + ExSQL.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da Ordine", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche l'Ordine che sia con lo stato Evaso </span></strong><br>" _
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
                ModalPopup.Show("Crea nuova Fattura da Ordine", "Verificare i dati della fattura appena creata <br><strong><span> " _
                            & "</span></strong> <br> Nuovo numero: <strong><span>" _
                            & NDoc.ToString.Trim & "<br> Verificare anche l'Ordine che sia con lo stato Evaso</span></strong><br>" _
                            & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = "AvviaRicerca"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea nuova Fattura da Ordine", "Verificare i dati della fattura appena creata <br><strong><span> " _
                        & "</span></strong> <br> Nuovo numero: <strong><span>" _
                        & NDoc.ToString.Trim & "<br> Verificare anche l'Ordine che sia con lo stato Evaso</span></strong><br>" _
                        & "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
        Session(IDDOCUMENTI) = myID.Trim
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub
    Public Sub AvviaRicerca()
        Call btnRicerca_Click(Nothing, Nothing)
    End Sub
#End Region
    'GIU210120
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
        Dim NomeStampa As String = Session(CSTTIPODOC)
        Dim SubDirDOC As String = ""
        'GIU160415 documenti con intestazione vecchia sono identificati 05(ditta) 01(versione vecchia) 
        'per poter stampare la versione vecchia nella tabella operatori al campo
        'codiceditta impostarlo 0501
        If Session(CSTTIPODOC) = SWTD(TD.Preventivi) Then
            NomeStampa = "PREVOFF.PDF"
            SubDirDOC = "Preventivi"
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
            NomeStampa = "ORDINE.PDF"
            SubDirDOC = "Ordini"
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
                '''    NomeStampa = "CONFORDINE.PDF"
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
                '''    NomeStampa = "CONFORDINE.PDF"
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
            SubDirDOC = "DDTClienti"
            NomeStampa = "DDTCLIENTE.PDF"
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
            SubDirDOC = "DDTFornit"
            NomeStampa = "DDTFORNIT.PDF"
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
            NomeStampa = "FATTURA.PDF"
            SubDirDOC = "Fatture"
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
            NomeStampa = "NOTACREDITO.PDF"
            SubDirDOC = "NoteCredito"
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
            NomeStampa = "ORDINEFOR.PDF"
            SubDirDOC = "Ordini"
            Rpt = New OrdineFornitore
            If CodiceDitta = "01" Then
                Rpt = New OrdineFornitore01
            ElseIf CodiceDitta = "05" Then
                Rpt = New OrdineFornitore05
            ElseIf CodiceDitta = "0501" Then
                Rpt = New OrdineFornitore0501
            End If
        ElseIf Session(CSTTIPODOC) = SWTD(TD.MovimentoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino) Or _
                Session(CSTTIPODOC) = SWTD(TD.ScaricoMagazzino) Then
            NomeStampa = "MOVMAG.PDF"
            SubDirDOC = "MovMag"
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
        ElseIf Session(CSTTIPODOC) = SWTD(TD.BuonoConsegna) Or _
            Session(CSTTIPODOC) = SWTD(TD.DocTrasportoCLavoro) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaAccompagnatoria) Or _
            Session(CSTTIPODOC) = SWTD(TD.FatturaScontrino) Or _
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
        Session(CSTNOMEPDF) = InizialiUT.Trim & NomeStampa.Trim
        'giu150320 giu170320 TROPPO LENTO LASCIO COME PRIMA 
        ' ''LnkStampa.HRef = "~/WebFormTables/Stampa.aspx"
        ' ''LnkStampa.Visible = True
        '' '' ''LnkConfOrdine.HRef = "~/WebFormTables/Stampa.aspx"
        '' '' ''LnkListaCarico.HRef = "~/WebFormTables/Stampa.aspx"
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
        ' ''    Rpt = Nothing
        ' ''Catch ex As Exception
        ' ''    Rpt = Nothing
        ' ''    Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
        ' ''    exit sub
        ' ''End Try
        ' ''Session("objReport") = myOBJ
        ' ''Exit Sub
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '---------
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "\", "")
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
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.Visible = True : lblStampe.Visible = False
            ' ''ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
            ' ''    LnkConfOrdine.Visible = True
            ' ''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            ' ''    LnkListaCarico.Visible = True
        Else
            LnkStampa.Visible = True : lblStampe.Visible = False
        End If

        Dim LnkName As String = "~/Documenti/" & IIf(SubDirDOC.Trim <> "", SubDirDOC.Trim & "/", "") & Session(CSTNOMEPDF)
        If Session(CSTTASTOST) = btnStampa.ID Then
            LnkStampa.HRef = LnkName
            ' ''ElseIf Session(CSTTASTOST) = btnConfOrdine.ID Then
            ' ''    LnkConfOrdine.HRef = LnkName
            ' ''ElseIf Session(CSTTASTOST) = btnListaCarico.ID Then
            ' ''    LnkListaCarico.HRef = LnkName
        Else
            LnkStampa.HRef = LnkName
        End If

    End Sub
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
End Class