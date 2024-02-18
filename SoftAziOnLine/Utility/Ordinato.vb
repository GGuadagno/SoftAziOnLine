Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Public Class Ordinato
    Inherits System.Web.UI.UserControl
#Region " ORDINATO CLIENTI"
    Public Function StampaOrdinatoCliente(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsOrdinatoPerCliente1 As DSOrdinatoPerCliente, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, Optional ByVal NomeCampoOrdinamento As String = "Codice_CoGe") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM OrdinatoPerCliente"

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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

        If NomeCampoOrdinamento.Trim = "Codice_CoGe" Then
            If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
                strOrdCli = strOrdCli & " WHERE Codice_CoGe >= '" & Controlla_Apice(myStrDa.Trim) &
                            "' AND  Codice_CoGe <= '" & Controlla_Apice(myStrA.Trim) & "' "
            End If
        ElseIf NomeCampoOrdinamento.Trim = "Rag_Soc" Then
            If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
                strOrdCli = strOrdCli & " WHERE Codice_CoGe >= '" & Controlla_Apice(myStrDa.Trim) &
                            "' AND  Codice_CoGe <= '" & Controlla_Apice(myStrA.Trim) & "' "
            End If
        End If
        'If NomeCampoOrdinamento.Trim <> "" Then
        '    strOrdCli = strOrdCli & " ORDER BY " & NomeCampoOrdinamento
        'End If
        ' ''strOrdCli = strOrdCli & " ORDER BY Cod_Articolo "
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoPerCliente1.OrdinatoPerCliente)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As DSOrdinatoPerCliente.OrdinatoPerClienteRow
        For Each r_OrdPerCli In dsOrdinatoPerCliente1.OrdinatoPerCliente
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsOrdinatoPerCliente1.AcceptChanges()

        Return True

    End Function

    Public Function StampaOrdinatoClienteOrdine(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsOrdinatoClienteOrdine1 As DSOrdinatoClienteOrdine, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String) As Boolean ', Optional ByVal NomeCampoOrdinamento As String = "Codice_CoGe") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_OrdinatoClienteOrdine "

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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

        'If NomeCampoOrdinamento.Trim = "Codice_CoGe" Then
        '    If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
        '        strOrdCli = strOrdCli & " WHERE Codice_CoGe >= '" & Controlla_Apice(myStrDa.Trim) & _
        '                    "' AND  Codice_CoGe <= '" & Controlla_Apice(myStrA.Trim) & "' "
        '    End If
        'ElseIf NomeCampoOrdinamento.Trim = "Rag_Soc" Then
        '    If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
        '        strOrdCli = strOrdCli & " WHERE Codice_CoGe >= '" & Controlla_Apice(myStrDa.Trim) & _
        '                    "' AND  Codice_CoGe <= '" & Controlla_Apice(myStrA.Trim) & "' "
        '    End If
        'End If
        If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
            strOrdCli = strOrdCli & " WHERE Cod_Cliente >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Cliente <= '" & Controlla_Apice(myStrA.Trim) & "' "
        End If
        ' ''strOrdCli = strOrdCli & " ORDER BY Riga "
        'If NomeCampoOrdinamento.Trim <> "" Then
        '    strOrdCli = strOrdCli & " ORDER BY " & NomeCampoOrdinamento
        'End If
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoClienteOrdine1.OrdinatoClienteOrdine)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As DSOrdinatoClienteOrdine.OrdinatoClienteOrdineRow
        For Each r_OrdPerCli In dsOrdinatoClienteOrdine1.OrdinatoClienteOrdine
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsOrdinatoClienteOrdine1.AcceptChanges()

        Return True

    End Function

    'alb12122012
    Public Function StampaStatOrdinatoClienteOrdine(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsStatOrdinatoClienteOrdine1 As dsStatOrdinatoClienteOrdine, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, ByVal Regione As String, ByVal Data1 As String, ByVal Data2 As String, ByVal Provincia As String, ByVal Agente As String, ByVal CCliente As String) As Boolean
        'alb080213 aggiunti provincia e agente
        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""
        Dim Where As Boolean = False

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_StatOrdinatoClienteOrdine "

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
        SqlAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand

        'alb180213
        Dim VisAgente As Boolean = False
        Dim VisProvincia As Boolean = False
        Dim VisRegione As Boolean = False
        'alb180213 END
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
            Where = True
            strOrdCli = strOrdCli & " WHERE Cod_Articolo >= '" & Controlla_Apice(myStrDa.Trim) &
                        "' AND Cod_Articolo <= '" & Controlla_Apice(myStrA.Trim) & "' "
        End If
        If CCliente <> "" Then
            If Where Then
                strOrdCli = strOrdCli & "AND "
            Else
                strOrdCli = strOrdCli & " WHERE "
                Where = True
            End If
            strOrdCli = strOrdCli & "Cod_Cliente = '" & Controlla_Apice(CCliente.Trim) & "' "
        End If
        If Data1 <> "" Then
            If Where Then
                strOrdCli = strOrdCli & "AND "
            Else
                strOrdCli = strOrdCli & " WHERE "
                Where = True
            End If
            strOrdCli = strOrdCli & "Data_Doc >= CONVERT(datetime, '" & Data1 & "', 103) "
        End If

        If Data2 <> "" Then
            If Where Then
                strOrdCli = strOrdCli & "AND "
            Else
                strOrdCli = strOrdCli & " WHERE "
                Where = True
            End If
            strOrdCli = strOrdCli & "Data_Doc <= CONVERT(datetime, '" & Data2 & "', 103) "
        End If

        If Regione.Trim <> "" Then
            If Where Then
                strOrdCli = strOrdCli & "AND "
            Else
                strOrdCli = strOrdCli & " WHERE "
                Where = True
            End If
            strOrdCli = strOrdCli & "Regione = " & Regione & " "
            VisRegione = True 'alb180213
        End If

        'alb080213
        If Provincia.Trim <> "" Then
            If Where Then
                strOrdCli = strOrdCli & "AND "
            Else
                strOrdCli = strOrdCli & "WHERE "
                Where = True
            End If
            strOrdCli = strOrdCli & "CodProvincia = '" & Provincia & "' "
            VisProvincia = True 'alb180213
        End If

        If Agente.Trim <> "" Then
            If Where Then
                strOrdCli = strOrdCli & "AND "
            Else
                strOrdCli = strOrdCli & "WHERE "
                Where = True
            End If
            strOrdCli = strOrdCli & "Agente = " & Agente & " "
            VisAgente = True 'alb180213
        End If
        'alb080213 END
        ' ''strOrdCli = strOrdCli & " ORDER BY Riga "
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsStatOrdinatoClienteOrdine1.StatOrdinatoClienteOrdine)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As dsStatOrdinatoClienteOrdine.StatOrdinatoClienteOrdineRow
        For Each r_OrdPerCli In dsStatOrdinatoClienteOrdine1.StatOrdinatoClienteOrdine
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
            'alb180213
            r_OrdPerCli.VisAgente = VisAgente
            r_OrdPerCli.VisRegione = VisRegione
            r_OrdPerCli.VisProvincia = VisProvincia
            'alb180213 END
        Next
        dsStatOrdinatoClienteOrdine1.AcceptChanges()

        Return True

    End Function
    '-----------

    Public Function StampaOrdinatoArticolo(ByVal TipoReport As Integer, ByVal Cod1 As String, ByVal Cod2 As String, ByVal Desc1 As String, ByVal Desc2 As String, ByVal txtData1 As String, ByVal txtData2 As String, ByVal _Esercizio As String, ByVal Ordinamento As Integer, ByRef DSOrdinatoArticolo1 As DSOrdinatoArticolo, ByRef ObjReport As Object, ByRef Errore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapOrdArtData As SqlDataAdapter
        Dim SqlDbSelectCmdOrdArtData As SqlCommand
        Try
            DSOrdinatoArticolo1.Clear()
            If TipoReport = 2 Then 'ORDINATO PER ARTICOLO DATA
                SqlConnOrd = New SqlConnection
                SqlAdapOrdArtData = New SqlDataAdapter
                SqlDbSelectCmdOrdArtData = New SqlCommand
                'giu190617
                Dim strValore As String = ""
                Dim strErrore As String = ""
                Dim myTimeOUT As Long = 5000
                If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                    If IsNumeric(strValore.Trim) Then
                        If CLng(strValore.Trim) > myTimeOUT Then
                            myTimeOUT = CLng(strValore.Trim)
                        End If
                    End If
                End If
                SqlDbSelectCmdOrdArtData.CommandTimeout = myTimeOUT
                '---------------------------
                SqlAdapOrdArtData.SelectCommand = SqlDbSelectCmdOrdArtData
                SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
                SqlDbSelectCmdOrdArtData.CommandText = "get_OrdinatoPerArticoloData"
                SqlDbSelectCmdOrdArtData.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbSelectCmdOrdArtData.Connection = SqlConnOrd
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Desc1", System.Data.SqlDbType.NVarChar, 150, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Desc2", System.Data.SqlDbType.NVarChar, 150, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data1", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data2", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

                '==============CARICAMENTO DATASET ===================
                SqlDbSelectCmdOrdArtData.Parameters.Item("@Cod1").Value = IIf(Cod1 = "", Cod1, Controlla_Apice(Cod1))
                SqlDbSelectCmdOrdArtData.Parameters.Item("@Cod2").Value = IIf(Cod2 = "", Cod2, Controlla_Apice(Cod2))
                SqlDbSelectCmdOrdArtData.Parameters.Item("@Desc1").Value = IIf(Desc1 = "", Desc1, Controlla_Apice(Desc1))
                SqlDbSelectCmdOrdArtData.Parameters.Item("@Desc2").Value = IIf(Desc2 = "", Desc2, Controlla_Apice(Desc2))
                If txtData1 = "" Then
                    SqlDbSelectCmdOrdArtData.Parameters.Item("@Data1").Value = ""
                Else
                    SqlDbSelectCmdOrdArtData.Parameters.Item("@Data1").Value = Format(DateTime.Parse(txtData1), "dd/MM/yyyy")
                End If
                If txtData2 = "" Then
                    SqlDbSelectCmdOrdArtData.Parameters.Item("@Data2").Value = ""
                Else
                    SqlDbSelectCmdOrdArtData.Parameters.Item("@Data2").Value = Format(DateTime.Parse(txtData2), "dd/MM/yyyy")
                End If
                SqlDbSelectCmdOrdArtData.Parameters.Item("@Ordinamento").Value = Ordinamento
                SqlAdapOrdArtData.Fill(DSOrdinatoArticolo1.OrdinatoPerArticoloData)

                If DSOrdinatoArticolo1.OrdinatoPerArticoloData.Count > 0 Then
                    For Each row As DSOrdinatoArticolo.OrdinatoPerArticoloDataRow In DSOrdinatoArticolo1.OrdinatoPerArticoloData.Rows
                        row.Azienda = _Esercizio
                        row.TitoloReport = "Ordinato per articolo"
                        row.ConsegnePreviste = "Consegne previste" & (IIf(txtData1 <> "", " dal " & txtData1, "")) & (IIf(txtData2 <> "", " al " & txtData2, ""))
                    Next
                End If

            ElseIf TipoReport = 1 Then 'ORDINATO PER ARTICOLO
                Dim SqlAdapOrdArt As SqlDataAdapter
                Dim SqlDbSelectCmdOrdArt As SqlCommand
                SqlConnOrd = New SqlConnection
                SqlAdapOrdArt = New SqlDataAdapter
                SqlDbSelectCmdOrdArt = New SqlCommand

                'giu190617
                Dim strValore As String = ""
                Dim strErrore As String = ""
                Dim myTimeOUT As Long = 5000
                If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                    If IsNumeric(strValore.Trim) Then
                        If CLng(strValore.Trim) > myTimeOUT Then
                            myTimeOUT = CLng(strValore.Trim)
                        End If
                    End If
                End If
                SqlDbSelectCmdOrdArt.CommandTimeout = myTimeOUT
                '---------------------------
                SqlAdapOrdArt.SelectCommand = SqlDbSelectCmdOrdArt
                SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
                SqlDbSelectCmdOrdArt.CommandText = "get_OrdinatoPerArticolo"
                SqlDbSelectCmdOrdArt.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbSelectCmdOrdArt.Connection = SqlConnOrd
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Desc1", System.Data.SqlDbType.NVarChar, 150, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Desc2", System.Data.SqlDbType.NVarChar, 150, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

                '==============CARICAMENTO DATASET ===================
                SqlDbSelectCmdOrdArt.Parameters.Item("@Cod1").Value = Controlla_Apice(Cod1)
                SqlDbSelectCmdOrdArt.Parameters.Item("@Cod2").Value = Controlla_Apice(Cod2)
                SqlDbSelectCmdOrdArt.Parameters.Item("@Desc1").Value = Controlla_Apice(Desc1)
                SqlDbSelectCmdOrdArt.Parameters.Item("@Desc2").Value = Controlla_Apice(Desc2)
                SqlDbSelectCmdOrdArt.Parameters.Item("@Ordinamento").Value = Ordinamento
                SqlAdapOrdArt.Fill(DSOrdinatoArticolo1.OrdinatoPerArticolo)

                If DSOrdinatoArticolo1.OrdinatoPerArticolo.Count > 0 Then
                    For Each row As DSOrdinatoArticolo.OrdinatoPerArticoloRow In DSOrdinatoArticolo1.OrdinatoPerArticolo.Rows
                        row.Azienda = _Esercizio
                        row.TitoloReport = "Ordinato per articolo"
                    Next
                End If
            End If

        Catch ex As Exception
            Errore = ex.Message & " - Stampa Ordinato per Articolo"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'rOB151211

    Public Function StampaOrdinatoArtCli(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsOrdinatoArtCli1 As DSOrdinatoArtCli, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrWhere As String, ByVal myTipoStampa As String) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = ""
        If myTipoStampa = "F" Then
            strOrdCli = "SELECT * FROM view_OrdArtCliFor WHERE " & myStrWhere
        Else
            strOrdCli = "SELECT * FROM view_OrdArtCli WHERE " & myStrWhere
        End If
        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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
        'Riempio ds
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        SqlDbSelectCmd.CommandText = strOrdCli

        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoArtCli1.OrdArtCli)
        'giu281221
        If myTipoStampa <> "F" Then
            Passo = 1
            Dim ObjDB As New DataBaseUtility
            strSQL = "Select Codice, LTRIM(RTRIM(Nome)) AS Nome From Operatori"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, dsOrdinatoArtCli1, "Operatori")
            Catch ex As Exception
                Errore = ex.Message & " - Inserimento dati nella Tabella Operatori. Passo: " & Passo.ToString
                Return False
                Exit Function
            End Try
            ObjDB = Nothing
        End If
        '---------
        'Ciclo che aÃ¬imposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdArtCli As DSOrdinatoArtCli.OrdArtCliRow
        'giu281221
        Dim X As Integer = 0
        Dim rsOperatori As DataRow
        Dim myIniziali As String = ""
        For Each r_OrdArtCli In dsOrdinatoArtCli1.OrdArtCli
            r_OrdArtCli.AziendaReport = strNomeAz
            r_OrdArtCli.TitoloReport = TitoloReport
            'giu281221
            If myTipoStampa <> "F" Then
                strSQL = ""
                If Not IsDBNull(r_OrdArtCli!ModificatoDa) Then
                    strSQL = r_OrdArtCli!ModificatoDa.ToString.Trim
                    X = InStr(strSQL, " ")
                    If X > 0 Then
                        strSQL = Mid(strSQL, 1, X - 1)
                    End If
                ElseIf Not IsDBNull(r_OrdArtCli!InseritoDa) Then
                    strSQL = r_OrdArtCli!InseritoDa.ToString.Trim
                    X = InStr(strSQL, " ")
                    If X > 0 Then
                        strSQL = Mid(strSQL, 1, X - 1)
                    End If
                End If
                If strSQL <> "" Then
                    myIniziali = ""
                    For Each rsOperatori In dsOrdinatoArtCli1.Tables("Operatori").Select("Nome like '%" & strSQL.Trim & "%'")
                        myIniziali = rsOperatori!Codice.ToString.Trim
                    Next
                    If myIniziali.Trim <> "" Then
                        strSQL = myIniziali.Trim
                    Else
                        strSQL = Mid(strSQL, 1, 2)
                    End If
                Else
                    strSQL = "XX"
                End If
                r_OrdArtCli!Iniziali = strSQL
                '---------
            End If
            '---------
        Next
        dsOrdinatoArtCli1.AcceptChanges()
        Return True

    End Function

    Public Function StampaListaCarico(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsListaCarico1 As DSListaCarico, ByRef ObjReport As Object, ByRef Errore As String, Optional ByVal IDDocumenti As Long = -1, Optional ByVal NumeroSpedizione As Long = -1) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand


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
        'Riempio ds
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1

        dsListaCarico1.Clear()
        Dim SqlStr As String = ""
        If IDDocumenti > 0 Then
            'Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCarico
            SqlDbSelectCmd.CommandText = "SELECT * FROM view_ListaCarico WHERE IDDocumenti = " & IDDocumenti
            'Riempio Dataset
            SqlAdap.Fill(dsListaCarico1.view_ListaCarico)

            SqlDbSelectCmd.CommandText = "SELECT * FROM DocumentiDLotti WHERE IDDocumenti = " & IDDocumenti
            'Riempio Dataset
            SqlAdap.Fill(dsListaCarico1.DocumentiDLotti)

            Dim r_ListaCarico As DSListaCarico.view_ListaCaricoRow
            For Each r_ListaCarico In dsListaCarico1.view_ListaCarico
                r_ListaCarico.AziendaReport = strNomeAz
                r_ListaCarico.TitoloReport = TitoloReport
            Next
            dsListaCarico1.AcceptChanges()
        ElseIf NumeroSpedizione > 0 Then
            'Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione
            SqlDbSelectCmd.CommandText = "SELECT * FROM view_ListaCaricoSpedizione WHERE NumeroSpedizione = " & NumeroSpedizione
            'Riempio Dataset
            SqlAdap.Fill(dsListaCarico1.view_ListaCaricoSpedizione)

            'Prendo i Lotti di tutti i documenti inclusi nella spedizione
            SqlStr = "SELECT * FROM DocumentiDLotti WHERE IDDocumenti In ("
            SqlStr = SqlStr & " SELECT SpedizioniDett.IDDocumenti"
            SqlStr = SqlStr & " FROM Spedizioni INNER JOIN"
            SqlStr = SqlStr & " SpedizioniDett ON Spedizioni.ID = SpedizioniDett.IDSpedizione"
            SqlStr = SqlStr & " WHERE (Spedizioni.NumeroSpedizione = " & NumeroSpedizione & ")"
            SqlStr = SqlStr & " )"
            SqlDbSelectCmd.CommandText = SqlStr
            'Riempio Dataset
            SqlAdap.Fill(dsListaCarico1.DocumentiDLotti)

            Dim r_ListaCarico As DSListaCarico.view_ListaCaricoSpedizioneRow
            For Each r_ListaCarico In dsListaCarico1.view_ListaCaricoSpedizione
                r_ListaCarico.AziendaReport = strNomeAz
                r_ListaCarico.TitoloReport = TitoloReport
            Next
            dsListaCarico1.AcceptChanges()
        ElseIf IDDocumenti < 0 And NumeroSpedizione < 0 Then 'giu270112 STAMPO TUTTE LE SPEDIZIONI
            SqlDbSelectCmd.CommandText = "SELECT * FROM view_ListaCaricoSpedizione"
            'Riempio Dataset
            SqlAdap.Fill(dsListaCarico1.view_ListaCaricoSpedizione)

            'Prendo i Lotti di tutti i documenti inclusi nella spedizione
            SqlStr = "SELECT * FROM DocumentiDLotti WHERE IDDocumenti In ("
            SqlStr = SqlStr & " SELECT SpedizioniDett.IDDocumenti"
            SqlStr = SqlStr & " FROM Spedizioni INNER JOIN"
            SqlStr = SqlStr & " SpedizioniDett ON Spedizioni.ID = SpedizioniDett.IDSpedizione )"
            SqlDbSelectCmd.CommandText = SqlStr
            'Riempio Dataset
            SqlAdap.Fill(dsListaCarico1.DocumentiDLotti)

            Dim r_ListaCarico As DSListaCarico.view_ListaCaricoSpedizioneRow
            For Each r_ListaCarico In dsListaCarico1.view_ListaCaricoSpedizione
                r_ListaCarico.AziendaReport = strNomeAz
                r_ListaCarico.TitoloReport = TitoloReport
            Next
            dsListaCarico1.AcceptChanges()
        Else
            Return False
        End If

        Return True

    End Function
    'alberto 11/05/2012
    Public Function StampaOrdinatoClienteAg(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsOrdinatoPerCliente1 As DSOrdinatoPerCliente, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String, ByVal Agente As Integer, Optional ByVal NomeCampoOrdinamento As String = "Codice_CoGe") As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM OrdinatoPerClienteAg"
        Dim OkWhere As Boolean = False

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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

        If NomeCampoOrdinamento.Trim = "Codice_CoGe" Then
            If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
                strOrdCli = strOrdCli & " WHERE Codice_CoGe >= '" & Controlla_Apice(myStrDa.Trim) &
                            "' AND  Codice_CoGe <= '" & Controlla_Apice(myStrA.Trim) & "' "
                OkWhere = True
            End If
        ElseIf NomeCampoOrdinamento.Trim = "Rag_Soc" Then
            If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
                strOrdCli = strOrdCli & " WHERE Codice_CoGe >= '" & Controlla_Apice(myStrDa.Trim) &
                            "' AND  Codice_CoGe <= '" & Controlla_Apice(myStrA.Trim) & "' "
                OkWhere = True
            End If
        End If

        If Agente = 0 Then
            If OkWhere Then
                strOrdCli = strOrdCli & " AND ISNULL(Agente, 0) = 0"
            Else
                strOrdCli = strOrdCli & " WHERE ISNULL(Agente, 0) = 0"
                OkWhere = True
            End If
        ElseIf Agente > 0 Then
            If OkWhere Then
                strOrdCli = strOrdCli & " AND Agente = " & Agente
            Else
                strOrdCli = strOrdCli & " WHERE Agente = " & Agente
                OkWhere = True
            End If
        End If
        ' ''strOrdCli = strOrdCli & " ORDER BY Cod_Articolo "
        SqlDbSelectCmd.CommandText = strOrdCli

        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoPerCliente1.OrdinatoPerCliente)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As DSOrdinatoPerCliente.OrdinatoPerClienteRow
        For Each r_OrdPerCli In dsOrdinatoPerCliente1.OrdinatoPerCliente
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsOrdinatoPerCliente1.AcceptChanges()

        Return True

    End Function
    Public Function StampaOrdinatoArticoloAG(ByVal TipoReport As Integer, ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal Desc1 As String, ByVal Desc2 As String, ByVal txtData1 As String, ByVal txtData2 As String, ByVal _Esercizio As String, ByVal Ordinamento As Integer, ByRef DSOrdinatoArticolo1 As DSOrdinatoArticolo, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodAgente As Integer) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapOrdArtData As SqlDataAdapter
        Dim SqlDbSelectCmdOrdArtData As SqlCommand
        Try
            DSOrdinatoArticolo1.Clear()
            If TipoReport = TIPOSTAMPAORDINATO.OrdinatoArticoloDataAg Then 'ORDINATO PER ARTICOLO DATA
                SqlConnOrd = New SqlConnection
                SqlAdapOrdArtData = New SqlDataAdapter
                SqlDbSelectCmdOrdArtData = New SqlCommand
                'giu190617
                Dim strValore As String = ""
                Dim strErrore As String = ""
                Dim myTimeOUT As Long = 5000
                If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                    If IsNumeric(strValore.Trim) Then
                        If CLng(strValore.Trim) > myTimeOUT Then
                            myTimeOUT = CLng(strValore.Trim)
                        End If
                    End If
                End If
                SqlDbSelectCmdOrdArtData.CommandTimeout = myTimeOUT
                '---------------------------
                SqlAdapOrdArtData.SelectCommand = SqlDbSelectCmdOrdArtData
                SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
                SqlDbSelectCmdOrdArtData.CommandText = "get_OrdinatoPerArticoloDataAG"
                SqlDbSelectCmdOrdArtData.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbSelectCmdOrdArtData.Connection = SqlConnOrd
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodArt1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodArt2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Desc1", System.Data.SqlDbType.NVarChar, 150, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Desc2", System.Data.SqlDbType.NVarChar, 150, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data1", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data2", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArtData.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodAgente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

                '==============CARICAMENTO DATASET ===================
                SqlDbSelectCmdOrdArtData.Parameters.Item("@CodArt1").Value = IIf(CodArt1 = "", CodArt1, Controlla_Apice(CodArt1))
                SqlDbSelectCmdOrdArtData.Parameters.Item("@CodArt2").Value = IIf(CodArt2 = "", CodArt2, Controlla_Apice(CodArt2))
                SqlDbSelectCmdOrdArtData.Parameters.Item("@Desc1").Value = IIf(Desc1 = "", Desc1, Controlla_Apice(Desc1))
                SqlDbSelectCmdOrdArtData.Parameters.Item("@Desc2").Value = IIf(Desc2 = "", Desc2, Controlla_Apice(Desc2))
                SqlDbSelectCmdOrdArtData.Parameters.Item("@CodAgente").Value = CodAgente

                If txtData1 = "" Then
                    SqlDbSelectCmdOrdArtData.Parameters.Item("@Data1").Value = ""
                Else
                    SqlDbSelectCmdOrdArtData.Parameters.Item("@Data1").Value = Format(DateTime.Parse(txtData1), "dd/MM/yyyy")
                End If
                If txtData2 = "" Then
                    SqlDbSelectCmdOrdArtData.Parameters.Item("@Data2").Value = ""
                Else
                    SqlDbSelectCmdOrdArtData.Parameters.Item("@Data2").Value = Format(DateTime.Parse(txtData2), "dd/MM/yyyy")
                End If
                SqlDbSelectCmdOrdArtData.Parameters.Item("@Ordinamento").Value = Ordinamento
                SqlAdapOrdArtData.Fill(DSOrdinatoArticolo1.OrdinatoPerArticoloData)

                If DSOrdinatoArticolo1.OrdinatoPerArticoloData.Count > 0 Then
                    For Each row As DSOrdinatoArticolo.OrdinatoPerArticoloDataRow In DSOrdinatoArticolo1.OrdinatoPerArticoloData.Rows
                        row.Azienda = _Esercizio
                        row.TitoloReport = "Ordinato per articolo per agente"
                        row.ConsegnePreviste = "Consegne previste" & (IIf(txtData1 <> "", " dal " & txtData1, "")) & (IIf(txtData2 <> "", " al " & txtData2, ""))
                    Next
                End If

            ElseIf TipoReport = TIPOSTAMPAORDINATO.OrdinatoArticoloAg Then 'ORDINATO PER ARTICOLO
                Dim SqlAdapOrdArt As SqlDataAdapter
                Dim SqlDbSelectCmdOrdArt As SqlCommand
                SqlConnOrd = New SqlConnection
                SqlAdapOrdArt = New SqlDataAdapter
                SqlDbSelectCmdOrdArt = New SqlCommand
                'giu190617
                Dim strValore As String = ""
                Dim strErrore As String = ""
                Dim myTimeOUT As Long = 5000
                If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                    If IsNumeric(strValore.Trim) Then
                        If CLng(strValore.Trim) > myTimeOUT Then
                            myTimeOUT = CLng(strValore.Trim)
                        End If
                    End If
                End If
                SqlDbSelectCmdOrdArt.CommandTimeout = myTimeOUT
                '---------------------------
                SqlAdapOrdArt.SelectCommand = SqlDbSelectCmdOrdArt
                SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
                SqlDbSelectCmdOrdArt.CommandText = "get_OrdinatoPerArticoloAG"
                SqlDbSelectCmdOrdArt.CommandType = System.Data.CommandType.StoredProcedure
                SqlDbSelectCmdOrdArt.Connection = SqlConnOrd
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodArt1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodArt2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Desc1", System.Data.SqlDbType.NVarChar, 150, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Desc2", System.Data.SqlDbType.NVarChar, 150, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                SqlDbSelectCmdOrdArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodAgente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

                '==============CARICAMENTO DATASET ===================
                SqlDbSelectCmdOrdArt.Parameters.Item("@CodArt1").Value = Controlla_Apice(CodArt1)
                SqlDbSelectCmdOrdArt.Parameters.Item("@CodArt2").Value = Controlla_Apice(CodArt2)
                SqlDbSelectCmdOrdArt.Parameters.Item("@Desc1").Value = Controlla_Apice(Desc1)
                SqlDbSelectCmdOrdArt.Parameters.Item("@Desc2").Value = Controlla_Apice(Desc2)
                SqlDbSelectCmdOrdArt.Parameters.Item("@Ordinamento").Value = Ordinamento
                SqlDbSelectCmdOrdArt.Parameters.Item("@CodAgente").Value = CodAgente
                SqlAdapOrdArt.Fill(DSOrdinatoArticolo1.OrdinatoPerArticolo)

                If DSOrdinatoArticolo1.OrdinatoPerArticolo.Count > 0 Then
                    For Each row As DSOrdinatoArticolo.OrdinatoPerArticoloRow In DSOrdinatoArticolo1.OrdinatoPerArticolo.Rows
                        row.Azienda = _Esercizio
                        row.TitoloReport = "Ordinato per articolo per agente"
                    Next
                End If
            End If

        Catch ex As Exception
            Errore = ex.Message & " - Stampa Ordinato per Articolo"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    Public Function StampaOrdinatoArtCliAg(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsOrdinatoArtCli1 As DSOrdinatoArtCli, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrWhere As String) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_OrdArtCliAg WHERE " & myStrWhere

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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
        'Riempio ds
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoArtCli1.OrdArtCli)
        'Ciclo che imposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdArtCli As DSOrdinatoArtCli.OrdArtCliRow
        For Each r_OrdArtCli In dsOrdinatoArtCli1.OrdArtCli
            r_OrdArtCli.AziendaReport = strNomeAz
            r_OrdArtCli.TitoloReport = TitoloReport
        Next
        dsOrdinatoArtCli1.AcceptChanges()

        Return True

    End Function
    Public Function StampaOrdinatoClienteOrdineAG(ByVal strNomeAz As String, ByVal TitoloReport As String,
                                                  ByRef dsOrdinatoClienteOrdine1 As DSOrdinatoClienteOrdine, ByRef ObjReport As Object,
                                                  ByRef Errore As String, ByVal myStrDa As String, ByVal myStrA As String,
                                                  ByVal CodAgente As Integer) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""
        Dim Where As Boolean = False

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_OrdinatoClienteOrdineAG "

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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
                strOrdCli = strOrdCli & " AND Agente = " & CodAgente
            Else
                strOrdCli = strOrdCli & " WHERE Agente = " & CodAgente
                Where = True
            End If
        End If
        ' ''strOrdCli = strOrdCli & " ORDER BY Riga "
        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoClienteOrdine1.OrdinatoClienteOrdine)
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim r_OrdPerCli As DSOrdinatoClienteOrdine.OrdinatoClienteOrdineRow
        For Each r_OrdPerCli In dsOrdinatoClienteOrdine1.OrdinatoClienteOrdine
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsOrdinatoClienteOrdine1.AcceptChanges()

        Return True

    End Function
