Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Categorie
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

    Public rk As StrCategorie

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSCategorie.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        lblLabelTipoRK.Text = "Gestione anagrafiche Categorie clienti"
        If (Not IsPostBack) Then
            ddlCategorie.Items.Clear()
            ddlCategorie.Items.Add("")
            ddlCategorie.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(IDCATEGORIE) = ""
        ddlCategorie.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        chkInvioMailSc.Checked = False
        chkSelSc.Checked = False
        rk = Nothing
        Session(RKCATEGORIE) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvCategorie As DataView
        dvCategorie = SqlDSCategorie.Select(DataSourceSelectArguments.Empty)
        If (dvCategorie Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvCategorie.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvCategorie.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvCategorie.Count > 0 Then
                Session(IDCATEGORIE) = dvCategorie.Item(0).Item("Codice")
                txtCodice.Text = dvCategorie.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvCategorie.Item(0).Item("Descrizione").ToString.Trim
                chkInvioMailSc.Checked = CBool(dvCategorie.Item(0).Item("InvioMailSc"))
                chkSelSc.Checked = CBool(dvCategorie.Item(0).Item("SelSc"))
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.IDCategorie = IIf(IsNumeric(Session(IDCATEGORIE)), Session(IDCATEGORIE), 0)
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
        Session(RKCATEGORIE) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlCategorie.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = _ID.ToString.Trim
        txtDescrizione.Text = ""
        txtDescrizione.Focus()
        chkInvioMailSc.Checked = False
        chkSelSc.Checked = False
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
            If ddlCategorie.SelectedIndex > 0 Then
                Session(IDCATEGORIE) = IIf(IsNumeric(ddlCategorie.SelectedValue), ddlCategorie.SelectedValue, 0)
            Else
                Session(IDCATEGORIE) = "0"
            End If
            SqlDSCategorie.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSCategorie.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSCategorie.UpdateParameters.Item("InvioMailSc").DefaultValue = CBool(chkInvioMailSc.Checked)
            SqlDSCategorie.UpdateParameters.Item("SelSc").DefaultValue = CBool(chkSelSc.Checked)
            SqlDSCategorie.Update()
            SqlDSCategorie.DataBind()
            '-----
            ddlCategorie.Items.Clear()
            ddlCategorie.Items.Add("")
            ddlCategorie.DataBind()
            PopolaEntityDati()
            Dim strErrore As String = ""
            App.CaricaCategorie(Session(ESERCIZIO), strErrore) 'pier110112 AGGIORNO LA CACHE
            If strErrore.Trim <> "" Then
                Call Chiudi("Errore caricamento dati in memoria temporanea" + strErrore)
            End If
        Catch Ex As Exception
            Call Chiudi("Errore in aggiorna: " + Ex.Message)
            Return False
        End Try

    End Function
    Private Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        End If
    End Sub

    Private Sub ddlCategorie_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCategorie.SelectedIndexChanged
        Session(IDCATEGORIE) = IIf(IsNumeric(ddlCategorie.SelectedValue), ddlCategorie.SelectedValue, 0)
        If ddlCategorie.SelectedIndex = 0 Then
            Session(IDCATEGORIE) = "0"
            txtCodice.Text = Session(IDCATEGORIE)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDCATEGORIE)
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
        Dim dvCategorie As DataView
        dvCategorie = SqlDSCategorie.Select(DataSourceSelectArguments.Empty)
        If (dvCategorie Is Nothing) Then
            Exit Sub
        End If
        If dvCategorie.Count > 0 Then
            dvCategorie.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvCategorie.Count > 0 Then
                Session(IDCATEGORIE) = dvCategorie.Item(0).Item("Codice")
                txtCodice.Text = dvCategorie.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvCategorie.Item(0).Item("Descrizione").ToString.Trim
                chkInvioMailSc.Checked = dvCategorie.Item(0).Item("InvioMailSc")
                chkSelSc.Checked = dvCategorie.Item(0).Item("SelSc")
            End If
        End If
    End Sub

    Private Sub chkInvioMailSc_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkInvioMailSc.CheckedChanged
        If chkInvioMailSc.Checked = False Then
            chkSelSc.Checked = False
        End If
    End Sub

    Private Sub chkSelSc_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelSc.CheckedChanged
        If chkInvioMailSc.Checked = False Then
            chkSelSc.Checked = False
        End If
    End Sub
End Class