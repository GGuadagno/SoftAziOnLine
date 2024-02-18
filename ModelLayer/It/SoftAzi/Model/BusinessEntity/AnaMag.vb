Option Strict On
Option Explicit On

Namespace It.SoftAzi.Model.Entity
    Public Class AnaMagEntity
        Dim mCodArticolo As String
        Dim mCodBarre As String
        Dim mTipoCodice As String
        Dim mDescrizione As String
        Dim mUm As String
        Dim mCategoria As String
        Dim mLinea As String
        Dim mCodiceFornitore As String
        Dim mSottoscorta As Decimal
        Dim mCodiceDelFornitore As String
        Dim mCodImba1 As String
        Dim mCodImba2 As String
        Dim mAutore As String
        Dim mCodiceIntra As String
        Dim mCodArticoloEAN As String
        Dim mCodAziendaEAN As String
        Dim mCodControlloEAN As String
        Dim mGiacenza As Decimal
        Dim mOrdClienti As Decimal
        Dim mOrdFornit As Decimal
        Dim mPesoUnitario As Decimal
        Dim mLunghezza As Decimal
        Dim mLarghezza As Decimal
        Dim mAltezza As Decimal
        Dim mProdotto As Decimal
        Dim mConfezionato As Decimal
        Dim mOrdinato As Decimal
        Dim mVenduto As Decimal
        Dim mRicarico As Decimal
        Dim mQtaOrdine As Decimal
        Dim mTipoArticolo As Integer
        Dim mNConfTipo1 As Integer
        Dim mNConfTipo2 As Integer
        Dim mGiorniConsegna As Integer
        Dim mCodPagamento As Integer
        Dim mCodPeso As Integer
        Dim mCodIva As Integer
        Dim mConfezione As Integer
        Dim mReparto As Integer
        Dim mScaffale As Integer
        Dim mPiano As Integer
        Dim mGestLotti As Boolean
        Dim mAvvisaSottoscorta As Boolean
        Dim mArticoloDiVendita As Boolean
        Dim mDataInizioProd As Date
        Dim mDataFineProd As Date
        Dim mPrezzoAcquisto As Decimal
        Dim mDataAcquisto As Date
        'giu031111
        Dim mLBase As Integer
        Dim mLOpz As Integer
        'giu070414
        Dim mNAnniGaranzia As Integer
        Dim mNAnniScadElettrodi As Integer
        Dim mNAnniScadBatterie As Integer
        '---------
        Dim mScFornitore As Decimal 'giu070115
        'giu260618
        Dim mIDModulo1 As Integer
        Dim mIDModulo2 As Integer
        Dim mIDModulo3 As Integer
        Dim mIDModulo4 As Integer

        Property CodArticolo() As String
            Get
                Return mCodArticolo
            End Get
            Set(ByVal value As String)
                mCodArticolo = value
            End Set
        End Property
        Property CodBarre() As String
            Get
                Return mCodBarre
            End Get
            Set(ByVal value As String)
                mCodBarre = value
            End Set
        End Property
        Property TipoCodice() As String
            Get
                Return mTipoCodice
            End Get
            Set(ByVal value As String)
                mTipoCodice = value
            End Set
        End Property
        Property Descrizione() As String
            Get
                Return mDescrizione
            End Get
            Set(ByVal value As String)
                mDescrizione = value
            End Set
        End Property
        Property CodIva() As Integer
            Get
                Return mCodIva
            End Get
            Set(ByVal value As Integer)
                mCodIva = value
            End Set
        End Property
        Property Um() As String
            Get
                Return mUm
            End Get
            Set(ByVal value As String)
                mUm = value
            End Set
        End Property
        Property Giacenza() As Decimal
            Get
                Return mGiacenza
            End Get
            Set(ByVal value As Decimal)
                mGiacenza = value
            End Set
        End Property
        Property OrdClienti() As Decimal
            Get
                Return mOrdClienti
            End Get
            Set(ByVal value As Decimal)
                mOrdClienti = value
            End Set
        End Property
        Property OrdFornit() As Decimal
            Get
                Return OrdFornit
            End Get
            Set(ByVal value As Decimal)
                mOrdFornit = value
            End Set
        End Property
        Property PesoUnitario() As Decimal
            Get
                Return mPesoUnitario
            End Get
            Set(ByVal value As Decimal)
                mPesoUnitario = value
            End Set
        End Property
        Property Confezione() As Integer
            Get
                Return mConfezione
            End Get
            Set(ByVal value As Integer)
                mConfezione = value
            End Set
        End Property
        Property Categoria() As String
            Get
                Return mCategoria
            End Get
            Set(ByVal value As String)
                mCategoria = value
            End Set
        End Property
        Property Linea() As String
            Get
                Return mLinea
            End Get
            Set(ByVal value As String)
                mLinea = value
            End Set
        End Property
        Property Reparto() As Integer
            Get
                Return mReparto
            End Get
            Set(ByVal value As Integer)
                mReparto = value
            End Set
        End Property
        Property Scaffale() As Integer
            Get
                Return mScaffale
            End Get
            Set(ByVal value As Integer)
                mScaffale = value
            End Set
        End Property
        Property Piano() As Integer
            Get
                Return mPiano
            End Get
            Set(ByVal value As Integer)
                mPiano = value
            End Set
        End Property
        Property GestLotti() As Boolean
            Get
                Return mGestLotti
            End Get
            Set(ByVal value As Boolean)
                mGestLotti = value
            End Set
        End Property
        Property Lunghezza() As Decimal
            Get
                Return mLunghezza
            End Get
            Set(ByVal value As Decimal)
                mLunghezza = value
            End Set
        End Property
        Property Larghezza() As Decimal
            Get
                Return mLarghezza
            End Get
            Set(ByVal value As Decimal)
                mLarghezza = value
            End Set
        End Property
        Property Altezza() As Decimal
            Get
                Return mAltezza
            End Get
            Set(ByVal value As Decimal)
                mAltezza = value
            End Set
        End Property
        Property CodiceFornitore() As String
            Get
                Return mCodiceFornitore
            End Get
            Set(ByVal value As String)
                mCodiceFornitore = value
            End Set
        End Property
        Property Sottoscorta() As Decimal
            Get
                Return mSottoscorta
            End Get
            Set(ByVal value As Decimal)
                mSottoscorta = value
            End Set
        End Property
        Property CodiceDelFornitore() As String
            Get
                Return mCodiceDelFornitore
            End Get
            Set(ByVal value As String)
                mCodiceDelFornitore = value
            End Set
        End Property
        Property TipoArticolo() As Integer
            Get
                Return mTipoArticolo
            End Get
            Set(ByVal value As Integer)
                mTipoArticolo = value
            End Set
        End Property
        Property NConfTipo1() As Integer
            Get
                Return mNConfTipo1
            End Get
            Set(ByVal value As Integer)
                mNConfTipo1 = value
            End Set
        End Property
        Property NConfTipo2() As Integer
            Get
                Return mNConfTipo2
            End Get
            Set(ByVal value As Integer)
                mNConfTipo2 = value
            End Set
        End Property
        Property CodImba1() As String
            Get
                Return mCodImba1
            End Get
            Set(ByVal value As String)
                mCodImba1 = value
            End Set
        End Property
        Property CodImba2() As String
            Get
                Return mCodImba2
            End Get
            Set(ByVal value As String)
                mCodImba2 = value
            End Set
        End Property
        Property Autore() As String
            Get
                Return mAutore
            End Get
            Set(ByVal value As String)
                mAutore = value
            End Set
        End Property
        Property CodiceIntra() As String
            Get
                Return mCodiceIntra
            End Get
            Set(ByVal value As String)
                mCodiceIntra = value
            End Set
        End Property
        Property Prodotto() As Decimal
            Get
                Return mProdotto
            End Get
            Set(ByVal value As Decimal)
                mProdotto = value
            End Set
        End Property
        Property Confezionato() As Decimal
            Get
                Return mConfezionato
            End Get
            Set(ByVal value As Decimal)
                mConfezionato = value
            End Set
        End Property
        Property Ordinato() As Decimal
            Get
                Return mOrdinato
            End Get
            Set(ByVal value As Decimal)
                mOrdinato = value
            End Set
        End Property
        Property Venduto() As Decimal
            Get
                Return mVenduto
            End Get
            Set(ByVal value As Decimal)
                mVenduto = value
            End Set
        End Property
        Property Ricarico() As Decimal
            Get
                Return mRicarico
            End Get
            Set(ByVal value As Decimal)
                mRicarico = value
            End Set
        End Property
        Property QtaOrdine() As Decimal
            Get
                Return mQtaOrdine
            End Get
            Set(ByVal value As Decimal)
                mQtaOrdine = value
            End Set
        End Property
        Property AvvisaSottoscorta() As Boolean
            Get
                Return mAvvisaSottoscorta
            End Get
            Set(ByVal value As Boolean)
                mAvvisaSottoscorta = value
            End Set
        End Property
        Property GiorniConsegna() As Integer
            Get
                Return mGiorniConsegna
            End Get
            Set(ByVal value As Integer)
                mGiorniConsegna = value
            End Set
        End Property
        Property CodPagamento() As Integer
            Get
                Return mCodPagamento
            End Get
            Set(ByVal value As Integer)
                mCodPagamento = value
            End Set
        End Property
        Property DataInizioProd() As Date
            Get
                Return mDataInizioProd
            End Get
            Set(ByVal value As Date)
                mDataInizioProd = value
            End Set
        End Property
        Property DataFineProd() As Date
            Get
                Return mDataFineProd
            End Get
            Set(ByVal value As Date)
                mDataFineProd = value
            End Set
        End Property
        Property CodPeso() As Integer
            Get
                Return mCodPeso
            End Get
            Set(ByVal value As Integer)
                mCodPeso = value
            End Set
        End Property
        Property ArticoloDiVendita() As Boolean
            Get
                Return mArticoloDiVendita
            End Get
            Set(ByVal value As Boolean)
                mArticoloDiVendita = value
            End Set
        End Property
        Property CodArticoloEAN() As String
            Get
                Return mCodArticoloEAN
            End Get
            Set(ByVal value As String)
                mCodArticoloEAN = value
            End Set
        End Property
        Property CodAziendaEAN() As String
            Get
                Return mCodAziendaEAN
            End Get
            Set(ByVal value As String)
                mCodAziendaEAN = value
            End Set
        End Property
        Property CodControlloEAN() As String
            Get
                Return mCodControlloEAN
            End Get
            Set(ByVal value As String)
                mCodControlloEAN = value
            End Set
        End Property
        Property PrezzoAcquisto() As Decimal
            Get
                Return mPrezzoAcquisto
            End Get
            Set(ByVal value As Decimal)
                mPrezzoAcquisto = value
            End Set
        End Property
        Property DataAcquisto() As Date
            Get
                Return mDataAcquisto
            End Get
            Set(ByVal value As Date)
                mDataAcquisto = value
            End Set
        End Property
        Property LBase() As Integer
            Get
                Return mLBase
            End Get
            Set(ByVal value As Integer)
                mLBase = value
            End Set
        End Property
        Property LOpz() As Integer
            Get
                Return mLOpz
            End Get
            Set(ByVal value As Integer)
                mLOpz = value
            End Set
        End Property
        'giu070414
        Property NAnniGaranzia() As Integer
            Get
                Return mNAnniGaranzia
            End Get
            Set(ByVal value As Integer)
                mNAnniGaranzia = value
            End Set
        End Property
        Property NAnniScadElettrodi() As Integer
            Get
                Return mNAnniScadElettrodi
            End Get
            Set(ByVal value As Integer)
                mNAnniScadElettrodi = value
            End Set
        End Property
        Property NAnniScadBatterie() As Integer
            Get
                Return mNAnniScadBatterie
            End Get
            Set(ByVal value As Integer)
                mNAnniScadBatterie = value
            End Set
        End Property
        'giu070115
        Property ScFornitore() As Decimal
            Get
                Return mScFornitore
            End Get
            Set(ByVal value As Decimal)
                mScFornitore = value
            End Set
        End Property
        'giu260618
        Property IDModulo1() As Integer
            Get
                Return mIDModulo1
            End Get
            Set(ByVal value As Integer)
                mIDModulo1 = value
            End Set
        End Property
        Property IDModulo2() As Integer
            Get
                Return mIDModulo2
            End Get
            Set(ByVal value As Integer)
                mIDModulo2 = value
            End Set
        End Property
        Property IDModulo3() As Integer
            Get
                Return mIDModulo3
            End Get
            Set(ByVal value As Integer)
                mIDModulo3 = value
            End Set
        End Property
        Property IDModulo4() As Integer
            Get
                Return mIDModulo4
            End Get
            Set(ByVal value As Integer)
                mIDModulo4 = value
            End Set
        End Property
    End Class
End Namespace
