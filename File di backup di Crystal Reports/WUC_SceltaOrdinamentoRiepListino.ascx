<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_SceltaOrdinamentoRiepListino.ascx.vb" Inherits="SoftAziOnLine.WUC_SceltaOrdinamentoRiepListino" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<link href="../App_Themes/Softlayout.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/StyleSheet.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaGridView.css" rel="stylesheet" type="text/css" />
<link href="../App_Themes/AcquaCalendar.css" rel="stylesheet" type="text/css" />

<style type="text/css">
    .modalBackground {
        background-color:Gray;
        filter:alpha(opacity=70);
        opacity:0.7;
    }

    .modalPopup {
        background-color:#ffffdd;
        border-width:3px;
        border-style:solid;
        border-color:Gray;
        padding:3px;
        width:250px;
    }
</style>

<ajaxToolkit:ModalPopupExtender runat="server" ID="ProgrammaticModalPopup"
    TargetControlID="LinkButton1"
    PopupControlID="programmaticPopup" 
    BackgroundCssClass="modalBackground"
    CancelControlID="btnAnnulla" 
    DropShadow="True"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>

<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
   
    <asp:Panel runat="Server" ID="Panel1" Style="cursor: move;background-color:#DDDDDD;border:solid 1px Gray;color:Black;text-align:center;">
        <asp:Label ID="Label3" runat="server" Text="Label"><b>Riepilogo Listini</b></asp:Label>
    </asp:Panel>
   
   <br />  
    <table border="2px">
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="Label"><b>Ordinamento</b></asp:Label><br /><br />
                <asp:RadioButton ID="RadioButton1" runat="server" Text="per codice" GroupName="Tipo"/>
                <asp:RadioButton ID="RadioButton2" runat="server" Text="per descrizione" GroupName="Tipo"/>
                <br /><br />
                <div style="text-align:left;">
                    <asp:Label ID="Label1" runat="server" Text="Label"><b>Scelta Fornitore</b></asp:Label>
                    <asp:DropDownList ID="ddlFornitori" runat="server" AppendDataBoundItems="true"
                        AutoPostBack="false" DataSourceID="SqlDataSourceFor" 
                        DataTextField="Rag_Soc" DataValueField="Codice_CoGe" Width="400px">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDataSourceFor" runat="server"
                         SelectCommand="SELECT Fornitori.Codice_CoGe, Fornitori.Rag_Soc FROM Fornitori INNER JOIN AnaMag ON Fornitori.Codice_CoGe = AnaMag.CodiceFornitore GROUP BY Fornitori.Codice_CoGe, Fornitori.Rag_Soc ORDER BY Fornitori.Rag_Soc" />
                </div>
                <br />       
                <br />  
                <div style="text-align:center;">    
                    <asp:Button ID="btnOK" runat="server" Text="OK"  OnClick="OkButton_Click"/>
                    <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" OnClick="CancelButton_Click"/>
                </div>
            </td>
        </tr> 
    </table>
    
   <br />

 </asp:Panel> 