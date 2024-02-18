Imports System.Data.SqlClient
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Magazzino
Imports Microsoft.Reporting.WebForms
Imports System.Web.Services.WebService
Imports System.Web.Script.Serialization

Partial Public Class WUC_ArticoliInstallati
    Inherits System.Web.UI.UserControl

    Private DsArtInst As New DSDocumenti 'giu040612
    Private dvDocT As DataView

    Private SqlConnDoc As SqlConnection
    Private SqlAdapDoc As SqlDataAdapter

    Private SqlDbSelectCmd As SqlCommand
    Private SqlDbInserCmd As SqlCommand
    Private SqlDbUpdateCmd As SqlCommand
    Private SqlDbDeleteCmd As SqlCommand

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ArticoliInstallati.Load (1))")
            Exit Sub
        End If
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

        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        '----------------------------
        If Not (sTipoUtente.Equals(CSTTECNICO)) Then
            btnNuovo.Visible = False
            btnElimina.Visible = False
        End If
        '----------------------------
        ' ''If (sTipoUtente.Equals(CSTVENDITE)) Then
        ' ''    btnNuovo.Visible = False
        ' ''    btnElimina.Visible = False
        ' ''End If
        ' ''If (sTipoUtente.Equals(CSTACQUISTI)) Then
        ' ''    btnNuovo.Visible = False
        ' ''    btnElimina.Visible = False
        ' ''End If
        '----------------------------
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))

        SqlDSCliForFilProvv.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSDestinazione.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSContratti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        Session(CSTTABCLIFOR) = TabCliFor
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            TabCliFor = "For"
        End If
        If TabCliFor = "Cli" Then
            LblCliente.Text = "Cliente" : Session(CSTTABCLIFOR) = "Cli"
        ElseIf TabCliFor = "For" Then
            LblCliente.Text = "Fornitore" : Session(CSTTABCLIFOR) = "For"
        Else 'DEFAULT giu191211
            LblCliente.Text = "Cli./For." : Session(CSTTABCLIFOR) = "CliFor"
        End If
        SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)"
        'giu280512 adesso è tutto uguale
        ' ''If String.Equals(TipoDoc, SWTD(TD.ArticoloInstallato)) Then
        ' ''    lblDatiContratto.Visible = False : lblNCA.Visible = False : txtNumero.Visible = False
        ' ''    lblValidoDal.Visible = False : lblValidoAl.Visible = False
        ' ''    txtDataInizio.Visible = False : imgBtnShowCalendarDIC.Visible = False
        ' ''    txtDataFine.Visible = False : imgBtnShowCalendarDFC.Visible = False
        ' ''    lblImporto.Visible = False : TxtImporto.Visible = False
        ' ''    lblTipoCA.Visible = False : DDLTipoContratto.Visible = False
        ' ''ElseIf String.Equals(TipoDoc, SWTD(TD.ContrattoAssistenza)) Then
        ' ''    BtnSelArticoloIns.Visible = False : TxtArticoloCod.Enabled = False : TxtArticoloDesc.Enabled = False
        ' ''    btnCercaAnagrafica.Visible = False : txtCodCliForFilProvv.Enabled = False
        ' ''End If
        If (Not IsPostBack) Then
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
            '-
            SetCdmDAdp()

            If Session(SWOP) = SWOPNUOVO Then
                DsArtInst.Clear()
                AzzeraTxtDocT()

                txtDataInstallazione.Text = Format(Now, FormatoData)
                rbtnAttivo.Checked = True
                ' ''Session(CSTSTATODOC) = "0"
                Dim strErrore As String = ""
                If AggNuovaTestata(strErrore) = False Then
                    Chiudi("Errore: Inserimento nuovo Articolo/Contratto. " & strErrore)
                    Exit Sub
                End If
            End If
            Dim myID As String = ""
            myID = Session(IDARTICOLOINST)
            '-------------------
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If myID = "" Or Not IsNumeric(myID) Then
                Chiudi("Errore: IDENTIFICATIVO ARTICOLO INSTALLATO/CONTRATTO SCONOSCIUTO")
                Exit Sub
            End If
            SqlDbSelectCmd.Parameters.Item("@IDArticoloInst").Value = CLng(myID)
            SqlAdapDoc.Fill(DsArtInst.ArticoliInstallati)

            dvDocT = New DataView(DsArtInst.ArticoliInstallati)
            Session("dvDocT") = dvDocT
            Session("SqlAdapDoc") = SqlAdapDoc
            Session("DsDocT") = DsArtInst
            Session("SqlDbSelectCmd") = SqlDbSelectCmd
            Session("SqlDbInserCmd") = SqlDbInserCmd
            Session("SqlDbDeleteCmd") = SqlDbDeleteCmd
            Session("SqlDbUpdateCmd") = SqlDbUpdateCmd
            '
            Try
                If Session(SWOP) <> SWOPNESSUNA Then
                    If dvDocT.Count > 0 Then
                        BtnSetEnabledTo(False)
                        If Session(SWOP) = SWOPELIMINA Then
                            CampiSetEnabledToT(False)
                            btnAnnulla.Enabled = True
                            btnElimina.Enabled = True
                        Else
                            CampiSetEnabledToT(True)
                            btnAnnulla.Enabled = True
                            btnAggiorna.Enabled = True
                        End If
                        PopolaTxtDocT()
                    Else
                        CampiSetEnabledToT(False)
                        BtnSetEnabledTo(False)
                        AzzeraTxtDocT()
                        Session(SWOP) = SWOPNESSUNA
                        btnNuovo.Enabled = True
                        Session(IDARTICOLOINST) = ""
                        'giu280512 unificato Session(IDCONTRATTOASSISTENZA) = ""
                    End If
                ElseIf Session(SWOP) = SWOPNESSUNA Then
                    CampiSetEnabledToT(False)
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    If dvDocT.Count > 0 Then
                        PopolaTxtDocT()
                        btnElimina.Enabled = True
                        btnModifica.Enabled = True
                        btnStampa.Enabled = True
                    Else
                        AzzeraTxtDocT()
                    End If
                End If
            Catch Ex As Exception
                Chiudi("Errore: Caricamento (ArticoliInstallati.Load (2)): " & Ex.Message)
                Exit Sub
            End Try

        End If

        ModalPopup.WucElement = Me
        WFP_Anagrafiche_Modify1.WucElement = Me
        WFPElencoCli.WucElement = Me
        WFPElencoFor.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If
        If Session(F_ANAGRCLIFOR_APERTA) = True Then
            WFP_Anagrafiche_Modify1.Show()
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(F_CLI_RICERCA) Then
                WFPElencoCli.Show()
            End If
            If Session(F_FOR_RICERCA) Then
                WFPElencoFor.Show()
            End If
        End If

    End Sub

    Private Function AggNuovaTestata(ByRef strErrore As String) As Boolean
        txtNumero.Text = "0" 'GetNewNumeroDocCA()
        If txtNumero.Text.Trim = "0" Then txtNumero.Text = ""
        AggNuovaTestata = AggiornaDocT(False)
        Session(SWOP) = SWOPMODIFICA
    End Function
    Private Function GetNewNumeroDocCA() As Long
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (GetNewNumeroDoc)")
            Exit Function
        End If
        If TipoDoc = SWTD(TD.ArticoloInstallato) Then
            GetNewNumeroDocCA = 0
        End If
        'If Not IsNumeric(txtRevNDoc.Text.Trim) Then txtRevNDoc.Text = "0"
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS Numero From ArticoliInst_ContrattiAss "
        'giu280512 unificato strSQL += "WHERE Tipo_Doc = '" & TipoDoc.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewNumeroDocCA = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewNumeroDocCA = 1
                    End If
                    Exit Function
                Else
                    GetNewNumeroDocCA = 1
                    Exit Function
                End If
            Else
                GetNewNumeroDocCA = 0
                Exit Function
            End If
        Catch Ex As Exception
            GetNewNumeroDocCA = 0
            Exit Function
        End Try

    End Function

#Region " Imposta Command e DataAdapter"
    'giu050514 Ok nuovi campi scadenze e invio email
    Private Function SetCdmDAdp() As Boolean
        SqlConnDoc = New SqlConnection
        SqlAdapDoc = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
        SqlDbInserCmd = New SqlCommand
        SqlDbUpdateCmd = New SqlCommand
        SqlDbDeleteCmd = New SqlCommand

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandText = "get_ArticoliInst_ContrattiAss"
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = Me.SqlConnDoc
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDArticoloInst", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'SqlInsertCommand1
        '
        Me.SqlDbInserCmd.CommandText = "[insert_ArticoliInst_ContrattiAss]"
        Me.SqlDbInserCmd.CommandType = System.Data.CommandType.StoredProcedure
        Me.SqlDbInserCmd.Connection = Me.SqlConnDoc
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "ID", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2, "Tipo_Doc"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Installazione", System.Data.SqlDbType.DateTime, 8, "Data_Installazione"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Coge", System.Data.SqlDbType.NVarChar, 16, "Cod_Coge"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riferimento", System.Data.SqlDbType.NVarChar, 150, "Riferimento"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteDocumento", System.Data.SqlDbType.NVarChar, 1073741823, "NoteDocumento"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NsRiferimento", System.Data.SqlDbType.NVarChar, 150, "NsRiferimento"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 1073741823, "Note"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Filiale", System.Data.SqlDbType.Int, 4, "Cod_Filiale"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione1", System.Data.SqlDbType.NVarChar, 150, "Destinazione1"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione2", System.Data.SqlDbType.NVarChar, 150, "Destinazione2"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione3", System.Data.SqlDbType.NVarChar, 150, "Destinazione3"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@LBase", System.Data.SqlDbType.Int, 4, "LBase"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@LOpz", System.Data.SqlDbType.Int, 4, "LOpz"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Descrizione", System.Data.SqlDbType.NVarChar, 150, "Descrizione"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataScadGaranzia", System.Data.SqlDbType.DateTime, 8, "DataScadGaranzia"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataScadElettrodi", System.Data.SqlDbType.DateTime, 8, "DataScadElettrodi"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataScadBatterie", System.Data.SqlDbType.DateTime, 8, "DataScadBatterie"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data1InvioScadGa", System.Data.SqlDbType.DateTime, 8, "Data1InvioScadGa"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data1InvioScadEl", System.Data.SqlDbType.DateTime, 8, "Data1InvioScadEl"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data1InvioScadBa", System.Data.SqlDbType.DateTime, 8, "Data1InvioScadBa"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 30, "NSerie"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 30, "Lotto"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Attivo", System.Data.SqlDbType.Bit, 1, "Attivo"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InRiparazione", System.Data.SqlDbType.Bit, 1, "InRiparazione"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Sostituito", System.Data.SqlDbType.Bit, 1, "Sostituito"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataSostituzione", System.Data.SqlDbType.DateTime, 8, "DataSostituzione"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDTipoContratto", System.Data.SqlDbType.Int, 4, "IDTipoContratto"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10, "Numero"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Importo", System.Data.SqlDbType.Money, 8, "Importo"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Inizio", System.Data.SqlDbType.DateTime, 8, "Data_Inizio"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Fine", System.Data.SqlDbType.DateTime, 8, "Data_Fine"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 4, "RefInt"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DesRefInt", System.Data.SqlDbType.NVarChar, 1073741823, "DesRefInt"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Reparto", System.Data.SqlDbType.Int, 4, "Reparto"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocDTMM", System.Data.SqlDbType.Int, 4, "IDDocDTMM"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_DocDTMM", System.Data.SqlDbType.NVarChar, 2, "Tipo_DocDTMM"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_DocDTMM", System.Data.SqlDbType.DateTime, 8, "Data_DocDTMM"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RigaDTMM", System.Data.SqlDbType.Int, 4, "RigaDTMM"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NColloDTMM", System.Data.SqlDbType.Int, 4, "NColloDTMM"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaColliDTMM", System.Data.SqlDbType.Int, 4, "QtaColliDTMM"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SfusiDTMM", System.Data.SqlDbType.Money, 8, "SfusiDTMM"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Operatore", System.Data.SqlDbType.NVarChar, 50, "Operatore"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NomePC", System.Data.SqlDbType.NVarChar, 50, "NomePC"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BloccatoDalPC", System.Data.SqlDbType.NVarChar, 50, "BloccatoDalPC"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50, "InseritoDa"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50, "ModificatoDa"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data2InvioScadGa", System.Data.SqlDbType.DateTime, 8, "Data2InvioScadGa"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data2InvioScadEl", System.Data.SqlDbType.DateTime, 8, "Data2InvioScadEl"))
        Me.SqlDbInserCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data2InvioScadBa", System.Data.SqlDbType.DateTime, 8, "Data2InvioScadBa"))

        '
        'SqlUpdateCommand1
        '
        Me.SqlDbUpdateCmd.CommandText = "[update_ArticoliInst_ContrattiAss]"
        Me.SqlDbUpdateCmd.CommandType = System.Data.CommandType.StoredProcedure
        Me.SqlDbUpdateCmd.Connection = Me.SqlConnDoc
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "ID", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2, "Tipo_Doc"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Installazione", System.Data.SqlDbType.DateTime, 8, "Data_Installazione"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Coge", System.Data.SqlDbType.NVarChar, 16, "Cod_Coge"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riferimento", System.Data.SqlDbType.NVarChar, 150, "Riferimento"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteDocumento", System.Data.SqlDbType.NVarChar, 1073741823, "NoteDocumento"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NsRiferimento", System.Data.SqlDbType.NVarChar, 150, "NsRiferimento"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 1073741823, "Note"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Filiale", System.Data.SqlDbType.Int, 4, "Cod_Filiale"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione1", System.Data.SqlDbType.NVarChar, 150, "Destinazione1"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione2", System.Data.SqlDbType.NVarChar, 150, "Destinazione2"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Destinazione3", System.Data.SqlDbType.NVarChar, 150, "Destinazione3"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@LBase", System.Data.SqlDbType.Int, 4, "LBase"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@LOpz", System.Data.SqlDbType.Int, 4, "LOpz"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Descrizione", System.Data.SqlDbType.NVarChar, 150, "Descrizione"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataScadGaranzia", System.Data.SqlDbType.DateTime, 8, "DataScadGaranzia"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataScadElettrodi", System.Data.SqlDbType.DateTime, 8, "DataScadElettrodi"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataScadBatterie", System.Data.SqlDbType.DateTime, 8, "DataScadBatterie"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data1InvioScadGa", System.Data.SqlDbType.DateTime, 8, "Data1InvioScadGa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data1InvioScadEl", System.Data.SqlDbType.DateTime, 8, "Data1InvioScadEl"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data1InvioScadBa", System.Data.SqlDbType.DateTime, 8, "Data1InvioScadBa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 30, "NSerie"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Lotto", System.Data.SqlDbType.NVarChar, 30, "Lotto"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Attivo", System.Data.SqlDbType.Bit, 1, "Attivo"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InRiparazione", System.Data.SqlDbType.Bit, 1, "InRiparazione"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Sostituito", System.Data.SqlDbType.Bit, 1, "Sostituito"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataSostituzione", System.Data.SqlDbType.DateTime, 8, "DataSostituzione"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDTipoContratto", System.Data.SqlDbType.Int, 4, "IDTipoContratto"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10, "Numero"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Importo", System.Data.SqlDbType.Money, 8, "Importo"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Inizio", System.Data.SqlDbType.DateTime, 8, "Data_Inizio"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Fine", System.Data.SqlDbType.DateTime, 8, "Data_Fine"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 4, "RefInt"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DesRefInt", System.Data.SqlDbType.NVarChar, 1073741823, "DesRefInt"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Reparto", System.Data.SqlDbType.Int, 4, "Reparto"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocDTMM", System.Data.SqlDbType.Int, 4, "IDDocDTMM"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_DocDTMM", System.Data.SqlDbType.NVarChar, 2, "Tipo_DocDTMM"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_DocDTMM", System.Data.SqlDbType.DateTime, 8, "Data_DocDTMM"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RigaDTMM", System.Data.SqlDbType.Int, 4, "RigaDTMM"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NColloDTMM", System.Data.SqlDbType.Int, 4, "NColloDTMM"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaColliDTMM", System.Data.SqlDbType.Int, 4, "QtaColliDTMM"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SfusiDTMM", System.Data.SqlDbType.Money, 8, "SfusiDTMM"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Operatore", System.Data.SqlDbType.NVarChar, 50, "Operatore"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NomePC", System.Data.SqlDbType.NVarChar, 50, "NomePC"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@BloccatoDalPC", System.Data.SqlDbType.NVarChar, 50, "BloccatoDalPC"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50, "InseritoDa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50, "ModificatoDa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data2InvioScadGa", System.Data.SqlDbType.DateTime, 8, "Data2InvioScadGa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data2InvioScadEl", System.Data.SqlDbType.DateTime, 8, "Data2InvioScadEl"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data2InvioScadBa", System.Data.SqlDbType.DateTime, 8, "Data2InvioScadBa"))
        '
        'SqlDeleteCommand1
        '
        SqlDbDeleteCmd.CommandText = "[delete_ArticoliInst_ContrattiAss]"
        SqlDbDeleteCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbDeleteCmd.Connection = Me.SqlConnDoc
        SqlDbDeleteCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbDeleteCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDArticoliInst", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "ID", System.Data.DataRowVersion.Original, Nothing))

        SqlAdapDoc.SelectCommand = SqlDbSelectCmd
        SqlAdapDoc.InsertCommand = SqlDbInserCmd
        SqlAdapDoc.DeleteCommand = SqlDbDeleteCmd
        SqlAdapDoc.UpdateCommand = SqlDbUpdateCmd

        Session("SqlAdapDoc") = SqlAdapDoc
        Session("SqlDbSelectCmd") = SqlDbSelectCmd
        Session("SqlDbInserCmd") = SqlDbInserCmd
        Session("SqlDbDeleteCmd") = SqlDbDeleteCmd
        Session("SqlDbUpdateCmd") = SqlDbUpdateCmd
    End Function
