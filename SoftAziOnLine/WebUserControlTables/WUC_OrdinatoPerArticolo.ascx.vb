Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
' ''Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports System.Data.SqlClient
Imports Microsoft.Reporting.WebForms
Imports SoftAziOnLine.Magazzino

Partial Public Class WUC_OrdinatoPerArticolo
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not IsPostBack) Then
            rbtnCodice.Checked = True
            chkTuttiArticoli.Checked = True
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCod1.Enabled = False
            btnCod2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False

            'txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            'txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            '-
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
        End If
        ModalPopup.WucElement = Me
        WFP_Articolo_SelezSing1.WucElement = Me
        If Session(F_SEL_ARTICOLO_APERTA) = True Then
            WFP_Articolo_SelezSing1.Show()
        End If
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        Dim DSOrdinatoArticolo1 As New DSOrdinatoArticolo
        Dim ObjReport As New Object
        Dim ClsPrint As New Ordinato
        Dim StrErrore As String = ""
        Dim Ordinamento As Integer 'INDICA SE REPORT è ORDINATO PER CODICE ARTICOLO O DESCRIZIONE ARTICOLO
        'CONTROLLI PRIMA DI AVVIARE LA STAMPA
        If chkTuttiArticoli.Checked = False Then
            If rbtnCodice.Checked Then
                If txtCod1.Text <> "" And txtCod2.Text <> "" Then
                    If txtCod1.Text > txtCod2.Text Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "Il codice di inizio intervallo è superiore a quello finale.", WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                End If
            Else
                If txtDesc1.Text <> "" And txtDesc2.Text <> "" Then
                    If txtDesc1.Text > txtDesc2.Text Then
                        Session(MODALPOPUP_CALLBACK_METHOD) = ""
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                        ModalPopup.Show("Attenzione", "La descrizione di inizio intervallo è superiore a quella finale.", WUC_ModalPopup.TYPE_ALERT)
                        Exit Sub
                    End If
                End If
            End If
        End If
        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Data inizio intervallo superiore alla data di fine.", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            End If
        End If
        Try
            Dim strErroreGiac As String = ""
            Dim SWNegativi As Boolean = False 'giu190613 forzo anche il riclacolo per l'impegno
            If Ricalcola_Giacenze("", strErroreGiac, SWNegativi, True) = False Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Ricalcolo giacenze", "Si è verificato un errore durante il ricalcolo delle giacenze: " & strErroreGiac, WUC_ModalPopup.TYPE_INFO)
            End If
            If txtDataDa.Text <> "" Or txtDataA.Text <> "" Then
                Session(CSTORDINATO) = 2 'REPORT ORDINATO PER ARTICOLO DATA
            Else
                Session(CSTORDINATO) = 1 'REPORT ORDINATO PER ARTICOLO
            End If
            If rbtnCodice.Checked Then
                Ordinamento = 1 'ORDINATO PER CODICE ARTICOLO
            Else
                Ordinamento = 2 'ORDINATO PER DESCRIZIONE ARTICOLO
            End If

            If ClsPrint.StampaOrdinatoArticolo(Session(CSTORDINATO), txtCod1.Text, txtCod2.Text, txtDesc1.Text, txtDesc2.Text, txtDataDa.Text, txtDataA.Text, Session(CSTAZIENDARPT), Ordinamento, DSOrdinatoArticolo1, ObjReport, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDSOrdinatoArticolo) = DSOrdinatoArticolo1
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebOrdinato.aspx")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", StrErrore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            ' ''Chiudi("Errore:" & ex.Message)
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Ordinato.btnStampa", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try

    End Sub

    Private Sub rbtnCodice_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnCodice.CheckedChanged
        If rbtnCodice.Checked = True Then
            pulisciCampi()
            If Not chkTuttiArticoli.Checked Then
                AbilitaDisabilitaComponenti(True)
                lblDal.Text = "Dal codice"
                lblAl.Text = "Al codice"
                txtCod1.Focus()
            Else
                lblDal.Text = "Dal codice"
                lblAl.Text = "Al codice"
            End If
        End If
    End Sub

    Private Sub rbtnDescrizione_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnDescrizione.CheckedChanged
        If rbtnDescrizione.Checked = True Then
            pulisciCampi()
            If Not chkTuttiArticoli.Checked Then
                AbilitaDisabilitaComponenti(False)
                lblDal.Text = "Dalla descrizione"
                lblAl.Text = "Alla descrizione"
                txtDesc1.Focus()
            Else
                lblDal.Text = "Dalla descrizione"
                lblAl.Text = "Alla descrizione"
            End If
        End If
    End Sub

    Private Sub chkTuttiArticoli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTuttiArticoli.CheckedChanged
        pulisciCampi()
        If chkTuttiArticoli.Checked Then
            txtCod1.Enabled = False
            txtCod2.Enabled = False
            btnCod1.Enabled = False
            btnCod2.Enabled = False
            txtDesc1.Enabled = False
            txtDesc2.Enabled = False
            btnStampa.Focus()
        Else
            If rbtnCodice.Checked = True Then
                AbilitaDisabilitaComponenti(True)
                txtCod1.Focus()
            Else
                AbilitaDisabilitaComponenti(False)
                txtDesc1.Focus()
            End If
        End If
    End Sub
    Private Sub AbilitaDisabilitaComponenti(ByVal Abilita As Boolean)
        txtCod1.Enabled = Abilita
        txtCod2.Enabled = Abilita
        btnCod1.Enabled = Abilita
        btnCod2.Enabled = Abilita
        txtDesc1.Enabled = Not (Abilita)
        txtDesc2.Enabled = Not (Abilita)
    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Try
            Response.Redirect("WF_Menu.aspx?labelForm=Menu principale ")
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        End Try
    End Sub
    Private Sub pulisciCampi()
        txtCod1.Text = ""
        txtCod2.Text = ""
        txtDesc1.Text = ""
        txtDesc2.Text = ""
    End Sub

    Private Sub txtCod1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod1.TextChanged
        txtDesc1.Text = App.GetValoreFromChiave(txtCod1.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub

    Private Sub txtCod2_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCod2.TextChanged
        txtDesc2.Text = App.GetValoreFromChiave(txtCod2.Text, Def.E_COD_ARTICOLI, Session(ESERCIZIO))
    End Sub

    Private Sub btnCod1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod1.Click
        Session(SWCOD1COD2) = 1
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Private Sub btnCod2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCod2.Click
        Session(SWCOD1COD2) = 2
        WFP_Articolo_SelezSing1.WucElement = Me
        Session(F_SEL_ARTICOLO_APERTA) = True
        WFP_Articolo_SelezSing1.Show()
    End Sub

    Public Sub CallBackWFPArticoloSelSing()
        'giu300512 Gestione singolo articolo usato inizalmente da Gestione contratti/Articoli installati
        'Public Const ARTICOLO_COD_SEL As String = "CodArticoloSelezionato"
        'Public Const ARTICOLO_DES_SEL As String = "DesArticoloSelezionato"
        'Public Const ARTICOLO_LBASE_SEL As String = "LBaseArticoloSelezionato"
        'Public Const ARTICOLO_LOPZ_SEL As String = "LOpzArticoloSelezionato"
        '-----------------------------------------------------------------------------------------------
        If String.IsNullOrEmpty(Session(SWCOD1COD2)) Then
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        ElseIf IsNumeric(Session(SWCOD1COD2).ToString.Trim) Then
            If Session(SWCOD1COD2).ToString.Trim = "1" Then
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                If txtCod2.Text.Trim = "" Then
                    txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                    txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                    txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                End If
            ElseIf Session(SWCOD1COD2).ToString.Trim = "2" Then
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            Else
                txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
                txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
                txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            End If
        Else
            txtCod1.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod1.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc1.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtCod2.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
            txtCod2.ToolTip = Session(ARTICOLO_DES_SEL).ToString.Trim
            txtDesc2.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        End If
        'TxtArticoloCod.Text = Session(ARTICOLO_COD_SEL).ToString.Trim
        'TxtArticoloDesc.Text = Session(ARTICOLO_DES_SEL).ToString.Trim
        'txt.Text = Session(ARTICOLO_LBASE_SEL)
        'txt.text = Session(ARTICOLO_LOPZ_SEL)
    End Sub
End Class