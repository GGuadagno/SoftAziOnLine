<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DistintaBase.ascx.vb" Inherits="SoftAziOnLine.WUC_DistintaBase" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
<uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
<table width="800px">
    <tr>
        <td style="width:110px">
            <asp:Button ID="btnInserisciPrima" runat="server" Text="Inserisci prima" Width="100px"/><br />
            <asp:Button ID="btnInserisciDopo" runat="server" Text="Inserisci dopo" Width="100px"/><br />
            <asp:Button ID="btnCancellaRiga" runat="server" Text="Cancella riga" Width="100px"/>
        </td>
        <td>
            <asp:SqlDataSource ID="SqlDataSourceDistBase" runat="server" 
                SelectCommand="get_DistBaseByCodiceArticolo" 
                SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter Name="Codice" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" BorderStyle="Solid" BorderWidth="1px">
            <div id="divGridViewDisB" style="overflow: auto; height: 150px; border-style:groove">
                <asp:GridView ID="GridViewBody" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" 
                    AllowSorting="False" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="CodFiglio"
                    ShowFooter="false">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:CommandField ButtonType="Button" CausesValidation="False" 
                            InsertVisible="False" SelectText="&gt;" 
                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" ControlStyle-Font-Size="XX-Small" />
                        <asp:BoundField DataField="Riga" HeaderText="Riga" 
                            SortExpression="Riga" />
                        <asp:BoundField DataField="CodFiglio" HeaderText="Codice" 
                            SortExpression="CodFiglio" />
                        <asp:BoundField DataField="DesFiglio" HeaderText="Descrizione" 
                            SortExpression="DesFiglio" />
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                            SortExpression="UM" />
                        <asp:BoundField DataField="Quantita" HeaderText="Quantità" 
                            SortExpression="Quantita" />
                    </Columns>
                </asp:GridView>
            </div>
            </asp:Panel>
            <div id="divMod" style="overflow: auto; height:auto; border-style:groove; background-color: Silver;">
              <div>
                  <asp:Label ID="Label6" runat="server" Width="30px">&nbsp;</asp:Label> 
                  <asp:Label ID="lblCodice" runat="server" Width="110px">Codice</asp:Label> 
                  <asp:Label ID="lblDescrizione" runat="server" Width="410px">Descrizione</asp:Label> 
                  <asp:Label ID="lblUM" runat="server" Width="25px">UM</asp:Label>
                  <asp:Label ID="lblQta" runat="server" Width="55px">Quantità</asp:Label> 
              </div>
              <div>
                  <asp:Button ID="BtnSelArticoloIns" runat="server" CommandName="BtnSelArticoloIns" Text="?" ToolTip="Ricerca articoli" />
                  <asp:TextBox ID="txtCodArt" runat="server" Width="105px" MaxLength="20" AutoPostBack="true" ></asp:TextBox>
                  <asp:Label ID="LblDesArt" runat="server" BorderStyle="Outset" Width="400px" Font-Bold="True"></asp:Label>
                  <asp:TextBox ID="txtUM" runat="server" MaxLength="2" Width="25px" AutoPostBack="false"></asp:TextBox>
                  <asp:TextBox ID="txtQta" runat="server" MaxLength="7" Width="65px" AutoPostBack="false"></asp:TextBox>
              </div>
              <div>&nbsp;
              </div>
            </div>
            <asp:Button ID="btnModificaRiga" runat="server" Text="Modifica riga" Width="18%"/>
        </td>
    </tr>
    <tr>
        <td style="width:110px"></td>
        <td>
        <asp:Label ID="lblMess" runat="server" Width="100%" Visible="false" Font-Bold="True" ForeColor="Red" Text=""></asp:Label>
        </td>
    </tr>
</table>