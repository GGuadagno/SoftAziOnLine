<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WF_MenuStatisPR.aspx.vb" Inherits="SoftAziOnLine.WF_MenuStatisPR" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<%--<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Gestione Aziendale - PREVENTIVI</title>
    <link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css"/>
    <link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css"/>
    <link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css"/>
    <link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css"/>  
    <link href="../App_Themes/gridheader.css" rel="stylesheet" type="text/css"/>
    <script src="../JScript/jquery-1.4.4.min.js" type="text/javascript"></script>
<script language="javascript" type="text/javascript">
    var scrollTop;
    
    function Loading() {
        window.history.forward(1)
        
        //Register Begin Request and End Request 
//        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
//        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

//        function BeginRequestHandler(sender, args) {
////            document.body.style.cursor = 'wait';
//            var m = document.getElementById('divGridViewPrevT');
//            if (m != null)
//                scrollTop = m.scrollTop;
//        }
//        function EndRequestHandler(sender, args) {
////            document.body.style.cursor = 'default';
//            var m = document.getElementById('divGridViewPrevT');
//            if (m != null)
//                m.scrollTop = scrollTop;
//        }
    }
    function KeepSessionAlive() {
        console.log('KeepSessionAlive');

        url = "../KeepSessionAlive.ashx?"; // Modificare l'url in produzione
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("GET", url, true);
        xmlHttp.send();
    }

    function captureBarcode() {

        setInterval("KeepSessionAlive()", 5 * 60 * 1000) // 5 minuti

    }
</script>
<style type="text/css">
     .style960
    {
    	width: 960px;
    }
     .btnstyle1R
    {
        Width: 80px;
        height: 30px;
    margin-left: 0px;
    }
     .btnstyle1RL
        {
            Width: 140px;
            height: 30px;
        margin-left: 0px;
        }
    .btnstyle
    {
        Width: 108px;
        height: 40px;
        margin-left: 0px;
        white-space: pre-wrap; 
    }
    .styleMenu
    {
        width: auto;
        border-style:groove;
    }
    .styleBordo
    {
        height: 35px;
        width: 860px;
        border-style:groove;
    }
    .style1
    {
        height: 125px;
    }
</style>    
</head>
<body onload="Loading();captureBarcode();">
    <form id="form1" runat="server">
    <div>
        <asp:ToolkitScriptManager runat="Server" ID="ScriptManager1" 
            enablescriptglobalization="true" enablescriptlocalization="true"  AsyncPostBackTimeOut= "360000">
        </asp:ToolkitScriptManager>
    </div>
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="960px" Height="100%" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />  
    <%--<uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn2" runat="server" Elenco="ListaFornitori" Titolo="Elenco fornitori" />--%>
    <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
            SelectCommand="SELECT -1 AS Codice, '' AS Descrizione UNION ALL SELECT * FROM [Regioni] ORDER BY [Descrizione]">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDSProvince" runat="server" 
            SelectCommand="SELECT '' AS Codice, 'Tutte le province' AS Descrizione UNION ALL SELECT [Codice], [Descrizione] FROM [Province] WHERE [Regione] = @Regione ORDER BY [Codice]">
            <SelectParameters>
                <asp:SessionParameter Name="Regione" DbType="Int32" Direction="Input" SessionField="CodRegione" />
            </SelectParameters>
    </asp:SqlDataSource>   
<table border="1" cellpadding="0" frame="box" style="border-style: Ridge; border-color: inherit; border-width: medium; width:960px; height:35px;">
<tr>
    <td style="width:960px;height:35px;">
        <div>
        <asp:Label ID="labelIdentificaUtente" runat="server" BorderStyle="Ridge" Font-Bold="True" Text="Utente: " ToolTip="Utente" Style="text-align:left" ForeColor="Blue"></asp:Label>
        <asp:Label ID="lblDataOdierna" runat="server" BorderStyle="Ridge" Text="Oggi, Venerdì 2 settembre 2011" Font-Bold="True" Style="text-align:center"></asp:Label>
        </div>
    </td>
