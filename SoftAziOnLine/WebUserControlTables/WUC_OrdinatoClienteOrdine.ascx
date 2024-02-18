<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_OrdinatoClienteOrdine.ascx.vb" Inherits="SoftAziOnLine.WUC_OrdinatoClienteOrdine" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="494px" 
    BackColor="white">
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
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 338px; width: 927px;" >
                <td class="style1">
                    <asp:Panel ID="PanelSelezione" style="margin-top: 0px;" runat="server" groupingtext="Selezione" Height="113px" Width="859px">
                        <asp:Label ID="lblDal" runat="server" Height="17px" TabIndex="2" Width="165px">Dal codice</asp:Label>
                        <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Height="25px" TabIndex="3" Text="?" Visible="true" Width="25px" />
                        &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server" MaxLength="20" Width="130px" AutoPostBack="True"></asp:TextBox>
                        &nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" MaxLength="150" TabIndex="4" Width="400px"></asp:TextBox>
                        <br>
                        <asp:Label ID="lblAl" runat="server" Height="16px" TabIndex="5" Width="165px">Al codice</asp:Label>
                        <asp:Button ID="btnCercaAnagrafica2" runat="server" class="btnstyle" Height="25px" TabIndex="6" Text="?" Visible="true" Width="25px" />
                        &nbsp;&nbsp;<asp:TextBox ID="txtCod2" runat="server" MaxLength="20" TabIndex="7" Width="130px" AutoPostBack="True"></asp:TextBox>
                        &nbsp;&nbsp;<asp:TextBox ID="txtDesc2" runat="server" MaxLength="150" TabIndex="8" Width="400px"></asp:TextBox>
                        <br>
                        <asp:Label ID="Label3" runat="server" Height="16px" TabIndex="9" Width="198px">Seleziona tutti i clienti</asp:Label>
                        <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" TabIndex="10" />
                        <br>                     
                    </asp:Panel>
                      <asp:Panel ID="PanelTipoStampa" style="margin-top: 0px;" 
                        runat="server" groupingtext="Selezione tipo riepilogo" Height="101px" Width="859px">
                          <br />
                          <asp:RadioButton ID="rbClienteOrdine" runat="server" Text ="Cliente/Ordine" GroupName="TipoRpt" AutoPostBack="True"/>
                         <asp:Label ID="Label1" runat="server" Height="16px" TabIndex="5" Width="15px"></asp:Label>
                          <asp:RadioButton ID="rbOrdine" runat="server" Text="Ordine" GroupName="TipoRpt" AutoPostBack="True"/>
                          <br />
                          <br />
                        </asp:Panel>   
                        <asp:Panel ID="PanelOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Selezione ordinamento" Height="99px" Width="859px">
                            <br />
                            <asp:RadioButton ID="rbOrdinamentoNDoc" runat="server" Text="Numero Documento" GroupName="TipoOrd" />
                            &nbsp;
                            <asp:RadioButton ID="rbOrdinamentoDataDoc" runat="server" Text="Data Documento" GroupName="TipoOrd" />
                            &nbsp;&nbsp;
                            <asp:RadioButton ID="rbOrdinamentoDataConsegna" runat="server" Text="Data Consegna" GroupName="TipoOrd"/>
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