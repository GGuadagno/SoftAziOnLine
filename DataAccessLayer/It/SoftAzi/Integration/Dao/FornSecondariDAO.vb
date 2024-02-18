Imports It.SoftAzi.Model.Entity.FornSecondariEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface FornSecondariDAO
        Function getFornSecondariByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        Function delFornSecByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
        Function InsertFornitoriSec(ByVal idTransazione As Integer, ByVal myFornSecEntity As Object) As Boolean
    End Interface
End Namespace
