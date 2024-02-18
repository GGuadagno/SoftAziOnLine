Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class TipoFattEntity
        Dim mCodice As String
        Dim mDescrizione As String
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
    End Class
End Namespace
