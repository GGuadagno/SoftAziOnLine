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

Partial Public Class WUC_FattAgenteCliente


    Inherits System.Web.UI.UserControl

    Public Const MSGSelezioneErrata As String = "Il numero di partenza deve essere precedere il numero di fine selezione."

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_Agenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)

        If (Not IsPostBack) Then
            chkNoNonDefinito.Checked = True 'giu240412
            ' ''txtDataDa.Text = CDate("01/" & Str(Now.Month) & "/" & Str(Now.Year)).AddMonths(-1)
            ' ''txtDataA.Text = CDate("01/" & Str(Now.Month) & "/" & Str(Now.Year)).AddDays(-1)
            'giu110712 
            ' ''txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            ' ''txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            Dim strValore As String = ""
            Dim strErrore As String = ""
            If App.GetDatiAbilitazioni(CSTABILCOGE, "GGChiusuraPR", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    SetPeriodoPrec(strValore.Trim)
                Else
                    txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
                    txtDataA.Text = "31/12/" & Session(ESERCIZIO)
                End If
            Else
                txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
                txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            End If

            If Request.QueryString("labelForm") = "Fatturato per agente sintetico" Then
                rbSintetico.Checked = True
            Else
                rbAnalitico.Checked = True
            End If
            chkTuttiAgenti.Checked = True
            ddlAgente.Enabled = False

            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            'GIU030412
            ' ''ddlAgente.Items.Clear()
            ' ''ddlAgente.Items.Add("")
            ' ''ddlAgente.DataBind()
            ddlAgente.Items.Clear()
            ddlAgente.Items.Add("[Agente non definito]")
            ddlAgente.DataBind()
        End If

        ModalPopup.WucElement = Me
    End Sub

    Private Sub SetPeriodoPrec(ByVal _NNGG As Integer)
        Try
            Dim _Dalla As Date : Dim _Alla As Date
            Dim InizioAnno As Date = CDate("01/01/" & Session(ESERCIZIO))
            Dim Data_Inizio_Esercizio As Date = CDate("01/01/" & Session(ESERCIZIO))
            Dim Data_Fine_Esercizio As Date = CDate("31/12/" & Session(ESERCIZIO))
            Dim MesiChiusura As Integer = _NNGG / 30
            Dim Intervallo As Integer = Fix(Month(Now) / MesiChiusura)
            If Intervallo < 1 Then
                txtDataDa.Text = Format(Data_Inizio_Esercizio, FormatoData)
                txtDataA.Text = Format(Data_Fine_Esercizio, FormatoData)
            Else
                _Dalla = RoundData(DateAdd("m", (Intervallo * MesiChiusura) - 1, InizioAnno), RoundDataEnum.FineMese)
                _Alla = RoundData(DateAdd("d", -_NNGG, _Dalla), RoundDataEnum.InizioMese)
                txtDataA.Text = Format(_Dalla, FormatoData)
                txtDataDa.Text = Format(_Alla, FormatoData)
            End If
        Catch ex As Exception
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
        End Try
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
        ' ''strRitorno = ""
        ' ''If strErrore.Trim = "" Then
        ' ''    strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        ' ''Else
        ' ''    strRitorno = "WF_Menu.aspx?labelForm=" & strErrore
        ' ''End If
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim DsFattAgente1 As New dsFattAgente
        Dim ObjReport As New Object
        Dim ClsPrint As New Statistiche
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"
        Dim StrErrore As String = ""
        Dim CodAgente As Integer = -1
        Dim VisualizzaPrezzoVendita As Boolean = False
        Dim Analitico As Boolean = False
        'Dim Statistica As Integer
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

        If chkTuttiAgenti.Checked = False Then
            If ddlAgente.Text.Trim = "" Then
                CodAgente = 0
            ElseIf Left(ddlAgente.Text.Trim, 1) = "[" Then
                CodAgente = 0
            Else
                CodAgente = ddlAgente.SelectedValue
            End If
        Else
            CodAgente = -1
        End If

        If ErroreCampi Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", StrErroreCampi, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If rbSintetico.Checked Then
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.FatturatoAgenteSintetico
            Analitico = False
        ElseIf rbAnalitico.Checked Then
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.FatturatoAgenteAnalitico
            Analitico = True
        End If


        Try
            If ClsPrint.StampaFattAgenteAnalitico(Analitico, CodAgente, txtDataDa.Text, txtDataA.Text, Session(CSTAZIENDARPT), DsFattAgente1, ObjReport, StrErrore, chkNoNonDefinito.Checked, chkDaConcordare.Checked) Then
                If DsFattAgente1.get_FatturatoAgente.Count > 0 Then
                    Session(CSTObjReport) = ObjReport
                    Session("dsFattAgente") = DsFattAgente1
                    Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_ALERT)
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Statistiche.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Protected Sub chkTuttiAgenti_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkTuttiAgenti.CheckedChanged
        ddlAgente.Enabled = Not (chkTuttiAgenti.Checked)
        chkNoNonDefinito.Enabled = chkTuttiAgenti.Checked
    End Sub
End Class