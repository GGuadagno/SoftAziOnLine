Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class FornSecondariEntity
        Dim mCodArticolo As String
        Dim mCodFornitore As String
        Dim mRagSoc As String
        Dim mTitolare As String
        Dim mRiferimento As String
        Dim mCodPagamento As Integer
        Dim mGiorniConsegna As Integer
        Dim mUltPrezzo As Decimal
        Property CodArticolo() As String
            Get
                Return mCodArticolo
            End Get
            Set(ByVal value As String)
                mCodArticolo = value
            End Set
        End Property
        Property CodFornitore() As String
            Get
                Return mCodFornitore
            End Get
            Set(ByVal value As String)
                mCodFornitore = value
            End Set
        End Property
        Property CodPagamento() As Integer
            Get
                Return mCodPagamento
            End Get
            Set(ByVal value As Integer)
                mCodPagamento = value
            End Set
        End Property
        Property GiorniConsegna() As Integer
            Get
                Return mGiorniConsegna
            End Get
            Set(ByVal value As Integer)
                mGiorniConsegna = value
            End Set
        End Property
        Property UltPrezzo() As Decimal
            Get
                Return mUltPrezzo
            End Get
            Set(ByVal value As Decimal)
                mUltPrezzo = value
            End Set
        End Property
        Property RagSoc() As String
            Get
                Return mRagSoc
            End Get
            Set(ByVal value As String)
                mRagSoc = value
            End Set
        End Property
        Property Titolare() As String
            Get
                Return mTitolare
            End Get
            Set(ByVal value As String)
                mTitolare = value
            End Set
        End Property
        Property Riferimento() As String
            Get
                Return mRiferimento
            End Get
            Set(ByVal value As String)
                mRiferimento = value
            End Set
        End Property
    End Class
End Namespace
