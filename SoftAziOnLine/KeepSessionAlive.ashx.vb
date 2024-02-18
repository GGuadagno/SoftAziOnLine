Imports System.Web
Imports System.Web.Services

Public Class KeepSessionAlive
    Implements System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1))
        context.Response.Cache.SetNoStore()
        context.Response.Cache.SetNoServerCaching()
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class