<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DestMerce.ascx.vb" Inherits="SoftAziOnLine.WUC_DestMerce" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControl/WFP_Elenco.ascx" TagName="WFPElenco" TagPrefix="wuc" %>
<wuc:WFPElenco ID="WFPElencoNazioni" runat="server" Tabella="Nazioni" Titolo="Elenco Nazioni"/>
<wuc:WFPElenco ID="WFPElencoProvince" runat="server" Tabella="Province" Titolo="Elenco Province"/>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<asp:SqlDataSource ID="SqlDSDestMerce" runat="server" 
    SelectCommand="SELECT Codice, Progressivo, Ragione_Sociale, Indirizzo, Cap, Localita, Provincia, Stato, Denominazione, Riferimento, Telefono1, Telefono2, Fax, EMail, CONVERT(INT, Tipo) AS Tipo,Ragione_Sociale35,Riferimento35 FROM [DestClienti] WHERE ([Codice] = @Codice) ORDER BY Ragione_Sociale" 
    UpdateCommand="InsertUpdate_DestClienti" 
    UpdateCommandType="StoredProcedure"
    DeleteCommand="Delete_DestClienti"
    DeleteCommandType="StoredProcedure">
    <SelectParameters>
        <asp:SessionParameter Name="Codice" SessionField="Codice_CoGeDM" Type="String" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
        <asp:Parameter Name="Progressivo" Type="Int32" />
        <asp:Parameter Name="Codice" Type="String" />
        <asp:Parameter Name="Tipo" Type="String" />
        <asp:Parameter Name="Ragione_Sociale" Type="String" />
        <asp:Parameter Name="Denominazione" Type="String" />
        <asp:Parameter Name="Riferimento" Type="String" />
        <asp:Parameter Name="Indirizzo" Type="String" />
        <asp:Parameter Name="CAP" Type="String" />
        <asp:Parameter Name="Localita" Type="String" />
        <asp:Parameter Name="Provincia" Type="String" />
        <asp:Parameter Name="Stato" Type="String" />
        <asp:Parameter Name="Telefono1" Type="String" />
        <asp:Parameter Name="Telefono2" Type="String" />
        <asp:Parameter Name="Fax" Type="String" />
        <asp:Parameter Name="Email" Type="String" />
        <asp:Parameter Name="Ragione_Sociale35" Type="String" />
        <asp:Parameter Name="Riferimento35" Type="String" />
    </UpdateParameters>
    <DeleteParameters>
        <asp:Parameter Name="Progressivo" Type="Int32" />
    </DeleteParameters>
