Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class DistBaseEntity
        Dim mCodPadre As String
        Dim mRiga As Integer
        Dim mCodFiglio As String
        Dim mDesFiglio As String
        Dim mUM As String
        Dim mQuantita As Decimal
        Property CodPadre() As String
            Get
                Return mCodPadre
            End Get
            Set(ByVal value As String)
                mCodPadre = value
            End Set
        End Property
        Property Riga() As Integer
            Get
                Return mRiga
            End Get
            Set(ByVal value As Integer)
                mRiga = value
            End Set
        End Property
        Property CodFiglio() As String
            Get
                Return mCodFiglio
            End Get
            Set(ByVal value As String)
                mCodFiglio = value
            End Set
        End Property
        Property DesFiglio() As String
            Get
                Return mDesFiglio
            End Get
            Set(ByVal value As String)
                mDesFiglio = value
            End Set
        End Property
        Property UM() As String
            Get
                Return mUM
            End Get
            Set(ByVal value As String)
                mUM = value
            End Set
        End Property
        Property Quantita() As Decimal
            Get
                Return mQuantita
            End Get
            Set(ByVal value As Decimal)
                mQuantita = value
            End Set
        End Property
    End Class
End Namespace
