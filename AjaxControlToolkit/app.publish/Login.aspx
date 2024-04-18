<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="SoftAziOnLine.Login" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%@ Register src="WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Gestione Aziendale - Login</title>
    <link href="App_Themes/Softlayout.css" rel="stylesheet" type="text/css"/>
    <link href="App_Themes/StyleSheet.css" rel="stylesheet" type="text/css"/>
    <link href="App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css"/>
    <link href="App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css"/>  
    <link href="App_Themes/gridheader.css" rel="stylesheet" type="text/css"/>
    <script src="JScript/jquery-1.4.4.min.js" type="text/javascript"></script>
<script language="javascript" type="text/javascript">
    function Loading() {
        window.history.forward(1)

        //Register Begin Request and End Request 
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        function BeginRequestHandler(sender, args) {
            document.body.style.cursor = 'wait';
        }
        function EndRequestHandler(sender, args) {
            document.body.style.cursor = 'default';
        }
    }
</script>
<style type="text/css">
    .btnstyle
    {
        Width: 108px;
        height: 40px;
    margin-left: 0px;
    }
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
<body onload="Loading();">
    <form id="form1" runat="server">
    <div>
        <uc1:WUC_ModalPopup ID="ModalPopup" runat="server" />
    </div>
    <table width="980" align="center" border="0" cellpadding="0" cellspacing="0" 
        style="background: #FFFFCC; border-style: solid;" class="float_right">
	<tr>
		<td width="848" style="background-color: #FFFF99; vertical-align: middle;">
			<img src="Immagini/soft_azienda.png" alt="" height="120" width="150"/>
			<asp:Label ID="lblintesta0" runat="server" Font-Bold="True" 
			Style="text-align: center; " Text="Soft Azienda OnLine"
                Width="237px" Height="34px" CssClass="style21"></asp:Label>
            &nbsp;
            <asp:Label ID="lblRelease" runat="server" Font-Bold="false"
            Style="text-align:justify; " Text="Release: 26 aprile 2012 17.00" Width="350px" Height="34px" CssClass="style21"></asp:Label>
            <img id="ImgAzienza" runat="server" src="Immagini/soft_azienda.png" alt="" height="80" width="220" visible="true"/>
        </td>
	</tr>
	<tr class="style21">
		<td valign="top" align="center" class="style24">
            <asp:Label ID="lblintesta" runat="server"
                Font-Bold="True" Font-Size="X-Large" Style="text-align:center" Text="Gestione Aziendale"
                Width="500px" CssClass="style23" Height="33px"></asp:Label>
            <asp:RoundedCornersExtender ID="lblintesta_RoundedCornersExtender" 
                runat="server"  Color="Black" Enabled="True" 
                TargetControlID="lblintesta" BorderColor="Black">
            </asp:RoundedCornersExtender>
            <br />
            <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
            </asp:ToolkitScriptManager>
                    <asp:Login ID="Login1" runat="server" BackColor="#999966" BorderColor="Black" 
                            BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="10pt" 
                            Height="159px" Width="500px" 
                            CreateUserText="Register"
                            CreateUserUrl="~/Register.aspx"
                            HelpPageText="Additional Help" HelpPageUrl="~/Help.aspx" 
                            InstructionText="Please enter your user name and password for login." 
                            onauthenticate="Login1_Authenticate" 
                            onloginerror="Login1_LoginError">
                        <TitleTextStyle BackColor="#6B696B" Font-Bold="True" ForeColor="#FFFFFF" />
                        <LayoutTemplate>
                            <table border="0" cellpadding="1" cellspacing="0" 
                                style="border-collapse:collapse;">
                                <tr>
                                    <td>
                                        <table border="0" cellpadding="0">
                                            <tr>
                                                <td align="center" colspan="2"
                                                    style="color:White;background-color:#6B696B;font-weight:bold;width:500px;">
                                                    Accedi</td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    &nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right"  class="style26">
                                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Nome utente:</asp:Label>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="UserName" runat="server" Width="150px"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                            ControlToValidate="UserName" ErrorMessage="Il nome utente è obbligatorio." 
                                            ToolTip="Il nome utente è obbligatorio." ValidationGroup="Login1">obbligatorio.</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    &nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right" class="style26">
                                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                                </td>
                                                <td align="left">
                                                    <opp:PasswordTextBox ID="Password" runat="server" Width="150px" 
                                            TextMode="Password"></opp:PasswordTextBox>
                                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                            ControlToValidate="Password" ErrorMessage="La password è obbligatoria." 
                                            ToolTip="La password è obbligatoria." ValidationGroup="Login1">obbligatoria.</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center" colspan="2" style="color:Red;">
                                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    &nbsp;</td>
                                            </tr>
                                            <tr>
                                                <td align="right"  class="style26">
                                                    Societ&agrave;:</td>
                                                <td align="left" style="height: auto" >
                                                    <asp:DropDownList ID="ddL_Ditte" runat="server" AutoPostBack="True" 
                                                        DataSourceID="SqlDataSource1" DataTextField="Descrizione" 
                                                        DataValueField="Codice" 
                                                        onselectedindexchanged="ddL_Ditte_SelectedIndexChanged" Width="370px">
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                                                        
                                                        
                                                        SelectCommand="SELECT [Codice], [Descrizione] FROM [Ditta] ORDER BY Descrizione">
                                                    </asp:SqlDataSource>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    &nbsp;</td>
                                            </tr>
                                            <caption>
                                                <br />
                                                <tr>
                                                    <td align="right" class="style27">
                                                        Esercizio:</td>
                                                    <td align="left" class="style25">
                                                        <div>
                                                            <asp:DropDownList ID="ddL_Esercizi" runat="server" Height="22px" Width="60px"  
                                                                DataSourceID="SqlDataSource2" DataTextField="Esercizio" 
                                                                DataValueField="Esercizio">
                                                            </asp:DropDownList>
                                                            <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                                                                SelectCommand="SELECT [Ditta], [Esercizio] FROM [Esercizi] WHERE ([Ditta] = @Ditta) ORDER BY Esercizio DESC">
                                                                <SelectParameters>
                                                                    <asp:SessionParameter DefaultValue="00" Name="Ditta" SessionField="CodiceDitta" Type="String" />
                                                                </SelectParameters>
                                                            </asp:SqlDataSource>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <%--<td align="right" colspan="2">
                                                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Accedi" 
                                                            ValidationGroup="Login1" Visible="false" Class="btnstyle" />
                                                    </td>--%>
                                                </tr>
                                            </caption>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </LayoutTemplate>
                    </asp:Login>
            <br />
            <asp:UpdatePanel ID="UpdatePanelLogin" runat="server">
                <ContentTemplate>
            <div></div>
            </ContentTemplate>
            </asp:UpdatePanel>
            <div>
                <asp:Button ID="LoginOK" runat="server" Text="Accedi" causesvalidation="true" ValidationGroup="Login1" Visible="true" Class="btnstyle"/>
            </div>
            <div>
                <asp:Label ID="MessageLabel" runat="server" ForeColor="#FF3300" Text=""></asp:Label>
            </div>
            <br />
            <div>
                <asp:Label ID="MessaggioInfo1" runat="server" ForeColor="#FF3300" Text="*******************************************************************************************" Visible="false"></asp:Label>
            </div>
            <div>
                <asp:Label ID="MessaggioInfo2" runat="server" ForeColor="#FF3300" Text="*******************************************************************************************" Visible="false"></asp:Label>
            </div>
            <div>
                <asp:Label ID="MessaggioInfo3" runat="server" ForeColor="#FF3300" Text="******* ATTENZIONE SIETE NELL'AMBIENTE DI PROVE E TEST DI FUNZIONAMENTO *******" Visible="false"></asp:Label>
            </div>
            <div>
                <asp:Label ID="MessaggioInfo4" runat="server" ForeColor="#FF3300" Text="*******************************************************************************************" Visible="false"></asp:Label>
            </div>
            <div>
                <asp:Label ID="MessaggioInfo5" runat="server" ForeColor="#FF3300" Text="*******************************************************************************************" Visible="false"></asp:Label>
            </div>
            <div id="updPrgPnlHeaderDefault1_master"  role="status" aria-hidden="true">
                <div id="PrgTmplPnlPaging11_master" class="boOverlay">
                    <div id="PrgTmplPnlPaging21_master" class="boLoaderCircle">
                                <div style="width:64px; height:64px;"></div>
                         </div>
                   </div>
            </div>
    <div></div>
    <br />
    </td>
	</tr>
	<tr>
		<td height="55" width="980px" style="background-color: #FFFF99">
		    <div align="center" class="style22">
                <span style="font-size: 10pt"><strong><span >Copyright © 2011-2024 Soft Solutions S.r.l.</span>
                </strong></span>
						</div>
			</td>
	</tr>
	</table>
    </form>
