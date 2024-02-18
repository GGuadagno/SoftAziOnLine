Imports System.Configuration
Module ModMain

    Sub Main()
        Dim InvioTest As String = ConfigurationManager.AppSettings("InvioTest").ToString()
        If PrevInstance(AppName) Then
            MsgBox("Risulta aperta una sessione di [Invio automatico E-mail].", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, AppName)
            Exit Sub
        End If
        Dim FrmInvio As New FrmInvioMail
        If InvioTest.Trim = "1" Then
            FrmInvio.set_InvioTest = True
        Else
            FrmInvio.set_InvioTest = False
        End If
        FrmInvio.ShowDialog()

    End Sub
    Private Function PrevInstance(ByVal CurrentProcess As String) As Boolean
        Dim TestCount As Integer = 0

        If CurrentProcess = AppName Then
            TestCount += 1
        End If
        If Diagnostics.Process.GetProcessesByName(CurrentProcess).Length > TestCount Then
            Return True
        Else
            Return False
        End If
    End Function
End Module
