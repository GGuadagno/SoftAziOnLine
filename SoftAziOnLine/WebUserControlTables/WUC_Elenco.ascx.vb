Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_Elenco
    Inherits System.Web.UI.UserControl

#Region "Costanti"

    Private Const FILTRO_ATTIVO As String = "FiltroAttivo"
    Private Const DESC_TITOLO As String = "Desc_Titolo"

#End Region

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
    Private _Tabella As String = String.Empty
    Property Tabella() As String
        Get
            Return _Tabella
        End Get
        Set(ByVal value As String)
            _Tabella = value
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

    Dim NoMess As Boolean = True

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            NoMess = True
            Try
                ddlRicerca.Items.Clear()
                ddlRicerca.Items.Add("Codice")
                ddlRicerca.Items(0).Value = "Codice"
                ddlRicerca.Items.Add("Descrizione")
                ddlRicerca.Items(1).Value = "Descrizione"

                If _Tabella = Def.CATEGORIE Then
                    ddlRicerca.Items.Add("Si/No Invio E-mail scadenze")
                    ddlRicerca.Items(2).Value = "I"

                    ddlRicerca.Items.Add("Si/No Selezione multipla per invio E-mail scadenze")
                    ddlRicerca.Items(3).Value = "S"
                    'grid
                    Dim nameColumn0 As New BoundField
                    nameColumn0.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                    nameColumn0.HeaderText = "Invio E-mail"
                    nameColumn0.DataField = "InvioMailSc"
                    'nameColumn0.DataFormatString = "{0:###}"
                    nameColumn0.ItemStyle.HorizontalAlign = HorizontalAlign.Center
                    nameColumn0.ItemStyle.Wrap = False
                    nameColumn0.HeaderStyle.Width = Unit.Pixel(25)
                    nameColumn0.ItemStyle.Width = Unit.Pixel(25)
                    nameColumn0.ReadOnly = True
                    GridViewBody.Columns.Add(nameColumn0)

                    Dim nameColumn1 As New BoundField
                    nameColumn1.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                    nameColumn1.HeaderStyle.Wrap = False
                    nameColumn1.HeaderText = "Selezione"
                    nameColumn1.DataField = "SelSc"
                    nameColumn1.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                    nameColumn1.ItemStyle.Wrap = False
                    nameColumn1.HeaderStyle.Width = Unit.Pixel(25)
                    nameColumn1.ItemStyle.Width = Unit.Pixel(25)
                    nameColumn1.ReadOnly = True 'giu110112
                    GridViewBody.Columns.Add(nameColumn1)
                End If

            Catch ex As Exception

            End Try
        End If
        If (Session(FILTRO_ATTIVO)) Then
            Exit Sub
        End If
    End Sub

    Private Sub GridViewBody_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewBody.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewBody.PageIndex = 0
        Else
            GridViewBody.PageIndex = e.NewPageIndex
        End If
        GridViewBody.SelectedIndex = -1
        Session(RTN_VALUE_F_ELENCO) = Nothing
        WfpElement.SetLblMessUtente("")
        Filtra()

    End Sub

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        'non ATTIVARE ALTRIMENTI NON FUNGE IL CAMBIO PAGINA
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 And _Tabella = Def.CATEGORIE Then
            If e.Row.Cells(3).Text.Trim = "False" Then
                e.Row.Cells(3).Text = "No"
            Else
                e.Row.Cells(3).Text = "Si"
            End If
            If e.Row.Cells(4).Text.Trim = "False" Then
                e.Row.Cells(4).Text = "No"
            Else
                e.Row.Cells(4).Text = "Si"
            End If
            'If IsDate(e.Row.Cells(CellIdxT.DataScadEl).Text) Then
            '    e.Row.Cells(CellIdxT.DataScadEl).Text = Format(CDate(e.Row.Cells(CellIdxT.DataScadEl).Text), FormatoData).ToString
            'End If
            'If IsDate(e.Row.Cells(CellIdxT.DataScadBa).Text) Then
            '    e.Row.Cells(CellIdxT.DataScadBa).Text = Format(CDate(e.Row.Cells(CellIdxT.DataScadBa).Text), FormatoData).ToString
            'End If
            'If IsDate(e.Row.Cells(CellIdxT.DataInvio).Text) Then
            '    e.Row.Cells(CellIdxT.DataInvio).Text = Format(CDate(e.Row.Cells(CellIdxT.DataInvio).Text), FormatoData).ToString
            'End If
            'If IsNumeric(e.Row.Cells(CellIdxT.NReInvio).Text) Then
            '    e.Row.Cells(CellIdxT.NReInvio).Text = Format(Val(e.Row.Cells(CellIdxT.NReInvio).Text), "###,###").ToString
            'End If
        End If
    End Sub

    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Try
            Dim rtn As New STR_RETURN_VALUE
            rtn.Codice = String.Format("{0}", GridViewBody.SelectedDataKey.Value)
            rtn.Descrizione = App.GetValoreFromChiave(rtn.Codice, _Tabella, Session(ESERCIZIO))
            ' ''rtn.Descrizione = GridViewBody.SelectedRow.Cells(2).Text
            Session(RTN_VALUE_F_ELENCO) = rtn
            WfpElement.SetLblMessUtente("")
            If PanelCategorie.Visible = True Then
                chkInvioMailSc.Enabled = True
                chkSelSc.Enabled = True
                If LeggiCategorie() = True Then
                    NoMess = False
                End If
            End If
        Catch ex As Exception
            chkInvioMailSc.Enabled = False
            chkSelSc.Checked = False
        End Try
        
    End Sub

    Private Sub btnRicerca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicerca.Click
        Filtra()
    End Sub

