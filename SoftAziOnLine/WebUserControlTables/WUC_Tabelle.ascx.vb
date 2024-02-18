Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WUC_Tabelle
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblDataOdierna.Text = "Oggi, " & Format(Now, "dddd d MMMM yyyy, HH:mm")
        Session(SWOP) = SWOPNESSUNA
        Session(SWOPDETTDOC) = SWOPNESSUNA
        Session(SWOPDETTDOCL) = SWOPNESSUNA

        ModalPopup.WucElement = Me
        Agenti.WucElement = Me
        BancheIBAN.WucElement = Me
        CapiGruppo.WucElement = Me
        CategorieArt.WucElement = Me
        CategorieCli.WucElement = Me
        CausaliNonEvasione.WucElement = Me
        LineeArt.WucElement = Me
        TipoCodArt.WucElement = Me
        Moduli.WucElement = Me
        Misure.WucElement = Me
        Magazzini.WucElement = Me
        Reparti.WucElement = Me
        Scaffali.WucElement = Me
        TipoFatturazione.WucElement = Me
        Vettori.WucElement = Me
        Zone.WucElement = Me
        TestiEmail.WucElement = Me
        RespArea.WucElement = Me
        RespVisite.WucElement = Me
        LeadSource.WucElement = Me
        Nazioni.WucElement = Me

        If Session(F_ANAGRAGENTI_APERTA) = True Then
            Agenti.Show()
            Exit Sub
        End If
        If Session(F_ANAGRNAZIONI_APERTA) = True Then
            Nazioni.Show()
            Exit Sub
        End If
        If Session(F_ANAGRCAPIGR_APERTA) = True Then
            CapiGruppo.Show()
            Exit Sub
        End If
        If Session(F_BANCHEIBAN_APERTA) = True Then
            BancheIBAN.Show()
            Exit Sub
        End If
        If Session(F_ANAGRCATEGORIEART_APERTA) = True Then
            CategorieArt.Show()
            Exit Sub
        End If
        If Session(F_ANAGRCATEGORIE_APERTA) = True Then
            CategorieCli.Show()
            Exit Sub
        End If
        If Session(F_ANAGRCAUSNONEV_APERTA) = True Then
            CausaliNonEvasione.Show()
            Exit Sub
        End If
        If Session(F_ANAGRLINEEART_APERTA) = True Then
            LineeArt.Show()
            Exit Sub
        End If
        If Session(F_ANAGRTIPOCODART_APERTA) = True Then
            TipoCodArt.Show()
            Exit Sub
        End If
        If Session(F_ANAGRMISURE_APERTA) = True Then
            Misure.Show()
            Exit Sub
        End If
        If Session(F_MAGAZZINI_APERTA) = True Then
            Magazzini.Show()
            Exit Sub
        End If
        If Session(F_ANAGRREP_APERTA) = True Then
            Reparti.Show()
            Exit Sub
        End If
        If Session(F_ANAGRSCAFFALI_APERTA) = True Then
            Scaffali.Show()
            Exit Sub
        End If
        If Session(F_ANAGRZONE_APERTA) = True Then
            Zone.Show()
            Exit Sub
        End If
        If Session(F_ANAGRTIPOFATT_APERTA) = True Then
            TipoFatturazione.Show()
            Exit Sub
        End If
        If Session(CSTSALVADB) = True Then
            Session(CSTSALVADB) = False
            RichiestaSalvaDB()
            Exit Sub
        End If
        'giu091012
        If Session(CSTUTILIY) = CallUtility.InitMySQL Then
            Session(CSTUTILIY) = CallUtility.Nessuna
            RichiestaInitMySQL()
            Exit Sub
        End If
        If Session(F_GESTIONETESTIEMAIL_APERTA) = True Then 'sim040618
            TestiEmail.Show()
            Exit Sub
        End If
        If Session(F_MODULI_APERTA) = True Then 'sim040618
            Moduli.Show()
            Exit Sub
        End If
        If Session(F_ANAGRVETTORI_APERTA) = True Then
            Vettori.Show()
            Exit Sub
        End If
        'giu021119 Gestione Contratti
        If Session(F_ANAGRRESPAREA_APERTA) = True Then
            RespArea.Show()
            Exit Sub
        End If
        If Session(F_ANAGRRESPVISITE_APERTA) = True Then
            RespVisite.Show()
            Exit Sub
        End If
        'GIU281220
        If Session(F_LEAD_APERTA) = True Then
            LeadSource.Show()
            Exit Sub
        End If
    End Sub
    'GIU050412
    Private Sub RichiestaSalvaDB()
        '--------------------------------------------
        Session(MODALPOPUP_CALLBACK_METHOD) = "SalvaDB"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "VisualMenu" 'giu091012 "NoSalvaDB"
        If GetOpConnessi() = False Then
            ModalPopup.Show("Salvataggio DataBase", "Confermi il salvataggio del DataBase ?", WUC_ModalPopup.TYPE_CONFIRM_YN)
        Else
            ModalPopup.Show("Salvataggio DataBase", "Confermi il salvataggio del DataBase ? <br><strong><span> " & _
                            "ATTENZIONE, sono collegati altri utenti!!!</span></strong> <br>" & _
                            "si consiglia di effettuare il salvataggio senza altri utenti collegati.", WUC_ModalPopup.TYPE_CONFIRM_YN)
        End If

    End Sub
    Private Function GetOpConnessi() As Boolean
        GetOpConnessi = False

        Dim strSQL As String = ""
        strSQL = "SELECT * FROM View_OperatoriConnessi "
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 1) Then
                    GetOpConnessi = True
                End If
            End If
        Catch Ex As Exception
            GetOpConnessi = False
            Exit Function
        End Try

    End Function
    Public Sub SalvaDB()

        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Salvataggio DataBase", "Attenzione, Sessione scaduta: Codice ditta/Esercizio non validi.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)

        Dim strSQL As String = ""
        strSQL = "SELECT * FROM Esercizi WHERE Ditta = '" & myCodDitta & "' AND Esercizio = '" & myEsercizio & "'"
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbInstall, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("DataUltimoBK")) Then
                        If ds.Tables(0).Rows(0).Item("DataUltimoBK").date <> Now.Date Then
                            'ok procedi
                        Else
                            Session(MODALPOPUP_CALLBACK_METHOD) = "OKSalvaDB"
                            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                            ModalPopup.Show("Salvataggio DataBase", "Salvataggio del DataBase già eseguito in data odierna. <br><br> Vuole procedere ugualmente ?", WUC_ModalPopup.TYPE_CONFIRM)
                            Exit Sub
                        End If
                    Else
                        'ok procedi
                    End If
                Else
                    'ok procedi
                End If
            Else
                'ok procedi
            End If
        Catch Ex As Exception
            ChiudiErrore("Errore Salavataggio DataBase: " & Ex.Message.Trim)
            Exit Sub
        End Try
        'OK PROCEDO
        OKSalvaDB()

    End Sub
    Public Sub OKSalvaDB()
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            ModalPopup.Show("Salavataggio DataBase", "Attenzione, Sessione scaduta: Codice ditta/Esercizio non validi.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        Dim strErrore As String = ""
        If SessionUtility.UpdDataUltimoBK(myCodDitta, myEsercizio, strErrore) = False Then
            'GIU091012 NO ALTRIMENTI NON SI FERMA QUI VisualizzaMenu()VisualizzaMenu()
            ModalPopup.Show("ERRORE Salavataggio DataBase", "Salvataggio del DataBase non eseguito. <br>" & strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        ElseIf SessionUtility.BKAll(myCodDitta, myEsercizio, strErrore) = False Then
            'GIU091012 NO ALTRIMENTI NON SI FERMA QUI VisualizzaMenu()VisualizzaMenu()
            SessionUtility.UpdDataUltimoBKErr(myCodDitta, myEsercizio, "")
            ModalPopup.Show("ERRORE Salavataggio DataBase", "Salvataggio del DataBase non eseguito. <br>" & strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        Else
            'GIU091012 NO ALTRIMENTI NON SI FERMA QUI VisualizzaMenu() VisualizzaMenu()
            ModalPopup.Show("Salavataggio DataBase", "Salvataggio del DataBase eseguito correttamente.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
    End Sub
    Public Sub VisualMenu()
        VisualizzaMenu()
    End Sub

    Private Sub ChiudiErrore(ByVal strErrore As String)
        Try
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=" & strErrore.Trim)
        Catch ex As Exception
            Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=" & strErrore.Trim)
        End Try
        Exit Sub
    End Sub
    Public Sub CallBackWFPAnagrAgenti()
        Session(F_ANAGRAGENTI_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrAgenti()
        Session(F_ANAGRAGENTI_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Function CallBackWFPBancheIBAN() As Boolean
        Session(F_BANCHEIBAN_APERTA) = False
        VisualizzaMenu()
    End Function
    Public Function CancBackWFPBancheIBAN() As Boolean
        Session(F_BANCHEIBAN_APERTA) = False
        VisualizzaMenu()
    End Function
    '--
    Public Sub CallBackWFPAnagrCapiGruppo()
        Session(F_ANAGRCAPIGR_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrCapiGruppo()
        Session(F_ANAGRCAPIGR_APERTA) = False
        VisualizzaMenu()
    End Sub
    '--
    Public Sub CallBackWFPAnagrCategorieArt()
        Session(F_ANAGRCATEGORIEART_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrCategorieArt()
        Session(F_ANAGRCATEGORIEART_APERTA) = False
        VisualizzaMenu()
    End Sub
    '--
    Public Sub CallBackWFPAnagrCategorie()
        Session(F_ANAGRCATEGORIE_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrCategorie()
        Session(F_ANAGRCATEGORIE_APERTA) = False
        VisualizzaMenu()
    End Sub
    '--
    Public Sub CallBackWFPAnagrLineeArt()
        Session(F_ANAGRLINEEART_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrLineeArt()
        Session(F_ANAGRLINEEART_APERTA) = False
        VisualizzaMenu()
    End Sub
    '--
    Public Sub CallBackWFPTipoCodArt()
        Session(F_ANAGRTIPOCODART_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPTipoCodArt()
        Session(F_ANAGRTIPOCODART_APERTA) = False
        VisualizzaMenu()
    End Sub
    '--
    Public Sub CallBackWFPAnagrMisure()
        Session(F_ANAGRMISURE_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrMisure()
        Session(F_ANAGRMISURE_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CallBackWFPMagazzini()
        Session(F_MAGAZZINI_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CallBackWFPReparti()
        Session(F_ANAGRREP_APERTA) = False
        VisualizzaMenu()
    End Sub
    '-
    Public Sub CancBackWFPMagazzini()
        Session(F_MAGAZZINI_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPReparti()
        Session(F_ANAGRREP_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPRespArea()
        Session(F_ANAGRRESPAREA_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CallBackWFPScaffali()
        Session(F_ANAGRSCAFFALI_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPScaffali()
        Session(F_ANAGRSCAFFALI_APERTA) = False
        VisualizzaMenu()
    End Sub
    '--
    Public Sub CallBackWFPAnagrVettori()
        Session(F_ANAGRVETTORI_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrVettori()
        Session(F_ANAGRVETTORI_APERTA) = False
        VisualizzaMenu()
    End Sub
    '--
    Public Sub CallBackWFPAnagrZone()
        Session(F_ANAGRZONE_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrZone()
        Session(F_ANAGRZONE_APERTA) = False
        VisualizzaMenu()
    End Sub
    '---
    Public Sub CallBackWFPTipoFatt()
        Session(F_ANAGRTIPOFATT_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPTipoFatt()
        Session(F_ANAGRTIPOFATT_APERTA) = False
        VisualizzaMenu()
    End Sub
    '---
    Public Sub CallBackWFPCausNonEvasione()
        Session(F_ANAGRCAUSNONEV_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPCausNonEvasione()
        Session(F_ANAGRCAUSNONEV_APERTA) = False
        VisualizzaMenu()
    End Sub
    'sim040618
    Public Sub CallBackWFPTestiEmail()
        Session(F_GESTIONETESTIEMAIL_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPTestiEmail()
        Session(F_GESTIONETESTIEMAIL_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CallBackWFPModuli()
        Session(F_MODULI_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPModuli()
        Session(F_MODULI_APERTA) = False
        VisualizzaMenu()
    End Sub
    'giu021119 Gestione Contratti
    Public Sub CallBackWFPAnagrRespArea()
        Session(F_ANAGRRESPAREA_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrRespArea()
        Session(F_ANAGRRESPAREA_APERTA) = False
        VisualizzaMenu()
    End Sub
    '-
    Public Sub CallBackWFPAnagrRespVisite()
        Session(F_ANAGRRESPVISITE_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPAnagrRespVisite()
        Session(F_ANAGRRESPVISITE_APERTA) = False
        VisualizzaMenu()
    End Sub
    'GIU281220
    '-
    Public Sub CallBackWFPLeadSource()
        Session(F_LEAD_APERTA) = False
        VisualizzaMenu()
    End Sub
    Public Sub CancBackWFPLeadSource()
        Session(F_LEAD_APERTA) = False
        VisualizzaMenu()
    End Sub
    '@@@@@@@@@@@@@@
    'giu200312 MENU
    '@@@@@@@@@@@@@@
    Private Sub VisualizzaMenu()
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
            Exit Sub
        Catch ex As Exception
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale")
            Exit Sub
        End Try
        Exit Sub
    End Sub

    'GIU091012
    Private Sub RichiestaInitMySQL()
        '--------------------------------------------
        Session(MODALPOPUP_CALLBACK_METHOD) = "OKInitMySQL"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "VisualMenu"
        ModalPopup.Show("Inizilizzazione DataBase MySQL", "Confermi l'inizializzazione del DataBase MySQL ?", WUC_ModalPopup.TYPE_CONFIRM_YN)

    End Sub
    Public Sub OKInitMySQL()
        If String.IsNullOrEmpty(Session(CSTCODDITTA)) Or _
           String.IsNullOrEmpty(Session(ESERCIZIO)) Then
            ModalPopup.Show("Inizializzazione DataBase MySQL", "Attenzione, Sessione scaduta: Codice ditta/Esercizio non validi.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
        Dim myCodDitta As String = Session(CSTCODDITTA)
        Dim myEsercizio As String = Session(ESERCIZIO)
        Dim strErrore As String = ""
        If DataBaseUtility.InitMySQLClienti(strErrore) = False Then
            'GIU091012 NO ALTRIMENTI NON SI FERMA QUI VisualizzaMenu()VisualizzaMenu()
            ModalPopup.Show("ERRORE Inizializzazione DataBase MySQL", "Inizializzazione DataBase MySQL non eseguito. <br>" & strErrore, WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        Else
            'GIU091012 NO ALTRIMENTI NON SI FERMA QUI VisualizzaMenu()
            ModalPopup.Show("Inizializzazione DataBase MySQL", "Inizializzazione DataBase MySQL eseguito correttamente.", WUC_ModalPopup.TYPE_INFO)
            Exit Sub
        End If
    End Sub
End Class