<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Tabelle.ascx.vb" Inherits="SoftAziOnLine.WUC_Tabelle" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc0" %>
<%@ Register src="../WebUserControl/WFP_Agenti.ascx" tagname="WFP_Agenti" tagprefix="uc1" %>
<%@ Register src="../WebUserControl/WFP_BancheIBAN.ascx" tagname="WFP_BancheIBAN" tagprefix="uc2" %>
<%@ Register src="../WebUserControl/WFP_CapiGruppo.ascx" tagname="WFP_CapiGruppo" tagprefix="uc9" %>
<%@ Register src="../WebUserControl/WFP_CategorieArt.ascx" tagname="WFP_CategorieArt" tagprefix="uc3" %>
<%@ Register src="../WebUserControl/WFP_Categorie.ascx" tagname="WFP_Categorie" tagprefix="uc4" %>
<%@ Register src="../WebUserControl/WFP_LineeArt.ascx" tagname="WFP_LineeArt" tagprefix="uc5" %>
<%@ Register src="../WebUserControl/WFP_TipoCodArt.ascx" tagname="WFP_TipoCodArt" tagprefix="uc16" %>
<%@ Register src="../WebUserControl/WFP_Misure.ascx" tagname="WFP_Misure" tagprefix="uc6" %>
<%@ Register src="../WebUserControl/WFP_Vettori.ascx" tagname="WFP_Vettori" tagprefix="uc7" %>
<%@ Register src="../WebUserControl/WFP_Zone.ascx" tagname="WFP_Zone" tagprefix="uc8" %>
<%@ Register src="../WebUserControl/WFP_Reparti.ascx" tagname="WFP_Reparti" tagprefix="uc10" %>
<%@ Register src="../WebUserControl/WFP_Scaffali.ascx" tagname="WFP_Scaffali" tagprefix="uc11" %>
<%@ Register src="../WebUserControl/WFP_TipoFatturazione.ascx" tagname="WFP_TipoFatturazione" tagprefix="uc12" %>
<%@ Register src="../WebUserControl/WFP_CausaliNonEvasione.ascx" tagname="WFP_CausaliNonEvasione" tagprefix="uc13" %>
<%@ Register src="../WebUserControl/WFP_TestiEmail.ascx" tagname="WFP_TestiEmail" tagprefix="uc14" %>
<%@ Register src="../WebUserControl/WFP_Moduli.ascx" tagname="WFP_Moduli" tagprefix="uc15" %>
<%@ Register src="../WebUserControl/WFP_RespArea.ascx" tagname="WFP_RespArea" tagprefix="uc16" %>
<%@ Register src="../WebUserControl/WFP_RespVisite.ascx" tagname="WFP_RespVisite" tagprefix="uc17" %>
<%@ Register src="../WebUserControl/WFP_Magazzini.ascx" tagname="WFP_Magazzini" tagprefix="uc18" %>
<%@ Register src="../WebUserControl/WFP_LeadSource.ascx" tagname="WFP_LeadSource" tagprefix="uc19" %>
<%@ Register src="../WebUserControl/WFP_Nazioni.ascx" tagname="WFP_Nazioni" tagprefix="uc20" %>
<style type="text/css">
        .style22
        {
            font-weight: bold;
            font-size: medium;
            height: 20px;
        }
        .style36
    {
        width: 480px;
        height: 26px;
    }
    .style37
    {
        width: 480px;
        height: 26px;
    }
        .style38
    {
    }
        .style42
    {
    }
    .style43
    {
        width: 124px;
        height: 26px;
    }
        .style44
    {
        width: 593px;
        height: 26px;
    }
        .style46
    {
        width: 384px;
        height: 26px;
    }
        .style47
    {
        width: 124px;
        height: 7px;
    }
    .style48
    {
        width: 480px;
        height: 7px;
    }
    .style49
    {
        width: 124px;
        height: 14px;
    }
    .style50
    {
        width: 480px;
        height: 14px;
    }
    .style51
    {
        width: 124px;
        height: 24px;
    }
    .style52
    {
        width: 480px;
        height: 24px;
    }
        </style>
<br />  
<asp:Panel ID="panelPrincipale" runat="server" BorderStyle="Double" 
    Width="1240px" Height="550px" BackColor="Silver">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>    
<uc0:ModalPopup ID="ModalPopup" runat="server" />
<uc1:WFP_Agenti ID="Agenti" runat="server" />
<uc2:WFP_BancheIBAN ID="BancheIBAN" runat="server" />  
<uc3:WFP_CategorieArt ID="CategorieArt" runat="server" />  
<uc4:WFP_Categorie ID="CategorieCli" runat="server" /> 
<uc5:WFP_LineeArt ID="LineeArt" runat="server" />
<uc16:WFP_TipoCodArt ID="TipoCodArt" runat="server" />
<uc6:WFP_Misure ID="Misure" runat="server" /> 
<uc7:WFP_Vettori ID="Vettori" runat="server" /> 
<uc8:WFP_Zone ID="Zone" runat="server" />
<uc9:WFP_CapiGruppo ID="CapiGruppo" runat="server" /> 
<uc10:WFP_Reparti ID="Reparti" runat="server" /> 
<uc11:WFP_Scaffali ID="Scaffali" runat="server" />
<uc12:WFP_TipoFatturazione ID="TipoFatturazione" runat="server" />
<uc13:WFP_CausaliNonEvasione ID="CausaliNonEvasione" runat="server" />
<uc14:WFP_TestiEmail ID="TestiEmail" runat="server" />
<uc15:WFP_Moduli ID="Moduli" runat="server" />
<uc16:WFP_RespArea ID="RespArea" runat="server" />
<uc17:WFP_RespVisite ID="RespVisite" runat="server" />
<uc18:WFP_Magazzini ID="Magazzini" runat="server" />
<uc19:WFP_LeadSource ID="LeadSource" runat="server" />
<uc20:WFP_Nazioni ID="Nazioni" runat="server" />
    <td class="style22" colspan="1">
    &nbsp;<asp:Label ID="lblDataOdierna" runat="server" BorderStyle="None" 
    Text="Oggi, Venerdì 2 settembre 2011" Width="475px" Font-Bold="True" 
    Font-Overline="False"></asp:Label>
    </td>
</ContentTemplate>
</asp:UpdatePanel>    
</asp:Panel>								