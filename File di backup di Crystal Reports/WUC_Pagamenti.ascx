<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Pagamenti.ascx.vb" Inherits="SoftAziOnLine.WUC_Pagamenti" %>
<%@ Register src="~/WebUserControl/WUC_ModalPopup.ascx" tagname="WUC_ModalPopup" tagprefix="uc1" %>

<style type="text/css">
    .style1
    {
        width: 506px;
    }
    .style2
    {
        width: 170px;
    }
    .style3
    {
        width: 76px;
    }
</style>

<uc1:WUC_ModalPopup ID="ModalPopup" runat="server" Visible="True" />
<table style="width:auto;height:auto;">
    <tr>
        <td class="style1" >
            <div id="div1" style="border-style:ridge;border-width:thin" id="noradio">
            <table style="width: 680px"> 
                <tr>
                    <td align="left" class="style2"></td>
                    <td colspan="3">
                    <asp:Label ID="lblLabelTipoRK" runat="server" Font-Bold="True">Elenco</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4" align="center" class="style2">                    
                        <asp:Label ID="LblErrore" runat="server" Font-Bold="True" ForeColor ="Red" ></asp:Label>
                    </td>                    
                </tr>                
                <tr>
                    <td align="left" class="style2">Seleziona</td>
                    <td colspan="3">
                        <asp:DropDownList ID="ddlPagamenti" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="99%"
                            DataSourceID="SqlDSPagamenti" DataTextField="Descrizione" 
                            DataValueField="Codice" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem></asp:DropDownList>
                        <asp:SqlDataSource ID="SqlDSPagamenti" runat="server" 
                            SelectCommand="SELECT * FROM [Pagamenti] ORDER BY [Descrizione]" 
                            UpdateCommand="InsertUpdate_Pagamenti" 
                            UpdateCommandType="StoredProcedure">
                            <UpdateParameters>
                                <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                                <asp:Parameter Name="Codice" Type="Int32" />
                                <asp:Parameter Name="Descrizione" Type="String" />
                                <asp:Parameter Name="Tipo_Pagamento" Type="Int32" />
                                <asp:Parameter Name="Tipo_Scadenza" Type="Int32" />
                                <asp:Parameter Name="Numero_Rate" Type="Int32" />
                                <asp:Parameter Name="Mese" Type="Int32" />
                                <asp:Parameter Name="Scadenza_1" Type="Int32" />
                                <asp:Parameter Name="Scadenza_2" Type="Int32" />
                                <asp:Parameter Name="Scadenza_3" Type="Int32" />
                                <asp:Parameter Name="Scadenza_4" Type="Int32" />
                                <asp:Parameter Name="Scadenza_5" Type="Int32" />
                                <asp:Parameter Name="Perc_Imponib_1" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imponib_2" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imponib_3" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imponib_4" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imponib_5" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imposta_1" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imposta_2" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imposta_3" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imposta_4" Type="Decimal" />
                                <asp:Parameter Name="Perc_Imposta_5" Type="Decimal" />
                                <asp:Parameter Name="Mese_Escluso_1" Type="Int32" />
                                <asp:Parameter Name="Mese_Escluso_2" Type="Int32" />
                                <asp:Parameter Name="Spese_Incasso" Type="Decimal" />
                                <asp:Parameter Name="IVA_Spese_Incasso" Type="Int32" />
                                <asp:Parameter Name="Sconto_Cassa" Type="Decimal" />
                                <asp:Parameter Name="Numero_Rate_Effettive" Type="Int32" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Codice</td>
                    <td align="left">
                    <asp:TextBox ID="txtCodice" AutoPostBack="true" runat="server" Width="80px" 
                            MaxLength="5"></asp:TextBox>
                    </td>
                </tr>               
                <tr>
                    <td align="left" class="style2">Descrizione</td>
                    <td colspan="3" style="margin-left: 120px">
                        <asp:TextBox ID="txtDescrizione" AutoPostBack="true" runat="server" Width="99%" 
                            MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" class="style2">Tipo pagamento</td>
                    <td style="margin-left: 200px">
                    <asp:DropDownList ID="ddlTipoPagamento" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="200px" Height="22px"></asp:DropDownList>
                    </td>
                    <td align="right" class="style2">Tipo scadenza</td>
                    <td style="margin-left: 200px">
                    <asp:DropDownList ID="ddlTipoScadenza" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="200px" Height="22px"></asp:DropDownList>
                    </td>                    
                </tr>
                <tr>
                    <td align="left" class="style2">Numero di rate (1-5)</td>
                    <td align="left" style="margin-left: 280px">
                        <asp:TextBox ID="TxtNRate" runat="server" Width="80px" 
                            MaxLength="50" />
                    </td>                                            
                    <td colspan ="2">
                        <asp:RadioButton ID="optMeseInCorso"  ValidationGroup="0" GroupName="GrpMese" runat="server" Text ="Mese in corso" AutoPostBack ="true"></asp:RadioButton>
                        <asp:RadioButton ID="optMeseSuccessivo"  ValidationGroup="0" GroupName="GrpMese" runat="server" Text ="Mese successivo" AutoPostBack ="true"></asp:RadioButton>
                    </td>
                </tr>    
                <tr>
                    <td align="left" class="style2">Numero rate effettive</td>
                    <td align="left" style="margin-left: 280px" colspan ="3" >
                        <asp:TextBox ID="TxtNRateEffettive" runat="server" Width="80px" 
                            MaxLength="50" />
                    </td>                                            
                </tr>                              
                <tr>
                    <td align="left" class="style2">Scadenze giorni</td>
                    <td colspan ="3">
                        <table>
                        <tr>
                        <td align="left" >r.1&nbsp;<asp:TextBox ID="TxtScad1" runat="server" 
                                Width="50px" MaxLength="3" /></td>
                        <td align="left" >r.2&nbsp;<asp:TextBox ID="TxtScad2" runat="server" 
                                Width="50px" MaxLength="3" /></td>
                        <td align="left" >r.3&nbsp;<asp:TextBox ID="TxtScad3" runat="server" 
                                Width="50px" MaxLength="3" /></td>                                           
                        <td align="left" >r.4&nbsp;<asp:TextBox ID="TxtScad4" runat="server" 
                                Width="50px" MaxLength="3" /></td>                            
                        <td align="left" >r.5&nbsp;<asp:TextBox ID="TxtScad5" runat="server" 
                                Width="50px" MaxLength="3" /></td>                            
                        </tr>                                
                        </table>                                
                    </td>                            
                </tr>
                <tr>
                    <td align="left" class="style2">Percent. Importo</td>
                    <td colspan ="3">
                        <table>
                        <tr>
                        <td align="left" >r.1&nbsp;<asp:TextBox ID="TxtPercImporto1" runat="server" 
                                Width="50px" MaxLength="3" /></td>
                        <td align="left" >r.2&nbsp;<asp:TextBox ID="TxtPercImporto2" runat="server" 
                                Width="50px" MaxLength="3" /></td>
                        <td align="left" >r.3&nbsp;<asp:TextBox ID="TxtPercImporto3" runat="server" 
                                Width="50px" MaxLength="3" /></td>                                           
                        <td align="left" >r.4&nbsp;<asp:TextBox ID="TxtPercImporto4" runat="server" 
                                Width="50px" MaxLength="3" /></td>                            
                        <td align="left" >r.5&nbsp;<asp:TextBox ID="TxtPercImporto5" runat="server" 
                                Width="50px" MaxLength="3" /></td>                            
                        </tr>                                
                        </table>                                
                    </td> 
                </tr>
                <tr>
                    <td align="left" class="style2">Percent. Imposta</td>
                    <td colspan ="3">
                        <table>
                        <tr>
                        <td align="left" >r.1&nbsp;<asp:TextBox ID="TxtPercImposta1" runat="server" 
                                Width="50px" MaxLength="3" /></td>
                        <td align="left" >r.2&nbsp;<asp:TextBox ID="TxtPercImposta2" runat="server" 
                                Width="50px" MaxLength="3" /></td>
                        <td align="left" >r.3&nbsp;<asp:TextBox ID="TxtPercImposta3" runat="server" 
                                Width="50px" MaxLength="3" /></td>                                           
                        <td align="left" >r.4&nbsp;<asp:TextBox ID="TxtPercImposta4" runat="server" 
                                Width="50px" MaxLength="3" /></td>                            
                        <td align="left" >r.5&nbsp;<asp:TextBox ID="TxtPercImposta5" runat="server" 
                                Width="50px" MaxLength="3" /></td>                            
                        </tr>                                
                        </table>                                
                    </td> 
                </tr>   
                <tr>
                    <td align="left" class="style2">Escludi il mese</td>
                    <td style="margin-left: 200px">
                    <asp:DropDownList ID="ddlEscludiMese1" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="200px" Height="22px"></asp:DropDownList>
                    </td>
                    <td align="right" class="style2">Escludi il mese</td>
                    <td style="margin-left: 200px">
                    <asp:DropDownList ID="ddlEscludiMese2" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="True" Width="200px" Height="22px"></asp:DropDownList>
                    </td>                    
                </tr>  
                <tr>
                    <td align="left" class="style2">Spese d'incasso</td>
                    <td align="left">
                    <asp:TextBox ID="TxtSpeseIncasso" runat="server" Width="80px" MaxLength="3" />
                    </td>
                    <td align="right" class="style2">Sconto cassa %</td>
                    <td align="left">
                    <asp:TextBox ID="TxtScontoCassa" runat="server" Width="50px" MaxLength="3" />
                    </td>                    
                </tr>   
                <tr>
                    <td align="left" class="style2">IVA Spese d'incasso</td>
                    <td align="left" colspan ="3">
                    <asp:DropDownList ID="ddlIVASpese" runat="server" AppendDataBoundItems="True" 
                            AutoPostBack="false" Width="200px"
                            DataSourceID="SqlDSIVA" DataTextField="Descrizione" 
                            DataValueField="Aliquota" Height="22px"><asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:SqlDataSource ID="SqlDSIVA" runat="server" 
                    SelectCommand="SELECT * FROM [Aliquote_IVA] ORDER BY [Descrizione]"                             
                    UpdateCommandType="StoredProcedure">
                    </asp:SqlDataSource>                            
                    </td>
                </tr>                                                           
            </table>
            </div>
        </td>
    </tr>
</table>
