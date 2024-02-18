Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.Model.Entity 'giu150312
Imports It.SoftAzi.Model.Facade 'Ho tutte le funzioni es. get_Operatori
Imports It.SoftAzi.Model.Entity.OperatoriEntity

Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWeb_IN
    Inherits System.Web.UI.Page

    Private CodiceDitta As String = "" 'giu310112
    'giu030512 
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Me.Title = Request.QueryString("labelForm")
        Catch ex As Exception
        End Try
    End Sub
    'prova
    'Protected Overrides Sub AddedControl(ByVal control As Control, ByVal index As Integer)
    '    'If (Request.ServerVariables("http_user_agent").IndexOf("Safari", StringComparison.CurrentCultureIgnoreCase) <> -1) Then
    '    '    Me.Page.ClientTarget = "uplevel" '_blank
    '    'End If
    '    Me.Page.ClientTarget = "_blank" '_blank
    '    MyBase.AddedControl(control, index)
    'End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            If Session(CSTNOBACK) = 1 Then
                btnRitorno.Visible = False
                Label1.Visible = False
            End If
        End If
        '-
        Dim SWTipoStampa As Integer = 0
        If Not String.IsNullOrEmpty(Session(CSTTipoStampaIN)) Then
            If IsNumeric(Session(CSTTipoStampaIN)) Then
                SWTipoStampa = Session(CSTTipoStampaIN)
            End If
        End If
        '-
        Dim Rpt As Object = Nothing
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        DsPrinWebDoc = Session(CSTDsPrinWebDoc) '"DsPrinWebDoc")
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        CodiceDitta = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            CodiceDitta = ""
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            CodiceDitta = ""
        End If
        '---------------------
        CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"

        If SWTipoStampa = TipoStampaIN.SingoloIN Then
            Rpt = New SchedaIN
            If CodiceDitta = "01" Then
                Rpt = New SchedaIN '01
            ElseIf CodiceDitta = "05" Then
                Rpt = New SchedaIN '05
            End If
        ElseIf SWTipoStampa = TipoStampaIN.ElencoIN Then
            Chiudi("Errore: STAMPA ELENCO SCHEDE INVENTARIO IN SVILUPPO")
        Else
            Chiudi("Errore: TIPO STAMPA SCONOSCIUTA")
        End If
        'ok
        Rpt.SetDataSource(DsPrinWebDoc)
        CrystalReportViewer1.ReportSource = Rpt
    End Sub

    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String = "WF_ElencoSchedeInventario.aspx?labelForm=Gestione schede inventario"
        Try
            Response.Redirect(strRitorno)
            Exit Sub
        Catch ex As Exception
            Response.Redirect(strRitorno)
            Exit Sub
        End Try
    End Sub

    Private Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        Else
            Dim strRitorno As String = "WF_ElencoSchedeInventario.aspx?labelForm=Gestione schede inventario"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        End If
    End Sub

End Class