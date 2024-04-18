<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_FattCliDoc.ascx.vb" Inherits="SoftAziOnLine.WUC_FattCliDoc" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="1240px" Height="494px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" EnableViewState="true" UpdateMode="Always" ChildrenAsTriggers="True">
    <ContentTemplate>
     <uc1:ModalPopup ID="ModalPopup" runat="server" />
     <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
     <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
            SelectCommand="SELECT * FROM [Regioni] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
     <asp:SqlDataSource ID="SqlDa_Agenti" runat="server" 
            SelectCommand="SELECT [Codice], [Descrizione] FROM [Agenti]">
        </asp:SqlDataSource>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 150px; width: 1240px;" >
            <tr>
                <td class="style1">
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;border-style:groove;" runat="server" GroupingText="Date (Solo anno corrente)" Width="1100px">
                    <asp:Label ID="lblDallaData" runat="server" Height="16px" Width="165px">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" Width="80px" MaxLength="10" 
                                TabIndex="1"></asp:TextBox>
                            &nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="2" />
                                                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                    TargetControlID="txtDataDa">
                                                </asp:CalendarExtender>
                            &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>
                            <asp:TextBox ID="txtDataA" runat="server" Width="80px" MaxLength="10" AutoPostBack="true" TabIndex="3"></asp:TextBox>
                            &nbsp;
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="4" />
                                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                    TargetControlID="txtDataA">
                                                </asp:CalendarExtender>
                     </asp:Panel>
                    <asp:Panel ID="PanelSelezione" style="margin-top: 0px;border-style:groove;" runat="server" groupingtext="Selezione"  Width="1100px">
                        <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Text="Codice Cliente"></asp:Label>
                        <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Height="25px" TabIndex="3" Text="?" Visible="true" Width="25px" />
                        <asp:TextBox ID="txtCod1" runat="server" MaxLength="20" Width="100px" AutoPostBack="true"></asp:TextBox>
                        <asp:TextBox ID="txtDesc1" runat="server"  TabIndex="4" Width="655px" Enabled="false"></asp:TextBox><br>
                        <asp:Label ID="Label5" runat="server" Height="17px" Text="Indicare il Codice Cliente altrimenti saranno stampati TUTTI i Clienti" ForeColor="Blue"></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="PanelTipoStampa" style="margin-top: 0px;border-style:groove;" runat="server" groupingtext="Selezione tipo riepilogo" Width="1100px">
                          <br />
                          <asp:RadioButton ID="rbSinteticoDoc" runat="server" Text="Elenco Fatture/N.C. sintetico" GroupName="TipoRpt" 
                              AutoPostBack="True"/>
                          <asp:CheckBox ID="chkRegioni" runat="server" Text="raggruppato per Regione" AutoPostBack="true" Checked="false" Enabled="false"/>
                          <asp:DropDownList ID="ddlRegioni" runat="server" 
                                AutoPostBack="false" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" 
                                DataValueField="Codice" Width="400px" AppendDataBoundItems="True" Enabled="False">
                                <asp:ListItem Text="" Value="0" Selected="True" >[Tutte le Regioni]</asp:ListItem>
                                </asp:DropDownList>
                          <br /><br />    
                          <asp:RadioButton ID="rbClienteDocumento" runat="server" Text ="Cliente/Documento" 
                              GroupName="TipoRpt" AutoPostBack="True"/>
                          <asp:Label ID="Label1" runat="server" Height="5px" Text=""></asp:Label>
                          <asp:RadioButton ID="rbDocumento" runat="server" Text="Documento" GroupName="TipoRpt" 
                              AutoPostBack="True"/>
                          <asp:Label ID="Label2" runat="server" Height="5px" Text=""></asp:Label>
                          <asp:RadioButton ID="RbDiffFTDT" runat="server" Text="Differenze Fatture con DDT (Sintetico)" GroupName="TipoRpt" 
                              AutoPostBack="True" Visible="false" Checked="false" />
                          <asp:RadioButton ID="RbDTFTDoppi" runat="server" Text="DDT Fatturati in Fatture diverse (Sintetico)" GroupName="TipoRpt" 
                              AutoPostBack="True" Visible="false" Checked="false"/>
                          <asp:RadioButton ID="RbFCNCCCausErr" runat="server" Text="Fatture/N.C. con Codice Causale errata (Sintetico)" GroupName="TipoRpt" 
                              AutoPostBack="True" Visible="false" Checked="false"/>
                          <br /><br />
                          <asp:Panel ID="PanelSelezionaRaggr" style="margin-top: 0px;" runat="server" GroupingText="" Width="1000px">
                                <asp:RadioButton ID="rbClienteDocumentoMarFF" runat="server" Text ="Cliente/Documento (con Margine - Costo FIFO)" 
                                      GroupName="TipoRpt" AutoPostBack="true" />
                                <asp:Label ID="Label6" runat="server" Height="17px" TabIndex="2" Text="" Width="50px"></asp:Label>
                                <asp:RadioButton ID="rbClienteDocumentoMarMP" runat="server" Text ="Cliente/Documento (con Margine - Costo medio ponderato)" 
                                      GroupName="TipoRpt" AutoPostBack="true" /> 
                                <br /><br />
                                <asp:CheckBox ID="chkAgentiMarg" runat="server" Text="raggruppato per Agente" AutoPostBack="true" Checked="false" Enabled="False"/>
                                <asp:DropDownList ID="ddlAgentiMarg" runat="server" DataSourceID="SqlDa_Agenti" 
                                      DataTextField="Descrizione" DataValueField="Codice" Width="300px" 
                                      AppendDataBoundItems="true" Enabled="False">
                                      <asp:ListItem Text="" Value="0" Selected="True" >[Tutti gli Agenti]</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="Label8" runat="server" Height="17px" TabIndex="2" Text="" Width="5px"></asp:Label>
                                <asp:CheckBox ID="chkRegioniMarg" runat="server" Text="oppure per Regione" AutoPostBack="true" Checked="false" Enabled="false"/>
                                <asp:DropDownList ID="DDlRegioniMarg" runat="server" 
                                    AutoPostBack="false" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" 
                                    DataValueField="Codice" Width="300px" AppendDataBoundItems="True" Enabled="False">
                                    <asp:ListItem Text="" Value="0" Selected="True" >[Tutte le Regioni]</asp:ListItem>
                                    </asp:DropDownList>
                          </asp:Panel>
                          <br />
                   </asp:Panel> 
                   <asp:Panel ID="PanelOrdinamento" style="margin-top: 0px;border-style:groove;" 
                        runat="server" groupingtext="Selezione ordinamento" Width="1100px">
                            <asp:RadioButton ID="rbOrdinamentoNDoc" runat="server" Text="Numero Documento" GroupName="TipoOrd" />
                            <asp:Label ID="Label4" runat="server" Height="5px" Text=""></asp:Label>
                            <asp:RadioButton ID="rbOrdinamentoDataDoc" runat="server" Text="Data Documento" GroupName="TipoOrd" />
                    </asp:Panel> 
                    <br />
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
            </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>     
</asp:Panel>