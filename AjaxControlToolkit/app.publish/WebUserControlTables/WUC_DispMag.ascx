<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DispMag.ascx.vb" Inherits="SoftAziOnLine.WUC_DispMag"  %>
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
    .style9
    {
        width: 847px;
        height: 324px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" 
    BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <uc2:WFP_ElencoCliForn ID="WFP_ElencoCliForn1" runat="server" Elenco="ListaFornitori" Titolo="Elenco fornitori" />
            <br /> <br />
            <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 340px; width: 940px;">
            <tr>
                <td>
                    <asp:Panel ID="PanelMagazzino" style="margin-top: 0px;" runat="server" GroupingText="Magazzino">
                        <asp:Label ID="lblMagazzino" runat="server" Width="146px" Height="16px">Magazzino</asp:Label>
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
                    <!--<asp:Panel ID="PanelRicerca" style="margin-top: 0px;" runat="server" GroupingText="Ricerca per">
                        <asp:Label ID="lblRicerca" runat="server" Width="155px" Height="16px">Codice</asp:Label>
                            <asp:TextBox ID="txtRicerca" runat="server" AutoPostBack="false"
                               Width="635px" ></asp:TextBox>
                    </asp:Panel>-->
                     <asp:Panel ID="PanelOrdinamento" style="margin-top: 0px;" runat="server" groupingtext="Ordinamento">
                        <table style="width: 100%">
                        <tr>
                            <td width="33%" align="center">
                            <asp:RadioButton ID="rbtnCodice" runat="server" Text="Codice" 
                            AutoPostBack="false" GroupName="Ordinamento" TabIndex="3" />
                            </td>
                            <td width ="33%" align="center">
                            <asp:RadioButton ID="rbtnDescrizione" runat="server" Text="Descrizione" AutoPostBack="false" 
                            GroupName="Ordinamento" TabIndex="4" />
                            </td>
                            <td width="34%" align="center"> 
                                <asp:CheckBox ID="chkRagrForn" runat="server" AutoPostBack="false" 
                                TabIndex="5" Text="Raggruppa per fornitore" /> 
                            </td>
                        </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="Panel" runat="server" groupingtext="Fornitori" style="margin-top: 0px;" Height="78px" Width="847px">
                            <asp:Label ID="lblFornitore" runat="server" Width="160px" Height="17px">Fornitore</asp:Label>
                            <asp:Button ID="btnFornitore" runat="server" class="btnstyle" Width="25px" Height="25px" Text="?" ToolTip="Ricerca fornitore" Enabled="false" />
                            <asp:TextBox ID="txtCodFornitore" runat="server" Width="138px" MaxLength="16" AutoPostBack="True" TabIndex="6" Enabled="false" ></asp:TextBox>
                            <asp:TextBox ID="txtDescFornitore" runat="server" Width="440px" MaxLength="50" TabIndex="7" Enabled="False"  ></asp:TextBox><br>
                            <asp:CheckBox ID="chkTuttiFornitori" runat="server" AutoPostBack="True" TabIndex="8" Text="Seleziona tutti i fornitori" TextAlign="Left" Checked="true" />
                            <br>
                        </asp:Panel>
                    <asp:Panel ID="PanelSelezione" runat="server" groupingtext="Selezione" style="margin-top: 0px;" Width="847px"> 
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
                        <asp:Label ID="lblDal" runat="server" Width="137px" Height="17px">Dal codice articolo</asp:Label>
                           &nbsp; <asp:Button ID="btnCod1" runat="server" class="btnstyle" Width="30px" 
                                Height="22px" Visible="False" Text="?" />
                            &nbsp;&nbsp;<asp:TextBox ID="txtCod1" runat="server"  
                                Width="145px" MaxLength="20" AutoPostBack="false" TabIndex="10" ></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblAl" runat="server" Width="135px" Height="17px">Al codice articolo</asp:Label>&nbsp;
                            <asp:Button ID="btnCod2" runat="server" class="btnstyle" Height="22px" 
                            Visible="False" Width="30px" Text="?" />
                        &nbsp;&nbsp;
                            <asp:TextBox ID="txtCod2" runat="server"  
                                Width="145px" MaxLength="20" TabIndex="11" AutoPostBack="false" ></asp:TextBox>
                        <td>
                    </tr>
                    <tr>
                        <td>
                            <table style="width: 100%;">
                                <tr>                                   
                                    <td width="50%" align="left">
                                        <asp:CheckBox ID="chkArtGiacDivZero" runat="server" AutoPostBack="false" 
                                            TabIndex="12" Text="Stampa solo articoli con giacenza diversa da 0" /> 
                                    </td>
                                    <td width="50%" align="left">
                                        <asp:CheckBox ID="chkArtMovimentati" runat="server" AutoPostBack="false" 
                                             TabIndex="13" Text="Stampa solo movimentati" /> 
                                    </td>                                 
                                </tr>
                                <tr>                                   
                                    <td width="50%" align="left">
                                        <asp:CheckBox ID="chkArtInclSottoScorta" runat="server" AutoPostBack="false" 
                                            TabIndex="12" Text="Considera Sottoscorta per preparare gli ordini" /> 
                                    </td>
                                    <td width="50%" align="left">
                                        <asp:CheckBox ID="chkArtDaOrdinare" runat="server" AutoPostBack="false" 
                                            TabIndex="12" Text="Stampa solo articoli da ordinare" /> 
                                    </td>                                 
                                </tr>
                                <tr>
                                    <td width="50%" align="left">
                                        <asp:CheckBox ID="chkSoloNegativi" runat="server" AutoPostBack="false"
                                            TabIndex="13" text="Stampa solo articoli con giacenza NEGATIVA" />
                                    </td>
                                    <td width="50%" align="left">
                                        <asp:CheckBox ID="chkArtFuoriListino" runat="server" AutoPostBack="false"
                                            TabIndex="14" Text="Includi gli articoli esclusi da LISTINO BASE" />
                                    </td>
                                </tr>   
                                <tr>
                                    <td width="50%" align="left">
                                        <asp:CheckBox ID="chkSoloArtDB" runat="server" AutoPostBack="false"
                                            TabIndex="15" text="Stampa solo articoli Distinta Base" Visible="false" />
                                    </td>
                                    <td width="50%" align="left">
                                        <asp:CheckBox ID="chkIncludiArtDB" runat="server" AutoPostBack="false"
                                            TabIndex="16" Text="Includi articoli inclusi nella Distinta Base" Visible="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table style="width: 100%;">
                                            <asp:Label ID="lblMess" runat="server" Height="17px" Visible="False" 
                                                ForeColor="Red" Font-Bold="True"></asp:Label>
                                        </table>
                                    </td>
                                </tr>                                  
                            </table>
                        </td>
                    </tr>
                    </table>
                    </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                 <div id="noradio">
                                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" 
                                          TabIndex="14" /><%--OnClientClick="apristampa();"--%>
                                </div>
                                <div style="height: 15px">
                                </div>
                                <div>
                                    <a ID="LnkStampa" runat="server" href="..\WebFormTables\WebFormStampe.aspx" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Proforma" style="border-color:snow;border-style:outset;background-color:yellow;">Apri Proforma</a>
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
                                    <asp:Button ID="BtnPropRiord" runat="server" class="btnstyle" Text="Proposta di riordino" 
                                        TabIndex="15" />
                                </div>                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    </tr>
            </table>
    </ContentTemplate>
</asp:UpdatePanel>     
</asp:Panel>