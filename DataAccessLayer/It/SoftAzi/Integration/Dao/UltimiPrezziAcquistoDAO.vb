Imports It.SoftAzi.Model.Entity.UltimiPrezziAcquistoEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface UltimiPrezziAcquistoDAO
        Function getUltimiPrezziAcquistoByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        Function InsertUltimiPrezziAcquisto(ByVal idTransazione As Integer, ByVal paramUltPrezzoAcquisto As Object) As Boolean
    End Interface
End Namespace
