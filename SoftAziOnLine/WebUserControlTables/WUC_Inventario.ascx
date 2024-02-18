<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Inventario.ascx.vb" Inherits="SoftAziOnLine.WUC_Inventario" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
<style type="text/css">
    .styleTDBTN
        {
            height: 478px;
        }
    .btnstyle
    {
        Width: 108px;
        height: 40px;
        margin-left: 0px;
        white-space: pre-wrap;      
    }
    .styleMenu
    {
        width: auto;
        border-style:groove;
    }
    .styleBordo
    {
        height: 35px;
        width: 860px;
        border-style:groove;
    }
    .style7
    {
        height: 324px;
    }
    .style9
    {
        width: 847px;
        height: 324px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
 <br /> <br /> <br /> 
            <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 340px; width: 940px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelMagazzino" style="margin-top: 0px;" runat="server" GroupingText="Magazzino">
                        <asp:Label ID="lblMagazzino" runat="server" Width="146px" Height="16px">Magazzino</asp:Label>
                        <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                            AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                            DataTextField="Descrizione" 
                            DataValueField="Codice" Width="545px" TabIndex="2">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                            SelectCommand="SELECT Codice, Descrizione FROM Magazzini ORDER BY Descrizione">
                        </asp:SqlDataSource>
                    </asp:Panel>
                    <!--<asp:Panel ID="PanelRicerca" style="margin-top: 0px;" runat="server" GroupingText="Ricerca per">
                        <asp:Label ID="lblRicerca" runat="server" Width="155px" Height="16px">Codice</asp:Label>
                            <asp:TextBox ID="txtRicerca" runat="server" AutoPostBack="false"
                               Width="635px" ></asp:TextBox>
                    </asp:Panel>-->
                    <asp:Panel ID="Panel1" style="margin-top: 0px;" runat="server" groupingtext="Ordinamento">
                        <table style="width: 100%">
                        <tr>
                            <td width="50%" align="center">
                            <asp:Panel ID="PanelBody" runat="server" Height="100px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                                <asp:GridView ID="GridView_OrdineInv" runat="server" 
                                GridLines="None" CssClass="GridViewStyle" 
                                AllowSorting="True" AutoGenerateColumns="False" 
                                EmptyDataText="Nessun dato disponibile."  
                                DataKeyNames="Riga"
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
                                    <%--<asp:BoundField DataField="Codice" HeaderText="Codice" SortExpression="Codice" />--%>
                                    <asp:BoundField DataField="Descrizione" HeaderText="Ordina per:" SortExpression="Descrizione" />
                                    <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                        InsertVisible="False" SelectText="&gt;" 
                                        ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" ControlStyle-Font-Size="XX-Small" HeaderStyle-Width="10" HeaderText="Sel." />
                                </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            </td>
                            <td width="50%" align="center"> 
                            <asp:Panel ID="Panel2" runat="server" Height="100px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                                <asp:GridView ID="GridView_OrdineInvSel" runat="server" 
                                GridLines="None" CssClass="GridViewStyle" 
                                AllowSorting="True" AutoGenerateColumns="False" 
                                EmptyDataText="Codice articolo [Ordinamento predefinito]"  
                                DataKeyNames="Riga"
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
                                        InsertVisible="False" SelectText="&lt;" 
                                        ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" ControlStyle-Font-Size="XX-Small" HeaderStyle-Width="10" HeaderText="DeSel." />
                                    <%--<asp:BoundField DataField="Codice" HeaderText="Codice" SortExpression="Codice" />--%>
                                    <asp:BoundField DataField="Descrizione" HeaderText="Ordine corrente:" SortExpression="Descrizione" />
                                </Columns>
                                </asp:GridView>
                            </asp:Panel>
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione" 
                        style="margin-top: 0px;" Width="847px"> 
                    <table>
                        <tr>
                        <td><asp:Label ID="Label4" runat="server" Width="145px" Height="16px">Fornitore</asp:Label>
                            <asp:Button ID="btnTrovaFornitore" runat="server" Height="22px" Text="?"
                               Visible="False" Width="30px" /> &nbsp;
                            <asp:TextBox ID="txtCodFornitore" runat="server" AutoPostBack="true"
                               Width="80px" TabIndex="6" MaxLength="10"></asp:TextBox> &nbsp;
                            <asp:DropDownList ID="DDLFornitori" runat="server" AppendDataBoundItems="true"
                               AutoPostBack="true" DataSourceID="SqlDataSourceFornitori" 
                               DataTextField="Rag_Soc" 
                               DataValueField="Codice_CoGe" Width="545px" TabIndex="7">
                               <asp:ListItem Text="" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDataSourceFornitori" runat="server"
                                SelectCommand="SELECT Codice_CoGe, Rag_Soc FROM Fornitori ORDER BY Rag_Soc">
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="Label1" runat="server" Width="145px" Height="16px">Reparti</asp:Label>
                            <asp:Button ID="btnTrovaReparto" runat="server" Height="22px" Text="?"
                               Visible="False" Width="30px" /> &nbsp;
                            <asp:TextBox ID="txtCodReparto" runat="server" AutoPostBack="true"
                               Width="80px" TabIndex="6" MaxLength="3"></asp:TextBox> &nbsp;
                            <asp:DropDownList ID="DDLReparto" runat="server" AppendDataBoundItems="true"
                               AutoPostBack="true" DataSourceID="SqlDataSourceReparto" 
                               DataTextField="Descrizione" 
                               DataValueField="Codice" Width="545px" TabIndex="7">
                               <asp:ListItem Text="" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDataSourceReparto" runat="server"
                                SelectCommand="SELECT Codice, Descrizione FROM Reparti WHERE ([Magazzino] = @CodMag) ORDER BY Descrizione">
                                <SelectParameters>
                                    <asp:SessionParameter Name="CodMag" SessionField="Codice_Mag" Type="Int32" />
                                </SelectParameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblCategoria" runat="server" Width="145px" Height="16px">Categoria</asp:Label>
                            <asp:Button ID="btnTrovaCategoria" runat="server" Height="22px" Text="?"
                               Visible="False" Width="30px" /> &nbsp;
                            <asp:TextBox ID="txtCodCategoria" runat="server" AutoPostBack="true"
                               Width="80px" TabIndex="6" MaxLength="3"></asp:TextBox> &nbsp;
                            <asp:DropDownList ID="ddlCatgoria" runat="server" AppendDataBoundItems="true"
                               AutoPostBack="true" DataSourceID="SqlDataSourceCategoria" 
                               DataTextField="Descrizione" 
                               DataValueField="Codice" Width="545px" TabIndex="7">
                               <asp:ListItem Text="" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDataSourceCategoria" runat="server"
                                SelectCommand="SELECT Codice, Descrizione FROM CategArt ORDER BY Descrizione">
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                    <tr>
                        <td>
                           <asp:Label ID="lblLinea" runat="server" Width="145px" Height="16px">Linea</asp:Label>
                                <asp:Button ID="btnTrovaLinea" runat="server" Height="22px" Text="?"
                                   Visible="False" Width="30px" /> &nbsp;
                                <asp:TextBox ID="txtCodLinea" runat="server" AutoPostBack="true" 
                                Width="80px" TabIndex="8" MaxLength="3"></asp:TextBox> &nbsp;
                                 <asp:DropDownList ID="ddlLinea" runat="server" AppendDataBoundItems="true"
                                 AutoPostBack="true" DataSourceID="SqlDataSourceLinea" 
                                  DataTextField="Descrizione" DataValueField="Codice" Width="545px" 
                                TabIndex="9">
                                 <asp:ListItem Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceLinea" runat="server"
                                SelectCommand="SELECT Codice, Descrizione FROM LineeArt ORDER BY Descrizione" />
                        </td>
                    </tr>
                    <tr>
                    <td>
                        <asp:Label ID="lblDal" runat="server" Height="17px">Dal codice articolo</asp:Label>
                           &nbsp;<asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="30px" 
                                Height="22px" Visible="true" Text="?" ToolTip="Ricerca articoli" />
                            <asp:TextBox ID="txtCod1" runat="server"  
                                Width="145px" MaxLength="20" AutoPostBack="false" TabIndex="10" ></asp:TextBox>
                            <asp:Label ID="Label2" runat="server" Width="50px" Height="17px"></asp:Label>
                            <asp:Label ID="lblAl" runat="server" Height="17px">Al codice articolo</asp:Label>
                            &nbsp;<asp:Button ID="btnCod2" runat="server" class="btnstyle" Height="22px" 
                            Visible="true" Width="30px" Text="?" ToolTip="Ricerca articoli"/>
                            <asp:TextBox ID="txtCod2" runat="server"  
                                Width="145px" MaxLength="20" TabIndex="11" AutoPostBack="false" ></asp:TextBox>
                        <td>
                    </tr>
                    <tr>
                        <td>
                           <asp:Label ID="Label3" runat="server" Width="152px" Height="16px">N° righe per pagina</asp:Label>
                           <asp:TextBox ID="txtNRigheXPag" runat="server" AutoPostBack="False" 
                                Width="80px" TabIndex="8" MaxLength="3" Enabled="true"></asp:TextBox> &nbsp;
                           <asp:CheckBox ID="chkUnaPagina" runat="server" Text="Singola pagina" AutoPostBack="true" Checked="false" Visible="false"/> &nbsp;
                           <asp:CheckBox ID="chkStampa" runat="server" Text="Stampa" AutoPostBack="false" Checked="false" Visible="false"/>
                        </td>
                    </tr>
                    </table>
                    </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div>
                                    <asp:Button ID="btnGeneraInventario" runat="server" class="btnstyle" Text="Genera schede inventario" 
                                        TabIndex="15" />
                                </div>  
                                <div style="height: 15px"></div>
                                <div>
                                    <asp:Button ID="btnElencoSchedeIN" runat="server" class="btnstyle" Text="Gestione schede inventario" 
                                       Visible="false" TabIndex="15" />
                                </div>  
                                <div style="height: 15px"></div>
                                <div>
                                    <asp:Button ID="btnEliminaSchede" runat="server" class="btnstyle" Text="Elimina schede inventario" 
                                       Visible="false" TabIndex="15" />
                                </div> 
                                <div style="height: 15px"></div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="15" />
                                </div>
                                <div style="height: 15px"></div>
                                <div style="height: 15px"></div>                     
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel> 
</asp:Panel>