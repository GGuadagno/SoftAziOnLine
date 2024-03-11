Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.Documenti
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms

Partial Public Class WUC_RistampaFatture
    Inherits System.Web.UI.UserControl

    Private TipoDoc As String = "" : Private TabCliFor As String = ""
    Dim strDesTipoDocumento As String = ""
    'giu150312 per il ricalcolo scadenze
    'DATE DI SCADENZE RATE
    Dim lblDataScad1 As String = ""
    Dim lblDataScad2 As String = ""
    Dim lblDataScad3 As String = ""
    Dim lblDataScad4 As String = ""
    Dim lblDataScad5 As String = ""
    'IMPORTO RATE DI SCADENZE RATE
    Dim lblImpRata1 As String = ""
    Dim lblImpRata2 As String = ""
    Dim lblImpRata3 As String = ""
    Dim lblImpRata4 As String = ""
    Dim lblImpRata5 As String = ""
    Dim LblTotaleRate As String = ""

    Private Enum CellIdxT
        TipoDC = 1
        Stato = 2
        NumDoc = 3
        DataDoc = 4
        DataCons = 5
        CodCliForProvv = 6
        RagSoc = 7
        Denom = 8
        Loc = 9
        Pr = 10
        CAP = 11
        PI = 12
        CF = 13
        DataVal = 14
        Riferimento = 15
        DesCausale = 16
        TotaleDoc = 17
        Cod_Valuta = 18
        Cod_Pagamento = 19
        ABI = 20
        CAB = 21
        IDDoc = 22
    End Enum
    Private Enum CellIdxD
        CodArt = 0
        DesArt = 1
        UM = 2
        QtaOr = 3
        QtaEv = 4
        QtaRe = 5
        QtaAl = 6
        IVA = 7
        Prz = 8
        ScV = 9
        Sc1 = 10
        Importo = 11
        ScR = 12
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup.WucElement = Me
        Attesa.WucElement = Me

        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If

        If (sTipoUtente.Equals(CSTMAGAZZINO)) Then
            btnStampaFattura.Visible = False
            btnStampaTutte.Visible = False
        End If
        '----------------------------
        ' ''If (sTipoUtente.Equals(CSTVENDITE)) Then
        ' ''    btnStampaFattura.Visible = False
        ' ''    btnStampaTutte.Visible = False
        ' ''End If
        '----------------------------
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Sub
        End If

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSPrevTElenco.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        SqlDSPrevDByIDDocumenti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        '-----
        If (Not IsPostBack) Then
            Try
                txtDataA.Text = Format(Now, FormatoData)
                '---
                btnStampaFattura.Text = "Stampa singola Fattura"
                btnStampaTutte.Text = "Stampa tutte le Fatture"
                '-----
                Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale)

                ddlRicerca.Items.Add("Numero documento")
                ddlRicerca.Items(0).Value = "N"
                ddlRicerca.Items.Add("Data documento")
                ddlRicerca.Items(1).Value = "D"
                ddlRicerca.Items.Add("Ragione Sociale")
                ddlRicerca.Items(2).Value = "R"
                ddlRicerca.Items.Add("Denominazione")
                ddlRicerca.Items(3).Value = "S"
                ddlRicerca.Items.Add("Partita IVA")
                ddlRicerca.Items(4).Value = "P"
                ddlRicerca.Items.Add("Codice Fiscale")
                ddlRicerca.Items(5).Value = "F"
                ddlRicerca.Items.Add("Località")
                ddlRicerca.Items(6).Value = "L"
                ddlRicerca.Items.Add("CAP")
                ddlRicerca.Items(7).Value = "M"
                ddlRicerca.Items.Add("Data consegna")
                ddlRicerca.Items(8).Value = "C"
                ddlRicerca.Items.Add("Data validità")
                ddlRicerca.Items(9).Value = "V"
                ddlRicerca.Items.Add("Riferimento")
                ddlRicerca.Items(10).Value = "NR"
                ddlRicerca.Items.Add("Codice CoGe")
                ddlRicerca.Items(11).Value = "CG"
                'GIU050314
                ddlRicerca.Items.Add("Codice Articolo")
                ddlRicerca.Items(12).Value = "CA"
                Session(CODARTSEL) = "Z" 'TUTTI
                '---------
                'giu130312
                BuidDett()
                Session(SWOP) = SWOPNESSUNA
            Catch Ex As Exception
                Chiudi("Errore caricamento Elenco Documenti: " & Ex.Message)
                Exit Sub
            End Try

        End If

        'Simone 290317
        WFPDocCollegati.WucElement = Me
        If Session(F_DOCCOLL_APERTA) Then
            WFPDocCollegati.Show()
        End If
    End Sub
    'giu080212
    Private Sub BtnSetByStato(ByVal myStato As String)
        BtnSetEnabledTo(True)
        If myStato.Trim = "OK trasf.in CoGe" Then
            btnModifica.Visible = False
        End If
        ' ''If Session(CSTTIPODOCSEL) = SWTD(TD.FatturaCommerciale) Then
        ' ''    btnStampaFattura.Visible = False
        ' ''    btnStampaTutte.Visible = False
        ' ''End If
        ' ''If Session(CSTTIPODOCSEL) = SWTD(TD.NotaCredito) Then
        ' ''    btnStampaFattura.Visible = False
        ' ''    btnStampaTutte.Visible = False
        ' ''End If
        ' ''If Session(CSTTIPODOCSEL) = SWTD(TD.DocTrasportoFornitori) Then
        ' ''    btnStampaFattura.Visible = False
        ' ''    btnStampaTutte.Visible = False
        ' ''End If

    End Sub

#Region " Funzioni e Procedure"
    Private Sub BtnSetEnabledTo(ByVal Valore As Boolean)
        btnStampaTutte.Enabled = Valore
        btnStampaFattura.Enabled = Valore
        btnModifica.Enabled = Valore
        btnDocCollegati.Enabled = Valore 'Simone290317
    End Sub
#End Region

