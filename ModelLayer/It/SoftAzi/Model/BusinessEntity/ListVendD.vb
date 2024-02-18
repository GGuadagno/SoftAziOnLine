Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class ListVenDEntity
        Dim mCodice As Integer
        Dim mRiga As Integer
        Dim mCod_Articolo As String
        Dim mDescrizione As String 'Presa da AnaMag
        Dim mPrezzo As Decimal
        Dim mPrezzo_Valuta As Decimal
        Dim mSconto_1 As Decimal
        Dim mSconto_2 As Decimal
        Dim mPrezzoMinimo As Decimal
        Dim mData_Cambio As DateTime
        Dim mCambio As Decimal
        Property Codice() As Integer
            Get
                Return mCodice
            End Get
            Set(ByVal Value As Integer)
                mCodice = Value
            End Set
        End Property
        Property Riga() As Integer
            Get
                Return mRiga
            End Get
            Set(ByVal Value As Integer)
                mRiga = Value
            End Set
        End Property
        Property Cod_Articolo() As String
            Get
                Return mCod_Articolo
            End Get
            Set(ByVal Value As String)
                mCod_Articolo = Value
            End Set
        End Property
        Property Descrizione() As String
            Get
                Return mDescrizione
            End Get
            Set(ByVal Value As String)
                mDescrizione = Value
            End Set
        End Property

        Property Prezzo() As Decimal
            Get
                Return mPrezzo
            End Get
            Set(ByVal Value As Decimal)
                mPrezzo = Value
            End Set
        End Property
        Property Prezzo_Valuta() As Decimal
            Get
                Return mPrezzo_Valuta
            End Get
            Set(ByVal Value As Decimal)
                mPrezzo_Valuta = Value
            End Set
        End Property
        Property Sconto_1() As Decimal
            Get
                Return mSconto_1
            End Get
            Set(ByVal Value As Decimal)
                mSconto_1 = Value
            End Set
        End Property
        Property Sconto_2() As Decimal
            Get
                Return mSconto_2
            End Get
            Set(ByVal Value As Decimal)
                mSconto_2 = Value
            End Set
        End Property
        Property PrezzoMinimo() As Decimal
            Get
                Return mPrezzoMinimo
            End Get
            Set(ByVal Value As Decimal)
                mPrezzoMinimo = Value
            End Set
        End Property
        Property Data_Cambio() As DateTime
            Get
                Return mData_Cambio
            End Get
            Set(ByVal Value As DateTime)
                mData_Cambio = Value
            End Set
        End Property

        Property Cambio() As Decimal
            Get
                Return mCambio
            End Get
            Set(ByVal Value As Decimal)
                mCambio = Value
            End Set
        End Property
    End Class
End Namespace

