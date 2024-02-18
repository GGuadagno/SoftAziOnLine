Imports SoftAziOnLine.Def

Partial Public Class WUC_Attesa
    Inherits System.Web.UI.UserControl

    Public Const TYPE_ALERT As Integer = 1
    Public Const TYPE_CONFIRM As Integer = 2
    Public Const TYPE_INFO As Integer = 3
    Public Const TYPE_ERROR As Integer = 4
    Public Const TYPE_GETVALUE As Integer = 5
    Public Const TYPE_INFO_ATTESA As Integer = 6
    Public Const TYPE_STAMPA As Integer = 7

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property
    
    Protected Sub OkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ' ''Session(ATTESA_CALLBACK_METHOD) = ""
        ' ''Session(CSTNOBACK) = 0
        ' ''ProgrammaticModalPopup.Hide()
        '-- giu160312
        ProgrammaticModalPopup.Hide()
        If String.IsNullOrEmpty(Session(MODALPOPUP_TYPE)) Then
            Session(MODALPOPUP_TYPE) = TYPE_CONFIRM
        End If
        If (Session(MODALPOPUP_TYPE).Equals(TYPE_CONFIRM) And Not String.IsNullOrEmpty(Session(ATTESA_CALLBACK_METHOD))) Then
            Dim MethodObj As System.Reflection.MethodInfo
            MethodObj = _WucElement.GetType().GetMethod(Session(ATTESA_CALLBACK_METHOD))
            Session(ATTESA_CALLBACK_METHOD) = String.Empty
            Session(CSTNOBACK) = 0
            MethodObj.Invoke(_WucElement, Nothing)
        End If
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Session(ATTESA_CALLBACK_METHOD) = ""
        Session(CSTNOBACK) = 0
        ProgrammaticModalPopup.Hide()
    End Sub

    Public Sub Hide()
        Session(ATTESA_CALLBACK_METHOD) = ""
        Session(CSTNOBACK) = 0
        ProgrammaticModalPopup.Hide()
    End Sub

    Public Sub ShowAttesa()
        Show("Info", MSGAttesa, TYPE_INFO_ATTESA)
    End Sub

    Public Sub ShowStampa(ByVal title As String, ByVal message As String, ByVal type As Integer, ByVal LnkName As String)

        LnkStampa.HRef = LnkName

        Session(MODALPOPUP_TYPE) = type
        
        LabelTitle.Text = title
        LabelMessage.Text = message

        OkButton.Visible = True
        CancelButton.Visible = True

        OkButton.Text = "Prossima Fattura"
        CancelButton.Text = "Annulla operazione"

        If type = TYPE_INFO_ATTESA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_STAMPA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_ALERT Then ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
        If type = TYPE_CONFIRM Then ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
        If type = TYPE_INFO Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        If type = TYPE_ERROR Then ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
        If type = TYPE_GETVALUE Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        ProgrammaticModalPopup.Show()

    End Sub
    Public Sub ShowStampaAll1(ByVal title As String, ByVal message As String, ByVal type As Integer, ByVal LnkName As String)

        LnkStampa.HRef = LnkName

        Session(MODALPOPUP_TYPE) = type

        LabelTitle.Text = title
        LabelMessage.Text = message

        OkButton.Visible = True
        CancelButton.Visible = True

        OkButton.Text = "Stampa fatture NO Sconti"
        CancelButton.Text = "Annulla operazione"

        If type = TYPE_INFO_ATTESA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_STAMPA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_ALERT Then ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
        If type = TYPE_CONFIRM Then ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
        If type = TYPE_INFO Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        If type = TYPE_ERROR Then ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
        If type = TYPE_GETVALUE Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        ProgrammaticModalPopup.Show()

    End Sub
    Public Sub ShowStampaAll2(ByVal title As String, ByVal message As String, ByVal type As Integer, ByVal LnkName As String)

        LnkStampa.HRef = LnkName

        Session(MODALPOPUP_TYPE) = type

        LabelTitle.Text = title
        LabelMessage.Text = message

        OkButton.Visible = True
        CancelButton.Visible = True
        CancelButton.Enabled = False
        OkButton.Text = "Termina operazione"
        CancelButton.Text = "Annulla operazione"

        If type = TYPE_INFO_ATTESA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_STAMPA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_ALERT Then ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
        If type = TYPE_CONFIRM Then ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
        If type = TYPE_INFO Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        If type = TYPE_ERROR Then ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
        If type = TYPE_GETVALUE Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        ProgrammaticModalPopup.Show()

    End Sub

    Public Sub ShowStampaAll(ByVal title As String, ByVal message As String, ByVal type As Integer, ByVal LnkName As String)

        LnkStampa.HRef = LnkName

        Session(MODALPOPUP_TYPE) = type

        LabelTitle.Text = title
        LabelMessage.Text = message

        OkButton.Visible = True
        CancelButton.Visible = True
        CancelButton.Enabled = False
        OkButton.Text = "Termina operazione"
        CancelButton.Text = "Annulla operazione"

        If type = TYPE_INFO_ATTESA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_STAMPA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_ALERT Then ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
        If type = TYPE_CONFIRM Then ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
        If type = TYPE_INFO Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        If type = TYPE_ERROR Then ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
        If type = TYPE_GETVALUE Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        ProgrammaticModalPopup.Show()

    End Sub

    Public Sub ShowStampaMovMag(ByVal title As String, ByVal message As String, ByVal type As Integer, ByVal LnkName As String)

        LnkStampa.HRef = LnkName

        Session(MODALPOPUP_TYPE) = type

        LabelTitle.Text = title
        LabelMessage.Text = message

        OkButton.Visible = True
        CancelButton.Visible = True

        OkButton.Text = "Prossimo movimento"
        CancelButton.Text = "Annulla operazione"

        If type = TYPE_INFO_ATTESA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_STAMPA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_ALERT Then ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
        If type = TYPE_CONFIRM Then ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
        If type = TYPE_INFO Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        If type = TYPE_ERROR Then ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
        If type = TYPE_GETVALUE Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        ProgrammaticModalPopup.Show()

    End Sub

    'giu040512 stampo tutti i movimenti caricati prima
    Public Sub ShowStampaMovMagAll(ByVal title As String, ByVal message As String, ByVal type As Integer, ByVal LnkName As String)

        LnkStampa.HRef = LnkName

        Session(MODALPOPUP_TYPE) = type

        LabelTitle.Text = title
        LabelMessage.Text = message

        OkButton.Visible = False
        CancelButton.Visible = True

        OkButton.Text = "Ok"
        CancelButton.Text = "Ok"

        If type = TYPE_INFO_ATTESA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_STAMPA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_ALERT Then ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
        If type = TYPE_CONFIRM Then ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
        If type = TYPE_INFO Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        If type = TYPE_ERROR Then ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
        If type = TYPE_GETVALUE Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        ProgrammaticModalPopup.Show()

    End Sub

    Public Sub ShowCFPivaDoppi(ByVal title As String, ByVal message As String, ByVal type As Integer, ByVal LnkName As String)

        LnkStampa.HRef = LnkName

        Session(MODALPOPUP_TYPE) = type

        Session("TipoStampa") = 7
        Session(CSTFinestraChiamante) = "Clienti"

        LabelTitle.Text = title
        LabelMessage.Text = message

        OkButton.Visible = False
        CancelButton.Visible = True

        OkButton.Text = "Visualizza"
        CancelButton.Text = "Chiudi"

        If type = TYPE_INFO_ATTESA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_STAMPA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_ALERT Then ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
        If type = TYPE_CONFIRM Then ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
        If type = TYPE_INFO Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        If type = TYPE_ERROR Then ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
        If type = TYPE_GETVALUE Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        ProgrammaticModalPopup.Show()

    End Sub

    Public Sub Show(ByVal title As String, ByVal message As String, ByVal type As Integer, Optional ByVal listName As String = "")
        Session(MODALPOPUP_TYPE) = type

        LabelTitle.Text = title
        LabelMessage.Text = message

        OkButton.Visible = True
        CancelButton.Visible = True

        OkButton.Text = "Ok"
        CancelButton.Text = "Annulla"

        If type = TYPE_INFO_ATTESA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_STAMPA Then ImageIcon.ImageUrl = "~/Immagini/Icone/aggiorna1.jpg"
        If type = TYPE_ALERT Then ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
        If type = TYPE_CONFIRM Then ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
        If type = TYPE_INFO Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        If type = TYPE_ERROR Then ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
        If type = TYPE_GETVALUE Then ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
        ProgrammaticModalPopup.Show()

        Dim strCall As String = Session(ATTESA_CALLBACK_METHOD)
        If IsNothing(strCall) Then
            strCall = ""
        ElseIf String.IsNullOrEmpty(strCall) Then
            strCall = ""
        End If
        If strCall.Trim <> "" Then
            Dim MethodObj As System.Reflection.MethodInfo
            MethodObj = _WucElement.GetType().GetMethod(Session(ATTESA_CALLBACK_METHOD))
            Session(ATTESA_CALLBACK_METHOD) = String.Empty
            MethodObj.Invoke(_WucElement, Nothing)
        End If

    End Sub

End Class