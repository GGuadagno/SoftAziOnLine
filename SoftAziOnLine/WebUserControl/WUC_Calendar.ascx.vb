Partial Public Class WUC_Calendar
    Inherits System.Web.UI.UserControl

    Public Const INDEX_CONTROL_VALUE = 0

    Private _SelectedDate As Date
    Property SelectedDate() As Date
        Get
            Return _SelectedDate
        End Get
        Set(ByVal value As Date)
            _SelectedDate = value
            If (Not calendarButtonExtender Is Nothing) Then
                calendarButtonExtender.SelectedDate = _SelectedDate
            End If
        End Set
    End Property

End Class