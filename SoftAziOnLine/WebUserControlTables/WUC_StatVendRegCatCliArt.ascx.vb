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
Partial Public Class WUC_StatVendRegCatCliArt
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_Regioni.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDSProvince.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDa_CatCli.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)

        If (Not IsPostBack) Then
            chkTuttiArticoli.Checked = True
            chkTuttiCategorie.Checked = True
            rbtnVenduto.Checked = True
            AbilitaDisabilitaCampiArticolo(False)
            AbilitaDisabilitaCampiCat(False)
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            txtNCData.Text = "31/12/" & Session(ESERCIZIO)
            '-
            chkTutteRegioni.Checked = True
            ddlRegioni.Enabled = False
            chkTutteProvince.Checked = True
            ddlProvince.Enabled = False
            Session("CodRegione") = 0
            SqlDSProvince.DataBind()
            '-----
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        ModalPopup.WucElement = Me
        WFP_ElencoCliForn1.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me

        If Session(F_SEL_ARTICOLO_APERTA) Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim DsStatVendCliArt1 As New DsStatVendCliArt
        Dim ObjReport As New Object
        Dim ClsPrint As New Statistiche
        Dim ErroreCampi As Boolean = False
        Dim StrErroreCampi As String = "Per procedere nella stampa, correggere i seguenti errori:"
        Dim StrErrore As String = ""
        Dim Statistica As Integer
        Dim CodRegione As Integer
        Dim Provincia As String
        Dim CodCateg As Integer
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

        If chkTuttiCategorie.Checked = False Then
            If (ddlCatCli.SelectedValue = "" Or ddlCatCli.SelectedIndex = -1) Then
                StrErroreCampi = StrErroreCampi & "<BR>- selezionare la categoria"
                ErroreCampi = True
            End If
        End If

        If chkTuttiArticoli.Checked = False Then
            If txtCod1.Text = "" And txtCod2.Text = "" Then
                StrErroreCampi = StrErroreCampi & "<BR>- occorre inserire il codice articolo di inizio o di fine intervallo"
                ErroreCampi = True
            End If
            If txtCod1.Text <> "" And txtCod2.Text <> "" Then
                If txtCod1.Text > txtCod2.Text Then
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

        Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegionePRCategCliArt

        If rbtnVenduto.Checked Then
            Statistica = 0  'venduto e fatturato
        ElseIf rbtnFatturato.Checked Then
            Statistica = 1  'fatturato
        ElseIf rbtnDaFatturare.Checked Then
            Statistica = 2  'da fatturare
        Else
            Statistica = 0
        End If
        If chkTutteRegioni.Checked Then
            CodRegione = -1
        Else
            CodRegione = ddlRegioni.SelectedValue
        End If
        If chkTutteProvince.Checked Then
            Provincia = ""
        Else
            Provincia = ddlProvince.SelectedValue
        End If

        If chkTuttiCategorie.Checked Then
            CodCateg = -1
        Else
            CodCateg = ddlCatCli.SelectedValue
        End If
        If chkRaggrCatCli.Checked Then
            SWRaggrCatCli = True
        Else
            SWRaggrCatCli = False
        End If
        Dim strCategRagg As String = ""
        Dim SWTratt As Integer = 0
        Dim AccorpaCR As Boolean = False
        SWTratt = InStr(ddlCatCli.SelectedItem.Text.Trim, " - ")
        If chkTuttiCategorie.Checked = False And chkRaggrCatCli.Checked = True Then
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
            If ClsPrint.StampaStatisticheVendutoRegPrCatCliArt(txtCod1.Text, txtCod2.Text, txtDataDa.Text, txtDataA.Text, txtNCData.Text, Statistica, Session(CSTAZIENDARPT), DsStatVendCliArt1, ObjReport, StrErrore, CodRegione, Provincia, CodCateg, strCategRagg, AccorpaCR) Then
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

#Region " Routine e controlli"
    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub
    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub
    Private Sub chkTuttiCategorie_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiCategorie.CheckedChanged
        pulisciCampiCategoria()
        If chkTuttiCategorie.Checked Then
            AbilitaDisabilitaCampiCat(False)
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkAccorpaCC.Enabled = False
            chkAccorpaCC.Checked = False
        Else
            AbilitaDisabilitaCampiCat(True)
            chkRaggrCatCli.Enabled = True
            ddlCatCli.Focus()
        End If
    End Sub
    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiArticoli.CheckedChanged
        pulisciCampiArticolo()
        If chkTuttiArticoli.Checked Then
            AbilitaDisabilitaCampiArticolo(False)
        Else
            AbilitaDisabilitaCampiArticolo(True)
            txtCod1.Focus()
        End If
    End Sub
    Private Sub AbilitaDisabilitaCampiArticolo(ByVal Abilita As Boolean)
        txtCod1.Enabled = Abilita
        txtCod2.Enabled = Abilita
        btnCod1.Enabled = Abilita
        btnCod2.Enabled = Abilita
        'txtDesc1.Enabled = Abilita
        'txtDesc2.Enabled = Abilita
    End Sub
    Private Sub AbilitaDisabilitaCampiCat(ByVal Abilita As Boolean)
        ddlCatCli.Enabled = Abilita
    End Sub
    Private Sub pulisciCampiArticolo()
        txtCod1.Text = ""
        txtCod2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
    End Sub
    Private Sub pulisciCampiCategoria()
        ddlCatCli.SelectedIndex = -1
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
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                If txtCod2.Text.Trim = "" Then
                    txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                    txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                    txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                End If
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        End If
        'TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        'TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub

    Private Sub ApriElencoFor1()
        Session(F_ELENCO_CLIFORN_APERTA) = True
        Session(OSCLI_F_ELENCO_FORN1_APERTA) = True
        WFP_ElencoCliForn1.Show(True)
    End Sub

#End Region

    Private Sub chkTuttiRegioni_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteRegioni.CheckedChanged
        If chkTutteRegioni.Checked Then
            ddlRegioni.Enabled = False
            Session("CodRegione") = 0 'alb080213
            ddlRegioni.SelectedIndex = -1
            ddlProvince.SelectedIndex = -1
            ddlProvince.Enabled = False
            chkTutteProvince.Checked = True
        Else
            Session("CodRegione") = ddlRegioni.SelectedValue 'alb080213
            ddlRegioni.Enabled = True
            ddlRegioni.Focus()
        End If
    End Sub

    Private Sub chkRaggrCatCli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRaggrCatCli.CheckedChanged
        If chkRaggrCatCli.Checked = True Then
            chkAccorpaCC.Enabled = True
        Else
            chkAccorpaCC.Enabled = False
            chkAccorpaCC.Checked = False
        End If
    End Sub

    Private Sub chkTutteProvince_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteProvince.CheckedChanged
        If chkTutteProvince.Checked Then
            ddlProvince.Enabled = False
            ddlProvince.SelectedIndex = -1
        Else
            ddlProvince.Enabled = True
            ddlProvince.Focus()
        End If
    End Sub

    Private Sub ddlRegioni_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegioni.SelectedIndexChanged
        Session("CodRegione") = ddlRegioni.SelectedValue
    End Sub
End Class