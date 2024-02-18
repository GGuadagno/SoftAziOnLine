Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_RespArea
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
        WUC_RespArea.WucElement = Me
    End Sub

    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUC_RespArea.Aggiorna()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPAnagrRespArea()
            Session(F_ANAGRRESPAREA_APERTA) = False
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        WUC_RespArea.SvuotaCampi()
        _WucElement.CancBackWFPAnagrRespArea()
        Session(F_ANAGRRESPAREA_APERTA) = False
    End Sub
    Protected Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        lblMessUtente.Text = "Inserimento nuovo elemento in tabella"
        WUC_RespArea.SetNewCodice(GetNewCodice)
    End Sub
    Private Function GetNewCodice() As Long
        Dim strSQL As String = ""
        strSQL = "Select MAX(ISNULL(Codice,0)) AS Codice From RespArea"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbScadenzario, strSQL, ds)
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
        WUC_RespArea.SvuotaCampi()
        lblMessUtente.Text = ""
        WUC_RespArea.SetNewCodice(GetNewCodice)
    End Sub

    Public Sub SetlblMessaggi(ByVal strMessaggio As String)
        lblMessUtente.Text = strMessaggio
    End Sub
End Class