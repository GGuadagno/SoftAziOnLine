Imports It.SoftAzi.Model.Facade 'Ho tutte le funzioni es. get_Operatori
Imports It.SoftAzi.Model.Entity
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility

Public Class SessionUtility

    Public Shared Function GetLogOnUtente(ByVal Nome As String, ByVal CodiceDitta As String, ByVal Postazione As String, ByVal Modulo As String, ByVal SessionID As String, ByVal ID As Integer, ByVal Codice As String, ByVal Azienda As String, ByVal Tipo As String, ByVal _Esercizio As String) As OperatoreConnessoEntity
        'giu050312
        If String.IsNullOrEmpty(Modulo) Then
            GetLogOnUtente = Nothing
            Exit Function
        End If
        If String.IsNullOrEmpty(SessionID) Then
            GetLogOnUtente = Nothing
            Exit Function
        End If
        GetLogOnUtente = Nothing
        Try
            Dim OpSys As New Operatori
            Dim ArrLogOnUtente As ArrayList = OpSys.OperatoreConnesso(Nome, CodiceDitta, Postazione, NomeModulo, SessionID, ID, Codice, Azienda, Tipo, _Esercizio)
            If (ArrLogOnUtente Is Nothing) Then
                GetLogOnUtente = Nothing
                Exit Function
            End If
            If ArrLogOnUtente.Count = 0 Then
                GetLogOnUtente = Nothing
                Exit Function
            Else
                For Each ut As OperatoreConnessoEntity In ArrLogOnUtente
                    If (ut.SessionID.Equals(SessionID)) Then
                        GetLogOnUtente = ut
                        Exit Function
                    End If
                    If Postazione.Trim <> "" Then
                        For Each ut2 As OperatoreConnessoEntity In ArrLogOnUtente
                            If (ut.Postazione.Equals(Postazione)) Then
                                If (ut.Modulo.Equals(NomeModulo)) Then
                                    GetLogOnUtente = ut2
                                    Exit Function
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        Catch ex As Exception
            GetLogOnUtente = Nothing
            Exit Function
        End Try

    End Function
    
    Public Shared Sub LogOutUtente(ByVal Session As HttpSessionState, ByVal response As HttpResponse)
        DelOpBySessionePostazione(Session.SessionID, "", NomeModulo)
        Session.Abandon()
        response.Redirect("..\Login.aspx?SessioneScaduta=0")
    End Sub
    Public Shared Function DelOpBySessionePostazione(ByVal strSessione As String, ByVal strPostazione As String, ByVal Modulo As String) As Boolean
        DelOpBySessionePostazione = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "DELETE FROM OperatoriConnessi "
            SQLStr += "Where (SessionID= '" & strSessione.Trim & "')"
            'giu190412 SQLStr += " OR (Postazione= '" & strPostazione.Trim & "' AND Modulo= '" & Modulo.Trim & "')"
            DelOpBySessionePostazione = ObjDB.ExecuteQueryUpdate(TipoDB.dbInstall, SQLStr)
        Catch ex As Exception
            DelOpBySessionePostazione = False
        End Try
        ObjDB = Nothing
    End Function
    'GIU240412
    Public Shared Function DelOpByNome(ByVal _NomeOp As String, ByRef _Errore As String) As Boolean
        DelOpByNome = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "DELETE FROM OperatoriConnessi "
            SQLStr += "WHERE NomeOperatore = '" & _NomeOp & "'"
            DelOpByNome = ObjDB.ExecuteQueryUpdate(TipoDB.dbInstall, SQLStr)
        Catch ex As Exception
            DelOpByNome = False
            _Errore = "Errore in DelOpByNome: " & ex.Message
        End Try
        ObjDB = Nothing
    End Function
    Public Shared Function CTROpBySessionePostazione(ByVal strSessione As String, ByVal strPostazione As String, ByVal Modulo As String) As Boolean
        CTROpBySessionePostazione = False
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "SELECT * FROM OperatoriConnessi "
            SQLStr += " Where (SessionID= '" & Mid(strSessione.Trim, 1, 50) & "')"
            'giu190412 SQLStr += " OR (Postazione= '" & strPostazione.Trim & "' AND Modulo= '" & Modulo.Trim & "')"
            Dim ds As New DataSet
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, SQLStr, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    CTROpBySessionePostazione = True
                    'da fare 
                    ' ''If Not IsDBNull(ds.Tables(0).Rows(0).Item("DataOraUltAzione")) Then
                    ' ''    If DateDiff(DateInterval.Hour, ds.Tables(0).Rows(0).Item("DataOraUltAzione"), Now) > 1 Then
                    ' ''        CTROpBySessionePostazione = False
                    ' ''    End If
                    ' ''End If
                End If
            End If
        Catch ex As Exception
            CTROpBySessionePostazione = False
        End Try
        ObjDB = Nothing
    End Function
    'giu290412
    Public Shared Function CTROpTimeOUT(ByRef _strErrore As String, Optional ByVal _Utente As String = "") As Boolean
        CTROpTimeOUT = True
        _strErrore = ""
        Dim strSQL As String = "" : Dim i As Integer = 0 : Dim ii As Integer = 0
        strSQL = "SELECT * FROM View_OperatoriConnessi "
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("Modulo")) Then
                            If UCase(ds.Tables(0).Rows(ii).Item("Modulo").ToString.Trim) <> "COGE" Then
                                If ds.Tables(0).Rows(ii).Item("NomeOperatore").ToString.Trim <> _Utente.Trim Then
                                    If Not IsDBNull(ds.Tables(0).Rows(ii).Item("DataOraUltAzione")) Then
                                        If DateDiff(DateInterval.Hour, ds.Tables(0).Rows(ii).Item("DataOraUltAzione"), Now) > 1 Then
                                            If SessionUtility.DelOpByNome(ds.Tables(0).Rows(ii).Item("NomeOperatore").ToString.Trim, _strErrore) = False Then
                                                CTROpTimeOUT = False
                                                Exit Function
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        ii += 1
                    Next
                End If
            End If
        Catch Ex As Exception
            _strErrore = Ex.Message
            CTROpTimeOUT = False
            Exit Function
        End Try
    End Function
    'GIU050718
    Public Shared Function GetDatoOperatore(ByVal _NomeOp As String, ByRef _Dato As String, ByRef _Errore As String) As Boolean
        GetDatoOperatore = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        _Errore = ""
        SQLStr = "SELECT * FROM Operatori WHERE Nome='" & _NomeOp.Trim & "'"
        Dim ds As New DataSet
        Try
            GetDatoOperatore = ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, SQLStr, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item(_Dato)) Then
                        _Dato = ds.Tables(0).Rows(0).Item(_Dato).ToString.Trim
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
        ObjDB = Nothing
    End Function
    'GIU050412
    Public Shared Function BloccoOperatore(ByVal _NomeOp As String, ByRef _Errore As String) As Boolean
        BloccoOperatore = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE Operatori SET NessunaScadenza=0,"
            SQLStr += " DataScadenza = CONVERT(DATETIME,'" & Replace(Format(DateAdd(DateInterval.Day, -1, Now), FormatoDataOra), ".", ":") & "' ,103)"
            SQLStr += " WHERE Nome = '" & _NomeOp & "'"
            BloccoOperatore = ObjDB.ExecuteQueryUpdate(TipoDB.dbInstall, SQLStr)
        Catch ex As Exception
            BloccoOperatore = False
        End Try
        ObjDB = Nothing
    End Function
    Public Shared Function SBloccoOperatore(ByVal _NomeOp As String, ByRef _Errore As String) As Boolean
        SBloccoOperatore = True
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE Operatori SET NessunaScadenza=1,"
            SQLStr += " DataScadenza = NULL"
            SQLStr += " WHERE Nome = '" & _NomeOp & "'"
            SBloccoOperatore = ObjDB.ExecuteQueryUpdate(TipoDB.dbInstall, SQLStr)
        Catch ex As Exception
            SBloccoOperatore = False
        End Try
        ObjDB = Nothing
    End Function
    'GIU040412 SALVATAGGIO DB
    Public Shared Function UpdDataUltimoBK(ByVal _Ditta As String, ByVal _Eser As String, ByRef _Errore As String) As Boolean
        UpdDataUltimoBK = True
        _Errore = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE Esercizi SET DataUltimoBk = CONVERT(DATETIME,'" & Replace(Format(Now, FormatoDataOra), ".", ":") & "' ,103)"
            SQLStr += " WHERE Ditta = '" & _Ditta & "' AND Esercizio = '" & _Eser & "'"
            UpdDataUltimoBK = ObjDB.ExecuteQueryUpdate(TipoDB.dbInstall, SQLStr)
        Catch ex As Exception
            _Errore = "Errore aggiorna data ultimo salvataggio DB: " & ex.Message.Trim
            UpdDataUltimoBK = False
        End Try
        ObjDB = Nothing
    End Function
    Public Shared Function BKAll(ByVal _Ditta As String, ByVal _Eser As String, ByRef _Errore As String) As Boolean
        BKAll = True
        _Errore = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        'INSTALL
        Try
            SQLStr = "sp_BackupDatabase '" & _Ditta.Trim & "Install', 'F'"
            BKAll = ObjDB.ExecuteQueryUpdate(TipoDB.dbMaster, SQLStr)
        Catch ex As Exception
            _Errore = "Errore salvataggio DB (" & _Ditta.Trim & "Install): " & ex.Message.Trim
            BKAll = False
            ObjDB = Nothing
            Exit Function
        End Try
        '[DITTA]OPZIONI
        Try
            SQLStr = "sp_BackupDatabase '" & _Ditta.Trim & "Opzioni', 'F'"
            BKAll = ObjDB.ExecuteQueryUpdate(TipoDB.dbMaster, SQLStr)
        Catch ex As Exception
            _Errore = "Errore salvataggio DB (" & _Ditta.Trim & "Opzioni): " & ex.Message.Trim
            BKAll = False
            ObjDB = Nothing
            Exit Function
        End Try
        '[DITTA]Scadenze
        Try
            SQLStr = "sp_BackupDatabase '" & _Ditta.Trim & "Scadenze', 'F'"
            BKAll = ObjDB.ExecuteQueryUpdate(TipoDB.dbMaster, SQLStr)
        Catch ex As Exception
            _Errore = "Errore salvataggio DB (" & _Ditta.Trim & "Scadenze): " & ex.Message.Trim
            BKAll = False
            ObjDB = Nothing
            Exit Function
        End Try
        '[DITTA][Esercizio]CoGe
        Try
            SQLStr = "sp_BackupDatabase '" & _Ditta.Trim & _Eser.Trim & "CoGe', 'F'"
            BKAll = ObjDB.ExecuteQueryUpdate(TipoDB.dbMaster, SQLStr)
        Catch ex As Exception
            _Errore = "Errore salvataggio DB (" & _Ditta.Trim & _Eser.Trim & "CoGe): " & ex.Message.Trim
            BKAll = False
            ObjDB = Nothing
            Exit Function
        End Try
        '[DITTA][Esercizio]GestAzi
        Try
            SQLStr = "sp_BackupDatabase '" & _Ditta.Trim & _Eser.Trim & "GestAzi', 'F'"
            BKAll = ObjDB.ExecuteQueryUpdate(TipoDB.dbMaster, SQLStr)
        Catch ex As Exception
            _Errore = "Errore salvataggio DB (" & _Ditta.Trim & _Eser.Trim & "GestAzi): " & ex.Message.Trim
            BKAll = False
            ObjDB = Nothing
            Exit Function
        End Try
        ObjDB = Nothing
    End Function
    Public Shared Function UpdDataUltimoBKErr(ByVal _Ditta As String, ByVal _Eser As String, ByRef _Errore As String) As Boolean
        UpdDataUltimoBKErr = True
        _Errore = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "UPDATE Esercizi SET DataUltimoBk = NULL "
            SQLStr += " WHERE Ditta = '" & _Ditta & "' AND Esercizio = '" & _Eser & "'"
            UpdDataUltimoBKErr = ObjDB.ExecuteQueryUpdate(TipoDB.dbInstall, SQLStr)
        Catch ex As Exception
            _Errore = "Errore aggiorna data ultimo salvataggio DB: " & ex.Message.Trim
            UpdDataUltimoBKErr = False
        End Try
        ObjDB = Nothing
    End Function

    'giu060412 Allineamento DATI CACHE
    Public Shared Function GetAggTabCG(ByVal _Eser As String, ByRef _ElencoTabAgg As String, ByRef _Errore As String) As Boolean
        GetAggTabCG = True
        _Errore = ""
        Dim strSQL As String = "" : Dim i As Integer = 0 : Dim ii As Integer = 0 : Dim NomeTab As String = ""
        strSQL = "SELECT * FROM _Aggiorna_Tabelle_"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("NomeTab")) Then
                            NomeTab = ds.Tables(0).Rows(ii).Item("NomeTab").ToString.Trim
                            Select Case NomeTab
                                Case "Clienti"
                                    GetAggTabCG = App.CaricaClienti(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Progressivi"
                                    GetAggTabCG = App.CaricaProgressiviCoGe(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Nazioni"
                                    GetAggTabCG = App.CaricaNazioni(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Aliquote_IVA"
                                    GetAggTabCG = App.CaricaAliquoteIva(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Province"
                                    GetAggTabCG = App.CaricaProvince(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Pagamenti"
                                    GetAggTabCG = App.CaricaPagamenti(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Zone"
                                    GetAggTabCG = App.CaricaZone(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Vettori"
                                    GetAggTabCG = App.CaricaVettori(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Categorie"
                                    GetAggTabCG = App.CaricaCategorie(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "PianoDeiConti"
                                    GetAggTabCG = App.CaricaPianoDeiConti(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Agenti"
                                    GetAggTabCG = App.CaricaAgenti(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                Case "Fornitori"
                                    GetAggTabCG = App.CaricaFornitori(_Eser, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                                    GetAggTabCG = DelAggTab(TipoDB.dbSoftCoge, NomeTab, _Errore)
                                    If GetAggTabCG = False Then Exit Function
                            End Select
                        End If
                        ii += 1
                    Next
                End If
            End If
        Catch Ex As Exception
            _Errore = "Errore caricamento dati CoGe: " & Ex.Message.Trim
            GetAggTabCG = False
            Exit Function
        End Try

    End Function
    Public Shared Function GetAggTabAZ(ByVal _Eser As String, ByRef _ElencoTabAgg As String, ByRef _Errore As String) As Boolean
        GetAggTabAZ = True
        _Errore = ""
        Dim strSQL As String = "" : Dim i As Integer = 0 : Dim ii As Integer = 0 : Dim NomeTab As String = ""
        strSQL = "SELECT * FROM _Aggiorna_Tabelle_"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("NomeTab")) Then
                            NomeTab = ds.Tables(0).Rows(ii).Item("NomeTab").ToString.Trim
                            Select Case NomeTab
                                Case "ParametriGeneraliAZI"
                                    GetAggTabAZ = App.CaricaParametri(_Eser, _Errore)
                                    If GetAggTabAZ = False Then Exit Function
                                    GetAggTabAZ = DelAggTab(TipoDB.dbSoftAzi, NomeTab, _Errore)
                                    If GetAggTabAZ = False Then Exit Function
                                Case "ListVenT"
                                    GetAggTabAZ = App.CaricaListVenT(_Eser, _Errore)
                                    If GetAggTabAZ = False Then Exit Function
                                    GetAggTabAZ = DelAggTab(TipoDB.dbSoftAzi, NomeTab, _Errore)
                                    If GetAggTabAZ = False Then Exit Function
                                Case "AnaMag"
                                    GetAggTabAZ = App.CaricaArticoli(_Eser, _Errore)
                                    If GetAggTabAZ = False Then Exit Function
                                    GetAggTabAZ = DelAggTab(TipoDB.dbSoftAzi, NomeTab, _Errore)
                                    If GetAggTabAZ = False Then Exit Function
                            End Select
                        End If
                        ii += 1
                    Next
                End If
            End If
        Catch Ex As Exception
            _Errore = "Errore caricamento dati Aziendale: " & Ex.Message.Trim
            GetAggTabAZ = False
            Exit Function
        End Try

    End Function
    Public Shared Function GetAggTabSC(ByVal _Eser As String, ByRef _ElencoTabAgg As String, ByRef _Errore As String) As Boolean
        GetAggTabSC = True
        _Errore = ""
        _ElencoTabAgg = ""
        Dim strSQL As String = "" : Dim i As Integer = 0 : Dim ii As Integer = 0 : Dim NomeTab As String = ""
        strSQL = "SELECT * FROM _Aggiorna_Tabelle_"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    ii = 0
                    For i = 1 To ds.Tables(0).Rows.Count
                        If Not IsDBNull(ds.Tables(0).Rows(ii).Item("NomeTab")) Then
                            NomeTab = ds.Tables(0).Rows(ii).Item("NomeTab").ToString.Trim
                            Select Case NomeTab
                                Case "ArticoliInst_ContrattiAss"
                                    _ElencoTabAgg = NomeTab
                                    GetAggTabSC = DelAggTab(TipoDB.dbScadenzario, NomeTab, _Errore)
                                    If GetAggTabSC = False Then Exit Function
                                Case "EmailInviateT"
                                    _ElencoTabAgg = NomeTab
                                    GetAggTabSC = DelAggTab(TipoDB.dbScadenzario, NomeTab, _Errore)
                                    If GetAggTabSC = False Then Exit Function
                                Case "EmailInviateDett"
                                    _ElencoTabAgg = NomeTab
                                    GetAggTabSC = DelAggTab(TipoDB.dbScadenzario, NomeTab, _Errore)
                                    If GetAggTabSC = False Then Exit Function
                            End Select
                        End If
                        ii += 1
                    Next
                End If
            End If
        Catch Ex As Exception
            _Errore = "Errore caricamento dati Scadenze: " & Ex.Message.Trim
            GetAggTabSC = False
            Exit Function
        End Try


    End Function
    Public Shared Function DelAggTab(ByVal _TipoDB As Integer, ByVal _NomeTab As String, Optional ByRef _Errore As String = "") As Boolean
        DelAggTab = True
        _Errore = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Try
            SQLStr = "DELETE FROM _Aggiorna_Tabelle_ "
            SQLStr += " WHERE NomeTab = '" & _NomeTab & "'"
            DelAggTab = ObjDB.ExecuteQueryUpdate(_TipoDB, SQLStr)
        Catch ex As Exception
            _Errore = ex.Message.Trim
            DelAggTab = False
        End Try
        ObjDB = Nothing
    End Function
    'Merge 
    Public Shared Sub Init(ByVal Session As HttpSessionState)
        'giu060412 ma xkè devo azzerare tutto ??????  proviamo senza 
        Session(COD_CLIENTE) = String.Empty
        Session(COD_ARTICOLO) = String.Empty
        Session(SWOP) = String.Empty
        Session(SWOPLOC) = String.Empty
        Session(SWOPDETTDOC) = String.Empty
        Session(SWOPDETTDOCR) = String.Empty
        Session(SWOPDETTDOCL) = String.Empty
        Session(SWMODIFICATO) = String.Empty

        'giu060412 ma xkè devo azzerare tutto ??????  proviamo senza 
        ' ''session(Def.ESERCIZIO) = String.Empty

        ' ''session(Def.L_VAR_ARTICOLI) = New List(Of ChiaveValore)
        ' ''session(Def.L_VAR_DESCESTESA) = New List(Of ChiaveValore)
        ' ''session(Def.L_VAR_FORNSEC) = New List(Of ChiaveValore)
        ' ''session(Def.L_VAR_FORNSEC_WFP) = New List(Of ChiaveValore)
        ' ''session(Def.L_VAR_CLIENTI) = New List(Of ChiaveValore)

        ' ''session(Def.L_VAR_ELENCO) = New List(Of ChiaveValore)
        ' ''session(Def.L_VAR_ELENCO_CLIFORN) = New List(Of ChiaveValore)
    End Sub
End Class
