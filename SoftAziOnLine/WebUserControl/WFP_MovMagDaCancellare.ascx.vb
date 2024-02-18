Imports SoftAziOnLine.Def

Partial Public Class WFP_MovMagDaCancellare
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
    'giu030512 
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUCMovMagDaCancellare.PopolaListaMovMagDaCanc()) Then
            Session(F_SCELTAMOVMAG_APERTA) = False
            lblMessUtente.Text = ""
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPMovMagDaCanc()
        Else
            lblMessUtente.Text = "Attenzione, nessun movimento è stato selezionato."
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_SCELTAMOVMAG_APERTA) = False
    End Sub

    Protected Sub btnSelTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        WUCMovMagDaCancellare.SelezionaTutti()
    End Sub

    Protected Sub btnDeselTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        WUCMovMagDaCancellare.DeselezionaTutti()
    End Sub

    'giu020512
    Protected Sub btnStampaTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUCMovMagDaCancellare.PopolaListaMovMagDaCanc()) Then
            Session(F_SCELTAMOVMAG_APERTA) = False
            lblMessUtente.Text = ""
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPMovMagDaStampare()
        Else
            lblMessUtente.Text = "Attenzione, nessun movimento è stato selezionato."
        End If
    End Sub
#End Region

#Region "Metodi public"

    Public Sub Show()
        If (Not IsPostBack) Then
            lblMessUtente.Text = "Seleziona i movimenti da Eliminare"
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub PopolaGridView()
        WUCMovMagDaCancellare.PopolaGridView()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub
#End Region

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            lblMessUtente.Text = "Seleziona i movimenti da Eliminare"
        End If
    End Sub

    
End Class