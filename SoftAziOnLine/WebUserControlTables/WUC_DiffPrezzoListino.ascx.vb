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

Partial Public Class WUC_DiffPrezzoListino
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDA_GetTipiDoc.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        If (Not IsPostBack) Then

            ''rbtnPrezzoVendita.Checked = True
            ''rbtnCliente.Checked = True
            'chkTuttiArticoli.Checked = True
            'chkTuttiClienti.Checked = True
            'txtCod1.Enabled = False
            'txtDaDT.Enabled = False
            txtDataDa.Text = "01/01/" & Session(ESERCIZIO)
            txtDataA.Text = "31/12/" & Session(ESERCIZIO)
            Try
                impostaDate()
            Catch ex As Exception
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Errore", ex.Message, WUC_ModalPopup.TYPE_ERROR)
            End Try
            chkTuttiTipiDoc.Checked = True
            ddlTipoDoc.Enabled = False
            'txtDaDT.Text = "1"
            'txtADT.Text = "999999"
            Session(SWOP) = SWOPNESSUNA
            Session(SWOPDETTDOC) = SWOPNESSUNA
            Session(SWOPDETTDOCL) = SWOPNESSUNA

            
            ddlTipoDoc.Items.Clear()
            ddlTipoDoc.Items.Add("")
            ddlTipoDoc.DataBind()
        End If
        

        ModalPopup.WucElement = Me
    End Sub

    Private Sub impostaDate()

        Dim objDB As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Dim tmpConn As New SqlConnection
        tmpConn.ConnectionString = objDB.getConnectionString(It.SoftAzi.Integration.Dao.DataSource.TipoConnessione.dbSoftAzi)
        Dim tmpCommand As New SqlCommand
        tmpCommand.Connection = tmpConn
        tmpCommand.CommandType = CommandType.StoredProcedure
        tmpCommand.CommandText = "get_dateDocPerRiepControlloPrezzoVendita"
        tmpCommand.Parameters.Add("@DaData", SqlDbType.DateTime)
        tmpCommand.Parameters("@DaData").Direction = ParameterDirection.Output
        tmpCommand.Parameters.Add("@AData", SqlDbType.DateTime)
        tmpCommand.Parameters("@AData").Direction = ParameterDirection.Output

        tmpConn.Open()
        tmpCommand.ExecuteNonQuery()

        If Not IsDBNull(tmpCommand.Parameters("@DaData").Value) Then
            txtDataDa.Text = Format(CDate(tmpCommand.Parameters("@DaData").Value), FormatoData)
        End If
        If Not IsDBNull(tmpCommand.Parameters("@AData").Value) Then
            txtDataA.Text = Format(CDate(tmpCommand.Parameters("@AData").Value), FormatoData)
        End If

        tmpConn.Close()
        tmpConn.Dispose()
        tmpConn = Nothing
        tmpCommand.Dispose()
        tmpCommand = Nothing
        objDB = Nothing

    End Sub

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
        
        Dim Errore As String = ""
        Dim clsStampa As New Controllo
        Dim DSDiffPrezzoListino1 As New DSControlli
        Dim TipoDoc As String = ""

        If txtDataDa.Text <> "" And txtDataA.Text <> "" Then
            If IsDate(txtDataDa.Text) And IsDate(txtDataA.Text) Then
                If CDate(txtDataDa.Text) > CDate(txtDataA.Text) Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Data inizio periodo superiore alla data fine periodo", WUC_ModalPopup.TYPE_ALERT)
                    Exit Sub
                End If
            End If
        End If
        If chkTuttiTipiDoc.Checked = False And ddlTipoDoc.Items.Count > 0 Then
            If ddlTipoDoc.SelectedValue.Trim.Length > 0 Then
                TipoDoc = ddlTipoDoc.SelectedValue.Trim
            End If
        End If

        Try
            If clsStampa.StampaDiffPrezzoListino(TipoDoc, txtDataDa.Text.Trim, txtDataA.Text.Trim, Session(CSTAZIENDARPT), DSDiffPrezzoListino1, Errore) Then
                If DSDiffPrezzoListino1.DiffPrezzoListino.Count <= 0 Then
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Controllo", "Non ci sono documenti che abbiano differenza tra il prezzo ed il prezzo di listino, secondo i criteri selezionati.", WUC_ModalPopup.TYPE_INFO)
                    Exit Sub
                End If
                Session(CSTDsPrinWebDoc) = DSDiffPrezzoListino1
                Session(CSTTIPORPTCONTROLLO) = TIPOSTAMPACONTROLLO.DiffPrezzoListino
                Session(CSTNOBACK) = 0 'giu040512
                Response.Redirect("..\WebFormTables\WF_PrintWebControllo.aspx")
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

        ''selezione = "{BolleCliT.Tipo_Doc} in [""DT"", ""FT""]"
    End Sub

    Private Sub btnStampa_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStampa.Click
        StampaReport()
    End Sub

    Protected Sub chkTuttiTipiDoc_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkTuttiTipiDoc.CheckedChanged
        ddlTipoDoc.Enabled = Not (chkTuttiTipiDoc.Checked)
    End Sub
End Class