Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_ModificaSchedaIN
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
        If (WUCModificaSchedaIN.PopolaLista()) Then
            Session(F_MODIFICASCHEDAIN_APERTA) = False
            lblMessUtente.Text = ""
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPModificaSchedaIN()
        Else
            btnChiudiScheda.Enabled = True
            lblMessUtente.Text = "Attenzione, nessuna differenza inventario presente."
        End If
    End Sub

    Protected Sub btnChiudiScheda_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If (WUCModificaSchedaIN.PopolaLista()) Then
            btnChiudiScheda.Enabled = False
            lblMessUtente.Text = "Attenzione, sono presenti differenze di inventario."
        Else
            Session(F_MODIFICASCHEDAIN_APERTA) = False
            lblMessUtente.Text = ""
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPChiudiSchedaIN()
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(F_MODIFICASCHEDAIN_APERTA) = False
        lblMessUtente.Text = ""
        ProgrammaticModalPopup.Hide()
        _WucElement.CallBackWFPEndModificaSchedaIN()
    End Sub

    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        If PrimaVolta Then
            lblMessUtente.Text = "Modifica Quantità inventario"
            btnChiudiScheda.Enabled = False
            WUCModificaSchedaIN.SelAndQtaTutti()
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

    ' ''Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    ' ''    If (Not IsPostBack) Then
    ' ''        lblMessUtente.Text = "Seleziona/modifica Quantità articoli da caricare"
    ' ''    End If
    ' ''End Sub
End Class