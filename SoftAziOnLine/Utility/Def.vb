Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework

Public Class Def
    'giu22022012 se aggiungi un nuovo tipo modificare anche in GLOBAL ASAX dove carica l'arrei logonutente con tutti i tipi qui sotto indicati
    Public Const CSTUFFICIO_AMMINISTRATIVO As String = "Ufficio Amministrativo" 'U
    Public Const CSTAMMINISTRATORE As String = "Amministratore" 'A
    Public Const CSTTECNICO As String = "Tecnico" 'T
    Public Const CSTMAGAZZINO As String = "Magazzino" 'M 'giu250112
    'giu2202012 Richiesta SFERA (Davide b.)
    Public Const CSTACQUISTI As String = "Ufficio Acquisti" 'Q 
    Public Const CSTVENDITE As String = "Manutenzione Contratti" 'V giu080620 ORA USATO PER LA GESTIONE CONTRATTI XKE' NON USATO DA NESSUNO
    'GIU200212 MENU E GESTIONE TABELLE
    Public Const CSTVISUALMENU As String = "VisualizzaMenu"
    Public Const CALLGESTIONE As String = "CALLGESTIONE" 'giu030323 quando ritorna da gestione non azzera la sessione dei dati ricercati da elenco
    '---------------------------------
    'Campo da creare in ParametriGeneraliAzi
    'Struttura codice Articolo: se è uguale a tutti 0 funzione come nella versione originale
    'Altrimenti 000000 è il BASE e 111 Opzione: la ricerca in questo caso non sara' di 100 in 100 nel popup
    'ma ci sara' un combo che prima seleziona il BASE e poi nella Grid si selezionano le Opzioni
    'giu031111 cambiato tutto adesso la lunghezza massima è di 20 mentre nel DB
    'ci sono 2 campi LBase e LOpz quando si tratta di una BASE sara valorizzato solo LBase mentre LOpz=0
    'La somma dei 2 e' la lunghezza del codice articolo
    'Ci possono essere articoli con base 12 e Opz
    ' ''Public Const MaskLevelArt As String = "000000111" '000000 BASE 111 OPZIONE
    Public Const CSTLOpz As Integer = 5 'giu231211 richiesta di Zibordi alemeno a 4
    'TRUE  selezionata la BASE Opzione è sempre obbligatoria
    'FALSE selezionata la BASE Opzione è facoltativa
    Public Const ObbOpzBase As Boolean = False
    'TRUE  selezionata una Opzione la BASE è sempre obbligatoria
    'FALSE selezionata una Opzione la BASE è opzionale
    Public Const ObbBaseOpz As Boolean = True

    Public Const LABELMESSAGERED As String = "LabelMessageRed" 'giu240322 messaggio in ROSSO
    Public Const HTML_SPAZIO As String = "&nbsp;"
    Public Const HTML_AND As String = "&amp;"
    Public Shared SEGNALA_KO As System.Drawing.Color = Drawing.Color.Red
    Public Shared SEGNALA_OK As System.Drawing.Color = Drawing.Color.White
    Public Shared SEGNALA_INFO As System.Drawing.Color = Drawing.Color.Green
    Public Shared SEGNALA_OKLBL As System.Drawing.Color = Drawing.Color.Silver
    Public Shared SEGNALA_OKBTN As System.Drawing.Color = Drawing.Color.Silver
    'se usiamo il FACADE nelle SP ci ritorna questa data per le DATE A NULL NEL DB
    Public Shared DATANULL As DateTime = CDate("01/01/1900")


#Region "Definizioni: formattazione DA USARE I VALORI DI ParametriGeneraliAzi"
    Public Const DecimaliValutaIntermedio As Integer = 4 'QUESTO VA BENE COME COSTANTE 
    Public Const FormatoValMag As String = "###,##0.0000" 'PREZZI PER MAGAZZINO E LISTINI
    Public Const FormatoSCMag As String = "##0.00" 'PREZZI PER MAGAZZINO E LISTINI
    Public Const FormatoValEuro As String = "###,##0.00" 'QUESTO VA BENE COME COSTANTE 
    Public Const FormatoData As String = "dd/MM/yyyy" 'QUESTO VA BENE COME COSTANTE 
    Public Const FormatoDataOra As String = "dd/MM/yyyy HH:mm" 'QUESTO VA BENE COME COSTANTE 
    Public Const FormatoOra As String = "HH:mm" 'QUESTO VA BENE COME COSTANTE 

    Public Enum RoundDataEnum
        InizioMese = 1
        FineMese = 2
    End Enum
    Public Shared Function RoundData(ByVal Data As Date, ByVal ArrotondaA As RoundDataEnum) As Date
        Dim Mese As Integer
        Dim Anno As Integer

        Mese = Month(Data)
        Anno = Year(Data)

        If ArrotondaA = RoundDataEnum.FineMese Then
            RoundData = DateAdd("d", -1, CDate("01/" & Mese + 1 & "/" & Anno))
        Else
            RoundData = CDate("01/" & Mese & "/" & Anno)
        End If

    End Function
#End Region

    Public Shared Function ConvalidaEmail(ByVal indirizzo As String) As Boolean
        Dim x1, x2
        indirizzo = indirizzo.Replace(",", ";") 'giu040119
        x1 = Split(indirizzo, ";")
        If UBound(x1) <= 0 Then 'giu040119
            ConvalidaEmail = OKConvalidaEmail(indirizzo.Trim)
        Else
            For Each x2 In x1
                If x2.ToString.Trim <> "" Then
                    ConvalidaEmail = OKConvalidaEmail(x2.ToString.Trim)
                End If
                If ConvalidaEmail = False Then
                    Exit Function
                End If
            Next
        End If
    End Function
    'GIU040322
    Public Shared Function ConvData(ByVal Data As String) As String

        Dim gg, mm, aa
        Dim separ1, separ2, separ3, sn_separ As Boolean

        separ1 = InStr(1, Data, "/")
        separ2 = InStr(1, Data, ".")
        separ3 = InStr(1, Data, ",")
        sn_separ = False

        If separ1 Then sn_separ = True
        If separ2 Then sn_separ = True
        If separ3 Then sn_separ = True

        'GIU121105
        '--- Solo il Giorno metto mese corrente e anno corrente MEGLIO GLESER Giu300606
        If (Len(Data) = 2 Or Len(Data) = 1) And IsNumeric(Data) Then
            gg = IIf(Len(Data) = 1, "0" & Data, Str(Data))
            If gg > 31 Or Val(gg) = 0 Then
                'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                ConvData = Data
                Exit Function
            End If
            mm = CStr(Format(Month(Now), "00"))
            aa = Str(Year(Now)) 'GIU040322 GLEser 'giu300606 Str(Year(Now))
            Data = Trim(gg) + "/" + Trim(mm) + "/" + Trim(aa)
            ConvData = Format(CDate(Data), FormatoData)
            Exit Function
        End If
        '--- Giorno + Mese
        If Len(Data) = 4 And IsNumeric(Data) Then
            gg = Trim$(Mid(Data, 1, 2))
            If gg > 31 Or Val(gg) = 0 Then
                'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                ConvData = Data
                Exit Function
            End If
            mm = Trim$(Mid(Data, 3, 2))
            If mm > 12 Or Val(mm) = 0 Then
                'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                ConvData = Data
                Exit Function
            End If
            aa = Str(Year(Now)) 'GIU040322 GLEser 'giu300606 Str(Year(Now))
            Data = Trim(gg) + "/" + Trim(mm) + "/" + Trim(aa)
            ConvData = Format(CDate(Data), FormatoData)
            Exit Function
        End If
        '---------
        '--- Giorno + Mese A UNA CIFRA o GIORNO A UNA CIFRA ?
        If Len(Data) = 3 And IsNumeric(Data) Then
            'MESE A UNA CIFRA GIORNO A 2 CIFRE
            gg = Trim$(Mid(Data, 1, 2))
            If gg > 31 Or Val(gg) = 0 Then
                'giu150706
                GoTo mm2gg1
                'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                ConvData = Data
                Exit Function
            End If
            mm = Trim$(Mid(Data, 3, 1))
            mm = "0" & mm
            If mm > 12 Or Val(mm) = 0 Then
                'giu150706
                GoTo mm2gg1
                'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                ConvData = Data
                Exit Function
            End If
            aa = Str(Year(Now)) 'GIU040322 GLEser 'giu300606 Str(Year(Now))
            Data = Trim(gg) + "/" + Trim(mm) + "/" + Trim(aa)
            ConvData = Format(CDate(Data), FormatoData)
            Exit Function
            'MESE A 2CIFRE GIORNO A 1 CIFRA giu150706
