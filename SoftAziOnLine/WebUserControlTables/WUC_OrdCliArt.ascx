<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_OrdCliArt.ascx.vb" Inherits="SoftAziOnLine.WUC_OrdCliArt" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
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
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco clienti"/>
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn2" runat="server" Elenco="ListaFornitori" Titolo="Elenco fornitori" />
        <br />
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 360px; width: 927px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelMagazzino" style="margin-top: 0px;" runat="server" GroupingText="Scelta Magazzino assegnato all'Ordine">
                        <div>
                            <asp:Label ID="lblMagazzino" runat="server" Height="16px">Magazzino</asp:Label>
                            <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                                   AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                                   DataTextField="Descrizione" 
                                   DataValueField="Codice" Width="545px" TabIndex="2">
                                <asp:ListItem Text="[Tutti i Magazzini]" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                                   SelectCommand="SELECT Codice, Descrizione FROM Magazzini WHERE Codice>0 ORDER BY Descrizione">
                            </asp:SqlDataSource>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;" runat="server" GroupingText="Date ordine / Tipo evasione">
                        <div>
                            <asp:Label ID="lblDallaData" runat="server" Height="16px" Width="165px">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" Width="100px" MaxLength="10" TabIndex="1"></asp:TextBox>
                            &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="2" CausesValidation="False" />
                                                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                    TargetControlID="txtDataDa"></asp:CalendarExtender>
                                                &nbsp;
                                                <%--<asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="txtDataDa" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                            &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>
                            <asp:TextBox ID="txtDataA" runat="server" Width="100px" MaxLength="10" TabIndex="3"></asp:TextBox>
                            &nbsp;
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" TabIndex="4" CausesValidation="False" />
                                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                    TargetControlID="txtDataA"></asp:CalendarExtender>
                                                &nbsp;
                                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtDataA" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                        </div>
                        <div>&nbsp;</div>
                        <div>
                            <asp:Label ID="Label1" runat="server" Height="16px"  Width="170px">Tipo evasione</asp:Label>
                            <asp:RadioButton ID="rbtnEvaso" runat="server" Text="Evasi" AutoPostBack="false" GroupName="Tipo" />
                            <asp:Label ID="Label0" runat="server" Width="5px">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnDaEvadere" runat="server" Text="Da evadere" AutoPostBack="false" GroupName="Tipo" />
                            <asp:Label ID="Label2" runat="server" Width="5px">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnParzEvaso" runat="server" Text="Parzialmente evasi" AutoPostBack="false" GroupName="Tipo" />
                            <asp:Label ID="Label3" runat="server" Width="5px">&nbsp;</asp:Label>
                             <asp:RadioButton ID="rbtnDaEvParEv" runat="server" Text="Da evadere + Parzialmente evasi" AutoPostBack="false" GroupName="Tipo"  Checked="true" />
                            <asp:Label ID="Label4" runat="server" Width="5px">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" Checked="false" AutoPostBack="false" GroupName="Tipo" />
                        </div>
                     </asp:Panel>
                     <asp:Panel ID="Panel" runat="server" groupingtext="Clienti" 
                            style="margin-top: 0px;" Height="78px" Width="859px">
                            <asp:Label ID="lblCliente" runat="server" Width="165px" Height="17px">Cliente</asp:Label>
                            <asp:Button ID="btnCliente" runat="server" class="btnstyle" Width="25px" 
                                Height="25px" Text="?" ToolTip="Ricerca cliente" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodCliente" runat="server"  Width="130px" MaxLength="16" AutoPostBack="True" TabIndex="6" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDescCliente" runat="server" Width="400px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox>
                        <br>
                        <asp:Label ID="lblTuttiClienti" runat="server" Width="170px" Height="16px">Seleziona tutti i clienti</asp:Label>
                        <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" TabIndex="8" />
                            <br>
                        </asp:Panel>
                    <asp:Panel ID="PanelFornitori" runat="server" groupingtext="Fornitori" style="margin-top: 0px;" Height="95px" Width="859px">
                            <asp:Label ID="lblFornitore" runat="server" Width="165px" Height="17px">Fornitore</asp:Label>
                            <asp:Button ID="btnFornitore" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca fornitore" Enabled="false" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodFornitore" runat="server" Width="130px" MaxLength="16" AutoPostBack="True" TabIndex="6" Enabled="false" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDescFornitore" runat="server" Width="400px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox>
                        <br>
                        <div>
                            <asp:Label ID="Label6" runat="server" Width="170px" Height="16px">Seleziona tutti i fornitori</asp:Label>
                            <asp:CheckBox ID="chkTuttiFornitori" runat="server" AutoPostBack="True" TabIndex="8" Text="" TextAlign="Left" Checked="true" />
                            <asp:Label ID="Label5" runat="server" Width="160px" Height="17px"></asp:Label>
                            <asp:CheckBox ID="chkOKFornitori" runat="server" AutoPostBack="false" TabIndex="8" Text="Mostra suddivisione fornitori" TextAlign="Left" Checked="false" />
                        </div>
                        <div>
                            <asp:Label ID="lblTipoStampa" runat="server" Height="16px"  Width="170px">Tipo Stampa</asp:Label>
                            <asp:RadioButton ID="rbtnStampaA" runat="server" Text="Analitica" AutoPostBack="false" GroupName="TipoStampa" Checked="true" />
                            <asp:Label ID="Label8" runat="server" Width="5px">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnStampaS" runat="server" Text="Sintetica" AutoPostBack="false" GroupName="TipoStampa" />
                        </div> 
                        </asp:Panel>
                        <br />
                    <asp:Panel ID="PanelArticolo" runat="server" groupingtext="Articoli" 
                            style="margin-top: 0px;" Height="110px" Width="859px">
                             <asp:Label ID="lblDal" runat="server" Width="165px" Height="17px">Dal codice</asp:Label>
                            <asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca articolo" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server" Width="130px" MaxLength="20" AutoPostBack="True" TabIndex="9" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" Width="400px" MaxLength="150" TabIndex="10" Enabled="False"  ></asp:TextBox>
                            <asp:Label ID="lblAl" runat="server" Width="165px" Height="17px">Al codice</asp:Label>
                            <asp:Button ID="btnCod2" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca articolo" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod2" runat="server" Width="130px" MaxLength="20" AutoPostBack="True" TabIndex="9" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesc2" runat="server" Width="400px" MaxLength="150" TabIndex="10" Enabled="False"  ></asp:TextBox>
                            <br>
                            <asp:Label ID="lblTuttiArticoli" runat="server" Height="16px" Width="170px">Seleziona tutti gli articoli</asp:Label>
                            <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" TabIndex="10" />
                        </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" TabIndex="20"  CausesValidation="False"/>
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" TabIndex="21"  CausesValidation="False"/>
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