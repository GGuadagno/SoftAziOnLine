<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ElencoCatCli.ascx.vb" Inherits="SoftAziOnLine.WUC_ElencoCatCli" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr>
        <td>
            <asp:Label ID="lbTitolo" runat="server" Text="" Font-Bold="true"/><br />
            <asp:Label ID="Label1" runat="server" Text="Ricerca per:" Font-Bold="false"/>
            <asp:DropDownList ID="ddlRicerca" runat="server" Width="210px">
            </asp:DropDownList>&nbsp&nbsp
            <asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
            <asp:TextBox ID="txtRicerca" runat="server" Width="140px"></asp:TextBox>&nbsp
            <asp:Button ID="btnRicerca" runat="server" Text="Cerca"/>
        </td>
    </tr>
    <tr>
        <td >
            <asp:Panel ID="PanelBody" runat="server" Height="350px" ScrollBars="Vertical" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewBody" runat="server" 
                    GridLines="Horizontal" CssClass="GridViewStyle" 
                    AllowSorting="false" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="Codice"
                    ShowFooter="false"
                    AllowPaging="true"
                    PageSize="12" 
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
                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" ControlStyle-Font-Size="XX-Small" ControlStyle-Width="10px" HeaderStyle-Width="10px" />
                        <asp:BoundField DataField="Codice" HeaderText="Codice" SortExpression="Codice" ControlStyle-Width="70px" />
                        <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" SortExpression="Descrizione" />
                        <asp:TemplateField HeaderText="Invio E-mail" ItemStyle-HorizontalAlign="Center" SortExpression="InvioMailSc">
                            <ItemTemplate>
                                <asp:CheckBox id="chkInvioEmailGR" runat="server" Enabled="true" AutoPostBack="True" Checked='<%# Convert.ToBoolean(Eval("InvioMailSc")) %>' 
                                    OnCheckedChanged="chkInvioEmailGR_CheckedChanged"></asp:CheckBox> 
                            </ItemTemplate>
                            <HeaderStyle Width="25px"/><ItemStyle Width="25px" />
                        </asp:TemplateField>                                                                 
                        <asp:TemplateField HeaderText="Selezione" ItemStyle-HorizontalAlign="Center" SortExpression="SelSc">
                            <ItemTemplate>
                                <asp:CheckBox id="chkSelScGR" runat="server" Enabled="true" AutoPostBack="True" Checked='<%# Convert.ToBoolean(Eval("SelSc")) %>' 
                                    OnCheckedChanged="chkSelScGR_CheckedChanged"></asp:CheckBox>
                            </ItemTemplate>
                            <HeaderStyle Width="25px"/><ItemStyle Width="25px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
</table>