mm2gg1:
            gg = Trim$(Mid(Data, 1, 1))
            gg = "0" & gg
            If gg > 31 Or Val(gg) = 0 Then
                'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                ConvData = Data
                Exit Function
            End If
            mm = Trim$(Mid(Data, 2, 2))
            If mm > 12 Or Val(mm) = 0 Then
                'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                ConvData = Data
                Exit Function
            End If
            aa = Str(Year(Now)) 'GIU040322 GLEser 'giu300606 Str(Year(Now))
            Data = Trim(gg) + "/" + Trim(mm) + "/" + Trim(aa)
            ConvData = Format(CDate(Data), FormatoData)
            Exit Function

        End If
        '---------
        If Len(Data) = 6 Then
            If sn_separ = False Then
                gg = Trim$(Mid(Data, 1, 2))
                mm = Trim$(Mid(Data, 3, 2))
                aa = Trim$(Mid(Data, 5, 2))
                If mm > 12 Or Val(mm) = 0 Then
                    'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                    ConvData = Data
                    Exit Function
                End If
                Data = Trim(gg) + "/" + Trim(mm) + "/" + Trim(aa)
                ConvData = Format(CDate(Data), FormatoData)
                Exit Function
            End If
        End If

        If Len(Data) = 8 Then
            If sn_separ = False Then
                gg = Trim$(Mid(Data, 1, 2))
                mm = Trim$(Mid(Data, 3, 2))
                aa = Trim$(Mid(Data, 5, 4))
                If mm > 12 Or Val(mm) = 0 Then
                    'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                    ConvData = Data
                    Exit Function
                End If
                Data = Trim(gg) + "/" + Trim(mm) + "/" + Trim(aa)
                ConvData = Format(CDate(Data), FormatoData)
                Exit Function
            End If
        End If
        ConvData = Data

    End Function

    Public Shared Function OKConvalidaEmail(ByVal indirizzo As String) As Boolean
        indirizzo = indirizzo.Replace(" ", "") 'giu040119
        Dim valido
        valido = True
        Dim nome, parte, i, c
        nome = Split(indirizzo, "@")
        If UBound(nome) <> 1 Then
            valido = False
            Exit Function
        End If
        For Each parte In nome
            If Len(parte) <= 0 Then
                valido = False
                Exit Function
            End If
            For i = 1 To Len(parte)
                c = LCase(Mid(parte, i, 1))
                If InStr("abcdefghijklmnopqrstuvwxyz_-.", c) <= 0 And Not IsNumeric(c) Then
                    valido = False
                    Exit Function
                End If
            Next
            If Left(parte, 1) = "." Or Right(parte, 1) = "." Then
                valido = False
                Exit Function
            End If
        Next
        If InStr(nome(1), ".") <= 0 Then
            valido = False
            Exit Function
        End If
        i = Len(nome(1)) - InStrRev(nome(1), ".")
        If i <> 2 And i <> 3 And i <> 4 And i <> 5 Then 'giu240124
            valido = False
            Exit Function
        End If
        If InStr(indirizzo, "..") > 0 Then
            valido = False
        End If
        OKConvalidaEmail = valido
    End Function

#Region "Definizione: nomi tabelle"

    Public Const TABELLA_CATEGORIA As String = "CategArt"
    Public Const TABELLA_FORNSECONDARI As String = "FornSecondari"

#End Region
    Public Const ERROREALL As String = "ERROREALL"
    Public Const MODALPOPUP_CALLBACK_METHOD As String = "ModalPopup_MethodExecute"
    Public Const MODALPOPUP_CALLBACK_METHOD_NO As String = "ModalPopup_MethodExecute_NO"
    Public Const MODALPOPUP_TYPE As String = "TypeModalPopup"
    'giu031211
    Public Const ATTESA_CALLBACK_METHOD As String = "Attesa_MethodExecute"
    Public Const CSTNOBACK As String = "NoBack"
    'GIU241111
    Public Const CSTCODDITTA As String = "CodiceDitta"
    Public Const CSTMAXLEVEL As String = "MaxLevel"
    Public Const VALMAXLEVEL As Integer = 8
    Public Const CSTMASKLEVEL As String = "MaskLevel"
    Public Const VALMASKLEVEL As String = "11122233"
    'giu251111 GIU271119
    Public Const CSTABILMSG As String = "MSGAbilitaz"
    Public Const CSTABILAZI As String = "Abilitazioni"
    Public Const CSTABILCOGE As String = "AbilitazioniCoGe"
    '---------
    Public Const ESERCIZIO As String = "Esercizio"
    Public Const CSTUTENTE As String = "Utente"
    Public Const CSTTIPOUTENTE As String = "TipoUtente" 'GIU080312
    Public Const CSTEMAILUTENTE As String = "EmailUtente"
    Public Const CSTAZIENDARPT As String = "AziendaRPT"

    Public Const DBCONNMAST As String = "dbConnMaster"
    Public Const DBCONNINST As String = "dbConnInstall"
    Public Const DBCONNOPZ As String = "dbConnOpzioni"
    Public Const DBCONNSCAD As String = "dbConnScadenze"
    Public Const DBCONNCOGE As String = "dbConnCoGe"
    Public Const DBCONNAZI As String = "dbConnAzi"
    Public Const NomeModulo As String = "AziOnLine"

    Public Const ERRORE As String = "ERRORE" 'GIU150319
    Public Const CSTLEAD As String = "LeadSource"
    'giu050722 per visulizzare panel Sintetico/Analitico per le stampe dove è disabilitato
    'WUC_pREVcLIENTEoRDINEaG
    Public Const SWSINTANAL As String = "SWSintAnal"
    'giu290722 PER LA GESTIONE E ATTIVARE I CONTROLLI
    Public Const CSTGESTSPEDVETT As String = "GestSpedVett"
    'giu060423 PER ATTIVARE I CONTROLLI
    Public Const CSTCKSPEDVETT As String = "CKSpedVett"

    Public Enum TipoDB
        dbMaster = 0
        dbInstall = 1
        dbOpzioni = 2
        dbScadenzario = 3
        dbSoftCoge = 4
        dbSoftAzi = 5
    End Enum
    'giu010514
    Public Enum TipoCMD
        Text = 0
        StoreProcedure = 1
        TableDirect = 2
    End Enum

