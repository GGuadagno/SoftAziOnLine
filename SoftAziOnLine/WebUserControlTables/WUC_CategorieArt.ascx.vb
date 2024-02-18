Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_CategorieArt
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

    Public rk As StrCategorieArt

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSCategorieArt.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        lblLabelTipoRK.Text = "Gestione anagrafiche Categorie articoli"
        If (Not IsPostBack) Then
            ddlCategorie.Items.Clear()
            ddlCategorie.Items.Add("")
            ddlCategorie.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(CODICECATART) = ""
        ddlCategorie.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        TxtContoRicavo.Text = "" : TxtContoRicavo.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKCATEGORIEART) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvCategorie As DataView
        dvCategorie = SqlDSCategorieArt.Select(DataSourceSelectArguments.Empty)
        If (dvCategorie Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvCategorie.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvCategorie.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvCategorie.Count > 0 Then
                Session(CODICECATART) = dvCategorie.Item(0).Item("Codice")
                txtCodice.Text = dvCategorie.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvCategorie.Item(0).Item("Descrizione").ToString.Trim
                TxtContoRicavo.Text = dvCategorie.Item(0).Item("ContoRicavo").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.Codice = Session(CODICECATART)
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
        TxtContoRicavo.BackColor = SEGNALA_OK

        If PopolaEntityDati = False Then Exit Function

        rk.Descrizione = Mid(txtDescrizione.Text.Trim, 1, 50)
        rk.ContoRicavo = Mid(TxtContoRicavo.Text.Trim, 1, 16)
        Session(RKCATEGORIEART) = rk
    End Function

    Public Sub SetNewCodice()
        ddlCategorie.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = ""
        txtDescrizione.Text = ""
        TxtContoRicavo.Text = ""
        txtCodice.Focus()
    End Sub
    ' ''Public Function GetNewCodice() As Int32
    ' ''    If txtCodice.Text.Trim = "" Or Not IsNumeric(txtCodice.Text.Trim) Then
    ' ''        GetNewCodice = 0
    ' ''        Exit Function
    ' ''    End If
    ' ''    GetNewCodice = CLng(txtCodice.Text.Trim)
    ' ''End Function

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
        TxtContoRicavo.BackColor = SEGNALA_OK

        If Aggiorna = False Then Exit Function

        Try
            If ddlCategorie.SelectedIndex > 0 Then
                Session(CODICECATART) = IIf(IsNumeric(ddlCategorie.SelectedValue), ddlCategorie.SelectedValue, 0)
            Else
                Session(CODICECATART) = ""
            End If
            SqlDSCategorieArt.UpdateParameters.Item("Codice").DefaultValue = txtCodice.Text.Trim
            SqlDSCategorieArt.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSCategorieArt.UpdateParameters.Item("ContoRicavo").DefaultValue = Mid(TxtContoRicavo.Text.Trim, 1, 16)
            SqlDSCategorieArt.Update()
            SqlDSCategorieArt.DataBind()
            '-----
            ddlCategorie.Items.Clear()
            ddlCategorie.Items.Add("")
            ddlCategorie.DataBind()
            PopolaEntityDati()
            '' ''Dim strErrore As String = ""
            '' ''App.CaricaCategorie(Session(ESERCIZIO), strErrore) 'pier110112 AGGIORNO LA CACHE
            '' ''If strErrore.Trim <> "" Then
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '' ''    ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            '' ''End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlCategorie_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCategorie.SelectedIndexChanged
        Session(CODICECATART) = IIf(IsNumeric(ddlCategorie.SelectedValue), ddlCategorie.SelectedValue, 0)
        If ddlCategorie.SelectedIndex = 0 Then
            Session(CODICECATART) = ""
            txtCodice.Text = Session(CODICECATART)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(CODICECATART)
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
        dvCategorie = SqlDSCategorieArt.Select(DataSourceSelectArguments.Empty)
        If (dvCategorie Is Nothing) Then
            Exit Sub
        End If
        If dvCategorie.Count > 0 Then
            dvCategorie.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvCategorie.Count > 0 Then
                Session(CODICECATART) = dvCategorie.Item(0).Item("Codice")
                txtCodice.Text = dvCategorie.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvCategorie.Item(0).Item("Descrizione").ToString.Trim
                TxtContoRicavo.Text = dvCategorie.Item(0).Item("ContoRicavo").ToString.Trim
            End If
        End If
    End Sub
End Class