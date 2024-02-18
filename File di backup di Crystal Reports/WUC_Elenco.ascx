<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Elenco.ascx.vb" Inherits="SoftAziOnLine.WUC_Elenco" %>
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
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="PanelCategorie" Visible="false" runat="server" Height="100px" BorderStyle="Solid" BorderWidth="1px">
                <div align="center">
                    <asp:Label ID="lblCategorie" runat="server" Text="MODIFICA DATI CATEGORIA CLIENTI" Width="99%" BorderWidth="1px" BorderColor="White" BorderStyle="Groove" Font-Bold="true" ForeColor="Black" ></asp:Label>
                </div>
                <div align="left">
                    <asp:CheckBox ID="chkInvioMailSc" runat="server" Text="Si/No Invio E-mail scadenze" AutoPostBack="true" Enabled="false"/>
                </div>
                <div align="left">
                    <asp:CheckBox ID="chkSelSc" runat="server" Text="Si/No Selezione multipla categorie per invio E-mail scadenze" AutoPostBack="true" Enabled="false"/>
                </div>
            </asp:Panel>
        </td>
    </tr>
</table>
    