Imports System.Data.SqlClient
Imports It.SoftAzi.Model.Facade
Imports It.SoftAzi.Model.Entity
Imports It.SoftAzi.SystemFramework
Imports SoftAziOnLine.DataBaseUtility
Imports SoftAziOnLine.Def
Imports SoftAziOnLine.App
Imports SoftAziOnLine.Formatta
Imports SoftAziOnLine.WebFormUtility
Imports SoftAziOnLine.Magazzino

Imports System.Web.Services.WebService
Imports System.Web.Script.Serialization

Partial Public Class WUC_ParametriGen
    Inherits System.Web.UI.UserControl

    Private DSParGen As New DSParametriGenerali
    Private dvParGen As DataView

    Private SqlConn As SqlConnection
    Private SqlDAdap As SqlDataAdapter

    Private SqlDbSelectCmd As SqlCommand
    Private SqlDbUpdateCmd As SqlCommand

    Private Const F_CATEGORIE As String = "ElencoCategorie"
    'giu230819 giu231019 PER ORA HO COMMENTATO PERCHE' NON VA BENE !!!! DA VERIFICARE BENE
    ' ''Dim myNumero As Long = -1
    ' ''Dim strErrNum As String = ""
    ' ''Dim SWErrNum As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ModalPopup.WucElement = Me
        WFPElencoCategorie.WucElement = Me
        Dim strEser As String = Session(ESERCIZIO)
        If IsNothing(strEser) Then
            strEser = ""
        End If
        If String.IsNullOrEmpty(strEser) Then
            strEser = ""
        End If
        If strEser = "" Or Not IsNumeric(strEser) Then
            Chiudi("Errore: ESERCIZIO SCONOSCIUTO")
            Exit Sub
        End If

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        Session(DBCONNAZI) = dbCon.getConnectionString(TipoDB.dbSoftAzi)
        Session(DBCONNCOGE) = dbCon.getConnectionString(TipoDB.dbSoftCoge)

        SqlDataSourceAliquotaIva.ConnectionString = Session(DBCONNCOGE)
        SqlDSCausMag.ConnectionString = Session(DBCONNAZI)
        SqlDSCausaliCoGe.ConnectionString = Session(DBCONNCOGE)
        SqlDSTipoFatt.ConnectionString = Session(DBCONNAZI)
        SqlDSDurataTipo.ConnectionString = dbCon.getConnectionString(TipoDB.dbScadenzario)
        SqlDa_CatCli.ConnectionString = Session(DBCONNCOGE)
        SqlDSVettori.ConnectionString = Session(DBCONNCOGE)
        If Not IsPostBack Then
            Session(SWOP) = SWOPNESSUNA
            Dim strErrore As String = ""
            If Not LeggiParametri(strErrore) Then
                Chiudi(strErrore)
            End If
            ' ''If SWErrNum = False Then
            ' ''    TabContainer1.ActiveTabIndex = 0
            ' ''Else
            ' ''    TabContainer1.ActiveTabIndex = 1
            ' ''End If
        End If
        Select Case Session(F_ELENCO_APERTA)
            Case F_CATEGORIE
                WFPElencoCategorie.Show()
        End Select
    End Sub
    Private Sub ImpostaDAdp()
        SqlConn = New SqlConnection
        SqlDAdap = New SqlDataAdapter
        SqlDbSelectCmd = New SqlCommand
        SqlDbUpdateCmd = New SqlCommand

        Dim dbCon As New dbStringaConnesioneFacade(Session(ESERCIZIO))
        SqlConn.ConnectionString = dbCon.getConnectionString(TipoDB.dbSoftAzi)

        SqlDbSelectCmd.CommandText = "get_ParametriGeneraliAZI"
        SqlDbSelectCmd.CommandType = System.Data.CommandType.StoredProcedure
        SqlDbSelectCmd.Connection = Me.SqlConn
        SqlDbSelectCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        '
        'SqlUpdateCommand1
        '
        Me.SqlDbUpdateCmd.CommandText = "[update_ParametriGeneraliAZI]"
        Me.SqlDbUpdateCmd.CommandType = System.Data.CommandType.StoredProcedure
        Me.SqlDbUpdateCmd.Connection = Me.SqlConn
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, False, CType(10, Byte), CType(0, Byte), "", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RighePerPaginaDDT", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RighePerPaginaDDT", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RighePerPaginaFatt", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RighePerPaginaFatt", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroDDT", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroDDT", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroFattura", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroFattura", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroNotaAccredito", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroNotaAccredito", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroNotaCdenza", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroNotaCdenza", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteAccreditoNumerazioneSeparata", System.Data.SqlDbType.Bit, 1, "NoteAccreditoNumerazioneSeparata"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NoteCdenzaNumSep", System.Data.SqlDbType.Bit, 1, "NoteCdenzaNumSep"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IVATrasporto", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IVATrasporto", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AnteprimaStampa", System.Data.SqlDbType.Bit, 1, "AnteprimaStampa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RighePerPaginaORDINI", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RighePerPaginaORDINI", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceContoRicavoCOGE", System.Data.SqlDbType.NVarChar, 16, "CodiceContoRicavoCOGE"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceCausaleCOGE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceCausaleCOGE", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AggiornaMagazzino", System.Data.SqlDbType.Bit, 1, "AggiornaMagazzino"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DataUltimaCompattazione", System.Data.SqlDbType.DateTime, 4, "DataUltimaCompattazione"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroOrdineFornitore", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroOrdineFornitore", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroRiordinoFornitore", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroRiordinoFornitore", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceCausaleRiordino", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceCausaleRiordino", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@LarghezzaBolla", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "LarghezzaBolla", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MaxDescrizione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "MaxDescrizione", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoCorrispettivi", System.Data.SqlDbType.NVarChar, 16, "ContoCorrispettivi"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoCassa", System.Data.SqlDbType.NVarChar, 16, "ContoCassa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceCausaleIncasso", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceCausaleIncasso", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ChiedoListino", System.Data.SqlDbType.Bit, 1, "ChiedoListino"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceDescrizione", System.Data.SqlDbType.Bit, 1, "CodiceDescrizione"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@2Copie", System.Data.SqlDbType.Bit, 1, "2Copie"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceCausaleTrasferimento", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceCausaleTrasferimento", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IvaSpese", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IvaSpese", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceCausaleCorrisp", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceCausaleCorrisp", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceCausaleCOGENA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceCausaleCOGENA", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceCausaleIncassoNA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceCausaleIncassoNA", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoSpeseIncasso", System.Data.SqlDbType.NVarChar, 16, "ContoSpeseIncasso"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoSpeseTrasporto", System.Data.SqlDbType.NVarChar, 16, "ContoSpeseTrasporto"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoSpeseVarie", System.Data.SqlDbType.NVarChar, 16, "ContoSpeseVarie"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceCausaleTrasferimentoFiliale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodiceCausaleTrasferimentoFiliale", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StringaConai", System.Data.SqlDbType.NVarChar, 50, "StringaConai"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StringaBolla", System.Data.SqlDbType.NVarChar, 4, "StringaBolla"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AspettoDeiBeni", System.Data.SqlDbType.NVarChar, 50, "AspettoDeiBeni"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CalcoloColliAutomatico", System.Data.SqlDbType.Bit, 1, "CalcoloColliAutomatico"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PasswordMovimenti", System.Data.SqlDbType.NVarChar, 10, "PasswordMovimenti"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Iva_Imballo", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Iva_Imballo", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoSpeseImballo", System.Data.SqlDbType.NVarChar, 16, "ContoSpeseImballo"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DicituraASPEST", System.Data.SqlDbType.NVarChar, 50, "DicituraASPEST"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DicituraPORTO", System.Data.SqlDbType.NVarChar, 50, "DicituraPORTO"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroOrdineCliente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroOrdineCliente", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CarPerRiga", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CarPerRiga", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Decimali_Sconto", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Decimali_Sconto", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Decimali_Provvigione", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Decimali_Provvigione", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroPreventivo", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroPreventivo", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RighePerPaginaPrev", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "RighePerPaginaPrev", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoRiBa", System.Data.SqlDbType.NVarChar, 16, "ContoRiBa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausaleRiBa", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausaleRiBa", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RegIncasso", System.Data.SqlDbType.Bit, 1, "RegIncasso"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumSconti", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumSconti", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Num_Differenziata", System.Data.SqlDbType.Bit, 1, "Num_Differenziata"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Cod_Valuta", System.Data.SqlDbType.NVarChar, 4, "Cod_Valuta"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Decimali_Prezzi", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Decimali_Prezzi", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Visual_2_Valute", System.Data.SqlDbType.Bit, 1, "Visual_2_Valute"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@causaleMMpos", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "causaleMMpos", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@causaleMMneg", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "causaleMMneg", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Decimali_Prezzi_2", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Decimali_Prezzi_2", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodCausaleVendita", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CodCausaleVendita", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodiceNumerico", System.Data.SqlDbType.Bit, 1, "CodiceNumerico"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@LunghezzaMaxCodice", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "LunghezzaMaxCodice", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ValoreMinimoOrdine", System.Data.SqlDbType.Decimal, 9, System.Data.ParameterDirection.Input, False, CType(9, Byte), CType(4, Byte), "ValoreMinimoOrdine", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@gg_lavorativi_sett", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "gg_lavorativi_sett", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@giorno_riposo", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "giorno_riposo", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@sett_verifica_qta", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "sett_verifica_qta", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumScontiForn", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumScontiForn", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DecScontoForn", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "DecScontoForn", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ControlloSottoscorta", System.Data.SqlDbType.Bit, 1, "ControlloSottoscorta"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroSped", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroSped", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RegPNRB", System.Data.SqlDbType.Bit, 1, "RegPNRB"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@LivelloMaxDistBase", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "LivelloMaxDistBase", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CalcoloScontoSuImporto", System.Data.SqlDbType.Bit, 1, "CalcoloScontoSuImporto"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CodTipoFatt", System.Data.SqlDbType.NVarChar, 2, "CodTipoFatt"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausaleRipristinoSaldi", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausaleRipristinoSaldi", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DisabilitaRiordino", System.Data.SqlDbType.Bit, 1, "DisabilitaRiordino"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AnniFuoriProd", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "AnniFuoriProd", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroOrdineDaDeposito", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroOrdineDaDeposito", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausSBNatale", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausSBNatale", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausSBPasqua", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausSBPasqua", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausDDTDep", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausDDTDep", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausVendDep", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausVendDep", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausResoDep", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausResoDep", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PercorsoStampaOrdini", System.Data.SqlDbType.NVarChar, 255, "PercorsoStampaOrdini"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PercorsoStampaDDT", System.Data.SqlDbType.NVarChar, 255, "PercorsoStampaDDT"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PercorsoStampaFatt", System.Data.SqlDbType.NVarChar, 255, "PercorsoStampaFatt"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PercorsoStampaPrev", System.Data.SqlDbType.NVarChar, 255, "PercorsoStampaPrev"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausNCResi", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausNCResi", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausNCAbbuono", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausNCAbbuono", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausNCScontoOmesso", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausNCScontoOmesso", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausNCDiffPrezzo", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausNCDiffPrezzo", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@UltAgRicalcolato", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "UltAgRicalcolato", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausNCSBPrec", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausNCSBPrec", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroBC", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroBC", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@2CopieNZ", System.Data.SqlDbType.Bit, 1, "2CopieNZ"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausRimInizialeDep", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausRimInizialeDep", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ComunicazioneRintracciabilita", System.Data.SqlDbType.NVarChar, 1073741823, "ComunicazioneRintracciabilita"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PieDiPaginaRintracciabilita", System.Data.SqlDbType.NVarChar, 1073741823, "PieDiPaginaRintracciabilita"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SMTPServer", System.Data.SqlDbType.NVarChar, 255, "SMTPServer"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SMTPPorta", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "SMTPPorta", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SMTPUserName", System.Data.SqlDbType.NVarChar, 255, "SMTPUserName"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SMTPPassword", System.Data.SqlDbType.NVarChar, 255, "SMTPPassword"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SMTPMailSender", System.Data.SqlDbType.NVarChar, 255, "SMTPMailSender"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DettaglioCestiDDT", System.Data.SqlDbType.Bit, 1, "DettaglioCestiDDT"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroOCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroOCL", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausOrdCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausOrdCL", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausDDTCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausDDTCL", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausResoCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausResoCL", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausFineCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausFineCL", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausRestiCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausRestiCL", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CausCarMagCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "CausCarMagCL", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ListinoCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "ListinoCL", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Banca", System.Data.SqlDbType.NVarChar, 50, "Banca"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ABI", System.Data.SqlDbType.NVarChar, 5, "ABI"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CAB", System.Data.SqlDbType.NVarChar, 5, "CAB"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CIN", System.Data.SqlDbType.NVarChar, 1, "CIN"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CC", System.Data.SqlDbType.NVarChar, 15, "CC"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NazIBAN", System.Data.SqlDbType.NVarChar, 3, "NazIBAN"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CINEUIBAN", System.Data.SqlDbType.NVarChar, 2, "CINEUIBAN"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SWIFT", System.Data.SqlDbType.NVarChar, 15, "SWIFT"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrezziDDT", System.Data.SqlDbType.Bit, 1, "PrezziDDT"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NUltimiPrezziAcq", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NUltimiPrezziAcq", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Decimali_Grandezze", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "Decimali_Grandezze", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NGG_Validita", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NGG_Validita", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NGG_Consegna", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NGG_Consegna", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroFA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroFA", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroPA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroPA", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroNCPA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroNCPA", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@NumeroNCPASep", System.Data.SqlDbType.Bit, 1, "NumeroNCPASep"))
        'giu300718
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SelAICatCli", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "SelAICatCli", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SelAIDaData", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "SelAIDaData", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SelAIAData", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "SelAIAData", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AIServizioEmail", System.Data.SqlDbType.Bit, 1, "AIServizioEmail"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SelAIScGa", System.Data.SqlDbType.Bit, 1, "SelAIScGa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SelAIScEl", System.Data.SqlDbType.Bit, 1, "SelAIScEl"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SelAIScBa", System.Data.SqlDbType.Bit, 1, "SelAIScBa"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ScCassaDett", System.Data.SqlDbType.Bit, 1, "ScCassaDett"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ImpMinBollo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "ImpMinBollo", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IVABollo", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IVABollo", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@IVAScMerce", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "IVAScMerce", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ContoRitAcconto", System.Data.SqlDbType.NVarChar, 16, "ContoRitAcconto"))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Bollo", System.Data.SqlDbType.Money, 4, System.Data.ParameterDirection.Input, False, CType(19, Byte), CType(0, Byte), "Bollo", System.Data.DataRowVersion.Current, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AIServizioEmailAttiva", System.Data.SqlDbType.NChar, 5, "AIServizioEmailAttiva"))

        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroDDT", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroDDT", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroFattura", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroFattura", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroNotaAccredito", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroNotaAccredito", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroNotaCdenza", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroNotaCdenza", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroOrdineFornitore", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroOrdineFornitore", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroRiordinoFornitore", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroRiordinoFornitore", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroOrdineCliente", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroOrdineCliente", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroPreventivo", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroPreventivo", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroSped", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroSped", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroOrdineDaDeposito", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroOrdineDaDeposito", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroBC", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroBC", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroOCL", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroOCL", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroFA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroFA", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroPA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroPA", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDbUpdateCmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_NumeroNCPA", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(10, Byte), CType(0, Byte), "NumeroNCPA", System.Data.DataRowVersion.Original, Nothing))

        SqlDAdap.SelectCommand = SqlDbSelectCmd
        SqlDAdap.UpdateCommand = SqlDbUpdateCmd

        Session("SqlDAdapParGen") = SqlDAdap
        Session("SqlDbSelectCmdParGen") = SqlDbSelectCmd
        Session("SqlDbUpdateCmdParGen") = SqlDbUpdateCmd
    End Sub

    Public Sub Chiudi(ByVal strErrore As String)
        If strErrore.Trim <> "" Then
            Try
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            Catch ex As Exception
                Response.Redirect("WF_Menu.aspx?labelForm=" & strErrore.Trim)
            End Try
            Exit Sub
        End If
    End Sub

    Private Sub SetEnabledTo(ByVal SW As Boolean)
        rbtnFranco.Enabled = SW
        rbtnAssegnato.Enabled = SW
        txtAspettoEst.Enabled = SW
        TxtConai.Enabled = SW
        DDLVettore1.Enabled = SW
        txtEmailSpedDDT.Enabled = SW
        txtTelSpedDDT.Enabled = SW
        txtCPagCA.Enabled = SW
        txtAccountSpedDDT.Enabled = SW
        TxtPassword.Enabled = SW
        TxtPasswordConferma.Enabled = SW
        DDLTipoFatturazione.Enabled = SW
        DDLDurataTipo.Enabled = SW
        rbtnScontoImporto.Enabled = SW
        rbtnScontoPrezzo.Enabled = SW
        rbtnScCassaTotDoc.Enabled = SW
        rbtnScCassaTotRiga.Enabled = SW
        chkNCNumSep.Enabled = SW : chkNCPASep.Enabled = SW 'giu220714
        txtUltFC.Enabled = SW
        txtUltFattAcc.Enabled = SW : txtUltFattPA.Enabled = SW 'GIU210714
        txtUltNC.Enabled = SW : txtUltNCPA.Enabled = SW 'GIU220714
        txtUltOC.Enabled = SW
        txtUltOF.Enabled = SW
        txtUltRF.Enabled = SW
        txtUltPrev.Enabled = SW
        txtUltSped.Enabled = SW
        txtUltDDT.Enabled = SW 'GIU230614
        ddlDecPercSc.Enabled = SW
        ddlDecPrecProvv.Enabled = SW
        ddlNumScDoc.Enabled = SW
        txtBancaAppoggio.Enabled = SW
        txtABIBancaAppoggio.Enabled = SW
        txtCABBancaAppoggio.Enabled = SW
        txtNCCBancaAppoggio.Enabled = SW
        txtCINBancaAppoggio.Enabled = SW
        txtNazIBAN.Enabled = SW
        txtCINEu.Enabled = SW
        txtSwift.Enabled = SW
        ddlCausFC.Enabled = SW
        ddlCausNC.Enabled = SW
        ddlCausCO.Enabled = SW
        ddlCausIN.Enabled = SW
        ddlCausPagNC.Enabled = SW
        ddlCausRiBa.Enabled = SW
        txtContoCassa.Enabled = SW
        txtContoRicavo.Enabled = SW
        txtContoCorr.Enabled = SW
        txtContoSpeseInc.Enabled = SW
        ddlAliqIVAInc.Enabled = SW
        txtContoSpeseVarie.Enabled = SW
        txtContoSpeseImb.Enabled = SW
        ddlAliqIVAImb.Enabled = SW
        txtContoSpeseTrasp.Enabled = SW
        ddlAliqVATrasp.Enabled = SW
        txtContoRiBa.Enabled = SW
        chkRegAutoInc.Enabled = SW
        chkRegRBPrimaNota.Enabled = SW
        ddlCausRiprSaldi.Enabled = SW
        DDLCausRiordino.Enabled = SW
        DDLCausMMPos.Enabled = SW
        DDLCausMMNeg.Enabled = SW
        'C/Vendita - Resi
        DDLCausVendita.Enabled = SW
        DDLCausResi.Enabled = SW
        'Contratti
        DDLCausCAMDAE.Enabled = SW
        DDLCausCALoc.Enabled = SW
        DDLCausCATelC.Enabled = SW
        'C/Deposito
        ddlDDTDep.Enabled = SW
        ddlVendDep.Enabled = SW
        ddlResoDep.Enabled = SW
        ddlRimIniz.Enabled = SW
        'giu260219
        ddlAliqIVABollo.Enabled = SW
        txtImpMinBollo.Enabled = SW
        ddlAliqIVAScMerce.Enabled = SW
        txtBollo.Enabled = SW
        txtContoRitAcconto.Enabled = SW
        'giu310718
        PanelScadenze.Enabled = SW
        If SW = True Then
            If chkTutteCatCli.Checked = False And chkSelCategorie.Checked = False Then
                ddlCatCli.Enabled = True
                chkRaggrCatCli.Enabled = True
            Else
                ddlCatCli.Enabled = False
                chkRaggrCatCli.Enabled = False
            End If
        End If
        PanelScadenzeDaA.Enabled = SW
        PanelEmail.Enabled = SW
        PanelAlertMenu.Enabled = SW
    End Sub
    Private Function LeggiParametri(ByRef _Errore As String) As Boolean
        btnAggiorna.Text = "Modifica"
        btnAnnulla.Enabled = False
        SetEnabledTo(False)
        _Errore = ""
        Dim strErrore As String = "" : Dim strValore As String = ""
        ' ''strErrNum = ""
        ' ''SWErrNum = False
        ImpostaDAdp()
        Dim strIBAN As String
        Dim strBBAN As String
        LeggiParametri = True
        Dim row As DSParametriGenerali.ParametriGeneraliAZIRow
        Try
            DSParGen.Clear()
            SqlDAdap.Fill(DSParGen.ParametriGeneraliAZI)
            DSParGen.AcceptChanges()
            dvParGen = New DataView(DSParGen.ParametriGeneraliAZI)
            Session("dvParGen") = dvParGen
            Session("DSParGen") = DSParGen
            Session("SqlDAdapParGen") = SqlDAdap
            Session("SqlDbSelectCmdParGen") = SqlDbSelectCmd
            Session("SqlDbUpdateCmdParGen") = SqlDbUpdateCmd
            row = DSParGen.ParametriGeneraliAZI.Rows(0)
            'porto franco/assegnato
            If Not row.IsDicituraPORTONull Then
                If row.DicituraPORTO.ToUpper = "A" Then
                    rbtnAssegnato.Checked = True
                ElseIf row.DicituraPORTO.ToUpper = "F" Then
                    rbtnFranco.Checked = True
                End If
            Else
                rbtnFranco.Checked = False
                rbtnAssegnato.Checked = False
            End If
            'aspetto esteriore
            If Not row.IsDicituraASPESTNull Then
                txtAspettoEst.Text = row.DicituraASPEST
            Else
                txtAspettoEst.Text = ""
            End If
            'descrizione CONAI
            If Not row.IsStringaConaiNull Then
                TxtConai.Text = row.StringaConai
            Else
                TxtConai.Text = ""
            End If
            '-
            'giu200722
            Call GetDatiAbilitazioni(CSTABILAZI, "SpedVettCsv", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If strValore.Trim <> "" Then
                PosizionaItemDDL(strValore.Trim, DDLVettore1)
            Else
                DDLVettore1.SelectedIndex = 0
            End If
            '-
            Call GetDatiAbilitazioni(CSTABILAZI, "AccountSped", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            txtAccountSpedDDT.Text = strValore.Trim
            '-
            Call GetDatiAbilitazioni(CSTABILAZI, "EmailSped", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            txtEmailSpedDDT.Text = strValore.Trim
            '-
            Call GetDatiAbilitazioni(CSTABILAZI, "TelSped", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            txtTelSpedDDT.Text = strValore.Trim
            '-
            Call GetDatiAbilitazioni(CSTABILAZI, "CPagCA", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            txtCPagCA.Text = strValore.Trim
            'password movimenti
            If Not row.IsPasswordMovimentiNull Then
                TxtPassword.Text = row.PasswordMovimenti
                TxtPasswordConferma.Text = row.PasswordMovimenti
            Else
                TxtPassword.Text = ""
                TxtPasswordConferma.Text = ""
            End If

            'tipo fatturazione predef.
            If Not row.IsCodTipoFattNull Then
                PosizionaItemDDL(row.CodTipoFatt, DDLTipoFatturazione)
            Else
                DDLTipoFatturazione.SelectedIndex = 0
            End If

            'calcolo sconto nell'importo di riga dei dettagli
            If row.CalcoloScontoSuImporto Then
                rbtnScontoImporto.Checked = True
            Else
                rbtnScontoPrezzo.Checked = True
            End If
            'calcolo sconto cassa 
            If row.ScCassaDett Then
                rbtnScCassaTotRiga.Checked = True
            Else
                rbtnScCassaTotDoc.Checked = True
            End If

            '' ''note corrispondenza con num. sep.
            ' ''chkNCorrNumSep.Checked = CBool(row.NoteCdenzaNumSep)

            'note credito con num. sep.
            chkNCNumSep.Checked = CBool(row.NoteAccreditoNumerazioneSeparata)
            'note credito PA con num. sep. GIU220714
            chkNCPASep.Checked = CBool(row.NumeroNCPASep)
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'CONTROLLO SU DOCUMENTIT se il numero coincide a quello memorizzato in ParGen
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'ultima fattura
            If Not row.IsNumeroFatturaNull Then
                txtUltFC.Text = row.NumeroFattura
            Else
                txtUltFC.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.FatturaCommerciale), False, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltFC.BackColor = SEGNALA_KO : txtUltFC.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltFC.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltFC.BackColor = SEGNALA_KO : txtUltFC.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltFC.BackColor = SEGNALA_OK : txtUltFC.ToolTip = ""
            ' ''End If

            'GIU210714 Fattura accompagnatoria (non usato per adesso)
            'ultima fattura
            If Not row.IsNumeroFANull Then
                txtUltFattAcc.Text = row.NumeroFA
            Else
                txtUltFattAcc.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.FatturaAccompagnatoria), False, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltFattAcc.BackColor = SEGNALA_KO : txtUltFattAcc.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltFattAcc.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltFattAcc.BackColor = SEGNALA_KO : txtUltFattAcc.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltFattAcc.BackColor = SEGNALA_OK : txtUltFattAcc.ToolTip = ""
            ' ''End If

            'ultima fattura PA
            If Not row.IsNumeroPANull Then
                txtUltFattPA.Text = row.NumeroPA
            Else
                txtUltFattPA.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.FatturaCommerciale), True, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltFattPA.BackColor = SEGNALA_KO : txtUltFattPA.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltFattPA.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltFattPA.BackColor = SEGNALA_KO : txtUltFattPA.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltFattPA.BackColor = SEGNALA_OK : txtUltFattPA.ToolTip = ""
            ' ''End If
            '----------------
            'ultima note di credito
            If Not row.IsNumeroNotaAccreditoNull Then
                txtUltNC.Text = row.NumeroNotaAccredito
            Else
                txtUltNC.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.NotaCredito), False, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltNC.BackColor = SEGNALA_KO : txtUltNC.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltNC.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltNC.BackColor = SEGNALA_KO : txtUltNC.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltNC.BackColor = SEGNALA_OK : txtUltNC.ToolTip = ""
            ' ''End If
            '-
            'GIU220714 ultima note di credito PA
            If Not row.IsNumeroNCPANull Then
                txtUltNCPA.Text = row.NumeroNCPA
            Else
                txtUltNCPA.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.NotaCredito), True, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltNCPA.BackColor = SEGNALA_KO : txtUltNCPA.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltNCPA.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltNCPA.BackColor = SEGNALA_KO : txtUltNCPA.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltNCPA.BackColor = SEGNALA_OK : txtUltNCPA.ToolTip = ""
            ' ''End If
            '-
            'ultimo ordine cliente
            If Not row.IsNumeroOrdineClienteNull Then
                txtUltOC.Text = row.NumeroOrdineCliente 'numeroOCL?
            Else
                txtUltOC.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.OrdClienti), False, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltOC.BackColor = SEGNALA_KO : txtUltOC.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltOC.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltOC.BackColor = SEGNALA_KO : txtUltOC.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltOC.BackColor = SEGNALA_OK : txtUltOC.ToolTip = ""
            ' ''End If
            '-
            'ultimo ordine fornitore
            If Not row.IsNumeroOrdineFornitoreNull Then
                txtUltOF.Text = row.NumeroOrdineFornitore
            Else
                txtUltOF.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.OrdFornitori), False, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltOF.BackColor = SEGNALA_KO : txtUltOF.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltOF.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltOF.BackColor = SEGNALA_KO : txtUltOF.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltOF.BackColor = SEGNALA_OK : txtUltOF.ToolTip = ""
            ' ''End If
            '-
            'ultimo riordine fornitore
            If Not row.IsNumeroRiordinoFornitoreNull Then
                txtUltRF.Text = row.NumeroRiordinoFornitore
            Else
                txtUltRF.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.PropOrdFornitori), False, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltRF.BackColor = SEGNALA_KO : txtUltRF.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltRF.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltRF.BackColor = SEGNALA_KO : txtUltRF.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltRF.BackColor = SEGNALA_OK : txtUltRF.ToolTip = ""
            ' ''End If
            '-
            'ultimo preventivo
            If Not row.IsNumeroPreventivoNull Then
                txtUltPrev.Text = row.NumeroPreventivo
            Else
                txtUltPrev.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.Preventivi), False, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltPrev.BackColor = SEGNALA_KO : txtUltPrev.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltPrev.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltPrev.BackColor = SEGNALA_KO : txtUltPrev.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltPrev.BackColor = SEGNALA_OK : txtUltPrev.ToolTip = ""
            ' ''End If
            '-
            'ultima spedizione
            If Not row.IsNumeroSpedNull Then
                txtUltSped.Text = row.NumeroSped
            Else
                txtUltSped.Text = "0"
            End If

            'ultimo DDT a Cliente GIU230614
            If Not row.IsNumeroDDTNull Then
                txtUltDDT.Text = row.NumeroDDT
            Else
                txtUltDDT.Text = "0"
            End If
            ' ''myNumero = CheckNumDoc(SWTD(TD.DocTrasportoClienti), False, strErrNum)
            ' ''If strErrNum.Trim <> "" Then
            ' ''    txtUltDDT.BackColor = SEGNALA_KO : txtUltDDT.ToolTip = strErrNum.Trim : SWErrNum = True
            ' ''ElseIf myNumero <> CLng(txtUltDDT.Text) Then
            ' ''    strErrNum = "N° previsto: " & myNumero.ToString.Trim
            ' ''    txtUltDDT.BackColor = SEGNALA_KO : txtUltDDT.ToolTip = strErrNum : SWErrNum = True
            ' ''Else
            ' ''    txtUltDDT.BackColor = SEGNALA_OK : txtUltDDT.ToolTip = ""
            ' ''End If
            '-
            'decimali % sconto
            If Not row.IsDecimali_ScontoNull Then
                PosizionaItemDDL(row.Decimali_Sconto, ddlDecPercSc)
            Else
                ddlDecPercSc.SelectedIndex = 0
            End If

            'decimali % provv
            If Not row.IsDecimali_ProvvigioneNull Then
                PosizionaItemDDL(row.Decimali_Provvigione, ddlDecPrecProvv)
            Else
                ddlDecPrecProvv.SelectedIndex = 0
            End If

            'numero sconti in doc.
            If Not row.IsNumScontiNull Then
                PosizionaItemDDL(row.NumSconti, ddlNumScDoc)
            Else
                ddlNumScDoc.SelectedIndex = 0
            End If

            'banca appoggio
            If Not row.IsBancaNull Then
                txtBancaAppoggio.Text = row.Banca
            Else
                txtBancaAppoggio.Text = ""
            End If

            'ABI
            If Not row.IsABINull Then
                txtABIBancaAppoggio.Text = row.ABI
            Else
                txtABIBancaAppoggio.Text = ""
            End If

            'CAB
            If Not row.IsCABNull Then
                txtCABBancaAppoggio.Text = row.CAB
            Else
                txtCABBancaAppoggio.Text = ""
            End If

            'N° C/C
            If Not row.IsCCNull Then
                txtNCCBancaAppoggio.Text = row.CC
            Else
                txtNCCBancaAppoggio.Text = ""
            End If

            'CIN
            If Not row.IsCINNull Then
                txtCINBancaAppoggio.Text = row.CIN
            Else
                txtCINBancaAppoggio.Text = ""
            End If

            'nazione
            If Not row.IsNazIBANNull Then
                txtNazIBAN.Text = row.NazIBAN
            Else
                txtNazIBAN.Text = ""
            End If

            'CIN EU
            If Not row.IsCINEUIBANNull Then
                txtCINEu.Text = row.CINEUIBAN
            Else
                txtCINEu.Text = ""
            End If

            'IBAN/BBAN
            If txtNazIBAN.Text.Trim = "" Then
                strIBAN = "[Nazione] "
            Else
                strIBAN = txtNazIBAN.Text.Trim & " "
            End If

            If txtCINEu.Text.Trim = "" Then
                strIBAN &= "[CIN Eu.] "
            Else
                strIBAN &= txtCINEu.Text.Trim & " "
            End If

            If txtCINBancaAppoggio.Text.Trim = "" Then
                strIBAN &= "[CIN] "
                strBBAN = "[CIN] "
            Else
                strIBAN &= txtCINBancaAppoggio.Text.Trim & " "
                strBBAN = txtCINBancaAppoggio.Text.Trim & " "
            End If

            If txtABIBancaAppoggio.Text.Trim = "" Then
                strIBAN &= "[ABI] "
                strBBAN &= "[ABI] "
            Else
                strIBAN &= txtABIBancaAppoggio.Text.Trim & " "
                strBBAN &= txtABIBancaAppoggio.Text.Trim & " "
            End If

            If txtCABBancaAppoggio.Text.Trim = "" Then
                strIBAN &= "[CAB] "
                strBBAN &= "[CAB] "
            Else
                strIBAN &= txtCABBancaAppoggio.Text.Trim & " "
                strBBAN &= txtCABBancaAppoggio.Text.Trim & " "
            End If

            If txtNCCBancaAppoggio.Text.Trim = "" Then
                strIBAN &= "[CC] "
                strBBAN &= "[CC] "
            Else
                strIBAN &= txtNCCBancaAppoggio.Text.Trim
                strBBAN &= txtNCCBancaAppoggio.Text.Trim
            End If

            txtIBAN.Text = strIBAN
            txtBBAN.Text = strBBAN

            'codice SWIFT
            If Not row.IsSWIFTNull Then
                txtSwift.Text = row.SWIFT
            Else
                txtSwift.Text = ""
            End If

            'causale fatture
            If Not row.IsCodiceCausaleCOGENull Then
                PosizionaItemDDL(row.CodiceCausaleCOGE, ddlCausFC)
            Else
                ddlCausFC.SelectedIndex = 0
            End If

            'causale note di credito
            If Not row.IsCodiceCausaleCOGENANull Then
                PosizionaItemDDL(row.CodiceCausaleCOGENA, ddlCausNC)
            Else
                ddlCausNC.SelectedIndex = 0
            End If

            'causale corrispettivi
            If Not row.IsCodiceCausaleCorrispNull Then
                PosizionaItemDDL(row.CodiceCausaleCorrisp, ddlCausCO)
            Else
                ddlCausCO.SelectedIndex = 0
            End If

            'causale incassi
            If Not row.IsCodiceCausaleIncassoNull Then
                PosizionaItemDDL(row.CodiceCausaleIncasso, ddlCausIN)
            Else
                ddlCausIN.SelectedIndex = 0
            End If

            'causale pagamento note di credito
            If Not row.IsCodiceCausaleIncassoNANull Then
                PosizionaItemDDL(row.CodiceCausaleIncassoNA, ddlCausPagNC)
            Else
                ddlCausPagNC.SelectedIndex = 0
            End If

            'causale RiBa
            If Not row.IsCausaleRiBaNull Then
                PosizionaItemDDL(row.CausaleRiBa, ddlCausRiBa)
            Else
                ddlCausRiBa.SelectedIndex = 0
            End If

            'conto cassa
            If Not row.IsContoCassaNull Then
                txtContoCassa.Text = row.ContoCassa
            Else
                txtContoCassa.Text = ""
            End If
            txtDContoCassa.Text = App.GetValoreFromChiave(txtContoCassa.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))

            'conto ricavo
            If Not row.IsCodiceContoRicavoCOGENull Then
                txtContoRicavo.Text = row.CodiceContoRicavoCOGE
            Else
                txtContoRicavo.Text = ""
            End If
            txtDContoRicavo.Text = App.GetValoreFromChiave(txtContoRicavo.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))

            'conto corrispettivi
            If Not row.IsContoCorrispettiviNull Then
                txtContoCorr.Text = row.ContoCorrispettivi
            Else
                txtContoCorr.Text = ""
            End If
            txtDContoCorr.Text = App.GetValoreFromChiave(txtContoCorr.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))

            'conto spese incasso
            If Not row.IsContoSpeseIncassoNull Then
                txtContoSpeseInc.Text = row.ContoSpeseIncasso
            Else
                txtContoSpeseInc.Text = ""
            End If
            txtDContoSpeseInc.Text = App.GetValoreFromChiave(txtContoSpeseInc.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))

            If Not row.IsIvaSpeseNull Then
                PosizionaItemDDL(row.IvaSpese, ddlAliqIVAInc)
            Else
                ddlAliqIVAInc.SelectedIndex = -1
            End If

            'conto spese varie bollo
            If Not row.IsContoSpeseVarieNull Then
                txtContoSpeseVarie.Text = row.ContoSpeseVarie
            Else
                txtContoSpeseVarie.Text = ""
            End If
            txtDContoSpeseVarie.Text = App.GetValoreFromChiave(txtContoSpeseVarie.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))
            If Not row.IsIVABolloNull Then
                PosizionaItemDDL(row.IVABollo, ddlAliqIVABollo)
            Else
                ddlAliqIVABollo.SelectedIndex = -1
            End If
            If Not row.IsImpMinBolloNull Then
                txtImpMinBollo.Text = row.ImpMinBollo
            Else
                txtImpMinBollo.Text = ""
            End If
            If Not row.IsBolloNull Then
                txtBollo.Text = row.Bollo
            Else
                txtBollo.Text = ""
            End If
            'sconto merce IVA
            If Not row.IsIVAScMerceNull Then
                PosizionaItemDDL(row.IVAScMerce, ddlAliqIVAScMerce)
            Else
                ddlAliqIVAScMerce.SelectedIndex = -1
            End If
            '----------------------------
            'conto spese imballo
            If Not row.IsContoSpeseImballoNull Then
                txtContoSpeseImb.Text = row.ContoSpeseImballo
            Else
                txtContoSpeseImb.Text = ""
            End If
            txtDContoSpeseImb.Text = App.GetValoreFromChiave(txtContoSpeseImb.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))
            If Not row.IsIva_ImballoNull Then
                PosizionaItemDDL(row.Iva_Imballo, ddlAliqIVAImb)
            Else
                ddlAliqIVAImb.SelectedIndex = -1
            End If

            'conto spese trasporto
            If Not row.IsContoSpeseTrasportoNull Then
                txtContoSpeseTrasp.Text = row.ContoSpeseTrasporto
            Else
                txtContoSpeseTrasp.Text = ""
            End If
            txtDContoSpeseTrasp.Text = App.GetValoreFromChiave(txtContoSpeseTrasp.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))
            If Not row.IsIVATrasportoNull Then
                PosizionaItemDDL(row.IVATrasporto, ddlAliqVATrasp)
            Else
                ddlAliqVATrasp.SelectedIndex = -1
            End If

            'conto RiBa
            If Not row.IsContoRiBaNull Then
                txtContoRiBa.Text = row.ContoRiBa
            Else
                txtContoRiBa.Text = ""
            End If
            txtDContoRiBa.Text = App.GetValoreFromChiave(txtContoRiBa.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))

            'conto Ritenute d'acconto
            If Not row.IsContoRitAccontoNull Then
                txtContoRitAcconto.Text = row.ContoRitAcconto
            Else
                txtContoRitAcconto.Text = ""
            End If
            txtDContoRitAcconto.Text = App.GetValoreFromChiave(txtContoRitAcconto.Text.Trim, Def.PIANODEICONTI, Session(ESERCIZIO))

            'registrazione auto incassi
            chkRegAutoInc.Checked = CBool(row.RegIncasso)

            'registrazione RiBa in PN
            chkRegRBPrimaNota.Checked = CBool(row.RegPNRB)

            '' ''giorni lavorativi
            ' ''If Not row.Isgg_lavorativi_settNull Then
            ' ''    txtGiorniLavorativi.Text = row.gg_lavorativi_sett
            ' ''Else
            ' ''    txtGiorniLavorativi.Text = ""
            ' ''End If

            '' ''giorno riposo
            ' ''If Not row.Isgiorno_riposoNull Then
            ' ''    ddlRiposoSett.SelectedValue = row.giorno_riposo
            ' ''Else
            ' ''    ddlRiposoSett.SelectedIndex = -1
            ' ''End If

            '' ''n° settimane creaz. auto ordini
            ' ''If Not row.Issett_verifica_qtaNull Then
            ' ''    txtNSettOrd.Text = row.sett_verifica_qta
            ' ''Else
            ' ''    txtNSettOrd.Text = ""
            ' ''End If

            'Magazzino
            If Not row.IsCausaleRipristinoSaldiNull Then
                PosizionaItemDDL(row.CausaleRipristinoSaldi, ddlCausRiprSaldi)
            Else
                ddlCausRiprSaldi.SelectedIndex = 0
            End If
            'causale riordino fornitore
            If Not row.IsCodiceCausaleRiordinoNull Then
                PosizionaItemDDL(row.CodiceCausaleRiordino, DDLCausRiordino)
            Else
                DDLCausRiordino.SelectedIndex = 0
            End If
            If Not row.IscausaleMMposNull Then
                PosizionaItemDDL(row.causaleMMpos, DDLCausMMPos)
            Else
                DDLCausMMPos.SelectedIndex = 0
            End If
            If Not row.IscausaleMMnegNull Then
                PosizionaItemDDL(row.causaleMMneg, DDLCausMMNeg)
            Else
                DDLCausMMNeg.SelectedIndex = 0
            End If
            'Documenti
            If Not row.IsCodCausaleVenditaNull Then
                PosizionaItemDDL(row.CodCausaleVendita, DDLCausVendita)
            Else
                DDLCausVendita.SelectedIndex = 0
            End If
            If Not row.IsCausNCResiNull Then
                PosizionaItemDDL(row.CausNCResi, DDLCausResi)
            Else
                DDLCausResi.SelectedIndex = 0
            End If
            'Contratti
            Call GetDatiAbilitazioni(CSTABILAZI, "DurataTipo", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If strValore.Trim <> "" Then
                PosizionaItemDDL(strValore.Trim, DDLDurataTipo)
            Else
                DDLDurataTipo.SelectedIndex = 0
            End If
            '-
            Call GetDatiAbilitazioni(CSTABILAZI, "ConAssMDAE", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If strValore.Trim <> "" Then
                PosizionaItemDDL(strValore.Trim, DDLCausCAMDAE)
            Else
                DDLCausCAMDAE.SelectedIndex = 0
            End If
            'DDLCausCALoc.Enabled = SW
            Call GetDatiAbilitazioni(CSTABILAZI, "ConAssLoc", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If strValore.Trim <> "" Then
                PosizionaItemDDL(strValore.Trim, DDLCausCALoc)
            Else
                DDLCausCALoc.SelectedIndex = 0
            End If
            'DDLCausCATelC.Enabled = SW
            Call GetDatiAbilitazioni(CSTABILAZI, "ConAssTelC", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            If strValore.Trim <> "" Then
                PosizionaItemDDL(strValore.Trim, DDLCausCATelC)
            Else
                DDLCausCATelC.SelectedIndex = 0
            End If

            '' ''anni predef fine produzione
            ' ''If Not row.IsAnniFuoriProdNull Then
            ' ''    txtAnniFineProd.Text = row.AnniFuoriProd
            ' ''Else
            ' ''    txtAnniFineProd.Text = ""
            ' ''End If

            'causale DDT a deposito
            If Not row.IsCausDDTDepNull Then
                PosizionaItemDDL(row.CausDDTDep, ddlDDTDep)
            Else
                ddlDDTDep.SelectedIndex = 0
            End If

            'causale vendita da deposito
            If Not row.IsCausVendDepNull Then
                PosizionaItemDDL(row.CausVendDep, ddlVendDep)
            Else
                ddlVendDep.SelectedIndex = 0
            End If

            'causale reso da deposito
            If Not row.IsCausResoDepNull Then
                PosizionaItemDDL(row.CausResoDep, ddlResoDep)
            Else
                ddlResoDep.SelectedIndex = 0
            End If

            'causale rimanenza iniziale
            If Not row.IsCausRimInizialeDepNull Then
                PosizionaItemDDL(row.CausRimInizialeDep, ddlRimIniz)
            Else
                ddlRimIniz.SelectedIndex = 0
            End If
            'giu310718 PanelScadenzeEmail
            If Not row.IsSelAICatCliNull Then
                If row.SelAICatCli = -1 Then 'TUTTE LE CATEGORIE
                    ddlCatCli.SelectedIndex = -1
                    chkRaggrCatCli.Checked = False
                    chkSelCategorie.AutoPostBack = False
                    chkTutteCatCli.AutoPostBack = False
                    chkSelCategorie.Checked = False
                    chkTutteCatCli.Checked = True
                    chkSelCategorie.AutoPostBack = True
                    chkTutteCatCli.AutoPostBack = True
                ElseIf row.SelAICatCli = 0 Then 'SELEZIONE MULTIPLA CATEGORIE
                    ddlCatCli.SelectedIndex = -1
                    chkRaggrCatCli.Checked = False
                    chkSelCategorie.AutoPostBack = False
                    chkTutteCatCli.AutoPostBack = False
                    chkSelCategorie.Checked = True
                    chkTutteCatCli.Checked = False
                    chkSelCategorie.AutoPostBack = True
                    chkTutteCatCli.AutoPostBack = True
                ElseIf row.SelAICatCli > 0 Then 'SELEZIONE SINGOLA CATEGORIA
                    PosizionaItemDDL(row.SelAICatCli, ddlCatCli)
                    chkRaggrCatCli.Checked = False
                    chkSelCategorie.AutoPostBack = False
                    chkTutteCatCli.AutoPostBack = False
                    chkSelCategorie.Checked = False
                    chkTutteCatCli.Checked = False
                    chkSelCategorie.AutoPostBack = True
                    chkTutteCatCli.AutoPostBack = True
                ElseIf row.SelAICatCli < 0 Then 'SELEZIONE SINGOLA CATEGORIA RAGGRUPPATA
                    PosizionaItemDDL(row.SelAICatCli * -1, ddlCatCli)
                    chkRaggrCatCli.Checked = True
                    chkSelCategorie.AutoPostBack = False
                    chkTutteCatCli.AutoPostBack = False
                    chkSelCategorie.Checked = False
                    chkTutteCatCli.Checked = False
                    chkSelCategorie.AutoPostBack = True
                    chkTutteCatCli.AutoPostBack = True
                End If
            Else
                ddlCatCli.SelectedIndex = -1
                chkRaggrCatCli.Checked = False
                chkSelCategorie.AutoPostBack = False
                chkTutteCatCli.AutoPostBack = False
                chkSelCategorie.Checked = False
                chkTutteCatCli.Checked = False
                chkSelCategorie.AutoPostBack = True
                chkTutteCatCli.AutoPostBack = True
            End If
            If Not row.IsSelAIDaDataNull Then
                txtDallaData.Text = row.SelAIDaData
            Else
                txtDallaData.Text = "0"
            End If
            If Not row.IsSelAIADataNull Then
                txtAllaData.Text = row.SelAIAData
            Else
                txtAllaData.Text = "0"
            End If
            If row.IsSelAIScGaNull Then
                chkSelScGa.Checked = False
            Else
                chkSelScGa.Checked = row.SelAIScGa
            End If
            If row.IsSelAIScElNull Then
                chkSelScEl.Checked = False
            Else
                chkSelScEl.Checked = row.SelAIScEl
            End If
            If row.IsSelAIScBaNull Then
                chkSelScBa.Checked = False
            Else
                chkSelScBa.Checked = row.SelAIScBa
            End If
            'PanelEmail
            If row.IsAIServizioEmailNull Then
                chkAIServizioEmail.Checked = False
            Else
                chkAIServizioEmail.Checked = row.AIServizioEmail
            End If
            If chkAIServizioEmail.Checked = False Then
                lblAIServizioEmail.BackColor = SEGNALA_KO
            Else
                lblAIServizioEmail.BackColor = SEGNALA_OKLBL
            End If
            If row.IsSMTPServerNull Then
                txtSMTPServer.Text = ""
            Else
                txtSMTPServer.Text = row.SMTPServer
            End If
            If row.IsSMTPPortaNull Then
                txtSMTPPorta.Text = ""
            Else
                txtSMTPPorta.Text = row.SMTPPorta
            End If
            If row.IsSMTPUserNameNull Then
                txtSMTPUserName.Text = ""
            Else
                txtSMTPUserName.Text = row.SMTPUserName
            End If
            If row.IsSMTPPasswordNull Then
                txtSMTPPassword.Text = ""
            Else
                txtSMTPPassword.Text = row.SMTPPassword
            End If
            If row.IsSMTPMailSenderNull Then
                txtSMTPMailSender.Text = ""
            Else
                txtSMTPMailSender.Text = row.SMTPMailSender
            End If
            If row.IsAIServizioEmailAttivaNull Then
                txtAIServizioEmailAttiva.Text = ""
            Else
                txtAIServizioEmailAttiva.Text = row.AIServizioEmailAttiva
            End If
            'PanelAlertMenu 
            strErrore = "" : strValore = ""
            Call GetDatiAbilitazioni(CSTABILCOGE, "NGGDistRiBa", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            txtAlertRiBaDocTrInCG.Text = strValore.Trim
            '- 
            strErrore = "" : strValore = ""
            Call GetDatiAbilitazioni(CSTABILAZI, "NGGCAScadPag", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            txtAlertScPagCA.Text = strValore.Trim
            '-
            strErrore = "" : strValore = ""
            Call GetDatiAbilitazioni(CSTABILAZI, "NGGCAScadAtt", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            txtAlertScAttCA.Text = strValore.Trim
            '-
            strErrore = "" : strValore = ""
            Call GetDatiAbilitazioni(CSTABILAZI, "NGGCAScadFin", strValore, strErrore)
            If strErrore.Trim <> "" Then
                strValore = ""
            End If
            txtAlertScadCA.Text = strValore.Trim
            '-
        Catch ex As Exception
            _Errore = "Errore Lettura ParGen: " & ex.Message
            LeggiParametri = False
        End Try
    End Function
    'giu230819 copiata da documenti elenco se modificate qui MODIFICARE anche da altre parti
    Private Function CheckNumDoc(ByVal myTipoDoc As String, ByVal SWFatturaPA As Boolean, ByRef strErrore As String) As Long
        strErrore = ""
        Dim strSQL As String = "Select COUNT(IDDocumenti) AS TotDoc, MAX(CONVERT(INT, Numero)) AS Numero From DocumentiT WHERE "
        If myTipoDoc = SWTD(TD.DocTrasportoClienti) Or
            myTipoDoc = SWTD(TD.DocTrasportoFornitori) Or
            myTipoDoc = SWTD(TD.DocTrasportoCLavoro) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoClienti) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoFornitori) & "' OR "
            strSQL += "Tipo_Doc = '" & SWTD(TD.DocTrasportoCLavoro) & "'"
        ElseIf myTipoDoc = SWTD(TD.FatturaCommerciale) Or
            myTipoDoc = SWTD(TD.FatturaScontrino) Then
            'GIU220714 giu110814
            If SWFatturaPA = True And myTipoDoc = SWTD(TD.FatturaCommerciale) Then
                strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0) "
                If chkNCPASep.Checked = False Then ' GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0)"
                End If
            Else
                strSQL += "(Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
                If chkNCNumSep.Checked = False Then ' GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0)"
                End If
            End If
        ElseIf myTipoDoc = SWTD(TD.FatturaAccompagnatoria) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaAccompagnatoria) & "'"
        ElseIf myTipoDoc = SWTD(TD.NotaCredito) Then
            'giu220714 giu110814
            If SWFatturaPA = True Then
                strSQL += "(Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)<>0) "
                If chkNCPASep.Checked = False Then 'GetParamGestAzi(Session(ESERCIZIO)).NumeroNCPASep = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)<>0)"
                End If
            Else
                strSQL += "(Tipo_Doc = '" & SWTD(TD.NotaCredito) & "' AND ISNULL(FatturaPA,0)=0) "
                If chkNCNumSep.Checked = False Then 'If GetParamGestAzi(Session(ESERCIZIO)).NoteAccreditoNumerazioneSeparata = False Then
                    strSQL += "OR (Tipo_Doc = '" & SWTD(TD.FatturaCommerciale) & "' AND ISNULL(FatturaPA,0)=0) OR "
                    strSQL += "Tipo_Doc = '" & SWTD(TD.FatturaScontrino) & "'"
                End If
            End If
        ElseIf myTipoDoc = SWTD(TD.NotaCorrispondenza) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.NotaCorrispondenza) & "'"
        ElseIf myTipoDoc = SWTD(TD.BuonoConsegna) Then
            strSQL += "Tipo_Doc = '" & SWTD(TD.BuonoConsegna) & "'"
        Else 'GIU260312 PER TUTTI GLI ALTRI 
            strSQL += "Tipo_Doc = '" & myTipoDoc.ToString.Trim & "'"
        End If

        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftAzi, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 0) Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item("Numero")) Then
                        CheckNumDoc = ds.Tables(0).Rows(0).Item("Numero")
                        ' ''CheckNumDoc = ds.Tables(0).Rows(0).Item("Numero") + 1
                        ' ''If (ds.Tables(0).Rows(0).Item("TotDoc") + 1) <> (ds.Tables(0).Rows(0).Item("Numero") + 1) Then
                        ' ''    'GIU171012
                        ' ''    CheckNumDoc = IIf((ds.Tables(0).Rows(0).Item("TotDoc") + 1) < CheckNumDoc, CheckNumDoc, (ds.Tables(0).Rows(0).Item("TotDoc") + 1))
                        ' ''End If
                    Else
                        CheckNumDoc = 0
                    End If
                    Exit Function
                Else
                    CheckNumDoc = 0
                    Exit Function
                End If
            Else
                CheckNumDoc = 0
                Exit Function
            End If
        Catch Ex As Exception
            strErrore = Ex.Message
            CheckNumDoc = -1
            Exit Function
        End Try

    End Function
    '-----------
    Private Sub btnAggiorna_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAggiorna.Click
        'giu080312
        Dim sTipoUtente As String = ""
        If String.IsNullOrEmpty(Session(CSTTIPOUTENTE)) Then
            Dim Utente As OperatoreConnessoEntity = SessionUtility.GetLogOnUtente("", "", "", NomeModulo, Session.SessionID, -1, "", "", "", "")
            If (Utente Is Nothing) Then
                Response.Redirect("WF_ErroreUtenteConnesso.aspx?labelForm=Errore: Sessione scaduta: utente non valido.")
                Exit Sub
            End If
            sTipoUtente = Utente.Tipo
        Else
            sTipoUtente = Session(CSTTIPOUTENTE)
        End If
        '-----------
        If Not (sTipoUtente.Equals(CSTAMMINISTRATORE)) And Not (sTipoUtente.Equals(CSTTECNICO)) Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Funzione non abilitata per questo operatore.", WUC_ModalPopup.TYPE_ALERT)
            Exit Sub
        End If
        If btnAggiorna.Text = "Modifica" Then
            'giu100223 leggo i dati in tempo reale x eventuali modifiche da altri utenti
            Dim Errore As String = ""
            If Not LeggiParametri(Errore) Then
                Chiudi(Errore)
            End If
            Session(SWOP) = SWOPMODIFICA
            SetEnabledTo(True)
            btnAggiorna.Text = "Aggiorna"
            btnAnnulla.Enabled = True
            Exit Sub
        End If
        '--------GIU070220
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Session("GetScadProdCons") = ""
        Session("GetNDocByDataScadPag") = ""
        Session("GetCAByDataScadPag") = ""
        Session("GetNDocByDataScadPagINCoGeRB") = ""
        Session("GetDDTByDataScadPag") = ""
        Session("GetNDocByDC") = ""
        'GIU220320
        Session("GetCAByDataScadAtt") = ""
        Session("NGGCAScadAtt") = ""
        Session("GetCAByDataScadPag") = ""
        Session("NGGCAScadPag") = ""
        Session("NGGCAScadFin") = ""
        '--------
        DSParGen = Session("DSParGen")
        Dim row As DSParametriGenerali.ParametriGeneraliAZIRow
        row = DSParGen.ParametriGeneraliAZI.Rows(0)
        row.BeginEdit()
        'porto franco/assegnato
        If rbtnAssegnato.Checked Then
            row.DicituraPORTO = "A"
        Else
            row.DicituraPORTO = "F"
        End If
        'aspetto esteriore
        row.DicituraASPEST = txtAspettoEst.Text.Trim
        'descrizione CONAI
        row.StringaConai = TxtConai.Text.Trim
        'password movimenti
        row.PasswordMovimenti = TxtPassword.Text.Trim
        'tipo fatturazione predef.
        If DDLTipoFatturazione.SelectedIndex > 0 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLTipoFatturazione.SelectedValue) Then
                If DDLTipoFatturazione.SelectedValue <> 0 Then
                    row.CodTipoFatt = DDLTipoFatturazione.SelectedValue
                Else
                    row.SetCodTipoFattNull()
                End If
            Else
                row.SetCodTipoFattNull()
            End If
        Else
            row.SetCodTipoFattNull()
        End If
        '-------------------------------------------------------
        'calcolo sconto nell'importo di riga dei dettagli
        If rbtnScontoImporto.Checked Then
            row.CalcoloScontoSuImporto = True
        Else
            row.CalcoloScontoSuImporto = False
        End If
        'calcolo sconto cassa
        If rbtnScCassaTotRiga.Checked Then
            row.ScCassaDett = True
        Else
            row.ScCassaDett = False
        End If
        '' ''note corrispondenza con num. sep.
        ' ''row.NoteCdenzaNumSep = chkNCorrNumSep.Checked
        'note credito con num. sep.
        row.NoteAccreditoNumerazioneSeparata = chkNCNumSep.Checked
        'note credito PA con num. sep. GIU220714
        row.NumeroNCPASep = chkNCPASep.Checked
        'ultima fattura
        If Not IsNumeric(txtUltFC.Text.Trim) Then txtUltFC.Text = "0"
        row.NumeroFattura = txtUltFC.Text.Trim
        'GIU210714
        'ultima fattura Accompagnatoria
        If Not IsNumeric(txtUltFattAcc.Text.Trim) Then txtUltFattAcc.Text = "0"
        row.NumeroFA = txtUltFattAcc.Text.Trim
        '-
        'ultima fattura PA
        If Not IsNumeric(txtUltFattPA.Text.Trim) Then txtUltFattPA.Text = "0"
        row.NumeroPA = txtUltFattPA.Text.Trim
        '---------
        'ultima note di credito
        If Not IsNumeric(txtUltNC.Text.Trim) Then txtUltNC.Text = "0"
        row.NumeroNotaAccredito = txtUltNC.Text.Trim
        'ultima note di credito PA GIU220714
        If Not IsNumeric(txtUltNCPA.Text.Trim) Then txtUltNCPA.Text = "0"
        row.NumeroNCPA = txtUltNCPA.Text.Trim
        'ultimo ordine cliente
        If Not IsNumeric(txtUltOC.Text.Trim) Then txtUltOC.Text = "0"
        row.NumeroOrdineCliente = txtUltOC.Text.Trim
        'ultimo ordine fornitore
        If Not IsNumeric(txtUltOF.Text.Trim) Then txtUltOF.Text = "0"
        row.NumeroOrdineFornitore = txtUltOF.Text.Trim
        'ultimo riordine fornitore
        If Not IsNumeric(txtUltRF.Text.Trim) Then txtUltRF.Text = "0"
        row.NumeroRiordinoFornitore = txtUltRF.Text.Trim
        'ultimo preventivo
        If Not IsNumeric(txtUltPrev.Text.Trim) Then txtUltPrev.Text = "0"
        row.NumeroPreventivo = txtUltPrev.Text.Trim
        'ultima spedizione
        If Not IsNumeric(txtUltSped.Text.Trim) Then txtUltSped.Text = "0"
        row.NumeroSped = txtUltSped.Text.Trim
        'Ultimo DDT a cliente giu230614
        If Not IsNumeric(txtUltDDT.Text.Trim) Then txtUltDDT.Text = "0"
        row.NumeroDDT = txtUltDDT.Text.Trim
        'decimali % sconto
        If ddlDecPercSc.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlDecPercSc.SelectedValue) Then
                If IsNumeric(ddlDecPercSc.SelectedValue) <> 0 Then
                    row.Decimali_Sconto = ddlDecPercSc.SelectedValue
                Else
                    row.Decimali_Sconto = 0
                End If
            Else
                row.Decimali_Sconto = 0
            End If
        Else
            row.Decimali_Sconto = 0
        End If
        'decimali % provv
        If ddlDecPrecProvv.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlDecPrecProvv.SelectedValue) Then
                If IsNumeric(ddlDecPrecProvv.SelectedValue) <> 0 Then
                    row.Decimali_Provvigione = ddlDecPrecProvv.SelectedValue
                Else
                    row.Decimali_Provvigione = 0
                End If
            Else
                row.Decimali_Provvigione = 0
            End If
        Else
            row.Decimali_Provvigione = 0
        End If
        'numero sconti in doc.
        If ddlNumScDoc.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlNumScDoc.SelectedValue) Then
                If IsNumeric(ddlNumScDoc.SelectedValue) <> 0 Then
                    row.NumSconti = ddlNumScDoc.SelectedValue
                Else
                    row.NumSconti = 0
                End If
            Else
                row.NumSconti = 0
            End If
        Else
            row.NumSconti = 0
        End If
        '---------------------------------------------------------
        'banca appoggio
        row.Banca = txtBancaAppoggio.Text.Trim
        'ABI
        row.ABI = txtABIBancaAppoggio.Text.Trim
        'CAB
        row.CAB = txtCABBancaAppoggio.Text.Trim
        'N° C/C
        row.CC = txtNCCBancaAppoggio.Text.Trim
        'CIN
        row.CIN = txtCINBancaAppoggio.Text.Trim
        'nazione
        row.NazIBAN = txtNazIBAN.Text.Trim
        'CIN EU
        row.CINEUIBAN = txtCINEu.Text.Trim
        'codice SWIFT
        row.SWIFT = txtSwift.Text.Trim
        'causale fatture
        If ddlCausFC.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlCausFC.SelectedValue) Then
                If ddlCausFC.SelectedValue <> 0 Then
                    row.CodiceCausaleCOGE = ddlCausFC.SelectedValue
                Else
                    row.SetCodiceCausaleCOGENull()
                End If
            Else
                row.SetCodiceCausaleCOGENull()
            End If
        Else
            row.SetCodiceCausaleCOGENull()
        End If
        '-------------------------------------------------------
        'causale note di credito
        If ddlCausNC.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlCausNC.SelectedValue) Then
                If ddlCausNC.SelectedValue <> 0 Then
                    row.CodiceCausaleCOGENA = ddlCausNC.SelectedValue
                Else
                    row.SetCodiceCausaleCOGENANull()
                End If
            Else
                row.SetCodiceCausaleCOGENANull()
            End If
        Else
            row.SetCodiceCausaleCOGENANull()
        End If
        '-------------------------------------------------------
        'causale corrispettivi
        If ddlCausCO.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlCausCO.SelectedValue) Then
                If ddlCausCO.SelectedValue <> 0 Then
                    row.CodiceCausaleCorrisp = ddlCausCO.SelectedValue
                Else
                    row.SetCodiceCausaleCorrispNull()
                End If
            Else
                row.SetCodiceCausaleCorrispNull()
            End If
        Else
            row.SetCodiceCausaleCorrispNull()
        End If
        '-------------------------------------------------------
        'causale incassi
        If ddlCausIN.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlCausIN.SelectedValue) Then
                If ddlCausIN.SelectedValue <> 0 Then
                    row.CodiceCausaleIncasso = ddlCausIN.SelectedValue
                Else
                    row.SetCodiceCausaleIncassoNull()
                End If
            Else
                row.SetCodiceCausaleIncassoNull()
            End If
        Else
            row.SetCodiceCausaleIncassoNull()
        End If
        '-------------------------------------------------------
        'causale pagamento note di credito
        If ddlCausPagNC.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlCausPagNC.SelectedValue) Then
                If ddlCausPagNC.SelectedValue <> 0 Then
                    row.CodiceCausaleIncassoNA = ddlCausPagNC.SelectedValue
                Else
                    row.SetCodiceCausaleIncassoNANull()
                End If
            Else
                row.SetCodiceCausaleIncassoNANull()
            End If
        Else
            row.SetCodiceCausaleIncassoNANull()
        End If
        'causale RiBa
        If ddlCausRiBa.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlCausRiBa.SelectedValue) Then
                If ddlCausRiBa.SelectedValue <> 0 Then
                    row.CausaleRiBa = ddlCausRiBa.SelectedValue
                Else
                    row.SetCausaleRiBaNull()
                End If
            Else
                row.SetCausaleRiBaNull()
            End If
        Else
            row.SetCodiceCausaleIncassoNANull()
        End If
        'conto cassa
        If txtContoCassa.Text.Trim <> "" Then
            row.ContoCassa = txtContoCassa.Text.Trim
        Else
            row.SetContoCassaNull()
        End If
        'conto ricavo
        If txtContoRicavo.Text.Trim <> "" Then
            row.CodiceContoRicavoCOGE = txtContoRicavo.Text.Trim
        Else
            row.SetCodiceContoRicavoCOGENull()
        End If
        'conto corrispettivi
        If txtContoCorr.Text.Trim <> "" Then
            row.ContoCorrispettivi = txtContoCorr.Text.Trim
        Else
            row.SetContoCorrispettiviNull()
        End If
        'conto spese incasso
        If txtContoSpeseInc.Text.Trim <> "" Then
            row.ContoSpeseIncasso = txtContoSpeseInc.Text.Trim
        Else
            row.SetContoSpeseIncassoNull()
        End If
        If ddlAliqIVAInc.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlAliqIVAInc.SelectedValue) Then
                If ddlAliqIVAInc.SelectedValue <> 0 Then
                    row.IvaSpese = ddlAliqIVAInc.SelectedValue
                Else
                    row.SetIvaSpeseNull()
                End If
            Else
                row.SetIvaSpeseNull()
            End If
        Else
            row.SetIvaSpeseNull()
        End If
        '-------------------------------------------------------
        'conto spese varie BOLLO GIU260219
        If txtContoSpeseVarie.Text.Trim <> "" Then
            row.ContoSpeseVarie = txtContoSpeseVarie.Text.Trim
        Else
            row.SetContoSpeseVarieNull()
        End If
        If ddlAliqIVABollo.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlAliqIVABollo.SelectedValue) Then
                If ddlAliqIVABollo.SelectedValue <> 0 Then
                    row.IVABollo = ddlAliqIVABollo.SelectedValue
                Else
                    row.SetIVABolloNull()
                End If
            Else
                row.SetIVABolloNull()
            End If
        Else
            row.SetIVABolloNull()
        End If
        '-------------------------------------------------------
        If Not IsNumeric(txtImpMinBollo.Text.Trim) Then txtImpMinBollo.Text = "0"
        row.ImpMinBollo = txtImpMinBollo.Text.Trim
        If Not IsNumeric(txtBollo.Text.Trim) Then txtBollo.Text = "0"
        row.Bollo = txtBollo.Text.Trim
        'IVA SCONTO MERCE
        If ddlAliqIVAScMerce.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlAliqIVAScMerce.SelectedValue) Then
                If ddlAliqIVAScMerce.SelectedValue <> 0 Then
                    row.IVAScMerce = ddlAliqIVAScMerce.SelectedValue
                Else
                    row.SetIVAScMerceNull()
                End If
            Else
                row.SetIVAScMerceNull()
            End If
        Else
            row.SetIVAScMerceNull()
        End If
        '-------------------------------------------------------
        'conto spese imballo
        If txtContoSpeseImb.Text.Trim <> "" Then
            row.ContoSpeseImballo = txtContoSpeseImb.Text.Trim
        Else
            row.SetContoSpeseImballoNull()
        End If
        If ddlAliqIVAImb.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlAliqIVAImb.SelectedValue) Then
                If ddlAliqIVAImb.SelectedValue <> 0 Then
                    row.Iva_Imballo = ddlAliqIVAImb.SelectedValue
                Else
                    row.SetIva_ImballoNull()
                End If
            Else
                row.SetIva_ImballoNull()
            End If
        Else
            row.SetIva_ImballoNull()
        End If
        '-------------------------------------------------------
        'conto spese trasporto
        If txtContoSpeseTrasp.Text.Trim <> "" Then
            row.ContoSpeseTrasporto = txtContoSpeseTrasp.Text.Trim
        Else
            row.SetContoSpeseTrasportoNull()
        End If
        If ddlAliqVATrasp.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlAliqVATrasp.SelectedValue) Then
                If ddlAliqVATrasp.SelectedValue <> 0 Then
                    row.IVATrasporto = ddlAliqVATrasp.SelectedValue
                Else
                    row.IsIVATrasportoNull()
                End If
            Else
                row.IsIVATrasportoNull()
            End If
        Else
            row.IsIVATrasportoNull()
        End If
        '-------------------------------------------------------
        'conto RiBa
        If txtContoRiBa.Text.Trim <> "" Then
            row.ContoRiBa = txtContoRiBa.Text.Trim
        Else
            row.SetContoRiBaNull()
        End If
        'conto Ritenuta d'acconto
        If txtContoRitAcconto.Text.Trim <> "" Then
            row.ContoRitAcconto = txtContoRitAcconto.Text.Trim
        Else
            row.SetContoRitAccontoNull()
        End If
        'registrazione auto incassi
        row.RegIncasso = chkRegAutoInc.Checked
        'registrazione RiBa in PN
        row.RegPNRB = chkRegRBPrimaNota.Checked
        'Magazzino
        If ddlCausRiprSaldi.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlCausRiprSaldi.SelectedValue) Then
                If ddlCausRiprSaldi.SelectedValue <> 0 Then
                    row.CausaleRipristinoSaldi = ddlCausRiprSaldi.SelectedValue
                Else
                    row.SetCausaleRipristinoSaldiNull()
                End If
            Else
                row.SetCausaleRipristinoSaldiNull()
            End If
        Else
            row.SetCausaleRipristinoSaldiNull()
        End If
        '-------------------------------------------------------
        If DDLCausRiordino.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLCausRiordino.SelectedValue) Then
                If DDLCausRiordino.SelectedValue <> 0 Then
                    row.CodiceCausaleRiordino = DDLCausRiordino.SelectedValue
                Else
                    row.SetCodiceCausaleRiordinoNull()
                End If
            Else
                row.SetCodiceCausaleRiordinoNull()
            End If
        Else
            row.SetCodiceCausaleRiordinoNull()
        End If
        '-------------------------------------------------------
        If DDLCausMMPos.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLCausMMPos.SelectedValue) Then
                If DDLCausMMPos.SelectedValue <> 0 Then
                    row.causaleMMpos = DDLCausMMPos.SelectedValue
                Else
                    row.SetcausaleMMposNull()
                End If
            Else
                row.SetcausaleMMposNull()
            End If
        Else
            row.SetcausaleMMposNull()
        End If
        '-------------------------------------------------------
        If DDLCausMMNeg.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLCausMMNeg.SelectedValue) Then
                If DDLCausMMNeg.SelectedValue <> 0 Then
                    row.causaleMMneg = DDLCausMMNeg.SelectedValue
                Else
                    row.SetcausaleMMnegNull()
                End If
            Else
                row.SetcausaleMMnegNull()
            End If
        Else
            row.SetcausaleMMnegNull()
        End If
        '-------------------------------------------------------
        'Documenti
        If DDLCausVendita.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLCausVendita.SelectedValue) Then
                If DDLCausVendita.SelectedValue <> 0 Then
                    row.CodCausaleVendita = DDLCausVendita.SelectedValue
                Else
                    row.SetCodCausaleVenditaNull()
                End If
            Else
                row.SetCodCausaleVenditaNull()
            End If
        Else
            row.SetCodCausaleVenditaNull()
        End If
        '-------------------------------------------------------
        If DDLCausResi.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLCausResi.SelectedValue) Then
                If DDLCausResi.SelectedValue <> 0 Then
                    row.CausNCResi = DDLCausResi.SelectedValue
                Else
                    row.SetCausNCResiNull()
                End If
            Else
                row.SetCausNCResiNull()
            End If
        Else
            row.SetCausNCResiNull()
        End If
        '-------------------------------------------------------
        'Contratti
        Dim strErrore As String = "" : Dim strValore As String = ""
        'GIU200722
        If DDLVettore1.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLVettore1.SelectedValue) Then
                If DDLVettore1.SelectedValue <> "" Then
                    strValore = DDLVettore1.SelectedValue.ToString.Trim
                Else
                    strValore = ""
                End If
            Else
                strValore = ""
            End If
        Else
            strValore = ""
        End If
        '-------------------------------------------------------
        Session("SpedVettCsv") = ""
        Session("SWSpedVettCsv") = ""
        Call AggiornaAbilitazione(CSTABILAZI, "SpedVettCsv", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE Abilitazioni SET Abilitato=1 WHERE Chiave='SpedVettCsv'"
        Else
            strSQL = "UPDATE Abilitazioni SET Abilitato=0 WHERE Chiave='SpedVettCsv'"
            'da fare ora solo su 1 vett1 - ATTIVO SEMPRE - VALE PER TUTTI I VETTORI
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore SpedVettCsv"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        strValore = txtAccountSpedDDT.Text.Trim
        Call AggiornaAbilitazione(CSTABILAZI, "AccountSped", strValore, strErrore)
        strValore = txtEmailSpedDDT.Text.Trim
        Call AggiornaAbilitazione(CSTABILAZI, "EmailSped", strValore, strErrore)
        strValore = txtTelSpedDDT.Text.Trim
        Call AggiornaAbilitazione(CSTABILAZI, "TelSped", strValore, strErrore)
        strValore = txtCPagCA.Text.Trim
        'GIU261122 CONTROLLO FORMALE: ,NNN,X, per ogni codice inserito
        If strValore.Trim <> "" Then
            Dim myPos As Integer = 0
            Dim StrDato() As String
            myPos = InStr(strValore, ",")
            If myPos > 0 Then
                myPos = 0
                StrDato = strValore.Split(",")
                For I = 0 To StrDato.Count - 1
                    myPos += 1
                    If myPos > 3 Then myPos = 1
                Next
                If myPos <> 3 Then
                    strErrore = "Tipo Pagamento Contrassegno per file spedizioni DDT - Formalmente errato (,NNN,X, per ogni codice inserito)"
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                                    strErrore, WUC_ModalPopup.TYPE_ERROR)
                    Exit Sub
                End If
            Else
                strErrore = "Tipo Pagamento Contrassegno per file spedizioni DDT - Formalmente errato (,NNN,X, per ogni codice inserito)"
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                                strErrore, WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
        End If
        '---------
        Call AggiornaAbilitazione(CSTABILAZI, "CPagCA", strValore, strErrore)
        '---------
        If DDLDurataTipo.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLDurataTipo.SelectedValue) Then
                If DDLDurataTipo.SelectedValue <> "" Then
                    strValore = DDLDurataTipo.SelectedValue.ToString.Trim
                Else
                    strValore = ""
                End If
            Else
                strValore = ""
            End If
        Else
            strValore = ""
        End If
        '-------------------------------------------------------
        Call AggiornaAbilitazione(CSTABILAZI, "DurataTipo", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE Abilitazioni SET Abilitato=1 WHERE Chiave='DurataTipo'"
        Else
            strSQL = "UPDATE Abilitazioni SET Abilitato=0 WHERE Chiave='DurataTipo'"
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore DurataTipo"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        If DDLCausCAMDAE.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLCausCAMDAE.SelectedValue) Then
                If DDLCausCAMDAE.SelectedValue <> 0 Then
                    strValore = DDLCausCAMDAE.SelectedValue.ToString.Trim
                Else
                    strValore = ""
                End If
            Else
                strValore = ""
            End If
        Else
            strValore = ""
        End If
        '-------------------------------------------------------
        Call AggiornaAbilitazione(CSTABILAZI, "ConAssMDAE", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE Abilitazioni SET Abilitato=1 WHERE Chiave='ConAssMDAE'"
        Else
            strSQL = "UPDATE Abilitazioni SET Abilitato=0 WHERE Chiave='ConAssMDAE'"
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore ConAssMDAE"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        'DDLCausCALoc.Enabled = SW
        If DDLCausCALoc.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLCausCALoc.SelectedValue) Then
                If DDLCausCALoc.SelectedValue <> 0 Then
                    strValore = DDLCausCALoc.SelectedValue.ToString.Trim
                Else
                    strValore = ""
                End If
            Else
                strValore = ""
            End If
        Else
            strValore = ""
        End If
        '-------------------------------------------------------
        Call AggiornaAbilitazione(CSTABILAZI, "ConAssLoc", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE Abilitazioni SET Abilitato=1 WHERE Chiave='ConAssLoc'"
        Else
            strSQL = "UPDATE Abilitazioni SET Abilitato=0 WHERE Chiave='ConAssLoc'"
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore ConAssLoc"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'DDLCausCATelC.Enabled = SW
        If DDLCausCATelC.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(DDLCausCATelC.SelectedValue) Then
                If DDLCausCATelC.SelectedValue <> 0 Then
                    strValore = DDLCausCATelC.SelectedValue.ToString.Trim
                Else
                    strValore = ""
                End If
            Else
                strValore = ""
            End If
        Else
            strValore = ""
        End If
        '-------------------------------------------------------
        Call AggiornaAbilitazione(CSTABILAZI, "ConAssTelC", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE Abilitazioni SET Abilitato=1 WHERE Chiave='ConAssTelC'"
        Else
            strSQL = "UPDATE Abilitazioni SET Abilitato=0 WHERE Chiave='ConAssTelC'"
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore ConAssTelC"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        'anni predef fine produzione
        ' ''If txtAnniFineProd.Text <> "" Then
        ' ''    row.AnniFuoriProd = txtAnniFineProd.Text.Trim
        ' ''Else
        ' ''    row.SetAnniFuoriProdNull()
        ' ''End If
        'causale DDT a deposito
        If ddlDDTDep.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlDDTDep.SelectedValue) Then
                If ddlDDTDep.SelectedValue <> 0 Then
                    row.CausDDTDep = ddlDDTDep.SelectedValue
                Else
                    row.SetCausDDTDepNull()
                End If
            Else
                row.SetCausDDTDepNull()
            End If
        Else
            row.SetCausDDTDepNull()
        End If
        '-------------------------------------------------------
        'causale vendita da deposito
        If ddlVendDep.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlVendDep.SelectedValue) Then
                If ddlVendDep.SelectedValue <> 0 Then
                    row.CausVendDep = ddlVendDep.SelectedValue
                Else
                    row.SetCausVendDepNull()
                End If
            Else
                row.SetCausVendDepNull()
            End If
        Else
            row.SetCausVendDepNull()
        End If
        '-------------------------------------------------------
        'causale reso da deposito
        If ddlResoDep.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlResoDep.SelectedValue) Then
                If ddlResoDep.SelectedValue <> 0 Then
                    row.CausResoDep = ddlResoDep.SelectedValue
                Else
                    row.SetCausResoDepNull()
                End If
            Else
                row.SetCausResoDepNull()
            End If
        Else
            row.SetCausResoDepNull()
        End If
        '-------------------------------------------------------
        'causale rimanenza iniziale
        If ddlRimIniz.SelectedIndex > -1 Then 'giu090224
            If Not String.IsNullOrEmpty(ddlRimIniz.SelectedValue) Then
                If ddlRimIniz.SelectedValue <> 0 Then
                    row.CausRimInizialeDep = ddlRimIniz.SelectedValue
                Else
                    row.SetCausRimInizialeDepNull()
                End If
            Else
                row.SetCausRimInizialeDepNull()
            End If
        Else
            row.SetCausRimInizialeDepNull()
        End If
        '-------------------------------------------------------
        '---
        'giu310718 PanelScadenze
        If chkTutteCatCli.Checked = True Then
            row.SelAICatCli = -1
        ElseIf chkSelCategorie.Checked = True Then
            row.SelAICatCli = 0
        ElseIf ddlCatCli.SelectedIndex > 0 Then
            If chkRaggrCatCli.Checked = False Then
                row.SelAICatCli = ddlCatCli.SelectedValue
            Else
                row.SelAICatCli = ddlCatCli.SelectedValue * -1
            End If
        Else
            chkTutteCatCli.AutoPostBack = False
            row.SelAICatCli = -1
            chkTutteCatCli.Checked = True
            chkTutteCatCli.AutoPostBack = True
        End If
        'PanelScadenzeDaA
        If Not IsNumeric(txtDallaData.Text.Trim) Then
            txtDallaData.Text = "0"
        End If
        If Not IsNumeric(txtAllaData.Text.Trim) Then
            txtAllaData.Text = "0"
        End If
        row.SelAIDaData = txtDallaData.Text.Trim
        row.SelAIAData = txtAllaData.Text.Trim
        row.SelAIScGa = chkSelScGa.Checked
        row.SelAIScEl = chkSelScEl.Checked
        row.SelAIScBa = chkSelScBa.Checked
        If chkSelScGa.Checked = False And chkSelScEl.Checked = False And chkSelScBa.Checked = False Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            "Selezionare almeno un tipo di scadenza dei Prodotti consumabili <br> " &
                            "Modificare nuovamente i dati e aggiornare.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        row.AIServizioEmail = chkAIServizioEmail.Checked
        row.SMTPServer = txtSMTPServer.Text.Trim
        If Not IsNumeric(txtSMTPPorta.Text.Trim) Then
            txtSMTPPorta.Text = "25"
        End If
        row.SMTPPorta = txtSMTPPorta.Text.Trim
        row.SMTPUserName = txtSMTPUserName.Text.Trim
        row.SMTPPassword = txtSMTPPassword.Text.Trim
        row.SMTPMailSender = txtSMTPMailSender.Text.Trim
        'giu270219
        Dim myErrOra As Boolean = False
        If txtAIServizioEmailAttiva.Text.Trim = "" Then
            'ok 
        Else
            Try
                If Not IsNumeric(Mid(txtAIServizioEmailAttiva.Text.Trim, 1, 2)) Then
                    myErrOra = True
                ElseIf Int(Mid(txtAIServizioEmailAttiva.Text.Trim, 1, 2)) < 0 Or Int(Mid(txtAIServizioEmailAttiva.Text.Trim, 1, 2)) > 24 Then
                    myErrOra = True
                End If
                If Mid(txtAIServizioEmailAttiva.Text.Trim, 3, 1) <> ":" And Mid(txtAIServizioEmailAttiva.Text.Trim, 3, 1) <> "." Then
                    myErrOra = True
                End If
                If Int(Mid(txtAIServizioEmailAttiva.Text.Trim, 4, 2)) < 0 Or Int(Mid(txtAIServizioEmailAttiva.Text.Trim, 4, 2)) > 60 Then
                    myErrOra = True
                End If
            Catch ex As Exception
                myErrOra = True
            End Try
        End If
        If myErrOra = True Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            "L'orario di attivazione servizio Invio E-Mail risulta errato <br> " &
                            "Modificare nuovamente i dati e aggiornare.", WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        row.AIServizioEmailAttiva = txtAIServizioEmailAttiva.Text.Trim
        '
        strValore = txtAlertRiBaDocTrInCG.Text.Trim
        Call AggiornaAbilitazione(CSTABILCOGE, "NGGDistRiBa", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE AbilitazioniCoGe SET Abilitato=1 WHERE Chiave='NGGDistRiBa'"
        Else
            strSQL = "UPDATE AbilitazioniCoGe SET Abilitato=0 WHERE Chiave='NGGDistRiBa'"
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore NGGDistRiBa"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        strValore = txtAlertScPagCA.Text.Trim
        Call AggiornaAbilitazione(CSTABILAZI, "NGGCAScadPag", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE Abilitazioni SET Abilitato=1 WHERE Chiave='NGGCAScadPag'"
        Else
            strSQL = "UPDATE Abilitazioni SET Abilitato=0 WHERE Chiave='NGGCAScadPag'"
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore NGGCAScadPag"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        strValore = txtAlertScAttCA.Text.Trim
        Call AggiornaAbilitazione(CSTABILAZI, "NGGCAScadAtt", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE Abilitazioni SET Abilitato=1 WHERE Chiave='NGGCAScadAtt'"
        Else
            strSQL = "UPDATE Abilitazioni SET Abilitato=0 WHERE Chiave='NGGCAScadAtt'"
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore NGGCAScadAtt"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-  
        strValore = txtAlertScadCA.Text.Trim
        Call AggiornaAbilitazione(CSTABILAZI, "NGGCAScadFin", strValore, strErrore)
        If strValore.Trim <> "" Then
            strSQL = "UPDATE Abilitazioni SET Abilitato=1 WHERE Chiave='NGGCAScadFin'"
        Else
            strSQL = "UPDATE Abilitazioni SET Abilitato=0 WHERE Chiave='NGGCAScadFin'"
        End If
        Try
            If ObjDB.ExecuteQueryUpdate(TipoDB.dbOpzioni, strSQL) = False Then
                strErrore = "Errore NGGCAScadFin"
            End If
        Catch ex As Exception
            strErrore = ex.Message
        End Try
        If strErrore.Trim <> "" Then
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                            strErrore, WUC_ModalPopup.TYPE_ERROR)
            Exit Sub
        End If
        '-
        ObjDB = Nothing
        '--------
        'OK AGGIORNA
        row.EndEdit()
        '---
        SqlDAdap = Session("SqlDAdapParGen")
        '--
        SqlDbSelectCmd = Session("SqlDbSelectCmdParGen")
        SqlDbUpdateCmd = Session("SqlDbUpdateCmdParGen")
        SqlDAdap.SelectCommand = SqlDbSelectCmd
        SqlDAdap.UpdateCommand = SqlDbUpdateCmd
        Try
            strErrore = ""
            Me.SqlDAdap.Update(DSParGen.ParametriGeneraliAZI)
            'SONO CAMBIATI DEI VALORI NEL FRATTEMPO AGGIORNAMENTO FALLITO
            If SqlDbUpdateCmd.Parameters.Item("@RETURN_VALUE").Value = 0 Then
                If Not LeggiParametri(strErrore) Then
                    Chiudi(strErrore)
                End If
                ' ''If SWErrNum = False Then
                ' ''    TabContainer1.ActiveTabIndex = 0
                ' ''Else
                ' ''    TabContainer1.ActiveTabIndex = 1
                ' ''End If
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Attenzione", "Aggiornamento non riuscito. <br> " &
                                "Cause possibili: Numerazione documenti cambiata <br> " &
                                "Modificare nuovamente i dati e aggiornare.", WUC_ModalPopup.TYPE_ERROR)
                Exit Sub
            End If
            'in sessione viene fatto in leggip....
            If Not LeggiParametri(strErrore) Then
                Chiudi(strErrore)
            End If
            ' ''If SWErrNum = False Then
            ' ''    TabContainer1.ActiveTabIndex = 0
            ' ''Else
            ' ''    TabContainer1.ActiveTabIndex = 1
            ' ''End If
            ' ''If (dvParGen Is Nothing) Then
            ' ''    dvParGen = New DataView(DSParGen.ParametriGeneraliAZI)
            ' ''End If
            ' ''Session("dvParGen") = dvParGen
            ' ''Session("DSParGen") = DSParGen
            Session(SWOP) = SWOPNESSUNA
        Catch ExSQL As SqlException
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore SQL in Aggiorna ParGen. ", ExSQL.Message, WUC_ModalPopup.TYPE_ERROR)
            'strErrore = "Errore SQL in Aggiorna ParGen. " & ExSQL.Message
        Catch Ex As Exception
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Errore in Aggiorna ParGen. ", Ex.Message, WUC_ModalPopup.TYPE_ERROR)
            'strErrore = "Errore in Aggiorna ParGen. " & Ex.Message
        End Try

    End Sub

    Private Sub btnAnnulla_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAnnulla.Click
        Session(SWOP) = SWOPNESSUNA
        Dim strErrore As String = ""
        If Not LeggiParametri(strErrore) Then
            Chiudi(strErrore)
        End If
        ' ''If SWErrNum = False Then
        ' ''    TabContainer1.ActiveTabIndex = 0
        ' ''Else
        ' ''    TabContainer1.ActiveTabIndex = 1
        ' ''End If
    End Sub
    'giu310718 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    Protected Sub btnSelCategorie_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        ApriElenco(F_CATEGORIE)
    End Sub
    Private Sub ApriElenco(ByVal finestra As String)
        Session(F_ELENCO_APERTA) = finestra
        Select Case finestra
            Case F_CATEGORIE
                WFPElencoCategorie.Show(True)
        End Select
    End Sub
    Public Sub CallBackWFPElenco(ByVal codice As String, ByVal descrizione As String, ByVal finestra As String)
        If chkSelCategorie.Checked = True Then
            Dim NSel As Integer = 0
            If LeggiCategorie("", NSel) = True Then
                btnSelCategorie.BackColor = SEGNALA_OKLBL
                ' ''lblMessUtente.BackColor = SEGNALA_OK
                ' ''lblMessUtente.Text = "N° Categorie selezionate: " & NSel.ToString.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Selezione multipla categorie", "N° Categorie selezionate: " & NSel.ToString.Trim, WUC_ModalPopup.TYPE_INFO)
            Else
                btnSelCategorie.BackColor = SEGNALA_KO
                chkSelCategorie.Checked = False
                Exit Sub
            End If
            ddlCatCli.SelectedIndex = -1
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkTutteCatCli.Checked = False
        Else
            btnSelCategorie.BackColor = SEGNALA_OKLBL
            ' ''lblMessUtente.BackColor = SEGNALA_OK
            ' ''lblMessUtente.Text = "Selezione/Deselezione clienti a cui inviare e-mail scadenza"
        End If
    End Sub
    Private Sub chkTutteCatCli_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTutteCatCli.CheckedChanged
        ddlCatCli.SelectedIndex = -1
        If chkTutteCatCli.Checked Then
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkSelCategorie.AutoPostBack = False
            chkSelCategorie.Checked = False
            chkSelCategorie.AutoPostBack = True
        Else
            If chkSelCategorie.Checked = False Then
                ddlCatCli.Enabled = True
                chkRaggrCatCli.Enabled = True
            End If
        End If
    End Sub
    Private Sub chkSelCategorie_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSelCategorie.CheckedChanged
        If chkSelCategorie.Checked = True Then
            Dim NSel As Integer = 0
            If LeggiCategorie("", NSel) = True Then
                btnSelCategorie.BackColor = SEGNALA_OKLBL
                ' ''lblMessUtente.BackColor = SEGNALA_OK
                ' ''lblMessUtente.Text = "N° Categorie selezionate: " & NSel.ToString.Trim
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Selezione multipla categorie", "N° Categorie selezionate: " & NSel.ToString.Trim, WUC_ModalPopup.TYPE_INFO)
            Else
                btnSelCategorie.BackColor = SEGNALA_KO
                chkSelCategorie.Checked = False
                Exit Sub
            End If
            ddlCatCli.SelectedIndex = -1
            ddlCatCli.Enabled = False
            chkRaggrCatCli.Enabled = False
            chkRaggrCatCli.Checked = False
            chkTutteCatCli.Checked = False
        Else
            If chkTutteCatCli.Checked = False Then
                ddlCatCli.Enabled = True
                chkRaggrCatCli.Enabled = True
            End If
        End If
    End Sub
    Private Function LeggiCategorie(ByRef CodCategSel As String, ByRef NSel As Integer) As Boolean
        LeggiCategorie = False
        CodCategSel = ""
        Dim strSQL As String = ""
        Dim ObjDB As New DataBaseUtility
        Dim ds As New DataSet
        Try
            strSQL = "SELECT Codice FROM Categorie WHERE ISNULL(SelSc,0)<>0 AND ISNULL(InvioMailSc,0)<>0"
            ObjDB.PopulateDatasetFromQuery(TipoDB.dbSoftCoge, strSQL, ds)
            ObjDB = Nothing
            If (ds.Tables.Count > 0) Then
                If (ds.Tables(0).Rows.Count > 1) Then
                    NSel = ds.Tables(0).Rows.Count
                    'ok
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        CodCategSel &= ds.Tables(0).Rows(i).Item("Codice").ToString & ";"
                    Next
                    CodCategSel = CodCategSel.Substring(0, CodCategSel.Length - 1) 'rimuovo ultimo ;
                Else
                    ' ''lblMessUtente.BackColor = SEGNALA_KO
                    ' ''lblMessUtente.Text = "Selezionare almeno 2 categorie."
                    Session(MODALPOPUP_CALLBACK_METHOD) = ""
                    Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                    ModalPopup.Show("Selezione multipla categorie", "Selezionare almeno 2 categorie.", WUC_ModalPopup.TYPE_ERROR)
                    Exit Function
                End If
            Else
                ' ''lblMessUtente.BackColor = SEGNALA_KO
                ' ''lblMessUtente.Text = "Selezionare almeno 2 categorie."
                Session(MODALPOPUP_CALLBACK_METHOD) = ""
                Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
                ModalPopup.Show("Selezione multipla categorie", "Selezionare almeno 2 categorie.", WUC_ModalPopup.TYPE_ERROR)
                Exit Function
            End If
        Catch Ex As Exception
            ' ''lblMessUtente.BackColor = SEGNALA_KO
            ' ''lblMessUtente.Text = "Selezionare almeno 2 categorie."
            Session(MODALPOPUP_CALLBACK_METHOD) = ""
            Session(MODALPOPUP_CALLBACK_METHOD_NO) = ""
            ModalPopup.Show("Selezione multipla categorie", "Selezionare almeno 2 categorie.", WUC_ModalPopup.TYPE_ERROR)
            Exit Function
        End Try
        LeggiCategorie = True
    End Function

    Private Sub chkAIServizioEmail_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkAIServizioEmail.CheckedChanged
        If chkAIServizioEmail.Checked = True Then
            lblAIServizioEmail.BackColor = SEGNALA_OKLBL
        Else
            lblAIServizioEmail.BackColor = SEGNALA_KO
        End If
    End Sub
End Class