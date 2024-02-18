Option Compare Text

Imports SoftAziOnLine.Def

Public Class WebFormUtility
    'giu280614 costanti per la gestione
    Public Const SWOP As String = "SWOp" 'Nuovo/Modifica/elimina etc etc GESTIONE DOCUMENTI
    Public Const SWOPLOC As String = "SWOpLoc" 'Nuovo/Modifica/elimina etc etc GESTIONE "Tabelle" CHIAMATA 
    'GIU220113
    Public Const SWOPCLI As String = "SWOpCli" 'Nuovo/Modifica/elimina etc etc GESTIONE DOCUMENTI
    '---------
    Public Const SWOPNESSUNA As String = ""
    Public Const SWOPNUOVO As String = "Nuovo"
    Public Const SWOPNUOVONUMDOC As String = "NuovoNumDoc" 'giu230312 SI PRENDE UN NUOVO NUMERO ELSE RECUPERA
    Public Const SWOPMODIFICA As String = "Modifica"
    Public Const SWOPAGGIORNA As String = "Aggiorna"
    Public Const SWOPELIMINA As String = "Elimina"
    Public Const SWOPANNULLA As String = "Annulla"
    Public Const SWOPSTAMPA As String = "Stampa"
    'Se modifico una riga di dettaglio articoli dal grid
    Public Const SWOPDETTDOC As String = "SWDettaglioDocumento"
    'Se modifico i lotti nel pannello senza aver aggiornato
    Public Const SWOPDETTDOCL As String = "SWDettaglioDocumentoL"
    'Se modifico la riga nel pannello senza aver aggiornato
    Public Const SWOPDETTDOCR As String = "SWDettaglioDocumentoR"
    'Se sono in modifica SCADENZE / ATTIVITA
    'Se modifico la riga nel pannello senza aver aggiornato
    Public Const SWOPMODSCATT As String = "SWModificaScadAttivita"
    ' ''Public Const SWOPAVANZATE As String = "Avanzate"
    ' ''Public Const SWOPDUPLICA As String = "Duplica"
    Public Const SWMODIFICATO As String = "SWModificato" 'Se ho modificato almeno un campo
    Public Const SWSI As String = "Si"
    Public Const SWNO As String = "No"
    Public Const SWERRORI_AGGIORNAMENTO As String = "ErroriAggiornamento" 'giu280614 costanti per la gestione
    '-
    Public Const TB0 As Integer = 0
    Public Const TB1 As Integer = 1
    Public Const TB2 As Integer = 2
    Public Const TB3 As Integer = 3
    Public Const TB4 As Integer = 4
    Public Const TB5 As Integer = 5
    Public Const TB6 As Integer = 6
    Public Const TB7 As Integer = 7
    Public Const TB8 As Integer = 8
    Public Const TB9 As Integer = 9


