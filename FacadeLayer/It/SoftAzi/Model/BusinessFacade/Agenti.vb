Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Agenti
        Public Function getAgenti() As ArrayList
            Dim myAgenti As New Roles.AgentiRole
            Return myAgenti.getAgenti()
        End Function
    End Class
End Namespace

