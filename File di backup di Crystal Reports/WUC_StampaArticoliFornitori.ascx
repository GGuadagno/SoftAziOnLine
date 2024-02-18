<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StampaArticoliFornitori.ascx.vb" Inherits="SoftAziOnLine.WUC_StampaArticoliFornitori" %>
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
    .btnstyleDoppio
        {
            Width: 108px;
            height: 48px;
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
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaFornitori" Titolo="Elenco fornitori" />
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 260px; width: 927px;" >
            <tr>
                <td>
                     <asp:Panel ID="Panel" runat="server" groupingtext="Seleziona dati" style="margin-top: 0px;" Height="78px" Width="859px">
                            <asp:Label ID="lblFornitore" runat="server" Width="100px" Height="17px">Fornitore</asp:Label>
                            <asp:Button ID="btnFornitore" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca fornitore" />
                            <asp:TextBox ID="txtCodFornitore" runat="server"  Width="100px" MaxLength="16" AutoPostBack="True" TabIndex="6" ></asp:TextBox>
                            <asp:TextBox ID="txtDescFornitore" runat="server" Width="400px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox>
                        <br>
                            <asp:CheckBox ID="chkTuttiFornitori" runat="server" AutoPostBack="True" TabIndex="8" Text="Seleziona tutti i fornitori" TextAlign="Left" />
                        <br>
                            <asp:Label ID="lblConfronta" runat="server" Height="17px">Confronta prezzi alla data</asp:Label>
                                &nbsp;<asp:TextBox ID="txtData" runat="server" MaxLength="10" TabIndex="1" 
                                    Width="70px"></asp:TextBox><asp:ImageButton ID="imgBtnShowCalendar" runat="server" 
                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                    ToolTip="apri il calendario" />
                                <asp:CalendarExtender ID="txtData_CalendarExtender" runat="server" 
                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar" 
                                    TargetControlID="txtData">
                                </asp:CalendarExtender>
                                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                    ControlToValidate="txtData" ErrorMessage="*" 
                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        <br>                
                        </asp:Panel>
                        <br><br>
                        <asp:Panel ID="PanelTipoOrdine" style="margin-top: 0px;" runat="server" groupingtext="Ordine stampa">
                        <table width="100%">
                        <tr>
                            <td width="50%">
                            <asp:RadioButton ID="rbtnCodice" runat="server" Text="Codice" AutoPostBack="True" GroupName="TipoOrdine" TabIndex="14" Checked="true" />
                            </td>
                            <td width ="50%">
                            <asp:RadioButton ID="rbtnDescrizione" runat="server" Text="Descrizione" AutoPostBack="True" GroupName="TipoOrdine" TabIndex="15" />
                            </td>
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