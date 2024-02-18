<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WFP_Articolo_Seleziona.ascx.vb" Inherits="SoftAziOnLine.WFP_Articolo_Seleziona" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />

<style type="text/css">
    .modalBackground {
        background-color:Gray;
        filter:alpha(opacity=70);
        opacity:0.7;
    }

    .modalPopup {
        background-color:#ffffdd;
        border-width:3px;
        border-style:solid;
        border-color:Gray;
        padding:3px;
        width:250px;
    }
</style>

<ajaxToolkit:ModalPopupExtender runat="server" ID="ProgrammaticModalPopup"
    TargetControlID="LinkButton1"
    PopupControlID="programmaticPopup" 
    BackgroundCssClass="modalBackground"
    DropShadow="true"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>
<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:center;">
        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" Height="25px">Ordinamento e ricerca per:&nbsp 
            <asp:DropDownList ID="ddlRicercaArtIn" runat="server" AutoPostBack="True" Width="130px"></asp:DropDownList>&nbsp
            <asp:CheckBox ID="checkParoleContenute" runat="server" AutoPostBack="True" Text="Parole contenute" Checked="false"/>&nbsp
            <asp:TextBox ID="txtRicercaArtIn" runat="server" Width="250px"></asp:TextBox>&nbsp
            <asp:Button ID="btnRicercaArtIn" runat="server" Text="Cerca articolo" />
            </asp:Panel>                           
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:Panel ID="Panel4" runat="server" Height="25px"><asp:Label ID="lblIntesta1" 
                runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="Articoli presenti nel listino" Width="99%"></asp:Label>
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server" Height="240px">
            <asp:UpdatePanel ID="UpdatePanel7" runat="server">
            <ContentTemplate>
                    <asp:SqlDataSource ID="SqlDSArtIn" runat="server" 
                    SelectCommand="get_ListVenDByCodListino" SelectCommandType="StoredProcedure">
                    <SelectParameters>
                    <asp:SessionParameter DefaultValue="1" Name="CodLis" SessionField="IDLISTINO" Type="Int32" />
                    <asp:SessionParameter DefaultValue="C" Name="SortListVenD" SessionField="SortListVenD" Type="String" />
                    </SelectParameters>                        
                    </asp:SqlDataSource>
                        <table style="width:100%;">
                            <tr><td>
                                <div id="divGridViewArtIn" 
                                    style="overflow: auto; height: 215px; border-style:groove; background-color: Silver;">
                                    <asp:GridView ID="GridViewArtIn" runat="server" AutoGenerateColumns="False" 
                                        CssClass="GridViewStyle" 
                                                    AllowSorting="false" 
                                                    EmptyDataText="Nessun dato disponibile."   
                                                    AllowPaging="true"
                                                    PageSize="15" 
                                                    PagerStyle-HorizontalAlign="Center" 
                                                    PagerSettings-Mode="NextPreviousFirstLast"
                                                    PagerSettings-Visible="true"
                                                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif"  
                                        DataKeyNames="Cod_Articolo" 
                                        DataSourceID="SqlDSArtIn" 
                                        EnableTheming="False" GridLines="None">
                                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                        <Columns>
                                            <%--<asp:TemplateField InsertVisible="False">
                                                <ItemTemplate><asp:Button ID="Button2" runat="server" CausesValidation="False" 
                                                        CommandName="Select" Text="↓" />
                                            </ItemTemplate>                                                                                    
                                            <controlstyle font-size="XX-Small" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox id="checkSel" AutoPostBack="false" Checked="false" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Cod_Articolo" 
                                                DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                                                SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" />
                                                <ItemStyle Width="100px" /></asp:BoundField>
                                            <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" 
                                                SortExpression="Descrizione" />
                                            <asp:BoundField DataField="Prezzo" HeaderText="Prezzo €" 
                                                SortExpression="Prezzo" />
                                            <asp:TemplateField HeaderText="Sconto 1 %" SortExpression="Sconto_1"><ItemTemplate>
                                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Sconto_1") %>'></asp:Label>
                                            </ItemTemplate>                                                                                    
                                        <EditItemTemplate>
                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Sconto_1") %>'></asp:TextBox>
                                        </EditItemTemplate>                                                                                   
                                        </asp:TemplateField>
                                        <%--<asp:TemplateField HeaderText="Sconto 2 %" SortExpression="Sconto_2">
                                                <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("Sconto_2") %>'></asp:Label>
                                                </ItemTemplate>                                                                                   
                                        <EditItemTemplate>
                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Sconto_2") %>'></asp:TextBox>
                                        </EditItemTemplate>                                           
                                            
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="Prezzo minimo €" SortExpression="PrezzoMinimo">
                                                <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("PrezzoMinimo") %>'></asp:Label>
                                                </ItemTemplate>                                                                                    
                                        <EditItemTemplate>
                                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("PrezzoMinimo") %>'></asp:TextBox>
                                        </EditItemTemplate>                                                                                    
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Prezzo Acquisto €" 
                                            SortExpression="PrezzoMinimo"><ItemTemplate><asp:Label ID="Label5" 
                                                runat="server" Text='<%# Bind("PrezzoAcquisto") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("PrezzoAcquisto") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="LBase" HeaderText="LBase" 
                                                SortExpression="LBase"/>
                                        <asp:BoundField DataField="LOpz" HeaderText="LOpz" 
                                                SortExpression="LOpz"/>
                                    </Columns>                                    
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
                            </asp:GridView></div></td></tr><tr><td bgcolor="Silver">
                            </td></tr>
                        </table>                        
                </ContentTemplate>            
        </asp:UpdatePanel>
    </asp:Panel>                             
    <asp:Panel ID="Panel3" runat="server" Height="30px"><asp:Label ID="lblMessUtente" 
                runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="" Width="99%"></asp:Label>
        </asp:Panel>    
</asp:Panel>
    <br />        
    <div style="text-align:center;">
        <asp:CheckBox id="checkSelAutoPadre" AutoPostBack="false" Checked="true" runat="server" Text="Seleziona automatica prodotto BASE" Font-Bold="True"/>   
        &nbsp&nbsp&nbsp<asp:Button ID="btnOk" runat="server" Text="OK" OnClick="btnOk_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Annulla" OnClick="btnCancel_Click" />
        <asp:Button ID="btnSelTutti" runat="server" Text="Seleziona tutti" OnClick="btnSelTutti_Click" />
        <asp:Button ID="btnDeselTutti" runat="server" Text="Deseleziona tutti" OnClick="btnDeselTutti_Click" />&nbsp&nbsp&nbsp
        <asp:CheckBox id="CheckSelFor" AutoPostBack="true" Checked="true" runat="server" Text="Seleziona prodotti del Fornitore" Font-Bold="True"/> 
        <asp:CheckBox id="ckNoDesArtEst" AutoPostBack="false" Checked="false" runat="server" Text="Non importare descrizioni aggiuntive" Font-Bold="True"/>  
    </div>
   <br />
   <div style="text-align:center;">
    <asp:Label ID="Label1" 
                runat="server" BorderStyle="Outset" Font-Bold="true" Font-Overline="False" 
                Style="text-align:center" Text="Nota: Il cambio pagina annulla eventuali articoli selezionati." Width="99%"></asp:Label>
   </div>
</asp:Panel>
