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


Partial Public Class WUC_ElencoOrdiniFornitori
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""

    Private Enum CellIdxT
        Stato = 1
        NumDoc = 2
        RevN = 3
        DataDoc = 4
        DataCons = 5
        CodCliForProvv = 6
        RagSoc = 7
        DataVal = 13
        Riferimento = 14
    End Enum
    Private Enum CellIdxD
        CodArt = 0
        DesArt = 1
        UM = 2
        Qta = 3
        QtaEv = 4
        QtaRe = 5
        IVA = 6
        Prz = 7
        ScV = 8
        Sc1 = 9
        Importo = 10
        ScR = 11
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Attesa.WucElement = Me 'giu030512
        'giu080312
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
        '-----------
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            btnNuovo.Visible = False
            btnModifica.Visible = False
            btnElimina.Visible = False
            btnCopia.Visible = False
            btnNuovaRev.Visible = False
        End If
        '----------------------------
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
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        'giu15112011 da ATTIVARE
        btnCaricoMagParz.Visible = True
        '-----------------------
        If (Not IsPostBack) Then
            Session(IDDOCUMSAVE) = 0 'PER L'EVASIONE PARZIALE
            Try
                btnSblocca.Text = "Sblocca documento"
                btnSblocca.Visible = False
                btnCambiaStato.Text = "Cambia Stato Note documento"
                '---
                rbtnDaEvadere.Checked = True
                Session(CSTTIPODOC) = SWTD(TD.OrdFornitori)
                Session(CSTSTATODOC) = "0"
                Session(CSTSORTPREVTEL) = "N"

                ddlRicerca.Items.Add("Numero ordine")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("Data ordine")
                ddlRicerca.Items(1).Value = "D"
                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(2).Value = "R"
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(3).Value = "S"
                ddlRicerca.Items.Add("Partita IVA")
                ddlRicerca.Items(4).Value = "P"
                ddlRicerca.Items.Add("Codice Fiscale")
                ddlRicerca.Items(5).Value = "F"
                ddlRicerca.Items.Add("Località")
                ddlRicerca.Items(6).Value = "L"
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(7).Value = "M"
                ddlRicerca.Items.Add("Data consegna")
                ddlRicerca.Items(8).Value = "C"
                ddlRicerca.Items.Add("Data validità")
                ddlRicerca.Items(9).Value = "V"
                ddlRicerca.Items.Add("Riferimento")
                ddlRicerca.Items(10).Value = "NR"

                btnCaricoMag.Text = "Carico totale Magazzino"
                btnCaricoMagParz.Text = "Carico parziale Magazzino"
                btnEliminaMM.Text = "Elimina Carichi"

                If GridViewPrevT.Rows.Count > 0 Then
                    BtnSetEnabledTo(True)
                    GridViewPrevT.SelectedIndex = 0
                    Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                    Try
                        Dim row As GridViewRow = GridViewPrevT.SelectedRow
                        Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                        BtnSetByStato(Stato) 'GIU010612
                    Catch ex As Exception
                        btnSblocca.Visible = False
                        BtnSetEnabledTo(False)
                        btnNuovo.Enabled = True
                        Session(IDDOCUMENTI) = ""
                    End Try
                Else
                    btnSblocca.Visible = False
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    Session(IDDOCUMENTI) = ""
                End If

                Session(SWOP) = SWOPNESSUNA
            Catch ex As Exception
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                'ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
                Chiudi("Errore: Caricamento Elenco ordini fornitori: " & ex.Message)
                Exit Sub
            End Try

        End If
        ModalPopup.WucElement = Me
        WFPMovMagDaCanc.WucElement = Me
        WFPEvasioneOF1.WucElement = Me
        '--
        If Session(F_SCELTAMOVMAG_APERTA) Then
            WFPMovMagDaCanc.Show()
        End If
        If Session(F_EVASIONEPARZ_APERTA) Then
            WFPEvasioneOF1.Show()
        End If
        '-
        WFPCambiaStatoOF.WucElement = Me
        If Session(F_CAMBIOSTATO_APERTA) Then
            WFPCambiaStatoOF.Show()
        End If
        'giu150222
        WFPDocCollegati.WucElement = Me
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
    End Sub
    'GIU010612
    Private Sub BtnSetByStato(ByVal myStato As String)
        'giu120612
        btnSblocca.Visible = False
        Dim SWBloccoModifica As Boolean = False
        Dim SWBloccoElimina As Boolean = False
        Dim SWBloccoCambiaStato As Boolean = False
        '--
        If myStato.Trim = "Da evadere" Then
            btnEliminaMM.Enabled = False
        End If
        If myStato.Trim = "Evaso" Then
            btnModifica.Enabled = False : SWBloccoModifica = True
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnCambiaStato.Enabled = False : SWBloccoCambiaStato = True
            btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
            btnEliminaMM.Enabled = True
            btnNuovaRev.Enabled = False
        End If
        If myStato.Trim = "Parz. evaso" Then
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnNuovaRev.Enabled = False
        End If
        If myStato.Trim = "Chiuso non evaso" Then
            btnModifica.Enabled = False
            btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
            btnEliminaMM.Enabled = False
            btnNuovaRev.Enabled = False
        End If
        If myStato.Trim = "Non evadibile" Then
            btnModifica.Enabled = False
            btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
            btnEliminaMM.Enabled = False
            btnNuovaRev.Enabled = False
        End If
        If myStato.Trim = "In Carico" Then
            btnModifica.Enabled = False : SWBloccoModifica = True
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
            btnEliminaMM.Enabled = False
            btnNuovaRev.Enabled = False
        End If
        If SWBloccoElimina Or SWBloccoModifica Or SWBloccoCambiaStato Then
            btnSblocca.Visible = True
        End If
    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnCambiaStato.Enabled = Valore
        btnNuovo.Enabled = Valore
        btnModifica.Enabled = Valore
        btnElimina.Enabled = Valore
        btnCaricoMag.Enabled = Valore
        btnCaricoMagParz.Enabled = Valore
        btnEliminaMM.Enabled = Valore
        btnStampa.Enabled = Valore
        btnCopia.Enabled = Valore
        btnNuovaRev.Enabled = Valore
    End Sub

