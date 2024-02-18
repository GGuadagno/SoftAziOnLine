Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class AnaMagDesEntity
        Dim mCodArticolo As String
        Dim mProgressivo As Integer
        Dim mDescrizione As String
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
        Property Descrizione() As String
            Get
                Return mDescrizione
            End Get
            Set(ByVal value As String)
                mDescrizione = value
            End Set
        End Property
    End Class
End Namespace
