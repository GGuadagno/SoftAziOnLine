﻿'------------------------------------------------------------------------------
' <generato automaticamente>
'     Questo codice è stato generato da uno strumento.
'
'     Le modifiche a questo file possono causare un comportamento non corretto e verranno perse se
'     il codice viene rigenerato. 
' </generato automaticamente>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class WUC_ContrattiElScad

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
    '''Controllo WFPCambiaStatoCA.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents WFPCambiaStatoCA As Global.SoftAziOnLine.WFP_CambiaStatoCA

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
    '''Controllo btnDocCollegati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnDocCollegati As Global.System.Web.UI.WebControls.Button

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
    '''Controllo Label15.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label15 As Global.System.Web.UI.WebControls.Label

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
    '''Controllo Label11.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label11 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo Label0.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label0 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo rbtnOrdCliente.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents rbtnOrdCliente As Global.System.Web.UI.WebControls.RadioButton

    '''<summary>
    '''Controllo rbtnOrdScadenza.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents rbtnOrdScadenza As Global.System.Web.UI.WebControls.RadioButton

    '''<summary>
    '''Controllo Label12.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label12 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo chkVisElenco.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkVisElenco As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo Label13.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label13 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo btnElencoSc.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnElencoSc As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo lnkElenco.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents lnkElenco As Global.System.Web.UI.HtmlControls.HtmlAnchor

    '''<summary>
    '''Controllo Label14.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label14 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo chkScadAnno.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkScadAnno As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo Label16.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Label16 As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo chkIncludiCliBlocco.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chkIncludiCliBlocco As Global.System.Web.UI.WebControls.CheckBox

    '''<summary>
    '''Controllo btnSblocca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnSblocca As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnCambiaStato.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnCambiaStato As Global.System.Web.UI.WebControls.Button

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
    '''Controllo btnVisualizza.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnVisualizza As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnNuovo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnNuovo As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnNuovoDaOC.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnNuovoDaOC As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnModifica.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnModifica As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo btnElimina.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnElimina As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo lblStampe.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents lblStampe As Global.System.Web.UI.WebControls.Label

    '''<summary>
    '''Controllo btnStampa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnStampa As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo LnkStampa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LnkStampa As Global.System.Web.UI.HtmlControls.HtmlAnchor

    '''<summary>
    '''Controllo btnVerbale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnVerbale As Global.System.Web.UI.WebControls.Button

    '''<summary>
    '''Controllo LnkVerbale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents LnkVerbale As Global.System.Web.UI.HtmlControls.HtmlAnchor

    '''<summary>
    '''Controllo btnOKModInviati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btnOKModInviati As Global.System.Web.UI.WebControls.Button
End Class
