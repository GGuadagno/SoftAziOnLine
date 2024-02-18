<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_CambiaStatoOC.ascx.vb" Inherits="SoftAziOnLine.WUC_CambiaStatoOC" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>

<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr> 
        <td >
            <asp:SqlDataSource ID="SqlDSDocT" runat="server" 
                            SelectCommand="get_DocTResiCFByID" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocCambiaSt" Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
            <asp:Panel ID="Panel1" runat="server" Height="150px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                <asp:GridView ID="GridViewDocT" runat="server" AutoGenerateColumns="False" 
                    CssClass="GridViewStyle" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti" 
                    DataSourceID="SqlDSDocT" EnableTheming="True" GridLines="None">
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <Columns>
                        <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                        SortExpression="DesStatoDoc"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="15px" /></asp:BoundField> 
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                            DataFormatString="{0:d}" HeaderText="Numero" ReadOnly="True" 
                            SortExpression="Numero">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="10px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Tipo_Doc" 
                            HeaderText="Tipo" ReadOnly="True" 
                            SortExpression="Tipo_Doc">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="5px" />
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
                            <HeaderStyle Wrap="false" />
                            <ItemStyle Width="15px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" 
                            SortExpression="Rag_Soc" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                            <HeaderStyle Wrap="true" />
                            <ItemStyle Width="50px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Denominazione" 
                            HeaderText="Denominazione" ReadOnly="True" 
                            SortExpression="Denominazione" ItemStyle-Wrap="true" HeaderStyle-Wrap="true" FooterStyle-Wrap="true">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="15px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                            HeaderText="Località" ReadOnly="True" 
                            SortExpression="Localita" FooterStyle-Wrap="true" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="15px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                            HeaderText="CAP" ReadOnly="True" 
                            SortExpression="CAP">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="5px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Partita_IVA" 
                            HeaderText="Partita IVA" ReadOnly="True" 
                            SortExpression="Partita_IVA" ItemStyle-Wrap="False" HeaderStyle-Wrap="False" FooterStyle-Wrap="False">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="15px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Codice_Fiscale" 
                            HeaderText="Codice Fiscale" ReadOnly="True" 
                            SortExpression="Codice_Fiscale" FooterStyle-Wrap="False" HeaderStyle-Wrap="False" ItemStyle-Wrap="False">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="15px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                            SortExpression="Riferimento" ShowHeader="true" ItemStyle-Wrap="true" HeaderStyle-Wrap="true" FooterStyle-Wrap="true">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="50px" />
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle CssClass="HeaderStyle" />
                    <PagerSettings Mode="NextPrevious" Visible="False" />
                    <PagerStyle CssClass="PagerStyle" />
                    <RowStyle CssClass="RowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle" />
                </asp:GridView>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td >
            <asp:Label ID="lblModificatoDa" 
                    runat="server" BorderStyle="Outset" Font-Bold="true" Font-Overline="False" 
                    Style="text-align:center" Text="" Width="99%"></asp:Label>
        </td>
    </tr>
    <tr>
        <td >
            <asp:SqlDataSource ID="SqlDSCausNonEvasione" runat="server" 
                SelectCommand="SELECT Codice, Descrizione FROM CausNonEvasione ORDER BY Descrizione" 
                SelectCommandType="Text">
            </asp:SqlDataSource>
            <asp:Panel ID="PanelBody" runat="server" Height="300px" ScrollBars="Auto" BorderStyle="Solid" BorderWidth="1px">
                <asp:Label ID="lblCambioStatoNote" 
                    runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                    Style="text-align:center" Text="CAMBIO STATO - NOTE VARIE DOCUMENTO" Width="99%"></asp:Label>
                <div style="text-align:left">
                    &nbsp;<asp:Label ID="Label2" 
                    runat="server" BorderStyle="None" Font-Bold="true" Font-Overline="False" 
                    Style="text-align:left" Text="Stato:" ></asp:Label>
                    <asp:RadioButton ID="rbtnEvaso" runat="server" Text="Evaso" AutoPostBack="false" 
                        GroupName="Tipo" />
                    <asp:Label ID="Label0" runat="server" Width="10px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnDaEvadere" runat="server" Text="Da evadere" 
                        AutoPostBack="false" GroupName="Tipo" />
                    <asp:Label ID="Label3" runat="server" Width="10px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnParzEvaso" runat="server" Text="Parzialmente evaso" 
                        AutoPostBack="false" GroupName="Tipo" />
                    <asp:Label ID="Label4" runat="server" Width="10px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnChiusoNoEvaso" runat="server" Text="Chiuso non evaso" 
                        AutoPostBack="false" GroupName="Tipo" />
                    <asp:Label ID="Label5" runat="server" Width="10px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnNonEvadibile" runat="server" Text="Non evadibile" 
                        AutoPostBack="false" GroupName="Tipo" />
                    <asp:Label ID="Label9" runat="server" Width="10px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnInAllestimento" runat="server" Text="In Allestimento" 
                        AutoPostBack="false" GroupName="Tipo" Enabled="false" />
                </div>
                <br /> 
                <div style="text-align:left">
                    &nbsp;<asp:Label ID="Label6" 
                    runat="server" BorderStyle="None" Font-Bold="true" Font-Overline="False" 
                    Style="text-align:left" Text="Note documento:" Width="150px"></asp:Label>
                </div>
                <div style="text-align:left">
                    <asp:TextBox ID="txtNote" runat="server" TabIndex="12"
                                                Width="750px" TextMode="MultiLine" ></asp:TextBox>
                </div>
                <br />    
                <asp:Label ID="lblChiusoNonEvaso" 
                    runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                    Style="text-align:center" Text="CHIUSO NON EVASO - NOTE" Width="99%"></asp:Label>
                <div style="text-align:left">
                    &nbsp;<asp:Label ID="Label1" 
                    runat="server" BorderStyle="None" Font-Bold="true" Font-Overline="False" 
                    Style="text-align:left" Text="Causale di non evasione" ></asp:Label>
                    <asp:DropDownList ID="DDLCausNonEvasione" runat="server" AppendDataBoundItems="True" 
                        AutoPostBack="false" Width="300px"
                        DataSourceID="SqlDSCausNonEvasione" DataTextField="Descrizione" 
                        DataValueField="Codice" Height="22px">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;&nbsp;&nbsp;<asp:Label ID="Label7" 
                    runat="server" BorderStyle="None" Font-Bold="true" Font-Overline="False" 
                    Style="text-align:left" Text="Data ora modifica" ></asp:Label>
                    <asp:Label ID="lblDataOraChiusoNonEvaso" 
                    runat="server" BorderStyle="None" Font-Bold="true" Font-Overline="False" 
                    Style="text-align:center" Text="" Width="150px" ></asp:Label>
                </div>
                <%--<br />--%>
                <div style="text-align:left">
                    &nbsp;<asp:Label ID="Label10" 
                    runat="server" BorderStyle="None" Font-Bold="true" Font-Overline="False" 
                    Style="text-align:left" Text="Note di non evasione:" ></asp:Label>
                </div>
                <div style="text-align:left">
                    <asp:TextBox ID="txtNoteNonEvasione" runat="server" TabIndex="12"
                                                Width="750px" TextMode="MultiLine" ></asp:TextBox>
                </div>
            </asp:Panel>
        </td>
    </tr>
</table>
