Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_LottiInsert
    Inherits System.Web.UI.UserControl

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objLotti As New WUC_LottiInsert
        objLotti.btnOk_Click(CInt(Session("NRiga")), txtCodArtIns.Text, Session("dt"), Session(IDDOCUMENTI))
        Session(F_CARICALOTTI) = False
        _WucElement.PostBackLottiInsert()
        ProgrammaticModalPopup.Dispose()
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(F_CARICALOTTI) = False
        _WucElement.PostBackLottiInsert()
        ProgrammaticModalPopup.Dispose()
    End Sub

    Public Sub GetTipoScansioneSL(ByRef valore As String, ByRef myQta As String)
        valore = "NSerie"
        myQta = txtQtaEv.Text.Trim
        If rbtnNLotto.Checked Then
            valore = "NLotto"
        ElseIf rbtnNSerie.Checked Then
            valore = "NSerie"
        Else
            valore = "NSerieNLotto"
        End If
    End Sub

    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        If PrimaVolta = True Then
            lblMessUtente.ForeColor = Drawing.Color.Black
            lblMessUtente.Text = "Scansionare il N° Serie / N° Lotto da caricare, secondo il metodo di Lettura selezionato."
            WUCLottiInsert.CaricaGridLottiDB()
        End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.ForeColor = Drawing.Color.Black
        lblMessUtente.Text = valore
    End Sub
    Public Sub SetLblMessUtenteRED(ByVal valore As String)
        lblMessUtente.ForeColor = Drawing.Color.Red
        lblMessUtente.Text = valore
    End Sub

    Public Sub setValoriRiga(ByVal txtCodArtIns1 As String, ByVal txtDesArtIns1 As String, ByVal txtIVAIns1 As String, ByVal txtPrezzoIns1 As String, ByVal txtQtaEv1 As String, ByVal txtQtaIns1 As String, ByVal txtSconto1Ins1 As String, ByVal txtUMIns1 As String, ByVal LblPrezzoNetto1 As String, ByVal LblImportoRiga1 As String, ByVal lblGiacenza1 As String, ByVal lblGiacImp1 As String, ByVal lblOrdFor1 As String, ByVal lblDataArr1 As String, ByVal NRiga As Int32, ByVal lblLabelQtaRe1 As String)

        Session("NRiga") = ""
        Session(COD_ARTICOLO) = ""

        Session(COD_ARTICOLO) = txtCodArtIns1
        Session("NRiga") = NRiga

        Session("passaDaDB") = ""
        Session("POPOLAGRID") = ""
        Session("NSerieChange") = ""
        Session("QtaEvasa") = ""
        Session("QtaEvasa") = txtQtaEv1

        txtCodArtIns.Text = txtCodArtIns1
        txtCodArtIns.Enabled = False
        txtDesArtIns.Text = txtDesArtIns1
        txtDesArtIns.Enabled = False
        txtIVAIns.Text = txtIVAIns1
        txtIVAIns.Enabled = False
        txtPrezzoIns.Text = txtPrezzoIns1
        txtPrezzoIns.Enabled = False
        txtQtaEv.Text = txtQtaEv1
        txtQtaEv.Enabled = False
        txtQtaIns.Text = txtQtaIns1
        txtQtaIns.Enabled = False
        txtSconto1Ins.Text = txtSconto1Ins1
        txtSconto1Ins.Enabled = False
        txtUMIns.Text = txtUMIns1
        txtUMIns.Enabled = False
        LblPrezzoNetto.Text = LblPrezzoNetto1
        LblImportoRiga.Text = LblImportoRiga1
        lblGiacenza.Text = lblGiacenza1
        lblGiacImp.Text = lblGiacImp1
        lblOrdFor.Text = lblOrdFor1
        lblLabelQtaRe.Text = lblLabelQtaRe1
        lblDataArr.Text = lblDataArr1
    End Sub

    Public Sub setValoriRigaDett(ByVal txtCodArtIns1 As String, ByVal txtDesArtIns1 As String, ByVal txtIVAIns1 As String, ByVal txtPrezzoIns1 As String, ByVal txtQtaEv1 As String, ByVal txtQtaIns1 As String, ByVal txtSconto1Ins1 As String, ByVal txtUMIns1 As String, ByVal LblPrezzoNetto1 As String, ByVal LblImportoRiga1 As String, ByVal lblGiacenza1 As String, ByVal lblGiacImp1 As String, ByVal lblOrdFor1 As String, ByVal lblDataArr1 As String, ByVal NRiga As Int32, ByVal lblLabelQtaRe1 As String)

        Session("NRiga") = ""
        Session(COD_ARTICOLO) = ""

        Session(COD_ARTICOLO) = txtCodArtIns1
        Session("NRiga") = NRiga

        Session("NSerieChange") = ""
        Session("QtaEvasa") = ""
        Session("QtaEvasa") = txtQtaEv1

        txtCodArtIns.Text = txtCodArtIns1
        txtCodArtIns.Enabled = False
        txtDesArtIns.Text = txtDesArtIns1
        txtDesArtIns.Enabled = False
        txtIVAIns.Text = txtIVAIns1
        txtIVAIns.Enabled = False
        txtPrezzoIns.Text = txtPrezzoIns1
        txtPrezzoIns.Enabled = False
        txtQtaEv.Text = txtQtaEv1
        txtQtaEv.Enabled = False
        txtQtaIns.Text = txtQtaIns1
        txtQtaIns.Enabled = False
        txtSconto1Ins.Text = txtSconto1Ins1
        txtSconto1Ins.Enabled = False
        txtUMIns.Text = txtUMIns1
        txtUMIns.Enabled = False
        LblPrezzoNetto.Text = LblPrezzoNetto1
        LblImportoRiga.Text = LblImportoRiga1
        lblGiacenza.Text = lblGiacenza1
        lblGiacImp.Text = lblGiacImp1
        lblOrdFor.Text = lblOrdFor1
        lblLabelQtaRe.Text = lblLabelQtaRe1
        lblDataArr.Text = lblDataArr1
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        WUCLottiInsert.WucElement = Me
        Session("TipoScansioneSL") = "NSerie"
        If rbtnNLotto.Checked Then
            Session("TipoScansioneSL") = "NLotto"
        ElseIf rbtnNSerie.Checked Then
            Session("TipoScansioneSL") = "NSerie"
        Else
            Session("TipoScansioneSL") = "NSerieNLotto"
        End If
    End Sub

    Private Sub rbtnTipoLettura_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbtnModNLotto.CheckedChanged, _
        rbtnModNSerie.CheckedChanged, rbtnNLotto.CheckedChanged, rbtnNSerie.CheckedChanged, rbtnNSerieLotto.CheckedChanged

        rbtnModNLotto.Font.Bold = False
        rbtnModNSerie.Font.Bold = False
        rbtnNLotto.Font.Bold = False
        rbtnNSerie.Font.Bold = False
        rbtnNSerieLotto.Font.Bold = False
        '-
        rbtnModNLotto.ForeColor = Drawing.Color.Black
        rbtnModNSerie.ForeColor = Drawing.Color.Black
        rbtnNLotto.ForeColor = Drawing.Color.Black
        rbtnNSerie.ForeColor = Drawing.Color.Black
        rbtnNSerieLotto.ForeColor = Drawing.Color.Black
        If sender.Checked Then
            sender.Font.Bold = True
            sender.ForeColor = Drawing.Color.Blue
        End If
        Session("TipoScansioneSL") = "NSerie"
        If rbtnNLotto.Checked Then
            Session("TipoScansioneSL") = "NLotto"
        ElseIf rbtnNSerie.Checked Then
            Session("TipoScansioneSL") = "NSerie"
        Else
            Session("TipoScansioneSL") = "NSerieNLotto"
        End If
        '-
        Dim valore As String = Session("TipoScansioneSL")
        Dim myQta As String = txtQtaEv.Text.Trim
        If IsNothing(myQta) Then
            myQta = "0"
        End If
        If String.IsNullOrEmpty(myQta) Then
            myQta = "0"
        ElseIf Not IsNumeric(myQta) Then
            myQta = "0"
        End If
        Dim TotQtaColli As Integer = WUCLottiInsert.GetQtaColli()
        Dim totArt As Integer = CInt(myQta) - TotQtaColli
        If totArt < 1 Then totArt = 0
        '-
        Dim myQtaRic As String = totArt.ToString.Trim
        If valore = "NLotto" Then
            valore = "NLotto"
        ElseIf valore = "NSerie" Then
            valore = "NSerie"
            If totArt > 1 Then myQtaRic = "1"
        ElseIf valore = "NSerieNLotto" Then
            valore = "NSerieNLotto"
            If totArt > 1 Then myQtaRic = "1"
        End If
        WUCLottiInsert.settxtNCollo(myQtaRic)
        '---
        If totArt = 0 Then
            SetLblMessUtenteRED("Sono già stati inseriti N°Serie/Lotti sufficienti!.<br>Totale N°Colli: RICHIESTI: " + (TotQtaColli - totArt).ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
        Else
            SetLblMessUtente("Totale N°Colli: RICHIESTI: " + (TotQtaColli - totArt).ToString.Trim + " - ASSEGNATI: " + TotQtaColli.ToString.Trim + ".")
        End If
        '------------
    End Sub
End Class