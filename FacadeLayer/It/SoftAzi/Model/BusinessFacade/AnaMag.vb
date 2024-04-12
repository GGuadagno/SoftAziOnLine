Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles
Imports It.SoftAzi.Model.Entity

Namespace It.SoftAzi.Model.Facade
    Public Class AnaMag
        Public Function getAnaMag() As ArrayList
            Dim myAnaMag As New Roles.AnaMagRole
            Return myAnaMag.getAnaMag()
        End Function
        Public Function getAnaMagByCodice(ByVal Codice As String) As ArrayList
            Dim myAnaMag As New Roles.AnaMagRole
            Return myAnaMag.getAnaMagByCodice(Codice)
        End Function
        Public Function delAnaMagByCodice(ByVal Codice As String) As Boolean
            Dim myAnaMag As New Roles.AnaMagRole
            Return myAnaMag.delAnaMagByCodice(Codice)
        End Function
        Public Function InsertUpdateAnaMag(ByVal myAnaMagEntity As Object) As Boolean
            Dim myAnaMag As New Roles.AnaMagRole
            Return myAnaMag.InsertUpdateAnaMag(myAnaMagEntity)
        End Function
        Public Function CIAnaMagByCodice(ByVal Codice As String) As Boolean
            Dim myAnaMag As New Roles.AnaMagRole
            Return myAnaMag.CIAnaMagByCodice(Codice)
        End Function
        'GIU110424 PER VELOCIZZARE I CARICAMENTI INIZIALI
        Public Function getAnaMagCodDes() As ArrayList
            Dim myAnaMag As New Roles.AnaMagRole
            Return myAnaMag.getAnaMagCodDes()
        End Function
    End Class
End Namespace

