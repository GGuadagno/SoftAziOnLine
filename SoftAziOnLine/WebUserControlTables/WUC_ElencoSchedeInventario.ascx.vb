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


Partial Public Class WUC_ElencoSchedeInventario
    Inherits System.Web.UI.UserControl
    Private TipoDoc As String = "" : Private TabCliFor As String = ""

    Private Enum CellIdxT
        Stato = 1
        NumDoc = 2
        RevN = 3
        DataDoc = 4
        CreatoDa = 5
        ModificatoDa = 6
    End Enum
    Private Enum CellIdxD
        Riga = 0
        CodArt = 1
        DesArt = 2
        UM = 3
        Qta = 4
        QtaEv = 5
        IVA = 6
        Prz = 7
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        Attesa.WucElement = Me
        ModalPopup.WucElement = Me
        WFPMovMagDaCanc.WucElement = Me
        WFPModificaSchedaIN1.WucElement = Me

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
            btnElimina.Visible = False
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
        If (Not IsPostBack) Then
            Session(IDDOCUMSAVE) = 0 'PER L'EVASIONE PARZIALE
            Try
                btnSblocca.Text = "Sblocca scheda"
                btnSblocca.Visible = False
                '---
                rbtnDaEvadere.Checked = True
                Session(CSTTIPODOC) = SWTD(TD.Inventari)
                Session(CSTSTATODOC) = "0"

                ddlRicerca.Items.Add("N° Pagina")
                ddlRicerca.Items(0).Value = "P"
                ddlRicerca.Items.Add("N° Inventario")
                ddlRicerca.Items(1).Value = "N"
                ddlRicerca.Items.Add("Data inventario")
                ddlRicerca.Items(2).Value = "D"
                ddlRicerca.Items.Add("Creata da")
                ddlRicerca.Items(3).Value = "C"
                ddlRicerca.Items.Add("Modificata da")
                ddlRicerca.Items(4).Value = "M"

                btnModificaScheda.Text = "Modifica scheda inventario"
                btnChiudiScheda.Text = "Chiudi scheda (No inventario)"
                btnApriScheda.Text = "Apri scheda inventario"
                btnElimina.Text = "Elimina scheda inventario"
                btnCreaMMRettifica.Text = "Crea movimenti di rettifica"
                btnEliminaMM.Text = "Elimina movimenti di rettifica"
                btnStampa.Text = "Stampa scheda inventario"

                If GridViewPrevT.Rows.Count > 0 Then
                    BtnSetEnabledTo(True)
                    GridViewPrevT.SelectedIndex = 0
                    Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                    Try
                        Dim row As GridViewRow = GridViewPrevT.SelectedRow
                        Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                        BtnSetByStato(Stato)
                    Catch ex As Exception
                        BtnSetEnabledTo(False)
                    End Try
                Else
                    BtnSetEnabledTo(False)
                    Session(IDDOCUMENTI) = ""
                End If

                Session(SWOP) = SWOPNESSUNA
            Catch ex As Exception
                'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                'ModalPopup.Show("Errore", Err.Description, WUC_ModalPopup.TYPE_ERROR)
                Chiudi("Errore: Caricamento elenco schede inventario: " & ex.Message)
                Exit Sub
            End Try

        End If
       
        '--
        If Session(F_SCELTAMOVMAG_APERTA) Then
            WFPMovMagDaCanc.Show()
        End If
        If Session(F_MODIFICASCHEDAIN_APERTA) Then
            WFPModificaSchedaIN1.Show()
        End If
    End Sub
    '
    Private Sub BtnSetByStato(ByVal myStato As String)
        btnSblocca.Visible = False
        Dim SWBloccoModifica As Boolean = False
        Dim SWBloccoElimina As Boolean = False
        '--
        If myStato.Trim = "Da inventariare" Then
            btnCreaMMRettifica.Enabled = True
            btnEliminaMM.Enabled = False
            btnApriScheda.Enabled = False : btnChiudiScheda.Enabled = True
        End If
        If myStato.Trim = "Chiusa e aggiornato magazzino" Then
            btnChiudiScheda.Enabled = False : btnApriScheda.Enabled = False
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnModificaScheda.Enabled = False : SWBloccoModifica = True
            btnCreaMMRettifica.Enabled = False
            btnEliminaMM.Enabled = True
        End If
        If myStato.Trim = "Chiusa non inventariata" Then
            btnChiudiScheda.Enabled = False : btnApriScheda.Enabled = True
            btnModificaScheda.Enabled = False : SWBloccoModifica = True
            btnCreaMMRettifica.Enabled = False
            btnEliminaMM.Enabled = False
        End If
        If myStato.Trim = "In modifica" Then
            btnChiudiScheda.Enabled = False : btnApriScheda.Enabled = False : SWBloccoModifica = True
            btnElimina.Enabled = False : SWBloccoElimina = True
            btnModificaScheda.Enabled = False : SWBloccoModifica = True
            btnCreaMMRettifica.Enabled = False
            btnEliminaMM.Enabled = False : SWBloccoElimina = True
        End If
        If SWBloccoElimina Or SWBloccoModifica Then
            btnSblocca.Visible = True
        End If
    End Sub

    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnModificaScheda.Enabled = Valore
        btnChiudiScheda.Enabled = Valore
        btnApriScheda.Enabled = Valore
        btnElimina.Enabled = Valore
        btnCreaMMRettifica.Enabled = Valore
        btnEliminaMM.Enabled = Valore
        btnStampa.Enabled = Valore
    End Sub

