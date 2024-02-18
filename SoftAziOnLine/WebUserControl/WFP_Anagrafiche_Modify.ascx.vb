Imports SoftAziOnLine.Def

Partial Public Class WFP_Anagrafiche_Modify
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
        If (WUC_Anagrafiche_Modify.AggiornaAnagrCliFor()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPAnagrCliFor()
            Session(F_ANAGRCLIFOR_APERTA) = False
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        WUC_Anagrafiche_Modify.SvuotaCampi()
        Session(F_ANAGRCLIFOR_APERTA) = False
    End Sub

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_Anagrafiche_Modify.SvuotaCampi()
    End Sub

    Public Sub PopolaCampi()
        WUC_Anagrafiche_Modify.PopolaCampi()
    End Sub

End Class