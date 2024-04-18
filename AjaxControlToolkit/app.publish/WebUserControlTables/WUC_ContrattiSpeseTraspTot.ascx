<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ContrattiSpeseTraspTot.ascx.vb" Inherits="SoftAziOnLine.WUC_ContrattiSpeseTraspTot" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WFP_Vettori.ascx" tagname="WFP_Vettori" tagprefix="uc2" %>
<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<div align="left" style="border:1 solid White;  width: 1100px; height:610px;">
<uc1:ModalPopup ID="ModalPopup" runat="server" />
<asp:TabContainer ID="TabContainer3" runat="server" ActiveTabIndex="0" BorderStyle="Groove" Height="610px" style="margin-top: 0px" Width="1090px">
<asp:TabPanel ID="TabAttivita" runat="server" HeaderText="Attività">
  <ContentTemplate>
  <div align="left" style="border:1 solid White;  width: 1090px; height:610px;">
        <asp:Panel ID="Panel2" runat="server" Height="615px" Width ="1090px" CssClass ="sfondopagine">
           <asp:Panel ID="Panel3" runat="server" style="border:2 solid #000000">
           <table width="100%" >
           <tr>
                <td id="TDAggDateAtt" style="width:400px" class="styleBordato">
                    <table >
                        <tr>
                        <td>
                            <div style="overflow-x:hidden; overflow-y:auto;height:320px;width:380px;text-align:center;">
                                <asp:Label ID="lblMessScadAtt" runat="server" BorderStyle="Outset" 
                                    style="text-align:left" Font-Bold="True" ForeColor="Blue" Width="375px"></asp:Label>
                            </div>
                        </td>
                        </tr>
                        <tr>
                            <td align="center"><br />
                                <asp:Label ID="Label10" runat="server" BorderStyle="Outset" 
                                style="text-align:center" Font-Bold="True" ForeColor="Blue" Width="380px">Parametri aggiornamento Scadenze Attività</asp:Label>
                            </td>
                        </tr>
                         <tr>
                            <td align="left">
                                <asp:Label ID="Label11" runat="server" BorderStyle="None" 
                                style="text-align:left" Font-Bold="false" ForeColor="Blue" Width="120px">Data da cambiare</asp:Label>
                                <asp:TextBox ID="txtDataOLD" runat="server" Width="70px" style="text-align:left" 
                                            MaxLength="10" AutoPostBack="false" BorderStyle="None"></asp:TextBox>
                                <asp:ImageButton ID="imgBtnShowCalendarDOLD" runat="server" CausesValidation="False"  
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                <asp:CalendarExtender ID="txtDataOLD_CalendarExtender" runat="server" 
                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDOLD" 
                                    TargetControlID="txtDataOLD">
                                </asp:CalendarExtender>
                                <%--<asp:RegularExpressionValidator ID="DateRegexValidatorDOLD" runat="server" 
                                    ControlToValidate="txtDataOLD" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>

                                <asp:Label ID="Label12" runat="server" BorderStyle="None" 
                                style="text-align:center" Font-Bold="false" ForeColor="Blue">in</asp:Label>
                                <asp:TextBox ID="txtDataNEW" runat="server" Width="70px" style="text-align:left" 
                                            MaxLength="10" AutoPostBack="false" BorderStyle="None"></asp:TextBox>
                                <asp:ImageButton ID="imgBtnShowCalendarDNEW" runat="server" CausesValidation="False"  
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                <asp:CalendarExtender ID="txtDataNEW_CalendarExtender" runat="server" 
                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDNEW" 
                                    TargetControlID="txtDataNEW">
                                </asp:CalendarExtender>
                                <%--<asp:RegularExpressionValidator ID="DateRegexValidatorDNEW" runat="server" 
                                    ControlToValidate="txtDataNEW" ErrorMessage="*" BackColor="Red" BorderStyle="Outset" Font-Bold="true"
                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="chkTutteLeDate" runat="server" Font-Bold="false" Text="Cambia Tutte le Date (Solo GG/MM/Anno invariato)" Checked="false" AutoPostBack="true"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:Label ID="Label3" runat="server" BorderStyle="None" 
                                style="text-align:left" Font-Bold="false" ForeColor="Blue" Width="120px">Solo il Periodo</asp:Label>
                                <asp:DropDownList ID="DDLPeriodo" runat="server" Width="260px" AutoPostBack="false">
                                      </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td><br />
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:Label ID="Label13" runat="server" BorderStyle="None" 
                                style="text-align:left" Font-Bold="false" ForeColor="Blue" Width="120px">Solo il N° Serie</asp:Label>
                                <asp:DropDownList ID="DDLDurNumRiga" runat="server" Width="260px" AutoPostBack="false">
                                      </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Label ID="Label14" runat="server" BorderStyle="None" 
                                style="text-align:center" Font-Bold="True" ForeColor="Blue">Se non indicato N° Serie aggiorna tutti i Seriali</asp:Label>
                            </td>
                        </tr>
                         <tr>
                            <td>
                                <asp:CheckBox ID="chkNonAggConsum" runat="server" Font-Bold="false" Text="Non aggiornare la data dei consumabili" Checked="false" AutoPostBack="false"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center"><br />
                                <asp:Button ID="btnAggDataScAtt" runat="server" Width="380px" Height="30px" Text="Aggiorna Data Attività in altra Data" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Label ID="Label15" runat="server" BorderStyle="Outset" 
                                style="text-align:center" Font-Bold="True" ForeColor="Red">Saranno aggiornate solo le Attività non Evase</asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td id="TDScadAttCA" style="width:700px;visibility:visible" class="styleBordato">
                        <asp:Panel ID="Panel1" runat="server" BorderStyle="Solid" BorderWidth="1px">
                            <div id="divGridViewDettCAAtt" style="overflow: auto;width:670px; height:535px; border-style:groove">
                                <asp:GridView ID="GridViewDettCAAtt" runat="server" AutoGenerateColumns="False" 
                                    EmptyDataText="Nessun dato disponibile."  
                                    DataKeyNames="Riga" 
                                    GridLines="None" CssClass="GridViewStyle" EnableTheming="True"
                                    AllowPaging="true" 
                                    PageSize="18" 
                                    PagerSettings-Mode="NextPreviousFirstLast"
                                    PagerSettings-Visible="true"
                                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" >
                                    <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                                        LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                                        Mode="NextPreviousFirstLast" 
                                        NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                        PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                                    <RowStyle CssClass="RowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass="HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle"/>
                                    <Columns>
                                        <asp:BoundField DataField="Riga" HeaderText="N°" 
                                            SortExpression="Riga">
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Width="5px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="SerieLotto" HeaderText="Serie" 
                                            SortExpression="SerieLotto">
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Width="50px" Wrap="True"/>
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Cod_Articolo" HeaderText="Codice articolo" 
                                            SortExpression="Cod_Articolo">
                                            <HeaderStyle Wrap="true" />
                                            <ItemStyle Width="50px" Wrap="false" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Data Scadenza" SortExpression="TextDataSc">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDataSC" runat="server" Width="70px" MaxLength="10" Text='<%# Bind("TextDataSc") %>' BorderColor="White"></asp:TextBox>
                                                 <asp:ImageButton ID="imgBtnShowCalendarDSC" runat="server" CausesValidation="False"  
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataSC_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDSC" 
                                                    TargetControlID="txtDataSC">
                                                </asp:CalendarExtender>
                                            </ItemTemplate>
                                            <HeaderStyle Width="70px" Wrap="true" />
                                            <ItemStyle Width="70px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Data Evasione" SortExpression="TextDataEv">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDataEv" runat="server" Width="70px" MaxLength="10" Text='<%# Bind("TextDataEv") %>' BorderColor="White"></asp:TextBox>
                                                 <asp:ImageButton ID="imgBtnShowCalendarDEv" runat="server" CausesValidation="False"  
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataEv_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDEv" 
                                                    TargetControlID="txtDataEv">
                                                </asp:CalendarExtender>
                                            </ItemTemplate>
                                            <HeaderStyle Width="70px" Wrap="true" />
                                            <ItemStyle Width="70px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ev." ItemStyle-HorizontalAlign="Center" SortExpression="Qta_Selezionata">
                                            <ItemTemplate>
                                                <asp:CheckBox id="chkEvasa" runat="server" Enabled="true" AutoPostBack="false" Checked='<%# Convert.ToBoolean(Eval("Qta_Selezionata")) %>'>
                                                </asp:CheckBox> 
                                            </ItemTemplate>
                                            <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Data Scadenza Consumabile" SortExpression="TextRefDataNC">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDataEVN" runat="server" Width="70px" MaxLength="10" Text='<%# Bind("TextRefDataNC") %>' BorderColor="White"></asp:TextBox>
                                                 <asp:ImageButton ID="imgBtnShowCalendarDEVN" runat="server" CausesValidation="False"  
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                                 <asp:CalendarExtender ID="txtDataEVN_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDEVN" 
                                                    TargetControlID="txtDataEVN">
                                                </asp:CalendarExtender>
                                            </ItemTemplate>
                                            <HeaderStyle Width="70px" Wrap="true" />
                                            <ItemStyle Width="70px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Fatt." ItemStyle-HorizontalAlign="Center" SortExpression="Qta_Fatturata">
                                            <ItemTemplate>
                                                <asp:CheckBox id="chkFatturata" runat="server" Enabled="true" AutoPostBack="false" Checked='<%# Convert.ToBoolean(Eval("Qta_Fatturata")) %>'>
                                                </asp:CheckBox> 
                                            </ItemTemplate>
                                            <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="SWModAgenti" HeaderText="CK" 
                                            SortExpression="SWModAgenti">
                                            <HeaderStyle Wrap="True" HorizontalAlign="Center" CssClass="nascondi"/>
                                            <ItemStyle Width="5px" HorizontalAlign="Center" CssClass="nascondi"/>
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </asp:Panel>
                         <div>
                            <asp:Label ID="Label1" runat="server" Width="1px"></asp:Label>
                            <asp:Button ID="btnModScAttCA" runat="server" Width="295px" Height="30px" Text="Modifica dati Attività" />
                            <asp:Button ID="btnAggScAttCA" runat="server" Width="145px" Height="30px" Text="Aggiorna dati" ForeColor="White" BackColor="DarkRed" Visible="false" />
                            <asp:Button ID="btnAnnScAttCA" runat="server" Width="145px" Height="30px" Text="Annulla modifica" ForeColor="White" BackColor="DarkRed" Visible="false"/>
                            <asp:Label ID="Label5" runat="server" Text="Attività Evase senza Data sarà impostata la data odierna" style="text-align:left" visible="true" Font-Bold="true" Font-Size="XX-Small" ForeColor="Red" BorderStyle="Outset"></asp:Label>
                        </div>
                        <div>
                            <asp:Label ID="Label2" runat="server" Width="1px"></asp:Label>
                            <asp:Label ID="Label7" runat="server" BorderStyle="Outset" style="text-align:left" Font-Bold="true">Totale Attività</asp:Label>
                             <asp:Label ID="lblTotaleAtt" runat="server" BorderStyle="Outset" style="text-align:left" Font-Bold="True">0</asp:Label>
                        </div>
                    </td>
           </tr>
           </table>
           </asp:Panel> 
       </asp:Panel> 
  </div>
