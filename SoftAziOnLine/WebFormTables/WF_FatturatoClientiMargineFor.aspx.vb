﻿Imports SoftAziOnLine.Def
Imports SoftAziOnLine.WebFormUtility
Partial Public Class WF_FatturatoClientiMargineFor
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session(CSTVISUALMENU) = SWSI
    End Sub

End Class