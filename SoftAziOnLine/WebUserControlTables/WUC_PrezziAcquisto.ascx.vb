Imports SoftAziOnLine.Def

Partial Public Class WUC_PrezziAcquisto
    Inherits System.Web.UI.UserControl

#Region "Variabili private"

    Private _Enabled As Boolean

#End Region

#Region "Property"

    Property Enabled() As Boolean
        Get
            Return _Enabled
        End Get
        Set(ByVal value As Boolean)
            _Enabled = value
            GridViewBody.Enabled = value
        End Set
    End Property

#End Region

#Region "Metodi private - eventi"

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(1).Text) Then
                e.Row.Cells(1).Text = Format(CDate(e.Row.Cells(1).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(2).Text) Then
                e.Row.Cells(2).Text = Formatta.FormattaNumero(CDec(e.Row.Cells(2).Text), 2).ToString
            End If
        End If
    End Sub

    Private Sub GridViewBody_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewBody.Sorting
    End Sub

#End Region

#Region "Metodi public"

    Public Sub PopolaGridView()
        If (Not Session(COD_ARTICOLO) Is Nothing) Then
            SqlDataSourcePrezziAcq.ConnectionString = Session(DBCONNAZI)
            SqlDataSourcePrezziAcq.SelectParameters("Codice").DefaultValue = Session(COD_ARTICOLO)
            GridViewBody.DataSource = SqlDataSourcePrezziAcq
            GridViewBody.DataBind()
        End If
    End Sub

    Public Sub SvuotaGridView()
        SqlDataSourcePrezziAcq.ConnectionString = Session(DBCONNAZI)
        SqlDataSourcePrezziAcq.SelectParameters("Codice").DefaultValue = "0"
        GridViewBody.DataSource = SqlDataSourcePrezziAcq
        GridViewBody.DataBind()
    End Sub

#End Region

End Class