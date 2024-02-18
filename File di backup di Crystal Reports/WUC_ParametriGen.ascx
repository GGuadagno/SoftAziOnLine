<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_ParametriGen.ascx.vb" Inherits="SoftAziOnLine.WUC_ParametriGen" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>   
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<%@ Register Src="~/WebUserControl/WFP_ElencoCatCli.ascx" TagName="WFPElencoCatCli" TagPrefix="wuc" %>

<style type="text/css">
    .btnstyle
    {
        Width: 108px;
        height: 35px;
        white-space: pre-wrap;      
    }
    .styleTDBTN
    {
        height: 500px;
    }
    .styleLblTB0
    {
        height: 25px;
        }
     .styleTxtCodTB0
    {
        height: 25px;
        }  
    .styleTBPagina
    {
        height: 500px;
        width: 850px;
    }
    .styleDest
    {
        height: 60px;
    }
    .styleLblDDl
    {
        height: 25px;
        width: 700px;
    }
    .style2
    {
        width: 247px;
    }
    .style3
    {
        width: 100px;
    }
    .style4
    {
        width: 100px;
    }
    .style9
    {
        width: 100px;
    }
    .style10
    {
        width: 200px;
    }
</style>
<br />
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" Width="980px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <wuc:WFPElencoCatCli ID="WFPElencoCategorie" runat="server" Tabella="Categorie" Titolo="Elenco Categorie"/>
        <asp:SqlDataSource ID="SqlDSCausMag" runat="server" 
            SelectCommand="SELECT * FROM [CausMag] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSCausaliCoGe" runat="server" 
            SelectCommand="SELECT [Codice], [Descrizione] FROM [Causali] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSourceAliquotaIva" runat="server"
            SelectCommand="SELECT Aliquota, Descrizione FROM Aliquote_Iva ORDER BY Descrizione">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDSVettori" runat="server" 
            SelectCommand="SELECT * FROM [Vettori] ORDER BY [Descrizione]">
        </asp:SqlDataSource>
        <table style="width:auto; height:auto;">
            <tr>
                <td class="styleTBPagina">
                    <asp:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" 
                        BorderStyle="Groove" Height="500px" style="margin-top: 0px" 
                        Width="850px">
                        <asp:TabPanel ID="Pan1" runat="server" HeaderText="Parametri documenti">
                            <HeaderTemplate>Parametri documenti</HeaderTemplate>
                            <ContentTemplate>
                                <asp:Panel ID="PanelParStampe" runat="server" BackColor="Silver" Height="500px" 
                                    Width="830px">
                                <table bgcolor="Silver" width="100%">
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="LblPorto" runat="server">Dicitura predefinita porto</asp:Label>
                                        </td>
                                        <td class="style2">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:RadioButton ID="rbtnFranco" runat="server" AutoPostBack="false" 
                                                            GroupName="Porto" Text="Franco" />
                                                    </td>
                                                    <td>
                                                        <asp:RadioButton ID="rbtnAssegnato" runat="server" AutoPostBack="false" 
                                                            GroupName="Porto" Text="Assegnato" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="lblAspettoEst" runat="server">Aspetto esteriore dei beni</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtAspettoEst" runat="server" MaxLength="50" TabIndex="3" 
                                                Width="350px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="lblConai" runat="server">Descrizione CONAI</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TxtConai" runat="server" MaxLength="50" TabIndex="3" 
                                                Width="350px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label69" runat="server">Vettore per SpedDDT</asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DDLVettore1" runat="server" AppendDataBoundItems="True" 
                                                AutoPostBack="false" DataSourceID="SqlDSVettori" DataTextField="Descrizione" 
                                                DataValueField="Codice" Height="22px" TabIndex="7" Width="160px">
                                            <asp:ListItem Text="" Value="" ></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label ID="Label70" runat="server">Nessun Vettore disabilita la funzione Spedizione file XLS/Csv</asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label72" runat="server">Account per SpedDDT</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtAccountSpedDDT" runat="server" MaxLength="50" TabIndex="3" Width="350px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label71" runat="server">Email per SpedDDT</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEmailSpedDDT" runat="server" MaxLength="50" TabIndex="3" Width="350px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label73" runat="server">Telefono SpedDDT</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtTelSpedDDT" runat="server" MaxLength="50" TabIndex="3" Width="350px"></asp:TextBox>
                                            <asp:Label ID="Label74" runat="server">Prefisso Italia (39) già presente</asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label75" runat="server">Tipo Pag. Contrassegno</asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCPagCA" runat="server" MaxLength="50" TabIndex="3" Width="350px" ToolTip="(X): Y: CASH, POS - Q: ASSEGNO BANCARIO O POSTALE INTESTATO AL MITTENTE"></asp:TextBox>
                                            <asp:Label ID="Label76" runat="server">es.: ,152,Y,108,Q, (,CPag,X,)</asp:Label>
                                        </td>
                                    </tr>
                                   <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label2" runat="server">Password per i movimenti</asp:Label>
                                        </td>
                                        <td class="style2">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="TxtPassword" runat="server" MaxLength="10" TabIndex="3" 
                                                            Width="150px"></asp:TextBox>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="Label4" runat="server" Width="200px">Conferma password</asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TxtPasswordConferma" runat="server" MaxLength="10" 
                                                            TabIndex="3" Width="150px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label3" runat="server">Tipo di fatturazione</asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DDLTipoFatturazione" runat="server" 
                                                AppendDataBoundItems="True" AutoPostBack="false" DataSourceID="SqlDSTipoFatt" 
                                                DataTextField="Descrizione" DataValueField="Codice" Height="22px" TabIndex="11" 
                                                Width="350px"><asp:ListItem Text=" " Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDSTipoFatt" runat="server" 
                                                SelectCommand="SELECT * FROM [TipoFatt] ORDER BY [Descrizione]">
                                            </asp:SqlDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label65" runat="server">Durata Contratto</asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="DDLDurataTipo" runat="server" 
                                                AppendDataBoundItems="True" AutoPostBack="false" DataSourceID="SqlDSDurataTipo" 
                                                DataTextField="Descrizione" DataValueField="Codice" Height="22px" TabIndex="11" 
                                                Width="350px"><asp:ListItem Text=" " Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDSDurataTipo" runat="server" 
                                                SelectCommand="SELECT * FROM [DurataTipoFatt] ORDER BY [Descrizione]">
                                            </asp:SqlDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label5" runat="server"></asp:Label>
                                        </td>
                                        <td class="style2">
                                            <asp:Panel ID="Panel1" runat="server" 
                                                                        GroupingText="Calcolo sconto nell'importo riga dei dettagli">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:RadioButton ID="rbtnScontoPrezzo" runat="server" AutoPostBack="false" 
                                                            GroupName="CalcoloSconti" Text="Calcolo dello sconto sul prezzo unitario" 
                                                            Width="300px" />
                                                    </td>
                                                    <td>
                                                        <asp:RadioButton ID="rbtnScontoImporto" runat="server" AutoPostBack="false" 
                                                            GroupName="CalcoloSconti" Text="Calcolo dello sconto sul totale importo" Width="300px" />
                                                    </td>
                                                </tr>
                                            </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style2">
                                            <asp:Label ID="Label46" runat="server"></asp:Label>
                                        </td>
                                        <td class="style2">
                                            <asp:Panel ID="Panel2" runat="server" 
                                                                        GroupingText="Calcolo sconto cassa">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:RadioButton ID="rbtnScCassaTotRiga" runat="server" AutoPostBack="false" 
                                                            GroupName="CalcoloScCassa" Text="Calcolo dello sconto sul prezzo unitario" 
                                                            Width="300px" />
                                                    </td>
                                                    <td>
                                                        <asp:RadioButton ID="rbtnScCassaTotDoc" runat="server" AutoPostBack="false" 
                                                            GroupName="CalcoloScCassa" Text="Calcolo dello sconto sul totale importo" Width="300px" />
                                                    </td>
                                                </tr>
                                            </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:TabPanel>
                        <asp:TabPanel ID="TabPanelNDoc" runat="server" HeaderText="N° Documenti / Banca" >
                            <ContentTemplate>
                                <asp:Panel ID="PanelNDocVarie" runat="server" BackColor="Silver" Height="500px" 
                                    Width="830px">
                                <table bgcolor="Silver" width="100%">
                                    <tr>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td style="width:40%">
                                                        <table>
                                                            <%--<tr>
                                                                <td colspan="2">
                                                                    <table>
                                                                        <tr>
                                                                            <td align="right">
                                                                                <asp:CheckBox ID="chkNCorrNumSep" runat="server" Text="Note di corrispondenza con numerazione separata" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>--%>
                                                            <tr>
                                                                <td colspan="2">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:CheckBox ID="chkNCNumSep" runat="server" Text="Note di credito numerazione separata" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:CheckBox ID="chkNCPASep" runat="server" Text="Note di credito PA numerazione separata" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label7" runat="server">Ultima fattura</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltFC" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label19" runat="server">Ultima fattura accompagnatoria</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltFattAcc" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label14" runat="server">Ultima fattura PA</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltFattPA" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label9" runat="server">Ultima nota di credito</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltNC" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label20" runat="server">Ultima nota di credito PA</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltNCPA" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label8" runat="server">Ultimo ordine a cliente</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltOC" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label10" runat="server">Ultimo ordine a fornitore</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltOF" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label11" runat="server">Ultimo riordino a fornitore</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltRF" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label12" runat="server">Ultimo preventivo</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltPrev" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label13" runat="server">Ultima spedizione</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltSped" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="style2">
                                                                    <asp:Label ID="Label6" runat="server">Ultimo DDT a cliente</asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtUltDDT" runat="server" MaxLength="10" TabIndex="3" 
                                                                        Width="100px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width:60%">
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <table align="right">
                                                                        <tr>
                                                                            <td align="right">
                                                                                <asp:Label ID="label50" runat="server" Text="Decimali % di sconto"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="ddlDecPercSc" runat="server"><asp:ListItem Text="0" Value="0"></asp:ListItem><asp:ListItem 
                                                                                        Text="1" Value="1"></asp:ListItem><asp:ListItem Text="2" Value="2"></asp:ListItem><asp:ListItem 
                                                                                        Text="3" Value="3"></asp:ListItem><asp:ListItem Text="4" Value="4"></asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">
                                                                                <asp:Label ID="label15" runat="server" Text="Decimali % di provvigione"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="ddlDecPrecProvv" runat="server"><asp:ListItem Text="0" 
                                                                                        Value="0"></asp:ListItem><asp:ListItem Text="1" Value="1"></asp:ListItem><asp:ListItem 
                                                                                        Text="2" Value="2"></asp:ListItem><asp:ListItem Text="3" Value="3"></asp:ListItem><asp:ListItem 
                                                                                        Text="4" Value="4"></asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="right">
                                                                                <asp:Label ID="label16" runat="server" 
                                                                                    Text="Numero di sconti gestiti nei documenti"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="ddlNumScDoc" runat="server">
                                                                                        <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                                                                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                                                                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                                                                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                                                                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Panel ID="PanelBancaAppoggio" runat="server" 
                                                                        GroupingText="BANCA D'APPOGGIO"><table 
                                                                            width="100%"><tr><td><asp:Label ID="Label17" runat="server" Text="Banca"></asp:Label></td><td 
                                                                                    colspan="5"><asp:TextBox ID="txtBancaAppoggio" runat="server" Width="100%"></asp:TextBox></td></tr><tr><td><asp:Label 
                                                                                    ID="Label18" runat="server" Text="ABI"></asp:Label></td><td><asp:TextBox 
                                                                                        ID="txtABIBancaAppoggio" runat="server" MaxLength="5" Width="61px"></asp:TextBox></td><td><asp:Label 
                                                                                        ID="Label21" runat="server" Text="CAB"></asp:Label></td><td colspan="2"><asp:TextBox 
                                                                                        ID="txtCABBancaAppoggio" runat="server" MaxLength="5" Width="61px"></asp:TextBox></td></tr><tr><td><asp:Label 
                                                                                    ID="Label22" runat="server" Text="N° C/C"></asp:Label></td><td colspan="3"><asp:TextBox 
                                                                                        ID="txtNCCBancaAppoggio" runat="server"></asp:TextBox></td><td 
                                                                                    align="right"><asp:Label ID="Label23" runat="server" Text="CIN"></asp:Label></td><td 
                                                                                    colspan="2"><asp:TextBox ID="txtCINBancaAppoggio" runat="server" MaxLength="1" 
                                                                                        Width="41px"></asp:TextBox></td></tr></table></asp:Panel>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Panel ID="PanelCodici" runat="server" 
                                                                        GroupingText="Codici: IBAN/BBAN/SWIFT"><table 
                                                                            width="100%"><tr><td><asp:Label ID="Label24" runat="server" Text="Nazione"></asp:Label></td><td><asp:TextBox 
                                                                                    ID="txtNazIBAN" runat="server" MaxLength="2" Width="56px"></asp:TextBox></td><td><asp:Label 
                                                                                        ID="Label25" runat="server" Text="CIN Europeo"></asp:Label></td><td><asp:TextBox 
                                                                                        ID="txtCINEu" runat="server" MaxLength="2" Width="56px"></asp:TextBox></td></tr><tr><td><asp:Label 
                                                                                    ID="Label26" runat="server" Text="IBAN"></asp:Label></td><td colspan="3"><asp:TextBox 
                                                                                        ID="txtIBAN" runat="server" Enabled="False" Width="80%"></asp:TextBox></td></tr><tr><td><asp:Label 
                                                                                    ID="Label27" runat="server" Text="BBAN"></asp:Label></td><td colspan="3"><asp:TextBox 
                                                                                        ID="txtBBAN" runat="server" Enabled="False" Width="70%"></asp:TextBox></td></tr><tr><td><asp:Label 
                                                                                    ID="Label28" runat="server" Text="Codice SWIFT"></asp:Label></td><td 
                                                                                    colspan="3"><asp:TextBox ID="txtSwift" runat="server"></asp:TextBox></td></tr></table></asp:Panel>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                </asp:Panel>
                                </ContentTemplate>
                        </asp:TabPanel>
                        <asp:TabPanel ID="Pan3" runat="server" HeaderText="Trasf. Doc. in CoGe">
                            <ContentTemplate>
                                <asp:Panel ID="PanelTrasfCoGe" runat="server" BackColor="Silver" 
                                    GroupingText="Parametri per le registrazioni contabili dei documenti" 
                                    Height="500px" Width="830px">
                                    <table bgcolor="Silver" width="100%">
                                    <tr>
                                        <td class="style5">
                                            <asp:Label ID="Label29" runat="server" Text="Codice causale fatture"></asp:Label>
                                        </td>
                                        <td colspan="2">
                                            <asp:DropDownList ID="ddlCausFC" runat="server" AppendDataBoundItems="True" 
                                                DataSourceID="SqlDSCausaliCoGe" DataTextField="Descrizione" DataValueField="Codice" 
                                                Width="93%">
                                                <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style5">
                                            <asp:Label ID="Label30" runat="server" Text="Codice causale note credito"></asp:Label>
                                        </td>
                                        <td colspan="2">
                                            <asp:DropDownList ID="ddlCausNC" runat="server" AppendDataBoundItems="True" 
                                                DataSourceID="SqlDSCausaliCoGe" DataTextField="Descrizione" DataValueField="Codice" 
                                                Width="93%">
                                                <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style5">
                                            <asp:Label ID="Label31" runat="server" Text="Codice causale corrispettivi"></asp:Label>
                                        </td>
                                    <td colspan="2">
                                        <asp:DropDownList ID="ddlCausCO" runat="server" AppendDataBoundItems="True" 
                                            DataSourceID="SqlDSCausaliCoGe" DataTextField="Descrizione" DataValueField="Codice" 
                                            Width="93%">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label32" runat="server" Text="Codice causale incassi"></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:DropDownList ID="ddlCausIN" runat="server" AppendDataBoundItems="True" 
                                            DataSourceID="SqlDSCausaliCoGe" DataTextField="Descrizione" DataValueField="Codice" 
                                            Width="93%">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label33" runat="server" Text="Codice causale pagamento NC"></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:DropDownList ID="ddlCausPagNC" runat="server" AppendDataBoundItems="True" 
                                            DataSourceID="SqlDSCausaliCoGe" DataTextField="Descrizione" DataValueField="Codice" 
                                            Width="93%">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5" width="254px">
                                        <asp:Label ID="Label34" runat="server" Text="Codice causale RiBa"></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:DropDownList ID="ddlCausRiBa" runat="server" AppendDataBoundItems="True" 
                                            DataSourceID="SqlDSCausaliCoGe" DataTextField="Descrizione" DataValueField="Codice" 
                                            Width="93%">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label35" runat="server" Text="Conto cassa"></asp:Label>
                                    </td>
                                    <td width="100px" class="style3">
                                        <asp:TextBox ID="txtContoCassa" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoCassa" runat="server" Enabled="False" Width="90%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label36" runat="server" Text="Conto ricavo"></asp:Label>
                                    </td>
                                    <td class="style3">
                                        <asp:TextBox ID="txtContoRicavo" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoRicavo" runat="server" Enabled="False" Width="90%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label37" runat="server" Text="Conto corrispettivi"></asp:Label>
                                    </td>
                                    <td class="style3">
                                        <asp:TextBox ID="txtContoCorr" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoCorr" runat="server" Enabled="False" Width="90%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label38" runat="server" Text="Conto spese incasso"></asp:Label>
                                    </td>
                                    <td class="style3">
                                        <asp:TextBox ID="txtContoSpeseInc" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoSpeseInc" runat="server" Enabled="False" Width="90%"></asp:TextBox>
                                    </td>
                                    <td width="20px">
                                        <asp:Label ID="Label39" runat="server" Text="IVA"></asp:Label>
                                    </td>
                                    <td class="style4" width="150px">
                                        <asp:DropDownList ID="ddlAliqIVAInc" runat="server" AppendDataBoundItems="True"  
                                            DataSourceID="SqlDataSourceAliquotaIva" DataTextField="Descrizione" 
                                            DataValueField="Aliquota" Width="150px">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label40" runat="server" Text="Conto spese Bollo"></asp:Label>
                                    </td>
                                    <td class="style3">
                                        <asp:TextBox ID="txtContoSpeseVarie" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoSpeseVarie" runat="server" Enabled="False" 
                                            Width="90%"></asp:TextBox>
                                    </td>
                                     <td width="20px">
                                        <asp:Label ID="Label47" runat="server" Text="IVA"></asp:Label>
                                    </td>
                                    <td class="style4" width="150px">
                                        <asp:DropDownList ID="ddlAliqIVABollo" runat="server" AppendDataBoundItems="True"  
                                            DataSourceID="SqlDataSourceAliquotaIva" DataTextField="Descrizione" 
                                            DataValueField="Aliquota" Width="150px">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label48" runat="server" Text="Importo minimo per spese Bollo"></asp:Label>
                                    </td>
                                    <td class="style3">
                                        <asp:TextBox ID="txtImpMinBollo" runat="server" MaxLength="7"></asp:TextBox>
                                        <asp:TextBox ID="txtBollo" runat="server" MaxLength="7"></asp:TextBox>
                                    </td>
                                    <td class="style2" align="right">
                                        <asp:Label ID="Label51" runat="server" Text="Sconto Merce"></asp:Label>
                                    </td>
                                     <td width="20px">
                                        <asp:Label ID="Label56" runat="server" Text="IVA"></asp:Label>
                                    </td>
                                    <td class="style4" width="150px">
                                        <asp:DropDownList ID="ddlAliqIVAScMerce" runat="server" AppendDataBoundItems="True"  
                                            DataSourceID="SqlDataSourceAliquotaIva" DataTextField="Descrizione" 
                                            DataValueField="Aliquota" Width="150px">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label41" runat="server" Text="Conto spese imballo"></asp:Label>
                                    </td>
                                    <td class="style3">
                                        <asp:TextBox ID="txtContoSpeseImb" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoSpeseImb" runat="server" Enabled="False" Width="90%"></asp:TextBox>
                                    </td>
                                    <td width="20px">
                                        <asp:Label ID="Label42" runat="server" Text="IVA"></asp:Label>
                                    </td>
                                    <td class="style4" width="150px">
                                        <asp:DropDownList ID="ddlAliqIVAImb" runat="server" AppendDataBoundItems="True"  
                                            DataSourceID="SqlDataSourceAliquotaIva" DataTextField="Descrizione" 
                                            DataValueField="Aliquota" Width="150px">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label43" runat="server" Text="Conto spese trasporto"></asp:Label>
                                    </td>
                                    <td class="style3">
                                        <asp:TextBox ID="txtContoSpeseTrasp" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoSpeseTrasp" runat="server" Enabled="False" 
                                            Width="90%"></asp:TextBox>
                                    </td>
                                    <td width="20px">
                                        <asp:Label ID="Label44" runat="server" Text="IVA"></asp:Label>
                                    </td>
                                    <td class="style4" width="150px">
                                        <asp:DropDownList ID="ddlAliqVATrasp" runat="server" AppendDataBoundItems="True"   
                                            DataSourceID="SqlDataSourceAliquotaIva" DataTextField="Descrizione" 
                                            DataValueField="Aliquota" Width="150px">
                                            <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label45" runat="server" Text="Conto ricevute bancarie"></asp:Label>
                                    </td>
                                    <td class="style5">
                                        <asp:TextBox ID="txtContoRiBa" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoRiBa" runat="server" Enabled="False" Width="90%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style5">
                                        <asp:Label ID="Label57" runat="server" Text="Conto Erario C/Ritenute"></asp:Label>
                                    </td>
                                    <td class="style5">
                                        <asp:TextBox ID="txtContoRitAcconto" runat="server" MaxLength="16"></asp:TextBox>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="txtDContoRitAcconto" runat="server" Enabled="False" Width="90%"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:CheckBox ID="chkRegAutoInc" runat="server" Text="Registrazione automatica incassi" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <asp:CheckBox ID="chkRegRBPrimaNota" runat="server" Text="Registrazione RiBa in prima nota" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                            </ContentTemplate>
                        </asp:TabPanel>
                        <asp:TabPanel ID="Pan4" runat="server" HeaderText="Causali di magazzino e Tipo C.A.">
                            <ContentTemplate>
                                <asp:Panel ID="PanelMagazzinoCDCL" runat="server" BackColor="Silver" Height="500px" Width="830px">
                                <table bgcolor="Silver" width="100%">
                                    <tr>
                                        <td>
                                             <asp:Panel ID="PanelMagazzino" runat="server" GroupingText="Magazzino" >
                                                <table align="center" bgcolor="Silver" width="100%">
                                                    <%--<tr>
                                                        <td class="style9">
                                                            <asp:Label ID="Label6" runat="server" Text="Giorni lavorativi settimanali"></asp:Label>
                                                        </td>
                                                        <td class="style10">
                                                            <asp:TextBox ID="txtGiorniLavorativi" runat="server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style9">
                                                            <asp:Label ID="Label14" runat="server" Text="Giorno di riposo settimanale"></asp:Label>
                                                        </td>
                                                        <td class="style10">
                                                            <asp:DropDownList ID="ddlRiposoSett" runat="server">
                                                                <asp:ListItem Text="Lunedì" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="Martedì" Value="2"></asp:ListItem>
                                                                <asp:ListItem Text="Mercoledì" Value="3"></asp:ListItem>
                                                                <asp:ListItem Text="Giovedì" Value="4"></asp:ListItem>
                                                                <asp:ListItem Text="Venerdì" Value="5"></asp:ListItem>
                                                                <asp:ListItem Text="Sabato" Value="6"></asp:ListItem>
                                                                <asp:ListItem Text="Domenica" Value="7"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style9">
                                                            <asp:Label ID="Label48" runat="server" 
                                                                Text="N.ro di settimane da considerare nella creazione automatica ordini"></asp:Label>
                                                        </td>
                                                        <td class="style10">
                                                            <asp:TextBox ID="txtNSettOrd" runat="server"></asp:TextBox>
                                                        </td>
                                                    </tr>--%>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label49" runat="server" Text="Causale ripresa saldi"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlCausRiprSaldi" runat="server" 
                                                                AppendDataBoundItems="true" DataSourceID="SqlDSCausMag" 
                                                                DataTextField="Descrizione" DataValueField="Codice" Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label62" runat="server" Text="Causale riordino a fornitori"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLCausRiordino" runat="server" 
                                                                AppendDataBoundItems="true" DataSourceID="SqlDSCausMag" 
                                                                DataTextField="Descrizione" DataValueField="Codice" Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label59" runat="server" Text="Causale Movimenti Positivi"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLCausMMPos" runat="server" 
                                                                AppendDataBoundItems="true" DataSourceID="SqlDSCausMag" 
                                                                DataTextField="Descrizione" DataValueField="Codice" Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label61" runat="server" Text="Causale Movimenti Negativi"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLCausMMNeg" runat="server" 
                                                                AppendDataBoundItems="true" DataSourceID="SqlDSCausMag" 
                                                                DataTextField="Descrizione" DataValueField="Codice" Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <%--<tr>
                                                        <td class="style9">
                                                            <asp:Label ID="Label51" runat="server" 
                                                                Text="Anni predefiniti per la data fine produzione" Visible="false"></asp:Label>
                                                        </td>
                                                        <td class="style10">
                                                            <asp:TextBox ID="txtAnniFineProd" runat="server" Visible="false"></asp:TextBox>
                                                        </td>
                                                    </tr>--%>
                                                </table>
                                             </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                             <asp:Panel ID="Panel3" runat="server" GroupingText="Parametri documenti C/Vendita - Resi">
                                                <table align="center" bgcolor="Silver" width="100%">
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label58" runat="server" Text="Causale Vendita"></asp:Label> 
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLCausVendita" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label60" runat="server" Text="Causale Resi"></asp:Label> 
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLCausResi" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                             </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                             <asp:Panel ID="Panel4" runat="server" GroupingText="Parametri Contratti">
                                                <table align="center" bgcolor="Silver" width="100%">
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label1" runat="server" Text="Causale Contratto C/Manutenzione DAE"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLCausCAMDAE" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label63" runat="server" Text="Causale Contratto C/Telecontrollo"></asp:Label> 
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLCausCATelC" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label64" runat="server" Text="Causale Contratto C/Locazione"></asp:Label> 
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="DDLCausCALoc" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                             </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                             <asp:Panel ID="PanelDocCdep" runat="server" GroupingText="Parametri documenti C/Deposito">
                                                <table align="center" bgcolor="Silver" width="100%">
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label52" runat="server" Text="Causale DDT a deposito"></asp:Label> 
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlDDTDep" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label53" runat="server" Text="Causale vendita da deposito"></asp:Label> 
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlVendDep" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label54" runat="server" Text="Causale reso da deposito"></asp:Label> 
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlResoDep" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="style1">
                                                            <asp:Label ID="Label55" runat="server" Text="Causale rimanenza iniziale"></asp:Label> 
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlRimIniz" runat="server" AppendDataBoundItems="true" 
                                                                DataSourceID="SqlDSCausMag" DataTextField="Descrizione" DataValueField="Codice" 
                                                                Width="90%">
                                                                    <asp:ListItem Text=" " Value="0"></asp:ListItem>
                                                                </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                             </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:TabPanel>                        
                        <asp:TabPanel ID="Pan5" runat="server" HeaderText="Consumabili:Scadenze/E-mail - Alert MENU">
                            <ContentTemplate>
                                <asp:Panel ID="PanelScadenzeEmail" runat="server" BackColor="Silver" Height="500px" Width="830px">
                                <table bgcolor="Silver" width="100%">
                                    <tr>
                                        <td>
                                            <asp:Panel ID="PanelScadenze" style="margin-top: 0px;" runat="server" GroupingText="Seleziona dati: Categoria Clienti">
                                                <table align="center" bgcolor="Silver" width="100%">
                                                    <tr>
                                                        <td align="left">Singola categoria</td>
                                                        <td>
                                                            <asp:DropDownList ID="ddlCatCli" runat="server" DataSourceID="SqlDa_CatCli" 
                                                                DataTextField="Descrizione" DataValueField="Codice" Width="450px" 
                                                                AppendDataBoundItems="true" Enabled="False" AutoPostBack="false">
                                                                <asp:ListItem Value="0" Text="Categoria non definita"></asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:SqlDataSource ID="SqlDa_CatCli" runat="server" 
                                                                SelectCommand="SELECT [Codice], [Descrizione] FROM [Categorie] WHERE ISNULL(Categorie.InvioMailSc,0) <> 0 ORDER BY Descrizione">
                                                            </asp:SqlDataSource>
                                                        </td>
                                                        <td align="right">
                                                            <asp:CheckBox ID="chkTutteCatCli" runat="server" Text="Seleziona tutte le categorie" 
                                                                AutoPostBack="true" Checked="True" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left"></td>
                                                        <td>
                                                            <asp:CheckBox ID="chkRaggrCatCli" runat="server" Text="Seleziona tutte le categorie per descrizione iniziale uguale" 
                                                                AutoPostBack="false" Checked="False" Enabled="false" />
                                                        </td>
                                                        <td align="right">
                                                            <asp:Button ID="btnSelCategorie" runat="server" Text="?" OnClick="btnSelCategorie_Click" />
                                                            <asp:CheckBox ID="chkSelCategorie" runat="server" Text="Selezione multipla categorie" 
                                                                AutoPostBack="True" Checked="False" Enabled="true" />
                                                        </td>
                                                     </tr>
                                                    <%--<tr>
                                                        <td align="left">Seleziona clienti:</td>
                                                        <td align="left">
                                                            <asp:RadioButton ID="rbtnCliConEmail" runat="server" ToolTip="Con E-mail" Text="Con E-mail" 
                                                                    AutoPostBack="True" GroupName="CliEmail"/>
                                                            <asp:Label ID="Label46" runat="server" Width="5px"></asp:Label>
                                                            <asp:RadioButton ID="rbtnCliSenzaMail" runat="server" ToolTip="Senza E-mail" Text="Senza E-mail" 
                                                                    AutoPostBack="true" GroupName="CliEmail"/>
                                                            <asp:Label ID="Label47" runat="server" Width="5px"></asp:Label>
                                                            <asp:RadioButton ID="rbtnCliTutti" runat="server" ToolTip="Tutti (con e senza E-mail)"
                                                                            Text="Tutti (con e senza E-mail)" AutoPostBack="True" Checked="True" GroupName="CliEmail"/>
                                                        </td>
                                                    </tr>--%>
                                               </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Panel ID="PanelScadenzeDaA" style="margin-top: 0px;" runat="server" GroupingText="Scadenze articoli">
                                               <table align="center" bgcolor="Silver" width="100%">
                                                    <tr>
                                                        <td align="left">Indicare il N° dei mesi da calcolare dal Mese Corrente:(0)</td>
                                                        <td align="left">Periodo: dal</td>
                                                        <td>
                                                            <asp:TextBox ID="txtDallaData" runat="server" TabIndex="1" Width="60px" MaxLength="7" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                        <td align="left">al</td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtAllaData" runat="server" TabIndex="2" Width="60px" MaxLength="7" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">  
                                                            <asp:CheckBox ID="chkSelScGa" runat="server" Text="Scadenza Garanzia" TabIndex="5" Checked="False" AutoPostBack="false" />
                                                            <asp:CheckBox ID="chkSelScEl" runat="server" Text="Scadenza Elettrodi" TabIndex="6" Checked="True" AutoPostBack="false" />
                                                            <asp:CheckBox ID="chkSelScBa" runat="server" Text="Scadenza Batteria" TabIndex="7" Checked="True" AutoPostBack="false" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Panel ID="PanelEmail" style="margin-top: 0px;" runat="server" GroupingText="Parametri invio E-mail">
                                                <table align="center" bgcolor="Silver" width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="chkAIServizioEmail" runat="server" Text="Attiva servizio" TabIndex="5" Checked="true" AutoPostBack="true" />
                                                        </td>
                                                        <td align="left">
                                                            <asp:Label ID="lblAIServizioEmail" runat="server">Invio E-mail Scadenze Prodotti consumabili - Riattiva servizio alle ore: </asp:Label>
                                                            <asp:TextBox ID="txtAIServizioEmailAttiva" runat="server" TabIndex="2" Width="60px" MaxLength="5" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">Server SMTP</td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtSMTPServer" runat="server" TabIndex="2" Width="220px" MaxLength="255" AutoPostBack="false" ></asp:TextBox>
                                                            <asp:Label ID="Label66" runat="server">Porta</asp:Label>
                                                            <asp:TextBox ID="txtSMTPPorta" runat="server" TabIndex="2" Width="60px" MaxLength="7" AutoPostBack="false" ></asp:TextBox>
                                                            <asp:Label ID="Label67" runat="server">Utente SMTP</asp:Label>
                                                            <asp:TextBox ID="txtSMTPUserName" runat="server" TabIndex="2" Width="227px" MaxLength="255" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">Password SMTP</td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtSMTPPassword" runat="server" TabIndex="2" Width="220px" MaxLength="255" AutoPostBack="false" ></asp:TextBox>
                                                            <asp:Label ID="Label68" runat="server">E-mail Mittente</asp:Label>
                                                            <asp:TextBox ID="txtSMTPMailSender" runat="server" TabIndex="2" Width="320px" MaxLength="255" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Panel ID="PanelAlertMenu" style="margin-top: 0px;" runat="server" GroupingText="Alert Menu principale">
                                               <table align="center" bgcolor="Silver" width="100%">
                                                    <tr>
                                                        <td align="left">N° giorni prima segnalazione in ROSSO Alert Ri.Ba. e trasferimento in CoGe:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtAlertRiBaDocTrInCG" runat="server" Width="50px" MaxLength="5" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">N° giorni prima segnalazione in ROSSO Alert ATTIVITA' C.A. in scadenza:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtAlertScAttCA" runat="server" Width="50px" MaxLength="5" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">N° giorni prima segnalazione in ROSSO Alert SCADENZE C.A. da fatturare:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtAlertScPagCA" runat="server" Width="50px" MaxLength="5" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">N° giorni prima segnalazione in ROSSO Alert SCADENZE CONTRATTI:</td>
                                                        <td>
                                                            <asp:TextBox ID="txtAlertScadCA" runat="server" Width="50px" MaxLength="5" AutoPostBack="false" ></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                                </asp:Panel>   
                            </ContentTemplate>
                        </asp:TabPanel>                        
                    </asp:TabContainer>
                </td>
                <td align="left" class="styleTDBTN">
                    <asp:UpdatePanel ID="UpdatePanelBTN" runat="server">
                        <ContentTemplate>
                            <div id="noradio">
                                <asp:Button ID="btnAggiorna" runat="server" class="btnstyle" Text="Aggiorna" />
                            </div>
                            <div style="height: 25px"></div>
                            <div>
                                <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>                
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>