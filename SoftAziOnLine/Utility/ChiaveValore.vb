Public Class ChiaveValore

    Private _Chiave As String
    Private _Valore As Object
    Property Chiave() As String
        Get
            Return _Chiave
        End Get
        Set(ByVal value As String)
            _Chiave = value
        End Set
    End Property
    Property Valore() As Object
        Get
            Return _Valore
        End Get
        Set(ByVal value As Object)
            _Valore = value
        End Set
    End Property

    Sub New(ByVal paramChiave As String, ByVal paramValore As Object)
        _Chiave = paramChiave
        _Valore = paramValore
    End Sub

End Class