#End Region

#Region " PREVENTIVI CLIENTI "
    Public Function StampaPrevClienteOrdineAG(ByVal strNomeAz As String, ByVal TitoloReport As String,
                                                  ByRef dsOrdinatoClienteOrdine1 As DSOrdinatoClienteOrdine, ByRef ObjReport As Object,
                                                  ByRef Errore As String,
                                                  ByVal pDaData As String, ByVal pAData As String,
                                                  ByVal pCAgente As Integer, ByVal pLeadSorce As Integer, ByVal pRegione As Integer,
                                                  ByVal myStrWhere As String,
                                                  ByVal TipoStampaSA As Integer) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""
        Dim Where As Boolean = False

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        'giu170622 ora uso la FUNZIONE parametrizzata "SELECT * FROM view_PrevClienteOrdineAG WHERE " & myStrWhere
        '(@DataDa nvarchar(10)=NULL,@DataA nvarchar(10)=NULL,@CodAgente INT=NULL,@LeadSource int=NULL,@Regione int=NULL)
        Dim strOrdCli As String = "SELECT * FROM fun_PrevClienteOrdineAG('" + pDaData + "','" + pAData + "'," +
                                    IIf(pCAgente > 0, pCAgente.ToString.Trim, "NULL") + "," +
                                    IIf(pLeadSorce > 0, pLeadSorce.ToString.Trim, "NULL") + "," +
                                    IIf(pRegione > 0, pRegione.ToString.Trim, "NULL") + ") as expr1" +
                                    IIf(myStrWhere.Trim <> "", " WHERE " + myStrWhere, "")
        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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

        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoClienteOrdine1.OrdinatoClienteOrdine)
        'TotAgenti
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
        SqlDbSelectCmd.CommandText = "SELECT Codice AS Agente, " &
        "0 AS TotDaConfAG, 0 AS TotConfAG, 0 AS TotChiusiNoConfAG, 0 TotNonConfAG, " & TipoStampaSA.ToString.Trim & " AS Sintetico, " &
        "0 AS TotDaConf, 0 AS TotConf, 0 AS TotChiusiNoConf, 0 TotNonConf " &
        "FROM Agenti"
        SqlAdap.Fill(dsOrdinatoClienteOrdine1.TotAgente)
        '- Sconosciuto
        SqlDbSelectCmd.CommandText = "SELECT 0 AS Agente, " &
        "0 AS TotDaConfAG, 0 AS TotConfAG, 0 AS TotChiusiNoConfAG, 0 TotNonConfAG, " & TipoStampaSA.ToString.Trim & " AS Sintetico, " &
        "0 AS TotDaConf, 0 AS TotConf, 0 AS TotChiusiNoConf, 0 TotNonConf "
        SqlAdap.Fill(dsOrdinatoClienteOrdine1.TotAgente)
        '-----
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim TotDaConf As Integer = 0 : Dim TotConf As Integer = 0
        Dim TotChiusiNoConf As Integer = 0 : Dim TotNonConf = 0
        Dim IDDoc As Long = 0 : Dim StatoDoc As Integer = -1 : Dim IDAgente As Long = -1
        Dim r_TotAgenti As DSOrdinatoClienteOrdine.TotAgenteRow
        Dim r_OrdPerCli As DSOrdinatoClienteOrdine.OrdinatoClienteOrdineRow
        For Each r_OrdPerCli In dsOrdinatoClienteOrdine1.OrdinatoClienteOrdine.Select("", "IDDocumenti")
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
            If IDDoc <> r_OrdPerCli.IDDocumenti Then
                If IDDoc = 0 Then
                    IDDoc = r_OrdPerCli.IDDocumenti
                    StatoDoc = r_OrdPerCli.StatoDoc
                    IDAgente = r_OrdPerCli.Agente
                Else
                    r_TotAgenti = Nothing
                    r_TotAgenti = dsOrdinatoClienteOrdine1.TotAgente.FindByAgente(IDAgente)
                    If Not r_TotAgenti Is Nothing Then
                        r_TotAgenti.BeginEdit()
                        Select Case StatoDoc
                            Case 0
                                r_TotAgenti.TotDaConfAG = r_TotAgenti.TotDaConfAG + 1
                                TotDaConf += 1
                            Case 1
                                r_TotAgenti.TotConfAG = r_TotAgenti.TotConfAG + 1
                                TotConf += 1
                            Case 3
                                r_TotAgenti.TotChiusoNoConfAG = r_TotAgenti.TotChiusoNoConfAG + 1
                                TotChiusiNoConf += 1
                            Case 4
                                r_TotAgenti.TotNonConfAG = r_TotAgenti.TotNonConfAG + 1
                                TotNonConf += 1
                        End Select
                        r_TotAgenti.EndEdit()
                    Else
                        r_TotAgenti = Nothing
                        r_TotAgenti = dsOrdinatoClienteOrdine1.TotAgente.FindByAgente(0)
                        If Not r_TotAgenti Is Nothing Then
                            r_TotAgenti.BeginEdit()
                            Select Case StatoDoc
                                Case 0
                                    r_TotAgenti.TotDaConfAG = r_TotAgenti.TotDaConfAG + 1
                                    TotDaConf += 1
                                Case 1
                                    r_TotAgenti.TotConfAG = r_TotAgenti.TotConfAG + 1
                                    TotConf += 1
                                Case 3
                                    r_TotAgenti.TotChiusoNoConfAG = r_TotAgenti.TotChiusoNoConfAG + 1
                                    TotChiusiNoConf += 1
                                Case 4
                                    r_TotAgenti.TotNonConfAG = r_TotAgenti.TotNonConfAG + 1
                                    TotNonConf += 1
                            End Select
                            r_TotAgenti.EndEdit()
                        End If
                    End If
                    '-
                    IDDoc = r_OrdPerCli.IDDocumenti
                    StatoDoc = r_OrdPerCli.StatoDoc
                    IDAgente = r_OrdPerCli.Agente

                End If
            End If
        Next
        'Ultimo
        r_TotAgenti = Nothing
        r_TotAgenti = dsOrdinatoClienteOrdine1.TotAgente.FindByAgente(IDAgente)
        If Not r_TotAgenti Is Nothing Then
            r_TotAgenti.BeginEdit()
            Select Case StatoDoc
                Case 0
                    r_TotAgenti.TotDaConfAG = r_TotAgenti.TotDaConfAG + 1
                    TotDaConf += 1
                Case 1
                    r_TotAgenti.TotConfAG = r_TotAgenti.TotConfAG + 1
                    TotConf += 1
                Case 3
                    r_TotAgenti.TotChiusoNoConfAG = r_TotAgenti.TotChiusoNoConfAG + 1
                    TotChiusiNoConf += 1
                Case 4
                    r_TotAgenti.TotNonConfAG = r_TotAgenti.TotNonConfAG + 1
                    TotNonConf += 1
            End Select
            r_TotAgenti.EndEdit()
        Else
            r_TotAgenti = Nothing
            r_TotAgenti = dsOrdinatoClienteOrdine1.TotAgente.FindByAgente(0)
            If Not r_TotAgenti Is Nothing Then
                r_TotAgenti.BeginEdit()
                Select Case StatoDoc
                    Case 0
                        r_TotAgenti.TotDaConfAG = r_TotAgenti.TotDaConfAG + 1
                        TotDaConf += 1
                    Case 1
                        r_TotAgenti.TotConfAG = r_TotAgenti.TotConfAG + 1
                        TotConf += 1
                    Case 3
                        r_TotAgenti.TotChiusoNoConfAG = r_TotAgenti.TotChiusoNoConfAG + 1
                        TotChiusiNoConf += 1
                    Case 4
                        r_TotAgenti.TotNonConfAG = r_TotAgenti.TotNonConfAG + 1
                        TotNonConf += 1
                End Select
                r_TotAgenti.EndEdit()
            End If
        End If
        '-
        'TotAgenti
        For Each r_TotAgenti In dsOrdinatoClienteOrdine1.TotAgente
            r_TotAgenti.TotDaConf = TotDaConf
            r_TotAgenti.TotConf = TotConf
            r_TotAgenti.TotChiusoNoConf = TotChiusiNoConf
            r_TotAgenti.TotNonConf = TotNonConf
        Next
        '-
        dsOrdinatoClienteOrdine1.AcceptChanges()

        Return True

    End Function
    Public Function StampaPrevClienteOrdineLS(ByVal strNomeAz As String, ByVal TitoloReport As String,
                                                  ByRef dsOrdinatoClienteOrdine1 As DSOrdinatoClienteOrdine, ByRef ObjReport As Object,
                                                  ByRef Errore As String,
                                                  ByVal pDaData As String, ByVal pAData As String,
                                                  ByVal pCAgente As Integer, ByVal pLeadSorce As Integer, ByVal pRegione As Integer,
                                                  ByVal myStrWhere As String,
                                                  ByVal TipoStampaSA As Integer) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""
        Dim Where As Boolean = False

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        'giu170622 ora uso la FUNZIONE parametrizzata "SELECT * FROM view_PrevClienteOrdineAG WHERE " & myStrWhere
        '(@DataDa nvarchar(10)=NULL,@DataA nvarchar(10)=NULL,@CodAgente INT=NULL,@LeadSource int=NULL,@Regione int=NULL)
        Dim strOrdCli As String = "SELECT * FROM fun_PrevClienteOrdineAG('" + pDaData + "','" + pAData + "'," +
                                    IIf(pCAgente > 0, pCAgente.ToString.Trim, "NULL") + "," +
                                    IIf(pLeadSorce > 0, pLeadSorce.ToString.Trim, "NULL") + "," +
                                    IIf(pRegione > 0, pRegione.ToString.Trim, "NULL") + ") as expr1" +
                                    IIf(myStrWhere.Trim <> "", " WHERE " + myStrWhere, "")
        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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

        SqlDbSelectCmd.CommandText = strOrdCli
        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoClienteOrdine1.OrdinatoClienteOrdine)
        'TotLeadSource
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDbSelectCmd.CommandText = "SELECT Codice AS LeadSource, Descrizione, " &
        "0 AS TotDaConfLS, 0 AS TotConfLS, 0 AS TotChiusiNoConfLS, 0 TotNonConfLS, " & TipoStampaSA.ToString.Trim & " AS Sintetico, " &
        "0 AS TotDaConf, 0 AS TotConf, 0 AS TotChiusiNoConf, 0 TotNonConf " &
        "FROM LeadSource"
        SqlAdap.Fill(dsOrdinatoClienteOrdine1.TotLeadSource)
        '- Sconosciuto
        SqlDbSelectCmd.CommandText = "SELECT 0 AS LeadSource, 'Non definito' AS Descrizione, " &
        "0 AS TotDaConfLS, 0 AS TotConfLS, 0 AS TotChiusiNoConfLS, 0 TotNonConfLS, " & TipoStampaSA.ToString.Trim & " AS Sintetico, " &
        "0 AS TotDaConf, 0 AS TotConf, 0 AS TotChiusiNoConf, 0 TotNonConf "
        SqlAdap.Fill(dsOrdinatoClienteOrdine1.TotLeadSource)
        '-----
        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe
        Dim TotDaConf As Integer = 0 : Dim TotConf As Integer = 0
        Dim TotChiusiNoConf As Integer = 0 : Dim TotNonConf = 0
        Dim IDDoc As Long = 0 : Dim StatoDoc As Integer = -1 : Dim IDLeadSource As Long = -1
        Dim r_TotLeadSource As DSOrdinatoClienteOrdine.TotLeadSourceRow
        Dim r_OrdPerCli As DSOrdinatoClienteOrdine.OrdinatoClienteOrdineRow
        For Each r_OrdPerCli In dsOrdinatoClienteOrdine1.OrdinatoClienteOrdine.Select("", "IDDocumenti")
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
            If IDDoc <> r_OrdPerCli.IDDocumenti Then
                If IDDoc = 0 Then
                    IDDoc = r_OrdPerCli.IDDocumenti
                    StatoDoc = r_OrdPerCli.StatoDoc
                    IDLeadSource = r_OrdPerCli.LeadSource
                Else
                    r_TotLeadSource = Nothing
                    r_TotLeadSource = dsOrdinatoClienteOrdine1.TotLeadSource.FindByLeadSource(IDLeadSource)
                    If Not r_TotLeadSource Is Nothing Then
                        r_TotLeadSource.BeginEdit()
                        Select Case StatoDoc
                            Case 0
                                r_TotLeadSource.TotDaConfLS = r_TotLeadSource.TotDaConfLS + 1
                                TotDaConf += 1
                            Case 1
                                r_TotLeadSource.TotConfLS = r_TotLeadSource.TotConfLS + 1
                                TotConf += 1
                            Case 3
                                r_TotLeadSource.TotChiusoNoConfLS = r_TotLeadSource.TotChiusoNoConfLS + 1
                                TotChiusiNoConf += 1
                            Case 4
                                r_TotLeadSource.TotNonConfLS = r_TotLeadSource.TotNonConfLS + 1
                                TotNonConf += 1
                        End Select
                        r_TotLeadSource.EndEdit()
                    Else
                        r_TotLeadSource = Nothing
                        r_TotLeadSource = dsOrdinatoClienteOrdine1.TotLeadSource.FindByLeadSource(0)
                        If Not r_TotLeadSource Is Nothing Then
                            r_TotLeadSource.BeginEdit()
                            Select Case StatoDoc
                                Case 0
                                    r_TotLeadSource.TotDaConfLS = r_TotLeadSource.TotDaConfLS + 1
                                    TotDaConf += 1
                                Case 1
                                    r_TotLeadSource.TotConfLS = r_TotLeadSource.TotConfLS + 1
                                    TotConf += 1
                                Case 3
                                    r_TotLeadSource.TotChiusoNoConfLS = r_TotLeadSource.TotChiusoNoConfLS + 1
                                    TotChiusiNoConf += 1
                                Case 4
                                    r_TotLeadSource.TotNonConfLS = r_TotLeadSource.TotNonConfLS + 1
                                    TotNonConf += 1
                            End Select
                            r_TotLeadSource.EndEdit()
                        End If
                    End If
                    '-
                    IDDoc = r_OrdPerCli.IDDocumenti
                    StatoDoc = r_OrdPerCli.StatoDoc
                    IDLeadSource = r_OrdPerCli.LeadSource

                End If
            End If
        Next
        'Ultimo
        r_TotLeadSource = Nothing
        r_TotLeadSource = dsOrdinatoClienteOrdine1.TotLeadSource.FindByLeadSource(IDLeadSource)
        If Not r_TotLeadSource Is Nothing Then
            r_TotLeadSource.BeginEdit()
            Select Case StatoDoc
                Case 0
                    r_TotLeadSource.TotDaConfLS = r_TotLeadSource.TotDaConfLS + 1
                    TotDaConf += 1
                Case 1
                    r_TotLeadSource.TotConfLS = r_TotLeadSource.TotConfLS + 1
                    TotConf += 1
                Case 3
                    r_TotLeadSource.TotChiusoNoConfLS = r_TotLeadSource.TotChiusoNoConfLS + 1
                    TotChiusiNoConf += 1
                Case 4
                    r_TotLeadSource.TotNonConfLS = r_TotLeadSource.TotNonConfLS + 1
                    TotNonConf += 1
            End Select
            r_TotLeadSource.EndEdit()
        Else
            r_TotLeadSource = Nothing
            r_TotLeadSource = dsOrdinatoClienteOrdine1.TotLeadSource.FindByLeadSource(0)
            If Not r_TotLeadSource Is Nothing Then
                r_TotLeadSource.BeginEdit()
                Select Case StatoDoc
                    Case 0
                        r_TotLeadSource.TotDaConfLS = r_TotLeadSource.TotDaConfLS + 1
                        TotDaConf += 1
                    Case 1
                        r_TotLeadSource.TotConfLS = r_TotLeadSource.TotConfLS + 1
                        TotConf += 1
                    Case 3
                        r_TotLeadSource.TotChiusoNoConfLS = r_TotLeadSource.TotChiusoNoConfLS + 1
                        TotChiusiNoConf += 1
                    Case 4
                        r_TotLeadSource.TotNonConfLS = r_TotLeadSource.TotNonConfLS + 1
                        TotNonConf += 1
                End Select
                r_TotLeadSource.EndEdit()
            End If
        End If
        '-
        'TotLeadSource
        For Each r_TotLeadSource In dsOrdinatoClienteOrdine1.TotLeadSource
            r_TotLeadSource.TotDaConf = TotDaConf
            r_TotLeadSource.TotConf = TotConf
            r_TotLeadSource.TotChiusoNoConf = TotChiusiNoConf
            r_TotLeadSource.TotNonConf = TotNonConf
        Next
        '-
        dsOrdinatoClienteOrdine1.AcceptChanges()

        Return True

    End Function
