Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class BancheEntity
        Private _ABI As String

        Private _Banca As String

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property ABI() As String
            Get
                Return Me._ABI
            End Get
            Set(ByVal value As String)
                If ((Me._ABI = value) _
                   = False) Then
                    Me._ABI = value
                End If
            End Set
        End Property

        Public Property Banca() As String
            Get
                Return Me._Banca
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Banca, value) = False) Then
                    Me._Banca = value
                End If
            End Set
        End Property

    End Class
End Namespace
