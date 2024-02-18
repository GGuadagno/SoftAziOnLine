<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Moduli.ascx.vb" Inherits="SoftAziOnLine.WUC_Moduli" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>

<style type="text/css">
    .style1
    {
        width: 600px;
    }
    .style2
    {
        width: 100px;
    }
    </style>

<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" Visible="True" />
<table style="width:auto;height:auto;">
    <tr>
        <td class="style1" >
            <div id="div1" style="border-style:ridge; border-width:thin">
            <table style="width: 800px"> 
                <tr>
                    <td align="left" class="style2"></td>
                    <td colspan="3">
                    <asp:Label ID="lblLabelTipoRK" runat="server" Font-Bold="True">Gestione allegati E-Mail scadenze</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Seleziona</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:DropDownList ID="ddlModuli" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="99%"
                            DataSourceID="SqlDSModuli" DataTextField="Tipo" 
                            DataValueField="ID" Height="22px">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        
                        <asp:SqlDataSource ID="SqlDSModuli" runat="server" 
                            SelectCommand="SELECT * FROM [Moduli] ORDER BY [Tipo]"
                            InsertCommand="InsertUpdate_Moduli" 
                            InsertCommandType="StoredProcedure"
                            UpdateCommand="InsertUpdate_Moduli" 
                            UpdateCommandType="StoredProcedure">
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="ID" Type="Int32" />
                                <asp:Parameter Name="Tipo" Type="String" />
                                <asp:Parameter Name="Percorso" Type="String" />
                            </UpdateParameters>
                            <InsertParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="ID" Type="Int32" />
                                <asp:Parameter Name="Tipo" Type="String" />
                                <asp:Parameter Name="Percorso" Type="String" />
                            </InsertParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <%--<tr>
                    <td align="left" class="style2">Tipo</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtTipo" AutoPostBack="true" runat="server" Width="99%" height="50%" 
                            MaxLength="50" TextMode="SingleLine"></asp:TextBox>
                    </td>
                </tr>--%> 
                <tr>
                    <td align="left" class="style2">Percorso</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtPercorso" runat="server" Width="99%" height="50%" 
                            MaxLength="300" TextMode="SingleLine"></asp:TextBox>
                    </td>
                </tr>               
            </table>
            </div>
        </td>
    </tr>
</table>
