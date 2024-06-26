﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_OrdinatoPerCliente.ascx.vb" Inherits="SoftAziOnLine.WUC_OrdinatoPerCliente" %>
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
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 260px; width: 927px;">
                <td>
                    <asp:Panel ID="PanelSelezionaOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Ordinamento">
                    <div>
                        <table width="100%">
                        <tr>
                            <td width="25%">
                            </td>
                            <td width ="25%">
                            <asp:RadioButton ID="rbtnCodice" runat="server" Text="Codice" AutoPostBack="false" 
                            GroupName="Tipo" />
                            </td>
                            <td width="25%">
                            <asp:RadioButton ID="rbtnRagSoc" runat="server" Text="Ragione Sociale" 
                            AutoPostBack="false" GroupName="Tipo" TabIndex="1" />
                            </td>
                            <td width="25%">
                            </td>
                        </tr>
                        </table>
                    </div>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione" style="margin-top: 0px;" Height="136px" Width="859px">
                            <asp:Label ID="lblDal" runat="server" Width="165px" Height="17px" TabIndex="2">Dal codice</asp:Label>
                            <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="3" Visible="true" Text="?"/>
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server" Width="130px" MaxLength="20" AutoPostBack="True" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesc1" runat="server" Width="400px" MaxLength="150" TabIndex="4"  ></asp:TextBox>
                            <br>
                            <asp:Label ID="lblAl" runat="server" Width="165px" Height="16px" TabIndex="5">Al codice</asp:Label>
                            <asp:Button ID="btnCercaAnagrafica2" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="6" Visible="true" Text="?" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod2" runat="server"  Width="130px" MaxLength="20" TabIndex="7" AutoPostBack="True" ></asp:TextBox>
                            &nbsp;&nbsp;<asp:TextBox ID="txtDesc2" runat="server" Width="400px" MaxLength="150" TabIndex="8"  ></asp:TextBox>
                        <br>
                        <asp:Label ID="Label3" runat="server" Width="198px" Height="16px" TabIndex="9">Seleziona tutti i clienti</asp:Label>
                        <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" TabIndex="10" />
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