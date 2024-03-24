<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Documenti.ascx.vb" Inherits="SoftAziOnLine.WUC_Documenti" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="WUC_DocumentiDett.ascx" tagname="WUC_DocumentiDett" tagprefix="uc2" %>
<%@ Register src="WUC_DocumentiSpeseTraspTot.ascx" tagname="WUC_DocumentiSpeseTraspTot" tagprefix="uc3" %>
<%@ Register src="../WebUserControl/WFP_AnagrProvv_Insert.ascx" tagname="WFP_AnagrProvv_Insert" tagprefix="uc4" %>
<%@ Register src="../WebUserControl/WFP_BancheIBAN.ascx" tagname="WFP_BancheIBAN" tagprefix="uc7" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc8" %>
<%@ Register src="../WebUserControl/WFP_Anagrafiche_Modify.ascx" tagname="WFP_Anagrafiche_Modify" tagprefix="uc5" %>
<%@ Register src="../WebUserControl/WFP_Agenti.ascx" tagname="WFP_Agenti" tagprefix="uc9" %>
<%@ Register Src="~/WebUserControl/WFP_Elenco.ascx" TagName="WFPElenco" TagPrefix="wuc" %>
<%@ Register src="../WebUserControl/WFP_TipoFatturazione.ascx" tagname="WFP_TipoFatturazione" tagprefix="uc12" %>
<%@ Register src="../WebUserControl/WFP_EtichettePrepara.ascx" TagName="WFPETP" TagPrefix="wfp1" %>
<%@ Register src="../WebUserControl/WFP_ElencoDestCF.ascx" tagname="WFP_ElencoDestCF" tagprefix="uc15" %>
<%@ Register Src="../WebUserControl/WFP_DestCliFor.ascx" TagName="WFP_DestCliFor" TagPrefix="uc16" %>
<%@ Register src="../WebUserControl/WFP_LeadSource.ascx" tagname="WFP_LeadSource" tagprefix="uc17" %>
<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .btnstyle
        {
            Width: 108px;
            height: 40px;
        margin-left: 0px;
        white-space: pre-wrap;
        }   
        .btnstyle1R
        {
            Width: 108px;
            height: 30px;
        margin-left: 0px;
        white-space: pre-wrap;
        }
        .styleTDBTN
        {
            height: 478px;
        }
         .styleTxtCodTB0
        {
            height: 25px;
            }  
        .styleTBPagina
        {
            height: 480px;
            width: 1110px;
        }
        .styleLblTB0
        {
            height: 25px;
            width: 120px;
            }
        .styleDest
        {
            height: 60px;
            width: 120px;
        }
        .styleNumDoc
        {
            height: 25px;
            width: 120px;
            }
        </style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc4:WFP_AnagrProvv_Insert ID="WFP_AnagrProvv_Insert1" runat="server" />
        <uc8:WFP_ElencoCliForn ID="WFPElencoCli" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
        <uc8:WFP_ElencoCliForn ID="WFPElencoFor" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori" />
        <uc7:WFP_BancheIBAN ID="WFP_BancheIBAN1" runat="server" />
        <uc5:WFP_Anagrafiche_Modify ID="WFP_Anagrafiche_Modify1" runat="server" />
        <uc16:WFP_DestCliFor ID="WFP_DestCliFor1" runat="server" />
        <uc9:WFP_Agenti ID="WFP_Agenti1" runat="server" />
        <uc12:WFP_TipoFatturazione ID="TipoFatturazione" runat="server" />
        <uc15:WFP_ElencoDestCF ID="WFPElencoDestCF" runat="server" Elenco="ListaDestCliFor" Titolo="Elenco Destinazioni" />
        <wuc:WFPElenco ID="WFPElencoAliquotaIVA" runat="server" Tabella="AliquoteIva" Titolo="Elenco Aliquote IVA"/>
        <wfp1:WFPETP ID="WFPETP" runat="server" />
        <uc17:WFP_LeadSource ID="WFP_LeadSource1" runat="server" />
        <asp:SqlDataSource ID="SqlDSCliForFilProvv" runat="server" 
            SelectCommand="SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)">
            <SelectParameters>
                <asp:SessionParameter Name="Codice" SessionField="Codice_CoGe" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSPagamenti" runat="server" 
            SelectCommand="SELECT * FROM [Pagamenti] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSLead" runat="server" 
            SelectCommand="SELECT * FROM [LeadSource] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSBancheIBAN" runat="server" 
            SelectCommand="SELECT *, Descrizione + ' - ' + IBAN AS DesCompleta FROM [BancheIBAN] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSAgenti" runat="server" 
            SelectCommand="SELECT *, RTRIM(CAST(Codice AS NVARCHAR(10))) + ' - ' + Descrizione AS CodDes FROM [Agenti] ORDER BY [Codice]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSCausMag" runat="server" 
            SelectCommand="SELECT * FROM [CausMag] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSListini" runat="server" 
            SelectCommand="SELECT * FROM [ListVenT] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSValuta" runat="server" 
            SelectCommand="SELECT * FROM [Valute] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSTipoFatt" runat="server" 
            SelectCommand="SELECT * FROM [TipoFatt] WHERE ISNULL(Descrizione,'') <>'' ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
               SelectCommand="SELECT Codice, Descrizione FROM Magazzini WHERE Codice<>0 ORDER BY Descrizione">
        </asp:SqlDataSource>
        <table style="width:auto; height:auto;">
            <tr>
                <td class="styleTBPagina">
                    <asp:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" 
                        BorderStyle="Groove" Height="500px" style="margin-top: 0px" 
                        Width="1110px">
                        <asp:TabPanel ID="Intestazione" runat="server" HeaderText="Intestazione">
                            <HeaderTemplate>
                                Intestazione
                            </HeaderTemplate>
                            <ContentTemplate>
                                <table bgcolor="Silver" style="width:1090px; height:500px;">
                                    <tr>
                                        <td class="styleLblTB0">
                                            <div>
                                            <asp:Label ID="lblNumeroDoc" runat="server">Numero</asp:Label>
                                            </div>
                                            <div>Data documento</div>
                                        </td>
                                        <td>
                                            <div>
                                                <asp:TextBox ID="txtNumero" runat="server" AutoPostBack="True" MaxLength="10" Width="70px" TabIndex="1" BorderStyle="None"></asp:TextBox>
                                                <asp:TextBox ID="txtRevNDoc" runat="server" AutoPostBack="True" MaxLength="3" Width="30px" BorderStyle="None"></asp:TextBox></div>
                                            <div>
                                            <asp:TextBox ID="txtDataDoc" runat="server" MaxLength="10" Width="70px" TabIndex="2" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendar" runat="server" CausesValidation="False" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataDoc_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar" 
                                                    TargetControlID="txtDataDoc">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="txtDataDoc" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                            </div>
                                        </td>
                                        <td>
                                            <div id="noradio" style="width:750px">
                                                <asp:Label ID="lblLabelDataVal" runat="server">Data/GG.Validitá</asp:Label>
                                                <asp:TextBox ID="txtDataValidita" runat="server" MaxLength="10" TabIndex="2" Width="70px" BorderStyle="None"></asp:TextBox>
                                                <asp:Label ID="lblNGG_Validita" runat="server"></asp:Label>
                                                <asp:TextBox ID="txtNGG_Validita" runat="server" MaxLength="3" Width="30px" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendarDV" runat="server" CausesValidation="False" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataValidita_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDV" 
                                                    TargetControlID="txtDataValidita">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                                    ControlToValidate="txtDataValidita" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                <asp:Label ID="lblDataCons" runat="server">Data/GG.Consegna</asp:Label>
                                                <asp:TextBox ID="txtDataConsegna" runat="server" MaxLength="10" TabIndex="1"
                                                    Width="70px" BorderStyle="None"></asp:TextBox>
                                                <asp:Label ID="lblNGG_Consegna" runat="server"></asp:Label>
                                                <asp:TextBox ID="txtNGG_Consegna" runat="server" MaxLength="3" Width="30px" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendarDC" runat="server" CausesValidation="False" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True"
                                                    Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDC"
                                                    TargetControlID="txtDataConsegna">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server"
                                                    ControlToValidate="txtDataConsegna" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                            </div>
                                            <div style="width:750px" >
                                             <asp:Label ID="lblCIGCUP" runat="server" Text="CIG/CUP"></asp:Label>
                                             <asp:TextBox ID="txtCIG" runat="server"
                                                MaxLength="10" TabIndex="3" Width="90px"  BorderStyle="None"></asp:TextBox>
                                             <asp:TextBox ID="txtCUP" runat="server"
                                                MaxLength="15" TabIndex="3" Width="140px" BorderStyle="None"></asp:TextBox>
                                             <asp:Label ID="Label9" runat="server" Width="5px"></asp:Label>
                                             <asp:Label ID="Label5" runat="server" Font-Bold="True">Tipo Evasione</asp:Label>
                                             <asp:RadioButton ID="optTipoEvTotaleTotale" ValidationGroup="0" GroupName="GrpTipoEvTotale" runat="server" Text ="Totale" AutoPostBack ="false"></asp:RadioButton>
                                             <asp:RadioButton ID="optTipoEvTotaleParziale" ValidationGroup="0" GroupName="GrpTipoEvTotale" runat="server" Text ="Parziale" AutoPostBack ="false"></asp:RadioButton>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">Riferimento</td>
                                        <td colspan="2" style="width:950px">
                                            <asp:TextBox ID="txtRiferimento" runat="server"
                                                MaxLength="20" TabIndex="3" Width="170px" BorderStyle="None"></asp:TextBox>
                                            <%--<asp:Label ID="Label10" runat="server" Text="" Width="05px"></asp:Label>--%>
                                            <asp:Label ID="Label6" runat="server" Text="Data riferimento"></asp:Label>
                                            <asp:TextBox ID="txtDataRif" runat="server" MaxLength="10" TabIndex="3"
                                                Width="70px" BorderStyle="None"></asp:TextBox>
                                            <asp:ImageButton ID="imgBtnShowCalendar0" runat="server" CausesValidation="False" 
                                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                ToolTip="apri il calendario" />
                                            <asp:CalendarExtender ID="txtDataRif_CalendarExtender" runat="server"
                                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar0"
                                                TargetControlID="txtDataRif">
                                            </asp:CalendarExtender>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                ControlToValidate="txtDataRif" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                            <%--<asp:Label ID="Label7" runat="server" Text="" Width="5px"></asp:Label>--%>
                                            <asp:Label ID="Label8" runat="server" Text="Tipo rapporto "></asp:Label>
                                            <asp:DropDownList ID="DDLTipoRapp" runat="server" 
                                                AppendDataBoundItems="True" AutoPostBack="false" Height="22px" TabIndex="2" Width="140px">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Ordine di acquisto" Value="OA"></asp:ListItem>
                                                <asp:ListItem Text="Dati Contratto" Value="DC"></asp:ListItem>
                                                <asp:ListItem Text="Dati Convenzione" Value="DV"></asp:ListItem>
                                            </asp:DropDownList>
                                            <%--<asp:Label ID="Label11" runat="server" Text="" Width="5px"></asp:Label>--%>
                                            <asp:Label ID="lblCCommessa" runat="server" Text="C.Commessa"></asp:Label>
                                            <asp:TextBox ID="txtCCommessa" runat="server"
                                                MaxLength="100" TabIndex="3" Width="200px" BorderStyle="None"></asp:TextBox>                                                
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">
                                        <div>
                                            <asp:Label ID="lblLabelCliForFilProvv" runat="server">Fornitore</asp:Label>
                                        </div>
                                        <div>
                                            <asp:Button 
                                                ID="btnCercaAnagrafica" runat="server" CommandName="btnCercaAnagrafica" 
                                                Text="?" ToolTip="Ricerca anagrafiche" />
                                            <asp:Button
                                                ID="btnModificaAnagrafica" runat="server" CommandName="btnModificaAnagrafica"
                                                Text="M" ToolTip="Modifica dati anagrafici" />
                                            <asp:Button 
                                                ID="btnInsAnagrProvv" runat="server" CommandName="btnInsAnagrProvv" 
                                                Text="+" ToolTip="Anagrafica provvisoria" />
                                        </div>
                                        </td>
                                        <td class="styleBordato" colspan="2">
                                            <div style="width:950px">
                                            <asp:TextBox ID="txtCodCliForFilProvv" runat="server" AutoPostBack="True" 
                                                MaxLength="16" TabIndex="5" Width="100px" BorderStyle="None"></asp:TextBox>
                                            <asp:Label ID="lblCliForFilProvv" runat="server" BorderColor="White" 
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="545px">Ragione Sociale</asp:Label>
                                            &nbsp;<asp:Label ID="lblLabelPICF" runat="server" Width="100px"> Partita IVA </asp:Label>&nbsp;<asp:Label ID="lblPICF" runat="server" Width="150px" BorderColor="White"
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                            </div>
                                            <div style="width:950px;">
                                                <asp:Label ID="lblIndirizzo" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="492px">INDIRIZZO</asp:Label>
                                                <asp:Label ID="lblLocalita" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="420px">LOCALITA</asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleDest">
                                            <div>
                                                <asp:Label ID="Label13" runat="server" Width="70px">Destinazione</asp:Label>
                                            </div>
                                            <div>
                                                <asp:Button 
                                                    ID="btnCercaDest" runat="server" CommandName="btnCercaDest" 
                                                    Text="?" ToolTip="Ricerca Destinazione" />
                                                <asp:Button
                                                    ID="btnModificaDest" runat="server" CommandName="btnModificaDest"
                                                    Text="+ M" ToolTip="Inserimento/Modifica dati Destinazione" />
                                                <asp:Button 
                                                    ID="btnInsDest" runat="server" CommandName="btnInsDest" 
                                                    Text="+" ToolTip="Nuova Destinazione" Visible="false"/>
                                                <asp:Button ID="btnDelDest" runat="server" CommandName="btnDelDest" 
                                                    Text="X" ToolTip="Nessuna Destinazione" />
                                            </div>
                                        </td>
                                        <td colspan="2" class="styleBordato">
                                            <div style="width: 950px">
                                               <asp:Label ID="Label12" runat="server">Selezionata</asp:Label>
                                               <asp:Label ID="lblDestSel" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="413px">SELEZIONATA</asp:Label>
                                                <asp:Label ID="lblDenominazioneD" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="420px" ToolTip="DENOMINAZIONE">DENOMINAZIONE</asp:Label>
                                            </div>
                                            <div style="width: 950px;height:21px">
                                                <asp:TextBox ID="txtDestinazione1" runat="server" 
                                                    MaxLength="150" Width="495px" BorderStyle="None" BorderColor="Black"></asp:TextBox>
                                                 <asp:Label ID="lblRiferimentoD" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="418px" ToolTip="RIFERIMENTO">RIFERIMENTO</asp:Label>
                                            </div>
                                            <div style="width: 950px;height:22px">
                                                <asp:TextBox ID="txtDestinazione2" runat="server" 
                                                    MaxLength="150" Width="495px" BorderStyle="None" BorderColor="Black"></asp:TextBox>
                                                 <asp:Label ID="lblTel1D" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="135px" ToolTip="TEL1">TEL1</asp:Label>
                                                  <asp:Label ID="lblTel2D" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="135px" ToolTip="TEL2">TEL2</asp:Label>
                                                  <asp:Label ID="lblFaxD" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="130px" ToolTip="FAX">FAX</asp:Label>
                                            </div>
                                            <div style="width: 950px;height:22px">
                                                <asp:TextBox ID="txtDestinazione3" runat="server" 
                                                    MaxLength="150" Width="495px" BorderStyle="None" BorderColor="Black"></asp:TextBox>
                                                <asp:Label ID="lblEMailD" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="418px" ToolTip="EMAIL">EMAIL</asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">
                                            Pagamento</td>
                                        <td colspan="2">
                                            <div style="width:950px">
                                                <asp:TextBox ID="txtPagamento" runat="server" AutoPostBack="True" MaxLength="5" 
                                                    TabIndex="6" Width="50px" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLPagamento" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSPagamenti" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" TabIndex="7" Width="442px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="lblLead" runat="server" Font-Bold="true" ForeColor="Blue">Lead Source</asp:Label>
                                                <asp:Button ID="btnLead" runat="server" CommandName="btnLead" 
                                                    Text="?+M" ToolTip="Seleziona/Gestione Lead Source"/>
                                                <asp:DropDownList ID="DDLLead" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="false" DataSourceID="SqlDSLead" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" TabIndex="7" Width="290px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">
                                            <div>Riferimenti Bancari</div>
                                            <div>
                                            <asp:Button 
                                                ID="btnCercaBanca" runat="server" CommandName="btnCercaBanca" 
                                                Text="+" ToolTip="Gestione anagrafiche Banca"/>
                                            </div>
                                        </td>
                                        <td colspan="2" class="styleBordato">
                                            <div style="width:950px">
                                                <asp:Label ID="Label2" runat="server">Seleziona da elenco</asp:Label>
                                                <asp:DropDownList ID="DDLBancheIBAN" runat="server" 
                                                    AppendDataBoundItems="True" AutoPostBack="True" 
                                                    DataSourceID="SqlDSBancheIBAN" DataTextField="DesCompleta" 
                                                    DataValueField="IBAN" Height="22px" TabIndex="11" Width="800px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div style="width:950px">
                                                <asp:Label ID="lblABI" runat="server"  BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black"  Width="50px"></asp:Label>
                                                <asp:Label ID="lblCAB" runat="server"  BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black"  Width="50px"></asp:Label>
                                                <asp:Label ID="lblBanca" runat="server" BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="400px">BANCA</asp:Label>
                                                <asp:Label ID="lblFiliale" runat="server" BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="400px">FILIALE</asp:Label>
                                            </div>
                                            <div style="width:950px">
                                            <asp:Label ID="Label3" runat="server" Width="115px">IBAN</asp:Label>
                                            <asp:Label ID="lblIBAN" runat="server" BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="295px">IBAN</asp:Label>
                                            <asp:Label ID="Label4" runat="server">Conto Corrente</asp:Label>
                                            <asp:Label ID="lblContoCorrente" runat="server" BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="140px">Conto Corrente</asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">
                                            <div>Agente
                                                <asp:Button ID="btnAgenti" runat="server" CommandName="btnAgenti" 
                                                    Text="+" ToolTip="Gestione anagrafiche Agenti"/>
                                            </div>
                                        </td>
                                        <td colspan="2">
                                            <div style="width:950px">
                                                <asp:TextBox ID="txtCodAgente" runat="server" AutoPostBack="True" MaxLength="5" 
                                                    TabIndex="6" Width="50px" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLAgenti" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSAgenti" DataTextField="CodDes" 
                                                    DataValueField="Codice" Height="22px" TabIndex="7" Width="365px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="lblMessAge" runat="server" Width="250px" Text=""
                                                    BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black" ></asp:Label>
                                                <asp:Label ID="lblMagazzino" runat="server" Height="16px" Font-Bold="true" ForeColor="Blue">Magazzino</asp:Label>
                                                <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"  
                                                       AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                                                       DataTextField="Descrizione" 
                                                       DataValueField="Codice" Width="150px">
                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">
                                            Causale</td>
                                        <td colspan="2">
                                            <div style="width:950px">
                                                <asp:TextBox ID="txtCodCausale" runat="server" AutoPostBack="True" 
                                                    MaxLength="5" TabIndex="6" Width="50px" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLCausali" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSCausMag" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" TabIndex="7" Width="365px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="lblMagazzino2" runat="server" Height="16px" Font-Bold="true" ForeColor="Blue" Visible="false">Magazzino(2)</asp:Label>
                                                <asp:DropDownList ID="DDLMagazzino2" runat="server" Visible="false" AppendDataBoundItems="true"  
                                                       AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                                                       DataTextField="Descrizione" 
                                                       DataValueField="Codice" Width="145px">
                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="lblCausale2" runat="server" Height="16px" Font-Bold="true" ForeColor="Blue" Visible="false">Causale(2)</asp:Label>
                                                <asp:DropDownList ID="DDLCausali2" runat="server" Visible="false" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSCausMag" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" TabIndex="7" Width="150px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">
                                            Listino</td>
                                        <td colspan="2">
                                            <div style="width:950px">
                                                <asp:TextBox ID="txtListino" runat="server" AutoPostBack="True" MaxLength="5" 
                                                    TabIndex="6" Width="50px" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLListini" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSListini" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" TabIndex="7" Width="365px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label1" runat="server">Valuta</asp:Label>
                                                <asp:Label ID="lblCodValuta" runat="server" BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Visible="False"></asp:Label>
                                                <asp:Label ID="lblDesValuta" runat="server" BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="60px"></asp:Label>
                                                <asp:Label ID="Label14" runat="server" Width="01px"></asp:Label> 
                                                <asp:CheckBox ID="checkADeposito" runat="server" Font-Bold="true" Text="C/Deposito" Checked="false" AutoPostBack="false"/>                                                   
                                                <asp:CheckBox ID="ChkAcconto" runat="server" Font-Bold="true" Text="Fattura per Acconto/Saldo" Checked="false" AutoPostBack="true"/>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">
                                            <div>Fatturazione
                                                <asp:Button ID="btnTipofatt" runat="server" CommandName="btnTipofatt" 
                                                    Text="+" ToolTip="Gestione Tipo Fatturazione"/>
                                            </div>
                                        </td>
                                        <td colspan="2">
                                            <div style="width:950px">
                                                <asp:TextBox ID="txtTipoFatt" runat="server" AutoPostBack="True" MaxLength="5" 
                                                    TabIndex="6" Width="50px" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLTipoFatt" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSTipoFatt" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" TabIndex="7" Width="365px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:CheckBox ID="chkFatturaPA" runat="server" Font-Bold="false" Text="Fattura/NC PA" Checked="false" AutoPostBack="true"/>
                                                <asp:Label ID="Label15" runat="server" Width="01px"></asp:Label> 
                                                <asp:CheckBox ID="chkFatturaAC" runat="server" Font-Bold="false" Text="Fattura Accompagnatoria" Checked="false" AutoPostBack="true"/>
                                                <asp:CheckBox ID="chkScGiacenza" runat="server" Font-Bold="false" Text="Scarica giacenze" Checked="false" AutoPostBack="false"/>
                                                <asp:Label ID="lblPagAntEff" runat="server" Font-Bold="True" ForeColor="Blue" Visible="false">Pagamento</asp:Label>
                                                <asp:RadioButton ID="optPagAnticipato" ValidationGroup="0" GroupName="GrpPagAntEff" runat="server" Text ="Anticipato" AutoPostBack ="true" Visible="false"></asp:RadioButton>
                                                <asp:RadioButton ID="optPagEffettuato" ValidationGroup="0" GroupName="GrpPagAntEff" runat="server" Text ="Effettuato" AutoPostBack ="true" Visible="false"></asp:RadioButton>
                                                <asp:RadioButton ID="optPagLista" ValidationGroup="0" GroupName="GrpPagAntEff" runat="server" Text ="Lista" AutoPostBack ="true" Visible="false"></asp:RadioButton>
                                                <asp:RadioButton ID="optPagSconosciuto" ValidationGroup="0" GroupName="GrpPagAntEff" runat="server" Text ="*" AutoPostBack ="true" Visible="false" Checked="true"></asp:RadioButton>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">Ns Riferimento</td>
                                        <td colspan="2" style="width:950px">
                                             <asp:TextBox ID="txtDesRefInt" runat="server"
                                                MaxLength="150" TabIndex="3" Width="900px" BorderStyle="None"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0" colspan="1">
                                            Note</td>
                                        <td colspan="2" style="width:950px">
                                            <asp:TextBox ID="txtNoteDocumento" runat="server" TabIndex="12"
                                                Width="900px" TextMode="MultiLine"  BorderStyle="None"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:TabPanel>
                        <asp:TabPanel ID="Dettaglio" runat="server" HeaderText="Dettaglio" >
                            <ContentTemplate>
                                <uc2:WUC_DocumentiDett ID="WUC_DocumentiDett1" runat="server"  />
                            </ContentTemplate>
                        </asp:TabPanel>
                        <asp:TabPanel ID="SpeseTraspTotale" runat="server" 
                            HeaderText="Sconti - Spese - Trasporto - Totale documento - Rit.Acconto">
                            <ContentTemplate>
                                <uc3:WUC_DocumentiSpeseTraspTot ID="WUC_DocumentiSpeseTraspTot1" 
                                    runat="server" />
                            </ContentTemplate>
                        </asp:TabPanel>
                    </asp:TabContainer>
                </td>
                <td align="left" class="styleTDBTN">
                    <asp:UpdatePanel ID="UpdatePanelBTN" runat="server">
                        <ContentTemplate>
                            <div>
                                <a ID="LnkSCEC" runat="server" href="..\WebFormTables\WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti (Scheda CoGe/E.C.)" target="_blank" onclick="return openUrl(this.href);">(Scheda CG/EC)</a>
                            </div>
                            <div>
                                <asp:LinkButton ID="LnkRegimeIVA" runat="server" OnClick="LnkAltriDatiDoc_Click" Text="Regime IVA" ToolTip="NOTA: Se presente un Regime IVA valido, questo sarà assegnato a tutte le voci del dettaglio del documento."></asp:LinkButton>
                            </div>
                            <div>
                                <asp:LinkButton ID="LnkDatiDocMM" runat="server" OnClick="LnkAltriDatiDoc_Click" Text="Dati Doc.MM"></asp:LinkButton>
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnRitorno" runat="server" class="btnstyle" Text="Menu precedente" OnClick="LnkRitorno_Click"/>
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyle1R" Text="Nuovo" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle1R" Text="Modifica" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnAggiorna" runat="server" class="btnstyle1R" Text="Aggiorna" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnAnnulla" runat="server" CausesValidation="False" class="btnstyle1R" Text="Annulla" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyle1R" Text="Elimina" />
                            </div>
                            <div style="height: 15px; text-align:center"><%--<b>Stampe</b>--%>
                                <asp:Label ID="lblStampe" runat="server" BorderColor="White"
                                BorderStyle="None" Font-Bold="True" ForeColor="Black">Stampe</asp:Label>
                            </div> 
                            <div style="height: 8px"></div>
                            <div>
                                <asp:Button ID="btnStampa" runat="server" class="btnstyle1R" Text="Documento" />
                            </div>
                            <div style="height: 15px"></div>
                            <div>
                                <a ID="LnkStampa" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Documento" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Documento</a>
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnConfOrdine" runat="server" class="btnstyle" Text="Conferma" visible="false"  />
                            </div>
                            <div style="height: 15px"></div>
                            <div>
                                <a ID="LnkConfOrdine" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Conferma Ordine" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Conferma Ordine</a>
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnListaCarico" runat="server" class="btnstyle" Text="Lista di carico" visible="false" />
                            </div>
                            <div style="height: 15px"></div>
                            <div>
                                <a ID="LnkListaCarico" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Lista di carico" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Lista di carico</a>
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnEtichette" class="btnstyle" runat="server" Text="Etichette" visible="false" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnCaricoLotti" runat="server" Text="Lettore Lotti" ToolTip="Carico lotti con lettore" class="btnstyle" Visible="false" />
                            </div>
                            <div style="height: 15px"></div>
                            <div style="height: 15px"></div>
                            <div style="height: 15px"></div>
                            <div style="height: 15px"></div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>