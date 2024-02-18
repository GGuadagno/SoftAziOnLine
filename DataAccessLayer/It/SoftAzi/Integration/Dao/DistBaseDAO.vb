Imports It.SoftAzi.Model.Entity.DistBaseEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface DistBaseDAO
        Function getDistBaseByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As ArrayList
        Function InsertDistBase(ByVal idTransazione As Integer, ByVal myDistBaseEntity As Object) As Boolean
        Function delDistBaseByCodiceArticolo(ByVal idTransazione As Integer, ByVal Codice As String) As Boolean
    End Interface
End Namespace