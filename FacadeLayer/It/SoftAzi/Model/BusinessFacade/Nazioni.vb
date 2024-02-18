Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Nazioni
        Public Function getNazioni() As ArrayList
            Dim myNazioni As New Roles.NazioniRole
            Return myNazioni.getNazioni()
        End Function
    End Class
End Namespace

