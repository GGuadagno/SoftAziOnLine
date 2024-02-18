Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWebOrdinato
    Inherits System.Web.UI.Page
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticolo Then 'ORDINATO PER ARTICOLO
            Dim Rpt As New OrdArt
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloData Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New OrdArtData
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCoge Then
            Dim Rpt As New OrdCli
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSoc Then
            Dim Rpt As New OrdCli_RagSoc
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloCliente Then
            Dim Rpt As New StOrdArtCli
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteFor Then
            Dim Rpt As New StOrdArtCliFor
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteForS Then
            Dim Rpt As New StOrdArtCliForSintetico
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloFornitore Then 'giu110612
            Dim Rpt As New StOrdArtFor
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdForOrdTutti Then 'giu281013
            Dim Rpt As New StatOrdinatoFornOrdine
            Dim dsStatOrdinatoClienteOrdine1 As dsStatOrdinatoClienteOrdine
            dsStatOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsStatOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdine Then
            Dim Rpt As New OrdinatoClienteOrdine
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCarico Then
            Dim Rpt As New ListaCarico
            Dim DSListaCarico1 As New DSListaCarico
            DSListaCarico1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSListaCarico1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione Then
            Dim Rpt As New ListaCaricoSpedizione
            Dim DSListaCarico1 As New DSListaCarico
            DSListaCarico1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSListaCarico1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDoc Then
            Dim Rpt As New OrdinatoOrdineSortByNDoc
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDoc Then
            Dim Rpt As New OrdinatoOrdineSortByDataDoc
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegna Then
            Dim Rpt As New OrdinatoOrdineSortByDataConsegna
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCogeAg Then
            Dim Rpt As New OrdCliAg
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSocAg Then
            Dim Rpt As New OrdCli_RagSocAg
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloAg Then
            Dim Rpt As New OrdArtAG
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloDataAg Then
            Dim Rpt As New OrdArtDataAG
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteAg Then
            Dim Rpt As New StOrdArtCliAg
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdineAG Then
            Dim Rpt As New OrdinatoClienteOrdineAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDocAG Then
            Dim Rpt As New OrdinatoOrdineSortByNDocAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDocAG Then
            Dim Rpt As New OrdinatoOrdineSortByDataDocAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegnaAG Then
            Dim Rpt As New OrdinatoOrdineSortByDataConsegnaAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdinatoClienteOrdine Then
            Dim Rpt As New StatOrdinatoClienteOrdine
            Dim DSStatOrdinatoClienteOrdine1 As New dsStatOrdinatoClienteOrdine
            DSStatOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSStatOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAG Or _
                Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAGCA Then 'giu080421 GIU060823
            Dim Rpt As New PrevClienteOrdineAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAG Or _
               Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAGCA Then 'giu050722 GIU060823
            Dim Rpt As New PrevClienteOrdineAGPrev
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineLS Then 'giu190421
            Dim Rpt As New PrevClienteOrdineLS
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
        Else
            Chiudi("Errore: TIPO STAMPA ORDINATO SCONOSCIUTA")
        End If
    End Sub


    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String

        If Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticolo Or Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloData Then
            strRitorno = "WF_OrdinatoPerArticolo.aspx?labelForm=Ordinato per articolo"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCoge Or Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSoc Then
            strRitorno = "WF_OrdinatoPerCliente.aspx?labelForm=Ordinato per cliente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloCliente Then
            strRitorno = "WF_OrdineStampaCliArt.aspx?labelForm=Ordinato per articolo/cliente/Tipo Evasione"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteFor Then
            strRitorno = "WF_OrdineStampaCliArt.aspx?labelForm=Ordinato per articolo/cliente/Tipo Evasione [Fornitore]"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteForS Then
            strRitorno = "WF_OrdineStampaCliArt.aspx?labelForm=Ordinato per articolo/cliente/Tipo Evasione [Fornitore]"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdine Then
            strRitorno = "WF_OrdinatoClienteOrdine.aspx?labelForm=Ordinato per cliente/ordine"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCarico Then
            If Session(CSTTIPODOC) = SWTD(TD.DocTrasportoClienti) Then
                strRitorno = "WF_DocumentiElenco.aspx?labelForm=Gestione documenti"
            Else
                strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
            End If
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione Then
            strRitorno = "WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDoc Then
            strRitorno = "WF_OrdinatoClienteOrdine.aspx?labelForm=Ordinato per ordine"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDoc Then
            strRitorno = "WF_OrdinatoClienteOrdine.aspx?labelForm=Ordinato per ordine"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegna Then
            strRitorno = "WF_OrdinatoClienteOrdine.aspx?labelForm=Ordinato per ordine"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCogeAg Then
            strRitorno = "WF_OrdinatoPerClienteAG.aspx?labelForm=Ordinato per cliente per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSocAg Then
            strRitorno = "WF_OrdinatoPerClienteAG.aspx?labelForm=Ordinato per cliente per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloAg Then
            strRitorno = "WF_OrdinatoPerArticoloAG.aspx?labelForm=Ordinato per articolo per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloDataAg Then
            strRitorno = "WF_OrdinatoPerArticoloAG.aspx?labelForm=Ordinato per articolo per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteAg Then
            strRitorno = "WF_OrdineStampaCliArtAG.aspx?labelForm=Ordinato per articolo/cliente per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdineAG Then
            strRitorno = "WF_OrdinatoClienteOrdineAG.aspx?labelForm=Ordinato per ordine per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDocAG Then
            strRitorno = "WF_OrdinatoClienteOrdineAG.aspx?labelForm=Ordinato per cliente/ordine per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDocAG Then
            strRitorno = "WF_OrdinatoClienteOrdineAG.aspx?labelForm=Ordinato per cliente/ordine per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegnaAG Then
            strRitorno = "WF_OrdinatoClienteOrdineAG.aspx?labelForm=Ordinato per cliente/ordine per agente"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdinatoClienteOrdine Then
            strRitorno = "WF_StatOrdinatoClienteOrdine.aspx?labelForm=Statistica ordinato per [Regione/Provincia/Agente] cliente/ordine [Tutti]"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloFornitore Then
            strRitorno = "WF_OrdineStampaForArt.aspx?labelForm=Ordinato per articolo/fornitore"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdForOrdTutti Then
            strRitorno = "WF_StatOrdForOrdTutti.aspx?labelForm=Statistica ordinato per fornitore/N°documento [Tutti]"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAG Or Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAG Then 'giu090421 giu050722
            strRitorno = "WF_PrevClienteOrdineAG.aspx?labelForm=Preventivi per Agente/Cliente/Preventivo - Stato:[Conf./Da Conf... Conteggio Totali]"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAGCA Or Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAGCA Then 'GIU060823
            strRitorno = "WF_MenuStatisPR.aspx?labelForm=Preventivi per Agente/Cliente/Preventivo - Stato:[Conf./Da Conf... Conteggio Totali]"
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineLS Then 'giu190421
            strRitorno = "WF_PrevClienteOrdineAG.aspx?labelForm=Preventivi per Lead Source/Cliente/Preventivo - Stato:[Conf./Da Conf... Conteggio Totali]"
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