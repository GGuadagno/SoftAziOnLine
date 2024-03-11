Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_DocCollegati
    Inherits System.Web.UI.UserControl

    'giu260122
    Private StrErrore As String = ""
    Private TipoDoc As String = "" : Dim TabCliFor As String = ""
    Private SWSconti As Boolean = False
    Private myID As String = ""
    Private myDataDoc As String = ""

    Private _WucElement As Object

    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        WUCDocCollegati.WucElement = Me
    End Sub
    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        'If (WUCDocCollegati.PopolaLista) Then
        Session(F_DOCCOLL_APERTA) = False
        lblMessUtente.Text = ""
        ProgrammaticModalPopup.Hide()
        _WucElement.CallBackWFPDocCollegati()
        'Else
        '    lblMessUtente.Text = "Attenzione, nessuna riga è stata selezionata."
        'End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(F_DOCCOLL_APERTA) = False
        lblMessUtente.Text = ""
        ProgrammaticModalPopup.Hide()
        _WucElement.CancBackWFPDocCollegati()
    End Sub

    Public Sub Show(Optional ByVal PrimaVolta As Boolean = False)
        'If PrimaVolta Then
        '    lblMessUtente.Text = "Seleziona/modifica Quantità articoli resi"
        '    WUCDocCollegati.SelAndQtaTutti()
        'End If
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SetLblMessUtente(ByVal valore As String)
        lblMessUtente.Text = valore
    End Sub

    Public Sub PopolaGrigliaWUCDocCollegati()
        WUCDocCollegati.PopolaGriglia()
    End Sub

    'GIU270721
    Public Sub PopolaGrigliaWUCDocCollegatiCM()
        WUCDocCollegati.PopolaGrigliaCM()
    End Sub

    
    Protected Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        myID = Session(IDDOCUMCOLL)
        If IsNothing(myID) Then
            myID = ""
            lblMessUtente.Text = "Attenzione, Nessun documento selezionato"
            Exit Sub
        End If
        If String.IsNullOrEmpty(myID) Then
            myID = ""
            lblMessUtente.Text = "Attenzione, Nessun documento selezionato"
            Exit Sub
        End If
        If Not IsNumeric(myID) Then
            lblMessUtente.Text = "Attenzione, Nessun documento selezionato"
            Exit Sub
        End If
        '-
        TipoDoc = Session(TIPODOCCOLL)
        If IsNothing(TipoDoc) Then
            TipoDoc = ""
            lblMessUtente.Text = "Attenzione, Nessun documento selezionato (Tipo Doc.)"
            Exit Sub
        End If
        If String.IsNullOrEmpty(TipoDoc) Then
            TipoDoc = ""
            lblMessUtente.Text = "Attenzione, Nessun documento selezionato (Tipo Doc.)"
            Exit Sub
        End If
        '-
        myDataDoc = Session(DATADOCCOLL)
        If IsNothing(myDataDoc) Then
            myDataDoc = ""
            lblMessUtente.Text = "Attenzione, Nessun documento selezionato (Data)"
            Exit Sub
        End If
        If String.IsNullOrEmpty(myDataDoc) Then
            myDataDoc = ""
            lblMessUtente.Text = "Attenzione, Nessun documento selezionato (Data)"
            Exit Sub
        End If
        If Not IsDate(myDataDoc) Then
            lblMessUtente.Text = "Attenzione, Nessun documento selezionato (Data)"
            Exit Sub
        End If
        '---------
        'giu260122
        If TipoDoc = "CA" Then
            Call OKStampaContratti()
        Else
            Call OKStampaDocumenti()
        End If

    End Sub
    'giu260122
    Private Sub OKStampaDocumenti()
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaDocumento(myID, TipoDoc, Session(CSTCODDITTA).ToString.Trim, CDate(myDataDoc).Year.ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
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
                Attesa.ShowStampaAll2("Stampa documento", "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCR.aspx?labelForm=Stampa documento")
            Else
                lblMessUtente.Text = "Errore, " & StrErrore.Trim
            End If
        Catch ex As Exception
            lblMessUtente.Text = "Errore, " & ex.Message.Trim
        End Try
    End Sub
    '-
    Private Sub OKStampaContratti()
        If Not IsNumeric(myID.Trim) Then
            myID = ""
            Exit Sub
        Else
            Dim myIDLong As Long
            myIDLong = CLng(myID.Trim)
            If myIDLong < 0 Then
                myIDLong = myIDLong * -1
            End If
            myID = CStr(myIDLong)
        End If
        Dim DsPrinWebDoc As New DSPrintWeb_Documenti
        Dim ObjReport As New Object
        Dim ClsPrint As New Documenti
        SWSconti = False
        Try
            DsPrinWebDoc.Clear() 'GIU020512
            If ClsPrint.StampaContratto(myID, TipoDoc, Session(CSTCODDITTA).ToString.Trim, Session(ESERCIZIO).ToString.Trim, TabCliFor, DsPrinWebDoc, ObjReport, SWSconti, StrErrore) Then
                Session(CSTObjReport) = ObjReport
                Session(CSTDsPrinWebDoc) = DsPrinWebDoc
                If SWSconti = True Then
                    Session(CSTSWScontiDoc) = 1
                Else
                    Session(CSTSWScontiDoc) = 0
                End If
                Session(CSTSWConfermaDoc) = 0
                Session(ATTESA_CALLBACK_METHOD) = ""
                Session(CSTNOBACK) = 1
                Session(CSTTASTOST) = "btnStampa"
                'GIU200423 NON SERVE E POI SE MODIF PROVOCA ERR IN GEST.DOC. Session(CSTTIPODOC) = Session(TIPODOCCOLL)
                Attesa.ShowStampaAll2("Stampa Contratto", "Richiesta dell'apertura di una nuova pagina per la stampa.", Attesa.TYPE_CONFIRM, "..\WebFormTables\WF_PrintWebCA.aspx?labelForm=Stampa Contratto - " + myID.Trim)
            Else
                lblMessUtente.Text = "Errore, " & StrErrore.Trim
            End If
        Catch ex As Exception
            lblMessUtente.Text = "Errore, " & ex.Message.Trim
        End Try
        'Call OKApriStampa(DsPrinWebDoc)
    End Sub
End Class