Imports It.SoftAzi.Model.Entity.DitteEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface DitteDAO
        Function getDitteByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        Function getDitte(ByVal idTransazione As Integer) As ArrayList
    End Interface
End Namespace