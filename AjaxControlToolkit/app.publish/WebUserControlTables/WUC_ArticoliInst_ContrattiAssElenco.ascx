<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ArticoliInst_ContrattiAssElenco.ascx.vb" Inherits="SoftAziOnLine.WUC_ArticoliInst_ContrattiAssElenco" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WUC_StampaElencoAI.ascx" tagname="WUC_StampaElencoAI" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_ArticoliInstEmail.ascx" tagname="WFP_ArticoliInstEmail" tagprefix="uc5" %>
<%@ Register src="../WebUserControl/WUC_Anagrafiche_ModifySint.ascx" tagname="WUC_Anagrafiche_ModifySint" tagprefix="uc3" %>
<%@ Register src="../WebUserControl/WUC_Attesa.ascx" tagname="Attesa" tagprefix="uc4" %>
<%@ Register Src="~/WebUserControl/WFP_DocCollegati.ascx" TagName="WFPDocCollegati" TagPrefix="wuc6" %>
<%@ Register Src="../WebUserControl/WFP_ElencoEmail.ascx" TagName="WFP_ElencoEmail" TagPrefix="wuc7" %>
<style type="text/css">
    .styleTDBTN
    {
        height: 478px;
    }
    .btnstyle1R
        {
            Width: 140px;
            height: 30px;
        margin-left: 0px;
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
    </style>    
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="550px" CssClass ="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc4:Attesa ID="Attesa" runat="server" />
    <uc2:WUC_StampaElencoAI ID="WUC_StampaElencoAI1" runat="server" />
    <uc5:WFP_ArticoliInstEmail ID="WFP_ArticoliInstEmail" runat="server" />
    <uc3:WUC_Anagrafiche_ModifySint ID="WUC_Anagrafiche_ModifySint1" runat="server" />
    <wuc6:WFPDocCollegati ID="WFPDocCollegati" runat="server" />
    <wuc7:WFP_ElencoEmail ID="WFP_ElencoEmail" runat="server" />
    <asp:SqlDataSource ID="SqlDSCliForFilProvv" runat="server" 
        SelectCommand="SELECT * FROM [CliFor] WHERE ([Codice_CoGe] = ''">
    </asp:SqlDataSource>
    <table border="0" cellpadding="0" frame="box" style="width:1240px; height:550px; margin-right:0;">
            <tr>
                <td align="left" class="style1" colspan="2">
                    <table>
                    <tr>
                        <td align="left" class="style1">
                            <asp:Label ID="Label1" runat="server" Text="Ricerca per:" Font-Bold="false"/>
                            <asp:DropDownList ID="ddlRicerca" runat="server" AutoPostBack="True" Width="145px"></asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false" AutoPostBack="True" />
                            <asp:TextBox ID="txtRicerca" runat="server" Width="200px"></asp:TextBox>
                            <asp:Button ID="btnRicerca" runat="server" Text="Avvia ricerca" class="btnstyle1R" />
                            &nbsp;<asp:Button ID="btnDocCollegati" runat="server" Text="Documenti Collegati" class="btnstyle1R" />
                        </td>
                        <td align="center" style="height:35px">
                            <asp:Label ID="lblInfoRicerca" runat="server" Text="" Width="99%" style="font-size: 10pt"></asp:Label>
                        </td>
                    </tr>
                    </table> 
                </td>
            </tr>
            <tr>
                <td align="left" class="styleBordo">
                    <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="25px" Width="1110px">
                    <table>
                        <tr>
                            <td style="width:350px" >
                                <div>
                                    <asp:RadioButton ID="rbtnAttivo" runat="server" ToolTip="Articolo attivo"
                                    Text="Attivo" AutoPostBack="True" GroupName="Tipo"/>
                                    <asp:RadioButton ID="rbtnDismesso" runat="server" ToolTip="Articolo dismesso"
                                    Text="Dismesso" AutoPostBack="True" GroupName="Tipo"/>
                                    <asp:RadioButton ID="rbtnSostituito" runat="server" ToolTip="Articolo sostituito" 
                                    Text="Sostituito" AutoPostBack="True" GroupName="Tipo" />
                                    <asp:RadioButton ID="rbtnInRiparazione" runat="server" ToolTip="Articolo in riparazione"
                                    Text="In riparazione" AutoPostBack="false" GroupName="Tipo" CssClass="nascondi" />
                                </div>
                            </td> 
                            <td style="width:150px">
                                <div>
                                    <asp:RadioButton ID="rbtnInviataEmail" runat="server" ToolTip="Inviata E-mail"
                                    Text="Inviata E-mail" AutoPostBack="True" GroupName="Tipo" />
                                </div>
                            </td>
                            <td style="width:300px">
                                    <asp:RadioButton ID="rbtnConScadenze" runat="server" ToolTip="Scadenze Elettrodi Batterie" 
                                    Text="Scadenze Elettrodi Batterie" AutoPostBack="True" GroupName="Tipo" />
                            </td>
                            <td style="width:150px">
                                <div>
                                    <asp:RadioButton ID="rbtnTutti" runat="server" ToolTip="Tutti (Prepara E-mail Scadenze)"
                                    Text="Tutti" AutoPostBack="True" GroupName="Tipo"/>  
                                </div>
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                </td>
                <td align="left" class="style2">
                    <div>
                        <asp:Button ID="btnSelClientiSc" runat="server" class="btnstyle" Text="Seleziona Scadenze" Visible="true" />
                    </div>
                </td>
            </tr>
            <tr>
                <td align="center" style="height:40px">
                    <div>
                    <asp:Label ID="lblDescrFiltri" runat="server" Text="" Font-Bold="true" Width="99%" Height="99%" style="font-size: 10pt" BorderStyle="Inset"  ToolTip="Filtro selezione Articoli consumabili Clienti"></asp:Label>
                    &nbsp;<asp:Button ID="btnCancFiltro" runat="server" Text="Cancella filtro" Width="120px" Height="25px"/>
                    </div>
                </td>
                <td align="center" style="height:40px">
                    <asp:Label ID="lblLogInvioEmail" runat="server" Text="" Width="99%" Height="99%" style="font-size: 10pt" BorderStyle="Inset" ToolTip="INFO invio E-mail" ></asp:Label></td>
            </tr>
            <tr>
                <td align="left" class="style3">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                    <div id="divGridViewPrevT" style="overflow:auto; width:1110px; height:380px; border-style:groove;">
                        <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="ID" 
                        AllowPaging="true"
                        PageSize="9" 
                        PagerStyle-HorizontalAlign="Center" 
                        PagerSettings-Mode="NextPreviousFirstLast"
                        PagerSettings-Visible="true"
                        PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                        PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                        PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                        PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                        EnableTheming="True" GridLines="Horizontal" BackColor="Silver"  AllowSorting="true" PagerSettings-Position="Bottom">
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
                            <asp:TemplateField InsertVisible="False">
                                <ItemTemplate>
                                    <asp:Button ID="btnSelAI" runat="server" CausesValidation="False" CommandName="Select" Text="&gt;" Width="5px"/>
                                </ItemTemplate>
                            <controlstyle font-size="XX-Small" />
                            <HeaderStyle Width="5px"/><ItemStyle Width="5px" HorizontalAlign="Center" VerticalAlign="Middle"/>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Cod_Coge" HeaderText="Codice CoGe" 
                                SortExpression="Cod_Coge" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="15px" /><ItemStyle Width="15px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Rag_Soc" HeaderText="Ragione Sociale" 
                                SortExpression="Rag_Soc" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="150px" /><ItemStyle Width="150px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Denominazione" 
                                HeaderText="Denominazione" ReadOnly="True" 
                                SortExpression="Denominazione" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="100px"/><ItemStyle Width="100px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" 
                                DataField="Cod_Articolo" HeaderText="Codice articolo" ReadOnly="True" 
                                SortExpression="Cod_Articolo" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                                <HeaderStyle Width="30px"/><ItemStyle Width="30px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Descrizione" 
                                HeaderText="Descrizione" SortExpression="Descrizione" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="150px"/><ItemStyle Width="150px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Data_Installazione" HeaderText="Data" 
                                SortExpression="Data_Installazione">
                                <HeaderStyle Width="10px"/><ItemStyle Width="10px" />
                            </asp:BoundField>  
                            <asp:BoundField DataField="NSerie" 
                                HeaderText="N° Serie / Lotto" SortExpression="NSerie" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="30px" /><ItemStyle Width="30px" />
                            </asp:BoundField>                                
                            <asp:BoundField DataField="DataScadElettrodi" 
                                HeaderText="Data scadenza elettrodi" SortExpression="DataScadElettrodi"
                                HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="10px"/><ItemStyle Width="10px" />
                            </asp:BoundField>  
                            <asp:BoundField DataField="DataScadBatterie" 
                                HeaderText="Data scadenza batterie" SortExpression="DataScadBatterie"
                                HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="10px"/><ItemStyle Width="10px" />
                            </asp:BoundField> 
                            <asp:TemplateField HeaderText="Attivo" ItemStyle-HorizontalAlign="Center" SortExpression="Attivo">
                                <ItemTemplate>
                                    <asp:CheckBox id="chkAttivo" runat="server" Enabled="true" AutoPostBack="True" Checked='<%# Convert.ToBoolean(Eval("Attivo")) %>' 
                                        OnCheckedChanged="chkAttivo_CheckedChanged"></asp:CheckBox> 
                                </ItemTemplate>
                                <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                            </asp:TemplateField>                                                                 
                            <asp:TemplateField HeaderText="Sost." ItemStyle-HorizontalAlign="Center" SortExpression="Sostituito">
                                <ItemTemplate>
                                    <asp:CheckBox id="chkSostituito" runat="server" Enabled="true" AutoPostBack="True" Checked='<%# Convert.ToBoolean(Eval("Sostituito")) %>' 
                                        OnCheckedChanged="chkSostituito_CheckedChanged"></asp:CheckBox>
                                </ItemTemplate>
                                <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="NReInvio" HeaderText="N° Invio E-mail" 
                                SortExpression="NReInvio" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="10px"/><ItemStyle Width="10px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DataInvio" HeaderText="Data Invio E-Mail" 
                                SortExpression="DataInvio" HeaderStyle-Wrap="true" ItemStyle-Wrap="false">
                                <HeaderStyle Width="10px"/><ItemStyle Width="10px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="DataScadGaranzia" HeaderText="Data scadenza garanzia" 
                                SortExpression="DataScadGaranzia" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="10px"/><ItemStyle Width="10px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                                HeaderText="Località" ReadOnly="True" 
                                SortExpression="Localita" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="15px"/><ItemStyle Width="15px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                                HeaderText="CAP" ReadOnly="True" 
                                SortExpression="CAP">
                                <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                            </asp:BoundField>                  
                            <asp:BoundField DataField="RiferimentiRic" HeaderText="Riferimenti documento" 
                                SortExpression="RiferimentiRic" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="150px"/><ItemStyle Width="150px" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Cod_Filiale" HeaderText="Cod_Filiale" ItemStyle-Width="1px" ItemStyle-CssClass="nascondi" >
                                <HeaderStyle CssClass="nascondi" Width="10px"/><ItemStyle Width="10px" />
                            </asp:BoundField>  
                             <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione1" 
                                HeaderText="Destinazione(1)" ReadOnly="True" 
                                SortExpression="Destinazione1" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="25px"/><ItemStyle Width="25px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione2" 
                                HeaderText="Destinazione(2)" ReadOnly="True" 
                                SortExpression="Destinazione2" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="25px"/><ItemStyle Width="25px" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione3" 
                                HeaderText="Destinazione(3)" ReadOnly="True" 
                                SortExpression="Destinazione3" HeaderStyle-Wrap="true" ItemStyle-Wrap="true">
                                <HeaderStyle Width="25px"/><ItemStyle Width="25px" />
                            </asp:BoundField>                        
                        </Columns>
                        </asp:GridView>
                        <div style="height: 15px">
                        </div>
                        <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                            SelectCommand="get_ArticoliInstallati" 
                            SelectCommandType="StoredProcedure">
                            <SelectParameters>
                                <asp:SessionParameter DefaultValue="ZZ" Name="TipoDoc" SessionField="TipoDoc" Type="String" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                    </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                </td>
                <td align="left">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <div>
                                <asp:Button ID="btnPreparaEmail" runat="server" class="btnstyle" Text="Prepara E-Mail" Visible="true" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnElencoEmail" runat="server" class="btnstyle" Text="Elenco E-Mail" Visible="true" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnRicarcaAll" runat="server" class="btnstyle" Text="Ricarica tutti i dati" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnAggiornaScad" runat="server" class="btnstyle" Text="Aggiorna date scadenza" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnNuovo" runat="server" class="btnstyle" Text="Nuovo Articolo/Contratto" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnVisualizza" runat="server" class="btnstyle" Text="Visualizza" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnModifica" runat="server" class="btnstyle" Text="Modifica" />
                            </div>
                            <%--<div style="height: 10px"></div>--%>
                            <div>
                                <asp:Button ID="btnElimina" runat="server" class="btnstyle" Text="Elimina"  Visible="false" />
                            </div>
                            <%--<div style="height: 10px"></div>--%>
                            <div>
                                <asp:Button ID="BtnNuovoCopia" runat="server" class="btnstyle" Text="Nuovo (Copia dati)"  Visible="false" />
                            </div>
                            <%--<div style="height: 10px"></div>--%>
                            <div>
                                <asp:Button ID="btnStampaSingolo" runat="server" class="btnstyle" Text="Singola" Visible="false" />
                            </div>
                            <%--<div style="height: 10px"></div>--%>
                            <div>
                                <asp:Button ID="btnStampaElenco" runat="server" class="btnstyle" Text="Stampa Elenco" Visible="false" />
                            </div>
                            <div style="height: 10px"></div>
                            <div>
                                <asp:Button ID="btnModificaAnagrafica" runat="server" class="btnstyle" Text="Modifica E-Mail" Visible="true" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
</table>
    </ContentTemplate>
 </asp:UpdatePanel>   
 </asp:Panel>