#Region " Ordinamento e ricerca"
    Private Sub rbtnDaEvadere_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDaEvadere.CheckedChanged
        btnCaricoMag.Enabled = True : btnCaricoMagParz.Enabled = True
        btnEliminaMM.Enabled = False
        Session(CSTSTATODOC) = "0"
        BuildDett()
    End Sub
    Private Sub rbtnEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnEvaso.CheckedChanged
        btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
        btnEliminaMM.Enabled = True
        Session(CSTSTATODOC) = "1"
        BuildDett()
    End Sub
    Private Sub rbtnParzEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnParzEvaso.CheckedChanged
        btnCaricoMag.Enabled = True : btnCaricoMagParz.Enabled = True
        btnEliminaMM.Enabled = True
        Session(CSTSTATODOC) = "2"
        BuildDett()
    End Sub
    Private Sub rbtnChiusoNoEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnChiusoNoEvaso.CheckedChanged
        btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
        btnEliminaMM.Enabled = False
        Session(CSTSTATODOC) = "3"
        BuildDett()
    End Sub
    Private Sub rbtnNonEvadibile_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnNonEvadibile.CheckedChanged
        btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
        btnEliminaMM.Enabled = False
        Session(CSTSTATODOC) = "4"
        BuildDett()
    End Sub
    Private Sub rbtnInCarico_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnInCarico.CheckedChanged
        btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
        btnEliminaMM.Enabled = False
        Session(CSTSTATODOC) = "5"
        BuildDett()
    End Sub
    Private Sub rbtnInRitardo_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnInRitardo.CheckedChanged
        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        'ORDINATO PER DATA DI CONSEGNA
        ddlRicerca.SelectedValue = "C"
        Session(CSTSORTPREVTEL) = "C"
        '-----------------------------
        SqlDSPrevTElenco.DataBind()
        btnCaricoMag.Enabled = True : btnCaricoMagParz.Enabled = True
        btnEliminaMM.Enabled = False
        Session(CSTSTATODOC) = "999"
        BuildDett()
        SetFilter() '250612
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        btnNuovo.Enabled = True

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato) 'GIU010612
            Catch ex As Exception
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTutti.CheckedChanged
        btnCaricoMag.Enabled = True : btnCaricoMagParz.Enabled = True
        btnEliminaMM.Enabled = False
        Session(CSTSTATODOC) = "999"
        BuildDett()
    End Sub
    Private Sub SetFilter()
        'IN RITARDO
        If rbtnInRitardo.Checked Then
            If SqlDSPrevTElenco.FilterExpression.Trim <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression = "DataOraConsegna < '" & CDate(Format(Now, FormatoData)) & "' AND (StatoDoc=0 OR StatoDoc=2)"
        End If
    End Sub
    Private Sub BuildDett()
        SetFilter() 'GIU250612
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato) 'GIU010612
            Catch ex As Exception
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        btnSblocca.Visible = False
        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        Session(CSTSORTPREVTEL) = Left(ddlRicerca.SelectedValue.Trim, 1)
        SetFilter()
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
        btnNuovo.Enabled = True
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato) 'GIU010612
            Catch ex As Exception
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If

    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        btnSblocca.Visible = False
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or _
               ddlRicerca.SelectedValue = "V" Or _
               ddlRicerca.SelectedValue = "C" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        If txtRicerca.Text.Trim = "" Then
            SqlDSPrevTElenco.FilterExpression = ""
            SetFilter()
            SqlDSPrevTElenco.DataBind()

            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True

            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            GridViewPrevT.DataBind()
            If GridViewPrevT.Rows.Count > 0 Then
                BtnSetEnabledTo(True)
                GridViewPrevT.SelectedIndex = 0
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Try
                    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                    BtnSetByStato(Stato) 'GIU010612
                Catch ex As Exception
                    btnSblocca.Visible = False
                    BtnSetEnabledTo(False)
                    btnNuovo.Enabled = True
                    Session(IDDOCUMENTI) = ""
                End Try
            Else
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End If
            Exit Sub
        End If
        If ddlRicerca.SelectedValue = "N" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtRicerca.Text.Trim
            Else
                SqlDSPrevTElenco.FilterExpression = "Numero = " & txtRicerca.Text.Trim
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Data_Doc >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Data_Doc = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "R" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Rag_Soc >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "C" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "DataOraConsegna >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "DataOraConsegna = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "V" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Data_Validita >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Data_Validita = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "L" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Localita >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "M" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "CAP >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "S" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Denominazione >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "P" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Partita_IVA >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "F" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "NR" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Riferimento >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        End If
        SetFilter()
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
        btnNuovo.Enabled = True

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato) 'GIU010612
            Catch ex As Exception
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If

    End Sub
