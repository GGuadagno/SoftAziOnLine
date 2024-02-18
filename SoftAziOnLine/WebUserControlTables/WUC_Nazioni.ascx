<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Nazioni.ascx.vb" Inherits="SoftAziOnLine.WUC_Nazioni" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<style type="text/css">
    .style1
    {
        width: 506px;
    }
    .style2
    {
        width: 140px;
    }
    .style3
    {
        width: 76px;
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
                        <asp:DropDownList ID="DDLNazioni" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="550px"
                            DataSourceID="SqlDSNazioni" DataTextField="Descrizione" 
                            DataValueField="Codice" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSNazioni" runat="server" 
                            SelectCommand="SELECT * FROM [Nazioni] ORDER BY [Descrizione]" 
                            UpdateCommand="InsertUpdate_Nazioni" 
                            UpdateCommandType="StoredProcedure">
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Codice" Type="String" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                                <asp:Parameter Name="Codice_ISO" Type="String" />
                                <asp:Parameter Name="CUnico" Type="String" />
                                <asp:Parameter Name="Prefisso" Type="String" />
                                <asp:Parameter Name="Prodotto_DHL" Type="String" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Codice</td>
                    <td align="left">
                    <asp:TextBox ID="txtCodice" AutoPostBack="true" runat="server" Width="50px" 
                            MaxLength="3"></asp:TextBox>
                    </td>
                </tr>               
                <tr>
                    <td align="left" class="style2">Descrizione</td>
                    <td colspan="3">
                        <asp:TextBox ID="txtDescrizione" AutoPostBack="true" runat="server" Width="545px" 
                            MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Codice ISO</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtCodiceISO" runat="server" Width="50px" 
                            MaxLength="2" /></td>                                            
                </tr>
                <tr>
                    <td align="left" class="style2">Codice CUnico</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtCUnico" runat="server" Width="50px" 
                            MaxLength="2" /></td>                                            
                </tr>                
                <tr>
                    <td align="left" class="style2">Prefisso</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtPrefisso" runat="server" Width="100px" 
                            MaxLength="10" /></td>                                         
                </tr>
                <tr>
                    <td align="left" class="style2">Prodotto DHL</td>
                    <td align="left" colspan="3">
                        <asp:DropDownList ID="DDLProdottoDHL" runat="server" AutoPostBack="false" Width="550px" Height="22px">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="ITALIA" Value="DOM"></asp:ListItem>
                        <asp:ListItem Text="EUROPA" Value="ECX"></asp:ListItem>
                        <asp:ListItem Text="DOCUMENTI EXTRACEE" Value="DOX"></asp:ListItem>
                        <asp:ListItem Text="MATERIALE EXTRACEE" Value="WPX"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>