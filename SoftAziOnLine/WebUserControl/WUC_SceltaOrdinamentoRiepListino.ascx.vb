Imports SoftAziOnLine.Def

Partial Public Class WUC_SceltaOrdinamentoRiepListino
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
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataSourceFor.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
    End Sub

    Public Sub Show()
        RadioButton1.Checked = True
        RadioButton2.Checked = False
        ProgrammaticModalPopup.Show()
    End Sub

    Protected Sub OkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Dim TipoOrdine As String = "Cod_Articolo"
        If RadioButton1.Checked = True Then TipoOrdine = "Cod_Articolo"
        If RadioButton2.Checked = True Then TipoOrdine = "Descrizione"
        _WucElement.CallBackTipoOrdine(TipoOrdine, ddlFornitori.SelectedValue.Trim, ddlFornitori.SelectedItem.Text.Trim)
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
    End Sub

    Public Sub Hide()
        ProgrammaticModalPopup.Hide()
    End Sub


End Class