#Region " Ordinamento e ricerca"

    Private Sub GridViewPrevT_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridViewPrevT.PageIndexChanging
        If (e.NewPageIndex = -1) Then
            GridViewPrevT.PageIndex = 0
        Else
            GridViewPrevT.PageIndex = e.NewPageIndex
        End If
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        SqlDSPrevTElenco.FilterExpression = ""
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "1"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "999999999"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "1"
            txtAlN.Text = "999999999"
        End If
        Session(CODARTSEL) = "Z" 'TUTTI
        'giu200912
        If txtRicerca.Text.Trim <> "" Then
            If ddlRicerca.SelectedValue = "N" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtRicerca.Text.Trim
                Else
                    SqlDSPrevTElenco.FilterExpression = "Numero = " & txtRicerca.Text.Trim
                End If
            ElseIf ddlRicerca.SelectedValue = "D" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "Data_Doc >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "Data_Doc = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "CG" Then 'GIU090212
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "R" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "C" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataOraConsegna >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataOraConsegna = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "V" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "Data_Validita >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "Data_Validita = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "L" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Localita = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "M" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "CAP = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "S" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Denominazione = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "P" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "F" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "NR" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Riferimento = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "CA" Then 'GIU050314
                Session(CODARTSEL) = Controlla_Apice(txtRicerca.Text.Trim)
            End If
        End If
        'giu140312
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "1"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "999999999"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "1"
            txtAlN.Text = "999999999"
        End If
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        SqlDSPrevTElenco.FilterExpression += "Numero >= " & txtDalN.Text.Trim & " AND "
        SqlDSPrevTElenco.FilterExpression += "Numero <= " & txtAlN.Text.Trim
        'giu140312
        If IsDate(txtDataA.Text.Trim) Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Data_Doc <= '" & CDate(txtDataA.Text.Trim) & "'"
        End If
        '----------
        'GIU110814
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        If chkFatturaPA.Checked = False Then
            SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)=0"
        Else
            SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)<>0"
        End If
        '---------
        SqlDSPrevTElenco.DataBind()
        'giu110412
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.Sorted
        GridViewPrevT.SelectedIndex = -1
        Session(IDDOCUMENTI) = ""
        SqlDSPrevTElenco.FilterExpression = ""
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "1"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "999999999"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "1"
            txtAlN.Text = "999999999"
        End If
        Session(CODARTSEL) = "Z" 'TUTTI
        'giu200912
        If txtRicerca.Text.Trim <> "" Then
            If ddlRicerca.SelectedValue = "N" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtRicerca.Text.Trim
                Else
                    SqlDSPrevTElenco.FilterExpression = "Numero = " & txtRicerca.Text.Trim
                End If
            ElseIf ddlRicerca.SelectedValue = "D" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "Data_Doc >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "Data_Doc = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "CG" Then 'GIU090212
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "R" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Rag_Soc = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "C" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "DataOraConsegna >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "DataOraConsegna = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "V" Then
                If IsDate(txtRicerca.Text.Trim) Then
                    txtRicerca.BackColor = SEGNALA_OK
                    If checkParoleContenute.Checked = True Then
                        SqlDSPrevTElenco.FilterExpression = "Data_Validita >= '" & CDate(txtRicerca.Text.Trim) & "'"
                    Else
                        SqlDSPrevTElenco.FilterExpression = "Data_Validita = '" & CDate(txtRicerca.Text.Trim) & "'"
                    End If
                Else
                    txtRicerca.BackColor = SEGNALA_KO
                End If
            ElseIf ddlRicerca.SelectedValue = "L" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Localita = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "M" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "CAP = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "S" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Denominazione = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "P" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Partita_IVA = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "F" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "NR" Then
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Riferimento = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
                End If
            ElseIf ddlRicerca.SelectedValue = "CA" Then 'GIU050314
                Session(CODARTSEL) = Controlla_Apice(txtRicerca.Text.Trim)
            End If
        End If
        'giu140312
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "1"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "999999999"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "1"
            txtAlN.Text = "999999999"
        End If
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        SqlDSPrevTElenco.FilterExpression += "Numero >= " & txtDalN.Text.Trim & " AND "
        SqlDSPrevTElenco.FilterExpression += "Numero <= " & txtAlN.Text.Trim
        'giu140312
        If IsDate(txtDataA.Text.Trim) Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Data_Doc <= '" & CDate(txtDataA.Text.Trim) & "'"
        End If
        '----------
        'GIU110814
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        If chkFatturaPA.Checked = False Then
            SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)=0"
        Else
            SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)<>0"
        End If
        '---------
        SqlDSPrevTElenco.DataBind()
        'giu110412
        GridViewPrevT.DataBind()
        GridViewPrevD.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
                GridViewPrevT_SelectedIndexChanged(GridViewPrevT, Nothing)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub
    Private Sub GridViewPrevT_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevT.RowDataBound
        'e.Row.Attributes.Add("onMouseOver", "this.style.cursor='hand';")
        'e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(GridViewPrevT, "Select$" + e.Row.RowIndex.ToString()))
        If e.Row.DataItemIndex > -1 Then
            If IsDate(e.Row.Cells(CellIdxT.DataDoc).Text) Then
                e.Row.Cells(CellIdxT.DataDoc).Text = Format(CDate(e.Row.Cells(CellIdxT.DataDoc).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataCons).Text) Then
                e.Row.Cells(CellIdxT.DataCons).Text = Format(CDate(e.Row.Cells(CellIdxT.DataCons).Text), FormatoData).ToString
            End If
            If IsDate(e.Row.Cells(CellIdxT.DataVal).Text) Then
                e.Row.Cells(CellIdxT.DataVal).Text = Format(CDate(e.Row.Cells(CellIdxT.DataVal).Text), FormatoData).ToString
            End If
            'TotaleDoc
            If IsNumeric(e.Row.Cells(CellIdxT.TotaleDoc).Text) Then
                If CDec(e.Row.Cells(CellIdxT.TotaleDoc).Text) <> 0 Then
                    e.Row.Cells(CellIdxT.TotaleDoc).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxT.TotaleDoc).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxT.TotaleDoc).Text = ""
                End If
            End If
        End If
    End Sub
    Private Sub GridViewPrevD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewPrevD.RowDataBound
        If e.Row.DataItemIndex > -1 Then
            If IsNumeric(e.Row.Cells(CellIdxD.QtaOr).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaOr).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaOr).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaOr).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaOr).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaEv).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaEv).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaEv).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaEv).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaEv).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaRe).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaRe).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaRe).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaRe).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaRe).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.QtaAl).Text) Then
                If CDec(e.Row.Cells(CellIdxD.QtaAl).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.QtaAl).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.QtaAl).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.QtaAl).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.IVA).Text) Then
                If CDec(e.Row.Cells(CellIdxD.IVA).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.IVA).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.IVA).Text), 0).ToString
                Else
                    e.Row.Cells(CellIdxD.IVA).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Prz).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Prz).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Prz).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Prz).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Prezzi).ToString
                Else
                    e.Row.Cells(CellIdxD.Prz).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScV).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScV).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScV).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScV).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.ScV).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Sc1).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Sc1).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Sc1).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Sc1).Text), GetParamGestAzi(Session(ESERCIZIO)).Decimali_Sconto).ToString
                Else
                    e.Row.Cells(CellIdxD.Sc1).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.Importo).Text) Then
                If CDec(e.Row.Cells(CellIdxD.Importo).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.Importo).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.Importo).Text), 2).ToString
                Else
                    e.Row.Cells(CellIdxD.Importo).Text = ""
                End If
            End If
            If IsNumeric(e.Row.Cells(CellIdxD.ScR).Text) Then
                If CDec(e.Row.Cells(CellIdxD.ScR).Text) <> 0 Then
                    e.Row.Cells(CellIdxD.ScR).Text = FormattaNumero(CDec(e.Row.Cells(CellIdxD.ScR).Text), 4).ToString
                Else
                    e.Row.Cells(CellIdxD.ScR).Text = ""
                End If
            End If
        End If
    End Sub
    Private Sub BuidDett()
        SqlDSPrevTElenco.FilterExpression = ""
        If ddlRicerca.SelectedValue = "N" Then
            If Not IsNumeric(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Or _
               ddlRicerca.SelectedValue = "V" Or _
               ddlRicerca.SelectedValue = "C" Then
            If Not IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.Text = ""
            End If
        End If
        Session(CODARTSEL) = "Z" 'TUTTI
        If txtRicerca.Text.Trim = "" Then
            'giu140312
            If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "1"
            If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "999999999"
            If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
                txtDalN.Text = "1"
                txtAlN.Text = "999999999"
            End If
            SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtDalN.Text.Trim & " AND "
            SqlDSPrevTElenco.FilterExpression += "Numero <= " & txtAlN.Text.Trim
            'giu140312
            If IsDate(txtDataA.Text.Trim) Then
                If SqlDSPrevTElenco.FilterExpression <> "" Then
                    SqlDSPrevTElenco.FilterExpression += " AND "
                End If
                SqlDSPrevTElenco.FilterExpression += "Data_Doc <= '" & CDate(txtDataA.Text.Trim) & "'"
            End If
            '-
            'GIU110814
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            If chkFatturaPA.Checked = False Then
                SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)=0"
            Else
                SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)<>0"
            End If
            '---------
            'GIU110814
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            If chkFatturaRA.Checked = False Then
                SqlDSPrevTElenco.FilterExpression += "ISNULL(RitAcconto,0)=0"
            Else
                SqlDSPrevTElenco.FilterExpression += "ISNULL(RitAcconto,0)<>0"
            End If
            '---------
            SqlDSPrevTElenco.DataBind()

            BtnSetEnabledTo(False)
            Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
            GridViewPrevT.DataBind()
            If GridViewPrevT.Rows.Count > 0 Then
                BtnSetEnabledTo(True)
                GridViewPrevT.SelectedIndex = 0
                Try
                    Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                    Dim row As GridViewRow = GridViewPrevT.SelectedRow
                    Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                    Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                    BtnSetByStato(Stato)
                Catch ex As Exception
                    Session(IDDOCUMENTI) = ""
                    Session(CSTTIPODOCSEL) = ""
                End Try
            Else
                BtnSetEnabledTo(False)
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End If
            Exit Sub
        End If
        If ddlRicerca.SelectedValue = "N" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Numero >= " & txtRicerca.Text.Trim
            Else
                SqlDSPrevTElenco.FilterExpression = "Numero = " & txtRicerca.Text.Trim
            End If
        ElseIf ddlRicerca.SelectedValue = "D" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Data_Doc >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Data_Doc = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "CG" Then 'GIU090212
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Cod_Cliente like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Cod_Cliente = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "R" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Rag_Soc like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Rag_Soc = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "C" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "DataOraConsegna >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "DataOraConsegna = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "V" Then
            If IsDate(txtRicerca.Text.Trim) Then
                txtRicerca.BackColor = SEGNALA_OK
                If checkParoleContenute.Checked = True Then
                    SqlDSPrevTElenco.FilterExpression = "Data_Validita >= '" & CDate(txtRicerca.Text.Trim) & "'"
                Else
                    SqlDSPrevTElenco.FilterExpression = "Data_Validita = '" & CDate(txtRicerca.Text.Trim) & "'"
                End If
            Else
                txtRicerca.BackColor = SEGNALA_KO
            End If
        ElseIf ddlRicerca.SelectedValue = "L" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Localita like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Localita = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "M" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "CAP like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "CAP = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "S" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Denominazione like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Denominazione = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "P" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Partita_IVA like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Partita_IVA = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "F" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Codice_Fiscale = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "NR" Then
            If checkParoleContenute.Checked = True Then
                SqlDSPrevTElenco.FilterExpression = "Riferimento like '%" & Controlla_Apice(txtRicerca.Text.Trim) & "%'"
            Else
                SqlDSPrevTElenco.FilterExpression = "Riferimento = '" & Controlla_Apice(txtRicerca.Text.Trim) & "'"
            End If
        ElseIf ddlRicerca.SelectedValue = "CA" Then 'GIU050314
            Session(CODARTSEL) = Controlla_Apice(txtRicerca.Text.Trim)
        End If
        'giu140312
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "1"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "999999999"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "1"
            txtAlN.Text = "999999999"
        End If
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        SqlDSPrevTElenco.FilterExpression += "Numero >= " & txtDalN.Text.Trim & " AND "
        SqlDSPrevTElenco.FilterExpression += "Numero <= " & txtAlN.Text.Trim
        'giu140312
        If IsDate(txtDataA.Text.Trim) Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Data_Doc <= '" & CDate(txtDataA.Text.Trim) & "'"
        End If
        '-
        'GIU110814
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        If chkFatturaPA.Checked = False Then
            SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)=0"
        Else
            SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)<>0"
        End If
        '---------
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If

    End Sub
    Private Sub ddlRicerca_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRicerca.SelectedIndexChanged
        Session(CODARTSEL) = "Z" 'TUTTI
        SqlDSPrevTElenco.FilterExpression = "" : txtRicerca.Text = ""
        If Not IsNumeric(txtDalN.Text.Trim) Then txtDalN.Text = "1"
        If Not IsNumeric(txtAlN.Text.Trim) Then txtAlN.Text = "999999999"
        If Val(txtDalN.Text.Trim) > Val(txtAlN.Text.Trim) Then
            txtDalN.Text = "1"
            txtAlN.Text = "999999999"
        End If
        SqlDSPrevTElenco.FilterExpression += "Numero >= " & txtDalN.Text.Trim & " AND "
        SqlDSPrevTElenco.FilterExpression += "Numero <= " & txtAlN.Text.Trim
        'giu140312
        If IsDate(txtDataA.Text.Trim) Then
            If SqlDSPrevTElenco.FilterExpression <> "" Then
                SqlDSPrevTElenco.FilterExpression += " AND "
            End If
            SqlDSPrevTElenco.FilterExpression += "Data_Doc <= '" & CDate(txtDataA.Text.Trim) & "'"
        End If
        '-
        'GIU110814
        If SqlDSPrevTElenco.FilterExpression <> "" Then
            SqlDSPrevTElenco.FilterExpression += " AND "
        End If
        If chkFatturaPA.Checked = False Then
            SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)=0"
        Else
            SqlDSPrevTElenco.FilterExpression += "ISNULL(FatturaPA,0)<>0"
        End If
        '---------
        SqlDSPrevTElenco.DataBind()

        BtnSetEnabledTo(False)
        Session(SWOP) = SWOPNESSUNA : Session(SWMODIFICATO) = SWNO
        GridViewPrevT.DataBind()
        If GridViewPrevT.Rows.Count > 0 Then
            BtnSetEnabledTo(True)
            GridViewPrevT.SelectedIndex = 0
            Try
                Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
                Dim row As GridViewRow = GridViewPrevT.SelectedRow
                Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
                Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
                BtnSetByStato(Stato)
            Catch ex As Exception
                Session(IDDOCUMENTI) = ""
                Session(CSTTIPODOCSEL) = ""
            End Try
        Else
            BtnSetEnabledTo(False)
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End If
    End Sub

    Protected Sub btnRicerca_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRicerca.Click
        BuidDett()
    End Sub
