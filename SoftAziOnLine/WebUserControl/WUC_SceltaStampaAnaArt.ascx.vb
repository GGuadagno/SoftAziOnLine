Imports SoftAziOnLine.Def
Partial Public Class WUC_SceltaStampaAnaArt
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

    Public Sub Show()
        rbCodice.Checked = True
        rbDescrizione.Checked = False
        rbSintetica.Checked = True
        rbArticoli.Checked = True
        ProgrammaticModalPopup.Show()
    End Sub

    Protected Sub OkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Dim TipoOrdine As String = "Cod_Articolo"
        Dim Analitico As Boolean = True
        Dim TuttiArticoli As Boolean = True
        Dim strDaCodice As String = ""
        Dim strACodice As String = ""

        If rbCodice.Checked = True Then TipoOrdine = "Cod_Articolo"
        If rbDescrizione.Checked = True Then TipoOrdine = "Descrizione"

        If rbAnalitica.Checked = True Then Analitico = True
        If rbSintetica.Checked = True Then Analitico = False

        If rbArticoli.Checked = True Then TuttiArticoli = True
        If rbDaCodiceAcodice.Checked = True Then TuttiArticoli = False
        
        If TuttiArticoli = False Then
            strDaCodice = txtDaCodice.Text
            strACodice = txtACodice.Text
        End If

        _WucElement.CallBackSceltaStampaAnaMag(TipoOrdine, Analitico, TuttiArticoli, strDaCodice, strACodice)
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
    End Sub

    Public Sub Hide()
        ProgrammaticModalPopup.Hide()
    End Sub

    
End Class