Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class OperatoriEntity
        Dim mCodice As String
        Dim mNome As String
        Dim mPassword As String
        Dim mLivello As String
        Dim mAnalitico As Integer
        Dim mRIBA As Integer
        Dim mContatore As Integer
        Dim mMod_CoGe As Integer
        Dim mMod_Azi As Integer
        Dim mMod_Fatture As Integer
        Dim mMod_Contratti As Integer
        Dim mMod_Hotel As Integer
        Dim mMod_Rist As Integer
        Dim mDataScadenza As DateTime
        Dim mNessunaScadenza As Boolean
        Dim mCodiceDitta As String
        Dim mDataOraUltimoAccesso As DateTime
        Dim mPwdSblocca As String

        Property Codice() As String
            Get
                Return mCodice
            End Get
            Set(ByVal Value As String)
                mCodice = Value
            End Set
        End Property

        Property Nome() As String
            Get
                Return mNome
            End Get
            Set(ByVal Value As String)
                mNome = Value
            End Set
        End Property

        Property Password() As String
            Get
                Return mPassword
            End Get
            Set(ByVal Value As String)
                mPassword = Value
            End Set
        End Property

        Property Livello() As String
            Get
                Return mLivello
            End Get
            Set(ByVal Value As String)
                mLivello = Value
            End Set
        End Property

        Property Analitico() As Integer
            Get
                Return mAnalitico
            End Get
            Set(ByVal Value As Integer)
                mAnalitico = Value
            End Set
        End Property

        Property RIBA() As Integer
            Get
                Return mRIBA
            End Get
            Set(ByVal Value As Integer)
                mRIBA = Value
            End Set
        End Property

        Property Contatore() As Integer
            Get
                Return mContatore
            End Get
            Set(ByVal Value As Integer)
                mContatore = Value
            End Set
        End Property

        Property Mod_Coge() As Integer
            Get
                Return mMod_CoGe
            End Get
            Set(ByVal Value As Integer)
                mMod_CoGe = Value
            End Set
        End Property

        Property Mod_Azi() As Integer
            Get
                Return mMod_Azi
            End Get
            Set(ByVal Value As Integer)
                mMod_Azi = Value
            End Set
        End Property

        Property Mod_Fatture() As Integer
            Get
                Return mMod_Fatture
            End Get
            Set(ByVal Value As Integer)
                mMod_Fatture = Value
            End Set
        End Property

        Property Mod_Contratti() As Integer
            Get
                Return mMod_Contratti
            End Get
            Set(ByVal Value As Integer)
                mMod_Contratti = Value
            End Set
        End Property

        Property Mod_Hotel() As Integer
            Get
                Return mMod_Hotel
            End Get
            Set(ByVal Value As Integer)
                mMod_Hotel = Value
            End Set
        End Property

        Property Mod_Rist() As Integer
            Get
                Return mMod_Rist
            End Get
            Set(ByVal Value As Integer)
                mMod_Rist = Value
            End Set
        End Property

        Property DataScadenza() As DateTime
            Get
                Return mDataScadenza
            End Get
            Set(ByVal Value As DateTime)
                mDataScadenza = Value
            End Set
        End Property

        Property NessunaScadenza() As Boolean
            Get
                Return mNessunaScadenza
            End Get
            Set(ByVal Value As Boolean)
                mNessunaScadenza = Value
            End Set
        End Property

        Property CodiceDitta() As String
            Get
                Return mCodiceDitta
            End Get
            Set(ByVal Value As String)
                mCodiceDitta = Value
            End Set
        End Property
        Property DataOraUltimoAccesso() As DateTime
            Get
                Return mDataOraUltimoAccesso
            End Get
            Set(ByVal Value As DateTime)
                mDataOraUltimoAccesso = Value
            End Set
        End Property
        Property PwdSblocca() As String
            Get
                Return mPwdSblocca
            End Get
            Set(ByVal Value As String)
                mPwdSblocca = Value
            End Set
        End Property
    End Class

    Public Class OperatoreConnessoEntity
        Dim mNomeOperatore As String
        Dim mCodiceDitta As String
        Dim mPostazione As String
        Dim mModulo As String
        Dim mSessionID As String
        Dim mID As Integer
        Dim mCodice As String
        Dim mAzienda As String
        Dim mTipo As String
        Dim mEsercizio As String

        Property NomeOperatore() As String
            Get
                Return mNomeOperatore
            End Get
            Set(ByVal Value As String)
                mNomeOperatore = Value
            End Set
        End Property
        Property CodiceDitta() As String
            Get
                Return mCodiceDitta
            End Get
            Set(ByVal Value As String)
                mCodiceDitta = Value
            End Set
        End Property
        Property Postazione() As String
            Get
                Return mPostazione
            End Get
            Set(ByVal Value As String)
                mPostazione = Value
            End Set
        End Property
        Property Modulo() As String
            Get
                Return mModulo
            End Get
            Set(ByVal Value As String)
                mModulo = Value
            End Set
        End Property
        Property SessionID() As String
            Get
                Return mSessionID
            End Get
            Set(ByVal Value As String)
                mSessionID = Value
            End Set
        End Property
        Property ID() As Integer
            Get
                Return mID
            End Get
            Set(ByVal Value As Integer)
                mID = Value
            End Set
        End Property
        Property Codice() As String
            Get
                Return mCodice
            End Get
            Set(ByVal Value As String)
                mCodice = Value
            End Set
        End Property
        Property Azienda() As String
            Get
                Return mAzienda
            End Get
            Set(ByVal Value As String)
                mAzienda = Value
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
        Property Esercizio() As String
            Get
                Return mEsercizio
            End Get
            Set(ByVal Value As String)
                mEsercizio = Value
            End Set
        End Property
    End Class
End Namespace