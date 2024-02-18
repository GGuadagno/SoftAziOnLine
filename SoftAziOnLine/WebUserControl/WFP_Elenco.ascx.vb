Imports SoftAziOnLine.Def

Partial Public Class WFP_Elenco
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
            Session(PANELCATEG) = False
            Dim rtn As STR_RETURN_VALUE = Session(RTN_VALUE_F_ELENCO)
            _WucElement.CallBackWFPElenco(rtn.Codice, rtn.Descrizione, Session(F_ELENCO_APERTA))
            lblMessUtente.Text = ""
            Session(RTN_VALUE_F_ELENCO) = Nothing
            Session(F_ELENCO_APERTA) = String.Empty
        ElseIf Not Session(PANELCATEG) Is Nothing Then
            If Session(PANELCATEG) = True Then
                Session(PANELCATEG) = False
                ProgrammaticModalPopup.Hide()
                _WucElement.CallBackWFPElenco(Nothing, Nothing, Nothing)
                lblMessUtente.Text = ""
                Session(RTN_VALUE_F_ELENCO) = Nothing
                Session(F_ELENCO_APERTA) = String.Empty
                Exit Sub
            End If
            lblMessUtente.Text = "Attenzione, nessun elemento è stato selezionato."
        Else
            lblMessUtente.Text = "Attenzione, nessun elemento è stato selezionato."
        End If

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        
        ProgrammaticModalPopup.Hide()
        Session(F_ELENCO_APERTA) = String.Empty

        If Not Session(PANELCATEG) Is Nothing Then
            If Session(PANELCATEG) = True Then
                Session(PANELCATEG) = False
                _WucElement.CallBackWFPElenco(Nothing, Nothing, Nothing)
                Exit Sub
            End If
        End If
    End Sub

#End Region

#Region "Metodi public"

    Public Sub Show(Optional ByVal isFirst As Boolean = False)
        If isFirst Then
            WUCElenco.Reset(_Titolo)
            Session(RTN_VALUE_F_ELENCO) = String.Empty
            Session(RTN_VALUE_F_ELENCO) = Nothing
            lblMessUtente.Text = ""
            Try
                If Session(PANELCATEG) Then
                    WUCElenco.SetPanelCategorie(True)
                Else
                    WUCElenco.SetPanelCategorie(False)
                End If
            Catch ex As Exception
                WUCElenco.SetPanelCategorie(False)
            End Try
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

#End Region

End Class