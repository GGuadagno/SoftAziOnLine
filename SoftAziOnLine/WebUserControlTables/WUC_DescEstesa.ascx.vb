Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports System.IO
Imports It.SoftAzi.Integration.Dao

Partial Public Class WUC_DescEstesa
    Inherits System.Web.UI.UserControl

#Region "Costanti"

    Private Const INS_RIGA_PRIMA As Integer = 0
    Private Const INS_RIGA_DOPO As Integer = 1
    Private Const SEL_PROGRESSIVO As String = "SelProgressivo"
    Private Const SELANAMAGDES As String = "SELANAMAGDES"
#End Region

#Region "Variabili private"

    Private _Enabled As Boolean

    Private sysAnaMagDes As New AnaMagDes
    Private ArrAnaMagDes As List(Of AnaMagDesEntity) = Nothing
    Private NewArrAnaMagDes As List(Of AnaMagDesEntity) = Nothing
    Private RowAnaMagDes As AnaMagDesEntity
    Private NewRowAnaMagDes As AnaMagDesEntity
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
        Try
            btnModificaRiga.Enabled = False
            btnCancellaRiga.Enabled = False
            lblMess.Visible = False
            If (ArrAnaMagDes Is Nothing) Then
                ArrAnaMagDes = Session(SELANAMAGDES)
                If (ArrAnaMagDes Is Nothing) Then
                    ArrAnaMagDes = New List(Of AnaMagDesEntity)
                    Session(SELANAMAGDES) = ArrAnaMagDes
                End If
                GridViewBody.DataSource = ArrAnaMagDes
                GridViewBody.DataBind()
            End If
            Dim myRowIndex As Integer = GridViewBody.SelectedIndex + (GridViewBody.PageSize * GridViewBody.PageIndex)
            If (ArrAnaMagDes Is Nothing) Then
                Session(SEL_PROGRESSIVO) = String.Empty
                txtRiga.Text = ""
                lblMess.Text = "ATTENZIONE Nessuna Riga selezionata"
                lblMess.Visible = True
            ElseIf ArrAnaMagDes.Count > 0 Then
                RowAnaMagDes = CType(ArrAnaMagDes(myRowIndex), AnaMagDesEntity)
                Session(SEL_PROGRESSIVO) = RowAnaMagDes.Progressivo
                txtRiga.Text = RowAnaMagDes.Descrizione.Trim
                btnModificaRiga.Enabled = True
                btnCancellaRiga.Enabled = True
            Else
                Session(SEL_PROGRESSIVO) = String.Empty
                txtRiga.Text = ""
                lblMess.Text = "ATTENZIONE Nessuna Riga selezionata"
                lblMess.Visible = True
            End If
        Catch ex As Exception
            lblMess.Text = "ERRORE selezione Riga " + ex.Message.Trim
            lblMess.Visible = True
            btnModificaRiga.Enabled = False
            btnCancellaRiga.Enabled = False
        End Try
    End Sub

    Private Sub btnInserisciDopo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserisciDopo.Click
        If txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            lblMess.Text = "Lunghezza massima consentita è di 150 caratteri"
            lblMess.Visible = True
        Else
            lblMess.Visible = False
            txtRiga.BackColor = SEGNALA_OK
            InserimentoRiga(INS_RIGA_DOPO)
        End If
    End Sub

    Private Sub btnInserisciPrima_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserisciPrima.Click
        If txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            lblMess.Text = "Lunghezza massima consentita è di 150 caratteri"
            lblMess.Visible = True
        Else
            lblMess.Visible = False
            txtRiga.BackColor = SEGNALA_OK
            InserimentoRiga(INS_RIGA_PRIMA)
        End If
    End Sub

    Private Sub btnCancellaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancellaRiga.Click
        ConfermaCancellazioneRiga()
    End Sub

    Private Sub btnModificaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaRiga.Click
        If txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            lblMess.Text = "Lunghezza massima consentita è di 150 caratteri"
            lblMess.Visible = True
        Else
            lblMess.Visible = False
            txtRiga.BackColor = SEGNALA_OK
            If (Not String.IsNullOrEmpty(txtRiga.Text)) Then
                ModificaRiga()
            Else
                txtRiga.Text = ""
            End If
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
        'giu100424
        txtRiga.BackColor = SEGNALA_OK
        lblMess.Visible = False
        lblMess.Text = ""
        If (String.IsNullOrEmpty(txtRiga.Text)) Then
            txtRiga.Text = ""
        ElseIf txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            lblMess.Text = "Lunghezza massima consentita è di 150 caratteri"
            lblMess.Visible = True
            Exit Sub
        End If
        '---------
        Dim descRiga As String = txtRiga.Text.Trim
        Dim progRiga As Integer = Val(Session(SEL_PROGRESSIVO))
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(descRiga)) Then
            Try
                lblMess.Visible = False
                If (ArrAnaMagDes Is Nothing) Then
                    ArrAnaMagDes = Session(SELANAMAGDES)
                End If
                If (ArrAnaMagDes Is Nothing) Then
                    Session(SEL_PROGRESSIVO) = String.Empty
                    lblMess.Text = "ATTENZIONE Nessuna Riga selezionata"
                    lblMess.Visible = True
                ElseIf ArrAnaMagDes.Count > 0 Then
                    NewArrAnaMagDes = New List(Of AnaMagDesEntity)
                    For I = 0 To ArrAnaMagDes.Count - 1
                        RowAnaMagDes = ArrAnaMagDes(I)
                        NewRowAnaMagDes = New AnaMagDesEntity
                        If (progRiga.Equals(progressivo)) Then
                            NewRowAnaMagDes.Descrizione = descRiga
                        Else
                            NewRowAnaMagDes.Descrizione = RowAnaMagDes.Descrizione.ToString.Trim
                        End If
                        NewRowAnaMagDes.Progressivo = progressivo
                        NewArrAnaMagDes.Add(NewRowAnaMagDes)
                        progressivo = progressivo + 1
                    Next
                    GridViewBody.DataSource = NewArrAnaMagDes
                    GridViewBody.DataBind()
                    Session(SELANAMAGDES) = NewArrAnaMagDes
                Else
                    Session(SEL_PROGRESSIVO) = String.Empty
                    lblMess.Text = "ATTENZIONE Nessuna Riga selezionata"
                    lblMess.Visible = True
                End If
            Catch ex As Exception
                lblMess.Text = "ERRORE selezione Riga " + ex.Message.Trim
                lblMess.Visible = True
            End Try
        End If
    End Sub

    Private Sub InserimentoRiga(ByVal posizione As Integer)
        'giu100424
        txtRiga.BackColor = SEGNALA_OK
        lblMess.Visible = False
        lblMess.Text = ""
        If (String.IsNullOrEmpty(txtRiga.Text)) Then
            txtRiga.Text = ""
            '''txtRiga.BackColor = SEGNALA_KO
            '''lblMess.Text = "Descrizione obbligatoria"
            '''lblMess.Visible = True
            '''Exit Sub
        ElseIf txtRiga.Text.Trim.Length > 150 Then
            txtRiga.BackColor = SEGNALA_KO
            lblMess.Text = "Lunghezza massima consentita è di 150 caratteri"
            lblMess.Visible = True
            Exit Sub
        End If
        '---------
        Dim nuovaRiga As String = txtRiga.Text.Trim
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
        '-
        Try
            Session(SEL_PROGRESSIVO) = String.Format("{0}", progNuovaRiga)
            lblMess.Visible = False
            If (ArrAnaMagDes Is Nothing) Then
                ArrAnaMagDes = Session(SELANAMAGDES)
            End If
            If (ArrAnaMagDes Is Nothing) Then
                Session(SEL_PROGRESSIVO) = String.Empty
                lblMess.Text = "ATTENZIONE Nessuna Riga selezionata"
                lblMess.Visible = True
            ElseIf ArrAnaMagDes.Count > 0 Then
                NewArrAnaMagDes = New List(Of AnaMagDesEntity)
                For I = 0 To ArrAnaMagDes.Count - 1
                    NewRowAnaMagDes = New AnaMagDesEntity
                    RowAnaMagDes = ArrAnaMagDes(I)
                    If (progNuovaRiga.Equals(progressivo)) Then
                        NewRowAnaMagDes.Descrizione = nuovaRiga
                        NewRowAnaMagDes.Progressivo = progressivo
                        NewArrAnaMagDes.Add(NewRowAnaMagDes)
                        progressivo = progressivo + 1
                        '-
                        NewRowAnaMagDes = New AnaMagDesEntity
                    End If
                    NewRowAnaMagDes.Progressivo = progressivo
                    NewRowAnaMagDes.Descrizione = RowAnaMagDes.Descrizione.ToString.Trim
                    NewArrAnaMagDes.Add(NewRowAnaMagDes)
                    progressivo = progressivo + 1
                Next
                If (progNuovaRiga.Equals(progressivo)) Then
                    If (NewArrAnaMagDes Is Nothing) Then
                        NewArrAnaMagDes = New List(Of AnaMagDesEntity)
                    End If
                    NewRowAnaMagDes = New AnaMagDesEntity
                    NewRowAnaMagDes.Descrizione = nuovaRiga
                    NewRowAnaMagDes.Progressivo = progressivo
                    NewArrAnaMagDes.Add(NewRowAnaMagDes)
                    progressivo = progressivo + 1
                End If
            Else
                NewArrAnaMagDes = New List(Of AnaMagDesEntity)
                NewRowAnaMagDes = New AnaMagDesEntity
                NewRowAnaMagDes.Descrizione = nuovaRiga
                NewRowAnaMagDes.Progressivo = progressivo
                NewArrAnaMagDes.Add(NewRowAnaMagDes)
            End If
            If (NewArrAnaMagDes Is Nothing) Then
                NewArrAnaMagDes = New List(Of AnaMagDesEntity)
            End If
            GridViewBody.DataSource = NewArrAnaMagDes
            GridViewBody.DataBind()
            Session(SELANAMAGDES) = NewArrAnaMagDes
            SelezionaRigaFromProgressivo(Session(SEL_PROGRESSIVO))
        Catch ex As Exception
            lblMess.Text = "ERRORE selezione Riga " + ex.Message.Trim
            lblMess.Visible = True
        End Try
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
        Try
            ArrAnaMagDes = Nothing
            For Each x As AnaMagDesEntity In sysAnaMagDes.getAnaMagDesByCodiceArticolo(Session(COD_ARTICOLO).ToString.Trim)
                If (ArrAnaMagDes Is Nothing) Then
                    ArrAnaMagDes = New List(Of AnaMagDesEntity)
                End If
                ArrAnaMagDes.Add(x)
            Next
            If (ArrAnaMagDes Is Nothing) Then
                ArrAnaMagDes = New List(Of AnaMagDesEntity)
            End If
            GridViewBody.DataSource = ArrAnaMagDes
            Session(SELANAMAGDES) = ArrAnaMagDes
            GridViewBody.DataBind()
            GridViewBody.SelectedIndex = 0
            'giu090424
            If (ArrAnaMagDes Is Nothing) Then
                Session(SEL_PROGRESSIVO) = String.Empty
                txtRiga.Text = ""
            ElseIf ArrAnaMagDes.Count > 0 Then
                RowAnaMagDes = CType(ArrAnaMagDes(0), AnaMagDesEntity)
                Session(SEL_PROGRESSIVO) = RowAnaMagDes.Progressivo
                txtRiga.Text = RowAnaMagDes.Descrizione.Trim
                btnModificaRiga.Enabled = True
                btnCancellaRiga.Enabled = True
            Else
                Session(SEL_PROGRESSIVO) = String.Empty
                txtRiga.Text = ""
            End If
        Catch ex As Exception
            lblMess.Text = "ERRORE selezione Riga " + ex.Message.Trim
            lblMess.Visible = True
            txtRiga.Text = String.Empty
            btnModificaRiga.Enabled = False
            btnCancellaRiga.Enabled = False
        End Try

    End Sub

    Public Sub SvuotaGridView()
        txtRiga.Text = String.Empty
        CaricaVariabili()
        ArrAnaMagDes = New List(Of AnaMagDesEntity)
        GridViewBody.DataSource = ArrAnaMagDes
        Session(SELANAMAGDES) = ArrAnaMagDes
        GridViewBody.DataBind()
    End Sub

    Public Sub ConfermaCancellazioneRiga()
        lblMess.Visible = False
        Dim progCancRiga As Integer = -1
        Dim progressivo As Integer = 1
        Try
            If (ArrAnaMagDes Is Nothing) Then
                ArrAnaMagDes = Session(SELANAMAGDES)
            End If
            If (ArrAnaMagDes Is Nothing) Then
                Session(SEL_PROGRESSIVO) = String.Empty
                lblMess.Text = "ATTENZIONE Nessuna Riga selezionata"
                lblMess.Visible = True
                Exit Sub
            End If
            If (Not String.IsNullOrEmpty(Session(SEL_PROGRESSIVO))) Then
                progCancRiga = Session(SEL_PROGRESSIVO)
                If ArrAnaMagDes.Count > 0 Then
                    NewArrAnaMagDes = New List(Of AnaMagDesEntity)
                    For I = 0 To ArrAnaMagDes.Count - 1
                        RowAnaMagDes = ArrAnaMagDes(I)
                        If (Not RowAnaMagDes.Progressivo.Equals(progCancRiga)) Then
                            NewRowAnaMagDes = New AnaMagDesEntity
                            NewRowAnaMagDes.Progressivo = progressivo
                            NewRowAnaMagDes.Descrizione = RowAnaMagDes.Descrizione.Trim
                            NewArrAnaMagDes.Add(NewRowAnaMagDes)
                            progressivo = progressivo + 1
                        End If
                    Next
                End If
                If (NewArrAnaMagDes Is Nothing) Then
                    NewArrAnaMagDes = New List(Of AnaMagDesEntity)
                End If
                GridViewBody.DataSource = NewArrAnaMagDes
                GridViewBody.DataBind()
                Session(SELANAMAGDES) = NewArrAnaMagDes
            Else
                lblMess.Text = "ATTENZIONE Nessuna Riga selezionata"
                lblMess.Visible = True
            End If
            If GridViewBody.Rows.Count = 0 Then
                btnModificaRiga.Enabled = False
                btnCancellaRiga.Enabled = False
            End If
        Catch ex As Exception
            lblMess.Text = "ERRORE cancella Riga " + ex.Message.Trim
            lblMess.Visible = True
        End Try

    End Sub

    Public Function GetListDescEstesa() As List(Of AnaMagDesEntity)
        Dim myListAnaMagDes As New List(Of AnaMagDesEntity)
        Try
            lblMess.Visible = False
            If (ArrAnaMagDes Is Nothing) Then
                ArrAnaMagDes = Session(SELANAMAGDES)
            End If
            If (ArrAnaMagDes Is Nothing) Then
                lblMess.Text = "ERRORE selezione Lista completa risulta vuota"
                lblMess.Visible = True
                GetListDescEstesa = myListAnaMagDes
                Exit Function
            End If
            If ArrAnaMagDes.Count > 0 Then
                For I = 0 To ArrAnaMagDes.Count - 1
                    NewRowAnaMagDes = New AnaMagDesEntity
                    RowAnaMagDes = ArrAnaMagDes(I)
                    NewRowAnaMagDes.Progressivo = RowAnaMagDes.Progressivo
                    NewRowAnaMagDes.CodArticolo = Session(COD_ARTICOLO).ToString.Trim
                    NewRowAnaMagDes.Descrizione = RowAnaMagDes.Descrizione.Trim
                    myListAnaMagDes.Add(NewRowAnaMagDes)
                Next
            End If
        Catch ex As Exception
            lblMess.Text = "ERRORE selezione Lista completa " + ex.Message.Trim
            lblMess.Visible = True
        End Try
        If Not (myListAnaMagDes Is Nothing) Then
            GetListDescEstesa = myListAnaMagDes
        Else
            myListAnaMagDes = New List(Of AnaMagDesEntity)
            GetListDescEstesa = myListAnaMagDes
        End If
    End Function


#End Region

End Class