<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_LottiInsert.ascx.vb" Inherits="SoftAziOnLine.WUC_LottiInsert" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr> 
        <td >
        </td>
    </tr>
    <tr>
    <td >
            <asp:Panel ID="PanelBody" runat="server" Height="380px" Width="1240px" ScrollBars="none" BorderStyle="Solid" BorderWidth="1px">
                <table style="height: 350px">
                    <tr>
                        <td align="left">
                            <div style=" text-align:center ">
                                <asp:Label ID="Label1" Width="100%" BackColor="azure" ForeColor="black" runat="server">Articoli presenti nel documento</asp:Label>
                            </div>
                            <asp:Panel Width="400px" ID="Panel3" runat="server" Height="350px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                                <div id="divGridViewDocDA">
                                <asp:GridView ID="GridViewDocDA" runat="server"
                                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                                    EmptyDataText="Nessun articolo presente.">
                                    <RowStyle CssClass="RowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass="HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                                    <Columns>     
                                        <asp:CommandField ButtonType="Button" ShowSelectButton="True" 
                                        HeaderStyle-Width="5%" SelectText=">" HeaderText="Sel."  > 
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="5%" />
                                        </asp:CommandField>                               
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="Riga" DataFormatString="{0:d}" HeaderText="Riga" ReadOnly="True" 
                                        SortExpression="Riga"><HeaderStyle Wrap="false" /><ItemStyle 
                                        Width="5%" /></asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="Cod_Articolo" HeaderText="Codice articolo" ReadOnly="True" 
                                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="20%" /></asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="Descrizione" HeaderText="Descrizione" ReadOnly="True" 
                                        SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="60%" /></asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="Qta_Evasa" HeaderText="Evasa" ReadOnly="True" 
                                        SortExpression="Qta_Evasa"><HeaderStyle Wrap="false" /><ItemStyle 
                                        Width="5%" /></asp:BoundField>
                                        </Columns>
                                </asp:GridView>
                                </div>
                            </asp:Panel>
                        </td>
                        <td align="left">
                            <div style=" text-align:center ">
                                <asp:Label Width="100%" BackColor="azure" ForeColor="black" runat="server">N°Serie/Lotti già presenti in archivio</asp:Label>
                            </div>
                            <div   style=" text-align:center; width: 400px;"> 
                                <asp:Label ID="Label3" runat="server" Height="20px" ForeColor="Blue" Text="Se mancasse il N°Serie o Lotto inserirlo manualmente"></asp:Label>
                            </div>
                            <asp:Panel Width="400px" ID="Panel1" runat="server" Height="330px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                                <div id="divGridViewDocD">
                                <asp:GridView ID="GridViewDocD" runat="server"
                                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                                    EmptyDataText="Nessun lotto è ancora stato caricato nell'archivio."  >
                                    <RowStyle CssClass="RowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass="HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                                    <Columns>                                    
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="NCollo" DataFormatString="{0:d}" HeaderText="Riga" ReadOnly="True" 
                                        SortExpression="NCollo"><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="10%" /></asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="NSerie" HeaderText="Numero di Serie" ReadOnly="True" 
                                        SortExpression="NSerie"><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="30%" /></asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="Lotto" HeaderText="Numero Lotto" ReadOnly="True" 
                                        SortExpression="Lotto"><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="20%" /></asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="QtaColli" HeaderText="Colli" ReadOnly="True" 
                                        SortExpression="QtaColli"><HeaderStyle Wrap="false" /><ItemStyle 
                                        Width="5%" /></asp:BoundField>
                                        </Columns>
                                </asp:GridView>
                                </div>
                            </asp:Panel>
                        </td>                            
                        <td  align="right">
                            <div style=" text-align:center; width: 420px;">
                                <asp:Label Width="100%" BackColor="Azure" ForeColor="black" runat="server">N°Serie/Lotti scansionati</asp:Label>
                            </div>
                            <div   style=" text-align:center; width: 400px;"> 
                                <asp:Label runat="server" Height="20px" ForeColor="Blue" Text="Scansionare il N° Serie/Lotto"></asp:Label>
                                <asp:TextBox type="text" runat="server" id="txtNserie" MaxLength="30" AutoPostBack="true" style="azimuth:center; width: 250px;"> </asp:TextBox>
                                <asp:Label ID="Label2" runat="server" Height="20px" ForeColor="Blue" Text="Qtà Colli"></asp:Label>
                                <asp:TextBox type="text" runat="server" id="txtNCollo" MaxLength="5" AutoPostBack="false" ForeColor="Blue" Font-Bold="true" style="text-align:center; width: 45px;"> </asp:TextBox>
                            </div>
                            <asp:Panel Width="420px" ID="Panel2" runat="server" Height="310px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">                                                              
                               <div id="divgridLotti" runat="server">
                                    <asp:GridView ID="gridLotti" runat="server"
                                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                                    EmptyDataText="Nessun codice seriale è stato ancora inserito." DataKeyNames="ID"  >
                                    <RowStyle CssClass="RowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass="HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle"/>       
                                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                                     <Columns>
                                        <%--<asp:CommandField ButtonType="Button" ShowSelectButton="True" 
                                        HeaderStyle-Width="5%" SelectText=">" HeaderText="Sel."  > 
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="5%" />
                                        </asp:CommandField>--%>
                                        <asp:BoundField ApplyFormatInEditMode="True"
                                        DataField="ID" DataFormatString="{0:d}" HeaderText="Riga" ReadOnly="True" 
                                        ><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="10%" /></asp:BoundField>      
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="NSerie" HeaderText="Numero di Serie" ReadOnly="True" 
                                        ><HeaderStyle Wrap="True" /><ItemStyle 
                                        Width="35%" /></asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="NLotto" HeaderText="Numero Lotto" ReadOnly="True" 
                                        SortExpression="NLotto"><HeaderStyle Wrap="True" />
                                        <ItemStyle Width="35%" /></asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="True" 
                                        DataField="QtaColli" HeaderText="Colli" ReadOnly="True" 
                                        SortExpression="QtaColli"><HeaderStyle Wrap="false" /><ItemStyle 
                                        Width="5%" /></asp:BoundField>
                                        <asp:CommandField ButtonType="Button" ShowDeleteButton="True" 
                                        HeaderStyle-Width="15%" DeleteText="X" HeaderText="Elimina" ItemStyle-HorizontalAlign="Center" > 
                                        <controlstyle font-size="XX-Small" />
                                        <HeaderStyle Width="15%" />
                                        </asp:CommandField>
                                     </Columns>                                                                        
                                    </asp:GridView>
                               </div>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>


    