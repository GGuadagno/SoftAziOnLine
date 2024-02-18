Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Banche
        Public Function getElencoBanche() As ArrayList
            Dim myBanche As New Roles.BancheRole
            Return myBanche.getBancheElenco()
        End Function
    End Class
End Namespace
