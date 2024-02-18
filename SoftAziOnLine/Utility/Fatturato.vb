Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Public Class Fatturato

    'giu060212 'GIU180523 AGGIUNTO DATE RICH. ZIBORDI
    Public Function StampaFatturatoClienteFattura(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsFatturatoClienteFattura1 As DSFatturatoClienteFattura,
                                                  ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String,
                                                  Optional ByVal myCodFor As String = "", Optional ByVal Regione As Integer = -1, Optional ByVal Agente As Integer = -1,
                                                  Optional ByVal txtDataDa As String = "", Optional ByVal txtDataA As String = "") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = ""
        If myCodFor = "" Then
            strOrdCli = "SELECT * FROM view_FatturatoClienteFattura "
        Else
            strOrdCli = "SELECT * FROM view_FatturatoClienteFatturaFor "
        End If
        '-
        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
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
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1

        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If
        Dim SWWhere As Boolean = False
        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            strOrdCli = strOrdCli & " WHERE Cod_Cliente >= '" & Controlla_Apice(myStrDa.Trim) & "' AND Cod_Cliente <= '" & Controlla_Apice(myStrA.Trim) & "' "
            If myCodFor.Trim <> "" And myCodFor.Trim <> "-1" Then
                strOrdCli = strOrdCli & " AND CodiceFornitore = '" & Controlla_Apice(myCodFor.Trim) & "' "
            End If
            SWWhere = True
        ElseIf myCodFor.Trim <> "" And myCodFor.Trim <> "-1" Then
            strOrdCli = strOrdCli & " WHERE CodiceFornitore = '" & Controlla_Apice(myCodFor.Trim) & "' "
            SWWhere = True
        End If
        'GIU180523 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        If txtDataDa.Trim <> "" Then
            If SWWhere = True Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
                SWWhere = True
            End If
            '---------
            strOrdCli = strOrdCli & "Data_Doc >= CONVERT(datetime, '" & txtDataDa.Trim & "', 103) "
        End If

        If txtDataA.Trim <> "" Then
            If SWWhere = True Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
                SWWhere = True
            End If
            '---------
            strOrdCli = strOrdCli & "Data_Doc <= CONVERT(datetime, '" & txtDataA.Trim & "', 103) "
        End If
        '---------------------
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        'GIU300122
        If Agente > 0 Then
            If SWWhere = True Then
                strOrdCli = strOrdCli & " AND "
            Else
                strOrdCli = strOrdCli & " WHERE "
            End If
            strOrdCli = strOrdCli & "Agente=" & Agente.ToString.Trim
        End If
        If Regione > 0 Then
            If SWWhere = True Then
                strOrdCli = strOrdCli & " AND "
            Else
                strOrdCli = strOrdCli & " WHERE "
            End If
            strOrdCli = strOrdCli & "Regione=" & Regione.ToString.Trim
        End If
        '---------
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsFatturatoClienteFattura1.FatturatoClienteFattura)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As DSFatturatoClienteFattura.FatturatoClienteFatturaRow
        For Each r_OrdPerCli In dsFatturatoClienteFattura1.FatturatoClienteFattura
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsFatturatoClienteFattura1.AcceptChanges()

        Return True

    End Function

    'giu120212 giu180523
    Public Function StampaFatturatoClienteSintetico(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsFatturatoClienteFattura1 As DSFatturatoClienteFattura, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, Optional ByVal Regione As Integer = -1,
                                                  Optional ByVal txtDataDa As String = "", Optional ByVal txtDataA As String = "") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String

        If Regione = -1 Then
            strOrdCli = "SELECT * FROM view_FatturatoClienteSintetico "
        Else
            strOrdCli = "SELECT * FROM view_FatturatoClienteSinteticoReg "
        End If

        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
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
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1

        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If

        Dim SWAnd As Boolean = False
        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            strOrdCli = strOrdCli & " WHERE Cod_Cliente >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Cliente <= '" & Controlla_Apice(myStrA.Trim) & "' "
            SWAnd = True
        End If
        'GIU180523 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        If txtDataDa.Trim <> "" Then
            If SWAnd = True Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
                SWAnd = True
            End If
            '---------
            strOrdCli = strOrdCli & "Data_Doc >= CONVERT(datetime, '" & txtDataDa.Trim & "', 103) "
        End If

        If txtDataA.Trim <> "" Then
            If SWAnd = True Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
                SWAnd = True
            End If
            '---------
            strOrdCli = strOrdCli & "Data_Doc <= CONVERT(datetime, '" & txtDataA.Trim & "', 103) "
        End If
        '---------------------
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        If Regione > 0 Then
            If SWAnd Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
            End If
            strOrdCli = strOrdCli & " Regione = " & Regione.ToString.Trim & " "
        End If

        strOrdCli = strOrdCli & " ORDER BY CONVERT(INT, Numero) "

        SqlDbSelectCmd.CommandText = strOrdCli

        'Riempio Dataset
        SqlAdap.Fill(dsFatturatoClienteFattura1.FatturatoClienteSintetico)

        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe


        Dim r_OrdPerCli As DSFatturatoClienteFattura.FatturatoClienteSinteticoRow
        For Each r_OrdPerCli In dsFatturatoClienteFattura1.FatturatoClienteSintetico
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsFatturatoClienteFattura1.AcceptChanges()

        Return True

    End Function

    'giu170212
    Public Function StampaValorizzaCMSMFor(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsValorizzaCMSMFor As DSMovMag, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, ByVal Data1 As String, ByVal Data2 As String, ByVal CMag As Integer) As Boolean ', Optional ByVal NomeCampoOrdinamento As String = "Codice_CoGe") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_FatturatoForCMSM "
        Dim passWhere As Boolean = False
        If CMag <> 0 Then
            strOrdCli = "SELECT * FROM view_FatturatoForCMSM WHERE CodiceMagazzino=" + CMag.ToString.Trim + " "
            passWhere = True
        End If
        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
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
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1

        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If

        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            If passWhere Then
                strOrdCli = strOrdCli & " AND Cod_Fornitore >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Fornitore <= '" & Controlla_Apice(myStrA.Trim) & "' "
            Else
                strOrdCli = strOrdCli & " WHERE Cod_Fornitore >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Fornitore <= '" & Controlla_Apice(myStrA.Trim) & "' "
                passWhere = True
            End If
        End If

        'alberto 11/05/2012
        If Data1.Trim <> "" Then
            If passWhere Then
                strOrdCli = strOrdCli & " AND Data_Doc >= CONVERT(datetime, '" & Data1.Trim & "', 103) "
            Else
                strOrdCli = strOrdCli & " WHERE Data_Doc >= CONVERT(datetime, '" & Data1.Trim & "', 103) "
                passWhere = True
            End If
        End If

        If Data2.Trim <> "" Then
            If passWhere Then
                strOrdCli = strOrdCli & " AND Data_Doc <= CONVERT(datetime, '" & Data2.Trim & "', 103) "
            Else
                strOrdCli = strOrdCli & " WHERE Data_Doc <= CONVERT(datetime, '" & Data2.Trim & "', 103) "
            End If
        End If
        '---------------------
        strOrdCli = strOrdCli & " ORDER BY Riga "
        'If NomeCampoOrdinamento.Trim <> "" Then
        '    strOrdCli = strOrdCli & " ORDER BY " & NomeCampoOrdinamento
        'End If
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsValorizzaCMSMFor.ValorizzaCMSMFor)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerFor As DSMovMag.ValorizzaCMSMForRow
        For Each r_OrdPerFor In dsValorizzaCMSMFor.ValorizzaCMSMFor
            r_OrdPerFor.AziendaReport = strNomeAz
            r_OrdPerFor.TitoloReport = TitoloReport
        Next
        dsValorizzaCMSMFor.AcceptChanges()
        Return True

    End Function

    Public Function StampaValorizzaCMSMForSintetico(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsValorizzaCMSMFor As DSMovMag, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, ByVal Data1 As String, ByVal Data2 As String, ByVal CMag As Integer) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""
        Dim SWhere As Boolean = False
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_FatturatoForCMSMSintetico "
        If CMag <> 0 Then
            strOrdCli = "SELECT * FROM view_FatturatoForCMSMSintetico WHERE CodiceMagazzino=" + CMag.ToString.Trim + " "
            SWhere = True
        End If
        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

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
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1

        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If

        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            If SWhere = True Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
                SWhere = True
            End If
            strOrdCli = strOrdCli & "Cod_Fornitore >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Fornitore <= '" & Controlla_Apice(myStrA.Trim) & "' "
            SWhere = True
        End If

        'alberto 11/05/2012
        If Data1.Trim <> "" Then
            'GIU220512
            If SWhere = True Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
                SWhere = True
            End If
            '---------
            strOrdCli = strOrdCli & "Data_Doc >= CONVERT(datetime, '" & Data1.Trim & "', 103) "
        End If

        If Data2.Trim <> "" Then
            'GIU220512
            If SWhere = True Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
                SWhere = True
            End If
            '---------
            strOrdCli = strOrdCli & "Data_Doc <= CONVERT(datetime, '" & Data2.Trim & "', 103) "
        End If
        '---------------------

        strOrdCli = strOrdCli & " ORDER BY CONVERT(INT, Numero) "
        'If NomeCampoOrdinamento.Trim <> "" Then
        '    strOrdCli = strOrdCli & " ORDER BY " & NomeCampoOrdinamento
        'End If

        SqlDbSelectCmd.CommandText = strOrdCli

        'Riempio Dataset
        SqlAdap.Fill(dsValorizzaCMSMFor.ValorizzaCMSMForSintetico)

        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe


        Dim r_OrdPerCli As DSMovMag.ValorizzaCMSMForSinteticoRow
        For Each r_OrdPerCli In dsValorizzaCMSMFor.ValorizzaCMSMForSintetico
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsValorizzaCMSMFor.AcceptChanges()

        Return True

    End Function

    'GIU060312 Differenze Fatture/N.C. con DDT
    Public Function StampaDiffFTDTSintetico(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsFatturatoClienteFattura1 As DSFatturatoClienteFattura, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String) As Boolean ', Optional ByVal NomeCampoOrdinamento As String = "Codice_CoGe") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_DifferenzaFatturatoByRefInt "
        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
        'giu190617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If
        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            strOrdCli = strOrdCli & " WHERE Cod_Cliente >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Cliente <= '" & Controlla_Apice(myStrA.Trim) & "' "
        End If
        strOrdCli = strOrdCli & " ORDER BY CONVERT(INT, Numero) "
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsFatturatoClienteFattura1.DifferenzaFatturatoByRefInt)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As DSFatturatoClienteFattura.DifferenzaFatturatoByRefIntRow
        For Each r_OrdPerCli In dsFatturatoClienteFattura1.DifferenzaFatturatoByRefInt
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsFatturatoClienteFattura1.AcceptChanges()

        Return True

    End Function
    'GIU151012 DT fatturati piu volte su fatture diverse?????
    Public Function StampaDTFTDoppiSintetico(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsFatturatoClienteFattura1 As DSFatturatoClienteFattura, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String) As Boolean ', Optional ByVal NomeCampoOrdinamento As String = "Codice_CoGe") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_DifferenzaFatturatoByRefIntDTDoppi "
        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
        'giu190617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If
        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            strOrdCli = strOrdCli & " WHERE Cod_Cliente >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Cliente <= '" & Controlla_Apice(myStrA.Trim) & "' "
        End If
        strOrdCli = strOrdCli & " ORDER BY CONVERT(INT, Numero) " 'NumeroRefInt
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsFatturatoClienteFattura1.DifferenzaFatturatoByRefInt)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        '1 FASE controllo numero fatture doppie
        Dim myNumero As Integer = -1
        Dim r_OrdPerCli As DSFatturatoClienteFattura.DifferenzaFatturatoByRefIntRow
        Dim r_Doppi As DSFatturatoClienteFattura.DifferenzaFatturatoByRefIntRow
        For Each r_OrdPerCli In dsFatturatoClienteFattura1.DifferenzaFatturatoByRefInt.Select("", "Numero")
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
            If myNumero = -1 Then 'PRIMA VOLTA
                myNumero = r_OrdPerCli.Numero
            ElseIf myNumero = r_OrdPerCli.Numero Then
                r_OrdPerCli.SWDoppio = True
                For Each r_Doppi In dsFatturatoClienteFattura1.DifferenzaFatturatoByRefInt.Select("Numero=" & myNumero & " AND SWDoppio=0", "Numero")
                    r_Doppi.SWDoppio = True
                Next
            Else
                myNumero = r_OrdPerCli.Numero
            End If
        Next
        dsFatturatoClienteFattura1.AcceptChanges()
        '2 FASE controllo numero DT doppi
        myNumero = -1
        For Each r_OrdPerCli In dsFatturatoClienteFattura1.DifferenzaFatturatoByRefInt.Select("", "NumeroRefInt")
            If myNumero = -1 Then 'PRIMA VOLTA
                myNumero = r_OrdPerCli.NumeroRefInt
            ElseIf myNumero = r_OrdPerCli.NumeroRefInt Then
                r_OrdPerCli.SWDoppio = True
                For Each r_Doppi In dsFatturatoClienteFattura1.DifferenzaFatturatoByRefInt.Select("NumeroRefInt=" & myNumero & " AND SWDoppio=0", "Numero")
                    r_Doppi.SWDoppio = True
                Next
            Else
                myNumero = r_OrdPerCli.NumeroRefInt
            End If
        Next
        dsFatturatoClienteFattura1.AcceptChanges()
        'FASE 3 ELIMINO TUTTI I NON DOPPI
        For Each r_Doppi In dsFatturatoClienteFattura1.DifferenzaFatturatoByRefInt.Select("SWDoppio=0", "Numero")
            r_Doppi.Delete()
        Next
        dsFatturatoClienteFattura1.AcceptChanges()
        Return True

    End Function
    'GIU161012 FT/NC Cod_Causale/Fatturabile = FALSE errori CAUSALE non valida
    Public Function FTNCCCausErrSintetico(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsFatturatoClienteFattura1 As DSFatturatoClienteFattura, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String) As Boolean ', Optional ByVal NomeCampoOrdinamento As String = "Codice_CoGe") As Boolean
        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * "
        strOrdCli += ", CausMag.Descrizione AS DesCausale, CausMag.Fatturabile "
        strOrdCli += " FROM view_FatturatoClienteSintetico INNER JOIN "
        strOrdCli += "CausMag ON view_FatturatoClienteSintetico.Cod_Causale = CausMag.Codice"

        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
        'giu190617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If
        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            strOrdCli = strOrdCli & " WHERE Cod_Cliente >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Cliente <= '" & Controlla_Apice(myStrA.Trim) & "' " &
                        " AND ISNULL(Fatturabile, 0) = 0"
        Else
            strOrdCli = strOrdCli & " WHERE ISNULL(Fatturabile, 0) = 0"
        End If
        strOrdCli = strOrdCli & " ORDER BY CONVERT(INT, Numero) " 'NumeroRefInt
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsFatturatoClienteFattura1.FatturatoClienteSintetico)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As DSFatturatoClienteFattura.FatturatoClienteSinteticoRow
        For Each r_OrdPerCli In dsFatturatoClienteFattura1.FatturatoClienteSintetico
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsFatturatoClienteFattura1.AcceptChanges()
        Return True

    End Function

    Public Function StampaFatturatoClienteFatturaAG(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsFatturatoClienteFattura1 As DSFatturatoClienteFattura, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, ByVal CodAgente As Integer) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_FatturatoClienteFatturaAG "
        Dim Where As Boolean = False

        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

        'giu190617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1

        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If

        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            strOrdCli = strOrdCli & " WHERE Cod_Cliente >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Cliente <= '" & Controlla_Apice(myStrA.Trim) & "' "
            Where = True
        End If

        If CodAgente <> -1 Then
            If Where Then
                strOrdCli = strOrdCli & "AND Agente = " & CodAgente.ToString
            Else
                strOrdCli = strOrdCli & "WHERE Agente = " & CodAgente.ToString
            End If
        End If

        strOrdCli = strOrdCli & " ORDER BY Riga "

        SqlDbSelectCmd.CommandText = strOrdCli

        'Riempio Dataset
        SqlAdap.Fill(dsFatturatoClienteFattura1.FatturatoClienteFattura)

        'Ciclo che imposta AziendaReport e Titolo Report per tutte le righe

        Dim r_OrdPerCli As DSFatturatoClienteFattura.FatturatoClienteFatturaRow
        For Each r_OrdPerCli In dsFatturatoClienteFattura1.FatturatoClienteFattura
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsFatturatoClienteFattura1.AcceptChanges()

        Return True

    End Function

    Public Function StampaFatturatoClienteSinteticoAG(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsFatturatoClienteFattura1 As DSFatturatoClienteFattura, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, ByVal CodAgente As Integer, Optional ByVal Regione As Integer = -1) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_FatturatoClienteSinteticoAG "

        SqlConn1 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
        'giu190617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        SqlDbSelectCmd.CommandTimeout = myTimeOUT
        '---------------------------
        'Riempio DSOrdinatoCliente
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1

        If myStrA.Trim = "" Then
            myStrA = "ZZZZZZZZZZZZZZZZ"
        End If

        Dim SWAnd As Boolean = False
        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            strOrdCli = strOrdCli & " WHERE Cod_Cliente >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Cliente <= '" & Controlla_Apice(myStrA.Trim) & "' "
            SWAnd = True
        End If
        If Regione > 0 Then
            If SWAnd Then
                strOrdCli += " AND "
            Else
                strOrdCli += " WHERE "
                SWAnd = True
            End If
            strOrdCli = strOrdCli & " Regione = " & Regione.ToString.Trim & " "
        End If

        If CodAgente <> -1 Then
            If SWAnd Then
                strOrdCli = strOrdCli & "AND Agente = " & CodAgente.ToString
            Else
                strOrdCli = strOrdCli & "WHERE Agente = " & CodAgente.ToString
            End If
        End If

        strOrdCli = strOrdCli & " ORDER BY CONVERT(INT, Numero) "
        SqlDbSelectCmd.CommandText = strOrdCli

        'Riempio Dataset
        SqlAdap.Fill(dsFatturatoClienteFattura1.FatturatoClienteSintetico)

        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As DSFatturatoClienteFattura.FatturatoClienteSinteticoRow
        For Each r_OrdPerCli In dsFatturatoClienteFattura1.FatturatoClienteSintetico
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsFatturatoClienteFattura1.AcceptChanges()

        Return True

    End Function
End Class
