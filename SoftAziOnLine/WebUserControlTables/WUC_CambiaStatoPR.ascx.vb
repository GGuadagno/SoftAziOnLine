Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_CambiaStatoPR
    Inherits System.Web.UI.UserControl

#Region "Variabili private"

    Private _WucElement As Object

#End Region

#Region "Property"

    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

#End Region

#Region "Metodi private - eventi"
    Private Enum CellIdxT
        Stato = 0
        NumDoc = 1
        Tipo = 2
        DataDoc = 3
        DataCons = 4
        CodCliForProvv = 5
        RagSoc = 6
        Denominazione = 7
        Localita = 8
        CAP = 9
        PIVA = 10
        CF = 11
        Riferimento = 12
    End Enum

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSDocT.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSCausNonEvasione.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        lblChiusoNonEvaso.BackColor = SEGNALA_OK
    End Sub
    Private Sub GridViewDocT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDocT.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
                e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            End If
        End If
    End Sub

#End Region

#Region "Metodi public"

    Public Function AggiornaDati() As Boolean
        Dim sUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Function
            End If
            sUtente = Utente.NomeOperatore
        Else
            sUtente = Session(CSTUTENTE)
        End If
        Dim myModificaDA As String = Mid(Trim(sUtente) & " " & Format(Now, FormatoDataOra), 1, 50)
        '---
        Dim ClsDBUt As New DataBaseUtility
        Dim StrErrore As String = ""
        Dim myID As String = Session(IDDOCCAMBIAST)
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
            Exit Function
        End If
        '--Stato
        Dim myStatoDoc As Integer = -1 : Dim SWChiuso As Boolean = False
        If rbtnEvaso.Checked Then myStatoDoc = 1
        If rbtnDaEvadere.Checked Then myStatoDoc = 0
        If rbtnParzEvaso.Checked Then myStatoDoc = 2
        If rbtnChiusoNoEvaso.Checked Then myStatoDoc = 3 : SWChiuso = True
        If SWChiuso = False Then SWChiuso = IIf(DDLCausNonEvasione.SelectedIndex > 0, True, False)
        If rbtnChiusoNoEvaso.Checked = False And SWChiuso = True Then
            rbtnChiusoNoEvaso.Checked = True : myStatoDoc = 3
        End If
        If rbtnNonEvadibile.Checked Then myStatoDoc = 4
        If rbtnInAllestimento.Checked Then myStatoDoc = 5
        If rbtnEvaso.Checked Or rbtnDaEvadere.Checked Or rbtnParzEvaso.Checked Or rbtnInAllestimento.Checked Then
            SWChiuso = False
            DDLCausNonEvasione.SelectedIndex = 0
            txtNoteNonEvasione.Text = ""
        End If
        If myStatoDoc = -1 Then
            AggiornaDati = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Nessun stato valido selezionato", "Nessun aggiornamento effettuato.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End If
        Dim StrSql As String
        Try
            StrSql = "Update DocumentiT Set StatoDoc=" & myStatoDoc & _
            ", Note='" & Controlla_Apice(txtNote.Text.Trim) & "'" & _
            ", ChiusoNonEvaso=" & IIf(SWChiuso, 1, 0) & _
            ", CodNonevasione=" & IIf(DDLCausNonEvasione.SelectedIndex > 0, DDLCausNonEvasione.SelectedValue, 0) & _
            IIf(SWChiuso, ", DataOraChiusoNonEvaso=GETDATE()", "") & _
            ", NoteNonEvasione='" & Controlla_Apice(txtNoteNonEvasione.Text.Trim) & "'" & _
            ", ModificatoDa='" & Controlla_Apice(myModificaDA.Trim) & "'" & _
            " Where IdDocumenti = " & Session(IDDOCCAMBIAST)
            ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, StrSql)
            AggiornaDati = True
        Catch ex As Exception
            AggiornaDati = False
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore aggiorna dati", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
    End Function

    Public Sub PrimaVolta()
        SetEnabledAll(True)
        DDLCausNonEvasione.SelectedIndex = 0
        rbtnEvaso.Checked = False
        rbtnDaEvadere.Checked = False
        rbtnParzEvaso.Checked = False
        rbtnChiusoNoEvaso.Checked = False
        rbtnNonEvadibile.Checked = False
        rbtnInAllestimento.Checked = False
        Dim dvDocT As DataView
        dvDocT = SqlDSDocT.Select(DataSourceSelectArguments.Empty)
        If (dvDocT Is Nothing) Then
            lblCambioStatoNote.BackColor = SEGNALA_KO
            lblCambioStatoNote.Text = "NESSUN DOCUMENTO SELEZIONATO"
            SetEnabledAll(False)
            Exit Sub
        Else
            lblCambioStatoNote.BackColor = SEGNALA_OK
            lblCambioStatoNote.Text = "CAMBIO STATO - NOTE VARIE DOCUMENTO"
        End If
        If dvDocT.Count > 0 Then
            lblModificatoDa.Text = "Ultima modifica effettuata da: " & IIf(IsDBNull(dvDocT.Item(0).Item("ModificatoDa").ToString.Trim), "[Nessuno]", dvDocT.Item(0).Item("ModificatoDa").ToString.Trim)
            Dim myTipoDoc As String = IIf(IsDBNull(dvDocT.Item(0).Item("Tipo_Doc")), "", dvDocT.Item(0).Item("Tipo_Doc"))
            If Left(myTipoDoc, 1) <> "P" Then

                Select Case myTipoDoc
                    Case "DT", "DF", "BC"
                        rbtnDaEvadere.Text = "Da Fatturare"
                        rbtnEvaso.Text = "Fatturato"
                        rbtnChiusoNoEvaso.Visible = False
                        rbtnInAllestimento.Visible = False
                        rbtnInAllestimento.Visible = False
                        rbtnNonEvadibile.Visible = False
                        rbtnParzEvaso.Visible = False
                    Case "FC", "FA", "FS", "NC", "NZ"
                        rbtnDaEvadere.Text = "Da trasf.in CoGe"
                        rbtnEvaso.Text = "OK trasf.in CoGe"
                        rbtnChiusoNoEvaso.Visible = False
                        rbtnInAllestimento.Visible = False
                        rbtnInAllestimento.Visible = False
                        rbtnNonEvadibile.Visible = False
                        rbtnParzEvaso.Visible = False
                    Case "CM", "SM", "MM"
                        rbtnDaEvadere.Text = "Stato Valido"
                        rbtnEvaso.Visible = False
                        rbtnChiusoNoEvaso.Visible = False
                        rbtnInAllestimento.Visible = False
                        rbtnInAllestimento.Visible = False
                        rbtnNonEvadibile.Visible = False
                        rbtnParzEvaso.Visible = False
                    Case Else

                End Select

            End If
            If dvDocT.Item(0).Item("ChiusoNonEvaso") <> 0 Then
                rbtnChiusoNoEvaso.Checked = True
            ElseIf dvDocT.Item(0).Item("StatoDoc") = 0 Then
                rbtnDaEvadere.Checked = True
            ElseIf dvDocT.Item(0).Item("StatoDoc") = 1 Then
                rbtnEvaso.Checked = True
                If CKRefInt("") = True Then
                    SetEnabledAll(False)
                End If
            ElseIf dvDocT.Item(0).Item("StatoDoc") = 2 Then
                rbtnParzEvaso.Checked = True
                SetStato2()
            ElseIf dvDocT.Item(0).Item("StatoDoc") = 3 Then
                rbtnChiusoNoEvaso.Checked = True
            ElseIf dvDocT.Item(0).Item("StatoDoc") = 4 Then
                rbtnNonEvadibile.Checked = True
            ElseIf dvDocT.Item(0).Item("StatoDoc") = 5 Then
                rbtnInAllestimento.Checked = True
                SetEnabledAll(False)
            End If
            txtNote.Text = dvDocT.Item(0).Item("Note").ToString.Trim
            '--
            PosizionaItemDDL(dvDocT.Item(0).Item("CodNonevasione").ToString, DDLCausNonEvasione)
            If dvDocT.Item(0).Item("DataOraChiusoNonEvaso").ToString = DATANULL Then
                lblDataOraChiusoNonEvaso.Text = ""
            ElseIf IsDate(dvDocT.Item(0).Item("DataOraChiusoNonEvaso")) Then
                lblDataOraChiusoNonEvaso.Text = Format(dvDocT.Item(0).Item("DataOraChiusoNonEvaso"), FormatoDataOra)
            Else
                lblDataOraChiusoNonEvaso.Text = ""
            End If
            txtNoteNonEvasione.Text = dvDocT.Item(0).Item("NoteNonEvasione").ToString.Trim
        Else
            lblCambioStatoNote.BackColor = SEGNALA_KO
            lblCambioStatoNote.Text = "NESSUN DOCUMENTO SELEZIONATO"
            SetEnabledAll(False)
            Exit Sub
        End If
    End Sub

    Private Sub SetEnabledAll(ByVal SW As Boolean)
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
        If (sTipoUtente.Equals(CSTTECNICO)) Then 'Or (sTipoUtente.Equals(CSTAMMINISTRATORE)) Then
            'ok PROCEDO
            If SW = False Then SW = True
        End If
        rbtnEvaso.Enabled = SW
        rbtnDaEvadere.Enabled = SW
        rbtnParzEvaso.Enabled = SW
        rbtnChiusoNoEvaso.Enabled = SW
        rbtnNonEvadibile.Enabled = SW
        rbtnInAllestimento.Enabled = False
        txtNote.Enabled = SW
        DDLCausNonEvasione.Enabled = SW
        txtNoteNonEvasione.Enabled = SW
    End Sub

    Private Sub SetStato2()
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
        If (sTipoUtente.Equals(CSTTECNICO)) Then 'Or (sTipoUtente.Equals(CSTAMMINISTRATORE)) Then
            'ok LASCIO TUTTO ABILITATO
            Exit Sub
        End If
        rbtnEvaso.Enabled = True
        rbtnDaEvadere.Enabled = False
        rbtnParzEvaso.Enabled = True
        rbtnChiusoNoEvaso.Enabled = False
        rbtnNonEvadibile.Enabled = False
        rbtnInAllestimento.Enabled = False
        txtNote.Enabled = True
        DDLCausNonEvasione.Enabled = False
        txtNoteNonEvasione.Enabled = False
    End Sub

    Private Function CKRefInt(ByRef _strErrore As String) As Boolean
        Dim myID As String = Session(IDDOCCAMBIAST)
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
            Exit Function
        End If
        _strErrore = ""
        Dim strSQL As String = ""
        strSQL = "Select TOP 1 IDDocumenti From DocumentiT Where RefInt = " & Session(IDDOCCAMBIAST)
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CKRefInt = True
                Else
                    CKRefInt = False
                End If
            Else
                CKRefInt = False
            End If
        Catch Ex As Exception
            CKRefInt = False
            _strErrore = "Errore Verifica CKRefInt: " & Ex.Message.Trim
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", _strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

    End Function
#End Region

End Class