</tr>        
</table>
       
  <table border="1" style="vertical-align:middle; background-color:Silver; border-style:ridge; height: 100%; width: 835px;" >
        <td class="style1">
            <asp:Panel ID="PanelSelezionaAgente" BorderStyle="Ridge" style="margin-top: 0px;" runat="server" GroupingText="">
            <table width="100%">
                <tr>
                    <td align="left" style="width:150px">Seleziona Agente</td><td>
                    <asp:DropDownList ID="ddlAgenti" runat="server" DataSourceID="SqlDa_Agenti" 
                        DataTextField="Descrizione" DataValueField="Codice" Width="400px" Height="30px"
                        AppendDataBoundItems="true" Enabled="true">
                        <asp:ListItem Value="0" Text="Agente non definito"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDa_Agenti" runat="server" 
                        SelectCommand="SELECT [Codice], [Descrizione] FROM [Agenti]">
                    </asp:SqlDataSource>
                    </td>
                    <td>
                    <asp:CheckBox ID="chkTuttiAgenti" runat="server" Text="Seleziona tutti" AutoPostBack="true" Checked="false" />
                    </td>
                </tr>
            </table>
            </asp:Panel>
            <%--<asp:Panel ID="PanelLead" BorderStyle="Ridge" style="margin-top: 0px;" runat="server" GroupingText="">
            <table width="100%">
                <tr>
                    <td align="left" style="width:150px">Lead Source</td><td>
                    <asp:DropDownList ID="DDLLead" runat="server" AppendDataBoundItems="True" 
                        AutoPostBack="false" DataSourceID="SqlDSLead" DataTextField="Descrizione" 
                        DataValueField="Codice" Height="30px" Enabled="true" Width="400px">
                        <asp:ListItem Value="0" Text="Non definito"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDSLead" runat="server" 
                        SelectCommand="SELECT * FROM [LeadSource] ORDER BY [Descrizione]">
                    </asp:SqlDataSource>
                    </td>
                    <td>
                    <asp:CheckBox ID="chkTuttiLead" runat="server" Text="Seleziona tutti" AutoPostBack="true" Checked="false" />
                    </td>
                </tr>
            </table>
            </asp:Panel>--%>
            <asp:Panel ID="PanelRegione" BorderStyle="Ridge" style="margin-top: 0px;" runat="server" GroupingText="" Height="60px">
                <table width="100%">
                    <tr valign="top">
                        <td align="left" style="width:150px">Seleziona</td><td>
                        <td style="width:40%;">
                            <asp:Label ID="Label1" runat="server" Width="70px" Height="16px">Regione</asp:Label>
                            <asp:DropDownList ID="ddlRegioni" runat="server" 
                            AutoPostBack="true" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" 
                            DataValueField="Codice" Width="200px" Height="30px" Enabled="false">
                            <asp:ListItem Text="" Value="" ></asp:ListItem>
                            </asp:DropDownList><br />
                            <asp:CheckBox ID="chkTutteRegioni" Text="Seleziona tutte le regioni" runat="server" AutoPostBack="True" Checked="true"/>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Width="70px" Height="16px">Provincia</asp:Label>
                            <asp:DropDownList ID="ddlProvince" runat="server" 
                            AutoPostBack="false" DataSourceID="SqlDSProvince" DataTextField="Descrizione" 
                            DataValueField="Codice" Width="200px" Height="30px" Enabled="false" AppendDataBoundItems="false">
                            <asp:ListItem Text="" Value="" ></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
               </table>
            </asp:Panel>
            <asp:Panel ID="PanelSelezioneCli" BorderStyle="Ridge" style="margin-top: 0px;" runat="server" groupingtext="Seleziona Cliente" Height="113px" Width="835px" Visible="false">
                <div style="height:5px"></div>
                <div>
                    <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Width="100px">Dal codice</asp:Label>
                    <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Height="25px" TabIndex="3" Text="?" Visible="false" Width="25px" />
                    <asp:TextBox ID="txtCodCli1" runat="server" MaxLength="20" Width="100px" AutoPostBack="True"></asp:TextBox>
                    <asp:TextBox ID="txtDesc1" runat="server" MaxLength="150" TabIndex="4" Width="400px"></asp:TextBox>
                    <br>
                    <asp:Label ID="lblAl" runat="server" Height="16px" TabIndex="5" Width="100px">Al codice</asp:Label>
                    <asp:Button ID="btnCercaAnagrafica2" runat="server" class="btnstyle" Height="25px" TabIndex="6" Text="?" Visible="false" Width="25px" />
                    <asp:TextBox ID="txtCodCli2" runat="server" MaxLength="20" TabIndex="7" Width="100px" AutoPostBack="True"></asp:TextBox>
                    <asp:TextBox ID="txtDesc2" runat="server" MaxLength="150" TabIndex="8" Width="400px"></asp:TextBox>
                    <br>
                    <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" TabIndex="10" Text="Seleziona tutti i clienti" />
                <br>  
                </div>                 
            </asp:Panel>
            <br>
            <asp:Panel ID="PanelSelezionaPrev" BorderStyle="Ridge" style="margin-top: 0px;" runat="server" groupingtext="" Height="95px" Width="835px">
                <div style="height:10px"></div>
                <div>
					<asp:Label ID="lblSelPeriodo" runat="server" Height="16px">Selezione: Date Periodo - Stato Preventivi</asp:Label>
				</div>
				<div style="height:10px"></div>
                <div>
                    <asp:Label ID="lblDallaData" runat="server" Height="16px">Dalla data</asp:Label>
                    <asp:TextBox ID="txtDataDa" runat="server" Width="100px" MaxLength="10" TabIndex="1"></asp:TextBox>
                    &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" 
                                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                            ToolTip="apri il calendario" TabIndex="2" CausesValidation="False" />
                                        <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                            TargetControlID="txtDataDa"></asp:CalendarExtender>
                                        &nbsp;
                                        <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                            ControlToValidate="txtDataDa" ErrorMessage="*" 
                                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                    &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px">Alla data</asp:Label>
                    <asp:TextBox ID="txtDataA" runat="server" Width="100px" MaxLength="10" TabIndex="3"></asp:TextBox>
                    &nbsp;
                    <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                            ToolTip="apri il calendario" TabIndex="4" CausesValidation="False"/>
                                        <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                            TargetControlID="txtDataA"></asp:CalendarExtender>
                                        &nbsp;
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                            ControlToValidate="txtDataA" ErrorMessage="*" 
                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                </div>
                <div style="height:5px"></div>
                <div>
                    <asp:RadioButton ID="rbtnConfermati" runat="server" Text="Confermati" AutoPostBack="false" GroupName="Tipo" />
                    <asp:Label ID="Label0" runat="server" Width="30px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnDaConfermare" runat="server" Text="Da confermare" AutoPostBack="false" GroupName="Tipo" />
                    <asp:Label ID="Label2" runat="server" Width="30px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnNonConferm" runat="server" Text="Non confermabile" AutoPostBack="false" GroupName="Tipo" />
                    <asp:Label ID="Label4" runat="server" Width="30px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnChiusoNoConf" runat="server" Text="Chiuso non confermato" AutoPostBack="false" GroupName="Tipo" />
                    <asp:Label ID="Label5" runat="server" Width="30px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" AutoPostBack="false" GroupName="Tipo" Checked="true" />
                </div>
            </asp:Panel>
            <asp:Panel ID="PanelFornitori" runat="server" BorderStyle="Ridge" groupingtext="" style="margin-top: 0px;" Height="75px" Width="835px">
                <br />
                <asp:Label ID="lblFornitore" runat="server" Width="160px" Height="17px">Fornitore</asp:Label>
                <asp:Button ID="btnFornitore" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca fornitore" Enabled="false" Visible="false"/>
                &nbsp;&nbsp;<asp:TextBox ID="txtCodFornitore" runat="server" Width="130px" MaxLength="16" AutoPostBack="True" TabIndex="6" Enabled="false" ></asp:TextBox>
                &nbsp;&nbsp;<asp:TextBox ID="txtDescFornitore" runat="server" Width="400px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox>
                <br>
                <div>
                    <asp:Label ID="Label6" runat="server" Width="165px" Height="16px"></asp:Label>
                    <asp:CheckBox ID="chkTuttiFornitori" runat="server" AutoPostBack="True" TabIndex="8" Text="Seleziona tutti i fornitori" Checked="true" />
                </div>
            </asp:Panel>
            <asp:Panel ID="PanelArticolo" BorderStyle="Ridge" runat="server" groupingtext="" style="margin-top: 0px;" Height="120px" Width="835px">
                <br />
                <asp:Label ID="Label10" runat="server" Height="17px">Seleziona Articoli</asp:Label>
                <br />
                <asp:Label ID="Label7" runat="server" Width="160px" Height="17px">Dal codice</asp:Label>
                <asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca articolo" Enabled="false" Visible="false"/>
                &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server" Width="130px" MaxLength="20" AutoPostBack="True" TabIndex="9" Enabled="false"></asp:TextBox>
                &nbsp;&nbsp;<asp:TextBox ID="txtDesArt1" runat="server" Width="400px" MaxLength="150" TabIndex="10" Enabled="False" ></asp:TextBox>
                <br />
                <asp:Label ID="Label9" runat="server" Width="160px" Height="17px">Al codice</asp:Label>
                <asp:Button ID="btnCod2" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca articolo" Enabled="false" Visible="false"/>
                &nbsp;&nbsp;<asp:TextBox ID="txtCod2" runat="server" Width="130px" MaxLength="20" AutoPostBack="True" TabIndex="9" Enabled="false" ></asp:TextBox>
                &nbsp;&nbsp;<asp:TextBox ID="txtDesArt2" runat="server" Width="400px" MaxLength="150" TabIndex="10" Enabled="False"></asp:TextBox>
                <br>
                <asp:Label ID="lblTuttiArticoli" runat="server" Height="16px" Width="165px"></asp:Label>
                <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" TabIndex="10" text="Seleziona tutti gli articoli" Checked="true" />
                <br />
            </asp:Panel>
            <table width="835px">
            <tr>
                <td>
                <asp:Panel ID="PanelSintAnal" BorderStyle="Ridge" style="margin-top: 0px;" runat="server" groupingtext="Selezione tipo riepilogo" Height="50px" 
                    Width="300px" Visible="true">
                    <asp:RadioButton ID="rbtnSintetico" runat="server" Text="Sintetico" Checked="true" GroupName="SintAnal" AutoPostBack="True"/>
                    &nbsp;
                    <asp:RadioButton ID="rbtnAnalitico" runat="server" Text="Analitico" GroupName="SintAnal" AutoPostBack="True" Checked="false"/>
                </asp:Panel>
                </td>
                <td>
                 <asp:Panel ID="PanelOrdinamento" BorderStyle="Ridge" style="margin-top: 0px;" runat="server" groupingtext="Selezione tipo Ordine di stampa" Height="50px" 
                    Width="550px" Visible="true">
                    <asp:RadioButton ID="rbtnOrdAgPrevCli" runat="server" Text="Agente/Preventivo/Cliente" Checked="true" GroupName="OrdineSt" AutoPostBack="false"/>
                    &nbsp;
                    <asp:RadioButton ID="rbtOrdAgCliPrev" runat="server" Text="Agente/Cliente/Preventivo" GroupName="OrdineSt" AutoPostBack="false" Checked="false"/>
                </asp:Panel>
                </td>
            </tr>
            </table>
            <br />
        </td>
        <td align="left" class="style1">
            <%--<asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>--%>
                    <div>
                        <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="STAMPA" TabIndex="20" CausesValidation="False" Font-Bold="true" BorderStyle="Outset"/>
                    </div>
                    <div style="height: 15px"></div>
                    <div>
                        <asp:CheckBox ID="chkStampaPDF" runat="server" AutoPostBack="false" TabIndex="10" Checked="false" Text="Stampa PDF" />
                    </div>
                    <div style="height: 15px"></div>
                    <div>
                        <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="APRI STAMPA"  style="border-style:outset;text-decoration:none;height:40px;width:108px;background-color:Green;color:White;">Apri Stampa</a>
                        <a ID="LnkApriStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="APRI STAMPA"  style="border-style:outset;text-decoration:none;height:40px;width:108px;background-color:Green;color:White;">Apri Stampa</a>
                    </div>
                    <div style="height: 15px"></div>
                    <div>
                        <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="ATTIVITA' CONTRATTI" TabIndex="21" CausesValidation="False" ForeColor="White" Font-Bold="true" BorderStyle="Outset" BackColor="DarkGreen"/>
                    </div>
                    <div style="height: 15px"></div>
                    <div style="height: 15px"></div>
                    <div>
                        <asp:Button ID="btnUscita" runat="server" class="btnstyle" Text="USCITA" TabIndex="21" CausesValidation="False" ForeColor="White" Font-Bold="true" BorderStyle="Outset" BackColor="DarkRed"/>
                    </div>
                <%--</ContentTemplate>
            </asp:UpdatePanel>--%>
        </td>
        </caption>
    </table>
