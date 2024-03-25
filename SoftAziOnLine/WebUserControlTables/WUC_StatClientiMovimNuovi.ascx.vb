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

Partial Public Class WUC_StatClientiMovimNuovi
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup.WucElement = Me
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSEserFinoAl.ConnectionString = dbCon.getConnectionString(TipoDB.dbInstall)
        Dim mylabelForm As String = Request.QueryString("labelForm")
        If InStr(mylabelForm.Trim.ToUpper, "CONTRATTI") > 0 Then
            PanelSelezionaDate.GroupingText = "Statistica Contratti Attivi/Nuovi/In Scadenza per Esercizio"
            lblTotale.Text = "Totale Contratti Attivi/Nuovi/In Scadenza al"
            lblPerRegione.Visible = False
        Else
            PanelSelezionaDate.GroupingText = "Statistica Clienti Movimentati/Nuovi per Esercizio"
            lblTotale.Text = "Totale Clienti Movimentati/Nuovi al"
            lblPerRegione.Visible = True
        End If
        '-
        If (Not IsPostBack) Then
            DDLEserFinoAl.Items.Clear()
            DDLEserFinoAl.Items.Add("")
            SqlDSEserFinoAl.DataBind()
            Try
                DDLEserFinoAl.SelectedIndex = 1
            Catch ex As Exception
            End Try
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If

    End Sub
    Private Sub StatCMAttiviNuovi()
        Dim DsStatVendCliArt1 As New DsStatVendCliArt
        Dim ObjReport As New Object
        Dim ClsPrint As New Statistiche
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"
        Try
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatTotaliContrattiAl
            'CONTROLLI PRIMA DI AVVIARE LA STAMPA
            If DDLEserFinoAl.SelectedIndex < 0 Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare l'esercizio fino al"
                ErroreCampi = True
            End If

            If ErroreCampi Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If

            Dim strDescrizioneSelezioneDati As String = ""
            strDescrizioneSelezioneDati += "Totale Contratti Attivi/Nuovi al " & DDLEserFinoAl.SelectedValue.Trim

            'tutti gli esercizi crescente
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
            Dim strEser As String = DDLEserFinoAl.SelectedValue.Trim
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
            Dim txtDataDa As String = "01/01/" + strEser.Trim
            Dim txtDataA As String = "31/12/" + strEser.Trim
            '-
            Dim StrErrore As String = ""
            Dim CodRegione As Integer = -1
            Dim Provincia As String = ""
            Dim CodCateg As Integer = -1
            Dim SWRaggrCatCli As Boolean = False
            Dim strCategRagg As String = ""
            Dim SWTratt As Integer = 0
            Dim AccorpaCR As Boolean = False
            Dim Modello As String = "ZZZ"
            Dim StatoDOC As String = "999"
            If ClsPrint.StampaContrattiRegPrCatCli(txtDataDa.Trim, txtDataA.Trim, Session(CSTAZIENDARPT), DsStatVendCliArt1, ObjReport, StrErrore, CodRegione, Provincia, CodCateg, strCategRagg, AccorpaCR, "CM", StatoDOC, Modello, True, "") Then
                If DsStatVendCliArt1.StatCMRegPrCCliStato.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDsPrinWebDoc) = DsStatVendCliArt1
                    Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
                Else
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If

        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in StatCMAttiviNuovi.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------
    End Sub
    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim mylabelForm As String = Request.QueryString("labelForm")
        If InStr(mylabelForm.Trim.ToUpper, "CONTRATTI") > 0 Then
            Call StatCMAttiviNuovi()
            Exit Sub
        End If
        Dim dsClienti1 As New dsClienti
        Dim ObjReport As New Object
        Dim StrErroreCampi As String = ""
        Dim ErroreCampi As Boolean = False

        Try
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatTotaliClientiAl
            'CONTROLLI PRIMA DI AVVIARE LA STAMPA
            If DDLEserFinoAl.SelectedIndex < 0 Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare l'esercizio fino al"
                ErroreCampi = True
            End If

            If ErroreCampi Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If

            Dim strDescrizioneSelezioneDati As String = ""
            strDescrizioneSelezioneDati += "Totale Clienti Movimentati/Nuovi fino al " & DDLEserFinoAl.SelectedValue.Trim & " - suddiviso per regione"

            'tutti gli esercizi crescente
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
            Dim dbcon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim SQLConn As New SqlConnection
            Dim strSQL As String = ""
            strSQL = "Select Ditta, Esercizio FROM Esercizi "
            strSQL += "WHERE Ditta = '" & strDitta.Trim & "' "
            strSQL += "ORDER BY Esercizio"
            '-
            Dim strEstraiDati As String = "SELECT Clienti.Codice_CoGe, Clienti.Provincia, Regioni.Descrizione " & _
                "FROM Regioni INNER JOIN Province ON Regioni.Codice = Province.Regione RIGHT OUTER JOIN " & _
                "DocumentiT INNER JOIN " & _
                "Clienti ON DocumentiT.Cod_Cliente = Clienti.Codice_CoGe ON Province.Codice = Clienti.Provincia " & _
                "WHERE     (LEFT(DocumentiT.Tipo_Doc, 1) <> 'P') " & _
                "GROUP BY Clienti.Codice_CoGe, Clienti.Provincia, Regioni.Descrizione "
            '----------
            Dim ObjDB As New DataBaseUtility
            Dim dsEser As New DataSet
            Dim rsEser As DataRow
            Dim rsDett As dsClienti.ClientiMovimentatiRow
            Dim rsEstr As dsClienti.ClientiMovimEstrRow
            Dim NewRow As dsClienti.ClientiMovimentatiRow
            Dim FindRow As dsClienti.ClientiMovimentatiRow
            Dim myEser As String = ""
            Dim myEserFinoAl As String = ""
            myEserFinoAl = DDLEserFinoAl.SelectedValue.Trim
            dsClienti1.ClientiMovimentati.Clear()
            dsClienti1.ClientiMovimEstr.Clear()
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, dsEser)
            If (dsEser.Tables.Count > 0) Then
                If (dsEser.Tables(0).Rows.Count > 0) Then
                    For Each rsEser In dsEser.Tables(0).Select("")
                        myEser = IIf(IsDBNull(rsEser!Esercizio), "", rsEser!Esercizio)
                        If myEser.Trim <= myEserFinoAl.Trim Then
                            If dsClienti1.ClientiMovimentati.Rows.Count = 0 Then
                                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strEstraiDati, dsClienti1, "ClientiMovimentati", myEser.Trim)
                            Else
                                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strEstraiDati, dsClienti1, "ClientiMovimEstr", myEser.Trim)
                                For Each rsEstr In dsClienti1.ClientiMovimEstr.Select("")
                                    FindRow = Nothing
                                    FindRow = dsClienti1.ClientiMovimentati.FindByCodice_Coge(rsEstr.Codice_Coge.Trim)
                                    If FindRow Is Nothing Then
                                        NewRow = dsClienti1.ClientiMovimentati.NewRow
                                        NewRow.Codice_Coge = rsEstr!Codice_Coge
                                        NewRow.Provincia = rsEstr!Provincia
                                        NewRow.Descrizione = rsEstr!Descrizione
                                        If myEser.Trim = myEserFinoAl.Trim Then
                                            NewRow.AnnoPrec = 1
                                        Else
                                            NewRow.AnnoPrec = 0
                                        End If
                                        dsClienti1.ClientiMovimentati.AddClientiMovimentatiRow(NewRow)
                                    End If
                                    'If dsClienti1.ClientiMovimentati.Select("Codice_Coge='" & rsEstr!Codice_CoGe & "'").Length = 0 Then
                                    '    NewRow = dsClienti1.ClientiMovimentati.NewRow
                                    '    NewRow.Codice_Coge = rsEstr!Codice_Coge
                                    '    NewRow.Provincia = rsEstr!Provincia
                                    '    NewRow.Descrizione = rsEstr!Descrizione
                                    '    dsClienti1.ClientiMovimentati.AddClientiMovimentatiRow(NewRow)
                                    'End If
                                Next
                            End If
                            dsClienti1.ClientiMovimEstr.Clear()
                        End If
                    Next
                End If
            End If

            If dsClienti1.ClientiMovimentati.Count > 0 Then
                Dim strNomeAz As String = Session(CSTAZIENDARPT)
                For Each rsDett In dsClienti1.ClientiMovimentati.Select("")
                    rsDett.BeginEdit()
                    If rsDett.IsDescrizioneNull Then
                        rsDett.Descrizione = "!!non definita!!"
                    ElseIf rsDett.Descrizione.Trim = "" Then
                        rsDett.Descrizione = "!!non definita!!"
                    End If
                    rsDett!TitoloReport = strDescrizioneSelezioneDati
                    rsDett!PiedeReport = ""
                    rsDett!Azienda = strNomeAz
                    rsDett.EndEdit()
                Next
                dsClienti1.AcceptChanges()
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = dsClienti1
                Session(CSTNOBACK) = 0
                Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErroreCampi, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in StatClientiMovimNuovi.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------
    End Sub
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
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

End Class