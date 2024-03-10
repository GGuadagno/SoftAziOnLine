<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WF_PrintWebDispMag.aspx.vb" Inherits="SoftAziOnLine.WF_PrintWebDispMag" %>
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title title="Stampa" id="Titolo"></title>
</head>
<body onload="javascript:window.history.forward(1);">
    <form id="form1" runat="server">
    <table>
    <tr>
    <td>
        <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" title="Ok Stampa">Ok Stampa</a>&nbsp;&nbsp
        <asp:LinkButton ID="LinkStampa" runat="server" Visible="false">Visualizza Stampa</asp:LinkButton>&nbsp;&nbsp  
        <asp:Button ID="btnRitorno" runat="server" class="btnstyle" 
                                    Text="Ritorno Menu precedente" Visible="True"/>&nbsp;&nbsp
       <%-- <asp:Label ID="lblMess" runat="server" Font-Bold="True" Font-Size="Large" 
            ForeColor="Red" Text="Chiudere eventuali stampe create in precedenza"></asp:Label>--%>
    </td>
    </tr>
    </table>
    <div>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="True" Height="50px" ReportSourceID="CrystalReportSource1" 
            Width="350px" />
        <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
            <Report FileName="..\Report\Report.rpt">
            </Report>
        </CR:CrystalReportSource>
    </div>
    </form>
</body>
</html>