#End Region

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        'giu191111 GIU030212
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCR) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA
        '------------------------------------------
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini FORNITORI")
    End Sub

    Private Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovo.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewOF"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo ordine Fornitore", "Confermi la creazione dell'ordine Fornitore ? ", WUC_ModalPopup.TYPE_CONFIRM)

    End Sub
    Public Sub CreaNewOF()
        Session(SWOP) = SWOPNUOVO
        Session(IDDOCUMENTI) = ""
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini FORNITORI")
    End Sub
#Region "Carico magazzino"

    Private Sub btnCaricoMag_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCaricoMag.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "CaricoMag"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Carico magazzino", "Confermi l'evasione TOTALE dell'ordine selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CaricoMag()
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim NDoc As Long = GetNewCM() : Dim NRev As Integer = 0
        'OK CREAZIONE NUOVO CM
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
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
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocCM]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.CaricoMagazzino)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@Cod_Causale").Value = GetParamGestAzi(Session(ESERCIZIO)).causaleMMpos
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
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
            myID = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.: (" & myID.ToString.Trim & ")", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.: (" & myID.ToString.Trim & ")", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'OK FATTO
        'GIU271211
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino)
        Response.Redirect("WF_Documenti.aspx?labelForm=Movimenti di magazzino CARICO DI MAGAZZINO")
        
    End Sub
    Private Function GetNewCM() As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE Tipo_Doc = '" & SWTD(TD.CaricoMagazzino) & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewCM = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewCM = 1
                    End If
                    Exit Function
                Else
                    GetNewCM = 1
                    Exit Function
                End If
            Else
                GetNewCM = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewCM = -1
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function
#End Region

