Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework
Imports CrystalDecisions.Shared
Imports System.IO

Partial Public Class WF_PrintWebStatistiche
    Inherits System.Web.UI.Page
    Private Sub WF_PrintWebStatistiche_Load(sender As Object, e As EventArgs) Handles Me.Load
        'GIU100324
        Try
            '''Dim strLabelForm As String = Request.QueryString("labelForm")
            '''If InStr(strLabelForm.Trim.ToUpper, "ESPORTA") > 0 Then
            '''    'OK PROSEGUO
            '''Else
            '''    VisualizzaRpt(Session("StampaMovMag"), Session("NomeRpt"))
            '''    Exit Sub
            '''End If
            '-NON va bene per il NOBACK 
            '''If Not String.IsNullOrEmpty(Session(CSTNOBACK)) Then
            '''    If Session(CSTNOBACK) = 1 Then
            '''        LnkRitorno.Visible = False
            '''        VisualizzaRpt(Session("StampaMovMag"), Session("NomeRpt"))
            '''        Exit Sub
            '''    End If
            '''End If
        Catch ex As Exception
        End Try
        '-
        If IsPostBack Then
            If Request.Params.Get("__EVENTTARGET").ToString = "LnkStampaOK" Then
                'Dim arg As String = Request.Form("__EVENTARGUMENT").ToString
                VisualizzaRpt(Session("StampaMovMag"), Session("NomeRpt"))
                Exit Sub
            End If
            If Request.Params.Get("__EVENTTARGET").ToString = "LnkRitornoOK" Then
                'Dim arg As String = Request.Form("__EVENTARGUMENT").ToString
                subRitorno()
                Exit Sub
            End If
        End If
    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticolo Then 'ORDINATO PER ARTICOLO
            Dim Rpt As New StCliArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoClienteArticolo"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloCliente Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StArtCli
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoArticoloCliente"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloREG Then 'ORDINATO PER ARTICOLO
            Dim Rpt As New StCliArtREG
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoClienteArticoloREG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteREG Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StArtCliREG
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoArticoloClienteREG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.ControlloVendutoCVByArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New ControlloCostoVendutoFIFOST
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "ControlloVendutoCVByArt"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoDDT Then
            Dim rpt As New RiepVenduto
            Dim dsReport As New DSStatRiepVendutoNumero
            dsReport = Session("dsVendutoDDT")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
            'giu090324
            Session("NomeRpt") = "VendutoDDT"
            getOutputRPT(rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.FatturatoAgenteAnalitico Then
            Dim rpt As New FatturatoAgenteAnalitico 'FattAgenteAnalitico
            Dim dsReport As New dsFattAgente
            dsReport = Session("dsFattAgente")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
            'giu090324
            Session("NomeRpt") = "FatturatoAgenteAnalitico"
            getOutputRPT(rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.FatturatoAgenteSintetico Then
            Dim rpt As New FatturatoAgenteSintetico 'FattAgenteSintetico
            Dim dsReport As New dsFattAgente
            dsReport = Session("dsFattAgente")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
            'giu090324
            Session("NomeRpt") = "FatturatoAgenteSintetico"
            getOutputRPT(rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloAG Then 'ORDINATO PER ARTICOLO PER AGENTE
            Dim Rpt As New StCliArtAG
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoClienteArticoloAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteAG Then 'ORDINATO PER ARTICOLO DATA PER AGENTE
            Dim Rpt As New StArtCliAG
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoArticoloClienteAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoDDTAG Then
            Dim rpt As New RiepVendutoAG
            Dim dsReport As New DSStatRiepVendutoNumero
            dsReport = Session("dsVendutoDDT")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
            'giu090324
            Session("NomeRpt") = "VendutoDDTAG"
            getOutputRPT(rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.IncidenzaNCFatturato Then
            Dim rpt As New rptIncNCFatt
            Dim dsReport As New DsStatVendCliArt
            dsReport = Session("DSIncNCFatt")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
            'giu090324
            Session("NomeRpt") = "IncidenzaNCFatturato"
            getOutputRPT(rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoForArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StArtCliForSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoForArt"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoForArtCli Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StArtCliFor
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoForArtCli"
            getOutputRPT(Rpt)
            'giu020216
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloCC Then 'ORDINATO PER ARTICOLO PER categoria
            Dim Rpt As New StCliArtCC
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoClienteArticoloCC"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteCC Then 'ORDINATO PER ARTICOLO  PER categoria
            Dim Rpt As New StArtCliCC
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoArticoloClienteCC"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgFortArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendAgForArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoAgFortArt"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegFortArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendRegForArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoRegFortArt"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegioneCateg Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendRegCatArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoRegioneCateg"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegioneCategSintetico Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendRegCatArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoRegioneCategSintetico"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoCliForArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendCliForArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoCliForArt"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteCCSintetico Then
            Dim Rpt As New StCliArtCCSint
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoArticoloClienteCCSintetico"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgenteCateg Then
            Dim Rpt As New StVendAgCategArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoAgenteCateg"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoCliForArtSintetico Then
            Dim Rpt As New StVendCliForArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoCliForArtSintetico"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgenteCategSintetico Then
            Dim Rpt As New StVendAgCategArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoAgenteCategSintetico"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegionePRCategCliArt Then
            Dim Rpt As New StVendRegPRCatCliArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
           ' CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "VendutoRegionePRCategCliArt"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatTotaliClientiAl Then
            Dim Rpt As New StClientiMovForReg
            Dim DsClienti1 As New dsClienti
            DsClienti1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsClienti1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "StatTotaliClientiAl"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPr Then
            Dim Rpt As New StatFattAnnoMese
            Dim DsClienti1 As New dsClienti
            DsClienti1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsClienti1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "StatFattAnnoMeseInPr"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt Then
            Dim Rpt As New StatFattAnnoMeseArt
            Dim dsFattCliFatt1 As New DSFatturatoClienteFattura
            dsFattCliFatt1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFattCliFatt1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "StatFattAnnoMeseInPrArt"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.InstallatoClientiArticolo Then
            Dim Rpt As New ElencoInstallatoClienti
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            CrystalReportViewer1.DisplayGroupTree = False
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "InstallatoClientiArticolo"
            getOutputRPT(Rpt)
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatTotaliContrattiAl Then
            Dim Rpt As New ElencoCMAttiviNEWSCAD
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "StatTotaliContrattiAl"
            getOutputRPT(Rpt)
        Else
            Chiudi("Errore: TIPO STAMPA STATISTICHE SCONOSCIUTA")
        End If
        If IsNothing(Session("StampaMovMag")) Then
            lblVuota.Visible = True
        End If
    End Sub
    Private Sub subRitorno()
        Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"

        If Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloCliente Or
           Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteREG Then
            strRitorno = "WF_StatVendCliArt.aspx?labelForm=Venduto per cliente/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticolo Or
           Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloREG Then
            strRitorno = "WF_StatVendCliArt.aspx?labelForm=Venduto per articolo/cliente"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.ControlloVendutoCVByArt Then
            strRitorno = "WF_StatVendCliArt.aspx?labelForm=Venduto per articolo/cliente"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoDDT Then
            strRitorno = "WF_StatRiepVendNumero.aspx?labelForm=Riepilogo venduto per DDT"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.FatturatoAgenteAnalitico Then
            strRitorno = "WF_FattAgenteCliente.aspx?labelForm=Fatturato per agente analitico"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.FatturatoAgenteSintetico Then
            strRitorno = "WF_FattAgenteCliente.aspx?labelForm=Fatturato per agente sintetico"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteAG Then
            strRitorno = "WF_StatVendCliArtAG.aspx?labelForm=Venduto/Fatturato per articolo/cliente per agente"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloAG Then
            strRitorno = "WF_StatVendCliArtAG.aspx?labelForm=Venduto/Fatturato per cliente/articolo per agente"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoDDTAG Then
            strRitorno = "WF_StatRiepVendNumeroAG.aspx?labelForm=Venduto cliente per DDT per agente"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.IncidenzaNCFatturato Then
            strRitorno = "WF_IncNCFatturato.aspx?labelForm=Incidenza NC su fatturato/Analisi ABC"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoForArt Or
            Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoForArtCli Then
            strRitorno = "WF_StatVendForArt.aspx?labelForm=Venduto/Fatturato per fornitore/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteCC Then
            strRitorno = "WF_StatVendCliArtCateg.aspx?labelForm=Venduto/Fatturato per categoria/articolo/cliente"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteCCSintetico Then
            strRitorno = "WF_StatVendCliArtCategSintetico.aspx?labelForm=Venduto/Fatturato per categoria/articolo/cliente sintetico"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloCC Then
            strRitorno = "WF_StatVendCliArtCateg.aspx?labelForm=Venduto/Fatturato per categoria/cliente/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgFortArt Then
            strRitorno = "WF_StatVendAgForArt.aspx?labelForm=Venduto/Fatturato per agente/fornitore/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgenteCateg Then
            strRitorno = "WF_StatVendAgCateg.aspx?labelForm=Venduto/Fatturato per agente/categoria/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoCliForArt Then
            strRitorno = "WF_StatVendCliForArt.aspx?labelForm=Venduto/Fatturato per cliente/fornitore/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegFortArt Then
            strRitorno = "WF_StatVendRegForArt.aspx?labelForm=Venduto/Fatturato per regione/fornitore/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegioneCateg Then
            strRitorno = "WF_StatVendRegCatArt.aspx?labelForm=Venduto/Fatturato per regione/categoria/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegioneCategSintetico Then
            strRitorno = "WF_StatVendRegCatArt.aspx?labelForm=Venduto/Fatturato per regione/categoria/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgenteCateg Then
            strRitorno = "WF_StatVendAgCategForArt.aspx?labelForm=Venduto/Fatturato per agente/categoria/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoCliForArtSintetico Then
            strRitorno = "WF_StatVendCliForArt.aspx?labelForm=Venduto/Fatturato per cliente/fornitore/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgenteCategSintetico Then
            strRitorno = "WF_StatVendAgCateg.aspx?labelForm=Venduto/Fatturato per agente/categoria/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegionePRCategCliArt Then
            strRitorno = "WF_StatVendRegCatCliArt.aspx?labelForm=Venduto/Fatturato per regione/provincia/categoria/cliente/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatTotaliClientiAl Then
            strRitorno = "WF_StatClientiMovimNuovi.aspx?labelForm=Statistica Clienti Movimentati/Nuovi per Esercizio/Regione"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPr Then
            strRitorno = "WF_StatFatturatoMese.aspx?labelForm=Fatturato Anno/Mese In corso/Precedente"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt Then
            strRitorno = "WF_StatFatturatoMese.aspx?labelForm=Fatturato Articoli Anno/Mese In corso/Precedente"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.InstallatoClientiArticolo Then
            strRitorno = "WF_StatVendCliForArt.aspx?labelForm=Estrazione foglio EXCEL Installato per Cliente/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatTotaliContrattiAl Then
            strRitorno = "WF_StatClientiMovimNuovi.aspx?labelForm=Statistica Contratti Attivi/Nuovi/In Scadenza per Esercizio"
        Else
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        End If

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
            Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
            Try
                Response.Redirect(strRitorno)
                Exit Sub
            Catch ex As Exception
                Response.Redirect(strRitorno)
                Exit Sub
            End Try
        End If
    End Sub

    Private Function getOutputRPT(ByVal _Rpt As Object) As Boolean
        '_Rpt.Refresh()
        Dim myStream As Stream
        Try
            '''If _Formato = ReportFormatEnum.Pdf Then
            '''    myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            '''ElseIf _Formato = ReportFormatEnum.Excel Then
            '''    myStream = _Rpt.ExportToStream(ExportFormatType.Excel)
            '''End If
            myStream = _Rpt.ExportToStream(ExportFormatType.PortableDocFormat)
            Dim byteReport() As Byte = GetStreamAsByteArray(myStream)
            Session("StampaMovMag") = byteReport
        Catch ex As Exception
            Return False
        End Try

        Try
            GC.WaitForPendingFinalizers()
            GC.Collect()
        Catch
        End Try
        getOutputRPT = True
    End Function

    Private Shared Function GetStreamAsByteArray(ByVal stream As System.IO.Stream) As Byte()

        Dim streamLength As Integer = Convert.ToInt32(stream.Length)

        Dim fileData As Byte() = New Byte(streamLength) {}

        ' Read the file into a byte array
        stream.Read(fileData, 0, streamLength)
        stream.Close()

        Return fileData
    End Function
    Private Sub VisualizzaRpt(ByVal byteReport() As Byte, ByVal _NomeRpt As String)
        Dim sErrore As String = ""
        Try
            If byteReport.Length > 0 Then
                With Me.Page
                    Response.Clear()
                    Response.Buffer = True
                    Response.ClearHeaders()

                    Response.AddHeader("Accept-Header", byteReport.Length.ToString())
                    Response.AddHeader("Cache-Control", "private")
                    Response.AddHeader("cache-control", "max-age=1")
                    Response.AddHeader("content-length", byteReport.Length.ToString())
                    Response.AppendHeader("content-disposition", "inline; filename=" & "" & _NomeRpt & ".pdf")
                    'Response.AppendHeader("content-disposition", "attachment; filename=" & "RicevutaAcquisto_" & sCodiceTransazione & ".pdf")      ' per download diretto
                    Response.AddHeader("Expires", "0")
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("Accept-Ranges", "bytes")

                    Response.BinaryWrite(byteReport)
                    Response.Flush()
                    Response.End()
                End With
            Else
                lblVuota.Visible = True
            End If
        Catch ex As Exception
            lblVuota.Visible = True
        End Try
    End Sub
End Class