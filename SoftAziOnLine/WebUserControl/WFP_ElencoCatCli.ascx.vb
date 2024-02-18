Imports SoftAziOnLine.Def

Partial Public Class WFP_ElencoCatCli
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
    Private _Tabella As String = String.Empty
    Property Tabella() As String
        Get
            Return _Tabella
        End Get
        Set(ByVal value As String)
            _Tabella = value
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
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("~/WebFormTables/WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        WUCElenco.Tabella = _Tabella
        If (Not IsPostBack) Then
            lblMessUtente.Text = ""
        End If
        WUCElenco.WfpElement = Me
    End Sub

    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        If Not Session(RTN_VALUE_F_ELENCO) Is Nothing Then
            ProgrammaticModalPopup.Hide()
            Dim rtn As STR_RETURN_VALUE = Session(RTN_VALUE_F_ELENCO)
            _WucElement.CallBackWFPElenco(rtn.Codice, rtn.Descrizione, Session(F_ELENCO_APERTA))
            lblMessUtente.Text = ""
            Session(RTN_VALUE_F_ELENCO) = Nothing
            Session(F_ELENCO_APERTA) = String.Empty
        Else
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPElenco(Nothing, Nothing, Nothing)
            lblMessUtente.Text = ""
            Session(RTN_VALUE_F_ELENCO) = Nothing
            Session(F_ELENCO_APERTA) = String.Empty
            Exit Sub
        End If

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_ELENCO_APERTA) = String.Empty
        _WucElement.CallBackWFPElenco(Nothing, Nothing, Nothing)
    End Sub

#End Region

#Region "Metodi public"

    Public Sub Show(Optional ByVal isFirst As Boolean = False)
        If isFirst Then
            WUCElenco.Reset(_Titolo)
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