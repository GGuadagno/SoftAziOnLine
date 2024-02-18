Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_Magazzini
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
        If (Not IsPostBack) Then
            lblMessUtente.Text = ""
        End If
        WUC_Magazzini.WucElement = Me
    End Sub

    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session(SWOPLOC) = SWOPNUOVO Then
            If CheckNewNumeroOnTab(CheckNewNumeroOnTab(WUC_Magazzini.GetNewCodice)) = False Then
                Exit Sub
            End If
        End If
        Session(SWOPLOC) = SWOPNESSUNA
        If (WUC_Magazzini.Aggiorna()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPMagazzini()
            Session(F_MAGAZZINI_APERTA) = False
        End If
    End Sub

    Private Function CheckNewNumeroOnTab(ByVal _ID As Int32) As Boolean
        Dim strSQL As String = "Select Codice From Magazzini WHERE Codice = " & _ID.ToString.Trim
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    lblMessUtente.Text = "Attenzione, Codice già presente in tabella"
                    Exit Function
                Else
                    CheckNewNumeroOnTab = True
                    Exit Function
                End If
            Else
                CheckNewNumeroOnTab = True
                Exit Function
            End If
        Catch Ex As Exception
            lblMessUtente.Text = "Errore, Verifica codice da impegnare: " & Ex.Message
            Exit Function
        End Try
    End Function

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNESSUNA
        ProgrammaticModalPopup.Hide()
        WUC_Magazzini.SvuotaCampi()
        _WucElement.CancBackWFPMagazzini()
        Session(F_MAGAZZINI_APERTA) = False
    End Sub
    Protected Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNUOVO
        lblMessUtente.Text = "Inserimento nuovo elemento in tabella"
        WUC_Magazzini.SetNewCodice(GetNewCodice)
    End Sub
    Private Function GetNewCodice() As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(ISNULL(Codice,0)) AS Codice From Magazzini"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Codice")) Then
                        GetNewCodice = ds.Tables(0).Rows(0).Item("Codice") + 1
                    Else
                        GetNewCodice = 1
                    End If
                    Exit Function
                Else
                    GetNewCodice = 1
                    Exit Function
                End If
            Else
                GetNewCodice = 1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewCodice = -1
            Exit Function
        End Try

    End Function

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_Magazzini.SvuotaCampi()
    End Sub

    Public Sub SetlblMessaggi(ByVal strMessaggio As String)
        lblMessUtente.Text = strMessaggio
    End Sub
End Class