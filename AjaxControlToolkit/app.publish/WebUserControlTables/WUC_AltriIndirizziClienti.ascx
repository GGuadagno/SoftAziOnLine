<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_AltriIndirizziClienti.ascx.vb" Inherits="SoftAziOnLine.WUC_AltriIndirizziClienti" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControl/WFP_Elenco.ascx" TagName="WFPElenco" TagPrefix="wuc" %>

<wuc:WFPElenco ID="WFPElencoNazioni" runat="server" Tabella="Nazioni" Titolo="Elenco Nazioni"/>
<wuc:WFPElenco ID="WFPElencoProvince" runat="server" Tabella="Province" Titolo="Elenco Province"/>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<asp:SqlDataSource ID="SqlDSIndirCF" runat="server" 
    SelectCommand="SELECT * FROM [IndirCF] WHERE ([Codice] = @Codice) ORDER BY Ragione_Sociale" 
    UpdateCommand="InsertUpdate_IndirCF" 
    UpdateCommandType="StoredProcedure"
    DeleteCommand="Delete_IndirCF"
    DeleteCommandType="StoredProcedure">
    <SelectParameters>
        <asp:SessionParameter Name="Codice" SessionField="Codice_CoGeDM" Type="String" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
        <asp:Parameter Name="Codice" Type="String" />
        <asp:Parameter Name="Tipo" Type="String" />
        <asp:Parameter Name="Ragione_Sociale" Type="String" />
        <asp:Parameter Name="Denominazione" Type="String" />
        <asp:Parameter Name="Riferimento" Type="String" />
        <asp:Parameter Name="Indirizzo" Type="String" />
        <asp:Parameter Name="CAP" Type="String" />
        <asp:Parameter Name="Localita" Type="String" />
        <asp:Parameter Name="Provincia" Type="String" />
        <asp:Parameter Name="Stato" Type="String" />
        <asp:Parameter Name="Telefono1" Type="String" />
        <asp:Parameter Name="Telefono2" Type="String" />
        <asp:Parameter Name="Fax" Type="String" />
        <asp:Parameter Name="Email" Type="String" />
    </UpdateParameters>
    <DeleteParameters>
        <asp:Parameter Name="Codice" Type="String" />
        <asp:Parameter Name="Tipo" Type="String" />
    </DeleteParameters>
</asp:SqlDataSource>
<table width="810px">
    <tr>
        <td style="width:100%">
            <asp:Panel ID="PanelBody" runat="server" BorderStyle="groove" BorderWidth="1px">
            <div id="divGridViewAltriInd" style="overflow: auto; height:250px; width:1050px; border-style:groove">
                <asp:GridView ID="GridViewBody" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."   
                    DataKeyNames="Tipo" DataSourceID="SqlDSIndirCF" AllowSorting="True">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:CommandField ButtonType="Button" CausesValidation="False" 
                            InsertVisible="False" SelectText="&gt;" 
                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" 
                            ControlStyle-Font-Size="XX-Small" >
                        <ControlStyle Font-Size="XX-Small" />
                        </asp:CommandField>
                        <asp:BoundField DataField="Tipo" HeaderText="Sigla" 
                            SortExpression="Tipo" />
                        <asp:BoundField DataField="Ragione_Sociale" HeaderText="Ragione sociale" 
                            SortExpression="Ragione_Sociale" />
                        <asp:BoundField DataField="Denominazione" HeaderText="Denominazione" 
                            SortExpression="Denominazione" />
                        <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                            SortExpression="Riferimento" />
                        <asp:BoundField DataField="Indirizzo" HeaderText="Indirizzo" 
                            SortExpression="Indirizzo" />
                        <asp:BoundField DataField="Cap" HeaderText="CAP" 
                            SortExpression="Cap" />
                        <asp:BoundField DataField="Localita" HeaderText="Località" 
                            SortExpression="Localita" />
                        <asp:BoundField DataField="Provincia" HeaderText="Provincia" 
                            SortExpression="Provincia" />
                        <asp:BoundField DataField="Stato" HeaderText="Naz." 
                            SortExpression="Stato" />
                        <asp:BoundField DataField="Telefono1" HeaderText="Telefono(1)" 
                            SortExpression="Telefono1" />
                        <asp:BoundField DataField="Telefono2" HeaderText="Telefono(2)" 
                            SortExpression="Telefono2" />
                        <asp:BoundField DataField="Fax" HeaderText="Fax" 
                            SortExpression="Fax" />
                        <asp:BoundField DataField="Email" HeaderText="E-mail" 
                            SortExpression="Email" />    
                    </Columns>
                </asp:GridView>
            </div>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Sigla</td>
                    <td><asp:TextBox ID="txtTipo" AutoPostBack="False" runat="server" Width="70px" MaxLength="5" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:200px">Ragione sociale</td>
                    <td><asp:TextBox ID="txtragSoc" AutoPostBack="false" runat="server" Width="425px" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Denominazione</td>
                    <td><asp:TextBox ID="txtDenominazione" AutoPostBack="false" runat="server" Width="425px" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                     <td style="width:110px">Riferimento</td>
                    <td><asp:TextBox ID="txtRiferimento" AutoPostBack="false" runat="server" Width="920px" MaxLength="500" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Indirizzo</td>
                    <td><asp:TextBox ID="txtIndirizzo" AutoPostBack="false" runat="server" Width="425px" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:95px">CAP</td>
                    <td><asp:TextBox ID="txtCap" AutoPostBack="false" runat="server" Width="50px" MaxLength="5" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Località</td>
                    <td><asp:TextBox ID="txtLocalita" AutoPostBack="false" runat="server" Width="425px" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:70px">Provincia</td>
                    <td><asp:Button ID="btnTrovaProv" runat="server" Text="?" Width="30px" Height="22px"/></td>
                    <td><asp:TextBox ID="txtProvincia" AutoPostBack="false" runat="server" Width="30px" MaxLength="2" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:54px">Nazione</td>
                    <td><asp:Button ID="btnTrovaNazione" runat="server" Text="?" Width="30px" Height="22px"/></td>
                    <td><asp:TextBox ID="txtCodNazione" AutoPostBack="false" runat="server" Width="40px" MaxLength="3" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Telefono 1</td>
                    <td><asp:TextBox ID="txtTel1" AutoPostBack="false" runat="server" Width="180px" MaxLength="30" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:100px">Telefono 2</td>
                    <td><asp:TextBox ID="txtTel2" AutoPostBack="false" runat="server" Width="180px" MaxLength="30" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:45px">Fax</td>
                    <td><asp:TextBox ID="txtFax" AutoPostBack="false" runat="server" Width="180px" MaxLength="30" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">E-mail</td>
                    <td><asp:TextBox ID="txtEMail" AutoPostBack="false" runat="server" Width="705px" MaxLength="100" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:Button ID="btnNuovo" runat="server" Text="Nuovo" class="btnstyle"/>&nbsp;&nbsp;
            <asp:Button ID="btnModifica" runat="server" Text="Modifica"  class="btnstyle"/>&nbsp;&nbsp;
            <asp:Button ID="btnElimina" runat="server" Text="Elimina"  class="btnstyle"/>&nbsp;&nbsp;
            <asp:Button ID="btnAggiorna" runat="server" Text="Aggiorna"  class="btnstyle"/>&nbsp;&nbsp;
            <asp:Button ID="btnAnnulla" runat="server" Text="Annulla"  class="btnstyle"/>
        </td>
    </tr>
</table>