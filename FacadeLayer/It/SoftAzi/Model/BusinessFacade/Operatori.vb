Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade

    Public Class Operatori
        Public Function getOperatoriByName(ByVal nome As String) As ArrayList
            Dim myOperatori As New Roles.OperatoriRole
            Return myOperatori.getOperatoriByName(nome)
        End Function
        Public Function UpdOperatoriDataOraUltAccesso(ByVal Nome As String) As Boolean
            Dim myOperatori As New Roles.OperatoriRole
            Return myOperatori.UpdOperatoriDataOraUltAccesso(Nome)
        End Function
        Public Function OperatoreConnesso(ByVal Nome As String, ByVal CodiceDitta As String, ByVal Postazione As String, ByVal Modulo As String, ByVal SessionID As String, ByVal ID As Integer, ByVal Codice As String, ByVal Azienda As String, ByVal Tipo As String, ByVal _Esercizio As String) As ArrayList
            Dim myOperatoreConnesso As New Roles.OperatoriRole
            Return myOperatoreConnesso.OperatoreConnesso(Nome, CodiceDitta, Postazione, Modulo, SessionID, ID, Codice, Azienda, Tipo, _Esercizio)
        End Function
        Public Function DelOperatoreConnesso(ByVal Nome As String, ByVal CodiceDitta As String, ByVal Modulo As String) As Boolean
            Dim myOperatori As New Roles.OperatoriRole
            Return myOperatori.DelOperatoreConnesso(Nome, CodiceDitta, Modulo)
        End Function
        Public Function UpdOperatoriDataOraUltAzione(ByVal Nome As String) As Boolean
            Dim myOperatori As New Roles.OperatoriRole
            Return myOperatori.UpdOperatoriDataOraUltAzione(Nome)
        End Function
    End Class
End Namespace