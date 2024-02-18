Imports SoftAziOnLine.Def
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports It.SoftAzi.Model.Facade

Partial Public Class WUC_StampaElencoAI
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
        Dim dbConn As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlDa_CatCli.ConnectionString = dbConn.getConnectionString(TipoDB.dbSoftCoge)
    End Sub

    Public Sub Show()
        If txtDallaDataSc.Text.Trim = "" Then txtDallaDataSc.Text = Format(CDate(DateAdd(DateInterval.Month, 1, Now.Date)).Date, FormatoData)
        If txtAllaDataSc.Text.Trim = "" Then txtAllaDataSc.Text = Format(CDate(DateAdd(DateInterval.Month, 2, Now.Date)).Date, FormatoData)
        txtDallaDataSc.BackColor = SEGNALA_OK : txtDallaDataSc.ToolTip = ""
        txtAllaDataSc.BackColor = SEGNALA_OK : txtAllaDataSc.ToolTip = ""
        'txtCodArticolo.BackColor = SEGNALA_OK : txtCodArticolo.ToolTip = ""
        'txtCodCliente.BackColor = SEGNALA_OK : txtCodCliente.ToolTip = ""
        ProgrammaticModalPopup.Show()
    End Sub

    Protected Sub OkButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Not IsDate(txtDallaDataSc.Text) Then
            txtDallaDataSc.BackColor = SEGNALA_KO
            txtDallaDataSc.ToolTip = "Data Scadenza errata"
            ProgrammaticModalPopup.Show()
            Exit Sub
        Else
            txtDallaDataSc.BackColor = SEGNALA_OK
        End If

        If Not IsDate(txtAllaDataSc.Text) Then
            txtAllaDataSc.BackColor = SEGNALA_KO
            txtAllaDataSc.ToolTip = "Data Scadenza errata"
            Exit Sub
        Else
            txtAllaDataSc.BackColor = SEGNALA_OK
        End If

        'If txtCodArticolo.Text.Trim <> "" Then
        '    If App.GetValoreFromChiave(txtCodArticolo.Text.Trim, Def.E_COD_ARTICOLI, Session(ESERCIZIO)) = "" Then
        '        txtCodArticolo.BackColor = SEGNALA_KO
        '        txtCodArticolo.ToolTip = "Codice Articolo inesistente"
        '        ProgrammaticModalPopup.Show()
        '        Exit Sub
        '    Else
        '        txtCodArticolo.BackColor = SEGNALA_OK
        '    End If
        'Else
        '    txtCodArticolo.BackColor = SEGNALA_OK
        'End If

        'If txtCodCliente.Text.Trim <> "" Then
        '    If App.GetValoreFromChiave(txtCodCliente.Text.Trim, Def.CLIENTI, Session(ESERCIZIO)) = "" Then
        '        txtCodCliente.BackColor = SEGNALA_KO
        '        txtCodCliente.ToolTip = "Codice Cliente inesistente"
        '        ProgrammaticModalPopup.Show()
        '        Exit Sub
        '    Else
        '        txtCodCliente.BackColor = SEGNALA_OK
        '    End If
        'Else
        '    txtCodCliente.BackColor = SEGNALA_OK
        'End If

        Dim TipoOrdine As String = "Cod_CoGe,Cod_Articolo,NSerie"
        Dim TipoOrdineRPT As Integer = TipoStampaAICA.ElencoAICACliArtNSerie
        Dim TipoOrdineST As String = "Cliente,Articolo,NSerie"
        If rbCliArt.Checked = True Then
            TipoOrdineST = "Cliente,Articolo,NSerie"
            TipoOrdine = "Cod_CoGe,Cod_Articolo,NSerie"
            TipoOrdineRPT = TipoStampaAICA.ElencoAICACliArtNSerie
        End If
        '-
        If rbArtCli.Checked = True Then
            TipoOrdineST = "Articolo,Cliente,NSerie"
            TipoOrdine = "Cod_Articolo,Cod_CoGe,NSerie"
            TipoOrdineRPT = TipoStampaAICA.ElencoAICAArtCliNSerie
        End If
        '-
        If rbScGaArtCli.Checked = True Then
            TipoOrdineST = "Data Scadenza Garanzia,Articolo,Cliente,NSerie"
            TipoOrdine = "DataScadGaranzia,Cod_Articolo,Cod_CoGe,NSerie"
            TipoOrdineRPT = TipoStampaAICA.ElencoAICAScGaArtCliNSerie
        End If
        '-
        If rbScElArtCli.Checked = True Then
            TipoOrdineST = "Data Scadenza Elettrodi,Articolo,Cliente,NSerie"
            TipoOrdine = "DataScadElettrodi,Cod_Articolo,Cod_CoGe,NSerie"
            TipoOrdineRPT = TipoStampaAICA.ElencoAICAScElArtCliNSerie
        End If
        '-
        If rbScBaArtCli.Checked = True Then
            TipoOrdineST = "Data Scadenza Batterie,Articolo,Cliente,NSerie"
            TipoOrdine = "DataScadBatterie,Cod_Articolo,Cod_CoGe,NSerie"
            TipoOrdineRPT = TipoStampaAICA.ElencoAICAScBaArtCliNSerie
        End If
        '-

        Dim SelScGa As Boolean = chkSelScGa.Checked
        Dim SelScEl As Boolean = chkSelScEl.Checked
        Dim SelScBa As Boolean = chkSelScBa.Checked
        Dim SelTutteCatCli As Boolean = chkTutteCatCli.Checked
        Dim SelRaggrCatCli As Boolean = chkRaggrCatCli.Checked
        Dim DataScadenzaDA As DateTime = CDate(txtDallaDataSc.Text).Date
        Dim DataScadenzaA As DateTime = CDate(txtAllaDataSc.Text).Date
        Dim DescCatCli As String = ddlCatCli.SelectedItem.Text.Trim
        Dim CodCategoria As Integer = ddlCatCli.SelectedValue

        'Imposto come data ultimo del mese selezionato
        Dim DaysInMonth As Integer = Date.DaysInMonth(DataScadenzaA.Year, DataScadenzaA.Month)
        Dim LastDayInMonthDate As Date = New Date(DataScadenzaA.Year, DataScadenzaA.Month, DaysInMonth)
        DataScadenzaA = LastDayInMonthDate
        '-

        If DataScadenzaA < DataScadenzaDA Then
            txtDallaDataSc.BackColor = SEGNALA_KO
            txtDallaDataSc.ToolTip = "Periodo data Scadenza errata"
            txtAllaDataSc.BackColor = SEGNALA_KO
            txtAllaDataSc.ToolTip = "Periodo data Scadenza errata"
            Exit Sub
        Else
            txtDallaDataSc.BackColor = SEGNALA_OK
            txtAllaDataSc.BackColor = SEGNALA_OK
        End If

        '-
        Session(CSTESPORTAPDF) = True   'imposto sessione esporta pdf a true

        'giu240514 @@@@@@@@@@@@@@@@@@@@@@@@@@ metodo classico
        ProgrammaticModalPopup.Hide()
        _WucElement.CallBackStampaElencoAI(TipoOrdineST, TipoOrdine, TipoOrdineRPT, DataScadenzaDA, DataScadenzaA, SelScGa, SelScEl, SelScBa, "", "", SelTutteCatCli, SelRaggrCatCli, DescCatCli, CodCategoria, False, True)
        Exit Sub
        '----------------------------------------------------
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ProgrammaticModalPopup.Hide()
        Session(F_STAMPAELENCOAI_APERTA) = False
    End Sub

    'Public Sub Hide()
    '    ProgrammaticModalPopup.Hide()
    'End Sub

    Private Sub chkTutteCatCli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteCatCli.CheckedChanged
        ddlCatCli.SelectedIndex = -1
        If chkTutteCatCli.Checked Then
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
        Else
            ddlCatCli.Enabled = True
            chkRaggrCatCli.Enabled = True
        End If
    End Sub

End Class