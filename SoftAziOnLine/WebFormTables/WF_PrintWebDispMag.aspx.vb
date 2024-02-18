Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.SystemFramework
'giu280315
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Imports System.IO 'giu140615

Partial Public Class WF_PrintWebdispMag
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            Dim Rpt As ReportClass
            'giu310112 gestione su piu aziende inserito controllo del codice ditta
            Dim CodiceDitta As String = Session(CSTCODDITTA)
            If IsNothing(CodiceDitta) Then
                CodiceDitta = ""
            End If
            If String.IsNullOrEmpty(CodiceDitta) Then
                CodiceDitta = ""
            End If
            '---------------------
            Session(CSTNOMEPDF) = ""
            If Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzino Then
                Session(CSTNOMEPDF) = "DisponibilitaMagazzino.pdf"
                Rpt = New DispMag
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzinoFornitori Then
                Session(CSTNOMEPDF) = "DisponibilitaMagazzinoFornitori.pdf"
                Rpt = New DispMagForn
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOAnalitico Then
                Session(CSTNOMEPDF) = "ValMagFIFOAnalitico.pdf"
                Rpt = New ValMagFIFO
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSintetico Then
                Session(CSTNOMEPDF) = "ValMagFIFOSintetico.pdf"
                Rpt = New ValMagFIFOS
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoFor Then
                Session(CSTNOMEPDF) = "ValMagFIFOSinteticoFor.pdf"
                Rpt = New ValMagFIFOSFor
                'GIU211121 ANCHE SENON CI PASSERA MAI
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoForS Then
                Session(CSTNOMEPDF) = "ValMagFIFOSinteticoForSint.pdf"
                Rpt = New ValMagFIFOSForSint
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqAnalitico Then
                Session(CSTNOMEPDF) = "ValMagUltPrzAcqAnalitico.pdf"
                Rpt = New ValMagUltPrzAcq
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqSintetico Then
                Session(CSTNOMEPDF) = "ValMagUltPrzAcqSintetico.pdf"
                Rpt = New ValMagUltPrzAcqS
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondAnalitico Then
                Session(CSTNOMEPDF) = "ValMagMediaPondAnalitico.pdf"
                Rpt = New ValMagMediaPond
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondSintetico Then
                Session(CSTNOMEPDF) = "ValMagMediaPondSintetico.pdf"
                Rpt = New ValMagMediaPondS
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFO Then
                Session(CSTNOMEPDF) = "ValMagCostoVendFIFO.pdf"
                Rpt = New ValMagCostoVendutoFIFO
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFOSint Then
                Session(CSTNOMEPDF) = "ValMagCostoVendFIFOSint.pdf"
                Rpt = New ValMagCostoVendutoFIFOSint
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOAnalitico Then
                Session(CSTNOMEPDF) = "ValMagLIFOAnalitico.pdf"
                Rpt = New ValMagLIFO
            ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOSintetico Then
                Session(CSTNOMEPDF) = "ValMagLIFOSintetico.pdf"
                Rpt = New ValMagLIFOS
            Else
                Chiudi("Errore: TIPO STAMPA DI MAGAZZINO SCONOSCIUTA")
                Exit Sub
            End If
            '-----------------------------------
            Dim DsMovMag1 As New DsMagazzino
            DsMovMag1 = Session(CSTDsPrinWebDoc)
            Rpt.SetDataSource(DsMovMag1)
            'Per evitare che solo un utente possa elaborare le stampe
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            Session(CSTNOMEPDF) = Format(Now, "yyyyMMddHHmmss") + Utente.Codice.Trim & Session(CSTNOMEPDF)
            '---------
            '' ''GIU230514 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ pdf FUZIONA PS LA DIR _RPT Ã¨ SUL SERVER,MA BISOGNA AVERE I PERMESSI
            Session(CSTESPORTAPDF) = True
            Session(CSTPATHPDF) = ConfigurationManager.AppSettings("AppPathPDF") & "StatMag\"
            Dim stPathReport As String = Session(CSTPATHPDF)
            Try 'giu281112 errore che il file Ã¨ gia aperto
                Rpt.ExportToDisk(ExportFormatType.PortableDocFormat, Trim(stPathReport & Session(CSTNOMEPDF)))
                'giu140124
                Rpt.Close()
                Rpt.Dispose()
                Rpt = Nothing
                '-
                GC.WaitForPendingFinalizers()
                GC.Collect()
                '-------------
            Catch ex As Exception
                Rpt = Nothing
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Stampa valorizzazione magazzino", "Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Chiudi("Errore in esporta PDF: " & Session(CSTNOMEPDF) & " " & ex.Message)
                Exit Sub
            End Try
            'giu140615 Dim LnkName As String = ConfigurationManager.AppSettings("AppPath") & "/Documenti/StatMag/" & Session(CSTNOMEPDF)
            Dim LnkName As String = "~/Documenti/StatMag/" & Session(CSTNOMEPDF)
            LnkStampa.HRef = LnkName
            'giu110320 giu180320 TROPPO LENTO
            ' ''LnkStampa.HRef = "Stampa.aspx"
            ' ''Dim myStream As Stream
            ' ''Dim ms As New MemoryStream
            ' ''Dim myOBJ() As Byte = Nothing
            ' ''Try
            ' ''    myStream = Rpt.ExportToStream(ExportFormatType.PortableDocFormat)

            ' ''    Dim Ret As Integer
            ' ''    Do
            ' ''        Ret = myStream.ReadByte() 'netstream.Read(Bytes, 0, Bytes.Length)
            ' ''        If Ret > 0 Then
            ' ''            ReDim Preserve myOBJ(myStream.Position - 1)
            ' ''            myOBJ(myStream.Position - 1) = Ret
            ' ''        End If
            ' ''    Loop Until Ret = -1

            ' ''Catch ex As Exception
            ' ''    Chiudi("Errore in elaborazione stampa: " & ex.Message)
            ' ''End Try
            ' ''Session("objReport") = myOBJ
        End If
    End Sub
    
    Protected Sub LinkStampa_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LinkStampa.Click
        'giu110320 giu180320 TROPPO LENTO
        ' ''Dim myOBJ() As Byte = Session("objReport")
        ' ''If IsNothing(myOBJ) Then
        ' ''    Chiudi("Attenzione, la stampa risulta vuota")
        ' ''    Exit Sub
        ' ''End If
        '-
        Dim myOBJ() As Byte = Session(Session(CSTNOMEPDF))
        If IsNothing(myOBJ) Then
            Chiudi("Attenzione, la stampa risulta vuota")
            Exit Sub
        End If
        ContentType = "application/pdf" 'vnd.ms-excel" '
        Dim contentDisposition = "inline;filename=" & Session(CSTNOMEPDF).ToString.Trim ' AllegatoC2.Pdf"

        With HttpContext.Current.Response

            .Clear()

            .ClearHeaders()

            .ClearContent()

            .ContentType = ContentType

            .AddHeader("Content-Disposition", contentDisposition)

            'Using ms As New MemoryStream()

            'Stream.CopyTo(ms)

            .BinaryWrite(myOBJ.ToArray)
        End With

        ' in order to use the binary write, we must convert our
        '   generic stream into a memory stream
    End Sub

    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String = ""

        If Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzino Or _
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.DisponibilitaMagazzinoFornitori Then
            strRitorno = "WF_DispMag.aspx?labelForm=Disponibilità di magazzino"
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFO Or _
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSintetico Or _
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoFor Or _
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOSinteticoForS Or _
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFOAnalitico Then
            strRitorno = "WF_ValorizzazioneMagazzino.aspx?labelForm=Valorizzazione di magazzino (FIFO)"
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFO Or _
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOSintetico Or _
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFOAnalitico Then
            strRitorno = "WF_ValorizzazioneMagazzino.aspx?labelForm=Valorizzazione di magazzino (LIFO)"
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPond Or _
           Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondSintetico Or _
           Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPondAnalitico Then
            strRitorno = "WF_ValorizzazioneMagazzino.aspx?labelForm=Valorizzazione di magazzino (Media ponderata)"
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcq Or _
           Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqSintetico Or _
           Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcqAnalitico Then
            strRitorno = "WF_ValorizzazioneMagazzino.aspx?labelForm=Valorizzazione di magazzino (Ultimo prezzo di acquisto)"
        ElseIf Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFO Or _
            Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagCostoVendFIFOSint Then
            strRitorno = "WF_ValMagCostoVenduto.aspx?labelForm=Costo del venduto (FIFO)"
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