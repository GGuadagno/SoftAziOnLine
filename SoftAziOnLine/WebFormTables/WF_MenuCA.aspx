<%@ Language="vb" AutoEventWireup="false" CodeBehind="WF_MenuCA.aspx.vb" Inherits="SoftAziOnLine.WF_MenuCA" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title>Gestione Aziendale - CONTRATTI</title>
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
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        function BeginRequestHandler(sender, args) {
//            document.body.style.cursor = 'wait';
            var m = document.getElementById('divGridViewPrevT');
            if (m != null)
                scrollTop = m.scrollTop;
        }
        function EndRequestHandler(sender, args) {
//            document.body.style.cursor = 'default';
            var m = document.getElementById('divGridViewPrevT');
            if (m != null)
                m.scrollTop = scrollTop;
        }
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
     .styleGridT
    {
        width: auto;
        border-style: none;
        height: 260px;
    }
     .styleGridTAtt
    {
        width: auto;
        border-style: none;
        height: 240px;
    }
</style>
</head>
<%--<body onload="Loading();captureBarcode();"> --%>
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
<%--<asp:UpdatePanel ID="UpdatePanel2" runat="server">
    <ContentTemplate> --%>   
<table border="1" cellpadding="0" frame="box" style="border-style: Ridge; border-color: inherit; border-width: medium; width:960px; height:80px;">
<tr>
    <td style="width:960px;height:150px;">
        <div>
        <asp:Label ID="labelIdentificaUtente" runat="server" BorderStyle="Ridge" Font-Bold="True" Text="Utente: " ToolTip="Utente" Style="text-align:left" ForeColor="Blue"></asp:Label>
        <asp:CheckBox ID="chkRespArea" runat="server" Font-Bold="false" ForeColor="Blue" Text="Responsabile Area" Checked="false" AutoPostBack="true"/>
        <asp:Label ID="lblDataOdierna" runat="server" BorderStyle="Ridge" Text="Oggi, Venerdì 2 settembre 2011" Font-Bold="True" Style="text-align:center"></asp:Label>
        <asp:Label ID="lblUltimoAccesso" runat="server" BorderStyle="Ridge" Text="Accesso precedente: Giovedì 1 settembre 2011, 09:00" Font-Bold="true" Visible="false" Style="text-align:right"></asp:Label>
        <asp:LinkButton ID="LnkLogOut" runat="server" Text="USCITA" Visible="true" BorderColor="Snow" Font-Bold="false" Style="text-align:center;width:100px" Font-Size="Medium" ForeColor="White" Font-Names="Arial" BorderStyle="Outset" BackColor="DarkRed" Font-Underline="false" CausesValidation="False"></asp:LinkButton>
        <asp:LinkButton ID="LnkUltimaSess" runat="server" Text="Recupera Ultima sessione" Visible="false" BorderColor="Snow" Font-Bold="false" Style="text-align:center;width:200px" Font-Size="Medium" ForeColor="White" Font-Names="Arial" BorderStyle="Outset" BackColor="DarkGreen" Font-Underline="false" CausesValidation="False" OnClientClick="__doPostBack('LnkUltimaSessOK','');"></asp:LinkButton>
        <asp:LinkButton ID="LnkMenuStatisPR" runat="server" Text="PREVENTIVI" Visible="false" BorderColor="Snow" Font-Bold="false" Style="text-align:center;width:100px" Font-Size="Medium" ForeColor="White" Font-Names="Arial" BorderStyle="Outset" BackColor="DarkGreen" Font-Underline="false" CausesValidation="False"></asp:LinkButton>
        </div>
        <div style="height:35px;">
            <asp:Label ID="Label21" runat="server" Style="text-align:left" text="Ricerca per Cliente/Sedi" ForeColor="Blue" Width="210px"></asp:Label>
            <asp:TextBox  ID="txtRicercaClienteSede" runat="server" Width="400px" Height="30px" MaxLength="50" BorderStyle="None" AutoPostBack="true" Enabled="true"></asp:TextBox>
            <%--<asp:Label ID="Label22" runat="server" Width="05px"></asp:Label>--%>
            <asp:Label ID="Label23" runat="server" Text="o per N° Contratto" ForeColor="Blue"></asp:Label>
            <asp:TextBox  ID="txtRicercaNContr" runat="server" Width="50px" Height="30px" MaxLength="6" BorderStyle="None" AutoPostBack="true" Enabled="true"></asp:TextBox>
        </div>
        <div style="height:10px;"></div>
        <div style="height:35px;">
        <asp:Label ID="Label6" runat="server" Style="text-align:center" Width="5%">Cliente</asp:Label>
        <asp:DropDownList ID="ddlClienti" runat="server" AutoPostBack="true" Width="90%" Height="30px" BorderStyle="None"></asp:DropDownList>
        </div>
        <div style="height:05px;"></div>
        <div style="height:35px;">
        <asp:Label ID="Label14" runat="server" Style="text-align:center" Width="5%">Sedi</asp:Label>
        <asp:DropDownList ID="DDLSediApp" runat="server" AutoPostBack="true" Width="90%" Height="30px" BorderStyle="None"></asp:DropDownList>
        </div>
    </td>
