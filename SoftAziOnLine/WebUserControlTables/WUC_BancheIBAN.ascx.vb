Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_BancheIBAN
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

    Public rk As StrBancheIBAN

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSBancheIBAN.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If Session(TIPORK) = "C" Then
            lblLabelTipoRK.Text = "Gestione Banca d'appoggio Clienti"
        ElseIf Session(TIPORK) = "F" Then
            lblLabelTipoRK.Text = "Gestione Banca d'appoggio Fornitori"
        ElseIf Session(TIPORK) = "A" Then
            lblLabelTipoRK.Text = "Gestione Banca d'appoggio Azienda"
        Else
            If Session(CSTTABCLIFOR) = "Cli" Then
                Session(TIPORK) = "A" 'BANCHE AZIENDA che usa il programma
                lblLabelTipoRK.Text = "Gestione Banca d'appoggio Azienda"
            ElseIf Session(CSTTABCLIFOR) = "For" Then
                Session(TIPORK) = "F"
                lblLabelTipoRK.Text = "Gestione Banca d'appoggio Fornitori"
            Else 'sessione scaduta??
                lblLabelTipoRK.Text = "Gestione Banca d'appoggio"
            End If
        End If
        If (Not IsPostBack) Then
            ddlBancheIBAN.Items.Clear()
            ddlBancheIBAN.Items.Add("")
            ddlBancheIBAN.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(IDBANCHEIBAN) = ""
        ddlBancheIBAN.SelectedIndex = 0
        TxtIBAN.Text = "" : TxtIBAN.BackColor = SEGNALA_OK
        TxtDescrizione.Text = "" : TxtDescrizione.BackColor = SEGNALA_OK
        TxtABI.Text = ""
        TxtCAB.Text = ""
        TxtContoCorrente.Text = ""
        rk = Nothing
        Session(RKBANCHEIBAN) = rk
    End Sub

    Public Function PopolaEntityDatiBancheIBAN() As Boolean
        PopolaEntityDatiBancheIBAN = True
        Dim dvBancheIBAN As DataView
        dvBancheIBAN = SqlDSBancheIBAN.Select(DataSourceSelectArguments.Empty)
        If (dvBancheIBAN Is Nothing) Then
            SvuotaCampi()
            Session(IDBANCHEIBAN) = ""
            Exit Function
        End If
        If dvBancheIBAN.Count > 0 Then
            dvBancheIBAN.RowFilter = "IBAN = '" & TxtIBAN.Text.Trim & "'"
            If dvBancheIBAN.Count > 0 Then
                Session(IDBANCHEIBAN) = dvBancheIBAN.Item(0).Item("IBAN")
                TxtDescrizione.Text = dvBancheIBAN.Item(0).Item("Descrizione").ToString
                TxtABI.Text = dvBancheIBAN.Item(0).Item("ABI").ToString
                TxtCAB.Text = dvBancheIBAN.Item(0).Item("CAB").ToString
                TxtContoCorrente.Text = dvBancheIBAN.Item(0).Item("ContoCorrente").ToString
            Else
                SvuotaCampi()
                Session(IDBANCHEIBAN) = ""
                Exit Function
            End If
        Else
            SvuotaCampi()
            Session(IDBANCHEIBAN) = ""
            Exit Function
        End If

        rk.IBAN = Session(IDBANCHEIBAN)

        If TxtIBAN.Text.Trim = "" Then
            TxtIBAN.BackColor = SEGNALA_KO : PopolaEntityDatiBancheIBAN = False
        Else
            TxtIBAN.BackColor = SEGNALA_OK
        End If
        If TxtDescrizione.Text.Trim = "" Then
            TxtDescrizione.BackColor = SEGNALA_KO : PopolaEntityDatiBancheIBAN = False
        Else
            TxtDescrizione.BackColor = SEGNALA_OK
        End If
        If PopolaEntityDatiBancheIBAN = False Then Exit Function
        rk.Descrizione = Mid(TxtDescrizione.Text.Trim, 1, 50)
        rk.ABI = Mid(TxtABI.Text.Trim, 1, 5)
        rk.CAB = Mid(TxtCAB.Text.Trim, 1, 5)
        rk.ContoCorrente = Mid(TxtContoCorrente.Text.Trim, 1, 12)
        rk.Tipo = Session(TIPORK) 'giu121011 Mid(Session(CSTTABCLIFOR), 1, 1)
        Session(RKBANCHEIBAN) = rk
    End Function

    Public Function AggiornaBancheIBAN() As Boolean
        AggiornaBancheIBAN = True

        If TxtIBAN.Text.Trim = "" Then
            TxtIBAN.BackColor = SEGNALA_KO : AggiornaBancheIBAN = False
        Else
            TxtIBAN.BackColor = SEGNALA_OK
        End If
        If TxtDescrizione.Text.Trim = "" Then
            TxtDescrizione.BackColor = SEGNALA_KO : AggiornaBancheIBAN = False
        Else
            TxtDescrizione.BackColor = SEGNALA_OK
        End If
        If AggiornaBancheIBAN = False Then Exit Function

        Try
            If ddlBancheIBAN.SelectedIndex > 0 Then
                Session(IDBANCHEIBAN) = ddlBancheIBAN.SelectedValue
            Else
                Session(IDBANCHEIBAN) = ""
            End If
            SqlDSBancheIBAN.UpdateParameters.Item("IBAN").DefaultValue = Mid(TxtIBAN.Text.Trim, 1, 27)
            SqlDSBancheIBAN.UpdateParameters.Item("Descrizione").DefaultValue = Mid(TxtDescrizione.Text.Trim, 1, 50)
            SqlDSBancheIBAN.UpdateParameters.Item("ABI").DefaultValue = Mid(TxtABI.Text.Trim, 1, 5)
            SqlDSBancheIBAN.UpdateParameters.Item("CAB").DefaultValue = Mid(TxtCAB.Text.Trim, 1, 5)
            SqlDSBancheIBAN.UpdateParameters.Item("ContoCorrente").DefaultValue = Mid(TxtContoCorrente.Text.Trim, 1, 12)
            SqlDSBancheIBAN.UpdateParameters.Item("Tipo").DefaultValue = Session(TIPORK) 'giu121011 Mid(Session(CSTTABCLIFOR), 1, 1)
            SqlDSBancheIBAN.Update()
            SqlDSBancheIBAN.DataBind()
            '-----
            ddlBancheIBAN.Items.Clear()
            ddlBancheIBAN.Items.Add("")
            ddlBancheIBAN.DataBind()
            PopolaEntityDatiBancheIBAN()
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlBancheIBAN_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlBancheIBAN.SelectedIndexChanged
        Session(IDBANCHEIBAN) = ddlBancheIBAN.SelectedValue
        If ddlBancheIBAN.SelectedIndex = 0 Then Session(IDBANCHEIBAN) = ""
        Dim dvBancheIBAN As DataView
        dvBancheIBAN = SqlDSBancheIBAN.Select(DataSourceSelectArguments.Empty)
        If (dvBancheIBAN Is Nothing) Then
            SvuotaCampi()
            Session(IDBANCHEIBAN) = ""
            Exit Sub
        End If
        If dvBancheIBAN.Count > 0 Then
            dvBancheIBAN.RowFilter = "IBAN = '" & Session(IDBANCHEIBAN) & "'"
            If dvBancheIBAN.Count > 0 Then
                TxtIBAN.Text = dvBancheIBAN.Item(0).Item("IBAN").ToString
                TxtDescrizione.Text = dvBancheIBAN.Item(0).Item("Descrizione").ToString
                TxtABI.Text = dvBancheIBAN.Item(0).Item("ABI").ToString
                TxtCAB.Text = dvBancheIBAN.Item(0).Item("CAB").ToString
                TxtContoCorrente.Text = dvBancheIBAN.Item(0).Item("ContoCorrente").ToString
                PopolaEntityDatiBancheIBAN()
            Else
                SvuotaCampi()
                Session(IDBANCHEIBAN) = ""
                Exit Sub
            End If
        Else
            SvuotaCampi()
            Session(IDBANCHEIBAN) = ""
        End If
    End Sub

    Private Sub txtIBAN_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtIBAN.TextChanged
        Dim dvBancheIBAN As DataView
        dvBancheIBAN = SqlDSBancheIBAN.Select(DataSourceSelectArguments.Empty)
        If (dvBancheIBAN Is Nothing) Then
            TxtIBAN.Focus()
            Exit Sub
        End If
        If dvBancheIBAN.Count > 0 Then
            dvBancheIBAN.RowFilter = "IBAN = '" & Controlla_Apice(TxtIBAN.Text.Trim) & "'"
            If dvBancheIBAN.Count > 0 Then
                Session(IDBANCHEIBAN) = dvBancheIBAN.Item(0).Item("IBAN")
                TxtDescrizione.Text = dvBancheIBAN.Item(0).Item("Descrizione").ToString
                TxtABI.Text = dvBancheIBAN.Item(0).Item("ABI").ToString
                TxtCAB.Text = dvBancheIBAN.Item(0).Item("CAB").ToString
                TxtContoCorrente.Text = dvBancheIBAN.Item(0).Item("ContoCorrente").ToString
            End If
        End If
        TxtDescrizione.Focus()
    End Sub
    Private Sub txtDescrizione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtDescrizione.TextChanged
        Dim dvBancheIBAN As DataView
        dvBancheIBAN = SqlDSBancheIBAN.Select(DataSourceSelectArguments.Empty)
        If (dvBancheIBAN Is Nothing) Then
            TxtIBAN.Focus()
            Exit Sub
        End If
        If dvBancheIBAN.Count > 0 Then
            dvBancheIBAN.RowFilter = "Descrizione = '" & Controlla_Apice(TxtDescrizione.Text.Trim) & "'"
            If dvBancheIBAN.Count > 0 Then
                Session(IDBANCHEIBAN) = dvBancheIBAN.Item(0).Item("IBAN")
                TxtDescrizione.Text = dvBancheIBAN.Item(0).Item("Descrizione").ToString
                TxtABI.Text = dvBancheIBAN.Item(0).Item("ABI").ToString
                TxtCAB.Text = dvBancheIBAN.Item(0).Item("CAB").ToString
                TxtContoCorrente.Text = dvBancheIBAN.Item(0).Item("ContoCorrente").ToString
            End If
        End If
        TxtABI.Focus()
    End Sub
End Class