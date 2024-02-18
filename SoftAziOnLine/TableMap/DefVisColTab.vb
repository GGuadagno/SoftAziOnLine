Public Class DefVisColTab

    Sub New( _
        ByVal sNomeTabella As String, _
        ByVal sRagioneSocialeAzienda As String, _
        ByVal sNomeColonna As String, _
        ByVal bVisibile As Boolean)

        _NomeTabella = sNomeTabella
        _NomeColonna = sNomeColonna
        _RagioneSocialeAzienda = sRagioneSocialeAzienda
        _Visibile = bVisibile
    End Sub

    Private _NomeTabella As String
    Property NomeTabella() As String
        Get
            Return _NomeTabella
        End Get
        Set(ByVal value As String)
            _NomeTabella = value
        End Set
    End Property

    Private _RagioneSocialeAzienda As String
    Property RagioneSocialeAzienda() As String
        Get
            Return _RagioneSocialeAzienda
        End Get
        Set(ByVal value As String)
            _RagioneSocialeAzienda = value
        End Set
    End Property

    Private _NomeColonna As String
    Property NomeColonna() As String
        Get
            Return _NomeColonna
        End Get
        Set(ByVal value As String)
            _NomeColonna = value
        End Set
    End Property

    Private _Visibile As Boolean
    Property Visibile() As Boolean
        Get
            Return _Visibile
        End Get
        Set(ByVal value As Boolean)
            _Visibile = value
        End Set
    End Property

End Class
