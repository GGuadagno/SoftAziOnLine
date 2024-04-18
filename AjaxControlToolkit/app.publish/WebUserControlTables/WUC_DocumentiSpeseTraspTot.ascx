<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_DocumentiSpeseTraspTot.ascx.vb" Inherits="SoftAziOnLine.WUC_DocumentiSpeseTraspTot" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WFP_Vettori.ascx" tagname="WFP_Vettori" tagprefix="uc2" %>
<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<asp:SqlDataSource ID="SqlDSVettori" runat="server" 
                                    SelectCommand="SELECT * FROM [Vettori] ORDER BY [Descrizione]">
                                </asp:SqlDataSource>
  <div align="left" style="border:1 solid White;  width: 1100px; height:500px;">
        <uc1:ModalPopup ID="ModalPopup" runat="server"  />
        <uc2:WFP_Vettori ID="WFP_Vettori1" runat="server" />
        <asp:Panel ID="PanelScontiSpeseTot" runat="server" Height="500px" Width ="1090px" CssClass ="sfondopagine">
           <asp:Panel ID="PanelDettScSpTot" runat="server" style="border:2 solid #000000">
           <table width="100%" >                           
                <tr>
                    <td colspan="3">
                    <table width="100%">
                    <tr>
                    <td style="width:135px">
                    Totale merce                
                    </td>
                    <td>
                    <asp:Label ID="lblTotMerce" runat="server" BorderStyle="Outset" Width="100px" 
                            style="text-align:right" Font-Bold="True">0,00</asp:Label>
                    </td>
                    <td colspan ="3">
                    <div>
                        <asp:Label ID="Label10" runat="server" Text="Sconto cassa %" Width="130px"></asp:Label>
                        <asp:TextBox  ID="TxtScontoCassa" runat="server" Width="100px" 
                            style="text-align:right" MaxLength="10" AutoPostBack="true" BorderStyle="None"></asp:TextBox>
                        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" 
                                                        ControlToValidate="TxtScontoCassa" ErrorMessage="*" 
                                                        ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                    </div>
                    </td>
                    <td align="right" colspan="2">
                    Totale Lordo merce  
                    <asp:Label ID="lblTotLordoMerce" runat="server" BorderStyle="Outset" Width="100px" 
                            style="text-align:right" Font-Bold="True">0,00</asp:Label>
                    <asp:Label ID="Label22" runat="server" Visible="false">TLM.Listino</asp:Label>
                    <asp:Label ID="lblTotLordoMercePL" runat="server" BorderStyle="Outset" Width="80px" 
                            style="text-align:right" Font-Bold="True" Visible="false">0,00</asp:Label> 
                    <asp:Label ID="Label39" runat="server" Visible="true" Font-Bold="True">Deduzioni</asp:Label>
                        <asp:Label ID="lblTotDeduzioni" runat="server" BorderStyle="Outset" Width="80px" 
                            style="text-align:right" Font-Bold="True" Visible="true">0,00</asp:Label>        
                    </td>
                    </tr>
                    <tr>
                    <td style="width:135px">
                    Spese incasso                
                    </td>
                    <td>
                    <asp:TextBox  ID="TxtSpeseIncasso" runat="server" Width="100px" 
                            style="text-align:right" MaxLength="10" BorderStyle="None"></asp:TextBox>
                        <%-- <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" 
                                                        ControlToValidate="TxtSpeseIncasso" ErrorMessage="*" 
                                                        ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                    </td>
                    <td colspan ="3">
                    <div>
                        <asp:Label ID="Label9" runat="server" Text="Spese trasporto" Width="130px"></asp:Label>
                        <asp:TextBox  ID="TxtSpeseTrasporto" runat="server" Width="100px" 
                            style="text-align:right" MaxLength="10" BorderStyle="None"></asp:TextBox>
                        <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" 
                                                        ControlToValidate="TxtSpeseTrasporto" ErrorMessage="*" 
                                                        ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                    </div>
                    </td>
                    <td align="right" colspan="2">
                        <asp:Label ID="Label40" runat="server" Text="Spese Bollo"></asp:Label>
                        <asp:TextBox  ID="txtBollo" runat="server" Width="50px" 
                            style="text-align:right" MaxLength="7" BorderStyle="None"></asp:TextBox>
                            <asp:Label ID="Label37" runat="server" Text="a carico del:"></asp:Label>
                        <asp:DropDownList ID="DDLSpeseBollo" runat="server" 
                            AppendDataBoundItems="True" AutoPostBack="true" Height="22px" TabIndex="2" Width="70px">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Mittente" Value="M"></asp:ListItem>
                            <%--<asp:ListItem Text="Destinatario" Value="D"></asp:ListItem>--%>
                        </asp:DropDownList> 
                    </td>
                    </tr>
                    <tr>
                    <td style="width:135px">
                    Spese imballo                
                    </td>
                    <td>
                    <asp:TextBox  ID="TxtSpeseImballo" runat="server" Width="100px" 
                            style="text-align:right" MaxLength="10" BorderStyle="None"></asp:TextBox>
                    <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" 
                                                    ControlToValidate="TxtSpeseImballo" ErrorMessage="*" 
                                                    ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                    </td>
                    <td colspan ="4">
                    <div>
                        <asp:Label ID="Label8" runat="server" Text="Descrizione imballo" Width="130px"></asp:Label>
                        <asp:TextBox  ID="TxtDescrizioneImballo" runat="server" Width="330px" 
                            MaxLength="150" BorderStyle="None"></asp:TextBox>
                        <asp:Label ID="Label42" runat="server" Text="" Width="15px"></asp:Label>
                        <asp:CheckBox ID="chkTotSpeseAdd0" runat="server" Font-Bold="false" Text="Totale Spese sono state già addebitate." 
                                Checked="false" AutoPostBack="true" Enabled="false" />
                    </div>
                    </td>                    
                    </tr>                
                    </table>
                    </td>
                </tr>   
                <tr>
                    <td style="width:350px" class="styleBordato">
                        <table >
                        <tr>
                        <td style="width:135px">
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
                        <td style="width:135px">
                        Riepilogo imposte
                        </td>
                        <td>
                        <asp:Label ID="LblIVA1" runat="server" BorderStyle="Outset" Width="50px" 
                                style="text-align:center" Font-Bold="True" ></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImponibile1" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImposta1" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:135px">
                        &nbsp;
                        </td>
                        <td>
                        <asp:Label ID="LblIVA2" runat="server" BorderStyle="Outset" Width="50px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImponibile2" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImposta2" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:135px">
                        &nbsp;
                        </td>
                        <td>
                        <asp:Label ID="LblIVA3" runat="server" BorderStyle="Outset" Width="50px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImponibile3" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImposta3" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:135px">
                        &nbsp;
                        </td>
                        <td>
                        <asp:Label ID="LblIVA4" runat="server" BorderStyle="Outset" Width="50px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImponibile4" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblImposta4" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:135px">
                        &nbsp;
                        </td>
                        <td>
                        &nbsp;
                        </td>
                        <td>
                        <asp:Label ID="LblTotaleImpon" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="LblTotaleImposta" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:135px">
                        Totale documento
                        </td>
                        <td>
                        &nbsp;
                        </td>
                        <td>
                        <asp:Label ID="LblTotDocumento" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        <td>
                        <asp:CheckBox ID="ChkSplitIVA" runat="server" Font-Bold="true" Text="Split payment" 
                                Checked="false" AutoPostBack="true" Width="135px" Height="21px"/>
                        </td>
                        </tr>
                        <tr>
                        <td style="width:135px">
                        Impon.Rit.Acconto
                        </td>
                        <td align="center">
                        %
                        </td>
                        <td align="right">
                        Ritenuta d'Acconto
                        </td>
                        </tr>
                        <tr>
                        <td style="width:135px">
                        <asp:TextBox  ID="txtImponibileRA" runat="server" Width="130px" 
                            style="text-align:right" MaxLength="10" Enabled="false" AutoPostBack="true" BorderStyle="None"></asp:TextBox>
                        </td>
                        <td align="center">
                        <asp:TextBox  ID="txtPercRA" runat="server" Width="50px" 
                            style="text-align:center" MaxLength="5" Enabled="false" AutoPostBack="true" BorderStyle="None"></asp:TextBox>
                        </td>
                        <td>
                        <asp:Label ID="LblTotaleRA" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:CheckBox ID="chkRitAcconto" runat="server" Font-Bold="true" Text="Rit. Acconto" Checked="false" AutoPostBack="true" Width="135px"/>
                        </td>
                        </tr>
                        <tr><td colspan="4"></td></tr>
                        <tr>
                            <td colspan="3">
                                <asp:Label ID="Label36" runat="server" BorderStyle="None"  
                                style="text-align:left" Font-Bold="True" Text="Arrotondamenti sul Netto a pagare"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAbbuono" runat="server" Width="135px" style="text-align:right" 
                                        MaxLength="10" AutoPostBack="true" BorderStyle="None"></asp:TextBox>
                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" 
                                                                ControlToValidate="txtAbbuono" ErrorMessage="*" 
                                                                ValidationExpression="^[0-9]+(\,[0-9]{1,2})?$" />--%>
                            </td>
                        </tr>
                        </table>
                    </td>                
                    <td style="width:250px" class="styleBordato">
                        <table >
                        <tr>
                        <td>
                        &nbsp;
                        </td>
                        <td align="center">
                        Data scadenza
                        </td>
                        <td align="right">
                        Importo rata
                        </td>
                        </tr>
                        <tr>
                        <td>
                        1
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad1" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="lblImpRata1" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        2
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad2" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="lblImpRata2" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        3
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad3" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="lblImpRata3" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        4
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad4" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="lblImpRata4" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        5
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad5" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="lblImpRata5" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        6
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad6" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="lblImpRata6" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        7
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad7" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="lblImpRata7" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        8
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad8" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="lblImpRata8" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        </table>
                    </td>
                    <td style="width:250px" class="styleBordato">
                        <table >
                        <tr>
                        <td>
                        &nbsp;
                        </td>
                        <td align="center">
                        Data scadenza
                        </td>
                        <td align="right">
                        Importo rata
                        </td>
                        </tr>
                        <tr>
                        <td>
                        9
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad9" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="lblImpRata9" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        10
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad10" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="lblImpRata10" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        11
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad11" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="lblImpRata11" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        12
                        </td>
                        <td>
                        <asp:Label ID="lblDataScad12" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="lblImpRata12" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td colspan ="3">
                            &nbsp;
                        </td>
                        </tr>
                        <tr>
                        <td colspan ="3">
                            <asp:Label ID="Label7" runat="server" BorderStyle="Outset" Width="260px" 
                            style="text-align:center" Font-Bold="false"></asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td colspan ="2">
                        Totale rate
                        </td>
                        <td>
                        <asp:Label ID="LblTotaleRate" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                            <td colspan ="3">
                                <asp:Label ID="Label38" runat="server" BorderStyle="Outset" Width="260px" 
                                style="text-align:center" Font-Bold="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan ="3">
                                <asp:Label ID="Label2" runat="server" BorderStyle="None" Width="260px" 
                                style="text-align:center" Font-Bold="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan ="2" align="center">
                            <asp:Label ID="Label1" runat="server" BorderStyle="Outset" Width="120px" 
                                style="text-align:center" Font-Bold="True">Netto a pagare</asp:Label>
                            </td>
                            <td>
                            <asp:Label ID="LblTotNettoPagare" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                            </td>
                        </tr>
                        </table>
                    </td>
                </tr>     
                </table>
                </asp:Panel> 
            <asp:Panel ID="PanelTrasporto" runat="server" style="border:2 solid #000000">
            <table width="100%" >   
            <tr>
                <td><%-- style="width:800px">--%>
                    <table><%-- style="width: 808px">--%>
                      <tr>
                        <td style="width:135px">
                        Trasporto a mezzo
                        </td>
                        <td align="left" colspan ="2">
                            <asp:RadioButton ID="optMittente"  ValidationGroup="0" GroupName="GrpTrasporto" runat="server" Font-Bold="true" Text ="Mittente" AutoPostBack ="true"></asp:RadioButton>
                            <asp:RadioButton ID="optDestinatario" ValidationGroup="0" GroupName="GrpTrasporto" runat="server" Font-Bold="true" Text ="Destinatario" AutoPostBack ="true"></asp:RadioButton>
                            <asp:RadioButton ID="optVettore" ValidationGroup="0" GroupName="GrpTrasporto" runat="server" Font-Bold="true" Text ="Vettori 1/2" AutoPostBack ="true"></asp:RadioButton>
                            <asp:Button ID="btnGestVett1" runat="server" CommandName="btnGestVett" 
                                                Text="+" ToolTip="Gestione anagrafica Vettori" />
                            <asp:DropDownList ID="DDLVettore1" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" DataSourceID="SqlDSVettori" DataTextField="Descrizione" 
                                                DataValueField="Codice" Height="22px" TabIndex="7" Width="145px">
                                                <asp:ListItem Text="" Value="" ></asp:ListItem>
                                            </asp:DropDownList>
                            <asp:DropDownList ID="DDLVettore2" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="True" DataSourceID="SqlDSVettori" DataTextField="Descrizione" 
                                                DataValueField="Codice" Height="22px" TabIndex="7" Width="145px">
                                                <asp:ListItem Text="" Value="" ></asp:ListItem>
                                            </asp:DropDownList>
                        </td> 
                        <%--<td style="visibility:visible">
                        <asp:Label ID="Label21" runat="server" BorderStyle="Groove" Width="120px" 
                                style="text-align:center" Font-Bold="false">Totale rate</asp:Label>
                        </td>
                        <td style="visibility:visible">
                        <asp:Label ID="Label6" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>--%>
                     </tr>  
                     <tr>
                        <td style="width:135px">
                        Evaso
                        </td>
                        <td align="left">                        
                            <asp:RadioButton ID="optTipoEvSaldoSaldo" ValidationGroup="0" GroupName="GrpTipoEvSaldo" runat="server" Font-Bold="true" Text ="Saldo" AutoPostBack ="false"></asp:RadioButton>
                            <asp:RadioButton ID="optTipoEvSaldoParziale" ValidationGroup="0" GroupName="GrpTipoEvSaldo" runat="server" Font-Bold="true" Text ="Acconto" AutoPostBack ="false"></asp:RadioButton>   
                        </td>
                        <td align="left">
                            Porto
                            <asp:RadioButton ID="optFranco" ValidationGroup="1" GroupName="GrpPorto" runat="server" Width="100px" Font-Bold="true" Text ="Franco" AutoPostBack ="true" ></asp:RadioButton>
                            <asp:RadioButton ID="optAssegnato" ValidationGroup="1" GroupName="GrpPorto" runat="server" Width="100px" Font-Bold="true" Text ="Assegnato" AutoPostBack ="true"></asp:RadioButton>
                        </td>
                     </tr>
                    <tr>
                        <td style="width:135px">
                        Descrizione porto
                        </td>
                        <td colspan ="2">
                        <asp:TextBox  ID="TxtDescPorto" runat="server" Width="650px" MaxLength="150" BorderStyle="None"></asp:TextBox>
                        </td>
                    </tr> 
                    <tr>
                        <td style="width:135px">
                        Aspetto esteriore
                        </td>
                        <td colspan ="2">
                        <asp:TextBox  ID="TxtDescAspetto" runat="server" Width="650px" MaxLength="150" BorderStyle="None"></asp:TextBox>
                        </td>
                    </tr> 
                     <tr>
                        <td style="width:135px">
                        Numero colli
                        </td>
                        <td colspan ="2" align ="left">
                            <div>
                                <asp:TextBox  ID="TxtNColli" runat="server" Width="45px" style="text-align:right" 
                                        MaxLength="05" BorderStyle="None"></asp:TextBox>
                                <asp:Label ID="Label3" runat="server" style="text-align:right">+ pezzi</asp:Label>
                                <asp:TextBox  ID="TxtNPezzi" runat="server" Width="45px" style="text-align:right" 
                                        MaxLength="05" BorderStyle="None"></asp:TextBox>
                                <asp:Label ID="Label4" runat="server" style="text-align:right">Peso in Kg.</asp:Label>
                                <asp:TextBox  ID="TxtPesoKG" runat="server" Width="45px" style="text-align:right" 
                                        MaxLength="05" BorderStyle="None"></asp:TextBox>
                                <%--<asp:Label ID="Label24" runat="server" style="text-align:right" Width="5px"></asp:Label>--%>
                                <asp:Label ID="Label23" runat="server" style="text-align:right" Font-Bold="true">Inizio Trasporto:</asp:Label>
                                <asp:Label ID="Label25" runat="server" style="text-align:right">Data</asp:Label>
                                <asp:TextBox  ID="TxtDataIniTrasp" runat="server" Width="70px" MaxLength="10" Font-Bold="true" BorderStyle="None"></asp:TextBox>
                                <asp:ImageButton ID="imgBtnShowCalendar" runat="server" CausesValidation="False" 
                                                    ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                                    ToolTip="apri il calendario" />
                                                <asp:CalendarExtender ID="TxtDataIniTrasp_CalendarExtender" runat="server" 
                                                    Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendar" 
                                                    TargetControlID="TxtDataIniTrasp">
                                                </asp:CalendarExtender>
                                                <%--<asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                                    ControlToValidate="TxtDataIniTrasp" ErrorMessage="Data non valida" 
                                                    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                                <asp:Label ID="Label5" runat="server" style="text-align:right">Ora:</asp:Label>
                                <asp:TextBox ID="TxtOraIniTrasp" runat="server" Width="40px" MaxLength="5" Font-Bold="true" BorderStyle="None"></asp:TextBox>
                                <%--<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                                                    ControlToValidate="TxtOraIniTrasp" ErrorMessage="Orario non valido" 
                                                    ValidationExpression="^(([01]?\d)|(2[0-3]))(:|.([0-5]?\d))?$" />--%>
                            </div>
                        </td>                        
                    </tr>  
                    <tr>
                        <td style="width:135px">
                        Annotazioni
                        </td>
                        <td colspan ="2">
                        <asp:TextBox  ID="txtNoteRitiro" runat="server" Width="650px" TextMode="MultiLine" BorderStyle="None"></asp:TextBox>
                        </td>
                        <%--<td style="visibility:visible">
                        <asp:Label ID="Label24" runat="server" BorderStyle="Groove" Width="120px" 
                                style="text-align:center" Font-Bold="false">Totale rate</asp:Label>
                        </td>
                        <td style="visibility:visible">
                        <asp:Label ID="Label26" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>--%>
                    </tr>    
            </table>
        </td> 
                <td id="TDAlert2" style="width:250px;height:150px;visibility:hidden" class="styleBordato">
                    <table> 
                        <tr>
                        <td>
                        9
                        </td>
                        <td>
                        <asp:Label ID="Label6" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                        <asp:Label ID="Label21" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        10
                        </td>
                        <td>
                        <asp:Label ID="Label24" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="Label26" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        11
                        </td>
                        <td>
                        <asp:Label ID="Label27" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="Label28" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        12
                        </td>
                        <td>
                        <asp:Label ID="Label29" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="Label30" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td>
                        13
                        </td>
                        <td>
                        <asp:Label ID="Label31" runat="server" BorderStyle="Outset" Width="100px" 
                                style="text-align:center" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                       <asp:Label ID="Label33" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                        <tr>
                        <td colspan ="2">
                        Totale rate
                        </td>
                        <td>
                        <asp:Label ID="Label34" runat="server" BorderStyle="Outset" Width="135px" 
                                style="text-align:right" Font-Bold="True">0,00</asp:Label>
                        </td>
                        </tr>
                    </table>  
                </td>
        </tr> 
       </table> 
       </asp:Panel> 
        </asp:Panel>   
  </div>