#End Region

    Public Function CallBackWFPAnagrCliFor() As Boolean

        Session(IDANAGRCLIFOR) = ""
        Dim rk As StrAnagrCliFor
        rk = Session(RKANAGRCLIFOR)
        If IsNothing(rk.Rag_Soc) Then
            Exit Function
        End If

        Dim strToolTip As String = ""
        lblRagSoc.Text = rk.Rag_Soc
        strToolTip += txtCodCliForFilProvv.Text.Trim + " " + rk.Rag_Soc + " " + rk.Denominazione + " " + rk.Riferimento + " "
        '---------
        lblPICF.Text = rk.Partita_IVA : lblLabelPICF.Text = "P.IVA"
        If lblPICF.Text.Trim = "" Then
            lblPICF.Text = rk.Codice_Fiscale : lblLabelPICF.Text = "C.Fis."
        End If
        strToolTip += lblLabelPICF.Text + ": " + lblPICF.Text + " "
        lblIndirizzo.Text = rk.Indirizzo + " " + rk.NumeroCivico
        strToolTip += lblIndirizzo.Text + " "
        lblLocalita.Text = rk.Cap & " " & rk.Localita & " " & IIf(rk.Provincia.ToString.Trim <> "", "(" & rk.Provincia.ToString & ")", "")
        strToolTip += lblLocalita.Text
        '-- OK
        txtCodCliForFilProvv.ToolTip = strToolTip
        lblRagSoc.ToolTip = strToolTip
        '-GIU090514
        lblTelefono1.Text = rk.Telefono1 : lblTelefono2.Text = rk.Telefono2 : lblFax.Text = rk.Fax
        TxtEmail.Text = rk.EMail
        txtEmailInvioScad.Text = rk.EMailInvioScad  'sim180518
    End Function
    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDoc = True
        TipoDoc = Session(CSTTIPODOC)
        If IsNothing(TipoDoc) Then
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        strDesTipoDocumento = ""
        'giu280512 unificato 
        '' ''Articolo installato
        ' ''If TipoDoc = SWTD(TD.ArticoloInstallato) Then
        ' ''    strDesTipoDocumento = "ARTICOLO INSTALLATO"
        ' ''ElseIf TipoDoc = SWTD(TD.ContrattoAssistenza) Then
        ' ''    strDesTipoDocumento = "CONTRATTO DI ASSISTENZA"
        ' ''End If
        strDesTipoDocumento = "ARTICOLO INSTALLATO/CONTRATTO DI ASSISTENZA"
        '-------------------
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
        If TabCliFor = "CliFor" Then
            If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
                TabCliFor = "Cli"
            ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
                TabCliFor = "For"
            End If
            myTabCliFor = TabCliFor
        End If
    End Function

    Private Sub BtnSetEnabledTo(ByVal SW As Boolean)
        btnAggiorna.Enabled = SW
        btnAnnulla.Enabled = SW
        btnElimina.Enabled = SW
        btnModifica.Enabled = SW
        btnNuovo.Enabled = SW
        btnStampa.Enabled = SW
    End Sub

