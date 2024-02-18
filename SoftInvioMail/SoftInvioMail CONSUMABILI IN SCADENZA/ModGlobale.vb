Imports System.Configuration
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Module ModGlobale
    Public Const AppName As String = "SoftInvioMail"
    'gestione password nel codice NOTA SONO PRESENTI NEI MODULI: SOFTAZI ONLINE, SOFTCOGESQL
    Public Const KeyPWDSoftSo As String = "SoftSolutions"
    Public Const KeyPWDIredeem As String = "Iredeem"
    Public Const KeyPWDSfera As String = "Sfera"

    Public Const PWDDBSoftSo As String = "%tgb6yhn"
    Public Const PWDDBIredeem As String = "Bottic1517"
    Public Const PWDDBSfera As String = "saadm2012sfera!"

    Public KeyPWD As String
    Public ErrMess As String
    Public Esercizio As String = ""
    Public Ditta As String = ""

    Public Function GetSmartKey() As String
        GetSmartKey = ""
        Try
            If KeyPWD = "" Then
                KeyPWD = ConfigurationManager.AppSettings("ClienteDB").ToString
                If KeyPWD = "NotFound" Or KeyPWD.Trim = "" Then
                    ErrMess = "Errore, manca l'indicazione del cliente per la password del DB."
                    Exit Function
                End If
            End If

            If UCase(KeyPWD) = UCase(KeyPWDSoftSo) Then
                GetSmartKey = PWDDBSoftSo
            ElseIf UCase(KeyPWD) = UCase(KeyPWDIredeem) Then
                GetSmartKey = PWDDBIredeem
            ElseIf UCase(KeyPWD) = UCase(KeyPWDSfera) Then
                GetSmartKey = PWDDBSfera
            Else
                ErrMess = "Password del database per il cliente " & KeyPWD & " non definita."
                GetSmartKey = ""
                Exit Function
            End If
        Catch ex As Exception
            ErrMess = "Errore: " & ex.Message
        End Try

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

    Public Function ScriviFileLog(ByVal StrMessaggio As String) As Boolean
        ScriviFileLog = False

        Dim Cst_PathFileLog As String
        Cst_PathFileLog = System.Configuration.ConfigurationManager.AppSettings("PathFileLog")
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
            TmpNomeFile = "LogEmail_" & Format(Now, "yyyyMMdd") & ".txt"
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
        If Mid(GetChiaveOP(), 1, 22) = "PREPARA EMAIL IN CORSO" Then
            AggChiaveOP = True
            Exit Function
        End If
        If SWServizioEmail() = False Then
            AggChiaveOP = True
            Exit Function
        End If
        Try
            Dim myCn As SqlConnection
            Dim myCmd As New SqlCommand
            Dim myAdapt As New SqlDataAdapter
            Dim SqlStr As String
            Dim ConnStringOP As String = ConfigurationManager.AppSettings("ConnDBOP").ToString
            ConnStringOP += ";Password=" & GetSmartKey()
            myCn = New SqlConnection(ConnStringOP)
            myCn.Open()
            strMessaggio = Mid(Controlla_Apice(strMessaggio), 1, 50)
            SqlStr = "IF EXISTS(SELECT Chiave FROM Abilitazioni WHERE Chiave='InvioEmail') " _
                     & " Update Abilitazioni SET Descrizione='" & strMessaggio & "' WHERE Chiave='InvioEmail' ELSE " _
                     & " INSERT INTO Abilitazioni VALUES ('InvioEmail', '" & strMessaggio & "', -1)"
            myCmd.CommandText = SqlStr
            myCmd.Connection = myCn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandTimeout = 120
            myCmd.ExecuteNonQuery()
            myCn.Close() : myCn = Nothing
            '---------
        Catch ex As Exception
            ErrMess = "Errore scrittura Abilitazioni.InvioEmail " & ex.Message.Trim
        End Try
        AggChiaveOP = True
    End Function

    Public Function GetChiaveOP() As String
        GetChiaveOP = ""
        Try
            Dim myCn As SqlConnection
            Dim myCmd As New SqlCommand
            Dim myAdapt As New SqlDataAdapter
            Dim DsGetOpzioni As New DataSet
            Dim SqlStr As String
            Dim ConnStringOP As String = ConfigurationManager.AppSettings("ConnDBOP").ToString
            ConnStringOP += ";Password=" & GetSmartKey()
            myCn = New SqlConnection(ConnStringOP)
            myCn.Open()
            SqlStr = "Select * FROM Abilitazioni WHERE Chiave='InvioEmail'"
            myCmd.CommandText = SqlStr
            myCmd.Connection = myCn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandTimeout = 0
            myAdapt.SelectCommand = myCmd
            myAdapt.Fill(DsGetOpzioni)
            myCn.Close() : myCn = Nothing
            GetChiaveOP = IIf(IsDBNull(DsGetOpzioni.Tables(0).Rows(0).Item("Descrizione")), "", DsGetOpzioni.Tables(0).Rows(0).Item("Descrizione"))
            DsGetOpzioni = Nothing
            '---------
        Catch ex As Exception
            GetChiaveOP = ""
            ErrMess = "Errore lettura Abilitazioni.InvioEmail " & ex.Message.Trim
        End Try

    End Function
    'giu310718
    Public Function SWServizioEmail() As Boolean
        SWServizioEmail = True
        Try
            Dim myCn As SqlConnection
            Dim myCmd As New SqlCommand
            Dim myAdapt As New SqlDataAdapter
            Dim SqlStr As String
            Dim ConnStringOP As String = ConfigurationManager.AppSettings("ConnDBNNAAAA").ToString
            ConnStringOP = ConnStringOP.Replace("NNAAAA", Ditta.Trim & Esercizio.Trim)
            Call ScriviFileLog("Connessione a: " & ConnStringOP)
            ConnStringOP += ";Password=" & GetSmartKey()
            Dim DsGetOpzioni As New DataSet
            myCn = New SqlConnection(ConnStringOP)
            myCn.Open()
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
            If DsGetOpzioni.Tables(0).Select().Length > 0 Then
                SWServizioEmail = DsGetOpzioni.Tables(0).Rows(X).Item("AIServizioEmail")
            End If
            DsGetOpzioni = Nothing
        Catch ex As Exception
            SWServizioEmail = False
            ErrMess = "Errore lettura ParametriGeneraliAZI.AIServizioEmail " & ex.Message.Trim
        End Try

    End Function

    'GIU050718
    Public Function GetDatoOperatore(ByVal _NomeOp As String, ByVal _Email As String, ByRef _Dato As String, ByRef _Errore As String) As Boolean
        GetDatoOperatore = True
        Try
            Dim myCn As SqlConnection
            Dim myCmd As New SqlCommand
            Dim myAdapt As New SqlDataAdapter
            Dim DsGetOpzioni As New DataSet
            Dim SQLStr As String = ""
            Dim ConnDBInstall As String = ConfigurationManager.AppSettings("ConnDBInstall").ToString
            ErrMess = ConnDBInstall
            Call ScriviFileLog(ErrMess)
            ConnDBInstall += ";Password=" & GetSmartKey()
            Dim DsGetDatoOperatore As New DataSet
            myCn = New SqlConnection(ConnDBInstall)
            myCn.Open()
            If _NomeOp.Trim <> "" Then
                SQLStr = "SELECT * FROM Operatori WHERE Nome='" & Controlla_Apice(_NomeOp.Trim) & "'"
            Else
                SQLStr = "SELECT * FROM Operatori WHERE Email='" & Controlla_Apice(_Email.Trim) & "'"
            End If
            ErrMess = SQLStr
            Call ScriviFileLog(ErrMess)
            myCmd.CommandText = SQLStr
            myCmd.Connection = myCn
            myCmd.CommandType = CommandType.Text
            myCmd.CommandTimeout = 0
            myAdapt.SelectCommand = myCmd
            myAdapt.Fill(DsGetDatoOperatore)
            myCn.Close() : myCn = Nothing
            '-
            If (DsGetDatoOperatore.Tables.Count > 0) Then
                If (DsGetDatoOperatore.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(DsGetDatoOperatore.Tables(0).Rows(0).Item(_Dato)) Then
                        _Dato = DsGetDatoOperatore.Tables(0).Rows(0).Item(_Dato).ToString.Trim
                    Else
                        _Dato = ""
                    End If
                Else
                    _Dato = ""
                End If
            Else
                _Dato = ""
            End If
        Catch ex As Exception
            _Dato = ""
            _Errore = ex.Message.Trim
            GetDatoOperatore = False
        End Try
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
End Module
