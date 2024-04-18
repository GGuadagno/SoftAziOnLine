<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WFP_ElencoEmail.ascx.vb" Inherits="SoftAziOnLine.WFP_ElencoEmail" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/WebUserControlTables/WUC_ElencoEmail.ascx" TagName="ElencoEmail" TagPrefix="wuc" %>
<%@ Register Src="~/WebUserControl/WFP_ElencoCatCli.ascx" TagName="WFPElencoCatCli" TagPrefix="wuc" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>

<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />

<style type="text/css">
    .modalBackground {
        background-color:Gray;
        filter:alpha(opacity=70);
        opacity:0.7;
    }

    .modalPopup {
        background-color:#ffffdd;
        border-width:3px;
        border-style:solid;
        border-color:Gray;
        padding:3px;
        width:250px;
    }
    .ajaxProgress
        {
            z-index:20;position:absolute;background:url('../Immagini/loading.gif') no-repeat center;
        }
    .fisso
    {
        z-index:auto;
        position:fixed;
    }
</style>
<wuc:WFPElencoCatCli ID="WFPElencoCategorie" runat="server" Tabella="Categorie" Titolo="Elenco Categorie"/>
<uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
<ajaxToolkit:ModalPopupExtender runat="server" ID="ProgrammaticModalPopup"
    TargetControlID="LinkButton1"
    PopupControlID="programmaticPopup" 
    BackgroundCssClass="modalBackground"
    DropShadow="true"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>
