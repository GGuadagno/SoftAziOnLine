Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_ElencoDestCF
    Inherits System.Web.UI.UserControl


    Private Const FILTRO_ATTIVO As String = "FiltroAttivo"
    Private Const LISTA As String = "NomeLista"
    Private Const DESC_TITOLO As String = "Desc_Titolo"
    Private SWPrima As Boolean = True
    'GIU101120 GESTIONE SORT DELLA GIGLIA POPUP
    Private DVGridView As DataView

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
    Private Enum CellIdxT
        Tipo = 1
        Rag_Soc = 2
        Denominazione = 3
        Riferimento = 4
        Cap = 5
        Localita = 6
        Indirizzo = 7
        Email = 8
    End Enum
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            SWPrima = True
            Try
                ddlRicerca.Items.Clear()
                ddlRicerca.Items.Add("")
                ddlRicerca.Items(0).Value = ""
                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(1).Value = "Rag_Soc"
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(2).Value = "Denominazione"
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(3).Value = "CAP"
                ddlRicerca.Items.Add("Indirizzo")
                ddlRicerca.Items(4).Value = "Indirizzo"
                ddlRicerca.Items.Add("Email")
                ddlRicerca.Items(5).Value = "Email"
                ddlRicerca.AutoPostBack = False
                ddlRicerca.SelectedIndex = 1
                txtLocalita.Enabled = True
                ddlRicerca.AutoPostBack = True
                checkParoleContenute.Checked = True
                txtRicerca.Focus()
            Catch ex As Exception

            End Try
        End If
        'ok
        Select Case ddlRicerca.SelectedValue
            Case "Indirizzo"
                txtLocalita.Enabled = True
            Case "Rag_Soc"
                txtLocalita.Enabled = True
            Case "Denominazione"
                txtLocalita.Enabled = True
            Case Else
                txtLocalita.Text = ""
                txtLocalita.Enabled = False
        End Select
        If (Session(F_ELENCO_CLIFORN_APERTA)) And SWPrima = True Then
            SWPrima = False
            CaricaDati()
        End If
    End Sub

    Private Sub GridViewBody_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewBody.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewBody.PageIndex = 0
        Else
            GridViewBody.PageIndex = e.NewPageIndex
        End If
        'giu101120
        ' ''GridViewBody.SelectedIndex = -1
        ' ''Session(RTN_VALUE_F_ELENCO) = Nothing
        ' ''WfpElement.SetLblMessUtente("")
        ' ''CaricaDati()
        Session(RTN_VALUE_F_ELENCO) = Nothing
        WfpElement.SetLblMessUtente("")
        If (Session(GRIDVIEWDESTCF) Is Nothing) Then
            CaricaDati()
        End If
        '-
        Try
            GridViewBody.SelectedIndex = -1
            DVGridView = Session(GRIDVIEWDESTCF)
            GridViewBody.DataSource = DVGridView
            GridViewBody.DataBind()
        Catch ex As Exception
            Exit Sub
        End Try
        SetRicerca()
        GridViewBody.DataBind()
        If GridViewBody.Rows.Count > 0 Then
            GridViewBody.SelectedIndex = 0
            Try
                GridViewBody_SelectedIndexChanged(GridViewBody, Nothing)
            Catch ex As Exception
                Session(RTN_VALUE_F_ELENCO) = Nothing
                WfpElement.SetLblMessUtente("")
            End Try
        Else
            Session(RTN_VALUE_F_ELENCO) = Nothing
            WfpElement.SetLblMessUtente("")
        End If
    End Sub

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
    End Sub

    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Dim rtn As New STR_RETURN_VALUE
        rtn.Codice = String.Format("{0}", GridViewBody.SelectedDataKey.Value)
        Dim i As Integer = -1 : Dim myRowIndex As Integer = -1
        Try
            Dim row As GridViewRow = GridViewBody.SelectedRow
            rtn.Descrizione = row.Cells(CellIdxT.Rag_Soc).Text.Trim
        Catch ex As Exception
            rtn.Descrizione = "Errore in seleziona riga: " & ex.Message
        End Try
        Session(RTN_VALUE_F_ELENCO) = rtn
        WfpElement.SetLblMessUtente("Selezionato: " & rtn.Codice & " - " & rtn.Descrizione.Trim)
    End Sub
    'giu101120
    Private Sub GridViewBody_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.Sorted
        GridViewBody.SelectedIndex = -1
        '-
        DVGridView = Session(GRIDVIEWDESTCF)
        GridViewBody.DataSource = DVGridView
        GridViewBody.DataBind()
        '-
        SetRicerca()
        GridViewBody.DataBind()
        If GridViewBody.Rows.Count > 0 Then
            GridViewBody.SelectedIndex = 0
            Try
                GridViewBody_SelectedIndexChanged(GridViewBody, Nothing)
            Catch ex As Exception
                Session(RTN_VALUE_F_ELENCO) = Nothing
                WfpElement.SetLblMessUtente("")
            End Try
        Else
            Session(RTN_VALUE_F_ELENCO) = Nothing
            WfpElement.SetLblMessUtente("")
        End If
    End Sub
    Private Sub GridViewBody_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewBody.Sorting
        Dim sortExpression As String = TryCast(ViewState("_GridView1LastSortExpression_"), String)
        Dim sortDirection As String = TryCast(ViewState("_GridView1LastSortDirection_"), String)

        If e.SortExpression <> sortExpression Then
            sortExpression = e.SortExpression
            sortDirection = "ASC"
        Else

            If sortDirection = "ASC" Then
                sortExpression = e.SortExpression
                sortDirection = "DESC"
            Else
                sortExpression = e.SortExpression
                sortDirection = "ASC"
            End If
        End If

        Try
            ViewState("_GridView1LastSortDirection_") = sortDirection
            ViewState("_GridView1LastSortExpression_") = sortExpression
            DVGridView = Session(GRIDVIEWDESTCF)
            DVGridView.Sort = ""
            If DVGridView.Count > 0 Then DVGridView.Sort = sortExpression & " " + sortDirection
            GridViewBody.DataSource = DVGridView
            GridViewBody.DataBind()
        Catch
            Session(RTN_VALUE_F_ELENCO) = Nothing
            WfpElement.SetLblMessUtente("")
        End Try
    End Sub

    Private Sub btnRicerca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicerca.Click
        CaricaDati(True)
    End Sub

    Private Sub ddlRicerca_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.TextChanged
        If ddlRicerca.SelectedValue.Equals("Indirizzo") Then
            txtLocalita.Enabled = True
        ElseIf ddlRicerca.SelectedValue.Equals("Rag_Soc") Then
            txtLocalita.Enabled = True
        ElseIf ddlRicerca.SelectedValue.Equals("Denominazione") Then
            txtLocalita.Enabled = True
        Else
            txtLocalita.Text = String.Empty
            txtLocalita.Enabled = False
        End If
        txtRicerca.Focus()
    End Sub

