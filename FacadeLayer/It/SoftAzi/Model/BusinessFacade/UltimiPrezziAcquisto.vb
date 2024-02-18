Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles
Imports It.SoftAzi.Model.Entity

Namespace It.SoftAzi.Model.Facade
    Public Class UltimiPrezziAcquisto
        Public Function geUltimiPrezziAcquistoByCodiceArticolo(ByVal Codice As String) As ArrayList
            Dim myUltimiPrezziAcquisto As New Roles.UltimiPrezziAcquistoRole
            Return myUltimiPrezziAcquisto.getUltimiPrezziAcquistoByCodiceArticolo(Codice)
        End Function
        Public Function InsertUltimiPrezziAcquisto(ByVal myUltPrezzoAcquisto As UltimiPrezziAcquistoEntity) As Boolean
            Dim myUltimiPrezziAcquisto As New Roles.UltimiPrezziAcquistoRole
            Return myUltimiPrezziAcquisto.InsertUltimiPrezziAcquisto(myUltPrezzoAcquisto)
        End Function
    End Class
End Namespace