#Region "Carico Magazzino Parziale"
    'giu211211
    Private Sub btnCaricoMagParz_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCaricoMagParz.Click

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "CaricoMagParz"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Carico magazzino", "Confermi l'evasione PARZIALE dell'ordine selezionato ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CaricoMagParz()
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI) 'PER L'EVASIONE PARZIALE
       
        ApriEvasioneParz()
    End Sub
    Private Sub ApriEvasioneParz()
        WFPEvasioneOF1.WucElement = Me
        WFPEvasioneOF1.SetLblMessUtente("Seleziona/modifica Quantità articoli da caricare")
        Session(F_EVASIONEPARZ_APERTA) = True
        WFPEvasioneOF1.Show(True)
    End Sub
    Public Sub CallBackWFPEvasioneParzOF()
        Dim ListaDocD As New List(Of String)
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If

        ListaDocD = Session(L_EVASIONEPARZ_DA_CAR)
        If ListaDocD.Count > 0 Then
            Dim I As Integer
            Dim QtaDaEv As Decimal
            Dim NRiga As Integer
            Dim StrDato() As String
            Dim StrSql As String
            For I = 0 To ListaDocD.Count - 1
                Try
                    StrDato = ListaDocD(I).Split("|")
                    QtaDaEv = StrDato(0)
                    NRiga = StrDato(1)
                    StrSql = "Update DocumentiD Set Qta_Allestita = " & QtaDaEv.ToString.Replace(",", ".") & " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta_Allestita Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next

            Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
            Dim SqlConn As New SqlConnection
            SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
            'giu200617
            Dim strValore As String = ""
            'Dim strErrore As String = ""
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
            Dim SqlDbNewCmd As New SqlCommand
            Dim NDoc As Long = GetNewCM() : Dim NRev As Integer = 0

            SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocCMP]"
            SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbNewCmd.Connection = SqlConn
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
            SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
            '--
            SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
            SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
            SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.CaricoMagazzino)
            SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
            SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
            SqlDbNewCmd.Parameters.Item("@Cod_Causale").Value = GetParamGestAzi(Session(ESERCIZIO)).causaleMMpos
            SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
            SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
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
        Else 'giu080415
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Nessuna voce è stata selezionata per l'evasione.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(SWOP) = SWOPMODIFICA
        Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino)
        Response.Redirect("WF_Documenti.aspx?labelForm=Movimenti di magazzino CARICO DI MAGAZZINO")
    End Sub
