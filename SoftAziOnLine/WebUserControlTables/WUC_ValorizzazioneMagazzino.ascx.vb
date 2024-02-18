Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports SoftAziOnLine.Magazzino

Imports System.IO 'giu140615

Partial Public Class WUC_ValorizzazioneMagazzino
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'giu211121
        Dim myStr As String = Request.QueryString("labelForm")
        If InStr(myStr.Trim.ToUpper, "FIFO") > 0 Then
            rbtnRaggrFornSint.Visible = True
        Else
            rbtnRaggrFornSint.Visible = False
        End If
        '---------
        lblMess.Visible = False
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceCategoria.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceLinea.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'giu130112
        LnkStampa.Visible = False
        If (Not IsPostBack) Then
            Session("SWConferma") = SWNO
            rbtnCodice.Checked = True
            ddlMagazzino.SelectedValue = 1
            BtnStampaControllo.Text = "Stampa di controllo"
            If Now > CDate("31/12/" & Session(ESERCIZIO)) Then
                txtData.Text = "31/12/" & Session(ESERCIZIO)
            Else
                txtData.Text = Format(Now, FormatoData)
            End If
        End If
        ModalPopup.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If
        WFP_ElencoCliForn1.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click, BtnStampaControllo.Click
        lblMess.Visible = False
        LnkStampa.Visible = False
        lblMessNegativi.Visible = False
        Dim Errore As Boolean = False
        ddlMagazzino.BackColor = SEGNALA_OK
        ddlCatgoria.BackColor = SEGNALA_OK
        ddlLinea.BackColor = SEGNALA_OK
        txtData.BackColor = SEGNALA_OK
        'GIU180523
        If Not IsDate(txtData.Text) Or txtData.Text.Length <> 10 Then
            txtData.BackColor = SEGNALA_KO
            Errore = True
        End If
        '------------------------------------
        Dim DsMagazzino1 As New DsMagazzino
        Dim ObjReport As New Object
        Dim ClsPrint As New Magazzino
        Dim Filtri As String = ""
        Dim Selezione As String = ""
        Dim StrErrore As String = ""
        Dim RagrForn As Boolean = False
        If ddlMagazzino.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlCatgoria.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlLinea.BackColor = SEGNALA_KO Then
            Errore = True
        End If

        If ddlMagazzino.SelectedItem.Text = "" Or Not IsNumeric(ddlMagazzino.SelectedValue.Trim) Then
            ddlMagazzino.BackColor = SEGNALA_KO
            Errore = True
        End If
        If txtCod1.Text.Trim <> "" And txtCod2.Text.Trim <> "" Then
            If txtCod2.Text.Trim < txtCod1.Text.Trim Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati selezione", "Attenzione, Codici articoli incongruenti. <br> (Al codice < di Dal codice).", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If chkTuttiFornitori.Checked = False Then
            If txtCodFornitore.Text.Trim = "" Or txtDescFornitore.Text = "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Controllo dati selezione", "Attenzione, Codice Fornitore richiesto.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If chkArtGiacNegativa.Checked = True And chkArtGiacNoZero.Checked = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati selezione", "Attenzione, Selezionare solo una delle opzioni:<br>Solo Giacenza Negativa o diversa da ZERO", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Errore Then
            ModalPopup.Show("Controllo dati stampa", "Attenzione, i campi segnalati in rosso non sono validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu050123
        '''Dim CodMag As Integer = 0
        '''Try
        '''    CodMag = ddlMagazzino.SelectedValue
        '''Catch ex As Exception
        '''    CodMag = 0
        '''End Try
        '''If CodMag = 0 Then
        '''    lblMess.Visible = True
        '''    lblMess.Text = "ATTENZIONE, La Valorizzazione per tutti i Magazzini è differente dalla Valorizzazione dei Singoli Magazzini" + _
        '''    "<br> Quindi non sono confrontabili i dati delle Valorizzazioni di TUTTI e SINGOLI MAGAZZINI"
        '''End If
        '''If String.IsNullOrEmpty(Session("SWConferma")) Then
        '''    Session("SWConferma") = SWNO
        '''End If
        '''If Session("SWConferma") = SWNO And CodMag = 0 Then
        '''    Session("SWConferma") = SWSI
        '''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        '''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        '''    ModalPopup.Show("ATTENZIONE", lblMess.Text + "<BR>Rieseguite la stampa se volete la stampa richiesta.", WUC_ModalPopup.TYPE_ALERT)
        '''    Exit Sub
        '''End If
        '-----------------------------
        Dim strErroreGiac As String = ""
        Dim SWNegativi As Boolean = False 'giu190613 spostato qui
        If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf strErroreGiac.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricalcolo giacenze", strErroreGiac.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        'FILTRO PER MAGAZZINO
        'GIU210920 VIENE SELEZIONATO NELLA CHIAMATA A funDispMagazzino Selezione = "WHERE Codice_Magazzino = " & ddlMagazzino.SelectedValue
        Selezione = ""
        '-------------
        Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper

        'FILTRO PER CATEGORIA
        If txtCodCategoria.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "CodCategoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
            Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
        End If

        'FILTRO PER LINEA
        If txtCodLinea.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "CodLinea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
            Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
        End If

        'FILTRO DA ARTICOLO 
        If txtCod1.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
            Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
        End If

        'FILTRO A ARTICOLO
        If txtCod2.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
            Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
        End If
        'FILTRO fornitore
        If txtCodFornitore.Text <> "" Then
            If Selezione.Trim <> "" Then
                Selezione += " AND "
            Else
                Selezione += "WHERE "
            End If
            Selezione = Selezione & "Cod_Fornitore = '" & Replace(txtCodFornitore.Text, "'", "''") & "'"
            Filtri = Filtri & " | Cod_Fornitore - " & txtCodFornitore.Text.ToUpper
        End If
        '==================================================================================
        'giu280512 QUI NO PERCHE LA GIACENZA E LA MOVIMENTAZIONE è SEMPRE RELATIVO AD OGGI
        'ANCHE IL REGGRUPPAMENTO FIRNITORE NON C'è 
        '==================================================================================
        'FILTRO PER GIACENZA > 0
        ' ''If chkArtGiacDivZero.Checked Then
        ' ''    Selezione = Selezione & " AND ((Giacenza <> 0) Or (Giac_Impegnata <>0))"
        ' ''    Filtri = Filtri & " | Giacenza diversa da 0"
        ' ''End If

        '' ''PER STAMPARE SOLO ARTICOLI MOVIMENTATI
        ' ''If chkArtMovimentati.Checked Then
        ' ''    Selezione = Selezione & " AND ( (Giac_Impegnata <>0) or (Ordinati <>0) Or (Ord_Clienti <>0) )"
        ' ''    Filtri = Filtri & " | Articoli movimentati"
        ' ''End If
        '==================================================================================
        'giu250118 non serve ORDINAMENTO PER CODICE / FORNITORE, CODICE
        ' ''If rbtnCodice.Checked Then
        ' ''    Selezione = Selezione & " ORDER BY Cod_Articolo"
        ' ''Else
        ' ''    Selezione = Selezione & " ORDER BY Rag_Soc,Cod_Articolo"
        ' ''End If
        'giu250118 Selezione = Selezione & " ORDER BY Cod_Articolo"
        'giu280512 END
        '
        Session("SAVETIPOSTAMPA") = Session(CSTTIPORPTDISPMAG)
        Session(CSTNOMEPDF) = "" 'giu300315
        Try
            If Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFO Then
                If sender.id = BtnStampaControllo.ID Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOAnalitico
                ElseIf rbtnCodice.Checked = True Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSintetico
                ElseIf rbtnRaggrFornCodice.Checked = True Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoFor
                    'GIU211121
                Else
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoForS
                End If
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFO Then
                If sender.id = BtnStampaControllo.ID Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOAnalitico
                Else
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOSintetico
                End If
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcq Then
                If sender.id = BtnStampaControllo.ID Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqAnalitico
                Else
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqSintetico
                End If
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPond Then
                If sender.id = BtnStampaControllo.ID Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondAnalitico
                Else
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondSintetico
                End If
            End If

            If Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOAnalitico Or _
                Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSintetico Or _
                Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoFor Or _
                Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoForS Then
                If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", "") + " - Valorizzazione di magazzino (FIFO) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, False, False, SWNegativi, chkArtGiacNegativa.Checked, True, ddlMagazzino.SelectedValue, , chkArtGiacNoZero.Checked) Then
                    If DsMagazzino1.DispMagazzino.Count > 0 Then
                        'giu281020 GIU210920 OK ADESSO ANCHE PER ALTRI MAGAZZINI
                        If Val(ddlMagazzino.SelectedValue.Trim) <> 0 Then
                            Selezione = "WHERE CodiceMagazzino = " & ddlMagazzino.SelectedValue & " AND "
                        Else
                            Selezione = "WHERE "
                        End If
                        Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper
                        'giu281020 FILTRO VALORIZZAZIONE ALLA DATA
                        Selezione += " Data_Doc <= CONVERT(DATETIME, '" & Format(CDate(txtData.Text), FormatoData) & "', 103)"
                        'FILTRO PER CATEGORIA
                        If txtCodCategoria.Text <> "" Then
                            Selezione = Selezione & " AND Categoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
                            Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
                        End If
                        'FILTRO PER LINEA
                        If txtCodLinea.Text <> "" Then
                            Selezione = Selezione & " AND Linea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
                            Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
                        End If
                        'FILTRO DA ARTICOLO 
                        If txtCod1.Text <> "" Then
                            Selezione = Selezione & " AND view_MovMagValorizzazione.Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
                            Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
                        End If
                        'FILTRO A ARTICOLO
                        If txtCod2.Text <> "" Then
                            Selezione = Selezione & " AND view_MovMagValorizzazione.Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
                            Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
                        End If
                        'giu250118 non serve Selezione += " ORDER BY view_MovMagValorizzazione.Cod_Articolo, Data_Doc"
                        If ClsPrint.ValMagFIFO_CostoVendutoFIFO(Session(CSTAZIENDARPT), "Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", "") + " - Valorizzazione di magazzino (FIFO) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, chkArtGiacNegativa.Checked, True, False, SWNegativi) Then
                            If DsMagazzino1.DispMagazzino.Count > 0 Then
                                ' ''Session(CSTDsPrinWebDoc) = DsMagazzino1
                                If SWNegativi = True Then
                                    If chkArtGiacNegativa.Checked = False Then
                                        lblMessNegativi.Text = "Attenzione, sono presenti movimenti in negativo."
                                        lblMessNegativi.Visible = True
                                    End If
                                End If
                                'Session(CSTNOBACK) = 0 'giu040512
                                'Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
                            Else
                                ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                                Exit Sub
                            End If
                        Else
                            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                    Else
                        ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                        Exit Sub
                    End If
                Else
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqAnalitico Or Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqSintetico Then
                If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", "") + " - Valorizzazione di magazzino (Ultimo prezzo di acquisto) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, False, False, SWNegativi, chkArtGiacNegativa.Checked, True, ddlMagazzino.SelectedValue, , chkArtGiacNoZero.Checked) Then
                    If DsMagazzino1.DispMagazzino.Count > 0 Then
                        'giu281020 GIU210920 OK ADESSO ANCHE PER ALTRI MAGAZZINI
                        If Val(ddlMagazzino.SelectedValue.Trim) <> 0 Then
                            Selezione = "WHERE CodiceMagazzino = " & ddlMagazzino.SelectedValue & " AND "
                        Else
                            Selezione = "WHERE "
                        End If
                        Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper
                        'giu281020 FILTRO VALORIZZAZIONE ALLA DATA
                        Selezione += " Data_Doc <= CONVERT(DATETIME, '" & Format(CDate(txtData.Text), FormatoData) & "', 103)"
                        'è NELL'INTESTAZIONE Filtri += " | Alla data - " & Format(CDate(txtData.Text), FormatoData)
                        'FILTRO PER CATEGORIA
                        If txtCodCategoria.Text <> "" Then
                            Selezione = Selezione & " AND Categoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
                            Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
                        End If
                        'FILTRO PER LINEA
                        If txtCodLinea.Text <> "" Then
                            Selezione = Selezione & " AND Linea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
                            Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
                        End If
                        'FILTRO DA ARTICOLO 
                        If txtCod1.Text <> "" Then
                            Selezione = Selezione & " AND view_MovMagValorizzazione.Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
                            Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
                        End If
                        'FILTRO A ARTICOLO
                        If txtCod2.Text <> "" Then
                            Selezione = Selezione & " AND view_MovMagValorizzazione.Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
                            Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
                        End If
                        'giu250118 non serve Selezione += " ORDER BY view_MovMagValorizzazione.Cod_Articolo, Data_Doc"
                        If ClsPrint.ValMagUltPrzAcq(Session(CSTAZIENDARPT), "Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", "") + " - Valorizzazione di magazzino (Ultimo prezzo di acquisto) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, chkArtGiacNegativa.Checked, SWNegativi) Then
                            If DsMagazzino1.DispMagazzino.Count > 0 Then
                                ' ''Session(CSTDsPrinWebDoc) = DsMagazzino1
                                If StrErrore <> "" Then
                                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "OKLink" '"Redirect"
                                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                    ' ''ModalPopup.Show("Attenzione", "Gli articoli " & StrErrore & "<br>presentano ultimo prezzo di acquisto pari a 0." & "<br>Desidera proseguire?", WUC_ModalPopup.TYPE_CONFIRM_YN)
                                    ' ''Exit Sub
                                    lblMess.Text = "Attenzione, gli articoli " & StrErrore & "<br>presentano ultimo prezzo di acquisto pari a 0."
                                    lblMess.Visible = True
                                ElseIf SWNegativi = True Then
                                    If chkArtGiacNegativa.Checked = False Then
                                        lblMessNegativi.Text = "Attenzione, sono presenti movimenti in negativo."
                                        lblMessNegativi.Visible = True
                                    End If
                                End If
                                'Session(CSTNOBACK) = 0 'giu040512
                                'Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
                            Else
                                ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                                Exit Sub
                            End If
                        Else
                            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                    Else
                        ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                        Exit Sub
                    End If
                Else
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondAnalitico Or Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondSintetico Then
                If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", "") + " - Valorizzazione di magazzino (Media ponderata) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, False, False, SWNegativi, chkArtGiacNegativa.Checked, True, ddlMagazzino.SelectedValue, , chkArtGiacNoZero.Checked) Then
                    If DsMagazzino1.DispMagazzino.Count > 0 Then
                        'giu281020 GIU210920 OK ADESSO ANCHE PER ALTRI MAGAZZINI
                        If Val(ddlMagazzino.SelectedValue.Trim) <> 0 Then
                            Selezione = "WHERE CodiceMagazzino = " & ddlMagazzino.SelectedValue & " AND "
                        Else
                            Selezione = "WHERE "
                        End If
                        Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper
                        'giu281020 FILTRO VALORIZZAZIONE ALLA DATA
                        Selezione += " Data_Doc <= CONVERT(DATETIME, '" & Format(CDate(txtData.Text), FormatoData) & "', 103)"
                        'è NELL'INTESTAZIONE Filtri += " | Alla data - " & Format(CDate(txtData.Text), FormatoData)
                        'FILTRO PER CATEGORIA
                        If txtCodCategoria.Text <> "" Then
                            Selezione = Selezione & " AND Categoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
                            Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
                        End If
                        'FILTRO PER LINEA
                        If txtCodLinea.Text <> "" Then
                            Selezione = Selezione & " AND Linea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
                            Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
                        End If
                        'FILTRO DA ARTICOLO 
                        If txtCod1.Text <> "" Then
                            Selezione = Selezione & " AND view_MovMagValorizzazione.Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
                            Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
                        End If
                        'FILTRO A ARTICOLO
                        If txtCod2.Text <> "" Then
                            Selezione = Selezione & " AND view_MovMagValorizzazione.Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
                            Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
                        End If
                        'giu250118 non serve Selezione += " ORDER BY view_MovMagValorizzazione.Cod_Articolo, Data_Doc"
                        If ClsPrint.ValMagMediaPond(Session(CSTAZIENDARPT), "Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", "") + " - Valorizzazione di magazzino (Media ponderata) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, chkArtGiacNegativa.Checked, SWNegativi) Then
                            If DsMagazzino1.DispMagazzino.Count > 0 Then
                                ' ''Session(CSTDsPrinWebDoc) = DsMagazzino1
                                If SWNegativi = True Then
                                    If chkArtGiacNegativa.Checked = False Then
                                        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "OKLink" '"Redirect"
                                        ' ''ModalPopup.Show("Attenzione", "Sono presenti movimenti in negativo. <br> Confermi l'elaborazione della stampa ?", WUC_ModalPopup.TYPE_CONFIRM)
                                        ' ''Exit Sub
                                        lblMessNegativi.Text = "Attenzione, sono presenti movimenti in negativo."
                                        lblMessNegativi.Visible = True
                                    End If
                                End If
                                'Session(CSTNOBACK) = 0 'giu040512
                                'Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
                            Else
                                ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                                Exit Sub
                            End If
                        Else
                            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                    Else
                        ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                        Exit Sub
                    End If
                Else
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If

            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOAnalitico Or Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOSintetico Then
                If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", "") + " - Valorizzazione di magazzino (LIFO) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, False, False, SWNegativi, chkArtGiacNegativa.Checked, True, ddlMagazzino.SelectedValue, , chkArtGiacNoZero.Checked) Then
                    If DsMagazzino1.DispMagazzino.Count > 0 Then
                        'giu281020 GIU210920 OK ADESSO ANCHE PER ALTRI MAGAZZINI
                        If Val(ddlMagazzino.SelectedValue.Trim) <> 0 Then
                            Selezione = "WHERE CodiceMagazzino = " & ddlMagazzino.SelectedValue & " AND "
                        Else
                            Selezione = "WHERE "
                        End If
                        Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper
                        'giu281020 FILTRO VALORIZZAZIONE ALLA DATA
                        Selezione += " Data_Doc <= CONVERT(DATETIME, '" & Format(CDate(txtData.Text), FormatoData) & "', 103)"
                        'è NELL'INTESTAZIONE Filtri += " | Alla data - " & Format(CDate(txtData.Text), FormatoData)
                        'FILTRO PER CATEGORIA
                        If txtCodCategoria.Text <> "" Then
                            Selezione = Selezione & " AND Categoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
                            Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
                        End If
                        'FILTRO PER LINEA
                        If txtCodLinea.Text <> "" Then
                            Selezione = Selezione & " AND Linea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
                            Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
                        End If
                        'FILTRO DA ARTICOLO 
                        If txtCod1.Text <> "" Then
                            Selezione = Selezione & " AND view_MovMagValorizzazione.Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
                            Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
                        End If
                        'FILTRO A ARTICOLO
                        If txtCod2.Text <> "" Then
                            Selezione = Selezione & " AND view_MovMagValorizzazione.Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
                            Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
                        End If
                        'giu250118 non serve Selezione += " ORDER BY view_MovMagValorizzazione.Cod_Articolo, Data_Doc"
                        If ClsPrint.ValMagLIFO(Session(CSTAZIENDARPT), "Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", "") + " - Valorizzazione di magazzino (LIFO) alla data " & Format(CDate(txtData.Text), FormatoData), Filtri, Selezione, DsMagazzino1, StrErrore, chkArtGiacNegativa.Checked, SWNegativi) Then
                            If DsMagazzino1.DispMagazzino.Count > 0 Then
                                ' ''Session(CSTDsPrinWebDoc) = DsMagazzino1
                                If SWNegativi = True Then
                                    If chkArtGiacNegativa.Checked = False Then
                                        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "OKLink" '"Redirect"
                                        ' ''ModalPopup.Show("Attenzione", "Sono presenti movimenti in negativo. <br> Confermi l'elaborazione della stampa ?", WUC_ModalPopup.TYPE_CONFIRM)
                                        ' ''Exit Sub
                                        lblMessNegativi.Text = "Attenzione, sono presenti movimenti in negativo."
                                        lblMessNegativi.Visible = True
                                    End If
                                End If
                                'Session(CSTNOBACK) = 0 'giu040512
                                'Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
                            Else
                                ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                                Exit Sub
                            End If
                        Else
                            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                    Else
                        ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                        Exit Sub
                    End If
                Else
                    ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKLink(DsMagazzino1)
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub txtCodCategoria_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCategoria.TextChanged
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        PosizionaItemDDLTxt(txtCodCategoria, ddlCatgoria)
        txtCodLinea.Focus()
    End Sub

    Private Sub txtCodLinea_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodLinea.TextChanged
        PosizionaItemDDLTxt(txtCodLinea, ddlLinea)
        txtCod1.Focus()
    End Sub

    Private Sub ddlLinea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlLinea.SelectedIndexChanged
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        txtCodLinea.Text = ddlLinea.SelectedValue
        txtCodLinea.BackColor = SEGNALA_OK
        ddlLinea.BackColor = SEGNALA_OK
    End Sub

    Private Sub ddlCatgoria_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCatgoria.SelectedIndexChanged
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        txtCodCategoria.Text = ddlCatgoria.SelectedValue
        txtCodCategoria.BackColor = SEGNALA_OK
        ddlCatgoria.BackColor = SEGNALA_OK
    End Sub

    ' ''Public Sub Redirect()
    ' ''    Session(CSTNOBACK) = 0 'giu040512
    ' ''    Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
    ' ''End Sub

    Private Sub btnCod1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod1.Click
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        Session(SWCOD1COD2) = 1
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Private Sub btnCod2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod2.Click
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        Session(SWCOD1COD2) = 2
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Public Sub CallBackWFPArticoloSelSing()
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        If String.IsNullOrEmpty(Session(SWCOD1COD2)) Then
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                If txtCod2.Text.Trim = "" Then
                    txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                    txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                End If
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
        End If
        'TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        'TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub
    'giu250118
    Private Sub rbtnCodice_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCodice.CheckedChanged
        If rbtnCodice.Checked = True Then
            BtnStampaControllo.Enabled = True
        Else
            BtnStampaControllo.Enabled = False
        End If
    End Sub
    Private Sub rbtnRaggrFornCodice_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnRaggrFornCodice.CheckedChanged
        If rbtnRaggrFornCodice.Checked = True Then
            BtnStampaControllo.Enabled = False
        ElseIf rbtnCodice.Checked = False Then
            BtnStampaControllo.Enabled = True
        End If
    End Sub
    Public Sub OKLink(ByRef DsMagazzino1 As DsMagazzino)

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim Rpt As ReportClass
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        Dim CodiceDitta As String = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            CodiceDitta = ""
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            CodiceDitta = ""
        End If
        '---------------------
        Session(CSTNOMEPDF) = ""
        If Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzino Then
            Session(CSTNOMEPDF) = "DisponibilitaMagazzino.pdf"
            Rpt = New DispMag
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzinoFornitori Then
            Session(CSTNOMEPDF) = "DisponibilitaMagazzinoFornitori.pdf"
            Rpt = New DispMagForn
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOAnalitico Then
            Session(CSTNOMEPDF) = "ValMagFIFOAnalitico.pdf"
            Rpt = New ValMagFIFO
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSintetico Then
            Session(CSTNOMEPDF) = "ValMagFIFOSintetico.pdf"
            Rpt = New ValMagFIFOS
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoFor Then
            Session(CSTNOMEPDF) = "ValMagFIFOSinteticoFor.pdf"
            Rpt = New ValMagFIFOSFor
            'giu211121 ValMagFIFOSinteticoForS
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoForS Then
            Session(CSTNOMEPDF) = "ValMagFIFOSinteticoForSint.pdf"
            Rpt = New ValMagFIFOSForSint
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqAnalitico Then
            Session(CSTNOMEPDF) = "ValMagUltPrzAcqAnalitico.pdf"
            Rpt = New ValMagUltPrzAcq
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqSintetico Then
            Session(CSTNOMEPDF) = "ValMagUltPrzAcqSintetico.pdf"
            Rpt = New ValMagUltPrzAcqS
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondAnalitico Then
            Session(CSTNOMEPDF) = "ValMagMediaPondAnalitico.pdf"
            Rpt = New ValMagMediaPond
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondSintetico Then
            Session(CSTNOMEPDF) = "ValMagMediaPondSintetico.pdf"
            Rpt = New ValMagMediaPondS
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFO Then
            Session(CSTNOMEPDF) = "ValMagCostoVendFIFO.pdf"
            Rpt = New ValMagCostoVendutoFIFO
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFOSint Then
            Session(CSTNOMEPDF) = "ValMagCostoVendFIFOSint.pdf"
            Rpt = New ValMagCostoVendutoFIFOSint
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOAnalitico Then
            Session(CSTNOMEPDF) = "ValMagLIFOAnalitico.pdf"
            Rpt = New ValMagLIFO
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOSintetico Then
            Session(CSTNOMEPDF) = "ValMagLIFOSintetico.pdf"
            Rpt = New ValMagLIFOS
        Else
            Chiudi("Errore: TIPO STAMPA DI MAGAZZINO SCONOSCIUTA")
            Exit Sub
        End If
        '-----------------------------------
        Rpt.SetDataSource(DsMagazzino1)
        'Per evitare che solo un utente possa elaborare le stampe
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Sub
        End If
        Session(CSTNOMEPDF) = Format(Now, "yyyyMMddHHmmss") + Utente.Codice.Trim & Session(CSTNOMEPDF)
        '---------
        '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
        Session(CSTESPORTAPDF) = True
        Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & "StatMag\"
        Dim stPathReport As String = Session(CSTPATHPDF)
        Try 'giu281112 errore che il file Ã¨ gia aperto
            Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
            'giu140124
            Rpt.Close()
            Rpt.Dispose()
            Rpt = Nothing
            '-
            GC.WaitForPendingFinalizers()
            GC.Collect()
            '-------------
        Catch ex As Exception
            Rpt = Nothing
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Stampa valorizzazione magazzino", "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
            Exit Sub
        End Try
        
        LnkStampa.Visible = True
        Dim LnkName As String = "~/Documenti/StatMag/" & Session(CSTNOMEPDF)
        LnkStampa.HRef = LnkName
        Session(CSTTIPORPTDISPMAG) = Session("SAVETIPOSTAMPA")
    End Sub
    Private Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        Else
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        End If
    End Sub

    'giu040420
    Private Sub chkTuttiFornitori_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiFornitori.CheckedChanged
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        pulisciCampiFornitore()
        If chkTuttiFornitori.Checked Then
            AbilitaDisabilitaCampiFornitore(False)
        Else
            AbilitaDisabilitaCampiFornitore(True)
            txtCodFornitore.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiFornitore(ByVal Abilita As Boolean)
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        txtCodFornitore.Enabled = Abilita
        btnFornitore.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiFornitore()
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        txtCodFornitore.Text = ""
        txtDescFornitore.Text = ""
    End Sub
    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        txtDescFornitore.Text = App.GetValoreFromChiave(txtCodFornitore.Text, Def.FORNITORI, Session(ESERCIZIO))
    End Sub
    Private Sub ApriElencoFornitori1()
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodFornitore.Text = codice
            txtDescFornitore.Text = descrizione
        End If
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    End Sub
    Private Sub btnFornitore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFornitore.Click
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        Session(F_FOR_RICERCA) = True
        ApriElencoFornitori1()
    End Sub

    Private Sub rbtnRaggrFornSint_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnRaggrFornSint.CheckedChanged
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        LnkStampa.Visible = False
        lblMessNegativi.Visible = False
        If rbtnRaggrFornSint.Checked = True Then
            BtnStampaControllo.Enabled = False
        ElseIf rbtnCodice.Checked = False Then
            BtnStampaControllo.Enabled = True
        End If
    End Sub

    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        Session("SWConferma") = SWNO
        lblMess.Visible = False
        lblMessNegativi.Visible = False
        LnkStampa.Visible = False
        '''Dim CodMag As Integer = 0
        '''Try
        '''    CodMag = ddlMagazzino.SelectedValue
        '''Catch ex As Exception
        '''    CodMag = 0
        '''End Try
        '''If CodMag = 0 And ddlMagazzino.SelectedItem.Text <> "" Then
        '''    lblMess.Visible = True
        '''    lblMess.Text = "ATTENZIONE, La Valorizzazione per tutti i Magazzini è differente dalla Valorizzazione dei Singoli Magazzini" + _
        '''    "<br> Quindi non sono confrontabili i dati delle Valorizzazioni di TUTTI e SINGOLI MAGAZZINI"
        '''End If
        If ddlMagazzino.SelectedItem.Text = "" Or Not IsNumeric(ddlMagazzino.SelectedValue.Trim) Then
            ddlMagazzino.BackColor = SEGNALA_KO
        Else
            ddlMagazzino.BackColor = SEGNALA_OK
        End If
    End Sub
End Class