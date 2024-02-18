<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_SceltaSpedizione.ascx.vb" Inherits="SoftAziOnLine.WUC_SceltaSpedizione" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>

<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr>
        <td >
            <asp:SqlDataSource ID="SqlDataSource" runat="server" 
                SelectCommand="SELECT ID, NumeroSpedizione, DataSpedizione, StatoSped, IDVettore, BloccatoDalPC, Operatore FROM Spedizioni Where StatoSped<5  ORDER BY DataSpedizione">
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" Height="200px" ScrollBars="Vertical" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewBody" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="ID" DataSourceID="SqlDataSource">
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
                        <asp:BoundField DataField="NumeroSpedizione" HeaderText="N° Spedizione" SortExpression="NumeroSpedizione" />
                        <asp:BoundField DataField="DataSpedizione" HeaderText="Data" SortExpression="DataSpedizione" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
</table>
    