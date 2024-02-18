Imports CrystalDecisions.CrystalReports
Imports SoftAziOnLine.Def
Imports It.SoftAzi.Model.Entity 'giu150312
Imports It.SoftAzi.Model.Facade 'Ho tutte le funzioni es. get_Operatori
Imports It.SoftAzi.Model.Entity.OperatoriEntity
Imports It.SoftAzi.SystemFramework

Partial Public Class WF_PrintWebEtichette
    Inherits System.Web.UI.Page

    Private CodiceDitta As String = "" 'giu310112
    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    'giu030512 
    ' ''Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    ' ''    Try
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    Catch ex As Exception
    ' ''        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    ' ''    End Try
    ' ''End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'giu310112 gestione su piu aziende inserito controllo del codice ditta
        If CKCSTTipoDoc() = False Then
            Try
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("..\Login.aspx?SessioneScaduta=1")
                Exit Sub
            End Try
            Exit Sub
        End If

        Dim Rpt As Object = Nothing

        Rpt = New EtichetteCollo
        If CodiceDitta = "01" Then
            Rpt = New EtichetteCollo01
        ElseIf CodiceDitta = "05" Then
            Rpt = New EtichetteCollo05
        ElseIf CodiceDitta = "0501" Then
            Rpt = New EtichetteCollo0501
        ElseIf CodiceDitta = "0502" Then
            Rpt = New EtichetteCollo0502
        End If
        Dim DsEtichette1 As New DSPrintWeb_Documenti
        DsEtichette1 = Session(CSTDsPrepEtichette)
        CrystalReportViewer1.ToolbarImagesFolderUrl = "~\Immagini\CR\"
        Rpt.SetDataSource(DsEtichette1)
        CrystalReportViewer1.ReportSource = Rpt

    End Sub


    Private Sub btnRitorno_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRitorno.Click
        Dim strRitorno As String

        strRitorno = Session(CSTDsRitornoEtichette) '"WF_EtichettePrepara.aspx?labelForm=Prepara etichette"
        If IsNothing(strRitorno) Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        End If
        If String.IsNullOrEmpty(strRitorno) Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        End If
        'giu150312
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            Try
                Response.Redirect("WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni")
                Exit Sub
            Catch ex As Exception
                Response.Redirect("WF_ElencoDistinteSped.aspx?labelForm=Gestione spedizioni")
                Exit Sub
            End Try
            Exit Sub
        End If
        '-----------
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

    Public Function CKCSTTipoDoc(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDoc = True
        TipoDoc = Session(CSTTIPODOC)
        If IsNothing(TipoDoc) Then
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            Return False
        End If
        If TipoDoc = "" Then
            Return False
        End If
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
        'giu270412 per testare i vari moduli di stampa personalizzati
        Dim sTipoUtente As String = ""
        Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
        If (Utente Is Nothing) Then
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
            Exit Function
        End If
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        'GIU040213 SE VIENE VAORIZZATO IL CODICE DITTA ESEGUE LA STAMPA SU QUEL CODICE 
        'SE NON ESISTE IL REPORT PERSONALIZZATO CON CODICE DITTA METTE QUELLO DI DEMO SENZA CODICE DITTA
        Try
            Dim OpSys As New Operatori
            Dim myOp As OperatoriEntity
            Dim arrOperatori As ArrayList = Nothing
            arrOperatori = OpSys.getOperatoriByName(Utente.NomeOperatore)
            If Not IsNothing(arrOperatori) Then
                If arrOperatori.Count > 0 Then
                    myOp = CType(arrOperatori(0), OperatoriEntity)
                    If myOp.CodiceDitta.Trim <> "" Then
                        CodiceDitta = myOp.CodiceDitta.Trim
                        Return True
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
        '------------------------------------------------------------
        'giu310112 codice ditta per la gestione delle stampe personalizzate
        CodiceDitta = Session(CSTCODDITTA)
        If IsNothing(CodiceDitta) Then
            Return False
        End If
        If String.IsNullOrEmpty(CodiceDitta) Then
            Return False
        End If
        If CodiceDitta = "" Then
            Return False
        End If
        '-------------------------------------------------------------------
    End Function
End Class