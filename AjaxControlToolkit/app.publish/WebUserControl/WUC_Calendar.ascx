<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WUC_Calendar.ascx.vb" Inherits="SoftAziOnLine.WUC_Calendar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:TextBox ID="TextData" runat="server"></asp:TextBox>
<asp:ImageButton ID="imgBtnShowCalendar" runat="server" ImageUrl="~/Immagini/Icone/Calendar_scheduleHS.png" ToolTip="apri il calendario" /> 
<ajaxToolkit:CalendarExtender ID="calendarButtonExtender" runat="server" TargetControlID="TextData" 
    PopupButtonID="imgBtnShowCalendar" Format="dd/MM/yyyy" />
    
<asp:RegularExpressionValidator ID="DateRegexValidator" runat="server" ControlToValidate="TextData" 
    ValidationExpression="(((0[1-9]|[12][0-9]|3[01])([/])(0[13578]|10|12)([/])(\d{4}))|(([0][1-9]|[12][0-9]|30)([/])(0[469]|11)([/])(\d{4}))|((0[1-9]|1[0-9]|2[0-8])([/])(02)([/])(\d{4}))|((29)(\.|-|\/)(02)([/])([02468][048]00))|((29)([/])(02)([/])([13579][26]00))|((29)([/])(02)([/])([0-9][0-9][0][48]))|((29)([/])(02)([/])([0-9][0-9][2468][048]))|((29)([/])(02)([/])([0-9][0-9][13579][26])))" 
    ErrorMessage="data invalida" />
    
