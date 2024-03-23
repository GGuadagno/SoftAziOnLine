Imports SoftAziOnLine.Def
Public Class WebFormStampe
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        VisualizzaRpt(Session("WebFormStampe"), Session(CSTNOMEPDF))
    End Sub
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

                    Response.AddHeader("Expires", "0")
                    Response.AppendHeader("content-disposition", "inline; filename=" & "" & _NomeRpt)
                    If Right(_NomeRpt, 4).ToString.ToUpper = ".PDF" Then
                        Response.ContentType = "application/pdf"
                    Else
                        Response.ContentType = "application/vnd.ms-excel"
                    End If
                    '-
                    Response.AddHeader("Accept-Ranges", "bytes")

                    Response.BinaryWrite(byteReport)
                    Response.Flush()
                    Response.End()
                End With
            Else
                'lnkElencoSc.Visible = False
                'LnkStampa.Visible = False
                'LnkVerbale.Visible = False
            End If
        Catch ex As Exception
            'lnkElencoSc.Visible = False
            'LnkStampa.Visible = False
            'LnkVerbale.Visible = False
        End Try
    End Sub
End Class