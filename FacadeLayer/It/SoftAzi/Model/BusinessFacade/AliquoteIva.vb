Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class AliquoteIva
        Public Function getAliquoteIva() As ArrayList
            Dim myAliquoteIva As New Roles.AliquoteIvaRole
            Return myAliquoteIva.getAliquoteIva()
        End Function
    End Class
End Namespace

