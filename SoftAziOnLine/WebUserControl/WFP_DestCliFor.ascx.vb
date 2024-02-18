Imports SoftAziOnLine.Def

Partial Public Class WFP_DestCliFor
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
    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUC_DestCliFor.AggiornaDestCliFor()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPDestCliFor()
            Session(F_DESTCLIFOR_APERTA) = False
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        WUC_DestCliFor.SvuotaCampi()
        Session(F_DESTCLIFOR_APERTA) = False
    End Sub

    Protected Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        WUC_DestCliFor.NewSvuotaCampi()
        btnNuovo.Enabled = False
    End Sub

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_DestCliFor.SvuotaCampi()
    End Sub

    Public Sub PopolaCampi()
        Dim pNuovo As String
        WUC_DestCliFor.PopolaCampi(pNuovo)
        If pNuovo = "S" Then
            btnNuovo.Enabled = False
        Else
            btnNuovo.Enabled = True
        End If
    End Sub

End Class