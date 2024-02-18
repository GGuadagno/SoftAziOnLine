Imports It.SoftAzi.Model.Entity.ListVenTEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface TipoFattDAO
        Function getTipoFattByCodice(ByVal idTransazione As Integer, ByVal Codice As Integer) As ArrayList
        Function getTipoFatt(ByVal idTransazione As Integer) As ArrayList
    End Interface
End Namespace
