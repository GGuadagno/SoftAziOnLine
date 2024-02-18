<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WFP_RespVisite.ascx.vb" Inherits="SoftAziOnLine.WFP_RespVisite" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/WebUserControlTables/WUC_RespVisite.ascx" TagName="WUCRespVisite" TagPrefix="wuc" %>
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
<asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
    SelectCommand="SELECT * FROM [Regioni] ORDER BY [Descrizione]">
</asp:SqlDataSource>
<asp:SqlDataSource ID="SqlDSProvince" runat="server" 
    SelectCommand="SELECT Codice, Descrizione FROM [Province] WHERE [Regione] = CASE WHEN @CodRegione = 0 THEN Regione ELSE @CodRegione END ORDER BY [Descrizione]">
    <SelectParameters>
        <asp:SessionParameter Name="CodRegione" DbType="Int32" Direction="Input" SessionField="CodRegione" />
    </SelectParameters>
</asp:SqlDataSource> 
<asp:SqlDataSource ID="SqlDSRegPrElenco" runat="server" 
    SelectCommand="SELECT RespVisiteRegPr.Codice, CodRegione, Provincia, Regioni.Descrizione AS DesRegione FROM RespVisiteRegPr LEFT OUTER JOIN Regioni ON RespVisiteRegPr.CodRegione = Regioni.Codice WHERE CodRespVisite = @IDRespVisite ORDER BY Regioni.Descrizione,Provincia">
    <SelectParameters>
        <asp:SessionParameter Name="IDRespVisite" DbType="Int32" Direction="Input" SessionField="IDRespVisite" />
    </SelectParameters>
</asp:SqlDataSource> 
    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:center;">
        <wuc:WUCRespVisite ID="WUC_RespVisite" runat="server" />
    </asp:Panel>
    <div style="text-align:center;">    
        <asp:Button ID="btnAggiorna" runat="server" Text="Seleziona e aggiorna" OnClick="btnAggiorna_Click" />
        <asp:Button ID="btnNuovo" runat="server" Text="Nuovo" OnClick="btnNuovo_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Annulla" OnClick="btnCancel_Click" />
    </div>
     <asp:Panel ID="Panel3" runat="server" Height="25px">
        <asp:Label ID="lblMessUtente" 
                runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="" Width="99%"></asp:Label>
        </asp:Panel>
    <asp:Panel ID="PanelRegProv" runat="server" Height="200px" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;">
        <div style="height:25px;">
            <asp:Label ID="Label1" 
                runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="Elenco abbinamenti Regioni/Province con Responsabili Visite/Area" Width="99%"></asp:Label>
        </div>
        <div id="divGridRegProv" style="overflow: auto; height:140px; border-style:groove">
                    <asp:GridView ID="GridViewBody" runat="server" 
                        GridLines="None" CssClass="GridViewStyle" 
                        AllowSorting="False" AutoGenerateColumns="False" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="Codice"
                        ShowFooter="false"
                        DataSourceID="SqlDSRegPrElenco" BackColor="Silver">
                        <RowStyle CssClass="RowStyle" />
                        <PagerStyle CssClass="PagerStyle" />
                        <HeaderStyle CssClass="HeaderStyle" />
                        <AlternatingRowStyle CssClass="AltRowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                        <PagerSettings Mode="NextPrevious" Visible="false"/>
                        <Columns>
                            <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                                ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                                <ControlStyle Font-Size="XX-Small" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                            </asp:CommandField>
                            <asp:BoundField DataField="DesRegione" HeaderText="Regione" 
                                SortExpression="DesRegione"><HeaderStyle Wrap="false" />
                                        <ItemStyle Width="250px" /></asp:BoundField>
                            <asp:BoundField DataField="Provincia" HeaderText="Provincia" 
                                SortExpression="Provincia">
                                <HeaderStyle Wrap="false" />
                                <ItemStyle Width="50px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Codice" HeaderText="Codice"  
                                SortExpression="Codice"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                                Width="10px" CssClass="nascondi" /></asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
         <div style="text-align:center;">   
            <asp:Label ID="Label3" runat="server" TabIndex="2">Reg.:</asp:Label>
                <asp:DropDownList ID="ddlRegioni" runat="server" AutoPostBack="true" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" DataValueField="Codice" Width="250px"
                    AppendDataBoundItems="true" Enabled="true">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                </asp:DropDownList>
            <asp:Label ID="Label7" runat="server" TabIndex="2">Pr.:</asp:Label>
                <asp:DropDownList ID="ddlProvince" runat="server" AutoPostBack="false" DataSourceID="SqlDSProvince" DataTextField="Descrizione" DataValueField="Codice" Width="250px"
                    AppendDataBoundItems="true" Enabled="true">
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                </asp:DropDownList>
            <asp:Button ID="btnAbbinaRegPr" runat="server" Text="Abbina" OnClick="btnAbbinaRegPr_Click" />
            <asp:Button ID="btnEliminaRegPr" runat="server" Text="Elimina" OnClick="btnEliminaRegPr_Click" />
        </div>
    </asp:Panel>
</asp:Panel>
