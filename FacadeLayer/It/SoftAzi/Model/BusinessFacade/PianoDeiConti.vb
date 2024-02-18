Option Strict On
Option Explicit On

Imports It.SoftAzi.Model.Roles

Namespace It.SoftAzi.Model.Facade
    Public Class PianoDeiConti
        Public Function getPianoDeiConti() As ArrayList
            Dim myPianoDeiConti As New Roles.PianoDeiContiRole
            Return myPianoDeiConti.getPianoDeiConti()
        End Function
    End Class
End Namespace

