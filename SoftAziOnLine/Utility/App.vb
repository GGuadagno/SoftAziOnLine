Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Def

Public Class App

#Region "Caricamento dati iniziale"

    Public Shared Function CaricaDatiApplicazione(ByVal numEsercizio As String, Optional ByVal Reload As Boolean = False, Optional ByRef strErrore As String = "") As Boolean
        Dim compflagDatiCaricati As String = String.Format("{0}_{1}", DATI_CARICATI, numEsercizio)
        If (HttpRuntime.Cache(compflagDatiCaricati) Is Nothing) Then
            HttpRuntime.Cache(compflagDatiCaricati) = False
        End If
        If Not HttpRuntime.Cache(compflagDatiCaricati) Or reload Then
            Dim resCariamento As New StringBuilder
            CaricaDatiApplicazione = True
            If (Not CaricaParametri(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftAzi, "ParametriGeneraliAZI")
            End If
            If (Not CaricaProgressiviCoGe(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Progressivi")
            End If
            If (Not CaricaNazioni(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Nazioni")
            End If
            If (Not CaricaAliquoteIva(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Aliquote_IVA")
            End If
            If (Not CaricaProvince(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Province")
            End If
            If (Not CaricaPagamenti(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Pagamenti")
            End If
            If (Not CaricaZone(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Zone")
            End If
            If (Not CaricaVettori(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Vettori")
            End If
            If (Not CaricaCategorie(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Categorie")
            End If
            If (Not CaricaPianoDeiConti(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "PianoDeiConti")
            End If
            If (Not CaricaAgenti(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Agenti")
            End If
            If (Not CaricaListVenT(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftAzi, "ListVenT")
            End If
            If (Not CaricaClienti(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Clienti")
            End If
            If (Not CaricaFornitori(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftCoge, "Fornitori")
            End If
            If (Not CaricaArticoli(numEsercizio, strErrore)) Then
                CaricaDatiApplicazione = False
                Exit Function
            Else
                SessionUtility.DelAggTab(TipoDB.dbSoftAzi, "AnaMag")
            End If
            If (Not CaricaDatiApplicazione) Then
                HttpRuntime.Cache(compflagDatiCaricati) = False
            Else
                HttpRuntime.Cache(compflagDatiCaricati) = True
            End If
        Else
            CaricaDatiApplicazione = True
        End If
    End Function

#End Region

#Region "Metodi di caricamento dati"

    Public Shared Function CaricaParametri(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim ParamGestAziSys As New ParametriGeneraliAzi
        Dim myParamGestAzi As ParametriGeneraliAziEntity
        Dim arrParamGestAzi As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
            PARAM_GESTAZI, numEsercizio)

        Try
            arrParamGestAzi = ParamGestAziSys.getParametriGeneraliAzi()
            If (arrParamGestAzi.Count > 0) Then
                myParamGestAzi = CType(arrParamGestAzi(0), ParametriGeneraliAziEntity)
                AssegnaVarToCache(composeChiave, myParamGestAzi)
                CaricaParametri = True
            Else
                strErrore = "Nessun dato presente in tabella Parametri generali"
                CaricaParametri = False
            End If
        Catch ex As Exception
            strErrore = "ParamGestAzi: " & ex.Message
            CaricaParametri = False
        End Try
    End Function

    Public Shared Function CaricaProgressiviCoGe(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Progressivi
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.PROGRESSIVI_COGE, numEsercizio)

        Try
            result = sys.getProgressivi()
            AssegnaVarToCache(composeChiave, CType(result(0), ProgressiviEntity))
            CaricaProgressiviCoGe = True
        Catch ex As Exception
            strErrore = "Progressivi CoGe: " & ex.Message
            CaricaProgressiviCoGe = False
        End Try
    End Function

    Public Shared Function CaricaNazioni(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Nazioni
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.NAZIONI, numEsercizio)

        Try
            result = sys.getNazioni()
            AssegnaVarToCache(composeChiave, result)
            CaricaNazioni = True
        Catch ex As Exception
            strErrore = "Nazioni: " & ex.Message
            CaricaNazioni = False
        End Try
    End Function

    Public Shared Function CaricaAliquoteIva(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New AliquoteIva
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.ALIQUOTA_IVA, numEsercizio)

        Try
            result = sys.getAliquoteIva()
            AssegnaVarToCache(composeChiave, result)
            CaricaAliquoteIva = True
        Catch ex As Exception
            strErrore = "Aliquote IVA: " & ex.Message
            CaricaAliquoteIva = False
        End Try
    End Function

    Public Shared Function CaricaProvince(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Province
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.PROVINCE, numEsercizio)

        Try
            result = sys.getProvince()
            AssegnaVarToCache(composeChiave, result)
            CaricaProvince = True
        Catch ex As Exception
            strErrore = "Province: " & ex.Message
            CaricaProvince = False
        End Try
    End Function

    Public Shared Function CaricaPagamenti(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Pagamenti
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.PAGAMENTI, numEsercizio)

        Try
            result = sys.getPagamenti()
            AssegnaVarToCache(composeChiave, result)
            CaricaPagamenti = True
        Catch ex As Exception
            strErrore = "Pagamenti: " & ex.Message
            CaricaPagamenti = False
        End Try
    End Function

    Public Shared Function CaricaZone(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Zone
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.ZONE, numEsercizio)

        Try
            result = sys.getZone()
            AssegnaVarToCache(composeChiave, result)
            CaricaZone = True
        Catch ex As Exception
            strErrore = "Zone: " & ex.Message
            CaricaZone = False
        End Try
    End Function

    Public Shared Function CaricaVettori(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Vettori
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.VETTORI, numEsercizio)

        Try
            result = sys.getVettori()
            AssegnaVarToCache(composeChiave, result)
            CaricaVettori = True
        Catch ex As Exception
            strErrore = "Vettori: " & ex.Message
            CaricaVettori = False
        End Try
    End Function

    Public Shared Function CaricaCategorie(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Categorie
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.CATEGORIE, numEsercizio)

        Try
            result = sys.getCategorie()
            AssegnaVarToCache(composeChiave, result)
            CaricaCategorie = True
        Catch ex As Exception
            strErrore = "Categorie: " & ex.Message
            CaricaCategorie = False
        End Try
    End Function

    Public Shared Function CaricaPianoDeiConti(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New PianoDeiConti
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.PIANODEICONTI, numEsercizio)

        Try
            result = sys.getPianoDeiConti()
            AssegnaVarToCache(composeChiave, result)
            CaricaPianoDeiConti = True
        Catch ex As Exception
            strErrore = "Piano dei Conti: " & ex.Message
            CaricaPianoDeiConti = False
        End Try
    End Function

    Public Shared Function CaricaAgenti(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Agenti
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.AGENTI, numEsercizio)

        Try
            result = sys.getAgenti()
            AssegnaVarToCache(composeChiave, result)
            CaricaAgenti = True
        Catch ex As Exception
            strErrore = "Agenti: " & ex.Message
            CaricaAgenti = False
        End Try
    End Function

    Public Shared Function CaricaListVenT(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New ListVenT
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.LISTVEN_T, numEsercizio)

        Try
            result = sys.getListVenT()
            AssegnaVarToCache(composeChiave, result)
            CaricaListVenT = True
        Catch ex As Exception
            strErrore = "Listini testata: " & ex.Message
            CaricaListVenT = False
        End Try
    End Function

    Public Shared Function CaricaClienti(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Clienti
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.CLIENTI, numEsercizio)

        Try
            result = sys.getClienti()
            AssegnaVarToCache(composeChiave, result)
            CaricaClienti = True
        Catch ex As Exception
            strErrore = "Clienti: " & ex.Message
            CaricaClienti = False
        End Try
    End Function

    Public Shared Function CaricaFornitori(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New Fornitore
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.FORNITORI, numEsercizio)

        Try
            result = sys.getFornitori()
            AssegnaVarToCache(composeChiave, result)
            CaricaFornitori = True
        Catch ex As Exception
            strErrore = "Fornitori: " & ex.Message
            CaricaFornitori = False
        End Try
    End Function

    Public Shared Function CaricaArticoli(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New AnaMag
        Dim result As ArrayList = Nothing

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.E_COD_ARTICOLI, numEsercizio)

        Try
            For Each x As AnaMagEntity In sys.getAnaMag
                If (result Is Nothing) Then
                    result = New ArrayList
                End If
                'giu101111
                result.Add(New TemplateGridViewCodici(x.CodArticolo, String.Empty, x.Descrizione))
                ' ''Select Case x.CodArticolo.Length
                ' ''    Case 6
                ' ''        result.Add(New TemplateGridViewCodici(x.CodArticolo.Substring(0, 6), String.Empty, x.Descrizione))
                ' ''    Case 9
                ' ''        result.Add(New TemplateGridViewCodici(x.CodArticolo.Substring(0, 6), x.CodArticolo.Substring(6, 3), x.Descrizione))
                ' ''    Case Else
                ' ''        result.Add(New TemplateGridViewCodici(x.CodArticolo, String.Empty, x.Descrizione))
                ' ''End Select
            Next
            AssegnaVarToCache(composeChiave, result)
            CaricaArticoli = True
        Catch ex As Exception
            strErrore = "Articoli: " & ex.Message
            CaricaArticoli = False
        End Try
    End Function

#End Region

    
#Region "Metodi di estrazione dati"

#Region "Estrazione elenco completo"

    Public Shared Function GetParamGestAzi(ByVal numEsercizio As String) As ParametriGeneraliAziEntity
        Dim composeChiave As String = String.Format("{0}_{1}", _
            Def.PARAM_GESTAZI, numEsercizio)
        GetParamGestAzi = CType(HttpRuntime.Cache(composeChiave), ParametriGeneraliAziEntity)
        GetParamGestAzi = Nothing 'giu020119 CARICO SEMPRE DAL DB ALTRIMENTI SE CAMBIO QUALCOSA LEGGE SEMPRE IL PRIMO LOAD
        If (GetParamGestAzi Is Nothing) Then 'giu280312
            CaricaParametri(numEsercizio, "")
            GetParamGestAzi = CType(HttpRuntime.Cache(composeChiave), ParametriGeneraliAziEntity)
        End If
    End Function
    'giu190419 se seve altro aggiungere all'occorrenza
    Public Shared Function GetParGenAnno(ByVal numEsercizio As String, ByRef StrErrore As String) As strParGenAnno
        StrErrore = ""
        Dim strSQL As String = ""
        Dim SWOk As Boolean = True
        strSQL = "SELECT ISNULL(ScCassaDett,0) AS ScCassaDett, ISNULL(CalcoloScontoSuImporto,0) AS CalcoloScontoSuImporto FROM ParametriGeneraliAZI"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow
        GetParGenAnno = Nothing
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds, "", numEsercizio)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    GetParGenAnno.ScCassaDett = IIf(IsDBNull(row(0).Item("ScCassaDett")), False, row(0).Item("ScCassaDett"))
                    GetParGenAnno.CalcoloScontoSuImporto = IIf(IsDBNull(row(0).Item("CalcoloScontoSuImporto")), False, row(0).Item("CalcoloScontoSuImporto"))
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            StrErrore = "(GetParGenAnno) Si è verificato il seguente errore:" & Ex.Message
        End Try
    End Function

    Public Shared Function GetProgressiviCoGe(ByVal numEsercizio As String) As ProgressiviEntity
        Dim composeChiave As String = String.Format("{0}_{1}", _
            Def.PROGRESSIVI_COGE, numEsercizio)
        GetProgressiviCoGe = CType(HttpRuntime.Cache(composeChiave), ProgressiviEntity)
        If (GetProgressiviCoGe Is Nothing) Then 'giu280312
            CaricaProgressiviCoGe(numEsercizio, "")
            GetProgressiviCoGe = CType(HttpRuntime.Cache(composeChiave), ProgressiviEntity)
        End If
    End Function

    Public Shared Function GetElencoCompleto(ByVal nomeLista As String, ByVal numEsercizio As String) As ArrayList
        Dim composeChiave As String = String.Format("{0}_{1}", _
            nomeLista, numEsercizio)
        GetElencoCompleto = HttpRuntime.Cache(composeChiave)
        If (GetElencoCompleto Is Nothing) Then 'giu280312
            If nomeLista = Def.AGENTI Then
                CaricaAgenti(numEsercizio, "")
            End If
            If nomeLista = Def.ALIQUOTA_IVA Then
                CaricaAliquoteIva(numEsercizio, "")
            End If
            If nomeLista = Def.CATEGORIE Then
                CaricaCategorie(numEsercizio, "")
            End If
            If nomeLista = Def.CLIENTI Then
                CaricaClienti(numEsercizio, "")
            End If
            If nomeLista = Def.FORNITORI Then
                CaricaFornitori(numEsercizio, "")
            End If
            If nomeLista = Def.NAZIONI Then
                CaricaNazioni(numEsercizio, "")
            End If
            If nomeLista = Def.PAGAMENTI Then
                CaricaPagamenti(numEsercizio, "")
            End If
            If nomeLista = Def.PIANODEICONTI Then
                CaricaPianoDeiConti(numEsercizio, "")
            End If
            If nomeLista = Def.PROGRESSIVI_COGE Then
                CaricaProgressiviCoGe(numEsercizio, "")
            End If
            If nomeLista = Def.PROVINCE Then
                CaricaProvince(numEsercizio, "")
            End If
            If nomeLista = Def.VETTORI Then
                CaricaVettori(numEsercizio, "")
            End If
            If nomeLista = Def.ZONE Then
                CaricaZone(numEsercizio, "")
            End If
            If nomeLista = Def.LISTVEN_T Then 'giu020412
                CaricaListVenT(numEsercizio, "")
            End If
            GetElencoCompleto = HttpRuntime.Cache(composeChiave)
        End If
        
    End Function

    Public Shared Function GetLista(ByVal nomeLista As String, ByVal numEsercizio As String) As ArrayList
        Dim composeChiave As String = String.Empty
        Dim arrLista As ArrayList = Nothing
        'giu211011 pe errore nothing
        If (nomeLista = Nothing) Then
            arrLista = New ArrayList
            arrLista.Add(New TemplateGridViewGenerica(String.Empty, String.Empty))
            GetLista = arrLista
            Exit Function
        ElseIf String.IsNullOrEmpty(nomeLista.Trim) Then
            arrLista = New ArrayList
            arrLista.Add(New TemplateGridViewGenerica(String.Empty, String.Empty))
            GetLista = arrLista
            Exit Function
        End If
        '-
        If (nomeLista = Def.NAZIONI Or _
            nomeLista = Def.ALIQUOTA_IVA Or _
            nomeLista = Def.PROVINCE Or _
            nomeLista = Def.PAGAMENTI Or _
            nomeLista = Def.VETTORI Or _
            nomeLista = Def.ZONE Or _
            nomeLista = Def.CATEGORIE Or _
            nomeLista = Def.PIANODEICONTI Or _
            nomeLista = Def.AGENTI Or _
            nomeLista = Def.LISTVEN_T Or _
            nomeLista = Def.CLIENTI Or _
            nomeLista = Def.FORNITORI Or _
            nomeLista = Def.E_COD_ARTICOLI Or _
            nomeLista = Def.LISTVEN_T Or _
            nomeLista = Def.TIPOFATT) Then   'ALBERTO 19/12/2012
            'FINE FABIO 14/12/2011
            'giu270312 ATTENZIONE ATTENZIONE SE una tabella riceve da ENTITY PIU CAMPI OLTRE AI CAMPI
            'CODICE E DESCRIZIONE RICHIAMARE SEMPRE LA FUNZIONE:
            'arrLista = AdattaTabellaToTemplate(arrLista) CHE RITORNA L'ARRAYLIST CON I SOLI CAMPI
            'CODICE E DESCRIZIONE PER IL GRID GENERICO
            composeChiave = String.Format("{0}_{1}", nomeLista, numEsercizio)
            arrLista = HttpRuntime.Cache(composeChiave)
            'giu290312 
            If (arrLista Is Nothing) Then 'giu280312
                If nomeLista = Def.AGENTI Then
                    CaricaAgenti(numEsercizio, "")
                End If
                If nomeLista = Def.ALIQUOTA_IVA Then
                    CaricaAliquoteIva(numEsercizio, "")
                End If
                If nomeLista = Def.CATEGORIE Then
                    CaricaCategorie(numEsercizio, "")
                End If
                If nomeLista = Def.CLIENTI Then
                    CaricaClienti(numEsercizio, "")
                End If
                If nomeLista = Def.FORNITORI Then
                    CaricaFornitori(numEsercizio, "")
                End If
                If nomeLista = Def.NAZIONI Then
                    CaricaNazioni(numEsercizio, "")
                End If
                If nomeLista = Def.PAGAMENTI Then
                    CaricaPagamenti(numEsercizio, "")
                End If
                If nomeLista = Def.PIANODEICONTI Then
                    CaricaPianoDeiConti(numEsercizio, "")
                End If
                If nomeLista = Def.PROGRESSIVI_COGE Then
                    CaricaProgressiviCoGe(numEsercizio, "")
                End If
                If nomeLista = Def.PROVINCE Then
                    CaricaProvince(numEsercizio, "")
                End If
                If nomeLista = Def.VETTORI Then
                    CaricaVettori(numEsercizio, "")
                End If
                If nomeLista = Def.ZONE Then
                    CaricaZone(numEsercizio, "")
                End If
                If nomeLista = Def.LISTVEN_T Then 'giu020412
                    CaricaListVenT(numEsercizio, "")
                End If
                If nomeLista = Def.TIPOFATT Then
                    CaricaTipoFatt(numEsercizio, "")
                End If
                arrLista = HttpRuntime.Cache(composeChiave)
            End If
            '--------- end giu290312
            Select Case nomeLista
                Case Def.ALIQUOTA_IVA
                    arrLista = AdattaAliquoteIvaToTemplate(arrLista)
                Case Def.CATEGORIE
                    arrLista = AdattaCategorieToTemplate(arrLista)
                Case Def.PIANODEICONTI
                    arrLista = AdattaPianoDeiContiToTemplate(arrLista) 'GENERICA IDEM COME L'ELSE
                Case Def.CLIENTI
                    arrLista = AdattaClientiToTemplate(arrLista)
                Case Def.FORNITORI
                    arrLista = AdattaFornitoriToTemplate(arrLista)
                Case Else
                    arrLista = AdattaTabellaToTemplate(arrLista)
            End Select
            GetLista = arrLista
        Else
            arrLista = New ArrayList
            arrLista.Add(New TemplateGridViewGenerica(String.Empty, String.Empty))
            GetLista = arrLista
        End If
    End Function
    'giu040722
    Public Shared Function GetValoreFromChiaveFC(ByVal chiave As String, ByVal arrLista As ArrayList, ByRef rkFormCampi As StrFormCampi) As Boolean
        Dim arrTabella As ArrayList = Nothing
        Try
            arrTabella = GetListaFormCampi(arrLista)
            Dim trovato = From x In arrTabella Where x.Campo.ToString.Trim.ToUpper.Equals(chiave.Trim.ToUpper)
            If IsNothing(trovato(0)) Then 'giu090224
                GetValoreFromChiaveFC = False
                Exit Function
            End If
            GetValoreFromChiaveFC = True
            rkFormCampi.Campo = trovato(0).Campo.ToString.Trim
            rkFormCampi.Valore = trovato(0).Valore.ToString.Trim
            rkFormCampi.Abilitato = trovato(0).Abilitato
            rkFormCampi.Visibile = trovato(0).Visibile
            If UCase(rkFormCampi.Valore) = "TRUE" Then
                rkFormCampi.TrueFalse = True
            ElseIf UCase(rkFormCampi.Valore) = "FALSE" Then
                rkFormCampi.TrueFalse = False
            ElseIf IsNumeric(rkFormCampi.Valore.Trim) Then
                rkFormCampi.TrueFalse = CBool(rkFormCampi.Valore.Trim)
            End If
        Catch
            GetValoreFromChiaveFC = False 'String.Empty
        End Try
    End Function
    Public Shared Function GetListaFormCampi(ByVal pLista As ArrayList) As ArrayList
        Dim arrLista As ArrayList = Nothing
        'giu211011 pe errore nothing
        If (pLista Is Nothing) Then
            arrLista = New ArrayList
            arrLista.Add(New TemplateFormCampi(String.Empty, String.Empty, False, False))
            GetListaFormCampi = arrLista
            Exit Function
        Else
            arrLista = AdattaTabellaToTemplateFC(pLista)
        End If
        GetListaFormCampi = arrLista
    End Function
    Private Shared Function AdattaTabellaToTemplateFC(ByVal arrTabella As ArrayList) As ArrayList
        Dim arrTemplate As ArrayList = Nothing
        If (Not arrTabella Is Nothing) Then
            For Each x In arrTabella
                If (arrTemplate Is Nothing) Then
                    arrTemplate = New ArrayList
                End If
                arrTemplate.Add(New TemplateFormCampi(String.Format("{0}", x.Campo), x.Valore, x.Abilitato, x.Visibile))
            Next
        End If
        AdattaTabellaToTemplateFC = arrTemplate
    End Function
#End Region

    Public Shared Function GetValoreFromChiave(ByVal chiave As String, ByVal nomeLista As String, ByVal numEsercizio As String)
        Dim arrTabella As ArrayList = Nothing
        'GIU121211
        If (nomeLista = Def.CLIENTI Or _
           nomeLista = Def.FORNITORI) Then
            Try
                arrTabella = GetLista(nomeLista, numEsercizio)
                Dim trovato = From x In arrTabella Where x.Codice_CoGe.ToString.Trim.ToUpper.Equals(chiave.Trim.ToUpper)
                If Not IsNothing(trovato(0)) Then 'giu090224
                    GetValoreFromChiave = trovato(0).Rag_Soc.ToString.Trim
                Else
                    GetValoreFromChiave = String.Empty
                End If
            Catch
                GetValoreFromChiave = String.Empty
            End Try
            Exit Function
        End If
        '-
        If (nomeLista = Def.PIANODEICONTI) Then
            Try
                arrTabella = GetLista(nomeLista, numEsercizio)
                Dim trovato = From x In arrTabella Where x.Codice.ToString.ToUpper.Equals(chiave.Trim.ToUpper)
                If Not IsNothing(trovato(0)) Then 'giu090224
                    GetValoreFromChiave = trovato(0).Descrizione.ToString.Trim
                Else
                    GetValoreFromChiave = String.Empty
                End If
            Catch
                GetValoreFromChiave = String.Empty
            End Try
            Exit Function
        End If
        'GIU121211
        If (nomeLista = Def.NAZIONI Or _
           nomeLista = Def.ALIQUOTA_IVA Or _
           nomeLista = Def.PROVINCE Or _
           nomeLista = Def.PAGAMENTI Or _
           nomeLista = Def.VETTORI Or _
           nomeLista = Def.ZONE Or _
           nomeLista = Def.CATEGORIE Or _
           nomeLista = Def.AGENTI Or _
           nomeLista = Def.LISTVEN_T Or _
           nomeLista = Def.E_COD_ARTICOLI Or _
           nomeLista = Def.TIPOFATT) Then 'ALBERTO 19/12/2012
            Try
                arrTabella = GetLista(nomeLista, numEsercizio)
                'ALBERTO 19/12/2012
                If nomeLista = Def.TIPOFATT Then
                    If chiave.Length < 2 Then
                        chiave = "0" & chiave
                    End If
                End If
                '-----
                Dim trovato = From x In arrTabella Where x.Codice.ToString.Trim.ToUpper.Equals(chiave.Trim.ToUpper)
                If Not IsNothing(trovato(0)) Then 'giu090224
                    If (nomeLista = Def.PROVINCE) Then
                        GetValoreFromChiave = trovato(0).Codice.ToString.Trim
                    Else
                        GetValoreFromChiave = trovato(0).Descrizione.ToString.Trim
                    End If
                Else
                    GetValoreFromChiave = String.Empty
                End If
            Catch
                GetValoreFromChiave = String.Empty
            End Try
        Else
            GetValoreFromChiave = String.Empty
        End If
    End Function

    Public Shared Function CaricaTipoFatt(ByVal numEsercizio As String, ByRef strErrore As String) As Boolean
        Dim sys As New TipoFatt
        Dim result As ArrayList

        Dim composeChiave As String = String.Format("{0}_{1}", _
           Def.TIPOFATT, numEsercizio)

        Try
            result = sys.getTipoFatt()
            AssegnaVarToCache(composeChiave, result)
            CaricaTipoFatt = True
        Catch ex As Exception
            strErrore = "Listini testata: " & ex.Message
            CaricaTipoFatt = False
        End Try
    End Function

#End Region

#Region "Metodi private"

    Private Shared Function AdattaClientiToTemplate(ByVal arrClienti As ArrayList) As ArrayList
        Dim arrTemplate As ArrayList = Nothing
        If (Not arrClienti Is Nothing) Then
            For Each x As ClientiEntity In arrClienti
                If (arrTemplate Is Nothing) Then
                    arrTemplate = New ArrayList
                End If
                arrTemplate.Add(New TemplateGridViewElencoClienti( _
                    x.Codice_CoGe, x.Rag_Soc, x.Denominazione, x.Partita_IVA, x.Codice_Fiscale, x.CAP, x.Localita, x.Indirizzo, x.Email, x.EmailInvioScad, x.EmailInvioFatt, x.PECEmail, x.IPA))
            Next
        End If
        AdattaClientiToTemplate = arrTemplate
    End Function

    Private Shared Function AdattaFornitoriToTemplate(ByVal arrFornitori As ArrayList) As ArrayList
        Dim arrTemplate As ArrayList = Nothing
        If (Not arrFornitori Is Nothing) Then
            For Each x As FornitoreEntity In arrFornitori
                If (arrTemplate Is Nothing) Then
                    arrTemplate = New ArrayList
                End If
                arrTemplate.Add(New TemplateGridViewElencoClienti( _
                    x.Codice_CoGe, x.Rag_Soc, x.Denominazione, x.Partita_IVA, x.Codice_Fiscale, x.CAP, x.Localita, x.Indirizzo, x.Email, "", "", x.PECEMail, ""))
            Next
        End If
        AdattaFornitoriToTemplate = arrTemplate
    End Function

    Private Shared Function AdattaAliquoteIvaToTemplate(ByVal arrAliquoteIva As ArrayList) As ArrayList
        Dim arrTemplate As ArrayList = Nothing
        If (Not arrAliquoteIva Is Nothing) Then
            For Each x As AliquoteIvaEntity In arrAliquoteIva
                If (arrTemplate Is Nothing) Then
                    arrTemplate = New ArrayList
                End If
                arrTemplate.Add(New TemplateGridViewGenerica(String.Format("{0}", x.Aliquota), x.Descrizione))
            Next
        End If
        AdattaAliquoteIvaToTemplate = arrTemplate
    End Function

    Private Shared Function AdattaCategorieToTemplate(ByVal arrCategorie As ArrayList) As ArrayList
        Dim arrTemplate As ArrayList = Nothing
        If (Not arrCategorie Is Nothing) Then
            For Each x As CategorieEntity In arrCategorie
                If (arrTemplate Is Nothing) Then
                    arrTemplate = New ArrayList
                End If
                arrTemplate.Add(New TemplateCategorie(String.Format("{0}", x.Codice), x.Descrizione, x.InvioMailSc, x.SelSc))
            Next
        End If
        AdattaCategorieToTemplate = arrTemplate
    End Function

    Private Shared Function AdattaPianoDeiContiToTemplate(ByVal arrPianoDeiConti As ArrayList) As ArrayList
        Dim arrTemplate As ArrayList = Nothing
        If (Not arrPianoDeiConti Is Nothing) Then
            For Each x As PianoDeiContiEntity In arrPianoDeiConti
                If (arrTemplate Is Nothing) Then
                    arrTemplate = New ArrayList
                End If
                arrTemplate.Add(New TemplateGridViewGenerica(String.Format("{0}", x.Codice_CoGe), x.Descrizione))
            Next
        End If
        AdattaPianoDeiContiToTemplate = arrTemplate
    End Function

    Private Shared Function AdattaTabellaToTemplate(ByVal arrTabella As ArrayList) As ArrayList
        Dim arrTemplate As ArrayList = Nothing
        If (Not arrTabella Is Nothing) Then
            For Each x In arrTabella
                If (arrTemplate Is Nothing) Then
                    arrTemplate = New ArrayList
                End If
                arrTemplate.Add(New TemplateGridViewGenerica(String.Format("{0}", x.Codice), x.Descrizione))
            Next
        End If
        AdattaTabellaToTemplate = arrTemplate
    End Function

    Private Shared Sub AssegnaVarToCache(ByVal chiave As String, ByVal valore As Object)
        If (valore Is Nothing) Then
           
        Else
            If (HttpRuntime.Cache(chiave) Is Nothing) Then
                HttpRuntime.Cache.Add(chiave, valore, Nothing, _
                    Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, _
                    CacheItemPriority.NotRemovable, Nothing)
            Else
                HttpRuntime.Cache(chiave) = valore
            End If
        End If
        
    End Sub
    'giu281022
    Public Shared Sub SetObjectToCache(ByVal chiave As String, ByVal valore As Object)
        If (valore Is Nothing) Then

        Else
            If (HttpRuntime.Cache(chiave) Is Nothing) Then
                HttpRuntime.Cache.Add(chiave, valore, Nothing, _
                    Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, _
                    CacheItemPriority.NotRemovable, Nothing)
            Else
                HttpRuntime.Cache(chiave) = valore
            End If
        End If
    End Sub
    Public Shared Sub GetObjectToCache(ByVal chiave As String, ByRef valore As Object)
        valore = HttpRuntime.Cache(chiave)
    End Sub

    'giu241111
    Public Shared Function GetDatiDitta(ByVal CodiceDitta As String, ByRef strErrore As String) As StrDitta
        strErrore = ""
        If CodiceDitta.Trim = "" Then
            GetDatiDitta = Nothing
            Exit Function
        End If
        GetDatiDitta.Codice = CodiceDitta
        Dim strSQL As String = ""
        strSQL = "Select * From Ditta WHERE Codice = '" & CodiceDitta.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    GetDatiDitta.Descrizione = IIf(IsDBNull(row(0).Item("Descrizione")), "", row(0).Item("Descrizione"))
                    GetDatiDitta.PartitaIVA = IIf(IsDBNull(row(0).Item("PartitaIVA")), "", row(0).Item("PartitaIVA"))
                    GetDatiDitta.Telefono = IIf(IsDBNull(row(0).Item("Telefono")), "", row(0).Item("Telefono"))
                    GetDatiDitta.Fax = IIf(IsDBNull(row(0).Item("Fax")), "", row(0).Item("Fax"))
                    GetDatiDitta.Indirizzo = IIf(IsDBNull(row(0).Item("Indirizzo")), "", row(0).Item("Indirizzo"))
                    GetDatiDitta.Citta = IIf(IsDBNull(row(0).Item("Citta")), "", row(0).Item("Citta"))
                    GetDatiDitta.CAP = IIf(IsDBNull(row(0).Item("CAP")), "", row(0).Item("CAP"))
                    GetDatiDitta.Provincia = IIf(IsDBNull(row(0).Item("Provincia")), "", row(0).Item("Provincia"))
                    GetDatiDitta.Password = IIf(IsDBNull(row(0).Item("Password")), "", row(0).Item("Password"))
                    GetDatiDitta.Livello_Op = IIf(IsDBNull(row(0).Item("Livello_Op")), "", row(0).Item("Livello_Op"))
                    GetDatiDitta.MaxLevel = IIf(IsDBNull(row(0).Item("MaxLevel")), VALMAXLEVEL, row(0).Item("MaxLevel")) 'GIU01122011
                    GetDatiDitta.MaskLevel = IIf(IsDBNull(row(0).Item("MaskLevel")), VALMASKLEVEL, row(0).Item("MaskLevel")) 'GIU01122011
                    GetDatiDitta.Analitico = IIf(IsDBNull(row(0).Item("Analitico")), False, CBool(row(0).Item("Analitico")))
                    GetDatiDitta.RIBA = IIf(IsDBNull(row(0).Item("RIBA")), False, CBool(row(0).Item("RIBA")))
                    GetDatiDitta.Iva_Credito = IIf(IsDBNull(row(0).Item("Iva_Credito")), 0, CDec(row(0).Item("Iva_Credito")))
                    GetDatiDitta.Iva_Debito = IIf(IsDBNull(row(0).Item("Iva_Debito")), 0, CDec(row(0).Item("Iva_Debito")))
                    Try
                        GetDatiDitta.Iva_Data = IIf(IsDBNull(row(0).Item("Iva_Data")), DATANULL, CDate(row(0).Item("Iva_Data")))
                    Catch ex As Exception
                        GetDatiDitta.Iva_Data = DATANULL
                    End Try

                    GetDatiDitta.TestoECC = IIf(IsDBNull(row(0).Item("TestoECC")), "", row(0).Item("TestoECC"))
                    GetDatiDitta.PiedeECC = IIf(IsDBNull(row(0).Item("PiedeECC")), "", row(0).Item("PiedeECC"))
                    GetDatiDitta.SL_Indirizzo = IIf(IsDBNull(row(0).Item("SL_Indirizzo")), "", row(0).Item("SL_Indirizzo"))
                    GetDatiDitta.SL_Cap = IIf(IsDBNull(row(0).Item("SL_Cap")), "", row(0).Item("SL_Cap"))
                    GetDatiDitta.SL_Citta = IIf(IsDBNull(row(0).Item("SL_Citta")), "", row(0).Item("SL_Citta"))
                    GetDatiDitta.SL_Provincia = IIf(IsDBNull(row(0).Item("SL_Provincia")), "", row(0).Item("SL_Provincia"))
                    GetDatiDitta.SWPersFis = IIf(IsDBNull(row(0).Item("SWPersFis")), False, CBool(row(0).Item("SWPersFis")))
                    GetDatiDitta.CodiceFiscale = IIf(IsDBNull(row(0).Item("CodiceFiscale")), "", row(0).Item("CodiceFiscale"))
                    GetDatiDitta.Cognome = IIf(IsDBNull(row(0).Item("Cognome")), "", row(0).Item("Cognome"))
                    GetDatiDitta.Nome = IIf(IsDBNull(row(0).Item("Nome")), "", row(0).Item("Nome"))
                    GetDatiDitta.Sesso = IIf(IsDBNull(row(0).Item("Sesso")), "", row(0).Item("Sesso"))
                    Try
                        GetDatiDitta.Data_Nascita = IIf(IsDBNull(row(0).Item("Data_Nascita")), DATANULL, CDate(row(0).Item("Data_Nascita")))
                    Catch ex As Exception
                        GetDatiDitta.Data_Nascita = DATANULL
                    End Try
                    GetDatiDitta.ComNas = IIf(IsDBNull(row(0).Item("ComNas")), "", row(0).Item("ComNas"))
                    GetDatiDitta.ProvNas = IIf(IsDBNull(row(0).Item("ProvNas")), "", row(0).Item("ProvNas"))
                    GetDatiDitta.SL_Denominazione = IIf(IsDBNull(row(0).Item("SL_Denominazione")), "", row(0).Item("SL_Denominazione"))
                    GetDatiDitta.CFSoggObbAllIVA = IIf(IsDBNull(row(0).Item("CFSoggObbAllIVA")), "", row(0).Item("CFSoggObbAllIVA"))
                    GetDatiDitta.CFIntermediario = IIf(IsDBNull(row(0).Item("CFIntermediario")), "", row(0).Item("CFIntermediario"))
                    GetDatiDitta.IscrCAF = IIf(IsDBNull(row(0).Item("IscrCAF")), "", row(0).Item("IscrCAF"))
                    GetDatiDitta.Blocca_Accesso = IIf(IsDBNull(row(0).Item("Blocca_Accesso")), False, CBool(row(0).Item("Blocca_Accesso")))
                    GetDatiDitta.GetNomePC = IIf(IsDBNull(row(0).Item("GetNomePC")), "", row(0).Item("GetNomePC"))
                    Try
                        GetDatiDitta.Blocco_Dalle = IIf(IsDBNull(row(0).Item("Blocco_Dalle")), DATANULL, CDate(row(0).Item("Blocco_Dalle")))
                    Catch ex As Exception
                        GetDatiDitta.Blocco_Dalle = DATANULL
                    End Try

                    Exit Function
                Else
                    strErrore = "Errore: Caricamento dati Società - nessun dato presente"
                    GetDatiDitta = Nothing
                    Exit Function
                End If
            Else
                strErrore = "Errore: Caricamento dati Società - nessun dato presente"
                GetDatiDitta = Nothing
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = "Errore: Caricamento dati Società - " & Ex.Message
            GetDatiDitta = Nothing
            Exit Function
        End Try
    End Function

    'giu251111
    Public Shared Function GetDatiAbilitazioni(ByVal Tabella As String, ByVal Chiave As String, ByRef Descrizione As String, ByRef strErrore As String) As Boolean
        Descrizione = "" : strErrore = ""
        If Tabella.Trim = "" Then
            GetDatiAbilitazioni = False
            Exit Function
        End If
        'GIU200617 DEFAULT SEMPRE ATTIVO
        If UCase(Chiave.Trim) = UCase("TimeOUTST") Then
            Descrizione = "12000"
            GetDatiAbilitazioni = True
            Exit Function
        End If
        '---------
        Dim strSQL As String = ""
        strSQL = "Select * From " & Tabella.Trim & " WHERE Chiave = '" & Trim(Mid(Trim(Chiave), 1, 12)) & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbOpzioni, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    Descrizione = IIf(IsDBNull(row(0).Item("Descrizione")), "", row(0).Item("Descrizione").ToString.Trim)
                    GetDatiAbilitazioni = IIf(IsDBNull(row(0).Item("Abilitato")), False, CBool(row(0).Item("Abilitato")))
                    Exit Function
                Else
                    Descrizione = ""
                    ''strErrore = "Non definita Chiave in " & Tabella.Trim & " : '" & Chiave.Trim & "'"
                    GetDatiAbilitazioni = False
                    Call InserisciAbilitazione(Tabella.Trim, Chiave.Trim, strErrore)
                    Exit Function
                End If
            Else
                Descrizione = ""
                ''strErrore = "Non definita Chiave in " & Tabella.Trim & " : '" & Chiave.Trim & "'"
                Call InserisciAbilitazione(Tabella.Trim, Chiave.Trim, strErrore)
                GetDatiAbilitazioni = False
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = "Errore: Lettura " & Tabella.Trim & " : '" & Chiave.Trim & "' Err.: " & Ex.Message
            Descrizione = ""
            GetDatiAbilitazioni = False
            Exit Function
        End Try
    End Function
    'giu311018
    Public Shared Function InserisciAbilitazione(ByVal Tabella As String, ByVal Chiave As String, ByRef strErrore As String) As Boolean
        InserisciAbilitazione = True
        Dim strSQL As String = "" : strErrore = ""
        strSQL = "INSERT INTO " & Tabella.Trim & " (Chiave, Descrizione, Abilitato) " & _
                          "VALUES ('" & Trim(Mid(Trim(Chiave), 1, 12)) & "', 'Inserimento automatico', 0)"
        Dim ObjDB As New DataBaseUtility
        Try
            ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL)
            ObjDB = Nothing
        Catch ex As Exception
            strErrore = "Errore: InserisciAbilitazione " & Tabella.Trim & " : '" & Chiave.Trim & "' Err.: " & ex.Message
            InserisciAbilitazione = False
            Exit Function
        End Try

    End Function
    'giu061219
    Public Shared Function AggiornaAbilitazione(ByVal Tabella As String, ByVal Chiave As String, ByVal Descrizione As String, ByRef strErrore As String) As Boolean
        AggiornaAbilitazione = True
        Dim strSQL As String = "" : strErrore = ""
        strSQL = "UPDATE " & Tabella.Trim & " SET Descrizione = '" & Descrizione.Trim & "' WHERE (Chiave = '" & Chiave.Trim & "')"
        Dim ObjDB As New DataBaseUtility
        Try
            ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL)
            ObjDB = Nothing
        Catch ex As Exception
            strErrore = "Errore: AggiornaAbilitazione " & Tabella.Trim & " : '" & Chiave.Trim & "' Des.: " & " : '" & Descrizione.Trim & "' Err.: " & ex.Message
            AggiornaAbilitazione = False
            Exit Function
        End Try

    End Function

    Public Shared Function GetDatiBanche(ByVal ABI As String, ByRef strErrore As String) As StrBanche
        strErrore = ""
        If ABI.Trim = "" Then
            GetDatiBanche = Nothing
            Exit Function
        End If
        GetDatiBanche.ABI = ABI.Trim
        Dim strSQL As String = ""
        strSQL = "Select * From Banche WHERE ABI = '" & ABI.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    GetDatiBanche.Banca = IIf(IsDBNull(row(0).Item("Banca")), "", row(0).Item("Banca"))
                    Exit Function
                Else
                    'strErrore = "Errore: Lettura dati Banca - nessun dato presente"
                    GetDatiBanche = Nothing
                    Exit Function
                End If
            Else
                'strErrore = "Errore: Lettura dati Banca - nessun dato presente"
                GetDatiBanche = Nothing
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = "Errore: Lettura dati Banca - " & Ex.Message
            GetDatiBanche = Nothing
            Exit Function
        End Try
    End Function

    Public Shared Function GetDatiFiliali(ByVal ABI As String, ByVal CAB As String, ByRef strErrore As String) As StrFiliali
        strErrore = ""
        If ABI.Trim = "" Then
            GetDatiFiliali = Nothing
            Exit Function
        End If
        If CAB.Trim = "" Then
            GetDatiFiliali = Nothing
            Exit Function
        End If
        GetDatiFiliali.ABI = ABI.Trim
        GetDatiFiliali.CAB = CAB.Trim
        Dim strSQL As String = ""
        strSQL = "Select * From Filiali WHERE ABI = '" & ABI.Trim & "' AND CAB = '" & CAB.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    GetDatiFiliali.Filiale = IIf(IsDBNull(row(0).Item("Filiale")), "", row(0).Item("Filiale"))
                    GetDatiFiliali.Nazione = IIf(IsDBNull(row(0).Item("Nazione")), "", row(0).Item("Nazione"))
                    GetDatiFiliali.CAP = IIf(IsDBNull(row(0).Item("CAP")), "", row(0).Item("CAP"))
                    GetDatiFiliali.Provincia = IIf(IsDBNull(row(0).Item("Provincia")), "", row(0).Item("Provincia"))
                    GetDatiFiliali.Citta = IIf(IsDBNull(row(0).Item("Citta")), "", row(0).Item("Citta"))
                    GetDatiFiliali.Indirizzo = IIf(IsDBNull(row(0).Item("Indirizzo")), "", row(0).Item("Indirizzo"))
                    Exit Function
                Else
                    'strErrore = "Errore: Lettura dati Filiale - nessun dato presente"
                    GetDatiFiliali = Nothing
                    Exit Function
                End If
            Else
                'strErrore = "Errore: Lettura dati Filiale - nessun dato presente"
                GetDatiFiliali = Nothing
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = "Errore: Lettura dati Filiale - " & Ex.Message
            GetDatiFiliali = Nothing
            Exit Function
        End Try
    End Function

    'giu021211
    Public Shared Function GetDatiValute(ByVal Codice As String, ByRef strErrore As String) As StrValute
        strErrore = ""
        If Codice.Trim = "" Then
            GetDatiValute = Nothing
            Exit Function
        End If
        GetDatiValute.Codice = Codice.Trim
        Dim strSQL As String = ""
        strSQL = "Select * From Valute WHERE Codice = '" & Codice.Trim & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    GetDatiValute.Descrizione = IIf(IsDBNull(row(0).Item("Descrizione")), "", row(0).Item("Descrizione"))
                    GetDatiValute.Cambio_Fisso = IIf(IsDBNull(row(0).Item("Cambio_Fisso")), False, CBool(row(0).Item("Cambio_Fisso")))
                    GetDatiValute.Decimali = IIf(IsDBNull(row(0).Item("Decimali")), 0, row(0).Item("Decimali"))
                    GetDatiValute.Simbolo = IIf(IsDBNull(row(0).Item("Simbolo")), "", row(0).Item("Simbolo"))
                    Exit Function
                Else
                    'strErrore = "Errore: Lettura dati Valute - nessun dato presente"
                    GetDatiValute = Nothing
                    Exit Function
                End If
            Else
                'strErrore = "Errore: Lettura dati Valute - nessun dato presente"
                GetDatiValute = Nothing
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = "Errore: Lettura dati Valute - " & Ex.Message
            GetDatiValute = Nothing
            Exit Function
        End Try
    End Function

    'giu170412
    Public Shared Function GetDatiCausMag(ByVal Codice As Int32, ByRef strErrore As String) As StrCausMag
        strErrore = ""
        GetDatiCausMag.Codice = Codice
        Dim strSQL As String = ""
        strSQL = "Select * From CausMag WHERE Codice = " & Codice.ToString.Trim
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    GetDatiCausMag.Descrizione = IIf(IsDBNull(row(0).Item("Descrizione")), "", row(0).Item("Descrizione"))
                    GetDatiCausMag.Tipo = IIf(IsDBNull(row(0).Item("Tipo")), "", row(0).Item("Tipo"))
                    GetDatiCausMag.Segno_Giacenza = IIf(IsDBNull(row(0).Item("Segno_Giacenza")), "", row(0).Item("Segno_Giacenza"))
                    GetDatiCausMag.Fatturabile = (IIf(IsDBNull(row(0).Item("Fatturabile")), False, CBool(row(0).Item("Fatturabile"))))
                    GetDatiCausMag.Password = (IIf(IsDBNull(row(0).Item("Password")), False, CBool(row(0).Item("Password"))))
                    GetDatiCausMag.Segno_Prodotto = IIf(IsDBNull(row(0).Item("Segno_Prodotto")), "", row(0).Item("Segno_Prodotto"))
                    GetDatiCausMag.Segno_Confezionato = IIf(IsDBNull(row(0).Item("Segno_Confezionato")), "", row(0).Item("Segno_Confezionato"))
                    GetDatiCausMag.Segno_Ordinato = IIf(IsDBNull(row(0).Item("Segno_Ordinato")), "", row(0).Item("Segno_Ordinato"))
                    GetDatiCausMag.Segno_Venduto = IIf(IsDBNull(row(0).Item("Segno_Venduto")), "", row(0).Item("Segno_Venduto"))
                    GetDatiCausMag.Componenti = (IIf(IsDBNull(row(0).Item("Componenti")), False, CBool(row(0).Item("Componenti"))))
                    GetDatiCausMag.CausaleIndotta = IIf(IsDBNull(row(0).Item("CausaleIndotta")), 0, row(0).Item("CausaleIndotta"))
                    GetDatiCausMag.Cod_Utente = IIf(IsDBNull(row(0).Item("Cod_Utente")), 0, row(0).Item("Cod_Utente"))
                    GetDatiCausMag.Movimento_Magazzini = (IIf(IsDBNull(row(0).Item("Movimento_Magazzini")), False, CBool(row(0).Item("Movimento_Magazzini"))))
                    GetDatiCausMag.Movimento = (IIf(IsDBNull(row(0).Item("Movimento")), False, CBool(row(0).Item("Movimento"))))
                    GetDatiCausMag.CodContoCoGE = IIf(IsDBNull(row(0).Item("CodContoCoGE")), "", row(0).Item("CodContoCoGE"))
                    GetDatiCausMag.Segno_Deposito = IIf(IsDBNull(row(0).Item("Segno_Deposito")), "", row(0).Item("Segno_Deposito"))
                    GetDatiCausMag.CausVend = (IIf(IsDBNull(row(0).Item("CausVend")), False, CBool(row(0).Item("CausVend"))))
                    GetDatiCausMag.Segno_Lotti = IIf(IsDBNull(row(0).Item("Segno_Lotti")), "", row(0).Item("Segno_Lotti"))
                    GetDatiCausMag.Segno_CL = IIf(IsDBNull(row(0).Item("Segno_CL")), "", row(0).Item("Segno_CL"))
                    GetDatiCausMag.CausCostoVenduto = (IIf(IsDBNull(row(0).Item("CausCostoVenduto")), False, CBool(row(0).Item("CausCostoVenduto"))))
                    GetDatiCausMag.CVisione = (IIf(IsDBNull(row(0).Item("CVisione")), False, CBool(row(0).Item("CVisione"))))
                    GetDatiCausMag.CDeposito = (IIf(IsDBNull(row(0).Item("CDeposito")), False, CBool(row(0).Item("CDeposito"))))
                    GetDatiCausMag.CausCVenditaDaCVisione = IIf(IsDBNull(row(0).Item("CausCVenditaDaCVisione")), 0, row(0).Item("CausCVenditaDaCVisione"))
                    GetDatiCausMag.CausCVenditaDaCDeposito = IIf(IsDBNull(row(0).Item("CausCVenditaDaCDeposito")), 0, row(0).Item("CausCVenditaDaCDeposito"))
                    GetDatiCausMag.CausResoDaCVisione = IIf(IsDBNull(row(0).Item("CausResoDaCVisione")), 0, row(0).Item("CausResoDaCVisione"))
                    GetDatiCausMag.CausResoDaCDeposito = IIf(IsDBNull(row(0).Item("CausResoDaCDeposito")), 0, row(0).Item("CausResoDaCDeposito"))
                    GetDatiCausMag.PrezzoAL = IIf(IsDBNull(row(0).Item("PrezzoAL")), "", row(0).Item("PrezzoAL"))
                    GetDatiCausMag.Reso = (IIf(IsDBNull(row(0).Item("Reso")), False, CBool(row(0).Item("Reso"))))
                    GetDatiCausMag.CausMag2 = (IIf(IsDBNull(row(0).Item("CausMag2")), 0, row(0).Item("CausMag2")))
                    Exit Function
                Else
                    'strErrore = "Errore: Lettura dati Causali di Magazzino - nessun dato presente"
                    GetDatiCausMag = Nothing
                    Exit Function
                End If
            Else
                'strErrore = "Errore: Lettura dati Causali di Magazzino - nessun dato presente"
                GetDatiCausMag = Nothing
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = "Errore: Lettura dati Causali di Magazzino - " & Ex.Message
            GetDatiCausMag = Nothing
            Exit Function
        End Try
    End Function

    'giu250320
    Public Shared Function GetDatiDestCliFor(ByVal Codice As String, ByRef strErrore As String, ByRef pDV As DataView) As ArrayList
        strErrore = ""
        pDV = Nothing
        If IsNothing(Codice) Then
            GetDatiDestCliFor = Nothing
            Exit Function
        End If
        If String.IsNullOrEmpty(Codice) Then
            GetDatiDestCliFor = Nothing
            Exit Function
        End If
        If Codice = "" Then
            GetDatiDestCliFor = Nothing
            Exit Function
        End If
        Dim strSQL As String = ""
        strSQL = "Select CONVERT(INT, Tipo) AS Sigla, * From DestClienti WHERE Codice = '" & Codice.ToString.Trim & "' ORDER BY Ragione_Sociale"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim myDestCF As DestCliForEntity
        Dim myArray As ArrayList
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                pDV = New DataView(ds.Tables(0))
                If (ds.Tables(0).Rows.Count > 0) Then
                    myArray = New ArrayList
                    For Each row In ds.Tables(0).Select()
                        myDestCF = New DestCliForEntity
                        With myDestCF
                            .Progressivo = IIf(IsDBNull(row.Item("Progressivo")), "", row.Item("Progressivo"))
                            .Ragione_Sociale = IIf(IsDBNull(row.Item("Ragione_Sociale")), "", row.Item("Ragione_Sociale"))
                            .Indirizzo = IIf(IsDBNull(row.Item("Indirizzo")), "", row.Item("Indirizzo"))
                            .Cap = IIf(IsDBNull(row.Item("Cap")), "", row.Item("Cap"))
                            .Localita = IIf(IsDBNull(row.Item("Localita")), "", row.Item("Localita"))
                            .Provincia = IIf(IsDBNull(row.Item("Provincia")), "", row.Item("Provincia"))
                            .Stato = IIf(IsDBNull(row.Item("Stato")), "", row.Item("Stato"))
                            .Denominazione = IIf(IsDBNull(row.Item("Denominazione")), "", row.Item("Denominazione"))
                            .Riferimento = IIf(IsDBNull(row.Item("Riferimento")), "", row.Item("Riferimento"))
                            .Telefono1 = IIf(IsDBNull(row.Item("Telefono1")), "", row.Item("Telefono1"))
                            .Telefono2 = IIf(IsDBNull(row.Item("Telefono2")), "", row.Item("Telefono2"))
                            .Fax = IIf(IsDBNull(row.Item("Fax")), "", row.Item("Fax"))
                            .EMail = IIf(IsDBNull(row.Item("EMail")), "", row.Item("EMail"))
                            .Tipo = IIf(IsDBNull(row.Item("Tipo")), "", row.Item("Tipo"))
                            .Ragione_Sociale35 = IIf(IsDBNull(row.Item("Ragione_Sociale35")), "", row.Item("Ragione_Sociale35"))
                            .Riferimento35 = IIf(IsDBNull(row.Item("Riferimento35")), "", row.Item("Riferimento35"))
                        End With
                        myArray.Add(myDestCF)
                    Next
                    GetDatiDestCliFor = myArray
                    Exit Function
                Else
                    GetDatiDestCliFor = Nothing
                    Exit Function
                End If
            Else
                GetDatiDestCliFor = Nothing
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = "Errore: Lettura DestClinti - " & Ex.Message
            GetDatiDestCliFor = Nothing
            Exit Function
        End Try
    End Function
#End Region

End Class
