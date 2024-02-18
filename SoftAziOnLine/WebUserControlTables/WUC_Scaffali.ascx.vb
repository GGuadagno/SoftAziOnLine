Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Scaffali
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

    Public rk As StrScaffali

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSReparto.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSScaffali.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        Dim myIDMag As String = Session(IDMAGAZZINO)
        If IsNothing(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        If String.IsNullOrEmpty(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        '-- Reparto??? 
        Dim myIDRep As String = Session(IDREPARTO)
        If IsNothing(myIDRep) Then
            Session(IDREPARTO) = 0
        End If
        If String.IsNullOrEmpty(myIDRep) Then
            Session(IDREPARTO) = 0
        End If
        '-------
        lblLabelTipoRK.Text = "Gestione anagrafiche Reparto/Scaffali"
        If (Not IsPostBack) Then
            ddlMagazzino.AutoPostBack = False
            ddlMagazzino.Items.Clear()
            ddlMagazzino.Items.Add("")
            ddlMagazzino.DataBind()
            Session(IDMAGAZZINO) = 1 'fisso
            Try
                Call PosizionaItemDDL(Session(IDMAGAZZINO).ToString.Trim, ddlMagazzino, True) 'true per Ok a codici a 0
            Catch ex As Exception
                ddlMagazzino.SelectedIndex = 0
            End Try
            ddlMagazzino.AutoPostBack = True
            '-
            DDLReparto.Items.Clear()
            DDLReparto.Items.Add("")
            DDLReparto.DataBind()
            '-
            DDLScaffali.Items.Clear()
            DDLScaffali.Items.Add("")
            DDLScaffali.DataBind()
        End If
        ModalPopup.WucElement = Me
      
    End Sub

    Public Sub SvuotaCampi()
        Session(IDREPARTO) = ""
        DDLReparto.SelectedIndex = 0 : DDLReparto.BackColor = SEGNALA_OK
        Session(IDSCAFFALE) = ""
        DDLScaffali.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKSCAFFALI) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        Dim myIDMag As String = Session(IDMAGAZZINO)
        If IsNothing(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        If String.IsNullOrEmpty(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        '-- Reparto??? 
        Dim myIDRep As String = Session(IDREPARTO)
        If IsNothing(myIDRep) Then
            Session(IDREPARTO) = 0
        End If
        If String.IsNullOrEmpty(myIDRep) Then
            Session(IDREPARTO) = 0
        End If
        '-------
        PopolaEntityDati = True
        Dim dvScaffali As DataView
        dvScaffali = SqlDSScaffali.Select(DataSourceSelectArguments.Empty)
        If (dvScaffali Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvScaffali.Count > 0 Then
            If txtCodice.Text.Trim = "" Then txtCodice.Text = "0"
            dvScaffali.RowFilter = "Scaffale = " & txtCodice.Text.Trim
            If dvScaffali.Count > 0 Then
                PosizionaItemDDL(dvScaffali.Item(0).Item("Reparto").ToString.Trim, DDLReparto)
                Session(IDSCAFFALE) = dvScaffali.Item(0).Item("Scaffale")
                txtCodice.Text = dvScaffali.Item(0).Item("Scaffale").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvScaffali.Item(0).Item("Descrizione").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        If DDLReparto.SelectedIndex < 1 Then
            DDLReparto.BackColor = SEGNALA_KO : PopolaEntityDati = False
        Else
            DDLReparto.BackColor = SEGNALA_OK
        End If
        rk.IDScaffale = IIf(IsNumeric(Session(IDSCAFFALE)), Session(IDSCAFFALE), 0)
        If txtCodice.Text.Trim = "" Then
            txtCodice.BackColor = SEGNALA_KO : PopolaEntityDati = False
        ElseIf Not IsNumeric(txtCodice.Text.Trim) Then
            txtCodice.BackColor = SEGNALA_KO : PopolaEntityDati = False
        Else
            txtCodice.BackColor = SEGNALA_OK
        End If
        '-
        If txtDescrizione.Text.Trim = "" Then
            txtDescrizione.BackColor = SEGNALA_KO : PopolaEntityDati = False
        Else
            txtDescrizione.BackColor = SEGNALA_OK
        End If

        If PopolaEntityDati = False Then Exit Function

        rk.IDReparto = DDLReparto.SelectedValue
        rk.Descrizione = Mid(txtDescrizione.Text.Trim, 1, 50)
        Session(RKSCAFFALI) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        'DDLReparto.SelectedIndex = 0
        DDLScaffali.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = _ID.ToString.Trim
        txtDescrizione.Text = ""
        txtDescrizione.Focus()
    End Sub
    Public Function GetNewCodice() As Int32
        If txtCodice.Text.Trim = "" Or Not IsNumeric(txtCodice.Text.Trim) Then
            GetNewCodice = 0
            Exit Function
        End If
        GetNewCodice = CLng(txtCodice.Text.Trim)
    End Function

    Public Function Aggiorna() As Boolean
        Aggiorna = True
        Dim myIDMag As String = Session(IDMAGAZZINO)
        If IsNothing(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        If String.IsNullOrEmpty(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        '---------------
        If DDLReparto.SelectedIndex < 1 Then
            DDLReparto.BackColor = SEGNALA_KO
            Aggiorna = False
        Else
            DDLReparto.BackColor = SEGNALA_OK
        End If
        If txtCodice.Text.Trim = "" Then
            txtCodice.BackColor = SEGNALA_KO : Aggiorna = False
        ElseIf Not IsNumeric(txtCodice.Text.Trim) Then
            txtCodice.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            txtCodice.BackColor = SEGNALA_OK
        End If
        '-
        If txtDescrizione.Text.Trim = "" Then
            txtDescrizione.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            txtDescrizione.BackColor = SEGNALA_OK
        End If
        If Aggiorna = False Then Exit Function

        Try
            Session(IDSCAFFALE) = txtCodice.Text.Trim
            SqlDSScaffali.UpdateParameters.Item("Magazzino").DefaultValue = CLng(myIDMag)
            SqlDSScaffali.UpdateParameters.Item("Reparto").DefaultValue = IIf(IsNumeric(DDLReparto.SelectedValue), DDLReparto.SelectedValue, 0)
            Session(IDREPARTO) = IIf(IsNumeric(DDLReparto.SelectedValue), DDLReparto.SelectedValue, 0)
            SqlDSScaffali.UpdateParameters.Item("Scaffale").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSScaffali.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSScaffali.Update()
            SqlDSScaffali.DataBind()
            '-----
            DDLScaffali.Items.Clear()
            DDLScaffali.Items.Add("")
            DDLScaffali.DataBind()
            PopolaEntityDati()
            ' ''Dim strErrore As String = ""
            ' ''App.CaricaAgenti(Session(ESERCIZIO), strErrore) 'giu040112 AGGIORNO LA CACHE
            ' ''If strErrore.Trim <> "" Then
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''    ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            ' ''End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub DDLReparto_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLReparto.SelectedIndexChanged
        Session(IDREPARTO) = IIf(IsNumeric(DDLReparto.SelectedValue), DDLReparto.SelectedValue, 0)
        SqlDSScaffali.DataBind()
        '-----
        DDLScaffali.Items.Clear()
        DDLScaffali.Items.Add("")
        DDLScaffali.DataBind()
        '-
        txtCodice.Text = ""
        txtDescrizione.Text = ""
    End Sub

    Private Sub DDLScaffali_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLScaffali.SelectedIndexChanged
        Session(IDSCAFFALE) = IIf(IsNumeric(DDLScaffali.SelectedValue), DDLScaffali.SelectedValue, 0)
        If DDLScaffali.SelectedIndex = 0 Then
            Session(IDSCAFFALE) = "0"
            txtCodice.Text = Session(IDSCAFFALE)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDSCAFFALE)
        PopolaEntityDati()
    End Sub

    Private Sub txtCodice_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodice.TextChanged
        Dim SAVECodice As String = txtCodice.Text.Trim
        PopolaEntityDati()
        txtDescrizione.Focus()
        If txtCodice.Text.Trim = "" Then
            txtCodice.Text = SAVECodice
        Else
            txtCodice.Enabled = False
            _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        End If
    End Sub
    Private Sub txtDescrizione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDescrizione.TextChanged
        Dim dvScaffali As DataView
        dvScaffali = SqlDSScaffali.Select(DataSourceSelectArguments.Empty)
        If (dvScaffali Is Nothing) Then
            txtDescrizione.Focus()
            Exit Sub
        End If
        If dvScaffali.Count > 0 Then
            dvScaffali.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvScaffali.Count > 0 Then
                PosizionaItemDDL(dvScaffali.Item(0).Item("Reparto").ToString.Trim, DDLReparto)
                Session(IDSCAFFALE) = dvScaffali.Item(0).Item("Scaffale")
                txtCodice.Text = dvScaffali.Item(0).Item("Scaffale").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvScaffali.Item(0).Item("Descrizione").ToString.Trim
            End If
        End If
        txtDescrizione.Focus()
    End Sub

    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        If ddlMagazzino.SelectedIndex > 0 Then
            Session(IDMAGAZZINO) = ddlMagazzino.SelectedValue
        Else
            Session(IDMAGAZZINO) = -1
        End If

        DDLReparto.Items.Clear()
        DDLReparto.Items.Add("")
        DDLReparto.DataBind()
        '-
        DDLScaffali.Items.Clear()
        DDLScaffali.Items.Add("")
        DDLScaffali.DataBind()
        '-
        txtCodice.Text = ""
        txtDescrizione.Text = ""
    End Sub
End Class