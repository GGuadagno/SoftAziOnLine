Option Strict On
Option Explicit On 

Imports System
Imports System.Diagnostics
Imports System.Configuration
Imports System.Collections
Imports System.Xml
Imports System.Collections.Specialized
Imports System.Web
Imports System.Web.SessionState

Namespace It.SoftAzi.SystemFramework

    '----------------------------------------------------------------
    ' Namespace: Duwamish7.SystemFramework
    ' Class: ApplicationConfiguration
    '
    ' Description:
    '   Standard configuration settings to enable tracing and logging
    '     with the ApplicationLog class. An application can use this
    '     class as a model for adding additional settings to a
    '     Config.Web file.
    '     
    ' Special Considerations:
    '   The OnApplicationStart function in this class must be called
    '     from the Application_OnStart event in Global.asax. This is
    '     currently used only to determine the path of the application,
    '     but the HttpContext object is passed it to enable the app
    '     to read other settings in the future, and to minimize the code
    '     in global.asax. The global.asax file should be similar
    '     to the following code:
    '
    '<%@ Import Namespace="Duwamish7.SystemFramework" Assembly="Duwamish7.SystemFramework.dll" %>
    '<script language="VB" runat=server>
    '    Sub Application_OnStart()
    '        ApplicationConfiguration.OnApplicationStart Context
    '    End Sub
    '</script>
    '----------------------------------------------------------------
    Public Class ApplicationConfiguration

        Implements IConfigurationSectionHandler, IRequiresSessionState
        
        'Costant value for Dao Setting
        Private Const FACTORY As String = "SystemFramework.Dao.Factory"
        Private Const DB_HOST As String = "SystemFramework.DB.Host"
        Private Const DB_NAMEMASTER As String = "SystemFramework.DB.Master" 'giu040412
        '<add key="SystemFramework.DB.Install" value="Install"/>
        '<add key="SystemFramework.DB.SoftAzi" value="NNAAAAGestAzi"/>
        '<add key="SystemFramework.DB.SoftCoge" value="NNAAAACoge"/>
        '<add key="SystemFramework.DB.Scadenze" value="NNScadene"/>
        '<add key="SystemFramework.DB.Opzioni" value="NNOpzioni"/>
        Private Const DB_NAMESOFTAZI As String = "SystemFramework.DB.SoftAzi"
        Private Const DB_NAMEINSALL As String = "SystemFramework.DB.Install"
        Private Const DB_NAMESOFTCOGE As String = "SystemFramework.DB.SoftCoge"
        Private Const DB_NAMESCADENZE As String = "SystemFramework.DB.Scadenze"
        Private Const DB_NAMEOPZIONI As String = "SystemFramework.DB.Opzioni"

        Private Const DB_LOGIN As String = "SystemFramework.DB.Login"
        Private Const DB_PASSWORD As String = "SystemFramework.DB.Password"
       
        '
        ' The root directory of the application. Established in the
        '   OnApplicationStart callback form Global.asax.
        '
        Private Shared fieldAppRoot As String

      

        'Dao Setting
        Private Shared fieldDbNameMaster As String  'giu040412
        Private Shared fieldFactory As String
        Private Shared fieldDbHost As String
        Private Shared fieldDbNameSoftCoge As String
        Private Shared fieldDbNameScadenze As String
        Private Shared fieldDbNameOpzioni As String
        Private Shared fieldDbNameSoftAzi As String
        Private Shared fieldDbNameInstall As String
        Private Shared fieldDbLogin As String
        Private Shared fieldDbPassword As String
        Private Shared fieldNNAAAA As String

        ' Constant values for DAO setting
        Private Const FACTORY_DEFAULT As String = "SQL"
        Private Const DB_HOST_DEFAULT As String = "GIA"
        Private Const DB_MASTER_DEFAULT As String = "master" 'GIU040412
        Private Const DB_NAME_DEFAULT As String = "XXXX"
        Private Const DB_LOGIN_DEFAULT As String = "sa"
        Private Const DB_PASSWORD_DEFAULT As String = "DB_PASSWORD_DEFAULT" 'GIU040412
        'giu050412
        Public Const DB_KEYPWD_SOFTSOL As String = "SoftSol"
        Public Const DB_KEYPWD_IREDEEM As String = "Iredeem"
        Public Const DB_KEYPWD_SFERA As String = "Sfera"
        '---
        Private Const DB_KEYSCAD_SOFTSOL As String = "31/12/2100"
        Private Const DB_KEYSCAD_IREDEEM As String = "31/12/2100"
        Private Const DB_KEYSCAD_SFERA As String = "31/12/2012"
        '---
        Private Const DB_PASSWORD_SOFTSOL As String = "%tgb6yhn"
        Private Const DB_PASSWORD_IREDEEM As String = "Bottic1517"
        Private Const DB_PASSWORD_SFERA As String = "saadm2012sfera!"
        '
        ' Constant values for web default settings.
        '

        '----------------------------------------------------------------
        ' Function Create:
        '   Called by ASP+ before the application starts to initialize
        '     settings from the Config.Web file(s). The app domain will
        '     restart if these settings change, so there is no reason
        '     to read these values more than once. This function uses the
        '     DictionarySectionHandler base class to generate a hashtable
        '     from the XML, which is then used to store the current settings.
        '     Because all settings are read here, we do not actually store
        '     the generated hashtable object for later retrieval by
        '     Context.GetConfig. The application should use the accessor
        '     functions directly.
        ' Returns:
        '   A ConfigOutput object, which we leave empty because all settings
        '     are stored at this point.
        ' Parameters:
        '   [in] parent: An object created by processing a section with this name
        '                in a Config.Web file in a parent directory.
        '   [in] configContext: The config's context.
        '   [in] section: The section to be read.
        '----------------------------------------------------------------
        Function Create(ByVal parent As Object, ByVal configContext As Object, ByVal section As System.Xml.XmlNode) As Object Implements IConfigurationSectionHandler.Create

            Dim Settings As New NameValueCollection

            Try
                Dim baseHandler As NameValueSectionHandler
                baseHandler = New NameValueSectionHandler
                settings = CType(baseHandler.Create(parent, configContext, section), NameValueCollection)
            Catch
            End Try
            If Settings Is Nothing Then
                ' Constant values for DAO setting
                fieldFactory = FACTORY_DEFAULT
                fieldDbHost = DB_HOST_DEFAULT
                fieldDbNameMaster = DB_MASTER_DEFAULT 'giu040412
                fieldDbNameInstall = DB_NAME_DEFAULT
                fieldDbNameSoftAzi = DB_NAME_DEFAULT
                fieldDbNameSoftCoge = DB_NAME_DEFAULT
                fieldDbNameScadenze = DB_NAME_DEFAULT
                fieldDbNameOpzioni = DB_NAME_DEFAULT
                fieldDbLogin = DB_LOGIN_DEFAULT
                fieldDbPassword = DB_PASSWORD_DEFAULT
            Else
                ' Constant values for DAO setting
                fieldFactory = ReadSetting(Settings, FACTORY, FACTORY_DEFAULT)
                fieldDbNameMaster = ReadSetting(Settings, DB_NAMEMASTER, DB_MASTER_DEFAULT) 'giu040412
                fieldDbNameInstall = ReadSetting(Settings, DB_NAMEINSALL, DB_NAME_DEFAULT)
                fieldDbNameSoftAzi = ReadSetting(Settings, DB_NAMESOFTAZI, DB_NAME_DEFAULT)
                fieldDbNameSoftCoge = ReadSetting(Settings, DB_NAMESOFTCOGE, DB_NAME_DEFAULT)
                fieldDbNameScadenze = ReadSetting(Settings, DB_NAMESCADENZE, DB_NAME_DEFAULT)
                fieldDbNameOpzioni = ReadSetting(Settings, DB_NAMEOPZIONI, DB_NAME_DEFAULT)
                fieldDbHost = ReadSetting(Settings, DB_HOST, DB_HOST_DEFAULT)
                fieldDbLogin = ReadSetting(Settings, DB_LOGIN, DB_LOGIN_DEFAULT)
                fieldDbPassword = ReadSetting(Settings, DB_PASSWORD, DB_PASSWORD_DEFAULT)
            End If

        End Function

        '----------------------------------------------------------------
        ' Shared Function ReadSetting:
        '   Reads a setting from a hashtable and converts it to the correct
        '     type. One of these functions is provided for each type
        '     expected in the hash table. These are public so that other
        '     classes don't have to duplicate them to read settings from
        '     a hash table.
        ' Returns:
        '   The value from the hash table, or the default if the item is not
        '     in the table or cannot be case to the expected type.
        ' Parameters:
        '   [in] settings: The Hashtable to read from
        '   [in] key: A key for the value in the Hashtable
        '   [in] default: The default value if the item is not found.
        '----------------------------------------------------------------

        '----------------------------------------------------------------
        ' String version of ReadSetting
        '----------------------------------------------------------------
        Public Overloads Shared Function ReadSetting(ByVal settings As NameValueCollection, ByVal key As String, ByVal defaultValue As String) As String
            Try
                Dim setting As Object = settings(key)
                If setting Is Nothing Then
                    ReadSetting = defaultValue
                Else
                    ReadSetting = CStr(setting)
                End If
            Catch
                ReadSetting = defaultValue
            End Try
        End Function

        '----------------------------------------------------------------
        ' Boolean version of ReadSetting
        '----------------------------------------------------------------
        Public Overloads Shared Function ReadSetting(ByVal settings As NameValueCollection, ByVal key As String, ByVal defaultValue As Boolean) As Boolean
            Try
                Dim setting As Object = settings(key)
                If setting Is Nothing Then
                    ReadSetting = defaultValue
                Else
                    ReadSetting = CBool(setting)
                End If
            Catch
                ReadSetting = defaultValue
            End Try
        End Function

        '----------------------------------------------------------------
        ' Long version of ReadSetting
        '----------------------------------------------------------------
        Public Overloads Shared Function ReadSetting(ByVal settings As NameValueCollection, ByVal key As String, ByVal defaultValue As Integer) As Integer
            Try
                Dim setting As Object = settings(key)
                If setting Is Nothing Then
                    ReadSetting = defaultValue
                Else
                    ReadSetting = CInt(setting)
                End If
            Catch
                ReadSetting = defaultValue
            End Try
        End Function

        '----------------------------------------------------------------
        ' TraceLevel version of ReadSetting
        '----------------------------------------------------------------
        Public Overloads Shared Function ReadSetting(ByVal settings As NameValueCollection, ByVal key As String, ByVal defaultValue As TraceLevel) As TraceLevel
            Try
                Dim setting As Object = settings(key)
                If setting Is Nothing Then
                    ReadSetting = defaultValue
                Else
                    ReadSetting = CType(CInt(setting), TraceLevel)
                End If
            Catch
                ReadSetting = defaultValue
            End Try
        End Function

        '----------------------------------------------------------------
        ' Shared Sub OnApplicationStart:
        '   Function to be called by Application_OnStart as described in the
        '     class description. Initializes the application root.
        ' Parameters:
        '   [in] AppRoot: The path of the running application.
        '----------------------------------------------------------------
        Public Shared Sub OnApplicationStart(ByVal AppRoot As String)
            fieldAppRoot = AppRoot
            System.Configuration.ConfigurationSettings.GetConfig("ApplicationConfiguration")
        End Sub

        '----------------------------------------------------------------
        ' Shared Property Get AppRoot:
        '   Retrieve the root path of the application
        ' Returns:
        '   Path
        '----------------------------------------------------------------
        Public Shared ReadOnly Property AppRoot() As String
            Get
                AppRoot = fieldAppRoot
            End Get
        End Property

      
        ' property for DAO setting
        Public Shared ReadOnly Property DbNameMaster() As String 'giu040412
            Get
                If fieldDbNameMaster Is Nothing Then
                    Return DB_MASTER_DEFAULT
                Else
                    Return fieldDbNameMaster
                End If
            End Get
        End Property
        Public Shared ReadOnly Property TypeFactory() As String
            Get
                If fieldFactory Is Nothing Then
                    Return FACTORY_DEFAULT
                Else
                    Return fieldFactory
                End If
            End Get
        End Property
        Public Shared ReadOnly Property DbHost() As String
            Get
                If fieldDbHost Is Nothing Then
                    Return DB_HOST_DEFAULT
                Else
                    Return fieldDbHost
                End If
            End Get
        End Property
        Public Shared ReadOnly Property DbNameInstall() As String
            Get
                If fieldDbNameInstall Is Nothing Then
                    Return DB_NAME_DEFAULT
                Else
                    Return fieldDbNameInstall
                End If
            End Get
        End Property
        Public Shared ReadOnly Property DbNameSoftAzi() As String
            Get
                If fieldDbNameSoftAzi Is Nothing Then
                    Return DB_NAME_DEFAULT
                Else
                    Return fieldDbNameSoftAzi
                End If
            End Get
        End Property

        Public Shared ReadOnly Property DbNameSoftCoge() As String
            Get
                If fieldDbNameSoftCoge Is Nothing Then
                    Return DB_NAME_DEFAULT
                Else
                    Return fieldDbNameSoftCoge
                End If
            End Get
        End Property

        Public Shared ReadOnly Property DbNameScadenze() As String
            Get
                If fieldDbNameScadenze Is Nothing Then
                    Return DB_NAME_DEFAULT
                Else
                    Return fieldDbNameScadenze
                End If
            End Get
        End Property

        Public Shared ReadOnly Property DbNameOpzioni() As String
            Get
                If fieldDbNameOpzioni Is Nothing Then
                    Return DB_NAME_DEFAULT
                Else
                    Return fieldDbNameOpzioni
                End If
            End Get
        End Property
        Public Shared ReadOnly Property DbLogin() As String
            Get
                If fieldDbLogin Is Nothing Then
                    Return DB_LOGIN_DEFAULT
                Else
                    Return DB_LOGIN_DEFAULT 'fieldDbLogin giu050412 muovo sempre l'utente sa 
                End If
            End Get
        End Property
        Public Shared ReadOnly Property DbPassword() As String
            Get
                If fieldDbPassword Is Nothing Then
                    Return Format(Now.Date, "dd/MM/yyyy HH:mm") 'DB_PASSWORD_DEFAULT
                Else
                    'fieldDbPassword giu040412 password di web.config
                    'giu050412
                    If fieldDbPassword = DB_KEYPWD_SOFTSOL Then
                        If Now.Date > CDate(DB_KEYSCAD_SOFTSOL).Date Then
                            Return Format(Now.Date, "dd/MM/yyyy HH:mm")
                        Else
                            Return DB_PASSWORD_SOFTSOL
                        End If
                    ElseIf fieldDbPassword = DB_KEYPWD_IREDEEM Then
                        If Now.Date > CDate(DB_KEYSCAD_IREDEEM).Date Then
                            Return Format(Now.Date, "dd/MM/yyyy HH:mm")
                        Else
                            Return DB_PASSWORD_IREDEEM
                        End If
                    ElseIf fieldDbPassword = DB_KEYPWD_SFERA Then
                        If Now.Date > CDate(DB_KEYSCAD_SFERA).Date Then
                            Return Format(Now.Date, "dd/MM/yyyy HH:mm")
                        Else
                            Return DB_PASSWORD_SFERA
                        End If
                    Else
                        Return Format(Now.Date, "dd/MM/yyyy HH:mm") 'DB_PASSWORD_DEFAULT
                    End If
                End If
            End Get
        End Property
        'giu311018 per sapere il cliente
        Public Shared ReadOnly Property DbCliente() As String
            Get
                If fieldDbPassword Is Nothing Then
                    Return "NON DEFINITO"
                Else
                    Return fieldDbPassword
                End If
            End Get
        End Property
        Public WriteOnly Property setNNAAAA() As String
            Set(ByVal value As String)
                fieldNNAAAA = value
            End Set
        End Property

        Public Shared ReadOnly Property getNNAAAA(Optional ByVal Anno As String = "") As String
            Get
                Try
                    If Anno = "" Then
                        Anno = HttpContext.Current.Session("Esercizio").ToString.Trim
                    ElseIf Anno.Trim <> HttpContext.Current.Session("Esercizio").ToString.Trim Then
                        Anno = Anno 'debug
                    End If
                Catch ex As Exception
                    Anno = ""
                End Try
                If fieldNNAAAA Is Nothing Then
                    Return "NNAAAA"
                Else
                    If Anno = "" Then
                        Return fieldNNAAAA
                    Else
                        Return Left(fieldNNAAAA, 2) & Anno
                    End If
                End If
            End Get
        End Property
    End Class
End Namespace
