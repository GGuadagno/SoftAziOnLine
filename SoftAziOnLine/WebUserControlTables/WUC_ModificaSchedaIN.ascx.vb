Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_ModificaSchedaIN
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
    Private Enum CellIdxT
        NumDoc = 0
        RevN = 1
        DataDoc = 2
        InseritoDa = 3
        ModificatoDa = 4
    End Enum
    Private Enum CellIdxD
        Riga = 0
        CodArt = 1
        DesArt = 2
        UM = 3
        Qta = 4
        QtaEvIns = 5
        Spazi = 6
        QtaEv = 7
        IVA = 8
        Prz = 9
    End Enum
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSDocT.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSDocD.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
    End Sub
    Private Sub GridViewDocT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDocT.RowDataBound
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewDocT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
        End If
    End Sub
    Private Sub GridViewDocD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDocD.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewDocD, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEvIns).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEvIns).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEvIns).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEvIns).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEvIns).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEv).Text = ""
                End If
            End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), 0).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.QtaRe).Text = ""
            ' ''    End If
            ' ''End If
            If IsNumeric(e.Row.Cells(CellIdxD.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxD.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Prz).Text), App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxD.Prz).Text = ""
                End If
            End If
        End If
    End Sub
#End Region

#Region "Metodi public"

    Public Function PopolaLista() As Boolean
        Dim ListaDocD As New List(Of String)
        Dim Giacenza As String = ""
        Dim txtQtaEv As TextBox
        For Each row As GridViewRow In GridViewDocD.Rows
            Giacenza = row.Cells(CellIdxD.Qta).Text.Trim
            txtQtaEv = CType(row.FindControl("txtQtaEv"), TextBox)
            If Not IsNumeric(Giacenza.Trim) Then Giacenza = "0"
            If Not IsNumeric(txtQtaEv.Text.Trim) Then txtQtaEv.Text = "0"
            If Giacenza.Trim <> txtQtaEv.Text.Trim Then
                ListaDocD.Add(txtQtaEv.Text & "|" & row.Cells(CellIdxD.Riga).Text)
            End If
        Next
        If (ListaDocD.Count > 0) Then
            PopolaLista = True
            Session(L_SCHEDAIN_DA_AGG) = ListaDocD
        Else
            PopolaLista = False
        End If
    End Function

    Public Sub SelAndQtaTutti()
        SqlDSDocD.DataBind()
        GridViewDocD.DataBind()
        For Each row As GridViewRow In GridViewDocD.Rows
            Dim QtaSel As TextBox = CType(row.FindControl("txtQtaEv"), TextBox)
            QtaSel.Text = row.Cells(CellIdxD.QtaEv).Text
        Next
    End Sub

#End Region

End Class