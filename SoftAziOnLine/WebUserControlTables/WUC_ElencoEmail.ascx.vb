Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_ElencoEmail
    Inherits System.Web.UI.UserControl

#Region "Def per gestire il GRID"
    Private aDataViewPrevT3 As DataView
#End Region

#Region "Variabili private"
    Private _WucElement As Object
#End Region
#Region "Property"
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property
#End Region

#Region "Metodi private - eventi"
    Private Enum CellIdxT
        CheckSel = 1
        DesStato = 2
        DataInvio = 3
        NEmail = 4
        CodCoGe = 5
        RagSoc = 6
        Denom = 7
        Loc = 8
        CAP = 9
        Cod_Filiale = 10
        Destinazione1 = 11
        Destinazione2 = 12
        Destinazione3 = 13
    End Enum

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataInvio).Text) Then
                e.Row.Cells(CellIdxT.DataInvio).Text = Format(CDate(e.Row.Cells(CellIdxT.DataInvio).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(CellIdxT.NEmail).Text) Then
                e.Row.Cells(CellIdxT.NEmail).Text = Format(Val(e.Row.Cells(CellIdxT.NEmail).Text), "###,###").ToString
            End If
        End If
    End Sub

    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged
        Try
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
            Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
            'BtnSetByStato("")
        Catch ex As Exception
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End Try
    End Sub

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        Session(IDARTICOLOINST) = ""
        Session(CSTCODCOGE) = ""
        Session(CSTCODFILIALE) = ""
        '-
        Try
            GridViewPrevT.SelectedIndex = -1
            aDataViewPrevT3 = Session("aDataViewPrevT3")
            If aDataViewPrevT3.Count > 0 Then aDataViewPrevT3.Sort = "Cod_CoGe" 'giu311011
            GridViewPrevT.DataSource = aDataViewPrevT3
            GridViewPrevT.DataBind()
        Catch ex As Exception
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
            Exit Sub
        End Try

        If GridViewPrevT.Rows.Count > 0 Then
            GridViewPrevT.SelectedIndex = 0
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(CSTCODCOGE) = ""
                Session(CSTCODFILIALE) = ""
            End Try
        Else
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        Session(IDARTICOLOINST) = ""
        Session(CSTCODCOGE) = ""
        Session(CSTCODFILIALE) = ""
        '-
        Try
            GridViewPrevT.SelectedIndex = -1
            aDataViewPrevT3 = Session("aDataViewPrevT3")
            GridViewPrevT.DataSource = aDataViewPrevT3
            GridViewPrevT.DataBind()
        Catch ex As Exception
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
            Exit Sub
        End Try

        If GridViewPrevT.Rows.Count > 0 Then
            GridViewPrevT.SelectedIndex = 0
            Try
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(CSTCODCOGE) = ""
                Session(CSTCODFILIALE) = ""
            End Try
        Else
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridViewPrevT.Sorting
        Dim sortExpression As String = TryCast(ViewState("_GridView1LastSortExpression_"), String)
        Dim sortDirection As String = TryCast(ViewState("_GridView1LastSortDirection_"), String)

        If e.SortExpression <> sortExpression Then
            sortExpression = e.SortExpression
            sortDirection = "ASC"
        Else

            If sortDirection = "ASC" Then
                sortExpression = e.SortExpression
                sortDirection = "DESC"
            Else
                sortExpression = e.SortExpression
                sortDirection = "ASC"
            End If
        End If

        Try
            ViewState("_GridView1LastSortDirection_") = sortDirection
            ViewState("_GridView1LastSortExpression_") = sortExpression
            aDataViewPrevT3 = Session("aDataViewPrevT3")
            aDataViewPrevT3.Sort = ""
            If aDataViewPrevT3.Count > 0 Then aDataViewPrevT3.Sort = sortExpression & " " + sortDirection
            GridViewPrevT.DataSource = aDataViewPrevT3
            GridViewPrevT.DataBind()
        Catch
        End Try
    End Sub
    Protected Sub checkSel_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        '----------------------------------------------------------------------------------------
        'ATTENZIONE LE COLONNE OGGETTO DI MODIFICA SUL GRID NON DEVONO ESSERE DI TIPO READOnly
        '----------------------------------------------------------------------------------------
        Try
            'selected row gridview
            Dim cb As CheckBox = CType(sender, CheckBox)
            Dim myrow As GridViewRow = CType(cb.NamingContainer, GridViewRow)
            Dim myRowIndex As Integer = myrow.RowIndex + (GridViewPrevT.PageSize * GridViewPrevT.PageIndex)
            GridViewPrevT.SelectedIndex = myrow.RowIndex        'indice della griglia
            GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)

            aDataViewPrevT3 = Session("aDataViewPrevT3")

            If aDataViewPrevT3 Is Nothing Then
                Exit Sub
            End If

            'pulisco i filtri per aDataViewPrevT3
            aDataViewPrevT3.RowFilter = ""
            aDataViewPrevT3.Item(myRowIndex).Item("Selezionato") = sender.Checked

            Session("aDataViewPrevT3") = aDataViewPrevT3
            GridViewPrevT.DataSource = aDataViewPrevT3
            GridViewPrevT.EditIndex = -1
            GridViewPrevT.DataBind()
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore checkSel_CheckedChanged", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub

