Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class ListVenT
        Public Function getListVenTByCodice(ByVal Codice As Integer) As ArrayList
            Dim myListVenT As New Roles.ListVenTRole
            Return myListVenT.getListVenTByCodice(Codice)
        End Function
        Public Function getListVenT() As ArrayList
            Dim myListVenT As New Roles.ListVenTRole
            Return myListVenT.getListVenT()
        End Function
    End Class
End Namespace