#Region " Dati Intestazione"
    Private Sub PopolaTxtDocT()
        SfondoCampiDocT()
        If (dvDocT Is Nothing) Then
            DsArtInst = Session("DsDocT")
            dvDocT = New DataView(DsArtInst.ArticoliInstallati)
        End If
        If dvDocT.Count = 0 Then
            AzzeraTxtDocT()
            Exit Sub
        End If
        TxtEmail.AutoPostBack = False
        txtEmailDest.AutoPostBack = False
        txtEmailInvioScad.AutoPostBack = False
        ' ''Session(CSTSTATODOC) = dvDocT.Item(0).Item("StatoDoc").ToString
        If IsDBNull(dvDocT.Item(0).Item("Data_Installazione")) Then
            txtDataInstallazione.Text = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data_Installazione")) Then
            txtDataInstallazione.Text = Format(dvDocT.Item(0).Item("Data_Installazione"), FormatoData)
        Else
            txtDataInstallazione.Text = ""
        End If
        If IsDBNull(dvDocT.Item(0).Item("DataScadGaranzia")) Then
            txtDataScadGaranzia.Text = ""
        ElseIf IsDate(dvDocT.Item(0).Item("DataScadGaranzia")) Then
            txtDataScadGaranzia.Text = Format(dvDocT.Item(0).Item("DataScadGaranzia"), FormatoData)
        Else
            txtDataScadGaranzia.Text = ""
        End If
        'giu050514
        If IsDBNull(dvDocT.Item(0).Item("Data1InvioScadGa")) Then
            chkEmailGa.Checked = False : chkEmailGa.ToolTip = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data1InvioScadGa")) Then
            chkEmailGa.Checked = True
            chkEmailGa.ToolTip = Format(dvDocT.Item(0).Item("Data1InvioScadGa"), FormatoData)
        Else
            chkEmailGa.Checked = False : chkEmailGa.ToolTip = ""
        End If
        'sim180518
        If IsDBNull(dvDocT.Item(0).Item("Data2InvioScadGa")) Then
            chkEmailGa2.Checked = False : chkEmailGa2.ToolTip = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data2InvioScadGa")) Then
            chkEmailGa2.Checked = True
            chkEmailGa2.ToolTip = Format(dvDocT.Item(0).Item("Data2InvioScadGa"), FormatoData)
        Else
            chkEmailGa2.Checked = False : chkEmailGa2.ToolTip = ""
        End If
        '----
        If IsDBNull(dvDocT.Item(0).Item("DataSostituzione")) Then
            txtDataSostituzione.Text = ""
        ElseIf IsDate(dvDocT.Item(0).Item("DataSostituzione")) Then
            txtDataSostituzione.Text = Format(dvDocT.Item(0).Item("DataSostituzione"), FormatoData)
        Else
            txtDataSostituzione.Text = ""
        End If
        lblModificatoDa.Text = IIf(IsDBNull(dvDocT.Item(0).Item("ModificatoDa")), "", dvDocT.Item(0).Item("ModificatoDa"))
        'Dati Contratto--------------------------------------------------------------------------
        txtNumero.Text = IIf(IsDBNull(dvDocT.Item(0).Item("Numero")), "", dvDocT.Item(0).Item("Numero").ToString.Trim)

        If IsDBNull(dvDocT.Item(0).Item("Data_Inizio")) Then
            txtDataInizio.Text = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data_Inizio")) Then
            txtDataInizio.Text = Format(dvDocT.Item(0).Item("Data_Inizio"), FormatoData)
        Else
            txtDataInizio.Text = ""
        End If
        If IsDBNull(dvDocT.Item(0).Item("Data_Fine")) Then
            txtDataFine.Text = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data_Fine")) Then
            txtDataFine.Text = Format(dvDocT.Item(0).Item("Data_Fine"), FormatoData)
        Else
            txtDataFine.Text = ""
        End If
        If IsDBNull(dvDocT.Item(0).Item("Importo")) Then
            TxtImporto.Text = "0,00"
        ElseIf IsNumeric(dvDocT.Item(0).Item("Importo")) Then
            TxtImporto.Text = FormattaNumero(dvDocT.Item(0).Item("Importo"), 2)
        Else
            TxtImporto.Text = "0,00"
        End If
        If IsDBNull(dvDocT.Item(0).Item("IDTipoContratto")) Then
            DDLTipoContratto.SelectedIndex = 0
        Else
            PosizionaItemDDL(dvDocT.Item(0).Item("IDTipoContratto").ToString.Trim, DDLTipoContratto)
        End If
        'Dati Articolo --------------------------------------------------------------------------
        TxtArticoloCod.Text = dvDocT.Item(0).Item("Cod_Articolo").ToString.Trim
        TxtArticoloDesc.Text = dvDocT.Item(0).Item("Descrizione").ToString.Trim
        TxtArticoloSN.Text = dvDocT.Item(0).Item("NSerie").ToString.Trim
        TxtArticoloLotto.Text = dvDocT.Item(0).Item("Lotto").ToString.Trim
        If IsDBNull(dvDocT.Item(0).Item("Attivo")) Then
            rbtnAttivo.Checked = True
        Else
            If dvDocT.Item(0).Item("Attivo") = 0 Then
                rbtnDismesso.Checked = True
            Else
                rbtnAttivo.Checked = True
            End If
        End If
        If IsDBNull(dvDocT.Item(0).Item("InRiparazione")) Then
            chkInRiparazione.Checked = False
        Else
            chkInRiparazione.Checked = CBool(dvDocT.Item(0).Item("InRiparazione"))
        End If
        If IsDBNull(dvDocT.Item(0).Item("Sostituito")) Then
            chkSostituito.Checked = False
        Else
            chkSostituito.Checked = CBool(dvDocT.Item(0).Item("Sostituito"))
        End If
        'giu050514 ELETTRODI
        If IsDBNull(dvDocT.Item(0).Item("DataScadElettrodi")) Then
            txtDataScadElettrodi.Text = ""
        ElseIf IsDate(dvDocT.Item(0).Item("DataScadElettrodi")) Then
            txtDataScadElettrodi.Text = Format(dvDocT.Item(0).Item("DataScadElettrodi"), FormatoData)
        Else
            txtDataScadElettrodi.Text = ""
        End If
        '-
        If IsDBNull(dvDocT.Item(0).Item("Data1InvioScadEl")) Then
            chkEmailEl.Checked = False : chkEmailEl.ToolTip = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data1InvioScadEl")) Then
            chkEmailEl.Checked = True
            chkEmailEl.ToolTip = Format(dvDocT.Item(0).Item("Data1InvioScadEl"), FormatoData)
        Else
            chkEmailEl.Checked = False : chkEmailEl.ToolTip = ""
        End If
        'sim180518
        If IsDBNull(dvDocT.Item(0).Item("Data2InvioScadEl")) Then
            chkEmailEl2.Checked = False : chkEmailEl2.ToolTip = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data2InvioScadEl")) Then
            chkEmailEl2.Checked = True
            chkEmailEl2.ToolTip = Format(dvDocT.Item(0).Item("Data2InvioScadEl"), FormatoData)
        Else
            chkEmailEl2.Checked = False : chkEmailEl2.ToolTip = ""
        End If
        '---- BATTERIE
        If IsDBNull(dvDocT.Item(0).Item("DataScadBatterie")) Then
            txtDataScadBatterie.Text = ""
        ElseIf IsDate(dvDocT.Item(0).Item("DataScadBatterie")) Then
            txtDataScadBatterie.Text = Format(dvDocT.Item(0).Item("DataScadBatterie"), FormatoData)
        Else
            txtDataScadBatterie.Text = ""
        End If
        '-
        If IsDBNull(dvDocT.Item(0).Item("Data1InvioScadBa")) Then
            chkEmailBa.Checked = False : chkEmailBa.ToolTip = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data1InvioScadBa")) Then
            chkEmailBa.Checked = True
            chkEmailBa.ToolTip = Format(dvDocT.Item(0).Item("Data1InvioScadBa"), FormatoData)
        Else
            chkEmailBa.Checked = False : chkEmailBa.ToolTip = ""
        End If
        'sim180518
        If IsDBNull(dvDocT.Item(0).Item("Data2InvioScadBa")) Then
            chkEmailBa2.Checked = False : chkEmailBa2.ToolTip = ""
        ElseIf IsDate(dvDocT.Item(0).Item("Data2InvioScadBa")) Then
            chkEmailBa2.Checked = True
            chkEmailBa2.ToolTip = Format(dvDocT.Item(0).Item("Data2InvioScadBa"), FormatoData)
        Else
            chkEmailBa2.Checked = False : chkEmailBa2.ToolTip = ""
        End If
        'Dati Cliente --------------------------------------------------------------------------
        txtCodCliForFilProvv.Text = dvDocT.Item(0).Item("Cod_Coge").ToString
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
            LblCliente.Text = "Cliente"
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            TabCliFor = "For"
            LblCliente.Text = "Fornitore"
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text
        If IsNumeric(dvDocT.Item(0).Item("Cod_Filiale").ToString) Then
            Session(CSTCODFILIALE) = dvDocT.Item(0).Item("Cod_Filiale").ToString
            GetDatiAnagraficiCliente(dvDocT.Item(0).Item("Cod_Filiale"))
        Else
            Session(CSTCODFILIALE) = ""
            GetDatiAnagraficiCliente(0)
        End If
        txtDestinazione1.Text = dvDocT.Item(0).Item("Destinazione1").ToString.Trim
        txtDestinazione2.Text = dvDocT.Item(0).Item("Destinazione2").ToString.Trim
        txtDestinazione3.Text = dvDocT.Item(0).Item("Destinazione3").ToString.Trim
        txtRiferimento.Text = dvDocT.Item(0).Item("Riferimento").ToString.Trim
        txtNsRiferimento.Text = dvDocT.Item(0).Item("NsRiferimento").ToString.Trim

        ' ''TxtTelefono1.Text = dvDocT.Item(0).Item("Telefono1").ToString.Trim
        ' ''TxtTelefono2.Text = dvDocT.Item(0).Item("Telefono2").ToString.Trim
        ' ''TxtFax.Text = dvDocT.Item(0).Item("Fax").ToString.Trim
        ' ''TxtEmail.Text = dvDocT.Item(0).Item("Email").ToString.Trim

        ' ''txtNoteDocumento.Text = dvDocT.Item(0).Item("NoteDocumento").ToString.Trim

        Call SetLblEmailInvio()
        TxtEmail.AutoPostBack = True
        txtEmailDest.AutoPostBack = True
        txtEmailInvioScad.AutoPostBack = True

        'Sim180518: Prelevo dato Email da DestClienti
        Dim strCodFil As String = Session(CSTCODFILIALE)
        If IsNothing(strCodFil) Then
            strCodFil = ""
        End If
        If String.IsNullOrEmpty(strCodFil) Then
            strCodFil = ""
        End If
        If strCodFil = "" Or Not IsNumeric(strCodFil) Then
            txtEmailDest.Text = ""
            Exit Sub
        End If
        '-
        Dim dvDest As DataView
        dvDest = SqlDSDestinazione.Select(DataSourceSelectArguments.Empty)
        If (dvDest Is Nothing) Then
            txtEmailDest.Text = ""
        Else
            If dvDest.Count > 0 Then
                dvDest.RowFilter = "Progressivo = " & Session(CSTCODFILIALE)
                If dvDest.Count > 0 Then
                    txtEmailDest.Text = dvDest.Item(0).Item("EMail").ToString   'popolo il campo EmailDest
                Else
                    txtEmailDest.Text = ""
                End If
            Else
                txtEmailDest.Text = ""
            End If
        End If
        Call SetLblEmailInvio()
        '----
        
    End Sub
    Private Sub BuildDest(ByVal CodFil As Integer)
        Dim dvDest As DataView
        dvDest = SqlDSDestinazione.Select(DataSourceSelectArguments.Empty)
        If (dvDest Is Nothing) Then
            DDLDestinazioni.Items.Clear()
            DDLDestinazioni.Items.Add("")
            DDLDestinazioni.DataBind()
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
            Session(CSTCODFILIALE) = ""
            Exit Sub
        End If
        If dvDest.Count > 0 Then
            DDLDestinazioni.Items.Clear()
            DDLDestinazioni.Items.Add("[SONO PRESENTI DESTINAZIONI DIVERSE]")
            DDLDestinazioni.DataBind()
        Else
            DDLDestinazioni.Items.Clear()
            DDLDestinazioni.Items.Add("")
            DDLDestinazioni.DataBind()
            Session(CSTCODFILIALE) = ""
        End If
        PosizionaItemDDL(CodFil.ToString, DDLDestinazioni)
    End Sub

    Private Sub AzzeraTxtDocT()
        SfondoCampiDocT()
        TxtEmail.AutoPostBack = False
        txtEmailDest.AutoPostBack = False
        txtEmailInvioScad.AutoPostBack = False
        ' ''Session(CSTSTATODOC) = "0"
        txtDataInstallazione.Text = ""
        txtDataScadGaranzia.Text = "" : txtDataScadGaranzia.ToolTip = ""
        chkEmailGa.Checked = False : chkEmailGa.ToolTip = "" 'GIU050514
        chkEmailGa2.Checked = False : chkEmailGa2.ToolTip = "" 'SIM180518
        ' ''lblDataScadContratto.Text = ""
        txtDataSostituzione.Text = ""

        txtNumero.Text = ""
        txtDataInizio.Text = ""
        txtDataFine.Text = ""
        TxtImporto.Text = FormattaNumero("0", 2)
        DDLTipoContratto.SelectedIndex = 0

        TxtArticoloCod.Text = ""
        TxtArticoloDesc.Text = ""
        TxtArticoloLotto.Text = ""
        TxtArticoloSN.Text = ""
        '-
        rbtnAttivo.Checked = True : rbtnDismesso.Checked = False
        chkInRiparazione.Checked = False : chkSostituito.Checked = False
        'GIU050514
        txtDataScadBatterie.Text = "" : txtDataScadBatterie.ToolTip = ""
        chkEmailBa.Checked = False : chkEmailBa.ToolTip = ""
        chkEmailBa2.Checked = False : chkEmailBa2.ToolTip = ""
        txtDataScadElettrodi.Text = "" : txtDataScadElettrodi.ToolTip = ""
        chkEmailEl.Checked = False : chkEmailEl.ToolTip = ""
        chkEmailEl2.Checked = False : chkEmailEl2.ToolTip = ""
        '---------
        LblCliente.Text = "Cliente" : txtCodCliForFilProvv.Text = ""
        lblRagSoc.Text = "" : lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""

        DDLDestinazioni.Items.Clear()
        txtDestinazione1.Text = ""
        txtDestinazione2.Text = ""
        txtDestinazione3.Text = ""
        txtRiferimento.Text = ""
        txtNsRiferimento.Text = ""
        txtEmailDest.Text = ""  'simone180518

        lblTelefono1.Text = "" : lblTelefono2.Text = "" : lblFax.Text = "" : TxtEmail.Text = "" : txtEmailInvioScad.Text = "" 'simone180518
        ' ''TxtTelefono2.Text = ""
        ' ''TxtEmail.Text = ""
        ' ''TxtFax.Text = ""
        lblEmailInvio.Text = ""
        chkAggEmailT.Checked = False
        TxtEmail.AutoPostBack = True
        txtEmailDest.AutoPostBack = True
        txtEmailInvioScad.AutoPostBack = True
        ' ''txtNoteDocumento.Text = ""
    End Sub
    Private Sub SfondoCampiDocT()
        txtDataInstallazione.BackColor = SEGNALA_OK : txtDataInstallazione.ToolTip = ""
        txtDataScadGaranzia.BackColor = SEGNALA_OK : txtDataScadGaranzia.ToolTip = ""
        ' ''lblDataScadContratto.BackColor = SEGNALA_OK
        ' ''lblDataScadContratto.ToolTip = ""
        txtDataSostituzione.BackColor = SEGNALA_OK : txtDataSostituzione.ToolTip = ""

        txtNumero.BackColor = SEGNALA_OK : txtNumero.ToolTip = ""
        txtDataInizio.BackColor = SEGNALA_OK
        txtDataInizio.ToolTip = ""
        txtDataFine.BackColor = SEGNALA_OK
        txtDataFine.ToolTip = ""
        TxtImporto.BackColor = SEGNALA_OK
        DDLTipoContratto.BackColor = SEGNALA_OK

        TxtArticoloCod.BackColor = SEGNALA_OK : TxtArticoloCod.ToolTip = ""
        TxtArticoloDesc.BackColor = SEGNALA_OK : TxtArticoloDesc.ToolTip = ""
        TxtArticoloLotto.BackColor = SEGNALA_OK : TxtArticoloLotto.ToolTip = ""
        TxtArticoloSN.BackColor = SEGNALA_OK : TxtArticoloSN.ToolTip = ""
        'GIU050514
        txtDataScadBatterie.BackColor = SEGNALA_OK : txtDataScadBatterie.ToolTip = ""
        txtDataScadElettrodi.BackColor = SEGNALA_OK : txtDataScadElettrodi.ToolTip = ""
        '---------
        txtCodCliForFilProvv.BackColor = SEGNALA_OK : txtCodCliForFilProvv.ToolTip = ""
        lblRagSoc.ForeColor = Drawing.Color.Black
        lblPICF.ForeColor = Drawing.Color.Black : lblIndirizzo.ForeColor = Drawing.Color.Black : lblLocalita.ForeColor = Drawing.Color.Black
        '---------
        lblRagSoc.ToolTip = "" : lblPICF.ToolTip = "" : lblIndirizzo.ToolTip = "" : lblLocalita.ToolTip = ""
        '-
        DDLDestinazioni.BackColor = SEGNALA_OK : DDLDestinazioni.ToolTip = ""
        txtDestinazione1.BackColor = SEGNALA_OK : txtDestinazione1.ToolTip = ""
        txtDestinazione2.BackColor = SEGNALA_OK : txtDestinazione2.ToolTip = ""
        txtDestinazione3.BackColor = SEGNALA_OK : txtDestinazione3.ToolTip = ""

        txtRiferimento.BackColor = SEGNALA_OK : txtRiferimento.ToolTip = ""
        txtNsRiferimento.BackColor = SEGNALA_OK : txtNsRiferimento.ToolTip = ""
        ' ''lblTelefono1.BackColor = SEGNALA_OK : lblTelefono1.ToolTip = ""
        ' ''TxtTelefono2.BackColor = SEGNALA_OK : TxtTelefono2.ToolTip = ""
        ' ''TxtFax.BackColor = SEGNALA_OK : TxtFax.ToolTip = ""
        ' ''TxtEmail.BackColor = SEGNALA_OK : TxtEmail.ToolTip = ""

        ' ''txtNoteDocumento.BackColor = SEGNALA_OK : txtNoteDocumento.ToolTip = ""

        'sim210518
        txtEmailDest.BackColor = SEGNALA_OK : txtEmailDest.ToolTip = ""
        TxtEmail.BackColor = SEGNALA_OK : TxtEmail.ToolTip = ""
        txtEmailInvioScad.BackColor = SEGNALA_OK : txtEmailInvioScad.ToolTip = ""
        '----
        chkEmailBa.BackColor = SEGNALA_OK
        chkEmailBa2.BackColor = SEGNALA_OK
        chkEmailEl.BackColor = SEGNALA_OK
        chkEmailEl2.BackColor = SEGNALA_OK
        chkEmailGa.BackColor = SEGNALA_OK
        chkEmailGa2.BackColor = SEGNALA_OK
    End Sub
    Private Sub CampiSetEnabledToT(ByVal Valore As Boolean)
        LnkRitorno.Enabled = Not Valore
        LnkRitorno.Visible = Not Valore

        txtDataInstallazione.Enabled = Valore
        imgBtnShowCalendarDInst.Enabled = Valore
        '-
        txtDataScadGaranzia.Enabled = Valore
        imgBtnShowCalendarDG.Enabled = Valore
        chkEmailGa.Enabled = Valore 'giu050514
        chkEmailGa2.Enabled = Valore 'sim180518
        '-
        txtDataSostituzione.Enabled = Valore
        imgBtnShowCalendarDSost.Enabled = Valore

        txtNumero.Enabled = Valore : btnGetNewNumero.Enabled = Valore
        txtDataInizio.Enabled = Valore
        imgBtnShowCalendarDIC.Enabled = Valore
        txtDataFine.Enabled = Valore
        imgBtnShowCalendarDFC.Enabled = Valore
        TxtImporto.Enabled = Valore
        '-
        DDLTipoContratto.Enabled = Valore
        '---
        BtnSelArticoloIns.Enabled = Valore
        TxtArticoloCod.Enabled = Valore
        TxtArticoloDesc.Enabled = Valore
        TxtArticoloLotto.Enabled = Valore
        TxtArticoloSN.Enabled = Valore
        '-
        rbtnAttivo.Enabled = Valore
        rbtnDismesso.Enabled = Valore
        chkInRiparazione.Enabled = Valore
        chkSostituito.Enabled = Valore
        'giu050514
        txtDataScadBatterie.Enabled = Valore : imgBtnShowCalendarDScadBa.Enabled = Valore : chkEmailBa.Enabled = Valore : chkEmailBa2.Enabled = Valore  'sim220518
        txtDataScadElettrodi.Enabled = Valore : imgBtnShowCalendarDSCEl.Enabled = Valore : chkEmailEl.Enabled = Valore : chkEmailEl2.Enabled = Valore 'sim220518
        '---------
        btnCercaAnagrafica.Enabled = Valore
        btnModificaAnagrafica.Enabled = Valore
        txtCodCliForFilProvv.Enabled = Valore
        DDLDestinazioni.Enabled = Valore
        txtDestinazione1.Enabled = Valore
        txtDestinazione2.Enabled = Valore
        txtDestinazione3.Enabled = Valore
        txtRiferimento.Enabled = Valore
        txtNsRiferimento.Enabled = Valore
        ' ''lblTelefono1.Enabled = Valore
        ' ''TxtTelefono2.Enabled = Valore
        ' ''TxtFax.Enabled = Valore
        ' ''TxtEmail.Enabled = Valore

        ' ''txtNoteDocumento.Enabled = Valore

        'simone210518
        TxtEmail.Enabled = Valore
        txtEmailDest.Enabled = Valore
        txtEmailInvioScad.Enabled = Valore
        chkAggEmailT.Enabled = Valore
        TipoDoc = Session(CSTTIPODOC)
        
    End Sub
