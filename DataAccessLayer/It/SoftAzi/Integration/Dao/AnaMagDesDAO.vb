Imports It.SoftAzi.Model.Entity.AnaMagDesEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface AnaMagDesDAO
        Function getAnaMagDesByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        Function InsertAnaMagDes(ByVal idTransazione As Integer, ByVal myAnaMagDesEntity As Object) As Boolean
        Function delAnaMagDesByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
    End Interface
End Namespace
