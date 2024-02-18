Imports System.Data.SqlClient
'giu060612 Imports It.SoftAzi.Model.BusinessEntity
Namespace It.SoftAzi.Integration.Dao.SQLDao
    Public Class SQLBaseDAO
        'giu060612 As Integer
        Public Const VINCOLO_NOT_NULL As Integer = 201
        Public Const VINCOLO_UNIQUE As Integer = 2627
        Public Const VINCOLO_CHECK As Integer = 547
        Public Const OVERFLOW As Integer = 8114
        Protected Function getCommnand(ByVal idTransazione As Integer) As SqlCommand
            Dim theDataSource As DataSource
            Dim theSqlConnection As SqlConnection
            Dim theSqlTransaction As SqlTransaction
            Dim theSqlCommand As SqlCommand = Nothing
            Try
                theDataSource = DataSource.getDataSource
                If (idTransazione >= 0) Then
                    theSqlConnection = theDataSource.getConnection(idTransazione)
                    theSqlTransaction = theDataSource.getTransaction(idTransazione)
                    theSqlCommand = theSqlConnection.CreateCommand
                    theSqlCommand.CommandTimeout = 12000
                    theSqlCommand.Connection = theSqlConnection
                    theSqlCommand.Transaction = theSqlTransaction
                End If
            Catch sex As SqlException
                Throw sex
            End Try
            Return theSqlCommand
        End Function
        Protected Function setStringToDB(ByRef stringaToPars As String) As String
            Dim myStringa As String
            myStringa = stringaToPars
            myStringa.Replace("'", "''")
            Return myStringa
        End Function
        'Protected Function getEsitoCod(ByVal errore As Integer, Optional ByRef addedDescription As String = "") As EsitoData
        '    Dim myEsitoData As New EsitoData
        '    myEsitoData.Cod = errore

        '    If (errore = 0) Then
        '    ElseIf (errore = -1) Then
        '        myEsitoData.Descrizione = "Una o piu' entita' referenziate non esistono su STOnLine"
        '    ElseIf (errore = -2) Then
        '        myEsitoData.Descrizione = "Errore logico nei dati"
        '    ElseIf (errore = -3) Then
        '        myEsitoData.Descrizione = "Errore logico nei dati che puo' essere forzato"
        '    ElseIf (errore = -4) Then
        '        myEsitoData.Descrizione = "Entita' duplicata"
        '    ElseIf (errore = -5) Then
        '        myEsitoData.Descrizione = "L'elemento non può essere eliminato in quanto esistono alcuni record ad esso correlati."
        '    End If
        '    If addedDescription <> "" Then
        '        myEsitoData.Descrizione = myEsitoData.Descrizione & ": " & addedDescription
        '    End If
        '    Return myEsitoData
        'End Function
    End Class
End Namespace