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
Partial Public Class WUC_ContrattiCKSerieLotto
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

    Private Sub StampaReport(ByVal SWAggiorna As Boolean)

        Dim Errore As String = ""
        Dim clsStampa As New Controllo
        Dim DSCKSerieLotto As New DSPrintWeb_Documenti
        Dim TipoDoc As String = ""

        Try
            If clsStampa.ContrattiCKSerieLotto(DSCKSerieLotto, SWAggiorna, Errore) Then
                If DSCKSerieLotto.ContrattiT.Select("Iniziali<>''").Count <= 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Controllo", "Non ci sono Contratti che abbiano caratteri speciali nel N° Serie/Lotto.", WUC_ModalPopup.TYPE_INFO)
                    Exit Sub
                End If
                Session(CSTDsPrinWebDoc) = DSCKSerieLotto
                Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.CKSerieLotto
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebControllo.aspx")
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

    Private Sub btnElabora_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click, btnAggiorna.Click
        Dim SWAggiorna As Boolean
        If sender.ID = btnStampa.ID Then
            SWAggiorna = False
        Else
            SWAggiorna = True
        End If
        StampaReport(SWAggiorna)
    End Sub

End Class