#End Region

#Region "Gestione variabili di sessione interne"

    Private Sub CaricaVariabili()
        Session(FILTRO_ATTIVO) = False
        Session(DESC_TITOLO) = String.Empty
    End Sub

#End Region

#Region "Metodi public"

    Public Sub Reset(ByVal titolo As String)
        txtRicerca.Text = String.Empty
        GridViewBody.SelectedIndex = -1
        Session(RTN_VALUE_F_ELENCO) = Nothing
        CaricaVariabili()
        Session(DESC_TITOLO) = titolo
        Filtra()
    End Sub

    Public Sub SetPanelCategorie(ByVal Visibile As Boolean)
        PanelCategorie.Visible = Visibile
    End Sub
#End Region

#Region "Metodi private"

    Private Sub Filtra()
        Dim arrTabella As ArrayList = Nothing
        Dim arrResFiltro As ArrayList = Nothing
        Dim res = Nothing
        Dim myBool As String = ""

        lbTitolo.Text = Session(DESC_TITOLO)
        'giu270312 ATTENZIONE ATTENZIONE SE una tabella riceve da ENTITY PIU CAMPI OLTRE AI CAMPI
        'CODICE E DESCRIZIONE RICHIAMARE SEMPRE LA FUNZIONE IN : arrTabella = App.GetLista(_Tabella, Session(ESERCIZIO))
        'arrLista = AdattaTabellaToTemplate(arrLista) CHE RITORNA L'ARRAYLIST CON I SOLI CAMPI
        'CODICE E DESCRIZIONE PER IL GRID GENERICO
        arrTabella = App.GetLista(_Tabella, Session(ESERCIZIO))
        If String.IsNullOrEmpty(txtRicerca.Text.Trim) Then
            GridViewBody.DataSource = arrTabella
            Session(FILTRO_ATTIVO) = False
        ElseIf (checkParoleContenute.Checked) Then
            Select Case ddlRicerca.SelectedValue
                Case "Codice"
                    res = From x In arrTabella Where x.Codice.ToString.ToUpper().Contains(txtRicerca.Text.ToUpper())
                Case "Descrizione"
                    res = From x In arrTabella Where x.Descrizione.ToString.ToUpper().Contains(txtRicerca.Text.ToUpper())
                Case "I"
                    If txtRicerca.Text.Trim.ToUpper = "SI" Or txtRicerca.Text.Trim.ToUpper = "S" Then
                        myBool = "True"
                    ElseIf txtRicerca.Text.Trim.ToUpper = "NO" Or txtRicerca.Text.Trim.ToUpper = "N" Then
                        myBool = "False"
                    Else
                        myBool = txtRicerca.Text.Trim
                    End If
                    res = From x In arrTabella Where x.InvioMailSc.ToString.ToUpper().Contains(myBool.ToUpper())
                Case "S"
                    If txtRicerca.Text.Trim.ToUpper = "SI" Or txtRicerca.Text.Trim.ToUpper = "S" Then
                        myBool = "True"
                    ElseIf txtRicerca.Text.Trim.ToUpper = "NO" Or txtRicerca.Text.Trim.ToUpper = "N" Then
                        myBool = "False"
                    Else
                        myBool = txtRicerca.Text.Trim
                    End If
                    res = From x In arrTabella Where x.SelSc.ToString.ToUpper().Contains(myBool.ToUpper())
            End Select
            If (Not res Is Nothing) Then
                arrResFiltro = New ArrayList()
                If _Tabella = Def.CATEGORIE Then
                    For Each El As TemplateCategorie In res
                        arrResFiltro.Add(El)
                    Next
                Else
                    For Each El As TemplateGridViewGenerica In res
                        arrResFiltro.Add(El)
                    Next
                End If
            End If
            GridViewBody.DataSource = arrResFiltro
            Session(FILTRO_ATTIVO) = True
        Else
            Select Case ddlRicerca.SelectedValue
                Case "Codice"
                    res = From x In arrTabella Where x.Codice.ToString.ToUpper().StartsWith(txtRicerca.Text.ToUpper())
                Case "Descrizione"
                    res = From x In arrTabella Where x.Descrizione.ToString.ToUpper().StartsWith(txtRicerca.Text.ToUpper())
                Case "I"
                    If txtRicerca.Text.Trim.ToUpper = "SI" Or txtRicerca.Text.Trim.ToUpper = "S" Then
                        myBool = "True"
                    ElseIf txtRicerca.Text.Trim.ToUpper = "NO" Or txtRicerca.Text.Trim.ToUpper = "N" Then
                        myBool = "False"
                    Else
                        myBool = txtRicerca.Text.Trim
                    End If
                    res = From x In arrTabella Where x.InvioMailSc.ToString.ToUpper().StartsWith(myBool.ToUpper())
                Case "S"
                    If txtRicerca.Text.Trim.ToUpper = "SI" Or txtRicerca.Text.Trim.ToUpper = "S" Then
                        myBool = "True"
                    ElseIf txtRicerca.Text.Trim.ToUpper = "NO" Or txtRicerca.Text.Trim.ToUpper = "N" Then
                        myBool = "False"
                    Else
                        myBool = txtRicerca.Text.Trim
                    End If
                    res = From x In arrTabella Where x.SelSc.ToString.ToUpper().StartsWith(myBool.ToUpper())
            End Select
            If (Not res Is Nothing) Then
                arrResFiltro = New ArrayList()
                If _Tabella = Def.CATEGORIE Then
                    For Each El As TemplateCategorie In res
                        arrResFiltro.Add(El)
                    Next
                Else
                    For Each El As TemplateGridViewGenerica In res
                        arrResFiltro.Add(El)
                    Next
                End If
            End If
            GridViewBody.DataSource = arrResFiltro
            Session(FILTRO_ATTIVO) = True
        End If
        GridViewBody.DataBind()
        GridViewBody.SelectedIndex = -1
        lblCategorie.Text = "MODIFICA DATI CATEGORIA CLIENTI"
        lblCategorie.BackColor = SEGNALA_OK
        Call GridViewBody_SelectedIndexChanged(GridViewBody, Nothing)
        Call LeggiCategorie()
    End Sub

