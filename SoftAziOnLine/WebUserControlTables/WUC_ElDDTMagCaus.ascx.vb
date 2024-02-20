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
Partial Public Class WUC_ElDDTMagCaus
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSCausale.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If (Not IsPostBack) Then
            ddlMagazzino.SelectedValue = 1
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            CaricaEsercizi()
            chkTuttiEser.Checked = False
        End If
        ModalPopup.WucElement = Me
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

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Session(CSTNOMEPDF) = "" 'giu300315
        Dim selezione As String = ""
        Dim filtri As String = "Filtri usati: "
        Dim Errore As String = ""
        Dim clsStampa As New Statistiche
        Dim dsMovMag1 As New DSMovMag
        If ddlMagazzino.SelectedItem.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Scegliere il magazzino su cui agire.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Data inizio periodo superiore alla data fine periodo", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
                'GIU111020
                Try
                    If String.IsNullOrEmpty(Session("MinEser")) Then
                        Session("MinEser") = CDate(txtDataDa.Text.Trim).Year
                    End If
                    If String.IsNullOrEmpty(Session("MaxEser")) Then
                        Session("MaxEser") = CDate(txtDataA.Text.Trim).Year
                    End If
                    If CDate(txtDataDa.Text.Trim).Year < Int(Session("MinEser")) Or CDate(txtDataA.Text.Trim).Year > Int(Session("MaxEser")) Then
                        txtDataDa.Text = "01/01/" & Session("MinEser")
                        txtDataA.Text = "31/12/" & Session("MaxEser")
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Controllo Data inizio/fine periodo.: Esercizi non compresi, imposto il primo e ultimo Esercizio", WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Controllo Data inizio/fine periodo.: " + ex.Message, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End Try
                '-
            End If
        End If
        '.GIU220920 giu230421 sempre attivo
        filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper & " - "
        If Val(ddlMagazzino.SelectedValue.Trim) = 0 Then
            'OK TUTTI
        Else
            selezione = " WHERE CodiceMagazzino=" & ddlMagazzino.SelectedValue.Trim
        End If
        '----------
        'giu230421 sempre attivo
        If txtDataDa.Text <> "" Then
            If IsDate(txtDataDa.Text) Then
                If selezione.Trim = "" Then
                    selezione = " WHERE (Data_Doc >= CONVERT(DATETIME,'" & Format(CDate(txtDataDa.Text), FormatoData) & "',103))"
                Else
                    selezione += " AND (Data_Doc >= CONVERT(DATETIME,'" & Format(CDate(txtDataDa.Text), FormatoData) & "',103))"
                End If
                filtri = filtri & "Dal " & txtDataDa.Text.Trim & " | "
            End If
        End If
        If txtDataA.Text <> "" Then
            If IsDate(txtDataA.Text) Then
                If selezione.Trim = "" Then
                    selezione = " WHERE (Data_Doc <= CONVERT(DATETIME,'" & Format(CDate(txtDataA.Text), FormatoData) & "',103))"
                Else
                    selezione = selezione & " AND (Data_Doc <= CONVERT(DATETIME,'" & Format(CDate(txtDataA.Text), FormatoData) & "',103))"
                End If
                filtri = filtri & "Al " & txtDataA.Text.Trim & " | "
            End If
        End If
        '-
        Dim myTitolo As String = "Elenco DDT Clienti per Magazzino/Causale " 'giu051112
        If selezione.Trim = "" Then
            If Val(DDLCausale.SelectedValue.Trim) = 0 Then
                selezione = " WHERE " 'tutte
            Else
                selezione = " WHERE Cod_Causale=" & DDLCausale.SelectedValue.Trim & " AND "
            End If
        Else
            If Val(DDLCausale.SelectedValue.Trim) = 0 Then
                selezione = selezione & " AND " 'tutte
            Else
                selezione = selezione & " AND Cod_Causale=" & DDLCausale.SelectedValue.Trim & " AND "
            End If
        End If
        selezione += " (Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR " & _
                        " Tipo_Doc = '" & SWTD(TD.FatturaAccompagnatoria) & "' OR " & _
                        " FatturaAC<>0) "
        filtri = filtri + " - Causale: " & DDLCausale.SelectedItem.Text.Trim
        filtri = filtri & " - Solo documenti di trasporto "
        '--------------------------

        Try
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.ElencoDDTMagCaus
            If clsStampa.StampaMovMag(Session(CSTAZIENDARPT), myTitolo.Trim, selezione, dsMovMag1, Errore, filtri, -1, "", False, "", "", False, False, txtDataDa.Text, txtDataA.Text, Session("MinEser"), Session("MaxEser")) Then

                If dsMovMag1.view_MovMag.Count > 0 Then
                    Session(CSTDsPrinWebDoc) = dsMovMag1
                    Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebMovMag.aspx")
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub chkTuttiEser_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiEser.CheckedChanged
        If chkTuttiEser.Checked Then
            txtDataDa.Text = "01/01/" & Session("MinEser")
            txtDataA.Text = "31/12/" & Session("MaxEser")
        Else
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
        End If
    End Sub

    Public Sub ErrEser()
        chkTuttiEser.Checked = False
    End Sub

    Private Sub CaricaEsercizi()
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim sqlConnInst As New SqlConnection
        Dim sqlCmdMinMaxEser As New SqlCommand
        Dim MinEser, MaxEser As Integer

        sqlConnInst.ConnectionString = dbCon.getConnectionString(TipoDB.dbInstall)
        sqlCmdMinMaxEser.Connection = sqlConnInst
        sqlCmdMinMaxEser.CommandType = CommandType.Text
        sqlCmdMinMaxEser.CommandText = "SELECT @MinEser = MIN(CAST(Esercizio AS INT)), @MaxEser = MAX(CAST(Esercizio AS INT)) FROM Esercizi WHERE Ditta = @Ditta"
        sqlCmdMinMaxEser.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ditta", System.Data.SqlDbType.NVarChar, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        sqlCmdMinMaxEser.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MinEser", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, True, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        sqlCmdMinMaxEser.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MaxEser", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, True, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        Try
            sqlCmdMinMaxEser.Parameters("@Ditta").Value = Session(CSTCODDITTA)

            If sqlConnInst.State <> ConnectionState.Open Then
                sqlConnInst.Open()
            End If

            sqlCmdMinMaxEser.ExecuteNonQuery()

            MinEser = sqlCmdMinMaxEser.Parameters("@MinEser").Value
            MaxEser = sqlCmdMinMaxEser.Parameters("@MaxEser").Value
            sqlConnInst.Close()
        Catch ex As Exception
            sqlConnInst.Close()
            Session(MODALPOPUP_CALLBACK_METHOD) = "ErrEser"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Si è verificato il seguente errore durante la lettura del minimo e del massimo esercizio: " & vbCr _
                            & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        Session("MinEser") = MinEser
        Session("MaxEser") = MaxEser
    End Sub

End Class