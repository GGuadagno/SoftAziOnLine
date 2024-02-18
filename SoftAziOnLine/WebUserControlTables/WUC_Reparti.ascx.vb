Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Reparti
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

    Public rk As StrReparti

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))

        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSReparti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        lblLabelTipoRK.Text = "Gestione Reparti"
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
            ddlReparti.Items.Clear()
            ddlReparti.Items.Add("")
            ddlReparti.DataBind()
        End If
        ModalPopup.WucElement = Me
    End Sub

    Public Sub SvuotaCampi()
        Session(IDREPARTO) = ""
        ddlReparti.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKREPARTI) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvReparti As DataView
        dvReparti = SqlDSReparti.Select(DataSourceSelectArguments.Empty)
        If (dvReparti Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvReparti.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvReparti.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvReparti.Count > 0 Then
                Session(IDREPARTO) = dvReparti.Item(0).Item("Codice")
                txtCodice.Text = dvReparti.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvReparti.Item(0).Item("Descrizione").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.IDReparto = IIf(IsNumeric(Session(IDREPARTO)), Session(IDREPARTO), 0)
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

        rk.Descrizione = Mid(txtDescrizione.Text.Trim, 1, 50)
        Session(RKREPARTI) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlReparti.SelectedIndex = 0
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
        End If
        If String.IsNullOrEmpty(myIDMag) Then
            myIDMag = "0"
        End If
        Session(IDMAGAZZINO) = myIDMag 'fisso
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
            If ddlReparti.SelectedIndex > 0 Then
                Session(IDREPARTO) = IIf(IsNumeric(ddlReparti.SelectedValue), ddlReparti.SelectedValue, 0)
            Else
                Session(IDREPARTO) = "0"
            End If
            SqlDSReparti.UpdateParameters.Item("Magazzino").DefaultValue = CLng(myIDMag)
            SqlDSReparti.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            'muove null SqlDSReparti.UpdateParameters.Item("Cod_Utente").DefaultValue = DBNull.Value
            SqlDSReparti.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSReparti.Update()
            SqlDSReparti.DataBind()
            '-----
            ddlReparti.Items.Clear()
            ddlReparti.Items.Add("")
            ddlReparti.DataBind()
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

    Private Sub ddlReparti_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlReparti.SelectedIndexChanged
        Session(IDREPARTO) = IIf(IsNumeric(ddlReparti.SelectedValue), ddlReparti.SelectedValue, 0)
        If ddlReparti.SelectedIndex = 0 Then
            Session(IDREPARTO) = "0"
            txtCodice.Text = Session(IDREPARTO)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDREPARTO)
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
        Dim dvReparti As DataView
        dvReparti = SqlDSReparti.Select(DataSourceSelectArguments.Empty)
        If (dvReparti Is Nothing) Then
            Exit Sub
        End If
        If dvReparti.Count > 0 Then
            dvReparti.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvReparti.Count > 0 Then
                Session(IDREPARTO) = dvReparti.Item(0).Item("Codice")
                txtCodice.Text = dvReparti.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvReparti.Item(0).Item("Descrizione").ToString.Trim
            End If
        End If
    End Sub

    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        If ddlMagazzino.SelectedIndex > 0 Then
            Session(IDMAGAZZINO) = ddlMagazzino.SelectedValue
        Else
            Session(IDMAGAZZINO) = -1
        End If

        ddlReparti.Items.Clear()
        ddlReparti.Items.Add("")
        ddlReparti.DataBind()
        txtCodice.Text = ""
        txtDescrizione.Text = ""
    End Sub
End Class