#Region " Ordinamento e ricerca"
    Private Sub rbtnDaEvadere_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnDaEvadere.CheckedChanged
        btnModificaScheda.Enabled = True
        btnEliminaMM.Enabled = False
        btnChiudiScheda.Enabled = True
        btnApriScheda.Enabled = False
        Session(CSTSTATODOC) = "0"
        BuildDett()
    End Sub
    Private Sub rbtnEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnEvaso.CheckedChanged
        btnModificaScheda.Enabled = False
        btnEliminaMM.Enabled = True
        btnChiudiScheda.Enabled = False
        btnApriScheda.Enabled = False
        Session(CSTSTATODOC) = "1"
        BuildDett()
    End Sub
    'Private Sub rbtnParzEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    '    rbtnParzEvaso.CheckedChanged
    '    btnCaricoMag.Enabled = True : btnCaricoMagParz.Enabled = True
    '    btnEliminaMM.Enabled = True
    '    Session(CSTSTATODOC) = "2"
    '    BuildDett()
    'End Sub
    Private Sub rbtnChiusoNoEvaso_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnChiusoNoEvaso.CheckedChanged
        btnModificaScheda.Enabled = False
        btnEliminaMM.Enabled = False
        btnChiudiScheda.Enabled = False
        btnApriScheda.Enabled = True
        Session(CSTSTATODOC) = "3"
        BuildDett()
    End Sub
    'Private Sub rbtnNonEvadibile_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
    '    rbtnNonEvadibile.CheckedChanged
    '    btnCaricoMag.Enabled = False : btnCaricoMagParz.Enabled = False
    '    btnEliminaMM.Enabled = False
    '    Session(CSTSTATODOC) = "4"
    '    BuildDett()
    'End Sub
    Private Sub rbtnInCarico_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnInCarico.CheckedChanged
        btnModificaScheda.Enabled = False
        btnEliminaMM.Enabled = False
        btnChiudiScheda.Enabled = False
        btnApriScheda.Enabled = False
        Session(CSTSTATODOC) = "5"
        BuildDett()
    End Sub
    Private Sub rbtnTutti_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _
        rbtnTutti.CheckedChanged
        btnModificaScheda.Enabled = True
        btnEliminaMM.Enabled = False
        Session(CSTSTATODOC) = "999"
        BuildDett()
    End Sub
    Private Sub BuildDett()
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
            Catch ex As Exception
                BtnSetEnabledTo(False)
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        btnSblocca.Visible = False
        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
            Catch ex As Exception
                BtnSetEnabledTo(False)
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
        End If

    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        btnSblocca.Visible = False
        If ddlRicerca.SelectedValue = "N" Or ddlRicerca.SelectedValue = "P" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        If txtRicerca.Text.Trim = "" Then
            SqlDSPrevTElenco.FilterExpression = ""
            SqlDSPrevTElenco.DataBind()

            BtnSetEnabledTo(False)

            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            GridViewPrevT.DataBind()
            If GridViewPrevT.Rows.Count > 0 Then
                BtnSetEnabledTo(True)
                GridViewPrevT.SelectedIndex = 0
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Try
                    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                    BtnSetByStato(Stato)
                Catch ex As Exception
                    BtnSetEnabledTo(False)
                End Try
            Else
                btnSblocca.Visible = False
                BtnSetEnabledTo(False)
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
        ElseIf ddlRicerca.SelectedValue = "P" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "RevisioneNDoc >= " & txtRicerca.Text.Trim
            Else
                SqlDSPrevTElenco.FilterExpression = "RevisioneNDoc = " & txtRicerca.Text.Trim
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
        ElseIf ddlRicerca.SelectedValue = "C" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "InseritoDa like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "InseritoDa >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "M" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "ModificatoDa like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "ModificatoDa >= '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        End If
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True

        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
            Catch ex As Exception
               BtnSetEnabledTo(False)
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            ' ''ddlRicerca.Enabled = True : txtRicerca.Enabled = True : btnRicerca.Enabled = True
            Session(IDDOCUMENTI) = ""
        End If

    End Sub
