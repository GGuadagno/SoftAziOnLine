Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Partial Public Class WUC_DistintaBase
    Inherits System.Web.UI.UserControl

#Region "Costanti"
    ' ''Private Const CAMPI_MODIFICATI As String = "CampiModificati"
    Private Const INS_RIGA_PRIMA As Integer = 0
    Private Const INS_RIGA_DOPO As Integer = 1
    Private Const SEL_RIGA As String = "SelRiga"

    Public Enum CellIdx
        Riga = 1
        CodArt = 2
        DesArt = 3
        UM = 4
        Qta = 5
    End Enum

#End Region

#Region "Variabili private"

    Private _Enabled As Boolean

#End Region

#Region "Property"

    Property Enabled() As Boolean
        Get
            Return _Enabled
        End Get
        Set(ByVal value As Boolean)
            _Enabled = value
            GridViewBody.Enabled = value
            PulsantiSetEnabledTo(value)
        End Set
    End Property

#End Region

#Region "Metodi private - eventi"

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not IsPostBack) Then
            CaricaVariabili()
        End If

        WFP_Articolo_SelezSing1.WucElement = Me
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub

    Private Sub GridViewBody_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewBody.SelectedIndexChanged
        Session(SEL_RIGA) = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(CellIdx.Riga).Text
        txtCodArt.Text = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(CellIdx.CodArt).Text
        LblDesArt.Text = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(CellIdx.DesArt).Text
        txtUM.Text = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(CellIdx.UM).Text
        txtQta.Text = GridViewBody.Rows(GridViewBody.SelectedIndex).Cells(CellIdx.Qta).Text
        btnModificaRiga.Enabled = True
        btnCancellaRiga.Enabled = True
        BtnSelArticoloIns.Enabled = True
        lblMess.Visible = False
    End Sub

    Private Sub GridViewBody_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewBody.RowDataBound
        ' ''e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        ' ''e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewBody, "Select$" + e.Row.RowIndex.ToString()))
        Try
            If e.Row.DataItemIndex > -1 Then
                If IsNumeric(e.Row.Cells(CellIdx.Qta).Text) Then
                    If CDec(e.Row.Cells(CellIdx.Qta).Text) <> 0 Then
                        e.Row.Cells(CellIdx.Qta).Text = FormattaNumero(CDec(e.Row.Cells(CellIdx.Qta).Text), -1).ToString
                    Else
                        e.Row.Cells(CellIdx.Qta).Text = ""
                    End If
                End If
            End If
        Catch ex As Exception
            Dim errore As Integer = 0
        End Try
    End Sub

    Private Sub btnInserisciDopo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserisciDopo.Click
        If (Not String.IsNullOrEmpty(txtCodArt.Text)) Then
            If txtCodArt.Text.Trim = Session(COD_ARTICOLO).ToString.Trim Then
                lblMess.Text = "Attenzione, non si può inserire l'articolo ""padre"" come componente."
                lblMess.Visible = True
                txtCodArt.Focus()
                Exit Sub
            End If
            If ControllaDBase(txtCodArt.Text.Trim) = False Then
                lblMess.Visible = True
                txtCodArt.Focus()
                Exit Sub
            End If
            If LeggiArticolo(txtCodArt.Text.Trim) = False Then
                lblMess.Text = "Codice articolo inesistente."
                lblMess.Visible = True
            ElseIf Not IsNumeric(txtQta.Text) Then
                txtQta.Text = "1"
                lblMess.Visible = False
                Session(SWMODIFICATO) = True
                ModificaRiga()
            ElseIf txtUM.Text.Trim <> "" And CDec(txtQta.Text.Trim) > 0 Then
                lblMess.Visible = False
                Session(SWMODIFICATO) = True
                InserimentoRiga(INS_RIGA_DOPO)
            Else
                lblMess.Text = "Dati obbligatori mancanti e/o errati. (UM,Qtà)"
                lblMess.Visible = True
                txtQta.Focus()
            End If
        Else
            LblDesArt.Text = ""
            txtUM.Text = ""
            lblMess.Text = "Codice articolo obbligatorio."
            lblMess.Visible = True
        End If
    End Sub

    Private Sub btnInserisciPrima_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnInserisciPrima.Click
        If (Not String.IsNullOrEmpty(txtCodArt.Text)) Then
            If txtCodArt.Text.Trim = Session(COD_ARTICOLO).ToString.Trim Then
                lblMess.Text = "Attenzione, non si può inserire l'articolo ""padre"" come componente."
                lblMess.Visible = True
                txtCodArt.Focus()
                Exit Sub
            End If
            If ControllaDBase(txtCodArt.Text.Trim) = False Then
                lblMess.Visible = True
                txtCodArt.Focus()
                Exit Sub
            End If
            If LeggiArticolo(txtCodArt.Text.Trim) = False Then
                lblMess.Text = "Codice articolo inesistente."
                lblMess.Visible = True
            ElseIf Not IsNumeric(txtQta.Text) Then
                txtQta.Text = "1"
                lblMess.Visible = False
                Session(SWMODIFICATO) = True
                ModificaRiga()
            ElseIf txtUM.Text.Trim <> "" And CDec(txtQta.Text.Trim) > 0 Then
                lblMess.Visible = False
                Session(SWMODIFICATO) = True
                InserimentoRiga(INS_RIGA_PRIMA)
            Else
                lblMess.Text = "Dati obbligatori mancanti e/o errati. (UM,Qtà)"
                lblMess.Visible = True
                txtQta.Focus()
            End If
        Else
            LblDesArt.Text = ""
            txtUM.Text = ""
            lblMess.Text = "Codice articolo obbligatorio."
            lblMess.Visible = True
        End If

    End Sub

    Private Sub btnCancellaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancellaRiga.Click
        If (Not String.IsNullOrEmpty(Session(SEL_RIGA))) Then
            If (Not String.IsNullOrEmpty(txtCodArt.Text)) Then
                lblMess.Visible = False
                Session(SWMODIFICATO) = True
                ConfermaCancellazioneRiga()
            Else
                lblMess.Text = "Nessun articolo selezionato"
                lblMess.Visible = True
            End If
        Else
            LblDesArt.Text = ""
            txtUM.Text = ""
            lblMess.Text = "Nessun articolo selezionato"
            lblMess.Visible = True
        End If
    End Sub

    Private Sub btnModificaRiga_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModificaRiga.Click
        If (Not String.IsNullOrEmpty(txtCodArt.Text)) Then
            If txtCodArt.Text.Trim = Session(COD_ARTICOLO).ToString.Trim Then
                lblMess.Text = "Attenzione, non si può inserire l'articolo ""padre"" come componente."
                lblMess.Visible = True
                txtCodArt.Focus()
                Exit Sub
            End If
            ' ''If ControllaDBase(txtCodArt.Text.Trim) = False Then
            ' ''    lblMess.Visible = True
            ' ''    txtCodArt.Focus()
            ' ''    Exit Sub
            ' ''End If
            If LeggiArticolo(txtCodArt.Text.Trim) = False Then
                lblMess.Text = "Codice articolo inesistente."
                lblMess.Visible = True
            ElseIf Not IsNumeric(txtQta.Text) Then
                txtQta.Text = "1"
                lblMess.Visible = False
                Session(SWMODIFICATO) = True
                ModificaRiga()
            ElseIf txtUM.Text.Trim <> "" And CDec(txtQta.Text.Trim) > 0 Then
                lblMess.Visible = False
                Session(SWMODIFICATO) = True
                ModificaRiga()
            Else
                lblMess.Text = "Dati obbligatori mancanti e/o errati. (UM,Qtà)"
                lblMess.Visible = True
                txtQta.Focus()
            End If
        Else
            LblDesArt.Text = ""
            txtUM.Text = ""
            lblMess.Text = "Codice articolo obbligatorio."
            lblMess.Visible = True
        End If
    End Sub

    Private Sub txtCodArt_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCodArt.TextChanged
        btnModificaRiga.Enabled = False
        btnCancellaRiga.Enabled = False
        BtnSelArticoloIns.Enabled = True
        If (Not String.IsNullOrEmpty(txtCodArt.Text)) Then
            If LeggiArticolo(txtCodArt.Text.Trim) = False Then
                lblMess.Text = "Codice articolo inesistente."
                lblMess.Visible = True
            Else
                btnModificaRiga.Enabled = True
                btnCancellaRiga.Enabled = True
                lblMess.Visible = False
                Session(SWMODIFICATO) = True
                If Not IsNumeric(txtQta.Text.Trim) Then
                    txtQta.Text = "1"
                End If
                txtQta.Focus()
            End If
        Else
            LblDesArt.Text = ""
            txtUM.Text = ""
            lblMess.Text = "Codice articolo obbligatorio."
            lblMess.Visible = True
        End If
    End Sub
    '-
    Private Function LeggiArticolo(ByVal myCodArt As String) As Boolean
        If myCodArt.Trim = "" Then
            LblDesArt.Text = ""
            txtUM.Text = ""
            LeggiArticolo = False
            Exit Function
        End If
        Dim AMSys As New AnaMag
        Dim myAM As AnaMagEntity
        Dim arrAM As ArrayList
        Try
            arrAM = AMSys.getAnaMagByCodice(myCodArt.Trim)
            If (arrAM.Count > 0) Then
                myAM = CType(arrAM(0), AnaMagEntity)
                LblDesArt.Text = myAM.Descrizione
                txtUM.Text = myAM.Um
                LeggiArticolo = True
            Else
                LblDesArt.Text = ""
                txtUM.Text = ""
                LeggiArticolo = False
            End If
        Catch ex As Exception
            LblDesArt.Text = ""
            txtUM.Text = ""
            lblMess.Text = "Errore lettura articolo."
            lblMess.Visible = True
            LeggiArticolo = False
        End Try
    End Function
