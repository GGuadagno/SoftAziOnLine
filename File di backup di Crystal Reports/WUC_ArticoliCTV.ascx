<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ArticoliCTV.ascx.vb" Inherits="SoftAziOnLine.WUC_ArticoliCTV" %>
<table class="sfondopagine" width="810px">
    <tr>
        <td style="width:18%">
            <asp:Button ID="btnInserisciPrima" runat="server" Text="Inserisci prima" Width="100px"/><br />
            <asp:Button ID="btnInserisciDopo" runat="server" Text="Inserisci dopo" Width="100px"/><br />
            <asp:Button ID="btnCancellaRiga" runat="server" Text="Cancella riga" Width="100px"/><br />
            <asp:Button ID="btnModificaRiga" runat="server" Text="Modifica riga" Width="100px"/><br />
            <asp:Label ID="lblMess" runat="server" Width="100%" Visible="false" Font-Bold="false" ForeColor="Red" Text="Campi Obbligatori"></asp:Label>
        </td>
        <td>
            <asp:SqlDataSource ID="SqlDataSourceAnaMagCTV" runat="server" SelectCommand="get_AnaMagCTVByCodiceArticolo" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter Name="Codice" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" BorderStyle="Solid" BorderWidth="1px">
                <div id="divGridArtCTV" style="overflow: auto; height:90px; border-style:groove">
                    <asp:GridView ID="GridViewBody" runat="server" 
                        GridLines="None" CssClass="GridViewStyle" 
                        AllowSorting="False" AutoGenerateColumns="False" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="CodArticolo"
                        ShowFooter="false">
                        <RowStyle CssClass="RowStyle" />
                        <PagerStyle CssClass="PagerStyle" />
                        <HeaderStyle CssClass="HeaderStyle" />
                        <AlternatingRowStyle CssClass="AltRowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                        <PagerSettings Mode="NextPrevious" Visible="false"/>
                        <Columns>
                            <asp:TemplateField InsertVisible="False">
                                    <ItemTemplate><asp:Button ID="Button1" runat="server" 
                                    CausesValidation="False" CommandName="Select" Text="&gt;" />
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                    </asp:TemplateField>
                            <asp:BoundField DataField="CodArticolo" HeaderText="Codice" 
                                SortExpression="CodArticolo" Visible="false" />
                            <asp:BoundField DataField="Progressivo" HeaderText="Progressivo" 
                                SortExpression="Progressivo">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="50px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Tipo" HeaderText="Tipo" 
                                SortExpression="Tipo" />
                            <asp:BoundField DataField="Valore" HeaderText="Valore" 
                                SortExpression="Valore" />
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td style="width:18%">
            <asp:Label ID="Label3" runat="server" Width="100%" Visible="true" Font-Bold="false" ForeColor="Blue" Text="Codice Tipo"></asp:Label>
            <asp:Label ID="Label4" runat="server" Width="100%" Visible="true" Font-Bold="false" ForeColor="Blue" Text="Codice Valore"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtTipo" runat="server" Width="50%" AutoPostBack="false" 
                    MaxLength="35" TextMode="SingleLine" ToolTip="* Chiavi specifiche: RiferimentoAmministrazione" />
            <asp:TextBox ID="txtValore" runat="server" Width="50%" AutoPostBack="false" 
                    MaxLength="35" TextMode="SingleLine"/>
        </td>
    </tr>
</table>