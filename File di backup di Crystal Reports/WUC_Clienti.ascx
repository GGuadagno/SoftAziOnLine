<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Clienti.ascx.vb" Inherits="SoftAziOnLine.WUC_Clienti" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<%@ Register src="~/WebUserControl/WFP_ElencoCliForn.ascx" TagName="WFPElencoCliForn" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControl/WFP_Elenco.ascx" TagName="WFPElenco" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControlTables/WUC_AltriIndirizziClienti.ascx" TagName="AltriIndirizziCliente" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControlTables/WUC_DestMerce.ascx" TagName="DestMerce" TagPrefix="wuc" %>
<%@ Register src="../WebUserControl/WUC_Attesa.ascx" tagname="WUC_Attesa" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_Zone.ascx" tagname="WFPZone" tagprefix="wuc3" %>
<%@ Register src="../WebUserControl/WFP_Vettori.ascx" tagname="WFP_Vettori" tagprefix="uc3" %>
<%@ Register src="../WebUserControl/WFP_Agenti.ascx" tagname="WFP_Agenti" tagprefix="uc4" %>
<%@ Register src="../WebUserControl/WFP_Categorie.ascx" tagname="WFP_Categorie" tagprefix="uc5" %>
<%@ Register src="../WebUserControl/WFP_BancheIBAN.ascx" tagname="WFP_BancheIBAN" tagprefix="uc7" %>
<%@ Register src="../WebUserControl/WFP_TipoFatturazione.ascx" tagname="WFP_TipoFatturazione" tagprefix="uc12" %>
<%@ Register src="../WebUserControlTables/WUC_ClientiOCPregr.ascx" TagName="ClientiOCPregr" TagPrefix="uc13"  %>
<%@ Register src="../WebUserControlTables/WUC_ClientiPRPregr.ascx" TagName="ClientiPRPregr" TagPrefix="uc14"  %>
<style type="text/css">
    .btnstyle
    {
        Width: 108px;
        height: 35px;
        margin-left: 0px;
        white-space: pre-wrap;
    }
    .btnstyle2R
    {
        Width: 108px;
        height: 45px;
        margin-left: 0px;
        white-space: pre-wrap;
    }   
    .style23
    {
        height: 478px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="600px" BackColor="Silver">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
            <wuc:WFPElencoCliForn ID="WFPElencoCli" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
            <uc2:WUC_Attesa ID="WUC_Attesa1" runat="server" />
            <wuc:WFPElenco ID="WFPElencoNazioni" runat="server" Tabella="Nazioni" Titolo="Elenco Nazioni"/>
            <wuc:WFPElenco ID="WFPElencoProvince" runat="server" Tabella="Province" Titolo="Elenco Province"/>
            <wuc:WFPElenco ID="WFPElencoAliquotaIVA" runat="server" Tabella="AliquoteIva" Titolo="Elenco Aliquote IVA"/>
            <wuc:WFPElenco ID="WFPElencoPagamenti" runat="server" Tabella="Pagamenti" Titolo="Elenco Pagamenti"/>
            <wuc:WFPElenco ID="WFPElencoAgenti" runat="server" Tabella="Agenti" Titolo="Elenco Agenti"/>
            <wuc:WFPElenco ID="WFPElencoAgentiEsePrec" runat="server" Tabella="Agenti" Titolo="Elenco Agenti"/>
            <wuc:WFPElenco ID="WFPElencoZone" runat="server" Tabella="Zone" Titolo="Elenco Zone"/>
            <wuc:WFPElenco ID="WFPElencoVettori" runat="server" Tabella="Vettori" Titolo="Elenco Vettori"/>
            <wuc:WFPElenco ID="WFPElencoCategorie" runat="server" Tabella="Categorie" Titolo="Elenco Categorie"/>
            <wuc:WFPElenco ID="WFPElencoListVenT" runat="server" Tabella="ListVenT" Titolo="Elenco Lista"/>
            <wuc:WFPElenco ID="WFPElencoConti" runat="server" Tabella="PianoDeiConti" Titolo="Elenco Conti"/>
            <wuc:WFPElenco ID="WFPElencoTipoFatt" runat="server" Tabella="TipoFatt" Titolo="Elenco Tipi fatturazione"/>
            <uc12:WFP_TipoFatturazione ID="WFPTipoFatturazione" runat="server" />
            <wuc3:WFPZone ID="WFPZone" runat="server"/>
            <uc3:WFP_Vettori ID="WFP_Vettori1" runat="server" />
            <uc4:WFP_Agenti ID="WFP_Agenti1" runat="server" />
            <uc5:WFP_Categorie ID="WFP_Categorie1" runat="server" />
            <uc7:WFP_BancheIBAN ID="WFP_BancheIBAN1" runat="server" />
            <asp:SqlDataSource ID="SqlDSBancheIBAN" runat="server" 
                    SelectCommand="SELECT *, Descrizione + ' - ' + IBAN AS DesCompleta FROM [BancheIBAN] ORDER BY [Descrizione]">
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SqlDSEstrConto" runat="server"
                    SelectCommand="get_Azi_EstrContoCli" 
                    SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:SessionParameter Name="AData" Type="String" SessionField="ECCDataA" />
                    <asp:SessionParameter Name="CodCliente" SessionField="CodiceCliente" 
                        Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
        <table style="width:auto; height:auto; vertical-align:top;" class="sfondopagine">
        <tr>
                    <td class="style23">
                        <div id="divPrincipale" runat="server" >
                            <table class="sfondopagine" style="width:100%; height:100%;">
                                <tr>
                                    <td style="width:125px">Codici: Cliente</td>
                                    <td><asp:TextBox ID="txtCodCliente" AutoPostBack="true" runat="server" Width="120px" MaxLength="16" BorderStyle="None"></asp:TextBox>
                                        <asp:Label ID="lblLabelNEW" runat="server" Width="300px" Visible="false" BorderColor="White"
                                                        BorderStyle="Outset" Font-Bold="True" ForeColor="Black">Nuovo codice</asp:Label>
                                        <asp:Label ID="Label8" runat="server" Visible="true" BorderStyle="None">IPA</asp:Label>
                                        <asp:TextBox ID="txtIPA" AutoPostBack="False" runat="server" Width="90px" MaxLength="7" BorderStyle="None"></asp:TextBox>
                                        <asp:Label ID="lblCMabell" runat="server" Visible="false" BorderStyle="None">MABELL</asp:Label>
                                        <asp:TextBox ID="txtCMabell" AutoPostBack="False" runat="server" Visible="false" Width="100px" MaxLength="16" BorderStyle="None"></asp:TextBox>
                                    </td>
                                    <td rowspan="2" align="right">
                                        <asp:Button ID="btnCreaPR" runat="server" class="btnstyle" Width="100px" Text="Nuovo Prev." />
                                        <asp:Button ID="btnCreaOC" runat="server" class="btnstyle" Width="100px" Text="Nuovo Ordine" />
                                        <asp:Button ID="btnRicerca" runat="server" class="btnstyle" Width="100px" Text="Ricerca clienti" ToolTip="Ricerca clienti da elenco"/>
                                        <asp:Button ID="btnFirst" runat="server" Text="<<" class="btnstyle" Width="30px" ToolTip="Primo"/>
                                        <asp:Button ID="btnPrev" runat="server" Text="<"  class="btnstyle" Width="30px" ToolTip="Precedente"/>
                                        <asp:Button ID="btnNext" runat="server" Text=">"  class="btnstyle" Width="30px" ToolTip="Successivo"/>
                                        <asp:Button ID="btnLast" runat="server" Text=">>"  class="btnstyle" Width="30px" ToolTip="Ultimo"/>
                                        <asp:Button ID="btnStampa" runat="server" Text="Stampe" class="btnstyle" Width="65px"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Ragione sociale</td>
                                    <td colspan="2"><asp:TextBox ID="txtRagSoc" runat="server" Width="425px" AutoPostBack="False" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                                </tr>
                                 <tr>
                                    <td colspan="3">
                                    <ajaxToolkit:TabContainer ID="TabContainer0" runat="server" ActiveTabIndex="0" Width="1110px" BackColor="Silver" Height="500px" AutoPostBack="True">
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanelDA" HeaderText="Dati Anagrafici" BackColor="Silver" Height ="100%" >
                                            <HeaderTemplate>Dati Anagrafici</HeaderTemplate>
                                            <ContentTemplate>
                                                <table align="center" class="sfondopagine" style="width:100%; height:100%;">
                                                    <tr>
                                                        <td>
                                                            <asp:Panel ID="panel5" runat="server" BorderWidth="0px" CssClass="sfondopagine" style="width:100%; height:100%;">
                                                                <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" BackColor="Silver" Width="1080px" AutoPostBack="True">
                                                                    <ajaxToolkit:TabPanel ID="TabPanelDADA" runat="server" BackColor="Silver" HeaderText="Dati anagrafici">
                                                                        <HeaderTemplate>Dati anagrafici</HeaderTemplate>
                                                                        <ContentTemplate>
                                                                            <table class="sfondopagine" style="width:100%; height:460px;">
                                                                                <tr>
                                                                                    <td style="width:150px">Denominazione</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtDenominazione" runat="server" MaxLength="50" Width="550px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Titolare</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtTitolare" runat="server" MaxLength="50" Width="550px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Indirizzo</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtIndirizzo" runat="server" MaxLength="50" Width="380px" Text="" BorderStyle="None"></asp:TextBox>
                                                                                        <asp:Label ID="Label7" runat="server" Text="N° Civico"></asp:Label>
                                                                                        <asp:TextBox ID="txtNumCivico" runat="server" MaxLength="10" Width="90px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Località</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtLocalita" runat="server" MaxLength="50" Width="380px" BorderStyle="None"></asp:TextBox>
                                                                                        <asp:Label ID="Label11" runat="server" Text="Provincia"></asp:Label>
                                                                                        <asp:Button ID="btnTrovaProvincia" runat="server" AutoPostBack="true" Height="22px" Text="?" Width="30px" />
                                                                                        &nbsp;<asp:TextBox ID="txtProvincia" runat="server" AutoPostBack="True" MaxLength="2" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>CAP</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtCap" runat="server" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                                        &nbsp;&nbsp;&nbsp;<asp:Label ID="Label12" runat="server" Text="Nazione"></asp:Label>
                                                                                        <asp:Button ID="btnTrovaNazione" runat="server" Height="22px" Text="?" Width="30px" />&#160;
                                                                                        <asp:TextBox ID="txtCodNazione" runat="server" AutoPostBack="True" MaxLength="3" Width="30px" BorderStyle="None"></asp:TextBox>&#160;
                                                                                        <asp:Label ID="lblNazione" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="335px"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Partita IVA</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtPartitaIVA" runat="server" MaxLength="11" Width="200px" 
                                                                                            AutoPostBack="True" ToolTip="P.IVA Italiana per i Clienti/Fornitori Esteri: ITnnnnnnnnnnn" BorderStyle="None"></asp:TextBox>&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160; Codice Fiscale 
                                                                                        <asp:TextBox ID="txtCodFiscale" runat="server" MaxLength="16" Width="190px" 
                                                                                            AutoPostBack="True" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Telefono 1</td>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtTelefono1" runat="server" MaxLength="30" Width="200px" BorderStyle="None" AutoPostBack="True" ></asp:TextBox>
                                                                                    </td>
                                                                                    <td rowspan="3">
                                                                                        <table>
                                                                                        <tr>
                                                                                                <td class="styleBordato">
                                                                                                    <asp:RadioButton ID="rdPersonaFisica" runat="server" AutoPostBack="True" GroupName="TipoPersona" Text="Persona fisica" TextAlign="Left" />
                                                                                                    <asp:RadioButton ID="rdPersonaGiuridica" runat="server" AutoPostBack="True" Checked="True" GroupName="TipoPersona" Text="Persona giuridica" TextAlign="Left" /><br /><br />Data di nascita&#160;
                                                                                                    <asp:TextBox ID="txtDataNascita" runat="server" Enabled="False" MaxLength="10" TabIndex="3" Width="80px" BorderStyle="None"></asp:TextBox>
                                                                                                    <asp:ImageButton ID="imgBtnShowCalendar" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" />
                                                                                                    <ajaxToolkit:CalendarExtender ID="txtDataNascita_CalendarExtender" runat="server" Enabled="False" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar" TargetControlID="txtDataNascita"></ajaxToolkit:CalendarExtender>
                                                                                                    <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" ControlToValidate="txtDataNascita" ErrorMessage="data invalida" ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Telefono 2</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtTelefono2" runat="server" MaxLength="30" Width="200px" BorderStyle="None" AutoPostBack="True" ></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Telefono Fax</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtTelefonoFax" runat="server" MaxLength="30" Width="200px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>E-mail</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Width="650px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>PEC E-mail</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtPECEMail" runat="server" MaxLength="310" Width="650px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>E-mail Fatture</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtEmailInvioFatt" runat="server" MaxLength="100" Width="650px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>E-mail Scadenze</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtEmailInvioScad" runat="server" MaxLength="100" Width="550px" BorderStyle="None"></asp:TextBox>
                                                                                        <asp:CheckBox ID="CheckInvioMailScad" runat="server" Text="Si Invia" TextAlign="Left" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Riferimento</td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtRiferimento" runat="server" MaxLength="500" Width="890px" BorderStyle="None"></asp:TextBox>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Sede&#160; 
                                                                                        <asp:Button ID="btnTrovaSede" runat="server" Height="22px" Text="?" Width="30px" />
                                                                                    </td>
                                                                                    <td colspan="2">
                                                                                        <asp:TextBox ID="txtCodSede" runat="server" AutoPostBack="True" MaxLength="16" Width="120px" BorderStyle="None"></asp:TextBox>&#160; 
                                                                                        <asp:Label ID="lblSede" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="295px"></asp:Label>&nbsp;&nbsp;
                                                                                        <asp:CheckBox ID="checkAllIVA" runat="server" Text="All.IVA > Sede" TextAlign="Left" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </ContentTemplate>
                                                                    </ajaxToolkit:TabPanel>
                                                                    <ajaxToolkit:TabPanel ID="TabPanelDAAD" runat="server" BackColor="Silver" HeaderText="Altri dati anagrafici / Indirizzi / Contatti" style="width:100%; height:100%;">
                                                                        <HeaderTemplate>Altri dati anagrafici / Indirizzi / Contatti</HeaderTemplate>
                                                                        <ContentTemplate>
                                                                            <table class="sfondopagine" style="width:100%; height:460px;">
                                                                                <tr>
                                                                                    <td>
                                                                                        <wuc:AltriIndirizziCliente ID="GWAltriIndirizzi" runat="server" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </ContentTemplate>
                                                                    </ajaxToolkit:TabPanel>
                                                                </ajaxToolkit:TabContainer>
                                                            </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanelC" HeaderText="Commerciale" BackColor="Silver" Height ="100%">
                                            <HeaderTemplate>Commerciale</HeaderTemplate>
                                                <ContentTemplate>
                                                    <table align="center" class="sfondopagine" style="width:100%; height:100%;">
                                                        <tr>
                                                            <td align="center">
                                                                <%--<br />--%>
                                                                <asp:Panel ID="panel4" runat="server" BorderWidth="0px" width="100%">
                                                                    <table align="left" class="sfondopagine" style="width:100%; height:500px;">
                                                                        <tr>
                                                                            <td>Regime IVA</td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtCodRegimeIVA" runat="server" AutoPostBack="true" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                                <asp:Button ID="btnTrovaRegimeIVA" runat="server" Height="22px" Text="?" Width="30px" />
                                                                                <asp:Label ID="lblRegimeIva" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="455px"></asp:Label>
                                                                            </td>
                                                                            <td rowspan="2">
                                                                                <asp:CheckBox ID="checkEscudiAllegatiIVA" runat="server" AutoPostBack="false" Height="15px" Text="Escludi allegati IVA" /><br />
                                                                                <asp:CheckBox ID="checkIVAInSospensione" runat="server" AutoPostBack="false" Height="15px" Text="IVA in sospensione" /><br />
                                                                                <asp:CheckBox ID="checkSpliIVA" runat="server" AutoPostBack="false" Height="15px" Text="Split payment IVA" /><br />
                                                                                <asp:CheckBox ID="checkNonFatturabile" runat="server" AutoPostBack="false" Height="15px" Text="Non fatturabile" />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>Pagamento</td>
                                                                            <td colspan="2">
                                                                                <asp:TextBox ID="txtCodPagamento" runat="server" AutoPostBack="true" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                                <asp:Button ID="btnTrovaPagamento" runat="server" Height="22px" Text="?" Width="30px" />
                                                                                <asp:Label ID="lblPagamento" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="455px"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left" colspan="3">
                                                                                <asp:Label ID="Label1" runat="server" BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="true" ForeColor="Black" Text="Riferimenti bancari del Cliente" Width="665px"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>Codice ABI</td>
                                                                            <td colspan="2">
                                                                                <asp:TextBox ID="txtCodABI" runat="server" AutoPostBack="true" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>&#160;&#160;Codice CAB 
                                                                                <asp:TextBox ID="txtCodCAB" runat="server" AutoPostBack="true" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>&#160;&#160;Numero C/Corrente &#160;
                                                                                <asp:TextBox ID="txtNumCC" runat="server" MaxLength="15" Width="120px" BorderStyle="None"></asp:TextBox>&#160;&#160;CIN 
                                                                                <asp:TextBox ID="txtCIN" runat="server" MaxLength="1" Width="30px" BorderStyle="None"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>Banca</td>
                                                                            <td>
                                                                                <asp:Button ID="btnTrovaBanca" runat="server" Height="22px" Text="?" Visible="false" Width="30px" />
                                                                                <asp:Label ID="lblBanca" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="550px"></asp:Label>
                                                                            </td>
                                                                            <td align="right" rowspan="9"><br />
                                                                                <asp:Panel ID="panel6" runat="server" align="center" BorderWidth="1px" Height="220px" width="150px"><table><tr><td>Giorni di chiusura<br /><br />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>Primo giorno<br />
                                                                                <asp:DropDownList ID="ddlPrimoGiorno1" runat="server">
                                                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                                    <asp:ListItem Text="Lunedì" Value="Lunedi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Martedì" Value="Martedi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Mercoledì" Value="Mercoledi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Giovedì" Value="Giovedi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Venerdì" Value="Venerdi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Sabato" Value="Sabato"></asp:ListItem>
                                                                                    <asp:ListItem Text="Domenica" Value="Domenica"></asp:ListItem>
                                                                                </asp:DropDownList><br />
                                                                                <asp:CheckBox ID="checkMattino1" runat="server" Text="Mattino" /><br />
                                                                                <asp:CheckBox ID="checkPomeriggio1" runat="server" Text="Pomeriggio" /><br /><br />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>Secondo giorno<br />
                                                                                <asp:DropDownList ID="ddlPrimoGiorno2" runat="server">
                                                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                                    <asp:ListItem Text="Lunedì" Value="Lunedi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Martedì" Value="Martedi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Mercoledì" Value="Mercoledi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Giovedì" Value="Giovedi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Venerdì" Value="Venerdi"></asp:ListItem>
                                                                                    <asp:ListItem Text="Sabato" Value="Sabato"></asp:ListItem>
                                                                                    <asp:ListItem Text="Domenica" Value="Domenica"></asp:ListItem>
                                                                                </asp:DropDownList><br />
                                                                                <asp:CheckBox ID="checkMattino2" runat="server" Text="Mattino" /><br />
                                                                                <asp:CheckBox ID="checkPomeriggio2" runat="server" Text="Pomeriggio" />
                                                                            </td>
                                                                        </tr>
                                                                    </table><br />
                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Filiale</td>
                                                            <td colspan="2">
                                                                <asp:Label ID="lblFiliale" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="550px"></asp:Label>
                                                            </td>
                                                        </tr>
                                                            <tr>
                                                            <td align="left" colspan="3">
                                                                <asp:Label ID="Label2" runat="server" BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="true" ForeColor="Black" Text="Riferimenti bancari Azienda" Width="665px"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td><asp:Label ID="Label3" runat="server">Seleziona Banca</asp:Label></td>
                                                            <td colspan="2">
                                                                <div>
                                                                    <asp:DropDownList ID="DDLBancheIBAN" runat="server" AppendDataBoundItems="True" AutoPostBack="True" DataSourceID="SqlDSBancheIBAN" DataTextField="DesCompleta" DataValueField="IBAN" Height="22px" TabIndex="11" Width="520px">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:Button ID="btnCercaBanca" runat="server" CausesValidation="False" CommandName="btnCercaBanca" Text="+" ToolTip="Gestione anagrafiche Banche Azienda" />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div>
                                                                    <asp:Label ID="lblABI" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="45px"></asp:Label>
                                                                    <asp:Label ID="lblCAB" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="45px"></asp:Label>
                                                                </div>
                                                            </td>
                                                            <td colspan="2">
                                                                <div>
                                                                    <asp:Label ID="Label4" runat="server">IBAN</asp:Label>
                                                                    <asp:Label ID="lblIBAN" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="290px">IBAN</asp:Label>
                                                                    <asp:Label ID="Label5" runat="server">C/Corrente</asp:Label>
                                                                    <asp:Label ID="lblContoCorrente" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="135px">Conto Corrente</asp:Label>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Agente</td>
                                                            <td colspan="2">
                                                                <div>
                                                                    <asp:TextBox ID="txtCodAgente" runat="server" AutoPostBack="true" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:Button ID="btnTrovaAgente" runat="server" Height="22px" Text="?" Width="30px" />
                                                                    <asp:Button ID="btnAgente" runat="server" CausesValidation="False" CommandName="btnAgente" Height="22px" Text="+" ToolTip="Gestione anagrafiche Agenti" Width="30px" />
                                                                    <asp:Label ID="lblAgente" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="422px"></asp:Label>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td></td><%--<td>Agente Es.Prec.</td>--%> 
                                                            <td colspan="2">
                                                                <div>
                                                                    <asp:TextBox ID="txtCodAgenteEsePrec" runat="server" AutoPostBack="true" MaxLength="5" Visible="false" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:Button ID="btnTrovaAgenteEsePrec" runat="server" Height="22px" Text="?" Visible="false" Width="30px" />
                                                                    <asp:Button ID="btnAgenteEsePrec" runat="server" CausesValidation="False" CommandName="btnAgenteEsePrec" Height="22px" Text="+" ToolTip="Gestione anagrafiche Agenti" Visible="false" Width="30px" />
                                                                    <asp:Label ID="lblAgenteEsePrec" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Visible="false" Width="422px"></asp:Label>
                                                                    <asp:Label ID="lblMessAge" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Blue" Visible="true" Width="550px"></asp:Label>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Zona</td>
                                                            <td colspan="2">
                                                                <div>
                                                                    <asp:TextBox ID="txtCodZona" runat="server" AutoPostBack="true" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:Button ID="btnTrovaZona" runat="server" Text="?" ToolTip="Ricerca Zone" />
                                                                    <asp:Button ID="btnZone" runat="server" CausesValidation="False" CommandName="btnZone" Height="22px" Text="+" ToolTip="Gestione anagrafiche Zone" Width="30px" />
                                                                    <asp:Label ID="lblZona" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="422px"></asp:Label>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Vettore</td>
                                                            <td colspan="2">
                                                                <div>
                                                                    <asp:TextBox ID="txtCodVettore" runat="server" AutoPostBack="true" MaxLength="5" Width="50px"  BorderStyle="None"></asp:TextBox>
                                                                    <asp:Button ID="btnTrovaVettore" runat="server" Text="?" ToolTip="Ricerca Vettore" />
                                                                    <asp:Button ID="btnVettori" runat="server" CausesValidation="False" CommandName="btnVettori" Height="22px" Text="+" ToolTip="Gestione anagrafiche Vettori" Width="30px" />
                                                                    <asp:Label ID="lblVettore" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="422px"></asp:Label>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Categoria</td>
                                                            <td colspan="2">
                                                                <div>
                                                                    <asp:TextBox ID="txtCodCategoria" runat="server" AutoPostBack="true" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                    <asp:Button ID="btnTrovaCategoria" runat="server" Text="?" ToolTip="Ricerca Categorie" />
                                                                    <asp:Button ID="btnCategorie" runat="server" CausesValidation="False" CommandName="btnCategorie" Height="22px" Text="+" ToolTip="Gestione anagrafiche Categorie" Width="30px" />
                                                                    <asp:Label ID="lblCategorie" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="422px"></asp:Label>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Listino</td>
                                                            <td colspan="2">
                                                                <asp:TextBox ID="txtCodListino" runat="server" AutoPostBack="true" MaxLength="5" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                <asp:Button ID="btnTrovaListino" runat="server" Height="22px" Text="?" Width="30px" />
                                                                <asp:Label ID="lblListino" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="455px"></asp:Label>
                                                                
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td></td>
                                                            <td colspan="2">
                                                                <asp:TextBox ID="txtCodRicavoFT" runat="server" AutoPostBack="true" MaxLength="16" Visible="false" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                <asp:Button ID="btnTrovaRicavoFT" runat="server" Height="22px" Text="?" Visible="false" Width="30px" />
                                                                <asp:TextBox ID="txtRicavoFT" runat="server" BorderWidth="1px" Enabled="false" Visible="false" Width="455px" BorderStyle="None"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>Fatturazione</td>
                                                            <td>
                                                                <asp:TextBox ID="txtCodTipoFatt" runat="server" AutoPostBack="true" MaxLength="2" Width="50px" BorderStyle="None"></asp:TextBox>
                                                                <asp:Button ID="btnTrovaTipoFatt" runat="server" Height="22px" Text="?" Width="30px" />
                                                                <asp:Button ID="btnGestTipoFatt" runat="server" CausesValidation="False" CommandName="btnGestTipoFatt" Height="22px" Text="+" ToolTip="Gestione anagrafiche Tipo Fatturazione" Width="30px" />
                                                                <asp:Label ID="lblTipoFatt" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="422px"></asp:Label>
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="Label6" runat="server" BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="true" ForeColor="Black" Text="Massimo credito"></asp:Label>
                                                                <asp:TextBox ID="txtMaxCredito" runat="server" MaxLength="15" Width="100px" BorderStyle="None"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanelDM" HeaderText="Destinazione Merce" style="width:100%; height:100%;">
                                            <HeaderTemplate>Destinazione Merce</HeaderTemplate>
                                            <ContentTemplate>
                                                <table align="left" class="sfondopagine" style="width:100%; height:100%;">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:Panel ID="panel2" runat="server" BorderWidth="0px" CssClass="sfondopagine">
                                                                <wuc:DestMerce ID="DestMerceCliente" runat="server" />
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanelN" HeaderText="Note" style="width:100%; height:100%;">
                                            <HeaderTemplate>Note</HeaderTemplate>
                                            <ContentTemplate>
                                                <table align="center" class="sfondopagine" style="width:100%; height:100%;">
                                                    <tr>
                                                        <td align="center"><br />
                                                            <asp:Panel ID="panelNote" runat="server" BorderWidth="0px">
                                                                <table>
                                                                    <tr>
                                                                        <td style="vertical-align:top">Note</td>
                                                                        <asp:CheckBox ID="chkSegnalaNote" runat="server" Text="Attiva avviso in gestione Preventivi/Ordini" Checked="false" ForeColor="Blue" Font-Bold="true" />
                                                                        <td><asp:TextBox ID="txtNote" runat="server" Height="450px" TextMode="MultiLine" Width="750px" BorderStyle="None"></asp:TextBox></td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanelS" HeaderText="Saldi" style="width:100%; height:100%;">
                                        <HeaderTemplate>Saldi</HeaderTemplate>
                                            <ContentTemplate>
                                                <table align="center" class="sfondopagine" style="width:100%; height:100%;">
                                                    <tr>
                                                        <td align="center"><br />
                                                            <asp:Panel ID="panel3" runat="server" BorderWidth="0px" style="width:100%; height:100%;">
                                                                <table align="center" class="sfondopagine" style="width:100%; height:480px;">
                                                                    <tr>
                                                                        <td></td>
                                                                        <td>Saldo Dare</td>
                                                                        <td>Saldo Avere</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>Saldo apertura</td>
                                                                        <td>
                                                                        <asp:Label ID="lblSaldoAperturaDare" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                                                                        </td>
                                                                        <td>
                                                                        <asp:Label ID="lblSaldoAperturaAvere" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>Saldo progressivo</td>
                                                                        <td>
                                                                        <asp:Label ID="lblSaldoProgDare" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                                                                        </td>
                                                                        <td>
                                                                        <asp:Label ID="lblSaldoProgAver" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>Saldo attuale</td>
                                                                        <td>
                                                                        <asp:Label ID="lblSaldoAttualeDare" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                                                                        </td>
                                                                        <td>
                                                                        <asp:Label ID="lblSaldoAttualeAvere" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>Saldo chiusura</td>
                                                                        <td>
                                                                        <asp:Label ID="lblSaldoChiusuraDare" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                                                                        </td>
                                                                        <td>
                                                                        <asp:Label ID="lblSaldoChiusuraAvere" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="3"><br /=""></br></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td></td>
                                                                        <td>Data ultimo aggiornamento saldi</td>
                                                                        <td>
                                                                        <asp:Label ID="lblDataSaldoUltimoAgg" runat="server" BorderStyle="Outset" Width="150px" 
                                                                                style="text-align:center" Font-Bold="True"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <%--<table align="center" class="sfondopagine" style="width:100%; height:100%;"></table>--%></asp:Panel>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanelEC" HeaderText="Estratto conto" style="width:100%; height:100%;">
                                            <HeaderTemplate>Estratto conto</HeaderTemplate>
                                            <ContentTemplate>
                                                <table align="center" class="sfondopagine" style="width:100%; height:445px;">
                                                    <tr valign="top">
                                                        <td align="center"><br />
                                                            <asp:Panel ID="PanelEC" runat="server" BorderWidth="0px" style="width:100%; height:100%;">
                                                                <table align="center" class="sfondopagine" style="width:100%; height:480px;">
                                                                    <tr>
                                                                        <td align="right" style="width:50%">Elenca scadenze con data inferiore o uguale a</td>
                                                                        <td style="width:50%">
                                                                            <asp:TextBox ID="txtECDataA" runat="server" MaxLength="10" Width="80px" BorderStyle="None"></asp:TextBox>
                                                                            <asp:ImageButton ID="imgBtnShowCalendarEC" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" />
                                                                            <ajaxToolkit:CalendarExtender ID="txtECDataA_CalendarExtender" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarEC" TargetControlID="txtECDataA"></ajaxToolkit:CalendarExtender>
                                                                            <asp:RegularExpressionValidator ID="RegExpValECDataA" runat="server" ControlToValidate="txtECDataA" ErrorMessage="data invalida" ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>&nbsp;</td>
                                                                        <td align="right"><asp:Button runat="server" ID="btnVisualizzaEC" Text="Visualizza" /><asp:Button runat="server" ID="btnStampaEC" Text="Stampa" /></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <asp:Panel ID="pnlScrollEC" runat="server" Width="100%" Height="400px" ScrollBars="Auto">
                                                                                <asp:GridView ID="GridViewEC" runat="server" AllowSorting="True" 
                                                                                    AutoGenerateColumns="False" BackColor="Silver" CssClass="GridViewStyle"  
                                                                                    DataSourceID="SqlDSEstrConto" EmptyDataText="Nessun dato disponibile." 
                                                                                    EnableTheming="True" GridLines="None" Width="100%" Visible="False" 
                                                                                    PageSize="15">
                                                                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                                                                    <Columns>
                                                                                        <asp:BoundField DataField="N_Doc" HeaderText="N° documento" ReadOnly="True"><ItemStyle Width="70px" /></asp:BoundField>
                                                                                        <asp:BoundField DataField="Data_Doc" HeaderText="Data documento" ReadOnly="True"><ItemStyle Width="80px" /></asp:BoundField>
                                                                                        <asp:BoundField DataField="Importo" HeaderText="Importo documento" ReadOnly="True"><ItemStyle Width="80px" /></asp:BoundField>
                                                                                        <asp:BoundField DataField="DesPagamento" HeaderText="Tipo di pagamento" ReadOnly="True"><ItemStyle Width="250px" /></asp:BoundField>
                                                                                        <asp:BoundField DataField="Importo_Residuo" HeaderText="Importo residuo" ReadOnly="True"><ItemStyle Width="100px" /></asp:BoundField>
                                                                                        <asp:BoundField DataField="Riga" HeaderText="NR" ReadOnly="True"><ItemStyle Width="20px" /></asp:BoundField>
                                                                                        <asp:BoundField DataField="Data_Scad" HeaderText="Data scadenza" ReadOnly="True"><ItemStyle Width="80px" /></asp:BoundField>
                                                                                    </Columns>
                                                                                    <HeaderStyle CssClass="HeaderStyle" />
                                                                                    <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" LastPageImageUrl="~/Immagini/GridView/page-last.gif" Mode="NextPreviousFirstLast" NextPageImageUrl="~/Immagini/GridView/page-next.gif" PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                                                                                    <PagerStyle CssClass="PagerStyle" HorizontalAlign="Center" />
                                                                                    <RowStyle CssClass="RowStyle" />
                                                                                    <SelectedRowStyle CssClass="SelectedRowStyle" />
                                                                                </asp:GridView>
                                                                            </asp:Panel>
                                                                        </td>
                                                                    </tr>
                                                                    <tr style="height:100%; vertical-align:top">
                                                                        <td>&nbsp;</td>
                                                                        <td>    
                                                                            <table width="100%" runat="server" id="TblScadenze" visible="False">
                                                                                <tr runat="server">
                                                                                    <td runat="server">&nbsp</td>
                                                                                    <td runat="server"><asp:Label ID="lblDesTotScadenze" runat="server" Text="Totale Importo residuo"></asp:Label></td>
                                                                                    <td runat="server"><asp:Label ID="lblTotScadenze" runat="server" 
                                                                                            BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="True" 
                                                                                            ForeColor="Black" Text="0,00"></asp:Label></td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanelSC" HeaderText="Scheda contabile" style="width:100%; height:100%;">
                                            <HeaderTemplate>Scheda contabile</HeaderTemplate>
                                            <ContentTemplate>
                                                <table align="center" class="sfondopagine" style="width:100%; height:480px;">
                                                    <tr valign="top">
                                                        <td align="center">
                                                            <asp:Panel ID="panel7" runat="server" BorderWidth="0px" style="width:100%; height:100%;">
                                                                <table align="center" class="sfondopagine" style="width:100%; height:100%; vertical-align:top;" width="700px">
                                                                    <tr>
                                                                        <td align="right" style="width:50%">Dalla data</td>
                                                                        <td style="width:50%">
                                                                            <asp:TextBox ID="txtSCDataDa" runat="server" MaxLength="10" Width="80px" BorderStyle="None"></asp:TextBox>
                                                                            <asp:ImageButton ID="imgBtnShowCalendarSCDataDa" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" />
                                                                            <ajaxToolkit:CalendarExtender ID="txtSCDataDa_CalendarExtender" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarSCDataDa" TargetControlID="txtSCDataDa"></ajaxToolkit:CalendarExtender>
                                                                            <asp:RegularExpressionValidator ID="RegExpValSCDataDa" runat="server" ControlToValidate="txtSCDataDa" ErrorMessage="data invalida" ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td align="right" style="width:50%">Alla data</td>
                                                                        <td style="width:50%">
                                                                            <asp:TextBox ID="txtSCDataA" runat="server" MaxLength="10" Width="80px" BorderStyle="None"></asp:TextBox>
                                                                            <asp:ImageButton ID="imgBtnShowCalendarSCDataA" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" />
                                                                            <ajaxToolkit:CalendarExtender ID="txtSCDataA_CalendarExtender" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarSCDataA" TargetControlID="txtSCDataA"></ajaxToolkit:CalendarExtender>
                                                                            <asp:RegularExpressionValidator ID="RegExpValSCDataA" runat="server" ControlToValidate="txtSCDataA" ErrorMessage="data invalida" ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>&nbsp;</td>
                                                                        <td align="right"><asp:Button runat="server" ID="btnVisualizzaSC" Text="Visualizza" /><asp:Button runat="server" ID="btnStampaSC" Text="Stampa" /></td>
                                                                    </tr>
                                                                    <tr style="height:200px">
                                                                        <td colspan="2" valign="top">
                                                                            <asp:Panel ID="pnlScrollSC" runat="server" ScrollBars="Auto" BorderStyle="None" Width="100%" Height="320px">
                                                                                <asp:GridView ID="GridViewSC" runat="server" AllowSorting="True" AutoGenerateColumns="False" BackColor="Silver" CssClass="GridViewStyle" EmptyDataText="Nessun dato disponibile." EnableTheming="True" GridLines="None" Width="100%" Visible="False" PagerSettings-PageButtonCount="10" PageSize="10">
                                                                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                                                                    <Columns>
                                                                                        <asp:BoundField DataField="Data_Reg" HeaderText="Data registrazione" ReadOnly="true" />
                                                                                        <asp:BoundField DataField="N_Reg" HeaderText="Prot. Co.Ge." ReadOnly="true" />
                                                                                        <asp:BoundField DataField="N_IVA" HeaderText="Prot. IVA" ReadOnly="true" />
                                                                                        <asp:BoundField DataField="N_Doc" HeaderText="N° documento" ReadOnly="True" />
                                                                                        <asp:BoundField DataField="Data_Doc" HeaderText="Data documento" ReadOnly="True" />
                                                                                        <asp:BoundField DataField="Des_Causale" HeaderText="Descrizione causale" ReadOnly="true" />
                                                                                        <asp:BoundField DataField="Dare" HeaderText="Dare" ReadOnly="true" />
                                                                                        <asp:BoundField DataField="Avere" HeaderText="Avere" ReadOnly="true" />
                                                                                    </Columns>
                                                                                    <HeaderStyle CssClass="HeaderStyle" />
                                                                                    <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" LastPageImageUrl="~/Immagini/GridView/page-last.gif" Mode="NextPreviousFirstLast" NextPageImageUrl="~/Immagini/GridView/page-next.gif" PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                                                                                    <PagerStyle CssClass="PagerStyle" HorizontalAlign="Center" />
                                                                                    <RowStyle CssClass="RowStyle" />
                                                                                    <SelectedRowStyle CssClass="SelectedRowStyle" />
                                                                                </asp:GridView>
                                                                            </asp:Panel>
                                                                        </td>
                                                                    </tr>
                                                                    <tr style="height:100%; vertical-align:top">
                                                                        <td>&nbsp;</td>
                                                                        <td>    
                                                                            <table width="100%" runat="server" id="tblSaldi" visible="false">
                                                                                <tr>
                                                                                    <td>&nbsp</td>
                                                                                    <td><asp:Label ID="Label9" runat="server" BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="true" ForeColor="Black" Text="Dare"></asp:Label></td>
                                                                                    <td><asp:Label ID="Label10" runat="server" BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="true" ForeColor="Black" Text="Avere"></asp:Label></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Saldo precedente</td>
                                                                                    <td><asp:Label ID="lblSaldoPrecDare" runat="server"></asp:Label></td>
                                                                                    <td><asp:Label ID="lblSaldoPrecAvere" runat="server"></asp:Label></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Totale movimenti</td>
                                                                                    <td><asp:Label ID="lblTotMovDare" runat="server"></asp:Label></td>
                                                                                    <td><asp:Label ID="lblTotMovAvere" runat="server"></asp:Label></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Saldo attuale</td>
                                                                                    <td><asp:Label ID="lblSaldoAttDare" runat="server" BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="true" ForeColor="Black" ></asp:Label></td>
                                                                                    <td><asp:Label ID="lblSaldoAttAvere" runat="server" BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="true" ForeColor="Black" ></asp:Label></td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanel1" HeaderText="Ordini pregressi"  style="width:100%; height:100%;">
                                            <HeaderTemplate>Preventivi pregressi</HeaderTemplate>
                                            <ContentTemplate>
                                                <table align="left" class="sfondopagine" style="width:100%; height:100%;">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:Panel ID="panel8" runat="server" BorderWidth="0px" CssClass="sfondopagine">
                                                                <uc14:ClientiPRPregr id="ClientiPRPregr1" runat="server" />
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                        <ajaxToolkit:TabPanel runat="server" ID="TabPanelOCP" HeaderText="Ordini pregressi"  style="width:100%; height:100%;">
                                            <HeaderTemplate>Ordini pregressi</HeaderTemplate>
                                            <ContentTemplate>
                                                <table align="left" class="sfondopagine" style="width:100%; height:100%;">
                                                    <tr>
                                                        <td align="left">
                                                            <asp:Panel ID="panel1" runat="server" BorderWidth="0px" CssClass="sfondopagine">
                                                                <uc13:ClientiOCPregr id="ClientiOCPregr1" runat="server" />
                                                            </asp:Panel>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </ajaxToolkit:TabPanel>
                                </ajaxToolkit:TabContainer>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
        <div id="divBottoni" runat="server">
            <td class="style23" align="left">
                <div>
                    <asp:Button ID="btnNuovo" runat="server" Text="Nuovo" class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                    <asp:Button ID="btnModifica" runat="server" Text="Modifica"  class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                    <asp:Button ID="btnAggiorna" runat="server" Text="Aggiorna" class="btnstyle" />
                </div>
                <div style="height: 15px"></div> 
                <div>
                    <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                    <asp:Button ID="btnElimina" runat="server" Text="Elimina" class="btnstyle" />
                </div>
                <div style="height: 15px"></div> 
                <div>
                    
                </div> 
                <div style="height: 15px"></div>
             </td>
        </div>
        </tr>
        </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>