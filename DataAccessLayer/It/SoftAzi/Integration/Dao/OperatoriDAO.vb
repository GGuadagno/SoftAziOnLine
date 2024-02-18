Imports It.SoftAzi.Model.Entity.OperatoriEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface OperatoriDAO
        Function getOperatoriByName(ByVal idTransazione As Integer, ByVal nome As String) As ArrayList
        Function UpdOperatoriDataOraUltAccesso(ByVal idTransazione As Integer, ByVal Nome As String) As Boolean
        Function OperatoreConnesso(ByVal idTransazione As Integer, ByVal Nome As String, ByVal CodiceDitta As String, ByVal Postazione As String, ByVal Modulo As String, ByVal SessionID As String, ByVal ID As Integer, ByVal Codice As String, ByVal Azienda As String, ByVal Tipo As String, ByVal Esercizio As String) As ArrayList
        Function DelOperatoreConnesso(ByVal idTransazione As Integer, ByVal Nome As String, ByVal CodiceDitta As String, ByVal Modulo As String) As Boolean
        Function UpdOperatoriDataOraUltAzione(ByVal idTransazione As Integer, ByVal Nome As String) As Boolean
    End Interface
End Namespace