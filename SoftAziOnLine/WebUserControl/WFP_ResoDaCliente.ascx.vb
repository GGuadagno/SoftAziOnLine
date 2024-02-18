Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_ResoDaCliente
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUCResoDaCliente.PopolaLista()) Then
            Session(F_RESODAC_APERTA) = False
            lblMessUtente.Text = ""
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPResoDaCliente()
        Else
            lblMessUtente.Text = "Attenzione, nessuna riga è stata selezionata."
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(F_RESODAC_APERTA) = False
        lblMessUtente.Text = ""
        ProgrammaticModalPopup.Hide()
        _WucElement.CancBackWFPResoDaCliente()
    End Sub

    Protected Sub btnSelTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        WUCResoDaCliente.SelezionaTutti()
    End Sub

    Protected Sub btnDeselTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        WUCResoDaCliente.DeselezionaTutti()
    End Sub

    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        If PrimaVolta Then
            lblMessUtente.Text = "Seleziona/modifica Quantità articoli resi"
            WUCResoDaCliente.SelAndQtaTutti()
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

End Class