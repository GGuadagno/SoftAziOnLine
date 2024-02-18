<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_OrdinatoPerArticoloAG.ascx.vb" Inherits="SoftAziOnLine.WUC_OrdinatoPerArticoloAG" %>
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
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 320px; width: 935px;">
                </caption>
                <tr>
                    <td>
                        <asp:Panel ID="PanelSelezionaAgente" runat="server" 
                            GroupingText="Selezione agente" style="margin-top: 0px;">
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        Singolo agente</td>
                                    <td>
                                        <asp:DropDownList ID="ddlAgenti" runat="server" AppendDataBoundItems="true" 
                                            DataSourceID="SqlDa_Agenti" DataTextField="Descrizione" DataValueField="Codice" 
                                            Enabled="False" Width="400px">
                                            <asp:ListItem Text="Agente non definito" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:SqlDataSource ID="SqlDa_Agenti" runat="server" 
                                            SelectCommand="SELECT [Codice], [Descrizione] FROM [Agenti]">
                                        </asp:SqlDataSource>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkTuttiAgenti" runat="server" AutoPostBack="true" 
                                            Checked="True" Text="Seleziona tutti gli agenti" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel ID="PanelSelezionaOrdinamento" runat="server" groupingtext="Ordinamento" style="margin-top: 0px;">
                            <div>
                                <table width="100%">
                                    <tr>
                                        <td width="25%">
                                        </td>
                                        <td width="25%">
                                            <asp:RadioButton ID="rbtnCodice" runat="server" AutoPostBack="false" 
                                                GroupName="Tipo" Text="Codice" />
                                        </td>
                                        <td width="25%">
                                            <asp:RadioButton ID="rbtnDescrizione" runat="server" AutoPostBack="false" 
                                                GroupName="Tipo" TabIndex="1" Text="Descrizione" />
                                        </td>
                                        <td width="25%">
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione" Height="139px" style="margin-top: 0px;" Width="859px">
                            <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Width="90px">Dal codice</asp:Label>
                            <asp:Button ID="btnCod1" runat="server" class="btnstyle" Height="25px" 
                                TabIndex="3" Text="?" ToolTip="Ricerca articoli" Width="25px" />
                            <asp:TextBox ID="txtCod1" runat="server" AutoPostBack="True" MaxLength="20" 
                                Width="100px"></asp:TextBox>
                            <asp:TextBox ID="txtDesc1" runat="server" MaxLength="150" TabIndex="4" 
                                Width="400px"></asp:TextBox>
                            <br>
                            </br>
                            <asp:Label ID="lblAl" runat="server" Height="16px" TabIndex="5" Width="90px">Al codice</asp:Label>
                            <asp:Button ID="btnCod2" runat="server" class="btnstyle" Height="25px" 
                                TabIndex="6" Text="?" ToolTip="Ricerca articoli" Width="25px" />
                            <asp:TextBox ID="txtCod2" runat="server" AutoPostBack="True" MaxLength="20" 
                                TabIndex="7" Width="100px"></asp:TextBox>
                            <asp:TextBox ID="txtDesc2" runat="server" MaxLength="150" TabIndex="8" 
                                Width="400px"></asp:TextBox>
                            <br>
                            </br>
                            <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" 
                                TabIndex="10" Text="Seleziona tutti gli articoli" TextAlign="Left" />
                            <br />
                            <br />
                            <asp:Label ID="Label4" runat="server" Height="16px" TabIndex="11" Width="100px">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" MaxLength="10" TabIndex="12" 
                                Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" 
                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="14" 
                                ToolTip="apri il calendario" />
                            <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                TargetControlID="txtDataDa">
                            </asp:CalendarExtender>
                            <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                ControlToValidate="txtDataDa" ErrorMessage="*" 
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                            <asp:Label ID="Label5" runat="server" Height="20px" TabIndex="15" Width="66px">Alla data</asp:Label>
                            <asp:TextBox ID="txtDataA" runat="server" MaxLength="10" TabIndex="16" 
                                Width="80px"></asp:TextBox>
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="17" 
                                ToolTip="apri il calendario" />
                            <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                TargetControlID="txtDataA">
                            </asp:CalendarExtender>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                ControlToValidate="txtDataA" ErrorMessage="*" TabIndex="18" 
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                            &nbsp;
                            <asp:Label ID="Label6" runat="server" Height="16px">(prevista consegna, non obbligatorie)</asp:Label>
                            <br />
                        </asp:Panel>
                    </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" TabIndex="20" Text="Stampa" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" TabIndex="21" Text="Annulla" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    </caption>
                </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>     
</asp:Panel>