Imports It.SoftAzi.Model.Entity.ListVenTEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface ListVenTDAO
        Function getListVenTByCodice(ByVal idTransazione As Integer, ByVal Codice As Integer) As ArrayList
        Function getListVenT(ByVal idTransazione As Integer) As ArrayList
    End Interface
End Namespace
