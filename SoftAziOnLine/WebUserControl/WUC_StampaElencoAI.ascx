<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_StampaElencoAI.ascx.vb" Inherits="SoftAziOnLine.WUC_StampaElencoAI" %>
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
    DropShadow="True"
    PopupDragHandleControlID="programmaticPopupDragHandle"
    RepositionMode="RepositionOnWindowScroll" >
</ajaxToolkit:ModalPopupExtender>

<asp:LinkButton ID="LinkButton1" runat="server" Text="" />
<asp:Panel runat="server" CssClass="modalPopup" ID="programmaticPopup" style="display:none;width:auto;padding:10px">
    <asp:Panel runat="Server" ID="programmaticPopupDragHandle" Style="cursor: move;;border:solid 1px Gray;color:Black;text-align:center;">
        <asp:Label ID="Label1" runat="server" Text="Label"><b>Stampa Articoli installati</b></asp:Label>
        <asp:Panel ID="PanelCategoria" style="margin-top: 0px;" runat="server" GroupingText="Categorie clienti">
            <table width="100%">
                <tr>
                    <td align="left">Singola categoria</td><td>
                    <asp:DropDownList ID="ddlCatCli" runat="server" DataSourceID="SqlDa_CatCli" 
                        DataTextField="Descrizione" DataValueField="Codice" Width="500px" 
                        AppendDataBoundItems="true" Enabled="False">
                        <asp:ListItem Value="0" Text="Categoria non definita"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDa_CatCli" runat="server" 
                        SelectCommand="SELECT [Codice], [Descrizione] FROM [Categorie]">
                    </asp:SqlDataSource>
                    </td>
                    <td>
                    <asp:CheckBox ID="chkTutteCatCli" runat="server" Text="Seleziona tutte le categorie" 
                            AutoPostBack="true" Checked="True" />
                    </td>
                </tr>
                <tr>
                    <td align="left"></td>
                    <td>
                    <asp:CheckBox ID="chkRaggrCatCli" runat="server" Text="Seleziona tutte le categorie per descrizione iniziale uguale" 
                            AutoPostBack="false" Checked="False" Enabled="false" />
                    </td>
                </tr>
           </table>
      </asp:Panel>
    </asp:Panel>
    <br />  
    <table width="99%" border="2px;" style="margin:10px;">
        <tr>
            <td style="padding:10px;">
                <asp:Label ID="Label3" runat="server" ><b>Ordinamento per:</b></asp:Label><br />
                <asp:RadioButton ID="rbCliArt" runat="server" Text="Cliente/Articolo" GroupName="Tipo" Checked="True" /><br />
                <asp:RadioButton ID="rbArtCli" runat="server" Text="Articolo/Cliente" GroupName="Tipo"/><br />
                <asp:RadioButton ID="rbScGaArtCli" runat="server" Text="Scadenza Garanzia/Articolo/Cliente" GroupName="Tipo"/><br />
                <asp:RadioButton ID="rbScElArtCli" runat="server" Text="Scadenza Elettrodi/Articolo/Cliente" GroupName="Tipo"/><br />
                <asp:RadioButton ID="rbScBaArtCli" runat="server" Text="Scadenza Batteria/Articolo/Cliente" GroupName="Tipo"/><br />
            </td>
        </tr>
    </table>  
    <table width="99%" border="2px;" style="margin:10px;">
        <tr>
            <td style="padding:10px;">
                <asp:Label ID="Label4" runat="server" ><b>Seleziona dati:</b></asp:Label><br />
                <table style="margin:0px;">
                    <tr>
                        <td >
                             <div>
                                <asp:Label ID="Label9" runat="server" Width="150px">Dalla data di scadenza </asp:Label>
                                <asp:TextBox ID="txtDallaDataSc" runat="server" MaxLength="10" TabIndex="1"
                                        Width="80px" style="margin-right:150px;" ></asp:TextBox>
                                <asp:Label ID="Label6" runat="server" Width="150px">Alla data di scadenza </asp:Label>
                                <asp:TextBox ID="txtAllaDataSc" runat="server" MaxLength="10" TabIndex="1"
                                        Width="80px"></asp:TextBox>
                                <%--<asp:ImageButton ID="imgBtnShowCalendarDSc" runat="server" 
                                        ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" 
                                        ToolTip="apri il calendario" />
                                <asp:CalendarExtender ID="txtDataScadenza_CalendarExtender" runat="server" 
                                        Enabled="True" Format="dd/MM/yyyy" PopupButtonID="imgBtnShowCalendarDSc" 
                                        TargetControlID="txtDataScadenza">
                                </asp:CalendarExtender>
                                <asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" 
                                        ControlToValidate="txtDataScadenza" ErrorMessage="*" 
                                        ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" />--%>
                              </div> 
                              <div><br /></div> 
                            <%--<div>
                                <asp:Label ID="Label2" runat="server" Width="150px">Codice Articolo </asp:Label>
                                <asp:TextBox ID="txtCodArticolo" runat="server" MaxLength="20" TabIndex="2" Width="120px" AutoPostBack="False"></asp:TextBox>
                              </div>   
                              <div><br /></div> 
                              <div>
                                <asp:Label ID="Label5" runat="server" Width="150px">Codice Cliente </asp:Label>
                                <asp:TextBox ID="txtCodCliente" runat="server" Width="120px" MaxLength="16" TabIndex="3" AutoPostBack="False" />
                              </div>
                              <div><br /></div>--%> 
                              <div>  
                                <asp:CheckBox ID="chkSelScGa" runat="server" Text="Seleziona scadenza Garanzia" TabIndex="5" Checked="True" />
                              </div> 
                              <div> 
                                <asp:CheckBox ID="chkSelScEl" runat="server" Text="Seleziona scadenza Elettrodi" TabIndex="6" Checked="True" />
                              </div>
                              <div>
                                <asp:CheckBox ID="chkSelScBa" runat="server" Text="Seleziona scadenza Batteria" TabIndex="7" Checked="True" />
                              </div>
                        </td>
                    </tr>
                </table>                               
            </td>
        </tr>
    </table>
    <div style="text-align:center;">    
        <asp:Button ID="btnOK" runat="server" Text="OK"  OnClick="OkButton_Click"/>
        <asp:Button ID="btnCancel" runat="server" Text="Annulla" OnClick="btnCancel_Click" />
    </div>
    <%--<br />--%>
</asp:Panel>
 