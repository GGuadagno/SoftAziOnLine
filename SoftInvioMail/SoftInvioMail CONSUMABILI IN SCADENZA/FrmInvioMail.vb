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
            If Cst_PathAllegati.Trim = "" And (Cst_All_1 <> "" Or Cst_All_2 <> "" Or Cst_All_3 <> "" Or Cst_All_4 <> "") Then
                ErrMess = "Attenzione, Cst_PathAllegati non definito. (Allegati definiti)"
                Call ScriviFileLog(ErrMess)
                ErrLoad = True
                Exit Sub
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
            '------------
            ' GIU310718 CAMBIATO CONNESSIONE PER DATI INVIO EMAIL 
            '------------
            ' ''Dim ConnStringOP As String = ConfigurationManager.AppSettings("ConnDBOP").ToString
            ' ''ConnStringOP += ";Password=" & GetSmartKey()
            Dim ConnStringOP As String = ConfigurationManager.AppSettings("ConnDBNNAAAA").ToString
            ConnStringOP = ConnStringOP.Replace("NNAAAA", Ditta.Trim & Esercizio.Trim)
            Call ScriviFileLog("Connessione a: " & ConnStringOP)
            ConnStringOP += ";Password=" & GetSmartKey()
            Dim DsGetOpzioni As New DataSet
            myCn = New SqlConnection(ConnStringOP)
            myCn.Open()
            ' ''SqlStr = "Select * FROM AbilitazioniCoGe"
            SqlStr = "Select * FROM ParametriGeneraliAZI"
            ErrMess = SqlStr
            Call ScriviFileLog(ErrMess)
            myCmd.CommandText = SqlStr
            myCmd.Connection = myCn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandTimeout = 0
            myAdapt.SelectCommand = myCmd
            myAdapt.Fill(DsGetOpzioni)
            myCn.Close() : myCn = Nothing
            '-
            Dim X As Integer = 0
            ' ''For X = 0 To DsGetOpzioni.Tables(0).Rows.Count - 1
            ' ''    If DsGetOpzioni.Tables(0).Rows(X).Item("Chiave") = "SMTPInvioRB" Then
            ' ''        Cst_EmailSmtp = DsGetOpzioni.Tables(0).Rows(X).Item("Descrizione")
            ' ''    ElseIf DsGetOpzioni.Tables(0).Rows(X).Item("Chiave") = "SMTPUser" Then
            ' ''        Cst_EmailUtente = DsGetOpzioni.Tables(0).Rows(X).Item("Descrizione")
            ' ''    ElseIf DsGetOpzioni.Tables(0).Rows(X).Item("Chiave") = "SMTPPwd" Then
            ' ''        Cst_EmailPassword = DsGetOpzioni.Tables(0).Rows(X).Item("Descrizione")
            ' ''    ElseIf DsGetOpzioni.Tables(0).Rows(X).Item("Chiave") = "SMTPPort" Then
            ' ''        Cst_EmailSmtpPort = DsGetOpzioni.Tables(0).Rows(X).Item("Descrizione")
            ' ''    End If
            ' ''Next
            If DsGetOpzioni.Tables(0).Select().Length > 0 Then
                Cst_EmailSmtp = DsGetOpzioni.Tables(0).Rows(X).Item("SMTPServer")
                Cst_EmailUtente = DsGetOpzioni.Tables(0).Rows(X).Item("SMTPUserName")
                Cst_EmailPassword = DsGetOpzioni.Tables(0).Rows(X).Item("SMTPPassword")
                Cst_EmailSmtpPort = DsGetOpzioni.Tables(0).Rows(X).Item("SMTPPorta")
            End If
            DsGetOpzioni = Nothing
            If Cst_EmailSmtpPort = 0 Then Cst_EmailSmtpPort = 25
            If Cst_EmailSmtp.Trim = "" Or Cst_EmailUtente.Trim = "" Or Cst_EmailPassword.Trim = "" Or Cst_EmailSmtpPort = 0 Then
                ErrMess = "Attenzione, parametri invio email mancanti."
                Call ScriviFileLog(ErrMess)
                ErrLoad = True
                Exit Sub
            End If
            
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
            Me.Text = "Gestione invio E-mail - !!!TEST!!! - Controllo ogni : " & _Minuti.ToString & " minuti"
        Else
            Me.Text = "Gestione invio E-mail - Controllo ogni : " & _Minuti.ToString & " minuti"
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
        If SWServizioEmail() = False Then
            subImpostaTimer(MinutiErr)
            LblInvio.Text = "SERVIZIO INVIO EMAIL SOSPESO " & Format(Now, "dd/MM/yyyy H:mm")
            ErrMess = LblInvio.Text
            ScriviFileLog(LblInvio.Text)
            Exit Sub
        End If
        If InviaEmail() = True Then
            If NEmailDaInviare = 0 And NEmailErrate = 0 Then
                Call subImpostaTimer(Minuti)
                ErrMess = LblInfoPacchetto.Text
                Call AggChiaveOP(ErrMess)
                If InviaEmailLogBK() = False Then
                    Call AggChiaveOP(LblInfoPacchetto.Text)
                End If
            ElseIf NEmailInviate = NEmailDaInviare Then 'GIU230219 NON CONSIDERO LE ERRATE
                Call subImpostaTimer(MinutiErr)
                LblInvio.Text = "TERMINE INVIO EMAIL " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                ErrMess = LblInvio.Text
                ScriviFileLog(ErrMess)
                Call AggChiaveOP(ErrMess)
                If InviaEmailLogBK() = False Then
                    Call AggChiaveOP(LblInvio.Text)
                End If
            Else 'ASPETTO OPERAZIONI DA SOFTAZIENDA eventuali correzioni email e altri tipi di errore
                Call subImpostaTimer(SleepSendMM)
                If NEmailErrate > 0 Then
                    LblInvio.Text = "ATTESA INVIO EMAIL " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                Else
                    LblInvio.Text = "ATTESA INVIO EMAIL " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
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

            Dim ConnDBNNAAAA As String = ConfigurationManager.AppSettings("ConnDBNNAAAA").ToString
            ConnDBNNAAAA = ConnDBNNAAAA.Replace("NNAAAA", Ditta.Trim & Esercizio.Trim)
            Call ScriviFileLog("Connessione a: " & ConnDBNNAAAA)
            ConnDBNNAAAA += ";Password=" & GetSmartKey()
            Dim DsEmailDaInviare As New DsMailDaInviare
            DsEmailDaInviare.Clear()
            myCn = New SqlConnection(ConnDBNNAAAA)
            myCn.Open()
            '-------------
            '0 DA INVIAE -1 ERRORE INVIO -2 INVIO SOLLECITO E DIVENTA 2 QUANDO E' STATO INVIATO SOLLECITO
            'GIU100918 PER I PARZ.CONCLUSE O CONCLUSE PRENDO TUTTI I MINORI DI 9 MENTRE -1 SONO ERRATE
            SqlStr = "Select * FROM EmailInviateT WHERE ISNULL(Stato,0)=0 OR ISNULL(Stato,0)<0"
            ErrMess = SqlStr
            Call ScriviFileLog(ErrMess)
            myCmd.CommandText = SqlStr
            myCmd.Connection = myCn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandTimeout = 2400
            myAdapt.SelectCommand = myCmd
            ErrMess = "myAdapt.Fill(DsEmailDaInviare.EmailInviateT)"
            Call ScriviFileLog(ErrMess)
            myAdapt.Fill(DsEmailDaInviare.EmailInviateT)
            '----
            NEmailDaInviare = DsEmailDaInviare.EmailInviateT.Select("Stato<>-1").Length 'GIU230219 NO CONTEGGIO LE ERRATE
            NEmailInviate = 0
            NEmailErrate = DsEmailDaInviare.EmailInviateT.Select("Stato=-1").Length
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
            If SWServizioEmail() = False Then
                ErrMess = "SERVIZIO INVIO EMAIL SOSPESO  - DA INVIARE: " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
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
            Dim RowEmailT As DsMailDaInviare.EmailInviateTRow
            'GIU100918 PER I PARZ.CONCLUSE O CONCLUSE "Stato=0 OR Stato=-2")
            For Each RowEmailT In DsEmailDaInviare.EmailInviateT.Select("Stato<>-1", "Anno,Numero")
                ErrInvio = False
                RowEmailT.BeginEdit()
                RowEmailT.Note = ""
                'MITTENTE
                If RowEmailT.IsMittenteNull Then
                    Cst_EmailMittente = ""
                Else
                    Cst_EmailMittente = RowEmailT.Mittente.Trim
                End If
                Me.Cst_EmailMittente.Replace(" ", "")
                If Cst_EmailMittente.Trim = "" Then
                    ErrMess = "ERRORE MITTENTE - N. " & RowEmailT.Anno.ToString & "/" & RowEmailT.Numero.ToString
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess)
                    ErrInvio = True
                    RowEmailT.Stato = -1
                    RowEmailT.Note = ErrMess
                End If
                Dim myNome As String = "Nome"
                If GetDatoOperatore("", Cst_EmailMittente, myNome, ErrMess) = True Then
                    If myNome.Trim <> "" Then myNome = "\" & myNome
                Else
                    myNome = ""
                End If
                Cst_EmailMittenteDescrizione = "CognomeNome"
                If GetDatoOperatore("", Cst_EmailMittente, Cst_EmailMittenteDescrizione, ErrMess) = True Then
                Else
                    Cst_EmailMittenteDescrizione = Cst_EmailMittente
                End If
                If Cst_EmailMittenteDescrizione.Trim = "" Then
                    Cst_EmailMittenteDescrizione = Cst_EmailMittente
                End If
                Cst_EmailMittenteCarica = "Carica"
                If GetDatoOperatore("", Cst_EmailMittente, Cst_EmailMittenteCarica, ErrMess) = True Then
                Else
                    Cst_EmailMittenteCarica = ""
                End If
                'DESTINATARIO /TEST
                If InvioTest = True Then
                    Try
                        Cst_EmailDestinatario = ConfigurationManager.AppSettings("EmailTest").ToString
                    Catch ex As Exception
                        Cst_EmailDestinatario = ""
                    End Try
                Else
                    If RowEmailT.IsEmailNull Then
                        Cst_EmailDestinatario = ""
                    Else
                        Cst_EmailDestinatario = RowEmailT.Email.Trim
                    End If
                End If
                Me.Cst_EmailDestinatario.Replace(" ", "")
                If Cst_EmailDestinatario.Trim = "" Then
                    ErrMess = "ERRORE DESTINATARIO - N. " & RowEmailT.Anno.ToString & "/" & RowEmailT.Numero.ToString
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess)
                    ErrInvio = True
                    RowEmailT.Stato = -1
                    RowEmailT.Note += IIf(RowEmailT.Note.Trim <> "", vbCr + ErrMess, ErrMess)
                End If
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim SWAllegati As Boolean = False
                If Not RowEmailT.IsIdModulo1Null Then
                    SWAllegati = RowEmailT.IdModulo1
                End If
                If SWAllegati = False Then
                    If Not RowEmailT.IsIdModulo2Null Then SWAllegati = RowEmailT.IdModulo2
                End If
                If SWAllegati = False Then
                    If Not RowEmailT.IsIdModulo3Null Then SWAllegati = RowEmailT.IdModulo3
                End If
                If SWAllegati = False Then
                    If Not RowEmailT.IsIdModulo4Null Then SWAllegati = RowEmailT.IdModulo4
                End If
                'giu150219 CONTROLLO : OBBLIGATORIO DEGLI ALLEGATI, ALMENO 1
                If SWAllegati = False Then
                    ErrMess = "ERRORE NESSUN ALLEGATO - N. " & RowEmailT.Anno.ToString & "/" & RowEmailT.Numero.ToString
                    LblInvio.Text = ErrMess
                    Call ScriviFileLog(ErrMess)
                    ErrInvio = True
                    RowEmailT.Stato = -1
                    RowEmailT.Note = ErrMess
                End If
                'INIZIO invio
                Dim objEmail As New clsEMailSender
                Dim SWMod1 As Boolean = False : Dim SWMod2 As Boolean = False : Dim SWMod3 As Boolean = False : Dim SWMod4 As Boolean = False
                If ErrInvio = False Then
                    With objEmail
                        If SWAllegati Then
                            .AttachmentArray = New ArrayList
                            If Not RowEmailT.IsIdModulo1Null Then
                                If RowEmailT.IdModulo1 = True Then
                                    .AttachmentArray.Add(Cst_PathAllegati & myNome & "\" & Cst_All_1)
                                    SWMod1 = True
                                End If
                            End If
                            If Not RowEmailT.IsIdModulo2Null Then
                                If RowEmailT.IdModulo2 = True Then
                                    .AttachmentArray.Add(Cst_PathAllegati & myNome & "\" & Cst_All_2)
                                    SWMod2 = True
                                End If
                            End If
                            If Not RowEmailT.IsIdModulo3Null Then
                                If RowEmailT.IdModulo3 = True Then
                                    .AttachmentArray.Add(Cst_PathAllegati & myNome & "\" & Cst_All_3)
                                    SWMod3 = True
                                End If
                            End If
                            If Not RowEmailT.IsIdModulo4Null Then
                                If RowEmailT.IdModulo4 = True Then
                                    .AttachmentArray.Add(Cst_PathAllegati & myNome & "\" & Cst_All_4)
                                    SWMod4 = True
                                End If
                            End If
                        End If
                        'FINE ALLEGATI
                        Cst_EmailMittente.Replace(" ", "")
                        .FromAddress = Cst_EmailMittente
                        Cst_EmailMittenteDescrizione.Replace(" ", "")
                        .FromDisplayAddress = Cst_EmailMittenteDescrizione.Trim
                        .CaricaMittente = Cst_EmailMittenteCarica 'lo passo ma non viene al momento scritto al pie' pagina 

                        Cst_EmailDestinatario.Replace(" ", "")
                        .ToAddress = Cst_EmailDestinatario.Trim

                        .CCAddress = ""
                        'giu230219 muovo l'email di test cosi non blocco l'email utente che genera le email meglio su GMAIL o ALTRO
                        Cst_EmailCopiaConoscenza = ConfigurationManager.AppSettings("EmailTest").ToString 'giu230219 Cst_EmailMittente.Trim
                        Cst_EmailCopiaConoscenza.Replace(" ", "")
                        .BCCAddress = Cst_EmailCopiaConoscenza.Trim

                        .SMTPPort = Cst_EmailSmtpPort
                        .SMTPServer = Cst_EmailSmtp
                        If Cst_EmailPassword <> "" Then
                            .SMTPAutenticazione = True
                            .SMTPPassword = Cst_EmailPassword
                            .SMTPUser = Cst_EmailUtente
                        End If

                        'OGGETTO
                        .Subject = IIf(InvioTest = True, "!!!TEST INVIO EMAIL!!! - " & RowEmailT.Email.Trim & " - ", "") & GetRagSocCliente(myCn, RowEmailT.Id) & " - MANUTENZIONE DEFIBRILLATORE PHILIPS - INVIO No. " & RowEmailT.Anno.ToString.Trim & "/" & RowEmailT.Numero.ToString.Trim
                        .Body = "<!doctype html><html xmlns='http://www.w3.org/1999/xhtml'> " & _
                                      "<head>" & _
                                      "<title>MANUTENZIONE DEFIBRILLATORE PHILIPS - INVIO No. " & RowEmailT.Anno.ToString.Trim & "/" & RowEmailT.Numero.ToString.Trim & "</title>" & _
                                      "<meta charset='utf-8'/> " & _
                                      "<meta name='viewport' content='width=device-width, initial-scale=1, maximum-scale=1'/>" & _
                                      "</style>" & _
                                      "</head>" & _
                                      "<body>"
                        If InvioTest = True Then
                            .Body += "!!!TEST INVIO EMAIL!!! - Elenco codici articoli:<br /><br />"
                            .Body += GetElencoArticoli(myCn, RowEmailT.Id) & "<br /><br />"
                            .Body += "!!!TEST INVIO EMAIL!!! - fine codici articoli:<br /><br />"
                        End If
                        'giu140219 UNICO TESTO (Francesca)
                        'HS1 - FR2 - FR3 - FRX
                        .Body += "Buongiorno, <br /><br />"
                        .Body += "A seguito dell’acquisto del Vs Defibrillatore Semiautomatico PHILIPS (DAE), il Vs contatto è stato inserito nel gestionale di Servizio gratuito ""Informativa per la Manutenzione Dae"" di Iredeem S.p.A.<br /><br />"

                        .Body += "Il Servizio vuole essere un promemoria a tutti i clienti per ricordare l'importanza di mantenere il DAE sempre pronto all'uso sostituendo regolarmente parti di ricambio consumabili - elettrodi e batteria - alla loro scadenza.<br /><br />"

                        .Body += "<span style='background: #FFFF99;'><b><u>Come capire se il vostro DAE è pronto all’uso?</u></b></span><br /><br />"

                        .Body += "1) Aprire la valigetta rossa contenente il DAE;<br />"
                        .Body += "2) Verificare che l’indicatore di stato in alto a destra presenti luce verde lampeggiante o clessidra (a seconda del Vs modello)<br />"
                        .Body += "3) Controllare la scadenza degli elettrodi e che la batteria funzionante: se il DAE non presenta la luce di stato o emette un segnale acustico, contattare IREDEEM.<br /><br />"

                        .Body += "Se avete difficoltà ad individuare la luce di stato o vi accorgete di anomalie, Vi invitiamo a visionare la videoguida per il Vs modello:<br />"
                        .Body += "HS1 &rarr; <a href=""https://youtu.be/PX3hPrAc12o"">Come risolvere allarmi o malfunzionamenti del Defibrillatore Philips Heartstart HS1 - Iredeem</a><br />"
                        .Body += "FRX &rarr; <a href=""https://youtu.be/blQ5hL5_nNY"">Come risolvere allarmi o malfunzionamenti del Defibrillatore Philips Heartstart FRx - Iredeem</a><br /><br />"

                        .Body += "<span style='background: #FF0000;'><b><u>ELETTRODI</u></b></span><br />"
                        .Body += "<u>Devono essere sostituiti ogni 2 anni e dopo ogni utilizzo poichè monouso.</u><br /><br />"

                        .Body += "La data di scadenza è visibile sul fronte della confezione di elettrodi, in basso, ed è tassativa.<br /><br />"

                        .Body += "Se avete difficoltà ad individuare la scadenza, Vi invitiamo a visionare la videoguida per il Vs modello:<br />"
                        .Body += "HS1 &rarr; <a href=""https://youtu.be/YsMfP4aCvFk"">Come sostituire la Cartuccia Elettrodi del defibrillatore Philips Heartstart HS1 (V2.00) - Iredeem</a><br />"
                        .Body += "FRX &rarr; <a href=""https://youtu.be/UKLSUyIN3mI"">Come sostituire gli Elettrodi del Defibrillatore Philips Heartstart FRx - Iredeem</a><br /><br />"

                        .Body += "<span style='background: #FF0000;'><b><u>BATTERIA</u></b></span><br />"
                        .Body += "<u>Ha durata di circa 4 anni in stand-by dalla data di inserimento della stessa nell'apparecchio.</u><br />"
                        .Body += "<b>Attenzione!</b> La data indicata sull’etichetta bianca ""INSTALL BEFORE"" non è quella di scadenza ma la data<br />"
                        .Body += "entro la quale la batteria deve essere inserita nel DAE perchè sia garantita la durata di 4 anni in stand-by.<br /><br />"

                        .Body += "Quando la batteria sarà vicina all’esaurimento, il DAE lo segnalerà mediante allarme acustico.<br /><br /><br />"

                        .Body += "A partire dal primo allarme, rimane ancora una minima autonomia.<br /><br />"

                        .Body += "Vi invitiamo a visionare la videoguida per il Vs modello:<br />"
                        .Body += "HS1 &rarr; <a href=""https://youtu.be/YOVe-knG_20"">Come sostituire la Batteria del Defibrillatore Philips Heartstart HS1 (V1.00) - Iredeem</a><br />"
                        .Body += "FRX &rarr; <a href=""https://youtu.be/AoEBRxFomCU"">Come sostituire la Batteria del Defibrillatore Philips Heartstart FRx (V1.00) - Iredeem</a><br /><br />"


                        .Body += "<span style='background: #FFFF99;'><b><u>Cosa fare se batteria o elettrodi sono scaduti?</u></b></span><br /><br />"

                        .Body += "Se avete verificato che gli elettrodi sono scaduti e/o la batteria è esaurita, inviateci il modulo d’ordine che trovate allegato alla mail debitamente compilato, e provvederemo a inviarvi conferma ordine e a seguire il materiale.<br /><br />"

                        .Body += "Vi invitiamo a leggere con attenzione le condizioni di compilazione e fornitura e di completarlo in tutte le sue parti.<br /><br />"

                        .Body += "Il modulo ha validità di 60 giorni dalla data di ricezione.<br /><br /><br />"

                        .Body += "Restando a disposizione per qualsiasi informazione,<br />"
                        .Body += "Porgo Cordiali Saluti. <br /><br />"
                        .Body += "<img src='cid:LogoEmail1'><img src='cid:LogoEmail2'><br /><br />"
                        '.Body += "<img src='cid:LogoEmail2'><br /><br />"
                        If Cst_EmailMittente.Trim <> Cst_EmailMittenteDescrizione.Trim Then
                            .Body += "<b>" & Cst_EmailMittenteDescrizione.ToUpper.Trim & "</b><br />"
                            .Body += "<b>" & Cst_EmailMittenteCarica.Trim & "</b><br />"
                            .Body += "<a href=""mailto:" & Cst_EmailMittente.ToLower.Trim & "?Subject=Comunicazione"" target=""_top"">" & Cst_EmailMittente.ToLower.Trim & "</a><br /><br />"
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
                            ErrMess += " ERRORE INVIO - N. " & RowEmailT.Anno.ToString & "/" & RowEmailT.Numero.ToString
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
                    NEmailInviate += 1
                    If NEmailErrate > 0 Then
                        LblInvio.Text = Cst_Invio & ": N. " & RowEmailT.Anno.ToString & "/" & RowEmailT.Numero.ToString & " ERR.: " & NEmailErrate.ToString & " - alle: " & Format(Now, "H:mm")
                    Else
                        LblInvio.Text = Cst_Invio & ": N. " & RowEmailT.Anno.ToString & "/" & RowEmailT.Numero.ToString & " - alle: " & Format(Now, "H:mm")
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

                RowEmailT.EndEdit()
                If Me.myCn.State = ConnectionState.Closed Then
                    Me.myCn.Open()
                End If
                SqlStr = "UPDATE EmailInviateT SET Stato=" & RowEmailT.Stato.ToString.Trim & ", DataInvio=GETDATE(), [Note]='" & Controlla_Apice(RowEmailT.Note.Trim) & _
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
                'giu300818 AGGIORNO NReInvio
                If ErrInvio = False Then
                    If UpdNReInvio(myCn, RowEmailT.Id) = False Then
                        InviaEmail = False
                        Call CloseAll()
                        Exit Function
                    End If
                End If
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

    Private Function GetRagSocCliente(ByVal _myCn As SqlConnection, ByVal _IdEmailInviateT As Long) As String
        'Dim myCmd As New SqlCommand
        Dim DsGetDatiCliente As New DataSet
        Dim _myCmd As New SqlCommand
        Dim _myAdapt As New SqlDataAdapter
        If _myCn.State = ConnectionState.Closed Then
            _myCn.Open()
        End If
        SqlStr = "SELECT TOP (1) Clienti.Rag_Soc, EmailInviateT.Email FROM EmailInviateDett INNER JOIN " & _
                 "EmailInviateT ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN " & _
                 "ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID INNER JOIN " & _
                 "Clienti ON ArticoliInst_ContrattiAss.Cod_Coge = Clienti.Codice_CoGe " & _
                 "WHERE (EmailInviateT.Id =" & _IdEmailInviateT.ToString.Trim & ")"
        ErrMess = SqlStr
        Call ScriviFileLog(ErrMess)
        _myCmd.CommandText = SqlStr
        _myCmd.Connection = _myCn
        _myCmd.CommandType = CommandType.Text
        _myCmd.CommandTimeout = 0
        _myAdapt.SelectCommand = _myCmd
        _myAdapt.Fill(DsGetDatiCliente)
        '-
        Try
            If (DsGetDatiCliente.Tables.Count > 0) Then
                If (DsGetDatiCliente.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(DsGetDatiCliente.Tables(0).Rows(0).Item("Rag_Soc")) Then
                        GetRagSocCliente = DsGetDatiCliente.Tables(0).Rows(0).Item("Rag_Soc").ToString.Trim
                    Else
                        GetRagSocCliente = ""
                    End If
                Else
                    GetRagSocCliente = ""
                End If
            Else
                GetRagSocCliente = ""
            End If
        Catch ex As Exception
            GetRagSocCliente = ex.Message.Trim
        End Try
    End Function
    Private Function GetElencoArticoli(ByVal _myCn As SqlConnection, ByVal _IdEmailInviateT As Long) As String
        'Dim myCmd As New SqlCommand
        Dim DsGetDati As New DataSet
        Dim _myCmd As New SqlCommand
        Dim _myAdapt As New SqlDataAdapter
        If _myCn.State = ConnectionState.Closed Then
            _myCn.Open()
        End If
        SqlStr = "SELECT ArticoliInst_ContrattiAss.Cod_Articolo FROM EmailInviateDett INNER JOIN " & _
                 "EmailInviateT ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN " & _
                 "ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID " & _
                 "WHERE (EmailInviateT.Id =" & _IdEmailInviateT.ToString.Trim & ") " & _
                 "GROUP BY ArticoliInst_ContrattiAss.Cod_Articolo "
        ErrMess = SqlStr
        Call ScriviFileLog(ErrMess)
        _myCmd.CommandText = SqlStr
        _myCmd.Connection = _myCn
        _myCmd.CommandType = CommandType.Text
        _myCmd.CommandTimeout = 0
        _myAdapt.SelectCommand = _myCmd
        _myAdapt.Fill(DsGetDati)
        '-
        GetElencoArticoli = ""
        Try
            If (DsGetDati.Tables.Count > 0) Then
                If (DsGetDati.Tables(0).Rows.Count > 0) Then
                    Dim X As Integer
                    For X = 0 To DsGetDati.Tables(0).Rows.Count - 1
                        If Not IsDBNull(DsGetDati.Tables(0).Rows(X).Item("Cod_Articolo")) Then
                            GetElencoArticoli += DsGetDati.Tables(0).Rows(X).Item("Cod_Articolo").ToString.Trim & " ; "
                        Else
                            GetElencoArticoli += " ; "
                        End If
                    Next
                Else
                    GetElencoArticoli = ""
                End If
            Else
                GetElencoArticoli = ""
            End If
        Catch ex As Exception
            GetElencoArticoli = ex.Message.Trim
        End Try
    End Function
    'GIU300818 OK AGGIORNO NREinvio dei singoli dettagli
    Private Function UpdNReInvio(ByVal _myCn As SqlConnection, ByVal _IdEmailInviateT As Long) As Boolean
        UpdNReInvio = True
        Dim DsGetDati As New DataSet
        Dim _myCmd As New SqlCommand
        Dim _myAdapt As New SqlDataAdapter
        If _myCn.State = ConnectionState.Closed Then
            _myCn.Open()
        End If
        SqlStr = "SELECT ArticoliInst_ContrattiAss.ID FROM EmailInviateDett INNER JOIN " & _
                 "EmailInviateT ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN " & _
                 "ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID " & _
                 "WHERE (EmailInviateT.Id =" & _IdEmailInviateT.ToString.Trim & ")"
        ErrMess = SqlStr
        Call ScriviFileLog(ErrMess)
        _myCmd.CommandText = SqlStr
        _myCmd.Connection = _myCn
        _myCmd.CommandType = CommandType.Text
        _myCmd.CommandTimeout = 0
        _myAdapt.SelectCommand = _myCmd
        _myAdapt.Fill(DsGetDati)
        '-
        Try
            If (DsGetDati.Tables.Count > 0) Then
                If (DsGetDati.Tables(0).Rows.Count > 0) Then
                    Dim X As Integer
                    For X = 0 To DsGetDati.Tables(0).Rows.Count - 1
                        SqlStr = "UPDATE ArticoliInst_ContrattiAss SET NReInvio= ISNULL(NReInvio,0) + 1 " & _
                                 "WHERE ID=" & DsGetDati.Tables(0).Rows(X).Item("ID").ToString.Trim
                        ErrMess = SqlStr
                        Call ScriviFileLog(ErrMess)
                        myCmd.CommandText = SqlStr
                        myCmd.Connection = myCn
                        myCmd.CommandType = CommandType.Text
                        myCmd.CommandTimeout = 2400
                        Try
                            myCmd.ExecuteNonQuery()
                        Catch ExSQL As SqlException
                            ErrMess = "ERRORE ExSQL - UpdNReInvio: " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                            LblInvio.Text = ErrMess
                            Call ScriviFileLog(ErrMess & " " & ExSQL.Message)
                            Call AggChiaveOP(ErrMess)
                            UpdNReInvio = False
                            Exit Function
                        Catch Ex As Exception
                            ErrMess = "ERRORE Ex - UpdNReInvio: " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
                            LblInvio.Text = ErrMess
                            Call ScriviFileLog(ErrMess & " " & Ex.Message)
                            Call AggChiaveOP(ErrMess)
                            UpdNReInvio = False
                            Exit Function
                        End Try
                    Next
                End If
            End If
        Catch ex As Exception
            ErrMess = "ERRORE Ex - UpdNReInvio: " & NEmailDaInviare.ToString & "/" & NEmailInviate.ToString & " ERR.: " & NEmailErrate.ToString
            LblInvio.Text = ErrMess
            Call ScriviFileLog(ErrMess & " " & ex.Message)
            Call AggChiaveOP(ErrMess)
            UpdNReInvio = False
            Exit Function
        End Try
    End Function

    '-
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Call subImpostaTimer(1)
    End Sub

    Private Function InviaEmailLogBK() As Boolean
        InviaEmailLogBK = False
        Dim PathLog As String = ""
        Dim TmpNomeFile As String = ""
        Dim TmpNomeFileNEW As String = ""
        Try
            Call ScriviFileLog("Controllo InviaEmailLogBK")
            Dim PathFileLogBK As String = ConfigurationManager.AppSettings("PathFileLogBK")
            If Not Directory.Exists(PathFileLogBK.Trim) Then
                Directory.CreateDirectory(PathFileLogBK.Trim)
            End If
            PathLog = AddDirSep(PathFileLogBK)
            TmpNomeFile = "LogBKAuto" & Format(Now, "yyyyMMdd") & ".txt"
            TmpNomeFileNEW = "LogBKAuto" & Format(Now, "yyyyMMddHHmmss") & ".txt"
            If Not Directory.Exists(PathFileLogBK.Trim + "\Inviato") Then
                Directory.CreateDirectory(PathFileLogBK.Trim + "\Inviato")
            End If
            If File.Exists(AddDirSep(PathFileLogBK) & TmpNomeFile) = True Then
                '
                If File.Exists(AddDirSep(PathFileLogBK) + "\Inviato\" & TmpNomeFileNEW) = True Then
                    'non cancello XKE' SEGNALA ERRORE SE E' LOCK QUNDI RIFORMATTO IL TmpNomeFileNEW PER HHmmss 
                    'File.Delete(AddDirSep(PathFileLogBK) + "\Inviato\" & TmpNomeFileNEW)
                    TmpNomeFileNEW = "LogBKAuto" & Format(Now, "yyyyMMddHHmmss") & ".txt"
                End If
                File.Copy(AddDirSep(PathFileLogBK) & TmpNomeFile, AddDirSep(PathFileLogBK) + "\Inviato\" & TmpNomeFileNEW)
                '
                If File.Exists(AddDirSep(PathFileLogBK) + "\Inviato\" & TmpNomeFileNEW) = True Then
                    'ok adesso invio
                    ' ''Dim X As Integer = 0
                    'MITTENTE
                    Cst_EmailMittente = ConfigurationManager.AppSettings("EmailMittenteBK") ' '' "gestionale@iredeem.it" 'Cst_EmailUtente
                    ' ''X = InStr(Cst_EmailMittente, "@")
                    ' ''If X > 0 Then
                    ' ''    'ok
                    ' ''Else 'aggiungo io il dominio della ditta
                    ' ''    Cst_EmailMittente += "@" & ConfigurationManager.AppSettings("ClienteDB") & ".it"
                    ' ''End If
                    ' ''Me.Cst_EmailMittente.Replace(" ", "")
                    ' ''If Cst_EmailMittente.Trim = "" Then
                    ' ''    ErrMess = "ERRORE MITTENTE - InviaEmailLogBK: " & TmpNomeFile
                    ' ''    LblInvio.Text = ErrMess
                    ' ''    Call ScriviFileLog(ErrMess)
                    ' ''    Exit Function
                    ' ''End If
                    Cst_EmailMittenteDescrizione = "Servizio automatico Invio E-mail"
                    Cst_EmailMittenteCarica = ""
                    'DESTINATARIO /TEST
                    Try
                        Cst_EmailDestinatario = ConfigurationManager.AppSettings("EmailTest").ToString
                    Catch ex As Exception
                        Cst_EmailDestinatario = ""
                    End Try
                    Me.Cst_EmailDestinatario.Replace(" ", "")
                    If Cst_EmailDestinatario.Trim = "" Then
                        ErrMess = "ERRORE DESTINATARIO - InviaEmailLogBK: " & TmpNomeFileNEW
                        LblInvio.Text = ErrMess
                        Call ScriviFileLog(ErrMess)
                        Exit Function
                    End If
                    'INIZIO invio
                    Dim objEmail As New clsEMailSender
                    With objEmail
                        .AttachmentArray = New ArrayList
                        .AttachmentArray.Add(AddDirSep(PathFileLogBK) + "\Inviato\" & TmpNomeFileNEW)
                        'FINE ALLEGATI
                        Cst_EmailMittente.Replace(" ", "")
                        .FromAddress = Cst_EmailMittente
                        Cst_EmailMittenteDescrizione.Replace(" ", "")
                        .FromDisplayAddress = Cst_EmailMittenteDescrizione.Trim
                        .CaricaMittente = Cst_EmailMittenteCarica 'lo passo ma non viene al momento scritto al pie' pagina 

                        Cst_EmailDestinatario.Replace(" ", "")
                        .ToAddress = Cst_EmailDestinatario.Trim

                        .CCAddress = ""
                        'PER EVITARE DI MANDARE EMAIL ANCHE AL MITTENTE CHE IN QUESTO CASO NON E' UN INDIRIZZO VALIDO
                        ' ''Cst_EmailCopiaConoscenza = Cst_EmailMittente.Trim
                        ' ''Cst_EmailCopiaConoscenza.Replace(" ", "")
                        ' ''.BCCAddress = Cst_EmailCopiaConoscenza.Trim
                        .BCCAddress = ""

                        .SMTPPort = Cst_EmailSmtpPort
                        .SMTPServer = Cst_EmailSmtp
                        If Cst_EmailPassword <> "" Then
                            .SMTPAutenticazione = True
                            .SMTPPassword = Cst_EmailPassword
                            .SMTPUser = Cst_EmailUtente
                        End If

                        'OGGETTO
                        .Subject = "LOG SALVATAGGIO AUTOMATICO E ALTRE OPERAZIONI : " & TmpNomeFileNEW
                        .Body = "<!doctype html><html xmlns='http://www.w3.org/1999/xhtml'> " & _
                                      "<head>" & _
                                      "<title>LOG SALVATAGGIO AUTOMATICO E ALTRE OPERAZIONI : " & TmpNomeFileNEW & "</title>" & _
                                      "<meta charset='utf-8'/> " & _
                                      "<meta name='viewport' content='width=device-width, initial-scale=1, maximum-scale=1'/>" & _
                                      "</style>" & _
                                      "</head>" & _
                                      "<body>"
                        .Body += "Buongiorno, <br /><br />"
                        .Body += "<b><u>LOG SALVATAGGIO AUTOMATICO E ALTRE OPERAZIONI DA CONTROLLARE</u></b><br/><br/>"
                        .Body += "<font color=""red""><b>Qualora dovesse presentare delle anomalie, non esitate a contattare la ns assistenza tecnica.</b></font><br /><br />"
                        .Body += "Cordiali Saluti. <br /><br />"
                        .Body += "<img src='cid:LogoEmail1'><br /><br />"
                        If Cst_EmailMittente.Trim <> Cst_EmailMittenteDescrizione.Trim Then
                            .Body += "<b>" & Cst_EmailMittenteDescrizione.ToUpper.Trim & "</b><br />"
                            .Body += "<b>" & Cst_EmailMittenteCarica.Trim & "</b><br />"
                            ' ''.Body += "<a href=""mailto:" & Cst_EmailMittente.ToLower.Trim & "?Subject=Comunicazione"" target=""_top"">" & Cst_EmailMittente.ToLower.Trim & "</a><br /><br />"
                        End If
                        .Body += "<b>IREDEEM S.p.A.</b><br />"
                        .Body += "Piazza dei Martiri 1943-1945 n.1, 40121 Bologna  -  T +39 051 0935879  F +39 051 0935882 <br />"
                        .Body += "<a href=""http://www.iredeem.it/"">www.iredeem.it</a><br /><br />"
                        .Body += "<span style='font-size:10.0pt;'>Dealer in Emergency Care & Resuscitation</span><br />"
                        .Body += "<img src='cid:LogoEmail2'><br /><br />"
                        ' ''.Body += "<span style='font-size:10.0pt;'>Per essere sempre informato seguici sui social</span><br />"
                        .Body += "</body>" & _
                                 "</html>"
                        '-
                        .Logo1 = Cst_PathLoghi & "\LogoEmail1.Gif"
                        .Logo2 = Cst_PathLoghi & "\LogoEmail2.Gif"
                        If Not .SendMailMessage(False) Then
                            ErrMess += " ERRORE INVIO - InviaEmailLogBK: " & TmpNomeFileNEW
                            LblInvio.Text = ErrMess
                            Call ScriviFileLog(ErrMess)
                            Exit Function
                        End If
                    End With
                    '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    'Infine cancello il LOG che ha copiato il SOFTBK  COSI NON VERRA' PIU' INVIATO
                    File.Delete(AddDirSep(PathFileLogBK) & TmpNomeFile)
                    'OK FINITO
                    objEmail.Dispose()
                    objEmail = Nothing
                    Call ScriviFileLog("Termine Controllo InviaEmailLogBK: file LogBK inviato con successo: " & TmpNomeFileNEW)
                End If
            Else
                Call ScriviFileLog("Termine Controllo InviaEmailLogBK: nessun file LogBK da inviare.: " & TmpNomeFileNEW)
            End If
        Catch ex As Exception
            InviaEmailLogBK = False
            ErrMess = "ERRORE DESTINATARIO - InviaEmailLogBK: " & TmpNomeFileNEW & " - Err.: " & ex.Message
            LblInvio.Text = ErrMess
            Call ScriviFileLog(ErrMess)
            Exit Function
        End Try
    End Function
End Class