#Region "Strutture Tabelle/Dati"
    'giu111211 Operazioni lanciate da un Operatore e che possono essere lanciate in modalita' ESCLUSIVA
    Public Structure StrSWOperazioni
        Dim Operatore As String
        Dim SWPropostaRiordino As Boolean
        Dim DataOraPropostaRiordino As DateTime
    End Structure
    'giu020722
    '0:Campo,1:Valore,2:Abilitato,3:Visibile 
    Public Structure StrFormCampi
        Dim Campo As String
        Dim Valore As String
        Dim Abilitato As Boolean
        Dim Visibile As Boolean
        Dim TrueFalse As Boolean 'dipende dal Valore true false 0 <>0
    End Structure
    'giu241111
    Public Structure StrDitta
        Dim Codice As String
        Dim Descrizione As String
        Dim PartitaIVA As String
        Dim Telefono As String
        Dim Fax As String
        Dim Indirizzo As String
        Dim Citta As String
        Dim CAP As String
        Dim Provincia As String
        Dim Password As String
        Dim Livello_Op As String
        Dim MaxLevel As String
        Dim MaskLevel As String
        Dim Analitico As Boolean
        Dim RIBA As Boolean
        Dim Iva_Credito As Decimal
        Dim Iva_Debito As Decimal
        Dim Iva_Data As Date
        Dim TestoECC As String
        Dim PiedeECC As String
        Dim SL_Indirizzo As String
        Dim SL_Cap As String
        Dim SL_Citta As String
        Dim SL_Provincia As String
        Dim SWPersFis As Boolean
        Dim CodiceFiscale As String
        Dim Cognome As String
        Dim Nome As String
        Dim Sesso As String
        Dim Data_Nascita As DateTime
        Dim ComNas As String
        Dim ProvNas As String
        Dim SL_Denominazione As String
        Dim CFSoggObbAllIVA As String
        Dim CFIntermediario As String
        Dim IscrCAF As String
        Dim Blocca_Accesso As Boolean
        Dim GetNomePC As String
        Dim Blocco_Dalle As DateTime
    End Structure
    '---------
    'TipoRK tipo record usato sia per le ANAGRAFICHE PROVVISORIE CHE PER LE BANCHEIBAN 
    'per sapere se è un Cliente=C Fornitore=F Azienda=A (quest'ultimo solo per le BancheIBAN)
    Public Const TIPORK As String = "TipoRK"
    'Struttura Anagrafiche provvisorie per il CALLBACK METHOD
    Public Const RKANAGRPROVV As String = "AnagrProvv"
    Public Const IDANAGRPROVV As String = "IDAnagrProvv"
    Public Const IDANAGRREALESN As String = "IDAnagrRealeSN" 'simone040417
    Public Structure StrAnagrProvv
        Dim IDAnagrProvv As String 'giu291119 Int32
        Dim Ragione_Sociale As String
        Dim Codice_Fiscale As String
        Dim Partita_IVA As String
        Dim Indirizzo As String
        Dim Cap As String
        Dim Localita As String
        Dim Provincia As String
        Dim Stato As String
        Dim Tipo As String 'C CLIENTI F FORNITORI
    End Structure
    'giu240320 lasciato solo la costante mentre i specifici campi sono stati tipizzati nella classe DestCliFor
    Public Const RKANAGRDESTCLI As String = "RKANAGRDESTCLI"
    Public Const IDDESTCLIFOR As String = "IDDESTCLIFOR"
    Public Const CGDESTCLIFOR As String = "CGListaDestCliFor"
    Public Const DESTCLIFOR As String = "ListaDestCliFor"
    Public Const F_DESTCLIFOR_APERTA As String = "F_DestCliFor_APERTA"
    'Struttura Anagrafiche Clienti/Fornitori per il CALLBACK METHOD
    Public Const RKANAGRCLIFOR As String = "AnagrCliFor"
    Public Const IDANAGRCLIFOR As String = "IDAnagrCliFor"
    Public Structure StrAnagrCliFor
        Dim Rag_Soc As String
        Dim Denominazione As String
        Dim Riferimento As String
        Dim Codice_Fiscale As String
        Dim Partita_IVA As String
        Dim Indirizzo As String
        Dim NumeroCivico As String
        Dim Cap As String
        Dim Localita As String
        Dim Provincia As String
        Dim Nazione As String
        Dim Telefono1 As String
        Dim Telefono2 As String
        Dim Fax As String
        Dim Regime_Iva As Integer
        Dim EMail As String 'giu060514
        Dim EMailInvioScad As String 'simone180518
        Dim InvioMailScad As Boolean 'GIU190618
        Dim EMailInvioFatt As String 'giu190122
        Dim PECEMail As String 'GIU291018
        Dim IPA As String 'GIU020420
        Dim SplitIVA As Boolean
    End Structure
   
    'Struttura BancheIBAN per il CALLBACK METHOD
    Public Const RKBANCHEIBAN As String = "BancheIBAN"
    Public Const IDBANCHEIBAN As String = "IBAN"
    Public Structure StrBancheIBAN
        Dim IBAN As String
        Dim Descrizione As String
        Dim ABI As String
        Dim CAB As String
        Dim ContoCorrente As String
        Dim Tipo As String 'C CLIENTI F FORNITORI A AZIENDA che ha in uso il programma
    End Structure
    'giu251111 Da (NN)Scadenze
    Public Structure StrBanche
        Dim ABI As String
        Dim Banca As String
    End Structure
    Public Structure StrFiliali
        Dim ABI As String
        Dim CAB As String
        Dim Filiale As String
        Dim Nazione As String
        Dim CAP As String
        Dim Provincia As String
        Dim Citta As String
        Dim Indirizzo As String
    End Structure
    'giu251111 Da (NNAAAA)CoGe
    Public Const RKVALUTE As String = "RKValute"
    Public Structure StrValute
        Dim Codice As String
        Dim Descrizione As String
        Dim Cambio_Fisso As Boolean
        Dim Decimali As Integer
        Dim Simbolo As String
    End Structure
    'giu040112 Struttura Anagrafiche Vettori per il CALLBACK METHOD CallBackWFPAnagrVettori
    Public Const RKVETTORI As String = "RKVettori"
    Public Const IDVETTORI As String = "IDVettori"
    Public Structure StrVettori
        Dim IDVettori As Int32
        Dim Descrizione As String
        Dim Residenza As String
        Dim Localita As String
        Dim Provincia As String
        Dim Partita_IVA As String
        Dim Codice_CoGe As String
    End Structure
    'giu060112 Struttura Anagrafiche Agenti per il CALLBACK METHOD CallBackWFPAnagrAgenti
    Public Const RKAGENTI As String = "RKAgenti"
    Public Const IDAGENTI As String = "IDAgenti"
    Public Structure StrAgenti
        Dim IDAgenti As Int32
        Dim Descrizione As String
        Dim Residenza As String
        Dim Localita As String
        Dim Provincia As String
        Dim Partita_IVA As String
        Dim Codice_CoGe As String
        Dim IDCapoGruppo As Int32
    End Structure
    'giu200312 Struttura Anagrafiche CapiGruppo per il CALLBACK METHOD CallBackWFPAnagrCapiGruppo
    Public Const RKCAPIGRUPPO As String = "RKCapiGruppo"
    Public Const IDCAPIGRUPPO As String = "IDCapiGruppo"
    Public Structure StrCapiGruppo
        Dim IDCapiGruppo As Int32
        Dim Descrizione As String
    End Structure
    'giu090112 Struttura Anagrafiche Zone per il CALLBACK METHOD CallBackWFPAnagrZone
    Public Const RKZONE As String = "RKZone"
    Public Const IDZONE As String = "IDZone"
    Public Structure StrZone
        Dim IDZone As Int32
        Dim Descrizione As String
    End Structure
    'pier100112 Struttura Anagrafiche Categorie per il CALLBACK METHOD CallBackWFPAnagrCategorie
    Public Const RKCATEGORIE As String = "RKCategorie"
    Public Const IDCATEGORIE As String = "IDCategorie"
    Public Structure StrCategorie
        Dim IDCategorie As Int32
        Dim Descrizione As String
        Dim InvioMailSc As Boolean
        Dim SelSc As Boolean
    End Structure
    'sim040618 Struttura Testi Email per il CALLBACK METHOD CallBackWFPTestiEmail
    Public Const RKTESTIEMAIL As String = "RKTestiEmail"
    Public Const IDTESTIEMAIL As String = "IDTestiEmail"
    Public Structure StrTestiEmail
        Dim ID As Int32
        Dim Descrizione As String
        Dim Oggetto As String
        Dim Corpo As String
        Dim PiePagina As String
    End Structure
    'sim040618 Struttura Moduli per il CALLBACK METHOD CallBackWFPModuli
    Public Const RKMODULI As String = "RKModuli"
    Public Const IDMODULO As String = "IDModulo"
    Public Structure StrModuli
        Dim ID As Int32
        Dim Tipo As String
        Dim Percorso As String
    End Structure
    'pier160112 Struttura Anagrafiche CategorieArt per il CALLBACK METHOD CallBackWFPAnagrCategorieArt
    Public Const RKCATEGORIEART As String = "RKCategorieART"
    Public Const CODICECATART As String = "CodiceCatArt"
    Public Structure StrCategorieArt
        Dim Codice As String
        Dim Descrizione As String
        Dim ContoRicavo As String
    End Structure
    'pier170112 Struttura Anagrafiche LineeArt per il CALLBACK METHOD CallBackWFPAnagrLineeArt
    Public Const RKLINEEART As String = "RKLineeART"
    Public Const CODICELINEAART As String = "CodiceLineeArt"
    Public Structure StrLineaArt
        Dim Codice As String
        Dim Descrizione As String
    End Structure
    'giu140419
    Public Const RKTIPOCODBARART As String = "RKTipoCodBarART"
    Public Const CODICETIPOCODBARART As String = "CodiceTipoCodBarART"
    Public Structure StrTipoCodBarArt
        Dim Tipo_Codice As String
        Dim Descrizione As String
    End Structure
    'pier170112 Struttura Anagrafiche Misure per il CALLBACK METHOD CallBackWFPAnagrMisure
    Public Const RKMISURE As String = "RKMisure"
    Public Const CODICEMISURE As String = "CodiceMisure"
    Public Structure StrMisure
        Dim Codice As String
        Dim Descrizione As String
    End Structure
    'pier180112 Struttura Pagamenti per il CALLBACK METHOD CallBackWFPPagamenti
    Public Const RKPAGAMENTI As String = "RKPagamenti"
    Public Const IDPAGAMENTO As String = "IdPagamento"
    Public Structure StrPagamenti
        Dim IDPagamento As String
        Dim Descrizione As String
        Dim Tipo_Pagamento As Integer
        Dim Tipo_Scadenza As Integer
        Dim Numero_Rate As Integer
        Dim Numero_Rate_Effettive As Integer
        Dim Mese As Integer
        Dim Scadenza_1 As Integer
        Dim Scadenza_2 As Integer
        Dim Scadenza_3 As Integer
        Dim Scadenza_4 As Integer
        Dim Scadenza_5 As Integer
        Dim Perc_Imponib_1 As Decimal
        Dim Perc_Imponib_2 As Decimal
        Dim Perc_Imponib_3 As Decimal
        Dim Perc_Imponib_4 As Decimal
        Dim Perc_Imponib_5 As Decimal
        Dim Perc_Imposta_1 As Decimal
        Dim Perc_Imposta_2 As Decimal
        Dim Perc_Imposta_3 As Decimal
        Dim Perc_Imposta_4 As Decimal
        Dim Perc_Imposta_5 As Decimal
        Dim Mese_Escluso_1 As Integer
        Dim Mese_Escluso_2 As Integer
        Dim Spese_Incasso As Decimal
        Dim IVA_Spese_Incasso As Integer
        Dim Sconto_Cassa As Decimal
    End Structure

    Public Const CSTSCADPAG As String = "ScadPag"

    'giu170412
    Public Const RKCAUSMAG As String = "RKCausMag"
    Public Const IDCAUSMAG As String = "IDCAUSMAG"
    Public Structure StrCausMag
        Dim Codice As Int32
        Dim Descrizione As String
        Dim Tipo As String
        Dim Segno_Giacenza As String
        Dim Fatturabile As Boolean
        Dim Password As Boolean
        Dim Segno_Prodotto As String
        Dim Segno_Confezionato As String
        Dim Segno_Ordinato As String
        Dim Segno_Venduto As String
        Dim Componenti As Boolean
        Dim CausaleIndotta As Integer
        Dim Cod_Utente As Integer
        Dim Movimento_Magazzini As Boolean
        Dim Movimento As Boolean
        Dim CodContoCoGE As String
        Dim Segno_Deposito As String
        Dim CausVend As Boolean
        Dim Segno_Lotti As String
        Dim Segno_CL As String
        Dim CausCostoVenduto As Boolean
        Dim CVisione As Boolean
        Dim CDeposito As Boolean
        Dim CausCVenditaDaCVisione As Integer
        Dim CausCVenditaDaCDeposito As Integer
        Dim CausResoDaCVisione As Integer
        Dim CausResoDaCDeposito As Integer
        Dim PrezzoAL As String
        Dim Reso As Boolean 'giu220714
        Dim CausMag2 As Integer 'giu121020
    End Structure
    'giu260820 Struttura Magazzini per il CALLBACK METHOD CallBackWFPMagazzini
    Public Const RKMAGAZZINI As String = "RKMagazzini"
    Public Const IDMAGAZZINI As String = "IDMagazzini"
    Public Structure StrMagazzini
        Dim IDMagazzini As Int32
        Dim Descrizione As String
    End Structure
    'giu190612 Struttura Reparti per il CALLBACK METHOD CallBackWFPReparti
    Public Const RKREPARTI As String = "RKReparti"
    Public Const IDREPARTO As String = "IDReparto"
    Public Structure StrReparti
        Dim Magazzino As Integer
        Dim IDReparto As Int32
        Dim Cod_Utente As Int32
        Dim Descrizione As String
    End Structure
    'giu200612 Struttura Reparti per il CALLBACK METHOD CallBackWFPScaffali
    Public Const RKSCAFFALI As String = "RKScaffali"
    Public Const IDSCAFFALE As String = "IDScaffale"
    Public Structure StrScaffali
        Dim Magazzino As Integer
        Dim IDReparto As Int32
        Dim IDScaffale As Int32
        Dim Cod_Utente As Int32
        Dim Descrizione As String
    End Structure
    'giu200612 Struttura Anagrafiche TipoFatt per il CALLBACK METHOD CallBackWFPTipoFatt
    Public Const RKTIPOFATT As String = "RKTipoFatt"
    Public Const CODICETIPOFATT As String = "CodiceTipoFatt"
    Public Structure StrTipoFatt
        Dim Codice As String
        Dim Descrizione As String
    End Structure
    'giu260612 Struttura Anagrafiche CausNonEvasione per il CALLBACK METHOD CallBackWFPCausNonEvasione
    Public Const RKCAUSNONEVASIONE As String = "RKCausNonEvasione"
    Public Const IDCAUSNONEVASIONE As String = "CodiceCausNonEvasione"
    Public Structure StrCausNonEvasione
        Dim Codice As Int32
        Dim Descrizione As String
    End Structure
    'giu281220 Struttura LeadSource per il CALLBACK METHOD CallBackWFPLeadSource
    Public Const RKLEADSOURCE As String = "RKLeadSource"
    Public Const IDLEADSOURCE As String = "CodiceLeadSource"
    Public Structure StrLeadSource
        Dim Codice As Int32
        Dim Descrizione As String
    End Structure
    'Strutture Altre
    Public Structure STR_RETURN_VALUE
        Public Codice As String
        Public Descrizione As String
    End Structure
    'Parametri Generali AZI
    Public Structure strParGenAnno
        Public CalcoloScontoSuImporto As Boolean
        Public ScCassaDett As Boolean
    End Structure
