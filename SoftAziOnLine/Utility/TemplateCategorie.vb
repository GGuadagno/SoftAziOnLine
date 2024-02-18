Public Class TemplateCategorie
    Private _Codice As String
    Private _Descrizione As Object
    Private _InvioMailSc As Boolean
    Private _SelSc As Boolean

    Property Codice() As String
        Get
            Return _Codice
        End Get
        Set(ByVal value As String)
            _Codice = value
        End Set
    End Property
    Property Descrizione() As Object
        Get
            Return _Descrizione
        End Get
        Set(ByVal value As Object)
            _Descrizione = value
        End Set
    End Property
    Property InvioMailSc() As Boolean
        Get
            Return Me._InvioMailSc
        End Get
        Set(ByVal value As Boolean)
            Me._InvioMailSc = value
        End Set
    End Property
    Property SelSc() As Boolean
        Get
            Return Me._SelSc
        End Get
        Set(ByVal value As Boolean)
            Me._SelSc = value
        End Set
    End Property

    Sub New(ByVal paramCodice As String, ByVal paramDescrizione As Object, ByVal parInvioMailSc As Boolean, ByVal parSelSc As Boolean)
        _Codice = paramCodice
        _Descrizione = paramDescrizione
        _InvioMailSc = parInvioMailSc
        _SelSc = parSelSc
    End Sub
End Class