#End Region

    Private Function LeggiCategorie() As Boolean
        If _Tabella <> Def.CATEGORIE Then
            LeggiCategorie = True
            Exit Function
        End If
        LeggiCategorie = False
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            strSQL = "SELECT * FROM Categorie WHERE Codice = " & String.Format("{0}", GridViewBody.SelectedDataKey.Value) & ""
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("InvioMailSc")) Then
                        chkInvioMailSc.Checked = CBool(ds.Tables(0).Rows(0).Item("InvioMailSc"))
                    Else
                        chkInvioMailSc.Checked = False
                    End If
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("SelSc")) Then
                        chkSelSc.Checked = CBool(ds.Tables(0).Rows(0).Item("SelSc"))
                    Else
                        chkSelSc.Checked = False
                    End If
                    lblCategorie.Text = "MODIFICA DATI CATEGORIA CLIENTI"
                    lblCategorie.BackColor = SEGNALA_OK
                Else
                    chkInvioMailSc.Enabled = False
                    chkSelSc.Checked = False
                    If Not NoMess Then
                        lblCategorie.Text = "Errore: non trovato elemento in tabella"
                        lblCategorie.BackColor = SEGNALA_KO
                    End If
                    Exit Function
                End If
            Else
                chkInvioMailSc.Enabled = False
                chkSelSc.Checked = False
                If Not NoMess Then
                    lblCategorie.Text = "Errore: non trovato elemento in tabella"
                    lblCategorie.BackColor = SEGNALA_KO
                End If
                Exit Function
            End If
        Catch Ex As Exception
            chkInvioMailSc.Enabled = False
            chkSelSc.Checked = False
            If Not NoMess Then
                lblCategorie.Text = "Errore: " & Ex.Message
                lblCategorie.BackColor = SEGNALA_KO
            End If
            Exit Function
        End Try
        LeggiCategorie = True
    End Function
    Private Sub chkInvioMailSc_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkInvioMailSc.CheckedChanged
        If chkInvioMailSc.Checked = False Then
            chkSelSc.Checked = False
        End If
        Call AggCategorie()
    End Sub
    Private Sub chkSelSc_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelSc.CheckedChanged
        If chkInvioMailSc.Checked = False Then
            chkSelSc.Checked = False
        End If
        Call AggCategorie()
    End Sub

    Private Sub AggCategorie()
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE Categorie SET InvioMailSc = " & IIf(chkInvioMailSc.Checked = True, "1", "0") & _
                    ", SelSc = " & IIf(chkSelSc.Checked = True, "1", "0") & _
                     " WHERE Codice = " & String.Format("{0}", GridViewBody.SelectedDataKey.Value) & ""

            If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftCoge, SQLStr) = False Then
                chkInvioMailSc.Enabled = False
                chkSelSc.Checked = False
                If Not NoMess Then
                    lblCategorie.Text = "Errore aggiornamento"
                    lblCategorie.BackColor = SEGNALA_KO
                End If
                Exit Sub
            End If
            lblCategorie.Text = "MODIFICA DATI CATEGORIA CLIENTI"
            lblCategorie.BackColor = SEGNALA_OK
        Catch ex As Exception
            chkInvioMailSc.Enabled = False
            chkSelSc.Checked = False
            If Not NoMess Then
                lblCategorie.Text = "Errore aggiorna: " & ex.Message
                lblCategorie.BackColor = SEGNALA_KO
            End If
            Exit Sub
        End Try
    End Sub
End Class