Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class DistBase
        Public Function getDistBaseByCodiceArticolo(ByVal Codice As String) As ArrayList
            Dim myDistBase As New Roles.DistBaseRole
            Return myDistBase.getDistBaseByCodiceArticolo(Codice)
        End Function
        Public Function InsertDistBase(ByVal myDistBaseEntity As Object) As Boolean
            Dim myDistBase As New Roles.DistBaseRole
            Return myDistBase.InsertDistBase(myDistBaseEntity)
        End Function
        Public Function delDistBaseByCodiceArticolo(ByVal Codice As String) As Boolean
            Dim myDistBase As New Roles.DistBaseRole
            Return myDistBase.delDistBaseByCodiceArticolo(Codice)
        End Function
    End Class
End Namespace
