Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class NazioniEntity

        Private _Codice As String

        Private _Descrizione As String

        Private _Codice_ISO As String

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property Codice() As String
            Get
                Return Me._Codice
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Codice, value) = False) Then
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

        Public Property Codice_ISO() As String
            Get
                Return Me._Codice_ISO
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Codice_ISO, value) = False) Then
                    Me._Codice_ISO = value
                End If
            End Set
        End Property
    End Class
End Namespace