#End Region

#Region "Metodi public"
    Public Sub SelezionaTutti()
        For Each row As GridViewRow In GridViewPrevT.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = True
        Next

        'aggiorno dataview
        aDataViewPrevT3 = Session("aDataViewPrevT3")
        If aDataViewPrevT3 IsNot Nothing Then
            For i = 0 To aDataViewPrevT3.Count - 1
                aDataViewPrevT3.Item(i).Item("Selezionato") = True
            Next

            Session("aDataViewPrevT3") = aDataViewPrevT3
            GridViewPrevT.DataSource = aDataViewPrevT3
            GridViewPrevT.EditIndex = -1
            GridViewPrevT.DataBind()
        End If
    End Sub

    Public Sub DeselezionaTutti()
        For Each row As GridViewRow In GridViewPrevT.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = False
        Next

        'aggiorno dataview
        aDataViewPrevT3 = Session("aDataViewPrevT3")
        If aDataViewPrevT3 IsNot Nothing Then
            For i = 0 To aDataViewPrevT3.Count - 1
                aDataViewPrevT3.Item(i).Item("Selezionato") = False
            Next

            Session("aDataViewPrevT3") = aDataViewPrevT3
            GridViewPrevT.DataSource = aDataViewPrevT3
            GridViewPrevT.EditIndex = -1
            GridViewPrevT.DataBind()
        End If
    End Sub

    Public Sub CambiaStatoEmail(ByVal _Stato As Integer, ByVal _InviaSollecito As Boolean, ByRef _Errori As String)
        _Errori = ""
        'GIU030918
        Dim ClsDB As New DataBaseUtility
        Dim LogInvioEmail As String = ClsDB.GetLogInvioEmail(Session(ESERCIZIO))
        If Mid(LogInvioEmail.Trim, 1, 20) = "INVIO EMAIL IN CORSO" Then
            _Errori = "Attenzione, INVIO EMAIL IN CORSO ... Attendere il termine dell'invio e riprovare."
            Exit Sub
        ElseIf UCase(Mid(LogInvioEmail.Trim, 1, 6)) = "ERRORE" Then
            _Errori = "Errore, lettura Parametri generali: " & LogInvioEmail.Trim
            Exit Sub
        End If
        '----------
        Dim DesStato As String = ""
        If _Stato = -1 Then DesStato = "Invio Errato"
        If _Stato = 0 Then DesStato = "Da inviare"
        If _Stato = 1 Then DesStato = "Inviata"
        If _Stato = 2 Then DesStato = "Sollecito inviato"
        If _Stato = 3 Then DesStato = "Parz.Conclusa"
        If _Stato = 9 Then DesStato = "Annullata"
        If _Stato = 99 Then DesStato = "Conclusa"
        If DesStato.Trim = "" Then
            _Errori = "Errore in CambiaStatoEmail. Selezionare il nuovo Stato E-mail da assegnare - Stato (" & _Stato.ToString & ")"
            Exit Sub
        End If

        Dim strSQL As String = ""
        'aggiorno dataview
        Try
            aDataViewPrevT3 = Session("aDataViewPrevT3")
            If aDataViewPrevT3 IsNot Nothing Then
                For i = 0 To aDataViewPrevT3.Count - 1
                    If aDataViewPrevT3.Item(i).Item("Selezionato") = True And aDataViewPrevT3.Item(i).Item("Stato") <> _Stato Then
                        aDataViewPrevT3.Item(i).Item("Stato") = _Stato
                        aDataViewPrevT3.Item(i).Item("DesStato") = DesStato
                        'Aggiorno il DB
                        If _Stato <> 0 Then
                            strSQL = "Update EmailInviateT Set Stato=" & _Stato.ToString.Trim & ", OLDStato=Stato " & _
                                     "Where Id = " & aDataViewPrevT3.Item(i).Item("IDEmailInviateT").ToString.Trim
                            If ClsDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL) = False Then
                                _Errori = "Errore in CambiaStatoEmail.1 (ExecuteQueryUpdate: " & strSQL.Trim
                                Exit Sub
                            End If
                            '0 DA INVIARE: RE-INVIO / INVIA SOLLECITO
                        ElseIf _InviaSollecito = True Then
                            If ReInvia_EmailInviateT(aDataViewPrevT3.Item(i).Item("IDEmailInviateT"), _Errori) = False Then
                                _Errori = "Errore in CambiaStatoEmail.2 (ExecuteQueryUpdate: " & _Errori.Trim
                                Exit Sub
                            End If
                            '-2 NEGATIVO FINO A QUANDO NON INVIA IL SOLLECITO
                            strSQL = "Update EmailInviateT Set Stato=-2, OLDStato=Stato " & _
                                     "Where Id = " & aDataViewPrevT3.Item(i).Item("IDEmailInviateT").ToString.Trim
                            If ClsDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL) = False Then
                                _Errori = "Errore in CambiaStatoEmail.3 (ExecuteQueryUpdate: " & strSQL.Trim
                                Exit Sub
                            End If
                        Else 'RE-INVIO
                            strSQL = "Update EmailInviateT Set Stato=" & _Stato.ToString.Trim & ", OLDStato=Stato " & _
                                    "Where Id = " & aDataViewPrevT3.Item(i).Item("IDEmailInviateT").ToString.Trim
                            If ClsDB.ExecuteQueryUpdate(TipoDB.dbScadenzario, strSQL) = False Then
                                _Errori = "Errore in CambiaStatoEmail.4 (ExecuteQueryUpdate: " & strSQL.Trim
                                Exit Sub
                            End If
                        End If
                    End If
                Next

                Session("aDataViewPrevT3") = aDataViewPrevT3
                GridViewPrevT.DataSource = aDataViewPrevT3
                GridViewPrevT.EditIndex = -1
                GridViewPrevT.DataBind()
            End If
            ClsDB = Nothing
        Catch ex As Exception
            _Errori = "Errore in CambiaStatoEmail: " & ex.Message.Trim
            Exit Sub
        End Try

    End Sub
    'GIU270718 giu301018 sposto da DataBaseUtility
    Private Function ReInvia_EmailInviateT(ByVal _IdEmailInviateT As Long, ByRef strErrore As String) As Boolean
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConnSC = New System.Data.SqlClient.SqlConnection
        '-
        Dim SqlUpd = New System.Data.SqlClient.SqlCommand
        '
        'SqlConnection1
        '
        SqlConnSC.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
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
        '---------------------------
        Try
            If SqlConnSC.State <> ConnectionState.Open Then
                SqlConnSC.Open()
            End If
            '--- Parametri
            SqlUpd.CommandText = "ReInvia_EmailInviateT"
            SqlUpd.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd.Connection = SqlConnSC
            SqlUpd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "ID", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RetVal", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '
            SqlUpd.Parameters("@ID").Value = _IdEmailInviateT
            SqlUpd.CommandTimeout = myTimeOUT '5000
            SqlUpd.ExecuteNonQuery()
            If SqlUpd.Parameters.Item("@RetVal").Value = -1 Then
                strErrore = "Errore nell'aggiornamento (SQL -1)!!!"
                Return False
            End If
            If SqlConnSC.State <> ConnectionState.Closed Then
                SqlConnSC.Close()
            End If

            Return True
        Catch exSQL As System.Data.SqlClient.SqlException
            strErrore = exSQL.Message
            Return False
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        End Try
    End Function

    Public Function PopolaGridT(ByVal DataEmailDA As DateTime, ByVal DataEmailA As DateTime, _
            ByVal SelScGa As Boolean, ByVal SelScEl As Boolean, ByVal SelScBa As Boolean, _
            ByVal Codice_CoGe As String, ByVal StatoEmail As Integer, _
            ByVal SelTutteCatCli As Boolean, ByVal SelRaggrCatCli As Boolean, _
            ByVal DescCatCli As String, ByVal CodCategoria As Integer, _
            ByVal SelCategorie As Boolean, ByVal CodCategSel As String, _
            ByVal DalNumero As Integer, ByVal AlNumero As Integer) As Integer
        '-
        Dim CountGrid As Integer = -1
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        '-
        Try
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
            DsPrinWebDoc.Clear() 'giu260618 qui ok tanto dopo carica i dettagli
            If ClsPrint.BuildEmailCliDett("", "", "", "", _
                DataEmailDA, DataEmailA, SelScGa, SelScEl, SelScBa, Codice_CoGe, StatoEmail, _
                DsPrinWebDoc, ObjReport, StrErrore, SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                SelCategorie, CodCategSel, DalNumero, AlNumero, "", False, False, "") = True Then

                'salvo in sessione il dataset ottenuto
                Session(CSTDsEmailInviate) = DsPrinWebDoc

                'popolo griglia
                aDataViewPrevT3 = New DataView(DsPrinWebDoc.ArticoliInstEmail)
                If aDataViewPrevT3.Count > 0 Then aDataViewPrevT3.Sort = "Cod_CoGe"
                GridViewPrevT.DataSource = aDataViewPrevT3
                Session("aDataViewPrevT3") = aDataViewPrevT3
                GridViewPrevT.DataBind()

                If GridViewPrevT.Rows.Count = 0 Then
                    'disabilito controlli o aggiungo riga vuota
                    WucElement.SetControl(False)
                Else
                    GridViewPrevT.SelectedIndex = 0 'seleziono la prima riga
                    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                    Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                    'abilito controlli
                    WucElement.SetControl(True)
                End If
                CountGrid = GridViewPrevT.Rows.Count
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

        Return CountGrid 'ritorno n. elementi griglia
    End Function

    Public Sub DatiSessioneGriglia()
        'caricamento grid default--Sim010618 'GIU250618
        Try
            Dim DsPrinWebDoc As New DSPrintWeb_Documenti
            If Not Session(CSTDsEmailInviate) Is Nothing Then
                DsPrinWebDoc = Session(CSTDsEmailInviate)
                '-
                aDataViewPrevT3 = New DataView(DsPrinWebDoc.ArticoliInstEmail)
            Else
                aDataViewPrevT3 = New DataView(DsPrinWebDoc.ArticoliInstEmail)
            End If
            If aDataViewPrevT3.Count > 0 Then aDataViewPrevT3.Sort = "Cod_CoGe"
            GridViewPrevT.DataSource = aDataViewPrevT3
            Session("aDataViewPrevT3") = aDataViewPrevT3
            GridViewPrevT.DataBind()
            Session(CSTCODCOGE) = ""
            Session(CSTCODFILIALE) = ""
            If GridViewPrevT.Rows.Count = 0 Then
                'disabilito controlli o aggiungo riga vuota
                WucElement.SetControl(False)
            Else
                GridViewPrevT.SelectedIndex = 0 'seleziono la prima riga
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTCODCOGE) = row.Cells(CellIdxT.CodCoGe).Text.Trim
                Session(CSTCODFILIALE) = row.Cells(CellIdxT.Cod_Filiale).Text.Trim
                'abilito controlli
                WucElement.SetControl(True)
            End If
            '---
        Catch ex As Exception

        End Try

    End Sub
    Public Sub SvuotaGriglia()
        GridViewPrevT.DataSource = Nothing
        GridViewPrevT.DataBind()
    End Sub

    Public Function CheckGrid() As Boolean
        If Session("aDataViewPrevT3") Is Nothing Then
            Return False
            Exit Function
        End If
        If GridViewPrevT.Rows.Count = 0 Then
            Return False
            Exit Function
        End If

        Return True
    End Function

    Public Function getSelDalNAlN() As String
        Dim SelDalNAlN As String = ""

        If Session("aDataViewPrevT3") Is Nothing Then
            Return ""
            Exit Function
        End If

        Dim aDataViewSelezione As New DataView
        aDataViewSelezione = Session("aDataViewPrevT3")
        aDataViewSelezione.RowFilter = "Selezionato <>0"
        If aDataViewSelezione.Count > 0 Then aDataViewSelezione.Sort = "NEmail"

        Dim dtCodCogeSelezionati As DataTable = aDataViewSelezione.ToTable(True, "NEmail")

        If dtCodCogeSelezionati.Rows.Count > 0 Then
            For i = 0 To dtCodCogeSelezionati.Rows.Count - 1
                SelDalNAlN &= dtCodCogeSelezionati.Rows(i).Item("NEmail").ToString & ";"
            Next
            SelDalNAlN = SelDalNAlN.Substring(0, SelDalNAlN.Length - 1) 'rimuovo ultimo ;
        End If

        'svuoto filtri
        aDataViewSelezione.RowFilter = ""

        Return SelDalNAlN
    End Function

    Public Function ShowModificaMail() As Boolean
        ShowModificaMail = False
        If GridViewPrevT.SelectedIndex < 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna anagrafica selezionata.", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If

        Dim myIDCOGE As String = Session(CSTCODCOGE)
        If IsNothing(myIDCOGE) Then
            myIDCOGE = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna anagrafica selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        If String.IsNullOrEmpty(myIDCOGE) Or Not IsNumeric(myIDCOGE.Trim) Then
            myIDCOGE = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessuna anagrafica selezionata", WUC_ModalPopup.TYPE_ALERT)
            Exit Function
        End If
        Session(IDANAGRCLIFOR) = myIDCOGE
        Session(CSTCODCOGE) = myIDCOGE

        Dim strSQL As String = ""
        strSQL = "SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = " & myIDCOGE & ") ORDER BY [Rag_Soc]"

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim row() As DataRow

        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    row = ds.Tables(0).Select()
                    Dim Rk As StrAnagrCliFor
                    Rk.Rag_Soc = row(0).Item("Rag_Soc").ToString
                    Rk.Denominazione = row(0).Item("Denominazione").ToString
                    Rk.Riferimento = row(0).Item("Riferimento").ToString
                    Rk.Codice_Fiscale = row(0).Item("Codice_Fiscale").ToString
                    Rk.Partita_IVA = row(0).Item("Partita_IVA").ToString
                    Rk.Indirizzo = row(0).Item("Indirizzo").ToString + " " + row(0).Item("NumeroCivico").ToString
                    Rk.NumeroCivico = ""
                    Rk.Cap = row(0).Item("CAP").ToString
                    Rk.Localita = row(0).Item("Localita").ToString
                    Rk.Provincia = row(0).Item("Provincia").ToString.Trim
                    Rk.Nazione = row(0).Item("Nazione").ToString.Trim
                    Rk.Telefono1 = row(0).Item("Telefono1").ToString.Trim
                    Rk.Telefono2 = row(0).Item("Telefono2").ToString.Trim
                    Rk.Fax = row(0).Item("Fax").ToString.Trim
                    Rk.Regime_Iva = row(0).Item("Regime_Iva").ToString.Trim
                    Rk.EMail = row(0).Item("EMail").ToString.Trim 'giu070514
                    Rk.EMailInvioScad = row(0).Item("EmailInvioScad").ToString.Trim 'sim180518
                    Rk.InvioMailScad = row(0).Item("InvioMailScad")
                    Rk.PECEMail = row(0).Item("PECEMail").ToString.Trim 'giu190122
                    Session(RKANAGRCLIFOR) = Rk
                Else
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Codice non trovato in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        '-ok trovato

        'BLOCCO PER LA DESTINAZIONE CLIENTE --SIMONE210518
        Try
            Dim ObjDBDest As New DataBaseUtility
            Dim ds1 As New DataSet
            Dim rowDest() As DataRow
            Session(RKANAGRDESTCLI) = Nothing
            Dim Progressivo As String = Session(CSTCODFILIALE) 'giu200618 GridViewPrevT.SelectedRow.Cells(CellIdxT.Cod_Filiale).Text.Trim 'CodiceFiliale
            Dim CodiceCoge As String = myIDCOGE.Trim

            If IsNumeric(Progressivo) = True And IsNumeric(CodiceCoge) = True And Progressivo <> "" And CodiceCoge <> "" Then
                strSQL = "SELECT * FROM [DestClienti] WHERE ([Codice] = " & CodiceCoge & ") AND ([Progressivo] = " & Progressivo & ")"

                Try
                    ObjDBDest.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds1)
                    ObjDBDest = Nothing
                    If (ds1.Tables.Count > 0) Then
                        If (ds1.Tables(0).Rows.Count > 0) Then
                            rowDest = ds1.Tables(0).Select()
                            Dim Rk1 As New DestCliForEntity
                            Rk1.Progressivo = rowDest(0).Item("Progressivo").ToString
                            Rk1.Ragione_Sociale = rowDest(0).Item("Ragione_Sociale").ToString
                            Rk1.Denominazione = rowDest(0).Item("Denominazione").ToString
                            Rk1.Riferimento = rowDest(0).Item("Riferimento").ToString
                            Rk1.Indirizzo = rowDest(0).Item("Indirizzo").ToString
                            Rk1.Cap = rowDest(0).Item("CAP").ToString
                            Rk1.Localita = rowDest(0).Item("Localita").ToString
                            Rk1.Provincia = rowDest(0).Item("Provincia").ToString.Trim
                            Rk1.Stato = rowDest(0).Item("Stato").ToString.Trim
                            Rk1.Telefono1 = rowDest(0).Item("Telefono1").ToString.Trim
                            Rk1.Telefono2 = rowDest(0).Item("Telefono2").ToString.Trim
                            Rk1.Fax = rowDest(0).Item("Fax").ToString.Trim
                            Rk1.EMail = rowDest(0).Item("EMail").ToString.Trim

                            Session(RKANAGRDESTCLI) = Rk1
                        Else
                            Session(RKANAGRDESTCLI) = Nothing
                        End If
                    Else
                        Session(RKANAGRDESTCLI) = Nothing
                    End If
                Catch Ex As Exception
                    Session(RKANAGRDESTCLI) = Nothing
                End Try
            Else
                Session(RKANAGRDESTCLI) = Nothing
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Filiale non trovata in archivio", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        '----
        ShowModificaMail = True

        WUC_Anagrafiche_ModifySint1.PopolaCampi()
        WUC_Anagrafiche_ModifySint1.Show()

    End Function
#End Region

    
End Class