#End Region

#Region "Modifica/Aggiorna/Elimina/Chiudi scheda"
    Private Sub btnChiudiScheda_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChiudiScheda.Click

        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsNumeric(myID.Trim) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu150612 BLOCCO LA SCHEDA CON STATO 5
        Dim myErrore As String = "" : Dim myUtente As String = "" : Dim myModificatoDa As String = ""
        Dim myStatoDoc As Integer = Documenti.CKStatoDoc(myID, myErrore, "", myModificatoDa, "", "", myUtente)
        If myStatoDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore controllo BLOCCO scheda", "Err.: " & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf myStatoDoc <> 0 Then
            If myUtente.Trim <> sUtente.Trim Then
                Dim myMess As String = "Codice Stato: " & myStatoDoc.ToString.Trim & " <br>"
                If myStatoDoc = 1 Then myMess += "Scheda Chiusa e aggiornato magazzino <br>"
                If myStatoDoc = 3 Then myMess += "Scheda Chiusa non inventariata <br>"
                If myStatoDoc = 5 Then myMess += "Modifica in corso <br>"
                If myModificatoDa.Trim <> "" Then myMess += myModificatoDa.Trim & " <br>"
                If myUtente.Trim <> "" Then myMess += myUtente.Trim & " <br>"
                myMess += myUtente.Trim & "Per accedere a questa scheda entrare con l'utente: " & myUtente.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione scheda bloccata", myMess, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '----------------------------------------
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=3, ChiusoNonEvaso=1, ModificatoDa='" & myModificaDA.Trim & "' WHERE IDDocumenti = " & myID.Trim
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore chiusura scheda", "Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        If SWOk = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Chiusura scheda", "Operazione fallita.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        BuildDett()
    End Sub
    Private Sub btnApriScheda_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApriScheda.Click
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsNumeric(myID.Trim) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu150612 BLOCCO LA SCHEDA CON STATO 5
        Dim myErrore As String = "" : Dim myUtente As String = "" : Dim myModificatoDa As String = ""
        Dim myStatoDoc As Integer = Documenti.CKStatoDoc(myID, myErrore, "", myModificatoDa, "", "", myUtente)
        If myStatoDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore controllo BLOCCO scheda", "Err.: " & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf myStatoDoc <> 3 Then
            If myUtente.Trim <> sUtente.Trim Then
                Dim myMess As String = "Codice Stato: " & myStatoDoc.ToString.Trim & " <br>"
                If myStatoDoc = 1 Then myMess += "Scheda Chiusa e aggiornato magazzino <br>"
                If myStatoDoc = 3 Then myMess += "Scheda Chiusa non inventariata <br>"
                If myStatoDoc = 5 Then myMess += "Modifica in corso <br>"
                If myModificatoDa.Trim <> "" Then myMess += myModificatoDa.Trim & " <br>"
                If myUtente.Trim <> "" Then myMess += myUtente.Trim & " <br>"
                myMess += myUtente.Trim & "Per accedere a questa scheda entrare con l'utente: " & myUtente.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione scheda bloccata", myMess, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '----------------------------------------
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=0, ChiusoNonEvaso=0, ModificatoDa='" & myModificaDA.Trim & "' WHERE IDDocumenti = " & myID.Trim
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore apri scheda", "Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        If SWOk = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Apri scheda", "Operazione fallita.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        BuildDett()
    End Sub

    Private Sub btnModificaScheda_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaScheda.Click
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsNumeric(myID.Trim) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI) 'forse non mi serve ma cmq lo salvo
        'giu150612 BLOCCO LA SCHEDA CON STATO 5
        Dim myErrore As String = "" : Dim myUtente As String = "" : Dim myModificatoDa As String = ""
        Dim myStatoDoc As Integer = Documenti.CKStatoDoc(myID, myErrore, "", myModificatoDa, "", "", myUtente)
        If myStatoDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore controllo BLOCCO scheda", "Err.: " & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf myStatoDoc <> 0 Then
            If myUtente.Trim <> sUtente.Trim Then
                Dim myMess As String = "Codice Stato: " & myStatoDoc.ToString.Trim & " <br>"
                If myStatoDoc = 1 Then myMess += "Scheda Chiusa e aggiornato magazzino <br>"
                If myStatoDoc = 3 Then myMess += "Scheda Chiusa non inventariata <br>"
                If myStatoDoc = 5 Then myMess += "Modifica in corso <br>"
                If myModificatoDa.Trim <> "" Then myMess += myModificatoDa.Trim & " <br>"
                If myUtente.Trim <> "" Then myMess += myUtente.Trim & " <br>"
                myMess += myUtente.Trim & "Per accedere a questa scheda entrare con l'utente: " & myUtente.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione scheda bloccata", myMess, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '----------------------------------------
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=5, " & _
            "ModificatoDa='" & myModificaDA.Trim & "', " & _
            "Operatore='" & sUtente.Trim & "' " & _
            "WHERE IDDocumenti = " & myID.Trim
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore BLOCCO scheda", "Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        If SWOk = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Blocco scheda per modifica", "Operazione fallita.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '---------------------------------------------
        WFPModificaSchedaIN1.WucElement = Me
        WFPModificaSchedaIN1.SetLblMessUtente("Modifica Quantità inventario")
        Session(F_MODIFICASCHEDAIN_APERTA) = True
        WFPModificaSchedaIN1.Show(True)
    End Sub

    Public Sub CallBackWFPModificaSchedaIN()
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)

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
        'giu150612 BLOCCO LA SCHEDA CON STATO 5
        Dim myErrore As String = "" : Dim myUtente As String = "" : Dim myModificatoDa As String = ""
        Dim myStatoDoc As Integer = Documenti.CKStatoDoc(myID, myErrore, "", myModificatoDa, "", "", myUtente)
        If myStatoDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore controllo BLOCCO scheda", "Err.: " & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf myStatoDoc <> 5 Then
            If myUtente.Trim <> sUtente.Trim Then
                Dim myMess As String = "Codice Stato: " & myStatoDoc.ToString.Trim & " <br>"
                If myStatoDoc = 1 Then myMess += "Scheda Chiusa e aggiornato magazzino <br>"
                If myStatoDoc = 3 Then myMess += "Scheda Chiusa non inventariata <br>"
                If myStatoDoc = 5 Then myMess += "Modifica in corso <br>"
                If myModificatoDa.Trim <> "" Then myMess += myModificatoDa.Trim & " <br>"
                If myUtente.Trim <> "" Then myMess += myUtente.Trim & " <br>"
                myMess += myUtente.Trim & "Per accedere a questa scheda entrare con l'utente: " & myUtente.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione scheda bloccata", myMess, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '----------------------------------------
        ListaDocD = Session(L_SCHEDAIN_DA_AGG)
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
                    StrSql = "Update DocumentiD Set Qta_Evasa = " & QtaDaEv.ToString.Replace(",", ".") & " Where IdDocumenti = " & Session(IDDOCUMENTI) & " And Riga = " & NRiga
                    ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
                Catch ex As Exception
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante l'aggiornamento della Qta' inventario Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End Try
            Next
            SqlDSPrevDByIDDocumenti.DataBind()
            GridViewPrevD.DataBind()

            Dim myNumero As String = "" : Dim myNRev As String = ""
            Dim myMess As String = "Confermi la chiusura della scheda inventario ? <br><strong><span> " & _
                            "!! Saranno creati movimenti di rettifica !!</span></strong> "
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            If myID.Trim <> Session(IDDOCUMENTI).ToString.Trim Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessuna scheda selezionata (N° ID diverso)", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                myNumero = row.Cells(CellIdxT.NumDoc).Text.Trim
                myNRev = row.Cells(CellIdxT.RevN).Text.Trim
            Catch ex As Exception
                myNumero = "" : myNRev = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End Try

            Session(MODALPOPUP_CALLBACK_METHOD) = "OKCreaMovimenti"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""

            ModalPopup.Show("Scheda aggiornata - Chiusura scheda inventario" & _
                            IIf(myNumero.Trim <> "", "<br>N° Inventario: " & myNumero.Trim, "") & _
                            IIf(myNRev.Trim <> "", " N° Pagina: " & myNRev.Trim, ""), myMess, WUC_ModalPopup.TYPE_CONFIRM)
            Exit Sub
        Else
            ChiudiSchedaIN(CLng(myID), myModificaDA)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Chiusura scheda eseguita", "Nessuna differenza inventario è stata riscontrata. Scheda chiusa Ok inventario.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If

    End Sub
    Public Sub OKCreaMovimenti()
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)
        '----
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
        '==========================
        Dim CodCoGeIN As String = "" : Dim StrErrore As String = ""
        If App.GetDatiAbilitazioni(CSTABILAZI, "CodCoGeIN", CodCoGeIN, StrErrore) = True Then
            If StrErrore.Trim <> "" Then
                CodCoGeIN = ""
            End If
        End If
        If String.IsNullOrEmpty(CodCoGeIN) Then
            CodCoGeIN = ""
        End If
        CodCoGeIN = CodCoGeIN.Trim
        If CodCoGeIN.Length > 16 Then
            CodCoGeIN = ""
        End If
        '==========================
        Dim SWOkDiff As Boolean = False
        Dim myErrore As String = ""
        myErrore = ""
        If Magazzino.GetPresenzaDiffIN(CLng(myID), "+", myErrore) = True Then
            If OKCreaDiffPositive(myID, CodCoGeIN, myModificaDA) = False Then
                Exit Sub
            End If
            SWOkDiff = True
        ElseIf myErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", myErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '--
        myErrore = ""
        If Magazzino.GetPresenzaDiffIN(CLng(myID), "-", myErrore) = True Then
            If OKCreaDiffNegative(myID, CodCoGeIN, myModificaDA) = False Then
                Exit Sub
            End If
            SWOkDiff = True
        ElseIf myErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", myErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        ChiudiSchedaIN(CLng(myID), myModificaDA)
        If SWOkDiff = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Chiusura scheda eseguita", "Nessuna differenza inventario è stata riscontrata. Scheda chiusa Ok inventario.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
    End Sub
    Private Function OKCreaDiffPositive(ByVal myID As String, ByVal CodCoGe As String, ByVal InseritoDa As String) As Boolean
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
        Dim SqlDbNewCmd As New SqlCommand
        Dim NDoc As Long = GetNewMM(SWTD(TD.CaricoMagazzino))
        If NDoc < 1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "(GetNewMM) Nuovo numero da impegnare per creare i movimenti di rettifica+.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocMMIN]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Cliente", System.Data.SqlDbType.NVarChar, 16))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Segno", System.Data.SqlDbType.NVarChar, 1))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.CaricoMagazzino)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@Cod_Cliente").Value = CodCoGe
        SqlDbNewCmd.Parameters.Item("@Cod_Causale").Value = GetParamGestAzi(Session(ESERCIZIO)).causaleMMpos
        SqlDbNewCmd.Parameters.Item("@Segno").Value = "+"
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
            Exit Function
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

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
            Exit Function
        End If
        OKCreaDiffPositive = True
        ' ''Session(SWOP) = SWOPMODIFICA
        ' ''Session(CSTTIPODOC) = SWTD(TD.CaricoMagazzino)
        ' ''Response.Redirect("WF_Documenti.aspx?labelForm=Movimenti di magazzino CARICO DI MAGAZZINO")
    End Function
    Private Function OKCreaDiffNegative(ByVal myID As String, ByVal CodCoGe As String, ByVal InseritoDa As String) As Boolean
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
        Dim SqlDbNewCmd As New SqlCommand
        Dim NDoc As Long = GetNewMM(SWTD(TD.ScaricoMagazzino))
        If NDoc < 1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "(GetNewMM) Nuovo numero da impegnare per creare i movimenti di rettifica-.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocMMIN]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocIN", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Cliente", System.Data.SqlDbType.NVarChar, 16))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Causale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Segno", System.Data.SqlDbType.NVarChar, 1))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        '--
        SqlDbNewCmd.Parameters.Item("@IDDocIN").Value = CLng(myID)
        SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value = -1
        SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.ScaricoMagazzino)
        SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
        SqlDbNewCmd.Parameters.Item("@Cod_Cliente").Value = CodCoGe
        SqlDbNewCmd.Parameters.Item("@Cod_Causale").Value = GetParamGestAzi(Session(ESERCIZIO)).causaleMMneg
        SqlDbNewCmd.Parameters.Item("@Segno").Value = "-"
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
            Exit Function
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

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
            Exit Function
        End If
        OKCreaDiffNegative = True
        ' ''Session(SWOP) = SWOPMODIFICA
        ' ''Session(CSTTIPODOC) = SWTD(TD.sCaricoMagazzino)
        ' ''Response.Redirect("WF_Documenti.aspx?labelForm=Movimenti di magazzino SCARICO DI MAGAZZINO")
    End Function
    Private Function GetNewMM(ByVal _TipoMM As String) As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE Tipo_Doc = '" & _TipoMM & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        GetNewMM = ds.Tables(0).Rows(0).Item("Numero") + 1
                    Else
                        GetNewMM = 1
                    End If
                    Exit Function
                Else
                    GetNewMM = 1
                    Exit Function
                End If
            Else
                GetNewMM = -1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewMM = -1
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function

    Public Sub CallBackWFPChiudiSchedaIN()
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsNumeric(myID.Trim) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDOCUMSAVE) = Session(IDDOCUMENTI) 'forse non mi serve ma cmq lo salvo
        'giu150612 BLOCCO LA SCHEDA CON STATO 5
        Dim myErrore As String = "" : Dim myUtente As String = "" : Dim myModificatoDa As String = ""
        Dim myStatoDoc As Integer = Documenti.CKStatoDoc(myID, myErrore, "", myModificatoDa, "", "", myUtente)
        If myStatoDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore controllo BLOCCO scheda", "Err.: " & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf myStatoDoc <> 5 Then
            If myUtente.Trim <> sUtente.Trim Then
                Dim myMess As String = "Codice Stato: " & myStatoDoc.ToString.Trim & " <br>"
                If myStatoDoc = 1 Then myMess += "Scheda Chiusa e aggiornato magazzino <br>"
                If myStatoDoc = 3 Then myMess += "Scheda Chiusa non inventariata <br>"
                If myStatoDoc = 5 Then myMess += "Modifica in corso <br>"
                If myModificatoDa.Trim <> "" Then myMess += myModificatoDa.Trim & " <br>"
                If myUtente.Trim <> "" Then myMess += myUtente.Trim & " <br>"
                myMess += myUtente.Trim & "Per accedere a questa scheda entrare con l'utente: " & myUtente.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione scheda bloccata", myMess, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '----------------------------------------
        If ChiudiSchedaIN(CLng(myID), myModificaDA) = True Then
            BuildDett()
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Chiusura scheda eseguita", "Nessuna differenza inventario è stata riscontrata. <br> Scheda chiusa Ok inventario.", WUC_ModalPopup.TYPE_INFO)
            BuildDett()
            Exit Sub
        End If
    End Sub

    Private Function ChiudiSchedaIN(ByVal _IDDoc As Long, ByVal _ModificaDa As String) As Boolean
        '-- OK POSSO CHIUDERE LA SCHEDA
        '----------------------------------------
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=1, ChiusoNonEvaso=0, ModificatoDa='" & _ModificaDa.Trim & "' WHERE IDDocumenti = " & _IDDoc.ToString.Trim
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore chiusura scheda", "Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        ObjDB = Nothing
        If SWOk = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Chiusura scheda", "Operazione fallita.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        ChiudiSchedaIN = True
        BuildDett()
    End Function

    Public Sub CallBackWFPEndModificaSchedaIN()
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsNumeric(myID.Trim) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        '-------------------
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)
        'giu150612 BLOCCO LA SCHEDA CON STATO 5
        Dim myErrore As String = "" : Dim myUtente As String = "" : Dim myModificatoDa As String = ""
        Dim myStatoDoc As Integer = Documenti.CKStatoDoc(myID, myErrore, "", myModificatoDa, "", "", myUtente)
        If myStatoDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore controllo BLOCCO scheda", "Err.: " & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf myStatoDoc <> 5 Then
            If myUtente.Trim <> sUtente.Trim Then
                Dim myMess As String = "Codice Stato: " & myStatoDoc.ToString.Trim & " <br>"
                If myStatoDoc = 1 Then myMess += "Scheda Chiusa e aggiornato magazzino <br>"
                If myStatoDoc = 3 Then myMess += "Scheda Chiusa non inventariata <br>"
                If myStatoDoc = 5 Then myMess += "Modifica in corso <br>"
                If myModificatoDa.Trim <> "" Then myMess += myModificatoDa.Trim & " <br>"
                If myUtente.Trim <> "" Then myMess += myUtente.Trim & " <br>"
                myMess += myUtente.Trim & "Per accedere a questa scheda entrare con l'utente: " & myUtente.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione scheda bloccata", myMess, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '----------------------------------------
        'giu150612 sBLOCCO LA SCHEDA CON STATO 0
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE DocumentiT SET StatoDoc=0, ModificatoDa='" & myModificaDA.Trim & "' WHERE IDDocumenti = " & myID.Trim
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
        Catch ex As Exception
            SWOk = False
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SBLOCCO scheda", "Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        If SWOk = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Sblocco scheda per modifica", "Operazione fallita.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '---------------------------------------------
        BuildDett()

    End Sub

    Private Sub btnElimina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnElimina.Click
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)
        '-----
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'giu150612 BLOCCO LA SCHEDA CON STATO 5
        Dim myErrore As String = "" : Dim myUtente As String = "" : Dim myModificatoDa As String = ""
        Dim myStatoDoc As Integer = Documenti.CKStatoDoc(myID, myErrore, "", myModificatoDa, "", "", myUtente)
        If myStatoDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore controllo BLOCCO scheda", "Err.: " & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf myStatoDoc <> 0 Then
            If myUtente.Trim <> sUtente.Trim Then
                Dim myMess As String = "Codice Stato: " & myStatoDoc.ToString.Trim & " <br>"
                If myStatoDoc = 1 Then myMess += "Scheda Chiusa e aggiornato magazzino <br>"
                If myStatoDoc = 3 Then myMess += "Scheda Chiusa non inventariata <br>"
                If myStatoDoc = 5 Then myMess += "Modifica in corso <br>"
                If myModificatoDa.Trim <> "" Then myMess += myModificatoDa.Trim & " <br>"
                If myUtente.Trim <> "" Then myMess += myUtente.Trim & " <br>"
                myMess += myUtente.Trim & "Per accedere a questa scheda entrare con l'utente: " & myUtente.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione scheda bloccata", myMess, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '----------------------------------------
        Dim myNumero As String = "" : Dim myNRev As String = ""
        Dim myMessElimina As String = "Confermi l'eliminazione della scheda inventario ? <br><strong><span> " & _
                        "Attenzione!! Saranno cancellati gli eventuali movimenti di rettifica ad essa collegati !!</span></strong> "
        Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
        If myID.Trim <> Session(IDDOCUMENTI).ToString.Trim Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata (N° ID diverso)", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Try
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            If Stato.Trim = "In modifica" Then
                myNumero = "" : myNRev = ""
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Impossibile eliminare la scheda. E' in stato di modifica.", WUC_ModalPopup.TYPE_ALERT)
                Exit Sub
            End If
            Dim _myMessEl As String = "Confermi l'eliminazione della scheda inventario ? "
            myNumero = row.Cells(CellIdxT.NumDoc).Text.Trim
            myNRev = row.Cells(CellIdxT.RevN).Text.Trim
            If SetMessElimina(Stato, _myMessEl) = True And _myMessEl.Trim <> "" Then
                myMessElimina = _myMessEl
            End If
        Catch ex As Exception
            myNumero = "" : myNRev = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End Try

        Session(MODALPOPUP_CALLBACK_METHOD) = "OKElimina"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""

        ModalPopup.Show("Elimina scheda inventario" & _
                        IIf(myNumero.Trim <> "", "<br>N° Inventario: " & myNumero.Trim, "") & _
                        IIf(myNRev.Trim <> "", " N° Pagina: " & myNRev.Trim, ""), myMessElimina, WUC_ModalPopup.TYPE_CONFIRM)
    End Sub
    Private Function SetMessElimina(ByVal myStato As String, ByRef _MessElimina As String) As Boolean
        If myStato.Trim = "Da inventariare" Then Return True
        If myStato.Trim = "Chiusa e aggiornato magazzino" Then
            _MessElimina += " <br><strong><span> " & _
            "Attenzione!! Saranno cancellati gli eventuali movimenti di rettifica ad essa collegati !!</span></strong> "
            Return True
        End If
        If myStato.Trim = "Chiusa non inventariata" Then Return True
    End Function
    Public Sub OKElimina()
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)
        '-----
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If Not IsNumeric(myID.Trim) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu150612 BLOCCO LA SCHEDA CON STATO 5
        Dim myErrore As String = "" : Dim myUtente As String = "" : Dim myModificatoDa As String = ""
        Dim myStatoDoc As Integer = Documenti.CKStatoDoc(myID, myErrore, "", myModificatoDa, "", "", myUtente)
        If myStatoDoc = -1 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore controllo BLOCCO scheda", "Err.: " & myErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        ElseIf myStatoDoc <> 0 Then
            If myUtente.Trim <> sUtente.Trim Then
                Dim myMess As String = "Codice Stato: " & myStatoDoc.ToString.Trim & " <br>"
                If myStatoDoc = 1 Then myMess += "Scheda Chiusa e aggiornato magazzino <br>"
                If myStatoDoc = 3 Then myMess += "Scheda Chiusa non inventariata <br>"
                If myStatoDoc = 5 Then myMess += "Modifica in corso <br>"
                If myModificatoDa.Trim <> "" Then myMess += myModificatoDa.Trim & " <br>"
                If myUtente.Trim <> "" Then myMess += myUtente.Trim & " <br>"
                myMess += myUtente.Trim & "Per accedere a questa scheda entrare con l'utente: " & myUtente.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione scheda bloccata", myMess, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '----------------------------------------
        Dim SWOk As Boolean = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        SQLStr = "EXEC delete_DocTByIDDocumenti " & myID.Trim
        Try
            SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, SQLStr)
            If SWOk = False Then
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante la cancellazione.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        Catch ex As Exception
            ObjDB = Nothing
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante la cancellazione. Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        ObjDB = Nothing
        BuildDett()
    End Sub

