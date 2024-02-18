Imports System.Web.SessionState
Imports System.Web.HttpContext
Imports It.SoftAzi.SystemFramework

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        WebControl.DisabledCssClass = "" 'giu030224 per i tasti se disabilitati si ridimensionano problema del FW4
        ' Generato quando l'applicazione viene avviata
        Dim listLogOnUtente As New ArrayList
        HttpRuntime.Cache.Insert("listLogOnUtente", listLogOnUtente)

        Dim lstTipoUtente As New ArrayList
        lstTipoUtente.Add("Amministratore")
        lstTipoUtente.Add("Ufficio Amministrativo")
        lstTipoUtente.Add("Tecnico")
        lstTipoUtente.Add("Magazzino")
        lstTipoUtente.Add("Ufficio Acquisti")
        lstTipoUtente.Add("Ufficio Vendite")
        HttpRuntime.Cache.Insert("TipoUtente", lstTipoUtente)
        'aggiunto da Giuliano e Giu
        ' ''ApplicationConfiguration.OnApplicationStart(Context.Request.ApplicationPath)
        ApplicationConfiguration.OnApplicationStart(ConfigurationManager.AppSettings("AppPath"))
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando la sessione viene avviata
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'inizio di ogni richiesta
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al tentativo di autenticare l'utilizzo
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando si verifica un errore
        If Not System.Diagnostics.EventLog.SourceExists("SoftAziOnLine") Then
            System.Diagnostics.EventLog.CreateEventSource("SoftAziOnLine", "Application")
        End If
        System.Diagnostics.EventLog.WriteEntry("SoftAziOnLine", Server.GetLastError().Message)
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al termine della sessione
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al termine dell'applicazione
    End Sub

End Class