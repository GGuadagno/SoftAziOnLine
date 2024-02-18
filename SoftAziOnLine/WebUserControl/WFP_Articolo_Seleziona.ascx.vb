Imports It.SoftAzi.Model.Facade
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta

Partial Public Class WFP_Articolo_Seleziona
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSArtIn.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '---- SESSIONE SCADUTA ??? o non definito
        Dim IDLT As String = Session(IDLISTINO)
        If IsNothing(IDLT) Then IDLT = "1" 'listino BASE DI DEFAULT
        If String.IsNullOrEmpty(IDLT) Then IDLT = "1" 'listino BASE DI DEFAULT
        '-----------
        lblIntesta1.Text = "Articoli presenti nel listino (" & IDLT & ")"
        If (Not IsPostBack) Then
            ddlRicercaArtIn.Items.Add("Codice")
            ddlRicercaArtIn.Items(0).Value = "C"
            ddlRicercaArtIn.Items.Add("Descrizione")
            ddlRicercaArtIn.Items(1).Value = "D"
            'giu150715
            Dim myCodCF As String = Session(CSTCODCOGE)
            If IsNothing(myCodCF) Then myCodCF = ""
            If String.IsNullOrEmpty(myCodCF) Then myCodCF = ""
            If Left(myCodCF, 1) = "9" Then
                CheckSelFor.Checked = True
                CheckSelFor.Visible = True
            Else
                CheckSelFor.Checked = False
                CheckSelFor.Visible = False
            End If
        End If
    End Sub

    Public Function PopolaGriglia(ByRef strMessErr As String) As Boolean
        PopolaGriglia = True
        Try
            BuidDett()
        Catch ex As Exception
            PopolaGriglia = False
            strMessErr = "Errore caricamento Elenco Articoli: " & ex.Message
            ' ''Chiudi("Errore caricamento Elenco Articoli: " & ex.Message)
            Exit Function
        End Try
    End Function
    Public Sub setckNoDesArtEst(ByVal SW As Boolean)
        ckNoDesArtEst.Checked = SW
        If SW Then
            Session("ckNoDesArtEst") = "SI"
        Else
            Session("ckNoDesArtEst") = "NO"
        End If
    End Sub

#Region "Metodi private"

#Region "Ricerca articoli"
    'giu150715
    Private Sub checkParoleContenute_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles checkParoleContenute.CheckedChanged
        BuidDett()
    End Sub
    Private Sub CheckSelFor_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckSelFor.CheckedChanged
        BuidDett()
    End Sub
    Private Sub ddlRicercaArtIn_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicercaArtIn.SelectedIndexChanged
        BuidDett()
    End Sub

    Protected Sub btnRicercaArtIn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicercaArtIn.Click
        BuidDett()
    End Sub

