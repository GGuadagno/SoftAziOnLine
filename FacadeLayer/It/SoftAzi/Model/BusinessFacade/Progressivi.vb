Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Progressivi
        Public Function getProgressivi() As ArrayList
            Dim myProgressivi As New Roles.ProgressiviRole
            Return myProgressivi.getProgressivi()
        End Function
    End Class
End Namespace

