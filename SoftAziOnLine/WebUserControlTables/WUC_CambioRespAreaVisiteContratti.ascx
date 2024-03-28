<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_CambioRespAreaVisiteContratti.ascx.vb" Inherits="SoftAziOnLine.WUC_CambioRespAreaVisiteContratti" %>
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
    .btnstyleDoppio
        {
            Width: 108px;
            height: 48px;
        margin-left: 0px;
        white-space: pre-wrap;   
        }
    .styleMenu
    {
        width: auto;
        border-style:groove;
    }
    .styleBordo
    {
        height: 35px;
        width: 860px;
        border-style:groove;
    }
    .style7
    {
        height: 185px;
    }
</style>    
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="600px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <asp:SqlDataSource ID="SqlDa_Regioni" runat="server" 
        SelectCommand="SELECT [Codice], [Descrizione] FROM [Regioni]">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDSProvince" runat="server" 
        SelectCommand="SELECT Codice, Descrizione FROM [Province] WHERE [Regione] = CASE WHEN @CodRegione = 0 THEN Regione ELSE @CodRegione END ORDER BY [Descrizione]">
        <SelectParameters>
            <asp:SessionParameter Name="CodRegione" DbType="Int32" Direction="Input" SessionField="CodRegione" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDSRespVisite" runat="server" 
        SelectCommand="Get_RespVisiteArea"> 
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDSRegPrElenco" runat="server" 
        SelectCommand="SELECT RespVisiteRegPr.Codice, CodRespVisite, CodRegione, Provincia, Regioni.Descrizione AS DesRegione FROM RespVisiteRegPr LEFT OUTER JOIN Regioni ON RespVisiteRegPr.CodRegione = Regioni.Codice WHERE CodRespVisite = @IDRespVisite ORDER BY Regioni.Descrizione,Provincia">
        <SelectParameters>
            <asp:SessionParameter Name="IDRespVisite" DbType="Int32" Direction="Input" SessionField="IDRespVisite" />
        </SelectParameters>
    </asp:SqlDataSource> 
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 550px; width: 927px;" >
            <tr>
                <td>
                    <asp:Panel ID="Panel2" style="margin-top: 0px;" runat="server" GroupingText="Seleziona il Responsabile Visita da cambiare nel NUOVO Responsabile Visita">
                        <table width="100%">
                            <td align="left" style="width:230px">Responsabile Visita da cambiare</td>
                            <td>
                                <asp:DropDownList ID="ddlRespVisiteOLD" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="True" Width="590px"
                                DataSourceID="SqlDSRespVisite" DataTextField="DesRespVisiteArea" 
                                DataValueField="Codice" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                            </td>
                        </table>
                        <table width="100%">
                            <td>
                            <div style="height:25px;">
                                <asp:Label ID="Label5" 
                                    runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                                    Style="text-align:center" Text="Elenco abbinamenti Regioni/Province del Responsabile Visita da cambiare" Width="99%"></asp:Label>
                            </div>
                            <div id="divGridRegProv" style="overflow: auto; height:110px; border-style:groove">
                                <asp:GridView ID="GridViewBody" runat="server" 
                                    GridLines="None" CssClass="GridViewStyle" 
                                    AllowSorting="False" AutoGenerateColumns="False" 
                                    EmptyDataText="Nessun dato disponibile."  
                                    DataKeyNames="Codice"
                                    ShowFooter="false"
                                    DataSourceID="SqlDSRegPrElenco" BackColor="Silver">
                                    <RowStyle CssClass="RowStyle" />
                                    <PagerStyle CssClass="PagerStyle" />
                                    <HeaderStyle CssClass="HeaderStyle" />
                                    <AlternatingRowStyle CssClass="AltRowStyle" />
                                    <SelectedRowStyle CssClass="SelectedRowStyle"/>         
                                    <PagerSettings Mode="NextPrevious" Visible="false"/>
                                    <Columns>
                                        <asp:CommandField ButtonType="Button" CausesValidation="False" 
                                            ControlStyle-Font-Size="XX-Small" InsertVisible="False" SelectText="&gt;" 
                                            ShowCancelButton="False" ShowHeader="True" ShowSelectButton="true">
                                            <ControlStyle Font-Size="XX-Small" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="05px" />
                                        </asp:CommandField>
                                        <asp:BoundField DataField="DesRegione" HeaderText="Regione" 
                                            SortExpression="DesRegione"><HeaderStyle Wrap="false" />
                                                    <ItemStyle Width="250px" /></asp:BoundField>
                                        <asp:BoundField DataField="Provincia" HeaderText="Provincia" 
                                            SortExpression="Provincia">
                                            <HeaderStyle Wrap="false" />
                                            <ItemStyle Width="50px" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Codice" HeaderText="Codice"  
                                            SortExpression="Codice"><HeaderStyle Wrap="True" Width="10px" CssClass="nascondi"/><ItemStyle 
                                            Width="10px" CssClass="nascondi" /></asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div>
                                <asp:Button ID="btnAbbinaRegPr" runat="server" Text="Abbina Regione/Provincia al NUOVO Responsabile Visita" Enabled="false" />
                                <asp:Label ID="lblMessUtente" 
                                    runat="server" BorderStyle="Outset" Font-Bold="false" Font-Overline="False" 
                                    Style="text-align:left" Text="" Width="99%"></asp:Label>
                            </div>
                            </td>
                            </table>
                        <table width="100%">
                            <td align="left" style="width:230px">NUOVO Responsabile Visita</td>
                            <td>
                                <asp:DropDownList ID="ddlRespVisiteNEW" runat="server" AppendDataBoundItems="True" 
                                AutoPostBack="True" Width="590px"
                                DataSourceID="SqlDSRespVisite" DataTextField="DesRespVisiteArea" 
                                DataValueField="Codice" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                            </td>
                        </table>
                    </asp:Panel>
                   
                    <asp:Panel ID="PanelSelezionaRegioneProv" style="margin-top: 0px;" runat="server" GroupingText="Seleziona la Regione/Provincia per il cambio Responsabile Visita">
                    <table width="100%">
                        <tr>
                            <td align="left">Singola regione</td><td>
                            <asp:DropDownList ID="ddlRegioni" runat="server" AutoPostBack="true" DataSourceID="SqlDa_Regioni" 
                                DataTextField="Descrizione" DataValueField="Codice" Width="400px" 
                                AppendDataBoundItems="true" Enabled="true">
                                <asp:ListItem Value="0" Text="Regione non definita"></asp:ListItem>
                            </asp:DropDownList>
                            </td>
                            <td>
                            <asp:CheckBox ID="chkTutteRegioni" runat="server" Text="Seleziona tutte le regioni" AutoPostBack="true" Checked="false" visible="false"/>
                            </td>
                         </tr>
                         <tr>
                            <td align="left">Singola provincia</td><td>
                            <asp:DropDownList ID="ddlProvince" runat="server" AutoPostBack="false" DataSourceID="SqlDSProvince" 
                                DataTextField="Descrizione" DataValueField="Codice" Width="400px">
                            </asp:DropDownList>
                            </td>
                            <td>
                            <asp:CheckBox ID="chkTutteProvince" Text="Seleziona tutte le province" runat="server" AutoPostBack="True" />
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;" runat="server" GroupingText="Date Scadenze Attività">
                        <div>
                            <asp:Label ID="lblDallaData" runat="server" Height="16px" Width="100px">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" Width="80px" MaxLength="10" AutoPostBack="false" TabIndex="1"></asp:TextBox>
                            <asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="2" />
                                                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                    TargetControlID="txtDataDa">
                                                </asp:CalendarExtender>
                                                <%--<asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="txtDataDa" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                            &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>
                            <asp:TextBox ID="txtDataA" runat="server" Width="80px" MaxLength="10" AutoPostBack="false" TabIndex="3"></asp:TextBox>
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="4" />
                                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                    TargetControlID="txtDataA">
                                                </asp:CalendarExtender>
                                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtDataA" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                        </div>
                        <div>&nbsp;</div>
                        <div>
                            <asp:Label ID="Label1" runat="server" Height="16px"  Width="100px">Tipo evasione</asp:Label>
                            <asp:RadioButton ID="rbtnEvaso" runat="server" Text="Evasi" AutoPostBack="true" GroupName="Tipo" Enabled="false" visible="false" />
                            <asp:Label ID="Label0" runat="server" Width="5px" visible="false">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnDaEvadere" runat="server" Text="Da evadere" AutoPostBack="true" GroupName="Tipo" Enabled="false" visible="false" />
                            <asp:Label ID="Label2" runat="server" Width="5px" visible="false">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnParzEvaso" runat="server" Text="Parzialmente evasi" AutoPostBack="true" GroupName="Tipo" Enabled="false" visible="false" />
                            <asp:Label ID="Label3" runat="server" Width="5px" visible="false">&nbsp;</asp:Label>
                             <asp:RadioButton ID="rbtnDaEvParEv" runat="server" Text="Da evadere + Parzialmente evasi" AutoPostBack="true" GroupName="Tipo"  Checked="true" Enabled="false" />
                            <asp:Label ID="Label4" runat="server" Width="5px" visible="false">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" Checked="false" AutoPostBack="true" GroupName="Tipo" Enabled="false" visible="false" />
                        </div>
                     </asp:Panel>
                    <asp:Panel ID="Panel1" runat="server" groupingtext="Tipo stampa" style="margin-top: 0px;" Height="55px" Width="859px">
                        <asp:RadioButton ID="rbtnPDF" runat="server" Text="PDF" AutoPostBack="true" Checked="true" GroupName="TipoST" />
                        <asp:Label ID="Label8" runat="server" Width="5px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnXLS" runat="server" Text="EXCEL" AutoPostBack="true" Checked="false" GroupName="TipoST" visible="true" />
                        <asp:Label ID="Label7" runat="server" Width="50px"></asp:Label>
                        <a ID="lnkElenco" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Elenco" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Elenco</a>
                    </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" 
                                         TabIndex="21" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="22" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    </tr>
                </caption>
                </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>