Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WF_Tabelle
    Inherits System.Web.UI.Page
    'Private Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
    '    Try
    '        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    Catch ex As Exception
    '        Response.Redirect("WF_Menu.aspx?labelForm=Errore pagina: Valori potenzialmente pericolosi: <%----%>")
    '    End Try
    'End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTVISUALMENU) = SWSI
        'giu200412 ======= NOTA NOTA ===== SE SI INSERISCE UNA NUOVA TABELLA===
        'METTERE IL Session(F_ANAGRAGENTI_APERTA) = FALSE IN WF_MENU.LOAD.ERROR
        'anche nel masterpage <%----%>
        '======================================================================
        Dim labelForm As String = Request.QueryString("labelForm")
        If IsNothing(labelForm) Then
            labelForm = ""
        End If
        If String.IsNullOrEmpty(labelForm) Then
            labelForm = ""
        End If
        If (Not IsPostBack) Then
            If Mid(UCase(labelForm.Trim), 1, 20) = "SALVATAGGIO DATABASE" Then
                Session(CSTSALVADB) = True

            ElseIf Mid(UCase(labelForm.Trim), 1, 16) = "GESTIONE TABELLE" Then

                If Mid(UCase(labelForm.Trim), 18) = "AGENTI" Then
                    Session(F_ANAGRAGENTI_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "BANCHE AZIENDA" Then
                    Session(F_BANCHEIBAN_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "CAPIGRUPPO" Then
                    Session(F_ANAGRCAPIGR_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "CATEGORIE ARTICOLI" Then
                    Session(F_ANAGRCATEGORIEART_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "CATEGORIE CLIENTI" Then
                    Session(F_ANAGRCATEGORIE_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "CAUSALI DI NON EVASIONE" Then
                    Session(F_ANAGRCAUSNONEV_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "LINEA ARTICOLI" Then
                    Session(F_ANAGRLINEEART_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "LINEA PRODOTTO" Then
                    Session(F_ANAGRTIPOCODART_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "ALLEGATI E-MAIL SCADENZE" Then
                    Session(F_MODULI_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "REPARTI" Then
                    Session(F_ANAGRREP_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "SCAFFALI" Then
                    Session(F_ANAGRSCAFFALI_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "TESTI E-MAIL" Then
                    Session(F_GESTIONETESTIEMAIL_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "TIPO FATTURAZIONE" Then
                    Session(F_ANAGRTIPOFATT_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "UNITA' DI MISURA ARTICOLI" Then
                    Session(F_ANAGRMISURE_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "VETTORI" Then
                    Session(F_ANAGRVETTORI_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "ZONE" Then
                    Session(F_ANAGRZONE_APERTA) = True
                End If
                'giu021119 Gestione Contratti
                If Mid(UCase(labelForm.Trim), 18) = "RESPONSABILI AREA" Then
                    Session(F_ANAGRRESPAREA_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18, 19) = "RESPONSABILI VISITE" Then
                    Session(F_ANAGRRESPVISITE_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "MAGAZZINI" Then 'giu260820
                    Session(F_MAGAZZINI_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "LEAD SOURCE" Then 'giu281220
                    Session(F_LEAD_APERTA) = True
                End If
                If Mid(UCase(labelForm.Trim), 18) = "NAZIONI" Then 'giu230722
                    Session(F_ANAGRNAZIONI_APERTA) = True
                End If
            Else
                'per altre chiamate
                If Mid(UCase(labelForm.Trim), 1, 9) = "UTILITY (" Then
                    Session(CSTUTILIY) = 0
                    Dim myFunzione As String = Mid(UCase(labelForm.Trim), labelForm.IndexOf("(") + 2)
                    myFunzione = Left(myFunzione, myFunzione.IndexOf(")"))
                    If IsNumeric(myFunzione.Trim) Then
                        Session(CSTUTILIY) = myFunzione.Trim
                    End If
                End If
            End If
        End If
    End Sub

End Class