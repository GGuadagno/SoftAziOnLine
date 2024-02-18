Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class ListVenTEntity
        Dim mCodice As Integer
        Dim mDescrizione As String
        Dim mTipo As String
        Dim mData_Inizio_Validita As DateTime
        Dim mAbilitato As Boolean
        Dim mValuta As String
        Dim mCod_Pagamento As Integer
        Dim mCategoria As Integer
        Dim mCliente As String
        Dim mNote As String
        Property Codice() As Integer
            Get
                Return mCodice
            End Get
            Set(ByVal Value As Integer)
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

        Property Tipo() As String
            Get
                Return mTipo
            End Get
            Set(ByVal Value As String)
                mTipo = Value
            End Set
        End Property

        Property Data_Inizio_Validita() As DateTime
            Get
                Return mData_Inizio_Validita
            End Get
            Set(ByVal Value As DateTime)
                mData_Inizio_Validita = Value
            End Set
        End Property

        Property Abilitato() As Boolean
            Get
                Return mAbilitato
            End Get
            Set(ByVal Value As Boolean)
                mAbilitato = Value
            End Set
        End Property

        Property Valuta() As String
            Get
                Return mValuta
            End Get
            Set(ByVal Value As String)
                mValuta = Value
            End Set
        End Property

        Property Cod_Pagamento() As Integer
            Get
                Return mCod_Pagamento
            End Get
            Set(ByVal Value As Integer)
                mCod_Pagamento = Value
            End Set
        End Property

        Property Categoria() As Integer
            Get
                Return mCategoria
            End Get
            Set(ByVal Value As Integer)
                mCategoria = Value
            End Set
        End Property

        Property Cliente() As String
            Get
                Return mCliente
            End Get
            Set(ByVal Value As String)
                mCliente = Value
            End Set
        End Property

        Property Note() As String
            Get
                Return mNote
            End Get
            Set(ByVal Value As String)
                mNote = Value
            End Set
        End Property
    End Class
End Namespace
