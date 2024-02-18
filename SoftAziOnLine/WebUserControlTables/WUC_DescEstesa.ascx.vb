Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def

Partial Public Class WUC_DescEstesa
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
        txtRiga.Text = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(3).Text
        btnModificaRiga.Enabled = True
        btnCancellaRiga.Enabled = True
        lblMess.Visible = False
    End Sub

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        ' ''e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        ' ''e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
    End Sub

    Private Sub btnInserisciDopo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserisciDopo.Click
        If txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            txtRiga.ToolTip = "Lunghezza massima consentita è di 150 caratteri"
        Else
            txtRiga.BackColor = SEGNALA_OK
            InserimentoRiga(INS_RIGA_DOPO)
        End If
    End Sub

    Private Sub btnInserisciPrima_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserisciPrima.Click
        If txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            txtRiga.ToolTip = "Lunghezza massima consentita è di 150 caratteri"
        Else
            txtRiga.BackColor = SEGNALA_OK
            InserimentoRiga(INS_RIGA_PRIMA)
        End If
    End Sub

    Private Sub btnCancellaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancellaRiga.Click
        If (Not String.IsNullOrEmpty(txtRiga.Text)) Then
            ConfermaCancellazioneRiga()
        End If
    End Sub

    Private Sub btnModificaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaRiga.Click
        If txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            txtRiga.ToolTip = "Lunghezza massima consentita è di 150 caratteri"
        Else
            txtRiga.BackColor = SEGNALA_OK
            If (Not String.IsNullOrEmpty(txtRiga.Text)) Then
                ModificaRiga()
            End If
        End If
    End Sub

    Private Sub txtRiga_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRiga.TextChanged
        btnModificaRiga.Enabled = False
        btnCancellaRiga.Enabled = False
        If (Not String.IsNullOrEmpty(txtRiga.Text)) Then
            btnModificaRiga.Enabled = True
            btnCancellaRiga.Enabled = True
            lblMess.Visible = False
        ElseIf txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            txtRiga.ToolTip = "Lunghezza massima consentita è di 150 caratteri"
        Else
            lblMess.Visible = True
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
        txtRiga.Enabled = valore
        If valore = False Then
            lblMess.Visible = False
        End If
    End Sub

    Private Sub ModificaRiga()
        Dim arrAnaMagDes As New ArrayList
        Dim descRiga As String = txtRiga.Text
        Dim progRiga As Integer = Val(Session(SEL_PROGRESSIVO))
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(descRiga)) Then
            If (GridViewBody.Rows.Count > 0) Then
                For Each row As GridViewRow In GridViewBody.Rows
                    Dim myAnaMagDes As New AnaMagDesEntity
                    If (progRiga.Equals(progressivo)) Then
                        myAnaMagDes.Descrizione = descRiga
                    Else
                        myAnaMagDes.Descrizione = row.Cells(3).Text
                    End If
                    myAnaMagDes.Progressivo = progressivo
                    arrAnaMagDes.Add(myAnaMagDes)
                    progressivo = progressivo + 1
                Next
            End If
            GridViewBody.DataSource = arrAnaMagDes
            GridViewBody.DataBind()
        End If
    End Sub

    Private Sub InserimentoRiga(ByVal posizione As Integer)
        lblMess.Visible = False
        Dim myAnaMagDes As New AnaMagDesEntity
        Dim arrAnaMagDes As New ArrayList
        Dim nuovaRiga As String = txtRiga.Text
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

        If (Not String.IsNullOrEmpty(nuovaRiga)) Then
            Session(SEL_PROGRESSIVO) = String.Format("{0}", progNuovaRiga)
            If (GridViewBody.Rows.Count > 0) Then
                For Each row As GridViewRow In GridViewBody.Rows
                    Dim myAMD As New AnaMagDesEntity
                    If (progNuovaRiga.Equals(progressivo)) Then
                        myAnaMagDes.Descrizione = nuovaRiga
                        myAnaMagDes.Progressivo = progressivo
                        arrAnaMagDes.Add(myAnaMagDes)
                        progressivo = progressivo + 1
                    End If
                    myAMD.Progressivo = progressivo
                    myAMD.Descrizione = row.Cells(3).Text
                    arrAnaMagDes.Add(myAMD)
                    progressivo = progressivo + 1
                Next
                If (progNuovaRiga.Equals(progressivo)) Then
                    myAnaMagDes.Descrizione = nuovaRiga
                    myAnaMagDes.Progressivo = progressivo
                    arrAnaMagDes.Add(myAnaMagDes)
                End If
            Else
                myAnaMagDes.Descrizione = nuovaRiga
                myAnaMagDes.Progressivo = progressivo
                arrAnaMagDes.Add(myAnaMagDes)
            End If
            GridViewBody.DataSource = arrAnaMagDes
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
        txtRiga.Text = String.Empty
        btnModificaRiga.Enabled = False
        btnCancellaRiga.Enabled = False
        CaricaVariabili()
        SqlDataSourceDescEstesa.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceDescEstesa.SelectParameters("Codice").DefaultValue = Session(COD_ARTICOLO)
        GridViewBody.DataSource = SqlDataSourceDescEstesa
        GridViewBody.DataBind()
        GridViewBody.SelectedIndex = 0
        If (GridViewBody.Rows.Count > 0) Then
            Session(SEL_PROGRESSIVO) = GridViewBody.Rows(0).Cells(2).Text
            txtRiga.Text = GridViewBody.Rows(0).Cells(3).Text
            btnModificaRiga.Enabled = True
            btnCancellaRiga.Enabled = True
        End If
    End Sub

    Public Sub SvuotaGridView()
        txtRiga.Text = String.Empty
        CaricaVariabili()
        SqlDataSourceDescEstesa.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceDescEstesa.SelectParameters("Codice").DefaultValue = "0"
        GridViewBody.DataSource = SqlDataSourceDescEstesa
        GridViewBody.DataBind()
    End Sub

    Public Sub ConfermaCancellazioneRiga()
        lblMess.Visible = False
        Dim arrAnaMagDes As New ArrayList
        Dim progCancRiga As String = String.Empty
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(Session(SEL_PROGRESSIVO))) Then
            progCancRiga = Session(SEL_PROGRESSIVO)
            If (GridViewBody.Rows.Count > 0) Then
                txtRiga.Text = String.Empty
                For Each row As GridViewRow In GridViewBody.Rows
                    If (Not row.Cells(2).Text.Equals(progCancRiga)) Then
                        Dim myAMD As New AnaMagDesEntity
                        myAMD.Progressivo = progressivo
                        myAMD.Descrizione = row.Cells(3).Text
                        arrAnaMagDes.Add(myAMD)
                        progressivo = progressivo + 1
                    End If
                Next
            End If
            GridViewBody.DataSource = arrAnaMagDes
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

    Public Function GetListDescEstesa() As List(Of AnaMagDesEntity)
        Dim myListAnaMagDes As New List(Of AnaMagDesEntity)

        For Each row As GridViewRow In GridViewBody.Rows
            Dim myAnaMagDes As New AnaMagDesEntity
            myAnaMagDes.CodArticolo = Session(COD_ARTICOLO)
            myAnaMagDes.Progressivo = row.Cells(2).Text
            myAnaMagDes.Descrizione = IIf(row.Cells(3).Text = HTML_SPAZIO, String.Empty, row.Cells(3).Text)
            myListAnaMagDes.Add(myAnaMagDes)
        Next

        GetListDescEstesa = myListAnaMagDes
    End Function

#End Region

End Class