Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class PianoDeiContiEntity

        Private _Codice_CoGe As String

        Private _Descrizione As String

        Private _Tipo As String

        Private _E_P As String

        Private _Bilancio_SN As String

        Private _Sezione As String

        Private _Apertura As System.Nullable(Of Decimal)

        Private _DA_Apertura As String

        Private _Saldo_Dare As System.Nullable(Of Decimal)

        Private _Saldo_Avere As System.Nullable(Of Decimal)

        Private _Data_Agg_Saldi As System.Nullable(Of Date)

        Private _Saldo_Prec As System.Nullable(Of Decimal)

        Private _DA_Saldo_Prec As String

        Private _Conto_Banca As System.Nullable(Of Integer)

        Private _ABI As String

        Private _CAB As String

        Private _Ragg_E_P As System.Nullable(Of Integer)

        Private _Livello As System.Nullable(Of Integer)

        Private _Dare_Chiusura As System.Nullable(Of Decimal)

        Private _Avere_Chiusura As System.Nullable(Of Decimal)

        Private _Saldo_Dare_2 As System.Nullable(Of Decimal)

        Private _Saldo_Avere_2 As System.Nullable(Of Decimal)

        Private _Dare_Chiusura_2 As System.Nullable(Of Decimal)

        Private _Avere_Chiusura_2 As System.Nullable(Of Decimal)

        Private _Apertura_2 As System.Nullable(Of Decimal)

        Private _Entrambe As System.Nullable(Of Integer)

        Public Sub New()
            MyBase.New()
        End Sub

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

        Public Property Tipo() As String
            Get
                Return Me._Tipo
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Tipo, value) = False) Then
                    Me._Tipo = value
                End If
            End Set
        End Property

        Public Property E_P() As String
            Get
                Return Me._E_P
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._E_P, value) = False) Then
                    Me._E_P = value
                End If
            End Set
        End Property

        Public Property Bilancio_SN() As String
            Get
                Return Me._Bilancio_SN
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Bilancio_SN, value) = False) Then
                    Me._Bilancio_SN = value
                End If
            End Set
        End Property

        Public Property Sezione() As String
            Get
                Return Me._Sezione
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Sezione, value) = False) Then
                    Me._Sezione = value
                End If
            End Set
        End Property

        Public Property Apertura() As System.Nullable(Of Decimal)
            Get
                Return Me._Apertura
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Apertura.Equals(value) = False) Then
                    Me._Apertura = value
                End If
            End Set
        End Property

        Public Property DA_Apertura() As String
            Get
                Return Me._DA_Apertura
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._DA_Apertura, value) = False) Then
                    Me._DA_Apertura = value
                End If
            End Set
        End Property

        Public Property Saldo_Dare() As System.Nullable(Of Decimal)
            Get
                Return Me._Saldo_Dare
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Saldo_Dare.Equals(value) = False) Then
                    Me._Saldo_Dare = value
                End If
            End Set
        End Property

        Public Property Saldo_Avere() As System.Nullable(Of Decimal)
            Get
                Return Me._Saldo_Avere
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Saldo_Avere.Equals(value) = False) Then
                    Me._Saldo_Avere = value
                End If
            End Set
        End Property
        Public Property Data_Agg_Saldi() As System.Nullable(Of Date)
            Get
                Return Me._Data_Agg_Saldi
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._Data_Agg_Saldi.Equals(value) = False) Then
                    Me._Data_Agg_Saldi = value
                End If
            End Set
        End Property

        Public Property Saldo_Prec() As System.Nullable(Of Decimal)
            Get
                Return Me._Saldo_Prec
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Saldo_Prec.Equals(value) = False) Then
                    Me._Saldo_Prec = value
                End If
            End Set
        End Property

        Public Property DA_Saldo_Prec() As String
            Get
                Return Me._DA_Saldo_Prec
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._DA_Saldo_Prec, value) = False) Then
                    Me._DA_Saldo_Prec = value
                End If
            End Set
        End Property

        Public Property Conto_Banca() As System.Nullable(Of Integer)
            Get
                Return Me._Conto_Banca
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Conto_Banca.Equals(value) = False) Then
                    Me._Conto_Banca = value
                End If
            End Set
        End Property

        Public Property ABI() As String
            Get
                Return Me._ABI
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ABI, value) = False) Then
                    Me._ABI = value
                End If
            End Set
        End Property

        Public Property CAB() As String
            Get
                Return Me._CAB
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CAB, value) = False) Then
                    Me._CAB = value
                End If
            End Set
        End Property

        Public Property Ragg_E_P() As System.Nullable(Of Integer)
            Get
                Return Me._Ragg_E_P
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Ragg_E_P.Equals(value) = False) Then
                    Me._Ragg_E_P = value
                End If
            End Set
        End Property

        Public Property Livello() As System.Nullable(Of Integer)
            Get
                Return Me._Livello
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Livello.Equals(value) = False) Then
                    Me._Livello = value
                End If
            End Set
        End Property

        Public Property Dare_Chiusura() As System.Nullable(Of Decimal)
            Get
                Return Me._Dare_Chiusura
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Dare_Chiusura.Equals(value) = False) Then
                    Me._Dare_Chiusura = value
                End If
            End Set
        End Property

        Public Property Avere_Chiusura() As System.Nullable(Of Decimal)
            Get
                Return Me._Avere_Chiusura
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Avere_Chiusura.Equals(value) = False) Then
                    Me._Avere_Chiusura = value
                End If
            End Set
        End Property

        Public Property Saldo_Dare_2() As System.Nullable(Of Decimal)
            Get
                Return Me._Saldo_Dare_2
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Saldo_Dare_2.Equals(value) = False) Then
                    Me._Saldo_Dare_2 = value
                End If
            End Set
        End Property

        Public Property Saldo_Avere_2() As System.Nullable(Of Decimal)
            Get
                Return Me._Saldo_Avere_2
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Saldo_Avere_2.Equals(value) = False) Then
                    Me._Saldo_Avere_2 = value
                End If
            End Set
        End Property

        Public Property Dare_Chiusura_2() As System.Nullable(Of Decimal)
            Get
                Return Me._Dare_Chiusura_2
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Dare_Chiusura_2.Equals(value) = False) Then
                    Me._Dare_Chiusura_2 = value
                End If
            End Set
        End Property

        Public Property Avere_Chiusura_2() As System.Nullable(Of Decimal)
            Get
                Return Me._Avere_Chiusura_2
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Avere_Chiusura_2.Equals(value) = False) Then
                    Me._Avere_Chiusura_2 = value
                End If
            End Set
        End Property

        Public Property Apertura_2() As System.Nullable(Of Decimal)
            Get
                Return Me._Apertura_2
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Apertura_2.Equals(value) = False) Then
                    Me._Apertura_2 = value
                End If
            End Set
        End Property

        Public Property Entrambe() As System.Nullable(Of Integer)
            Get
                Return Me._Entrambe
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Entrambe.Equals(value) = False) Then
                    Me._Entrambe = value
                End If
            End Set
        End Property
    End Class
End Namespace
