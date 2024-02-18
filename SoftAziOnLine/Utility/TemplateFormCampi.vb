Public Class TemplateFormCampi
    Private _Campo As String
    Private _Valore As String
    Private _Abilitato As Boolean
    Private _Visibile As Boolean
    Property Campo() As String
        Get
            Return _Campo
        End Get
        Set(ByVal value As String)
            _Campo = value
        End Set
    End Property
    Property Valore() As String
        Get
            Return _Valore
        End Get
        Set(ByVal value As String)
            _Valore = value
        End Set
    End Property
    Property Abilitato() As Boolean
        Get
            Return _Abilitato
        End Get
        Set(ByVal value As Boolean)
            _Abilitato = value
        End Set
    End Property
    Property Visibile() As Boolean
        Get
            Return _Visibile
        End Get
        Set(ByVal value As Boolean)
            _Visibile = value
        End Set
    End Property

    Sub New(ByVal pCampo As String, ByVal pValore As String, ByVal pAbilitato As Boolean, ByVal pVisibile As Boolean)
        _Campo = pCampo
        _Valore = pValore
        _Abilitato = pAbilitato
        _Visibile = pVisibile
    End Sub
End Class
