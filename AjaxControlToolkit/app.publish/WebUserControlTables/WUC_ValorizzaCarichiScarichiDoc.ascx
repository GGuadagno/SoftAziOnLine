<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ValorizzaCarichiScarichiDoc.ascx.vb" Inherits="SoftAziOnLine.WUC_ValorizzaCarichiScarichiDoc" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="510px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori" />
        <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn2" runat="server" Elenco="ListaFornitori" Titolo="Elenco Fornitori" />
    <br>
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 338px; width: 927px;" >
                <td class="style1">
                    <asp:Panel ID="PanelDate" style="margin-top: 0px; text-align: center" 
                        runat="server" groupingtext="Selezione" Height="80px" Width="859px">
                        &nbsp;&nbsp;<asp:Label ID="lblAllaData0" runat="server" Height="20px" Width="90px">Dalla data</asp:Label><asp:TextBox ID="txtDataDa" runat="server" MaxLength="10" TabIndex="1" Width="70px"></asp:TextBox>&nbsp;<asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="2" ToolTip="apri il calendario" />
                        <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                            TargetControlID="txtDataDa"></asp:CalendarExtender>
                        &nbsp;
                        <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" ControlToValidate="txtDataDa" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>&nbsp;<asp:TextBox ID="txtDataA" runat="server" MaxLength="10" TabIndex="3" 
                            Width="70px"></asp:TextBox>&nbsp;&nbsp;<asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                            ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="4" 
                            ToolTip="apri il calendario" />
                        <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                            TargetControlID="txtDataA"></asp:CalendarExtender>
                        &nbsp;&nbsp;<asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                            runat="server" ControlToValidate="txtDataA" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        &nbsp;<br />
                        <br />
                        <asp:Label ID="lblMagazzino" runat="server" Width="146px" Height="16px">Magazzino</asp:Label>
                        <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                               AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                               DataTextField="Descrizione" 
                               DataValueField="Codice" Width="545px" TabIndex="2">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                               SelectCommand="SELECT Codice, Descrizione FROM Magazzini ORDER BY Descrizione">
                        </asp:SqlDataSource>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezione" style="margin-top: 0px;" 
                        runat="server" groupingtext="Selezione" Height="113px" Width="859px">
                        <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Width="165px">Dal 
                        codice</asp:Label>
                        <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" 
                            Height="25px" TabIndex="5" Text="?" Visible="true" Width="25px" />
                        &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server" MaxLength="20" 
                            Width="138px" AutoPostBack="True" TabIndex="6"></asp:TextBox>&nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" MaxLength="150" TabIndex="7" 
                            Width="400px"></asp:TextBox><br>
                        <asp:Label ID="lblAl" runat="server" Height="16px" TabIndex="5" Width="165px">Al 
                        codice</asp:Label>
                        <asp:Button ID="btnCercaAnagrafica2" runat="server" class="btnstyle" 
                            Height="25px" TabIndex="8" Text="?" Visible="true" Width="25px" />
                        &nbsp;&nbsp;<asp:TextBox ID="txtCod2" runat="server" MaxLength="20" TabIndex="9" 
                            Width="138px" AutoPostBack="True"></asp:TextBox>&nbsp;&nbsp;<asp:TextBox ID="txtDesc2" runat="server" MaxLength="150" TabIndex="10" 
                            Width="400px"></asp:TextBox><br>
                        <!--<asp:Label ID="Label3" runat="server" Height="16px" TabIndex="9" Width="198px">Seleziona 
                        tutti i fornitori</asp:Label>-->
                        <asp:CheckBox ID="chkTutti" runat="server" AutoPostBack="True" 
                            TabIndex="11" Text="Seleziona tutti i fornitori" TextAlign="Left"/>
                        <br>                     
                    </asp:Panel>
                        
                      <asp:Panel ID="PanelTipoStampa" style="margin-top: 0px;" 
                        runat="server" groupingtext="Selezione tipo riepilogo" Height="101px" 
                        Width="859px">
                          <br />
                          <asp:RadioButton ID="rbFornitoreDocumento" runat="server" Text ="Fornitore/Documento" 
                              GroupName="TipoRpt" AutoPostBack="True" TabIndex="12"/>
                          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                          <asp:RadioButton ID="rbDocumento" runat="server" Text="Documento" GroupName="TipoRpt" 
                              AutoPostBack="True" TabIndex="13"/>
                          &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                          <asp:RadioButton ID="rbSinteticoDoc" runat="server" 
                              Text="Elenco Carichi/Scarichi sintetico" GroupName="TipoRpt" 
                              AutoPostBack="True" TabIndex="14"/>
                          <br />
                          <br />
                        </asp:Panel>   
                        <asp:Panel ID="PanelOrdinamento" style="margin-top: 0px;" 
                        runat="server" groupingtext="Selezione ordinamento" Height="99px" Width="859px">
                            <br />
                            <asp:RadioButton ID="rbOrdinamentoNDoc" runat="server" Text="Numero Documento" 
                                GroupName="TipoOrd" TabIndex="15" />
                            &nbsp;
                            <asp:RadioButton ID="rbOrdinamentoDataDoc" runat="server" Text="Data Documento" 
                                GroupName="TipoOrd" TabIndex="16" />
                            <%--&nbsp;&nbsp;
                            <asp:RadioButton ID="rbOrdinamentoDataConsegna" runat="server" Text="Data Consegna" GroupName="TipoOrd"/>--%>
                            <br />
                            <br />
                        </asp:Panel> 
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
                                    <br />
                                    <asp:Button ID="Button1" runat="server" Text="Button" Visible="False" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </caption>
</table>
    </ContentTemplate>
</asp:UpdatePanel>     
</asp:Panel>