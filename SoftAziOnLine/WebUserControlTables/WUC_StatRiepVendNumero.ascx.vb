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

Partial Public Class WUC_StatRiepVendNumero


    Inherits System.Web.UI.UserControl

    Public Const MSGSelezioneErrata As String = "Il numero di partenza deve essere precedere il numero di fine selezione."

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not IsPostBack) Then
            ''rbtnPrezzoVendita.Checked = True
            ''rbtnCliente.Checked = True
            'chkTuttiArticoli.Checked = True
            'chkTuttiClienti.Checked = True
            'txtCod1.Enabled = False
            'txtDaDT.Enabled = False
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            ' ''Try
            ' ''    impostaDate()
            ' ''Catch ex As Exception
            ' ''    ' ''Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''    ' ''Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ' ''    ' ''ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            ' ''    txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            ' ''    txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            ' ''End Try
            txtDaDT.Text = "1"
            txtADT.Text = "999999"
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA
            rbtnVenduto.Checked = True 'alberto 11/05/2012
        End If
        ModalPopup.WucElement = Me
    End Sub

    ' ''Private Sub impostaDate()

    ' ''    Dim objDB As New dbStringaConnesioneFacade
    ' ''    Dim tmpConn As New SqlConnection
    ' ''    tmpConn.ConnectionString = objDB.getConnectionString(It.SoftAzi.Integration.Dao.DataSource.TipoConnessione.dbSoftAzi)
    ' ''    Dim tmpCommand As New SqlCommand
    ' ''    tmpCommand.Connection = tmpConn
    ' ''    tmpCommand.CommandType = CommandType.StoredProcedure
    ' ''    tmpCommand.CommandText = "get_dateDocPerRiepVendutoFatturato"
    ' ''    tmpCommand.Parameters.Add("@DaData", SqlDbType.DateTime)
    ' ''    tmpCommand.Parameters("@DaData").Direction = ParameterDirection.Output
    ' ''    tmpCommand.Parameters.Add("@AData", SqlDbType.DateTime)
    ' ''    tmpCommand.Parameters("@AData").Direction = ParameterDirection.Output

    ' ''    tmpConn.Open()
    ' ''    tmpCommand.ExecuteNonQuery()

    ' ''    If Not IsDBNull(tmpCommand.Parameters("@DaData").Value) Then
    ' ''        txtDataDa.Text = Format(CDate(tmpCommand.Parameters("@DaData").Value), FormatoData)
    ' ''    End If
    ' ''    If Not IsDBNull(tmpCommand.Parameters("@AData").Value) Then
    ' ''        txtDataA.Text = Format(CDate(tmpCommand.Parameters("@AData").Value), FormatoData)

    ' ''    End If

    ' ''    tmpConn.Close()
    ' ''    tmpConn.Dispose()
    ' ''    tmpConn = Nothing
    ' ''    tmpCommand.Dispose()
    ' ''    tmpCommand = Nothing
    ' ''    objDB = Nothing

    ' ''End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        ' ''Try
        ' ''    Response.Redirect("..\WebFormTables\WF_Menu.aspx?labelForm=Menu principale")
        ' ''Catch ex As Exception
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
        ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        ' ''    ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
        ' ''End Try
        Dim strRitorno As String = "WF_Menu.aspx?labelForm=Menu principale"
        ' ''strRitorno = ""
        ' ''If strErrore.Trim = "" Then
        ' ''    strRitorno = "WF_Menu.aspx?labelForm=Menu principale"
        ' ''Else
        ' ''    strRitorno = "WF_Menu.aspx?labelForm=" & strErrore
        ' ''End If
        Try
            Response.Redirect(strRitorno)
        Catch ex As Exception
            Response.Redirect(strRitorno)
        End Try
    End Sub

    Private Sub StampaReport()
        Dim okWhere As Boolean = False
        Dim Errore As String = ""
        Dim clsStampa As New Statistiche
        Dim dsStampa As New DSStatRiepVendutoNumero
        Dim TipoStat As Integer 'alberto 11/05/2012

        'alberto 11/05/2012
        If rbtnVenduto.Checked Then
            TipoStat = 0  'venduto e fatturato
        ElseIf rbtnFatturato.Checked Then
            TipoStat = 1  'fatturato
        ElseIf rbtnDaFatturare.Checked Then
            TipoStat = 2  'da fatturare
        Else
            TipoStat = 0
        End If
        '----------------------

        If NoRecord(TipoStat) Then Exit Sub

        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Data inizio periodo superiore alla data fine periodo", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            Else
                If Not IsDate(txtDataDa.Text.Trim) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Il valore inserito non è un valore data valido.", WUC_ModalPopup.TYPE_ALERT)
                    txtDataDa.Focus()
                    Exit Sub
                End If
                If Not IsDate(txtDataA.Text.Trim) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Il valore inserito non è un valore data valido.", WUC_ModalPopup.TYPE_ALERT)
                    txtDataA.Focus()
                    Exit Sub
                End If
            End If
        End If

        'alberto 11/05/2012 commentato perchè sostituita view con stored procedure
        'selezione = " WHERE (Tipo_Doc = 'DT') AND (CAST(Numero AS int) >= " & CInt(txtDaDT.Text.Trim) & " AND CAST(Numero AS int) <= " & CInt(txtADT.Text.Trim) & ") " '_
        '& "OR (Tipo_Doc = 'FA') AND (CAST(Numero AS int) >= " & Val(txtDaFA.Text.Trim) & " AND CAST(Numero AS int) <= " & Val(txtAFA.Text.Trim) & ")"

        'If txtDataDa.Text <> "" Then
        '    If IsDate(txtDataDa.Text) Then
        '        selezione = selezione & " AND (Data_Bolla >= CONVERT(DATETIME,'" & Format(CDate(txtDataDa.Text), FormatoData) & "',103))"
        '    End If
        'End If

        'If txtDataA.Text <> "" Then
        '    If IsDate(txtDataA.Text) Then
        '        selezione = selezione & " AND (Data_Bolla <= CONVERT(DATETIME,'" & Format(CDate(txtDataA.Text), FormatoData) & "',103))"
        '    End If
        'End If

        Try
            If clsStampa.StampaVendutoDDT(Session(CSTAZIENDARPT), dsStampa, Errore, TipoStat, CInt(txtDaDT.Text.Trim), CInt(txtADT.Text.Trim), txtDataDa.Text.Trim, txtDataA.Text.Trim) Then
                Session("dsVendutoDDT") = dsStampa
                Session(CSTSTATISTICHE) = TIPOSTAMPASTATISTICA.VendutoDDT
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebStatistiche.aspx")
            Else
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
            End If
        Catch ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore", Errore, WUC_ModalPopup.TYPE_ERROR)
        End Try

        'selezione = "{BolleCliT.Tipo_Doc} in [""DT"", ""FT""]"
    End Sub

    Private Function NoRecord(ByVal TipoStatistica As Integer) As Boolean
        Dim ds As New DataSet()
        'alberto 11/05/2012
        Dim clsDB As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim SQLAdp As New SqlDataAdapter
        Dim SQLCmd As New SqlCommand
        Dim SQLConn As New SqlConnection
        '-------------------

        'controllo che ci siano numeri di bolla all'interno dell'intervallo richiesto
        If txtDaDT.Text.Trim <> "" And txtADT.Text.Trim <> "" Then 'And txtDaFA.Text.Trim <> "" And txtAFA.Text.Trim <> "" Then
            If CInt(txtDaDT.Text.Trim) > CInt(txtADT.Text.Trim) Then
                NoRecord = True 'Clausola "Da numero" > di "A numero"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Riepilogo venduto per DDT", MSGSelezioneErrata, WUC_ModalPopup.TYPE_ALERT)
                ds = Nothing
                Exit Function
            End If
            'strSQL = "SELECT * FROM vi_VendutoDDT WHERE (Tipo_Doc = N'DT') AND (CAST(vi_VendutoDDT.Numero AS int) >= " & CInt(txtDaDT.Text.Trim) & " AND CAST(vi_VendutoDDT.Numero AS int) <= " & CInt(txtADT.Text.Trim) & ") " ' _
            '& "(Tipo_Doc = N'FA') AND (CAST(vi_VendutoDDT.Numero AS int) >= " & CInt(txtDaFA.Text.Trim) & " AND CAST(vi_VendutoDDT.Numero AS int) <= " & CInt(txtDaFA.Text.Trim) & ")"

        ElseIf txtDaDT.Text.Trim = "" Or txtADT.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Riepilogo venduto per DDT", "Selezionare i numeri di bolla.", WUC_ModalPopup.TYPE_ALERT)
            'ElseIf txtDaFA.Text.Trim = "" Or txtAFA.Text.Trim = "" Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '   ModalPopup.Show("Riepilogo venduto per D.D.T.", "Selezionare i numeri di fattura accompagnatoria.", WUC_ModalPopup.TYPE_ALERT)
        End If

        'alberto 11/05/2012
        Try
            SQLConn.ConnectionString = clsDB.getConnectionString(TipoDB.dbSoftAzi)

            SQLCmd.CommandText = "[get_VendutoDDTPerStampa]"

            SQLCmd.CommandType = CommandType.StoredProcedure
            SQLCmd.Connection = SQLConn

            SQLCmd.Parameters.Add(New SqlParameter("@TipoStatistica", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@DaDDT", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@ADDT", SqlDbType.Int, 4))
            SQLCmd.Parameters.Add(New SqlParameter("@DataDa", SqlDbType.NVarChar, 10))
            SQLCmd.Parameters.Add(New SqlParameter("@DataA", SqlDbType.NVarChar, 10))

            SQLCmd.Parameters("@TipoStatistica").Value = TipoStatistica
            SQLCmd.Parameters("@DaDDT").Value = CInt(txtDaDT.Text.Trim)
            SQLCmd.Parameters("@ADDT").Value = CInt(txtADT.Text.Trim)

            If txtDataDa.Text.Trim <> "" Then
                SQLCmd.Parameters("@DataDa").Value = txtDataDa.Text.Trim
            End If

            If txtDataA.Text.Trim <> "" Then
                SQLCmd.Parameters("@DataA").Value = txtDataA.Text.Trim
            End If
            '----------------

            SQLAdp.SelectCommand = SQLCmd

            SQLAdp.Fill(ds, "VendutoNumero")

        Catch ex As Exception
            NoRecord = True
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Verifica presenza DDT", "Si è verificato un errore durante il controllo della presenza di documenti di trasporto compresi nei criteri di scelta selezionati.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try

        If ds.Tables("VendutoNumero").Rows.Count = 0 Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Verifica presenza DDT", "Non sono presenti documenti di trasporto compresi nei criteri di scelta selezionati.", WUC_ModalPopup.TYPE_INFO)
            txtDaDT.Focus()
            NoRecord = True
        Else
            NoRecord = False
        End If
    End Function

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        If txtADT.Text.Trim = "" Or txtDaDT.Text.Trim = "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Riepilogo venduto per DDT", "Selezionare i numeri di bolla.", WUC_ModalPopup.TYPE_ALERT)
            If txtADT.Text.Trim = "" Then
                txtADT.Focus()
            ElseIf txtDaDT.Text.Trim = "" Then
                txtDaDT.Focus()
            End If
            Exit Sub
        End If

        If IsNumeric(txtDaDT.Text.Trim) And IsNumeric(txtADT.Text.Trim) Then
            'If txtAFA.Text.Trim = "" Then
            '    txtAFA.Text = txtADT.Text
            'Else
            '    If Not IsNumeric(txtAFA.Text.Trim) Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '        ModalPopup.Show("Riepilogo venduto per D.D.T.", "Richiesti valori numerici.", WUC_ModalPopup.TYPE_ALERT)
            '        txtAFA.Focus()
            '        Exit Sub
            '    End If
            'End If
            'If txtDaFA.Text.Trim = "" Then
            '    txtDaFA.Text = txtDaDT.Text
            'Else
            '    If Not IsNumeric(txtDaFA.Text.Trim) Then
            'Session(MODALPOPUP_CALLBACK_METHOD) = ""
            'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '        ModalPopup.Show("Riepilogo venduto per D.D.T.", "Richiesti valori numerici.", WUC_ModalPopup.TYPE_ALERT)
            '        txtDaFA.Focus()
            '        Exit Sub
            '    End If
            'End If
        Else
            If Not IsNumeric(txtDaDT.Text.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Riepilogo venduto per DDT", "Richiesti valori numerici.", WUC_ModalPopup.TYPE_ALERT)
                txtDaDT.Focus()
                Exit Sub
            End If
            If Not IsNumeric(txtADT.Text.Trim) Then
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Riepilogo venduto per DDT", "Richiesti valori numerici.", WUC_ModalPopup.TYPE_ALERT)
                txtADT.Focus()
                Exit Sub
            End If
        End If

        'If txtAFA.Text.Trim = "" Or txtDaFA.Text.Trim = "" Then
        'Session(MODALPOPUP_CALLBACK_METHOD) = ""
        'Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
        '    ModalPopup.Show("Riepilogo venduto per DDT", "Selezionare i numeri di fattura accompagnatoria.", WUC_ModalPopup.TYPE_ALERT)
        '    If txtAFA.Text.Trim = "" Then
        '        txtAFA.Focus()
        '    ElseIf txtDaFA.Text.Trim = "" Then
        '        txtDaFA.Focus()
        '    End If
        '    Exit Sub
        'End If
        StampaReport()
    End Sub
End Class