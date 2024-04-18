<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DescEstesa.ascx.vb" Inherits="SoftAziOnLine.WUC_DescEstesa" %>
<table class="sfondopagine" width="810px">
    <tr>
        <td style="width:18%">
            <asp:Button ID="btnInserisciPrima" runat="server" Text="Inserisci prima" Width="100px"/><br />
            <asp:Button ID="btnInserisciDopo" runat="server" Text="Inserisci dopo" Width="100px"/><br />
            <asp:Button ID="btnCancellaRiga" runat="server" Text="Cancella riga" Width="100px"/><br />
            <asp:Button ID="btnModificaRiga" runat="server" Text="Modifica riga" Width="100px"/><br />
            <asp:Label ID="lblMess" runat="server" Width="100%" Visible="false" Font-Bold="True" ForeColor="Red" Text="Descrizione obbligatoria"></asp:Label>
        </td>
        <td>
            <asp:SqlDataSource ID="SqlDataSourceDescEstesa" runat="server" SelectCommand="get_AnaMagDesByCodiceArticolo" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter Name="Codice" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" BorderStyle="Solid" BorderWidth="1px">
                <div id="divGridViewBody" style="overflow: auto; height: 110px; border-style:groove">
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
                            <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" 
                                SortExpression="Descrizione" />
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:TextBox ID="txtRiga" runat="server" Width="95%" AutoPostBack="false" 
                    MaxLength="150" Rows="2" TextMode="MultiLine"/>
            </asp:Panel>
        </td>
    </tr>
</table>