#End Region

#Region "Gestione variabili di sessione interne"

    Private Sub CaricaVariabili()
        Session(SEL_RIGA) = String.Empty
    End Sub

#End Region

#Region "Metodi private"

    Private Sub PulsantiSetEnabledTo(ByVal valore As Boolean)
        btnCancellaRiga.Enabled = valore
        btnInserisciDopo.Enabled = valore
        btnInserisciPrima.Enabled = valore
        btnCancellaRiga.Visible = valore
        btnInserisciDopo.Visible = valore
        btnInserisciPrima.Visible = valore
        btnModificaRiga.Visible = valore
        BtnSelArticoloIns.Enabled = valore
        txtCodArt.Enabled = valore
        txtUM.Enabled = valore
        txtQta.Enabled = valore
        If valore = False Then
            lblMess.Visible = False
        End If
    End Sub

    Private Sub ModificaRiga()
        Dim arrDistBase As New ArrayList
        Dim codFiglio As String = txtCodArt.Text
        Dim desFiglio As String = LblDesArt.Text
        Dim UM As String = txtUM.Text
        Dim quantita As Decimal = IIf(IsNumeric(txtQta.Text.Trim), CDec(txtQta.Text.Trim.Trim), 0)
        Dim progRiga As Integer = Val(Session(SEL_RIGA))
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(codFiglio)) Then
            If (GridViewBody.Rows.Count > 0) Then
                For Each row As GridViewRow In GridViewBody.Rows
                    Dim myDistBase As New DistBaseEntity
                    If (progRiga.Equals(progressivo)) Then
                        myDistBase.CodFiglio = codFiglio
                        myDistBase.DesFiglio = desFiglio
                        myDistBase.UM = UM
                        myDistBase.Quantita = quantita
                    Else
                        myDistBase.CodFiglio = row.Cells(CellIdx.CodArt).Text
                        myDistBase.DesFiglio = row.Cells(CellIdx.DesArt).Text
                        myDistBase.UM = row.Cells(CellIdx.UM).Text
                        myDistBase.Quantita = IIf(IsNumeric(row.Cells(CellIdx.Qta).Text.Trim), CDec(row.Cells(CellIdx.Qta).Text.Trim), 0)
                    End If
                    myDistBase.CodPadre = Session(COD_ARTICOLO)
                    myDistBase.Riga = progressivo
                    arrDistBase.Add(myDistBase)
                    progressivo = progressivo + 1
                Next
            End If
            GridViewBody.DataSource = arrDistBase
            GridViewBody.DataBind()
        End If
    End Sub

    Private Sub InserimentoRiga(ByVal posizione As Integer)
        lblMess.Visible = False
        Dim myDistBase As New DistBaseEntity
        Dim arrDistBase As New ArrayList
        Dim CnuovaRiga As String = txtCodArt.Text
        Dim DnuovaRiga As String = LblDesArt.Text
        Dim UM As String = txtUM.Text
        Dim Quantita As Decimal = IIf(IsNumeric(txtQta.Text.Trim), CDec(txtQta.Text.Trim.Trim), 0)
        Dim progNuovaRiga As Integer = 0
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(Session(SEL_RIGA))) Then
            If (posizione.Equals(INS_RIGA_PRIMA)) Then
                progNuovaRiga = Val(Session(SEL_RIGA))
            ElseIf (posizione.Equals(INS_RIGA_DOPO)) Then
                progNuovaRiga = Val(Session(SEL_RIGA)) + 1
            End If
        Else
            If (posizione.Equals(INS_RIGA_PRIMA)) Then
                progNuovaRiga = 1
            ElseIf (posizione.Equals(INS_RIGA_DOPO)) Then
                progNuovaRiga = GridViewBody.Rows.Count + 1
            End If
        End If

        If (Not String.IsNullOrEmpty(CnuovaRiga)) Then
            Session(SEL_RIGA) = String.Format("{0}", progNuovaRiga)
            If (GridViewBody.Rows.Count > 0) Then
                For Each row As GridViewRow In GridViewBody.Rows
                    Dim myDB As New DistBaseEntity
                    If (progNuovaRiga.Equals(progressivo)) Then
                        myDistBase.CodPadre = Session(COD_ARTICOLO)
                        myDistBase.CodFiglio = CnuovaRiga
                        myDistBase.DesFiglio = DnuovaRiga
                        myDistBase.Riga = progressivo
                        myDistBase.UM = UM
                        myDistBase.Quantita = Quantita
                        arrDistBase.Add(myDistBase)
                        progressivo = progressivo + 1
                    End If
                    myDB.CodPadre = Session(COD_ARTICOLO)
                    myDB.Riga = progressivo
                    myDB.CodFiglio = row.Cells(CellIdx.CodArt).Text
                    myDB.DesFiglio = row.Cells(CellIdx.DesArt).Text
                    myDB.UM = row.Cells(CellIdx.UM).Text
                    myDB.Quantita = IIf(IsNumeric(row.Cells(CellIdx.Qta).Text.Trim), CDec(row.Cells(CellIdx.Qta).Text.Trim), 0)
                    arrDistBase.Add(myDB)
                    progressivo = progressivo + 1
                Next
                If (progNuovaRiga.Equals(progressivo)) Then
                    myDistBase.CodPadre = Session(COD_ARTICOLO)
                    myDistBase.CodFiglio = CnuovaRiga
                    myDistBase.DesFiglio = DnuovaRiga
                    myDistBase.Riga = progressivo
                    myDistBase.UM = UM
                    myDistBase.Quantita = Quantita
                    arrDistBase.Add(myDistBase)
                End If
            Else
                myDistBase.CodPadre = Session(COD_ARTICOLO)
                myDistBase.CodFiglio = CnuovaRiga
                myDistBase.DesFiglio = DnuovaRiga
                myDistBase.Riga = progressivo
                myDistBase.UM = UM
                myDistBase.Quantita = Quantita
                arrDistBase.Add(myDistBase)
            End If
            GridViewBody.DataSource = arrDistBase
            GridViewBody.DataBind()
            SelezionaRigaFromProgressivo(Session(SEL_RIGA))
        Else
            lblMess.Visible = True
        End If
    End Sub

    Private Sub SelezionaRigaFromProgressivo(ByVal progressivo As String)
        Dim index As Integer = 0
        For Each row As GridViewRow In GridViewBody.Rows
            If (row.Cells(CellIdx.Riga).Text = progressivo) Then
                GridViewBody.SelectedIndex = index
                btnModificaRiga.Enabled = True
                BtnSelArticoloIns.Enabled = True
                Exit For
            End If
            index = index + 1
        Next
    End Sub

