Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class FornSecondari
        Public Function getFornSecondariByCodiceArticolo(ByVal Codice As String) As ArrayList
            Dim myFornSecondari As New Roles.FornSecondariRole
            Return myFornSecondari.getFornSecondariByCodiceArticolo(Codice)
        End Function
        Public Function delFornSecByCodiceArticolo(ByVal Codice As String) As Boolean
            Dim myFornSecondari As New Roles.FornSecondariRole
            Return myFornSecondari.delFornSecByCodiceArticolo(Codice)
        End Function
        Public Function InsertFornitoriSec(ByVal myFornSecEntity As Object) As Boolean
            Dim myFornSecondari As New Roles.FornSecondariRole
            Return myFornSecondari.InsertFornitoriSec(myFornSecEntity)
        End Function
    End Class
End Namespace