#End Region

    Private Sub Chiudi(ByVal strErrore As String)

        ' ''Try
        ' ''    Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale" & strErrore)
        ' ''Catch ex As Exception
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", ex.Message & " " & strErrore, WUC_ModalPopup.TYPE_ERROR)
        ' ''End Try
        Dim strRitorno As String = ""
        strRitorno = ""
        If strErrore.Trim = "" Then
            strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        Else
            strRitorno = "WF_Menu.aspx?labelForm=" & strErrore
        End If
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try
    End Sub
    Private Function CKCSTTipoDocSel(Optional ByRef myTD As String = "", Optional ByRef myTabCliFor As String = "") As Boolean
        CKCSTTipoDocSel = True
        TipoDoc = Session(CSTTIPODOCSEL)
        If IsNothing(TipoDoc) Then
            TipoDoc = ""
            Return False
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            TipoDoc = ""
            Return False
        End If
        strDesTipoDocumento = DesTD(TipoDoc)
        myTD = TipoDoc
        If CtrTipoDoc(TipoDoc, TabCliFor) = False Then
            Return False
        End If
        myTabCliFor = TabCliFor
    End Function

    Private Sub GridViewPrevT_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewPrevT.SelectedIndexChanged

        Try
            Session(IDDOCUMENTI) = GridViewPrevT.SelectedDataKey.Value
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            Session(CSTTIPODOCSEL) = row.Cells(CellIdxT.TipoDC).Text.Trim
            Dim Stato As String = row.Cells(CellIdxT.Stato).Text.Trim
            BtnSetByStato(Stato)
        Catch ex As Exception
            Session(IDDOCUMENTI) = ""
            Session(CSTTIPODOCSEL) = ""
        End Try
    End Sub

    Private Sub btnStampaFattura_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaFattura.Click
        Dim StrErrore As String = ""
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "TIPO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        'giu080512
        If Session(IDDOCUMENTI).ToString.Trim <> GridViewPrevT.SelectedDataKey.Value.ToString.Trim Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Numero documento non uguale a quello visualizzato. <br> " _
                            & "Selezionare un documento.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim strNDoc As String = ""
        Try
            Dim row As GridViewRow = GridViewPrevT.SelectedRow
            strNDoc = row.Cells(CellIdxT.NumDoc).Text.Trim
        Catch ex As Exception
            strNDoc = ""
        End Try
        '---------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        TipoDoc = Session(CSTTIPODOCSEL)
        myID = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Chiudi("Errore: IDENTIFICATIVO DOCUMENTO SCONOSCIUTO")
            Exit Sub
        End If
        Dim SWSconti As Boolean = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(Session(IDDOCUMENTI), TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                ' ''Session(CSTNOBACK) = 0 
                ' ''Response.Redirect("..\WebFormTables\WF_PrintWebCR.aspx?labelForm=ESPORTA: " & Session(IDDOCUMENTI).ToString.Trim)
                Session(ATTESA_CALLBACK_METHOD) = ""
                Session(CSTNOBACK) = 1
                'giu100324
                If chkEsporta.Checked = False Then
                    Attesa.ShowStampaAll2("Stampa singola Fattura. N° " & strNDoc, "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Stampa singola Fattura. N° " & strNDoc)
                Else
                    Attesa.ShowStampaAll2("Stampa singola Fattura. N° " & strNDoc, "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=ESPORTA Stampa singola Fattura. N° " & strNDoc)
                End If
                '-
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub txtDataA_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDataA.TextChanged
        BuidDett()
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnModifica.Click
        If CKCSTTipoDocSel() = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
        End If
        If Not IsNumeric(myID) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        Session(CSTTIPODOC) = Session(CSTTIPODOCSEL)
        Session(CSTSWRbtnTD) = Session(CSTTIPODOCSEL)
        Session(SWOP) = SWOPMODIFICA
        Response.Redirect("WF_Documenti.aspx?labelForm=" & strDesTipoDocumento)
    End Sub

    Private Sub btnChiudi_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChiudi.Click
        Chiudi("")
    End Sub

    Private Sub btnCercaNum_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCercaNum.Click
        BuidDett()
    End Sub

    'giu140312
    Private Sub btnStampaTutte_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampaTutte.Click
        BuidDett()
        'giu200912 
        Dim dvElDoc As DataView
        dvElDoc = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)
        'non serve in quanto prende il rowfilter quando viene assegnato
        'dvElDoc.RowFilter = SqlDSPrevTElenco.FilterExpression
        If (dvElDoc Is Nothing) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la ristampa.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        If dvElDoc.Count > 0 Then dvElDoc.Sort = "Numero"
        '--------------------------------------------------------------------
        Dim strNDoc As String = "" : Dim strDataDoc As String = ""
        If dvElDoc.Count > 0 Then 'giu200912 If GridViewPrevT.Rows.Count > 0 Then
            'ok procedo mi serve solo il primo 
            'N°DOCUMENTO
            If Not IsDBNull(dvElDoc.Item(0).Item("Numero")) Then
                strNDoc = FormattaNumero(dvElDoc.Item(0).Item("Numero"))
            Else
                strNDoc = ""
            End If
            If String.IsNullOrEmpty(strNDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUMERO DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'DATA DOCUMENTO
            If Not IsDBNull(dvElDoc.Item(0).Item("Data_Doc")) Then
                strDataDoc = Format(dvElDoc.Item(0).Item("Data_Doc"), FormatoData)
            Else
                strDataDoc = ""
            End If
            If String.IsNullOrEmpty(strDataDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf Not IsDate(CDate(strDataDoc)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            ' ''For Each rowCTR As GridViewRow In GridViewPrevT.Rows
            ' ''    'N°DOCUMENTO
            ' ''    strNDoc = rowCTR.Cells(CellIdxT.NumDoc).Text
            ' ''    If String.IsNullOrEmpty(strNDoc) Then
            ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''        ModalPopup.Show("Errore", "NUMERO DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            ' ''        Exit Sub
            ' ''    End If
            ' ''    'DATA DOCUMENTO
            ' ''    strDataDoc = rowCTR.Cells(CellIdxT.DataDoc).Text
            ' ''    If String.IsNullOrEmpty(strDataDoc) Then
            ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''        ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            ' ''        Exit Sub
            ' ''    ElseIf Not IsDate(CDate(strDataDoc)) Then
            ' ''        Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''        ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            ' ''        Exit Sub
            ' ''    End If
            ' ''    Exit For
            ' ''Next
        Else
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la ristampa.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        'giu040512
        Session(EL_DOC_TOPRINT_SCY) = Nothing
        Session(EL_DOC_TOPRINT_SCN) = Nothing
        '---------
        Session(MODALPOPUP_CALLBACK_METHOD) = "StampaFattureTutte"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ModalPopup.Show("Stampa TUTTE le Fatture selezionate", "Confermi la stampa di TUTTE le Fatture ? <br>" & _
                        "Primo documento selezionato per la stampa: N° " & strNDoc & " del " & strDataDoc, WUC_ModalPopup.TYPE_CONFIRM_YN)
    End Sub
   
    Public Sub StampaFattureTutte() 'TUTTE

        Session(CSTDECIMALIVALUTADOC) = ""
        Dim StrErrore As String = ""
        Dim strNDoc As String = "" : Dim strDataDoc As String = "" : Dim strPr As String = "" : Dim strCausale As String = ""
        Dim myID As String = ""
        Dim strCPag As String = "" : Dim strTotaleDoc As String = "" : Dim Valuta As String = ""
        Dim strABI As String = "" : Dim strCAB As String = ""
        'giu160312 richiesta di Cinzia
        Dim strPI As String = "" : Dim strCF As String = ""
        'CONTROLLO
        'giu200912 
        BuidDett()
        Dim dvElDoc As DataView
        dvElDoc = SqlDSPrevTElenco.Select(DataSourceSelectArguments.Empty)
        'non serve in quanto prende il rowfilter quando viene assegnato
        'dvElDoc.RowFilter = SqlDSPrevTElenco.FilterExpression
        If (dvElDoc Is Nothing) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la ristampa.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        If dvElDoc.Count > 0 Then dvElDoc.Sort = "Numero"
        If dvElDoc.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato per la ristampa.", WUC_ModalPopup.TYPE_CONFIRM_Y)
            Exit Sub
        End If
        Dim i As Integer = 0 : Dim ii As Integer = 0
        Dim SWSconti As Boolean = False
        Dim ELDocToPrintSCY As New List(Of String)
        Dim ELDocToPrintSCN As New List(Of String)
        '------------------------------------------------------
        Dim SWTotaleZero As Boolean = False 'giu290519
        For i = 1 To dvElDoc.Count 'giu200912 For Each rowCTR As GridViewRow In GridViewPrevT.Rows
            'N°DOCUMENTO
            If Not IsDBNull(dvElDoc.Item(ii).Item("Numero")) Then
                strNDoc = FormattaNumero(dvElDoc.Item(ii).Item("Numero"))
            Else
                strNDoc = ""
            End If
            If String.IsNullOrEmpty(strNDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "NUMERO DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'DATA DOCUMENTO
            If Not IsDBNull(dvElDoc.Item(ii).Item("Data_Doc")) Then
                strDataDoc = Format(dvElDoc.Item(ii).Item("Data_Doc"), FormatoData)
            Else
                strDataDoc = ""
            End If
            If String.IsNullOrEmpty(strDataDoc) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf Not IsDate(CDate(strDataDoc)) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "DATA DOCUMENTO ERRATO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'ID DOCUMENTI
            'ID = rowCTR.Cells(CellIdxT.IDDoc).Text
            If Not IsDBNull(dvElDoc.Item(ii).Item("IDDocumenti")) Then
                myID = dvElDoc.Item(ii).Item("IDDocumenti").ToString.Trim
            Else
                myID = ""
            End If
            Session(IDDOCUMENTI) = myID
            myID = Session(IDDOCUMENTI)
            If IsNothing(myID) Then
                myID = ""
            End If
            If String.IsNullOrEmpty(myID) Then
                myID = ""
            End If
            If Not IsNumeric(myID) Then
                ' ''Chiudi("Errore: " & "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.")
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO. N°: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'CODICE PAGAMENTO
            'strCPag = rowCTR.Cells(CellIdxT.Cod_Pagamento).Text
            If Not IsDBNull(dvElDoc.Item(ii).Item("Cod_Pagamento")) Then
                strCPag = dvElDoc.Item(ii).Item("Cod_Pagamento").ToString.Trim
            Else
                strCPag = ""
            End If
            If String.IsNullOrEmpty(strCPag) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf Not IsNumeric(strCPag) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            ElseIf CInt(strCPag) = 0 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE PAGAMENTO SCONOSCIUTO. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'VALUTA
            'Valuta = rowCTR.Cells(CellIdxT.Cod_Valuta).Text
            If Not IsDBNull(dvElDoc.Item(ii).Item("Cod_Valuta")) Then
                Valuta = dvElDoc.Item(ii).Item("Cod_Valuta").ToString.Trim
            Else
                Valuta = ""
            End If
            If String.IsNullOrEmpty(Valuta) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CODICE VALUTA SCONOSCIUTO. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'TOTALE DOCUMENTO giu290519 SEGNALO SOLO SE CI FOSSERO DEI DOCUMENTI CON TOTALE A ZERO
            'strTotaleDoc = rowCTR.Cells(CellIdxT.TotaleDoc).Text

            If Not IsDBNull(dvElDoc.Item(ii).Item("TotaleDoc")) Then
                strTotaleDoc = dvElDoc.Item(ii).Item("TotaleDoc").ToString.Trim
            Else
                strTotaleDoc = ""
            End If
            If String.IsNullOrEmpty(strTotaleDoc) Then
                SWTotaleZero = True
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Sub
            ElseIf Not IsNumeric(strTotaleDoc.Trim) Then
                SWTotaleZero = True
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Sub
            ElseIf CDec(strTotaleDoc) = 0 Then
                SWTotaleZero = True
                ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
                ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ' ''ModalPopup.Show("Errore", "TOTALE DOCUMENTO ERRATO. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                ' ''Exit Sub
            End If
            'GIU030315 STRANO SEGNALA CHE NON C'è
            '' ''ABI
            '' ''strABI = rowCTR.Cells(CellIdxT.ABI).Text
            ' ''If Not IsDBNull(dvElDoc.Item(ii).Item("ABI")) Then
            ' ''    strABI = dvElDoc.Item(ii).Item("ABI").ToString.Trim
            ' ''Else
            ' ''    strABI = ""
            ' ''End If
            ' ''If String.IsNullOrEmpty(strABI) Then
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''    ModalPopup.Show("Errore", "CODICE ABI. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            ' ''    Exit Sub
            ' ''End If
            '' ''CAB
            '' ''strCAB = rowCTR.Cells(CellIdxT.CAB).Text
            ' ''If Not IsDBNull(dvElDoc.Item(ii).Item("CAB")) Then
            ' ''    strCAB = dvElDoc.Item(ii).Item("CAB").ToString.Trim
            ' ''Else
            ' ''    strCAB = ""
            ' ''End If
            ' ''If String.IsNullOrEmpty(strCAB) Then
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''    ModalPopup.Show("Errore", "CODICE CAB. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
            ' ''    Exit Sub
            ' ''End If
            'PI'CF
            'strPI = rowCTR.Cells(CellIdxT.PI).Text
            If Not IsDBNull(dvElDoc.Item(ii).Item("Partita_IVA")) Then
                strPI = dvElDoc.Item(ii).Item("Partita_IVA").ToString.Trim
            Else
                strPI = ""
            End If
            'strCF = rowCTR.Cells(CellIdxT.CF).Text
            If Not IsDBNull(dvElDoc.Item(ii).Item("Codice_Fiscale")) Then
                strCF = dvElDoc.Item(ii).Item("Codice_Fiscale").ToString.Trim
            Else
                strCF = ""
            End If
            If String.IsNullOrEmpty(strPI) And String.IsNullOrEmpty(strCF) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "P.IVA - Cod.Fiscale MANCANTI. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'PROVINCIA
            'strPr = rowCTR.Cells(CellIdxT.Pr).Text
            If Not IsDBNull(dvElDoc.Item(ii).Item("Provincia")) Then
                strPr = dvElDoc.Item(ii).Item("Provincia").ToString.Trim
            Else
                strPr = ""
            End If
            If String.IsNullOrEmpty(strPr) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "PROVINCIA NON DEFINITA. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'CAUSALE
            'strCausale = rowCTR.Cells(CellIdxT.DesCausale).Text
            If Not IsDBNull(dvElDoc.Item(ii).Item("DesCausale")) Then
                strCausale = dvElDoc.Item(ii).Item("DesCausale").ToString.Trim
            Else
                strCausale = ""
            End If
            If String.IsNullOrEmpty(strCausale) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "CAUSALE NON DEFINITA. N° Fattura: " & strNDoc, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'giu210912
            'OK POSSO STAMPARE LA FATTURA 'giu210912 .. VERIFICO se ha sconti o no per il modulo diveso
            Session(CSTTIPODOC) = SWTD(TD.FatturaCommerciale)
            SWSconti = CKSconti(CLng(myID), StrErrore)
            If SWSconti = True Then
                Session(CSTSWScontiDoc) = 1
                'giu040512 giu070512
                If Not IsNothing(Session(EL_DOC_TOPRINT_SCY)) Then
                    ELDocToPrintSCY = Session(EL_DOC_TOPRINT_SCY)
                End If
                ELDocToPrintSCY.Add(myID.Trim)
                Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
            Else
                Session(CSTSWScontiDoc) = 0
                'giu040512 giu070512
                If Not IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
                    ELDocToPrintSCN = Session(EL_DOC_TOPRINT_SCN)
                End If
                ELDocToPrintSCN.Add(myID.Trim)
                Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
            End If
            Session(CSTSWConfermaDoc) = 0
            ii += 1 'giu200912
            If i > 500 Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Superato il limite massimo di documenti da stampare. <br> <strong><span> " & _
                    "Limite massimo consentito 500 documenti</span></strong>", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            '---------
        Next
        '' ''FINE CONTROLLO
        'OK FATTO
        Session(MODALPOPUP_CALLBACK_METHOD) = "StampaDocumentiAll1"
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CallChiudi"
        If SWTotaleZero = False Then
            ModalPopup.Show("Stampa TUTTE le Fatture selezionate", _
                "Selezione/Controllo fatture avvenuta con successo. <br> <strong><span> " & _
                "Vuole stampare tutte le fatture ?</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        Else
            ModalPopup.Show("Stampa TUTTE le Fatture selezionate", _
                "Selezione/Controllo fatture avvenuta con successo. <br> <strong><span> " & _
                "ATTENZIONE, alcune Fatture risultano con importo uguale a ZERO " & _
                "Vuole stampare tutte le fatture ?</span></strong>", WUC_ModalPopup.TYPE_CONFIRM_YN)
        End If
    End Sub

    'giu160312
    ' ''Private Function OKStampaDoc(ByVal IDDocFC As Long, ByVal NDoc As long, ByRef strErrore As String) As Boolean
    ' ''    OKStampaDoc = True
    ' ''    Dim DsPrinWebDoc As New DSPrintWeb_Documenti
    ' ''    Dim ObjReport As New Object
    ' ''    Dim ClsPrint As New Documenti
    ' ''    strErrore = ""
    ' ''    Dim SWSconti As Boolean = False
    ' ''    Dim ELDocToPrintSCY As New List(Of String)
    ' ''    Dim ELDocToPrintSCN As New List(Of String)
    ' ''    Try
    ' ''        DsPrinWebDoc.Clear() 'GIU020512
    ' ''        If ClsPrint.StampaDocumento(IDDocFC.ToString.Trim, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, "Cli", DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
    ' ''            Session(CSTObjReport) = ObjReport
    ' ''            Session(CSTDsPrinWebDoc) = DsPrinWebDoc
    ' ''            If SWSconti = True Then
    ' ''                Session(CSTSWScontiDoc) = 1
    ' ''                'giu040512 giu070512
    ' ''                If Not IsNothing(Session(EL_DOC_TOPRINT_SCY)) Then
    ' ''                    ELDocToPrintSCY = Session(EL_DOC_TOPRINT_SCY)
    ' ''                End If
    ' ''                ELDocToPrintSCY.Add(IDDocFC.ToString.Trim)
    ' ''                Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
    ' ''            Else
    ' ''                Session(CSTSWScontiDoc) = 0
    ' ''                'giu040512 giu070512
    ' ''                If Not IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
    ' ''                    ELDocToPrintSCN = Session(EL_DOC_TOPRINT_SCN)
    ' ''                End If
    ' ''                ELDocToPrintSCN.Add(IDDocFC.ToString.Trim)
    ' ''                Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
    ' ''            End If
    ' ''            Session(CSTSWConfermaDoc) = 0
    ' ''        Else
    ' ''            'L'ERRORE è INIZIALIZZATO ALLA FUNZIONE STAMPADOCUMENTO
    ' ''            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''            ' ''ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
    ' ''            OKStampaDoc = False
    ' ''            'giu040512
    ' ''            ELDocToPrintSCN = Nothing
    ' ''            Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
    ' ''            ELDocToPrintSCY = Nothing
    ' ''            Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
    ' ''        End If
    ' ''    Catch ex As Exception
    ' ''        OKStampaDoc = False
    ' ''        'giu040512
    ' ''        ELDocToPrintSCN = Nothing
    ' ''        Session(EL_DOC_TOPRINT_SCN) = ELDocToPrintSCN
    ' ''        ELDocToPrintSCY = Nothing
    ' ''        Session(EL_DOC_TOPRINT_SCY) = ELDocToPrintSCY
    ' ''        ' ''Chiudi("Errore:" & ex.Message)
    ' ''        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
    ' ''        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
    ' ''        ' ''ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
    ' ''        strErrore = "Errore:" & ex.Message
    ' ''    End Try

    ' ''End Function

    Public Sub StampaDocumentiAll1()
        If IsNothing(Session(EL_DOC_TOPRINT_SCY)) Then
            StampaDocumentiAll2()
            Exit Sub
        End If
        '-----------------------------------------------------------------------------
        'giu030512 ESEMPIO COME STAMPARE TUTTI I DOCUMENTI PERSONALIZZATI (DDT,FC,etc)
        '-----------------------------------------------------------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Dim SWSconti As Boolean = False
        Dim SWRitAcc As Boolean = False
        DsPrinWebDoc.Clear()

        '--------
        Dim SWOk As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ELDocToPrintSCY As List(Of String) = Session(EL_DOC_TOPRINT_SCY)
        Dim TotFatture As Integer = 0
        If (ELDocToPrintSCY.Count > 0) Then
            TotFatture = ELDocToPrintSCY.Count
            Try
                For Each Codice As String In ELDocToPrintSCY
                    NumInt = Codice
                    SWOk = ClsPrint.StampaDocumentiDalAl(NumInt, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, SWRitAcc, StrErrore)
                    If SWOk = False Then Exit For
                Next
                If SWOk = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante la stampa fatture con SCONTI. N° Interno (" & NumInt & ") Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTSWScontiDoc) = 1
                Session(CSTSWRitAcc) = IIf(SWRitAcc = True, 1, 0)
                Session(CSTSWConfermaDoc) = 0
            Catch ex As Exception
                SWOk = False
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante la stampa fatture con SCONTI. N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else
            StampaDocumentiAll2()
            Exit Sub
        End If
        ObjDB = Nothing
        '---------
        Session(ATTESA_CALLBACK_METHOD) = "StampaDocumentiAll2"
        Session(CSTNOBACK) = 1
        Dim SWOKAll2 As Boolean = True
        If IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
            SWOKAll2 = False
        Else
            Dim ELDocToPrintSCN As List(Of String) = Session(EL_DOC_TOPRINT_SCN)
            If (ELDocToPrintSCN.Count > 0) Then
                'nulla
            Else
                SWOKAll2 = False
            End If
        End If
        If SWOKAll2 = True Then
            'giu100324
            If chkEsporta.Checked = False Then
                Attesa.ShowStampaAll1("Totale Fatture con SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Fatture con SCONTI")
            Else
                Attesa.ShowStampaAll1("Totale Fatture con SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=ESPORTA Fatture con SCONTI")
            End If
            '-
        Else
            'giu100324
            If chkEsporta.Checked = False Then
                Attesa.ShowStampaAll2("Totale Fatture con SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Fatture con SCONTI")
            Else
                Attesa.ShowStampaAll2("Totale Fatture con SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=ESPORTA Fatture con SCONTI")
            End If
            '-
        End If
    End Sub
    Public Sub StampaDocumentiAll2()
        If IsNothing(Session(EL_DOC_TOPRINT_SCN)) Then
            Exit Sub
        End If
        '-----------------------------------------------------------------------------
        'giu030512 ESEMPIO COME STAMPARE TUTTI I DOCUMENTI PERSONALIZZATI (DDT,FC,etc)
        '-----------------------------------------------------------------------------
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Dim StrErrore As String = ""
        Dim SWSconti As Boolean = False
        Dim SWRitAcc As Boolean = False
        DsPrinWebDoc.Clear()

        '--------
        Dim SWOk As Boolean = True : Dim NumInt As String = ""
        Dim SQLStr As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ELDocToPrintSCN As List(Of String) = Session(EL_DOC_TOPRINT_SCN)
        Dim TotFatture As Integer = 0
        If (ELDocToPrintSCN.Count > 0) Then
            TotFatture = ELDocToPrintSCN.Count
            Try
                For Each Codice As String In ELDocToPrintSCN
                    NumInt = Codice
                    SWOk = ClsPrint.StampaDocumentiDalAl(NumInt, SWTD(TD.FatturaCommerciale), Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, SWRitAcc, StrErrore)
                    If SWOk = False Then Exit For
                Next
                If SWOk = False Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Errore", "Errore durante la stampa fatture NO SCONTI. N° Interno (" & NumInt & ") Err.: " & StrErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                Session(CSTSWScontiDoc) = 0
                Session(CSTSWRitAcc) = IIf(SWRitAcc = True, 1, 0)
                Session(CSTSWConfermaDoc) = 0
            Catch ex As Exception
                SWOk = False
                ObjDB = Nothing
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", "Errore durante la stampa fatture NO SCONTI. N° Interno (" & NumInt & ") Err.: " & ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try
        Else
            Exit Sub
        End If
        ObjDB = Nothing
        '---------
        Session(ATTESA_CALLBACK_METHOD) = "CallChiudi"
        Session(CSTNOBACK) = 1
        'giu100324
        If chkEsporta.Checked = False Then
            Attesa.ShowStampaAll2("Totale Fatture NO SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Fatture NO SCONTI")
        Else
            Attesa.ShowStampaAll2("Totale Fatture NO SCONTI: " & FormattaNumero(TotFatture), "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=ESPORTA Fatture NO SCONTI")
        End If
        '-
    End Sub
    Public Sub CallChiudi()
        Chiudi("")
    End Sub

    Private Sub chkFatturaPA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFatturaPA.CheckedChanged
        If chkFatturaPA.Checked = True Then
            chkFatturaRA.AutoPostBack = False
            chkFatturaRA.Checked = False
            chkFatturaRA.AutoPostBack = True
        Else
            chkFatturaRA.AutoPostBack = False
            chkFatturaRA.Checked = True
            chkFatturaRA.AutoPostBack = True
        End If
        BuidDett()
    End Sub
    Private Sub chkFatturaRA_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkFatturaRA.CheckedChanged
        If chkFatturaRA.Checked = True Then
            chkFatturaPA.AutoPostBack = False
            chkFatturaPA.Checked = False
            chkFatturaPA.AutoPostBack = True
        Else
            chkFatturaPA.AutoPostBack = False
            chkFatturaPA.Checked = True
            chkFatturaPA.AutoPostBack = True
        End If
        BuidDett()
    End Sub

    'Simone290317
#Region "Creazione DOCUMENTI COLLEGATI"
    Public Sub CancBackWFPDocCollegati()
        'nulla
    End Sub

    Public Sub CallBackWFPDocCollegati()
        'Dim myID As String = Session(IDDOCUMENTI)
        'If IsNothing(myID) Then
        '    myID = ""
        'End If
        'If String.IsNullOrEmpty(myID) Then
        '    myID = ""
        'End If
        'If Not IsNumeric(myID) Then
        '    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        '    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        '    ModalPopup.Show("Errore", "IDENTIFICATIVO DOCUMENTO SCONOSCIUTO.", WUC_ModalPopup.TYPE_ERROR)
        '    Exit Sub
        'End If
    End Sub

    Private Sub btnDocCollegati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDocCollegati.Click
        Dim myID As String = Session(IDDOCUMENTI)
        If IsNothing(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Nessun documento selezionato", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        Session(IDDOCUMCOLLCALL) = Session(IDDOCUMENTI) 'giu201221
        WFPDocCollegati.PopolaGrigliaWUCDocCollegati()
        ' ''WFPDocCollegati.WucElement = Me
        Session(F_DOCCOLL_APERTA) = True
        WFPDocCollegati.Show()
    End Sub
#End Region

    
End Class