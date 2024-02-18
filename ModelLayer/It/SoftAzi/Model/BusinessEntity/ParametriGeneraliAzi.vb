Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class ParametriGeneraliAziEntity
        
        Private _RighePerPaginaDDT As System.Nullable(Of Integer)

        Private _RighePerPaginaFatt As System.Nullable(Of Integer)

        Private _NumeroDDT As System.Nullable(Of Integer)

        Private _NumeroFattura As System.Nullable(Of Integer)

        Private _NumeroNotaAccredito As System.Nullable(Of Integer)

        Private _NumeroNotaCdenza As System.Nullable(Of Integer)

        Private _NoteAccreditoNumerazioneSeparata As Boolean

        Private _NoteCdenzaNumSep As Boolean

        Private _IVATrasporto As System.Nullable(Of Integer)

        Private _AnteprimaStampa As Boolean

        Private _RighePerPaginaORDINI As System.Nullable(Of Integer)

        Private _CodiceContoRicavoCOGE As String

        Private _CodiceCausaleCOGE As System.Nullable(Of Integer)

        Private _AggiornaMagazzino As Boolean

        Private _DataUltimaCompattazione As System.Nullable(Of Date)

        Private _NumeroOrdineFornitore As System.Nullable(Of Integer)

        Private _NumeroRiordinoFornitore As System.Nullable(Of Integer)

        Private _CodiceCausaleRiordino As System.Nullable(Of Integer)

        Private _LarghezzaBolla As System.Nullable(Of Integer)

        Private _MaxDescrizione As System.Nullable(Of Integer)

        Private _ContoCorrispettivi As String

        Private _ContoCassa As String

        Private _CodiceCausaleIncasso As System.Nullable(Of Integer)

        Private _ChiedoListino As Boolean

        Private _CodiceDescrizione As Boolean

        Private _DueCopie As Boolean

        Private _CodiceCausaleTrasferimento As System.Nullable(Of Integer)

        Private _IvaSpese As System.Nullable(Of Integer)

        Private _CodiceCausaleCorrisp As System.Nullable(Of Integer)

        Private _CodiceCausaleCOGENA As System.Nullable(Of Integer)

        Private _CodiceCausaleIncassoNA As System.Nullable(Of Integer)

        Private _ContoSpeseIncasso As String

        Private _ContoSpeseTrasporto As String

        Private _ContoSpeseVarie As String

        Private _CodiceCausaleTrasferimentoFiliale As System.Nullable(Of Integer)

        Private _StringaConai As String

        Private _StringaBolla As String

        Private _AspettoDeiBeni As String

        Private _CalcoloColliAutomatico As Boolean

        Private _PasswordMovimenti As String

        Private _Iva_Imballo As System.Nullable(Of Integer)

        Private _ContoSpeseImballo As String

        Private _DicituraASPEST As String

        Private _DicituraPORTO As String

        Private _NumeroOrdineCliente As System.Nullable(Of Integer)

        Private _CarPerRiga As System.Nullable(Of Integer)

        Private _Decimali_Sconto As System.Nullable(Of Integer)

        Private _Decimali_Provvigione As System.Nullable(Of Integer)

        Private _NumeroPreventivo As System.Nullable(Of Integer)

        Private _RighePerPaginaPrev As System.Nullable(Of Integer)

        Private _ContoRiBa As String

        Private _CausaleRiBa As System.Nullable(Of Integer)

        Private _RegIncasso As Boolean

        Private _NumSconti As System.Nullable(Of Integer)

        Private _Num_Differenziata As Boolean

        Private _Cod_Valuta As String

        Private _Decimali_Prezzi As System.Nullable(Of Integer)

        Private _Visual_2_Valute As Boolean

        Private _causaleMMpos As System.Nullable(Of Integer)

        Private _causaleMMneg As System.Nullable(Of Integer)

        Private _Decimali_Prezzi_2 As System.Nullable(Of Integer)

        Private _CodCausaleVendita As System.Nullable(Of Integer)

        Private _CodiceNumerico As Boolean

        Private _LunghezzaMaxCodice As System.Nullable(Of Integer)

        Private _ValoreMinimoOrdine As System.Nullable(Of Decimal)

        Private _gg_lavorativi_sett As System.Nullable(Of Integer)

        Private _giorno_riposo As System.Nullable(Of Integer)

        Private _sett_verifica_qta As System.Nullable(Of Integer)

        Private _NumScontiForn As System.Nullable(Of Integer)

        Private _DecScontoForn As System.Nullable(Of Integer)

        Private _ControlloSottoscorta As Boolean

        Private _NumeroSped As System.Nullable(Of Integer)

        Private _RegPNRB As Boolean

        Private _LivelloMaxDistBase As System.Nullable(Of Integer)

        Private _CalcoloScontoSuImporto As Boolean

        Private _CodTipoFatt As String

        Private _CausaleRipristinoSaldi As System.Nullable(Of Integer)

        Private _DisabilitaRiordino As Boolean

        Private _AnniFuoriProd As System.Nullable(Of Integer)

        Private _NumeroOrdineDaDeposito As System.Nullable(Of Integer)

        Private _CausSBNatale As System.Nullable(Of Integer)

        Private _CausSBPasqua As System.Nullable(Of Integer)

        Private _CausDDTDep As System.Nullable(Of Integer)

        Private _CausVendDep As System.Nullable(Of Integer)

        Private _CausResoDep As System.Nullable(Of Integer)

        Private _PercorsoStampaOrdini As String

        Private _PercorsoStampaDDT As String

        Private _PercorsoStampaFatt As String

        Private _PercorsoStampaPrev As String

        Private _CausNCResi As System.Nullable(Of Integer)

        Private _CausNCAbbuono As System.Nullable(Of Integer)

        Private _CausNCScontoOmesso As System.Nullable(Of Integer)

        Private _CausNCDiffPrezzo As System.Nullable(Of Integer)

        Private _UltAgRicalcolato As System.Nullable(Of Integer)

        Private _CausNCSBPrec As System.Nullable(Of Integer)

        Private _NumeroBC As System.Nullable(Of Integer)

        Private _DueCopieNZ As Boolean

        Private _CausRimInizialeDep As System.Nullable(Of Integer)

        Private _ComunicazioneRintracciabilita As String

        Private _PieDiPaginaRintracciabilita As String

        Private _SMTPServer As String

        Private _SMTPPorta As System.Nullable(Of Integer)

        Private _SMTPUserName As String

        Private _SMTPPassword As String

        Private _SMTPMailSender As String

        Private _DettaglioCestiDDT As Boolean

        Private _NumeroOCL As System.Nullable(Of Integer)

        Private _CausOrdCL As System.Nullable(Of Integer)

        Private _CausDDTCL As System.Nullable(Of Integer)

        Private _CausResoCL As System.Nullable(Of Integer)

        Private _CausFineCL As System.Nullable(Of Integer)

        Private _CausRestiCL As System.Nullable(Of Integer)

        Private _CausCarMagCL As System.Nullable(Of Integer)

        Private _ListinoCL As System.Nullable(Of Integer)

        Private _Banca As String

        Private _ABI As String

        Private _CAB As String

        Private _CIN As String

        Private _CC As String

        Private _NazIBAN As String

        Private _CINEUIBAN As String

        Private _SWIFT As String

        Private _PrezziDDT As Boolean

        Private _NUltimiPrezziAcq As System.Nullable(Of Integer)

        Private _Decimali_Grandezze As System.Nullable(Of Integer)

        Private _NGG_Validita As System.Nullable(Of Integer)
        Private _NGG_Consegna As System.Nullable(Of Integer)
        Private _NumeroFA As System.Nullable(Of Integer) 'GIU240312
        Private _NumeroPA As System.Nullable(Of Integer) 'GIU210714
        Private _NumeroNCPA As System.Nullable(Of Integer) 'GIU220714
        Private _NumeroNCPASep As Boolean 'GIU220714
        'giu300718
        Private _SelAICatCli As System.Nullable(Of Integer)
        Private _SelAIDaData As System.Nullable(Of Integer)
        Private _SelAIAData As System.Nullable(Of Integer)
        Private _AIServizioEmail As Boolean
        Private _SelAIScGa As Boolean
        Private _SelAIScEl As Boolean
        Private _SelAIScBa As Boolean
        Private _ScCassaDett As Boolean 'giu301218
        'giu270219
        Private _ImpMinBollo As System.Nullable(Of Decimal)
        Private _IVABollo As System.Nullable(Of Integer)
        Private _IVAScMerce As System.Nullable(Of Integer)
        Private _ContoRitAcconto As String
        Private _Bollo As System.Nullable(Of Decimal)
        Private _AIServizioEmailAttiva As String

        Public Property RighePerPaginaDDT() As System.Nullable(Of Integer)
            Get
                Return Me._RighePerPaginaDDT
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RighePerPaginaDDT.Equals(value) = False) Then
                    Me._RighePerPaginaDDT = value
                End If
            End Set
        End Property

        Public Property RighePerPaginaFatt() As System.Nullable(Of Integer)
            Get
                Return Me._RighePerPaginaFatt
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RighePerPaginaFatt.Equals(value) = False) Then
                    Me._RighePerPaginaFatt = value
                End If
            End Set
        End Property

        Public Property NumeroDDT() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroDDT
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroDDT.Equals(value) = False) Then
                    Me._NumeroDDT = value
                End If
            End Set
        End Property

        Public Property NumeroFattura() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroFattura
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroFattura.Equals(value) = False) Then
                    Me._NumeroFattura = value
                End If
            End Set
        End Property

        Public Property NumeroNotaAccredito() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroNotaAccredito
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroNotaAccredito.Equals(value) = False) Then
                    Me._NumeroNotaAccredito = value
                End If
            End Set
        End Property

        Public Property NumeroNotaCdenza() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroNotaCdenza
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroNotaCdenza.Equals(value) = False) Then
                    Me._NumeroNotaCdenza = value
                End If
            End Set
        End Property

        Public Property NoteAccreditoNumerazioneSeparata() As Boolean
            Get
                Return Me._NoteAccreditoNumerazioneSeparata
            End Get
            Set(ByVal value As Boolean)
                If ((Me._NoteAccreditoNumerazioneSeparata = value) _
                   = False) Then
                    Me._NoteAccreditoNumerazioneSeparata = value
                End If
            End Set
        End Property

        Public Property NoteCdenzaNumSep() As Boolean
            Get
                Return Me._NoteCdenzaNumSep
            End Get
            Set(ByVal value As Boolean)
                If ((Me._NoteCdenzaNumSep = value) _
                   = False) Then
                    Me._NoteCdenzaNumSep = value
                End If
            End Set
        End Property

        Public Property IVATrasporto() As System.Nullable(Of Integer)
            Get
                Return Me._IVATrasporto
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IVATrasporto.Equals(value) = False) Then
                    Me._IVATrasporto = value
                End If
            End Set
        End Property

        Public Property AnteprimaStampa() As Boolean
            Get
                Return Me._AnteprimaStampa
            End Get
            Set(ByVal value As Boolean)
                If ((Me._AnteprimaStampa = value) _
                   = False) Then
                    Me._AnteprimaStampa = value
                End If
            End Set
        End Property

        Public Property RighePerPaginaORDINI() As System.Nullable(Of Integer)
            Get
                Return Me._RighePerPaginaORDINI
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RighePerPaginaORDINI.Equals(value) = False) Then
                    Me._RighePerPaginaORDINI = value
                End If
            End Set
        End Property

        Public Property CodiceContoRicavoCOGE() As String
            Get
                Return Me._CodiceContoRicavoCOGE
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CodiceContoRicavoCOGE, value) = False) Then
                    Me._CodiceContoRicavoCOGE = value
                End If
            End Set
        End Property

        Public Property CodiceCausaleCOGE() As System.Nullable(Of Integer)
            Get
                Return Me._CodiceCausaleCOGE
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodiceCausaleCOGE.Equals(value) = False) Then
                    Me._CodiceCausaleCOGE = value
                End If
            End Set
        End Property

        Public Property AggiornaMagazzino() As Boolean
            Get
                Return Me._AggiornaMagazzino
            End Get
            Set(ByVal value As Boolean)
                If ((Me._AggiornaMagazzino = value) _
                   = False) Then
                    Me._AggiornaMagazzino = value
                End If
            End Set
        End Property

        Public Property DataUltimaCompattazione() As System.Nullable(Of Date)
            Get
                Return Me._DataUltimaCompattazione
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._DataUltimaCompattazione.Equals(value) = False) Then
                    Me._DataUltimaCompattazione = value
                End If
            End Set
        End Property

        Public Property NumeroOrdineFornitore() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroOrdineFornitore
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroOrdineFornitore.Equals(value) = False) Then
                    Me._NumeroOrdineFornitore = value
                End If
            End Set
        End Property

        Public Property NumeroRiordinoFornitore() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroRiordinoFornitore
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroRiordinoFornitore.Equals(value) = False) Then
                    Me._NumeroRiordinoFornitore = value
                End If
            End Set
        End Property

        Public Property CodiceCausaleRiordino() As System.Nullable(Of Integer)
            Get
                Return Me._CodiceCausaleRiordino
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodiceCausaleRiordino.Equals(value) = False) Then
                    Me._CodiceCausaleRiordino = value
                End If
            End Set
        End Property

        Public Property LarghezzaBolla() As System.Nullable(Of Integer)
            Get
                Return Me._LarghezzaBolla
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._LarghezzaBolla.Equals(value) = False) Then
                    Me._LarghezzaBolla = value
                End If
            End Set
        End Property

        Public Property MaxDescrizione() As System.Nullable(Of Integer)
            Get
                Return Me._MaxDescrizione
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._MaxDescrizione.Equals(value) = False) Then
                    Me._MaxDescrizione = value
                End If
            End Set
        End Property

        Public Property ContoCorrispettivi() As String
            Get
                Return Me._ContoCorrispettivi
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ContoCorrispettivi, value) = False) Then
                    Me._ContoCorrispettivi = value
                End If
            End Set
        End Property

        Public Property ContoCassa() As String
            Get
                Return Me._ContoCassa
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ContoCassa, value) = False) Then
                    Me._ContoCassa = value
                End If
            End Set
        End Property

        Public Property CodiceCausaleIncasso() As System.Nullable(Of Integer)
            Get
                Return Me._CodiceCausaleIncasso
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodiceCausaleIncasso.Equals(value) = False) Then
                    Me._CodiceCausaleIncasso = value
                End If
            End Set
        End Property

        Public Property ChiedoListino() As Boolean
            Get
                Return Me._ChiedoListino
            End Get
            Set(ByVal value As Boolean)
                If ((Me._ChiedoListino = value) _
                   = False) Then
                    Me._ChiedoListino = value
                End If
            End Set
        End Property

        Public Property CodiceDescrizione() As Boolean
            Get
                Return Me._CodiceDescrizione
            End Get
            Set(ByVal value As Boolean)
                If ((Me._CodiceDescrizione = value) _
                   = False) Then
                    Me._CodiceDescrizione = value
                End If
            End Set
        End Property

        Public Property DueCopie() As Boolean
            Get
                Return Me._DueCopie
            End Get
            Set(ByVal value As Boolean)
                If ((Me._DueCopie = value) _
                   = False) Then
                    Me._DueCopie = value
                End If
            End Set
        End Property

        Public Property CodiceCausaleTrasferimento() As System.Nullable(Of Integer)
            Get
                Return Me._CodiceCausaleTrasferimento
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodiceCausaleTrasferimento.Equals(value) = False) Then
                    Me._CodiceCausaleTrasferimento = value
                End If
            End Set
        End Property

        Public Property IvaSpese() As System.Nullable(Of Integer)
            Get
                Return Me._IvaSpese
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IvaSpese.Equals(value) = False) Then
                    Me._IvaSpese = value
                End If
            End Set
        End Property

        Public Property CodiceCausaleCorrisp() As System.Nullable(Of Integer)
            Get
                Return Me._CodiceCausaleCorrisp
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodiceCausaleCorrisp.Equals(value) = False) Then
                    Me._CodiceCausaleCorrisp = value
                End If
            End Set
        End Property

        Public Property CodiceCausaleCOGENA() As System.Nullable(Of Integer)
            Get
                Return Me._CodiceCausaleCOGENA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodiceCausaleCOGENA.Equals(value) = False) Then
                    Me._CodiceCausaleCOGENA = value
                End If
            End Set
        End Property

        Public Property CodiceCausaleIncassoNA() As System.Nullable(Of Integer)
            Get
                Return Me._CodiceCausaleIncassoNA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodiceCausaleIncassoNA.Equals(value) = False) Then
                    Me._CodiceCausaleIncassoNA = value
                End If
            End Set
        End Property

        Public Property ContoSpeseIncasso() As String
            Get
                Return Me._ContoSpeseIncasso
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ContoSpeseIncasso, value) = False) Then
                    Me._ContoSpeseIncasso = value
                End If
            End Set
        End Property

        Public Property ContoSpeseTrasporto() As String
            Get
                Return Me._ContoSpeseTrasporto
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ContoSpeseTrasporto, value) = False) Then
                    Me._ContoSpeseTrasporto = value
                End If
            End Set
        End Property

        Public Property ContoSpeseVarie() As String
            Get
                Return Me._ContoSpeseVarie
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ContoSpeseVarie, value) = False) Then
                    Me._ContoSpeseVarie = value
                End If
            End Set
        End Property

        Public Property CodiceCausaleTrasferimentoFiliale() As System.Nullable(Of Integer)
            Get
                Return Me._CodiceCausaleTrasferimentoFiliale
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodiceCausaleTrasferimentoFiliale.Equals(value) = False) Then
                    Me._CodiceCausaleTrasferimentoFiliale = value
                End If
            End Set
        End Property

        Public Property StringaConai() As String
            Get
                Return Me._StringaConai
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._StringaConai, value) = False) Then
                    Me._StringaConai = value
                End If
            End Set
        End Property

        Public Property StringaBolla() As String
            Get
                Return Me._StringaBolla
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._StringaBolla, value) = False) Then
                    Me._StringaBolla = value
                End If
            End Set
        End Property

        Public Property AspettoDeiBeni() As String
            Get
                Return Me._AspettoDeiBeni
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._AspettoDeiBeni, value) = False) Then
                    Me._AspettoDeiBeni = value
                End If
            End Set
        End Property

        Public Property CalcoloColliAutomatico() As Boolean
            Get
                Return Me._CalcoloColliAutomatico
            End Get
            Set(ByVal value As Boolean)
                If ((Me._CalcoloColliAutomatico = value) _
                   = False) Then
                    Me._CalcoloColliAutomatico = value
                End If
            End Set
        End Property

        Public Property PasswordMovimenti() As String
            Get
                Return Me._PasswordMovimenti
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PasswordMovimenti, value) = False) Then
                    Me._PasswordMovimenti = value
                End If
            End Set
        End Property

        Public Property Iva_Imballo() As System.Nullable(Of Integer)
            Get
                Return Me._Iva_Imballo
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Iva_Imballo.Equals(value) = False) Then
                    Me._Iva_Imballo = value
                End If
            End Set
        End Property

        Public Property ContoSpeseImballo() As String
            Get
                Return Me._ContoSpeseImballo
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ContoSpeseImballo, value) = False) Then
                    Me._ContoSpeseImballo = value
                End If
            End Set
        End Property

        Public Property DicituraASPEST() As String
            Get
                Return Me._DicituraASPEST
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._DicituraASPEST, value) = False) Then
                    Me._DicituraASPEST = value
                End If
            End Set
        End Property

        Public Property DicituraPORTO() As String
            Get
                Return Me._DicituraPORTO
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._DicituraPORTO, value) = False) Then
                    Me._DicituraPORTO = value
                End If
            End Set
        End Property

        Public Property NumeroOrdineCliente() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroOrdineCliente
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroOrdineCliente.Equals(value) = False) Then
                    Me._NumeroOrdineCliente = value
                End If
            End Set
        End Property

        Public Property CarPerRiga() As System.Nullable(Of Integer)
            Get
                Return Me._CarPerRiga
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CarPerRiga.Equals(value) = False) Then
                    Me._CarPerRiga = value
                End If
            End Set
        End Property

        Public Property Decimali_Sconto() As System.Nullable(Of Integer)
            Get
                Return Me._Decimali_Sconto
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Decimali_Sconto.Equals(value) = False) Then
                    Me._Decimali_Sconto = value
                End If
            End Set
        End Property

        Public Property Decimali_Provvigione() As System.Nullable(Of Integer)
            Get
                Return Me._Decimali_Provvigione
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Decimali_Provvigione.Equals(value) = False) Then
                    Me._Decimali_Provvigione = value
                End If
            End Set
        End Property

        Public Property NumeroPreventivo() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroPreventivo
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroPreventivo.Equals(value) = False) Then
                    Me._NumeroPreventivo = value
                End If
            End Set
        End Property

        Public Property RighePerPaginaPrev() As System.Nullable(Of Integer)
            Get
                Return Me._RighePerPaginaPrev
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RighePerPaginaPrev.Equals(value) = False) Then
                    Me._RighePerPaginaPrev = value
                End If
            End Set
        End Property

        Public Property ContoRiBa() As String
            Get
                Return Me._ContoRiBa
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ContoRiBa, value) = False) Then
                    Me._ContoRiBa = value
                End If
            End Set
        End Property

        Public Property CausaleRiBa() As System.Nullable(Of Integer)
            Get
                Return Me._CausaleRiBa
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausaleRiBa.Equals(value) = False) Then
                    Me._CausaleRiBa = value
                End If
            End Set
        End Property

        Public Property RegIncasso() As Boolean
            Get
                Return Me._RegIncasso
            End Get
            Set(ByVal value As Boolean)
                If ((Me._RegIncasso = value) _
                   = False) Then
                    Me._RegIncasso = value
                End If
            End Set
        End Property

        Public Property NumSconti() As System.Nullable(Of Integer)
            Get
                Return Me._NumSconti
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumSconti.Equals(value) = False) Then
                    Me._NumSconti = value
                End If
            End Set
        End Property

        Public Property Num_Differenziata() As Boolean
            Get
                Return Me._Num_Differenziata
            End Get
            Set(ByVal value As Boolean)
                If ((Me._Num_Differenziata = value) _
                   = False) Then
                    Me._Num_Differenziata = value
                End If
            End Set
        End Property

        Public Property Cod_Valuta() As String
            Get
                Return Me._Cod_Valuta
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Cod_Valuta, value) = False) Then
                    Me._Cod_Valuta = value
                End If
            End Set
        End Property

        Public Property Decimali_Prezzi() As System.Nullable(Of Integer)
            Get
                Return Me._Decimali_Prezzi
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Decimali_Prezzi.Equals(value) = False) Then
                    Me._Decimali_Prezzi = value
                End If
            End Set
        End Property

        Public Property Visual_2_Valute() As Boolean
            Get
                Return Me._Visual_2_Valute
            End Get
            Set(ByVal value As Boolean)
                If ((Me._Visual_2_Valute = value) _
                   = False) Then
                    Me._Visual_2_Valute = value
                End If
            End Set
        End Property

        Public Property causaleMMpos() As System.Nullable(Of Integer)
            Get
                Return Me._causaleMMpos
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._causaleMMpos.Equals(value) = False) Then
                    Me._causaleMMpos = value
                End If
            End Set
        End Property

        Public Property causaleMMneg() As System.Nullable(Of Integer)
            Get
                Return Me._causaleMMneg
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._causaleMMneg.Equals(value) = False) Then
                    Me._causaleMMneg = value
                End If
            End Set
        End Property

        Public Property Decimali_Prezzi_2() As System.Nullable(Of Integer)
            Get
                Return Me._Decimali_Prezzi_2
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Decimali_Prezzi_2.Equals(value) = False) Then
                    Me._Decimali_Prezzi_2 = value
                End If
            End Set
        End Property

        Public Property CodCausaleVendita() As System.Nullable(Of Integer)
            Get
                Return Me._CodCausaleVendita
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CodCausaleVendita.Equals(value) = False) Then
                    Me._CodCausaleVendita = value
                End If
            End Set
        End Property

        Public Property CodiceNumerico() As Boolean
            Get
                Return Me._CodiceNumerico
            End Get
            Set(ByVal value As Boolean)
                If ((Me._CodiceNumerico = value) _
                   = False) Then
                    Me._CodiceNumerico = value
                End If
            End Set
        End Property

        Public Property LunghezzaMaxCodice() As System.Nullable(Of Integer)
            Get
                Return Me._LunghezzaMaxCodice
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._LunghezzaMaxCodice.Equals(value) = False) Then
                    Me._LunghezzaMaxCodice = value
                End If
            End Set
        End Property

        Public Property ValoreMinimoOrdine() As System.Nullable(Of Decimal)
            Get
                Return Me._ValoreMinimoOrdine
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._ValoreMinimoOrdine.Equals(value) = False) Then
                    Me._ValoreMinimoOrdine = value
                End If
            End Set
        End Property

        Public Property gg_lavorativi_sett() As System.Nullable(Of Integer)
            Get
                Return Me._gg_lavorativi_sett
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._gg_lavorativi_sett.Equals(value) = False) Then
                    Me._gg_lavorativi_sett = value
                End If
            End Set
        End Property

        Public Property giorno_riposo() As System.Nullable(Of Integer)
            Get
                Return Me._giorno_riposo
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._giorno_riposo.Equals(value) = False) Then
                    Me._giorno_riposo = value
                End If
            End Set
        End Property

        Public Property sett_verifica_qta() As System.Nullable(Of Integer)
            Get
                Return Me._sett_verifica_qta
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._sett_verifica_qta.Equals(value) = False) Then
                    Me._sett_verifica_qta = value
                End If
            End Set
        End Property

        Public Property NumScontiForn() As System.Nullable(Of Integer)
            Get
                Return Me._NumScontiForn
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumScontiForn.Equals(value) = False) Then
                    Me._NumScontiForn = value
                End If
            End Set
        End Property

        Public Property DecScontoForn() As System.Nullable(Of Integer)
            Get
                Return Me._DecScontoForn
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._DecScontoForn.Equals(value) = False) Then
                    Me._DecScontoForn = value
                End If
            End Set
        End Property

        Public Property ControlloSottoscorta() As Boolean
            Get
                Return Me._ControlloSottoscorta
            End Get
            Set(ByVal value As Boolean)
                If ((Me._ControlloSottoscorta = value) _
                   = False) Then
                    Me._ControlloSottoscorta = value
                End If
            End Set
        End Property

        Public Property NumeroSped() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroSped
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroSped.Equals(value) = False) Then
                    Me._NumeroSped = value
                End If
            End Set
        End Property

        Public Property RegPNRB() As Boolean
            Get
                Return Me._RegPNRB
            End Get
            Set(ByVal value As Boolean)
                If ((Me._RegPNRB = value) _
                   = False) Then
                    Me._RegPNRB = value
                End If
            End Set
        End Property

        Public Property LivelloMaxDistBase() As System.Nullable(Of Integer)
            Get
                Return Me._LivelloMaxDistBase
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._LivelloMaxDistBase.Equals(value) = False) Then
                    Me._LivelloMaxDistBase = value
                End If
            End Set
        End Property

        Public Property CalcoloScontoSuImporto() As Boolean
            Get
                Return Me._CalcoloScontoSuImporto
            End Get
            Set(ByVal value As Boolean)
                If ((Me._CalcoloScontoSuImporto = value) _
                   = False) Then
                    Me._CalcoloScontoSuImporto = value
                End If
            End Set
        End Property

        Public Property CodTipoFatt() As String
            Get
                Return Me._CodTipoFatt
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CodTipoFatt, value) = False) Then
                    Me._CodTipoFatt = value
                End If
            End Set
        End Property

        Public Property CausaleRipristinoSaldi() As System.Nullable(Of Integer)
            Get
                Return Me._CausaleRipristinoSaldi
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausaleRipristinoSaldi.Equals(value) = False) Then
                    Me._CausaleRipristinoSaldi = value
                End If
            End Set
        End Property

        Public Property DisabilitaRiordino() As Boolean
            Get
                Return Me._DisabilitaRiordino
            End Get
            Set(ByVal value As Boolean)
                If ((Me._DisabilitaRiordino = value) _
                   = False) Then
                    Me._DisabilitaRiordino = value
                End If
            End Set
        End Property

        Public Property AnniFuoriProd() As System.Nullable(Of Integer)
            Get
                Return Me._AnniFuoriProd
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._AnniFuoriProd.Equals(value) = False) Then
                    Me._AnniFuoriProd = value
                End If
            End Set
        End Property

        Public Property NumeroOrdineDaDeposito() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroOrdineDaDeposito
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroOrdineDaDeposito.Equals(value) = False) Then
                    Me._NumeroOrdineDaDeposito = value
                End If
            End Set
        End Property

        Public Property CausSBNatale() As System.Nullable(Of Integer)
            Get
                Return Me._CausSBNatale
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausSBNatale.Equals(value) = False) Then
                    Me._CausSBNatale = value
                End If
            End Set
        End Property

        Public Property CausSBPasqua() As System.Nullable(Of Integer)
            Get
                Return Me._CausSBPasqua
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausSBPasqua.Equals(value) = False) Then
                    Me._CausSBPasqua = value
                End If
            End Set
        End Property

        Public Property CausDDTDep() As System.Nullable(Of Integer)
            Get
                Return Me._CausDDTDep
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausDDTDep.Equals(value) = False) Then
                    Me._CausDDTDep = value
                End If
            End Set
        End Property

        Public Property CausVendDep() As System.Nullable(Of Integer)
            Get
                Return Me._CausVendDep
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausVendDep.Equals(value) = False) Then
                    Me._CausVendDep = value
                End If
            End Set
        End Property

        Public Property CausResoDep() As System.Nullable(Of Integer)
            Get
                Return Me._CausResoDep
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausResoDep.Equals(value) = False) Then
                    Me._CausResoDep = value
                End If
            End Set
        End Property

        Public Property PercorsoStampaOrdini() As String
            Get
                Return Me._PercorsoStampaOrdini
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PercorsoStampaOrdini, value) = False) Then
                    Me._PercorsoStampaOrdini = value
                End If
            End Set
        End Property

        Public Property PercorsoStampaDDT() As String
            Get
                Return Me._PercorsoStampaDDT
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PercorsoStampaDDT, value) = False) Then
                    Me._PercorsoStampaDDT = value
                End If
            End Set
        End Property

        Public Property PercorsoStampaFatt() As String
            Get
                Return Me._PercorsoStampaFatt
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PercorsoStampaFatt, value) = False) Then
                    Me._PercorsoStampaFatt = value
                End If
            End Set
        End Property

        Public Property PercorsoStampaPrev() As String
            Get
                Return Me._PercorsoStampaPrev
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PercorsoStampaPrev, value) = False) Then
                    Me._PercorsoStampaPrev = value
                End If
            End Set
        End Property

        Public Property CausNCResi() As System.Nullable(Of Integer)
            Get
                Return Me._CausNCResi
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausNCResi.Equals(value) = False) Then
                    Me._CausNCResi = value
                End If
            End Set
        End Property

        Public Property CausNCAbbuono() As System.Nullable(Of Integer)
            Get
                Return Me._CausNCAbbuono
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausNCAbbuono.Equals(value) = False) Then
                    Me._CausNCAbbuono = value
                End If
            End Set
        End Property

        Public Property CausNCScontoOmesso() As System.Nullable(Of Integer)
            Get
                Return Me._CausNCScontoOmesso
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausNCScontoOmesso.Equals(value) = False) Then
                    Me._CausNCScontoOmesso = value
                End If
            End Set
        End Property

        Public Property CausNCDiffPrezzo() As System.Nullable(Of Integer)
            Get
                Return Me._CausNCDiffPrezzo
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausNCDiffPrezzo.Equals(value) = False) Then
                    Me._CausNCDiffPrezzo = value
                End If
            End Set
        End Property

        Public Property UltAgRicalcolato() As System.Nullable(Of Integer)
            Get
                Return Me._UltAgRicalcolato
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._UltAgRicalcolato.Equals(value) = False) Then
                    Me._UltAgRicalcolato = value
                End If
            End Set
        End Property

        Public Property CausNCSBPrec() As System.Nullable(Of Integer)
            Get
                Return Me._CausNCSBPrec
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausNCSBPrec.Equals(value) = False) Then
                    Me._CausNCSBPrec = value
                End If
            End Set
        End Property

        Public Property NumeroBC() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroBC
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroBC.Equals(value) = False) Then
                    Me._NumeroBC = value
                End If
            End Set
        End Property

        Public Property DueCopieNZ() As Boolean
            Get
                Return Me._DueCopieNZ
            End Get
            Set(ByVal value As Boolean)
                If ((Me._DueCopieNZ = value) _
                   = False) Then
                    Me._DueCopieNZ = value
                End If
            End Set
        End Property

        Public Property CausRimInizialeDep() As System.Nullable(Of Integer)
            Get
                Return Me._CausRimInizialeDep
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausRimInizialeDep.Equals(value) = False) Then
                    Me._CausRimInizialeDep = value
                End If
            End Set
        End Property

        Public Property ComunicazioneRintracciabilita() As String
            Get
                Return Me._ComunicazioneRintracciabilita
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ComunicazioneRintracciabilita, value) = False) Then
                    Me._ComunicazioneRintracciabilita = value
                End If
            End Set
        End Property

        Public Property PieDiPaginaRintracciabilita() As String
            Get
                Return Me._PieDiPaginaRintracciabilita
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PieDiPaginaRintracciabilita, value) = False) Then
                    Me._PieDiPaginaRintracciabilita = value
                End If
            End Set
        End Property

        Public Property SMTPServer() As String
            Get
                Return Me._SMTPServer
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._SMTPServer, value) = False) Then
                    Me._SMTPServer = value
                End If
            End Set
        End Property

        Public Property SMTPPorta() As System.Nullable(Of Integer)
            Get
                Return Me._SMTPPorta
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._SMTPPorta.Equals(value) = False) Then
                    Me._SMTPPorta = value
                End If
            End Set
        End Property

        Public Property SMTPUserName() As String
            Get
                Return Me._SMTPUserName
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._SMTPUserName, value) = False) Then
                    Me._SMTPUserName = value
                End If
            End Set
        End Property

        Public Property SMTPPassword() As String
            Get
                Return Me._SMTPPassword
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._SMTPPassword, value) = False) Then
                    Me._SMTPPassword = value
                End If
            End Set
        End Property

        Public Property SMTPMailSender() As String
            Get
                Return Me._SMTPMailSender
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._SMTPMailSender, value) = False) Then
                    Me._SMTPMailSender = value
                End If
            End Set
        End Property

        Public Property DettaglioCestiDDT() As Boolean
            Get
                Return Me._DettaglioCestiDDT
            End Get
            Set(ByVal value As Boolean)
                If ((Me._DettaglioCestiDDT = value) _
                   = False) Then
                    Me._DettaglioCestiDDT = value
                End If
            End Set
        End Property

        Public Property NumeroOCL() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroOCL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroOCL.Equals(value) = False) Then
                    Me._NumeroOCL = value
                End If
            End Set
        End Property

        Public Property CausOrdCL() As System.Nullable(Of Integer)
            Get
                Return Me._CausOrdCL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausOrdCL.Equals(value) = False) Then
                    Me._CausOrdCL = value
                End If
            End Set
        End Property

        Public Property CausDDTCL() As System.Nullable(Of Integer)
            Get
                Return Me._CausDDTCL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausDDTCL.Equals(value) = False) Then
                    Me._CausDDTCL = value
                End If
            End Set
        End Property

        Public Property CausResoCL() As System.Nullable(Of Integer)
            Get
                Return Me._CausResoCL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausResoCL.Equals(value) = False) Then
                    Me._CausResoCL = value
                End If
            End Set
        End Property

        Public Property CausFineCL() As System.Nullable(Of Integer)
            Get
                Return Me._CausFineCL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausFineCL.Equals(value) = False) Then
                    Me._CausFineCL = value
                End If
            End Set
        End Property

        Public Property CausRestiCL() As System.Nullable(Of Integer)
            Get
                Return Me._CausRestiCL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausRestiCL.Equals(value) = False) Then
                    Me._CausRestiCL = value
                End If
            End Set
        End Property

        Public Property CausCarMagCL() As System.Nullable(Of Integer)
            Get
                Return Me._CausCarMagCL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CausCarMagCL.Equals(value) = False) Then
                    Me._CausCarMagCL = value
                End If
            End Set
        End Property

        Public Property ListinoCL() As System.Nullable(Of Integer)
            Get
                Return Me._ListinoCL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._ListinoCL.Equals(value) = False) Then
                    Me._ListinoCL = value
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

        Public Property CC() As String
            Get
                Return Me._CC
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CC, value) = False) Then
                    Me._CC = value
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

        Public Property PrezziDDT() As Boolean
            Get
                Return Me._PrezziDDT
            End Get
            Set(ByVal value As Boolean)
                If ((Me._PrezziDDT = value) _
                   = False) Then
                    Me._PrezziDDT = value
                End If
            End Set
        End Property

        Public Property NUltimiPrezziAcq() As System.Nullable(Of Integer)
            Get
                Return Me._NUltimiPrezziAcq
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NUltimiPrezziAcq.Equals(value) = False) Then
                    Me._NUltimiPrezziAcq = value
                End If
            End Set
        End Property

        Public Property Decimali_Grandezze() As System.Nullable(Of Integer)
            Get
                Return Me._Decimali_Grandezze
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Decimali_Grandezze.Equals(value) = False) Then
                    Me._Decimali_Grandezze = value
                End If
            End Set
        End Property

        Public Property NGG_Validita() As System.Nullable(Of Integer)
            Get
                Return Me._NGG_Validita
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NGG_Validita.Equals(value) = False) Then
                    Me._NGG_Validita = value
                End If
            End Set
        End Property
        Public Property NGG_Consegna() As System.Nullable(Of Integer)
            Get
                Return Me._NGG_Consegna
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NGG_Consegna.Equals(value) = False) Then
                    Me._NGG_Consegna = value
                End If
            End Set
        End Property
        'GIU240312
        Public Property NumeroFA() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroFA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroFA.Equals(value) = False) Then
                    Me._NumeroFA = value
                End If
            End Set
        End Property
        'GIU210714
        Public Property NumeroPA() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroPA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroPA.Equals(value) = False) Then
                    Me._NumeroPA = value
                End If
            End Set
        End Property
        'GIU220714
        Public Property NumeroNCPA() As System.Nullable(Of Integer)
            Get
                Return Me._NumeroNCPA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NumeroNCPA.Equals(value) = False) Then
                    Me._NumeroNCPA = value
                End If
            End Set
        End Property
        '-
        Public Property NumeroNCPASep() As Boolean
            Get
                Return Me._NumeroNCPASep
            End Get
            Set(ByVal value As Boolean)
                If ((Me._NumeroNCPASep = value) _
                   = False) Then
                    Me._NumeroNCPASep = value
                End If
            End Set
        End Property
        'giu300718
        Public Property SelAICatCli() As System.Nullable(Of Integer)
            Get
                Return Me._SelAICatCli
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._SelAICatCli.Equals(value) = False) Then
                    Me._SelAICatCli = value
                End If
            End Set
        End Property
        Public Property SelAIDaData() As System.Nullable(Of Integer)
            Get
                Return Me._SelAIDaData
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._SelAIDaData.Equals(value) = False) Then
                    Me._SelAIDaData = value
                End If
            End Set
        End Property
        Public Property SelAIAData() As System.Nullable(Of Integer)
            Get
                Return Me._SelAIAData
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._SelAIAData.Equals(value) = False) Then
                    Me._SelAIAData = value
                End If
            End Set
        End Property
        Public Property AIServizioEmail() As Boolean
            Get
                Return Me._AIServizioEmail
            End Get
            Set(ByVal value As Boolean)
                If ((Me._AIServizioEmail = value) _
                   = False) Then
                    Me._AIServizioEmail = value
                End If
            End Set
        End Property
        Public Property SelAIScGa() As Boolean
            Get
                Return Me._SelAIScGa
            End Get
            Set(ByVal value As Boolean)
                If ((Me._SelAIScGa = value) _
                   = False) Then
                    Me._SelAIScGa = value
                End If
            End Set
        End Property
        Public Property SelAIScEl() As Boolean
            Get
                Return Me._SelAIScEl
            End Get
            Set(ByVal value As Boolean)
                If ((Me._SelAIScEl = value) _
                   = False) Then
                    Me._SelAIScEl = value
                End If
            End Set
        End Property
        Public Property SelAIScBa() As Boolean
            Get
                Return Me._SelAIScBa
            End Get
            Set(ByVal value As Boolean)
                If ((Me._SelAIScBa = value) _
                   = False) Then
                    Me._SelAIScBa = value
                End If
            End Set
        End Property
        Public Property ScCassaDett() As Boolean
            Get
                Return Me._ScCassaDett
            End Get
            Set(ByVal value As Boolean)
                If ((Me._ScCassaDett = value) _
                   = False) Then
                    Me._ScCassaDett = value
                End If
            End Set
        End Property
        'giu270219
        Public Property ImpMinBollo() As System.Nullable(Of Decimal)
            Get
                Return Me._ImpMinBollo
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._ImpMinBollo.Equals(value) = False) Then
                    Me._ImpMinBollo = value
                End If
            End Set
        End Property
        Public Property IVABollo() As System.Nullable(Of Integer)
            Get
                Return Me._IVABollo
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IVABollo.Equals(value) = False) Then
                    Me._IVABollo = value
                End If
            End Set
        End Property
        Public Property IVAScMerce() As System.Nullable(Of Integer)
            Get
                Return Me._IVAScMerce
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IVAScMerce.Equals(value) = False) Then
                    Me._IVAScMerce = value
                End If
            End Set
        End Property
        Public Property ContoRitAcconto() As String
            Get
                Return Me._ContoRitAcconto
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._ContoRitAcconto, value) = False) Then
                    Me._ContoRitAcconto = value
                End If
            End Set
        End Property
        Public Property Bollo() As System.Nullable(Of Decimal)
            Get
                Return Me._Bollo
            End Get
            Set(ByVal value As System.Nullable(Of Decimal))
                If (Me._Bollo.Equals(value) = False) Then
                    Me._Bollo = value
                End If
            End Set
        End Property
        Public Property AIServizioEmailAttiva() As String
            Get
                Return Me._AIServizioEmailAttiva
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._AIServizioEmailAttiva, value) = False) Then
                    Me._AIServizioEmailAttiva = value
                End If
            End Set
        End Property
    End Class
End Namespace
