Imports SoftAziOnLine.Def

Partial Public Class WFP_ElencoDestCF
    Inherits System.Web.UI.UserControl

#Region "Property"

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property
    Private _Elenco As String
    Property Elenco() As String
        Get
            Return _Elenco
        End Get
        Set(ByVal value As String)
            _Elenco = value
        End Set
    End Property
    Private _Titolo As String
    Property Titolo() As String
        Get
            Return _Titolo
        End Get
        Set(ByVal value As String)
            _Titolo = value
        End Set
    End Property

#End Region

#Region "Metodi private"
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            lblMessUtente.Text = ""
        End If
        WUCElencoDestCF.WfpElement = Me
    End Sub

    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        If Not Session(RTN_VALUE_F_ELENCO) Is Nothing Then
            ProgrammaticModalPopup.Hide()
            Dim rtn As STR_RETURN_VALUE = Session(RTN_VALUE_F_ELENCO)
            _WucElement.CallBackWFPElencoDestCF(rtn.Codice, rtn.Descrizione)
            Session(RTN_VALUE_F_ELENCO) = Nothing
            lblMessUtente.Text = ""
            Session(F_ELENCO_DESTCF_APERTA) = False
            Session(F_ELENCO_DESTCFD_APERTA) = False
        Else
            lblMessUtente.Text = "Attenzione, nessuna anagrafica è stata selezionata."
        End If

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_ELENCO_DESTCF_APERTA) = False
        Session(F_ELENCO_DESTCFD_APERTA) = False
    End Sub

#End Region

#Region "Metodi public"

    Public Sub Show(Optional ByVal isFirst As Boolean = False)
        If isFirst Then
            WUCElencoDestCF.Reset(_Elenco, _Titolo)
            Session(RTN_VALUE_F_ELENCO) = String.Empty
            Session(RTN_VALUE_F_ELENCO) = Nothing
            lblMessUtente.Text = ""
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

#End Region

End Class