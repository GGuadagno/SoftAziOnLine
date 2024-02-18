Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports System.Data.SqlClient

Partial Public Class WUC_TestiEmail
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

    Public rk As StrTestiEmail

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDSTesti.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        lblLabelTipoRK.Text = "Gestione testi e-mail"
        If (Not IsPostBack) Then
            'ddlTesti.Items.Clear()
            'ddlTesti.Items.Add("")
            'ddlTesti.DataBind()

            'carico le chiavo predefinite o le seleziono se già presenti
            Dim ChiaveTipo1 As String = "ScadArtCons"
            Dim ChiaveTipo2 As String = "InvioFatture"

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
                strSQL = "IF EXISTS(SELECT ID FROM Testi WHERE Descrizione = '" & ChiaveTipo1 & "') "
                strSQL &= " SELECT ID FROM Testi WHERE Descrizione = '" & ChiaveTipo1 & "' "
                strSQL &= " ELSE "
                strSQL &= " INSERT INTO Testi ([Descrizione],[Oggetto],[Corpo],[PiePagina]) VALUES ('" & ChiaveTipo1 & "','','','') "
                SQLCmdIns.CommandText = strSQL
                SQLCmdIns.ExecuteNonQuery()

                ' insert (se non presente) / select ---CHIAVE 2
                strSQL = "IF EXISTS(SELECT ID FROM Testi WHERE Descrizione = '" & ChiaveTipo2 & "') "
                strSQL &= " SELECT ID FROM Testi WHERE Descrizione = '" & ChiaveTipo2 & "' "
                strSQL &= " ELSE "
                strSQL &= " INSERT INTO Testi ([Descrizione],[Oggetto],[Corpo],[PiePagina]) VALUES ('" & ChiaveTipo2 & "','','','') "
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
            ddlTesti.Items.Clear()
            ddlTesti.Items.Add("")
            ddlTesti.DataBind()
        End If
    End Sub

    Public Sub SvuotaCampi()
        Session(IDTESTIEMAIL) = ""
        ddlTesti.SelectedIndex = 0
        'txtDescrizione.Text = "" : txtDescrizione.BackColor = SEGNALA_OK
        txtOggetto.Text = "" : txtOggetto.BackColor = SEGNALA_OK
        txtCorpo.Text = "" : txtCorpo.BackColor = SEGNALA_OK
        txtPiePagina.Text = "" : txtPiePagina.BackColor = SEGNALA_OK
        rk = Nothing
        Session(RKTESTIEMAIL) = rk
    End Sub

    Public Function PopolaEntityDati() As Boolean
        PopolaEntityDati = True
        Dim dvTesti As DataView
        dvTesti = SqlDSTesti.Select(DataSourceSelectArguments.Empty)
        If (dvTesti Is Nothing) Then
            SvuotaCampi()
            Exit Function
        End If

        If dvTesti.Count > 0 Then
            If (Session(IDTESTIEMAIL).ToString.Trim = "") Or (Not IsNumeric(Session(IDTESTIEMAIL))) Then Session(IDTESTIEMAIL) = "0"
            dvTesti.RowFilter = "ID = " & Session(IDTESTIEMAIL).ToString.Trim
            If dvTesti.Count > 0 Then
                Session(IDTESTIEMAIL) = dvTesti.Item(0).Item("ID")
                'txtDescrizione.Text = dvTesti.Item(0).Item("Descrizione").ToString.Trim
                txtOggetto.Text = dvTesti.Item(0).Item("Oggetto").ToString.Trim
                txtCorpo.Text = dvTesti.Item(0).Item("Corpo").ToString.Trim
                txtPiePagina.Text = dvTesti.Item(0).Item("PiePagina").ToString.Trim
            Else
                SvuotaCampi()
                Exit Function
            End If
        Else
            SvuotaCampi()
            Exit Function
        End If

        rk.ID = Session(IDTESTIEMAIL)
        '-
        'If txtDescrizione.Text.Trim = "" Then
        '    txtDescrizione.BackColor = SEGNALA_KO : PopolaEntityDati = False
        'Else
        '    txtDescrizione.BackColor = SEGNALA_OK
        'End If
        txtOggetto.BackColor = SEGNALA_OK
        txtCorpo.BackColor = SEGNALA_OK
        txtPiePagina.BackColor = SEGNALA_OK

        If PopolaEntityDati = False Then Exit Function

        'rk.Descrizione = txtDescrizione.Text.Trim
        rk.Oggetto = txtOggetto.Text.Trim
        rk.Corpo = txtCorpo.Text.Trim
        rk.PiePagina = txtPiePagina.Text.Trim

        Session(RKTESTIEMAIL) = rk
    End Function

    Public Function Aggiorna() As Boolean
        Aggiorna = True

        'If txtDescrizione.Text.Trim = "" Then
        '    txtDescrizione.BackColor = SEGNALA_KO : Aggiorna = False
        'Else
        '    txtDescrizione.BackColor = SEGNALA_OK
        'End If
        txtOggetto.BackColor = SEGNALA_OK
        txtCorpo.BackColor = SEGNALA_OK
        txtPiePagina.BackColor = SEGNALA_OK

        If Aggiorna = False Then Exit Function

        Try
            If ddlTesti.SelectedIndex > 0 Then
                Session(IDTESTIEMAIL) = IIf(IsNumeric(ddlTesti.SelectedValue), ddlTesti.SelectedValue, 0)
            Else
                Session(IDTESTIEMAIL) = ""
            End If
            SqlDSTesti.UpdateParameters.Item("ID").DefaultValue = Session(IDTESTIEMAIL)
            SqlDSTesti.UpdateParameters.Item("Descrizione").DefaultValue = ddlTesti.SelectedItem.Text.Trim 'txtDescrizione.Text.Trim
            SqlDSTesti.UpdateParameters.Item("Oggetto").DefaultValue = txtOggetto.Text.Trim
            SqlDSTesti.UpdateParameters.Item("Corpo").DefaultValue = txtCorpo.Text.Trim
            SqlDSTesti.UpdateParameters.Item("PiePagina").DefaultValue = txtPiePagina.Text.Trim
            SqlDSTesti.Update()
            SqlDSTesti.DataBind()
            '-----
            ddlTesti.Items.Clear()
            ddlTesti.Items.Add("")  'aggiungo elemento vuoto
            ddlTesti.DataBind()
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

    Private Sub ddlTesti_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTesti.SelectedIndexChanged
        Session(IDTESTIEMAIL) = IIf(IsNumeric(ddlTesti.SelectedValue), ddlTesti.SelectedValue, 0)
        If ddlTesti.SelectedIndex = 0 Then
            Session(IDTESTIEMAIL) = ""
            SvuotaCampi()
            Exit Sub
        End If
        _WucElement.SetlblMessaggi("Seleziona e aggiorna elemento in tabella")
        PopolaEntityDati()
    End Sub

    'Private Sub txtDescrizione_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDescrizione.TextChanged
    '    Dim dvTesti As DataView
    '    dvTesti = SqlDSTesti.Select(DataSourceSelectArguments.Empty)
    '    If (dvTesti Is Nothing) Then
    '        Exit Sub
    '    End If
    '    If dvTesti.Count > 0 Then
    '        dvTesti.RowFilter = "Descrizione = '" & Controlla_Apice(txtDescrizione.Text.Trim) & "'"
    '        If dvTesti.Count > 0 Then
    '            Session(IDTESTIEMAIL) = dvTesti.Item(0).Item("ID")
    '            txtDescrizione.Text = dvTesti.Item(0).Item("Descrizione").ToString.Trim
    '            txtOggetto.Text = dvTesti.Item(0).Item("Oggetto").ToString.Trim
    '            txtCorpo.Text = dvTesti.Item(0).Item("Corpo").ToString.Trim
    '            txtPiePagina.Text = dvTesti.Item(0).Item("PiePagina").ToString.Trim
    '        End If
    '    End If
    'End Sub
End Class