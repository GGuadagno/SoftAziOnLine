Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Categorie
        Public Function getCategorie() As ArrayList
            Dim myCategorie As New Roles.CategorieRole
            Return myCategorie.getCategorie()
        End Function
    End Class
End Namespace

