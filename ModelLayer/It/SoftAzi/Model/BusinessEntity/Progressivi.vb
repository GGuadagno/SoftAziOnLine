Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class ProgressiviEntity

        Private _Effetti As System.Nullable(Of Integer)

        Private _Distinte As System.Nullable(Of Integer)

        Private _Ragg_Bil_Clienti As System.Nullable(Of Integer)

        Private _Ragg_Bil_Fornit As System.Nullable(Of Integer)

        Private _Bilancio_Apertura As String

        Private _Raggr_Clienti As String

        Private _Raggr_Fornitori As String

        Private _Data_Apertura As System.Nullable(Of Date)

        Private _Chiusura_Patrimoniale As String

        Private _Chiusura_Economico As String

        Private _Data_Chiusura As System.Nullable(Of Date)

        Private _Utile_Esercizio As String

        Private _Perdita_Esercizio As String

        Private _Ditta As String

        Private _IC_Num As System.Nullable(Of Integer)

        Private _IC_Sez As System.Nullable(Of Integer)

        Private _IC_Rinum As System.Nullable(Of Integer)

        Private _Valuta_1 As String

        Private _Valuta_2 As String

        Private _Valuta_Dec_1 As System.Nullable(Of Integer)

        Private _Valuta_Dec_2 As System.Nullable(Of Integer)

        Private _Cambio_Fisso_2 As System.Nullable(Of Integer)

        Private _Valuta_Mod_1 As System.Nullable(Of Integer)

        Private _Valuta_Mod_2 As System.Nullable(Of Integer)

        Private _Valuta_Old_1 As String

        Private _Valuta_Old_2 As String

        Private _Perdita_Cambi As String

        Private _Utile_Cambi As String

        Private _TestoECC As String

        Private _PiedeECC As String

        Private _Campi_TDL As System.Nullable(Of Integer)

        Private _ProRata_P As System.Nullable(Of Integer)

        Private _ProRata_D As System.Nullable(Of Integer)

        Private _ProRata As System.Nullable(Of Integer)

        Private _Ragg_Bil_Fornit_RA As System.Nullable(Of Integer)

        Private _IVA_Corrispettivi As System.Nullable(Of Integer)

        Private _CentriDiCosto As System.Nullable(Of Integer)

        Private _IVA_Autotrasporti As String

        Private _Ricavi_Autotrasporti As String

        Private _Aliquota_Autotrasporti As System.Nullable(Of Integer)

        Private _Data_Compattazione As System.Nullable(Of Date)

        Private _Dimensione As System.Nullable(Of Integer)

        Private _PrimaNotaInFattura As System.Nullable(Of Integer)

        Private _NoteVariazione As System.Nullable(Of Integer)

        Private _StampaProtCogeInRegIVA As System.Nullable(Of Integer)

        Private _Reg1 As String

        Private _Reg2 As String

        Private _TitoloRespBilancio As String

        Private _PersonaRespBilancio As String

        Private _CapitaleSociale As String

        Private _CodAziendaRB As String

        Private _UltimaPagRiepIVA As System.Nullable(Of Integer)

        Private _BloccoCEE As System.Nullable(Of Integer)

        Private _CentriDiRicavo As System.Nullable(Of Integer)

        Private _TipoCO As System.Nullable(Of Integer)

        Private _IFFE As System.Nullable(Of Integer)

        Private _RegCdCR As System.Nullable(Of Integer)

        Private _SWMaggRIVA As System.Nullable(Of Integer)

        Private _PercMaggRIVA As System.Nullable(Of Integer)

        Private _RACCosto As String

        Private _RACCostoCP As String

        Private _RACErarioCRit As String

        Private _RAGCErarioCRit As String

        Private _RAPerc As System.Nullable(Of Integer)

        Private _RAIVA As System.Nullable(Of Integer)

        Private _RACausFT As System.Nullable(Of Integer)

        Private _RACausRA As System.Nullable(Of Integer)

        Private _CodTributo As System.Nullable(Of Integer)

        Private _RAPercImp As System.Nullable(Of Integer)

        Private _RAPercEnasarco As System.Nullable(Of Integer)

        Private _RAPercCP As System.Nullable(Of Integer)

        Private _CEnasarco As String

        Private _IntraIVACredito As String

        Private _IntraIVADebito As String

        Private _IntraCosto As String

        Private _IntraRicavo As String

        Private _SWRagSoc25 As Integer

        Private _SezCO As System.Nullable(Of Boolean)

        Private _TestoECC_RiBa As String

        Private _PiedeECC_RiBa As String

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property Effetti() As System.Nullable(Of Integer)
            Get
                Return Me._Effetti
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Effetti.Equals(value) = False) Then
                    Me._Effetti = value
                End If
            End Set
        End Property

        Public Property Distinte() As System.Nullable(Of Integer)
            Get
                Return Me._Distinte
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Distinte.Equals(value) = False) Then
                    Me._Distinte = value
                End If
            End Set
        End Property

        Public Property Ragg_Bil_Clienti() As System.Nullable(Of Integer)
            Get
                Return Me._Ragg_Bil_Clienti
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Ragg_Bil_Clienti.Equals(value) = False) Then
                    Me._Ragg_Bil_Clienti = value
                End If
            End Set
        End Property

        Public Property Ragg_Bil_Fornit() As System.Nullable(Of Integer)
            Get
                Return Me._Ragg_Bil_Fornit
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Ragg_Bil_Fornit.Equals(value) = False) Then
                    Me._Ragg_Bil_Fornit = value
                End If
            End Set
        End Property

        Public Property Bilancio_Apertura() As String
            Get
                Return Me._Bilancio_Apertura
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Bilancio_Apertura, value) = False) Then
                    Me._Bilancio_Apertura = value
                End If
            End Set
        End Property

        Public Property Raggr_Clienti() As String
            Get
                Return Me._Raggr_Clienti
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Raggr_Clienti, value) = False) Then
                    Me._Raggr_Clienti = value
                End If
            End Set
        End Property

        Public Property Raggr_Fornitori() As String
            Get
                Return Me._Raggr_Fornitori
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Raggr_Fornitori, value) = False) Then
                    Me._Raggr_Fornitori = value
                End If
            End Set
        End Property

        Public Property Data_Apertura() As System.Nullable(Of Date)
            Get
                Return Me._Data_Apertura
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._Data_Apertura.Equals(value) = False) Then
                    Me._Data_Apertura = value
                End If
            End Set
        End Property

        Public Property Chiusura_Patrimoniale() As String
            Get
                Return Me._Chiusura_Patrimoniale
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Chiusura_Patrimoniale, value) = False) Then
                    Me._Chiusura_Patrimoniale = value
                End If
            End Set
        End Property

        Public Property Chiusura_Economico() As String
            Get
                Return Me._Chiusura_Economico
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Chiusura_Economico, value) = False) Then
                    Me._Chiusura_Economico = value
                End If
            End Set
        End Property

        Public Property Data_Chiusura() As System.Nullable(Of Date)
            Get
                Return Me._Data_Chiusura
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._Data_Chiusura.Equals(value) = False) Then
                    Me._Data_Chiusura = value
                End If
            End Set
        End Property

        Public Property Utile_Esercizio() As String
            Get
                Return Me._Utile_Esercizio
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Utile_Esercizio, value) = False) Then
                    Me._Utile_Esercizio = value
                End If
            End Set
        End Property

        Public Property Perdita_Esercizio() As String
            Get
                Return Me._Perdita_Esercizio
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Perdita_Esercizio, value) = False) Then
                    Me._Perdita_Esercizio = value
                End If
            End Set
        End Property

        Public Property Ditta() As String
            Get
                Return Me._Ditta
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Ditta, value) = False) Then
                    Me._Ditta = value
                End If
            End Set
        End Property

        Public Property IC_Num() As System.Nullable(Of Integer)
            Get
                Return Me._IC_Num
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IC_Num.Equals(value) = False) Then
                    Me._IC_Num = value
                End If
            End Set
        End Property

        Public Property IC_Sez() As System.Nullable(Of Integer)
            Get
                Return Me._IC_Sez
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IC_Sez.Equals(value) = False) Then
                    Me._IC_Sez = value
                End If
            End Set
        End Property

        Public Property IC_Rinum() As System.Nullable(Of Integer)
            Get
                Return Me._IC_Rinum
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IC_Rinum.Equals(value) = False) Then
                    Me._IC_Rinum = value
                End If
            End Set
        End Property

        Public Property Valuta_1() As String
            Get
                Return Me._Valuta_1
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Valuta_1, value) = False) Then
                    Me._Valuta_1 = value
                End If
            End Set
        End Property

        Public Property Valuta_2() As String
            Get
                Return Me._Valuta_2
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Valuta_2, value) = False) Then
                    Me._Valuta_2 = value
                End If
            End Set
        End Property

        Public Property Valuta_Dec_1() As System.Nullable(Of Integer)
            Get
                Return Me._Valuta_Dec_1
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Valuta_Dec_1.Equals(value) = False) Then
                    Me._Valuta_Dec_1 = value
                End If
            End Set
        End Property

        Public Property Valuta_Dec_2() As System.Nullable(Of Integer)
            Get
                Return Me._Valuta_Dec_2
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Valuta_Dec_2.Equals(value) = False) Then
                    Me._Valuta_Dec_2 = value
                End If
            End Set
        End Property

        Public Property Cambio_Fisso_2() As System.Nullable(Of Integer)
            Get
                Return Me._Cambio_Fisso_2
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Cambio_Fisso_2.Equals(value) = False) Then
                    Me._Cambio_Fisso_2 = value
                End If
            End Set
        End Property

        Public Property Valuta_Mod_1() As System.Nullable(Of Integer)
            Get
                Return Me._Valuta_Mod_1
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Valuta_Mod_1.Equals(value) = False) Then
                    Me._Valuta_Mod_1 = value
                End If
            End Set
        End Property

        Public Property Valuta_Mod_2() As System.Nullable(Of Integer)
            Get
                Return Me._Valuta_Mod_2
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Valuta_Mod_2.Equals(value) = False) Then
                    Me._Valuta_Mod_2 = value
                End If
            End Set
        End Property

        Public Property Valuta_Old_1() As String
            Get
                Return Me._Valuta_Old_1
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Valuta_Old_1, value) = False) Then
                    Me._Valuta_Old_1 = value
                End If
            End Set
        End Property

        Public Property Valuta_Old_2() As String
            Get
                Return Me._Valuta_Old_2
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Valuta_Old_2, value) = False) Then
                    Me._Valuta_Old_2 = value
                End If
            End Set
        End Property

        Public Property Perdita_Cambi() As String
            Get
                Return Me._Perdita_Cambi
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Perdita_Cambi, value) = False) Then
                    Me._Perdita_Cambi = value
                End If
            End Set
        End Property

        Public Property Utile_Cambi() As String
            Get
                Return Me._Utile_Cambi
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Utile_Cambi, value) = False) Then
                    Me._Utile_Cambi = value
                End If
            End Set
        End Property

        Public Property TestoECC() As String
            Get
                Return Me._TestoECC
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._TestoECC, value) = False) Then
                    Me._TestoECC = value
                End If
            End Set
        End Property

        Public Property PiedeECC() As String
            Get
                Return Me._PiedeECC
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PiedeECC, value) = False) Then
                    Me._PiedeECC = value
                End If
            End Set
        End Property

        Public Property Campi_TDL() As System.Nullable(Of Integer)
            Get
                Return Me._Campi_TDL
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Campi_TDL.Equals(value) = False) Then
                    Me._Campi_TDL = value
                End If
            End Set
        End Property

        Public Property ProRata_P() As System.Nullable(Of Integer)
            Get
                Return Me._ProRata_P
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._ProRata_P.Equals(value) = False) Then
                    Me._ProRata_P = value
                End If
            End Set
        End Property

        Public Property ProRata_D() As System.Nullable(Of Integer)
            Get
                Return Me._ProRata_D
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._ProRata_D.Equals(value) = False) Then
                    Me._ProRata_D = value
                End If
            End Set
        End Property

        Public Property ProRata() As System.Nullable(Of Integer)
            Get
                Return Me._ProRata
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._ProRata.Equals(value) = False) Then
                    Me._ProRata = value
                End If
            End Set
        End Property

        Public Property Ragg_Bil_Fornit_RA() As System.Nullable(Of Integer)
            Get
                Return Me._Ragg_Bil_Fornit_RA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Ragg_Bil_Fornit_RA.Equals(value) = False) Then
                    Me._Ragg_Bil_Fornit_RA = value
                End If
            End Set
        End Property

        Public Property IVA_Corrispettivi() As System.Nullable(Of Integer)
            Get
                Return Me._IVA_Corrispettivi
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IVA_Corrispettivi.Equals(value) = False) Then
                    Me._IVA_Corrispettivi = value
                End If
            End Set
        End Property

        Public Property CentriDiCosto() As System.Nullable(Of Integer)
            Get
                Return Me._CentriDiCosto
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CentriDiCosto.Equals(value) = False) Then
                    Me._CentriDiCosto = value
                End If
            End Set
        End Property

        Public Property IVA_Autotrasporti() As String
            Get
                Return Me._IVA_Autotrasporti
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._IVA_Autotrasporti, value) = False) Then
                    Me._IVA_Autotrasporti = value
                End If
            End Set
        End Property

        Public Property Ricavi_Autotrasporti() As String
            Get
                Return Me._Ricavi_Autotrasporti
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Ricavi_Autotrasporti, value) = False) Then
                    Me._Ricavi_Autotrasporti = value
                End If
            End Set
        End Property

        Public Property Aliquota_Autotrasporti() As System.Nullable(Of Integer)
            Get
                Return Me._Aliquota_Autotrasporti
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Aliquota_Autotrasporti.Equals(value) = False) Then
                    Me._Aliquota_Autotrasporti = value
                End If
            End Set
        End Property

        Public Property Data_Compattazione() As System.Nullable(Of Date)
            Get
                Return Me._Data_Compattazione
            End Get
            Set(ByVal value As System.Nullable(Of Date))
                If (Me._Data_Compattazione.Equals(value) = False) Then
                    Me._Data_Compattazione = value
                End If
            End Set
        End Property

        Public Property Dimensione() As System.Nullable(Of Integer)
            Get
                Return Me._Dimensione
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._Dimensione.Equals(value) = False) Then
                    Me._Dimensione = value
                End If
            End Set
        End Property

        Public Property PrimaNotaInFattura() As System.Nullable(Of Integer)
            Get
                Return Me._PrimaNotaInFattura
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._PrimaNotaInFattura.Equals(value) = False) Then
                    Me._PrimaNotaInFattura = value
                End If
            End Set
        End Property

        Public Property NoteVariazione() As System.Nullable(Of Integer)
            Get
                Return Me._NoteVariazione
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._NoteVariazione.Equals(value) = False) Then
                    Me._NoteVariazione = value
                End If
            End Set
        End Property

        Public Property StampaProtCogeInRegIVA() As System.Nullable(Of Integer)
            Get
                Return Me._StampaProtCogeInRegIVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._StampaProtCogeInRegIVA.Equals(value) = False) Then
                    Me._StampaProtCogeInRegIVA = value
                End If
            End Set
        End Property

        Public Property Reg1() As String
            Get
                Return Me._Reg1
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Reg1, value) = False) Then
                    Me._Reg1 = value
                End If
            End Set
        End Property

        Public Property Reg2() As String
            Get
                Return Me._Reg2
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._Reg2, value) = False) Then
                    Me._Reg2 = value
                End If
            End Set
        End Property

        Public Property TitoloRespBilancio() As String
            Get
                Return Me._TitoloRespBilancio
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._TitoloRespBilancio, value) = False) Then
                    Me._TitoloRespBilancio = value
                End If
            End Set
        End Property

        Public Property PersonaRespBilancio() As String
            Get
                Return Me._PersonaRespBilancio
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PersonaRespBilancio, value) = False) Then
                    Me._PersonaRespBilancio = value
                End If
            End Set
        End Property

        Public Property CapitaleSociale() As String
            Get
                Return Me._CapitaleSociale
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CapitaleSociale, value) = False) Then
                    Me._CapitaleSociale = value
                End If
            End Set
        End Property

        Public Property CodAziendaRB() As String
            Get
                Return Me._CodAziendaRB
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CodAziendaRB, value) = False) Then
                    Me._CodAziendaRB = value
                End If
            End Set
        End Property

        Public Property UltimaPagRiepIVA() As System.Nullable(Of Integer)
            Get
                Return Me._UltimaPagRiepIVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._UltimaPagRiepIVA.Equals(value) = False) Then
                    Me._UltimaPagRiepIVA = value
                End If
            End Set
        End Property

        Public Property BloccoCEE() As System.Nullable(Of Integer)
            Get
                Return Me._BloccoCEE
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._BloccoCEE.Equals(value) = False) Then
                    Me._BloccoCEE = value
                End If
            End Set
        End Property

        Public Property CentriDiRicavo() As System.Nullable(Of Integer)
            Get
                Return Me._CentriDiRicavo
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._CentriDiRicavo.Equals(value) = False) Then
                    Me._CentriDiRicavo = value
                End If
            End Set
        End Property

        Public Property TipoCO() As System.Nullable(Of Integer)
            Get
                Return Me._TipoCO
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._TipoCO.Equals(value) = False) Then
                    Me._TipoCO = value
                End If
            End Set
        End Property

        Public Property IFFE() As System.Nullable(Of Integer)
            Get
                Return Me._IFFE
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._IFFE.Equals(value) = False) Then
                    Me._IFFE = value
                End If
            End Set
        End Property

        Public Property RegCdCR() As System.Nullable(Of Integer)
            Get
                Return Me._RegCdCR
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RegCdCR.Equals(value) = False) Then
                    Me._RegCdCR = value
                End If
            End Set
        End Property

        Public Property SWMaggRIVA() As System.Nullable(Of Integer)
            Get
                Return Me._SWMaggRIVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._SWMaggRIVA.Equals(value) = False) Then
                    Me._SWMaggRIVA = value
                End If
            End Set
        End Property

        Public Property PercMaggRIVA() As System.Nullable(Of Integer)
            Get
                Return Me._PercMaggRIVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._PercMaggRIVA.Equals(value) = False) Then
                    Me._PercMaggRIVA = value
                End If
            End Set
        End Property

        Public Property RACCosto() As String
            Get
                Return Me._RACCosto
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._RACCosto, value) = False) Then
                    Me._RACCosto = value
                End If
            End Set
        End Property

        Public Property RACCostoCP() As String
            Get
                Return Me._RACCostoCP
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._RACCostoCP, value) = False) Then
                    Me._RACCostoCP = value
                End If
            End Set
        End Property

        Public Property RACErarioCRit() As String
            Get
                Return Me._RACErarioCRit
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._RACErarioCRit, value) = False) Then
                    Me._RACErarioCRit = value
                End If
            End Set
        End Property

        Public Property RAGCErarioCRit() As String
            Get
                Return Me._RAGCErarioCRit
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._RAGCErarioCRit, value) = False) Then
                    Me._RAGCErarioCRit = value
                End If
            End Set
        End Property

        Public Property RAPerc() As System.Nullable(Of Integer)
            Get
                Return Me._RAPerc
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RAPerc.Equals(value) = False) Then
                    Me._RAPerc = value
                End If
            End Set
        End Property

        Public Property RAIVA() As System.Nullable(Of Integer)
            Get
                Return Me._RAIVA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RAIVA.Equals(value) = False) Then
                    Me._RAIVA = value
                End If
            End Set
        End Property

        Public Property RACausFT() As System.Nullable(Of Integer)
            Get
                Return Me._RACausFT
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RACausFT.Equals(value) = False) Then
                    Me._RACausFT = value
                End If
            End Set
        End Property

        Public Property RACausRA() As System.Nullable(Of Integer)
            Get
                Return Me._RACausRA
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RACausRA.Equals(value) = False) Then
                    Me._RACausRA = value
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

        Public Property RAPercImp() As System.Nullable(Of Integer)
            Get
                Return Me._RAPercImp
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RAPercImp.Equals(value) = False) Then
                    Me._RAPercImp = value
                End If
            End Set
        End Property

        Public Property RAPercEnasarco() As System.Nullable(Of Integer)
            Get
                Return Me._RAPercEnasarco
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RAPercEnasarco.Equals(value) = False) Then
                    Me._RAPercEnasarco = value
                End If
            End Set
        End Property

        Public Property RAPercCP() As System.Nullable(Of Integer)
            Get
                Return Me._RAPercCP
            End Get
            Set(ByVal value As System.Nullable(Of Integer))
                If (Me._RAPercCP.Equals(value) = False) Then
                    Me._RAPercCP = value
                End If
            End Set
        End Property

        Public Property CEnasarco() As String
            Get
                Return Me._CEnasarco
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._CEnasarco, value) = False) Then
                    Me._CEnasarco = value
                End If
            End Set
        End Property

        Public Property IntraIVACredito() As String
            Get
                Return Me._IntraIVACredito
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._IntraIVACredito, value) = False) Then
                    Me._IntraIVACredito = value
                End If
            End Set
        End Property

        Public Property IntraIVADebito() As String
            Get
                Return Me._IntraIVADebito
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._IntraIVADebito, value) = False) Then
                    Me._IntraIVADebito = value
                End If
            End Set
        End Property

        Public Property IntraCosto() As String
            Get
                Return Me._IntraCosto
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._IntraCosto, value) = False) Then
                    Me._IntraCosto = value
                End If
            End Set
        End Property

        Public Property IntraRicavo() As String
            Get
                Return Me._IntraRicavo
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._IntraRicavo, value) = False) Then
                    Me._IntraRicavo = value
                End If
            End Set
        End Property

        Public Property SWRagSoc25() As Integer
            Get
                Return Me._SWRagSoc25
            End Get
            Set(ByVal value As Integer)
                If ((Me._SWRagSoc25 = value) _
                   = False) Then
                    Me._SWRagSoc25 = value
                End If
            End Set
        End Property

        Public Property SezCO() As System.Nullable(Of Boolean)
            Get
                Return Me._SezCO
            End Get
            Set(ByVal value As System.Nullable(Of Boolean))
                If (Me._SezCO.Equals(value) = False) Then
                    Me._SezCO = value
                End If
            End Set
        End Property

        Public Property TestoECC_RiBa() As String
            Get
                Return Me._TestoECC_RiBa
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._TestoECC_RiBa, value) = False) Then
                    Me._TestoECC_RiBa = value
                End If
            End Set
        End Property

        Public Property PiedeECC_RiBa() As String
            Get
                Return Me._PiedeECC_RiBa
            End Get
            Set(ByVal value As String)
                If (String.Equals(Me._PiedeECC_RiBa, value) = False) Then
                    Me._PiedeECC_RiBa = value
                End If
            End Set
        End Property

    End Class
End Namespace