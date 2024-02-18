Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_ElencoCliForn
    Inherits System.Web.UI.UserControl

#Region "Costanti"

    Private Const FILTRO_ATTIVO As String = "FiltroAttivo"
    Private Const LISTA As String = "NomeLista"
    Private Const DESC_TITOLO As String = "Desc_Titolo"

#End Region

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
        If (Not IsPostBack) Then
            Try
                ddlRicerca.Items.Clear()
                ddlRicerca.Items.Add("")
                ddlRicerca.Items(0).Value = ""
                ddlRicerca.Items.Add("Codice")
                ddlRicerca.Items(1).Value = "Codice_CoGe"
                ddlRicerca.Items.Add("Codice IPA/Dest.")
                ddlRicerca.Items(2).Value = "IPA"
                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(3).Value = "Rag_Soc"
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(4).Value = "Denominazione"
                ddlRicerca.Items.Add("Partita IVA")
                ddlRicerca.Items(5).Value = "Partita_Iva"
                ddlRicerca.Items.Add("Codice Fiscale")
                ddlRicerca.Items(6).Value = "Codice_Fiscale"
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(7).Value = "CAP"
                ddlRicerca.Items.Add("Indirizzo")
                ddlRicerca.Items(8).Value = "Indirizzo"
                ddlRicerca.Items.Add("Email (Tutte)")
                ddlRicerca.Items(9).Value = "Email"
                ddlRicerca.AutoPostBack = False
                ddlRicerca.SelectedIndex = 3
                txtLocalita.Enabled = True
                ddlRicerca.AutoPostBack = True
                checkParoleContenute.Checked = True
                '-giu180420
                GridViewBody.DataSource = Nothing
                '-
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
        If (Session(F_ELENCO_CLIFORN_APERTA)) Then
            Filtra()
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
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
    End Sub

    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Dim rtn As New STR_RETURN_VALUE
        rtn.Codice = String.Format("{0}", GridViewBody.SelectedDataKey.Value)
        rtn.Descrizione = App.GetValoreFromChiave(rtn.Codice, Session(LISTA), Session(ESERCIZIO))
        Session(RTN_VALUE_F_ELENCO) = rtn
        WfpElement.SetLblMessUtente("Selezionato: " & rtn.Codice & " - " & rtn.Descrizione.Trim)
    End Sub

    Private Sub GridViewBody_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewBody.Sorting
    End Sub

    Private Sub btnRicerca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicerca.Click
        Filtra(True)
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
        GridViewBody.DataSource = Nothing
        GridViewBody.DataBind()
        CaricaVariabili()
        Session(LISTA) = nomeLista
        Session(DESC_TITOLO) = titolo
        Filtra()
    End Sub

#End Region

