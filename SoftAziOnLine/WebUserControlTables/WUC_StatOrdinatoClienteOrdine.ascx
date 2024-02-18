<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StatOrdinatoClienteOrdine.ascx.vb" Inherits="SoftAziOnLine.WUC_StatOrdinatoClienteOrdine" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
<style type="text/css">
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
       vertical-align: middle;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
        <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
            SelectCommand="SELECT * FROM [Regioni] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSProvince" runat="server" 
            SelectCommand="SELECT Codice, Descrizione FROM [Province] WHERE [Regione] = CASE WHEN @CodRegione = 0 THEN Regione ELSE @CodRegione END ORDER BY [Descrizione]">
            <SelectParameters>
                <asp:SessionParameter Name="CodRegione" DbType="Int32" Direction="Input" SessionField="CodRegione" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSAgenti" runat="server" 
            SelectCommand="SELECT [Codice], [Descrizione] FROM [Agenti] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
    <br>
    <br> 
    <br>
    <br>
    <br>   
<table style="vertical-align:middle; background-color:Silver; border-style:double; height:350px; width: 927px;">
    <tr>
        <td style="vertical-align: top;">
            <asp:Panel ID="Panel1" runat="server" groupingtext="Selezione" style="margin-top: 0px;" Height="330px" Width="859px">
            <div>
                 <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Width="165px">Codice cliente</asp:Label>
                 <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Height="25px" TabIndex="3" Text="?" Visible="true" Width="25px" />&nbsp;&nbsp;
                 <asp:TextBox ID="txtCodCli1" runat="server" MaxLength="20" Width="130px" AutoPostBack="True"></asp:TextBox>&nbsp;&nbsp;
                 <asp:TextBox ID="txtDesCli1" runat="server" MaxLength="150" TabIndex="4" Width="400px" Enabled="false"></asp:TextBox>
            </div>
            <br />
            <div>
                <asp:Label ID="Label1" runat="server" Width="165px" Height="17px" TabIndex="2">Dal codice articolo</asp:Label>
                <asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="3" Text="?" ToolTip="Ricerca articoli" />&nbsp;&nbsp;
                <asp:TextBox ID="txtCod1" runat="server" Width="130px" MaxLength="20" AutoPostBack="True" ></asp:TextBox>&nbsp;&nbsp;
                <asp:TextBox ID="txtDesc1" runat="server" Width="400px" MaxLength="150" TabIndex="4"  ></asp:TextBox>
            </div>
            <div>
                <asp:Label ID="Label2" runat="server" Width="165px" Height="16px" TabIndex="5">Al codice articolo</asp:Label>
                <asp:Button ID="btnCod2" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="6" Text="?" ToolTip="Ricerca articoli" />&nbsp;&nbsp;
                <asp:TextBox ID="txtCod2" runat="server" Width="130px" MaxLength="20" TabIndex="7" AutoPostBack="True" ></asp:TextBox>&nbsp;&nbsp;
                <asp:TextBox ID="txtDesc2" runat="server" Width="400px" MaxLength="150" TabIndex="8"></asp:TextBox>
            </div>
            <div style="text-align:center">
                <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" TabIndex="10" Text="Seleziona tutti gli articoli" TextAlign="Left" />
            </div>
            <br />
            <div>
                <asp:Label ID="Label4" runat="server" Height="16px" Width="170px" TabIndex="11">Dalla data</asp:Label>
                <asp:TextBox ID="txtDataDa" runat="server" Width="100px" MaxLength="10" TabIndex="12"></asp:TextBox>&nbsp;
                <asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="14" />
                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" TargetControlID="txtDataDa"></asp:CalendarExtender>&nbsp;
                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" ControlToValidate="txtDataDa" ErrorMessage="*" ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />&nbsp;
                <asp:Label ID="Label5" runat="server" Height="20px" Width="66px" TabIndex="15">Alla data</asp:Label>
                <asp:TextBox ID="txtDataA" runat="server" Width="100px" MaxLength="10" TabIndex="16"></asp:TextBox>&nbsp;
                <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="17" />
                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" TargetControlID="txtDataA"></asp:CalendarExtender>&nbsp;
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtDataA" ErrorMessage="*" ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" TabIndex="18" />&nbsp;
                <asp:Label ID="Label6" runat="server" Height="16px" Width="147px" TabIndex="19">(non obbligatorie)</asp:Label>
            </div>
            <br />
            <div>
                <asp:Label ID="Label3" runat="server" Height="16px" Width="170px" TabIndex="2">Regione:</asp:Label>
                <asp:DropDownList ID="ddlRegioni" runat="server" AutoPostBack="true" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" DataValueField="Codice" Width="250px"
                    AppendDataBoundItems="true" Enabled="False">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                </asp:DropDownList>
                <asp:CheckBox ID="chkTutteRegioni" Text="Seleziona tutte le regioni" runat="server" AutoPostBack="True" />
            </div>
            <div>
                <asp:Label ID="Label7" runat="server" Height="16px" Width="170px" TabIndex="2">Provincia:</asp:Label>
                <asp:DropDownList ID="ddlProvince" runat="server" AutoPostBack="false" DataSourceID="SqlDSProvince" DataTextField="Descrizione" DataValueField="Codice" Width="250px"
                    AppendDataBoundItems="true" Enabled="False">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                </asp:DropDownList>
                <asp:CheckBox ID="chkTutteProvince" Text="Seleziona tutte le province" runat="server" AutoPostBack="True" />
             </div>
             <br />
             <div>
                <asp:Label ID="Label8" runat="server" Height="16px" Width="170px" TabIndex="2">Agente:</asp:Label>
                <asp:DropDownList ID="ddlAgenti" runat="server" AutoPostBack="false" DataSourceID="SqlDSAgenti" DataTextField="Descrizione" DataValueField="Codice" Width="250px"
                    AppendDataBoundItems="true" Enabled="False">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                </asp:DropDownList>
                <asp:CheckBox ID="chkTuttiAgenti" Text="Seleziona tutti gli agenti" runat="server" AutoPostBack="True" />
             </div>
             <br />
             <div style="text-align:center">
                <asp:Label ID="Label9" runat="server" Height="16px" ForeColor="Blue">Verranno selezionati tutti gli ordini che presentano causale VENDITA</asp:Label>
             </div>
            </asp:Panel>    
        </td>
        <td align="left" class="style1">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div>
                        <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" TabIndex="20" />
                    </div>
                    <div style="height: 15px">
                    </div>
                    <div>
                        <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" TabIndex="21" /><br />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
</table>
</ContentTemplate>
 </asp:UpdatePanel>    
</asp:Panel>