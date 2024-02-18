<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_RicollegaVisteCoGe.ascx.vb" Inherits="SoftAziOnLine.WUC_RicollegaVisteCoGe" %>
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
<uc1:ModalPopup ID="ModalPopup1" runat="server" />
<br />
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="495px" BackColor="white">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <br>
            <br>
            <br>
            <br>
            <br>
            <table style="vertical-align:middle; background-color:Silver; border-style:double; height: 260px; width: 927px; text-align:center" width: 980px; >
                <tr>
                    <td>Aggiornamento delle seguenti viste:</td>
                </tr>
                <tr>
                    <td>[Agenti]</td>
                </tr>
                <tr>
                    <td>[Clienti]</td>
                </tr>
                <tr>
                    <td>[CliFor]</td>
                </tr>
                <tr>
                    <td>[DestClienti]</td>
                </tr>
                <tr>
                    <td>[Fornitori]</td>
                </tr>
                <tr>
                    <td>[Pagamenti]</td>
                </tr>
                <tr>
                    <td>[Province]</td>
                </tr>
                <tr>
                    <td>[Regioni]</td>
                </tr>
                <tr>
                    <td>[Vettori]</td>
                </tr>
                <tr>
                    <td>[DocumentiD_ALLANNI] Documenti anno precedente per l'evasione ordini di esercizi diversi</td>
                </tr>
                <tr>
                    <td>[DocumentiD_AP] Documenti anno precedente per l'evasione ordini di esercizi diversi</td>
                </tr>
                <tr>
                    <td>[Categorie]</td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>    
</asp:Panel>