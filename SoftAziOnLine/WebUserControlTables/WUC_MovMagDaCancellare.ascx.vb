Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_MovMagDaCancellare
    Inherits System.Web.UI.UserControl

#Region "Variabili private"

    Private _WucElement As Object

#End Region

#Region "Property"

    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataSourceMovMagT.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
    End Sub

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
    End Sub

#End Region

#Region "Metodi public"

    Public Sub PopolaGridView()
        If (Not Session(REFINTMOVMAG) Is Nothing) Then
            GridViewBody.DataSource = SqlDataSourceMovMagT
            GridViewBody.DataBind()
        End If
    End Sub

    Public Sub SvuotaGridView()
        GridViewBody.DataSource = SqlDataSourceMovMagT
        GridViewBody.DataBind()
    End Sub

    Public Function PopolaListaMovMagDaCanc() As Boolean
        Dim listaMovMagDaCanc As New List(Of String)
        For Each row As GridViewRow In GridViewBody.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            If (checkSel.Checked) Then
                listaMovMagDaCanc.Add(row.Cells(1).Text)
            End If
        Next
        If (listaMovMagDaCanc.Count > 0) Then
            PopolaListaMovMagDaCanc = True
            Session(MOVMAG_DA_CANC) = listaMovMagDaCanc
        Else
            PopolaListaMovMagDaCanc = False
        End If
    End Function

    Public Sub SelezionaTutti()
        For Each row As GridViewRow In GridViewBody.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = True
        Next
    End Sub

    Public Sub DeselezionaTutti()
        For Each row As GridViewRow In GridViewBody.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = False
        Next
    End Sub

#End Region

End Class