#End Region

#Region "Movimenti da cancellare legati alla scheda"
    'giu211211
    Private Sub btnEliminaMM_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEliminaMM.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
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
        '
        btnModificaScheda.Enabled = True
        btnCreaMMRettifica.Enabled = True
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
            ModalPopup.Show("Elimina movimenti di rettifica", "Operazione completata con successo.", WUC_ModalPopup.TYPE_INFO)
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore durante la cancellazione dei movimenti di rettifica.", WUC_ModalPopup.TYPE_ERROR)
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
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                BtnSetEnabledTo(False)
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
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
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                BtnSetEnabledTo(False)
            End Try
        Else
            btnSblocca.Visible = False
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged

        Try
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            BtnSetByStato(Stato) 'GIU010612
        Catch ex As Exception
            BtnSetEnabledTo(False)
        End Try
    End Sub

    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
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
            ' ''If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), 0).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxD.QtaRe).Text = ""
            ' ''    End If
            ' ''End If
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
        End If
    End Sub
#End Region

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click

        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna scheda selezionata", WUC_ModalPopup.TYPE_ALERT)
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

        Try
            DsPrinWebDoc.Clear()
            If ClsPrint.StampaSchedaIN(Session(IDDOCUMENTI), DsPrinWebDoc, ObjReport, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTNOBACK) = 0
                Session(CSTTipoStampaIN) = 1
                Response.Redirect("..\WebFormTables\WF_PrintWeb_IN.aspx?labelForm=" & Session(IDDOCUMENTI).ToString.Trim)
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
            btnModificaScheda.Enabled = True
            btnElimina.Enabled = True
            btnEliminaMM.Enabled = True
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If

    End Sub

End Class