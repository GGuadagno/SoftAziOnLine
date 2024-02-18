Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles
Imports It.SoftAzi.Model.Entity

Namespace It.SoftAzi.Model.Facade
    Public Class ListVenD
        Public Function getListVenDByCodLisCodArt(ByVal CodLis As Integer, ByVal CodArt As String) As ArrayList
            Dim myListVenD As New Roles.ListVenDRole
            Return myListVenD.getListVenDByCodLisCodArt(CodLis, CodArt)
        End Function
        Public Function InsertUpdateListVenD(ByVal myListVenDEntity As ListVenDEntity) As Boolean
            Dim myListVenD As New Roles.ListVenDRole
            Return myListVenD.InsertUpdateListVenD(myListVenDEntity)
        End Function
        Public Function InsertiArticoliOu(ByVal CodLis As Integer) As Boolean
            Dim myListVenD As New Roles.ListVenDRole
            Return myListVenD.InsertiArticoliOu(CodLis)
        End Function
    End Class
End Namespace