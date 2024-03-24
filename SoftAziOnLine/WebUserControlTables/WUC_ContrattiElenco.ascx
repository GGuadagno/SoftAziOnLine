<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ContrattiElenco.ascx.vb" Inherits="SoftAziOnLine.WUC_ContrattiElenco" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControl/WFP_CambiaStatoCA.ascx" TagName="WFPCambiaStatoCA" TagPrefix="wuc3" %>
<%@ Register Src="~/WebUserControl/WFP_FatturaCA.ascx" TagName="WFPFatturaCA" TagPrefix="wuc4" %>
<%@ Register Src="~/WebUserControl/WFP_DocCollegati.ascx" TagName="WFPDocCollegati" TagPrefix="wuc5" %>
<style type="text/css">
    .btnstyle1RL
        {
            Width: 140px;
            height: 30px;
        margin-left: 0px;
        }
     .btnstyle1RLL
        {
            Width: 195px;
            height: 30px;
        margin-left: 0px;
        }
    .styleTDBTN
    {
        height: 478px;
    }
    .btnstyle2R
    {
        Width: 108px;
        height: 40px;
    margin-left: 0px;
    white-space: pre-wrap;
    }   
    .btnstyle1R
    {
        Width: 108px;
        height: 30px;
    margin-left: 0px;
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
        height: 240px;
    }
    .style5
    {
        width: auto;
        border-style: none;
        height: 200px;
    }
    .style6
    {
        height: 200px;
    }
    .style7
    {
        height: 185px;
    }
     .styleh30
    {
        height: 30px;
    }
