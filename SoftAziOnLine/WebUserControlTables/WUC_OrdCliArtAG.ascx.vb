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
Partial Public Class WUC_OrdCliArtAG
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_Agenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        If (Not IsPostBack) Then
            'rbtnPrezzoVendita.Checked = True
            'rbtnCliente.Checked = True
            chkTuttiArticoli.Checked = True
            chkTuttiClienti.Checked = True
            txtCodArticolo.Enabled = False
            txtCodCliente.Enabled = False
            btnCliente.Enabled = False
            btnCod1.Enabled = False

            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            Try
                impostaDate()
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            End Try
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        ModalPopup.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me

        If Session(F_ELENCO_CLIFORN_APERTA) Then
            If Session(OSCLI_F_ELENCO_CLI1_APERTA) = True Then
                WFP_ElencoCliForn1.Show()
            End If
        End If
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
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
        tmpCommand.CommandText = "get_dateDocPerRiepOrdinato"
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
        End If

        tmpConn.Close()
        tmpConn.Dispose()
        tmpConn = Nothing
        tmpCommand.Dispose()
        tmpCommand = Nothing
        objDB = Nothing

    End Sub
    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click

        'Roberto 14/12/2011---
        Dim dsOrdinatoArtCli1 As New DSOrdinatoArtCli
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErroreCampi As String = ""
        Dim ErroreCampi As Boolean = False
        Dim strWhere As String

        Dim SWSconti As Boolean = False
        Try
            Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteAg
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

            If chkTuttiClienti.Checked = False Then
                If (txtCodCliente.Text = "" Or txtDescCliente.Text = "") Then
                    StrErroreCampi = StrErroreCampi & "<BR>- selezionare il cliente"
                    ErroreCampi = True
                End If
            End If

            If chkTuttiArticoli.Checked = False Then
                If txtCodArticolo.Text = "" Then
                    StrErroreCampi = StrErroreCampi & "<BR>- occorre inserire il codice articolo."
                    ErroreCampi = True
                End If
            End If



            If ErroreCampi Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If

            Dim strNomeAz As String = Session(CSTAZIENDARPT)
            Session(CSTTIPORPTMAG) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteAg

            strWhere = " Data_Ordine >= CONVERT(DATETIME, '" & Format(CDate(txtDataDa.Text), FormatoData) & "', 103) AND " & _
                       " Data_Ordine <= CONVERT(DATETIME, '" & Format(CDate(txtDataA.Text), FormatoData) & "', 103) "

            Dim strDescrizioneSelezioneDati As String

            strDescrizioneSelezioneDati = "Dal " & txtDataDa.Text & " al " & txtDataA.Text


            If chkTuttiArticoli.Checked = False Then
                strWhere = strWhere & " AND LTRIM(RTRIM(Cod_Articolo)) = '" & txtCodArticolo.Text.Trim & "' "

            End If

            If chkTuttiClienti.Checked = False Then
                strWhere = strWhere & " AND LTRIM(RTRIM(Codice_Coge)) = '" & txtCodCliente.Text.Trim & "' "
            End If

            If chkTuttiAgenti.Checked = False Then
                strWhere = strWhere & " AND Agente = " & ddlAgenti.SelectedValue
            End If

            If ClsPrint.StampaOrdinatoArtCliAg(strNomeAz, strDescrizioneSelezioneDati, dsOrdinatoArtCli1, ObjReport, StrErroreCampi, strWhere) Then
                If dsOrdinatoArtCli1.OrdArtCli.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session(CSTDsPrinWebDoc) = dsOrdinatoArtCli1
                    Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebOrdinato.aspx")
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErroreCampi, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in OrdCliArt.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
        '--------------------

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


    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodArticolo.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCodArticolo.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub

    'Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
    '    'txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    'End Sub


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
            txtCodArticolo.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiArticolo(ByVal Abilita As Boolean)
        txtCodArticolo.Enabled = Abilita
        'txtCod2.Enabled = Abilita
        btnCod1.Enabled = Abilita
        'btnCod2.Enabled = Abilita
        'txtDesc1.Enabled = Abilita
        'txtDesc2.Enabled = Abilita
    End Sub
    Private Sub AbilitaDisabilitaCampiCliente(ByVal Abilita As Boolean)
        txtCodCliente.Enabled = Abilita
        btnCliente.Enabled = Abilita
        'txtDescCliente.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiArticolo()
        txtCodArticolo.Text = ""
        'txtCod2.Text = ""
        txtDesc1.Text = ""
        'txtDesc2.Text = ""
    End Sub
    Private Sub pulisciCampiCliente()
        txtCodCliente.Text = ""
        txtDescCliente.Text = ""
    End Sub

    Private Sub txtCodCliente_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodCliente.TextChanged
        txtDescCliente.Text = App.GetValoreFromChiave(txtCodCliente.Text, Def.CLIENTI, Session(ESERCIZIO))
    End Sub

    Private Sub chkTuttiAgenti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiAgenti.CheckedChanged
        ddlAgenti.SelectedIndex = -1
        If chkTuttiAgenti.Checked Then
            ddlAgenti.Enabled = False
        Else
            ddlAgenti.Enabled = True
        End If
    End Sub

    Private Sub btnCod1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod1.Click
        Session(SWCOD1COD2) = 1
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
            txtCodArticolo.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCodArticolo.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            'txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            'txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            'txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCodArticolo.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCodArticolo.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                'ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                '    txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                '    txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                '    txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCodArticolo.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCodArticolo.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                'txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                'txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                'txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCodArticolo.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCodArticolo.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            'txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            'txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            'txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
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
End Class