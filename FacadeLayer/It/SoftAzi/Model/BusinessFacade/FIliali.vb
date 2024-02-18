Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class FIliali
        Public Function getElencoFilialiByABI(ByVal ABI As String) As ArrayList
            Dim myFiliali As New Roles.FilialiRole
            Return myFiliali.getElencoFilialiByABI(ABI)
        End Function
    End Class
End Namespace