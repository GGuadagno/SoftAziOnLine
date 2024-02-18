Imports SoftAziOnLine.Def
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.Utility

Public Class Magazzino
    'GIU270820 MODIFICHE PER LA GESTIONE MAGAZZINI 0,1,2,3......)
    'giu251120 IMPEGNO GIACENZA TENER CONTO DEL MAGAZZINO PER LA GIACENZA DA IMPEGNARE
#Region "RICALCOLO GIACENZE DI MAGAZZINO"
    Public Shared Function Ricalcola_Giacenze(ByVal CodiceArt As String, ByRef StrErrore As String, ByRef SWNegativi As Boolean, ByVal SWSi As Boolean)
        SWNegativi = False 'giu190613
        'giu070124 SWSi NON SERVE A NULLA XKE ESEGUO SEMPRE LA FUNZIONE E CORRETTO IL RETURN FALSE NEL CONTROLLO CONCORRENZA
        'GIU200112 FORZA L'AGGIORNAMENTO SOLO DA RICALCOLO GIACENZE
        Dim strValore As String = ""
        Try
            If App.GetDatiAbilitazioni(CSTABILAZI, "SWRicGiac", strValore, StrErrore) = False Then
                'PROSEGUO COMUNQUE
            End If
        Catch ex As Exception
            'PROSEGUO COMUNQUE
        End Try
        'giu250423 per evitare l'agg.in concorrenza
        If IsDate(strValore.Trim) Then
            If DateDiff(DateInterval.Minute, CDate(strValore.Trim), CDate(Now)) < 5 Then
                StrErrore = "Attenzione, ricalcolo Giacenze in corso, si prega di attendere"
                Return True
            Else 'ok procedo
            End If
        Else 'ok procedo memorizzando l'inizio e per i prossimi 5minuti aspetto
            If App.AggiornaAbilitazione(CSTABILAZI, "SWRicGiac", Now.ToString(), StrErrore) = False Then
                'PROSEGUO COMUNQUE
            End If
        End If
        '-------------------------------------------------------------------------------------------------------------------
        Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConnMagaz1 = New System.Data.SqlClient.SqlConnection

        Dim DA_ArtDiMag = New System.Data.SqlClient.SqlDataAdapter
        Dim DA_Lotti = New System.Data.SqlClient.SqlDataAdapter
        Dim DA_MovMagaz = New System.Data.SqlClient.SqlDataAdapter
        Dim DA_MovMagazLotti = New System.Data.SqlClient.SqlDataAdapter
        Dim DA_MovMagazOrdini = New System.Data.SqlClient.SqlDataAdapter
        Dim DA_MovMagazOrdiniFor = New System.Data.SqlClient.SqlDataAdapter
        Dim Da_AnaMag = New System.Data.SqlClient.SqlDataAdapter

        Dim SqlSel_ArtDiMag = New System.Data.SqlClient.SqlCommand
        Dim SqlIns_ArtDiMag = New System.Data.SqlClient.SqlCommand
        Dim SqlUpd_ArtDiMag = New System.Data.SqlClient.SqlCommand
        Dim SqlDel_ArtDiMag = New System.Data.SqlClient.SqlCommand

        Dim SqlSel_MovMagaz = New System.Data.SqlClient.SqlCommand
        Dim SqlSel_MovMagazLotti = New System.Data.SqlClient.SqlCommand
        Dim SqlSel_MovMagazOrdini = New System.Data.SqlClient.SqlCommand
        Dim SqlSel_MovMagazOrdiniFor = New System.Data.SqlClient.SqlCommand

        Dim SqlGet_Lotti = New System.Data.SqlClient.SqlCommand
        Dim SqlIns_Lotti = New System.Data.SqlClient.SqlCommand
        Dim SqlUpd_Lotti = New System.Data.SqlClient.SqlCommand
        Dim SqlDel_Lotti = New System.Data.SqlClient.SqlCommand

        Dim SqlGet_AnaMag = New System.Data.SqlClient.SqlCommand
        Dim SqlIns_AnaMag = New System.Data.SqlClient.SqlCommand
        Dim SqlUpd_AnaMag = New System.Data.SqlClient.SqlCommand

        Dim SqlUpd_ImpArticolo As New System.Data.SqlClient.SqlCommand

        Dim DsMagazzino1 As New DsMagazzino

        Dim SqlUpd_AzzeraMagazzino = New System.Data.SqlClient.SqlCommand

        'GIU111120 Dim TransTmp As SqlClient.SqlTransaction = Nothing

        Dim StrErrImp As String = ""
        Dim ObjUt As New Utility
        If ObjUt.Impegna_Giacenze(CodiceArt, StrErrImp) = False Then 'giu270814
            StrErrore = StrErrImp
            Return False
        End If
        '
        'DA_ArtDiMag
        '
        DA_ArtDiMag.DeleteCommand = SqlDel_ArtDiMag
        DA_ArtDiMag.InsertCommand = SqlIns_ArtDiMag
        DA_ArtDiMag.SelectCommand = SqlSel_ArtDiMag
        DA_ArtDiMag.UpdateCommand = SqlUpd_ArtDiMag
        '
        'SqlConnection1
        '
        SqlConnMagaz1.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        Dim strFase As String = "" 'giu040416 per sapere dove va in errore quando va nel Catch
        Try
            strFase = "1. Apertura connesione dbSoftAzi"
            If SqlConnMagaz1.State <> ConnectionState.Open Then
                SqlConnMagaz1.Open()
            End If

            'GIU111120 TransTmp = SqlConnMagaz1.BeginTransaction(IsolationLevel.ReadCommitted)
            '
            'SqlSel_ArtDiMag
            '
            SqlSel_ArtDiMag.CommandText = "[get_ArtDiMag]"
            SqlSel_ArtDiMag.CommandType = System.Data.CommandType.StoredProcedure
            SqlSel_ArtDiMag.Connection = SqlConnMagaz1
            'GIU111120 SqlSel_ArtDiMag.Transaction = TransTmp
            SqlSel_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '
            'SqlIns_ArtDiMag
            '
            SqlIns_ArtDiMag.CommandText = "[insert_ArtDiMag]"
            SqlIns_ArtDiMag.CommandType = System.Data.CommandType.StoredProcedure
            SqlIns_ArtDiMag.Connection = SqlConnMagaz1
            'GIU111120 SqlIns_ArtDiMag.Transaction = TransTmp
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Codice_Magazzino", System.Data.SqlDbType.Int, 4, "Codice_Magazzino"))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giacenza", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giacenza", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ord_Clienti", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Clienti", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Reparto", System.Data.SqlDbType.Int, 4, "Reparto"))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Scaffale", System.Data.SqlDbType.Int, 4, "Scaffale"))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Piano", System.Data.SqlDbType.Int, 4, "Piano"))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinati", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ordinati", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Arrivo", System.Data.SqlDbType.DateTime, 8, "Data_Arrivo"))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Prodotto_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prodotto_P", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Confezionato_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Confezionato_P", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinato_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(0, Byte), "Ordinato_P", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Venduto_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Venduto_P", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giac_Impegnata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Impegnata", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giac_Prenotata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Prenotata", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ord_Clienti_Evasi", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Clienti_Evasi", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaArrivoFornit", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "QtaArrivoFornit", System.Data.DataRowVersion.Current, Nothing))
            '
            'SqlUpd_ArtDiMag
            '
            SqlUpd_ArtDiMag.CommandText = "[update_ArtDiMag]"
            SqlUpd_ArtDiMag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_ArtDiMag.Connection = SqlConnMagaz1
            'GIU111120 SqlUpd_ArtDiMag.Transaction = TransTmp
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Codice_Magazzino", System.Data.SqlDbType.Int, 4, "Codice_Magazzino"))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giacenza", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giacenza", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ord_Clienti", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Clienti", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Reparto", System.Data.SqlDbType.Int, 4, "Reparto"))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Scaffale", System.Data.SqlDbType.Int, 4, "Scaffale"))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Piano", System.Data.SqlDbType.Int, 4, "Piano"))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinati", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ordinati", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Arrivo", System.Data.SqlDbType.DateTime, 8, "Data_Arrivo"))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Prodotto_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prodotto_P", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Confezionato_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Confezionato_P", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinato_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ordinato_P", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Venduto_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Venduto_P", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giac_Impegnata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Impegnata", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giac_Prenotata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Prenotata", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ord_Clienti_Evasi", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Clienti_Evasi", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaArrivoFornit", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(0, Byte), "QtaArrivoFornit", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Articolo", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Codice_Magazzino", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Codice_Magazzino", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Confezionato_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Confezionato_P", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Data_Arrivo", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Data_Arrivo", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Giac_Impegnata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Impegnata", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Giac_Prenotata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Prenotata", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Giacenza", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giacenza", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Ord_Clienti", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Clienti", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Ordinati", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ordinati", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Ordinato_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ordinato_P", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Piano", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Piano", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Prodotto_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prodotto_P", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Reparto", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Reparto", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Scaffale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Scaffale", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Venduto_P", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Venduto_P", System.Data.DataRowVersion.Original, Nothing))
            '
            'SqlDel_ArtDiMag
            '
            SqlDel_ArtDiMag.CommandText = "[delete_ArtDiMag]"
            SqlDel_ArtDiMag.CommandType = System.Data.CommandType.StoredProcedure
            SqlDel_ArtDiMag.Connection = SqlConnMagaz1
            'GIU111120 SqlDel_ArtDiMag.Transaction = TransTmp
            SqlDel_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDel_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Articolo", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_ArtDiMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Codice_Magazzino", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Codice_Magazzino", System.Data.DataRowVersion.Original, Nothing))
            '
            'SqlGet_Lotti
            '
            SqlGet_Lotti.CommandText = "[get_Lotti]"
            SqlGet_Lotti.CommandType = System.Data.CommandType.StoredProcedure
            SqlGet_Lotti.Connection = SqlConnMagaz1
            'GIU111120 SqlGet_Lotti.Transaction = TransTmp
            SqlGet_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '
            'SqlIns_Lotti
            '
            SqlIns_Lotti.CommandText = "[insert_Lotti]"
            SqlIns_Lotti.CommandType = System.Data.CommandType.StoredProcedure
            SqlIns_Lotti.Connection = SqlConnMagaz1
            'GIU111120 SqlIns_Lotti.Transaction = TransTmp
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Lotto", System.Data.SqlDbType.NVarChar, 30, "Cod_Lotto"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Magaz", System.Data.SqlDbType.Int, 4, "Cod_Magaz"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 30, "NSerie"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Produzione", System.Data.SqlDbType.DateTime, 4, "Data_Produzione"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza", System.Data.SqlDbType.DateTime, 4, "Data_Scadenza"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Responsabile", System.Data.SqlDbType.NVarChar, 3, "Cod_Responsabile"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Selezionato", System.Data.SqlDbType.Bit, 1, "Selezionato"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AggancioScarto", System.Data.SqlDbType.NVarChar, 20, "AggancioScarto"))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Iniziale", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Iniziale", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giacenza", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Giacenza", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Caricata", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Caricata", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Scarto", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Scarto", System.Data.DataRowVersion.Current, Nothing))
            '
            'SqlUpd_Lotti
            '
            SqlUpd_Lotti.CommandText = "[update_Lotti]"
            SqlUpd_Lotti.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_Lotti.Connection = SqlConnMagaz1
            'GIU111120 SqlUpd_Lotti.Transaction = TransTmp
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Lotto", System.Data.SqlDbType.NVarChar, 30, "Cod_Lotto"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Magaz", System.Data.SqlDbType.Int, 4, "Cod_Magaz"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NSerie", System.Data.SqlDbType.NVarChar, 30, "NSerie"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Produzione", System.Data.SqlDbType.DateTime, 4, "Data_Produzione"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Scadenza", System.Data.SqlDbType.DateTime, 4, "Data_Scadenza"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Responsabile", System.Data.SqlDbType.NVarChar, 3, "Cod_Responsabile"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Selezionato", System.Data.SqlDbType.Bit, 1, "Selezionato"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AggancioScarto", System.Data.SqlDbType.NVarChar, 20, "AggancioScarto"))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Iniziale", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Iniziale", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giacenza", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Giacenza", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Caricata", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Caricata", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Qta_Scarto", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Scarto", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Articolo", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Lotto", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Lotto", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Magaz", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Magaz", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NSerie", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "NSerie", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_AggancioScarto", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "AggancioScarto", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Responsabile", System.Data.SqlDbType.NVarChar, 3, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Responsabile", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Data_Produzione", System.Data.SqlDbType.DateTime, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Data_Produzione", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Data_Scadenza", System.Data.SqlDbType.DateTime, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Data_Scadenza", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Giacenza", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Giacenza", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Qta_Caricata", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Caricata", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Qta_Iniziale", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Iniziale", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Qta_Scarto", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Scarto", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Selezionato", System.Data.SqlDbType.Bit, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Selezionato", System.Data.DataRowVersion.Original, Nothing))
            '
            'SqlDel_Lotti
            '
            SqlDel_Lotti.CommandText = "[delete_Lotti]"
            SqlDel_Lotti.CommandType = System.Data.CommandType.StoredProcedure
            SqlDel_Lotti.Connection = SqlConnMagaz1
            'GIU111120 SqlDel_Lotti.Transaction = TransTmp
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Articolo", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Lotto", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Lotto", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Magaz", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Magaz", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NSerie", System.Data.SqlDbType.NVarChar, 16, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "NSerie", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_AggancioScarto", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "AggancioScarto", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Responsabile", System.Data.SqlDbType.NVarChar, 3, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Responsabile", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Data_Produzione", System.Data.SqlDbType.DateTime, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Data_Produzione", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Data_Scadenza", System.Data.SqlDbType.DateTime, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Data_Scadenza", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Giacenza", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Giacenza", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Qta_Caricata", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Caricata", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Qta_Iniziale", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Iniziale", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Qta_Scarto", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Scarto", System.Data.DataRowVersion.Original, Nothing))
            SqlDel_Lotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Selezionato", System.Data.SqlDbType.Bit, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Selezionato", System.Data.DataRowVersion.Original, Nothing))
            '
            'Sqlupdate_ImpegnaOrdineArticolo
            '
            SqlUpd_ImpArticolo.CommandText = "[update_ImpegnaOrdineArticolo]"
            SqlUpd_ImpArticolo.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_ImpArticolo.Connection = SqlConnMagaz1
            'GIU111120 SqlUpd_ImpArticolo.Transaction = TransTmp
            SqlUpd_ImpArticolo.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_ImpArticolo.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, "IDDocumenti"))
            SqlUpd_ImpArticolo.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 4, "Riga"))
            SqlUpd_ImpArticolo.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaImpegnata", System.Data.SqlDbType.Decimal, 5, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "QtaImpegnata", System.Data.DataRowVersion.Current, Nothing))


            DA_Lotti.DeleteCommand = SqlDel_Lotti
            DA_Lotti.InsertCommand = SqlIns_Lotti
            DA_Lotti.SelectCommand = SqlGet_Lotti
            DA_Lotti.UpdateCommand = SqlUpd_Lotti


            'FABIO 20/12/2011

            Da_AnaMag.InsertCommand = SqlIns_AnaMag
            Da_AnaMag.SelectCommand = SqlGet_AnaMag
            Da_AnaMag.UpdateCommand = SqlUpd_AnaMag
            '
            'SqlGet_AnaMag
            '
            SqlGet_AnaMag.CommandText = "[get_AnaMagRicalcoloGiac]"
            SqlGet_AnaMag.CommandType = System.Data.CommandType.StoredProcedure
            SqlGet_AnaMag.Connection = SqlConnMagaz1
            'GIU111120 SqlGet_AnaMag.Transaction = TransTmp
            SqlGet_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            '
            'SqlIns_AnaMag
            '
            SqlIns_AnaMag.CommandText = "[Insert_AnaMagRicalcoloGiac]"
            SqlIns_AnaMag.CommandType = System.Data.CommandType.StoredProcedure
            SqlIns_AnaMag.Connection = SqlConnMagaz1
            'GIU111120 SqlIns_AnaMag.Transaction = TransTmp
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giacenza", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giacenza", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ord_Clienti", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Clienti", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ord_Fornit", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Fornit", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Prodotto", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prodotto", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Confezionato", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Confezionato", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinato", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ordinato", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Venduto", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Venduto", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giac_Impegnata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Impegnata", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giac_Prenotata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Prenotata", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaArrivoFornit", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "QtaArrivoFornit", System.Data.DataRowVersion.Current, Nothing))
            SqlIns_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Arrivo", System.Data.SqlDbType.DateTime, 8, "Data_Arrivo"))
            '
            'SqlUpd_AnaMag
            '
            SqlUpd_AnaMag.CommandText = "[update_AnaMagRicalcoloGiac]"
            SqlUpd_AnaMag.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_AnaMag.Connection = SqlConnMagaz1
            'GIU111120 SqlUpd_AnaMag.Transaction = TransTmp
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giacenza", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giacenza", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ord_Clienti", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Clienti", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ord_Fornit", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ord_Fornit", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Prodotto", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Prodotto", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Confezionato", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Confezionato", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Ordinato", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Ordinato", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Venduto", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Venduto", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giac_Impegnata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Impegnata", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Giac_Prenotata", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Giac_Prenotata", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Cod_Articolo", System.Data.DataRowVersion.Original, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@QtaArrivoFornit", System.Data.SqlDbType.Money, 5, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "QtaArrivoFornit", System.Data.DataRowVersion.Current, Nothing))
            SqlUpd_AnaMag.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Data_Arrivo", System.Data.SqlDbType.DateTime, 8, "Data_Arrivo"))

            '

            'FINE FABIO 20/12/2011
            'giu070920 modificata NON INSERISCE GLI ARTICOLI MANCANTI DAL MAGAZZINO ZERO (VERIFICARE IL XKE' LO FACESSI)
            SqlUpd_AzzeraMagazzino.CommandText = "[set_AzzeraDisponibilitaGiacenze]"
            SqlUpd_AzzeraMagazzino.CommandType = System.Data.CommandType.StoredProcedure
            SqlUpd_AzzeraMagazzino.Connection = SqlConnMagaz1
            'GIU111120 SqlUpd_AzzeraMagazzino.Transaction = TransTmp
            SqlUpd_AzzeraMagazzino.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            'SqlUpd_AzzeraMagazzino.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FiltroTipoDoc", System.Data.SqlDbType.NVarChar, 30, ""))
            If CodiceArt = "" Then
                SqlUpd_AzzeraMagazzino.Parameters("@Cod_Articolo").Value = DBNull.Value
            Else
                SqlUpd_AzzeraMagazzino.Parameters("@Cod_Articolo").Value = CodiceArt
            End If
            'SqlUpd_AzzeraMagazzino.Parameters("@FiltroTipoDoc").Value = SWTD(TD.OrdClienti)

            '====MOVIMENTI DI MAGAZZINO ==========================================
            SqlSel_MovMagaz.CommandText = "[get_MovimentiMagazzino]" 'giu111018 ok FC+ScGiacenza
            SqlSel_MovMagaz.CommandType = System.Data.CommandType.StoredProcedure
            SqlSel_MovMagaz.Connection = SqlConnMagaz1
            'GIU111120 SqlSel_MovMagaz.Transaction = TransTmp
            SqlSel_MovMagaz.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            If CodiceArt = "" Then
                SqlSel_MovMagaz.Parameters("@Cod_Articolo").Value = DBNull.Value
            Else
                SqlSel_MovMagaz.Parameters("@Cod_Articolo").Value = CodiceArt
            End If
            DA_MovMagaz.SelectCommand = SqlSel_MovMagaz

            '====LOTTI ==========================================
            SqlSel_MovMagazLotti.CommandText = "[get_MovimentiMagazzinoLotti]" 'giu111018 ok FC+ScGiacenza
            SqlSel_MovMagazLotti.CommandType = System.Data.CommandType.StoredProcedure
            SqlSel_MovMagazLotti.Connection = SqlConnMagaz1
            'GIU111120 SqlSel_MovMagazLotti.Transaction = TransTmp
            SqlSel_MovMagazLotti.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            If CodiceArt = "" Then
                SqlSel_MovMagazLotti.Parameters("@Cod_Articolo").Value = DBNull.Value
            Else
                SqlSel_MovMagazLotti.Parameters("@Cod_Articolo").Value = CodiceArt
            End If
            DA_MovMagazLotti.SelectCommand = SqlSel_MovMagazLotti


            '===ORDINI CLIENTI====================================
            SqlSel_MovMagazOrdini.CommandText = "[get_MovimentiMagazzinoOrdini]"
            SqlSel_MovMagazOrdini.CommandType = System.Data.CommandType.StoredProcedure
            SqlSel_MovMagazOrdini.Connection = SqlConnMagaz1
            'GIU111120 SqlSel_MovMagazOrdini.Transaction = TransTmp
            SqlSel_MovMagazOrdini.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            SqlSel_MovMagazOrdini.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FiltroTipoDoc", System.Data.SqlDbType.NVarChar, 30, ""))
            If CodiceArt = "" Then
                SqlSel_MovMagazOrdini.Parameters("@Cod_Articolo").Value = DBNull.Value
            Else
                SqlSel_MovMagazOrdini.Parameters("@Cod_Articolo").Value = CodiceArt
            End If
            SqlSel_MovMagazOrdini.Parameters("@FiltroTipoDoc").Value = "('" & SWTD(TD.OrdClienti) & "')"
            DA_MovMagazOrdini.SelectCommand = SqlSel_MovMagazOrdini

            '===ORDINI FORNITORI ====================================
            SqlSel_MovMagazOrdiniFor.CommandText = "[get_MovimentiMagazzinoOrdiniFor]"
            SqlSel_MovMagazOrdiniFor.CommandType = System.Data.CommandType.StoredProcedure
            SqlSel_MovMagazOrdiniFor.Connection = SqlConnMagaz1
            'GIU111120 SqlSel_MovMagazOrdiniFor.Transaction = TransTmp
            SqlSel_MovMagazOrdiniFor.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 20, "Cod_Articolo"))
            SqlSel_MovMagazOrdiniFor.Parameters.Add(New System.Data.SqlClient.SqlParameter("@FiltroTipoDoc", System.Data.SqlDbType.NVarChar, 30, ""))
            If CodiceArt = "" Then
                SqlSel_MovMagazOrdiniFor.Parameters("@Cod_Articolo").Value = DBNull.Value
            Else
                SqlSel_MovMagazOrdiniFor.Parameters("@Cod_Articolo").Value = CodiceArt
            End If
            SqlSel_MovMagazOrdiniFor.Parameters("@FiltroTipoDoc").Value = "('" & SWTD(TD.OrdFornitori) & "')"
            DA_MovMagazOrdiniFor.SelectCommand = SqlSel_MovMagazOrdiniFor

            '==CARICO I DATASET
            strFase = "2. Azzeramento quantita' magazzino"
            SqlUpd_AzzeraMagazzino.ExecuteNonQuery()    'Parto con l'azzeramento delle quantità
            strFase = "3. Caricamento di tutte le tabelle di input"
            DA_ArtDiMag.Fill(DsMagazzino1.ArtdiMag)
            Da_AnaMag.Fill(DsMagazzino1.AnaMag) 'FABIO 20/12/2011 PRENDO ARTICOLI DA ANAMAG
            DA_MovMagaz.Fill(DsMagazzino1.MovimentiMagazzino)
            DA_MovMagazLotti.Fill(DsMagazzino1.MovimentiMagazzinoLotti)
            DA_MovMagazOrdini.Fill(DsMagazzino1.MovimentiMagazzinoOrdini) 'Solo StatoDoc = 0 oppure 2
            DA_MovMagazOrdiniFor.Fill(DsMagazzino1.MovimentiMagazzinoOrdiniFor) 'Solo StatoDoc = 0 oppure 2
            DA_Lotti.Fill(DsMagazzino1.Lotti)

            Dim I As Integer = 0
            Dim L As Integer = 0
            Dim OC As Integer = 0 'GIU230218 si confonde con lo 0 zero
            Dim RowArtDiMag() As DataRow
            Dim RowAnaMag() As DataRow
            Dim RowLotti() As DataRow
            Dim Cod_Mag As Integer
            Dim Cod_Art As String
            Dim Cod_ArtAnaMag As String
            Dim TmpTipoDoc As String
            Dim TmpTipoDocAnaMag As String
            Dim TmpScGiacenza As Boolean
            Dim TmpTipoArticolo As Integer 'giu230219
            Dim dc As DataColumn 'giu281020
            Dim OKInsert As Boolean = False 'giu041120 per inserire in ARTDIMAG i protti con giacenza ma non in LISTINO
            '==Ciclo Movimenti di Magazzino ===============
            'sono esclusi dal calcolo 'OD' 'OC' 'OF' 'PF' 'PR' 'FC' 'FS' 'NC' 'NZ' tranne FC+ScGiacenza

            'FABIO 20/12/2011
            'MODIFICA - VADO AD AGGIORNARE LA TABELLA ANAMAG (CONTIENE IL TOTALE DEI CAMPI PRESENTI IN ARTDIMAG SENZA LA SUDDIVISIONE PER MAGAZZINO)
            'SCHEMA DEI CAMPI
            '|---------------|------------------|-----------------------------------------|
            '|    ANAMAG     |    ARTDIMAG      |                NOTE                     |              
            '|---------------|------------------|-----------------------------------------|
            '|Giacenza       | Giacenza         | Contengono Giacenza                     |
            '|Ord_Clienti    | Ord_Clienti      | Contengono gli ordini in corso clienti  | 
            '|Ordinato       | Ord_Clienti_Evasi| Contengono gli ordini clienti evasi     |
            '|Ord_Fornit     | Ordinati         | Contengono ordini fornitori             |
            '|QtaArrivoFornit| QtaArrivoFornit  | Contiene la Qta' i arrivo da foritori   |
            '|Venduto        | Venduto_P        | Contengono il venduto                   |
            '|Prodotto       | Prodotto_P       | Contengono il prodotto                  |
            '|Confezionato   | Confezionato_P   | Contengono il confezionato              |
            '|Giac_Impegnata | Giac_Impegnata   | Contengono la giacenza impegnata        |
            '|Giac_Prenotata | Giac_Prenotata   | Contengono la giacenza prenotata        |
            '|---------------|------------------|-----------------------------------------|
            strFase = "4. Ciclo aggiornamento giacenze (AnaMag,ArtDiMag)"
            For I = 0 To DsMagazzino1.MovimentiMagazzino.Rows.Count - 1       'ciclo movimenti di magazzino
                'FABIO 20/12/2011
                'SALVO IN TABELLA ANAMAG
                Cod_ArtAnaMag = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Cod_Articolo")
                strFase = "4.1 Ciclo aggiornamento giacenze (AnaMag) Articolo: " + Cod_ArtAnaMag
                RowAnaMag = DsMagazzino1.AnaMag.Select("Cod_Articolo = '" & Replace(Cod_ArtAnaMag, "'", "''") & "'")
                TmpTipoArticolo = 0 'DEFAULT ok viene cosiderato come Articolo
                If RowAnaMag.Length > 0 Then
                    TmpTipoDocAnaMag = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Tipo_Doc")
                    TmpScGiacenza = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("ScGiacenza") 'GIU111018
                    TmpTipoArticolo = IIf(IsDBNull(RowAnaMag(0).Item("TipoArticolo")), 0, RowAnaMag(0).Item("TipoArticolo")) 'giu230219
                    If TmpTipoArticolo = 0 Then
                        If (TmpTipoDocAnaMag = SWTD(TD.DocTrasportoClienti)) Or (TmpTipoDocAnaMag = SWTD(TD.DocTrasportoFornitori)) Or _
                       (TmpTipoDocAnaMag = SWTD(TD.BuonoConsegna)) Or (TmpTipoDocAnaMag = SWTD(TD.FatturaScontrino)) Or _
                       (TmpTipoDocAnaMag = SWTD(TD.DocTrasportoCLavoro)) Or (TmpTipoDocAnaMag = SWTD(TD.MovimentoMagazzino)) Or _
                       (TmpTipoDocAnaMag = SWTD(TD.CaricoMagazzino)) Or (TmpTipoDocAnaMag = SWTD(TD.ScaricoMagazzino)) Or _
                       (TmpTipoDocAnaMag = SWTD(TD.FatturaAccompagnatoria)) Or (TmpScGiacenza = True) Then 'GIU111018
                            'Muovo la giacenza per DT,MM, CM, SM 
                            If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Giacenza") = "+" Then
                                RowAnaMag(0).Item("Giacenza") = RowAnaMag(0).Item("Giacenza") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                            ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Giacenza") = "-" Then
                                RowAnaMag(0).Item("Giacenza") = RowAnaMag(0).Item("Giacenza") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                            End If
                        End If
                    End If

                    If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Prodotto") = "+" Then
                        RowAnaMag(0).Item("Prodotto") = RowAnaMag(0).Item("Prodotto") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                    ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Prodotto") = "-" Then
                        RowAnaMag(0).Item("Prodotto") = RowAnaMag(0).Item("Prodotto") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                    End If

                    If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Confezionato") = "+" Then
                        RowAnaMag(0).Item("Confezionato") = RowAnaMag(0).Item("Confezionato") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                    ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Confezionato") = "-" Then
                        RowAnaMag(0).Item("Confezionato") = RowAnaMag(0).Item("Confezionato") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                    End If

                    If (TmpTipoDocAnaMag = SWTD(TD.DocTrasportoClienti)) Or (TmpTipoDocAnaMag = SWTD(TD.DocTrasportoFornitori)) Or _
                       (TmpTipoDocAnaMag = SWTD(TD.BuonoConsegna)) Or (TmpTipoDocAnaMag = SWTD(TD.FatturaScontrino)) Or _
                       (TmpTipoDocAnaMag = SWTD(TD.DocTrasportoCLavoro)) Or (TmpTipoDocAnaMag = SWTD(TD.FatturaAccompagnatoria)) Or _
                       (TmpScGiacenza = True) Then 'GIU111018
                        'Muovo Venduto_P per DT, DF, DL, FA
                        'Escludo la Fatt.Commerciale
                        If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Venduto") = "+" Then
                            RowAnaMag(0).Item("Venduto") = RowAnaMag(0).Item("Venduto") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                        ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Venduto") = "-" Then
                            RowAnaMag(0).Item("Venduto") = RowAnaMag(0).Item("Venduto") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                        End If
                    End If
                End If
                'FINE FABIO 20/05/2011

                If Not IsDBNull(DsMagazzino1.MovimentiMagazzino.Rows(I).Item("CodiceMagazzino")) Then
                    Cod_Mag = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("CodiceMagazzino")
                Else
                    Cod_Mag = 1 'giu281020 no 0 ok 1 Magazzino valido 'Pier 12/01/2012 - concordato con Giuseppe, se Null muovo su Magazzino 0
                End If
                Cod_Art = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Cod_Articolo")
                strFase = "4.2 Ciclo aggiornamento giacenze (ArtDiMag) Articolo: " + Cod_Art
                RowArtDiMag = DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = " & Cod_Mag & " AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'")
                If RowArtDiMag.Length > 0 Then
                    TmpTipoDoc = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Tipo_Doc")
                    TmpScGiacenza = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("ScGiacenza") 'GIU111018
                    If TmpTipoArticolo = 0 Then
                        If (TmpTipoDoc = SWTD(TD.DocTrasportoClienti)) Or (TmpTipoDoc = SWTD(TD.DocTrasportoFornitori)) Or _
                       (TmpTipoDoc = SWTD(TD.BuonoConsegna)) Or (TmpTipoDoc = SWTD(TD.FatturaScontrino)) Or _
                       (TmpTipoDoc = SWTD(TD.DocTrasportoCLavoro)) Or (TmpTipoDoc = SWTD(TD.MovimentoMagazzino)) Or _
                       (TmpTipoDoc = SWTD(TD.CaricoMagazzino)) Or (TmpTipoDoc = SWTD(TD.ScaricoMagazzino)) Or _
                       (TmpTipoDoc = SWTD(TD.FatturaAccompagnatoria)) Or (TmpScGiacenza = True) Then
                            'Muovo la giacenza per DT,MM, CM, SM
                            If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Giacenza") = "+" Then
                                RowArtDiMag(0).Item("Giacenza") = RowArtDiMag(0).Item("Giacenza") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                            ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Giacenza") = "-" Then
                                RowArtDiMag(0).Item("Giacenza") = RowArtDiMag(0).Item("Giacenza") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                            End If
                        End If
                    End If

                    If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Prodotto") = "+" Then
                        RowArtDiMag(0).Item("Prodotto_P") = RowArtDiMag(0).Item("Prodotto_P") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                    ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Prodotto") = "-" Then
                        RowArtDiMag(0).Item("Prodotto_P") = RowArtDiMag(0).Item("Prodotto_P") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                    End If

                    If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Confezionato") = "+" Then
                        RowArtDiMag(0).Item("Confezionato_P") = RowArtDiMag(0).Item("Confezionato_P") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                    ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Confezionato") = "-" Then
                        RowArtDiMag(0).Item("Confezionato_P") = RowArtDiMag(0).Item("Confezionato_P") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                    End If

                    If (TmpTipoDoc = SWTD(TD.DocTrasportoClienti)) Or (TmpTipoDoc = SWTD(TD.DocTrasportoFornitori)) Or _
                       (TmpTipoDoc = SWTD(TD.BuonoConsegna)) Or (TmpTipoDoc = SWTD(TD.FatturaScontrino)) Or _
                       (TmpTipoDoc = SWTD(TD.DocTrasportoCLavoro)) Or (TmpTipoDoc = SWTD(TD.FatturaAccompagnatoria)) Or _
                       (TmpScGiacenza = True) Then
                        'Muovo Venduto_P per DT, DF, DL, FA
                        'Escludo la Fatt.Commerciale
                        If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Venduto") = "+" Then
                            RowArtDiMag(0).Item("Venduto_P") = RowArtDiMag(0).Item("Venduto_P") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                        ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Venduto") = "-" Then
                            RowArtDiMag(0).Item("Venduto_P") = RowArtDiMag(0).Item("Venduto_P") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                        End If
                    End If
                Else 'giu250920 inserisco anche se non è incluso, cosi evito di inserirli GIU031120 SOLO SE LA SOMMAGIACENZA E' DIVERSA DA ZERO
                    If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita") <> 0 Then
                        OKInsert = False
                        Dim newRowArtDiMag As DsMagazzino.ArtdiMagRow
                        newRowArtDiMag = DsMagazzino1.ArtdiMag.NewRow
                        '-init
                        newRowArtDiMag.Item("Codice_Magazzino") = Cod_Mag
                        newRowArtDiMag.Item("Cod_Articolo") = Cod_Art
                        newRowArtDiMag.Item("Giacenza") = 0
                        newRowArtDiMag.Item("Ord_Clienti") = 0
                        newRowArtDiMag.Item("Reparto") = DBNull.Value
                        newRowArtDiMag.Item("Scaffale") = DBNull.Value
                        newRowArtDiMag.Item("Piano") = 0
                        newRowArtDiMag.Item("Ordinati") = 0
                        newRowArtDiMag.Item("Data_Arrivo") = DBNull.Value
                        newRowArtDiMag.Item("Prodotto_P") = 0
                        newRowArtDiMag.Item("Confezionato_P") = 0
                        newRowArtDiMag.Item("Ordinato_P") = 0
                        newRowArtDiMag.Item("Venduto_P") = 0
                        newRowArtDiMag.Item("Giac_Impegnata") = 0
                        newRowArtDiMag.Item("Giac_Prenotata") = 0
                        newRowArtDiMag.Item("Ord_Clienti_Evasi") = 0
                        newRowArtDiMag.Item("QtaArrivoFornit") = 0
                        newRowArtDiMag.Item("Sottoscorta") = 0
                        '-ok
                        TmpTipoDoc = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Tipo_Doc")
                        TmpScGiacenza = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("ScGiacenza") 'GIU111018
                        If TmpTipoArticolo = 0 Then
                            If (TmpTipoDoc = SWTD(TD.DocTrasportoClienti)) Or (TmpTipoDoc = SWTD(TD.DocTrasportoFornitori)) Or _
                           (TmpTipoDoc = SWTD(TD.BuonoConsegna)) Or (TmpTipoDoc = SWTD(TD.FatturaScontrino)) Or _
                           (TmpTipoDoc = SWTD(TD.DocTrasportoCLavoro)) Or (TmpTipoDoc = SWTD(TD.MovimentoMagazzino)) Or _
                           (TmpTipoDoc = SWTD(TD.CaricoMagazzino)) Or (TmpTipoDoc = SWTD(TD.ScaricoMagazzino)) Or _
                           (TmpTipoDoc = SWTD(TD.FatturaAccompagnatoria)) Or (TmpScGiacenza = True) Then
                                'Muovo la giacenza per DT,MM, CM, SM
                                If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Giacenza") = "+" Then
                                    newRowArtDiMag.Item("Giacenza") = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Giacenza") = "-" Then
                                    newRowArtDiMag.Item("Giacenza") = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita") * -1
                                End If
                                OKInsert = True
                            End If
                        End If

                        If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Prodotto") = "+" Then
                            newRowArtDiMag.Item("Prodotto_P") = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                        ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Prodotto") = "-" Then
                            newRowArtDiMag.Item("Prodotto_P") = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita") * -1
                        End If

                        If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Confezionato") = "+" Then
                            newRowArtDiMag.Item("Confezionato_P") = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                        ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Confezionato") = "-" Then
                            newRowArtDiMag.Item("Confezionato_P") = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita") * -1
                        End If

                        If (TmpTipoDoc = SWTD(TD.DocTrasportoClienti)) Or (TmpTipoDoc = SWTD(TD.DocTrasportoFornitori)) Or _
                           (TmpTipoDoc = SWTD(TD.BuonoConsegna)) Or (TmpTipoDoc = SWTD(TD.FatturaScontrino)) Or _
                           (TmpTipoDoc = SWTD(TD.DocTrasportoCLavoro)) Or (TmpTipoDoc = SWTD(TD.FatturaAccompagnatoria)) Or _
                           (TmpScGiacenza = True) Then
                            'Muovo Venduto_P per DT, DF, DL, FA
                            'Escludo la Fatt.Commerciale
                            If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Venduto") = "+" Then
                                newRowArtDiMag.Item("Venduto_P") = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                            ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Venduto") = "-" Then
                                newRowArtDiMag.Item("Venduto_P") = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita") * -1
                            End If
                        End If
                        If OKInsert = True Then
                            DsMagazzino1.ArtdiMag.AddArtdiMagRow(newRowArtDiMag)
                            'giu070323 aggiunto AND DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = " & Cod_Mag & " AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'")
                            If Cod_Mag <> 0 And DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = 0 AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'").Length = 0 Then 'giu291020
                                Dim newRowArtDiMag0 As DsMagazzino.ArtdiMagRow
                                newRowArtDiMag0 = DsMagazzino1.ArtdiMag.NewRow
                                For Each dc In DsMagazzino1.ArtdiMag.Columns
                                    newRowArtDiMag0.Item(dc.ColumnName) = newRowArtDiMag.Item(dc.ColumnName)
                                Next
                                newRowArtDiMag0.Item("Codice_Magazzino") = 0
                                DsMagazzino1.ArtdiMag.AddArtdiMagRow(newRowArtDiMag0)
                            Else 'GIU130323 OK ESISTE NEL MAG0 QUINDI SOMMO
                                RowArtDiMag = DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = 0 AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'")
                                If RowArtDiMag.Length > 0 Then
                                    TmpTipoDoc = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Tipo_Doc")
                                    TmpScGiacenza = DsMagazzino1.MovimentiMagazzino.Rows(I).Item("ScGiacenza") 'GIU111018
                                    If TmpTipoArticolo = 0 Then
                                        If (TmpTipoDoc = SWTD(TD.DocTrasportoClienti)) Or (TmpTipoDoc = SWTD(TD.DocTrasportoFornitori)) Or _
                                       (TmpTipoDoc = SWTD(TD.BuonoConsegna)) Or (TmpTipoDoc = SWTD(TD.FatturaScontrino)) Or _
                                       (TmpTipoDoc = SWTD(TD.DocTrasportoCLavoro)) Or (TmpTipoDoc = SWTD(TD.MovimentoMagazzino)) Or _
                                       (TmpTipoDoc = SWTD(TD.CaricoMagazzino)) Or (TmpTipoDoc = SWTD(TD.ScaricoMagazzino)) Or _
                                       (TmpTipoDoc = SWTD(TD.FatturaAccompagnatoria)) Or (TmpScGiacenza = True) Then
                                            'Muovo la giacenza per DT,MM, CM, SM
                                            If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Giacenza") = "+" Then
                                                RowArtDiMag(0).Item("Giacenza") = RowArtDiMag(0).Item("Giacenza") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                            ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Giacenza") = "-" Then
                                                RowArtDiMag(0).Item("Giacenza") = RowArtDiMag(0).Item("Giacenza") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                            End If
                                        End If
                                    End If

                                    If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Prodotto") = "+" Then
                                        RowArtDiMag(0).Item("Prodotto_P") = RowArtDiMag(0).Item("Prodotto_P") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                    ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Prodotto") = "-" Then
                                        RowArtDiMag(0).Item("Prodotto_P") = RowArtDiMag(0).Item("Prodotto_P") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                    End If

                                    If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Confezionato") = "+" Then
                                        RowArtDiMag(0).Item("Confezionato_P") = RowArtDiMag(0).Item("Confezionato_P") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                    ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Confezionato") = "-" Then
                                        RowArtDiMag(0).Item("Confezionato_P") = RowArtDiMag(0).Item("Confezionato_P") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                    End If

                                    If (TmpTipoDoc = SWTD(TD.DocTrasportoClienti)) Or (TmpTipoDoc = SWTD(TD.DocTrasportoFornitori)) Or _
                                       (TmpTipoDoc = SWTD(TD.BuonoConsegna)) Or (TmpTipoDoc = SWTD(TD.FatturaScontrino)) Or _
                                       (TmpTipoDoc = SWTD(TD.DocTrasportoCLavoro)) Or (TmpTipoDoc = SWTD(TD.FatturaAccompagnatoria)) Or _
                                       (TmpScGiacenza = True) Then
                                        'Muovo Venduto_P per DT, DF, DL, FA
                                        'Escludo la Fatt.Commerciale
                                        If DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Venduto") = "+" Then
                                            RowArtDiMag(0).Item("Venduto_P") = RowArtDiMag(0).Item("Venduto_P") + DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                        ElseIf DsMagazzino1.MovimentiMagazzino.Rows(I).Item("Segno_Venduto") = "-" Then
                                            RowArtDiMag(0).Item("Venduto_P") = RowArtDiMag(0).Item("Venduto_P") - DsMagazzino1.MovimentiMagazzino.Rows(I).Item("SommaDiQuantita")
                                        End If
                                    End If
                                End If
                            End If
                        End If

                    End If
                End If
            Next
            'giu190613 giu280920
            If DsMagazzino1.ArtdiMag.Select("Giacenza<0 and Codice_Magazzino = " & Cod_Mag.ToString.Trim & "").Length > 0 Then 'giu280920 Or DsMagazzino1.AnaMag.Select("Giacenza<0").Length > 0 Then
                SWNegativi = True
            Else
                SWNegativi = False
            End If
            '---------
            '==Ciclo Lotti ===============
            'GIU271120 LA STORE PROCEDURE get_MovimentiMagazzinoLotti RITORNA SEMPRE 0 ZERO RKS TANTO NON SERVE
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            strFase = "4. Ciclo aggiornamento giacenze Lotti"
            For L = 0 To DsMagazzino1.MovimentiMagazzinoLotti.Rows.Count - 1
                RowLotti = DsMagazzino1.Lotti.Select("Cod_Magaz = " & DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("CodiceMagazzino") & _
                        " And Cod_Articolo =  '" & Replace(DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Cod_Articolo"), "'", "''") & "'" & _
                        " And Cod_Lotto ='" & Replace(IIf(IsDBNull(DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Lotto")), "", DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Lotto")), "'", "''") & "'" & _
                        " And NSerie ='" & Replace(IIf(IsDBNull(DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("NSerie")), "", DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("NSerie")), "'", "''") & "'")
                If RowLotti.Length = 0 Then
                    Dim RowL As DsMagazzino.LottiRow
                    RowL = DsMagazzino1.Lotti.NewRow

                    RowL.Item("Selezionato") = 0
                    RowL.Item("Cod_Articolo") = DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Cod_Articolo")
                    RowL.Item("Cod_Lotto") = IIf(IsDBNull(DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Lotto")), "", DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Lotto"))
                    RowL.Item("Cod_Magaz") = IIf(IsDBNull(DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("CodiceMagazzino")), 0, DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("CodiceMagazzino"))
                    RowL.Item("NSerie") = IIf(IsDBNull(DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("NSerie")), "", DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("NSerie"))
                    RowL.Item("Data_Produzione") = DBNull.Value
                    RowL.Item("Data_Scadenza") = DBNull.Value
                    If DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Segno_Lotti") = "+" Then
                        RowL.Item("Giacenza") = DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("QtaColli")
                        RowL.Item("Qta_Caricata") = DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("QtaColli")
                    ElseIf DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Segno_Lotti") = "-" Then
                        RowL.Item("Giacenza") = DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("QtaColli") * -1
                        RowL.Item("Qta_Caricata") = DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("QtaColli") * -1
                    End If
                    DsMagazzino1.Lotti.AddLottiRow(RowL)
                Else
                    If DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Segno_Lotti") = "+" Then
                        RowLotti(0).Item("Giacenza") = RowLotti(0).Item("Giacenza") + DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("QtaColli")
                        RowLotti(0).Item("Qta_Caricata") = RowLotti(0).Item("Qta_Caricata") + DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("QtaColli")
                    ElseIf DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("Segno_Lotti") = "-" Then
                        RowLotti(0).Item("Giacenza") = RowLotti(0).Item("Giacenza") - DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("QtaColli")
                        RowLotti(0).Item("Qta_Caricata") = RowLotti(0).Item("Qta_Caricata") - DsMagazzino1.MovimentiMagazzinoLotti.Rows(L).Item("QtaColli")
                    End If
                End If
            Next

            '== Ciclo Ordini Clienti ===============
            strFase = "5. Ciclo aggiornamento Ordinato Clienti"
            For OC = 0 To DsMagazzino1.MovimentiMagazzinoOrdini.Rows.Count - 1
                'FABIO 21/12/2011
                'AGGIORNO TABELLA ANAMAG
                Cod_ArtAnaMag = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Cod_Articolo")
                RowAnaMag = DsMagazzino1.AnaMag.Select("Cod_Articolo =  '" & Replace(Cod_ArtAnaMag, "'", "''") & "'")
                If RowAnaMag.Length > 0 Then
                    'Pier 11/01/2012 - da fare il 12/01/2012 verificare campo Ordinato
                    If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("StatoDoc") = 1 Then 'ORDINI EVASI
                        RowAnaMag(0).Item("Ordinato") = RowAnaMag(0).Item("Ordinato") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta2")  'QTA INVIATA (utilizza campo Qta_Allestita)
                    Else 'PER GLI ALTRI STATI DOC (0 E 2)
                        RowAnaMag(0).Item("Ord_Clienti") = RowAnaMag(0).Item("Ord_Clienti") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")       'QTA ORDINATA IN ESSERE
                        RowAnaMag(0).Item("Ordinato") = RowAnaMag(0).Item("Ordinato") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta2")  'QTA INVIATA (utilizza campo Qta_Allestita)
                    End If
                    'giu190613 correzione errore Giacenze negative
                    If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("StatoDoc") <> 1 Then 'ORDINI EVASI
                        Dim TmpGiacenza As Decimal = RowAnaMag(0).Item("Giacenza") 'giu190613
                        If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1") > 0 And _
                            TmpGiacenza > 0 Then 'Qta1 è il Residuo = [Qta_Ordinata]-[Qta_Allestita]
                            If RowAnaMag(0).Item("Giacenza") >= DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1") Then
                                RowAnaMag(0).Item("Giacenza") = RowAnaMag(0).Item("Giacenza") - DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                'SPOSTO QUANTITA' IMPEGNATA DA GIACENZA A GIACENZA_IMPEGNATA
                                RowAnaMag(0).Item("Giac_Impegnata") = RowAnaMag(0).Item("Giac_Impegnata") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                '' ''===IMPEGNO L'ORDINE =================================='GIU251120 SOSTATO SOTTO IN ARTDIMAG
                                ' ''SqlUpd_ImpArticolo.Parameters("@IDDocumenti").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("IDDocumenti")
                                ' ''SqlUpd_ImpArticolo.Parameters("@Riga").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Riga")
                                ' ''SqlUpd_ImpArticolo.Parameters("@QtaImpegnata").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                ' ''SqlUpd_ImpArticolo.ExecuteNonQuery()
                                '' ''======================================================
                            Else
                                RowAnaMag(0).Item("Giacenza") = 0  'Non c'è giacenza per coprire tutto l'impegno, quindi azzero la giacenza
                                If TmpGiacenza > 0 Then 'Se la giacenza è già a zero, non devo più impegnare....
                                    RowAnaMag(0).Item("Giac_Impegnata") = RowAnaMag(0).Item("Giac_Impegnata") + TmpGiacenza 'impegno solo quanto è rimasto in Giacenza
                                    '' ''===IMPEGNO L'ORDINE =================================='GIU251120 SOSTATO SOTTO IN ARTDIMAG
                                    ' ''SqlUpd_ImpArticolo.Parameters("@IDDocumenti").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("IDDocumenti")
                                    ' ''SqlUpd_ImpArticolo.Parameters("@Riga").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Riga")
                                    ' ''SqlUpd_ImpArticolo.Parameters("@QtaImpegnata").Value = TmpGiacenza
                                    ' ''SqlUpd_ImpArticolo.ExecuteNonQuery()
                                    '' ''======================================================
                                End If
                            End If
                        End If
                    End If
                    '== Fine Pier 11/01/2012 =============================================================
                    RowAnaMag(0).Item("Giac_Prenotata") = RowAnaMag(0).Item("Giac_Prenotata") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("QtaPren")
                End If
                'FINE FABIO 21/12/2011

                If Not IsDBNull(DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("CodiceMagazzino")) Then
                    Cod_Mag = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("CodiceMagazzino")
                Else
                    Cod_Mag = 0 'Pier 12/01/2012 - concordato con Giuseppe, se Null muovo su Magazzino 0
                End If
                Cod_Art = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Cod_Articolo")
                RowArtDiMag = DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = " & Cod_Mag & " AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'")
                If RowArtDiMag.Length > 0 Then
                    If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("StatoDoc") = 1 Then 'ORDINI EVASI
                        RowArtDiMag(0).Item("Ord_Clienti_Evasi") = RowArtDiMag(0).Item("Ord_Clienti_Evasi") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta2")
                        'QTA INVIATA (utilizza campo Qta_Allestita)
                    Else 'PER GLI ALTRI STATI DOC (0 E 2)
                        RowArtDiMag(0).Item("Ord_Clienti") = RowArtDiMag(0).Item("Ord_Clienti") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                        'QTA ORDINATA IN ESSERE
                        RowArtDiMag(0).Item("Ord_Clienti_Evasi") = RowArtDiMag(0).Item("Ord_Clienti_Evasi") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta2")
                        'QTA INVIATA (utilizza campo Qta_Allestita)
                    End If
                    'giu190613 correzione errore Giacenze negative
                    If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("StatoDoc") <> 1 Then 'ORDINI EVASI
                        Dim TmpGiacenza As Decimal = RowArtDiMag(0).Item("Giacenza") 'giu190613
                        If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1") > 0 And _
                            TmpGiacenza > 0 Then
                            If RowArtDiMag(0).Item("Giacenza") >= DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1") Then
                                RowArtDiMag(0).Item("Giacenza") = RowArtDiMag(0).Item("Giacenza") - DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                'SPOSTO QUANTITA' IMPEGNATA DA GIACENZA A GIACENZA_IMPEGNATA
                                RowArtDiMag(0).Item("Giac_Impegnata") = RowArtDiMag(0).Item("Giac_Impegnata") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                '===IMPEGNO L'ORDINE =================================='GIU251120
                                SqlUpd_ImpArticolo.Parameters("@IDDocumenti").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("IDDocumenti")
                                SqlUpd_ImpArticolo.Parameters("@Riga").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Riga")
                                SqlUpd_ImpArticolo.Parameters("@QtaImpegnata").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                SqlUpd_ImpArticolo.ExecuteNonQuery()
                                '======================================================
                            Else
                                RowArtDiMag(0).Item("Giacenza") = 0 'Non c'è giacenza per coprire tutto l'impegno, quindi azzero la giacenza
                                If TmpGiacenza > 0 Then 'Se la giacenza è già a zero, non devo più impegnare....
                                    RowArtDiMag(0).Item("Giac_Impegnata") = RowArtDiMag(0).Item("Giac_Impegnata") + TmpGiacenza 'impegno solo quanto è rimasto in Giacenza
                                    '===IMPEGNO L'ORDINE =================================='GIU251120 
                                    SqlUpd_ImpArticolo.Parameters("@IDDocumenti").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("IDDocumenti")
                                    SqlUpd_ImpArticolo.Parameters("@Riga").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Riga")
                                    SqlUpd_ImpArticolo.Parameters("@QtaImpegnata").Value = TmpGiacenza
                                    SqlUpd_ImpArticolo.ExecuteNonQuery()
                                    '======================================================
                                End If
                            End If
                        End If
                    End If
                    '== Fine Pier 12/01/2012 =============================================================
                    RowArtDiMag(0).Item("Giac_Prenotata") = RowArtDiMag(0).Item("Giac_Prenotata") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("QtaPren")
                Else 'giu250920 inserisco anche se non è incluso, cosi evito di inserirli
                    OKInsert = False
                    Dim newRowArtDiMag As DsMagazzino.ArtdiMagRow
                    newRowArtDiMag = DsMagazzino1.ArtdiMag.NewRow
                    '-init
                    newRowArtDiMag.Item("Codice_Magazzino") = Cod_Mag
                    newRowArtDiMag.Item("Cod_Articolo") = Cod_Art
                    newRowArtDiMag.Item("Giacenza") = 0
                    newRowArtDiMag.Item("Ord_Clienti") = 0
                    newRowArtDiMag.Item("Reparto") = DBNull.Value
                    newRowArtDiMag.Item("Scaffale") = DBNull.Value
                    newRowArtDiMag.Item("Piano") = 0
                    newRowArtDiMag.Item("Ordinati") = 0
                    newRowArtDiMag.Item("Data_Arrivo") = DBNull.Value
                    newRowArtDiMag.Item("Prodotto_P") = 0
                    newRowArtDiMag.Item("Confezionato_P") = 0
                    newRowArtDiMag.Item("Ordinato_P") = 0
                    newRowArtDiMag.Item("Venduto_P") = 0
                    newRowArtDiMag.Item("Giac_Impegnata") = 0
                    newRowArtDiMag.Item("Giac_Prenotata") = 0
                    newRowArtDiMag.Item("Ord_Clienti_Evasi") = 0
                    newRowArtDiMag.Item("QtaArrivoFornit") = 0
                    newRowArtDiMag.Item("Sottoscorta") = 0
                    '-ok
                    If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("StatoDoc") = 1 Then 'ORDINI EVASI
                        newRowArtDiMag.Item("Ord_Clienti_Evasi") = newRowArtDiMag.Item("Ord_Clienti_Evasi") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta2")
                        'QTA INVIATA (utilizza campo Qta_Allestita)
                    Else 'PER GLI ALTRI STATI DOC (0 E 2)
                        newRowArtDiMag.Item("Ord_Clienti") = newRowArtDiMag.Item("Ord_Clienti") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                        'QTA ORDINATA IN ESSERE
                        newRowArtDiMag.Item("Ord_Clienti_Evasi") = newRowArtDiMag.Item("Ord_Clienti_Evasi") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta2")
                        'QTA INVIATA (utilizza campo Qta_Allestita)
                    End If
                    'giu190613 correzione errore Giacenze negative
                    If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("StatoDoc") <> 1 Then 'ORDINI EVASI
                        Dim TmpGiacenza As Decimal = newRowArtDiMag.Item("Giacenza") 'giu190613
                        If DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1") > 0 And _
                            TmpGiacenza > 0 Then
                            If newRowArtDiMag.Item("Giacenza") >= DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1") Then
                                newRowArtDiMag.Item("Giacenza") = newRowArtDiMag.Item("Giacenza") - DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                'SPOSTO QUANTITA' IMPEGNATA DA GIACENZA A GIACENZA_IMPEGNATA
                                newRowArtDiMag.Item("Giac_Impegnata") = newRowArtDiMag.Item("Giac_Impegnata") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                '===IMPEGNO L'ORDINE =================================='GIU251120
                                SqlUpd_ImpArticolo.Parameters("@IDDocumenti").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("IDDocumenti")
                                SqlUpd_ImpArticolo.Parameters("@Riga").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Riga")
                                SqlUpd_ImpArticolo.Parameters("@QtaImpegnata").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Qta1")
                                SqlUpd_ImpArticolo.ExecuteNonQuery()
                                '======================================================
                            Else
                                newRowArtDiMag.Item("Giacenza") = 0 'Non c'è giacenza per coprire tutto l'impegno, quindi azzero la giacenza
                                If TmpGiacenza > 0 Then 'Se la giacenza è già a zero, non devo più impegnare....
                                    newRowArtDiMag.Item("Giac_Impegnata") = newRowArtDiMag.Item("Giac_Impegnata") + TmpGiacenza 'impegno solo quanto è rimasto in Giacenza
                                    '===IMPEGNO L'ORDINE =================================='GIU251120 
                                    SqlUpd_ImpArticolo.Parameters("@IDDocumenti").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("IDDocumenti")
                                    SqlUpd_ImpArticolo.Parameters("@Riga").Value = DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("Riga")
                                    SqlUpd_ImpArticolo.Parameters("@QtaImpegnata").Value = TmpGiacenza
                                    SqlUpd_ImpArticolo.ExecuteNonQuery()
                                    '======================================================
                                End If
                            End If
                            OKInsert = True 'giu111020
                        End If
                    End If
                    '== Fine Pier 12/01/2012 =============================================================
                    newRowArtDiMag.Item("Giac_Prenotata") = newRowArtDiMag.Item("Giac_Prenotata") + DsMagazzino1.MovimentiMagazzinoOrdini.Rows(OC).Item("QtaPren")
                    If OKInsert = True Then 'giu111020
                        DsMagazzino1.ArtdiMag.AddArtdiMagRow(newRowArtDiMag)
                        'giu070323 aggiunto And DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = 0 AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'").Length = 0 Then
                        If Cod_Mag <> 0 And DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = 0 AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'").Length = 0 Then 'giu291020
                            Dim newRowArtDiMag0 As DsMagazzino.ArtdiMagRow
                            newRowArtDiMag0 = DsMagazzino1.ArtdiMag.NewRow
                            For Each dc In DsMagazzino1.ArtdiMag.Columns
                                newRowArtDiMag0.Item(dc.ColumnName) = newRowArtDiMag.Item(dc.ColumnName)
                            Next
                            newRowArtDiMag0.Item("Codice_Magazzino") = 0
                            DsMagazzino1.ArtdiMag.AddArtdiMagRow(newRowArtDiMag0)
                        End If
                    End If
                    '-
                End If
            Next

            '== Ciclo Ordini Fornitori ===============
            strFase = "6. Ciclo aggiornamento Ordinato Fornitori"
            For OC = 0 To DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows.Count - 1
                'FABIO 21/12/2011
                Cod_ArtAnaMag = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("Cod_Articolo")
                RowAnaMag = DsMagazzino1.AnaMag.Select("Cod_Articolo =  '" & Replace(Cod_ArtAnaMag, "'", "''") & "'")

                'Ricerco la data di consegna
                Dim TmpDataOraConsegna As Date
                If IsDBNull(DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("DataOraConsegna")) And IsDBNull(DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("NGG_Consegna")) Then
                    TmpDataOraConsegna = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("Data_Doc")
                ElseIf Not IsDBNull(DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("DataOraConsegna")) Then
                    TmpDataOraConsegna = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("DataOraConsegna")
                ElseIf Not IsDBNull(DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("NGG_Consegna")) Then
                    TmpDataOraConsegna = DateAdd(DateInterval.Day, DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("NGG_Consegna"), DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("Data_Doc"))
                End If
                '===================================================

                If RowAnaMag.Length > 0 Then
                    RowAnaMag(0).Item("Ord_Fornit") = RowAnaMag(0).Item("Ord_Fornit") + DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("Qta1")

                    If Not IsDBNull(RowAnaMag(0).Item("Data_Arrivo")) Then
                        If CDate(RowAnaMag(0).Item("Data_Arrivo")) = TmpDataOraConsegna Then 'Se è uguale sommo, altrimenti vuol dire che è più recente. Non può essere maggiore.
                            RowAnaMag(0).Item("QtaArrivoFornit") = RowAnaMag(0).Item("QtaArrivoFornit") + DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                            RowAnaMag(0).Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                        Else
                            If DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit") > 0 Then 'Pier 23/01/2012 solo se è maggiore di zero
                                RowAnaMag(0).Item("QtaArrivoFornit") = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                                RowAnaMag(0).Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                            End If
                        End If
                    Else
                        If DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit") > 0 Then 'Pier 23/01/2012 solo se è maggiore di zero
                            RowAnaMag(0).Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                            RowAnaMag(0).Item("QtaArrivoFornit") = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                        End If
                    End If
                    If RowAnaMag(0).Item("Ord_Fornit") <= 0 Then
                        RowAnaMag(0).Item("Data_Arrivo") = DBNull.Value
                    End If
                End If
                'FINE FABIO 21/12/2011

                If Not IsDBNull(DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("CodiceMagazzino")) Then
                    Cod_Mag = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("CodiceMagazzino")
                Else
                    Cod_Mag = 0 'Pier 12/01/2012 - concordato con Giuseppe, se Null muovo su Magazzino 0
                End If
                Cod_Art = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("Cod_Articolo")
                RowArtDiMag = DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = " & Cod_Mag & " AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'")
                If RowArtDiMag.Length > 0 Then
                    RowArtDiMag(0).Item("Ordinati") = RowArtDiMag(0).Item("Ordinati") + DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("Qta1")
                    If Not IsDBNull(RowArtDiMag(0).Item("Data_Arrivo")) Then
                        If RowArtDiMag(0).Item("Data_Arrivo") = TmpDataOraConsegna Then 'Se è uguale sommo, altrimenti vuol dire che è più recente. Non può essere maggiore.
                            RowArtDiMag(0).Item("QtaArrivoFornit") = RowArtDiMag(0).Item("QtaArrivoFornit") + DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                            RowArtDiMag(0).Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                        Else
                            If DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit") > 0 Then    'Pier 23/01/2012 solo se è maggiore di zero
                                RowArtDiMag(0).Item("QtaArrivoFornit") = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                                RowArtDiMag(0).Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                            End If
                        End If
                    Else
                        If DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit") > 0 Then    'Pier 23/01/2012 solo se è maggiore di zero
                            RowArtDiMag(0).Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                            RowArtDiMag(0).Item("QtaArrivoFornit") = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                        End If
                    End If

                    If RowArtDiMag(0).Item("Ordinati") <= 0 Then
                        RowArtDiMag(0).Item("Data_Arrivo") = DBNull.Value
                    End If
                Else 'giu250920 inserisco anche se non è incluso, cosi evito di inserirli
                    OKInsert = False 'giu111020
                    Dim newRowArtDiMag As DsMagazzino.ArtdiMagRow
                    newRowArtDiMag = DsMagazzino1.ArtdiMag.NewRow
                    '-init
                    newRowArtDiMag.Item("Codice_Magazzino") = Cod_Mag
                    newRowArtDiMag.Item("Cod_Articolo") = Cod_Art
                    newRowArtDiMag.Item("Giacenza") = 0
                    newRowArtDiMag.Item("Ord_Clienti") = 0
                    newRowArtDiMag.Item("Reparto") = DBNull.Value
                    newRowArtDiMag.Item("Scaffale") = DBNull.Value
                    newRowArtDiMag.Item("Piano") = 0
                    newRowArtDiMag.Item("Ordinati") = 0
                    newRowArtDiMag.Item("Data_Arrivo") = DBNull.Value
                    newRowArtDiMag.Item("Prodotto_P") = 0
                    newRowArtDiMag.Item("Confezionato_P") = 0
                    newRowArtDiMag.Item("Ordinato_P") = 0
                    newRowArtDiMag.Item("Venduto_P") = 0
                    newRowArtDiMag.Item("Giac_Impegnata") = 0
                    newRowArtDiMag.Item("Giac_Prenotata") = 0
                    newRowArtDiMag.Item("Ord_Clienti_Evasi") = 0
                    newRowArtDiMag.Item("QtaArrivoFornit") = 0
                    newRowArtDiMag.Item("Sottoscorta") = 0
                    '-ok
                    newRowArtDiMag.Item("Ordinati") = newRowArtDiMag.Item("Ordinati") + DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("Qta1")
                    If Not IsDBNull(newRowArtDiMag.Item("Data_Arrivo")) Then
                        If newRowArtDiMag.Item("Data_Arrivo") = TmpDataOraConsegna Then 'Se è uguale sommo, altrimenti vuol dire che è più recente. Non può essere maggiore.
                            newRowArtDiMag.Item("QtaArrivoFornit") = newRowArtDiMag.Item("QtaArrivoFornit") + DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                            newRowArtDiMag.Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                        Else
                            If DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit") > 0 Then    'Pier 23/01/2012 solo se è maggiore di zero
                                newRowArtDiMag.Item("QtaArrivoFornit") = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                                newRowArtDiMag.Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                                OKInsert = True
                            End If
                        End If
                    Else
                        If DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit") > 0 Then    'Pier 23/01/2012 solo se è maggiore di zero
                            newRowArtDiMag.Item("Data_Arrivo") = TmpDataOraConsegna  'inserisce ultima data di consegna (la più lontana)
                            newRowArtDiMag.Item("QtaArrivoFornit") = DsMagazzino1.MovimentiMagazzinoOrdiniFor.Rows(OC).Item("QtaArrivoFornit")
                            OKInsert = True
                        End If
                    End If

                    If newRowArtDiMag.Item("Ordinati") <= 0 Then
                        newRowArtDiMag.Item("Data_Arrivo") = DBNull.Value
                    Else
                        OKInsert = True
                    End If
                    '-
                    If OKInsert = True Then 'giu111020
                        DsMagazzino1.ArtdiMag.AddArtdiMagRow(newRowArtDiMag)
                        'giu070323 aggiunto And DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = 0 AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'").Length = 0 Then
                        If Cod_Mag <> 0 And DsMagazzino1.ArtdiMag.Select("Codice_Magazzino = 0 AND Cod_Articolo =  '" & Replace(Cod_Art, "'", "''") & "'").Length = 0 Then 'giu291020
                            Dim newRowArtDiMag0 As DsMagazzino.ArtdiMagRow
                            newRowArtDiMag0 = DsMagazzino1.ArtdiMag.NewRow
                            For Each dc In DsMagazzino1.ArtdiMag.Columns
                                newRowArtDiMag0.Item(dc.ColumnName) = newRowArtDiMag.Item(dc.ColumnName)
                            Next
                            newRowArtDiMag0.Item("Codice_Magazzino") = 0
                            DsMagazzino1.ArtdiMag.AddArtdiMagRow(newRowArtDiMag0)
                        End If
                    End If
                    '-
                End If
            Next
            'DA COMPLETARE CON LA GESTIONE DELLE CONFEZIONI
            strFase = "7. Aggiornamento tabelle AnaMag,ArtDiMag,Lotti"
            Da_AnaMag.Update(DsMagazzino1.AnaMag) 'FABIO 21/12/2011 'AGGIORNO ANAMAG NEL DB
            DA_ArtDiMag.Update(DsMagazzino1.ArtdiMag)
            Try
                DA_Lotti.Update(DsMagazzino1.Lotti)
            Catch ex As Exception
                'ok proseguo solo per i lotti giu300423
            End Try
            'GIU111120 TransTmp.Commit()
            If SqlConnMagaz1.State <> ConnectionState.Closed Then
                SqlConnMagaz1.Close()
            End If
            'giu300423 ok terminato
            If App.AggiornaAbilitazione(CSTABILAZI, "SWRicGiac", "ENDRicGiac", StrErrore) = False Then
                StrErrore = StrErrore
                Return False
            End If
            '-----------------------
            Return True
        Catch ex As Exception
            'GIU111120 TransTmp.Rollback()
            StrErrore = strFase & " - " & ex.Message
            Return False
        End Try

    End Function

    'giu060418
    Public Shared Function RicalcolaGicenzaDOC(ByVal _IDDoc As Long, ByRef StrErrore As String, ByRef strConferma As String) As Boolean
        RicalcolaGicenzaDOC = True
        StrErrore = "" : strConferma = ""
        Dim strSQL As String = ""
        strSQL = "SELECT ISNULL(Cod_Articolo,'') AS Cod_Articolo, Riga FROM DocumentiD"
        strSQL += " WHERE (IDDocumenti = " & _IDDoc.ToString.Trim & ") "
        strSQL += " AND (Qta_Ordinata<>0)"

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Dim rsDett As DataRow
        Dim myCodArt As String
        Dim SWNegativi As Boolean = False
        Try
            ds.Clear()
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each rsDett In ds.Tables(0).Select("")
                        myCodArt = IIf(IsDBNull(rsDett!Cod_Articolo), "", rsDett!Cod_Articolo)
                        If myCodArt.Trim <> "" Then
                            If Ricalcola_Giacenze(myCodArt, StrErrore, SWNegativi, True) = False Then
                                Return False
                            End If
                        End If
                    Next
                Else
                    StrErrore = "Attenzione, nessun articolo presente. (RicalcolaGicenzaDOC)"
                    Return False
                End If
                'Controllo che ci sia almeno un articolo con impegnato altrimenti lo segnalo
                strSQL = "SELECT ISNULL(Cod_Articolo,'') AS Cod_Articolo, Riga FROM DocumentiD"
                strSQL += " WHERE (IDDocumenti = " & _IDDoc.ToString.Trim & ") "
                strSQL += " AND (Qta_Impegnata<>0)"
                ds.Clear()
                ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
                ObjDB = Nothing
                If (ds.Tables.Count > 0) Then
                    If (ds.Tables(0).Rows.Count > 0) Then
                        'ok C'E' ALMENO UNA VOCE
                    Else
                        strConferma = "Attenzione, nessun articolo presente. (RicalcolaGicenzaDOC)"
                    End If
                Else
                    strConferma = "Attenzione, nessun articolo presente. (RicalcolaGicenzaDOC)"
                End If
            Else
                StrErrore = "Attenzione, nessun articolo presente. (RicalcolaGicenzaDOC)"
                Return False
            End If
        Catch Ex As Exception
            StrErrore = "(RicalcolaGicenzaDOC) Si è verificato il seguente errore:" & Ex.Message
            Return False
        End Try
    End Function

    Public Shared Function InfoPerRicalcoloGiacenza(ByRef TotArticoli As Integer, ByRef TotMovimenti As Integer, ByRef DataPrimoMovimento As Date, ByRef DataUltimoMovimento As Date, ByRef strErrore As String, ByRef TotOrdCli As Integer, ByRef DataPrimoOrdCli As Date, ByRef DataUltimoOrdCli As Date, ByRef TotOrdFor As Integer, ByRef DataPrimoOrdFor As Date, ByRef DataUltimoOrdFor As Date)
        Try
            Dim dbCon As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
            Dim SqlConnInfo = New System.Data.SqlClient.SqlConnection
            Dim SqlSel_Info = New System.Data.SqlClient.SqlCommand

            SqlSel_Info.CommandText = "[get_infoRicalcoloGiacenza]"
            SqlSel_Info.CommandType = System.Data.CommandType.StoredProcedure
            SqlConnInfo.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            SqlSel_Info.Connection = SqlConnInfo
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotArticoli", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotMovimenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataPrimoMovimento", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataUltimoMovimento", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotOrdiniCli", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataPrimoOrdCli", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataUltimoOrdCli", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@TotOrdiniFor", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataPrimoOrdFor", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
            SqlSel_Info.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataUltimoOrdFor", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Output, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))

            If SqlConnInfo.State = ConnectionState.Closed Then
                SqlConnInfo.Open()
            End If

            SqlSel_Info.ExecuteNonQuery()

            TotArticoli = SqlSel_Info.Parameters("@TotArticoli").Value

            '===MOVIMENTI DI MAGAZZINO
            TotMovimenti = SqlSel_Info.Parameters("@TotMovimenti").Value
            If Not IsDBNull(SqlSel_Info.Parameters("@DataPrimoMovimento").Value) Then
                DataPrimoMovimento = SqlSel_Info.Parameters("@DataPrimoMovimento").Value
            Else
                DataPrimoMovimento = CDate("01/01/1900")
            End If
            If Not IsDBNull(SqlSel_Info.Parameters("@DataUltimoMovimento").Value) Then
                DataUltimoMovimento = SqlSel_Info.Parameters("@DataUltimoMovimento").Value
            Else
                DataUltimoMovimento = CDate("01/01/1900")
            End If

            '===ORDINI CLIENTI
            TotOrdCli = SqlSel_Info.Parameters("@TotOrdiniCli").Value
            If Not IsDBNull(SqlSel_Info.Parameters("@DataPrimoOrdCli").Value) Then
                DataPrimoOrdCli = SqlSel_Info.Parameters("@DataPrimoOrdCli").Value
            Else
                DataPrimoOrdCli = CDate("01/01/1900")
            End If
            If Not IsDBNull(SqlSel_Info.Parameters("@DataUltimoOrdCli").Value) Then
                DataUltimoOrdCli = SqlSel_Info.Parameters("@DataUltimoOrdCli").Value
            Else
                DataUltimoOrdCli = CDate("01/01/1900")
            End If

            '===ORDINI FORNITORI
            TotOrdFor = SqlSel_Info.Parameters("@TotOrdiniFor").Value
            If Not IsDBNull(SqlSel_Info.Parameters("@DataPrimoOrdFor").Value) Then
                DataPrimoOrdFor = SqlSel_Info.Parameters("@DataPrimoOrdFor").Value
            Else
                DataPrimoOrdFor = CDate("01/01/1900")
            End If
            If Not IsDBNull(SqlSel_Info.Parameters("@DataUltimoOrdFor").Value) Then
                DataUltimoOrdFor = SqlSel_Info.Parameters("@DataUltimoOrdFor").Value
            Else
                DataUltimoOrdFor = CDate("01/01/1900")
            End If

            SqlConnInfo.Close()

            Return True
        Catch ex As Exception
            strErrore = ex.Message
            Return False
        End Try
    End Function

#End Region

#Region "DISTINTA BASE SETTEMBRE 2014"
    'GIU070914
    Public Shared Function GetDatiGiacenzaDB(ByVal CodArt As String) As Boolean
        ' ''lblGiacenza.Text = "" : lblGiacenza.ForeColor = Drawing.Color.Black
        ' ''lblGiacImp.Text = ""
        ' ''lblOrdFor.Text = ""
        ' ''lblDataArr.Text = ""
        ' ''If CodArt.Trim = "" Then Exit Function

        ' ''Dim strSQL As String = "" : Dim Comodo = ""
        ' ''Dim ObjDB As New DataBaseUtility
        ' ''Dim dsArt As New DataSet
        ' ''Dim rowArt() As DataRow
        ' ''Dim myQtaGiac As Decimal = 0 : Dim myQtaImp As Decimal = 0 : Dim myQtaOF As Decimal = 0
        ' ''Dim myQtaOFArr As Decimal = 0 : Dim myDataArr = ""
        ' ''Dim I As Integer
        ' ''strSQL = "SELECT DistBase.CodPadre, DistBase.CodFiglio, DistBase.Quantita, AnaMag.Giacenza, AnaMag.Giac_Impegnata, " & _
        ' ''             "AnaMag.Ord_Fornit, AnaMag.QtaArrivoFornit, AnaMag.Data_Arrivo " & _
        ' ''             "FROM DistBase INNER JOIN AnaMag ON DistBase.CodFiglio = AnaMag.Cod_Articolo " & _
        ' ''             "WHERE (DistBase.CodPadre = '" & CodArt.Trim & "')"
        ' ''Try
        ' ''    ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, dsArt)
        ' ''    If (dsArt.Tables.Count > 0) Then
        ' ''        If (dsArt.Tables(0).Rows.Count > 0) Then
        ' ''            rowArt = dsArt.Tables(0).Select()
        ' ''            For I = 0 To dsArt.Tables(0).Rows.Count - 1
        ' ''                myQtaGiac += IIf(IsDBNull(rowArt(0).Item("Giacenza")), 0, rowArt(0).Item("Giacenza"))
        ' ''            Next
        ' ''            lblGiacenza.Text = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Giacenza")), 0, rowArt(0).Item("Giacenza")), -1)
        ' ''            If lblGiacenza.Text.Trim = "0" Then lblGiacenza.Text = ""
        ' ''            If IsNumeric(lblGiacenza.Text.Trim) Then
        ' ''                If CDbl(lblGiacenza.Text.Trim) < 0 Then
        ' ''                    lblGiacenza.ForeColor = SEGNALA_KO
        ' ''                Else
        ' ''                    lblGiacenza.ForeColor = Drawing.Color.Black
        ' ''                End If
        ' ''            End If
        ' ''            lblGiacImp.Text = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Giac_Impegnata")), 0, rowArt(0).Item("Giac_Impegnata")),-1)
        ' ''            If lblGiacImp.Text.Trim = "0" Then lblGiacImp.Text = ""
        ' ''            lblOrdFor.Text = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("Ord_Fornit")), 0, rowArt(0).Item("Ord_Fornit")),-1)
        ' ''            If lblOrdFor.Text.Trim = "0" Then lblOrdFor.Text = ""
        ' ''            lblDataArr.Text = ""
        ' ''            Comodo = IIf(IsDBNull(rowArt(0).Item("Data_Arrivo")), "", rowArt(0).Item("Data_Arrivo"))
        ' ''            If IsDate(Comodo.Trim) Then
        ' ''                If CDate(Comodo.Trim) = DATANULL Then
        ' ''                    Comodo = ""
        ' ''                End If
        ' ''            Else
        ' ''                Comodo = ""
        ' ''            End If
        ' ''            If Comodo.Trim <> "" Then
        ' ''                lblDataArr.Text = Format(CDate(Comodo), FormatoData) & " "
        ' ''                Comodo = FormattaNumero(IIf(IsDBNull(rowArt(0).Item("QtaArrivoFornit")), 0, rowArt(0).Item("QtaArrivoFornit")),-1)
        ' ''                If CLng(Comodo.Trim) > 0 Then
        ' ''                    lblDataArr.Text += "(" & Comodo.Trim & ")"
        ' ''                End If
        ' ''            End If
        ' ''        Else
        ' ''            'ARTICOLO NON IN DISTINTA BASE
        ' ''            Exit Function
        ' ''        End If
        ' ''    Else
        ' ''        'ARTICOLO NON IN DISTINTA BASE
        ' ''        Exit Function
        ' ''    End If
        ' ''Catch Ex As Exception
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore in DocumentiDett.GetDatiGiacenzaDB", "Lettura articoli DISTINTA BASE: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
        ' ''    Exit Function
        ' ''End Try
    End Function
#End Region

#Region "GESTIONE DISPONIBILITA E VALORIZZAZIONE MAGAZZINO"
    Public Function StampaDispMagazzino(ByVal AzReport As String, ByVal TitoloReport As String, ByVal Filtri As String, ByVal SQLstrWhere As String, ByRef dsDispMagazzino1 As DsMagazzino, ByRef Errore As String, ByVal IncludiSottoScorta As Boolean, ByVal SoloDaOrdinare As Boolean, ByRef SWNegativi As Boolean, ByVal SoloNegativi As Boolean, ByVal SWValorizzazione As Boolean, ByVal CodMag As Integer, Optional ByVal Connessione As SqlConnection = Nothing, Optional ByVal SoloGiacDivZero As Boolean = False) As Boolean 'giu200523 richiesta erika del 19/05/2023
        Dim clsDB As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection
        'giu190613 non va inizializzato anche xkè prima c'è il ricalcolo che lo inizializza SWNegativi = False
        Try
            'giu150617
            Dim strValore As String = ""
            Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            '
            If Connessione Is Nothing Then
                SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
            Else 'alberto 08/01/2013
                SQLConn = Connessione
            End If
            'giu040920
            SQLCmd.CommandText = "SELECT * FROM funDispMagazzino(" + CodMag.ToString.Trim + ") AS Expr1 " & SQLstrWhere
            'giu040920 end
            SQLCmd.CommandType = CommandType.Text
            SQLCmd.Connection = SQLConn
            '-
            SQLCmd.CommandTimeout = myTimeOUT '5000
            SQLAdp.SelectCommand = SQLCmd
            SQLAdp.SelectCommand.CommandTimeout = myTimeOUT
            SQLAdp.Fill(dsDispMagazzino1.DispMagazzino)
            If Not IsNothing(SQLCmd.Connection) Then 'giu240120
                If SQLCmd.Connection.State <> ConnectionState.Closed Then
                    SQLCmd.Connection.Close()
                    SQLCmd.Connection = Nothing
                End If
            End If
            Dim TmpResiduo As Decimal
            Dim TmpSottoScorta As Decimal
            For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                row.BeginEdit()
                row.Azienda = AzReport
                row.TitoloRpt = TitoloReport
                row.Filtri = Filtri
                row.VisuaSottoScorta = IncludiSottoScorta
                TmpSottoScorta = row.SottoScorta
                If IncludiSottoScorta = False Then
                    TmpSottoScorta = 0
                End If
                'giu020714 se null va in errore
                If row.IsOrdinatiNull Then row.Ordinati = 0
                If row.IsGiac_ImpegnataNull Then row.Giac_Impegnata = 0
                If row.IsGiacenzaNull Then row.Giacenza = 0
                If row.IsOrd_ClientiNull Then row.Ord_Clienti = 0
                If row.IsDaOrdinareNull Then row.DaOrdinare = 0 'giu230219
                If row.IsTipoArticoloNull Then row.TipoArticolo = 0 'giu230219
                '------------------------------
                'giu280512 Richiesta modifica di Zibordi: l'impegnato è da considerare come evaso per 
                'il riordino è giusto cosi verificato con Pierangelo
                '------------- ORD.FORN.---- GIAC.IMP. DA OC ---- GIAC.DISP.----- OC=ORDINI IN CORSO
                If row.TipoArticolo <> 9 Then 'giu230219
                    TmpResiduo = (row.Ordinati + row.Giac_Impegnata + row.Giacenza) - row.Ord_Clienti
                    If TmpResiduo < TmpSottoScorta Then
                        row.DaOrdinare = TmpSottoScorta - TmpResiduo
                    End If
                End If
                'giu280212
                row.GiacenzaDaMMVal = 0
                row.Prezzo_NettoDaMMVal = 0
                row.Importo_DaMMVal = 0
                '----------
                row.EndEdit()
            Next
            '===========================================================================================
            'giu290512 le successive righe sono riservate solo per la DISPONIBILIT'A E RIORDINO
            'XKè i dati sono relativi ad oggi e non a date diverserse es. se chiedola valorizzazione
            'di magazzino al 31 marzo 2012 mentre oggi siamo a maggio non va bene eseguire le succ.righe
            '===========================================================================================
            'giu220512 giu060124 tolto: Codice_Magazzino=" + CodMag.ToString.Trim + " AND 
            If dsDispMagazzino1.DispMagazzino.Select("Giacenza<0").Length > 0 Then
                SWNegativi = True
            End If
            If SWValorizzazione = False Then 'giu290512
                If SoloDaOrdinare Then
                    For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                        If row.DaOrdinare = 0 Then
                            row.Delete()
                        End If
                    Next
                End If
                '--
                If SoloNegativi And SoloGiacDivZero = False Then
                    For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Select("(Giacenza+Giac_Impegnata)>0 OR (Giacenza+Giac_Impegnata)=0")
                        row.Delete()
                    Next
                ElseIf SoloGiacDivZero = True Then
                    For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Select("(Giacenza+Giac_Impegnata)<>0")
                        row.Delete()
                    Next
                End If
                '---------
            Else
                If SoloNegativi And SoloGiacDivZero = False Then
                    For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Select("(Giacenza+Giac_Impegnata)>0 OR (Giacenza+Giac_Impegnata)=0")
                        row.Delete()
                    Next
                ElseIf SoloGiacDivZero = True Then
                    For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Select("(Giacenza+Giac_Impegnata)=0")
                        row.Delete()
                    Next
                End If
                '---------
            End If
            dsDispMagazzino1.AcceptChanges()
            Return True
        Catch ex As Exception
            Errore = ex.Message & " - Stampa disponibilità di magazzino"
            Return False
            Exit Function
        End Try
    End Function
    'GIU080312 FARE ATTENZIONE VIENE USATA ANCHE PER LA STAMPA COSTO DEL VENDUTO 
    'IL DISCRIMINANTE E' IL CAMPO SoloCostoVenduto 
    'ACCORPA VENDUTO attenzione al prezzo non va eseguito se si calcola il COSTO DEL VENDUTO
    'giu190920 GESTIONE MAGAZZINI CMag default =0  ByVal CMag As Integer, gestisco nella where di chiamata la scelta del magazzino
    Public Function ValMagFIFO_CostoVendutoFIFO(ByVal AzReport As String, ByVal TitoloReport As String, ByVal Filtri As String, ByVal SQLstrWhere As String, ByRef dsDispMagazzino1 As DsMagazzino, ByRef Errore As String, ByVal SoloArtGiacNeg As Boolean, ByVal AccorpaVenduto As Boolean, ByVal SoloCostoVenduto As Boolean, ByRef SWNegativi As Boolean, Optional ByVal Connessione As SqlConnection = Nothing) As Boolean
        Dim clsDB As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection
        'giu190613 non inizializzo piu xkè c'e' il ricalcolo giacenze prima SWNegativi = False
        Try
            If Connessione Is Nothing Then
                SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
            Else
                SQLConn = Connessione
            End If
            If SoloCostoVenduto = False Then
                SQLCmd.CommandText = "SELECT view_MovMagValorizzazione.Cod_Articolo, view_MovMagValorizzazione.DesArtDocD, view_MovMagValorizzazione.Data_Doc," _
                & " view_MovMagValorizzazione.Qta_Evasa, view_MovMagValorizzazione.Prezzo, view_MovMagValorizzazione.Prezzo_Netto," _
                & " view_MovMagValorizzazione.Importo, view_MovMagValorizzazione.Segno_Giacenza, view_MovMagValorizzazione.DesCausale," _
                & " view_MovMagValorizzazione.QtaCS, 0 AS Costo_Totale, view_MovMagValorizzazione.Fatturabile, view_MovMagValorizzazione.CausCostoVenduto," _
                & " 0 AS NonCodificati, view_MovMagValorizzazione.CodCausale," _
                & " view_MovMagValorizzazione.CVisione, view_MovMagValorizzazione.CDeposito, view_MovMagValorizzazione.Tipo_Doc," _
                & " view_MovMagValorizzazione.Reso, view_MovMagValorizzazione.PrezzoCosto," _
                & " view_MovMagValorizzazione.QtaCS AS QtaCS_CDCV, FatturaAC, ScGiacenza" _
                & " FROM view_MovMagValorizzazione " & SQLstrWhere 
            Else
                'REPLACE NON DOVREBBE PIU TROVARLO SISTEMATO NEL FORM CHIAMANTE
                SQLstrWhere.Replace("view_MovMagValorizzazione.", "view_MovMagValorizzazioneCS.")
                SQLCmd.CommandText = "SELECT view_MovMagValorizzazioneCS.Cod_Articolo, view_MovMagValorizzazioneCS.DesArtDocD, view_MovMagValorizzazioneCS.Data_Doc," _
                & " view_MovMagValorizzazioneCS.Qta_Evasa, view_MovMagValorizzazioneCS.Prezzo, view_MovMagValorizzazioneCS.Prezzo_Netto," _
                & " view_MovMagValorizzazioneCS.Importo, view_MovMagValorizzazioneCS.Segno_Giacenza, view_MovMagValorizzazioneCS.DesCausale," _
                & " view_MovMagValorizzazioneCS.QtaCS, 0 AS Costo_Totale, view_MovMagValorizzazioneCS.Fatturabile, view_MovMagValorizzazioneCS.CausCostoVenduto," _
                & " CASE WHEN AnaMag.Cod_Articolo IS NULL THEN 1 ELSE 0 END AS NonCodificati, view_MovMagValorizzazioneCS.CodCausale," _
                & " view_MovMagValorizzazioneCS.CVisione, view_MovMagValorizzazioneCS.CDeposito, view_MovMagValorizzazioneCS.Tipo_Doc," _
                & " view_MovMagValorizzazioneCS.Reso, view_MovMagValorizzazioneCS.PrezzoCosto," _
                & " view_MovMagValorizzazioneCS.QtaCS AS QtaCS_CDCV, FatturaAC, ScGiacenza" _
                & " FROM view_MovMagValorizzazioneCS LEFT OUTER JOIN" _
                & " AnaMag ON view_MovMagValorizzazioneCS.Cod_Articolo = AnaMag.Cod_Articolo " & SQLstrWhere 
            End If

            SQLCmd.CommandType = CommandType.Text
            SQLCmd.Connection = SQLConn
            'giu150617
            Dim strValore As String = ""
            Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
            '---------------------------
            SQLCmd.CommandTimeout = myTimeOUT '5000
            SQLAdp.SelectCommand = SQLCmd
            SQLAdp.SelectCommand.CommandTimeout = myTimeOUT
            dsDispMagazzino1.MovMagValorizzazione.Clear() 'giu210523
            SQLAdp.Fill(dsDispMagazzino1.MovMagValorizzazione)
            If Not IsNothing(SQLCmd.Connection) Then 'giu240120
                If SQLCmd.Connection.State <> ConnectionState.Closed Then
                    SQLCmd.Connection.Close()
                    SQLCmd.Connection = Nothing
                End If
            End If
            'GIU060312 DEVO AGGIUNGERE LE VOCI VENDUTE MA NON CODIFICATE IN ARCHIVIO
            For Each rowNonCod As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("NonCodificati<>0")
                If dsDispMagazzino1.DispMagazzino.Select("Cod_Articolo='" & rowNonCod.Cod_Articolo.Trim & "'").Length = 0 Then
                    Dim newRowArt As DsMagazzino.DispMagazzinoRow = dsDispMagazzino1.DispMagazzino.NewDispMagazzinoRow
                    With newRowArt
                        .BeginEdit()
                        .Azienda = AzReport 'GIU180412
                        .TitoloRpt = TitoloReport 'GIU180412
                        .Filtri = Filtri 'GIU180412
                        .Cod_Articolo = rowNonCod.Cod_Articolo
                        .Descrizione = rowNonCod.DesArtDocD 'GIU180412
                        'giu260412
                        .GiacenzaDaMMVal = 0
                        .Prezzo_NettoDaMMVal = 0
                        .Importo_DaMMVal = 0
                        '----------
                        .EndEdit()
                    End With
                    dsDispMagazzino1.DispMagazzino.AddDispMagazzinoRow(newRowArt)
                    newRowArt = Nothing
                End If
            Next
            dsDispMagazzino1.AcceptChanges()
            '---------------------- ok li ho tutti
            'Leggo gli Articoli selezionati
            For Each rowArt As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                rowArt.BeginEdit()
                'Differenza Carichi-Scarichi
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC'", "Data_Doc,CostoUnitario")
                    rowScar.BeginEdit()
                    For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='+' " & _
                            "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                            "AND QtaCS>0 AND Data_Doc <= '" & _
                            rowScar.Data_Doc & "'", "Data_Doc,CostoUnitario")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS >= rowScar.QtaCS Then
                            rowCar.QtaCS -= rowScar.QtaCS
                            rowScar.Costo_Totale += rowScar.QtaCS * rowCar.Prezzo_Netto
                            rowScar.QtaCS = 0
                            Exit For
                        Else
                            rowScar.QtaCS -= rowCar.QtaCS
                            rowScar.Costo_Totale += rowCar.QtaCS * rowCar.Prezzo_Netto
                            rowCar.QtaCS = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowScar.EndEdit()
                Next
                'Se la differenza Carichi-Scarichi rimangono dei scarichi (Vendite) con QtaCS<>0
                'vengono sottratti da tutti i carichi che ci sono in ordine crescente di Data
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND QtaCS>0 ", "Data_Doc,CostoUnitario")
                    rowScar.BeginEdit()
                    For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='+' " & _
                            "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                            "AND QtaCS>0 AND Data_Doc >= '" & _
                            rowScar.Data_Doc & "'", "Data_Doc,CostoUnitario")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS >= rowScar.QtaCS Then
                            rowCar.QtaCS -= rowScar.QtaCS
                            rowScar.Costo_Totale += rowScar.QtaCS * rowCar.Prezzo_Netto
                            rowScar.QtaCS = 0
                            Exit For
                        Else
                            rowScar.QtaCS -= rowCar.QtaCS
                            rowScar.Costo_Totale += rowCar.QtaCS * rowCar.Prezzo_Netto
                            rowCar.QtaCS = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowScar.EndEdit()
                Next
                '-------- C/Visione C/Deposito ---------------------------------------------------
                'giu260412 devo spostare il COSTO (Carico) sul documento C/Visione C/Deposito (FC)
                '-------- C/Visione C/Deposito ---------------------------------------------------
                'Prima ricalcolo il CostoUnitario del Costo_Totale
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND (CDeposito<>0 OR CVisione<>0)", "Data_Doc")
                    If rowCar.QtaCS_CDCV <> 0 And rowCar.Costo_Totale <> 0 Then
                        rowCar.BeginEdit()
                        rowCar.CostoUnitario = rowCar.Costo_Totale / rowCar.QtaCS_CDCV
                        rowCar.EndEdit()
                    End If
                Next
                'C/DEPOSITO
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND CDeposito<>0 ", "Data_Doc")
                    rowCar.BeginEdit()
                    'SELEZIONO LE VENDITE (USCITE DA C/DEPOSITO)
                    For Each rowSCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                            "AND Tipo_Doc='FC' AND CDeposito<>0 " & _
                            "AND QtaCS>0", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS_CDCV >= rowSCar.QtaCS Then
                            rowCar.QtaCS_CDCV -= rowSCar.QtaCS
                            rowSCar.Costo_Totale += rowSCar.QtaCS * rowCar.CostoUnitario
                            rowSCar.QtaCS = 0
                        Else
                            rowSCar.QtaCS -= rowCar.QtaCS_CDCV
                            rowSCar.Costo_Totale += rowCar.QtaCS_CDCV * rowCar.CostoUnitario
                            rowCar.QtaCS_CDCV = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowCar.EndEdit()
                Next
                'C/VISIONE sono i restanti
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND CVisione<>0 ", "Data_Doc")
                    rowCar.BeginEdit()
                    'SELEZIONO LE VENDITE (USCITE DA C/VISIONE)
                    For Each rowSCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                            "AND Tipo_Doc='FC' AND CVisione<>0 " & _
                            "AND QtaCS>0", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS_CDCV >= rowSCar.QtaCS Then
                            rowCar.QtaCS_CDCV -= rowSCar.QtaCS
                            rowSCar.Costo_Totale += rowSCar.QtaCS * rowCar.CostoUnitario
                            rowSCar.QtaCS = 0
                        Else
                            rowSCar.QtaCS -= rowCar.QtaCS_CDCV
                            rowSCar.Costo_Totale += rowCar.QtaCS_CDCV * rowCar.CostoUnitario
                            rowCar.QtaCS_CDCV = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowCar.EndEdit()
                Next
                'END -------- C/Visione C/Deposito ------------------------------------------------
                'GIU010612 PER LE FATTURE SENZA RIFERIMENTO A DDT E NON C/Deposito o C/Visione
                For Each rowFC As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND QtaCS>0 " & _
                        "AND Tipo_Doc='FC' AND NonCodificati=0 " & _
                        "AND (CDeposito=0) AND (CVisione=0)", "Data_Doc")
                    rowFC.BeginEdit()
                    rowFC.QtaCS = 0
                    rowFC.EndEdit()
                Next
                '-----------------------------------------------------------------------------
                'possono rimanere delle vendite senza carico quindi giacenza in negativo
                'giu020312 aggiungo Rks per i negativi
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                    dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                    rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND QtaCS>0 " & _
                    "AND NonCodificati=0", "Data_Doc")
                    rowScar.BeginEdit()
                    Dim newRow As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRow
                        .BeginEdit()
                        .Cod_Articolo = rowScar.Cod_Articolo
                        .Data_Doc = rowScar.Data_Doc
                        .Qta_Evasa = 0
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = 0
                        .Segno_Giacenza = "+"
                        .DesCausale = "!!MOVIMENTO NEGATIVO!!"
                        .QtaCS = rowScar.QtaCS * -1
                        .Costo_Totale = 0
                        .DesArtDocD = rowScar.DesArtDocD 'giu190412
                        rowArt.GiacenzaDaMMVal += .QtaCS
                        .EndEdit()
                        SWNegativi = True
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRow)
                    newRow = Nothing
                    rowScar.QtaCS = 0
                    rowScar.EndEdit()
                Next
                If AccorpaVenduto = True Then
                    'giu020312 accorpo GLI SCARICHI A PARITA DI DATA E CAUSALE
                    'attenzione al prezzo non va eseguito se si calcola il COSTO DEL VENDUTO
                    Dim SWData As DateTime = CDate("01/01/1900") : Dim SWDesCausale As String = ""
                    Dim TotaleQtaEvasa As Decimal = 0 : Dim TotaleImporto As Decimal = 0
                    For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND Qta_Evasa<>0 ", "Data_Doc,DesCausale")
                        If SWDesCausale.Trim = "" Then
                            SWData = rowScar.Data_Doc
                            SWDesCausale = rowScar.DesCausale
                            TotaleImporto = 0 : TotaleQtaEvasa = 0
                        End If
                        If rowScar.Data_Doc.Date <> SWData.Date Or rowScar.DesCausale.Trim <> SWDesCausale.Trim Then
                            Dim newRowAcc1 As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                            With newRowAcc1
                                .BeginEdit()
                                .Cod_Articolo = rowArt.Cod_Articolo
                                .Data_Doc = SWData
                                .Qta_Evasa = TotaleQtaEvasa
                                .Prezzo = 0
                                .Prezzo_Netto = 0
                                .Importo = TotaleImporto
                                .Segno_Giacenza = "-"
                                .DesCausale = SWDesCausale.Trim
                                .QtaCS = 0
                                .DesArtDocD = rowArt.Descrizione 'giu190412
                                .EndEdit()
                            End With
                            dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRowAcc1)
                            '-
                            newRowAcc1 = Nothing
                            SWData = rowScar.Data_Doc
                            SWDesCausale = rowScar.DesCausale
                            TotaleImporto = 0 : TotaleQtaEvasa = 0
                        End If
                        TotaleImporto += rowScar.Importo
                        TotaleQtaEvasa += rowScar.Qta_Evasa
                        rowScar.Delete()
                    Next
                    If SWDesCausale.Trim <> "" Then
                        Dim newRowAcc2 As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                        With newRowAcc2
                            .BeginEdit()
                            .Cod_Articolo = rowArt.Cod_Articolo
                            .Data_Doc = SWData
                            .Qta_Evasa = TotaleQtaEvasa
                            .Prezzo = 0
                            .Prezzo_Netto = 0
                            .Importo = TotaleImporto
                            .Segno_Giacenza = "-"
                            .DesCausale = SWDesCausale.Trim
                            .QtaCS = 0
                            .DesArtDocD = rowArt.Descrizione 'giu190412
                            .EndEdit()
                        End With
                        dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRowAcc2)
                        newRowAcc2 = Nothing
                    End If
                End If
                'ARTICOLI
                rowArt.EndEdit()
            Next
            'GIU041120 CANCELLO ARTICOLI NON IN LISTINO CON GIACENZA ZERO
            Dim TotQtaCS As Decimal = 0
            If SoloArtGiacNeg Then
                For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                    ' ''If row.GiacenzaDaMMVal >= 0 Then
                    ' ''    row.Delete()
                    ' ''End If
                    TotQtaCS = 0
                    For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                        TotQtaCS += rowGiacVal.QtaCS
                    Next
                    If TotQtaCS >= 0 Then
                        row.Delete()
                    End If
                Next
            Else
                For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                    If row.IsIDListVenDNull Then
                        'giu311220
                        If row.TipoArticolo <> 0 Then
                            row.Delete()
                        Else
                            TotQtaCS = 0
                            For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                                TotQtaCS += rowGiacVal.QtaCS
                            Next
                            If TotQtaCS = 0 Then
                                row.Delete()
                            End If
                        End If
                    Else
                        'giu311220
                        If row.TipoArticolo <> 0 Then
                            row.Delete()
                        End If
                    End If
                Next
            End If
            dsDispMagazzino1.AcceptChanges() 'ALTRIMENTI VA IN ERRORE DOPO PER ESCLUDERE QUELLI CANCELLATI
            'GIU210920 AGGIUNGO I PRODOTTI SENZA ALCUNA MOVIMENTAZIONE DI INIZIO ANNO
            For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                If dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo.Trim & "'").Length = 0 Then
                    Dim newRow As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRow
                        .BeginEdit()
                        .Cod_Articolo = row.Cod_Articolo
                        .Data_Doc = CDate("01/01/1900")
                        .Qta_Evasa = 0
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = 0
                        .Segno_Giacenza = "+"
                        .DesCausale = "!!NESSUN MOVIMENTO!!"
                        .QtaCS = 0
                        .Costo_Totale = 0
                        .DesArtDocD = row.Descrizione
                        .EndEdit()
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRow)
                    newRow = Nothing
                End If
            Next
            '------------------------------------------------------------------------
            dsDispMagazzino1.AcceptChanges()
            Return True
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Valorizzazione di magazzino (FIFO)"
            Return False
            Exit Function
        End Try
    End Function

    'giu290512 segnalo solo i primi 5 altrimenti ModalPopup non si riesce a chiudere
    Dim MaxArtNoPrezzo As Integer = 0
    Public Function ValMagUltPrzAcq(ByVal AzReport As String, ByVal TitoloReport As String, ByVal Filtri As String, ByVal SQLstrWhere As String, ByRef dsDispMagazzino1 As DsMagazzino, ByRef Errore As String, ByVal SoloArtGiacNeg As Boolean, ByRef SWNegativi As Boolean) As Boolean
        Dim clsDB As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection
        Dim UltPrzAcquisto As Decimal
        'giu190613 non inizializzo piu xkè c'e' il ricalcolo giacenze prima SWNegativi = False
        Try
            SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
            'GIU250118 NonCodificati SERVE solo per il COSTO DEL VENDUTO
            ' ''SQLCmd.CommandText = "SELECT view_MovMagValorizzazione.Cod_Articolo, view_MovMagValorizzazione.DesArtDocD, view_MovMagValorizzazione.Data_Doc," _
            ' ''& " view_MovMagValorizzazione.Qta_Evasa, view_MovMagValorizzazione.Prezzo, view_MovMagValorizzazione.Prezzo_Netto," _
            ' ''& " view_MovMagValorizzazione.Importo, view_MovMagValorizzazione.Segno_Giacenza, view_MovMagValorizzazione.DesCausale," _
            ' ''& " view_MovMagValorizzazione.QtaCS, 0 AS Costo_Totale, view_MovMagValorizzazione.Fatturabile, view_MovMagValorizzazione.CausCostoVenduto," _
            ' ''& " CASE WHEN AnaMag.Cod_Articolo IS NULL THEN 1 ELSE 0 END AS NonCodificati, view_MovMagValorizzazione.CodCausale," _
            ' ''& " view_MovMagValorizzazione.CVisione, view_MovMagValorizzazione.CDeposito, view_MovMagValorizzazione.Tipo_Doc," _
            ' ''& " view_MovMagValorizzazione.Reso, view_MovMagValorizzazione.PrezzoCosto, CASE WHEN view_MovMagValorizzazione.Tipo_Doc <> 'CM' THEN '01/01/1900' ELSE view_MovMagValorizzazione.Data_Doc END AS Data_Carico" _
            ' ''& " FROM view_MovMagValorizzazione LEFT OUTER JOIN" _
            ' ''& " AnaMag ON view_MovMagValorizzazione.Cod_Articolo = AnaMag.Cod_Articolo " & SQLstrWhere
            SQLCmd.CommandText = "SELECT view_MovMagValorizzazione.Cod_Articolo, view_MovMagValorizzazione.DesArtDocD, view_MovMagValorizzazione.Data_Doc," _
            & " view_MovMagValorizzazione.Qta_Evasa, view_MovMagValorizzazione.Prezzo, view_MovMagValorizzazione.Prezzo_Netto," _
            & " view_MovMagValorizzazione.Importo, view_MovMagValorizzazione.Segno_Giacenza, view_MovMagValorizzazione.DesCausale," _
            & " view_MovMagValorizzazione.QtaCS, 0 AS Costo_Totale, view_MovMagValorizzazione.Fatturabile, view_MovMagValorizzazione.CausCostoVenduto," _
            & " 0 AS NonCodificati, view_MovMagValorizzazione.CodCausale," _
            & " view_MovMagValorizzazione.CVisione, view_MovMagValorizzazione.CDeposito, view_MovMagValorizzazione.Tipo_Doc," _
            & " view_MovMagValorizzazione.Reso, view_MovMagValorizzazione.PrezzoCosto, CASE WHEN view_MovMagValorizzazione.Tipo_Doc <> 'CM' THEN '01/01/1900' ELSE view_MovMagValorizzazione.Data_Doc END AS Data_Carico" _
            & " FROM view_MovMagValorizzazione " & SQLstrWhere
            SQLCmd.CommandType = CommandType.Text
            SQLCmd.Connection = SQLConn
            'giu190617
            Dim strValore As String = ""
            Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
            '---------------------------
            SQLCmd.CommandTimeout = myTimeOUT '5000    QUI
            SQLAdp.SelectCommand = SQLCmd
            SQLAdp.SelectCommand.CommandTimeout = myTimeOUT 'strValore.Trim()
            SQLAdp.Fill(dsDispMagazzino1.MovMagValorizzazione)
            If Not IsNothing(SQLCmd.Connection) Then 'giu240120
                If SQLCmd.Connection.State <> ConnectionState.Closed Then
                    SQLCmd.Connection.Close()
                    SQLCmd.Connection = Nothing
                End If
            End If
            'Leggo gli Articoli selezionati
            For Each rowArt As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                rowArt.BeginEdit()
                'Differenza Carichi-Scarichi
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC'", "Data_Doc")
                    rowScar.BeginEdit()
                    For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='+' " & _
                            "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                            "AND QtaCS>0 AND Data_Doc <= '" & _
                            rowScar.Data_Doc & "'", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS >= rowScar.QtaCS Then
                            rowCar.QtaCS -= rowScar.QtaCS
                            rowScar.Costo_Totale += rowScar.QtaCS * rowCar.Prezzo_Netto
                            rowScar.QtaCS = 0
                            Exit For
                        Else
                            rowScar.QtaCS -= rowCar.QtaCS
                            rowScar.Costo_Totale += rowCar.QtaCS * rowCar.Prezzo_Netto
                            rowCar.QtaCS = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowScar.EndEdit()
                Next
                'Se la differenza Carichi-Scarichi rimangono dei scarichi (Vendite) con QtaCS<>0
                'vengono sottratti da tutti i carichi che ci sono in ordine crescente di Data
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND QtaCS>0 ", "Data_Doc")
                    rowScar.BeginEdit()
                    For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='+' " & _
                            "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                            "AND QtaCS>0 AND Data_Doc >= '" & _
                            rowScar.Data_Doc & "'", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS >= rowScar.QtaCS Then
                            rowCar.QtaCS -= rowScar.QtaCS
                            rowScar.Costo_Totale += rowScar.QtaCS * rowCar.Prezzo_Netto
                            rowScar.QtaCS = 0
                            Exit For
                        Else
                            rowScar.QtaCS -= rowCar.QtaCS
                            rowScar.Costo_Totale += rowCar.QtaCS * rowCar.Prezzo_Netto
                            rowCar.QtaCS = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowScar.EndEdit()
                Next
                '-------- C/Visione C/Deposito ---------------------------------------------------
                'giu260412 devo spostare il COSTO (Carico) sul documento C/Visione C/Deposito (FC)
                '-------- C/Visione C/Deposito ---------------------------------------------------
                'Prima ricalcolo il CostoUnitario del Costo_Totale
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND (CDeposito<>0 OR CVisione<>0)", "Data_Doc")
                    If rowCar.QtaCS_CDCV <> 0 And rowCar.Costo_Totale <> 0 Then
                        rowCar.BeginEdit()
                        rowCar.CostoUnitario = rowCar.Costo_Totale / rowCar.QtaCS_CDCV
                        rowCar.EndEdit()
                    End If
                Next
                'C/DEPOSITO
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND CDeposito<>0 ", "Data_Doc")
                    rowCar.BeginEdit()
                    'SELEZIONO LE VENDITE (USCITE DA C/DEPOSITO)
                    For Each rowSCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                            "AND Tipo_Doc='FC' AND CDeposito<>0 " & _
                            "AND QtaCS>0", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS_CDCV >= rowSCar.QtaCS Then
                            rowCar.QtaCS_CDCV -= rowSCar.QtaCS
                            rowSCar.Costo_Totale += rowSCar.QtaCS * rowCar.CostoUnitario
                            rowSCar.QtaCS = 0
                        Else
                            rowSCar.QtaCS -= rowCar.QtaCS_CDCV
                            rowSCar.Costo_Totale += rowCar.QtaCS_CDCV * rowCar.CostoUnitario
                            rowCar.QtaCS_CDCV = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowCar.EndEdit()
                Next
                'C/VISIONE sono i restanti
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND CVisione<>0 ", "Data_Doc")
                    rowCar.BeginEdit()
                    'SELEZIONO LE VENDITE (USCITE DA C/DEPOSITO)
                    For Each rowSCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                            "AND Tipo_Doc='FC' AND CVisione<>0 " & _
                            "AND QtaCS>0", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS_CDCV >= rowSCar.QtaCS Then
                            rowCar.QtaCS_CDCV -= rowSCar.QtaCS
                            rowSCar.Costo_Totale += rowSCar.QtaCS * rowCar.CostoUnitario
                            rowSCar.QtaCS = 0
                        Else
                            rowSCar.QtaCS -= rowCar.QtaCS_CDCV
                            rowSCar.Costo_Totale += rowCar.QtaCS_CDCV * rowCar.CostoUnitario
                            rowCar.QtaCS_CDCV = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowCar.EndEdit()
                Next
                'END -------- C/Visione C/Deposito ------------------------------------------------
                'possono rimanere delle vendite senza carico quindi giacenza in negativo
                'GIU010612 PER LE FATTURE SENZA RIFERIMENTO A DDT E NON C/Deposito o C/Visione
                For Each rowFC As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND QtaCS>0 " & _
                        "AND Tipo_Doc='FC' AND NonCodificati=0 " & _
                        "AND (CDeposito=0) AND (CVisione=0)", "Data_Doc")
                    rowFC.BeginEdit()
                    rowFC.QtaCS = 0
                    rowFC.EndEdit()
                Next
                '-----------------------------------------------------------------------------
                'giu020312 aggiungo Rks per i negativi
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                    dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                    rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND QtaCS>0 " & _
                    "AND NonCodificati=0", "Data_Doc")
                    rowScar.BeginEdit()
                    Dim newRow As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRow
                        .BeginEdit()
                        .Cod_Articolo = rowScar.Cod_Articolo
                        .Data_Doc = rowScar.Data_Doc
                        .Qta_Evasa = 0
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = 0
                        .Segno_Giacenza = "+"
                        .DesCausale = "!!MOVIMENTO NEGATIVO!!"
                        .QtaCS = rowScar.QtaCS * -1
                        .Costo_Totale = 0
                        .DesArtDocD = rowScar.DesArtDocD 'giu190412
                        rowArt.GiacenzaDaMMVal += .QtaCS
                        .EndEdit()
                        SWNegativi = True
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRow)
                    newRow = Nothing
                    rowScar.QtaCS = 0
                    rowScar.EndEdit()
                Next
                'giu020312 accorpo GLI SCARICHI A PARITA DI DATA E CAUSALE
                'attenzione al prezzo non va eseguito se si calcola il COSTO DEL VENDUTO
                Dim SWData As DateTime = CDate("01/01/1900") : Dim SWDesCausale As String = ""
                Dim TotaleQtaEvasa As Decimal = 0 : Dim TotaleImporto As Decimal = 0
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND Qta_Evasa<>0 ", "Data_Doc,DesCausale")
                    If SWDesCausale.Trim = "" Then
                        SWData = rowScar.Data_Doc
                        SWDesCausale = rowScar.DesCausale
                        TotaleImporto = 0 : TotaleQtaEvasa = 0
                    End If
                    If rowScar.Data_Doc.Date <> SWData.Date Or rowScar.DesCausale.Trim <> SWDesCausale.Trim Then
                        Dim newRowAcc1 As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                        With newRowAcc1
                            .BeginEdit()
                            .Cod_Articolo = rowArt.Cod_Articolo
                            .Data_Doc = SWData
                            .Qta_Evasa = TotaleQtaEvasa
                            .Prezzo = 0
                            .Prezzo_Netto = 0
                            .Importo = TotaleImporto
                            .Segno_Giacenza = "-"
                            .DesCausale = SWDesCausale.Trim
                            .QtaCS = 0
                            .DesArtDocD = rowArt.Descrizione 'giu190412
                            .EndEdit()
                        End With
                        dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRowAcc1)
                        '-
                        newRowAcc1 = Nothing
                        SWData = rowScar.Data_Doc
                        SWDesCausale = rowScar.DesCausale
                        TotaleImporto = 0 : TotaleQtaEvasa = 0
                    End If
                    TotaleImporto += rowScar.Importo
                    TotaleQtaEvasa += rowScar.Qta_Evasa
                    rowScar.Delete()
                Next
                If SWDesCausale.Trim <> "" Then
                    Dim newRowAcc2 As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRowAcc2
                        .BeginEdit()
                        .Cod_Articolo = rowArt.Cod_Articolo
                        .Data_Doc = SWData
                        .Qta_Evasa = TotaleQtaEvasa
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = TotaleImporto
                        .Segno_Giacenza = "-"
                        .DesCausale = SWDesCausale.Trim
                        .QtaCS = 0
                        .DesArtDocD = rowArt.Descrizione 'giu190412
                        .EndEdit()
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRowAcc2)
                    newRowAcc2 = Nothing
                End If
                'ARTICOLI
                rowArt.EndEdit()
            Next
            'GIU041120 CANCELLO ARTICOLI NON IN LISTINO CON GIACENZA ZERO
            Dim TotQtaCS As Decimal = 0
            If SoloArtGiacNeg Then
                For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                    ' ''If row.GiacenzaDaMMVal >= 0 Then
                    ' ''    row.Delete()
                    ' ''End If
                    TotQtaCS = 0
                    For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                        TotQtaCS += rowGiacVal.QtaCS
                    Next
                    If TotQtaCS >= 0 Then
                        row.Delete()
                    End If
                Next
            Else
                For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                    ' ''If row.IsIDListVenDNull Then
                    ' ''    TotQtaCS = 0
                    ' ''    For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                    ' ''        TotQtaCS += rowGiacVal.QtaCS
                    ' ''    Next
                    ' ''    If TotQtaCS = 0 Then
                    ' ''        row.Delete()
                    ' ''    End If
                    ' ''End If
                    If row.IsIDListVenDNull Then
                        'giu311220
                        If row.TipoArticolo <> 0 Then
                            row.Delete()
                        Else
                            TotQtaCS = 0
                            For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                                TotQtaCS += rowGiacVal.QtaCS
                            Next
                            If TotQtaCS = 0 Then
                                row.Delete()
                            End If
                        End If
                    Else
                        'giu311220
                        If row.TipoArticolo <> 0 Then
                            row.Delete()
                        End If
                    End If
                Next
            End If
            dsDispMagazzino1.AcceptChanges() 'ALTRIMENTI VA IN ERRORE DOPO PER ESCLUDERE QUELLI CANCELLATI
            '-------------------------------------
            Errore = ""
            MaxArtNoPrezzo = 0
            For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                UltPrzAcquisto = get_UltPrzAcquisto(dsDispMagazzino1, row.Cod_Articolo, row.Giacenza + row.Giac_Impegnata, Errore)
                'NON SERVE UltPrzAcquisto = dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo = '" & row.Cod_Articolo & "' AND Tipo_Doc = 'CM'", "Data_Carico DESC")(0)("Prezzo_Netto")
                row.BeginEdit()
                row.Prezzo_NettoDaMMVal = UltPrzAcquisto
                row.EndEdit()
            Next
            'GIU210920 AGGIUNGO I PRODOTTI SENZA ALCUNA MOVIMENTAZIONE DI INIZIO ANNO
            For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                If dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo.Trim & "'").Length = 0 Then
                    Dim newRow As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRow
                        .BeginEdit()
                        .Cod_Articolo = row.Cod_Articolo
                        .Data_Doc = CDate("01/01/1900")
                        .Qta_Evasa = 0
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = 0
                        .Segno_Giacenza = "+"
                        .DesCausale = "!!NESSUN MOVIMENTO!!"
                        .QtaCS = 0
                        .Costo_Totale = 0
                        .DesArtDocD = row.Descrizione
                        .EndEdit()
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRow)
                    newRow = Nothing
                End If
            Next
            '------------------------------------------------------------------------
            dsDispMagazzino1.AcceptChanges()
            Return True
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Valorizzazione di magazzino (Ultimo prezzo di acquisto)"
            Return False
            Exit Function
        End Try
    End Function
    Private Function get_UltPrzAcquisto(ByVal ds As DsMagazzino, ByVal CodArticolo As String, ByVal Giacenza As Decimal, ByRef strErrore As String) As Decimal
        Dim view As DataView

        view = New DataView(ds.MovMagValorizzazione, "Cod_Articolo = '" & CodArticolo & "' AND Tipo_Doc = 'CM' AND Prezzo_Netto<>0", "Data_Carico DESC", DataViewRowState.CurrentRows)

        If view.Count > 0 Then
            get_UltPrzAcquisto = view.Item(0)("Prezzo_Netto")
        Else
            get_UltPrzAcquisto = 0
        End If

        If get_UltPrzAcquisto = 0 And Giacenza > 0 Then
            MaxArtNoPrezzo += 1
            If MaxArtNoPrezzo < 6 Then
                strErrore = strErrore & "<br>- " & CodArticolo
            End If
        End If
    End Function

    Public Function ValMagMediaPond(ByVal AzReport As String, ByVal TitoloReport As String, ByVal Filtri As String, ByVal SQLstrWhere As String, ByRef dsDispMagazzino1 As DsMagazzino, ByRef Errore As String, ByVal SoloArtGiacNeg As Boolean, ByRef SWNegativi As Boolean) As Boolean
        Dim clsDB As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection
        Dim SommaPrezzi As Decimal
        Dim SommaQta As Decimal
        'giu190613 non inizializzo piu xkè c'e' il ricalcolo giacenze prima SWNegativi = False
        Try
            SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
            'GIU250118 NonCodificati SERVE solo per il COSTO DEL VENDUTO
            ' ''SQLCmd.CommandText = "SELECT view_MovMagValorizzazione.Cod_Articolo, view_MovMagValorizzazione.DesArtDocD, view_MovMagValorizzazione.Data_Doc," _
            ' ''& " view_MovMagValorizzazione.Qta_Evasa, view_MovMagValorizzazione.Prezzo, view_MovMagValorizzazione.Prezzo_Netto," _
            ' ''& " view_MovMagValorizzazione.Importo, view_MovMagValorizzazione.Segno_Giacenza, view_MovMagValorizzazione.DesCausale," _
            ' ''& " view_MovMagValorizzazione.QtaCS, 0 AS Costo_Totale, view_MovMagValorizzazione.Fatturabile, view_MovMagValorizzazione.CausCostoVenduto," _
            ' ''& " CASE WHEN AnaMag.Cod_Articolo IS NULL THEN 1 ELSE 0 END AS NonCodificati, view_MovMagValorizzazione.CodCausale," _
            ' ''& " view_MovMagValorizzazione.CVisione, view_MovMagValorizzazione.CDeposito, view_MovMagValorizzazione.Tipo_Doc," _
            ' ''& " view_MovMagValorizzazione.Reso, view_MovMagValorizzazione.PrezzoCosto" _
            ' ''& " FROM view_MovMagValorizzazione LEFT OUTER JOIN" _
            ' ''& " AnaMag ON view_MovMagValorizzazione.Cod_Articolo = AnaMag.Cod_Articolo " & SQLstrWhere
            SQLCmd.CommandText = "SELECT view_MovMagValorizzazione.Cod_Articolo, view_MovMagValorizzazione.DesArtDocD, view_MovMagValorizzazione.Data_Doc," _
            & " view_MovMagValorizzazione.Qta_Evasa, view_MovMagValorizzazione.Prezzo, view_MovMagValorizzazione.Prezzo_Netto," _
            & " view_MovMagValorizzazione.Importo, view_MovMagValorizzazione.Segno_Giacenza, view_MovMagValorizzazione.DesCausale," _
            & " view_MovMagValorizzazione.QtaCS, 0 AS Costo_Totale, view_MovMagValorizzazione.Fatturabile, view_MovMagValorizzazione.CausCostoVenduto," _
            & " 0 AS NonCodificati, view_MovMagValorizzazione.CodCausale," _
            & " view_MovMagValorizzazione.CVisione, view_MovMagValorizzazione.CDeposito, view_MovMagValorizzazione.Tipo_Doc," _
            & " view_MovMagValorizzazione.Reso, view_MovMagValorizzazione.PrezzoCosto" _
            & " FROM view_MovMagValorizzazione " & SQLstrWhere
            SQLCmd.CommandType = CommandType.Text
            SQLCmd.Connection = SQLConn
            'giu190617
            Dim strValore As String = ""
            Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
            '---------------------------
            SQLCmd.CommandTimeout = myTimeOUT '5000
            SQLAdp.SelectCommand = SQLCmd
            SQLAdp.SelectCommand.CommandTimeout = myTimeOUT 'strValore.Trim()
            SQLAdp.Fill(dsDispMagazzino1.MovMagValorizzazione)
            If Not IsNothing(SQLCmd.Connection) Then 'giu240120
                If SQLCmd.Connection.State <> ConnectionState.Closed Then
                    SQLCmd.Connection.Close()
                    SQLCmd.Connection = Nothing
                End If
            End If
            'Leggo gli Articoli selezionati
            For Each rowArt As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                rowArt.BeginEdit()
                'Differenza Carichi-Scarichi
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC'", "Data_Doc")
                    rowScar.BeginEdit()
                    For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='+' " & _
                            "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                            "AND QtaCS>0 AND Data_Doc <= '" & _
                            rowScar.Data_Doc & "'", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS >= rowScar.QtaCS Then
                            rowCar.QtaCS -= rowScar.QtaCS
                            'rowScar.Costo_Totale += rowScar.QtaCS * rowCar.Prezzo_Netto
                            rowScar.QtaCS = 0
                            Exit For
                        Else
                            rowScar.QtaCS -= rowCar.QtaCS
                            'rowScar.Costo_Totale += rowCar.QtaCS * rowCar.Prezzo_Netto
                            rowCar.QtaCS = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowScar.EndEdit()
                Next
                'Se la differenza Carichi-Scarichi rimangono dei scarichi (Vendite) con QtaCS<>0
                'vengono sottratti da tutti i carichi che ci sono in ordine crescente di Data
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND QtaCS>0 ", "Data_Doc")
                    rowScar.BeginEdit()
                    For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='+' " & _
                            "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                            "AND QtaCS>0 AND Data_Doc >= '" & _
                            rowScar.Data_Doc & "'", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS >= rowScar.QtaCS Then
                            rowCar.QtaCS -= rowScar.QtaCS
                            'rowScar.Costo_Totale += rowScar.QtaCS * rowCar.Prezzo_Netto
                            rowScar.QtaCS = 0
                            Exit For
                        Else
                            rowScar.QtaCS -= rowCar.QtaCS
                            'rowScar.Costo_Totale += rowCar.QtaCS * rowCar.Prezzo_Netto
                            rowCar.QtaCS = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowScar.EndEdit()
                Next
                '-------- C/Visione C/Deposito ---------------------------------------------------
                'giu260412 devo spostare il COSTO (Carico) sul documento C/Visione C/Deposito (FC)
                '-------- C/Visione C/Deposito ---------------------------------------------------
                'Prima ricalcolo il CostoUnitario del Costo_Totale
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND (CDeposito<>0 OR CVisione<>0)", "Data_Doc")
                    If rowCar.QtaCS_CDCV <> 0 And rowCar.Costo_Totale <> 0 Then
                        rowCar.BeginEdit()
                        'rowCar.CostoUnitario = rowCar.Costo_Totale / rowCar.QtaCS_CDCV
                        rowCar.EndEdit()
                    End If
                Next
                'C/DEPOSITO
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND CDeposito<>0 ", "Data_Doc")
                    rowCar.BeginEdit()
                    'SELEZIONO LE VENDITE (USCITE DA C/DEPOSITO)
                    For Each rowSCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                            "AND Tipo_Doc='FC' AND CDeposito<>0 " & _
                            "AND QtaCS>0", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS_CDCV >= rowSCar.QtaCS Then
                            rowCar.QtaCS_CDCV -= rowSCar.QtaCS
                            'rowSCar.Costo_Totale += rowSCar.QtaCS * rowCar.CostoUnitario
                            rowSCar.QtaCS = 0
                        Else
                            rowSCar.QtaCS -= rowCar.QtaCS_CDCV
                            'rowSCar.Costo_Totale += rowCar.QtaCS_CDCV * rowCar.CostoUnitario
                            rowCar.QtaCS_CDCV = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowCar.EndEdit()
                Next
                'C/VISIONE sono i restanti
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND CVisione<>0 ", "Data_Doc")
                    rowCar.BeginEdit()
                    'SELEZIONO LE VENDITE (USCITE DA C/DEPOSITO)
                    For Each rowSCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                            "AND Tipo_Doc='FC' AND CVisione<>0 " & _
                            "AND QtaCS>0", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS_CDCV >= rowSCar.QtaCS Then
                            rowCar.QtaCS_CDCV -= rowSCar.QtaCS
                            'rowSCar.Costo_Totale += rowSCar.QtaCS * rowCar.CostoUnitario
                            rowSCar.QtaCS = 0
                        Else
                            rowSCar.QtaCS -= rowCar.QtaCS_CDCV
                            'rowSCar.Costo_Totale += rowCar.QtaCS_CDCV * rowCar.CostoUnitario
                            rowCar.QtaCS_CDCV = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowCar.EndEdit()
                Next
                'END -------- C/Visione C/Deposito ------------------------------------------------
                'GIU010612 PER LE FATTURE SENZA RIFERIMENTO A DDT E NON C/Deposito o C/Visione
                For Each rowFC As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND QtaCS>0 " & _
                        "AND Tipo_Doc='FC' AND NonCodificati=0 " & _
                        "AND (CDeposito=0) AND (CVisione=0)", "Data_Doc")
                    rowFC.BeginEdit()
                    rowFC.QtaCS = 0
                    rowFC.EndEdit()
                Next
                '-----------------------------------------------------------------------------
                'possono rimanere delle vendite senza carico quindi giacenza in negativo
                'giu020312 aggiungo Rks per i negativi
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                    dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                    rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND QtaCS>0 " & _
                    "AND NonCodificati=0", "Data_Doc")
                    rowScar.BeginEdit()
                    Dim newRow As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRow
                        .BeginEdit()
                        .Cod_Articolo = rowScar.Cod_Articolo
                        .Data_Doc = rowScar.Data_Doc
                        .Qta_Evasa = 0
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = 0
                        .Segno_Giacenza = "+"
                        .DesCausale = "!!MOVIMENTO NEGATIVO!!"
                        .QtaCS = rowScar.QtaCS * -1
                        .Costo_Totale = 0
                        .DesArtDocD = rowScar.DesArtDocD 'giu190412
                        rowArt.GiacenzaDaMMVal += .QtaCS
                        .EndEdit()
                        SWNegativi = True
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRow)
                    newRow = Nothing
                    rowScar.QtaCS = 0
                    rowScar.EndEdit()
                Next
                'giu020312 accorpo GLI SCARICHI A PARITA DI DATA E CAUSALE
                'attenzione al prezzo non va eseguito se si calcola il COSTO DEL VENDUTO
                Dim SWData As DateTime = CDate("01/01/1900") : Dim SWDesCausale As String = ""
                Dim TotaleQtaEvasa As Decimal = 0 : Dim TotaleImporto As Decimal = 0
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND Qta_Evasa<>0 ", "Data_Doc,DesCausale")
                    If SWDesCausale.Trim = "" Then
                        SWData = rowScar.Data_Doc
                        SWDesCausale = rowScar.DesCausale
                        TotaleImporto = 0 : TotaleQtaEvasa = 0
                    End If
                    If rowScar.Data_Doc.Date <> SWData.Date Or rowScar.DesCausale.Trim <> SWDesCausale.Trim Then
                        Dim newRowAcc1 As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                        With newRowAcc1
                            .BeginEdit()
                            .Cod_Articolo = rowArt.Cod_Articolo
                            .Data_Doc = SWData
                            .Qta_Evasa = TotaleQtaEvasa
                            .Prezzo = 0
                            .Prezzo_Netto = 0
                            .Importo = TotaleImporto
                            .Segno_Giacenza = "-"
                            .DesCausale = SWDesCausale.Trim
                            .QtaCS = 0
                            .DesArtDocD = rowArt.Descrizione 'giu190412
                            .EndEdit()
                        End With
                        dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRowAcc1)
                        '-
                        newRowAcc1 = Nothing
                        SWData = rowScar.Data_Doc
                        SWDesCausale = rowScar.DesCausale
                        TotaleImporto = 0 : TotaleQtaEvasa = 0
                    End If
                    TotaleImporto += rowScar.Importo
                    TotaleQtaEvasa += rowScar.Qta_Evasa
                    rowScar.Delete()
                Next
                If SWDesCausale.Trim <> "" Then
                    Dim newRowAcc2 As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRowAcc2
                        .BeginEdit()
                        .Cod_Articolo = rowArt.Cod_Articolo
                        .Data_Doc = SWData
                        .Qta_Evasa = TotaleQtaEvasa
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = TotaleImporto
                        .Segno_Giacenza = "-"
                        .DesCausale = SWDesCausale.Trim
                        .QtaCS = 0
                        .DesArtDocD = rowArt.Descrizione 'giu190412
                        .EndEdit()
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRowAcc2)
                    newRowAcc2 = Nothing
                End If
                'ARTICOLI
                rowArt.EndEdit()
            Next

            'GIU041120 CANCELLO ARTICOLI NON IN LISTINO CON GIACENZA ZERO
            Dim TotQtaCS As Decimal = 0
            If SoloArtGiacNeg Then
                For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                    TotQtaCS = 0
                    For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                        TotQtaCS += rowGiacVal.QtaCS
                    Next
                    If TotQtaCS >= 0 Then
                        row.Delete()
                    End If
                Next
            Else
                For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                    ' ''If row.IsIDListVenDNull Then
                    ' ''    TotQtaCS = 0
                    ' ''    For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                    ' ''        TotQtaCS += rowGiacVal.QtaCS
                    ' ''    Next
                    ' ''    If TotQtaCS = 0 Then
                    ' ''        row.Delete()
                    ' ''    End If
                    ' ''End If
                    If row.IsIDListVenDNull Then
                        'giu311220
                        If row.TipoArticolo <> 0 Then
                            row.Delete()
                        Else
                            TotQtaCS = 0
                            For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                                TotQtaCS += rowGiacVal.QtaCS
                            Next
                            If TotQtaCS = 0 Then
                                row.Delete()
                            End If
                        End If
                    Else
                        'giu311220
                        If row.TipoArticolo <> 0 Then
                            row.Delete()
                        End If
                    End If
                Next
            End If
            dsDispMagazzino1.AcceptChanges() 'ALTRIMENTI VA IN ERRORE DOPO PER ESCLUDERE QUELLI CANCELLATI
            '-------------------------------------
            SommaPrezzi = 0
            SommaQta = 0
            For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                'giu080512
                SommaPrezzi = 0
                SommaQta = 0
                '---------
                For Each dett As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo = '" & row.Cod_Articolo & "' AND Tipo_Doc = 'CM'")
                    SommaPrezzi = SommaPrezzi + (dett.Prezzo_Netto * dett.Qta_Evasa)
                    SommaQta = SommaQta + dett.Qta_Evasa
                Next
                'giu220512
                If SommaPrezzi <> 0 And SommaQta <> 0 Then
                    row.Prezzo_NettoDaMMVal = Format(SommaPrezzi / SommaQta, "0.00")
                Else
                    row.Prezzo_NettoDaMMVal = Format(0, "0.00")
                End If
            Next
            'GIU210920 AGGIUNGO I PRODOTTI SENZA ALCUNA MOVIMENTAZIONE DI INIZIO ANNO
            For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                If dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo.Trim & "'").Length = 0 Then
                    Dim newRow As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRow
                        .BeginEdit()
                        .Cod_Articolo = row.Cod_Articolo
                        .Data_Doc = CDate("01/01/1900")
                        .Qta_Evasa = 0
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = 0
                        .Segno_Giacenza = "+"
                        .DesCausale = "!!NESSUN MOVIMENTO!!"
                        .QtaCS = 0
                        .Costo_Totale = 0
                        .DesArtDocD = row.Descrizione
                        .EndEdit()
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRow)
                    newRow = Nothing
                End If
            Next
            '------------------------------------------------------------------------
            dsDispMagazzino1.AcceptChanges()
            Return True
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Valorizzazione di magazzino (Media ponderata)"
            Return False
            Exit Function
        End Try
    End Function

    Public Function ValMagLIFO(ByVal AzReport As String, ByVal TitoloReport As String, ByVal Filtri As String, ByVal SQLstrWhere As String, ByRef dsDispMagazzino1 As DsMagazzino, ByRef Errore As String, ByVal SoloArtGiacNeg As Boolean, ByRef SWNegativi As Boolean) As Boolean
        Dim clsDB As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection

        'giu190613 non inizializzo piu xkè c'e' il ricalcolo giacenze prima SWNegativi = False
        Try
            SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)
            'GIU250118 NonCodificati SERVE solo per il COSTO DEL VENDUTO
            ' ''SQLCmd.CommandText = "SELECT view_MovMagValorizzazione.Cod_Articolo, view_MovMagValorizzazione.DesArtDocD, view_MovMagValorizzazione.Data_Doc," _
            ' ''& " view_MovMagValorizzazione.Qta_Evasa, view_MovMagValorizzazione.Prezzo, view_MovMagValorizzazione.Prezzo_Netto," _
            ' ''& " view_MovMagValorizzazione.Importo, view_MovMagValorizzazione.Segno_Giacenza, view_MovMagValorizzazione.DesCausale," _
            ' ''& " view_MovMagValorizzazione.QtaCS, 0 AS Costo_Totale, view_MovMagValorizzazione.Fatturabile, view_MovMagValorizzazione.CausCostoVenduto," _
            ' ''& " CASE WHEN AnaMag.Cod_Articolo IS NULL THEN 1 ELSE 0 END AS NonCodificati, view_MovMagValorizzazione.CodCausale," _
            ' ''& " view_MovMagValorizzazione.CVisione, view_MovMagValorizzazione.CDeposito, view_MovMagValorizzazione.Tipo_Doc," _
            ' ''& " view_MovMagValorizzazione.Reso, view_MovMagValorizzazione.PrezzoCosto," _
            ' ''& " view_MovMagValorizzazione.QtaCS AS QtaCS_CDCV" _
            ' ''& " FROM view_MovMagValorizzazione LEFT OUTER JOIN" _
            ' ''& " AnaMag ON view_MovMagValorizzazione.Cod_Articolo = AnaMag.Cod_Articolo " & SQLstrWhere
            SQLCmd.CommandText = "SELECT view_MovMagValorizzazione.Cod_Articolo, view_MovMagValorizzazione.DesArtDocD, view_MovMagValorizzazione.Data_Doc," _
            & " view_MovMagValorizzazione.Qta_Evasa, view_MovMagValorizzazione.Prezzo, view_MovMagValorizzazione.Prezzo_Netto," _
            & " view_MovMagValorizzazione.Importo, view_MovMagValorizzazione.Segno_Giacenza, view_MovMagValorizzazione.DesCausale," _
            & " view_MovMagValorizzazione.QtaCS, 0 AS Costo_Totale, view_MovMagValorizzazione.Fatturabile, view_MovMagValorizzazione.CausCostoVenduto," _
            & " 0 AS NonCodificati, view_MovMagValorizzazione.CodCausale," _
            & " view_MovMagValorizzazione.CVisione, view_MovMagValorizzazione.CDeposito, view_MovMagValorizzazione.Tipo_Doc," _
            & " view_MovMagValorizzazione.Reso, view_MovMagValorizzazione.PrezzoCosto," _
            & " view_MovMagValorizzazione.QtaCS AS QtaCS_CDCV" _
            & " FROM view_MovMagValorizzazione " & SQLstrWhere
            SQLCmd.CommandType = CommandType.Text
            SQLCmd.Connection = SQLConn
            'giu190617
            Dim strValore As String = ""
            Dim strErrore As String = ""
            Dim myTimeOUT As Long = 5000
            If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, strErrore) = True Then
                If IsNumeric(strValore.Trim) Then
                    If CLng(strValore.Trim) > myTimeOUT Then
                        myTimeOUT = CLng(strValore.Trim)
                    End If
                End If
            End If
            'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
            '---------------------------
            SQLCmd.CommandTimeout = myTimeOUT '5000
            SQLAdp.SelectCommand = SQLCmd
            SQLAdp.SelectCommand.CommandTimeout = myTimeOUT
            SQLAdp.Fill(dsDispMagazzino1.MovMagValorizzazione)
            If Not IsNothing(SQLCmd.Connection) Then 'giu240120
                If SQLCmd.Connection.State <> ConnectionState.Closed Then
                    SQLCmd.Connection.Close()
                    SQLCmd.Connection = Nothing
                End If
            End If
            'GIU060312 DEVO AGGIUNGERE LE VOCI VENDUTE MA NON CODIFICATE IN ARCHIVIO
            For Each rowNonCod As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("NonCodificati<>0")
                If dsDispMagazzino1.DispMagazzino.Select("Cod_Articolo='" & rowNonCod.Cod_Articolo.Trim & "'").Length = 0 Then
                    Dim newRowArt As DsMagazzino.DispMagazzinoRow = dsDispMagazzino1.DispMagazzino.NewDispMagazzinoRow
                    With newRowArt
                        .BeginEdit()
                        .Azienda = AzReport 'GIU180412
                        .TitoloRpt = TitoloReport 'GIU180412
                        .Filtri = Filtri 'GIU180412
                        .Cod_Articolo = rowNonCod.Cod_Articolo
                        .Descrizione = rowNonCod.DesArtDocD 'GIU180412
                        'giu260412
                        .GiacenzaDaMMVal = 0
                        .Prezzo_NettoDaMMVal = 0
                        .Importo_DaMMVal = 0
                        '----------
                        .EndEdit()
                    End With
                    dsDispMagazzino1.DispMagazzino.AddDispMagazzinoRow(newRowArt)
                    newRowArt = Nothing
                End If
            Next
            dsDispMagazzino1.AcceptChanges()
            '---------------------- ok li ho tutti
            'Leggo gli Articoli selezionati
            For Each rowArt As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                rowArt.BeginEdit()
                'Differenza Carichi-Scarichi
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC'", "Data_Doc")
                    rowScar.BeginEdit()
                    For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='+' " & _
                            "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                            "AND QtaCS>0 AND Data_Doc <= '" & _
                            rowScar.Data_Doc & "'", "Data_Doc DESC")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS >= rowScar.QtaCS Then
                            rowCar.QtaCS -= rowScar.QtaCS
                            rowScar.Costo_Totale += rowScar.QtaCS * rowCar.Prezzo_Netto
                            rowScar.QtaCS = 0
                            Exit For
                        Else
                            rowScar.QtaCS -= rowCar.QtaCS
                            rowScar.Costo_Totale += rowCar.QtaCS * rowCar.Prezzo_Netto
                            rowCar.QtaCS = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowScar.EndEdit()
                Next
                'Se la differenza Carichi-Scarichi rimangono dei scarichi (Vendite) con QtaCS<>0
                'vengono sottratti da tutti i carichi che ci sono in ordine crescente di Data
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND QtaCS>0 ", "Data_Doc")
                    rowScar.BeginEdit()
                    For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='+' " & _
                            "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                            "AND QtaCS>0 AND Data_Doc >= '" & _
                            rowScar.Data_Doc & "'", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS >= rowScar.QtaCS Then
                            rowCar.QtaCS -= rowScar.QtaCS
                            rowScar.Costo_Totale += rowScar.QtaCS * rowCar.Prezzo_Netto
                            rowScar.QtaCS = 0
                            Exit For
                        Else
                            rowScar.QtaCS -= rowCar.QtaCS
                            rowScar.Costo_Totale += rowCar.QtaCS * rowCar.Prezzo_Netto
                            rowCar.QtaCS = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowScar.EndEdit()
                Next
                '-------- C/Visione C/Deposito ---------------------------------------------------
                'giu260412 devo spostare il COSTO (Carico) sul documento C/Visione C/Deposito (FC)
                '-------- C/Visione C/Deposito ---------------------------------------------------
                'Prima ricalcolo il CostoUnitario del Costo_Totale
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND (CDeposito<>0 OR CVisione<>0)", "Data_Doc")
                    If rowCar.QtaCS_CDCV <> 0 And rowCar.Costo_Totale <> 0 Then
                        rowCar.BeginEdit()
                        rowCar.CostoUnitario = rowCar.Costo_Totale / rowCar.QtaCS_CDCV
                        rowCar.EndEdit()
                    End If
                Next
                'C/DEPOSITO
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND CDeposito<>0 ", "Data_Doc")
                    rowCar.BeginEdit()
                    'SELEZIONO LE VENDITE (USCITE DA C/DEPOSITO)
                    For Each rowSCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                            "AND Tipo_Doc='FC' AND CDeposito<>0 " & _
                            "AND QtaCS>0", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS_CDCV >= rowSCar.QtaCS Then
                            rowCar.QtaCS_CDCV -= rowSCar.QtaCS
                            rowSCar.Costo_Totale += rowSCar.QtaCS * rowCar.CostoUnitario
                            rowSCar.QtaCS = 0
                        Else
                            rowSCar.QtaCS -= rowCar.QtaCS_CDCV
                            rowSCar.Costo_Totale += rowCar.QtaCS_CDCV * rowCar.CostoUnitario
                            rowCar.QtaCS_CDCV = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowCar.EndEdit()
                Next
                'C/VISIONE sono i restanti
                For Each rowCar As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                        "AND Tipo_Doc<>'FC' AND Tipo_Doc<>'NC' " & _
                        "AND CVisione<>0 ", "Data_Doc")
                    rowCar.BeginEdit()
                    'SELEZIONO LE VENDITE (USCITE DA C/DEPOSITO)
                    For Each rowSCar As DsMagazzino.MovMagValorizzazioneRow In _
                            dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                            rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' " & _
                            "AND Tipo_Doc='FC' AND CVisione<>0 " & _
                            "AND QtaCS>0", "Data_Doc")
                        rowCar.BeginEdit()
                        If rowCar.QtaCS_CDCV >= rowSCar.QtaCS Then
                            rowCar.QtaCS_CDCV -= rowSCar.QtaCS
                            rowSCar.Costo_Totale += rowSCar.QtaCS * rowCar.CostoUnitario
                            rowSCar.QtaCS = 0
                        Else
                            rowSCar.QtaCS -= rowCar.QtaCS_CDCV
                            rowSCar.Costo_Totale += rowCar.QtaCS_CDCV * rowCar.CostoUnitario
                            rowCar.QtaCS_CDCV = 0
                        End If
                        rowCar.EndEdit()
                    Next
                    rowCar.EndEdit()
                Next
                'END -------- C/Visione C/Deposito ------------------------------------------------
                'possono rimanere delle vendite senza carico quindi giacenza in negativo
                'GIU010612 PER LE FATTURE SENZA RIFERIMENTO A DDT E NON C/Deposito o C/Visione
                For Each rowFC As DsMagazzino.MovMagValorizzazioneRow In _
                        dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                        rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND QtaCS>0 " & _
                        "AND Tipo_Doc='FC' AND NonCodificati=0 " & _
                        "AND (CDeposito=0) AND (CVisione=0)", "Data_Doc")
                    rowFC.BeginEdit()
                    rowFC.QtaCS = 0
                    rowFC.EndEdit()
                Next
                '-----------------------------------------------------------------------------
                'giu020312 aggiungo Rks per i negativi
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In _
                    dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & _
                    rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND QtaCS>0 " & _
                    "AND NonCodificati=0", "Data_Doc")
                    rowScar.BeginEdit()
                    Dim newRow As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRow
                        .BeginEdit()
                        .Cod_Articolo = rowScar.Cod_Articolo
                        .Data_Doc = rowScar.Data_Doc
                        .Qta_Evasa = 0
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = 0
                        .Segno_Giacenza = "+"
                        .DesCausale = "!!MOVIMENTO NEGATIVO!!"
                        .QtaCS = rowScar.QtaCS * -1
                        .Costo_Totale = 0
                        .DesArtDocD = rowScar.DesArtDocD 'giu190412
                        rowArt.GiacenzaDaMMVal += .QtaCS
                        .EndEdit()
                        SWNegativi = True
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRow)
                    newRow = Nothing
                    rowScar.QtaCS = 0
                    rowScar.EndEdit()
                Next
                'giu020312 accorpo GLI SCARICHI A PARITA DI DATA E CAUSALE
                'attenzione al prezzo non va eseguito se si calcola il COSTO DEL VENDUTO
                Dim SWData As DateTime = CDate("01/01/1900") : Dim SWDesCausale As String = ""
                Dim TotaleQtaEvasa As Decimal = 0 : Dim TotaleImporto As Decimal = 0
                For Each rowScar As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & rowArt.Cod_Articolo & "' AND Segno_Giacenza='-' AND Qta_Evasa<>0 ", "Data_Doc,DesCausale")
                    If SWDesCausale.Trim = "" Then
                        SWData = rowScar.Data_Doc
                        SWDesCausale = rowScar.DesCausale
                        TotaleImporto = 0 : TotaleQtaEvasa = 0
                    End If
                    If rowScar.Data_Doc.Date <> SWData.Date Or rowScar.DesCausale.Trim <> SWDesCausale.Trim Then
                        Dim newRowAcc1 As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                        With newRowAcc1
                            .BeginEdit()
                            .Cod_Articolo = rowArt.Cod_Articolo
                            .Data_Doc = SWData
                            .Qta_Evasa = TotaleQtaEvasa
                            .Prezzo = 0
                            .Prezzo_Netto = 0
                            .Importo = TotaleImporto
                            .Segno_Giacenza = "-"
                            .DesCausale = SWDesCausale.Trim
                            .QtaCS = 0
                            .DesArtDocD = rowArt.Descrizione 'giu190412
                            .EndEdit()
                        End With
                        dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRowAcc1)
                        '-
                        newRowAcc1 = Nothing
                        SWData = rowScar.Data_Doc
                        SWDesCausale = rowScar.DesCausale
                        TotaleImporto = 0 : TotaleQtaEvasa = 0
                    End If
                    TotaleImporto += rowScar.Importo
                    TotaleQtaEvasa += rowScar.Qta_Evasa
                    rowScar.Delete()
                Next
                If SWDesCausale.Trim <> "" Then
                    Dim newRowAcc2 As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRowAcc2
                        .BeginEdit()
                        .Cod_Articolo = rowArt.Cod_Articolo
                        .Data_Doc = SWData
                        .Qta_Evasa = TotaleQtaEvasa
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = TotaleImporto
                        .Segno_Giacenza = "-"
                        .DesCausale = SWDesCausale.Trim
                        .QtaCS = 0
                        .DesArtDocD = rowArt.Descrizione 'giu190412
                        .EndEdit()
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRowAcc2)
                    newRowAcc2 = Nothing
                End If
                'ARTICOLI
                rowArt.EndEdit()
            Next

            'GIU041120 CANCELLO ARTICOLI NON IN LISTINO CON GIACENZA ZERO
            Dim TotQtaCS As Decimal = 0
            If SoloArtGiacNeg Then
                For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                    ' ''If row.GiacenzaDaMMVal >= 0 Then
                    ' ''    row.Delete()
                    ' ''End If
                    TotQtaCS = 0
                    For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                        TotQtaCS += rowGiacVal.QtaCS
                    Next
                    If TotQtaCS >= 0 Then
                        row.Delete()
                    End If
                Next
            Else
                For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                    ' ''If row.IsIDListVenDNull Then
                    ' ''    TotQtaCS = 0
                    ' ''    For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                    ' ''        TotQtaCS += rowGiacVal.QtaCS
                    ' ''    Next
                    ' ''    If TotQtaCS = 0 Then
                    ' ''        row.Delete()
                    ' ''    End If
                    ' ''End If
                    If row.IsIDListVenDNull Then
                        'giu311220
                        If row.TipoArticolo <> 0 Then
                            row.Delete()
                        Else
                            TotQtaCS = 0
                            For Each rowGiacVal As DsMagazzino.MovMagValorizzazioneRow In dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo & "'")
                                TotQtaCS += rowGiacVal.QtaCS
                            Next
                            If TotQtaCS = 0 Then
                                row.Delete()
                            End If
                        End If
                    Else
                        'giu311220
                        If row.TipoArticolo <> 0 Then
                            row.Delete()
                        End If
                    End If
                Next
            End If
            dsDispMagazzino1.AcceptChanges() 'ALTRIMENTI VA IN ERRORE DOPO PER ESCLUDERE QUELLI CANCELLATI
            '-------------------------------------
            'GIU210920 AGGIUNGO I PRODOTTI SENZA ALCUNA MOVIMENTAZIONE DI INIZIO ANNO
            For Each row As DsMagazzino.DispMagazzinoRow In dsDispMagazzino1.DispMagazzino.Rows
                If dsDispMagazzino1.MovMagValorizzazione.Select("Cod_Articolo='" & row.Cod_Articolo.Trim & "'").Length = 0 Then
                    Dim newRow As DsMagazzino.MovMagValorizzazioneRow = dsDispMagazzino1.MovMagValorizzazione.NewMovMagValorizzazioneRow
                    With newRow
                        .BeginEdit()
                        .Cod_Articolo = row.Cod_Articolo
                        .Data_Doc = CDate("01/01/1900")
                        .Qta_Evasa = 0
                        .Prezzo = 0
                        .Prezzo_Netto = 0
                        .Importo = 0
                        .Segno_Giacenza = "+"
                        .DesCausale = "!!NESSUN MOVIMENTO!!"
                        .QtaCS = 0
                        .Costo_Totale = 0
                        .DesArtDocD = row.Descrizione
                        .EndEdit()
                    End With
                    dsDispMagazzino1.MovMagValorizzazione.AddMovMagValorizzazioneRow(newRow)
                    newRow = Nothing
                End If
            Next
            '------------------------------------------------------------------------
            dsDispMagazzino1.AcceptChanges()
            Return True
        Catch ex As Exception
            Errore = ex.Message & " - Stampa Valorizzazione di magazzino (LIFO)"
            Return False
            Exit Function
        End Try
    End Function
