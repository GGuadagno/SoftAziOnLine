﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Categorie.ascx.vb" Inherits="SoftAziOnLine.WUC_Categorie" %>
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
                    <td align="left" class="style2">Seleziona</td>
                    <td colspan="3">
                        <asp:DropDownList ID="ddlCategorie" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="99%"
                            DataSourceID="SqlDSCategorie" DataTextField="Descrizione" 
                            DataValueField="Codice" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSCategorie" runat="server" 
                            SelectCommand="SELECT * FROM [Categorie] ORDER BY [Descrizione]" 
                            UpdateCommand="InsertUpdate_Categorie" 
                            UpdateCommandType="StoredProcedure">
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Codice" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                                <asp:Parameter Name="InvioMailSc" Type="Boolean" DefaultValue="False" />
                                <asp:Parameter Name="SelSc" Type="Boolean" DefaultValue="False" />
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
                <tr>
                    <td align="left" class="style2"></td>
                    <td align="left">
                        <asp:CheckBox ID="chkInvioMailSc" AutoPostBack="true" runat="server" Text="Invio E-mail scadenze" />
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2"></td>
                    <td align="left">
                        <asp:CheckBox ID="chkSelSc" AutoPostBack="true" runat="server" Text="Selezione per invio E-mail scadenze" />
                    </td>
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>
