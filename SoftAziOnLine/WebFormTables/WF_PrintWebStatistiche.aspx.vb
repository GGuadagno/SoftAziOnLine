Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWebStatistiche
    Inherits System.Web.UI.Page
    'giu030512 
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticolo Then 'ORDINATO PER ARTICOLO
            Dim Rpt As New StCliArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloCliente Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StArtCli
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
            'GIU161012
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloREG Then 'ORDINATO PER ARTICOLO
            Dim Rpt As New StCliArtREG
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteREG Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StArtCliREG
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
            'GIU051112
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.ControlloVendutoCVByArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New ControlloCostoVendutoFIFOST
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoDDT Then
            Dim rpt As New RiepVenduto
            Dim dsReport As New DSStatRiepVendutoNumero
            dsReport = Session("dsVendutoDDT")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.FatturatoAgenteAnalitico Then
            Dim rpt As New FatturatoAgenteAnalitico 'FattAgenteAnalitico
            Dim dsReport As New dsFattAgente
            dsReport = Session("dsFattAgente")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.FatturatoAgenteSintetico Then
            Dim rpt As New FatturatoAgenteSintetico 'FattAgenteSintetico
            Dim dsReport As New dsFattAgente
            dsReport = Session("dsFattAgente")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloAG Then 'ORDINATO PER ARTICOLO PER AGENTE
            Dim Rpt As New StCliArtAG
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteAG Then 'ORDINATO PER ARTICOLO DATA PER AGENTE
            Dim Rpt As New StArtCliAG
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoDDTAG Then
            Dim rpt As New RiepVendutoAG
            Dim dsReport As New DSStatRiepVendutoNumero
            dsReport = Session("dsVendutoDDT")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.IncidenzaNCFatturato Then
            Dim rpt As New rptIncNCFatt
            Dim dsReport As New DsStatVendCliArt
            dsReport = Session("DSIncNCFatt")
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            rpt.SetDataSource(dsReport)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoForArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StArtCliForSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'GIU161012
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoForArtCli Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StArtCliFor
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'GIU161012
            'giu020216
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticoloCC Then 'ORDINATO PER ARTICOLO PER categoria
            Dim Rpt As New StCliArtCC
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteCC Then 'ORDINATO PER ARTICOLO  PER categoria
            Dim Rpt As New StArtCliCC
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgFortArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendAgForArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'GIU030216
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegFortArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendRegForArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'FABIO09022016
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegioneCateg Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendRegCatArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegioneCategSintetico Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendRegCatArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoCliForArt Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New StVendCliForArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'FABIO05022016
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteCCSintetico Then
            Dim Rpt As New StCliArtCCSint
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'FABIO05022016
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgenteCateg Then
            Dim Rpt As New StVendAgCategArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'FABIO09022016
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoCliForArtSintetico Then
            Dim Rpt As New StVendCliForArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'FABIO19022016
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoAgenteCategSintetico Then
            Dim Rpt As New StVendAgCategArtSintetico
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'FABIO19022016
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoRegionePRCategCliArt Then
            Dim Rpt As New StVendRegPRCatCliArt
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatTotaliClientiAl Then
            Dim Rpt As New StClientiMovForReg
            Dim DsClienti1 As New dsClienti
            DsClienti1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsClienti1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPr Then
            Dim Rpt As New StatFattAnnoMese
            Dim DsClienti1 As New dsClienti
            DsClienti1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsClienti1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatFattAnnoMeseInPrArt Then
            Dim Rpt As New StatFattAnnoMeseArt
            Dim dsFattCliFatt1 As New DSFatturatoClienteFattura
            dsFattCliFatt1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsFattCliFatt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.InstallatoClientiArticolo Then
            Dim Rpt As New ElencoInstallatoClienti
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDSStatVendCliArt)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.StatTotaliContrattiAl Then
            Dim Rpt As New ElencoCMAttiviNEWSCAD
            Dim DsStatVendCliArt1 As New DsStatVendCliArt
            DsStatVendCliArt1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DsStatVendCliArt1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        Else
            Chiudi("Errore: TIPO STAMPA STATISTICHE SCONOSCIUTA")
        End If


    End Sub


    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"

        If Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloCliente Or _
           Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoArticoloClienteREG Then
            strRitorno = "WF_StatVendCliArt.aspx?labelForm=Venduto per cliente/articolo"
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoClienteArticolo Or _
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
        ElseIf Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoForArt Or _
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
End Class