#Region "Strutture Tabelle/Dati GESTIONE CONTRATTI"
    'giu021119 Struttura Anagrafiche RespArea per il CALLBACK METHOD CallBackWFPAnagrAgenti
    Public Const RKRESPAREA As String = "RKRespArea"
    Public Const IDRESPAREA As String = "IDRespArea"
    Public Const IDRESPAREAAPP As String = "IDRespAreaApp"
    Public Structure StrRespArea
        Dim IDRespArea As Int32
        Dim Descrizione As String
        Dim Residenza As String
        Dim Localita As String
        Dim Provincia As String
        Dim Partita_IVA As String
        Dim Codice_CoGe As String
        Dim Email As String
    End Structure
    '-
    Public Const RKRESPVISITE As String = "RKRespVisite"
    Public Const IDRESPVISITE As String = "IDRespVisite"
    Public Const IDRESPVISITEAPP As String = "IDRespVisiteApp"
    Public Structure StrRespVisite
        Dim IDRespVisite As Int32
        Dim Descrizione As String
        Dim Residenza As String
        Dim Localita As String
        Dim Provincia As String
        Dim Partita_IVA As String
        Dim Codice_CoGe As String
        Dim Email As String
        Dim IDRespArea As Int32
    End Structure
    'giu181219 giu270120
    Public Const RKTIPOCONTRATTO As String = "RKTipoContratto"
    Public Const IDTIPOCONTRATTO As String = "IdTipoContratto"
    Public Structure StrTipoContratto
        Dim Codice As Int32
        Dim Descrizione As String
        Dim TipoPagamento As Integer
        Dim TipoScadenza As Integer
        Dim FineMese As Boolean
        Dim Anticipato As Boolean
        Dim MeseCS As Integer
        Dim GiornoFisso As Integer
        Dim Cod_Causale As Integer
    End Structure
    Public Const CSTSCADPAGCA As String = "ScadPagCA"
    Public Const CSTSCADATTCA As String = "ScadAttCA"
#End Region

#End Region