#End Region

#Region " ORDINATO FORNITORI" 'giu110612 giu281013

    Public Function StampaOrdinatoArtFor(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsOrdinatoArtCli1 As DSOrdinatoArtCli, ByRef ObjReport As Object, ByRef Errore As String, ByVal myStrWhere As String) As Boolean

        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdFor As String = "SELECT * FROM view_OrdArtFor WHERE " & myStrWhere

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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
        'Riempio ds
        SqlAdap.SelectCommand = SqlDbSelectCmd
        SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
        SqlDbSelectCmd.Connection = SqlConn1

        'If NomeCampoOrdinamento.Trim = "Codice_CoGe" Then
        '    If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
        '        strOrdCli = strOrdCli & " WHERE Codice_CoGe >= '" & myStrDa.Trim & _
        '                    "' AND  Codice_CoGe <= '" & myStrA.Trim & "' "
        '    End If
        'ElseIf NomeCampoOrdinamento.Trim = "Rag_Soc" Then
        '    If myStrDa.Trim <> "" And myStrA.Trim <> "" Then
        '        strOrdCli = strOrdCli & " WHERE Rag_Soc >= '" & myStrDa.Trim & _
        '                    "' AND  Rag_Soc <= '" & myStrA.Trim & "' "
        '    End If
        'End If
        'If NomeCampoOrdinamento.Trim <> "" Then
        '    strOrdCli = strOrdCli & " ORDER BY " & NomeCampoOrdinamento
        'End If

        SqlDbSelectCmd.CommandText = strOrdFor

        'Riempio Dataset
        SqlAdap.Fill(dsOrdinatoArtCli1.OrdArtCli)

        'Ciclo che aÃ¬imposta AziendaReport e Titolo Report per tutte le righe


        Dim r_OrdArtCli As DSOrdinatoArtCli.OrdArtCliRow
        For Each r_OrdArtCli In dsOrdinatoArtCli1.OrdArtCli
            r_OrdArtCli.AziendaReport = strNomeAz
            r_OrdArtCli.TitoloReport = TitoloReport
        Next
        dsOrdinatoArtCli1.AcceptChanges()


        Return True

    End Function

    Public Function StampaStatOrdinatoForOrdine(ByVal strNomeAz As String, ByVal TitoloReport As String, ByRef dsStatOrdinatoClienteOrdine1 As dsStatOrdinatoClienteOrdine, ByRef ObjReport As Object, ByRef Errore As String, ByRef strWhere As String) As Boolean
        Dim Passo As Integer = 0
        Dim strErrore As String = ""
        Dim strSQL As String = ""
        Dim Where As Boolean = False

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))

        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdap As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Dim strOrdCli As String = "SELECT * FROM view_StatOrdinatoFornOrdine WHERE " & strWhere

        SqlConn1 = New SqlConnection
        SqlConn2 = New SqlConnection
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

        ' ''If myCFor.Trim <> "" Then
        ' ''    Where = True
        ' ''    strOrdCli = strOrdCli & " WHERE Cod_Cliente = '" & Controlla_Apice(myCFor.Trim) & "' "
        ' ''End If

        ' ''If myStrCArt.Trim <> "" Then
        ' ''    If Where Then
        ' ''        strOrdCli = strOrdCli & " AND "
        ' ''    Else
        ' ''        strOrdCli = strOrdCli & " WHERE "
        ' ''        Where = True
        ' ''    End If
        ' ''    strOrdCli = strOrdCli & "Cod_Articolo = '" & Controlla_Apice(myStrCArt.Trim) & "' "
        ' ''End If

        ' ''If Data1 <> "" Then
        ' ''    If Where Then
        ' ''        strOrdCli = strOrdCli & " AND "
        ' ''    Else
        ' ''        strOrdCli = strOrdCli & " WHERE "
        ' ''        Where = True
        ' ''    End If
        ' ''    strOrdCli = strOrdCli & "Data_Doc >= CONVERT(datetime, '" & Data1 & "', 103) "
        ' ''End If

        ' ''If Data2 <> "" Then
        ' ''    If Where Then
        ' ''        strOrdCli = strOrdCli & " AND "
        ' ''    Else
        ' ''        strOrdCli = strOrdCli & " WHERE "
        ' ''        Where = True
        ' ''    End If
        ' ''    strOrdCli = strOrdCli & "Data_Doc <= CONVERT(datetime, '" & Data2 & "', 103) "
        ' ''End If

        strOrdCli = strOrdCli & " ORDER BY IDDocumenti, Riga "

        SqlDbSelectCmd.CommandText = strOrdCli

        'Riempio Dataset
        SqlAdap.Fill(dsStatOrdinatoClienteOrdine1.StatOrdinatoClienteOrdine)

        'Ciclo che aìimposta AziendaReport e Titolo Report per tutte le righe


        Dim r_OrdPerCli As dsStatOrdinatoClienteOrdine.StatOrdinatoClienteOrdineRow
        For Each r_OrdPerCli In dsStatOrdinatoClienteOrdine1.StatOrdinatoClienteOrdine
            r_OrdPerCli.AziendaReport = strNomeAz
            r_OrdPerCli.TitoloReport = TitoloReport
        Next
        dsStatOrdinatoClienteOrdine1.AcceptChanges()

        Return True

    End Function
#End Region

End Class
