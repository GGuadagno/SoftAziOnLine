Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def

Partial Public Class WUC_ArticoliCTV
    Inherits System.Web.UI.UserControl

#Region "Costanti"

    Private Const INS_RIGA_PRIMA As Integer = 0
    Private Const INS_RIGA_DOPO As Integer = 1
    Private Const SEL_PROGRESSIVO As String = "SelProgressivo"
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
            PulsantiSetEnabledTo(value)
        End Set
    End Property

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            CaricaVariabili()
        End If

    End Sub

    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Session(SEL_PROGRESSIVO) = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(2).Text
        txtTipo.Text = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(3).Text.Replace(HTML_SPAZIO, "").Trim
        txtValore.Text = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(4).Text.Replace(HTML_SPAZIO, "").Trim
        btnModificaRiga.Enabled = True
        btnCancellaRiga.Enabled = True
        lblMess.Visible = False
    End Sub

    Private Function funCKCampiTV() As Boolean
        funCKCampiTV = False
        txtTipo.Text.Replace("'", " ") 'GIU120424
        txtValore.Text.Replace("'", " ")
        If txtTipo.Text.Trim.Length > 35 Then
            txtTipo.BackColor = SEGNALA_KO
            lblMess.Text = "Lunghezza massima consentita è di 35 caratteri"
            lblMess.Visible = True
            Exit Function
        ElseIf txtTipo.Text.Trim.Length = 0 Then
            lblMess.Text = "Campi Obbligatori"
            lblMess.Visible = True
            Exit Function
        End If
        If txtValore.Text.Trim.Length > 35 Then
            txtValore.BackColor = SEGNALA_KO
            lblMess.Text = "Lunghezza massima consentita è di 35 caratteri"
            lblMess.Visible = True
            Exit Function
        ElseIf txtValore.Text.Trim.Length = 0 Then
            lblMess.Text = "Campi Obbligatori"
            lblMess.Visible = True
            Exit Function
        End If
        funCKCampiTV = True
    End Function
    Private Sub btnInserisciDopo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserisciDopo.Click
        If funCKCampiTV() = False Then
            Exit Sub
        End If
        txtTipo.BackColor = SEGNALA_OK
        txtValore.BackColor = SEGNALA_OK
        InserimentoRiga(INS_RIGA_DOPO)
    End Sub

    Private Sub btnInserisciPrima_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserisciPrima.Click
        If funCKCampiTV() = False Then
            Exit Sub
        End If
        txtTipo.BackColor = SEGNALA_OK
        txtValore.BackColor = SEGNALA_OK
        InserimentoRiga(INS_RIGA_PRIMA)
    End Sub

    Private Sub btnCancellaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancellaRiga.Click
        ConfermaCancellazioneRiga()
    End Sub

    Private Sub btnModificaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaRiga.Click
        If funCKCampiTV() = False Then
            Exit Sub
        End If
        txtTipo.BackColor = SEGNALA_OK
        txtValore.BackColor = SEGNALA_OK
        If (Not String.IsNullOrEmpty(txtTipo.Text)) And (Not String.IsNullOrEmpty(txtValore.Text)) Then
            ModificaRiga()
        End If
    End Sub

#End Region

#Region "Gestione variabili di sessione interne"

    Private Sub CaricaVariabili()
        Session(SEL_PROGRESSIVO) = String.Empty
    End Sub

#End Region

