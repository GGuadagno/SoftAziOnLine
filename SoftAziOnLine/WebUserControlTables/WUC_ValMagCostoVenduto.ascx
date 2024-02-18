<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ValMagCostoVenduto.ascx.vb" Inherits="SoftAziOnLine.WUC_ValMagCostoVenduto" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_Articolo_SelezSing.ascx" tagname="WFP_Articolo_SelezSing" tagprefix="uc10" %>
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
    .styleBordo
    {
        height: 35px;
        width: 860px;
        border-style:groove;
    }
    .style7
    {
        height: 324px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc10:WFP_Articolo_SelezSing ID="WFP_Articolo_SelezSing1" runat="server" />
 <br /> <br /> <br /> 
            <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 340px; width: 940px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelMagazzino" style="margin-top: 0px;" runat="server" GroupingText="Magazzino">
                        <asp:Label ID="lblMagazzino" runat="server" Width="146px" Height="16px">Magazzino</asp:Label>
                           <asp:Button ID="btnMagazzino" runat="server" Height="22px" Text="?"
                                Visible="False" Width="30px" /> &nbsp;
                            <asp:TextBox ID="txtCodMagazzino" runat="server" AutoPostBack="true"
                               Width="80px" TabIndex="1"></asp:TextBox>  &nbsp;
                                <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                                                                        AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                                                                        DataTextField="Descrizione" 
                            DataValueField="Codice" Width="545px" TabIndex="2">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                
                                                                <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                                                                    SelectCommand="SELECT Codice, Descrizione FROM Magazzini ORDER BY Descrizione">
                                                                </asp:SqlDataSource>
                    </asp:Panel>
                    <%--<asp:Panel ID="PanelRicerca" style="margin-top: 0px;" runat="server" GroupingText="Ricerca per">
                        <asp:Label ID="lblRicerca" runat="server" Width="155px" Height="16px">Codice</asp:Label>
                            <asp:TextBox ID="txtRicerca" runat="server" AutoPostBack="false"
                               Width="635px" ></asp:TextBox>
                    </asp:Panel>--%>
                     <asp:Panel ID="PanelOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Ordinamento">
                        <table style="width: 100%">
                        <tr>
                            <td width="100%" align="center">
                            <asp:RadioButton ID="rbtnCodice" runat="server" Text="Codice articolo" 
                            AutoPostBack="false" GroupName="Ordinamento" TabIndex="3" />
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione" 
                        style="margin-top: 0px;" Width="847px"> 
                    <table>
                    <tr>
                        <td><asp:Label ID="lblCategoria" runat="server" Width="145px" Height="16px">Categoria</asp:Label>
                            <asp:Button ID="btnTrovaCategoria" runat="server" Height="22px" Text="?"
                                Visible="False" Width="30px" /> &nbsp;
                            <asp:TextBox ID="txtCodCategoria" runat="server" AutoPostBack="true"
                               Width="80px" TabIndex="6" MaxLength="3"></asp:TextBox> &nbsp;
                                <asp:DropDownList ID="ddlCatgoria" runat="server" AppendDataBoundItems="true"
                                                                        AutoPostBack="true" DataSourceID="SqlDataSourceCategoria" 
                                                                        DataTextField="Descrizione" 
                                DataValueField="Codice" Width="545px" TabIndex="7">
                                                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                
                                                                <asp:SqlDataSource ID="SqlDataSourceCategoria" runat="server"
                                                                    SelectCommand="SELECT Codice, Descrizione FROM CategArt ORDER BY Descrizione">
                                                                </asp:SqlDataSource>
                           </td>
                    </tr>
                    <tr>
                        <td>
                           <asp:Label ID="lblLinea" runat="server" Width="145px" Height="16px">Linea</asp:Label>
                                <asp:Button ID="btnTrovaLinea" runat="server" Height="22px" Text="?"
                                   Visible="False" Width="30px" /> &nbsp;
                                <asp:TextBox ID="txtCodLinea" runat="server" AutoPostBack="true" 
                                Width="80px" TabIndex="8" MaxLength="3"></asp:TextBox> &nbsp;
                                 <asp:DropDownList ID="ddlLinea" runat="server" AppendDataBoundItems="true"
                                 AutoPostBack="true" DataSourceID="SqlDataSourceLinea" 
                                  DataTextField="Descrizione" DataValueField="Codice" Width="545px" 
                                TabIndex="9">
                                 <asp:ListItem Text="" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSourceLinea" runat="server"
                                SelectCommand="SELECT Codice, Descrizione FROM LineeArt ORDER BY Descrizione" />
                        </td>
                    </tr>
                    <tr>
                    <td>
                        <asp:Label ID="lblDal" runat="server" Width="145px" Height="17px">Dal codice articolo</asp:Label>
                           &nbsp; <asp:Button ID="btnCod1" runat="server" Width="30px" Height="22px" Text="?" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server"  
                                Width="145px" MaxLength="20" AutoPostBack="false" TabIndex="10" ></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblAl" runat="server" Width="135px" Height="17px">Al codice articolo</asp:Label>
                            &nbsp;
                            <asp:Button ID="btnCod2" runat="server" Height="22px" Width="30px" Text="?" />
                        &nbsp;&nbsp;
                            <asp:TextBox ID="txtCod2" runat="server"  
                                Width="145px" MaxLength="20" TabIndex="11" AutoPostBack="false" ></asp:TextBox>
                        <tr>
                            <td>
                                <table style="width: 100%;">
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%;">
                                    <tr>
                                        <td align="left" width="100%">
                                            <asp:CheckBox ID="chkArtGiacNegativa" runat="server" AutoPostBack="false" 
                                                TabIndex="12" Text="Stampa solo articoli con giacenza NEGATIVA" 
                                                Visible="true" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <br />
                                <table style="width: 100%;">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label1" runat="server" Height="17px" Width="170px">Costo del Venduto alla data</asp:Label>
                                            &nbsp;<asp:TextBox ID="txtData" runat="server" MaxLength="10" TabIndex="1" 
                                                Width="70px"></asp:TextBox>
                                            <asp:ImageButton ID="imgBtnShowCalendar" runat="server" 
                                                ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                ToolTip="apri il calendario" />
                                            <asp:CalendarExtender ID="txtData_CalendarExtender" runat="server" 
                                                Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar" 
                                                TargetControlID="txtData">
                                            </asp:CalendarExtender>
                                            <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                ControlToValidate="txtData" ErrorMessage="*" 
                                                ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />
                                            &nbsp;&nbsp;<asp:CheckBox ID="chkDebug" runat="server" AutoPostBack="false" TabIndex="12" 
                                                Text="Seleziona dati dall'inizio mese della data richiesta (N.B. DATI NEGATIVI)" 
                                                Visible="true" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%;">
                                    <asp:Label ID="lblMess" runat="server" Height="17px" Visible="false" ForeColor="Red" Font-Bold="true"></asp:Label>
                                </table>
                            </td>
                        </tr>
                    </tr>
                    </table>
                      <br>
                    </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div style="height: 25px" id="noradio">
                                </div>
                                <div style="height: 25px">
                                </div>
                                <div style="height: 60px">
                                </div>
                                <div>
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa analitica" 
                                         TabIndex="14" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnStampaSint" runat="server" class="btnstyle" Text="Stampa sintetica" 
                                         TabIndex="14" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" 
                                        TabIndex="15" />
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div style="height: 15px">
                                </div> 
                                <div>
                                    <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Stampa">Apri Stampa</a>
                                </div>
                                <div style="height: 15px">
                                </div>                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>   
</asp:Panel>