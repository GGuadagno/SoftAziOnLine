Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_ElencoCatCli
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
    Dim SQLStr As String = ""

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

                ddlRicerca.Items.Add("Si/No Invio E-mail scadenze")
                ddlRicerca.Items(2).Value = "I"

                ddlRicerca.Items.Add("Si/No Selezione multipla per invio E-mail scadenze")
                ddlRicerca.Items(3).Value = "S"

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

    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Try
            Dim rtn As New STR_RETURN_VALUE
            rtn.Codice = String.Format("{0}", GridViewBody.SelectedDataKey.Value)
            rtn.Descrizione = App.GetValoreFromChiave(rtn.Codice, _Tabella, Session(ESERCIZIO))
            ' ''rtn.Descrizione = GridViewBody.SelectedRow.Cells(2).Text
            Session(RTN_VALUE_F_ELENCO) = rtn
            WfpElement.SetLblMessUtente("")
        Catch ex As Exception
            
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
                For Each El As TemplateCategorie In res
                    arrResFiltro.Add(El)
                Next
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
        Call GridViewBody_SelectedIndexChanged(GridViewBody, Nothing)
    End Sub

#End Region


    Private Sub AggCategorie()
        Dim ObjDB As New DataBaseUtility
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftCoge, SQLStr) = False Then
                WfpElement.SetLblMessUtente("Errore aggiornamento")
            End If
        Catch ex As Exception
            WfpElement.SetLblMessUtente("Errore aggiorna: " & ex.Message)
        End Try
    End Sub

    Protected Sub chkInvioEmailGR_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim cb As CheckBox = CType(sender, CheckBox)
            Dim myrow As GridViewRow = CType(cb.NamingContainer, GridViewRow)
            Dim myRowIndex As Integer = myrow.RowIndex + (GridViewBody.PageSize * GridViewBody.PageIndex)
            GridViewBody.SelectedIndex = myrow.RowIndex        'indice della griglia
            GridViewBody_SelectedIndexChanged(GridViewBody, Nothing)
            SQLStr = "UPDATE Categorie SET InvioMailSc = " & IIf(sender.Checked = True, "1", "0") & _
                         " WHERE Codice = " & String.Format("{0}", GridViewBody.SelectedDataKey.Value) & ""
            Call AggCategorie()
        Catch ex As Exception
            WfpElement.SetLblMessUtente("Errore aggiornamento: " & ex.Message)
        End Try
        
    End Sub
    Protected Sub chkSelScGR_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim cb As CheckBox = CType(sender, CheckBox)
            Dim myrow As GridViewRow = CType(cb.NamingContainer, GridViewRow)
            Dim myRowIndex As Integer = myrow.RowIndex + (GridViewBody.PageSize * GridViewBody.PageIndex)
            GridViewBody.SelectedIndex = myrow.RowIndex        'indice della griglia
            GridViewBody_SelectedIndexChanged(GridViewBody, Nothing)
            SQLStr = "UPDATE Categorie SET SelSc = " & IIf(sender.Checked = True, "1", "0") & _
                    " WHERE Codice = " & String.Format("{0}", GridViewBody.SelectedDataKey.Value) & ""
            Call AggCategorie()
        Catch ex As Exception
            WfpElement.SetLblMessUtente("Errore aggiornamento: " & ex.Message)
        End Try
        
    End Sub
End Class