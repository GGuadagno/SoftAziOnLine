Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class AnaMagCTV
        Public Function getAnaMagCTVByCodiceArticolo(ByVal Codice As String) As ArrayList
            Dim myAnaMagCTV As New Roles.AnaMagCTVRole
            Return myAnaMagCTV.getAnaMagCTVByCodiceArticolo(Codice)
        End Function
        Public Function InsertAnaMagCTV(ByVal myAnaMagCTVEntity As Object) As Boolean
            Dim myAnaMagCTV As New Roles.AnaMagCTVRole
            Return myAnaMagCTV.InsertAnaMagCTV(myAnaMagCTVEntity)
        End Function
        Public Function delAnaMagCTVByCodiceArticolo(ByVal Codice As String) As Boolean
            Dim myAnaMagCTV As New Roles.AnaMagCTVRole
            Return myAnaMagCTV.delAnaMagCTVByCodiceArticolo(Codice)
        End Function
    End Class
End Namespace

