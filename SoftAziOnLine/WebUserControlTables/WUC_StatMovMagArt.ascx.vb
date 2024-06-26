﻿Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms

Partial Public Class WUC_StatMovMagArt
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataMagazzino.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If (Not IsPostBack) Then
            ddlMagazzino.SelectedValue = 1

            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)

            chkTuttiArticoli.Checked = True
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCod1.Enabled = False
            btnCod2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            CaricaEsercizi() 'alb28012013

            chkTuttiEser.Checked = True
            chkTuttiEser_CheckedChanged(Nothing, Nothing)
        End If
        ModalPopup.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub



    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            If Session(STAMPAMOVMAGTORNAAELENCO) = True Then
                Session(STAMPAMOVMAGTORNAAELENCO) = False
                Response.Redirect("WF_ElencoMovMag.aspx?labelForm=Gestione movimenti di magazzino")
            Else
                Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Session(CSTNOMEPDF) = "" 'giu300315
        Dim selezione As String = ""
        Dim filtri As String = "Filtri usati: "
        Dim Errore As String = ""
        Dim clsStampa As New Statistiche
        Dim dsMovMag1 As New DSMovMag
        'giu090512
        'If chkFindLottiSerie.Checked = True Then commentato da Alberto 29/01/13 per abilitare
        '    chkTuttiArticoli.Checked = False     la ricerca degli articoli per n°serie/lotto
        '    chkStampaLotti.Checked = False       senza codice articolo
        '    'alb100512
        '    txtCod1.Enabled = True
        '    txtCod2.Enabled = True
        '    btnCod1.Enabled = True
        '    btnCod2.Enabled = True
        '    '-----
        'End If
        If ddlMagazzino.SelectedItem.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Scegliere il magazzino su cui agire.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Data inizio periodo superiore alla data fine periodo", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
                'GIU111020
                Try
                    If CDate(txtDataDa.Text.Trim).Year < Int(Session("MinEser")) Or CDate(txtDataA.Text.Trim).Year > Int(Session("MaxEser")) Then
                        txtDataDa.Text = "01/01/" & Session("MinEser")
                        txtDataA.Text = "31/12/" & Session("MaxEser")
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Controllo Data inizio/fine periodo.: Esercizi non compresi, imposto il primo e ultimo Esercizio", WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Controllo Data inizio/fine periodo.: " + ex.Message, WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End Try
                '-
            End If
        End If

        If chkTuttiArticoli.Checked = False Then
            If txtCod1.Text.Trim = "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Occorre inserire il codice articolo.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            If txtCod1.Text.Trim <> "" And txtCod2.Text.Trim <> "" Then
                If txtCod1.Text.Trim > txtCod2.Text.Trim Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Il codice articolo di inizio intervallo è superiore a quello finale.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            End If
        Else
            If txtDesc1.Text.Trim <> "" And txtDesc2.Text.Trim <> "" Then
                If txtDesc1.Text.Trim > txtDesc2.Text.Trim Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "La descrizione di inizio intervallo è superiore a quella finale.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            End If
        End If
        'Giu090512
        If chkFindLottiSerie.Checked Then
            If txtLotto.Text.Trim = "" And txtNSerie.Text.Trim = "" Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "La ricerca per Lotto/N° Serie richiede almeno un dato valido da ricercare.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            'If txtCod1.Text.Trim = "" Or txtCod2.Text.Trim = "" Then
            '    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            '    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '    ModalPopup.Show("Attenzione", "La ricerca per Lotto/N° Serie richiede un solo codice articolo.", WUC_ModalPopup.TYPE_ALERT)
            '    Exit Sub
            'End If
            'If txtCod1.Text.Trim <> txtCod2.Text.Trim Then
            '    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            '    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '    ModalPopup.Show("Attenzione", "La ricerca per Lotto/N° Serie richiede un solo codice articolo.", WUC_ModalPopup.TYPE_ALERT)
            '    Exit Sub
            'End If
        End If
        '---------
        '.GIU220920
        filtri = "Filtri usati: Magazzino - " & ddlMagazzino.SelectedItem.Text.ToUpper & " - "
        If Val(ddlMagazzino.SelectedValue.Trim) = 0 Then
            'OK TUTTI
        Else
            selezione = " WHERE CodiceMagazzino=" & ddlMagazzino.SelectedValue.Trim
        End If
        '----------
        If txtDataDa.Text <> "" Then
            If IsDate(txtDataDa.Text) Then
                If selezione.Trim = "" Then
                    selezione = " WHERE (Data_Doc >= CONVERT(DATETIME,'" & Format(CDate(txtDataDa.Text), FormatoData) & "',103))"
                Else
                    selezione += " AND (Data_Doc >= CONVERT(DATETIME,'" & Format(CDate(txtDataDa.Text), FormatoData) & "',103))"
                End If
                filtri = filtri & "Dal " & txtDataDa.Text.Trim & " | "
            End If
        End If
        If txtDataA.Text <> "" Then
            If IsDate(txtDataA.Text) Then
                If selezione.Trim = "" Then
                    selezione = " WHERE (Data_Doc <= CONVERT(DATETIME,'" & Format(CDate(txtDataA.Text), FormatoData) & "',103))"
                Else
                    selezione = selezione & " AND (Data_Doc <= CONVERT(DATETIME,'" & Format(CDate(txtDataA.Text), FormatoData) & "',103))"
                End If
                filtri = filtri & "Al " & txtDataA.Text.Trim & " | "
            End If
        End If

        If chkTuttiArticoli.Checked = False Then
            If txtCod1.Text.Trim <> "" Then
                If selezione.Trim = "" Then
                    selezione = " WHERE (Cod_Articolo >= '" & Controlla_Apice(txtCod1.Text.Trim) & "') "
                Else
                    selezione = selezione & " AND (Cod_Articolo >= '" & Controlla_Apice(txtCod1.Text.Trim) & "') "
                End If
                filtri = filtri & "Dal codice articolo - " & txtCod1.Text.Trim & " | "
            End If

            If txtCod2.Text.Trim <> "" Then
                If selezione.Trim = "" Then
                    selezione = " WHERE (Cod_Articolo <= '" & Controlla_Apice(txtCod2.Text.Trim) & "') "
                Else
                    selezione = selezione & " AND (Cod_Articolo <= '" & Controlla_Apice(txtCod2.Text.Trim) & "') "
                End If
                filtri = filtri & "Al codice articolo - " & txtCod2.Text.Trim & " | "
            End If
        End If

        If rbSoloCarichi.Checked Then
            If selezione.Trim = "" Then
                selezione = " WHERE (LTRIM(RTRIM(Segno_Giacenza)) <> '-') "
            Else
                selezione = selezione & " AND (LTRIM(RTRIM(Segno_Giacenza)) <> '-') "
            End If
            filtri = filtri & "Solo carichi | "
        ElseIf rbScarichi.Checked Then
            If selezione.Trim = "" Then
                selezione = " WHERE (LTRIM(RTRIM(Segno_Giacenza)) <> '+') "
            Else
                selezione = selezione & " AND (LTRIM(RTRIM(Segno_Giacenza)) <> '+') "
            End If
            filtri = filtri & "Solo scarichi | "
        End If

        'alb070512 giu090512 aggiunto nuovo chk CORRETTO AND A OR SE PRESENTI ENTRAMBI
        If chkFindLottiSerie.Checked Then
            If txtNSerie.Text.Trim <> "" And txtLotto.Text.Trim <> "" Then
                If selezione.Trim = "" Then
                    'selezione = " WHERE IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE NSerie = '" & Controlla_Apice(txtNSerie.Text.Trim) & "' OR Lotto = '" & Controlla_Apice(txtLotto.Text.Trim) & "')" alb180213
                    selezione = " WHERE IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE NSerie LIKE '%" & Controlla_Apice(txtNSerie.Text.Trim) & "%' OR Lotto LIKE '%" & Controlla_Apice(txtLotto.Text.Trim) & "%') AND Cod_Articolo IN (SELECT Cod_Articolo FROM DocumentiDLotti WHERE NSerie LIKE '%" & Controlla_Apice(txtNSerie.Text.Trim) & "%' OR Lotto LIKE '%" & Controlla_Apice(txtLotto.Text.Trim) & "%') "
                Else
                    'selezione = selezione & " AND IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE NSerie = '" & Controlla_Apice(txtNSerie.Text.Trim) & "' OR Lotto = '" & Controlla_Apice(txtLotto.Text.Trim) & "')" alb180213
                    selezione = selezione & " AND IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE NSerie LIKE '%" & Controlla_Apice(txtNSerie.Text.Trim) & "%' OR Lotto LIKE '%" & Controlla_Apice(txtLotto.Text.Trim) & "%') AND Cod_Articolo IN (SELECT Cod_Articolo FROM DocumentiDLotti WHERE NSerie LIKE '%" & Controlla_Apice(txtNSerie.Text.Trim) & "%' OR Lotto LIKE '%" & Controlla_Apice(txtLotto.Text.Trim) & "%') "
                End If
            ElseIf txtLotto.Text.Trim <> "" Then
                If selezione.Trim = "" Then
                    'selezione = " WHERE IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE Lotto = '" & Controlla_Apice(txtLotto.Text.Trim) & "')" alb180213
                    selezione = " WHERE IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE Lotto LIKE '%" & Controlla_Apice(txtLotto.Text.Trim) & "%') AND Cod_Articolo IN (SELECT Cod_Articolo FROM DocumentiDLotti WHERE Lotto LIKE '%" & Controlla_Apice(txtLotto.Text.Trim) & "%') "
                Else
                    'selezione = selezione & " AND IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE Lotto = '" & Controlla_Apice(txtLotto.Text.Trim) & "')" alb180213
                    selezione = selezione & " AND IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE Lotto LIKE '%" & Controlla_Apice(txtLotto.Text.Trim) & "%') AND Cod_Articolo IN (SELECT Cod_Articolo FROM DocumentiDLotti WHERE Lotto LIKE '%" & Controlla_Apice(txtLotto.Text.Trim) & "%') "
                End If
            ElseIf txtNSerie.Text.Trim <> "" Then
                If selezione.Trim = "" Then
                    'selezione = " WHERE IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE NSerie = '" & Controlla_Apice(txtNSerie.Text.Trim) & "')" alb180213
                    selezione = " WHERE IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE NSerie LIKE '%" & Controlla_Apice(txtNSerie.Text.Trim) & "%') AND Cod_Articolo IN (SELECT Cod_Articolo FROM DocumentiDLotti WHERE NSerie LIKE '%" & Controlla_Apice(txtNSerie.Text.Trim) & "%') "
                Else
                    'selezione = selezione & " AND IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE NSerie = '" & Controlla_Apice(txtNSerie.Text.Trim) & "')" alb180213
                    selezione = selezione & " AND IDDocumenti IN (SELECT IDDocumenti FROM DocumentiDLotti WHERE NSerie LIKE '%" & Controlla_Apice(txtNSerie.Text.Trim) & "%') AND Cod_Articolo IN (SELECT Cod_Articolo FROM DocumentiDLotti WHERE NSerie LIKE '%" & Controlla_Apice(txtNSerie.Text.Trim) & "%') "
                End If
            End If
        End If
        '--------------------------

        Try
            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByArticolo
            If clsStampa.StampaMovMag(Session(CSTAZIENDARPT), "Riepilogo movimenti di magazzino per articolo - Magazzino: " & ddlMagazzino.SelectedItem.Text.Trim & IIf(CBool(chkStampaLotti.Checked), " (Lotti/N° Serie collegati)", " (Senza Lotti/N° Serie collegati)"), selezione, dsMovMag1, Errore, filtri, -1, "", CBool(chkStampaLotti.Checked), txtNSerie.Text.Trim, txtLotto.Text.Trim, CBool(chkFindLottiSerie.Checked), False, txtDataDa.Text, txtDataA.Text, Session("MinEser"), Session("MaxEser")) Then

                If dsMovMag1.view_MovMag.Count > 0 Then
                    Session(CSTDsPrinWebDoc) = dsMovMag1
                    Session(CSTNOBACK) = 0 'giu040512
                    Response.Redirect("..\WebFormTables\WF_PrintWebMovMag.aspx")
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Nessun dato soddisfa i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                End If
            Else
                'Session(STAMPAMOVMAGTORNAAELENCO) = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Protected Sub btnCod1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCod1.Click
        Session(SWCOD1COD2) = 1
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiArticoli.CheckedChanged
        If chkTuttiArticoli.Checked Then
            txtCod1.Text = ""
            txtCod2.Text = ""
            txtDesc1.Text = ""
            txtDesc2.Text = ""
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            btnCod1.Enabled = False
            btnCod2.Enabled = False
        Else
            txtCod1.Enabled = True
            txtCod2.Enabled = True
            btnCod1.Enabled = True
            btnCod2.Enabled = True
        End If
    End Sub

    Private Sub btnCod2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod2.Click
        Session(SWCOD1COD2) = 2
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Protected Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
        If txtCod2.Text.Trim = "" Or chkStampaLotti.Checked Then 'alb07052012
            txtCod2.Text = txtCod1.Text
            txtDesc2.Text = txtDesc1.Text
        End If
        txtCod2.Focus()
    End Sub
    Protected Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub

    Private Sub chkFindLottiSerie_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFindLottiSerie.CheckedChanged
        If chkFindLottiSerie.Checked Then
            txtNSerie.Enabled = True
            txtLotto.Enabled = True
            'Commentato da Alberto 29/01/13
            'chkTuttiArticoli.Checked = False
            'txtCod1.Enabled = True : txtCod2.Enabled = True
            'btnCod1.Enabled = True : btnCod2.Enabled = True
            'chkStampaLotti.Checked = False
        Else
            txtNSerie.Enabled = False
            txtLotto.Enabled = False
            txtNSerie.Text = ""
            txtLotto.Text = ""
        End If
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

    'alb28012013
    Private Sub chkTuttiEser_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiEser.CheckedChanged
        If chkTuttiEser.Checked Then
            txtDataDa.Text = "01/01/" & Session("MinEser")
            txtDataA.Text = "31/12/" & Session("MaxEser")
        Else
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
        End If
    End Sub

    Public Sub ErrEser()
        chkTuttiEser.Checked = False
    End Sub

    Private Sub CaricaEsercizi()
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim sqlConnInst As New SqlConnection
        Dim sqlCmdMinMaxEser As New SqlCommand
        Dim MinEser, MaxEser As Integer

        sqlConnInst.ConnectionString = dbCon.getConnectionString(TipoDB.dbInstall)
        sqlCmdMinMaxEser.Connection = sqlConnInst
        sqlCmdMinMaxEser.CommandType = CommandType.Text
        sqlCmdMinMaxEser.CommandText = "SELECT @MinEser = MIN(CAST(Esercizio AS INT)), @MaxEser = MAX(CAST(Esercizio AS INT)) FROM Esercizi WHERE Ditta = @Ditta"
        sqlCmdMinMaxEser.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ditta", System.Data.SqlDbType.NVarChar, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        sqlCmdMinMaxEser.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MinEser", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, True, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        sqlCmdMinMaxEser.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MaxEser", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, True, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

        Try
            sqlCmdMinMaxEser.Parameters("@Ditta").Value = Session(CSTCODDITTA)

            If sqlConnInst.State <> ConnectionState.Open Then
                sqlConnInst.Open()
            End If

            sqlCmdMinMaxEser.ExecuteNonQuery()

            MinEser = sqlCmdMinMaxEser.Parameters("@MinEser").Value
            MaxEser = sqlCmdMinMaxEser.Parameters("@MaxEser").Value
            sqlConnInst.Close()
        Catch ex As Exception
            sqlConnInst.Close()
            Session(MODALPOPUP_CALLBACK_METHOD) = "ErrEser"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Si è verificato il seguente errore durante la lettura del minimo e del massimo esercizio: " & vbCr _
                            & ex.Message, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try
        Session("MinEser") = MinEser
        Session("MaxEser") = MaxEser
    End Sub
    '-----------------
End Class