Imports System.Data.SqlClient
Imports System.Configuration
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Integration.Dao.DataSource
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App

Public Class DataBaseUtility

    Public Const QUERY_TYPE_INSERT As Int32 = 1
    Public Const QUERY_TYPE_UPDATE As Int32 = 2
    Public Const QUERY_TYPE_DELETE As Int32 = 3

    Public Const VARCHAR_MAX As Int32 = -1

    Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO)) 'giu110124 Session(ESERCIZIO))
    Dim connectionString As Object = ""

    Public Function PopulateDatasetFromQuery(ByVal TipoDB As Integer, ByVal sQuery As String, ByRef ds As DataSet, Optional ByVal sTable As String = "", Optional ByVal Anno As String = "") As Boolean
        'giu190617
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim myTimeOUT As Long = 10000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
        '---------------------------
        'giu140911 OK FUNZIONA 
        connectionString = dbCon.getConnectionString(TipoDB, Anno)

        Dim conn As New SqlConnection(connectionString.ToString())
        Dim da As New SqlDataAdapter(sQuery, conn)
        da.SelectCommand.CommandTimeout = myTimeOUT '5000
        Try
            If sTable.Trim = "" Then
                da.Fill(ds)
            Else
                da.Fill(ds.Tables(sTable))
            End If
            da.Dispose()
        Catch SQLEx As SqlException
            Throw SQLEx
            Exit Function
        Catch Ex As Exception
            Throw Ex
            Exit Function
        Finally
            If Not IsNothing(conn) Then 'giu271119
                If conn.State <> ConnectionState.Closed Then
                    conn.Close()
                    conn = Nothing
                End If
            End If
        End Try

        PopulateDatasetFromQuery = True
    End Function

    Public Function ExecuteQueryUpdate(ByVal TipoDB As Integer, ByVal sQuery As String, Optional ByVal CMDType As Integer = TipoCMD.Text, Optional ByVal Anno As String = "") As Boolean
        'giu010514 aggiunto paramentro Anno
        'CMDType
        'Aumentato il TimeOut a 10000
        'giu190617
        Dim strValore As String = ""
        Dim strErrore As String = ""
        Dim myTimeOUT As Long = 10000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
        '---------------------------
        Dim command As SqlCommand
        Try
            connectionString = dbCon.getConnectionString(TipoDB, Anno)
            Dim conn As New SqlConnection(connectionString.ToString())
            command = New SqlCommand(sQuery, conn)
            If CMDType = TipoCMD.Text Then
                command.CommandType = CommandType.Text
            ElseIf CMDType = TipoCMD.StoreProcedure Then
                command.CommandType = CommandType.StoredProcedure
            ElseIf CMDType = TipoCMD.TableDirect Then
                command.CommandType = CommandType.TableDirect
            Else
                command.CommandType = CommandType.Text
            End If
            command.Connection.Open()
            command.CommandTimeout = myTimeOUT '10000 '5000
            command.ExecuteNonQuery()
        Catch SQLEx As SqlException
            Throw SQLEx
            Exit Function
        Catch Ex As Exception
            Throw Ex
            Exit Function
        Finally
            If Not IsNothing(command.Connection) Then 'giu271119
                If command.Connection.State <> ConnectionState.Closed Then
                    command.Connection.Close()
                    command.Connection = Nothing
                End If
            End If
        End Try
        ExecuteQueryUpdate = True
    End Function
    'GIU180912 giu091012
    Public Shared Function MySQLClienti(ByVal _Codice_CoGe As String, ByVal _Status As String, ByRef strErrore As String, Optional ByVal SWProvv As Boolean = False) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnCG = New System.Data.SqlClient.SqlConnection
        '-
        'Dim SqlSel_MySQLCli = New System.Data.SqlClient.SqlCommand
        'Dim SqlIns_MySQLCli = New System.Data.SqlClient.SqlCommand
        Dim SqlUpd_MySQLCli = New System.Data.SqlClient.SqlCommand
        'Dim SqlDel_MySQLCli = New System.Data.SqlClient.SqlCommand
        '
        'SqlConnection1
        '
        SqlConnCG.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        'giu190617
        Dim strValore As String = ""
        ' ''Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
        '---------------------------
        Try
            If SqlConnCG.State <> ConnectionState.Open Then
                SqlConnCG.Open()
            End If
            '--- Parametri
            SqlUpd_MySQLCli.CommandText = "[update_MySQLClienti]"
            SqlUpd_MySQLCli.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_MySQLCli.Connection = SqlConnCG
            SqlUpd_MySQLCli.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_MySQLCli.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Codice_CoGe", System.Data.SqlDbType.NVarChar, 20, "Codice_CoGe"))
            SqlUpd_MySQLCli.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Status", System.Data.SqlDbType.VarChar, 30, "Status"))
            SqlUpd_MySQLCli.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWProvv", System.Data.SqlDbType.VarChar, 1, "SWProvv"))
            '
            SqlUpd_MySQLCli.Parameters("@Codice_CoGe").Value = _Codice_CoGe
            SqlUpd_MySQLCli.Parameters("@Status").Value = _Status
            If SWProvv = False Then
                SqlUpd_MySQLCli.Parameters("@SWProvv").Value = "0"
            Else
                SqlUpd_MySQLCli.Parameters("@SWProvv").Value = "1"
            End If
            SqlUpd_MySQLCli.CommandTimeout = myTimeOUT '5000
            SqlUpd_MySQLCli.ExecuteNonQuery()

            If SqlConnCG.State <> ConnectionState.Closed Then
                SqlConnCG.Close()
            End If

            Return True
        Catch exSQL As SqlException
            strErrore = exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        Finally
            If Not IsNothing(SqlConnCG) Then 'giu271119
                If SqlConnCG.State <> ConnectionState.Closed Then
                    SqlConnCG.Close()
                    SqlConnCG = Nothing
                End If
            End If
        End Try
    End Function

    Public Shared Function InitMySQLClienti(ByRef strErrore As String) As Boolean
        strErrore = ""
        'giu190617
        Dim strValore As String = ""
        ' ''Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
        '---------------------------
        InitMySQLClienti = True
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnCG = New System.Data.SqlClient.SqlConnection
        '-
        Dim SqlUpd_MySQLCli = New System.Data.SqlClient.SqlCommand
        SqlConnCG.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        Try
            If SqlConnCG.State <> ConnectionState.Open Then
                SqlConnCG.Open()
            End If
            '--- Parametri
            SqlUpd_MySQLCli.CommandText = "[UpdateInit_MySQLClienti]"
            SqlUpd_MySQLCli.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_MySQLCli.Connection = SqlConnCG
            SqlUpd_MySQLCli.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_MySQLCli.CommandTimeout = myTimeOUT '5000
            SqlUpd_MySQLCli.ExecuteNonQuery()
            If SqlConnCG.State <> ConnectionState.Closed Then
                SqlConnCG.Close()
            End If
            If SqlUpd_MySQLCli.Parameters("@RETURN_VALUE").Value = -1 Then
                strErrore = "Errore in aggiornamento DataBase MySQL Clienti (-1)"
                Return False
            Else
                Return True
            End If
        Catch exSQL As SqlException
            strErrore = "Errore in aggiornamento DataBase MySQL Clienti: " & exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = "Errore in aggiornamento DataBase MySQL Clienti: " & ex.Message
            Return False
        Finally
            If Not IsNothing(SqlConnCG) Then 'giu271119
                If SqlConnCG.State <> ConnectionState.Closed Then
                    SqlConnCG.Close()
                    SqlConnCG = Nothing
                End If
            End If
        End Try
    End Function

    Public Function GetLogInvioEmail(ByVal _Esercizio As String) As String
        GetLogInvioEmail = ""
        Dim strErrore As String = ""
        Try
            If CaricaParametri(_Esercizio, strErrore) Then
                If GetParamGestAzi(_Esercizio).AIServizioEmail = False Then
                    GetLogInvioEmail = "SERVIZIO INVIO EMAIL SOSPESO"
                    If GetParamGestAzi(_Esercizio).AIServizioEmailAttiva.Trim <> "" Then
                        GetLogInvioEmail = "SOSPESO - ATTIVAZIONE PREVISTA PER LE ORE " & GetParamGestAzi(_Esercizio).AIServizioEmailAttiva.Trim
                    End If
                    'giu270219
                    Exit Function
                End If
            Else
                GetLogInvioEmail = "Errore GetLogInvioEmail: Caricamento parametri generali. " & strErrore
                Exit Function
            End If
        Catch ex As Exception
            GetLogInvioEmail = "Errore GetLogInvioEmail: Caricamento parametri generali. " & strErrore
            Exit Function
        End Try
        Dim strSQL As String = ""
        strSQL = "Select ISNULL(Descrizione,'') AS Descrizione From Abilitazioni WHERE Chiave='InvioEmail'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbOpzioni, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Descrizione")) Then
                        GetLogInvioEmail = ds.Tables(0).Rows(0).Item("Descrizione")
                    Else
                        GetLogInvioEmail = ""
                    End If
                    Exit Function
                Else
                    GetLogInvioEmail = ""
                    Exit Function
                End If
            Else
                GetLogInvioEmail = ""
                Exit Function
            End If
        Catch Ex As Exception
            GetLogInvioEmail = "Errore: " & Ex.Message
            Exit Function
        End Try

    End Function
    'giu301018
    Public Function GetLogBKAuto(ByVal _Esercizio As String) As String
        GetLogBKAuto = ""
        Dim strErrore As String = ""
        ' ''Try
        ' ''    If CaricaParametri(_Esercizio, strErrore) Then
        ' ''        If GetParamGestAzi(_Esercizio).AIServizioEmail = False Then
        ' ''            GetLogBKAuto = "SERVIZIO INVIO EMAIL SOSPESO"
        ' ''            Exit Function
        ' ''        End If
        ' ''    Else
        ' ''        GetLogBKAuto = "Errore GetLogBKAuto: Caricamento parametri generali. " & strErrore
        ' ''        Exit Function
        ' ''    End If
        ' ''Catch ex As Exception
        ' ''    GetLogBKAuto = "Errore GetLogBKAuto: Caricamento parametri generali. " & strErrore
        ' ''    Exit Function
        ' ''End Try
        Dim strSQL As String = ""
        strSQL = "Select ISNULL(Descrizione,'') AS Descrizione From Abilitazioni WHERE Chiave='BKAuto'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbOpzioni, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Descrizione")) Then
                        GetLogBKAuto = ds.Tables(0).Rows(0).Item("Descrizione")
                    Else
                        GetLogBKAuto = ""
                    End If
                    Exit Function
                Else
                    GetLogBKAuto = ""
                    Exit Function
                End If
            Else
                GetLogBKAuto = ""
                Exit Function
            End If
        Catch Ex As Exception
            GetLogBKAuto = "Errore: " & Ex.Message
            Exit Function
        End Try

    End Function

    'FUNZIONI COMMENTATE CHE POSSONO SERVIRE GIUSEPPE
    ' ''Public Sub AddInputParameter(ByRef parameters As ArrayList, ByVal name As String, ByVal type As Integer, _
    ' ''                             ByVal value As Object, ByVal size As Integer, Optional ByVal nQueryType As Integer = 0)

    ' ''    parameters.Add(New SqlParameter(name, type, size, ParameterDirection.Input))

    ' ''    If (nQueryType = 3) Then
    ' ''        If (type.Equals(SqlDbType.Int) Or type.Equals(SqlDbType.Float) Or type.Equals(SqlDbType.Money) Or type.Equals(SqlDbType.Real)) Then
    ' ''            parameters(parameters.Count - 1).Value = 0
    ' ''        ElseIf (type.Equals(SqlDbType.VarChar)) Then
    ' ''            parameters(parameters.Count - 1).Value = "-"
    ' ''        End If
    ' ''    Else
    ' ''        parameters(parameters.Count - 1).Value = value
    ' ''    End If

    ' ''    If (value <> Nothing And type.Equals(SqlDbType.Time)) Then
    ' ''        If (nQueryType = 3) Then
    ' ''            parameters(parameters.Count - 1).Value = DateTime.Parse("0:0").TimeOfDay
    ' ''        Else
    ' ''            parameters(parameters.Count - 1).Value = DateTime.Parse(value).TimeOfDay
    ' ''        End If
    ' ''    End If

    ' ''    If (type.Equals(SqlDbType.Bit)) Then
    ' ''        If (nQueryType = 3) Then
    ' ''            parameters(parameters.Count - 1).Value = 0
    ' ''        Else
    ' ''            If (value = True) Then
    ' ''                parameters(parameters.Count - 1).Value = 1
    ' ''            Else
    ' ''                parameters(parameters.Count - 1).Value = 0
    ' ''            End If
    ' ''        End If
    ' ''    End If
    ' ''End Sub

    ' ''Public Sub AddOutputParameter(ByRef parameters As ArrayList, ByVal name As String, ByVal type As Integer, ByVal value As Object, ByVal size As Integer)
    ' ''    parameters.Add(New SqlParameter(name, type, size, ParameterDirection.Output))
    ' ''    parameters(parameters.Count - 1).Value = value
    ' ''End Sub


    ' ''Public Function ExectuteStoredProcedure(ByVal TipoDB As Integer, ByVal sStoredProcedure As String, ByVal parameters As ArrayList, Optional ByVal modalPopup As WUC_ModalPopup = Nothing) As Boolean
    ' ''    connectionString = dbCon.getConnectionString(TipoDB)

    ' ''    Dim conn As New SqlConnection(connectionString.ToString())
    ' ''    Dim command As New SqlCommand(sStoredProcedure, conn)

    ' ''    Try
    ' ''        command.CommandType = CommandType.StoredProcedure

    ' ''        parameters.Add(New SqlParameter("RETURN", SqlDbType.VarChar, 100))
    ' ''        parameters(parameters.Count - 1).Direction = ParameterDirection.ReturnValue

    ' ''        For Each param As SqlParameter In parameters
    ' ''            command.Parameters.Add(param)
    ' ''        Next
    '' ''giu190617
    ' ''Dim strValore As String = ""
    ' ''Dim strErrore As String = ""
    ' ''Dim myTimeOUT As Long = 5000
    ' ''        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
    ' ''            If IsNumeric(strValore.Trim) Then
    ' ''                If CLng(strValore.Trim) > myTimeOUT Then
    ' ''                    myTimeOUT = CLng(strValore.Trim)
    ' ''                End If
    ' ''            End If
    ' ''        End If
    '' ''esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
    '---------------------------
    ' ''        command.Connection.Open()
    ' ''        command.CommandTimeout = myTimeOUT
    ' ''        command.ExecuteNonQuery()
    ' ''    Catch ex As Exception
    ' ''        ExectuteStoredProcedure = False
    ' ''        If (parameters(parameters.Count - 1).value Is Nothing) Then
    ' ''            Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''            modalPopup.Show("Errore " + sStoredProcedure + "", "Si è verificato un errore durante l'esecuzione della procedura indicata." + vbCrLf + "Contattare l'amministratore di sistema", WUC_ModalPopup.TYPE_ERROR)
    ' ''            Exit Function
    ' ''        Else
    ' ''            If (parameters(parameters.Count - 1).value <> 0) Then
    ' ''                If (parameters(parameters.Count - 2).value <> "") Then
    ' ''                    modalPopup.Show("Errore " + sStoredProcedure + "", parameters(parameters.Count - 2).value + vbCrLf + "Contattare l'amministratore di sistema", WUC_ModalPopup.TYPE_ERROR)
    ' ''                Else
    ' ''                    modalPopup.Show("Errore " + sStoredProcedure + "", "Si è verificato un errore durante l'esecuzione della procedura indicata." + vbCrLf + "Contattare l'amministratore di sistema", WUC_ModalPopup.TYPE_ERROR)
    ' ''                End If
    ' ''                Exit Function
    ' ''            Else
    ' ''                modalPopup.Show("Errore", "Si è presentato un errore non gestito, non sarà possibile continuare con l'operazione richiesta.", WUC_ModalPopup.TYPE_ERROR)
    ' ''                Exit Function
    ' ''            End If
    ' ''        End If
    ' ''    End Try

    ' ''    If (parameters(parameters.Count - 1).value = 1) Then
    ' ''        modalPopup.Show("Errore", "Si è presentato un errore non gestito, non sarà possibile continuare con l'operazione richiesta.", WUC_ModalPopup.TYPE_ERROR)
    ' ''        ExectuteStoredProcedure = False
    ' ''        Exit Function
    ' ''    End If

    ' ''    If (parameters(parameters.Count - 1).value = 2) Then
    ' ''        modalPopup.Show("Errore", "Non è possibile eliminare il record selezionato dovuto a che è referenziato da altre tabelle.", WUC_ModalPopup.TYPE_ERROR)
    ' ''        ExectuteStoredProcedure = False
    ' ''        Exit Function
    ' ''    End If

    ' ''    If (parameters(parameters.Count - 1).value = 3) Then
    ' ''        modalPopup.Show("Errore", "Non è possibile effettuare l'operazione richiesta dovuto alla incongruenza dei dati presenti nel record corrente.", WUC_ModalPopup.TYPE_ERROR)
    ' ''        ExectuteStoredProcedure = False
    ' ''        Exit Function
    ' ''    End If

    ' ''    If (parameters(parameters.Count - 1).value = 4 Or parameters(parameters.Count - 1).value = 41) Then
    ' ''        modalPopup.Show("Errore", Compose_WarningMessage_DuplicateKey(sStoredProcedure, parameters), WUC_ModalPopup.TYPE_ERROR)
    ' ''        ExectuteStoredProcedure = False
    ' ''        Exit Function
    ' ''    End If

    ' ''    ExectuteStoredProcedure = True
    ' ''End Function

    ' ''Public Sub ComposeQueryFilter(ByRef sQuery As String, ByVal oControlCollection As Collection, ByVal oCheckCollection As Collection, ByVal lstFieldList As ArrayList)
    ' ''    Dim i As Integer

    ' ''    For i = 1 To oControlCollection.Count
    ' ''        Dim sFieldName As String = lstFieldList.Item(i - 1)

    ' ''        If (oControlCollection(i).ToString().Contains("TextBox")) Then

    ' ''            Dim textbox As TextBox = CType(oControlCollection(i), TextBox)
    ' ''            Dim sColumnName As String = textbox.ID.Split("_")(1)

    ' ''            If (Not textbox.Text.Equals(String.Empty)) Then
    ' ''                sQuery += " And " & sFieldName & " = '" & textbox.Text & "'"
    ' ''            End If

    ' ''        ElseIf (oControlCollection(i).ToString().Contains("DropDownList")) Then

    ' ''            Dim dropdownlist As DropDownList = CType(oControlCollection(i), DropDownList)
    ' ''            Dim sColumnName As String = dropdownlist.ID.Split("_")(1)
    ' ''            Dim bChecked As Boolean = CheckCheckedControl(oCheckCollection, sColumnName)

    ' ''            If (Not dropdownlist.SelectedValue.Equals(String.Empty) And bChecked.Equals(True)) Then
    ' ''                sQuery += " And " & sFieldName & " = '" & GetValue_From_Split(dropdownlist.SelectedValue) & "'"
    ' ''            End If

    ' ''        ElseIf (oControlCollection(i).ToString().Contains("Calendar")) Then

    ' ''            Dim calendar1 As WUC_Calendar = CType(oControlCollection(i), WUC_Calendar)
    ' ''            Dim calendar2 As WUC_Calendar = CType(oControlCollection(i + 1), WUC_Calendar)
    ' ''            Dim textbox1 As TextBox = calendar1.Controls(0)
    ' ''            Dim textbox2 As TextBox = calendar2.Controls(0)
    ' ''            Dim sColumnName As String = calendar1.ID.Split("_")(1)
    ' ''            Dim data1 As String = String.Empty
    ' ''            Dim data2 As String = String.Empty

    ' ''            If (Not textbox1.Text.Equals(String.Empty)) Then
    ' ''                data1 = Format("{0:d}", textbox1.Text)
    ' ''            End If

    ' ''            If (Not textbox2.Text.Equals(String.Empty)) Then
    ' ''                data2 = Format("{0:d}", textbox2.Text)
    ' ''            End If

    ' ''            If (Not data1.Equals(String.Empty) And Not data2.Equals(String.Empty)) Then
    ' ''                sQuery += String.Format(" And {0} >= CONVERT(DATETIME,'{1}', 103) AND {0} <= CONVERT(DATETIME,'{2}', 103)", sFieldName, data1, data2)
    ' ''            ElseIf (Not data1.Equals(String.Empty)) Then
    ' ''                sQuery += String.Format(" And {0} = CONVERT(DATETIME,'{1}', 103)", sFieldName, data1)
    ' ''            ElseIf (Not data2.Equals(String.Empty)) Then
    ' ''                sQuery += String.Format(" And {0} = CONVERT(DATETIME,'{1}', 103)", sFieldName, data2)
    ' ''            End If
    ' ''            i = i + 1

    ' ''        ElseIf (oControlCollection(i).ToString().Contains("CheckBox")) Then

    ' ''            Dim checkbox As CheckBox = CType(oControlCollection(i), CheckBox)

    ' ''            If (checkbox.Checked.Equals(True)) Then
    ' ''                sQuery += " And " & sFieldName & " = 1 "
    ' ''            ElseIf (checkbox.Checked.Equals(False)) Then
    ' ''                sQuery += " And " & sFieldName & " = 0 "
    ' ''            End If

    ' ''        End If
    ' ''    Next
    ' ''End Sub

    ' ''Private Function GetValue_From_Split(ByVal value As String)
    ' ''    If (value.Contains("|")) Then
    ' ''        GetValue_From_Split = value.Split("|")(0).Trim()
    ' ''    Else
    ' ''        GetValue_From_Split = value
    ' ''    End If
    ' ''End Function

    ' ''Private Function CheckCheckedControl(ByVal oCheckCollection As Collection, ByVal sColumnName As String) As Boolean
    ' ''    For i = 1 To oCheckCollection.Count
    ' ''        If (oCheckCollection(i).ToString().Contains("CheckBox")) Then
    ' ''            Dim checkbox As CheckBox = CType(oCheckCollection(i), CheckBox)
    ' ''            If (checkbox.ID.Contains(sColumnName)) Then
    ' ''                CheckCheckedControl = checkbox.Checked
    ' ''                Exit Function
    ' ''            End If
    ' ''        End If
    ' ''    Next
    ' ''    CheckCheckedControl = False
    ' ''End Function

    ' ''Public Sub PopulateGridView(ByVal TipoDB As TipoConnessione, ByRef oGridView As GridView, ByVal sQuery As String, ByVal oModuleOfSearch As ModuleOfSearch, ByVal session As HttpSessionState, Optional ByVal sQueryName As String = Nothing, Optional ByVal bRefresh As Boolean = False)
    ' ''    Dim ds As New DataSet

    ' ''    Dim listLogOnUtente As ArrayList = HttpRuntime.Cache("listLogOnUtente")
    ' ''    For Each utente As LogOnUtente In listLogOnUtente
    ' ''        If (utente.SessionId.Equals(session.SessionID)) Then
    ' ''            If (Not utente.Azienda.Equals("")) Then
    ' ''                Dim sTableName As String = ""
    ' ''                Dim sFrom As String = ""
    ' ''                For i As Integer = 0 To sQuery.Count - 1
    ' ''                    If (sQuery(i) <> " " And sFrom.ToUpper() <> "FROM") Then
    ' ''                        sFrom += sQuery(i)
    ' ''                    ElseIf (sQuery(i) = " " And sFrom.ToUpper() <> "FROM") Then
    ' ''                        sFrom = ""
    ' ''                    ElseIf (sQuery(i) <> " " And sFrom.ToUpper() = "FROM") Then
    ' ''                        sTableName += sQuery(i)
    ' ''                    ElseIf (sTableName <> "" And sFrom.ToUpper() = "FROM") Then
    ' ''                        Exit For
    ' ''                    End If
    ' ''                Next

    ' ''                If (Not sQuery.ToUpper().Contains("WHERE")) Then
    ' ''                    sQuery += " WHERE " & sTableName & ".IdAzienda = " & utente.IdAzienda
    ' ''                Else
    ' ''                    sQuery += " AND " & sTableName & ".IdAzienda = " & utente.IdAzienda
    ' ''                End If
    ' ''            End If
    ' ''            Exit For
    ' ''        End If
    ' ''    Next

    ' ''    If (Not (oModuleOfSearch Is Nothing)) Then
    ' ''        ComposeQueryFilter(sQuery, oModuleOfSearch.ControlCollection, oModuleOfSearch.ChechCollection, oModuleOfSearch.FieldList)
    ' ''    End If

    ' ''    If (Not sQueryName Is Nothing) Then
    ' ''        If (bRefresh) Then
    ' ''            PopulateDatasetFromQuery(TipoDB, sQuery, ds)
    ' ''            session.Add(sQueryName, ds)
    ' ''            oGridView.PageSize = 15
    ' ''        Else
    ' ''            If (session(sQueryName) Is Nothing) Then
    ' ''                PopulateDatasetFromQuery(TipoDB, sQuery, ds)
    ' ''                oGridView.PageSize = 15
    ' ''                session.Add(sQueryName, ds)
    ' ''            Else
    ' ''                ds = session(sQueryName)
    ' ''            End If
    ' ''        End If
    ' ''    Else
    ' ''        oGridView.PageSize = 15
    ' ''        PopulateDatasetFromQuery(TipoDB, sQuery, ds)
    ' ''    End If

    ' ''    If (ds.Tables.Count > 0) Then
    ' ''        oGridView.DataSource = ds.Tables(0)
    ' ''    Else
    ' ''        oGridView.DataSource = Nothing
    ' ''    End If

    ' ''    oGridView.DataBind()
    ' ''End Sub

    ' ''Public Sub PopulateDetailsView(ByVal TipoDB As TipoConnessione, ByRef oDetailsView As DetailsView, ByVal sQuery As String)
    ' ''    Dim ds As New DataSet

    ' ''    PopulateDatasetFromQuery(TipoDB, sQuery, ds)
    ' ''    If (ds.Tables.Count > 0) Then
    ' ''        oDetailsView.DataSource = ds.Tables(0)
    ' ''    Else
    ' ''        oDetailsView.DataSource = Nothing
    ' ''    End If

    ' ''    oDetailsView.DataBind()
    ' ''End Sub

    ' ''Public Sub PopulateArrayList(ByVal TipoDB As TipoConnessione, ByRef list As ArrayList, ByVal sColumnName As String, ByVal sQuery As String, Optional ByVal sColumnNameDesc1 As String = Nothing, Optional ByVal sColumnNameDesc2 As String = Nothing)
    ' ''    Dim ds As New DataSet

    ' ''    PopulateDatasetFromQuery(TipoDB, sQuery, ds)

    ' ''    If (ds.Tables.Count > 0) Then
    ' ''        For Each r As DataRow In ds.Tables(0).Rows
    ' ''            If ((Not sColumnNameDesc1 Is Nothing) And (Not sColumnNameDesc2 Is Nothing)) Then
    ' ''                list.Add(r(sColumnName) & " | " & r(sColumnNameDesc1) & " " + r(sColumnNameDesc2))
    ' ''            ElseIf ((Not sColumnNameDesc1 Is Nothing) And (sColumnNameDesc2 Is Nothing)) Then
    ' ''                list.Add(r(sColumnName) & " | " & r(sColumnNameDesc1))
    ' ''            ElseIf ((sColumnNameDesc1 Is Nothing) And (sColumnNameDesc2 Is Nothing)) Then
    ' ''                list.Add(r(sColumnName))
    ' ''            End If
    ' ''        Next
    ' ''    Else
    ' ''        list = Nothing
    ' ''    End If

    ' ''End Sub

    ' ''Private Function Compose_WarningMessage_DuplicateKey(ByVal sStoredProcedure As String, ByVal parameters As ArrayList) As String
    ' ''    Dim sMessage As String = "Non è possibile effettuare effettuare l'operazione richiesta dovuto a che esiste già un record avente: {0}<br>"

    ' ''    Dim sDetails As String = ""

    ' ''    If (sStoredProcedure = "dbo.sp_AssocClienteAzienda") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Cliente: {0}<br>Azienda: {1}<br>", _
    ' ''            parameters(2).value, parameters(3).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Indirizzi") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Descrizione: {0}<br>Comune: {1}<br>", _
    ' ''            parameters(3).value, parameters(7).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Comuni") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Descrizione: {0}<br>Regione: {1}<br>", _
    ' ''            parameters(2).value, parameters(3).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Regioni") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Descrizione: {0}<br>", _
    ' ''            parameters(2).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Attivita") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Numero Intervento: {0}<br>Progressivo: {1}<br>Codice Attività: {2}<br>", _
    ' ''            parameters(3).value, parameters(4).value, parameters(2).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_AttrezzaturaInstallata") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>K Number: {0}<br>", parameters(5).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Azienda" Or _
    ' ''            sStoredProcedure = "dbo.sp_Cliente") Then

    ' ''        sDetails = String.Format("<br><br>Ragione Sociale: {0}<br>", parameters(2).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Attrezzatura" Or _
    ' ''            sStoredProcedure = "dbo.sp_CausaleIndisponibilita" Or _
    ' ''            sStoredProcedure = "dbo.sp_CondizioniPagamento" Or _
    ' ''            sStoredProcedure = "dbo.sp_ContrattoAssAttInstallata" Or _
    ' ''            sStoredProcedure = "dbo.sp_Difetto" Or _
    ' ''            sStoredProcedure = "dbo.sp_Ricambio" Or _
    ' ''            sStoredProcedure = "dbo.sp_TipoApparecchiatura" Or _
    ' ''            sStoredProcedure = "dbo.sp_Zona") Then

    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Codice: {0}<br>", parameters(2).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_IndisponibilitaTecnico") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Causale: {0}<br>Cespite: {1}<br>Data Indisp. Da: {2}<br>Data Indisp. A: {3}<br>", _
    ' ''            parameters(5).value, parameters(2).value, parameters(3).value, parameters(4).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Intervento") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Numero Intervento: {0}<br>Progressivo: {1}<br>", _
    ' ''            parameters(2).value, parameters(9).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_InterventoProgrammato") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Cod. Contratto Ass.: {0}<br>Progressivo: {1}<br>", _
    ' ''            parameters(4).value, parameters(2).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_SostituzioneRicambi") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Numero Intervento: {0}<br>Progressivo: {1}<br>Codice Ricambio: {2}<br>", _
    ' ''            parameters(2).value, parameters(3).value, parameters(4).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Struttura") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Descrizione: {0}<br>Cliente: {1}<br>", _
    ' ''            parameters(2).value, parameters(3).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Tecnico") Then
    ' ''        If (parameters(parameters.Count - 1).value = 41) Then
    ' ''            sMessage = "Non è possibile effettuare l'operazione richiesta dovuto a che il campo Cespite '{0}' è assegnato ad un'altro Tecnico<br>"
    ' ''            Compose_WarningMessage_DuplicateKey = String.Format(sMessage, parameters(2).value)
    ' ''            Exit Function
    ' ''        End If

    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Cognome: {0}<br>Nome: {1}<br>", _
    ' ''            parameters(3).value, parameters(4).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_TecnicoAssocAttInstallata") Then
    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Azienda: {0}<br>Attrezzatura Installata: {1}<br>Tecnico: {2}<br>", _
    ' ''            parameters(2).value, parameters(3).value, parameters(4).value)

    ' ''    ElseIf (sStoredProcedure = "dbo.sp_Utente") Then
    ' ''        If (parameters(parameters.Count - 1).value = 41) Then
    ' ''            sMessage = "Non è possibile effettuare l'operazione richiesta dovuto a che il campo Username '{0}' è assegnato ad un'altro utente<br>"
    ' ''            Compose_WarningMessage_DuplicateKey = String.Format(sMessage, parameters(2).value)
    ' ''            Exit Function
    ' ''        End If

    ' ''        sDetails = String.Format( _
    ' ''            "<br><br>Cognome: {0}<br>Nome: {1}<br>Tipo Uteza: {2}<br>Azienda: {3}<br>", _
    ' ''            parameters(5).value, parameters(6).value, parameters(9).value, parameters(4).value)
    ' ''    End If

    ' ''    Compose_WarningMessage_DuplicateKey = String.Format(sMessage, sDetails)
    ' ''End Function

    ' ''Public Function ExectuteSPAndPopulateDataTable(ByVal TipoDB As Integer, ByVal sDescErrorMessageTitle As String, ByVal sStoredProcedure As String, ByVal parameters As ArrayList, ByRef dt As DataTable, Optional ByVal modalPopup As WUC_ModalPopup = Nothing) As Boolean
    ' ''    connectionString = dbCon.getConnectionString(TipoDB)

    ' ''    Dim conn As New SqlConnection(connectionString.ToString())
    ' ''    Dim command As New SqlCommand(sStoredProcedure, conn)
    ' ''    Dim da As New SqlDataAdapter(sStoredProcedure, conn)

    ' ''    Try
    ' ''        da.SelectCommand.CommandType = CommandType.StoredProcedure
    ' ''        For Each param As SqlParameter In parameters
    ' ''            da.SelectCommand.Parameters.Add(param)
    ' ''        Next

    ' ''        Dim ds As New DataSet
    ' ''        da.Fill(ds)
    ' ''        da.Dispose()

    ' ''        If (ds.Tables.Count > 0) Then
    ' ''            If (ds.Tables(0).Rows.Count > 0) Then
    ' ''                ExectuteSPAndPopulateDataTable = True
    ' ''                conn.Close()
    ' ''                dt = ds.Tables(0)
    ' ''                Exit Function
    ' ''            End If
    ' ''        End If

    ' ''        ExectuteSPAndPopulateDataTable = False
    ' ''    Catch ex As Exception
    ' ''        modalPopup.Show( _
    ' ''            String.Format("Errore {0}", sDescErrorMessageTitle), _
    ' ''            String.Format("Si è verificato un errore non gestito. {0}\nContattare l'amministratore di sistema", ex.Message), _
    ' ''            WUC_ModalPopup.TYPE_ERROR)

    ' ''        ExectuteSPAndPopulateDataTable = False
    ' ''    End Try

    ' ''    conn.Close()
    ' ''End Function

    Public Function ExecUpgNoteRitiro(ByVal _IDDoc As String, ByVal _NoteRitiro As String, ByRef strErrore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnSC = New System.Data.SqlClient.SqlConnection
        '-
        Dim SqlUpgNoteRitiro = New System.Data.SqlClient.SqlCommand
        '
        'SqlConnection1
        '
        SqlConnSC.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        '
        Dim strValore As String = ""
        ' ''Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '---------------------------
        Try
            If SqlConnSC.State <> ConnectionState.Open Then
                SqlConnSC.Open()
            End If
            '--- Parametri
            SqlUpgNoteRitiro.CommandText = "[Update_NoteRitiro]"
            SqlUpgNoteRitiro.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpgNoteRitiro.Connection = SqlConnSC
            SqlUpgNoteRitiro.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpgNoteRitiro.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, "IDDocumenti"))
            SqlUpgNoteRitiro.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteRitiro", System.Data.SqlDbType.NVarChar, 1073741823, "NoteRitiro"))
            '
            SqlUpgNoteRitiro.Parameters("@IDDocumenti").Value = CLng(_IDDoc.Trim)
            SqlUpgNoteRitiro.Parameters("@NoteRitiro").Value = _NoteRitiro.Trim
            SqlUpgNoteRitiro.CommandTimeout = myTimeOUT '5000
            SqlUpgNoteRitiro.ExecuteNonQuery()

            If SqlConnSC.State <> ConnectionState.Closed Then
                SqlConnSC.Close()
            End If

            Return True
        Catch exSQL As SqlException
            strErrore = exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        Finally
            If Not IsNothing(SqlConnSC) Then 'giu271119
                If SqlConnSC.State <> ConnectionState.Closed Then
                    SqlConnSC.Close()
                    SqlConnSC = Nothing
                End If
            End If
        End Try
    End Function

    'giu181223 
    Public Function ExecUpgScadPagCA(ByVal _IDDoc As String, ByVal _ScadPagCA As String, ByRef strErrore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnSC = New System.Data.SqlClient.SqlConnection
        '-
        Dim SqlUpgNoteRitiro = New System.Data.SqlClient.SqlCommand
        '
        'SqlConnection1
        '
        SqlConnSC.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        '
        Dim strValore As String = ""
        ' ''Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        '---------------------------
        Try
            If SqlConnSC.State <> ConnectionState.Open Then
                SqlConnSC.Open()
            End If
            '--- Parametri
            SqlUpgNoteRitiro.CommandText = "[Update_ScadPagCA]"
            SqlUpgNoteRitiro.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpgNoteRitiro.Connection = SqlConnSC
            SqlUpgNoteRitiro.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpgNoteRitiro.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, "IDDocumenti"))
            SqlUpgNoteRitiro.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScadPagCA", System.Data.SqlDbType.NVarChar, 1073741823, "ScadPagCA"))
            '
            SqlUpgNoteRitiro.Parameters("@IDDocumenti").Value = CLng(_IDDoc.Trim)
            SqlUpgNoteRitiro.Parameters("@ScadPagCA").Value = _ScadPagCA.Trim
            SqlUpgNoteRitiro.CommandTimeout = myTimeOUT '5000
            SqlUpgNoteRitiro.ExecuteNonQuery()

            If SqlConnSC.State <> ConnectionState.Closed Then
                SqlConnSC.Close()
            End If

            Return True
        Catch exSQL As SqlException
            strErrore = exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        Finally
            If Not IsNothing(SqlConnSC) Then 'giu271119
                If SqlConnSC.State <> ConnectionState.Closed Then
                    SqlConnSC.Close()
                    SqlConnSC = Nothing
                End If
            End If
        End Try
    End Function
End Class
