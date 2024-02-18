<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DocCollegati.ascx.vb" Inherits="SoftAziOnLine.WUC_DocCollegati" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="545px" CssClass ="sfondopagine">
<table style="width:1240px;height:545px;">
    <tr> 
        <td >
            <asp:Panel ID="PanelTestata" runat="server" Height="75px" BorderStyle="Solid" BorderWidth="1px">
            <div id="divGridViewDCT" style="overflow:auto; width:1230px; height:75px; border-style:groove;">
                <asp:GridView ID="GridViewDocT" runat="server" AutoGenerateColumns="False" 
                    CssClass="GridViewStyle" EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti" DataSourceID=""
                     EnableTheming="True" GridLines="None">
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <PagerSettings Mode="NextPrevious" Visible="False" />
                    <PagerStyle CssClass="PagerStyle" />
                    <RowStyle CssClass="RowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle" />
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
                        <asp:BoundField DataField="FatturaPA" 
                            HeaderText="FCPA" ReadOnly="True" 
                            SortExpression="FatturaPA">
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
                </asp:GridView>
            </div>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="PanelTestataCM" runat="server" Height="80px" BorderStyle="Solid" BorderWidth="1px" Visible="false">
            <div id="divGridViewPrevTCM" style="overflow:auto; width:1230px; height:80px; border-style:groove;">
                <asp:GridView ID="GridViewPrevTCM" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="IDDocumenti" DataSourceID="" 
                        AllowPaging="true"
                        PageSize="5" 
                        PagerStyle-HorizontalAlign="Center" 
                        PagerSettings-Mode="NextPreviousFirstLast"
                        PagerSettings-Visible="true"
                        PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                        PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                        PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                        PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                        EnableTheming="True" GridLines="None"
                        BackColor="Silver" AllowSorting="false">
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
                            <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                            SortExpression="DesStatoDoc"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" /></asp:BoundField> 
                            <asp:BoundField DataField="Numero" HeaderText="Numero" 
                                SortExpression="Numero">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="10px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="SiglaCA" 
                                HeaderText="Tipo"  
                                SortExpression="SiglaCA">
                                <HeaderStyle Wrap="false" Width="5px"/>
                                <ItemStyle Width="1px" Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Data_Doc" HeaderText="Data Documento" 
                                SortExpression="Data_Doc">
                                <HeaderStyle Wrap="true"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="DataAccetta" HeaderText="Data Accettazione" 
                                SortExpression="DataAccetta">
                                <HeaderStyle Wrap="true"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <%--<asp:BoundField DataField="PercImp" HeaderText="%Imp. Quantità" 
                                SortExpression="PercImp"><HeaderStyle Wrap="True"/><ItemStyle 
                                Width="15px" Wrap="false"/></asp:BoundField>
                            <asp:BoundField DataField="PercImPorto" HeaderText="%Imp. Importo" 
                                SortExpression="PercImPorto"><HeaderStyle Wrap="True"/><ItemStyle 
                                Width="15px" Wrap="false"/></asp:BoundField>--%>
                            <asp:BoundField DataField="Cod_Cliente" HeaderText="Codice Cliente" 
                                SortExpression="Cod_Cliente">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="10px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" 
                                SortExpression="Rag_Soc">
                                <HeaderStyle Wrap="false" />
                                <ItemStyle Width="50px" Wrap="true" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Denominazione" 
                                HeaderText="Denominazione" ReadOnly="True" 
                                SortExpression="Denominazione">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="15px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                                HeaderText="Località"  
                                SortExpression="Localita">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                                HeaderText="CAP"  
                                SortExpression="CAP">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="5px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Partita_IVA" 
                                HeaderText="Partita IVA"  
                                SortExpression="Partita_IVA">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Codice_Fiscale" 
                                HeaderText="Codice Fiscale"  
                                SortExpression="Codice_Fiscale">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>  
                            <asp:BoundField DataField="DataInizio" HeaderText="Inizio Contratto" 
                                SortExpression="DataInizio">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="10px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="DataFine" HeaderText="Fine Contratto" 
                                SortExpression="DataFine">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="10px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                                SortExpression="Riferimento">                                    
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="50px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione1" 
                                HeaderText="Destinazione(1)"  
                                SortExpression="Destinazione1">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione2" 
                                HeaderText="Destinazione(2)"  
                                SortExpression="Destinazione2">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione3" 
                                HeaderText="Destinazione(3)"  
                                SortExpression="Destinazione3">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="DurataTipo" 
                                HeaderText="DT"  
                                SortExpression="DurataTipo">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="5px"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="DurataNum" 
                                HeaderText="DN"  
                                SortExpression="DurataNum">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="5px"/>
                            </asp:BoundField>
                        </Columns>
                        <%--<HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />--%>
                    </asp:GridView>
            </div>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td >
            <asp:Panel ID="PanelBody" runat="server" Height="200px" BorderStyle="Solid" BorderWidth="1px">
            <div id="divGridViewPrevT" style="overflow:auto; width:1230px; height:200px; border-style:groove;">
                <asp:GridView ID="GridViewDocD" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti" DataSourceID="">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:CommandField ButtonType="Button" CausesValidation="False" 
	                        InsertVisible="False" SelectText="&gt;" 
	                        ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true" 
	                        ControlStyle-Font-Size="XX-Small" >
                            <ControlStyle Font-Size="XX-Small" />
                        <ItemStyle Width="5px" />
                        </asp:CommandField>
                        <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                            SortExpression="DesStatoDoc"><HeaderStyle Wrap="True" /><ItemStyle 
                            Width="15px" /></asp:BoundField> 
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Numero" 
                            DataFormatString="{0:d}" HeaderText="Numero" ReadOnly="True" 
                            SortExpression="Numero">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="10px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Tipo_Doc" HeaderText="Tipo" 
                            ReadOnly="True" SortExpression="Tipo_Doc">
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="5px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="FatturaPA" 
                            HeaderText="FCPA" ReadOnly="True" 
                            SortExpression="FatturaPA">
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
                            SortExpression="Denominazione" ItemStyle-Wrap="true" 
                            HeaderStyle-Wrap="true" FooterStyle-Wrap="true">
                            <FooterStyle Wrap="True" />
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="15px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                            HeaderText="Località" ReadOnly="True" 
                            SortExpression="Localita" FooterStyle-Wrap="true" 
                            HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                            <FooterStyle Wrap="True" />
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
                            SortExpression="Partita_IVA" ItemStyle-Wrap="False" 
                            HeaderStyle-Wrap="False" FooterStyle-Wrap="False">
                            <FooterStyle Wrap="False" />
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="15px" />
                        </asp:BoundField>
                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Codice_Fiscale" 
                            HeaderText="Codice Fiscale" ReadOnly="True" 
                            SortExpression="Codice_Fiscale" FooterStyle-Wrap="False" 
                            HeaderStyle-Wrap="False" ItemStyle-Wrap="False">
                            <FooterStyle Wrap="False" />
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="15px" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                            SortExpression="Riferimento" ShowHeader="true" ItemStyle-Wrap="true" 
                            HeaderStyle-Wrap="true" FooterStyle-Wrap="true">
                            <FooterStyle Wrap="True" />
                            <HeaderStyle Wrap="True" />
                            <ItemStyle Width="50px" />
                        </asp:BoundField>
