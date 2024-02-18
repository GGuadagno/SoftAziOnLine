<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_FatturatoClientiMargineFor.ascx.vb" Inherits="SoftAziOnLine.WUC_FatturatoClientiMargineFor" %>
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
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori" />
    <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
            SelectCommand="SELECT * FROM [Regioni] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
     <asp:SqlDataSource ID="SqlDa_Agenti" runat="server" 
            SelectCommand="SELECT [Codice], [Descrizione] FROM [Agenti]">
        </asp:SqlDataSource>
    <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 150px; width: 1240px;" >
            <tr>
                <td class="style1">
                <br />
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
                    <asp:Panel ID="PanelSelezione" style="margin-top: 0px;border-style:groove;" 
                        runat="server" groupingtext="Selezione" Height="115px" Width="1100px">
                        <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Width="165px">Fornitore</asp:Label>
                        <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" 
                            Height="25px" TabIndex="3" Text="?" Visible="true" Width="25px" />
                        &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server" MaxLength="20" 
                            Width="138px" AutoPostBack="True"></asp:TextBox>&nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" MaxLength="150" TabIndex="4" 
                            Width="462px"></asp:TextBox><br>
                        <br>       
                        <asp:CheckBox ID="chkStampaSintetica" runat="server" AutoPostBack="true" TabIndex="8" Text="Stampa sintetica per Fornitore" TextAlign="Left" Checked="false" />              
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="PanelTipoStampa" style="margin-top: 0px;border-style:groove;" runat="server" groupingtext="Selezione tipo riepilogo" Width="1100px">
                          <br />
                           <asp:RadioButton ID="rbClienteDocumentoMarFF" runat="server" Text ="Cliente/Documento (con Margine - Costo FIFO)" 
                              GroupName="TipoRpt" AutoPostBack="true" Checked="true" />
                           <asp:Label ID="Label6" runat="server" Height="17px" TabIndex="2" Text="" Width="50px"></asp:Label>
                           <asp:RadioButton ID="rbClienteDocumentoMarMP" runat="server" Text ="Cliente/Documento (con Margine - Costo medio ponderato)" 
                              GroupName="TipoRpt" AutoPostBack="true" />
                          <br /><br />
                          <asp:CheckBox ID="chkAgentiMarg" runat="server" Text="raggruppato per Agente" AutoPostBack="true" Checked="false" Enabled="true"/>
                          <asp:DropDownList ID="ddlAgentiMarg" runat="server" DataSourceID="SqlDa_Agenti" 
                              DataTextField="Descrizione" DataValueField="Codice" Width="300px" 
                              AppendDataBoundItems="true" Enabled="False">
                              <asp:ListItem Text="" Value="0" Selected="True" >[Tutti gli Agenti]</asp:ListItem>
                          </asp:DropDownList>
                          <asp:Label ID="Label8" runat="server" Height="17px" TabIndex="2" Text="" Width="5px"></asp:Label>
                          <asp:CheckBox ID="chkRegioniMarg" runat="server" Text="oppure per Regione" AutoPostBack="true" Checked="false" Enabled="true"/>
                          <asp:DropDownList ID="DDlRegioniMarg" runat="server" 
                            AutoPostBack="false" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" 
                            DataValueField="Codice" Width="300px" AppendDataBoundItems="True" Enabled="False">
                            <asp:ListItem Text="" Value="0" Selected="True" >[Tutte le Regioni]</asp:ListItem>
                            </asp:DropDownList>
                          <br /><br />
                        </asp:Panel> 
                        <br />
                </td>
                    <td align="left" class="style1">
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
            </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>     
</asp:Panel>