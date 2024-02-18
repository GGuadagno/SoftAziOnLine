Public Partial Class WUC_SceltaStampaUbiArt
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

    Public Sub Show(ByVal _Titolo As String)
        Label1.Text = _Titolo.Trim : Label1.Font.Bold = True
        rbCodice.Checked = True
        rbDescrizione.Checked = False
        rbRepScaPia.Checked = False
        ProgrammaticModalPopup.Show()
    End Sub

    Protected Sub OkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Dim TipoOrdine As String = "Cod_Articolo"

        If rbCodice.Checked = True Then TipoOrdine = "Cod_Articolo"
        If rbDescrizione.Checked = True Then TipoOrdine = "Descrizione"
        If rbRepScaPia.Checked = True Then TipoOrdine = "Reparto, Scaffale, Piano, Descrizione"

        _WucElement.CallBackSceltaStampaUbiMag(TipoOrdine)
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
    End Sub

    Public Sub Hide()
        ProgrammaticModalPopup.Hide()
    End Sub


End Class