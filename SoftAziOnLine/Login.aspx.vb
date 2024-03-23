Imports It.SoftAzi.Model.Facade 'Ho tutte le funzioni es. get_Operatori
Imports It.SoftAzi.Model.Entity.OperatoriEntity
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.Integration.Dao.DataSource
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Imports System.Configuration

Partial Public Class Login
    Inherits System.Web.UI.Page

    Protected WithEvents ddL_Ditte As DropDownList
    Protected WithEvents ddL_Esercizi As DropDownList
    Protected WithEvents SqlDataSource1 As SqlDataSource
    Protected WithEvents SqlDataSource2 As SqlDataSource
    Private NDitte As Integer = 0 'giu120417 per visualizzare LOGO SOFT SE LE DITTE SONO PIU' DI UNA
    'giu041122
    Dim composeChiave As String = ""
    Dim myObject As Object = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
#Region "Note modiche"
        'giu311018 nel caso servisse sapere il CLIENTE
        'Dim myAzi As New It.SoftAzi.SystemFramework.ApplicationConfiguration
        'lblRelease.Text += " (" & myAzi.DbCliente.Trim & ")"
        ''myAzi.DbPassword.Trim   CONTIENE LA PASSWORD IN CHIARO PER ACCEDERE AL DB SQL
        'If myAzi.DbCliente.Trim = myAzi.DB_KEYPWD_IREDEEM Then
        '    lblRelease.Text += " ( NO " & myAzi.DB_KEYPWD_IREDEEM.Trim & ")"
        'Else
        '    lblRelease.Text += " ( SI " & myAzi.DB_KEYPWD_IREDEEM.Trim & ")"
        'End If
        '-
        'giu150518 ATTIVARE LE SUCCESSIVE RIGHE QUANDO LE MODIFICHE SONO PER IL TEST
        ' in WEBCONFIG c'èil parametro DEBUG, se è True si attivano altrimenti No
        'giu220224 DEBUG FINO A CHE NON E' A REGIME
        '''MessaggioInfo1.Visible = True
        '''MessaggioInfo2.Visible = True
        '''MessaggioInfo3.Visible = True
        '''MessaggioInfo4.Visible = True
        '''MessaggioInfo5.Visible = True
        '----------------------
        '---------------------------------------------------------------------------
        'lblRelease.ToolTip = "Ultima modifica 21 DICEMBRE 2014: Fatture per servizi (Ordine)"
        'lblRelease.ToolTip = "Ultima modifica 08 GENNAIO 2015: Sconto fornitore per articolo"
        'lblRelease.ToolTip = "Ultima modifica 18 GENNAIO 2015: Prezzo listino/Acquisto quando aggiorno la riga rimette quello memorizzato in anagrafica o listino"
        'lblRelease.ToolTip = "Correzione gestione allestimento - Modifica stampa fattura PA con Split Payment"
        'lblRelease.ToolTip = "Modifica stampa fattura PA con Split Payment aggiunta dicitura<br>Fatturazione normale con Split Payment a livello cliente<br>Seleziona articoli di un fornitore in gestione ordini fornitori"
        'lblRelease.ToolTip = "Richiesta tel Cinzia: Modifica ristampa fatture ricercando un determinato articolo"
        'lblRelease.ToolTip = "Richiesta tel Barbara(MABELL): Modifica stampa fatture/NC indicazione totale documento per lo Split payment"
        'lblRelease.ToolTip = "Richiesta tel.30/03/2015 Barbara(MABELL): Modifica stampa fatture/NC (PA) indicazione totale documento per lo Split payment SOLO SE INDICATO IN ANAGRAFICA, ANCHE SE è FATTURA PA - STAMPE VALORIZZAZIONE MAGAZZINO VELOCIZZATE"
        'lblRelease.ToolTip = "Richiesta tel. 01/04/2015 Zibordi: Stampa ordine fornitore in valuta estera"
        'lblRelease.ToolTip = "Richiesta tel Alberto (24/03/2015): Fatturazione parziale ordini per servizi/contratti"
        'lblRelease.ToolTip = "Richiesta Email del 15/04/2015: cambio sigla società da srl a spa e capitale sociale"
        'lblRelease.ToolTip = "Correzioni: stampe di magazzino in PDF "
        'lblRelease.ToolTip = "Articoli aggiornamento piu listini per fornitore - " & _
        '                     "Stampa Listini diverso dal base - " & _
        '                     "Descrizione estesa: cancellazione da ordine (da terminare)- " & _
        '                     "Seleziona articoli per fornitore - " & _
        '                     "Prezzo imputato dopo aggiorna riga viene ripristinato con prezzo Listino"
        'lblRelease.ToolTip = "Modifica indirizzo sede operativa - " & _
        '                     "Descrizione estesa: cancellazione da ordine (da terminare)"
        'lblRelease.ToolTip = "Errore Ricalcolo giacenze - " & _
        '                     "Descrizione estesa: cancellazione da ordine (da terminare)"
        'lblRelease.ToolTip = "Statistiche Venduto/Fatturato: per categoria cliente - Regione o Agente raggruppate per Fornitore - " & _
        '                     "Descrizione estesa: cancellazione da ordine (da terminare)"
        'lblRelease.ToolTip = "Release: 08 MAGGIO 2016 - Carico Lotti/N° Serie da file CSV / Lettore ottico"
        'lblRelease.ToolTip = "Release: 12 GENNAIO 2017 Corretto errore: Gestione righe estese prodotti"
        'lblRelease.ToolTip = "Release: 28 APRILE 2017: Sviluppo nuove funzioni: Destinazione Merce in elenco - " & _
        '                     "Ricerca documenti collegati - " & _
        '                     "Gestione anagrafiche provvisorie per non creare doppioni di P.IVA / C.F. - " & _
        '                     "Errore stampa scheda contabile in errore per data documento errata"
        'lblRelease.ToolTip = "Richiesta EMail del 15/06/2017: Attivare la ricerca dei coumenti, dove è presente, " & _
        '                     "dei campi Destinazione(1) - Destinazione(2) - Destinazione(3)"
        'lblRelease.ToolTip = "Richiesta Tel. Zibordi del 19/07/2017: Nuova stampa Articoli per fornitore, Ordine per codice o descrizione - " & _
        '                     "Dettaglio stampa: Cod.Art. / Descrizione / Prz.Acq. / Sc.Forn. / Prz.Netto / Ult.Prz.Acq. / Data Ult.Acq."
        'lblRelease.ToolTip = "Richiesta Tel.Erika del 28/08/2017 - Nuova statistica: Venduto/Fatturato per regione/provincia/categoria/cliente/articolo"
        'lblRelease.ToolTip = "Richiesta Tel.Elena: modifica statistiche ordinato cliente aggiungere dove manca al articolo"
        'lblRelease.ToolTip = "Richiesta Tel.Erika del 12/10/2017: corretto errore selezione AliquoteIVA, non presenta il codice Aliquota"
        'lblRelease.ToolTip = "Release: 2 NOVEMBRE 2017 16.00 - Richiesta Tel.Zibordi del 27/10/2017: stampa Preventivi/Offerte e Ordini: riportate iniziali dell'operatore"
        'lblRelease.ToolTip = "Release: 15 NOVEMBRE 2017 12.00 - Richiesta Tel.Elenza del 14/11/2017: lentezza in gestione anagrafica Clienti/Fornitori per la destinazione merce"
        'lblRelease.ToolTip = "Release: 25 GENNAIO 2018 - Valorizzazione Magazzino FIFO reggruppato per fornitore - DA TERMINARE: Ritenuta d'acconto - SPLITIVA memorizzato nel documento"
        'lblRelease.ToolTip = "Release: 23 FEBBRAIO 2018 - Stampa DDT: modifica note per pagamento in contrassegno e totale da pagare considera anche lo SPLIT PAYMENT 
        '- DA TERMINARE: Ritenuta d'acconto"
        'lblRelease.ToolTip = "18/05/2018 BLOCCO ALLESTIMENTO CON QTA' ZERO - SVILUPPO MODIFICHE ALLESTIMENTO ORDINI - MODULI PR/OR/DT/FC P.IVA E C.F. Erika120418"
        'lblRelease.ToolTip = "Release: 26/11/2018 21.30 - Gestione scadenze prodotti consumabili - Fattura AC, Scarico Giacenze - Margine fatturato Cliente (FIFO)"
        'lblRelease.ToolTip = "Release: 01/12/2018 15.00 - Modifica evasione documenti: calcolo della quantità inviata/evasa considerare anche la descrizione prodotto a parità di codice"
        'lblRelease.Text = "Release: 05/01/2019 00.40 - Modifiche riguardo le Fatture Elettroiche e Prodotti Consumabili: Email errate controlli ulteriori"
        'lblRelease.Text = "Release: 09/01/2019 19.15 - Modifiche riguardo le Fatture Elettroiche: Lunghezza testo Riferimento: 20 car. x FC/PA altrimenti 100 car."
        'lblRelease.Text = "Release: 15/01/2019 00.10 - Modifica stampa Fatturato per Agente/Fornitore/Articolo : aggiunto filtro REGIONE"
        'lblRelease.Text = "Release: 19/02/2019 18.40 - gestione campo riferimento con limite di 20car per tutti i documenti - Attivato campo data riferimento 
        '- controllo stampa documenti: avvolte non stampa i codici CIG/CUP - Controllo formale CF se presente - Email articoli consumabili: corretto la creazione - Ok Stampa Rif.DOC."
        ' ''lblRelease.Text = "Release: 10/03/2019 12.55 - Corretto ricerca TipoPag. in anagrafiche Clienti/Fornitori - " & _
        ' ''"Articolo Tipo:9 Non tiene conto della giacenza e riordino - " & _
        ' ''"Stampa e gestione documenti: Bollo,Cod.Destinatario,Copia non valida - " & _
        ' ''"Modifiche ricerca documenti - "
        'lblRelease.Text = "Release: 15/03/2019 12.37 - Possibilità di VISUALIZZARE DOCUMENTI di esercizi precedenti all'esercizio selezionato. - <br> " & _
        '                  "18/03/2019 19.15 - Stampa Documenti non riportare il Totale merce ma il Totale Imponibile Merce"
        'lblRelease.Text = "Release: 22/03/2019 23.15 - Stampa documenti TUTTI: Corretto problema quando ci sono più pagine, il dettaglio stampa su tutto il foglio. - <br> " & _
        '                  "Stampa documenti: Offerte e Ordini a fornitori al fondo inserita frase riguardo la Privacy. - <br> " & _
        '                  "Release: 26/03/2019 23.15 - Stampa Nota Credito correzione."
        '"Release: 15/04/2019 00.05 - Corretto stampa elenchi Anagrafiche Clienti/Fornitori.<br>" & _
        '                  "Nuovo Campo tabellare Linea prodotto in anagrafica Articoli.<br>" & _
        '"Release: 18/04/2019 19.15 - Corretto errore in gestione documenti quando l'articolo non è presente nel listino di vendita.<br>" & _
        'lblRelease.Text = "Release: 17/04/2019 18.45 - Visualizza dettaglio documenti ricercati da Documenti collegati.<br>" & _
        '                  "Release: 19/04/2019 18.35 - Stampa documenti ricercati da Documenti collegati.<br>Ricerca Clienti per:<br>Email (tutte),Codice IPA / Destinatario.<br>" & _
        '                  "Release: 23/04/2019 18.20 - Stampa documenti ricercati da Elenco Documenti con esercizio diverso da quello corrente.<br>" & _
        'Acconti e Fattura a zero (meno acconti): 26/04/2019(3h) - 29/04/2019(6h) - 30/04/2019(5h) - 02/05/2019(5h) 
        '- 03/05/2019(4h) OK RAPPORTINO 06/05/2019 15 ORE A 50,00 TOTALE 750,00 EURO + IVA
        'lblRelease.Text = "Release: 05/05/2019 22.30 - Qtà Inviata - Acconti e Fattura a zero (meno acconti) 
        '<br>- Correzione errore emissione Fattura da ordine e ripristino dopo cancellazione della fattura.<br>" & _
        '"SVILUPPO: 07/05/2019: NOTA!!: Lanciare ricollega VISTE per DocumentiD_ALLANNI<br>"
        'lblRelease.Text = "Release: 20/05/2019 20.15 - Modifica stampa Ordinato per articolo/cliente: NUOVI filtri per Evaso/Da evadere/Parzialmente evaso....<br>" & _
        '                  "Modifica funzione DocumentiD_ALLANNI per determinare lo stato e la quantità evasa di un documento.<br>" & _
        'lblRelease.Text = "Release: 01/06/2019 08.30 - Possibilità di emettere documenti con totale documento a ZERO <br>- Ok anche la fatturazione con il totale da pagare uguale a ZERO"
        'lblRelease.Text = "Release: 05/07/2019 12.50 - Modifica Stampa Offerta senza sconti per indirizzo errato <br>" & _
        '                  "- Modifica Gestione articoli - quando si modifica un articolo non presente nel listino non viene inserito" & _
        '                  "- Risolto problema stampa in PDF stampa disponibilita"
        'lblRelease.Text = "Release: 12/07/2019 18.00 - Modifica Gestione articoli - quando si modifica il prezzo di un articolo non aggiorna il LISTINO VENDITA se è già presente"
        'lblRelease.Text = "Release: 30/07/2019 11.20 - Modifica Gestione articoli - Inserimento Articolo nel listino se non presente nel listino di BASE <br>" & _
        '                  "- Gestione Listini: disabilitato la funzione INCLUDI TUTTI gli articoli esclusi, rimane invariato l'inclusione sigolo articolo"
        '"Correzioni al 20/12/2019 10.00: Stampa Ordinato Articoli/Cliente/Tipo Evasione <br>"
        '"Correzioni al 19/12/2019 17.00: Errore ricerca ordini quando si ritorna da stampa documenti collegati <br>" & _
        '"Correzioni al 19/12/2019 13.00: AutopostBack ottimizzato nella gestione documenti <br>" & _
        '"Correzioni al 06/12/2019: Gestione causali magazzino non si posiziona bene sul primo rk dopo una ricerca <br>" & _
        '"Correzioni al 03/12/2019: Prodotti Consumabili (ALERT Sscadenze): Esclusione Clienti con segnale InvioEmail=No <br>" & _
        '"Correzioni al 28/11/2019: Fattute per ACCONTO/SALDO, segnale SI in automatico quando viene emesse una nuova fattura <br>" & _
        '"Correzioni al 23/11/2019: Nuova anagrafica provvisoria errore nel riportare il codice selezionato/generato <br>" & _
        '"AGOSTO 2019: Correzioni: Anagrafiche provvisorie - Numerazione Spedizioni <br>" & _
        '"- DESTINAZIONE MERCE: Non permettere la duplicazione <br>" & _
        '"Release: 30/07/2019 18.50 - Modifica Statistica articoli venduti / fatturati per cliente / articolo: <br>" & _
        '"Possibilità di stampare i soli clienti a cui è stata inviata E-Mail ALERT Consumabili con esito CONCLUSA <br>" & _
        '"e non riportare in stampa clienti a cui non è previsto l'invio dell'E-mail. <br>" & _
        '"Correzioni al 16/01/2020 16.00 Stampa Ordinato Articolo/Ordine/Tipo Evasione aggiunto lo stato Ordine" & _
        'lblRelease.ToolTip = "Correzioni al 18/01/2020 11.00 NUOVA Statistica CLienti Nuovi anno Prec.Succ e totali complessivo per anno o tutti gli esercizi e suddiviso per regione. <br>" & _
        '"(Elena 17012020) Non aggiorno il campo ModificatoDa quando l'ordine è nello stato di ALLESTIMENTO" & _
        '"(Elena 17012020) Ottimizzato stampe documenti - OK riportarlo nelle funzioni mancanti E CONTRATTI"
        'lblRelease.Text = "Release: 23/10/2019 22.25 (...) Modifiche al 05/04/2020 19.40"
        'lblRelease.ToolTip = "05/04 - Disponibilità/Valorizzazione di Magazzino: inserito filtro per Fornitore ** " & _
        '                     "11-18/03 Correzione stampa disponibilita' di magazzino e valorizzazioni di magazzino ** " & _
        '                     "10/03 MODIFICA Stampa fatturato clienti per categoria sintetico: aggiunto totale clienti ** " & _
        '                     "09/03 MODIFICA Stampa fatturato clienti con margine: incluso il controppo fattura per acconto e deduzione per acconto ** " & _
        '                     "07/03 MODIFICA Stampa fatturato clienti con margine: righe dettaglio senza codice e importo negativo non considerate ** " & _
        '                     "Dettaglio documenti: codice articolo che si sovrappone alla descrizione - disposto su piu righe <br> " & _
        '                     "(Elena 11022020) Nuova stampa movimenti con Serie/lotti Stampare l'ordine collegato al DTT <br> " & _
        '                     "ATTIVATO ricerca abbinata alla LOCALITA quando il tipo di ricerca è per Rag.Soc. / Denominazione"
        '@@@@@@ IN CORSO "Stampa PDF DOCUMENTI solo per il download"

        'lblRelease.ToolTip = "04/08/2020 Corretto aggiornamento Contratti ** 19/06/2020 15.33 EVASIONE Attività da Responsabili Area/Visita ** 05/06/2020 18.50 FATTURAZIONE SCADENZE CONTRATTI **  " & _
        '"27/05/2020 11.30 Correzione errori EMISSIONE documenti con IVA diverse ** 27/05/2020 08.35 Correzione errori nelle statistiche Venduto per la ricerca dei Clienti ** " & _
        '"24/05/2020 23.15 Modifica Elenco Scadenze per Resp.Area/Tutti (Serie/Scadenze accorpate) ** " & _
        '"23/05/2020 02.50 Corretto ricerca Destinazioni Merci ** " & _
        '"19/05/2020 22.50 Modifiche Stampe: Proforma, Verbale e Estrazione Elenco Scadenze attività in EXCEL ** " & _
        '"14/05/2020 23.30 Corretto errore inserimento Contratti - Sblocco Resp.Area Elenco Scadenze Attività **  " & _
        '"13/05/2020 21.35 Stampa Elenco Scadenze + XLS ** 05/05/2020 22.00 Stampa Verbale ** 24/04/2020 **  " & _
        '"Stampa Proforma Contratto Ok ** 22/04/2020 19.15: Attivato accorpa scadenze fatturazione per ANNO ** " & _
        '"Release: 21/04/2020 12.20: N° Serie colleato alle Rate Scadenza **  Calcolo rate per Periodo al Costo effettivo / 1 Fattura per Anno * " & _
        '"Release: 17/04/2020 13.25: Problema stampa DDT Dal Al / Fatturazione documenti emessi * "
        ' ''"GESTIONE CONTRATTI DI MANUTENZIONE E DI TELECONTROLLO * " & _
        ' ''"dal 09/12/2019 al 11/12/2019ore: 15.00 * " & _
        ' ''"16/12/2019 ore: 5.00 Totale:  20.00 * " & _
        ' ''"17/12/2019 ore: 3.30 Totale:  23.30 * " & _
        ' ''"18/12/2019 ore: 4.30 Totale:  28.00 * " & _
        ' ''"19/12/2019 ore: 3.00 Totale:  31.00 * " & _
        ' ''"20/12/2019 ore: 2.00 Totale:  33.00 * " & _
        ' ''"09/01/2020 ore: 2.00 Totale:  35.00 * " & _
        ' ''"10/01/2020 ore: 4.30 Totale:  39.30 * " & _
        ' ''"11/01/2020 ore: 3.00 Totale:  42.30 * " & _
        ' ''"13/01/2020 ore: 6.00 Totale:  48.30 * " & _
        ' ''"14/01/2020 ore: 1.30 Totale:  50.00 * " & _
        ' ''"25/01/2020 ore: 3.00 Totale:  53.00 * " & _
        ' ''"28/01/2020 ore: 3.30 Totale:  56.30 * " & _
        ' ''"29/01/2020 ore: 6.30 Totale:  63.00 * " & _
        ' ''"30/01/2020 ore: 4.00 Totale:  67.00 * " & _
        ' ''"31/01/2020 ore: 3.00 Totale:  70.00 * " & _
        ' ''"01/02/2020 ore: 2.00 Totale:  72.00 * " & _
        ' ''"04/02/2020 ore: 2.00 Totale:  73.00 * " & _
        ' ''"05/02/2020 ore: 2.00 Totale:  75.00 * " & _
        ' ''"06/02/2020 ore: 2.00 Totale:  77.00 * " & _
        ' ''"07/02/2020 ore: 3.00 Totale:  80.00 * " & _
        ' ''"10/02/2020 ore: 3.00 Totale:  83.00 * " & _
        ' ''"11/02/2020 ore: 2.00 Totale:  85.00 * " & _
        ' ''"12-19/2020 ore:10.00 Totale:  95.00 * " & _
        ' ''"20-23/2020 ore: 5.00 Totale: 100.00 * " & _
        ' ''"24-25/2020 ore: 4.00 Totale: 104.00 * " & _
        ' ''"26/02/2020 ore: 3.00 Totale: 107.00 * " & _
        ' ''"27/02/2020 ore: 6.00 Totale: 113.00 * " & _
        ' ''"28/02/2020 ore: 7.00 Totale: 120.00 * " & _
        ' ''"02/03/2020 ore: 6.00 Totale: 126.00 * " & _
        ' ''"06/03/2020 ore: 2.00 Totale: 128.00 * " & _
        ' ''"12/03/2020 ore: 6.00 Totale: 134.00 * " & _
        ' ''"13/03/2020 ore: 6.00 Totale: non in offerta: Stampe in PDF - Abbuoni * " & _
        ' ''"18-23/03/2020 ore: 10.00 Totale: 144.00 * "

        ' ''lblRelease.Text = "Release 04/11/2020 18.30 Corretto calcolo Tot.Rate scadenza per periodo + 29/10/2020 23.00 Controllo Valorizzazione Magazzino " & _
        ' ''"- 25/10/2020 Gestione magazzini - Gestione documenti dettaglio: Nuova riga sopra - Nuova Statistica Fatturato Anno/Mese"
        'lblRelease.Text = "Release 09/11/2020 18.10 Statistica Fatturato Articoli Annuo (In Corso/Precedente) nei totali tolto la somma e % delle Quantita' 
        '- Correzione Contratti Aggiornamento Data Scadenza Check e Data Inizio"
        'lblRelease.Text = "Release 11/11/2020 17.10 Stampa Contratto proforma per il N° Serie non stampato 
        '- Dettaglio documenti visualizza tutte le voci anche in visualizza - Ordinamento Elenco destinazioni Merce per Sigla o altro campo visualizzato per una rapida ricerca e scelta."
        'lblRelease.Text = "Release 17/11/2020 18.50 Nuova selezione Elenco Ordini: In Contratto visualizzati solo in *Tutti* e *In Contratto*"
        'lblRelease.Text = "Release 19/11/2020 10.35 Copia documenti - Aggiornamento TIPO PAGAMENTO in essere al momento della copia."
        'lblRelease.Text = "Release 23/11/2020 12.00 Nelle funzioni dove richiede la scelta del Magazzino, proporre sempre il Magazzino 1 (TORINO)."
        'lblRelease.Text = "Release 25/11/2020 16.05 Impegno Giacenza Ordini: Deve tener conto del magazzino."
        'lblRelease.Text = "Release 18/12/2020 18.20 Controllo e blocco documenti: Qtà evasa maggiore della Qtà ordinata."
        'lblRelease.Text = "Release 31/12/2020 03.00 Valorizzazione FIFO escluso articoli con Tipo articolo diverso da 0."
        'lblRelease.Text = "Release 31/12/2020 10.45 Valorizzazione FIFO,LIFO,... escluso articoli con Tipo articolo diverso da 0."
        'lblRelease.Text = "Release 13/01/2021 15.10 Regime IVA Cliente se valido aggiorna l'IVA Articoli documento."
        'lblRelease.Text = "Release 20/01/2021 13.00 Stampa elenco Anagrafiche Clienti: Nuovo filtro per Agente."
        'lblRelease.Text = "Release 03/02/2021 23.05 Stampa articoli fornitori CONFRONTO con Storico Prezzi."
        'lblRelease.Text = "Release 05/03/2021 17.45 Fatture: abilitato fino a 12 rate e modifica modulo. "
        '"- Stampa Listini Vendita: Selezione per Singolo Fornitore - Clienti: Riferimento a 150 Car. Modifica stampa Analitica con Riferimento"
        'lblRelease.Text = "Sviluppo 23/10/2021 10:00"
        'lblRelease.ToolTip = "17/03/2021: Clienti: Anagrafica Clienti: Nuovo Preventivo,Ordine<br/>Modifica anagrafica se chiamato da Documenti" & _
        '"<br/>Nuova sezione 'Ordini pregressi' nella gestione Anagrafiche Clienti<br/>"
        '22/03/2021 Contratti: Copia Contratto e abbinamento a Ordine, Scelta Magazzino scarico Giacenze" & _
        '"<br/>24-28/03/21 10.15: Fatturazione contratti: Oltre alla Rata da fatturare riportare anche le singole voci evase (Check,Elettrodi,Batterie)" & _
        '"<br/>proporre sempre come Fattura accompagnatoria e Scarico Merci" & _
        '"<br/>25/03/2021 C.Commessa nei documenti e in stampa" & _
        '"<br/>12-21/04/2021 Preventivi per Agente/Cliente /Preventivo - Stato:[Conf./Da Conf... Conteggio Totali]" & _
        '"<br/>20-21/04/2021 Preventivi per Lead_Source/Cliente /Preventivo - Stato:[Conf./Da Conf... Conteggio Totali]" & _
        '"<br/>23-24/04/2021 Venduto per Lead_Source Analitico e Sintetico (Regione-Provincia)" & _
        '"<br/>21/07/2021 Corretto dati propositivo del Magazzino contratti" & _
        '"<br/>22/07/2021 Corretto Fatturazione contratti:seleziona solo attività periodo" & _
        '"<br/>28/07/2021-24/08/2021 Contratti: Inserito tasto ""Documenti collegati""" & _
        'lblRelease.Text = "Sviluppo 20-23/10/2021 10:50" 23/10/2021 Anagrafica Clienti: Preventivi pregressi (Erika)" & _
        '"<*>22/10/2021 Controlli Riferimento e IPA Clienti OBB. (Erika) no per Utente MAGAZZINO" & _
        '"<*>20/10/2021 Ottimizzato velocità apertura elenco documenti"
        'lblRelease.ToolTip = "08/11/2021 Stampa Ordinato Clienti per articolo/cliente/Tipo Evasione [Fornitore]: Analitico/Sintetico (Zibordi)" & _
        '"<*>08/11/2021 No Controlli Riferimento nei Ordini Fornitori"
        'lblRelease.ToolTip = "Sviluppo 21/11/2021 18:55: 20/11/2021 Stampa Valorizzazione Magazzino FIFO sintetico per Fornitore."
        'lblRelease.Text = "Release 23/12/2021 15:25"
        'lblRelease.ToolTip = "23-20-18-16/12/2021 - Corretto Gestione Preventivi/Ordini da Anagrafiche clienti" & _
        '" - 14/12/2021 - Modifica Gestione Manutenzione Contratti " & _
        '" - 12/12/2021 Stampa Sintetica Fatturato per cliente/N° documento con Margine per Fornitore " & _
        '" - 21/11/2021 Preventivi per Agente-Lead Souce/Cliente/Preventivo - Stato:[Conf./Da Conf... Conteggio Totali]: " & _
        '" - Selezione periodo dalla data alla data - Selezione articoli + Fornitore" ' - Abilitato anche ricerca per singolo Cliente ora invisibile ma se lo chiedesse"
        '"28-29/12/2021 - Gestione Ordini Nuovi Campi (P)ag.Effettuato (PA)g.Anticipato (L)ista - " & _
        '"Modifica stampa Ordini per Articolo/Cliente Tipo Evasione: Riportare i Nuovi campi di cui sopra e la Sigla di chi ha inserito l'Ordine"
        '"19/01/2022 1:55 - Modifiche Contratto manutenzioni: Abilitato Modelli C1,C2 - Trasferimento giacenze inizio anno: Escluso TUTTI - " & _
        '"21/01/2022 - Documenti/Contratti: Modifica Anagrafica abilitato PEC-EMail - Contratti: 1 Bozza con Note Contratto/Intervento - " & _
        '"21/01/2022 - Contratti: Aggiornamento modifiche attivita, chiede CONFERMA aggiornamento e aggiunta NOTE INTERVENTO - " & _
        '"22/01/2022 04:15 - Il responsabile AREA deve visualizzare sia le sue manutenzioni che quelle del responsabile visita (modifica chiesta da Elena in data 10/11 ma che non vedo in area test) - " & _
        '"26/01/2022 18:40 - Contratti: Possibilità di collegare più contratti ad un medesimo ordine - La creazione del contratto a partire dall’ordine deve cambiare lo status del contratto in EVASO - Documenti collegati da Documenti deve comparire anche i Contratti" & _
        '"02/02/2022 - Corretto cambio stato da elenco Documenti bloccati - " & _
        '"31/01/2022 18:20 - Corretto errore Contratti - Modifica stampe Fatturato Clienti con Margine: Aggiunto raggruppamenti Agente e Regione - " & _
        '"10/02/2022 Modifica Modello per singolo Apparecchiatura - Aggiornamento Luogo.App. in automatico per tutti i periodi - "
        '"12/02/2022 1:00 Varie modifiche Gestione contratti Resp. Visita e modifica Ins.nuova voce nel Periodo - "
        '"15/02/2022 17:30 Correzione Em.Fatt. su Ordini da Gestione Anagrafica Clienti - "
        '"Release 19/02/2022 23:30 - Blocco pagine quando è in elaborazione - 15/02/2022 19:30 Documenti collegati nella gestione Ordini Fornitori - "
        '"22/02/2022 - Anagrafica Clienti - Correzioni quando è chiamato per visualizzare Scheda - "
        '"Release 12/03/2022 01.25 - Modifiche evasione attività Contratti assistenza: Note puntuali al N° Serie - "
        '"Release 18/03/2022 12.50 - Modifiche Fatturazione Scadenze Attività Contratto anche superiore alla data di Fatturazione - "
        '"Release 22-23/03/2022 18.15 - Corretto Visualizza Clienti da Gestione Documenti e Nuovo Documento riportare il cliente scelto da Anagrafica Clienti - "
        '"Release 07/04/2022 00.55 - Aggionamento Note Intervento - Segnalazione in fatturazione scadenze con anno superiore all'anno scadenza di Fatturazione - "
        '"Corretto funzione Genera Attività periodo: Data Scadenza Check pari alla minor scadenza di Batteria,Elettrodo, ...... - "
        '"Release 11/04/2022 15.30 Ottimizzato controlli avvio funzioni con blocco pagina, per evitare di rilanciare piu volte la funzione"
        '"Release 15/04/2022 10.05 - Gestione Anagrafica Clienti: corretto problema visualizzazione Preventivi e Ordini pregressi"
        'lblRelease.Text = "Release 06/05/2022 17.20 - Ricerca e seleziona articoli: verifica ordinamento avvolte non funziona" & _
        '                     " - 12/05/2022 Modifica MenuCA modifica Funzione di estrazione scadenze solo Contratti Manutenzioni"
        'lblRelease.Text = "Release 02/06/2022 23.40 - Modifica lunghezza N° Serie/Lotto 30 caratteri"
        'lblRelease.Text = "Release 11/06/2022 20.30 - Modifica funzione Attività da fatturare : visualizza anche le attività parzialmente evase"
        'lblRelease.Text = "Release 17/06/2022 17.45 - Modifica funzione Statistica Preventivo per Agente/Cliente/Preventivo - Stato:(conf/da conf...) Conteggio totali - " & _
        '"Aggiunta selezione per Regione/provincia"
        'lblRelease.Text = "Release 25-29/06/2022 15.40 - Nuova funzione Stampa elenco Ordinato per Articolo/Cliente/Responsabile Visita"
        'lblRelease.Text = "Release 05-06/07/2022 17.30 - Modifica Stampa Preventivi per Agente/Cliente/Preventivo - Stato:[Conf./Da Conf... Conteggio Totali]: " & _
        '    "Solo analitico/Tipo Ordine di stampa/Riporto selezioni fatte per la stampa"
        'lblRelease.Text = "Release 08-09-11(4h)-14-15-18-19(2h)-20(4h)21(4h)22(4h)23(2h)24/07/2022 ??.?? - Nuove funzioni: Creazione file contenente Lista DDT in spedizione(14.00-14.00)" TOTALE ORE 18.00
        'release 28/07/2022 controllo accesso stesso browser con 2 utenti diversi BLOCCA
        'IN GESTIONE DOCUMENTI SE LA FATTURA E' ACCONTO NON PUO MAI SELEZIONARE SCARICO GIACENZE
        '"25/07/2022 17.55 Nuove funzioni: Creazione file contenente Lista DDT in spedizione - "
        '"27/07/2022 19.00 Corretto funzione aggiornamento automatico stato Ordine quando si evade con DDT - "
        '"30/07/2022 23.40 Modifica ricerche scadenze attività Contratto e possibilità di modificare le rate di scadenze - " 
        '"02/08/2022 23.05 Stampa Etichette sovracollo formato A4 solo per Bologna non visibile a Utente Magazzino"
        '09/08/2022 16.00 Controllo obbligo N° Telefono per singolo DDT in spedizione - "
        'lblRelease.Text = "Release 07/09/2022 11.50 Modifica Gestione Contratti: abilitato campi CIG/CUP alla modifica - " & _
        '"06/09/2022 16.20 Correzione stampe ordini Fornitore: Non riporta in stampa correttamente l'ordine chiuso non evaso - " & _
        '"24/08/2022 11.20 Controllo Cliente NON FATTURABILE, segnalazione in: Allestimento / Em.Fattura / Em.DDT / Contratto e da Preventivo>>Ordine - " & _
        '"09/08/2022 16.00 Controllo obbligo N° Telefono per singolo DDT in spedizione"
        'lblRelease.Text = "Release 10/10/2022 15.05 Modifica Gestione Spedizione file DDT: Aggiunta la Profondita / Modifica campo Località senza la Pr. e EUR "
        '+ Format(Now, "dddd d MMMM yyyy, HH:mm:ss") & 
#End Region
        lblRelease.Text = "Release 23/03/2024 18:20 - Stampe in PDF senza salvare dati sul SERVER - Nuova funzione cambio Responsabili Area/Visite nei contratti"
        lblRelease.ToolTip = "10-11/03/2024 - Gestione anteprima Stampe ed esporta in PDF - 08/03/2024 12.45 - Limite stampe: modifica controllo errore in fase di stampa dopo la modifica del n.limite stampe nel registro di sistema del Server" +
        " - 07/03/2024 Ultima Sessione solo con SESSION e non IP - 06/03/2024 Ottimizzato accesso al gestionale - 05/03/2024 Contratti - Gestione evasione attività SW Esterno: corretto errore aggiornamento Note attività" +
        " - 29/02/2024 Elenco Scadenze Attività contratti: Segnala Scadenze Anno disabilitato -  Collaga Viste Ultimo Anno: Aggiunto Regioni,Categorie e Agenti " +
        " - 26/02/2024 Corretto documenti/Causale Magazzino: movimenti tra Magazzini, non deve cambiare la Causale 2 e Magazzino 2 in aggiornamento" +
        " - 20/02/2024 Modifica Nuova stampa Elenco DDT per Magazzino/Causale: Aggiunto Dal/Al Magazzino quando è un Trasferimento" +
        " - 15/02/2024 Gestione documenti/Causale Magazzino: movimenti tra Magazzini, abilitato oltre alla 2 Causale anche il 2 Magazzino" +
        " - Corretto Gestione ricerca Cliente da gestionale esterno: la Combo clienti non veniva caricata correttamente" +
        " - 11/02/2024 Corretto errore in aggiornamento Parametri Generali " +
        " - 26/01/2024 12.00 Modifica funzione carico N° Serie e N° Lotto in DDT/FA/MM/... in automatico: Aggiunto anche il N° Lotto " +
        " - 25/01/2024 18.10 Nuova Stampa: Elenco DDT Clienti per Magazzino/Causale - Controllo Email: implementato nuova verifica " +
        " - 21/01/2024 14.00 Corretto funzione rigenerazione periodo attività, dopo modifica N° Serie " +
        " - 18/01/2024 17.20 Modifica Controllo Totale Documento con Totale Rate in emissione Fattura da Contratto" +
        " - 14/01/2024 19.35 Corretto errata Connessione archivi su esercizi diversi - Doppio Login stesso Browser - Export PDF" +
        " - 08/01/2024 15.50 Sviluppo nuove funzioni Riunione Bologna 26/09/2023" +
        " - 04/10/2023 Corretto carico Serie/Lotti con lettore: superato i 9 pezzi non permetteva piu di caricare i restanti Lotti" +
        " - 22/09/2023 13.15 Corretto Gestione attività Contratti (Esterno): Ricerca attività non presentava correttamete tutte le attività da evadere del Contratto cercato" +
        " - 08/09/2023 12.10 - 03/09/2023 23.20 - Estrazione foglio EXCEL Installato per Cliente/articolo" +
        " - 31/08/2023 19.00 - Stampa DDT/Fatture con lotti nuova versione" +
        " - 28/08/2023 18.25 - Allestimento Ordini: corretto nuovo N° Spedizione  - 06/08/2023 15.15 - Elenco Preventivi per Responsabili Visita Gestione Attivita Contratti" +
        " - 21-26/07/2023 18.35 Anagrafica Fornitori IBAN per disposizione Bonifici come in COGE Aggiunto SWIFT e Controllo lunghezza IBAN" +
        " - 20/07/2023 18.50 Funzione di verifica,modifica e stampa N° Serie / Lotto senza caratteri speciali" +
        " - 18/07/2023 23.00 Contratti eliminati caratteri speciali dal N° Serie per evitare errore stampa / gestione" +
        " - 07/07/2023 12.40 - ELENCO ATTIVITA' IN SCADENZA: Aggiunta N° CONTRATTO (INTERNO/ESTERNO)" +
        " - 29/06/203 19.00 OTTIMIZZATO ricerca contratti Gestione ESTERNA " +
        " - 27/06/2023 14.30 Corretto emissione DDT da Anagrafica Clienti: segnalazione quando mancava il vettore non gestito" +
        " - Allineato controllo come avviene in elenco ordini" +
        " - 26/06/2023 16.00 Corretto modifica dettaglio contratti: Righe periodo non deve aggiornare tutte le righe con il N° Serie/Lotto" +
        " - 23/06/2023 11.35 Corretto Fatturazione scadenze Contratti: dettaglio scadenze per anno e non per data, per evitare che non vengano " +
        "   visualizzate tutte quando le date di scadenze non sono tutte uguali " +
        " - 30/05/2023 17.25 Modifica File spedizioni: N° Telefono lasciare solo caratteri numerici" +
        " - Eliminare in tutti i campi AlfaNumerici i caratteri - e accenti" +
        " - Ristampa documenti (DDT/Fatture) portato limite a 500" +
        " - 22/05/2023 15.35 - Valorizzazione Magazzino solo Articoli con giacenza diversa da ZERO" +
        " - Controllo vettore OBB solo se consegna per Vettore " +
        " - 18-19/05/2023 19.20 Modifica Stampe Fatturato: Aggiunto filtro per Date solo anno corrente" +
        " - 16/05/2023 - Aggiunta colonna J Shipper_Country_Code_Phone - Stampa Elenco Scadenze Contratti in anteprima per poterlo esportare in PDF,Excel,...." &
        " - 08/05/2023 17:05 - Correzione Contratti: controllo caratteri speciali N°Serie nelle funzioni di Aggiornamento/Stampa" +
        " - Modifica creazione file Spedizioni: Vettore Obbligatorio - Eliminato colonna J Shipper_Country_Code_Phone" +
        " - 04/05/2023 18:00 - Modifica file spedizioni e-mail Ilaria" &
        " - 02/05/2023 18.35 Corretto Inserimento/Modifica Destinazioni Merci: se inserisco nuova Destinazione non prende il primo N° Libero" +
        " - 30/04/2023 Stampe di Magazzino aggiunto i ss per evitare sovrapposizione del PDF - Distponibilità di magazzino solo Esercizio corrente" +
        " - 30/04/2023 Ricalcolo giacenze modifica per evitare concorrenza nel ricalcolo" +
        " - 28/04/2023 Modifica gestione contratti ESTERNO: riportare nelle note, email invio Verbale se venisse modificata Rapp.1,00h" &
        " - 26/04/2023 15.55 Correzione Gestione distinta base" &
        " - 22-18/04/2023 Correzione emissione fatture da DDT e Fattura riepilogativa" &
        " - 20/04/2023 10.30 Blocchi: P.IVA/C.F./IPA - Destinazione Merce e/o campi dati corriere in Allestimento,Nuovo DDT.OKR" &
        " - 17-18-19/04/2023 Blocco se manca la destinazione Merce e/o campi dati corriere in Allestimento,Nuovo DDT.OKR" &
        " - 15/04/2023 11.45 Corretto Sessione precedente/scaduta/Cambio connessione WIFI" &
                          " - 11/04/2023 Riportato modifica stampa Elenco Scadenze Attività contratto anche nella sezione Ufficio OKR" &
                          " come per gli Resp.Visita Scadenze anno olre la data richiesta. Rapp.0,30 ore" &
                          " - 10/04/2023 17.15 Documenti collegati da Ordini non vengono visualizzate FC Contratti" &
                          " - 08/04/2023 Corretto errore stampa Verbali in bianco" &
                          " - 03-05-06/04/2023 Modifiche Dati Corriere max 35 caratteri" &
                          " - 22-27-28-29-31/03/2023 Modifiche Carico Lotti/N° Serie con Lettore gestire la Qtà Lotti per riga" &
                          " - 29/03/2023 15.15 Correzioni Anagrafiche Clienti: Destinazione Merce / Altri indirizzi" &
        " - 21/03/2023 15.35 Modifica Blocco Allestimento Ordini, emissione DDT/FATTURE se manca P.IVA/CF/NON FATTURABILE " &
        " - Release 19/03/2023 23.05 Modifica carico N° Serie/Lotto con lettore " &
        " - 13/03/2023 : Modifiche: elenco DDT per file Csv spedizione - Ricalcolo giacenze se esiste in Mag.0 aggiorno" &
        " - Correzioni caricolo N° Serie da file CSV a DDT Lotti" &
        " - Correzioni: 07/03/2023 18.35 ricalcolo giacenze errore in Insert magazzino 0,Cod_Art" &
        " - Correzioni: 06/03/2023 18.50 Stampa elenco scadenze attivita con scadenze anno tutte" &
        " - Correzioni: 02/03/2023 15.30 Elenco Attività scadenze/Fatturare: aggiunto controlli " &
        " - Correzioni: 27/02/2023 18.50 Scadenze Consumabili nelle Note - Elenco Attività scadenze: aggiunto controlli " &
        " *** Gestione Contratti (Resp.Area/Visita): Recupero ultima sessione valida" &
        " - Ordine Elenco PDF/EXCEL Scadenze per Cliente o Data Scadenza" &
        " - Aggiorna Note solo quelle Eseguite/Modificate" &
        " - Aggiunto Referente nella griglia delle scadenze attività" &
        " - Aggiunto Ricerca scadenze attività per ragione sociale Cliente/Sedi oppure N° Contratto" &
        " - Aggiunto Nuovo campo Data Scadenza consumabile" &
        " - Aggiunto Nuovo campo ALLA PRESENZA DI: ... specifico per N° Serie/Lotto" &
        " - Stampo tutti i VERBALI solo se sono stati evasi anche parzialmente" &
        " - Scadenze attività: nuovo CHECK Visualizza Scadenze anno senza tener presente le date di selezione" &
        " - Scadenza Consumabili: Da riportare sempre nelle NOTE Intervento" &
        " - *** Gestione Contratti (Ufficio) Stampo elenco Scadenze: possibilità di scegliere l'ordine per Cliente oppure per Data Scadenza (default Cliente)" &
        " - Referente portato a 500 caratteri prima era 150" &
        " - Creazione rapida del PDF Fattura Contratti" &
        " *** Estratto Conto - Coordinate bancarie" &
        " *** Gestione Spedizione file DDT: N° decimali per il peso e No per il N° Colli, Solo 1 EMAIL Destinazione/Cliente(18/01/2023),Pagamento Contrassegno" &
        " *** Anagrafica Articoli: Nuovi campi per file XML Tipo/Valore - Chiave x XML RiferimentoAmministrativo" &
        " *** Lista di Carico: Aggiunta casella Compilatore per la firma" &
        " *** Ordinato per articolo/cliente/Tipo Evasione: Scelta magazzino assegnato all'ordine"
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '23102019 DA FARE - Numerazione documenti, che sia sequenziale (RICORDATI MYNUMDOC +1
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        If (Not IsPostBack) Then
            Session(CSTCODDITTA) = "z"
            Session("AccessoLogin") = "0"
            Session(SWOP) = SWOPNESSUNA
        End If
        Dim strErrore As String = "" : Dim strValore As String = ""

        ddL_Ditte = CType(Login1.FindControl("ddL_Ditte"), DropDownList)
        ddL_Esercizi = CType(Login1.FindControl("ddL_Esercizi"), DropDownList)
        SqlDataSource1 = CType(Login1.FindControl("SqlDataSource1"), SqlDataSource)
        SqlDataSource2 = CType(Login1.FindControl("SqlDataSource2"), SqlDataSource)
        Dim myEsercizio As String = Now.Year.ToString
        If String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            Session(ESERCIZIO) = myEsercizio
        End If
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDataSource1.ConnectionString = dbCon.getConnectionString(TipoConnessione.dbInstall)
        SqlDataSource2.ConnectionString = dbCon.getConnectionString(TipoConnessione.dbInstall)
        ModalPopup.WucElement = Me
        'GIU230622 PER LA GESTIONE ERRORI DA STAMPE NON PREVISTE PER ESEMPIO O ALTRE ECCEZIONI
        Dim swSessione As String = Request.QueryString("SessioneScaduta")
        If IsNothing(swSessione) Then
            swSessione = ""
        End If
        If String.IsNullOrEmpty(swSessione) Then
            swSessione = ""
        End If
        If swSessione.Trim <> "" Then
            If Mid(swSessione, 1, 1) = "1" Then
                'MessageLabel.Text = "Sessione scaduta: Accesso non autorizzato su una pagina web disponibile solo effettuando il login"
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("ATTENZIONE", "Sessione scaduta: Accesso non autorizzato su una pagina web disponibile solo effettuando il login", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            ElseIf Mid(swSessione, 1, 1) <> "" Then
                If swSessione.Trim <> "0" Then
                    'MessageLabel.Text = swSessione
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("ATTENZIONE", swSessione, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
            End If

        End If
        '-------------------------------------------------------------------------------------
        If (Not IsPostBack) Then 'giu130312 deve farlo solo la prima volta
            MessageLabel.Text = ""
            'giu290412
            strErrore = ""
            If SessionUtility.CTROpTimeOUT(strErrore, "") = False Then
                'MessageLabel.Text = "Errore cancella OperatoriConnessi: " & strErrore
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore cancella OperatoriConnessi: " & strErrore, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            '---------
            Dim DittaSys As New Ditte
            Dim myDitta As DitteEntity
            Dim arrDitte As ArrayList = Nothing
            Try
                arrDitte = DittaSys.getDitte()
            Catch ex As Exception
                'MessageLabel.Text = "Errore accesso alla tabella società: " & ex.Message
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore accesso alla tabella società: " & ex.Message, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End Try

            If Not IsNothing(arrDitte) Then
                If arrDitte.Count = 0 Then
                    'MessageLabel.Text = "Nessuna Società definita."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Nessuna Società definita.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                Else
                    NDitte = arrDitte.Count
                    myDitta = CType(arrDitte(0), DitteEntity)
                    Session(CSTCODDITTA) = myDitta.Codice
                    SetImgAzienda()
                End If
            Else
                'MessageLabel.Text = "Nessuna Società definita."
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Nessuna Società definita.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'GIU051219
            Dim myCodDitta As String = ""
            If IsNothing(Session(CSTCODDITTA)) Then
                Session(CSTCODDITTA) = ""
            End If
            If String.IsNullOrEmpty(Session(CSTCODDITTA)) Then
                Session(CSTCODDITTA) = ""
            End If
            myCodDitta = Session(CSTCODDITTA)
            If myCodDitta.Trim = "" Then 'NON FUNZ LA PRIMA VOLTA
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", "Nessuna Società definita.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                ' ''Exit Sub
            ElseIf myCodDitta.Trim = "00" Then
                MessaggioInfo1.Visible = True
                MessaggioInfo2.Visible = True
                MessaggioInfo3.Visible = True
                MessaggioInfo4.Visible = True
                MessaggioInfo5.Visible = True
            End If
            Dim SWDebug As String = ConfigurationManager.AppSettings("debug")
            If Not String.IsNullOrEmpty(SWDebug) Then
                If SWDebug.Trim.ToUpper = "TRUE" Then
                    MessaggioInfo1.Visible = True
                    MessaggioInfo2.Visible = True
                    MessaggioInfo3.Visible = True
                    MessaggioInfo4.Visible = True
                    MessaggioInfo5.Visible = True
                End If
            End If
            'giu210814
            '=========================
            'giu180412
            '.UserHostName mi ritorna la porta DNS quindi 2 postazioni da TORINO vanno in conflitto
            '.UserHostAddress questo dovrebbe ottenere l'IP REALE DI CONNESSIONE
            '=========================
            If SessionUtility.CTROpBySessionePostazione(Session.SessionID, Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo) = True Then
                If Session("AccessoLogin") = "0" And MessageLabel.Text = "" Then
                    Session("AccessoLogin") = "1"
                    'MessageLabel.Text = "ATTENZIONE, postazione già connessa. Se si proseguisse verrà disconnessa la sessione precedente."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("ATTENZIONE", "Postazione già connessa. Se si proseguisse verrà disconnessa la sessione precedente.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    myObject = True
                    composeChiave = String.Format("{0}_{1}",
                    "SWGetUltSess", Mid(Session.SessionID.Trim, 1, 50)) 'giu070324 non piu l'ip ma la sessione
                    App.SetObjectToCache(composeChiave, myObject)
                    Exit Sub
                Else 'giu120124 NON PERMETTO L'ACCESSO UNA SECONDA VOLTA E CANCELLO LA CONNESSIONE ?
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("ATTENZIONE", "Postazione già connessa. Impossibile eseguire l'accesso.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
            End If
        Else
            'giu120124
            If Session("AccessoLogin") = "1" And MessageLabel.Text = "" Then
                'giu120124 NON PERMETTO L'ACCESSO UNA SECONDA VOLTA E CANCELLO LA CONNESSIONE ?
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("ATTENZIONE", "Postazione già connessa. Impossibile eseguire l'accesso.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'GIU230622
            'GIU051219
            Dim myCodDitta As String = ""
            If IsNothing(Session(CSTCODDITTA)) Then
                Session(CSTCODDITTA) = ""
            End If
            If String.IsNullOrEmpty(Session(CSTCODDITTA)) Then
                Session(CSTCODDITTA) = ""
            End If
            If myCodDitta.Trim = "" Then 'NON FUNZ LA PRIMA VOLTA
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", "Nessuna Società definita.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                ' ''Exit Sub
            ElseIf myCodDitta.Trim = "00" Then
                MessaggioInfo1.Visible = True
                MessaggioInfo2.Visible = True
                MessaggioInfo3.Visible = True
                MessaggioInfo4.Visible = True
                MessaggioInfo5.Visible = True
            End If
            Dim SWDebug As String = ConfigurationManager.AppSettings("debug")
            If Not String.IsNullOrEmpty(SWDebug) Then
                If SWDebug.Trim.ToUpper = "TRUE" Then
                    MessaggioInfo1.Visible = True
                    MessaggioInfo2.Visible = True
                    MessaggioInfo3.Visible = True
                    MessaggioInfo4.Visible = True
                    MessaggioInfo5.Visible = True
                End If
            End If
        End If

        Login1.Focus()

    End Sub

    Public Sub Login1_Authenticate(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.AuthenticateEventArgs) Handles Login1.Authenticate

        'giu311011 Gestione ACCESSO e controllo se utente gia' connesso
        'giu240212 Per testare la PwdSblocca al secondo accesso
        Dim AccessoLogin = Session("AccessoLogin")
        If IsNothing(AccessoLogin) Then
            Session("AccessoLogin") = "0"
        End If
        If String.IsNullOrEmpty(AccessoLogin) Then
            Session("AccessoLogin") = "0"
        End If
        If AccessoLogin = "" Then
            Session("AccessoLogin") = "0"
        End If
        AccessoLogin = Session("AccessoLogin")

        Dim strErrore As String = ""
        Dim OpSys As New Operatori : Dim DittaSys As New Ditte
        Dim myOp As OperatoriEntity : Dim myDitta As DitteEntity
        Dim arrOperatori As ArrayList : Dim arrDitte As ArrayList
        Dim setMyNNAAAA As New It.SoftAzi.SystemFramework.ApplicationConfiguration
        Try
            arrOperatori = OpSys.getOperatoriByName(Login1.UserName)
        Catch ex As Exception
            'MessageLabel.Text = "Errore accesso alla tabella utente: " & ex.Message
            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Errore accesso alla tabella utente: " & ex.Message, WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End Try

        If Not IsNothing(arrOperatori) Then
            If arrOperatori.Count = 0 Then
                'MessageLabel.Text = "Utente inesistente"
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Utente inesistente", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            myOp = CType(arrOperatori(0), OperatoriEntity)

            If Not myOp.NessunaScadenza Then
                Dim dDataValidita As Date = myOp.DataScadenza
                If (Date.Compare(dDataValidita, Date.Now) < 0) Then
                    Session("AccessoLogin") = "4"
                    AccessoLogin = "4"
                End If
            End If

            Dim UtenteConnesso As New OperatoreConnessoEntity
            Try
                UtenteConnesso.NomeOperatore = myOp.Nome
                UtenteConnesso.Codice = myOp.Codice
                UtenteConnesso.ID = myOp.Contatore
                'giu240212 sblocco Utente 
                If AccessoLogin <> "4" Then
                    If myOp.Livello = "T" Then
                        If (Now.Date.Day + Now.Date.Month + Now.Date.Year).ToString.Trim <> Login1.Password Then
                            If AccessoLogin = "0" Then Session("AccessoLogin") = "1" : MessageLabel.Text = "Password utente errata - Tentativi rimanenti 3"
                            If AccessoLogin = "1" Then Session("AccessoLogin") = "2" : MessageLabel.Text = "Password utente errata - Tentativi rimanenti 2"
                            If AccessoLogin = "2" Then Session("AccessoLogin") = "3" : MessageLabel.Text = "Password utente errata - Tentativi rimanenti 1"
                            If AccessoLogin = "3" Then Session("AccessoLogin") = "4" : MessageLabel.Text = "Password utente errata - UTENTE BLOCCATO - digitare la 2° Password per sbloccare l'utenza"
                            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", MessageLabel.Text, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            MessageLabel.Text = ""
                            Exit Sub
                        End If
                    Else
                        If myOp.Password <> Login1.Password Then
                            If AccessoLogin = "0" Then Session("AccessoLogin") = "1" : MessageLabel.Text = "Password utente errata - Tentativi rimanenti 3"
                            If AccessoLogin = "1" Then Session("AccessoLogin") = "2" : MessageLabel.Text = "Password utente errata - Tentativi rimanenti 2"
                            If AccessoLogin = "2" Then Session("AccessoLogin") = "3" : MessageLabel.Text = "Password utente errata - Tentativi rimanenti 1"
                            If AccessoLogin = "3" Then Session("AccessoLogin") = "4" : MessageLabel.Text = "Password utente errata - UTENTE BLOCCATO - digitare la 2° Password per sbloccare l'utenza"
                            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", MessageLabel.Text, WUC_ModalPopup.TYPE_CONFIRM_Y)
                            MessageLabel.Text = ""
                            Exit Sub
                        End If
                    End If
                Else
                    If myOp.Livello = "T" Then
                        If (Now.Date.Day + Now.Date.Month + Now.Date.Year).ToString.Trim <> Login1.Password Then
                            If SessionUtility.BloccoOperatore(UtenteConnesso.NomeOperatore, "") = True Then
                                'OK BLOCCATO
                            End If
                            'MessageLabel.Text = "Errore(UTENTE BLOCCATO): Sblocco Operatore fallito; chiudere tutte le finetre attive e riprovare."
                            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", "(UTENTE BLOCCATO): Sblocco Operatore fallito; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        Else
                            If SessionUtility.SBloccoOperatore(UtenteConnesso.NomeOperatore, "") = True Then
                                'OK SBLOCCATO
                                Session("AccessoLogin") = "0"
                                'MessageLabel.Text = "Sblocco Operatore riuscito; riprovare ad accedere alla procedura."
                                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Sblocco", "Sblocco Operatore riuscito; riprovare ad accedere alla procedura.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                                Exit Sub
                            Else
                                'MessageLabel.Text = "Errore(UTENTE BLOCCATO): Sblocco Operatore fallito; chiudere tutte le finetre attive e riprovare."
                                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Errore", "(UTENTE BLOCCATO): Sblocco Operatore fallito; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                                Exit Sub
                            End If
                        End If
                    Else
                        If myOp.PwdSblocca <> Login1.Password Then
                            If SessionUtility.BloccoOperatore(UtenteConnesso.NomeOperatore, "") = True Then
                                'OK BLOCCATO
                            End If
                            'MessageLabel.Text = "Errore(UTENTE BLOCCATO): Sblocco Operatore fallito; chiudere tutte le finetre attive e riprovare."
                            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", "(UTENTE BLOCCATO): Sblocco Operatore fallito; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        Else
                            If SessionUtility.SBloccoOperatore(UtenteConnesso.NomeOperatore, "") = True Then
                                'OK SBLOCCATO
                                Session("AccessoLogin") = "0"
                                'MessageLabel.Text = "Sblocco Operatore riuscito; riprovare ad accedere alla procedura."
                                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Sblocco", "Sblocco Operatore riuscito; riprovare ad accedere alla procedura.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                                Exit Sub
                            Else
                                'MessageLabel.Text = "Errore(UTENTE BLOCCATO): Sblocco Operatore fallito; chiudere tutte le finetre attive e riprovare."
                                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                                ModalPopup.Show("Errore", "(UTENTE BLOCCATO): Sblocco Operatore fallito; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                                Exit Sub
                            End If
                        End If
                    End If
                End If
                'giu120124
                If Session("AccessoLogin") = "1" And MessageLabel.Text = "" Then
                    'giu090324 controllo se esiste la stessa sessione di adesso
                    If SessionUtility.CTROpBySessionePostazione(Session.SessionID, Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo) = True Then
                        If myOp.Livello = "V" Then 'GIU06032024
                            'OK PROSEGUO PER IL SW ESTERNO SU TABLET
                        Else
                            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("ATTENZIONE", "Postazione già connessa. Impossibile eseguire l'accesso.<br><b>CHIUDERE IL BROWSER E RIAPRIRLO PER POTER ACCEDERE</b>", WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                    End If
                    '---------
                    'giu120124 NON PERMETTO L'ACCESSO UNA SECONDA VOLTA E CANCELLO LA CONNESSIONE ?
                    SessionUtility.DelOpBySessionePostazione(Session.SessionID, Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo)
                    '---------
                    If OpSys.DelOperatoreConnesso(UtenteConnesso.NomeOperatore, Session(CSTCODDITTA), NomeModulo) = True Then
                        'OK cancellato cosi le altre sessioni saranno sconnesse alla prima operazione lato server
                    Else
                        'MessageLabel.Text = "Errore: cancella operatore connesso; chiudere tutte le finetre attive e riprovare."
                        Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "Cancella operatore connesso; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                    '''If myOp.Livello = "V" Then 'GIU06032024
                    '''    'OK PROSEGUO PER IL SW ESTERNO SU TABLET
                    '''Else
                    '''    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    '''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    '''    ModalPopup.Show("ATTENZIONE", "Postazione già connessa. Impossibile eseguire l'accesso.<br><b>CHIUDERE IL BROWSER E RIAPRIRLO PER POTER ACCEDERE</b>", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    '''    Exit Sub
                    '''End If
                End If
                '--------------------------------------------------------------
                '-
                If myOp.Livello = "U" Then
                    UtenteConnesso.Tipo = CSTUFFICIO_AMMINISTRATIVO
                ElseIf myOp.Livello = "A" Then
                    UtenteConnesso.Tipo = CSTAMMINISTRATORE
                ElseIf myOp.Livello = "T" Then
                    UtenteConnesso.Tipo = CSTTECNICO
                ElseIf myOp.Livello = "M" Then
                    UtenteConnesso.Tipo = CSTMAGAZZINO
                ElseIf myOp.Livello = "Q" Then
                    UtenteConnesso.Tipo = CSTACQUISTI
                ElseIf myOp.Livello = "V" Then
                    UtenteConnesso.Tipo = CSTVENDITE
                Else
                    'MessageLabel.Text = "Tipo utente sconosciuto, contattare l'amministratore di sistema"
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Tipo utente sconosciuto, contattare l'amministratore di sistema", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If

                If Not myOp.NessunaScadenza Then
                    Dim dDataValidita As Date = myOp.DataScadenza
                    If (Date.Compare(dDataValidita, Date.Now) < 0) Then
                        'MessageLabel.Text = "L'utenza non è più valida, contattare l'amministratore di sistema."
                        Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "L'utenza non è più valida, contattare l'amministratore di sistema.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                End If
                'Ditta ed Esercizio
                If ddL_Ditte.SelectedValue = "" Then
                    'MessageLabel.Text = "Selezionare la società"
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Selezionare la società", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
                If ddL_Esercizi.SelectedValue = "" Then
                    'MessageLabel.Text = "Selezionare l'esercizio"
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Selezionare l'esercizio", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If

                Try
                    arrDitte = DittaSys.getDitteByCodice(ddL_Ditte.SelectedValue)
                Catch Ex As Exception
                    'MessageLabel.Text = "Errore accesso alla tabella società: " & Ex.Message
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore accesso alla tabella società: " & Ex.Message, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End Try
                If Not IsNothing(arrDitte) Then
                    If arrDitte.Count = 0 Then
                        'MessageLabel.Text = "Società inesistente"
                        Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "Società inesistente", WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End If
                    myDitta = CType(arrDitte(0), DitteEntity)
                Else
                    'MessageLabel.Text = "Società inesistente"
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Società inesistente", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If

                UtenteConnesso.Azienda = myDitta.Descrizione
                UtenteConnesso.CodiceDitta = myDitta.Codice.Trim
                Session(CSTCODDITTA) = myDitta.Codice.Trim

                strErrore = ""
                Session(CSTMAXLEVEL) = App.GetDatiDitta(myDitta.Codice.Trim, strErrore).MaxLevel.Trim
                If strErrore.Trim <> "" Then
                    'MessageLabel.Text = "Errore: Caricamento dati Società - MaxLevel non definito"
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Caricamento dati Società - MaxLevel non definito", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
                Dim strObbligatorio As String = Session(CSTMAXLEVEL)
                If IsNothing(strObbligatorio) Then
                    strObbligatorio = ""
                End If
                If String.IsNullOrEmpty(strObbligatorio) Then
                    strObbligatorio = ""
                End If
                If strObbligatorio = "" Or Not IsNumeric(strObbligatorio) Then
                    'MessageLabel.Text = "Errore: Caricamento dati Società - MaxLevel non definito"
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Caricamento dati Società - MaxLevel non definito", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
                '--
                UtenteConnesso.Esercizio = ddL_Esercizi.SelectedValue
            Catch ex As Exception
                'MessageLabel.Text = "Errore: Caricamento dati Società: " & strErrore
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Caricamento dati Società: " & strErrore, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End Try
            '--------------------------------------------------------------
            'giu050312 GIU060312
            Dim ArrLogOnUtente As ArrayList = OpSys.OperatoreConnesso(UtenteConnesso.NomeOperatore, UtenteConnesso.CodiceDitta, Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo, Session.SessionID, UtenteConnesso.ID, UtenteConnesso.Codice, UtenteConnesso.Azienda, UtenteConnesso.Tipo, UtenteConnesso.Esercizio)
            If ArrLogOnUtente.Count > 0 Then
                ''giu280212---------------------------------------------
                'giu240212 sblocco Utente 
                If AccessoLogin = "0" Then
                    Session("AccessoLogin") = "1"
                    'MessageLabel.Text = "ATTENZIONE, postazione già connessa. Se si proseguisse verrà disconnessa la sessione precedente."
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("ATTENZIONE", "Postazione già connessa. Se si proseguisse verrà disconnessa la sessione precedente.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                    myObject = True
                    composeChiave = String.Format("{0}_{1}",
                    "SWGetUltSess", Mid(Session.SessionID.Trim, 1, 50)) 'giu070324 no ip ok sessione
                    App.SetObjectToCache(composeChiave, myObject)
                    Exit Sub
                ElseIf AccessoLogin = "1" Then
                    Try
                        'GIU010312
                        SessionUtility.DelOpBySessionePostazione(Session.SessionID, Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo)
                        '---------
                        If OpSys.DelOperatoreConnesso(UtenteConnesso.NomeOperatore, Session(CSTCODDITTA), NomeModulo) = True Then
                            'OK cancellato cosi le altre sessioni saranno sconnesse alla prima operazione lato server
                        Else
                            'MessageLabel.Text = "Errore: cancella operatore connesso; chiudere tutte le finetre attive e riprovare."
                            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", "Cancella operatore connesso; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                        'GIU060312
                        OpSys.OperatoreConnesso(UtenteConnesso.NomeOperatore, UtenteConnesso.CodiceDitta, Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo, Session.SessionID, UtenteConnesso.ID, UtenteConnesso.Codice, UtenteConnesso.Azienda, UtenteConnesso.Tipo, UtenteConnesso.Esercizio)
                        If SessionUtility.CTROpBySessionePostazione(Session.SessionID, Mid(Request.UserHostAddress.Trim, 1, 50), NomeModulo) = False Then
                            'MessageLabel.Text = "Errore: Inserimento operatore connesso; chiudere tutte le finetre attive e riprovare."
                            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Errore", "Inserimento operatore connesso; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                            Exit Sub
                        End If
                    Catch ex As Exception
                        'MessageLabel.Text = "Errore: cancella operatore connesso; chiudere tutte le finetre attive e riprovare.: " & ex.Message.Trim
                        Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Errore", "cancella operatore connesso; chiudere tutte le finetre attive e riprovare.: " & ex.Message.Trim, WUC_ModalPopup.TYPE_CONFIRM_Y)
                        Exit Sub
                    End Try
                End If
            End If
            If OpSys.UpdOperatoriDataOraUltAccesso(UtenteConnesso.NomeOperatore) = False Then
                'MessageLabel.Text = "Errore aggiornamento data accesso operatore; chiudere tutte le finetre attive e riprovare."
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "aggiornamento data accesso operatore; chiudere tutte le finetre attive e riprovare.", WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            '-
            UtenteConnesso.SessionID = Session.SessionID
            setMyNNAAAA.setNNAAAA = UtenteConnesso.CodiceDitta & UtenteConnesso.Esercizio
            '--- inizializzo le variabili di sessione ---
            SessionUtility.Init(Session)
            Session(ESERCIZIO) = UtenteConnesso.Esercizio
            Session(CSTUTENTE) = UtenteConnesso.NomeOperatore
            Session(CSTCODDITTA) = myDitta.Codice
            'giu130312
            Session(CSTTIPOUTENTE) = UtenteConnesso.Tipo
            ' '' '' ''--- la funzionalità  di reload: può essere utilizzata in debug per forzare il caricamento delle liste
            Dim Reload As Boolean = True
            If (Not Request.Params("reload") Is Nothing) Then
                Reload = True
            End If
            '-
            'giu130212 non eseguo il ricalcolo se sono in Magazzino allestimento
            If (UtenteConnesso.Tipo.Equals(CSTMAGAZZINO)) Or (UtenteConnesso.Tipo.Equals(CSTVENDITE)) Then
                'NON ESEGUO IL RICALCOLO
                Reload = False
            End If
            '--- caricamento delle liste contenenti da db modifica giuseppe
            'Ricarico sempre i ParametriGeneraliAzi
            strErrore = ""
            If App.CaricaParametri(UtenteConnesso.Esercizio, strErrore) = False Then
                'MessageLabel.Text = "Errore nel caricamento Parametri generali azienda, contattare l'amministratore di sistema. Errore: " & strErrore
                Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "caricamento Parametri generali azienda, contattare l'amministratore di sistema. Errore: " & strErrore, WUC_ModalPopup.TYPE_CONFIRM_Y)
                Exit Sub
            End If
            'giu080620 per velocizzare gli accessi ai RESPONSABILI VISITA/AREA PER I CONTRATTI
            If (UtenteConnesso.Tipo.Equals(CSTVENDITE)) Then
            Else
                strErrore = ""
                If App.CaricaDatiApplicazione(UtenteConnesso.Esercizio, Reload, strErrore) = False Then
                    'MessageLabel.Text = "Errore nel caricamento Tabelle azienda, contattare l'amministratore di sistema. Errore: " & strErrore
                    Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "caricamento Tabelle azienda, contattare l'amministratore di sistema. Errore: " & strErrore, WUC_ModalPopup.TYPE_CONFIRM_Y)
                    Exit Sub
                End If
            End If

            'giu251111
            Session(CSTABILMSG) = SWNO
            strErrore = "" : Dim strValore As String = ""
            If App.GetDatiAbilitazioni(CSTABILAZI, CSTABILMSG, strValore, strErrore) = True Then
                Session(CSTABILMSG) = SWSI
            End If
            '---------
            Session(SWOP) = SWOPNESSUNA
            If (UtenteConnesso.Tipo.Equals(CSTVENDITE)) Then
                'giu150223 ok anche per il REALE
                Response.Redirect("WebFormTables\WF_MenuCA.aspx?labelForm=Menu Gestione CONTRATTI")
            Else
                Response.Redirect("WebFormTables\WF_Menu.aspx?labelForm=Menu principale")
            End If
        Else
            'MessageLabel.Text = "Utente inesistente"
            'Messaggio
            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Utente inesistente", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
            '-------------------------------------------
            'Exit Sub
        End If
    End Sub

    Public Sub ddL_Ditte_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Session(CSTCODDITTA) = ddL_Ditte.SelectedValue
        SetImgAzienda()
    End Sub
    Public Sub SetImgAzienda()
        If NDitte > 1 Then
            ImgAzienza.Src = ""
            ImgAzienza.Visible = False
            Exit Sub
        End If
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Then
            ImgAzienza.Src = "Immagini/soft_azienda.png"
        ElseIf Session(CSTCODDITTA) = "01" Then
            ImgAzienza.Src = "Immagini/Sfera_Sitopiccolo.jpg"
        ElseIf Session(CSTCODDITTA) = "05" Then
            ImgAzienza.Src = "Immagini/Iredeem_Ridotta.jpg"
        Else
            ImgAzienza.Src = "Immagini/soft_azienda.png"
        End If
    End Sub

    Private Sub LoginOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LoginOK.Click

        If Login1.UserName.Trim = "" Or Login1.Password.Trim = "" Then
            Exit Sub
        End If
        'Ditta ed Esercizio
        If ddL_Ditte.SelectedValue = "" Then
            'MessageLabel.Text = "Selezionare la società"
            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Selezionare la società", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        If ddL_Esercizi.SelectedValue = "" Then
            'MessageLabel.Text = "Selezionare l'esercizio"
            Session(MODALPOPUP_CALLBACK_METHOD) = "SetImgAzienda"
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "Selezionare l'esercizio", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If

        'MessageLabel.Text = "Caricamento dati in corso .... Attendere"
        ' ''giu040220 ImgAzienza.Src = "Immagini/ajax-loader-large.gif"
        Login1_Authenticate(Login1, Nothing)
    End Sub


    Public Sub Login1_LoginError(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login1.LoginError
        'obbligatorio
    End Sub


End Class