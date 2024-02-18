Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class AnaMagCTVEntity
        Dim mCodArticolo As String
        Dim mProgressivo As Integer
        Dim mTipo As String
        Dim mValore As String
        Property CodArticolo() As String
            Get
                Return mCodArticolo
            End Get
            Set(ByVal value As String)
                mCodArticolo = value
            End Set
        End Property
        Property Progressivo() As Integer
            Get
                Return mProgressivo
            End Get
            Set(ByVal value As Integer)
                mProgressivo = value
            End Set
        End Property
        Property Tipo() As String
            Get
                Return mTipo
            End Get
            Set(ByVal value As String)
                mTipo = value
            End Set
        End Property
        Property Valore() As String
            Get
                Return mValore
            End Get
            Set(ByVal value As String)
                mValore = value
            End Set
        End Property
    End Class
End Namespace
