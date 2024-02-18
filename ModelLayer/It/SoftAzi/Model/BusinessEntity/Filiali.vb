Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class FilialiEntity
        Private _ABI As String
        Private _CAB As String
        Private _Filiale As String
        Private _Nazione As String
        Private _CAP As String
        Private _Provincia As String
        Private _Citta As String
        Private _Indirizzo As String

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
        Public Property CAB() As String
            Get
                Return Me._CAB
            End Get
            Set(ByVal value As String)
                If ((Me._CAB = value) _
                   = False) Then
                    Me._CAB = value
                End If
            End Set
        End Property
        Public Property Filiale() As String
            Get
                Return Me._Filiale
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Filiale, value) = False) Then
                    Me._Filiale = value
                End If
            End Set
        End Property
        Public Property Nazione() As String
            Get
                Return Me._Nazione
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Nazione, value) = False) Then
                    Me._Nazione = value
                End If
            End Set
        End Property
        Public Property CAP() As String
            Get
                Return Me._CAP
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CAP, value) = False) Then
                    Me._CAP = value
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
        Public Property Citta() As String
            Get
                Return Me._Citta
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Citta, value) = False) Then
                    Me._Citta = value
                End If
            End Set
        End Property
        Public Property Indirizzo() As String
            Get
                Return Me._Indirizzo
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Indirizzo, value) = False) Then
                    Me._Indirizzo = value
                End If
            End Set
        End Property
    End Class
End Namespace
