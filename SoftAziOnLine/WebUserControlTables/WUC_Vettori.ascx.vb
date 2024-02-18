Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Vettori
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

    Public rk As StrVettori

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSVettori.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        lblLabelTipoRK.Text = "Gestione anagrafiche Vettori"
        If (Not IsPostBack) Then
            ddlVettori.Items.Clear()
            ddlVettori.Items.Add("")
            ddlVettori.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(IDVETTORI) = ""
        ddlVettori.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        TxtResidenza.Text = ""
        TxtLocalita.Text = ""
        TxtProvincia.Text = "" : TxtProvincia.BackColor = SEGNALA_OK
        txtPartitaIVA.Text = ""
        txtCodice_CoGe.Text = ""
        rk = Nothing
        Session(RKVETTORI) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvVettori As DataView
        dvVettori = SqlDSVettori.Select(DataSourceSelectArguments.Empty)
        If (dvVettori Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvVettori.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvVettori.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvVettori.Count > 0 Then
                Session(IDVETTORI) = dvVettori.Item(0).Item("Codice")
                txtCodice.Text = dvVettori.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvVettori.Item(0).Item("Descrizione").ToString.Trim
                TxtResidenza.Text = dvVettori.Item(0).Item("Residenza").ToString.Trim
                TxtLocalita.Text = dvVettori.Item(0).Item("Localita").ToString.Trim
                TxtProvincia.Text = dvVettori.Item(0).Item("Provincia").ToString.Trim
                txtPartitaIVA.Text = dvVettori.Item(0).Item("Partita_IVA").ToString.Trim
                txtCodice_CoGe.Text = dvVettori.Item(0).Item("Codice_CoGe").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.IDVettori = IIf(IsNumeric(Session(IDVETTORI)), Session(IDVETTORI), 0)
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
        rk.Residenza = Mid(TxtResidenza.Text.Trim, 1, 50)
        rk.Localita = Mid(TxtLocalita.Text.Trim, 1, 50)
        rk.Provincia = Mid(TxtProvincia.Text.Trim, 1, 2)
        rk.Partita_IVA = Mid(txtPartitaIVA.Text.Trim, 1, 16)
        rk.Codice_CoGe = Mid(txtCodice_CoGe.Text.Trim, 1, 16)
        Session(RKVETTORI) = rk
    End Function


    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlVettori.SelectedIndex = 0
        txtCodice.Enabled = True
        txtCodice.Text = _ID.ToString.Trim
        txtDescrizione.Text = ""
        TxtResidenza.Text = ""
        TxtLocalita.Text = ""
        TxtProvincia.Text = ""
        txtPartitaIVA.Text = ""
        txtCodice_CoGe.Text = ""
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
        If TxtProvincia.Text.Trim <> "" Then
            If App.GetValoreFromChiave(TxtProvincia.Text, Def.PROVINCE, Session(ESERCIZIO)) = "" Then
                TxtProvincia.BackColor = SEGNALA_KO
                Aggiorna = False
            Else
                TxtProvincia.BackColor = SEGNALA_OK
            End If
        Else
            TxtProvincia.BackColor = SEGNALA_OK
        End If
        If Aggiorna = False Then Exit Function

        Try
            If ddlVettori.SelectedIndex > 0 Then
                Session(IDVETTORI) = IIf(IsNumeric(ddlVettori.SelectedValue), ddlVettori.SelectedValue, 0)
            Else
                Session(IDVETTORI) = "0"
            End If
            
            SqlDSVettori.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSVettori.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSVettori.UpdateParameters.Item("Residenza").DefaultValue = Mid(TxtResidenza.Text.Trim, 1, 50)
            SqlDSVettori.UpdateParameters.Item("Localita").DefaultValue = Mid(TxtLocalita.Text.Trim, 1, 50)
            SqlDSVettori.UpdateParameters.Item("Provincia").DefaultValue = Mid(TxtProvincia.Text.Trim, 1, 2)
            SqlDSVettori.UpdateParameters.Item("Partita_IVA").DefaultValue = Mid(txtPartitaIVA.Text.Trim, 1, 16)
            SqlDSVettori.UpdateParameters.Item("Codice_Coge").DefaultValue = Mid(txtCodice_CoGe.Text.Trim, 1, 16)
            SqlDSVettori.Update()
            SqlDSVettori.DataBind()
            '---------------------------------------------
            Dim strSQL As String = "UPDATE Vettori SET Descrizione='" + Controlla_Apice(Mid(txtDescrizione.Text.Trim, 1, 50)) + "', " + _
                "Residenza='" & Controlla_Apice(Mid(TxtResidenza.Text.Trim, 1, 50)) & "', " & _
                "Localita='" & Controlla_Apice(Mid(TxtLocalita.Text.Trim, 1, 50)) & "', " & _
                "Provincia='" & Controlla_Apice(Mid(TxtProvincia.Text.Trim, 1, 2)) & "', " & _
                "Partita_IVA='" & Controlla_Apice(Mid(txtPartitaIVA.Text.Trim, 1, 16)) & "', " & _
                "Codice_Coge='" & Controlla_Apice(Mid(txtCodice_CoGe.Text.Trim, 1, 16)) & "' " & _
                "WHERE Codice = " & txtCodice.Text.Trim
            '---------
            Dim ObjDB As New DataBaseUtility
            Dim strerrore As String = ""
            Dim SWOK As Boolean = False
            Try
                SWOK = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftCoge, strSQL)
                If SWOK = False Then
                    ObjDB = Nothing
                    strErrore = "Errore: Si è verificato un errore durante l'aggiornamento testata (UpgStatoDocEmail)"
                    Exit Function
                End If
            Catch Ex As Exception
                strerrore = "Errore: Si è verificato un errore durante l'aggiornamento - " & Ex.Message.Trim
                Exit Function
            End Try
            ObjDB = Nothing
            SqlDSVettori.DataBind()
            '-----
            ddlVettori.Items.Clear()
            ddlVettori.Items.Add("")
            ddlVettori.DataBind()
            PopolaEntityDati()
            'Dim strErrore As String = ""
            App.CaricaVettori(Session(ESERCIZIO), strErrore) 'giu040112 AGGIORNO LA CACHE
            If strErrore.Trim <> "" Then
                ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch Ex As Exception
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function
   
    Private Sub ddlVettori_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlVettori.SelectedIndexChanged
        Session(IDVETTORI) = IIf(IsNumeric(ddlVettori.SelectedValue), ddlVettori.SelectedValue, 0)
        If ddlVettori.SelectedIndex = 0 Then
            Session(IDVETTORI) = "0"
            txtCodice.Text = Session(IDVETTORI)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDVETTORI)
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
        Dim dvVettori As DataView
        dvVettori = SqlDSVettori.Select(DataSourceSelectArguments.Empty)
        If (dvVettori Is Nothing) Then
            TxtResidenza.Focus()
            Exit Sub
        End If
        If dvVettori.Count > 0 Then
            dvVettori.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvVettori.Count > 0 Then
                Session(IDVETTORI) = dvVettori.Item(0).Item("Codice")
                txtCodice.Text = dvVettori.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = dvVettori.Item(0).Item("Descrizione").ToString.Trim
                TxtResidenza.Text = dvVettori.Item(0).Item("Residenza").ToString.Trim
                TxtLocalita.Text = dvVettori.Item(0).Item("Localita").ToString.Trim
                TxtProvincia.Text = dvVettori.Item(0).Item("Provincia").ToString.Trim
                txtPartitaIVA.Text = dvVettori.Item(0).Item("Partita_IVA").ToString.Trim
                txtCodice_CoGe.Text = dvVettori.Item(0).Item("Codice_CoGe").ToString.Trim
            End If
        End If
        TxtResidenza.Focus()
    End Sub
End Class