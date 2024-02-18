<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Fornitori.ascx.vb" Inherits="SoftAziOnLine.WUC_Fornitori" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajaxToolkit" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControl/WFP_ElencoCliForn.ascx" TagName="WFPElencoCliForn" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControl/WFP_Elenco.ascx" TagName="WFPElenco" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControlTables/WUC_AltriIndirizziClienti.ascx" TagName="AltriIndirizziFornitore" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControlTables/WUC_DestMerce.ascx" TagName="DestMerce" TagPrefix="wuc" %>
<%@ Register src="../WebUserControl/WUC_Attesa.ascx" tagname="WUC_Attesa" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_BancheIBAN.ascx" tagname="WFP_BancheIBAN" tagprefix="uc7" %>
<%@ Register src="../WebUserControl/WFP_Zone.ascx" tagname="WFPZone" tagprefix="wuc3" %>
<%@ Register src="../WebUserControl/WFP_Vettori.ascx" tagname="WFP_Vettori" tagprefix="uc3" %>
<%@ Register src="../WebUserControl/WFP_Agenti.ascx" tagname="WFP_Agenti" tagprefix="uc4" %>
<%@ Register src="../WebUserControl/WFP_Categorie.ascx" tagname="WFP_Categorie" tagprefix="uc5" %>
<style type="text/css">
    .btnstyle
    {
        Width: 101px;
        height: 35px;
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
    <wuc:WFPElencoCliForn ID="WFPElencoFor" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori" />
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
    <wuc:WFPElenco ID="WFPElenco1" runat="server" Tabella="Agenti" Titolo="Elenco Agenti"/>
    <wuc3:WFPZone ID="WFPZone" runat="server"/>
    <uc3:WFP_Vettori ID="WFP_Vettori1" runat="server" />
    <uc4:WFP_Agenti ID="WFP_Agenti1" runat="server" />
    <uc5:WFP_Categorie ID="WFP_Categorie1" runat="server" />
    <uc7:WFP_BancheIBAN ID="WFP_BancheIBAN1" runat="server" />
    <asp:SqlDataSource ID="SqlDSBancheIBAN" runat="server" 
            SelectCommand="SELECT *, Descrizione + ' - ' + IBAN AS DesCompleta FROM [BancheIBAN] ORDER BY [Descrizione]">
    </asp:SqlDataSource>    
    <table style="width:auto; height:auto; vertical-align:top;" class="sfondopagine">
        <tr>
            <td class="style23">
                <div id="divPrincipale" runat="server" >
                    <table class="sfondopagine" style="width:100%; height:100%;">
                        <tr>
                            <td style="width:125px">Codice fornitore</td>
                            <td><asp:TextBox ID="txtCodFornitore" AutoPostBack="true" runat="server" 
                                    Width="120px" MaxLength="16"/>
                            <asp:Label ID="lblLabelNEW" runat="server" Width="300px" Visible="false" BorderColor="White"
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black">Nuovo codice</asp:Label></td>
                            <td rowspan="2" align="right">
                                <asp:Button ID="btnFirst" runat="server" Text="<<" class="btnstyle" Width="50px" ToolTip="Primo"/>
                                <asp:Button ID="btnPrev" runat="server" Text="<"  class="btnstyle" Width="50px" ToolTip="Precedente"/>
                                <asp:Button ID="btnNext" runat="server" Text=">"  class="btnstyle" Width="50px" ToolTip="Successivo"/>
                                <asp:Button ID="btnLast" runat="server" Text=">>"  class="btnstyle" Width="50px" ToolTip="Ultimo"/>
                            </td>
                        </tr>
                        <tr>
                            <td>Ragione sociale</td>
                            <td colspan="2"><asp:TextBox ID="txtRagSoc" runat="server" Width="425px" AutoPostBack="False" MaxLength="50"/></td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <ajaxToolkit:TabContainer ID="Tabs" runat="server" ActiveTabIndex="0" Width="1120px" BackColor="Silver">
                                    <ajaxToolkit:TabPanel runat="server" ID="TabPanelDA" HeaderText="Dati Anagrafici"  BackColor="Silver" Height ="100%">
                                        <HeaderTemplate>Dati Anagrafici</HeaderTemplate>
                                        <ContentTemplate>
                                            <table align="center" class="sfondopagine" style="width:100%; height:100%;">
                                                <tr>
                                                    <td>
                                                        <asp:Panel ID="panel5" runat="server" BorderWidth="0px" CssClass="sfondopagine">
                                                            <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Width="1100px" BackColor="Silver">
                                                                <ajaxToolkit:TabPanel runat="server" ID="TabPanelDADA" HeaderText="Dati anagrafici"  BackColor="Silver">
                                                                    <HeaderTemplate>Dati anagrafici</HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table class="sfondopagine" style="width:100%; height:400px;">
                                                                            <tr>
                                                                                <td>Denominazione</td>
                                                                                <td colspan="2"><asp:TextBox ID="txtDenominazione" runat="server" Width="550px" 
                                                                                        MaxLength="50" BorderStyle="None"/></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Titolare</td>
                                                                                <td colspan="2"><asp:TextBox ID="txtTitolare" runat="server" Width="550px" MaxLength="50" BorderStyle="None"/></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Indirizzo</td>
                                                                                <td colspan="2">
                                                                                    <asp:TextBox ID="txtIndirizzo" runat="server" Width="380px" MaxLength="50" BorderStyle="None"/>
                                                                                    &nbsp;N° Civico&nbsp;<asp:TextBox ID="txtNumCivico" runat="server" Width="90px" 
                                                                                        MaxLength="10"  BorderStyle="None"/>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Località</td>
                                                                                <td colspan="2">
                                                                                    <asp:TextBox ID="txtLocalita" runat="server" Width="380px" MaxLength="50" BorderStyle="None"/>
                                                                                    &nbsp;Provincia <asp:Button ID="btnTrovaProvincia" runat="server" Text="?" Width="30px" Height="22px"  AutoPostBack="true" BorderStyle="None"/>
                                                                                    &nbsp;<asp:TextBox ID="txtProvincia" runat="server" Width="50px" AutoPostBack="True" MaxLength="2"  BorderStyle="None"/>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>CAP</td>
                                                                                <td colspan="2">
                                                                                    <asp:TextBox ID="txtCap" runat="server" Width="50px" MaxLength="5"  BorderStyle="None"/>&nbsp;&nbsp;&nbsp;
                                                                                    Nazione <asp:Button ID="btnTrovaNazione" runat="server" Text="?" Width="30px" Height="22px"  BorderStyle="None"/>
                                                                                    &nbsp;<asp:TextBox ID="txtCodNazione" runat="server" Width="30px" AutoPostBack="True" MaxLength="3" BorderStyle="None"/>
                                                                                    <asp:Label ID="lblNazione" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="335px"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Partita IVA</td>
                                                                                <td colspan="2">
                                                                                    <asp:TextBox ID="txtPartitaIVA" runat="server" Width="200px" AutoPostBack="True" MaxLength="12"  BorderStyle="None" ToolTip="P.IVA Italiana per i Clienti/Fornitori Esteri: ITnnnnnnnnnnn"/>
                                                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                    Codice Fiscale 
                                                                                    <asp:TextBox ID="txtCodFiscale" runat="server" Width="190px" AutoPostBack="True" MaxLength="16"  BorderStyle="None"/>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Telefono 1</td>
                                                                                <td><asp:TextBox ID="txtTelefono1" runat="server" Width="200px" MaxLength="30" BorderStyle="None"/></td>
                                                                                <td rowspan="3">
                                                                                    <table>
                                                                                        <tr>
                                                                                            <td class="styleBordato">
                                                                                                <asp:RadioButton ID="rdPersonaFisica" runat="server" Text="Persona fisica" 
                                                                                                    TextAlign="Left" GroupName="TipoPersona" AutoPostBack="True"/>
                                                                                                <asp:RadioButton ID="rdPersonaGiuridica" runat="server" 
                                                                                                    Text="Persona giuridica" TextAlign="Left" Checked="True" 
                                                                                                    GroupName="TipoPersona" AutoPostBack="True"/>
                                                                                                <br /><br />
                                                                                                Data di nascita&nbsp;<asp:TextBox ID="txtDataNascita" runat="server" 
                                                                                                    MaxLength="10" Width="80px" TabIndex="3" Enabled="False" />
                                                                                                <asp:ImageButton ID="imgBtnShowCalendar" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" /> 
                                                                                                <ajaxToolkit:CalendarExtender ID="txtDataNascita_CalendarExtender" runat="server" 
                                                                                                    Enabled="False" TargetControlID="txtDataNascita" 
                                                                                                    PopupButtonID="imgBtnShowCalendar" Format="dd/MM/yyyy">
                                                                                                </ajaxToolkit:CalendarExtender>
                                                                                                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" ControlToValidate="txtDataNascita" 
                                                                                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" 
                                                                                                ErrorMessage="data invalida" />
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Telefono 2</td>
                                                                                <td colspan="2"><asp:TextBox ID="txtTelefono2" runat="server" Width="200px" MaxLength="30" BorderStyle="None"/></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Telefono Fax</td>
                                                                                <td colspan="2"><asp:TextBox ID="txtTelefonoFax" runat="server" Width="200px" MaxLength="30" BorderStyle="None"/></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>E-mail</td>
                                                                                <td colspan="2"><asp:TextBox ID="txtEmail" runat="server" Width="550px" MaxLength="100" BorderStyle="None"/></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>PEC E-mail</td>
                                                                                <td colspan="2"><asp:TextBox ID="txtPECEmail" runat="server" Width="550px" MaxLength="310" BorderStyle="None"/></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Riferimento</td>
                                                                                <td colspan="2">
                                                                                    <asp:TextBox ID="txtRiferimento" runat="server" Width="550px" MaxLength="50"  BorderStyle="None"/>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>Sede&nbsp;
                                                                                <asp:Button ID="btnTrovaSede" runat="server" Text="?" Width="30px" Height="22px" />
                                                                                </td>
                                                                                <td colspan="2">
                                                                                    <asp:TextBox ID="txtCodSede" runat="server" Width="120px" AutoPostBack="True" MaxLength="16" BorderStyle="None"/>&nbsp;
                                                                                    <asp:Label ID="lblSede" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="295px"></asp:Label>&nbsp;&nbsp;
                                                                                    <asp:CheckBox ID="checkAllIVA" runat="server" Text="All.IVA > Sede" TextAlign="Left" />   
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ContentTemplate>
                                                                </ajaxToolkit:TabPanel>
                                                                <ajaxToolkit:TabPanel runat="server" ID="TabPanelDAAD" HeaderText="Altri dati anagrafici / Indirizzi / Contatti"  BackColor="Silver" style="width:100%; height:100%;">
                                                                    <HeaderTemplate>
                                                                        Altri dati anagrafici / Indirizzi / Contatti
                                                                    </HeaderTemplate>
                                                                    <ContentTemplate>
                                                                        <table class="sfondopagine" style="width:100%; height:400px;">
                                                                            <tr>
                                                                                <td>
                                                                                    <wuc:AltriIndirizziFornitore ID="GWAltriIndirizzi" runat="server" />
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
                                    <ajaxToolkit:TabPanel runat="server" ID="TabPanelC" HeaderText="Commerciale" style="width:100%; height:100%;">
                                        <HeaderTemplate>Commerciale</HeaderTemplate> 
                                        <ContentTemplate>
                                            <table align="center" class="sfondopagine" style="width:100%; height:445px;">
                                                <tr>
                                                    <td align="center">
                                                        <br />
                                                        <asp:Panel ID="panel4" runat="server" BorderWidth="0px" width="100%" >
                                                            <table width="100%" align="left" class="sfondopagine" style="width:100%; height:100%;">
                                                                <tr>
                                                                    <td>Regime IVA</td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtCodRegimeIVA" runat="server" Width="50px" MaxLength="5" AutoPostBack="true"  BorderStyle="None"/>
                                                                        <asp:Button ID="btnTrovaRegimeIVA" runat="server" Text="?" Width="30px" Height="22px" />
                                                                        <%--<asp:TextBox ID="txtRegimeIva" runat="server" Width="250px" BorderWidth="1px" Enabled="false" />--%>
                                                                        <asp:Label ID="lblRegimeIva" runat="server" Width="455px" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                                                    </td>
                                                                    <td rowspan="2">
                                                                        <asp:CheckBox ID="checkEscudiAllegatiIVA" runat="server" Text="Escludi allegati IVA" Height="15px"/><br />
                                                                        <asp:CheckBox ID="checkIVAInSospensione" runat="server" Text="IVA in sospensione" Height="15px" Visible="false"/><br />
                                                                        <asp:CheckBox ID="checkNonFatturabile" runat="server" Text="Non fatturabile" Height="15px"/>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Pagamento</td>
                                                                    <td colspan="2">
                                                                        <asp:TextBox ID="txtCodPagamento" runat="server" Width="50px" MaxLength="5" AutoPostBack="true"  BorderStyle="None"/>
                                                                        <asp:Button ID="btnTrovaPagamento" runat="server" Text="?" Width="30px" Height="22px" />
                                                                        <%--<asp:TextBox ID="txtPagamento" runat="server" Width="250px" BorderWidth="1px" Enabled="false" /> --%>
                                                                        <asp:Label ID="lblPagamento" runat="server" Width="455px" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td><br /></td>
                                                                    <td colspan="2">
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="3" align="left">
                                                                        <asp:Label ID="Label1" runat="server" Text="Riferimenti bancari del Fornitore" Width="700px" BorderWidth="1px" BorderColor="White" BorderStyle="Groove" Font-Bold="true" ForeColor="Black" ></asp:Label>
                                                                    </td>
                                                                </tr>  
                                                                <tr>
                                                                    <td>Nazione IBAN</td>
                                                                    <td colspan="2">
                                                                        <asp:TextBox ID="txtCodNazioneIBAN" runat="server" Width="50px" AutoPostBack="True" MaxLength="2" BorderStyle="None"/>
                                                                        <asp:Button ID="btnNazioneIBAN" runat="server" Text="?" Width="30px" Height="22px"  BorderStyle="None"/>
                                                                        <asp:Label ID="lblNazioneIBAN" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="245px"></asp:Label>
                                                                        &nbsp;&nbsp;CIN EU <asp:TextBox ID="txtCINEU" runat="server" Width="30px" MaxLength="2"  BorderStyle="None"/>
                                                                        &nbsp;&nbsp;CIN Naz. <asp:TextBox ID="txtCIN" runat="server" Width="30px" MaxLength="1"  BorderStyle="None"/> 
                                                                    </td>                                                         
                                                                </tr>     
                                                                <tr>
                                                                    <td>Codice ABI</td>
                                                                    <td colspan="2">
                                                                        <asp:TextBox ID="txtCodABI" runat="server" Width="50px" MaxLength="5" AutoPostBack="true" BorderStyle="None"/>
                                                                        <asp:Label ID="Label7" runat="server" Text="Codice CAB " ></asp:Label>
                                                                        <asp:TextBox ID="txtCodCAB" runat="server" Width="50px" MaxLength="5" AutoPostBack="true" BorderStyle="None"/>
                                                                        <asp:Label ID="Label9" runat="server" Text="" Width="90px"></asp:Label>
                                                                        <asp:Label ID="Label8" runat="server" Text="Numero C/Corrente "></asp:Label>
                                                                        <asp:TextBox ID="txtNumCC" runat="server" Width="120px" MaxLength="15" BorderStyle="None" /> 
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Banca</td>
                                                                    <td>
                                                                        <asp:Button ID="btnTrovaBanca" runat="server" Text="?" Width="30px" Height="22px" Visible="false" />
                                                                        <%-- %><asp:TextBox ID="txtBanca" runat="server" Width="345px" Enabled="false" BorderWidth="1px" />--%>
                                                                        <asp:Label ID="lblBanca" runat="server" Width="550px" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                                                    </td>
                                                                    <td rowspan="9" align="right">
                                                                        <br />
                                                                        <asp:Panel ID="panel6" runat="server" BorderWidth="1px" width="210px" Height="250px" align="center" Visible="false">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>Giorni di chiusura<br /><br /></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        Primo giorno<br />
                                                                                        <asp:DropDownList ID="ddlPrimoGiorno1" runat="server">
                                                                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                                                                            <asp:ListItem Value="Lunedi" Text="Lunedì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Martedi" Text="Martedì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Mercoledi" Text="Mercoledì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Giovedi" Text="Giovedì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Venerdi" Text="Venerdì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Sabato" Text="Sabato"></asp:ListItem>
                                                                                            <asp:ListItem Value="Domenica" Text="Domenica"></asp:ListItem>
                                                                                        </asp:DropDownList><br />
                                                                                        <asp:CheckBox ID="checkMattino1" runat="server" Text="Mattino" /><br />
                                                                                        <asp:CheckBox ID="checkPomeriggio1" runat="server" Text="Pomeriggio" /><br /><br />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        Secondo giorno<br />
                                                                                        <asp:DropDownList ID="ddlPrimoGiorno2" runat="server">
                                                                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                                                                            <asp:ListItem Value="Lunedi" Text="Lunedì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Martedi" Text="Martedì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Mercoledi" Text="Mercoledì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Giovedi" Text="Giovedì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Venerdi" Text="Venerdì"></asp:ListItem>
                                                                                            <asp:ListItem Value="Sabato" Text="Sabato"></asp:ListItem>
                                                                                            <asp:ListItem Value="Domenica" Text="Domenica"></asp:ListItem>
                                                                                        </asp:DropDownList><br />
                                                                                        <asp:CheckBox ID="checkMattino2" runat="server" Text="Mattino" /><br />
                                                                                        <asp:CheckBox ID="checkPomeriggio2" runat="server" Text="Pomeriggio" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <br />
                                                                        </asp:Panel>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Filiale</td>
                                                                    <td colspan="2">
                                                                        <%--<asp:TextBox ID="txtFiliale" runat="server" Width="345px" Enabled="false" BorderWidth="1px" />--%>
                                                                        <asp:Label ID="lblFiliale" runat="server" Width="550px" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>IBAN</td>
                                                                    <td colspan="2">
                                                                        <asp:Label ID="lblIBANFor" runat="server" Width="550px" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Codice SWIFT</td>
                                                                    <td colspan="2">
                                                                        <asp:TextBox ID="txtSWIFT" runat="server" Width="150px" MaxLength="15" BorderStyle="None" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td><br /></td>
                                                                    <td colspan="2">
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <%--<td>&nbsp;</td>
                                                                    <td colspan="2">--%>
                                                                    <td colspan="3" align="left">
                                                                        <asp:Label ID="Label2" runat="server" Text="Riferimenti bancari Azienda (Disposizione bonifici)" Width="700px" BorderWidth="1px" BorderColor="White" BorderStyle="Groove" Font-Bold="true" ForeColor="Black" ></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td><asp:Label ID="Label3" runat="server">Seleziona Banca</asp:Label>
                                                                    </td>
                                                                    <td colspan="2">
                                                                        <div>
                                                                        <asp:DropDownList ID="DDLBancheIBAN" runat="server" 
                                                                            AppendDataBoundItems="True" AutoPostBack="True" 
                                                                            DataSourceID="SqlDSBancheIBAN" DataTextField="DesCompleta" 
                                                                            DataValueField="IBAN" Height="22px" TabIndex="11" Width="520px">
                                                                            <asp:ListItem ></asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        <asp:Button 
                                                                        ID="btnCercaBanca" runat="server" CausesValidation="False" CommandName="btnCercaBanca" 
                                                                        Text="+" ToolTip="Gestione anagrafiche Banche Azienda"/>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <%--<td>IBAN</td>--%>
                                                                    <td>
                                                                        <div>
                                                                            <asp:Label ID="lblABI" runat="server"  BorderColor="White" 
                                                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black"  Width="45px"></asp:Label>
                                                                            <asp:Label ID="lblCAB" runat="server"  BorderColor="White" 
                                                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black"  Width="45px"></asp:Label>
                                                                        </div>
                                                                    </td>
                                                                    <td colspan="2">
                                                                        <div>
                                                                        <asp:Label ID="Label4" runat="server">IBAN</asp:Label>
                                                                        <asp:Label ID="lblIBAN" runat="server" BorderColor="White" 
                                                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="285px">IBAN</asp:Label>
                                                                        <asp:Label ID="Label5" runat="server">C/Corrente</asp:Label>
                                                                        <asp:Label ID="lblContoCorrente" runat="server" BorderColor="White" 
                                                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="135px">Conto Corrente</asp:Label>
                                                                        </div>
                                                                    </td>
                                                                </tr> 
                                                                <tr>
                                                                    <td><br /></td>
                                                                    <td colspan="2">
                                                                        <br />
                                                                    </td>
                                                                </tr>                                                               
                                                                <tr>
                                                                    <td>Zona</td>
                                                                    <td colspan="2">
                                                                        <asp:TextBox ID="txtCodZona" runat="server" Width="50px" AutoPostBack="true" MaxLength="5" BorderStyle="None" />
                                                                        <asp:Button ID="btnTrovaZona" runat="server" Text="?" Width="30px" Height="22px" />
                                                                        <asp:Button ID="btnZone" runat="server" CausesValidation="False" CommandName="btnZone" 
                                                                            Text="+" ToolTip="Gestione anagrafiche Zone" Width="30px" Height="22px" />                                                                                                                                                
                                                                        <%--<asp:TextBox ID="txtZona" runat="server" Width="250px" Enabled="false" BorderWidth="1px" />--%>
                                                                        <asp:Label ID="lblZona" runat="server" Width="422px" BorderColor="White"
                                                                            BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>                                                                        
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Categoria</td>
                                                                    <td colspan="2">
                                                                        <asp:TextBox ID="txtCodCategoria" runat="server" Width="50px" AutoPostBack="true" MaxLength="5" BorderStyle="None"/>
                                                                        <asp:Button ID="btnTrovaCategoria" runat="server" Text="?" Width="30px" Height="22px" />
                                                                        <asp:Button ID="btnCategorie" runat="server" CausesValidation="False" CommandName="btnCategorie" 
                                                                            Text="+" ToolTip="Gestione anagrafiche Categorie" Width="30px" Height="22px" />                                                                                                                                                                                                                             
                                                                        <%--<asp:TextBox ID="txtCategoria" runat="server" Width="250px" Enabled="false" BorderWidth="1px" />--%>
                                                                        <asp:Label ID="lblCategorie" runat="server" Width="422px" BorderColor="White"
                                                                            BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>                                                                        
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Listino</td>
                                                                    <td colspan="2">
                                                                        <asp:TextBox ID="txtCodListino" runat="server" Width="50px" AutoPostBack="true" MaxLength="5" BorderStyle="None"/>
                                                                        <asp:Button ID="btnTrovaListino" runat="server" Text="?" Width="30px" Height="22px" />
                                                                        <%-- %><asp:TextBox ID="txtListino" runat="server" Width="250px" Enabled="false" BorderWidth="1px" />--%>
                                                                        <asp:Label ID="lblListino" runat="server" Width="455px" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <%--<td>C/Ricavo FT</td>--%>
                                                                    <td></td>
                                                                    <td colspan="2">
                                                                        <asp:TextBox ID="txtCodRicavoFT" runat="server" Width="50px" AutoPostBack="true" Visible="false" MaxLength="16" BorderStyle="None"/>
                                                                        <asp:Button ID="btnTrovaRicavoFT" runat="server" Text="?" Width="30px" Height="22px" Visible="false"/>
                                                                        <asp:TextBox ID="txtRicavoFT" runat="server" Width="250px" Enabled="false" BorderWidth="1px" Visible="false" BorderStyle="None"/>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="3" align="right">
                                                                        <asp:Label ID="Label6" runat="server" Visible="false" BorderColor="White" BorderStyle="Groove" BorderWidth="1px" Font-Bold="true" ForeColor="Black" Text="Massimo credito" Width="155px"></asp:Label>
                                                                        <asp:TextBox ID="txtMaxCredito" Visible="false" runat="server" Width="100px" MaxLength="15" BorderStyle="None"/>
                                                                    </td>
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
                                            <table align="center" class="sfondopagine" style="width:100%; height:445px;">
                                                <tr>
                                                    <td  align="center" >
                                                        <br />
                                                        <asp:Panel ID="panel3" runat="server" BorderWidth="0px" style="width:100%; height:100%;">
                                                            <table  align="center" width="700px" class="sfondopagine" style="width:100%; height:100%;">
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
                                                                    <td colspan="3">
                                                                        <br / >
                                                                    </td>
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
                                                            <wuc:DestMerce ID="DestMerceFornitore" runat="server" />
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </ajaxToolkit:TabPanel>
                                    <ajaxToolkit:TabPanel runat="server" ID="TabPanelN" HeaderText="Note"  Visible="false" style="width:100%; height:100%;">
                                        <HeaderTemplate>Note</HeaderTemplate>
                                        <ContentTemplate>
                                            <table align="center" class="sfondopagine" style="width:100%; height:445px;">
                                                <tr>
                                                    <td align="center">
                                                        <br />
                                                        <asp:Panel ID="panelNote" runat="server" BorderWidth="0px">
                                                            <table>
                                                                <tr>
                                                                    <td style="vertical-align:top">Note</td>
                                                                    <td><asp:TextBox ID="txtNote" runat="server" Width="600px" Height="200px" 
                                                                            TextMode="MultiLine" BorderStyle="None" /></td>
                                                                </tr>
                                                            </table>
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
            <div id="divBottoni" runat="server" >
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
                <asp:Button ID="btnAggiorna" runat="server" Text="Aggiorna"  class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnAnnulla" runat="server" Text="Annulla"  class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnElimina" runat="server" Text="Elimina"  class="btnstyle" />
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnRicerca" runat="server" Text="Ricerca"  class="btnstyle"/>
                </div>
                <div style="height: 15px"></div> 
                <%--<div>
                <input name="ELIMINA" type="button" value="ELIMINA" onclick='submitAuthentication()' />
                </div>--%>
                <div style="height: 15px; text-align:center"><b>Stampe</b></div> 
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnStampa" runat="server" Text="Stampe" class="btnstyle" />
                </div> 
                <div>
                <asp:Button ID="btnStampaElencoSint" runat="server" Text="Sintetico" class="btnstyle" Visible="false" />
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnStampaRubrica" runat="server" Text="Rubrica" class="btnstyle" Visible="false"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnStampaElencoAna" runat="server" Text="Analitico" class="btnstyle" Visible="false"/>
                </div>
                <div style="height: 15px"></div> 
                <div>
                <asp:Button ID="btnStampaElencoCodici" runat="server" Text="Codici" class="btnstyle" Visible="false"/>
                </div>
            </td>
            </div>
        </tr>
    </table>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>