#End Region

#Region "GESTIONE PROPOSTA RIORDINO"
    Public Function GetPresenzaPropostaRiordino(ByRef StrErrore As String) As Boolean
        'PIER030212 Verifico presenza Proposta di Riordino - Giuseppe li cancella quando conferma l'ordine
        Dim strSQL As String = ""
        strSQL = "SELECT TOP 1 IDDocumenti FROM DocumentiT"
        strSQL = strSQL & " WHERE (Tipo_Doc = '" & SWTD(TD.PropOrdFornitori) & "')"

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Catch Ex As Exception
            StrErrore = "(GetPresenzaPropostaRiordino) Si è verificato il seguente errore:" & Ex.Message
            Return False
        End Try
    End Function
#End Region

#Region "GESTIONE INVENTARIO"
    'giu080612 INVENTARIO
    Public Function GetPresenzaInventario(ByRef StrErrore As String) As Boolean
        StrErrore = ""
        Dim strSQL As String = ""
        strSQL = "SELECT TOP 1 IDDocumenti FROM DocumentiT"
        strSQL = strSQL & " WHERE (Tipo_Doc = '" & SWTD(TD.Inventari) & "') AND StatoDoc=0"

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Catch Ex As Exception
            StrErrore = "(GetPresenzaInventario) Si è verificato il seguente errore:" & Ex.Message
            Return False
        End Try
    End Function
    'giu180714
    Public Function CreateInventario(ByRef dsMagazzino1 As DsMagazzino, _
                                     ByVal NDoc As Long, _
                                     ByRef NRev As Integer, _
                                     ByVal RighePerPaginaIN As Integer, _
                                     ByVal chkUnaPagina As Boolean, _
                                     ByVal Utente As String, _
                                     ByVal CMag As Integer, _
                                     ByRef StrErrore As String) As Boolean
        Dim myID As Long = -1
        StrErrore = ""
        CreateInventario = False
        '-
        Dim SqlDbInserCmdForInsert As New SqlCommand
        Dim SqlAdapDocForInsert As New SqlDataAdapter
        Dim DsDocDettForInsert As New DSDocumenti
        Dim RowDettForIns As DSDocumenti.DocumentiDForInsertRow
        Dim RowDett As DsMagazzino.DispMagazzinoRow
        '-
        'OK CREAZIONE NUOVA SCHEDA INVENTARIO
        Dim dbConn As New dbStringaConnesioneFacade(HttpContext.Current.Session(ESERCIZIO))
        Dim SqlConn As New SqlConnection
        SqlConn.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftAzi)
        Dim SqlDbNewCmd As New SqlCommand
        SqlDbNewCmd.CommandText = "[Insert_DocTCreateNewDocIN]"
        SqlDbNewCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbNewCmd.Connection = SqlConn
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Output, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Tipo_Doc", System.Data.SqlDbType.NVarChar, 2))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Numero", System.Data.SqlDbType.NVarChar, 10))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RevisioneNDoc", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InseritoDa", System.Data.SqlDbType.NVarChar, 50))
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ModificatoDa", System.Data.SqlDbType.NVarChar, 50))
        'GIU210920 GESTIONE MAGAZZINI
        SqlDbNewCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodMag", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        '
        'SqlDbInserCmdForInsert
        '
        SqlDbInserCmdForInsert.CommandText = "insert_DocDByIDDocumenti_ForInsertIN"
        SqlDbInserCmdForInsert.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbInserCmdForInsert.Connection = SqlConn
        SqlDbInserCmdForInsert.Parameters.AddRange(New System.Data.SqlClient.SqlParameter() {New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.[Variant], 0, System.Data.ParameterDirection.ReturnValue, False, CType(0, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@IDDocumenti", System.Data.SqlDbType.Int, 0, "IDDocumenti"), New System.Data.SqlClient.SqlParameter("@Riga", System.Data.SqlDbType.Int, 0, "Riga"), New System.Data.SqlClient.SqlParameter("@Cod_Articolo", System.Data.SqlDbType.NVarChar, 0, "Cod_Articolo"), New System.Data.SqlClient.SqlParameter("@Descrizione", System.Data.SqlDbType.NVarChar, 0, "Descrizione"), New System.Data.SqlClient.SqlParameter("@Um", System.Data.SqlDbType.NVarChar, 0, "Um"), New System.Data.SqlClient.SqlParameter("@Qta_Ordinata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Ordinata", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Cod_Iva", System.Data.SqlDbType.Int, 0, "Cod_Iva"), New System.Data.SqlClient.SqlParameter("@Prezzo", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Importo", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(11, Byte), CType(2, Byte), "Importo", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Qta_Evasa", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Evasa", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Qta_Residua", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Residua", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_1", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_1", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_2", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_2", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ScontoReale", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ScontoReale", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ScontoValore", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ScontoValore", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@ImportoProvvigione", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ImportoProvvigione", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Pro_Agente", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Pro_Agente", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Cod_Agente", System.Data.SqlDbType.Int, 0, "Cod_Agente"), New System.Data.SqlClient.SqlParameter("@Confezione", System.Data.SqlDbType.Int, 0, "Confezione"), New System.Data.SqlClient.SqlParameter("@Prezzo_Netto", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo_Netto", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@SWPNettoModificato", System.Data.SqlDbType.Bit, 0, "SWPNettoModificato"), New System.Data.SqlClient.SqlParameter("@Prezzo_Netto_Inputato", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Prezzo_Netto_Inputato", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_3", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_3", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_4", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_4", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_Pag", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_Pag", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Sconto_Merce", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Sconto_Merce", System.Data.DataRowVersion.Current, Nothing), New System.Data.SqlClient.SqlParameter("@Note", System.Data.SqlDbType.NVarChar, 0, "Note"), New System.Data.SqlClient.SqlParameter("@OmaggioImponibile", System.Data.SqlDbType.Bit, 0, "OmaggioImponibile"), New System.Data.SqlClient.SqlParameter("@OmaggioImposta", System.Data.SqlDbType.Bit, 0, "OmaggioImposta"), New System.Data.SqlClient.SqlParameter("@NumeroPagina", System.Data.SqlDbType.Int, 0, "NumeroPagina"), New System.Data.SqlClient.SqlParameter("@N_Pacchi", System.Data.SqlDbType.Int, 0, "N_Pacchi"), New System.Data.SqlClient.SqlParameter("@Qta_Casse", System.Data.SqlDbType.Int, 0, "Qta_Casse"), New System.Data.SqlClient.SqlParameter("@Flag_Imb", System.Data.SqlDbType.Int, 0, "Flag_Imb"), New System.Data.SqlClient.SqlParameter("@Riga_Trasf", System.Data.SqlDbType.Int, 0, "Riga_Trasf"), New System.Data.SqlClient.SqlParameter("@Riga_Appartenenza", System.Data.SqlDbType.Int, 0, "Riga_Appartenenza"), New System.Data.SqlClient.SqlParameter("@RefInt", System.Data.SqlDbType.Int, 0, "RefInt"), New System.Data.SqlClient.SqlParameter("@RefNumPrev", System.Data.SqlDbType.Int, 0, "RefNumPrev"), New System.Data.SqlClient.SqlParameter("@RefDataPrev", System.Data.SqlDbType.DateTime, 0, "RefDataPrev"), New System.Data.SqlClient.SqlParameter("@RefNumOrd", System.Data.SqlDbType.Int, 0, "RefNumOrd"), New System.Data.SqlClient.SqlParameter("@RefDataOrd", System.Data.SqlDbType.DateTime, 0, "RefDataOrd"), New System.Data.SqlClient.SqlParameter("@RefNumDDT", System.Data.SqlDbType.Int, 0, "RefNumDDT"), New System.Data.SqlClient.SqlParameter("@RefDataDDT", System.Data.SqlDbType.DateTime, 0, "RefDataDDT"), New System.Data.SqlClient.SqlParameter("@RefNumNC", System.Data.SqlDbType.Int, 0, "RefNumNC"), New System.Data.SqlClient.SqlParameter("@RefDataNC", System.Data.SqlDbType.DateTime, 0, "RefDataNC"), _
              New System.Data.SqlClient.SqlParameter("@LBase", System.Data.SqlDbType.Int, 0, "LBase"), _
              New System.Data.SqlClient.SqlParameter("@LOpz", System.Data.SqlDbType.Int, 0, "LOpz"), _
              New System.Data.SqlClient.SqlParameter("@Qta_Impegnata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Impegnata", System.Data.DataRowVersion.Current, Nothing), _
              New System.Data.SqlClient.SqlParameter("@Qta_Prenotata", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Prenotata", System.Data.DataRowVersion.Current, Nothing), _
              New System.Data.SqlClient.SqlParameter("@Qta_Allestita", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "Qta_Allestita", System.Data.DataRowVersion.Current, Nothing), _
              New System.Data.SqlClient.SqlParameter("@PrezzoListino", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "PrezzoListino", System.Data.DataRowVersion.Current, Nothing), _
              New System.Data.SqlClient.SqlParameter("@PrezzoAcquisto", System.Data.SqlDbType.[Decimal], 0, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "PrezzoAcquisto", System.Data.DataRowVersion.Current, Nothing), _
              New System.Data.SqlClient.SqlParameter("@SWModAgenti", System.Data.SqlDbType.Bit, 0, "SWModAgenti")})
        SqlAdapDocForInsert.InsertCommand = SqlDbInserCmdForInsert
        '=========================================
        'giu190617
        Dim strValore As String = ""
        'Dim strErrore As String = ""
        Dim myTimeOUT As Long = 5000
        If App.GetDatiAbilitazioni(CSTABILCOGE, "TimeOUTST", strValore, StrErrore) = True Then
            If IsNumeric(strValore.Trim) Then
                If CLng(strValore.Trim) > myTimeOUT Then
                    myTimeOUT = CLng(strValore.Trim)
                End If
            End If
        End If
        'esempio SqlDbSelectDiffPrezzoListino.CommandTimeout = myTimeOUT
        '---------------------------
        Dim NRiga As Integer = 0 'PER RIGA = 0 CREO LA TESTATA
        For Each RowDett In dsMagazzino1.DispMagazzino
            If RowDett.RowState <> DataRowState.Deleted Then
                If NRiga = 0 Then
                    SqlDbNewCmd.Parameters.Item("@Tipo_Doc").Value = SWTD(TD.Inventari)
                    SqlDbNewCmd.Parameters.Item("@Numero").Value = NDoc
                    NRev = NRev + 1
                    SqlDbNewCmd.Parameters.Item("@RevisioneNDoc").Value = NRev
                    SqlDbNewCmd.Parameters.Item("@InseritoDa").Value = Utente
                    SqlDbNewCmd.Parameters.Item("@ModificatoDa").Value = Utente
                    SqlDbNewCmd.Parameters.Item("@CodMag").Value = CMag
                    Try
                        If SqlConn.State = ConnectionState.Closed Then SqlConn.Open()
                        SqlDbNewCmd.CommandTimeout = myTimeOUT '5000
                        SqlDbNewCmd.ExecuteNonQuery()
                        SqlConn.Close()
                        myID = SqlDbNewCmd.Parameters.Item("@IDDocumenti").Value
                    Catch ExSQL As SqlException
                        StrErrore = "Errore SQL: " & ExSQL.Message
                        Exit Function
                    Catch Ex As Exception
                        StrErrore = "Errore: " & Ex.Message
                        Exit Function
                    End Try
                    NRiga = 1
                End If
                Try
                    '===PREPARO GLI ARTICOLO DA INSERIRE ============================
                    RowDettForIns = DsDocDettForInsert.DocumentiDForInsert.NewRow
                    RowDettForIns.Item("IDDOCUMENTI") = myID
                    RowDettForIns.Item("Riga") = NRiga
                    RowDettForIns.Item("COD_ARTICOLO") = RowDett.Cod_Articolo
                    RowDettForIns.Item("Descrizione") = RowDett.Descrizione
                    RowDettForIns.Item("Cod_Iva") = 0
                    RowDettForIns.Item("Prezzo") = 0
                    RowDettForIns.Item("Prezzo_Netto") = 0
                    RowDettForIns.Item("SWPNettoModificato") = 0
                    RowDettForIns.Item("Prezzo_Netto_Inputato") = 0
                    RowDettForIns.Item("Sconto_1") = 0
                    RowDettForIns.Item("Sconto_2") = 0
                    RowDettForIns.Item("Sconto_3") = 0
                    RowDettForIns.Item("Sconto_4") = 0
                    RowDettForIns.Item("Sconto_Pag") = 0
                    RowDettForIns.Item("ScontoValore") = 0
                    RowDettForIns.Item("Sconto_Merce") = 0
                    RowDettForIns.Item("ScontoReale") = 0
                    RowDettForIns.Item("Um") = "PZ" 'Aggiorno nella Store quando setto i prezzi
                    RowDettForIns.Item("Qta_Ordinata") = RowDett.Giacenza + RowDett.Giac_Impegnata
                    ' ''If RowDettForIns.Item("Qta_Ordinata") < 0 Then
                    ' ''    RowDettForIns.Item("Qta_Ordinata") = 0
                    ' ''End If
                    RowDettForIns.Item("Qta_Evasa") = RowDett.Giacenza + RowDett.Giac_Impegnata
                    If RowDettForIns.Item("Qta_Evasa") < 0 Then
                        RowDettForIns.Item("Qta_Evasa") = 0
                    End If
                    RowDettForIns.Item("Qta_Residua") = 0
                    RowDettForIns.Item("Importo") = 0
                    RowDettForIns.Item("Cod_Agente") = DBNull.Value
                    RowDettForIns.Item("Pro_Agente") = 0
                    RowDettForIns.Item("ImportoProvvigione") = 0
                    RowDettForIns.Item("Note") = ""
                    RowDettForIns.Item("OmaggioImponibile") = 0
                    RowDettForIns.Item("OmaggioImposta") = 0
                    RowDettForIns.Item("NumeroPagina") = 0
                    RowDettForIns.Item("N_Pacchi") = 0
                    RowDettForIns.Item("Qta_Casse") = 0
                    RowDettForIns.Item("Flag_Imb") = 0
                    RowDettForIns.Item("Confezione") = 0
                    RowDettForIns.Item("Riga_Trasf") = DBNull.Value
                    RowDettForIns.Item("Riga_Appartenenza") = DBNull.Value
                    RowDettForIns.Item("RefInt") = DBNull.Value
                    RowDettForIns.Item("RefNumPrev") = DBNull.Value
                    RowDettForIns.Item("RefDataPrev") = DBNull.Value
                    RowDettForIns.Item("RefNumOrd") = DBNull.Value
                    RowDettForIns.Item("RefDataOrd") = DBNull.Value
                    RowDettForIns.Item("RefNumDDT") = DBNull.Value
                    RowDettForIns.Item("RefDataDDT") = DBNull.Value
                    RowDettForIns.Item("RefNumNC") = DBNull.Value
                    RowDettForIns.Item("RefDataNC") = DBNull.Value
                    RowDettForIns.Item("LBase") = 0 'Aggiorno nella Store quando setto i prezzi
                    RowDettForIns.Item("LOpz") = 0 'Aggiorno nella Store quando setto i prezzi
                    RowDettForIns.Item("Qta_Impegnata") = 0
                    RowDettForIns.Item("Qta_Prenotata") = 0
                    RowDettForIns.Item("Qta_Allestita") = 0
                    DsDocDettForInsert.DocumentiDForInsert.AddDocumentiDForInsertRow(RowDettForIns)

                    NRiga = NRiga + 1
                    '-
                    If NRiga > RighePerPaginaIN And Not chkUnaPagina Then
                        NRiga = 0
                    End If
                Catch Ex As Exception
                    StrErrore = "Errore: " & Ex.Message
                    Exit Function
                End Try
            End If
        Next
        'giu020714
        Try
            SqlAdapDocForInsert.Update(DsDocDettForInsert.DocumentiDForInsert)
            If Not IsNothing(SqlDbInserCmdForInsert.Connection) Then 'giu240120
                If SqlDbInserCmdForInsert.Connection.State <> ConnectionState.Closed Then
                    SqlDbInserCmdForInsert.Connection.Close()
                    SqlDbInserCmdForInsert.Connection = Nothing
                End If
            End If
        Catch ExSQL As SqlException
            StrErrore = "Errore SQL: " & ExSQL.Message
            Exit Function
        Catch Ex As Exception
            StrErrore = "Errore: " & Ex.Message
            Exit Function
        End Try
        '===============================================================
        'OK FATTO
        CreateInventario = True
    End Function
    'giu140612
    Public Function DelInventario(ByRef StrErrore As String) As Boolean
        StrErrore = ""
        DelInventario = True
        Dim strSQL As String = ""
        Dim SWOk As Boolean = True
        strSQL = "SELECT IDDocumenti FROM DocumentiT"
        strSQL = strSQL & " WHERE (Tipo_Doc = '" & SWTD(TD.Inventari) & "') AND StatoDoc=0"

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    For Each rsTestata In ds.Tables(0).Select("")
                        strSQL = "EXEC delete_DocTByIDDocumenti " & rsTestata![IDDocumenti].ToString.Trim
                        SWOk = ObjDB.ExecuteQueryUpdate(TipoDB.dbSoftAzi, strSQL)
                        If SWOk = False Then
                            ObjDB = Nothing
                            Return False
                        End If
                    Next
                End If
            End If
            ObjDB = Nothing
        Catch Ex As Exception
            StrErrore = "(DelInventario) Si è verificato il seguente errore:" & Ex.Message
            Return False
        End Try
    End Function
    'giu150612
    Public Shared Function GetPresenzaDiffIN(ByVal _IDDoc As Long, ByVal Segno As String, ByRef StrErrore As String) As Boolean
        StrErrore = ""
        Dim strSQL As String = ""
        strSQL = "SELECT TOP 1 IDDocumenti, Riga FROM DocumentiD"
        strSQL += " WHERE (IDDocumenti = " & _IDDoc.ToString.Trim & ") "
        If Segno = "+" Then
            strSQL += " AND (Qta_Ordinata<Qta_Evasa)"
        ElseIf Segno = "-" Then
            strSQL += " AND (Qta_Ordinata>Qta_Evasa)"
        Else
            Return False
        End If

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Catch Ex As Exception
            StrErrore = "(GetPresenzaDiffIN) Si è verificato il seguente errore:" & Ex.Message
            Return False
        End Try
    End Function
#End Region

End Class