</body>
    <script type="text/javascript">
        var ismodal = 0;
        function InIEvent() {
            ismodal = 0;
            console.log('InIEvent');
            $('#updPrgPnlHeaderDefault1_master').hide();
            $('.modalPopup input[type="submit"]').click(function(e) {
                console.log('erere');
                $('#updPrgPnlHeaderDefault1_master').hide();
                ismodal = 1;

            });
            $('input[type="submit"]').click(function() {
                if (ismodal == 0) {
                    var x1 = document.getElementById('Login1_UserNameRequired');
                    var x2 = document.getElementById('Login1_PasswordRequired');
                    if (x1 != null) {
                        console.log('X1');
                        if (x1.style.visibility === 'hidden') {
                            if (x2.style.visibility === 'hidden') {
                                console.log('hidden X1');
                                $('#updPrgPnlHeaderDefault1_master').show();
                            } else {
                                console.log('NO hidden X1');
                                $('#updPrgPnlHeaderDefault1_master').hide();
                            }
                        } else {
                            console.log('NO hidden X1');
                            $('#updPrgPnlHeaderDefault1_master').hide();
                        }
                    }
                    if (x2 != null) {
                        console.log('X2');
                        if (x2.style.visibility === 'hidden') {
                            if (x1.style.visibility === 'hidden') {
                                console.log('hidden X2');
                                $('#updPrgPnlHeaderDefault1_master').show();
                            } else {
                                console.log('NO hidden X2');
                                $('#updPrgPnlHeaderDefault1_master').hide();
                            }
                        } else {
                            console.log('NO hidden X2');
                            $('#updPrgPnlHeaderDefault1_master').hide();
                        }
                    }
                }
                ismodal = 0;
            });
            ismodal = 0;
        }
        $(document).ready(InIEvent);
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(InIEvent);
    </script>
</html>