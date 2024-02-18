Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class CategorieEntity

        Private _Codice As Integer
        Private _Descrizione As String
        Private _InvioMailSc As Boolean
        Private _SelSc As Boolean

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property Codice() As Integer
            Get
                Return Me._Codice
            End Get
            Set(ByVal value As Integer)
                If ((Me._Codice = value) _
                   = False) Then
                    Me._Codice = value
                End If
            End Set
        End Property

        Public Property Descrizione() As String
            Get
                Return Me._Descrizione
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Descrizione, value) = False) Then
                    Me._Descrizione = value
                End If
            End Set
        End Property

        Public Property InvioMailSc() As Boolean
            Get
                Return Me._InvioMailSc
            End Get
            Set(ByVal value As Boolean)
                Me._InvioMailSc = value
            End Set
        End Property
        Public Property SelSc() As Boolean
            Get
                Return Me._SelSc
            End Get
            Set(ByVal value As Boolean)
                Me._SelSc = value
            End Set
        End Property
    End Class
End Namespace
