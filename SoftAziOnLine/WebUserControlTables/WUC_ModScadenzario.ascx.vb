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
Partial Public Class WUC_ModScadenzario
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        ModalPopup.WucElement = Me
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try
    End Sub

    Public Sub AggiornaElScadPA()

        Dim Errore As String = ""
        Dim clsStampa As New Controllo
        Dim dsDocContT As New DSPrintWeb_Documenti

        Try
            If clsStampa.AggContrattiScadPagCA(dsDocContT, Errore) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Aggiornamento", "Elenco Scadenze contratti: aggiunto nuovo campo Totale Fatturato/Residuo.<br>Terminato con successo", WUC_ModalPopup.TYPE_INFO)
                Exit Sub
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

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        Session(MODALPOPUP_CALLBACK_METHOD) = "AggiornaElScadPA"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Aggiornamento", "CONFERMI L'AGGIORNAMENTO:<br>Elenco Scadenze contratti: aggiunto nuovo campo Totale Fatturato/Residuo.?", WUC_ModalPopup.TYPE_CONFIRM_YN)
    End Sub
End Class