</tr>        
</table>
<%--    </ContentTemplate>
</asp:UpdatePanel>--%>
<table border="1" cellpadding="0" frame="box" style="border-style: Ridge; border-color: inherit; border-width: medium; width:960px; height:60px;">
<tr>
    <td style="width:960px;height:60px;">
        <div>
         <asp:Label ID="Label9" runat="server" Font-Bold="true" BorderStyle="Outset" Width="75px">Scadenze:</asp:Label>
         <asp:Label ID="Label10" runat="server">dal</asp:Label>
         <asp:TextBox ID="txtDallaData" runat="server" AutoPostBack="true" MaxLength="10" TabIndex="5" Width="70px" BorderStyle="None"></asp:TextBox>
            <asp:ImageButton ID="imgBtnShowCalendarDD" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario"/>
            <asp:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDD" TargetControlID="txtDallaData">
            </asp:CalendarExtender>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtDallaData" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
         <asp:Label ID="Label8" runat="server">al</asp:Label>
         <asp:TextBox ID="txtAllaData" runat="server" AutoPostBack="true" MaxLength="10" TabIndex="5" Width="70px" BorderStyle="None"></asp:TextBox>
            <asp:ImageButton ID="imgBtnShowCalendarAD" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario"/>
            <asp:CalendarExtender ID="txtAllaData_CalendarExtender" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarAD" TargetControlID="txtAllaData">
            </asp:CalendarExtender>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtAllaData" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
        <asp:CheckBox ID="chkSoloDaEv" runat="server" Font-Bold="false" ForeColor="Blue" Text="da fare" Checked="true" AutoPostBack="true"/>
        <asp:Button ID="btnRicerca" runat="server" Text="Aggiorna" class="btnstyle1R" />
        <asp:CheckBox ID="chkSingoloNS" runat="server" Font-Bold="false" ForeColor="Blue" Text="Visualizza Attività singolo Seriale" Checked="true" AutoPostBack="true"/>
        </div>
        <div>
        <asp:Label ID="Label16" runat="server" Font-Bold="true" BorderStyle="Outset" Width="75px">Stampe:</asp:Label>
        <asp:Button ID="btnVerbale" runat="server" class="btnstyle1R" Text="Verbale" />
        <a ID="LnkVerbale" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Verbale"  style="border-style:outset;text-decoration:none;height:30px;background-color:Green;color:White;">Apri Verbale</a>
        <a ID="LnkApriVerbale" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Verbale"  style="border-style:outset;text-decoration:none;height:30px;background-color:Green;color:White;">Apri Verbale</a>
        <asp:Label ID="Label15" runat="server" Width="15px"></asp:Label>
        <asp:Button ID="btnElencoSc" runat="server" class="btnstyle1R" Text="Elenco" />
        <asp:CheckBox ID="chkElencoXLS" runat="server" Font-Bold="false" ForeColor="Black" Text="EXCEL" Checked="false" AutoPostBack="false"/>
        <a ID="lnkElencoSc" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Elenco Sc."  style="border-style:outset;text-decoration:none;height:30px;background-color:Green;color:White;">Apri Elenco</a>
        <asp:Label ID="Label0" runat="server" Text="Ordine elenco per:" ForeColor="Blue"></asp:Label>
        <asp:RadioButton ID="rbtnOrdCliente" runat="server" Text="Cliente" AutoPostBack="true" GroupName="TipoOrdine" Checked="true" />
        <asp:RadioButton ID="rbtnOrdScadenza" runat="server" Text="Scadenza" AutoPostBack="true" GroupName="TipoOrdine" />
        <asp:Label ID="Label4" runat="server" Width="5px"></asp:Label>
        <asp:CheckBox ID="chkScadAnno" runat="server" Font-Bold="true" ForeColor="Blue" Text="Stampa scadenze Anno" Checked="true" AutoPostBack="false"/>
        </div>
    </td>
