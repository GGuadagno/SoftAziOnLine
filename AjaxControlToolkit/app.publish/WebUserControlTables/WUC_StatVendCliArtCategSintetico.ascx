<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StatVendCliArtCategSintetico.ascx.vb" Inherits="SoftAziOnLine.WUC_StatVendCliArtCategSintetico" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="558px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco clienti" />
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 500px; width: 927px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelSelezionaAgente" style="margin-top: 0px;" runat="server" GroupingText="Categorie clienti">
                    <table width="100%">
                        <tr>
                            <td align="left">Singola categoria</td><td>
                            <asp:DropDownList ID="ddlCatCli" runat="server" DataSourceID="SqlDa_CatCli" 
                                DataTextField="Descrizione" DataValueField="Codice" Width="400px" 
                                AppendDataBoundItems="true" Enabled="False">
                                <asp:ListItem Value="0" Text="Categoria non definita"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDa_CatCli" runat="server" 
                                SelectCommand="SELECT [Codice], [Descrizione] FROM [Categorie]">
                            </asp:SqlDataSource>
                            </td>
                            <td>
                            <asp:CheckBox ID="chkTutteCatCli" runat="server" Text="Seleziona tutte le categorie" AutoPostBack="true" Checked="True" />
                            </td>
                        </tr>
                        <tr>
                            <td align="left"></td>
                            <td>
                            <asp:CheckBox ID="chkRaggrCatCli" runat="server" Text="Seleziona tutte le categorie per descrizione iniziale uguale" AutoPostBack="true" Checked="False" Enabled="false" />
                            </td>
                            <td>
                            <asp:CheckBox ID="chkAccorpaCC" runat="server" Text="Accorpa le categorie" AutoPostBack="False" Checked="False" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;" runat="server" GroupingText="Date (Su tutti gli esercizi in base alle date inserite)">
                    <asp:Label ID="lblDallaData" runat="server" Height="16px" Width="100px">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" Width="80px" MaxLength="10" TabIndex="1"></asp:TextBox>
                            &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="2" />
                                                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                    TargetControlID="txtDataDa"></asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="txtDataDa" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                            &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>
                            <asp:TextBox ID="txtDataA" runat="server" Width="80px" MaxLength="10" AutoPostBack="true" TabIndex="3"></asp:TextBox>
                            &nbsp;
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="4" />
                                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                    TargetControlID="txtDataA"></asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtDataA" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                            &nbsp;&nbsp;<asp:Label ID="lblDataNC" runat="server" Height="20px" >N.C. alla data</asp:Label>
                            <asp:TextBox ID="txtNCData" runat="server" Width="80px" MaxLength="10" TabIndex="5"></asp:TextBox>
                            &nbsp;
                            <asp:ImageButton ID="imgBtnShowCalendarNC" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="6" />
                                                <asp:CalendarExtender ID="txtNCData_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarNC" 
                                                    TargetControlID="txtNCData"></asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                                    ControlToValidate="txtNCData" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        </asp:Panel>
                        <asp:Panel ID="Panel" runat="server" groupingtext="Clienti" style="margin-top: 0px;" Height="78px" Width="859px">
                            <asp:Label ID="lblCliente" runat="server" Width="100px" Height="17px">Cliente</asp:Label>
                            <asp:Button ID="btnCliente" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca cliente" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodCliente" runat="server" Width="100px" MaxLength="16" AutoPostBack="True" TabIndex="7" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDescCliente" runat="server" Width="400px" MaxLength="50" TabIndex="8" Enabled="False"  ></asp:TextBox>
                        <br>
                        <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" TabIndex="9" Text="Seleziona tutti i clienti" TextAlign="Left" />
                            <br>
                        </asp:Panel>
                    <asp:Panel ID="PanelArticolo" runat="server" groupingtext="Articoli" style="margin-top: 0px;" Height="105px" Width="859px">
                            <asp:Label ID="lblDal" runat="server" Width="100px" Height="17px">Dal codice</asp:Label>
                            <asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca articoli" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodArt1" runat="server"  Width="100px" MaxLength="20" AutoPostBack="True" TabIndex="10" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" Width="400px" MaxLength="150" TabIndex="11" Enabled="False"  ></asp:TextBox>
                            <br>
                            <asp:Label ID="lblAl" runat="server" Width="100px" Height="16px">Al codice</asp:Label>
                            <asp:Button ID="btnCod2" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca articoli" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodArt2" runat="server" Width="100px" MaxLength="20" TabIndex="12" AutoPostBack="True" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesc2" runat="server" Width="400px" MaxLength="150" TabIndex="13" Enabled="False"  ></asp:TextBox>
                        <br>
                        <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" TabIndex="10" Text="Seleziona tutti gli articoli" TextAlign="Left" /> 
                        </asp:Panel>
                        <asp:Panel ID="PanelTipoStatistica" style="margin-top: 0px;" runat="server" groupingtext="Statistica">
                        <table width="100%">
                        <tr>
                            <td width="33%">
                            <asp:RadioButton ID="rbtnVenduto" runat="server" Text="Venduto" AutoPostBack="True" GroupName="TipoStatistica" TabIndex="14" />
                            </td>
                            <td width ="33%">
                            <asp:RadioButton ID="rbtnFatturato" runat="server" Text="Fatturato" AutoPostBack="True" GroupName="TipoStatistica" TabIndex="15" />
                            </td>
                            <td width="33%">
                            <asp:RadioButton ID="rbtnDaFatturare" runat="server" Text="Da Fatturare" AutoPostBack="True" GroupName="TipoStatistica" TabIndex="16" />
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                        <asp:Panel ID="PanelSelezionaOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Ordinamento" Visible="false" Enabled="false">
                        <table width="100%">
                        <tr>
                            <td width="25%">
                            </td>
                            <td width ="25%">
                            <asp:RadioButton ID="rbtnCliente" runat="server" Text="Cliente / Articolo" AutoPostBack="True" GroupName="Ordinamento" TabIndex="17" />
                            </td>
                            <td width="25%">
                            <asp:RadioButton ID="rbtnArticolo" runat="server" Text="Articolo / Cliente" AutoPostBack="True" GroupName="Ordinamento" TabIndex="18" />
                            </td>
                            <td width="25%">
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelVisualizzazione" style="margin-top: 0px;" runat="server" groupingtext="Visualizzazione" Visible="false" Enabled="false">
                        <table width="100%">
                        <tr>
                            <td width="25%">
                            </td>
                            <td width ="25%">
                            <asp:RadioButton ID="rbtnPrezzoVendita" runat="server" Text="Prezzo di vendita" AutoPostBack="True" GroupName="Visualizzazione" TabIndex="19" />
                            </td>
                            <td width="25%">
                            <asp:RadioButton ID="rbtnPrezzoListino" runat="server" Text="Prezzo di listino" AutoPostBack="True" GroupName="Visualizzazione" TabIndex="20" />
                            </td>
                            <td width="25%">
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