</style> 
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="610px" CssClass="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <wuc3:WFPCambiaStatoCA ID="WFPCambiaStatoCA" runat="server" />
    <wuc4:WFPFatturaCA ID="WFPFatturaCA" runat="server" />
    <wuc5:WFPDocCollegati ID="WFPDocCollegati" runat="server" />
    <asp:SqlDataSource ID="SqlDSCausMag" runat="server" 
        SelectCommand="SELECT * FROM [CausMag] WHERE Descrizione LIKE '%CONTR%' ORDER BY [Descrizione]">
    </asp:SqlDataSource>
     <asp:SqlDataSource ID="SqlDSRespArea" runat="server" 
            SelectCommand="SELECT *, RTRIM(CAST(Codice AS NVARCHAR(10))) + ' - ' + Descrizione AS CodDes FROM [RespArea] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSRespVisite" runat="server" 
            SelectCommand="SELECT Descrizione FROM [RespVisite] WHERE ([CodRespArea] = (CASE WHEN ISNULL(@IDRespArea,0) > 0 THEN @IDRespArea ELSE CodRespArea END)) GROUP BY [Descrizione] ORDER BY [Descrizione]">
            <SelectParameters>
                <asp:SessionParameter Name="IDRespArea" SessionField="IDRespArea" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    <table border="0" cellpadding="0" frame="box" style="width:auto; height:610px; margin-right:0;">
        <tr>
            <td style="height:40px;width:1220px" align="left" colspan="2" >
                    <table style="width:1220px">
                    <tr>
                        <td>
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" 
                                AutoPostBack="true" Width="145px">
                            </asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="200px"></asp:TextBox>&nbsp;<asp:Button ID="btnRicerca" runat="server" Text="Cerca contratto" class="btnstyle1RL" />
                            &nbsp;<asp:Button ID="btnFatturaCA" runat="server" Text="Emissione Fattura" class="btnstyle1RL" Visible="false" />
                            &nbsp;<asp:Button ID="btnFatturaCAAC" runat="server" Text="Fattura per Acconto/Saldo" class="btnstyle1RLL" Visible="false"/>
                            &nbsp;<asp:Button ID="btnDocCollegati" runat="server" Text="Documenti Collegati" class="btnstyle1RL" Visible="true"/>
                        </td>
                    </tr>
                    </table> 
            </td>
        </tr>
        <tr>
            <td style="height:70px;width:1110px" align="left">
                <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="70px" BorderStyle="Groove">
                <div>
                    <asp:Label ID="Label8" runat="server" Width="68px">&nbsp;Seleziona:</asp:Label>
                    <asp:RadioButton ID="rbtnEvaso" runat="server" Text="Evasi" AutoPostBack="True" 
                        GroupName="StatoDoc" />
                    <asp:Label ID="Label0" runat="server" Width="40px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnDaEvadere" runat="server" Text="Da evadere" 
                        AutoPostBack="True" GroupName="StatoDoc" />
                    <asp:Label ID="Label1" runat="server" Width="40px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnParzEvaso" runat="server" Text="Parzialmente evasi" 
                        AutoPostBack="True" GroupName="StatoDoc" />
                    <asp:Label ID="Label12" runat="server" Width="40px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnNonCompleto" runat="server" Text="Non completo" 
                        AutoPostBack="True" GroupName="StatoDoc" ForeColor="DarkRed"/>
                    <asp:Label ID="Label2" runat="server" Width="40px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnChiusoNoEvaso" runat="server" Text="Chiuso non evasi" 
                        AutoPostBack="True" GroupName="StatoDoc" />
                    <asp:Label ID="Label5" runat="server" Width="40px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnNonEvadibile" runat="server" Text="Non evadibile" 
                        AutoPostBack="True" GroupName="StatoDoc" />
                    <asp:Label ID="Label9" runat="server" Width="40px">&nbsp;</asp:Label>
                    <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" AutoPostBack="True" GroupName="StatoDoc" Checked="true" />
                </div>
                <div style="height:15px">&nbsp;
                </div>
                <div>
                   <%-- <asp:Label ID="Label10" runat="server" Width="69px">&nbsp;</asp:Label>--%>
                    <asp:CheckBox ID="checkCausale" runat="server" Font-Bold="false" Text="Tipo Contratto" Checked="false" AutoPostBack="true"/>
                    <asp:DropDownList ID="DDLCausali" runat="server" AppendDataBoundItems="True" 
                        AutoPostBack="True" DataSourceID="SqlDSCausMag" DataTextField="Descrizione" 
                        DataValueField="Codice" Height="22px" TabIndex="7" Width="250px">
                        <asp:ListItem ></asp:ListItem>
                    </asp:DropDownList>   
                    <asp:CheckBox ID="checkRespArea" runat="server" Font-Bold="false" Text="Resp.Area" Checked="false" AutoPostBack="true"/> 
                    <asp:DropDownList ID="DDLRespArea" runat="server" AppendDataBoundItems="True" Enabled="false" 
                        AutoPostBack="True" DataSourceID="SqlDSRespArea" DataTextField="Descrizione" 
                        DataValueField="Codice" Height="22px" Width="250px">
                        <asp:ListItem ></asp:ListItem>
                    </asp:DropDownList>
                     <asp:CheckBox ID="CheckRespVisite" runat="server" Font-Bold="false" Text="Resp.Visite" Checked="false" AutoPostBack="true"/> 
                    <asp:DropDownList ID="DDLRespVisite" runat="server" AppendDataBoundItems="True" Enabled="false" 
                        AutoPostBack="True" DataSourceID="SqlDSRespVisite" DataTextField="Descrizione" 
                        DataValueField="Descrizione" Height="22px" Width="250px">
                        <asp:ListItem ></asp:ListItem>
                    </asp:DropDownList>
                </div>
                </asp:Panel>
            </td>
            <td align="left" class="style2">
                <div>
                    <asp:Button ID="btnSblocca" runat="server" class="btnstyle2R" Text="Sblocca Doc." />
                </div>
                <div>
                    <asp:Button ID="btnCambiaStato" runat="server" class="btnstyle1R" Text="Cambia stato" />
                </div>
            </td>
        </tr>
        <tr>
            <td align="left" class="style3">
            <%--<div id="divGridViewPrevT" style="overflow-x:hidden; overflow-y:hidden; width:860px; height:195px; border-style:groove;">--%> 
            <div id="divGridViewPrevT" style="overflow:auto; width:1110px; height:240px; border-style:groove;">                
                    <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="IDDocumenti" 
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
                        DataSourceID="SqlDSPrevTElenco" BackColor="Silver" AllowSorting="True">
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
                                ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                                ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                                <ControlStyle Font-Size="XX-Small" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                            </asp:CommandField>
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
                    <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                        SelectCommand="get_ConTElenco" 
                        SelectCommandType="StoredProcedure">
                        <SelectParameters>
                            <asp:SessionParameter DefaultValue="CA" Name="TipoDoc" SessionField="TipoDoc" Type="String" />
                            <asp:SessionParameter DefaultValue="0" Name="StatoDoc" SessionField="StatoDoc" Type="Int32" />
                            <asp:SessionParameter DefaultValue="N" Name="SortPrevTEl" SessionField="SortPrevTEl" Type="String" />
                            <%--<asp:SessionParameter DefaultValue="0" Name="Ricerca" SessionField="Ricerca" Type="Int32" />--%>
                        </SelectParameters>
                    </asp:SqlDataSource>
               </div>
               <div align="center" style="height:25px">
                    <asp:Label ID="Label11" runat="server" BorderStyle="None" Font-Bold="True" Text="App/Periodo"></asp:Label>
                    <asp:DropDownList ID="DDLTipoDettagli" runat="server" Width="150px" AutoPostBack="true">
                                            <asp:ListItem Text="Apparecchiature" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Attività per periodo" Value="1"></asp:ListItem>
                                      </asp:DropDownList>
                    <asp:Label ID="lblDurNumRiga" runat="server" BorderStyle="None" Font-Bold="True" Text="N°"></asp:Label>
                    <asp:DropDownList ID="DDLDurNumRIga" runat="server" Width="180px" AutoPostBack="true">
                                      </asp:DropDownList>
                    <asp:CheckBox ID="chkSostNSerie" runat="server" Font-Bold="true" Text="Sostituzione N°Serie in" Checked="false" AutoPostBack="true" ForeColor="Blue"/>                  
                    <asp:TextBox ID="txtSerieNEW" runat="server" Width="120px" MaxLength="16" BorderStyle="None" AutoPostBack="false" Enabled="false"></asp:TextBox>
                    <asp:Label ID="lblAPartireDal" runat="server" ForeColor="Blue">a partire dal</asp:Label>
                    <asp:TextBox ID="txtDataDal" runat="server" Width="70px" style="text-align:left" 
                                            MaxLength="10" AutoPostBack="false" BorderStyle="None" Enabled="false"></asp:TextBox>
                    <asp:ImageButton ID="ImgDataDal" runat="server" CausesValidation="False" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png"
                                        ToolTip="apri il calendario" Enabled="false"/>
                                    <asp:CalendarExtender ID="txtDataDal_CalendarExtender1" runat="server" Enabled="True"
                                        Format="dd/MM/yyyy" PopupButtonID="ImgDataDal"
                                        TargetControlID="txtDataDal">
                                    </asp:CalendarExtender>                        
                    <asp:Button ID="btnSostNSerie" runat="server" Height="25px" ForeColor="Blue" Font-Bold="true" Text="OK Sostituzione N°Serie" Enabled="false"/>
               </div>
            </td>
            <td align="left" class="style7">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div>
                            <asp:Button ID="btnVisualizza" runat="server" class="btnstyle1R" Text="Visualizza" />
                        </div>
                        <div style="height: 5px"></div>
                        <div>
                            <asp:Button ID="btnNuovo" runat="server" class="btnstyle1R" Text="Nuovo" />
                        </div>
                        <div style="height: 5px">&nbsp;</div>
                        <div>
                            <asp:Button ID="btnNuovoDaOC" runat="server" class="btnstyle2R" Text="Nuovo:Carica dati da Ordine" />
                        </div>
                        <div style="height: 5px">&nbsp;</div>
                        <div>
                             <asp:Button ID="btnCopia" runat="server" class="btnstyle2R" Text="Nuovo:Copia dati Contratto"/>
                                </div>
                        <div style="height: 5px">&nbsp;</div>
                        <div>
                            <asp:Button ID="btnModifica" runat="server" class="btnstyle1R" Text="Modifica" />
                        </div>
                        <div style="height: 5px">&nbsp;</div> 
                        </div>
                        <div>
                            <asp:Button ID="btnElimina" runat="server" class="btnstyle1R" Text="Elimina" />
                        </div>
                        <div style="height: 5px">&nbsp;</div> 
                        </div>
                        <div>
                            <asp:Button ID="btnCreaDDT" runat="server" class="btnstyle2R" Text="Crea DDT" Visible="false"/>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="left" class="style5">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
               <%--<div id="divGridViewPrevD" style="overflow-x:hidden; overflow-y:hidden; width:1110px; height:200px; border-style:groove;">--%>  
               <div id="divGridViewPrevD" style="overflow:auto; width:1110px; height:200px; border-style:groove;">              
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                         CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="DNRRiga" 
                        AllowPaging="false"
                        PageSize="5" 
                        PagerStyle-HorizontalAlign="Center" 
                        PagerSettings-Mode="NextPreviousFirstLast"
                        PagerSettings-Visible="true"
                        PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                        PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                        PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                        PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" 
                        EnableTheming="True" GridLines="None"
                        AllowSorting="True"
                        DataSourceID="SqlDSPrevDByIDDocumenti"><AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                                ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                                <ControlStyle Font-Size="XX-Small" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                            </asp:CommandField>
                        <asp:BoundField DataField="DesTipoDett" HeaderText="Tipo Dett." 
                            SortExpression="DesTipoDett"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                            Width="5px" /></asp:BoundField> 
                        <asp:BoundField DataField="DNRRiga" HeaderText="Riga" 
                            SortExpression="DNRRiga">
                            <HeaderStyle Wrap="false" Width="5px"/>
                            <ItemStyle Wrap="false" Width="5px" /></asp:BoundField> 
                        <asp:BoundField DataField="Cod_Articolo" 
                        HeaderText="Codice articolo" ReadOnly="True" 
                        SortExpression="Cod_Articolo">
                        <HeaderStyle Wrap="false" Width="20px"/>
                        <ItemStyle Width="20px" Wrap="false" /></asp:BoundField>
                        <asp:BoundField DataField="Descrizione" 
                        HeaderText="Descrizione" SortExpression="Descrizione"><HeaderStyle Wrap="True" Width="300px"/><ItemStyle 
                        Width="300px" /></asp:BoundField>
                        <asp:BoundField DataField="SerieLotto" 
                        HeaderText="Serie" SortExpression="SerieLotto">
                        <HeaderStyle Wrap="false" Width="100px"/>
                        <ItemStyle Wrap="false" Width="100px" /></asp:BoundField>
                        <asp:BoundField DataField="UM" HeaderText="UM" 
                        SortExpression="UM"><HeaderStyle Wrap="True" Width="05px" HorizontalAlign="Center"/>
                        <ItemStyle Width="05px" HorizontalAlign="Center"/></asp:BoundField>
                        <asp:BoundField DataField="Qta_Ordinata" HeaderText="Quantità ordinata" 
                        SortExpression="Qta_Ordinata"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Evasa" HeaderText="Quantità evasa" 
                        SortExpression="Qta_Evasa"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Residua" HeaderText="Quantità residua"  
                        SortExpression="Qta_Residua"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                        Width="10px" CssClass="nascondi" /></asp:BoundField>
                        <asp:BoundField DataField="Qta_Fatturata" HeaderText="Quantità Fatturata" 
                        SortExpression="Qta_Fatturata"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px"/></asp:BoundField>
                        <asp:BoundField DataField="Des_Filiale" HeaderText="Luogo App." 
                        SortExpression="Des_Filiale"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="DataSc" HeaderText="Data scadenza" 
                        SortExpression="DataSc"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
                        Width="10px" /></asp:BoundField>
                        <asp:BoundField DataField="RefDataNC" HeaderText="Data scadenza consumabile" 
                        SortExpression="RefDataNC"><HeaderStyle Wrap="True" Width="10px"/><ItemStyle 
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
                        SortExpression="ScontoReale">
                        <HeaderStyle Wrap="True" Width="05px" CssClass="nascondi"/><ItemStyle 
                        Width="05px" CssClass="nascondi"/></asp:BoundField>
                        </Columns>
                        <HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                    <asp:SqlDataSource ID="SqlDSPrevDByIDDocumenti" runat="server" 
                        SelectCommand="get_ConDByIDDocElenco" 
                        SelectCommandType="StoredProcedure">
                        <SelectParameters>
                            <asp:SessionParameter DefaultValue="0" Name="IDDocumenti" SessionField="IDDocumenti" Type="Int32" />
                            <asp:SessionParameter DefaultValue="-1" Name="DurataNum" SessionField="DurataNum" Type="Int32" />
                            <asp:SessionParameter DefaultValue="-1" Name="DurataNumRiga" SessionField="DurataNumRiga" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
            </td>
            <td align="left" class="style6">
                    <div>
                         <asp:Button ID="btnNuovaRev" runat="server" class="btnstyle1R" Text="Nuova Rev.N°" Visible="false"/>
                            </div>
                    <div style="height: 5px">&nbsp;</div>
                    <div>
                         <asp:Button ID="btnCreaOF" runat="server" class="btnstyle2R" Text="Crea ordine fornitore" Visible="false"/>
                            </div> 
                    <div style="height: 5px">&nbsp;</div>
                    <div style="height: 20px; text-align:center"><%--<b>Stampe</b>--%>
                        <asp:Label ID="lblStampe" runat="server" BorderColor="White"
                        BorderStyle="None" Font-Bold="True" ForeColor="Black" Visible="true">Stampe</asp:Label>
                    </div> 
                    <div style="height: 5px">&nbsp;</div> 
                    <div>
                        <asp:Button ID="btnStampa" runat="server" class="btnstyle1R" Text="Proforma" Visible="true"/>
                            </div>
                    <div style="height:5px">&nbsp;</div>
                    <div>
                        <a ID="LnkStampa" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Proforma" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Proforma</a>
                    </div>
                     <div style="height:5px">&nbsp;</div>
                    <div>
                        <asp:Button ID="btnVerbale" runat="server" class="btnstyle1R" Text="Verbale" Visible="true"/>
                            </div>
                     <div style="height:5px">&nbsp;</div>
                    <div>
                        <a ID="LnkVerbale" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Verbale" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Verbale</a>
                    </div>
                     <div style="height:5px">&nbsp;</div>
                </td>
           </tr>
</table>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>
<%--<script src="../JScript/jquery-1.4.4.min.js" type="text/javascript"></script>
<script src="../JScript/ScrollableGridPlugin.js" type="text/javascript"></script>
<script type="text/javascript" src="../JScript/gridviewscroll.js"></script>
<script type = "text/javascript">
    function GridScroll() {
        var options2 = new GridViewScrollOptions();
        options2.elementID = "<%=GridViewPrevD.ClientID %>";
        options2.width = 1110;
        options2.height = 200;
        options2.freezeColumn = true;
        options2.freezeFooter = false;
        options2.freezeColumnCssClass = "GridViewScrollItemFreeze";
        options2.freezeFooterCssClass = "GridViewScrollFooterFreeze";
        options2.freezeHeaderRowCount = 1;
        options2.freezeColumnCount = 0;

        gridViewScroll2 = new GridViewScroll(options2);

        gridViewScroll2.enhance();
    }
    $(document).ready(GridScroll);
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(GridScroll);

</script>	--%>