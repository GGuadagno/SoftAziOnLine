Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Province
        Public Function getProvince() As ArrayList
            Dim myProvince As New Roles.ProvinceRole
            Return myProvince.getProvince()
        End Function
    End Class
End Namespace