<table border="1" cellpadding="0" frame="box" style="border-style: Ridge; border-color: inherit; border-width: medium; width:960px; height:60%;">
    <tr>
       <td>
        <div>
            <asp:Label ID="Label26" runat="server" Width="1px"></asp:Label>
            <asp:Label ID="lblMessAttivita" runat="server" BorderStyle="Ridge" Text="" Font-Bold="false" Visible="true" Style="text-align:left" BackColor="#6B696B" ForeColor="White" Width="940px"></asp:Label>
        </div>
       </td>
    </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter ="1">
    <ProgressTemplate>
         <div id="updPrgPnlHeaderDefault1_master" role="status" aria-hidden="true">
                <div id="PrgTmplPnlPaging11_master" class="boOverlay">
                    <div id="PrgTmplPnlPaging21_master" class="boLoaderCircle">
                                <div style="width:64px; height:64px;"></div>
                         </div>
                   </div>
            </div>
    </ProgressTemplate> 
</asp:UpdateProgress>
</asp:Panel>
<div style="height: 9px; width: 960px;">
    <div class="styleCopyright960">Copyright © 2011-2023 Soft Solutions S.r.l.</div>
</div>
    </form>
</body>
<script type="text/javascript">
    var ismodal = 0;
    function InIEvent() {
        //        $(this).keydown(function(event) {
        //            console.log('KEYDOWN');
        //            if (event.which == 13) {
        //                event.preventDefault();
        //    
        //            }

        //        });

        if ($('#LnkStampa').is(':visible')) {
            //            console.log('visible');
            var href = $('#LnkStampa').attr('href');
            window.open(href);  //causes the browser to refresh and load the requested url
            $('#LnkStampa').hide();
        } else {
            //            console.log('non visible');
        }
    }
    $(document).ready(InIEvent);
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(InIEvent);
    </script>
</html>
