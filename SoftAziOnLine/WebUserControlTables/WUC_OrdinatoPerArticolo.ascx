<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_OrdinatoPerArticolo.ascx.vb" Inherits="SoftAziOnLine.WUC_OrdinatoPerArticolo" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
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
    .style7
    {
        height: 185px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
    <br>
    <br>
    <br>
    <br>
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 260px; width: 927px;">
                <td>
                    <asp:Panel ID="PanelSelezionaOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Ordinamento">
                    <div>
                        <table width="100%">
                        <tr>
                            <td width="25%">
                            </td>
                            <td width ="25%">
                            <asp:RadioButton ID="rbtnCodice" runat="server" Text="Codice" AutoPostBack="True" 
                            GroupName="Tipo" />
                            <!--<asp:Label ID="Label0" runat="server" Width="60px">&nbsp;</asp:Label>-->
                            </td>
                            <td width="25%">
                            <asp:RadioButton ID="rbtnDescrizione" runat="server" Text="Descrizione" 
                            AutoPostBack="True" GroupName="Tipo" TabIndex="1" />
                            </td>
                            <td width="25%">
                            </td>
                        </tr>
                        </table>
                    </div>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione" 
                            style="margin-top: 0px;" Height="136px" Width="859px">
                            <asp:Label ID="lblDal" runat="server" Width="165px" Height="17px" TabIndex="2">Dal codice</asp:Label>
                            <asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="3" Text="?" ToolTip="Ricerca articoli" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server"  Width="130px" MaxLength="20" AutoPostBack="True" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" Width="400px" MaxLength="150" TabIndex="4"  ></asp:TextBox>
                            <br>
                            <asp:Label ID="lblAl" runat="server" Width="165px" Height="16px" TabIndex="5">Al codice</asp:Label>
                            <asp:Button ID="btnCod2" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="6" Text="?" ToolTip="Ricerca articoli" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod2" runat="server" Width="130px" MaxLength="20" TabIndex="7" AutoPostBack="True" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesc2" runat="server" Width="400px" MaxLength="150" TabIndex="8"  ></asp:TextBox>
                        <br>
                        <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" TabIndex="10" Text="Seleziona tutti gli articoli" TextAlign="Left" />
                            <br>
                            <asp:Label ID="Label4" runat="server" Height="16px" Width="165px" TabIndex="11">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" Width="138px" MaxLength="10" TabIndex="12"></asp:TextBox>
                            &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="14" CausesValidation="False" />
                                                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                    TargetControlID="txtDataDa"></asp:CalendarExtender>
                                                &nbsp;
                                                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="txtDataDa" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                            &nbsp;<asp:Label ID="Label5" runat="server" Height="20px" Width="66px" 
                                TabIndex="15">Alla data</asp:Label>
                            <asp:TextBox ID="txtDataA" runat="server" Width="138px" MaxLength="10" 
                                TabIndex="16"></asp:TextBox>
                            &nbsp;
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="17" CausesValidation="False" />
                                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                    TargetControlID="txtDataA"></asp:CalendarExtender>
                                                &nbsp;
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtDataA" ErrorMessage="*" 
                                                    
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" 
                                TabIndex="18" />&nbsp;
                            <asp:Label ID="Label6" runat="server" Height="16px" Width="147px" TabIndex="19">(prevista consegna,<br> non obbligatorie)</asp:Label>
                        </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" CausesValidation="False" TabIndex="20" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" TabIndex="21" CausesValidation="False" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </caption>
</table>
</ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>