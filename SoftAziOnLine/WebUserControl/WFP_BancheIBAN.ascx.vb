Imports SoftAziOnLine.Def

Partial Public Class WFP_BancheIBAN
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
    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUC_BancheIBAN.AggiornaBancheIBAN()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPBancheIBAN()
            Session(F_BANCHEIBAN_APERTA) = False
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        WUC_BancheIBAN.SvuotaCampi()
        _WucElement.CancBackWFPBancheIBAN()
        Session(F_BANCHEIBAN_APERTA) = False
    End Sub

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_BancheIBAN.SvuotaCampi()
    End Sub

End Class