#Region "Costanti per la gestione DOCUMENTI - Definizioni per la gestione del campo TipoDoc (DOCUMENTI)"
    Public Enum StatoDocPR
        DaConfermare = 0
        Confermato = 1
        ChiusoNonConf = 3
        NonConfermabile = 4
    End Enum
    Public Enum StatoDocOC
        DaEvadere = 0
        Evaso = 1
        ParzEvaso = 2
        ChiusoNonEvaso = 3
        NonEvadibile = 4
        InAllestimento = 5
        Allestito = 6
        ImpegnoForzato = 7
    End Enum
    Public Enum StatoDocOF
        DaEvadere = 0
        Evaso = 1
        ParzEvaso = 2
        ChiusoNonEvaso = 3
        NonEvadibile = 4
        InCarico = 5 'Allesttimento poi passa allo stato 1 EVASO
    End Enum
    Public Enum StatoDocDT
        DaEvadere = 0
        Evaso = 1
        ParzEvaso = 2
        ChiusoNonEvaso = 3
        NonEvadibile = 4
    End Enum
    Public Enum StatoDocFT 'Fatture commerciali / Riepilogative
        DaTrasfCoGe = 0
        TrasfCoGe = 1
    End Enum
    'GIU210714
    Public Enum StatoDocFA 'Fatture accompagnatorie
        DaTrasfCoGe = 0
        TrasfCoGe = 1
    End Enum

    Public Const IDSPEDIZIONI As String = "IDSpedizioni"
    Public Enum StatoSpedizione
        Allestimento = -1 'Seleziona le spedizioni d'allestire (Raggruppamento)
        Libero = 0
        ProntoPerStamp = 1
        StampatoBrogl = 2
        InAllestimento = 3
        ParzAllestito = 4
        Bloccato = 5
        Allestite = 6
        Chiuso = 7
    End Enum

    Public Enum TD
        Preventivi = 1 'PR
        OrdClienti = 2 'OC
        OrdDepositi = 3 'OD
        PropOrdFornitori = 4 'PF
        OrdFornitori = 5 'OF
        BuonoConsegna = 6 'BC
        DocTrasportoClienti = 7 'DT
        DocTrasportoFornitori = 8 'DF
        DocTrasportoCLavoro = 9 'DL
        FatturaCommerciale = 10 'FC
        FatturaAccompagnatoria = 11 'FA
        FatturaScontrino = 12 'FS
        NotaCredito = 13 'NC
        NotaCorrispondenza = 14 'NZ
        MovimentoMagazzino = 15 'MM Modimenti di MAGAZZINO
        CaricoMagazzino = 16 'CM Carichi di MAGAZZINO
        ScaricoMagazzino = 17 'SM Scarichi di MAGAZZINO
        ArticoloInstallato = 18 'AI Articolo Installato
        ContrattoAssistenza = 19 'CA Contratto Assistenza
        Inventari = 20 'IN Inventari
        'GIU220714 NOTA NON ESISTE UN TIPO DOC FAPA o NCPA ma sono FC/nc con FatturaPA<>0 
        'FatturaCommercialePA = 21 'FCPA
        'NotaCreditoPA = 22 'NCPA
        TipoContratto = 23
    End Enum
    Public Shared Function SWTD(ByVal TipoDoc As TD) As String
        'ATTENZIONE NON CREARE TIPI DOC = A PA O PN SONO FCPA NCPA
        'FATTURE/NC PA=FCPA PN=NCPA
        'giu211212 sblocca num a partita di anno: posso avere stessi numeri ma con anno diverso MAI UGUALE
        Select Case TipoDoc
            Case TD.Preventivi : SWTD = "PR" 'sblocca num a partita di anno
            Case TD.OrdClienti : SWTD = "OC" 'sblocca num a partita di anno
            Case TD.OrdDepositi : SWTD = "OD" 'sblocca num a partita di anno
            Case TD.PropOrdFornitori : SWTD = "PF" 'sblocca num a partita di anno
            Case TD.OrdFornitori : SWTD = "OF" 'sblocca num a partita di anno
            Case TD.BuonoConsegna : SWTD = "BC"
            Case TD.DocTrasportoClienti : SWTD = "DT"
            Case TD.DocTrasportoFornitori : SWTD = "DF"
            Case TD.DocTrasportoCLavoro : SWTD = "DL"
            Case TD.FatturaCommerciale : SWTD = "FC"
            Case TD.FatturaAccompagnatoria : SWTD = "FA"
            Case TD.FatturaScontrino : SWTD = "FS"
            Case TD.NotaCredito : SWTD = "NC"
            Case TD.NotaCorrispondenza : SWTD = "NZ"
            Case TD.MovimentoMagazzino : SWTD = "MM"
            Case TD.CaricoMagazzino : SWTD = "CM"
            Case TD.ScaricoMagazzino : SWTD = "SM"
            Case TD.ArticoloInstallato : SWTD = "AI"
            Case TD.Inventari : SWTD = "IN"
                'GIU220714 NOTA NON ESISTE UN TIPO DOC FAPA o NCPA ma sono FC/nc con FatturaPA<>0 
                'Case TD.FatturaCommercialePA : SWTD = "FCPA"
                'Case TD.NotaCreditoPA : SWTD = "NCPA"
                'ATTENZIONE NON CREARE TIPI DOC = A PA O PN SONO FCPA NCPA
                'FATTURE/NC PA=FCPA PN=NCPA
            Case TD.TipoContratto : SWTD = "TC"
            Case TD.ContrattoAssistenza : SWTD = "CA"
            Case Else : SWTD = ""
        End Select
    End Function
    Public Shared Function DesTD(ByVal TipoDoc As String) As String
        DesTD = "???SCONOSCIUTO???"
        'Preventivi
        If TipoDoc = SWTD(TD.Preventivi) Then
            DesTD = "PREVENTIVI CLIENTI"
        End If
        'Ordini
        If TipoDoc = SWTD(TD.OrdClienti) Then
            DesTD = "ORDINE CLIENTI"
        End If
        'Ordini C/Deposito
        If TipoDoc = SWTD(TD.OrdDepositi) Then
            DesTD = "ORDINE C/DEPOSITO"
        End If
        'PropOrdFornitori
        If TipoDoc = SWTD(TD.PropOrdFornitori) Then
            DesTD = "PROPOSTA ORDINI FORNITORE"
        End If
        'OrdFornitori
        If TipoDoc = SWTD(TD.OrdFornitori) Then
            DesTD = "ORDINE FORNITORE"
        End If
        'DDT
        If TipoDoc = SWTD(TD.DocTrasportoClienti) Then
            DesTD = "DOCUMENTO DI TRASPORTO CLIENTI"
        End If
        If TipoDoc = SWTD(TD.DocTrasportoCLavoro) Then
            DesTD = "DOCUMENTO DI TRASPORTO C/LAVORO"
        End If
        If TipoDoc = SWTD(TD.DocTrasportoFornitori) Then
            DesTD = "DOCUMENTO DI TRASPORTO FORNITORI"
        End If
        'fatture, NC,
        If TipoDoc = SWTD(TD.FatturaCommerciale) Then
            DesTD = "FATTURA COMMERCIALE"
        End If
        If TipoDoc = SWTD(TD.FatturaAccompagnatoria) Then
            DesTD = "FATTURA ACCOMPAGNATORIA"
        End If
        If TipoDoc = SWTD(TD.FatturaScontrino) Then
            DesTD = "FATTURA CON SCONTRINO"
        End If
        If TipoDoc = SWTD(TD.NotaCredito) Then
            DesTD = "NOTA DI CREDITO"
        End If
        If TipoDoc = SWTD(TD.NotaCorrispondenza) Then
            DesTD = "NOTA CORRISPONDENZA"
        End If
        If TipoDoc = SWTD(TD.BuonoConsegna) Then
            DesTD = "BUONO CONSEGNA"
        End If
        If TipoDoc = SWTD(TD.MovimentoMagazzino) Then
            DesTD = "MOVIMENTO DI MAGAZZINO"
        End If
        If TipoDoc = SWTD(TD.CaricoMagazzino) Then
            DesTD = "CARICO DI MAGAZZINO"
        End If
        If TipoDoc = SWTD(TD.ScaricoMagazzino) Then
            DesTD = "SCARICO DI MAGAZZINO"
        End If
        If TipoDoc = SWTD(TD.TipoContratto) Then
            DesTD = "TIPO CONTRATTO"
        End If
        If TipoDoc = SWTD(TD.ContrattoAssistenza) Then
            DesTD = "CONTRATTO ASSISTENZA"
        End If
    End Function
    Public Shared Function CtrTipoDoc(ByVal _TD As String, Optional ByRef TabCliFor As String = "", Optional ByVal CF As String = "") As Boolean
        'ATTENZIONE NON CREARE TIPI DOC = A PA O PN SONO FCPA NCPA
        'FATTURE/NC PA=FCPA PN=NCPA
        Select Case _TD
            Case "PR" : CtrTipoDoc = True : TabCliFor = "Cli" 'sblocca num a partita di anno
            Case "OC" : CtrTipoDoc = True : TabCliFor = "Cli" 'sblocca num a partita di anno
            Case "OD" : CtrTipoDoc = True : TabCliFor = "Cli" 'Al momento non usato
            Case "PF" : CtrTipoDoc = True : TabCliFor = "For" 'sblocca num a partita di anno
            Case "OF" : CtrTipoDoc = True : TabCliFor = "For" 'sblocca num a partita di anno
            Case "BC" : CtrTipoDoc = True : TabCliFor = "Cli" 'Al momento non usato
            Case "DT" : CtrTipoDoc = True : TabCliFor = "Cli"
            Case "DF" : CtrTipoDoc = True : TabCliFor = "For"
            Case "DL" : CtrTipoDoc = True : TabCliFor = "Cli" 'Al momento non usato
            Case "FC" : CtrTipoDoc = True : TabCliFor = "Cli"
            Case "FA" : CtrTipoDoc = True : TabCliFor = "Cli" 'Al momento non usato
            Case "FS" : CtrTipoDoc = True : TabCliFor = "Cli" 'Al momento non usato
            Case "NC" : CtrTipoDoc = True : TabCliFor = "Cli"
            Case "NZ" : CtrTipoDoc = True : TabCliFor = "Cli" 'Al momento non usato
            Case "MM" : CtrTipoDoc = True : TabCliFor = "CliFor"
            Case "CM" : CtrTipoDoc = True : TabCliFor = "CliFor"
            Case "SM" : CtrTipoDoc = True : TabCliFor = "CliFor"
            Case "AI" : CtrTipoDoc = True : TabCliFor = "Cli" 'Articolo installato
            Case "IN" : CtrTipoDoc = True : TabCliFor = "CliFor" 'Inventari
                'GIU220714 NOTA NON ESISTE UN TIPO DOC FAPA o NCPA ma sono FC/nc con FatturaPA<>0 
                'Case "FCPA" : CtrTipoDoc = True : TabCliFor = "Cli"
                'Case "NCPA" : CtrTipoDoc = True : TabCliFor = "Cli"
                'ATTENZIONE NON CREARE TIPI DOC = A PA O PN SONO FCPA NCPA
                'FATTURE/NC PA=FCPA PN=NCPA
            Case "TC", "CA"
                CtrTipoDoc = True : TabCliFor = "Cli"
            Case Else : CtrTipoDoc = False : TabCliFor = ""
        End Select
    End Function
#End Region

