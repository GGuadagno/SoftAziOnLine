<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_GiacInizioAnno.ascx.vb" Inherits="SoftAziOnLine.WUC_GiacInizioAnno" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" BackColor="white">
    <asp:UpdatePanel ID="UpdatePanelElabora" runat="server">
        <ContentTemplate>
        <uc1:ModalPopup ID="ModalPopup" runat="server" />
            <br>
            <br>
            <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 20px; text-align:center; width: 980px;">
            <tr>
                <td>
                    <asp:Label ID="lblMagazzino" runat="server" Height="16px">Magazzino</asp:Label>
                    <asp:DropDownList ID="ddlMagazzino" runat="server" AppendDataBoundItems="true"
                           AutoPostBack="true" DataSourceID="SqlDataMagazzino" 
                           DataTextField="Descrizione" 
                           DataValueField="Codice" Width="545px" TabIndex="2">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDataMagazzino" runat="server"
                           SelectCommand="SELECT Codice, Descrizione FROM Magazzini WHERE Codice >0 ORDER BY Descrizione">
                    </asp:SqlDataSource>
                </td>
            </tr>
            </table> 
            <br>
            <br>
            <br>
            <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 260px; text-align:center" width="980px" >
                <tr>
                    <td>
                        Cliccando sul tasto OK verr&agrave; inserito un movimento di magazzino per trasferire le giacenze di inizio anno dall' esercizio precedente<br />
                        Il prezzo degli articoli verrà preso secondo il metodo FIFO.<br />
                    </td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblErrore" runat="server" ForeColor="Red" ></asp:Label></td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:Button ID="btnOK" runat="server" Text="OK Elabora" class="btnstyle"/>
                        <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla"/>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>   
</asp:Panel>