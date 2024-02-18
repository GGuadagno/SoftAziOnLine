<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ElDDTMagCaus.ascx.vb" Inherits="SoftAziOnLine.WUC_ElDDTMagCaus" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<style type="text/css">
    .styleTDBTN
    {
        height: 478px;
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
    .style2
    {
        width: 532px;
        vertical-align:top;
    }
    .style3
    {
        width: 164px;
    }
    </style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="550px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <div align="center" >    
        <br /><br /><br /><br />    
        <table style="vertical-align:middle; background-color:Silver; border-style:double; height:200px; width: 780px;">
            <tr>
                <td class="style2">
                    <br />
                    <asp:Panel ID="PanelMagazzino" style="margin-top: 0px;" runat="server" GroupingText="Seleziona:" Width="650px">
                        <asp:Label ID="lblMagazzino" runat="server" Width="90px" Height="16px">Magazzino</asp:Label>
                        <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                               AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                               DataTextField="Descrizione" 
                               DataValueField="Codice" Width="450px" TabIndex="2">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                               SelectCommand="SELECT Codice, Descrizione FROM Magazzini ORDER BY Descrizione">
                        </asp:SqlDataSource>
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="PanelSelezionaDate" runat="server" GroupingText="Date (Su tutti gli esercizi in base alle date inserite)" style="margin-top: 0px;" Width="650px" Height="60px">                        
                        <asp:Label ID="lblAllaData0" runat="server" Height="20px" Width="90px">Dalla data</asp:Label>
                        <asp:TextBox ID="txtDataDa" runat="server" MaxLength="10" TabIndex="1" Width="80px"></asp:TextBox>
                        <asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="2" ToolTip="apri il calendario" />
                        <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                            TargetControlID="txtDataDa"></asp:CalendarExtender>
                        <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                            ControlToValidate="txtDataDa" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        &nbsp;&nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>
                        &nbsp;<asp:TextBox ID="txtDataA" runat="server" MaxLength="10" TabIndex="3" Width="80px"></asp:TextBox>
                        <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="4" 
                            ToolTip="apri il calendario" />
                        <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                            TargetControlID="txtDataA"></asp:CalendarExtender>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                            runat="server" ControlToValidate="txtDataA" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        <br />
                        <asp:CheckBox ID="chkTuttiEser" Text="Estrazione da tutti gli esercizi" runat="server" Checked="false" AutoPostBack="true" />
                    </asp:Panel><br />
                    <asp:Panel ID="PanelSceltaCaus" style="margin-top: 0px;" runat="server" GroupingText="Seleziona:" Width="650px">
                        <asp:Label ID="Label1" runat="server" Height="16px">Causale DDT/Fatture accompagnatorie</asp:Label>
                        <asp:DropDownList ID="DDLCausale" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="false" DataSourceID="SqlDSCausale" DataTextField="Descrizione" 
                                DataValueField="Codice" Height="22px" Enabled="true" Width="350px">
                                <asp:ListItem Value="0" Text="[Tutte]"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDSCausale" runat="server" 
                                SelectCommand="SELECT * FROM [CausMag] ORDER BY [Descrizione]">
                            </asp:SqlDataSource>
                    </asp:Panel>
                    <br />
                </td>
                <td align="left" class="style3">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div id="noradio">
                                <asp:Button ID="btnStampa" runat="server" class="btnstyle" TabIndex="20" 
                                    Text="Stampa" />
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div>
                                <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" TabIndex="21" 
                                    Text="Annulla" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            </caption>
            </tr>
        </table>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>