<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DestCliFor.ascx.vb" Inherits="SoftAziOnLine.WUC_DestCliFor" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" Visible="True" />
<asp:SqlDataSource ID="SqlDSAnagrCliFor" runat="server" 
    SelectCommand="SELECT * FROM [DestClienti] WHERE ([Codice] = @Codice) AND ([Progressivo]=@CodFil)" 
    UpdateCommand="InsertUpdate_DestClienti" 
    UpdateCommandType="StoredProcedure">
    <SelectParameters>
        <asp:SessionParameter Name="Codice" SessionField="Codice_CoGeGDM" Type="String" />
        <asp:SessionParameter Name="CodFil" SessionField="IDDESTCLIFOR" Type="Int32" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
        <asp:Parameter Name="Codice" Type="String" />
        <asp:Parameter Name="Progressivo" Type="Int32" />
        <asp:Parameter Name="Ragione_Sociale" Type="String" />
        <asp:Parameter Name="Indirizzo" Type="String" />
        <asp:Parameter Name="CAP" Type="String" />
        <asp:Parameter Name="Localita" Type="String" />
        <asp:Parameter Name="Provincia" Type="String" />
        <asp:Parameter Name="Stato" Type="String" />
        <asp:Parameter Name="Denominazione" Type="String" />
        <asp:Parameter Name="Riferimento" Type="String" />
        <asp:Parameter Name="Telefono1" Type="String" />
        <asp:Parameter Name="Telefono2" Type="String" />
        <asp:Parameter Name="Fax" Type="String" />
        <asp:Parameter Name="Email" Type="String" />
        <asp:Parameter Name="Tipo" Type="String" />
        <asp:Parameter Name="Ragione_Sociale35" Type="String" />
        <asp:Parameter Name="Riferimento35" Type="String" />
    </UpdateParameters>
</asp:SqlDataSource>
<table style="width:auto;height:auto;">
    <tr>
        <td style="width:700px" >
            <div id="div1" style="border-style:ridge;border-width:thin">
            <table style="width: 680px"> 
                <tr>
                    <td align="left" style="width:120px">
                        <asp:Label ID="lblCodice" runat="server" Font-Bold="True" BorderStyle="Groove" >Codice</asp:Label>
                    </td>
                    <td align="center" colspan="3">
                        <asp:Label ID="lblLabelTipoRK" runat="server" Font-Bold="True" BorderStyle="Groove" Width="99%">Elenco</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width:120px">Sigla</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtTipo" runat="server" Width="50px" MaxLength="5" BorderStyle="None"/></td>
                </tr>
                <tr>
                    <td align="left" style="width:120px">Ragione sociale</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtRagioneSoc" AutoPostBack="false" runat="server" Width="99%" MaxLength="50" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width:120px">Denominazione</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtDenominazione" AutoPostBack="false" runat="server" Width="99%" MaxLength="50" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width:120px">Riferimento</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtRiferimento" AutoPostBack="false" runat="server" Width="99%" MaxLength="500" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width:120px"><asp:Label ID="Label1" runat="server" Text="Dati Corriere" ForeColor="Blue" Font-Bold="true"></asp:Label></td>
                    <td align="left" colspan="3"><asp:Label ID="Label2" runat="server" Text="Ragione sociale" ForeColor="Blue" Width="120px"></asp:Label>
                    <asp:TextBox ID="txtRagSoc35" AutoPostBack="false" runat="server" Width="300px" MaxLength="35" BorderStyle="None" BackColor="Aqua"></asp:TextBox></td>
                </tr>
                <tr>
                    <td align="left" style="width:120px"><asp:Label ID="Label4" runat="server" Text="........................." ForeColor="Blue" Font-Bold="true"></asp:Label></td>
                    <td align="left" colspan="3"><asp:Label ID="Label3" runat="server" Text="Riferimento" ForeColor="Blue" Width="120px"></asp:Label>
                    <asp:TextBox ID="txtRiferimento35" AutoPostBack="false" runat="server" Width="300px" MaxLength="35" BorderStyle="None" BackColor="Aqua"></asp:TextBox></td>
                </tr>
                <tr>
                    <td align="left" style="width:120px">Indirizzo</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="TxtIndirizzo" runat="server" Width="99%" MaxLength="50" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width:120px">Localit&agrave;</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="TxtLocalita" runat="server" Width="99%" MaxLength="50"  BorderStyle="None"/></td>                                            
                </tr>                
                <tr>
                    <td align="left" style="width:120px">CAP</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="TxtCAP" runat="server" Width="50px" MaxLength="5" BorderStyle="None" /></td>
                </tr>
                <tr>
                    <td align="left" style="width:120px">Provincia</td>
                    <td align="left">
                        <asp:TextBox ID="TxtProvincia" runat="server" Width="50px" MaxLength="2" AutoPostBack="false" BorderStyle="None"/></td>
                    <td align="right" style="width:100px">Nazione</td>
                    <td align="left">
                        <asp:TextBox ID="TxtStato" runat="server" Width="50px" MaxLength="3" AutoPostBack="false" BorderStyle="None"/></td>                                           
                </tr>
                <tr>
                    <td align="left" style="width:120px">Telefono 1</td>
                    <td align="left">
                        <asp:TextBox ID="txtTelefono1" runat="server" Width="200px" MaxLength="30" BorderStyle="None" /></td>
                    <td align="right" style="width:100px">Telefono 2</td>
                    <td align="left"><asp:TextBox ID="txtTelefono2" runat="server" Width="200px" MaxLength="30" BorderStyle="None"/></td>                                           
                </tr>
                <tr>
                    <td align="left" style="width:120px">Fax</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtFax" runat="server" Width="200px" MaxLength="30" BorderStyle="None" /></td>
                </tr>    
                <tr>
                    <td align="left" style="width:120px">E-mail</td>
                    <td align="left" colspan="3">
                        <asp:TextBox ID="txtEmail" AutoPostBack="false" runat="server" Width="99%" MaxLength="100" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>     
                <tr>
                    <td align="left" style="width:120px"></td>
                    <td align="center" colspan="3">
                        <asp:Label ID="lblMessDett" runat="server" Font-Bold="True" BorderStyle="Groove" Width="99%" ></asp:Label></td>
                </tr>     
            </table>
            </div>
        </td>
    </tr>
</table>
    