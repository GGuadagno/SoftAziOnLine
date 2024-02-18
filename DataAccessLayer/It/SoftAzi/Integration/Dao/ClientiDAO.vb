Imports It.SoftAzi.Model.Entity.ClientiEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface ClientiDAO
        Function getClienti(ByVal idTransazione As Integer) As ArrayList
        Function getClientiByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        Function InsertUpdateCliente(ByVal idTransazione As Integer, ByVal myClientiEntity As Object) As Boolean
        Function delClientiByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function CIClienteByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function CIClienteByCodiceAZI(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function CIClienteByCodiceSCAD(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
    End Interface
End Namespace
