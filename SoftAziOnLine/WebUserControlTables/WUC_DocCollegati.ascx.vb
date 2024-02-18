Imports It.SoftAzi.Model.Facade
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient

Partial Public Class WUC_DocCollegati
    Inherits System.Web.UI.UserControl

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
        Stato = 0
        NumDoc = 1
        Tipo = 2
        FCPA = 3
        DataDoc = 4
        DataCons = 5
        CodCliForProvv = 6
        RagSoc = 7
        Denominazione = 8
        Localita = 9
        CAP = 10
        PIVA = 11
        CF = 12
        Riferimento = 13
    End Enum
    'GIU270721
    Private Enum CellIdxTCM
        Stato = 0
        NumDoc = 1
        RevN = 2
        DataDoc = 3
        DataAccetta = 4

        CodCliForProvv = 5
        RagSoc = 6
        Denom = 7
        Loc = 8
        CAP = 9
        PIVA = 10
        CF = 11
        '------ ....
        DataInizio = 12
        DataFine = 13
        Riferimento = 14
        Dest1 = 15
        Dest2 = 16
        Dest3 = 17
        DurataTipo = 18
        DurataNum = 19
    End Enum
    Private Enum CellIdxD
        Stato = 1
        NumDoc = 2
        Tipo = 3
        FCPA = 4
        DataDoc = 5
        DataCons = 6
        CodCliForProvv = 7
        RagSoc = 8
        Denominazione = 9
        Localita = 10
        CAP = 11
        PIVA = 12
        CF = 13
        Riferimento = 14
    End Enum
    'GIU170419
    Private Enum CellIdxDPR
        CodArt = 0
        DesArt = 1
        UM = 2
        Qta = 3
        IVA = 4
        Prz = 5
        ScV = 6
        Sc1 = 7
        Importo = 8
        ScR = 9
    End Enum
    Private Enum CellIdxDettOC
        CodArt = 0
        DesArt = 1
        UM = 2
        Qta = 3
        QtaEv = 4
        QtaIm = 5
        QtaRe = 6
        QtaAl = 7 'Inviata
        IVA = 8
        Prz = 9
        ScV = 10
        Sc1 = 11
        Importo = 12
        ScR = 13
    End Enum
    Private Enum CellIdxDDTFC
        CodArt = 0
        DesArt = 1
        UM = 2
        QtaOr = 3
        QtaEv = 4
        QtaRe = 5
        QtaAl = 6
        IVA = 7
        Prz = 8
        TipoSc = 9
        ScV = 10
        Sc1 = 11
        Importo = 12
        Ded = 13
        ScR = 14
        PAge = 15
        ImpProvvAge = 16
    End Enum
    'giu260122
    Private Enum CellIdxCA
        TipoDett = 0
        TipoDettR = 1
        CodArt = 2
        DesArt = 3
        SerieLotto = 4
        UM = 5
        Qta = 6
        QtaEv = 7
        QtaIn = 8
        QtaRe = 9
        Filiale = 10
        DataSc = 11
        IVA = 12
        Prz = 13
        ScV = 14
        Sc1 = 15
        Importo = 16
        ScR = 17
    End Enum
    'giu011218
    Dim myNumDoc As String
    Dim myTipo As String
    Dim MyDataDoc As String 'giu170419

    Private Sub GridViewDocT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDocT.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
                e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            End If
        End If
    End Sub
    'GIU270721
    Private Sub GridViewPrevTCM_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevTCM.RowDataBound
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxTCM.DataDoc).Text) Then
                e.Row.Cells(CellIdxTCM.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxTCM.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxTCM.DataAccetta).Text) Then
                e.Row.Cells(CellIdxTCM.DataAccetta).Text = Format(CDate(e.Row.Cells(CellIdxTCM.DataAccetta).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxTCM.DataInizio).Text) Then
                e.Row.Cells(CellIdxTCM.DataInizio).Text = Format(CDate(e.Row.Cells(CellIdxTCM.DataInizio).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxTCM.DataFine).Text) Then
                e.Row.Cells(CellIdxTCM.DataFine).Text = Format(CDate(e.Row.Cells(CellIdxTCM.DataFine).Text), FormatoData).ToString
            End If
            'giu031219
            ' ''If IsNumeric(e.Row.Cells(CellIdxTCM.PercImp).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxTCM.PercImp).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxTCM.PercImp).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxTCM.PercImp).Text), 2).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxTCM.PercImp).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxTCM.PercImPorto).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxTCM.PercImPorto).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxTCM.PercImPorto).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxTCM.PercImPorto).Text), 2).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxTCM.PercImPorto).Text = ""
            ' ''    End If
            ' ''End If
            ' ''If IsDate(e.Row.Cells(CellIdxTCM.DataRif).Text) Then
            ' ''    e.Row.Cells(CellIdxTCM.DataRif).Text = Format(CDate(e.Row.Cells(CellIdxTCM.DataRif).Text), FormatoData).ToString
            ' ''End If
            'giu160520 per evitare di superare le 3 righe per ciascuna scadenza
            If Len(e.Row.Cells(CellIdxTCM.RagSoc).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxTCM.RagSoc).Text = Mid(e.Row.Cells(CellIdxTCM.RagSoc).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxTCM.Denom).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxTCM.Denom).Text = Mid(e.Row.Cells(CellIdxTCM.Denom).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxTCM.Riferimento).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxTCM.Riferimento).Text = Mid(e.Row.Cells(CellIdxTCM.Riferimento).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxTCM.Dest1).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxTCM.Dest1).Text = Mid(e.Row.Cells(CellIdxTCM.Dest1).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxTCM.Dest2).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxTCM.Dest2).Text = Mid(e.Row.Cells(CellIdxTCM.Dest2).Text, 1, 20)
            End If
            If Len(e.Row.Cells(CellIdxTCM.Dest3).Text.Trim) > 20 Then
                e.Row.Cells(CellIdxTCM.Dest3).Text = Mid(e.Row.Cells(CellIdxTCM.Dest3).Text, 1, 20)
            End If
        End If
    End Sub
    'GIU011218
    Private Sub BtnSetEnabledTo(ByVal Abilita As Boolean)
    End Sub
    Private Sub GridViewDocD_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewDocD.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewDocD.PageIndex = 0
        Else
            GridViewDocD.PageIndex = e.NewPageIndex
        End If
        GridViewDocD.SelectedIndex = -1
        Session(IDDOCUMCOLL) = "" : Session(DATADOCCOLL) = ""
        GridViewDocD.DataBind()
        If GridViewDocD.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewDocD.SelectedIndex = 0
            Try
                Session(IDDOCUMCOLL) = GridViewDocD.SelectedDataKey.Value
                'Dim row As GridViewRow = GridViewDocD.SelectedRow
                GridViewDocD_SelectedIndexChanged(GridViewDocD, Nothing)
            Catch ex As Exception
                Session(IDDOCUMCOLL) = "" : Session(DATADOCCOLL) = ""
                Session(TIPODOCCOLL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMCOLL) = "" : Session(DATADOCCOLL) = ""
        End If
    End Sub
    Private Sub GridViewDocD_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewDocD.SelectedIndexChanged
        myNumDoc = "" : myTipo = "" : MyDataDoc = ""
        Try
            Session(IDDOCUMCOLL) = GridViewDocD.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewDocD.SelectedRow
            myNumDoc = row.Cells(CellIdxD.NumDoc).Text.Trim
            myTipo = row.Cells(CellIdxD.Tipo).Text.Trim
            Session(TIPODOCCOLL) = myTipo 'giu230419
            MyDataDoc = row.Cells(CellIdxD.DataDoc).Text.Trim
            Session(DATADOCCOLL) = MyDataDoc
            'GIU170419
            Panel1DettPR.Visible = False
            Panel1DettOC.Visible = False
            PanelDettDTFC.Visible = False
            PanelDettCA.Visible = False
            If myTipo = "PR" Then
                Panel1DettPR.Visible = True
            ElseIf myTipo = "OC" Then
                Panel1DettOC.Visible = True
            ElseIf myTipo = "DT" Then
                PanelDettDTFC.Visible = True
            ElseIf myTipo = "FC" Then
                PanelDettDTFC.Visible = True
            ElseIf myTipo = "NC" Then
                PanelDettDTFC.Visible = True
            ElseIf myTipo = "CA" Then 'giu260122
                PanelDettCA.Visible = True
            Else
                PanelDettDTFC.Visible = True
            End If
            '---------
            If myTipo = "CA" Then
                PopolaGridViewDettCA()
            Else
                PopolaGridViewDettOC()
            End If

        Catch ex As Exception
            Session(IDDOCUMCOLL) = "" : Session(DATADOCCOLL) = ""
        End Try
        lblMessUtenteDett.Text = "Dettaglio documento selezionato: (" & Trim(myTipo) & ") " & DesTD(myTipo) & " N° " & Trim(myNumDoc) & " del " & MyDataDoc
        _WucElement.SetLblMessUtente("")
    End Sub
    'END GIU011218
    Private Sub GridViewDocD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDocD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxD.DataDoc).Text) Then
                e.Row.Cells(CellIdxD.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxD.DataDoc).Text), FormatoData).ToString
            End If
            '-
            If e.Row.Cells(CellIdxD.Tipo).Text.Trim <> "PR" And e.Row.Cells(CellIdxD.Tipo).Text.Trim <> "OC" And e.Row.Cells(CellIdxD.Tipo).Text.Trim <> "DT" Then
                e.Row.Cells(CellIdxD.DataCons).Text = ""
            ElseIf IsDate(e.Row.Cells(CellIdxD.DataCons).Text) Then
                e.Row.Cells(CellIdxD.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxD.DataCons).Text), FormatoData).ToString
            End If
        End If
    End Sub

    'GIU170419
    Private Sub PopolaGridViewDettOC()
        Dim myID As String = Session(IDDOCUMCOLL)
        If IsNothing(myID) Then
            myID = ""
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Exit Sub
        End If
        '-
        Dim MyAnno As String = ""
        If IsDate(MyDataDoc.Trim) Then
            MyAnno = Format(CDate(MyDataDoc).Year, "0000")
        End If
        '-
        Dim TipoDoc As String = Session(TIPODOCCOLL)
        If IsNothing(TipoDoc) Then
            TipoDoc = ""
            Exit Sub
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            TipoDoc = ""
        End If
        If TipoDoc.Trim = "" Then
            lblMessUtenteDett.Text = "Tipo Documento sconosciuto: (" & Trim(myTipo) & ") " 
            Exit Sub
        ElseIf Left(TipoDoc, 1) = "O" Or Left(TipoDoc, 1) = "P" Then
            MyAnno = "" 'GIU230419 SOLO PER GLI ORDINI E PREVENTIVI LEGGERE SEMPRE L'ANNO CORRENTE ALTRIMENTI LE MODIFICHE NON SONO LE ULTIME
        End If
        '-------
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConnDoc As New SqlConnection
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi, myAnno)
        'giu200617
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
        'DETTAGLI
        Try
            Dim dsD As New DataSet
            Dim SqlAdapDocColl As New SqlDataAdapter
            Dim SqlDbSelectDocColl As New SqlCommand
            SqlDbSelectDocColl.CommandText = "get_PrevDByIDDocumenti"
            SqlDbSelectDocColl.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDocColl.Connection = SqlConnDoc
            SqlDbSelectDocColl.CommandTimeout = myTimeOUT
            SqlDbSelectDocColl.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocColl.SelectCommand = SqlDbSelectDocColl
            '==============CARICAMENTO==============
            SqlDbSelectDocColl.Parameters.Item("@IDDocumenti").Value = IIf(myID.Trim = "", -1, myID.Trim)
            SqlAdapDocColl.Fill(dsD)
            If myTipo = "PR" Then
                GridViewDettPR.DataSource = dsD
                GridViewDettPR.DataBind()
            ElseIf myTipo = "OC" Then
                GridViewDettOC.DataSource = dsD
                GridViewDettOC.DataBind()
            ElseIf myTipo = "DT" Then
                GridViewDettDTFC.DataSource = dsD
                GridViewDettDTFC.DataBind()
            ElseIf myTipo = "FC" Then
                GridViewDettDTFC.DataSource = dsD
                GridViewDettDTFC.DataBind()
            ElseIf myTipo = "NC" Then
                GridViewDettDTFC.DataSource = dsD
                GridViewDettDTFC.DataBind()
            ElseIf myTipo = "CA" Then
                GridViewDettCA.DataSource = dsD
                GridViewDettCA.DataBind()
            Else
                GridViewDettDTFC.DataSource = dsD
                GridViewDettDTFC.DataBind()
            End If
        Catch ExSQL As SqlException
            lblMessUtenteDett.Text = "Errore SQL Caricamento documenti collegati DETTAGLIO RIGHE: " & ExSQL.Message
            _WucElement.SetLblMessUtente("")
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore SQL", "Caricamento documenti collegati DETTAGLIO RIGHE: " & ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            lblMessUtenteDett.Text = "Errore Ex Caricamento documenti collegati DETTAGLIO RIGHE: " & Ex.Message
            _WucElement.SetLblMessUtente("")
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore", "Caricamento documenti collegati DETTAGLIO RIGHE: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub
    'giu260122
    Private Sub PopolaGridViewDettCA()
        Dim myID As String = Session(IDDOCUMCOLL)
        If IsNothing(myID) Then
            myID = ""
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Exit Sub
        End If
        If Not IsNumeric(myID.Trim) Then
            myID = ""
            Exit Sub
        Else
            Dim myIDLong As Long
            myIDLong = CLng(myID.Trim)
            If myIDLong < 0 Then
                myIDLong = myIDLong * -1
            End If
            myID = CStr(myIDLong)
        End If
        '-
        Dim TipoDoc As String = Session(TIPODOCCOLL)
        If IsNothing(TipoDoc) Then
            TipoDoc = ""
            Exit Sub
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            TipoDoc = ""
        End If
        If TipoDoc.Trim = "" Then
            lblMessUtenteDett.Text = "Tipo Documento sconosciuto: (" & Trim(myTipo) & ") "
            Exit Sub
        End If
        '-------
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConnDoc As New SqlConnection
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        'giu200617
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
        'DETTAGLI
        Try
            Dim dsD As New DataSet
            Dim SqlAdapDocColl As New SqlDataAdapter
            Dim SqlDbSelectDocColl As New SqlCommand
            SqlDbSelectDocColl.CommandText = "get_ConDByIDDocElenco"
            SqlDbSelectDocColl.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDocColl.Connection = SqlConnDoc
            SqlDbSelectDocColl.CommandTimeout = myTimeOUT
            SqlDbSelectDocColl.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocColl.SelectCommand = SqlDbSelectDocColl
            '==============CARICAMENTO==============
            SqlDbSelectDocColl.Parameters.Item("@IDDocumenti").Value = IIf(myID.Trim = "", -1, myID.Trim)
            SqlAdapDocColl.Fill(dsD)
            GridViewDettCA.DataSource = dsD
            GridViewDettCA.DataBind()
        Catch ExSQL As SqlException
            lblMessUtenteDett.Text = "Errore SQL Caricamento documenti collegati (CA) DETTAGLIO RIGHE: " & ExSQL.Message
            _WucElement.SetLblMessUtente("")
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore SQL", "Caricamento documenti collegati DETTAGLIO RIGHE: " & ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            lblMessUtenteDett.Text = "Errore Ex Caricamento documenti collegati (CA) DETTAGLIO RIGHE: " & Ex.Message
            _WucElement.SetLblMessUtente("")
            ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''ModalPopup.Show("Errore", "Caricamento documenti collegati DETTAGLIO RIGHE: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
    End Sub
    Private Sub GridViewDettPR_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDettPR.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxDPR.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxDPR.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxDPR.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDPR.Qta).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxDPR.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDPR.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxDPR.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxDPR.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDPR.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDPR.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDPR.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxDPR.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxDPR.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDPR.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxDPR.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDPR.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxDPR.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxDPR.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDPR.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDPR.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDPR.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxDPR.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxDPR.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDPR.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxDPR.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDPR.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxDPR.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxDPR.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDPR.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDPR.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDPR.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxDPR.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxDPR.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDPR.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDPR.ScR).Text = ""
                End If
            End If
            'If IsDate(e.Row.Cells(6).Text) Then
            '    e.Row.Cells(6).Text = Format(CDate(e.Row.Cells(6).Text), FormatoData).ToString
            'End If
        End If
    End Sub
    Private Sub GridViewDettOC_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDettOC.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxDettOC.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.QtaIm).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.QtaIm).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.QtaIm).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.QtaIm).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.QtaIm).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.QtaAl).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.QtaAl).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.QtaAl).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.QtaAl).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.QtaAl).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDettOC.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxDettOC.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxDettOC.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDettOC.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDettOC.ScR).Text = ""
                End If
            End If
            'If IsDate(e.Row.Cells(6).Text) Then
            '    e.Row.Cells(6).Text = Format(CDate(e.Row.Cells(6).Text), FormatoData).ToString
            'End If
        End If
    End Sub
    Private Sub GridViewDettDTFC_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDettDTFC.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.QtaOr).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.QtaOr).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.QtaOr).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.QtaOr).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.QtaOr).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.QtaEv).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.QtaRe).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.QtaAl).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.QtaAl).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.QtaAl).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.QtaAl).Text), -1).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.QtaAl).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.Importo).Text = ""
                End If
            End If
            If e.Row.Cells(CellIdxDDTFC.Ded).Text.Trim <> "True" Then
                e.Row.Cells(CellIdxDDTFC.Ded).Text = ""
            Else
                e.Row.Cells(CellIdxDDTFC.Ded).Text = "SI"
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.ScR).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.PAge).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.PAge).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.PAge).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.PAge).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.PAge).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxDDTFC.ImpProvvAge).Text) Then
                If CDec(e.Row.Cells(CellIdxDDTFC.ImpProvvAge).Text) <> 0 Then
                    e.Row.Cells(CellIdxDDTFC.ImpProvvAge).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxDDTFC.ImpProvvAge).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxDDTFC.ImpProvvAge).Text = ""
                End If
            End If
        End If
    End Sub
    'giu260122
    Private Sub GridViewDettCA_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewDettCA.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxCA.Qta).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.Qta).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.Qta).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxCA.Qta).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxCA.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.QtaIn).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.QtaIn).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.QtaIn).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.QtaIn).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxCA.QtaIn).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxCA.QtaRe).Text = ""
                End If
            End If
            ' ''If IsNumeric(e.Row.Cells(CellIdxCA.QtaFa).Text) Then
            ' ''    If CDec(e.Row.Cells(CellIdxCA.QtaFa).Text) <> 0 Then
            ' ''        e.Row.Cells(CellIdxCA.QtaFa).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.QtaFa).Text), 0).ToString
            ' ''    Else
            ' ''        e.Row.Cells(CellIdxCA.QtaFa).Text = ""
            ' ''    End If
            ' ''End If
            If Len(e.Row.Cells(CellIdxCA.Filiale).Text.Trim) > 5 Then
                e.Row.Cells(CellIdxCA.Filiale).Text = Mid(e.Row.Cells(CellIdxCA.Filiale).Text, 1, 5)
            End If
            If IsDate(e.Row.Cells(CellIdxCA.DataSc).Text) Then
                e.Row.Cells(CellIdxCA.DataSc).Text = Format(CDate(e.Row.Cells(CellIdxCA.DataSc).Text), FormatoData).ToString
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxCA.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxCA.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxCA.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxCA.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxCA.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxCA.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxCA.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxCA.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxCA.ScR).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxCA.ScR).Text = ""
                End If
            End If
            'If IsDate(e.Row.Cells(6).Text) Then
            '    e.Row.Cells(6).Text = Format(CDate(e.Row.Cells(6).Text), FormatoData).ToString
            'End If
        End If
    End Sub
