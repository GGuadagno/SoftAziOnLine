Imports SoftAziOnLine.Def

Public Class Formatta

    Public Shared Function NormalizzaStringa(ByVal stringa As String) As String
        stringa = stringa.Replace(HTML_SPAZIO, String.Empty)
        stringa = stringa.Replace(HTML_AND, "&")
        NormalizzaStringa = stringa
    End Function

    Public Shared Function Controlla_Apice(ByVal Stringa_In As String) As String

        Dim X As Integer
        Dim Y As Integer

        Dim Comodo As String

        X = InStr(Stringa_In, "'")
        If X > 0 Then
            Y = 1
            Comodo = Stringa_In
            Do Until X <= 0
                'giu010416 Comodo = Mid(Comodo, Y, X) + "'" + Right(Comodo, Len(Comodo) - X)
                Comodo = Mid(Comodo, 1, X) + "'" + Right(Comodo, Len(Comodo) - X)
                Y = X + 2
                X = InStr(Y, Comodo, "'")
            Loop
            Controlla_Apice = Comodo
        Else
            Controlla_Apice = Stringa_In
        End If

    End Function
    'giu231114 per la gestione dei decimali distinta base 
    'per non creare problemi lascio i decimali se ci sono senza arrotondare 
    'NOTA il massimo dei decimali sono 4 
    'SE SERVISSE SCOMMENTARE LA FUNZIONE
    ' ''Public Shared Function LasciaNumeriDecimale(ByVal A As String) As String

    ' ''    Dim J As Integer
    ' ''    Dim e As String
    ' ''    e = ""
    ' ''    A = Trim(A)
    ' ''    If A <> "" Then
    ' ''        For J = 1 To Len(A)
    ' ''            'If (IsNumeric(Mid(A, j, 1))) Or (Mid(A, j, 1) = ",") Then
    ' ''            If (IsNumeric(Mid(A, J, 1))) Or (Mid(A, J, 1) = ",") Or (Mid(A, J, 1) = "-") Then
    ' ''                e = e & Mid(A, J, 1)
    ' ''            End If
    ' ''        Next J
    ' ''    End If

    ' ''    LasciaNumeriDecimale = e

    ' ''End Function
    Public Shared Sub EliminaDecimaliNonSignificativi(ByRef Value As Object)
        Dim myPos As Integer = InStr(Value, ",")
        If myPos > 0 Then
            If Right(Value, 1) = "," Then
                Value = Left(Value, Len(Value) - 1)
                Exit Sub
            End If
            Do Until Right(Value, 1) <> 0 And Right(Value, 1) <> ","
                If Right(Value, 1) = "," Then
                    Value = Left(Value, Len(Value) - 1)
                    Exit Do
                End If
                Value = Left(Value, Len(Value) - 1)
                If Right(Value, 1) = "," Then
                    Value = Left(Value, Len(Value) - 1)
                    Exit Sub
                End If
            Loop
        End If

    End Sub
    'GIU231114 NdECIMALI -1 VUOL DIRE CHE DEVO LASCIARE I DECIMALI AL MAX LUNG. 4 DECIMALI
    'giu260423 mancava decimali = 1
    Public Shared Function FormattaNumero(ByRef valore As String, Optional ByRef nDecimali As Integer = 0) As String
        If valore.Trim = "" Then FormattaNumero = "" : Exit Function
        If nDecimali = -1 Or nDecimali = 0 Then
            EliminaDecimaliNonSignificativi(valore)
            Dim myPos As Integer = InStr(valore, ",")
            If myPos > 0 Then
                nDecimali = Len(Mid(valore, myPos + 1))
                If nDecimali > 4 Then
                    nDecimali = 4
                End If
            Else
                nDecimali = 0
            End If
        End If
        Dim FormatValuta As String = "###,###,##0"
        Select Case CInt(nDecimali)
            Case 0
                FormatValuta = "###,###,##0"
            Case 1
                FormatValuta = "###,###,##0.0"
            Case 2
                FormatValuta = "###,###,##0.00"
            Case 3
                FormatValuta = "###,###,##0.000"
            Case 4
                FormatValuta = "###,###,##0.0000"
            Case Else
                FormatValuta = "###,###,##0"
        End Select
        FormattaNumero = Format(Decimal.Parse(valore), FormatValuta)
    End Function

    'GIU090123 
    Public Shared Function FormattaNomeFile(ByVal sIn As String) As String
        FormattaNomeFile = ""
        Dim c
        Dim sOut As String = ""
        For i = 1 To Len(sIn)
            c = Mid(sIn, i, 1)
            If InStr(UCase("abcdefghijklmnopqrstuvwxyz_-"), UCase(c)) <= 0 And Not IsNumeric(c) Then
                'nulla lo scarto e non metto alcun spazio
            Else
                sOut += c
            End If
        Next
        FormattaNomeFile = sOut.Trim
    End Function
    'GIU290523 
    Public Shared Function SoloNumeri(ByVal sIn As String) As String
        SoloNumeri = ""
        Dim c
        Dim sOut As String = ""
        Dim SpazioPrec As Boolean = False
        For i = 1 To Len(sIn)
            c = Mid(sIn, i, 1)
            If InStr(UCase("0123456789"), UCase(c)) <= 0 And Not IsNumeric(c) Then
                If SpazioPrec = False Then
                    sOut += " "
                    SpazioPrec = True
                Else
                    'scarto senza accodare nulla
                End If
            Else
                sOut += c
            End If
        Next
        SoloNumeri = sOut.Trim
    End Function
    'GIU290523 
    Public Shared Function NoLettereAcc(ByVal sIn As String) As String
        NoLettereAcc = ""
        Dim sOut As String = sIn.Replace("-", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("è", "e")
        sOut = sOut.Replace("é", "e")
        sOut = sOut.Replace("ò", "o")
        sOut = sOut.Replace("ù", "u")
        sOut = sOut.Replace("ì", "i")
        sOut = sOut.Replace("à", "a")
        sOut = sOut.Replace("€", "E")
        sOut = sOut.Replace("ç", "c")
        sOut = sOut.Replace("|", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("£", "L")
        sOut = sOut.Replace("$", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("%", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("&", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("^", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("°", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("§", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("@", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("#", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("[", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("]", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("{", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("}", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("_", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("-", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("<", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace(">", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("=", " ")
        sOut = sOut.Replace("  ", " ")
        sOut = sOut.Replace("'", " ")
        sOut = sOut.Replace("  ", " ")
        NoLettereAcc = sOut.Trim
    End Function
End Class