#Region "Stampe e statistiche"
    'GIU240514
    Public Const CSTESPORTAPDF As String = "EsportaPDF"
    Public Const CSTPATHPDF As String = "PathPDF"
    Public Const CSTNOMEPDF As String = "NomePDF"
    Public Const CSTESPORTABIN As String = "EsportaBIN"
    Public Const CSTNOMEBIN As String = "NomeBIN"
    Public Const CSTPATHPDFWEB As String = "PathPDFWEB"
    '---------
    'Roberto 10/12/2011 'FABIO 14/12/2011
    Public Const CSTTIPORPTMAG As String = "TipoRptMag"
    Public Const CSTORDINATO As String = "CSTORDINATO"
    Public Const CSTFATTURATO As String = "CSTFATTURATO"
    Public Const CSTSTATISTICHE As String = "CSTSTATISTICHE" 'FABIO 14/12/2011
    Public Const CSTTIPORPTMOVMAG As String = "TipoRptMovMag"
    Public Const CSTTIPORPTDISPMAG As String = "TipoRptDispMag"
    Public Const CSTTIPORPTCONTROLLO As String = "TipoRptControllo"
    'GIU040612 Articoli installati / Contratti di assistenza
    Public Const CSTTipoStampaAICA As String = "TipoStampaAICA"
    Public Enum TipoStampaAICA
        SingoloAICA = 0
        ElencoAICACliArtNSerie = 1
        ElencoAICAArtCliNSerie = 2
        ElencoAICAScGaArtCliNSerie = 3
        ElencoAICAScElArtCliNSerie = 4
        ElencoAICAScBaArtCliNSerie = 5
        ElencoAICASintetico = 6
    End Enum
    'GIU130612 Inventario
    Public Const CSTTipoStampaIN As String = "TipoStampaIN"
    Public Enum TipoStampaIN
        SingoloIN = 1
        ElencoIN = 2
    End Enum

    Public Enum TIPOSTAMPALISTINO

        Listini = 1
        ArticoliAnalitica = 2
        ArticoliSintetica = 3
        ArticoliUbicazione = 4
        ArticoliFornitoreCOD = 5
        ArticoliFornitoreDES = 6
        ArticoliUbicazioneGM = 7
        'giu010221
        ArticoliFornitoreCODP = 8
        ArticoliFornitoreDESP = 9

    End Enum
    Public Enum TIPOSTAMPAORDINATO

        OrdinatoArticolo = 1
        OrdinatoArticoloData = 2
        OrdinatoClienteCodiceCoge = 3
        OrdinatoClienteRagSoc = 4
        OrdinatoArticoloCliente = 5
        OrdinatoClienteOrdine = 6
        ListaCarico = 7
        ListaCaricoSpedizione = 8
        OrdinatoOrdineSortByNDoc = 9
        OrdinatoOrdineSortByDataDoc = 10
        OrdinatoOrdineSortByDataConsegna = 11
        OrdinatoClienteCodiceCogeAg = 12
        OrdinatoClienteRagSocAg = 13
        OrdinatoArticoloAg = 14
        OrdinatoArticoloDataAg = 15
        OrdinatoArticoloClienteAg = 16
        OrdinatoClienteOrdineAG = 17
        OrdinatoOrdineSortByNDocAG = 18
        OrdinatoOrdineSortByDataDocAG = 19
        OrdinatoOrdineSortByDataConsegnaAG = 20
        StatOrdinatoClienteOrdine = 21 'alb11122012
        OrdinatoArticoloClienteFor = 22 'giu060420
        'giu080421
        PrevClienteOrdineAG = 23
        PrevClienteOrdineLS = 24 'giu190421
        PrevOrdineClienteAG = 25 'giu050722 ordine per N° Prev. crescente o data crescente
        'giu060823 stampa da gestione attivitàcontratti
        PrevClienteOrdineAGCA = 26
        PrevOrdineClienteAGCA = 27
        '---------
        'giu110612 
        OrdinatoArticoloFornitore = 50
        'giu271013 OF TUTTI EVASI/PARZ.EVASI TUTTI
        StatOrdForOrdTutti = 51
        OrdinatoArticoloClienteForS = 52 'giu081121
    End Enum
    Public Enum TIPOSTAMPAFATTURATO
        'giu060211
        FattOrdineSortClienteNDoc = 1
        FattOrdineSortByNDoc = 2
        FattOrdineSortByDataDoc = 3
        FattSintOrdineSortByNDoc = 4
        DiffFTDTSintOrdineSortByNDoc = 5 'giu060312
        FattOrdineSortClienteNDocAG = 6 'alb18062012
        FattOrdineSortByNDocAG = 7 'alb18062012
        FattOrdineSortByDataDocAG = 8 'alb18062012
        FattSintOrdineSortByNDocAG = 9 'alb18062012
        FattSintOrdineSortByNDocReg = 10 'alb19062012
        DTFTDoppiSintOrdineSortByNDoc = 11 'giu151012
        FTNCCCausErrSintOrdineSortByNDoc = 12 'giu161012
        '-
        FattOrdineSortClienteNDocMargMP = 13 'Costo medio ponderato
        FattOrdineSortClienteNDocMargFF = 14 'Costo FIFO data_doc <= data_carico di magazzino
        
    End Enum
    Public Enum TIPOSTAMPASTATISTICA  'FABIO 14/12/2011
        VendutoClienteArticolo = 1
        VendutoArticoloCliente = 2
        VendutoDDT = 3
        FatturatoAgenteSintetico = 4
        FatturatoAgenteAnalitico = 5
        'Alberto 16/05/2012
        VendutoClienteArticoloAG = 6
        VendutoArticoloClienteAG = 7
        VendutoDDTAG = 8
        IncidenzaNCFatturato = 9 'alb22062012
        '----
        ControlloVendutoCVByArt = 10 'giu151012
        'giu051112
        VendutoClienteArticoloREG = 11
        VendutoArticoloClienteREG = 12
        'giu160513
        VendutoForArt = 13 'sintetico
        VendutoForArtCli = 14 'analitico anche pr controllo non ATTIVATO
        'giu020216 'raggruppamento per CATEGORIA CLIENTE
        VendutoClienteArticoloCC = 15 '15 ORDINATO PER CLIENTE / ARTICOLO
        VendutoArticoloClienteCC = 16 '16 ORDINATO PER ARTICOLO / CLIENTE 
        'giu030216
        VendutoAgFortArt = 17
        VendutoRegFortArt = 18
        'FABIO 05022016
        VendutoCliForArt = 19
        'FABIO 08022016
        VendutoArticoloClienteCCSintetico = 20
        VendutoAgenteCateg = 21
        VendutoRegioneCateg = 22
        VendutoCliForArtSintetico = 23 'Fabio19022016
        VendutoAgenteCategSintetico = 24 'Fabio19022016
        VendutoRegioneCategSintetico = 25 'Fabio19022016
        VendutoRegionePRCategCliArt = 26 'giu280717

        InstallatoClientiArticolo = 27 'giu020923

        StatTotaliClientiAl = 30
        StatFattAnnoMeseInPr = 31
        StatFattAnnoMeseInPrArt = 32
        StatTotaliContrattiAl = 33
    End Enum 'FINE  FABIO 14/12/2011
    Public Enum TIPOSTAMPADISPMAGAZZINO  'FABIO 14/12/2011
        DisponibilitaMagazzino = 1
        DisponibilitaMagazzinoFornitori = 2
        'giu020312  GIUBESTIA190412
        '-VALORIZZAZZIONE MAGAZZINO (FIFO)
        ValMagFIFO = 3
        ValMagFIFOSintetico = 4
        ValMagFIFOAnalitico = 5
        '-VALORIZZAZZIONE MAGAZZINO (LIFO)
        ValMagLIFO = 6
        ValMagLIFOSintetico = 7 'stampa definitiva
        ValMagLIFOAnalitico = 8 'stampa di controllo (TUTTO I MOVIMENTI)
        '-VALORIZZAZZIONE MAGAZZINO COSTO ULTIMO
        ValMagUltPrzAcq = 9
        ValMagUltPrzAcqSintetico = 10 'stampa definitiva
        ValMagUltPrzAcqAnalitico = 11 'stampa di controllo (SOLO CARICHI in ordine cronologico)
        '-VALORIZZAZZIONE MAGAZZINO MEDIA PONDERATA
        ValMagMediaPond = 12
        ValMagMediaPondSintetico = 13 'stampa definitiva
        ValMagMediaPondAnalitico = 14 'stampa di controllo (TUTTO I MOVIMENTI)
        '-COSTO DEL VENDUTO
        ValMagCostoVendFIFO = 15
        ValMagCostoVendFIFOSint = 16
        'giu250118 VALORIZZAZZIONE MAGAZZINO SINTETICO (FIFO) RAGGRUPPATO PER FORNITORE
        ValMagFIFOSinteticoFor = 17
        ValMagFIFOSinteticoForS = 18 'GIU181121 SINTETICO: TOTALI FORNITORI
    End Enum 'FINE  FABIO 14/12/2011

    Public Enum TIPOSTAMPAMOVMAG
        MovMagDaDataAData = 1
        MovMagByIDDocumenti = 2
        MovMagByArticolo = 3
        'giu170211
        ValCMSMOrdineSortCliForNDoc = 4
        ValCMSMOrdineSortByNDoc = 5
        ValCMSMOrdineSortByDataDoc = 6
        ValCMSMSintOrdineSortByNDoc = 7
        'GIU230421
        VendutoLeadSourceA = 8
        VendutoLeadSourceS = 9
        'Elenco DDT per magazzino/Causale
        ElencoDDTMagCaus = 10 'giu250124
    End Enum
    Public Enum TIPOSTAMPACONTROLLO

        DiffPrezzoListino = 1
        DiffImportoRiga = 2
        CKSerieLotto = 3 'giu190723

    End Enum

    Public Const STAMPAMOVMAGTORNAAELENCO As String = ""

    Public Const CSTDSStatVendCliArt As String = "DsStatVendCliArt"
    Public Const CSTDSOrdinatoArticolo As String = "DSOrdinatoArticolo"

