Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports It.SoftAzi.Model.Entity

Partial Public Class WUC_Anagrafiche_ModifySint
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property
    Public Sub PopolaCampi()
        Dim Rk As StrAnagrCliFor
        rk = Session(RKANAGRCLIFOR)
        If IsNothing(Rk.Rag_Soc) Then
            lblRagSoc.BackColor = SEGNALA_KO
            lblIndirizzo.BackColor = SEGNALA_KO
            lblLocalita.BackColor = SEGNALA_KO
            Exit Sub
        End If
        txtEmail.AutoPostBack = False
        txtEmailInvioScad.AutoPostBack = False
        txtEmailDest.AutoPostBack = False

        lblCodice.Text = Session(IDANAGRCLIFOR)
        lblRagSoc.Text = Rk.Rag_Soc.Trim
        lblDenominazione.Text = Rk.Denominazione.Trim
        lblIndirizzo.Text = Rk.Indirizzo.Trim + " " + Rk.NumeroCivico.Trim
        lblLocalita.Text = Rk.Localita.Trim + " " + IIf(Rk.Provincia.Trim <> "", "(" + Rk.Provincia.Trim + ")", "")
        txtEmail.Text = Rk.EMail.Trim

        'Sim180518
        If Not IsNothing(Rk.EMailInvioScad) Then
            txtEmailInvioScad.Text = Rk.EMailInvioScad.Trim
        Else
            txtEmailInvioScad.Text = ""
        End If

        chkInvioMailScad.AutoPostBack = False
        If Not IsNothing(Rk.InvioMailScad) Then
            chkInvioMailScad.Checked = Rk.InvioMailScad
        Else
            chkInvioMailScad.Checked = False
        End If
        chkChiudiEmail.Checked = False
        chkChiudiEmail.Visible = False
        chkApriEmail.Checked = False
        chkApriEmail.Visible = False
        chkInvioMailScad.AutoPostBack = True

        'BLOCCO DESTINAZIONE CLIENTE 
        If Not (Session(RKANAGRDESTCLI) Is Nothing) Then
            Dim Rk1 As DestCliForEntity
            Rk1 = Session(RKANAGRDESTCLI)

            lblCodFiliale.Text = Rk1.Progressivo.Trim
            lblRagSocDest.Text = Rk1.Ragione_Sociale.Trim
            lblIndirizzoDest.Text = Rk1.Indirizzo.Trim
            lblLocalitaDest.Text = Rk1.Localita.Trim + " " + IIf(Rk1.Provincia.Trim <> "", "(" + Rk1.Provincia.Trim + ")", "")
            txtEmailDest.Text = Rk1.EMail.Trim
        Else
            lblCodFiliale.Text = ""
            lblRagSocDest.Text = ""
            lblIndirizzoDest.Text = ""
            lblLocalitaDest.Text = ""
            txtEmailDest.Text = ""
        End If
        '---
        'giu070918
        chkAggEmailT.Checked = False
        txtEmail.AutoPostBack = True
        txtEmailInvioScad.AutoPostBack = True
        txtEmailDest.AutoPostBack = True
        Call SetLblEmailInvio()
    End Sub
    Private Sub SetLblEmailInvio()
        If txtEmailDest.Text.Trim = "" Then
            If txtEmailInvioScad.Text.Trim = "" Then
                lblEmailInvio.Text = txtEmail.Text
            Else
                lblEmailInvio.Text = txtEmailInvioScad.Text
            End If
        Else
            lblEmailInvio.Text = txtEmailDest.Text
        End If
    End Sub
    Public Sub Show()
        'Sim180518
        Dim myIDCOGE As String = Session(CSTCODCOGE)
        If Not IsNothing(myIDCOGE) Then
            If Left(myIDCOGE.Trim, 1) = "9" Then    'se fornitore disabilito txtEmailInvioScad
                txtEmailInvioScad.Enabled = False
            Else
                txtEmailInvioScad.Enabled = True
            End If
        End If

        'se destinazione non presente disabilito txtEmailDest
        If (Session(RKANAGRDESTCLI) Is Nothing) Then
            txtEmailDest.Enabled = False
        Else
            txtEmailDest.Enabled = True
        End If
        '---

        ProgrammaticModalPopup.Show()
    End Sub

    Protected Sub OkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ProgrammaticModalPopup.Hide()

        'GIU030918
        Dim ClsDB As New DataBaseUtility
        Dim LogInvioEmail As String = ClsDB.GetLogInvioEmail(Session(ESERCIZIO))
        If Mid(LogInvioEmail.Trim, 1, 20) = "INVIO EMAIL IN CORSO" Then
            ProgrammaticModalPopup.Show()
            lblMess.Text = "Attenzione, INVIO EMAIL IN CORSO ... Attendere il termine dell'invio e riprovare."
            Exit Sub
        ElseIf UCase(Mid(LogInvioEmail.Trim, 1, 6)) = "ERRORE" Then
            ProgrammaticModalPopup.Show()
            lblMess.Text = "Errore, lettura Parametri generali: " & LogInvioEmail.Trim
            Exit Sub
        End If

        'SIMONE250518--- EFFETTUO UPDATE DIRETTAMENTE DA QUI, DOPO AVER DATO L'OK
        Dim Email As String = txtEmail.Text.Trim
        Dim EmailInvioScad As String = txtEmailInvioScad.Text.Trim
        Dim EmailDest As String = txtEmailDest.Text.Trim

        Dim rk As StrAnagrCliFor
        rk = Session(RKANAGRCLIFOR)
        Dim rk1 As DestCliForEntity
        rk1 = Session(RKANAGRDESTCLI)

        If IsNothing(rk.Rag_Soc) Then
            Exit Sub
        End If

        Dim myIDCOGE As String = Session(CSTCODCOGE)
        If IsNothing(myIDCOGE) Then
            myIDCOGE = ""
            ProgrammaticModalPopup.Show()
            lblMess.Text = "Nessuna anagrafica selezionata"
            Exit Sub
        End If
        If String.IsNullOrEmpty(myIDCOGE) Then
            myIDCOGE = ""
            ProgrammaticModalPopup.Show()
            lblMess.Text = "Nessuna anagrafica selezionata"
            Exit Sub
        End If
        If Not IsNumeric(myIDCOGE.Trim) Then
            ProgrammaticModalPopup.Show()
            lblMess.Text = "Codice anagrafica non valido"
            Exit Sub
        End If

        Dim SQLStr As String = ""
        If (rk.EMail.Trim = Email.Trim) And (rk.EMailInvioScad.Trim = EmailInvioScad.Trim) And _
               (rk.InvioMailScad = chkInvioMailScad.Checked) And _
               (chkChiudiEmail.Checked = False And chkApriEmail.Checked = False And chkAggEmailT.Checked = False) Then
            'nessun dato aggiornato 
        Else
            Try
                If Left(myIDCOGE.Trim, 1) = "1" Then
                    SQLStr = "UPDATE Clienti SET EMail = '" & Controlla_Apice(Email.Trim) & "', EmailInvioScad = '" & Controlla_Apice(EmailInvioScad.Trim) & "', InvioMailScad=" & IIf(chkInvioMailScad.Checked, 1, 0) & " WHERE Codice_CoGe= '" & myIDCOGE.Trim & "'"
                ElseIf Left(myIDCOGE.Trim, 1) = "9" Then
                    SQLStr = "UPDATE Fornitori SET EMail = '" & Controlla_Apice(Email.Trim) & "' WHERE Codice_CoGe= '" & myIDCOGE.Trim & "'"
                Else
                    SQLStr = "UPDATE Clienti SET EMail = '" & Controlla_Apice(Email.Trim) & "', EmailInvioScad = '" & Controlla_Apice(EmailInvioScad.Trim) & "', InvioMailScad=" & IIf(chkInvioMailScad.Checked, 1, 0) & "  WHERE Codice_CoGe= '" & myIDCOGE.Trim & "'"
                End If
                If ClsDB.ExecuteQueryUpdate(TipoDB.dbSoftCoge, SQLStr) = False Then
                    ProgrammaticModalPopup.Show()
                    lblMess.Text = "Aggiornamento anagrafica non riuscito."
                    Exit Sub
                End If
                'giu070918 ATTENZIONE LO STATO DI ANNULLAMENTO E' 9 @@@@@@@@@@@@@@@@@@@@@@
                If chkInvioMailScad.Checked = False And chkChiudiEmail.Checked = True Then
                    SQLStr = "UPDATE EmailInviateT SET Stato=9, OLDStato=Stato "
                    If chkAggEmailT.Checked = True And lblEmailInvio.Text.Trim <> "" Then
                        SQLStr += ", Email='" & Controlla_Apice(lblEmailInvio.Text.Trim) & "' "
                    End If
                    SQLStr += "FROM EmailInviateT INNER JOIN " & _
                             "EmailInviateDett ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN " & _
                             "ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID " & _
                             "WHERE (ArticoliInst_ContrattiAss.Cod_Coge = N'" & myIDCOGE.Trim & "') AND " & _
                             "      (EmailInviateT.Stato<9)"
                    If ClsDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                        ProgrammaticModalPopup.Show()
                        lblMess.Text = "Aggiornamento E-mail: Chiusura."
                        Exit Sub
                    End If
                ElseIf chkInvioMailScad.Checked = True And chkApriEmail.Checked = True Then
                    SQLStr = "UPDATE EmailInviateT SET Stato=OLDStato, OLDStato=Stato "
                    If chkAggEmailT.Checked = True And lblEmailInvio.Text.Trim <> "" Then
                        SQLStr += ", Email='" & Controlla_Apice(lblEmailInvio.Text.Trim) & "' "
                    End If
                    SQLStr += "FROM EmailInviateT INNER JOIN " & _
                             "EmailInviateDett ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN " & _
                             "ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID " & _
                             "WHERE (ArticoliInst_ContrattiAss.Cod_Coge = N'" & myIDCOGE.Trim & "') AND " & _
                             "      (EmailInviateT.Stato=9)"
                    If ClsDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                        ProgrammaticModalPopup.Show()
                        lblMess.Text = "Aggiornamento E-mail: Apertura."
                        Exit Sub
                    End If
                ElseIf chkAggEmailT.Checked = True And lblEmailInvio.Text.Trim <> "" Then 'aggiorno E-mail inviate
                    SQLStr = "UPDATE EmailInviateT SET Email='" & Controlla_Apice(lblEmailInvio.Text.Trim) & "' "
                    SQLStr += "FROM EmailInviateT INNER JOIN " & _
                             "EmailInviateDett ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN " & _
                             "ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID " & _
                             "WHERE (ArticoliInst_ContrattiAss.Cod_Coge = N'" & myIDCOGE.Trim & "')"
                    If ClsDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, SQLStr) = False Then
                        ProgrammaticModalPopup.Show()
                        lblMess.Text = "Aggiornamento tutte l'E-mail inviate in archivio."
                        Exit Sub
                    End If
                End If
            Catch ex As Exception
                ProgrammaticModalPopup.Show()
                lblMess.Text = "Aggiornamento non riuscito.: " & ex.Message.Trim
                Exit Sub
            End Try
        End If
        '-
        'UPDATE EMAIL DESTINAZIONE CLIENTE
        If (Not Session(RKANAGRDESTCLI) Is Nothing) Then
            If EmailDest.Trim = rk1.EMail.Trim Then
                Exit Sub
            End If
            Try
                SQLStr = "UPDATE DestClienti SET EMail = '" & Controlla_Apice(EmailDest.Trim) & "' WHERE Codice= '" & myIDCOGE.Trim & "' AND Progressivo = '" & rk1.Progressivo.Trim & "'"
                If ClsDB.ExecuteQueryUpdate(TipoDB.dbSoftCoge, SQLStr) = False Then
                    ProgrammaticModalPopup.Show()
                    lblMess.Text = "Aggiornamento E-mail destinazione non riuscito."
                    Exit Sub
                End If
            Catch ex As Exception
                ProgrammaticModalPopup.Show()
                lblMess.Text = "Aggiornamento E-mail destinazione non riuscito.: " & ex.Message.Trim
                Exit Sub
            End Try
        End If
        '-
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
    End Sub

    Public Sub Hide()
        ProgrammaticModalPopup.Hide()
    End Sub

    Private Sub chkInvioMailScad_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkInvioMailScad.CheckedChanged
        If chkInvioMailScad.Checked = False Then
            chkChiudiEmail.Visible = True : chkChiudiEmail.Enabled = True : chkChiudiEmail.Checked = False
            chkApriEmail.Visible = False : chkApriEmail.Enabled = False : chkApriEmail.Checked = False
        Else
            chkChiudiEmail.Visible = False : chkChiudiEmail.Enabled = False : chkChiudiEmail.Checked = False
            chkApriEmail.Visible = True : chkApriEmail.Enabled = True : chkApriEmail.Checked = False
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Private Sub txtEmail_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEmail.TextChanged
        Call SetLblEmailInvio()
        ProgrammaticModalPopup.Show()
    End Sub

    Private Sub txtEmailDest_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEmailDest.TextChanged
        Call SetLblEmailInvio()
        ProgrammaticModalPopup.Show()
    End Sub

    Private Sub txtEmailInvioScad_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtEmailInvioScad.TextChanged
        Call SetLblEmailInvio()
        ProgrammaticModalPopup.Show()
    End Sub
End Class