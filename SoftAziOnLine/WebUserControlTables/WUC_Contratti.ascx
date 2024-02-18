<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Contratti.ascx.vb" Inherits="SoftAziOnLine.WUC_Contratti" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="WUC_ContrattiDett.ascx" tagname="WUC_ContrattiDett" tagprefix="uc2" %>
<%@ Register src="WUC_ContrattiSpeseTraspTot.ascx" tagname="WUC_ContrattiSpeseTraspTot" tagprefix="uc3" %>
<%@ Register src="../WebUserControl/WFP_AnagrProvv_Insert.ascx" tagname="WFP_AnagrProvv_Insert" tagprefix="uc4" %>
<%@ Register src="../WebUserControl/WFP_BancheIBAN.ascx" tagname="WFP_BancheIBAN" tagprefix="uc7" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc8" %>
<%@ Register src="../WebUserControl/WFP_Agenti.ascx" tagname="WFP_Agenti" tagprefix="uc9" %>
<%@ Register src="../WebUserControl/WFP_Anagrafiche_Modify.ascx" tagname="WFP_Anagrafiche_Modify" tagprefix="uc5" %>
<%@ Register Src="~/WebUserControl/WFP_Elenco.ascx" TagName="WFPElenco" TagPrefix="wuc" %>
<%@ Register src="../WebUserControl/WFP_RespArea.ascx" tagname="WFP_RespArea" tagprefix="uc13" %>
<%@ Register src="../WebUserControl/WFP_RespVisite.ascx" tagname="WFP_RespVisite" tagprefix="uc14" %>
<%@ Register src="../WebUserControl/WFP_ElencoDestCF.ascx" tagname="WFP_ElencoDestCF" tagprefix="uc15" %>
<%@ Register Src="../WebUserControl/WFP_DestCliFor.ascx" TagName="WFP_DestCliFor" TagPrefix="uc16" %>
<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .btnstyle
        {
            Width: 108px;
            height: 40px;
        margin-left: 0px;
        white-space:pre-wrap;
        }
        .btnstyle2R
        {
            Width: 108px;
            height: 45px;
        margin-left: 0px;
        white-space: pre-wrap;
        }      
        .btnstyle1R
        {
            Width: 108px;
            height: 30px;
        margin-left: 0px;
        }
         .btnstyle05R
        {
            Width: 108px;
            height: 20px;
        margin-left: 0px;
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
            height: 650px;
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
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="685px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc4:WFP_AnagrProvv_Insert ID="WFP_AnagrProvv_Insert1" runat="server" />
        <uc8:WFP_ElencoCliForn ID="WFPElencoCli" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
        <uc8:WFP_ElencoCliForn ID="WFPElencoFor" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori" />
        <uc7:WFP_BancheIBAN ID="WFP_BancheIBAN1" runat="server" />
        <uc5:WFP_Anagrafiche_Modify ID="WFP_Anagrafiche_Modify1" runat="server" />
        <uc16:WFP_DestCliFor ID="WFP_DestCliFor1" runat="server" />
        <uc9:WFP_Agenti ID="WFP_Agenti1" runat="server" />
        <uc13:WFP_RespArea ID="WFP_RespArea1" runat="server" />
        <uc14:WFP_RespVisite ID="WFP_RespVisite1" runat="server" />
        <uc15:WFP_ElencoDestCF ID="WFPElencoDestCF" runat="server" Elenco="ListaDestCliFor" Titolo="Elenco Destinazioni" />
        <wuc:WFPElenco ID="WFPElencoAliquotaIVA" runat="server" Tabella="AliquoteIva" Titolo="Elenco Aliquote IVA"/>
        <asp:SqlDataSource ID="SqlDSCliForFilProvv" runat="server" 
            SelectCommand="SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)">
            <SelectParameters>
                <asp:SessionParameter Name="Codice" SessionField="Codice_CoGe" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSPagamenti" runat="server" 
            SelectCommand="SELECT TipoContratto.Codice, TipoContratto.Descrizione, TipoContratto.TipoPagamento, TipoContratto.TipoScadenza, TipoContratto.FineMese, 
                           TipoContratto.MeseCS, TipoContratto.GiornoFisso, TipoContratto.Cod_Causale, 
                           CausMag.Descrizione + ' - ' + TipoContratto.Descrizione AS DesTipoCaus 
                           FROM TipoContratto INNER JOIN CausMag ON TipoContratto.Cod_Causale = CausMag.Codice ORDER BY CausMag.Descrizione, TipoContratto.Descrizione">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSBancheIBAN" runat="server" 
            SelectCommand="SELECT *, Descrizione + ' - ' + IBAN AS DesCompleta FROM [BancheIBAN] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSAgenti" runat="server" 
            SelectCommand="SELECT *, RTRIM(CAST(Codice AS NVARCHAR(10))) + ' - ' + Descrizione AS CodDes FROM [Agenti] ORDER BY [Codice]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSRespArea" runat="server" 
            SelectCommand="SELECT *, RTRIM(CAST(Codice AS NVARCHAR(10))) + ' - ' + Descrizione AS CodDes FROM [RespArea] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSRespVisite" runat="server" 
            SelectCommand="SELECT *, RTRIM(CAST(Codice AS NVARCHAR(10))) + ' - ' + Descrizione AS CodDes FROM [RespVisite] WHERE ([CodRespArea] = @IDRespArea) ORDER BY [Descrizione]">
            <SelectParameters>
                <asp:SessionParameter Name="IDRespArea" SessionField="IDRespArea" Type="String" />
            </SelectParameters>
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
            SelectCommand="SELECT * FROM [DurataTipoFatt] ORDER BY [DescrizioneF]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSDurataTipo" runat="server" 
            SelectCommand="SELECT * FROM [DurataTipoFatt] WHERE Codice<>'U' ORDER BY [Descrizione]">
        </asp:SqlDataSource>
         <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
               SelectCommand="SELECT Codice, Descrizione FROM Magazzini WHERE Codice<>0 ORDER BY Descrizione">
        </asp:SqlDataSource>
        <table style="width:auto; height:auto;">
            <tr>
                <td class="styleTBPagina">
                    <asp:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" BorderStyle="Groove" Height="650px" style="margin-top: 0px" Width="1110px">
                        <asp:TabPanel ID="Intestazione" runat="server" HeaderText="Dati: Cliente / Contratto">
                            <HeaderTemplate>
                                Dati: Cliente / Contratto
                            </HeaderTemplate>
                            <ContentTemplate>
                                <table class="sfondopagine" style="width:1090px; height:650px;">
                                    <tr>
                                        <td class="styleLblTB0">
                                            <div>
                                                <asp:Label ID="lblNumeroDoc" runat="server">Numero</asp:Label>
                                            </div>
                                            <div>Data documento</div>
                                            <div>
                                                <asp:Button ID="btnDaOrdine" runat="server" CssClass="btnstyle2R" Text="Carica dati da Ordine" Visible="false"/>
                                                <asp:Button ID="btnCollegaOC" runat="server" CssClass="btnstyle2R" Text="Collega N° Ordine" Visible="false"/>
                                            </div>
                                        </td>
                                        <td>
                                            <div style="width:170px;">
                                                <asp:TextBox ID="txtNumero" runat="server" AutoPostBack="True" MaxLength="10" Width="70px" TabIndex="1" BorderStyle="None"></asp:TextBox>
                                                <%--<asp:Label ID="Label12" runat="server">Rev.N°</asp:Label>
                                                <asp:TextBox ID="txtRevNDoc" runat="server" AutoPostBack="true" MaxLength="3" Width="30px" BorderStyle="None"></asp:TextBox>--%>
                                            </div>
                                            <div style="width:170px;">
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
                                            <div style="width:170px;">
                                                <asp:Label ID="lblNOrdine" runat="server" ForeColor="DarkRed" Font-Bold="true" Visible="false">Ordine da cui caricare i dati:</asp:Label>
                                                <asp:TextBox ID="txtNOrdine" runat="server" AutoPostBack="True" MaxLength="10" Width="70px" TabIndex="1" Visible="false" BorderStyle="None"></asp:TextBox>
                                                <asp:TextBox ID="txtNOrdineRev" runat="server" AutoPostBack="True" MaxLength="3" Width="30px" Visible="false" BorderStyle="None"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td class="styleBordato" title="Dati Contratto">
                                            <div style="width:100%;height:27px">
                                                <asp:Label ID="lblDurataNum" runat="server">Durata N°</asp:Label>
                                                <%--<asp:Label ID="Label21" runat="server" Width="1px"></asp:Label>--%>
                                                <asp:TextBox ID="txtDurataNum" runat="server" AutoPostBack="true" MaxLength="3" Width="34px" TabIndex="3" BorderStyle="None"></asp:TextBox>
                                                <%--<asp:Label ID="Label14" runat="server">Tipo durata</asp:Label>--%>
                                                <asp:DropDownList ID="DDLDurataTipo" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="true" DataSourceID="SqlDSDurataTipo" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" Width="95px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label19" runat="server" Width="15px"></asp:Label>
                                                <asp:Label ID="Label18" runat="server">N° Visite per periodo</asp:Label>
                                                <%--<asp:Label ID="Label11" runat="server" Width="10px"></asp:Label>--%>
                                                <asp:TextBox ID="txtNVisite" runat="server" AutoPostBack="false" MaxLength="2" Width="20px" BorderStyle="None"></asp:TextBox>
                                                <asp:Label ID="lblNumRate" runat="server" Width="200px" BorderColor="White"
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                                                                               
                                                <asp:Label ID="lblLabelDataVal" runat="server" visible="false">Data/Giorni Validità</asp:Label>
                                                <asp:TextBox ID="txtDataValidita" runat="server" MaxLength="10" Width="70px" visible="false" BorderStyle="None"></asp:TextBox>
                                                <asp:Label ID="lblNGG_Validita" runat="server" visible="false"></asp:Label>
                                                <asp:TextBox ID="txtNGG_Validita" runat="server" MaxLength="3" Width="30px" visible="false" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendarDV" runat="server" visible="false" CausesValidation="False"  
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataValidita_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDV" 
                                                    TargetControlID="txtDataValidita">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                                    ControlToValidate="txtDataValidita" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                
                                                <asp:Label ID="Label9" runat="server" Width="10px" Visible="false"></asp:Label>
                                                
                                                <asp:Label ID="lblDataCons" runat="server" Visible="false">Data/GG.Consegna</asp:Label>
                                                <asp:TextBox ID="txtDataConsegna" runat="server" MaxLength="10" Width="70px" Visible="false" BorderStyle="None"></asp:TextBox>
                                                <asp:Label ID="lblNGG_Consegna" runat="server" Visible="false"></asp:Label>
                                                <asp:TextBox ID="txtNGG_Consegna" runat="server" MaxLength="3" Width="30px" Visible="false" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendarDC" runat="server" CausesValidation="False" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                    ToolTip="apri il calendario"  Visible="false" />
                                                <asp:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True"
                                                    Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDC"
                                                    TargetControlID="txtDataConsegna">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server"
                                                    ControlToValidate="txtDataConsegna" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                            </div>
                                            <div  id="noradio" style="width:100%;height:27px">
                                                <asp:Label ID="Label13" runat="server">Date inizio / fine</asp:Label>
                                                    <%--<asp:Label ID="Label17" runat="server" Width="1px"></asp:Label>--%>
                                                    <asp:TextBox ID="txtDataInizio" runat="server" AutoPostBack="true" MaxLength="10" Width="70px" TabIndex="4" BorderStyle="None"></asp:TextBox>
                                                    <asp:ImageButton ID="imgBtnShowCalendarDI" runat="server" CausesValidation="False" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                        ToolTip="apri il calendario"/>
                                                    <asp:CalendarExtender ID="txtDataInizio_CalendarExtender" runat="server" Enabled="True"
                                                        Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDI"
                                                        TargetControlID="txtDataInizio">
                                                    </asp:CalendarExtender>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server"
                                                        ControlToValidate="txtDataInizio" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                                        ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                    
                                                    <asp:TextBox ID="txtDataFine" runat="server" AutoPostBack="true" MaxLength="10" TabIndex="5" Width="70px" BorderStyle="None"></asp:TextBox>
                                                    <asp:ImageButton ID="imgBtnShowCalendarDF" runat="server" CausesValidation="False" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                        ToolTip="apri il calendario"/>
                                                    <asp:CalendarExtender ID="txtDataFine_CalendarExtender" runat="server" Enabled="True"
                                                        Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDF"
                                                        TargetControlID="txtDataFine">
                                                    </asp:CalendarExtender>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server"
                                                        ControlToValidate="txtDataFine" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                                        ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                
                                                <asp:Label ID="Label10" runat="server">Accettazione</asp:Label>
                                                <asp:TextBox ID="txtDataAccettazione" runat="server" AutoPostBack="true" MaxLength="10" TabIndex="6" Width="70px" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendarDA" runat="server" CausesValidation="False" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                    ToolTip="apri il calendario"/>
                                                <asp:CalendarExtender ID="txtDataAccettazione_CalendarExtender" runat="server" Enabled="True"
                                                    Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDA"
                                                    TargetControlID="txtDataAccettazione">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server"
                                                    ControlToValidate="txtDataAccettazione" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                
                                                <asp:Label ID="Label20" runat="server">Fatturazione</asp:Label>
                                                <asp:Button ID="btnTipofatt" runat="server" CommandName="btnTipofatt" Text="+" ToolTip="Gestione Tipo Fatturazione" Visible="false"/>
                                                <asp:TextBox ID="txtTipoFatt" runat="server" AutoPostBack="True" MaxLength="1" Width="50px" CssClass="nascondi" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLTipoFatt" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSTipoFatt" DataTextField="DescrizioneF" 
                                                    DataValueField="Codice" Height="22px" Width="120px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                
                                                <asp:Label ID="Label5" runat="server" Font-Bold="True" Visible="false">Tipo Evasione</asp:Label>
                                                <asp:RadioButton ID="optTipoEvTotaleTotale" ValidationGroup="0" GroupName="GrpTipoEvTotale" runat="server" Text ="Totale" AutoPostBack ="false" Visible="false"></asp:RadioButton>
                                                <asp:RadioButton ID="optTipoEvTotaleParziale" ValidationGroup="0" GroupName="GrpTipoEvTotale" runat="server" Text ="Parziale" AutoPostBack ="false" Visible="false"></asp:RadioButton>
                                            </div>
                                            <div style="width:100%;height:27px">
                                                <asp:Label ID="Label15" runat="server">Tipo Contratto</asp:Label>
                                                <asp:Label ID="Label22" runat="server" Width="11px"></asp:Label>
                                                <asp:TextBox ID="txtPagamento" runat="server" AutoPostBack="True" MaxLength="5" Visible="false" Width="50px" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLPagamento" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSPagamenti" DataTextField="DesTipoCaus" 
                                                    DataValueField="Codice" Height="22px" Width="655px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:CheckBox ID="chkSWTipoModSint" runat="server" Font-Bold="true" Text="Verbale sintetico" Visible="false" Checked="false" AutoPostBack="false"/>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">Riferimento</td>
                                        <td colspan="2" style="width:950px">
                                            <asp:TextBox ID="txtRiferimento" runat="server" MaxLength="20" TabIndex="7" Width="170px" BorderStyle="None"></asp:TextBox>         
                                            <asp:Label ID="Label6" runat="server" Text="Data riferimento"></asp:Label>
                                                <asp:TextBox ID="txtDataRif" runat="server" MaxLength="10" Width="70px" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendarDR" runat="server" Visible="false" CausesValidation="False" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataRif_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDR" 
                                                    TargetControlID="txtDataRif">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                    ControlToValidate="txtDataRif" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                            <asp:Label ID="lblCIGCUP" runat="server" Text="CIG/CUP"></asp:Label>
                                            <asp:TextBox ID="txtCIG" runat="server" MaxLength="10" Width="90px" BorderStyle="None"></asp:TextBox>
                                            <asp:TextBox ID="txtCUP" runat="server" MaxLength="15" Width="140px" BorderStyle="None"></asp:TextBox>
                                                    
                                            <asp:Label ID="Label8" runat="server" Text="Tipo rapporto " Visible="false"></asp:Label>
                                            <asp:DropDownList ID="DDLTipoRapp" runat="server" Visible="false" 
                                                AppendDataBoundItems="True" AutoPostBack="false" Height="22px" Width="175px">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Ordine di acquisto PA" Value="OA"></asp:ListItem>
                                                <asp:ListItem Text="Dati Contratto PA" Value="DC"></asp:ListItem>
                                                <asp:ListItem Text="Dati Convenzione" Value="DV"></asp:ListItem>
                                            </asp:DropDownList>  
                                            <asp:Label ID="Label23" runat="server" Width="05px"></asp:Label>
                                             <asp:Label ID="lblMagazzino" runat="server" Height="16px" Font-Bold="true" ForeColor="Blue">Magazzino</asp:Label>
                                             <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"  
                                                   AutoPostBack="false" DataSourceID="SqlDataMagazzino" 
                                                   DataTextField="Descrizione" 
                                                   DataValueField="Codice" Width="154px">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                             </asp:DropDownList>                                              
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
                                                Text="+" ToolTip="Anagrafica provvisoria"  />
                                        </div>
                                        </td>
                                        <td class="styleBordato" colspan="2">
                                            <div style="width:950px">
                                            <asp:TextBox ID="txtCodCliForFilProvv" runat="server" AutoPostBack="True" 
                                                MaxLength="16" TabIndex="8" Width="100px" BorderStyle="None"></asp:TextBox>
                                            <asp:Label ID="lblCliForFilProvv" runat="server" BorderColor="White" 
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="541px">Ragione Sociale</asp:Label>
                                            &nbsp;<asp:Label ID="lblLabelPICF" runat="server" Width="100px"> Partita IVA </asp:Label>&nbsp;<asp:Label ID="lblPICF" runat="server" Width="150px" BorderColor="White"
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label></div>
                                            <div style="width:950px;">
                                                <asp:Label ID="lblIndirizzo" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="493px">INDIRIZZO</asp:Label>
                                                <asp:Label ID="lblLocalita" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="415px">LOCALITA</asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleDest">
                                            <div>
                                                <asp:Label ID="Label21" runat="server" Width="70px">Destinazione</asp:Label>
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
                                        <div id="divDestinazione" style="overflow:auto; width:950px; height:110px; border-style:none;">
                                            <div style="width: 950px">
                                               <asp:Label ID="Label12" runat="server">Selezionata</asp:Label>
                                               <asp:Label ID="lblDestSel" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="413px">SELEZIONATA</asp:Label>
                                                <asp:Label ID="lblDenominazioneD" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="415px" ToolTip="DENOMINAZIONE">DENOMINAZIONE</asp:Label>
                                            </div>
                                            <div style="width: 950px;height:21px">
                                                <asp:TextBox ID="txtDestinazione1" runat="server" 
                                                    MaxLength="150" Width="495px" BorderStyle="None" BorderColor="Black"></asp:TextBox>
                                                 <asp:Label ID="lblRiferimentoD" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="415px" ToolTip="RIFERIMENTO">RIFERIMENTO</asp:Label>
                                            </div>
                                            <div style="width: 950px;height:21px">
                                                <asp:TextBox ID="txtDestinazione2" runat="server" 
                                                    MaxLength="150" Width="495px" BorderStyle="None" BorderColor="Black"></asp:TextBox>
                                                 <asp:Label ID="lblTel1D" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="135px" ToolTip="TEL1">TEL1</asp:Label>
                                                  <asp:Label ID="lblTel2D" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="135px" ToolTip="TEL2">TEL2</asp:Label>
                                                  <asp:Label ID="lblFaxD" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="130px" ToolTip="FAX">FAX</asp:Label>
                                            </div>
                                            <div style="width: 950px;height:21px">
                                                <asp:TextBox ID="txtDestinazione3" runat="server" 
                                                    MaxLength="150" Width="495px" BorderStyle="None" BorderColor="Black"></asp:TextBox>
                                                <asp:Label ID="lblEMailD" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="false" ForeColor="Black" Width="418px" ToolTip="EMAIL">EMAIL</asp:Label>
                                            </div>
                                             <div style="width: 950px">
                                                <asp:Label ID="Label17" runat="server" Width="930px" Font-Bold="true" Font-Size="XX-Small" ForeColor="Blue"  Text="Nota: nel caso la destinazione della singola Apparecchiatura non sia assegnata, Sarà questa la destinazione assegnata"></asp:Label>
                                            </div>
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
                                                    DataValueField="IBAN" Height="22px" Width="545px">
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
                                            <div>Resp.Area
                                                <asp:Button ID="btnRespArea" runat="server" CommandName="btnRespArea" 
                                                    Text="+" ToolTip="Gestione anagrafiche Resp.Area"/>
                                            </div>
                                        </td>
                                        <td colspan="2">
                                            <div style="width:950px">
                                                <asp:TextBox ID="txtCodRespArea" runat="server" AutoPostBack="True" MaxLength="5" 
                                                    TabIndex="9" Width="50px" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLRespArea" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSRespArea" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" Width="365px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                
                                                <asp:Label ID="Label14" runat="server">Resp.Visite</asp:Label>
                                                 <asp:Button ID="btnRespVisite" runat="server" CommandName="btnRespVisite" 
                                                    Text="+" ToolTip="Gestione anagrafiche Resp.Visite"/>
                                                <asp:TextBox ID="txtCodRespVisite" runat="server" AutoPostBack="True" MaxLength="5" 
                                                    TabIndex="10" Width="50px" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLRespVisite" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSRespVisite" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" Width="350px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
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
                                                    
                                                <asp:Label ID="Label16" runat="server">Listino</asp:Label>
                                                <asp:TextBox ID="txtListino" runat="server" AutoPostBack="True" MaxLength="5" Width="50px" CssClass="nascondi" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLListini" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSListini" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" Width="100px">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:Label ID="Label1" runat="server">Valuta</asp:Label>
                                                <asp:Label ID="lblCodValuta" runat="server" BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Visible="False"></asp:Label>
                                                <asp:Label ID="lblDesValuta" runat="server" BorderColor="White" 
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="50px"></asp:Label>
                                                    
                                                <asp:TextBox ID="txtCodCausale" runat="server" AutoPostBack="True" 
                                                    MaxLength="5" Width="50px" CssClass="nascondi" BorderStyle="None"></asp:TextBox>
                                                <asp:DropDownList ID="DDLCausali" runat="server" AppendDataBoundItems="True" 
                                                    AutoPostBack="True" DataSourceID="SqlDSCausMag" DataTextField="Descrizione" 
                                                    DataValueField="Codice" Height="22px" Width="365px" CssClass="nascondi">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:CheckBox ID="chkFatturaPA" runat="server" Font-Bold="false" Text="Fattura/NC PA" Checked="false" AutoPostBack="false" Visible="false"/>
                                                <asp:CheckBox ID="chkFatturaAC" runat="server" Font-Bold="false" Text="Fattura Accompagnatoria" Checked="false" AutoPostBack="false" Visible="false"/>
                                                <asp:CheckBox ID="chkScGiacenza" runat="server" Font-Bold="false" Text="Scarica giacenze" Checked="false" AutoPostBack="false" Visible="false"/>
                                                <asp:CheckBox ID="checkADeposito" runat="server" Font-Bold="true" Text="C/Deposito" Checked="false" AutoPostBack="false" Visible="false"/>
                                                <asp:CheckBox ID="ChkAcconto" runat="server" Font-Bold="true" Text="Fattura per Acconto/Saldo" Checked="false" AutoPostBack="false" Visible="false"/>
                                                
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0">Ns Riferimento</td>
                                        <td colspan="2" style="width:950px">
                                             <asp:TextBox ID="txtDesRefInt" runat="server"
                                                MaxLength="150" TabIndex="11" Width="900px" BorderStyle="None"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleLblTB0" colspan="1">
                                            Note</td>
                                        <td colspan="2" style="width:950px">
                                            <asp:TextBox ID="txtNoteDocumento" runat="server" TabIndex="12"
                                                Width="900px" Height="80px" TextMode="MultiLine"  BorderStyle="None"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:TabPanel>
                        <asp:TabPanel runat="server" ID="PanelDettAppLottiNrSerie" HeaderText="Dati Apparecchiature: N° di serie / Scadenze" BackColor="Silver" Visible="false">
                         <HeaderTemplate>Dati Apparecchiature: N° di serie / Scadenze</HeaderTemplate>
                         <ContentTemplate>
                         <table class="sfondopagine" style="width:1090px; height:500px;">
                         <tr>
                         <td align="left">
                            <asp:Panel ID="Panel2" runat="server" Height="590px" Width ="1085px">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server"><ContentTemplate>
                            <div id="divGridViewDettL" style="overflow: auto; height:440px; border-style:groove; background-color: Silver;">
                              <asp:GridView ID="GridViewDettL" runat="server" AutoGenerateColumns="False" 
                                    EmptyDataText="Nessun dato disponibile."  
                                    DataKeyNames="NCollo" 
                                    GridLines="None" CssClass="GridViewStyle" EnableTheming="True"
                                    AllowPaging="False" 
                                    PagerSettings-Mode="NextPreviousFirstLast"
                                    PagerSettings-Visible="True"
                                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" >
                                    <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                                        LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                                        Mode="NextPreviousFirstLast" 
                                        NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                        PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                                    <RowStyle CssClass="RowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass="HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle"/>
                                    <Columns>
                                        <asp:TemplateField InsertVisible="True"><ItemTemplate>
                                        <asp:Button ID="btnInsRigaDopoL" runat="server" CommandName="Select" Text="+" />
                                        </ItemTemplate>
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="20px" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="10px" />
                                        </asp:TemplateField>
                                        <asp:CommandField ButtonType="Button" ShowEditButton="True" 
                                            HeaderStyle-Width="50px" >
                                            <controlstyle font-size="XX-Small" />
                                            <HeaderStyle Width="50px" />
                                        </asp:CommandField>
                                        <asp:CommandField ButtonType="Button" ShowDeleteButton="True" 
                                            HeaderStyle-Width="20px" DeleteText="X" > 
                                            <controlstyle font-size="XX-Small" />
                                            <HeaderStyle Width="20px" />
                                        </asp:CommandField>
                                    </Columns>
                              </asp:GridView>
                              </div>
                              </ContentTemplate>
                              </asp:UpdatePanel>
                            <div style="height:10px"></div>
                            <div>
                                <asp:Button ID="btnPrimaRigaL" runat="server" CommandName="btnPrimaRigaL" Text="Inserisci N° di serie" 
                                 ToolTip="Inserisci N° di serie" class="btnstyle2R" />
                                 <asp:Button ID="btnCaricoLotti" runat="server" Text="Lettore N° di serie"
                                 ToolTip="Carico N° di serie con lettore" class="btnstyle2R" Visible="false" />
                            </div>  
                            </asp:Panel> 
                         </td>
                         </tr>
                        </table>
                        </ContentTemplate>
                        </asp:TabPanel>
                        <asp:TabPanel ID="Dettaglio" runat="server" HeaderText="Dettagli: Dati Apparecchiature: N° di serie / Scadenze Attività" >
                            <ContentTemplate>
                                <uc2:WUC_ContrattiDett ID="WUC_ContrattiDett1" runat="server"  />
                            </ContentTemplate>
                        </asp:TabPanel>
                        <asp:TabPanel ID="SpeseTraspTotale" runat="server" 
                            HeaderText="Attività / Sconti - Spese - Totale documento - Scadenze">
                            <ContentTemplate>
                                <uc3:WUC_ContrattiSpeseTraspTot ID="WUC_ContrattiSpeseTraspTot1" 
                                    runat="server" />
                            </ContentTemplate>
                        </asp:TabPanel>
                    </asp:TabContainer>
                </td>
                <td align="left" class="styleTDBTN">
                    <div>
                    <asp:UpdatePanel ID="UpdatePanelBTN" runat="server" ChildrenAsTriggers="True" UpdateMode="Always" EnableViewState="True">
                        <ContentTemplate>
                            <div>
                                <a ID="LnkSCEC" runat="server" href="..\WebFormTables\WF_AnagraficaClienti.aspx?labelForm=Anagrafica clienti (Scheda CoGe/E.C.)" target="_blank" onclick="return openUrl(this.href);">(Scheda Cliente)</a>
                            </div>
                            <div>
                                <asp:LinkButton ID="LnkRegimeIVA" runat="server" OnClick="LnkAltriDatiDoc_Click" Text="Regime IVA"></asp:LinkButton>
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
                            <div style="height:5px"></div>
                            <div>
                                <asp:Button ID="btnDuplicaDNum" runat="server" class="btnstyle2R" Text="Duplica apparecchiatura" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnNuovaDNum" runat="server" class="btnstyle2R" Text="Nuova apparecchiatura" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnGeneraAttDNum" runat="server" class="btnstyle2R" Text="Genera attività per periodo"/>
                            </div>
                            <%--<div style="height:5px"></div>--%>
                            <div style="height:10px; text-align:center">
                                <asp:Label ID="lblStampe" runat="server" BorderColor="White"
                                BorderStyle="None" Font-Bold="true" Font-Size="XX-Small" ForeColor="Blue" Visible="true">Stampe</asp:Label>
                            </div> 
                            <%--<div style="height:10px"></div>--%>
                            <div>
                                <asp:Button ID="btnStampa" runat="server" class="btnstyle05R" Text="Proforma" Visible="true"/>
                                <asp:Button ID="btnVerbale" runat="server" class="btnstyle05R" Text="Verbale" Visible="true"/>
                            </div>
                            <div>
                                <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Proforma">Apri Proforma</a>
                                <a ID="LnkVerbale" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Verbale">Apri Verbale</a>
                            </div>
                            <div style="height: 5px"></div>
                            <div style="overflow-x:hidden; overflow-y:auto;height:220px;width:105px;text-align:center;">
                                 <asp:Label ID="lblMessDoc" runat="server" BorderColor="Snow" Font-Bold="false" Font-Size="Smaller" ForeColor="White" 
                                    BorderStyle="Outset" BackColor="DarkRed" Style="text-decoration:none;" Height="1500px" Width="80px" Visible="false"></asp:Label>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </div>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>