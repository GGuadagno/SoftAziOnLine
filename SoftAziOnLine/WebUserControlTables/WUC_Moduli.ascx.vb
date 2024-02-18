Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient

Partial Public Class WUC_Moduli
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

    Public rk As StrModuli

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSModuli.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        lblLabelTipoRK.Text = "Gestione allegati E-Mail scadenze"
        If (Not IsPostBack) Then
            'carico le chiavo predefinite o le seleziono se già presenti
            Dim ChiaveTipo1 As String = "HS1"
            Dim ChiaveTipo2 As String = "FR2"
            Dim ChiaveTipo3 As String = "FR3"
            Dim ChiaveTipo4 As String = "FRX"

            Dim strSQL As String = ""
            Dim SqlConn As New SqlConnection
            SqlConn.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
            Dim SQLCmdIns As New SqlCommand

            SQLCmdIns.CommandType = CommandType.Text
            SQLCmdIns.Connection = SqlConn
            SQLCmdIns.CommandText = strSQL

            Try
                If SqlConn.State <> ConnectionState.Open Then
                    SqlConn.Open()
                End If

                ' insert (se non presente) / select ---CHIAVE 1
                strSQL = "IF EXISTS(SELECT ID FROM Moduli WHERE Tipo = '" & ChiaveTipo1 & "') "
                strSQL &= " SELECT ID FROM Moduli WHERE Tipo = '" & ChiaveTipo1 & "' "
                strSQL &= " ELSE "
                strSQL &= " INSERT INTO Moduli ([Tipo],[Percorso]) VALUES ('" & ChiaveTipo1 & "', '') "
                SQLCmdIns.CommandText = strSQL
                SQLCmdIns.ExecuteNonQuery()

                ' insert (se non presente) / select ---CHIAVE 2
                strSQL = "IF EXISTS(SELECT ID FROM Moduli WHERE Tipo = '" & ChiaveTipo2 & "') "
                strSQL &= " SELECT ID FROM Moduli WHERE Tipo = '" & ChiaveTipo2 & "' "
                strSQL &= " ELSE "
                strSQL &= " INSERT INTO Moduli ([Tipo],[Percorso]) VALUES ('" & ChiaveTipo2 & "', '') "
                SQLCmdIns.CommandText = strSQL
                SQLCmdIns.ExecuteNonQuery()

                ' insert (se non presente) / select ---CHIAVE 3
                strSQL = "IF EXISTS(SELECT ID FROM Moduli WHERE Tipo = '" & ChiaveTipo3 & "') "
                strSQL &= " SELECT ID FROM Moduli WHERE Tipo = '" & ChiaveTipo3 & "' "
                strSQL &= " ELSE "
                strSQL &= " INSERT INTO Moduli ([Tipo],[Percorso]) VALUES ('" & ChiaveTipo3 & "', '') "
                SQLCmdIns.CommandText = strSQL
                SQLCmdIns.ExecuteNonQuery()

                ' insert (se non presente) / select ---CHIAVE 4
                strSQL = "IF EXISTS(SELECT ID FROM Moduli WHERE Tipo = '" & ChiaveTipo4 & "') "
                strSQL &= " SELECT ID FROM Moduli WHERE Tipo = '" & ChiaveTipo4 & "' "
                strSQL &= " ELSE "
                strSQL &= " INSERT INTO Moduli ([Tipo],[Percorso]) VALUES ('" & ChiaveTipo4 & "', '') "
                SQLCmdIns.CommandText = strSQL
                SQLCmdIns.ExecuteNonQuery()

                If SqlConn.State <> ConnectionState.Closed Then
                    SqlConn.Close()
                End If
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore durante il caricamento delle chiavi", ex.Message, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End Try

            'effettuo select dal DB e carico la DropDownList
            ddlModuli.Items.Clear()
            ddlModuli.Items.Add("")
            ddlModuli.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(IDMODULO) = ""
        ddlModuli.SelectedIndex = 0
        ' txtTipo.Text = "" : txtTipo.BackColor = SEGNALA_OK
        txtPercorso.Text = "" : txtPercorso.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKMODULI) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvModuli As DataView
        dvModuli = SqlDSModuli.Select(DataSourceSelectArguments.Empty)
        If (dvModuli Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If

        If dvModuli.Count > 0 Then
            If (Session(IDMODULO).ToString.Trim = "") Or (Not IsNumeric(Session(IDMODULO))) Then Session(IDMODULO) = "0"
            dvModuli.RowFilter = "ID = " & Session(IDMODULO).ToString.Trim
            If dvModuli.Count > 0 Then
                Session(IDMODULO) = dvModuli.Item(0).Item("ID")
                'txtTipo.Text = dvModuli.Item(0).Item("Tipo").ToString.Trim
                txtPercorso.Text = dvModuli.Item(0).Item("Percorso").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.ID = Session(IDMODULO)
        '-
        'If txtTipo.Text.Trim = "" Then
        '    txtTipo.BackColor = SEGNALA_KO : PopolaEntityDati = False
        'Else
        '    txtTipo.BackColor = SEGNALA_OK
        'End If
        txtPercorso.BackColor = SEGNALA_OK

        If PopolaEntityDati = False Then Exit Function

        'rk.Tipo = txtTipo.Text.Trim
        rk.Percorso = txtPercorso.Text.Trim

        Session(RKMODULI) = rk
    End Function

    Public Function Aggiorna() As Boolean
        Aggiorna = True

        'If txtTipo.Text.Trim = "" Then
        '    txtTipo.BackColor = SEGNALA_KO : Aggiorna = False
        'Else
        '    txtTipo.BackColor = SEGNALA_OK
        'End If
        txtPercorso.BackColor = SEGNALA_OK

        If Aggiorna = False Then Exit Function

        Try
            If ddlModuli.SelectedIndex > 0 Then
                Session(IDMODULO) = IIf(IsNumeric(ddlModuli.SelectedValue), ddlModuli.SelectedValue, 0)
            Else
                Session(IDMODULO) = ""
            End If
            SqlDSModuli.UpdateParameters.Item("ID").DefaultValue = Session(IDMODULO)
            SqlDSModuli.UpdateParameters.Item("Tipo").DefaultValue = ddlModuli.SelectedItem.Text.Trim 'txtTipo.Text.Trim
            SqlDSModuli.UpdateParameters.Item("Percorso").DefaultValue = txtPercorso.Text.Trim
            SqlDSModuli.Update()
            SqlDSModuli.DataBind()
            '-----
            ddlModuli.Items.Clear()
            ddlModuli.Items.Add("")  'aggiungo elemento vuoto
            ddlModuli.DataBind()
            PopolaEntityDati()
            '' ''Dim strErrore As String = ""
            '' ''App.CaricaCategorie(Session(ESERCIZIO), strErrore) 'pier110112 AGGIORNO LA CACHE
            '' ''If strErrore.Trim <> "" Then
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD) = ""
            ' ''    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            '' ''    ModalPopup.Show("Errore caricamento dati in memoria temporanea", strErrore.Trim, WUC_ModalPopup.TYPE_ERROR)
            '' ''End If
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in aggiorna", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            Return False
        End Try

    End Function

    Private Sub ddlModuli_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlModuli.SelectedIndexChanged
        Session(IDMODULO) = IIf(IsNumeric(ddlModuli.SelectedValue), ddlModuli.SelectedValue, 0)
        If ddlModuli.SelectedIndex = 0 Then
            Session(IDMODULO) = ""
            SvuotaCampi()
            Exit Sub
        End If
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        PopolaEntityDati()
    End Sub

    'Private Sub txtTipo_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTipo.TextChanged
    '    Dim dvModuli As DataView
    '    dvModuli = SqlDSModuli.Select(DataSourceSelectArguments.Empty)
    '    If (dvModuli Is Nothing) Then
    '        Exit Sub
    '    End If
    '    If dvModuli.Count > 0 Then
    '        dvModuli.RowFilter = "Tipo = '" & Controlla_Apice(txtTipo.Text.Trim) & "'"
    '        If dvModuli.Count > 0 Then
    '            Session(IDMODULO) = dvModuli.Item(0).Item("ID")
    '            'txtTipo.Text = dvModuli.Item(0).Item("Tipo").ToString.Trim
    '            txtPercorso.Text = dvModuli.Item(0).Item("Percorso").ToString.Trim
    '        End If
    '    End If
    'End Sub

End Class