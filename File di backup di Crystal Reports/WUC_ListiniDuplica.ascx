<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ListiniDuplica.ascx.vb" Inherits="SoftAziOnLine.WUC_ListiniDuplica" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_AnagrProvv_Insert.ascx" tagname="WFPAnagrProvvInsert1" tagprefix="uc2" %>
<style type="text/css">
    .style3
    {
        width: 250px;
        height: 25px;
    }
    .style4
    {
        width: 120px;
        height: 28px;
    }
    .style5
    {
        height: 28px;
    }
    .style6
    {
        width: 74px;
        height: 25px;
    }
    .style7
    {
        width: 120px;
        height: 25px;
    }
    .style8
    {
        height: 25px;
    }
    .style9
    {
        height: 45px;
    }
</style>
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="None" Width="835px" Height="495px" CssClass ="sfondopagine">
    <asp:Panel ID="panel1" runat="server" BorderStyle="Double"  Width="835px" Height="160px" CssClass ="sfondopagine">
    
    <table style="width:98%;">
        <tr>
            <td class="style3">
                Listino da duplicare</td>
            <td class="style4">
                <asp:DropDownList ID="ddlListinoDa" runat="server" Width="100%">
                </asp:DropDownList>
            </td>
            <td class="style5" colspan ="2">
                <asp:TextBox ID="txtListinoDaDesc" runat="server" Width="100%"></asp:TextBox>
                </td>
        </tr>
        <tr>
            <td class="style3">
                Listino da aggiornare o da creare nuovo</td>
            <td class="style7">
                <asp:DropDownList ID="ddlListinoA" runat="server" Width="100%">
                </asp:DropDownList>
            </td>
            <td class="style8" colspan ="2">
                <asp:TextBox ID="txtListinoADesc" runat="server" Width="100%"></asp:TextBox>
             </td>
        </tr>
        <tr>
            <td class="style3">&nbsp;</td>
            <td class="style4" align="left">
                Sconto 1 %                
            </td>
            <td class="style5" align="left">
                Sconto 2 %
            </td>
            <td class="style5" align="left">
                <asp:RadioButton ID="rbtnNoPrezzo" runat="server" Text="Senza prezzo" AutoPostBack="True" GroupName="Tipo" />                
            </td>            
        </tr>        
        <tr>
            <td class="style3">Sconti:</td>
            <td class="style4" align="center">                
                <asp:TextBox ID="txtSconto1" runat="server"></asp:TextBox>
            </td>
            <td class="style5" align="left">
                <asp:TextBox ID="txtSconto2" runat="server"></asp:TextBox>                
            </td>
            <td class="style5" align="left">
                <asp:RadioButton ID="rbtnRicarico" runat="server" Text="Ricarico %" AutoPostBack="True" GroupName="Tipo" />
                <asp:TextBox ID="TxtPercRicarico" runat="server" Width="35px"></asp:TextBox>
            </td>  
        </tr>
    </table>
    </asp:Panel>   
    <asp:Panel ID="panel2" runat="server" BorderStyle="Double"  Width="835px" Height="330px" CssClass ="sfondopagine">
        <table style="width:100%;">
            <tr>
                <td colspan="2">
                    <asp:Label ID="LblCategorie" runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" BackColor="White"
                            Style="text-align:center" Text="Categorie presenti nel listino da duplicare, selezionare quelle desiderate" Width="99%">
                     </asp:Label>                                
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Panel ID="panel3" runat="server" ScrollBars="Auto" Width="810px" Height="240px" CssClass ="sfondopagine">
                        <asp:GridView ID="GridViewCat" runat="server" 
                        GridLines="None" CssClass="GridViewStyle" 
                        AllowSorting="True" AutoGenerateColumns="False" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="Codice"
                        ShowFooter="false" DataSourceID="SqlDSCategArt">
                        <RowStyle CssClass="RowStyle" />
                        <PagerStyle CssClass="PagerStyle" />
                        <HeaderStyle CssClass="HeaderStyle" />
                        <AlternatingRowStyle CssClass="AltRowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                        <PagerSettings Mode="NextPrevious" Visible="false"/>
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox id="checkSel"  AutoPostBack="false" Checked="false" runat="server" Width ="10px" />
                                </ItemTemplate>
                            </asp:TemplateField>                            
                            <asp:BoundField DataField="Descrizione" HeaderText="Descrizione" SortExpression="Descrizione" />
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="SqlDSCategArt" runat="server" 
                     SelectCommand="SELECT * FROM [CategArt] ORDER BY Descrizione"></asp:SqlDataSource>
                     </asp:Panel> 
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>            
            <tr>
                <td>
                    <asp:CheckBox ID="chkArtPreEs" runat="server" Text="Aggiornamento articoli preesistenti" />
                </td>
                <td>
                    <asp:CheckBox ID="SelTutto" runat="server" Text="Seleziona tutte le categorie" />
                </td>
            </tr>
        </table>       
        <div>
        &nbsp;
        </div>
        <div>
        &nbsp;
        </div>
        <asp:Button ID="BtnTestInsArticoli" runat="server" Text ="Test"  />
    </asp:Panel> 
</asp:Panel>
<uc2:WFPAnagrProvvInsert1 ID="WFPAnagrProvvInsert1" runat="server" />