#End Region

    Public Const CSTCODCOGE As String = "Codice_CoGe"
    Public Const CSTCODCOGEDM As String = "Codice_CoGeDM" 'GIU120321 PER LA DESTINAZIONE MERCE NELLA GEZIONE ANAGRAFICA CLIENTI E FORNITORI
    Public Const CSTCODCOGEOCPR As String = "Codice_CoGeOCPR" 'GIU010223 PER LA VISUALIZZAZIONE PREVENTIVI/ORDINI DA GESTIONE CLIENTI
    Public Const CSTCODCOGENEW As String = "Codice_CoGe_New"
    Public Const CSTCODFILIALE As String = "Codice_Filiale"
    'Tipo DOCUMENTO 
    Public Const CSTTIPODOCSEL As String = "TipoDocSelezionato" 'giu241111
    'ATTENZIONE PARAMETRO DI SESSIONE NEL SqlDSPrevTElenco USATO PER TUTTI I FRM DI ELENCO
    Public Const CSTTIPODOC As String = "TipoDoc"
    'GIU220714 NOTA NON ESISTE UN TIPO DOC FAPA o NCPA ma sono FC/nc con FatturaPA<>0 
    Public Const CSTFATTURAPA As String = "FatturaPA"
    Public Const CSTSPLITIVA As String = "SplitIVA" 'GIU221217
    Public Const CSTFCACSA As String = "FCACCONTOSALDO" 'GIU290419
    'ATTENZIONE PARAMETRO DI SESSIONE NEL SqlDSPrevTElenco USATO PER TUTTI I FRM DI ELENCO
    'Stato DOCUMENTO 0 Da confermare/evadere 1 Confermato/Evaso 2 Parz. confermato/Evaso
    '                3 Chiuso non confermato/evaso 4 Non confermabile/evadibile
    '                999 TUTTI
    Public Const CSTSTATODOC As String = "StatoDoc"
    Public Const CSTSTATODOCSEL As String = "StatoDocSEL"
    'ATTENZIONE PARAMETRO DI SESSIONE NEL SqlDSPrevTElenco USATO PER TUTTI I FRM DI ELENCO
    'Ordinamento documenti N Numero documento D Data documento      R Ragione Sociale 
    '                      C DataOraConsegna  V DataOraValidità     
    Public Const CSTSORTPREVTEL As String = "SortPrevTEl"
    'Nel Load si un form con tanti Option seleziona quello che mi interessa es DocumentiElenco
    Public Const CSTSWRbtnTD As String = "SWRbtnTD" 'DocumentiElenco
    Public Const CSTSWRbtnMM As String = "SWRbtnMM" 'Gestione Movimenti di magazzino giu230612
    'TABELLA select Clienti/Fornitori
    Public Const CSTTABCLIFOR As String = "TabCliFor"
    'Quando passo da un form Elenco al form di Gestione
    Public Const IDDOCUMENTI As String = "IDDocumenti"
    'GIU221021 NEI CLIENTI PER EVITARE CONFLITTO CON PREVENTIVI
    Public Const IDDOCPRCLI As String = "IDDocPRCli"
    Public Const CSTTIPODOCPR As String = "TipoDocPR"
    Public Const CSTSTATODOCPR As String = "StatoDocPR"
    Public Const CSTSTATODOCSELPR As String = "StatoDocSELPR"
    Public Const CSTSWRbtnTDPR As String = "SWRbtnTDPR"
    'GIU181221 NEI CLIENTI PER EVITARE CONFLITTO CON ORDINI
    Public Const IDDOCOCCLI As String = "IDDocOCCli"
    Public Const CSTTIPODOCOC As String = "TipoDocOC"
    Public Const CSTSTATODOCOC As String = "StatoDocOC"
    Public Const CSTSTATODOCSELOC As String = "StatoDocSELOC"
    Public Const CSTSWRbtnTDOC As String = "SWRbtnTDOC"
    'giu181221 per evitare di perdere IDDocumenti
    Public Const IDDOCCAMBIAST As String = "IDDocCambiaSt"
    'Quando in Gestione faccio nuovo e nel caso annullo rileggo il documento precedente else RITORNO
    Public Const IDDOCUMSAVE As String = "IDDocumSave"
    'GIU011218 DA DOCUMENTI COLLEGATI PER LA FUNZIONE DI AGGIORNAMENTO QTAEV_QTAALL_STATODOC
    Public Const IDDOCUMCOLLCALL As String = "IDDocumCollCall"
    Public Const IDDOCUMCOLL As String = "IDDocumColl"
    Public Const DATADOCCOLL As String = "DataDocColl"
    Public Const TIPODOCCOLL As String = "TipoDocColl"
    'Data Documento per il calcolo scadenze (PER QUESTE SESSION creare una chiamata con ritorno dati)
    Public Const CSTDATADOC As String = "DataDocumento"
    'Listino del CLIENTE/DOCUMENTO se cambiato per DocumentiDett QUANDO devo prendere i PREZZI
    Public Const IDLISTINO As String = "IDListino"
    'Gestione Articoli Installati   'Pier 150512 
    Public Const IDARTICOLOINST As String = "IDArticoloInst"
    Public Const IDEMAILINVIATE As String = "IDEmailInviate"
    'giu240512 UNIFICATO WUC quindi uso IDARTICOLOINST x entrambi
    ' ''Public Const IDCONTRATTOASSISTENZA As String = "IDContrattoAssist" GIU291119 AL MOMENTO USO IDDOCUMENTI 
    'Codice Valuta del documento che si sta modificando
    Public Const CSTVALUTADOC As String = "ValutaListinoDoc"
    'Codice Valuta del documento che si sta modificando
    Public Const CSTDECIMALIVALUTADOC As String = "DecimaliValutaListinoDoc"
    'Regime IVA del CLIENTE per il calcolo del Totale documento
    'NOTA INTERCETTARE QUANDO PRENDE L'ARTICOLO E L'IVA SE IL REGIME > 49 METTERE QUELLO CHE HA IL CLIENTE
    'E NON IL 20% CHE HA L'ARTICOLO
    Public Const CSTREGIMEIVA As String = "RegimeIVA"
    'Codice Pagamento per il caldolo delle scadenze 
    'GIU280120 NEL CASO DEI CONTRATTI CONTIENE ID DELLA TABELLA TIPOCONTRATTO NEL DB SCADENZARIO
    Public Const CSTIDPAG As String = "IDPagamento"
    'GIU280120 
    'giu050220 contratti per le chiamate alle SP get del upg etc etc 
    Public Const IDDURATANUM As String = "DurataNum"
    Public Const IDDURATANUMRIGA As String = "DurataNumRiga"
    '------
    Public Const CSTDURATATIPO As String = "DurataTipo"
    Public Const CSTDURATANUM As String = "DurataNumPeriodo"
    Public Const CSTTIPOFATT As String = "TipoFatturazione"
    Public Const CSTNUMGGANNO As String = "NumGGAnno"
    Public Const CSTDATAINIZIO As String = "DATAINIZIO"
    Public Const CSTDATAFINE As String = "DATAFINE"
    Public Const CSTDATAACCETTA As String = "DATAACCETTA"
    Public Const CSTNONCOMPLETO As String = "NONCOMPLETO"
    Public Const CSTSERIELOTTO As String = "SERIELOTTO"
    Public Const CSTSERIELOTTOSAVE As String = "SERIELOTTOSAVE"
    Public Const CSTNUOVOCADAOC As String = "NUOVOCADAOC"
    Public Const CSTNUOVOCADACA As String = "NUOVOCADACA"
    Public Const L_NSERIELOTTO As String = "L_NSERIELOTTO" 'GIU010322 PER LE NOTE PUNTUALI AL NSERIELOTTO
    'Codice Agente del DOCUMENTO (Cliente) per DocumentiDett 
    Public Const IDAGENTE As String = "IDAgente"
    'SCONTO CASSA del DOCUMENTO della pagina Spese, Trasporti e Totali
    Public Const CSTSCCASSA As String = "ScontoCassa" 'PERCENTUALE
    'ABBUONO del DOCUMENTO della pagina Spese, Trasporti e Totali
    Public Const CSTABBUONO As String = "Abbuono" 'PERCENTUALE
    'SPESE TRASPORTO del DOCUMENTO della pagina Spese, Trasporti e Totali
    Public Const CSTSPTRASP As String = "SpeseTrasporto"
    'SPESE IMBALLO del DOCUMENTO della pagina Spese, Trasporti e Totali
    Public Const CSTSPIMBALLO As String = "SpeseImballo"
    'SPESE INCASSO del DOCUMENTO della pagina Spese, Trasporti e Totali
    Public Const CSTSPINCASSO As String = "SpeseIncasso"
    'giu040319 BOLLO E CONTROLLO SE E' DA APPLICARE (ESCLUSI PER GLI STRANIERI)
    Public Const CSTBOLLO As String = "BOLLO"
    Public Const CSTBOLLOACARICODEL As String = "BOLLOACARICODEL"
    'giu270412 Segno_Giacenza INIZIALIZZATO IN CKPrezzoAL xkè letto la causale. 
    'Mi serve per il controllo della qta'evasa se vado in NEGATIVO quando vendo
    Public Const CSTSEGNOGIACENZA As String = "Segno_Giacenza"
    'giu050315 per selezionare i documenti in cui è presente un articolo
    Public Const CODARTSEL As String = "CodArtSel"

#Region "Messaggi fissi"
    Public Const MSGEccedIva As String = "Il numero di voci dell'Iva utilizzate per questo documento eccede il numero previsto."
    Public Const MSGAttesa As String = "Operazione di reperimento dati in corso, attendere ....."
    Public Const MSGTimeout As String = "Tempo scaduto, è possibile che l'operazione per il reperimento dati dal server non sia andato a buon fine. Contattare l'amministratore di sistema."
#End Region

