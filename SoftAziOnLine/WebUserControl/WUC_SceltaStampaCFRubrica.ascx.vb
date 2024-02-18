Imports SoftAziOnLine.Def
Partial Public Class WUC_SceltaStampaCFRubrica
    Inherits System.Web.UI.UserControl

    Private _WUCElement As Object
    Public WriteOnly Property WUCElement() As Object
        Set(ByVal value As Object)
            _WUCElement = value
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
        rbtnContatti.Visible = True
        rbtnDestinazioni.Visible = True
        rbtnNessuno.Visible = True
        If Session(CSTFinestraChiamante) = "Clienti" Then
            rbtnNessuno.Text = "Stampa anagrafica clienti"
        ElseIf Session(CSTFinestraChiamante) = "Fornitori" Then
            rbtnNessuno.Text = "Stampa anagrafica fornitori"
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Protected Sub OkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()

        If rbtnNessuno.Checked Then
            _WUCElement.StampaRubrica(0)
        ElseIf rbtnContatti.Checked Then
            _WUCElement.StampaRubrica(1)
        ElseIf rbtnDestinazioni.Checked Then
            _WUCElement.StampaRubrica(2)
        End If
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
    End Sub

    Public Sub Hide()
        ProgrammaticModalPopup.Hide()
    End Sub

End Class