<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GenericErrorPage.aspx.vb" Inherits="SoftAziOnLine.GenericErrorPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    
    <title>Gestione Aziendale - Informazioni</title>
    
   <%-- <link href="App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />--%>
   
    <style type="text/css">

        .style21
        {
            font-family: Arial, Helvetica, sans-serif;
            font-weight: bold;
            font-size: medium;
        }
        .style22
        {
            font-family: Arial, Helvetica, sans-serif;
            font-weight: bold;
            font-size: medium;
            height: 20px;
        }
        .style23
        {
            font-family: Arial, Helvetica, sans-serif;
            font-weight: bold;
            font-size: large;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
     <asp:Label ID="lblintesta" runat="server"
                Font-Bold="True" Font-Size="X-Large" Style="text-align: center" Text="Errore sconosciuto"
                Width="865px" CssClass="style23" Height="38px"></asp:Label>
    </form>
</body>
</html>
