Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WUC_ArticoliInstEmail
    Inherits System.Web.UI.UserControl

#Region "Def per gestire il GRID"
    Private aDataViewPrevT2 As DataView
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
        CodCoGe = 2
        RagSoc = 3
        Denom = 4
        DesCateg = 5
        EmailInvio = 6
        Loc = 7
        CAP = 8
        Cod_Filiale = 9
        Destinazione1 = 10
        Destinazione2 = 11
        Destinazione3 = 12
    End Enum

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
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
            aDataViewPrevT2 = Session("aDataViewPrevT2")
            GridViewPrevT.DataSource = aDataViewPrevT2
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
            aDataViewPrevT2 = Session("aDataViewPrevT2")
            GridViewPrevT.DataSource = aDataViewPrevT2
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
            aDataViewPrevT2 = Session("aDataViewPrevT2")
            aDataViewPrevT2.Sort = ""
            If aDataViewPrevT2.Count > 0 Then aDataViewPrevT2.Sort = sortExpression & " " + sortDirection
            GridViewPrevT.DataSource = aDataViewPrevT2
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

            aDataViewPrevT2 = Session("aDataViewPrevT2")

            If aDataViewPrevT2 Is Nothing Then
                Exit Sub
            End If

            'pulisco i filtri per aDataViewPrevT2
            aDataViewPrevT2.RowFilter = ""
            aDataViewPrevT2.Item(myRowIndex).Item("Selezionato") = sender.Checked

            Session("aDataViewPrevT2") = aDataViewPrevT2
            GridViewPrevT.DataSource = aDataViewPrevT2
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
        aDataViewPrevT2 = Session("aDataViewPrevT2")
        If aDataViewPrevT2 IsNot Nothing Then
            For i = 0 To aDataViewPrevT2.Count - 1
                aDataViewPrevT2.Item(i).Item("Selezionato") = True
            Next

            Session("aDataViewPrevT2") = aDataViewPrevT2
            GridViewPrevT.DataSource = aDataViewPrevT2
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
        aDataViewPrevT2 = Session("aDataViewPrevT2")
        If aDataViewPrevT2 IsNot Nothing Then
            For i = 0 To aDataViewPrevT2.Count - 1
                aDataViewPrevT2.Item(i).Item("Selezionato") = False
            Next

            Session("aDataViewPrevT2") = aDataViewPrevT2
            GridViewPrevT.DataSource = aDataViewPrevT2
            GridViewPrevT.EditIndex = -1
            GridViewPrevT.DataBind()
        End If
    End Sub

    Public Function PopolaGridT(ByVal DataScadenzaDA As DateTime, ByVal DataScadenzaA As DateTime, _
            ByVal SelScGa As Boolean, ByVal SelScEl As Boolean, ByVal SelScBa As Boolean, _
            ByVal Codice_CoGe As String, ByVal Codice_Art As String, _
            ByVal SelTutteCatCli As Boolean, ByVal SelRaggrCatCli As Boolean, _
            ByVal DescCatCli As String, ByVal CodCategoria As Integer, _
            ByVal SelCategorie As Boolean, ByVal CodCategSel As String, _
            ByVal CliSenzaMail As Boolean, ByVal CliConMail As Boolean, ByVal CliNoInvioEmail As Boolean, ByVal CliConMailErr As Boolean) As Integer
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
            If ClsPrint.BuildArtInstCliDett("", "", "", "", _
                DataScadenzaDA, DataScadenzaA, SelScGa, SelScEl, SelScBa, Codice_CoGe, Codice_Art, _
                DsPrinWebDoc, ObjReport, StrErrore, SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, _
                SelCategorie, CodCategSel, _
                CliSenzaMail, CliConMail, CliNoInvioEmail, CliConMailErr, False, False, "") = True Then
                
                'salvo in sessione il dataset ottenuto
                Session(CSTDsArticoliInstEmail) = DsPrinWebDoc

                'popolo griglia
                aDataViewPrevT2 = New DataView(DsPrinWebDoc.ArticoliInstEmail)

                GridViewPrevT.DataSource = aDataViewPrevT2
                Session("aDataViewPrevT2") = aDataViewPrevT2
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
            If Not Session(CSTDsArticoliInstEmail) Is Nothing Then
                DsPrinWebDoc = Session(CSTDsArticoliInstEmail)
                '-
                aDataViewPrevT2 = New DataView(DsPrinWebDoc.ArticoliInstEmail)
            Else
                aDataViewPrevT2 = New DataView(DsPrinWebDoc.ArticoliInstEmail)
            End If
            GridViewPrevT.DataSource = aDataViewPrevT2
            Session("aDataViewPrevT2") = aDataViewPrevT2
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
        If Session("aDataViewPrevT2") Is Nothing Then
            Return False
            Exit Function
        End If
        If GridViewPrevT.Rows.Count = 0 Then
            Return False
            Exit Function
        End If

        Return True
    End Function

    Public Function getCodCogeSelezionati() As String
        Dim CodCoge As String = ""

        If Session("aDataViewPrevT2") Is Nothing Then
            Return ""
            Exit Function
        End If

        Dim aDataViewSelezione As New DataView
        aDataViewSelezione = Session("aDataViewPrevT2")
        aDataViewSelezione.RowFilter = "Selezionato <>0"

        Dim dtCodCogeSelezionati As DataTable = aDataViewSelezione.ToTable(True, "Cod_Coge")

        If dtCodCogeSelezionati.Rows.Count > 0 Then
            For i = 0 To dtCodCogeSelezionati.Rows.Count - 1
                CodCoge &= dtCodCogeSelezionati.Rows(i).Item("Cod_Coge").ToString & ";"
            Next
            CodCoge = CodCoge.Substring(0, CodCoge.Length - 1) 'rimuovo ultimo ;
        End If

        'svuoto filtri
        aDataViewSelezione.RowFilter = ""

        Return CodCoge
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