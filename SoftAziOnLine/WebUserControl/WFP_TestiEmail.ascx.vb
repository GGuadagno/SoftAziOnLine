Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_TestiEmail
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

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            lblMessUtente.Text = ""
        End If
        WUC_TestiEmail.WucElement = Me
    End Sub

    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNESSUNA
        If (WUC_TestiEmail.Aggiorna()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPTestiEmail()
            Session(F_GESTIONETESTIEMAIL_APERTA) = False
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNESSUNA
        ProgrammaticModalPopup.Hide()
        WUC_TestiEmail.SvuotaCampi()
        _WucElement.CancBackWFPTestiEmail()
        Session(F_GESTIONETESTIEMAIL_APERTA) = False
    End Sub

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_TestiEmail.SvuotaCampi()
    End Sub

    Public Sub SetlblMessaggi(ByVal strMessaggio As String)
        lblMessUtente.Text = strMessaggio
    End Sub
End Class