#End Region

    Private Sub btnCopia_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopia.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "CreaCopia"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuovo Ordine (Copia ordine)", "Confermi la copia dell'ordine selezionato ?<br><strong><span> " _
                        & "Attenzione, Il tipo Pagamento sarà aggiornato a quello in essere.<br>Si prega di verificare i dati Pagamento.</span></strong>", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub CreaCopia()

        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0
        If CaricaParametri(Session(ESERCIZIO), StrErrore) Then
            NDoc = GetParamGestAzi(Session(ESERCIZIO)).NumeroOrdineFornitore + 1
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento parametri generali.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        If AggiornaNumDoc(SWTD(TD.OrdFornitori), NDoc, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'OK CREAZIONE NUOVO ORDINE
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
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
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDoc]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.OrdFornitori)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
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
            myID = Session(IDDOCUMENTI)
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
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'GIU181120 GIU191120 IMPOSTO A 'Non evadibile' cosi impongo l'aggiornamento dell'ordine appena copiato cosi se il tipo pagamento è stato cambiato aggiorna tutto
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "DECLARE @CodiceCG NVARCHAR(16)=NULL, @TipoPag INT=NULL"
            SQLStr += " SELECT @CodiceCG=ISNULL(Cod_Cliente,Cod_Fornitore) FROM DocumentiT WHERE IDDocumenti=" & myID.Trim
            SQLStr += " SELECT @TipoPag=Pagamento_N FROM CliFor WHERE Codice_Coge=@CodiceCG"
            SQLStr += " IF NOT @TipoPag IS NULL"
            SQLStr += " UPDATE DocumentiT SET Cod_Pagamento=@TipoPag, Totale=0,TotNettoPagare=0 WHERE IDDocumenti=" & myID.Trim
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Aggiornamento TIPO PAGAMENTO N° Doc.: (" & NDoc.ToString.Trim & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini FORNITORI")
    End Sub
    Private Sub btnNuovaRev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNuovaRev.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "NuovaRev"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Crea nuova Rev.Ordine", "Confermi la creazione nuova Rev. dell'ordine selezionato ? <br> Attenzione, la Rev.Ordine precedente <br> sarà ANNULLATO.", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub NuovaRev()

        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Dim NDoc As Long = 0 : Dim NRev As Integer = 0

        Dim row As GridViewRow = GridViewPrevT.SelectedRow
        Dim strNumero As String = row.Cells(CellIdxT.NumDoc).Text.Trim
        If strNumero.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Numero documento non valido.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        ElseIf Not IsNumeric(strNumero.Trim) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Numero documento non valido.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If

        If GetNewRevN(SWTD(TD.OrdFornitori), strNumero.Trim, NRev) = False Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' SEGNALATO NELLA FUNCTION ModalPopup.Show("Errore", "Aggiornamento numero documento", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        NDoc = CLng(strNumero.Trim)
        'OK CREAZIONE NUOVO ORDINE
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
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
        Dim SqlDbNewCmd As New SqlCommand

        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDoc]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.OrdFornitori)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
        SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
        SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Mid(Trim(Session(CSTUTENTE)) & " " & Format(Now, FormatoDataOra), 1, 50)
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
            myID = Session(IDDOCUMENTI)
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
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "NUOVO IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try

        'AGGIORNO LE REVISIONI PRECEDENTI DA STATO 0 A 3 CHIUSO NON EVASO
        Dim SWOk As Boolean = True : Dim SWOkT As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=3 WHERE IDDocumenti <> " & Session(IDDOCUMENTI).ToString.Trim
            SQLStr += " AND Tipo_Doc = '" & SWTD(TD.OrdFornitori) & "'"
            SQLStr += " AND Numero = '" & strNumero.Trim & "'"
            SQLStr += " AND StatoDoc = 0"
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore", "Errore durante la cancellazione movimenti N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            ' ''Exit Sub
        End Try
        ObjDB = Nothing

        'OK FATTO
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=Gestione ordini FORNITORI")
    End Sub
    Private Function GetNewRevN(ByVal TDoc As String, ByVal NDoc As String, ByRef NRev As Integer) As Boolean
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Function
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If myID = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select MAX(RevisioneNDoc) AS RevisioneNDoc From DocumentiT WHERE Tipo_Doc = '" & Session(CSTTIPODOC).ToString.Trim & "'"
        strSQL += " AND Numero = '" & NDoc.Trim & "'"
        ' ''strSQL += " AND RevisioneNDoc = " & txtRevNDoc.Text.Trim & ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim rowTes() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    rowTes = ds.Tables(0).Select()
                    NRev = IIf(IsDBNull(rowTes(0).Item("RevisioneNDoc")), 0, rowTes(0).Item("RevisioneNDoc"))
                    NRev += 1
                    GetNewRevN = True
                    Exit Function
                Else
                    NRev = 0
                    GetNewRevN = True
                    Exit Function
                End If
            Else
                NRev = 0
                GetNewRevN = True
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Verifica N.Documento/Rev.N° da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function

    Private Function AggiornaNumDoc(ByVal TDoc As String, ByVal NDoc As Long, ByVal NRev As Integer) As Boolean
        AggiornaNumDoc = True
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
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

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub

        Session(MODALPOPUP_CALLBACK_METHOD) = "OKElimina"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Elimina ordine Fornitore", "Confermi l'eliminazione dell'ordine Fornitore ? <br><strong><span> " & _
                        "Attenzione!! Saranno cancellati gli eventuali carichi ad esso collegati !!</span></strong> ", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub OKElimina()
        Session(SWOP) = SWOPELIMINA
        Response.Redirect("WF_Documenti.aspx?labelForm=Elimina Ordine a FORNITORE")
    End Sub

