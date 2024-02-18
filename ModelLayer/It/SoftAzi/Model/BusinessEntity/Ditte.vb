Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class DitteEntity
        Dim mCodice As String
        Dim mDescrizione As String
        Dim mPartitaIVA As String
        Dim mTelefono As String
        Dim mFax As String
        Dim mIndirizzo As String
        Dim mCitta As String
        Dim mCAP As String
        Dim mProvincia As String
        Dim mBlocca_Accesso As Integer
        Dim mGetNomePC As String
        Dim mBlocco_Dalle As Date
        Property Codice() As String
            Get
                Return mCodice
            End Get
            Set(ByVal Value As String)
                mCodice = Value
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
        Property PartitaIVA() As String
            Get
                Return mPartitaIVA
            End Get
            Set(ByVal Value As String)
                mPartitaIVA = Value
            End Set
        End Property
        Property Telefono() As String
            Get
                Return mTelefono
            End Get
            Set(ByVal Value As String)
                mTelefono = Value
            End Set
        End Property
        Property FAX() As String
            Get
                Return mFax
            End Get
            Set(ByVal Value As String)
                mFax = Value
            End Set
        End Property
        Property Indirizzo() As String
            Get
                Return mIndirizzo
            End Get
            Set(ByVal Value As String)
                mIndirizzo = Value
            End Set
        End Property
        Property Citta() As String
            Get
                Return mCitta
            End Get
            Set(ByVal Value As String)
                mCitta = Value
            End Set
        End Property
        Property CAP() As String
            Get
                Return mCAP
            End Get
            Set(ByVal Value As String)
                mCAP = Value
            End Set
        End Property
        Property Provincia() As String
            Get
                Return mProvincia
            End Get
            Set(ByVal Value As String)
                mProvincia = Value
            End Set
        End Property
        Property Blocca_Accesso() As Integer
            Get
                Return mBlocca_Accesso
            End Get
            Set(ByVal Value As Integer)
                mBlocca_Accesso = Value
            End Set
        End Property
        Property GetNomePC() As String
            Get
                Return mGetNomePC
            End Get
            Set(ByVal Value As String)
                mGetNomePC = Value
            End Set
        End Property
        Property Blocco_Dalle() As Date
            Get
                Return mBlocco_Dalle
            End Get
            Set(ByVal Value As Date)
                mBlocco_Dalle = Value
            End Set
        End Property
       
    End Class
End Namespace