Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class AnaMagDes
        Public Function getAnaMagDesByCodiceArticolo(ByVal Codice As String) As ArrayList
            Dim myAnaMagDes As New Roles.AnaMagDesRole
            Return myAnaMagDes.getAnaMagDesByCodiceArticolo(Codice)
        End Function
        Public Function InsertAnaMagDes(ByVal myAnaMagDesEntity As Object) As Boolean
            Dim myAnaMagDes As New Roles.AnaMagDesRole
            Return myAnaMagDes.InsertAnaMagDes(myAnaMagDesEntity)
        End Function
        Public Function delAnaMagDesByCodiceArticolo(ByVal Codice As String) As Boolean
            Dim myAnaMagDes As New Roles.AnaMagDesRole
            Return myAnaMagDes.delAnaMagDesByCodiceArticolo(Codice)
        End Function
    End Class
End Namespace

