Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class ParametriGeneraliAzi
        Public Function getParametriGeneraliAzi() As ArrayList
            Dim myParamGenAzi As New Roles.ParametriGeneraliAziRole
            Return myParamGenAzi.getParametriGeneraliAzi()
        End Function
    End Class
End Namespace

