<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_UploadLotti.ascx.vb" Inherits="SoftAziOnLine.WUC_UploadLotti" %>
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
<asp:Panel ID="panelPrincipale" runat="server" Width="980px" Height="800px" BackColor="white">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <uc1:ModalPopup ID="ModalPopup1" runat="server" />
    <br>
<table style="vertical-align:middle; background-color:Silver; border-style:double; height: 800; width: 950px;" >
                <tr>
                    <td>
                        <asp:Panel ID="PanelSelezionaDoc" style="margin-top: 0px;" runat="server" 
                            GroupingText="">
                            <table width="100%">
                            <tr>                            
                                <td width="15%" >
                                    <asp:Label ID="LblDescDataDoc" runat="server" Width="165px" Height="17px">Data documento</asp:Label>
                                </td>


                    </td>
                    <td>
                                    <table width="100%">
                                    <tr>
                                        <td >
                                            <asp:Label ID="LblDataDoc" runat="server" Width="150px" Height="17px" BorderStyle="Outset"></asp:Label>
                                        </td>                                        
                                        <td align="right" >
                                            <asp:Label ID="LblDescNumDoc" runat="server" Width="165px" Height="17px">Numero documento</asp:Label>
                                        </td>
                                        <td >
                                            <asp:Label ID="LblNumDoc" runat="server" Width="250px" Height="17px" BorderStyle="Outset"></asp:Label>
                                        </td>                                                                                           
                                    </tr>                                    
                                    </table> 
                    </td>                                                       
                </tr>                                               
                <tr>                            
                    <td><asp:Label ID="LblDescDenominazione" runat="server" Width="165px" Height="17px">Intestazione documento</asp:Label></td>
                    <td width="50%" align="left" >
                        <asp:Label ID="LblRiga1" runat="server" Width="99%" 
                            Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
                </tr>
                <tr>                            
                    <td>&nbsp;</td>
                    <td width="50%"  align="left" >
                        <asp:Label ID="LblRiga2" runat="server" Width="99%" 
                            Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
                </tr>                       
                <tr>                            
                    <td>&nbsp;</td>
                    <td width="50%" align="left" >
                        <asp:Label ID="LblRiga3" runat="server" Width="99%" Height="17px" 
                            BorderStyle="Outset"></asp:Label></td>                                                       
                </tr>                                               
                <tr>                            
                    <td>&nbsp;</td>
                    <td width="50%" align="left" >
                        <asp:Label ID="LblRiga4" runat="server" Width="99%" 
                            Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
                </tr>   
                <tr>                            
                    <td>&nbsp;</td>
                    <td width="50%" align="left" >
                        <asp:Label ID="LblRiga5" runat="server" Width="99%" 
                            Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
                </tr>  
                <tr>                            
                    <td>&nbsp;</td>
                    <td width="50%" align="left" >
                        <asp:Label ID="LblRiga6" runat="server" Width="99%" 
                            Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
                </tr>  
                <tr>                            
                    <td>&nbsp;</td>
                    <td width="50%" align="left" >
                        <asp:Label ID="LblRiga7" runat="server" Width="99%" 
                            Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
                </tr>                                                                                                 
                <tr>                            
                    <td>&nbsp;</td>
                    <td width="50%" align="left" >
                        <asp:Label ID="LblRiga8" runat="server" Width="99%" 
                            Height="17px" BorderStyle="Outset"></asp:Label></td>                                                       
                </tr> 
                <tr>                            
                    <%--<td>&nbsp;</td>--%>
                    <td width="99%" align="left" colspan="2">
                        <asp:Label ID="Label1" runat="server" Width="99%" 
                        Height="17px" BorderStyle="None" ForeColor="Blue" Font-Bold="true" Text="Formato file Csv: ID ; Cod.Cliente ; N° XX ; Data ; Cod.Articolo ; N°Serie ; N°Lotto
"></asp:Label></td>                                                       
                </tr>   
                <tr>                            
                    <%--<td>&nbsp;</td>--%>
                    <td width="99%" align="left" colspan="2">
                        <asp:Label ID="Label2" runat="server" Width="99%" 
                            Height="17px" BorderStyle="None" ForeColor="Blue" Font-Bold="true" Text="NOTA solo nella 1 riga dopo il N° indicare uno dei seguenti valori: DT/FC/FA/CM/SM/MM sostituendo XX"></asp:Label></td>                                                       
                </tr>  
       </asp:Panel>                     
</table>                        
      <asp:Panel ID="Panel" runat="server" groupingtext="Caricamento File" style="margin-top: 0px;" Width="860px">
           <table width="100%">
           <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <Triggers>
                    <asp:PostBackTrigger ControlID="btnUpload" />
                </Triggers>
                <ContentTemplate>
                <tr>                            
                    <td><asp:FileUpload ID="FileUpload1" runat="server" BorderStyle="Outset" /></td>
                    <td><asp:Button ID="btnUpload" runat="server" Text="Controlla File" /></td>
                    <td><asp:Label ID="lblFile" runat="server"></asp:Label></td>                                                                  
                </tr>
                </ContentTemplate>
            </asp:UpdatePanel>
            </table>            
        </asp:Panel>
                </td>
                    <td align="left" class="style7">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div>
                                    <asp:Button ID="btnAggiorna" runat="server" class="btnstyle" Text="Aggiorna Lotti" Enabled="false" TabIndex="20" />
                                </div>
                                <div style="height: 15px"></div>
                                <div>
                                    <asp:Button ID="btnAnnulla" runat="server" class="btnstyle" Text="Annulla" Enabled="true" TabIndex="21" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>                                                               
                    </td>
                        </tr>
                    </caption>
                    </tr>
            </table>
    </ContentTemplate>
 </asp:UpdatePanel>    
</asp:Panel>