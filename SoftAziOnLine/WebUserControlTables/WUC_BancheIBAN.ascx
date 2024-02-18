<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_BancheIBAN.ascx.vb" Inherits="SoftAziOnLine.WUC_BancheIBAN" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>

<style type="text/css">
    .style1
    {
        width: 506px;
    }
    .style2
    {
        width: 120px;
    }
    .style3
    {
        width: 84px;
    }
    .style4
    {
        width: 117px;
    }
</style>

<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" Visible="True" />
<table style="width:auto;height:auto;">
    <tr>
        <td class="style1" >
            <div id="div1" style="border-style:ridge;border-width:thin">
            <table style="width: 680px"> 
                    <tr>
                    <td align="left" class="style2"></td>
                    <td colspan="3">
                    <asp:Label ID="lblLabelTipoRK" runat="server" Font-Bold="True">Elenco</asp:Label>
                    </td>
                    </tr>         
                    <tr>
                    <td align="left" class="style2">Seleziona</td>
                    <td colspan="3">
                        <asp:DropDownList 
                        ID="ddlBancheIBAN" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="99%"
                            DataSourceID="SqlDSBancheIBAN" DataTextField="DesCompleta" 
                            DataValueField="IBAN" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSBancheIBAN" runat="server" 
                            SelectCommand="SELECT *, Descrizione + ' - ' + IBAN AS DesCompleta FROM [BancheIBAN] ORDER BY [Descrizione]" 
                            UpdateCommand="InsertUpdate_BancheIBAN" 
                            UpdateCommandType="StoredProcedure">
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="IBAN" Type="String" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                                <asp:Parameter Name="ABI" Type="String" />
                                <asp:Parameter Name="CAB" Type="String" />
                                <asp:Parameter Name="ContoCorrente" Type="String" />
                                <asp:Parameter Name="Tipo" Type="String" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>                    
                <tr>
                    <td align="left" class="style2">IBAN</td>
                    <td colspan="3" align="left">
                        <asp:TextBox ID="TxtIBAN" AutoPostBack="true" runat="server" Width="200px" 
                            MaxLength="27"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Descrizione</td>
                    <td colspan="3" align="left">
                        <asp:TextBox ID="TxtDescrizione" runat="server" Width="400px" 
                            MaxLength="50" AutoPostBack="True"></asp:TextBox>
                    </td>
                </tr>           
                <tr>
                    <td align="left" class="style2">ABI</td>
                    <td align="left" class="style4">
                        <asp:TextBox ID="TxtABI" runat="server" 
                            Width="50px" MaxLength="5" /></td>
                    <td align="right" style="width:100px">CAB</td>
                    <td align="left">
                        <asp:TextBox ID="TxtCAB" runat="server" 
                            Width="50px" MaxLength="5" /></td>                                           
                </tr>
                <tr>
                    <td align="left" class="style2">Conto corrente</td>
                    <td align="left" class="style3" colspan ="2">
                        <asp:TextBox ID="TxtContoCorrente" runat="server" 
                            Width="100px" MaxLength="12" /></td>                    
                    <td align="left">&nbsp;</td>                                           
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>
    