Imports Microsoft.VisualBasic
Imports System.Net.Mail
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Text

Public Class FrmInvioMail
    Dim InvioTest As Boolean

    WriteOnly Property set_InvioTest() As Boolean
        Set(ByVal Value As Boolean)
            InvioTest = Value
        End Set
    End Property

    Const Cst_Invio As String = "INVIO EMAIL IN CORSO"

    Dim SleepSendMM As Integer = 1

    Public Cst_EmailMittente As String = ""
    Public Cst_EmailMittenteDescrizione As String = "CognomeNome"
    Public Cst_EmailMittenteCarica As String = "Carica"
    Public Cst_EmailCopiaConoscenza As String = ""
    Public Cst_EmailDestinatario As String = ""

    Public Cst_EmailSmtp As String = ""
    Public Cst_EmailUtente As String = ""
    Public Cst_EmailPassword As String = ""
    Public Cst_EmailSmtpPort As Integer = 25
    '-
    Public Cst_All_1 As String = ""
    Public Cst_All_2 As String = ""
    Public Cst_All_3 As String = ""
    Public Cst_All_4 As String = ""
    '----
    Dim myCn As SqlConnection
    Dim myCmd As New SqlCommand
    Dim myAdapt As New SqlDataAdapter
    Dim SqlStr As String

    Dim LoadForm As Boolean = True : Dim ErrLoad As Boolean = False

    Dim Minuti As Integer = 15
    Const MinutiErr As Integer = 60

    Dim Cst_PathLoghi As String = ""
    Dim Cst_PathAllegati As String = ""

    Dim NEmailDaInviare As Integer = 0
    Dim NEmailInviate As Integer = 0
    Dim NEmailErrate As Integer = 0

    Private Sub FrmInvioMail_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        If ErrLoad = True Then
            If ErrMess.Trim = "" Then ErrMess = "Chiusura programma - FrmInvioMail_Activated ErrLoad = True"
            Call ScriviFileLog(ErrMess)
            Call AggChiaveOP(ErrMess)
            Reset()
            End
            Exit Sub
        End If
        If LoadForm = True Or Not Me.Visible Then Exit Sub

        Me.Refresh()
    End Sub
    Private Sub FrmInvioMail_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Cursor = Cursors.AppStarting
        LoadForm = True : ErrLoad = False
        ErrMess = ""
        Try
            LblInvio.Text = "Avvio programma " & IIf(InvioTest, "INVIOTEST", "REALE") & " ... Caricamento dati in corso .... "
            Call ScriviFileLog(LblInvio.Text)
            ErrMess = LblInvio.Text
            Try
                Ditta = ConfigurationManager.AppSettings("Ditta").ToString
            Catch
                Ditta = ""
            End Try
            If Ditta.Trim = "" Then
                ErrMess = "Attenzione, codice ditta non definito."
                Call ScriviFileLog(ErrMess)
                ErrLoad = True
                Exit Sub
            End If
            '-
            Try
                Cst_PathLoghi = ConfigurationManager.AppSettings("PathLoghi").ToString
                If Not Directory.Exists(Cst_PathLoghi) Then
                    Directory.CreateDirectory(Cst_PathLoghi)
                End If
            Catch
                Cst_PathLoghi = ""
            End Try
            If Cst_PathLoghi.Trim = "" Then
                ErrMess = "Attenzione, Cst_PathLoghi non definito."
                Call ScriviFileLog(ErrMess)
                ErrLoad = True
                Exit Sub
            End If
            '-PathAllegati
            Try
                Cst_PathAllegati = ConfigurationManager.AppSettings("PathAllegati").ToString
                If Not Directory.Exists(Cst_PathAllegati) Then
                    Directory.CreateDirectory(Cst_PathAllegati)
                End If
            Catch
                Cst_PathAllegati = ""
            End Try
            'giu110119 non va bene qui perche cambia la posizione in funzione dell'utente
            '' ''a1
            Try
                Cst_All_1 = ConfigurationManager.AppSettings("Cst_All_1").ToString
                ' ''If File.Exists(Cst_PathAllegati & "\" & Cst_All_1) = False Then
                ' ''    ErrMess = "Attenzione, Cst_All_1 non ESISTE."
                ' ''    Call ScriviFileLog(ErrMess)
                ' ''    ErrLoad = True
                ' ''    Exit Sub
                ' ''End If
            Catch
                Cst_All_1 = ""
            End Try
            'a2
            Try
                Cst_All_2 = ConfigurationManager.AppSettings("Cst_All_2").ToString
                ' ''If File.Exists(Cst_PathAllegati & "\" & Cst_All_2) = False Then
                ' ''    ErrMess = "Attenzione, Cst_All_2 non ESISTE."
                ' ''    Call ScriviFileLog(ErrMess)
                ' ''    ErrLoad = True
                ' ''    Exit Sub
                ' ''End If
            Catch
                Cst_All_2 = ""
            End Try
            'a3
            Try
                Cst_All_3 = ConfigurationManager.AppSettings("Cst_All_3").ToString
                ' ''If File.Exists(Cst_PathAllegati & "\" & Cst_All_3) = False Then
                ' ''    ErrMess = "Attenzione, Cst_All_3 non ESISTE."
                ' ''    Call ScriviFileLog(ErrMess)
                ' ''    ErrLoad = True
                ' ''    Exit Sub
                ' ''End If
            Catch
                Cst_All_3 = ""
            End Try
            'a4
            Try
                Cst_All_4 = ConfigurationManager.AppSettings("Cst_All_4").ToString
                ' ''If File.Exists(Cst_PathAllegati & "\" & Cst_All_4) = False Then
                ' ''    ErrMess = "Attenzione, Cst_All_4 non ESISTE."
                ' ''    Call ScriviFileLog(ErrMess)
                ' ''    ErrLoad = True
                ' ''    Exit Sub
                ' ''End If
            Catch
                Cst_All_4 = ""
            End Try
            'test 
            If Cst_PathAllegati.Trim = "" Then
                Cst_All_1 = "" : Cst_All_2 = "" : Cst_All_3 = "" : Cst_All_4 = ""
                ErrMess = "Attenzione, Cst_PathAllegati non definito."
                Call ScriviFileLog(ErrMess)
                ' ''ErrLoad = True
                ' ''Exit Sub
            ElseIf Cst_All_1 = "" And Cst_All_2 = "" And Cst_All_3 = "" And Cst_All_4 = "" Then
                ErrMess = "Attenzione, Allegati non definiti"
                Call ScriviFileLog(ErrMess)
                ' ''ErrLoad = True
                ' ''Exit Sub
            End If
            'MINUTI CONTROLLO SENZA ERRORI (15) ALTRIMENTI MINUTIERR (60)
            Try
                Minuti = ConfigurationManager.AppSettings("ContrOgniMM").ToString
            Catch
                Minuti = 15
            End Try
            Try
                SleepSendMM = ConfigurationManager.AppSettings("SleepSendMM").ToString
            Catch
                SleepSendMM = 5
            End Try
            If SleepSendMM = 0 Then SleepSendMM = 5
            '------------
            Dim ConnDBInstall As String = ConfigurationManager.AppSettings("ConnDBInstall").ToString
            ErrMess = ConnDBInstall
            Call ScriviFileLog(ErrMess)
            ConnDBInstall += ";Password=" & GetSmartKey()
            Dim DsGetUltEsercizio As New DataSet
            myCn = New SqlConnection(ConnDBInstall)
            myCn.Open()
            SqlStr = "Select TOP 1 * FROM Esercizi WHERE Ditta='" & Ditta.Trim & "' ORDER BY Esercizio DESC"
            ErrMess = SqlStr
            Call ScriviFileLog(ErrMess)
            myCmd.CommandText = SqlStr
            myCmd.Connection = myCn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandTimeout = 0
            myAdapt.SelectCommand = myCmd
            myAdapt.Fill(DsGetUltEsercizio)
            myCn.Close() : myCn = Nothing
            '-
            Try
                Esercizio = DsGetUltEsercizio.Tables(0).Rows(0).Item("Esercizio")
            Catch ex As Exception
                Esercizio = ""
            End Try
            DsGetUltEsercizio = Nothing
            If Esercizio.Trim = "" Then
                ErrMess = "Attenzione, Ultimo esercizio non trovato."
                Call ScriviFileLog(ErrMess)
                ErrLoad = True
                Exit Sub
            End If
            '
            Try
                Cst_EmailSmtp = ConfigurationManager.AppSettings("SMTPServer").ToString.Trim
                Cst_EmailUtente = ConfigurationManager.AppSettings("SMTPUserName").ToString.Trim
                Cst_EmailPassword = ConfigurationManager.AppSettings("SMTPPassword").ToString.Trim
                Cst_EmailSmtpPort = ConfigurationManager.AppSettings("SMTPPorta").ToString.Trim
                If Cst_EmailSmtpPort = 0 Then Cst_EmailSmtpPort = 25
                If Cst_EmailSmtp.Trim = "" Or Cst_EmailUtente.Trim = "" Or Cst_EmailPassword.Trim = "" Or Cst_EmailSmtpPort = 0 Then
                    ErrMess = "Attenzione, parametri invio email mancanti."
                    Call ScriviFileLog(ErrMess)
                    ErrLoad = True
                    Exit Sub
                End If
                Cst_EmailMittente = ConfigurationManager.AppSettings("EMAILMittente").ToString.Trim()
                Cst_EmailMittenteDescrizione = ConfigurationManager.AppSettings("DESCMittente").ToString.Trim()
                Cst_EmailMittenteCarica = ConfigurationManager.AppSettings("CARICAMittente").ToString.Trim()
            Catch ex As Exception
                ErrMess = "Attenzione, parametri invio email mancanti."
                Call ScriviFileLog(ErrMess)
                ErrLoad = True
                Exit Sub
            End Try
            ProgressBar1.Value = 0
            ProgressBar1.Maximum = 0
            LblInvio.Text = "In attesa ...."
            Call subImpostaTimer(1)
            Call AggChiaveOP("AVVIO PROGRAMMA " & LblInfoPacchetto.Text)
        Catch ex As Exception
            ErrMess = "Errore (Load): " & ex.Message
            Call ScriviFileLog(ErrMess)
            ErrLoad = True
            Exit Sub
        End Try
       
    End Sub
    Private Sub subImpostaTimer(ByVal _Minuti As Integer)
        Timer1.Stop()
        If _Minuti = 0 Then _Minuti = 15
        Timer1.Interval = _Minuti * 60000
        Timer1.Start()
        If InvioTest Then
            Me.Text = "Gestione invio E-mail SOLO OGGETTO - !!!TEST!!! - Controllo ogni : " & _Minuti.ToString & " minuti"
        Else
            Me.Text = "Gestione invio E-mail SOLO OGGETTO - Controllo ogni : " & _Minuti.ToString & " minuti"
        End If
        LblInfoPacchetto.Text = "Prossimo controllo: " & Format(Now, "dd/MM/yyyy") + " " & Format(DateAdd(DateInterval.Minute, _Minuti, Now), "H:mm") & IIf(InvioTest, " TEST", "")
        Me.Refresh()
        Call ScriviFileLog(LblInfoPacchetto.Text)
    End Sub
    Private Sub FrmInvioMail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        e.Cancel = True
    End Sub
    Private Sub FrmInvioMail_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = False
            NotifyIcon1.Visible = True
        Else
            Me.Width = 575 : Me.Height = 175
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
        End If
    End Sub
    Private Sub NotifyIcon1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDown
        Visualizzami()
    End Sub
    Private Sub Visualizzami()
        If Me.WindowState = FormWindowState.Normal Then Exit Sub
        Me.WindowState = FormWindowState.Normal
        Me.Refresh()
    End Sub
    Private Sub btnChiudi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChiudi.Click
        ErrMess = "CHIUSURA PROGRAMMA INVIO EMAIL"
        Call ScriviFileLog(ErrMess)
        Call AggChiaveOP(ErrMess)
        Me.NotifyIcon1.Dispose()
        Me.Dispose()
        Reset()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        LblInvio.Text = "Inizio controllo E-Mail da inviare"
        Call ScriviFileLog(LblInvio.Text)
        Timer1.Stop()
        If Mid(GetChiaveOP(), 1, 22) = "PREPARA EMAIL IN CORSO" Then
            subImpostaTimer(MinutiErr)
            LblInvio.Text = "PREPARA EMAIL IN CORSO " & Format(Now, "dd/MM/yyyy H:mm")
            ErrMess = LblInvio.Text
            ScriviFileLog(LblInvio.Text)
            Exit Sub
        End If
        If InviaEmail() = True Then
            If NEmailDaInviare = 0 And NEmailErrate = 0 Then
                Call subImpostaTimer(Minuti)
                ErrMess = LblInfoPacchetto.Text
                Call AggChiaveOP(ErrMess)
            ElseIf NEmailInviate = NEmailDaInviare Then 'GIU230219 NON CONSIDERO LE ERRATE
                Call subImpostaTimer(MinutiErr)
                LblInvio.Text = "TERMINE INVIO EMAIL " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                ErrMess = LblInvio.Text
                ScriviFileLog(ErrMess)
                Call AggChiaveOP(ErrMess)
            Else 'ASPETTO OPERAZIONI DA SOFTAZIENDA eventuali correzioni email e altri tipi di errore
                Call subImpostaTimer(SleepSendMM)
                If NEmailErrate > 0 Then
                    LblInvio.Text = "ATTESA INVIO EMAIL (MISTA 18052023)" & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                Else
                    LblInvio.Text = "ATTESA INVIO EMAIL (MISTA 18052023)" & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                End If
                ErrMess = LblInvio.Text
                ScriviFileLog(ErrMess)
                Call AggChiaveOP(LblInvio.Text)
            End If
        Else
            Call subImpostaTimer(MinutiErr)
            If NEmailDaInviare > 0 Then
                LblInvio.Text = "ERRORE INVIO EMAIL " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
            Else
                LblInvio.Text = "ERRORE CONTROLLO EMAIL " & Format(Now, "dd/MM/yyyy H:mm") & " - " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
            End If
            ErrMess = LblInvio.Text
            ScriviFileLog(ErrMess)
            Call AggChiaveOP(LblInvio.Text)
        End If
    End Sub

    Private Function InviaEmail() As Boolean
        InviaEmail = True
        NEmailDaInviare = 0
        NEmailInviate = 0
        NEmailErrate = 0
        Try

            Dim ConnDBNNScadenze As String = ConfigurationManager.AppSettings("ConnDBNNScadenze").ToString
            ConnDBNNScadenze = ConnDBNNScadenze.Replace("NN", Ditta.Trim)
            Call ScriviFileLog("Connessione a: " & ConnDBNNScadenze)
            ConnDBNNScadenze += ";Password=" & GetSmartKey()
            Dim DsEmailDaInviare As New DsMailDaInviare
            DsEmailDaInviare.Clear()
            myCn = New SqlConnection(ConnDBNNScadenze)
            myCn.Open()
            '-------------
            '0 DA INVIAE -1 ERRORE INVIO -2 INVIO SOLLECITO E DIVENTA 2 QUANDO E' STATO INVIATO SOLLECITO
            SqlStr = "Select * FROM EmailInviateMISTA18052023 WHERE ISNULL(Stato,0)=0 OR ISNULL(Stato,0)<0"
            ErrMess = SqlStr
            Call ScriviFileLog(ErrMess)
            myCmd.CommandText = SqlStr
            myCmd.Connection = myCn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandTimeout = 2400
            myAdapt.SelectCommand = myCmd
            ErrMess = "myAdapt.Fill(DsEmailDaInviare.EmailInviateTOggetto)"
            Call ScriviFileLog(ErrMess)
            myAdapt.Fill(DsEmailDaInviare.EmailInviateTOggetto)
            '----
            NEmailDaInviare = DsEmailDaInviare.EmailInviateTOggetto.Select("Stato<>-1").Length 'GIU230219 NO CONTEGGIO LE ERRATE
            NEmailInviate = 0
            NEmailErrate = DsEmailDaInviare.EmailInviateTOggetto.Select("Stato=-1").Length
            If NEmailDaInviare = 0 Then
                ErrMess = "If NEmailDaInviare = 0 Then - " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                Call ScriviFileLog(ErrMess)
                DsEmailDaInviare = Nothing
                Call CloseAll()
                Exit Function
            End If
            '-
            If Mid(GetChiaveOP(), 1, 22) = "PREPARA EMAIL IN CORSO" Then
                ErrMess = "PREPARA EMAIL IN CORSO - DA INVIARE: " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                LblInvio.Text = ErrMess
                Call ScriviFileLog(ErrMess)
                Call CloseAll()
                Exit Function
            End If
            '------------
            '- OK BLOCCO L'ON LINE 
            If NEmailErrate > 0 Then
                LblInvio.Text = Cst_Invio & ": " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
            Else
                LblInvio.Text = Cst_Invio & ": " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString
            End If
            ErrMess = LblInvio.Text
            Call ScriviFileLog(LblInvio.Text)
            Call AggChiaveOP(LblInvio.Text)

            Dim ErrInvio As Boolean = False
            '-
            Dim RowEmailT As DsMailDaInviare.EmailInviateTOggettoRow
            'GIU100918 PER I PARZ.CONCLUSE O CONCLUSE "Stato=0 OR Stato=-2")
            For Each RowEmailT In DsEmailDaInviare.EmailInviateTOggetto.Select("Stato<>-1", "ID")
                ErrInvio = False
                RowEmailT.BeginEdit()
                RowEmailT.Note = ""
                'MITTENTE
                Me.Cst_EmailMittente.Replace(" ", "")
                If Cst_EmailMittente.Trim = "" Then
                    ErrMess = "ERRORE MITTENTE - N. " & RowEmailT.Id.ToString.Trim
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess)
                    ErrInvio = True
                    RowEmailT.Stato = -1
                    RowEmailT.Note = ErrMess
                End If
                If ConvalidaEmail(Cst_EmailMittente.Trim) = False Then
                    ErrMess = "ERRORE MITTENTE - N. " & RowEmailT.Id.ToString.Trim
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess)
                    ErrInvio = True
                    RowEmailT.Stato = -1
                    RowEmailT.Note = ErrMess
                End If
                If Cst_EmailMittenteDescrizione.Trim = "" Then
                    Cst_EmailMittenteDescrizione = Cst_EmailMittente
                End If
                If Cst_EmailMittenteDescrizione.Trim = "" Then
                    Cst_EmailMittenteDescrizione = Cst_EmailMittente
                End If
                'DESTINATARIO /TEST
                Cst_EmailDestinatario = ""
                If InvioTest = True Then
                    Try
                        Cst_EmailDestinatario = ConfigurationManager.AppSettings("EmailTest").ToString
                    Catch ex As Exception
                        Cst_EmailDestinatario = ""
                    End Try
                Else
                    If RowEmailT.IsDestinatarioNull Then
                        'NULLA
                    ElseIf RowEmailT.Destinatario.Trim <> "" Then
                        Cst_EmailDestinatario = RowEmailT.Destinatario.Trim
                    End If
                End If
                Me.Cst_EmailDestinatario.Replace(" ", "")
                If Cst_EmailDestinatario.Trim = "" Then
                    ErrMess = "ERRORE DESTINATARIO - N. " & RowEmailT.Id.ToString.Trim
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess)
                    ErrInvio = True
                    RowEmailT.Stato = -1
                    RowEmailT.Note += IIf(RowEmailT.Note.Trim <> "", vbCr + ErrMess, ErrMess)
                    RowEmailT.Destinatario = Cst_EmailDestinatario.Trim
                ElseIf ConvalidaEmail(Cst_EmailDestinatario.Trim) = False Then
                    ErrMess = "ERRORE DESTINATARIO - N. " & RowEmailT.Id.ToString.Trim
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess)
                    ErrInvio = True
                    RowEmailT.Stato = -1
                    RowEmailT.Note += IIf(RowEmailT.Note.Trim <> "", vbCr + ErrMess, ErrMess)
                    RowEmailT.Destinatario = Cst_EmailDestinatario.Trim
                End If
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim SWAllegati As Boolean = False
                If Cst_PathAllegati.Trim = "" Then
                    Cst_All_1 = "" : Cst_All_2 = "" : Cst_All_3 = "" : Cst_All_4 = ""
                End If
                If Cst_All_1 = "" And Cst_All_2 = "" And Cst_All_3 = "" And Cst_All_4 = "" Then
                    'nulla
                Else
                    SWAllegati = True
                End If
                'INIZIO invio
                Dim objEmail As New clsEMailSender
                If ErrInvio = False Then
                    With objEmail
                        If SWAllegati Then
                            .AttachmentArray = New ArrayList
                            If Cst_All_1.Trim <> "" Then
                                .AttachmentArray.Add(Cst_PathAllegati & "\" & Cst_All_1)
                            End If
                            If Cst_All_2.Trim <> "" Then
                                .AttachmentArray.Add(Cst_PathAllegati & "\" & Cst_All_2)
                            End If
                            If Cst_All_3.Trim <> "" Then
                                .AttachmentArray.Add(Cst_PathAllegati & "\" & Cst_All_3)
                            End If
                            If Cst_All_4.Trim <> "" Then
                                .AttachmentArray.Add(Cst_PathAllegati & "\" & Cst_All_4)
                            End If
                        End If
                        'FINE ALLEGATI
                        Cst_EmailMittente.Replace(" ", "")
                        .FromAddress = Cst_EmailMittente
                        Cst_EmailMittenteDescrizione.Replace(" ", "")
                        .FromDisplayAddress = Cst_EmailMittenteDescrizione.Trim
                        If Cst_EmailMittenteCarica.Trim <> "" Then
                            .CaricaMittente = Cst_EmailMittenteCarica
                        End If

                        Cst_EmailDestinatario.Replace(" ", "")
                        .ToAddress = Cst_EmailDestinatario.Trim

                        .CCAddress = ""
                        'giu230219 muovo l'email di test cosi non blocco l'email utente che genera le email meglio su GMAIL o ALTRO
                        Cst_EmailCopiaConoscenza = ConfigurationManager.AppSettings("EmailTest").ToString 'giu230219 Cst_EmailMittente.Trim
                        If Cst_EmailCopiaConoscenza.Trim <> "" Then
                            Cst_EmailCopiaConoscenza.Replace(" ", "")
                            .BCCAddress = Cst_EmailCopiaConoscenza.Trim
                        Else
                            .BCCAddress = Cst_EmailCopiaConoscenza.Trim
                        End If

                        .SMTPPort = Cst_EmailSmtpPort
                        .SMTPServer = Cst_EmailSmtp
                        If Cst_EmailPassword <> "" Then
                            .SMTPAutenticazione = True
                            .SMTPPassword = Cst_EmailPassword
                            .SMTPUser = Cst_EmailUtente
                        End If

                        'OGGETTO
                        .Subject = IIf(InvioTest = True, "!!! VERIFICARE SE TUTTO CORRETTO GRAZIE !!! - *** PROMEMORIA*** Field Service Notification – INFORMATION REQUIRED ***PROMEMORIA***", "*** PROMEMORIA*** Field Service Notification – INFORMATION REQUIRED ***PROMEMORIA***")
                        .Body = "<!doctype html><html xmlns='http://www.w3.org/1999/xhtml'> " & _
                                      "<head>" & _
                                      "<title>*** PROMEMORIA*** Field Service Notification – INFORMATION REQUIRED ***PROMEMORIA***</title>" & _
                                      "<meta charset='utf-8'/> " & _
                                      "<meta name='viewport' content='width=device-width, initial-scale=1, maximum-scale=1'/>" & _
                                      "</style>" & _
                                      "</head>" & _
                                      "<body>"
                        .Body += "<b>Per chi " + "<span style='color: #FF0000;'>NON </span> " + " avesse ancora risposto, prego leggere ed inviare quanto richiesto.</b><br /><br />"
                        .Body += "<span style='color: #FF0000;'><b>NUMERO DI CODICE CLIENTE </b></span> " + " <b>94375750</b>" + "<br /><br />"
                        .Body += "Gentile cliente di DAE Philips, <br /><br />"
                        .Body += "recentemente ha ricevuto una lettera, un'e-mail e/o una cartolina per informarla di un problema identificato con gli elettrodi DAE Philips M5071A (pazienti adulti) che può rappresentare un rischio per i pazienti. <br />"

                        .Body += "Philips invierà cartucce di elettrodi SMART sostitutive per ogni DAE in dotazione. A questo scopo, è necessario verificare le informazioni di contatto e il numero di serie del DAE in dotazione.<br />"
                        .Body += "Questa verifica ci consentirà di velocizzare le operazioni di sostituzione degli elettrodi.<br /><br />"
                        .Body += "Completare il PDF allegato e restituire quanto prima possibile.<br /><br />"
                        .Body += "Grazie in anticipo per la collaborazione.<br /><br />"
                        .Body += "Distinti saluti,<br />"
                        .Body += "Iredeem S.p.A. <br /><br />"
                        .Body += "<img src='cid:LogoEmail1'><img src='cid:LogoEmail2'><br /><br />"
                        '.Body += "<img src='cid:LogoEmail2'><br /><br />"
                        If Cst_EmailMittente.Trim <> Cst_EmailMittenteDescrizione.Trim Then
                            .Body += "<b>" & Cst_EmailMittenteDescrizione.Trim & "</b><br />"
                            If Cst_EmailMittenteCarica.Trim <> "" Then
                                .Body += "<b>" & Cst_EmailMittenteCarica.Trim & "</b><br />"
                            End If
                            .Body += "<a href=""mailto:" & Cst_EmailMittente.Trim & "?Subject=Comunicazione"" target=""_top"">" & Cst_EmailMittente.Trim & "</a><br /><br />"
                        End If
                        .Body += "<b>IREDEEM S.p.A.</b><br />"
                        .Body += "Piazza dei Martiri 1943-1945 n.1, 40121 Bologna  -  T +39 051 0935879  F +39 051 0935882 <br />"
                        .Body += "<a href=""http://www.iredeem.it/"">www.iredeem.it</a><br /><br />"
                        '.Body += "<span style='font-size:10.0pt;'>Dealer in Emergency Care & Resuscitation</span><br />"
                        '.Body += "<img src='cid:LogoEmail2'><br /><br />"
                        ' ''.Body += "<span style='font-size:10.0pt;'>Per essere sempre informato seguici sui social</span><br />"
                        .Body += "</body>" & _
                                 "</html>"
                        '---------
                        .Logo1 = Cst_PathLoghi & "\LogoEmail1.Gif"
                        .Logo2 = Cst_PathLoghi & "\LogoEmail2.jpg"

                        If Not .SendMailMessage() Then
                            ErrMess += " ERRORE INVIO - N° " & RowEmailT.Id.ToString.Trim
                            LblInvio.Text = ErrMess
                            Call ScriviFileLog(ErrMess)
                            ErrInvio = True
                            RowEmailT.Stato = -1
                            RowEmailT.Note += IIf(RowEmailT.Note.Trim <> "", vbCr + ErrMess, ErrMess)
                            NEmailErrate += 1
                        End If
                    End With
                End If
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If ErrInvio = False Then
                    'invio
                    NEmailInviate = RowEmailT.Id '+= 1
                    If NEmailErrate > 0 Then
                        LblInvio.Text = Cst_Invio & ": - N. " & RowEmailT.Id.ToString.Trim
                    Else
                        LblInvio.Text = Cst_Invio & ": - N. " & RowEmailT.Id.ToString.Trim
                    End If
                    Call ScriviFileLog(LblInvio.Text)
                    Call AggChiaveOP(LblInvio.Text)
                    If RowEmailT.Stato = 0 Then
                        RowEmailT.Stato = 1
                        'giu230219 per i -1 non eseguo nulla perche' errate
                    ElseIf RowEmailT.Stato < 0 And RowEmailT.Stato <> -1 Then ' lo trasformo i positivo quindi -2 diventa 2 INVIO SOLLECITO
                        RowEmailT.Stato = RowEmailT.Stato * -1
                    End If
                    RowEmailT.Note = LblInvio.Text
                End If
                RowEmailT.Destinatario = Cst_EmailDestinatario.Trim()
                RowEmailT.EndEdit()
                If Me.myCn.State = ConnectionState.Closed Then
                    Me.myCn.Open()
                End If
                SqlStr = "UPDATE EmailInviateMISTA18052023 SET Stato=" & RowEmailT.Stato.ToString.Trim & ", DataInvio=GETDATE(),[Note]='" & Controlla_Apice(RowEmailT.Note.Trim) & _
                         "' WHERE ID=" & RowEmailT.Id.ToString.Trim
                ErrMess = SqlStr
                Call ScriviFileLog(ErrMess)
                myCmd.CommandText = SqlStr
                myCmd.Connection = myCn
                myCmd.CommandType = CommandType.Text
                myCmd.CommandTimeout = 2400
                Try
                    myCmd.ExecuteNonQuery()
                Catch ExSQL As SqlException
                    ErrMess = "ERRORE ExSQL - DA INVIARE: " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess & " " & ExSQL.Message)
                    Call AggChiaveOP(ErrMess)
                    InviaEmail = False
                    Call CloseAll()
                    Exit Function
                Catch Ex As Exception
                    ErrMess = "ERRORE Ex - DA INVIARE: " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess & " " & Ex.Message)
                    Call AggChiaveOP(ErrMess)
                    InviaEmail = False
                    Call CloseAll()
                    Exit Function
                End Try
                '---------------------------
                'SUCESSIVA @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                objEmail.Dispose()
                objEmail = Nothing
                If ErrInvio = False Then
                    Exit For
                End If
                ' ''System.Threading.Thread.Sleep(SleepSendMM * 60000)
            Next

            Call CloseAll()
            DsEmailDaInviare = Nothing

        Catch ex As Exception
            Call CloseAll()
            ErrMess = "Errore (InviaEmail): " & ex.Message
            LblInvio.Text = ErrMess
            Call ScriviFileLog(ErrMess)
            InviaEmail = False
            Exit Function
        End Try

    End Function

    Private Sub CloseAll()
        Try
            If Me.myCn.State = ConnectionState.Open Then
                Me.myCn.Close()
            End If
            myCn = Nothing
        Catch ex As Exception

        End Try
    End Sub
    '-
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Call subImpostaTimer(1)
    End Sub

End Class
