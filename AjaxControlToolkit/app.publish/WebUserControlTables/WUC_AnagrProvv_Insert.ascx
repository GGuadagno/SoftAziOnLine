<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_AnagrProvv_Insert.ascx.vb" Inherits="SoftAziOnLine.WUC_AnagrProvv_Insert" %>
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
                    <asp:Label ID="lblLabelTipoRK" runat="server" Font-Bold="True" BorderStyle="Groove">Elenco</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Seleziona</td>
                    <td colspan="3">
                        <asp:DropDownList 
                        ID="ddlAnagrProvv" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="99%"
                            DataSourceID="SqlDSAnagrProvv" DataTextField="Ragione_Sociale" 
                            DataValueField="IDAnagrProvv" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSAnagrProvv" runat="server" 
                            SelectCommand="SELECT * FROM [AnagrProvv] WHERE WHERE IDAnagrProvv=-1" 
                            UpdateCommand="InsertUpdate_AnagrProvv" 
                            UpdateCommandType="StoredProcedure">
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Ragione_Sociale" Type="String" />
                                <asp:Parameter Name="Codice_Fiscale" Type="String" />
                                <asp:Parameter Name="Partita_IVA" Type="String" />
                                <asp:Parameter Name="Indirizzo" Type="String" />
                                <asp:Parameter Name="CAP" Type="String" />
                                <asp:Parameter Name="Localita" Type="String" />
                                <asp:Parameter Name="Provincia" Type="String" />
                                <asp:Parameter Name="Stato" Type="String" />
                                <asp:Parameter Name="Tipo" Type="String" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>               
                <tr>
                    <td align="left" class="style2">Ragione sociale</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtRagioneSoc" AutoPostBack="true" runat="server" Width="99%" 
                            MaxLength="50" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Indirizzo</td>
                    <td colspan="3" style="margin-left: 40px">
                        <asp:TextBox ID="TxtIndirizzo" runat="server" Width="99%" 
                            MaxLength="50" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width:100px">Localit&agrave;</td>
                    <td align="left" colspan="3" style="margin-left: 280px">
                        <asp:TextBox ID="TxtLocalita" runat="server" Width="99%" 
                            MaxLength="50" BorderStyle="None" /></td>                                            
                </tr>                
                <tr>
                    <td align="left" class="style2">CAP</td>
                    <td align="left" class="style3" colspan="3"">
                        <asp:TextBox ID="TxtCAP" runat="server" 
                            Width="80px" MaxLength="5" BorderStyle="None" /></td>
                                       
                </tr>
                <tr>
                    <td align="left" class="style2">Provincia</td>
                    <td align="left" class="style3">
                        <asp:TextBox ID="TxtProvincia" runat="server" 
                            Width="50px" MaxLength="2" BorderStyle="None" /></td>
                    <td align="right" style="width:100px">Nazione</td>
                    <td align="left">
                        <asp:TextBox ID="TxtStato" runat="server" 
                            Width="50px" MaxLength="3" BorderStyle="None" /></td>                                           
                </tr>
                <tr>
                    <td align="left" class="style2">Partita IVA</td>
                    <td align="left" class="style3"><asp:TextBox ID="txtPartitaIVA" runat="server" AutoPostBack="true"
                            Width="100px" MaxLength="11"  BorderStyle="None"/></td>
                    <td align="right" style="width:100px">Codice Fiscale</td>
                    <td align="left"><asp:TextBox ID="txtCodiceFiscale" runat="server" Width="155px" AutoPostBack="true"
                            MaxLength="16" BorderStyle="None"/></td>                                           
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>
    