Imports SoftAziOnLine.Def

Partial Public Class WFP_FornitoreSec
    Inherits System.Web.UI.UserControl

#Region "Property"

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

#End Region

#Region "Metodi private"
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUC_FornitoreSec.PopolaEntityDatiFornitoreSec()) Then
            Session(F_FORNSEC_APERTA) = False
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPFornitoriSec()
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        WUC_FornitoreSec.SvuotaCampi()
        Session(F_FORNSEC_APERTA) = False
    End Sub

#End Region

#Region "Metodi public"

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_FornitoreSec.SvuotaCampi()
    End Sub

#End Region

End Class