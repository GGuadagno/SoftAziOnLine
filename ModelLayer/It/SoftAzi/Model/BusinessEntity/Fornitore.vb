Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class FornitoreEntity
        Private _Codice_CoGe As String

        Private _Rag_Soc As String

        Private _Indirizzo As String

        Private _Localita As String

        Private _CAP As String

        Private _Provincia As String

        Private _Nazione As String

        Private _Telefono1 As String

        Private _Telefono2 As String

        Private _Fax As String

        Private _Societa As System.Nullable(Of Integer)

        Private _Codice_Fiscale As String

        Private _Partita_IVA As String

        Private _Zona As System.Nullable(Of Integer)

        Private _Categoria As System.Nullable(Of Integer)

        Private _ABI_N As String

        Private _CAB_N As String

        Private _Regime_IVA As System.Nullable(Of Integer)

        Private _Pagamento_N As System.Nullable(Of Integer)

        Private _Bilancio_SN As String

        Private _Ragg_P As System.Nullable(Of Integer)

        Private _Allegato_IVA As System.Nullable(Of Integer)

        Private _Apertura As System.Nullable(Of Decimal)

        Private _DA_Apertura As String

        Private _Saldo_Dare As System.Nullable(Of Decimal)

        Private _Saldo_Avere As System.Nullable(Of Decimal)

        Private _Data_Agg_Saldi As System.Nullable(Of Date)

        Private _Saldo_Prec As System.Nullable(Of Decimal)

        Private _DA_Saldo_Prec As String

        Private _Dare_Chiusura As System.Nullable(Of Decimal)

        Private _Avere_Chiusura As System.Nullable(Of Decimal)

        Private _Riferimento As String

        Private _Denominazione As String

        Private _Titolare As String

        Private _Email As String

        Private _Saldo_Dare_2 As System.Nullable(Of Decimal)

        Private _Saldo_Avere_2 As System.Nullable(Of Decimal)

        Private _Dare_Chiusura_2 As System.Nullable(Of Decimal)

        Private _Avere_Chiusura_2 As System.Nullable(Of Decimal)

        Private _Apertura_2 As System.Nullable(Of Decimal)

        Private _DA_Apertura_2 As String

        Private _FuoriZona As System.Nullable(Of Integer)

        Private _Modalita_Invio_Ordine As String

        Private _Data_Inizio_Chiusura As System.Nullable(Of Date)

        Private _Data_Fine_Chiusura As System.Nullable(Of Date)

        Private _Conto_Corrente As String

        Private _Rit_Acconto As System.Nullable(Of Integer)

        Private _Provincia_Estera As String

        Private _IndirizzoSenzaNumero As String

        Private _NumeroCivico As String

        Private _Data_Nascita As System.Nullable(Of Date)

        Private _StRit_Acconto As System.Nullable(Of Integer)

        Private _CodTributo As System.Nullable(Of Integer)

        Private _Codice_SEDE As String

        Private _Codice_Costo As String

        Private _NoFatt As System.Nullable(Of Integer)

        Private _CIN As String

        Private _NazIBAN As String

        Private _CINEUIBAN As String

        Private _SWIFT As String

        Private _Listino As System.Nullable(Of Integer)

        Private _CSAggrAllIVA As System.Nullable(Of Boolean)

        Private _InseritoDa As String
        Private _ModificatoDa As String
        Private _IBAN_Ditta As String
        Private _PECEMail As String

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

        Public Property Rag_Soc() As String
            Get
                Return Me._Rag_Soc
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Rag_Soc, value) = False) Then
                    Me._Rag_Soc = value
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

        Public Property Telefono1() As String
            Get
                Return Me._Telefono1
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Telefono1, value) = False) Then
                    Me._Telefono1 = value
                End If
            End Set
        End Property

        Public Property Telefono2() As String
            Get
                Return Me._Telefono2
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Telefono2, value) = False) Then
                    Me._Telefono2 = value
                End If
            End Set
        End Property

        Public Property Fax() As String
            Get
                Return Me._Fax
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Fax, value) = False) Then
                    Me._Fax = value
                End If
            End Set
        End Property

        Public Property Societa() As System.Nullable(Of Integer)
            Get
                Return Me._Societa
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Societa.Equals(value) = False) Then
                    Me._Societa = value
                End If
            End Set
        End Property

        Public Property Codice_Fiscale() As String
            Get
                Return Me._Codice_Fiscale
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Codice_Fiscale, value) = False) Then
                    Me._Codice_Fiscale = value
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

        Public Property Zona() As System.Nullable(Of Integer)
            Get
                Return Me._Zona
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Zona.Equals(value) = False) Then
                    Me._Zona = value
                End If
            End Set
        End Property

        Public Property Categoria() As System.Nullable(Of Integer)
            Get
                Return Me._Categoria
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Categoria.Equals(value) = False) Then
                    Me._Categoria = value
                End If
            End Set
        End Property

        Public Property ABI_N() As String
            Get
                Return Me._ABI_N
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ABI_N, value) = False) Then
                    Me._ABI_N = value
                End If
            End Set
        End Property

        Public Property CAB_N() As String
            Get
                Return Me._CAB_N
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CAB_N, value) = False) Then
                    Me._CAB_N = value
                End If
            End Set
        End Property

        Public Property Regime_IVA() As System.Nullable(Of Integer)
            Get
                Return Me._Regime_IVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Regime_IVA.Equals(value) = False) Then
                    Me._Regime_IVA = value
                End If
            End Set
        End Property

        Public Property Pagamento_N() As System.Nullable(Of Integer)
            Get
                Return Me._Pagamento_N
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Pagamento_N.Equals(value) = False) Then
                    Me._Pagamento_N = value
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

        Public Property Ragg_P() As System.Nullable(Of Integer)
            Get
                Return Me._Ragg_P
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Ragg_P.Equals(value) = False) Then
                    Me._Ragg_P = value
                End If
            End Set
        End Property

        Public Property Allegato_IVA() As System.Nullable(Of Integer)
            Get
                Return Me._Allegato_IVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Allegato_IVA.Equals(value) = False) Then
                    Me._Allegato_IVA = value
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

        Public Property Riferimento() As String
            Get
                Return Me._Riferimento
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Riferimento, value) = False) Then
                    Me._Riferimento = value
                End If
            End Set
        End Property

        Public Property Denominazione() As String
            Get
                Return Me._Denominazione
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Denominazione, value) = False) Then
                    Me._Denominazione = value
                End If
            End Set
        End Property

        Public Property Titolare() As String
            Get
                Return Me._Titolare
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Titolare, value) = False) Then
                    Me._Titolare = value
                End If
            End Set
        End Property

        Public Property Email() As String
            Get
                Return Me._Email
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Email, value) = False) Then
                    Me._Email = value
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

        Public Property DA_Apertura_2() As String
            Get
                Return Me._DA_Apertura_2
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._DA_Apertura_2, value) = False) Then
                    Me._DA_Apertura_2 = value
                End If
            End Set
        End Property

        Public Property FuoriZona() As System.Nullable(Of Integer)
            Get
                Return Me._FuoriZona
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._FuoriZona.Equals(value) = False) Then
                    Me._FuoriZona = value
                End If
            End Set
        End Property

        Public Property Modalita_Invio_Ordine() As String
            Get
                Return Me._Modalita_Invio_Ordine
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Modalita_Invio_Ordine, value) = False) Then
                    Me._Modalita_Invio_Ordine = value
                End If
            End Set
        End Property

        Public Property Data_Inizio_Chiusura() As System.Nullable(Of Date)
            Get
                Return Me._Data_Inizio_Chiusura
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._Data_Inizio_Chiusura.Equals(value) = False) Then
                    Me._Data_Inizio_Chiusura = value
                End If
            End Set
        End Property

        Public Property Data_Fine_Chiusura() As System.Nullable(Of Date)
            Get
                Return Me._Data_Fine_Chiusura
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._Data_Fine_Chiusura.Equals(value) = False) Then
                    Me._Data_Fine_Chiusura = value
                End If
            End Set
        End Property

        Public Property Conto_Corrente() As String
            Get
                Return Me._Conto_Corrente
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Conto_Corrente, value) = False) Then
                    Me._Conto_Corrente = value
                End If
            End Set
        End Property

        Public Property Rit_Acconto() As System.Nullable(Of Integer)
            Get
                Return Me._Rit_Acconto
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Rit_Acconto.Equals(value) = False) Then
                    Me._Rit_Acconto = value
                End If
            End Set
        End Property

        Public Property Provincia_Estera() As String
            Get
                Return Me._Provincia_Estera
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Provincia_Estera, value) = False) Then
                    Me._Provincia_Estera = value
                End If
            End Set
        End Property

        Public Property IndirizzoSenzaNumero() As String
            Get
                Return Me._IndirizzoSenzaNumero
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._IndirizzoSenzaNumero, value) = False) Then
                    Me._IndirizzoSenzaNumero = value
                End If
            End Set
        End Property

        Public Property NumeroCivico() As String
            Get
                Return Me._NumeroCivico
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._NumeroCivico, value) = False) Then
                    Me._NumeroCivico = value
                End If
            End Set
        End Property

        Public Property Data_Nascita() As System.Nullable(Of Date)
            Get
                Return Me._Data_Nascita
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._Data_Nascita.Equals(value) = False) Then
                    Me._Data_Nascita = value
                End If
            End Set
        End Property

        Public Property StRit_Acconto() As System.Nullable(Of Integer)
            Get
                Return Me._StRit_Acconto
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._StRit_Acconto.Equals(value) = False) Then
                    Me._StRit_Acconto = value
                End If
            End Set
        End Property

        Public Property CodTributo() As System.Nullable(Of Integer)
            Get
                Return Me._CodTributo
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodTributo.Equals(value) = False) Then
                    Me._CodTributo = value
                End If
            End Set
        End Property

        Public Property Codice_SEDE() As String
            Get
                Return Me._Codice_SEDE
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Codice_SEDE, value) = False) Then
                    Me._Codice_SEDE = value
                End If
            End Set
        End Property

        Public Property Codice_Costo() As String
            Get
                Return Me._Codice_Costo
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Codice_Costo, value) = False) Then
                    Me._Codice_Costo = value
                End If
            End Set
        End Property

        Public Property NoFatt() As System.Nullable(Of Integer)
            Get
                Return Me._NoFatt
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NoFatt.Equals(value) = False) Then
                    Me._NoFatt = value
                End If
            End Set
        End Property

        Public Property CIN() As String
            Get
                Return Me._CIN
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CIN, value) = False) Then
                    Me._CIN = value
                End If
            End Set
        End Property

        Public Property NazIBAN() As String
            Get
                Return Me._NazIBAN
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._NazIBAN, value) = False) Then
                    Me._NazIBAN = value
                End If
            End Set
        End Property

        Public Property CINEUIBAN() As String
            Get
                Return Me._CINEUIBAN
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CINEUIBAN, value) = False) Then
                    Me._CINEUIBAN = value
                End If
            End Set
        End Property

        Public Property SWIFT() As String
            Get
                Return Me._SWIFT
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._SWIFT, value) = False) Then
                    Me._SWIFT = value
                End If
            End Set
        End Property

        Public Property Listino() As System.Nullable(Of Integer)
            Get
                Return Me._Listino
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Listino.Equals(value) = False) Then
                    Me._Listino = value
                End If
            End Set
        End Property

        Public Property CSAggrAllIVA() As System.Nullable(Of Boolean)
            Get
                Return Me._CSAggrAllIVA
            End Get
            Set(ByVal value As System.Nullable(Of Boolean))
                If (Me._CSAggrAllIVA.Equals(value) = False) Then
                    Me._CSAggrAllIVA = value
                End If
            End Set
        End Property

        Public Property InseritoDa() As String
            Get
                Return Me._InseritoDa
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._InseritoDa, value) = False) Then
                    Me._InseritoDa = value
                End If
            End Set
        End Property
        Public Property ModificatoDa() As String
            Get
                Return Me._ModificatoDa
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ModificatoDa, value) = False) Then
                    Me._ModificatoDa = value
                End If
            End Set
        End Property
        Public Property IBAN_Ditta() As String
            Get
                Return Me._IBAN_Ditta
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._IBAN_Ditta, value) = False) Then
                    Me._IBAN_Ditta = value
                End If
            End Set
        End Property
        Public Property PECEMail() As String
            Get
                Return Me._PECEMail
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PECEMail, value) = False) Then
                    Me._PECEMail = value
                End If
            End Set
        End Property
    End Class
End Namespace