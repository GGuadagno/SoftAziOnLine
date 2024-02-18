Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    'TUTTI DATI STRINGA ALTRIMENTI E' UN CASINO QUANDO ALCUNI CAMPI NON SONO VALORIZZATI COME LA DATA
    Public Class ScadPagCAEntity
        Dim mNRata As String
        Dim mData As String
        Dim mImporto As String
        Dim mEvasa As Boolean
        Dim mNFC As String
        Dim mDataFC As String
        Dim mSerie As String
        Dim mImportoF As String
        Dim mImportoR As String
        Property NRata() As String
            Get
                Return mNRata
            End Get
            Set(ByVal value As String)
                mNRata = value
            End Set
        End Property
        Property Data() As String
            Get
                Return mData
            End Get
            Set(ByVal value As String)
                mData = value
            End Set
        End Property
        Property Importo() As String
            Get
                Return mImporto
            End Get
            Set(ByVal value As String)
                mImporto = value
            End Set
        End Property
        Property Evasa() As Boolean
            Get
                Return mEvasa
            End Get
            Set(ByVal value As Boolean)
                mEvasa = value
            End Set
        End Property
        Property NFC() As String
            Get
                Return mNFC
            End Get
            Set(ByVal value As String)
                mNFC = value
            End Set
        End Property
        Property DataFC() As String
            Get
                Return mDataFC
            End Get
            Set(ByVal value As String)
                mDataFC = value
            End Set
        End Property
        Property Serie() As String
            Get
                Return mSerie
            End Get
            Set(ByVal value As String)
                mSerie = value
            End Set
        End Property
        Property ImportoF() As String
            Get
                Return mImportoF
            End Get
            Set(ByVal value As String)
                mImportoF = value
            End Set
        End Property
        Property ImportoR() As String
            Get
                Return mImportoR
            End Get
            Set(ByVal value As String)
                mImportoR = value
            End Set
        End Property
    End Class
    '-
    Public Class ScadAttCAEntity
        Dim mRiga As String
        Dim mSerie As String
        Dim mCodArt As String
        Dim mEvasa As Boolean
        Dim mDataSc As String
        Property Riga() As String
            Get
                Return mRiga
            End Get
            Set(ByVal value As String)
                mRiga = value
            End Set
        End Property
        Property Serie() As String
            Get
                Return mSerie
            End Get
            Set(ByVal value As String)
                mSerie = value
            End Set
        End Property
        Property CodArt() As String
            Get
                Return mCodArt
            End Get
            Set(ByVal value As String)
                mCodArt = value
            End Set
        End Property
        Property Evasa() As Boolean
            Get
                Return mEvasa
            End Get
            Set(ByVal value As Boolean)
                mEvasa = value
            End Set
        End Property
        Property DataSc() As String
            Get
                Return mDataSc
            End Get
            Set(ByVal value As String)
                mDataSc = value
            End Set
        End Property
    End Class

    Public Class ScadPagEntity
        Dim mNRata As String
        Dim mData As String
        Dim mImporto As String
        Property NRata() As String
            Get
                Return mNRata
            End Get
            Set(ByVal value As String)
                mNRata = value
            End Set
        End Property
        Property Data() As String
            Get
                Return mData
            End Get
            Set(ByVal value As String)
                mData = value
            End Set
        End Property
        Property Importo() As String
            Get
                Return mImporto
            End Get
            Set(ByVal value As String)
                mImporto = value
            End Set
        End Property
    End Class
End Namespace
