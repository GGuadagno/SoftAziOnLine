Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class Fornitore
        Public Function getFornitori() As ArrayList
            Dim myFornitore As New Roles.FornitoreRole
            Return myFornitore.getFornitori()
        End Function
        Public Function getFornitoreByCodice(ByVal Codice As String) As ArrayList
            Dim myFornitore As New Roles.FornitoreRole
            Return myFornitore.getFornitoreByCodice(Codice)
        End Function
        'giu121211
        Public Function InsertUpdateFornitore(ByVal myFornitoriEntity As Object) As Boolean
            Dim myFornitori As New Roles.FornitoreRole
            Return myFornitori.InsertUpdateFornitore(myFornitoriEntity)
        End Function
        Public Function delFornitoreByCodice(ByVal Codice As String) As Boolean
            Dim myFornitori As New Roles.FornitoreRole
            Return myFornitori.delFornitoriByCodice(Codice)
        End Function
        Public Function CIFornitoreByCodice(ByVal Codice As String) As Boolean
            Dim myFornitori As New Roles.FornitoreRole
            Return myFornitori.CIFornitoreByCodice(Codice)
        End Function
        Public Function CIFornitoreByCodiceAZI(ByVal Codice As String) As Boolean
            Dim myFornitori As New Roles.FornitoreRole
            Return myFornitori.CIFornitoreByCodiceAZI(Codice)
        End Function
        Public Function CIFornitoreByCodiceSCAD(ByVal Codice As String) As Boolean
            Dim myFornitori As New Roles.FornitoreRole
            Return myFornitori.CIFornitoreByCodiceSCAD(Codice)
        End Function
    End Class
End Namespace

