<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WFP_LottiInsert.ascx.vb" Inherits="SoftAziOnLine.WFP_LottiInsert" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<%@ Register Src="~/WebUserControlTables/WUC_LottiInsert.ascx" TagName="WUCLottiInsert" TagPrefix="wuc" %>

<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />

<style type="text/css">
    .modalBackground {
        background-color:Gray;
        filter:alpha(opacity=70);
        opacity:0.7;
    }

    .modalPopup {
        background-color:#ffffdd;
        border-width:3px;
        border-style:solid;
        border-color:Gray;
        padding:3px;
        width:250px;
    }
</style>

<ajaxToolkit:ModalPopupExtender runat="server" ID="ProgrammaticModalPopup"
    TargetControlID="LinkButton1"
    PopupControlID="programmaticPopup" 
    BackgroundCssClass="modalBackground"
    DropShadow="true"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>
<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:left;">
        <asp:Label ID="Label1" 
                runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="CARICO NUMERO DI SERIE LOTTI" Width="99%"></asp:Label>               
        <asp:Panel ID="Panel1" runat="server" Height="125px" Width ="1240px">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ><ContentTemplate>
                        <div id="div2" style="overflow: auto; height:120px; border-style:groove; background-color: Silver;">
                          <div>
                              <asp:Label ID="Label20" runat="server" Width="875px"></asp:Label>
                              <asp:Label ID="Label21" runat="server" Text="Metodo di Lettura selezionato: " ForeColor="Blue"></asp:Label>
                          </div>
                          <div>
                              <asp:Label ID="Label4" runat="server" Width="5px"></asp:Label> 
                              <asp:Label ID="lblCodice" runat="server" Width="130px">Codice</asp:Label> 
                              <asp:Label ID="lblDescrizione" runat="server" Width="475px">Descrizione</asp:Label> 
                              <asp:Label ID="Label7" runat="server" Width="25px">UM</asp:Label>
                              <asp:Label ID="lblQta" runat="server" Width="55px">Ordinata</asp:Label> 
                              <asp:Label ID="lblQtaEv" runat="server" Width="50px">Evasa</asp:Label>
                              <asp:Label ID="lblLabelQtaRe" runat="server" Width="45px">Residua</asp:Label> 
                              <asp:Label ID="Label8" runat="server" Width="53px"></asp:Label> 
                              <asp:RadioButton ID="rbtnNSerie" runat="server" Text="(1 Lettura) N° Serie" AutoPostBack="true" GroupName="TipoLettura" Checked="true" Font-Bold="true" ForeColor="Blue"/>
                          </div>
                          <div>
                              <asp:Label ID="Label6" runat="server" Width="5px"></asp:Label> 
                              <asp:TextBox ID="txtCodArtIns" runat="server" Width="125px" MaxLength="20" AutoPostBack="false" Font-Bold="true" ForeColor="Blue"></asp:TextBox>
                              <asp:TextBox ID="txtDesArtIns" runat="server" Width="465px" MaxLength="150" Font-Bold="true" ForeColor="Blue"></asp:TextBox>
                              <asp:TextBox ID="txtUMIns" runat="server" MaxLength="2" Width="25px" Font-Bold="true" ForeColor="Blue"></asp:TextBox>
                              <asp:TextBox ID="txtQtaIns" runat="server" MaxLength="5" Width="45px" Font-Bold="true" style="text-align:right;" ForeColor="Blue"></asp:TextBox>
                              <asp:TextBox ID="txtQtaEv" runat="server" MaxLength="5" Width="45px" Font-Bold="true" ForeColor="Blue" style="text-align:right;"></asp:TextBox>
                              <asp:Label ID="LblQtaRe" runat="server" BorderStyle="Outset" Width="50px" Font-Bold="True" ForeColor="Blue" style="text-align:right;"></asp:Label>
                              <asp:Label ID="Label11" runat="server" Width="40px"></asp:Label>
                              <asp:RadioButton ID="rbtnNLotto" runat="server" Text="(1 Lettura) N° Lotto" AutoPostBack="true" GroupName="TipoLettura" Checked="false" Font-Bold="false" ForeColor="Black"/>
                          </div>
                          <div>
                              <asp:Label ID="Label22" runat="server" Width="870px"></asp:Label>
                              <asp:RadioButton ID="rbtnNSerieLotto" runat="server" Text="(2 Letture) N° Serie - Lettura N° Lotto" AutoPostBack="true" GroupName="TipoLettura" Checked="false" Font-Bold="false" ForeColor="Black"/>
                          </div>
                          <div>
                              <asp:Label ID="Label17" runat="server" Width="15px"></asp:Label> 
                              <asp:Label ID="Label12" runat="server" Width="30px">IVA</asp:Label> 
                              <asp:Label ID="lblPrezzoAL" runat="server" Width="107px" Text="Prezzo listino"></asp:Label>
                              <asp:Label ID="Label10" runat="server" Width="50px">Sc. (1)</asp:Label> 
                              <asp:Label ID="Label5" runat="server" Width="105px">Prezzo Netto</asp:Label> 
                              <asp:Label ID="Label2" runat="server" Width="110px">Importo riga</asp:Label> 
                              <asp:Label ID="Label3" runat="server" Width="68px">Giacenza</asp:Label>
                              <asp:Label ID="Label9" runat="server" Width="75px">Giac.Imp.</asp:Label>
                              <asp:Label ID="Label15" runat="server" Width="90px">Ord.Forn.</asp:Label>
                              <asp:Label ID="Label16" runat="server" Width="120px">Data/Qtà arrivo</asp:Label>
                              <asp:Label ID="Label13" runat="server" Width="55px"></asp:Label>
                              <asp:Label ID="Label24" runat="server" Text="Selezionando una riga fra quelli scansionati" ForeColor="Blue" Visible="false"></asp:Label>
                          </div>
                          <div>
                              <asp:Label ID="Label18" runat="server" Width="5px"></asp:Label> 
                              <asp:TextBox ID="txtIVAIns" runat="server" Width="30px" MaxLength="20" AutoPostBack="true"></asp:TextBox>
                              <asp:TextBox ID="txtPrezzoIns" runat="server" Width="100px" MaxLength="15"></asp:TextBox>
                              <asp:TextBox ID="txtSconto1Ins" runat="server" MaxLength="5" Width="50px"></asp:TextBox>
                              <asp:Label ID="LblPrezzoNetto" runat="server" BorderStyle="Outset" Width="100px" Font-Bold="True"></asp:Label>
                              <asp:Label ID="LblImportoRiga" runat="server" BorderStyle="Outset" 
                                  Width="100px" Font-Bold="True"></asp:Label>
                              <asp:Label ID="lblGiacenza" runat="server" BorderStyle="Outset" 
                                  Width="65px" Font-Bold="False"></asp:Label>
                              <asp:Label ID="lblGiacImp" runat="server" BorderStyle="Outset" 
                                  Width="65px" Font-Bold="False"></asp:Label>
                              <asp:Label ID="lblOrdFor" runat="server" BorderStyle="Outset" 
                                  Width="65px" Font-Bold="False"></asp:Label>
                              <asp:Label ID="lblDataArr" runat="server" BorderStyle="Outset" 
                                  Width="145px" Font-Bold="False"></asp:Label>
                              <asp:Label ID="Label14" runat="server" Width="45px"></asp:Label>
                              <asp:Label ID="Label23" runat="server" Text="potrete modificare il N° Serie/N° Lotto" ForeColor="Blue" Visible="false"></asp:Label>
                              <asp:Label ID="Label19" runat="server" Text="Modifica Riga Sel.: " ForeColor="Black" Visible="false"></asp:Label>
                              <asp:RadioButton ID="rbtnModNSerie" runat="server" Text="N° Serie" AutoPostBack="true" GroupName="TipoLettura" Checked="false" Font-Bold="false" ForeColor="Black" Visible="false"/>
                              <asp:RadioButton ID="rbtnModNLotto" runat="server" Text="N° Lotto" AutoPostBack="true" GroupName="TipoLettura" Checked="false" Font-Bold="false" ForeColor="Black" Visible="false"/>
                          </div>
                        </div>
                        </ContentTemplate>
                        </asp:UpdatePanel>
            </asp:Panel>  
            <wuc:WUCLottiInsert ID="WUCLottiInsert" runat="server" />
    </asp:Panel>
    <asp:Panel ID="Panel3" runat="server" Height="25px"><asp:Label ID="lblMessUtente" 
                runat="server" BorderStyle="Outset" Font-Bold="True" Font-Overline="False" 
                Style="text-align:center" Text="Seleziona/modifica Qtà articoli da caricare" Width="99%"></asp:Label>
        </asp:Panel> 
    <br /><br />
    <div style="text-align:center;">    
        <asp:Button ID="btnOk" runat="server" Text="OK Carico lotti" OnClick="btnOk_Click" Height="35px" Width="120px" />
        <asp:Button ID="btnCancel" runat="server" Text="Annulla" OnClick="btnCancel_Click" Height="35px" Width="120px"/>
    </div>
   <%--<br />--%>
</asp:Panel>

