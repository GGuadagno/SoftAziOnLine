Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WF_ValorizzazioneMagazzino
    Inherits System.Web.UI.Page
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTVISUALMENU) = SWSI

        Dim labelForm As String = Request.QueryString("labelForm")
        If IsNothing(labelForm) Then
            labelForm = ""
        End If
        If String.IsNullOrEmpty(labelForm) Then
            labelForm = ""
        End If
        If (Not IsPostBack) Then
            If Mid(UCase(labelForm.Trim), 1, 28) = "VALORIZZAZIONE DI MAGAZZINO " Then

                If Mid(UCase(labelForm.Trim), 29) = "(FIFO)" Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagFIFO
                End If
                If Mid(UCase(labelForm.Trim), 29) = UCase("(Ultimo prezzo di acquisto)") Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagUltPrzAcq
                End If
                If Mid(UCase(labelForm.Trim), 29) = UCase("(LIFO)") Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagLIFO
                End If
                If Mid(UCase(labelForm.Trim), 29) = UCase("(Media ponderata)") Then
                    Session(CSTTIPORPTDISPMAG) = TIPOSTAMPADISPMAGAZZINO.ValMagMediaPond
                End If

            Else
                'per altre chiamate
            End If
        End If
    End Sub

End Class