<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_MovMagDaCancellare.ascx.vb" Inherits="SoftAziOnLine.WUC_MovMagDaCancellare" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>

<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr>
        <td >
            <asp:SqlDataSource ID="SqlDataSourceMovMagT" runat="server" 
                SelectCommand="SELECT IDDocumenti, Tipo_Doc, Numero, Data_Doc, Cod_Causale, Cod_Cliente, Cod_Fornitore, Riferimento, Note, NoteRitiro, InseritoDa FROM DocumentiT Where RefInt = @RefIntMovMag ORDER BY Data_Doc DESC">
                <SelectParameters>
                    <asp:SessionParameter DefaultValue="-1" Name="RefIntMovMag" SessionField="RefIntMovMag" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" Height="200px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewBody" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti"
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
                        <%--<asp:TemplateField>
                            <ItemTemplate>
                                <asp:TextBox ID="txtNumero" runat="server" AutoPostBack="false"  HeaderText="Quantità" MaxLength="7"
                                                Width="60px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:BoundField DataField="IDDocumenti" HeaderText="N° Interno" SortExpression="IDDocumenti" />
                        <asp:BoundField DataField="Tipo_Doc" HeaderText="Tipo MM" SortExpression="Tipo_Doc" />
                        <asp:BoundField DataField="Numero" HeaderText="Numero MM" SortExpression="Numero" />
                        <asp:BoundField DataField="Data_Doc" HeaderText="Data MM" SortExpression="Data_Doc" />
                        <asp:BoundField DataField="Cod_Causale" HeaderText="Cod. Causale" SortExpression="Cod_Causale" />
                        <asp:BoundField DataField="Cod_Cliente" HeaderText="Cod. Cliente" SortExpression="Cod_Cliente" />
                        <asp:BoundField DataField="Cod_Fornitore" HeaderText="Cod. Fornitore" SortExpression="Cod_Fornitore" />
                        <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" SortExpression="Riferimento" />
                        <asp:BoundField DataField="Note" HeaderText="Note" SortExpression="Note" />
                        <asp:BoundField DataField="NoteRitiro" HeaderText="Note ritiro" SortExpression="NoteRitiro" />
                        <asp:BoundField DataField="InseritoDa" HeaderText="Inserito da" SortExpression="InseritoDa" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
    <%--<tr> 
        <td >
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                SelectCommand="SELECT IDDocumenti, Tipo_Doc, Numero, Data_Doc, Cod_Causale, Cod_Cliente, Cod_Fornitore, Note, NoteRitiro FROM DocumentiT Where RefInt = @RefIntMovMag ORDER BY Data_Doc DESC">
                <SelectParameters>
                    <asp:SessionParameter DefaultValue="-1" Name="RefIntMovMag" SessionField="RefIntMovMag" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Panel ID="Panel1" runat="server" Height="200px" ScrollBars="Vertical" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridView1" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti"
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
                        <asp:BoundField DataField="IDDocumenti" HeaderText="N°Interno" SortExpression="IDDocumenti" />
                        <asp:BoundField DataField="Tipo_Doc" HeaderText="Tipo MM" SortExpression="Tipo_Doc" />
                        <asp:BoundField DataField="Numero" HeaderText="Numero MM" SortExpression="Numero" />
                        <asp:BoundField DataField="Data_Doc" HeaderText="Data MM" SortExpression="Data_Doc" />
                        <asp:BoundField DataField="Cod_Causale" HeaderText="Cod. Causale" SortExpression="Cod_Causale" />
                        <asp:BoundField DataField="Cod_Cliente" HeaderText="Cod. Cliente" SortExpression="Cod_Cliente" />
                        <asp:BoundField DataField="Cod_Fornitore" HeaderText="Cod. Fornitore" SortExpression="Cod_Fornitore" />
                        <asp:BoundField DataField="Note" HeaderText="Note" SortExpression="Note" />
                        <asp:BoundField DataField="NoteRitiro" HeaderText="Note ritiro" SortExpression="NoteRitiro" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>--%>
</table>
    