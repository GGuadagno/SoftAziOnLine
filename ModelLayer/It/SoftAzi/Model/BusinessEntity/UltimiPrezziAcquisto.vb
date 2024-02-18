Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class UltimiPrezziAcquistoEntity
        Dim mCodArticolo As String
        Dim mDataAcquisto As Date
        Dim mPrezzo As Decimal
        Property CodArticolo() As String
            Get
                Return mCodArticolo
            End Get
            Set(ByVal value As String)
                mCodArticolo = value
            End Set
        End Property
        Property DataAcquisto() As Date
            Get
                Return mDataAcquisto
            End Get
            Set(ByVal value As Date)
                mDataAcquisto = value
            End Set
        End Property
        Property Prezzo() As Decimal
            Get
                Return mPrezzo
            End Get
            Set(ByVal value As Decimal)
                mPrezzo = value
            End Set
        End Property
    End Class
End Namespace
