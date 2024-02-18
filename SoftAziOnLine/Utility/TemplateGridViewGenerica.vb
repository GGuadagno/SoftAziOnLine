Public Class TemplateGridViewGenerica

    Private _Codice As String
    Private _Descrizione As Object
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

    Sub New(ByVal paramCodice As String, ByVal paramDescrizione As Object)
        _Codice = paramCodice
        _Descrizione = paramDescrizione
    End Sub

End Class
