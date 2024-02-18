<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_InserimentoFornitoreSec.ascx.vb" Inherits="SoftAziOnLine.WUC_InserimentoFornitoreSec" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
<table style="width:auto;height:auto;">
    <tr>
        <td >
            <div id="div1" style="border-style:ridge;border-width:thin">
            <table>
                <asp:SqlDataSource ID="SqlDataSourceFornitore" runat="server" SelectCommand="SELECT Codice_CoGe, Rag_Soc, Titolare, Riferimento FROM Fornitori ORDER BY Rag_Soc" />
                <tr>
                    <td align="left">Fornitore</td>
                    <td colspan="2">
                        <asp:TextBox ID="txtCodFornitore" AutoPostBack="true" runat="server" 
                            Width="100px" MaxLength="16"></asp:TextBox>&nbsp
                        <asp:DropDownList ID="ddlFornitore" AutoPostBack="true" DataTextField="Rag_Soc" DataValueField="Codice_CoGe" DataSourceID="SqlDataSourceFornitore" runat="server" Width="420px" AppendDataBoundItems="true">
                            <asp:ListItem Value="" Text=""></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td align="left">Titolare</td>
                    <td colspan="2"><asp:Label ID="lbTitolare" runat="server" Width="530px" Enabled="false" BorderStyle="Outset"/></td>
                </tr>
                <tr>
                    <td align="left">Riferimento</td>
                    <td colspan="2"><asp:Label ID="lbRiferimento" runat="server" Width="530px" Enabled="false" BorderStyle="Outset"/></td>
                </tr>
                <tr>
                    <td align="left">Condizioni pagamento</td>
                    <td colspan="2">
                        <asp:TextBox ID="txtCodCondizioniPag" AutoPostBack="true" runat="server" 
                            Width="100px" MaxLength="5"></asp:TextBox>&nbsp
                        <asp:DropDownList ID="ddlCondizioniPag" AutoPostBack="true" DataTextField="Descrizione" DataValueField="Codice" DataSourceID="SqlDataSourceCondizioniPag" runat="server" Width="420px" AppendDataBoundItems="true">
                            <asp:ListItem Value="" Text=""></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <asp:SqlDataSource ID="SqlDataSourceCondizioniPag" runat="server" SelectCommand="SELECT Codice, Descrizione FROM Pagamenti ORDER BY Descrizione" />
                </tr>
                <tr>
                    <td align="left">Giorni di consegna</td>
                    <td colspan="2" align="left">
                        <asp:TextBox ID="txtGiorniConsegna" AutoPostBack="true" runat="server" 
                            Width="100px" MaxLength="5" />&nbsp&nbsp
                        Prezzo&nbsp<asp:TextBox ID="txtPrezzo" AutoPostBack="true" runat="server" 
                            Width="100px" MaxLength="10" />
                    </td>
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>
    