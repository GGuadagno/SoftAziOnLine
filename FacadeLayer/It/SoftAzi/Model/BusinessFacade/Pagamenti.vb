Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Pagamenti
        Public Function getPagamenti() As ArrayList
            Dim myPagamenti As New Roles.PagamentiRole
            Return myPagamenti.getPagamenti()
        End Function
    End Class
End Namespace