<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>     
    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:center;">
        <asp:Panel ID="PanelCategoria" style="margin-top: 0px;" runat="server" GroupingText="PARAMETRI DI RICERCA E-MAIL INVIATE">
            <table width="100%">
                <tr style="display: none">
                    <td align="left">Singola categoria</td>
                    <td>
                        <asp:DropDownList ID="ddlCatCli" runat="server" DataSourceID="SqlDa_CatCli" 
                            DataTextField="Descrizione" DataValueField="Codice" Width="500px" 
                            AppendDataBoundItems="true" Enabled="False" AutoPostBack="True">
                            <asp:ListItem Value="0" Text="Categoria non definita"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDa_CatCli" runat="server" 
                            SelectCommand="SELECT [Codice], [Descrizione] FROM [Categorie] WHERE ISNULL(Categorie.InvioMailSc,0) <> 0 ORDER BY Descrizione">
                        </asp:SqlDataSource>
                    </td>
                    <td align="right">
                        <asp:CheckBox ID="chkTutteCatCli" runat="server" Text="Seleziona tutte le categorie" 
                            AutoPostBack="true" Checked="True" />
                    </td>
                </tr>
                <tr style="display: none">
                    <td align="left"></td>
                    <td>
                        <asp:CheckBox ID="chkRaggrCatCli" runat="server" Text="Seleziona tutte le categorie per descrizione iniziale uguale" 
                            AutoPostBack="True" Checked="False" Enabled="false" />
                    </td>
                    <td align="right">
                        <asp:Button ID="btnSelCategorie" runat="server" Text="?" OnClick="btnSelCategorie_Click" />
                        <asp:CheckBox ID="chkSelCategorie" runat="server" Text="Selezione multipla categorie" 
                            AutoPostBack="True" Checked="False" Enabled="true" />
                    </td>
                 </tr>
                 <tr>
                    <td align="left">Codice cliente:</td>
                    <td align="left">
                        <asp:Button ID="btnCercaAnagrafica1" runat="server"  
                            Height="25px" TabIndex="3" Text="?" Visible="true" Width="25px" Enabled="false" />
                        &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server" MaxLength="20" 
                            Width="138px" AutoPostBack="True" Enabled="false"></asp:TextBox>
                        &nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" MaxLength="150" TabIndex="4" 
                            Width="460px" Enabled="false"></asp:TextBox>
                        &nbsp;&nbsp;<asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" Checked="true"
                            TabIndex="10" Text="Tutti i clienti" />
                    </td>
                 </tr>
                 <tr>
                    <td align="left">Stato E-mail:</td>
                    <td align="left">
                        <asp:RadioButton ID="rbtnDaInviare" runat="server" ToolTip="Da inviare" Text="Da inviare" 
                                AutoPostBack="True" GroupName="CliEmail"/>
                        <asp:RadioButton ID="rbtnInviata" runat="server" ToolTip="Inviata" Text="Inviata" 
                                AutoPostBack="true" GroupName="CliEmail"/>
                        <asp:RadioButton ID="rbtnSollecito" runat="server" ToolTip="Sollecito inviato"
                                        Text="Sollecito inviato" AutoPostBack="True" GroupName="CliEmail"/>
                        <asp:RadioButton ID="rbtnAnnullata" runat="server" ToolTip="Annullata" Text="Annullata" 
                                AutoPostBack="true" GroupName="CliEmail"/>
                        <asp:RadioButton ID="rbtnParzConclusa" runat="server" ToolTip="Parz.Conclusa" Text="Parz.Conclusa" 
                                AutoPostBack="true" GroupName="CliEmail"/>
                        <asp:RadioButton ID="rbtnConclusa" runat="server" ToolTip="Conclusa" Text="Conclusa" 
                                AutoPostBack="true" GroupName="CliEmail"/>
                        <asp:RadioButton ID="rbtnInvioErr" runat="server" ToolTip="Invio Errato" Text="Invio Errato" 
                                AutoPostBack="true" GroupName="CliEmail"/>
                       
                    </td>
                </tr>
                <tr>
                    <td align="left"></td>
                    <td align="left">
                         <asp:RadioButton ID="rbtnTutte" runat="server" ToolTip="Tutte" Text="Tutte" 
                                    AutoPostBack="true" Checked="True" GroupName="CliEmail"/>
                         <asp:Label ID="Label2" runat="server" Width="30px"></asp:Label>
                         <asp:DropDownList ID="ddlStato" runat="server" AutoPostBack="true" Width="150px"></asp:DropDownList>
                         <asp:Button ID="btnCambiaStato" runat="server" Text="Cambia stato" />
                         <asp:Label ID="Label1" runat="server" Width="5px"></asp:Label>
                         <asp:CheckBox ID="chkInvioSollecito" runat="server" AutoPostBack="false" Checked="false" Enabled="false" 
                            TabIndex="10" Text="Invia sollecito altrimenti Re-Invia E-mail" />
                    </td>
                </tr>
           </table>
      </asp:Panel>
      <asp:Panel ID="Panel1" style="margin-top: 0px;" runat="server" GroupingText="">
           <table width="100%">
                <tr>
                    <td align="left">Periodo: dal</td>
                    <td align="left">
                        <asp:TextBox ID="txtDallaData" runat="server" TabIndex="1" Width="70px" MaxLength="10" AutoPostBack="True" ></asp:TextBox>
                        <asp:ImageButton ID="imgBtnShowCalendarDData" runat="server" 
                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                            ToolTip="apri il calendario" />
                        <ajaxToolkit:CalendarExtender ID="txtDallaData_CalendarExtender" runat="server" 
                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDData" 
                            TargetControlID="txtDallaData">
                        </ajaxToolkit:CalendarExtender>
                        <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                             ControlToValidate="txtDallaData" ErrorMessage="*" 
                             ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                    </td>
                    <td align="left">al</td>
                    <td align="left">
                        <asp:TextBox ID="txtAllaData" runat="server" TabIndex="2" Width="70px" MaxLength="10" AutoPostBack="True" ></asp:TextBox>
                        <asp:ImageButton ID="imgBtnShowCalendarAData" runat="server" 
                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                            ToolTip="apri il calendario" />
                        <ajaxToolkit:CalendarExtender ID="txtAllaData_CalendarExtender" runat="server" 
                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarAData" 
                            TargetControlID="txtAllaData">
                        </ajaxToolkit:CalendarExtender>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                             ControlToValidate="txtAllaData" ErrorMessage="*" 
                             ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                    </td>
                    <td align="left">dal Numero:</td>
                    <td align="left">
                        <asp:TextBox ID="txtDalNumeno" runat="server" TabIndex="2" Width="50px" MaxLength="5" AutoPostBack="True" ></asp:TextBox>
                    </td>
                    <td align="left">al Numero:</td>
                    <td align="left">
                        <asp:TextBox ID="txtAlNumero" runat="server" TabIndex="2" Width="50px" MaxLength="5" AutoPostBack="True" ></asp:TextBox>
                    </td>
                    <td style="display: none">  
                        <asp:CheckBox ID="chkSelScGa" runat="server" Text="Scadenza Garanzia" TabIndex="5" Checked="False" AutoPostBack="True" />
                        <asp:CheckBox ID="chkSelScEl" runat="server" Text="Scadenza Elettrodi" TabIndex="6" Checked="True" AutoPostBack="True" />
                        <asp:CheckBox ID="chkSelScBa" runat="server" Text="Scadenza Batteria" TabIndex="7" Checked="True" AutoPostBack="True" />
                    </td>
                    <td><asp:Button ID="btnVisualizza" runat="server" Text="Visualizza" OnClick="btnVisualizza_Click" /></td>
                </tr>
            </table>
    </asp:Panel>
    <wuc:ElencoEmail ID="WUCElencoEmail" runat="server" />
    </asp:Panel>
    <asp:Panel ID="Panel3" runat="server" Height="25px"><asp:Label ID="lblMessUtente" 
                runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="Selezione/Deselezione E-mail clienti" Width="99%"></asp:Label>
    </asp:Panel> 
    <div style="text-align:center;">    
        <asp:Button ID="btnSelTutti" runat="server" Text="Seleziona tutte le righe" OnClick="btnSelTutti_Click" />
        <asp:Button ID="btnDeselTutti" runat="server" Text="Deseleziona tutte le righe" OnClick="btnDeselTutti_Click" />
        <asp:Button ID="btnModificaMail" runat="server" Text="Modifica E-Mail" OnClick="btnModificaMail_Click" />
        <asp:Button ID="btnStampaElenco" runat="server" Text="Stampa elenco scadenze clienti" OnClick="btnStampaElenco_Click" />  
    </div>
    <br />
    <div style="text-align:center;">
        <asp:Button ID="btnVisualizzaDett" runat="server" Text="Visualizza dettaglio selezione" OnClick="btnVisualizzaDett_Click" />     
        <asp:Button ID="btnCancel" runat="server" Text="Annulla selezione e ritorna all'elenco articoli consumabili" OnClick="btnCancel_Click" />
    </div>
     </ContentTemplate>
 </asp:UpdatePanel>  
</asp:Panel>

