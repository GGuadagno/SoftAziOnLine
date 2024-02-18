Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class PagamentiEntity

        Private _Codice As Integer

        Private _Descrizione As String

        Private _Tipo_Pagamento As System.Nullable(Of Integer)

        Private _Tipo_Scadenza As System.Nullable(Of Integer)

        Private _Numero_Rate As System.Nullable(Of Integer)

        Private _Mese As System.Nullable(Of Integer)

        Private _Scadenza_1 As System.Nullable(Of Integer)

        Private _Scadenza_2 As System.Nullable(Of Integer)

        Private _Scadenza_3 As System.Nullable(Of Integer)

        Private _Scadenza_4 As System.Nullable(Of Integer)

        Private _Scadenza_5 As System.Nullable(Of Integer)

        Private _Perc_Imponib_1 As System.Nullable(Of Single)

        Private _Perc_Imponib_2 As System.Nullable(Of Single)

        Private _Perc_Imponib_3 As System.Nullable(Of Single)

        Private _Perc_Imponib_4 As System.Nullable(Of Single)

        Private _Perc_Imponib_5 As System.Nullable(Of Single)

        Private _Perc_Imposta_1 As System.Nullable(Of Single)

        Private _Perc_Imposta_2 As System.Nullable(Of Single)

        Private _Perc_Imposta_3 As System.Nullable(Of Single)

        Private _Perc_Imposta_4 As System.Nullable(Of Single)

        Private _Perc_Imposta_5 As System.Nullable(Of Single)

        Private _Mese_Escluso_1 As System.Nullable(Of Integer)

        Private _Mese_Escluso_2 As System.Nullable(Of Integer)

        Private _Spese_Incasso As System.Nullable(Of Decimal)

        Private _IVA_Spese_Incasso As System.Nullable(Of Integer)

        Private _Sconto_Cassa As System.Nullable(Of Single)

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

        Public Property Tipo_Pagamento() As System.Nullable(Of Integer)
            Get
                Return Me._Tipo_Pagamento
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Tipo_Pagamento.Equals(value) = False) Then
                    Me._Tipo_Pagamento = value
                End If
            End Set
        End Property

        Public Property Tipo_Scadenza() As System.Nullable(Of Integer)
            Get
                Return Me._Tipo_Scadenza
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Tipo_Scadenza.Equals(value) = False) Then
                    Me._Tipo_Scadenza = value
                End If
            End Set
        End Property

        Public Property Numero_Rate() As System.Nullable(Of Integer)
            Get
                Return Me._Numero_Rate
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Numero_Rate.Equals(value) = False) Then
                    Me._Numero_Rate = value
                End If
            End Set
        End Property

        Public Property Mese() As System.Nullable(Of Integer)
            Get
                Return Me._Mese
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Mese.Equals(value) = False) Then
                    Me._Mese = value
                End If
            End Set
        End Property

        Public Property Scadenza_1() As System.Nullable(Of Integer)
            Get
                Return Me._Scadenza_1
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Scadenza_1.Equals(value) = False) Then
                    Me._Scadenza_1 = value
                End If
            End Set
        End Property

        Public Property Scadenza_2() As System.Nullable(Of Integer)
            Get
                Return Me._Scadenza_2
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Scadenza_2.Equals(value) = False) Then
                    Me._Scadenza_2 = value
                End If
            End Set
        End Property

        Public Property Scadenza_3() As System.Nullable(Of Integer)
            Get
                Return Me._Scadenza_3
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Scadenza_3.Equals(value) = False) Then
                    Me._Scadenza_3 = value
                End If
            End Set
        End Property

        Public Property Scadenza_4() As System.Nullable(Of Integer)
            Get
                Return Me._Scadenza_4
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Scadenza_4.Equals(value) = False) Then
                    Me._Scadenza_4 = value
                End If
            End Set
        End Property

        Public Property Scadenza_5() As System.Nullable(Of Integer)
            Get
                Return Me._Scadenza_5
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Scadenza_5.Equals(value) = False) Then
                    Me._Scadenza_5 = value
                End If
            End Set
        End Property

        Public Property Perc_Imponib_1() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imponib_1
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imponib_1.Equals(value) = False) Then
                    Me._Perc_Imponib_1 = value
                End If
            End Set
        End Property

        Public Property Perc_Imponib_2() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imponib_2
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imponib_2.Equals(value) = False) Then
                    Me._Perc_Imponib_2 = value
                End If
            End Set
        End Property

        Public Property Perc_Imponib_3() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imponib_3
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imponib_3.Equals(value) = False) Then
                    Me._Perc_Imponib_3 = value
                End If
            End Set
        End Property

        Public Property Perc_Imponib_4() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imponib_4
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imponib_4.Equals(value) = False) Then
                    Me._Perc_Imponib_4 = value
                End If
            End Set
        End Property

        Public Property Perc_Imponib_5() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imponib_5
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imponib_5.Equals(value) = False) Then
                    Me._Perc_Imponib_5 = value
                End If
            End Set
        End Property

        Public Property Perc_Imposta_1() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imposta_1
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imposta_1.Equals(value) = False) Then
                    Me._Perc_Imposta_1 = value
                End If
            End Set
        End Property

        Public Property Perc_Imposta_2() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imposta_2
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imposta_2.Equals(value) = False) Then
                    Me._Perc_Imposta_2 = value
                End If
            End Set
        End Property

        Public Property Perc_Imposta_3() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imposta_3
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imposta_3.Equals(value) = False) Then
                    Me._Perc_Imposta_3 = value
                End If
            End Set
        End Property

        Public Property Perc_Imposta_4() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imposta_4
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imposta_4.Equals(value) = False) Then
                    Me._Perc_Imposta_4 = value
                End If
            End Set
        End Property

        Public Property Perc_Imposta_5() As System.Nullable(Of Single)
            Get
                Return Me._Perc_Imposta_5
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Perc_Imposta_5.Equals(value) = False) Then
                    Me._Perc_Imposta_5 = value
                End If
            End Set
        End Property

        Public Property Mese_Escluso_1() As System.Nullable(Of Integer)
            Get
                Return Me._Mese_Escluso_1
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Mese_Escluso_1.Equals(value) = False) Then
                    Me._Mese_Escluso_1 = value
                End If
            End Set
        End Property

        Public Property Mese_Escluso_2() As System.Nullable(Of Integer)
            Get
                Return Me._Mese_Escluso_2
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Mese_Escluso_2.Equals(value) = False) Then
                    Me._Mese_Escluso_2 = value
                End If
            End Set
        End Property

        Public Property Spese_Incasso() As System.Nullable(Of Decimal)
            Get
                Return Me._Spese_Incasso
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Spese_Incasso.Equals(value) = False) Then
                    Me._Spese_Incasso = value
                End If
            End Set
        End Property

        Public Property IVA_Spese_Incasso() As System.Nullable(Of Integer)
            Get
                Return Me._IVA_Spese_Incasso
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IVA_Spese_Incasso.Equals(value) = False) Then
                    Me._IVA_Spese_Incasso = value
                End If
            End Set
        End Property

        Public Property Sconto_Cassa() As System.Nullable(Of Single)
            Get
                Return Me._Sconto_Cassa
            End Get
            Set(ByVal value As System.Nullable(Of Single))
                If (Me._Sconto_Cassa.Equals(value) = False) Then
                    Me._Sconto_Cassa = value
                End If
            End Set
        End Property
    End Class
End Namespace
