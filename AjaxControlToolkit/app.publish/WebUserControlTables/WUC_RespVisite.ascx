<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_RespVisite.ascx.vb" Inherits="SoftAziOnLine.WUC_RespVisite" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<style type="text/css">
    .style1
    {
        width: 506px;
    }
    .style2
    {
        width: 80px;
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
                        <asp:DropDownList ID="ddlRespVisiteTABRV" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="100%"
                            DataSourceID="SqlDSRespVisite" DataTextField="DesRespVisiteArea" 
                            DataValueField="Codice" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSRespVisite" runat="server" 
                            SelectCommand="Get_RespVisiteArea" 
                            UpdateCommand="InsertUpdate_RespVisite" 
                            UpdateCommandType="StoredProcedure">
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Codice" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                                <asp:Parameter Name="Residenza" Type="String" />
                                <asp:Parameter Name="Localita" Type="String" />
                                <asp:Parameter Name="Provincia" Type="String" />
                                <asp:Parameter Name="Partita_IVA" Type="String" />
                                <asp:Parameter Name="Codice_CoGe" Type="String" />
                                <asp:Parameter Name="Email" Type="String" />
                                <asp:Parameter Name="CodRespArea" Type="Int32" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Codice</td>
                    <td align="left">
                    <asp:TextBox ID="txtCodice" AutoPostBack="true" runat="server" Width="80px" BorderStyle="None"
                            MaxLength="5"></asp:TextBox>
                    </td>
                </tr>               
                <tr>
                    <td align="left" class="style2">Descrizione</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtDescrizione" AutoPostBack="true" runat="server" Width="100%" BorderStyle="None"
                            MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Residenza</td>
                    <td colspan="3" style="margin-left: 40px">
                        <asp:TextBox ID="TxtResidenza" runat="server" Width="100%" 
                            MaxLength="50" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Localit&agrave;</td>
                    <td align="left" colspan="3" style="margin-left: 280px">
                        <asp:TextBox ID="TxtLocalita" runat="server" Width="100%" 
                            MaxLength="50" BorderStyle="None"/></td>                                            
                </tr>                
                <tr>
                    <td align="left" class="style2">Partita IVA</td>
                    <td align="left" class="style3"><asp:TextBox ID="txtPartitaIVA" runat="server" 
                            Width="155px" MaxLength="11" BorderStyle="None"/></td>
                    <td align="right" style="width:100px">Provincia</td>
                    <td align="left" class="style3">
                        <asp:TextBox ID="TxtProvincia" runat="server" 
                            Width="50px" MaxLength="2" BorderStyle="None"/></td>                                           
                </tr>
                <tr>
                    <td align="left" class="style2">E-Mail</td>
                    <td align="left" colspan="3" style="margin-left: 280px">
                        <asp:TextBox ID="TxtEmail" runat="server" Width="100%" 
                            MaxLength="310" BorderStyle="None" /></td>                                            
                </tr>  
                <tr>
                    <td align="left" class="style2">Codice CoGe</td>
                    <td align="left"><asp:TextBox ID="txtCodice_CoGe" runat="server" Width="155px" BorderStyle="None"
                            MaxLength="16"/></td>
                    <td align="right" style="width:100px">Resp.Area</td>
                    <td align="left" style="width:200px">
                        <asp:DropDownList ID="DDLRespAreaTABRV" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="false" Width="100%"
                            DataSourceID="SqlDSRespArea" DataTextField="Descrizione" 
                            DataValueField="Codice" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSRespArea" runat="server" 
                            SelectCommand="SELECT * FROM [RespArea] ORDER BY [Descrizione]"> 
                        </asp:SqlDataSource>
                    </td>
                    <%--<td align="left"><asp:Button ID="btnTrovaFor" runat="server" Text="?" Width="30px" Height="22px" /></td>--%>
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>