#Region "Metodi private"

    Private Sub PulsantiSetEnabledTo(ByVal valore As Boolean)
        btnCancellaRiga.Enabled = valore
        btnInserisciDopo.Enabled = valore
        btnInserisciPrima.Enabled = valore
        btnCancellaRiga.Visible = valore
        btnInserisciDopo.Visible = valore
        btnInserisciPrima.Visible = valore
        btnModificaRiga.Visible = valore
        txtTipo.Enabled = valore
        txtValore.Enabled = valore
        If valore = False Then
            lblMess.Visible = False
        End If
    End Sub

    Private Sub ModificaRiga()
        Dim arrAnaMagCTV As New ArrayList
        Dim TipoRiga As String = txtTipo.Text.Trim
        Dim ValoreRiga As String = txtValore.Text.Trim
        Dim progRiga As Integer = Val(Session(SEL_PROGRESSIVO))
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(TipoRiga)) And (Not String.IsNullOrEmpty(ValoreRiga)) Then
            If (GridViewBody.Rows.Count > 0) Then
                For Each row As GridViewRow In GridViewBody.Rows
                    Dim myAnaMagCTV As New AnaMagCTVEntity
                    If (progRiga.Equals(progressivo)) Then
                        myAnaMagCTV.Tipo = TipoRiga
                        myAnaMagCTV.Valore = ValoreRiga
                    Else
                        myAnaMagCTV.Tipo = row.Cells(3).Text.Replace(HTML_SPAZIO, "").Trim
                        myAnaMagCTV.Valore = row.Cells(4).Text.Replace(HTML_SPAZIO, "").Trim
                    End If
                    myAnaMagCTV.Progressivo = progressivo
                    arrAnaMagCTV.Add(myAnaMagCTV)
                    progressivo = progressivo + 1
                Next
            End If
            GridViewBody.DataSource = arrAnaMagCTV
            GridViewBody.DataBind()
        End If
    End Sub

    Private Sub InserimentoRiga(ByVal posizione As Integer)
        lblMess.Visible = False
        Dim myAnaMagCTV As New AnaMagCTVEntity
        Dim arrAnaMagCTV As New ArrayList
        Dim nuovoTipo As String = txtTipo.Text
        Dim nuovoValore As String = txtValore.Text
        Dim progNuovaRiga As Integer = 0
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(Session(SEL_PROGRESSIVO))) Then
            If (posizione.Equals(INS_RIGA_PRIMA)) Then
                progNuovaRiga = Val(Session(SEL_PROGRESSIVO))
            ElseIf (posizione.Equals(INS_RIGA_DOPO)) Then
                progNuovaRiga = Val(Session(SEL_PROGRESSIVO)) + 1
            End If
        Else
            If (posizione.Equals(INS_RIGA_PRIMA)) Then
                progNuovaRiga = 1
            ElseIf (posizione.Equals(INS_RIGA_DOPO)) Then
                progNuovaRiga = GridViewBody.Rows.Count + 1
            End If
        End If

        If (Not String.IsNullOrEmpty(nuovoTipo)) And (Not String.IsNullOrEmpty(nuovoValore)) Then
            Session(SEL_PROGRESSIVO) = String.Format("{0}", progNuovaRiga)
            If (GridViewBody.Rows.Count > 0) Then
                For Each row As GridViewRow In GridViewBody.Rows
                    Dim myAMCTV As New AnaMagCTVEntity
                    If (progNuovaRiga.Equals(progressivo)) Then
                        myAMCTV.Tipo = nuovoTipo
                        myAMCTV.Valore = nuovoValore
                        myAMCTV.Progressivo = progressivo
                        arrAnaMagCTV.Add(myAnaMagCTV)
                        progressivo = progressivo + 1
                    End If
                    myAMCTV.Progressivo = progressivo
                    myAMCTV.Tipo = row.Cells(3).Text.Replace(HTML_SPAZIO, "").Trim
                    myAMCTV.Valore = row.Cells(4).Text.Replace(HTML_SPAZIO, "").Trim
                    arrAnaMagCTV.Add(myAMCTV)
                    progressivo = progressivo + 1
                Next
                If (progNuovaRiga.Equals(progressivo)) Then
                    myAnaMagCTV.Tipo = nuovoTipo
                    myAnaMagCTV.Valore = nuovoValore
                    myAnaMagCTV.Progressivo = progressivo
                    arrAnaMagCTV.Add(myAnaMagCTV)
                End If
            Else
                myAnaMagCTV.Tipo = nuovoTipo
                myAnaMagCTV.Valore = nuovoValore
                myAnaMagCTV.Progressivo = progressivo
                arrAnaMagCTV.Add(myAnaMagCTV)
            End If
            GridViewBody.DataSource = arrAnaMagCTV
            GridViewBody.DataBind()
            SelezionaRigaFromProgressivo(Session(SEL_PROGRESSIVO))
        Else
            lblMess.Visible = True
        End If
    End Sub

    Private Sub SelezionaRigaFromProgressivo(ByVal progressivo As String)
        Dim index As Integer = 0
        For Each row As GridViewRow In GridViewBody.Rows
            If (row.Cells(2).Text = progressivo) Then
                GridViewBody.SelectedIndex = index
                btnModificaRiga.Enabled = True
                Exit For
            End If
            index = index + 1
        Next
    End Sub

