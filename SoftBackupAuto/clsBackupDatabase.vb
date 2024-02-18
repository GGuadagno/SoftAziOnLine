Option Explicit On 

Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Configuration.ConfigurationSettings

Public Class clsBackupDatabase
    Implements IDisposable

    Private SqlConnTickets As SqlConnection
    Private SqlConnectionDB As SqlConnection
    Private SqlBackupCommand As SqlCommand

    Private objDatabase As clsDatabase

    Private Const IntestazioneMessaggi As String = "Backup database"

    Private MyPosizioneBackupDB As String
    Private MyDataOraInizio As Date
    Private MyNomeFileBackup As String
    Private MyPercorsoNomeFileBackup As String
    Private MyProgressivoBackup As Integer
    Private MySubDirBackup As String
    'giu250809
    Private MyNMaxBKTieckets As Integer
    Private MyNMaxBKRiepiloghiSIAE As Integer
    Private MyNMaxBKViario As Integer
    Private NDBSaveOK As Integer = 0
    Private MyIDNomeFileBackup As String
    'giu240909
    Private MyPosizioneLOG1 As String
    Private MyPosizioneLOG2 As String
    'GIU260809 
    Private MyPosizioneBkLOG1 As String
    Private MyPosizioneBkLOG2 As String
    '---------

    Dim frmMasterizza As frmMasterizza

    Public Sub New()
        ' Inizializzazione dei componenti la classe
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        ' Inizializzazione dei componenti la classe
        Me.SqlConnTickets = New SqlConnection()
        Me.SqlConnectionDB = New SqlConnection()
        Me.SqlBackupCommand = New SqlCommand()
       
        Me.objDatabase = New clsDatabase()

        
    End Sub

    Public Sub Dispose() Implements System.IDisposable.Dispose
        ' Chiudi e scarica gli oggetti utilizzati
        Me.SqlBackupCommand.Dispose()
        Me.SqlBackupCommand = Nothing
        
        Me.SqlConnectionDB.Dispose()
        Me.SqlConnectionDB = Nothing

        Me.objDatabase = Nothing

    End Sub
    ' -----------------------
    ' Modifica del 27/11/2004
    ' -----------------------
    Public Function BackupDatabase(ByVal TipoDatabase As String, _
                                    ByVal SupportoRemovibile As Boolean, _
                                    ByVal PercorsoSupportoRemovibile As String, _
                                    ByVal ConnDatabase As String) As Boolean

        Me.SqlConnTickets.ConnectionString = IIf(ConnDatabase.Trim = "", objDatabase.getConnectionString(TipoDatabase), ConnDatabase.Trim)

        If Me.funLeggiPercorsoBackup Then 'OK SWBKAuto
            If Me.funEseguiBackup(TipoDatabase, ConnDatabase) Then 'OK SWBKAuto
                If SupportoRemovibile Then
                    'GIU090909
                    If PercorsoSupportoRemovibile.Trim = "." Then 'NON PREVISTO SUPPORTO REMOVIBILE
                        If SWBKAuto = True Then
                            EsitoBKAuto = "Scrittura su supporto removibile non definito. Backup eseguito solo su : " & MyPosizioneBackupDB
                        Else
                            MsgBox("Scrittura su supporto removibile non definito. Backup eseguito solo su : " & Chr(Keys.Return) _
                                & MyPosizioneBackupDB, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                        End If
                        ScriviFileLog("Scrittura su supporto removibile non definito. Backup eseguito solo su : " & MyPosizioneBackupDB & " (" & TipoDatabase & ")")
                        '---------
                        Me.frmMasterizza = New frmMasterizza()
                        frmMasterizza.Text = "Scrittura su supporto removibile non definito"
                        frmMasterizza.Show()
                        frmMasterizza._Label = "Scrittura su supporto removibile non definito. Backup eseguito solo su : " & MyPosizioneBackupDB
                        frmMasterizza.Refresh()
                        '--------------------------------
                        frmMasterizza.Close()
                        frmMasterizza.Dispose()
                        frmMasterizza = Nothing
                        Return True
                    End If
                    '----- ok supporto removibile 
                    Me.frmMasterizza = New frmMasterizza()
                    frmMasterizza.Text = "Backup database su supporto removibile"
                    frmMasterizza.Show()
                    frmMasterizza._Label = "Verifica disponibilità supporto removibile, attendere ..."
                    frmMasterizza.Refresh()
                    ScriviFileLog("Scrittura su supporto removibile. (" & PercorsoSupportoRemovibile.Trim & "\" & TipoDatabase & ")")
                    '---------
                    If Me.funScriviSupportoRemovibile(PercorsoSupportoRemovibile) Then 'OK SWBKAuto
                        frmMasterizza._Label = "Pulitura memorie flash, attendere ..."
                        frmMasterizza.Refresh()
                        Me.subCancellaFlash()
                    End If
                    frmMasterizza.Close()
                    frmMasterizza.Dispose()
                    frmMasterizza = Nothing
                    Return True
                Else
                    'GIU090909 NON DEVE MAI CAPITARE
                    If SWBKAuto = True Then
                        EsitoBKAuto = "ERRORE : Non è stato definito il tipo di supporto esterno per il Backup"
                    Else
                        MsgBox("Si è verificato il seguente errore durante il salvataggio : " & Chr(Keys.Return) _
                            & "Non è stato definito il tipo di supporto esterno per il Backup", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                    End If
                    ScriviFileLog("ERRORE : Non è stato definito il tipo di supporto esterno per il Backup (" & TipoDatabase & ")")
                    Return False
                End If
                ' --------------------
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    Private Function funScriviSupportoRemovibile(ByVal PercorsoSupportoRemovibile As String) As Boolean
        ' Me.MyPosizioneBackupDB + "\" + Me.MySubDirBackup, 
        ' Me.MyNomeFileBackup, Me.MySubDirBackup, Me.MyProgressivoBackup,
        ' SoftMasterizza.enTipoFile.BAK, False, SoftMasterizza.enTipoCD.CDRW)

        Dim UnitaRemovibile As String = PercorsoSupportoRemovibile.Substring(0, 2)
        Dim MBDisponibili As Integer
        Dim fs As FileStream
        Dim fLen As Integer
        '----------------------------------------------------------------------------------
        Try
            'giu280809 Salvataggio LOG anche su SupportoRemovibile
            If MyIDNomeFileBackup.Trim <> "" Then 'solo per TipoDataBase = "SoftTickets"
                'giu250809 giu260809 giu280809 Dir legato al nomeDB per il BK log
                Dim strF1 As String = UnitaRemovibile + "\" + Me.MySubDirBackup + "\" + Me.MyIDNomeFileBackup + "\Flash_01"
                Dim strF2 As String = UnitaRemovibile + "\" + Me.MySubDirBackup + "\" + Me.MyIDNomeFileBackup + "\Flash_02"
                If Not System.IO.Directory.Exists(strF1) Then
                    System.IO.Directory.CreateDirectory(strF1)
                End If
                If Not System.IO.Directory.Exists(strF2) Then
                    System.IO.Directory.CreateDirectory(strF2)
                End If
                '---------
                If funBKLOG12UnitaExt(strF1, strF2) = False Then
                    Return False
                End If

            End If
        Catch exp As Exception
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE: sull'unità " & UnitaRemovibile & " non è sufficiente oppure inesistente." & exp.Message
            Else
                MsgBox("Lo spazio disponibile sull'unità " & UnitaRemovibile & " non è sufficiente oppure inesistente. Rimuovere l'unità e collegarne una vuota." & Chr(Keys.Return) & exp.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Backup")
            End If

            Return False

        End Try
        '----------------------------------------------------------------------------------

        Try
            ' Read file and return contents
            fs = File.Open(Me.MyPosizioneBackupDB + "\" + Me.MySubDirBackup + "\" & Me.MyNomeFileBackup, FileMode.Open, FileAccess.Read)
            fLen = fs.Length
            fLen = BytesToMegabytes(fLen)
        Catch exp As Exception
            fLen = 0
        Finally
            If Not fs Is Nothing Then
                fs.Close()
            End If
        End Try
        If Directory.Exists(PercorsoSupportoRemovibile) Then
            MBDisponibili = GetFreeSpace(UnitaRemovibile)
            If fLen > MBDisponibili Then
                If SWBKAuto = True Then
                    EsitoBKAuto = "ERRORE: Lo spazio disponibile sull'unità " & UnitaRemovibile & " non è sufficiente. Rimuovere l'unità e collegarne una vuota."
                Else
                    MsgBox("Lo spazio disponibile sull'unità " & UnitaRemovibile & " non è sufficiente. Rimuovere l'unità e collegarne una vuota.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Backup")
                End If

                Return False
            Else
                frmMasterizza._Label = "Copia su supporto removibile in corso, attendere ..."
                frmMasterizza.Refresh()
                If Directory.Exists(UnitaRemovibile + "\" + Me.MySubDirBackup) Then
                    Try
                        File.Copy(Me.MyPosizioneBackupDB + "\" + Me.MySubDirBackup + "\" + Me.MyNomeFileBackup, UnitaRemovibile + "\" + Me.MySubDirBackup + "\" + Me.MyNomeFileBackup)
                        Return True
                    Catch ex As Exception
                        'giu280809 MsgBox("Si è verificato il seguente errore durante la copia del file di backup:" & Chr(Keys.Return) & ex.Message)
                        If SWBKAuto = True Then
                            EsitoBKAuto = "ERRORE: Lo spazio disponibile sull'unità " & UnitaRemovibile & " non è sufficiente. Rimuovere l'unità e collegarne una vuota." & ex.Message
                        Else
                            MsgBox("Lo spazio disponibile sull'unità " & UnitaRemovibile & " non è sufficiente. Rimuovere l'unità e collegarne una vuota." & Chr(Keys.Return) & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Backup")
                        End If

                        Return False
                    End Try
                Else
                    Try
                        Directory.CreateDirectory(UnitaRemovibile + "\" + Me.MySubDirBackup)
                        File.Copy(Me.MyPosizioneBackupDB + "\" + Me.MySubDirBackup + "\" + Me.MyNomeFileBackup, UnitaRemovibile + "\" + Me.MySubDirBackup + "\" + Me.MyNomeFileBackup)
                        Return True
                    Catch ex As Exception
                        'giu280809 MsgBox("Si è verificato il seguente errore durante la copia del file di backup:" & Chr(Keys.Return) & ex.Message)
                        If SWBKAuto = True Then
                            EsitoBKAuto = "ERRORE: Lo spazio disponibile sull'unità " & UnitaRemovibile & " non è sufficiente. Rimuovere l'unità e collegarne una vuota." & ex.Message
                        Else
                            MsgBox("Lo spazio disponibile sull'unità " & UnitaRemovibile & " non è sufficiente. Rimuovere l'unità e collegarne una vuota." & Chr(Keys.Return) & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Backup")
                        End If

                        Return False
                    End Try
                End If
            End If
        Else
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE: Supporto removibile non trovato alla locazione " & PercorsoSupportoRemovibile
            Else
                MsgBox("Supporto removibile non trovato alla locazione " & PercorsoSupportoRemovibile, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Backup")
            End If

            Return False
        End If

    End Function


    Private Function funLeggiPercorsoBackup() As Boolean

        Try
            MyPosizioneBackupDB = getConfigurationString("PercorsoFilesBackup")
            If InStr(UCase(MyPosizioneBackupDB), "\\") > 0 Then
                MyPosizioneBackupDB = ""
            End If
            MyPosizioneLOG1 = getConfigurationString("PercorsoFlash_1")
            If InStr(UCase(MyPosizioneLOG1), "\\") > 0 Then
                MyPosizioneLOG1 = ""
            End If
            MyPosizioneLOG2 = getConfigurationString("PercorsoFlash_2")
            If InStr(UCase(MyPosizioneLOG2), "\\") > 0 Then
                MyPosizioneLOG2 = ""
            End If
            MyPosizioneBkLOG1 = getConfigurationString("PosizioneBackupFlash_1")
            If InStr(UCase(MyPosizioneBkLOG1), "\\") > 0 Then
                MyPosizioneBkLOG1 = ""
            End If
            MyPosizioneBkLOG2 = getConfigurationString("PosizioneBackupFlash_2")
            If InStr(UCase(MyPosizioneBkLOG2), "\\") > 0 Then
                MyPosizioneBkLOG2 = ""
            End If
            '----------------------------------------------------------------------------------------
            If MyPosizioneBackupDB.Trim = "" Then
                If SWBKAuto Then
                    EsitoBKAuto = "ERRORE : tabella di configurazione: Posizione di Backup non definite/percorsi di rete non ammessi nel file App.Config"
                Else
                    MsgBox("Si è verificato il seguente errore durante la lettura della configurazione: " & Chr(13) _
                                                    & "Posizione di Backup non definite/percorsi di rete non ammessi nel file App.Config", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, IntestazioneMessaggi)
                End If
                Me.MyPosizioneBackupDB = ""
                MyPosizioneBkLOG1 = ""
                MyPosizioneBkLOG2 = ""
                MyPosizioneLOG1 = ""
                MyPosizioneLOG2 = ""
                Return False
            End If
            If Val(getConfigurationString("NMaxBKTickets")) > 0 Then
                MyNMaxBKTieckets = getConfigurationString("NMaxBKTickets")
            Else
                MyNMaxBKTieckets = 5
            End If
            '---
            If Val(getConfigurationString("NMaxBKRiepiloghiSIAE")) > 0 Then
                MyNMaxBKRiepiloghiSIAE = getConfigurationString("NMaxBKRiepiloghiSIAE")
            Else
                MyNMaxBKRiepiloghiSIAE = 1
            End If
            '---
            If Val(getConfigurationString("NMaxBKViario")) > 0 Then
                MyNMaxBKViario = getConfigurationString("NMaxBKViario")
            Else
                MyNMaxBKViario = 1
            End If
            '---------
            Return True
        Catch ex As Exception
            Me.objDatabase.ControllaConnessione(Me.SqlConnTickets)
            If SWBKAuto Then
                EsitoBKAuto = "ERRORE durante la lettura della configurazione: " & ex.Message
            Else
                MsgBox("Si è verificato il seguente errore durante la lettura della configurazione: " & Chr(13) _
                & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, IntestazioneMessaggi)
            End If

            Me.MyPosizioneBackupDB = ""
            MyPosizioneBkLOG1 = ""
            MyPosizioneBkLOG2 = ""
            MyPosizioneLOG1 = ""
            MyPosizioneLOG2 = ""
            Return False
        End Try
    End Function

    Private Function funEseguiBackup(ByVal TipoDatabase As String, ByVal ConnDatabase As String) As Boolean

        Me.MyDataOraInizio = Now
        '----------------------------------------------------------------------
        ScriviFileLog("Inizio salvataggio Database: ")
        '----------------------------------------------------------------------
        If TipoDatabase = "SoftTickets" Then 'giu270809 IMPORTATNTE SOLO PER DBSoftTicktes
            ScriviFileLog("Database SoftTickets controllo LOG1/LOG2: ")
            'giu250809 giu260809 Dir legato al nomeDB per il BK log
            MyIDNomeFileBackup = TipoDatabase + "_" + Format(Me.MyDataOraInizio, "yyyyMMddHHmm") + "_" + Format(Me.MyProgressivoBackup, "000")
            If (Me.MyPosizioneLOG1.Trim <> "" And Me.MyPosizioneLOG2.Trim <> "") Then
                If (Me.MyPosizioneBkLOG1.Trim <> "" And Me.MyPosizioneBkLOG2.Trim <> "") Then
                    If Not System.IO.Directory.Exists(Me.MyPosizioneBkLOG1 + "\" + Me.MyIDNomeFileBackup) Then
                        System.IO.Directory.CreateDirectory(Me.MyPosizioneBkLOG1 + "\" + Me.MyIDNomeFileBackup)
                    End If
                    If Not System.IO.Directory.Exists(Me.MyPosizioneBkLOG2 + "\" + Me.MyIDNomeFileBackup) Then
                        System.IO.Directory.CreateDirectory(Me.MyPosizioneBkLOG2 + "\" + Me.MyIDNomeFileBackup)
                    End If
                    '---------
                    If funBKLOG12() = False Then 'OK SWBKAuto
                        Return False
                    End If
                Else
                    MyIDNomeFileBackup = ""
                End If
            Else
                MyIDNomeFileBackup = "" 'giu270809 IMPORTATNTE SOLO PER DBSoftTicktes
            End If
        Else
            MyIDNomeFileBackup = ""
        End If
        ScriviFileLog("Controllo dimensione Database e spazio sul disco (" & TipoDatabase & ")")
        If funCheckSpaceDbToDisk(TipoDatabase, ConnDatabase) = False Then 'giu250809 'OK SWBKAuto
            ScriviFileLog("ERRORE Controllo dimensione Database e spazio sul disco (" & TipoDatabase & ")")
            Return False
        End If
        If NomeDB.Trim = "" Then
            NomeDB = TipoDatabase
        End If

        Dim objDatabase As New clsDatabase()

        Me.SqlConnectionDB.ConnectionString = objDatabase.getConnectionString("Master")

        Me.MySubDirBackup = Format(Me.MyDataOraInizio, "yyyy_MM")

        If Not System.IO.Directory.Exists(Me.MyPosizioneBackupDB + "\" + Me.MySubDirBackup) Then
            System.IO.Directory.CreateDirectory(Me.MyPosizioneBackupDB + "\" + Me.MySubDirBackup)
        End If

        Me.MyNomeFileBackup = TipoDatabase + "_" + Format(Me.MyDataOraInizio, "yyyyMMddHHmm") + "_" + Format(Me.MyProgressivoBackup, "000") + ".bak"
        Me.MyPercorsoNomeFileBackup = Me.MyPosizioneBackupDB + "\" + Me.MySubDirBackup + "\" + Me.MyNomeFileBackup

        SqlBackupCommand.CommandType = CommandType.Text
        SqlBackupCommand.CommandTimeout = g_SQLTimeout
        SqlBackupCommand.Connection = Me.SqlConnectionDB
        ScriviFileLog("Salvataggio Database (" & NomeDB & ")")
        Me.SqlBackupCommand.CommandText = "BACKUP DATABASE [" + NomeDB + "] TO DISK='" + Me.MyPercorsoNomeFileBackup + "' WITH INIT, PASSWORD='" + Decripta() + "'"

        Try
            If Me.SqlConnectionDB.State = ConnectionState.Closed Then
                Me.SqlConnectionDB.Open()
            End If
            Me.SqlBackupCommand.ExecuteNonQuery()
            Me.SqlConnectionDB.Close()
            EsitoBKAuto = "Eseguito con successo Backup del database " & TipoDatabase & " nella posizione " & Me.MyPercorsoNomeFileBackup
            ScriviFileLog(EsitoBKAuto)
            '--------------
            Return True
        Catch ex As Exception
            Me.objDatabase.ControllaConnessione(Me.SqlConnectionDB)
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE : backup del database " & TipoDatabase & " nella posizione " & Me.MyPercorsoNomeFileBackup & ":" & ex.Message
            Else
                MsgBox("Si è verificato il seguente errore durante il backup del database " & TipoDatabase & " nella posizione " & Me.MyPercorsoNomeFileBackup & ":" & Chr(Keys.Return) _
                            & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
            End If
            ScriviFileLog(EsitoBKAuto)
            Return False
        End Try
    End Function
    'giu130918
    Public Function GetUltimoEser(ByVal ConnDatabase As String, ByVal myDitta As String, ByRef AnniBK As Integer) As String
        GetUltimoEser = ""
        Dim myCn As SqlConnection
        Dim myCmd As New SqlCommand
        Dim myAdapt As New SqlDataAdapter
        Dim SqlStr As String
        Dim DsGetUltEsercizio As New DataSet
        myCn = New SqlConnection(ConnDatabase)
        myCn.Open()
        SqlStr = "Select * FROM Esercizi WHERE Ditta='" & myDitta.Trim & "' ORDER BY Esercizio DESC"
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
            GetUltimoEser = DsGetUltEsercizio.Tables(0).Rows(0).Item("Esercizio")
            If Not IsNumeric(GetUltimoEser.Trim) Then
                Exit Function
            End If
            Dim PrimoEser As String = Format(CInt(GetUltimoEser.Trim) - AnniBK, "0000")
            'ok ora verifico se ho tutti gli esercizi da salvare
            Dim i As Integer = DsGetUltEsercizio.Tables(0).Select().Length
            If i < AnniBK Then
                AnniBK = i
            End If
        Catch ex As Exception
            GetUltimoEser = ex.Message.Trim
        End Try
        DsGetUltEsercizio = Nothing
    End Function
    'giu040618
    Public Function UpdateDataBk(ByVal TipoDatabase As String, ByVal ConnDatabase As String, ByVal strDittaAnno As String) As Boolean
        Dim objDatabase As New clsDatabase()
        Me.SqlConnectionDB.ConnectionString = ConnDatabase

        SqlBackupCommand.CommandType = CommandType.Text
        SqlBackupCommand.CommandTimeout = g_SQLTimeout
        SqlBackupCommand.Connection = Me.SqlConnectionDB

        SQLstr = "UPDATE Esercizi SET DataUltimoBk = CONVERT(DATETIME,'" & Replace(Format(Now, FormatoDataOra), ".", ":") & "' ,103)"
        SQLstr += " WHERE Ditta = '" & Mid(strDittaAnno, 1, 2) & "' AND Esercizio = '" & Mid(strDittaAnno, 3, 4) & "'"
        Me.SqlBackupCommand.CommandText = SQLstr

        Try
            If Me.SqlConnectionDB.State = ConnectionState.Closed Then
                Me.SqlConnectionDB.Open()
            End If
            Me.SqlBackupCommand.ExecuteNonQuery()
            Me.SqlConnectionDB.Close()
            EsitoBKAuto = "Aggiornamento data Backup del database eseguito con successo.: Install " & strDittaAnno
            Return True
        Catch ex As Exception
            Me.objDatabase.ControllaConnessione(Me.SqlConnectionDB)
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE : Aggiornamento data Backup del database Install: " & strDittaAnno & " : " & ex.Message
            Else
                MsgBox("ERRORE : Aggiornamento data Backup del database Install: " & strDittaAnno & " : " & Chr(Keys.Return) _
                            & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
            End If
            ScriviFileLog("ERRORE : Aggiornamento data Backup del database Install: " & strDittaAnno & " : " & ex.Message)
            Return False
        End Try

    End Function
    'giu120918
    Public Function MySQLTrasfDoc(ByVal ConnDatabase As String) As Boolean
        MySQLTrasfDoc = True
        Dim objDatabase As New clsDatabase()
        Me.SqlConnectionDB.ConnectionString = ConnDatabase

        SqlBackupCommand.CommandType = CommandType.StoredProcedure
        SqlBackupCommand.CommandTimeout = g_SQLTimeout
        SqlBackupCommand.Connection = Me.SqlConnectionDB

        SQLstr = "Build_MySQL_testatadoc_corpo_doc"
        ScriviFileLog("Esecuzione SP: " & SQLstr)
        Me.SqlBackupCommand.CommandText = SQLstr
        Try
            If Me.SqlConnectionDB.State = ConnectionState.Closed Then
                Me.SqlConnectionDB.Open()
            End If
            Me.SqlBackupCommand.ExecuteNonQuery()
            If Me.SqlConnectionDB.State = ConnectionState.Open Then
                Me.SqlConnectionDB.Close()
            End If
            Return True
        Catch SQLEx As SqlException
            ScriviFileLog("ERRORE trasferimento documenti per CRM Appuntamenti: " & SQLEx.Message)
            If SWBKAuto = False Then
                MsgBox("ERRORE trasferimento documenti per CRM Appuntamenti: " & Chr(Keys.Return) _
                            & SQLEx.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
            End If
            Return False
        Catch Ex As Exception
            ScriviFileLog("ERRORE trasferimento documenti per CRM Appuntamenti: " & Ex.Message)
            If SWBKAuto = False Then
                MsgBox("ERRORE trasferimento documenti per CRM Appuntamenti: " & Chr(Keys.Return) _
                            & Ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
            End If
            Return False
        Finally
            If Me.SqlConnectionDB.State = ConnectionState.Open Then
                Me.SqlConnectionDB.Close()
            End If
        End Try

    End Function

    'GIU250809
    Private Function funCheckSpaceDbToDisk(ByVal TipoDatabase As String, ByVal ConnDatabase As String) As Boolean

        Dim SqlConnDB As New SqlConnection()
        Dim SqlCmdCheckDB As New SqlCommand()
        Dim myReader As SqlDataReader
        Dim dbSizeStr As String
        Dim dbSize As Long = 0
        Dim dbNonAll As Long = 0

        Dim frmMess1 As New frmMasterizza()
        frmMess1.Text = "Backup database " & TipoDatabase
        frmMess1.Show()
        frmMess1._Label = "Verifica dimensione del database da salvare, attendere ..."
        frmMess1.Refresh()
        '
        Dim objDatabase As New clsDatabase()
        SqlConnDB.ConnectionString = IIf(ConnDatabase.Trim = "", objDatabase.getConnectionString(TipoDatabase), ConnDatabase.Trim)
        '-------------------------------------- dimensione del DB da salvare
        SqlCmdCheckDB.CommandType = CommandType.Text
        SqlCmdCheckDB.CommandTimeout = g_SQLTimeout
        SqlCmdCheckDB.Connection = SqlConnDB
        SqlCmdCheckDB.CommandText = "EXEC sp_spaceused"
        Try
            SqlConnDB.Open()
            myReader = SqlCmdCheckDB.ExecuteReader(CommandBehavior.CloseConnection)
            myReader.Read()
            NomeDB = myReader.Item("database_name")
            If NomeDB = "" Then
                NomeDB = TipoDatabase
            End If

            dbSizeStr = myReader.Item("database_size")
            dbSizeStr = dbSizeStr.Replace(".", ",")

            frmMess1._Label = "Dimensione del database da salvare " + dbSizeStr + " , attendere ..."
            frmMess1.Refresh()

            If dbSizeStr.IndexOf("MB") > 0 Then
                dbSize = CLng(Microsoft.VisualBasic.Left(dbSizeStr, dbSizeStr.IndexOf("MB")))
            ElseIf dbSizeStr.IndexOf(",") > 0 Then
                dbSize = CLng(Microsoft.VisualBasic.Left(dbSizeStr, dbSizeStr.IndexOf(",")))
            Else
                dbSize = CLng(dbSizeStr)
            End If
            '------
            dbSizeStr = myReader.Item("unallocated space")
            dbSizeStr = dbSizeStr.Replace(".", ",")
            If dbSizeStr.IndexOf("MB") > 0 Then
                dbNonAll = CLng(Microsoft.VisualBasic.Left(dbSizeStr, dbSizeStr.IndexOf("MB")))
            ElseIf dbSizeStr.IndexOf(",") > 0 Then
                dbNonAll = CLng(Microsoft.VisualBasic.Left(dbSizeStr, dbSizeStr.IndexOf(",")))
            Else
                dbNonAll = CLng(dbSizeStr)
            End If
            '------
            dbSize = dbSize + dbNonAll
            frmMess1._Label = "Dimensione del database da salvare " + Format(dbSize, "###,###,##0.00") + " , attendere ..."
            frmMess1.Refresh()

            myReader.Close()
            If SqlConnDB.State = ConnectionState.Open Then
                SqlConnDB.Close()
            End If
            '
        Catch ex As Exception
            Me.objDatabase.ControllaConnessione(Me.SqlConnectionDB)
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE = durante la verifica della dimensione del database " & ex.Message
            Else
                MsgBox("Si è verificato il seguente errore durante la verifica della dimensione del database " & TipoDatabase & " :" & Chr(Keys.Return) _
                            & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                '---
            End If

            frmMess1.Close()
            frmMess1.Dispose()
            frmMess1 = Nothing
            '---
            Return False
        End Try

        '-----------
        Try
            '
            Dim UnitaBK As String = Me.MyPosizioneBackupDB.Substring(0, 2)
            Dim MBDisPrima As Integer
            MBDisPrima = GetFreeSpace(UnitaBK)

            If funFreeSpaceToDisk(TipoDatabase, dbSize) = False Then
                frmMess1._Label = "Spazio del disco " + UnitaBK.ToString + "\ " + Format(MBDisPrima, "###,###,##0.00") + " MB , attendere ..."
                frmMess1.Refresh()
                '-
                frmMess1.Close()
                frmMess1.Dispose()
                frmMess1 = Nothing
                Return False
            Else
                Dim MBDisDopo As Integer
                MBDisDopo = GetFreeSpace(UnitaBK)

                frmMess1._Label = "Spazio del disco " + UnitaBK.ToString + "\ " + Format(MBDisDopo, "###,###,##0.00") + " MB dopo il salvataggio, attendere ..."
                frmMess1.Refresh()
                '-
                frmMess1.Close()
                frmMess1.Dispose()
                frmMess1 = Nothing
                Return True 'sono riuscito a liberare abbastanza spazio
            End If

        Catch ex As Exception
            Me.objDatabase.ControllaConnessione(Me.SqlConnectionDB)
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE : durante la verifica della dimensione del database " & ex.Message
            Else
                MsgBox("Si è verificato il seguente errore durante la verifica della dimensione del database " & TipoDatabase & " :" & Chr(Keys.Return) _
                                & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
            End If
            '---
            frmMess1.Close()
            frmMess1.Dispose()
            frmMess1 = Nothing
            '---
            Return False
        End Try
        '-----------
        objDatabase = Nothing
        frmMess1.Close()
        frmMess1.Dispose()
        frmMess1 = Nothing
        Return True

    End Function
    'GIU250809 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    Private Function funFreeSpaceToDisk(ByVal TipoDatabase As String, ByVal dbSize As Long) As Boolean
        Dim frmMess2 As New frmMasterizza()
        frmMess2.Text = "Backup database " & TipoDatabase
        frmMess2.Show() 'ErrGiu270809 
        'DETERMINO IL FILTRO PER DATABASE DA CANCELLARE
        'E IL NUMERO DI DB DA TENERE COME SALVATAGGIO 
        Dim SWFiltro As String = ""     'Filtro del DB che sto salvando
        Dim NDBSaveStor As Integer = 0  'N° DB da tenere salvati come storico BK
        Dim NDBSave As Integer = 0      'N° TOTALE di DB Salvati sul disco
        SWFiltro = Mid(TipoDatabase, 1, 7)
        SWFiltro = SWFiltro.Trim & "*.*"

        If NDBSaveStor = 0 Then
            NDBSaveStor = 6
        End If
        '----------------------------------------------
        Me.MyDataOraInizio = Now
        'TROVO IL PRIMO ANNO_MM
        Dim Anno As Integer = Year(Me.MyDataOraInizio)
        Dim Mese As Integer = Month(Me.MyDataOraInizio)
        'MEMO Anno Mese CORRENTE OLTRE AL QUALE NON DEVE ANDARE
        Dim AnnoCorr As Integer = Year(Me.MyDataOraInizio)
        Dim MeseCorr As Integer = Month(Me.MyDataOraInizio)
        '------------------------------------------------------
        Me.MySubDirBackup = Format(Me.MyDataOraInizio, "yyyy_MM")
        Dim TrovaDir As String = ""
        'TORNO INDIETRO DI 1 SE NON ESISTE VUOL DIRE CHE è IL PRIMO BK 
        'SE NON CI FOSSE ALLORA IL DISCO è FULL QUINDI STOP 
        Dim SWOk As Boolean = False : NDBSave = 0
        'CONTEGGIO MESE CORRENTE NBSave
        TrovaDir = Format(Anno, "0000") & "_" & Format(Mese, "00")
        Call funCheckDirFull(Me.MyPosizioneBackupDB + "\" + TrovaDir, SWFiltro, NDBSave) 'OK SWBKAUTO
        NDBSaveOK = NDBSave

        'ErrGiu270809 frmMess2.Show()
        frmMess2._Label = "N° database salvati nel periodo : " & TrovaDir & " " & Format(NDBSaveOK, "###,##0") & " , attendere ..."
        frmMess2.Refresh()
        '------------------------------
        Dim UnitaBK As String = Me.MyPosizioneBackupDB.Substring(0, 2)
        Dim MBDisponibili As Integer
        '------------------------------
        'CONTROLLO SOLO PRIMA VOLTA INDIETRO
        '------------------------------
        If Mese > 1 Then
            Mese = Mese - 1
        Else
            Anno = Anno - 1
            Mese = 12
        End If
        TrovaDir = Format(Anno, "0000") & "_" & Format(Mese, "00")
        If Not Directory.Exists(Me.MyPosizioneBackupDB + "\" + TrovaDir) Then
            If NDBSave > 0 Then
                SWOk = False
            Else
                SWOk = True
                'CONTROLLO PER SICUREZZA
                MBDisponibili = GetFreeSpace(UnitaBK)
                If dbSize > MBDisponibili Then
                    If SWBKAuto = True Then
                        EsitoBKAuto = "ERRORE : Lo spazio disponibile sull'unità " & UnitaBK & " non è sufficiente. Impossibile effettuare il Backup."
                    Else
                        MsgBox("Lo spazio disponibile sull'unità " & UnitaBK & " non è sufficiente. Impossibile effettuare il Backup.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Backup")
                    End If
                    '---
                    frmMess2.Close()
                    frmMess2.Dispose()
                    frmMess2 = Nothing
                    Return False
                Else
                    '---
                    frmMess2.Close()
                    frmMess2.Dispose()
                    frmMess2 = Nothing
                    Return True
                End If
            End If

        End If
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Anno = Year(Me.MyDataOraInizio)
        Mese = Month(Me.MyDataOraInizio)
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Do
            'INDIETRO
            If Mese > 1 Then
                Mese = Mese - 1
            Else
                Anno = Anno - 1
                Mese = 12
            End If
            TrovaDir = Format(Anno, "0000") & "_" & Format(Mese, "00")
            If Not Directory.Exists(Me.MyPosizioneBackupDB + "\" + TrovaDir) Then
                'AVANTI SUL PRIMO DIR PIU VECCHIO
                If Mese > 11 Then
                    Anno = Anno + 1
                    Mese = 1
                Else
                    Mese = Mese + 1
                End If
                TrovaDir = Format(Anno, "0000") & "_" & Format(Mese, "00")
                If funCheckDirFull(Me.MyPosizioneBackupDB + "\" + TrovaDir, SWFiltro, 0) Then
                    'è piena ok per cancellare
                    SWOk = True
                Else
                    'è vuota vado AVANTI
                    Do
                        If Mese > 11 Then
                            Anno = Anno + 1
                            Mese = 1
                        Else
                            Mese = Mese + 1
                        End If
                        TrovaDir = Format(Anno, "0000") & "_" & Format(Mese, "00")
                        If funCheckDirFull(Me.MyPosizioneBackupDB + "\" + TrovaDir, SWFiltro, 0) Then 'è piena ok per cancellare
                            SWOk = True
                        ElseIf AnnoCorr = Anno And MeseCorr = Mese Then
                            SWOk = True
                        End If
                    Loop Until SWOk = True
                End If
            Else
                'CONTEGGIO NBSave
                Call funCheckDirFull(Me.MyPosizioneBackupDB + "\" + TrovaDir, SWFiltro, NDBSave)
                'ErrGiu270809 frmMess2.Show()
                frmMess2._Label = "N° database salvati " & Format(NDBSave, "###,##0") & " , attendere ..."
                frmMess2.Refresh()
            End If
        Loop Until SWOk = True
        '---------------------------------------------------------------------------------
        'OK TROVATA
        NDBSaveOK = NDBSave
        'ErrGiu270809 frmMess2.Show()
        frmMess2._Label = "N° database salvati " & Format(NDBSaveOK, "###,##0") & " , attendere ..."
        frmMess2.Refresh()
        'ErrGiu270809 qui va in loop 
        Do Until NDBSave < NDBSaveStor 'CONTINUO FINO A CHE NDBSave < NDSaveStor
            TrovaDir = Format(Anno, "0000") & "_" & Format(Mese, "00")
            If funCheckDirFull(Me.MyPosizioneBackupDB + "\" + TrovaDir, SWFiltro, 0) Then 'è piena ok per cancellare
                SWOk = funDeleteFileDir(Me.MyPosizioneBackupDB + "\" + TrovaDir, SWFiltro, NDBSaveStor, NDBSave)
            End If
            If Mese > 11 Then
                Anno = Anno + 1
                Mese = 1
            Else
                Mese = Mese + 1
            End If
            'ErrGiu270809 qui va in loop se non esiste quindi esco - sopra va bene perche c'è test sul annocorr
            TrovaDir = Format(Anno, "0000") & "_" & Format(Mese, "00")
            If Not Directory.Exists(TrovaDir) Then
                'esco
                Exit Do
            End If
            '---------------------------
        Loop
        '
        '---------------------------------------------------------------------------------
        'RICONTROLLO PER SICUREZZA
        MBDisponibili = GetFreeSpace(UnitaBK)
        If dbSize > MBDisponibili Then
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE : Lo spazio disponibile sull'unità " & UnitaBK & " non è sufficiente. Impossibile effettuare il Backup."
            Else
                MsgBox("Lo spazio disponibile sull'unità " & UnitaBK & " non è sufficiente. Impossibile effettuare il Backup.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Backup")
                '---
            End If

            frmMess2.Close()
            frmMess2.Dispose()
            frmMess2 = Nothing
            Return False
        Else
            '---
            frmMess2.Close()
            frmMess2.Dispose()
            frmMess2 = Nothing
            Return True
        End If
        '---
        frmMess2.Close()
        frmMess2.Dispose()
        frmMess2 = Nothing
    End Function

    Private Function funCheckDirFull(ByVal NomeDir As String, ByVal SWFiltro As String, ByRef NDBSave As Integer) As Boolean

        Dim FilesDaCancellare() As String

        Try

            If Directory.Exists(NomeDir) Then
                '
                FilesDaCancellare = Directory.GetFiles(NomeDir, SWFiltro)
                If FilesDaCancellare.Length > 0 Then
                    NDBSave += FilesDaCancellare.Length
                    FilesDaCancellare = Nothing
                    Return True
                Else
                    FilesDaCancellare = Nothing
                    Return False
                End If

            Else
                FilesDaCancellare = Nothing
                Return False
            End If

            FilesDaCancellare = Nothing
        Catch ex As Exception
            FilesDaCancellare = Nothing
            Return False
        End Try

        FilesDaCancellare = Nothing
    End Function

    Private Function funDeleteFileDir(ByVal NomeDir As String, ByVal SWFiltro As String, ByVal NDBSaveStor As Integer, ByRef NDBSave As Integer) As Boolean
        If NDBSaveStor = 0 Or Not IsNumeric(NDBSaveStor) Or IsNothing(NDBSaveStor) Then
            NDBSaveStor = 10
        End If

        Dim FilesDaCancellare() As String
        Dim Ind As Integer
        Try

            If Directory.Exists(NomeDir) Then

                FilesDaCancellare = Directory.GetFiles(NomeDir, SWFiltro)
                If FilesDaCancellare.Length > NDBSaveStor Then
                    For Ind = 0 To FilesDaCancellare.Length - NDBSaveStor
                        File.Delete(FilesDaCancellare(Ind))
                        funDeleteDirLOG(FilesDaCancellare(Ind)) 'giu260809 non mi interessa se andasse in errore
                        NDBSave -= 1 'sottraggo quelli CANCELLATI, se poi è il caso VADO AVANTI E CONTINUO
                    Next
                ElseIf FilesDaCancellare.Length > 0 Then
                    'CANCELLO SOLO 1 IL PIU VECCHIO
                    File.Delete(FilesDaCancellare(Ind))
                    funDeleteDirLOG(FilesDaCancellare(Ind)) 'giu260809 non mi interessa se andasse in errore
                    NDBSave -= 1 'sottraggo quelli CANCELLATI, se poi è il caso VADO AVANTI E CONTINUO
                End If
                '-----------------------------------------------
                'SE è COMPLETAMENTE VUOTA OK CANCELLA ANCHE LA DIR
                'non la CANCELLO MAI PERCHE' SE NON TESTO CHE PRIMA CI SIA UN'ALTRA
                'CREO UN BUCO NEI ANNO_MESE
                'FilesDaCancellare = Directory.GetFiles(NomeDir)
                'If FilesDaCancellare.Length = 0 Then
                '    ''''ATTENZIONE QUI CANCELLO IL DIR MA è vuoto
                '    Dim dirF1 As New DirectoryInfo(NomeDir)
                '    Console.WriteLine(Format(Now, "HH.mm.ss.fff"))
                '    dirF1.Delete(True)
                '    Console.WriteLine(Format(Now, "HH.mm.ss.fff"))
                'End If
                '-----------------------------------------------
            Else
                FilesDaCancellare = Nothing
                Return False
            End If

            FilesDaCancellare = Nothing

        Catch ex As Exception
            FilesDaCancellare = Nothing
            Return False
        End Try

        FilesDaCancellare = Nothing
    End Function
    'giu260809
    Private Function funDeleteDirLOG(ByVal IDNomeFileBKLog As String) As Boolean
        'giu270809 IMPORTANTE SOLO X : TipoDatabase <> "SoftTickets" Then
        If MyIDNomeFileBackup.Trim = "" Then 'giu270809 SOLO X : TipoDatabase <> "SoftTickets" Then
            Return True
        End If
        'giu270809 SOLO X : TipoDatabase <> "SoftTickets" Then
        Dim FilesDaCancellare() As String
        Dim Ind As Integer
        Try
            Dim k As String = IDNomeFileBKLog.Substring(0, IDNomeFileBKLog.IndexOf("."))
            Dim i As Integer = 0
            Do
                i = k.IndexOf("\")
                If i > 0 Then
                    i += 2
                    k = Mid(k, i)
                End If
            Loop Until i < 1
            'Torvato il nome del DB che ho cancellato adesso cancello i LOG1 e 2 
            If Directory.Exists(MyPosizioneBkLOG1 + "\" + k) Then
                FilesDaCancellare = Directory.GetFiles(MyPosizioneBkLOG1 + "\" + k)
                If FilesDaCancellare.Length > 0 Then
                    For Ind = 0 To FilesDaCancellare.Length - 1
                        File.Delete(FilesDaCancellare(Ind))
                    Next
                End If
                Dim dirF1 As New DirectoryInfo(MyPosizioneBkLOG1 + "\" + k)
                dirF1.Delete(True)
            End If
            FilesDaCancellare = Nothing
            '---
            If Directory.Exists(MyPosizioneBkLOG2 + "\" + k) Then
                FilesDaCancellare = Directory.GetFiles(MyPosizioneBkLOG2 + "\" + k)
                If FilesDaCancellare.Length > 0 Then
                    For Ind = 0 To FilesDaCancellare.Length - 1
                        File.Delete(FilesDaCancellare(Ind))
                    Next
                End If
                Dim dirF2 As New DirectoryInfo(MyPosizioneBkLOG2 + "\" + k)
                dirF2.Delete(True)
            End If
            FilesDaCancellare = Nothing
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
    Private Function funBKLOG12() As Boolean 'giu270809
        'giu270809 IMPORTANTE SOLO X : TipoDatabase <> "SoftTickets" Then
        If MyIDNomeFileBackup.Trim = "" Then
            Return True
        End If

        Dim FilesDaCopiare() As String
        Dim Ind As Integer
        Dim k As String = ""
        Dim i As Integer = 0

        Try
            '--- LOG1
            Do
                If Directory.Exists(MyPosizioneLOG1) Then

                    FilesDaCopiare = Directory.GetFiles(MyPosizioneLOG1)
                    If FilesDaCopiare.Length > 0 Then
                        For Ind = 0 To FilesDaCopiare.Length - 1
                            k = FilesDaCopiare(Ind)
                            Do
                                i = k.IndexOf("\")
                                If i > 0 Then
                                    i += 2
                                    k = Mid(k, i)
                                End If
                            Loop Until i < 1
                            File.Copy(FilesDaCopiare(Ind), MyPosizioneBkLOG1 + "\" + MyIDNomeFileBackup + "\" + k)
                        Next
                    End If
                    'MsgBox("Memoria flash 1 salvata.", MsgBoxStyle.OKOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                    Exit Do
                Else
                    If SWBKAuto = True Then
                        EsitoBKAuto = "ERRORE :La memoria flash 1 non è pronta per il salvataggio."
                        Return False
                        Exit Do
                    Else
                        If MsgBox("La memoria flash 1 non è pronta per il salvataggio. Inserire la cartuccia e riprovare.", MsgBoxStyle.RetryCancel Or MsgBoxStyle.Information, Me.IntestazioneMessaggi) = MsgBoxResult.Cancel Then
                            MsgBox("La memoria flash 1 non è stata salvata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                            Return False
                            Exit Do
                        End If
                    End If

                End If
            Loop
            FilesDaCopiare = Nothing
            '--- LOG2
            Do
                If Directory.Exists(MyPosizioneLOG2) Then

                    FilesDaCopiare = Directory.GetFiles(MyPosizioneLOG2)
                    If FilesDaCopiare.Length > 0 Then
                        For Ind = 0 To FilesDaCopiare.Length - 1
                            k = FilesDaCopiare(Ind)
                            Do
                                i = k.IndexOf("\")
                                If i > 0 Then
                                    i += 2
                                    k = Mid(k, i)
                                End If
                            Loop Until i < 1
                            File.Copy(FilesDaCopiare(Ind), MyPosizioneBkLOG2 + "\" + MyIDNomeFileBackup + "\" + k)
                        Next
                    End If
                    'MsgBox("Memoria flash 2 salvata.", MsgBoxStyle.OKOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                    Exit Do
                Else
                    If SWBKAuto = True Then
                        EsitoBKAuto = "ERRORE :La memoria flash 2 non è pronta per il salvataggio."
                        Return False
                        Exit Do
                    Else
                        If MsgBox("La memoria flash 2 non è pronta per il salvataggio. Inserire la cartuccia e riprovare.", MsgBoxStyle.RetryCancel Or MsgBoxStyle.Information, Me.IntestazioneMessaggi) = MsgBoxResult.Cancel Then
                            MsgBox("La memoria flash 2 non è stata salvata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                            Return False
                            Exit Do
                        End If
                    End If

                End If
            Loop

        Catch ex As Exception
            If SWBKAuto = True Then
                EsitoBKAuto = "Si è verificato il seguente errore durante il salvataggio delle memorie flash: " & ex.Message
            Else
                MsgBox("Si è verificato il seguente errore durante il salvataggio delle memorie flash: " & Chr(Keys.Return) _
                            & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
            End If

            Return False
        End Try

        FilesDaCopiare = Nothing
        Return True
    End Function
    'giu280809 SAVE su REMOVIBILE
    Private Function funBKLOG12UnitaExt(ByVal strF1 As String, ByVal strF2 As String) As Boolean
        'giu270809 IMPORTANTE SOLO X : TipoDatabase <> "SoftTickets" Then
        If MyIDNomeFileBackup.Trim = "" Then
            Return True
        End If

        Dim FilesDaCopiare() As String
        Dim Ind As Integer
        Dim k As String = ""
        Dim i As Integer = 0

        Try
            '--- LOG1
            Do
                If Directory.Exists(MyPosizioneLOG1) Then

                    FilesDaCopiare = Directory.GetFiles(MyPosizioneLOG1)
                    If FilesDaCopiare.Length > 0 Then
                        For Ind = 0 To FilesDaCopiare.Length - 1
                            k = FilesDaCopiare(Ind)
                            Do
                                i = k.IndexOf("\")
                                If i > 0 Then
                                    i += 2
                                    k = Mid(k, i)
                                End If
                            Loop Until i < 1
                            File.Copy(FilesDaCopiare(Ind), strF1 + "\" + k)
                        Next
                    End If
                    'MsgBox("Memoria flash 1 salvata.", MsgBoxStyle.OKOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                    Exit Do
                Else
                    If SWBKAuto = True Then
                        EsitoBKAuto = "ERRORE : La memoria flash 1 non è pronta per il salvataggio. Inserire la cartuccia e riprovare."
                        Return False
                        Exit Do
                    Else
                        If MsgBox("La memoria flash 1 non è pronta per il salvataggio. Inserire la cartuccia e riprovare.", MsgBoxStyle.RetryCancel Or MsgBoxStyle.Information, Me.IntestazioneMessaggi) = MsgBoxResult.Cancel Then
                            MsgBox("La memoria flash 1 non è stata salvata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                            Return False
                            Exit Do
                        End If
                    End If

                End If
            Loop
            FilesDaCopiare = Nothing
            '--- LOG2
            Do
                If Directory.Exists(MyPosizioneLOG2) Then

                    FilesDaCopiare = Directory.GetFiles(MyPosizioneLOG2)
                    If FilesDaCopiare.Length > 0 Then
                        For Ind = 0 To FilesDaCopiare.Length - 1
                            k = FilesDaCopiare(Ind)
                            Do
                                i = k.IndexOf("\")
                                If i > 0 Then
                                    i += 2
                                    k = Mid(k, i)
                                End If
                            Loop Until i < 1
                            File.Copy(FilesDaCopiare(Ind), strF2 + "\" + k)
                        Next
                    End If
                    'MsgBox("Memoria flash 2 salvata.", MsgBoxStyle.OKOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                    Exit Do
                Else
                    If SWBKAuto = True Then
                        EsitoBKAuto = "ERRORE : La memoria flash 2 non è pronta per il salvataggio. Inserire la cartuccia e riprovare."
                        Return False
                        Exit Do
                    Else
                        If MsgBox("La memoria flash 2 non è pronta per il salvataggio. Inserire la cartuccia e riprovare.", MsgBoxStyle.RetryCancel Or MsgBoxStyle.Information, Me.IntestazioneMessaggi) = MsgBoxResult.Cancel Then
                            MsgBox("La memoria flash 2 non è stata salvata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                            Return False
                            Exit Do
                        End If
                    End If

                End If
            Loop

        Catch ex As Exception
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE: Si è verificato il seguente errore durante il salvataggio delle memorie flash: " & ex.Message
            Else
                MsgBox("Si è verificato il seguente errore durante il salvataggio delle memorie flash: " & Chr(Keys.Return) _
                            & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
            End If

            Return False
        End Try

        FilesDaCopiare = Nothing
        Return True
    End Function
    '---------@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    Private Sub subCancellaFlash()
        If MyPosizioneLOG1.Trim = "" Then Exit Sub
        ScriviFileLog("subCancellaFlash: " & MyPosizioneLOG1.Trim & " - " & MyPosizioneLOG2.Trim)
        '---------
        Dim FilesDaCancellare() As String
        Dim Ind As Integer

        Try
            Do
                If Directory.Exists(MyPosizioneLOG1) Then
                    ' 07/03/2005
                    Dim dirF1 As New DirectoryInfo(MyPosizioneLOG1)
                    Console.WriteLine(Format(Now, "HH.mm.ss.fff"))

                    dirF1.Delete(True)
                    dirF1.Create()
                    Console.WriteLine(Format(Now, "HH.mm.ss.fff"))
                    ' -
                    FilesDaCancellare = Directory.GetFiles(MyPosizioneLOG1)
                    If FilesDaCancellare.Length > 0 Then
                        For Ind = 0 To FilesDaCancellare.Length - 1
                            File.Delete(FilesDaCancellare(Ind))
                        Next
                    End If
                    If SWBKAuto = True Then
                        'OK VA BENE 
                    Else
                        MsgBox("Memoria flash 1 cancellata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                    End If

                    Exit Do
                Else
                    If SWBKAuto = True Then
                        EsitoBKAuto = "ERRORE: La memoria flash 1 non è pronta per la cancellazione. Inserire la cartuccia e riprovare."
                        Exit Do
                    Else
                        If MsgBox("La memoria flash 1 non è pronta per la cancellazione. Inserire la cartuccia e riprovare.", MsgBoxStyle.RetryCancel Or MsgBoxStyle.Information, Me.IntestazioneMessaggi) = MsgBoxResult.Cancel Then
                            MsgBox("La memoria flash 1 non è stata cancellata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                            Exit Do
                        End If
                    End If

                End If
            Loop
            FilesDaCancellare = Nothing
            Do
                If Directory.Exists(MyPosizioneLOG2) Then
                    ' 07/03/2005
                    Dim dirF2 As New DirectoryInfo(MyPosizioneLOG2)
                    dirF2.Delete(True)
                    dirF2.Create()
                    ' -
                    FilesDaCancellare = Directory.GetFiles(MyPosizioneLOG2)
                    If FilesDaCancellare.Length > 0 Then
                        For Ind = 0 To FilesDaCancellare.Length - 1
                            File.Delete(FilesDaCancellare(Ind))
                        Next
                    End If
                    If SWBKAuto = True Then
                        'OK VA BENE 
                    Else
                        MsgBox("Memoria flash 2 cancellata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                    End If

                    Exit Do
                Else
                    If SWBKAuto = True Then
                        EsitoBKAuto = "ERRRORE: La memoria flash 2 non è pronta per la cancellazione. Inserire la cartuccia e riprovare."
                        Exit Do
                    Else
                        If MsgBox("La memoria flash 2 non è pronta per la cancellazione. Inserire la cartuccia e riprovare.", MsgBoxStyle.RetryCancel Or MsgBoxStyle.Information, Me.IntestazioneMessaggi) = MsgBoxResult.Cancel Then
                            MsgBox("La memoria flash 2 non è stata cancellata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
                            Exit Do
                        End If
                    End If

                End If
            Loop

            If Not Directory.Exists(MyPosizioneLOG1) Then
                Dim dirF1 As New DirectoryInfo(MyPosizioneLOG1)
                dirF1.Create()
            End If
            If Not Directory.Exists(MyPosizioneLOG2) Then
                Dim dirF2 As New DirectoryInfo(MyPosizioneLOG2)
                dirF2.Create()
            End If
        Catch ex As Exception
            If SWBKAuto = True Then
                EsitoBKAuto = "ERRORE: Si è verificato il seguente errore durante la cancellazione delle memorie flash: " & ex.Message
            Else
                MsgBox("Si è verificato il seguente errore durante la cancellazione delle memorie flash: " & Chr(Keys.Return) _
                            & ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.IntestazioneMessaggi)
            End If

        End Try

        FilesDaCancellare = Nothing
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