</asp:SqlDataSource>
<table align="left" class="sfondopagine" style="width:100%; height:100%;">
    <tr>
        <td align="center" class="sfondopagine" style="width:1070px; height:100%;">
            <asp:Panel ID="PanelBody" runat="server" BorderStyle="groove" BorderWidth="1px">
            <div id="divGridViewDestMerce" style="overflow: auto; height:240px; width:1070px; border-style:groove">
                <asp:GridView ID="GridViewBody" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."   
                    DataKeyNames="Progressivo" DataSourceID="SqlDSDestMerce" 
                    AllowSorting="True"
                    AllowPaging="true"
                    PageSize="100" 
                    PagerStyle-HorizontalAlign="Center" 
                    PagerSettings-Mode="NextPreviousFirstLast"
                    PagerSettings-Visible="true"
                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                    EnableTheming="True" BackColor="Silver" PagerSettings-Position="Bottom">
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
                        <asp:CommandField ButtonType="Button" CausesValidation="False" 
                            InsertVisible="False" SelectText="&gt;" 
                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" 
                            ControlStyle-Font-Size="XX-Small" >
                        <ControlStyle Font-Size="XX-Small" />
                        </asp:CommandField>
                        <asp:BoundField DataField="Tipo" HeaderText="Sigla" 
                            SortExpression="Tipo" />
                        <asp:BoundField DataField="Ragione_Sociale" HeaderText="Ragione sociale" 
                            SortExpression="Ragione_Sociale" />
                        <asp:BoundField DataField="Denominazione" HeaderText="Denominazione" 
                            SortExpression="Denominazione" />
                        <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                            SortExpression="Riferimento" />
                        <asp:BoundField DataField="Indirizzo" HeaderText="Indirizzo" 
                            SortExpression="Indirizzo" />
                        <asp:BoundField DataField="Cap" HeaderText="CAP" 
                            SortExpression="Cap" />
                        <asp:BoundField DataField="Localita" HeaderText="Località" 
                            SortExpression="Localita" />
                        <asp:BoundField DataField="Provincia" HeaderText="Pr." 
                            SortExpression="Provincia" />
                        <asp:BoundField DataField="Stato" HeaderText="Naz." 
                            SortExpression="Stato" />
                        <asp:BoundField DataField="Telefono1" HeaderText="Tel.(1)" 
                            SortExpression="Telefono1" />
                        <asp:BoundField DataField="Telefono2" HeaderText="Tel.(2)" 
                            SortExpression="Telefono2" />
                        <asp:BoundField DataField="Fax" HeaderText="Fax" 
                            SortExpression="Fax" />
                        <asp:BoundField DataField="Email" HeaderText="E-mail" 
                            SortExpression="Email" />    
                    </Columns>
                </asp:GridView>
            </div>    
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Sigla</td>
                    <td><asp:TextBox ID="txtTipo" AutoPostBack="False" runat="server" Width="70px" MaxLength="5" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:215px">Ragione sociale</td>
                    <td><asp:TextBox ID="txtRagSoc" AutoPostBack="false" runat="server" Width="425px" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Denominazione</td>
                    <td><asp:TextBox ID="txtDenominazione" AutoPostBack="false" runat="server" Width="425px" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                     <td style="width:110px">Riferimento</td>
                    <td><asp:TextBox ID="txtRiferimento" AutoPostBack="false" runat="server" Width="950px" MaxLength="500" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px"><asp:Label ID="Label1" runat="server" Text="Dati Corriere" ForeColor="Blue" Font-Bold="true"></asp:Label></td>
                    <td style="width:124px"><asp:Label ID="Label2" runat="server" Text="Ragione sociale" ForeColor="Blue"></asp:Label></td>
                    <td><asp:TextBox ID="txtRagSoc35" AutoPostBack="false" runat="server" Width="300px" MaxLength="35" BorderStyle="None" BackColor="Aqua"></asp:TextBox></td>
                    <td style="width:100px"></td>
                    <td style="width:110px"><asp:Label ID="Label3" runat="server" Text="Riferimento" ForeColor="Blue"></asp:Label></td>
                    <td><asp:TextBox ID="txtRiferimento35" AutoPostBack="false" runat="server" Width="300px" MaxLength="35" BorderStyle="None" BackColor="Aqua"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Indirizzo</td>
                    <td><asp:TextBox ID="txtIndirizzo" AutoPostBack="false" runat="server" Width="425px" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:80px">CAP</td>
                    <td><asp:TextBox ID="txtCap" AutoPostBack="false" runat="server" Width="50px" MaxLength="5" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Località</td>
                    <td><asp:TextBox ID="txtLocalita" AutoPostBack="false" runat="server" Width="425px" MaxLength="50" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:80px">Provincia</td>
                    <td><asp:Button ID="btnTrovaProv" runat="server" Text="?" Width="30px" Height="22px"/></td>
                    <td><asp:TextBox ID="txtProvincia" AutoPostBack="false" runat="server" Width="30px" MaxLength="2" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:58px">Nazione</td>
                    <td><asp:Button ID="btnTrovaNazione" runat="server" Text="?" Width="30px" Height="22px"/></td>
                    <td><asp:TextBox ID="txtCodNazione" AutoPostBack="false" runat="server" Width="40px" MaxLength="3" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">Telefono 1</td>
                    <td><asp:TextBox ID="txtTel1" AutoPostBack="false" runat="server" Width="180px" MaxLength="30" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:105px">Telefono 2</td>
                    <td><asp:TextBox ID="txtTel2" AutoPostBack="false" runat="server" Width="180px" MaxLength="30" BorderStyle="None"></asp:TextBox></td>
                    <td align="right" style="width:55px">Fax</td>
                    <td><asp:TextBox ID="txtFax" AutoPostBack="false" runat="server" Width="180px" MaxLength="30" BorderStyle="None"></asp:TextBox></td>
                    <td><asp:Label ID="lblMessTel" runat="server" Text="Nota Telefono 1 Telefono 2 Obbligatori per le Spedizioni" ForeColor="Blue" Visible="false"></asp:Label></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td style="width:110px">E-mail</td>
                    <td><asp:TextBox ID="txtEMail" AutoPostBack="false" runat="server" Width="720px" MaxLength="100" BorderStyle="None"></asp:TextBox></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:Button ID="btnNuovo" runat="server" Text="Nuovo" class="btnstyle"/>&nbsp;&nbsp;
            <asp:Button ID="btnModifica" runat="server" Text="Modifica"  class="btnstyle"/>&nbsp;&nbsp;
            <asp:Button ID="btnElimina" runat="server" Text="Elimina"  class="btnstyle"/>&nbsp;&nbsp;
            <asp:Button ID="btnAggiorna" runat="server" Text="Aggiorna"  class="btnstyle"/>&nbsp;&nbsp;
            <asp:Button ID="btnAnnulla" runat="server" Text="Annulla"  class="btnstyle"/>
        </td>
    </tr>
</table>