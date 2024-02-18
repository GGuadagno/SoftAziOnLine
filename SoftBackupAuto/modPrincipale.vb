Option Explicit On 

Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Configuration
Imports System.Configuration.ConfigurationSettings
Imports System.IO

Module modPrincipale
   
    ' Nome applicazione per localizzazione
    Public Const AppName As String = "SoftBackupAuto"
    Public Const MyAppName As String = "SoftBackupAuto"
    
    Public objConnection As SqlConnection
    Public objCommand As SqlCommand
    Public SQLstr As String

    ' Proprietà controlli
    Public vbPronto As Color = Color.Yellow
    Public vbSfondo As Color = Color.Silver
    Public vbSfondoErrore As Color = Color.Red

    Public FormatoData As String = "dd/MM/yyyy"
    Public FormatoOra As String = "HH.mm"
    Public Const FormatoDataOra As String = "dd/MM/yyyy HH:mm"

    Public g_SQLTimeout As Integer = Val(SqlTimeout)
    
    'giu290809 giu310809
    Public SWBkAttivo As Boolean = False : Public SWBkConfig As Boolean = False
    Public SWBKAuto As Boolean = False : Public EsitoBKAuto As String = "" : Public SWGGEsito As Integer = 0
    Public SWBKManuale As Boolean = False 'giu260518
    '
    Public NomeDB As String

    'giu120918
    Public ErrMess As String = ""
    Public Function ScriviFileLog(ByVal StrMessaggio As String) As Boolean
        ScriviFileLog = False

        Dim Cst_PathFileLog As String
        Cst_PathFileLog = getConfigurationString("PercorsoFilesBackup")
        Dim PathLog As String = AddDirSep(Cst_PathFileLog)
        Try
            If Not Directory.Exists(PathLog) Then
                Directory.CreateDirectory(PathLog)
            End If
        Catch E As Exception
            ErrMess = "Impossibile creare la cartella " & PathLog & " necessaria per la scrittura del file LOG."
            Exit Function
        End Try

        Try
            Dim TmpNomeFile As String
            Dim RecordFile As StreamWriter
            TmpNomeFile = "LogBKAuto" & Format(Now, "yyyyMMdd") & ".txt"
            RecordFile = File.AppendText(AddDirSep(PathLog) & TmpNomeFile)
            RecordFile.WriteLine(Now & " - " & StrMessaggio)
            RecordFile.Close()
        Catch ex As Exception
            ErrMess = "Impossibile scrivere LOG " & PathLog
            Exit Function
        End Try

        ScriviFileLog = True
    End Function
    Public Function AggChiaveOP(ByVal strMessaggio As String) As Boolean
        AggChiaveOP = False
        Try
            Dim strConnDB As String = ""
            Dim strNomeDB As String = ""
            Dim strDitta As String = getConfigurationString("Ditta")
            If strDitta.Trim = "" Then
                ScriviFileLog("* * * ATTENZIONE * * * ERRORE Config: non è stato definito il codice Ditta")
                Exit Function
            End If
            strNomeDB = strDitta & "Opzioni"
            strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()

            Dim myCn As SqlConnection
            Dim myCmd As New SqlCommand
            Dim myAdapt As New SqlDataAdapter
            Dim SqlStr As String
            myCn = New SqlConnection(strConnDB)
            myCn.Open()
            strMessaggio = Mid(Controlla_Apice(Now & " - " & strMessaggio), 1, 50)
            SqlStr = "IF EXISTS(SELECT Chiave FROM Abilitazioni WHERE Chiave='BKAuto') " _
                     & " Update Abilitazioni SET Descrizione='" & strMessaggio & "' WHERE Chiave='BKAuto' ELSE " _
                     & " INSERT INTO Abilitazioni VALUES ('BKAuto', '" & strMessaggio & "', -1)"
            myCmd.CommandText = SqlStr
            myCmd.Connection = myCn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandTimeout = 120
            myCmd.ExecuteNonQuery()
            myCn.Close() : myCn = Nothing
            '---------
        Catch ex As Exception
            ErrMess = "Errore scrittura Abilitazioni.BKAuto " & ex.Message.Trim
            ScriviFileLog(ErrMess)
        End Try
        AggChiaveOP = True
    End Function
    Public Function Controlla_Apice(ByVal Stringa_In As String) As String

        Dim X As Integer
        Dim Y As Integer

        Dim Comodo As String

        X = InStr(Stringa_In, "'")
        If X > 0 Then
            Y = 1
            Comodo = Stringa_In
            Do Until X <= 0
                'giu010416 Comodo = Mid(Comodo, Y, X) + "'" + Right(Comodo, Len(Comodo) - X)
                Comodo = Mid(Comodo, 1, X) + "'" + Right(Comodo, Len(Comodo) - X)
                Y = X + 2
                X = InStr(Y, Comodo, "'")
            Loop
            Controlla_Apice = Comodo
        Else
            Controlla_Apice = Stringa_In
        End If

    End Function
    Public Function AddDirSep(ByVal strPathName As String) As String
        Const gstrSEP_URLDIR = "//"
        Const gstrSEP_DIR = "\"

        'Aggiunge lo \ al path quando manca

        strPathName = RTrim$(strPathName)
        If Right$(strPathName, Len(gstrSEP_URLDIR)) <> gstrSEP_URLDIR Then
            If Right$(strPathName, Len(gstrSEP_DIR)) <> gstrSEP_DIR Then
                strPathName = strPathName & gstrSEP_DIR
            End If

        End If
        AddDirSep = strPathName
    End Function
    Public Function SendLogBK() As Boolean
        SendLogBK = True
        Dim PathFileLogBK As String = getConfigurationString("PathFileLogBK")
        If PathFileLogBK.Trim <> "" Then
            Try
                If Not Directory.Exists(PathFileLogBK.Trim) Then
                    Directory.CreateDirectory(PathFileLogBK.Trim)
                End If
                Dim Cst_PathFileLog As String
                Cst_PathFileLog = getConfigurationString("PercorsoFilesBackup")
                Dim PathLog As String = AddDirSep(Cst_PathFileLog)
                Dim TmpNomeFile As String = "LogBKAuto" & Format(Now, "yyyyMMdd") & ".txt"
                If File.Exists(AddDirSep(PathFileLogBK) & TmpNomeFile) = True Then
                    File.Delete(AddDirSep(PathFileLogBK) & TmpNomeFile)
                End If
                File.Copy(AddDirSep(PathLog) & TmpNomeFile, AddDirSep(PathFileLogBK) & TmpNomeFile)
            Catch ex As Exception
                SendLogBK = False
            End Try
        End If
    End Function

    Sub Main()
        Cursor.Current = Cursors.AppStarting

        Dim frmStatus As New frmStatus()
        frmStatus._Messaggio = "Caricamento Backup automatico in corso, attendere ..."
        frmStatus.Show()
        frmStatus.Refresh()

        Dim objControlli As New clsControlli()

        'AttivaSoftMasterizza()

        vbPronto = System.Drawing.Color.FromKnownColor(KnownColor.Window)
        vbSfondo = System.Drawing.Color.FromKnownColor(KnownColor.Control)
        vbSfondoErrore = System.Drawing.Color.FromKnownColor(KnownColor.Red)
        '
        '--------------------------------------------------------------------------------------------------
        '
        ' Verifica se c'è già una istanza aperta del programma,
        ' se c'è esce
        '
        If PrevInstance(AppName) Then
            ' ''MsgBox("Risulta aperta una sessione di [Backup automatico].", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, AppName)
            ScriviFileLog("* * * ATTENZIONE * * * Risulta aperta una sessione di [Backup automatico].")
            frmStatus._Messaggio = "* * * ATTENZIONE * * * Risulta aperta una sessione di [Backup automatico]."
            frmStatus.Refresh()
            System.Threading.Thread.Sleep(60000)
        ElseIf PrevInstance("SoftTickets") Then
            ' ''MsgBox("Risulta aperta una sessione di [SoftTickets Administrator].", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, AppName)
            ScriviFileLog("* * * ATTENZIONE * * * Risulta aperta una sessione di [SoftTickets Administrator].")
            frmStatus._Messaggio = "* * * ATTENZIONE * * * Risulta aperta una sessione di [SoftTickets Administrator]."
            frmStatus.Refresh()
            System.Threading.Thread.Sleep(60000)
        ElseIf PrevInstance("SoftTicketsCassa") Then
            ' ''MsgBox("Risulta aperta una sessione di [SoftTickets Cassa].", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, AppName)
            ScriviFileLog("* * * ATTENZIONE * * * Risulta aperta una sessione di [SoftTickets Cassa].")
            frmStatus._Messaggio = "* * * ATTENZIONE * * * Risulta aperta una sessione di [SoftTickets Cassa]."
            frmStatus.Refresh()
            System.Threading.Thread.Sleep(60000)
        Else
            frmStatus.Close()
            frmStatus = Nothing
            Cursor.Current = Cursors.Default
            '
            ' Controllo database accessibile
            '
            If funCheckDBOK("Master") Then
                Dim frmBkAuto As New frmBKAuto()
                frmBkAuto.ShowDialog()
                frmBkAuto = Nothing

            Else
                '
                ' Database non accessibile
                '
                ' ''MsgBox("Il database non è accessibile.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical, AppName)
                ScriviFileLog("* * * ATTENZIONE * * * Il database non è accessibile.")
                frmStatus._Messaggio = "* * * ATTENZIONE * * * Il database non è accessibile."
                frmStatus.Refresh()
                System.Threading.Thread.Sleep(60000)
            End If
        End If

        objControlli = Nothing

        If Not frmStatus Is Nothing Then
            frmStatus.Close()
            frmStatus = Nothing
        End If

        End
    End Sub

    Public Function getConfigurationString(ByVal Key As String) As String
        Dim Valore As String = ""
        Try
            Valore = AppSettings.Get(Key)
            If Valore Is Nothing Then
                'MsgBox("Si è verificato un errore nella lettura della configurazione per il ripristino del database [SoftTickets]: " & Chr(Keys.Return) _
                '& "non è stato impostato il valore per la chiave [" & Key & "]." & Chr(Keys.Return) & Chr(Keys.Return) _
                '& "Il ripristino del database sarà ora interrotto.", MsgBoxStyle.OKOnly Or MsgBoxStyle.Information, IntestazioneMessaggi)
                Valore = ""
            End If
        Catch ex As Exception
            Valore = ""
        End Try
        Return Valore
    End Function
    Public Function SqlTimeout() As String
        Try
            SqlTimeout = ConfigurationSettings.AppSettings.Get("SqlTimeout")
            If Val(SqlTimeout) = 0 Then
                SqlTimeout = "2400"
                g_SQLTimeout = "2400"
            End If
        Catch ex As Exception
            SqlTimeout = "2400"
            g_SQLTimeout = "2400"
        End Try
    End Function
    Public Function Decripta() As String
        Dim PWDSQL As String = ""
        Try
            PWDSQL = ConfigurationSettings.AppSettings.Get("PWDSQL")
        Catch ex As Exception
            PWDSQL = ""
        End Try
        If PWDSQL.Trim = "SoftSol" Then
            Decripta = "%tgb6yhn"
        ElseIf PWDSQL.Trim = "Iredeem" Then
            Decripta = "Bottic1517"
        Else
            Decripta = "%tgb6yhn"
        End If
    End Function

    Public Function getAppPath() As String
        Dim AppPathDir As String = System.Reflection.Assembly.GetExecutingAssembly.Location
        Dim ind As Integer = AppPathDir.LastIndexOf("\")
        AppPathDir = AppPathDir.Substring(0, ind)
        'Console.WriteLine(AppPathDir)
        getAppPath = AppPathDir
    End Function

    Public Function getGiorno(ByVal Numgg As Integer) As String

        Select Case Numgg
            Case 1
                getGiorno = "Lunedì"
            Case 2
                getGiorno = "Martedì"
            Case 3
                getGiorno = "Mercoledì"
            Case 4
                getGiorno = "Giovedì"
            Case 5
                getGiorno = "Venerdì"
            Case 6
                getGiorno = "Sabato"
            Case 7
                getGiorno = "Domenica"
            Case Else
                getGiorno = ""
        End Select
    End Function

    Public Function PrevInstance(ByVal CurrentProcess As String) As Boolean
        Dim TestCount As Integer = 0

        If CurrentProcess = AppName Then
            TestCount += 1
        End If
        If Diagnostics.Process.GetProcessesByName(CurrentProcess).Length > TestCount Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function funCheckDBOK(ByVal TipoDatabase As String) As Boolean
        Dim objDatabase As New clsDatabase()

        If objDatabase.SettaConnessione(objConnection, TipoDatabase) Then
            Try
                objConnection.Open()
                objConnection.Close()
                Return True
            Catch ex As Exception
                MsgBox("Si è verificato il seguente errore durante l'accesso al database: " & Chr(Keys.Return) _
                    & ex.Message, MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, AppName)
                Return False
            Finally
                objDatabase = Nothing
            End Try
        End If
    End Function

    Private Declare Function GetDiskFreeSpaceEx _
                            Lib "kernel32" _
                            Alias "GetDiskFreeSpaceExA" _
                            (ByVal lpDirectoryName As String, _
                            ByRef lpFreeBytesAvailableToCaller As Long, _
                            ByRef lpTotalNumberOfBytes As Long, _
                            ByRef lpTotalNumberOfFreeBytes As Long) As Long

    Public Function GetFreeSpace(ByVal Drive As String) As Long
        'returns free space in MB, formatted to two decimal places
        'e.g., msgbox("Free Space on C: "& GetFreeSpace("C:\") & "MB")

        Dim lBytesTotal, lFreeBytes, lFreeBytesAvailable As Long

        Dim iAns As Long

        iAns = GetDiskFreeSpaceEx(Drive, lFreeBytesAvailable, _
             lBytesTotal, lFreeBytes)
        If iAns > 0 Then

            Return BytesToMegabytes(lFreeBytes)
        Else
            Throw New Exception("Invalid or unreadable drive")
        End If
    End Function

    Public Function GetTotalSpace(ByVal Drive As String) As String
        'returns total space in MB, formatted to two decimal places
        'e.g., msgbox("Free Space on C: "& GetTotalSpace("C:\") & "MB")

        Dim lBytesTotal, lFreeBytes, lFreeBytesAvailable As Long

        Dim iAns As Long

        iAns = GetDiskFreeSpaceEx(Drive, lFreeBytesAvailable, _
             lBytesTotal, lFreeBytes)
        If iAns > 0 Then

            Return BytesToMegabytes(lBytesTotal)
        Else
            Throw New Exception("Invalid or unreadable drive")
        End If
    End Function

    Public Function BytesToMegabytes(ByVal Bytes As Long) As Long

        Dim dblAns As Double
        dblAns = (Bytes / 1024) / 1024
        BytesToMegabytes = Format(dblAns, "###,###,##0.00")

    End Function

    Sub CentraForm(ByVal frm As Form, Optional ByVal Owner As Form = Nothing, Optional ByVal PiuTop As Integer = 0, Optional ByVal PiuLeft As Integer = 0, Optional ByVal MenoTop As Integer = 0, Optional ByVal MenoLeft As Integer = 0)
        If Owner Is Nothing Then
            frm.Top = ((Screen.PrimaryScreen.Bounds.Height - frm.Height) / 2) + PiuTop - MenoTop
            frm.Left = ((Screen.PrimaryScreen.Bounds.Width - frm.Width) / 2) + PiuLeft - MenoLeft
        Else
            frm.Top = ((Owner.Height - frm.Height) / 2) + PiuTop - MenoTop
            frm.Left = ((Owner.Width - frm.Width) / 2) + PiuLeft - MenoLeft
        End If

    End Sub

End Module
