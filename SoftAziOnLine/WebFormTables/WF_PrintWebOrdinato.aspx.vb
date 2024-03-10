Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework
Imports CrystalDecisions.Shared
Imports System.IO
Imports AjaxControlToolkit.AsyncFileUpload.Constants

Partial Public Class WF_PrintWebOrdinato
    Inherits System.Web.UI.Page
    Private TipoDoc As String = "" : Private TabCliFor As String = ""

    Private Sub WF_PrintWebOrdinato_Load(sender As Object, e As EventArgs) Handles Me.Load
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
        If Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticolo Then 'ORDINATO PER ARTICOLO
            Dim Rpt As New OrdArt
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticolo"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloData Then 'ORDINATO PER ARTICOLO DATA
            Dim Rpt As New OrdArtData
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticoloData"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCoge Then
            Dim Rpt As New OrdCli
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoClienteCodiceCoge"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSoc Then
            Dim Rpt As New OrdCli_RagSoc
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoClienteRagSoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloCliente Then
            Dim Rpt As New StOrdArtCli
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticoloCliente"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteFor Then
            Dim Rpt As New StOrdArtCliFor
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticoloClienteFor"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteForS Then
            Dim Rpt As New StOrdArtCliForSintetico
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticoloClienteForS"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloFornitore Then 'giu110612
            Dim Rpt As New StOrdArtFor
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticoloFornitore"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdForOrdTutti Then 'giu281013
            Dim Rpt As New StatOrdinatoFornOrdine
            Dim dsStatOrdinatoClienteOrdine1 As dsStatOrdinatoClienteOrdine
            dsStatOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(dsStatOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "StatOrdForOrdTutti"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdine Then
            Dim Rpt As New OrdinatoClienteOrdine
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoClienteOrdine"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCarico Then
            Dim Rpt As New ListaCarico
            Dim DSListaCarico1 As New DSListaCarico
            DSListaCarico1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSListaCarico1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "ListaCarico"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.ListaCaricoSpedizione Then
            Dim Rpt As New ListaCaricoSpedizione
            Dim DSListaCarico1 As New DSListaCarico
            DSListaCarico1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSListaCarico1)
            CrystalReportViewer1.DisplayGroupTree = False
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "ListaCaricoSpedizione"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDoc Then
            Dim Rpt As New OrdinatoOrdineSortByNDoc
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoOrdineSortByNDoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDoc Then
            Dim Rpt As New OrdinatoOrdineSortByDataDoc
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoOrdineSortByDataDoc"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegna Then
            Dim Rpt As New OrdinatoOrdineSortByDataConsegna
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoOrdineSortByDataConsegna"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteCodiceCogeAg Then
            Dim Rpt As New OrdCliAg
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoClienteCodiceCogeAg"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteRagSocAg Then
            Dim Rpt As New OrdCli_RagSocAg
            Dim DSOrdinatoPerCliente1 As New DSOrdinatoPerCliente
            DSOrdinatoPerCliente1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoPerCliente1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoClienteRagSocAg"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloAg Then
            Dim Rpt As New OrdArtAG
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticoloAg"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloDataAg Then
            Dim Rpt As New OrdArtDataAG
            Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
            DSOrdinatoArticolo1 = Session(CSTDSOrdinatoArticolo)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArticolo1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticoloDataAg"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoArticoloClienteAg Then
            Dim Rpt As New StOrdArtCliAg
            Dim DSOrdinatoArtCli1 As New DSOrdinatoArtCli
            DSOrdinatoArtCli1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoArtCli1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoArticoloClienteAg"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoClienteOrdineAG Then
            Dim Rpt As New OrdinatoClienteOrdineAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoClienteOrdineAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByNDocAG Then
            Dim Rpt As New OrdinatoOrdineSortByNDocAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoOrdineSortByNDocAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataDocAG Then
            Dim Rpt As New OrdinatoOrdineSortByDataDocAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoOrdineSortByDataDocAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.OrdinatoOrdineSortByDataConsegnaAG Then
            Dim Rpt As New OrdinatoOrdineSortByDataConsegnaAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "OrdinatoOrdineSortByDataConsegnaAG"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.StatOrdinatoClienteOrdine Then
            Dim Rpt As New StatOrdinatoClienteOrdine
            Dim DSStatOrdinatoClienteOrdine1 As New dsStatOrdinatoClienteOrdine
            DSStatOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSStatOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "StatOrdinatoClienteOrdine"
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAG Or
                Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAGCA Then 'giu080421 GIU060823
            Dim Rpt As New PrevClienteOrdineAG
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            If Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineAG Then
                Session("NomeRpt") = "PrevClienteOrdineAG"
            ElseIf TIPOSTAMPAORDINATO.PrevClienteOrdineAGCA Then
                Session("NomeRpt") = "PrevClienteOrdineAGCA"
            End If
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAG Or
               Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAGCA Then 'giu050722 GIU060823
            Dim Rpt As New PrevClienteOrdineAGPrev
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            If Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevOrdineClienteAG Then
                Session("NomeRpt") = "PrevOrdineClienteAG"
            ElseIf TIPOSTAMPAORDINATO.PrevOrdineClienteAGCA Then
                Session("NomeRpt") = "PrevOrdineClienteAGCA"
            End If
            getOutputRPT(Rpt)
        ElseIf Session(CSTORDINATO) = TIPOSTAMPAORDINATO.PrevClienteOrdineLS Then 'giu190421
            Dim Rpt As New PrevClienteOrdineLS
            Dim DSOrdinatoClienteOrdine1 As New DSOrdinatoClienteOrdine
            DSOrdinatoClienteOrdine1 = Session(CSTDsPrinWebDoc)
            CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
            Rpt.SetDataSource(DSOrdinatoClienteOrdine1)
            CrystalReportViewer1.DisplayGroupTree = True
            CrystalReportViewer1.ReportSource = Rpt
            'giu090324
            Session("NomeRpt") = "PrevClienteOrdineLS"
            getOutputRPT(Rpt)
        Else
            Chiudi("Errore: TIPO STAMPA ORDINATO SCONOSCIUTA")
        End If
        If String.IsNullOrEmpty(Session("MovMag")) Then
            lblVuota.Visible = True
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
    Private Sub subRitorno()
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