#Region "Gestione selezione item su DropDownList"
    'giu200112 Optional VALORE 0 VALIDO (REGIME IVA NORMALE)
    Public Shared Sub PosizionaItemDDLTxt(ByRef txt As TextBox, ByRef ddl As DropDownList, Optional ByVal SW0 As Boolean = False)
        Try
            If SW0 = False Then
                If txt.Text.Trim = "" Or txt.Text.Trim = "0" Then
                    txt.BackColor = SEGNALA_OK
                    ddl.BackColor = SEGNALA_OK
                    ddl.SelectedIndex = 0
                    Exit Sub
                End If
            Else
                If txt.Text.Trim = "" Then
                    txt.BackColor = SEGNALA_OK
                    ddl.BackColor = SEGNALA_OK
                    ddl.SelectedIndex = 0
                    Exit Sub
                End If
            End If

            If ddl.Items.Count = 1 Then 'giu140911 aggiunto solo se è la prima volta
                ddl.Items.Clear()
                ddl.Items.Add("")
                ddl.DataBind()
            End If
            If ddl.Items.Count > 1 Then
                Try
                    '''ddl.SelectedValue = txt.Text.Trim
                    '''txt.Text = ddl.SelectedValue
                    '''txt.BackColor = SEGNALA_OK
                    '''ddl.BackColor = SEGNALA_OK
                    'giu261023
                    Dim item As ListItem = ddl.Items.FindByValue(txt.Text.Trim)
                    If (Not item Is Nothing) Then
                        ddl.SelectedValue = txt.Text.Trim
                        txt.BackColor = SEGNALA_OK
                        ddl.BackColor = SEGNALA_OK
                    Else
                        txt.BackColor = SEGNALA_KO
                        ddl.BackColor = SEGNALA_KO
                        ddl.SelectedIndex = 0
                    End If
                Catch
                    txt.BackColor = SEGNALA_KO
                    ddl.BackColor = SEGNALA_KO
                    ddl.SelectedIndex = 0
                End Try
            Else
                txt.BackColor = SEGNALA_KO
                ddl.BackColor = SEGNALA_OK
                ddl.SelectedIndex = 0
            End If

            ' ''NON FUNGE If ddl.Items.Count > 0 Then
            ' ''    Dim item As ListItem = ddl.Items.FindByValue(txt.Text)
            ' ''    If (Not item Is Nothing) Then
            ' ''        txt.BackColor = SEGNALA_OK
            ' ''        ddl.BackColor = SEGNALA_OK
            ' ''        ddl.SelectedIndex = ddl.Items.IndexOf(item)

            ' ''    Else
            ' ''        txt.BackColor = SEGNALA_KO
            ' ''        ddl.BackColor = SEGNALA_KO
            ' ''        ddl.SelectedIndex = 0
            ' ''    End If
            ' ''End If
        Catch ex As Exception

        End Try
        
    End Sub
    Public Shared Sub PosizionaItemDDL(ByVal valore As String, ByRef ddl As DropDownList, Optional ByVal SW0 As Boolean = False)
        Try
            If SW0 = False Then
                If valore.Trim = "" Or valore.Trim = "0" Then
                    ddl.BackColor = SEGNALA_OK
                    ddl.SelectedIndex = 0
                    Exit Sub
                End If
            Else
                If valore.Trim = "" Then
                    ddl.BackColor = SEGNALA_OK
                    ddl.SelectedIndex = 0
                    Exit Sub
                End If
            End If

            If ddl.Items.Count = 1 Then 'giu140911 aggiunto solo se è la prima volta
                ddl.Items.Clear()
                ddl.Items.Add("")
                ddl.DataBind()
            End If
            If ddl.Items.Count > 1 Then
                Try
                    'giu261023
                    '''ddl.SelectedValue = valore.Trim
                    '''ddl.BackColor = SEGNALA_OK
                    Dim item As ListItem = ddl.Items.FindByValue(valore)
                    If (Not item Is Nothing) Then
                        ddl.SelectedValue = valore.Trim
                        ddl.BackColor = SEGNALA_OK
                    Else
                        ddl.BackColor = SEGNALA_KO
                        ddl.SelectedIndex = 0
                    End If
                Catch
                    ddl.BackColor = SEGNALA_KO
                    ddl.SelectedIndex = 0
                End Try
            Else
                ddl.BackColor = SEGNALA_OK
                ddl.SelectedIndex = 0
            End If
        Catch ex As Exception
            ddl.BackColor = SEGNALA_KO
        End Try
        
        ' ''NON FUNGE If ddl.Items.Count > 0 Then
        ' ''    Dim item As ListItem = ddl.Items.FindByValue(valore)
        ' ''    If (Not item Is Nothing) Then
        ' ''        ddl.BackColor = SEGNALA_OK
        ' ''        ddl.SelectedIndex = ddl.Items.IndexOf(item)
        ' ''    Else
        ' ''        ddl.BackColor = SEGNALA_KO
        ' ''        ddl.SelectedIndex = 0
        ' ''    End If
        ' ''End If
    End Sub
    Public Shared Sub PosizionaItemDDLByTxt(ByRef Txt As TextBox, ByRef ddl As DropDownList, Optional ByVal SW0 As Boolean = False)
        Try
            If SW0 = False Then
                If Txt.Text.Trim = "" Or Txt.Text.Trim = "0" Then
                    Txt.BackColor = SEGNALA_OK
                    ddl.BackColor = SEGNALA_OK
                    ddl.SelectedIndex = 0
                    Exit Sub
                End If
            Else
                If Txt.Text.Trim = "" Then
                    Txt.BackColor = SEGNALA_OK
                    ddl.BackColor = SEGNALA_OK
                    ddl.SelectedIndex = 0
                    Exit Sub
                End If
            End If

            If ddl.Items.Count = 1 Then 'giu140911 aggiunto solo se è la prima volta
                ddl.Items.Clear()
                ddl.Items.Add("")
                ddl.DataBind()
            End If
            If ddl.Items.Count > 1 Then
                Try
                    '''ddl.SelectedValue = Txt.Text.Trim
                    '''Txt.BackColor = SEGNALA_OK
                    '''ddl.BackColor = SEGNALA_OK
                    'giu261023
                    Dim item As ListItem = ddl.Items.FindByValue(Txt.Text.Trim)
                    If (Not item Is Nothing) Then
                        ddl.SelectedValue = Txt.Text.Trim
                        Txt.BackColor = SEGNALA_OK
                        ddl.BackColor = SEGNALA_OK
                    Else
                        Txt.BackColor = SEGNALA_KO
                        ddl.BackColor = SEGNALA_KO
                        ddl.SelectedIndex = 0
                    End If
                Catch
                    Txt.BackColor = SEGNALA_KO
                    ddl.BackColor = SEGNALA_KO
                    ddl.SelectedIndex = 0
                End Try
            Else
                Txt.BackColor = SEGNALA_KO
            End If
        Catch ex As Exception

        End Try
        
    End Sub
#End Region

#Region "Controllo formato dati su TextBox (di tipo numerico) da scrivere su campo tabella"

    Public Shared Function ControllaAssegnaValoreDecimal(ByRef campo As Object, ByRef txt As TextBox) As Boolean
        Dim valore As String = txt.Text
        If (String.IsNullOrEmpty(valore)) Then
            valore = "0"
        End If
        If (IsNumeric(valore)) Then
            campo = Decimal.Parse(valore)
            ControllaAssegnaValoreDecimal = True
            txt.BackColor = SEGNALA_OK
        Else
            txt.BackColor = SEGNALA_KO
            ControllaAssegnaValoreDecimal = False
        End If
    End Function

    Public Shared Function ControllaAssegnaValoreInteger(ByRef campo As Object, ByVal txt As TextBox) As Boolean
        Dim valore As String = txt.Text
        If (String.IsNullOrEmpty(valore)) Then
            valore = "0"
        End If
        If (IsNumeric(valore)) Then
            campo = Integer.Parse(valore)
            ControllaAssegnaValoreInteger = True
            txt.BackColor = SEGNALA_OK
        Else
            ControllaAssegnaValoreInteger = False
            txt.BackColor = SEGNALA_KO
        End If
    End Function

#End Region

End Class