</ContentTemplate>
</asp:TabPanel>
<asp:TabPanel ID="TabScadenze" runat="server" HeaderText="Sconti - Spese - Totale documento - Scadenze">
  <ContentTemplate>
  <div align="left" style="border:1 solid White;  width: 1100px; height:610px;">
        <asp:Panel ID="PanelScontiSpeseTot" runat="server" Height="615px" Width ="1090px" CssClass ="sfondopagine">
           <asp:Panel ID="PanelDettScSpTot" runat="server" style="border:2 solid #000000">
           <table width="100%" >                         
                <tr>
                    <td colspan="3">
                    <table width="100%">
                    <tr>
                    <td style="width:100px;visibility:hidden">
                    Totale merce                
                    </td>
                    <td>
                    <asp:Label ID="lblTotMerce" runat="server" BorderStyle="Outset" Width="100px" 
                            style="text-align:right" Font-Bold="True" Visible="false">0,00</asp:Label>
                    </td>
                    <td style="visibility:hidden">
                    Sconto cassa %
                    </td>
                    <td align="left" colspan="2">
                        <asp:TextBox  ID="TxtScontoCassa" runat="server" Width="100px" 
                                style="text-align:right" MaxLength="10" AutoPostBack="true" BorderStyle="None" Visible="false"></asp:TextBox>
                        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" 
                                                        ControlToValidate="TxtScontoCassa" ErrorMessage="*" 
                                                        ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                        <asp:Label ID="Label6" runat="server" Width="100px"></asp:Label>
                        <asp:Button ID="btnAggFCFromOC" runat="server" Height="25px" ForeColor="Blue" Font-Bold="true" Text="Aggiorna N°Data Fattura da Ordine" />
                        <asp:CheckBox ID="chkCTRFatturato" runat="server" Text="Fatturato?" Font-Bold="false" Checked="false" AutoPostBack="true" Visible="true"/>
                        <asp:DropDownList ID="DDLCTRFatturato" runat="server" AutoPostBack="false" Width="300px" Enabled="false"></asp:DropDownList>
                        <%--Totale Lordo merce--%>  
                        <asp:Label ID="lblTotLordoMerce" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:right" Font-Bold="True"  Visible="false">0,00</asp:Label>
                        <asp:Label ID="Label39" runat="server" Font-Bold="false" Text="Deduzioni" style="text-align:right" Visible="false"></asp:Label>
                        <asp:Label ID="lblTotDeduzioni" runat="server" BorderStyle="Outset" Width="80px" 
                                style="text-align:right" Font-Bold="True" text="0,00" Visible="false"></asp:Label>   
                        <asp:Label ID="Label9" runat="server" Text="Spese Bollo" visible="false"></asp:Label>
                        <asp:TextBox  ID="txtBollo" runat="server" Width="50px" 
                            style="text-align:right" MaxLength="7" BorderStyle="None" visible="false"></asp:TextBox>
                            <asp:Label ID="Label37" runat="server" Text="a carico del:" visible="false"></asp:Label>
                        <asp:DropDownList ID="DDLSpeseBollo" runat="server" visible="false" 
                            AppendDataBoundItems="True" AutoPostBack="false" Height="22px" TabIndex="2" Width="70px">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Mittente" Value="M"></asp:ListItem>
                            <%--<asp:ListItem Text="Destinatario" Value="D"></asp:ListItem>--%>
                        </asp:DropDownList>  
                    </td>
                    </tr>
                    <tr>
                    <td style="width:100px;visibility:hidden">
                    Spese incasso                
                    </td>
                    <td style="visibility:hidden">
                    <asp:TextBox  ID="TxtSpeseIncasso" runat="server" Width="100px" 
                            style="text-align:right" MaxLength="10" BorderStyle="None" Visible="false"></asp:TextBox>
                   <%-- <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" 
                                                    ControlToValidate="TxtSpeseIncasso" ErrorMessage="*" 
                                                    ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                    </td>
                    <td style="visibility:hidden">
                    Spese trasporto
                    </td>
                    <td align="left" colspan="2">
                        <asp:TextBox ID="TxtSpeseTrasporto" runat="server" Width="100px" style="text-align:right" MaxLength="10" BorderStyle="None" Visible="false"></asp:TextBox>
                        <asp:Label ID="lblTotaleFatturato" runat="server" Text="" style="text-align:right" visible="true" Width="500px" Font-Bold="true" ForeColor="Blue"></asp:Label>
                    </td>
                    </tr>
                    <tr>
                    <td style="width:100px;visibility:hidden">
                    Spese imballo                
                    </td>
                    <td style="visibility:hidden">
                    <asp:TextBox  ID="TxtSpeseImballo" runat="server" Width="100px" style="text-align:right" MaxLength="10" BorderStyle="None"></asp:TextBox>
                    </td>
                    <%--<td>
                    Descrizione imballo
                    </td>--%>
                    <td colspan ="3" align="right">
                        <asp:TextBox  ID="TxtDescrizioneImballo" runat="server" Width="250px" MaxLength="150" BorderStyle="None" Visible="false"></asp:TextBox>
                        <asp:CheckBox ID="chkNoDivTotRate" runat="server" Font-Bold="false" Text="Totale Rate Scadenze per periodo" Checked="false" AutoPostBack="true" Height="21px"/>
                        <asp:CheckBox ID="chkAccorpaRateAA" runat="server" Font-Bold="false" Text="1 Fattura per Anno" Checked="false" AutoPostBack="true" Height="21px" />
                        <asp:Label ID="Label4" runat="server" Width="10px"></asp:Label>
                    </td>
                    </tr>                
                    </table>
                    </td>
                </tr>   
                <tr>
                    <td id="TDTotaleDoc" style="width:400px" class="styleBordato">
                        <div style="overflow-x:hidden; overflow-y:auto;height:190px;width:380px;text-align:center;">
                             <asp:Label ID="lblMessRatePag" runat="server" Text="" style="text-align:left" Width="375px" visible="true" Font-Bold="true" BorderStyle="Outset" ForeColor="Red"></asp:Label>
                        </div>
                        <table >
                        <tr>
                        <td style="width:80px">
                        &nbsp;
                        </td>
                        <td>
                        Cod.IVA
                        </td>
                        <td align="right">
                        Imponibile
                        </td>
                        <td align="right">
                        Imposta
                        </td>
                        </tr>
                        <tr>
                        <td style="width:80px">
                        Riepilogo
                        </td>
                        <td>
                        <asp:Label ID="LblIVA1" runat="server" BorderStyle="Outset" Width="50px" 
                                style="text-align:center" Font-Bold="True" ></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImponibile1" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImposta1" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:80px">
                        Imposte
                        </td>
                        <td>
                        <asp:Label ID="LblIVA2" runat="server" BorderStyle="Outset" Width="50px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImponibile2" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImposta2" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:80px">
                        &nbsp;
                        </td>
                        <td>
                        <asp:Label ID="LblIVA3" runat="server" BorderStyle="Outset" Width="50px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImponibile3" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImposta3" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:80px">
                        &nbsp;
                        </td>
                        <td>
                        <asp:Label ID="LblIVA4" runat="server" BorderStyle="Outset" Width="50px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImponibile4" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImposta4" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:80px">
                        &nbsp;
                        </td>
                        <td>
                        &nbsp;
                        </td>
                        <td>
                        <asp:Label ID="LblTotaleImpon" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblTotaleImposta" runat="server" BorderStyle="Outset" Width="110px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="center"><%--<br />--%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"><%--style="width:80px"--%> 
                            Totale documento
                            </td>
                            <td>
                            <asp:Label ID="LblTotDocumento" runat="server" BorderStyle="Outset" Width="110px" 
                                    style="text-align:right" Font-Bold="True">0,00</asp:Label>
                            </td>
                            <td>
                                <asp:CheckBox ID="ChkSplitIVA" runat="server" Font-Bold="true" Text="Split IVA" 
                                    Checked="false" AutoPostBack="true" Width="115px" Height="21px"/>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="center"><%--<br />--%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" style="visibility:hidden">
                                <asp:Label ID="Label36" runat="server" BorderStyle="None"  
                                style="text-align:left" Font-Bold="True" Text="Arrotondamenti sul Netto a pagare"></asp:Label>
                            </td>
                            <td style="visibility:hidden">
                                <asp:TextBox ID="txtAbbuono" runat="server" Width="110px" style="text-align:right" 
                                            MaxLength="10" AutoPostBack="true" BorderStyle="None"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="center"><%--<br />--%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="Label8" runat="server" BorderStyle="Outset" Width="135px" 
                                    style="text-align:center" Font-Bold="True">Netto a pagare</asp:Label>
                            </td>
                            <td colspan="2">
                               <asp:Label ID="LblTotNettoPagare" runat="server" BorderStyle="Outset" Width="230px" 
                                    style="text-align:center" Font-Bold="True">0,00</asp:Label>
                            </td>
                        </tr>
                        
                        </table>
                    </td>                
                    <td id="TDScadPagCA" style="width:700px;visibility:visible" class="styleBordato">
                        <asp:Panel ID="PanelBody" runat="server" BorderStyle="Solid" BorderWidth="1px">
                            <div id="divGridViewDettCASC" style="overflow: auto;width:670px; height:455px; border-style:groove">
                                <asp:GridView ID="GridViewDettCASC" runat="server" AutoGenerateColumns="False" 
                                    EmptyDataText="Nessun dato disponibile."  
                                    DataKeyNames="NRata" 
                                    GridLines="None" CssClass="GridViewStyle" EnableTheming="True" 
                                    PagerSettings-Mode="NextPreviousFirstLast"
                                    PagerSettings-Visible="True"
                                    PagerSettings-FirstPageImageUrl="~/Immagini/GridView/page-first.gif"
                                    PagerSettings-LastPageImageUrl="~/Immagini/GridView/page-last.gif"
                                    PagerSettings-NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                    PagerSettings-PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" >
                                    <PagerSettings FirstPageImageUrl="~/Immagini/GridView/page-first.gif" 
                                        LastPageImageUrl="~/Immagini/GridView/page-last.gif" 
                                        Mode="NextPreviousFirstLast" 
                                        NextPageImageUrl="~/Immagini/GridView/page-next.gif" 
                                        PreviousPageImageUrl="~/Immagini/GridView/page-prev.gif" />
                                    <RowStyle CssClass="RowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass="HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle"/>
                                    <Columns>
                                        <asp:BoundField DataField="NRata" HeaderText="N°" 
                                            SortExpression="NRata" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Width="5px" />
                                        </asp:BoundField>
                                         <asp:TemplateField HeaderText="Data Scadenza" SortExpression="Data">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDataScAtt" runat="server" Width="70px" Text='<%# Bind("Data") %>' BorderColor="White" Enabled="false" BorderStyle="None"></asp:TextBox>
                                            </ItemTemplate>
                                            <HeaderStyle Width="50px" Wrap="true" />
                                            <ItemStyle Width="50px" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Importo" HeaderText="Importo Rata" 
                                            SortExpression="Importo">
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Width="50px" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Ev." ItemStyle-HorizontalAlign="Center" SortExpression="Evasa">
                                            <ItemTemplate>
                                                <asp:CheckBox id="chkEvasa" runat="server" Enabled="true" AutoPostBack="false" Checked='<%# Convert.ToBoolean(Eval("Evasa")) %>'></asp:CheckBox> 
                                            </ItemTemplate>
                                            <HeaderStyle Width="5px"/><ItemStyle Width="5px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="N° Fattura/e" SortExpression="NFC">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtNFC" runat="server" Width="100px" MaxLength="300" Text='<%# Bind("NFC") %>' BorderColor="White"></asp:TextBox>
                                            </ItemTemplate>
                                            <HeaderStyle Width="100px" Wrap="true" />
                                            <ItemStyle Width="100px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Data Fattura" SortExpression="DataFC">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtDataFC" runat="server" Width="70px" MaxLength="10" Text='<%# Bind("DataFC") %>' BorderColor="White"></asp:TextBox>
                                                 <asp:ImageButton ID="imgBtnShowCalendartxtDataFC" runat="server" CausesValidation="False"  
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="txtDataFC_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendartxtDataFC" 
                                                    TargetControlID="txtDataFC">
                                                </asp:CalendarExtender>
                                            </ItemTemplate>
                                            <HeaderStyle Width="90px" Wrap="false" />
                                            <ItemStyle Width="90px" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ImportoF" HeaderText="Importo Fatturato" 
                                            SortExpression="ImportoF">
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Width="50px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ImportoR" HeaderText="Importo Residuo" 
                                            SortExpression="ImportoR">
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Width="50px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Serie" HeaderText="Serie" 
                                            SortExpression="Serie">
                                            <HeaderStyle Wrap="false" />
                                            <ItemStyle Width="50px" Wrap="false" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </asp:Panel>
                        <div>
                            <asp:Button ID="btnModScPagCA" runat="server" Width="265px" Height="30px" Text="Modifica dati Scadenze" />
                            <asp:Button ID="btnAggScPagCA" runat="server" Width="130px" Height="30px" Text="Aggiorna dati" ForeColor="White" BackColor="DarkRed" Visible="false" />
                            <asp:Button ID="btnAnnScPagCA" runat="server" Width="130px" Height="30px" Text="Annulla modifica" ForeColor="White" BackColor="DarkRed" Visible="false"/>
                        </div>
                        <div>
                            <asp:Label ID="lblNumRate" runat="server" BorderStyle="Outset" Width="125px" 
                                style="text-align:left" Font-Bold="true">Totale rate</asp:Label>
                             <asp:Label ID="LblTotaleRate" runat="server" BorderStyle="Outset" Width="125px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                            <asp:Label ID="Label16" runat="server" Text=" Fatturato: "></asp:Label>
                            <asp:Label ID="lblTotFatturato" runat="server" BorderStyle="Outset" Width="125px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                            <asp:Label ID="Label19" runat="server" Text=" Residuo: "></asp:Label>
                            <asp:Label ID="lblTotResiduo" runat="server" BorderStyle="Outset" Width="125px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </div>
                    </td>
                </tr>     
                </table>
           </asp:Panel> 
       </asp:Panel> 
  </div>
</ContentTemplate>
</asp:TabPanel>
</asp:TabContainer>
</div>