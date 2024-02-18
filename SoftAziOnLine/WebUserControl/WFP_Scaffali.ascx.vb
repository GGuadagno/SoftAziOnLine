Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility

Partial Public Class WFP_Scaffali
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
    
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            Session(IDMAGAZZINO) = 1 'fisso
            Session("IDReparto") = 0 'solo la prima volta
            lblMessUtente.Text = ""
        End If
        WUC_Scaffali.WucElement = Me
    End Sub

    Protected Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session(SWOPLOC) = SWOPNUOVO Then
            If CheckNewNumeroOnTab(CheckNewNumeroOnTab(WUC_Scaffali.GetNewCodice)) = False Then
                Exit Sub
            End If
        End If
        Session(SWOPLOC) = SWOPNESSUNA
        If (WUC_Scaffali.Aggiorna()) Then
            ProgrammaticModalPopup.Hide()
            _WucElement.CallBackWFPScaffali()
            Session(F_ANAGRSCAFFALI_APERTA) = False
        End If
    End Sub
    Private Function CheckNewNumeroOnTab(ByVal _ID As Int32) As Boolean
        If _ID = 0 Then Exit Function
        Dim myIDMag As String = Session(IDMAGAZZINO)
        If IsNothing(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        If String.IsNullOrEmpty(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        '-- Reparto??? 
        Dim myIDRep As String = Session(IDREPARTO)
        If IsNothing(myIDRep) Then
            lblMessUtente.Text = "Attenzione, Selezionare il Reparto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myIDRep) Then
            lblMessUtente.Text = "Attenzione, Selezionare il Reparto"
            Exit Function
        End If
        Dim strSQL As String = "Select Scaffale From Scaffali WHERE Magazzino=" & myIDMag.Trim
        strSQL += " AND Reparto=" & myIDRep.Trim & " AND Scaffale=" & _ID.ToString.Trim
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    'Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    'ModalPopup.Show("Attenzione", "Numero documento già presente in tabella", WUC_ModalPopup.TYPE_ALERT)
                    lblMessUtente.Text = "Attenzione, Codice già presente in tabella"
                    Exit Function
                Else
                    CheckNewNumeroOnTab = True
                    Exit Function
                End If
            Else
                CheckNewNumeroOnTab = True
                Exit Function
            End If
        Catch Ex As Exception
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            'ModalPopup.Show("Errore in Documenti.CheckNewNumeroOnTab", "Verifica N.Documento da impegnare: " & Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            lblMessUtente.Text = "Errore, Verifica codice da impegnare: " & Ex.Message
            Exit Function
        End Try
    End Function

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNESSUNA
        ProgrammaticModalPopup.Hide()
        WUC_Scaffali.SvuotaCampi()
        _WucElement.CancBackWFPScaffali()
        Session(F_ANAGRSCAFFALI_APERTA) = False
    End Sub
    Protected Sub btnNuovo_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session(SWOPLOC) = SWOPNUOVO
        lblMessUtente.Text = "Inserimento nuovo elemento in tabella"
        WUC_Scaffali.SetNewCodice(GetNewCodice)
    End Sub
    Private Function GetNewCodice() As Long
        Dim myIDMag As String = Session(IDMAGAZZINO)
        If IsNothing(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        If String.IsNullOrEmpty(myIDMag) Then
            myIDMag = "0"
            Session(IDMAGAZZINO) = myIDMag 'fisso
        End If
        '-- Reparto??? 
        Dim myIDRep As String = Session(IDREPARTO)
        If IsNothing(myIDRep) Then
            GetNewCodice = -1
            lblMessUtente.Text = "Attenzione, Selezionare il Reparto"
            Exit Function
        End If
        If String.IsNullOrEmpty(myIDRep) Then
            GetNewCodice = -1
            lblMessUtente.Text = "Attenzione, Selezionare il Reparto"
            Exit Function
        End If
        Dim strSQL As String = "Select MAX(ISNULL(Scaffale,0)) AS Codice From Scaffali "
        strSQL += "WHERE Magazzino=" & myIDMag.Trim & " AND Reparto=" & myIDRep.Trim
        '---
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Codice")) Then
                        GetNewCodice = ds.Tables(0).Rows(0).Item("Codice") + 1
                    Else
                        GetNewCodice = 1
                    End If
                    Exit Function
                Else
                    GetNewCodice = 1
                    Exit Function
                End If
            Else
                GetNewCodice = 1
                Exit Function
            End If
        Catch Ex As Exception
            GetNewCodice = -1
            Exit Function
        End Try

    End Function

    Public Sub Show()
        ProgrammaticModalPopup.Show()
    End Sub

    Public Sub SvuotaCampi()
        WUC_Scaffali.SvuotaCampi()
    End Sub

    Public Sub SetlblMessaggi(ByVal strMessaggio As String)
        lblMessUtente.Text = strMessaggio
    End Sub
End Class