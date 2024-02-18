Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_FornitoriSec
    Inherits System.Web.UI.UserControl

#Region "Costanti"

    Private Const SEL_CODICE_FORN As String = "FornitoriSec_SelCodice"

#End Region

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
            btnEliminaFornitoreSec.Enabled = value
        End Set
    End Property

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            CaricaVariabili()
        End If

        ModalPopup.WucElement = Me
    End Sub

    Private Sub GridViewBody_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewBody.Sorting
    End Sub

    Private Sub btnEliminaFornitoreSec_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEliminaFornitoreSec.Click
        If (Not String.IsNullOrEmpty(Session(SEL_CODICE_FORN))) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "ConfermaCancellazioneRiga"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Cancellazione riga", "Si vuole cancellare la riga selezionata?", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub

    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Session(SEL_CODICE_FORN) = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(1).Text
    End Sub

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
    End Sub

#End Region

#Region "Gestione variabili di sessione interne"

    Private Sub CaricaVariabili()
        Session(SEL_CODICE_FORN) = String.Empty
    End Sub
#End Region

#Region "Metodi public"

    Public Sub PopolaGridView()
        CaricaVariabili()
        If (Not Session(COD_ARTICOLO) Is Nothing) Then
            SqlDataSourceFornitoriSec.ConnectionString = Session(DBCONNAZI)
            SqlDataSourceFornitoriSec.SelectParameters("Codice").DefaultValue = Session(COD_ARTICOLO)
            GridViewBody.DataSource = SqlDataSourceFornitoriSec
            GridViewBody.DataBind()
        End If
    End Sub

    Public Sub SvuotaGridView()
        CaricaVariabili()
        SqlDataSourceFornitoriSec.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceFornitoriSec.SelectParameters("Codice").DefaultValue = "0"
        GridViewBody.DataSource = SqlDataSourceFornitoriSec
        GridViewBody.DataBind()
    End Sub

    Public Sub AggiungiRigaFornSec(ByVal myFornitoreSec As FornSecondariEntity)
        Dim arrFornSec As New ArrayList
        Dim flagFornitoreEsistente As Boolean = False

        If (GridViewBody.Rows.Count > 0) Then
            For Each row As GridViewRow In GridViewBody.Rows
                If (row.Cells(1).Text.Equals(myFornitoreSec.CodFornitore)) Then
                    flagFornitoreEsistente = True
                End If
                Dim myFornSec As New FornSecondariEntity
                myFornSec.CodFornitore = row.Cells(1).Text
                myFornSec.RagSoc = NormalizzaStringa(row.Cells(2).Text)
                myFornSec.GiorniConsegna = CInt(row.Cells(3).Text)
                myFornSec.CodPagamento = CInt(row.Cells(4).Text)
                myFornSec.UltPrezzo = CDec(row.Cells(5).Text)
                myFornSec.Titolare = NormalizzaStringa(row.Cells(6).Text)
                myFornSec.Riferimento = NormalizzaStringa(row.Cells(7).Text)
                arrFornSec.Add(myFornSec)
            Next
        End If
        If (Not flagFornitoreEsistente) Then
            arrFornSec.Add(myFornitoreSec)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Scelta fornitore secondario", "Fornitore già selezionato per questo articolo", WUC_ModalPopup.TYPE_INFO)
        End If
        GridViewBody.DataSource = arrFornSec
        GridViewBody.DataBind()
    End Sub

    Public Sub ConfermaCancellazioneRiga()
        Dim arrFornSec As New ArrayList
        Dim codiceCancRiga As String = Session(SEL_CODICE_FORN)
        Dim codice As Integer = 1
        Session(SWMODIFICATO) = True
        If (Not String.IsNullOrEmpty(codiceCancRiga)) Then
            If (GridViewBody.Rows.Count > 0) Then
                For Each row As GridViewRow In GridViewBody.Rows
                    If (Not row.Cells(1).Text.Equals(codiceCancRiga)) Then
                        Dim myFornSec As New FornSecondariEntity
                        myFornSec.CodFornitore = row.Cells(1).Text
                        myFornSec.RagSoc = NormalizzaStringa(row.Cells(2).Text)
                        myFornSec.GiorniConsegna = CInt(row.Cells(3).Text)
                        myFornSec.CodPagamento = CInt(row.Cells(4).Text)
                        myFornSec.UltPrezzo = CDec(row.Cells(5).Text)
                        myFornSec.Titolare = NormalizzaStringa(row.Cells(6).Text)
                        myFornSec.Riferimento = NormalizzaStringa(row.Cells(7).Text)
                        arrFornSec.Add(myFornSec)
                    End If
                Next
            End If
            GridViewBody.DataSource = arrFornSec
            GridViewBody.DataBind()
        End If
    End Sub

    Public Function GetListFornitoriSec() As List(Of FornSecondariEntity)
        Dim myListFornSec As New List(Of FornSecondariEntity)

        For Each row As GridViewRow In GridViewBody.Rows
            Dim myFornSec As New FornSecondariEntity
            myFornSec.CodArticolo = Session(COD_ARTICOLO)
            myFornSec.CodFornitore = row.Cells(1).Text
            myFornSec.RagSoc = NormalizzaStringa(row.Cells(2).Text)
            myFornSec.GiorniConsegna = CInt(row.Cells(3).Text)
            myFornSec.CodPagamento = CInt(row.Cells(4).Text)
            myFornSec.UltPrezzo = CDec(row.Cells(5).Text)
            myFornSec.Titolare = NormalizzaStringa(row.Cells(6).Text)
            myFornSec.Riferimento = NormalizzaStringa(row.Cells(7).Text)
            myListFornSec.Add(myFornSec)
        Next

        GetListFornitoriSec = myListFornSec
    End Function

#End Region

End Class