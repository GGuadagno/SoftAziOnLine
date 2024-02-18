Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_Agenti
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
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            lblMessUtente.Text = ""
        End If
        WUC_Agenti.WucElement = Me
    End Sub

    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session(SWOPLOC) = SWOPNUOVO Then
            If CheckNewNumeroOnTab(CheckNewNumeroOnTab(WUC_Agenti.GetNewCodice)) = False Then
                Exit Sub
            End If
        End If
        Session(SWOPLOC) = SWOPNESSUNA
        If (WUC_Agenti.Aggiorna()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPAnagrAgenti()
            Session(F_ANAGRAGENTI_APERTA) = False
        End If
    End Sub
    Private Function CheckNewNumeroOnTab(ByVal myID As Int32) As Boolean
        If myID = 0 Then Exit Function

        Dim strSQL As String = "Select Codice From Agenti WHERE Codice = " & myID.ToString.Trim
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    'ModalPopup.Show("Attenzione", "Numero documento già presente in tabella", WUC_ModalPopup.TYPE_ALERT)
                    lblMessUtente.Text = "Attenzione, Codice Agente già presente in tabella"
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
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            lblMessUtente.Text = "Errore, Verifica codice Agente da impegnare: " & Ex.Message
            Exit Function
        End Try
    End Function

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNESSUNA
        ProgrammaticModalPopup.Hide()
        WUC_Agenti.SvuotaCampi()
        _WucElement.CancBackWFPAnagrAgenti()
        Session(F_ANAGRAGENTI_APERTA) = False
    End Sub
    Protected Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNUOVO
        lblMessUtente.Text = "Inserimento nuovo elemento in tabella"
        WUC_Agenti.SetNewCodice(GetNewCodice)
    End Sub
    Private Function GetNewCodice() As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(ISNULL(Codice,0)) AS Codice From Agenti"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
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
        WUC_Agenti.SvuotaCampi()
    End Sub

    Public Sub SetlblMessaggi(ByVal strMessaggio As String)
        lblMessUtente.Text = strMessaggio
    End Sub
End Class