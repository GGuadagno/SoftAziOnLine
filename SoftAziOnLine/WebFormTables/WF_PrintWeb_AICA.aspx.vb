Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.Model.Entity 'giu150312
Imports It.SoftAzi.Model.Facade 'Ho tutte le funzioni es. get_Operatori
Imports It.SoftAzi.Model.Entity.OperatoriEntity

Imports It.SoftAzi.SystemFramework
'GIU230514
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports System.IO
'---------

Partial Public Class WF_PrintWeb_AICA
    Inherits System.Web.UI.Page

    Private CodiceDitta As String = "" 'giu310112
    
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
                lblMessaggi.Visible = False
            End If
        End If
        '-
        Dim SWTipoStampa As Integer = -1
        If Not String.IsNullOrEmpty(Session(CSTTipoStampaAICA)) Then
            If IsNumeric(Session(CSTTipoStampaAICA)) Then
                SWTipoStampa = Session(CSTTipoStampaAICA)
            End If
        End If
        '-
        'GIU230514 Dim Rpt As Object = Nothing
        Dim Rpt As ReportClass
        '-------------------------------------
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

        If SWTipoStampa = TipoStampaAICA.SingoloAICA Then
            Rpt = New SchedaAICA
            'Uguale per tutte le eventuali aziende, diversamente cambiare il RPT per il CodiceDitta
            If CodiceDitta = "01" Then
                Rpt = New SchedaAICA '01
            ElseIf CodiceDitta = "05" Then
                Rpt = New SchedaAICA '05
            End If
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICACliArtNSerie Then
            Rpt = New ElencoAICliArtNSerie
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAArtCliNSerie Then
            Rpt = New ElencoAIArtCliNSerie
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScGaArtCliNSerie Then
            Rpt = New ElencoAIScGaArtCliNSerie
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScElArtCliNSerie Then
            Rpt = New ElencoAIScElArtCliNSerie
        ElseIf SWTipoStampa = TipoStampaAICA.ElencoAICAScBaArtCliNSerie Then
            Rpt = New ElencoAIScBaArtCliNSerie
        Else
            Chiudi("Errore: TIPO STAMPA SCONOSCIUTA")
            Exit Sub
        End If
        'ok
        Rpt.SetDataSource(DsPrinWebDoc)
        CrystalReportViewer1.ReportSource = Rpt
    End Sub

    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String = "WF_ArticoliInst_ContrattiAssElenco.aspx?labelForm=Articoli consumabili Clienti"
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
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        End If
    End Sub

End Class