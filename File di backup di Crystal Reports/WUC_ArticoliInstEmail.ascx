<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ArticoliInstEmail.ascx.vb" Inherits="SoftAziOnLine.WUC_ArticoliInstEmail" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<%@ Register src="~/WebUserControl/WUC_Anagrafiche_ModifySint.ascx" tagname="WUC_Anagrafiche_ModifySint" tagprefix="uc2" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<uc2:WUC_Anagrafiche_ModifySint ID="WUC_Anagrafiche_ModifySint1" runat="server" />     
<table style="width:auto;height:auto;">
    <tr>
        <td >
            <asp:Panel ID="PanelBody" runat="server" BorderStyle="Solid" BorderWidth="1px">
                <asp:UpdatePanel ID="UpdatePanelDett" runat="server"><ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:950px; height:300px; border-style:groove;">
                      <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                            CssClass="GridViewStyle" 
                            EmptyDataText="Nessun dato disponibile."  
                            DataKeyNames="" 
                            AllowPaging="true"
                            PageSize="10" 
                            PagerStyle-HorizontalAlign="Center" 
                            PagerSettings-Mode="NextPreviousFirstLast"
                            PagerSettings-Visible="true"
                            PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                            PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                            PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                            PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                            EnableTheming="True" GridLines="Horizontal" BackColor="Silver" AllowSorting="true" PagerSettings-Position="Bottom">
                            <RowStyle CssClass="RowStyle" />
                            <PagerStyle CssClass="PagerStyle" />
                            <HeaderStyle CssClass="HeaderStyle" />
                            <AlternatingRowStyle CssClass="AltRowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle"/>
                            <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                                LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                                Mode="NextPreviousFirstLast" 
                                NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                            <Columns>
                                <asp:TemplateField InsertVisible="False">
                                    <ItemTemplate>
                                        <asp:Button ID="btnSelAI" runat="server" CausesValidation="False" CommandName="Select" Text="&gt;" />
                                    </ItemTemplate>
                                    <controlstyle font-size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderStyle-Width="5px" HeaderText="Sel." >
                                    <ItemTemplate>
                                        <asp:CheckBox id="checkSel" AutoPostBack="True" runat="server" Width="5px" Checked='<%# Convert.ToBoolean(Eval("Selezionato")) %>' OnCheckedChanged="checkSel_CheckedChanged" />
                                    </ItemTemplate>
                                     <HeaderStyle Width="5px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Cod_Coge" HeaderText="Codice CoGe" 
                                    SortExpression="Cod_Coge">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" 
                                    SortExpression="Rag_Soc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="50px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Denominazione" 
                                    HeaderText="Denominazione" ReadOnly="True" 
                                    SortExpression="Denominazione">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="DesCateg" 
                                    HeaderText="Categoria" ReadOnly="True" 
                                    SortExpression="DesCateg">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="EmailInvio" 
                                    HeaderText="E-mail Invio" ReadOnly="True" 
                                    SortExpression="EmailInvio">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                                    HeaderText="Località" ReadOnly="True" 
                                    SortExpression="Localita">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                                    HeaderText="CAP" ReadOnly="True" 
                                    SortExpression="CAP">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>                      
                                <asp:BoundField DataField="Cod_Filiale" HeaderText="Cod_Filiale" ItemStyle-Width="1px" ItemStyle-CssClass="nascondi" >
                                    <HeaderStyle CssClass="nascondi" />
                                    <ItemStyle CssClass="nascondi" Width="1px" />
                                </asp:BoundField>  
                                 <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione1" 
                                    HeaderText="Destinazione(1)" ReadOnly="True" 
                                    SortExpression="Destinazione1">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="25px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione2" 
                                    HeaderText="Destinazione(2)" ReadOnly="True" 
                                    SortExpression="Destinazione2">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="25px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione3" 
                                    HeaderText="Destinazione(3)" ReadOnly="True" 
                                    SortExpression="Destinazione3">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="25px" />
                                </asp:BoundField>                        
                            </Columns>
                        </asp:GridView>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </td>
    </tr>
</table>		