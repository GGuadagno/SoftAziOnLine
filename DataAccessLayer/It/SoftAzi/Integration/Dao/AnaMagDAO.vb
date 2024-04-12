Imports It.SoftAzi.Model.Entity.AnaMagEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface AnaMagDAO
        Function getAnaMag(ByVal idTransazione As Integer) As ArrayList
        Function getAnaMagByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        Function delAnaMagByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function InsertUpdateAnaMag(ByVal idTransazione As Integer, ByVal myAnaMag As Object) As Boolean
        Function CIAnaMagByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function getAnaMagCodDes(ByVal idTransazione As Integer) As ArrayList 'giu110424
    End Interface
End Namespace
