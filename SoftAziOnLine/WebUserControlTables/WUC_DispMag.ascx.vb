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
'Imports Microsoft.Reporting.WebForms
Imports SoftAziOnLine.Magazzino

Imports System.IO 'giu140615

Partial Public Class WUC_DispMag
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceCategoria.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDataSourceLinea.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'giu080312
        If (Not IsPostBack) Then
            lblMess.Visible = False : LnkStampa.Visible = False
            rbtnCodice.Checked = True
            ddlMagazzino.SelectedValue = 1
            chkArtInclSottoScorta.Checked = True
            chkArtFuoriListino.Checked = False 'giu251114 giu151214
            chkRagrForn.Checked = True
            BtnPropRiord.Text = "Proposta di  riordino"
        End If
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        
        lblMess.Visible = False
        LnkStampa.Visible = False 'giu110320
        Dim DsMagazzino1 As New DsMagazzino
        Dim ObjReport As New Object
        Dim ClsPrint As New Magazzino
        Dim Errore As Boolean = False
        Dim Filtri As String = ""
        Dim SoloArtConMov As Boolean = False
        Dim Selezione As String = ""
        Dim StrErrore As String = ""
        Dim RagrForn As Boolean = False
        Dim SWNegativi As Boolean 'alb04052012
        If ddlMagazzino.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlCatgoria.BackColor = SEGNALA_KO Then
            Errore = True
        End If
        If ddlLinea.BackColor = SEGNALA_KO Then
            Errore = True
        End If

        If ddlMagazzino.Text = "" Then
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
                ModalPopup.Show("Controllo dati selezione", "Attenzione, Codice Fornitore richiesto..", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
        End If
        If Errore Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati stampa", "Attenzione, i campi segnalati in rosso non sono validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Dim strErroreGiac As String = ""
        If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then 'giu190613 aggiunto SWNegativi
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf strErroreGiac.Trim <> "" Then
            'If Session("SWConferma") = SWNO Then 'giu300423
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Ricalcolo giacenze", strErroreGiac.Trim, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        'FILTRO PER MAGAZZINO
        Selezione = "WHERE Codice_Magazzino = " & ddlMagazzino.SelectedValue
        Filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper

        'FILTRO PER CATEGORIA
        If txtCodCategoria.Text <> "" Then
            Selezione = Selezione & " AND CodCategoria = '" & CStr(Replace(txtCodCategoria.Text, "'", "''")) & "'"
            Filtri = Filtri & " | Categoria - " & ddlCatgoria.SelectedItem.Text.ToUpper
        End If

        'FILTRO PER LINEA
        If txtCodLinea.Text <> "" Then
            Selezione = Selezione & " AND CodLinea = '" & CStr(Replace(txtCodLinea.Text, "'", "''")) & "'"
            Filtri = Filtri & " | Linea - " & ddlLinea.SelectedItem.Text.ToUpper
        End If

        'FILTRO DA ARTICOLO 
        If txtCod1.Text <> "" Then
            Selezione = Selezione & " AND Cod_Articolo >= '" & Replace(txtCod1.Text, "'", "''") & "'"
            Filtri = Filtri & " | Dal codice articolo - " & txtCod1.Text.ToUpper
        End If

        'FILTRO A ARTICOLO
        If txtCod2.Text <> "" Then
            Selezione = Selezione & " AND Cod_Articolo <= '" & Replace(txtCod2.Text, "'", "''") & "'"
            Filtri = Filtri & " | Al codice articolo - " & txtCod2.Text.ToUpper
        End If
        'FILTRO fornitore
        If txtCodFornitore.Text <> "" Then
            Selezione = Selezione & " AND Cod_Fornitore = '" & Replace(txtCodFornitore.Text, "'", "''") & "'"
            Filtri = Filtri & " | Cod_Fornitore - " & txtCodFornitore.Text.ToUpper
        End If

        'FILTRO PER GIACENZA > 0
        If chkArtGiacDivZero.Checked Then
            'Pier 01/02/12 aggiunto anche Giac_Impegnata
            Selezione = Selezione & " AND ((Giacenza <> 0) Or (Giac_Impegnata <>0))"
            Filtri = Filtri & " | Giacenza diversa da 0"
        End If

        'PER STAMPARE SOLO ARTICOLI MOVIMENTATI
        SoloArtConMov = chkArtMovimentati.Checked
        If SoloArtConMov Then
            Selezione = Selezione & " AND ( (Giac_Impegnata <>0) or (Ordinati <>0) Or (Ord_Clienti <>0) )"
            Filtri = Filtri & " | Articoli movimentati"
        End If

        'alb090113 ARTICOLI FUORI LISTINO
        If chkArtFuoriListino.Checked = False Then 'se si vogliono solo quelli inclusi
            Selezione = Selezione & " AND ( (NOT IDListVenD IS NULL) OR ( (Giacenza <> 0) Or (Giac_Impegnata <> 0) ) )"
            Filtri = Filtri & " | Solo articoli inclusi in listino"
        Else
            Filtri = Filtri & " | Anche articoli esclusi da listino"
        End If
        '------------------------

        'ORDINAMENTO PER CODICE
        If rbtnCodice.Checked Then
            If chkRagrForn.Checked Then 'RAGGRUPPATI PER FORNITORE
                Selezione = Selezione & " ORDER BY Rag_Soc,Cod_Articolo"
            Else
                Selezione = Selezione & " ORDER BY Cod_Articolo"
            End If

        Else  'ORDINAMENTO PER DESCRIZIONE
            If chkRagrForn.Checked Then 'RAGGRUPPATI PER FORNITORE
                Selezione = Selezione & " ORDER BY Rag_Soc,Descrizione"
            Else
                Selezione = Selezione & " ORDER BY Descrizione"
            End If
        End If
        '
        Session("SAVETIPOSTAMPA") = Session(CSTTIPORPTDISPMAG)
        Session(CSTNOMEPDF) = "" 'giu300315
        Try
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzino
            If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Disponibilità articoli di Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", ""), _
                Filtri, Selezione, DsMagazzino1, StrErrore, chkArtInclSottoScorta.Checked, chkArtDaOrdinare.Checked, SWNegativi, chkSoloNegativi.Checked, False, ddlMagazzino.SelectedValue) Then
                If DsMagazzino1.DispMagazzino.Count > 0 Then
                    ' ''Session(CSTDsPrinWebDoc) = DsMagazzino1
                    If chkRagrForn.Checked Then
                        Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzinoFornitori
                    Else
                        Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzino
                    End If
                    If SWNegativi = True And Not chkSoloNegativi.Checked Then
                        lblMess.Text = "Attenzione, sono presenti movimenti in negativo."
                        lblMess.Visible = True
                    End If
                    ' ''OKStampa()
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                    Exit Sub
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        Call OKLink(DsMagazzino1)
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
        '''Session(CSTESPORTAPDF) = True
        '''Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & "StatMag\"
        '''Dim stPathReport As String = Session(CSTPATHPDF)
        '''Try 'giu281112 errore che il file Ã¨ gia aperto
        '''    Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
        '''    'giu140124
        '''    Rpt.Close()
        '''    Rpt.Dispose()
        '''    Rpt = Nothing
        '''    '-
        '''    GC.WaitForPendingFinalizers()
        '''    GC.Collect()
        '''    '-------------
        '''Catch ex As Exception
        '''    Rpt = Nothing
        '''    Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
        '''    Exit Sub
        '''End Try
        LnkStampa.Visible = True
        '''Dim LnkName As String = "~/Documenti/StatMag/" & Session(CSTNOMEPDF)
        '''LnkStampa.HRef = LnkName
        Session(CSTTIPORPTDISPMAG) = Session("SAVETIPOSTAMPA")
        getOutputRPT(Rpt, "PDF")
    End Sub
    '''Public Sub OKStampa()
    '''    Session(CSTNOBACK) = 0 'giu040512
    '''    Response.Redirect("..\WebFormTables\WF_PrintWebDispMag.aspx")
    '''End Sub
    '@@@@@
    Private Function getOutputRPT(ByVal _Rpt As Object, ByVal _Formato As String) As Boolean
        '_Rpt.Refresh()
        Dim myStream As Stream
        Try
            If _Formato = "PDF" Then
                myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            Else
                myStream = _Rpt.ExportToStream(ExportFormatType.ExcelRecord)
            End If
            Dim byteReport() As Byte = GetStreamAsByteArray(myStream)
            Session("WebFormStampe") = byteReport
        Catch ex As Exception
            Return False
        End Try

        Try
            _Rpt.Close()
            _Rpt.Dispose()
            _Rpt = Nothing
            GC.WaitForPendingFinalizers()
            GC.Collect()
        Catch
        End Try
        getOutputRPT = True
    End Function
    Private Shared Function GetStreamAsByteArray(ByVal stream As System.IO.Stream) As Byte()

        Dim streamLength As Integer = Convert.ToInt32(stream.Length)

        Dim fileData As Byte() = New Byte(streamLength) {}

        ' Read the file into a byte array
        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function
    '@@@@@
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

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub txtCodCategoria_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCategoria.TextChanged
        lblMess.Visible = False : LnkStampa.Visible = False
        PosizionaItemDDLTxt(txtCodCategoria, ddlCatgoria)
        txtCodLinea.Focus()
    End Sub

    Private Sub txtCodLinea_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodLinea.TextChanged
        lblMess.Visible = False : LnkStampa.Visible = False
        PosizionaItemDDLTxt(txtCodLinea, ddlLinea)
        txtCod1.Focus()
    End Sub

    Private Sub ddlLinea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlLinea.SelectedIndexChanged
        lblMess.Visible = False : LnkStampa.Visible = False
        txtCodLinea.Text = ddlLinea.SelectedValue
        txtCodLinea.BackColor = SEGNALA_OK
        ddlLinea.BackColor = SEGNALA_OK
    End Sub

    Private Sub ddlCatgoria_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCatgoria.SelectedIndexChanged
        lblMess.Visible = False : LnkStampa.Visible = False
        txtCodCategoria.Text = ddlCatgoria.SelectedValue
        txtCodCategoria.BackColor = SEGNALA_OK
        ddlCatgoria.BackColor = SEGNALA_OK
    End Sub
    Private Sub BtnPropRiord_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnPropRiord.Click
        'giu300423
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Sub
        End If
        Dim strEserDiverso As String = ""
        If Now.Year.ToString.Trim <> strEser.Trim Then
            strEserDiverso = "<b><br>Attenzione,       ANNO corrente: " + Now.Year.ToString.Trim + _
                             "<br>DIVERSO dall'esercizio corrente: " + strEser.Trim + "</b>"
        End If
        '-
        Dim StrErrore As String = ""
        Dim ClsMag As New Magazzino

        If ClsMag.GetPresenzaPropostaRiordino(StrErrore) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "CancPF"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea proposta di riordino", "In archivio è già presente una proposta di riordino, confermi la creazione della proposta di un nuovo riordino ?<BR>NOTA la proposta di riordino precedente sarà cancellata." + strEserDiverso, WUC_ModalPopup.TYPE_CONFIRM)
        Else
            If StrErrore <> "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewPF"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Crea proposta di riordino", "Confermi la creazione della proposta di riordino ?" + strEserDiverso, WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    Public Sub CancPF()
        'giu311018
        Dim strSQL As String = ""
        strSQL = "SELECT IDDocumenti FROM DocumentiT"
        strSQL = strSQL & " WHERE (Tipo_Doc = '" & SWTD(TD.PropOrdFornitori) & "')"
        Dim SWOK As Boolean = True
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each RowDett In ds.Tables(0).Rows
                        strSQL = "DELETE FROM DocumentiD WHERE (IDDocumenti = " & RowDett.Item("IDDocumenti").ToString.Trim & ")"
                        ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL)
                    Next
                    'OK CACELLO LE TESTATE
                    strSQL = "DELETE FROM DocumentiT WHERE (Tipo_Doc = '" & SWTD(TD.PropOrdFornitori) & "')"
                    ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL)
                End If
            End If
        Catch Ex As Exception
            SWOK = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "(Cancella PropostaRiordino) Si è verificato il seguente errore:" & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Finally
            ObjDB = Nothing
        End Try
        If SWOK Then
            Call CreaNewPF()
        End If
    End Sub
    Public Sub CreaNewPF()
        Dim DsArtDaOrd1 As New DsMagazzino

        Dim ClsPrint As New Magazzino
        Dim Errore As Boolean = False
        Dim Filtri As String = ""
        Dim Selezione As String = ""
        Dim StrErroreDisp As String = ""
        'ELABORO LA DISPONIBILITA'

        If ClsPrint.StampaDispMagazzino(Session(CSTAZIENDARPT), "Disponibilità articoli di Magazzino: " + ddlMagazzino.SelectedItem.Text.Trim.ToUpper.Replace("MAGAZZINO", ""), _
                                        Filtri, Selezione, DsArtDaOrd1, StrErroreDisp, chkArtInclSottoScorta.Checked, True, False, False, False, ddlMagazzino.SelectedValue) Then
            If DsArtDaOrd1.DispMagazzino.Rows.Count > 0 Then

                Dim StrErrore As String = ""
                Dim SqlDbInserCmdForInsert As New SqlCommand 'Pier311011
                Dim SqlAdapDocForInsert As New SqlDataAdapter   'Pier311011
                Dim DsDocDettForInsert As New DSDocumenti
                Dim RowDettForIns As DSDocumenti.DocumentiDForInsertRow
                Dim RowDett As DsMagazzino.DispMagazzinoRow
                Dim RowFornit As DsMagazzino.DispMagazzinoFornitRow

                Dim NDoc As Long = 0 : Dim NRev As Integer = 0

                For Each RowDett In DsArtDaOrd1.DispMagazzino
                    If RowDett.RowState <> DataRowState.Deleted Then
                        If DsArtDaOrd1.DispMagazzinoFornit.Select("Cod_Fornitore='" & RowDett.Cod_Fornitore & "'").Length = 0 Then
                            RowFornit = DsArtDaOrd1.DispMagazzinoFornit.NewRow
                            RowFornit.Item("Cod_Fornitore") = RowDett.Cod_Fornitore
                            RowFornit.Item("Rag_Soc") = RowDett.Rag_Soc
                            DsArtDaOrd1.DispMagazzinoFornit.AddDispMagazzinoFornitRow(RowFornit)
                        End If
                    End If
                Next

                'OK CREAZIONE NUOVA PROPOSTA DI RIORDINE
                Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
                Dim SqlConn As New SqlConnection
                SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
                Dim SqlDbNewCmd As New SqlCommand
                'GIU040920 MANDARE IL MAGAZZINO
                SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocPF]"
                SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbNewCmd.Connection = SqlConn
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Fornitore", System.Data.SqlDbType.NVarChar, 16))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
                'GIU040920 GESTIONE MAGAZZINI
                SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodMag", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                '
                '
                'SqlDbInserCmdForInsert
                '
                SqlDbInserCmdForInsert.CommandText = "insert_DocDByIDDocumenti_ForInsertPF"
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
                For Each RowFornit In DsArtDaOrd1.DispMagazzinoFornit
                    If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
                        NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroRiordinoFornitore + 1
                    Else
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If
                    '-
                    If AggiornaNumDoc(SWTD(TD.PropOrdFornitori), NDoc, NRev) = False Then
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End If

                    SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.PropOrdFornitori)
                    If RowFornit.Cod_Fornitore.ToString.Trim = "" Then
                        SqlDbNewCmd.Parameters.Item("@Cod_Fornitore").Value = DBNull.Value
                    Else
                        SqlDbNewCmd.Parameters.Item("@Cod_Fornitore").Value = RowFornit.Cod_Fornitore
                    End If
                    SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
                    SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                    SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
                    SqlDbNewCmd.Parameters.Item("@CodMag").Value = ddlMagazzino.SelectedValue
                    'giu190617
                    Dim strValore As String = ""
                    'Dim strErrore As String = ""
                    Dim myTimeOUT As Long = 5000
                    If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
                        If IsNumeric(strValore.Trim) Then
                            If CLng(strValore.Trim) > myTimeOUT Then
                                myTimeOUT = CLng(strValore.Trim)
                            End If
                        End If
                    End If
                    'esempio.CommandTimeout = myTimeOUT
                    '---------------------------
                    Try
                        SqlConn.Open()
                        SqlDbNewCmd.CommandTimeout = myTimeOUT
                        SqlDbNewCmd.ExecuteNonQuery()
                        SqlConn.Close()
                        Session(IDDOCUMENTI) = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
                    Catch ExSQL As SqlException
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    Catch Ex As Exception
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End Try

                    Try
                        Dim myID = Session(IDDOCUMENTI)
                        If IsNothing(myID) Then
                            myID = ""
                        End If
                        If String.IsNullOrEmpty(myID) Then
                            myID = ""
                        End If
                        If Not IsNumeric(myID) Then
                            Session(MODALPOPUP_CALLBACK_METHOD) = ""
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
                            Exit Sub
                        End If
                        Dim NRiga As Integer
                        '===PREPARO GLI ARTICOLO DA INSERIRE ============================
                        NRiga = 1
                        For Each RowDett In DsArtDaOrd1.DispMagazzino.Select("Cod_Fornitore='" & RowFornit.Cod_Fornitore & "'")
                            If RowDett.RowState <> DataRowState.Deleted Then
                                RowDettForIns = DsDocDettForInsert.DocumentiDForInsert.NewRow
                                RowDettForIns.Item("IDDOCUMENTI") = myID
                                RowDettForIns.Item("Riga") = NRiga
                                RowDettForIns.Item("COD_ARTICOLO") = RowDett.Cod_Articolo
                                RowDettForIns.Item("Descrizione") = RowDett.Descrizione
                                RowDettForIns.Item("Cod_Iva") = 0
                                RowDettForIns.Item("Prezzo") = 0
                                RowDettForIns.Item("Prezzo_Netto") = 0
                                RowDettForIns.Item("SWPNettoModificato") = 0
                                RowDettForIns.Item("Prezzo_Netto_Inputato") = 0
                                RowDettForIns.Item("Sconto_1") = 0
                                RowDettForIns.Item("Sconto_2") = 0
                                RowDettForIns.Item("Sconto_3") = 0
                                RowDettForIns.Item("Sconto_4") = 0
                                RowDettForIns.Item("Sconto_Pag") = 0
                                RowDettForIns.Item("ScontoValore") = 0
                                RowDettForIns.Item("Sconto_Merce") = 0
                                RowDettForIns.Item("ScontoReale") = 0
                                RowDettForIns.Item("Um") = "PZ" 'Aggiorno nella Store quando setto i prezzi
                                RowDettForIns.Item("Qta_Ordinata") = RowDett.DaOrdinare
                                RowDettForIns.Item("Qta_Evasa") = 0
                                RowDettForIns.Item("Qta_Residua") = RowDett.DaOrdinare
                                RowDettForIns.Item("Importo") = 0
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
                            End If
                            NRiga = NRiga + 1
                        Next
                        SqlAdapDocForInsert.Update(DsDocDettForInsert.DocumentiDForInsert)
                        '===============================================================
                    Catch ex As Exception
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
                        Exit Sub
                    End Try
                Next
                'OK FATTO
                Session(CSTTIPODOC) = SWTD(TD.PropOrdFornitori)
                Response.Redirect("WF_ElencoPropostaRiordino.aspx?labelForm=Gestione proposte di riordino")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Proposta riordino", "In archivio non sono presenti articoli da ordinare.", WUC_ModalPopup.TYPE_INFO)
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", StrErroreDisp, WUC_ModalPopup.TYPE_ERROR)
        End If
    End Sub
    Private Function AggiornaNumDoc(ByVal TDoc As String, ByVal NDoc As Long, ByVal NRev As Integer) As Boolean
        ''Pier 020212 come da accordi con Giuseppe, copiato paro paro da WUC_ElencoOrdini
        AggiornaNumDoc = True
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        Dim SqlDbUpdCmd As New SqlCommand

        SqlDbUpdCmd.CommandText = "[Update_NDocPargen]"
        SqlDbUpdCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbUpdCmd.Connection = SqlConn
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWOp", System.Data.SqlDbType.VarChar, 1))
        SqlDbUpdCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '--
        SqlDbUpdCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbUpdCmd.Parameters.Item("@SWOp").Value = "N"

        SqlDbUpdCmd.Parameters.Item("@Tipo_Doc").Value = TDoc
        SqlDbUpdCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbUpdCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        'giu190617
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        Try
            SqlConn.Open()
            SqlDbUpdCmd.CommandTimeout = myTimeOUT
            SqlDbUpdCmd.ExecuteNonQuery()
            SqlConn.Close()
            If SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -1 Then 'IMPEGNATO GIA ESISTE SOMMO 1 E RIPROVO
                AggiornaNumDoc = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiorna numero documento", "NUOVO IDENTIFICATIVO DOCUMENTO GIA' PRESENTE.", WUC_ModalPopup.TYPE_ERROR)
            ElseIf SqlDbUpdCmd.Parameters.Item("@RetVal").Value = -2 Then 'ERRORE O PER SQL OPPURE TIPO DOC NON GESTITO
                AggiornaNumDoc = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiorna numero documento", "NUOVO TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ExSQL As SqlException
            AggiornaNumDoc = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
        Catch Ex As Exception
            AggiornaNumDoc = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Function

    'giu040420
    Private Sub chkTuttiFornitori_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiFornitori.CheckedChanged
        lblMess.Visible = False : LnkStampa.Visible = False
        pulisciCampiFornitore()
        If chkTuttiFornitori.Checked Then
            AbilitaDisabilitaCampiFornitore(False)
        Else
            AbilitaDisabilitaCampiFornitore(True)
            txtCodFornitore.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiFornitore(ByVal Abilita As Boolean)
        txtCodFornitore.Enabled = Abilita
        btnFornitore.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiFornitore()
        txtCodFornitore.AutoPostBack = False
        txtCodFornitore.Text = ""
        txtCodFornitore.AutoPostBack = True
        txtDescFornitore.Text = ""
    End Sub
    Private Sub txtCodFornitore_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodFornitore.TextChanged
        lblMess.Visible = False : LnkStampa.Visible = False
        txtDescFornitore.Text = App.GetValoreFromChiave(txtCodFornitore.Text, Def.FORNITORI, Session(ESERCIZIO))
    End Sub
    Private Sub ApriElencoFornitori1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        lblMess.Visible = False : LnkStampa.Visible = False
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodFornitore.AutoPostBack = False
            txtCodFornitore.Text = codice
            txtCodFornitore.AutoPostBack = True
            txtDescFornitore.Text = descrizione
        End If
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    End Sub
    Private Sub btnFornitore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFornitore.Click
        lblMess.Visible = False : LnkStampa.Visible = False
        Session(F_FOR_RICERCA) = True
        ApriElencoFornitori1()
    End Sub

    Private Sub ddlMagazzino_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlMagazzino.SelectedIndexChanged
        lblMess.Visible = False : LnkStampa.Visible = False
    End Sub
End Class