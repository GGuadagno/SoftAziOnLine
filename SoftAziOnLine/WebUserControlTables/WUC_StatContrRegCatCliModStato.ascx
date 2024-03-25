<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StatContrRegCatCliModStato.ascx.vb" Inherits="SoftAziOnLine.WUC_StatContrRegCatCliModStato" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_ElencoCliForn.ascx" tagname="WFP_ElencoCliForn" tagprefix="uc2" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="500px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCli" runat="server" Elenco="ListaClienti" Titolo="Elenco clienti" />
    <asp:SqlDataSource ID="SqlDa_Regioni" runat="server" 
        SelectCommand="SELECT [Codice], [Descrizione] FROM [Regioni]">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="SqlDSProvince" runat="server" 
        SelectCommand="SELECT Codice, Descrizione FROM [Province] WHERE [Regione] = CASE WHEN @CodRegione = 0 THEN Regione ELSE @CodRegione END ORDER BY [Descrizione]">
        <SelectParameters>
            <asp:SessionParameter Name="CodRegione" DbType="Int32" Direction="Input" SessionField="CodRegione" />
        </SelectParameters>
    </asp:SqlDataSource>
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 350px; width: 927px;" >
            <tr>
                <td>
                    <asp:Panel ID="Panel2" runat="server" groupingtext="Clienti" style="margin-top: 0px;" Height="78px" Width="859px">
                        <asp:Label ID="lblCliente" runat="server" Width="100px" Height="17px" Enabled="false">Cliente</asp:Label>
                        <asp:Button ID="btnCliente" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca cliente" Enabled="false"/>
                        &nbsp;&nbsp;<asp:TextBox ID="txtCodCliente" runat="server"  Width="100px" MaxLength="16" AutoPostBack="True" TabIndex="7" Enabled="false"></asp:TextBox>
                        &nbsp;&nbsp;<asp:TextBox ID="txtDescCliente" runat="server" Width="400px" MaxLength="50" TabIndex="8" Enabled="False"  ></asp:TextBox>
                        <br>
                        <asp:CheckBox ID="chkTuttiClienti" runat="server" AutoPostBack="True" Checked="true" TabIndex="9" Text="Seleziona tutti i clienti" TextAlign="Left" />
                        <br>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezionaRegioneProv" style="margin-top: 0px;" runat="server" GroupingText="Regioni/Provincia">
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
                            <asp:CheckBox ID="chkTutteRegioni" runat="server" Text="Seleziona tutte le regioni" AutoPostBack="true" Checked="True" />
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
                    <asp:Panel ID="PanelSelezionaDate" style="margin-top: 0px;" runat="server" GroupingText="Date Contratto / Tipo evasione">
                        <div>
                            <asp:Label ID="lblDallaData" runat="server" Height="16px" Width="100px">Dalla data</asp:Label>
                            <asp:TextBox ID="txtDataDa" runat="server" Width="80px" MaxLength="10" AutoPostBack="false" TabIndex="1"></asp:TextBox>
                            <asp:ImageButton ID="imgBtnShowCalendarDa" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="2" />
                                                <asp:CalendarExtender ID="txtDataDa_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDa" 
                                                    TargetControlID="txtDataDa">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="txtDataDa" ErrorMessage="*" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                            &nbsp;<asp:Label ID="lblAllaData" runat="server" Height="20px" Width="66px">Alla data</asp:Label>
                            <asp:TextBox ID="txtDataA" runat="server" Width="80px" MaxLength="10" AutoPostBack="false" TabIndex="3"></asp:TextBox>
                            <asp:ImageButton ID="imgBtnShowCalendarA" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" TabIndex="4" />
                                                <asp:CalendarExtender ID="txtDataA_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarA" 
                                                    TargetControlID="txtDataA">
                                                </asp:CalendarExtender>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="txtDataA" ErrorMessage="*" 
                            ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                        </div>
                        <div>&nbsp;</div>
                        <div>
                            <asp:Label ID="Label1" runat="server" Height="16px"  Width="100px">Tipo evasione</asp:Label>
                            <asp:RadioButton ID="rbtnEvaso" runat="server" Text="Evasi" AutoPostBack="true" GroupName="Tipo" />
                            <asp:Label ID="Label0" runat="server" Width="5px">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnDaEvadere" runat="server" Text="Da evadere" AutoPostBack="true" GroupName="Tipo" />
                            <asp:Label ID="Label2" runat="server" Width="5px">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnParzEvaso" runat="server" Text="Parzialmente evasi" AutoPostBack="true" GroupName="Tipo" />
                            <asp:Label ID="Label3" runat="server" Width="5px">&nbsp;</asp:Label>
                             <asp:RadioButton ID="rbtnDaEvParEv" runat="server" Text="Da evadere + Parzialmente evasi" AutoPostBack="true" GroupName="Tipo"  Checked="true" />
                            <asp:Label ID="Label4" runat="server" Width="5px">&nbsp;</asp:Label>
                            <asp:RadioButton ID="rbtnTutti" runat="server" Text="Tutti" Checked="false" AutoPostBack="true" GroupName="Tipo" />
                        </div>
                     </asp:Panel>
                     <asp:Panel ID="PanelCategoria" style="margin-top: 0px;" runat="server" GroupingText="Categoria Clienti">
                    <table width="100%">
                        <tr>
                            <td align="left">Singola categoria</td><td>
                            <asp:DropDownList ID="ddlCatCli" runat="server" DataSourceID="SqlDa_CatCli" 
                                DataTextField="Descrizione" DataValueField="Codice" Width="400px" 
                                AppendDataBoundItems="true" Enabled="False">
                                <asp:ListItem Value="0" Text="Categoria non definita"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:SqlDataSource ID="SqlDa_CatCli" runat="server" 
                                SelectCommand="SELECT [Codice], [Descrizione] FROM [Categorie]">
                            </asp:SqlDataSource>
                            </td>
                            <td>
                            <asp:CheckBox ID="chkTuttiCategorie" runat="server" Text="Seleziona tutte le categorie" AutoPostBack="true" Checked="True" />
                            </td>
                        </tr>
                        <tr>
                            <td align="left"></td>
                            <td>
                            <asp:CheckBox ID="chkRaggrCatCli" runat="server" Text="Seleziona tutte le categorie per descrizione iniziale uguale" AutoPostBack="true" Checked="False" Enabled="false" />
                            </td>
                            <td>
                            <asp:CheckBox ID="chkAccorpaCC" runat="server" Text="Accorpa le categorie" AutoPostBack="False" Checked="False" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelModello" runat="server" groupingtext="Modello" style="margin-top: 0px;" Height="55px" Width="859px">
                        <%--<asp:Label ID="Label23" runat="server" BorderStyle="None" Font-Bold="false" Text="Modello"></asp:Label>--%>
                        <asp:CheckBox ID="chkHS1" runat="server" AutoPostBack="True" TabIndex="10" Text="HS1" TextAlign="Left" ToolTip="1" />
                        <asp:CheckBox ID="chkFR2" runat="server" AutoPostBack="True" TabIndex="10" Text="FR2" TextAlign="Left" ToolTip="2" />
                        <asp:CheckBox ID="chkFR3" runat="server" AutoPostBack="True" TabIndex="10" Text="FR3" TextAlign="Left" ToolTip="3" />
                        <asp:CheckBox ID="chkFRX" runat="server" AutoPostBack="True" TabIndex="10" Text="FRX" TextAlign="Left" ToolTip="4" />
                        <asp:CheckBox ID="chkC1" runat="server" AutoPostBack="True" TabIndex="10" Text="C1" TextAlign="Left" ToolTip="5" />
                        <asp:CheckBox ID="chkC2" runat="server" AutoPostBack="True" TabIndex="10" Text="C2" TextAlign="Left" ToolTip="6" />
                        <asp:Label ID="Label5" runat="server" Width="50px"></asp:Label>
                        <asp:CheckBox ID="chkMisti" runat="server" AutoPostBack="True" TabIndex="10" Font-Bold="true" ForeColor="Blue" Text="Misti" TextAlign="Left" ToolTip="8" />
                        <%--<asp:DropDownList ID="DDLModello" runat="server" Width="60px" AutoPostBack="false">
                                        <asp:ListItem Text="" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="HS1" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="FR2" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="FR3" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="FRX" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="C1" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="C2" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="Misti" Value="9"></asp:ListItem>
                                  </asp:DropDownList>--%>
                         <asp:Label ID="Label6" runat="server" Width="50px"></asp:Label>
                        <asp:CheckBox ID="chkTuttiModelli" runat="server" AutoPostBack="True" TabIndex="10" Font-Bold="true" ForeColor="Blue" Text="Seleziona tutti" TextAlign="Left" ToolTip="9" /> 
                    </asp:Panel>
                    <asp:Panel ID="Panel1" runat="server" groupingtext="Tipo stampa" style="margin-top: 0px;" Height="55px" Width="859px">
                        <asp:RadioButton ID="rbtnPDF" runat="server" Text="PDF" AutoPostBack="true" GroupName="TipoST" />
                        <asp:Label ID="Label8" runat="server" Width="5px">&nbsp;</asp:Label>
                        <asp:RadioButton ID="rbtnXLS" runat="server" Text="EXCEL" AutoPostBack="true" Checked="true" GroupName="TipoST" />
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