<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ArticoliInstallati.ascx.vb" Inherits="SoftAziOnLine.WUC_ArticoliInstallati" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc8" %>
<%@ Register src="../WebUserControl/WFP_Anagrafiche_Modify.ascx" tagname="WFP_Anagrafiche_Modify" tagprefix="uc5" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
<style type="text/css">
    .btnstyle
    {
        Width: 108px;
        height: 40px;
        white-space: pre-wrap;      
    }
    .styleTDBTN
    {
        height: 478px;
    }
    .styleLblTB0
    {
        height: 25px;
        width: 160px;
        }
     .styleTxtCodTB0
    {
        height: 25px;
        }  
    .styleTBPagina
    {
        height: 480px;
        width: 850px;
    }
    .styleDest
    {
        height: 60px;
    }
    .styleLblDDl
    {
        height: 25px;
        width: 700px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" 
    Width="980px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc8:WFP_ElencoCliForn ID="WFPElencoCli" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
        <uc8:WFP_ElencoCliForn ID="WFPElencoFor" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori" />
        <uc5:WFP_Anagrafiche_Modify ID="WFP_Anagrafiche_Modify1" runat="server" />
        <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
        <asp:SqlDataSource ID="SqlDSCliForFilProvv" runat="server" 
            SelectCommand="SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = @Codice)">
            <SelectParameters>
                <asp:SessionParameter Name="Codice" SessionField="Codice_CoGe" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSContratti" runat="server" 
            SelectCommand="SELECT * FROM [ContrattiAss_Tipo] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSDestinazione" runat="server" 
            SelectCommand="SELECT * FROM [DestClienti] WHERE ([Codice] = @Codice) ORDER BY [Ragione_Sociale]">
            <SelectParameters>
                <asp:SessionParameter Name="Codice" SessionField="Codice_CoGe" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

        <table style="width:auto; height:auto;">
            <tr>
                <td class="styleTBPagina">                    
                            <ContentTemplate>
                                <table bgcolor="Silver" style="width:840px; height:500px;">
                                    <tr>
                                        <td colspan="3" align="left">
                                        <div>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label9" runat="server">Data</asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtDataInstallazione" runat="server" MaxLength="10" TabIndex="1" Width="70px" BorderStyle="None"></asp:TextBox>
                                                    <asp:ImageButton ID="imgBtnShowCalendarDInst" runat="server" 
                                                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                            ToolTip="apri il calendario" />
                                                    <asp:CalendarExtender ID="txtDataInstallazione_CalendarExtender" runat="server" 
                                                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDInst" 
                                                            TargetControlID="txtDataInstallazione">
                                                    </asp:CalendarExtender>
                                                    <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                            ControlToValidate="txtDataInstallazione" ErrorMessage="*" 
                                                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label11" runat="server">Scadenza garanzia</asp:Label>
                                                </td>    
                                                <td>
                                                    <asp:TextBox ID="txtDataScadGaranzia" runat="server" MaxLength="10" TabIndex="2" Width="70px" BorderStyle="None"></asp:TextBox>
                                                    <asp:ImageButton ID="imgBtnShowCalendarDG" runat="server" 
                                                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                            ToolTip="apri il calendario" />
                                                    <asp:CalendarExtender ID="txtDataScadGaranzia_CalendarExtender" runat="server" 
                                                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDG" 
                                                            TargetControlID="txtDataScadGaranzia">
                                                    </asp:CalendarExtender>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                                                            ControlToValidate="txtDataScadGaranzia" ErrorMessage="*" 
                                                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="chkEmailGa" runat="server" Text="1 E-mail scadenza garanzia" TabIndex="16" AutoPostBack="true"/>
                                                    <asp:CheckBox ID="chkEmailGa2" runat="server" Text="2 E-mail scadenza garanzia" TabIndex="16" AutoPostBack="true"/>
                                                </td>
                                                <%--<asp:Label ID="Label8" runat="server">Scadenza contratto</asp:Label>
                                                <asp:Label ID="lblDataScadContratto" runat="server" Width="70px" BorderColor="White"
                                                        BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>--%>
                                                <%--&nbsp;&nbsp;&nbsp;--%>
                                            </tr>
                                            <tr>
                                                <td>        
                                                    <asp:Label ID="Label12" runat="server">Sostituito il</asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtDataSostituzione" runat="server" MaxLength="10" TabIndex="3" Width="70px" BorderStyle="None"></asp:TextBox>
                                                    <asp:ImageButton ID="imgBtnShowCalendarDSost" runat="server" 
                                                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                            ToolTip="apri il calendario" />
                                                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" 
                                                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDSost" 
                                                            TargetControlID="txtDataSostituzione">
                                                    </asp:CalendarExtender>
                                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                                                            ControlToValidate="txtDataSostituzione" ErrorMessage="*" 
                                                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                </td>
                                                <td><asp:CheckBox ID="chkSostituito" runat="server" Text="Sostituito" TabIndex="16" AutoPostBack="true"/></td>
                                                <td><asp:Label ID="Label17" runat="server">Ultima modifica:</asp:Label></td>
                                                <td><asp:Label ID="lblModificatoDa" runat="server" Width="350px" BorderColor="White" BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label></td>
                                            </tr>
                                        </table>
                                    </div>
                                        </td>
                                    </tr>
                                    <%--NASCONDO RIGHE BLOCCO DATI DEL CONTRATTO--%>
                                    <tr style="display: none">
                                        <td colspan="3" align="left">
                                            <asp:Label ID="lblDatiContratto" runat="server" Text="Dati contratto di assistenza" Width="99%" BorderWidth="1px" BorderColor="White" BorderStyle="Groove" Font-Bold="true" ForeColor="Black" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="display: none">
                                        <td class="styleLblTB0">
                                            <asp:Label ID="lblNCA" runat="server">N° Contratto</asp:Label>
                                        </td>
                                        <td>
                                            <div style="width: 140px">
                                            <asp:TextBox ID="txtNumero" runat="server" MaxLength="10" TabIndex="4"
                                                    Width="80px" AutoPostBack="true" ></asp:TextBox>
                                            <asp:Button ID="btnGetNewNumero" runat="server"  
                                                    Text="+" />
                                            </div>
                                        </td>
                                        <td>
                                            <div style="width:560px">
                                                <asp:Label ID="lblValidoDal" runat="server">Valido dal </asp:Label>
                                                <asp:TextBox ID="txtDataInizio" runat="server" MaxLength="10" TabIndex="5" Width="70px" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendarDIC" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataInizio_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDIC" 
                                                    TargetControlID="txtDataInizio">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtDataInizio" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                <asp:Label ID="lblValidoAl" runat="server">al</asp:Label>
                                                <asp:TextBox ID="txtDataFine" runat="server" MaxLength="10" TabIndex="6" Width="70px" BorderStyle="None"></asp:TextBox>
                                                <asp:ImageButton ID="imgBtnShowCalendarDFC" runat="server" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataFine_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDFC" 
                                                    TargetControlID="txtDataFine">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
                                                    ControlToValidate="txtDataFine" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                &nbsp;&nbsp;<asp:Label ID="lblImporto" runat="server">Totale Importo</asp:Label>
                                                <asp:TextBox ID="TxtImporto" runat="server" MaxLength="20" TabIndex="7" Width="150px" BorderStyle="None"></asp:TextBox>   
                                            </div>
                                        </td>
                                    </tr>
                                    <tr style="display: none">
                                        <td class="styleLblTB0">
                                            <asp:Label ID="lblTipoCA" runat="server">Tipo contratto</asp:Label>
                                        </td>
                                        <td colspan="2">
                                            <div style="width: 720px">
                                                <asp:DropDownList ID="DDLTipoContratto" runat="server" 
                                                    AppendDataBoundItems="True" AutoPostBack="false" 
                                                    DataSourceID="SqlDSContratti" DataTextField="Descrizione" 
                                                    DataValueField="ID" Height="22px" TabIndex="8" Width="703px" BorderStyle="None">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                    <%--FINE BLOCCO DATI CONTRATTO--%>
                                    <tr>
                                        <td colspan="3" align="left">
                                            <asp:Label ID="Label4" runat="server" Text="Dati articolo / N°Serie/Lotto / Stato / Data scadenze: Elettrodi,Batterie / Invio comunicazione scadenze" Width="99%" BorderWidth="1px" BorderColor="White" BorderStyle="Groove" Font-Bold="true" ForeColor="Black" ></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td >
                                            <div>
                                            <asp:Label ID="LblCodArt" runat="server">Articolo</asp:Label>&nbsp;&nbsp;
                                            <asp:Button ID="BtnSelArticoloIns" runat="server" CommandName="BtnSelArticoloIns" Text="?" ToolTip="Ricerca articoli" Enabled="true"/>
                                        </div>
                                        </td>
                                        <td colspan="2">
                                            <div style="width: 720px">
                                                <asp:TextBox ID="TxtArticoloCod" runat="server" AutoPostBack="True" MaxLength="20" TabIndex="9" Width="140px" BorderStyle="None"></asp:TextBox>
                                                <asp:TextBox ID="TxtArticoloDesc" runat="server" AutoPostBack="false" MaxLength="150" TabIndex="10" Width="560px" BorderStyle="None"></asp:TextBox>                                                
                                                <asp:Label ID="LblSN" runat="server">N° Serie</asp:Label>
                                                <asp:TextBox ID="TxtArticoloSN" runat="server" AutoPostBack="false" MaxLength="30" TabIndex="11" Width="200px" BorderStyle="None"></asp:TextBox>&nbsp;&nbsp;
                                                <%--<asp:Label ID="LblLotto" runat="server">Lotto&nbsp;</asp:Label>--%>&nbsp;
                                                <asp:TextBox ID="TxtArticoloLotto" runat="server" AutoPostBack="false" MaxLength="30" TabIndex="12" Width="200px" Visible="false" BorderStyle="None"></asp:TextBox>
                                                <asp:Label ID="Label7" runat="server">Stato&nbsp;</asp:Label>&nbsp;
                                                <asp:RadioButton ID="rbtnAttivo" runat="server" ToolTip="Articolo attivo"
                                                    Text="Attivo" AutoPostBack="false" GroupName="Tipo" TabIndex="13"/>
                                                <asp:RadioButton ID="rbtnDismesso" runat="server" ToolTip="Articolo dismesso"
                                                    Text="Dismesso" AutoPostBack="false" GroupName="Tipo" TabIndex="14"/>
                                                <asp:Label ID="Label6" runat="server" Width="50px"></asp:Label>
                                                <asp:CheckBox ID="chkInRiparazione" runat="server" Text="In riparazione" TabIndex="15" />
                                            </div>
                                        </td>                                                                              
                                    </tr>
                                    <%--<tr>
                                        <td class="styleLblTB0">
                                            Stato</td>
                                        <td class="styleTxtCodTB0" colspan="2">
                                            <div style="width: 720px">
                                                <asp:RadioButton ID="rbtnAttivo" runat="server" ToolTip="Articolo attivo"
                                                    Text="Attivo" AutoPostBack="false" GroupName="Tipo" TabIndex="13"/>
                                                <asp:RadioButton ID="rbtnDismesso" runat="server" ToolTip="Articolo dismesso"
                                                    Text="Dismesso" AutoPostBack="false" GroupName="Tipo" TabIndex="14"/>
                                                <asp:Label ID="Label6" runat="server" Width="50px"></asp:Label>
                                                <asp:CheckBox ID="chkInRiparazione" runat="server" Text="In riparazione" TabIndex="15" />
                                                <asp:Label ID="Label7" runat="server" Width="10px"></asp:Label>
                                                <asp:CheckBox ID="chkSostituito" runat="server" Text="Sostituito" TabIndex="16"/>
                                            </div>
                                        </td>
                                    </tr>--%>  
                                    <tr>
                                        <td colspan="3" align="left">
                                        <div>
                                            <table>
                                                <tr>
                                                    <td>
                                                    <%--<asp:Label ID="Label1" runat="server" Width="110px">Data scadenze</asp:Label>--%>
                                                        <asp:Label ID="Label10" runat="server">Elettrodi</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtDataScadElettrodi" runat="server" MaxLength="10" TabIndex="1" Width="70px" BorderStyle="None"></asp:TextBox>
                                                        <asp:ImageButton ID="imgBtnShowCalendarDSCEl" runat="server" 
                                                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                                ToolTip="apri il calendario" />
                                                        <asp:CalendarExtender ID="CalendarExtender2" runat="server" 
                                                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDSCEl" 
                                                                TargetControlID="txtDataScadElettrodi">
                                                        </asp:CalendarExtender>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" 
                                                                ControlToValidate="txtDataScadElettrodi" ErrorMessage="*" 
                                                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                    </td>
                                                    <td><asp:CheckBox ID="chkEmailEl" runat="server" Text="1 E-mail scadenza elettrodi" TabIndex="16" AutoPostBack="true"/></td>
                                                    <td><asp:CheckBox ID="chkEmailEl2" runat="server" Text="2 E-mail scadenza elettrodi" TabIndex="16" AutoPostBack="true"/></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <%--<asp:Label ID="Label8" runat="server" Width="20px"></asp:Label>--%>
                                                        <asp:Label ID="Label2" runat="server">Batterie</asp:Label>
                                                    </td>
                                                    <td>        
                                                        <asp:TextBox ID="txtDataScadBatterie" runat="server" MaxLength="10" TabIndex="2" Width="70px" BorderStyle="None"></asp:TextBox>
                                                        <asp:ImageButton ID="imgBtnShowCalendarDScadBa" runat="server" 
                                                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                                                ToolTip="apri il calendario" />
                                                        <asp:CalendarExtender ID="CalendarExtender3" runat="server" 
                                                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDScadBa" 
                                                                TargetControlID="txtDataScadBatterie">
                                                        </asp:CalendarExtender>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" 
                                                                ControlToValidate="txtDataScadBatterie" ErrorMessage="*" 
                                                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                                    </td>
                                                    <td><asp:CheckBox ID="chkEmailBa" runat="server" Text="1 E-mail scadenza batterie" TabIndex="16" AutoPostBack="true"/></td>
                                                    <td><asp:CheckBox ID="chkEmailBa2" runat="server" Text="2 E-mail scadenza batterie" TabIndex="16" AutoPostBack="true"/></td>
                                                </tr>
                                            </table>
                                        </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3" align="left">
                                            <asp:Label ID="Label5" runat="server" Text="Dati cliente / Luogo installazione / Riferimenti" Width="99%" BorderWidth="1px" BorderColor="White" BorderStyle="Groove" Font-Bold="true" ForeColor="Black" ></asp:Label>
                                        </td>
                                    </tr>                                   
                                    <tr>
                                        <td class="styleLblTB0">
                                        <div>
                                            <asp:Label ID="LblCliente" runat="server">Cliente</asp:Label>
                                        </div>
                                        <div>
                                            <asp:Button 
                                                ID="btnCercaAnagrafica" runat="server" CausesValidation="False" CommandName="btnCercaAnagrafica" 
                                                Text="?" ToolTip="Ricerca anagrafiche" />
                                            <asp:Button
                                                ID="btnModificaAnagrafica" runat="server" CausesValidation="False" CommandName="btnModificaAnagrafica"
                                                Text="M" ToolTip="Modifica dati anagrafici" />
                                        </div>
                                        </td>
                                        <td class="styleTxtCodTB0" colspan="2">
                                            <div style="width: 720px">
                                            <asp:TextBox ID="txtCodCliForFilProvv" runat="server" AutoPostBack="True" MaxLength="16" TabIndex="17" Width="100px" BorderStyle="None"></asp:TextBox>
                                            <asp:Label ID="lblRagSoc" runat="server" BorderColor="White" 
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="375px">Ragione Sociale</asp:Label>
                                            &nbsp;<asp:Label ID="lblLabelPICF" runat="server" Width="30px">P.IVA</asp:Label>
                                            &nbsp;<asp:Label ID="lblPICF" runat="server" Width="165px" BorderColor="White"
                                                BorderStyle="Outset" Font-Bold="True" ForeColor="Black"></asp:Label>
                                            </div>
                                            <div style="width: 720px; background-color: silver;">
                                                <asp:Label ID="lblIndirizzo" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="370px">INDIRIZZO</asp:Label>
                                                <asp:Label ID="lblLocalita" runat="server" BorderColor="White"
                                                    BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="320px">LOCALITA</asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="styleDest">
                                            <asp:Label ID="lblLabelDest" runat="server" Width="70px">Destinazione</asp:Label>
                                        </td>
                                        <td colspan="2">
                                            <div style="width: 700px">
                                                <asp:Label ID="lblLabelSelDest" runat="server">Seleziona da elenco</asp:Label>
                                                &nbsp;<asp:DropDownList ID="DDLDestinazioni" runat="server" 
                                                    AppendDataBoundItems="True" AutoPostBack="True" 
                                                    DataSourceID="SqlDSDestinazione" DataTextField="Ragione_Sociale" 
                                                    DataValueField="Progressivo" Height="22px" TabIndex="18" Width="500px" BorderStyle="None">
                                                    <asp:ListItem ></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div style="width: 700px">
                                                <asp:TextBox ID="txtDestinazione1" runat="server" MaxLength="150" TabIndex="19" Width="700px" BorderStyle="None"></asp:TextBox>
                                            </div>
                                            <div style="width: 700px">
                                                <asp:TextBox ID="txtDestinazione2" runat="server" MaxLength="150" TabIndex="20" Width="700px" BorderStyle="None"></asp:TextBox>
                                            </div>
                                            <div style="width: 700px">
                                                <asp:TextBox ID="txtDestinazione3" runat="server" MaxLength="150" TabIndex="21" Width="700px" BorderStyle="None"></asp:TextBox>
                                            </div>
                                            <div style="width: 700px">
                                                <asp:Label ID="Label1" runat="server">E-mail destinazione</asp:Label>
                                                <asp:TextBox ID="txtEmailDest" runat="server" MaxLength="100" TabIndex="21" AutoPostBack="true" Width="500px" BorderStyle="None"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>  
                                    <tr>
                                        <td class="styleLblTB0">Riferimento</td>
                                        <td colspan="2" style="width:700px">
                                            <asp:TextBox ID="txtRiferimento" runat="server" MaxLength="100" TabIndex="22" Width="700px" BorderStyle="None"></asp:TextBox>
                                        </td>
                                    </tr>  
                                    <tr>
                                        <td class="styleLblTB0">Ns Riferimento</td>
                                        <td colspan="2" style="width:700px">
                                            <asp:TextBox ID="txtNsRiferimento" runat="server" MaxLength="100" TabIndex="22" Width="700px" BorderStyle="None"></asp:TextBox>
                                        </td>
                                    </tr>                                     
                                    <tr>
                                        <td class="styleLblTB0">Telefono 1</td> 
                                        <td colspan="2">
                                            <div style="width:700px">
                                                <asp:Label ID="lblTelefono1" runat="server" BorderColor="White"
                                                        BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="150px">TELEFONO 1</asp:Label>
                                                <asp:Label ID="Label3" runat="server" Text="" Width="25px"></asp:Label>
                                                <asp:Label ID="Label13" runat="server" Text="Telefono 2"></asp:Label>
                                                <asp:Label ID="lblTelefono2" runat="server" BorderColor="White"
                                                        BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="150px">TELEFONO 2</asp:Label>
                                                <asp:Label ID="Label14" runat="server" Text="" Width="30px"></asp:Label>
                                                <asp:Label ID="Label8" runat="server" Text="Fax"></asp:Label>
                                                <asp:Label ID="lblFax" runat="server" BorderColor="White"
                                                        BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="150px">FAX</asp:Label>
                                            </div>
                                        </td>                                                                              
                                    </tr>    
                                    <tr>
                                        <td class="styleLblTB0">E-mail cliente</td> 
                                        <td colspan="2">
                                            <div style="width:700px">
                                                <asp:TextBox ID="TxtEmail" runat="server" MaxLength="100" TabIndex="26" AutoPostBack="true" Width="99%" BorderStyle="None"></asp:TextBox>
                                                <asp:Label ID="Label15" runat="server" Text="E-mail invio scadenze"></asp:Label>
                                                <asp:TextBox ID="txtEmailInvioScad" runat="server" MaxLength="100" TabIndex="26" AutoPostBack="true" Width="480px" BorderStyle="None"></asp:TextBox>
                                            </div>
                                        </td>                                                                              
                                    </tr>    
                                    <tr>
                                        <td class="styleLblTB0">
                                            <asp:CheckBox ID="chkAggEmailT" AutoPostBack="false" runat="server" Font-Bold="True" Text="Si Aggiorna" TabIndex="1" />
                                        </td> 
                                        <td colspan="2">
                                            <div style="width:700px">
                                                <asp:Label ID="Label16" runat="server" Text="tutte le E-mail inviate in archivio"></asp:Label>
                                                <asp:Label ID="lblEmailInvio" runat="server" BorderColor="White"
                                                        BorderStyle="Outset" Font-Bold="True" ForeColor="Black" Width="400px"></asp:Label>
                                            </div>
                                        </td>                                                                              
                                    </tr>    
                                    <%--<tr>
                                        <td class="styleLblTB0" colspan="1">Note</td>
                                        <td class="styleTxtCodTB0" colspan="2">
                                            <asp:TextBox ID="txtNoteDocumento" runat="server" TabIndex="27"
                                                Width="700px" TextMode="MultiLine" ></asp:TextBox>
                                        </td>
                                    </tr>--%>
                                </table>
                            </ContentTemplate>
                </td>
                <td align="left" class="styleTDBTN">
                    <asp:UpdatePanel ID="UpdatePanelBTN" runat="server">
                        <ContentTemplate>
                            <div id="noradio">
                                <asp:LinkButton ID="LnkRitorno" runat="server" OnClick="LnkRitorno_Click">Menu precedente</asp:LinkButton>
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyle" Text="Nuovo" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnAggiorna" runat="server" class="btnstyle" Text="Aggiorna" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyle" Text="Elimina" />
                            </div>
                            <div style="height:15px"></div>
                            <div style="height:15px"></div>
                            <div style="height:15px"></div>
                            <div style="height:15px"></div>
                            <div style="height:15px; text-align:center">
                                <asp:Label ID="lblStampe" runat="server" BorderColor="White"
                                BorderStyle="None" Font-Bold="True" ForeColor="Black"></asp:Label>
                            </div> 
                            <div style="height:5px"></div>
                            <div>
                                <asp:Button ID="btnStampa" runat="server" class="nascondi" Text="Stampa"/>
                            </div>
                            <div style="height:50px"></div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>