#Region "Metodi private"
    
    Private Sub Filtra(Optional ByVal _Ricerca As Boolean = False)
        Dim arrTabella As ArrayList = Nothing
        Dim arrResFiltro As ArrayList = Nothing
        Dim res = Nothing

        lbTitolo.Text = Session(DESC_TITOLO)
        arrTabella = App.GetLista(Session(LISTA), Session(ESERCIZIO))
        If String.IsNullOrEmpty(txtRicerca.Text.Trim) And String.IsNullOrEmpty(txtLocalita.Text.Trim) Then
            GridViewBody.DataSource = arrTabella
            Session(FILTRO_ATTIVO) = False
        ElseIf (checkParoleContenute.Checked) Then
            Select Case ddlRicerca.SelectedValue
                Case "Codice_CoGe"
                    res = From x In arrTabella Where x.Codice_Coge.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                Case "IPA"
                    res = From x In arrTabella Where x.IPA.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                Case "Rag_Soc"
                    ' ''res = From x In arrTabella Where x.Rag_Soc.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        res = From x In arrTabella Where x.Rag_Soc.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper()) And _
                            x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.ToUpper())
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        res = From x In arrTabella Where x.Rag_Soc.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        res = From x In arrTabella Where x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.Trim.ToUpper())
                    End If
                Case "Denominazione"
                    ' ''res = From x In arrTabella Where x.Denominazione.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        res = From x In arrTabella Where x.Denominazione.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper()) And _
                            x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.ToUpper())
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        res = From x In arrTabella Where x.Denominazione.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        res = From x In arrTabella Where x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.Trim.ToUpper())
                    End If
                Case "Partita_Iva"
                    res = From x In arrTabella Where x.Partita_Iva.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                Case "Codice_Fiscale"
                    res = From x In arrTabella Where x.Codice_Fiscale.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                Case "Cap"
                    res = From x In arrTabella Where x.Cap.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                Case "Indirizzo"
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        res = From x In arrTabella Where x.Indirizzo.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper()) And _
                            x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.ToUpper())
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        res = From x In arrTabella Where x.Indirizzo.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper())
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        res = From x In arrTabella Where x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.Trim.ToUpper())
                    End If
                Case "Email"
                    res = From x In arrTabella Where x.Email.ToString.ToUpper().Contains(txtRicerca.Text.Trim.ToUpper()) Or _
                            x.EmailInvioScad.ToString.ToUpper().Contains(txtRicerca.Text.ToUpper()) Or _
                            x.EmailInvioFatt.ToString.ToUpper().Contains(txtRicerca.Text.ToUpper()) Or _
                            x.PECEmail.ToString.ToUpper().Contains(txtRicerca.Text.ToUpper())
            End Select
            If (Not res Is Nothing) Then
                arrResFiltro = New ArrayList()
                For Each el As TemplateGridViewElencoClienti In res
                    arrResFiltro.Add(el)
                Next
            End If
            GridViewBody.DataSource = arrResFiltro
            Session(FILTRO_ATTIVO) = True
        Else
            Select Case ddlRicerca.SelectedValue
                Case "Codice_CoGe"
                    res = From x In arrTabella Where x.Codice_Coge.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
                Case "IPA"
                    res = From x In arrTabella Where x.IPA.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
                Case "Rag_Soc"
                    res = From x In arrTabella Where x.Rag_Soc.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
                Case "Denominazione"
                    res = From x In arrTabella Where x.Denominazione.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
                Case "Partita_Iva"
                    res = From x In arrTabella Where x.Partita_Iva.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
                Case "Codice_Fiscale"
                    res = From x In arrTabella Where x.Codice_Fiscale.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
                Case "Cap"
                    res = From x In arrTabella Where x.Cap.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
                Case "Indirizzo"
                    If txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim <> "" Then
                        res = From x In arrTabella Where x.Indirizzo.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper()) And _
                            x.Localita.ToString.ToUpper().Contains(txtLocalita.Text.ToUpper())
                    ElseIf txtRicerca.Text.Trim <> "" And txtLocalita.Text.Trim = "" Then
                        res = From x In arrTabella Where x.Indirizzo.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper())
                    ElseIf txtRicerca.Text.Trim = "" And txtLocalita.Text.Trim <> "" Then
                        res = From x In arrTabella Where x.Localita.ToString.ToUpper().StartsWith(txtLocalita.Text.Trim.ToUpper())
                    End If
                Case "Email"
                    res = From x In arrTabella Where x.Email.ToString.ToUpper().StartsWith(txtRicerca.Text.Trim.ToUpper()) Or _
                            x.EmailInvioScad.ToString.ToUpper().StartsWith(txtRicerca.Text.ToUpper()) Or _
                            x.EmailInvioFatt.ToString.ToUpper().StartsWith(txtRicerca.Text.ToUpper()) Or _
                            x.PECEmail.ToString.ToUpper().StartsWith(txtRicerca.Text.ToUpper())
            End Select
            If (Not res Is Nothing) Then
                arrResFiltro = New ArrayList()
                For Each el As TemplateGridViewElencoClienti In res
                    arrResFiltro.Add(el)
                Next
            End If
            GridViewBody.DataSource = arrResFiltro
            Session(FILTRO_ATTIVO) = True
        End If
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

#End Region

End Class