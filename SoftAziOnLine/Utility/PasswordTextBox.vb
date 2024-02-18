Imports System
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports System.Text
' aggiungere questo tag nella sezione page del web.config
' <add tagPrefix="opp" namespace="SoftAziOnLine.opp" assembly="SoftAziOnLine"/>

Namespace opp
    Public Class PasswordTextBox
        Inherits TextBox
        Public Overrides Property TextMode() As System.Web.UI.WebControls.TextBoxMode
            Get
                Return MyBase.TextMode
            End Get
            Set(ByVal value As System.Web.UI.WebControls.TextBoxMode)
                MyBase.TextMode = value
            End Set
        End Property
        Public Overrides Property Text() As String
            Get
                Return MyBase.Text
            End Get
            Set(ByVal value As String)
                MyBase.Text = value
                Attributes("value") = value
            End Set
        End Property

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)
            Attributes("value") = Text
        End Sub
    End Class
End Namespace
