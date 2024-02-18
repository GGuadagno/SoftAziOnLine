Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class AgentiEntity

        Private _Codice As Integer

        Private _Descrizione As String

        Private _Residenza As String

        Private _Localita As String

        Private _Provincia As String

        Private _Partita_IVA As String

        Private _Aliquota_IVA As System.Nullable(Of Integer)

        Private _Aliquota_RitAcc As System.Nullable(Of Double)

        Private _Aliquota_Enasarco As System.Nullable(Of Double)

        Private _Codice_CoGe As String

        Private _Estero As System.Nullable(Of Integer)

        Private _DataInizioCollaborazione As System.Nullable(Of Date)

        Private _DataFineCollaborazione As System.Nullable(Of Date)

        Private _CodCapogruppo As System.Nullable(Of Integer)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property Codice() As Integer
            Get
                Return Me._Codice
            End Get
            Set(ByVal value As Integer)
                If ((Me._Codice = value) = False) Then
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

        Public Property Aliquota_IVA() As System.Nullable(Of Integer)
            Get
                Return Me._Aliquota_IVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Aliquota_IVA.Equals(value) = False) Then
                    Me._Aliquota_IVA = value
                End If
            End Set
        End Property

        Public Property Aliquota_RitAcc() As System.Nullable(Of Double)
            Get
                Return Me._Aliquota_RitAcc
            End Get
            Set(ByVal value As System.Nullable(Of Double))
                If (Me._Aliquota_RitAcc.Equals(value) = False) Then
                    Me._Aliquota_RitAcc = value
                End If
            End Set
        End Property

        Public Property Aliquota_Enasarco() As System.Nullable(Of Double)
            Get
                Return Me._Aliquota_Enasarco
            End Get
            Set(ByVal value As System.Nullable(Of Double))
                If (Me._Aliquota_Enasarco.Equals(value) = False) Then
                    Me._Aliquota_Enasarco = value
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

        Public Property Estero() As System.Nullable(Of Integer)
            Get
                Return Me._Estero
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Estero.Equals(value) = False) Then
                    Me._Estero = value
                End If
            End Set
        End Property

        Public Property DataInizioCollaborazione() As System.Nullable(Of Date)
            Get
                Return Me._DataInizioCollaborazione
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._DataInizioCollaborazione.Equals(value) = False) Then
                    Me._DataInizioCollaborazione = value
                End If
            End Set
        End Property

        Public Property DataFineCollaborazione() As System.Nullable(Of Date)
            Get
                Return Me._DataFineCollaborazione
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._DataFineCollaborazione.Equals(value) = False) Then
                    Me._DataFineCollaborazione = value
                End If
            End Set
        End Property

        Public Property CodCapogruppo() As System.Nullable(Of Integer)
            Get
                Return Me._CodCapogruppo
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodCapogruppo.Equals(value) = False) Then
                    Me._CodCapogruppo = value
                End If
            End Set
        End Property
    End Class
End Namespace
