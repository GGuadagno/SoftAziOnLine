<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_OrdinatoPerClienteAG.ascx.vb" Inherits="SoftAziOnLine.WUC_OrdinatoPerClienteAG" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
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
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn2" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
    <br>
    <br>
    <br>
    <br>
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 260px; width: 927px;" >
                <td>
                    <asp:Panel ID="PanelSelezionaAgente" style="margin-top: 0px;" runat="server" GroupingText="Selezione agente">
                    <table width="100%">
                        <tr>
                            <td align="left">Singolo agente</td><td>
                            <asp:DropDownList ID="ddlAgenti" runat="server" DataSourceID="SqlDa_Agenti" 
                                DataTextField="Descrizione" DataValueField="Codice" Width="400px" 
                                AppendDataBoundItems="true" Enabled="False">
                                <asp:ListItem Value="0" Text="Agente non definito"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDa_Agenti" runat="server" 
                                SelectCommand="SELECT [Codice], [Descrizione] FROM [Agenti]">
                            </asp:SqlDataSource>
                            </td>
                            <td>
                            <asp:CheckBox ID="chkTuttiAgenti" runat="server" Text="Seleziona tutti gli agenti" AutoPostBack="true" Checked="True" />
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezionaOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Ordinamento">
                    <div>
                        <table width="100%">
                        <tr>
                            <td width="25%">
                            </td>
                            <td width ="25%">
                            <asp:RadioButton ID="rbtnCodice" runat="server" Text="Codice" AutoPostBack="True" GroupName="Tipo" />
                            <!--<asp:Label ID="Label0" runat="server" Width="60px">&nbsp;</asp:Label>-->
                            </td>
                            <td width="25%">
                            <asp:RadioButton ID="rbtnRagSoc" runat="server" Text="Ragione Sociale" AutoPostBack="True" GroupName="Tipo" TabIndex="1" />
                            </td>
                            <td width="25%">
                            </td>
                        </tr>
                        </table>
                    </div>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione clienti" style="margin-top: 0px;" Height="136px" Width="859px">
                            <asp:Label ID="lblDal" runat="server" Width="100px" Height="17px" TabIndex="2">Dal codice</asp:Label>
                            <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="3" Visible="true" Text="?" ToolTip="Ricerca clienti"/>
                            <asp:TextBox ID="txtCodCli1" runat="server"  Width="100px" MaxLength="20" AutoPostBack="True" ></asp:TextBox>
                            <asp:TextBox ID="txtDesc1" runat="server" Width="400px" MaxLength="150" TabIndex="4"  ></asp:TextBox>
                            <br>
                            <asp:Label ID="lblAl" runat="server" Width="100px" Height="16px" TabIndex="5">Al codice</asp:Label>
                            <asp:Button ID="btnCercaAnagrafica2" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="6" Visible="true" Text="?" ToolTip="Ricerca clienti" />
                            <asp:TextBox ID="txtCodCli2" runat="server"  Width="100px" MaxLength="20" TabIndex="7" AutoPostBack="True" ></asp:TextBox>
                            <asp:TextBox ID="txtDesc2" runat="server" Width="400px" MaxLength="150" TabIndex="8"  ></asp:TextBox>
                        <br />
                        <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" TabIndex="10" Text="Seleziona tutti i clienti" TextAlign="Left"/>
                            <br>
                        </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" 
                                         TabIndex="20" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="21" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </caption>
            </table>
    </ContentTemplate>
 </asp:UpdatePanel>    
</asp:Panel>