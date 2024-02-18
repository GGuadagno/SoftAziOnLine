Imports It.SoftAzi.Model.Entity.FilialiEntity
Namespace It.SoftAzi.Integration.Dao
    Public Interface FilialiDAO
        Function getFilialiElenco(ByVal idTransazione As Integer, ByVal ABI As String) As ArrayList
    End Interface
End Namespace