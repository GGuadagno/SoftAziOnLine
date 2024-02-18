<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_PrevClienteOrdineAG.ascx.vb" Inherits="SoftAziOnLine.WUC_PrevClienteOrdineAG" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
<style type="text/css">
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
    .style1
    {
        height: 125px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="550px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn2" runat="server" Elenco="ListaFornitori" Titolo="Elenco fornitori" />
    <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
            SelectCommand="SELECT -1 AS Codice, '' AS Descrizione UNION ALL SELECT * FROM [Regioni] ORDER BY [Descrizione]">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDSProvince" runat="server" 
            SelectCommand="SELECT '' AS Codice, 'Tutte le province' AS Descrizione UNION ALL SELECT [Codice], [Descrizione] FROM [Province] WHERE [Regione] = @Regione ORDER BY [Codice]">
            <SelectParameters>
                <asp:SessionParameter Name="Regione" DbType="Int32" Direction="Input" SessionField="CodRegione" />
            </SelectParameters>
    </asp:SqlDataSource>    
  <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 100%; width: 927px;" >
                <td class="style1">
                    <asp:Panel ID="PanelSelezionaAgente" style="margin-top: 0px;" runat="server" GroupingText="Selezione: Agente">
                    <table width="100%">
                        <tr>
                            <td align="left">Agente    </td><td>
                            <asp:DropDownList ID="ddlAgenti" runat="server" DataSourceID="SqlDa_Agenti" 
                                DataTextField="Descrizione" DataValueField="Codice" Width="400px" 
                                AppendDataBoundItems="true" Enabled="true">
                                <asp:ListItem Value="0" Text="Agente non definito"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDa_Agenti" runat="server" 
                                SelectCommand="SELECT [Codice], [Descrizione] FROM [Agenti]">
                            </asp:SqlDataSource>
                            </td>
                            <td>
                            <asp:CheckBox ID="chkTuttiAgenti" runat="server" Text="Seleziona tutti" AutoPostBack="true" Checked="false" />
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelLead" style="margin-top: 0px;" runat="server" GroupingText="Raggruppato per: Lead Source/Cliente/Preventivo">
                    <table width="100%">
                        <tr>
                            <td align="left">Lead Source</td><td>
                            <asp:DropDownList ID="DDLLead" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="false" DataSourceID="SqlDSLead" DataTextField="Descrizione" 
                                DataValueField="Codice" Height="22px" Enabled="true" Width="400px">
                                <asp:ListItem Value="0" Text="Non definito"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDSLead" runat="server" 
                                SelectCommand="SELECT * FROM [LeadSource] ORDER BY [Descrizione]">
                            </asp:SqlDataSource>
                            </td>
                            <td>
                            <asp:CheckBox ID="chkTuttiLead" runat="server" Text="Seleziona tutti" AutoPostBack="true" Checked="false" />
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelRegione" style="margin-top: 0px;" runat="server" GroupingText="Selezione: Regioni/Province" Height="68px">
                        <table width="100%">
                            <tr valign="top">
                                <td style="width:50%;">
                                    <asp:Label ID="Label1" runat="server" Width="60px" Height="16px">Regione</asp:Label>
                                    <asp:DropDownList ID="ddlRegioni" runat="server" 
                                    AutoPostBack="true" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" 
                                    DataValueField="Codice" Width="200px" Enabled="false">
                                    <asp:ListItem Text="" Value="" ></asp:ListItem>
                                    </asp:DropDownList><br />
                                    <asp:CheckBox ID="chkTutteRegioni" Text="Seleziona tutte le regioni" runat="server" AutoPostBack="True" Checked="true"/>
                                </td>
                                <td>
                                    <asp:Label ID="Label8" runat="server" Width="70px" Height="16px">Provincia</asp:Label>
                                    <asp:DropDownList ID="ddlProvince" runat="server" 
                                    AutoPostBack="false" DataSourceID="SqlDSProvince" DataTextField="Descrizione" 
                                    DataValueField="Codice" Width="200px" Enabled="false" AppendDataBoundItems="false">
                                    <asp:ListItem Text="" Value="" ></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                       </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezioneCli" style="margin-top: 0px;" runat="server" groupingtext="Selezione: Cliente" Height="113px" Width="859px" Visible="false">
                        <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Width="100px">Dal codice</asp:Label>
                        <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Height="25px" TabIndex="3" Text="?" Visible="true" Width="25px" />
                        <asp:TextBox ID="txtCodCli1" runat="server" MaxLength="20" Width="100px" AutoPostBack="True"></asp:TextBox>
                        <asp:TextBox ID="txtDesc1" runat="server" MaxLength="150" TabIndex="4" Width="400px"></asp:TextBox>
                        <br>
                        <asp:Label ID="lblAl" runat="server" Height="16px" TabIndex="5" Width="100px">Al codice</asp:Label>
                        <asp:Button ID="btnCercaAnagrafica2" runat="server" class="btnstyle" Height="25px" TabIndex="6" Text="?" Visible="true" Width="25px" />
                        <asp:TextBox ID="txtCodCli2" runat="server" MaxLength="20" TabIndex="7" Width="100px" AutoPostBack="True"></asp:TextBox>
                        <asp:TextBox ID="txtDesc2" runat="server" MaxLength="150" TabIndex="8" Width="400px"></asp:TextBox>
                        <br>
                        <asp:Label ID="Label3" runat="server" Height="16px" TabIndex="9" Width="198px">Seleziona tutti i clienti</asp:Label>
                        <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" TabIndex="10" />
                        <br>                     
                    </asp:Panel>
                    <br>
                    <asp:Panel ID="PanelSelezionaPrev" style="margin-top: 0px;" runat="server" groupingtext="Selezione: Date Periodo - Stato Preventivi" Height="95px" Width="859px">
                    <div>
                        <asp:Label ID="lblDallaData" runat="server" Height="16px" Width="90px">Dalla data</asp:Label>
                        <asp:TextBox ID="txtDataDa" runat="server" Width="100px" MaxLength="10" TabIndex="1"></asp:TextBox>
                        &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" 
                                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                ToolTip="apri il calendario" TabIndex="2" CausesValidation="False" />
                                            <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                TargetControlID="txtDataDa"></asp:CalendarExtender>
                                            &nbsp;
                                            <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                ControlToValidate="txtDataDa" ErrorMessage="*" 
                                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>
                        <asp:TextBox ID="txtDataA" runat="server" Width="100px" MaxLength="10" TabIndex="3"></asp:TextBox>
                        &nbsp;
                        <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                ToolTip="apri il calendario" TabIndex="4" CausesValidation="False"/>
                                            <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                TargetControlID="txtDataA"></asp:CalendarExtender>
                                            &nbsp;
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                ControlToValidate="txtDataA" ErrorMessage="*" 
                        ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                    </div>
                    <div style="height:5px"></div>
                    <div>
                        <asp:RadioButton ID="rbtnConfermati" runat="server" Text="Confermati" AutoPostBack="false" GroupName="Tipo" />
                        <asp:Label ID="Label0" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnDaConfermare" runat="server" Text="Da confermare" AutoPostBack="false" GroupName="Tipo" />
                        <asp:Label ID="Label2" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnNonConferm" runat="server" Text="Non confermabile" AutoPostBack="false" GroupName="Tipo" />
                        <asp:Label ID="Label4" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnChiusoNoConf" runat="server" Text="Chiuso non confermato" AutoPostBack="false" GroupName="Tipo" />
                        <asp:Label ID="Label5" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" AutoPostBack="false" GroupName="Tipo" Checked="true" />
                    </div>
                    </asp:Panel>
                    <asp:Panel ID="PanelFornitori" runat="server" groupingtext="Fornitori" style="margin-top: 0px;" Height="85px" Width="859px">
                            <asp:Label ID="lblFornitore" runat="server" Width="165px" Height="17px">Fornitore</asp:Label>
                            <asp:Button ID="btnFornitore" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca fornitore" Enabled="false" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCodFornitore" runat="server" Width="130px" MaxLength="16" AutoPostBack="True" TabIndex="6" Enabled="false" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDescFornitore" runat="server" Width="400px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox>
                        <br>
                        <div>
                            <asp:Label ID="Label6" runat="server" Width="170px" Height="16px">Seleziona tutti i fornitori</asp:Label>
                            <asp:CheckBox ID="chkTuttiFornitori" runat="server" AutoPostBack="True" TabIndex="8" Text="" TextAlign="Left" Checked="true" />
                        </div>
                        </asp:Panel>
                    <asp:Panel ID="PanelArticolo" runat="server" groupingtext="Articoli" style="margin-top: 0px;" Height="110px" Width="859px">
                             <asp:Label ID="Label7" runat="server" Width="165px" Height="17px">Dal codice</asp:Label>
                            <asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca articolo" Enabled="false"/>
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server" Width="130px" MaxLength="20" AutoPostBack="True" TabIndex="9" Enabled="false"></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesArt1" runat="server" Width="400px" MaxLength="150" TabIndex="10" Enabled="False" ></asp:TextBox>
                            <asp:Label ID="Label9" runat="server" Width="165px" Height="17px">Al codice</asp:Label>
                            <asp:Button ID="btnCod2" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca articolo" Enabled="false"/>
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod2" runat="server" Width="130px" MaxLength="20" AutoPostBack="True" TabIndex="9" Enabled="false" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesArt2" runat="server" Width="400px" MaxLength="150" TabIndex="10" Enabled="False"></asp:TextBox>
                            <br>
                            <asp:Label ID="lblTuttiArticoli" runat="server" Height="16px" Width="170px">Seleziona tutti gli articoli</asp:Label>
                            <asp:CheckBox ID="chkTuttiArticoli" runat="server" AutoPostBack="True" TabIndex="10" Checked="true" />
                        </asp:Panel>
                    <table width="859px">
                    <tr>
                        <td>
                        <asp:Panel ID="PanelSintAnal" style="margin-top: 0px;" runat="server" groupingtext="Selezione tipo riepilogo" Height="50px" 
                            Width="300px" Visible="true">
                            <asp:RadioButton ID="rbtnSintetico" runat="server" Text="Sintetico" Checked="true" GroupName="SintAnal" AutoPostBack="True"/>
                            &nbsp;
                            <asp:RadioButton ID="rbtnAnalitico" runat="server" Text="Analitico" GroupName="SintAnal" AutoPostBack="True" Checked="false"/>
                        </asp:Panel>
                        </td>
                        <td>
                         <asp:Panel ID="PanelOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Selezione tipo Ordine di stampa" Height="50px" 
                            Width="550px" Visible="true">
                            <asp:RadioButton ID="rbtnOrdAgPrevCli" runat="server" Text="Agente/Preventivo/Cliente" Checked="true" GroupName="OrdineSt" AutoPostBack="false"/>
                            &nbsp;
                            <asp:RadioButton ID="rbtOrdAgCliPrev" runat="server" Text="Agente/Cliente/Preventivo" GroupName="OrdineSt" AutoPostBack="false" Checked="false"/>
                        </asp:Panel>
                        </td>
                    </tr>
                    </table>
                    <br />
                </td>
                    <td align="left" class="style1">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" TabIndex="20" CausesValidation="False"/>
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" TabIndex="21" CausesValidation="False"/>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </caption>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>