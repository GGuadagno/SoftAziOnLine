Imports It.SoftAzi.Model.Entity.ListVenDEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface ListVenDDAO
        Function getListVenDByCodLisCodArt(ByVal idTransazione As Integer, ByVal CodLis As Integer, ByVal CodArt As String) As ArrayList
        Function InsertUpdateListVenD(ByVal idTransazione As Integer, ByVal myListVenD As Object) As Boolean
        Function InsertiArticoliOu(ByVal idTransazione As Integer, ByVal CodLis As Integer) As Boolean
    End Interface
End Namespace
