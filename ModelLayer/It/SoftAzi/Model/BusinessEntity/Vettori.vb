Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class VettoriEntity

        Private _Codice As Integer

        Private _Descrizione As String

        Private _Residenza As String

        Private _Localita As String

        Private _Provincia As String

        Private _Partita_IVA As String

        Private _Codice_CoGe As String

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

        Public Property Residenza() As String
            Get
                Return Me._Residenza
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Residenza, value) = False) Then
                    Me._Residenza = value
                End If
            End Set
        End Property

        Public Property Localita() As String
            Get
                Return Me._Localita
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Localita, value) = False) Then
                    Me._Localita = value
                End If
            End Set
        End Property

        Public Property Provincia() As String
            Get
                Return Me._Provincia
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Provincia, value) = False) Then
                    Me._Provincia = value
                End If
            End Set
        End Property

        Public Property Partita_IVA() As String
            Get
                Return Me._Partita_IVA
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Partita_IVA, value) = False) Then
                    Me._Partita_IVA = value
                End If
            End Set
        End Property

        Public Property Codice_CoGe() As String
            Get
                Return Me._Codice_CoGe
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Codice_CoGe, value) = False) Then
                    Me._Codice_CoGe = value
                End If
            End Set
        End Property
    End Class
End Namespace
