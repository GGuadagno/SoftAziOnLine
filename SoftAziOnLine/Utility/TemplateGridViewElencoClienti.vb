Public Class TemplateGridViewElencoClienti

    Private _Codice_CoGe As String
    Private _Rag_Soc As String
    Private _Denominazione As String
    Private _Partita_Iva As String
    Private _Codice_Fiscale As String
    Private _Cap As String
    Private _Localita As String
    Private _Indirizzo As String
    Private _Email As String
    Private _EmailInvioScad As String
    Private _EmailInvioFatt As String
    Private _PECEmail As String
    Private _IPA As String
    Property Codice_CoGe() As String
        Get
            Return _Codice_CoGe
        End Get
        Set(ByVal value As String)
            _Codice_CoGe = value
        End Set
    End Property
    Property Rag_Soc() As String
        Get
            Return _Rag_Soc
        End Get
        Set(ByVal value As String)
            _Rag_Soc = value
        End Set
    End Property
    Property Denominazione() As String
        Get
            Return _Denominazione
        End Get
        Set(ByVal value As String)
            _Denominazione = value
        End Set
    End Property
    Property Partita_Iva() As String
        Get
            Return _Partita_Iva
        End Get
        Set(ByVal value As String)
            _Partita_Iva = value
        End Set
    End Property
    Property Codice_Fiscale() As String
        Get
            Return _Codice_Fiscale
        End Get
        Set(ByVal value As String)
            _Codice_Fiscale = value
        End Set
    End Property
    Property Cap() As String
        Get
            Return _Cap
        End Get
        Set(ByVal value As String)
            _Cap = value
        End Set
    End Property
    Property Localita() As String
        Get
            Return _Localita
        End Get
        Set(ByVal value As String)
            _Localita = value
        End Set
    End Property
    Property Indirizzo() As String
        Get
            Return _Indirizzo
        End Get
        Set(ByVal value As String)
            _Indirizzo = value
        End Set
    End Property
    Property Email() As String
        Get
            Return _Email
        End Get
        Set(ByVal value As String)
            _Email = value
        End Set
    End Property
    Property EmailInvioScad() As String
        Get
            Return _EmailInvioScad
        End Get
        Set(ByVal value As String)
            _EmailInvioScad = value
        End Set
    End Property
    Property EmailInvioFatt() As String
        Get
            Return _EmailInvioFatt
        End Get
        Set(ByVal value As String)
            _EmailInvioFatt = value
        End Set
    End Property
    Property PECEmail() As String
        Get
            Return _PECEmail
        End Get
        Set(ByVal value As String)
            _PECEmail = value
        End Set
    End Property
    Property IPA() As String
        Get
            Return _IPA
        End Get
        Set(ByVal value As String)
            _IPA = value
        End Set
    End Property
    Sub New(ByVal paramCodice_Coge As String, ByVal paramRag_Soc As String, ByVal paramDenominazione As String, _
            ByVal paramPartita_Iva As String, ByVal paramCodice_Fiscale As String, _
            ByVal paramCap As String, ByVal paramLocalita As String, _
            ByVal paramIndirizzo As String, ByVal paramEmail As String, ByVal paramEmailInvioScad As String, _
            ByVal paramEmailInvioFatt As String, ByVal paramPECEmail As String, ByVal paramIPA As String)

        _Codice_CoGe = paramCodice_Coge
        _Rag_Soc = paramRag_Soc
        _Denominazione = paramDenominazione
        _Partita_Iva = paramPartita_Iva
        _Codice_Fiscale = paramCodice_Fiscale
        _Cap = paramCap
        _Localita = paramLocalita
        _Indirizzo = paramIndirizzo
        _Email = paramEmail
        _EmailInvioScad = paramEmailInvioScad
        _EmailInvioFatt = paramEmailInvioFatt
        _PECEmail = paramPECEmail
        _IPA = paramIPA
    End Sub

End Class
