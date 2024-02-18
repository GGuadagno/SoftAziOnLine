Option Explicit On 
Imports System.Data.SqlClient
Imports System.Windows.Forms


Public Class clsControlli

    Public Const ControllaDati_TipoIntero As Byte = 0
    Public Const ControllaDati_TipoData As Byte = 1
    Public Const ControllaDati_TipoOra As Byte = 2
    Public Const ControllaDati_TipoDecimale As Byte = 3
    Public Const ControllaDati_TipoAlfanumerico As Byte = 4
    Public Const ControllaDati_TipoTelefono As Byte = 5
    ' Questa funzione controlla se nella form sono presenti
    ' TextBox o ComboBox e setta il colore di fondo

    Public Sub ControlsBackcolor(ByVal Form1 As System.Windows.Forms.Form, _
                                    ByVal Abilitato As Boolean, _
                                    ByVal vbSfondo As System.Drawing.Color, _
                                    ByVal vbPronto As System.Drawing.Color)

        Dim Ind As Integer
        Dim MyObj As Object
        Dim MyBackColor As System.Drawing.Color

        If Abilitato Then
            MyBackColor = vbSfondo
        Else
            MyBackColor = vbPronto
        End If

        'Console.Write("Numero di controlli: " & Form1.Controls.Count & Chr(13))
        'Console.Write("Form name: " & Form1.Name & Chr(13))

        For Ind = 0 To Form1.Controls.Count - 1
            'Console.Write("Nome controllo: " & Form1.Controls.Item(Ind).Name & Chr(13))
            If TypeOf Form1.Controls.Item(Ind) Is GroupBox Then
                'Console.Write("E' un GroupBox" & Chr(13))
                For Each MyObj In Form1.Controls.Item(Ind).Controls
                    'Console.Write("Nome controllo: " & Myobj.Name & Chr(13))
                    If TypeOf MyObj Is TextBox Then
                        MyObj.BackColor = MyBackColor   ' Control.DefaultBackColor
                    ElseIf TypeOf MyObj Is ComboBox Then
                        MyObj.BackColor = MyBackColor   ' Control.DefaultBackColor
                    End If
                Next
            ElseIf TypeOf MyObj Is TextBox Then
                MyObj.BackColor = MyBackColor   ' Control.DefaultBackColor
            ElseIf TypeOf MyObj Is ComboBox Then
                MyObj.BackColor = MyBackColor   ' Control.DefaultBackColor
            End If
        Next

    End Sub

    ' Questa funzione controlla i caratteri ricevuti nell'evento
    ' KeyPress e convalida solo i numeri

    ' Sviluppata il 2 Aprile 2002

    Public Sub ControllaDati(ByVal Param As System.Windows.Forms.KeyPressEventArgs, _
                             ByVal Tipo As Byte)
        ' Tipo = 0 Intero
        ' Tipo = 1 Data

        Select Case Param.KeyChar
            Case "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", _
                    Chr(System.Windows.Forms.Keys.Cancel), _
                    Chr(System.Windows.Forms.Keys.Back)
            Case Else
                Select Case Tipo
                    Case ControllaDati_TipoIntero
                        Param.Handled = True

                    Case ControllaDati_TipoData
                        If Param.KeyChar <> "/" And Param.KeyChar <> "." And Param.KeyChar <> "-" Then
                            Param.Handled = True
                        End If

                    Case ControllaDati_TipoOra
                        If Param.KeyChar <> "." And _
                            Param.KeyChar <> System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator Then
                            Param.Handled = True
                        End If

                    Case Me.ControllaDati_TipoDecimale
                        Param.Handled = True
                        'Select Case Param.KeyChar
                        '    Case "."
                        '        Param.Handled = True
                        '    Case ","
                        '        'Car = Asc(",")
                        '        'If InStr(stringa, ",") > 0 Then
                        '        '    Car = 0
                        '        'End If
                        'End Select
                    Case Me.ControllaDati_TipoAlfanumerico
                        Select Case Param.KeyChar
                            Case "a" To "z", "A" To "Z"                            
                            Case "è", "é", "ò", "à", "ù", "ì"
                            Case ".", ":", ",", ";", "'", Chr(34)
                            Case "@", "(", ")", "_", "-", Chr(System.Windows.Forms.Keys.Space)
                                'Case Asc("!"), Asc("^"), Asc("_"), Asc("#")
                                'Case Asc("-"), Asc("+"), Asc("*"), Asc("/"), Asc("%"), Asc("£"), Asc("€")
                            Case "€"
                            Case Else
                                Param.Handled = True
                        End Select
                    Case Me.ControllaDati_TipoTelefono
                        Select Case Param.KeyChar
                            Case "/", "-", "_", "+", ".", _
                                 Chr(System.Windows.Forms.Keys.Space)
                            Case Else
                                Param.Handled = True
                        End Select
                End Select
        End Select
    End Sub

    Public Sub ControllaDatiKeyDown(ByRef Param As System.Windows.Forms.KeyEventArgs, _
                             ByVal Tipo As Byte)
        ' Tipo = 0 Intero
        ' Tipo = 1 Data

        Select Case Chr(Param.KeyCode)
            Case "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", _
                    Chr(System.Windows.Forms.Keys.Cancel), _
                    Chr(System.Windows.Forms.Keys.Back)
            Case Else
                Select Case Tipo
                    Case ControllaDati_TipoIntero
                        Param.Handled = True

                    Case ControllaDati_TipoData
                        If Chr(Param.KeyCode) <> "/" And Chr(Param.KeyCode) <> "." And Chr(Param.KeyCode) <> "-" Then
                            Param.Handled = True
                        End If

                    Case ControllaDati_TipoOra
                        If Chr(Param.KeyCode) <> "." Then
                            Param.Handled = True
                        End If
                End Select
        End Select
    End Sub

    ' Questa funzione permette di inserire la data senza i
    ' separatori. Se mancano vengono aggiunti

    Function ConvertiData(ByVal Data As String) As String

        Dim GG As String
        Dim MM As String
        Dim AA As String

        Dim Sep1 As String
        Dim Sep2 As String
        Dim Sep3 As String
        Dim SN_Sep As Boolean

        Sep1 = InStr(1, Data, "/")
        Sep2 = InStr(1, Data, ".")
        Sep3 = InStr(1, Data, ",")
        SN_Sep = False

        If Sep1 Then SN_Sep = True
        If Sep2 Then SN_Sep = True
        If Sep3 Then SN_Sep = True

        If Data.Length = 6 Then
            If SN_Sep = False Then
                GG = Trim$(Mid(Data, 1, 2))
                MM = Trim$(Mid(Data, 3, 2))
                AA = Trim$(Mid(Data, 5, 2))
                If MM > 12 Then
                    'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                    ConvertiData = Data
                    Exit Function
                End If
                Data = GG + "/" + MM + "/" + AA
                ConvertiData = Data
                Exit Function
            End If
        End If

        If Data.Length = 8 Then
            If SN_Sep = False Then
                GG = Trim$(Mid(Data, 1, 2))
                MM = Trim$(Mid(Data, 3, 2))
                AA = Trim$(Mid(Data, 5, 4))
                If MM > 12 Then
                    'MsgBox "Data errata.", vbOKOnly + vbInformation, "Errore"
                    ConvertiData = Data
                    Exit Function
                End If
                Data = GG + "/" + MM + "/" + AA
                ConvertiData = Data
                Exit Function
            End If
        End If

        ConvertiData = Data

    End Function

    Sub ComboKeyPress(ByVal Combo1 As ComboBox, ByVal Param As KeyPressEventArgs)
        'Questa versione non funziona con sp3 di framwork
        'Dim CB As Integer
        'Dim FindString As String

        'If Asc(Param.KeyChar) = Keys.Escape Then
        '    Combo1.SelectedIndex = -1
        '    Combo1.Text = ""
        'ElseIf Asc(Param.KeyChar) = Keys.Back Then 'giuseppe
        '    'FindString = Combo1.Text
        '    Combo1.SelectedIndex = -1
        'Else
        '    FindString = Combo1.Text
        '    CB = Combo1.FindString(FindString)
        '    If CB <> -1 Then
        '        Combo1.SelectedIndex = CB
        '        Combo1.SelectionStart = FindString.Length
        '        Combo1.SelectionLength = Combo1.Text.Length - Combo1.SelectionStart
        '    End If
        'End If

        'Param.Handled = True

        Dim CB As Integer
        Dim FindString As String

        If Asc(Param.KeyChar) = Keys.Escape Then
            Combo1.SelectedIndex = -1
            Combo1.SelectionStart = 0
            Combo1.SelectionLength = Combo1.Text.Length - Combo1.SelectionStart
        ElseIf Asc(Param.KeyChar) = Keys.Back Then 'giuseppe
            'FindString = Combo1.Text
            Combo1.SelectedIndex = -1
            Combo1.SelectionStart = 0
            Combo1.SelectionLength = Combo1.Text.Length - Combo1.SelectionStart
        Else
            FindString = Combo1.Text
            If FindString.Length > 0 Then
                If FindString.Substring(FindString.Length - 1, 1) <> Param.KeyChar Then
                    If Combo1.SelectionLength = 0 Then
                        FindString = Combo1.Text & Param.KeyChar
                    Else
                        FindString = Combo1.Text.Substring(0, Combo1.SelectionStart) & Param.KeyChar
                    End If
                End If
            Else
                FindString = Param.KeyChar
            End If
            CB = Combo1.FindString(FindString)
            If CB <> -1 Then
                Combo1.SelectedIndex = CB
                Combo1.SelectionStart = FindString.Length
                Combo1.SelectionLength = Combo1.Text.Length - Combo1.SelectionStart
            End If
        End If

        Param.Handled = True

    End Sub

    Public OraFormattata As String
    Function ControllaOre(ByVal Ora As String) As Boolean

        Dim Sp As Integer
        'Dim Ore As Integer

        Dim Ind As Byte
        Dim Ore As String
        Dim Minuti As String

        OraFormattata = ""

        Try
            ControllaOre = True
            Sp = -1
            If Ora.Length < 4 Then
                Return False
            Else
                For Ind = 0 To Ora.Length - 1
                    If Not Char.IsDigit(Ora.Substring(Ind, 1)) Then
                        Sp = Ind
                        Exit For
                    End If
                Next
                Ore = "" : Minuti = ""
                If Sp > 0 Then
                    Ore = Ora.Substring(0, Sp)
                    If Ora.Length > (Sp + 1) Then
                        Minuti = Ora.Substring(Sp + 1)
                    End If
                Else
                    If Ora.Length = 4 Then
                        Ore = Ora.Substring(0, 2)
                        Minuti = Ora.Substring(2, 2)
                    End If
                End If
                If Ore = "" Then
                    Ore = "00"
                End If
                If Minuti = "" Then
                    Minuti = "00"
                End If
                If Val(Ore) > 23 Then
                    Return False
                ElseIf Val(Minuti) > 59 Then
                    Return False
                Else
                    OraFormattata = Format(CDate(Ore + ":" + Minuti), "HH:mm")
                End If
            End If

            'OraFormattata = Ora

            'Sp = OraFormattata.IndexOf(".")
            'If Sp < 0 Then
            '    Sp = OraFormattata.IndexOf(System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator)
            'End If

            'If OraFormattata.Length = 4 Then
            '    If Sp = -1 Then
            '        'OraFormattata = OraFormattata.Substring(0, 2) + "." + OraFormattata.Substring(2, 2)
            '        OraFormattata = OraFormattata.Substring(0, 2) + System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator + OraFormattata.Substring(2, 2)
            '        'Sp = OraFormattata.IndexOf(".")
            '        Sp = OraFormattata.IndexOf(System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator)
            '    End If
            'End If

            'If Sp = -1 Then
            '    'allora sono solo ore
            '    If Val(OraFormattata) > 23 Then
            '        Return False
            '    Else
            '        OraFormattata = OraFormattata + System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator + "00"
            '    End If
            'Else
            '    If Sp = 1 Then OraFormattata = "00" + OraFormattata
            '    Sp = OraFormattata.IndexOf(".")
            '    If OraFormattata.IndexOf(".", Sp + 1) <> -1 Then
            '        Return False
            '    ElseIf OraFormattata.Length <> Sp + 3 Then
            '        Return False
            '    ElseIf CInt(OraFormattata.Substring(OraFormattata.Length - 2, 2)) > 59 Then
            '        Return False
            '    End If
            '    If OraFormattata.Substring(0, 2) > 23 Then
            '        Return False
            '    End If
            'End If
        Catch
            Return False
        End Try
    End Function

    Public Function FormattaImporto(ByVal Stringa As String, _
            ByVal CasellaImporto As TextBox, _
            ByVal Inizio As Integer, _
            ByVal CIFRE_INTERE As Byte, _
            ByVal CIFRE_DECIMALI As Byte) As String

        Dim MioImporto() As String
        Dim Intero As String
        Dim Decimale As String

        If Stringa <> "" Then
            MioImporto = Split(Stringa, ",")
            If UBound(MioImporto) <= 0 Then
                If Len(MioImporto(0)) > CIFRE_INTERE Then
                    Intero = MioImporto(0).Substring(0, CIFRE_INTERE)
                    'Intero = Left(MioImporto(0), CIFRE_INTERE)
                    Decimale = ""
                Else
                    Intero = MioImporto(0)
                    Decimale = ""
                End If
            Else
                If Len(MioImporto(1)) > CIFRE_DECIMALI Then
                    'Intero = MioImporto(0).Substring(0, CIFRE_INTERE)
                    Intero = Microsoft.VisualBasic.Left(MioImporto(0), CIFRE_INTERE)
                    'Decimale = MioImporto(1).Substring(0, CIFRE_DECIMALI)
                    Decimale = Microsoft.VisualBasic.Left(MioImporto(1), CIFRE_DECIMALI)
                Else
                    Intero = MioImporto(0)
                    Decimale = MioImporto(1)
                End If
            End If
        Else
            Intero = ""
            Decimale = ""
        End If
        If Decimale <> "" Then
            CasellaImporto.Text = Intero & "," & Decimale
        Else
            If InStr(Stringa, ",") = 0 Then
                CasellaImporto.Text = Intero
            Else
                CasellaImporto.Text = Intero & "," & Decimale
            End If
        End If
        If Inizio = Len(CasellaImporto.Text) Then
            CasellaImporto.SelectionStart = Len(CasellaImporto.Text)
        Else
            CasellaImporto.SelectionStart = Inizio
        End If
    End Function
    Function AggiungiApici(ByVal St As String) As String

        'Aggiungo un apice per ogni apice che trovo
        Dim I%

        I = 1
        I = InStr(I, St, "'", CompareMethod.Text)
        While (I <> 0)
            St = Left$(St, I) + "'" + Right$(St, Len(St) - I)
            I = InStr(I + 2, St, "'", 1)
        End While
        AggiungiApici = St

    End Function


End Class