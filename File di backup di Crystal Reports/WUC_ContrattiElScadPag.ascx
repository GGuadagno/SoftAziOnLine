<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ContrattiElScadPag.ascx.vb" Inherits="SoftAziOnLine.WUC_ContrattiElScadPag" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
<%@ Register Src="~/WebUserControl/WFP_DocCollegati.ascx" TagName="WFPDocCollegati" TagPrefix="wuc5" %>
<%@ Register src="../WebUserControl/WUC_Attesa.ascx" tagname="Attesa" tagprefix="uc3" %>
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
     .btnstyle4R
    {
        Width: 108px;
        height: 60px;
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
    .styleGridT
    {
        width: auto;
        border-style: none;
        height: 250px;
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
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="1240px" Height="650px" CssClass="sfondopagine">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaClienti" Titolo="Elenco Clienti" />
    <uc3:Attesa ID="Attesa" runat="server" />
    <wuc5:WFPDocCollegati ID="WFPDocCollegati" runat="server" />
     <asp:SqlDataSource ID="SqlDSRegioni" runat="server" 
        SelectCommand="SELECT * FROM [Regioni] ORDER BY [Descrizione]">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDSProvince" runat="server" 
        SelectCommand="SELECT Codice, Descrizione FROM [Province] WHERE [Regione] = CASE WHEN @CodRegione = 0 THEN Regione ELSE @CodRegione END ORDER BY [Descrizione]">
        <SelectParameters>
            <asp:SessionParameter Name="CodRegione" DbType="Int32" Direction="Input" SessionField="CodRegione" />
        </SelectParameters>
    </asp:SqlDataSource>
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
    <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
           SelectCommand="SELECT Codice, Descrizione FROM Magazzini WHERE Codice<>0 ORDER BY Descrizione">
    </asp:SqlDataSource>
    <table border="0" cellpadding="0" frame="box" style="width:auto; height:650px; margin-right:0;">
        <tr>
            <td  width ="100%" align="left" colspan="2" >
                    <table style="width:1230px">
                    <tr>
                        <td>
                            <asp:Label ID="Label15" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="true" ForeColor="Black" Text="Fatturazione: Primo N° di Fattura"></asp:Label>
                            <asp:TextBox ID="txtPrimoNFattura" runat="server" MaxLength="10" TabIndex="3" Width="80px" Enabled="false" Font-Bold="true"></asp:TextBox>
                            &nbsp;&nbsp;<asp:Label ID="Label14" runat="server" BorderColor="White" BorderStyle="Outset" Font-Bold="true" ForeColor="Black" Text="Primo N° di Fattura/PA"></asp:Label>
                            <asp:TextBox ID="txtPrimoNFatturaPA" runat="server" MaxLength="10" TabIndex="3" Width="80px" Enabled="false" Font-Bold="true"></asp:TextBox>
                            <asp:Label ID="Label17" runat="server" Width="20px"></asp:Label>
                            <asp:Label ID="Label16" runat="server" BorderColor="White" BorderStyle="None" Font-Bold="false" ForeColor="Black" Text="Data Fatturazione" ></asp:Label>
                            <asp:TextBox ID="txtDataFattura" runat="server" MaxLength="10" TabIndex="3" Width="70px" BorderStyle="None" AutoPostBack="false"></asp:TextBox>
                            <asp:ImageButton ID="ImageButtonDF" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" TabIndex="4" ToolTip="apri il calendario" />
                            <asp:CalendarExtender ID="CalendarExtender2" runat="server" 
                            Enabled="True" Format="dd/MM/yyyy" PopupButtonID="ImageButtonDF" 
                            TargetControlID="txtDataFattura"></asp:CalendarExtender>
                            <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator3" 
                            runat="server" ControlToValidate="txtDataFattura" ErrorMessage="*" Font-Bold="true" BorderStyle="Outset"
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                            <asp:Label ID="Label18" runat="server" Width="10px"></asp:Label>
                            <asp:Label ID="lblMagazzino" runat="server" Height="16px" Font-Bold="true" ForeColor="Blue" ToolTip="Nessun magazzino scelto assegna quello del Contratto">Magazzino scarico giacenze</asp:Label>
                            <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true" ToolTip="Nessun magazzino scelto assegna quello del Contratto" 
                                   AutoPostBack="false" DataSourceID="SqlDataMagazzino" 
                                   DataTextField="Descrizione" 
                                   DataValueField="Codice" Width="160px">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                            </asp:DropDownList>  
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;Ricerca per:&nbsp;<asp:DropDownList ID="ddlRicerca" runat="server" AutoPostBack="true" Width="150px"></asp:DropDownList>
                            &nbsp;<asp:CheckBox ID="checkParoleContenute" runat="server" Text="Parole contenute" Checked="false"/>
                            &nbsp;<asp:TextBox ID="txtRicerca" runat="server" Width="200px" AutoPostBack="false"></asp:TextBox>
                            &nbsp;<asp:Button ID="btnRicerca" runat="server" Text="Visualizza Scadenze" class="btnstyle1RL" TabIndex="1"/>
                            <asp:CheckBox ID="chkFattRiep" runat="server" Text="Fattura Riepilogativa" ForeColor="Blue" Font-Bold="true" Checked="false" AutoPostBack="true" Visible="true"/>
                            <asp:CheckBox ID="chkCTRFatturato" runat="server" Text="Fatturato?" Font-Bold="false" Checked="false" AutoPostBack="true" Visible="true" Enabled="false"/>
                            <asp:DropDownList ID="DDLCTRFatturato" runat="server" AutoPostBack="false" Width="220px" Enabled="false"></asp:DropDownList>
                        </td>
                    </tr>
                    </table> 
            </td>
        </tr>
        <tr>
           <td  width ="1110px" align="left">
                <asp:Panel ID="PanelSelezionaPrev" runat="server" Height="80px" BorderStyle="Groove">
               <div>
                    <asp:Label ID="Label9" runat="server" Font-Bold="true" BorderStyle="Outset">Elenco Attività da fatturare nel Periodo: </asp:Label>
                     <asp:Label ID="Label11" runat="server" Width="5px"></asp:Label>
                    <asp:Label ID="Label12" runat="server" text="Cliente"></asp:Label>
                     <asp:Button ID="btnCercaAnagrafica1" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="3" Visible="true" Text="?"/>
                     <asp:Label ID="lblDesCliente" runat="server" Font-Bold="true" BorderStyle="Outset" Width="520px" ></asp:Label>
                      <asp:Button ID="btnNoCliente" runat="server" class="btnstyle" Width="25px" Height="25px" TabIndex="3" Visible="true" Text="X"/>
                      <asp:CheckBox ID="chkSoloEvase" Text="Solo Attività Evase" runat="server" AutoPostBack="True" Font-Bold="true" ForeColor="Blue" Checked="true" />
               </div>
                <div>
                    <asp:Label ID="Label2" runat="server" Width="10px"></asp:Label>
                        <asp:Label ID="Label10" runat="server">dal</asp:Label>
                        <asp:TextBox ID="txtDallaData" runat="server" AutoPostBack="false" MaxLength="10" TabIndex="5" Width="70px" BorderStyle="None"></asp:TextBox>
                            <asp:ImageButton ID="imgBtnShowCalendarDD" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario"/>
                            <asp:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDD" TargetControlID="txtDallaData">
                            </asp:CalendarExtender>
                            <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtDallaData" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                         <asp:Label ID="Label8" runat="server">al</asp:Label>
                        <asp:TextBox ID="txtAllaData" runat="server" AutoPostBack="false" MaxLength="10" TabIndex="5" Width="70px" BorderStyle="None"></asp:TextBox>
                            <asp:ImageButton ID="imgBtnShowCalendarAD" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario"/>
                            <asp:CalendarExtender ID="txtAllaData_CalendarExtender" runat="server" Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarAD" TargetControlID="txtAllaData">
                            </asp:CalendarExtender>
                            <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtAllaData" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                        <asp:Label ID="Label4" runat="server" Width="140px"></asp:Label>                                
                        <asp:CheckBox ID="chkTutteRegioni" Text="(Tutte) Regione" runat="server" AutoPostBack="True" />
                        <asp:DropDownList ID="ddlRegioni" runat="server" AutoPostBack="true" DataSourceID="SqlDSRegioni" DataTextField="Descrizione" DataValueField="Codice" Width="213px" Height="22px" 
                            AppendDataBoundItems="true" Enabled="False">
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="Label5" runat="server" Width="1px"></asp:Label>
                         <asp:CheckBox ID="chkTutteProvince" Text="(Tutte) Provincia" runat="server" AutoPostBack="True" />
                        <asp:DropDownList ID="ddlProvince" runat="server" AutoPostBack="true" DataSourceID="SqlDSProvince" DataTextField="Descrizione" DataValueField="Codice" Width="210px" Height="22px" 
                            AppendDataBoundItems="true" Enabled="False">
                            <asp:ListItem Value="0" Text=""></asp:ListItem>
                        </asp:DropDownList>
                </div>
                <div>
                    <asp:Label ID="Label1" runat="server" Width="7px"></asp:Label>
                    <asp:CheckBox ID="checkCausale" runat="server" Font-Bold="false" Text="Tipo Contratto" Checked="false" AutoPostBack="true"/>
                    <asp:DropDownList ID="DDLCausali" runat="server" AppendDataBoundItems="True" 
                        AutoPostBack="True" DataSourceID="SqlDSCausMag" DataTextField="Descrizione" 
                        DataValueField="Codice" Height="22px" TabIndex="7" Width="250px">
                        <asp:ListItem ></asp:ListItem>
                    </asp:DropDownList>   
                     <asp:Label ID="Label3" runat="server" Width="5px"></asp:Label>
                    <asp:CheckBox ID="checkRespArea" runat="server" Font-Bold="false" Text="Resp.Area" AutoPostBack="true" /> 
                    <asp:DropDownList ID="DDLRespArea" runat="server" AppendDataBoundItems="True"  Enabled="false" 
                        AutoPostBack="True" DataSourceID="SqlDSRespArea" DataTextField="Descrizione" 
                        DataValueField="Codice" Height="22px" Width="248px">
                        <asp:ListItem ></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="Label6" runat="server" Width="1px"></asp:Label>
                     <asp:CheckBox ID="CheckRespVisite" runat="server" Font-Bold="false" Text="Resp.Visite" AutoPostBack="true"/> 
                    <asp:DropDownList ID="DDLRespVisite" runat="server" AppendDataBoundItems="True" Enabled="false" 
                        AutoPostBack="True" DataSourceID="SqlDSRespVisite" DataTextField="Descrizione" 
                        DataValueField="Descrizione" Height="22px" Width="245px">
                        <asp:ListItem ></asp:ListItem>
                    </asp:DropDownList>
                </div>
                </asp:Panel>
            </td>
            <td align="left" class="style2">
                <div>
                     <asp:Button ID="btnVisualizza" runat="server" class="btnstyle2R" Text="Visualizza Modifica" />
                </div>
                <div style="height: 5px">&nbsp;</div>
                <div>
                    <asp:Button ID="btnDocCollegati" runat="server" Text="Documenti Collegati" class="btnstyle2R" Visible="true"/>
                </div>
            </td>
        </tr>
        <tr>
            <td align="left" class="style5">
            <%--<div id="divGridViewPrevT" style="overflow-x:hidden; overflow-y:hidden; width:1110px; height:420px; border-style:groove;">--%> 
            <div id="divGridViewPrevT" style="overflow:auto; width:1110px; height:250px; border-style:groove;">                
                    <asp:GridView ID="GridViewPrevT" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato disponibile."  
                        DataKeyNames="IDDocumenti" 
                        AllowPaging="true"
                        PageSize="7" 
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
                            <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                                ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                                <ControlStyle Font-Size="XX-Small" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                            </asp:CommandField>
                             <asp:BoundField DataField="DesRespArea" 
                                HeaderText="RespArea" SortExpression="DesRespArea"><HeaderStyle Wrap="True" Width="100px" CssClass="nascondi" /><ItemStyle 
                                Width="100px" CssClass="nascondi" /></asp:BoundField>
                              <asp:BoundField DataField="DesRespVisite" 
                                HeaderText="RespVisite" SortExpression="DesRespVisite"><HeaderStyle Wrap="True" Width="100px" CssClass="nascondi"/><ItemStyle 
                                Width="100px" CssClass="nascondi" /></asp:BoundField>
                              <asp:BoundField DataField="TotaleF1" 
                                HeaderText="Importo Selezionato" ReadOnly="True" 
                                SortExpression="TotaleF1">
                                <HeaderStyle Wrap="true" Width="20px"/>
                                <ItemStyle Width="20px" Wrap="false" /></asp:BoundField>
                              <asp:TemplateField HeaderText="Sel." ItemStyle-HorizontalAlign="Center" SortExpression="Sel">
                                    <ItemTemplate>
                                        <asp:CheckBox id="chkSel" runat="server" Enabled="true" AutoPostBack="true" Checked='<%# Convert.ToBoolean(Eval("Sel")) %>'
                                            OnCheckedChanged="chkSel_CheckedChanged"></asp:CheckBox>
                                    </ItemTemplate>
                                    <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                                </asp:TemplateField>   
                             <asp:BoundField DataField="DataSc1" HeaderText="Data Scadenza" 
                                SortExpression="DataSc1"><HeaderStyle Wrap="True" Width="10px"/>
                                <ItemStyle Width="10px" />
                             </asp:BoundField>
                             <asp:BoundField DataField="Rata1" 
                                HeaderText="Importo Rata" ReadOnly="True" 
                                SortExpression="Rata1">
                                <HeaderStyle Wrap="false" Width="20px"/>
                                <ItemStyle Width="20px" Wrap="false" /></asp:BoundField>
                              <asp:BoundField DataField="DesRata" HeaderText="N° Rata" 
                                SortExpression="DesRata"><HeaderStyle Wrap="false" Width="20px"/><ItemStyle 
                                Width="20px" Wrap="false"/></asp:BoundField>
                            <asp:BoundField DataField="CKAttEvAllaData" HeaderText="Attività Evase?" 
                                SortExpression="CKAttEvAllaData"><HeaderStyle Wrap="True" Width="20px"/><ItemStyle 
                                Width="20px" Wrap="false"/></asp:BoundField>
                            <asp:BoundField DataField="Serie" HeaderText="Serie" 
                                SortExpression="Serie"><HeaderStyle Wrap="True" Width="20px"/>
                                <ItemStyle Width="20px" Wrap="false"/>
                             </asp:BoundField>
                            <asp:BoundField DataField="SiglaCA" 
                                HeaderText="Tipo"  
                                SortExpression="SiglaCA">
                                <HeaderStyle Wrap="false" Width="5px"/>
                                <ItemStyle Width="1px" Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Numero" HeaderText="Numero" 
                                SortExpression="Numero">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="10px" />
                            </asp:BoundField>
                           
                            <asp:BoundField DataField="Data_Doc" HeaderText="Data Documento" 
                                SortExpression="Data_Doc">
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
                                <ItemStyle Width="50px" Wrap="false" />
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Denominazione" 
                                HeaderText="Denominazione" ReadOnly="True" 
                                SortExpression="Denominazione">
                                <HeaderStyle Wrap="True" />
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Localita" 
                                HeaderText="Località"  
                                SortExpression="Localita">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="15px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Provincia" 
                                HeaderText="Pr."  
                                SortExpression="Provincia">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="5px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="CAP" 
                                HeaderText="CAP"  
                                SortExpression="CAP">
                                <HeaderStyle Wrap="false"/>
                                <ItemStyle Width="5px" Wrap="false"/>
                            </asp:BoundField>
                            
                            <asp:BoundField DataField="Riferimento" HeaderText="Riferimento" 
                                SortExpression="Riferimento">                                    
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="50px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField DataField="Destinazione1" 
                                HeaderText="Destinazione(1)"  
                                SortExpression="Destinazione1">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione2" 
                                HeaderText="Destinazione(2)"  
                                SortExpression="Destinazione2">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" Wrap="false"/>
                            </asp:BoundField>
                            <asp:BoundField ApplyFormatInEditMode="True" DataField="Destinazione3" 
                                HeaderText="Destinazione(3)"  
                                SortExpression="Destinazione3">
                                <HeaderStyle Wrap="True"/>
                                <ItemStyle Width="25px" Wrap="false"/>
                            </asp:BoundField>
                             <asp:BoundField DataField="DesStatoDoc" HeaderText="Stato" 
                                SortExpression="DesStatoDoc"><HeaderStyle Wrap="true" Width="5px"/><ItemStyle 
                                Width="5px" Wrap="false"/></asp:BoundField>
                        </Columns>
                    </asp:GridView>
                    <asp:SqlDataSource ID="SqlDSPrevTElenco" runat="server" 
                        SelectCommand="get_ElencoScadPag" 
                        SelectCommandType="StoredProcedure">
                        <SelectParameters>
                            <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                            <asp:Parameter Name="Cod_Cliente" Type="String" />
                            <asp:Parameter Name="DallaData" Type="String" />
                            <asp:Parameter Name="AllaData" Type="String" />
                            <asp:Parameter Name="DaIncTutti" Type="Int32" />
                            <asp:Parameter Name="RespArea" Type="Int32" />
                            <asp:Parameter Name="RespVisite" Type="Int32" />
                            <asp:Parameter Name="Causale" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
               </div>
            </td>
            <td align="left" class="style2">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div style="height: 5px">&nbsp;</div> 
                        <div style="height:20px; text-align:center"><%--<b>Stampe</b>--%>
                            <asp:Label ID="Label13" runat="server" BorderColor="White"
                            BorderStyle="None" Font-Bold="True" ForeColor="Black" Visible="true">Fatturazione</asp:Label>
                        </div> 
                        <div>
                            <asp:Button ID="btnSelSc" runat="server" class="btnstyle2R" Text="Seleziona tutte le Scadenze" />
                        </div>
                        <%--<div style="height: 5px">&nbsp;</div>--%>
                        <div>
                            <asp:Button ID="btnDeselSc" runat="server" class="btnstyle2R" Text="Deseleziona Tutte" />
                        </div>
                        <%--<div style="height: 5px">&nbsp;</div>--%>
                        <div>
                            <asp:Button ID="btnEmFattSc" runat="server" class="btnstyle4R" Text="Emetti fatture Scadenze selezionate" />
                        </div>
                        <%--<div style="height: 5px">&nbsp;</div>--%>
                        <div>
                            <asp:Button ID="btnStFattScEm" runat="server" class="btnstyle2R" Text="Stampa Fatture emesse" />
                        </div>
                        <%--<div style="height:05px">&nbsp;</div>--%>
                        <div>
                            <asp:CheckBox ID="chkVisALLSc" runat="server" Font-Bold="false" Text="Visualizza tutte le Attività" AutoPostBack="true"  Visible="false"/>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
		<td align="left" class="style5">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                        <ContentTemplate>
               <%--<div id="divGridViewPrevD" style="overflow-x:hidden; overflow-y:hidden; width:1110px; height:110px; border-style:groove;">--%>  
               <div id="divGridViewPrevD" style="overflow:auto; width:1110px; height:220px; border-style:groove;">              
                    <asp:GridView ID="GridViewPrevD" runat="server" AutoGenerateColumns="False" 
                        CssClass="GridViewStyle" 
                        EmptyDataText="Nessun dato selezionato. CAUSE POSSIBILI: ATTIVITA' EVASE NESSUNA. (Solo Attività Evase SELEZIONATO) - (Attività già FATTURATE MA SCADENZA ANCORA APERTA)"  
                        DataKeyNames="IDDocumenti" 
                        EnableTheming="True" GridLines="None" 
                        AllowSorting="false">
                        <AlternatingRowStyle CssClass="AltRowStyle" />
                        <Columns>
                        <asp:TemplateField HeaderText="Sel." ItemStyle-HorizontalAlign="Center" SortExpression="Sel">
                            <ItemTemplate>
                                <asp:CheckBox id="chkSelD" runat="server" Enabled="true" AutoPostBack="true" Checked='<%# Convert.ToBoolean(Eval("Sel")) %>'
                                    OnCheckedChanged="chkSelD_CheckedChanged"></asp:CheckBox>
                            </ItemTemplate>
                            <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                        </asp:TemplateField> 
                        <asp:BoundField DataField="DesTipoDett" HeaderText="Tipo Dett." SortExpression="DesTipoDett">
                            <HeaderStyle Wrap="true" Width="5px" CssClass="nascondi"/>
                            <ItemStyle Width="5px" CssClass="nascondi"/>
                        </asp:BoundField> 
                        <asp:BoundField DataField="DesTipoDettR" HeaderText="Riga" SortExpression="DesTipoDettR">
                            <HeaderStyle Wrap="true" Width="5px" CssClass="nascondi"/>
                            <ItemStyle Width="5px" CssClass="nascondi"/>
                        </asp:BoundField> 
                        <asp:BoundField DataField="SelInRata" HeaderText="Sel. in N°Rata" SortExpression="SelInRata">
                            <HeaderStyle Wrap="true" Width="5px"/>
                            <ItemStyle Wrap="false" Width="20px"/>
                        </asp:BoundField> 
                        <asp:BoundField DataField="Cod_Articolo" HeaderText="Codice articolo" ReadOnly="True" SortExpression="Cod_Articolo">
                            <HeaderStyle Wrap="false" Width="20px"/>
                            <ItemStyle Width="20px" Wrap="false" />
                        </asp:BoundField>
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
                        <asp:BoundField DataField="Qta_Fatturata" HeaderText="Quantità fatturata" 
                        SortExpression="Qta_Fatturata"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                        Width="10px" CssClass="nascondi"/></asp:BoundField>
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
                        <HeaderStyle CssClass="HeaderStyle" />
                        <PagerSettings Mode="NextPrevious" Visible="False" />
                        <PagerStyle CssClass="PagerStyle" />
                        <RowStyle CssClass="RowStyle" />
                        <SelectedRowStyle CssClass="SelectedRowStyle" />
                        </asp:GridView>
                    <asp:SqlDataSource ID="SqlDSPrevDByIDDocumenti" runat="server" 
                        SelectCommand="SELECT * FROM ContrattiD WHERE IDDocumenti=-1" 
                        SelectCommandType="Text">
                    </asp:SqlDataSource>
            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
        </td>
        <td align="left" class="style2">
             <%--<div style="height:5px">&nbsp;</div>--%>
             <div style="height:50px; text-align:center">
                <asp:Label ID="lblStampe" runat="server" BorderStyle="Outset" ForeColor="Blue" Width="100px" text="Stampe">
                    <a ID="LnkVerbale" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="APRI VERBALE">APRI VERBALE</a>
                    <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="APRI PROFORMA">APRI PROFORMA</a>
                    <a ID="LnkElencoSc" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="APRI ELENCO SCADENZE">APRI ELENCO SCADENZE</a>
                    <a ID="LnkFatturePDF" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="APRI PDF FATTURE">APRI PDF FATTURE</a>
                </asp:Label>
            </div> 
            <div style="height:10px">&nbsp;</div>
            <div>
                <asp:Button ID="btnStampa" runat="server" class="btnstyle1R" Text="Proforma" Visible="true"/>
            </div>
            <div style="height:5px">&nbsp;</div>
            <div>
                <asp:Button ID="btnVerbale" runat="server" class="btnstyle1R" Text="Verbale" Visible="true"/>
            </div>
            <div style="height:5px">&nbsp;</div>
            <div>
                <asp:Button ID="btnElencoSc" runat="server" class="btnstyle2R" Text="Elenco Scadenze" Visible="true"/>
            </div>
            <div style="height:5px">&nbsp;</div>
            <div>
                <asp:CheckBox ID="chkVisElenco" runat="server" Font-Bold="false" ForeColor="Blue" Text="Elenco XLS" Checked="false" AutoPostBack="false" Visible="true"/>               
            </div>
            <%--<div style="height:5px">&nbsp;</div>--%>
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
        options2.height = 110;
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