Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Partial Public Class WUC_StatFatturatoMese
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup.WucElement = Me
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSEserFinoAl.ConnectionString = dbCon.getConnectionString(TipoDB.dbInstall)
        If (Not IsPostBack) Then
            DDLEser.Items.Clear()
            'DDLEserFinoAl.Items.Add("")
            SqlDSEserFinoAl.DataBind()
            Try
                DDLEser.SelectedIndex = 0
                DDLFinoAlMM.SelectedIndex = Now.Date.Month - 1
            Catch ex As Exception
            End Try
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            If Session(CSTSTATISTICHE) <> TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPr And _
                Session(CSTSTATISTICHE) <> TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt Then
                Session(MODALPOPUP_CALLBACK_METHOD) = "CallMenu"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Sessione scaduta, si prega di riprovare", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            If Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt Then
                PanelFor.Visible = True
            Else
                PanelFor.Visible = False
            End If
        End If

        WFP_ElencoCliForn1.WucElement = Me
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click

        Dim StrErroreCampi As String = ""
        Dim ErroreCampi As Boolean = False

        If Session(CSTSTATISTICHE) <> TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPr And _
                Session(CSTSTATISTICHE) <> TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CallMenu"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Sessione scaduta, si prega di riprovare", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If DDLEser.SelectedIndex < 0 Then
            StrErroreCampi = StrErroreCampi & "<BR>- selezionare l'esercizio in corso"
            ErroreCampi = True
        End If
        If Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt Then
            If chkTuttiFornitori.Checked = False Then
                If txtCodFornitore.Text.Trim = "" Or txtDescFornitore.Text = "" Then
                    StrErroreCampi = StrErroreCampi & "<BR>- Codice Fornitore richiesto."
                    ErroreCampi = True
                End If
            End If
        End If
        If ErroreCampi Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPr Then
            Call OKStampaAnnoMese()
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt Then
            Call OKStampaAnnoArtFor()
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Stampa non prevista.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

    End Sub
    Private Function OKStampaAnnoArtFor() As Boolean
        Dim strTitolo As String = "Fatturato Annuo Articoli (" & DDLEser.SelectedValue.Trim & "/" & (CLng(DDLEser.SelectedValue) - 1).ToString.Trim & ") Fino a " & DDLFinoAlMM.SelectedItem.Text.Trim & " " & DDLEser.SelectedValue.Trim
        Dim dsFattCliFatt1 As New DSFatturatoClienteFattura
        Dim ObjReport As New Object

        Dim dbcon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SQLConn As New SqlConnection
        Dim ObjDB As New DataBaseUtility
        '-
        Dim strSQL As String = ""
        dsFattCliFatt1.ArtFor.Clear()
        If chkTuttiFornitori.Checked = False Or txtCodFornitore.Text.Trim <> "" Then
            strSQL = "SELECT AnaMag.Cod_Articolo, ISNULL(AnaMag.CodiceFornitore,'') AS CodiceFornitore, ISNULL(Fornitori.Rag_Soc,'') AS DesFor " & _
                   "FROM AnaMag LEFT OUTER JOIN " & _
                   "Fornitori ON AnaMag.CodiceFornitore = Fornitori.Codice_CoGe"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsFattCliFatt1, "ArtFor")
            Catch Ex As Exception
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore Carico Articoli/Fornitori", Ex.Message, WUC_ModalPopup.TYPE_ALERT)
                Return False
                Exit Function
            End Try
        End If
        'NOTA ci sono 3 funzioni nel caso chiedesse: DaFatturareClientiArticolo,FatturatoClientiArticolo,VendutoClientiArticolo
        'Select Cod_Articolo, Descrizione, SUM(TotQta) AS TOTQTA, SUM(Imponibile) AS TOTIMP  
        'From FatturatoClientiArticolo('01/01/2020','31/10/2020','31/10/2020',NULL,NULL,NULL,NULL,NULL) AS Expr1
        'GROUP BY Cod_Articolo, Descrizione
        Dim strEstraiDati As String = "Select Cod_Articolo, Descrizione, SUM(TotQta) AS TOTQTA, SUM(Imponibile) AS TOTIMP  From FatturatoClientiArticolo("
        '----------
        Dim dsEstrDati As New DataSet
        Try
            dsFattCliFatt1.FatturatoAnnuoArticoli.Clear()
            Dim NewRow As DSFatturatoClienteFattura.FatturatoAnnuoArticoliRow
            Dim EditRow As DSFatturatoClienteFattura.FatturatoAnnuoArticoliRow
            Dim FindRow As DSFatturatoClienteFattura.ArtForRow
            '-
            Dim DaData As String = "" : Dim AData As String = ""
            Dim AnnoIn As String = DDLEser.SelectedItem.Text.Trim
            Dim AnnoPr As String = Format(CLng(AnnoIn) - 1, "0000")
            '-
            DaData = "01/" + Format(1, "00") + "/" + AnnoIn
            AData = "01/" + Format(DDLFinoAlMM.SelectedIndex, "00") + "/" + AnnoIn
            AData = Format(DateAdd(DateInterval.Month, 1, CDate(AData)), FormatoData)
            AData = Format(DateAdd(DateInterval.Day, -1, CDate(AData)), FormatoData)
            '-
            
            strSQL = strEstraiDati + "'" + DaData + "','" + AData + "','" + AData + "',NULL,NULL,NULL,NULL,NULL) AS Expr1 GROUP BY Cod_Articolo, Descrizione"
            dsEstrDati.Clear()
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsEstrDati)
            If (dsEstrDati.Tables.Count > 0) Then
                If (dsEstrDati.Tables(0).Rows.Count > 0) Then
                    For Each rsDatiA1 In dsEstrDati.Tables(0).Select("")
                        'GIU300321 ACORPARE PER CODICE ARTICOLO (ZIBORDI)
                        EditRow = dsFattCliFatt1.FatturatoAnnuoArticoli.FindByCod_Articolo(rsDatiA1.Item("Cod_Articolo"))
                        If EditRow Is Nothing Then
                            NewRow = dsFattCliFatt1.FatturatoAnnuoArticoli.NewRow
                            NewRow.Cod_Articolo = rsDatiA1.Item("Cod_Articolo")
                            NewRow.Descrizione = rsDatiA1.Item("Descrizione")
                            NewRow.AziendaReport = Session(CSTAZIENDARPT).ToString.Trim
                            NewRow.TitoloReport = strTitolo
                            'NewRow.PiedeReport = ""
                            NewRow.QtaA1 = rsDatiA1.Item("TOTQTA")
                            NewRow.ImpA1 = rsDatiA1.Item("TOTIMP")
                            '-
                            If chkTuttiFornitori.Checked = False Or txtCodFornitore.Text.Trim <> "" Then
                                FindRow = dsFattCliFatt1.ArtFor.FindByCod_Articolo(rsDatiA1.Item("Cod_Articolo"))
                                If FindRow Is Nothing Then
                                    NewRow.CodiceFornitore = ""
                                    NewRow.DesFor = ""
                                Else
                                    NewRow.CodiceFornitore = FindRow.CodiceFornitore
                                    NewRow.DesFor = FindRow.DesFor
                                End If
                            Else
                                NewRow.CodiceFornitore = ""
                                NewRow.DesFor = ""
                            End If
                            '-
                            NewRow.QtaA2 = 0
                            NewRow.ImpA2 = 0
                            If txtCodFornitore.Text.Trim <> "" Then
                                NewRow.SingoloFornitore = "Fornitore: " + txtCodFornitore.Text.Trim + " - " + txtDescFornitore.Text.Trim
                            Else
                                NewRow.SingoloFornitore = ""
                            End If
                            NewRow.AnnoA1 = AnnoIn
                            NewRow.AnnoA2 = AnnoPr
                            dsFattCliFatt1.FatturatoAnnuoArticoli.AddFatturatoAnnuoArticoliRow(NewRow)
                        Else
                            EditRow.BeginEdit()
                            EditRow.QtaA1 += rsDatiA1.Item("TOTQTA")
                            EditRow.ImpA1 += rsDatiA1.Item("TOTIMP")
                            EditRow.EndEdit()
                        End If
                    Next

            End If
            End If
            '-precedente
            DaData = "01/" + Format(1, "00") + "/" + AnnoPr
            AData = "01/" + Format(DDLFinoAlMM.SelectedIndex, "00") + "/" + AnnoPr
            AData = Format(DateAdd(DateInterval.Month, 1, CDate(AData)), FormatoData)
            AData = Format(DateAdd(DateInterval.Day, -1, CDate(AData)), FormatoData)
            '-
            strSQL = strEstraiDati + "'" + DaData + "','" + AData + "','" + AData + "',NULL,NULL,NULL,NULL,NULL) AS Expr1 GROUP BY Cod_Articolo, Descrizione"
            dsEstrDati.Clear()
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsEstrDati)
            If (dsEstrDati.Tables.Count > 0) Then
                If (dsEstrDati.Tables(0).Rows.Count > 0) Then
                    For Each rsDatiA2 In dsEstrDati.Tables(0).Select("")
                        EditRow = dsFattCliFatt1.FatturatoAnnuoArticoli.FindByCod_Articolo(rsDatiA2.Item("Cod_Articolo"))
                        If EditRow Is Nothing Then
                            NewRow = dsFattCliFatt1.FatturatoAnnuoArticoli.NewRow
                            NewRow.Cod_Articolo = rsDatiA2.Item("Cod_Articolo")
                            NewRow.Descrizione = rsDatiA2.Item("Descrizione")
                            NewRow.AziendaReport = Session(CSTAZIENDARPT).ToString.Trim
                            NewRow.TitoloReport = strTitolo
                            'NewRow.PiedeReport = ""
                            NewRow.QtaA2 = rsDatiA2.Item("TOTQTA")
                            NewRow.ImpA2 = rsDatiA2.Item("TOTIMP")
                            If chkTuttiFornitori.Checked = False Or txtCodFornitore.Text.Trim <> "" Then
                                FindRow = dsFattCliFatt1.ArtFor.FindByCod_Articolo(rsDatiA2.Item("Cod_Articolo"))
                                If FindRow Is Nothing Then
                                    NewRow.CodiceFornitore = ""
                                    NewRow.DesFor = ""
                                Else
                                    NewRow.CodiceFornitore = FindRow.CodiceFornitore
                                    NewRow.DesFor = FindRow.DesFor
                                End If
                            Else
                                NewRow.CodiceFornitore = ""
                                NewRow.DesFor = ""
                            End If
                            NewRow.QtaA1 = 0
                            NewRow.ImpA1 = 0
                            If txtCodFornitore.Text.Trim <> "" Then
                                NewRow.SingoloFornitore = "Fornitore: " + txtCodFornitore.Text.Trim + " - " + txtDescFornitore.Text.Trim
                            Else
                                NewRow.SingoloFornitore = ""
                            End If
                            NewRow.AnnoA1 = AnnoIn
                            NewRow.AnnoA2 = AnnoPr
                            dsFattCliFatt1.FatturatoAnnuoArticoli.AddFatturatoAnnuoArticoliRow(NewRow)
                        Else
                            EditRow.BeginEdit()
                            EditRow.QtaA2 += rsDatiA2.Item("TOTQTA")
                            EditRow.ImpA2 += rsDatiA2.Item("TOTIMP")
                            EditRow.EndEdit()
                        End If

                    Next

                End If
            End If
            If chkTuttiFornitori.Checked = False And txtCodFornitore.Text.Trim <> "" Then
                For Each rsCancella In dsFattCliFatt1.FatturatoAnnuoArticoli.Select("CodiceFornitore<>'" & txtCodFornitore.Text.Trim & "'")
                    rsCancella.Delete()
                Next
            End If
            dsFattCliFatt1.AcceptChanges()
            'OK
            If dsFattCliFatt1.FatturatoAnnuoArticoli.Count > 0 Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = dsFattCliFatt1
                Session(CSTNOBACK) = 0
                Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessun dato estratto.", WUC_ModalPopup.TYPE_INFO)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in StatFatturatoMese.OKStampaAnnoArtFor", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------
    End Function
    Private Function OKStampaAnnoMese() As Boolean
        Dim strTitolo As String = "Fatturato Articoli Annuo (" & DDLEser.SelectedValue.Trim & "/" & (CLng(DDLEser.SelectedValue) - 1).ToString.Trim & ") Fino a " & DDLFinoAlMM.SelectedItem.Text.Trim & " " & DDLEser.SelectedValue.Trim
        Dim dsClienti1 As New dsClienti
        Dim ObjReport As New Object

        Dim dbcon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SQLConn As New SqlConnection
        'NOTA ci sono 3 funzioni nel caso chiedesse: DaFatturareClientiArticolo,FatturatoClientiArticolo,VendutoClientiArticolo
        'Select 'GENNAIO 2020' AS MESE, SUM(IMPONIBILE) AS TOTALE From FatturatoClientiArticolo('01/01/2020','31/01/2020','31/01/2020',NULL,NULL,NULL,NULL,NULL) AS Expr1 
        'Select 'GENNAIO 2019' AS MESE, SUM(IMPONIBILE) AS TOTALE From FatturatoClientiArticolo('01/01/2019','31/01/2019','31/01/2019',NULL,NULL,NULL,NULL,NULL) AS Expr1 

        Dim strSQL As String = ""
        Dim strEstraiDati As String = "Select SUM(IMPONIBILE) AS TOTALE From FatturatoClientiArticolo("
        '----------
        Dim ObjDB As New DataBaseUtility
        Dim dsEstrDati As New DataSet
        Try
            ' ''Dim rsEstrDati As DataRow
            ' ''Dim rsFattAnnoMese As dsClienti.FattAnnoMeseInPrRow
            dsClienti1.FattAnnoMeseInPr.Clear()
            Dim DaData As String = "" : Dim AData As String = ""
            Dim AnnoIn As String = DDLEser.SelectedItem.Text.Trim
            Dim AnnoPr As String = Format(CLng(AnnoIn) - 1, "0000")
            Dim myTotale As Decimal = 0
            '-
            Dim NewRow As dsClienti.FattAnnoMeseInPrRow
            NewRow = dsClienti1.FattAnnoMeseInPr.NewRow
            NewRow.TitoloReport = strTitolo
            NewRow.PiedeReport = ""
            NewRow.Azienda = Session(CSTAZIENDARPT).ToString.Trim
            NewRow.AnnoIn = AnnoIn
            NewRow.AnnoPr = AnnoPr
            For Mese As Integer = 1 To 12
                If Mese <= DDLFinoAlMM.SelectedIndex Then
                    DaData = "01/" + Format(Mese, "00") + "/" + AnnoIn
                    AData = Format(DateAdd(DateInterval.Month, 1, CDate(DaData)), FormatoData)
                    AData = Format(DateAdd(DateInterval.Day, -1, CDate(AData)), FormatoData)
                    strSQL = strEstraiDati + "'" + DaData + "','" + AData + "','" + AData + "',NULL,NULL,NULL,NULL,NULL) AS Expr1"
                    dsEstrDati.Clear()
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsEstrDati)
                    myTotale = 0
                    If (dsEstrDati.Tables.Count > 0) Then
                        If (dsEstrDati.Tables(0).Rows.Count > 0) Then
                            If Not IsDBNull(dsEstrDati.Tables(0).Rows(0).Item("TOTALE")) Then
                                myTotale = dsEstrDati.Tables(0).Rows(0).Item("TOTALE")
                            End If
                        End If
                    End If
                    NewRow.Item("MeseIn" + Format(Mese, "00")) = myTotale
                    '-precedente
                    DaData = "01/" + Format(Mese, "00") + "/" + AnnoPr
                    AData = Format(DateAdd(DateInterval.Month, 1, CDate(DaData)), FormatoData)
                    AData = Format(DateAdd(DateInterval.Day, -1, CDate(AData)), FormatoData)
                    strSQL = strEstraiDati + "'" + DaData + "','" + AData + "','" + AData + "',NULL,NULL,NULL,NULL,NULL) AS Expr1"
                    dsEstrDati.Clear()
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsEstrDati)
                    myTotale = 0
                    If (dsEstrDati.Tables.Count > 0) Then
                        If (dsEstrDati.Tables(0).Rows.Count > 0) Then
                            If Not IsDBNull(dsEstrDati.Tables(0).Rows(0).Item("TOTALE")) Then
                                myTotale = dsEstrDati.Tables(0).Rows(0).Item("TOTALE")
                            End If
                        End If
                    End If
                    NewRow.Item("MesePr" + Format(Mese, "00")) = myTotale
                Else
                    NewRow.Item("MeseIn" + Format(Mese, "00")) = 0
                    NewRow.Item("MesePr" + Format(Mese, "00")) = 0
                End If
            Next
            'OK
            dsClienti1.FattAnnoMeseInPr.AddFattAnnoMeseInPrRow(NewRow)
            If dsClienti1.FattAnnoMeseInPr.Count > 0 Then
                dsClienti1.AcceptChanges()
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = dsClienti1
                Session(CSTNOBACK) = 0
                Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessun dato estratto.", WUC_ModalPopup.TYPE_INFO)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in StatFatturatoMese.OKStampaAnnoMese", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------
    End Function

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
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Call Chiudi("")
    End Sub
    Public Sub CallMenu()
       Call Chiudi("")
    End Sub
    Private Sub DDLEser_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLEser.SelectedIndexChanged
        Try
            If CLng(DDLEser.SelectedValue) = Now.Date.Year Then
                DDLFinoAlMM.SelectedIndex = Now.Date.Month - 1
            Else
                DDLFinoAlMM.SelectedIndex = 12
            End If

        Catch ex As Exception
        End Try
    End Sub
    'giu051120
    Private Sub chkTuttiFornitori_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiFornitori.CheckedChanged
        pulisciCampiFornitore()
        If chkTuttiFornitori.Checked Then
            AbilitaDisabilitaCampiFornitore(False)
        Else
            AbilitaDisabilitaCampiFornitore(True)
            txtCodFornitore.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiFornitore(ByVal Abilita As Boolean)
        txtCodFornitore.Enabled = Abilita
        btnFornitore.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiFornitore()
        txtCodFornitore.Text = ""
        txtDescFornitore.Text = ""
    End Sub
    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        txtDescFornitore.Text = App.GetValoreFromChiave(txtCodFornitore.Text, Def.FORNITORI, Session(ESERCIZIO))
    End Sub
    Private Sub ApriElencoFornitori1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodFornitore.Text = codice
            txtDescFornitore.Text = descrizione
        End If
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    End Sub
    Private Sub btnFornitore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFornitore.Click
        Session(F_FOR_RICERCA) = True
        ApriElencoFornitori1()
    End Sub

End Class