Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WFP_Nazioni
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
        WUC_Nazioni.WucElement = Me
    End Sub
    Private Sub VisualizzaMenu()
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
            Exit Sub
        Catch ex As Exception
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
            Exit Sub
        End Try
        Exit Sub
    End Sub
    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session(SWOPLOC) = SWOPNUOVO Then
            If CheckNewNumeroOnTab(CheckNewNumeroOnTab(WUC_Nazioni.GetNewCodice)) = False Then
                Exit Sub
            End If
        End If
        Session(SWOPLOC) = SWOPNESSUNA
        If (WUC_Nazioni.Aggiorna()) Then
            ProgrammaticModalPopup.Hide()
            Session(F_ANAGRNAZIONI_APERTA) = False
            VisualizzaMenu()
        End If
    End Sub
    Private Function CheckNewNumeroOnTab(ByVal myID As String) As Boolean
        If myID = 0 Then Exit Function

        Dim strSQL As String = "Select Codice From Nazioni WHERE Codice = '" & myID.Trim + "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    lblMessUtente.Text = "Attenzione, Codice Nazione già presente in tabella"
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
            lblMessUtente.Text = "Errore, Verifica codice Nazione da impegnare: " & Ex.Message
            Exit Function
        End Try
    End Function

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNESSUNA
        ProgrammaticModalPopup.Hide()
        WUC_Nazioni.SvuotaCampi()
        Session(F_ANAGRNAZIONI_APERTA) = False
        VisualizzaMenu()
    End Sub
    Protected Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNUOVO
        lblMessUtente.Text = "Inserimento nuovo elemento in tabella"
        WUC_Nazioni.SetNewCodice("")
    End Sub
    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_Nazioni.SvuotaCampi()
    End Sub

    Public Sub SetlblMessaggi(ByVal strMessaggio As String)
        lblMessUtente.Text = strMessaggio
    End Sub
End Class