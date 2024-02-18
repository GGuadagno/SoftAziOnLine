Option Explicit On 

Imports System.Text
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Configuration

Public Class clsEMailSender
    Implements IDisposable

    Private PrvFromAddress As String = ""
    Private PrvFromDisplayAddress As String = ""
    Private PrvCaricaMittente As String = ""
    Private PrvToAddress As String = ""
    Private PrvCCAddress As String = ""
    Private PrvBCCAddress As String = ""
    Private PrvSubject As String = ""
    Private PrvBody As String = ""
    Private PrvPriority As Integer = 0
    Private PrvAuthenticationMethod As Integer = 1
    '-----------------
    ' Versione 1.1.0.0
    '-----------------
    Private PrvAttachmentArray As ArrayList
    Private PrvSMTPServer As String = ""

    Private PrvSMTPPort As Integer = 25
    Private PrvPOPServer As String = ""
    Private PrvUser As String = ""
    Private PrvPassword As String = ""
    Private PrvDominio As String = ""
    ' -----------------------
    Private PrvUserSMTP As String = ""
    Private PrvPasswordSMTP As String = ""
    Private PrvAutenticazioneSMTP As Boolean = False
    Private PrvLogo1 As String = ""
    Private PrvLogo2 As String = ""

    Public Property Logo1() As String
        Get
            Return Me.PrvLogo1
        End Get
        Set(ByVal value As String)
            Me.PrvLogo1 = value
        End Set
    End Property
    Public Property Logo2() As String
        Get
            Return Me.PrvLogo2
        End Get
        Set(ByVal value As String)
            Me.PrvLogo2 = value
        End Set
    End Property

    Public Property AttachmentArray() As ArrayList
        Get
            Return PrvAttachmentArray
        End Get
        Set(ByVal Value As ArrayList)
            PrvAttachmentArray = Value
        End Set
    End Property

    Public Property SMTPAutenticazione() As Boolean
        Get
            Return PrvAutenticazioneSMTP
        End Get
        Set(ByVal Value As Boolean)
            PrvAutenticazioneSMTP = Value
        End Set
    End Property

    Public Property SMTPUser() As String
        Get
            Return PrvUserSMTP
        End Get
        Set(ByVal Value As String)
            PrvUserSMTP = Value
        End Set
    End Property

    Public Property SMTPPassword() As String
        Get
            Return PrvPasswordSMTP
        End Get
        Set(ByVal Value As String)
            PrvPasswordSMTP = Value
        End Set
    End Property
    '-----------------
    ' Versione 1.2.0.0
    '-----------------
    Private PrvMailDaInviare As String = ""
    '-----------------
    '
    ' Indirizzo del mittente
    '
    Public Property FromAddress() As String
        Get
            FromAddress = PrvFromAddress
        End Get
        Set(ByVal Value As String)
            PrvFromAddress = Value
        End Set
    End Property
    '
    ' Nome del mittente
    '
    Public Property FromDisplayAddress() As String
        Get
            Return PrvFromDisplayAddress
        End Get
        Set(ByVal Value As String)
            PrvFromDisplayAddress = Value
        End Set
    End Property
    Public Property CaricaMittente() As String
        Get
            Return PrvCaricaMittente
        End Get
        Set(ByVal Value As String)
            PrvCaricaMittente = Value
        End Set
    End Property
    '
    ' Indirizzo del destinatario
    '
    Public Property ToAddress() As String
        Get
            ToAddress = PrvToAddress
        End Get
        Set(ByVal Value As String)
            PrvToAddress = Value
        End Set
    End Property
    '
    ' Indirizzo per Copia per conoscenza
    '
    Public Property CCAddress() As String
        Get
            CCAddress = PrvCCAddress
        End Get
        Set(ByVal Value As String)
            PrvCCAddress = Value
        End Set
    End Property

    Public Property BCCAddress() As String
        Get
            BCCAddress = PrvBCCAddress
        End Get
        Set(ByVal Value As String)
            PrvBCCAddress = Value
        End Set
    End Property
    '
    ' Soggetto del messaggio
    '
    Public Property Subject() As String
        Get
            Subject = PrvSubject
        End Get
        Set(ByVal Value As String)
            PrvSubject = Value
        End Set
    End Property
    '
    ' Corpo del messaggio
    '
    Public Property Body() As String
        Get
            Body = PrvBody
        End Get
        Set(ByVal Value As String)
            PrvBody = Value
        End Set
    End Property
    '
    ' Priorità
    ' Normale = 1
    '
    Public Property Priority() As Integer
        Get
            Priority = PrvPriority
        End Get
        Set(ByVal Value As Integer)
            PrvPriority = Value
        End Set
    End Property
    '
    ' 1 = codifica base64
    '
    Public Property AuthenticationMethod() As Integer
        Get
            AuthenticationMethod = PrvAuthenticationMethod
        End Get
        Set(ByVal Value As Integer)
            PrvAuthenticationMethod = Value
        End Set
    End Property
    '
    ' Porta 25
    '
    Public Property SMTPPort() As Integer
        Get
            SMTPPort = PrvSMTPPort
        End Get
        Set(ByVal Value As Integer)
            PrvSMTPPort = Value
        End Set
    End Property

    Public WriteOnly Property MailDaInviare() As String
        Set(ByVal Value As String)
            Me.PrvMailDaInviare = Value
        End Set
    End Property
    '-----------------
    '
    ' Server della posta in uscita
    '
    Public Property SMTPServer() As String
        Get
            SMTPServer = PrvSMTPServer
        End Get
        Set(ByVal Value As String)
            PrvSMTPServer = Value
        End Set
    End Property

    Public Property Dominio() As String
        Get
            Dominio = PrvDominio
        End Get
        Set(ByVal Value As String)
            PrvDominio = Value
        End Set
    End Property

    Public Property User() As String
        Get
            User = PrvUser
        End Get
        Set(ByVal Value As String)
            PrvUser = Value
        End Set
    End Property

    Public Property Password() As String
        Get
            Password = PrvPassword
        End Get
        Set(ByVal Value As String)
            PrvPassword = Value
        End Set
    End Property

    Public Sub New()

    End Sub

    Private TcpC As System.Net.Sockets.TcpClient
    Private Const IntestaMessaggio As String = "Invio posta elettronica"
    ''' <summary>
    ''' Sends an email message
    ''' </summary>
    Public Function SendMailMessage(Optional ByVal SWEnglese As Boolean = False) As Boolean
        ' Instantiate a new instance of MailMessage
        '------------

        Dim mMailMessage As New MailMessage()
        Try
            ' Set the sender address of the mail message
            mMailMessage.From = New MailAddress(Me.PrvFromAddress, Me.PrvFromDisplayAddress)
            ' Set the recepient address of the mail message
            Dim IndirizziTO() As String
            Dim IndirizziCC() As String
            Dim IndirizziBCC() As String
            Dim i As Integer
            PrvToAddress = PrvToAddress.Replace(",", ";")
            If PrvToAddress.IndexOf(";") <> -1 Then
                IndirizziTO = Split(PrvToAddress, ";", , CompareMethod.Text)
            Else
                ReDim IndirizziTO(0)
                IndirizziTO(0) = PrvToAddress
            End If
            For i = 0 To IndirizziTO.Length - 1
                IndirizziTO(i) = IndirizziTO(i).Replace(" ", "") 'GIU170916
                If IndirizziTO(i).Trim <> "" Then 'giu040119
                    mMailMessage.To.Add(New MailAddress(IndirizziTO(i)))
                End If
            Next

            ' Check if the bcc value is nothing or an empty string
            If Not Me.PrvBCCAddress Is Nothing And Me.PrvBCCAddress <> String.Empty Then
                ' Set the Bcc address of the mail message
                PrvBCCAddress = PrvBCCAddress.Replace(",", ";")
                PrvBCCAddress = PrvBCCAddress.Replace(" ", "") 'GIU170916
                If PrvBCCAddress.IndexOf(";") <> -1 Then
                    IndirizziBCC = Split(PrvBCCAddress, ";", , CompareMethod.Text)
                Else
                    ReDim IndirizziBCC(0)
                    IndirizziBCC(0) = PrvBCCAddress
                End If
                For i = 0 To IndirizziBCC.Length - 1
                    IndirizziBCC(i) = IndirizziBCC(i).Replace(" ", "") 'GIU170916
                    If IndirizziBCC(i).Trim <> "" Then 'giu040119
                        mMailMessage.Bcc.Add(New MailAddress(IndirizziBCC(i)))
                    End If
                Next
            End If

            ' Check if the cc value is nothing or an empty value
            If Not Me.PrvCCAddress Is Nothing And Me.PrvCCAddress <> String.Empty Then
                ' Set the CC address of the mail message
                PrvCCAddress = PrvCCAddress.Replace(",", ";")
                PrvCCAddress = PrvCCAddress.Replace(" ", "") 'GIU170916
                If PrvCCAddress.IndexOf(";") <> -1 Then
                    IndirizziCC = Split(PrvCCAddress, ";", , CompareMethod.Text)
                Else
                    ReDim IndirizziCC(0)
                    IndirizziCC(0) = PrvCCAddress
                End If
                For i = 0 To IndirizziCC.Length - 1
                    IndirizziCC(i) = IndirizziCC(i).Replace(" ", "") 'GIU170916
                    If IndirizziCC(i).Trim <> "" Then 'giu040119
                        mMailMessage.CC.Add(New MailAddress(IndirizziCC(i)))
                    End If
                Next
            End If

            ' Set the subject of the mail message
            mMailMessage.Subject = Me.PrvSubject
            ' Set the body of the mail message
            mMailMessage.Body = Me.PrvBody
            If SWEnglese = False Then
                mMailMessage.Body += "<br />" _
                    & "<span style='font-size:10.0pt;'>Le informazioni contenute nella presente comunicazione e i relativi allegati possono essere riservate e sono, comunque, destinate esclusivamente alle persone o alla Societa' sopraindicati. " _
                    & "La diffusione, distribuzione e/o copiatura del documento trasmesso da parte di qualsiasi soggetto diverso dal destinatario e' proibita ai sensi del Regolamento UE 2016/679. " _
                    & "Se avete ricevuto questo messaggio per errore, Vi preghiamo di distruggerlo e di informarci immediatamente per telefono al numero +39 051 0935879 o inviando un messaggio all'indirizzo e-mail <a href=""mailto:commerciale@iredeem.it?Subject=Comunicazione"" target=""_top"">commerciale@iredeem.it</a>" _
                    & "</span><br />" _
                    & "<span style='font-size:10.0pt;'>The information contained in this message may be confidential and legally protected under applicable law, " _
                    & "EU Regulation 2016/679. " _
                    & "The message is intended solely for the addressee(s). If you are not the intended recipient, you are hereby notified that any use, forwarding, dissemination, or reproduction of this message " _
                    & "is strictly prohibited and may be unlawful. If you are not the intended recipient, please contact the sender by return e-mail and destroy all copies of the original message. " _
                    & "</span><br />"
            Else
                mMailMessage.Body += "<br />" _
                    & "<span style='font-size:10.0pt;'>The information contained in this message may be confidential and legally protected under applicable law, " _
                    & "EU Regulation 2016/679. " _
                    & "The message is intended solely for the addressee(s). If you are not the intended recipient, you are hereby notified that any use, forwarding, dissemination, or reproduction of this message " _
                    & "is strictly prohibited and may be unlawful. If you are not the intended recipient, please contact the sender by return e-mail and destroy all copies of the original message. " _
                    & "</span><br />"
            End If

            'ALTRO ESEMPIO IN INGLESE 
            'This electronic mail transmission may contain confidential information addressed only to the person(s) named. 
            'Any use, distribution, copying or disclosure by any other person and/or entities other than the 
            'intended recipient is prohibited. If you received this transmission in error, please inform the 
            'sender immediately and delete the material.

            ' Set the format of the mail message body as HTML
            mMailMessage.IsBodyHtml = True
            ' Set the priority of the mail message to normal
            mMailMessage.Priority = MailPriority.Normal

            Dim myAttachment As Attachment
            Dim myAttachmentPathName As String

            If Not Me.PrvAttachmentArray Is Nothing Then
                If Me.PrvAttachmentArray.Count > 0 Then
                    For Each myAttachmentPathName In Me.PrvAttachmentArray
                        myAttachment = New Attachment(myAttachmentPathName)
                        mMailMessage.Attachments.Add(myAttachment)
                    Next
                End If
            End If

            If Me.PrvLogo1 <> "" Then
                Dim LinkedImage As New LinkedResource(Me.Logo1)
                LinkedImage.ContentId = "LogoEmail1"
                If InStr(Me.PrvLogo1, "Jpg", CompareMethod.Text) > 0 Or InStr(Me.PrvLogo1, "Jpeg", CompareMethod.Text) Then
                    LinkedImage.ContentType = New System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Image.Jpeg)
                ElseIf InStr(Me.PrvLogo1, "Gif", CompareMethod.Text) > 0 Then
                    LinkedImage.ContentType = New System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Image.Gif)
                Else
                    LinkedImage.ContentType = New System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Image.Jpeg)
                End If
                Dim htmlView As AlternateView = AlternateView.CreateAlternateViewFromString(mMailMessage.Body, Nothing, "text/html")
                htmlView.LinkedResources.Add(LinkedImage)
                mMailMessage.AlternateViews.Add(htmlView)
            End If
            If Me.PrvLogo2 <> "" Then
                Dim LinkedImage As New LinkedResource(Me.Logo2)
                LinkedImage.ContentId = "LogoEmail2"
                If InStr(Me.PrvLogo2, "Jpg", CompareMethod.Text) > 0 Or InStr(Me.PrvLogo2, "Jpeg", CompareMethod.Text) Then
                    LinkedImage.ContentType = New System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Image.Jpeg)
                ElseIf InStr(Me.PrvLogo2, "Gif", CompareMethod.Text) > 0 Then
                    LinkedImage.ContentType = New System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Image.Gif)
                Else
                    LinkedImage.ContentType = New System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Image.Jpeg)
                End If
                Dim htmlView As AlternateView = AlternateView.CreateAlternateViewFromString(mMailMessage.Body, Nothing, "text/html")
                htmlView.LinkedResources.Add(LinkedImage)
                mMailMessage.AlternateViews.Add(htmlView)
            End If

            ' Instantiate a new instance of SmtpClient
            Dim mSmtpClient As New SmtpClient()

            mSmtpClient.Host = SMTPServer
            mSmtpClient.Port = SMTPPort

            If Me.PrvAutenticazioneSMTP Then
                mSmtpClient.Credentials = New NetworkCredential(Me.PrvUserSMTP, Me.PrvPasswordSMTP)
            End If

            ' Send the mail message

            mSmtpClient.Send(mMailMessage)
            Return True
        Catch ex As Exception
            ErrMess = "Errore invio E-mail: " & PrvToAddress.Trim & " ERR.: " & ex.Message.Trim
            ScriviFileLog(ErrMess)
            Return False
        Finally
            mMailMessage.Dispose()
            mMailMessage = Nothing
        End Try
    End Function

    Private disposedValue As Boolean = False        ' Per rilevare chiamate ridondanti

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: liberare altro stato (oggetti gestiti).
            End If

            ' TODO: liberare lo stato personale (oggetti non gestiti).
            ' TODO: impostare campi di grandi dimensioni su null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' Questo codice è aggiunto da Visual Basic per implementare in modo corretto il modello Disposable.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Non modificare questo codice. Inserire il codice di pulitura in Dispose(ByVal disposing As Boolean).
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

   
End Class
