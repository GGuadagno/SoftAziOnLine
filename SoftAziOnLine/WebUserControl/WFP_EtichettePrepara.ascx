<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WFP_EtichettePrepara.ascx.vb" Inherits="SoftAziOnLine.WFP_EtichettePrepara" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register src="../WebUserControl/WUC_ModalPopup.ascx" tagname="ModalPopup" tagprefix="uc1" %>
<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />
<style type="text/css">
    .modalBackground {
        background-color:Gray;
    }

    .modalPopup {
        background-color:#ffffdd;
        border-width:3px;
        border-style:solid;
        border-color:Gray;
        padding:3px;
        width:250px;
    }
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
    .btnstyle70
    {
        Width: 108px;
        height: 70px;
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
<ajaxToolkit:ModalPopupExtender runat="server" ID="ProgrammaticModalPopup"
    TargetControlID="LinkButton1"
    PopupControlID="programmaticPopup" 
    BackgroundCssClass="modalBackground"
    DropShadow="True"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>

<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
<asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:center;">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup" runat="server" />
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 260px; width: 927px;">
 <tr>   
  <td>
    <asp:Panel ID="PanelSelezionaDoc" runat="server" GroupingText="Destinazione merce" style="margin-top: 0px;text-align:left;" Width="859px">
        <table width="100%">
            <tr>                            
                <td width="15%" align="right">
                    <asp:Label ID="LblDescDataDoc" runat="server" Height="17px" Text="Data documento"></asp:Label>
                </td>
                <td>
                    <table width="75%">
                    <tr>
                        <td align="center">
                            <asp:Label ID="LblDataDoc" runat="server" Width="150px" Height="17px" BorderStyle="Outset" Font-Bold="true"></asp:Label>
                        </td>                                        
                        <td align="right" >
                            <asp:Label ID="LblDescNumDoc" runat="server" Width="165px" Height="17px" Text="Numero documento"></asp:Label>
                        </td>
                        <td align="center">
                            <asp:Label ID="LblNumDoc" runat="server" Width="150px" Height="17px" BorderStyle="Outset" Font-Bold="true"></asp:Label>
                        </td>                                                                                           
                    </tr>                                    
                    </table> 
                 </td>                                                       
            </tr>                                               
            <tr>                            
                <td align="right"><asp:Label ID="LblDescDenominazione" runat="server" Width="165px" Height="17px" Text="Intestazione etichetta"></asp:Label></td>
                <td width="50%" align="left" ><asp:Label ID="LblRiga1" runat="server" Width="100%" Height="17px" BorderStyle="Outset" Font-Bold="true"></asp:Label></td>                                                       
            </tr>
            <tr>                            
                <td>&nbsp;</td>
                <td width="50%"  align="left" ><asp:Label ID="LblRiga2" runat="server" Width="100%" Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
            </tr>                       
            <tr>                            
                <td>&nbsp;</td>
                <td width="50%" align="left" ><asp:Label ID="LblRiga3" runat="server" Width="100%" Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
            </tr>                                               
            <tr>                            
                <td>&nbsp;</td>
                <td width="50%" align="left" ><asp:Label ID="LblRiga4" runat="server" Width="100%" Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
            </tr>   
            <tr>                            
                <td>&nbsp;</td>
                <td width="50%" align="left" ><asp:Label ID="LblRiga5" runat="server" Width="100%" Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
            </tr>  
            <tr>                            
                <td>&nbsp;</td>
                <td width="50%" align="left" ><asp:Label ID="LblRiga6" runat="server" Width="100%" Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
            </tr>  
            <tr>                            
                <td>&nbsp;</td>
                <td width="50%" align="left" ><asp:Label ID="LblRiga7" runat="server" Width="100%" Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
            </tr>                                                                                                 
            <tr>                            
                <td>&nbsp;</td>
                <td width="50%" align="left" ><asp:Label ID="LblRiga8" runat="server" Width="100%" Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
            </tr>                          
        </table>                        
    </asp:Panel>
                    
    <asp:Panel ID="Panel" runat="server" groupingtext="Quantità di etichette da stampare" style="margin-top: 0px;text-align:left;" Width="859px">
        <table width="100%">
            <tr>                            
                <td width="15%" align="right"><asp:Label ID="lblNColli" runat="server" Width="200px" Height="17px" Text="Quantità colli"></asp:Label></td>
                <td width="15%" align="left"><asp:TextBox ID="txtNColli" runat="server"  Width="80px" MaxLength="16" AutoPostBack="false" TabIndex="6" ></asp:TextBox></td>                                                       
                <td width="70%"><asp:Button ID="BtnEtichettaA4" runat="server" class="btnstyle70" Text="Etichetta A4 Stampa 1 per foglio" TabIndex="20" /></td>
            </tr>
        </table>
    </asp:Panel>
    
    <asp:Panel ID="PanelSelezionaOrdinamento" runat="server" groupingtext="Indicare l'etichetta di partenza" style="margin-top: 0px;text-align:left;" Width="859px">
        <table width="100%">
            <tr>
                <td width="25%">          </td>
                <td width="25%"><asp:Button ID="BtnEtichetta1" runat="server" class="btnstyle" Text="Etichetta 1" TabIndex="20" /></td>
                <td width="25%"><asp:Button ID="BtnEtichetta3" runat="server" class="btnstyle" Text="Etichetta 3" TabIndex="20" /></td>
                <td width="25%" rowspan ="2" align="center">orientamento orizzontale<br />
                    <img id="ImgOrient" runat="server" alt="Formato" src="../Immagini/Icone/Orizzontale.jpg" /> 
                </td>
            </tr>
            <tr>
                <td width="25%">          </td>
                <td width="25%"><asp:Button ID="BtnEtichetta2" runat="server" class="btnstyle" Text="Etichetta 2" TabIndex="20" /></td>
                <td width="25%"><asp:Button ID="BtnEtichetta4" runat="server" class="btnstyle" Text="Etichetta 4" TabIndex="20" /></td>
            </tr>                        
        </table>
    </asp:Panel>
  </td>
  <td align="left" class="style7">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                 <div>
                    <asp:Button ID="btnStampa" runat="server" class="btnstyle" Text="Stampa" TabIndex="20" />
                </div>
                <div style="height:15px">&nbsp;</div>
                    <div>
                        <a ID="LnkStampa" runat="server" href="#" target="_blank" onclick="return openUrl(this.href);" visible="false" title="Apri Documento">Apri Documento</a>
                    </div>
                <div style="height: 15px">
                </div>
                 <div>
                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" TabIndex="21" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
  </td>
                   
 </tr>              
</table>
   </ContentTemplate>
 </asp:UpdatePanel>
</asp:Panel>
<div id="BlkPagEtPrepara"  role="status" aria-hidden="true">
        <div id="BlkPag1" class="boOverlay">
            <div id="BlkPag2" class="boLoaderCircle">
                    <div style="width:64px; height:64px;"></div>
             </div>
        </div>
    </div>
<div>
    <asp:Label ID="lblErrore" runat="server" Width="100%" 
                        Height="17px" BorderStyle="Outset" ForeColor="Red" Visible="false" Font-Bold="true"></asp:Label>
    </div>
</asp:Panel>
<script type="text/javascript">
    function IEEtPrepara() {
        $('#BlkPagEtPrepara').hide();
        $('input[type="submit"]').click(function() {
            $('#BlkPagEtPrepara').show();
        });
    }
    $(document).ready(IEEtPrepara);
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_endRequest(IEEtPrepara);
    </script>

