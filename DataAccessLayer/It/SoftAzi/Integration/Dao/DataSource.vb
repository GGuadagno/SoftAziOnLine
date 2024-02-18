Imports It.SoftAzi.SystemFramework
Imports System.Data.SqlClient
Imports System.Collections
Namespace It.SoftAzi.Integration.Dao
    Public Class DataSource
        Public Enum TipoConnessione
            dbMaster = 0 'giu040412
            dbInstall = 1
            dbOpzioni = 2
            dbScadenzario = 3
            dbSoftCoge = 4
            dbSoftAzi = 5
        End Enum

        Private Shared myDataSouce As DataSource = Nothing
        Private Shared ReadOnly classLock As New DataSource
        Private myDbStringConnectionMaster As String 'giu040412
        Private myDbStringConnectionAzi As String
        Private myDbStringConnectionInstall As String
        Private myDbStringConnectionCoge As String
        Private myDbStringConnectionScadenze As String
        Private myDbStringConnectionOpzioni As String

        Private myActiveConnections() As SqlConnection
        Private myActiveTransactions() As SqlTransaction
        Private myPosizioniLibere As Stack
        Private Const DIMARRAYCONNECTIONS As Integer = 100
        Public Property dbStringConnection(ByVal TipoDB As TipoConnessione) As String
            Get
                Select Case TipoDB
                    Case TipoConnessione.dbMaster 'giu040412
                        Return myDbStringConnectionMaster
                    Case TipoConnessione.dbInstall
                        Return myDbStringConnectionInstall
                    Case TipoConnessione.dbOpzioni
                        Return myDbStringConnectionOpzioni
                    Case TipoConnessione.dbScadenzario
                        Return myDbStringConnectionScadenze
                    Case TipoConnessione.dbSoftAzi
                        Return myDbStringConnectionAzi
                    Case TipoConnessione.dbSoftCoge
                        Return myDbStringConnectionCoge
                    Case Else 'giu040412
                        Return ""
                End Select
            End Get
            Set(ByVal Value As String)
                Select Case TipoDB
                    Case TipoConnessione.dbMaster 'giu040412
                        myDbStringConnectionMaster = Value
                    Case TipoConnessione.dbInstall
                        myDbStringConnectionInstall = Value
                    Case TipoConnessione.dbOpzioni
                        myDbStringConnectionOpzioni = Value
                    Case TipoConnessione.dbScadenzario
                        myDbStringConnectionScadenze = Value
                    Case TipoConnessione.dbSoftAzi
                        myDbStringConnectionAzi = Value
                    Case TipoConnessione.dbSoftCoge
                        myDbStringConnectionCoge = Value
                End Select
            End Set
        End Property
        Public Function getConnectionString(ByVal tipodb As TipoConnessione, Optional ByVal Anno As String = "") As String
            Dim stNNAAAA As String = ApplicationConfiguration.getNNAAAA(Anno)

            Select Case tipodb
                Case TipoConnessione.dbMaster 'giu040412
                    Return myDbStringConnectionMaster
                Case TipoConnessione.dbInstall
                    Return myDbStringConnectionInstall
                Case TipoConnessione.dbOpzioni
                    Return myDbStringConnectionOpzioni.Replace("NN", stNNAAAA.Substring(0, 2))
                Case TipoConnessione.dbScadenzario
                    Return myDbStringConnectionScadenze.Replace("NN", stNNAAAA.Substring(0, 2))
                Case TipoConnessione.dbSoftAzi
                    Return myDbStringConnectionAzi.Replace("NNAAAA", stNNAAAA)
                Case TipoConnessione.dbSoftCoge
                    Return myDbStringConnectionCoge.Replace("NNAAAA", stNNAAAA)
            End Select
        End Function

        Public Shared Function getDataSource() As DataSource
            SyncLock (classLock)
                If (myDataSouce Is Nothing) Then
                    myDataSouce = New DataSource
                End If
            End SyncLock
            Return myDataSouce
        End Function
        Private Sub New()
            Dim host As String = ApplicationConfiguration.DbHost
            Dim login As String = ApplicationConfiguration.DbLogin
            Dim password As String = ApplicationConfiguration.DbPassword
            Dim stNNAAAA As String = ApplicationConfiguration.getNNAAAA

            'Dim dbName As String = "" 'A'pplicationConfiguration.DbName
            Dim i As Integer
            'ESEMPIO: myDbStringConnection = "Persist Security Info=False;User ID=SoftTiketsAdmin;Password=SoftTiketsAdmin;Initial Catalog=SoftTiketsDB;Data Source=AMD;Packet Size=4096"
            myDbStringConnectionMaster = "Persist Security Info=False;User ID=" & login _
                        & ";Password=" & password & ";Initial Catalog=" & ApplicationConfiguration.DbNameMaster & ";Data Source=" & _
                        host & ";Packet Size=4096"
            myDbStringConnectionInstall = "Persist Security Info=False;User ID=" & login _
                        & ";Password=" & password & ";Initial Catalog=" & ApplicationConfiguration.DbNameInstall & ";Data Source=" & _
                        host & ";Packet Size=4096"
            myDbStringConnectionOpzioni = "Persist Security Info=False;User ID=" & login _
                        & ";Password=" & password & ";Initial Catalog=" & ApplicationConfiguration.DbNameOpzioni & ";Data Source=" & _
                        host & ";Packet Size=4096"
            myDbStringConnectionScadenze = "Persist Security Info=False;User ID=" & login _
                        & ";Password=" & password & ";Initial Catalog=" & ApplicationConfiguration.DbNameScadenze & ";Data Source=" & _
                        host & ";Packet Size=4096"
            myDbStringConnectionAzi = "Persist Security Info=False;User ID=" & login _
                        & ";Password=" & password & ";Initial Catalog=" & ApplicationConfiguration.DbNameSoftAzi & ";Data Source=" & _
                        host & ";Packet Size=4096"
            myDbStringConnectionCoge = "Persist Security Info=False;User ID=" & login _
                        & ";Password=" & password & ";Initial Catalog=" & ApplicationConfiguration.DbNameSoftCoge & ";Data Source=" & _
                        host & ";Packet Size=4096"

            ReDim myActiveConnections(DIMARRAYCONNECTIONS)
            ReDim myActiveTransactions(DIMARRAYCONNECTIONS)
            For i = 0 To DIMARRAYCONNECTIONS
                myActiveConnections(i) = Nothing
                myActiveTransactions(i) = Nothing
            Next i
            myPosizioniLibere = New Stack
            For i = 0 To DIMARRAYCONNECTIONS
                myPosizioniLibere.Push(i)
            Next
        End Sub
        Public Function beginTransaction(ByVal tipodb As TipoConnessione, Optional ByVal withLock As Boolean = False) As Integer
            Dim mySqlConnection As SqlConnection = Nothing
            Dim mySQlTransaction As SqlTransaction
            Dim index As Integer = -1
            Dim stNNAAAA As String = ApplicationConfiguration.getNNAAAA

            SyncLock (classLock)
                If (myPosizioniLibere.Count <> 0) Then
                    Try

                        Select Case tipodb
                            Case TipoConnessione.dbMaster 'giu040412
                                mySqlConnection = New SqlConnection(myDbStringConnectionMaster)
                            Case TipoConnessione.dbInstall
                                mySqlConnection = New SqlConnection(myDbStringConnectionInstall)
                            Case TipoConnessione.dbOpzioni
                                mySqlConnection = New SqlConnection(myDbStringConnectionOpzioni.Replace("NN", stNNAAAA.Substring(0, 2)))
                            Case TipoConnessione.dbScadenzario
                                mySqlConnection = New SqlConnection(myDbStringConnectionScadenze.Replace("NN", stNNAAAA.Substring(0, 2)))
                            Case TipoConnessione.dbSoftAzi
                                mySqlConnection = New SqlConnection(myDbStringConnectionAzi.Replace("NNAAAA", stNNAAAA))
                            Case TipoConnessione.dbSoftCoge
                                mySqlConnection = New SqlConnection(myDbStringConnectionCoge.Replace("NNAAAA", stNNAAAA))
                        End Select
                        If (mySqlConnection Is Nothing) Then

                        Else
                            mySqlConnection.Open()
                            If withLock = True Then
                                mySQlTransaction = mySqlConnection.BeginTransaction(IsolationLevel.Serializable)
                            Else
                                mySQlTransaction = mySqlConnection.BeginTransaction()
                            End If
                            index = myPosizioniLibere.Pop()
                            myActiveConnections(index) = mySqlConnection
                            myActiveTransactions(index) = mySQlTransaction
                        End If
                    Catch sex As SqlException
                        Throw sex
                    Catch ex As Exception
                        Throw ex
                    End Try
                End If
            End SyncLock
            Return index
        End Function
        Public Sub endTransaction(ByVal index As Integer, ByVal toCommitted As Boolean)
            SyncLock (classLock)
                Try
                    If (toCommitted = True) Then
                        myActiveTransactions(index).Commit()
                    Else
                        myActiveTransactions(index).Rollback()
                    End If
                    myActiveConnections(index).Close()
                    myActiveConnections(index) = Nothing
                    myActiveTransactions(index) = Nothing
                    myPosizioniLibere.Push(index)
                Catch sex As SqlException
                    Throw sex
                Catch ex As Exception
                    Throw ex
                End Try
            End SyncLock
        End Sub
        'giu060612 as Object
        Public Function getConnection(ByVal indexOfTransaction As Object) As SqlConnection
            Return myActiveConnections(indexOfTransaction)
        End Function
        'giu060612 as Object
        Public Function getTransaction(ByVal indexOfTransaction As Object) As SqlTransaction
            Return myActiveTransactions(indexOfTransaction)
        End Function
    End Class
End Namespace