</tr>
</table>
<table border="1" cellpadding="0" frame="box" style="border-style: Ridge; border-color: inherit; border-width: medium; width:960px; height:200px;">
    <tr>
       <td>
            <div id="divGridViewPrevT" style="overflow:auto; width:940px;height:200px;border-style:groove;">                
                <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                    CssClass="GridViewStyle" 
                    EmptyDataText="Nessun dato disponibile. VERIFICARE LE DATE DI SCADENZE INSERITE, E LA DATA INDICATA NELL'ELENCO Singolo Cliente "  
                    DataKeyNames="IDDocumenti" 
                    AllowPaging="false"
                    PageSize="6" 
                    PagerStyle-HorizontalAlign="Center" 
                    PagerSettings-Mode="NextPreviousFirstLast"
                    PagerSettings-Visible="true"
                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                    EnableTheming="True" GridLines="None"
                    BackColor="Silver" AllowSorting="True">
                    <%--<RowStyle CssClass="RowStyle" />--%>
                    <rowstyle backcolor="LightCyan" forecolor="DarkBlue"/>
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <%--<AlternatingRowStyle CssClass="AltRowStyle" />--%>
                    <alternatingrowstyle backcolor="PaleTurquoise" forecolor="DarkBlue"/>
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>
                    <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                        LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                        Mode="NextPreviousFirstLast" 
                        NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                        PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                    <Columns>
                        <asp:CommandField ButtonType="Button" CausesValidation="False" 
                            ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                            <ControlStyle Font-Size="XX-Small" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                        </asp:CommandField>
                         <asp:BoundField DataField="DesRespArea" 
                            HeaderText="RespArea" SortExpression="DesRespArea"><HeaderStyle Wrap="True" Width="100px" CssClass="nascondi"/>
                            <ItemStyle Width="100px" CssClass="nascondi" /></asp:BoundField>
                          <asp:BoundField DataField="DesRespVisite" 
                            HeaderText="RespVisite" SortExpression="DesRespVisite"><HeaderStyle Wrap="True" Width="100px" />
                            <ItemStyle Width="100px"/></asp:BoundField>
                         <asp:BoundField DataField="DataSc" HeaderText="Data scadenza" 
                            SortExpression="DataSc"><HeaderStyle Wrap="true" Width="10px"/><ItemStyle 
                            Width="10px" Wrap="false"/></asp:BoundField>
                         <asp:BoundField DataField="Cod_Articolo" 
                            HeaderText="Codice articolo" ReadOnly="True" 
                            SortExpression="Cod_Articolo">
                            <HeaderStyle Wrap="false" Width="20px" CssClass="nascondi"/>
                            <ItemStyle Width="20px" Wrap="false" CssClass="nascondi" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                            HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" Width="350px"/><ItemStyle 
                            Width="350px" /></asp:BoundField>
                        <asp:BoundField DataField="SerieLotto" 
                            HeaderText="Serie" SortExpression="SerieLotto"><HeaderStyle Wrap="false" Width="100px"/><ItemStyle 
                            Width="100px" Wrap="false"/></asp:BoundField>
                        <asp:BoundField DataField="Modello" HeaderText="Mod." 
                            SortExpression="Modello"><HeaderStyle Wrap="True" Width="05px" HorizontalAlign="Center"/>
                            <ItemStyle Width="05px" HorizontalAlign="Center"/></asp:BoundField> 
                        <asp:BoundField DataField="NoteApp" HeaderText="Note App." 
                            SortExpression="NoteApp"><HeaderStyle Wrap="True" Width="05px" HorizontalAlign="Center" />
                            <ItemStyle Width="05px" HorizontalAlign="Center"/></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                            SortExpression="UM"><HeaderStyle Wrap="True" Width="05px" HorizontalAlign="Center" CssClass="nascondi"/>
                            <ItemStyle Width="05px" HorizontalAlign="Center" CssClass="nascondi"/></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" 
                            SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                            Width="10px" CssClass="nascondi"/></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità evasa" 
                            SortExpression="Qta_Evasa"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                            Width="10px" CssClass="nascondi"/></asp:BoundField> 
                        <asp:BoundField DataField="Rag_SocDenom" HeaderText="Ragione Sociale" 
                            SortExpression="Rag_SocDenom">
                            <HeaderStyle Wrap="false" />
                            <ItemStyle Width="10px" Wrap="true" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="RagSocApp" 
                            HeaderText="Rag.Soc:App." ReadOnly="True" 
                            SortExpression="RagSocApp">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="10px"/>
                        </asp:BoundField>
                         <asp:BoundField DataField="IndirApp" HeaderText="Indir.App." 
                            SortExpression="IndirApp"><HeaderStyle Wrap="false" Width="10px"/><ItemStyle 
                            Width="10px" Wrap="false" /></asp:BoundField>
                         <asp:BoundField DataField="LocApp" HeaderText="Luogo App." 
                            SortExpression="LocApp"><HeaderStyle Wrap="false" Width="10px"/><ItemStyle 
                            Width="10px" /></asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="CAPApp" 
                            HeaderText="CAP"  
                            SortExpression="CAPApp">
                            <HeaderStyle Wrap="false"/>
                            <ItemStyle Width="5px" Wrap="false"/>
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="PrApp" 
                            HeaderText="Pr."  
                            SortExpression="PrApp">
                            <HeaderStyle Wrap="false"/>
                            <ItemStyle Width="5px" Wrap="false"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Importo" HeaderText="Importo" 
                            SortExpression="Importo"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                            Width="10px" CssClass="nascondi"/></asp:BoundField>
                        
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Telefono12" 
                            HeaderText="Telefono1-2"  
                            SortExpression="Telefono12">
                            <HeaderStyle Wrap="false" Width="5px"/>
                            <ItemStyle Width="20px" Wrap="false"/>
                        </asp:BoundField>
                        
                        <asp:BoundField DataField="Referente" HeaderText="Referente" SortExpression="Referente">
                            <HeaderStyle Wrap="true" Width="5px"/>
                            <ItemStyle Width="25px" Wrap="false"/>
                        </asp:BoundField>
                            
                        <asp:BoundField DataField="EmailVerbale" HeaderText="Email invio Verbale" 
                            SortExpression="EmailVerbale"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" Wrap="true"/></asp:BoundField>
                            
                        <asp:BoundField DataField="NoteSerieLotto" HeaderText="Note Intervento" 
                            SortExpression="NoteSerieLotto"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" Wrap="true"/></asp:BoundField>
                            
                        <asp:BoundField DataField="Numero" HeaderText="N°" 
                            SortExpression="Numero">
                            <HeaderStyle Wrap="false"/>
                            <ItemStyle Width="10px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DesTipoDettR" HeaderText="Riga" 
                            SortExpression="DesTipoDettR"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" /></asp:BoundField> 
                        <asp:BoundField DataField="Data_Doc" HeaderText="Data Documento" 
                            SortExpression="Data_Doc">
                            <HeaderStyle Wrap="true"/>
                            <ItemStyle Width="15px" Wrap="false"/>
                        </asp:BoundField>
                        
                        <asp:BoundField DataField="Cod_Cliente" HeaderText="Codice Cliente" 
                            SortExpression="Cod_Cliente">
                            <HeaderStyle Wrap="True"/>
                            <ItemStyle Width="10px" Wrap="false"/>
                        </asp:BoundField>
                       
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                            HeaderText="Località"  
                            SortExpression="Localita">
                            <HeaderStyle Wrap="false"/>
                            <ItemStyle Width="15px" Wrap="false"/>
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                            HeaderText="CAP"  
                            SortExpression="CAP">
                            <HeaderStyle Wrap="false"/>
                            <ItemStyle Width="5px" Wrap="false"/>
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Provincia" 
                            HeaderText="Pr."  
                            SortExpression="Provincia">
                            <HeaderStyle Wrap="false"/>
                            <ItemStyle Width="5px" Wrap="false"/>
                        </asp:BoundField>
                        <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                            SortExpression="Riferimento">                                    
                            <HeaderStyle Wrap="True" CssClass="nascondi"/>
                            <ItemStyle Width="50px" CssClass="nascondi"/>
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione1" 
                            HeaderText="Destinazione(1)"  
                            SortExpression="Destinazione1">
                            <HeaderStyle Wrap="True"/>
                            <ItemStyle Width="25px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione2" 
                            HeaderText="Destinazione(2)"  
                            SortExpression="Destinazione2">
                            <HeaderStyle Wrap="True"/>
                            <ItemStyle Width="25px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione3" 
                            HeaderText="Destinazione(3)"  
                            SortExpression="Destinazione3">
                            <HeaderStyle Wrap="True"/>
                            <ItemStyle Width="25px" />
                        </asp:BoundField>
                         <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                            SortExpression="DesStatoDoc"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" /></asp:BoundField>
                          <asp:BoundField ApplyFormatInEditMode="True" DataField="DurataNumRiga" 
                                HeaderText="DNR"  
                                SortExpression="DurataNumRiga">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="5px"/>
                            </asp:BoundField>
                            
                    </Columns>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                    SelectCommand="get_ElencoScadAG" 
                    SelectCommandType="StoredProcedure">
                    <SelectParameters>
                        <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                        <asp:Parameter Name="DallaData" Type="String" />
                        <asp:Parameter Name="AllaData" Type="String" />
                        <asp:Parameter Name="Escludi" Type="Boolean" />
                        <asp:Parameter Name="RespArea" Type="Int32" />
                        <asp:Parameter Name="RespVisite" Type="Int32" />
                        <asp:Parameter Name="Causale" Type="Int32" />
                        <asp:Parameter Name="SoloDaEv" Type="Boolean" />
                    </SelectParameters>
                </asp:SqlDataSource>
        </div>
            <div>
                <asp:Label ID="Label13" runat="server" Width="1px"></asp:Label>
                <asp:Label ID="Label3" runat="server" Text="E-Mail invio Verbale" Font-Bold="true" Width="170px"></asp:Label>
                <asp:TextBox  ID="txtEmailInvio" runat="server" Width="670px" MaxLength="100" BorderStyle="None" AutoPostBack="false" Enabled="false"></asp:TextBox>
            </div>
            <div style="height: 5px">&nbsp;</div>
            <div>
                <asp:Label ID="Label12" runat="server" Width="1px"></asp:Label>
                <asp:Label ID="lblNoteIntervento" runat="server" Text="Note Intervento" Font-Bold="true" Width="170px"></asp:Label>
                <asp:TextBox  ID="txtNoteDocumento" runat="server" Width="670px" Height="45px" TextMode="MultiLine" BorderStyle="None" AutoPostBack="false" Enabled="false"></asp:TextBox>
            </div>
            <div>
                <asp:Label ID="Label17" runat="server" Width="1px"></asp:Label>
                <asp:Label ID="lblInfoNoteIntervento" runat="server" Text="NOTA le Note intervento/Alla presenza, saranno assegnate solo N° Serie Eseguiti/Modificati in questo aggiornamento." Font-Bold="true" ForeColor="Blue" Visible="false"></asp:Label>
            </div>
            <div style="height: 5px">&nbsp;</div>
            <div>
                <asp:Label ID="Label24" runat="server" Width="1px"></asp:Label>
                <asp:Label ID="lblAllaPresenzaDi" runat="server" Text="Alla presenza di " Font-Bold="true" Width="170px"></asp:Label>
                <asp:TextBox  ID="txtAllaPresezaDi" runat="server" Width="670px" MaxLength="200" BorderStyle="None" AutoPostBack="false" Enabled="false"></asp:TextBox>
            </div>
        </td>
    </tr>
