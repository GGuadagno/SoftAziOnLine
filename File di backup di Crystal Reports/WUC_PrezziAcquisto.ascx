<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_PrezziAcquisto.ascx.vb" Inherits="SoftAziOnLine.WUC_PrezziAcquisto" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table width="250px">
    <tr>
        <td>
            <asp:SqlDataSource ID="SqlDataSourcePrezziAcq" runat="server" SelectCommand="get_UltimiPrezziAcquistoByCodiceArticolo" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter Name="Codice" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" Height="100px" ScrollBars="Vertical" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewBody" runat="server" AllowPaging="false"
                    GridLines="None" CssClass="GridViewStyle" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="Cod_Articolo"
                    ShowFooter="false">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:BoundField DataField="Cod_Articolo" HeaderText="Codice" 
                            SortExpression="Cod_Articolo" Visible="false" />
                        <asp:BoundField DataField="DataAcquisto" HeaderText="Data" 
                            SortExpression="DataAcquisto" />
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                            SortExpression="Prezzo" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
</table>