Imports It.SoftAzi.Model.Entity.AnaMagCTVEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface AnaMagCTVDAO
        Function getAnaMagCTVByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        Function InsertAnaMagCTV(ByVal idTransazione As Integer, ByVal myAnaMagCTVEntity As Object) As Boolean
        Function delAnaMagCTVByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
    End Interface
End Namespace