#End Region

#Region "Metodi public"
    Public Sub PopolaGriglia()
        PanelTestata.Visible = True
        PanelTestataCM.Visible = False
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConnDoc As New SqlConnection
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        'giu200617
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
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        'TETSTATA
        Try
            Dim dsT As New DataSet
            Dim SqlAdapDocT As New SqlDataAdapter
            Dim SqlDbSelectDocT As New SqlCommand
            SqlDbSelectDocT.CommandText = "get_DocCollegatiByID"
            SqlDbSelectDocT.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDocT.Connection = SqlConnDoc
            SqlDbSelectDocT.CommandTimeout = myTimeOUT
            SqlDbSelectDocT.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocT.SelectCommand = SqlDbSelectDocT
            '==============CARICAMENTO e MERGE DATASET==============
            SqlDbSelectDocT.Parameters.Item("@IDDocumenti").Value = Session(IDDOCUMCOLLCALL).ToString
            SqlAdapDocT.Fill(dsT)
            GridViewDocT.DataSource = dsT
            GridViewDocT.DataBind()
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", "Caricamento documento(T): " & ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento documento(T): " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'DETTAGLI
        Try
            Dim dsD As New DataSet
            Dim SqlAdapDocColl As New SqlDataAdapter
            Dim SqlDbSelectDocColl As New SqlCommand
            SqlDbSelectDocColl.CommandText = "get_DocumentiCollegati"
            SqlDbSelectDocColl.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDocColl.Connection = SqlConnDoc
            SqlDbSelectDocColl.CommandTimeout = myTimeOUT
            SqlDbSelectDocColl.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocColl.SelectCommand = SqlDbSelectDocColl
            '==============CARICAMENTO e MERGE DATASET==============
            SqlDbSelectDocColl.Parameters.Item("@IDDocumenti").Value = Session(IDDOCUMCOLLCALL).ToString
            SqlAdapDocColl.Fill(dsD)
            '-Non server perchè la SP restituisce una sola SELECT
            ' ''For i = 1 To dsD.Tables.Count - 1
            ' ''    dsD.Tables(0).Merge(dsD.Tables(i))
            ' ''Next
            GridViewDocD.DataSource = dsD
            GridViewDocD.DataBind()
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", "Caricamento documenti collegati: " & ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento documenti collegati: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'GIU280721
        GridViewDettPR.DataSource = Nothing
        GridViewDettPR.DataBind()
        GridViewDettOC.DataSource = Nothing
        GridViewDettOC.DataBind()
        GridViewDettDTFC.DataSource = Nothing
        GridViewDettDTFC.DataBind()
        'giu250122
        GridViewDettCA.DataSource = Nothing
        GridViewDettCA.DataBind()
        ',,,,,,,,
        'GIU011218
        GridViewDocD.SelectedIndex = -1
        Session(IDDOCUMCOLL) = "" : Session(DATADOCCOLL) = ""
        myNumDoc = "" : myTipo = ""
        If GridViewDocD.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewDocD.SelectedIndex = 0
        End If
        GridViewDocD_SelectedIndexChanged(GridViewDocD, Nothing)
    End Sub
    'giu270721
    Public Sub PopolaGrigliaCM()
        PanelTestata.Visible = False
        PanelTestataCM.Visible = True
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SqlConnDoc As New SqlConnection
        SqlConnDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-
        Dim SqlConnDocCM As New SqlConnection
        SqlConnDocCM.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        'giu200617
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
        'esempio.CommandTimeout = myTimeOUT
        '---------------------------
        'TETSTATA
        Try
            Dim dsT As New DataSet
            Dim SqlAdapDocT As New SqlDataAdapter
            Dim SqlDbSelectDocT As New SqlCommand
            SqlDbSelectDocT.CommandText = "get_DocCollegatiByIDCM"
            SqlDbSelectDocT.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDocT.Connection = SqlConnDocCM
            SqlDbSelectDocT.CommandTimeout = myTimeOUT
            SqlDbSelectDocT.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocT.SelectCommand = SqlDbSelectDocT
            '==============CARICAMENTO e MERGE DATASET==============
            SqlDbSelectDocT.Parameters.Item("@IDDocumenti").Value = Session(IDDOCUMCOLLCALL).ToString
            SqlAdapDocT.Fill(dsT)
            GridViewPrevTCM.DataSource = dsT
            GridViewPrevTCM.DataBind()
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", "Caricamento CONTRATTO(T): " & ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento CONTRATTO(T): " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'DETTAGLI
        Try
            Dim dsD As New DataSet
            Dim SqlAdapDocColl As New SqlDataAdapter
            Dim SqlDbSelectDocColl As New SqlCommand
            SqlDbSelectDocColl.CommandText = "get_DocumentiCollegatiCM"
            SqlDbSelectDocColl.CommandType = System.Data.CommandType.StoredProcedure
            SqlDbSelectDocColl.Connection = SqlConnDoc
            SqlDbSelectDocColl.CommandTimeout = myTimeOUT
            SqlDbSelectDocColl.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlAdapDocColl.SelectCommand = SqlDbSelectDocColl
            '==============CARICAMENTO e MERGE DATASET==============
            SqlDbSelectDocColl.Parameters.Item("@IDDocumenti").Value = Session(IDDOCUMCOLLCALL).ToString
            SqlAdapDocColl.Fill(dsD)
            '-Non server perchè la SP restituisce una sola SELECT
            ' ''For i = 1 To dsD.Tables.Count - 1
            ' ''    dsD.Tables(0).Merge(dsD.Tables(i))
            ' ''Next
            GridViewDocD.DataSource = dsD
            GridViewDocD.DataBind()
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL", "Caricamento documenti collegati: " & ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Caricamento documenti collegati: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End Try
        'GIU280721
        GridViewDettPR.DataSource = Nothing
        GridViewDettPR.DataBind()
        GridViewDettOC.DataSource = Nothing
        GridViewDettOC.DataBind()
        GridViewDettDTFC.DataSource = Nothing
        GridViewDettDTFC.DataBind()
        GridViewDettCA.DataSource = Nothing
        GridViewDettCA.DataBind()
        ',,,,,,,,
        'GIU011218
        GridViewDocD.SelectedIndex = -1
        Session(IDDOCUMCOLL) = "" : Session(DATADOCCOLL) = ""
        myNumDoc = "" : myTipo = ""
        If GridViewDocD.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewDocD.SelectedIndex = 0
        End If
        GridViewDocD_SelectedIndexChanged(GridViewDocD, Nothing)
    End Sub
#End Region

    
End Class