#Region "Definizione: CST di sessione Elenco"

    Public Const CSTAttivo As String = "Attivo"
    Public Const CSTInattivo As String = "Inattivo"
    Public Const CSTPotenziale As String = "Cliente Potenziale"
    'giu111012 serve a Inattivo i Potenziali Clienti in definitivi CoGe: (nn);12345678
    Public Const CSTUpgAngrProvvCG As String = "ClientePotenzialeTOCoGe"

    Public Const COD_CLIENTE As String = "CodiceCliente"
    Public Const COD_FORNITORE As String = "CodiceFornitore" 'giu261111

    Public Const IDMAGAZZINO As String = "IDMagazzino" 'GIU190612
    Public Const COD_ARTICOLO As String = "CodArticolo"

    Public Const FORNITORE_SEC As String = "FornitoreSec"

    Public Const CSTSALVADB As String = "SalvataggioDataBase"

    Public Const CSTUTILIY As String = "Utility" 'giu091012
    Public Enum CallUtility
        Nessuna = 0
        InitMySQL = 1
    End Enum

    Public Const F_ANAGR_PROVV_APERTA As String = "FinestraAnagraProvvAperta"
    Public Const F_ANAGRCLIFOR_APERTA As String = "FinestraAnagrCliForAperta"
    Public Const F_ANAGRVETTORI_APERTA As String = "FinestraAnagrVettoriAperta"
    Public Const F_ANAGRAGENTI_APERTA As String = "FinestraAnagrAgentiAperta"
    Public Const F_ANAGRNAZIONI_APERTA As String = "FinestraAnagrNazioniAperta"
    Public Const F_ANAGRCAPIGR_APERTA As String = "FinestraAnagrCapiGruppoAperta"
    Public Const F_ANAGRZONE_APERTA As String = "FinestraAnagrZoneAperta"
    Public Const F_ANAGRTIPOFATT_APERTA As String = "FinestraAnagrTipoFattAperta"
    Public Const F_ANAGRCATEGORIE_APERTA As String = "FinestraAnagrCategorieAperta"
    Public Const F_ANAGRCATEGORIEART_APERTA As String = "FinestraAnagrCategorieArtAperta"
    Public Const F_ANAGRLINEEART_APERTA As String = "FinestraAnagrLineeArtAperta"
    Public Const F_ANAGRTIPOCODART_APERTA As String = "FinestraAnagrTipoCodiceArtAperta"
    Public Const F_ANAGRMISURE_APERTA As String = "FinestraAnagrMisureAperta"
    Public Const F_PAGAMENTI_APERTA As String = "FinestraPagamentiAperta"
    Public Const F_SEL_ARTICOLO_APERTA As String = "FinestraSelArticoloAperta"
    Public Const F_BANCHEIBAN_APERTA As String = "FinestraBancheIbanAperta"
    Public Const F_FORNSEC_APERTA As String = "FinestraFornSecAperta"
    Public Const F_SCELTALISTINI_APERTA As String = "FinestraSceltaListiniAperta"
    Public Const F_ANAGRREP_APERTA As String = "FinestraAnagrRepartiAperta"
    Public Const F_ANAGRSCAFFALI_APERTA As String = "FinestraAnagrScaffaliAperta"
    Public Const F_GESTIONETESTIEMAIL_APERTA As String = "FinestraGestioneTestiEmailAperta"
    'giu240112 Carico di Magazzino da OF
    Public Const F_SCELTAMOVMAG_APERTA As String = "FinestraSceltaMovMagAperta" 'filtra i movimenti di REFINTMOVMAG
    Public Const F_EVASIONEPARZ_APERTA As String = "FinestraEvasioneParzAperta"
    Public Const F_ELENCO_CLIFORN_APERTA As String = "FinestraElencoCliFornAperta"
    Public Const F_ELENCO_DESTCF_APERTA As String = "FinestraElencoDestCFAperta"
    Public Const F_ELENCO_DESTCFD_APERTA As String = "FinestraElencoDestCFDAperta"
    Public Const LISTINI_DA_AGG As String = "ListiniDaAggiornare"
    'giu140612 GESTIONE INVENTARIO
    Public Const F_MODIFICASCHEDAIN_APERTA As String = "FinestraModificaSchedaINAperta"
    Public Const L_SCHEDAIN_DA_AGG As String = "SchedaINDaAgg" 'Lista Righe da Aggiornare/Differenza
    'giu220612 GESTIONE RESI
    Public Const F_RESODAC_APERTA As String = "FinestraResoDaCAperta"
    Public Const F_RESOAF_APERTA As String = "FinestraResoAFAperta"
    Public Const F_NCAAC_APERTA As String = "FinestraNCAClienteAperta"
    Public Const L_RESODACF As String = "ListaResoDaCF" 'Lista Righe da caricare
    'giu260612 
    Public Const F_ANAGRCAUSNONEV_APERTA As String = "FinestraAnagrCausNonEvasioneAperta"
    Public Const F_CAMBIOSTATO_APERTA As String = "FinestraCambioStatoAperta"
    'Roberto 16/12/2011---
    Public Const OSCLI_F_ELENCO_CLI1_APERTA As String = "OSCliFinestraElencoCli1Aperta"
    Public Const OSCLI_F_ELENCO_CLI2_APERTA As String = "OSCliFinestraElencoCli2Aperta"
    Public Const OSCLI_F_ELENCO_FORN1_APERTA As String = "OSCliFinestraElencoForn1Aperta"
    Public Const OSCLI_F_ELENCO_FORN2_APERTA As String = "OSCliFinestraElencoForn2Aperta"
    Public Const OSART_F_ELENCO_ART1_APERTA As String = "OSArtFinestraElencoArt1Aperta"
    Public Const OSART_F_ELENCO_ART2_APERTA As String = "OSArtFinestraElencoArt2Aperta"
    '---------------------
    'giu270412 spostato da wuc_documenti
    Public Const F_CLI_RICERCA As String = "FinestraClientiApertaDaRicerca"
    Public Const F_FOR_RICERCA As String = "FinestraFornitoriApertaDaRicerca"
    'giu210918 spostato dalle varie finestre di statistica
    Public Const SWCOD1COD2 As String = "SWCod1Cod2"

    Public Const F_ALIQUOTAIVA As String = "ElencoAliquotaIVA" 'contiene il nome LISTA
    '-----------------------------------
    Public Const F_ELENCO_APERTA As String = "FinestraElencoAperta" 'contiene il nome LISTA
    '---------------------
    'giu281211 Allestimento Scelta spedizione dove includere un ordine
    Public Const F_SCELTASPED_APERTA As String = "FinestraSceltaSpedAperta"
    '---------------------
    Public Const REFINTMOVMAG As String = "RefIntMovMag" 'Seleziona i movimenti caricati da un ordine a fornitore
    Public Const MOVMAG_DA_CANC As String = "MovMagDaCanc" 'Lista MovMag da cancellare

    'giu240112 Carico di Magazzino da OF
    '-- ID viene usato IDDOCUMENTI
    Public Const L_EVASIONEPARZ_DA_CAR As String = "EvasioneParzDaCar" 'Lista Righe da caricare

    Public Const RTN_VALUE_F_ELENCO As String = "CodiceFinestraElenco"

    Public Const PANELCATEG As String = "ModificaCategorie"

    'Gestione DocumentiD scelta Articoli e inserimento
    Public Const ARTICOLI_DA_INS As String = "ListaCodiciArtSel"
    'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
    Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
    Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
    Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
    Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
    '-----------------------------------------------------------------------------------------------
    'giu281211 Allestimento Scelta spedizione dove includere un ordine
    Public Const IDSPEDIZIONESEL As String = "IDSpedizioneSelezionata"

    'giu040512 Elenco Documenti da stampare con sconti o senza sconti
    Public Const EL_DOC_TOPRINT_SCY As String = "ElencoIDDocDaStampareSiSconti"
    Public Const EL_DOC_TOPRINT_SCN As String = "ElencoIDDocDaStampareNoSconti"
    'giu200912
    Public Const EL_DOC_TOPRINT As String = "ElencoIDDocDaStampare"
    'giu080416
    Public Const F_CARICALOTTI As String = "CaricaLotti"
    'sim270317
    Public Const F_DOCCOLL_APERTA As String = "DocCollegatiAperta"
    'GIU210518 GIU120718 GIU130718
    Public Const F_ARTINSTEMAIL_EMAILINVIATE As String = "ArtInstEmailElencoEmailInviate"
    Public Const F_ARTINSTEMAIL_APERTA As String = "FinestraArtInstEmailAperta"
    Public Const F_EMAILINVIATE_APERTA As String = "FinestraElencoEmailInviate"
    'SIM250518
    Public Const F_STAMPAELENCOAI_APERTA As String = "FinestraStampaElencoAIAperta"
    'sim010618
    Public Const F_VISUALIZZADETT_APERTA As String = "FinestraVisualizzaDettElencoAperta"
    Public Const F_MODULI_APERTA As String = "FinestraModuliAperta"
    'sim080618
    Public Const CSTDsArticoliInstEmail As String = "DsArticoliInstEmail"
    Public Const CSTDsEmailInviate As String = "DsEmailInviate" 'giu130718
    Public Const F_ETP_APERTA As String = "EtichettePreparaAperta"
    'giu011119
    Public Const F_ANAGRRESPAREA_APERTA As String = "FinestraAnagrRespAreaAperta"
    Public Const F_ANAGRRESPVISITE_APERTA As String = "FinestraAnagrRespVisiteAperta"
    Public Const F_MAGAZZINI_APERTA As String = "FinestraMagazziniAperta" 'giu260820
    Public Const F_LEAD_APERTA As String = "FinestraLeadAperta" 'giu260820
#End Region

#Region "Definizione: CST di applicazioni e cache"

    Public Const DATI_CARICATI As String = "FlagDatiCaricati"
    Public Const PARAM_GESTAZI As String = "ParamGestAzi"
    Public Const PROGRESSIVI_COGE As String = "ProgressiviCoGe"
    Public Const NAZIONI As String = "Nazioni"
    Public Const ALIQUOTA_IVA As String = "AliquoteIva"
    Public Const PROVINCE As String = "Province"
    Public Const PAGAMENTI As String = "Pagamenti"
    Public Const ZONE As String = "Zone"
    Public Const VETTORI As String = "Vettori"
    Public Const CATEGORIE As String = "Categorie"
    Public Const PIANODEICONTI As String = "PianoDeiConti"
    Public Const AGENTI As String = "Agenti"
    Public Const LISTVEN_T As String = "ListVenT"
    Public Const TIPOFATT As String = "TipoFatt"
    Public Const CLIENTI As String = "ListaClienti"
    Public Const FORNITORI As String = "ListaFornitori"
    Public Const E_COD_ARTICOLI As String = "ElencoCodiciArticoliBaseOpzioneDesc"

    'giu101120 per DataView da caricare nella griglia cosi da poter fare sort e altro meglio della ListaArr
    Public Const GRIDVIEWDESTCF As String = "GridViewDestCF"

#End Region

#Region " Definizioni CST per la stampa Documenti"
    Public Const CSTObjReport As String = "ObjReport"
    Public Const CSTDsPrinWebDoc As String = "DsPrinWebDoc"
    '- SWScontiDoc per prendere il report con la colonna Sconti oppure senza (Nato per i Preventivi)
    Public Const CSTSWScontiDoc As String = "SWScontiDoc"
    Public Const CSTSWRitAcc As String = "SWRitAcc"
    'Se devo stampare la Conferma del documento
    Public Const CSTSWConfermaDoc As String = "SWConfermaDoc"
    Public Const CSTDsRitornoEtichette As String = "RitornoEtichette"
    Public Const CSTDsPrepEtichette As String = "DsPrepEti"
    Public Const CSTChiamatoDa As String = "ChiamatoDa"
    Public Const CSTRitornoDaStampa As String = "RitornoDaStampa"
    Public Const CSTFinestraChiamante As String = "FinestraChiamante"
    Public Const CSTTASTOST As String = "TASTOST"
    'giu200520
    Public Const CSTTIPOELENCOSCATT As String = "TIPOELENCOSCATT"
    Public Enum TIPOELENCOSCATT
        ScadAttivita = 1
        ScadAttivitaPag = 2
        Verbale = 3
        Proforma = 4
        ElArtCliRespVis = 5 'giu200622
        StatCMRegPrCCliStato = 6 'giu311023
    End Enum
#End Region

End Class
