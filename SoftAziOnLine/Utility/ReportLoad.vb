Imports Microsoft.Reporting.WebForms

Public Class ReportLoad
    Private _ReportPath As String
    Private _TypeName As String
    Private _Report_Parameter As List(Of ReportParameter)
    Private _Report_DataSource As ArrayList
    Property ReportPath() As String
        Get
            Return _ReportPath
        End Get
        Set(ByVal value As String)
            _ReportPath = value
        End Set
    End Property
    Property TypeName() As String
        Get
            Return _TypeName
        End Get
        Set(ByVal value As String)
            _TypeName = value
        End Set
    End Property
    Property Report_Parameter() As List(Of ReportParameter)
        Get
            Return _Report_Parameter
        End Get
        Set(ByVal value As List(Of ReportParameter))
            _Report_Parameter = value
        End Set
    End Property
    Property Report_DataSource() As ArrayList
        Get
            Return _Report_DataSource
        End Get
        Set(ByVal value As ArrayList)
            _Report_DataSource = value
        End Set
    End Property
End Class