#Region "Movimenti da cancellare legati all'ordine"
    'giu211211
    Private Sub btnEliminaMM_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEliminaMM.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

        ApriSceltaMovMagDaCanc()
    End Sub
    Private Sub ApriSceltaMovMagDaCanc()
        'Parametro di sessione per la select dei movimenti
        Session(REFINTMOVMAG) = Session(IDDOCUMENTI)
        WFPMovMagDaCanc.WucElement = Me
        WFPMovMagDaCanc.SetLblMessUtente("Seleziona i movimenti da Eliminare")
        WFPMovMagDaCanc.PopolaGridView()
        Session(F_SCELTAMOVMAG_APERTA) = True
        WFPMovMagDaCanc.Show()
    End Sub
    Public Sub CallBackWFPMovMagDaCanc()
        Dim SWOk As Boolean = True : Dim SWOkT As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim listaMovMagDaCanc As List(Of String) = Session(MOVMAG_DA_CANC)
        If (listaMovMagDaCanc.Count > 0) Then
            Try
                For Each Codice As String In listaMovMagDaCanc
                    NumInt = Codice
                    SQLStr = "EXEC delete_DocTByIDDocumenti " & NumInt
                    SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
                    If SWOk = False Then Exit For
                Next
            Catch ex As Exception
                SWOk = False
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante la cancellazione movimenti N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        End If
        ObjDB = Nothing
        'giu030212
        btnCaricoMag.Enabled = True : btnCaricoMagParz.Enabled = True
        btnEliminaMM.Enabled = False
        BuildDett()
        '---------
        If SWOk = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante la cancellazione movimenti N° Interno (" & NumInt & ")", WUC_ModalPopup.TYPE_ERROR)
        ElseIf SWOkT = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Elimina Carico", "Operazione completata con successo.", WUC_ModalPopup.TYPE_INFO)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante il rispristino dell'ordine a fornitore.", WUC_ModalPopup.TYPE_ERROR)
        End If
        '--
    End Sub

    'giu020512 prova stampa tutti i movimenti
    Public Sub CallBackWFPMovMagDaStampare()
        'giu040512 tutti
        Dim Selezione As String = ""
        Dim Errore As String = ""
        Dim clsStampa As New Statistiche
        Dim dsMovMag1 As New DSMovMag
        Dim SWOk As Boolean = True : Dim NumInt As String = ""
        Dim listaMovMagDaCanc As List(Of String) = Session(MOVMAG_DA_CANC)
        If (listaMovMagDaCanc.Count > 0) Then
            Try
                For Each Codice As String In listaMovMagDaCanc
                    NumInt = Codice
                    Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti
                    SWOk = clsStampa.StampaMovMag(Session(CSTAZIENDARPT), "Riepilogo movimento di magazzino (Senza Lotti/N° Serie collegati)", Selezione, dsMovMag1, Errore, "", CLng(NumInt), "", False, "", "", False, False)
                    If SWOk = False Then Exit For
                Next
            Catch ex As Exception
                SWOk = False
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante la stampa movimenti N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else
            Exit Sub
        End If
        '
        If SWOk = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante la stampa movimenti N° Interno (" & NumInt & ") Err.: " & Errore.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(CSTDsPrinWebDoc) = dsMovMag1
        Session(ATTESA_CALLBACK_METHOD) = ""
        Session(CSTNOBACK) = 1
        Attesa.ShowStampaMovMagAll("Stampa movimenti", "Richiesta dell'apertura di una nuova pagina per la stampa.", WUC_Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebMovMag.aspx?labelForm=TUTTI")

        '--------------------------------------------------------------------------------
        'giu040512 versione richista stampa in altra pagina e conferma per ogni movimento
        '--------------------------------------------------------------------------------
        ' ''Dim Selezione As String = ""
        ' ''Dim Errore As String = ""
        ' ''Dim clsStampa As New Statistiche
        ' ''Dim dsMovMag1 As New DSMovMag
        ' ''Dim SWOk As Boolean = True : Dim NumInt As String = ""
        ' ''Dim listaMovMagDaCanc As List(Of String) = Session(MOVMAG_DA_CANC)
        ' ''If (listaMovMagDaCanc.Count > 0) Then
        ' ''    Try
        ' ''        For Each Codice As String In listaMovMagDaCanc
        ' ''            NumInt = Codice
        ' ''            Session(CSTTIPORPTMOVMAG) = TIPOSTAMPAMOVMAG.MovMagByIDDocumenti
        ' ''            SWOk = clsStampa.StampaMovMag(Session(CSTAZIENDARPT), "Riepilogo movimento di magazzino (Senza Lotti/N° Serie collegati)", Selezione, dsMovMag1, Errore, clng(NumInt), , False)
        ' ''            If SWOk = False Then Exit For
        ' ''            listaMovMagDaCanc.RemoveAt(0)
        ' ''            Session(MOVMAG_DA_CANC) = listaMovMagDaCanc
        ' ''            Exit For
        ' ''        Next
        ' ''    Catch ex As Exception
        ' ''        SWOk = False
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Errore", "Errore durante la stampa movimenti N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
        ' ''        Exit Sub
        ' ''    End Try
        ' ''Else
        ' ''    Exit Sub
        ' ''End If
        '' ''
        ' ''If SWOk = False Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", "Errore durante la stampa movimenti N° Interno (" & NumInt & ") Err.: " & Errore.Trim, WUC_ModalPopup.TYPE_ERROR)
        ' ''    Exit Sub
        ' ''End If
        ' ''Session(CSTDsPrinWebDoc) = dsMovMag1
        ' ''Session(ATTESA_CALLBACK_METHOD) = "CallBackWFPMovMagDaStampare"
        ' ''Session(CSTNOBACK) = 1
        ' ''Attesa.ShowStampaMovMag("Stampa movimenti N° Interno (" & NumInt & ")", "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebMovMag.aspx?" & NumInt.Trim)

        '-----------------------------------------------------------------------------
        'giu030512 ESEMPIO COME STAMPARE TUTTI I DOCUMENTI PERSONALIZZATI (DDT,FC,etc)
        '-----------------------------------------------------------------------------
        ' ''Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        ' ''Dim ObjReport As New Object
        ' ''Dim ClsPrint As New Documenti
        ' ''Dim StrErrore As String = ""
        ' ''Dim SWSconti As Boolean = False
        ' ''DsPrinWebDoc.Clear() 'GIU020512

        '' ''--------
        ' ''Dim SWOk As Boolean = True : Dim NumInt As String = ""
        ' ''Dim SQLStr As String = ""
        ' ''Dim ObjDB As New DataBaseUtility
        ' ''Dim listaMovMagDaCanc As List(Of String) = Session(MOVMAG_DA_CANC)
        ' ''If (listaMovMagDaCanc.Count > 0) Then
        ' ''    Try
        ' ''        For Each Codice As String In listaMovMagDaCanc
        ' ''            NumInt = Codice
        ' ''            SWOk = ClsPrint.StampaDocumentiDalAl(NumInt, TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore)
        ' ''            If SWOk = False Then Exit For
        ' ''        Next
        ' ''    Catch ex As Exception
        ' ''        SWOk = False
        ' ''        ObjDB = Nothing
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Errore", "Errore durante la stampa movimenti N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
        ' ''        Exit Sub
        ' ''    End Try
        ' ''End If
        ' ''ObjDB = Nothing
        '' ''---------

        '' ''---------
        ' ''If SWOk = False Then
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", "Errore durante la stampa movimenti N° Interno (" & NumInt & ")", WUC_ModalPopup.TYPE_ERROR)
        ' ''Else
        ' ''    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ' ''ModalPopup.Show("Stampa Carichi", "Operazione completata con successo.", WUC_ModalPopup.TYPE_INFO)
        ' ''    Try
        ' ''        Session(CSTObjReport) = ObjReport
        ' ''        Session(CSTDsPrinWebDoc) = DsPrinWebDoc
        ' ''        If SWSconti = True Then
        ' ''            Session(CSTSWScontiDoc) = 1
        ' ''        Else
        ' ''            Session(CSTSWScontiDoc) = 0
        ' ''        End If
        ' ''        Session(CSTSWConfermaDoc) = 0
        ' ''        Session(CSTNOBACK) = 0 'giu040512
        ' ''        Response.Redirect("..\WebFormTables\WF_PrintWebCR.aspx?TUTTI")
        ' ''    Catch ex As Exception
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''        ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        ' ''    End Try
        ' ''End If
        '' ''--
    End Sub
#End Region

#Region "Grid "
    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        SetFilter() 'GIU250612
        'giu110412
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato) 'GIU010612
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        SetFilter() 'GIU250612
        'giu110412
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato) 'GIU010612
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged

        Try
            BtnSetEnabledTo(True)
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            BtnSetByStato(Stato) 'GIU010612
        Catch ex As Exception
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End Try
    End Sub

    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
                e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataVal).Text) Then
                e.Row.Cells(CellIdxT.DataVal).Text = Format(CDate(e.Row.Cells(CellIdxT.DataVal).Text), FormatoData).ToString
            End If
        End If
    End Sub

    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxD.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxD.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxD.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScR).Text = ""
                End If
            End If
            'If IsDate(e.Row.Cells(6).Text) Then
            '    e.Row.Cells(6).Text = Format(CDate(e.Row.Cells(6).Text), FormatoData).ToString
            'End If
        End If
    End Sub