<%--                        <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione1" 
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
                </asp:GridView>
            </div>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel ID="PanelMessaggi" runat="server" Height="25px">
                <asp:Label ID="lblMessUtenteDett" runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                    Style="text-align:center" Text="" Width="99%"></asp:Label>
           </asp:Panel> 
        </td>
    </tr>
    <tr>
        <td >
            <asp:Panel ID="Panel1DettPR" runat="server" Height="200px" BorderStyle="Solid" BorderWidth="1px" Visible="false">
            <div id="divGridViewPrevPR" style="overflow:auto; width:1230px; height:200px; border-style:groove;">
                <asp:GridView ID="GridViewDettPR" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti" DataSourceID="">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:BoundField ApplyFormatInEditMode="True" 
                        DataField="Cod_Articolo" HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo">
                        <HeaderStyle Width="20px" Wrap="false" />
                        <ItemStyle Width="20px" Wrap="false" /></asp:BoundField>
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
                </asp:GridView>
            </div>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td >
            <asp:Panel ID="Panel1DettOC" runat="server" Height="200px" BorderStyle="Solid" BorderWidth="1px" Visible="false">
            <div id="div1" style="overflow:auto; width:1230px; height:200px; border-style:groove;">
                <asp:GridView ID="GridViewDettOC" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti" DataSourceID="">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:BoundField DataField="Cod_Articolo" 
                        HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo">
                        <HeaderStyle Width="20px" Wrap="false" />
                        <ItemStyle Width="20px" Wrap="false" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" Width="80px"/><ItemStyle 
                        Width="80px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Impegnata" HeaderText="Quantità impegnata" 
                        SortExpression="Qta_Impegnata"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità allestita" 
                        SortExpression="Qta_Evasa"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua" 
                        SortExpression="Qta_Residua"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Allestita" HeaderText="Quantità inviata" 
                        SortExpression="Qta_Allestita"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
                        SortExpression="Cod_IVA"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                        SortExpression="Prezzo"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoValore" HeaderText="Sconto valore" 
                        SortExpression="ScontoValore"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Sconto_1" HeaderText="Sc.(1)" 
                        SortExpression="Sconto_1"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Importo" HeaderText="Importo" 
                        SortExpression="Importo"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoReale" HeaderText="Sc.Riga" 
                        SortExpression="ScontoReale"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                   </Columns>
                </asp:GridView>
            </div>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td >
            <asp:Panel ID="PanelDettDTFC" runat="server" Height="200px" BorderStyle="Solid" BorderWidth="1px" Visible="false">
            <div id="div2" style="overflow:auto; width:1230px; height:200px; border-style:groove;">
                <asp:GridView ID="GridViewDettDTFC" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti" DataSourceID="">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:BoundField DataField="Cod_Articolo" 
                        HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo">
                        <HeaderStyle Width="20px" Wrap="false" />
                        <ItemStyle Width="20px" Wrap="false" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" /><ItemStyle Width="250px" /></asp:BoundField>
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
                        <asp:BoundField DataField="TipoScontoMerce" HeaderText="SM OM" 
                        SortExpression="TipoScontoMerce"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoValore" HeaderText="Sconto valore" 
                        SortExpression="ScontoValore"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Sconto_1" HeaderText="Sc.(1)" 
                        SortExpression="Sconto_1"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Importo" HeaderText="Importo" 
                        SortExpression="Importo"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="DedPerAcconto" HeaderText="Ded." 
                        SortExpression="DedPerAcconto"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoReale" HeaderText="Sc.Riga" 
                        SortExpression="ScontoReale"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Pro_Agente" HeaderText="Provv." 
                        SortExpression="Pro_Agente"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="ImportoProvvigione" HeaderText="Imp.Provv." 
                        SortExpression="ImportoProvvigione"><HeaderStyle Wrap="True" /><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        </Columns>
                </asp:GridView>
            </div>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td >
            <asp:Panel ID="PanelDettCA" runat="server" Height="200px" BorderStyle="Solid" BorderWidth="1px" Visible="false">
            <div id="div3" style="overflow:auto; width:1230px; height:200px; border-style:groove;">
                <asp:GridView ID="GridViewDettCA" runat="server" 
                    GridLines="None" CssClass="GridViewStyle" AutoGenerateColumns="False" 
                    EmptyDataText="Nessun dato disponibile."  
                    DataKeyNames="IDDocumenti" DataSourceID="">
                    <RowStyle CssClass="RowStyle" />
                    <PagerStyle CssClass="PagerStyle" />
                    <HeaderStyle CssClass="HeaderStyle" />
                    <AlternatingRowStyle CssClass="AltRowStyle" />
                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                    <Columns>
                        <asp:BoundField DataField="DesTipoDett" HeaderText="Tipo Dett." 
                            SortExpression="DesTipoDett"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" /></asp:BoundField> 
                        <asp:BoundField DataField="DesTipoDettR" HeaderText="Riga" 
                            SortExpression="DesTipoDettR"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" /></asp:BoundField> 
                        <asp:BoundField DataField="Cod_Articolo" 
                        HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo">
                        <HeaderStyle Wrap="false" Width="20px"/>
                        <ItemStyle Width="20px" Wrap="false" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" Width="350px"/><ItemStyle 
                        Width="350px" /></asp:BoundField>
                        <asp:BoundField DataField="SerieLotto" 
                        HeaderText="Serie" SortExpression="SerieLotto"><HeaderStyle Wrap="True" Width="100px"/><ItemStyle 
                        Width="100px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" Width="05px" HorizontalAlign="Center"/>
                        <ItemStyle Width="05px" HorizontalAlign="Center"/></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità evasa" 
                        SortExpression="Qta_Evasa"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Inviata" HeaderText="Quantità inviata" 
                        SortExpression="Qta_Inviata"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                        Width="10px" CssClass="nascondi"/></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua"  
                        SortExpression="Qta_Residua"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                        Width="10px" CssClass="nascondi" /></asp:BoundField>
                        <asp:BoundField DataField="Des_Filiale" HeaderText="Luogo App." 
                        SortExpression="Des_Filiale"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="DataSc" HeaderText="Data scadenza" 
                        SortExpression="DataSc"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Cod_IVA" HeaderText="IVA" 
                        SortExpression="Cod_IVA"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Prezzo" HeaderText="Prezzo" 
                        SortExpression="Prezzo"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoValore" HeaderText="Sconto valore" 
                        SortExpression="ScontoValore"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Sconto_1" HeaderText="Sc.(1)" 
                        SortExpression="Sconto_1"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        <asp:BoundField DataField="Importo" HeaderText="Importo" 
                        SortExpression="Importo"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="ScontoReale" HeaderText="Sc.Riga" 
                        SortExpression="ScontoReale"><HeaderStyle Wrap="True" Width="05px"/><ItemStyle 
                        Width="05px" /></asp:BoundField>
                        </Columns>
                </asp:GridView>
            </div>
            </asp:Panel>
        </td>
    </tr>
</table>
</asp:Panel>   
