<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_EvadiDocumenti.ascx.vb" Inherits="SoftAziOnLine.WUC_EvadiDocumenti" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
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
        
    .wrap { white-space: normal; width: 100px; }
    .style9
    {
        width: 504px;
    }
</style>    
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="980px" CssClass ="sfondopagine">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup1" runat="server" />
    <table border="0" cellpadding="0" frame="box" style="width:930; margin-right:0;">
             <tr>
                <td colspan ="2">
                    <table width="100%">
                    <tr>
                    <td align ="left" class="style9">
                        Clienti che hanno almeno un ordine da evadere o parzialmente evaso :</td>
                    <td align ="right">
                    <asp:Button ID="BtnVisualizza" runat="server"  Text="Visualizza ordini del cliente selezionato" />
                    </td>
                    </tr>
                    </table>
                </td>
                 <td>                    
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td align="left" ><asp:TextBox ID="txtRicerca" runat="server" Width="100px"></asp:TextBox></td>
                <td align="left"><asp:DropDownList ID="ddlRicerca" runat="server" AutoPostBack="True" Width="750px">
                    </asp:DropDownList>
                </td>
                <td>                    
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td align="left" colspan ="2">
                    <asp:Label ID="LblOrdiniTest" runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" BackColor="White"
                            Style="text-align:center" Text="Ordini da evadere o parzialmente evasi del cliente selezionato" Width="99%">
                     </asp:Label>                                     
                <asp:Panel ID="pnlTestata" runat="server" BorderStyle="Double" Width="850px" Height="130px" ScrollBars="Auto"  CssClass="sfondopagine">
                    <asp:UpdatePanel ID="UpdPnlTestata" runat="server" >
                            <ContentTemplate>                            
                            <asp:GridView ID="GridViewTestata" runat="server" AutoGenerateColumns="False" 
                            EmptyDataText="Nessun dato disponibile."  
                            DataKeyNames="IDDocumenti" EnableTheming="False" GridLines="None" 
                            DataSourceID="SqlDSDocumentiT">
                               <AlternatingRowStyle CssClass="AltRowStyle" />
                              <Columns><asp:TemplateField InsertVisible="False">
                                <ItemTemplate><asp:Button ID="Button1" runat="server" 
                                CausesValidation="False" CommandName="Select" Text="&gt;" />
                                </ItemTemplate>
                                 <controlstyle font-size="XX-Small" />
                                </asp:TemplateField> 
                               </Columns> 
                               <HeaderStyle CssClass="HeaderStyle" />
                               <PagerSettings Mode="NextPrevious" Visible="False" />
                               <PagerStyle CssClass="PagerStyle" />
                               <RowStyle CssClass="RowStyle" />
                               <SelectedRowStyle CssClass="SelectedRowStyle" />
                            </asp:GridView>
                            <asp:SqlDataSource ID="SqlDSDocumentiT" runat="server" 
                                SelectCommand="SELECT * FROM [DocumentiT]"></asp:SqlDataSource>
                           </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   </asp:Panel> 
                </td>
                <td align="center" rowspan ="3" >                    
                    <asp:UpdatePanel ID="UpdPnlBtn" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnAltroOrdine" runat="server" class="btnstyle" Text="Seleziona altro ordine"   />
                            </div>
                            <div style="height: 5px">
                            </div>
                            <div>
                                <asp:Button ID="btnEvadiParz" runat="server" class="btnstyle" Text="Evadi parzialmente" />
                            </div>
                            <div>
                                <asp:Button ID="btnEvadiSing" runat="server" class="btnstyle" Text="Evadi singola riga ordine" />
                            </div>
                            <div>
                                <asp:Button ID="btnEvadiAlles" runat="server" class="btnstyle" Text="Evadi righe ALLESTITE" />
                            </div>
                            <div style="height: 5px">
                            </div>
                            <div>
                                <asp:Button ID="btnEvadiDett" runat="server" class="btnstyle" Text="Evadi tutti i dettagli" />
                            </div>
                            <div style="height: 5px">
                            </div>                            
                            <div>
                                <asp:Button ID="BtnConfermaEvas" runat="server" class="btnstyle" Text="Conferma evasione" />
                            </div>                            
                            <div>
                                <asp:Button ID="BtnCreaDDT" runat="server" class="btnstyle" Text="Crea DDT" />
                            </div>                            
                            <div style="height: 5px">
                            </div>                            
                            <div>
                                <asp:Button ID="BtnEliminaRiga" runat="server" class="btnstyle" Text="Elimina riga" />
                            </div>                            
                            <div>
                                <asp:Button ID="BtnEliminaDett" runat="server" class="btnstyle" Text="Elimina tutti i dettagli" />
                            </div>                            
                        </ContentTemplate>
                    </asp:UpdatePanel>                    
                </td>
            </tr>
        <tr>
            <td align="left" colspan ="2">            
                <asp:Label ID="Label3" runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" BackColor="White"
                        Style="text-align:center" Text="Dettaglio ordine" Width="99%">
                 </asp:Label>            
                <asp:Panel ID="pnlDettaglio" runat="server" BorderStyle="Double" Width="850px" Height="130px" ScrollBars="Auto" CssClass="sfondopagine">
                    <asp:UpdatePanel ID="UpdPnlDettaglio" runat="server">
                        <ContentTemplate>
                          <div>
                            <asp:GridView ID="GridViewDettOrdine" runat="server" AutoGenerateColumns="False" 
                               EmptyDataText="Nessun dato disponibile."  
                               DataKeyNames="IDDocumenti" EnableTheming="False" GridLines="None" 
                                  DataSourceID="SqlDSDocumentiD" >
                               <AlternatingRowStyle CssClass="AltRowStyle" />
                               <Columns>
                                    <asp:TemplateField InsertVisible="False">
                                    <ItemTemplate><asp:Button ID="Button1" runat="server" 
                                    CausesValidation="False" CommandName="Select" Text="&gt;" />
                                    </ItemTemplate>
                                     <controlstyle font-size="XX-Small" />
                                    </asp:TemplateField> 
                                   <asp:CommandField ShowEditButton="True" />
                               </Columns>
                               <HeaderStyle CssClass="HeaderStyle" />
                               <PagerSettings Mode="NextPrevious" Visible="False" />
                               <PagerStyle CssClass="PagerStyle" />
                               <RowStyle CssClass="RowStyle" />
                               <SelectedRowStyle CssClass="SelectedRowStyle" />
                           </asp:GridView>
                              <asp:SqlDataSource ID="SqlDSDocumentiD" runat="server" 
                                  SelectCommand="SELECT * FROM [DocumentiD]"></asp:SqlDataSource>
                           </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel> 
                </td>
        </tr>
        
         <tr>
            <td align="left" colspan ="2">
                <asp:Label ID="Label1" runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" BackColor="White"
                        Style="text-align:center" Text="Dettaglio nuovo documento di trasporto (qui puoi modificare la quantità dei pacchi cliccandoci sopra)" Width="99%">
                 </asp:Label>                        
                <asp:Panel ID="pnlDettaglioDoc" runat="server" BorderStyle="Double" Width="850px" Height="130px" ScrollBars="Auto" CssClass="sfondopagine">
                    <asp:UpdatePanel ID="UpdPnlDettaglioDoc" runat="server">
                        <ContentTemplate>
                        <div>
                           <asp:GridView ID="GridViewDettDoc" runat="server" AutoGenerateColumns="False" 
                           EmptyDataText="Nessun dato disponibile."  
                           DataKeyNames="IDDocumenti" EnableTheming="False" GridLines="None" 
                              DataSourceID="SqlDSDocumentiD">
                           <AlternatingRowStyle CssClass="AltRowStyle" />
                           <Columns>
                               <asp:CommandField ShowDeleteButton="True" />
                           </Columns>
                           <HeaderStyle CssClass="HeaderStyle" />
                           <PagerSettings Mode="NextPrevious" Visible="False" />
                           <PagerStyle CssClass="PagerStyle" />
                           <RowStyle CssClass="RowStyle" />
                           <SelectedRowStyle CssClass="SelectedRowStyle" />
                       </asp:GridView>
                       </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel> 
                </td>

        </tr>
</table>
    </ContentTemplate>
 </asp:UpdatePanel>    
 </asp:Panel>
	