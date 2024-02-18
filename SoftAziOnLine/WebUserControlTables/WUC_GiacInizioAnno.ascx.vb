Imports It.SoftAzi.Model.Entity.OperatoriEntity
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao.DataSource
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Def
Imports It.SoftAzi.Model.Facade
Imports System.Data.SqlClient
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WUC_GiacInizioAnno
    Inherits System.Web.UI.UserControl

    Dim ConnessioneAzi As SqlConnection
    Dim ConnessioneAziAP As SqlConnection
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Private strErrore As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim dbcon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim cmd As New SqlCommand
        Dim CodCausale As Integer

        ModalPopup.WucElement = Me

        ConnessioneAzi = New SqlConnection
        ConnessioneAziAP = New SqlConnection
        ConnessioneAzi.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataMagazzino.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi)
        ConnessioneAziAP.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi, CStr(CInt(Session(ESERCIZIO)) - 1))

        cmd.Connection = ConnessioneAzi
        cmd.CommandType = CommandType.Text
        cmd.Parameters.Add(New SqlParameter("@CodCausale", SqlDbType.Int, 4, ParameterDirection.Output, True, 0, 0, "", DataRowVersion.Current, 0))
        cmd.CommandText = "SELECT @CodCausale = CausaleRipristinoSaldi FROM ParametriGeneraliAzi"

        If (Not IsPostBack) Then

            Try
                If ConnessioneAzi.State <> ConnectionState.Open Then
                    ConnessioneAzi.Open()
                End If

                cmd.ExecuteNonQuery()

                If IsDBNull(cmd.Parameters("@CodCausale").Value) Then
                    lblErrore.Text = "La causale da utilizzare per il movimento non è stata impostata."
                    lblErrore.Visible = True
                    btnOK.Enabled = False
                ElseIf CStr(cmd.Parameters("@CodCausale").Value).Trim = "" Then
                    lblErrore.Text = "La causale da utilizzare per il movimento non è stata impostata."
                    lblErrore.Visible = True
                    btnOK.Enabled = False
                Else
                    CodCausale = CInt(cmd.Parameters("@CodCausale").Value)
                    cmd.CommandText = "SELECT @CodCausale = Codice FROM CausMag WHERE Codice = " & CodCausale
                    cmd.Connection = ConnessioneAzi

                    If ConnessioneAzi.State <> ConnectionState.Open Then
                        ConnessioneAzi.Open()
                    End If

                    cmd.ExecuteNonQuery()

                    If IsDBNull(cmd.Parameters("@CodCausale").Value) Then
                        lblErrore.Text = "La causale da utilizzare impostata non è presente tra le causali."
                        lblErrore.Visible = True
                        btnOK.Enabled = False
                        Exit Sub
                    Else
                        Session("CodCausale") = cmd.Parameters("@CodCausale").Value
                    End If

                    If App.GetDatiAbilitazioni(CSTABILAZI, "CodCoGeDitta", Session("CodCoGeDitta"), strErrore) = True Then
                        If strErrore.Trim <> "" Then
                            Session("CodCoGeDitta") = ""
                            lblErrore.Text = strErrore
                            lblErrore.Visible = True
                            btnOK.Enabled = False
                            Exit Sub
                        End If
                    Else
                        If strErrore.Trim <> "" Then
                            Session("CodCoGeDitta") = ""
                            lblErrore.Text = strErrore
                            lblErrore.Visible = True
                            btnOK.Enabled = False
                            Exit Sub
                        End If
                    End If

                    If Session("CodCoGeDitta") = "" Then
                        lblErrore.Text = "Codice CoGe di " & Session(CSTAZIENDARPT) & " non impostato."
                        lblErrore.Visible = True
                        btnOK.Enabled = False
                        Exit Sub
                    End If

                    'alb29012013 controllo se trasferimento già effettuato
                    ddlMagazzino.SelectedIndex = 0
                    lblErrore.Text = "Selezionare il Magazzino su cui trasferire i movimenti di inzio anno."
                    lblErrore.Visible = True
                    btnOK.Enabled = False
                   
                End If
            Catch ex As Exception
                lblErrore.Text = "La causale da utilizzare per il movimento non è stata impostata."
                lblErrore.Visible = True
                btnOK.Enabled = False
            End Try
        End If
    End Sub
    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        Dim IDMovCM As String = ""
        If ddlMagazzino.SelectedIndex = 0 Then
            lblErrore.Text = "Selezionare il Magazzino su cui trasferire i movimenti di inzio anno."
            lblErrore.Visible = True
            btnOK.Enabled = False
            Exit Sub
        End If
        Dim Chiave As String = "CMInizio"
        If ddlMagazzino.SelectedValue.Trim <> "0" Then
            Chiave = ddlMagazzino.SelectedValue.Trim + "Inizio"
        End If
        App.GetDatiAbilitazioni(CSTABILAZI, Chiave & Session(ESERCIZIO), IDMovCM, strErrore)
        If String.IsNullOrEmpty(IDMovCM) Then IDMovCM = ""
        If IsNumeric(IDMovCM) Then
            lblErrore.Text = "Il trasferimento è già stato effettuato."
            lblErrore.Visible = True
            btnOK.Enabled = False
            Exit Sub
        End If

        lblErrore.Visible = False
        btnOK.Enabled = True
    End Sub
    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click
        'GIU090920 NON VA BENE LA VERSIONE 2020 ORA LA GIACENZA E' QUELLA DELLA DISPONIBILITA' 
        '' ''GIU210220 nella versione 2019 qui faccio la nuova
        ' ''Dim ElabCMInizio As String = ""
        ' ''App.GetDatiAbilitazioni(CSTABILAZI, "ElabCMInizio" & Session(ESERCIZIO), ElabCMInizio, strErrore)
        ' ''If String.IsNullOrEmpty(ElabCMInizio) Then ElabCMInizio = ""
        ' ''If Not IsNumeric(ElabCMInizio) Then
        ' ''    Call GiacInizioAnno()
        ' ''ElseIf ElabCMInizio = "2020" Then
        ' ''    Call GiacInizioAnno2020()
        ' ''End If
        Call GiacInizioAnno()
        btnOK.Enabled = False
    End Sub
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Call CallMenu()
    End Sub
    'giu210220 
    Private Function GiacInizioAnno() As Boolean
        'VERSIONE A TUTTO IL 2019 MA NON VA BENE PER POCHE CENTINAIA DI EURO
        Dim dbcon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim DsMagazzino1 As New DsMagazzino
        Dim ClsPrint As New Magazzino
        Dim Selezione As String
        Dim Inserisci As Boolean
        Dim rowGiac As DsMagazzino.GiacInizioAnnoRow
        Dim myID As String = ""
        Dim DecimaliVal As Integer = 2
        Dim DecimaliPrezzi As Integer = App.GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi

        Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSintetico
        ConnessioneAzi = New SqlConnection
        ConnessioneAziAP = New SqlConnection
        ConnessioneAzi.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi)
        ConnessioneAziAP.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi, CStr(CInt(Session(ESERCIZIO)) - 1))

        'riempimento dataset come per stampa valorizzazione magazzino FIFO
        'GIU080920 OK VA BENE SOLO SU DISPMAGAZZINO CHE FA DA FILTRO SU TUTTO 
        If ClsPrint.StampaDispMagazzino("", "", "", "", DsMagazzino1, strErrore, False, False, False, False, True, ddlMagazzino.SelectedValue.Trim, ConnessioneAziAP) Then
            If DsMagazzino1.DispMagazzino.Count > 0 Then
                'alberto 08/01/2013
                'FIFO
                If Val(ddlMagazzino.SelectedValue.Trim) <> 0 Then 'giu060123
                    Selezione = "WHERE CodiceMagazzino = " & ddlMagazzino.SelectedValue & " AND "
                Else
                    Selezione = "WHERE "
                End If
                Selezione += " Data_Doc <= CONVERT(DATETIME, '" & CDate("31/12/" & CInt(Session(ESERCIZIO)) - 1) & "', 103)"
                Selezione += " ORDER BY view_MovMagValorizzazione.Cod_Articolo, Data_Doc"
                If ClsPrint.ValMagFIFO_CostoVendutoFIFO("", "", "", Selezione, DsMagazzino1, strErrore, False, True, False, False, ConnessioneAziAP) Then
                    If DsMagazzino1.MovMagValorizzazione.Count > 0 Then
                        DsMagazzino1.GiacInizioAnno.Clear()
                        For Each rowVal As DsMagazzino.MovMagValorizzazioneRow In DsMagazzino1.MovMagValorizzazione.Rows
                            If DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(rowVal.Cod_Articolo) Is Nothing Then
                                rowGiac = DsMagazzino1.GiacInizioAnno.NewGiacInizioAnnoRow
                                With rowGiac
                                    .Cod_Articolo = rowVal.Cod_Articolo
                                    .Qta_Rimanente = rowVal.QtaCS
                                    If rowVal.Segno_Giacenza = "+" Then
                                        .ImportoTotale = Math.Round(rowVal.Prezzo_Netto * rowVal.QtaCS, DecimaliVal)
                                    End If
                                    If .Qta_Rimanente > 0 Then
                                        .ImportoUnita = Math.Round(.ImportoTotale / .Qta_Rimanente, DecimaliVal)
                                    Else
                                        .ImportoUnita = 0
                                    End If
                                    .EndEdit()
                                End With
                                DsMagazzino1.GiacInizioAnno.AddGiacInizioAnnoRow(rowGiac)
                            Else
                                rowGiac = DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(rowVal.Cod_Articolo)
                                With rowGiac
                                    .BeginEdit()
                                    .Qta_Rimanente = .Qta_Rimanente + rowVal.QtaCS
                                    If rowVal.Segno_Giacenza = "+" Then
                                        .ImportoTotale = .ImportoTotale + Math.Round(rowVal.Prezzo_Netto * rowVal.QtaCS, DecimaliVal)
                                    End If
                                    If .Qta_Rimanente > 0 Then
                                        .ImportoUnita = Math.Round(.ImportoTotale / .Qta_Rimanente, DecimaliVal)
                                    Else
                                        .ImportoUnita = 0
                                    End If
                                    .EndEdit()
                                End With
                            End If
                        Next
                    Else
                        lblErrore.Text = "Attenzione, Nessun articolo selezionato.<BR>NESSUN MOVIMENTO INSERITO"
                        lblErrore.Visible = True
                        btnOK.Enabled = False
                        Exit Function
                    End If
                Else
                    lblErrore.Text = strErrore
                    lblErrore.Visible = True
                    btnOK.Enabled = False
                    Exit Function
                End If
                '--------------------------
                Dim SqlDbInserCmdForInsert As New SqlCommand
                Dim SqlAdapDocForInsert As New SqlDataAdapter
                Dim DsDocDettForInsert As New DSDocumenti
                Dim RowDettForIns As DSDocumenti.DocumentiDForInsertRow
                Dim RowDett As DsMagazzino.DispMagazzinoRow

                Dim NDoc As Long = GetNewMM()
                Dim NRev As Integer = 0

                'OK CREAZIONE NUOVA SCHEDA INVENTARIO
                Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
                Dim SqlConn As New SqlConnection
                SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
                Dim SqlDbNewCmd As New SqlCommand
                SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewCMInizioAnno]"
                SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbNewCmd.Connection = SqlConn
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Esercizio", System.Data.SqlDbType.NVarChar, 4))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCausale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente", System.Data.SqlDbType.NVarChar, 16))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDoc", System.Data.SqlDbType.NVarChar, 10))
                'GIU040920 GESTIONE MAGAZZINI
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoFatt", System.Data.SqlDbType.NVarChar, 2))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodMag", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                '
                '
                'SqlDbInserCmdForInsert
                '
                SqlDbInserCmdForInsert.CommandText = "[insert_DocDByIDDocumenti_ForInsertCMInizioAnno]"
                SqlDbInserCmdForInsert.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbInserCmdForInsert.Connection = SqlConn
                SqlDbInserCmdForInsert.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), New System.Data.SqlClient.SqlParameter("@Descrizione", System.Data.SqlDbType.NVarChar, 0, "Descrizione"), New System.Data.SqlClient.SqlParameter("@Um", System.Data.SqlDbType.NVarChar, 0, "Um"), New System.Data.SqlClient.SqlParameter("@Qta_Ordinata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Ordinata", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Cod_Iva", System.Data.SqlDbType.Int, 0, "Cod_Iva"), New System.Data.SqlClient.SqlParameter("@Prezzo", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Importo", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(11, Byte), CType(2, Byte), "Importo", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Qta_Evasa", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Evasa", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Qta_Residua", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Residua", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_1", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_1", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_2", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_2", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ScontoReale", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ScontoReale", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ScontoValore", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ScontoValore", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ImportoProvvigione", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ImportoProvvigione", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Pro_Agente", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Pro_Agente", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Cod_Agente", System.Data.SqlDbType.Int, 0, "Cod_Agente"), New System.Data.SqlClient.SqlParameter("@Confezione", System.Data.SqlDbType.Int, 0, "Confezione"), New System.Data.SqlClient.SqlParameter("@Prezzo_Netto", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo_Netto", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@SWPNettoModificato", System.Data.SqlDbType.Bit, 0, "SWPNettoModificato"), New System.Data.SqlClient.SqlParameter("@Prezzo_Netto_Inputato", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo_Netto_Inputato", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_3", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_3", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_4", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_4", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_Pag", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_Pag", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_Merce", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_Merce", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 0, "Note"), New System.Data.SqlClient.SqlParameter("@OmaggioImponibile", System.Data.SqlDbType.Bit, 0, "OmaggioImponibile"), New System.Data.SqlClient.SqlParameter("@OmaggioImposta", System.Data.SqlDbType.Bit, 0, "OmaggioImposta"), New System.Data.SqlClient.SqlParameter("@NumeroPagina", System.Data.SqlDbType.Int, 0, "NumeroPagina"), New System.Data.SqlClient.SqlParameter("@N_Pacchi", System.Data.SqlDbType.Int, 0, "N_Pacchi"), New System.Data.SqlClient.SqlParameter("@Qta_Casse", System.Data.SqlDbType.Int, 0, "Qta_Casse"), New System.Data.SqlClient.SqlParameter("@Flag_Imb", System.Data.SqlDbType.Int, 0, "Flag_Imb"), New System.Data.SqlClient.SqlParameter("@Riga_Trasf", System.Data.SqlDbType.Int, 0, "Riga_Trasf"), New System.Data.SqlClient.SqlParameter("@Riga_Appartenenza", System.Data.SqlDbType.Int, 0, "Riga_Appartenenza"), New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 0, "RefInt"), New System.Data.SqlClient.SqlParameter("@RefNumPrev", System.Data.SqlDbType.Int, 0, "RefNumPrev"), New System.Data.SqlClient.SqlParameter("@RefDataPrev", System.Data.SqlDbType.DateTime, 0, "RefDataPrev"), New System.Data.SqlClient.SqlParameter("@RefNumOrd", System.Data.SqlDbType.Int, 0, "RefNumOrd"), New System.Data.SqlClient.SqlParameter("@RefDataOrd", System.Data.SqlDbType.DateTime, 0, "RefDataOrd"), New System.Data.SqlClient.SqlParameter("@RefNumDDT", System.Data.SqlDbType.Int, 0, "RefNumDDT"), New System.Data.SqlClient.SqlParameter("@RefDataDDT", System.Data.SqlDbType.DateTime, 0, "RefDataDDT"), New System.Data.SqlClient.SqlParameter("@RefNumNC", System.Data.SqlDbType.Int, 0, "RefNumNC"), New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 0, "RefDataNC"), _
                      New System.Data.SqlClient.SqlParameter("@LBase", System.Data.SqlDbType.Int, 0, "LBase"), _
                      New System.Data.SqlClient.SqlParameter("@LOpz", System.Data.SqlDbType.Int, 0, "LOpz"), _
                      New System.Data.SqlClient.SqlParameter("@Qta_Impegnata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Impegnata", System.Data.DataRowVersion.Current, Nothing), _
                      New System.Data.SqlClient.SqlParameter("@Qta_Prenotata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Prenotata", System.Data.DataRowVersion.Current, Nothing), _
                      New System.Data.SqlClient.SqlParameter("@Qta_Allestita", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Allestita", System.Data.DataRowVersion.Current, Nothing), _
                      New System.Data.SqlClient.SqlParameter("@PrezzoListino", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "PrezzoListino", System.Data.DataRowVersion.Current, Nothing), _
                      New System.Data.SqlClient.SqlParameter("@PrezzoAcquisto", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "PrezzoAcquisto", System.Data.DataRowVersion.Current, Nothing), _
                      New System.Data.SqlClient.SqlParameter("@SWModAgenti", System.Data.SqlDbType.Bit, 0, "SWModAgenti")})
                SqlAdapDocForInsert.InsertCommand = SqlDbInserCmdForInsert
                'giu150617
                Dim strValore As String = ""
                ' ''Dim strErrore As String = ""
                Dim myTimeOUT As Long = 5000
                If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                    If IsNumeric(strValore.Trim) Then
                        If CLng(strValore.Trim) > myTimeOUT Then
                            myTimeOUT = CLng(strValore.Trim)
                        End If
                    End If
                End If
                'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
                '---------------------------
                '=========================================
                Dim NRiga As Integer = 0 'PER RIGA = 0 CREO LA TESTATA
                For Each RowDett In DsMagazzino1.DispMagazzino
                    If RowDett.RowState <> DataRowState.Deleted Then
                        If NRiga = 0 Then
                            SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.CaricoMagazzino)
                            SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
                            NRev = NRev + 1
                            SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
                            SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                            SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                            SqlDbNewCmd.Parameters.Item("@Esercizio").Value = Session(ESERCIZIO)
                            SqlDbNewCmd.Parameters.Item("@CodCausale").Value = Session("CodCausale")
                            SqlDbNewCmd.Parameters.Item("@CodCliente").Value = Session("CodCoGeDitta")
                            SqlDbNewCmd.Parameters.Item("@DataDoc").Value = "01/01/" & Session(ESERCIZIO).ToString.Trim
                            SqlDbNewCmd.Parameters.Item("@TipoFatt").Value = App.GetParamGestAzi(Session(ESERCIZIO)).CodTipoFatt
                            SqlDbNewCmd.Parameters.Item("@CodMag").Value = ddlMagazzino.SelectedValue.Trim
                            Try
                                SqlConn.Open()
                                SqlDbNewCmd.CommandTimeout = myTimeOUT '5000
                                SqlDbNewCmd.ExecuteNonQuery()
                                SqlConn.Close()
                                Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
                            Catch ExSQL As SqlException
                                lblErrore.Text = "Errore: " & ExSQL.Message
                                lblErrore.Visible = True
                                btnOK.Enabled = False
                                Exit Function
                            Catch Ex As Exception
                                lblErrore.Text = "Errore: " & Ex.Message
                                lblErrore.Visible = True
                                btnOK.Enabled = False
                                Exit Function
                            End Try
                            NRiga = 1
                        End If
                        Try
                            myID = Session(IDDOCUMENTI)
                            If IsNothing(myID) Then
                                myID = ""
                            End If
                            If String.IsNullOrEmpty(myID) Then
                                myID = ""
                            End If
                            If Not IsNumeric(myID) Then
                                lblErrore.Text = "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO."
                                lblErrore.Visible = True
                                btnOK.Enabled = False
                                Exit Function
                            End If
                            '===PREPARO GLI ARTICOLO DA INSERIRE ============================
                            rowGiac = DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(RowDett.Cod_Articolo)
                            If (Not rowGiac Is Nothing) Then
                                If (RowDett.Giacenza + RowDett.Giac_Impegnata) > 0 Then 'GIU090920 (rowGiac.Qta_Rimanente > 0) Then
                                    Inserisci = True
                                Else
                                    Inserisci = False
                                    Continue For
                                End If
                            Else
                                Inserisci = False
                                Continue For
                            End If
                            If Inserisci Then
                                RowDettForIns = DsDocDettForInsert.DocumentiDForInsert.NewRow
                                RowDettForIns.Item("IDDOCUMENTI") = myID
                                RowDettForIns.Item("Riga") = NRiga
                                RowDettForIns.Item("COD_ARTICOLO") = RowDett.Cod_Articolo
                                RowDettForIns.Item("Descrizione") = RowDett.Descrizione
                                RowDettForIns.Item("Cod_Iva") = 0
                                RowDettForIns.Item("Prezzo") = Math.Round(rowGiac.ImportoUnita, DecimaliPrezzi)
                                RowDettForIns.Item("Prezzo_Netto") = Math.Round(rowGiac.ImportoUnita, DecimaliPrezzi)
                                RowDettForIns.Item("Prezzo_Netto_Inputato") = Math.Round(rowGiac.ImportoUnita, DecimaliPrezzi)
                                RowDettForIns.Item("Qta_Ordinata") = rowGiac.Qta_Rimanente 'giu060123 RowDett.Giacenza + RowDett.Giac_Impegnata 'GIU090920 rowGiac.Qta_Rimanente
                                RowDettForIns.Item("Qta_Evasa") = rowGiac.Qta_Rimanente 'giu060123 RowDett.Giacenza + RowDett.Giac_Impegnata 'GIU090920 rowGiac.Qta_Rimanente
                                RowDettForIns.Item("Importo") = Math.Round(rowGiac.ImportoTotale, DecimaliVal) 'giu060123 Math.Round(RowDettForIns.Item("Prezzo") * RowDettForIns.Item("Qta_Evasa"), DecimaliVal) 'GIU090920 rowGiac.ImportoTotale
                                RowDettForIns.Item("SWPNettoModificato") = 0
                                RowDettForIns.Item("Sconto_1") = 0
                                RowDettForIns.Item("Sconto_2") = 0
                                RowDettForIns.Item("Sconto_3") = 0
                                RowDettForIns.Item("Sconto_4") = 0
                                RowDettForIns.Item("Sconto_Pag") = 0
                                RowDettForIns.Item("ScontoValore") = 0
                                RowDettForIns.Item("Sconto_Merce") = 0
                                RowDettForIns.Item("ScontoReale") = 0
                                RowDettForIns.Item("Um") = "PZ" 'Aggiorno nella Store quando setto i prezzi
                                If RowDettForIns.Item("Qta_Evasa") < 0 Then
                                    RowDettForIns.Item("Qta_Evasa") = 0
                                    RowDettForIns.Item("Importo") = 0 'giu060123
                                End If
                                RowDettForIns.Item("Qta_Residua") = 0
                                RowDettForIns.Item("Cod_Agente") = DBNull.Value
                                RowDettForIns.Item("Pro_Agente") = 0
                                RowDettForIns.Item("ImportoProvvigione") = 0
                                RowDettForIns.Item("Note") = ""
                                RowDettForIns.Item("OmaggioImponibile") = 0
                                RowDettForIns.Item("OmaggioImposta") = 0
                                RowDettForIns.Item("NumeroPagina") = 0
                                RowDettForIns.Item("N_Pacchi") = 0
                                RowDettForIns.Item("Qta_Casse") = 0
                                RowDettForIns.Item("Flag_Imb") = 0
                                RowDettForIns.Item("Confezione") = 0
                                RowDettForIns.Item("Riga_Trasf") = DBNull.Value
                                RowDettForIns.Item("Riga_Appartenenza") = DBNull.Value
                                RowDettForIns.Item("RefInt") = DBNull.Value
                                RowDettForIns.Item("RefNumPrev") = DBNull.Value
                                RowDettForIns.Item("RefDataPrev") = DBNull.Value
                                RowDettForIns.Item("RefNumOrd") = DBNull.Value
                                RowDettForIns.Item("RefDataOrd") = DBNull.Value
                                RowDettForIns.Item("RefNumDDT") = DBNull.Value
                                RowDettForIns.Item("RefDataDDT") = DBNull.Value
                                RowDettForIns.Item("RefNumNC") = DBNull.Value
                                RowDettForIns.Item("RefDataNC") = DBNull.Value
                                RowDettForIns.Item("LBase") = 0 'Aggiorno nella Store quando setto i prezzi
                                RowDettForIns.Item("LOpz") = 0 'Aggiorno nella Store quando setto i prezzi
                                RowDettForIns.Item("Qta_Impegnata") = 0
                                RowDettForIns.Item("Qta_Prenotata") = 0
                                RowDettForIns.Item("Qta_Allestita") = 0
                                DsDocDettForInsert.DocumentiDForInsert.AddDocumentiDForInsertRow(RowDettForIns)
                                NRiga = NRiga + 1
                            End If
                            '===============================================================
                        Catch ex As Exception
                            lblErrore.Text = "Errore: " & ex.Message
                            lblErrore.Visible = True
                            btnOK.Enabled = False
                            Exit Function
                        End Try
                    End If
                Next
                Try
                    SqlAdapDocForInsert.Update(DsDocDettForInsert.DocumentiDForInsert)
                Catch ExSQL As SqlException
                    lblErrore.Text = "Errore: " & ExSQL.Message
                    lblErrore.Visible = True
                    btnOK.Enabled = False
                    Exit Function
                Catch Ex As Exception
                    lblErrore.Text = "Errore: " & Ex.Message
                    lblErrore.Visible = True
                    btnOK.Enabled = False
                    Exit Function
                End Try
                'alb29012013 memorizzo l'ID del movimento in DB Opzioni per bloccare il
                'trasferiento giacenze dopo la prima volta
                Dim SQLConnOpz As New SqlConnection
                Dim SQLCmdUpdOpz As New SqlCommand
                SQLConnOpz.ConnectionString = dbConn.getConnectionString(TipoDB.dbOpzioni)
                'giu080920
                Dim Chiave As String = "CMInizio"
                If ddlMagazzino.SelectedValue.Trim <> "0" Then
                    Chiave = ddlMagazzino.SelectedValue.Trim + "Inizio"
                End If
                '----
                SQLCmdUpdOpz.CommandType = CommandType.Text
                SQLCmdUpdOpz.Connection = SQLConnOpz
                SQLCmdUpdOpz.CommandText = "IF EXISTS(SELECT Chiave FROM Abilitazioni WHERE Chiave = '" + Chiave.Trim & Session(ESERCIZIO) & "') " _
                                            & " UPDATE Abilitazioni SET Descrizione = '" & myID & "' WHERE Chiave = '" + Chiave.Trim & Session(ESERCIZIO) & "' ELSE " _
                                            & "INSERT INTO Abilitazioni VALUES ('" + Chiave.Trim & Session(ESERCIZIO) & "', '" & myID & "', -1)"
                Try
                    If SQLConnOpz.State <> ConnectionState.Open Then
                        SQLConnOpz.Open()
                    End If

                    SQLCmdUpdOpz.ExecuteNonQuery()

                    SQLConnOpz.Close()
                Catch ex As Exception
                    lblErrore.Text = "Si è verificato il seguente errore: " & ex.Message
                    lblErrore.Visible = True
                    btnOK.Enabled = False
                    Exit Function
                End Try
                '-------------------
                'giu040213
                Session(MODALPOPUP_CALLBACK_METHOD) = "CallMenu"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Trasferimento giacenze di inizio anno", "Operazione completata con successo", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Function
                ' ''lblerrore.Text = "Operazione completata con successo"
                ' ''Exit Function
            Else
                lblErrore.Text = "Attenzione, Nessun articolo selezionato.<BR>NESSUN MOVIMENTO INSERITO"
                lblErrore.Visible = True
                btnOK.Enabled = False
                Exit Function
            End If
        Else
            lblErrore.Text = "Errore: " & strErrore
            lblErrore.Visible = True
            btnOK.Enabled = False
            Exit Function
        End If
    End Function
    'GIU090920 GiacInizioAnno2020 NON VA BENE PER LA GESTIONE MAGAZZINI
    '' ''GIU090920 NON VA PIU' BENE SE NON PER IL MAGAZZINO PRINCIPALE
    ' ''Private Function GiacInizioAnno2020() As Boolean
    ' ''    'GIU090920 NON VA PIU' BENE SE NON PER IL MAGAZZINO PRINCIPALE
    ' ''    Dim dbcon As New dbStringaConnesioneFacade
    ' ''    Dim DsMagazzino1 As New DsMagazzino
    ' ''    Dim ClsPrint As New Magazzino
    ' ''    Dim Selezione As String
    ' ''    Dim Inserisci As Boolean
    ' ''    Dim rowGiac As DsMagazzino.GiacInizioAnnoRow
    ' ''    Dim rowGiac1 As DsMagazzino.GiacInizioAnno1Row
    ' ''    Dim myID As String = ""
    ' ''    Dim PrimomyID As String = ""
    ' ''    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSintetico
    ' ''    ConnessioneAzi = New SqlConnection
    ' ''    ConnessioneAziAP = New SqlConnection
    ' ''    ConnessioneAzi.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi)
    ' ''    ConnessioneAziAP.ConnectionString = dbcon.getConnectionString(TipoDB.dbSoftAzi, CStr(CInt(Session(ESERCIZIO)) - 1))

    ' ''    'riempimento dataset come per stampa valorizzazione magazzino FIFO
    ' ''    'GIU080920 OK VA BENE SOLO SU DISPMAGAZZINO CHE FA DA FILTRO SU TUTTO 
    ' ''    If ClsPrint.StampaDispMagazzino("", "", "", "", DsMagazzino1, strErrore, False, False, False, False, True, ddlMagazzino.SelectedValue.Trim, ConnessioneAziAP) Then
    ' ''        If DsMagazzino1.DispMagazzino.Count > 0 Then
    ' ''            'alberto 08/01/2013
    ' ''            'FIFO
    ' ''            Selezione = "WHERE Data_Doc <= CONVERT(DATETIME, '" & CDate("31/12/" & CInt(Session(ESERCIZIO)) - 1) & "', 103)"
    ' ''            Selezione += " ORDER BY view_MovMagValorizzazione.Cod_Articolo, Data_Doc"
    ' ''            'GIU040920 MA MODIFICARE ANCHE ClsPrint.ValMagFIFO_CostoVendutoFIFO
    ' ''            If ClsPrint.ValMagFIFO_CostoVendutoFIFO("", "", "", Selezione, DsMagazzino1, strErrore, False, True, False, False, ConnessioneAziAP) Then
    ' ''                If DsMagazzino1.MovMagValorizzazione.Count > 0 Then
    ' ''                    DsMagazzino1.GiacInizioAnno.Clear() : DsMagazzino1.GiacInizioAnno1.Clear()
    ' ''                    For Each rowVal As DsMagazzino.MovMagValorizzazioneRow In DsMagazzino1.MovMagValorizzazione.Rows
    ' ''                        If DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(rowVal.Cod_Articolo) Is Nothing Then
    ' ''                            rowGiac = DsMagazzino1.GiacInizioAnno.NewGiacInizioAnnoRow
    ' ''                            With rowGiac
    ' ''                                .Cod_Articolo = rowVal.Cod_Articolo
    ' ''                                .Qta_Rimanente = rowVal.QtaCS
    ' ''                                .ImportoTotale = rowVal.Prezzo_Netto * rowVal.QtaCS
    ' ''                                If .Qta_Rimanente > 0 Then
    ' ''                                    .ImportoUnita = .ImportoTotale / .Qta_Rimanente
    ' ''                                End If
    ' ''                                .EndEdit()
    ' ''                            End With
    ' ''                            DsMagazzino1.GiacInizioAnno.AddGiacInizioAnnoRow(rowGiac)
    ' ''                        Else
    ' ''                            rowGiac = DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(rowVal.Cod_Articolo)
    ' ''                            With rowGiac
    ' ''                                .BeginEdit()
    ' ''                                .Qta_Rimanente = .Qta_Rimanente + rowVal.QtaCS
    ' ''                                .ImportoTotale = .ImportoTotale + rowVal.Prezzo_Netto * rowVal.QtaCS
    ' ''                                If .Qta_Rimanente > 0 Then
    ' ''                                    .ImportoUnita = .ImportoTotale / .Qta_Rimanente
    ' ''                                End If
    ' ''                                .EndEdit()
    ' ''                            End With
    ' ''                        End If
    ' ''                        '-
    ' ''                        rowGiac1 = DsMagazzino1.GiacInizioAnno1.NewGiacInizioAnno1Row
    ' ''                        With rowGiac1
    ' ''                            .Cod_Articolo = rowVal.Cod_Articolo
    ' ''                            .Qta_Rimanente = rowVal.QtaCS
    ' ''                            .ImportoTotale = rowVal.Prezzo_Netto * rowVal.QtaCS
    ' ''                            If .Qta_Rimanente > 0 Then
    ' ''                                .ImportoUnita = .ImportoTotale / .Qta_Rimanente
    ' ''                            End If
    ' ''                            .DataDoc = rowVal.Data_Doc
    ' ''                            .EndEdit()
    ' ''                        End With
    ' ''                        DsMagazzino1.GiacInizioAnno1.AddGiacInizioAnno1Row(rowGiac1)
    ' ''                    Next
    ' ''                Else
    ' ''                    lblErrore.Text = "Attenzione, Nessun articolo selezionato.<BR>NESSUN MOVIMENTO INSERITO"
    ' ''                    lblErrore.Visible = True
    ' ''                    btnOK.Enabled = False
    ' ''                    Exit Function
    ' ''                End If
    ' ''            Else
    ' ''                lblErrore.Text = strErrore
    ' ''                lblErrore.Visible = True
    ' ''                btnOK.Enabled = False
    ' ''                Exit Function
    ' ''            End If
    ' ''            'GIU090920 NON VA BENE PERCHE' LA QTA' E' QUELLA DI DISPMAGAZZINO
    ' ''            '' ''giu200220()
    ' ''            ' ''For Each rowGiac1 In DsMagazzino1.GiacInizioAnno1.Select("Qta_rimanente=0")
    ' ''            ' ''    If rowGiac1.RowState <> DataRowState.Deleted Then
    ' ''            ' ''        rowGiac1.Delete()
    ' ''            ' ''    End If
    ' ''            ' ''Next
    ' ''            ' ''DsMagazzino1.GiacInizioAnno1.AcceptChanges()
    ' ''            '' ''GIU210220
    ' ''            ' ''For Each rowGiac In DsMagazzino1.GiacInizioAnno.Select("Qta_rimanente=0")
    ' ''            ' ''    If rowGiac.RowState <> DataRowState.Deleted Then
    ' ''            ' ''        For Each rowGiac1 In DsMagazzino1.GiacInizioAnno1.Select("Cod_Articolo='" & rowGiac.Cod_Articolo.Trim & "'")
    ' ''            ' ''            If rowGiac1.RowState <> DataRowState.Deleted Then
    ' ''            ' ''                rowGiac1.Delete()
    ' ''            ' ''            End If
    ' ''            ' ''        Next
    ' ''            ' ''    End If
    ' ''            ' ''Next
    ' ''            ' ''DsMagazzino1.GiacInizioAnno1.AcceptChanges()
    ' ''            '------------------

    ' ''            Dim SqlDbInserCmdForInsert As New SqlCommand
    ' ''            Dim SqlAdapDocForInsert As New SqlDataAdapter
    ' ''            Dim DsDocDettForInsert As New DSDocumenti
    ' ''            Dim RowDettForIns As DSDocumenti.DocumentiDForInsertRow
    ' ''            Dim RowDett As DsMagazzino.DispMagazzinoRow

    ' ''            Dim NDoc As Long = GetNewMM()
    ' ''            Dim NRev As Integer = 0

    ' ''            'OK CREAZIONE NUOVA SCHEDA INVENTARIO
    ' ''            Dim dbConn As New dbStringaConnesioneFacade
    ' ''            Dim SqlConn As New SqlConnection
    ' ''            SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
    ' ''            '
    ' ''            'SqlDbInserCmdForInsert
    ' ''            '
    ' ''            SqlDbInserCmdForInsert.CommandText = "[insert_DocDByIDDocumenti_ForInsertCMInizioAnno]"
    ' ''            SqlDbInserCmdForInsert.CommandType = System.Data.CommandType.StoredProcedure
    ' ''            SqlDbInserCmdForInsert.Connection = SqlConn
    ' ''            SqlDbInserCmdForInsert.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), New System.Data.SqlClient.SqlParameter("@Descrizione", System.Data.SqlDbType.NVarChar, 0, "Descrizione"), New System.Data.SqlClient.SqlParameter("@Um", System.Data.SqlDbType.NVarChar, 0, "Um"), New System.Data.SqlClient.SqlParameter("@Qta_Ordinata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Ordinata", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Cod_Iva", System.Data.SqlDbType.Int, 0, "Cod_Iva"), New System.Data.SqlClient.SqlParameter("@Prezzo", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Importo", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(11, Byte), CType(2, Byte), "Importo", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Qta_Evasa", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Evasa", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Qta_Residua", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Residua", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_1", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_1", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_2", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_2", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ScontoReale", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ScontoReale", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ScontoValore", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ScontoValore", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ImportoProvvigione", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ImportoProvvigione", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Pro_Agente", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Pro_Agente", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Cod_Agente", System.Data.SqlDbType.Int, 0, "Cod_Agente"), New System.Data.SqlClient.SqlParameter("@Confezione", System.Data.SqlDbType.Int, 0, "Confezione"), New System.Data.SqlClient.SqlParameter("@Prezzo_Netto", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo_Netto", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@SWPNettoModificato", System.Data.SqlDbType.Bit, 0, "SWPNettoModificato"), New System.Data.SqlClient.SqlParameter("@Prezzo_Netto_Inputato", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo_Netto_Inputato", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_3", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_3", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_4", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_4", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_Pag", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_Pag", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_Merce", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_Merce", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 0, "Note"), New System.Data.SqlClient.SqlParameter("@OmaggioImponibile", System.Data.SqlDbType.Bit, 0, "OmaggioImponibile"), New System.Data.SqlClient.SqlParameter("@OmaggioImposta", System.Data.SqlDbType.Bit, 0, "OmaggioImposta"), New System.Data.SqlClient.SqlParameter("@NumeroPagina", System.Data.SqlDbType.Int, 0, "NumeroPagina"), New System.Data.SqlClient.SqlParameter("@N_Pacchi", System.Data.SqlDbType.Int, 0, "N_Pacchi"), New System.Data.SqlClient.SqlParameter("@Qta_Casse", System.Data.SqlDbType.Int, 0, "Qta_Casse"), New System.Data.SqlClient.SqlParameter("@Flag_Imb", System.Data.SqlDbType.Int, 0, "Flag_Imb"), New System.Data.SqlClient.SqlParameter("@Riga_Trasf", System.Data.SqlDbType.Int, 0, "Riga_Trasf"), New System.Data.SqlClient.SqlParameter("@Riga_Appartenenza", System.Data.SqlDbType.Int, 0, "Riga_Appartenenza"), New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 0, "RefInt"), New System.Data.SqlClient.SqlParameter("@RefNumPrev", System.Data.SqlDbType.Int, 0, "RefNumPrev"), New System.Data.SqlClient.SqlParameter("@RefDataPrev", System.Data.SqlDbType.DateTime, 0, "RefDataPrev"), New System.Data.SqlClient.SqlParameter("@RefNumOrd", System.Data.SqlDbType.Int, 0, "RefNumOrd"), New System.Data.SqlClient.SqlParameter("@RefDataOrd", System.Data.SqlDbType.DateTime, 0, "RefDataOrd"), New System.Data.SqlClient.SqlParameter("@RefNumDDT", System.Data.SqlDbType.Int, 0, "RefNumDDT"), New System.Data.SqlClient.SqlParameter("@RefDataDDT", System.Data.SqlDbType.DateTime, 0, "RefDataDDT"), New System.Data.SqlClient.SqlParameter("@RefNumNC", System.Data.SqlDbType.Int, 0, "RefNumNC"), New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 0, "RefDataNC"), _
    ' ''                  New System.Data.SqlClient.SqlParameter("@LBase", System.Data.SqlDbType.Int, 0, "LBase"), _
    ' ''                  New System.Data.SqlClient.SqlParameter("@LOpz", System.Data.SqlDbType.Int, 0, "LOpz"), _
    ' ''                  New System.Data.SqlClient.SqlParameter("@Qta_Impegnata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Impegnata", System.Data.DataRowVersion.Current, Nothing), _
    ' ''                  New System.Data.SqlClient.SqlParameter("@Qta_Prenotata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Prenotata", System.Data.DataRowVersion.Current, Nothing), _
    ' ''                  New System.Data.SqlClient.SqlParameter("@Qta_Allestita", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Allestita", System.Data.DataRowVersion.Current, Nothing), _
    ' ''                  New System.Data.SqlClient.SqlParameter("@PrezzoListino", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "PrezzoListino", System.Data.DataRowVersion.Current, Nothing), _
    ' ''                  New System.Data.SqlClient.SqlParameter("@PrezzoAcquisto", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "PrezzoAcquisto", System.Data.DataRowVersion.Current, Nothing), _
    ' ''                  New System.Data.SqlClient.SqlParameter("@SWModAgenti", System.Data.SqlDbType.Bit, 0, "SWModAgenti")})
    ' ''            SqlAdapDocForInsert.InsertCommand = SqlDbInserCmdForInsert
    ' ''            'giu150617
    ' ''            Dim strValore As String = ""
    ' ''            ' ''Dim strErrore As String = ""
    ' ''            Dim myTimeOUT As Long = 5000
    ' ''            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
    ' ''                If IsNumeric(strValore.Trim) Then
    ' ''                    If CLng(strValore.Trim) > myTimeOUT Then
    ' ''                        myTimeOUT = CLng(strValore.Trim)
    ' ''                    End If
    ' ''                End If
    ' ''            End If
    ' ''            'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
    ' ''            '---------------------------
    ' ''            '=========================================
    ' ''            Session(IDDOCUMENTI) = ""
    ' ''            For Each RowDett In DsMagazzino1.DispMagazzino
    ' ''                If RowDett.RowState <> DataRowState.Deleted Then
    ' ''                    rowGiac = DsMagazzino1.GiacInizioAnno.FindByCod_Articolo(RowDett.Cod_Articolo)
    ' ''                    If (Not rowGiac Is Nothing) Then
    ' ''                        If (RowDett.Giacenza + RowDett.Giac_Impegnata) > 0 Then 'GIU090920 (rowGiac.Qta_Rimanente > 0) Then
    ' ''                            Inserisci = True
    ' ''                        Else
    ' ''                            Inserisci = False
    ' ''                            Continue For
    ' ''                        End If
    ' ''                    Else
    ' ''                        Inserisci = False
    ' ''                        Continue For
    ' ''                    End If
    ' ''                    '-
    ' ''                    For Each rowGiac1 In DsMagazzino1.GiacInizioAnno1.Select("Cod_Articolo='" & RowDett.Cod_Articolo.Trim & "'", "DataDoc")
    ' ''                        If CreateDocT(myTimeOUT, NDoc, NRev, Format(rowGiac1.DataDoc, FormatoData)) = False Then
    ' ''                            Exit Function
    ' ''                        End If
    ' ''                        Try
    ' ''                            myID = Session(IDDOCUMENTI)
    ' ''                            If IsNothing(myID) Then
    ' ''                                myID = ""
    ' ''                            End If
    ' ''                            If String.IsNullOrEmpty(myID) Then
    ' ''                                myID = ""
    ' ''                            End If
    ' ''                            If Not IsNumeric(myID) Then
    ' ''                                lblErrore.Text = "Errore: NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO."
    ' ''                                lblErrore.Visible = True
    ' ''                                btnOK.Enabled = False
    ' ''                                Exit Function
    ' ''                            End If
    ' ''                            '===PREPARO GLI ARTICOLO DA INSERIRE ============================
    ' ''                            If Inserisci Then
    ' ''                                RowDettForIns = DsDocDettForInsert.DocumentiDForInsert.NewRow
    ' ''                                RowDettForIns.Item("IDDOCUMENTI") = myID
    ' ''                                RowDettForIns.Item("Riga") = 1
    ' ''                                RowDettForIns.Item("COD_ARTICOLO") = RowDett.Cod_Articolo.Trim
    ' ''                                RowDettForIns.Item("Descrizione") = RowDett.Descrizione
    ' ''                                RowDettForIns.Item("Cod_Iva") = 0
    ' ''                                'GIU090920
    ' ''                                RowDettForIns.Item("Prezzo") = Math.Round(rowGiac.ImportoUnita, 2)
    ' ''                                RowDettForIns.Item("Prezzo_Netto") = Math.Round(rowGiac.ImportoUnita, 2)
    ' ''                                RowDettForIns.Item("Prezzo_Netto_Inputato") = Math.Round(rowGiac.ImportoUnita, 2)
    ' ''                                RowDettForIns.Item("Qta_Ordinata") = RowDett.Giacenza + RowDett.Giac_Impegnata 'GIU090920 rowGiac.Qta_Rimanente
    ' ''                                RowDettForIns.Item("Qta_Evasa") = RowDett.Giacenza + RowDett.Giac_Impegnata 'GIU090920 rowGiac.Qta_Rimanente
    ' ''                                RowDettForIns.Item("Importo") = Math.Round(RowDettForIns.Item("Prezzo") * RowDettForIns.Item("Qta_Evasa"), 2) 'GIU090920 rowGiac.ImportoTotale
    ' ''                                '--
    ' ''                                RowDettForIns.Item("SWPNettoModificato") = 0
    ' ''                                RowDettForIns.Item("Sconto_1") = 0
    ' ''                                RowDettForIns.Item("Sconto_2") = 0
    ' ''                                RowDettForIns.Item("Sconto_3") = 0
    ' ''                                RowDettForIns.Item("Sconto_4") = 0
    ' ''                                RowDettForIns.Item("Sconto_Pag") = 0
    ' ''                                RowDettForIns.Item("ScontoValore") = 0
    ' ''                                RowDettForIns.Item("Sconto_Merce") = 0
    ' ''                                RowDettForIns.Item("ScontoReale") = 0
    ' ''                                RowDettForIns.Item("Um") = "PZ" 'Aggiorno nella Store quando setto i prezzi
    ' ''                                If RowDettForIns.Item("Qta_Evasa") < 0 Then
    ' ''                                    RowDettForIns.Item("Qta_Evasa") = 0
    ' ''                                End If
    ' ''                                RowDettForIns.Item("Qta_Residua") = 0
    ' ''                                RowDettForIns.Item("Cod_Agente") = DBNull.Value
    ' ''                                RowDettForIns.Item("Pro_Agente") = 0
    ' ''                                RowDettForIns.Item("ImportoProvvigione") = 0
    ' ''                                RowDettForIns.Item("Note") = ""
    ' ''                                RowDettForIns.Item("OmaggioImponibile") = 0
    ' ''                                RowDettForIns.Item("OmaggioImposta") = 0
    ' ''                                RowDettForIns.Item("NumeroPagina") = 0
    ' ''                                RowDettForIns.Item("N_Pacchi") = 0
    ' ''                                RowDettForIns.Item("Qta_Casse") = 0
    ' ''                                RowDettForIns.Item("Flag_Imb") = 0
    ' ''                                RowDettForIns.Item("Confezione") = 0
    ' ''                                RowDettForIns.Item("Riga_Trasf") = DBNull.Value
    ' ''                                RowDettForIns.Item("Riga_Appartenenza") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefInt") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefNumPrev") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefDataPrev") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefNumOrd") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefDataOrd") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefNumDDT") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefDataDDT") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefNumNC") = DBNull.Value
    ' ''                                RowDettForIns.Item("RefDataNC") = DBNull.Value
    ' ''                                RowDettForIns.Item("LBase") = 0 'Aggiorno nella Store quando setto i prezzi
    ' ''                                RowDettForIns.Item("LOpz") = 0 'Aggiorno nella Store quando setto i prezzi
    ' ''                                RowDettForIns.Item("Qta_Impegnata") = 0
    ' ''                                RowDettForIns.Item("Qta_Prenotata") = 0
    ' ''                                RowDettForIns.Item("Qta_Allestita") = 0
    ' ''                                DsDocDettForInsert.DocumentiDForInsert.AddDocumentiDForInsertRow(RowDettForIns)
    ' ''                            End If
    ' ''                            '===============================================================
    ' ''                        Catch ex As Exception
    ' ''                            lblErrore.Text = "Errore: " & ex.Message
    ' ''                            lblErrore.Visible = True
    ' ''                            btnOK.Enabled = False
    ' ''                            Exit Function
    ' ''                        End Try
    ' ''                    Next
    ' ''                End If
    ' ''            Next
    ' ''            '---------
    ' ''            Try
    ' ''                SqlAdapDocForInsert.Update(DsDocDettForInsert.DocumentiDForInsert)
    ' ''            Catch ExSQL As SqlException
    ' ''                lblErrore.Text = "Errore: " & ExSQL.Message
    ' ''                lblErrore.Visible = True
    ' ''                btnOK.Enabled = False
    ' ''                Exit Function
    ' ''            Catch Ex As Exception
    ' ''                lblErrore.Text = "Errore: " & Ex.Message
    ' ''                lblErrore.Visible = True
    ' ''                btnOK.Enabled = False
    ' ''                Exit Function
    ' ''            End Try
    ' ''            'alb29012013 memorizzo l'ID del movimento in DB Opzioni per bloccare il
    ' ''            'trasferiento giacenze dopo la prima volta
    ' ''            Dim SQLConnOpz As New SqlConnection
    ' ''            Dim SQLCmdUpdOpz As New SqlCommand
    ' ''            SQLConnOpz.ConnectionString = dbConn.getConnectionString(TipoDB.dbOpzioni)
    ' ''            'giu080920
    ' ''            Dim Chiave As String = "CMInizio"
    ' ''            If ddlMagazzino.SelectedValue.Trim <> "0" Then
    ' ''                Chiave = ddlMagazzino.SelectedValue.Trim + "Inizio"
    ' ''            End If
    ' ''            '-
    ' ''            Dim ChiaveF As String = "CMInizio"
    ' ''            If ddlMagazzino.SelectedValue.Trim <> "0" Then
    ' ''                Chiave = ddlMagazzino.SelectedValue.Trim + "Fine"
    ' ''            End If
    ' ''            '----
    ' ''            PrimomyID = Session("PrimoMyID")
    ' ''            SQLCmdUpdOpz.CommandType = CommandType.Text
    ' ''            SQLCmdUpdOpz.Connection = SQLConnOpz
    ' ''            SQLCmdUpdOpz.CommandText = "IF EXISTS(SELECT Chiave FROM Abilitazioni WHERE Chiave = '" & Chiave.Trim + Session(ESERCIZIO) & "') " _
    ' ''                                        & " UPDATE Abilitazioni SET Descrizione = '" & PrimomyID & "' WHERE Chiave = '" & Chiave.Trim + Session(ESERCIZIO) & "' ELSE " _
    ' ''                                        & "INSERT INTO Abilitazioni VALUES ('" & Chiave.Trim + Session(ESERCIZIO) & "', '" & PrimomyID & "', -1)"
    ' ''            Try
    ' ''                If SQLConnOpz.State <> ConnectionState.Open Then
    ' ''                    SQLConnOpz.Open()
    ' ''                End If

    ' ''                SQLCmdUpdOpz.ExecuteNonQuery()

    ' ''                SQLCmdUpdOpz.CommandText = "IF EXISTS(SELECT Chiave FROM Abilitazioni WHERE Chiave = '" & ChiaveF.Trim + Session(ESERCIZIO) & "') " _
    ' ''                                       & " UPDATE Abilitazioni SET Descrizione = '" & myID & "' WHERE Chiave = '" & ChiaveF.Trim + Session(ESERCIZIO) & "' ELSE " _
    ' ''                                       & "INSERT INTO Abilitazioni VALUES ('" & ChiaveF.Trim + Session(ESERCIZIO) & "', '" & myID & "', -1)"
    ' ''                SQLCmdUpdOpz.ExecuteNonQuery()
    ' ''                SQLConnOpz.Close()
    ' ''            Catch ex As Exception
    ' ''                lblErrore.Text = "Si è verificato il seguente errore: " & ex.Message
    ' ''                lblErrore.Visible = True
    ' ''                btnOK.Enabled = False
    ' ''                Exit Function
    ' ''            End Try
    ' ''            '-------------------
    ' ''            'giu040213
    ' ''            Session(MODALPOPUP_CALLBACK_METHOD) = "CallMenu"
    ' ''            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''            ModalPopup.Show("Trasferimento giacenze di inizio anno", "Operazione completata con successo", WUC_ModalPopup.TYPE_CONFIRM_Y)
    ' ''            Exit Function
    ' ''            ' ''lblerrore.Text = "Operazione completata con successo"
    ' ''            ' ''Exit Function
    ' ''        Else
    ' ''            lblErrore.Text = "Attenzione, Nessun articolo selezionato.<BR>NESSUN MOVIMENTO INSERITO"
    ' ''            lblErrore.Visible = True
    ' ''            btnOK.Enabled = False
    ' ''            Exit Function
    ' ''        End If
    ' ''    Else
    ' ''        lblErrore.Text = "Errore: " & strErrore
    ' ''        lblErrore.Visible = True
    ' ''        btnOK.Enabled = False
    ' ''        Exit Function
    ' ''    End If
    ' ''End Function
    Private Function GetNewMM() As Long
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE Tipo_Doc = '" & TipoDoc.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewMM = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewMM = 1
                    End If
                    Exit Function
                Else
                    GetNewMM = 1
                    Exit Function
                End If
            Else
                GetNewMM = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewMM = -1
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function
    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDoc = True
        TipoDoc = SWTD(TD.CaricoMagazzino)
        'If TipoDoc = SWTD(TD.CaricoMagazzino) Then
        'strDesTipoDocumento = "CARICO DI MAGAZZINO"
        'End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
        'GIU090312 NEL DUBBIO ESEMIO CM SM E MM VERIFICO IL CODICE CHE è STATO INSERITO SE 9=For O 1=Cli
        'mi serve per sapere quale prezzo prendere se di acquisto o di LISTINO VENDITA
        TabCliFor = "Cli"
        myTabCliFor = TabCliFor
    End Function
    Private Function CreateDocT(ByVal myTimeOUT As String, ByVal NDoc As Long, ByRef NRev As Integer, ByVal DataDoc As String) As Boolean
        CreateDocT = False
        'OK CREAZIONE NUOVA SCHEDA INVENTARIO
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        Dim SqlDbNewCmd As New SqlCommand
        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewCMInizioAnno]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Esercizio", System.Data.SqlDbType.NVarChar, 4))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCausale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente", System.Data.SqlDbType.NVarChar, 16))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDoc", System.Data.SqlDbType.NVarChar, 10))
        'GIU040920 GESTIONE MAGAZZINI
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoFatt", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodMag", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'If IsNumeric(Session(IDDOCUMENTI)) Then
        '    SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = CLng(Session(IDDOCUMENTI))
        'Else
        '    SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = DBNull.Value
        'End If
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.CaricoMagazzino)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        NRev = NRev + 1
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@Esercizio").Value = Session(ESERCIZIO)
        SqlDbNewCmd.Parameters.Item("@CodCausale").Value = Session("CodCausale")
        SqlDbNewCmd.Parameters.Item("@CodCliente").Value = Session("CodCoGeDitta")
        SqlDbNewCmd.Parameters.Item("@DataDoc").Value = DataDoc '"01/01/" & Session(ESERCIZIO)
        SqlDbNewCmd.Parameters.Item("@TipoFatt").Value = App.GetParamGestAzi(Session(ESERCIZIO)).CodTipoFatt
        SqlDbNewCmd.Parameters.Item("@CodMag").Value = ddlMagazzino.SelectedValue.Trim
        Try
            SqlConn.Open()
            SqlDbNewCmd.CommandTimeout = myTimeOUT '5000
            SqlDbNewCmd.ExecuteNonQuery()
            SqlConn.Close()
            Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
            If String.IsNullOrEmpty(Session("PrimoMyID")) Then
                Session("PrimoMyID") = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
            End If
        Catch ExSQL As SqlException
            lblErrore.Text = "Errore: " & ExSQL.Message
            lblErrore.Visible = True
            btnOK.Enabled = False
            Exit Function
        Catch Ex As Exception
            lblErrore.Text = "Errore: " & Ex.Message
            lblErrore.Visible = True
            btnOK.Enabled = False
            Exit Function
        End Try
        CreateDocT = True
    End Function

    Public Sub CallMenu()
        Session(SWOP) = SWOPNESSUNA
        Try
            Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale")
        Catch ex As Exception
            Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale")
        End Try
    End Sub

    
End Class