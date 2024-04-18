<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WF_PrintWebEtichette.aspx.vb" Inherits="SoftAziOnLine.WF_PrintWebEtichette" %>
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title title="Stampa" id="Titolo"></title>
<script language="javascript" type="text/javascript">
    // Per visualizzare pdf in una nuova scheda
    function SetTarget() {
        document.forms[0].target = "_blank";
        setTimeout(function () {
            document.forms[0].target = "";
        }, 1000);
    }
</script>
</head>
<body onload="javascript:window.history.forward(1);">
    <form id="form1" runat="server">
    <table>
    <tr>
    <td>
        &nbsp;<a onclick="SetTarget();" id="LnkStampa" href="javascript:__doPostBack('LnkStampaOK','');" style="border-color:snow;border-style:outset;background-color:yellow;width:300px;">STAMPA oppure SALVA PDF</a>
        &nbsp;&nbsp;<asp:LinkButton ID="LnkRitorno" runat="server" Text="Ritorno Menu precedente" Visible="true" BorderColor="Snow" Font-Bold="false" Style="text-align:center;width:200px" Font-Size="Medium" ForeColor="Black" Font-Names="Arial" BorderStyle="Outset" BackColor="Silver" Font-Underline="false" CausesValidation="False" OnClientClick="__doPostBack('LnkRitornoOK','');"></asp:LinkButton>
        &nbsp;<asp:Label ID="lblVuota" runat="server" Font-Bold="True" Font-Size="Large" 
                    ForeColor="Red" Text="*" Visible="false"></asp:Label>
    </td>
    </tr>
    </table>
    <div>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="True" Height="50px" ReportSourceID="CrystalReportSource1" 
            Width="350px" ClientIDMode="AutoID" HasCrystalLogo="False" EnableDrillDown="False" HasPrintButton="False" HasDrilldownTabs="False" HasDrillUpButton="False" EnableDatabaseLogonPrompt="False" />
        <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
            <Report FileName="..\Report\Report.rpt">
            </Report>
        </CR:CrystalReportSource>
    </div>
    </form>
</body>
</html>
