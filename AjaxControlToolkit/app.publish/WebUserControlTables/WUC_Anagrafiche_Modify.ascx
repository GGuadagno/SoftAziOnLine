<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Anagrafiche_Modify.ascx.vb" Inherits="SoftAziOnLine.WUC_Anagrafiche_Modify" %>
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

<asp:SqlDataSource ID="SqlDSAnagrCliFor" runat="server" 
    SelectCommand="SELECT * FROM [Clienti] WHERE ([Codice_CoGe] = @Codice)" 
    UpdateCommand="Update_AnagrCliFor_C" 
    UpdateCommandType="StoredProcedure">
    <SelectParameters>
        <asp:SessionParameter Name="Codice" SessionField="IDAnagrCliFor" Type="String" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
        <asp:Parameter Name="Codice_CoGe" Type="String" />
        <asp:Parameter Name="Rag_Soc" Type="String" />
        <asp:Parameter Name="Denominazione" Type="String" />
        <asp:Parameter Name="Riferimento" Type="String" />
        <asp:Parameter Name="Codice_Fiscale" Type="String" />
        <asp:Parameter Name="Partita_IVA" Type="String" />
        <asp:Parameter Name="Indirizzo" Type="String" />
        <asp:Parameter Name="NumeroCivico" Type="String" />
        <asp:Parameter Name="CAP" Type="String" />
        <asp:Parameter Name="Localita" Type="String" />
        <asp:Parameter Name="Provincia" Type="String" />
        <asp:Parameter Name="Nazione" Type="String" />
        <asp:Parameter Name="Telefono1" Type="String" />
        <asp:Parameter Name="Telefono2" Type="String" />
        <asp:Parameter Name="Fax" Type="String" />
        <asp:Parameter Name="Regime_Iva" Type="Int32" />
        <asp:Parameter Name="Email" Type="String" />
        <asp:Parameter Name="IPA" Type="String" />
        <asp:Parameter Name="SplitIVA" Type="Byte" />
        <asp:Parameter Name="EMailInvioScad" Type="String" />
        <asp:Parameter Name="EMailInvioFatt" Type="String" />
        <asp:Parameter Name="PECEMail" Type="String" />
    </UpdateParameters>
</asp:SqlDataSource>
                        
<table style="width:auto;height:auto;">
    <tr>
        <td class="style1" >
            <div id="div1" style="border-style:ridge;border-width:thin">
            <table style="width: 680px"> 
                <tr>
                    <td align="left" class="style2">
                    <asp:Label ID="lblCodice" runat="server" Font-Bold="True" BorderStyle="Groove">Codice</asp:Label>
                    </td>
                    <td colspan="3">
                    <asp:Label ID="lblLabelTipoRK" runat="server" Font-Bold="True" BorderStyle="Groove">Elenco</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">
                        <asp:Label ID="lblIPA" runat="server" Font-Bold="false" Text="Codice IPA"></asp:Label>
                    </td>
                    <td colspan="3"  align="left"  style="margin-left: 120px">
                        <asp:TextBox ID="txtIPA" AutoPostBack="false" runat="server" Width="100px" 
                            MaxLength="10" BorderStyle="None"></asp:TextBox>
                        <asp:CheckBox ID="chkSplitIVA" runat="server" Font-Bold="false" Text="Split IVA (a carico del Cliente)" Checked="false" AutoPostBack="false"/>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Ragione sociale</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtRagioneSoc" AutoPostBack="false" runat="server" Width="99%" 
                            MaxLength="50" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Denominazione</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtDenominazione" AutoPostBack="false" runat="server" Width="99%" 
                            MaxLength="50" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Riferimento</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtRiferimento" AutoPostBack="false" runat="server" Width="99%" 
                            MaxLength="500" BorderStyle="None"></asp:TextBox>
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
                            Width="50px" MaxLength="2"  BorderStyle="None"/></td>
                    <td align="right" style="width:100px">Nazione</td>
                    <td align="left">
                        <asp:TextBox ID="TxtStato" runat="server" 
                            Width="50px" MaxLength="3" AutoPostBack="false" BorderStyle="None"/></td>                                           
                </tr>
                <tr>
                    <td align="left" class="style2">Partita IVA</td>
                    <td align="left" class="style3"><asp:TextBox ID="txtPartitaIVA" runat="server" 
                            Width="155px" MaxLength="11" AutoPostBack="true" ToolTip="P.IVA Italiana per i Clienti/Fornitori Esteri: ITnnnnnnnnnnn" BorderStyle="None" /></td>
                    <td align="right" style="width:100px">Codice Fiscale</td>
                    <td align="left"><asp:TextBox ID="txtCodiceFiscale" runat="server" Width="155px" 
                            MaxLength="16" AutoPostBack="true" BorderStyle="None" /></td>                                           
                </tr>
                <tr>
                    <td align="left" class="style2">Telefono 1</td>
                    <td align="left" class="style3"><asp:TextBox ID="txtTelefono1" runat="server" 
                            Width="200px" MaxLength="30"  BorderStyle="None"/></td>
                    <td align="right" style="width:100px">Telefono 2</td>
                    <td align="left"><asp:TextBox ID="txtTelefono2" runat="server" Width="200px" 
                            MaxLength="30" BorderStyle="None"/></td>                                           
                </tr>
                <tr>
                    <td align="left" class="style2">Fax</td>
                    <td align="left" class="style3"><asp:TextBox ID="txtFax" runat="server" 
                            Width="200px" MaxLength="30" BorderStyle="None" /></td>
                    <td align="right" style="width:100px">Regime IVA</td>
                    <td align="left">
                    <asp:DropDownList ID="ddlIVASpese" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="false" Width="250px"
                            DataSourceID="SqlDSIVA" DataTextField="Descrizione" 
                            DataValueField="Aliquota" Height="22px"><%--<asp:ListItem Text="" Value=""></asp:ListItem>--%>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDSIVA" runat="server" 
                    SelectCommand="SELECT * FROM [Aliquote_IVA] ORDER BY [Descrizione]"                             
                    UpdateCommandType="StoredProcedure">
                    </asp:SqlDataSource>  
                    </td>
                </tr>    
                <tr>
                    <td align="left" class="style2">E-mail</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtEmail" AutoPostBack="false" runat="server" Width="99%" 
                            MaxLength="100" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>   
                <tr>
                    <td align="left" class="style2">PEC E-mail</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtPECEmail" AutoPostBack="false" runat="server" Width="99%" 
                            MaxLength="310" BorderStyle="None"></asp:TextBox>
                    </td>
                </tr>     
            </table>
            </div>
        </td>
    </tr>
</table>
    