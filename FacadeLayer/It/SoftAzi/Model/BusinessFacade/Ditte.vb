Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Ditte
        Public Function getDitteByCodice(ByVal Codice As String) As ArrayList
            Dim myDitte As New Roles.DitteRole
            Return myDitte.getDitteByCodice(Codice)
        End Function
        Public Function getDitte() As ArrayList
            Dim myDitte As New Roles.DitteRole
            Return myDitte.getDitte()
        End Function
    End Class
End Namespace