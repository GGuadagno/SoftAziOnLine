﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_FatturazioneDDT.ascx.vb" Inherits="SoftAziOnLine.WUC_FatturazioneDDT" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WUC_Attesa.ascx" tagname="Attesa" tagprefix="uc3" %>
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
    .style1
    {
        height: 35px;
        }
    .styleBordo
    {
        height: 35px;
        width: 860px;
        border-style:groove;
    }
    .style2
    {
        height: 35px;
    }
    .style3
    {
        width: auto;
        border-style: none;
        height: 185px;
    }
    .style5
    {
        width: auto;
        border-style: none;
        height: 80px;
    }
    .style6
    {
        height: 239px;
    }
    .style7
    {
        height: 185px;
    }
</style>    
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" 
    Width="980px" Height="550px" CssClass ="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
     <uc3:Attesa ID="Attesa" runat="server" />
    <asp:SqlDataSource ID="SqlDSTipoFatt" runat="server" 
            SelectCommand="SELECT * FROM [TipoFatt] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
    <table border="0" cellpadding="0" frame="box" 
            style="width:auto; height:550px; margin-right:0;">
        </td>
    </tr>
            <tr>
                <td align="left" class="style1" colspan="2">
                    <table>
                    <tr>
                        <td align="left" class="style1">
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                                AutoPostBack="True" Width="240px">
                            </asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="275px"></asp:TextBox>
                            &nbsp;<asp:Button ID="btnRicerca" runat="server"  
                                Text="Cerca documento" Width="120px" />
                        </td>
                    </tr>
                    </table> 
                </td>
            </tr>
            <tr>
                <td align="left" class="styleBordo" colspan="2">
                    <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="55px">
                    <table style="width: 960px">
                        <tr>
                            <td style="width:300px" align="right" >
                                <asp:Label ID="Label2" runat="server" BorderColor="White"
                                BorderStyle="None" Font-Bold="false" ForeColor="Black" 
                                Text="Fattura tutti i documenti fino alla data del" ></asp:Label>&nbsp;
                            </td> 
                            <td style="width:150px">
                                <asp:TextBox ID="txtDataA" runat="server" MaxLength="10" TabIndex="3" 
                                Width="70px" AutoPostBack="true" ></asp:TextBox>
                                <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" 
                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="4" 
                                ToolTip="apri il calendario" />
                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                TargetControlID="txtDataA"></asp:CalendarExtender>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                                runat="server" ControlToValidate="txtDataA" ErrorMessage="*" 
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                            </td>
                            <td style="width:400px">
                                <div>
                                <asp:CheckBox ID="chkTipoFT" runat="server" 
                                    Text="Tipo Fatturazione" AutoPostBack="True" Visible="true" GroupName="Tipo" Font-Bold="false"/>
                                <asp:DropDownList ID="ddlTipoFattur" runat="server" 
                                AutoPostBack="True"  DataSourceID="SqlDSTipoFatt" DataTextField="Descrizione" 
                                DataValueField="Codice" Width="250px">
                                <asp:ListItem Text="" Value="" ></asp:ListItem>
                                </asp:DropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:300px" align="left">
                                &nbsp;<asp:Label ID="Label4" runat="server" BorderColor="White"
                                    BorderStyle="None" Font-Bold="false" ForeColor="Black" 
                                    Text="Dal numero DDT"></asp:Label>&nbsp;&nbsp;
                                    <asp:TextBox ID="txtDalNDDT" runat="server" MaxLength="10" TabIndex="3" 
                                    Width="70px" Font-Bold="false"></asp:TextBox>
                                    &nbsp;&nbsp;<asp:Label ID="Label5" runat="server" BorderColor="White"
                                    BorderStyle="None" Font-Bold="false" ForeColor="Black" 
                                    Text="Al numero DDT" ></asp:Label>
                            </td> 
                            <td style="width:150px">
                                <div>
                                    <asp:TextBox ID="txtAlNDDT" runat="server" MaxLength="10" TabIndex="3" 
                                    Width="70px"></asp:TextBox>
                                    <asp:Button ID="btnCercaDDTNum" runat="server"  
                                    Text="Cerca" />
                                </div>
                            </td>
                            <td style="width:400px">
                                <div>
                                    &nbsp;<asp:Label ID="Label1" runat="server" BorderColor="White"
                                    BorderStyle="None" Font-Bold="false" ForeColor="Black" 
                                    Text="Primo N° di fattura"></asp:Label>&nbsp;&nbsp;
                                    <asp:TextBox ID="txtPrimoNFattura" runat="server" MaxLength="10" TabIndex="3" 
                                    Width="80px" Enabled="false" Font-Bold="true"></asp:TextBox>
                                    &nbsp;&nbsp;<asp:Label ID="Label3" runat="server" BorderColor="White"
                                    BorderStyle="None" Font-Bold="false" ForeColor="Black" 
                                    Text="Data" ></asp:Label>
                                    <asp:TextBox ID="txtDataFattura" runat="server" MaxLength="10" TabIndex="3" 
                                    Width="70px"></asp:TextBox>
                                    <asp:ImageButton ID="ImageButtonDF" runat="server" 
                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="4" 
                                    ToolTip="apri il calendario" />
                                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" 
                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="ImageButtonDF" 
                                    TargetControlID="txtDataFattura"></asp:CalendarExtender>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" 
                                    runat="server" ControlToValidate="txtDataFattura" ErrorMessage="*" 
                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                </div>
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                </td>
              </tr>
            <tr>
                <td align="left" class="style3">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:860px; height:195px; border-style:groove;">
                        <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" DataSourceID="SqlDSPrevTElenco" AllowSorting="True"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns><asp:TemplateField InsertVisible="False">
                        <ItemTemplate><asp:Button ID="Button1" runat="server" 
                        CausesValidation="False" CommandName="Select" Text="&gt;" />
                        </ItemTemplate>
                        <controlstyle font-size="XX-Small" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10px" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Tipo_Doc" HeaderText="Tipo" 
                        SortExpression="Tipo_Doc"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                                SortExpression="DesStatoDoc"><HeaderStyle Wrap="True" /><ItemStyle 
                                Width="15px" /></asp:BoundField> 
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                    DataFormatString="{0:d}" HeaderText="Numero" ReadOnly="True" 
                                    SortExpression="Numero">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data_Doc" HeaderText="Data" 
                                    SortExpression="Data_Doc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DataOraConsegna" HeaderText="Data consegna" 
                                    SortExpression="DataOraConsegna">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Cod_Cliente" HeaderText="Codice Cliente" 
                                    SortExpression="Cod_Cliente">
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
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                                    HeaderText="Località" ReadOnly="True" 
                                    SortExpression="Localita">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Provincia" 
                                    HeaderText="Pr." ReadOnly="True" 
                                    SortExpression="Provincia">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                                    HeaderText="CAP" ReadOnly="True" 
                                    SortExpression="CAP">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Partita_IVA" 
                                    HeaderText="Partita IVA" ReadOnly="True" 
                                    SortExpression="Partita_IVA">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Codice_Fiscale" 
                                    HeaderText="Codice Fiscale" ReadOnly="True" 
                                    SortExpression="Codice_Fiscale">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="15px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Data_Validita" HeaderText="Data validità" 
                                    SortExpression="Data_Validita">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                                    SortExpression="Riferimento">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="50px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="DesCausale" HeaderText="Causale" 
                                    SortExpression="DesCausale"><HeaderStyle Wrap="True" /><ItemStyle 
                                    Width="10px" /></asp:BoundField>
                                <asp:BoundField DataField="TotaleDoc" HeaderText="Totale documento" 
                                    SortExpression="TotaleDoc"><HeaderStyle Wrap="True" /><ItemStyle 
                                    Width="10px" /></asp:BoundField>
                                <asp:BoundField DataField="TotNettoPagare" HeaderText="Tot.Netto a pagare" 
                                    SortExpression="TotNettoPagare"><HeaderStyle Wrap="True" /><ItemStyle 
                                    Width="10px" /></asp:BoundField>
                                <asp:BoundField DataField="TotaleImp" HeaderText="Totale imponibile" 
                                    SortExpression="TotaleImp"><HeaderStyle Wrap="True" /><ItemStyle 
                                    Width="10px" /></asp:BoundField>
                                <asp:BoundField DataField="TotaleIVA" HeaderText="Totale IVA" 
                                    SortExpression="TotaleIVA"><HeaderStyle Wrap="True" /><ItemStyle 
                                    Width="10px" /></asp:BoundField>
                                <asp:BoundField DataField="FatturaPA" HeaderText="PA" 
                                    SortExpression="FatturaPA">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="SplitIVA" HeaderText="Split" 
                                    SortExpression="SplitIVA">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="RitAcconto" HeaderText="Rit.Acc." 
                                    SortExpression="RitAcconto">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Cod_Valuta" HeaderText="Valuta" 
                                    SortExpression="Cod_Valuta">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Cod_Pagamento" HeaderText="C.Pag." 
                                    SortExpression="Cod_Pagamento">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="ABI" HeaderText="ABI" 
                                    SortExpression="ABI">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="CAB" HeaderText="CAB" 
                                    SortExpression="CAB">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="IDDocumenti" HeaderText="IDDocumenti" ReadOnly="True" 
                                    SortExpression="IDDocumenti">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                        </Columns>
                        <HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                            SelectCommand="get_DocTElencoFatture" 
                            SelectCommandType="StoredProcedure">
                        </asp:SqlDataSource>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </td>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnCreaTutte" runat="server" OnClick="DisableAll" OnClientClick="DisableAll" class="btnstyle" Text="Stampa tutte" />
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div style="height: 40px">
                                <asp:CheckBox ID="chkFatturaPA" runat="server" Text="Seleziona DDT Fattura PA" Checked="false" AutoPostBack="true" Font-Bold="true"/>
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div style="height: 15px">
                            </div>
                            <div>
                                <asp:Button ID="btnCreaFattura" runat="server" OnClick="DisableAll" OnClientClick="DisableAll" class="btnstyle" Text="Singola Fattura"/>
                            </div>
                            <div style="height: 15px">
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        <tr>
            <td align="left">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                <div id="divGridViewPrevD" style="overflow:auto; width:860px; height:230px; border-style:groove;">
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" 
                        DataSourceID="SqlDSPrevDByIDDocumenti"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="30px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="80px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Ordinata"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità evasa" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Evasa"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Residua"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Allestita" HeaderText="Quantità inviata" HeaderStyle-Wrap="true" 
                        SortExpression="Qta_Allestita"><HeaderStyle /><ItemStyle Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
                        SortExpression="Cod_IVA"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                        SortExpression="Prezzo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoValore" HeaderText="Sconto valore" 
                        SortExpression="ScontoValore"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Sconto_1" HeaderText="Sc.(1)" 
                        SortExpression="Sconto_1"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Importo" HeaderText="Importo" 
                        SortExpression="Importo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoReale" HeaderText="Sc.Riga" 
                        SortExpression="ScontoReale"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        </Columns>
                        <HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                    <asp:SqlDataSource ID="SqlDSPrevDByIDDocumenti" runat="server" 
                        SelectCommand="get_PrevDByIDDocumenti" 
                        SelectCommandType="StoredProcedure">
                        <SelectParameters>
                            <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocumenti" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    </div>
            </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td align="left" class="style6">
                    <div style="height: 15px">
                    </div>
                    <div style="height: 40px">
                                <asp:CheckBox ID="ChkStampaSingFC" runat="server" Text="Chiedi stampa singola fattura" Checked="false" AutoPostBack="false" Font-Bold="true" Visible="false"/>
                            </div>
                            <div style="height: 15px">
                            </div>
                    <div style="height: 15px">
                    </div>
                    <div>
                        <asp:Button ID="btnModifica" runat="server" OnClick="DisableAll" OnClientClick="DisableAll" class="btnstyle" Text="Modifica"/>
                    </div>
                    <div style="height: 15px">
                    </div>
                    <div>
                        <asp:Button ID="btnChiudi" runat="server" OnClick="DisableAll" OnClientClick="DisableAll" class="btnstyle" Text="Chiudi" />
                    </div>
                    <div style="height: 15px">
                    </div>
                    <div style="height: 15px">
                    </div>
                </td>
        </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>