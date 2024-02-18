Imports SoftAziOnLine.Def
Imports SoftAziOnLine.DataBaseUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Public Class Documenti
    'giu270612 variabili per quando si verifica un errore sulla stampa del documento
    Dim myIDDoc As String = "" : Dim myNDoc As String = "" : Dim myDataDoc As String = "" : Dim myTipoDoc As String = ""
    'giu030512
    Public Shared Function CalcolaTotaleDoc(ByVal dsDocDett As DataSet, _
                                            ByRef Iva() As Integer, _
                                            ByRef Imponibile() As Decimal, _
                                            ByRef Imposta() As Decimal, _
                                            ByVal DecimaliValuta As Integer, _
                                            ByRef MoltiplicatoreValuta As Integer, _
                                            ByRef Totale As Decimal, ByRef TotaleLordoMerce As Decimal, _
                                            ByVal ScontoCassa As Decimal, ByVal Listino As Long, _
                                            ByVal TipoDocumento As String, ByVal Abbuono As Decimal, _
                                            ByVal IdDocumento As Long, ByRef strErrore As String, _
                                            ByVal ScCassaDett As Boolean, _
                                            ByRef TotaleLordoMercePL As Decimal, ByRef Deduzioni As Decimal) As Boolean
        'giu300419 Optional ByRef TotaleLordoMercePL As Decimal = 0) As Boolean 

        'giu030512
        strErrore = ""
        If Listino < 1 Then Listino = 1
        '--------
        Dim Cont As Integer
        Dim TmpImp As Decimal
        ' ''Dim RsConf1 As Recordset
        ' ''Dim SQLstr As String

        Dim TabOmaggioConIVA_IVA(4) As Integer
        Dim TabOmaggioConIVA_Imponibile(4) As Decimal
        Dim TabOmaggioConIVA_Imposta(4) As Decimal

        Dim IndOm As Integer

        'creazione del formato valuta
        Dim FormatValuta As String = "###,###,##0"
        Select Case DecimaliValuta
            Case 0
                FormatValuta = "###,###,##0"
            Case 2
                FormatValuta = "###,###,##0.00"
            Case 3
                FormatValuta = "###,###,##0.000"
            Case 4
                FormatValuta = "###,###,##0.0000"
            Case Else
                FormatValuta = "###,###,##0"
        End Select

        ' ''SQLstr = "SELECT Confezioni.Cod_Confezione " _
        ' ''& "From Confezioni " _
        ' ''& "Where (((Confezioni.Listino) = " & Listino & ")) " _
        ' ''& "ORDER BY Confezioni.Cod_Confezione;"
        ' ''RsConf1 = DBAzi.OpenRecordset(SQLstr, dbOpenSnapshot)

        For Cont = 0 To 3
            Iva(Cont) = 0
            Imposta(Cont) = 0
            Imponibile(Cont) = 0
            TabOmaggioConIVA_IVA(Cont) = 0
            TabOmaggioConIVA_Imponibile(Cont) = 0
        Next Cont

        Totale = 0
        MoltiplicatoreValuta = 1
        Cont = 0
        While Cont < DecimaliValuta
            Cont = Cont + 1
            MoltiplicatoreValuta = MoltiplicatoreValuta * 10
        End While

        Dim rsDettagli As DataRow
        'GIU020512
        Totale = 0
        TotaleLordoMerce = 0
        TotaleLordoMercePL = 0
        Deduzioni = 0
        Dim SWDed As Boolean = False
        '---------
        If (dsDocDett.Tables("DocumentiD").Rows.Count > 0) Then
            For Each rsDettagli In dsDocDett.Tables("DocumentiD").Select("IdDocumenti=" & IdDocumento.ToString.Trim, "Riga") 'GIU290519 TOLTO & " AND Importo<>0" ALTRIMENTI NON MI CALCOLA IL TOTALE LORDO MERCE
                If IsDBNull(rsDettagli!DedPerAcconto) Then
                    SWDed = False
                Else
                    SWDed = CBool(rsDettagli!DedPerAcconto)
                End If
                'GIU290519 END SPOSTATO QUI SOPRA 
                If rsDettagli![Importo] <> 0 Then
                    ' ''CONFEZIONI ADESSO NON E' GESTITO
                    ' ''RsConf1.FindFirst("Cod_Confezione = '" & rsDettagli!Cod_Articolo & "'")
                    ' ''If Not RsConf1.NoMatch Then
                    ' ''    Call CalcolaIVAConfezioni(rsDettagli, Iva(), Imponibile(), Imposta(), TabOmaggioConIVA_IVA(), TabOmaggioConIVA_Imponibile(), DecimaliValuta, MoltiplicatoreValuta, Totale, ScontoCassa, Listino, DecimaliValuta, TipoDoc)
                    ' ''Else
                    If SWDed = True Then
                        Deduzioni = Deduzioni + rsDettagli![Importo]
                    End If
                    Cont = 0
                    While ((rsDettagli![Cod_Iva] <> Iva(Cont)) And _
                          Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                        Cont = Cont + 1
                        If Cont = 4 Then
                            strErrore = MSGEccedIva
                            CalcolaTotaleDoc = False
                            Exit Function
                        End If
                    End While
                    Iva(Cont) = rsDettagli![Cod_Iva]
                    If SWDed = True Then 'GIU020519
                        TmpImp = rsDettagli![Importo]
                    ElseIf ScCassaDett Then 'GIU020119
                        TmpImp = rsDettagli![Importo]
                    Else
                        TmpImp = rsDettagli![Importo] * (100 - ScontoCassa) / 100
                    End If

                    If rsDettagli![OmaggioImponibile] = False Then
                        Totale = Totale + rsDettagli![Importo]
                        Imponibile(Cont) = Imponibile(Cont) + TmpImp
                    ElseIf Not rsDettagli!OmaggioImposta Then
                        For IndOm = 0 To 3
                            If TabOmaggioConIVA_IVA(IndOm) = rsDettagli!Cod_Iva Then
                                TabOmaggioConIVA_Imponibile(IndOm) = TabOmaggioConIVA_Imponibile(IndOm) + TmpImp
                                Exit For
                            ElseIf TabOmaggioConIVA_IVA(IndOm) = 0 Then
                                TabOmaggioConIVA_IVA(IndOm) = rsDettagli!Cod_Iva
                                TabOmaggioConIVA_Imponibile(IndOm) = TabOmaggioConIVA_Imponibile(IndOm) + TmpImp
                                Exit For
                            End If
                        Next IndOm
                    End If

                    If rsDettagli![OmaggioImposta] = False Then
                        If (Iva(Cont) < 50) And (Iva(Cont) > 0) Then
                            Imposta(Cont) = Imposta(Cont) + Format((TmpImp / 100) * (Iva(Cont)), FormatValuta)
                        End If
                    End If

                    'End If CONFEZIONI ADESSO NON E' GESTITO
                    If SWDed = False Then 'GIU020519()
                        Select Case Left(TipoDocumento, 1)
                            Case "O"
                                If rsDettagli![Qta_Ordinata] <> 0 Then
                                    TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                    TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                End If
                            Case Else
                                If TipoDocumento = "PR" Or TipoDocumento = "CA" Or TipoDocumento = "TC" Then 'GIU021219
                                    If rsDettagli![Qta_Ordinata] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                    End If
                                Else
                                    If rsDettagli![Qta_Evasa] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Evasa]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Evasa])
                                    End If
                                End If
                        End Select
                    End If
                Else 'GIU290519 PER CALCOLARE COMUNQUE IL TOTALE LORDO MERCE 
                    If SWDed = False Then 'GIU020519
                        Select Case Left(TipoDocumento, 1)
                            Case "O"
                                If rsDettagli![Qta_Ordinata] <> 0 Then
                                    TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                    TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                End If
                            Case Else
                                If TipoDocumento = "PR" Or TipoDocumento = "CA" Or TipoDocumento = "TC" Then 'GIU021219
                                    If rsDettagli![Qta_Ordinata] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                    End If
                                Else
                                    If rsDettagli![Qta_Evasa] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Evasa]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Evasa])
                                    End If
                                End If
                        End Select
                    End If
                End If
            Next
        Else
            Totale = 0
            TotaleLordoMerce = 0
            TotaleLordoMercePL = 0
            Deduzioni = 0
        End If
        'giu130320
        ' ''Dim ContImp As Long
        ' ''ContImp = 0
        ' ''If Abbuono <> 0 Then
        ' ''    For Cont = 0 To UBound(Imponibile)
        ' ''        If Imponibile(Cont) <> 0 Then
        ' ''            ContImp = ContImp + 1
        ' ''        End If
        ' ''    Next Cont
        ' ''    If ContImp <> 0 Then
        ' ''        For Cont = 0 To UBound(Imponibile)
        ' ''            If Imponibile(Cont) <> 0 Then
        ' ''                Imponibile(Cont) = Imponibile(Cont) - (Abbuono / ContImp)
        ' ''            End If
        ' ''        Next Cont
        ' ''    End If
        ' ''End If
        '---

        For Cont = 0 To UBound(Imposta)
            If (Iva(Cont) < 50) And (Iva(Cont) > 0) Then
                Imposta(Cont) = Format((Imponibile(Cont) / 100) * (Iva(Cont)), FormatValuta)
            End If
        Next Cont

        'CONFEZIONI ADESSO NON E' GESTITO
        ' ''RsConf1.Close()
        ' ''RsConf1 = Nothing

        '' ''giu210206 Controllo se l'Omaggio riesco a Inserirlo nell'array delle Aliquote altrimenti lo ACCORPO
        Dim SWOkOMG As Boolean : SWOkOMG = False
        For IndOm = 0 To 3
            If TabOmaggioConIVA_IVA(IndOm) <> 0 Then
                If (TabOmaggioConIVA_IVA(IndOm) < 50) And (TabOmaggioConIVA_IVA(IndOm) > 0) Then
                    TabOmaggioConIVA_Imposta(IndOm) = Int((TabOmaggioConIVA_Imponibile(IndOm) * (TabOmaggioConIVA_IVA(IndOm) / 100)) * MoltiplicatoreValuta + 0.9999) / MoltiplicatoreValuta
                End If
                'giu100106
                SWOkOMG = False 'giu300407
                For Cont = 0 To 3
                    If Iva(Cont) = 0 Then 'X LIBERO MUOVO TUTTO
                        Iva(Cont) = TabOmaggioConIVA_IVA(IndOm)
                        Imposta(Cont) = TabOmaggioConIVA_Imposta(IndOm)
                        SWOkOMG = True
                        Exit For
                    End If
                Next Cont
                '---------
                If SWOkOMG = False Then
                    For Cont = 0 To 3
                        If Iva(Cont) = TabOmaggioConIVA_IVA(IndOm) Then 'X UGUALE SOMMO
                            Imposta(Cont) = Imposta(Cont) + TabOmaggioConIVA_Imposta(IndOm)
                            Exit For
                        End If
                    Next Cont
                End If
                '---------
            End If
        Next IndOm

        CalcolaTotaleDoc = True
    End Function
    'GIU031219 CONTRATTI
    Public Shared Function CalcolaTotaleDocCA(ByVal dsDocDett As DataSet, _
                                            ByRef Iva() As Integer, _
                                            ByRef Imponibile() As Decimal, _
                                            ByRef Imposta() As Decimal, _
                                            ByVal DecimaliValuta As Integer, _
                                            ByRef MoltiplicatoreValuta As Integer, _
                                            ByRef Totale As Decimal, ByRef TotaleLordoMerce As Decimal, _
                                            ByVal ScontoCassa As Decimal, ByVal Listino As Long, _
                                            ByVal TipoDocumento As String, ByVal Abbuono As Decimal, _
                                            ByVal IdDocumento As Long, ByRef strErrore As String, _
                                            ByVal ScCassaDett As Boolean, _
                                            ByRef TotaleLordoMercePL As Decimal, ByRef Deduzioni As Decimal) As Boolean
        'giu300419 Optional ByRef TotaleLordoMercePL As Decimal = 0) As Boolean 

        'giu030512
        strErrore = ""
        If Listino < 1 Then Listino = 1
        '--------
        Dim Cont As Integer
        Dim TmpImp As Decimal
        ' ''Dim RsConf1 As Recordset
        ' ''Dim SQLstr As String

        Dim TabOmaggioConIVA_IVA(4) As Integer
        Dim TabOmaggioConIVA_Imponibile(4) As Decimal
        Dim TabOmaggioConIVA_Imposta(4) As Decimal

        Dim IndOm As Integer

        'creazione del formato valuta
        Dim FormatValuta As String = "###,###,##0"
        Select Case DecimaliValuta
            Case 0
                FormatValuta = "###,###,##0"
            Case 2
                FormatValuta = "###,###,##0.00"
            Case 3
                FormatValuta = "###,###,##0.000"
            Case 4
                FormatValuta = "###,###,##0.0000"
            Case Else
                FormatValuta = "###,###,##0"
        End Select

        ' ''SQLstr = "SELECT Confezioni.Cod_Confezione " _
        ' ''& "From Confezioni " _
        ' ''& "Where (((Confezioni.Listino) = " & Listino & ")) " _
        ' ''& "ORDER BY Confezioni.Cod_Confezione;"
        ' ''RsConf1 = DBAzi.OpenRecordset(SQLstr, dbOpenSnapshot)

        For Cont = 0 To 3
            Iva(Cont) = 0
            Imposta(Cont) = 0
            Imponibile(Cont) = 0
            TabOmaggioConIVA_IVA(Cont) = 0
            TabOmaggioConIVA_Imponibile(Cont) = 0
        Next Cont

        Totale = 0
        MoltiplicatoreValuta = 1
        Cont = 0
        While Cont < DecimaliValuta
            Cont = Cont + 1
            MoltiplicatoreValuta = MoltiplicatoreValuta * 10
        End While

        Dim rsDettagli As DataRow
        'GIU020512
        Totale = 0
        TotaleLordoMerce = 0
        TotaleLordoMercePL = 0
        Deduzioni = 0
        Dim SWDed As Boolean = False
        '---------
        'giu180220 oppure qui passare tutte le attivita per periodo

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        If (dsDocDett.Tables("ContrattiD").Rows.Count > 0) Then
            For Each rsDettagli In dsDocDett.Tables("ContrattiD").Select("IdDocumenti=" & IdDocumento.ToString.Trim, "Riga") 'GIU290519 TOLTO & " AND Importo<>0" ALTRIMENTI NON MI CALCOLA IL TOTALE LORDO MERCE
                If rsDettagli.RowState = DataRowState.Deleted Then
                    Continue For
                End If
                If IsDBNull(rsDettagli!DedPerAcconto) Then
                    SWDed = False
                Else
                    SWDed = CBool(rsDettagli!DedPerAcconto)
                End If
                'GIU290519 END SPOSTATO QUI SOPRA 
                If rsDettagli![Importo] <> 0 Then
                    ' ''CONFEZIONI ADESSO NON E' GESTITO
                    ' ''RsConf1.FindFirst("Cod_Confezione = '" & rsDettagli!Cod_Articolo & "'")
                    ' ''If Not RsConf1.NoMatch Then
                    ' ''    Call CalcolaIVAConfezioni(rsDettagli, Iva(), Imponibile(), Imposta(), TabOmaggioConIVA_IVA(), TabOmaggioConIVA_Imponibile(), DecimaliValuta, MoltiplicatoreValuta, Totale, ScontoCassa, Listino, DecimaliValuta, TipoDoc)
                    ' ''Else
                    If SWDed = True Then
                        Deduzioni = Deduzioni + rsDettagli![Importo]
                    End If
                    Cont = 0
                    While ((rsDettagli![Cod_Iva] <> Iva(Cont)) And _
                          Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                        Cont = Cont + 1
                        If Cont = 4 Then
                            strErrore = MSGEccedIva
                            CalcolaTotaleDocCA = False
                            Exit Function
                        End If
                    End While
                    Iva(Cont) = rsDettagli![Cod_Iva]
                    If SWDed = True Then 'GIU020519
                        TmpImp = rsDettagli![Importo]
                    ElseIf ScCassaDett Then 'GIU020119
                        TmpImp = rsDettagli![Importo]
                    Else
                        TmpImp = rsDettagli![Importo] * (100 - ScontoCassa) / 100
                    End If

                    If rsDettagli![OmaggioImponibile] = False Then
                        Totale = Totale + rsDettagli![Importo]
                        Imponibile(Cont) = Imponibile(Cont) + TmpImp
                    ElseIf Not rsDettagli!OmaggioImposta Then
                        For IndOm = 0 To 3
                            If TabOmaggioConIVA_IVA(IndOm) = rsDettagli!Cod_Iva Then
                                TabOmaggioConIVA_Imponibile(IndOm) = TabOmaggioConIVA_Imponibile(IndOm) + TmpImp
                                Exit For
                            ElseIf TabOmaggioConIVA_IVA(IndOm) = 0 Then
                                TabOmaggioConIVA_IVA(IndOm) = rsDettagli!Cod_Iva
                                TabOmaggioConIVA_Imponibile(IndOm) = TabOmaggioConIVA_Imponibile(IndOm) + TmpImp
                                Exit For
                            End If
                        Next IndOm
                    End If

                    If rsDettagli![OmaggioImposta] = False Then
                        If (Iva(Cont) < 50) And (Iva(Cont) > 0) Then
                            Imposta(Cont) = Imposta(Cont) + Format((TmpImp / 100) * (Iva(Cont)), FormatValuta)
                        End If
                    End If

                    'End If CONFEZIONI ADESSO NON E' GESTITO
                    If SWDed = False Then 'GIU020519()
                        Select Case Left(TipoDocumento, 1)
                            Case "O"
                                If rsDettagli![Qta_Ordinata] <> 0 Then
                                    TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                    TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                End If
                            Case Else
                                If TipoDocumento = "PR" Or TipoDocumento = "CA" Or TipoDocumento = "TC" Then 'GIU021219
                                    If rsDettagli![Qta_Ordinata] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                    End If
                                Else
                                    If rsDettagli![Qta_Evasa] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Evasa]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Evasa])
                                    End If
                                End If
                        End Select
                    End If
                Else 'GIU290519 PER CALCOLARE COMUNQUE IL TOTALE LORDO MERCE 
                    If SWDed = False Then 'GIU020519
                        Select Case Left(TipoDocumento, 1)
                            Case "O"
                                If rsDettagli![Qta_Ordinata] <> 0 Then
                                    TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                    TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                End If
                            Case Else
                                If TipoDocumento = "PR" Or TipoDocumento = "CA" Or TipoDocumento = "TC" Then 'GIU021219
                                    If rsDettagli![Qta_Ordinata] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Ordinata]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Ordinata])
                                    End If
                                Else
                                    If rsDettagli![Qta_Evasa] <> 0 Then
                                        TotaleLordoMerce += IIf(rsDettagli![Prezzo] = 0, rsDettagli![PrezzoListino], rsDettagli![Prezzo]) * rsDettagli![Qta_Evasa]
                                        TotaleLordoMercePL += (IIf(rsDettagli![PrezzoListino] = 0, rsDettagli![Prezzo], rsDettagli![PrezzoListino]) * rsDettagli![Qta_Evasa])
                                    End If
                                End If
                        End Select
                    End If
                End If
            Next
        Else
            Totale = 0
            TotaleLordoMerce = 0
            TotaleLordoMercePL = 0
            Deduzioni = 0
        End If
        'giu130320
        ' ''Dim ContImp As Long
        ' ''ContImp = 0
        ' ''If Abbuono <> 0 Then
        ' ''    For Cont = 0 To UBound(Imponibile)
        ' ''        If Imponibile(Cont) <> 0 Then
        ' ''            ContImp = ContImp + 1
        ' ''        End If
        ' ''    Next Cont
        ' ''    If ContImp <> 0 Then
        ' ''        For Cont = 0 To UBound(Imponibile)
        ' ''            If Imponibile(Cont) <> 0 Then
        ' ''                Imponibile(Cont) = Imponibile(Cont) - (Abbuono / ContImp)
        ' ''            End If
        ' ''        Next Cont
        ' ''    End If
        ' ''End If
        '---

        For Cont = 0 To UBound(Imposta)
            If (Iva(Cont) < 50) And (Iva(Cont) > 0) Then
                Imposta(Cont) = Format((Imponibile(Cont) / 100) * (Iva(Cont)), FormatValuta)
            End If
        Next Cont

        'CONFEZIONI ADESSO NON E' GESTITO
        ' ''RsConf1.Close()
        ' ''RsConf1 = Nothing

        '' ''giu210206 Controllo se l'Omaggio riesco a Inserirlo nell'array delle Aliquote altrimenti lo ACCORPO
        Dim SWOkOMG As Boolean : SWOkOMG = False
        For IndOm = 0 To 3
            If TabOmaggioConIVA_IVA(IndOm) <> 0 Then
                If (TabOmaggioConIVA_IVA(IndOm) < 50) And (TabOmaggioConIVA_IVA(IndOm) > 0) Then
                    TabOmaggioConIVA_Imposta(IndOm) = Int((TabOmaggioConIVA_Imponibile(IndOm) * (TabOmaggioConIVA_IVA(IndOm) / 100)) * MoltiplicatoreValuta + 0.9999) / MoltiplicatoreValuta
                End If
                'giu100106
                SWOkOMG = False 'giu300407
                For Cont = 0 To 3
                    If Iva(Cont) = 0 Then 'X LIBERO MUOVO TUTTO
                        Iva(Cont) = TabOmaggioConIVA_IVA(IndOm)
                        Imposta(Cont) = TabOmaggioConIVA_Imposta(IndOm)
                        SWOkOMG = True
                        Exit For
                    End If
                Next Cont
                '---------
                If SWOkOMG = False Then
                    For Cont = 0 To 3
                        If Iva(Cont) = TabOmaggioConIVA_IVA(IndOm) Then 'X UGUALE SOMMO
                            Imposta(Cont) = Imposta(Cont) + TabOmaggioConIVA_Imposta(IndOm)
                            Exit For
                        End If
                    Next Cont
                End If
                '---------
            End If
        Next IndOm

        CalcolaTotaleDocCA = True
    End Function

    Public Shared Function CalcolaImporto(ByVal PrezzoUnit As Decimal, _
                        ByVal Quantita As Decimal, _
                        ByVal Sconto1 As Decimal, _
                        ByVal Sconto2 As Decimal, _
                        ByVal Sconto3 As Decimal, _
                        ByVal Sconto4 As Decimal, _
                        ByVal ScontoPag As Decimal, _
                        ByVal ScontoVal As Decimal, _
                        ByVal OldImporto As Decimal, _
                        ByVal ScontoSuImporto As Boolean, _
                        ByVal DecimaliValutaFinito As Integer, _
                        ByRef PrezzoNetto As Decimal, _
                        ByVal ScCassaDett As Boolean, _
                        ByVal ScontoCassa As Decimal, _
                        ByVal DedPerAcconto As Boolean) As Decimal
        'giu120101 non funziona piu cosi qui lo sconto valore è la differenza del Prz.Listino e Vendita
        ' ''Optional ByVal PNettoMod As Boolean = False, _
        ' ''Optional ByVal ValorePNettoInputato As Decimal = 0) As Decimal

        Dim Importo As Decimal
        Dim Sconto As Decimal
        ' ''Dim ScontoSuImporto As Boolean = GetParamGestAzi.CalcoloScontoSuImporto
        'giu120101 non funziona piu cosi qui lo sconto valore è la differenza del Prz.Listino e Vendita
        ' ''If PNettoMod = False Then
        'GIU120219 PER FATTURAZIONE ELETTRONICA IN PRESENZA DI PIU' SCONTI IL CALCOLO DEVE AVVENIRE CON TUTTI I DECIMALI ALTRIMENTI NON TORNANO I CONTI 
        If Not ScontoSuImporto Then
            PrezzoNetto = PrezzoUnit
            If DedPerAcconto = False Then
                If ScontoVal <> 0 Then
                    PrezzoNetto = PrezzoNetto - CDec(IIf(CDec(ScontoVal) < 0, CDec(ScontoVal) * -1, CDec(ScontoVal)))
                End If
                '-----
                If IsNumeric(Sconto1) And Sconto1 <> 0 Then
                    'GIU120219 Sconto = Math.Round(PrezzoNetto * Sconto1 / 100, 4)
                    Sconto = PrezzoNetto * Sconto1 / 100
                    PrezzoNetto = PrezzoNetto - Sconto
                End If
                If Sconto2 <> 0 Then
                    'GIU120219 Sconto = Math.Round(PrezzoNetto * Sconto2 / 100, 4)
                    Sconto = PrezzoNetto * Sconto2 / 100
                    PrezzoNetto = PrezzoNetto - Sconto
                End If
                If Sconto3 <> 0 Then
                    'GIU120219 Sconto = Math.Round(PrezzoNetto * Sconto3 / 100, 4)
                    Sconto = PrezzoNetto * Sconto3 / 100
                    PrezzoNetto = PrezzoNetto - Sconto
                End If
                If Sconto4 <> 0 Then
                    'GIU120219 Sconto = Math.Round(PrezzoNetto * Sconto4 / 100, 4)
                    Sconto = PrezzoNetto * Sconto4 / 100
                    PrezzoNetto = PrezzoNetto - Sconto
                End If
                If ScontoPag <> 0 Then
                    'GIU120219 Sconto = Math.Round(PrezzoNetto * ScontoPag / 100, 4)
                    Sconto = PrezzoNetto * ScontoPag / 100
                    PrezzoNetto = PrezzoNetto - Sconto
                End If
                'giu120101 non funziona piu cosi qui lo sconto valore è la differenza del Prz.Listino e Vendita
                ' ''If IsNumeric(ScontoVal) And ScontoVal <> 0 Then
                ' ''    PrezzoNetto = PrezzoNetto - ScontoVal
                ' ''End If

                If ScCassaDett And ScontoCassa <> 0 Then 'giu010119
                    'GIU120219 Sconto = Math.Round(PrezzoNetto * ScontoCassa / 100, 4)
                    Sconto = PrezzoNetto * ScontoCassa / 100
                    PrezzoNetto = PrezzoNetto - Sconto
                End If
                PrezzoNetto = Math.Round(PrezzoNetto, DecimaliValutaIntermedio)
                Importo = PrezzoNetto * Quantita
            Else 'NEGATIVO L'IMPORTO
                PrezzoNetto = Math.Round(PrezzoNetto, DecimaliValutaIntermedio)
                Importo = (PrezzoNetto * Quantita) * -1
            End If
        Else
            If DedPerAcconto = False Then
                'giu120101 non funziona piu cosi qui lo sconto valore è la differenza del Prz.Listino e Vendita
                Importo = (PrezzoUnit - CDec(IIf(CDec(ScontoVal) < 0, CDec(ScontoVal) * -1, CDec(ScontoVal)))) * Quantita
                '-----
                If Sconto1 <> 0 Then
                    Sconto = Math.Round(Importo * Sconto1 / 100, 4)
                    Importo = Importo - Sconto
                End If
                If Sconto2 <> 0 Then
                    Sconto = Math.Round(Importo * Sconto2 / 100, 4)
                    Importo = Importo - Sconto
                End If
                If Sconto3 <> 0 Then
                    Sconto = Math.Round(Importo * Sconto3 / 100, 4)
                    Importo = Importo - Sconto
                End If
                If Sconto4 <> 0 Then
                    Sconto = Math.Round(Importo * Sconto4 / 100, 4)
                    Importo = Importo - Sconto
                End If
                If ScontoPag <> 0 Then
                    Sconto = Math.Round(Importo * ScontoPag / 100, 4)
                    Importo = Importo - Sconto
                End If

                If ScCassaDett And ScontoCassa <> 0 Then 'giu010119
                    Sconto = Math.Round(Importo * ScontoCassa / 100, 4)
                    Importo = Importo - Sconto
                End If
                '-
                If Quantita <> 0 Then
                    PrezzoNetto = Importo / Quantita
                Else
                    PrezzoNetto = 0
                End If
            Else
                Importo = PrezzoUnit * Quantita
                If Quantita <> 0 Then
                    PrezzoNetto = Importo / Quantita
                Else
                    PrezzoNetto = 0
                End If
                Importo = Importo * -1
            End If
            'giu120101 non funziona piu cosi qui lo sconto valore è la differenza del Prz.Listino e Vendita
            ' ''If ScontoVal <> 0 Then
            ' ''    Importo = Importo - CDec(ScontoVal)
            ' ''End If
            
        End If
        'giu120101 non funziona piu cosi qui lo sconto valore è la differenza del Prz.Listino e Vendita
        ' ''Else
        ' ''    'GIU290911 IL PREZZO IMPUTATO SE è = A ZERO?? OK PER ADESSO VA BENE COSI
        ' ''    If ScontoPag = 0 Then
        ' ''        PrezzoNetto = ValorePNettoInputato
        ' ''        PrezzoNetto = Math.Round(PrezzoNetto, DecimaliValutaIntermedio)
        ' ''        Importo = PrezzoNetto * Quantita
        ' ''    Else
        ' ''        PrezzoNetto = ValorePNettoInputato - ((ValorePNettoInputato / 100) * ScontoPag)
        ' ''        PrezzoNetto = Math.Round(PrezzoNetto, DecimaliValutaIntermedio)
        ' ''        Importo = PrezzoNetto * Quantita
        ' ''    End If
        ' ''End If

        PrezzoNetto = Math.Round(PrezzoNetto, DecimaliValutaIntermedio)
        'giu040920
        Dim myImporto As Decimal
        Dim myArr As Decimal
        Select Case DecimaliValutaIntermedio
            Case 1
                myArr = 0.1
            Case 2
                myArr = 0.01
            Case 3
                myArr = 0.001
            Case 4
                myArr = 0.0001
            Case Else
                myArr = 0
        End Select
        If Importo <> 0 Then
            If Importo > 0 Then
                myImporto = Importo + myArr
            ElseIf Importo < 0 Then
                myImporto = Importo + (myArr * -1)
            End If
        Else
            myImporto = Importo
        End If
        Importo = Math.Round(myImporto, DecimaliValutaFinito)
        'giu040920 Importo = Math.Round(Importo, DecimaliValutaFinito)
        '-------------
        CalcolaImporto = Importo
    End Function

    'giu010321 GIU050321
    Public Shared Function StessoPasso(ByVal rowPag() As DataRow, ByVal Documento As String, ByRef Passo As Integer, ByRef PercImponib As Double, ByRef PercImposta As Double, ByRef strErrore As String) As Boolean
        StessoPasso = False
        PercImponib = rowPag(0).Item("Perc_Imponib_5")
        PercImposta = rowPag(0).Item("Perc_Imposta_5")
        '-
        Passo = rowPag(0).Item("Scadenza_5") - rowPag(0).Item("Scadenza_4")
        If Passo = 0 Then
            strErrore = "Non è possibile calcolare automaticamente le scadenze successive alla quinta per il documento " & Documento & "." & vbCrLf & _
                    "L'incremento di giorni tra una scadenza e l'altra è uguale a ZERO."
            Exit Function
        End If

        If PercImponib = 0 Then
            strErrore = "Non è possibile calcolare automaticamente le scadenze successive alla quinta per il documento " & Documento & "." & vbCrLf & _
                     "La suddivisione dell'importo tra le rate è uguale a ZERO."
            Exit Function
        End If

        If PercImposta = 0 Then
            strErrore = "Non è possibile calcolare automaticamente le scadenze successive alla quinta per il documento " & Documento & "." & vbCrLf & _
                    "La suddivisione dell'imposta tra le rate è uguale a ZERO."
            Exit Function
        End If

        StessoPasso = True

    End Function

    Public Shared Function CompilaScadenze(ByVal rowPag() As DataRow, _
                               ByVal Data_Inizio As Date, _
                               ByVal Importo_Documento As Decimal, _
                               ByVal Valuta_Dec_Op As Integer, _
                               ByRef Totale_Rate As Integer, _
                               ByRef ArrayGG() As String, _
                               ByRef ArrayScadenze() As String, _
                               ByRef ArrayRate() As Decimal, _
                               ByRef TotaleRate As Decimal, _
                               ByRef strErrore As String)
        CompilaScadenze = True
        ' ''    'Tipopag            tabella dei pagamenti
        ' ''    'Data_inizio        data documento
        ' ''    'ImportoDocumento   importo da dividere
        ' ''    'Valuta_dec_op      decimali valuta di inserimento
        ' ''    'totale_rate        restituisce le rate in totale
        ' ''    'Arraygg            restituisce i giorni

        Dim Array_Scadenze() As Date : Array_Scadenze = Nothing
        Dim Array_Importi() As Decimal : Array_Importi = Nothing
        Dim Array_Giorni() As Integer : Array_Giorni = Nothing

        Dim ind As Integer = 0
        Dim Numero_Rate As Integer = IIf(IsDBNull(rowPag(0).Item("Numero_Rate")), 0, rowPag(0).Item("Numero_Rate"))
        'GIU010321 Numero_Rate_Effettive
        Dim Comodo As String = IIf(IsDBNull(rowPag(0).Item("Numero_Rate_Effettive")), "0", rowPag(0).Item("Numero_Rate_Effettive"))
        If String.IsNullOrEmpty(Comodo) Then Comodo = "0"
        If Not IsNumeric(Comodo) Then Comodo = "0"
        If CLng(Comodo) > 0 Then
            Numero_Rate = CLng(Comodo)
        End If
        '-
        'giu010321 calcolo scadenze rate>5
        Dim GiorniUltScadenza As Integer
        Dim PassoGiorni As Integer
        Dim PercImponib As Double
        If Numero_Rate > 5 Then
            If StessoPasso(rowPag, "", PassoGiorni, PercImponib, 0, strErrore) = False Then
                CompilaScadenze = False
                ' ''Exit Function
            End If
        End If
        GiorniUltScadenza = rowPag(0).Item("Scadenza_5")
        '-------------------------
        ReDim Array_Giorni(Numero_Rate)
        For ind = 0 To (Numero_Rate - 1)
            If ind <= 4 Then 'giu010321
                Array_Giorni(ind) = rowPag(0).Item("Scadenza_" + Trim(ind + 1))
            Else
                Array_Giorni(ind) = GiorniUltScadenza + PassoGiorni
                GiorniUltScadenza = Array_Giorni(ind)
            End If
        Next ind
        If CalcolaScadenze(Data_Inizio, Importo_Documento, Valuta_Dec_Op, Numero_Rate, _
                              rowPag(0).Item("Tipo_Scadenza"), Array_Giorni, _
                              rowPag(0).Item("Mese"), rowPag(0).Item("Mese_Escluso_1"), _
                              rowPag(0).Item("Mese_Escluso_2"), Array_Scadenze, Array_Importi, rowPag, _
                              Importo_Documento, Importo_Documento, strErrore) = False Then
            CompilaScadenze = False
        End If
        Totale_Rate = 0
        For ind = 0 To UBound(Array_Scadenze) - 1
            ArrayGG(ind) = DateDiff("d", Data_Inizio, Array_Scadenze(ind))
            ArrayScadenze(ind) = Format(Array_Scadenze(ind), FormatoData)
            ArrayRate(ind) = Array_Importi(ind)
            TotaleRate = TotaleRate + Array_Importi(ind)
        Next ind

    End Function
    Public Shared Function CalcolaScadenze(ByVal Data_Partenza As Date, _
                                ByVal Importo As Decimal, _
                                ByVal Decimali_Valuta As Integer, _
                                ByVal Numero_Rate As Integer, _
                                ByVal Tipo_Scadenza As Byte, _
                                ByRef Array_Giorni() As Integer, _
                                ByVal Mese_Succ As Byte, _
                                ByVal Mese_Escluso_1 As Byte, _
                                ByVal Mese_Escluso_2 As Byte, _
                                ByRef Data_Scadenza() As Date, _
                                ByRef Imp_Scadenza() As Decimal, _
                                ByVal rowPag() As DataRow, _
                                ByVal Imponibile As Decimal, _
                                ByVal Imposta As Decimal, _
                                ByRef strErrore As String) As Boolean

        'Data_Partenza      = Data da cui si calcolano le scadenze
        'Importo            = Importo da frazionare
        'Decimali_Valuta    = Quantità di decimali per la valuta utilizzata
        'Numero_Rate        = Numero di scadenze in cui frazionare l'importo
        'Tipo_Scadenza      = (0 = Data Fattura) (1 = Fine mese) (2 = Giorno fisso)
        'Array_Giorni       = Per il tipo scadenza 0 e 1 contiene i giorni per
        '                     calcolare la scadenza a partire dalla data_partenza,
        '                     per il tipo scadenza 2 contiene il giorno del mese
        '                     in cui dovrà cadere la scadenza
        'Mese_Succ          = Per il tipo scadenza = 2 indica:
        '                     0 = Calcola la scadenza a partire dal mese della data di partenza
        '                     1 = Calcola la scadenza a partire dal mese successivo alla data di partenza
        'Mese_Escluso_1     = Mese da escludere dal calcolo della scadenza (0-12)
        'Mese_Escluso_2     = Mese da escludere dal calcolo della scadenza (0-12)
        'Data_Scadenza      = Array restituito al chiamante contenente le date di scadenza
        'Imp_Scadenza       = Array restituito al chiamante contenente gli importi delle rate
        'RsPag              = Puntatore alla tabella Pagamenti
        'Imponibile         = Parte dell'importo indicato come imponibile
        'Imposta            = Parte dell'importo indicato come imposta

        CalcolaScadenze = True

        Dim ind As Integer
        Dim IndM1 As Integer
        Dim IndM2 As Integer
        Dim Mese As Integer
        Dim Anno As Integer
        Dim Escluso As Boolean

        ReDim Data_Scadenza(Numero_Rate)
        ReDim Imp_Scadenza(Numero_Rate)

        Dim NumMesi As Object
        Dim NumGiorni As Object

        If Tipo_Scadenza <> 2 Then  'Data fattura o fine mese
            For ind = 0 To (Numero_Rate - 1)
                'Aggiungo i mesi o i giorni di scadenza alla data di partenza
                NumMesi = Int(Array_Giorni(ind) / 30)
                NumGiorni = NumMesi * 30
                If Array_Giorni(ind) = NumGiorni Then
                    Data_Scadenza(ind) = DateAdd("m", NumMesi, Data_Partenza)
                Else
                    Data_Scadenza(ind) = DateAdd("d", Array_Giorni(ind), Data_Partenza)
                End If

                If Tipo_Scadenza = 1 Then   'Calcolo la data di Fine Mese
                    Mese = Month(Data_Scadenza(ind)) + 1
                    Anno = Year(Data_Scadenza(ind))
                    If Mese = 13 Then Mese = 1 : Anno = Anno + 1
                    Data_Scadenza(ind) = DateAdd(DateInterval.Day, -1, CDate("01/" & Format(Mese, "00") & "/" & Format(Anno, "0000"))) 'DateSerial(Anno, Mese, 1) - 1
                End If
            Next ind
            '**************************************************************
            'Verifico i mesi da escludere dalle scadenze:
            'se trovo che un mese è da escludere aggiungo un mese da quella
            'scadenza in poi finchè in nessuna data è presente un mese da
            'escludere
            Escluso = False
            Do Until Escluso = True
                Escluso = True
                For IndM1 = 0 To (Numero_Rate - 1)
                    If Month(Data_Scadenza(IndM1)) = Mese_Escluso_1 Or _
                       Month(Data_Scadenza(IndM1)) = Mese_Escluso_2 Then
                        Escluso = False
                        Data_Scadenza(IndM1) = DateAdd("m", 1, Data_Scadenza(IndM1))
                        If Tipo_Scadenza = 1 Then   'Fine Mese
                            Mese = Month(Data_Scadenza(IndM1)) + 1
                            Anno = Year(Data_Scadenza(IndM1))
                            If Mese = 13 Then Mese = 1 : Anno = Anno + 1
                            Data_Scadenza(IndM1) = DateAdd(DateInterval.Day, -1, CDate("01/" & Format(Mese, "00") & "/" & Format(Anno, "0000"))) 'DateSerial(Anno, Mese, 1) - 1
                        End If
                        For IndM2 = IndM1 + 1 To (Numero_Rate - 1)
                            Data_Scadenza(IndM2) = DateAdd("m", 1, Data_Scadenza(IndM2))
                            If Tipo_Scadenza = 1 Then   'Fine Mese
                                Mese = Month(Data_Scadenza(IndM2)) + 1
                                Anno = Year(Data_Scadenza(IndM2))
                                If Mese = 13 Then Mese = 1 : Anno = Anno + 1
                                Data_Scadenza(IndM2) = DateAdd(DateInterval.Day, -1, CDate("01/" & Format(Mese, "00") & "/" & Format(Anno, "0000"))) 'DateSerial(Anno, Mese, 1) - 1
                            End If
                        Next IndM2
                    End If
                Next IndM1
            Loop
            '**************************************************************
        Else            'Giorno fisso
            If Day(Data_Partenza) > Array_Giorni(0) Or Mese_Succ = 1 Then
                Data_Scadenza(0) = DateAdd("m", 1, Data_Partenza)
            Else
                Data_Scadenza(0) = Data_Partenza
            End If
            'On Error Resume Next
            ind = 0
            Data_Scadenza(0) = CDate(Trim(Array_Giorni(0)) + "/" + _
                        Trim(Month(Data_Scadenza(0))) + "/" + Trim(Year(Data_Scadenza(0))))
            Dim SWErr As Boolean = False
            Try
                Do Until SWErr = False
                    ind = ind + 1
                    Data_Scadenza(0) = CDate(Trim(Array_Giorni(0) - ind) + "/" + _
                            Trim(Month(Data_Scadenza(0))) + "/" + Trim(Year(Data_Scadenza(0))))
                Loop
            Catch ex As Exception
                SWErr = True
            End Try
            'Do Until Err = 0
            '    ind = ind + 1
            '    Err = 0
            '    Data_Scadenza(0) = CDate(Trim(Array_Giorni(0) - ind) + "/" + _
            '            Trim(Month(Data_Scadenza(0))) + "/" + Trim(Year(Data_Scadenza(0))))
            'Loop
            'On Error GoTo 0
            '--
            For IndM1 = 1 To (Numero_Rate - 1)
                'On Error Resume Next
                Data_Scadenza(IndM1) = DateAdd("m", 1, Data_Scadenza(IndM1 - 1))
                Data_Scadenza(IndM1) = CDate(Trim(Array_Giorni(IndM1)) + "/" + _
                        Trim(Month(Data_Scadenza(IndM1))) + "/" + Trim(Year(Data_Scadenza(IndM1))))
                ind = 0
                SWErr = False
                Try
                    Do Until SWErr = False
                        ind = ind + 1
                        Data_Scadenza(IndM1) = CDate(Trim(Array_Giorni(IndM1) - ind) + "/" + _
                            Trim(Month(Data_Scadenza(IndM1))) + "/" + Trim(Year(Data_Scadenza(IndM1))))
                    Loop
                Catch ex As Exception
                    SWErr = True
                End Try
                'Do Until Err() = 0
                '    ind = ind + 1
                '    Err = 0
                '    Data_Scadenza(IndM1) = CDate(Trim(Array_Giorni(IndM1) - ind) + "/" + _
                '        Trim(Month(Data_Scadenza(IndM1))) + "/" + Trim(Year(Data_Scadenza(IndM1))))
                'Loop
                'On Error GoTo 0
            Next IndM1
            '**************************************************************
            Escluso = False
            Do Until Escluso = True
                Escluso = True
                For IndM1 = 0 To (Numero_Rate - 1)
                    If Month(Data_Scadenza(IndM1)) = Mese_Escluso_1 Or _
                       Month(Data_Scadenza(IndM1)) = Mese_Escluso_2 Then
                        Escluso = False
                        Data_Scadenza(IndM1) = DateAdd("m", 1, Data_Scadenza(IndM1))
                        'Qui metto il giorno giusto, se dà errore 13 vuol dire che
                        'la data non è formalmente corretta tipo 31/02/1998
                        'On Error Resume Next
                        Data_Scadenza(IndM1) = CDate(Trim(Array_Giorni(IndM1)) + "/" + _
                            Trim(Month(Data_Scadenza(IndM1))) + "/" + _
                            Trim(Year(Data_Scadenza(IndM1))))
                        ind = 0
                        SWErr = False
                        Try
                            Do Until SWErr = False
                                ind = ind + 1
                                Data_Scadenza(IndM1) = CDate(Trim(Array_Giorni(IndM1) - ind) + "/" + _
                                    Trim(Month(Data_Scadenza(IndM1))) + "/" + _
                                    Trim(Year(Data_Scadenza(IndM1))))
                            Loop
                        Catch ex As Exception
                            SWErr = True
                        End Try
                        'Do Until Err() = 0
                        '    ind = ind + 1
                        '    Err = 0
                        '    Data_Scadenza(IndM1) = CDate(Trim(Array_Giorni(IndM1) - ind) + "/" + _
                        '        Trim(Month(Data_Scadenza(IndM1))) + "/" + _
                        '        Trim(Year(Data_Scadenza(IndM1))))
                        'Loop
                        'On Error GoTo 0
                        For IndM2 = IndM1 + 1 To (Numero_Rate - 1)
                            Data_Scadenza(IndM2) = DateAdd("m", 1, Data_Scadenza(IndM2))
                            'On Error Resume Next
                            Data_Scadenza(IndM2) = CDate(Trim(Array_Giorni(IndM2)) + "/" + _
                                Trim(Month(Data_Scadenza(IndM2))) + "/" + _
                                Trim(Year(Data_Scadenza(IndM2))))
                            ind = 0
                            SWErr = False
                            Try
                                Do Until SWErr = False
                                    ind = ind + 1
                                    Data_Scadenza(IndM2) = CDate(Trim(Array_Giorni(IndM2) - ind) + _
                                        "/" + Trim(Month(Data_Scadenza(IndM2))) + _
                                        "/" + Trim(Year(Data_Scadenza(IndM2))))
                                Loop
                            Catch ex As Exception
                                SWErr = True
                            End Try
                            'Do Until Err() = 0
                            '    ind = ind + 1
                            '    Err = 0
                            '    Data_Scadenza(IndM2) = CDate(Trim(Array_Giorni(IndM2) - ind) + _
                            '        "/" + Trim(Month(Data_Scadenza(IndM2))) + _
                            '        "/" + Trim(Year(Data_Scadenza(IndM2))))
                            'Loop
                            'On Error GoTo 0
                        Next IndM2
                    End If
                Next IndM1
            Loop
            '*******************
        End If

        'Importi
        Dim Imp_Rata As Decimal
        Dim Tot_Importi As Decimal

        'Rata singola
        If Numero_Rate = 1 Then
            Imp_Rata = Importo
            Select Case Decimali_Valuta         'Arrotondo in base alla valuta
                Case 0 : Imp_Rata = Format(Imp_Rata, "###,###,###,##0")
                Case 1 : Imp_Rata = Format(Imp_Rata, "###,###,###,##0.0")
                Case 2 : Imp_Rata = Format(Imp_Rata, "###,###,###,##0.00")
            End Select
            Tot_Importi = Imp_Rata
            Imp_Scadenza(0) = Imp_Rata
            Exit Function
        End If

        'Rate multiple
        Tot_Importi = 0
        For ind = 1 To Numero_Rate
            If ind < 6 Then 'giu010321
                Imp_Rata = (Imponibile * CDec(rowPag(0).Item("Perc_Imponib_" & Trim(ind))))
            Else
                Imp_Rata = (Imponibile * CDec(rowPag(0).Item("Perc_Imponib_5")))
            End If
            'Imp_Rata = Imp_Rata + ((Imposta / 100) * RsPag("Perc_Imposta_" & Trim(Ind)))
            Select Case Decimali_Valuta         'Arrotondo in base alla valuta
                Case 0 : Imp_Rata = Format(Imp_Rata, "###,###,###,##0")
                Case 1 : Imp_Rata = Format(Imp_Rata, "###,###,###,##0.0")
                Case 2 : Imp_Rata = Format(Imp_Rata, "###,###,###,##0.00")
            End Select
            Tot_Importi = Tot_Importi + Imp_Rata
            Imp_Scadenza(ind - 1) = Imp_Rata
        Next ind

        Tot_Importi = Importo - Tot_Importi
        Imp_Scadenza(Numero_Rate - 1) = Imp_Scadenza(Numero_Rate - 1) + Tot_Importi


    End Function

#Region "CALCOLO SCADENZE CONTRATTI"
    Public Shared Function CompilaScadenzeCA(ByVal rowPag() As DataRow, _
                               ByVal Data_Inizio As Date, _
                               ByVal Importo_Documento As Decimal, _
                               ByVal Valuta_Dec_Op As Integer, _
                               ByVal NumRate As Integer, _
                               ByVal NumGG As Integer, _
                               ByRef ArrayGG() As String, _
                               ByRef ArrayScadenze() As String, _
                               ByRef ArrayRate() As Decimal, _
                               ByRef TotaleRate As Decimal, _
                               ByRef strErrore As String)
        CompilaScadenzeCA = True
        strErrore = ""
        'Tipopag            tabella dei pagamenti(TipoContratto)
        'Data_inizio        data documento/data Inizio/Data accettazione
        'ImportoDocumento   importo da dividere
        'Valuta_dec_op      decimali valuta di inserimento
        'Arraygg            restituisce i giorni
        Dim Array_Scadenze() As Date : Array_Scadenze = Nothing
        Dim Array_Importi() As Decimal : Array_Importi = Nothing
        Dim Array_Giorni() As Integer : Array_Giorni = Nothing

        Dim ind As Integer = 0
        'ora NumRate Dim Numero_Rate As Integer = IIf(IsDBNull(rowPag(0).Item("Numero_Rate")), 0, rowPag(0).Item("Numero_Rate"))

        ReDim Array_Giorni(NumRate)
        'giu290120 qui gestire l'anticipato o posticipato gg 1 se e' anticipato alla prima scadenza
        Dim PagAnticipato As Integer = IIf(IsDBNull(rowPag(0).Item("TipoPagamento")), 0, rowPag(0).Item("TipoPagamento"))
        If PagAnticipato <> 1 Then
            PagAnticipato = 0
        End If
        Dim Anticipato As Integer = IIf(IsDBNull(rowPag(0).Item("Anticipato")), 0, rowPag(0).Item("Anticipato"))
        If Anticipato <> 0 Then
            PagAnticipato = 1
        End If
        For ind = 0 To (NumRate - 1)
            If PagAnticipato <> 0 Then
                Array_Giorni(ind) = NumGG * (ind)
            Else
                Array_Giorni(ind) = NumGG * (ind + 1)
            End If

        Next ind
        'giu290120 nessun mese escluso ora passo 0 per entrambi
        Dim Mese_Escluso_1 As Byte = 0 : Dim Mese_Escluso_2 As Byte = 0
        If CalcolaScadenzeCA(Data_Inizio, Importo_Documento, Valuta_Dec_Op, NumRate, _
                              rowPag(0).Item("TipoScadenza"), Array_Giorni, _
                              rowPag(0).Item("GiornoFisso"), _
                              IIf(IsDBNull(rowPag(0).Item("FineMese")), 0, rowPag(0).Item("FineMese")), _
                              IIf(IsDBNull(rowPag(0).Item("MeseCS")), 0, rowPag(0).Item("MeseCS")), _
                              Mese_Escluso_1, Mese_Escluso_2, _
                              Array_Scadenze, Array_Importi, rowPag, _
                              strErrore) = False Then
            CompilaScadenzeCA = False
        End If
        'Totale_Rate = 0
        For ind = 0 To UBound(Array_Scadenze) - 1
            ArrayGG(ind) = DateDiff("d", Data_Inizio, Array_Scadenze(ind))
            ArrayScadenze(ind) = Format(Array_Scadenze(ind), FormatoData)
            ArrayRate(ind) = Array_Importi(ind)
            TotaleRate = TotaleRate + Array_Importi(ind)
        Next ind

    End Function
    Public Shared Function CalcolaScadenzeCA(ByVal Data_Partenza As Date, _
                                ByVal Importo As Decimal, _
                                ByVal Decimali_Valuta As Integer, _
                                ByVal Numero_Rate As Integer, _
                                ByVal TipoScadenza As Byte, _
                                ByRef Array_Giorni() As Integer, _
                                ByVal GiornoFisso As Integer, _
                                ByVal FineMese As Byte, _
                                ByVal Mese_Succ As Byte, _
                                ByVal Mese_Escluso_1 As Byte, _
                                ByVal Mese_Escluso_2 As Byte, _
                                ByRef Data_Scadenza() As Date, _
                                ByRef Imp_Scadenza() As Decimal, _
                                ByVal rowPag() As DataRow, _
                                ByRef strErrore As String) As Boolean
        strErrore = ""
        'Data_Partenza      = Data da cui si calcolano le scadenze
        'Importo            = Importo da frazionare
        'Decimali_Valuta    = Quantità di decimali per la valuta utilizzata
        'Numero_Rate        = Numero di scadenze in cui frazionare l'importo
        'TipoScadenza       = (1 = Data Contratto) (2 = Data Accettazione) (3 = Data inizio) 
        'Array_Giorni       = Per il tipo scadenza 1 e 3 contiene i giorni per
        '                     calcolare la scadenza a partire dalla data_partenza,
        'GiornoFisso        = Contiene il giorno del mese
        '                     in cui dovrà cadere la scadenza
        'Mese_Succ          = Per GiornoFisso<>0:
        '                     1 = Calcola la scadenza a partire dal mese della data di partenza
        '                     2 = Calcola la scadenza a partire dal mese successivo alla data di partenza
        'Mese_Escluso_1     = Mese da escludere dal calcolo della scadenza (0-12)
        'Mese_Escluso_2     = Mese da escludere dal calcolo della scadenza (0-12)
        'Data_Scadenza      = Array restituito al chiamante contenente le date di scadenza
        'Imp_Scadenza       = Array restituito al chiamante contenente gli importi delle rate
        'RsPag              = Puntatore alla tabella Pagamenti
        'Imponibile         = Parte dell'importo indicato come imponibile
        'Imposta            = Parte dell'importo indicato come imposta

        CalcolaScadenzeCA = True

        Dim ind As Integer
        Dim IndM1 As Integer
        Dim IndM2 As Integer
        Dim Mese As Integer
        Dim Anno As Integer
        Dim Escluso As Boolean

        ReDim Data_Scadenza(Numero_Rate)
        ReDim Imp_Scadenza(Numero_Rate)

        Dim NumMesi As Object
        Dim NumGiorni As Object

        If GiornoFisso = 0 Then  'Data documento o fine mese
            For ind = 0 To (Numero_Rate - 1)
                'Aggiungo i mesi o i giorni di scadenza alla data di partenza
                NumMesi = Int(Array_Giorni(ind) / 30)
                NumGiorni = NumMesi * 30
                If Array_Giorni(ind) = NumGiorni Then
                    Data_Scadenza(ind) = DateAdd("m", NumMesi, Data_Partenza)
                Else
                    Data_Scadenza(ind) = DateAdd("d", Array_Giorni(ind), Data_Partenza)
                End If

                If CBool(FineMese) Then   'Calcolo la data di Fine Mese
                    Mese = Month(Data_Scadenza(ind)) + 1
                    Anno = Year(Data_Scadenza(ind))
                    If Mese = 13 Then Mese = 1 : Anno = Anno + 1
                    Data_Scadenza(ind) = DateAdd(DateInterval.Day, -1, CDate("01/" & Format(Mese, "00") & "/" & Format(Anno, "0000"))) 'DateSerial(Anno, Mese, 1) - 1
                End If
            Next ind
            '**************************************************************
            'Verifico i mesi da escludere dalle scadenze:
            'se trovo che un mese è da escludere aggiungo un mese da quella
            'scadenza in poi finchè in nessuna data è presente un mese da
            'escludere
            Escluso = False
            Do Until Escluso = True
                Escluso = True
                For IndM1 = 0 To (Numero_Rate - 1)
                    If Month(Data_Scadenza(IndM1)) = Mese_Escluso_1 Or _
                       Month(Data_Scadenza(IndM1)) = Mese_Escluso_2 Then
                        Escluso = False
                        Data_Scadenza(IndM1) = DateAdd("m", 1, Data_Scadenza(IndM1))
                        If CBool(FineMese) Then   'Fine Mese
                            Mese = Month(Data_Scadenza(IndM1)) + 1
                            Anno = Year(Data_Scadenza(IndM1))
                            If Mese = 13 Then Mese = 1 : Anno = Anno + 1
                            Data_Scadenza(IndM1) = DateAdd(DateInterval.Day, -1, CDate("01/" & Format(Mese, "00") & "/" & Format(Anno, "0000"))) 'DateSerial(Anno, Mese, 1) - 1
                        End If
                        For IndM2 = IndM1 + 1 To (Numero_Rate - 1)
                            Data_Scadenza(IndM2) = DateAdd("m", 1, Data_Scadenza(IndM2))
                            If CBool(FineMese) Then   'Fine Mese
                                Mese = Month(Data_Scadenza(IndM2)) + 1
                                Anno = Year(Data_Scadenza(IndM2))
                                If Mese = 13 Then Mese = 1 : Anno = Anno + 1
                                Data_Scadenza(IndM2) = DateAdd(DateInterval.Day, -1, CDate("01/" & Format(Mese, "00") & "/" & Format(Anno, "0000"))) 'DateSerial(Anno, Mese, 1) - 1
                            End If
                        Next IndM2
                    End If
                Next IndM1
            Loop
            '**************************************************************
        Else            'Giorno fisso
            If Day(Data_Partenza) > GiornoFisso Or Mese_Succ = 2 Then
                Data_Scadenza(0) = DateAdd("m", 1, Data_Partenza)
            Else
                Data_Scadenza(0) = Data_Partenza
            End If
            'On Error Resume Next
            ind = 0
            Data_Scadenza(0) = CDate(Trim(GiornoFisso) + "/" + _
                        Trim(Month(Data_Scadenza(0))) + "/" + Trim(Year(Data_Scadenza(0))))
            Dim SWErr As Boolean = False
            Try
                Do Until SWErr = False
                    ind = ind + 1
                    Data_Scadenza(0) = CDate(Trim(GiornoFisso - ind) + "/" + _
                            Trim(Month(Data_Scadenza(0))) + "/" + Trim(Year(Data_Scadenza(0))))
                Loop
            Catch ex As Exception
                SWErr = True
            End Try
            'Do Until Err = 0
            '    ind = ind + 1
            '    Err = 0
            '    Data_Scadenza(0) = CDate(Trim(GiornoFisso - ind) + "/" + _
            '            Trim(Month(Data_Scadenza(0))) + "/" + Trim(Year(Data_Scadenza(0))))
            'Loop
            'On Error GoTo 0
            '--
            For IndM1 = 1 To (Numero_Rate - 1)
                'On Error Resume Next
                Data_Scadenza(IndM1) = DateAdd("m", 1, Data_Scadenza(IndM1 - 1))
                Data_Scadenza(IndM1) = CDate(Trim(GiornoFisso) + "/" + _
                        Trim(Month(Data_Scadenza(IndM1))) + "/" + Trim(Year(Data_Scadenza(IndM1))))
                ind = 0
                SWErr = False
                Try
                    Do Until SWErr = False
                        ind = ind + 1
                        Data_Scadenza(IndM1) = CDate(Trim(GiornoFisso) + "/" + _
                            Trim(Month(Data_Scadenza(IndM1))) + "/" + Trim(Year(Data_Scadenza(IndM1))))
                    Loop
                Catch ex As Exception
                    SWErr = True
                End Try
                'Do Until Err() = 0
                '    ind = ind + 1
                '    Err = 0
                '    Data_Scadenza(IndM1) = CDate(Trim(GiornoFisso) + "/" + _
                '        Trim(Month(Data_Scadenza(IndM1))) + "/" + Trim(Year(Data_Scadenza(IndM1))))
                'Loop
                'On Error GoTo 0
            Next IndM1
            '**************************************************************
            Escluso = False
            Do Until Escluso = True
                Escluso = True
                For IndM1 = 0 To (Numero_Rate - 1)
                    If Month(Data_Scadenza(IndM1)) = Mese_Escluso_1 Or _
                       Month(Data_Scadenza(IndM1)) = Mese_Escluso_2 Then
                        Escluso = False
                        Data_Scadenza(IndM1) = DateAdd("m", 1, Data_Scadenza(IndM1))
                        'Qui metto il giorno giusto, se dà errore 13 vuol dire che
                        'la data non è formalmente corretta tipo 31/02/1998
                        'On Error Resume Next
                        Data_Scadenza(IndM1) = CDate(Trim(GiornoFisso) + "/" + _
                            Trim(Month(Data_Scadenza(IndM1))) + "/" + _
                            Trim(Year(Data_Scadenza(IndM1))))
                        ind = 0
                        SWErr = False
                        Try
                            Do Until SWErr = False
                                ind = ind + 1
                                Data_Scadenza(IndM1) = CDate(Trim(GiornoFisso) + "/" + _
                                    Trim(Month(Data_Scadenza(IndM1))) + "/" + _
                                    Trim(Year(Data_Scadenza(IndM1))))
                            Loop
                        Catch ex As Exception
                            SWErr = True
                        End Try
                        'Do Until Err() = 0
                        '    ind = ind + 1
                        '    Err = 0
                        '    Data_Scadenza(IndM1) = CDate(Trim(GiornoFisso) + "/" + _
                        '        Trim(Month(Data_Scadenza(IndM1))) + "/" + _
                        '        Trim(Year(Data_Scadenza(IndM1))))
                        'Loop
                        'On Error GoTo 0
                        For IndM2 = IndM1 + 1 To (Numero_Rate - 1)
                            Data_Scadenza(IndM2) = DateAdd("m", 1, Data_Scadenza(IndM2))
                            'On Error Resume Next
                            Data_Scadenza(IndM2) = CDate(Trim(GiornoFisso) + "/" + _
                                Trim(Month(Data_Scadenza(IndM2))) + "/" + _
                                Trim(Year(Data_Scadenza(IndM2))))
                            ind = 0
                            SWErr = False
                            Try
                                Do Until SWErr = False
                                    ind = ind + 1
                                    Data_Scadenza(IndM2) = CDate(Trim(GiornoFisso) + _
                                        "/" + Trim(Month(Data_Scadenza(IndM2))) + _
                                        "/" + Trim(Year(Data_Scadenza(IndM2))))
                                Loop
                            Catch ex As Exception
                                SWErr = True
                            End Try
                            'Do Until Err() = 0
                            '    ind = ind + 1
                            '    Err = 0
                            '    Data_Scadenza(IndM2) = CDate(Trim(GiornoFisso) + _
                            '        "/" + Trim(Month(Data_Scadenza(IndM2))) + _
                            '        "/" + Trim(Year(Data_Scadenza(IndM2))))
                            'Loop
                            'On Error GoTo 0
                        Next IndM2
                    End If
                Next IndM1
            Loop
            '*******************
        End If
        '
        Dim FormatoValuta As String = FormatoValEuro
        Select Case Decimali_Valuta         'Arrotondo in base alla valuta
            Case 0 : FormatoValuta = "###,###,###,##0"
            Case 1 : FormatoValuta = "###,###,###,##0.0"
            Case 2 : FormatoValuta = "###,###,###,##0.00"
        End Select
        'Importi
        Dim Imp_Rata As Decimal
        Dim Tot_Importi As Decimal
        'Rata singola
        If Numero_Rate = 1 Then
            Imp_Rata = Importo
            Tot_Importi = Imp_Rata
            Imp_Scadenza(0) = Imp_Rata
            Exit Function
        End If
        'Rate multiple
        Tot_Importi = 0
        Imp_Rata = Format((Importo / Numero_Rate), FormatoValuta)
        For ind = 1 To Numero_Rate
            Tot_Importi = Tot_Importi + Imp_Rata
            Imp_Scadenza(ind - 1) = Imp_Rata
        Next
        '---------------

        Tot_Importi = Importo - Tot_Importi
        Imp_Scadenza(Numero_Rate - 1) = Imp_Scadenza(Numero_Rate - 1) + Tot_Importi

    End Function
#End Region

    'giu130412 spostato da WUC_DOCUMENTI E WUC_DOCUMENTIDETT
    'GIU151017 _Cod_Iva as Integer
    Public Shared Function GetPrezziListinoAcquisto(ByVal TipoDoc As String, ByVal IDLT As String, _
                                            ByVal CodArt As String, ByRef _PrezzoListino As Decimal, _
                                            ByRef _PrezzoAcquisto As Decimal, _
                                            ByRef _Sconto1 As Decimal, _
                                            ByRef _Sconto2 As Decimal, _
                                            ByRef _Cod_Iva As Integer, _
                                            ByRef strErrore As String, _
                                            Optional ByRef _TipoArticolo As Integer = 0) As Boolean
        '=============================================================================
        'GIU170412 ATTENZIONE SE SI MODIFICA QUI VERIFICARE LA CORRISPONDENTE FUNZIONE
        'IN WUC_DocumentiDett.GetDatiAnaMag
        '=============================================================================
        GetPrezziListinoAcquisto = True
        _PrezzoListino = 0 : _PrezzoAcquisto = 0
        _Sconto1 = 0 : _Sconto2 = 0
        _Cod_Iva = 0 : _TipoArticolo = 0
        strErrore = ""
        If CodArt.Trim = "" Then Exit Function

        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim dsArt As New DataSet
        Dim rowArt() As DataRow
        '------------------------------------
        'AnaMag
        '------------------------------------
        strSQL = "Select * From AnaMag WHERE Cod_Articolo = '" & CodArt.Trim & "'"
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsArt)
            If (dsArt.Tables.Count > 0) Then
                If (dsArt.Tables(0).Rows.Count > 0) Then
                    rowArt = dsArt.Tables(0).Select()
                    'GIU280219 qui cambiare iif che se fosse null va tutto in errore 
                    If IsDBNull(rowArt(0).Item("PrezzoAcquisto")) Then
                        _PrezzoAcquisto = 0
                    Else
                        _PrezzoAcquisto = rowArt(0).Item("PrezzoAcquisto")
                    End If
                    If IsDBNull(rowArt(0).Item("Cod_Iva")) Then
                        _Cod_Iva = 0
                    Else
                        _Cod_Iva = rowArt(0).Item("Cod_Iva")
                    End If
                    If IsDBNull(rowArt(0).Item("TipoArticolo")) Then
                        _TipoArticolo = 0
                    Else
                        _TipoArticolo = rowArt(0).Item("TipoArticolo")
                    End If
                Else
                    strErrore = "(GetPrezziListinoAcquisto)Non trovato Cod.Articolo nella tabella Anagrafica articoli: " & CodArt.Trim
                    Return False
                End If
            Else
                strErrore = "(GetPrezziListinoAcquisto)Non trovato Cod.Articolo nella tabella Anagrafica articoli: " & CodArt.Trim
                Return False
            End If
        Catch Ex As Exception
            strErrore = "(GetPrezziListinoAcquisto)Lettura articoli: " & Ex.Message
            Return False
        End Try

        GetPrezziListinoAcquisto = GetPrezziListVenD(TipoDoc, IDLT, CodArt, _
                                              _PrezzoListino, _Sconto1, _Sconto2, strErrore)
    End Function

    Public Shared Function GetPrezziListVenD(ByVal TipoDoc As String, ByVal IDLT As String, _
                                            ByVal CodArt As String, _
                                            ByRef _PrezzoListino As Decimal, _
                                            ByRef _Sconto1 As Decimal, _
                                            ByRef _Sconto2 As Decimal, _
                                            ByRef strErrore As String) As Boolean
        GetPrezziListVenD = True
        _PrezzoListino = 0
        _Sconto1 = 0 : _Sconto2 = 0
        strErrore = ""
        If CodArt.Trim = "" Then Exit Function
        '------------------------------------
        'Listino per i prezzi e SCONTI
        '------------------------------------
        Dim dsLis As New DataSet
        Dim rowLis() As DataRow
        Dim SWOkLT As Boolean = True

        Dim strSQL As String = "" : Dim Comodo = ""
        Dim ObjDB As New DataBaseUtility

        strSQL = "Select * From ListVenD WHERE "
        strSQL += "Codice = " & IDLT.Trim & " "
        strSQL += "AND Cod_Articolo = '" & CodArt.Trim & "'"
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsLis)
            If (dsLis.Tables.Count > 0) Then
                If (dsLis.Tables(0).Rows.Count > 0) Then
                    rowLis = dsLis.Tables(0).Select()
                    _PrezzoListino = IIf(IsDBNull(rowLis(0).Item("Prezzo")), 0, rowLis(0).Item("Prezzo"))
                    _Sconto1 = IIf(IsDBNull(rowLis(0).Item("Sconto_1")), 0, rowLis(0).Item("Sconto_1"))
                    _Sconto2 = IIf(IsDBNull(rowLis(0).Item("Sconto_2")), 0, rowLis(0).Item("Sconto_2"))
                Else
                    strErrore = "(GetPrezziListVenD)Non trovato articolo nel listino: " & IDLT.Trim & " (" & CodArt.Trim & ")"
                    SWOkLT = False
                End If
            Else
                strErrore = "(GetPrezziListVenD)Non trovato articolo nel listino: " & IDLT.Trim & " (" & CodArt.Trim & ")"
                SWOkLT = False
            End If
        Catch Ex As Exception
            strErrore = "(GetPrezziListVenD)Lettura Listino di vendita: " & Ex.Message
            Return False
        End Try
        If SWOkLT = False Then
            If IDLT.Trim = "1" Then
                strErrore = "(GetPrezziListVenD)Non trovato articolo nel listino: " & IDLT.Trim & " (" & CodArt.Trim & ")"
                Return False
            Else
                IDLT = "1" ' LISTINO BASE
                strSQL = "Select * From ListVenD WHERE "
                strSQL += "Codice = " & IDLT.Trim & " "
                strSQL += "AND Cod_Articolo = '" & CodArt.Trim & "'"
                Try
                    dsLis.Clear()
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsLis)
                    If (dsLis.Tables.Count > 0) Then
                        If (dsLis.Tables(0).Rows.Count > 0) Then
                            rowLis = dsLis.Tables(0).Select()
                            _PrezzoListino = IIf(IsDBNull(rowLis(0).Item("Prezzo")), 0, rowLis(0).Item("Prezzo"))
                            _Sconto1 = IIf(IsDBNull(rowLis(0).Item("Sconto_1")), 0, rowLis(0).Item("Sconto_1"))
                            _Sconto2 = IIf(IsDBNull(rowLis(0).Item("Sconto_2")), 0, rowLis(0).Item("Sconto_2"))
                        Else
                            strErrore = "(GetPrezziListVenD)Non trovato articolo nel listino: " & IDLT.Trim & " (" & CodArt.Trim & ")"
                            Return False
                        End If
                    Else
                        strErrore = "(GetPrezziListVenD)Non trovato articolo nel listino: " & IDLT.Trim & " (" & CodArt.Trim & ")"
                        Return False
                    End If
                Catch Ex As Exception
                    strErrore = "(GetPrezziListVenD)Lettura Listino di vendita: " & Ex.Message
                    Return False
                End Try
            End If
        End If

    End Function

    Public Shared Function Calcola_ProvvAgente(ByVal _CodArt As String, _
                                               ByVal _CodAg As Integer, _
                                               ByRef _ProvvAg As Decimal, _
                                               ByVal _ScRiga As Decimal, _
                                               ByRef _SuperatoSconto As Boolean, _
                                               ByRef _Errore As String) As Boolean 'giu300312
        _Errore = ""
        _ProvvAg = 0
        Calcola_ProvvAgente = True
        If _CodArt.Trim = "" Then Exit Function
        If _CodAg = 0 Then Exit Function
        Dim MyCodFam As Integer = 0
        Dim ScontoMax As Decimal = 0
        Dim ProvvMinima As Decimal = 0
        Dim NumTabella As Long = 0
        _SuperatoSconto = False
        '---
        Dim ObjDB As New DataBaseUtility
        Dim dsProvvSconti As New DataSet
        Dim rowProvvScontiT() As DataRow
        Dim strSQL As String = "Select * From ProvvScontiT WHERE CodArticolo = '" & _CodArt.Trim & "'" _
                & " AND CodAgente = " & _CodAg.ToString.Trim
        Try
            dsProvvSconti.Clear()
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsProvvSconti)
            If (dsProvvSconti.Tables.Count > 0) Then
                If (dsProvvSconti.Tables(0).Rows.Count > 1) Then
                    _Errore = "Nelle tabelle provvigioni/sconti è definta più volte la tabella per l'agente (" & _CodAg.ToString.Trim & ") <br>" _
                            & " e l'articolo (" & _CodArt.Trim & "). La riga del documento non verrà modificata. <br>" _
                            & " Lasciare un solo abbinamento agente/articolo prima di procedere oltre."
                    _ProvvAg = 0
                    Calcola_ProvvAgente = False
                    Exit Function
                ElseIf (dsProvvSconti.Tables(0).Rows.Count = 0) Then 'PROVO PER TUTTI/ARTICOLO
                    strSQL = "Select * From ProvvScontiT WHERE CodArticolo = '" & _CodArt.Trim & "'" _
                            & " AND CodAgente IS NULL"
                    dsProvvSconti.Clear()
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsProvvSconti)
                    If (dsProvvSconti.Tables.Count > 0) Then
                        If (dsProvvSconti.Tables(0).Rows.Count > 1) Then
                            _Errore = "Nelle tabelle provvigioni/sconti è definta più volte la tabella per l'articolo (" & _CodArt.ToString.Trim & ") <br>" _
                                    & " La riga del documento non verrà modificata. <br>" _
                                    & " Lasciare un solo abbinamento agente/articolo prima di procedere oltre."
                            _ProvvAg = 0
                            Calcola_ProvvAgente = False
                            Exit Function
                        ElseIf (dsProvvSconti.Tables(0).Rows.Count = 0) Then 'NESSUNA PROVVIGIONE
                            _ProvvAg = 0
                            Calcola_ProvvAgente = True
                            Exit Function
                        Else 'OK PER TUTTI/ARTICOLO
                            'PROSEGUO
                        End If
                    Else 'NESSUNA PROVVIGIONE
                        _ProvvAg = 0
                        Calcola_ProvvAgente = True
                        Exit Function
                    End If
                Else 'OK PER AGENTE/ARTICOLO
                    'PROSEGUO
                End If
            Else 'PROVO PER TUTTI/ARTICOLO
                strSQL = "Select * From ProvvScontiT WHERE CodArticolo = '" & _CodArt.Trim & "'" _
                & " AND CodAgente IS NULL"
                dsProvvSconti.Clear()
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsProvvSconti)
                If (dsProvvSconti.Tables.Count > 0) Then
                    If (dsProvvSconti.Tables(0).Rows.Count > 1) Then
                        _Errore = "Nelle tabelle provvigioni/sconti è definta più volte la tabella per l'articolo (" & _CodArt.ToString.Trim & ") <br>" _
                                & " La riga del documento non verrà modificata. <br>" _
                                & " Lasciare un solo abbinamento agente/articolo prima di procedere oltre."
                        _ProvvAg = 0
                        Calcola_ProvvAgente = False
                        Exit Function
                    ElseIf (dsProvvSconti.Tables(0).Rows.Count = 0) Then 'NESSUNA PROVVIGIONE
                        _ProvvAg = 0
                        Calcola_ProvvAgente = True
                        Exit Function
                    Else 'OK PER TUTTI/ARTICOLO
                        'PROSEGUO
                    End If
                Else 'NESSUNA PROVVIGIONE
                    _ProvvAg = 0
                    Calcola_ProvvAgente = True
                    Exit Function
                End If
            End If
        Catch Ex As Exception
            _ProvvAg = 0
            Calcola_ProvvAgente = False
            _Errore = "Errore in Calcola_ProvvAgente: <br>" & Ex.Message
            Exit Function
        End Try
        'PROSEGUO
        rowProvvScontiT = dsProvvSconti.Tables(0).Select()
        If rowProvvScontiT.Count > 1 Then
            _Errore = "Nelle tabelle provvigioni/sconti è definta più volte la tabella per l'articolo (" & _CodArt.ToString.Trim & ") <br>" _
                               & " La riga del documento non verrà modificata. <br>" _
                               & " Lasciare un solo abbinamento agente/articolo prima di procedere oltre."
            Exit Function
            _ProvvAg = 0
            Calcola_ProvvAgente = False
            Exit Function
        ElseIf rowProvvScontiT.Count = 0 Then 'NESSUNA PROVVIGIONE
            _ProvvAg = 0
            Calcola_ProvvAgente = True
            Exit Function
        End If
        NumTabella = rowProvvScontiT(0).Item("ID")
        ScontoMax = IIf(IsDBNull(rowProvvScontiT(0).Item("ScontoMassimo")), 0, rowProvvScontiT(0).Item("ScontoMassimo"))
        ProvvMinima = IIf(IsDBNull(rowProvvScontiT(0).Item("ProvvMinima")), 0, rowProvvScontiT(0).Item("ProvvMinima"))
        MyCodFam = IIf(IsDBNull(rowProvvScontiT(0).Item("CodFamiglia")), 0, rowProvvScontiT(0).Item("CodFamiglia"))
        '---- 
        If _ScRiga <= ScontoMax And Not _SuperatoSconto Then
            _SuperatoSconto = False
        Else
            _SuperatoSconto = True
            _ProvvAg = 0
            Calcola_ProvvAgente = True
            Exit Function
        End If
        Dim CAge_All As Integer = IIf(IsDBNull(rowProvvScontiT(0).Item("CodAgente")), 0, rowProvvScontiT(0).Item("CodAgente"))
        If CAge_All = 0 Then 'GENERICA TUTTI/ARTICOLO
            strSQL = "SELECT TOP 1 * FROM ProvvScontiD" _
            & " WHERE CodAgente IS NULL " _
            & " AND CodArticolo = '" & _CodArt & "'" _
            & " AND CodFamiglia = " & MyCodFam.ToString.Trim _
            & " AND FinoASconto >= " & _ScRiga.ToString.Replace(",", ".") _
            & " ORDER BY FinoASconto"
        Else 'SPECIFICA PER AGENTE/ARTICOLO
            strSQL = "SELECT TOP 1 * FROM ProvvScontiD" _
            & " WHERE CodAgente = " & _CodAg.ToString.Trim _
            & " AND CodArticolo = '" & _CodArt & "'" _
            & " AND CodFamiglia = " & MyCodFam.ToString.Trim _
            & " AND FinoASconto >= " & _ScRiga.ToString.Replace(",", ".") _
            & " ORDER BY FinoASconto"
        End If

        dsProvvSconti.Clear()
        Dim rowProvvScontiD() As DataRow
        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsProvvSconti)
        If (dsProvvSconti.Tables.Count > 0) Then
            If (dsProvvSconti.Tables(0).Rows.Count > 0) Then
                rowProvvScontiD = dsProvvSconti.Tables(0).Select()
                If rowProvvScontiD.Count > 0 Then
                    If _ScRiga > IIf(IsDBNull(rowProvvScontiD(0).Item("FinoASconto")), 0, rowProvvScontiD(0).Item("FinoASconto")) Then
                        _SuperatoSconto = True
                        _ProvvAg = 0
                        Calcola_ProvvAgente = True
                        Exit Function
                    End If
                    _ProvvAg = IIf(IsDBNull(rowProvvScontiD(0).Item("Provvigione")), 0, rowProvvScontiD(0).Item("Provvigione"))
                    Calcola_ProvvAgente = True
                    Exit Function
                Else 'NESSUNA SCALETTA -  GLI PASSO LA MINIMA
                    _ProvvAg = ProvvMinima
                    Calcola_ProvvAgente = True
                    Exit Function
                End If

            Else 'NESSUNA SCALETTA -  GLI PASSO LA MINIMA
                _ProvvAg = ProvvMinima
                Calcola_ProvvAgente = True
                Exit Function
            End If
        Else 'NESSUNA SCALETTA -  GLI PASSO LA MINIMA
            _ProvvAg = ProvvMinima
            Calcola_ProvvAgente = True
            Exit Function
        End If
        ' ''    If Not RsProvvScontiD.EOF Then
        ' ''        RsProvvScontiD.FindFirst("FinoASconto >= CCur('" & PercSconto & "')")
        ' ''        If Not RsProvvScontiD.NoMatch Then
        ' ''            DBAzi.Execute("UPDATE " & TabellaD & " SET Pro_Agente = CCur('" & RsProvvScontiD!Provvigione & "') WHERE Cod_Articolo = '" & rsdettagli!Cod_Articolo & "' AND CodiceBolla = " & CodiceBolla & " AND NOT(OmaggioImponibile)")
        ' ''        Else
        ' ''            If PercSconto <= ScontoMax Then
        ' ''                DBAzi.Execute("UPDATE " & TabellaD & " SET Pro_Agente = CCur('" & RsProvvSconti!ProvvMinima & "') WHERE Cod_Articolo = '" & rsdettagli!Cod_Articolo & "' AND CodiceBolla = " & CodiceBolla & " AND NOT(OmaggioImponibile)")
        ' ''            Else
        ' ''                DBAzi.Execute("UPDATE " & TabellaD & " SET Pro_Agente = 0 WHERE Cod_Articolo = '" & rsdettagli!Cod_Articolo & "' AND CodiceBolla = " & CodiceBolla & " AND NOT(OmaggioImponibile)")
        ' ''            End If
        ' ''        End If
        ' ''    End If
    End Function

    '-DEF. per il controllo DOCUMENTO
    Dim RegimeIVA As String = "0" 'Session(CSTREGIMEIVA)
    'TOTALI,SPESE E TRASPORTO
    Dim ScCassa As String = "0" 'Session(CSTSCCASSA)
    '-
    Dim Abbuono As String = "0" 'Session(CSTABBUONO)
    '-
    'ASSEGNAZIONE TOTALE SPESE TRASPORTO
    Dim SpeseTrasporto As String = "0" 'Session(CSTSPTRASP)
    '-
    Dim SpeseIncasso = "0" 'non mi serve al momento ma nella testata c'è 
    Dim SpeseImballo = "0" 'non mi serve al momento ma nella testata c'è 
    Dim SpeseVarie = "0" 'non mi serve al momento ma nella testata c'è 
    '-----
    Dim ImponibileCheck As Decimal = 0
    Dim ImpostaCheck As Decimal = 0
    Dim TotaleCheck As Decimal = 0
    Dim TotaleMCheck As Decimal = 0
    '-----
    Dim ImponibileMemo As Decimal = 0
    Dim ImpostaMemo As Decimal = 0
    Dim TotaleMemo As Decimal = 0
    Dim TotaleMMemo As Decimal = 0
    'GIU180118
    Dim mySplitIVA As Boolean = False
    Dim myRitAcconto As Boolean = False
    Dim myTotaleRAMM As Decimal = 0
    Dim myTotaleRACK As Decimal = 0
    Dim myTotNettoPagareCK As Decimal = 0
    Dim myTotNettoPagareMM As Decimal = 0
    '-----
    Dim myTotDeduzioniCK As Decimal = 0 'giu020519
    Dim myTotLordoMerceCK As Decimal = 0 'GIU290519
    '---------
    'giu020512 stampa tutti i documenti fatturati o da numero a numero fill in APPEND
    Public Function StampaDocumento(ByVal IdDocumento As Long, ByVal TipoDoc As String, ByVal CodiceDitta As String,
                                    ByVal _Esercizio As String, ByVal TabCliFor As String, ByRef DsPrinWebDoc As DSPrintWeb_Documenti,
                                    ByRef ObjReport As Object, ByRef SWSconti As Boolean, ByRef Errore As String) As Boolean
        'GIU31082023 stampa lotti in documento senza il SUBReport
        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim SWStampaDocLotti As Boolean = False
        If App.GetDatiAbilitazioni(CSTABILAZI, "SWSTDOCLT", strValore, strErrore) = True Then
            SWStampaDocLotti = True
        Else
            SWStampaDocLotti = False
        End If
        '---------
        'giu270612
        myIDDoc = IdDocumento.ToString.Trim
        myTipoDoc = TipoDoc.Trim
        myDataDoc = ""
        myNDoc = ""
        '---------
        If TipoDoc.Trim = "" Then
            Errore = "Tipo Documento sconosciuto. Passo: 0 <br>" &
           "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        ElseIf Left(TipoDoc, 1) = "O" Or Left(TipoDoc, 1) = "P" Then
            _Esercizio = "" 'GIU230419 SOLO PER GLI ORDINI E PREVENTIVI LEGGERE SEMPRE L'ANNO CORRENTE ALTRIMENTI LE MODIFICHE NON SONO LE ULTIME
        End If
        'GIU01122011 ATTENZIONE SE ESTRAE 2 RKS DUPLICA I DETTAGLI DEGLI ARTICOLI
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim Passo As Integer = 0
        Dim SqlConnDoc As SqlConnection
        Dim SqlAdapDoc As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Try
            SqlConnDoc = New SqlConnection
            SqlAdapDoc = New SqlDataAdapter
            SqlDbSelectCmd = New SqlCommand

            SqlAdapDoc.SelectCommand = SqlDbSelectCmd
            SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, _Esercizio) 'GIU180419
            SqlDbSelectCmd.CommandText = "get_DocTByIDDocumenti"
            SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectCmd.Connection = SqlConnDoc
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            Dim SqlAdapDocDett As New SqlDataAdapter
            Dim SqlDbSelectDettCmd As New SqlCommand
            SqlDbSelectDettCmd.CommandText = "get_DocDByIDDocumenti"
            SqlDbSelectDettCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettCmd.Connection = SqlConnDoc
            SqlDbSelectDettCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocDett.SelectCommand = SqlDbSelectDettCmd

            'GIU301111 LOTTI
            Dim SqlAdapDocDettL As New SqlDataAdapter
            Dim SqlDbSelectDettCmdL As New SqlCommand
            SqlDbSelectDettCmdL.CommandText = "get_DocDLByIDDocRiga"
            SqlDbSelectDettCmdL.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettCmdL.Connection = SqlConnDoc
            SqlDbSelectDettCmdL.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDettCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocDettL.SelectCommand = SqlDbSelectDettCmdL

            '==============CARICAMENTO DATASET ===================
            Passo = 1
            DsPrinWebDoc.Clear()
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlAdapDoc.Fill(DsPrinWebDoc.DocumentiT)
            Passo = 2
            SqlDbSelectDettCmd.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlAdapDocDett.Fill(DsPrinWebDoc.DocumentiD)
            Passo = 3
            SqlDbSelectDettCmdL.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlDbSelectDettCmdL.Parameters.Item("@Riga").Value = DBNull.Value
            SqlAdapDocDettL.Fill(DsPrinWebDoc.DocumentiDLotti)
        Catch ex As Exception
            Errore = ex.Message & " - Documento lettura testata e dettagli. Passo: " & Passo.ToString & " <br> " &
            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '---------------------------------------------------------------------------------------------
        'giu021211 RIATTIVATA ROUTINE DI CALCOLO IMPORTO E DOCUMENTO PER IL PROBLEMA DEI DATI SALVATI
        'DA UNA SESSIONE PRECEDENTE (BACK SULLA PAGINA WEB) (CONTROLLO DETTAGLIO CON TESTATA DOCUMENTO
        '---------------------------------------------------------------------------------------------
        Passo = 4
        strErrore = ""
        'Listino documento
        Dim Listino As String = "1"
        'Valuta per i decimali per il calcolo 
        Dim DecimaliVal As String = "2" ' Session(CSTDECIMALIVALUTADOC)
        Dim CodValuta As String = "Euro"
        '---
        Listino = "1"
        CodValuta = "Euro"
        DecimaliVal = "2" 'Euro
        SpeseIncasso = "0"
        SpeseTrasporto = "0"
        SpeseImballo = "0"
        SpeseVarie = "0"
        RegimeIVA = "0"
        ScCassa = "0"
        Abbuono = "0"
        ImponibileMemo = 0
        ImpostaMemo = 0
        TotaleMemo = 0
        TotaleMMemo = 0
        '-
        ImponibileCheck = 0
        ImpostaCheck = 0
        TotaleCheck = 0
        TotaleMCheck = 0

        mySplitIVA = False
        myRitAcconto = False
        myTotaleRACK = 0
        myTotaleRAMM = 0
        myTotNettoPagareCK = 0
        myTotNettoPagareMM = 0
        '---
        'giu021117 Richiesta Zibordi del 27/10/2017 (Solo per PR/OC riportare le Iniziali=Codice Operatore)
        Dim strSQL As String = ""
        Dim X As Integer = 0
        Dim ObjDB As New DataBaseUtility
        '--------------------------------------------------------------------------------------------------
        Dim SWTabCliFor As String = ""
        'giu020512
        Dim myCodiceCoGe As String = ""
        Dim myCodFiliale As String = ""
        Dim myNazione As String = ""
        Dim rsTestata As DataRow
        '' ''giu220319
        ' ''Dim RighePerPagina As Integer = 0
        ' ''Dim RighePagina As Integer = 0
        ' ''Select Case Left(TipoDoc, 1)
        ' ''    Case "DT"
        ' ''        RighePerPagina = App.GetParamGestAzi(_Esercizio).RighePerPaginaDDT
        ' ''        RighePerPagina -= 5
        ' ''    Case Else
        ' ''        RighePerPagina = App.GetParamGestAzi(_Esercizio).RighePerPaginaDDT
        ' ''        RighePerPagina -= 5
        ' ''End Select
        '' ''---------
        Try
            If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                'GIU020512 STAMPALL
                For Each rsTestata In DsPrinWebDoc.Tables("DocumentiT").Select("")
                    If IIf(IsDBNull(rsTestata!StatoDoc), 0, rsTestata!StatoDoc) = 9 Then
                        Errore = "Documento BLOCCATO per Qta' evasa maggiore Qtà Ordinata. Passo: 0 <br>" &
           "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                        Return False
                        Exit Function
                    End If
                    'giu270712
                    myDataDoc = IIf(IsDBNull(rsTestata!Data_Doc), "", rsTestata!Data_Doc)
                    myNDoc = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero)
                    myTipoDoc = IIf(IsDBNull(rsTestata!Tipo_Doc), "", rsTestata!Tipo_Doc)
                    '---------
                    'GIU020512()
                    If Not IsDBNull(rsTestata![Cod_Cliente]) Then
                        TabCliFor = "Cli"
                        myCodiceCoGe = rsTestata![Cod_Cliente].ToString.Trim
                    ElseIf Not IsDBNull(rsTestata![Cod_Fornitore]) Then
                        TabCliFor = "For"
                        myCodiceCoGe = rsTestata![Cod_Fornitore].ToString.Trim
                        'Dim strCodice As String = DsPrinWebDoc.DocumentiT.Rows(0).Item("IDAnagrProvv").ToString.Trim
                    ElseIf Not IsDBNull(rsTestata![IDAnagrProvv]) Then
                        TabCliFor = "Provv"
                        myCodiceCoGe = rsTestata![IDAnagrProvv].ToString.Trim
                    Else
                        TabCliFor = "Cli"
                        myCodiceCoGe = ""
                    End If
                    '--
                    If Not IsDBNull(rsTestata![Cod_Filiale]) Then
                        myCodFiliale = rsTestata![Cod_Filiale].ToString.Trim
                    End If
                    '---------
                    Listino = rsTestata![Listino]
                    CodValuta = rsTestata![Cod_Valuta]
                    SpeseIncasso = rsTestata![Spese_Incasso]
                    SpeseTrasporto = rsTestata![Spese_Trasporto]
                    SpeseImballo = rsTestata![Spese_Imballo]
                    SpeseVarie = rsTestata![SpeseVarie]
                    RegimeIVA = rsTestata![Cod_Iva]
                    ScCassa = rsTestata![Sconto_Cassa]
                    Abbuono = rsTestata![Abbuono]
                    '-
                    ImponibileMemo += rsTestata![Imponibile1] + rsTestata![Imponibile2] + rsTestata![Imponibile3] + rsTestata![Imponibile4]
                    ImpostaMemo += rsTestata![Imposta1] + rsTestata![Imposta2] + rsTestata![Imposta3] + rsTestata![Imposta4]
                    TotaleMemo += rsTestata![Totale]
                    TotaleMMemo += rsTestata![TotaleM]
                    'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
                    rsTestata.BeginEdit()
                    If IsDBNull(rsTestata!Destinazione1) Then rsTestata!Destinazione1 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione2 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione3 = ""
                    If IsDBNull(rsTestata!ABI) Then rsTestata!ABI = ""
                    If IsDBNull(rsTestata!CAB) Then rsTestata!CAB = ""
                    If IsDBNull(rsTestata!IBAN) Then rsTestata!IBAN = ""
                    If IsDBNull(rsTestata!ContoCorrente) Then rsTestata!ContoCorrente = ""
                    If IsDBNull(rsTestata!CIG) Then rsTestata!CIG = ""
                    If IsDBNull(rsTestata!CUP) Then rsTestata!CUP = ""
                    If IsDBNull(rsTestata!FatturaPA) Then rsTestata!FatturaPA = False 'GIU120814
                    'GIU150118
                    If IsDBNull(rsTestata!SplitIVA) Then rsTestata!SplitIVA = False
                    mySplitIVA = rsTestata!SplitIVA
                    If IsDBNull(rsTestata!RitAcconto) Then rsTestata!RitAcconto = False
                    myRitAcconto = rsTestata!RitAcconto
                    If IsDBNull(rsTestata!ImponibileRA) Then rsTestata!ImponibileRA = 0
                    If IsDBNull(rsTestata!PercRA) Then rsTestata!PercRA = 0
                    If IsDBNull(rsTestata!TotaleRA) Then rsTestata!TotaleRA = 0
                    myTotaleRAMM = rsTestata!TotaleRA
                    If rsTestata!RitAcconto = False Then
                        If rsTestata!ImponibileRA <> 0 Or rsTestata!PercRA <> 0 Or rsTestata!TotaleRA <> 0 Then
                            Errore = "Errore calcolo Ritenuta d'acconto. (StampaDocumento). Passo: " & Passo.ToString & " <br> " &
                            "Per risolvere il problema controllare la sezione Totali modificando e aggiornando i dati Rit.Acconto." & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End If
                    Else
                        If IsDBNull(rsTestata!ImponibileRA) Then rsTestata!ImponibileRA = 0
                        If IsDBNull(rsTestata!PercRA) Then rsTestata!PercRA = 0
                        If IsDBNull(rsTestata!TotaleRA) Then rsTestata!TotaleRA = 0
                        If rsTestata!ImponibileRA <> 0 And rsTestata!PercRA <> 0 Then
                            myTotaleRACK = rsTestata!ImponibileRA * rsTestata!PercRA / 100
                            myTotaleRACK = FormatCurrency(myTotaleRACK, 2)
                        Else
                            myTotaleRACK = 0
                        End If
                        If rsTestata!ImponibileRA = 0 Or rsTestata!PercRA = 0 Or rsTestata!TotaleRA = 0 Or
                            myTotaleRACK <> myTotaleRAMM Then
                            Errore = "Errore calcolo Ritenuta d'acconto. (StampaDocumento). Passo: " & Passo.ToString & " <br> " &
                            "Per risolvere il problema controllare la sezione Totali modificando e aggiornando i dati Rit.Acconto." & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End If
                    End If
                    If IsDBNull(rsTestata!TotNettoPagare) Then rsTestata!TotNettoPagare = 0
                    myTotNettoPagareMM = rsTestata!TotNettoPagare
                    '---------
                    'giu021117
                    strSQL = ""
                    If Not IsDBNull(rsTestata!ModificatoDa) Then
                        strSQL = rsTestata!ModificatoDa.ToString.Trim
                        X = InStr(strSQL, " ")
                        If X > 0 Then
                            strSQL = Mid(strSQL, 1, X - 1)
                        End If
                    ElseIf Not IsDBNull(rsTestata!InseritoDa) Then
                        strSQL = rsTestata!InseritoDa.ToString.Trim
                        X = InStr(strSQL, " ")
                        If X > 0 Then
                            strSQL = Mid(strSQL, 1, X - 1)
                        End If
                    End If
                    If strSQL <> "" Then
                        strSQL = "Select Codice, Nome From Operatori WHERE (Nome LIKE N'%" & strSQL & "%') "
                        Try
                            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, DsPrinWebDoc, "Operatori")
                            If (DsPrinWebDoc.Tables("Operatori").Rows.Count > 0) Then
                                If Not IsDBNull(DsPrinWebDoc.Operatori.Rows(0).Item("Codice")) Then
                                    strSQL = DsPrinWebDoc.Operatori.Rows(0).Item("Codice").ToString.Trim
                                End If
                            End If
                        Catch ex As Exception
                            Errore = ex.Message & " - Inserimento dati nella Tabella Operatori. Passo: " & Passo.ToString
                            Return False
                            Exit Function
                        End Try
                    End If
                    If strSQL = "" Then strSQL = "XX"
                    rsTestata!Iniziali = strSQL
                    '---------
                    'giu070319
                    If IsDBNull(rsTestata!FatturaAC) Then rsTestata!FatturaAC = False
                    If IsDBNull(rsTestata!ScGiacenza) Then rsTestata!ScGiacenza = False
                    If IsDBNull(rsTestata!Acconto) Then rsTestata!Acconto = 0
                    If IsDBNull(rsTestata!TipoRapportoPA) Then rsTestata!TipoRapportoPA = ""
                    If IsDBNull(rsTestata!Bollo) Then rsTestata!Bollo = 0
                    If IsDBNull(rsTestata!BolloACaricoDel) Then rsTestata!BolloACaricoDel = ""
                    '---------
                    'giu020321 SCADENZE OLTRE 5 RATE
                    'AZZERO
                    rsTestata!Data_Scadenza_6 = DBNull.Value
                    rsTestata!Rata_6 = 0
                    rsTestata!Data_Scadenza_7 = DBNull.Value
                    rsTestata!Rata_7 = 0
                    rsTestata!Data_Scadenza_8 = DBNull.Value
                    rsTestata!Rata_8 = 0
                    rsTestata!Data_Scadenza_9 = DBNull.Value
                    rsTestata!Rata_9 = 0
                    rsTestata!Data_Scadenza_10 = DBNull.Value
                    rsTestata!Rata_10 = 0
                    rsTestata!Data_Scadenza_11 = DBNull.Value
                    rsTestata!Rata_11 = 0
                    rsTestata!Data_Scadenza_12 = DBNull.Value
                    rsTestata!Rata_12 = 0
                    '-
                    Dim TotRate As Decimal = myTotNettoPagareMM
                    Dim NRate As Integer = 0
                    'Dim ArrScadPag As ArrayList
                    Try
                        If IsDBNull(rsTestata!NoteNonEvasione) Then
                            rsTestata!NoteNonEvasione = ""
                        End If
                        'ArrScadPag = New ArrayList
                        Dim myScad As ScadPagEntity
                        If rsTestata!NoteNonEvasione.ToString.Trim <> "" Then
                            Dim lineaSplit As String() = rsTestata!NoteNonEvasione.Split(";")
                            TotRate = 0
                            For i = 0 To lineaSplit.Count - 1
                                If lineaSplit(i).Trim <> "" And (i + 2) <= lineaSplit.Count - 1 Then

                                    myScad = New ScadPagEntity
                                    myScad.NRata = lineaSplit(i).Trim
                                    i += 1
                                    myScad.Data = lineaSplit(i).Trim
                                    i += 1
                                    myScad.Importo = lineaSplit(i).Trim
                                    TotRate += CDec(myScad.Importo)
                                    Select Case NRate
                                        Case 0
                                            rsTestata!Data_Scadenza_1 = CDate(myScad.Data)
                                            rsTestata!Rata_1 = CDec(myScad.Importo)
                                        Case 1
                                            rsTestata!Data_Scadenza_2 = CDate(myScad.Data)
                                            rsTestata!Rata_2 = CDec(myScad.Importo)
                                        Case 2
                                            rsTestata!Data_Scadenza_3 = CDate(myScad.Data)
                                            rsTestata!Rata_3 = CDec(myScad.Importo)
                                        Case 3
                                            rsTestata!Data_Scadenza_4 = CDate(myScad.Data)
                                            rsTestata!Rata_4 = CDec(myScad.Importo)
                                        Case 4
                                            rsTestata!Data_Scadenza_5 = CDate(myScad.Data)
                                            rsTestata!Rata_5 = CDec(myScad.Importo)
                                        Case 5
                                            rsTestata!Data_Scadenza_6 = CDate(myScad.Data)
                                            rsTestata!Rata_6 = CDec(myScad.Importo)
                                        Case 6
                                            rsTestata!Data_Scadenza_7 = CDate(myScad.Data)
                                            rsTestata!Rata_7 = CDec(myScad.Importo)
                                        Case 7
                                            rsTestata!Data_Scadenza_8 = CDate(myScad.Data)
                                            rsTestata!Rata_8 = CDec(myScad.Importo)
                                        Case 8
                                            rsTestata!Data_Scadenza_9 = CDate(myScad.Data)
                                            rsTestata!Rata_9 = CDec(myScad.Importo)
                                        Case 9
                                            rsTestata!Data_Scadenza_10 = CDate(myScad.Data)
                                            rsTestata!Rata_10 = CDec(myScad.Importo)
                                        Case 10
                                            rsTestata!Data_Scadenza_11 = CDate(myScad.Data)
                                            rsTestata!Rata_11 = CDec(myScad.Importo)
                                        Case 11
                                            rsTestata!Data_Scadenza_12 = CDate(myScad.Data)
                                            rsTestata!Rata_12 = CDec(myScad.Importo)
                                    End Select
                                    '-
                                    'ArrScadPag.Add(myScad)
                                    NRate += 1
                                End If
                            Next
                        End If
                    Catch ex As Exception
                        NRate = 0
                        'TotRate = 0
                    End Try
                    If myTotNettoPagareMM <> TotRate Then
                        Errore = "Errore - Totale Netto a Pagare diverso dal Totale Rate Scadenze. Passo: " & Passo.ToString & " <br> " &
                                 "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                        Return False
                        Exit Function
                    End If
                    '-------------------------------
                    rsTestata.EndEdit()
                    rsTestata.AcceptChanges()
                Next
            Else
                Errore = "Errore - Documento lettura testata. Passo: " & Passo.ToString & " <br> " &
                "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
        Catch ex As Exception
            Listino = "1"
            CodValuta = "Euro"
            DecimaliVal = "2" 'Euro
            SpeseIncasso = "0"
            SpeseTrasporto = "0"
            SpeseImballo = "0"
            SpeseVarie = "0"
            RegimeIVA = "0"
            ScCassa = "0"
            Abbuono = "0"
            ImponibileMemo = 0
            ImpostaMemo = 0
            TotaleMemo = 0
            TotaleMMemo = 0
            myTotaleRAMM = 0
            myTotaleRACK = 0
            myTotNettoPagareCK = 0
            myTotNettoPagareMM = 0
            Errore = ex.Message & " - Documento lettura testata. Passo: " & Passo.ToString & " <br> " &
            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        If TipoDoc <> myTipoDoc Then 'giu190419
            TipoDoc = myTipoDoc
        End If
        Passo = 5
        'Ricalcolo Importo
        Dim MyImporto As Decimal = 0
        Dim rsDettagli As DataRow
        If (DsPrinWebDoc.Tables("DocumentiD").Rows.Count > 0) Then
            Dim myQtaO As Decimal = 0 : Dim myQtaE As Decimal = 0 : Dim myQtaR As Decimal = 0
            Dim myQuantita As Decimal = 0 : Dim SWQta = ""
            '---- Calcolo sconto su 
            Dim ScontoSuImporto As Boolean = True
            Dim ScCassaDett As Boolean = False 'giu010119 
            Try
                'giu190419 se seve altro aggiungere all'occorrenza ora solo i successivi 2 
                ScontoSuImporto = App.GetParGenAnno(_Esercizio, strErrore).CalcoloScontoSuImporto 'giu190419 App.GetParamGestAzi(_Esercizio).CalcoloScontoSuImporto
                ScCassaDett = App.GetParGenAnno(_Esercizio, strErrore).ScCassaDett 'giu190419 App.GetParamGestAzi(Esercizio).ScCassaDett
                If strErrore.Trim <> "" Then
                    Errore = strErrore & " - CONTROLLO Documento (GetParGenAnno). Passo: " & Passo.ToString & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
            Catch ex As Exception
                Errore = ex.Message.Trim & " " & strErrore & " - CONTROLLO Documento (GetParGenAnno). Passo: " & Passo.ToString & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
                ScontoSuImporto = True
                ScCassaDett = False
            End Try
            '-------------------------------------------------------------------------------------
            Passo = 6
            'GIU020512
            For Each rsDettagli In DsPrinWebDoc.Tables("DocumentiD").Select("", "Riga")
                Select Case Left(TipoDoc, 1)
                    Case "O"
                        myQuantita = rsDettagli![Qta_Ordinata]
                        SWQta = "O"
                    Case Else
                        If TipoDoc = "PR" Or TipoDoc = "CA" Or TipoDoc = "TC" Then 'GIU021219
                            myQuantita = rsDettagli![Qta_Ordinata]
                            SWQta = "O"
                        Else
                            myQuantita = rsDettagli![Qta_Evasa]
                            SWQta = "E"
                        End If
                End Select
                'giu020519 FATTURE PER ACCONTI 
                rsDettagli.BeginEdit()
                If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
                rsDettagli.EndEdit()
                rsDettagli.AcceptChanges()
                'CONTROLLO IMPORTO CHE SIANO UGLUALI
                TotaleCheck += rsDettagli![Importo]
                MyImporto = CalcolaImporto(rsDettagli![Prezzo], myQuantita,
                        rsDettagli![Sconto_1],
                        rsDettagli![Sconto_2],
                        rsDettagli![Sconto_3],
                        rsDettagli![Sconto_4],
                        rsDettagli![Sconto_Pag],
                        rsDettagli![ScontoValore],
                        rsDettagli![Importo],
                        ScontoSuImporto,
                        CInt(DecimaliVal),
                        rsDettagli![Prezzo_Netto], ScCassaDett, CDec(ScCassa), rsDettagli!DedPerAcconto) 'giu010119 giu020519 DedPerAcconto
                If rsDettagli![Importo] <> MyImporto Then
                    Errore = "Errore CONTROLLO Documento: Importo riga: (" & rsDettagli![Riga].ToString.Trim & ") diverso dall'importo riga ricalcolato. Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
                rsDettagli![StampaLotti] = False 'GIU181219
                'giu270913
                If myQuantita > 0 Then
                    If DsPrinWebDoc.Tables("DocumentiDLotti").Select("IDDocumenti=" & myIDDoc & " And Riga=" & rsDettagli![Riga].ToString.Trim).Length > 0 Then
                        rsDettagli![StampaLotti] = True
                    End If
                End If
                '---------
            Next
            Passo = 7
            'giu060319 al TotaleChek bisogna aggiungere l'IVA quindi il test verra fatto dopo CheckTotDoc che mi somma l'IVA MENTRE il Totale NettoAPagare è quello che puo non contenere IVA (SPLIT)
            ' ''If TotaleCheck <> TotaleMemo Then
            ' ''    Errore = "CONTROLLO Totali Merce(1) Documento (CheckTotDoc). Passo: " & Passo.ToString & " <br> " & _
            ' ''        "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " & _
            ' ''        "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            ' ''    Return False
            ' ''    Exit Function
            ' ''End If
            '---------
            Passo = 8
            strErrore = ""
            'giu020512 per l stampa ALLL
            'GIU031013 IVA 21 E 22 DAL 01/10/2013
            Dim SWIVA2122 As Boolean = False
            'GIU210819 If _Esercizio.Trim = "2013" Then
            If CDate(myDataDoc) < CDate("01/10/2013") Then
                SWIVA2122 = True
            End If
            'GIU210819 End If
            '------------------------------------
            If CheckTotDoc(DsPrinWebDoc, _Esercizio, TipoDoc, DecimaliVal, Listino, IdDocumento, strErrore, SWIVA2122) = False Then
                If strErrore.Trim <> "" Then
                    Errore = strErrore & " - CONTROLLO Documento (CheckTotDoc). Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
            End If
            'secondo controllo perche viene ricalcolato in checkTot
            Passo = 9
            'giu060319 al TotaleMCheck è stato corretto quindi per le stampe non CONTROLLO PERCHE' E' SICURAMENTE DIVERSO PER I VECCHI DOCUMENTI
            ' ''If TotaleMCheck <> TotaleMMemo Then
            ' ''    Errore = "CONTROLLO Totali Lordo Merce(2) Documento (CheckTotDoc). Passo: " & Passo.ToString & " <br> " & _
            ' ''        "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " & _
            ' ''        "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            ' ''    Return False
            ' ''    Exit Function
            ' ''End If
            'GIU270520 CONTROLLO SOLO IL TOTALE NETTO DA PAGARE ??? TROPPO RISCOSCHIO 
            ImponibileCheck = FormatCurrency(ImponibileCheck, 2)
            ImpostaCheck = FormatCurrency(ImpostaCheck, 2)
            TotaleCheck = FormatCurrency(TotaleCheck, 2)
            If ImponibileCheck <> ImponibileMemo Or ImpostaCheck <> ImpostaMemo Or TotaleCheck <> TotaleMemo Then
                Errore = "CONTROLLO Tot.(CheckTotDoc). Passo: " & Passo.ToString & " Imp.: " & ImponibileCheck.ToString & " " & ImponibileMemo.ToString & " IVA: " & ImpostaCheck.ToString & " " & ImpostaMemo.ToString & " Tot.: " & TotaleCheck.ToString & " " & TotaleMemo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
            'giu180118 giu190118
            Passo = 10
            If mySplitIVA Then
                myTotNettoPagareCK = ImponibileCheck
            Else
                myTotNettoPagareCK = TotaleCheck
            End If
            If myRitAcconto Then
                myTotNettoPagareCK = myTotNettoPagareCK - myTotaleRAMM
            End If
            'GIU140320
            If Abbuono <> 0 Then
                myTotNettoPagareCK = myTotNettoPagareCK + Abbuono
            End If
            '---------
            If myTotNettoPagareCK <> myTotNettoPagareMM Then
                'GIU300819 IL NETTO A PAGARE PRIMA DEL 2019 POTREBBE ESSERE COMPRESO IVA QUINDI RIPROVO DOPO AVER SOTTRATTO L'IVA (SPLIT PAYMENT)
                If myTotNettoPagareCK <> myTotNettoPagareMM - ImpostaMemo Then
                    Errore = "CONTROLLO Totale Netto da pagare (StampaDocumento). Passo: " & Passo.ToString & " TotNP.: " & myTotNettoPagareCK.ToString & " " & myTotNettoPagareMM.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
                '--------------------------
            End If
            '---------
            'giu020519
            If myTotNettoPagareCK = 0 And myTotDeduzioniCK = 0 And myTotLordoMerceCK = 0 Then
                Errore = "CONTROLLO Totale Netto da pagare/Totale Deduzioni=0 / Tot.Lordo Merce=0 (StampaDocumento). Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
            '---------
        End If
        'giu030112 per i campi NULL in stampa
        Dim rsTBDL As DataRow
        Try
            If (DsPrinWebDoc.Tables("DocumentiDLotti").Rows.Count > 0) Then
                For Each rsTBDL In DsPrinWebDoc.Tables("DocumentiDLotti").Select("")
                    rsTBDL.BeginEdit()
                    If IsDBNull(rsTBDL!Cod_Articolo) Then rsTBDL!Cod_Articolo = ""
                    If IsDBNull(rsTBDL!Lotto) Then rsTBDL!Lotto = ""
                    If IsDBNull(rsTBDL!QtaColli) Then rsTBDL!QtaColli = 0
                    If IsDBNull(rsTBDL!Sfusi) Then rsTBDL!Sfusi = 0
                    If IsDBNull(rsTBDL!NSerie) Then rsTBDL!NSerie = ""
                    rsTBDL.EndEdit()
                    rsTBDL.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. DocumentiDLotti CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '---------------------------------------------------------------------------------------------
        'SWScontiDoc per stampare il RPT con o senza la colonna sconti PREVENTIVI/ORDINI
        Passo = 11
        'GIU020512 OK SU TUTTI
        If (DsPrinWebDoc.Tables("DocumentiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
            SWSconti = True
        Else
            SWSconti = False
        End If
        '---------------------------------------------------------------------------------------------
        Passo = 12
        strSQL = "Select * From Ditta WHERE Codice = '" & CodiceDitta.Trim & "'"
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, DsPrinWebDoc, "Ditta")
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Ditta" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        'giu191211
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Cliente")) Then
            TabCliFor = "Cli"
        ElseIf Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Fornitore")) Then
            TabCliFor = "For"
        Else
            TabCliFor = "Cli"
        End If
        '----------
        If TabCliFor = "Cli" Then
            If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Cliente")) Then
                strSQL = "Select * From Clienti WHERE Codice_CoGe = '" & DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Cliente") & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Clienti" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            Else
                'GIU140513 Stato AS Nazione
                Dim strCodice As String = DsPrinWebDoc.DocumentiT.Rows(0).Item("IDAnagrProvv").ToString.Trim
                strSQL = "Select AnagrProvv.*, " & strCodice & " AS Codice_CoGe, Ragione_Sociale AS Rag_Soc, '' AS NumeroCivico, Stato AS Nazione From AnagrProvv WHERE IDAnagrProvv = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("IDAnagrProvv") & ""
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. ClientiProvv" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                'Aggiorno in Clienti il Codice
                If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                    For Each rsTestata In DsPrinWebDoc.Tables("DocumentiT").Select("")
                        rsTestata.BeginEdit()
                        rsTestata![Cod_Cliente] = strCodice
                        rsTestata.EndEdit()
                        rsTestata.AcceptChanges() 'giu030112
                    Next
                End If
            End If
        ElseIf TabCliFor = "For" Then
            If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Fornitore")) Then
                strSQL = "Select * From Fornitori WHERE Codice_CoGe = '" & DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Fornitore") & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Fornitori" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                'Aggiorno in Clienti il Codice
                If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                    For Each rsTestata In DsPrinWebDoc.Tables("DocumentiT").Select("")
                        rsTestata.BeginEdit()
                        rsTestata![Cod_Cliente] = DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Fornitore")
                        rsTestata.EndEdit()
                        rsTestata.AcceptChanges() 'giu030112
                    Next
                End If
            Else
                'GIU140513 Stato AS Nazione
                Dim strCodice As String = DsPrinWebDoc.DocumentiT.Rows(0).Item("IDAnagrProvv").ToString.Trim
                strSQL = "Select AnagrProvv.*, " & strCodice & " AS Codice_CoGe, Ragione_Sociale AS Rag_Soc '' AS NumeroCivico, Stato AS Nazione From AnagrProvv WHERE IDAnagrProvv = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("IDAnagrProvv") & ""
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. ClientiProvv" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                ''Aggiorno in Fornitore il Codice
                'If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                '    For Each rsTestata In DsPrinWebDoc.Tables("DocumentiT").Select("")
                '        rsTestata.BeginEdit()
                '        rsTestata!Cod_Fornitore = strCodice
                '        rsTestata.EndEdit()
                '    Next
                'End If
                'Aggiorno in Clienti il Codice
                If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                    For Each rsTestata In DsPrinWebDoc.Tables("DocumentiT").Select("")
                        rsTestata.BeginEdit()
                        rsTestata![Cod_Cliente] = strCodice
                        rsTestata.EndEdit()
                        rsTestata.AcceptChanges() 'giu030112
                    Next
                End If
            End If
        End If
        'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
        Dim rsTBCliFor As DataRow
        myNazione = ""
        Try
            If (DsPrinWebDoc.Tables("Clienti").Rows.Count > 0) Then
                For Each rsTBCliFor In DsPrinWebDoc.Tables("Clienti").Select("")
                    rsTBCliFor.BeginEdit()
                    If IsDBNull(rsTBCliFor!Rag_Soc) Then rsTBCliFor!Rag_Soc = ""
                    If IsDBNull(rsTBCliFor!Indirizzo) Then rsTBCliFor!Indirizzo = ""
                    If IsDBNull(rsTBCliFor!Localita) Then rsTBCliFor!Localita = ""
                    If IsDBNull(rsTBCliFor!CAP) Then rsTBCliFor!CAP = ""
                    If IsDBNull(rsTBCliFor!Provincia) Then rsTBCliFor!Provincia = ""
                    If IsDBNull(rsTBCliFor!Nazione) Then rsTBCliFor!Nazione = ""
                    If rsTBCliFor!Nazione.ToString.Trim = "" Then rsTBCliFor!Nazione = "I" 'GIU140513
                    myNazione = rsTBCliFor!Nazione
                    If IsDBNull(rsTBCliFor!Telefono1) Then rsTBCliFor!Telefono1 = ""
                    If IsDBNull(rsTBCliFor!Telefono2) Then rsTBCliFor!Telefono2 = ""
                    If IsDBNull(rsTBCliFor!Fax) Then rsTBCliFor!Fax = ""
                    If IsDBNull(rsTBCliFor!Codice_Fiscale) Then rsTBCliFor!Codice_Fiscale = ""
                    If IsDBNull(rsTBCliFor!Partita_IVA) Then rsTBCliFor!Partita_IVA = ""
                    If IsDBNull(rsTBCliFor!Denominazione) Then rsTBCliFor!Denominazione = ""
                    If IsDBNull(rsTBCliFor!Titolare) Then rsTBCliFor!Titolare = ""
                    If IsDBNull(rsTBCliFor!Email) Then rsTBCliFor!Email = ""
                    If IsDBNull(rsTBCliFor!ABI_N) Then rsTBCliFor!ABI_N = ""
                    If IsDBNull(rsTBCliFor!CAB_N) Then rsTBCliFor!CAB_N = ""
                    If IsDBNull(rsTBCliFor!Provincia_Estera) Then rsTBCliFor!Provincia_Estera = ""
                    If IsDBNull(rsTBCliFor!IndirizzoSenzaNumero) Then rsTBCliFor!IndirizzoSenzaNumero = ""
                    If IsDBNull(rsTBCliFor!NumeroCivico) Then rsTBCliFor!NumeroCivico = ""
                    If IsDBNull(rsTBCliFor!Note) Then rsTBCliFor!Note = ""
                    If IsDBNull(rsTBCliFor!IVASosp) Then rsTBCliFor!IVASosp = 0 'giu070212
                    If IsDBNull(rsTBCliFor!IPA) Then rsTBCliFor!IPA = "" 'GIU120814
                    If IsDBNull(rsTBCliFor!SplitIVA) Then rsTBCliFor!SplitIVA = False 'GIU150215
                    'giu140620
                    If IsDBNull(rsTBCliFor!IBAN_Ditta) Then rsTBCliFor!IBAN_Ditta = ""
                    If IsDBNull(rsTBCliFor!EmailInvioScad) Then rsTBCliFor!EmailInvioScad = ""
                    If IsDBNull(rsTBCliFor!EmailInvioFatt) Then rsTBCliFor!EmailInvioFatt = ""
                    If IsDBNull(rsTBCliFor!PECEmail) Then rsTBCliFor!PECEmail = ""
                    rsTBCliFor.EndEdit()
                    rsTBCliFor.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Clienti/Fornitori. CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '-Destinazione Merce
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Filiale")) Then
            strSQL = "Select * From DestClienti WHERE Progressivo = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Filiale") & ""
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "DestClienti", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. DestClienti" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        End If
        'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
        Dim rsTBDest As DataRow
        Try
            If (DsPrinWebDoc.Tables("DestClienti").Rows.Count > 0) Then
                For Each rsTBDest In DsPrinWebDoc.Tables("DestClienti").Select("")
                    rsTBDest.BeginEdit()
                    If IsDBNull(rsTBDest!Ragione_Sociale) Then rsTBDest!Ragione_Sociale = ""
                    If IsDBNull(rsTBDest!Indirizzo) Then rsTBDest!Indirizzo = ""
                    If IsDBNull(rsTBDest!Localita) Then rsTBDest!Localita = ""
                    If IsDBNull(rsTBDest!CAP) Then rsTBDest!CAP = ""
                    If IsDBNull(rsTBDest!Provincia) Then rsTBDest!Provincia = ""
                    If IsDBNull(rsTBDest!Stato) Then rsTBDest!Stato = ""
                    If IsDBNull(rsTBDest!Telefono1) Then rsTBDest!Telefono1 = ""
                    If IsDBNull(rsTBDest!Telefono2) Then rsTBDest!Telefono2 = ""
                    If IsDBNull(rsTBDest!Fax) Then rsTBDest!Fax = ""
                    If IsDBNull(rsTBDest!Denominazione) Then rsTBDest!Denominazione = ""
                    If IsDBNull(rsTBDest!Riferimento) Then rsTBDest!Riferimento = ""
                    If IsDBNull(rsTBDest!Email) Then rsTBDest!Email = ""
                    If IsDBNull(rsTBDest!Tipo) Then rsTBDest!Tipo = ""
                    rsTBDest.EndEdit()
                    rsTBDest.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Destinazione Merce CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '-
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Pagamento")) Then
            strSQL = "Select * From Pagamenti WHERE Codice = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Pagamento")
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Pagamenti", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Pagamenti" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            DsPrinWebDoc.Pagamenti.Clear()
        End If
        '-
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Valuta")) Then
            strSQL = "Select * From Valute WHERE Codice = '" & DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Valuta") & "'"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Valute", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Valute" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Valute WHERE Codice = 'Z'"
            DsPrinWebDoc.Valute.Clear()
        End If
        'GIU01122011 ATTENZIONE SE ESTRAE 2 RKS DUPLICA I DETTAGLI DEGLI ARTICOLO
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("IBAN")) Then
            strSQL = "Select TOP 1 * From BancheIBAN WHERE (IBAN = '" & DsPrinWebDoc.DocumentiT.Rows(0).Item("IBAN") & "')"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "BancheIBAN", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. BancheIBAN" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        ElseIf Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("ABI")) And Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("CAB")) Then
            If DsPrinWebDoc.DocumentiT.Rows(0).Item("ABI").ToString.Trim <> "" And DsPrinWebDoc.DocumentiT.Rows(0).Item("CAB").ToString.Trim <> "" Then
                strSQL = "Select TOP 1 * From BancheIBAN WHERE (ABI = '" & DsPrinWebDoc.DocumentiT.Rows(0).Item("ABI") & "') And (CAB = '" & DsPrinWebDoc.DocumentiT.Rows(0).Item("CAB") & "')"
            Else
                strSQL = "Select TOP 1 * From BancheIBAN WHERE (ABI = 'ZZZZZ')"
            End If
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "BancheIBAN", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. BancheIBAN" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From BancheIBAN WHERE (ABI = 'ZZZZZ')"
            DsPrinWebDoc.BancheIBAN.Clear()
        End If
        '---
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Vettore_1")) Then
            strSQL = "Select * From Vettori WHERE (Codice = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Vettore_1") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettori_1", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Vettore1" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Vettori WHERE (Codice = -1)"
            DsPrinWebDoc.Vettori_1.Clear()
        End If
        'giu030112 per i campi NULL in stampa
        Dim rsTBV1 As DataRow
        Try
            If (DsPrinWebDoc.Tables("Vettori_1").Rows.Count > 0) Then
                For Each rsTBV1 In DsPrinWebDoc.Tables("Vettori_1").Select("")
                    rsTBV1.BeginEdit()
                    If IsDBNull(rsTBV1!Descrizione) Then rsTBV1!Descrizione = ""
                    If IsDBNull(rsTBV1!Residenza) Then rsTBV1!Residenza = ""
                    If IsDBNull(rsTBV1!Localita) Then rsTBV1!Localita = ""
                    If IsDBNull(rsTBV1!Provincia) Then rsTBV1!Provincia = ""
                    If IsDBNull(rsTBV1!Partita_IVA) Then rsTBV1!Partita_IVA = ""
                    If IsDBNull(rsTBV1!Codice_CoGe) Then rsTBV1!Codice_CoGe = ""
                    rsTBV1.EndEdit()
                    rsTBV1.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Vettori_1 CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '-
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Vettore_2")) Then
            strSQL = "Select * From Vettori WHERE (Codice = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Vettore_2") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettore_2", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Vettore2" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Vettori WHERE (Codice = -1)"
            DsPrinWebDoc.Vettori_2.Clear()
        End If
        'giu030112 per i campi NULL in stampa
        Dim rsTBV2 As DataRow
        Try
            If (DsPrinWebDoc.Tables("Vettori_2").Rows.Count > 0) Then
                For Each rsTBV2 In DsPrinWebDoc.Tables("Vettori_2").Select("")
                    rsTBV2.BeginEdit()
                    If IsDBNull(rsTBV2!Descrizione) Then rsTBV2!Descrizione = ""
                    If IsDBNull(rsTBV2!Residenza) Then rsTBV2!Residenza = ""
                    If IsDBNull(rsTBV2!Localita) Then rsTBV2!Localita = ""
                    If IsDBNull(rsTBV2!Provincia) Then rsTBV2!Provincia = ""
                    If IsDBNull(rsTBV2!Partita_IVA) Then rsTBV2!Partita_IVA = ""
                    If IsDBNull(rsTBV2!Codice_CoGe) Then rsTBV2!Codice_CoGe = ""
                    rsTBV2.EndEdit()
                    rsTBV2.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Vettori_2 CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '-
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Vettore_3")) Then
            strSQL = "Select * From Vettori WHERE (Codice = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Vettore_3") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettore_3", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Vettore3" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Vettori WHERE (Codice = -1)"
            DsPrinWebDoc.Vettori_3.Clear()
        End If
        'giu030112 per i campi NULL in stampa
        Dim rsTBV3 As DataRow
        Try
            If (DsPrinWebDoc.Tables("Vettori_3").Rows.Count > 0) Then
                For Each rsTBV3 In DsPrinWebDoc.Tables("Vettori_3").Select("")
                    rsTBV3.BeginEdit()
                    If IsDBNull(rsTBV3!Descrizione) Then rsTBV3!Descrizione = ""
                    If IsDBNull(rsTBV3!Residenza) Then rsTBV3!Residenza = ""
                    If IsDBNull(rsTBV3!Localita) Then rsTBV3!Localita = ""
                    If IsDBNull(rsTBV3!Provincia) Then rsTBV3!Provincia = ""
                    If IsDBNull(rsTBV3!Partita_IVA) Then rsTBV3!Partita_IVA = ""
                    If IsDBNull(rsTBV3!Codice_CoGe) Then rsTBV3!Codice_CoGe = ""
                    rsTBV3.EndEdit()
                    rsTBV3.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Vettori_3 CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        'Aliquote IVA per il BLOCCO IVA
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Iva1")) Then
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Iva1") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA1", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Aliquota_IVA1" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = -1)"
            DsPrinWebDoc.Aliquote_IVA1.Clear()
        End If

        '-
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Iva2")) Then
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Iva2") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA2", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Aliquota_IVA2" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = -1)"
            DsPrinWebDoc.Aliquote_IVA2.Clear()
        End If

        '-
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Iva3")) Then
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Iva3") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA3", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Aliquota_IVA3" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = -1)"
            DsPrinWebDoc.Aliquote_IVA3.Clear()
        End If

        '-
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Iva4")) Then
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Iva4") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA4", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Aliquota_IVA4" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = -1)"
            DsPrinWebDoc.Aliquote_IVA4.Clear()
        End If

        '-CausMag
        If Not IsDBNull(DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Causale")) Then
            strSQL = "Select Codice, Descrizione From CausMag WHERE (Codice = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Causale") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "CausMag", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. CausMag" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select Codice, Descrizione From CausMag WHERE (Codice = -1)"
            DsPrinWebDoc.CausMag.Clear()
        End If
        'giu130412
        If Not IsDBNull(DsPrinWebDoc.Clienti.Rows(0).Item("Nazione")) Then
            strSQL = "Select * From Nazioni WHERE Codice = '" & DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim & "'"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Nazioni", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Nazioni" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
            'GIU150513 OBB. PER LA STAMPA altrimenti non mi stampa la LOCALITA'
            If DsPrinWebDoc.Nazioni.Select("").Count = 0 Then
                Dim newRow As DSPrintWeb_Documenti.NazioniRow = DsPrinWebDoc.Nazioni.NewNazioniRow
                With newRow
                    .BeginEdit()
                    .Codice = DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim
                    .Descrizione = "!!SCONOSCIUTA!!"
                    .Codice_ISO = DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim
                    .EndEdit()
                End With
                Try
                    DsPrinWebDoc.Nazioni.AddNazioniRow(newRow)
                    newRow = Nothing
                Catch Ex As Exception
                    Errore = Ex.Message & " - Tab. Nazioni inserimento (" & DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim & ") <br> " &
                        "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                DsPrinWebDoc.Nazioni.AcceptChanges()
            End If
        Else
            'strSQL = "Select * From Nazioni WHERE Codice = ''"
            DsPrinWebDoc.Nazioni.Clear()
            'GIU150513 OBB. PER LA STAMPA altrimenti non mi stampa la LOCALITA'
            Dim newRow As DSPrintWeb_Documenti.NazioniRow = DsPrinWebDoc.Nazioni.NewNazioniRow
            With newRow
                .BeginEdit()
                .Codice = "I"
                .Descrizione = "ITALIA"
                .Codice_ISO = "I"
                .EndEdit()
            End With
            Try
                DsPrinWebDoc.Nazioni.AddNazioniRow(newRow)
                newRow = Nothing
            Catch Ex As Exception
                Errore = Ex.Message & " - Tab. Nazioni inserimento (I)" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
            DsPrinWebDoc.Nazioni.AcceptChanges()
        End If
        'giu080923 errore su alcuni DDT giu310823 carico tutte le righe lotto come riga descrittiva - 2 per riga
        If SWStampaDocLotti = True Then
            Try
                If (DsPrinWebDoc.Tables("DocumentiDLotti").Rows.Count > 0) Then
                    strValore = ""
                    Dim NLS As Integer = 0 : Dim myNCollo As Integer : Dim NRigaPrec As Integer = 0
                    For Each rsTBDL In DsPrinWebDoc.Tables("DocumentiDLotti").Select("")

                        If NRigaPrec = 0 Then
                            NRigaPrec = rsTBDL!Riga
                        ElseIf NRigaPrec <> rsTBDL!Riga Then
                            If NLS <> 0 Then
                                Dim newRowDocD As DSPrintWeb_Documenti.DocumentiDRow = DsPrinWebDoc.DocumentiD.NewDocumentiDRow
                                With newRowDocD
                                    .BeginEdit()
                                    .IDDocumenti = CLng(myIDDoc)
                                    .Riga = NRigaPrec
                                    .NCollo = myNCollo
                                    .Descrizione = strValore.Trim
                                    .Prezzo = 0
                                    .Prezzo_Netto = 0
                                    .Qta_Allestita = 0
                                    .Qta_Evasa = 0
                                    .Qta_Impegnata = 0
                                    .Qta_Ordinata = 0
                                    .Qta_Prenotata = 0
                                    .Qta_Residua = 0
                                    .Importo = 0
                                    'giu170412
                                    .PrezzoAcquisto = 0
                                    .PrezzoListino = 0
                                    'giu190412
                                    .SWModAgenti = False
                                    .PrezzoCosto = 0
                                    .Qta_Inviata = 0
                                    .Qta_Fatturata = 0
                                    '---------
                                    .EndEdit()
                                End With
                                DsPrinWebDoc.DocumentiD.AddDocumentiDRow(newRowDocD)
                                newRowDocD = Nothing
                                '-
                            End If
                            NLS = 0
                            strValore = ""
                            NRigaPrec = rsTBDL!Riga
                        End If
                        '-
                        If strValore.Trim <> "" Then
                            strValore += " - (" + rsTBDL!QtaColli.ToString.Trim + ") "
                        Else
                            strValore += "(" + rsTBDL!QtaColli.ToString.Trim + ") "
                        End If
                        '------------
                        If IsDBNull(rsTBDL!Lotto) Then
                            'nulla
                        ElseIf rsTBDL!Lotto.ToString.Trim <> "" Then
                            If strValore.Trim <> "" Then
                                strValore += " - Lotto: " + rsTBDL!Lotto.ToString.Trim + " "
                            Else
                                strValore += "Lotto: " + rsTBDL!Lotto.ToString.Trim + " "
                            End If
                            '-
                            NLS += 1
                        End If
                        '------------
                        myNCollo = rsTBDL!NCollo
                        If NLS > 1 Then
                            Dim newRowDocD As DSPrintWeb_Documenti.DocumentiDRow = DsPrinWebDoc.DocumentiD.NewDocumentiDRow
                            With newRowDocD
                                .BeginEdit()
                                .IDDocumenti = CLng(myIDDoc)
                                .Riga = NRigaPrec
                                .NCollo = myNCollo
                                .Descrizione = strValore.Trim
                                .Prezzo = 0
                                .Prezzo_Netto = 0
                                .Qta_Allestita = 0
                                .Qta_Evasa = 0
                                .Qta_Impegnata = 0
                                .Qta_Ordinata = 0
                                .Qta_Prenotata = 0
                                .Qta_Residua = 0
                                .Importo = 0
                                'giu170412
                                .PrezzoAcquisto = 0
                                .PrezzoListino = 0
                                'giu190412
                                .SWModAgenti = False
                                .PrezzoCosto = 0
                                .Qta_Inviata = 0
                                .Qta_Fatturata = 0
                                '---------
                                .EndEdit()
                            End With
                            DsPrinWebDoc.DocumentiD.AddDocumentiDRow(newRowDocD)
                            newRowDocD = Nothing
                            '-
                            NLS = 0
                            strValore = ""
                        End If
                        '------------
                        If IsDBNull(rsTBDL!NSerie) Then
                            'nulla
                        ElseIf rsTBDL!NSerie.ToString.Trim <> "" Then
                            If strValore.Trim <> "" Then
                                strValore += " - N° Serie: " + rsTBDL!NSerie.ToString.Trim + " "
                            Else
                                strValore += "N° Serie: " + rsTBDL!NSerie.ToString.Trim + " "
                            End If
                            '
                            NLS += 1
                        End If
                        '------------
                        myNCollo = rsTBDL!NCollo
                        If NLS > 1 Then
                            Dim newRowDocD As DSPrintWeb_Documenti.DocumentiDRow = DsPrinWebDoc.DocumentiD.NewDocumentiDRow
                            With newRowDocD
                                .BeginEdit()
                                .IDDocumenti = CLng(myIDDoc)
                                .Riga = NRigaPrec
                                .NCollo = myNCollo
                                .Descrizione = strValore.Trim
                                .Prezzo = 0
                                .Prezzo_Netto = 0
                                .Qta_Allestita = 0
                                .Qta_Evasa = 0
                                .Qta_Impegnata = 0
                                .Qta_Ordinata = 0
                                .Qta_Prenotata = 0
                                .Qta_Residua = 0
                                .Importo = 0
                                'giu170412
                                .PrezzoAcquisto = 0
                                .PrezzoListino = 0
                                'giu190412
                                .SWModAgenti = False
                                .PrezzoCosto = 0
                                .Qta_Inviata = 0
                                .Qta_Fatturata = 0
                                '---------
                                .EndEdit()
                            End With
                            DsPrinWebDoc.DocumentiD.AddDocumentiDRow(newRowDocD)
                            newRowDocD = Nothing
                            '-
                            NLS = 0
                            strValore = ""
                        End If
                    Next
                    If NLS <> 0 Then
                        Dim newRowDocD As DSPrintWeb_Documenti.DocumentiDRow = DsPrinWebDoc.DocumentiD.NewDocumentiDRow
                        With newRowDocD
                            .BeginEdit()
                            .IDDocumenti = CLng(myIDDoc)
                            .Riga = NRigaPrec
                            .NCollo = myNCollo
                            .Descrizione = strValore.Trim
                            .Prezzo = 0
                            .Prezzo_Netto = 0
                            .Qta_Allestita = 0
                            .Qta_Evasa = 0
                            .Qta_Impegnata = 0
                            .Qta_Ordinata = 0
                            .Qta_Prenotata = 0
                            .Qta_Residua = 0
                            .Importo = 0
                            'giu170412
                            .PrezzoAcquisto = 0
                            .PrezzoListino = 0
                            'giu190412
                            .SWModAgenti = False
                            .PrezzoCosto = 0
                            .Qta_Inviata = 0
                            .Qta_Fatturata = 0
                            '---------
                            .EndEdit()
                        End With
                        DsPrinWebDoc.DocumentiD.AddDocumentiDRow(newRowDocD)
                        newRowDocD = Nothing
                    End If
                End If
                DsPrinWebDoc.DocumentiD.AcceptChanges()
            Catch ex As Exception
                Errore = ex.Message & " - Tab. DocumentiD inserimento LOTTI" & " <br> " &
                        "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        End If
        '==FINE CARICAMENTO ====================================
        ObjDB = Nothing

        Return True

    End Function
    'giu180723 per i caratteri speciali NSerie/Lotto
    Private Function NoCarSpecNoteSL(ByVal pNoteRitiro As String) As String
        NoCarSpecNoteSL = ""
        '------------------------------------------
        Dim myPos As Integer = 0
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim ListaSL As New List(Of String)
        Dim StrDato() As String
        myPos = InStr(pNoteRitiro, "§")
        If myPos > 0 Then
            StrDato = pNoteRitiro.Trim.Split("§")
            For I = 0 To StrDato.Count - 1
                mySL = Formatta.FormattaNomeFile(StrDato(I))
                If I > StrDato.Count - 1 Then
                    myNoteRitiro = ""
                Else
                    I += 1
                    myNoteRitiro = StrDato(I)
                End If
                ListaSL.Add(mySL + "§" + myNoteRitiro.Trim)
            Next
            'ok qui ripasso tutta la stringa senza i caratteri
            For II = 0 To ListaSL.Count - 1
                If NoCarSpecNoteSL.Trim <> "" Then
                    NoCarSpecNoteSL += "§"
                End If
                NoCarSpecNoteSL += ListaSL.Item(II).Trim
            Next
            If NoCarSpecNoteSL.Trim = "" Then
                NoCarSpecNoteSL = pNoteRitiro.Trim
            End If
        Else 'c'è una descrizione ma non assegnata a nessuna quindi appartiene a tutti i N° di serie
            'vediamo dopo Call SetNoteSLALLApp(pNoteRitiro)
            NoCarSpecNoteSL = pNoteRitiro.Trim
        End If
    End Function
    Public Function StampaContratto(ByVal IdDocumento As Long, ByVal TipoDoc As String, ByVal CodiceDitta As String,
                                    ByVal _Esercizio As String, ByVal TabCliFor As String, ByRef DsPrinWebDoc As DSPrintWeb_Documenti,
                                    ByRef ObjReport As Object, ByRef SWSconti As Boolean, ByRef Errore As String) As Boolean
        'giu270612
        myIDDoc = IdDocumento.ToString.Trim
        myTipoDoc = TipoDoc.Trim
        myDataDoc = ""
        myNDoc = ""
        '---------
        If TipoDoc.Trim = "" Then
            Errore = "Tipo Contratto sconosciuto. Passo: 0 <br>" &
           "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        Else
            _Esercizio = ""
        End If
        'GIU01122011 ATTENZIONE SE ESTRAE 2 RKS DUPLICA I DETTAGLI DEGLI ARTICOLI
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim Passo As Integer = 0
        Dim SqlConnDoc As SqlConnection
        Dim SqlAdapDoc As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Try
            SqlConnDoc = New SqlConnection
            SqlAdapDoc = New SqlDataAdapter
            SqlDbSelectCmd = New SqlCommand

            SqlAdapDoc.SelectCommand = SqlDbSelectCmd
            SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario, _Esercizio) 'GIU180419
            SqlDbSelectCmd.CommandText = "get_ConTByIDDocumenti"
            SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectCmd.Connection = SqlConnDoc
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            Dim SqlAdapDocDett As New SqlDataAdapter
            Dim SqlDbSelectDettCmd As New SqlCommand
            SqlDbSelectDettCmd.CommandText = "get_ConDByIDDocumenti"
            SqlDbSelectDettCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettCmd.Connection = SqlConnDoc
            SqlDbSelectDettCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNum", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DurataNumRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '
            SqlAdapDocDett.SelectCommand = SqlDbSelectDettCmd

            'GIU301111 LOTTI
            Dim SqlAdapDocDettL As New SqlDataAdapter
            Dim SqlDbSelectDettCmdL As New SqlCommand
            SqlDbSelectDettCmdL.CommandText = "get_ConDLByIDDocRiga"
            SqlDbSelectDettCmdL.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettCmdL.Connection = SqlConnDoc
            SqlDbSelectDettCmdL.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDettCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocDettL.SelectCommand = SqlDbSelectDettCmdL

            '==============CARICAMENTO DATASET ===================
            Passo = 1
            DsPrinWebDoc.Clear()
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlAdapDoc.Fill(DsPrinWebDoc.ContrattiT)
            Passo = 2
            SqlDbSelectDettCmd.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlDbSelectDettCmd.Parameters.Item("@DurataNum").Value = 1 'fisso per le attività per periodo
            SqlDbSelectDettCmd.Parameters.Item("@DurataNumRiga").Value = DBNull.Value
            SqlAdapDocDett.Fill(DsPrinWebDoc.ContrattiD)
            '''Passo = 3
            '''SqlDbSelectDettCmdL.Parameters.Item("@IDDocumenti").Value = IdDocumento
            '''SqlDbSelectDettCmdL.Parameters.Item("@Riga").Value = DBNull.Value
            '''SqlAdapDocDettL.Fill(DsPrinWebDoc.ContrattiDLotti)
        Catch ex As Exception
            Errore = ex.Message & " - Contratto lettura testata e dettagli. Passo: " & Passo.ToString & " <br> " &
            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '---------------------------------------------------------------------------------------------
        'giu021211 RIATTIVATA ROUTINE DI CALCOLO IMPORTO E DOCUMENTO PER IL PROBLEMA DEI DATI SALVATI
        'DA UNA SESSIONE PRECEDENTE (BACK SULLA PAGINA WEB) (CONTROLLO DETTAGLIO CON TESTATA DOCUMENTO
        '---------------------------------------------------------------------------------------------
        Passo = 4
        Dim strErrore As String = ""
        'Listino documento
        Dim Listino As String = "1"
        'Valuta per i decimali per il calcolo 
        Dim DecimaliVal As String = "2" ' Session(CSTDECIMALIVALUTADOC)
        Dim CodValuta As String = "Euro"
        '---
        Listino = "1"
        CodValuta = "Euro"
        DecimaliVal = "2" 'Euro
        SpeseIncasso = "0"
        SpeseTrasporto = "0"
        SpeseImballo = "0"
        SpeseVarie = "0"
        RegimeIVA = "0"
        ScCassa = "0"
        Abbuono = "0"
        ImponibileMemo = 0
        ImpostaMemo = 0
        TotaleMemo = 0
        TotaleMMemo = 0
        '-
        ImponibileCheck = 0
        ImpostaCheck = 0
        TotaleCheck = 0
        TotaleMCheck = 0

        mySplitIVA = False
        myRitAcconto = False
        myTotaleRACK = 0
        myTotaleRAMM = 0
        myTotNettoPagareCK = 0
        myTotNettoPagareMM = 0
        '---
        'giu021117 Richiesta Zibordi del 27/10/2017 (Solo per PR/OC riportare le Iniziali=Codice Operatore)
        Dim strSQL As String = ""
        Dim X As Integer = 0
        Dim ObjDB As New DataBaseUtility
        '--------------------------------------------------------------------------------------------------
        Dim SWTabCliFor As String = ""
        'giu020512
        Dim myCodiceCoGe As String = ""
        Dim myCodFiliale As String = ""
        Dim myNazione As String = ""
        Dim rsTestata As DataRow
        Try
            If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                'GIU020512 STAMPALL
                For Each rsTestata In DsPrinWebDoc.Tables("ContrattiT").Select("")
                    'giu270712
                    myDataDoc = IIf(IsDBNull(rsTestata!Data_Doc), "", rsTestata!Data_Doc)
                    myNDoc = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero)
                    myTipoDoc = IIf(IsDBNull(rsTestata!Tipo_Doc), "", rsTestata!Tipo_Doc)
                    '---------
                    'GIU020512()
                    If Not IsDBNull(rsTestata![Cod_Cliente]) Then
                        TabCliFor = "Cli"
                        myCodiceCoGe = rsTestata![Cod_Cliente].ToString.Trim
                    ElseIf Not IsDBNull(rsTestata![Cod_Fornitore]) Then
                        TabCliFor = "For"
                        myCodiceCoGe = rsTestata![Cod_Fornitore].ToString.Trim
                        'Dim strCodice As String = DsPrinWebDoc.ContrattiT.Rows(0).Item("IDAnagrProvv").ToString.Trim
                    ElseIf Not IsDBNull(rsTestata![IDAnagrProvv]) Then
                        TabCliFor = "Provv"
                        myCodiceCoGe = rsTestata![IDAnagrProvv].ToString.Trim
                    Else
                        TabCliFor = "Cli"
                        myCodiceCoGe = ""
                    End If
                    '--
                    If Not IsDBNull(rsTestata![Cod_Filiale]) Then
                        myCodFiliale = rsTestata![Cod_Filiale].ToString.Trim
                    End If
                    '---------
                    Listino = rsTestata![Listino]
                    CodValuta = rsTestata![Cod_Valuta]
                    SpeseIncasso = rsTestata![Spese_Incasso]
                    SpeseTrasporto = rsTestata![Spese_Trasporto]
                    SpeseImballo = rsTestata![Spese_Imballo]
                    SpeseVarie = rsTestata![SpeseVarie]
                    RegimeIVA = rsTestata![Cod_Iva]
                    ScCassa = rsTestata![Sconto_Cassa]
                    Abbuono = rsTestata![Abbuono]
                    '-
                    ImponibileMemo += rsTestata![Imponibile1] + rsTestata![Imponibile2] + rsTestata![Imponibile3] + rsTestata![Imponibile4]
                    ImpostaMemo += rsTestata![Imposta1] + rsTestata![Imposta2] + rsTestata![Imposta3] + rsTestata![Imposta4]
                    TotaleMemo += rsTestata![Totale]
                    TotaleMMemo += rsTestata![TotaleM]
                    'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
                    rsTestata.BeginEdit()
                    If IsDBNull(rsTestata!Cod_Filiale) Then rsTestata!Cod_Filiale = 0
                    If IsDBNull(rsTestata!Destinazione1) Then rsTestata!Destinazione1 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione2 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione3 = ""
                    If IsDBNull(rsTestata!ABI) Then rsTestata!ABI = ""
                    If IsDBNull(rsTestata!CAB) Then rsTestata!CAB = ""
                    If IsDBNull(rsTestata!IBAN) Then rsTestata!IBAN = ""
                    If IsDBNull(rsTestata!ContoCorrente) Then rsTestata!ContoCorrente = ""
                    If IsDBNull(rsTestata!CIG) Then rsTestata!CIG = ""
                    If IsDBNull(rsTestata!CUP) Then rsTestata!CUP = ""
                    If IsDBNull(rsTestata!FatturaPA) Then rsTestata!FatturaPA = False 'GIU120814
                    'GIU150118
                    If IsDBNull(rsTestata!SplitIVA) Then rsTestata!SplitIVA = False
                    mySplitIVA = rsTestata!SplitIVA
                    If IsDBNull(rsTestata!RitAcconto) Then rsTestata!RitAcconto = False
                    myRitAcconto = rsTestata!RitAcconto
                    If IsDBNull(rsTestata!ImponibileRA) Then rsTestata!ImponibileRA = 0
                    If IsDBNull(rsTestata!PercRA) Then rsTestata!PercRA = 0
                    If IsDBNull(rsTestata!TotaleRA) Then rsTestata!TotaleRA = 0
                    myTotaleRAMM = rsTestata!TotaleRA
                    If rsTestata!RitAcconto = False Then
                        If rsTestata!ImponibileRA <> 0 Or rsTestata!PercRA <> 0 Or rsTestata!TotaleRA <> 0 Then
                            Errore = "Errore calcolo Ritenuta d'acconto. (StampaContratto). Passo: " & Passo.ToString & " <br> " &
                            "Per risolvere il problema controllare la sezione Totali modificando e aggiornando i dati Rit.Acconto." & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End If
                    Else
                        If IsDBNull(rsTestata!ImponibileRA) Then rsTestata!ImponibileRA = 0
                        If IsDBNull(rsTestata!PercRA) Then rsTestata!PercRA = 0
                        If IsDBNull(rsTestata!TotaleRA) Then rsTestata!TotaleRA = 0
                        If rsTestata!ImponibileRA <> 0 And rsTestata!PercRA <> 0 Then
                            myTotaleRACK = rsTestata!ImponibileRA * rsTestata!PercRA / 100
                            myTotaleRACK = FormatCurrency(myTotaleRACK, 2)
                        Else
                            myTotaleRACK = 0
                        End If
                        If rsTestata!ImponibileRA = 0 Or rsTestata!PercRA = 0 Or rsTestata!TotaleRA = 0 Or
                            myTotaleRACK <> myTotaleRAMM Then
                            Errore = "Errore calcolo Ritenuta d'acconto. (StampaContratto). Passo: " & Passo.ToString & " <br> " &
                            "Per risolvere il problema controllare la sezione Totali modificando e aggiornando i dati Rit.Acconto." & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End If
                    End If
                    If IsDBNull(rsTestata!TotNettoPagare) Then rsTestata!TotNettoPagare = 0
                    myTotNettoPagareMM = rsTestata!TotNettoPagare
                    '---------
                    'giu021117
                    strSQL = ""
                    If Not IsDBNull(rsTestata!ModificatoDa) Then
                        strSQL = rsTestata!ModificatoDa.ToString.Trim
                        X = InStr(strSQL, " ")
                        If X > 0 Then
                            strSQL = Mid(strSQL, 1, X - 1)
                        End If
                    ElseIf Not IsDBNull(rsTestata!InseritoDa) Then
                        strSQL = rsTestata!InseritoDa.ToString.Trim
                        X = InStr(strSQL, " ")
                        If X > 0 Then
                            strSQL = Mid(strSQL, 1, X - 1)
                        End If
                    End If
                    If strSQL <> "" Then
                        strSQL = "Select Codice, Nome From Operatori WHERE (Nome LIKE N'%" & strSQL & "%') "
                        Try
                            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, DsPrinWebDoc, "Operatori")
                            If (DsPrinWebDoc.Tables("Operatori").Rows.Count > 0) Then
                                If Not IsDBNull(DsPrinWebDoc.Operatori.Rows(0).Item("Codice")) Then
                                    strSQL = DsPrinWebDoc.Operatori.Rows(0).Item("Codice").ToString.Trim
                                End If
                            End If
                        Catch ex As Exception
                            Errore = ex.Message & " - Inserimento dati nella Tabella Operatori. Passo: " & Passo.ToString
                            Return False
                            Exit Function
                        End Try
                    End If
                    If strSQL = "" Then strSQL = "XX"
                    rsTestata!Iniziali = strSQL
                    '---------
                    'giu070319
                    If IsDBNull(rsTestata!FatturaAC) Then rsTestata!FatturaAC = False
                    If IsDBNull(rsTestata!ScGiacenza) Then rsTestata!ScGiacenza = False
                    If IsDBNull(rsTestata!Acconto) Then rsTestata!Acconto = 0
                    If IsDBNull(rsTestata!TipoRapportoPA) Then rsTestata!TipoRapportoPA = ""
                    If IsDBNull(rsTestata!Bollo) Then rsTestata!Bollo = 0
                    If IsDBNull(rsTestata!BolloACaricoDel) Then rsTestata!BolloACaricoDel = ""
                    If IsDBNull(rsTestata!NoteRitiro) Then rsTestata!NoteRitiro = ""
                    '---------
                    rsTestata!NoteRitiro = NoCarSpecNoteSL(rsTestata!NoteRitiro.ToString.Trim)
                    rsTestata.EndEdit()
                    rsTestata.AcceptChanges()
                Next
            Else
                Errore = "Errore - Contratto lettura testata. Passo: " & Passo.ToString & " <br> " &
                "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
        Catch ex As Exception
            Listino = "1"
            CodValuta = "Euro"
            DecimaliVal = "2" 'Euro
            SpeseIncasso = "0"
            SpeseTrasporto = "0"
            SpeseImballo = "0"
            SpeseVarie = "0"
            RegimeIVA = "0"
            ScCassa = "0"
            Abbuono = "0"
            ImponibileMemo = 0
            ImpostaMemo = 0
            TotaleMemo = 0
            TotaleMMemo = 0
            myTotaleRAMM = 0
            myTotaleRACK = 0
            myTotNettoPagareCK = 0
            myTotNettoPagareMM = 0
            Errore = ex.Message & " - Contratto lettura testata. Passo: " & Passo.ToString & " <br> " &
            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        If TipoDoc <> myTipoDoc Then 'giu190419
            TipoDoc = myTipoDoc
        End If
        Passo = 5
        'Ricalcolo Importo
        Dim MyImporto As Decimal = 0
        Dim rsDettagli As DataRow
        'giu220223 memorizzo le scadenze consumabili da accodare alle NOTE INTERVENTO
        Dim ListaScadConsumabili As New List(Of String)
        Dim mySLScadCons As String = ""
        '----------------------------------------------------------------------------
        If (DsPrinWebDoc.Tables("ContrattiD").Rows.Count > 0) Then
            Dim myQtaO As Decimal = 0 : Dim myQtaE As Decimal = 0 : Dim myQtaR As Decimal = 0
            Dim myQuantita As Decimal = 0 : Dim SWQta = ""
            '---- Calcolo sconto su 
            Dim ScontoSuImporto As Boolean = True
            Dim ScCassaDett As Boolean = False 'giu010119 
            Try
                'giu190419 se seve altro aggiungere all'occorrenza ora solo i successivi 2 
                ScontoSuImporto = App.GetParGenAnno(_Esercizio, strErrore).CalcoloScontoSuImporto 'giu190419 App.GetParamGestAzi(_Esercizio).CalcoloScontoSuImporto
                ScCassaDett = App.GetParGenAnno(_Esercizio, strErrore).ScCassaDett 'giu190419 App.GetParamGestAzi(_Esercizio).ScCassaDett
                If strErrore.Trim <> "" Then
                    Errore = strErrore & " - CONTROLLO Contratto (GetParGenAnno). Passo: " & Passo.ToString & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
            Catch ex As Exception
                Errore = ex.Message.Trim & " " & strErrore & " - CONTROLLO Contratto (GetParGenAnno). Passo: " & Passo.ToString & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
                ScontoSuImporto = True
                ScCassaDett = False
            End Try
            '-------------------------------------------------------------------------------------
            Passo = 6
            'GIU020512 
            Dim SaveCodFil As Integer = 0
            Dim SaveRespVisite As Integer = 0 'giu141123
            For Each rsDettagli In DsPrinWebDoc.Tables("ContrattiD").Select("", "Riga")
                Select Case Left(TipoDoc, 1)
                    Case "O"
                        myQuantita = rsDettagli![Qta_Ordinata]
                        SWQta = "O"
                    Case Else
                        If TipoDoc = "PR" Or TipoDoc = "CA" Or TipoDoc = "TC" Then 'GIU021219
                            myQuantita = rsDettagli![Qta_Ordinata]
                            SWQta = "O"
                        Else
                            myQuantita = rsDettagli![Qta_Evasa]
                            SWQta = "E"
                        End If
                End Select
                'giu020519 FATTURE PER ACCONTI 
                rsDettagli.BeginEdit()
                'giu270420 modello
                If IsDBNull(rsDettagli!Serie) Then rsDettagli!Serie = ""
                'giu180723
                rsDettagli!Serie = Formatta.FormattaNomeFile(rsDettagli!Serie)
                If IsDBNull(rsDettagli!Lotto) Then rsDettagli!Lotto = ""
                rsDettagli!Lotto = Formatta.FormattaNomeFile(rsDettagli!Lotto)
                If IsDBNull(rsDettagli!SerieLotto) Then rsDettagli!SerieLotto = ""
                rsDettagli!SerieLotto = Formatta.FormattaNomeFile(rsDettagli!SerieLotto)
                '-
                If IsDBNull(rsDettagli!QtaDurataNumR0) Then rsDettagli!QtaDurataNumR0 = 0
                If rsDettagli!QtaDurataNumR0 = 0 Then
                    rsDettagli!Modello = ""
                ElseIf rsDettagli!QtaDurataNumR0 = 1 Then
                    rsDettagli!Modello = "HS1"
                ElseIf rsDettagli!QtaDurataNumR0 = 2 Then
                    rsDettagli!Modello = "FR2"
                ElseIf rsDettagli!QtaDurataNumR0 = 3 Then
                    rsDettagli!Modello = "FR3"
                ElseIf rsDettagli!QtaDurataNumR0 = 4 Then
                    rsDettagli!Modello = "FRX"
                ElseIf rsDettagli!QtaDurataNumR0 = 5 Then
                    rsDettagli!Modello = "C1"
                ElseIf rsDettagli!QtaDurataNumR0 = 6 Then
                    rsDettagli!Modello = "C2"
                Else
                    rsDettagli!Modello = "???"
                End If
                '.
                If IsDBNull(rsDettagli!Cod_Filiale) Then rsDettagli!Cod_Filiale = 0
                '-Destinazione Merce Apparecchiature
                If rsDettagli!Cod_Filiale <> SaveCodFil Then
                    SaveCodFil = rsDettagli!Cod_Filiale
                    If DsPrinWebDoc.DestClienti.Select("Progressivo = " & rsDettagli!Cod_Filiale.ToString.Trim).Length = 0 Then
                        strSQL = "Select * From DestClienti WHERE Progressivo = " & rsDettagli!Cod_Filiale.ToString.Trim & ""
                        Try
                            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "DestClienti", _Esercizio) 'giu230419
                        Catch ex As Exception
                            Errore = ex.Message & " - Tab. DestClienti (DETT)" & " <br> " &
                                "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End Try
                    End If
                End If
                'GIU141123 RESPVISITE APP
                If IsDBNull(rsDettagli!RespVisite) Then rsDettagli!RespVisite = 0
                If rsDettagli!RespVisite <> SaveRespVisite Then
                    SaveRespVisite = rsDettagli!RespVisite
                    If DsPrinWebDoc.RespVisite.Select("Codice = " & rsDettagli!RespVisite.ToString.Trim).Length = 0 Then
                        strSQL = "Select * From RespVisite WHERE (Codice = " & rsDettagli!RespVisite.ToString.Trim & ") "
                        Try
                            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, DsPrinWebDoc, "RespVisite") 'giu230419
                        Catch ex As Exception
                            Errore = ex.Message & " - Tab. RespVisite (DETT)" & " <br> " &
                                "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End Try
                    End If
                End If
                '-------------------------
                If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
                If IsDBNull(rsDettagli!SWCalcoloTot) Then rsDettagli!SWCalcoloTot = False
                'giu220223
                If IsDBNull(rsDettagli!Cod_Articolo) Then rsDettagli!Cod_Articolo = ""
                If IsDBNull(rsDettagli!Descrizione) Then rsDettagli!Descrizione = ""
                '---------
                rsDettagli.EndEdit()
                rsDettagli.AcceptChanges()
                'CONTROLLO IMPORTO CHE SIANO UGLUALI
                TotaleCheck += rsDettagli![Importo]
                If rsDettagli!SWCalcoloTot = True Then myQuantita = 0
                MyImporto = CalcolaImporto(rsDettagli![Prezzo], myQuantita,
                        rsDettagli![Sconto_1],
                        rsDettagli![Sconto_2],
                        rsDettagli![Sconto_3],
                        rsDettagli![Sconto_4],
                        rsDettagli![Sconto_Pag],
                        rsDettagli![ScontoValore],
                        rsDettagli![Importo],
                        ScontoSuImporto,
                        CInt(DecimaliVal),
                        rsDettagli![Prezzo_Netto], ScCassaDett, CDec(ScCassa), rsDettagli!DedPerAcconto) 'giu010119 giu020519 DedPerAcconto
                If rsDettagli![Importo] <> MyImporto Then
                    Errore = "Errore CONTROLLO Contratto: Importo riga: (" & rsDettagli![Riga].ToString.Trim & ") diverso dall'importo riga ricalcolato. Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
                'giu290420 per i contratti non server giu270913
                rsDettagli![StampaLotti] = False 'GIU181219
                ' ''If myQuantita > 0 Then
                ' ''    If DsPrinWebDoc.Tables("ContrattiDLotti").Select("IDDocumenti=" & myIDDoc & " And Riga=" & rsDettagli![Riga].ToString.Trim).Length > 0 Then
                ' ''        rsDettagli![StampaLotti] = True
                ' ''    End If
                ' ''End If
                '---------
                'giu220223 carico ListaScadConsumabili
                If IsDBNull(rsDettagli!RefDataNC) Then
                    'nulla
                ElseIf IsDate(rsDettagli!RefDataNC) Then 'OK carico
                    If rsDettagli!Cod_Articolo <> "" Or rsDettagli!Descrizione <> "" Then
                        mySLScadCons = IIf(IsDBNull(rsDettagli!SerieLotto), "", rsDettagli!SerieLotto.ToString.Trim)
                        If mySLScadCons.Trim <> "" Then
                            '+ " - " + Mid(rsDettagli!Descrizione.ToString.Replace("§", " ").Trim, 1, 10)
                            ListaScadConsumabili.Add(mySLScadCons + "§" + " - " + IIf(IsDBNull(rsDettagli!DataSc), "", "(Anno Rif.") +
                                                     CDate(rsDettagli!DataSc).Date.ToString("yyyy") + ") " +
                                                     rsDettagli!Cod_Articolo.ToString.Replace("§", " ").Trim _
                                                     + " - " + rsDettagli!Descrizione.ToString.Replace("§", " ").Trim _
                                                     + " Scadenza: " + CDate(rsDettagli!RefDataNC).Date.ToString("dd/MM/yyyy"))
                        End If
                    End If
                End If
                '-------------------------------------
            Next
            Passo = 7
            'giu060319 al TotaleChek bisogna aggiungere l'IVA quindi il test verra fatto dopo CheckTotDoc che mi somma l'IVA MENTRE il Totale NettoAPagare è quello che puo non contenere IVA (SPLIT)
            ' ''If TotaleCheck <> TotaleMemo Then
            ' ''    Errore = "CONTROLLO Totali Merce(1) Contratto (CheckTotDoc). Passo: " & Passo.ToString & " <br> " & _
            ' ''        "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " & _
            ' ''        "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            ' ''    Return False
            ' ''    Exit Function
            ' ''End If
            '---------
            Passo = 8
            strErrore = ""
            'giu020512 per l stampa ALLL
            'GIU031013 IVA 21 E 22 DAL 01/10/2013
            Dim SWIVA2122 As Boolean = False
            'GIU210819 If _Esercizio.Trim = "2013" Then
            If CDate(myDataDoc) < CDate("01/10/2013") Then
                SWIVA2122 = True
            End If
            'GIU210819 End If
            '------------------------------------
            If CheckTotDocCA(DsPrinWebDoc, _Esercizio, TipoDoc, DecimaliVal, Listino, IdDocumento, strErrore, SWIVA2122) = False Then
                If strErrore.Trim <> "" Then
                    Errore = strErrore & " - CONTROLLO Contratto (CheckTotDocCA). Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
            End If
            'secondo controllo perche viene ricalcolato in checkTot
            Passo = 9
            'giu060319 al TotaleMCheck è stato corretto quindi per le stampe non CONTROLLO PERCHE' E' SICURAMENTE DIVERSO PER I VECCHI DOCUMENTI
            ' ''If TotaleMCheck <> TotaleMMemo Then
            ' ''    Errore = "CONTROLLO Totali Lordo Merce(2) Contratto (CheckTotDocCA). Passo: " & Passo.ToString & " <br> " & _
            ' ''        "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " & _
            ' ''        "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            ' ''    Return False
            ' ''    Exit Function
            ' ''End If
            ImponibileCheck = FormatCurrency(ImponibileCheck, 2)
            ImpostaCheck = FormatCurrency(ImpostaCheck, 2)
            TotaleCheck = FormatCurrency(TotaleCheck, 2)
            If ImponibileCheck <> ImponibileMemo Or ImpostaCheck <> ImpostaMemo Or TotaleCheck <> TotaleMemo Then
                Errore = "CONTROLLO Tot.(CheckTotDoc). Passo: " & Passo.ToString & " Imp.: " & ImponibileCheck.ToString & " " & ImponibileMemo.ToString & " IVA: " & ImpostaCheck.ToString & " " & ImpostaMemo.ToString & " Tot.: " & TotaleCheck.ToString & " " & TotaleMemo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
            'giu180118 giu190118
            Passo = 10
            If mySplitIVA Then
                myTotNettoPagareCK = ImponibileCheck
            Else
                myTotNettoPagareCK = TotaleCheck
            End If
            If myRitAcconto Then
                myTotNettoPagareCK = myTotNettoPagareCK - myTotaleRAMM
            End If
            'GIU140320
            If Abbuono <> 0 Then
                myTotNettoPagareCK = myTotNettoPagareCK + Abbuono
            End If
            '---------
            If myTotNettoPagareCK <> myTotNettoPagareMM Then
                'GIU300819 IL NETTO A PAGARE PRIMA DEL 2019 POTREBBE ESSERE COMPRESO IVA QUINDI RIPROVO DOPO AVER SOTTRATTO L'IVA (SPLIT PAYMENT)
                If myTotNettoPagareCK <> myTotNettoPagareMM - ImpostaMemo Then
                    Errore = "CONTROLLO Totale Netto da pagare (StampaContratto). Passo: " & Passo.ToString & " TotNP.: " & myTotNettoPagareCK.ToString & " " & myTotNettoPagareMM.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
                '--------------------------
            End If
            '---------
            'giu020519
            If myTotNettoPagareCK = 0 And myTotDeduzioniCK = 0 And myTotLordoMerceCK = 0 Then
                Errore = "CONTROLLO Totale Netto da pagare/Totale Deduzioni=0 / Tot.Lordo Merce=0 (StampaContratto). Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
            '---------
        End If
        'giu030112 per i campi NULL in stampa
        '''Dim rsTBDL As DataRow
        '''Try
        '''    If (DsPrinWebDoc.Tables("ContrattiDLotti").Rows.Count > 0) Then
        '''        For Each rsTBDL In DsPrinWebDoc.Tables("ContrattiDLotti").Select("")
        '''            rsTBDL.BeginEdit()
        '''            If IsDBNull(rsTBDL!Cod_Articolo) Then rsTBDL!Cod_Articolo = ""
        '''            If IsDBNull(rsTBDL!Lotto) Then rsTBDL!Lotto = ""
        '''            If IsDBNull(rsTBDL!QtaColli) Then rsTBDL!QtaColli = 0
        '''            If IsDBNull(rsTBDL!Sfusi) Then rsTBDL!Sfusi = 0
        '''            If IsDBNull(rsTBDL!NSerie) Then rsTBDL!NSerie = ""
        '''            rsTBDL.EndEdit()
        '''            rsTBDL.AcceptChanges()
        '''        Next
        '''    End If
        '''Catch ex As Exception
        '''    Errore = ex.Message & " - Tab. ContrattiDLotti CAMPI NULL" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''    Return False
        '''    Exit Function
        '''End Try
        '---------------------------------------------------------------------------------------------
        'SWScontiDoc per stampare il RPT con o senza la colonna sconti PREVENTIVI/ORDINI
        Passo = 11
        'giu290420 non serve
        SWSconti = False
        'GIU020512 OK SU TUTTI
        ' ''If (DsPrinWebDoc.Tables("ContrattiD").Select("Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0").Count > 0) Then
        ' ''    SWSconti = True
        ' ''Else
        ' ''    SWSconti = False
        ' ''End If
        '---------------------------------------------------------------------------------------------
        Passo = 12
        '''strSQL = "Select * From Ditta WHERE Codice = '" & CodiceDitta.Trim & "'"
        '''Try
        '''    ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, DsPrinWebDoc, "Ditta")
        '''Catch ex As Exception
        '''    Errore = ex.Message & " - Tab. Ditta" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''    Return False
        '''    Exit Function
        '''End Try
        'giu191211
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Cliente")) Then
            TabCliFor = "Cli"
        ElseIf Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Fornitore")) Then
            TabCliFor = "For"
        Else
            TabCliFor = "Cli"
        End If
        '----------
        If TabCliFor = "Cli" Then
            If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Cliente")) Then
                strSQL = "Select * From Clienti WHERE Codice_CoGe = '" & DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Cliente") & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Clienti" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            Else
                'GIU140513 Stato AS Nazione
                Dim strCodice As String = DsPrinWebDoc.ContrattiT.Rows(0).Item("IDAnagrProvv").ToString.Trim
                strSQL = "Select AnagrProvv.*, " & strCodice & " AS Codice_CoGe, Ragione_Sociale AS Rag_Soc, '' AS NumeroCivico, Stato AS Nazione From AnagrProvv WHERE IDAnagrProvv = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("IDAnagrProvv") & ""
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. ClientiProvv" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                'Aggiorno in Clienti il Codice
                If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                    For Each rsTestata In DsPrinWebDoc.Tables("ContrattiT").Select("")
                        rsTestata.BeginEdit()
                        rsTestata![Cod_Cliente] = strCodice
                        rsTestata.EndEdit()
                        rsTestata.AcceptChanges() 'giu030112
                    Next
                End If
            End If
        ElseIf TabCliFor = "For" Then
            If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Fornitore")) Then
                strSQL = "Select * From Fornitori WHERE Codice_CoGe = '" & DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Fornitore") & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Fornitori" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                'Aggiorno in Clienti il Codice
                If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                    For Each rsTestata In DsPrinWebDoc.Tables("ContrattiT").Select("")
                        rsTestata.BeginEdit()
                        rsTestata![Cod_Cliente] = DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Fornitore")
                        rsTestata.EndEdit()
                        rsTestata.AcceptChanges() 'giu030112
                    Next
                End If
            Else
                'GIU140513 Stato AS Nazione
                Dim strCodice As String = DsPrinWebDoc.ContrattiT.Rows(0).Item("IDAnagrProvv").ToString.Trim
                strSQL = "Select AnagrProvv.*, " & strCodice & " AS Codice_CoGe, Ragione_Sociale AS Rag_Soc '' AS NumeroCivico, Stato AS Nazione From AnagrProvv WHERE IDAnagrProvv = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("IDAnagrProvv") & ""
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. ClientiProvv" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                ''Aggiorno in Fornitore il Codice
                'If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                '    For Each rsTestata In DsPrinWebDoc.Tables("ContrattiT").Select("")
                '        rsTestata.BeginEdit()
                '        rsTestata!Cod_Fornitore = strCodice
                '        rsTestata.EndEdit()
                '    Next
                'End If
                'Aggiorno in Clienti il Codice
                If (DsPrinWebDoc.Tables("ContrattiT").Rows.Count > 0) Then
                    For Each rsTestata In DsPrinWebDoc.Tables("ContrattiT").Select("")
                        rsTestata.BeginEdit()
                        rsTestata![Cod_Cliente] = strCodice
                        rsTestata.EndEdit()
                        rsTestata.AcceptChanges() 'giu030112
                    Next
                End If
            End If
        End If
        'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
        Dim rsTBCliFor As DataRow
        myNazione = ""
        Try
            If (DsPrinWebDoc.Tables("Clienti").Rows.Count > 0) Then
                For Each rsTBCliFor In DsPrinWebDoc.Tables("Clienti").Select("")
                    rsTBCliFor.BeginEdit()
                    If IsDBNull(rsTBCliFor!Rag_Soc) Then rsTBCliFor!Rag_Soc = ""
                    If IsDBNull(rsTBCliFor!Indirizzo) Then rsTBCliFor!Indirizzo = ""
                    If IsDBNull(rsTBCliFor!Localita) Then rsTBCliFor!Localita = ""
                    If IsDBNull(rsTBCliFor!CAP) Then rsTBCliFor!CAP = ""
                    If IsDBNull(rsTBCliFor!Provincia) Then rsTBCliFor!Provincia = ""
                    If IsDBNull(rsTBCliFor!Nazione) Then rsTBCliFor!Nazione = ""
                    If rsTBCliFor!Nazione.ToString.Trim = "" Then rsTBCliFor!Nazione = "I" 'GIU140513
                    myNazione = rsTBCliFor!Nazione
                    If IsDBNull(rsTBCliFor!Telefono1) Then rsTBCliFor!Telefono1 = ""
                    If IsDBNull(rsTBCliFor!Telefono2) Then rsTBCliFor!Telefono2 = ""
                    If IsDBNull(rsTBCliFor!Fax) Then rsTBCliFor!Fax = ""
                    If IsDBNull(rsTBCliFor!Codice_Fiscale) Then rsTBCliFor!Codice_Fiscale = ""
                    If IsDBNull(rsTBCliFor!Partita_IVA) Then rsTBCliFor!Partita_IVA = ""
                    If IsDBNull(rsTBCliFor!Denominazione) Then rsTBCliFor!Denominazione = ""
                    If IsDBNull(rsTBCliFor!Titolare) Then rsTBCliFor!Titolare = ""
                    If IsDBNull(rsTBCliFor!Email) Then rsTBCliFor!Email = ""
                    If IsDBNull(rsTBCliFor!ABI_N) Then rsTBCliFor!ABI_N = ""
                    If IsDBNull(rsTBCliFor!CAB_N) Then rsTBCliFor!CAB_N = ""
                    If IsDBNull(rsTBCliFor!Provincia_Estera) Then rsTBCliFor!Provincia_Estera = ""
                    If IsDBNull(rsTBCliFor!IndirizzoSenzaNumero) Then rsTBCliFor!IndirizzoSenzaNumero = ""
                    If IsDBNull(rsTBCliFor!NumeroCivico) Then rsTBCliFor!NumeroCivico = ""
                    If IsDBNull(rsTBCliFor!Note) Then rsTBCliFor!Note = ""
                    If IsDBNull(rsTBCliFor!IVASosp) Then rsTBCliFor!IVASosp = 0 'giu070212
                    If IsDBNull(rsTBCliFor!IPA) Then rsTBCliFor!IPA = "" 'GIU120814
                    If IsDBNull(rsTBCliFor!SplitIVA) Then rsTBCliFor!SplitIVA = False 'GIU150215
                    'giu140620
                    If IsDBNull(rsTBCliFor!IBAN_Ditta) Then rsTBCliFor!IBAN_Ditta = ""
                    If IsDBNull(rsTBCliFor!EmailInvioScad) Then rsTBCliFor!EmailInvioScad = ""
                    If IsDBNull(rsTBCliFor!EmailInvioFatt) Then rsTBCliFor!EmailInvioFatt = ""
                    If IsDBNull(rsTBCliFor!PECEmail) Then rsTBCliFor!PECEmail = ""
                    rsTBCliFor.EndEdit()
                    rsTBCliFor.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Clienti/Fornitori. CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '-Destinazione Merce
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Filiale")) Then
            If DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Filiale") <> 0 Then
                If DsPrinWebDoc.DestClienti.Select("Progressivo = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Filiale").ToString.Trim).Length = 0 Then
                    strSQL = "Select * From DestClienti WHERE Progressivo = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Filiale") & ""
                    Try
                        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "DestClienti", _Esercizio) 'giu230419
                    Catch ex As Exception
                        Errore = ex.Message & " - Tab. DestClienti" & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                        Return False
                        Exit Function
                    End Try
                End If
            End If
        End If
        'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
        Dim rsTBDest As DataRow
        Try
            If (DsPrinWebDoc.Tables("DestClienti").Rows.Count > 0) Then
                For Each rsTBDest In DsPrinWebDoc.Tables("DestClienti").Select("")
                    rsTBDest.BeginEdit()
                    If IsDBNull(rsTBDest!Ragione_Sociale) Then rsTBDest!Ragione_Sociale = ""
                    If IsDBNull(rsTBDest!Indirizzo) Then rsTBDest!Indirizzo = ""
                    If IsDBNull(rsTBDest!Localita) Then rsTBDest!Localita = ""
                    If IsDBNull(rsTBDest!CAP) Then rsTBDest!CAP = ""
                    If IsDBNull(rsTBDest!Provincia) Then rsTBDest!Provincia = ""
                    If IsDBNull(rsTBDest!Stato) Then rsTBDest!Stato = ""
                    If IsDBNull(rsTBDest!Telefono1) Then rsTBDest!Telefono1 = ""
                    If IsDBNull(rsTBDest!Telefono2) Then rsTBDest!Telefono2 = ""
                    If IsDBNull(rsTBDest!Fax) Then rsTBDest!Fax = ""
                    If IsDBNull(rsTBDest!Denominazione) Then rsTBDest!Denominazione = ""
                    If IsDBNull(rsTBDest!Riferimento) Then rsTBDest!Riferimento = ""
                    If IsDBNull(rsTBDest!Email) Then rsTBDest!Email = ""
                    If IsDBNull(rsTBDest!Tipo) Then rsTBDest!Tipo = ""
                    rsTBDest.EndEdit()
                    rsTBDest.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Destinazione Merce CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '-
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("TipoContratto")) Then
            strSQL = "Select * From TipoContratto WHERE Codice = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("TipoContratto")
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, DsPrinWebDoc, "TipoContratto")
                Dim myCodVisita As String = ""
                myCodVisita = DsPrinWebDoc.TipoContratto.Rows(0).Item("CodVisita")
                If String.IsNullOrEmpty(myCodVisita) Then
                    myCodVisita = ""
                End If
                If myCodVisita.Trim <> "" Then
                    If (DsPrinWebDoc.Tables("ContrattiD").Rows.Count > 0) Then
                        For Each rsD In DsPrinWebDoc.Tables("ContrattiD").Select("Cod_Articolo<>'" & myCodVisita.Trim & "'")
                            rsD.BeginEdit()
                            rsD![TextDataSc] = "0"
                            rsD.EndEdit()
                            rsD.AcceptChanges()
                        Next
                    End If
                End If
            Catch ex As Exception
                Errore = ex.Message & " - Tab. TipoContratto" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            DsPrinWebDoc.TipoContratto.Clear()
        End If
        'giu240420 RATE FATTURAZIONE 
        Dim strScadPagCA As String = ""
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("ScadPagCA")) Then
            strScadPagCA = DsPrinWebDoc.ContrattiT.Rows(0).Item("ScadPagCA")
            If String.IsNullOrEmpty(strScadPagCA.Trim) Then
                strScadPagCA = ""
            End If
        End If
        Dim NRate As Integer = 0
        Dim TotRate As Decimal = 0
        Dim I As Integer = 0
        If strScadPagCA.Trim <> "" Then
            Try
                Dim lineaSplit As String() = strScadPagCA.Trim.Split(";")
                For I = 0 To lineaSplit.Count - 1
                    If lineaSplit(I).Trim <> "" And (I + 8) <= lineaSplit.Count - 1 Then 'giu191223 da i + 6
                        Dim newRow As DSPrintWeb_Documenti.ScadPagCARow = DsPrinWebDoc.ScadPagCA.NewScadPagCARow
                        newRow.BeginEdit()
                        newRow.IDDocumenti = myIDDoc.Trim
                        newRow.NRata = CInt(lineaSplit(I).Trim)
                        I += 1
                        newRow.DataSc = CDate(lineaSplit(I).Trim)
                        I += 1
                        newRow.Importo = CDec(lineaSplit(I).Trim)
                        TotRate += newRow.Importo
                        I += 1
                        newRow.Evasa = CBool(lineaSplit(I).Trim)
                        I += 1
                        newRow.NFC = lineaSplit(I).Trim
                        I += 1
                        newRow.DataFC = lineaSplit(I).Trim
                        I += 1
                        newRow.Serie = lineaSplit(I).Trim
                        I += 1
                        newRow.ImportoF = lineaSplit(I).Trim
                        I += 1
                        newRow.ImportoR = lineaSplit(I).Trim
                        newRow.TotNRate = 0
                        newRow.TotRate = 0
                        newRow.EndEdit()
                        DsPrinWebDoc.ScadPagCA.AddScadPagCARow(newRow)
                        NRate += 1
                    End If
                Next
                Dim rsScadPagCA As DSPrintWeb_Documenti.ScadPagCARow
                For Each rsScadPagCA In DsPrinWebDoc.Tables("ScadPagCA").Select("")
                    rsScadPagCA.BeginEdit()
                    rsScadPagCA.TotNRate = NRate
                    rsScadPagCA.TotRate = TotRate
                    rsScadPagCA.EndEdit()
                    rsScadPagCA.AcceptChanges()
                Next
                '-
                DsPrinWebDoc.ContrattiT.Rows(0).BeginEdit()
                DsPrinWebDoc.ContrattiT.Rows(0).Item("TotNRate") = NRate
                DsPrinWebDoc.ContrattiT.Rows(0).EndEdit()
                DsPrinWebDoc.ContrattiT.AcceptChanges()
            Catch ex As Exception
                Errore = ex.Message & " - Tab. ScadPagCA" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            DsPrinWebDoc.ScadPagCA.Clear()
        End If
        '-
        '''If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Valuta")) Then
        '''    strSQL = "Select * From Valute WHERE Codice = '" & DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Valuta") & "'"
        '''    Try
        '''        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Valute", _Esercizio) 'giu230419
        '''    Catch ex As Exception
        '''        Errore = ex.Message & " - Tab. Valute" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''        Return False
        '''        Exit Function
        '''    End Try
        '''Else
        '''    'strSQL = "Select * From Valute WHERE Codice = 'Z'"
        '''    DsPrinWebDoc.Valute.Clear()
        '''End If
        'GIU01122011 ATTENZIONE SE ESTRAE 2 RKS DUPLICA I DETTAGLI DEGLI ARTICOLO
        '''If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("IBAN")) Then
        '''    strSQL = "Select TOP 1 * From BancheIBAN WHERE (IBAN = '" & DsPrinWebDoc.ContrattiT.Rows(0).Item("IBAN") & "')"
        '''    Try
        '''        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "BancheIBAN", _Esercizio) 'giu230419
        '''    Catch ex As Exception
        '''        Errore = ex.Message & " - Tab. BancheIBAN" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''        Return False
        '''        Exit Function
        '''    End Try
        '''ElseIf Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("ABI")) And Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("CAB")) Then
        '''    If DsPrinWebDoc.ContrattiT.Rows(0).Item("ABI").ToString.Trim <> "" And DsPrinWebDoc.ContrattiT.Rows(0).Item("CAB").ToString.Trim <> "" Then
        '''        strSQL = "Select TOP 1 * From BancheIBAN WHERE (ABI = '" & DsPrinWebDoc.ContrattiT.Rows(0).Item("ABI") & "') And (CAB = '" & DsPrinWebDoc.ContrattiT.Rows(0).Item("CAB") & "')"
        '''    Else
        '''        strSQL = "Select TOP 1 * From BancheIBAN WHERE (ABI = 'ZZZZZ')"
        '''    End If
        '''    Try
        '''        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "BancheIBAN", _Esercizio) 'giu230419
        '''    Catch ex As Exception
        '''        Errore = ex.Message & " - Tab. BancheIBAN" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''        Return False
        '''        Exit Function
        '''    End Try
        '''Else
        '''    'strSQL = "Select * From BancheIBAN WHERE (ABI = 'ZZZZZ')"
        '''    DsPrinWebDoc.BancheIBAN.Clear()
        '''End If
        '---
        '''If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Vettore_1")) Then
        '''    strSQL = "Select * From Vettori WHERE (Codice = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Vettore_1") & ") "
        '''    Try
        '''        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettori_1", _Esercizio) 'giu230419
        '''    Catch ex As Exception
        '''        Errore = ex.Message & " - Tab. Vettore1" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''        Return False
        '''        Exit Function
        '''    End Try
        '''Else
        '''    'strSQL = "Select * From Vettori WHERE (Codice = -1)"
        '''    DsPrinWebDoc.Vettori_1.Clear()
        '''End If
        ''''giu030112 per i campi NULL in stampa
        '''Dim rsTBV1 As DataRow
        '''Try
        '''    If (DsPrinWebDoc.Tables("Vettori_1").Rows.Count > 0) Then
        '''        For Each rsTBV1 In DsPrinWebDoc.Tables("Vettori_1").Select("")
        '''            rsTBV1.BeginEdit()
        '''            If IsDBNull(rsTBV1!Descrizione) Then rsTBV1!Descrizione = ""
        '''            If IsDBNull(rsTBV1!Residenza) Then rsTBV1!Residenza = ""
        '''            If IsDBNull(rsTBV1!Localita) Then rsTBV1!Localita = ""
        '''            If IsDBNull(rsTBV1!Provincia) Then rsTBV1!Provincia = ""
        '''            If IsDBNull(rsTBV1!Partita_IVA) Then rsTBV1!Partita_IVA = ""
        '''            If IsDBNull(rsTBV1!Codice_CoGe) Then rsTBV1!Codice_CoGe = ""
        '''            rsTBV1.EndEdit()
        '''            rsTBV1.AcceptChanges()
        '''        Next
        '''    End If
        '''Catch ex As Exception
        '''    Errore = ex.Message & " - Tab. Vettori_1 CAMPI NULL" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''    Return False
        '''    Exit Function
        '''End Try
        '------------------------------------------------------------------------
        '-
        '''If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Vettore_2")) Then
        '''    strSQL = "Select * From Vettori WHERE (Codice = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Vettore_2") & ") "
        '''    Try
        '''        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettore_2", _Esercizio) 'giu230419
        '''    Catch ex As Exception
        '''        Errore = ex.Message & " - Tab. Vettore2" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''        Return False
        '''        Exit Function
        '''    End Try
        '''Else
        '''    'strSQL = "Select * From Vettori WHERE (Codice = -1)"
        '''    DsPrinWebDoc.Vettori_2.Clear()
        '''End If
        ''''giu030112 per i campi NULL in stampa
        '''Dim rsTBV2 As DataRow
        '''Try
        '''    If (DsPrinWebDoc.Tables("Vettori_2").Rows.Count > 0) Then
        '''        For Each rsTBV2 In DsPrinWebDoc.Tables("Vettori_2").Select("")
        '''            rsTBV2.BeginEdit()
        '''            If IsDBNull(rsTBV2!Descrizione) Then rsTBV2!Descrizione = ""
        '''            If IsDBNull(rsTBV2!Residenza) Then rsTBV2!Residenza = ""
        '''            If IsDBNull(rsTBV2!Localita) Then rsTBV2!Localita = ""
        '''            If IsDBNull(rsTBV2!Provincia) Then rsTBV2!Provincia = ""
        '''            If IsDBNull(rsTBV2!Partita_IVA) Then rsTBV2!Partita_IVA = ""
        '''            If IsDBNull(rsTBV2!Codice_CoGe) Then rsTBV2!Codice_CoGe = ""
        '''            rsTBV2.EndEdit()
        '''            rsTBV2.AcceptChanges()
        '''        Next
        '''    End If
        '''Catch ex As Exception
        '''    Errore = ex.Message & " - Tab. Vettori_2 CAMPI NULL" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''    Return False
        '''    Exit Function
        '''End Try
        '------------------------------------------------------------------------
        '-
        '''If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Vettore_3")) Then
        '''    strSQL = "Select * From Vettori WHERE (Codice = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Vettore_3") & ") "
        '''    Try
        '''        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettore_3", _Esercizio) 'giu230419
        '''    Catch ex As Exception
        '''        Errore = ex.Message & " - Tab. Vettore3" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''        Return False
        '''        Exit Function
        '''    End Try
        '''Else
        '''    'strSQL = "Select * From Vettori WHERE (Codice = -1)"
        '''    DsPrinWebDoc.Vettori_3.Clear()
        '''End If
        ''''giu030112 per i campi NULL in stampa
        '''Dim rsTBV3 As DataRow
        '''Try
        '''    If (DsPrinWebDoc.Tables("Vettori_3").Rows.Count > 0) Then
        '''        For Each rsTBV3 In DsPrinWebDoc.Tables("Vettori_3").Select("")
        '''            rsTBV3.BeginEdit()
        '''            If IsDBNull(rsTBV3!Descrizione) Then rsTBV3!Descrizione = ""
        '''            If IsDBNull(rsTBV3!Residenza) Then rsTBV3!Residenza = ""
        '''            If IsDBNull(rsTBV3!Localita) Then rsTBV3!Localita = ""
        '''            If IsDBNull(rsTBV3!Provincia) Then rsTBV3!Provincia = ""
        '''            If IsDBNull(rsTBV3!Partita_IVA) Then rsTBV3!Partita_IVA = ""
        '''            If IsDBNull(rsTBV3!Codice_CoGe) Then rsTBV3!Codice_CoGe = ""
        '''            rsTBV3.EndEdit()
        '''            rsTBV3.AcceptChanges()
        '''        Next
        '''    End If
        '''Catch ex As Exception
        '''    Errore = ex.Message & " - Tab. Vettori_3 CAMPI NULL" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''    Return False
        '''    Exit Function
        '''End Try
        '------------------------------------------------------------------------
        'giu290420
        '-
        '''If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("RespArea")) Then
        '''    strSQL = "Select * From RespArea WHERE (Codice = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("RespArea") & ") "
        '''    Try
        '''        ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, DsPrinWebDoc, "RespArea") 'giu230419
        '''    Catch ex As Exception
        '''        Errore = ex.Message & " - Tab. RespArea" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''        Return False
        '''        Exit Function
        '''    End Try
        '''Else
        '''    'strSQL = "Select * From Vettori WHERE (Codice = -1)"
        '''    DsPrinWebDoc.RespArea.Clear()
        '''End If
        ''''per i campi NULL in stampa
        '''Dim rsTBRA As DataRow
        '''Try
        '''    If (DsPrinWebDoc.Tables("RespArea").Rows.Count > 0) Then
        '''        For Each rsTBRA In DsPrinWebDoc.Tables("RespArea").Select("")
        '''            rsTBRA.BeginEdit()
        '''            If IsDBNull(rsTBRA!Descrizione) Then rsTBRA!Descrizione = ""
        '''            rsTBRA!Descrizione = rsTBRA!Descrizione.ToString.Trim.ToUpper
        '''            If IsDBNull(rsTBRA!Residenza) Then rsTBRA!Residenza = ""
        '''            If IsDBNull(rsTBRA!Localita) Then rsTBRA!Localita = ""
        '''            If IsDBNull(rsTBRA!Provincia) Then rsTBRA!Provincia = ""
        '''            If IsDBNull(rsTBRA!Partita_IVA) Then rsTBRA!Partita_IVA = ""
        '''            If IsDBNull(rsTBRA!EMail) Then rsTBRA!EMail = ""
        '''            If IsDBNull(rsTBRA!Codice_CoGe) Then rsTBRA!Codice_CoGe = ""
        '''            rsTBRA.EndEdit()
        '''            rsTBRA.AcceptChanges()
        '''        Next
        '''    End If
        '''Catch ex As Exception
        '''    Errore = ex.Message & " - Tab. RespArea CAMPI NULL" & " <br> " & _
        '''            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
        '''    Return False
        '''    Exit Function
        '''End Try
        '-
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("RespVisite")) Then
            'GIU141123 CI SONO ANCHE I RESPVISITE APP (DETTAGLI)
            If DsPrinWebDoc.RespVisite.Select("Codice = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("RespVisite").ToString.Trim).Length = 0 Then
                strSQL = "Select * From RespVisite WHERE (Codice = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("RespVisite") & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, DsPrinWebDoc, "RespVisite") 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. RespVisite" & " <br> " &
                        "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        Else
            DsPrinWebDoc.RespVisite.Clear()
        End If
        'per i campi NULL in stampa
        Dim rsTBRV As DataRow
        Try
            If (DsPrinWebDoc.Tables("RespVisite").Rows.Count > 0) Then
                For Each rsTBRV In DsPrinWebDoc.Tables("RespVisite").Select("")
                    rsTBRV.BeginEdit()
                    If IsDBNull(rsTBRV!Descrizione) Then rsTBRV!Descrizione = ""
                    rsTBRV!Descrizione = rsTBRV!Descrizione.ToString.Trim.ToUpper
                    If IsDBNull(rsTBRV!Residenza) Then rsTBRV!Residenza = ""
                    If IsDBNull(rsTBRV!Localita) Then rsTBRV!Localita = ""
                    If IsDBNull(rsTBRV!Provincia) Then rsTBRV!Provincia = ""
                    If IsDBNull(rsTBRV!Partita_IVA) Then rsTBRV!Partita_IVA = ""
                    If IsDBNull(rsTBRV!EMail) Then rsTBRV!EMail = ""
                    If IsDBNull(rsTBRV!Codice_CoGe) Then rsTBRV!Codice_CoGe = ""
                    rsTBRV.EndEdit()
                    rsTBRV.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. RespVisite CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        'Aliquote IVA per il BLOCCO IVA
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Iva1")) Then
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Iva1") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA1", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Aliquota_IVA1" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = -1)"
            DsPrinWebDoc.Aliquote_IVA1.Clear()
        End If

        '-
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Iva2")) Then
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Iva2") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA2", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Aliquota_IVA2" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = -1)"
            DsPrinWebDoc.Aliquote_IVA2.Clear()
        End If

        '-
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Iva3")) Then
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Iva3") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA3", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Aliquota_IVA3" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = -1)"
            DsPrinWebDoc.Aliquote_IVA3.Clear()
        End If

        '-
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Iva4")) Then
            strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Iva4") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA4", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Aliquota_IVA4" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = -1)"
            DsPrinWebDoc.Aliquote_IVA4.Clear()
        End If

        '-CausMag
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Causale")) Then
            strSQL = "Select Codice, Descrizione From CausMag WHERE (Codice = " & DsPrinWebDoc.ContrattiT.Rows(0).Item("Cod_Causale") & ") "
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "CausMag", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. CausMag" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select Codice, Descrizione From CausMag WHERE (Codice = -1)"
            DsPrinWebDoc.CausMag.Clear()
        End If
        '-NoteIntervento giu070322
        Dim StrDato() As String
        Dim myPos As Integer = 0
        Dim mySL As String = "" : Dim myNoteRitiro As String = ""
        Dim pNoteRitiro As String = ""
        If Not IsDBNull(DsPrinWebDoc.ContrattiT.Rows(0).Item("NoteRitiro")) Then
            DsPrinWebDoc.NoteIntervento.Clear()
            Dim ListaSL As New List(Of String)
            pNoteRitiro = DsPrinWebDoc.ContrattiT.Rows(0).Item("NoteRitiro").ToString.Trim
            myPos = InStr(pNoteRitiro, "§")
            If myPos > 0 Then
                StrDato = pNoteRitiro.Trim.Split("§")
                For I = 0 To StrDato.Count - 1
                    mySL = Formatta.FormattaNomeFile(StrDato(I)) 'giu070523
                    If I > StrDato.Count - 1 Then
                        myNoteRitiro = ""
                    Else
                        I += 1
                        myNoteRitiro = StrDato(I)
                    End If
                    ListaSL.Add(mySL + "§" + myNoteRitiro.Trim)
                Next
            Else 'c'è una descrizione ma non assegnata a nessuna quindi appartiene a tutti i N° di serie
                Call SetNoteSLALLApp(IdDocumento, pNoteRitiro, ListaSL)
            End If
            '-----
            For I = 0 To ListaSL.Count - 1
                StrDato = ListaSL(I).Split("§")
                mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
                myNoteRitiro = StrDato(1)
                '-
                If DsPrinWebDoc.NoteIntervento.Select("SerieLotto='" + mySL + "'").Length > 0 Then 'giu070523
                    Dim EditRow As DSPrintWeb_Documenti.NoteInterventoRow
                    For Each EditRow In DsPrinWebDoc.NoteIntervento.Select("SerieLotto='" + mySL + "'")
                        With EditRow
                            .BeginEdit()
                            .SerieLotto = mySL
                            .AllaPresenzaDi = GetAllaPresenzaDi(myNoteRitiro)
                            .NoteIntervento = Trim(myNoteRitiro + " " + GetScadConsumabili(mySL, ListaScadConsumabili, myNoteRitiro))
                            .EndEdit()
                        End With
                    Next
                Else
                    Dim newRow As DSPrintWeb_Documenti.NoteInterventoRow = DsPrinWebDoc.NoteIntervento.NewNoteInterventoRow
                    With newRow
                        .BeginEdit()
                        .SerieLotto = mySL
                        .AllaPresenzaDi = GetAllaPresenzaDi(myNoteRitiro)
                        .NoteIntervento = Trim(myNoteRitiro + " " + GetScadConsumabili(mySL, ListaScadConsumabili, myNoteRitiro))
                        .EndEdit()
                    End With
                    Try
                        DsPrinWebDoc.NoteIntervento.AddNoteInterventoRow(newRow)
                        newRow = Nothing
                    Catch Ex As Exception
                        Errore = Ex.Message & " - Tab. NoteIntevento inserimento (I)" & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                        Return False
                        Exit Function
                    End Try
                End If
            Next
            DsPrinWebDoc.NoteIntervento.AcceptChanges()
        Else
            DsPrinWebDoc.NoteIntervento.Clear()
            Dim ListaSL As New List(Of String)
            pNoteRitiro = ""
            Call SetNoteSLALLApp(IdDocumento, pNoteRitiro, ListaSL)
            '-----
            For I = 0 To ListaSL.Count - 1
                StrDato = ListaSL(I).Split("§")
                mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
                myNoteRitiro = StrDato(1)
                '-
                If DsPrinWebDoc.NoteIntervento.Select("SerieLotto='" + mySL + "'").Length > 0 Then
                    Dim EditRow As DSPrintWeb_Documenti.NoteInterventoRow
                    For Each EditRow In DsPrinWebDoc.NoteIntervento.Select("SerieLotto='" + mySL + "'")
                        With EditRow
                            .BeginEdit()
                            .SerieLotto = mySL
                            .AllaPresenzaDi = GetAllaPresenzaDi(myNoteRitiro)
                            .NoteIntervento = Trim(myNoteRitiro + " " + GetScadConsumabili(mySL, ListaScadConsumabili, myNoteRitiro))
                            .EndEdit()
                        End With
                    Next
                Else
                    Dim newRow As DSPrintWeb_Documenti.NoteInterventoRow = DsPrinWebDoc.NoteIntervento.NewNoteInterventoRow
                    With newRow
                        .BeginEdit()
                        .SerieLotto = mySL
                        .AllaPresenzaDi = GetAllaPresenzaDi(myNoteRitiro)
                        .NoteIntervento = Trim(myNoteRitiro + " " + GetScadConsumabili(mySL, ListaScadConsumabili, myNoteRitiro))
                        .EndEdit()
                    End With
                    Try
                        DsPrinWebDoc.NoteIntervento.AddNoteInterventoRow(newRow)
                        newRow = Nothing
                    Catch Ex As Exception
                        Errore = Ex.Message & " - Tab. NoteIntevento inserimento (I)" & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                        Return False
                        Exit Function
                    End Try
                End If
            Next
            DsPrinWebDoc.NoteIntervento.AcceptChanges()
            '-----------------------------------------------------
        End If
        'giu070423 cotrollo Note intervento che ci siano tutti altrimenti stampa vuoto il verbale
        Dim ListaSLCK As New List(Of String)
        pNoteRitiro = ""
        Call SetNoteSLALLApp(IdDocumento, pNoteRitiro, ListaSLCK)
        '-----
        For I = 0 To ListaSLCK.Count - 1
            StrDato = ListaSLCK(I).Split("§")
            mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
            myNoteRitiro = StrDato(1)
            '-
            If DsPrinWebDoc.NoteIntervento.Select("SerieLotto='" + mySL.Trim + "'").Length = 0 Then
                Dim newRow As DSPrintWeb_Documenti.NoteInterventoRow = DsPrinWebDoc.NoteIntervento.NewNoteInterventoRow
                With newRow
                    .BeginEdit()
                    .SerieLotto = mySL
                    .AllaPresenzaDi = GetAllaPresenzaDi(myNoteRitiro)
                    .NoteIntervento = Trim(myNoteRitiro + " " + GetScadConsumabili(mySL, ListaScadConsumabili, myNoteRitiro))
                    .EndEdit()
                End With
                Try
                    DsPrinWebDoc.NoteIntervento.AddNoteInterventoRow(newRow)
                    newRow = Nothing
                Catch Ex As Exception
                    Errore = Ex.Message & " - Tab. NoteIntevento inserimento (II)" & " <br> " &
                        "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        Next
        DsPrinWebDoc.NoteIntervento.AcceptChanges()
        '----------------------------------------------------------------------------------------
        'giu130412
        If Not IsDBNull(DsPrinWebDoc.Clienti.Rows(0).Item("Nazione")) Then
            strSQL = "Select * From Nazioni WHERE Codice = '" & DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim & "'"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Nazioni", _Esercizio) 'giu230419
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Nazioni" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
            'GIU150513 OBB. PER LA STAMPA altrimenti non mi stampa la LOCALITA'
            If DsPrinWebDoc.Nazioni.Select("").Count = 0 Then
                Dim newRow As DSPrintWeb_Documenti.NazioniRow = DsPrinWebDoc.Nazioni.NewNazioniRow
                With newRow
                    .BeginEdit()
                    .Codice = DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim
                    .Descrizione = "!!SCONOSCIUTA!!"
                    .Codice_ISO = DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim
                    .EndEdit()
                End With
                Try
                    DsPrinWebDoc.Nazioni.AddNazioniRow(newRow)
                    newRow = Nothing
                Catch Ex As Exception
                    Errore = Ex.Message & " - Tab. Nazioni inserimento (" & DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim & ") <br> " &
                        "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                DsPrinWebDoc.Nazioni.AcceptChanges()
            End If
        Else
            'strSQL = "Select * From Nazioni WHERE Codice = ''"
            DsPrinWebDoc.Nazioni.Clear()
            'GIU150513 OBB. PER LA STAMPA altrimenti non mi stampa la LOCALITA'
            Dim newRow As DSPrintWeb_Documenti.NazioniRow = DsPrinWebDoc.Nazioni.NewNazioniRow
            With newRow
                .BeginEdit()
                .Codice = "I"
                .Descrizione = "ITALIA"
                .Codice_ISO = "I"
                .EndEdit()
            End With
            Try
                DsPrinWebDoc.Nazioni.AddNazioniRow(newRow)
                newRow = Nothing
            Catch Ex As Exception
                Errore = Ex.Message & " - Tab. Nazioni inserimento (I)" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Contratto: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
            DsPrinWebDoc.Nazioni.AcceptChanges()
        End If
        '==FINE CARICAMENTO ====================================
        ObjDB = Nothing

        Return True

    End Function
    Private Function SetNoteSLALLApp(ByVal pIdDocumento As Long, ByVal pNoteRitiro As String, ByRef ListaSL As List(Of String)) As Boolean
        SetNoteSLALLApp = False
        If pIdDocumento < 0 Then
            Exit Function
        End If
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim strSQL As String = ""
        strSQL = "Select ISNULL(Serie,'') + ISNULL(Lotto,'') AS SerieLotto From ContrattiD " & _
                 "WHERE (IDDocumenti = " + pIdDocumento.ToString.Trim + ") AND (DurataNum =0) " & _
                 "GROUP BY ISNULL(Serie,'') + ISNULL(Lotto,'')"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim mySL As String = ""
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each row In ds.Tables(0).Select()
                        mySL = Formatta.FormattaNomeFile(IIf(IsDBNull(row.Item("SerieLotto")), "", row.Item("SerieLotto"))) 'giu070523
                        If mySL.Trim <> "" Then
                            ListaSL.Add(mySL + "§" + pNoteRitiro.Trim)
                        End If
                    Next
                Else
                    'per adesso proseguo
                    Exit Function
                End If
            Else
                'per adesso proseguo
                Exit Function
            End If
        Catch Ex As Exception
            'per adesso proseguo
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore: ", "Assegna Note Intervento(SetNoteSLALLApp): " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        SetNoteSLALLApp = True
    End Function
    'giu290123
    Private Function GetAllaPresenzaDi(ByRef pNoteRitiro As String) As String
        GetAllaPresenzaDi = ""
        Dim StrDato() As String
        Dim myPos As Integer = InStr(pNoteRitiro, "|")
        Try
            If myPos > 0 Then
                StrDato = pNoteRitiro.Trim.Split("|")
                pNoteRitiro = StrDato(0)
                GetAllaPresenzaDi = StrDato(1)
            End If
            pNoteRitiro += " "
            For I = 2 To StrDato.Count - 1
                pNoteRitiro += StrDato(I)
            Next
        Catch ex As Exception
            GetAllaPresenzaDi = ""
        End Try
        pNoteRitiro = Trim(pNoteRitiro)
        GetAllaPresenzaDi = Trim(GetAllaPresenzaDi)
    End Function
    'giu220223 Scadenze Consumabili per N° Serie Lotto
    Private Function GetScadConsumabili(ByVal pSerieLotto As String, ByVal ListaScadConsumabili As List(Of String), ByVal pNoteIntervento As String) As String
        GetScadConsumabili = ""
        Dim StrDato() As String
        Dim myPos As Integer = 0
        Dim mySL As String = ""
        For I = 0 To ListaScadConsumabili.Count - 1
            StrDato = ListaScadConsumabili(I).Split("§")
            mySL = Formatta.FormattaNomeFile(StrDato(0)) 'giu070523
            If mySL = pSerieLotto Then
                'giu291123
                If pNoteIntervento.ToUpper.Contains(Mid(StrDato(1), 4).Trim.ToUpper) Then
                    'nulla
                Else
                    GetScadConsumabili += StrDato(1)
                End If
            End If
        Next
    End Function
    'giu020512 stampa tutti i documenti fatturati o da numero a numero fill in APPEND
    'giu220722 Carica solo le testate DDT per la generazione file CSV 
    Public Function StampaDocumentiDalAl(ByVal IdDocumento As Long, ByVal TipoDoc As String, ByVal CodiceDitta As String,
                                         ByVal _Esercizio As String, ByVal TabCliFor As String, ByRef DsPrinWebDoc As DSPrintWeb_Documenti,
                                         ByRef ObjReport As Object, ByRef SWSconti As Boolean, ByRef SWRitAcc As Boolean, ByRef Errore As String,
                                         Optional ByVal SWSpedDDT As Boolean = False) As Boolean
        'GIU31082023 stampa lotti in documento senza il SUBReport
        Dim strErrore As String = "" : Dim strValore As String = ""
        Dim SWStampaDocLotti As Boolean = False
        If App.GetDatiAbilitazioni(CSTABILAZI, "SWSTDOCLT", strValore, strErrore) = True Then
            SWStampaDocLotti = True
        Else
            SWStampaDocLotti = False
        End If
        '---------
        'giu270612
        myIDDoc = IdDocumento.ToString.Trim
        myTipoDoc = TipoDoc.Trim
        myDataDoc = ""
        myNDoc = ""
        '---------
        If TipoDoc.Trim = "" Then
            Errore = "Tipo Documento sconosciuto. Passo: 0 <br>" &
           "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        ElseIf Left(TipoDoc, 1) = "O" Or Left(TipoDoc, 1) = "P" Then
            _Esercizio = "" 'GIU230419 SOLO PER GLI ORDINI E PREVENTIVI LEGGERE SEMPRE L'ANNO CORRENTE ALTRIMENTI LE MODIFICHE NON SONO LE ULTIME
        End If
        '---------
        'GIU01122011 ATTENZIONE SE ESTRAE 2 RKS DUPLICA I DETTAGLI DEGLI ARTICOLI
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim Passo As Integer = 0
        Dim SqlConnDoc As SqlConnection
        Dim SqlAdapDoc As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Try
            SqlConnDoc = New SqlConnection
            SqlAdapDoc = New SqlDataAdapter
            SqlDbSelectCmd = New SqlCommand

            SqlAdapDoc.SelectCommand = SqlDbSelectCmd
            SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, _Esercizio) 'giu230419 giu170420 errore su connessone db (dai contratti)
            SqlDbSelectCmd.CommandText = "get_DocTByIDDocumenti"
            SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectCmd.Connection = SqlConnDoc
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            Dim SqlAdapDocDett As New SqlDataAdapter
            Dim SqlDbSelectDettCmd As New SqlCommand
            SqlDbSelectDettCmd.CommandText = "get_DocDByIDDocumenti"
            SqlDbSelectDettCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettCmd.Connection = SqlConnDoc
            SqlDbSelectDettCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocDett.SelectCommand = SqlDbSelectDettCmd

            'GIU301111 LOTTI
            Dim SqlAdapDocDettL As New SqlDataAdapter
            Dim SqlDbSelectDettCmdL As New SqlCommand
            SqlDbSelectDettCmdL.CommandText = "get_DocDLByIDDocRiga"
            SqlDbSelectDettCmdL.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettCmdL.Connection = SqlConnDoc
            SqlDbSelectDettCmdL.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDettCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocDettL.SelectCommand = SqlDbSelectDettCmdL

            '==============CARICAMENTO DATASET ===================
            Passo = 1
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlAdapDoc.Fill(DsPrinWebDoc.DocumentiT)
            If SWSpedDDT = False Then
                Passo = 2
                SqlDbSelectDettCmd.Parameters.Item("@IDDocumenti").Value = IdDocumento
                SqlAdapDocDett.Fill(DsPrinWebDoc.DocumentiD)
                Passo = 3
                SqlDbSelectDettCmdL.Parameters.Item("@IDDocumenti").Value = IdDocumento
                SqlDbSelectDettCmdL.Parameters.Item("@Riga").Value = DBNull.Value
                SqlAdapDocDettL.Fill(DsPrinWebDoc.DocumentiDLotti)
            End If

        Catch ex As Exception
            Errore = ex.Message & " - Documento lettura testata e dettagli. Passo: " & Passo.ToString & " <br> " &
            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '---------------------------------------------------------------------------------------------
        'giu021211 RIATTIVATA ROUTINE DI CALCOLO IMPORTO E DOCUMENTO PER IL PROBLEMA DEI DATI SALVATI
        'DA UNA SESSIONE PRECEDENTE (BACK SULLA PAGINA WEB) (CONTROLLO DETTAGLIO CON TESTATA DOCUMENTO
        '---------------------------------------------------------------------------------------------
        Passo = 4
        strErrore = ""
        'Listino documento
        Dim Listino As String = "1"
        'Valuta per i decimali per il calcolo 
        Dim DecimaliVal As String = "2" ' Session(CSTDECIMALIVALUTADOC)
        Dim CodValuta As String = "Euro"
        '---
        Listino = "1"
        CodValuta = "Euro"
        DecimaliVal = "2" 'Euro
        SpeseIncasso = "0"
        SpeseTrasporto = "0"
        SpeseImballo = "0"
        SpeseVarie = "0"
        RegimeIVA = "0"
        ScCassa = "0"
        Abbuono = "0"
        ImponibileMemo = 0
        ImpostaMemo = 0
        TotaleMemo = 0
        TotaleMMemo = 0
        '-
        ImponibileCheck = 0
        ImpostaCheck = 0
        TotaleCheck = 0
        TotaleMCheck = 0

        mySplitIVA = False
        myRitAcconto = False
        myTotaleRACK = 0
        myTotaleRAMM = 0
        myTotNettoPagareCK = 0
        myTotNettoPagareMM = 0
        '---
        'giu021117 Richiesta Zibordi del 27/10/2017 (Solo per PR/OC riportare le Iniziali=Codice Operatore)
        Dim strSQL As String = ""
        Dim X As Integer = 0
        Dim ObjDB As New DataBaseUtility
        '--------------------------------------------------------------------------------------------------
        Dim SWTabCliFor As String = ""
        'giu020512
        Dim myCodiceCoGe As String = ""
        Dim myCodFiliale As String = ""
        Dim myNazione As String = ""
        Dim rsTestata As DataRow
        '' ''giu220319
        ' ''Dim RighePerPagina As Integer = 0
        ' ''Dim RighePagina As Integer = 0
        ' ''Select Case Left(TipoDoc, 1)
        ' ''    Case "DT"
        ' ''        RighePerPagina = App.GetParamGestAzi(_Esercizio).RighePerPaginaDDT
        ' ''        RighePerPagina -= 5
        ' ''    Case Else
        ' ''        RighePerPagina = App.GetParamGestAzi(_Esercizio).RighePerPaginaDDT
        ' ''        RighePerPagina -= 5
        ' ''End Select
        '---------
        Try
            If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
                'GIU020512 STAMPALL
                For Each rsTestata In DsPrinWebDoc.Tables("DocumentiT").Select("IDDocumenti=" & IdDocumento.ToString.Trim)
                    'giu270712
                    myDataDoc = IIf(IsDBNull(rsTestata!Data_Doc), "", rsTestata!Data_Doc)
                    myNDoc = IIf(IsDBNull(rsTestata!Numero), "", rsTestata!Numero)
                    myTipoDoc = IIf(IsDBNull(rsTestata!Tipo_Doc), "", rsTestata!Tipo_Doc)
                    '---------
                    'GIU020512()
                    If Not IsDBNull(rsTestata![Cod_Cliente]) Then
                        TabCliFor = "Cli"
                        myCodiceCoGe = rsTestata![Cod_Cliente].ToString.Trim
                    ElseIf Not IsDBNull(rsTestata![Cod_Fornitore]) Then
                        TabCliFor = "For"
                        myCodiceCoGe = rsTestata![Cod_Fornitore].ToString.Trim
                        'Dim strCodice As String = DsPrinWebDoc.DocumentiT.Rows(0).Item("IDAnagrProvv").ToString.Trim
                    ElseIf Not IsDBNull(rsTestata![IDAnagrProvv]) Then
                        TabCliFor = "Provv"
                        myCodiceCoGe = rsTestata![IDAnagrProvv].ToString.Trim
                    Else
                        TabCliFor = "Cli"
                        myCodiceCoGe = ""
                    End If
                    '--
                    If Not IsDBNull(rsTestata![Cod_Filiale]) Then
                        myCodFiliale = rsTestata![Cod_Filiale].ToString.Trim
                    End If
                    '---------
                    Listino = rsTestata![Listino]
                    CodValuta = rsTestata![Cod_Valuta]
                    SpeseIncasso = rsTestata![Spese_Incasso]
                    SpeseTrasporto = rsTestata![Spese_Trasporto]
                    SpeseImballo = rsTestata![Spese_Imballo]
                    SpeseVarie = rsTestata![SpeseVarie]
                    RegimeIVA = rsTestata![Cod_Iva]
                    ScCassa = rsTestata![Sconto_Cassa]
                    Abbuono = rsTestata![Abbuono]
                    '-
                    ImponibileMemo += rsTestata![Imponibile1] + rsTestata![Imponibile2] + rsTestata![Imponibile3] + rsTestata![Imponibile4]
                    ImpostaMemo += rsTestata![Imposta1] + rsTestata![Imposta2] + rsTestata![Imposta3] + rsTestata![Imposta4]
                    TotaleMemo += rsTestata![Totale]
                    TotaleMMemo += rsTestata![TotaleM]
                    'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
                    rsTestata.BeginEdit()
                    If IsDBNull(rsTestata!Destinazione1) Then rsTestata!Destinazione1 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione2 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione3 = ""
                    If IsDBNull(rsTestata!ABI) Then rsTestata!ABI = ""
                    If IsDBNull(rsTestata!CAB) Then rsTestata!CAB = ""
                    If IsDBNull(rsTestata!IBAN) Then rsTestata!IBAN = ""
                    If IsDBNull(rsTestata!ContoCorrente) Then rsTestata!ContoCorrente = ""
                    If IsDBNull(rsTestata!CIG) Then rsTestata!CIG = ""
                    If IsDBNull(rsTestata!CUP) Then rsTestata!CUP = ""
                    If IsDBNull(rsTestata!FatturaPA) Then rsTestata!FatturaPA = False 'GIU120814
                    'GIU150118
                    If IsDBNull(rsTestata!SplitIVA) Then rsTestata!SplitIVA = False
                    mySplitIVA = rsTestata!SplitIVA
                    If IsDBNull(rsTestata!RitAcconto) Then rsTestata!RitAcconto = False
                    myRitAcconto = rsTestata!RitAcconto
                    If IsDBNull(rsTestata!ImponibileRA) Then rsTestata!ImponibileRA = 0
                    If IsDBNull(rsTestata!PercRA) Then rsTestata!PercRA = 0
                    If IsDBNull(rsTestata!TotaleRA) Then rsTestata!TotaleRA = 0
                    myTotaleRAMM = rsTestata!TotaleRA
                    SWRitAcc = rsTestata!RitAcconto
                    If rsTestata!RitAcconto = False Then
                        If rsTestata!ImponibileRA <> 0 Or rsTestata!PercRA <> 0 Or rsTestata!TotaleRA <> 0 Then
                            Errore = "Errore calcolo Ritenuta d'acconto. (StampaDocumentiDaAl). Passo: " & Passo.ToString & " <br> " &
                            "Per risolvere il problema controllare la sezione Totali modificando e aggiornando i dati Rit.Acconto." & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End If
                    Else
                        If IsDBNull(rsTestata!ImponibileRA) Then rsTestata!ImponibileRA = 0
                        If IsDBNull(rsTestata!PercRA) Then rsTestata!PercRA = 0
                        If IsDBNull(rsTestata!TotaleRA) Then rsTestata!TotaleRA = 0
                        If rsTestata!ImponibileRA <> 0 And rsTestata!PercRA <> 0 Then
                            myTotaleRACK = rsTestata!ImponibileRA * rsTestata!PercRA / 100
                            myTotaleRACK = FormatCurrency(myTotaleRACK, 2)
                        Else
                            myTotaleRACK = 0
                        End If
                        If rsTestata!ImponibileRA = 0 Or rsTestata!PercRA = 0 Or rsTestata!TotaleRA = 0 Or
                            myTotaleRACK <> myTotaleRAMM Then
                            Errore = "Errore calcolo Ritenuta d'acconto. (StampaDocumentiDaAl). Passo: " & Passo.ToString & " <br> " &
                            "Per risolvere il problema controllare la sezione Totali modificando e aggiornando i dati Rit.Acconto." & " <br> " &
                            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                            Return False
                            Exit Function
                        End If
                    End If
                    '---------
                    If IsDBNull(rsTestata!TotNettoPagare) Then rsTestata!TotNettoPagare = 0
                    myTotNettoPagareMM = rsTestata!TotNettoPagare
                    '---------
                    'giu021117
                    strSQL = ""
                    If Not IsDBNull(rsTestata!ModificatoDa) Then
                        strSQL = rsTestata!ModificatoDa.ToString.Trim
                        X = InStr(strSQL, " ")
                        If X > 0 Then
                            strSQL = Mid(strSQL, 1, X - 1)
                        End If
                    ElseIf Not IsDBNull(rsTestata!InseritoDa) Then
                        strSQL = rsTestata!InseritoDa.ToString.Trim
                        X = InStr(strSQL, " ")
                        If X > 0 Then
                            strSQL = Mid(strSQL, 1, X - 1)
                        End If
                    End If
                    If strSQL <> "" Then
                        strSQL = "Select Codice, Nome From Operatori WHERE (Nome LIKE N'%" & strSQL & "%') "
                        Try
                            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, DsPrinWebDoc, "Operatori")
                            If (DsPrinWebDoc.Tables("Operatori").Rows.Count > 0) Then
                                If Not IsDBNull(DsPrinWebDoc.Operatori.Rows(0).Item("Codice")) Then
                                    strSQL = DsPrinWebDoc.Operatori.Rows(0).Item("Codice").ToString.Trim
                                End If
                            End If
                        Catch ex As Exception
                            Errore = ex.Message & " - Inserimento dati nella Tabella Operatori. Passo: " & Passo.ToString
                            Return False
                            Exit Function
                        End Try
                    End If
                    If strSQL = "" Then strSQL = "XX"
                    rsTestata!Iniziali = strSQL
                    '---------
                    'giu070319
                    If IsDBNull(rsTestata!FatturaAC) Then rsTestata!FatturaAC = False
                    If IsDBNull(rsTestata!ScGiacenza) Then rsTestata!ScGiacenza = False
                    If IsDBNull(rsTestata!Acconto) Then rsTestata!Acconto = 0
                    If IsDBNull(rsTestata!TipoRapportoPA) Then rsTestata!TipoRapportoPA = ""
                    If IsDBNull(rsTestata!Bollo) Then rsTestata!Bollo = 0
                    If IsDBNull(rsTestata!BolloACaricoDel) Then rsTestata!BolloACaricoDel = ""
                    '---------
                    'giu020321 SCADENZE OLTRE 5 RATE
                    'AZZERO
                    rsTestata!Data_Scadenza_6 = DBNull.Value
                    rsTestata!Rata_6 = 0
                    rsTestata!Data_Scadenza_7 = DBNull.Value
                    rsTestata!Rata_7 = 0
                    rsTestata!Data_Scadenza_8 = DBNull.Value
                    rsTestata!Rata_8 = 0
                    rsTestata!Data_Scadenza_9 = DBNull.Value
                    rsTestata!Rata_9 = 0
                    rsTestata!Data_Scadenza_10 = DBNull.Value
                    rsTestata!Rata_10 = 0
                    rsTestata!Data_Scadenza_11 = DBNull.Value
                    rsTestata!Rata_11 = 0
                    rsTestata!Data_Scadenza_12 = DBNull.Value
                    rsTestata!Rata_12 = 0
                    '-
                    Dim TotRate As Decimal = myTotNettoPagareMM
                    Dim NRate As Integer = 0
                    'Dim ArrScadPag As ArrayList
                    Try
                        If IsDBNull(rsTestata!NoteNonEvasione) Then
                            rsTestata!NoteNonEvasione = ""
                        End If
                        'ArrScadPag = New ArrayList
                        Dim myScad As ScadPagEntity
                        If rsTestata!NoteNonEvasione.ToString.Trim <> "" Then
                            Dim lineaSplit As String() = rsTestata!NoteNonEvasione.Split(";")
                            TotRate = 0
                            For i = 0 To lineaSplit.Count - 1
                                If lineaSplit(i).Trim <> "" And (i + 2) <= lineaSplit.Count - 1 Then

                                    myScad = New ScadPagEntity
                                    myScad.NRata = lineaSplit(i).Trim
                                    i += 1
                                    myScad.Data = lineaSplit(i).Trim
                                    i += 1
                                    myScad.Importo = lineaSplit(i).Trim
                                    TotRate += CDec(myScad.Importo)
                                    Select Case NRate
                                        Case 0
                                            rsTestata!Data_Scadenza_1 = CDate(myScad.Data)
                                            rsTestata!Rata_1 = CDec(myScad.Importo)
                                        Case 1
                                            rsTestata!Data_Scadenza_2 = CDate(myScad.Data)
                                            rsTestata!Rata_2 = CDec(myScad.Importo)
                                        Case 2
                                            rsTestata!Data_Scadenza_3 = CDate(myScad.Data)
                                            rsTestata!Rata_3 = CDec(myScad.Importo)
                                        Case 3
                                            rsTestata!Data_Scadenza_4 = CDate(myScad.Data)
                                            rsTestata!Rata_4 = CDec(myScad.Importo)
                                        Case 4
                                            rsTestata!Data_Scadenza_5 = CDate(myScad.Data)
                                            rsTestata!Rata_5 = CDec(myScad.Importo)
                                        Case 5
                                            rsTestata!Data_Scadenza_6 = CDate(myScad.Data)
                                            rsTestata!Rata_6 = CDec(myScad.Importo)
                                        Case 6
                                            rsTestata!Data_Scadenza_7 = CDate(myScad.Data)
                                            rsTestata!Rata_7 = CDec(myScad.Importo)
                                        Case 7
                                            rsTestata!Data_Scadenza_8 = CDate(myScad.Data)
                                            rsTestata!Rata_8 = CDec(myScad.Importo)
                                        Case 8
                                            rsTestata!Data_Scadenza_9 = CDate(myScad.Data)
                                            rsTestata!Rata_9 = CDec(myScad.Importo)
                                        Case 9
                                            rsTestata!Data_Scadenza_10 = CDate(myScad.Data)
                                            rsTestata!Rata_10 = CDec(myScad.Importo)
                                        Case 10
                                            rsTestata!Data_Scadenza_11 = CDate(myScad.Data)
                                            rsTestata!Rata_11 = CDec(myScad.Importo)
                                        Case 11
                                            rsTestata!Data_Scadenza_12 = CDate(myScad.Data)
                                            rsTestata!Rata_12 = CDec(myScad.Importo)
                                    End Select
                                    '-
                                    'ArrScadPag.Add(myScad)
                                    NRate += 1
                                End If
                            Next
                        End If
                    Catch ex As Exception
                        NRate = 0
                        'TotRate = 0
                    End Try
                    If myTotNettoPagareMM <> TotRate Then
                        Errore = "Errore - Totale Netto a Pagare diverso dal Totale Rate Scadenze. Passo: " & Passo.ToString & " <br> " &
                                 "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                        Return False
                        Exit Function
                    End If
                    '-------------------------------
                    rsTestata.EndEdit()
                    rsTestata.AcceptChanges()
                Next
            Else
                Errore = "Errore - Documento lettura testata. Passo: " & Passo.ToString & " <br> " &
                "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
        Catch ex As Exception
            Listino = "1"
            CodValuta = "Euro"
            DecimaliVal = "2" 'Euro
            SpeseIncasso = "0"
            SpeseTrasporto = "0"
            SpeseImballo = "0"
            SpeseVarie = "0"
            RegimeIVA = "0"
            ScCassa = "0"
            Abbuono = "0"
            ImponibileMemo = 0
            ImpostaMemo = 0
            TotaleMemo = 0
            TotaleMMemo = 0
            myTotaleRAMM = 0
            myTotaleRACK = 0
            myTotNettoPagareCK = 0
            myTotNettoPagareMM = 0
            Errore = ex.Message & " - Documento lettura testata. Passo: " & Passo.ToString & " <br> " &
            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        If TipoDoc <> myTipoDoc Then 'giu190419
            TipoDoc = myTipoDoc
        End If
        Passo = 5
        'Ricalcolo Importo
        Dim MyImporto As Decimal = 0
        Dim rsDettagli As DataRow
        If (DsPrinWebDoc.Tables("DocumentiD").Rows.Count > 0) Then
            Dim myQtaO As Decimal = 0 : Dim myQtaE As Decimal = 0 : Dim myQtaR As Decimal = 0
            Dim myQuantita As Decimal = 0 : Dim SWQta = ""
            '---- Calcolo sconto su 
            Dim ScontoSuImporto As Boolean = True
            Dim ScCassaDett As Boolean = False
            Try
                ScontoSuImporto = App.GetParamGestAzi(_Esercizio).CalcoloScontoSuImporto
                ScCassaDett = App.GetParamGestAzi(_Esercizio).ScCassaDett
            Catch ex As Exception
                ScontoSuImporto = True
                ScCassaDett = False
            End Try
            '-------------------------------------------------------------------------------------
            Passo = 6
            'GIU020512
            For Each rsDettagli In DsPrinWebDoc.Tables("DocumentiD").Select("IDDocumenti=" & IdDocumento.ToString.Trim, "Riga")
                Select Case Left(TipoDoc, 1)
                    Case "O"
                        myQuantita = rsDettagli![Qta_Ordinata]
                        SWQta = "O"
                    Case Else
                        If TipoDoc = "PR" Or TipoDoc = "CA" Or TipoDoc = "TC" Then 'GIU021219
                            myQuantita = rsDettagli![Qta_Ordinata]
                            SWQta = "O"
                        Else
                            myQuantita = rsDettagli![Qta_Evasa]
                            SWQta = "E"
                        End If
                End Select
                'giu020519 FATTURE PER ACCONTI 
                rsDettagli.BeginEdit()
                If IsDBNull(rsDettagli!DedPerAcconto) Then rsDettagli!DedPerAcconto = False
                rsDettagli.EndEdit()
                rsDettagli.AcceptChanges()
                'CONTROLLO IMPORTO CHE SIANO UGLUALI
                TotaleMCheck += rsDettagli![Importo]
                MyImporto = CalcolaImporto(rsDettagli![Prezzo], myQuantita,
                        rsDettagli![Sconto_1],
                        rsDettagli![Sconto_2],
                        rsDettagli![Sconto_3],
                        rsDettagli![Sconto_4],
                        rsDettagli![Sconto_Pag],
                        rsDettagli![ScontoValore],
                        rsDettagli![Importo],
                        ScontoSuImporto,
                        CInt(DecimaliVal),
                        rsDettagli![Prezzo_Netto], ScCassaDett, CDec(ScCassa), rsDettagli!DedPerAcconto) 'giu010119) giu020519
                If rsDettagli![Importo] <> MyImporto Then
                    Errore = "Errore CONTROLLO Documento: Importo riga: (" & rsDettagli![Riga].ToString.Trim & ") diverso dall'importo riga ricalcolato. Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
                'giu270913
                rsDettagli![StampaLotti] = False 'GIU181219
                If myQuantita > 0 Then
                    If DsPrinWebDoc.Tables("DocumentiDLotti").Select("IDDocumenti=" & myIDDoc & " And Riga=" & rsDettagli![Riga].ToString.Trim).Length > 0 Then
                        rsDettagli![StampaLotti] = True
                    End If
                End If
                '---------
            Next
            Passo = 7
            'giu060319 al TotaleChek bisogna aggiungere l'IVA quindi il test verra fatto dopo CheckTotDoc che mi somma l'IVA MENTRE il Totale NettoAPagare è quello che puo non contenere IVA (SPLIT)
            ' ''If TotaleMCheck <> TotaleMMemo Then
            ' ''    Errore = "CONTROLLO Totali Merce(1) Documento (CheckTotDoc). Passo: " & Passo.ToString & " <br> " & _
            ' ''        "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe."
            ' ''    Return False
            ' ''    Exit Function
            ' ''End If
            Passo = 8
            strErrore = ""
            'giu020512 per l stampa ALLL
            'GIU031013 IVA 21 E 22 DAL 01/10/2013
            Dim SWIVA2122 As Boolean = False
            'GIU210819 If _Esercizio.Trim = "2013" Then
            If CDate(myDataDoc) < CDate("01/10/2013") Then
                SWIVA2122 = True
            End If
            'GIU210819 End If
            '------------------------------------
            If CheckTotDoc(DsPrinWebDoc, _Esercizio, TipoDoc, DecimaliVal, Listino, IdDocumento, strErrore, SWIVA2122) = False Then
                If strErrore.Trim <> "" Then
                    Errore = strErrore & " - CONTROLLO Documento (CheckTotDoc). Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
            End If
            'secondo controllo perche viene ricalcolato in checkTot
            Passo = 9
            'giu060319 al TotaleMCheck è stato corretto quindi per le stampe non CONTROLLO PERCHE' E' SICURAMENTE DIVERSO PER I VECCHI DOCUMENTI
            ' ''If TotaleMCheck <> TotaleMMemo Then
            ' ''    Errore = "CONTROLLO Totali Merce(2) Documento (CheckTotDoc). Passo: " & Passo.ToString & " <br> " & _
            ' ''        "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe."
            ' ''    Return False
            ' ''    Exit Function
            ' ''End If
            ImponibileCheck = FormatCurrency(ImponibileCheck, 2)
            ImpostaCheck = FormatCurrency(ImpostaCheck, 2)
            TotaleCheck = FormatCurrency(TotaleCheck, 2)
            If ImponibileCheck <> ImponibileMemo Or ImpostaCheck <> ImpostaMemo Or TotaleCheck <> TotaleMemo Then
                Errore = "CONTROLLO Tot.(CheckTotDoc). Passo: " & Passo.ToString & " Imp.: " & ImponibileCheck.ToString & " " & ImponibileMemo.ToString & " IVA: " & ImpostaCheck.ToString & " " & ImpostaMemo.ToString & " Tot.: " & TotaleCheck.ToString & " " & TotaleMemo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
            'giu180118 giu190118
            Passo = 10
            If mySplitIVA Then
                myTotNettoPagareCK = ImponibileCheck
            Else
                myTotNettoPagareCK = TotaleCheck
            End If
            If myRitAcconto Then
                myTotNettoPagareCK = myTotNettoPagareCK - myTotaleRAMM
            End If
            'GIU140320
            If Abbuono <> 0 Then
                myTotNettoPagareCK = myTotNettoPagareCK + Abbuono
            End If
            '---------
            If myTotNettoPagareCK <> myTotNettoPagareMM Then
                'GIU300819 IL NETTO A PAGARE PRIMA DEL 2019 POTREBBE ESSERE COMPRESO IVA QUINDI RIPROVO DOPO AVER SOTTRATTO L'IVA (SPLIT PAYMENT)
                If myTotNettoPagareCK <> myTotNettoPagareMM - ImpostaMemo Then
                    Errore = "CONTROLLO Totale Netto da pagare (StampaDocumentoDalAl). Passo: " & Passo.ToString & " TotNP.: " & myTotNettoPagareCK.ToString & " " & myTotNettoPagareMM.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End If
                '----------------------------
            End If
            '---------
            'giu020519
            If myTotNettoPagareCK = 0 And myTotDeduzioniCK = 0 And myTotLordoMerceCK = 0 Then
                Errore = "CONTROLLO Totale Netto da pagare/Totale Deduzioni=0 / Tot.Lordo Merce=0 (StampaDocumentoDalAl). Passo: " & Passo.ToString & " <br> " &
                    "Per risolvere il problema controllare le righe di dettaglio modificando e aggiornando le singole righe." & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End If
            '---------
        End If
        'giu030112 per i campi NULL in stampa
        Dim rsTBDL As DataRow
        Try
            If (DsPrinWebDoc.Tables("DocumentiDLotti").Rows.Count > 0) Then
                'GIU020512 OK 
                For Each rsTBDL In DsPrinWebDoc.Tables("DocumentiDLotti").Select("IDDocumenti=" & IdDocumento.ToString.Trim)
                    rsTBDL.BeginEdit()
                    If IsDBNull(rsTBDL!Cod_Articolo) Then rsTBDL!Cod_Articolo = ""
                    If IsDBNull(rsTBDL!Lotto) Then rsTBDL!Lotto = ""
                    If IsDBNull(rsTBDL!QtaColli) Then rsTBDL!QtaColli = 0
                    If IsDBNull(rsTBDL!Sfusi) Then rsTBDL!Sfusi = 0
                    If IsDBNull(rsTBDL!NSerie) Then rsTBDL!NSerie = ""
                    rsTBDL.EndEdit()
                    rsTBDL.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. DocumentiDLotti CAMPI NULL" & " <br> " &
            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '---------------------------------------------------------------------------------------------
        'SWScontiDoc per stampare il RPT con o senza la colonna sconti PREVENTIVI/ORDINI
        Passo = 11
        If (DsPrinWebDoc.Tables("DocumentiD").Select("IDDocumenti=" & IdDocumento.ToString.Trim & " AND (Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0)").Count > 0) Then
            SWSconti = True
        Else
            SWSconti = False
        End If
        '---------------------------------------------------------------------------------------------
        Passo = 12
        If (DsPrinWebDoc.Tables("Ditta").Rows.Count = 0) Then
            strSQL = "Select * From Ditta WHERE Codice = '" & CodiceDitta.Trim & "'"
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, DsPrinWebDoc, "Ditta")
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Ditta" & " <br> " &
                "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        End If
        '---
        'giu020512
        If DsPrinWebDoc.Tables("Clienti").Select("Codice_CoGe = '" & myCodiceCoGe.Trim & "'").Length > 0 Then
            'OK GIA' CARICATO
        Else
            If TabCliFor = "Cli" Then
                strSQL = "Select * From Clienti WHERE Codice_CoGe = '" & myCodiceCoGe.Trim & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Clienti" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            ElseIf TabCliFor = "For" Then
                strSQL = "Select * From Fornitori WHERE Codice_CoGe = '" & myCodiceCoGe.Trim & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Fornitori" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            ElseIf TabCliFor = "Provv" Then
                'GIU140513 Stato AS Nazione
                strSQL = "Select AnagrProvv.*, " & myCodiceCoGe.Trim & " AS Codice_CoGe, Ragione_Sociale AS Rag_Soc '' AS NumeroCivico, Stato AS Nazione From AnagrProvv WHERE IDAnagrProvv = " & myCodiceCoGe.Trim & ""
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. ClientiProvv" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            Else
                strSQL = "Select * From CliFor WHERE Codice_CoGe = '" & myCodiceCoGe.Trim & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Clienti/Fornitori" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        'giu020512
        'Aggiorno in Clienti il Codice
        Dim myrsTestata As DSPrintWeb_Documenti.DocumentiTRow
        myrsTestata = Nothing
        If (DsPrinWebDoc.Tables("DocumentiT").Rows.Count > 0) Then
            For Each rsTestata In DsPrinWebDoc.Tables("DocumentiT").Select("IDDocumenti=" & IdDocumento.ToString.Trim)
                rsTestata.BeginEdit()
                rsTestata![Cod_Cliente] = myCodiceCoGe.Trim
                rsTestata.EndEdit()
                rsTestata.AcceptChanges()
                myrsTestata = rsTestata
            Next
        End If
        'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
        Dim rsTBCliFor As DataRow
        myNazione = ""
        Try
            If (DsPrinWebDoc.Tables("Clienti").Rows.Count > 0) Then
                For Each rsTBCliFor In DsPrinWebDoc.Tables("Clienti").Select("Codice_CoGe = '" & myCodiceCoGe.Trim & "'")
                    rsTBCliFor.BeginEdit()
                    If IsDBNull(rsTBCliFor!Rag_Soc) Then rsTBCliFor!Rag_Soc = ""
                    If IsDBNull(rsTBCliFor!Indirizzo) Then rsTBCliFor!Indirizzo = ""
                    If IsDBNull(rsTBCliFor!Localita) Then rsTBCliFor!Localita = ""
                    If IsDBNull(rsTBCliFor!CAP) Then rsTBCliFor!CAP = ""
                    If IsDBNull(rsTBCliFor!Provincia) Then rsTBCliFor!Provincia = ""
                    If IsDBNull(rsTBCliFor!Nazione) Then rsTBCliFor!Nazione = ""
                    If rsTBCliFor!Nazione.ToString.Trim = "" Then rsTBCliFor!Nazione = "I" 'GIU140513
                    myNazione = rsTBCliFor!Nazione
                    If IsDBNull(rsTBCliFor!Telefono1) Then rsTBCliFor!Telefono1 = ""
                    If IsDBNull(rsTBCliFor!Telefono2) Then rsTBCliFor!Telefono2 = ""
                    If IsDBNull(rsTBCliFor!Fax) Then rsTBCliFor!Fax = ""
                    If IsDBNull(rsTBCliFor!Codice_Fiscale) Then rsTBCliFor!Codice_Fiscale = ""
                    If IsDBNull(rsTBCliFor!Partita_IVA) Then rsTBCliFor!Partita_IVA = ""
                    If IsDBNull(rsTBCliFor!Denominazione) Then rsTBCliFor!Denominazione = ""
                    If IsDBNull(rsTBCliFor!Titolare) Then rsTBCliFor!Titolare = ""
                    If IsDBNull(rsTBCliFor!Email) Then rsTBCliFor!Email = ""
                    If IsDBNull(rsTBCliFor!ABI_N) Then rsTBCliFor!ABI_N = ""
                    If IsDBNull(rsTBCliFor!CAB_N) Then rsTBCliFor!CAB_N = ""
                    If IsDBNull(rsTBCliFor!Provincia_Estera) Then rsTBCliFor!Provincia_Estera = ""
                    If IsDBNull(rsTBCliFor!IndirizzoSenzaNumero) Then rsTBCliFor!IndirizzoSenzaNumero = ""
                    If IsDBNull(rsTBCliFor!NumeroCivico) Then rsTBCliFor!NumeroCivico = ""
                    If IsDBNull(rsTBCliFor!Note) Then rsTBCliFor!Note = ""
                    If IsDBNull(rsTBCliFor!IVASosp) Then rsTBCliFor!IVASosp = 0 'giu070212
                    If IsDBNull(rsTBCliFor!IPA) Then rsTBCliFor!IPA = "" 'GIU120814
                    If IsDBNull(rsTBCliFor!SplitIVA) Then rsTBCliFor!SplitIVA = False
                    'giu140620
                    If IsDBNull(rsTBCliFor!IBAN_Ditta) Then rsTBCliFor!IBAN_Ditta = ""
                    If IsDBNull(rsTBCliFor!EmailInvioScad) Then rsTBCliFor!EmailInvioScad = ""
                    If IsDBNull(rsTBCliFor!EmailInvioFatt) Then rsTBCliFor!EmailInvioFatt = ""
                    If IsDBNull(rsTBCliFor!PECEmail) Then rsTBCliFor!PECEmail = ""
                    rsTBCliFor.EndEdit()
                    rsTBCliFor.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Clienti/Fornitori. CAMPI NULL" & " <br> " &
            "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '-Destinazione Merce
        If myCodFiliale.Trim <> "" Then
            If DsPrinWebDoc.Tables("DestClienti").Select("Progressivo = " & myCodFiliale.Trim & "").Length > 0 Then
                'OK GIA' CARICATO
            Else
                strSQL = "Select * From DestClienti WHERE Progressivo = " & myCodFiliale.Trim & ""
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "DestClienti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. DestClienti" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
            'giu030112 per i campi NULL in stampa non vengono stampati tel. fax PI/CF
            Dim rsTBDest As DataRow
            Try
                If (DsPrinWebDoc.Tables("DestClienti").Rows.Count > 0) Then
                    For Each rsTBDest In DsPrinWebDoc.Tables("DestClienti").Select("Progressivo = " & myCodFiliale.Trim)
                        rsTBDest.BeginEdit()
                        If IsDBNull(rsTBDest!Ragione_Sociale) Then rsTBDest!Ragione_Sociale = ""
                        If IsDBNull(rsTBDest!Indirizzo) Then rsTBDest!Indirizzo = ""
                        If IsDBNull(rsTBDest!Localita) Then rsTBDest!Localita = ""
                        If IsDBNull(rsTBDest!CAP) Then rsTBDest!CAP = ""
                        If IsDBNull(rsTBDest!Provincia) Then rsTBDest!Provincia = ""
                        If IsDBNull(rsTBDest!Stato) Then rsTBDest!Stato = ""
                        If IsDBNull(rsTBDest!Telefono1) Then rsTBDest!Telefono1 = ""
                        If IsDBNull(rsTBDest!Telefono2) Then rsTBDest!Telefono2 = ""
                        If IsDBNull(rsTBDest!Fax) Then rsTBDest!Fax = ""
                        If IsDBNull(rsTBDest!Denominazione) Then rsTBDest!Denominazione = ""
                        If IsDBNull(rsTBDest!Riferimento) Then rsTBDest!Riferimento = ""
                        If IsDBNull(rsTBDest!Email) Then rsTBDest!Email = ""
                        If IsDBNull(rsTBDest!Tipo) Then rsTBDest!Tipo = ""
                        rsTBDest.EndEdit()
                        rsTBDest.AcceptChanges()
                    Next
                End If
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Destinazione Merce CAMPI NULL" & " <br> " &
                "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try

        End If
        '------------------------------------------------------------------------
        'TESTATA DOCUMENTO CORRENTE
        If (myrsTestata Is Nothing) Then
            Errore = "Errore - Testata documento non valida"
            Return False
            Exit Function
        End If
        '--------------------------
        If Not IsDBNull(myrsTestata!Cod_Pagamento) Then
            If DsPrinWebDoc.Tables("Pagamenti").Select("Codice = " & myrsTestata!Cod_Pagamento.ToString.Trim).Length > 0 Then
                'OK GIA' CARICATO
            Else
                strSQL = "Select * From Pagamenti WHERE Codice = " & myrsTestata!Cod_Pagamento.ToString.Trim
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Pagamenti", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Pagamenti" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        '-
        If Not IsDBNull(myrsTestata!Cod_Valuta) Then
            If DsPrinWebDoc.Tables("Valute").Select("Codice = '" & myrsTestata!Cod_Valuta.ToString.Trim & "'").Length > 0 Then
                'OK GIA' CARICATO
            Else
                strSQL = "Select * From Valute WHERE Codice = '" & myrsTestata!Cod_Valuta.ToString.Trim & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Valute", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Valute" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        'GIU01122011 ATTENZIONE SE ESTRAE 2 RKS DUPLICA I DETTAGLI DEGLI ARTICOLO
        If Not IsDBNull(myrsTestata!IBAN) Then
            If DsPrinWebDoc.Tables("BancheIBAN").Select("IBAN = '" & myrsTestata!IBAN.ToString.Trim & "'").Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select TOP 1 * From BancheIBAN WHERE (IBAN = '" & myrsTestata!IBAN.ToString.Trim & "')"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "BancheIBAN", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. BancheIBAN" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        ElseIf Not IsDBNull(myrsTestata!ABI) And Not IsDBNull(myrsTestata!CAB) Then
            If myrsTestata!ABI.ToString.Trim <> "" And myrsTestata!CAB.ToString.Trim <> "" Then
                If DsPrinWebDoc.Tables("BancheIBAN").Select("(ABI = '" & myrsTestata!ABI.ToString.Trim & "') And (CAB = '" & myrsTestata!CAB.ToString.Trim & "')").Length > 0 Then
                    'gia' caricato
                Else
                    strSQL = "Select TOP 1 * From BancheIBAN WHERE (ABI = '" & myrsTestata!ABI.ToString.Trim & "') And (CAB = '" & myrsTestata!CAB.ToString.Trim & "')"
                    Try
                        ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "BancheIBAN", _Esercizio) 'giu230419
                    Catch ex As Exception
                        Errore = ex.Message & " - Tab. BancheIBAN" & " <br> " &
                        "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                        Return False
                        Exit Function
                    End Try
                End If
            End If
        End If
        '---
        If Not IsDBNull(myrsTestata!Vettore_1) Then
            If DsPrinWebDoc.Tables("Vettori_1").Select("Codice = " & myrsTestata!Vettore_1.ToString.Trim).Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select * From Vettori WHERE (Codice = " & myrsTestata!Vettore_1.ToString.Trim & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettori_1", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Vettore1" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                'giu030112 per i campi NULL in stampa
                Dim rsTBV1 As DataRow
                Try
                    If (DsPrinWebDoc.Tables("Vettori_1").Rows.Count > 0) Then
                        For Each rsTBV1 In DsPrinWebDoc.Tables("Vettori_1").Select("Codice = " & myrsTestata!Vettore_1.ToString.Trim)
                            rsTBV1.BeginEdit()
                            If IsDBNull(rsTBV1!Descrizione) Then rsTBV1!Descrizione = ""
                            If IsDBNull(rsTBV1!Residenza) Then rsTBV1!Residenza = ""
                            If IsDBNull(rsTBV1!Localita) Then rsTBV1!Localita = ""
                            If IsDBNull(rsTBV1!Provincia) Then rsTBV1!Provincia = ""
                            If IsDBNull(rsTBV1!Partita_IVA) Then rsTBV1!Partita_IVA = ""
                            If IsDBNull(rsTBV1!Codice_CoGe) Then rsTBV1!Codice_CoGe = ""
                            rsTBV1.EndEdit()
                            rsTBV1.AcceptChanges()
                        Next
                    End If
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Vettori_1 CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                '------------------------------------------------------------------------
            End If
        End If
        '-
        If Not IsDBNull(myrsTestata!Vettore_2) Then
            If DsPrinWebDoc.Tables("Vettori_2").Select("Codice = " & myrsTestata!Vettore_2.ToString.Trim).Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select * From Vettori WHERE (Codice = " & myrsTestata!Vettore_2.ToString.Trim & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettori_2", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Vettore2" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                'giu030112 per i campi NULL in stampa
                Dim rsTBV2 As DataRow
                Try
                    If (DsPrinWebDoc.Tables("Vettori_2").Rows.Count > 0) Then
                        For Each rsTBV2 In DsPrinWebDoc.Tables("Vettori_2").Select("Codice = " & myrsTestata!Vettore_2.ToString.Trim)
                            rsTBV2.BeginEdit()
                            If IsDBNull(rsTBV2!Descrizione) Then rsTBV2!Descrizione = ""
                            If IsDBNull(rsTBV2!Residenza) Then rsTBV2!Residenza = ""
                            If IsDBNull(rsTBV2!Localita) Then rsTBV2!Localita = ""
                            If IsDBNull(rsTBV2!Provincia) Then rsTBV2!Provincia = ""
                            If IsDBNull(rsTBV2!Partita_IVA) Then rsTBV2!Partita_IVA = ""
                            If IsDBNull(rsTBV2!Codice_CoGe) Then rsTBV2!Codice_CoGe = ""
                            rsTBV2.EndEdit()
                            rsTBV2.AcceptChanges()
                        Next
                    End If
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Vettori_2 CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                '------------------------------------------------------------------------
            End If
        End If
        '-
        If Not IsDBNull(myrsTestata!Vettore_3) Then
            If DsPrinWebDoc.Tables("Vettore_3").Select("Codice = " & myrsTestata!Vettore_3.ToString.Trim).Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select * From Vettori WHERE (Codice = " & myrsTestata!Vettore_3.ToString.Trim & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Vettore_3", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Vettore3" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                'giu030112 per i campi NULL in stampa
                Dim rsTBV3 As DataRow
                Try
                    If (DsPrinWebDoc.Tables("Vettore_3").Rows.Count > 0) Then
                        For Each rsTBV3 In DsPrinWebDoc.Tables("Vettore_3").Select("Codice = " & myrsTestata!Vettore_3.ToString.Trim)
                            rsTBV3.BeginEdit()
                            If IsDBNull(rsTBV3!Descrizione) Then rsTBV3!Descrizione = ""
                            If IsDBNull(rsTBV3!Residenza) Then rsTBV3!Residenza = ""
                            If IsDBNull(rsTBV3!Localita) Then rsTBV3!Localita = ""
                            If IsDBNull(rsTBV3!Provincia) Then rsTBV3!Provincia = ""
                            If IsDBNull(rsTBV3!Partita_IVA) Then rsTBV3!Partita_IVA = ""
                            If IsDBNull(rsTBV3!Codice_CoGe) Then rsTBV3!Codice_CoGe = ""
                            rsTBV3.EndEdit()
                            rsTBV3.AcceptChanges()
                        Next
                    End If
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Vettori_3 CAMPI NULL" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
                '------------------------------------------------------------------------
            End If
        End If
        '------------------------------
        'Aliquote IVA per il BLOCCO IVA
        If Not IsDBNull(myrsTestata!Iva1) Then
            If DsPrinWebDoc.Tables("Aliquote_IVA1").Select("Aliquota = " & myrsTestata!Iva1.ToString.Trim).Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & myrsTestata!Iva1.ToString.Trim & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA1", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Aliquota_IVA1" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        '-
        If Not IsDBNull(myrsTestata!Iva2) Then
            If DsPrinWebDoc.Tables("Aliquote_IVA2").Select("Aliquota = " & myrsTestata!Iva2.ToString.Trim).Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & myrsTestata!Iva2.ToString.Trim & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA2", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Aliquota_IVA2" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        '-
        If Not IsDBNull(myrsTestata!Iva3) Then
            If DsPrinWebDoc.Tables("Aliquote_IVA3").Select("Aliquota = " & myrsTestata!Iva3.ToString.Trim).Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & myrsTestata!Iva3.ToString.Trim & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA3", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Aliquota_IVA3" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        '-
        If Not IsDBNull(myrsTestata!Iva4) Then
            If DsPrinWebDoc.Tables("Aliquote_IVA4").Select("Aliquota = " & myrsTestata!Iva4.ToString.Trim).Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select * From Aliquote_IVA WHERE (Aliquota = " & myrsTestata!Iva4.ToString.Trim & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Aliquote_IVA4", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Aliquota_IVA4" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        '-
        '-CausMag
        If Not IsDBNull(myrsTestata!Cod_Causale) Then
            If DsPrinWebDoc.Tables("CausMag").Select("Codice = " & myrsTestata!Cod_Causale.ToString.Trim).Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select Codice, Descrizione From CausMag WHERE (Codice = " & DsPrinWebDoc.DocumentiT.Rows(0).Item("Cod_Causale") & ") "
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "CausMag", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. CausMag" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        'giu130412
        If myNazione.Trim <> "" Then
            If DsPrinWebDoc.Tables("Nazioni").Select("Codice = '" & myNazione.Trim & "'").Length > 0 Then
                'gia' caricato
            Else
                strSQL = "Select * From Nazioni WHERE Codice = '" & myNazione.Trim & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Nazioni", _Esercizio) 'giu230419
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Nazioni" & " <br> " &
                    "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                    Return False
                    Exit Function
                End Try
            End If
        End If
        'giu310823 carico tutte le righe lotto come riga descrittiva - 2 per riga
        If SWStampaDocLotti = True Then
            Try
                If (DsPrinWebDoc.Tables("DocumentiDLotti").Rows.Count > 0) Then
                    strValore = ""
                    Dim NLS As Integer = 0 : Dim myNCollo As Integer : Dim NRigaPrec As Integer = 0
                    For Each rsTBDL In DsPrinWebDoc.Tables("DocumentiDLotti").Select("IDDocumenti=" + myIDDoc.Trim)

                        If NRigaPrec = 0 Then
                            NRigaPrec = rsTBDL!Riga
                        ElseIf NRigaPrec <> rsTBDL!Riga Then
                            If NLS <> 0 Then
                                Dim newRowDocD As DSPrintWeb_Documenti.DocumentiDRow = DsPrinWebDoc.DocumentiD.NewDocumentiDRow
                                With newRowDocD
                                    .BeginEdit()
                                    .IDDocumenti = CLng(myIDDoc)
                                    .Riga = NRigaPrec
                                    .NCollo = myNCollo
                                    .Descrizione = strValore.Trim
                                    .Prezzo = 0
                                    .Prezzo_Netto = 0
                                    .Qta_Allestita = 0
                                    .Qta_Evasa = 0
                                    .Qta_Impegnata = 0
                                    .Qta_Ordinata = 0
                                    .Qta_Prenotata = 0
                                    .Qta_Residua = 0
                                    .Importo = 0
                                    'giu170412
                                    .PrezzoAcquisto = 0
                                    .PrezzoListino = 0
                                    'giu190412
                                    .SWModAgenti = False
                                    .PrezzoCosto = 0
                                    .Qta_Inviata = 0
                                    .Qta_Fatturata = 0
                                    '---------
                                    .EndEdit()
                                End With
                                DsPrinWebDoc.DocumentiD.AddDocumentiDRow(newRowDocD)
                                newRowDocD = Nothing
                            End If
                            '-
                            NLS = 0
                            strValore = ""
                            NRigaPrec = rsTBDL!Riga
                        End If
                        '-
                        If strValore.Trim <> "" Then
                            strValore += " - (" + rsTBDL!QtaColli.ToString.Trim + ") "
                        Else
                            strValore += "(" + rsTBDL!QtaColli.ToString.Trim + ") "
                        End If
                        '------------
                        If IsDBNull(rsTBDL!Lotto) Then
                            'nulla
                        ElseIf rsTBDL!Lotto.ToString.Trim <> "" Then
                            If strValore.Trim <> "" Then
                                strValore += " - Lotto: " + rsTBDL!Lotto.ToString.Trim + " "
                            Else
                                strValore += "Lotto: " + rsTBDL!Lotto.ToString.Trim + " "
                            End If
                            '-
                            NLS += 1
                        End If
                        '------------
                        myNCollo = rsTBDL!NCollo
                        If NLS > 1 Then
                            Dim newRowDocD As DSPrintWeb_Documenti.DocumentiDRow = DsPrinWebDoc.DocumentiD.NewDocumentiDRow
                            With newRowDocD
                                .BeginEdit()
                                .IDDocumenti = CLng(myIDDoc)
                                .Riga = NRigaPrec
                                .NCollo = myNCollo
                                .Descrizione = strValore.Trim
                                .Prezzo = 0
                                .Prezzo_Netto = 0
                                .Qta_Allestita = 0
                                .Qta_Evasa = 0
                                .Qta_Impegnata = 0
                                .Qta_Ordinata = 0
                                .Qta_Prenotata = 0
                                .Qta_Residua = 0
                                .Importo = 0
                                'giu170412
                                .PrezzoAcquisto = 0
                                .PrezzoListino = 0
                                'giu190412
                                .SWModAgenti = False
                                .PrezzoCosto = 0
                                .Qta_Inviata = 0
                                .Qta_Fatturata = 0
                                '---------
                                .EndEdit()
                            End With
                            DsPrinWebDoc.DocumentiD.AddDocumentiDRow(newRowDocD)
                            newRowDocD = Nothing
                            '-
                            NLS = 0
                            strValore = ""
                        End If
                        '------------
                        If IsDBNull(rsTBDL!NSerie) Then
                            'nulla
                        ElseIf rsTBDL!NSerie.ToString.Trim <> "" Then
                            If strValore.Trim <> "" Then
                                strValore += " - N° Serie: " + rsTBDL!NSerie.ToString.Trim + " "
                            Else
                                strValore += "N° Serie: " + rsTBDL!NSerie.ToString.Trim + " "
                            End If
                            '
                            NLS += 1
                        End If
                        '------------
                        myNCollo = rsTBDL!NCollo
                        If NLS > 1 Then
                            Dim newRowDocD As DSPrintWeb_Documenti.DocumentiDRow = DsPrinWebDoc.DocumentiD.NewDocumentiDRow
                            With newRowDocD
                                .BeginEdit()
                                .IDDocumenti = CLng(myIDDoc)
                                .Riga = NRigaPrec
                                .NCollo = myNCollo
                                .Descrizione = strValore.Trim
                                .Prezzo = 0
                                .Prezzo_Netto = 0
                                .Qta_Allestita = 0
                                .Qta_Evasa = 0
                                .Qta_Impegnata = 0
                                .Qta_Ordinata = 0
                                .Qta_Prenotata = 0
                                .Qta_Residua = 0
                                .Importo = 0
                                'giu170412
                                .PrezzoAcquisto = 0
                                .PrezzoListino = 0
                                'giu190412
                                .SWModAgenti = False
                                .PrezzoCosto = 0
                                .Qta_Inviata = 0
                                .Qta_Fatturata = 0
                                '---------
                                .EndEdit()
                            End With
                            DsPrinWebDoc.DocumentiD.AddDocumentiDRow(newRowDocD)
                            newRowDocD = Nothing
                            '-
                            NLS = 0
                            strValore = ""
                        End If
                    Next
                    If NLS <> 0 Then
                        Dim newRowDocD As DSPrintWeb_Documenti.DocumentiDRow = DsPrinWebDoc.DocumentiD.NewDocumentiDRow
                        With newRowDocD
                            .BeginEdit()
                            .IDDocumenti = CLng(myIDDoc)
                            .Riga = NRigaPrec
                            .NCollo = myNCollo
                            .Descrizione = strValore.Trim
                            .Prezzo = 0
                            .Prezzo_Netto = 0
                            .Qta_Allestita = 0
                            .Qta_Evasa = 0
                            .Qta_Impegnata = 0
                            .Qta_Ordinata = 0
                            .Qta_Prenotata = 0
                            .Qta_Residua = 0
                            .Importo = 0
                            'giu170412
                            .PrezzoAcquisto = 0
                            .PrezzoListino = 0
                            'giu190412
                            .SWModAgenti = False
                            .PrezzoCosto = 0
                            .Qta_Inviata = 0
                            .Qta_Fatturata = 0
                            '---------
                            .EndEdit()
                        End With
                        DsPrinWebDoc.DocumentiD.AddDocumentiDRow(newRowDocD)
                        newRowDocD = Nothing
                    End If
                End If
                DsPrinWebDoc.DocumentiD.AcceptChanges()
            Catch ex As Exception
                Errore = ex.Message & " - Tab. DocumentiD inserimento LOTTI" & " <br> " &
                        "IDDocumento: " & myIDDoc.Trim & " N°Documento: " & myNDoc & " Tipo: " & myTipoDoc & " Data: " & myDataDoc
                Return False
                Exit Function
            End Try
        End If
        '==FINE CARICAMENTO ====================================
        ObjDB = Nothing

        Return True

    End Function

    Private Function CheckTotDoc(ByVal dsDocDett As DataSet, ByVal _Esercizio As String, ByVal TipoDoc As String, ByVal DecimaliVal As String, ByVal Listino As String, ByVal IdDocumento As Long, ByRef strErrore As String, ByVal SWCambioIVA As Boolean) As Boolean
        'giu030512
        strErrore = ""
        If Listino < 1 Then Listino = 1
        '--------
        '------------------------------------
        Dim FormatValuta As String = "###,###,##0"
        Select Case CInt(DecimaliVal)
            Case 0
                FormatValuta = "###,###,##0"
            Case 2
                FormatValuta = "###,###,##0.00"
            Case 3
                FormatValuta = "###,###,##0.000"
            Case 4
                FormatValuta = "###,###,##0.0000"
            Case Else
                FormatValuta = "###,###,##0"
        End Select
        '--------------------------------------------------
        'Blocco IVA
        Dim Iva(4) As Integer
        Dim Imponibile(4) As Decimal
        Dim Imposta(4) As Decimal
        'Spese Global
        Dim IvaTrasporto As Integer
        Dim IvaIncasso As Integer
        Dim IvaImballo As Integer

        Dim Totale As Decimal = 0 : Dim TotaleLordoMerce As Decimal = 0
        Dim MoltiplicatoreValuta As Integer 'Dichiarato ma mi sa che non server :)
        Dim TmpImp As Decimal
        Dim Cont As Integer
        '-GIU020119
        '---- Calcolo sconto su 
        Dim ScontoSuImporto As Boolean = True 'OK ANCHE SE NON SERVE QUI LA ScCassaDett SI SERVER
        Dim ScCassaDett As Boolean = False 'giu010119
        Try
            ScontoSuImporto = App.GetParamGestAzi(_Esercizio).CalcoloScontoSuImporto
            ScCassaDett = App.GetParamGestAzi(_Esercizio).ScCassaDett
        Catch ex As Exception
            ScontoSuImporto = True
            ScCassaDett = False
        End Try
        'giu010119
        strErrore = ""
        If CalcolaTotaleDoc(dsDocDett, Iva, Imponibile, Imposta, _
                              CInt(DecimaliVal), MoltiplicatoreValuta, _
                              Totale, TotaleLordoMerce, CDec(ScCassa), CLng(Val(Listino)), _
                              TipoDoc, CDec(Abbuono), IdDocumento, strErrore, ScCassaDett, 0, myTotDeduzioniCK) = False Then
            If strErrore <> "" Then
                strErrore = "Errore: in ChechTotDoc.CalcolaTotaleDoc - " & strErrore
            End If
            Exit Function
        End If
        myTotLordoMerceCK = TotaleLordoMerce 'giu290519
        Cont = 0

        If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        '-
        If Val(RegimeIVA) > 49 Then
            IvaTrasporto = Val(RegimeIVA)
            IvaImballo = Val(RegimeIVA)
            IvaIncasso = Val(RegimeIVA)
        Else
            IvaTrasporto = App.GetParamGestAzi(_Esercizio).IVATrasporto
            IvaIncasso = App.GetParamGestAzi(_Esercizio).IvaSpese
            IvaImballo = App.GetParamGestAzi(_Esercizio).Iva_Imballo
        End If
        'GIU031013 SWIVA2122
        If SWCambioIVA = True Then
            IvaTrasporto = 21 'App.GetParamGestAzi(_Esercizio).IVATrasporto
            IvaIncasso = 21 ' App.GetParamGestAzi(_Esercizio).IvaSpese
            IvaImballo = 21 'App.GetParamGestAzi(_Esercizio).Iva_Imballo
        End If
        '-------------------

        'ASSEGNAZIONE TOTALE SPESE TRASPORTO
        If IsNothing(SpeseTrasporto) Then SpeseTrasporto = "0"
        If String.IsNullOrEmpty(SpeseTrasporto) Then SpeseTrasporto = "0"
        If Not IsNumeric(SpeseTrasporto) Then SpeseTrasporto = "0"
        '-
        If CDec(SpeseTrasporto) Then
            While ((IvaTrasporto <> Iva(Cont)) And _
                    Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    strErrore = "Errore: in ChechTotDoc.CalcolaTotaleDoc - " & MSGEccedIva
                    Exit Function
                End If
            End While
            Iva(Cont) = IvaTrasporto
            TmpImp = CDec(SpeseTrasporto)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If IvaTrasporto < 50 Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        'ASSEGNAZIONE AL TOTALE DELLE SPESE DI IMBALLO E ATTRIBUZIONE NEL COMPUTO DELL'IVA
        If IsNothing(SpeseImballo) Then SpeseImballo = "0"
        If String.IsNullOrEmpty(SpeseImballo) Then SpeseImballo = "0"
        If Not IsNumeric(SpeseImballo) Then SpeseImballo = "0"
        '-
        If (CDec(SpeseImballo)) > 0 Then
            While ((IvaImballo <> Iva(Cont)) And Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    strErrore = "Errore: in ChechTotDoc.CalcolaTotaleDoc - " & MSGEccedIva
                    Exit Function
                End If
            End While
            Iva(Cont) = IvaImballo
            TmpImp = CDec(SpeseImballo)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If IvaImballo < 50 Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        If IsNothing(SpeseIncasso) Then SpeseIncasso = "0"
        If String.IsNullOrEmpty(SpeseIncasso) Then SpeseIncasso = "0"
        If Not IsNumeric(SpeseIncasso) Then SpeseIncasso = "0"
        '-
        If (CDec(SpeseIncasso)) > 0 Then
            While ((IvaIncasso <> Iva(Cont)) And Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    strErrore = "Errore: in ChechTotDoc.CalcolaTotaleDoc - " & MSGEccedIva
                    Exit Function
                End If
            End While
            Iva(Cont) = IvaIncasso
            TmpImp = CDec(SpeseIncasso)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If (Iva(Cont) > 0) And (Iva(Cont) < 50) Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        ImponibileCheck = Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4)
        ImpostaCheck = Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4)
        TotaleCheck = Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4)
        TotaleCheck += Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4)
        TotaleMCheck = TotaleLordoMerce 'giu050319

    End Function
    'giu031219
    Private Function CheckTotDocCA(ByVal dsDocDett As DataSet, ByVal _Esercizio As String, ByVal TipoDoc As String, ByVal DecimaliVal As String, ByVal Listino As String, ByVal IdDocumento As Long, ByRef strErrore As String, ByVal SWCambioIVA As Boolean) As Boolean
        'giu030512
        strErrore = ""
        If Listino < 1 Then Listino = 1
        '--------
        '------------------------------------
        Dim FormatValuta As String = "###,###,##0"
        Select Case CInt(DecimaliVal)
            Case 0
                FormatValuta = "###,###,##0"
            Case 2
                FormatValuta = "###,###,##0.00"
            Case 3
                FormatValuta = "###,###,##0.000"
            Case 4
                FormatValuta = "###,###,##0.0000"
            Case Else
                FormatValuta = "###,###,##0"
        End Select
        '--------------------------------------------------
        'Blocco IVA
        Dim Iva(4) As Integer
        Dim Imponibile(4) As Decimal
        Dim Imposta(4) As Decimal
        'Spese Global
        Dim IvaTrasporto As Integer
        Dim IvaIncasso As Integer
        Dim IvaImballo As Integer

        Dim Totale As Decimal = 0 : Dim TotaleLordoMerce As Decimal = 0
        Dim MoltiplicatoreValuta As Integer 'Dichiarato ma mi sa che non server :)
        Dim TmpImp As Decimal
        Dim Cont As Integer
        '-GIU020119
        '---- Calcolo sconto su 
        Dim ScontoSuImporto As Boolean = True 'OK ANCHE SE NON SERVE QUI LA ScCassaDett SI SERVER
        Dim ScCassaDett As Boolean = False 'giu010119
        Try
            ScontoSuImporto = App.GetParamGestAzi(_Esercizio).CalcoloScontoSuImporto
            ScCassaDett = App.GetParamGestAzi(_Esercizio).ScCassaDett
        Catch ex As Exception
            ScontoSuImporto = True
            ScCassaDett = False
        End Try
        'giu010119
        strErrore = ""
        If CalcolaTotaleDocCA(dsDocDett, Iva, Imponibile, Imposta, _
                              CInt(DecimaliVal), MoltiplicatoreValuta, _
                              Totale, TotaleLordoMerce, CDec(ScCassa), CLng(Val(Listino)), _
                              TipoDoc, CDec(Abbuono), IdDocumento, strErrore, ScCassaDett, 0, myTotDeduzioniCK) = False Then
            If strErrore <> "" Then
                strErrore = "Errore: in ChechTotDocCA.CalcolaTotaleDocCA - " & strErrore
            End If
            Exit Function
        End If
        myTotLordoMerceCK = TotaleLordoMerce 'giu290519
        Cont = 0

        If IsNothing(RegimeIVA) Then RegimeIVA = "0"
        If String.IsNullOrEmpty(RegimeIVA) Then RegimeIVA = "0"
        If Not IsNumeric(RegimeIVA) Then RegimeIVA = "0"
        '-
        If Val(RegimeIVA) > 49 Then
            IvaTrasporto = Val(RegimeIVA)
            IvaImballo = Val(RegimeIVA)
            IvaIncasso = Val(RegimeIVA)
        Else
            IvaTrasporto = App.GetParamGestAzi(_Esercizio).IVATrasporto
            IvaIncasso = App.GetParamGestAzi(_Esercizio).IvaSpese
            IvaImballo = App.GetParamGestAzi(_Esercizio).Iva_Imballo
        End If
        'GIU031013 SWIVA2122
        If SWCambioIVA = True Then
            IvaTrasporto = 21 'App.GetParamGestAzi(_Esercizio).IVATrasporto
            IvaIncasso = 21 ' App.GetParamGestAzi(_Esercizio).IvaSpese
            IvaImballo = 21 'App.GetParamGestAzi(_Esercizio).Iva_Imballo
        End If
        '-------------------

        'ASSEGNAZIONE TOTALE SPESE TRASPORTO
        If IsNothing(SpeseTrasporto) Then SpeseTrasporto = "0"
        If String.IsNullOrEmpty(SpeseTrasporto) Then SpeseTrasporto = "0"
        If Not IsNumeric(SpeseTrasporto) Then SpeseTrasporto = "0"
        '-
        If CDec(SpeseTrasporto) Then
            While ((IvaTrasporto <> Iva(Cont)) And _
                    Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    strErrore = "Errore: in ChechTotDocCA.CalcolaTotaleDocCA - " & MSGEccedIva
                    Exit Function
                End If
            End While
            Iva(Cont) = IvaTrasporto
            TmpImp = CDec(SpeseTrasporto)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If IvaTrasporto < 50 Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        'ASSEGNAZIONE AL TOTALE DELLE SPESE DI IMBALLO E ATTRIBUZIONE NEL COMPUTO DELL'IVA
        If IsNothing(SpeseImballo) Then SpeseImballo = "0"
        If String.IsNullOrEmpty(SpeseImballo) Then SpeseImballo = "0"
        If Not IsNumeric(SpeseImballo) Then SpeseImballo = "0"
        '-
        If (CDec(SpeseImballo)) > 0 Then
            While ((IvaImballo <> Iva(Cont)) And Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    strErrore = "Errore: in ChechTotDocCA.CalcolaTotaleDocCA - " & MSGEccedIva
                    Exit Function
                End If
            End While
            Iva(Cont) = IvaImballo
            TmpImp = CDec(SpeseImballo)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If IvaImballo < 50 Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        If IsNothing(SpeseIncasso) Then SpeseIncasso = "0"
        If String.IsNullOrEmpty(SpeseIncasso) Then SpeseIncasso = "0"
        If Not IsNumeric(SpeseIncasso) Then SpeseIncasso = "0"
        '-
        If (CDec(SpeseIncasso)) > 0 Then
            While ((IvaIncasso <> Iva(Cont)) And Not ((Iva(Cont) = 0) And (Imponibile(Cont) = 0)))
                Cont = Cont + 1
                If Cont = 4 Then
                    strErrore = "Errore: in ChechTotDocCA.CalcolaTotaleDocCA - " & MSGEccedIva
                    Exit Function
                End If
            End While
            Iva(Cont) = IvaIncasso
            TmpImp = CDec(SpeseIncasso)
            Imponibile(Cont) = Imponibile(Cont) + TmpImp
            If (Iva(Cont) > 0) And (Iva(Cont) < 50) Then
                Imposta(Cont) = Imposta(Cont) + Format(((TmpImp / 100) * Iva(Cont)), FormatValuta)
            End If
        End If

        ImponibileCheck = Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4)
        ImpostaCheck = Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4)
        TotaleCheck = Imponibile(0) + Imponibile(1) + Imponibile(2) + Imponibile(3) + Imponibile(4)
        TotaleCheck += Imposta(0) + Imposta(1) + Imposta(2) + Imposta(3) + Imposta(4)
        TotaleMCheck = TotaleLordoMerce 'giu050319

    End Function

    Public Function StampaAICA(ByVal IdDocumento As Long, ByVal TipoDoc As String, ByVal CodiceDitta As String, ByVal _Esercizio As String, ByVal TabCliFor As String, ByRef DsPrinWebDoc As DSPrintWeb_Documenti, ByRef ObjReport As Object, ByRef Errore As String) As Boolean
        'GIU01122011 ATTENZIONE SE ESTRAE 2 RKS DUPLICA I DETTAGLI DEGLI ARTICOLI
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim Passo As Integer = 0
        Dim SqlConnDoc As SqlConnection
        Dim SqlAdapDoc As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Try
            SqlConnDoc = New SqlConnection
            SqlAdapDoc = New SqlDataAdapter
            SqlDbSelectCmd = New SqlCommand

            SqlAdapDoc.SelectCommand = SqlDbSelectCmd
            SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectCmd.CommandText = "get_ArticoliInst_ContrattiAss"
            SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectCmd.Connection = SqlConnDoc
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDArticoloInst", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '==============CARICAMENTO DATASET ===================
            Passo = 1
            DsPrinWebDoc.ArticoliInstallatiST.Clear() 'GIU260618 OK SULLA TABELLA DI STAMPA
            SqlDbSelectCmd.Parameters.Item("@IDArticoloInst").Value = IdDocumento
            SqlAdapDoc.Fill(DsPrinWebDoc.ArticoliInstallatiST)
        Catch ex As Exception
            Errore = ex.Message & " - StampaAICA lettura testata e dettagli. Passo: " & Passo.ToString
            Return False
            Exit Function
        End Try
        Passo = 2
        Dim strErrore As String = ""
        Dim SWTabCliFor As String = ""
        Dim rsTestata As DataRow
        Try
            If (DsPrinWebDoc.Tables("ArticoliInstallati").Rows.Count > 0) Then
                For Each rsTestata In DsPrinWebDoc.Tables("ArticoliInstallati").Select("")
                    'per i campi NULL in stampa non vengono stampati tel. fax PI/CF
                    rsTestata.BeginEdit()
                    If IsDBNull(rsTestata!Destinazione1) Then rsTestata!Destinazione1 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione2 = ""
                    If IsDBNull(rsTestata!Destinazione3) Then rsTestata!Destinazione3 = ""
                    If IsDBNull(rsTestata!Cod_Articolo) Then rsTestata!Cod_Articolo = ""
                    If IsDBNull(rsTestata!LBase) Then rsTestata!LBase = 0
                    If IsDBNull(rsTestata!LOpz) Then rsTestata!LOpz = 0
                    If IsDBNull(rsTestata!Descrizione) Then rsTestata!Descrizione = ""
                    If IsDBNull(rsTestata!NSerie) Then rsTestata!NSerie = ""
                    If IsDBNull(rsTestata!Lotto) Then rsTestata!Lotto = ""
                    If IsDBNull(rsTestata!Attivo) Then rsTestata!Attivo = False
                    If IsDBNull(rsTestata!INRiparazione) Then rsTestata!InRiparazione = False
                    If IsDBNull(rsTestata!Sostituito) Then rsTestata!Sostituito = False
                    If IsDBNull(rsTestata!Riferimento) Then rsTestata!Riferimento = ""
                    If IsDBNull(rsTestata!Telefono1) Then rsTestata!Telefono1 = ""
                    If IsDBNull(rsTestata!Telefono2) Then rsTestata!Telefono2 = ""
                    If IsDBNull(rsTestata!Fax) Then rsTestata!Fax = ""
                    If IsDBNull(rsTestata!EMail) Then rsTestata!EMail = ""
                    If IsDBNull(rsTestata!Note) Then rsTestata!Note = ""
                    If IsDBNull(rsTestata!Numero) Then rsTestata!Numero = ""
                    If IsDBNull(rsTestata!Importo) Then rsTestata!Importo = 0
                    If IsDBNull(rsTestata!DesRefInt) Then rsTestata!DesRefInt = ""
                    rsTestata.EndEdit()
                    rsTestata.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - StampaAICA verifica testata e dettagli. Passo: " & Passo.ToString
            Return False
            Exit Function
        End Try
        '--------
        Passo = 3
        '--------
        Dim strSQL As String = ""
        strSQL = "Select * From Ditta WHERE Codice = '" & CodiceDitta.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, DsPrinWebDoc, "Ditta")
            ObjDB = Nothing
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Ditta"
            Return False
            Exit Function
        End Try
        '
        If Not IsDBNull(DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Coge")) Then
            If Left(DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Coge").ToString.Trim, 1) = "1" Then
                TabCliFor = "Cli"
            ElseIf Left(DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Coge").ToString.Trim, 1) = "9" Then
                TabCliFor = "For"
            End If
        Else
            TabCliFor = "Cli"
        End If
        '----------
        ObjDB = New DataBaseUtility
        If TabCliFor = "Cli" Then
            If Not IsDBNull(DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Coge")) Then
                strSQL = "Select * From Clienti WHERE Codice_CoGe = '" & DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Coge") & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti")
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Clienti"
                    Return False
                    Exit Function
                End Try
                ObjDB = Nothing
            Else
                'nulla
            End If
        ElseIf TabCliFor = "For" Then
            If Not IsDBNull(DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Coge")) Then
                strSQL = "Select * From Fornitori WHERE Codice_CoGe = '" & DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Coge") & "'"
                Try
                    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, DsPrinWebDoc, "Clienti")
                Catch ex As Exception
                    Errore = ex.Message & " - Tab. Fornitori"
                    Return False
                    Exit Function
                End Try
                ObjDB = Nothing
            Else
                'nulla
            End If
        End If
        'per i campi NULL in stampa non vengono stampati tel. fax PI/CF
        Dim rsTBCliFor As DataRow
        Try
            If (DsPrinWebDoc.Tables("Clienti").Rows.Count > 0) Then
                For Each rsTBCliFor In DsPrinWebDoc.Tables("Clienti").Select("")
                    rsTBCliFor.BeginEdit()
                    If IsDBNull(rsTBCliFor!Rag_Soc) Then rsTBCliFor!Rag_Soc = ""
                    If IsDBNull(rsTBCliFor!Indirizzo) Then rsTBCliFor!Indirizzo = ""
                    If IsDBNull(rsTBCliFor!Localita) Then rsTBCliFor!Localita = ""
                    If IsDBNull(rsTBCliFor!CAP) Then rsTBCliFor!CAP = ""
                    If IsDBNull(rsTBCliFor!Provincia) Then rsTBCliFor!Provincia = ""
                    If IsDBNull(rsTBCliFor!Nazione) Then rsTBCliFor!Nazione = ""
                    If IsDBNull(rsTBCliFor!Telefono1) Then rsTBCliFor!Telefono1 = ""
                    If IsDBNull(rsTBCliFor!Telefono2) Then rsTBCliFor!Telefono2 = ""
                    If IsDBNull(rsTBCliFor!Fax) Then rsTBCliFor!Fax = ""
                    If IsDBNull(rsTBCliFor!Codice_Fiscale) Then rsTBCliFor!Codice_Fiscale = ""
                    If IsDBNull(rsTBCliFor!Partita_IVA) Then rsTBCliFor!Partita_IVA = ""
                    If IsDBNull(rsTBCliFor!Denominazione) Then rsTBCliFor!Denominazione = ""
                    If IsDBNull(rsTBCliFor!Titolare) Then rsTBCliFor!Titolare = ""
                    If IsDBNull(rsTBCliFor!Email) Then rsTBCliFor!Email = ""
                    If IsDBNull(rsTBCliFor!ABI_N) Then rsTBCliFor!ABI_N = ""
                    If IsDBNull(rsTBCliFor!CAB_N) Then rsTBCliFor!CAB_N = ""
                    If IsDBNull(rsTBCliFor!Provincia_Estera) Then rsTBCliFor!Provincia_Estera = ""
                    If IsDBNull(rsTBCliFor!IndirizzoSenzaNumero) Then rsTBCliFor!IndirizzoSenzaNumero = ""
                    If IsDBNull(rsTBCliFor!NumeroCivico) Then rsTBCliFor!NumeroCivico = ""
                    If IsDBNull(rsTBCliFor!Note) Then rsTBCliFor!Note = ""
                    If IsDBNull(rsTBCliFor!IVASosp) Then rsTBCliFor!IVASosp = 0 'giu070212
                    'giu140620
                    If IsDBNull(rsTBCliFor!IBAN_Ditta) Then rsTBCliFor!IBAN_Ditta = ""
                    If IsDBNull(rsTBCliFor!EmailInvioScad) Then rsTBCliFor!EmailInvioScad = ""
                    If IsDBNull(rsTBCliFor!EmailInvioFatt) Then rsTBCliFor!EmailInvioFatt = ""
                    If IsDBNull(rsTBCliFor!PECEmail) Then rsTBCliFor!PECEmail = ""
                    rsTBCliFor.EndEdit()
                    rsTBCliFor.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Clienti/Fornitori. CAMPI NULL"
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        '-Destinazione Merce
        If Not IsDBNull(DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Filiale")) Then
            strSQL = "Select * From DestClienti WHERE Progressivo = " & DsPrinWebDoc.ArticoliInstallati.Rows(0).Item("Cod_Filiale") & ""
            ObjDB = New DataBaseUtility
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "DestClienti")
                ObjDB = Nothing
            Catch ex As Exception
                Errore = ex.Message & " - Tab. DestClienti"
                Return False
                Exit Function
            End Try
        End If
        'per i campi NULL in stampa non vengono stampati tel. fax PI/CF
        Dim rsTBDest As DataRow
        Try
            If (DsPrinWebDoc.Tables("DestClienti").Rows.Count > 0) Then
                For Each rsTBDest In DsPrinWebDoc.Tables("DestClienti").Select("")
                    rsTBDest.BeginEdit()
                    If IsDBNull(rsTBDest!Ragione_Sociale) Then rsTBDest!Ragione_Sociale = ""
                    If IsDBNull(rsTBDest!Indirizzo) Then rsTBDest!Indirizzo = ""
                    If IsDBNull(rsTBDest!Localita) Then rsTBDest!Localita = ""
                    If IsDBNull(rsTBDest!CAP) Then rsTBDest!CAP = ""
                    If IsDBNull(rsTBDest!Provincia) Then rsTBDest!Provincia = ""
                    If IsDBNull(rsTBDest!Stato) Then rsTBDest!Stato = ""
                    If IsDBNull(rsTBDest!Telefono1) Then rsTBDest!Telefono1 = ""
                    If IsDBNull(rsTBDest!Telefono2) Then rsTBDest!Telefono2 = ""
                    If IsDBNull(rsTBDest!Fax) Then rsTBDest!Fax = ""
                    If IsDBNull(rsTBDest!Denominazione) Then rsTBDest!Denominazione = ""
                    If IsDBNull(rsTBDest!Riferimento) Then rsTBDest!Riferimento = ""
                    If IsDBNull(rsTBDest!Email) Then rsTBDest!Email = ""
                    If IsDBNull(rsTBDest!Tipo) Then rsTBDest!Tipo = ""
                    rsTBDest.EndEdit()
                    rsTBDest.AcceptChanges()
                Next
            End If
        Catch ex As Exception
            Errore = ex.Message & " - Tab. Destinazione Merce CAMPI NULL"
            Return False
            Exit Function
        End Try
        '------------------------------------------------------------------------
        If Not IsDBNull(DsPrinWebDoc.Clienti.Rows(0).Item("Nazione")) Then
            strSQL = "Select * From Nazioni WHERE Codice = '" & DsPrinWebDoc.Clienti.Rows(0).Item("Nazione").ToString.Trim & "'"
            ObjDB = New DataBaseUtility
            Try
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, DsPrinWebDoc, "Nazioni")
                ObjDB = Nothing
            Catch ex As Exception
                Errore = ex.Message & " - Tab. Nazioni"
                Return False
                Exit Function
            End Try
        Else
            'strSQL = "Select * From Nazioni WHERE Codice = ''"
            DsPrinWebDoc.Nazioni.Clear()
        End If

        '==FINE CARICAMENTO ====================================
        Return True

    End Function
    'giu140514 giu200618 giu031219 esclusione clientiNoInvio Email
    Public Function BuildArtInstCliDett(ByVal AziendaReport As String, ByVal TitoloReport As String, ByVal TipoOrdineST As String,
                                         ByVal TipoOrdine As String, ByVal DataScadenzaDA As DateTime, ByVal DataScadenzaA As DateTime,
        ByVal SelScGa As Boolean, ByVal SelScEl As Boolean, ByVal SelScBa As Boolean, ByVal Codice_CoGe As String, ByVal Codice_Art As String,
        ByRef DsPrinWebDoc As DSPrintWeb_Documenti, ByRef ObjReport As Object, ByRef Errore As String,
        ByVal SelTutteCatCli As Boolean, ByVal SelRaggrCatCli As Boolean, ByVal DescCatCli As String, ByVal CodCategoria As Integer,
        ByVal SelCategorie As Boolean, ByVal CodCategSel As String,
        ByVal CliSenzaMail As Boolean, ByVal CliConMail As Boolean, ByVal CliNoInvioEmail As Boolean, ByVal CliConMailErr As Boolean,
        Optional ByVal EffettuaStampa As Boolean = False,
        Optional ByVal CaricaGrigliaDettaglio As Boolean = False, Optional ByVal CodCogeSelezionati As String = "") As Boolean

        Errore = ""
        'GIU200918 HA PRIORITA Codice_CoGe SU CodCogeSelezionati
        '' ''giu250618 o uno o l'altro giu270618 se invio un Codice_Coge e CodCogeSelezionati="" movo li il codice 
        ' ''If CodCogeSelezionati.Trim <> "" And Codice_CoGe.Trim <> "" Then
        ' ''    Errore = "Attenzione - Caricamento Tabella: Articoli installati " & CaricaGrigliaDettaglio.ToString.Trim & " If CodCogeSelezionati.Trim <> "" And Codice_CoGe.Trim <> "" Then - Avvisare Soft Solutions S.r.l."
        ' ''    Return False
        ' ''    Exit Function
        ' ''ElseIf Codice_CoGe.Trim <> "" And CodCogeSelezionati.Trim = "" And CaricaGrigliaDettaglio = True Then
        ' ''    Errore = "Attenzione - Caricamento Tabella: Articoli installati " & CaricaGrigliaDettaglio.ToString.Trim & " If Codice_CoGe.Trim <> "" And CodCogeSelezionati.Trim = "" And CaricaGrigliaDettaglio = True Then - Avvisare Soft Solutions S.r.l."
        ' ''    Return False
        ' ''    Exit Function
        ' ''ElseIf Codice_CoGe.Trim = "" And CodCogeSelezionati.Trim <> "" And CaricaGrigliaDettaglio = False Then
        ' ''    Errore = "Attenzione - Caricamento Tabella: Articoli installati " & CaricaGrigliaDettaglio.ToString.Trim & " If Codice_CoGe.Trim = "" And CodCogeSelezionati.Trim <> "" And CaricaGrigliaDettaglio = False Then - Avvisare Soft Solutions S.r.l."
        ' ''    Return False
        ' ''    Exit Function
        ' ''End If
        '----------
        If CodCogeSelezionati.Trim <> "" And Codice_CoGe.Trim <> "" Then
            CodCogeSelezionati = ""
        End If
        '----------
        '***************QUERY PER LA TESTATA******************
        Dim strSQLTestata As String = ""
        'RAGGRUPPAMENTO

        strSQLTestata = "SELECT ArticoliInst_ContrattiAss.Cod_Coge, ArticoliInst_ContrattiAss.Cod_Filiale, " &
         "DestClienti.Ragione_Sociale AS Destinazione1, DestClienti.Indirizzo AS Destinazione2, " &
         "CASE WHEN DestClienti.Provincia <>'' THEN DestClienti.Localita + ' (' + DestClienti.Provincia + ')' " &
         "ELSE DestClienti.Localita END AS Destinazione3, " &
         "Clienti.Rag_Soc, Clienti.Denominazione, Clienti.Partita_IVA, Clienti.Codice_Fiscale, " &
         "Clienti.Localita, Clienti.Provincia, Clienti.Cap, Clienti.Telefono1, Clienti.Telefono2, Clienti.Fax, '" &
         AziendaReport.Trim & "' AS AziendaReport, '" & TitoloReport.Trim & "' AS TitoloReport, '" &
         TipoOrdineST & "' AS TipoOrdinamento, " &
         "Clienti.Email AS EmailCliente, " &
         "Clienti.EmailInvioScad AS EmailCliInvioScad, DestClienti.Email AS EmailDest, "

        'Campo Email secondo priorità specificate (DestClienti -> EmailInvioScad -> EmailCliente)
        strSQLTestata &= "CASE WHEN LTRIM(RTRIM(ISNULL(DestClienti.Email, '')))='' THEN " &
                        "       CASE WHEN LTRIM(RTRIM(ISNULL(Clienti.EmailInvioScad, '')))='' THEN " &
                        "                 LTRIM(RTRIM(ISNULL(Clienti.Email, ''))) ELSE " &
                        "				  LTRIM(RTRIM(ISNULL(Clienti.EmailInvioScad, '')))" &
                        "       END" &
                        " ELSE LTRIM(RTRIM(ISNULL(DestClienti.Email, ''))) " &
                        " END AS EmailInvio "
        strSQLTestata &= ",Categorie.Descrizione AS DesCateg "
        strSQLTestata &= "FROM DestClienti RIGHT OUTER JOIN " &
        "ArticoliInst_ContrattiAss ON DestClienti.Codice = ArticoliInst_ContrattiAss.Cod_Coge AND " &
        "DestClienti.Progressivo = ArticoliInst_ContrattiAss.Cod_Filiale LEFT OUTER JOIN " &
        "Categorie RIGHT OUTER JOIN " &
        "Clienti ON Categorie.Codice = Clienti.Categoria ON ArticoliInst_ContrattiAss.Cod_Coge = Clienti.Codice_CoGe " &
        "WHERE Attivo<>0 AND ISNULL(Categorie.InvioMailSc,1) <> 0 "
        If SelScBa = True Or SelScEl = True Or SelScGa = True Then strSQLTestata += " AND ("
        Dim SWOR As Boolean = False
        'nessun test per i contratti al momento
        '" Data_Fine <= CONVERT(DATETIME, '" & Format(DataScadenza, FormatoData) & "', 103) OR " 
        If SelScGa = True Then
            strSQLTestata += " (DataScadGaranzia <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & " 23:59:59', 103) "
            strSQLTestata += "AND DataScadGaranzia >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
            strSQLTestata += "AND ISNULL(Data1InvioScadGa,0)=0) "
            SWOR = True
        End If
        If SelScEl = True Then
            If SWOR = True Then strSQLTestata += " OR "
            strSQLTestata += " (DataScadElettrodi <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & " 23:59:59', 103) "
            strSQLTestata += "AND DataScadElettrodi >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
            strSQLTestata += "AND ISNULL(Data1InvioScadEl,0)=0) "
            SWOR = True
        End If
        If SelScBa = True Then
            If SWOR = True Then strSQLTestata += " OR "
            strSQLTestata += " (DataScadBatterie <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & " 23:59:59', 103) "
            strSQLTestata += "AND DataScadBatterie >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
            strSQLTestata += "AND ISNULL(Data1InvioScadBa,0)=0) "
            SWOR = True
        End If
        '
        If SelScBa = True Or SelScEl = True Or SelScGa = True Then strSQLTestata += ")"

        If Codice_CoGe.Trim <> "" Then
            strSQLTestata += " AND ArticoliInst_ContrattiAss.Cod_Coge = '" & Codice_CoGe.Trim & "'"
        End If
        If Codice_Art.Trim <> "" Then
            strSQLTestata += " AND ArticoliInst_ContrattiAss.Cod_Articolo = '" & Codice_Art.Trim & "'"
        End If
        '-

        'se una mail è vuota o null
        If CliSenzaMail = True Then
            strSQLTestata += " AND ("
            strSQLTestata += " LTRIM(RTRIM(ISNULL(Clienti.Email, '')))=''"
            strSQLTestata += " AND LTRIM(RTRIM(ISNULL(Clienti.EmailInvioScad, '')))=''"
            strSQLTestata += " AND LTRIM(RTRIM(ISNULL(DestClienti.Email, '')))=''"
            strSQLTestata += ")"
        ElseIf CliConMail = True Or CliConMailErr = True Then 'giu261018 saranno poi filtrati nel DS EmailErr=TRUE valorizzato da codice
            strSQLTestata += " AND NOT ("
            strSQLTestata += " LTRIM(RTRIM(ISNULL(Clienti.Email, '')))=''"
            strSQLTestata += " AND LTRIM(RTRIM(ISNULL(Clienti.EmailInvioScad, '')))=''"
            strSQLTestata += " AND LTRIM(RTRIM(ISNULL(DestClienti.Email, '')))=''"
            strSQLTestata += ")"
        End If 'giu031219 per escludere o no i clienti che non hanno accettao l'invio email
        If CliNoInvioEmail = True Then
            strSQLTestata += " AND (ISNULL(Clienti.InvioMailScad,0)=0)"
        Else
            strSQLTestata += " AND (ISNULL(Clienti.InvioMailScad,0)<>0)"
        End If

        'selezione categoria
        If SelTutteCatCli = False And SelCategorie = False Then
            If SelRaggrCatCli = True Then
                strSQLTestata += " AND (Categorie.Descrizione like '" & DescCatCli & "%')"
            Else
                strSQLTestata += " AND (ISNULL(Categorie.Codice,0) =" & CodCategoria & ")"
            End If
        ElseIf SelCategorie = True And CodCategSel.Trim <> "" Then
            Dim arrCodCategoria As String() = CodCategSel.Split(";")
            strSQLTestata += " AND Categorie.Codice IN ("
            For i = 0 To arrCodCategoria.Count - 1
                If arrCodCategoria(i).ToString <> "" Then
                    strSQLTestata += arrCodCategoria(i).ToString & ","
                End If
            Next
            strSQLTestata = strSQLTestata.Substring(0, strSQLTestata.Length - 1) 'rimuovo ultima virgola
            strSQLTestata += ") "
        End If
        '---

        'se devo popolare la griglia clienti senza e-mail
        strSQLTestata += " GROUP BY ArticoliInst_ContrattiAss.Cod_Coge, ArticoliInst_ContrattiAss.Cod_Filiale, " &
         "Clienti.Rag_Soc, Clienti.Denominazione, Clienti.Partita_IVA, Clienti.Codice_Fiscale, " &
         "Clienti.Localita, Clienti.Provincia, Clienti.Cap, Clienti.Telefono1, Clienti.Telefono2, Clienti.Fax, Clienti.Email, " &
         "DestClienti.Ragione_Sociale , DestClienti.Indirizzo, DestClienti.Localita, DestClienti.Provincia, " &
         "Clienti.EmailInvioScad, DestClienti.Email, Categorie.Descrizione "

        If TipoOrdine.Trim <> "" Then
            strSQLTestata += " ORDER BY " & TipoOrdine.Trim
        End If
        '***************FINE TESTATA*******************


        '***************QUERY PER IL DETTAGLIO*******************
        Dim strSQLDettaglio As String = ""
        'DETTAGLIO
        'GIU290519 TOLTO DISTINCT
        strSQLDettaglio = "SELECT ArticoliInst_ContrattiAss.ID,ArticoliInst_ContrattiAss.Tipo_Doc,ArticoliInst_ContrattiAss.Data_Installazione,ArticoliInst_ContrattiAss.Cod_Coge,ArticoliInst_ContrattiAss.Riferimento,CONVERT(NVARCHAR(150),ArticoliInst_ContrattiAss.NoteDocumento) AS NoteDocumento " &
        ",ArticoliInst_ContrattiAss.NsRiferimento,CONVERT(NVARCHAR(150),ArticoliInst_ContrattiAss.Note) AS Note,ArticoliInst_ContrattiAss.Cod_Filiale,ArticoliInst_ContrattiAss.Destinazione1,ArticoliInst_ContrattiAss.Destinazione2,ArticoliInst_ContrattiAss.Destinazione3 " &
        ",ArticoliInst_ContrattiAss.Cod_Articolo,ArticoliInst_ContrattiAss.LBase,ArticoliInst_ContrattiAss.LOpz,ArticoliInst_ContrattiAss.Descrizione,ArticoliInst_ContrattiAss.DataScadGaranzia,ArticoliInst_ContrattiAss.DataScadElettrodi,ArticoliInst_ContrattiAss.DataScadBatterie,ArticoliInst_ContrattiAss.Data1InvioScadGa " &
        ",ArticoliInst_ContrattiAss.Data1InvioScadEl,ArticoliInst_ContrattiAss.Data1InvioScadBa,ArticoliInst_ContrattiAss.NSerie,ArticoliInst_ContrattiAss.Lotto,ArticoliInst_ContrattiAss.Attivo,ArticoliInst_ContrattiAss.InRiparazione,ArticoliInst_ContrattiAss.Sostituito,ArticoliInst_ContrattiAss.DataSostituzione " &
        ",ArticoliInst_ContrattiAss.IDTipoContratto,ArticoliInst_ContrattiAss.Numero,ArticoliInst_ContrattiAss.Importo,ArticoliInst_ContrattiAss.Data_Inizio,ArticoliInst_ContrattiAss.Data_Fine,ArticoliInst_ContrattiAss.RefInt,CONVERT(NVARCHAR(150),ArticoliInst_ContrattiAss.DesRefInt) AS DesRefInt " &
        ",ArticoliInst_ContrattiAss.Reparto,ArticoliInst_ContrattiAss.IDDocDTMM,ArticoliInst_ContrattiAss.Tipo_DocDTMM,ArticoliInst_ContrattiAss.Data_DocDTMM,ArticoliInst_ContrattiAss.RigaDTMM,ArticoliInst_ContrattiAss.NColloDTMM,ArticoliInst_ContrattiAss.QtaColliDTMM,ArticoliInst_ContrattiAss.SfusiDTMM,ArticoliInst_ContrattiAss.Operatore " &
        ",ArticoliInst_ContrattiAss.NomePC,ArticoliInst_ContrattiAss.BloccatoDalPC,ArticoliInst_ContrattiAss.InseritoDa,ArticoliInst_ContrattiAss.ModificatoDa,ArticoliInst_ContrattiAss.Data2InvioScadGa,ArticoliInst_ContrattiAss.Data2InvioScadEl,ArticoliInst_ContrattiAss.Data2InvioScadBa,ArticoliInst_ContrattiAss.NReInvio, " &
        "Clienti.Rag_Soc, Clienti.Denominazione, Clienti.Partita_IVA, Clienti.Codice_Fiscale, " &
        "Clienti.Localita, Clienti.Provincia, Clienti.Cap, Clienti.Telefono1, Clienti.Telefono2, Clienti.Fax, Clienti.Email AS EmailCliente, '" &
        AziendaReport.Trim & "' AS AziendaReport, '" & TitoloReport.Trim & "' AS TitoloReport, '" &
        TipoOrdineST & "' AS TipoOrdinamento, " &
        "CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & "', 103) AS DataScadenza, " &
        "LTRIM(RTRIM(ISNULL(ArticoliInst_ContrattiAss.NsRiferimento,''))) + ' ' + LTRIM(RTRIM(ISNULL(ArticoliInst_ContrattiAss.Riferimento,''))) " &
        "AS RiferimentiRic, Clienti.EmailInvioScad AS EmailCliInvioScad, DestClienti.Email AS EmailDest, "
        'Campo Email secondo priorità specificate (DestClienti -> EmailInvioScad -> EmailCliente)
        strSQLDettaglio &= "CASE WHEN LTRIM(RTRIM(ISNULL(DestClienti.Email, '')))='' THEN " &
                        "       CASE WHEN LTRIM(RTRIM(ISNULL(Clienti.EmailInvioScad, '')))='' THEN " &
                        "                 LTRIM(RTRIM(ISNULL(Clienti.Email, ''))) ELSE " &
                        "				  LTRIM(RTRIM(ISNULL(Clienti.EmailInvioScad, '')))" &
                        "       END" &
                        " ELSE LTRIM(RTRIM(ISNULL(DestClienti.Email, ''))) " &
                        " END AS EmailInvio, 0 AS NReInvio, NULL AS DataInvio "
        strSQLDettaglio &= ",Categorie.Descrizione AS DesCateg "
        strSQLDettaglio &= "FROM DestClienti RIGHT OUTER JOIN " &
        "ArticoliInst_ContrattiAss ON DestClienti.Codice = ArticoliInst_ContrattiAss.Cod_Coge AND " &
        "DestClienti.Progressivo = ArticoliInst_ContrattiAss.Cod_Filiale LEFT OUTER JOIN " &
        "Categorie RIGHT OUTER JOIN " &
        "Clienti ON Categorie.Codice = Clienti.Categoria ON ArticoliInst_ContrattiAss.Cod_Coge = Clienti.Codice_CoGe " &
        "WHERE Attivo<>0 AND ISNULL(Categorie.InvioMailSc,1) <> 0 "

        If SelScBa = True Or SelScEl = True Or SelScGa = True Then strSQLDettaglio += " AND ("
        Dim SWORDett As Boolean = False
        'nessun test per i contratti al momento
        '" Data_Fine <= CONVERT(DATETIME, '" & Format(DataScadenza, FormatoData) & "', 103) OR " 
        If SelScGa = True Then
            strSQLDettaglio += " (DataScadGaranzia <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & " 23:59:59', 103) "
            strSQLDettaglio += "AND DataScadGaranzia >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
            strSQLDettaglio += "AND ISNULL(Data1InvioScadGa,0)=0) "
            SWORDett = True
        End If
        If SelScEl = True Then
            If SWORDett = True Then strSQLDettaglio += " OR "
            strSQLDettaglio += " (DataScadElettrodi <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & " 23:59:59', 103) "
            strSQLDettaglio += "AND DataScadElettrodi >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
            strSQLDettaglio += "AND ISNULL(Data1InvioScadEl,0)=0) "
            SWORDett = True
        End If
        If SelScBa = True Then
            If SWORDett = True Then strSQLDettaglio += " OR "
            strSQLDettaglio += " (DataScadBatterie <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & " 23:59:59', 103) "
            strSQLDettaglio += "AND DataScadBatterie >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
            strSQLDettaglio += "AND ISNULL(Data1InvioScadBa,0)=0) "
            SWORDett = True
        End If
        '
        If SelScBa = True Or SelScEl = True Or SelScGa = True Then strSQLDettaglio += ")"
        If Codice_CoGe.Trim <> "" Then
            strSQLDettaglio += " AND ArticoliInst_ContrattiAss.Cod_Coge = '" & Codice_CoGe.Trim & "'"
        End If
        If Codice_Art.Trim <> "" Then
            strSQLDettaglio += " AND ArticoliInst_ContrattiAss.Cod_Articolo = '" & Codice_Art.Trim & "'"
        End If
        '-
        'se una mail è vuota o null
        If CliSenzaMail = True Then
            strSQLDettaglio += " AND ("
            strSQLDettaglio += " LTRIM(RTRIM(ISNULL(Clienti.Email, '')))=''"
            strSQLDettaglio += " AND LTRIM(RTRIM(ISNULL(Clienti.EmailInvioScad, '')))=''"
            strSQLDettaglio += " AND LTRIM(RTRIM(ISNULL(DestClienti.Email, '')))=''"
            strSQLDettaglio += ")"
        ElseIf CliConMail = True Or CliConMailErr Then 'giu261018 saranno poi filtrati nel DS EmailErr=TRUE valorizzato da codice
            strSQLDettaglio += " AND NOT ("
            strSQLDettaglio += " LTRIM(RTRIM(ISNULL(Clienti.Email, '')))=''"
            strSQLDettaglio += " AND LTRIM(RTRIM(ISNULL(Clienti.EmailInvioScad, '')))=''"
            strSQLDettaglio += " AND LTRIM(RTRIM(ISNULL(DestClienti.Email, '')))=''"
            strSQLDettaglio += ")"
        End If 'giu031219 per escludere o no i clienti che non hanno accettao l'invio email
        If CliNoInvioEmail = True Then
            strSQLDettaglio += " AND (ISNULL(Clienti.InvioMailScad,0)=0)"
        Else
            strSQLDettaglio += " AND (ISNULL(Clienti.InvioMailScad,0)<>0)"
        End If

        'selezione categoria
        If SelTutteCatCli = False And SelCategorie = False Then
            If SelRaggrCatCli = True Then
                strSQLDettaglio += " AND (Categorie.Descrizione like '" & DescCatCli & "%')"
            Else
                strSQLDettaglio += " AND (ISNULL(Categorie.Codice,0) =" & CodCategoria & ")"
            End If
        ElseIf SelCategorie = True And CodCategSel.Trim <> "" Then
            Dim arrCodCategoria As String() = CodCategSel.Split(";")
            strSQLDettaglio += " AND Categorie.Codice IN ("
            For i = 0 To arrCodCategoria.Count - 1
                If arrCodCategoria(i).ToString <> "" Then
                    strSQLDettaglio += arrCodCategoria(i).ToString & ","
                End If
            Next
            strSQLDettaglio = strSQLDettaglio.Substring(0, strSQLDettaglio.Length - 1) 'rimuovo ultima virgola
            strSQLDettaglio += ") "
        End If

        'se sono in visualizzazione dettaglio passo cod_coge selezionati da visualizzare
        If CodCogeSelezionati.Trim <> "" Then
            Dim arrCodCoge As String() = CodCogeSelezionati.Split(";")
            strSQLDettaglio += " AND Cod_Coge IN ("
            For i = 0 To arrCodCoge.Count - 1
                If arrCodCoge(i).ToString <> "" Then
                    strSQLDettaglio += "'" & arrCodCoge(i).ToString & "',"
                End If
            Next
            strSQLDettaglio = strSQLDettaglio.Substring(0, strSQLDettaglio.Length - 1) 'rimuovo ultima virgola
            strSQLDettaglio += ") "
        End If
        '---

        If TipoOrdine.Trim <> "" Then
            strSQLDettaglio += " ORDER BY " & TipoOrdine.Trim
        End If
        '***************FINE DETTAGLIO*******************
        Dim ObjDB As New DataBaseUtility
        If strSQLTestata <> "" And Not CaricaGrigliaDettaglio Then
            Try
                DsPrinWebDoc.ArticoliInstEmail.Clear()
                DsPrinWebDoc.ArticoliInstallati.Clear()
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQLTestata, DsPrinWebDoc, "ArticoliInstEmail")
                'giu261018
                If CliConMailErr = True Then
                    If CKEmail(DsPrinWebDoc, "T", "E", Errore) = False Then
                        ObjDB = Nothing
                        Return False
                        Exit Function
                    End If
                ElseIf CliConMail = True Then 'GIU020119
                    If CKEmail(DsPrinWebDoc, "T", "C", Errore) = False Then
                        ObjDB = Nothing
                        Return False
                        Exit Function
                    End If
                End If
                '----------
            Catch ex As Exception
                ObjDB = Nothing
                Errore = ex.Message & " - Caricamento Tabella: ArticoliInstEmail"
                Return False
                Exit Function
            End Try
        End If
        '-
        If strSQLDettaglio <> "" And CaricaGrigliaDettaglio Then
            Try
                DsPrinWebDoc.ArticoliInstallati.Clear()
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQLDettaglio, DsPrinWebDoc, "ArticoliInstallati")
                'giu261018
                If CliConMailErr = True Then
                    If CKEmail(DsPrinWebDoc, "D", "E", Errore) = False Then
                        ObjDB = Nothing
                        Return False
                        Exit Function
                    End If
                ElseIf CliConMail = True Then 'GIU020119
                    If CKEmail(DsPrinWebDoc, "T", "C", Errore) = False Then
                        ObjDB = Nothing
                        Return False
                        Exit Function
                    End If
                End If
                '----------
            Catch ex As Exception
                ObjDB = Nothing
                Errore = ex.Message & " - Caricamento Tabella: ArticoliInstallati"
                Return False
                Exit Function
            End Try
        End If
        ObjDB = Nothing
        Return True
    End Function
    'giu261018
    Private Function CKEmail(ByRef myDs As DSPrintWeb_Documenti, ByVal myTipo As String, ByVal myCliConMail As String, ByRef _StrErrore As String) As Boolean
        Try
            CKEmail = True
            If myTipo = "T" Then
                Dim rowAIEmail As DSPrintWeb_Documenti.ArticoliInstEmailRow
                For Each rowAIEmail In myDs.ArticoliInstEmail.Select("", "EmailInvio")
                    If Not rowAIEmail.IsEmailInvioNull Then
                        If rowAIEmail.EmailInvio.Trim <> "" Then
                            If ConvalidaEmail(rowAIEmail.EmailInvio.Trim) = False Then
                                rowAIEmail.EMailErrata = True
                            Else
                                rowAIEmail.EmailInvio = rowAIEmail.EmailInvio.Trim.ToLower 'GIU250219
                            End If
                        Else
                            rowAIEmail.EMailErrata = True
                        End If
                    Else
                        rowAIEmail.EMailErrata = True
                    End If
                Next
                If myCliConMail = "E" Then 'giu020119
                    For Each rowAIEmail In myDs.ArticoliInstEmail.Select("EMailErrata = False", "EmailInvio")
                        rowAIEmail.Delete()
                    Next
                Else
                    For Each rowAIEmail In myDs.ArticoliInstEmail.Select("EMailErrata = True", "EmailInvio")
                        rowAIEmail.Delete()
                    Next
                End If

                myDs.AcceptChanges()
            Else
                Dim rowAIEmailD As DSPrintWeb_Documenti.ArticoliInstallatiRow
                For Each rowAIEmailD In myDs.ArticoliInstallati.Select("", "EmailInvio")
                    If Not rowAIEmailD.IsEmailInvioNull Then
                        If rowAIEmailD.EmailInvio.Trim <> "" Then
                            If ConvalidaEmail(rowAIEmailD.EmailInvio.Trim) = False Then
                                rowAIEmailD.EMailErrata = True
                            Else
                                rowAIEmailD.EmailInvio = rowAIEmailD.EmailInvio.Trim.ToLower 'GIU250219
                            End If
                        Else
                            rowAIEmailD.EMailErrata = True
                        End If
                    Else
                        rowAIEmailD.EMailErrata = True
                    End If
                Next
                If myCliConMail = "E" Then 'giu020119
                    For Each rowAIEmailD In myDs.ArticoliInstallati.Select("EMailErrata = False", "EmailInvio")
                        rowAIEmailD.Delete()
                    Next
                Else
                    For Each rowAIEmailD In myDs.ArticoliInstallati.Select("EMailErrata = True", "EmailInvio")
                        rowAIEmailD.Delete()
                    Next
                End If

                myDs.AcceptChanges()
            End If
        Catch ex As Exception
            CKEmail = False
            _StrErrore = ex.Message.Trim
        End Try
    End Function
    'giu130718
    Public Function BuildEmailCliDett(ByVal AziendaReport As String, ByVal TitoloReport As String, ByVal TipoOrdineST As String,
                                         ByVal TipoOrdine As String, ByVal DataEmailDA As DateTime, ByVal DataEmailA As DateTime,
        ByVal SelScGa As Boolean, ByVal SelScEl As Boolean, ByVal SelScBa As Boolean, ByVal Codice_CoGe As String, ByVal StatoEmail As Integer,
        ByRef DsPrinWebDoc As DSPrintWeb_Documenti, ByRef ObjReport As Object, ByRef Errore As String,
        ByVal SelTutteCatCli As Boolean, ByVal SelRaggrCatCli As Boolean, ByVal DescCatCli As String, ByVal CodCategoria As Integer,
        ByVal SelCategorie As Boolean, ByVal CodCategSel As String,
        ByVal DalNumero As Integer, ByVal AlNumero As Integer, ByVal SelDalNAlN As String,
        Optional ByVal EffettuaStampa As Boolean = False,
        Optional ByVal CaricaGrigliaDettaglio As Boolean = False, Optional ByVal CodCogeSelezionati As String = "") As Boolean

        Errore = ""
        'GIU200918 HA PRIORITA Codice_CoGe SU CodCogeSelezionati
        '' ''giu250618 o uno o l'altro giu270618 se invio un Codice_Coge e CodCogeSelezionati="" movo li il codice 
        ' ''If CodCogeSelezionati.Trim <> "" And Codice_CoGe.Trim <> "" Then
        ' ''    Errore = "Attenzione - Caricamento Tabella: Articoli installati " & CaricaGrigliaDettaglio.ToString.Trim & " If CodCogeSelezionati.Trim <> "" And Codice_CoGe.Trim <> "" Then - Avvisare Soft Solutions S.r.l."
        ' ''    Return False
        ' ''    Exit Function
        ' ''ElseIf Codice_CoGe.Trim <> "" And CodCogeSelezionati.Trim = "" And CaricaGrigliaDettaglio = True Then
        ' ''    Errore = "Attenzione - Caricamento Tabella: Articoli installati " & CaricaGrigliaDettaglio.ToString.Trim & " If Codice_CoGe.Trim <> "" And CodCogeSelezionati.Trim = "" And CaricaGrigliaDettaglio = True Then - Avvisare Soft Solutions S.r.l."
        ' ''    Return False
        ' ''    Exit Function
        ' ''ElseIf Codice_CoGe.Trim = "" And CodCogeSelezionati.Trim <> "" And CaricaGrigliaDettaglio = False Then
        ' ''    Errore = "Attenzione - Caricamento Tabella: Articoli installati " & CaricaGrigliaDettaglio.ToString.Trim & " If Codice_CoGe.Trim = "" And CodCogeSelezionati.Trim <> "" And CaricaGrigliaDettaglio = False Then - Avvisare Soft Solutions S.r.l."
        ' ''    Return False
        ' ''    Exit Function
        ' ''End If
        If CodCogeSelezionati.Trim <> "" And Codice_CoGe.Trim <> "" Then
            CodCogeSelezionati = ""
        End If
        '----------
        '***************QUERY PER LA TESTATA******************
        Dim strSQLTestata As String = ""
        'RAGGRUPPAMENTO

        strSQLTestata = "SELECT IdEmailInviateT, EmailInviateT.DataInvio, EmailInviateT.Anno, EmailInviateT.Numero AS NEmail, EmailInviateT.Stato, "
        strSQLTestata &= "CASE EmailInviateT.Stato " &
             "WHEN -1 THEN 'Invio Errato' " &
             "WHEN 0 THEN 'Da inviare' " &
             "WHEN 1 THEN 'Inviata' " &
             "WHEN 2 THEN 'Sollecito inviato' " &
             "WHEN -2 THEN 'Invio in corso Sollecito' " &
             "WHEN 3 THEN 'Parz.Conclusa' " &
             "WHEN -3 THEN 'Invio in corso Parz.Conclusa' " &
             "WHEN 9 THEN 'Annullata' " &
             "WHEN 99 THEN 'Conclusa' " &
             "WHEN -99 THEN 'Invio in corso Conclusa' " &
             "ELSE '' END AS DesStato, "
        strSQLTestata &= "ArticoliInst_ContrattiAss.Cod_Coge, ArticoliInst_ContrattiAss.Cod_Filiale, " &
        "DestClienti.Ragione_Sociale AS Destinazione1, DestClienti.Indirizzo AS Destinazione2, " &
        "CASE WHEN DestClienti.Provincia <>'' THEN DestClienti.Localita + ' (' + DestClienti.Provincia + ')' " &
        "ELSE DestClienti.Localita END AS Destinazione3, " &
        "Clienti.Rag_Soc, Clienti.Denominazione, Clienti.Partita_IVA, Clienti.Codice_Fiscale, " &
        "Clienti.Localita, Clienti.Provincia, Clienti.Cap, Clienti.Telefono1, Clienti.Telefono2, Clienti.Fax, '" &
        AziendaReport.Trim & "' AS AziendaReport, '" & TitoloReport.Trim & "' AS TitoloReport, '" &
        TipoOrdineST & "' AS TipoOrdinamento, " &
        "Clienti.Email AS EmailCliente, " &
        "Clienti.EmailInvioScad AS EmailCliInvioScad, DestClienti.Email AS EmailDest, EmailInviateT.Email AS EmailInvio "
        strSQLTestata &= "FROM DestClienti RIGHT OUTER JOIN " &
        "ArticoliInst_ContrattiAss ON DestClienti.Codice = ArticoliInst_ContrattiAss.Cod_Coge AND " &
        "DestClienti.Progressivo = ArticoliInst_ContrattiAss.Cod_Filiale LEFT OUTER JOIN " &
        "Categorie RIGHT OUTER JOIN " &
        "Clienti ON Categorie.Codice = Clienti.Categoria ON ArticoliInst_ContrattiAss.Cod_Coge = Clienti.Codice_CoGe INNER JOIN " &
        "EmailInviateDett ON ArticoliInst_ContrattiAss.ID = EmailInviateDett.IdArticoliInst_ContrattiAss INNER JOIN " &
        "EmailInviateT ON EmailInviateT.ID = EmailInviateDett.IdEmailInviateT "
        If StatoEmail = 999 Then
            strSQLTestata &= "WHERE "
        Else
            strSQLTestata &= "WHERE ISNULL(EmailInviateT.Stato,0) = " & StatoEmail.ToString.Trim & " AND "
        End If
        strSQLTestata &= "(DataInvio >= CONVERT(DATETIME, '" & Format(DataEmailDA, FormatoData) & "', 103) AND " &
               "DataInvio <= CONVERT(DATETIME, '" & Format(DataEmailA, FormatoData) & " 23:59:59', 103) ) AND " &
               "(EmailInviateT.Numero >= " & DalNumero.ToString.Trim & " AND " &
               "EmailInviateT.Numero <= " & AlNumero.ToString.Trim & ")"

        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@ 
        ' ''If SelScBa = True Or SelScEl = True Or SelScGa = True Then strSQLTestata += " AND ("
        ' ''Dim SWOR As Boolean = False
        '' ''nessun test per i contratti al momento
        ' ''If SelScGa = True Then
        ' ''    strSQLTestata += " (DataScadGaranzia <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & "', 103) "
        ' ''    strSQLTestata += "AND DataScadGaranzia >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
        ' ''    strSQLTestata += "AND ISNULL(Data1InvioScadGa,0)=0) "
        ' ''    SWOR = True
        ' ''End If
        ' ''If SelScEl = True Then
        ' ''    If SWOR = True Then strSQLTestata += " OR "
        ' ''    strSQLTestata += " (DataScadElettrodi <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & "', 103) "
        ' ''    strSQLTestata += "AND DataScadElettrodi >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
        ' ''    strSQLTestata += "AND ISNULL(Data1InvioScadEl,0)=0) "
        ' ''    SWOR = True
        ' ''End If
        ' ''If SelScBa = True Then
        ' ''    If SWOR = True Then strSQLTestata += " OR "
        ' ''    strSQLTestata += " (DataScadBatterie <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & "', 103) "
        ' ''    strSQLTestata += "AND DataScadBatterie >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
        ' ''    strSQLTestata += "AND ISNULL(Data1InvioScadBa,0)=0) "
        ' ''    SWOR = True
        ' ''End If
        '' ''
        ' ''If SelScBa = True Or SelScEl = True Or SelScGa = True Then strSQLTestata += ")"

        If Codice_CoGe.Trim <> "" Then
            strSQLTestata += " AND ArticoliInst_ContrattiAss.Cod_Coge = '" & Codice_CoGe.Trim & "'"
        End If
        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@ 
        'selezione categoria
        ' ''If SelTutteCatCli = False And SelCategorie = False Then
        ' ''    If SelRaggrCatCli = True Then
        ' ''        strSQLTestata += " AND (Categorie.Descrizione like '" & DescCatCli & "%')"
        ' ''    Else
        ' ''        strSQLTestata += " AND (ISNULL(Categorie.Codice,0) =" & CodCategoria & ")"
        ' ''    End If
        ' ''ElseIf SelCategorie = True And CodCategSel.Trim <> "" Then
        ' ''    Dim arrCodCategoria As String() = CodCategSel.Split(";")
        ' ''    strSQLTestata += " AND Categorie.Codice IN ("
        ' ''    For i = 0 To arrCodCategoria.Count - 1
        ' ''        If arrCodCategoria(i).ToString <> "" Then
        ' ''            strSQLTestata += arrCodCategoria(i).ToString & ","
        ' ''        End If
        ' ''    Next
        ' ''    strSQLTestata = strSQLTestata.Substring(0, strSQLTestata.Length - 1) 'rimuovo ultima virgola
        ' ''    strSQLTestata += ") "
        ' ''End If
        '---

        strSQLTestata += " GROUP BY IdEmailInviateT, EmailInviateT.DataInvio, EmailInviateT.Anno, EmailInviateT.Numero, EmailInviateT.Stato, "
        strSQLTestata &= "CASE EmailInviateT.Stato " &
             "WHEN -1 THEN 'Invio Errato' " &
             "WHEN 0 THEN 'Da inviare' " &
             "WHEN 1 THEN 'Inviata' " &
             "WHEN 2 THEN 'Sollecito inviato' " &
             "WHEN -2 THEN 'Invio in corso Sollecito' " &
             "WHEN 3 THEN 'Parz.Conclusa' " &
             "WHEN -3 THEN 'Invio in corso Parz.Conclusa' " &
             "WHEN 9 THEN 'Annullata' " &
             "WHEN 99 THEN 'Conclusa' " &
             "WHEN -99 THEN 'Invio in corso Conclusa' " &
             "ELSE '' END, " &
         "ArticoliInst_ContrattiAss.Cod_Coge, ArticoliInst_ContrattiAss.Cod_Filiale, " &
         "Clienti.Rag_Soc, Clienti.Denominazione, Clienti.Partita_IVA, Clienti.Codice_Fiscale, " &
         "Clienti.Localita, Clienti.Provincia, Clienti.Cap, Clienti.Telefono1, Clienti.Telefono2, Clienti.Fax, Clienti.Email, " &
         "DestClienti.Ragione_Sociale , DestClienti.Indirizzo, DestClienti.Localita, DestClienti.Provincia, " &
         "Clienti.EmailInvioScad, DestClienti.Email, EmailInviateT.Email"

        If TipoOrdine.Trim <> "" Then
            strSQLTestata += " ORDER BY " & TipoOrdine.Trim
        End If
        '***************FINE TESTATA*******************


        '***************QUERY PER IL DETTAGLIO*******************
        Dim strSQLDettaglio As String = ""
        'DETTAGLIO
        'GIU290519 TOLTO IL DISTINCT 
        strSQLDettaglio = "SELECT IdEmailInviateT, EmailInviateT.DataInvio, EmailInviateT.Anno, EmailInviateT.Numero AS NEmail, EmailInviateT.Stato, "
        strSQLDettaglio &= "CASE EmailInviateT.Stato " &
             "WHEN -1 THEN 'Invio Errato' " &
             "WHEN 0 THEN 'Da inviare' " &
             "WHEN 1 THEN 'Inviata' " &
             "WHEN 2 THEN 'Sollecito inviato' " &
             "WHEN -2 THEN 'Invio in corso Sollecito' " &
             "WHEN 3 THEN 'Parz.Conclusa' " &
             "WHEN -3 THEN 'Invio in corso Parz.Conclusa' " &
             "WHEN 9 THEN 'Annullata' " &
             "WHEN 99 THEN 'Conclusa' " &
             "WHEN -99 THEN 'Invio in corso Conclusa' " &
             "ELSE '' END AS DesStato, ArticoliInst_ContrattiAss.ID,ArticoliInst_ContrattiAss.Tipo_Doc,ArticoliInst_ContrattiAss.Data_Installazione,ArticoliInst_ContrattiAss.Cod_Coge,ArticoliInst_ContrattiAss.Riferimento,CONVERT(NVARCHAR(150),ArticoliInst_ContrattiAss.NoteDocumento) AS NoteDocumento " &
        ",ArticoliInst_ContrattiAss.NsRiferimento,CONVERT(NVARCHAR(150),ArticoliInst_ContrattiAss.Note) AS Note,ArticoliInst_ContrattiAss.Cod_Filiale,ArticoliInst_ContrattiAss.Destinazione1,ArticoliInst_ContrattiAss.Destinazione2,ArticoliInst_ContrattiAss.Destinazione3 " &
        ",ArticoliInst_ContrattiAss.Cod_Articolo,ArticoliInst_ContrattiAss.LBase,ArticoliInst_ContrattiAss.LOpz,ArticoliInst_ContrattiAss.Descrizione,ArticoliInst_ContrattiAss.DataScadGaranzia,ArticoliInst_ContrattiAss.DataScadElettrodi,ArticoliInst_ContrattiAss.DataScadBatterie,ArticoliInst_ContrattiAss.Data1InvioScadGa " &
        ",ArticoliInst_ContrattiAss.Data1InvioScadEl,ArticoliInst_ContrattiAss.Data1InvioScadBa,ArticoliInst_ContrattiAss.NSerie,ArticoliInst_ContrattiAss.Lotto,ArticoliInst_ContrattiAss.Attivo,ArticoliInst_ContrattiAss.InRiparazione,ArticoliInst_ContrattiAss.Sostituito,ArticoliInst_ContrattiAss.DataSostituzione " &
        ",ArticoliInst_ContrattiAss.IDTipoContratto,ArticoliInst_ContrattiAss.Numero,ArticoliInst_ContrattiAss.Importo,ArticoliInst_ContrattiAss.Data_Inizio,ArticoliInst_ContrattiAss.Data_Fine,ArticoliInst_ContrattiAss.RefInt,CONVERT(NVARCHAR(150),ArticoliInst_ContrattiAss.DesRefInt) AS DesRefInt " &
        ",ArticoliInst_ContrattiAss.Reparto,ArticoliInst_ContrattiAss.IDDocDTMM,ArticoliInst_ContrattiAss.Tipo_DocDTMM,ArticoliInst_ContrattiAss.Data_DocDTMM,ArticoliInst_ContrattiAss.RigaDTMM,ArticoliInst_ContrattiAss.NColloDTMM,ArticoliInst_ContrattiAss.QtaColliDTMM,ArticoliInst_ContrattiAss.SfusiDTMM,ArticoliInst_ContrattiAss.Operatore " &
        ",ArticoliInst_ContrattiAss.NomePC,ArticoliInst_ContrattiAss.BloccatoDalPC,ArticoliInst_ContrattiAss.InseritoDa,ArticoliInst_ContrattiAss.ModificatoDa,ArticoliInst_ContrattiAss.Data2InvioScadGa,ArticoliInst_ContrattiAss.Data2InvioScadEl,ArticoliInst_ContrattiAss.Data2InvioScadBa,ArticoliInst_ContrattiAss.NReInvio, " &
        "Clienti.Rag_Soc, Clienti.Denominazione, Clienti.Partita_IVA, Clienti.Codice_Fiscale, " &
        "Clienti.Localita, Clienti.Provincia, Clienti.Cap, Clienti.Telefono1, Clienti.Telefono2, Clienti.Fax, Clienti.Email AS EmailCliente, '" &
        AziendaReport.Trim & "' AS AziendaReport, '" & TitoloReport.Trim & "' AS TitoloReport, '" &
        TipoOrdineST & "' AS TipoOrdinamento, " &
        "CONVERT(DATETIME, '" & Format(DataEmailA, FormatoData) & "', 103) AS DataScadenza, " &
        "LTRIM(RTRIM(ISNULL(ArticoliInst_ContrattiAss.NsRiferimento,''))) + ' ' + LTRIM(RTRIM(ISNULL(ArticoliInst_ContrattiAss.Riferimento,''))) " &
        "AS RiferimentiRic, Clienti.EmailInvioScad AS EmailCliInvioScad, DestClienti.Email AS EmailDest, " &
        "EmailInviateT.Email AS EmailInvio, NReInvio, DataInvio "

        strSQLDettaglio &= "FROM DestClienti RIGHT OUTER JOIN " &
        "ArticoliInst_ContrattiAss ON DestClienti.Codice = ArticoliInst_ContrattiAss.Cod_Coge AND " &
        "DestClienti.Progressivo = ArticoliInst_ContrattiAss.Cod_Filiale LEFT OUTER JOIN " &
        "Categorie RIGHT OUTER JOIN " &
        "Clienti ON Categorie.Codice = Clienti.Categoria ON ArticoliInst_ContrattiAss.Cod_Coge = Clienti.Codice_CoGe INNER JOIN " &
        "EmailInviateDett ON ArticoliInst_ContrattiAss.ID = EmailInviateDett.IdArticoliInst_ContrattiAss INNER JOIN " &
        "EmailInviateT ON EmailInviateT.ID = EmailInviateDett.IdEmailInviateT "
        If StatoEmail = 999 Then
            strSQLDettaglio &= "WHERE "
        Else
            strSQLDettaglio &= "WHERE ISNULL(EmailInviateT.Stato,0) = " & StatoEmail.ToString.Trim & " AND "
        End If
        strSQLDettaglio &= "(DataInvio >= CONVERT(DATETIME, '" & Format(DataEmailDA, FormatoData) & "', 103) AND " &
               "DataInvio <= CONVERT(DATETIME, '" & Format(DataEmailA, FormatoData) & " 23:59:59', 103) ) AND " &
               "(EmailInviateT.Numero >= " & DalNumero.ToString.Trim & " AND " &
               "EmailInviateT.Numero <= " & AlNumero.ToString.Trim & ")"
        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@
        ''If SelScBa = True Or SelScEl = True Or SelScGa = True Then strSQLDettaglio += " AND ("
        ''Dim SWORDett As Boolean = False
        ' ''nessun test per i contratti al momento
        ' ''" Data_Fine <= CONVERT(DATETIME, '" & Format(DataScadenza, FormatoData) & "', 103) OR " 
        ''If SelScGa = True Then
        ''    strSQLDettaglio += " (DataScadGaranzia <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & "', 103) "
        ''    strSQLDettaglio += "AND DataScadGaranzia >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
        ''    strSQLDettaglio += "AND ISNULL(Data1InvioScadGa,0)=0) "
        ''    SWORDett = True
        ''End If
        ''If SelScEl = True Then
        ''    If SWORDett = True Then strSQLDettaglio += " OR "
        ''    strSQLDettaglio += " (DataScadElettrodi <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & "', 103) "
        ''    strSQLDettaglio += "AND DataScadElettrodi >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
        ''    strSQLDettaglio += "AND ISNULL(Data1InvioScadEl,0)=0) "
        ''    SWORDett = True
        ''End If
        ''If SelScBa = True Then
        ''    If SWORDett = True Then strSQLDettaglio += " OR "
        ''    strSQLDettaglio += " (DataScadBatterie <= CONVERT(DATETIME, '" & Format(DataScadenzaA, FormatoData) & "', 103) "
        ''    strSQLDettaglio += "AND DataScadBatterie >= CONVERT(DATETIME, '" & Format(DataScadenzaDA, FormatoData) & "', 103) "
        ''    strSQLDettaglio += "AND ISNULL(Data1InvioScadBa,0)=0) "
        ''    SWORDett = True
        ''End If
        ''If SelScBa = True Or SelScEl = True Or SelScGa = True Then strSQLDettaglio += ")"
        If Codice_CoGe.Trim <> "" Then
            strSQLDettaglio += " AND ArticoliInst_ContrattiAss.Cod_Coge = '" & Codice_CoGe.Trim & "'"
        End If
        '-
        'Al momento non usati ma ci sono NASCOSTI @@@@@@@@@@@@@@@@@@@@@@@@@
        'selezione categoria
        ' ''If SelTutteCatCli = False And SelCategorie = False Then
        ' ''    If SelRaggrCatCli = True Then
        ' ''        strSQLDettaglio += " AND (Categorie.Descrizione like '" & DescCatCli & "%')"
        ' ''    Else
        ' ''        strSQLDettaglio += " AND (ISNULL(Categorie.Codice,0) =" & CodCategoria & ")"
        ' ''    End If
        ' ''ElseIf SelCategorie = True And CodCategSel.Trim <> "" Then
        ' ''    Dim arrCodCategoria As String() = CodCategSel.Split(";")
        ' ''    strSQLDettaglio += " AND Categorie.Codice IN ("
        ' ''    For i = 0 To arrCodCategoria.Count - 1
        ' ''        If arrCodCategoria(i).ToString <> "" Then
        ' ''            strSQLDettaglio += arrCodCategoria(i).ToString & ","
        ' ''        End If
        ' ''    Next
        ' ''    strSQLDettaglio = strSQLDettaglio.Substring(0, strSQLDettaglio.Length - 1) 'rimuovo ultima virgola
        ' ''    strSQLDettaglio += ") "
        ' ''End If

        'se sono in visualizzazione dettaglio passo cod_coge selezionati da visualizzare
        If CodCogeSelezionati.Trim <> "" Then 'giu030918 SEMPRE VUOTO AL MOMENTO, 
            Dim arrCodCoge As String() = CodCogeSelezionati.Split(";")
            strSQLDettaglio += " AND Cod_Coge IN ("
            For i = 0 To arrCodCoge.Count - 1
                If arrCodCoge(i).ToString <> "" Then
                    strSQLDettaglio += "'" & arrCodCoge(i).ToString & "',"
                End If
            Next
            strSQLDettaglio = strSQLDettaglio.Substring(0, strSQLDettaglio.Length - 1) 'rimuovo ultima virgola
            strSQLDettaglio += ") "
        End If
        '---
        'giu030918 
        If SelDalNAlN.Trim <> "" Then
            Dim arrDalNAlN As String() = SelDalNAlN.Split(";")
            strSQLDettaglio += " AND EmailInviateT.Numero IN ("
            For i = 0 To arrDalNAlN.Count - 1
                If arrDalNAlN(i).ToString <> "" Then
                    strSQLDettaglio += arrDalNAlN(i).ToString & ","
                End If
            Next
            strSQLDettaglio = strSQLDettaglio.Substring(0, strSQLDettaglio.Length - 1) 'rimuovo ultima virgola
            strSQLDettaglio += ") AND EmailInviateT.Anno=" + DataEmailDA.Year.ToString.Trim + " "
        End If
        '---

        If TipoOrdine.Trim <> "" Then
            strSQLDettaglio += " ORDER BY " & TipoOrdine.Trim
        End If
        '***************FINE DETTAGLIO*******************
        Dim ObjDB As New DataBaseUtility
        If strSQLTestata <> "" And Not CaricaGrigliaDettaglio Then
            Try
                DsPrinWebDoc.ArticoliInstEmail.Clear()
                DsPrinWebDoc.ArticoliInstallati.Clear()
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQLTestata, DsPrinWebDoc, "ArticoliInstEmail")
            Catch ex As Exception
                ObjDB = Nothing
                Errore = ex.Message & " - Caricamento Tabella: ArticoliInstEmail"
                Return False
                Exit Function
            End Try
        End If
        '-
        If strSQLDettaglio <> "" And CaricaGrigliaDettaglio Then
            Try
                DsPrinWebDoc.ArticoliInstallati.Clear()
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQLDettaglio, DsPrinWebDoc, "ArticoliInstallati")
            Catch ex As Exception
                ObjDB = Nothing
                Errore = ex.Message & " - Caricamento Tabella: ArticoliInstallati"
                Return False
                Exit Function
            End Try
        End If
        ObjDB = Nothing
        Return True
    End Function
    'giu110618
    Public Shared Function ControllaUltEs(ByVal parDitta As String, ByVal parEser As String, ByRef strErrore As String) As Boolean
        ControllaUltEs = False
        strErrore = ""
        If String.IsNullOrEmpty(parDitta) Or _
           String.IsNullOrEmpty(parEser) Then
            strErrore = "(ControllaUltEs) Errore, Codice ditta/Esercizio non validi."
            Exit Function
        End If
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        SQLStr = "SELECT TOP (1) * FROM Esercizi WHERE Ditta = '" & parDitta & "' ORDER BY ESERCIZIO DESC"
        Dim dsCTR As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, SQLStr, dsCTR)
            If (dsCTR.Tables.Count > 0) Then
                If (dsCTR.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(dsCTR.Tables(0).Rows(dsCTR.Tables(0).Rows.Count - 1).Item("Esercizio")) Then
                        If dsCTR.Tables(0).Rows(dsCTR.Tables(0).Rows.Count - 1).Item("Esercizio").ToString.Trim <> parEser.Trim Then
                            '' 'strErrore = "Attenzione, l'ultimo esercizio risulta essere: " & dsCTR.Tables(0).Rows(dsCTR.Tables(0).Rows.Count - 1).Item("Esercizio").ToString.Trim
                            Exit Function
                        Else
                            ControllaUltEs = True
                        End If
                    Else
                        'NON E'POSSIBILE
                        strErrore = "(ControllaUltEs) Errore, Esercizio errato nella tabella Esercizi"
                        Exit Function
                    End If
                Else
                    'NESSUN ESERCIZIO
                    'NON E'POSSIBILE
                    strErrore = "(ControllaUltEs) Errore, nessun esercizio presente nella tabella Esercizi"
                    Exit Function
                End If
            Else
                'NESSUN ESERCIZIO
                'NON E'POSSIBILE
                strErrore = "(ControllaUltEs) Errore, nessun esercizio presente nella tabella Esercizi"
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = "(ControllaUltEs) Errore lettura dati azienda/esercizio: " & Ex.Message.Trim
            Exit Function
        End Try
        '-------------------------------------------------------------------
    End Function

    'giu130612
    Public Function StampaSchedaIN(ByVal IdDocumento As Long, ByRef DsPrinWebDoc As DSPrintWeb_Documenti, ByRef ObjReport As Object, ByRef Errore As String) As Boolean

        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim Passo As Integer = 0
        Dim SqlConnDoc As SqlConnection
        Dim SqlAdapDoc As SqlDataAdapter
        Dim SqlDbSelectCmd As SqlCommand
        Try
            SqlConnDoc = New SqlConnection
            SqlAdapDoc = New SqlDataAdapter
            SqlDbSelectCmd = New SqlCommand

            SqlAdapDoc.SelectCommand = SqlDbSelectCmd
            SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlDbSelectCmd.CommandText = "get_DocTByIDDocumenti"
            SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectCmd.Connection = SqlConnDoc
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            Dim SqlAdapDocDett As New SqlDataAdapter
            Dim SqlDbSelectDettCmd As New SqlCommand
            SqlDbSelectDettCmd.CommandText = "get_DocDByIDDocumenti"
            SqlDbSelectDettCmd.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettCmd.Connection = SqlConnDoc
            SqlDbSelectDettCmd.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocDett.SelectCommand = SqlDbSelectDettCmd

            'GIU301111 LOTTI
            Dim SqlAdapDocDettL As New SqlDataAdapter
            Dim SqlDbSelectDettCmdL As New SqlCommand
            SqlDbSelectDettCmdL.CommandText = "get_DocDLByIDDocRiga"
            SqlDbSelectDettCmdL.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDettCmdL.Connection = SqlConnDoc
            SqlDbSelectDettCmdL.Parameters.AddRange(New SqlParameter() {New SqlParameter("@RETURN_VALUE", SqlDbType.[Variant], 0, ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", DataRowVersion.Current, Nothing)})
            SqlDbSelectDettCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDbSelectDettCmdL.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocDettL.SelectCommand = SqlDbSelectDettCmdL

            '==============CARICAMENTO DATASET ===================
            Passo = 1
            DsPrinWebDoc.Clear()
            SqlDbSelectCmd.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlAdapDoc.Fill(DsPrinWebDoc.DocumentiT)
            Passo = 2
            SqlDbSelectDettCmd.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlAdapDocDett.Fill(DsPrinWebDoc.DocumentiD)
            Passo = 3
            SqlDbSelectDettCmdL.Parameters.Item("@IDDocumenti").Value = IdDocumento
            SqlDbSelectDettCmdL.Parameters.Item("@Riga").Value = DBNull.Value
            SqlAdapDocDettL.Fill(DsPrinWebDoc.DocumentiDLotti)
        Catch ex As Exception
            Errore = ex.Message & " - Documento lettura testata e dettagli. Passo: " & Passo.ToString
            Return False
            Exit Function
        End Try
        '==FINE CARICAMENTO ====================================
        Return True

    End Function

    'GIU150612 
    Public Shared Function CKStatoDoc(ByVal _ID As Long, ByRef StrErrore As String, Optional ByRef _InseritoDa As String = "", Optional ByRef _ModificatoDa As String = "", Optional ByRef _BloccatoDa As String = "", Optional ByRef _NomePC As String = "", Optional ByRef _Operatore As String = "") As Integer
        CKStatoDoc = -1
        _InseritoDa = "" : _ModificatoDa = "" : _BloccatoDa = "" : _NomePC = "" : _Operatore = ""
        Dim strSQL As String = ""
        Dim SWOk As Boolean = True
        strSQL = "SELECT TOP 1 IDDocumenti, StatoDoc, InseritoDa, ModificatoDa, BloccatoDalPC, NomePC, Operatore FROM DocumentiT"
        strSQL = strSQL & " WHERE (IDDocumenti = " & _ID.ToString.Trim & ")"
        'CODICE IN CARICO... AND StatoDoc=5"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each rsTestata In ds.Tables(0).Select("")
                        CKStatoDoc = IIf(IsDBNull(rsTestata![StatoDoc]), 0, rsTestata![StatoDoc])
                        _InseritoDa = IIf(IsDBNull(rsTestata![InseritoDa]), "", rsTestata![InseritoDa])
                        _ModificatoDa = IIf(IsDBNull(rsTestata![ModificatoDa]), "", rsTestata![ModificatoDa])
                        _BloccatoDa = IIf(IsDBNull(rsTestata![BloccatoDalPC]), "", rsTestata![BloccatoDalPC])
                        _NomePC = IIf(IsDBNull(rsTestata![NomePC]), "", rsTestata![NomePC])
                        _Operatore = IIf(IsDBNull(rsTestata![Operatore]), "", rsTestata![Operatore])
                    Next
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            CKStatoDoc = -1
            StrErrore = "(CKStatoDoc) Si è verificato il seguente errore:" & Ex.Message
        End Try
    End Function

    'giu220612
    Public Shared Function BloccaDoc(ByVal _ID As Long, ByRef StrErrore As String, ByRef _BloccatoDa As String, ByRef _NomePC As String, ByRef _Operatore As String, ByVal SWAzzeraQtaSel As Boolean) As Boolean
        BloccaDoc = True
        Dim myBloccatoDa As String = "" : Dim myNomePC As String = "" : Dim myOperatore As String = ""
        Dim strSQL As String = ""
        strSQL = "SELECT TOP 1 IDDocumenti, StatoDoc, InseritoDa, ModificatoDa, BloccatoDalPC, NomePC, Operatore FROM DocumentiT"
        strSQL = strSQL & " WHERE (IDDocumenti = " & _ID.ToString.Trim & ")"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            If ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds) = False Then
                StrErrore = "Errore in lettura documento.!!"
                Return False
            End If
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each rsTestata In ds.Tables(0).Select("")
                        '_StatoDoc = IIf(IsDBNull(rsTestata![StatoDoc]), 0, rsTestata![StatoDoc])
                        '_InseritoDa = IIf(IsDBNull(rsTestata![InseritoDa]), "", rsTestata![InseritoDa])
                        '_ModificatoDa = IIf(IsDBNull(rsTestata![ModificatoDa]), "", rsTestata![ModificatoDa])
                        myBloccatoDa = IIf(IsDBNull(rsTestata![BloccatoDalPC]), "", rsTestata![BloccatoDalPC])
                        myNomePC = IIf(IsDBNull(rsTestata![NomePC]), "", rsTestata![NomePC])
                        myOperatore = IIf(IsDBNull(rsTestata![Operatore]), "", rsTestata![Operatore])
                    Next
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            BloccaDoc = False
            StrErrore = "(BloccaDoc) Si è verificato il seguente errore:" & Ex.Message
            Exit Function
        End Try
        If myBloccatoDa <> "" Then
            If _Operatore.Trim <> myOperatore.Trim Then
                _BloccatoDa = myBloccatoDa
                _NomePC = myNomePC
                _Operatore = myOperatore
                BloccaDoc = False
                StrErrore = "Attenzione, documento bloccato da: <br> " & myBloccatoDa
                Exit Function
            End If
        End If
        Dim ClsDBUt As New DataBaseUtility
        Try
            strSQL = "Update DocumentiT Set BloccatoDalPC='" & _BloccatoDa.Trim & "', " & _
            "Operatore='" & _Operatore.Trim & "' Where IdDocumenti = " & _ID.ToString.Trim
            If ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL) = False Then
                StrErrore = "Errore in aggiornamento blocco documento.!!"
                Return False
            End If
        Catch ex As Exception
            ClsDBUt = Nothing
            BloccaDoc = False
            StrErrore = "(BloccaDoc Azzera Qta_Selezionata) Si è verificato il seguente errore:" & ex.Message
            Exit Function
        End Try
        If SWAzzeraQtaSel Then
            Try
                strSQL = "Update DocumentiD Set Qta_Selezionata=0 Where IdDocumenti = " & _ID.ToString.Trim
                If ClsDBUt.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL) = False Then
                    StrErrore = "Errore in Azzera Qta_Selezionata documento.!!"
                    Return False
                End If
            Catch ex As Exception
                ClsDBUt = Nothing
                BloccaDoc = False
                StrErrore = "(BloccaDoc Azzera Qta_Selezionata) Si è verificato il seguente errore:" & ex.Message
                Exit Function
            End Try
        End If
        ClsDBUt = Nothing
    End Function

    'giu210912
    Public Shared Function CKSconti(ByVal _ID As Long, ByRef StrErrore As String) As Boolean
        CKSconti = True
        Dim strSQL As String = ""
        strSQL = "SELECT TOP 1 IDDocumenti FROM DocumentiD"
        strSQL += " WHERE (IDDocumenti = " & _ID.ToString.Trim & ") AND "
        strSQL += " (Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0)"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            If ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds) = False Then
                StrErrore = "Errore in lettura documento.!! (CKSconti)"
                Return True
            End If
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each rsTestata In ds.Tables(0).Select("")
                        If _ID = IIf(IsDBNull(rsTestata![IDDocumenti]), 0, rsTestata![IDDocumenti]) Then
                            CKSconti = True
                        End If
                    Next
                Else
                    CKSconti = False
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            CKSconti = True
            StrErrore = "(CKSconti) Si è verificato il seguente errore:" & Ex.Message
            Exit Function
        End Try
    End Function
    'giu041219
    Public Shared Function CKScontiCA(ByVal _ID As Long, ByRef StrErrore As String) As Boolean
        CKScontiCA = True
        Dim strSQL As String = ""
        strSQL = "SELECT TOP 1 IDDocumenti FROM ContrattiD"
        strSQL += " WHERE (IDDocumenti = " & _ID.ToString.Trim & ") AND "
        strSQL += " (Sconto_1<>0 OR Sconto_2<>0 OR Sconto_3<>0 OR Sconto_4<>0 OR Sconto_Pag<>0 OR ScontoValore<>0)"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            If ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds) = False Then
                StrErrore = "Errore in lettura contrattiD.!! (CKScontiCA)"
                Return True
            End If
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each rsTestata In ds.Tables(0).Select("")
                        If _ID = IIf(IsDBNull(rsTestata![IDDocumenti]), 0, rsTestata![IDDocumenti]) Then
                            CKScontiCA = True
                        End If
                    Next
                Else
                    CKScontiCA = False
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            CKScontiCA = True
            StrErrore = "(CKScontiCA) Si è verificato il seguente errore:" & Ex.Message
            Exit Function
        End Try
    End Function
    'giu040814 giu160215 splitIVA GIU221217 OK FATTO giu030219 abilito per tutti i tipi di documenti 
    'Differenti se leggo per documento valorizzo Differenti con i dati non uguali solo descrittivo
    Public Shared Function CKClientiIPAByIDDocORCod(ByVal _IDDoc As Long, ByVal _CodCli As String, ByRef _SplitIVA As Boolean, ByRef StrErrore As String, Optional ByRef _Nazione As String = "", _
                                                    Optional ByRef _TotaleDoc As Decimal = 0, Optional ByRef _TotaleNettoPag As Decimal = 0) As Boolean
        CKClientiIPAByIDDocORCod = False : StrErrore = ""
        _SplitIVA = False 'GIU221217
        Dim strSQL As String = ""
        If _IDDoc > 0 Then
            'giu080814 se leggo il doc. e il tipodoc è DT,FC,NC COMANDA FatturaPA
            '--------- ALTRIMENTI COMANDA IPA dell'anagrafica del clienti
            'GIU030219 SE LEGGO IL DOCUMENTO RESTITUISCO I DATI MEMORIZZATI IN TESTATA (FatturaPA,SplitIVA)
            'giu221217 stessa regola per lo SPLIT IVA
            ' ''strSQL = "SELECT Tipo_Doc, ISNULL(Clienti.IPA, N'') AS IPA, ISNULL(DocumentiT.FatturaPA, 0) AS FatturaPA, " & _
            ' ''         "ISNULL(Clienti.SplitIVA, 0) AS SplitIVA, ISNULL(DocumentiT.SplitIVA, 0) AS SplitIVADoc"
            ' ''strSQL += " FROM DocumentiT LEFT OUTER JOIN Clienti ON DocumentiT.Cod_Cliente = Clienti.Codice_CoGe"
            ' ''strSQL += " WHERE (IDDocumenti = " & _IDDoc.ToString.Trim & ")"
            '-
            strSQL = "SELECT Tipo_Doc, ISNULL(DocumentiT.FatturaPA, 0) AS FatturaPA, " & _
                     "ISNULL(DocumentiT.SplitIVA, 0) AS SplitIVADoc, ISNULL(Totale,0) AS TotaleDoc, ISNULL(TotNettoPagare,0) AS TotNettoPagare"
            strSQL += " FROM DocumentiT "
            strSQL += " WHERE (IDDocumenti = " & _IDDoc.ToString.Trim & ")"
            '----------------------------------------------------------------------------------------------
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                If ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds) = False Then
                    StrErrore = "Errore in lettura documento.!! (CKClientiIPAByIDDocORCod) 1"
                    Exit Function
                End If
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        '----------------------------------------------------------------------
                        'giu080814 se leggo il doc. e il tipodoc è DT,FC,NC COMANDA FatturaPA 
                        ' del documento
                        '--------- ALTRIMENTI COMANDA IPA dell'anagrafica del clienti
                        'giu221217 stessa regola per lo SPLIT IVA
                        'giu030219 abilito per tutti i tipi di documenti 
                        'giu030219 qui comada il DOCUMENTO SE VALORIZZATI 
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("FatturaPA")) Then 'GIU221217
                            If Val(ds.Tables(0).Rows(0).Item("FatturaPA")) <> 0 Then
                                CKClientiIPAByIDDocORCod = True
                            End If
                        End If
                        'giu160215 GIU221217
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("SplitIVADoc")) Then
                            If Val(ds.Tables(0).Rows(0).Item("SplitIVADoc")) <> 0 Then
                                _SplitIVA = True
                            End If
                        End If
                        'giu290519
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("TotaleDoc")) Then
                            _TotaleDoc = ds.Tables(0).Rows(0).Item("TotaleDoc")
                        Else
                            _TotaleDoc = 0
                        End If
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("TotNettoPagare")) Then
                            _TotaleNettoPag = ds.Tables(0).Rows(0).Item("TotNettoPagare")
                        Else
                            _TotaleNettoPag = 0
                        End If
                        '' ''DETERMINO SE SONO DIFFERENTI DALL'ANAGRAFICA
                        ' ''If Not IsDBNull(ds.Tables(0).Rows(0).Item("IPA")) Then
                        ' ''    If ds.Tables(0).Rows(0).Item("IPA").ToString.Trim <> "" And _
                        ' ''        ds.Tables(0).Rows(0).Item("IPA").ToString.Trim.Length = 6 Then 'GIU020119
                        ' ''        If CKClientiIPAByIDDocORCod = True Then
                        ' ''            'ok
                        ' ''        Else
                        ' ''            'Differenti = "Attenzione, il cliente selezionato ha il codice IPA (lungo 6 PA altrimenti 7 Privati/Ditte)"
                        ' ''        End If
                        ' ''    End If
                        ' ''End If
                        '' ''giu160215 GIU221217
                        ' ''If Not IsDBNull(ds.Tables(0).Rows(0).Item("SplitIVA")) Then
                        ' ''    If Val(ds.Tables(0).Rows(0).Item("SplitIVA")) <> 0 Then
                        ' ''        If _SplitIVA = True Then
                        ' ''            'ok
                        ' ''        Else
                        ' ''            'Differenti = "Attenzione, il cliente selezionato ha SplitIVA"
                        ' ''        End If
                        ' ''    End If
                        ' ''End If
                    End If
                End If
                ObjDB = Nothing
            Catch Ex As Exception
                StrErrore = "(CKClientiIPAByIDDocORCod) 1 Si è verificato il seguente errore:" & Ex.Message
                Exit Function
            End Try
        ElseIf _CodCli.Trim <> "" Then
            strSQL = "SELECT ISNULL(Clienti.IPA, N'') AS IPA, ISNULL(Clienti.SplitIVA, 0) AS SplitIVA, ISNULL(Clienti.Nazione,N'') AS Nazione" 'giu040319
            strSQL += " FROM Clienti WHERE (Codice_CoGe = '" & _CodCli.ToString.Trim & "')"
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                If ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds) = False Then
                    StrErrore = "Errore in lettura cliente.!! (CKClientiIPAByIDDocORCod) 2"
                    Exit Function
                End If
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("IPA")) Then
                            If ds.Tables(0).Rows(0).Item("IPA").ToString.Trim <> "" And _
                                ds.Tables(0).Rows(0).Item("IPA").ToString.Trim.Length = 6 Then 'GIU020119
                                CKClientiIPAByIDDocORCod = True
                            End If
                        End If
                        'giu160215 GIU221217
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("SplitIVA")) Then
                            If Val(ds.Tables(0).Rows(0).Item("SplitIVA")) <> 0 Then
                                _SplitIVA = True
                            End If
                        End If
                        'giu040319
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("Nazione")) Then
                            _Nazione = ds.Tables(0).Rows(0).Item("Nazione").ToString.Trim
                        End If
                    End If
                End If
                ObjDB = Nothing
            Catch Ex As Exception
                StrErrore = "(CKClientiIPAByIDDocORCod) 2 Si è verificato il seguente errore:" & Ex.Message
                Exit Function
            End Try
        Else
            StrErrore = "(CKClientiIPAByIDDocORCod) 3 Chiamata funzione senza parametri."
            Exit Function
        End If
    End Function
    'giu100120 abilito per tutti i tipi di documenti 
    'Differenti se leggo per documento valorizzo Differenti con i dati non uguali solo descrittivo
    Public Shared Function CKClientiIPAByIDConORCod(ByVal _IDDoc As Long, ByVal _CodCli As String, ByRef _SplitIVA As Boolean, ByRef StrErrore As String, Optional ByRef _Nazione As String = "", _
                                                    Optional ByRef _TotaleDoc As Decimal = 0, Optional ByRef _TotaleNettoPag As Decimal = 0) As Boolean
        CKClientiIPAByIDConORCod = False : StrErrore = ""
        _SplitIVA = False
        Dim strSQL As String = ""
        If _IDDoc > 0 Then
            'giu080814 se leggo il doc. e il tipodoc è DT,FC,NC COMANDA FatturaPA
            '--------- ALTRIMENTI COMANDA IPA dell'anagrafica del clienti
            'GIU030219 SE LEGGO IL DOCUMENTO RESTITUISCO I DATI MEMORIZZATI IN TESTATA (FatturaPA,SplitIVA)
            'giu221217 stessa regola per lo SPLIT IVA
            strSQL = "SELECT Tipo_Doc, ISNULL(ContrattiT.FatturaPA, 0) AS FatturaPA, " & _
                     "ISNULL(ContrattiT.SplitIVA, 0) AS SplitIVADoc, ISNULL(Totale,0) AS TotaleDoc, ISNULL(TotNettoPagare,0) AS TotNettoPagare"
            strSQL += " FROM ContrattiT "
            strSQL += " WHERE (IDDocumenti = " & _IDDoc.ToString.Trim & ")"
            '----------------------------------------------------------------------------------------------
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                If ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds) = False Then
                    StrErrore = "Errore in lettura contratto.!! (CKClientiIPAByIDConORCod) 1"
                    Exit Function
                End If
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        '----------------------------------------------------------------------
                        'giu080814 se leggo il doc. e il tipodoc è DT,FC,NC COMANDA FatturaPA 
                        ' del documento
                        '--------- ALTRIMENTI COMANDA IPA dell'anagrafica del clienti
                        'giu221217 stessa regola per lo SPLIT IVA
                        'giu030219 abilito per tutti i tipi di documenti 
                        'giu030219 qui comada il DOCUMENTO SE VALORIZZATI 
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("FatturaPA")) Then 'GIU221217
                            If Val(ds.Tables(0).Rows(0).Item("FatturaPA")) <> 0 Then
                                CKClientiIPAByIDConORCod = True
                            End If
                        End If
                        'giu160215 GIU221217
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("SplitIVADoc")) Then
                            If Val(ds.Tables(0).Rows(0).Item("SplitIVADoc")) <> 0 Then
                                _SplitIVA = True
                            End If
                        End If
                        'giu290519
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("TotaleDoc")) Then
                            _TotaleDoc = ds.Tables(0).Rows(0).Item("TotaleDoc")
                        Else
                            _TotaleDoc = 0
                        End If
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("TotNettoPagare")) Then
                            _TotaleNettoPag = ds.Tables(0).Rows(0).Item("TotNettoPagare")
                        Else
                            _TotaleNettoPag = 0
                        End If
                        '' ''DETERMINO SE SONO DIFFERENTI DALL'ANAGRAFICA
                        ' ''If Not IsDBNull(ds.Tables(0).Rows(0).Item("IPA")) Then
                        ' ''    If ds.Tables(0).Rows(0).Item("IPA").ToString.Trim <> "" And _
                        ' ''        ds.Tables(0).Rows(0).Item("IPA").ToString.Trim.Length = 6 Then 'GIU020119
                        ' ''        If CKClientiIPAByIDConORCod = True Then
                        ' ''            'ok
                        ' ''        Else
                        ' ''            'Differenti = "Attenzione, il cliente selezionato ha il codice IPA (lungo 6 PA altrimenti 7 Privati/Ditte)"
                        ' ''        End If
                        ' ''    End If
                        ' ''End If
                        '' ''giu160215 GIU221217
                        ' ''If Not IsDBNull(ds.Tables(0).Rows(0).Item("SplitIVA")) Then
                        ' ''    If Val(ds.Tables(0).Rows(0).Item("SplitIVA")) <> 0 Then
                        ' ''        If _SplitIVA = True Then
                        ' ''            'ok
                        ' ''        Else
                        ' ''            'Differenti = "Attenzione, il cliente selezionato ha SplitIVA"
                        ' ''        End If
                        ' ''    End If
                        ' ''End If
                    End If
                End If
                ObjDB = Nothing
            Catch Ex As Exception
                StrErrore = "(CKClientiIPAByIDConORCod) 1 Si è verificato il seguente errore:" & Ex.Message
                Exit Function
            End Try
        ElseIf _CodCli.Trim <> "" Then
            strSQL = "SELECT ISNULL(Clienti.IPA, N'') AS IPA, ISNULL(Clienti.SplitIVA, 0) AS SplitIVA, ISNULL(Clienti.Nazione,N'') AS Nazione" 'giu040319
            strSQL += " FROM Clienti WHERE (Codice_CoGe = '" & _CodCli.ToString.Trim & "')"
            Dim ObjDB As New DataBaseUtility
            Dim ds As New DataSet
            Try
                If ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds) = False Then
                    StrErrore = "Errore in lettura cliente.!! (CKClientiIPAByIDConORCod) 2"
                    Exit Function
                End If
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("IPA")) Then
                            If ds.Tables(0).Rows(0).Item("IPA").ToString.Trim <> "" And _
                                ds.Tables(0).Rows(0).Item("IPA").ToString.Trim.Length = 6 Then 'GIU020119
                                CKClientiIPAByIDConORCod = True
                            End If
                        End If
                        'giu160215 GIU221217
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("SplitIVA")) Then
                            If Val(ds.Tables(0).Rows(0).Item("SplitIVA")) <> 0 Then
                                _SplitIVA = True
                            End If
                        End If
                        'giu040319
                        If Not IsDBNull(ds.Tables(0).Rows(0).Item("Nazione")) Then
                            _Nazione = ds.Tables(0).Rows(0).Item("Nazione").ToString.Trim
                        End If
                    End If
                End If
                ObjDB = Nothing
            Catch Ex As Exception
                StrErrore = "(CKClientiIPAByIDConORCod) 2 Si è verificato il seguente errore:" & Ex.Message
                Exit Function
            End Try
        Else
            StrErrore = "(CKClientiIPAByIDConORCod) 3 Chiamata funzione senza parametri."
            Exit Function
        End If
    End Function
    'GIU221021
    '''Public Shared Function ckIPAClientiByIDDocOrCod(ByVal _IDDoc As Long, ByVal _CodCli As String, ByRef _strErrore As String) As Boolean
    '''    ckIPAClientiByIDDocOrCod = False : _strErrore = ""
    '''    Dim strSQL As String = ""
    '''    If _IDDoc > 0 Then
    '''        strSQL = "SELECT ISNULL(Clienti.IPA, N'') AS IPA"
    '''        strSQL += " FROM Clienti  INNER JOIN"
    '''        strSQL += " DocumentiT ON Clienti.Codice_CoGe = DocumentiT.Cod_Cliente"
    '''        strSQL += " WHERE (DocumentiT.IDDocumenti = " & _IDDoc.ToString.Trim & ")"
    '''        '----------------------------------------------------------------------------------------------
    '''        Dim ObjDB As New DataBaseUtility
    '''        Dim ds As New DataSet
    '''        Try
    '''            If ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds) = False Then
    '''                _strErrore = "Errore in lettura documento.!! (ckIPAClientiByIDDocOrCod) 1"
    '''                Exit Function
    '''            End If
    '''            If (ds.Tables.Count > 0) Then
    '''                If (ds.Tables(0).Rows.Count > 0) Then
    '''                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("IPA")) Then
    '''                        If ds.Tables(0).Rows(0).Item("IPA").ToString.Trim <> "" Then
    '''                            ckIPAClientiByIDDocOrCod = True
    '''                        End If
    '''                    End If
    '''                End If
    '''            End If
    '''            ObjDB = Nothing
    '''        Catch Ex As Exception
    '''            _strErrore = "(ckIPAClientiByIDDocOrCod) 1 Si è verificato il seguente errore:" & Ex.Message
    '''            Exit Function
    '''        End Try
    '''    ElseIf _CodCli.Trim <> "" Then
    '''        strSQL = "SELECT ISNULL(Clienti.IPA, N'') AS IPA"
    '''        strSQL += " FROM Clienti WHERE (Codice_CoGe = '" & _CodCli.ToString.Trim & "')"
    '''        Dim ObjDB As New DataBaseUtility
    '''        Dim ds As New DataSet
    '''        Try
    '''            If ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds) = False Then
    '''                _strErrore = "Errore in lettura cliente.!! (ckIPAClientiByIDDocOrCod) 2"
    '''                Exit Function
    '''            End If
    '''            If (ds.Tables.Count > 0) Then
    '''                If (ds.Tables(0).Rows.Count > 0) Then
    '''                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("IPA")) Then
    '''                        If ds.Tables(0).Rows(0).Item("IPA").ToString.Trim <> "" Then
    '''                            ckIPAClientiByIDDocOrCod = True
    '''                        End If
    '''                    End If
    '''                End If
    '''            End If
    '''            ObjDB = Nothing
    '''        Catch Ex As Exception
    '''            _strErrore = "(ckIPAClientiByIDDocOrCod) Si è verificato il seguente errore 2:" & Ex.Message
    '''            Exit Function
    '''        End Try
    '''    Else
    '''        _strErrore = "(ckIPAClientiByIDDocOrCod) Chiamata funzione senza parametri."
    '''        Exit Function
    '''    End If
    '''End Function
End Class
