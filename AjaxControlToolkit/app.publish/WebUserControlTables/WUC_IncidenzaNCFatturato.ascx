<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_IncidenzaNCFatturato.ascx.vb" Inherits="SoftAziOnLine.WUC_IncidenzaNCFatturato" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="550px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco clienti" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn2" runat="server" Elenco="ListaClienti" Titolo="Elenco clienti" />
    <br />
        <br />
        <br />
        <br />
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 380px; width: 927px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;" runat="server" GroupingText="Date">
                    <asp:Label ID="lblDallaData" runat="server" Height="16px" Width="172px">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" Width="100px" MaxLength="10" 
                                TabIndex="1"></asp:TextBox>
                            &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="2" />
                                                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                    TargetControlID="txtDataDa">
                                                </asp:CalendarExtender>
                                                &nbsp;
                                                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="txtDataDa" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                            &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>
                            <asp:TextBox ID="txtDataA" runat="server" Width="100px" MaxLength="10" AutoPostBack="true"
                                TabIndex="3"></asp:TextBox>
                            &nbsp;
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="4" />
                                                   
                                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                    TargetControlID="txtDataA">
                                                </asp:CalendarExtender>
                                                &nbsp;
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtDataA" ErrorMessage="*" 
                                                    
                                
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />&nbsp;&nbsp;<asp:Label 
                            ID="lblDataNC" runat="server" 
                            Height="20px" Width="98px">N.C. alla data</asp:Label>
                            <asp:TextBox ID="txtNCData" runat="server" Width="100px" MaxLength="10" 
                                TabIndex="5"></asp:TextBox>
                            &nbsp;
                            <asp:ImageButton ID="imgBtnShowCalendarNC" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="6" />
                                                   
                                                <asp:CalendarExtender ID="txtNCData_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarNC" 
                                                    TargetControlID="txtNCData">
                                                </asp:CalendarExtender>
                                                &nbsp;
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                                    ControlToValidate="txtNCData" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />&nbsp;</asp:Panel>
                     <asp:Panel ID="Panel" runat="server" groupingtext="Clienti" 
                            style="margin-top: 0px;" Width="859px">
                            <asp:Label ID="lblCliente1" runat="server" Width="165px" Height="17px">Dal codice</asp:Label>
                            <asp:Button ID="btnCliente1" runat="server" class="btnstyle" Width="25px" 
                                Height="25px" Text="?" ToolTip="Ricerca cliente" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodCliente1" runat="server"  
                                Width="138px" MaxLength="16" AutoPostBack="True" TabIndex="7" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDescCliente1" runat="server" 
                                Width="462px" MaxLength="50" TabIndex="8" Enabled="False"  ></asp:TextBox>
                        <br />
                        <asp:Label ID="lblCliente2" runat="server" Width="165px" Height="17px">Al codice</asp:Label>
                            <asp:Button ID="btnCliente2" runat="server" class="btnstyle" Width="25px" 
                                Height="25px" Text="?" ToolTip="Ricerca cliente" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodCliente2" runat="server"  
                                Width="138px" MaxLength="16" AutoPostBack="True" TabIndex="7" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDescCliente2" runat="server" 
                                Width="462px" MaxLength="50" TabIndex="8" Enabled="False"  ></asp:TextBox>
                        <br>
                        <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" 
                                TabIndex="9" Text="Seleziona tutti i clienti" TextAlign="Left" />&nbsp;
                        </asp:Panel>
                        <asp:Panel ID="PanelOrdinam" runat="server" groupingtext="Ordinamento" 
                            style="margin-top: 0px;" Width="859px">                            
                            <table align="center">
                            <tr>
                                <td><asp:RadioButton ID="rbtnCodice" runat="server" Text="Codice" Checked="true" GroupName="rbtnOrdinamento" /></td>
                                <td><asp:RadioButton ID="rbtnRagSoc" runat="server" Text="Ragione sociale" GroupName="rbtnOrdinamento" /></td>
                            </tr>
                            <tr>
                                <td><asp:RadioButton ID="rbtnNetto" runat="server" Text="Importo netto" GroupName="rbtnOrdinamento" /> </td>
                                <td><asp:RadioButton ID ="rbtnNettoDesc" runat="server" Text="Importo netto decrescente" GroupName="rbtnOrdinamento" /></td>
                            </tr>
                            <tr>
                                <td><asp:RadioButton ID ="rbtnNC" runat="server" Text="Importo note di credito" GroupName="rbtnOrdinamento" /></td>
                                <td><asp:RadioButton ID ="rbtnNCDesc" runat="server" Text="Importo note di credito decrescente" GroupName="rbtnOrdinamento" /></td>
                            </tr>
                            </table>
                        </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" 
                                         TabIndex="21" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="22" />
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