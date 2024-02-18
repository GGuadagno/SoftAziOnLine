<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Scaffali.ascx.vb" Inherits="SoftAziOnLine.WUC_Scaffali" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<style type="text/css">
    .style1
    {
        width: 506px;
    }
    .style2
    {
        width: 100px;
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
                    <td align="left" class="style2">Magazzino</td>
                    <td colspan="3">
                    <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                            AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                            DataTextField="Descrizione" 
                            DataValueField="Codice" Width="100%">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                            SelectCommand="SELECT Codice, Descrizione FROM Magazzini ORDER BY Descrizione">
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Reparto</td>
                    <td colspan="3">
                        <asp:DropDownList ID="DDLReparto" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="100%"
                            DataSourceID="SqlDSReparto" DataTextField="Descrizione" 
                            DataValueField="Codice" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSReparto" runat="server" 
                            SelectCommand="SELECT Magazzino, Codice, Cod_utente, Descrizione FROM Reparti WHERE Magazzino=@IDMagazzino ORDER BY Descrizione" SelectCommandType="Text"> 
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="0" Name="IDMagazzino" SessionField="IDMagazzino" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Scaffale</td>
                    <td colspan="3">
                        <asp:DropDownList ID="DDLScaffali" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="100%"
                            DataSourceID="SqlDSScaffali" DataTextField="Descrizione" 
                            DataValueField="Scaffale" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSScaffali" runat="server" 
                            SelectCommand="SELECT * FROM [Scaffali] WHERE Magazzino=@IDMagazzino AND Reparto=@IDReparto ORDER BY [Descrizione]" 
                            UpdateCommand="InsertUpdate_Scaffali" 
                            UpdateCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="0" Name="IDMagazzino" SessionField="IDMagazzino" Type="Int32" />
                                <asp:SessionParameter DefaultValue="0" Name="IDReparto" SessionField="IDReparto" Type="Int32" />
                            </SelectParameters>
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Magazzino" Type="Int32" />
                                <asp:Parameter Name="Reparto" Type="Int32" />
                                <asp:Parameter Name="Scaffale" Type="Int32" />
                                <asp:Parameter Name="Cod_utente" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Codice</td>
                    <td align="left">
                    <asp:TextBox ID="txtCodice" AutoPostBack="true" runat="server" Width="80px" 
                            MaxLength="5"></asp:TextBox>
                    </td>
                </tr>               
                <tr>
                    <td align="left" class="style2">Descrizione</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtDescrizione" AutoPostBack="true" runat="server" Width="99%" 
                            MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>
