<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_OrdForArt.ascx.vb" Inherits="SoftAziOnLine.WUC_OrdForArt" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
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
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaFornitori" Titolo="Elenco fornitori" />
        <br />
        <br />
        <br />
        <br />
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 260px; width: 927px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;" runat="server" 
                        GroupingText="Date ordine">
                    <asp:Label ID="lblDallaData" runat="server" Height="16px" Width="172px">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" Width="100px" MaxLength="10" 
                                TabIndex="1"></asp:TextBox>
                            &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="2" />
                                                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                    TargetControlID="txtDataDa"></asp:CalendarExtender>
                                                &nbsp;
                                                <%--<asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="txtDataDa" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                            &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label><asp:TextBox ID="txtDataA" runat="server" Width="100px" MaxLength="10" 
                                TabIndex="3"></asp:TextBox>&nbsp;
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="4" />
                                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                    TargetControlID="txtDataA"></asp:CalendarExtender>
                                                &nbsp;
                                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtDataA" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                        &nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="chkEscludiChiuso" runat="server" AutoPostBack="false" 
                                TabIndex="8" Text="Escludi Ordini Chiuso Non Evaso" TextAlign="Left" Checked="true" />
                        </asp:Panel>
                     <asp:Panel ID="Panel" runat="server" groupingtext="Fornitori" 
                            style="margin-top: 0px;" Height="78px" Width="859px">
                            <asp:Label ID="lblFornitore" runat="server" Width="165px" Height="17px">Fornitore</asp:Label>
                            <asp:Button ID="btnFornitore" runat="server" class="btnstyle" Width="25px" 
                                Height="25px" Text="?" ToolTip="Ricerca fornitore" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodFornitore" runat="server"  
                                Width="138px" MaxLength="16" AutoPostBack="True" TabIndex="6" ></asp:TextBox>&nbsp;&nbsp;<asp:TextBox ID="txtDescFornitore" runat="server" 
                                Width="400px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox><br>
                        <asp:CheckBox ID="chkTuttiFornitori" runat="server" AutoPostBack="True" 
                                TabIndex="8" Text="Seleziona tutti i fornitori" TextAlign="Left" />
                            <br>
                        </asp:Panel>
                    <asp:Panel ID="PanelArticolo" runat="server" groupingtext="Articoli" 
                            style="margin-top: 0px;" Height="105px" Width="859px">
                            <asp:Label ID="lblDal" runat="server" Width="165px" Height="17px">Codice articolo</asp:Label>
                            <asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="25px" 
                                Height="25px" Text="?" ToolTip="Ricerca articolo" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server"  
                                Width="138px" MaxLength="20" AutoPostBack="True" TabIndex="9" ></asp:TextBox>&nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" 
                                Width="400px" MaxLength="150" TabIndex="10" Enabled="False"  ></asp:TextBox><br>
                            <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" 
                                TabIndex="10" Text="Seleziona tutti gli articoli" TextAlign="Left" />
                        </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div>
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" 
                                         TabIndex="20" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="21" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    </tr>
                </caption>
                </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>