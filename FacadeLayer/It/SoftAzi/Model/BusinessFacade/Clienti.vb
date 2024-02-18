Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Clienti
        Public Function getClienti() As ArrayList
            Dim myClienti As New Roles.ClientiRole
            Return myClienti.getClienti()
        End Function
        Public Function getClientiByCodice(ByVal Codice As String) As ArrayList
            Dim myClienti As New Roles.ClientiRole
            Return myClienti.getClientiByCodice(Codice)
        End Function
        Public Function InsertUpdateCliente(ByVal myClientiEntity As Object) As Boolean
            Dim myClienti As New Roles.ClientiRole
            Return myClienti.InsertUpdateCliente(myClientiEntity)
        End Function
        Public Function delClientiByCodice(ByVal Codice As String) As Boolean
            Dim myClienti As New Roles.ClientiRole
            Return myClienti.delClientiByCodice(Codice)
        End Function
        Public Function CIClienteByCodice(ByVal Codice As String) As Boolean
            Dim myClienti As New Roles.ClientiRole
            Return myClienti.CIClienteByCodice(Codice)
        End Function
        Public Function CIClienteByCodiceAZI(ByVal Codice As String) As Boolean
            Dim myClienti As New Roles.ClientiRole
            Return myClienti.CIClienteByCodiceAZI(Codice)
        End Function
        Public Function CIClienteByCodiceSCAD(ByVal Codice As String) As Boolean
            Dim myClienti As New Roles.ClientiRole
            Return myClienti.CIClienteByCodiceSCAD(Codice)
        End Function
    End Class
End Namespace

