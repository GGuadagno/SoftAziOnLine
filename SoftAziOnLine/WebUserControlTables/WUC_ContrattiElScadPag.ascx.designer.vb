'------------------------------------------------------------------------------
' <generato automaticamente>
'     Questo codice è stato generato da uno strumento.
'
'     Le modifiche a questo file possono causare un comportamento non corretto e verranno perse se
'     il codice viene rigenerato. 
' </generato automaticamente>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class WUC_ContrattiElScadPag

    '''<summary>
    '''Controllo panelPrincipale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents panelPrincipale As Global.System.Web.UI.WebControls.Panel

    '''<summary>
    '''Controllo UpdatePanel1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents UpdatePanel1 As Global.System.Web.UI.UpdatePanel

    '''<summary>
    '''Controllo ModalPopup.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ModalPopup As Global.SoftAziOnLine.WUC_ModalPopup

    '''<summary>
    '''Controllo WFP_ElencoCliForn1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents WFP_ElencoCliForn1 As Global.SoftAziOnLine.WFP_ElencoCliForn

    '''<summary>
    '''Controllo Attesa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Attesa As Global.SoftAziOnLine.WUC_Attesa

    '''<summary>
    '''Controllo WFPDocCollegati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents WFPDocCollegati As Global.SoftAziOnLine.WFP_DocCollegati

    '''<summary>
    '''Controllo SqlDSRegioni.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents SqlDSRegioni As Global.System.Web.UI.WebControls.SqlDataSource

    '''<summary>
    '''Controllo SqlDSProvince.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents SqlDSProvince As Global.System.Web.UI.WebControls.SqlDataSource

    '''<summary>
    '''Controllo SqlDSCausMag.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents SqlDSCausMag As Global.System.Web.UI.WebControls.SqlDataSource

    '''<summary>
    '''Controllo SqlDSRespArea.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents SqlDSRespArea As Global.System.Web.UI.WebControls.SqlDataSource

    '''<summary>
    '''Controllo SqlDSRespVisite.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents SqlDSRespVisite As Global.System.Web.UI.WebControls.SqlDataSource

    '''<summary>
    '''Controllo SqlDataMagazzino.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents SqlDataMagazzino As Global.System.Web.UI.WebControls.SqlDataSource

    '''<summary>
    '''Controllo Label15.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label15 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo txtPrimoNFattura.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtPrimoNFattura As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Label14.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label14 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo txtPrimoNFatturaPA.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtPrimoNFatturaPA As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Label17.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label17 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Label16.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label16 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo txtDataFattura.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtDataFattura As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo ImageButtonDF.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ImageButtonDF As Global.System.Web.UI.WebControls.ImageButton

    '''<summary>
    '''Controllo CalendarExtender2.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents CalendarExtender2 As Global.AjaxControlToolkit.CalendarExtender

    '''<summary>
    '''Controllo Label18.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label18 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo lblMagazzino.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents lblMagazzino As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo ddlMagazzino.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlMagazzino As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo ddlRicerca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlRicerca As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo checkParoleContenute.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents checkParoleContenute As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo txtRicerca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtRicerca As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo btnRicerca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnRicerca As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo chkFattRiep.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkFattRiep As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo chkCTRFatturato.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkCTRFatturato As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo DDLCTRFatturato.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents DDLCTRFatturato As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo PanelSelezionaPrev.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents PanelSelezionaPrev As Global.System.Web.UI.WebControls.Panel

    '''<summary>
    '''Controllo Label9.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label9 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Label11.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label11 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Label12.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label12 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo btnCercaAnagrafica1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnCercaAnagrafica1 As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo lblDesCliente.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents lblDesCliente As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo btnNoCliente.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnNoCliente As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo chkSoloEvase.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkSoloEvase As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo Label2.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label2 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Label10.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label10 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo txtDallaData.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtDallaData As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo imgBtnShowCalendarDD.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents imgBtnShowCalendarDD As Global.System.Web.UI.WebControls.ImageButton

    '''<summary>
    '''Controllo CalendarExtender1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents CalendarExtender1 As Global.AjaxControlToolkit.CalendarExtender

    '''<summary>
    '''Controllo Label8.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label8 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo txtAllaData.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtAllaData As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo imgBtnShowCalendarAD.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents imgBtnShowCalendarAD As Global.System.Web.UI.WebControls.ImageButton

    '''<summary>
    '''Controllo txtAllaData_CalendarExtender.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents txtAllaData_CalendarExtender As Global.AjaxControlToolkit.CalendarExtender

    '''<summary>
    '''Controllo Label4.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label4 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo chkTutteRegioni.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkTutteRegioni As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo ddlRegioni.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlRegioni As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Label5.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label5 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo chkTutteProvince.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkTutteProvince As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo ddlProvince.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlProvince As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Label1.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label1 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo checkCausale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents checkCausale As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo DDLCausali.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents DDLCausali As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Label3.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label3 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo checkRespArea.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents checkRespArea As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo DDLRespArea.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents DDLRespArea As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Label6.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label6 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo CheckRespVisite.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents CheckRespVisite As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo DDLRespVisite.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents DDLRespVisite As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo btnVisualizza.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnVisualizza As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnDocCollegati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnDocCollegati As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo GridViewPrevT.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents GridViewPrevT As Global.System.Web.UI.WebControls.GridView

    '''<summary>
    '''Controllo SqlDSPrevTElenco.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents SqlDSPrevTElenco As Global.System.Web.UI.WebControls.SqlDataSource

    '''<summary>
    '''Controllo UpdatePanel2.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents UpdatePanel2 As Global.System.Web.UI.UpdatePanel

    '''<summary>
    '''Controllo Label13.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label13 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo btnSelSc.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnSelSc As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnDeselSc.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnDeselSc As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnEmFattSc.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnEmFattSc As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnStFattScEm.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnStFattScEm As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo chkVisALLSc.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkVisALLSc As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo UpdatePanel4.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents UpdatePanel4 As Global.System.Web.UI.UpdatePanel

    '''<summary>
    '''Controllo GridViewPrevD.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents GridViewPrevD As Global.System.Web.UI.WebControls.GridView

    '''<summary>
    '''Controllo SqlDSPrevDByIDDocumenti.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents SqlDSPrevDByIDDocumenti As Global.System.Web.UI.WebControls.SqlDataSource

    '''<summary>
    '''Controllo lblStampe.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents lblStampe As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo LnkVerbale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LnkVerbale As Global.System.Web.UI.HtmlControls.HtmlAnchor

    '''<summary>
    '''Controllo LnkStampa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LnkStampa As Global.System.Web.UI.HtmlControls.HtmlAnchor

    '''<summary>
    '''Controllo LnkElencoSc.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LnkElencoSc As Global.System.Web.UI.HtmlControls.HtmlAnchor

    '''<summary>
    '''Controllo LnkFatturePDF.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LnkFatturePDF As Global.System.Web.UI.HtmlControls.HtmlAnchor

    '''<summary>
    '''Controllo btnStampa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnStampa As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnVerbale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnVerbale As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnElencoSc.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnElencoSc As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo chkVisElenco.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkVisElenco As Global.System.Web.UI.WebControls.CheckBox
End Class