#End Region

#Region "Gestione variabili di sessione interne"

    Private Sub CaricaVariabili()
        Session(FILTRO_ATTIVO) = False
        Session(LISTA) = String.Empty
        Session(DESC_TITOLO) = String.Empty
    End Sub

#End Region

#Region "Metodi public"

    Public Sub Reset(ByVal nomeLista As String, ByVal titolo As String)
        txtRicerca.Text = String.Empty
        txtLocalita.Text = String.Empty
        GridViewBody.SelectedIndex = -1
        Session(RTN_VALUE_F_ELENCO) = Nothing
        Session(FILTRO_ATTIVO) = False
        CaricaVariabili()
        Session(LISTA) = nomeLista
        Session(DESC_TITOLO) = titolo
        CaricaDati()
    End Sub

#End Region

#Region "Metodi private"

    Private Sub CaricaDati(Optional ByVal _Ricerca As Boolean = False)
        Dim arrTabella As ArrayList = Nothing
        Dim arrResFiltro As ArrayList = Nothing
        Dim res = Nothing
        Dim strErrore As String = ""
        lbTitolo.Text = Session(DESC_TITOLO)
        If Not IsNothing(Session(CGDESTCLIFOR)) Then
            If Not String.IsNullOrEmpty(Session(CGDESTCLIFOR)) Then
                If Session(CGDESTCLIFOR) = Session(CSTCODCOGE) Then
                    If Not (Session(DESTCLIFOR) Is Nothing) And Not (Session(GRIDVIEWDESTCF) Is Nothing) Then
                        arrTabella = Session(DESTCLIFOR)
                        DVGridView = Session(GRIDVIEWDESTCF)
                    Else
                        arrTabella = App.GetDatiDestCliFor(Session(CSTCODCOGE), strErrore, DVGridView)
                        If strErrore.Trim <> "" Then
                            Session(DESTCLIFOR) = Nothing
                            Session(CGDESTCLIFOR) = Nothing
                            WfpElement.SetLblMessUtente(strErrore)
                            Exit Sub
                        End If
                        Session(DESTCLIFOR) = arrTabella
                        Session(CGDESTCLIFOR) = Session(CSTCODCOGE)
                    End If
                Else
                    arrTabella = App.GetDatiDestCliFor(Session(CSTCODCOGE), strErrore, DVGridView)
                    If strErrore.Trim <> "" Then
                        Session(DESTCLIFOR) = Nothing
                        Session(CGDESTCLIFOR) = Nothing
                        WfpElement.SetLblMessUtente(strErrore)
                        Exit Sub
                    End If
                    Session(DESTCLIFOR) = arrTabella
                    Session(CGDESTCLIFOR) = Session(CSTCODCOGE)
                End If
            Else
                arrTabella = App.GetDatiDestCliFor(Session(CSTCODCOGE), strErrore, DVGridView)
                If strErrore.Trim <> "" Then
                    Session(DESTCLIFOR) = Nothing
                    Session(CGDESTCLIFOR) = Nothing
                    WfpElement.SetLblMessUtente(strErrore)
                    Exit Sub
                End If
                Session(DESTCLIFOR) = arrTabella
                Session(CGDESTCLIFOR) = Session(CSTCODCOGE)
            End If
        Else
            arrTabella = App.GetDatiDestCliFor(Session(CSTCODCOGE), strErrore, DVGridView)
            If strErrore.Trim <> "" Then
                Session(DESTCLIFOR) = Nothing
                Session(CGDESTCLIFOR) = Nothing
                WfpElement.SetLblMessUtente(strErrore)
                Exit Sub
            End If
            Session(DESTCLIFOR) = arrTabella
            Session(CGDESTCLIFOR) = Session(CSTCODCOGE)
        End If
        'OK
        'giu101120
        GridViewBody.DataSource = DVGridView
        Session(GRIDVIEWDESTCF) = DVGridView
        If (DVGridView Is Nothing) Then
            GridViewBody.DataBind()
            WfpElement.SetLblMessUtente("!!SESSIONE SCADUTA!!")
            Exit Sub
        End If
        SetRicerca()
        '-end giu101120
        '@@@@@@@@@
        ' ''If (Session(DESTCLIFOR) Is Nothing) Then
        ' ''    GridViewBody.DataBind()
        ' ''    WfpElement.SetLblMessUtente("!!SESSIONE SCADUTA!!")
        ' ''    Exit Sub
        ' ''End If
        ' ''If String.IsNullOrEmpty(txtRicerca.Text.Trim) And String.IsNullOrEmpty(txtLocalita.Text.Trim) Then
        ' ''    GridViewBody.DataSource = arrTabella
        ' ''    Session(FILTRO_ATTIVO) = False
        ' ''ElseIf (checkParoleContenute.Checked) Then
        ' ''    Select Case ddlRicerca.SelectedValue
        ' ''        Case "Rag_Soc"
        ' ''            If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
        ' ''                res = From x In arrTabella Where x.Ragione_Sociale.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper()) And _
        ' ''                    x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.ToUpper())
        ' ''            ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
        ' ''                res = From x In arrTabella Where x.Ragione_Sociale.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
        ' ''            ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
        ' ''                res = From x In arrTabella Where x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.Trim.ToUpper())
        ' ''            End If
        ' ''        Case "Denominazione"
        ' ''            If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
        ' ''                res = From x In arrTabella Where x.Denominazione.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper()) And _
        ' ''                    x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.ToUpper())
        ' ''            ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
        ' ''                res = From x In arrTabella Where x.Denominazione.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
        ' ''            ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
        ' ''                res = From x In arrTabella Where x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.Trim.ToUpper())
        ' ''            End If
        ' ''        Case "Cap"
        ' ''            res = From x In arrTabella Where x.Cap.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
        ' ''        Case "Indirizzo"
        ' ''            If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
        ' ''                res = From x In arrTabella Where x.Indirizzo.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper()) And _
        ' ''                    x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.ToUpper())
        ' ''            ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
        ' ''                res = From x In arrTabella Where x.Indirizzo.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
        ' ''            ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
        ' ''                res = From x In arrTabella Where x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.Trim.ToUpper())
        ' ''            End If
        ' ''        Case "Email"
        ' ''            res = From x In arrTabella Where x.Email.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
        ' ''    End Select
        ' ''    If (Not res Is Nothing) Then
        ' ''        arrResFiltro = New ArrayList()
        ' ''        For Each el As DestCliForEntity In res
        ' ''            arrResFiltro.Add(el)
        ' ''        Next
        ' ''    End If
        ' ''    GridViewBody.DataSource = arrResFiltro
        ' ''    Session(FILTRO_ATTIVO) = True
        ' ''Else
        ' ''    Select Case ddlRicerca.SelectedValue
        ' ''        Case "Rag_Soc"
        ' ''            res = From x In arrTabella Where x.Ragione_Sociale.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
        ' ''        Case "Denominazione"
        ' ''            res = From x In arrTabella Where x.Denominazione.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
        ' ''        Case "Cap"
        ' ''            res = From x In arrTabella Where x.Cap.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
        ' ''        Case "Indirizzo"
        ' ''            If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
        ' ''                res = From x In arrTabella Where x.Indirizzo.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper()) And _
        ' ''                    x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.ToUpper())
        ' ''            ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
        ' ''                res = From x In arrTabella Where x.Indirizzo.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
        ' ''            ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
        ' ''                res = From x In arrTabella Where x.Localita.ToString.ToUpper().StartsWith(txtLocalita.Text.Trim.ToUpper())
        ' ''            End If
        ' ''        Case "Email"
        ' ''            res = From x In arrTabella Where x.Email.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
        ' ''    End Select
        ' ''    If (Not res Is Nothing) Then
        ' ''        arrResFiltro = New ArrayList()
        ' ''        For Each el As DestCliForEntity In res
        ' ''            arrResFiltro.Add(el)
        ' ''        Next
        ' ''    End If
        ' ''    GridViewBody.DataSource = arrResFiltro
        ' ''    Session(FILTRO_ATTIVO) = True
        ' ''End If
        GridViewBody.DataBind()
        WfpElement.SetLblMessUtente("")
        If _Ricerca Then 'giu280819
            If GridViewBody.Rows.Count > 0 Then
                GridViewBody.SelectedIndex = 0
                GridViewBody_SelectedIndexChanged(Nothing, Nothing)
            Else
                GridViewBody.SelectedIndex = -1
            End If
        End If

    End Sub
    Private Sub SetRicerca()
        DVGridView.RowFilter = ""
        If String.IsNullOrEmpty(txtRicerca.Text.Trim) And String.IsNullOrEmpty(txtLocalita.Text.Trim) Then
            GridViewBody.DataSource = DVGridView
            Session(FILTRO_ATTIVO) = False
        ElseIf (checkParoleContenute.Checked) Then
            Select Case ddlRicerca.SelectedValue
                Case "Rag_Soc"
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Ragione_Sociale like '%" + Controlla_Apice(txtRicerca.Text.Trim) + "%' And " + _
                        "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        DVGridView.RowFilter = "Ragione_Sociale like '%" + Controlla_Apice(txtRicerca.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    End If
                Case "Denominazione"
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Denominazione like '%" + Controlla_Apice(txtRicerca.Text.Trim) + "%' And " + _
                        "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        DVGridView.RowFilter = "Denominazione like '%" + Controlla_Apice(txtRicerca.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    End If
                Case "Cap"
                    DVGridView.RowFilter = "Cap like '%" + Controlla_Apice(txtRicerca.Text.Trim) + "%'"
                Case "Indirizzo"
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Indirizzo like '%" + Controlla_Apice(txtRicerca.Text.Trim) + "%' And " + _
                        "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        DVGridView.RowFilter = "Indirizzo like '%" + Controlla_Apice(txtRicerca.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    End If
                Case "Email"
                    DVGridView.RowFilter = "Email like '%" + Controlla_Apice(txtRicerca.Text.Trim) + "%'"
            End Select
            GridViewBody.DataSource = DVGridView
            Session(FILTRO_ATTIVO) = True
        Else
            Select Case ddlRicerca.SelectedValue
                Case "Rag_Soc"
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Ragione_Sociale >='" + Controlla_Apice(txtRicerca.Text.Trim) + "' And " + _
                        "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        DVGridView.RowFilter = "Ragione_Sociale >='" + Controlla_Apice(txtRicerca.Text.Trim) + "'"
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Localita >='" + Controlla_Apice(txtLocalita.Text.Trim) + "'"
                    End If
                Case "Denominazione"
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Denominazione >='" + Controlla_Apice(txtRicerca.Text.Trim) + "' And " + _
                        "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        DVGridView.RowFilter = "Denominazione >='" + Controlla_Apice(txtRicerca.Text.Trim) + "'"
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Localita >='" + Controlla_Apice(txtLocalita.Text.Trim) + "'"
                    End If
                Case "Cap"
                    DVGridView.RowFilter = "Cap ='" + Controlla_Apice(txtRicerca.Text.Trim) + "'"
                Case "Indirizzo"
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Indirizzo >='" + Controlla_Apice(txtRicerca.Text.Trim) + "' And " + _
                        "Localita like '%" + Controlla_Apice(txtLocalita.Text.Trim) + "%'"
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        DVGridView.RowFilter = "Indirizzo >='" + Controlla_Apice(txtRicerca.Text.Trim) + "'"
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        DVGridView.RowFilter = "Localita >='" + Controlla_Apice(txtLocalita.Text.Trim) + "'"
                    End If
                Case "Email"
                    DVGridView.RowFilter = "Email >='" + Controlla_Apice(txtRicerca.Text.Trim) + "'"
            End Select
            GridViewBody.DataSource = DVGridView
            Session(FILTRO_ATTIVO) = True
        End If
    End Sub
#End Region

End Class