#End Region

#Region "Metodi public"

    Public Sub PopolaGridView()
        txtTipo.Text = String.Empty
        txtValore.Text = String.Empty
        btnModificaRiga.Enabled = False
        btnCancellaRiga.Enabled = False
        CaricaVariabili()
        SqlDataSourceAnaMagCTV.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceAnaMagCTV.SelectParameters("Codice").DefaultValue = Session(COD_ARTICOLO)
        GridViewBody.DataSource = SqlDataSourceAnaMagCTV
        GridViewBody.DataBind()
        GridViewBody.SelectedIndex = 0
        If (GridViewBody.Rows.Count > 0) Then
            Session(SEL_PROGRESSIVO) = GridViewBody.Rows(0).Cells(2).Text
            txtTipo.Text = GridViewBody.Rows(0).Cells(3).Text.Replace(HTML_SPAZIO, "").Trim
            txtValore.Text = GridViewBody.Rows(0).Cells(4).Text.Replace(HTML_SPAZIO, "").Trim
            btnModificaRiga.Enabled = True
            btnCancellaRiga.Enabled = True
        End If
    End Sub

    Public Sub SvuotaGridView()
        txtTipo.Text = String.Empty
        txtValore.Text = String.Empty
        CaricaVariabili()
        SqlDataSourceAnaMagCTV.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceAnaMagCTV.SelectParameters("Codice").DefaultValue = "0"
        GridViewBody.DataSource = SqlDataSourceAnaMagCTV
        GridViewBody.DataBind()
    End Sub

    Public Sub ConfermaCancellazioneRiga()
        lblMess.Visible = False
        Dim arrAnaMagCTV As New ArrayList
        Dim progCancRiga As String = String.Empty
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(Session(SEL_PROGRESSIVO))) Then
            progCancRiga = Session(SEL_PROGRESSIVO)
            If (GridViewBody.Rows.Count > 0) Then
                txtTipo.Text = String.Empty
                txtValore.Text = String.Empty
                For Each row As GridViewRow In GridViewBody.Rows
                    If (Not row.Cells(2).Text.Equals(progCancRiga)) Then
                        Dim myAMCTV As New AnaMagCTVEntity
                        myAMCTV.Progressivo = progressivo
                        myAMCTV.Tipo = row.Cells(3).Text.Replace(HTML_SPAZIO, "").Trim
                        myAMCTV.Valore = row.Cells(4).Text.Replace(HTML_SPAZIO, "").Trim
                        arrAnaMagCTV.Add(myAMCTV)
                        progressivo = progressivo + 1
                    End If
                Next
            End If
            GridViewBody.DataSource = arrAnaMagCTV
            GridViewBody.SelectedIndex = -1
            GridViewBody.DataBind()
            If (GridViewBody.Rows.Count = 0) Then
                btnModificaRiga.Enabled = False
                btnCancellaRiga.Enabled = False
            End If
        End If

        If GridViewBody.Rows.Count = 0 Then
            btnModificaRiga.Enabled = False
            btnCancellaRiga.Enabled = False
        End If
    End Sub

    Public Function GetListAnaMagCTV() As List(Of AnaMagCTVEntity)
        Dim myListAnaMagCTV As New List(Of AnaMagCTVEntity)

        For Each row As GridViewRow In GridViewBody.Rows
            Dim myAnaMagCTV As New AnaMagCTVEntity
            myAnaMagCTV.CodArticolo = Session(COD_ARTICOLO)
            myAnaMagCTV.Progressivo = row.Cells(2).Text
            myAnaMagCTV.Tipo = row.Cells(3).Text.Replace(HTML_SPAZIO, "").Trim
            myAnaMagCTV.Valore = row.Cells(4).Text.Replace(HTML_SPAZIO, "").Trim
            myListAnaMagCTV.Add(myAnaMagCTV)
        Next

        GetListAnaMagCTV = myListAnaMagCTV
    End Function

#End Region

End Class