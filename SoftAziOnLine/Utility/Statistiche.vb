Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Public Class Statistiche
    'GIU110619 OK SU PIU' ANNI
    Public Function StampaStatisticheVendutoArticoloCliente(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodCliente As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Ordinamento As Integer, ByVal Statistica As Integer, ByVal VisualizzaPrezzoVendita As Boolean, ByVal SWRegione As Boolean, ByVal CodRegione As Integer, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String,
                                                            ByVal InvioEmail As Boolean, ByVal EscludiCliCateg As Boolean, ByVal EscludiCliNoEmail As Boolean) As Boolean
        'GIU140819 , Optional ByVal txtDataInvioEmail As String = "" SEMMAI SI FARA' LA MODIFICA PER DATA INVIO
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            '---------------------------
            'GIU070619 @@@@@@@@@@@@@@@@@ ADAPTER E' LA VECCHIA RELEASE CHE ESTRAE DATI DAL SOLO ANNO CORRENTE QUINDI NON PIU USATA
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            'giu051112 SONO IDENTICHE QUINDI USO SEMPRE LA SP:get_StaVendCliArtPrezzoVendita
            ' ''If VisualizzaPrezzoVendita Then
            ' ''    SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVendita"
            ' ''Else
            ' ''    SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtNOPrezzoVendita"
            ' ''End If
            If SWRegione = True Then
                SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVenditaREG"
            Else
                SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVendita"
            End If

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodCliente <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = Controlla_Apice(CodCliente)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = System.DBNull.Value
            End If
            SqlDbSelectStatVendCliArt.Parameters.Item("@Ordinamento").Value = Ordinamento
            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica

            If SWRegione = True Then
                SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodRegione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
                If CodRegione = 0 Then
                    SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = System.DBNull.Value
                Else
                    SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = CodRegione
                End If
            End If
            'giu040619
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                If Statistica = 1 Then 'fatturato
                    strSQL = "Select * From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL = "Select * From DaFatturareClientiArticolo("
                Else
                    strSQL = "Select * From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodCliente <> "" Then
                    strSQL += "'" & Controlla_Apice(CodCliente) & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL,NULL) AS Expr1 " & vbCr 'Agente,CodCateg
                Dim SWWhere As Boolean = False
                If SWRegione = True And CodRegione <> 0 Then
                    SWWhere = True
                    strSQL += " WHERE Provincia IN (SELECT  Province.Codice FROM Province INNER JOIN Regioni ON Province.Regione = Regioni.Codice WHERE (Province.Regione = " & CodRegione.ToString.Trim & "))"
                End If
                If InvioEmail Then
                    'giu140819 txtDataInvioEmail DA AGGIUNGERE (SE SI FARA' LA MODIFICA) NELLE 2 SELECT CHE SEGUONO
                    'AND EmailInviateT.DataInvio >= CONVERT(DATETIME, '22/02/2019', 103) AND EmailInviateT.DataInvio <= CONVERT(DATETIME, '22/02/2019 23:59:59', 103)
                    'giu300719
                    If SWWhere = True Then
                        strSQL += " AND Codice_CoGe IN ("
                    Else
                        SWWhere = True
                        strSQL += " WHERE Codice_CoGe IN ("
                    End If
                    strSQL += "SELECT ArticoliInst_ContrattiAss.Cod_Coge FROM EmailInviateT INNER JOIN EmailInviateDett ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID WHERE (EmailInviateT.Stato = 99) GROUP BY ArticoliInst_ContrattiAss.Cod_Coge)"
                    If SWWhere = True Then
                        strSQL += " AND Cod_Articolo IN ("
                    Else
                        SWWhere = True
                        strSQL += " WHERE Cod_Articolo IN ("
                    End If
                    strSQL += "SELECT ArticoliInst_ContrattiAss.Cod_Articolo FROM EmailInviateT INNER JOIN EmailInviateDett ON EmailInviateT.Id = EmailInviateDett.IdEmailInviateT INNER JOIN ArticoliInst_ContrattiAss ON EmailInviateDett.IdArticoliInst_ContrattiAss = ArticoliInst_ContrattiAss.ID WHERE (EmailInviateT.Stato = 99) AND (ArticoliInst_ContrattiAss.DataScadGaranzia IS NULL) GROUP BY ArticoliInst_ContrattiAss.Cod_Articolo)"
                End If
                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto Per Cliente / Articolo"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select * From Regioni"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DSStatVendCliArt1, "Regioni")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto Per Cliente / Articolo REGIONI"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Province"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DSStatVendCliArt1, "Province")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto Per Cliente / Articolo PROVINCE"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Categorie"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DSStatVendCliArt1, "Categorie")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto  Per Cliente / Articolo CATEGORIE"
                    Return False
                    Exit Function
                End Try
                '-
                If EscludiCliNoEmail Then 'giu300719
                    strSQL = "Select Codice_CoGe,InvioMailScad From Clienti WHERE InvioMailScad=0"
                    Try
                        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DSStatVendCliArt1, "CliNoEmail")
                    Catch Ex As Exception
                        ObjDB = Nothing
                        Errore = Ex.Message & " - Stampa Statistica Venduto  Per Cliente / Articolo CliNoEmail"
                        Return False
                        Exit Function
                    End Try
                End If
                ObjDB = Nothing
            End If
            '---------
            Dim rowRegione As DsStatVendCliArt.RegioniRow = Nothing
            Dim rowProvince As DsStatVendCliArt.ProvinceRow = Nothing
            'giu300719
            Dim rowCatecorie As DsStatVendCliArt.CategorieRow = Nothing
            Dim rowCliNoEmail As DsStatVendCliArt.CliNoEmailRow = Nothing
            Dim SWCanc As Boolean = False
            '---------
            If DSStatVendCliArt1.StCliArt.Count > 0 Then
                For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                    row.Azienda = _Esercizio
                    Dim strRegione As String = ""
                    If SWRegione = True Then
                        strRegione = "regione / "
                    End If
                    If Ordinamento = TIPOSTAMPASTATISTICA.VendutoClienteArticolo Or
                       Ordinamento = TIPOSTAMPASTATISTICA.VendutoClienteArticoloREG Then
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica articoli venduti / fatturati per " & strRegione & "cliente / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica articoli fatturati per " & strRegione & "cliente / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica articoli da fatturare per " & strRegione & "cliente / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                    ElseIf Ordinamento = TIPOSTAMPASTATISTICA.VendutoArticoloCliente Or
                           Ordinamento = TIPOSTAMPASTATISTICA.VendutoArticoloClienteREG Then
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica articoli venduti / fatturati per " & strRegione & "articolo / cliente (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica articoli fatturati per " & strRegione & "articolo / cliente (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica articoli da fatturare per " & strRegione & "articolo / cliente (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                    End If
                    If InvioEmail Then
                        row.TitoloReport += " - Inviata E-Mail ALERT e conclusa"
                    End If
                    If row.IsTitoloReportNull Then 'giu140819
                        row.TitoloReport = "Statistica articoli venduti / fatturati per " & strRegione & "cliente / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf row.TitoloReport.Trim = "" Then
                        row.TitoloReport = "Statistica articoli venduti / fatturati per " & strRegione & "cliente / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    End If
                    row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                    If InvioEmail Then
                        row.PiedeReport += " - Inviata E-Mail ALERT e conclusa"
                    End If
                    If VisualizzaPrezzoVendita Then
                        row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                    Else
                        row.VisualizzaPrezzoVendita = False
                    End If
                    If SWStatVendOLD = False Then 'solo per statistica su piu anni
                        'giu300719
                        SWCanc = False
                        If EscludiCliCateg Then
                            rowCatecorie = DSStatVendCliArt1.Categorie.FindByCodice(row.Cod_Categoria)
                            If Not rowCatecorie Is Nothing Then
                                If Not rowCatecorie.IsInvioMailScNull Then
                                    If rowCatecorie.InvioMailSc = False Then
                                        row.Delete()
                                        SWCanc = True
                                    End If
                                End If
                                If SWCanc = False Then
                                    If Not rowCatecorie.IsSelScNull Then
                                        If rowCatecorie.SelSc = False Then
                                            row.Delete()
                                            SWCanc = True
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        If EscludiCliNoEmail And SWCanc = False Then
                            rowCliNoEmail = DSStatVendCliArt1.CliNoEmail.FindByCodice_Coge(row.Codice_CoGe)
                            If Not rowCliNoEmail Is Nothing Then
                                If Not rowCliNoEmail.IsInvioMailScadNull Then
                                    If rowCliNoEmail.InvioMailScad = False Then
                                        row.Delete()
                                        SWCanc = True
                                    End If
                                End If
                            End If
                        End If
                        '---------
                        If SWCanc = False Then
                            rowProvince = DSStatVendCliArt1.Province.FindByCodice(row.Provincia)
                            If Not rowProvince Is Nothing Then
                                row.DesProvincia = rowProvince.Descrizione
                                rowRegione = DSStatVendCliArt1.Regioni.FindByCodice(rowProvince.Regione)
                                If Not rowRegione Is Nothing Then
                                    row.Regione = rowProvince.Regione
                                    row.DesRegione = rowRegione.Descrizione
                                Else
                                    row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                                End If
                            Else
                                row.DesProvincia = "Sconosciuta " & row.Provincia.Trim
                                row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                            End If
                        End If
                    End If
                Next
                DSStatVendCliArt1.AcceptChanges() 'giu300719
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto Per Cliente / Articolo"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    Public Function StampaVendutoDDT(ByVal AzReport As String, ByRef DataS As DSStatRiepVendutoNumero, ByRef Errore As String, ByVal TipoStatistica As Integer, ByVal DaDDT As Integer, ByVal ADDT As Integer, ByVal DataDa As String, ByVal DataA As String) As Boolean
        StampaVendutoDDT = True
        Dim clsDB As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection



        Try
            SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)

            'alberto 11/05/2012 SQLCmd.CommandText = "SELECT * FROM vi_VendutoDDT " & SQLstrWhere
            SQLCmd.CommandText = "[get_VendutoDDTPerStampa]" 'alberto 11/05/2012

            'alberto 11/05/2012 SQLCmd.CommandType = CommandType.Text
            SQLCmd.CommandType = CommandType.StoredProcedure 'alberto 11/05/2012
            SQLCmd.Connection = SQLConn

            'alberto 11/05/2012
            SQLCmd.Parameters.Add(New SqlParameter("@TipoStatistica", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@DaDDT", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@ADDT", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@DataDa", SqlDbType.NVarChar, 10))
            SQLCmd.Parameters.Add(New SqlParameter("@DataA", SqlDbType.NVarChar, 10))

            SQLCmd.Parameters("@TipoStatistica").Value = TipoStatistica
            SQLCmd.Parameters("@DaDDT").Value = DaDDT
            SQLCmd.Parameters("@ADDT").Value = ADDT

            If DataDa <> "" Then
                SQLCmd.Parameters("@DataDa").Value = DataDa
            End If

            If DataA <> "" Then
                SQLCmd.Parameters("@DataA").Value = DataA
            End If
            '----------------

            SQLAdp.SelectCommand = SQLCmd
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
            SQLAdp.SelectCommand.CommandTimeout = myTimeOUT
            '---------------------------
            SQLAdp.Fill(DataS.VendutoNumero)

            For Each row As DSStatRiepVendutoNumero.VendutoNumeroRow In DataS.VendutoNumero.Rows
                row.BeginEdit()
                row.Ditta = AzReport
                row.EndEdit()
            Next

            DataS.AcceptChanges()
        Catch ex As Exception
            Errore = ex.Message & " - Stampa venduto per DDT."
            StampaVendutoDDT = False
            Exit Function
        End Try
    End Function
    'GIU110619 E' GIA SU PIU ANNI giu120220 Filtro per cliente (fatto dal chiamante) - Stampa doc.RefInt es. ORDINE per i DDT trovati
    'GIU220920 GESTIONE MAGAZZINI ARRIVA NELLA WHERE
    Public Function StampaMovMag(ByVal AzReport As String, ByVal TitoloReport As String, ByVal SQLstrWhere As String, ByRef dsMovMag1 As DSMovMag, ByRef Errore As String, ByVal Filtri As String, ByVal IDDocumento As Long, ByVal codiceArticolo As String, ByVal SWStampaLotti As Boolean, ByVal NSerie As String, ByVal Lotto As String, ByVal SWFindLottiSerie As Boolean, ByVal SWStDocRF As Boolean, Optional ByVal DataDa As String = "-", Optional ByVal DataA As String = "-", Optional ByVal MinEser As String = "", Optional ByVal MaxEser As String = "") As Boolean
        StampaMovMag = True
        Dim clsDB As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection
        Dim NSerieLotto As String
        Dim Eser As Integer

        Try
            dsMovMag1.EnforceConstraints = False
            If IDDocumento > 0 Then
                SQLCmd.CommandText = "SELECT * FROM view_MovMag WHERE IDDocumenti = " & IDDocumento & " ORDER BY Data_Doc"
            ElseIf codiceArticolo.Trim <> "" Then
                SQLCmd.CommandText = "SELECT * FROM view_MovMag WHERE Cod_Articolo = '" & codiceArticolo & "' ORDER BY Data_Doc"
            Else
                SQLCmd.CommandText = "SELECT * FROM view_MovMag " & SQLstrWhere & " ORDER BY Data_Doc"
            End If

            SQLCmd.CommandType = CommandType.Text
            SQLCmd.Connection = SQLConn

            SQLAdp.SelectCommand = SQLCmd
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
            SQLAdp.SelectCommand.CommandTimeout = myTimeOUT
            '---------------------------
            'alb25012013 adesso se si passano datada e dataa lavora su tutti gli esercizi
            'nell intervallo specificato
            If DataDa <> "-" And DataA <> "-" Then
                If DataDa = "" And DataA <> "" Then
                    For Eser = CInt(MinEser) To Year(CDate(DataA)) Step 1
                        SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                        SQLAdp.Fill(dsMovMag1.view_MovMag)
                        SQLConn.Close()
                    Next
                ElseIf DataDa <> "" And DataA = "" Then
                    For Eser = Year(CDate(DataDa)) To CInt(MaxEser) Step 1
                        SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                        SQLAdp.Fill(dsMovMag1.view_MovMag)
                        SQLConn.Close()
                    Next
                ElseIf DataDa = "" And DataA = "" Then
                    For Eser = CInt(MinEser) To CInt(MaxEser) Step 1
                        SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                        SQLAdp.Fill(dsMovMag1.view_MovMag)
                        SQLConn.Close()
                    Next
                Else
                    For Eser = Year(CDate(DataDa)) To Year(CDate(DataA)) Step 1
                        SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                        SQLAdp.Fill(dsMovMag1.view_MovMag)
                        SQLConn.Close()
                    Next
                End If
            Else
                SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
                SQLAdp.Fill(dsMovMag1.view_MovMag)
                SQLConn.Close()
            End If
            '----------------------------

            If NSerie.Trim <> "" And Lotto.Trim <> "" Then
                NSerieLotto = "Numero di serie selezionato: " & NSerie.Trim & " Lotto selezionato: " & Lotto.Trim
            ElseIf Lotto.Trim <> "" Then
                NSerieLotto = "Lotto selezionato: " & Lotto.Trim
            ElseIf NSerie.Trim <> "" Then
                NSerieLotto = "Numero di serie selezionato: " & NSerie.Trim
            Else
                NSerieLotto = ""
            End If
            '-
            'giu120312 giu090512 And NSerie.Trim = "" And Lotto.Trim = ""
            If SWStampaLotti = True Or SWStDocRF = True Then 'giu120220
                'Prendo i Lotti 
                Dim SqlConn1 As SqlConnection
                Dim SqlAdap As SqlDataAdapter
                Dim SqlDbSelectCmd As SqlCommand
                '-
                Dim SqlAdapRF As SqlDataAdapter
                Dim SqlDbSelectCmdRF As SqlCommand

                SqlConn1 = New SqlConnection

                SqlAdap = New SqlDataAdapter
                SqlDbSelectCmd = New SqlCommand
                SqlAdap.SelectCommand = SqlDbSelectCmd
                '-
                SqlAdapRF = New SqlDataAdapter
                SqlDbSelectCmdRF = New SqlCommand
                SqlAdapRF.SelectCommand = SqlDbSelectCmdRF

                SqlDbSelectCmd.CommandType = System.Data.CommandType.Text
                SqlDbSelectCmd.Connection = SqlConn1
                '-
                SqlDbSelectCmdRF.CommandType = System.Data.CommandType.Text
                SqlDbSelectCmdRF.Connection = SqlConn1

                'giu110220
                Dim SqlStrRefInt As String
                SqlStrRefInt = "SELECT * FROM DocumentiD WHERE IDDocumenti IN (SELECT Refint FROM view_MovMag " & SQLstrWhere & " GROUP BY Refint )"
                SqlDbSelectCmdRF.CommandText = SqlStrRefInt
                '---------
                Dim SqlStr As String
                If IDDocumento > 0 Then
                    SqlStr = "SELECT * FROM DocumentiDLotti WHERE IDDocumenti = " & IDDocumento
                ElseIf codiceArticolo.Trim <> "" Then
                    SqlStr = "SELECT * FROM DocumentiDLotti WHERE Cod_Articolo = '" & codiceArticolo & "'"
                Else
                    SqlStr = "SELECT * FROM DocumentiDLotti WHERE IDDocumenti IN (SELECT IDDocumenti FROM view_MovMag " & SQLstrWhere & " GROUP BY IDDocumenti )" 'giu110220
                End If
                SqlDbSelectCmd.CommandText = SqlStr
                'Riempio Dataset 
                'alb25012013 adesso se si passano datada e dataa lavora su tutti gli esercizi
                'nell intervallo specificato
                If DataDa <> "-" And DataA <> "-" Then
                    If DataDa = "" And DataA <> "" Then
                        For Eser = Year(CDate(DataA)) To CInt(MinEser) Step -1
                            If SWStampaLotti = True Then
                                SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                                SqlConn1.Open()
                                SqlAdap.Fill(dsMovMag1.DocumentiDLotti)
                                SqlConn1.Close()
                            End If
                            '-
                            If SWStDocRF = True Then
                                SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                                SqlConn1.Open()
                                SqlAdapRF.Fill(dsMovMag1.DocumentiD)
                                SqlConn1.Close()
                            End If
                        Next
                    ElseIf DataDa <> "" And DataA = "" Then
                        For Eser = CInt(MaxEser) To Year(CDate(DataDa)) Step -1
                            If SWStampaLotti = True Then
                                SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                                SqlConn1.Open()
                                SqlAdap.Fill(dsMovMag1.DocumentiDLotti)
                                SqlConn1.Close()
                            End If
                            '-
                            If SWStDocRF = True Then
                                SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                                SqlConn1.Open()
                                SqlAdapRF.Fill(dsMovMag1.DocumentiD)
                                SqlConn1.Close()
                            End If
                        Next
                    ElseIf DataDa = "" And DataA = "" Then
                        For Eser = CInt(MaxEser) To CInt(MinEser) Step -1
                            If SWStampaLotti = True Then
                                SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                                SqlConn1.Open()
                                SqlAdap.Fill(dsMovMag1.DocumentiDLotti)
                                SqlConn1.Close()
                            End If
                            '-
                            If SWStDocRF = True Then
                                SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                                SqlConn1.Open()
                                SqlAdapRF.Fill(dsMovMag1.DocumentiD)
                                SqlConn1.Close()
                            End If
                        Next
                    Else
                        For Eser = Year(CDate(DataA)) To Year(CDate(DataDa)) Step -1
                            If SWStampaLotti = True Then
                                SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                                SqlConn1.Open()
                                SqlAdap.Fill(dsMovMag1.DocumentiDLotti)
                                SqlConn1.Close()
                            End If
                            '-
                            If SWStDocRF = True Then
                                SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi, Eser.ToString)
                                SqlConn1.Open()
                                SqlAdapRF.Fill(dsMovMag1.DocumentiD)
                                SqlConn1.Close()
                            End If
                        Next
                    End If
                Else
                    If SWStampaLotti = True Then
                        SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
                        SqlConn1.Open()
                        SqlAdap.Fill(dsMovMag1.DocumentiDLotti)
                        SqlConn1.Close()
                    End If
                    '-
                    If SWStDocRF = True Then
                        SqlConn1.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
                        SqlConn1.Open()
                        SqlAdapRF.Fill(dsMovMag1.DocumentiD)
                        SqlConn1.Close()
                    End If
                End If
            Else
                dsMovMag1.DocumentiDLotti.Clear()
                dsMovMag1.DocumentiD.Clear()
            End If
            'giu200224 nuova stampa DDT con Magazzino da/a
            SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
            SQLCmd.CommandType = CommandType.Text
            SQLCmd.Connection = SQLConn
            SQLCmd.CommandText = "SELECT * FROM Magazzini"
            SQLAdp.SelectCommand = SQLCmd
            SQLAdp.Fill(dsMovMag1.Magazzini)
            SQLConn.Close()
            Dim rowMagazzini As DSMovMag.MagazziniRow = Nothing
            'giu120220 spostato qui
            Dim myIDPrec As Long = -1 'giu120220 per non stampare piu volte il DOC.REFINT MA SOLO 1 VOLTA
            For Each row As DSMovMag.view_MovMagRow In dsMovMag1.view_MovMag.Rows
                row.BeginEdit()
                row.AziendaRpt = AzReport
                row.TitoloRpt = TitoloReport
                row.Filtri = Filtri 'alb07052012
                'giu200224
                row.DesMagazzino = ""
                row.DesMagazzino2 = ""
                If Not row.IsCodiceMagazzinoNull Then
                    If row.CodiceMagazzino <> 0 Then
                        rowMagazzini = Nothing
                        rowMagazzini = dsMovMag1.Magazzini.FindByCodice(row.CodiceMagazzino)
                        If Not rowMagazzini Is Nothing Then
                            row.DesMagazzino = rowMagazzini.Descrizione.Trim
                        End If
                    End If
                End If
                If Not row.IsCodiceMagazzinoM2Null Then
                    If row.CodiceMagazzinoM2 <> 0 Then
                        rowMagazzini = Nothing
                        rowMagazzini = dsMovMag1.Magazzini.FindByCodice(row.CodiceMagazzinoM2)
                        If Not rowMagazzini Is Nothing Then
                            row.DesMagazzino2 = rowMagazzini.Descrizione.Trim
                        End If
                    End If
                End If
                '---------
                row.NSerieLotto = NSerieLotto 'alb07052012
                row.SWStampaLotti = SWStampaLotti
                row.SWStDocRF = SWStDocRF
                If SWStampaLotti = True Or SWStDocRF = True Then 'giu120220
                    If dsMovMag1.DocumentiDLotti.Select("IDDocumenti=" & row.IDDocumenti.ToString.Trim).Length = 0 Then
                        row.SWStampaLotti = False
                        row.SWStDocRF = False
                    ElseIf SWStDocRF = True Then
                        If myIDPrec = row.IDDocumenti Then
                            row.SWStDocRF = False
                        Else
                            myIDPrec = row.IDDocumenti 'ok stampo il DOC
                        End If
                    End If
                End If
                '-
                Select Case row.Tipo_Doc
                    Case "PR"
                        row.DescrizioneTipoDocumento = "Preventivo"
                    Case "OC"
                        row.DescrizioneTipoDocumento = "Ordine"
                    Case "OD"
                        row.DescrizioneTipoDocumento = "Ordine C/Deposito"
                    Case "OF"
                        row.DescrizioneTipoDocumento = "Ordine fornitori"
                    Case "PF"
                        row.DescrizioneTipoDocumento = "Proposta ordine fornitori"
                    Case "BC"
                        row.DescrizioneTipoDocumento = "Buono consegna"
                    Case "DT"
                        row.DescrizioneTipoDocumento = "Documento di trasporto"
                    Case "DF"
                        row.DescrizioneTipoDocumento = "Documento di trasporto fornitori"
                    Case "DL"
                        row.DescrizioneTipoDocumento = "Documento di trasporto C/Lavoro"
                    Case "FC"
                        row.DescrizioneTipoDocumento = "Fattura commerciale"
                        'GIU101018 VIENE FATTO NELLA STORE PROCEDURE
                        'If row.ScGiacenza = True Then
                        '    row.DescrizioneTipoDocumento = "Fattura accompagnatoria"
                        '    row.Tipo_Doc = "FA"
                        'End If
                    Case "FA"
                        row.DescrizioneTipoDocumento = "Fattura accompagnatoria"
                    Case "FS"
                        row.DescrizioneTipoDocumento = "Fattura con scontrino"
                    Case "NC"
                        row.DescrizioneTipoDocumento = "Nota di credito"
                    Case "NZ"
                        row.DescrizioneTipoDocumento = "Nota corrispondenza"
                    Case "MM"
                        row.DescrizioneTipoDocumento = "Movimento di magazzino"
                    Case "CM"
                        row.DescrizioneTipoDocumento = "Carico di magazzino"
                    Case "SM"
                        row.DescrizioneTipoDocumento = "Scarico di magazzino"
                    Case Else
                        row.DescrizioneTipoDocumento = ""
                End Select
                row.EndEdit()
            Next
            dsMovMag1.AcceptChanges()
        Catch ex As Exception
            Errore = ex.Message & " - Stampa movimento di magazzino"
            StampaMovMag = False
            Exit Function
        End Try
    End Function

    Public Function StampaFattAgenteAnalitico(ByVal Analitico As Boolean, ByVal CodiceAgente As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal _Esercizio As String, ByRef DSFattAgente1 As dsFattAgente, ByRef ObjReport As Object, ByRef Errore As String, ByVal OmettiNonDefinito As Boolean, ByVal OmettiDaConcordare As Boolean) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConn1 As SqlConnection
        Dim SqlConn2 As SqlConnection
        Dim SqlAdapStatFattAgenteAnalitico As SqlDataAdapter
        Dim SqlDbSelectStatFattAgenteAnalitico As SqlCommand
        Try
            DSFattAgente1.Clear()

            SqlConn1 = New SqlConnection
            SqlConn2 = New SqlConnection
            SqlAdapStatFattAgenteAnalitico = New SqlDataAdapter
            SqlDbSelectStatFattAgenteAnalitico = New SqlCommand


            Dim SqlAdapAgenti As SqlDataAdapter
            Dim SqlDbSelectAgenti As SqlCommand
            SqlAdapAgenti = New SqlDataAdapter
            SqlDbSelectAgenti = New SqlCommand

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
            SqlDbSelectStatFattAgenteAnalitico.CommandTimeout = myTimeOUT
            SqlDbSelectAgenti.CommandTimeout = myTimeOUT
            '---------------------------
            SqlAdapStatFattAgenteAnalitico.SelectCommand = SqlDbSelectStatFattAgenteAnalitico
            SqlConn1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatFattAgenteAnalitico.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectStatFattAgenteAnalitico.CommandText = "get_FatturatoAgente"
            SqlDbSelectStatFattAgenteAnalitico.Connection = SqlConn1

            SqlDbSelectStatFattAgenteAnalitico.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatFattAgenteAnalitico.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodAgente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatFattAgenteAnalitico.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatFattAgenteAnalitico.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            If IsDate(txtDataDa.Trim) Then
                SqlDbSelectStatFattAgenteAnalitico.Parameters.Item("@DataDa").Value = Format(CDate(txtDataDa), FormatoData)
            Else
                txtDataDa = ""
            End If
            If IsDate(txtDataA.Trim) Then
                SqlDbSelectStatFattAgenteAnalitico.Parameters.Item("@DataA").Value = Format(CDate(txtDataA), FormatoData)
            Else
                txtDataA = ""
            End If
            If CodiceAgente <> -1 Then
                SqlDbSelectStatFattAgenteAnalitico.Parameters.Item("@CodAgente").Value = CodiceAgente
            Else
                If OmettiNonDefinito Then
                    SqlDbSelectStatFattAgenteAnalitico.Parameters.Item("@CodAgente").Value = -1
                End If
            End If
            SqlAdapStatFattAgenteAnalitico.Fill(DSFattAgente1.get_FatturatoAgente)

            'Agenti
            SqlAdapAgenti.SelectCommand = SqlDbSelectAgenti
            SqlConn2.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftCoge)
            SqlDbSelectAgenti.CommandType = System.Data.CommandType.Text
            If CodiceAgente > 0 Then 'singolo
                SqlDbSelectAgenti.CommandText = "SELECT Codice, Descrizione FROM Agenti WHERE Codice = " & CodiceAgente
            ElseIf CodiceAgente = -1 Then 'tutti+non definito
                SqlDbSelectAgenti.CommandText = "SELECT 0 AS Codice , '   Agente non definito' AS Descrizione UNION SELECT Codice, Descrizione FROM Agenti"
            Else 'non definito
                SqlDbSelectAgenti.CommandText = "SELECT 0 AS Codice , '   Agente non definito' AS Descrizione "
            End If
            SqlDbSelectAgenti.Connection = SqlConn2
            SqlAdapAgenti.Fill(DSFattAgente1.get_Agenti)


            '-------

            ''Aliquote IVA
            'Dim SqlAdapAliquoteIVA As New SqlDataAdapter
            'Dim SqlDbSelectAliquoteIVA As New SqlCommand
            'SqlAdapAliquoteIVA.SelectCommand = SqlDbSelectAliquoteIVA
            'SqlDbSelectAliquoteIVA.CommandType = System.Data.CommandType.Text
            'SqlDbSelectAliquoteIVA.CommandText = "SELECT Aliquota, AliqIVA FROM Aliquote_IVA "

            'SqlDbSelectAliquoteIVA.Connection = SqlConn2
            'SqlAdapAliquoteIVA.Fill(DSFattAgente1.Aliquote_IVA)

            ''-------

            If DSFattAgente1.get_FatturatoAgente.Count > 0 Then
                For Each row As dsFattAgente.get_FatturatoAgenteRow In DSFattAgente1.get_FatturatoAgente.Rows
                    row.AziendaReport = _Esercizio
                    If Analitico Then
                        row.TitoloReport = "Fatturato per agente analitico - Dal " & txtDataDa & " al " & txtDataA
                    Else
                        row.TitoloReport = "Fatturato per agente sintetico - Dal " & txtDataDa & " al " & txtDataA
                    End If
                    If CodiceAgente > 0 Then 'singolo
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " - [Singolo agente]"
                    ElseIf CodiceAgente = -1 Then 'tutti+non definito
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " - [Tutti gli agenti]"
                    Else 'non definito
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " - [Agente non definito]"
                    End If
                    row.ProvvDaConc = Not OmettiDaConcordare
                Next
            End If

        Catch ex As Exception
            Errore = ex.Message & " - Stampa Fatturato per agente" & IIf(Analitico = True, " analitico", " sintetico")
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'Alberto 16/05/2012 GIU110619 OK SU PIU ANNI
    Public Function StampaStatisticheVendutoArticoloClienteAG(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodCliente As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Ordinamento As Integer, ByVal Statistica As Integer, ByVal VisualizzaPrezzoVendita As Boolean, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodAgente As Integer) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            '
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            '-
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            '---------------------------
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            'giu051112 SONO IDENTICHE QUINDI USO SEMPRE LA SP:get_StaVendCliArtPrezzoVenditaAG
            ' ''If VisualizzaPrezzoVendita Then
            ' ''    SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVenditaAG"
            ' ''Else
            ' ''    SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtNOPrezzoVenditaAG"
            ' ''End If
            SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVenditaAG"
            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodAgente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodCliente <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = Controlla_Apice(CodCliente)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = System.DBNull.Value
            End If
            'giu020216
            If Ordinamento = 6 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Ordinamento").Value = 1
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Ordinamento").Value = 2
            End If
            ' ''SqlDbSelectStatVendCliArt.Parameters.Item("@Ordinamento").Value = Ordinamento
            '---------
            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica

            If CodAgente <> -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodAgente").Value = CodAgente
            End If
            'GIU080619
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                If Statistica = 1 Then 'fatturato
                    strSQL = "Select * From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL = "Select * From DaFatturareClientiArticolo("
                Else
                    strSQL = "Select * From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodCliente <> "" Then
                    strSQL += "'" & Controlla_Apice(CodCliente) & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodAgente <> -1 Then
                    strSQL += CodAgente.ToString.Trim & ",NULL) AS Expr1 " & vbCr 'Agente,CodCateg
                Else
                    strSQL += "NULL,NULL) AS Expr1 " & vbCr 'Agente,CodCateg
                End If

                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto  per agente / cliente / articolo"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select Codice, Descrizione From Agenti"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Agenti")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto  per agente / cliente / articolo AGENTI"
                    Return False
                    Exit Function
                End Try
                ObjDB = Nothing
            End If
            '---------
            Dim rowAgenti As DsStatVendCliArt.AgentiRow = Nothing
            If DSStatVendCliArt1.StCliArt.Count > 0 Then
                For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                    row.Azienda = _Esercizio
                    If Ordinamento = TIPOSTAMPASTATISTICA.VendutoClienteArticoloAG Then
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica articoli venduti / fatturati per agente / cliente / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica articoli fatturati per agente / cliente / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica articoli da fatturare per agente / cliente / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                    ElseIf Ordinamento = TIPOSTAMPASTATISTICA.VendutoArticoloClienteAG Then
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica articoli venduti / fatturati per agente / articolo / cliente (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica articoli fatturati per agente / articolo / cliente (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica articoli da fatturare per agente / articolo / cliente (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                    End If
                    row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                    If VisualizzaPrezzoVendita Then
                        row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                    Else
                        row.VisualizzaPrezzoVendita = False
                    End If
                    'giu080619
                    If SWStatVendOLD = True Then
                        'OK
                    Else
                        If row.IsAgenteNull Then
                            row.DesAgente = "Sconosciuto"
                        Else
                            rowAgenti = DSStatVendCliArt1.Agenti.FindByCodice(row.Agente)
                            If Not rowAgenti Is Nothing Then
                                row.DesAgente = rowAgenti.Descrizione
                            Else
                                row.DesAgente = "Sconosciuto " & row.Agente.ToString.Trim
                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto Per Cliente / Articolo per agente"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    Public Function StampaVendutoDDTAG(ByVal AzReport As String, ByRef DataS As DSStatRiepVendutoNumero, ByRef Errore As String, ByVal TipoStatistica As Integer, ByVal DaDDT As Integer, ByVal ADDT As Integer, ByVal DataDa As String, ByVal DataA As String, ByVal CodAgente As Integer) As Boolean
        StampaVendutoDDTAG = True
        Dim clsDB As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection

        Try
            SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)

            'alberto 11/05/2012 
            SQLCmd.CommandText = "[get_VendutoDDTPerStampaAG]" 'alberto 11/05/2012

            'alberto 11/05/2012 SQLCmd.CommandType = CommandType.Text
            SQLCmd.CommandType = CommandType.StoredProcedure 'alberto 11/05/2012
            SQLCmd.Connection = SQLConn

            'alberto 11/05/2012
            SQLCmd.Parameters.Add(New SqlParameter("@TipoStatistica", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@DaDDT", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@ADDT", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@DataDa", SqlDbType.NVarChar, 10))
            SQLCmd.Parameters.Add(New SqlParameter("@DataA", SqlDbType.NVarChar, 10))
            SQLCmd.Parameters.Add(New SqlParameter("@CodAgente", SqlDbType.Int, 4))

            SQLCmd.Parameters("@TipoStatistica").Value = TipoStatistica
            SQLCmd.Parameters("@DaDDT").Value = DaDDT
            SQLCmd.Parameters("@ADDT").Value = ADDT

            If DataDa <> "" Then
                SQLCmd.Parameters("@DataDa").Value = DataDa
            End If

            If DataA <> "" Then
                SQLCmd.Parameters("@DataA").Value = DataA
            End If

            If CodAgente <> -1 Then
                SQLCmd.Parameters("@CodAgente").Value = CodAgente
            End If
            '----------------

            SQLAdp.SelectCommand = SQLCmd
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
            SQLAdp.SelectCommand.CommandTimeout = myTimeOUT
            '---------------------------
            SQLAdp.Fill(DataS.VendutoNumero)

            For Each row As DSStatRiepVendutoNumero.VendutoNumeroRow In DataS.VendutoNumero.Rows
                row.BeginEdit()
                row.Ditta = AzReport
                row.EndEdit()
            Next

            DataS.AcceptChanges()
        Catch ex As Exception
            Errore = ex.Message & " - Stampa venduto per DDT."
            StampaVendutoDDTAG = False
            Exit Function
        End Try
    End Function

    Public Function StampaStatisticheIncidenzaNCFatturato(ByVal CodCliente1 As String, ByVal CodCliente2 As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal Ordine As Integer) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            '---------------------------
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectStatVendCliArt.CommandText = "get_IncNCFatt"
            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente1", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente2", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordine", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodCliente1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente1").Value = Controlla_Apice(CodCliente1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente1").Value = System.DBNull.Value
            End If
            If CodCliente2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente2").Value = Controlla_Apice(CodCliente2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente2").Value = System.DBNull.Value
            End If

            SqlDbSelectStatVendCliArt.Parameters.Item("@Ordine").Value = Ordine

            SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.get_IncNCFatt)

            If DSStatVendCliArt1.get_IncNCFatt.Count > 0 Then
                For Each row As DsStatVendCliArt.get_IncNCFattRow In DSStatVendCliArt1.get_IncNCFatt.Rows
                    row.Azienda = _Esercizio
                    row.TitoloReport = "Incidenza Note di Credito sul fatturato/Analisi ABC (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                Next
            End If

        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto Per Cliente / Articolo"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'GIU110619 VENDUTO SU PIU' ANNI
    Public Function StampaStatisticheVendutoFornitoreArticolo(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodFornitore As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Statistica As Integer, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            '---------------------------
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            '-
            SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVenditaByFor"

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodFornitore", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodFornitore <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodFornitore").Value = Controlla_Apice(CodFornitore)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodFornitore").Value = System.DBNull.Value
            End If
            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica
            'GIU110619
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                strSQL += "SELECT TabUnione.Codice_CoGe, TabUnione.Rag_Soc, TabUnione.Cod_Articolo, TabUnione.Descrizione,TotQta," &
                            "TabUnione.PrezzoListino, TabUnione.PrezzoVendita, TabUnione.Imponibile,TabUnione.Data_Ultimo," &
                            "TabUnione.Segno,Agente, TabUnione.Cod_Categoria, TabUnione.Provincia, ISNULL(AnaMag.CodiceFornitore,'') AS CodiceFornitore "
                If Statistica = 1 Then 'fatturato
                    strSQL += "From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL += "From DaFatturareClientiArticolo("
                Else
                    strSQL += "From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL," 'CLIENTE
                strSQL += "NULL," 'AGENTE
                strSQL += "NULL) AS TabUnione " & vbCr 'CodCateg
                Dim SWWhere As Boolean = False
                If CodFornitore <> "" Then
                    SWWhere = True
                    strSQL += " LEFT OUTER JOIN AnaMag ON TabUnione.Cod_Articolo = AnaMag.Cod_Articolo " & vbCr
                    strSQL += " WHERE (AnaMag.CodiceFornitore='" & CodFornitore.Trim & "')"
                Else
                    strSQL += " LEFT OUTER JOIN AnaMag ON TabUnione.Cod_Articolo = AnaMag.Cod_Articolo " & vbCr
                End If
                '-
                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per fornitore / articolo"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select Codice_Coge, Rag_Soc From Fornitori"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Fornitori")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per fornitore / articolo FORNITORI"
                    Return False
                    Exit Function
                End Try
                '-
                ObjDB = Nothing
            End If
            '---------
            Dim rowFornitori As DsStatVendCliArt.FornitoriRow = Nothing
            If DSStatVendCliArt1.StCliArt.Count > 0 Then
                For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                    row.Azienda = _Esercizio
                    If Statistica = 0 Then
                        row.TitoloReport = "Statistica articoli venduti / fatturati per fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf Statistica = 1 Then
                        row.TitoloReport = "Statistica articoli fatturati per fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf Statistica = 2 Then
                        row.TitoloReport = "Statistica articoli da fatturare per fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    End If
                    row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                    ' ''If VisualizzaPrezzoVendita Then
                    ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                    ' ''Else
                    ' ''    row.VisualizzaPrezzoVendita = False
                    ' ''End If
                    row.VisualizzaPrezzoVendita = True 'fisso
                    If SWStatVendOLD = False Then 'giu211019 altrimenti mette sconosciuto xkè non caricato prima
                        If row.IsCodiceFornitoreNull Then
                            row.Fornitore = "Sconosciuto"
                        Else
                            rowFornitori = DSStatVendCliArt1.Fornitori.FindByCodice_Coge(row.CodiceFornitore)
                            If Not rowFornitori Is Nothing Then
                                row.Fornitore = rowFornitori.Rag_Soc.Trim
                            Else
                                row.Fornitore = "Sconosciuto " & row.CodiceFornitore.Trim
                            End If
                        End If
                    End If
                    '-
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto per fornitore / articolo / cliente "
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'giu020216 GIU070619 SU PIU ANNI
    Public Function StampaStatisticheVendutoArticoloClienteCC(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodCliente As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Ordinamento As Integer, ByVal Statistica As Integer, ByVal VisualizzaPrezzoVendita As Boolean, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodCatCli As Integer, ByVal strCategRagg As String, ByVal AccorpaCR As Boolean) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            'GIU070619 @@@@@@@@@@@@@@@@@ ADAPTER E' LA VECCHIA RELEASE CHE ESTRAE DATI DAL SOLO ANNO CORRENTE QUINDI NON PIU USATA
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            '---------------------------
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            If strCategRagg.Trim <> "" Then
                If AccorpaCR = False Then
                    SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVenditaCCRaggr"
                Else
                    SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVenditaCCRA"
                End If
            Else
                SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliArtPrezzoVenditaCC"
            End If

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinamento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCatCli", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RaggrCatCli", System.Data.SqlDbType.NVarChar, 30, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodCliente <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = Controlla_Apice(CodCliente)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = System.DBNull.Value
            End If
            If Ordinamento = 15 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Ordinamento").Value = 1
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Ordinamento").Value = 2
            End If

            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica

            If CodCatCli <> -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCatCli").Value = CodCatCli
            End If
            If strCategRagg.Trim <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@RaggrCatCli").Value = strCategRagg & "%"
            End If

            'giu070619
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                If Statistica = 1 Then 'fatturato
                    strSQL = "Select * From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL = "Select * From DaFatturareClientiArticolo("
                Else
                    strSQL = "Select * From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodCliente <> "" Then
                    strSQL += "'" & Controlla_Apice(CodCliente) & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL," 'AGENTE QUI NON RICHIESTO
                If CodCatCli <> -1 And strCategRagg.Trim = "" Then
                    strSQL += CodCatCli.ToString.Trim & ") AS Expr1 " & vbCr 'CodCateg
                Else
                    strSQL += "NULL) AS Expr1 " & vbCr 'CodCateg
                End If
                Dim SWWhere As Boolean = False
                If strCategRagg.Trim <> "" Then
                    SWWhere = True
                    strSQL += " WHERE Cod_Categoria IN (SELECT Categorie.Codice FROM Categorie WHERE (Categorie.Descrizione LIKE '" & strCategRagg & "%'))"
                End If
                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per categoria / Articolo"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select * From Categorie"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Categorie")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per categoria / Articolo CATEGORIA"
                    Return False
                    Exit Function
                End Try
                ObjDB = Nothing
            End If
            '---------
            Dim rowCategorie As DsStatVendCliArt.CategorieRow = Nothing
            If DSStatVendCliArt1.StCliArt.Count > 0 Then
                For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                    row.Azienda = _Esercizio
                    If Statistica = 0 Then
                        row.TitoloReport = "Statistica articoli venduti / fatturati per categoria (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf Statistica = 1 Then
                        row.TitoloReport = "Statistica articoli fatturati per categoria  (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf Statistica = 2 Then
                        row.TitoloReport = "Statistica articoli da fatturare per categoria (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    End If
                    row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                    If VisualizzaPrezzoVendita Then
                        row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                    Else
                        row.VisualizzaPrezzoVendita = False
                    End If
                    If SWStatVendOLD = False Then 'per ora per tutti  2 Then
                        If AccorpaCR = False Then
                            rowCategorie = DSStatVendCliArt1.Categorie.FindByCodice(row.Cod_Categoria)
                            If Not rowCategorie Is Nothing Then
                                row.Categoria = rowCategorie.Descrizione
                            Else
                                row.Categoria = "Sconosciuta " & row.Cod_Categoria
                            End If
                        Else
                            row.Cod_Categoria = 999
                            row.Categoria = strCategRagg.Trim & " (Accorpata)"
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto per categoria / Cliente / Articolo"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'giu030216 giu100619 estrazioni su piu anni
    Public Function StampaStatisticheVendutoAgenteForArt(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodFornitore As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Statistica As Integer, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodAgente As Integer, Optional ByVal Regione As Integer = -1) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            '---------------------------
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            '-
            SqlDbSelectStatVendCliArt.CommandText = "get_StaVendAgForArtPrezzoVenditaByFor"

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodFornitore", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodAgente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodRegione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodFornitore <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodFornitore").Value = Controlla_Apice(CodFornitore)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodFornitore").Value = System.DBNull.Value
            End If
            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica
            If CodAgente = -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodAgente").Value = System.DBNull.Value
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodAgente").Value = CodAgente
            End If
            If Regione = -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = System.DBNull.Value
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = Regione
            End If
            'GIU110619
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                strSQL += "SELECT TabUnione.Codice_CoGe, TabUnione.Rag_Soc, TabUnione.Cod_Articolo, TabUnione.Descrizione,TotQta," &
                            "TabUnione.PrezzoListino, TabUnione.PrezzoVendita, TabUnione.Imponibile,TabUnione.Data_Ultimo," &
                            "TabUnione.Segno,Agente, TabUnione.Cod_Categoria, TabUnione.Provincia, ISNULL(AnaMag.CodiceFornitore,'') AS CodiceFornitore "
                If Statistica = 1 Then 'fatturato
                    strSQL += "From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL += "From DaFatturareClientiArticolo("
                Else
                    strSQL += "From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL," 'CLIENTE
                If CodAgente <> -1 Then
                    strSQL += CodAgente.ToString.Trim & ","
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL) AS TabUnione " & vbCr 'CodCateg
                Dim SWWhere As Boolean = False
                If CodFornitore <> "" Then
                    SWWhere = True
                    strSQL += " LEFT OUTER JOIN AnaMag ON TabUnione.Cod_Articolo = AnaMag.Cod_Articolo " & vbCr
                    strSQL += " WHERE (AnaMag.CodiceFornitore='" & CodFornitore.Trim & "')"
                Else
                    strSQL += " LEFT OUTER JOIN AnaMag ON TabUnione.Cod_Articolo = AnaMag.Cod_Articolo " & vbCr
                End If
                If Regione <> 0 And Regione <> -1 Then
                    If SWWhere = True Then
                        strSQL += " AND Provincia IN (SELECT  Province.Codice FROM Province INNER JOIN Regioni ON Province.Regione = Regioni.Codice WHERE (Province.Regione = " & Regione.ToString.Trim & "))"
                    Else
                        strSQL += " WHERE Provincia IN (SELECT  Province.Codice FROM Province INNER JOIN Regioni ON Province.Regione = Regioni.Codice WHERE (Province.Regione = " & Regione.ToString.Trim & "))"
                    End If
                End If
                '-
                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Agente / Regione / fornitore / articolo / cliente / articolo"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select Codice_Coge, Rag_Soc From Fornitori"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Fornitori")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Agente / Regione / fornitore / articolo / cliente FORNITORI"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Regioni"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Regioni")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Agente / Regione / fornitore / articolo / cliente REGIONI"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Province"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Province")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Agente / Regione / fornitore / articolo / cliente  PROVINCE"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Agenti"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Agenti")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Agente / Regione / fornitore / articolo / cliente  AGENTI"
                    Return False
                    Exit Function
                End Try
                '-
                ObjDB = Nothing
            End If
            '---------
            Dim rowFornitori As DsStatVendCliArt.FornitoriRow = Nothing
            Dim rowRegione As DsStatVendCliArt.RegioniRow = Nothing
            Dim rowProvince As DsStatVendCliArt.ProvinceRow = Nothing
            Dim rowAgenti As DsStatVendCliArt.AgentiRow = Nothing
            If DSStatVendCliArt1.StCliArt.Count > 0 Then
                For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                    row.Azienda = _Esercizio
                    If SWStatVendOLD = False Then 'per ora per tutti  2 Then
                        If row.IsProvinciaNull Then
                            row.DesRegione = "Sconosciuta"
                        Else
                            rowProvince = DSStatVendCliArt1.Province.FindByCodice(row.Provincia)
                            If Not rowProvince Is Nothing Then
                                row.DesProvincia = rowProvince.Descrizione
                                rowRegione = DSStatVendCliArt1.Regioni.FindByCodice(rowProvince.Regione)
                                If Not rowRegione Is Nothing Then
                                    row.Regione = rowProvince.Regione
                                    row.DesRegione = rowRegione.Descrizione
                                Else
                                    row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                                End If
                            Else
                                row.DesProvincia = "Sconosciuta " & row.Provincia.Trim
                                row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                            End If
                        End If
                        '-
                        If row.IsCodiceFornitoreNull Then
                            row.Fornitore = "Sconosciuto"
                        Else
                            rowFornitori = DSStatVendCliArt1.Fornitori.FindByCodice_Coge(row.CodiceFornitore)
                            If Not rowFornitori Is Nothing Then
                                row.Fornitore = rowFornitori.Rag_Soc.Trim
                            Else
                                row.Fornitore = "Sconosciuto " & row.CodiceFornitore.Trim
                            End If
                        End If
                        '-
                        If row.IsAgenteNull Then
                            row.DesAgente = "Sconosciuto"
                        Else
                            rowAgenti = DSStatVendCliArt1.Agenti.FindByCodice(row.Agente)
                            If Not rowAgenti Is Nothing Then
                                row.DesAgente = rowAgenti.Descrizione.Trim
                            Else
                                row.DesAgente = "Sconosciuto " & row.Agente.ToString.Trim
                            End If
                        End If
                    End If
                    '--- altrimenti non compare la regione nella TESTATA
                    If Statistica = 0 Then
                        row.TitoloReport = "Statistica articoli venduti / fatturati per agente " & IIf(Regione = -1, "", "/ regione ") & " / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ") " & IIf(Regione < 1, "", row.DesRegione.Trim)
                    ElseIf Statistica = 1 Then
                        row.TitoloReport = "Statistica articoli fatturati per agente " & IIf(Regione = -1, "", "/ regione ") & "/ fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ") " & IIf(Regione < 1, "", row.DesRegione.Trim)
                    ElseIf Statistica = 2 Then
                        row.TitoloReport = "Statistica articoli da fatturare per agente " & IIf(Regione = -1, "", "/ regione ") & "/ fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ") " & IIf(Regione < 1, "", row.DesRegione.Trim)
                    End If
                    row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                    ' ''If VisualizzaPrezzoVendita Then
                    ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                    ' ''Else
                    ' ''    row.VisualizzaPrezzoVendita = False
                    ' ''End If
                    row.VisualizzaPrezzoVendita = True 'fisso
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto per Agente / Regione / fornitore / articolo / cliente "
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'FABIO 02032016 GIU110619
    Public Function StampaStatisticheVendutoRegioneForArt(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodFornitore As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Statistica As Integer, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodRegione As Integer) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            '---------------------------
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            '-
            SqlDbSelectStatVendCliArt.CommandText = "get_StaVendRegForArtPrezzoVenditaByFor"

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodFornitore", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodRegione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodFornitore <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodFornitore").Value = Controlla_Apice(CodFornitore)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodFornitore").Value = System.DBNull.Value
            End If
            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica
            If CodRegione = -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = System.DBNull.Value
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = CodRegione
            End If
            'GIU110619 giu211019
            Dim ObjDB As New DataBaseUtility
            Dim strSQL As String = ""
            '-
            If SWStatVendOLD Then
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                strSQL += "SELECT TabUnione.Codice_CoGe, TabUnione.Rag_Soc, TabUnione.Cod_Articolo, TabUnione.Descrizione,TotQta," &
                            "TabUnione.PrezzoListino, TabUnione.PrezzoVendita, TabUnione.Imponibile,TabUnione.Data_Ultimo," &
                            "TabUnione.Segno,Agente, TabUnione.Cod_Categoria, TabUnione.Provincia, ISNULL(AnaMag.CodiceFornitore,'') AS CodiceFornitore "
                If Statistica = 1 Then 'fatturato
                    strSQL += "From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL += "From DaFatturareClientiArticolo("
                Else
                    strSQL += "From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL," 'CLIENTE
                strSQL += "NULL," 'AGENTE
                strSQL += "NULL) AS TabUnione " & vbCr 'CodCateg
                Dim SWWhere As Boolean = False
                If CodFornitore <> "" Then
                    SWWhere = True
                    strSQL += " LEFT OUTER JOIN AnaMag ON TabUnione.Cod_Articolo = AnaMag.Cod_Articolo " & vbCr
                    strSQL += " WHERE (AnaMag.CodiceFornitore='" & CodFornitore.Trim & "')"
                Else
                    strSQL += " LEFT OUTER JOIN AnaMag ON TabUnione.Cod_Articolo = AnaMag.Cod_Articolo " & vbCr
                End If
                If CodRegione <> 0 And CodRegione <> -1 Then
                    If SWWhere = True Then
                        strSQL += " AND Provincia IN (SELECT  Province.Codice FROM Province INNER JOIN Regioni ON Province.Regione = Regioni.Codice WHERE (Province.Regione = " & CodRegione.ToString.Trim & "))"
                    Else
                        strSQL += " WHERE Provincia IN (SELECT  Province.Codice FROM Province INNER JOIN Regioni ON Province.Regione = Regioni.Codice WHERE (Province.Regione = " & CodRegione.ToString.Trim & "))"
                    End If
                End If
                '-
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Regione / fornitore / articolo "
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select Codice_Coge, Rag_Soc From Fornitori"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Fornitori")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Regione / fornitore / articolo FORNITORI"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Regioni"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Regioni")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Regione / fornitore / articolo REGIONI"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Province"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Province")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per Regione / fornitore / articolo PROVINCE"
                    Return False
                    Exit Function
                End Try
                '-
            End If
            ObjDB = Nothing
            '---------
            Dim rowFornitori As DsStatVendCliArt.FornitoriRow = Nothing
            Dim rowRegione As DsStatVendCliArt.RegioniRow = Nothing
            Dim rowProvince As DsStatVendCliArt.ProvinceRow = Nothing
            If DSStatVendCliArt1.StCliArt.Count > 0 Then
                For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                    row.Azienda = _Esercizio
                    If Statistica = 0 Then
                        row.TitoloReport = "Statistica articoli venduti / fatturati per regione / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf Statistica = 1 Then
                        row.TitoloReport = "Statistica articoli fatturati per regione / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf Statistica = 2 Then
                        row.TitoloReport = "Statistica articoli da fatturare per regione / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    End If
                    row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                    ' ''If VisualizzaPrezzoVendita Then
                    ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                    ' ''Else
                    ' ''    row.VisualizzaPrezzoVendita = False
                    ' ''End If
                    row.VisualizzaPrezzoVendita = True 'fisso
                    If SWStatVendOLD = False Then
                        If row.IsProvinciaNull Then
                            row.Regione = 99
                            row.DesRegione = "Sconosciuta"
                        Else
                            rowProvince = DSStatVendCliArt1.Province.FindByCodice(row.Provincia)
                            If Not rowProvince Is Nothing Then
                                row.DesProvincia = rowProvince.Descrizione
                                rowRegione = DSStatVendCliArt1.Regioni.FindByCodice(rowProvince.Regione)
                                If Not rowRegione Is Nothing Then
                                    row.Regione = rowRegione.Codice
                                    row.DesRegione = rowRegione.Descrizione
                                Else
                                    row.Regione = 99
                                    row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                                End If
                            Else
                                row.Regione = 99
                                row.DesProvincia = "Sconosciuta " & row.Provincia.Trim
                                row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                            End If
                        End If
                        '-
                        If row.IsCodiceFornitoreNull Then
                            row.CodiceFornitore = "Z"
                            row.Fornitore = "Sconosciuto"
                        Else
                            rowFornitori = DSStatVendCliArt1.Fornitori.FindByCodice_Coge(row.CodiceFornitore)
                            If Not rowFornitori Is Nothing Then
                                row.Fornitore = rowFornitori.Rag_Soc.Trim
                            Else
                                row.Fornitore = "Sconosciuto " & row.CodiceFornitore.Trim
                            End If
                        End If
                        '-
                    End If
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto per regione / fornitore / articolo"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'FABIO 02032016 GIU070619 DATI SU PIU ANNI
    Public Function StampaStatisticheVendutoClienteForArt(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodFornitore As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Statistica As Integer, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodCliente As String, ByVal sintetico As Boolean) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            '---------------------------
            'GIU070619 @@@@@@@@@@@@@@@@@ ADAPTER E' LA VECCHIA RELEASE CHE ESTRAE DATI DAL SOLO ANNO CORRENTE QUINDI NON PIU USATA
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            '-
            SqlDbSelectStatVendCliArt.CommandText = "get_StaVendCliForArtPrezzoVenditaByFor"

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodFornitore", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodFornitore <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodFornitore").Value = Controlla_Apice(CodFornitore)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodFornitore").Value = System.DBNull.Value
            End If
            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica
            If Not IsNumeric(CodCliente.Trim) Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = System.DBNull.Value
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = CodCliente.Trim
            End If
            '------ GIU
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                strSQL += "SELECT TabUnione.Codice_CoGe, TabUnione.Rag_Soc, TabUnione.Cod_Articolo, TabUnione.Descrizione,TotQta," &
                            "TabUnione.PrezzoListino, TabUnione.PrezzoVendita, TabUnione.Imponibile,TabUnione.Data_Ultimo," &
                            "TabUnione.Segno,Agente, TabUnione.Cod_Categoria, TabUnione.Provincia, ISNULL(AnaMag.CodiceFornitore,'') AS CodiceFornitore "
                If Statistica = 1 Then 'fatturato
                    strSQL += "From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL += "From DaFatturareClientiArticolo("
                Else
                    strSQL += "From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodCliente <> "" Then
                    strSQL += "'" & Controlla_Apice(CodCliente) & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL,NULL) AS TabUnione " & vbCr 'Agente,CodCateg
                If CodFornitore <> "" Then
                    strSQL += " LEFT OUTER JOIN AnaMag ON TabUnione.Cod_Articolo = AnaMag.Cod_Articolo " & vbCr
                    strSQL += " WHERE (AnaMag.CodiceFornitore='" & CodFornitore.Trim & "')"
                Else
                    strSQL += " LEFT OUTER JOIN AnaMag ON TabUnione.Cod_Articolo = AnaMag.Cod_Articolo " & vbCr
                End If
                '-
                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per cliente / fornitore / articolo"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select Codice_Coge, Rag_Soc From Fornitori"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Fornitori")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto Per Cliente / Articolo FORNITORI"
                    Return False
                    Exit Function
                End Try
                '-
                ObjDB = Nothing
            End If
            '---------
            Dim rowFornitori As DsStatVendCliArt.FornitoriRow = Nothing
            If sintetico = False Then 'FABIO 19022016
                If DSStatVendCliArt1.StCliArt.Count > 0 Then
                    For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                        row.Azienda = _Esercizio
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica articoli venduti / fatturati per cliente / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica articoli fatturati per cliente / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica articoli da fatturare per cliente / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                        ' ''If VisualizzaPrezzoVendita Then
                        ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                        ' ''Else
                        ' ''    row.VisualizzaPrezzoVendita = False
                        ' ''End If
                        row.VisualizzaPrezzoVendita = True 'fisso
                        If SWStatVendOLD = False Then
                            If row.IsCodiceFornitoreNull Then
                                row.Fornitore = "Sconosciuto "
                            ElseIf row.CodiceFornitore.Trim = "" Then
                                row.Fornitore = "Sconosciuto "
                            Else
                                rowFornitori = DSStatVendCliArt1.Fornitori.FindByCodice_Coge(row.CodiceFornitore)
                                If Not rowFornitori Is Nothing Then
                                    row.Fornitore = rowFornitori.Rag_Soc
                                Else
                                    row.Fornitore = "Sconosciuto " & row.CodiceFornitore
                                End If
                            End If
                            '-
                        End If
                    Next
                End If
            Else 'FABIO 19022016
                If DSStatVendCliArt1.StCliArt.Count > 0 Then
                    For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                        row.Azienda = _Esercizio
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica sintetico articoli venduti / fatturati per cliente / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica sintetico articoli fatturati per cliente / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica sintetico articoli da fatturare per cliente / fornitore / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                        ' ''If VisualizzaPrezzoVendita Then
                        ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                        ' ''Else
                        ' ''    row.VisualizzaPrezzoVendita = False
                        ' ''End If
                        row.VisualizzaPrezzoVendita = True 'fisso
                        If SWStatVendOLD = False Then
                            If row.IsCodiceFornitoreNull Then
                                row.Fornitore = "Sconosciuto "
                            ElseIf row.CodiceFornitore.Trim = "" Then
                                row.Fornitore = "Sconosciuto "
                            Else
                                rowFornitori = DSStatVendCliArt1.Fornitori.FindByCodice_Coge(row.CodiceFornitore)
                                If Not rowFornitori Is Nothing Then
                                    row.Fornitore = rowFornitori.Rag_Soc
                                Else
                                    row.Fornitore = "Sconosciuto " & row.CodiceFornitore
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto per cliente / fornitore / articolo "
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'FABIO 08032016 giu080619 dati su piu' anni
    Public Function StampaStatisticheVendutoAgenteCategArt(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodCateg As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Statistica As Integer, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodAgente As Integer, ByVal sintetico As Boolean) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            '-
            '---------------------------
            'GIU080619 @@@@@@@@@@@@@@@@@ ADAPTER E' LA VECCHIA RELEASE CHE ESTRAE DATI DAL SOLO ANNO CORRENTE QUINDI NON PIU USATA
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            '-
            SqlDbSelectStatVendCliArt.CommandText = "get_StaVendAgCategArtPrezzoVenditaByCateg"

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCategoria", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodAgente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodCateg <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCategoria").Value = Controlla_Apice(CodCateg)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCategoria").Value = System.DBNull.Value
            End If
            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica
            If CodAgente = -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodAgente").Value = System.DBNull.Value
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodAgente").Value = CodAgente
            End If
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                If Statistica = 1 Then 'fatturato
                    strSQL = "Select * From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL = "Select * From DaFatturareClientiArticolo("
                Else
                    strSQL = "Select * From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL," 'Cliente
                If CodAgente = -1 Then
                    strSQL += "NULL,"
                Else
                    strSQL += CodAgente.ToString.Trim & ","
                End If
                If CodCateg <> "" Then
                    strSQL += CodCateg.Trim & ") AS Expr1 " & vbCr
                Else
                    strSQL += "NULL) AS Expr1 " & vbCr
                End If
                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per agente / categoria / articolo (StCliArt)"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select Codice, Descrizione From Agenti"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Agenti")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per agente / categoria / articolo AGENTI"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select * From Categorie"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Categorie")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per agente / categoria / articolo Categorie"
                    Return False
                    Exit Function
                End Try
                ObjDB = Nothing
            End If
            '---------
            Dim rowAgenti As DsStatVendCliArt.AgentiRow = Nothing
            Dim rowCategorie As DsStatVendCliArt.CategorieRow = Nothing
            If sintetico = False Then
                If DSStatVendCliArt1.StCliArt.Count > 0 Then
                    For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                        row.Azienda = _Esercizio
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica articoli venduti / fatturati per agente / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica articoli fatturati per agente / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica articoli da fatturare per agente / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                        ' ''If VisualizzaPrezzoVendita Then
                        ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                        ' ''Else
                        ' ''    row.VisualizzaPrezzoVendita = False
                        ' ''End If
                        row.VisualizzaPrezzoVendita = True 'fisso
                        If SWStatVendOLD = False Then
                            If row.IsCod_CategoriaNull Then
                                row.Categoria = "Sconosciuta"
                            Else
                                rowCategorie = DSStatVendCliArt1.Categorie.FindByCodice(row.Cod_Categoria)
                                If Not rowCategorie Is Nothing Then
                                    row.Categoria = rowCategorie.Descrizione.Trim
                                Else
                                    row.Categoria = "Sconosciuta " & row.Cod_Categoria.ToString.Trim
                                End If
                            End If
                            '-
                            If row.IsAgenteNull Then
                                row.DesAgente = "Sconosciuto"
                            Else
                                rowAgenti = DSStatVendCliArt1.Agenti.FindByCodice(row.Agente)
                                If Not rowAgenti Is Nothing Then
                                    row.DesAgente = rowAgenti.Descrizione.Trim
                                Else
                                    row.DesAgente = "Sconosciuto " & row.Agente.ToString.Trim
                                End If
                            End If
                        End If
                    Next
                End If
            Else
                If DSStatVendCliArt1.StCliArt.Count > 0 Then
                    For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                        row.Azienda = _Esercizio
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica sintetico articoli venduti / fatturati per agente / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica sintetico articoli fatturati per agente / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica sintetico articoli da fatturare per agente / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                        ' ''If VisualizzaPrezzoVendita Then
                        ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                        ' ''Else
                        ' ''    row.VisualizzaPrezzoVendita = False
                        ' ''End If
                        row.VisualizzaPrezzoVendita = True 'fisso
                        If SWStatVendOLD = False Then
                            If row.IsCod_CategoriaNull Then
                                row.Categoria = "Sconosciuta"
                            Else
                                rowCategorie = DSStatVendCliArt1.Categorie.FindByCodice(row.Cod_Categoria)
                                If Not rowCategorie Is Nothing Then
                                    row.Categoria = rowCategorie.Descrizione.Trim
                                Else
                                    row.Categoria = "Sconosciuta " & row.Cod_Categoria.ToString.Trim
                                End If
                            End If
                            '-
                            If row.IsAgenteNull Then
                                row.DesAgente = "Sconosciuto"
                            Else
                                rowAgenti = DSStatVendCliArt1.Agenti.FindByCodice(row.Agente)
                                If Not rowAgenti Is Nothing Then
                                    row.DesAgente = rowAgenti.Descrizione.Trim
                                Else
                                    row.DesAgente = "Sconosciuto " & row.Agente.ToString.Trim
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto per agente / categoria / articolo "
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'FABIO 09022016 giu070619 dati su piu' anni
    Public Function StampaStatisticheVendutoRegioneCatArt(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodCategoria As Integer, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Statistica As Integer, ByVal _Esercizio As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodRegione As Integer, ByVal sintetico As Boolean) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            '---------------------------
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            'GIU070619 @@@@@@@@@@@@@@@@@ ADAPTER E' LA VECCHIA RELEASE CHE ESTRAE DATI DAL SOLO ANNO CORRENTE QUINDI NON PIU USATA
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            '-
            SqlDbSelectStatVendCliArt.CommandText = "get_StaVendRegCatArtPrezzoVenditaByCat"

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCategoria", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodRegione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)

            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            If CodCategoria = -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCategoria").Value = System.DBNull.Value
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCategoria").Value = CodCategoria
            End If
            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica
            If CodRegione = -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = System.DBNull.Value
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = CodRegione
            End If

            'giu070619 SU PIU' ANNI
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                If Statistica = 1 Then 'fatturato
                    strSQL = "Select * From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL = "Select * From DaFatturareClientiArticolo("
                Else
                    strSQL = "Select * From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL," 'If CodCliente <> "" Then
                strSQL += "NULL," 'Agente
                If CodCategoria = -1 Then
                    strSQL += "NULL) AS Expr1 " & vbCr 'CodCateg
                Else
                    strSQL += CodCategoria.ToString.Trim & ") AS Expr1 " & vbCr 'CodCateg
                End If

                Dim SWWhere As Boolean = False
                If CodRegione <> 0 And CodRegione <> -1 Then
                    SWWhere = True
                    strSQL += " WHERE Provincia IN (SELECT  Province.Codice FROM Province INNER JOIN Regioni ON Province.Regione = Regioni.Codice WHERE (Province.Regione = " & CodRegione.ToString.Trim & "))"
                End If
                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per regione / categoria / articolo"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select * From Regioni"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Regioni")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per regione / categoria / articolo REGIONI"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Province"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Province")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per regione / categoria / articolo PROVINCE"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Categorie"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Categorie")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per categoria / Articolo CATEGORIA"
                    Return False
                    Exit Function
                End Try
                '-
                ObjDB = Nothing
            End If
            '---------
            Dim rowRegione As DsStatVendCliArt.RegioniRow = Nothing
            Dim rowProvince As DsStatVendCliArt.ProvinceRow = Nothing
            Dim rowCategorie As DsStatVendCliArt.CategorieRow = Nothing
            If sintetico = False Then
                If DSStatVendCliArt1.StCliArt.Count > 0 Then
                    For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                        row.Azienda = _Esercizio
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica articoli venduti / fatturati per regione / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica articoli fatturati per regione / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica articoli da fatturare per regione / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                        ' ''If VisualizzaPrezzoVendita Then
                        ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                        ' ''Else
                        ' ''    row.VisualizzaPrezzoVendita = False
                        ' ''End If
                        row.VisualizzaPrezzoVendita = True 'fisso
                        If SWStatVendOLD = False Then 'per ora per tutti  2 Then
                            If row.IsProvinciaNull Then
                                row.DesRegione = "Sconosciuta"
                            Else
                                rowProvince = DSStatVendCliArt1.Province.FindByCodice(row.Provincia)
                                If Not rowProvince Is Nothing Then
                                    row.DesProvincia = rowProvince.Descrizione
                                    rowRegione = DSStatVendCliArt1.Regioni.FindByCodice(rowProvince.Regione)
                                    If Not rowRegione Is Nothing Then
                                        row.Regione = rowProvince.Regione
                                        row.DesRegione = rowRegione.Descrizione
                                    Else
                                        row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                                    End If
                                Else
                                    row.DesProvincia = "Sconosciuta " & row.Provincia.Trim
                                    row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                                End If
                            End If
                            '-
                            If row.IsCod_CategoriaNull Then
                                row.Categoria = "Sconosciuta"
                            Else
                                rowCategorie = DSStatVendCliArt1.Categorie.FindByCodice(row.Cod_Categoria)
                                If Not rowCategorie Is Nothing Then
                                    row.Categoria = rowCategorie.Descrizione.Trim
                                Else
                                    row.Categoria = "Sconosciuta " & row.Cod_Categoria.ToString.Trim
                                End If
                            End If
                        End If
                    Next
                End If
            Else
                If DSStatVendCliArt1.StCliArt.Count > 0 Then
                    For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                        row.Azienda = _Esercizio
                        If Statistica = 0 Then
                            row.TitoloReport = "Statistica sintetico articoli venduti / fatturati per regione / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 1 Then
                            row.TitoloReport = "Statistica sintetico articoli fatturati per regione / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        ElseIf Statistica = 2 Then
                            row.TitoloReport = "Statistica sintetico articoli da fatturare per regione / categoria / articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                        End If
                        row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                        ' ''If VisualizzaPrezzoVendita Then
                        ' ''    row.VisualizzaPrezzoVendita = True 'CAMPO UTILIZZATO PER OMETTERE LA VISUALIZZAZIONE DEL PREZZO DI VENDITA
                        ' ''Else
                        ' ''    row.VisualizzaPrezzoVendita = False
                        ' ''End If
                        row.VisualizzaPrezzoVendita = True 'fisso
                        If SWStatVendOLD = False Then 'per ora per tutti  2 Then
                            If row.IsProvinciaNull Then
                                row.DesRegione = "Sconosciuta"
                            Else
                                rowProvince = DSStatVendCliArt1.Province.FindByCodice(row.Provincia)
                                If Not rowProvince Is Nothing Then
                                    row.DesProvincia = rowProvince.Descrizione
                                    rowRegione = DSStatVendCliArt1.Regioni.FindByCodice(rowProvince.Regione)
                                    If Not rowRegione Is Nothing Then
                                        row.Regione = rowProvince.Regione
                                        row.DesRegione = rowRegione.Descrizione
                                    Else
                                        row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                                    End If
                                Else
                                    row.DesProvincia = "Sconosciuta " & row.Provincia.Trim
                                    row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                                End If
                            End If
                            '-
                            If row.IsCod_CategoriaNull Then
                                row.Categoria = "Sconosciuta"
                            Else
                                rowCategorie = DSStatVendCliArt1.Categorie.FindByCodice(row.Cod_Categoria)
                                If Not rowCategorie Is Nothing Then
                                    row.Categoria = rowCategorie.Descrizione.Trim
                                Else
                                    row.Categoria = "Sconosciuta " & row.Cod_Categoria.ToString.Trim
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto per regione / categoria / articolo"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    'giu310717 Nuova stastistica venduto/fatturato/da fatturare per:
    'regione/provincia/categoria/cliente/articolo
    'GIU110619 OK SU PIU ANNI
    Public Function StampaStatisticheVendutoRegPrCatCliArt(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal txtDataDa As String, ByVal txtDataA As String, ByVal txtDataNC As String, ByVal Statistica As Integer, ByVal Azienda As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodRegione As Integer, ByVal Provincia As String, ByVal CodCatCli As Integer, ByVal strCategRagg As String, ByVal AccorpaCR As Boolean) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            Dim SWStatVendOLD As Boolean = App.GetDatiAbilitazioni(CSTABILAZI, "StatVendOLD", strValore, strErrore)
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            '---------------------------
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectStatVendCliArt.CommandText = "get_StaVendRegPrCatCliArt"

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataDa", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataA", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataNC", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '-
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod1", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod2", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '-
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCategoria", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '-
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Statistica", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '-
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodRegione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Provincia", System.Data.SqlDbType.NVarChar, 2, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '-
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RaggrCatCli", System.Data.SqlDbType.NVarChar, 30, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AccorpaCC", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            'OK ASSEGNO I PARAMETRI
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataDa").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataA").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@DataNC").Value = Format(DateTime.Parse(txtDataNC), FormatoData)
            '-
            If CodArt1 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = Controlla_Apice(CodArt1)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod1").Value = System.DBNull.Value
            End If

            If CodArt2 <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = Controlla_Apice(CodArt2)
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Cod2").Value = System.DBNull.Value
            End If

            SqlDbSelectStatVendCliArt.Parameters.Item("@Statistica").Value = Statistica

            If CodCatCli <> -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCategoria").Value = CodCatCli
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCategoria").Value = System.DBNull.Value
            End If
            If CodRegione <> -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = CodRegione
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodRegione").Value = System.DBNull.Value
            End If
            If Provincia.Trim <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@Provincia").Value = Provincia.Trim
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@Provincia").Value = System.DBNull.Value
            End If
            If strCategRagg.Trim <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@RaggrCatCli").Value = strCategRagg & "%"
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@RaggrCatCli").Value = System.DBNull.Value
            End If
            If AccorpaCR Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@AccorpaCC").Value = 1
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@AccorpaCC").Value = 0
            End If
            'GIU110619
            If SWStatVendOLD Then 'per oralofaccio x tutti 2 Then 'Da Fatturare
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StCliArt) 'SOLO ANNO CORRENTE
            Else 'uso la FUNCTION su piu' esercizi
                Dim strSQL As String = ""
                If Statistica = 1 Then 'fatturato
                    strSQL = "Select * From FatturatoClientiArticolo("
                ElseIf Statistica = 2 Then 'Da Fatturare
                    strSQL = "Select * From DaFatturareClientiArticolo("
                Else
                    strSQL = "Select * From VendutoClientiArticolo("
                End If
                strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "','" & txtDataNC.Trim & "',"
                If CodArt1 <> "" Then
                    strSQL += "'" & CodArt1.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                If CodArt2 <> "" Then
                    strSQL += "'" & CodArt2.Trim & "',"
                Else
                    strSQL += "NULL,"
                End If
                strSQL += "NULL," 'If CodCliente <> "" Then
                strSQL += "NULL," 'Agente
                If CodCatCli <> -1 And strCategRagg.Trim = "" Then
                    strSQL += CodCatCli.ToString.Trim & ") AS Expr1 " & vbCr 'CodCateg
                Else
                    strSQL += "NULL) AS Expr1 " & vbCr 'CodCateg
                End If
                Dim SWWhere As Boolean = False
                If strCategRagg.Trim <> "" Then
                    SWWhere = True
                    strSQL += " WHERE Cod_Categoria IN (SELECT Categorie.Codice FROM Categorie WHERE (Categorie.Descrizione LIKE '" & strCategRagg & "%'))"
                End If
                '-
                If CodRegione <> 0 And CodRegione <> -1 Then
                    If SWWhere = True Then
                        strSQL += " AND Provincia IN (SELECT  Province.Codice FROM Province INNER JOIN Regioni ON Province.Regione = Regioni.Codice WHERE (Province.Regione = " & CodRegione.ToString.Trim & "))"
                    Else
                        strSQL += " WHERE Provincia IN (SELECT  Province.Codice FROM Province INNER JOIN Regioni ON Province.Regione = Regioni.Codice WHERE (Province.Regione = " & CodRegione.ToString.Trim & "))"
                    End If
                End If
                Dim ObjDB As New DataBaseUtility
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "StCliArt")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per regione / categoria / articolo"
                    Return False
                    Exit Function
                End Try
                '-
                strSQL = "Select * From Regioni"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Regioni")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per regione / categoria / articolo REGIONI"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Province"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Province")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per regione / categoria / articolo PROVINCE"
                    Return False
                    Exit Function
                End Try
                strSQL = "Select * From Categorie"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Categorie")
                Catch Ex As Exception
                    ObjDB = Nothing
                    Errore = Ex.Message & " - Stampa Statistica Venduto per categoria / Articolo CATEGORIA"
                    Return False
                    Exit Function
                End Try
                '-
                ObjDB = Nothing
            End If
            '---------
            Dim rowRegione As DsStatVendCliArt.RegioniRow = Nothing
            Dim rowProvince As DsStatVendCliArt.ProvinceRow = Nothing
            Dim rowCategorie As DsStatVendCliArt.CategorieRow = Nothing
            If DSStatVendCliArt1.StCliArt.Count > 0 Then
                For Each row As DsStatVendCliArt.StCliArtRow In DSStatVendCliArt1.StCliArt.Rows
                    row.Azienda = Azienda
                    If Statistica = 0 Then
                        row.TitoloReport = "Statistica articoli venduti / fatturati per regione/provincia/categoria/cliente/articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf Statistica = 1 Then
                        row.TitoloReport = "Statistica articoli fatturati per regione/provincia/categoria/cliente/articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    ElseIf Statistica = 2 Then
                        row.TitoloReport = "Statistica articoli da fatturare per regione/provincia/categoria/cliente/articolo (Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC & ")"
                    End If
                    row.PiedeReport = "Dal " & txtDataDa & " al " & txtDataA & " con N.C. al " & txtDataNC
                    If CodRegione <> -1 Then
                        row.PiedeReport += " - Cod.Regione: " & Str(CodRegione).Trim
                    Else
                        row.PiedeReport += " - Tutte le regioni "
                    End If
                    If Provincia.Trim <> "" Then
                        row.PiedeReport += " - Provincia: " & Provincia.Trim
                    Else
                        row.PiedeReport += " - Tutte le provincie "
                    End If
                    If CodCatCli <> -1 Then
                        row.PiedeReport += " - Cod.Categ.Cliente: " & Str(CodCatCli).Trim
                    Else
                        row.PiedeReport += " - Tutte le categ.Cliente "
                    End If
                    If strCategRagg.Trim <> "" Then
                        row.PiedeReport += " - Seleziona tutte le Categ.Cliente che iniziano per: " & strCategRagg.Trim
                    End If
                    If AccorpaCR = True Then
                        row.PiedeReport += " - Accorpa tutte le Categ.Cliente in: " & strCategRagg.Trim
                    End If
                    '    
                    If CodArt1 <> "" Then
                        row.PiedeReport += " - Dal Cod.Articolo: " & CodArt1.Trim
                    End If
                    If CodArt2 <> "" Then
                        row.PiedeReport += " - Al Cod.Articolo: " & CodArt2.Trim
                    End If
                    row.VisualizzaPrezzoVendita = True 'Aggiorno comunque anche se nel report non è gestito
                    If SWStatVendOLD = False Then 'per ora per tutti  2 Then
                        If row.IsProvinciaNull Then
                            row.DesRegione = "Sconosciuta"
                        Else
                            rowProvince = DSStatVendCliArt1.Province.FindByCodice(row.Provincia)
                            If Not rowProvince Is Nothing Then
                                row.DesProvincia = rowProvince.Descrizione
                                rowRegione = DSStatVendCliArt1.Regioni.FindByCodice(rowProvince.Regione)
                                If Not rowRegione Is Nothing Then
                                    row.Regione = rowProvince.Regione
                                    row.DesRegione = rowRegione.Descrizione
                                Else
                                    row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                                End If
                            Else
                                row.DesProvincia = "Sconosciuta " & row.Provincia.Trim
                                row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                            End If
                        End If
                        '-
                        If AccorpaCR = False Then
                            If row.IsCod_CategoriaNull Then
                                row.Categoria = "Sconosciuta"
                            Else
                                rowCategorie = DSStatVendCliArt1.Categorie.FindByCodice(row.Cod_Categoria)
                                If Not rowCategorie Is Nothing Then
                                    row.Categoria = rowCategorie.Descrizione.Trim
                                Else
                                    row.Categoria = "Sconosciuta " & row.Cod_Categoria.ToString.Trim
                                End If
                            End If
                        Else
                            row.Cod_Categoria = 999
                            row.Categoria = strCategRagg.Trim & " (Accorpata)"
                        End If

                    End If
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Statistica Venduto per regione/provincia/categoria/cliente/articolo"
            Return False
            Exit Function
        End Try

        Return True

    End Function

    Public Function InstallatoClienti(ByVal CodArt1 As String, ByVal CodArt2 As String, ByVal CodFornitore As String,
                                      ByVal txtDataDa As String, ByVal txtDataA As String, ByVal CodCliente As String,
                                      ByVal Titolo As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String) As Boolean
        InstallatoClienti = True
        DSStatVendCliArt1.Clear()
        'uso la FUNCTION su piu' esercizi
        Dim strSQL As String = ""
        strSQL = "Select * From InstallatoClientiArticolo("
        strSQL += "'" & txtDataDa.Trim & "','" & txtDataA.Trim & "',"
        If CodArt1 <> "" Then
            strSQL += "'" & CodArt1.Trim & "',"
        Else
            strSQL += "NULL,"
        End If
        If CodArt2 <> "" Then
            strSQL += "'" & CodArt2.Trim & "',"
        Else
            strSQL += "NULL,"
        End If
        strSQL += "NULL," 'CodCliente
        If CodFornitore <> "" Then
            strSQL += "'" & CodFornitore.Trim & "',"
        Else
            strSQL += "NULL,"
        End If
        strSQL += "NULL," 'CodAgente
        strSQL += "NULL" 'CodCatCli
        strSQL += ") AS Expr1 " & vbCr
        '-
        Dim ObjDB As New DataBaseUtility
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "InstallatoClienti")
            If DSStatVendCliArt1.InstallatoClienti.Count > 0 Then
                For Each row As DsStatVendCliArt.InstallatoClientiRow In DSStatVendCliArt1.InstallatoClienti.Rows
                    row.TitoloReport = Titolo
                Next
            End If
        Catch Ex As Exception
            ObjDB = Nothing
            Errore = Ex.Message & " - Estrazione Installato Clienti"
            Return False
            Exit Function
        End Try
        '-
    End Function

    'giu311023
    Public Function StampaContrattiRegPrCatCli(ByVal txtDataDa As String, ByVal txtDataA As String, ByVal Azienda As String, ByRef DSStatVendCliArt1 As DsStatVendCliArt, ByRef ObjReport As Object, ByRef Errore As String, ByVal CodRegione As Integer, ByVal Provincia As String, ByVal CodCatCli As Integer, ByVal strCategRagg As String, ByVal AccorpaCR As Boolean, ByVal TipoDoc As String, ByVal StatoDoc As String, ByVal Modello As String, ByVal StampaCMAttiviNuovi As Boolean, ByVal CodCliente As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnOrd As SqlConnection
        Dim SqlAdapStatVendCliArt As SqlDataAdapter
        Dim SqlDbSelectStatVendCliArt As SqlCommand
        Try
            DSStatVendCliArt1.Clear()

            SqlConnOrd = New SqlConnection
            SqlAdapStatVendCliArt = New SqlDataAdapter
            SqlDbSelectStatVendCliArt = New SqlCommand

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
            SqlDbSelectStatVendCliArt.CommandTimeout = myTimeOUT
            '---------------------------
            SqlAdapStatVendCliArt.SelectCommand = SqlDbSelectStatVendCliArt
            SqlConnOrd.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
            SqlDbSelectStatVendCliArt.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectStatVendCliArt.CommandText = "get_ElencoCMRegPrCCliTipoDoc"

            SqlDbSelectStatVendCliArt.Connection = SqlConnOrd

            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DAllaData", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AllaData", System.Data.SqlDbType.NVarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TipoDoc", System.Data.SqlDbType.NVarChar, 2, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StatoDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCategoria", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RaggrCatCli", System.Data.SqlDbType.NVarChar, 30, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AccorpaCC", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectStatVendCliArt.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCliente", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            'OK ASSEGNO I PARAMETRI
            SqlDbSelectStatVendCliArt.Parameters.Item("@DAllaData").Value = Format(DateTime.Parse(txtDataDa), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@AllaData").Value = Format(DateTime.Parse(txtDataA), FormatoData)
            SqlDbSelectStatVendCliArt.Parameters.Item("@TipoDoc").Value = TipoDoc
            SqlDbSelectStatVendCliArt.Parameters.Item("@StatoDoc").Value = StatoDoc
            '-
            If CodCatCli <> -1 Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCategoria").Value = CodCatCli
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCategoria").Value = System.DBNull.Value
            End If
            If strCategRagg.Trim <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@RaggrCatCli").Value = strCategRagg & "%"
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@RaggrCatCli").Value = System.DBNull.Value
            End If
            If AccorpaCR Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@AccorpaCC").Value = 1
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@AccorpaCC").Value = 0
            End If
            '------
            If CodCliente.Trim <> "" Then
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = CodCliente.Trim
            Else
                SqlDbSelectStatVendCliArt.Parameters.Item("@CodCliente").Value = System.DBNull.Value
            End If
            Try
                SqlAdapStatVendCliArt.Fill(DSStatVendCliArt1.StatCMRegPrCCliStato)
            Catch Ex As Exception
                Errore = Ex.Message & " - Statistica Contratti per Tipo evasione/Categoria cliente/Modello CONTRATTI"
                Return False
                Exit Function
            End Try
            '-
            Dim ObjDB As New DataBaseUtility
            Dim strSQL As String = "Select * From Regioni"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Regioni")
            Catch Ex As Exception
                ObjDB = Nothing
                Errore = Ex.Message & " - Statistica Contratti per Tipo evasione/Categoria cliente/Modello REGIONI"
                Return False
                Exit Function
            End Try
            strSQL = "Select * From Province"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Province")
            Catch Ex As Exception
                ObjDB = Nothing
                Errore = Ex.Message & " - Statistica Contratti per Tipo evasione/Categoria cliente/Modello PROVINCE"
                Return False
                Exit Function
            End Try
            strSQL = "Select * From Categorie"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DSStatVendCliArt1, "Categorie")
            Catch Ex As Exception
                ObjDB = Nothing
                Errore = Ex.Message & " - Statistica Contratti per Tipo evasione/Categoria cliente/Modello CATEGORIE"
                Return False
                Exit Function
            End Try
            '-
            ObjDB = Nothing
            '---------
            Dim rowRegione As DsStatVendCliArt.RegioniRow = Nothing
            Dim rowProvince As DsStatVendCliArt.ProvinceRow = Nothing
            Dim rowCategorie As DsStatVendCliArt.CategorieRow = Nothing
            Dim IDDocPrec As String = "" : Dim ModelloPrec As String = ""
            '-
            Dim TOTClienti As Integer = 0 : Dim strCClientePrec As String = ","
            Dim TOTContratti As Integer = 0 : Dim strIDDocPrec = ","
            Dim TOTNSerie As Integer = 0 : Dim strSeriePrec As String = ","
            Dim TOTContrattiNEW As Integer = 0
            Dim TOTContrattiSCA As Integer = 0
            '--------
            If DSStatVendCliArt1.StatCMRegPrCCliStato.Count > 0 Then
                For Each row As DsStatVendCliArt.StatCMRegPrCCliStatoRow In DSStatVendCliArt1.StatCMRegPrCCliStato.Rows
                    If row.RowState = DataRowState.Deleted Then
                        Continue For
                    End If
                    '.
                    If Modello = "ZZZ" Then 'tutti
                        'ok
                    ElseIf Modello = "XXX" Then 'misti
                        If IDDocPrec.Trim <> row.IDDocumenti.ToString.Trim Then
                            IDDocPrec = row.IDDocumenti.ToString.Trim
                            ModelloPrec = row.Modello.Trim.ToUpper
                            If DSStatVendCliArt1.StatCMRegPrCCliStato.Select("IDDocumenti=" + IDDocPrec + " AND Modello<>'" + ModelloPrec + "'").Length > 0 Then
                                'ok
                            Else
                                For Each rowDel As DsStatVendCliArt.StatCMRegPrCCliStatoRow In DSStatVendCliArt1.StatCMRegPrCCliStato.Select("IDDocumenti=" + IDDocPrec + "")
                                    rowDel.Delete()
                                Next
                                Continue For
                            End If
                        End If
                    Else
                        If Modello.Contains(row.Modello.Trim.ToUpper) Then
                            'ok
                        Else
                            row.Delete()
                            Continue For
                        End If
                    End If
                    '-
                    Dim strDesRegione As String = "" 'giu250324
                    If row.IsPrAppNull Then
                        row.DesRegione = "Sconosciuta"
                    Else
                        If Provincia.Trim <> row.PrApp.Trim And Provincia.Trim <> "" Then
                            row.Delete()
                            Continue For
                        End If
                        rowProvince = DSStatVendCliArt1.Province.FindByCodice(row.PrApp.Trim)
                        If Not rowProvince Is Nothing Then
                            If CodRegione > 0 Then
                                If CodRegione <> rowProvince.Regione Then
                                    row.Delete()
                                    Continue For
                                End If
                            End If
                            row.DesProvincia = rowProvince.Descrizione
                            rowRegione = DSStatVendCliArt1.Regioni.FindByCodice(rowProvince.Regione)
                            If Not rowRegione Is Nothing Then
                                row.Regione = rowProvince.Regione
                                row.DesRegione = rowRegione.Descrizione
                                strDesRegione = rowRegione.Descrizione
                            Else
                                row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                            End If
                        Else
                            row.DesProvincia = "Sconosciuta " & row.Provincia.Trim
                            row.DesRegione = "Sconosciuta " & row.Provincia.Trim
                        End If
                    End If
                    '-TOTALI
                    If Not strIDDocPrec.Contains("," + row.IDDocumenti.ToString.Trim + ",") Then
                        strIDDocPrec += row.IDDocumenti.ToString.Trim + ","
                        TOTContratti += 1
                        If CDate(row.DataInizio) >= CDate(txtDataDa.Trim) Then
                            TOTContrattiNEW += 1
                        End If
                        If CDate(row.DataFine) <= CDate(txtDataA.Trim) Then
                            TOTContrattiSCA += 1
                        End If
                    End If
                    If Not strCClientePrec.Contains("," + row.Cod_Cliente.ToString.Trim + ",") Then
                        strCClientePrec += row.Cod_Cliente.ToString.Trim + ","
                        TOTClienti += 1
                    End If
                    If Not strSeriePrec.Contains("," + row.Serie.ToString.Trim + ",") Then
                        strSeriePrec += row.Serie.ToString.Trim + ","
                        TOTNSerie += 1
                    End If
                    'OK
                    row.Azienda = Azienda
                    row.TitoloReport = "STATISTICA CONTRATTI per Tipo evasione/Categoria cliente/Modello (Dal " & txtDataDa & " al " & txtDataA & ")"
                    If Modello = "ZZZ" Then 'tutti
                        row.TitoloReport += " - Tutti i Modelli"
                    ElseIf Modello = "XXX" Then 'misti
                        row.TitoloReport += " - Modelli Misti"
                    Else
                        row.TitoloReport += " - Modelli: " + Modello.Trim
                    End If
                    If CodRegione <> -1 Then
                        If strDesRegione.Trim = "" Then
                            row.TitoloReport += " - Cod.Regione: " & Str(CodRegione).Trim
                        Else
                            row.TitoloReport += " - Regione: " & strDesRegione.Trim
                        End If
                    Else
                        row.TitoloReport += " - Tutte le regioni "
                    End If
                    If Provincia.Trim <> "" Then
                        row.TitoloReport += " - Provincia: " & Provincia.Trim
                    Else
                        row.TitoloReport += " - Tutte le provincie "
                    End If
                    If CodCatCli <> -1 Then
                        row.TitoloReport += " - Cod.Categ.Cliente: " & Str(CodCatCli).Trim
                    Else
                        row.TitoloReport += " - Tutte le Categorie Cliente "
                    End If
                    If strCategRagg.Trim <> "" Then
                        row.TitoloReport += " - Seleziona tutte le Categorie Cliente che iniziano per: " & strCategRagg.Trim
                    End If
                    If AccorpaCR = True Then
                        row.TitoloReport += " - Accorpa tutte le Categorie Cliente in: " & strCategRagg.Trim
                    End If
                    If StampaCMAttiviNuovi = True Then
                        row.TitoloReport = "TOTALI Contratti Attivi/Nuovi/In Scadenza - Esercizio " & CDate(txtDataA).Year.ToString.Trim
                    End If
                    '    
                    If AccorpaCR = False Then
                        If row.IsCod_CategoriaNull Then
                            row.Categoria = "Sconosciuta"
                        Else
                            rowCategorie = DSStatVendCliArt1.Categorie.FindByCodice(row.Cod_Categoria)
                            If Not rowCategorie Is Nothing Then
                                row.Categoria = rowCategorie.Descrizione.Trim
                            Else
                                row.Categoria = "Sconosciuta " & row.Cod_Categoria.ToString.Trim
                            End If
                        End If
                    Else
                        row.Cod_Categoria = 999
                        row.Categoria = strCategRagg.Trim & " (Accorpata)"
                    End If
                Next
                DSStatVendCliArt1.AcceptChanges()
                'totali
                '"Dal " & txtDataDa & " al " & txtDataA & " "
                For Each rowTot As DsStatVendCliArt.StatCMRegPrCCliStatoRow In DSStatVendCliArt1.StatCMRegPrCCliStato.Rows
                    If rowTot.RowState = DataRowState.Deleted Then
                        Continue For
                    End If
                    '.
                    rowTot.PiedeReport = ""
                    If StampaCMAttiviNuovi = False Then
                        rowTot.PiedeReport += "Totale Clienti:       " + TOTClienti.ToString.Trim + vbCr
                        rowTot.PiedeReport += "Totale Contratti:     " + TOTContratti.ToString.Trim + vbCr
                        rowTot.PiedeReport += "Totale N° Serie DAE:  " + TOTNSerie.ToString.Trim + vbCr
                    Else
                        rowTot.PiedeReport += "Totale Contratti:     " + TOTContratti.ToString.Trim + vbCr
                        rowTot.PiedeReport += "- di cui Nuovi:       " + TOTContrattiNEW.ToString.Trim + vbCr
                        rowTot.PiedeReport += "- di cui In Scadenza: " + TOTContrattiSCA.ToString.Trim + vbCr
                    End If
                Next
                DSStatVendCliArt1.AcceptChanges()
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Statistica Contratti(StampaContrattiRegPrCatCli)"
            Return False
            Exit Function
        End Try

        Return True

    End Function
End Class
