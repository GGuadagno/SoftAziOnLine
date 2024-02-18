Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient

Partial Public Class WUC_GesMagazzini
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New It.SoftAzi.Model.Facade.dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSMagazzini.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSArtDiMag.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSArtIn.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSArtOu.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        If (Not IsPostBack) Then
            Try
                Session(IDMAGAZZINI) = 0
                ddlRicerca.Items.Add("Codice")
                ddlRicerca.Items(0).Value = "C"
                ddlRicerca.Items.Add("Descrizione")
                ddlRicerca.Items(1).Value = "D1"
                ddlRicerca.Items.Add("Descrizione (Parole contenute)")
                ddlRicerca.Items(2).Value = "D2"

                ddlRicercaArtIn.Items.Add("Codice")
                ddlRicercaArtIn.Items(0).Value = "C"
                ddlRicercaArtIn.Items.Add("Descrizione")
                ddlRicercaArtIn.Items(1).Value = "D1"
                ddlRicercaArtIn.Items.Add("Descrizione (Parole contenute)")
                ddlRicercaArtIn.Items(2).Value = "D2"

                DDLRicercaArtOu.Items.Add("Codice")
                DDLRicercaArtOu.Items(0).Value = "C"
                DDLRicercaArtOu.Items.Add("Descrizione")
                DDLRicercaArtOu.Items(1).Value = "D1"
                DDLRicercaArtOu.Items.Add("Descrizione (Parole contenute)")
                DDLRicercaArtOu.Items(2).Value = "D2"

                CampiSetEnabledTo(False)
                BtnSetEnabledTo(True)
                btnAggiorna.Enabled = False
                btnAnnulla.Enabled = False
                btnElimina.Visible = True : btnElimina.Enabled = False
                btnEliminaArt.Visible = False : btnStampaElenco.Visible = True
                btnIncludiAll.Visible = False : btnEscludiAll.Visible = False
                If GridViewMagazzini.Rows.Count > 0 Then
                    GridViewMagazzini.SelectedIndex = 0
                    Session(IDMAGAZZINI) = GridViewMagazzini.SelectedDataKey.Value
                    PopolaCampiMAG()
                Else
                    Session(IDMAGAZZINI) = 0
                    AzzeraCampiMAG()
                End If
                Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
                Session("TabMagazzini") = TB0
                TabContainer1.ActiveTabIndex = 0
            Catch Ex As Exception
                Chiudi("Errore: Caricamento Elenco Magazzini: " & Ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
        WUC_SceltaStampaUbiArt1.WucElement = Me
    End Sub
    Public Sub Chiudi(ByVal strErrore As String)
        Dim strRitorno As String = ""
        strRitorno = ""
        If strErrore.Trim = "" Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        Else
            strRitorno = "WF_Menu.aspx?labelForm=" & strErrore
        End If
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try
    End Sub
    Private Function CKIDMagazzini(ByRef IDMAG As Integer) As Boolean
        CKIDMagazzini = True
        '---- SESSIONE SCADUTA ???
        IDMAG = 0
        Dim myIDMAG As String = Session(IDMAGAZZINI).ToString.Trim
        If IsNothing(myIDMAG) Then myIDMAG = ""
        If String.IsNullOrEmpty(myIDMAG) Then myIDMAG = ""
        If Not IsNumeric(myIDMAG.Trim) Then myIDMAG = ""
        If myIDMAG = "" Then
            CKIDMagazzini = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("SESSIONE LAVORO SCADUTA", "Selezionare nuovamente il MAGAZZINO su cui operare.", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Function
        End If
        IDMAG = myIDMAG
        '-----------
    End Function
#Region "Gestione Grid"

    Protected Sub GridViewMagazzini_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewMagazzini.SelectedIndexChanged
        Session(IDMAGAZZINI) = GridViewMagazzini.SelectedDataKey.Value
        PopolaCampiMAG()
    End Sub

    Private Sub GridViewArtMag_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewArtMag.SelectedIndexChanged
        Dim row As GridViewRow = GridViewArtMag.SelectedRow
        lblCodArticolo.Text = row.Cells(1).Text.Trim
        lblDescrizione.Text = row.Cells(2).Text.Trim
    End Sub

    Private Sub GridViewArtIn_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewArtIn.SelectedIndexChanged
        Dim row As GridViewRow = GridViewArtIn.Rows(GridViewArtIn.SelectedIndex)
        Dim strDesArt As String = row.Cells(2).Text
        Session(MODALPOPUP_CALLBACK_METHOD) = "ConfEliminaADMIn"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Attenzione", "Confermi l'esclusione dell'articolo ? " & vbCrLf & strDesArt.Trim, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub

    Private Sub GridViewArtOu_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewArtOu.SelectedIndexChanged
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        Try
            SqlDSArtOu.InsertParameters.Item("CodMag").DefaultValue = IDMAG
            SqlDSArtOu.InsertParameters.Item("CodArt").DefaultValue = GridViewArtOu.SelectedDataKey.Value.ToString.Trim
            SqlDSArtOu.Insert()
            SqlDSArtOu.DataBind()
            SqlDSArtIn.DataBind()
            GridViewArtOu.DataBind()
            GridViewArtIn.DataBind()
            SqlDSArtDiMag.DataBind()
            GridViewArtMag.DataBind()
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewArtIn.Enabled = True
        GridViewArtOu.Enabled = True
        If GridViewArtIn.Rows.Count > 0 Then
            ' ''GridViewArtIn.SelectedIndex = 0
            btnEscludiAll.Enabled = True
            btnStampaElenco.Enabled = True
        Else
            btnEscludiAll.Enabled = False
            btnStampaElenco.Enabled = False
        End If
        If GridViewArtOu.Rows.Count > 0 Then
            ' ''GridViewArtOu.SelectedIndex = 0
            btnIncludiAll.Enabled = True
        Else
            btnIncludiAll.Enabled = False
        End If
    End Sub
#End Region

#Region "Gestione BTN"

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Session(SWOP) = SWOPMODIFICA : Session(SWMODIFICATO) = SWNO
        GridViewMagazzini.Enabled = False
        CampiSetEnabledTo(True)
        BtnSetEnabledTo(False)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True

    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click

        If Session(SWMODIFICATO) = SWSI Then
            Session(MODALPOPUP_CALLBACK_METHOD) = "ConfAnnullaModMag"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Modifiche Magazzino", "Si vuole annullare le modifiche?", WUC_ModalPopup.TYPE_CONFIRM)
        Else
            ConfAnnullaModMag()
        End If
    End Sub
    Public Sub ConfAnnullaModMag()
        GridViewMagazzini.Enabled = True
        CampiSetEnabledTo(False)
        BtnSetEnabledTo(True)
        btnAggiorna.Enabled = False
        btnAnnulla.Enabled = False
        If GridViewMagazzini.Rows.Count > 0 Then
            GridViewMagazzini.SelectedIndex = 0
            Session(IDMAGAZZINI) = GridViewMagazzini.SelectedDataKey.Value
            If Session(IDMAGAZZINI) = 0 Then
                btnElimina.Enabled = False
            End If
            PopolaCampiMAG()
        Else
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            AzzeraCampiMAG()
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub

    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        If ControllaCampi() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Controllo dati Magazzino", "Attenzione, i campi segnalati in rosso non sono validi", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Try
            ' ''-InsertUpdate è uguale
            If Session(SWOP) = SWOPMODIFICA Then
                SqlDSMagazzini.UpdateParameters.Item("Codice").DefaultValue = txtCodice.Text.Trim
                SqlDSMagazzini.UpdateParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
                SqlDSMagazzini.Update()
                SqlDSMagazzini.DataBind()
            Else
                SqlDSMagazzini.InsertParameters.Item("Codice").DefaultValue = txtCodice.Text.Trim
                SqlDSMagazzini.InsertParameters.Item("Descrizione").DefaultValue = Mid(txtDescrizione.Text.Trim, 1, 50)
                SqlDSMagazzini.Insert()
                SqlDSMagazzini.DataBind()
            End If
            Session(IDMAGAZZINI) = txtCodice.Text.Trim
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '-
        GridViewMagazzini.Enabled = True
        GridViewMagazzini.DataBind()
        If GridViewMagazzini.Rows.Count > 0 Then
            ' ''GridViewMagazzini.SelectedIndex = 0
            GridViewMagazzini_SelectedIndexChanged(GridViewMagazzini, Nothing)
            PopolaCampiMAG()
            CampiSetEnabledTo(False)
            BtnSetEnabledTo(True)
            If Session(IDMAGAZZINI).ToString.Trim = "0" Then
                btnElimina.Enabled = False
            End If
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
        Else
            AzzeraCampiMAG()
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub

    Private Sub btnIncludiAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnIncludiAll.Click
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        If GridViewArtOu.Rows.Count < 1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun articolo risulta escluso in questo Magazzino.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "InAllArtDiMag"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show(btnIncludiAll.Text, "Includere tutti gli articoli nel MAGAZZINO ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Private Sub btnEscludiAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEscludiAll.Click
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        If GridViewArtIn.Rows.Count < 1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun articolo risulta inclusi in questo Magazzino.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "OuAllArtDiMag"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show(btnIncludiAll.Text, "Ecludere tutti gli articoli dal MAGAZZINO ?", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Function InAllArtDiMag() As Boolean
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Function
        '-----------
        btnEliminaArt.Enabled = False : btnIncludiAll.Enabled = False : btnEscludiAll.Enabled = False
        GridViewArtIn.Enabled = False : GridViewArtOu.Enabled = False
        Dim strErrore As String = ""
        If OKInOuAllArtDiMag("I", IDMAG, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Operazione di Includi tutti gli articoli fallita.<br>" & strErrore.Trim, WUC_ModalPopup.TYPE_INFO)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Includi tutti", "Operazione di Includi tutti gli articoli riuscita con successo", WUC_ModalPopup.TYPE_INFO)
        End If

        SqlDSArtOu.DataBind()
        SqlDSArtIn.DataBind()
        GridViewArtOu.DataBind()
        GridViewArtIn.DataBind()
        SqlDSArtDiMag.DataBind()
        GridViewArtMag.DataBind()
        GridViewArtIn.Enabled = True
        GridViewArtOu.Enabled = True
        If GridViewArtIn.Rows.Count > 0 Then
            GridViewArtIn.SelectedIndex = 0
            btnEscludiAll.Enabled = True
            btnStampaElenco.Enabled = True
        Else
            btnEscludiAll.Enabled = False
            btnStampaElenco.Enabled = False
        End If
        If GridViewArtOu.Rows.Count > 0 Then
            GridViewArtOu.SelectedIndex = 0
            btnIncludiAll.Enabled = True
        Else
            btnIncludiAll.Enabled = False
        End If
    End Function
    Public Function OuAllArtDiMag() As Boolean
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Function
        '-----------
        btnEliminaArt.Enabled = False : btnIncludiAll.Enabled = False : btnEscludiAll.Enabled = False
        GridViewArtIn.Enabled = False : GridViewArtOu.Enabled = False
        Dim strErrore As String = ""
        If OKInOuAllArtDiMag("E", IDMAG, strErrore) = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Operazione di Escludi tutti gli articoli fallita.<br>" & strErrore.Trim, WUC_ModalPopup.TYPE_INFO)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Includi tutti", "Operazione di Escludi tutti gli articoli riuscita con successo", WUC_ModalPopup.TYPE_INFO)
        End If

        SqlDSArtOu.DataBind()
        SqlDSArtIn.DataBind()
        GridViewArtOu.DataBind()
        GridViewArtIn.DataBind()
        SqlDSArtDiMag.DataBind()
        GridViewArtMag.DataBind()
        GridViewArtIn.Enabled = True
        GridViewArtOu.Enabled = True
        If GridViewArtIn.Rows.Count > 0 Then
            GridViewArtIn.SelectedIndex = 0
            btnEscludiAll.Enabled = True
            btnStampaElenco.Enabled = True
        Else
            btnEscludiAll.Enabled = False
            btnStampaElenco.Enabled = False
        End If
        If GridViewArtOu.Rows.Count > 0 Then
            GridViewArtOu.SelectedIndex = 0
            btnIncludiAll.Enabled = True
        Else
            btnIncludiAll.Enabled = False
        End If
    End Function
    Private Function OKInOuAllArtDiMag(ByVal _InOu As String, ByVal _CMag As Integer, ByRef strErrore As String) As Boolean

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn = New System.Data.SqlClient.SqlConnection
        '-
        'Dim SqlSel_InOuAllArtDiMag = New System.Data.SqlClient.SqlCommand
        'Dim SqlIns_InOuAllArtDiMag = New System.Data.SqlClient.SqlCommand
        Dim SqlUpd_InOuAllArtDiMag = New System.Data.SqlClient.SqlCommand
        'Dim SqlDel_InOuAllArtDiMag = New System.Data.SqlClient.SqlCommand
        '
        'SqlConnection1
        '
        SqlConn.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '
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
        '---------------------------
        Try
            If SqlConn.State <> ConnectionState.Open Then
                SqlConn.Open()
            End If
            '--- Parametri
            SqlUpd_InOuAllArtDiMag.CommandText = "InOuAllArtDiMag"
            SqlUpd_InOuAllArtDiMag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_InOuAllArtDiMag.Connection = SqlConn
            SqlUpd_InOuAllArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_InOuAllArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodMag", System.Data.SqlDbType.Int, 4, "CodMag"))
            SqlUpd_InOuAllArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InOu", System.Data.SqlDbType.VarChar, 1, "InOu"))
            '
            SqlUpd_InOuAllArtDiMag.Parameters("@CodMag").Value = _CMag
            SqlUpd_InOuAllArtDiMag.Parameters("@InOu").Value = _InOu
            SqlUpd_InOuAllArtDiMag.CommandTimeout = myTimeOUT '5000
            SqlUpd_InOuAllArtDiMag.ExecuteNonQuery()

            If SqlConn.State <> ConnectionState.Closed Then
                SqlConn.Close()
            End If

            Return True
        Catch exSQL As SqlException
            strErrore = exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        Finally
            If Not IsNothing(SqlConn) Then
                If SqlConn.State <> ConnectionState.Closed Then
                    SqlConn.Close()
                    SqlConn = Nothing
                End If
            End If
        End Try
    End Function

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        If GridViewMagazzini.SelectedDataKey.Value.ToString.Trim = "0" Or IDMAG = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Impossibile cancellare il Magazzino principale", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        Session(MODALPOPUP_CALLBACK_METHOD) = "ConfEliminaMAG"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Attenzione", "Confermi l'eliminazione del Magazzino selezionato ? Attenzione saranno cancellati anche gli articoli collegati.", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub ConfEliminaMAG()
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        Try
            SqlDSMagazzini.DeleteParameters.Item("Codice").DefaultValue = IDMAG
            SqlDSMagazzini.Delete()
            SqlDSMagazzini.DataBind()
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewMagazzini.Enabled = True
        GridViewMagazzini.DataBind()
        If GridViewMagazzini.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            '-
            GridViewMagazzini.SelectedIndex = 0
            Session(IDMAGAZZINI) = GridViewMagazzini.SelectedDataKey.Value
            PopolaCampiMAG()
        Else
            AzzeraCampiMAG()
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
        End If
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
    End Sub
    Private Sub btnEliminaArt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEliminaArt.Click
        If lblCodArticolo.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Selezionare prima l'articolo.", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        ElseIf GridViewArtMag.Rows.Count > 0 Then
            Dim row As GridViewRow = GridViewArtMag.SelectedRow
            lblCodArticolo.Text = row.Cells(1).Text.Trim
            lblDescrizione.Text = row.Cells(2).Text.Trim
        Else
            lblCodArticolo.Text = ""
            lblDescrizione.Text = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun articolo da escludere.", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "ConfEliminaADM"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Attenzione", "Confermi l'esclusione dell'articolo selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub ConfEliminaADM()
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        If lblCodArticolo.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Selezionare prima l'articolo.", WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        End If
        Try
            SqlDSArtDiMag.DeleteParameters.Item("Original_Codice_Magazzino").DefaultValue = IDMAG
            SqlDSArtDiMag.DeleteParameters.Item("Original_Cod_Articolo").DefaultValue = lblCodArticolo.Text.Trim
            SqlDSArtDiMag.Delete()
            SqlDSArtDiMag.DataBind()
            GridViewArtMag.DataBind()
            'GIU020920
            SqlDSArtIn.DataBind()
            GridViewArtIn.DataBind()
            SqlDSArtOu.DataBind()
            GridViewArtOu.DataBind()
            '
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewArtMag.Enabled = True
        If GridViewArtMag.Rows.Count > 0 Then
            btnEliminaArt.Enabled = True
            btnStampaElenco.Enabled = True
            '-
            GridViewArtMag.SelectedIndex = 0
            Dim row As GridViewRow = GridViewArtMag.SelectedRow
            lblCodArticolo.Text = row.Cells(1).Text.Trim
            lblDescrizione.Text = row.Cells(2).Text.Trim
        Else
            lblCodArticolo.Text = ""
            lblDescrizione.Text = ""
            btnEliminaArt.Enabled = False
            btnStampaElenco.Enabled = False
        End If
    End Sub
    Public Sub ConfEliminaADMIn()
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        Try
            SqlDSArtIn.DeleteParameters.Item("Original_Codice_Magazzino").DefaultValue = IDMAG
            SqlDSArtIn.DeleteParameters.Item("Original_Cod_Articolo").DefaultValue = GridViewArtIn.SelectedDataKey.Value.ToString.Trim
            SqlDSArtIn.Delete()
            SqlDSArtIn.DataBind()
            GridViewArtIn.DataBind()
            SqlDSArtOu.DataBind()
            GridViewArtOu.DataBind()
            '
            SqlDSArtDiMag.DataBind()
            GridViewArtMag.DataBind()
        Catch
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
        End Try

        GridViewArtIn.Enabled = True
        GridViewArtOu.Enabled = True
        If GridViewArtMag.Rows.Count > 0 Then
            GridViewArtMag.SelectedIndex = 0
            btnEliminaArt.Enabled = True
            btnStampaElenco.Enabled = True
        Else
            btnEliminaArt.Enabled = False
            btnStampaElenco.Enabled = False
        End If
        If GridViewArtIn.Rows.Count > 0 Then
            GridViewArtIn.SelectedIndex = 0
            btnEscludiAll.Enabled = True
        Else
            btnEscludiAll.Enabled = False
        End If
        If GridViewArtOu.Rows.Count > 0 Then
            GridViewArtOu.SelectedIndex = 0
            btnIncludiAll.Enabled = True
        Else
            btnIncludiAll.Enabled = False
        End If

    End Sub
    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        Session(SWOP) = SWOPNUOVO : Session(SWMODIFICATO) = SWNO

        GridViewMagazzini.Enabled = False
        CampiSetEnabledTo(True)
        BtnSetEnabledTo(False)
        btnAggiorna.Enabled = True
        btnAnnulla.Enabled = True
        AzzeraCampiMAG()
        txtCodice.Focus()
    End Sub

    Private Sub btnStampaElenco_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaElenco.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        WUC_SceltaStampaUbiArt1.Show("Stampa articoli<br>Magazzino:" + txtDescrizione.Text.Trim.ToUpper.Replace("MAGAZZINO", ""))
    End Sub
    Public Sub CallBackSceltaStampaUbiMag(ByVal Ordinamento As String)
        Dim dsAnaMag1 As New DSAnaMag
        Dim ObjReport As New Object
        Dim ClsPrint As New Listini
        Dim StrErrore As String = ""
        Dim strTmpTitoloRpt As String

        Dim SWSconti As Boolean = False


        Try
            Session(CSTTIPORPTMAG) = TIPOSTAMPALISTINO.ArticoliUbicazioneGM
            strTmpTitoloRpt = "Riepilogo ubicazione articoli"
            Dim strNomeAz As String = ""

            strNomeAz = Session(CSTAZIENDARPT).ToString.Trim

            Select Case Ordinamento
                Case "Cod_Articolo"
                    strTmpTitoloRpt = strTmpTitoloRpt & " ordinato per codice articolo"
                Case "Descrizione"
                    strTmpTitoloRpt = strTmpTitoloRpt & " ordinato per descrizione articolo"
                Case "Reparto, Scaffale, Piano, Descrizione"
                    strTmpTitoloRpt = strTmpTitoloRpt & " ordinato per reparto, scaffale e piano"
            End Select

            strTmpTitoloRpt += " - Magazzino: " + txtDescrizione.Text.ToUpper.Trim.Replace("MAGAZZINO", "")

            If ClsPrint.StampaUbiMag(strNomeAz, strTmpTitoloRpt, dsAnaMag1, ObjReport, StrErrore, Ordinamento, txtCodice.Text.Trim) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = dsAnaMag1
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebCR_Mag.aspx")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in GesMagazzini.btnStampaElenco", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------
    End Sub
#End Region

#Region "Gestione Controlli"
    Private Sub TabContainer1_ActiveTabChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabContainer1.ActiveTabChanged
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        If Session(SWOP) = SWOPNUOVO Or Session(SWOP) = SWOPMODIFICA Then
            TabContainer1.ActiveTabIndex = Int(Session("TabMagazzini"))
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Completare prima l'operazione in corso: " & Session(SWOP), WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session("TabMagazzini") = TabContainer1.ActiveTabIndex
        If TabContainer1.ActiveTabIndex = TB0 Then
            btnNuovo.Visible = True : btnNuovo.Enabled = True
            btnModifica.Visible = True
            btnAggiorna.Visible = True
            btnAnnulla.Visible = True
            If IDMAG <> 0 Then
                btnElimina.Visible = True
            Else
                btnElimina.Visible = False
            End If
            If GridViewMagazzini.Rows.Count > 0 Then
                btnModifica.Enabled = True
                btnAggiorna.Enabled = False
                btnAnnulla.Enabled = False
                If IDMAG <> 0 Then
                    btnElimina.Enabled = True
                Else
                    btnElimina.Enabled = False
                End If
            Else
                btnModifica.Enabled = False
                btnAggiorna.Enabled = False
                btnAnnulla.Enabled = False
                If IDMAG <> "0" Then
                    btnElimina.Enabled = False
                Else
                    btnElimina.Enabled = False
                End If
            End If
            If GridViewArtMag.Rows.Count > 0 Then
                btnStampaElenco.Enabled = True
            Else
                btnStampaElenco.Enabled = False
            End If
            btnEliminaArt.Visible = False
            lblDescIncl.Visible = False 'alb080213
            btnIncludiAll.Visible = False : btnEscludiAll.Visible = False
        ElseIf TabContainer1.ActiveTabIndex = TB1 Then
            btnNuovo.Visible = False
            btnModifica.Visible = False
            btnAggiorna.Visible = False
            btnAnnulla.Visible = False
            'giu290719
            btnModifica.Enabled = False
            btnAggiorna.Enabled = False
            btnAnnulla.Enabled = False
            '---------
            btnElimina.Visible = False
            btnEliminaArt.Visible = True
            btnIncludiAll.Visible = False : btnEscludiAll.Visible = False
            lblDescIncl.Visible = False 'alb080213
            If GridViewArtMag.Rows.Count > 0 Then
                btnEliminaArt.Enabled = True
                btnStampaElenco.Enabled = True
            Else
                lblCodArticolo.Text = ""
                lblDescrizione.Text = ""
                btnEliminaArt.Enabled = False
                btnStampaElenco.Enabled = False
            End If
        ElseIf TabContainer1.ActiveTabIndex = TB2 Then
            btnNuovo.Visible = False
            btnModifica.Visible = False
            btnAggiorna.Visible = False
            btnAnnulla.Visible = False
            btnElimina.Visible = False : btnEliminaArt.Visible = False : btnEliminaArt.Enabled = False
            btnIncludiAll.Visible = True : btnEscludiAll.Visible = True
            If GridViewArtIn.Rows.Count > 0 Then
                btnEscludiAll.Enabled = True
                btnStampaElenco.Enabled = True
            Else
                btnEscludiAll.Enabled = False
                btnStampaElenco.Enabled = False
            End If
            If GridViewArtOu.Rows.Count > 0 Then
                btnIncludiAll.Enabled = True
            Else
                btnIncludiAll.Enabled = False
            End If
            lblDescIncl.Visible = True 
        End If

    End Sub


    Private Sub txtCodice_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodice.TextChanged
        If Session(SWOP) <> SWOPNESSUNA Then
            Session(SWMODIFICATO) = SWSI
            If Not IsNumeric(txtCodice.Text.Trim) Then
                txtCodice.BackColor = SEGNALA_KO
                txtCodice.Focus()
                Exit Sub
            End If
        End If
        If CheckNewCodMAGOnTab() = False Then
            txtCodice.Focus()
            Exit Sub
        End If
        txtDescrizione.Focus()
    End Sub

    Private Sub txtDescrizione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDescrizione.TextChanged
        If Session(SWOP) <> SWOPNESSUNA Then
            Session(SWMODIFICATO) = SWSI
            If txtDescrizione.Text.Trim = "" Then
                txtDescrizione.BackColor = SEGNALA_KO
                txtDescrizione.Focus()
                Exit Sub
            End If
        End If
        If CheckNewDesMAGOnTab() = False Then
            txtDescrizione.Focus()
            Exit Sub
        Else
            txtDescrizione.BackColor = SEGNALA_OK
        End If
        btnAggiorna.Focus()
    End Sub

#End Region

#Region "Funzioni e procedure"
    Private Sub PopolaCampiMAG()
        If GridViewMagazzini.Rows.Count > 0 Then
            Dim row As GridViewRow = GridViewMagazzini.SelectedRow
            txtCodice.Text = row.Cells(1).Text.Trim
            txtDescrizione.Text = row.Cells(2).Text.Trim
            TabPanel1.HeaderText = "MAGAZZINO selezionato: (" & txtCodice.Text.Trim & ") - " & txtDescrizione.Text.Trim.Trim.Replace("MAGAZZINO", "")
            TabPanel2.HeaderText = "Articoli presenti nel MAGAZZINO: (" & txtCodice.Text.Trim & ") - " & txtDescrizione.Text.Trim.Trim.Replace("MAGAZZINO", "")
            TabPanel3.HeaderText = "Includi/Escludi articoli dal MAGAZZINO: (" & txtCodice.Text.Trim & ") - " & txtDescrizione.Text.Trim.Trim.Replace("MAGAZZINO", "")
            If txtCodice.Text.Trim <> "0" Then
                btnElimina.Visible = True : btnElimina.Enabled = True
            Else
                btnElimina.Visible = False : btnElimina.Enabled = False
            End If
            SqlDSArtDiMag.DataBind()
            GridViewArtMag.DataBind()
            If GridViewArtMag.Rows.Count > 0 Then
                GridViewArtMag.SelectedIndex = 0
                Dim rowADM As GridViewRow = GridViewArtMag.SelectedRow
                lblCodArticolo.Text = rowADM.Cells(1).Text.Trim
                lblDescrizione.Text = rowADM.Cells(2).Text.Trim
                btnEliminaArt.Enabled = True
                btnStampaElenco.Enabled = True
            Else
                lblCodArticolo.Text = ""
                lblDescrizione.Text = ""
                btnEliminaArt.Enabled = False
                btnStampaElenco.Enabled = False
            End If
        Else
            txtCodice.Text = ""
            txtDescrizione.Text = ""
            btnElimina.Visible = True : btnElimina.Enabled = False
            TabPanel1.HeaderText = "MAGAZZINO selezionato: (????)"
            TabPanel2.HeaderText = "Articoli presenti nel MAGAZZINO: (????)"
            TabPanel3.HeaderText = "Includi/Escludi articoli dal MAGAZZINO: (????)"
            AzzeraCampiMAG()
            lblCodArticolo.Text = ""
            lblDescrizione.Text = ""
            btnEliminaArt.Enabled = False
            btnStampaElenco.Enabled = False
        End If
        Session(SWMODIFICATO) = SWNO
    End Sub
    Private Sub AzzeraCampiMAG()
        SfondoCampiMAG()
        txtCodice.Text = "" : txtCodice.ToolTip = ""
        txtDescrizione.Text = "" : txtDescrizione.ToolTip = ""
        Session(SWMODIFICATO) = SWNO
    End Sub
    Private Sub SfondoCampiMAG()
        txtCodice.BackColor = SEGNALA_OK
        txtDescrizione.BackColor = SEGNALA_OK
    End Sub

    Private Sub CampiSetEnabledTo(ByVal Valore As Boolean)
        If Session(SWOP) = SWOPMODIFICA Then
            txtCodice.Enabled = False
        Else
            txtCodice.Enabled = Valore
        End If
        txtDescrizione.Enabled = Valore

    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        ddlRicerca.Enabled = Valore : txtRicerca.Enabled = Valore : btnRicercaArticolo.Enabled = Valore
        btnAggiorna.Enabled = Valore
        btnAnnulla.Enabled = Valore
        btnElimina.Enabled = Valore : btnEliminaArt.Enabled = Valore
        btnModifica.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnStampaElenco.Enabled = Valore
        btnIncludiAll.Enabled = Valore : btnEscludiAll.Enabled = Valore
    End Sub

    Private Function ControllaCampi() As Boolean
        ControllaCampi = True : SfondoCampiMAG()
        If Not IsNumeric(txtCodice.Text.Trim) Then
            txtCodice.BackColor = SEGNALA_KO : ControllaCampi = False
        Else
            If CheckNewCodMAGOnTab() = False Then
                txtCodice.BackColor = SEGNALA_KO : ControllaCampi = False
            Else
                txtCodice.BackColor = SEGNALA_OK
            End If
        End If
        If txtDescrizione.Text.Trim = "" Then
            txtDescrizione.BackColor = SEGNALA_KO : ControllaCampi = False
        ElseIf CheckNewDesMAGOnTab() = False Then
            txtDescrizione.BackColor = SEGNALA_KO : ControllaCampi = False
        Else
            txtDescrizione.BackColor = SEGNALA_OK
        End If
    End Function
    Private Function CheckNewCodMAGOnTab() As Boolean
        CheckNewCodMAGOnTab = True
        If Session(SWOP) <> SWOPNUOVO Then
            Exit Function
        End If
        If txtCodice.Text.Trim <> "" And IsNumeric(txtCodice.Text.Trim) Then
            Dim Codice As Integer = Int(txtCodice.Text.Trim)
            Try
                Dim dvMagazzini As DataView
                dvMagazzini = SqlDSMagazzini.Select(DataSourceSelectArguments.Empty)
                If (dvMagazzini Is Nothing) Then
                    Exit Function
                End If
                If dvMagazzini.Count > 0 Then
                    dvMagazzini.RowFilter = "Codice = " & Codice.ToString.Trim & ""
                    If dvMagazzini.Count > 0 Then
                        CheckNewCodMAGOnTab = False
                        txtCodice.ToolTip = "Codice già presente in tabella."
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Verifica nuovo codice", "Attenzione, codice già presente in tabella.", WUC_ModalPopup.TYPE_ERROR)
                        Exit Function
                    End If
                End If
            Catch ex As Exception
                CheckNewCodMAGOnTab = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in lettura dati Magazzini (CheckNewCodMAGOnTab)", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End Try
        Else
            CheckNewCodMAGOnTab = False
        End If
    End Function
    Private Function CheckNewDesMAGOnTab() As Boolean
        CheckNewDesMAGOnTab = True
        If Session(SWOP) <> SWOPNUOVO Then
            Exit Function
        End If
        If txtDescrizione.Text.Trim <> "" Then
            Dim Codice As Integer = Int(txtCodice.Text.Trim)
            Try
                Dim dvMagazzini As DataView
                dvMagazzini = SqlDSMagazzini.Select(DataSourceSelectArguments.Empty)
                If (dvMagazzini Is Nothing) Then
                    Exit Function
                End If
                If dvMagazzini.Count > 0 Then
                    dvMagazzini.RowFilter = "Codice<>" + Codice.ToString.Trim + " AND Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
                    If dvMagazzini.Count > 0 Then
                        CheckNewDesMAGOnTab = False
                        txtCodice.ToolTip = "Descrizione già presente in tabella."
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Verifica descrizione", "Attenzione, descrizione già presente in tabella.", WUC_ModalPopup.TYPE_ERROR)
                        Exit Function
                    End If
                End If
            Catch ex As Exception
                CheckNewDesMAGOnTab = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore in lettura dati Magazzini (CheckNewDesMAGOnTab)", ex.Message.Trim, WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End Try
        Else
            CheckNewDesMAGOnTab = False
        End If
    End Function
#End Region


    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        '---- SESSIONE SCADUTA ???
        Dim IDMAG As Integer
        If CKIDMagazzini(IDMAG) = False Then Exit Sub
        '-----------
        SqlDSArtDiMag.FilterExpression = "" : txtRicerca.Text = ""
        Session("SortArtDiMag") = Mid(ddlRicerca.SelectedValue.Trim, 1, 1)
        SqlDSArtDiMag.DataBind()
        lblCodArticolo.Text = ""
        lblDescrizione.Text = ""
        BtnSetEnabledTo(False)
        ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicercaArticolo.Enabled = True
        btnNuovo.Enabled = True
        GridViewArtMag.DataBind()
        If GridViewArtMag.Rows.Count > 0 Then
            GridViewArtMag.SelectedIndex = 0
            Dim row As GridViewRow = GridViewArtMag.SelectedRow
            lblCodArticolo.Text = row.Cells(1).Text.Trim
            lblDescrizione.Text = row.Cells(2).Text.Trim
        Else
            lblCodArticolo.Text = ""
            lblDescrizione.Text = ""
        End If
    End Sub

    Protected Sub btnRicercaArticolo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicercaArticolo.Click

        If txtRicerca.Text.Trim = "" Then
            SqlDSArtDiMag.FilterExpression = ""
            SqlDSArtDiMag.DataBind()
            lblCodArticolo.Text = ""
            lblDescrizione.Text = ""
            BtnSetEnabledTo(False)
            ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicercaArticolo.Enabled = True
            GridViewArtMag.DataBind()
            If GridViewArtMag.Rows.Count > 0 Then
                GridViewArtMag.SelectedIndex = 0
                Dim row As GridViewRow = GridViewArtMag.SelectedRow
                lblCodArticolo.Text = row.Cells(1).Text.Trim
                lblDescrizione.Text = row.Cells(2).Text.Trim
            Else
                lblCodArticolo.Text = ""
                lblDescrizione.Text = ""
            End If
            Exit Sub
        End If
        If ddlRicerca.SelectedValue = "C" Then
            SqlDSArtDiMag.FilterExpression = "Cod_Articolo like '" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
        ElseIf ddlRicerca.SelectedValue = "D1" Then
            SqlDSArtDiMag.FilterExpression = "Descrizione >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
        ElseIf ddlRicerca.SelectedValue = "D2" Then
            SqlDSArtDiMag.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
        End If
        SqlDSArtDiMag.DataBind()
        lblCodArticolo.Text = ""
        lblDescrizione.Text = ""
        ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicercaArticolo.Enabled = True
        GridViewArtMag.DataBind()
        If GridViewArtMag.Rows.Count > 0 Then
            GridViewArtMag.SelectedIndex = 0
            Dim row As GridViewRow = GridViewArtMag.SelectedRow
            lblCodArticolo.Text = row.Cells(1).Text.Trim
            lblDescrizione.Text = row.Cells(2).Text.Trim
            btnEliminaArt.Enabled = True
            btnStampaElenco.Enabled = True
        Else
            lblCodArticolo.Text = ""
            lblDescrizione.Text = ""
            btnEliminaArt.Enabled = False
            btnStampaElenco.Enabled = False
        End If

    End Sub

    'alb070213
#Region "Ricerche in includi/escludi"
    Private Sub btnRicercaArtIn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicercaArtIn.Click
        'Ricerca IN
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        'Codice (Senza prezzo) C2
        'Descrizione (Senza prezzo) D3
        Session("btnIN") = True
        If ddlRicercaArtIn.SelectedValue = "C" Then
            SqlDSArtIn.FilterExpression = "Cod_Articolo like '" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "%'"
        ElseIf ddlRicercaArtIn.SelectedValue = "D1" Then
            SqlDSArtIn.FilterExpression = "Descrizione >= '" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "'"
        ElseIf ddlRicercaArtIn.SelectedValue = "D2" Then
            SqlDSArtIn.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "%'"
        ElseIf ddlRicercaArtIn.SelectedValue = "C2" Then
            SqlDSArtIn.FilterExpression = "Prezzo = 0"
        ElseIf ddlRicercaArtIn.SelectedValue = "D3" Then
            SqlDSArtIn.FilterExpression = "Prezzo = 0"
        End If
        SqlDSArtIn.DataBind()
        If Not Session("btnOUT") Then
            btnRicercaArtOu_Click(Nothing, Nothing)
        ElseIf GridViewArtIn.Rows.Count = 0 Then
            btnRicercaArtOu_Click(Nothing, Nothing)
        End If
        Session("btnIN") = False
    End Sub

    Private Sub btnRicercaArtOu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRicercaArtOu.Click
        'Ricerca OUT
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        Session("btnOUT") = True
        If DDLRicercaArtOu.SelectedValue = "C" Then
            SqlDSArtOu.FilterExpression = "Cod_Articolo like '" & Controlla_Apice(txtRicercaArtOu.Text.Trim) & "%'"
        ElseIf DDLRicercaArtOu.SelectedValue = "D1" Then
            SqlDSArtOu.FilterExpression = "Descrizione >= '" & Controlla_Apice(txtRicercaArtOu.Text.Trim) & "'"
        ElseIf DDLRicercaArtOu.SelectedValue = "D2" Then
            SqlDSArtOu.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicercaArtOu.Text.Trim) & "%'"
        End If
        SqlDSArtOu.DataBind()
        If Not Session("btnIN") Then
            btnRicercaArtIn_Click(Nothing, Nothing)
        ElseIf GridViewArtOu.Rows.Count = 0 Then
            btnRicercaArtIn_Click(Nothing, Nothing)
        End If
        Session("btnOUT") = False
    End Sub

    Private Sub ddlRicercaArtIn_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicercaArtIn.SelectedIndexChanged
        'Ricerca IN
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        'Codice (Senza prezzo) C2
        'Descrizione (Senza prezzo) D3

        'Ricerca OUT
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        If ddlRicercaArtIn.SelectedValue = "C2" Then
            DDLRicercaArtOu.SelectedValue = "C"
        ElseIf ddlRicercaArtIn.SelectedValue = "D3" Then
            DDLRicercaArtOu.SelectedValue = "D1"
        Else
            DDLRicercaArtOu.SelectedValue = ddlRicercaArtIn.SelectedValue
        End If
        DDLRicercaArtOu_SelectedIndexChanged(Nothing, Nothing)
        'Session("SortArtDiMag") = Mid(ddlRicercaArtIn.SelectedValue.Trim, 1, 1)
        Select Case ddlRicercaArtIn.SelectedValue
            Case "C"
                GridViewArtIn.Sort("Cod_Articolo", SortDirection.Ascending)
            Case "D1"
                GridViewArtIn.Sort("Descrizione", SortDirection.Ascending)
            Case "D2"
                GridViewArtIn.Sort("Descrizione", SortDirection.Ascending)
            Case "C2"
                GridViewArtIn.Sort("Cod_Articolo", SortDirection.Ascending)
            Case "D3"
                GridViewArtIn.Sort("Descrizione", SortDirection.Ascending)
        End Select
    End Sub

    Private Sub DDLRicercaArtOu_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRicercaArtOu.SelectedIndexChanged
        'Ricerca IN
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        'Codice (Senza prezzo) C2
        'Descrizione (Senza prezzo) D3

        'Ricerca OUT
        'Codice C
        'Descrizione D1
        'Descrizione (Parole contenute) D2
        'Session("SortArtDiMag") = Mid(ddlRicercaArtIn.SelectedValue.Trim, 1, 1)
        Select Case DDLRicercaArtOu.SelectedValue
            Case "C"
                GridViewArtOu.Sort("Cod_Articolo", SortDirection.Ascending)
                If Left(ddlRicercaArtIn.SelectedValue, 1) <> "C" Then
                    ddlRicercaArtIn.SelectedValue = DDLRicercaArtOu.SelectedValue
                End If
            Case "D1"
                GridViewArtOu.Sort("Descrizione", SortDirection.Ascending)
                If Left(ddlRicercaArtIn.SelectedValue, 1) <> "D" Then
                    ddlRicercaArtIn.SelectedValue = DDLRicercaArtOu.SelectedValue
                End If
            Case "D2"
                GridViewArtOu.Sort("Descrizione", SortDirection.Ascending)
                If Left(ddlRicercaArtIn.SelectedValue, 1) <> "D" Then
                    ddlRicercaArtIn.SelectedValue = DDLRicercaArtOu.SelectedValue
                End If
        End Select
    End Sub

    Private Sub txtRicercaArtIn_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRicercaArtIn.TextChanged
        txtRicercaArtOu.Text = txtRicercaArtIn.Text
    End Sub

    Private Sub txtRicercaArtOu_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtRicercaArtOu.TextChanged
        txtRicercaArtIn.Text = txtRicercaArtOu.Text
    End Sub
#End Region
    'alb070213 END
End Class