#End Region

    Public Sub LnkRitorno_Click()
        If Session(SWOP) = SWOPELIMINA Then
            Session(SWOP) = SWOPNESSUNA
            Chiudi("")
            Exit Sub
        End If
        If Session(SWOP) <> SWOPNESSUNA Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA
        Chiudi("")
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        If Session(SWOP) = SWOPELIMINA Then
            Session(SWOP) = SWOPNESSUNA
            Chiudi("")
            Exit Sub
        End If
        If Session(SWMODIFICATO) = SWSI Then
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = "AnnullaModificheDocumento"
            ModalPopup.Show("Annulla modifiche", "Confermi l'annullamento modifiche?", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            AnnullaModificheDocumento()
        End If

    End Sub
    Public Sub AnnullaModificheDocumento()
        Session(SWOP) = SWOPNESSUNA
        Session(SWMODIFICATO) = SWNO
        Dim myID As String = ""
        'giu280512 unificato 
        ' ''TipoDoc = Session(CSTTIPODOC)
        ' ''Try
        ' ''    If TipoDoc = SWTD(TD.ArticoloInstallato) Then
        ' ''        myID = Session(IDARTICOLOINST)
        ' ''    ElseIf TipoDoc = SWTD(TD.ContrattoAssistenza) Then
        ' ''        myID = Session(IDCONTRATTOASSISTENZA)
        ' ''    End If
        ' ''Catch ex As Exception
        ' ''    myID = "-1"
        myID = Session(IDARTICOLOINST)
        '---------------------------
        If Session(SWOP) = SWOPNUOVO Then
            myID = Session(IDDOCUMSAVE)
        End If
        If IsNothing(myID) Then
            myID = "-1"
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = "-1"
        End If
        If myID = "" Then
            Chiudi("")
            Exit Sub
        End If
        'giu280512 unificato 
        ' ''Try
        ' ''    If TipoDoc = SWTD(TD.ArticoloInstallato) Then
        ' ''        Session(IDARTICOLOINST) = myID
        ' ''    ElseIf TipoDoc = SWTD(TD.ContrattoAssistenza) Then
        ' ''        Session(IDCONTRATTOASSISTENZA) = myID
        ' ''    End If
        ' ''Catch ex As Exception
        ' ''    myID = "-1"
        ' ''End Try
        Session(IDARTICOLOINST) = myID
        '--------------------
        SqlDbSelectCmd = Session("SqlDbSelectCmd")
        SqlDbInserCmd = Session("SqlDbInserCmd")
        SqlDbDeleteCmd = Session("SqlDbDeleteCmd")
        SqlDbUpdateCmd = Session("SqlDbUpdateCmd")
        SqlAdapDoc = Session("SqlAdapDoc")
        SqlAdapDoc.SelectCommand = SqlDbSelectCmd
        SqlAdapDoc.InsertCommand = SqlDbInserCmd
        SqlAdapDoc.DeleteCommand = SqlDbDeleteCmd
        SqlAdapDoc.UpdateCommand = SqlDbUpdateCmd

        SqlDbSelectCmd.Parameters.Item("@IDARTICOLOINST").Value = CLng(myID)
        DsArtInst.ArticoliInstallati.Clear()
        SqlAdapDoc.Fill(DsArtInst.ArticoliInstallati)
        'popolo
        Session("DsDocT") = DsArtInst
        If (dvDocT Is Nothing) Then
            dvDocT = New DataView(DsArtInst.ArticoliInstallati)
        End If
        Session("dvDocT") = dvDocT

        CampiSetEnabledToT(False)
        If dvDocT.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            PopolaTxtDocT()
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            AzzeraTxtDocT()
        End If

    End Sub

    Public Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        End If
        Try
            Response.Redirect("WF_ArticoliInst_ContrattiAssElenco.aspx?labelForm=Articoli consumabili Clienti")
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ArticoliInstallati.Chiudi", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        btnAggiorna.Click

        BtnAggionaDocumento()

    End Sub
    Public Sub BtnAggionaDocumento()
        BtnSetEnabledTo(False) 'disattivo i tasti per evitare
        If ControllaDocT() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati articolo", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_ALERT)
            btnAggiorna.Enabled = True
            btnAnnulla.Enabled = True
            Exit Sub
        End If
       
        Session(SWOP) = SWOPNESSUNA
        BtnSetEnabledTo(True)
        btnAggiorna.Enabled = False
        btnAnnulla.Enabled = False
        OkAggiornaDoc()
    End Sub
    Public Sub OkAggiornaDoc()
        Dim strErrore As String = ""
        Dim SWTB0 As Boolean = AggiornaDocT(strErrore)

        SqlDbSelectCmd = Session("SqlDbSelectCmd")
        SqlDbInserCmd = Session("SqlDbInserCmd")
        SqlDbDeleteCmd = Session("SqlDbDeleteCmd")
        SqlDbUpdateCmd = Session("SqlDbUpdateCmd")
        SqlAdapDoc = Session("SqlAdapDoc")
        SqlAdapDoc.SelectCommand = SqlDbSelectCmd
        SqlAdapDoc.InsertCommand = SqlDbInserCmd
        SqlAdapDoc.DeleteCommand = SqlDbDeleteCmd
        SqlAdapDoc.UpdateCommand = SqlDbUpdateCmd

        Dim myID As String = ""
        'giu280512 unificato 
        ' ''TipoDoc = Session(CSTTIPODOC)
        ' ''Try
        ' ''    If TipoDoc = SWTD(TD.ArticoloInstallato) Then
        ' ''        myID = Session(IDARTICOLOINST)
        ' ''    ElseIf TipoDoc = SWTD(TD.ContrattoAssistenza) Then
        ' ''        myID = Session(IDCONTRATTOASSISTENZA)
        ' ''    End If
        ' ''Catch ex As Exception
        ' ''    myID = ""
        ' ''End Try
        myID = Session(IDARTICOLOINST)
        '-------------------
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Or Not IsNumeric(myID) Then
            Session(SWOP) = SWOPNESSUNA
            Chiudi("Errore: IDENTIFICATIVO ARTICOLO INSTALLATO/CONTRATTO SCONOSCIUTO")
            Exit Sub
        End If
        SqlDbSelectCmd.Parameters.Item("@IDARTICOLOINST").Value = CLng(myID)
        SqlAdapDoc.Fill(DsArtInst.ArticoliInstallati)
        Session("DsDocT") = DsArtInst
        If (dvDocT Is Nothing) Then
            dvDocT = New DataView(DsArtInst.ArticoliInstallati)
        End If
        Session("dvDocT") = dvDocT
        CampiSetEnabledToT(False)
        If dvDocT.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            PopolaTxtDocT()
        Else
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCR) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            btnNuovo.Enabled = True
            AzzeraTxtDocT()
            Session(IDARTICOLOINST) = ""
            'giu280512 unificato Session(IDCONTRATTOASSISTENZA) = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Aggiorna dati", "Attenzione, nessun dato aggiornato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPNESSUNA


        ' ''Dim sTipoUtente As String = ""
        ' ''If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
        ' ''    Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        ' ''    If (Utente Is Nothing) Then
        ' ''        'non faccio nulla ESEGUO IL RICALCOLO
        ' ''    Else
        ' ''        sTipoUtente = Utente.Tipo
        ' ''    End If
        ' ''Else
        ' ''    sTipoUtente = Session(CSTTIPOUTENTE)
        ' ''End If

        'GIU070918
        Try
            If chkAggEmailT.Checked = True And lblEmailInvio.Text.Trim <> "" Then
                Dim ClsDB As New DataBaseUtility
                Dim LogInvioEmail As String = ClsDB.GetLogInvioEmail(Session(ESERCIZIO))
                If Mid(LogInvioEmail.Trim, 1, 20) = "INVIO EMAIL IN CORSO" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Attenzione, INVIO EMAIL IN CORSO ... Attendere il termine dell'invio e riprovare.<BR>" & _
                                    "Aggiornamento tutte le E-mail inviate in archivio", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                ElseIf UCase(Mid(LogInvioEmail.Trim, 1, 6)) = "ERRORE" Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Errore, lettura Parametri generali: " & LogInvioEmail.Trim & "<BR>" & _
                                    "Aggiornamento tutte le E-mail inviate in archivio", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                'aggiorno E-mail inviate
                Dim SQLStr As String = "UPDATE EmailInviateT SET Email='" & Controlla_Apice(lblEmailInvio.Text.Trim) & "' " & _
                    "FROM EmailInviateT INNER JOIN " & _
                    "EmailInviateDett ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN " & _
                    "ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID " & _
                    "WHERE (ArticoliInst_ContrattiAss.Cod_Coge = N'" & txtCodCliForFilProvv.Text.Trim & "')"
                If ClsDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Aggiornamento E-mail invio.", WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                ClsDB = Nothing
                chkAggEmailT.Checked = False
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento E-mail invio.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        
    End Sub
    Private Sub SetLblEmailInvio()
        If txtEmailDest.Text.Trim = "" Then
            If txtEmailInvioScad.Text.Trim = "" Then
                lblEmailInvio.Text = TxtEmail.Text
            Else
                lblEmailInvio.Text = txtEmailInvioScad.Text
            End If
        ElseIf DDLDestinazioni.SelectedIndex > 0 Then
            lblEmailInvio.Text = txtEmailDest.Text
        Else
            If txtEmailInvioScad.Text.Trim = "" Then
                lblEmailInvio.Text = TxtEmail.Text
            Else
                lblEmailInvio.Text = txtEmailInvioScad.Text
            End If
        End If
    End Sub
    Public Sub NoAggiornaDoc()
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
    End Sub
    Private Function ControllaDocT() As Boolean
        ControllaDocT = True
        SfondoCampiDocT()
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ArticoliInstallati.ControllaDocT)")
            Exit Function
        End If
        'Date
        If txtDataInstallazione.Text.Trim <> "" Then
            If Not IsDate(txtDataInstallazione.Text.Trim) Then
                txtDataInstallazione.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        Else
            txtDataInstallazione.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        If txtDataScadGaranzia.Text.Trim <> "" Then
            If Not IsDate(txtDataScadGaranzia.Text.Trim) Then
                txtDataScadGaranzia.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf IsDate(txtDataInstallazione.Text.Trim) Then
                If CDate(txtDataScadGaranzia.Text.Trim).Date < CDate(txtDataInstallazione.Text.Trim).Date Then
                    txtDataScadGaranzia.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            End If
        Else
            'NON Obbligatoria txtDataScadGaranzia.BackColor = SEGNALA_KO : ControllaDocT = False
        End If

        If txtDataSostituzione.Text.Trim <> "" Then
            If Not IsDate(txtDataSostituzione.Text.Trim) Then
                txtDataSostituzione.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf IsDate(txtDataInstallazione.Text.Trim) Then
                If CDate(txtDataSostituzione.Text.Trim).Date < CDate(txtDataInstallazione.Text.Trim).Date Then
                    txtDataSostituzione.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            End If
        Else
            'NON Obbligatoria txtDataSostituzione.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        '=======================
        'Dati Contratto
        'giu280512 adesso è tutto uguale
        'giu280512 adesso è tutto uguale If String.Equals(TipoDoc, SWTD(TD.ContrattoAssistenza)) Then
        Dim SWContratto As Boolean = False
        If txtNumero.Text.Trim <> "" Then
            If IsNumeric(txtNumero.Text.Trim) Then
                If CLng(txtNumero.Text.Trim) > 0 Then
                    SWContratto = True
                End If
            End If
        End If
        If txtDataInizio.Text.Trim <> "" Then SWContratto = True
        If txtDataFine.Text.Trim <> "" Then SWContratto = True
        If TxtImporto.Text.Trim = "" Then TxtImporto.Text = "0"
        If TxtImporto.Text.Trim <> "" Then
            If IsNumeric(TxtImporto.Text.Trim) Then
                If CDec(TxtImporto.Text.Trim) > 0 Then
                    SWContratto = True
                End If
            End If
        End If
        If DDLTipoContratto.SelectedIndex > 0 Then SWContratto = True
        '--------------------------------
        If txtNumero.Text.Trim <> "" Then
            If Not IsNumeric(txtNumero.Text.Trim) Then
                txtNumero.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf CLng(txtNumero.Text.Trim) < 1 Then
                txtNumero.BackColor = SEGNALA_OK : txtNumero.Text = ""
            ElseIf CheckNewNumeroOnTab() = True Then
                txtNumero.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        ElseIf SWContratto = True Then
            txtNumero.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        '-
        If txtDataInizio.Text.Trim <> "" Then
            If Not IsDate(txtDataInizio.Text.Trim) Then
                txtDataInizio.BackColor = SEGNALA_KO : ControllaDocT = False
                ' ''ElseIf IsDate(txtDataInstallazione.Text.Trim) Then
                ' ''    If CDate(txtDataInizio.Text.Trim).Date < CDate(txtDataInstallazione.Text.Trim).Date Then
                ' ''        txtDataInizio.BackColor = SEGNALA_KO : ControllaDocT = False
                ' ''    End If
            End If
        ElseIf SWContratto = True Then
            txtDataInizio.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        '-
        If txtDataFine.Text.Trim <> "" Then
            If Not IsDate(txtDataFine.Text.Trim) Then
                txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
                ' ''ElseIf IsDate(txtDataInstallazione.Text.Trim) Then
                ' ''    If CDate(txtDataFine.Text.Trim).Date < CDate(txtDataInstallazione.Text.Trim).Date Then
                ' ''        txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
                ' ''    End If
            ElseIf IsDate(txtDataInizio.Text.Trim) Then
                If CDate(txtDataFine.Text.Trim).Date < CDate(txtDataInizio.Text.Trim).Date Then
                    txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            End If
        ElseIf SWContratto = True Then
            txtDataFine.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        If TxtImporto.Text.Trim = "" Then TxtImporto.Text = "0"
        Try
            If Not IsNumeric(CDec(TxtImporto.Text.Trim)) Then TxtImporto.BackColor = SEGNALA_KO
            If Not IsNumeric(CDec(TxtImporto.Text.Trim)) Then TxtImporto.Text = "0"
        Catch ex As Exception
            TxtImporto.Text = "0" : TxtImporto.BackColor = SEGNALA_KO
        End Try
        FormattaNumero(TxtImporto.Text.Trim, 2)
        If DDLTipoContratto.SelectedIndex > 0 Then
            'OK
        ElseIf SWContratto = True Then
            DDLTipoContratto.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        If SWContratto = False Then
            txtNumero.Text = "0" : txtDataInizio.Text = "" : txtDataFine.Text = "" : TxtImporto.Text = "0"
            DDLTipoContratto.SelectedIndex = 0
        End If
        '=================
        'Dati Articolo 
        'Nota: per i contratti l'articolo non è obbligatorio esempio contratto per software della makkina
        If SWContratto = False Then
            If TxtArticoloCod.Text.Trim <> "" Then
                Dim myCodArt As String = TxtArticoloCod.Text
                Dim myDesArt As String = ""
                If LeggiArticolo(myCodArt, myDesArt) = False Then
                    TxtArticoloCod.BackColor = SEGNALA_KO : ControllaDocT = False
                ElseIf myDesArt.Trim = "" Then
                    TxtArticoloCod.BackColor = SEGNALA_KO
                Else
                    TxtArticoloCod.Text = myCodArt
                    TxtArticoloCod.BackColor = SEGNALA_OK
                    TxtArticoloDesc.Text = myDesArt.Trim
                End If
            Else
                TxtArticoloCod.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        Else
            If TxtArticoloCod.Text.Trim <> "" Then
                Dim myCodArt As String = TxtArticoloCod.Text
                Dim myDesArt As String = ""
                If LeggiArticolo(myCodArt, myDesArt) = False Then
                    TxtArticoloCod.BackColor = SEGNALA_KO : ControllaDocT = False
                ElseIf myDesArt.Trim = "" Then
                    TxtArticoloCod.BackColor = SEGNALA_KO
                Else
                    TxtArticoloCod.Text = myCodArt
                    TxtArticoloCod.BackColor = SEGNALA_OK
                    TxtArticoloDesc.Text = myDesArt.Trim
                End If
            End If
        End If

        If rbtnAttivo.Checked = False And rbtnDismesso.Checked = False Then
            rbtnAttivo.Checked = True : rbtnDismesso.Checked = False
        End If
        'GIU050514
        If txtDataScadElettrodi.Text.Trim <> "" Then
            If Not IsDate(txtDataScadElettrodi.Text.Trim) Then
                txtDataScadElettrodi.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf IsDate(txtDataInstallazione.Text.Trim) Then
                If CDate(txtDataScadElettrodi.Text.Trim).Date < CDate(txtDataInstallazione.Text.Trim).Date Then
                    txtDataScadElettrodi.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            End If
        Else
            'NON Obbligatoria txtDataScadElettrodi.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        '-
        If txtDataScadBatterie.Text.Trim <> "" Then
            If Not IsDate(txtDataScadBatterie.Text.Trim) Then
                txtDataScadBatterie.BackColor = SEGNALA_KO : ControllaDocT = False
            ElseIf IsDate(txtDataInstallazione.Text.Trim) Then
                If CDate(txtDataScadBatterie.Text.Trim).Date < CDate(txtDataInstallazione.Text.Trim).Date Then
                    txtDataScadBatterie.BackColor = SEGNALA_KO : ControllaDocT = False
                End If
            End If
        Else
            'NON Obbligatoria txtDataScadBatterie.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        '---------
        'Dati Cliente
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            If App.GetValoreFromChiave(txtCodCliForFilProvv.Text.Trim, Def.CLIENTI, Session(ESERCIZIO)) = "" Then
                txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            If App.GetValoreFromChiave(txtCodCliForFilProvv.Text.Trim, Def.FORNITORI, Session(ESERCIZIO)) = "" Then
                txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
            End If
        Else
            txtCodCliForFilProvv.BackColor = SEGNALA_KO : ControllaDocT = False
        End If
        '-
        If chkEmailBa.Checked = False And chkEmailBa2.Checked Then
            chkEmailBa.BackColor = SEGNALA_OK : ControllaDocT = False
        End If
        If chkEmailEl.Checked = False And chkEmailEl2.Checked Then
            chkEmailEl.BackColor = SEGNALA_OK : ControllaDocT = False
        End If
        If chkEmailGa.Checked = False And chkEmailGa2.Checked Then
            chkEmailGa.BackColor = SEGNALA_OK : ControllaDocT = False
        End If
    End Function
    Private Sub txtNumero_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNumero.TextChanged
        If txtNumero.Text.Trim = "" Or Not IsNumeric(txtNumero.Text.Trim) Then
            txtNumero.BackColor = SEGNALA_KO
            txtNumero.Focus()
            Exit Sub
        End If
        If CheckNewNumeroOnTab() = True Then
            txtNumero.BackColor = SEGNALA_KO
            'txtRevNDoc.BackColor = SEGNALA_KO
        Else
            txtNumero.BackColor = SEGNALA_OK
            'txtRevNDoc.BackColor = SEGNALA_OK
        End If
        txtDataInizio.Focus()
    End Sub
    Private Function CheckNewNumeroOnTab() As Boolean
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ArticoliInstallati.CheckNewNumeroOnTab)")
            Exit Function
        End If
        Dim myID As String = ""
        'giu280512 unificato 
        ' ''TipoDoc = Session(CSTTIPODOC)
        ' ''Try
        ' ''    If TipoDoc = SWTD(TD.ArticoloInstallato) Then
        ' ''        myID = Session(IDARTICOLOINST)
        ' ''    ElseIf TipoDoc = SWTD(TD.ContrattoAssistenza) Then
        ' ''        myID = Session(IDCONTRATTOASSISTENZA)
        ' ''    End If
        ' ''Catch ex As Exception
        ' ''    myID = ""
        ' ''End Try
        myID = Session(IDARTICOLOINST)
        '---------------------
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" And Session(SWOP) <> SWOPNUOVO Then
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
        '
        Dim strErrore As String = ""
        'If Not IsNumeric(txtRevNDoc.Text.Trim) Then txtRevNDoc.Text = "0"
        'giu140514 Dim NRev As Integer = 0
        If txtNumero.Text.Trim <> "" And IsNumeric(txtNumero.Text.Trim) Then
            Dim strSQL As String = "Select ID From ArticoliInst_ContrattiAss WHERE "
            'giu280512 unificato 
            ' ''strSQL += "Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
            ' ''If Session(SWOP) = SWOPNUOVO Then
            ' ''    strSQL += " AND Numero = '" & txtNumero.Text.Trim & "'"
            ' ''    strSQL += " AND RevisioneNDoc = " & NRev.ToString.Trim & ""
            ' ''Else
            ' ''    strSQL += " AND ID <> " & myID.Trim
            ' ''    strSQL += " AND Numero = '" & txtNumero.Text.Trim & "'"
            ' ''    strSQL += " AND RevisioneNDoc = " & NRev.ToString.Trim & ""
            ' ''End If
            If Session(SWOP) = SWOPNUOVO Then
                strSQL += " Numero = '" & txtNumero.Text.Trim & "'"
                'giu140514 strSQL += " AND RevisioneNDoc = " & NRev.ToString.Trim & ""
            Else
                strSQL += " ID <> " & myID.Trim
                strSQL += " AND Numero = '" & txtNumero.Text.Trim & "'"
                'giu140514 strSQL += " AND RevisioneNDoc = " & NRev.ToString.Trim & ""
            End If
            '-------------------
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
                ObjDB = Nothing
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        CheckNewNumeroOnTab = True
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Numero già presente in tabella", WUC_ModalPopup.TYPE_ALERT)
                        Exit Function
                    End If
                Else
                    Exit Function
                End If
            Catch Ex As Exception
                CheckNewNumeroOnTab = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in CheckNewNumeroOnTab", "Verifica N° da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End Try
        End If
    End Function
    Private Sub btnGetNewNumero_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGetNewNumero.Click
        txtNumero.Text = GetNewNumeroDocCA()
        txtDataInizio.Focus()
    End Sub

    Public Function AggiornaDocT(ByRef strErrore As String) As Boolean

        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO ARTICOLO INSTALLATO/CONTRATTO (ArticoliInstallati.AggiornaDocT)")
            Exit Function
        End If

        AggiornaDocT = True
        Dim SWErr As Boolean = False
        '--------
        If TxtImporto.Text.Trim = "" Then TxtImporto.Text = "0"
        Try
            If Not IsNumeric(CDec(TxtImporto.Text.Trim)) Then TxtImporto.BackColor = SEGNALA_KO
            If Not IsNumeric(CDec(TxtImporto.Text.Trim)) Then TxtImporto.Text = "0"
        Catch ex As Exception
            TxtImporto.Text = "0" : TxtImporto.BackColor = SEGNALA_KO
        End Try
        FormattaNumero(TxtImporto.Text.Trim, 2)
        '--------------------------------------------------------------------------------------
        If Session(SWOP) = SWOPNUOVO Then
            Dim newRowDocT As DSDocumenti.ArticoliInstallatiRow = DsArtInst.ArticoliInstallati.NewArticoliInstallatiRow
            With newRowDocT
                .BeginEdit()
                .Tipo_Doc = Session(CSTTIPODOC)
                If IsDate(txtDataInstallazione.Text) Then
                    .Data_Installazione = CDate(txtDataInstallazione.Text)
                Else
                    .SetData_InstallazioneNull()
                End If
                If IsDate(txtDataScadGaranzia.Text) Then
                    .DataScadGaranzia = CDate(txtDataScadGaranzia.Text)
                Else
                    .SetDataScadGaranziaNull()
                End If
                '-GIU050514
                If chkEmailGa.Checked = True Then
                    If .IsData1InvioScadGaNull Then
                        .Data1InvioScadGa = CDate(Now.Date)
                    End If
                Else
                    .SetData1InvioScadGaNull()
                End If
                '-sim180518
                If chkEmailGa2.Checked = True Then
                    If .IsData2InvioScadGaNull Then
                        .Data2InvioScadGa = CDate(Now.Date)
                    End If
                Else
                    .SetData2InvioScadGaNull()
                End If
                '---
                If IsDate(txtDataSostituzione.Text) Then
                    .DataSostituzione = CDate(txtDataSostituzione.Text)
                Else
                    .SetDataSostituzioneNull()
                End If
                'Dati Contratto --------------------------------------------------------------
                If txtNumero.Text.Trim = "" Then txtNumero.Text = "0"
                If Not IsNumeric(txtNumero.Text.Trim) Then txtNumero.Text = "0"
                .Numero = txtNumero.Text.Trim
                If IsDate(txtDataInizio.Text) Then
                    .Data_Inizio = CDate(txtDataInizio.Text)
                Else
                    .SetData_InizioNull()
                End If
                If IsDate(txtDataFine.Text) Then
                    .Data_Fine = CDate(txtDataFine.Text)
                Else
                    .SetData_FineNull()
                End If
                .Importo = CDec(TxtImporto.Text.Trim)
                If DDLTipoContratto.SelectedIndex > 0 Then
                    .IDTipoContratto = DDLTipoContratto.SelectedValue
                Else
                    .SetIDTipoContrattoNull()
                End If
                'Dati Articolo -------------------------------------------------------------
                .Cod_Articolo = TxtArticoloCod.Text.Trim
                .Descrizione = TxtArticoloDesc.Text.Trim
                .NSerie = TxtArticoloSN.Text.Trim
                .Lotto = TxtArticoloLotto.Text.Trim
                .Attivo = CBool(rbtnAttivo.Checked)
                .InRiparazione = CBool(chkInRiparazione.Checked)
                .Sostituito = CBool(chkSostituito.Checked)
                'GIU050514 SCADENZE EL. BA. E INVIO EMAIL
                If IsDate(txtDataScadElettrodi.Text) Then
                    .DataScadElettrodi = CDate(txtDataScadElettrodi.Text)
                Else
                    .SetDataScadElettrodiNull()
                End If
                If chkEmailEl.Checked = True Then
                    If .IsData1InvioScadElNull Then
                        .Data1InvioScadEl = CDate(Now.Date)
                    End If
                Else
                    .SetData1InvioScadElNull()
                End If
                '-sim180518
                If chkEmailEl2.Checked = True Then
                    If .IsData2InvioScadElNull Then
                        .Data2InvioScadEl = CDate(Now.Date)
                    End If
                Else
                    .SetData2InvioScadElNull()
                End If
                '-
                If IsDate(txtDataScadBatterie.Text) Then
                    .DataScadBatterie = CDate(txtDataScadBatterie.Text)
                Else
                    .SetDataScadBatterieNull()
                End If
                If chkEmailBa.Checked = True Then
                    If .IsData1InvioScadBaNull Then
                        .Data1InvioScadBa = CDate(Now.Date)
                    End If
                Else
                    .SetData1InvioScadBaNull()
                End If
                'sim180518
                If chkEmailBa2.Checked = True Then
                    If .IsData2InvioScadBaNull Then
                        .Data2InvioScadBa = CDate(Now.Date)
                    End If
                Else
                    .SetData2InvioScadBaNull()
                End If
                '---------------------------------------
                'Dati Cliente --------------------------------------------------------------
                .Cod_Coge = txtCodCliForFilProvv.Text.Trim
                If DDLDestinazioni.SelectedIndex > 0 Then
                    .Cod_Filiale = DDLDestinazioni.SelectedValue
                Else
                    .SetCod_FilialeNull()
                End If
                .Destinazione1 = txtDestinazione1.Text.Trim
                .Destinazione2 = txtDestinazione2.Text.Trim
                .Destinazione3 = txtDestinazione3.Text.Trim
                .Riferimento = txtRiferimento.Text.Trim
                .NsRiferimento = txtNsRiferimento.Text.Trim
                '-
                .SetRefIntNull()
                '-------------------
                .Reparto = 0
                ' ''.DesRefInt = ""
                ' ''.NoteDocumento = txtNoteDocumento.Text.Trim
                .InseritoDa = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)

                .EMail = TxtEmail.Text.Trim 'simone210518
                .EndEdit()
            End With
            DsArtInst.ArticoliInstallati.AddArticoliInstallatiRow(newRowDocT)
            newRowDocT = Nothing
            Session("DsDocT") = DsArtInst
        Else 'MODIFICA
            dvDocT = Session("dvDocT")
            If IsDate(txtDataInstallazione.Text) Then
                dvDocT.Item(0).Item("Data_Installazione") = CDate(txtDataInstallazione.Text)
            Else
                dvDocT.Item(0).Item("Data_Installazione") = DBNull.Value
            End If
            If IsDate(txtDataScadGaranzia.Text) Then
                dvDocT.Item(0).Item("DataScadGaranzia") = CDate(txtDataScadGaranzia.Text)
            Else
                dvDocT.Item(0).Item("DataScadGaranzia") = DBNull.Value
            End If
            '-GIU050514
            If chkEmailGa.Checked = True Then
                If IsDBNull(dvDocT.Item(0).Item("Data1InvioScadGa")) Then
                    dvDocT.Item(0).Item("Data1InvioScadGa") = CDate(Now.Date)
                End If
            Else
                dvDocT.Item(0).Item("Data1InvioScadGa") = DBNull.Value
            End If
            '-sim180518
            If chkEmailGa2.Checked = True Then
                If IsDBNull(dvDocT.Item(0).Item("Data2InvioScadGa")) Then
                    dvDocT.Item(0).Item("Data2InvioScadGa") = CDate(Now.Date)
                End If
            Else
                dvDocT.Item(0).Item("Data2InvioScadGa") = DBNull.Value
            End If
            '---
            'la Scadenza del contratto viene aggiornato dai contratti
            If IsDate(txtDataSostituzione.Text) Then
                dvDocT.Item(0).Item("DataSostituzione") = CDate(txtDataSostituzione.Text)
            Else
                dvDocT.Item(0).Item("DataSostituzione") = DBNull.Value
            End If
            'Dati Contratto --------------------------------------------------------------
            If txtNumero.Text.Trim = "" Then txtNumero.Text = "0"
            If Not IsNumeric(txtNumero.Text.Trim) Then txtNumero.Text = "0"
            dvDocT.Item(0).Item("Numero") = txtNumero.Text.Trim
            If IsDate(txtDataInizio.Text) Then
                dvDocT.Item(0).Item("Data_Inizio") = CDate(txtDataInizio.Text)
            Else
                dvDocT.Item(0).Item("Data_Inizio") = DBNull.Value
            End If
            If IsDate(txtDataFine.Text) Then
                dvDocT.Item(0).Item("Data_Fine") = CDate(txtDataFine.Text)
            Else
                dvDocT.Item(0).Item("Data_Fine") = DBNull.Value
            End If
            dvDocT.Item(0).Item("Importo") = CDec(TxtImporto.Text.Trim)
            If DDLTipoContratto.SelectedIndex > 0 Then
                dvDocT.Item(0).Item("IDTipoContratto") = DDLTipoContratto.SelectedValue
            Else
                dvDocT.Item(0).Item("IDTipoContratto") = DBNull.Value
            End If
            'Dati Articolo--------------------------------------------------------------
            dvDocT.Item(0).Item("Cod_Articolo") = TxtArticoloCod.Text.Trim
            dvDocT.Item(0).Item("Descrizione") = TxtArticoloDesc.Text.Trim
            dvDocT.Item(0).Item("NSerie") = TxtArticoloSN.Text.Trim
            dvDocT.Item(0).Item("Lotto") = TxtArticoloLotto.Text.Trim
            dvDocT.Item(0).Item("Attivo") = CBool(rbtnAttivo.Checked)
            dvDocT.Item(0).Item("InRiparazione") = CBool(chkInRiparazione.Checked)
            dvDocT.Item(0).Item("Sostituito") = CBool(chkSostituito.Checked)
            'GIU050514 SCADENZE EL., BA E INVIO EMAIL
            If IsDate(txtDataScadElettrodi.Text) Then
                dvDocT.Item(0).Item("DataScadElettrodi") = CDate(txtDataScadElettrodi.Text)
            Else
                dvDocT.Item(0).Item("DataScadElettrodi") = DBNull.Value
            End If
            '-
            If chkEmailEl.Checked = True Then
                If IsDBNull(dvDocT.Item(0).Item("Data1InvioScadEl")) Then
                    dvDocT.Item(0).Item("Data1InvioScadEl") = CDate(Now.Date)
                End If
            Else
                dvDocT.Item(0).Item("Data1InvioScadEl") = DBNull.Value
            End If
            '-sim180518
            If chkEmailEl2.Checked = True Then
                If IsDBNull(dvDocT.Item(0).Item("Data2InvioScadEl")) Then
                    dvDocT.Item(0).Item("Data2InvioScadEl") = CDate(Now.Date)
                End If
            Else
                dvDocT.Item(0).Item("Data2InvioScadEl") = DBNull.Value
            End If
            '--
            If IsDate(txtDataScadBatterie.Text) Then
                dvDocT.Item(0).Item("DataScadBatterie") = CDate(txtDataScadBatterie.Text)
            Else
                dvDocT.Item(0).Item("DataScadBatterie") = DBNull.Value
            End If
            '-
            If chkEmailBa.Checked = True Then
                If IsDBNull(dvDocT.Item(0).Item("Data1InvioScadBa")) Then
                    dvDocT.Item(0).Item("Data1InvioScadBa") = CDate(Now.Date)
                End If
            Else
                dvDocT.Item(0).Item("Data1InvioScadBa") = DBNull.Value
            End If
            '-sim180518
            If chkEmailBa2.Checked = True Then
                If IsDBNull(dvDocT.Item(0).Item("Data2InvioScadBa")) Then
                    dvDocT.Item(0).Item("Data2InvioScadBa") = CDate(Now.Date)
                End If
            Else
                dvDocT.Item(0).Item("Data2InvioScadBa") = DBNull.Value
            End If
            'Dati Cliente --------------------------------------------------------------
            dvDocT.Item(0).Item("Cod_Coge") = txtCodCliForFilProvv.Text.Trim
            If DDLDestinazioni.SelectedIndex > 0 Then
                dvDocT.Item(0).Item("Cod_Filiale") = DDLDestinazioni.SelectedValue
            Else
                dvDocT.Item(0).Item("Cod_Filiale") = DBNull.Value
            End If
            dvDocT.Item(0).Item("Destinazione1") = txtDestinazione1.Text.Trim
            dvDocT.Item(0).Item("Destinazione2") = txtDestinazione2.Text.Trim
            dvDocT.Item(0).Item("Destinazione3") = txtDestinazione3.Text.Trim
            dvDocT.Item(0).Item("Riferimento") = txtRiferimento.Text.Trim
            dvDocT.Item(0).Item("NsRiferimento") = txtNsRiferimento.Text.Trim
            '-
            dvDocT.Item(0).Item("ModificatoDa") = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            dvDocT.Item(0).Item("EMail") = TxtEmail.Text.Trim   'simone210518
            DsArtInst = Session("DsDocT")
        End If

        SqlAdapDoc = Session("SqlAdapDoc")
        Try
            Me.SqlAdapDoc.Update(DsArtInst.ArticoliInstallati)
            If (dvDocT Is Nothing) Then
                dvDocT = New DataView(DsArtInst.ArticoliInstallati)
            End If
            If dvDocT.Count > 0 Then
                'giu280512 unificato 
                Try
                    TipoDoc = dvDocT.Item(0).Item("Tipo_Doc")
                    Session(CSTTIPODOC) = dvDocT.Item(0).Item("Tipo_Doc")
                    Session(IDARTICOLOINST) = dvDocT.Item(0).Item("ID")
                Catch ex As Exception
                    Session(IDARTICOLOINST) = ""
                    SWErr = True
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore aggiornamento testata", ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    strErrore = "Errore aggiornamento testata. " & ex.Message
                End Try
            Else
                Session(IDARTICOLOINST) = ""
                'giu280512 unificato Session(IDCONTRATTOASSISTENZA) = ""
                SWErr = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore aggiornamento testata", "Nessun dato è stato salvato", WUC_ModalPopup.TYPE_ERROR)
                strErrore = "Errore aggiornamento testata. Nessun dato è stato salvato"
            End If
            Session("dvDocT") = dvDocT
            Session("DsDocT") = DsArtInst
        Catch ExSQL As SqlException
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL in ArticoliInstallati.AggiornaDocT. ", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "Errore SQL in ArticoliInstallati.AggiornaDocT. " & ExSQL.Message
        Catch Ex As Exception
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ArticoliInstallati.AggiornaDocT. ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "Errore in ArticoliInstallati.AggiornaDocT. " & Ex.Message
        End Try

        'AGGIORNO EMAIL CLIENTE E EMAIL DESTINAZIONE CLIENTE-------SIMONE210518
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE Clienti SET EMail = '" & Controlla_Apice(TxtEmail.Text.Trim) & "', EmailInvioScad = '" & Controlla_Apice(txtEmailInvioScad.Text.Trim) & "' WHERE Codice_CoGe= '" & txtCodCliForFilProvv.Text.Trim & "'"
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftCoge, SQLStr) = False Then
                SWErr = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in ArticoliInstallati.AggiornaDocT. ", "", WUC_ModalPopup.TYPE_ERROR)
                strErrore = "Errore in ArticoliInstallati.AggiornaDocT. "
            End If
        Catch ex As Exception
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ArticoliInstallati.AggiornaDocT. ", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            strErrore = "Errore in ArticoliInstallati.AggiornaDocT. " & ex.Message
        End Try
        '-

        'UPDATE EMAIL DESTINAZIONE CLIENTE  (se presente)
        If DDLDestinazioni.SelectedIndex > 0 Then
            Try
                SQLStr = "UPDATE DestClienti SET EMail = '" & Controlla_Apice(txtEmailDest.Text.Trim) & "' WHERE Codice= '" & txtCodCliForFilProvv.Text.Trim & "' AND Progressivo = '" & DDLDestinazioni.SelectedValue.Trim & "'"
                If ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftCoge, SQLStr) = False Then
                    SWErr = True
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore in ArticoliInstallati.AggiornaDocT. ", "", WUC_ModalPopup.TYPE_ERROR)
                    strErrore = "Errore in ArticoliInstallati.AggiornaDocT. "
                End If
            Catch ex As Exception
                SWErr = True
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in ArticoliInstallati.AggiornaDocT. ", ex.Message, WUC_ModalPopup.TYPE_ERROR)
                strErrore = "Errore in ArticoliInstallati.AggiornaDocT. " & ex.Message
            End Try
        End If
        '------

        If SWErr = True Then
            AggiornaDocT = False
        Else
            Session(SWOP) = SWOPNESSUNA
        End If
    End Function

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO (ArticoliInstallati.btnNuovo_Click)")
            Exit Sub
        End If
        Session(SWOP) = SWOPNUOVO
        Try
            Session(IDDOCUMSAVE) = Session(IDARTICOLOINST)
        Catch ex As Exception
            Session(IDDOCUMSAVE) = ""
        End Try
        AzzeraTxtDocT()
        txtDataInstallazione.Text = Format(Now, FormatoData)
        Dim strErrore As String = ""
        If AggNuovaTestata(strErrore) = False Then
            Chiudi("Errore: Inserimento nuovo documento. " & strErrore)
            Exit Sub
        End If
        BtnSetEnabledTo(False)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        Session(SWMODIFICATO) = SWNO
    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO DOCUMENTO (ArticoliInstallati.btnElimina_Click)")
            Exit Sub
        End If
        Session(SWOP) = SWOPELIMINA
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        Session(MODALPOPUP_CALLBACK_METHOD) = "EliminaArticoloInstallato"
        ModalPopup.Show("Elimina scadenza articolo consumabile", "Confermi l'eliminazione ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub EliminaArticoloInstallato()
        Dim SWErr As Boolean = False

        dvDocT = Session("dvDocT")
        DsArtInst = Session("DsDocT")
        dvDocT.Item(0).Delete()
        SqlAdapDoc = Session("SqlAdapDoc")
        Try
            Me.SqlAdapDoc.Update(DsArtInst.ArticoliInstallati)
            If (dvDocT Is Nothing) Then
                dvDocT = New DataView(DsArtInst.ArticoliInstallati)
            End If
            'giu280512 unificato 
            Try
                If dvDocT.Count > 0 Then
                    TipoDoc = dvDocT.Item(0).Item("Tipo_Doc")
                    Session(CSTTIPODOC) = dvDocT.Item(0).Item("Tipo_Doc")
                    Session(IDARTICOLOINST) = dvDocT.Item(0).Item("ID")
                Else
                    Session(IDARTICOLOINST) = ""
                End If
            Catch ex As Exception
                Session(IDARTICOLOINST) = ""
            End Try
            Session("dvDocT") = dvDocT
            Session("DsDocT") = DsArtInst
        Catch ExSQL As SqlException
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL in ArticoliInstallati.EliminaArticoloInstallato", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
        Catch Ex As Exception
            SWErr = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in ArticoliInstallati.EliminaArticoloInstallato", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        Session(SWOP) = SWOPNESSUNA
        Chiudi("")
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Session(SWOP) = SWOPMODIFICA
        CampiSetEnabledToT(True)
        BtnSetEnabledTo(False)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        txtDataInstallazione.Focus()
    End Sub

    Private Sub DDLDestinazioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLDestinazioni.SelectedIndexChanged
        'giu060514 txtRiferimento non modificato perche' contiene il riferimento del documento DDT
        'GIU120514 CAMPI PRESI DAL CODICE CLIENTE E NON DESTINAZIONE 
        Session(CSTCODFILIALE) = IIf(IsNumeric(DDLDestinazioni.SelectedValue), DDLDestinazioni.SelectedValue, 0)
        If DDLDestinazioni.SelectedIndex = 0 Then
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = "" : txtEmailDest.Text = ""   'sim180518
            Session(CSTCODFILIALE) = ""
            Session(SWMODIFICATO) = SWSI
            Exit Sub
        End If
        Dim strCodFil As String = Session(CSTCODFILIALE)
        If IsNothing(strCodFil) Then
            strCodFil = ""
        End If
        If String.IsNullOrEmpty(strCodFil) Then
            strCodFil = ""
        End If
        If strCodFil = "" Or Not IsNumeric(strCodFil) Then
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = "" : txtEmailDest.Text = ""   'sim180518
            Session(CSTCODFILIALE) = ""
            Session(SWMODIFICATO) = SWSI
            Exit Sub
        End If
        '-
        Dim dvDest As DataView
        dvDest = SqlDSDestinazione.Select(DataSourceSelectArguments.Empty)
        If (dvDest Is Nothing) Then
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = "" : txtEmailDest.Text = ""   'sim180518
            'giu040612
            'GIU120514  txtRiferimento.Text = ""
            ' ''lblTelefono1.Text = "" : lblTelefono2.Text = "" : lblFax.Text = "" : lblEMail.Text = ""
            Session(CSTCODFILIALE) = ""
            Exit Sub
        End If
        If dvDest.Count > 0 Then
            dvDest.RowFilter = "Progressivo = " & Session(CSTCODFILIALE)
            If dvDest.Count > 0 Then
                txtDestinazione1.Text = dvDest.Item(0).Item("Ragione_Sociale").ToString
                txtDestinazione2.Text = dvDest.Item(0).Item("Indirizzo").ToString
                txtDestinazione3.Text = dvDest.Item(0).Item("CAP").ToString & " " & dvDest.Item(0).Item("Localita").ToString & " " & IIf(dvDest.Item(0).Item("Provincia").ToString.Trim <> "", "(" & dvDest.Item(0).Item("Provincia").ToString & ")", "")
                txtEmailDest.Text = dvDest.Item(0).Item("EMail").ToString   'simone180518
                'GIU120514 CAMPI PRESI DAL CODICE CLIENTE E NON DESTINAZIONE
                '' ''giu040612 
                ' ''txtRiferimento.Text = dvDest.Item(0).Item("Riferimento").ToString
                ' ''lblTelefono1.Text = dvDest.Item(0).Item("Telefono1").ToString
                ' ''lblTelefono2.Text = dvDest.Item(0).Item("Telefono2").ToString
                ' ''lblFax.Text = dvDest.Item(0).Item("Fax").ToString
                ' ''lblEMail.Text = dvDest.Item(0).Item("EMail").ToString
            Else
                txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = "" : txtEmailDest.Text = ""   'sim180518
                'GIU120514 CAMPI PRESI DAL CODICE CLIENTE E NON DESTINAZIONE
                '' ''giu040612
                ' ''txtRiferimento.Text = ""
                ' ''lblTelefono1.Text = "" : lblTelefono2.Text = "" : lblFax.Text = "" : lblEMail.Text = ""
                Session(CSTCODFILIALE) = ""
                Exit Sub
            End If
        Else
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = "" : txtEmailDest.Text = ""   'sim180518
            'GIU120514 CAMPI PRESI DAL CODICE CLIENTE E NON DESTINAZIONE
            '' ''giu040612
            ' ''txtRiferimento.Text = ""
            ' ''lblTelefono1.Text = "" : lblTelefono2.Text = "" : lblFax.Text = "" : lblEMail.Text = ""
            Session(CSTCODFILIALE) = ""
        End If
        Session(SWMODIFICATO) = SWSI
        txtRiferimento.Focus()
    End Sub

    Private Sub TxtArticoloCod_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtArticoloCod.TextChanged
        Session(SWMODIFICATO) = SWSI
        If TxtArticoloCod.Text.Trim <> "" Then
            Dim myCodArt As String = TxtArticoloCod.Text
            Dim myDesArt As String = ""
            If LeggiArticolo(myCodArt, myDesArt) = False Then
                TxtArticoloCod.BackColor = SEGNALA_KO
                TxtArticoloCod.Focus()
            ElseIf myDesArt.Trim = "" Then
                TxtArticoloCod.BackColor = SEGNALA_KO
                TxtArticoloCod.Focus()
            Else
                TxtArticoloCod.Text = myCodArt
                TxtArticoloCod.BackColor = SEGNALA_OK
                TxtArticoloDesc.Text = myDesArt.Trim
                TxtArticoloSN.Focus()
            End If
            Exit Sub
        End If
        TxtArticoloSN.Focus()
    End Sub
    Private Function LeggiArticolo(ByRef myCodArt As String, ByRef myDesArt As String) As Boolean
        If myCodArt.Trim = "" Then
            LeggiArticolo = True
            Exit Function
        End If
        Dim AMSys As New AnaMag
        Dim myAM As AnaMagEntity
        Dim arrAM As ArrayList
        Try
            arrAM = AMSys.getAnaMagByCodice(myCodArt.Trim)
            If (arrAM.Count > 0) Then
                myAM = CType(arrAM(0), AnaMagEntity)
                LeggiArticolo = True
                myCodArt = myAM.CodArticolo
                myDesArt = myAM.Descrizione
            Else
                LeggiArticolo = False
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in LeggiArticolo", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            LeggiArticolo = False
        End Try
    End Function
    Private Sub txtCodCliForFilProvv_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCliForFilProvv.TextChanged
        GetDatiAnagraficiCliente(0)
        Session(SWMODIFICATO) = SWSI
        If lblRagSoc.Text.Trim = "" Then
            txtCodCliForFilProvv.BackColor = SEGNALA_KO
            txtCodCliForFilProvv.Focus()
        Else
            txtCodCliForFilProvv.BackColor = SEGNALA_OK
            txtRiferimento.Focus()
        End If
    End Sub
    Private Sub chkSostituito_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSostituito.CheckedChanged
        If chkSostituito.Checked = True Then
            If txtDataSostituzione.Text.Trim = "" Then
                txtDataSostituzione.Text = Format(Now, FormatoData)
            End If
        Else
            txtDataSostituzione.Text = ""
        End If
    End Sub
    Protected Sub btnStampa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStampa.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDARTICOLOINST)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun dato è stato selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Session(CSTTipoStampaAICA) = TipoStampaAICA.SingoloAICA
        TipoDoc = SWTD(TD.ArticoloInstallato)
        Try
            DsPrinWebDoc.Clear()
            If ClsPrint.StampaAICA(Session(IDARTICOLOINST), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTNOBACK) = 0
                Response.Redirect("..\WebFormTables\WF_PrintWeb_AICA.aspx?labelForm=Scheda Articolo installato." & Session(IDARTICOLOINST).ToString.Trim)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

