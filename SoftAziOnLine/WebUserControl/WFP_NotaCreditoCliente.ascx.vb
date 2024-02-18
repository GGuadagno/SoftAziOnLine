Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WFP_NotaCreditoCliente
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
   
    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUCNotaCreditoCliente.PopolaLista()) Then
            Session(F_NCAAC_APERTA) = False
            lblMessUtente.Text = ""
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPNotaCreditoCliente()
        Else
            lblMessUtente.Text = "Attenzione, nessuna riga è stata selezionata."
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(F_NCAAC_APERTA) = False
        lblMessUtente.Text = ""
        ProgrammaticModalPopup.Hide()
        _WucElement.CancBackWFPNotaCreditoCliente()
    End Sub

    Protected Sub btnSelTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        WUCNotaCreditoCliente.SelezionaTutti()
    End Sub

    Protected Sub btnDeselTutti_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        WUCNotaCreditoCliente.DeselezionaTutti()
    End Sub

    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        If PrimaVolta Then
            lblMessUtente.Text = "Seleziona/modifica Quantità articoli resi"
            WUCNotaCreditoCliente.SelAndQtaTutti()
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub
    'giu120417
    Public Sub PopolaGrigliaWUCDocTD()
        WUCNotaCreditoCliente.PopolaDocTD()
    End Sub
End Class