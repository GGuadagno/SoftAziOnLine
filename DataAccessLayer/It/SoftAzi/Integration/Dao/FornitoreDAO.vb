Imports It.SoftAzi.Model.Entity.FornitoreEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface FornitoreDAO
        Function getFornitori(ByVal idTransazione As Integer) As ArrayList
        Function getFornitoreByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        'giu121211
        Function InsertUpdateFornitore(ByVal idTransazione As Integer, ByVal myFornitoriEntity As Object) As Boolean
        Function delFornitoriByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function CIFornitoreByCodice(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function CIFornitoreByCodiceAZI(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function CIFornitoreByCodiceSCAD(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
    End Interface
End Namespace