#End Region

    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (PopolaListaCodiciArtSel()) Then
            Session(F_SEL_ARTICOLO_APERTA) = False
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPArticoloSel()
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_SEL_ARTICOLO_APERTA) = False
    End Sub

    Protected Sub btnSelTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        SelezionaTutti()
    End Sub

    Protected Sub btnDeselTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        DeselezionaTutti()
    End Sub

    Private Function PopolaListaCodiciArtSel() As Boolean
        lblMessUtente.Text = ""
        'giu031111 NON PIU FISSO MA VAR PER ARTICOLO 
        '' ''Controllo BASE+OPZIONI solo se LOpz <> 0 obbligatorio selezionare la BASE CORRISPONDENTE
        '' ''Se seleziono solo l'Opzione devo anche selezionare la BASE
        ' ''Dim LBase As Integer = 0
        ' ''Dim LOpz As Integer = 0
        ' ''Documenti.GetLBaseLOpz(LBase, LOpz)
        ' ''If LOpz <> 0 Then
        ' ''    If CKBaseOpz(LBase, LOpz) = False Then
        ' ''        lblMessUtente.Text = "Attenzione, obbligo Base+Opzione, verificare i dati selezionati automaticamente e confermare se è corretto."
        ' ''        PopolaListaCodiciArtSel = False
        ' ''        Exit Function
        ' ''    End If
        ' ''End If
        '----------------------------------------------------------------------------------------
        'Controllo se ho selezionato la base
        'giu301111 per evitare di inserire articoli gia' presenti nel documento
        ' GESTITO DIRETTAMENTE IN SELEZIONA ARTICOLI E NON QUI, mi arrivato i dati effettivi
        If checkSelAutoPadre.Checked = True Then
            If CKBaseOpz(0, 0) = False Then
                lblMessUtente.Text = "Attenzione, obbligo Base+Opzione, verificare i dati selezionati automaticamente e confermare se è corretto."
                PopolaListaCodiciArtSel = False
                Exit Function
            End If
        End If
        '----------------------------------------------------------------------------------------
        Dim listaCodiciArticolo As New List(Of String)
        For Each row As GridViewRow In GridViewArtIn.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            If (checkSel.Checked) Then
                listaCodiciArticolo.Add(row.Cells(CellIdx.Codice).Text.Trim)
            End If
        Next
        If (listaCodiciArticolo.Count > 0) Then
            PopolaListaCodiciArtSel = True
            Session(ARTICOLI_DA_INS) = listaCodiciArticolo
        Else
            lblMessUtente.Text = "Attenzione, nessun articolo selezionato."
            PopolaListaCodiciArtSel = False
        End If
    End Function

    Private Function CKBaseOpz(ByRef _LBase As Integer, ByRef _LOpz As Integer) As Boolean
        CKBaseOpz = True
        Dim CodSel As String = ""
        For Each row As GridViewRow In GridViewArtIn.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            If (checkSel.Checked) Then
                CodSel = row.Cells(CellIdx.Codice).Text.Trim
                If IsNumeric(row.Cells(CellIdx.LBase).Text.Trim) Then
                    _LBase = row.Cells(CellIdx.LBase).Text.Trim
                Else
                    _LBase = CodSel.Trim.Length
                End If
                If IsNumeric(row.Cells(CellIdx.LOpz).Text.Trim) Then
                    _LOpz = row.Cells(CellIdx.LOpz).Text.Trim
                Else
                    _LOpz = 0
                End If
                If CodSel.Length = _LBase + _LOpz Then 'Controllo se ho selezionato la base
                    If ObbBaseOpz = True Then
                        CKBaseOpz = funObbBaseOpz(_LBase, _LOpz, CodSel)
                    End If
                ElseIf CodSel.Length = _LBase Then 'Controllo se ho selezionato l'opzione
                    If ObbOpzBase = True Then
                        CKBaseOpz = funObbOpzBase(_LBase, _LOpz, CodSel)
                    End If
                End If
            End If
        Next
    End Function

    Private Function funObbBaseOpz(ByVal _LBase As Integer, ByVal _LOpz As Integer, ByVal _CodSel As String) As Boolean
        Dim CodSel As String = ""
        For Each row As GridViewRow In GridViewArtIn.Rows
            CodSel = row.Cells(CellIdx.Codice).Text.Trim
            If CodSel.Length = _LBase And CodSel.Trim = Mid(_CodSel, 1, _LBase) Then
                Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
                If (checkSel.Checked) Then
                    funObbBaseOpz = True
                    Exit Function
                Else
                    checkSel.Checked = True
                    funObbBaseOpz = True
                    Exit Function
                End If
            End If
        Next
    End Function
    Private Function funObbOpzBase(ByVal _LBase As Integer, ByVal _LOpz As Integer, ByVal _CodSel As String) As Boolean
        'Controllo se ho selezionato l'opzione
        Dim CodSel As String = ""
        For Each row As GridViewRow In GridViewArtIn.Rows
            CodSel = row.Cells(CellIdx.Codice).Text.Trim
            If CodSel.Length = _LBase + _LOpz And CodSel.Trim = Mid(_CodSel, 1, _LBase) Then
                Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
                If (checkSel.Checked) Then
                    funObbOpzBase = True
                    Exit Function
                Else
                    checkSel.Checked = True
                    funObbOpzBase = False
                    Exit Function
                End If
            End If
        Next
    End Function

    Private Sub GridViewArtIn_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewArtIn.PageIndexChanging
        'giu150715
        If (e.NewPageIndex = -1) Then
            GridViewArtIn.PageIndex = 0
        Else
            GridViewArtIn.PageIndex = e.NewPageIndex
        End If
        GridViewArtIn.SelectedIndex = -1
        BuidDett()
    End Sub
    'giu150715
    Private Sub BuidDett()
        Session("SortListVenD") = ddlRicercaArtIn.SelectedValue.Trim 'giu060522
        SetRicerca()
        SetFilter()
        SqlDSArtIn.DataBind()
        '---------
        GridViewArtIn.DataBind()
    End Sub
    Private Sub SetRicerca()
        SqlDSArtIn.FilterExpression = ""
        If txtRicercaArtIn.Text.Trim <> "" Then
            If ddlRicercaArtIn.SelectedValue = "C" Then
                If checkParoleContenute.Checked = False Then
                    SqlDSArtIn.FilterExpression = "Cod_Articolo like '" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "%'"
                Else
                    SqlDSArtIn.FilterExpression = "Cod_Articolo like '%" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "%'"
                End If

            ElseIf ddlRicercaArtIn.SelectedValue = "D" Then
                If checkParoleContenute.Checked = False Then
                    SqlDSArtIn.FilterExpression = "Descrizione >= '" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "'"
                Else
                    SqlDSArtIn.FilterExpression = "Descrizione like '%" & Controlla_Apice(txtRicercaArtIn.Text.Trim) & "%'"
                End If
            End If
        End If
    End Sub
    Private Sub SetFilter()

        If CheckSelFor.Checked Then
            Dim myCodCF As String = Session(CSTCODCOGE)
            Try
                If IsNothing(myCodCF) Then myCodCF = ""
                If String.IsNullOrEmpty(myCodCF) Then myCodCF = ""
                If Left(myCodCF, 1) = "9" Then
                    If SqlDSArtIn.FilterExpression.Trim <> "" Then
                        SqlDSArtIn.FilterExpression += " AND "
                    End If
                    SqlDSArtIn.FilterExpression += "CodiceFornitore='" & myCodCF.Trim & "'"
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub
#End Region

#Region "Metodi Public"
    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    'giu170412
    Public Enum CellIdx
        Codice = 1
        Descrizione = 2
        Prezzo = 3
        Sconto1 = 4
        PrezzoMin = 5
        PrezzoAcq = 6
        LBase = 7
        LOpz = 8
    End Enum

    Public Sub SelezionaTutti()
        For Each row As GridViewRow In GridViewArtIn.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = True
        Next
    End Sub

    Public Sub DeselezionaTutti()
        For Each row As GridViewRow In GridViewArtIn.Rows
            Dim checkSel As CheckBox = CType(row.FindControl("checkSel"), CheckBox)
            checkSel.Checked = False
        Next
    End Sub

#End Region

End Class