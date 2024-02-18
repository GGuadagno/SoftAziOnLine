Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_EvasioneOF
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
        Stato = 0
        NumDoc = 1
        RevN = 2
        DataDoc = 3
        DataCons = 4
        CodCliForProvv = 5
        RagSoc = 6
        '--.....
        DataVal = 12
        Riferimento = 13
    End Enum
    Private Enum CellIdxD
        Selez = 0
        QtaEvIns = 1
        Riga = 2
        CodArt = 3
        DesArt = 4
        UM = 5
        Qta = 6
        QtaEv = 7
        QtaRe = 8
        IVA = 9
        Prz = 10
        ScV = 11
        Sc1 = 12
        Importo = 13
        ScR = 14
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
            If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
                e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataVal).Text) Then
                e.Row.Cells(CellIdxT.DataVal).Text = Format(CDate(e.Row.Cells(CellIdxT.DataVal).Text), FormatoData).ToString
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
            If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaRe).Text = ""
                End If
            End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.IVA).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.IVA).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.IVA).Text), 0).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.IVA).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.Prz).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.Prz).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Prz).Text), App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.Prz).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.ScV).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.ScV).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScV).Text), 2).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.ScV).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.Sc1).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.Sc1).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Sc1).Text), App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.Sc1).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.Importo).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.Importo).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Importo).Text), 2).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.Importo).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.ScR).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.ScR).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScR).Text), 4).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.ScR).Text = ""
            ' ''    End If
            ' ''End If
        End If
    End Sub
#End Region

#Region "Metodi public"

    Public Function PopolaLista() As Boolean
        Dim ListaDocD As New List(Of String)
        For Each row As GridViewRow In GridViewDocD.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            Dim txtQtaEv As TextBox = CType(row.FindControl("txtQtaEv"), TextBox)
            If (checkSel.Checked) Then
                If txtQtaEv.Text <> "" And txtQtaEv.Text <> "0" Then
                    ListaDocD.Add(txtQtaEv.Text & "|" & row.Cells(2).Text)
                End If
            End If
        Next
        If (ListaDocD.Count > 0) Then
            PopolaLista = True
            Session(L_EVASIONEPARZ_DA_CAR) = ListaDocD
        Else
            PopolaLista = False
        End If
    End Function

    Public Sub SelezionaTutti()
        For Each row As GridViewRow In GridViewDocD.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = True
        Next
    End Sub

    Public Sub SelAndQtaTutti()
        For Each row As GridViewRow In GridViewDocD.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = True
            Dim QtaSel As TextBox = CType(row.FindControl("txtQtaEv"), TextBox)
            QtaSel.Text = row.Cells(CellIdxD.QtaRe).Text
        Next
    End Sub

    Public Sub DeselezionaTutti()
        For Each row As GridViewRow In GridViewDocD.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = False
        Next
    End Sub

#End Region

End Class