#Region "Gestione Anagrafiche"

    Private Sub btnModificaAnagrafica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaAnagrafica.Click
        If CKCSTTipoDoc(TipoDoc, TabCliFor) = False Then
            Chiudi("Errore: TIPO ARTICOLO INSTALLATO SCONOSCIUTO (ArticoliInstallati.btnModificaAnagrafica_Click)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        Session(TIPORK) = "C"

        If txtCodCliForFilProvv.Text.Trim = "" Then
            Exit Sub
        End If
        If Not IsNumeric(txtCodCliForFilProvv.Text.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non valido", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        Session(CSTTABCLIFOR) = TabCliFor
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
            Session(TIPORK) = "C"
            LblCliente.Text = "Cliente"
            Session(CSTTABCLIFOR) = "Cli"
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            TabCliFor = "For"
            Session(TIPORK) = "F"
            LblCliente.Text = "Fornitore"
            Session(CSTTABCLIFOR) = "For"
        End If
        SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = @Codice)"
        '----------
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
        Dim dvCliFor As DataView
        dvCliFor = SqlDSCliForFilProvv.Select(DataSourceSelectArguments.Empty)
        If (dvCliFor Is Nothing) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(IDANAGRCLIFOR) = txtCodCliForFilProvv.Text.Trim
        Dim Rk As StrAnagrCliFor
        If dvCliFor.Count > 0 Then
            Rk.Rag_Soc = dvCliFor.Item(0).Item("Rag_Soc").ToString
            Rk.Denominazione = dvCliFor.Item(0).Item("Denominazione").ToString
            Rk.Riferimento = dvCliFor.Item(0).Item("Riferimento").ToString
            Rk.Codice_Fiscale = dvCliFor.Item(0).Item("Codice_Fiscale").ToString
            Rk.Partita_IVA = dvCliFor.Item(0).Item("Partita_IVA").ToString
            Rk.Indirizzo = dvCliFor.Item(0).Item("Indirizzo").ToString + " " + dvCliFor.Item(0).Item("NumeroCivico").ToString
            Rk.NumeroCivico = "" 'dvCliFor.Item(0).Item("NumeroCivico").ToString
            Rk.Cap = dvCliFor.Item(0).Item("CAP").ToString
            Rk.Localita = dvCliFor.Item(0).Item("Localita").ToString
            Rk.Provincia = dvCliFor.Item(0).Item("Provincia").ToString.Trim
            Rk.Nazione = dvCliFor.Item(0).Item("Nazione").ToString.Trim
            Rk.Telefono1 = dvCliFor.Item(0).Item("Telefono1").ToString.Trim
            Rk.Telefono2 = dvCliFor.Item(0).Item("Telefono2").ToString.Trim
            Rk.Fax = dvCliFor.Item(0).Item("Fax").ToString.Trim
            Rk.Regime_Iva = dvCliFor.Item(0).Item("Regime_Iva").ToString.Trim
            Rk.EMail = dvCliFor.Item(0).Item("EMail").ToString.Trim 'giu070514
            Rk.EMailInvioScad = dvCliFor.Item(0).Item("EMailInvioScad").ToString.Trim 'sim210518
            Rk.InvioMailScad = dvCliFor.Item(0).Item("InvioMailScad")
            Rk.PECEMail = dvCliFor.Item(0).Item("PECEMail").ToString.Trim 'giu190122
            If TabCliFor = "Cli" Then
                Rk.IPA = dvCliFor.Item(0).Item("IPA").ToString.Trim
                Rk.SplitIVA = CBool(dvCliFor.Item(0).Item("SplitIVA"))
            End If
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-ok trovato
        'giu190618 BLOCCO PER LA DESTINAZIONE CLIENTE 
        Try
            Session(RKANAGRDESTCLI) = Nothing
            Dim myIDCOGE As String = txtCodCliForFilProvv.Text.Trim
            Dim Progressivo As String = Session(CSTCODFILIALE)
            If IsNumeric(Progressivo) = True And IsNumeric(myIDCOGE) = True And Progressivo <> "" And myIDCOGE <> "" Then
                SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [DestClienti] WHERE ([Codice] = '" & myIDCOGE & "') AND ([Progressivo] = " & Progressivo & ")"

                Dim Rk1 As New DestCliForEntity
                Dim dvDestCli As DataView
                dvDestCli = SqlDSCliForFilProvv.Select(DataSourceSelectArguments.Empty)

                If Not (dvDestCli Is Nothing) Then
                    If dvDestCli.Count > 0 Then
                        Rk1.Progressivo = dvDestCli.Item(0).Item("Progressivo").ToString
                        Rk1.Ragione_Sociale = dvDestCli.Item(0).Item("Ragione_Sociale").ToString
                        Rk1.Denominazione = dvDestCli.Item(0).Item("Denominazione").ToString
                        Rk1.Riferimento = dvDestCli.Item(0).Item("Riferimento").ToString
                        Rk1.Indirizzo = dvDestCli.Item(0).Item("Indirizzo").ToString
                        Rk1.Cap = dvDestCli.Item(0).Item("CAP").ToString
                        Rk1.Localita = dvDestCli.Item(0).Item("Localita").ToString
                        Rk1.Provincia = dvDestCli.Item(0).Item("Provincia").ToString.Trim
                        Rk1.Stato = dvDestCli.Item(0).Item("Stato").ToString.Trim
                        Rk1.Telefono1 = dvDestCli.Item(0).Item("Telefono1").ToString.Trim
                        Rk1.Telefono2 = dvDestCli.Item(0).Item("Telefono2").ToString.Trim
                        Rk1.Fax = dvDestCli.Item(0).Item("Fax").ToString.Trim
                        Rk1.EMail = dvDestCli.Item(0).Item("EMail").ToString.Trim

                        Session(RKANAGRDESTCLI) = Rk1
                    Else
                        'nessun dato trovato
                        Session(RKANAGRDESTCLI) = Nothing
                    End If
                Else
                    Session(RKANAGRDESTCLI) = Nothing
                End If
            Else
                Session(RKANAGRDESTCLI) = Nothing
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Filiale non trovata in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        '----
        '-----------
        Session(RKANAGRCLIFOR) = Rk
        WFP_Anagrafiche_Modify1.WucElement = Me
        WFP_Anagrafiche_Modify1.PopolaCampi()
        Session(F_ANAGRCLIFOR_APERTA) = True
        WFP_Anagrafiche_Modify1.Show()
    End Sub

#End Region

#Region "Ricerca Clienti/Fornitori"


    Private Sub btnCercaAnagrafica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaAnagrafica.Click
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO (ArticoliInstallati.btnCercaAnagrafica_Click)")
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If TabCliFor = "Cli" Then
            ApriElencoClienti()
        ElseIf TabCliFor = "For" Then
            ApriElencoFornitori()
        Else 'sessione scaduta
            If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
                SWCercaCli()
                Exit Sub
            ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
                SWCercaFor()
                Exit Sub
            End If
            ' ''Chiudi("Errore: TIPO TABELLA Clienti/Fornitori da ricercare sconosciuto")
            ' ''Exit Sub
            Session(MODALPOPUP_CALLBACK_METHOD) = "SWCercaCli"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = "SWCercaFor"
            ModalPopup.Show("Ricerca anagrafiche Clienti/Fornitori", "Scegli il tipo di ricerca ......", WUC_ModalPopup.TYPE_CONFIRM)
        End If
    End Sub
    Public Sub SWCercaCli()
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti()
    End Sub
    Public Sub SWCercaFor()
        Session(F_FOR_RICERCA) = True
        ApriElencoFornitori()
    End Sub
    
    Private Sub ApriElencoClienti()
        Session(F_CLI_RICERCA) = True
        Session(F_ELENCO_CLIFORN_APERTA) = True
        WFPElencoCli.Show(True)
    End Sub
    Private Sub ApriElencoFornitori()
        Session(F_FOR_RICERCA) = True
        Session(F_ELENCO_CLIFORN_APERTA) = True
        WFPElencoFor.Show(True)
    End Sub
    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        'giu191211
        Session(F_CLI_RICERCA) = False
        Session(F_FOR_RICERCA) = False
        '------
        txtCodCliForFilProvv.Text = codice
        lblRagSoc.Text = descrizione

        GetDatiAnagraficiCliente(0)

        Session(SWMODIFICATO) = SWSI
    End Sub
#End Region

#Region "Elenco Tabelle"
    Private Sub ApriElenco(ByVal finestra As String)
        Session(F_ELENCO_APERTA) = finestra
    End Sub
    Public Sub CallBackWFPElenco(ByVal codice As String, ByVal descrizione As String, ByVal finestra As String)

    End Sub
#End Region

#Region "Ricerca e scelta articoli"
    Private Sub BtnSelArticoloIns_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnSelArticoloIns.Click
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
        TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub
#End Region

    Private Sub GetDatiAnagraficiCliente(ByVal CodFil As Integer)
        'GIU120514 NON AZZERO TXTRiferimento se contiene già dei dati (RIF. ORD. CLIENTE)
        Session(IDLISTINO) = "1" 'default
        txtCodCliForFilProvv.ToolTip = ""
        lblRagSoc.ToolTip = ""
        '
        lblRagSoc.ForeColor = Drawing.Color.Black
        lblPICF.ForeColor = Drawing.Color.Black : lblIndirizzo.ForeColor = Drawing.Color.Black : lblLocalita.ForeColor = Drawing.Color.Black
        '---------
        If txtCodCliForFilProvv.Text.Trim = "" Then
            lblRagSoc.Text = ""
            lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
            'Provvisorie non hanno destinazioni
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
            DDLDestinazioni.Items.Clear()
            DDLDestinazioni.Items.Add("")
            DDLDestinazioni.DataBind()
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
            DDLDestinazioni.SelectedIndex = 0
            txtDestinazione1.Text = "" : txtDestinazione2.Text = "" : txtDestinazione3.Text = ""
            txtEmailDest.Text = ""  'simone180518
            '-
            'GIU120514 txtRiferimento.Text = ""
            lblTelefono1.Text = ""
            lblTelefono2.Text = ""
            lblFax.Text = ""
            TxtEmail.Text = ""  'simone180518
            txtEmailInvioScad.Text = ""  'simone180518
            Exit Sub
        End If
        Session(CSTTABCLIFOR) = TabCliFor
        If Left(txtCodCliForFilProvv.Text.Trim, 1) = "1" Then
            TabCliFor = "Cli"
            LblCliente.Text = "Cliente" : Session(CSTTABCLIFOR) = "Cli"
        ElseIf Left(txtCodCliForFilProvv.Text.Trim, 1) = "9" Then
            TabCliFor = "For"
            LblCliente.Text = "Fornitore" : Session(CSTTABCLIFOR) = "For"
        End If
        SqlDSCliForFilProvv.SelectCommand = "SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = @Codice)"
        '----------
        Session(CSTCODCOGE) = txtCodCliForFilProvv.Text.Trim
        BuildDest(CodFil)
        '--------
        Dim dvCliFor As DataView : Dim strInfo As String = ""
        dvCliFor = SqlDSCliForFilProvv.Select(DataSourceSelectArguments.Empty)
        If (dvCliFor Is Nothing) Then
            lblRagSoc.Text = ""
            lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
            'GIU120514 txtRiferimento.Text = ""
            lblTelefono1.Text = ""
            lblTelefono2.Text = ""
            lblFax.Text = ""
            TxtEmail.Text = ""
            txtEmailInvioScad.Text = ""
            Exit Sub
        End If
        If dvCliFor.Count > 0 Then
            If dvCliFor.Count > 0 Then
                If Not IsDBNull(dvCliFor.Item(0).Item("Listino")) Then
                    Session(IDLISTINO) = dvCliFor.Item(0).Item("Listino").ToString
                End If
                If Not IsDBNull(dvCliFor.Item(0).Item("NoFatt")) Then
                    If dvCliFor.Item(0).Item("NoFatt") = True Then
                        lblRagSoc.ForeColor = SEGNALA_KO
                        lblPICF.ForeColor = SEGNALA_KO : lblIndirizzo.ForeColor = SEGNALA_KO : lblLocalita.ForeColor = SEGNALA_KO
                    Else
                        lblRagSoc.ForeColor = Drawing.Color.Black
                        lblPICF.ForeColor = Drawing.Color.Black : lblIndirizzo.ForeColor = Drawing.Color.Black : lblLocalita.ForeColor = Drawing.Color.Black
                    End If
                Else
                    lblRagSoc.ForeColor = Drawing.Color.Black
                    lblPICF.ForeColor = Drawing.Color.Black : lblIndirizzo.ForeColor = Drawing.Color.Black : lblLocalita.ForeColor = Drawing.Color.Black
                End If
                '---------
                lblRagSoc.Text = dvCliFor.Item(0).Item("Rag_Soc").ToString
                strInfo += txtCodCliForFilProvv.Text.Trim & " "
                strInfo += dvCliFor.Item(0).Item("Rag_Soc").ToString & " "
                strInfo += dvCliFor.Item(0).Item("Denominazione").ToString & " "
                strInfo += dvCliFor.Item(0).Item("Riferimento").ToString & " "
                lblLabelPICF.Text = "P.IVA"
                lblPICF.Text = dvCliFor.Item(0).Item("Partita_IVA").ToString
                strInfo += "P.IVA: " & lblPICF.Text & " "
                If lblPICF.Text.Trim = "" Then
                    lblPICF.Text = dvCliFor.Item(0).Item("Codice_Fiscale").ToString
                    If lblPICF.Text.Trim <> "" Then
                        lblLabelPICF.Text = "C.Fis."
                        strInfo += "C.Fis.: " & lblPICF.Text & " "
                    End If
                Else
                    strInfo += "C.Fis.: " & dvCliFor.Item(0).Item("Codice_Fiscale").ToString & " "
                End If
                '-Indirizzo
                lblIndirizzo.Text = dvCliFor.Item(0).Item("Indirizzo").ToString & " " & dvCliFor.Item(0).Item("NumeroCivico").ToString
                strInfo += lblIndirizzo.Text & " "
                lblLocalita.Text = dvCliFor.Item(0).Item("CAP").ToString & " " & dvCliFor.Item(0).Item("Localita").ToString & " " & IIf(dvCliFor.Item(0).Item("Provincia").ToString.Trim <> "", "(" & dvCliFor.Item(0).Item("Provincia").ToString & ")", "")
                strInfo += lblLocalita.Text & " "
                'GIU120514 txtRiferimento.Text = dvCliFor.Item(0).Item("Riferimento").ToString
                lblTelefono1.Text = dvCliFor.Item(0).Item("Telefono1").ToString
                lblTelefono2.Text = dvCliFor.Item(0).Item("Telefono2").ToString
                lblFax.Text = dvCliFor.Item(0).Item("Fax").ToString
                TxtEmail.Text = dvCliFor.Item(0).Item("EMail").ToString
                txtEmailInvioScad.Text = dvCliFor.Item(0).Item("EmailInvioScad").ToString
            Else
                lblRagSoc.Text = ""
                lblPICF.Text = "" : lblIndirizzo.Text = "" : lblLocalita.Text = ""
                'GIU120514 txtRiferimento.Text = ""
                lblTelefono1.Text = ""
                lblTelefono2.Text = ""
                lblFax.Text = ""
                TxtEmail.Text = ""
                txtEmailInvioScad.Text = ""
            End If
            txtCodCliForFilProvv.ToolTip = strInfo
            lblRagSoc.ToolTip = strInfo
        End If
    End Sub

    
    Private Sub chkEmailBa_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEmailBa.CheckedChanged
        If chkEmailBa.Checked Then
            chkEmailBa2.Checked = False
        End If
    End Sub

    Private Sub chkEmailBa2_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEmailBa2.CheckedChanged
        If chkEmailBa2.Checked And chkEmailBa.Checked = False Then
            chkEmailBa2.Checked = False
        End If
    End Sub

    Private Sub chkEmailEl_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEmailEl.CheckedChanged
        If chkEmailEl.Checked Then
            chkEmailEl2.Checked = False
        End If
    End Sub

    Private Sub chkEmailEl2_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEmailEl2.CheckedChanged
        If chkEmailEl2.Checked And chkEmailEl.Checked = False Then
            chkEmailEl2.Checked = False
        End If
    End Sub

    Private Sub chkEmailGa_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEmailGa.CheckedChanged
        If chkEmailGa.Checked Then
            chkEmailGa2.Checked = False
        End If
    End Sub

    Private Sub chkEmailGa2_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEmailGa2.CheckedChanged
        If chkEmailGa2.Checked And chkEmailGa.Checked = False Then
            chkEmailGa2.Checked = False
        End If
    End Sub

    Private Sub TxtEmail_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TxtEmail.TextChanged
        Call SetLblEmailInvio()
    End Sub

    Private Sub txtEmailDest_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEmailDest.TextChanged
        Call SetLblEmailInvio()
    End Sub

    Private Sub txtEmailInvioScad_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEmailInvioScad.TextChanged
        Call SetLblEmailInvio()
    End Sub
End Class