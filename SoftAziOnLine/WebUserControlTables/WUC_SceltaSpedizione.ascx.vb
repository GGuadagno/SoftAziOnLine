Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_SceltaSpedizione
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

    Private _WfpElement As Object
    Property WfpElement() As Object
        Get
            Return _WfpElement
        End Get
        Set(ByVal value As Object)
            _WfpElement = value
        End Set
    End Property
#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataSource.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        If Session(F_SCELTASPED_APERTA) Then
            GridViewBody.DataSourceID = "SqlDataSource"
        Else
            GridViewBody.DataSource = Nothing
            GridViewBody.DataSourceID = Nothing
        End If
    End Sub

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
    End Sub

    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Session(IDSPEDIZIONESEL) = GridViewBody.SelectedDataKey.Value
        WfpElement.SetLblMessUtente("")
    End Sub

#End Region

#Region "Metodi public"

    Public Sub Reset(ByVal nomeLista As String, ByVal titolo As String)
        GridViewBody.SelectedIndex = -1
        Session(IDSPEDIZIONESEL) = Nothing
    End Sub

#End Region


End Class