</table>    
<table border="1" cellpadding="0" frame="box" style="border-style: Ridge; border-color: inherit; border-width: medium; width:960px; height:60%;">
    <tr>
       <td>
       <div id="divGridViewDettCAAtt" style="overflow:auto; width:940px;border-style:groove;">
            <asp:GridView ID="GridViewDettCAAtt" runat="server" AutoGenerateColumns="False" 
                EmptyDataText="Nessun dato disponibile."  
                DataKeyNames="Riga" 
                GridLines="None" CssClass="GridViewStyle" EnableTheming="True"
                AllowPaging="false" 
                PagerSettings-Mode="NextPreviousFirstLast"
                PagerSettings-Visible="false"
                PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" >
                <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                    LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                    Mode="NextPreviousFirstLast" 
                    NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                    PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                <%--<RowStyle CssClass="RowStyle" />--%>
                <rowstyle backcolor="LightCyan" forecolor="DarkBlue"/>
                <PagerStyle CssClass="PagerStyle" />
                <HeaderStyle CssClass="HeaderStyle" />
                <%--<AlternatingRowStyle CssClass="AltRowStyle" />--%>
                <alternatingrowstyle backcolor="PaleTurquoise" forecolor="DarkBlue"/>
                <SelectedRowStyle CssClass="SelectedRowStyle"/>
                <Columns>
                    <%--<asp:CommandField ButtonType="Button" CausesValidation="False" 
                            ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                            <ControlStyle Font-Size="XX-Small" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                        </asp:CommandField>--%>
                    <asp:BoundField DataField="Riga" HeaderText="N°" 
                        SortExpression="Riga">
                        <HeaderStyle Wrap="True" HorizontalAlign="Center" />
                        <ItemStyle Width="5px" HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:BoundField DataField="SerieLotto" HeaderText="Serie" 
                        SortExpression="SerieLotto">
                        <HeaderStyle Wrap="false" />
                        <ItemStyle Width="50px" Wrap="false"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" 
                        SortExpression="Descrizione">
                        <HeaderStyle Wrap="true" />
                        <ItemStyle Width="250px" Wrap="true" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Eseguita" ItemStyle-HorizontalAlign="Center" SortExpression="Qta_Selezionata">
                        <ItemTemplate>
                            <asp:CheckBox id="chkEvasa" runat="server" Enabled="true" AutoPostBack="false" Checked='<%# Convert.ToBoolean(Eval("Qta_Selezionata")) %>'>
                            </asp:CheckBox> 
                        </ItemTemplate>
                        <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="TextDataSc" HeaderText="Data scadenza attività" 
                            SortExpression="TextDataSc"><HeaderStyle Wrap="true" Width="10px"/><ItemStyle 
                            Width="10px" Wrap="false"/></asp:BoundField>
                    <%--<asp:TemplateField HeaderText="Data scadenza" SortExpression="TextDataSc">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDataSC" runat="server" Width="70px" MaxLength="10" Text='<%# Bind("TextDataSc") %>' BorderColor="White"></asp:TextBox>
                             <asp:ImageButton ID="imgBtnShowCalendar" runat="server" 
                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                ToolTip="apri il calendario" />
                            <asp:CalendarExtender ID="txtDataSC_CalendarExtender" runat="server" 
                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar" 
                                TargetControlID="txtDataSC">
                            </asp:CalendarExtender>
                            <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                ControlToValidate="txtDataSC" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        </ItemTemplate>
                        <HeaderStyle Width="70px" Wrap="false" />
                        <ItemStyle Width="70px" />
                    </asp:TemplateField>--%>
                    <asp:TemplateField HeaderText="Data Esecuzione" SortExpression="DataEv">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDataEV" runat="server" Width="70px" MaxLength="10" Text='<%# Bind("TextDataEv") %>' BorderColor="White"></asp:TextBox>
                             <asp:ImageButton ID="imgBtnShowCalendarDEV" runat="server" 
                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                ToolTip="apri il calendario" />
                            <asp:CalendarExtender ID="txtDataEV_CalendarExtender" runat="server" 
                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDEV" 
                                TargetControlID="txtDataEV">
                            </asp:CalendarExtender>
                            <%--<asp:RegularExpressionValidator ID="DateRegexValidatorEV" runat="server" 
                                ControlToValidate="txtDataEV" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                        </ItemTemplate>
                        <HeaderStyle Width="70px" Wrap="false" />
                        <ItemStyle Width="70px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Data scadenza consumabile" SortExpression="RefDataNC">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDataEVN" runat="server" Width="70px" MaxLength="10" Text='<%# Bind("TextRefDataNC") %>' BorderColor="White"></asp:TextBox>
                             <asp:ImageButton ID="imgBtnShowCalendarDEVN" runat="server" 
                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                ToolTip="apri il calendario" />
                            <asp:CalendarExtender ID="txtDataEVN_CalendarExtender" runat="server" 
                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDEVN" 
                                TargetControlID="txtDataEVN">
                            </asp:CalendarExtender>
                            <%--<asp:RegularExpressionValidator ID="DateRegexValidatorEVN" runat="server" 
                                ControlToValidate="txtDataEVN" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                        </ItemTemplate>
                        <HeaderStyle Width="70px" Wrap="true" />
                        <ItemStyle Width="70px" />
                    </asp:TemplateField>
                    <%--<asp:TemplateField HeaderText="Note" SortExpression="Note">
                        <ItemTemplate>
                            <asp:TextBox ID="Note" runat="server" Width="70px" MaxLength="30" Text='<%# Bind("Note") %>' BorderColor="White"></asp:TextBox>
                            </ItemTemplate>
                        <HeaderStyle Width="300px" Wrap="false" />
                        <ItemStyle Width="300px" Wrap="false"  />
                    </asp:TemplateField>--%>
                    <asp:BoundField DataField="SWModAgenti" HeaderText="CK" 
                        SortExpression="SWModAgenti">
                        <HeaderStyle Wrap="True" HorizontalAlign="Center" CssClass="nascondi"/>
                        <ItemStyle Width="5px" HorizontalAlign="Center" CssClass="nascondi"/>
                    </asp:BoundField>
                    <asp:BoundField DataField="TipoScontoMerce" HeaderText="Mod" 
                        SortExpression="TipoScontoMerce">
                        <HeaderStyle Wrap="True" HorizontalAlign="Center" CssClass="nascondi"/>
                        <ItemStyle Width="5px" HorizontalAlign="Center" CssClass="nascondi" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </div>
        <div>
            <asp:Label ID="Label26" runat="server" Width="1px"></asp:Label>
            <asp:Label ID="lblMessAttivita" runat="server" BorderStyle="Ridge" Text="" Font-Bold="false" Visible="true" Style="text-align:left" BackColor="#6B696B" ForeColor="White" Width="940px"></asp:Label>
        </div>
        <div>
            <asp:Label ID="Label1" runat="server" Width="1px"></asp:Label>
            <asp:Button ID="btnModScAttCA" runat="server" Width="220px" Height="30px" Text="Modifica dati Attività" />
            <asp:Button ID="btnAggScAttCA" runat="server" Width="105px" Height="30px" Text="Salva modifica" ForeColor="White" BackColor="DarkRed" Visible="false" />
            <asp:Button ID="btnAnnScAttCA" runat="server" Width="115px" Height="30px" Text="Annulla modifica" ForeColor="White" BackColor="DarkRed" Visible="false"/>
            <asp:CheckBox ID="chkAllScCA" runat="server" Font-Bold="true" ForeColor="Blue" Text="Mostra tutti anni contratto" Checked="false" AutoPostBack="true"/>
            <asp:Label ID="Label11" runat="server" Width="50px"></asp:Label>
            <asp:CheckBox ID="chkEvadiTutte" runat="server" Font-Bold="true" ForeColor="Blue" Text="Seleziona tutte" Checked="false" AutoPostBack="true"/>
            <asp:Label ID="Label19" runat="server" Width="50px"></asp:Label>
            <asp:Label ID="Label18" runat="server" Font-Bold="true" BorderStyle="Outset" Width="75px">Stampe:</asp:Label>
            <asp:Button ID="btnVerbale2" runat="server" class="btnstyle1R" Text="Verbale" />
            <a ID="LnkApriVerbale2" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Verbale"  style="border-style:outset;text-decoration:none;height:30px;background-color:Green;color:White;">Apri Verbale</a>
        </div>
        <div>
            <asp:Label ID="Label2" runat="server" Width="1px"></asp:Label>
            <asp:Label ID="Label7" runat="server" BorderStyle="Outset" style="text-align:left" Font-Bold="true">Totale attività</asp:Label>
            <asp:Label ID="lblTotaleAtt" runat="server" BorderStyle="Outset" style="text-align:left" Font-Bold="True">0</asp:Label>
            <asp:Label ID="Label5" runat="server" BorderStyle="none" style="text-align:left" Font-Bold="false" ForeColor="Blue" Text="Attività Evase senza Data sarà impostata la data odierna"></asp:Label>
            <asp:Label ID="Label20" runat="server" Width="50px"></asp:Label>
            <asp:CheckBox ID="chkSingoloNS2" runat="server" Font-Bold="false" ForeColor="Blue" Text="Visualizza Attività singolo Seriale" Checked="true" AutoPostBack="true"/>
        </div>
        <div>
            <asp:Label ID="Label25" runat="server" Width="1px"></asp:Label>
            <asp:Label ID="lblMessDebug" runat="server" BorderStyle="Ridge" Text="" Font-Bold="false" Visible="false" Style="text-align:left" BackColor="DarkRed" ForeColor="White" Width="940px"></asp:Label>
        </div>
       </td>
    </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>
