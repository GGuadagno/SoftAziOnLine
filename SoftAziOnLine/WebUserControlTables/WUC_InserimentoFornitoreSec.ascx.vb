Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_InserimentoFornitoreSec
    Inherits System.Web.UI.UserControl

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

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SqlDataSourceFornitore.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceCondizioniPag.ConnectionString = Session(DBCONNAZI)

        If (Not IsPostBack) Then
            CaricaVariabili()
        End If
    End Sub

    Private Sub ddlFornitore_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlFornitore.SelectedIndexChanged
        txtCodFornitore.Text = ddlFornitore.SelectedValue
        txtCodFornitore.BackColor = SEGNALA_OK
        ddlFornitore.BackColor = SEGNALA_OK
    End Sub

    Private Sub ddlCondizioniPag_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCondizioniPag.SelectedIndexChanged
        txtCodCondizioniPag.Text = ddlCondizioniPag.SelectedValue
        txtCodCondizioniPag.BackColor = SEGNALA_OK
        ddlCondizioniPag.BackColor = SEGNALA_OK
    End Sub

    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        PosizionaItemDDLTxt(txtCodFornitore, ddlFornitore)
    End Sub

    Private Sub txtCodCondizioniPag_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCondizioniPag.TextChanged
        ControllaValoreNumerico(txtCodCondizioniPag, 0)
        If (txtCodCondizioniPag.BackColor = Def.SEGNALA_OK) Then
            PosizionaItemDDLTxt(txtCodCondizioniPag, ddlCondizioniPag)
        End If
    End Sub

    Private Sub txtGiorniConsegna_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGiorniConsegna.TextChanged
        ControllaValoreNumerico(txtGiorniConsegna, App.GetParamGestAzi(Session(esercizio)).Decimali_Grandezze)
        txtPrezzo.Focus()
    End Sub

    Private Sub txtPrezzo_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPrezzo.TextChanged
        ControllaValoreNumerico(txtPrezzo, App.GetParamGestAzi(Session(esercizio)).Decimali_Prezzi)
    End Sub

#End Region

#Region "Gestione variabili di sessione interne"

    Private Sub CaricaVariabili()
        Session(SWERRORI_AGGIORNAMENTO) = False
    End Sub

#End Region

#Region "Metodi private"

    Private Sub ControllaValoreNumerico(ByRef txt As TextBox, ByVal nDecimali As Integer)
        txt.BackColor = IIf(IsNumeric(txt.Text), Def.SEGNALA_OK, Def.SEGNALA_KO)
        If (txt.BackColor = Def.SEGNALA_KO) Then
            Session(SWERRORI_AGGIORNAMENTO) = True
        Else
            txt.Text = FormattaNumero(txt.Text, nDecimali)
        End If
    End Sub

    Private Sub ControllaEsistenzaCampiInErrore()
        If ( _
            txtCodFornitore.BackColor = Def.SEGNALA_KO Or _
            txtCodCondizioniPag.BackColor = Def.SEGNALA_KO Or _
            txtGiorniConsegna.BackColor = Def.SEGNALA_KO Or _
            txtPrezzo.BackColor = Def.SEGNALA_KO _
            ) Then
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
    End Sub

    Private Sub ValorizzaTextBoxCombinata(ByVal valore As String, ByRef txt As TextBox, ByRef ddl As DropDownList)
        txt.Text = valore
        ddl.Items.Clear()
        ddl.Items.Add("")
        ddl.DataBind()
        PosizionaItemDDLTxt(txt, ddl)
        If (txt.BackColor = SEGNALA_KO) Then
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
    End Sub

#End Region

#Region "Metodi public"

    Public Sub SvuotaCampi()
        CaricaVariabili()
        txtCodFornitore.Text = String.Empty
        txtCodCondizioniPag.Text = FormattaNumero("0")
        lbTitolare.Text = String.Empty
        lbRiferimento.Text = String.Empty
        txtGiorniConsegna.Text = FormattaNumero("0", App.GetParamGestAzi(Session(esercizio)).Decimali_Grandezze)
        txtPrezzo.Text = FormattaNumero("0", App.GetParamGestAzi(Session(esercizio)).Decimali_Prezzi)
        ddlFornitore.SelectedValue = String.Empty
        ddlCondizioniPag.SelectedValue = String.Empty
    End Sub

    Public Function PopolaEntityDatiFornitoreSec() As Boolean
        If (String.IsNullOrEmpty(txtCodFornitore.Text)) Then
            txtCodFornitore.BackColor = SEGNALA_KO
            Session(SWERRORI_AGGIORNAMENTO) = True
        End If
        ControllaEsistenzaCampiInErrore()
        If (Not Session(SWERRORI_AGGIORNAMENTO)) Then
            Dim myFornSec As New FornSecondariEntity
            myFornSec.CodFornitore = txtCodFornitore.Text
            myFornSec.RagSoc = ddlFornitore.SelectedItem.Text
            myFornSec.GiorniConsegna = CInt(txtGiorniConsegna.Text)
            myFornSec.CodPagamento = CInt(txtCodCondizioniPag.Text)
            myFornSec.UltPrezzo = CDec(txtPrezzo.Text)
            myFornSec.Titolare = lbTitolare.Text
            myFornSec.Riferimento = lbRiferimento.Text
            Session(FORNITORE_SEC) = myFornSec
            SvuotaCampi()
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati fornitore secondario", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_INFO)
            PopolaEntityDatiFornitoreSec = False
            Exit Function
        End If
        PopolaEntityDatiFornitoreSec = True
    End Function

#End Region

End Class