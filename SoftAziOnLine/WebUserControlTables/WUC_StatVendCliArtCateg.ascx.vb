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
Partial Public Class WUC_StatVendCliArtCateg
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_CatCli.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)

        If (Not IsPostBack) Then
            rbtnPrezzoVendita.Checked = True
            rbtnCliente.Checked = True
            chkTuttiArticoli.Checked = True
            chkTuttiClienti.Checked = True
            rbtnVenduto.Checked = True
            AbilitaDisabilitaCampiArticolo(False)
            AbilitaDisabilitaCampiCliente(False)
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            txtNCData.Text = "31/12/" & Session(ESERCIZIO)
            ' ''Try
            ' ''    impostaDate()
            ' ''Catch ex As Exception
            ' ''    ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            ' ''End Try
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            If Request.QueryString("labelForm") = "Venduto/Fatturato per categoria/articolo/cliente" Then
                rbtnArticolo.Checked = True
                rbtnCliente.Checked = False
            Else
                rbtnCliente.Checked = True
                rbtnArticolo.Checked = False
            End If
        End If
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If
        If Session(F_SEL_ARTICOLO_APERTA) Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub
    Private Sub impostaDate()

        Dim objDB As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim tmpConn As New SqlConnection
        tmpConn.ConnectionString = objDB.getConnectionString(It.SoftAzi.Integration.Dao.DataSource.TipoConnessione.dbSoftAzi)
        Dim tmpCommand As New SqlCommand
        tmpCommand.Connection = tmpConn
        tmpCommand.CommandType = CommandType.StoredProcedure
        tmpCommand.CommandText = "get_dateDocPerRiepVendutoFatturato"
        tmpCommand.Parameters.Add("@DaData", SqlDbType.DateTime)
        tmpCommand.Parameters("@DaData").Direction = ParameterDirection.Output
        tmpCommand.Parameters.Add("@AData", SqlDbType.DateTime)
        tmpCommand.Parameters("@AData").Direction = ParameterDirection.Output

        tmpConn.Open()
        tmpCommand.ExecuteNonQuery()

        If Not IsDBNull(tmpCommand.Parameters("@DaData").Value) Then
            txtDataDa.Text = Format(CDate(tmpCommand.Parameters("@DaData").Value), FormatoData)
        End If
        If Not IsDBNull(tmpCommand.Parameters("@AData").Value) Then
            txtDataA.Text = Format(CDate(tmpCommand.Parameters("@AData").Value), FormatoData)
            txtNCData.Text = txtDataA.Text
        End If

        tmpConn.Close()
        tmpConn.Dispose()
        tmpConn = Nothing
        tmpCommand.Dispose()
        tmpCommand = Nothing
        objDB = Nothing

    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim DsStatVendCliArt1 As New DsStatVendCliArt
        Dim ObjReport As New Object
        Dim ClsPrint As New Statistiche
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"
        Dim StrErrore As String = ""
        Dim Ordinamento As Integer = 0 'INDICA SE REPORT E' ORDINATO PER CLIENTE/ARTICOLO O ARTICOLO/CLIENTE
        Dim VisualizzaPrezzoVendita As Boolean = False
        Dim Statistica As Integer
        Dim CodCatCli As Integer
        Dim SWRaggrCatCli As Boolean
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If txtDataDa.Text = "" Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di inizio periodo"
            ErroreCampi = True
        End If
        If txtDataA.Text = "" Then
            StrErroreCampi = StrErroreCampi & "<BR>- inserire la data di fine periodo"
            ErroreCampi = True
        End If

        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    StrErroreCampi = StrErroreCampi & "<BR>- data inizio periodo superiore alla data fine periodo"
                    ErroreCampi = True
                End If
            End If
        End If

        If txtDataA.Text <> "" And txtNCData.Text <> "" Then
            If IsDate(txtDataA.Text) And IsDate(txtNCData.Text) Then
                If CDate(txtDataA.Text) > CDate(txtNCData.Text) Then
                    StrErroreCampi = StrErroreCampi & "<BR>- data fine periodo superiore alla data fine periodo NC"
                    ErroreCampi = True
                End If
            End If
        End If

        If chkTuttiClienti.Checked = False Then
            If (txtCodCliente.Text = "" Or txtDescCliente.Text = "") Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare il cliente"
                ErroreCampi = True
            End If
        End If

        If chkTuttiArticoli.Checked = False Then
            If txtCodArt1.Text = "" And txtCodArt2.Text = "" Then
                StrErroreCampi = StrErroreCampi & "<BR>- occorre inserire il codice articolo di inizio o di fine intervallo"
                ErroreCampi = True
            End If
            If txtCodArt1.Text <> "" And txtCodArt2.Text <> "" Then
                If txtCodArt1.Text > txtCodArt2.Text Then
                    StrErroreCampi = StrErroreCampi & "<BR>- il codice articolo di inizio intervallo è superiore a quello finale"
                    ErroreCampi = True
                End If
            End If
        End If
        If ErroreCampi Then
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If txtNCData.Text = "" Then
            txtNCData.Text = txtDataA.Text
        End If

        If rbtnCliente.Checked Then
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloCC  '15 ORDINATO PER CLIENTE / ARTICOLO
        Else
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteCC  '16 ORDINATO PER ARTICOLO / CLIENTE 
        End If

        VisualizzaPrezzoVendita = rbtnPrezzoVendita.Checked

        If rbtnVenduto.Checked Then
            Statistica = 0  'venduto e fatturato
        ElseIf rbtnFatturato.Checked Then
            Statistica = 1  'fatturato
        ElseIf rbtnDaFatturare.Checked Then
            Statistica = 2  'da fatturare
        Else
            Statistica = 0
        End If

        If chkTutteCatCli.Checked Then
            CodCatCli = -1
        Else
            CodCatCli = ddlCatCli.SelectedValue
        End If
        If chkRaggrCatCli.Checked Then
            SWRaggrCatCli = True
        Else
            SWRaggrCatCli = False
        End If
        Dim strCategRagg As String
        Dim SWTratt As Integer
        Dim AccorpaCR As Boolean
        SWTratt = InStr(ddlCatCli.SelectedItem.Text.Trim, " - ")
        If chkTutteCatCli.Checked = False And chkRaggrCatCli.Checked = True Then
            If SWTratt = 0 Then
                strCategRagg = ddlCatCli.SelectedItem.Text.Trim
            Else
                'VA BENE ANCHE QUESTA strCategRagg = Mid(ddlCatCli.SelectedItem.Text.Trim, 1, SWTratt - 1)
                strCategRagg = Left(ddlCatCli.SelectedItem.Text.Trim, SWTratt - 1)
            End If
            AccorpaCR = chkAccorpaCC.Checked
        Else
            strCategRagg = ""
            AccorpaCR = False
        End If
        Try
            If ClsPrint.StampaStatisticheVendutoArticoloClienteCC(txtCodArt1.Text, txtCodArt2.Text, txtCodCliente.Text, txtDataDa.Text, txtDataA.Text, txtNCData.Text, Session(CSTSTATISTICHE), Statistica, VisualizzaPrezzoVendita, Session(CSTAZIENDARPT), DsStatVendCliArt1, ObjReport, StrErrore, CodCatCli, strCategRagg, AccorpaCR) Then
                If DsStatVendCliArt1.StCliArt.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDSStatVendCliArt) = DsStatVendCliArt1
                    Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
                Else
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            ModalPopup.Show("Errore in Statistiche.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub


    Private Sub txtCodArt1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodArt1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCodArt1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub

    Private Sub txtCodArt2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodArt2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCodArt2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub


    Private Sub chkTuttiClienti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiClienti.CheckedChanged
        pulisciCampiCliente()
        If chkTuttiClienti.Checked Then
            AbilitaDisabilitaCampiCliente(False)
        Else
            AbilitaDisabilitaCampiCliente(True)
            txtCodCliente.Focus()
        End If
    End Sub
    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiArticoli.CheckedChanged
        pulisciCampiArticolo()
        If chkTuttiArticoli.Checked Then
            AbilitaDisabilitaCampiArticolo(False)
        Else
            AbilitaDisabilitaCampiArticolo(True)
            txtCodArt1.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiArticolo(ByVal Abilita As Boolean)
        txtCodArt1.Enabled = Abilita
        txtCodArt2.Enabled = Abilita
        btnCod1.Enabled = Abilita
        btnCod2.Enabled = Abilita
        'txtDesc1.Enabled = Abilita
        'txtDesc2.Enabled = Abilita
    End Sub
    Private Sub AbilitaDisabilitaCampiCliente(ByVal Abilita As Boolean)
        txtCodCliente.Enabled = Abilita
        btnCliente.Enabled = Abilita
        'txtDescCliente.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiArticolo()
        txtCodArt1.Text = ""
        txtCodArt2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
    End Sub
    Private Sub pulisciCampiCliente()
        txtCodCliente.Text = ""
        txtDescCliente.Text = ""
    End Sub

    Private Sub txtCodCliente_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCliente.TextChanged
        txtDescCliente.Text = App.GetValoreFromChiave(txtCodCliente.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    Private Sub chkTutteCatCli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteCatCli.CheckedChanged
        ddlCatCli.SelectedIndex = -1
        If chkTutteCatCli.Checked Then
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkAccorpaCC.Enabled = False
            chkAccorpaCC.Checked = False
        Else
            ddlCatCli.Enabled = True
            chkRaggrCatCli.Enabled = True
        End If
    End Sub

    'GIU300512
    Private Sub txtDataA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataA.TextChanged
        txtNCData.Text = txtDataA.Text
        txtNCData.Focus()
    End Sub

    Private Sub btnCod1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod1.Click
        Session(SWCOD1COD2) = 1
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Private Sub btnCod2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod2.Click
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
            txtCodArt1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCodArt1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCodArt2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCodArt2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCodArt1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCodArt1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                If txtCodArt2.Text.Trim = "" Then
                    txtCodArt2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                    txtCodArt2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                    txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                End If
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCodArt2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCodArt2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCodArt1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCodArt1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtCodArt2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCodArt2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCodArt1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCodArt1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCodArt2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCodArt2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        End If
        'TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        'TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub

    Private Sub btnCliente_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCliente.Click
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()
    End Sub

    Private Sub ApriElencoClienti1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodCliente.Text = codice
            txtDescCliente.Text = descrizione
        End If
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    End Sub

    Private Sub chkRaggrCatCli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRaggrCatCli.CheckedChanged
        If chkRaggrCatCli.Checked = True Then
            chkAccorpaCC.Enabled = True
        Else
            chkAccorpaCC.Enabled = False
            chkAccorpaCC.Checked = False
        End If
    End Sub
End Class