#End Region

#Region "Metodi public"

    Public Sub PopolaGridView()
        txtCodArt.Text = String.Empty
        LblDesArt.Text = String.Empty
        txtUM.Text = String.Empty
        txtQta.Text = String.Empty
        btnModificaRiga.Enabled = False
        btnCancellaRiga.Enabled = False
        CaricaVariabili()
        SqlDataSourceDistBase.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceDistBase.SelectParameters("Codice").DefaultValue = Session(COD_ARTICOLO)
        GridViewBody.DataSource = SqlDataSourceDistBase
        GridViewBody.DataBind()
        GridViewBody.SelectedIndex = 0
        If (GridViewBody.Rows.Count > 0) Then
            Session(SEL_RIGA) = GridViewBody.Rows(0).Cells(CellIdx.Riga).Text
            txtCodArt.Text = GridViewBody.Rows(0).Cells(CellIdx.CodArt).Text
            LblDesArt.Text = GridViewBody.Rows(0).Cells(CellIdx.DesArt).Text
            txtUM.Text = GridViewBody.Rows(0).Cells(CellIdx.UM).Text
            txtQta.Text = GridViewBody.Rows(0).Cells(CellIdx.Qta).Text
            btnModificaRiga.Enabled = True
            btnCancellaRiga.Enabled = True
        End If
    End Sub

    Public Sub SvuotaGridView()
        txtCodArt.Text = String.Empty
        LblDesArt.Text = String.Empty
        txtUM.Text = String.Empty
        txtQta.Text = String.Empty
        CaricaVariabili()
        SqlDataSourceDistBase.ConnectionString = Session(DBCONNAZI)
        SqlDataSourceDistBase.SelectParameters("Codice").DefaultValue = "0"
        GridViewBody.DataSource = SqlDataSourceDistBase
        GridViewBody.DataBind()
    End Sub

    Public Sub ConfermaCancellazioneRiga()
        lblMess.Visible = False
        Dim arrDistBase As New ArrayList
        Dim progCancRiga As String = String.Empty
        Dim progressivo As Integer = 1

        If (Not String.IsNullOrEmpty(Session(SEL_RIGA))) Then
            progCancRiga = Session(SEL_RIGA)
            If (GridViewBody.Rows.Count > 0) Then
                txtCodArt.Text = String.Empty
                LblDesArt.Text = String.Empty
                txtUM.Text = String.Empty
                txtQta.Text = String.Empty
                For Each row As GridViewRow In GridViewBody.Rows
                    If (Not row.Cells(CellIdx.Riga).Text.Equals(progCancRiga)) Then
                        Dim myDB As New DistBaseEntity
                        myDB.CodPadre = Session(COD_ARTICOLO)
                        myDB.Riga = progressivo
                        myDB.CodFiglio = row.Cells(CellIdx.CodArt).Text
                        myDB.DesFiglio = row.Cells(CellIdx.DesArt).Text
                        myDB.UM = row.Cells(CellIdx.UM).Text
                        myDB.Quantita = IIf(IsNumeric(row.Cells(CellIdx.Qta).Text.Trim), CDec(row.Cells(CellIdx.Qta).Text.Trim), 0)
                        arrDistBase.Add(myDB)
                        progressivo = progressivo + 1
                    End If
                Next
            End If
            GridViewBody.DataSource = arrDistBase
            GridViewBody.SelectedIndex = -1
            GridViewBody.DataBind()
            If (GridViewBody.Rows.Count = 0) Then
                btnModificaRiga.Enabled = False
                btnCancellaRiga.Enabled = False
            End If
        End If

        If GridViewBody.Rows.Count = 0 Then
            btnModificaRiga.Enabled = False
            btnCancellaRiga.Enabled = False
        End If
    End Sub

    Public Function GetListDistBase() As List(Of DistBaseEntity)
        Dim myListDistBase As New List(Of DistBaseEntity)

        For Each row As GridViewRow In GridViewBody.Rows
            Dim myDistBase As New DistBaseEntity
            myDistBase.CodPadre = Session(COD_ARTICOLO)
            myDistBase.Riga = row.Cells(CellIdx.Riga).Text
            myDistBase.CodFiglio = row.Cells(CellIdx.CodArt).Text
            myDistBase.DesFiglio = IIf(row.Cells(CellIdx.DesArt).Text = HTML_SPAZIO, String.Empty, row.Cells(CellIdx.DesArt).Text)
            myDistBase.UM = row.Cells(CellIdx.UM).Text
            myDistBase.Quantita = IIf(IsNumeric(row.Cells(CellIdx.Qta).Text.Trim), CDec(row.Cells(CellIdx.Qta).Text.Trim), 0)
            myListDistBase.Add(myDistBase)
        Next

        GetListDistBase = myListDistBase
    End Function
    Private Function ControllaDBase(ByVal _CodArt As String) As Boolean
        ControllaDBase = False
        If String.IsNullOrEmpty(_CodArt) Then ControllaDBase = True : Exit Function
        If _CodArt.Trim = "" Then ControllaDBase = True : Exit Function
        '-
        For Each row As GridViewRow In GridViewBody.Rows
            If (Not String.IsNullOrEmpty(Session(COD_ARTICOLO))) Then
                If Session(COD_ARTICOLO).ToString.Trim = _CodArt.Trim Then
                    lblMess.Text = "Attenzione, non si può inserire l'articolo ""padre"" come componente."
                    lblMess.Visible = True
                    Exit Function
                End If
            End If
            If _CodArt.Trim = row.Cells(CellIdx.CodArt).Text.Trim Then
                lblMess.Text = "Attenzione, articolo già presente come componente."
                lblMess.Visible = True
                Exit Function
            End If
        Next
        ControllaDBase = True
    End Function

#End Region

    Private Sub BtnSelArticoloIns_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnSelArticoloIns.Click
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Public Sub CallBackWFPArticoloSelSing()
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        txtCodArt.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        LblDesArt.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        If (Not String.IsNullOrEmpty(txtCodArt.Text)) Then
            If txtCodArt.Text.Trim = Session(COD_ARTICOLO).ToString.Trim Then
                lblMess.Text = "Attenzione, non si può inserire l'articolo ""padre"" come componente."
                lblMess.Visible = True
                txtCodArt.Focus()
                Exit Sub
            End If
            If ControllaDBase(txtCodArt.Text.Trim) = False Then
                lblMess.Visible = True
                txtCodArt.Focus()
                Exit Sub
            End If
        End If
        '-
        If LeggiArticolo(txtCodArt.Text.Trim) = False Then
            lblMess.Text = "Codice articolo inesistente."
            lblMess.Visible = True
        Else
            btnModificaRiga.Enabled = True
            btnCancellaRiga.Enabled = True
            lblMess.Visible = False
            If Not IsNumeric(txtQta.Text.Trim) Then
                txtQta.Text = "1"
            End If
        End If
    End Sub
End Class