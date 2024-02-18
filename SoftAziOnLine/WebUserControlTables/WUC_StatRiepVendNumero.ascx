<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StatRiepVendNumero.ascx.vb" Inherits="SoftAziOnLine.WUC_StatRiepVendNumero" %>
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
</style>
<br />
<br />
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="500px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <br />
        <br />
        <br />
        <br />
        <br />
        <div align="center" >        
        <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 250px; width: 708px;">
            <tr>
                <td class="style8">
                    <asp:Panel ID="PanelSelezionaDate" runat="server" GroupingText="Date DDT" 
                        style="margin-top: 0px;" Width="572px" heigth="66px">                        
                        
                        &nbsp;&nbsp;<asp:Label ID="lblAllaData0" runat="server" Height="20px" Width="90px">Dalla 
                        data</asp:Label>
                        <asp:TextBox ID="txtDataDa" runat="server" MaxLength="10" TabIndex="1" 
                            Width="70px"></asp:TextBox>
                        &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" 
                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="2" 
                            ToolTip="apri il calendario" />
                        <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                            TargetControlID="txtDataDa"></asp:CalendarExtender>
                        &nbsp;
                        <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                            ControlToValidate="txtDataDa" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        &nbsp;&nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla 
                        data</asp:Label>
                        &nbsp;<asp:TextBox ID="txtDataA" runat="server" MaxLength="10" TabIndex="3" 
                            Width="70px"></asp:TextBox>
                        &nbsp;&nbsp;<asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="4" 
                            ToolTip="apri il calendario" />
                        <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                            TargetControlID="txtDataA"></asp:CalendarExtender>
                        &nbsp;&nbsp;<asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                            runat="server" ControlToValidate="txtDataA" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        &nbsp;<br />
                        <br />
                    </asp:Panel>
                        
                    <br />
                        
                    <asp:Panel ID="Panel" runat="server" groupingtext="Selezione numero DDT" 
                        Height="77px" style="margin-top: 0px;" Width="572px">
                        &nbsp;&nbsp;<asp:Label ID="lblCliente" runat="server" Height="17px" Width="90px">Dal 
                        numero</asp:Label>
                        &nbsp;<asp:TextBox ID="txtDaDT" runat="server" AutoPostBack="False" MaxLength="10" 
                            TabIndex="6" Width="70px"></asp:TextBox>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Label ID="lblCliente0" runat="server" Height="17px" Width="78px">Al numero</asp:Label>
                        <asp:TextBox ID="txtADT" runat="server" MaxLength="10" 
                            TabIndex="7" Width="70px" EnableTheming="True"></asp:TextBox>
                        
                        <br />
                        
                        <br />
                      
                    </asp:Panel>
                    <asp:Panel ID="PanelTipoStatistica" style="margin-top: 0px;" runat="server" groupingtext="Statistica" Width="572px">
                        <table width="100%">
                        <tr>
                            <td width="33%">
                            <asp:RadioButton ID="rbtnVenduto" runat="server" Text="Venduto" 
                            AutoPostBack="false" GroupName="TipoStatistica" TabIndex="14" />
                            </td>
                            <td width ="33%">
                            <asp:RadioButton ID="rbtnFatturato" runat="server" Text="Fatturato" AutoPostBack="false" 
                            GroupName="TipoStatistica" TabIndex="15" />
                            </td>
                            <td width="34%">
                            <asp:RadioButton ID="rbtnDaFatturare" runat="server" Text="Da Fatturare" 
                            AutoPostBack="false" GroupName="TipoStatistica" TabIndex="16" />
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <br />
                    <asp:Label ID="Label1" runat="server" ForeColor="Blue" Font-Bold="true" Text="(NOTA sono escluse le Fatture accompagnatorie)"></asp:Label>
                </td>
                <td align="left" class="style7">
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