<%--<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel2" DisplayAfter ="1">
    <ProgressTemplate>
         <div id="updPrgPnlHeaderDefault12_master" role="status" aria-hidden="true">
                <div id="PrgTmplPnlPaging112_master" class="boOverlay">
                    <div id="PrgTmplPnlPaging212_master" class="boLoaderCircle">
                                <div style="width:64px; height:64px;"></div>
                         </div>
                   </div>
            </div>
    </ProgressTemplate> 
</asp:UpdateProgress>--%>
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
    <div class="styleCopyright960">Copyright © 2011-2024 Soft Solutions S.r.l.</div>
</div>
<%--<div id="updPrgPnlHeaderDefault1_master"  role="status" aria-hidden="true">
        <div id="PrgTmplPnlPaging11_master" class="boOverlay">
            <div id="PrgTmplPnlPaging21_master" class="boLoaderCircle">
                    <div style="width:64px; height:64px;"></div>
             </div>
        </div>
    </div>--%>
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

        if ($('#LnkVerbale').is(':visible')) {
//            console.log('visible');
            var href = $('#LnkVerbale').attr('href');
            window.open(href);  //causes the browser to refresh and load the requested url
            $('#LnkVerbale').hide();
        } else {
//            console.log('non visible');
        }
//        console.log('InIEvent hide');
//        $('#updPrgPnlHeaderDefault1_master').hide();
//        $('.modalPopup input[type="submit"]').click(function(e) {
////            console.log('modalPopup hide');
//            $('#updPrgPnlHeaderDefault1_master').hide();
//            ismodal = 1;
////            console.log('.modalPopup ismodal = 1');
//        });
//        $('input[type="submit"]').click(function() {
//        console.log('.click(function()');
//        if (ismodal == 0) {
//            console.log('ismodal == 0 show');
//            $('#updPrgPnlHeaderDefault1_master').show();
//        } else {
//            console.log('ismodal <> 0 hide');
//            $('#updPrgPnlHeaderDefault1_master').hide();
//            ismodal = 1;
//        }
//        });
//    ismodal = 0;
    }
    $(document).ready(InIEvent);
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(InIEvent);
    </script>
</html>