#End Region

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        If Session(SWOP) <> SWOPNESSUNA Then Exit Sub
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        If CKCSTTipoDoc() = False Then
            Chiudi("Errore: TIPO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebCR.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Public Sub Chiudi(ByVal strErrore As String)
     
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale " & strErrore)
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub
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
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
    End Function

    Private Sub btnSblocca_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSblocca.Click
        'giu080312
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

        If Not (sTipoUtente.Equals(CSTAMMINISTRATORE)) And _
            Not (sTipoUtente.Equals(CSTTECNICO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '----------------------------
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(MODALPOPUP_CALLBACK_METHOD) = "SbloccaDoc"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Sblocca Documento", "Confermi lo sblocco del documento ?", WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Public Sub SbloccaDoc()
        If CKCSTTipoDoc() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun Tipo documento selezionato tra quelli validi.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        btnSblocca.Visible = False
        'giu080312
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

        If (sTipoUtente.Equals(CSTTECNICO)) Or (sTipoUtente.Equals(CSTAMMINISTRATORE)) Then
            'giu230412
            ' ''If TipoDoc <> SWTD(TD.FatturaCommerciale) And TipoDoc <> SWTD(TD.FatturaAccompagnatoria) Then
            ' ''    If btnCreaFattura.Enabled = False Then btnCreaFattura.Enabled = True
            ' ''End If
            If btnModifica.Enabled = False Then btnModifica.Enabled = True
            If btnElimina.Enabled = False Then btnElimina.Enabled = True
            If btnCambiaStato.Enabled = False Then btnCambiaStato.Enabled = True
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

    End Sub
    'giu260612 CAMBIO STATO DOCUMENTO
    Private Sub btnCambiaStato_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCambiaStato.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu080312
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
        If (sTipoUtente.Equals(CSTTECNICO)) Or (sTipoUtente.Equals(CSTAMMINISTRATORE)) Then
            'ok PROCEDO
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        WFPCambiaStatoOF.WucElement = Me
        WFPCambiaStatoOF.SetLblMessUtente("Seleziona/modifica dati")
        Session(F_CAMBIOSTATO_APERTA) = True
        WFPCambiaStatoOF.Show(True)
    End Sub
    Public Sub CancBackWFPCambiaStatoOF()
        'nulla
    End Sub
    Public Sub CallBackWFPCambiaStatoOF()
        btnSblocca.Visible = False
        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        Session(CSTSORTPREVTEL) = Left(ddlRicerca.SelectedValue.Trim, 1)
        SetFilter()
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
        btnNuovo.Enabled = True
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato) 'GIU010612
            Catch ex As Exception
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
                btnNuovo.Enabled = True
                Session(IDDOCUMENTI) = ""
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            btnNuovo.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If

    End Sub

    Private Sub btnDocCollegati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocCollegati.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDOCUMCOLLCALL) = Session(IDDOCUMENTI) 'giu201221
        WFPDocCollegati.PopolaGrigliaWUCDocCollegati()
        ' ''WFPDocCollegati.WucElement = Me
        Session(F_DOCCOLL_APERTA) = True
        WFPDocCollegati.Show()
    End Sub
    Public Sub CancBackWFPDocCollegati()
        'GIU191219
        Session(CSTTIPODOC) = SWTD(TD.OrdFornitori)
        Session(CSTSORTPREVTEL) = "N"
        Session(SWOP) = SWOPNESSUNA
    End Sub

    Public Sub CallBackWFPDocCollegati()
        Session(CSTTIPODOC) = SWTD(TD.OrdFornitori)
        Session(CSTSORTPREVTEL) = "N"
        Session(SWOP) = SWOPNESSUNA
    End Sub
End Class