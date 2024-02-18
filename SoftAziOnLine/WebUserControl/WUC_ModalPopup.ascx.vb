Imports SoftAziOnLine.Def

Partial Public Class WUC_ModalPopup
    Inherits System.Web.UI.UserControl

    Public Const TYPE_ALERT As Integer = 1
    Public Const TYPE_CONFIRM As Integer = 2
    Public Const TYPE_INFO As Integer = 3
    Public Const TYPE_ERROR As Integer = 4
    Public Const TYPE_GETVALUE As Integer = 5
    'giu150312
    Public Const TYPE_CONFIRM_YN As Integer = 6
    Public Const TYPE_CONFIRM_Y As Integer = 7
    Public Const TYPE_CONFIRM_YNA As Integer = 8

    Private _WucElement As Object
    Property WucElement() As Object
        Get
            Return _WucElement
        End Get
        Set(ByVal value As Object)
            _WucElement = value
        End Set
    End Property

    Private _Valore As String
    Property Valore() As String
        Get
            Return txtValore.Text
        End Get
        Set(ByVal value As String)
            _Valore = value
        End Set
    End Property
    'giu260122
    Private _Valore2 As String
    Property Valore2() As String
        Get
            Return txtValore2.Text
        End Get
        Set(ByVal value As String)
            _Valore2 = value
        End Set
    End Property

    Protected Sub OkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            ProgrammaticModalPopup.Hide()
            If String.IsNullOrEmpty(Session(MODALPOPUP_TYPE)) Then
                Session(MODALPOPUP_TYPE) = TYPE_CONFIRM
            End If
            'If (OkButton.Text.Equals("Conferma") Or OkButton.Text.Equals("Ok") Or OkButton.Text.Equals("Si") Or OkButton.Text.Equals("Clienti")) Then
            If (Session(MODALPOPUP_TYPE).Equals(TYPE_GETVALUE)) Then
                _WucElement.GetValue()
            ElseIf (Session(MODALPOPUP_TYPE).Equals(TYPE_CONFIRM) And Not String.IsNullOrEmpty(Session(MODALPOPUP_CALLBACK_METHOD))) Then
                Dim MethodObj As System.Reflection.MethodInfo
                MethodObj = _WucElement.GetType().GetMethod(Session(MODALPOPUP_CALLBACK_METHOD))
                Session(MODALPOPUP_CALLBACK_METHOD) = String.Empty
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                MethodObj.Invoke(_WucElement, Nothing)
                'GIU150312
            ElseIf (Session(MODALPOPUP_TYPE).Equals(TYPE_CONFIRM_YN) And Not String.IsNullOrEmpty(Session(MODALPOPUP_CALLBACK_METHOD))) Then
                Dim MethodObj As System.Reflection.MethodInfo
                MethodObj = _WucElement.GetType().GetMethod(Session(MODALPOPUP_CALLBACK_METHOD))
                Session(MODALPOPUP_CALLBACK_METHOD) = String.Empty
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                MethodObj.Invoke(_WucElement, Nothing)
            ElseIf (Session(MODALPOPUP_TYPE).Equals(TYPE_CONFIRM_YNA) And Not String.IsNullOrEmpty(Session(MODALPOPUP_CALLBACK_METHOD))) Then
                Dim MethodObj As System.Reflection.MethodInfo
                MethodObj = _WucElement.GetType().GetMethod(Session(MODALPOPUP_CALLBACK_METHOD))
                Session(MODALPOPUP_CALLBACK_METHOD) = String.Empty
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                MethodObj.Invoke(_WucElement, Nothing)
            ElseIf (Session(MODALPOPUP_TYPE).Equals(TYPE_CONFIRM_Y) And Not String.IsNullOrEmpty(Session(MODALPOPUP_CALLBACK_METHOD))) Then
                Dim MethodObj As System.Reflection.MethodInfo
                MethodObj = _WucElement.GetType().GetMethod(Session(MODALPOPUP_CALLBACK_METHOD))
                Session(MODALPOPUP_CALLBACK_METHOD) = String.Empty
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                MethodObj.Invoke(_WucElement, Nothing)
            End If
        Catch ex As Exception

        End Try
        
        'End If
    End Sub
    Protected Sub NoButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            ProgrammaticModalPopup.Hide()
            If String.IsNullOrEmpty(Session(MODALPOPUP_TYPE)) Then
                Session(MODALPOPUP_TYPE) = TYPE_CONFIRM
            End If
            'If (NoButton.Text.Equals("No") Or NoButton.Text.Equals("Annulla") Or NoButton.Text.Equals("Fornitori")) Then
            If (Session(MODALPOPUP_TYPE).Equals(TYPE_CONFIRM) And Not String.IsNullOrEmpty(Session(MODALPOPUP_CALLBACK_METHOD_NO))) Then
                Dim MethodObj As System.Reflection.MethodInfo
                MethodObj = _WucElement.GetType().GetMethod(Session(MODALPOPUP_CALLBACK_METHOD_NO))
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = String.Empty
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                MethodObj.Invoke(_WucElement, Nothing)
                'GIU150312
            ElseIf (Session(MODALPOPUP_TYPE).Equals(TYPE_CONFIRM_YN) And Not String.IsNullOrEmpty(Session(MODALPOPUP_CALLBACK_METHOD_NO))) Then
                Dim MethodObj As System.Reflection.MethodInfo
                MethodObj = _WucElement.GetType().GetMethod(Session(MODALPOPUP_CALLBACK_METHOD_NO))
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = String.Empty
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                MethodObj.Invoke(_WucElement, Nothing)
            ElseIf (Session(MODALPOPUP_TYPE).Equals(TYPE_CONFIRM_YNA) And Not String.IsNullOrEmpty(Session(MODALPOPUP_CALLBACK_METHOD_NO))) Then
                Dim MethodObj As System.Reflection.MethodInfo
                MethodObj = _WucElement.GetType().GetMethod(Session(MODALPOPUP_CALLBACK_METHOD_NO))
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = String.Empty
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                MethodObj.Invoke(_WucElement, Nothing)
            ElseIf (Session(MODALPOPUP_TYPE).Equals(TYPE_GETVALUE)) Then
                _WucElement.NoGetValue()
            End If
        Catch ex As Exception

        End Try
        
        'End If
    End Sub
    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        'qui non arriva MAI è legato al componente AJAX che ha l'effetto di CANCEL = TRUE o false
        ProgrammaticModalPopup.Hide()
        Session(MODALPOPUP_CALLBACK_METHOD_NO) = String.Empty
        Session(MODALPOPUP_CALLBACK_METHOD) = ""
        If (CancelButton.Text.Equals("No") Or CancelButton.Text.Equals("Annulla")) Then
        End If
    End Sub

    Public Sub Hide()
        ProgrammaticModalPopup.Hide()
    End Sub

    Public Sub Show(ByVal title As String, ByVal message As String, ByVal type As Integer, Optional ByVal listName As String = "", Optional ByVal message2 As String = "", Optional ByVal listName2 As String = "")
        Session(MODALPOPUP_TYPE) = type

        LabelTitle.Text = title
        'GIU240322
        LabelMessage.ForeColor = Drawing.Color.Black
        If Not String.IsNullOrEmpty(Session(LABELMESSAGERED)) Then
            If Session(LABELMESSAGERED).ToString.Trim <> "" Then
                Session(LABELMESSAGERED) = ""
                LabelMessage.ForeColor = Drawing.Color.Red
            End If
        End If
        '---------
        LabelMessage.Text = message
        LabelMessage2.Text = message2

        OkButton.Visible = False
        CancelButton.Visible = False
        '-
        OkButton.Enabled = True
        NoButton.Enabled = True
        NoButton.Visible = False
        CancelButton.Enabled = True

        OkButton.Text = "Ok"
        CancelButton.Text = "Annulla"

        _Valore = String.Empty
        txtValore.Text = String.Empty
        txtValore.Visible = False
        '-
        _Valore2 = String.Empty
        txtValore2.Text = String.Empty
        txtValore2.Visible = False
        LabelMessage2.Visible = False

        If (type = TYPE_ALERT) Then
            ImageIcon.ImageUrl = "~/Immagini/Icone/warning.png"
            CancelButton.Visible = True
            CancelButton.Text = "Ok"
        ElseIf (type = TYPE_CONFIRM) Or (type = TYPE_CONFIRM_YN) Or (type = TYPE_CONFIRM_Y) Or (type = TYPE_CONFIRM_YNA) Then
            ImageIcon.ImageUrl = "~/Immagini/Icone/question.gif"
            CancelButton.Visible = True
            OkButton.Visible = True
            OkButton.Text = "Si"
            CancelButton.Text = "No"

            'giu191211
            If Not String.IsNullOrEmpty(Session(MODALPOPUP_CALLBACK_METHOD_NO)) Then
                If Session(MODALPOPUP_CALLBACK_METHOD_NO) = "SWCercaFor" Then
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "SWCercaCli"
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = "SWCercaFor"
                    NoButton.Visible = True
                    OkButton.Text = "Clienti"
                    NoButton.Text = "Fornitori"
                    CancelButton.Text = "Annulla"
                ElseIf Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewDocRecuperaNum" Or _
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaDDTRecuperaNum" Then
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewDoc"
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewDocRecuperaNum"
                    NoButton.Visible = True
                    OkButton.Text = "Ok Nuovo numero"
                    NoButton.Text = "Recupera numero"
                    CancelButton.Text = "Annulla"
                ElseIf Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaFatturaRecuperaNum" Or _
                        Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewFCRecuperaNum" Or _
                        InStr(Session(MODALPOPUP_CALLBACK_METHOD_NO).ToString.Trim.ToUpper, "RECUPERANUM") > 0 Then 'GIU290419 Then 'giu120412 'GIU290419
                    NoButton.Visible = True
                    OkButton.Text = "Ok Nuovo numero"
                    NoButton.Text = "Recupera numero"
                    CancelButton.Text = "Annulla"
                ElseIf Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewNCRecuperaNum" Then ''giu150513
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "CreaNewNC"
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = "CreaNewNCRecuperaNum"
                    NoButton.Visible = True
                    OkButton.Text = "Ok Nuovo numero"
                    NoButton.Text = "Recupera numero"
                    CancelButton.Text = "Annulla"
                ElseIf Session(MODALPOPUP_CALLBACK_METHOD_NO) = "OKCreaResoAFornitoreDF" Then 'giu250612
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = "OKCreaResoAFornitoreSM"
                    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = "OKCreaResoAFornitoreDF"
                    NoButton.Visible = True
                    OkButton.Text = "Reso a Fornitore (SCARICO DI MAGAZZINO)"
                    NoButton.Text = "Reso a Fornitore (DOCUMENTO DI TRASPORTO)"
                    CancelButton.Text = "Annulla"
                ElseIf Session(MODALPOPUP_CALLBACK_METHOD_NO) = "AggiornaScadAI" Then 'giu230514
                    NoButton.Visible = True
                    OkButton.Text = "Aggiorna tutto"
                    NoButton.Text = "Aggiorna solo le date non valorizzate"
                    CancelButton.Text = "Annulla"
                ElseIf Session(MODALPOPUP_CALLBACK_METHOD_NO) = "FatturaAC" Then 'giu290419
                    NoButton.Visible = True
                    OkButton.Text = "Ordine selezionato"
                    NoButton.Text = "Nessun collegamento"
                    CancelButton.Text = "Annulla emissione"
                Else
                    NoButton.Visible = True
                    OkButton.Text = "Si"
                    NoButton.Text = "No"
                    CancelButton.Text = "Annulla"
                End If
                If (type = TYPE_CONFIRM_YN) Then
                    CancelButton.Text = "Annulla"
                    CancelButton.Enabled = False
                End If
            Else
                If (type = TYPE_CONFIRM_Y) Then
                    OkButton.Text = "Ok"
                    NoButton.Enabled = False
                    NoButton.Visible = False
                    CancelButton.Text = "Annulla"
                    CancelButton.Enabled = False
                End If
            End If
            '----------
        ElseIf (type = TYPE_INFO) Then
            ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
            CancelButton.Visible = True
            CancelButton.Text = "Ok"
        ElseIf (type = TYPE_ERROR) Then
            ImageIcon.ImageUrl = "~/Immagini/Icone/delete.png"
            CancelButton.Visible = True
            CancelButton.Text = "Ok"
        ElseIf (type = TYPE_GETVALUE) Then
            ImageIcon.ImageUrl = "~/Immagini/Icone/info.png"
            CancelButton.Visible = True
            OkButton.Visible = True
            txtValore.Visible = True
            txtValore.Width = Unit.Pixel(300)
            txtValore.Height = Unit.Pixel(60)
            txtValore.TextMode = TextBoxMode.MultiLine
            txtValore.ToolTip = listName.Trim
            'giu260123
            If message2.Trim <> "" Then
                LabelMessage2.Visible = True
                txtValore2.Visible = True
                txtValore2.Width = Unit.Pixel(300)
                txtValore2.Height = Unit.Pixel(30)
                txtValore2.TextMode = TextBoxMode.SingleLine
                txtValore2.Text = listName2.Trim
            End If
            '-
            OkButton.Height = Unit.Pixel(45)
            OkButton.Width = Unit.Pixel(105)
            OkButton.Text = "Conferma"
            CancelButton.Height = Unit.Pixel(45)
            CancelButton.Width = Unit.Pixel(105)
            CancelButton.Text = "Annulla"
        End If
        ProgrammaticModalPopup.Show()
    End Sub

End Class