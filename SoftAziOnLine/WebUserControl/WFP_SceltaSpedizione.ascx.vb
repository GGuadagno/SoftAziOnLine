Imports SoftAziOnLine.Def
Partial Public Class WFP_SceltaSpedizione
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
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            lblMessUtente.Text = ""
        End If
        WUCSceltaSpedizione.WfpElement = Me
    End Sub

    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        If Not Session(IDSPEDIZIONESEL) Is Nothing Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPSceltaSped(Session(IDSPEDIZIONESEL))
            lblMessUtente.Text = ""
            Session(F_SCELTASPED_APERTA) = False
        Else
            lblMessUtente.Text = "Attenzione, nessuna spedizione è stata selezionata."
        End If

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_SCELTASPED_APERTA) = False
    End Sub

#End Region

#Region "Metodi public"

    Public Sub Show(Optional ByVal isFirst As Boolean = False)
        If isFirst Then
            lblMessUtente.Text = ""
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

#End Region

End Class