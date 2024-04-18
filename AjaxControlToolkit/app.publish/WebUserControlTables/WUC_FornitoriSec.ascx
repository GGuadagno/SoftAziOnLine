<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_FornitoriSec.ascx.vb" Inherits="SoftAziOnLine.WUC_FornitoriSec" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table width="650px">
    <tr>
        <td style="width:100%">
            <asp:SqlDataSource ID="SqlDataSourceFornitoriSec" runat="server" SelectCommand="get_FornSecondariByCodiceArticolo" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter Name="Codice" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" BorderStyle="Solid" BorderWidth="1px">
            <div id="divGridViewForSec" style="overflow: auto; height: 80px; border-style:groove">
                <asp:GridView ID="GridViewBody" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="CodFornitore"
                    ShowFooter="false">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:CommandField ButtonType="Button" CausesValidation="False" 
                            InsertVisible="False" SelectText="&gt;" 
                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" ControlStyle-Font-Size="XX-Small" />
                        <asp:BoundField DataField="CodFornitore" HeaderText="Codice forn." 
                            SortExpression="CodFornitore" />
                        <asp:BoundField DataField="RagSoc" HeaderText="Ragione sociale" 
                            SortExpression="RagSoc" />
                        <asp:BoundField DataField="GiorniConsegna" HeaderText="Giorni cons." 
                            SortExpression="GiorniConsegna" />
                        <asp:BoundField DataField="CodPagamento" HeaderText="Cond. pagamento" 
                            SortExpression="CodPagamento" />
                        <asp:BoundField DataField="UltPrezzo" HeaderText="Prezzo" 
                            SortExpression="UltPrezzo" />
                        <asp:BoundField DataField="Titolare" HeaderText="Titolare" 
                            SortExpression="Titolare" />
                        <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                            SortExpression="Riferimento" />
                    </Columns>
                </asp:GridView>
            </div>
            </asp:Panel>
        </td>
        <td>
            <asp:Button ID="btnEliminaFornitoreSec" runat="server" Text="Elimina" Width="70"/>
        </td>
    </tr>
</table>