<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ElencoDestCF.ascx.vb" Inherits="SoftAziOnLine.WUC_ElencoDestCF" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>

<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr>
        <td>
            <asp:Label ID="lbTitolo" runat="server" Text="" Font-Bold="true"/><br />
            Ricerca per:&nbsp;
            <asp:DropDownList ID="ddlRicerca" AutoPostBack="true" runat="server" Width="130px">
            </asp:DropDownList>&nbsp&nbsp
            <asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
            <asp:TextBox ID="txtRicerca" runat="server" Width="200px"></asp:TextBox>&nbsp&nbsp
            Località <asp:TextBox ID="txtLocalita" runat="server" Width="100px" Enabled="false"></asp:TextBox>&nbsp
            <asp:Button ID="btnRicerca" runat="server" Text="Cerca"/>
        </td>
    </tr>
    <tr>
        <td >
            <asp:Panel ID="PanelBody" runat="server" BorderStyle="None" BorderWidth="1px" >
                <div id="divGridViewPrevT" style="overflow:auto; width:1200px; height:350px; border-style:groove;">
                    <asp:GridView ID="GridViewBody" runat="server" 
                        GridLines="None" CssClass="GridViewStyle" 
                        AllowSorting="true" AutoGenerateColumns="false"
                        EmptyDataText="Nessun dato disponibile."   
                        DataKeyNames="Progressivo"
                        ShowFooter="true" 
                        AllowPaging="true"
                        PageSize="15" 
                        PagerStyle-HorizontalAlign="Center" 
                        PagerSettings-Mode="NextPreviousFirstLast"
                        PagerSettings-Visible="true"
                        PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                        PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                        PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                        PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" >
                        <RowStyle CssClass="RowStyle" />
                        <PagerStyle CssClass="PagerStyle" />
                        <HeaderStyle CssClass="HeaderStyle" />
                        <AlternatingRowStyle CssClass="AltRowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle"/> 
                        <Columns>
                            <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                InsertVisible="False" SelectText="&gt;" 
                                ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" ControlStyle-Font-Size="XX-Small" />
                            <asp:BoundField DataField="Sigla" HeaderText="Sigla" SortExpression="Sigla" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="Ragione_Sociale" HeaderText="Ragione Sociale" SortExpression="Ragione_Sociale" />
                            <asp:BoundField DataField="Denominazione" HeaderText="Denominazione" SortExpression="Denominazione" />
                            <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" SortExpression="Riferimento" />
                            <asp:BoundField DataField="Cap" HeaderText="CAP" SortExpression="Cap" />
                            <asp:BoundField DataField="Localita" HeaderText="Località" SortExpression="Localita" />
                            <asp:BoundField DataField="Indirizzo" HeaderText="Indirizzo" SortExpression="Indirizzo" />
                            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
        </td>
    </tr>
</table>
