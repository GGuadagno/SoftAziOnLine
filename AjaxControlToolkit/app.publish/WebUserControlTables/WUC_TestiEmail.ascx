<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_TestiEmail.ascx.vb" Inherits="SoftAziOnLine.WUC_TestiEmail" %>
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
                    <asp:Label ID="lblLabelTipoRK" runat="server" Font-Bold="True">Gestione testi e-mail</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Seleziona</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:DropDownList ID="ddlTesti" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="99%"
                            DataSourceID="SqlDSTesti" DataTextField="Descrizione" 
                            DataValueField="ID" Height="22px">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        
                        <asp:SqlDataSource ID="SqlDSTesti" runat="server" 
                            SelectCommand="SELECT * FROM [Testi] ORDER BY [Descrizione]" 
                            UpdateCommand="InsertUpdate_TestiEmail" 
                            UpdateCommandType="StoredProcedure">
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="ID" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                                <asp:Parameter Name="Oggetto" Type="String" />
                                <asp:Parameter Name="Corpo" Type="String" />
                                <asp:Parameter Name="PiePagina" Type="String" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
            <%--<tr>
                    <td align="left" class="style2">Descrizione</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtDescrizione" AutoPostBack="true" runat="server" Width="99%" height="50%" 
                            MaxLength="15" TextMode="SingleLine"></asp:TextBox>
                    </td>
                </tr> --%>
                <tr>
                    <td align="left" class="style2">Oggetto</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtOggetto" runat="server" Width="99%" height="50%" 
                            MaxLength="80" TextMode="SingleLine"></asp:TextBox>
                    </td>
                </tr>               
                <tr>
                    <td align="left" class="style2">Corpo</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtCorpo" runat="server" Width="99%" height="50%" 
                            MaxLength="256" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Pi&egrave di pagina</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtPiePagina" runat="server" Width="99%" height="50%" 
                            MaxLength="256" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>
