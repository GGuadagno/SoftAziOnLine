<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ListiniDaAggiornare.ascx.vb" Inherits="SoftAziOnLine.WUC_ListiniDaAggiornare" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr>
        <td >
            <asp:SqlDataSource ID="SqlDataSourceListiniVenD" runat="server" 
                SelectCommand="SELECT Codice, Descrizione, Tipo, Data_Inizio_Validita, Abilitato, Valuta, Cod_Pagamento, Categoria, Cliente FROM ListVenT Where Codice <> 1">
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" Height="100px" ScrollBars="Vertical" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewBody" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="Codice"
                    ShowFooter="false">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox id="checkSel" AutoPostBack="false" Checked="false" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codice" HeaderText="Codice" SortExpression="Codice" />
                        <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" SortExpression="Descrizione" />
                        <asp:BoundField DataField="Tipo" HeaderText="Tipo" SortExpression="Tipo" />
                        <asp:BoundField DataField="Data_Inizio_Validita" HeaderText="Data inizio validita" SortExpression="Data_Inizio_Validita" />
                        <asp:BoundField DataField="Abilitato" HeaderText="Abilitato" SortExpression="Abilitato" />
                        <asp:BoundField DataField="Valuta" HeaderText="Valuta" SortExpression="Valuta" />
                        <asp:BoundField DataField="Cod_Pagamento" HeaderText="Codice pagamento" SortExpression="Cod_Pagamento" />
                        <asp:BoundField DataField="Categoria" HeaderText="Categoria" SortExpression="Categoria" />
                        <asp:BoundField DataField="Cliente" HeaderText="Cliente" SortExpression="Cliente" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
</table>
    