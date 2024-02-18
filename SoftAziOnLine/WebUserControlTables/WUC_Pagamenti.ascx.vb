Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Partial Public Class WUC_Pagamenti
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

    Public rk As StrPagamenti

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSPagamenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSIVA.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        lblLabelTipoRK.Text = "Condizioni di pagamento"
        LblErrore.Text = ""
        If (Not IsPostBack) Then
            ddlPagamenti.Items.Clear()
            ddlPagamenti.Items.Add("")
            ddlPagamenti.DataBind()
            CaricaCombo()
        End If
    End Sub
    Private Sub CaricaCombo()
        ddlEscludiMese1.Items.Add("")
        ddlEscludiMese1.Items.Add("Gennaio")
        ddlEscludiMese1.Items.Add("Febbraio")
        ddlEscludiMese1.Items.Add("Marzo")
        ddlEscludiMese1.Items.Add("Aprile")
        ddlEscludiMese1.Items.Add("Maggio")
        ddlEscludiMese1.Items.Add("Giugno")
        ddlEscludiMese1.Items.Add("Luglio")
        ddlEscludiMese1.Items.Add("Agosto")
        ddlEscludiMese1.Items.Add("Settembre")
        ddlEscludiMese1.Items.Add("Ottobre")
        ddlEscludiMese1.Items.Add("Novembre")
        ddlEscludiMese1.Items.Add("Dicembre")

        ddlEscludiMese2.Items.Add("")
        ddlEscludiMese2.Items.Add("Gennaio")
        ddlEscludiMese2.Items.Add("Febbraio")
        ddlEscludiMese2.Items.Add("Marzo")
        ddlEscludiMese2.Items.Add("Aprile")
        ddlEscludiMese2.Items.Add("Maggio")
        ddlEscludiMese2.Items.Add("Giugno")
        ddlEscludiMese2.Items.Add("Luglio")
        ddlEscludiMese2.Items.Add("Agosto")
        ddlEscludiMese2.Items.Add("Settembre")
        ddlEscludiMese2.Items.Add("Ottobre")
        ddlEscludiMese2.Items.Add("Novembre")
        ddlEscludiMese2.Items.Add("Dicembre")

        Dim Itm As ListItem
        '=============================================
        Itm = New ListItem
        Itm.Text = "Rimessa diretta"
        Itm.Value = 0
        ddlTipoPagamento.Items.Add(Itm) '0
        '=============================================
        '=============================================
        Itm = New ListItem
        Itm.Text = "Ricevuta bancaria"
        Itm.Value = 1
        ddlTipoPagamento.Items.Add(Itm) '1
        '=============================================
        '=============================================
        Itm = New ListItem
        Itm.Text = "Bonifico"
        Itm.Value = 2
        ddlTipoPagamento.Items.Add(Itm) '2
        '=============================================

        '=============================================
        Itm = New ListItem
        Itm.Text = "Data fattura"
        Itm.Value = 0
        ddlTipoScadenza.Items.Add(Itm) '0
        '=============================================
        '=============================================
        Itm = New ListItem
        Itm.Text = "Fine mese"
        Itm.Value = 1
        ddlTipoScadenza.Items.Add(Itm) '1
        '=============================================
        '=============================================
        Itm = New ListItem
        Itm.Text = "Giorno fisso"
        Itm.Value = 2
        ddlTipoScadenza.Items.Add(Itm) '2
    End Sub

    Public Sub SvuotaCampi()
        Session(IDPAGAMENTO) = ""
        ddlPagamenti.SelectedIndex = 0
        txtCodice.Text = "" : txtCodice.BackColor = SEGNALA_OK
        txtCodice.Enabled = True
        txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        TxtNRate.Text = "" : TxtNRate.BackColor = SEGNALA_OK
        TxtNRateEffettive.Text = "" : TxtNRateEffettive.BackColor = SEGNALA_OK
        TxtScad1.Text = "" : TxtScad1.BackColor = SEGNALA_OK
        TxtScad2.Text = "" : TxtScad2.BackColor = SEGNALA_OK
        TxtScad3.Text = "" : TxtScad3.BackColor = SEGNALA_OK
        TxtScad4.Text = "" : TxtScad4.BackColor = SEGNALA_OK
        TxtScad5.Text = "" : TxtScad5.BackColor = SEGNALA_OK
        TxtPercImporto1.Text = "" : TxtPercImporto1.BackColor = SEGNALA_OK
        TxtPercImporto2.Text = "" : TxtPercImporto2.BackColor = SEGNALA_OK
        TxtPercImporto3.Text = "" : TxtPercImporto3.BackColor = SEGNALA_OK
        TxtPercImporto4.Text = "" : TxtPercImporto4.BackColor = SEGNALA_OK
        TxtPercImporto5.Text = "" : TxtPercImporto5.BackColor = SEGNALA_OK
        TxtPercImposta1.Text = "" : TxtPercImposta1.BackColor = SEGNALA_OK
        TxtPercImposta2.Text = "" : TxtPercImposta2.BackColor = SEGNALA_OK
        TxtPercImposta3.Text = "" : TxtPercImposta3.BackColor = SEGNALA_OK
        TxtPercImposta4.Text = "" : TxtPercImposta4.BackColor = SEGNALA_OK
        TxtPercImposta5.Text = "" : TxtPercImposta5.BackColor = SEGNALA_OK
        TxtSpeseIncasso.Text = "" : TxtSpeseIncasso.BackColor = SEGNALA_OK
        TxtScontoCassa.Text = "" : TxtScontoCassa.BackColor = SEGNALA_OK
        ddlTipoPagamento.SelectedIndex = -1 : ddlTipoPagamento.BackColor = SEGNALA_OK
        ddlTipoScadenza.SelectedIndex = -1 : ddlTipoScadenza.BackColor = SEGNALA_OK
        ddlEscludiMese1.SelectedIndex = -1 : ddlEscludiMese1.BackColor = SEGNALA_OK
        ddlEscludiMese2.SelectedIndex = -1 : ddlEscludiMese2.BackColor = SEGNALA_OK
        ddlIVASpese.SelectedIndex = -1 : ddlIVASpese.BackColor = SEGNALA_OK

        rk = Nothing
        Session(RKPAGAMENTI) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvPagamenti As DataView
        dvPagamenti = SqlDSPagamenti.Select(DataSourceSelectArguments.Empty)
        If (dvPagamenti Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If
        If dvPagamenti.Count > 0 Then
            If (txtCodice.Text.Trim = "") Or (Not IsNumeric(txtCodice.Text.Trim)) Then txtCodice.Text = "0"
            dvPagamenti.RowFilter = "Codice = " & txtCodice.Text.Trim
            If dvPagamenti.Count > 0 Then
                Session(IDPAGAMENTO) = dvPagamenti.Item(0).Item("Codice")
                txtCodice.Text = dvPagamenti.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Descrizione")), dvPagamenti.Item(0).Item("Descrizione").ToString.Trim, "")
                TxtNRate.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Numero_Rate")), dvPagamenti.Item(0).Item("Numero_Rate").ToString.Trim, "0")
                TxtNRateEffettive.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Numero_Rate_Effettive")), dvPagamenti.Item(0).Item("Numero_Rate_Effettive").ToString.Trim, "0")
                TxtScad1.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_1")), dvPagamenti.Item(0).Item("Scadenza_1").ToString.Trim, "0")
                TxtScad2.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_2")), dvPagamenti.Item(0).Item("Scadenza_2").ToString.Trim, "0")
                TxtScad3.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_3")), dvPagamenti.Item(0).Item("Scadenza_3").ToString.Trim, "0")
                TxtScad4.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_4")), dvPagamenti.Item(0).Item("Scadenza_4").ToString.Trim, "0")
                TxtScad5.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_5")), dvPagamenti.Item(0).Item("Scadenza_5").ToString.Trim, "0")
                TxtPercImporto1.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_1")), dvPagamenti.Item(0).Item("Perc_Imponib_1").ToString.Trim, "0")
                TxtPercImporto2.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_2")), dvPagamenti.Item(0).Item("Perc_Imponib_2").ToString.Trim, "0")
                TxtPercImporto3.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_3")), dvPagamenti.Item(0).Item("Perc_Imponib_3").ToString.Trim, "0")
                TxtPercImporto4.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_4")), dvPagamenti.Item(0).Item("Perc_Imponib_4").ToString.Trim, "0")
                TxtPercImporto5.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_5")), dvPagamenti.Item(0).Item("Perc_Imponib_5").ToString.Trim, "0")
                TxtPercImposta1.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_1")), dvPagamenti.Item(0).Item("Perc_Imposta_1").ToString.Trim, "0")
                TxtPercImposta2.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_2")), dvPagamenti.Item(0).Item("Perc_Imposta_2").ToString.Trim, "0")
                TxtPercImposta3.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_3")), dvPagamenti.Item(0).Item("Perc_Imposta_3").ToString.Trim, "0")
                TxtPercImposta4.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_4")), dvPagamenti.Item(0).Item("Perc_Imposta_4").ToString.Trim, "0")
                TxtPercImposta5.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_5")), dvPagamenti.Item(0).Item("Perc_Imposta_5").ToString.Trim, "0")
                TxtSpeseIncasso.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Spese_Incasso")), dvPagamenti.Item(0).Item("Spese_Incasso").ToString.Trim, "0")
                TxtScontoCassa.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Sconto_Cassa")), dvPagamenti.Item(0).Item("Sconto_Cassa").ToString.Trim, "0")
                If Not IsDBNull(dvPagamenti.Item(0).Item("Mese")) Then
                    If dvPagamenti.Item(0).Item("Mese") = 0 Then
                        optMeseInCorso.Checked = True
                        optMeseSuccessivo.Checked = False
                    Else
                        optMeseInCorso.Checked = False
                        optMeseSuccessivo.Checked = True
                    End If
                Else
                    optMeseInCorso.Checked = False
                    optMeseSuccessivo.Checked = False
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("Tipo_Pagamento")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("Tipo_Pagamento").ToString.Trim, ddlTipoPagamento)
                Else
                    ddlTipoPagamento.SelectedIndex = 0
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("Tipo_Scadenza")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("Tipo_Scadenza").ToString.Trim, ddlTipoScadenza)
                Else
                    ddlTipoScadenza.SelectedIndex = 0
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("Mese_Escluso_1")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("Mese_Escluso_1").ToString.Trim, ddlEscludiMese1)
                Else
                    ddlEscludiMese1.SelectedIndex = 0
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("Mese_Escluso_2")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("Mese_Escluso_2").ToString.Trim, ddlEscludiMese2)
                Else
                    ddlEscludiMese2.SelectedIndex = 0
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("IVA_Spese_Incasso")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("IVA_Spese_Incasso").ToString.Trim, ddlIVASpese)
                Else
                    ddlIVASpese.SelectedIndex = 0
                End If
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.IDPagamento = IIf(IsNumeric(Session(IDPAGAMENTO)), Session(IDPAGAMENTO), 0)
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

        ''per il momento non popolo gli altri campi, non servono...


        Session(RKPAGAMENTI) = rk
    End Function

    Public Sub SetNewCodice(ByVal _ID As Int32)
        ddlPagamenti.SelectedIndex = 0
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

        ' ''PIER!! Per il momento salva paro paro ciò che c'è nelle casella.
        ' ''Bisogna inserire le funzioni che generano le percentuali in base al 
        ' ''numero rate. IN VB6 venivano fatte nel lostfocus delle caselle di testo,
        ' ''inoltre bisogna continuare ad inserire i controlli prima di aggiornare
        ' ''Per quando va in errore ho inserito una label che indica l'errore verificatosi

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

        '-
        If ddlTipoPagamento.SelectedIndex < 0 Then
            ddlTipoPagamento.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            ddlTipoPagamento.BackColor = SEGNALA_OK
        End If

        '-
        If ddlTipoScadenza.SelectedIndex < 0 Then
            ddlTipoScadenza.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            ddlTipoScadenza.BackColor = SEGNALA_OK
        End If

        '-
        If (TxtNRate.Text = "") Or Not IsNumeric(TxtNRate.Text) Then
            TxtNRate.BackColor = SEGNALA_KO : Aggiorna = False
        Else
            TxtNRate.BackColor = SEGNALA_OK
        End If

        '-
        If (TxtNRate.Text = "") Or Not IsNumeric(TxtNRate.Text) Or (TxtNRate.Text = "0") Then
            TxtNRate.BackColor = SEGNALA_KO : Aggiorna = False
            LblErrore.Text = "Il numero di rate deve essere compreso tra 1 e 5."
            Exit Function
        Else
            TxtNRate.BackColor = SEGNALA_OK
        End If

        '-
        If (CInt(TxtNRate.Text) > 5) Then
            TxtNRate.BackColor = SEGNALA_KO : Aggiorna = False
            LblErrore.Text = "Il numero di rate deve essere compreso tra 1 e 5."
            Exit Function
        Else
            TxtNRate.BackColor = SEGNALA_OK
        End If

        '-
        If ddlTipoScadenza.SelectedIndex = 2 Then   'Giorno fisso
            If optMeseInCorso.Checked = False And optMeseSuccessivo.Checked = False Then
                Aggiorna = False
                LblErrore.Text = "Se scegli come tipo di scadenza [Giorno fisso], devi indicare se vuoi far partire il calcolo della scadenza dal mese corrente (data fattura) o successivo."
                Exit Function
            End If
            If (CInt(TxtNRate.Text) > 0) Then
                Dim I As Integer = 0
                For I = 1 To TxtNRate.Text
                    Select Case I
                        Case 1
                            If TxtScad1.Text = "" Or Not IsNumeric(TxtScad1.Text) Or TxtScad1.Text = "0" Then
                                TxtScad1.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                            If CInt(TxtScad1.Text) > 31 Then
                                TxtScad1.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                        Case 2
                            If TxtScad2.Text = "" Or Not IsNumeric(TxtScad2.Text) Or TxtScad2.Text = "0" Then
                                TxtScad2.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                            If CInt(TxtScad2.Text) > 31 Then
                                TxtScad2.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                        Case 3
                            If TxtScad3.Text = "" Or Not IsNumeric(TxtScad3.Text) Or TxtScad3.Text = "0" Then
                                TxtScad3.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                            If CInt(TxtScad3.Text) > 31 Then
                                TxtScad3.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                        Case 4
                            If TxtScad4.Text = "" Or Not IsNumeric(TxtScad4.Text) Or TxtScad4.Text = "0" Then
                                TxtScad4.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                            If CInt(TxtScad4.Text) > 31 Then
                                TxtScad4.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                        Case 5
                            If TxtScad5.Text = "" Or Not IsNumeric(TxtScad5.Text) Or TxtScad5.Text = "0" Then
                                TxtScad5.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                            If CInt(TxtScad5.Text) > 31 Then
                                TxtScad5.BackColor = SEGNALA_KO
                                Aggiorna = False
                                LblErrore.Text = "Devi inserire il numero del giorno 1-31"
                                Exit Function
                            End If
                    End Select
                Next I
            End If
        Else
            TxtNRate.BackColor = SEGNALA_OK
        End If


        If Aggiorna = False Then Exit Function

        Try
            If ddlPagamenti.SelectedIndex > 0 Then
                Session(IDPAGAMENTO) = IIf(IsNumeric(ddlPagamenti.SelectedValue), ddlPagamenti.SelectedValue, 0)
            Else
                Session(IDPAGAMENTO) = "0"
            End If

            SqlDSPagamenti.UpdateParameters.Item("Codice").DefaultValue = CLng(txtCodice.Text.Trim)
            SqlDSPagamenti.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
            SqlDSPagamenti.UpdateParameters.Item("Numero_Rate").DefaultValue = TxtNRate.Text
            SqlDSPagamenti.UpdateParameters.Item("Numero_Rate_Effettive").DefaultValue = TxtNRateEffettive.Text
            SqlDSPagamenti.UpdateParameters.Item("Scadenza_1").DefaultValue = TxtScad1.Text
            SqlDSPagamenti.UpdateParameters.Item("Scadenza_2").DefaultValue = TxtScad2.Text
            SqlDSPagamenti.UpdateParameters.Item("Scadenza_3").DefaultValue = TxtScad3.Text
            SqlDSPagamenti.UpdateParameters.Item("Scadenza_4").DefaultValue = TxtScad4.Text
            SqlDSPagamenti.UpdateParameters.Item("Scadenza_5").DefaultValue = TxtScad5.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imponib_1").DefaultValue = TxtPercImporto1.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imponib_2").DefaultValue = TxtPercImporto2.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imponib_3").DefaultValue = TxtPercImporto3.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imponib_4").DefaultValue = TxtPercImporto4.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imponib_5").DefaultValue = TxtPercImporto5.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imposta_1").DefaultValue = TxtPercImposta1.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imposta_2").DefaultValue = TxtPercImposta2.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imposta_3").DefaultValue = TxtPercImposta3.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imposta_4").DefaultValue = TxtPercImposta4.Text
            SqlDSPagamenti.UpdateParameters.Item("Perc_Imposta_5").DefaultValue = TxtPercImposta5.Text
            SqlDSPagamenti.UpdateParameters.Item("Spese_Incasso").DefaultValue = TxtSpeseIncasso.Text
            SqlDSPagamenti.UpdateParameters.Item("Sconto_Cassa").DefaultValue = TxtScontoCassa.Text
            SqlDSPagamenti.UpdateParameters.Item("Tipo_Pagamento").DefaultValue = ddlTipoPagamento.SelectedValue
            SqlDSPagamenti.UpdateParameters.Item("Tipo_Scadenza").DefaultValue = ddlTipoScadenza.SelectedValue
            If ddlEscludiMese1.SelectedIndex > 0 Then
                SqlDSPagamenti.UpdateParameters.Item("Mese_Escluso_1").DefaultValue = ddlEscludiMese1.SelectedValue
            Else
                SqlDSPagamenti.UpdateParameters.Item("Mese_Escluso_1").DefaultValue = 0
            End If
            If ddlEscludiMese2.SelectedIndex > 0 Then
                SqlDSPagamenti.UpdateParameters.Item("Mese_Escluso_2").DefaultValue = ddlEscludiMese2.SelectedValue
            Else
                SqlDSPagamenti.UpdateParameters.Item("Mese_Escluso_2").DefaultValue = 0
            End If
            If ddlIVASpese.SelectedIndex > 0 Then
                SqlDSPagamenti.UpdateParameters.Item("IVA_Spese_Incasso").DefaultValue = ddlIVASpese.SelectedValue
            Else
                SqlDSPagamenti.UpdateParameters.Item("IVA_Spese_Incasso").DefaultValue = 0 'Regime normale
            End If
            If optMeseInCorso.Checked = True Then
                SqlDSPagamenti.UpdateParameters.Item("Mese").DefaultValue = 0
            Else
                SqlDSPagamenti.UpdateParameters.Item("Mese").DefaultValue = 1
            End If
            SqlDSPagamenti.Update()
            SqlDSPagamenti.DataBind()
            '-----
            ddlPagamenti.Items.Clear()
            ddlPagamenti.Items.Add("")
            ddlPagamenti.DataBind()
            PopolaEntityDati()
            Dim strErrore As String = ""
            App.CaricaPagamenti(Session(ESERCIZIO), strErrore) 'pier180112 AGGIORNO LA CACHE
            If strErrore.Trim <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlPagamenti_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPagamenti.SelectedIndexChanged
        Session(IDPAGAMENTO) = IIf(IsNumeric(ddlPagamenti.SelectedValue), ddlPagamenti.SelectedValue, 0)
        If ddlPagamenti.SelectedIndex = 0 Then
            Session(IDPAGAMENTO) = "0"
            txtCodice.Text = Session(IDPAGAMENTO)
            txtCodice.Enabled = True
            SvuotaCampi()
            Exit Sub
        End If
        txtCodice.Enabled = False
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        txtCodice.Text = Session(IDPAGAMENTO)
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
        Dim dvPagamenti As DataView
        dvPagamenti = SqlDSPagamenti.Select(DataSourceSelectArguments.Empty)
        If (dvPagamenti Is Nothing) Then
            txtCodice.Focus()
            Exit Sub
        End If
        If dvPagamenti.Count > 0 Then
            dvPagamenti.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
            If dvPagamenti.Count > 0 Then
                Session(IDPAGAMENTO) = dvPagamenti.Item(0).Item("Codice")
                txtCodice.Text = dvPagamenti.Item(0).Item("Codice").ToString.Trim
                txtCodice.Enabled = False
                txtDescrizione.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Descrizione")), dvPagamenti.Item(0).Item("Descrizione").ToString.Trim, "")
                TxtNRate.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Numero_Rate")), dvPagamenti.Item(0).Item("Numero_Rate").ToString.Trim, "0")
                TxtNRateEffettive.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Numero_Rate_Effettive")), dvPagamenti.Item(0).Item("Numero_Rate_Effettive").ToString.Trim, "0")
                TxtScad1.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_1")), dvPagamenti.Item(0).Item("Scadenza_1").ToString.Trim, "0")
                TxtScad2.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_2")), dvPagamenti.Item(0).Item("Scadenza_2").ToString.Trim, "0")
                TxtScad3.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_3")), dvPagamenti.Item(0).Item("Scadenza_3").ToString.Trim, "0")
                TxtScad4.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_4")), dvPagamenti.Item(0).Item("Scadenza_4").ToString.Trim, "0")
                TxtScad5.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Scadenza_5")), dvPagamenti.Item(0).Item("Scadenza_5").ToString.Trim, "0")
                TxtPercImporto1.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_1")), dvPagamenti.Item(0).Item("Perc_Imponib_1").ToString.Trim, "0")
                TxtPercImporto2.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_2")), dvPagamenti.Item(0).Item("Perc_Imponib_2").ToString.Trim, "0")
                TxtPercImporto3.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_3")), dvPagamenti.Item(0).Item("Perc_Imponib_3").ToString.Trim, "0")
                TxtPercImporto4.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_4")), dvPagamenti.Item(0).Item("Perc_Imponib_4").ToString.Trim, "0")
                TxtPercImporto5.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imponib_5")), dvPagamenti.Item(0).Item("Perc_Imponib_5").ToString.Trim, "0")
                TxtPercImposta1.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_1")), dvPagamenti.Item(0).Item("Perc_Imposta_1").ToString.Trim, "0")
                TxtPercImposta2.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_2")), dvPagamenti.Item(0).Item("Perc_Imposta_2").ToString.Trim, "0")
                TxtPercImposta3.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_3")), dvPagamenti.Item(0).Item("Perc_Imposta_3").ToString.Trim, "0")
                TxtPercImposta4.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_4")), dvPagamenti.Item(0).Item("Perc_Imposta_4").ToString.Trim, "0")
                TxtPercImposta5.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Perc_Imposta_5")), dvPagamenti.Item(0).Item("Perc_Imposta_5").ToString.Trim, "0")
                TxtSpeseIncasso.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Spese_Incasso")), dvPagamenti.Item(0).Item("Spese_Incasso").ToString.Trim, "0")
                TxtScontoCassa.Text = IIf(Not IsDBNull(dvPagamenti.Item(0).Item("Sconto_Cassa")), dvPagamenti.Item(0).Item("Sconto_Cassa").ToString.Trim, "0")
                If Not IsDBNull(dvPagamenti.Item(0).Item("Mese")) Then
                    If dvPagamenti.Item(0).Item("Mese") = 0 Then
                        optMeseInCorso.Checked = True
                        optMeseSuccessivo.Checked = False
                    Else
                        optMeseInCorso.Checked = False
                        optMeseSuccessivo.Checked = True
                    End If
                Else
                    optMeseInCorso.Checked = False
                    optMeseSuccessivo.Checked = False
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("Tipo_Pagamento")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("Tipo_Pagamento").ToString.Trim, ddlTipoPagamento)
                Else
                    ddlTipoPagamento.SelectedIndex = 0
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("Tipo_Scadenza")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("Tipo_Scadenza").ToString.Trim, ddlTipoScadenza)
                Else
                    ddlTipoScadenza.SelectedIndex = 0
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("Mese_Escluso_1")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("Mese_Escluso_1").ToString.Trim, ddlEscludiMese1)
                Else
                    ddlEscludiMese1.SelectedIndex = 0
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("Mese_Escluso_2")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("Mese_Escluso_2").ToString.Trim, ddlEscludiMese2)
                Else
                    ddlEscludiMese2.SelectedIndex = 0
                End If
                If Not IsDBNull(dvPagamenti.Item(0).Item("IVA_Spese_Incasso")) Then
                    PosizionaItemDDL(dvPagamenti.Item(0).Item("IVA_Spese_Incasso").ToString.Trim, ddlIVASpese)
                Else
                    ddlIVASpese.SelectedIndex = 0
                End If
            End If
        End If
        txtCodice.Focus()
    End Sub
End Class