<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StatMovMag.ascx.vb" Inherits="SoftAziOnLine.WUC_StatMovMag" %>
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
    <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco clienti" />
    <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
            SelectCommand="SELECT -1 AS Codice, '' AS Descrizione UNION ALL SELECT * FROM [Regioni] ORDER BY [Descrizione]">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDSProvince" runat="server" 
            SelectCommand="SELECT '' AS Codice, 'Tutte le province' AS Descrizione UNION ALL SELECT [Codice], [Descrizione] FROM [Province] WHERE [Regione] = @Regione ORDER BY [Codice]">
            <SelectParameters>
                <asp:SessionParameter Name="Regione" DbType="Int32" Direction="Input" SessionField="CodRegione" />
            </SelectParameters>
    </asp:SqlDataSource>
        <div align="center" >        
        <table style="vertical-align:middle; background-color:Silver; border-style:double; height:540px; width: 780px;">
            <tr>
                <td class="style2">
                    <asp:Panel ID="PanelMagazzino" style="margin-top: 0px;" runat="server" GroupingText="Magazzino">
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
                    <asp:Panel ID="PanelSelezionaDate" runat="server" GroupingText="Selezione date" style="margin-top: 0px;" Width="650px" Height="60px">                        
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
                    <asp:Panel ID="PanelClienti" runat="server" groupingtext="Clienti" style="margin-top: 0px;" Height="80px" Width="650px">
                            <asp:Label ID="lblCliente" runat="server" Height="17px">Cliente</asp:Label>
                            <asp:Button ID="btnCliente" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca cliente" Enabled="False"/>
                            <asp:TextBox ID="txtCodCliente" runat="server"  Width="100px" MaxLength="16" AutoPostBack="True" Enabled="False"></asp:TextBox>
                            <asp:TextBox ID="txtDescCliente" runat="server" Width="350px" MaxLength="50" TabIndex="8" Enabled="False"></asp:TextBox>
                        <br>
                             <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" TabIndex="9" Text="Seleziona tutti i clienti" TextAlign="Left" Checked="true"/>
                    </asp:Panel>    
                    <asp:Panel ID="PanelArticolo" runat="server" groupingtext="Articoli" Height="150px" style="margin-top: 0px;" Width="650px">
                        <asp:Label ID="lblDal" runat="server" Height="17px" Width="100px">Dal codice</asp:Label>
                        <asp:Button ID="btnCod1" runat="server" Height="25px" Width="25px" Text="?" ToolTip="Ricerca articolo" />
                        <asp:TextBox ID="txtCod1" runat="server" AutoPostBack="True" MaxLength="20" TabIndex="9" Width="100px"></asp:TextBox>
                        <asp:TextBox ID="txtDesc1" runat="server" Enabled="False" MaxLength="150" TabIndex="10" Width="300px"></asp:TextBox>
                        <br>
                        <asp:Label ID="lblAl" runat="server" Height="16px" Width="100px">Al codice</asp:Label>
                        <asp:Button ID="btnCod2" runat="server" Height="25px" Width="25px" Text="?" ToolTip="Ricerca articolo" />
                        <asp:TextBox ID="txtCod2" runat="server" AutoPostBack="True" MaxLength="20" TabIndex="11" Width="100px"></asp:TextBox>
                        <asp:TextBox ID="txtDesc2" runat="server" Enabled="False" MaxLength="150" TabIndex="12" Width="300px"></asp:TextBox>
                        <br>
                        <%--<asp:Label ID="lblTuttiArticoli" runat="server" Height="16px" Width="150px">Seleziona 
                        </asp:Label>--%>
                        <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" TabIndex="10" Text="Seleziona tutti gli articoli" />
                        <asp:Label ID="Label3" runat="server" Height="16px" Width="50px" Text=""></asp:Label>
                        <%--<asp:Label ID="Label2" runat="server" Height="16px" Width="240px" Text="Stampa anche i Lotti/N° Serie collegati"></asp:Label>--%>
                        <asp:CheckBox ID="chkStampaLotti" runat="server" AutoPostBack="false" TabIndex="10" Text="Stampa anche i Lotti/N° Serie collegati" />
                        <br />
                        <asp:CheckBox ID="chkStampaDocRefInt" runat="server" AutoPostBack="false" TabIndex="10" Text="Stampa anche il documento collegato (Ordine<>DDT)" />
                        <br />
                        <asp:Label ID="Label1" runat="server" Text="Seleziona: "></asp:Label>
                        <asp:RadioButton ID="rbSoloCarichi" runat="server" Text="Carichi" GroupName="segno"/>
                        <asp:RadioButton ID="rbScarichi" runat="server" Text="Scarichi" GroupName="segno"/>
                        <asp:RadioButton ID="rbDDT" runat="server" Text="Documento di trasporto"  GroupName="segno"/>
                        <asp:RadioButton ID="rbTutto" runat="server" GroupName="segno" Text="Tutto" Checked="True" />
                        <br />
                    </asp:Panel>
                    <asp:Panel ID="PanelNSerieLotto" runat="server" GroupingText="Lotti/N° Serie" Height="68px" style="margin-top: 0px;" Width="650px">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <asp:CheckBox ID="chkFindLottiSerie" runat="server" AutoPostBack="True" TabIndex="10" Text="Ricerca Lotto/N° Serie (parole contenute)" />
                                </td>
                            </tr>
                            <tr>
                                <td>Lotto&nbsp;</td><td>
                                <asp:TextBox ID="txtLotto" runat="server" Enabled="False" MaxLength="30"></asp:TextBox></td>
                                <td>N° serie&nbsp;</td><td>
                                <asp:TextBox ID="txtNSerie" runat="server" Enabled="False" MaxLength="30"></asp:TextBox></td>
                            </tr>
                        </table>
                    </asp:Panel><br />
                    <asp:Panel ID="PanelRegione" runat="server" GroupingText="Regioni/Province" Height="68px" style="margin-top: 0px;" Width="650px">
                        <table width="100%">
                            <tr valign="top">
                                <td style="width:50%;">
                                    <asp:Label ID="Label2" runat="server" Width="60px" Height="16px">Regione</asp:Label>
                                    <asp:DropDownList ID="ddlRegioni" runat="server" 
                                    AutoPostBack="true" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" 
                                    DataValueField="Codice" Width="200px" Enabled="false">
                                    <asp:ListItem Text="" Value="" ></asp:ListItem>
                                    </asp:DropDownList><br />
                                    <asp:CheckBox ID="chkTutteRegioni" Text="Seleziona tutte le regioni" runat="server" AutoPostBack="True" Checked="true"/>
                                </td>
                                <td>
                                    <asp:Label ID="Label4" runat="server" Width="70px" Height="16px">Provincia</asp:Label>
                                    <asp:DropDownList ID="ddlProvince" runat="server" 
                                    AutoPostBack="false" DataSourceID="SqlDSProvince" DataTextField="Descrizione" 
                                    DataValueField="Codice" Width="200px" Enabled="false" AppendDataBoundItems="false">
                                    <asp:ListItem Text="" Value="" ></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                       </table>
                    </asp:Panel><br />
                    <asp:Panel ID="PanelLead" style="margin-top: 0px;" runat="server" GroupingText="Seleziona:">
                    <table width="100%">
                        <tr>
                            <td align="left">Lead Source</td><td>
                            <asp:DropDownList ID="DDLLead" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="false" DataSourceID="SqlDSLead" DataTextField="Descrizione" 
                                DataValueField="Codice" Height="22px" Enabled="true" Width="350px">
                                <asp:ListItem Value="0" Text="Non definito"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDSLead" runat="server" 
                                SelectCommand="SELECT * FROM [LeadSource] ORDER BY [Descrizione]">
                            </asp:SqlDataSource>
                            </td>
                            <td>
                            <asp:CheckBox ID="chkTuttiLead" runat="server" Text="Seleziona tutti" AutoPostBack="true" Checked="false" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                            &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                            <asp:CheckBox ID="chkStampaLottiLS" runat="server" AutoPostBack="true" TabIndex="10" Text="Stampa anche i Lotti/N° Serie collegati" />    
                            </td>
                        </tr>
                         <tr>
                            <td colspan="2">
                            <asp:CheckBox ID="chkStampaDocRefIntLS" runat="server" AutoPostBack="true" TabIndex="10" Text="Stampa anche il documento collegato (Ordine<>DDT)" />
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelSintAnal" style="margin-top: 0px;" runat="server" groupingtext="Selezione tipo riepilogo" Height="100px" 
                        Width="100%">
                        <asp:RadioButton ID="rbtnSintetico" runat="server" Text="Sintetico" Checked="true" GroupName="SintAnal" AutoPostBack="True"/>
                        &nbsp;
                        <asp:RadioButton ID="rbtnAnalitico" runat="server" Text="Analitico" GroupName="SintAnal" AutoPostBack="True" Checked="false"/>
                        <br />
                    </asp:Panel>
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