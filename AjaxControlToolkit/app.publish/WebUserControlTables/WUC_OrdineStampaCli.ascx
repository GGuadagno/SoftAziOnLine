<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_OrdineStampaCli.ascx.vb" Inherits="SoftAziOnLine.WUC_OrdineStampaCli" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WUC_SceltaStampaCFRubrica.ascx" tagname="WUC_SceltaStampaCFRubrica" tagprefix="uc3" %>
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
    .style8
    {
        width: 21px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="1240px" Height="495px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc3:WUC_SceltaStampaCFRubrica ID="WUC_SceltaStampaCFRubrica1" runat="server" />
    <br>
    <br>
    <br>
       <%-- <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
            SelectCommand="SELECT * FROM [Regioni] ORDER BY [Descrizione]" SelectCommandType="Text"></asp:SqlDataSource>--%>
        <asp:SqlDataSource ID="SqlDSCategorie" runat="server" 
            SelectCommand="SELECT * FROM Categorie ORDER BY Descrizione" SelectCommandType="Text">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSProvince" runat="server" 
            SelectCommand="SELECT * FROM Province ORDER BY Descrizione" SelectCommandType="Text"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSZone" runat="server" 
            SelectCommand="SELECT * FROM Zone ORDER BY Descrizione" SelectCommandType="Text"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSAgenti" runat="server" 
            SelectCommand="SELECT * FROM Agenti ORDER BY Descrizione" SelectCommandType="Text"></asp:SqlDataSource>
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti"/>
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn2" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti"/>
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn3" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori"/>
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn4" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori"/>
    <br>
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 200px; width: 1240px;">
                <td>
                    <asp:Panel ID="PanelSelezionaOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Ordinamento" Width="100%">
                    <div>
                        <table width="100%">
                        <tr>
                            <td width="25%">
                                <asp:RadioButton ID="rbtnLocalita" runat="server" AutoPostBack="False" 
                                    GroupName="Tipo" TabIndex="1" Text="Località" Visible="False" />
                            </td>
                            <td width="144px">
                                <asp:RadioButton ID="rbtnCodice" runat="server" AutoPostBack="False" 
                                    GroupName="Tipo" Text="Codice" Checked="True" />
                            </td>
                            <td width="25%">
                            <asp:RadioButton ID="rbtnRagSoc" runat="server" Text="Ragione Sociale" 
                            AutoPostBack="False" GroupName="Tipo" />
                                <%--<asp:Label ID="Label0" runat="server" Width="60px">&nbsp;</asp:Label>--%>
                            </td>
                            
                            
                            <td>
                            <asp:RadioButton ID="rbtnPIva" runat="server" Text="Partita IVA" AutoPostBack="False" 
                            GroupName="Tipo" TabIndex="1" Visible="False" />
                            </td>
                            
                        </tr>
                        </table>
                    </div>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione" 
                            style="margin-top: 0px;" Height="180px" Width="100%">
                            <table width="100%">
                                <tr>
                                    <td colspan="3">
                                        <table>
                                            <tr align="left">
                                                <td><asp:Label ID="lblDal" runat="server" TabIndex="2">Dal codice</asp:Label></td>
                                                <td class="style8">
                                                    <asp:Button ID="btnCercaAnagrafica1" runat="server" CausesValidation="False" 
                                                        CommandName="btnCercaAnagrafica" Text="?" ToolTip="Ricerca anagrafiche" />
                                                </td>
                                                <td><asp:TextBox ID="txtCod1" runat="server" Width="138px" MaxLength="20" 
                                                        style="margin-left: 5px" AutoPostBack="True" ></asp:TextBox></td>
                                                <td><asp:TextBox ID="txtDesc1" runat="server" Width="550px" MaxLength="150" 
                                                        TabIndex="4" Enabled="False"  ></asp:TextBox></td>
                                            </tr>
                                            <tr align="left">
                                                <td><asp:Label ID="lblAl" runat="server" TabIndex="5">Al codice</asp:Label></td>
                                                <td class="style8">
                                                    <asp:Button ID="btnCercaAnagrafica2" runat="server" CausesValidation="False" 
                                                        CommandName="btnCercaAnagrafica" Text="?" ToolTip="Ricerca anagrafiche" />
                                                </td>
                                                <td><asp:TextBox ID="txtCod2" runat="server" Width="137px" MaxLength="20" 
                                                        TabIndex="7" style="margin-left: 5px" AutoPostBack="True" ></asp:TextBox></td>
                                                <td><asp:TextBox ID="txtDesc2" runat="server" Width="550px" MaxLength="150" 
                                                        TabIndex="8" Enabled="False"  ></asp:TextBox></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table>
                                            <tr align="left">
                                                <td style="width:25%">Categoria</td>
                                                <%--<td style="width:25%">Regione</td>--%>
                                                <td style="width:25%">Provincia</td>
                                                <td style="width:25%">Zona</td>
                                                <td style="width:25%"><asp:Label ID="lblAgente" runat="server" TabIndex="5">Agente</asp:Label></td>
                                            </tr>
                                            <tr align="left">
                                                <td>                                                                                                             
                                                    <asp:DropDownList ID="ddlCategorie" runat="server" 
                                                        DataSourceID="SqlDSCategorie" DataTextField="Descrizione" 
                                                        DataValueField="Codice" Width="200px" AppendDataBoundItems="True">
                                                        <asp:ListItem Text="" Value="0" Selected="True"></asp:ListItem>
                                                    </asp:DropDownList>                                                            
                                                </td>
                                                <%-- <td><asp:DropDownList ID="ddlRegioni" runat="server" Width="200px" 
                                                        DataSourceID="SqlDSRegioni" DataTextField="Descrizione" 
                                                        DataValueField="Codice" AppendDataBoundItems="True" AutoPostBack="true">
                                                    <asp:ListItem 
                                                    Text="" Value="0" Selected="True"></asp:ListItem></asp:DropDownList></td>--%>
                                                <td><asp:DropDownList ID="ddlProvince" runat="server" Width="200px" 
                                                        DataSourceID="SqlDSProvince" DataTextField="Descrizione" 
                                                        DataValueField="Codice" AppendDataBoundItems="True">
                                                    <asp:ListItem 
                                                    Text="" Value="0" Selected="True"></asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList ID="ddlZone" runat="server" Width="200px" 
                                                        DataSourceID="SqlDSZone" DataTextField="Descrizione" 
                                                        DataValueField="Codice" AppendDataBoundItems="True">
                                                    <asp:ListItem 
                                                    Text="" Value="0" Selected="True"></asp:ListItem></asp:DropDownList></td>
                                                <td><asp:DropDownList ID="DDLAgenti" runat="server" Width="200px" 
                                                        DataSourceID="SqlDSAgenti" DataTextField="Descrizione" 
                                                        DataValueField="Codice" AppendDataBoundItems="True">
                                                    <asp:ListItem 
                                                    Text="" Value="0" Selected="True"></asp:ListItem></asp:DropDownList></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                    </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div id="noradio">
                                    <asp:Button ID="btnStampaAn" runat="server" class="btnstyle" Text="Elenco analitico" 
                                         TabIndex="20"/>
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnStampaSint" runat="server" class="btnstyle" Text="Elenco sintetico" 
                                         TabIndex="20"/>
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnStampaRub" runat="server" class="btnstyle" Text="Rubrica" 
                                         TabIndex="20"/>
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnStampaCod" runat="server" class="btnstyle" Text="Elenco codici" 
                                         TabIndex="20"/>
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="21"/>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </caption>
            </table>
            <asp:Panel ID="Panel3" runat="server" Height="25px"><asp:Label ID="lblMessUtente" 
                runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="" Width="99%"></asp:Label>
        </asp:Panel> 
    </ContentTemplate>
 </asp:UpdatePanel>    
</asp:Panel>