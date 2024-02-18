Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WFP_CambiaStatoCA
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUCCambiaStatoCA.AggiornaDati()) Then
            Session(F_CAMBIOSTATO_APERTA) = False
            lblMessUtente.Text = ""
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPCambiaStatoCA()
        Else
            lblMessUtente.Text = "Attenzione, Aggiornamento dati non riuscito."
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(F_CAMBIOSTATO_APERTA) = False
        lblMessUtente.Text = ""
        ProgrammaticModalPopup.Hide()
        _WucElement.CancBackWFPCambiaStatoCA()
    End Sub

    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        If PrimaVolta Then
            lblMessUtente.Text = "Seleziona/modifica dati"
            WUCCambiaStatoCA.PrimaVolta()
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

End Class