<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_FattCliDocAG.ascx.vb" Inherits="SoftAziOnLine.WUC_FattCliDocAG" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="494px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
    <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
            SelectCommand="SELECT * FROM [Regioni] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 338px; width: 927px;" >
            <tr>
                <td class="style1">
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
                    <asp:Panel ID="PanelSelezione" style="margin-top: 0px;" runat="server" groupingtext="Selezione" Height="113px" Width="859px">
                        <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Width="100px">Dal codice</asp:Label>
                        <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Height="25px" TabIndex="3" Text="?" Visible="true" Width="25px" />
                        <asp:TextBox ID="txtCod1" runat="server" MaxLength="20" Width="100px" AutoPostBack="True"></asp:TextBox>
                        <asp:TextBox ID="txtDesc1" runat="server" MaxLength="150" TabIndex="4" Width="400px" Enabled="false"></asp:TextBox>
                        <asp:Label ID="Label5" runat="server" Height="17px" Text="Indicare il Codice Cliente altrimenti saranno stampati TUTTI i Clienti" ForeColor="Blue"></asp:Label>
                    </asp:Panel>
                      <asp:Panel ID="PanelTipoStampa" style="margin-top: 0px;" runat="server" groupingtext="Selezione tipo riepilogo" Height="101px" Width="859px">
                          <asp:RadioButton ID="rbSinteticoDoc" runat="server" Text="Elenco Fatture/N.C. sintetico per " GroupName="TipoRpt" AutoPostBack="True"/>
                          <asp:CheckBox ID="chkRegioni" runat="server" AutoPostBack="True" TabIndex="10" Text="raggruppato per Regione" Checked="false" Enabled="false" />
                            &nbsp;<asp:DropDownList ID="ddlRegioni" runat="server" 
                                AutoPostBack="false" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" 
                                DataValueField="Codice" Width="200px" AppendDataBoundItems="True">
                                <asp:ListItem Text="" Value="0" Selected="True" >[Tutte le regioni]</asp:ListItem>
                                </asp:DropDownList>
                          <br /><br />
                          <asp:RadioButton ID="rbClienteDocumento" runat="server" Text ="Cliente/Documento" GroupName="TipoRpt" AutoPostBack="True"/>
                          <asp:Label ID="Label1" runat="server" Height="16px" TabIndex="5" Width="10px"></asp:Label>
                          <asp:RadioButton ID="rbDocumento" runat="server" Text="Documento" GroupName="TipoRpt" AutoPostBack="True"/>
                          <asp:Label ID="Label2" runat="server" Height="16px" TabIndex="5" Width="10px"></asp:Label>
                          
                        </asp:Panel>   
                        <asp:Panel ID="PanelOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Selezione ordinamento" Height="99px" Width="859px">
                            <br />
                            <asp:RadioButton ID="rbOrdinamentoNDoc" runat="server" Text="Numero Documento" GroupName="TipoOrd" />
                            <asp:Label ID="Label4" runat="server" Height="5px" Text=""></asp:Label>
                            <asp:RadioButton ID="rbOrdinamentoDataDoc" runat="server" Text="Data Documento" GroupName="TipoOrd" />
                            <br />
                            <br />
                        </asp:Panel> 
                </td>
                    <td align="left" class="style1">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
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
            </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>