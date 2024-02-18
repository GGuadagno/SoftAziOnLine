Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Vettori
        Public Function getVettori() As ArrayList
            Dim myVettori As New Roles.VettoriRole
            Return myVettori.getVettori()
        End Function
    End Class
End Namespace

