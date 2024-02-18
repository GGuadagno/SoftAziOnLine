﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ElencoPropostaRiordino.ascx.vb" Inherits="SoftAziOnLine.WUC_ElencoPropostaRiordino" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
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
    Width="980px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
<table border="0" cellpadding="0" frame="box" style="width:auto; height:550px; margin-right:0;">
            <tr>
                <td align="left" class="style1" colspan="2">
                    &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                        AutoPostBack="True" Width="240px">
                    </asp:DropDownList>
                    &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                    &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="275px"></asp:TextBox>
                    &nbsp;<asp:Button ID="btnRicerca" runat="server"  
                        Text="Cerca proposta" Width="120px" />
                </td>
            </tr>
            <%--<tr>
                <td align="left" class="styleBordo">
                    <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="25px">
                    <div>
                    &nbsp;Seleziona:&nbsp;
                        <asp:RadioButton ID="rbtnConfermati" runat="server" Text="Confermati" AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label0" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnDaConfermare" runat="server" Text="Da confermare" AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label1" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnNonConferm" runat="server" Text="Non confermabile" AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label2" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnChiusoNoConf" runat="server" Text="Chiuso non confermato" AutoPostBack="True" GroupName="Tipo" />
                        <asp:Label ID="Label5" runat="server" Width="50px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" AutoPostBack="True" GroupName="Tipo" />
                    </div>
                    </asp:Panel>
                </td>
                <td align="left" class="style2">
                    <div>
                        <asp:Button ID="btnCambiaStato" runat="server" class="btnstyle" Text="Cambia stato" />
                    </div>
                </td>
            </tr>--%>
            <tr>
                <td align="left" class="style3">
                <div id="divGridViewPrevT" style="overflow:auto; width:860px; height:250px; border-style:groove;">
                        <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                            CssClass="GridViewStyle" 
                            EmptyDataText="Nessun dato disponibile."  
                            DataKeyNames="IDDocumenti" 
                            DataSourceID="SqlDSPrevTElenco" EnableTheming="True" GridLines="None" AllowSorting="True">
                            <AlternatingRowStyle CssClass="AltRowStyle" />
                            <Columns>
                                <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                    ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                                    ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                                    <ControlStyle Font-Size="XX-Small" />
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                                </asp:CommandField>
                                <%--<asp:BoundField DataField="DesStatoPR" HeaderText="Stato" 
                                SortExpression="DesStatoPR"><HeaderStyle Wrap="True" /><ItemStyle 
                                Width="15px" /></asp:BoundField> --%>
                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                                    DataFormatString="{0:d}" HeaderText="Numero" ReadOnly="True" 
                                    SortExpression="Numero">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                               <%-- <asp:BoundField ApplyFormatInEditMode="True" DataField="RevisioneNDoc" 
                                    DataFormatString="{0:d}" HeaderText="Rev.N°" ReadOnly="True" 
                                    SortExpression="RevisioneNDoc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="5px" />
                                </asp:BoundField>--%>
                                <asp:BoundField DataField="Data_Doc" HeaderText="Data" 
                                    SortExpression="Data_Doc">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Cod_Fornitore" HeaderText="Codice Fornitore" 
                                    SortExpression="Cod_Fornitore">
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
                                <asp:BoundField DataField="DataOraConsegna" HeaderText="Data consegna" 
                                    SortExpression="DataOraConsegna">
                                    <HeaderStyle Wrap="True" />
                                    <ItemStyle Width="10px" />
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
<%--                                <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione1" 
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
                                </asp:BoundField>--%>
                            </Columns>
                            <HeaderStyle CssClass="HeaderStyle" />
                            <PagerSettings Mode="NextPrevious" Visible="False" />
                            <PagerStyle CssClass="PagerStyle" />
                            <RowStyle CssClass="RowStyle" />
                            <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                        <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                            SelectCommand="get_PrevTElencoFOR" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="PF" Name="TipoDoc" SessionField="TipoDoc" Type="String" />
                                <asp:SessionParameter DefaultValue="0" Name="StatoDoc" SessionField="StatoDoc" Type="Int32" />
                                <asp:SessionParameter DefaultValue="N" Name="SortPrevTEl" SessionField="SortPrevTEl" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                </td>
                <td align="left" class="style7">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnVisualizza" runat="server" class="btnstyle" Text="Visualizza" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyle" Text="Nuovo" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyle" Text="Elimina" />
                            </div>
                            <div style="height: 5px"></div>
                            <div>
                                <asp:Button ID="btnCreaOrdine" runat="server" class="btnstyle" Text="Crea ordine" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        <tr>
            <td align="left" class="style5">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
                <div id="divGridViewPrevD" style="overflow:auto; width:860px; height:230px; border-style:groove;">
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato selezionato."  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" 
                        DataSourceID="SqlDSPrevDByIDDocumenti"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" DataFormatString="{0:d}" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="20px" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="80px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
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
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div>
                    <div style="height: 15px">&nbsp;</div> 
                    <%--<div style="height: 15px; text-align:center"><b>Stampe</b></div> --%>
                    <div style="height: 15px">&nbsp;</div> 
                    <div style="height: 15px">&nbsp;</div> 
                    <div style="height: 15px">&nbsp;</div> 
                    <div style="height: 15px">&nbsp;</div> 
                    <%--<div style="height: 15px">
                        <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" />
                            </div>--%>
                    <div style="height: 15px">&nbsp;</div> 
                </td>
           </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel> 
</asp:Panel>