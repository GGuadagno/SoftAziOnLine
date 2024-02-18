Imports SoftAziOnLine.Def

Partial Public Class WFP_AnagrProvv_Insert
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
        If (WUC_AnagrProvv_Insert.AggiornaAnagrProvv()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPAnagrProvv()
            Session(F_ANAGR_PROVV_APERTA) = False
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        WUC_AnagrProvv_Insert.SvuotaCampi()
        Session(F_ANAGR_PROVV_APERTA) = False
    End Sub

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Function PopolaDLL(ByRef strErrMess As String) As Boolean
        PopolaDLL = WUC_AnagrProvv_Insert.PopolaDLL(strErrMess)
    End Function
    Public Sub SvuotaCampi()
        WUC_AnagrProvv_Insert.SvuotaCampi()
    End Sub

    'giu150513
    Public Sub PopolaCampiModifica()
        WUC_AnagrProvv_Insert.PopolaCampiModifica()
    End Sub

    'Simone300317
    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

    Private Sub btnUsaAnagPresentiCF_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUsaAnagPresentiCF.Click
        WUC_AnagrProvv_Insert.PopolaPerCF()
    End Sub

    Private Sub btnUsaAnagPresentiRag_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUsaAnagPresentiRag.Click
        WUC_AnagrProvv_Insert.PopolaPerRag()
    End Sub

    Private Sub btnUsaAnagPresentiPIVA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUsaAnagPresentiPIVA.Click
        WUC_AnagrProvv_Insert.PopolaPerPIVA()
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            lblMessUtente.Text = ""
        End If
        WUC_AnagrProvv_Insert.WucElement = Me
    End Sub
    '---
End Class