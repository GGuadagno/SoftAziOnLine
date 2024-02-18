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
Partial Public Class WUC_IncidenzaNCFatturato
    Inherits System.Web.UI.UserControl


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not IsPostBack) Then
            chkTuttiClienti.Checked = True
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
        End If
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        WFP_ElencoCliForn2.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
            If Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
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
        Dim Ordinamento As Integer = 0
        Dim VisualizzaPrezzoVendita As Boolean = False
        Dim Statistica As Integer
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
            If (txtCodCliente1.Text = "" Or txtDescCliente1.Text = "") Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare i clienti"
                ErroreCampi = True
            End If
            If (txtCodCliente2.Text = "" Or txtDescCliente2.Text = "") Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare i clienti"
                ErroreCampi = True
            End If
        End If

        If ErroreCampi Then
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If txtNCData.Text = "" Then
            txtNCData.Text = txtDataA.Text
        End If

        Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.IncidenzaNCFatturato

        Statistica = 1  'fatturato

        If rbtnCodice.Checked Then
            Ordinamento = 0
        ElseIf rbtnRagSoc.Checked Then
            Ordinamento = 1
        ElseIf rbtnNetto.Checked Then
            Ordinamento = 2
        ElseIf rbtnNettoDesc.Checked Then
            Ordinamento = 3
        ElseIf rbtnNC.Checked Then
            Ordinamento = 4
        ElseIf rbtnNCDesc.Checked Then
            Ordinamento = 5
        End If

        Try
            If ClsPrint.StampaStatisticheIncidenzaNCFatturato(txtCodCliente1.Text, txtCodCliente2.Text, txtDataDa.Text, txtDataA.Text, txtNCData.Text, Session(CSTAZIENDARPT), DsStatVendCliArt1, ObjReport, StrErrore, Ordinamento) Then
                If DsStatVendCliArt1.get_IncNCFatt.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session("DSIncNCFatt") = DsStatVendCliArt1
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

    Private Sub chkTuttiClienti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiClienti.CheckedChanged
        pulisciCampiCliente()
        If chkTuttiClienti.Checked Then
            AbilitaDisabilitaCampiCliente(False)
        Else
            AbilitaDisabilitaCampiCliente(True)
            txtCodCliente1.Focus()
        End If
    End Sub
    
    Private Sub pulisciCampiCliente()
        txtCodCliente1.Text = ""
        txtDescCliente1.Text = ""
        txtCodCliente2.Text = ""
        txtDescCliente2.Text = ""
    End Sub

    Private Sub txtCodCliente1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCliente1.TextChanged
        txtDescCliente1.Text = App.GetValoreFromChiave(txtCodCliente1.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    Private Sub txtCodCliente2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCliente2.TextChanged
        txtDescCliente2.Text = App.GetValoreFromChiave(txtCodCliente2.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub
    'GIU300512
    Private Sub txtDataA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataA.TextChanged
        txtNCData.Text = txtDataA.Text
        txtNCData.Focus()
    End Sub

    Private Sub ApriElencoClienti1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub

    Public Sub CallBackWFPElencoCliForn(ByVal codice As String, ByVal descrizione As String)
        If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN1_APERTA) = True Then
            txtCodCliente1.Text = codice
            txtDescCliente1.Text = descrizione
            If txtCodCliente2.Text.Trim = "" Then
                txtCodCliente2.Text = codice
                txtDescCliente2.Text = descrizione
            End If
        ElseIf Session(OSCLI_F_ELENCO_CLI2_APERTA) = True Or Session(OSCLI_F_ELENCO_FORN2_APERTA) = True Then
            txtCodCliente2.Text = codice
            txtDescCliente2.Text = descrizione
        End If
        Session(OSCLI_F_ELENCO_CLI1_APERTA) = False
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = False
        Session(OSCLI_F_ELENCO_FORN2_APERTA) = False
    End Sub

    Private Sub btnCliente1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCliente1.Click
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti1()
    End Sub

    Private Sub btnCliente2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCliente2.Click
        Session(F_CLI_RICERCA) = True
        ApriElencoClienti2()
    End Sub

    Private Sub ApriElencoClienti2()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_CLI2_APERTA) = True
        WFP_ElencoCliForn2.Show(True)
    End Sub

    Private Sub AbilitaDisabilitaCampiCliente(ByVal abilita As Boolean)
        txtCodCliente1.Enabled = abilita
        txtCodCliente2.Enabled = abilita
        btnCliente1.Enabled = abilita
        btnCliente2.Enabled = abilita
    End Sub
End Class