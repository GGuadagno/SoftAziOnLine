Imports System.Data.SqlClient
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports Microsoft.Reporting.WebForms

Partial Public Class WFP_Articolo_SelezSing
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
            Try
                ddlRicercaArtIn.Items.Add("Codice")
                ddlRicercaArtIn.Items(0).Value = "C"
                ddlRicercaArtIn.Items.Add("Descrizione")
                ddlRicercaArtIn.Items(1).Value = "D"
            Catch ex As Exception

            End Try
        End If
    End Sub


#Region "Metodi private"

#Region "Ricerca articoli"

    Private Sub ddlRicercaArtIn_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicercaArtIn.SelectedIndexChanged

        SqlDSArtIn.FilterExpression = "" : txtRicercaArtIn.Text = ""
        Session("SortListVenD") = ddlRicercaArtIn.SelectedValue.Trim
        SqlDSArtIn.DataBind()

        ddlRicercaArtIn.Enabled = True : txtRicercaArtIn.Enabled = True : btnRicercaArtIn.Enabled = True
        GridViewArtIn.DataBind()
        ' ''If GridViewArtIn.Rows.Count > 0 Then
        ' ''    GridViewArtIn.SelectedIndex = 0
        ' ''End If
    End Sub

    Protected Sub btnRicercaArtIn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicercaArtIn.Click

        If txtRicercaArtIn.Text.Trim = "" Then
            SqlDSArtIn.FilterExpression = ""
            SqlDSArtIn.DataBind()
            ddlRicercaArtIn.Enabled = True : txtRicercaArtIn.Enabled = True : btnRicercaArtIn.Enabled = True
            GridViewArtIn.DataBind()
            ' ''If GridViewArtIn.Rows.Count > 0 Then
            ' ''    GridViewArtIn.SelectedIndex = 0
            ' ''End If
            Exit Sub
        End If
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
        SqlDSArtIn.DataBind()
        ddlRicercaArtIn.Enabled = True : txtRicercaArtIn.Enabled = True : btnRicercaArtIn.Enabled = True
        GridViewArtIn.DataBind()
        ' ''If GridViewArtIn.Rows.Count > 0 Then
        ' ''    GridViewArtIn.SelectedIndex = 0
        ' ''End If

    End Sub

#End Region

    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (PopolaCodArtSel()) Then
            Session(F_SEL_ARTICOLO_APERTA) = False
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPArticoloSelSing()
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_SEL_ARTICOLO_APERTA) = False
    End Sub

    Private Function PopolaCodArtSel() As Boolean
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        lblMessUtente.Text = ""
        Try
            Session(ARTICOLO_COD_SEL) = GridViewArtIn.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewArtIn.SelectedRow
            Session(ARTICOLO_DES_SEL) = row.Cells(CellIdx.Descrizione).Text.Trim
            Session(ARTICOLO_LBASE_SEL) = row.Cells(CellIdx.LBase).Text.Trim
            Session(ARTICOLO_LOPZ_SEL) = row.Cells(CellIdx.LOpz).Text.Trim
            If Not (String.IsNullOrEmpty(Session(ARTICOLO_COD_SEL))) Then
                PopolaCodArtSel = True
            Else
                lblMessUtente.Text = "Attenzione, nessun articolo selezionato."
                PopolaCodArtSel = False
            End If
        Catch ex As Exception
            lblMessUtente.Text = "Attenzione, nessun articolo selezionato." '"Errore: " & ex.Message
            PopolaCodArtSel = False
            Session(ARTICOLO_COD_SEL) = ""
            Session(ARTICOLO_DES_SEL) = ""
            Session(ARTICOLO_LBASE_SEL) = ""
            Session(ARTICOLO_LOPZ_SEL) = ""
        End Try
    End Function

#End Region

    Private Sub GridViewArtIn_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewArtIn.PageIndexChanging
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
    End Sub

    Private Sub GridViewArtIn_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewArtIn.SelectedIndexChanged
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        Try
            Session(ARTICOLO_COD_SEL) = GridViewArtIn.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewArtIn.SelectedRow
            Session(ARTICOLO_DES_SEL) = row.Cells(CellIdx.Descrizione).Text.Trim
            Session(ARTICOLO_LBASE_SEL) = row.Cells(CellIdx.LBase).Text.Trim
            Session(ARTICOLO_LOPZ_SEL) = row.Cells(CellIdx.LOpz).Text.Trim
        Catch ex As Exception
            Session(ARTICOLO_COD_SEL) = ""
            Session(ARTICOLO_DES_SEL) = ""
            Session(ARTICOLO_LBASE_SEL) = ""
            Session(ARTICOLO_LOPZ_SEL) = ""
        End Try
    End Sub
End Class