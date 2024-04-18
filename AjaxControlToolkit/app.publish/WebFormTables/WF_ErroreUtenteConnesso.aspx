<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WF_ErroreUtenteConnesso.aspx.vb" Inherits="SoftAziOnLine.WF_ErroreUtenteConnesso" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Gestione Aziendale - Errore</title>
    <link href="App_themes/StyleSheet.css" type="text/css" rel="Stylesheet" />
    <style type="text/css">

        .style21
        {
            font-weight: bold;
            font-size: medium;
        }
        .style22
        {
            font-weight: bold;
            font-size: medium;
            height: 20px;
        }
        .style23
        {
            font-weight: bold;
            font-size: large;
        }
        .style24
        {
            font-weight: bold;
            font-size: medium;
            height: 403px;
            width: 849px;
        }
        .style25
        {
            height: 24px;
        }
        .style26
        {
            width: 121px;
        }
        .style27
        {
            height: 24px;
            width: 121px;
        }
        </style>
</head>
<body>
    <form id="form1" runat="server">
    <table width="848" align="center" border="0" cellpadding="0" cellspacing="0" 
        style="background: #FFFFCC; border-style: solid;" class="float_right">
	
	<tr>
		<td colspan="2" width="848" 
            style="background-color: #FFFF99; vertical-align: middle;">
			<img src="../Immagini/soft_azienda.png" alt="" height="120" width="120"/><asp:Label 
                ID="lblintesta0" runat="server"
                Font-Bold="True" 
                Style="text-align: center; " Text="Soft Azienda OnLine"
                Width="237px" Height="34px" CssClass="style21"></asp:Label>
            &nbsp;</td>
        
	</tr>
	
	<tr class="style21">
		<td valign="top" align="center" class="style24">
        <div>
         <br />
          <br />
           <br />
        <asp:Label ID="MessageLabel" runat="server" ForeColor="#FF3300"></asp:Label>
            <br />
            <br />
            <br />
        <asp:LinkButton ID="LogOut" runat="server">Ritorna al Login</asp:LinkButton>
        </div>
    <br />
    </td>
	</tr>	
	<tr>
		<td colspan="2" height="48" width="848" style="background-color: #FFFF99">
		    <div align="center" class="style22">
                <span style="font-size: 10pt"><strong><span >Copyright © 2011-2020 Soft Solutions S.r.l.</span>
                </strong></span>
						</div>
			</td>
	</tr>
</table>
    </form>
</body>
</html>
