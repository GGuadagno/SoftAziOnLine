Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class AliquoteIvaEntity

        Private _Aliquota As Integer

        Private _Descrizione As String

        Private _ProRata As Integer

        Private _Escludi_ProRata As Integer

        Private _TipOp As System.Nullable(Of Integer)

        Private _AliqIVA As System.Nullable(Of Integer)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property Aliquota() As Integer
            Get
                Return Me._Aliquota
            End Get
            Set(ByVal value As Integer)
                If ((Me._Aliquota = value) = False) Then
                    Me._Aliquota = value
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

        Public Property ProRata() As Integer
            Get
                Return Me._ProRata
            End Get
            Set(ByVal value As Integer)
                If ((Me._ProRata = value) _
                   = False) Then
                    Me._ProRata = value
                End If
            End Set
        End Property

        Public Property Escludi_ProRata() As Integer
            Get
                Return Me._Escludi_ProRata
            End Get
            Set(ByVal value As Integer)
                If ((Me._Escludi_ProRata = value) _
                   = False) Then
                    Me._Escludi_ProRata = value
                End If
            End Set
        End Property

        Public Property TipOp() As System.Nullable(Of Integer)
            Get
                Return Me._TipOp
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._TipOp.Equals(value) = False) Then
                    Me._TipOp = value
                End If
            End Set
        End Property

        Public Property AliqIVA() As System.Nullable(Of Integer)
            Get
                Return Me._AliqIVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._AliqIVA.Equals(value) = False) Then
                    Me._AliqIVA = value
                End If
            End Set
        End Property
    End Class
End Namespace
