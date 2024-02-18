Option Strict On
Option Explicit On

Imports System.IO
Imports System.Data.SqlClient

Namespace It.SoftAzi.SystemFramework

    Public Class FileSystemUtil
        Public Shared INVALID_DATE As DateTime = CDate("01/01/1900")

        Public Sub createDir(ByVal strPath As String)
            Dim di As DirectoryInfo = Directory.CreateDirectory(strPath)
        End Sub
        Public Function existFile(ByVal strPath As String) As Boolean
            Return File.Exists(strPath)
        End Function

        Public Sub deleteFile(ByVal strPath As String)
            File.Delete(strPath)
        End Sub
        Public Function getFileName(ByVal strPath As String) As String
            Dim strTok As String()
            strTok = strPath.Split("\"c)
            Return strTok(strTok.Length - 1)
        End Function
        Public Function getFileNameBack(ByVal strPath As String) As String
            Dim strTok As String()
            strTok = strPath.Split("/"c)
            Return strTok(strTok.Length - 1)
        End Function
        Public Shared Function setParameter(ByVal param As SqlParameter, ByRef theSqlCommand As SqlCommand, _
                                        ByVal nomeColonna As String, ByVal tipo As SqlDbType, _
                                        ByVal valore As Object, Optional ByVal len As Integer = -1) As SqlParameter

            If (len = -1) Then
                param = theSqlCommand.Parameters.Add(nomeColonna, tipo)
            Else
                param = theSqlCommand.Parameters.Add(nomeColonna, tipo, len)
            End If
            param.Direction = ParameterDirection.Input
            param.Value = valore
            setParameter = param
        End Function
        Public Shared Function getStringParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer) As String
            If (theSqlDataReader.IsDBNull(index)) Then
                getStringParameter = String.Empty
            Else
                getStringParameter = theSqlDataReader.GetString(index)
            End If
        End Function
        Public Shared Function getDataTimeParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer) As Date
            If (theSqlDataReader.IsDBNull(index)) Then
                getDataTimeParameter = INVALID_DATE
            Else
                getDataTimeParameter = theSqlDataReader.GetDateTime(index)
            End If
        End Function
        Public Shared Function getDecimalParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer) As Decimal
            If (theSqlDataReader.IsDBNull(index)) Then
                getDecimalParameter = 0
            Else
                getDecimalParameter = theSqlDataReader.GetDecimal(index)
            End If
        End Function
        Public Shared Function getDoubleParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer) As Double
            If (theSqlDataReader.IsDBNull(index)) Then
                getDoubleParameter = 0
            Else
                getDoubleParameter = theSqlDataReader.GetDouble(index)
            End If
        End Function
        Public Shared Function getByteParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer) As Byte
            If (theSqlDataReader.IsDBNull(index)) Then
                getByteParameter = 0
            Else
                getByteParameter = theSqlDataReader.GetByte(index)
            End If
        End Function
        Public Shared Function GetFloatParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer) As Single
            If (theSqlDataReader.IsDBNull(index)) Then
                GetFloatParameter = 0
            Else
                GetFloatParameter = theSqlDataReader.GetFloat(index)
            End If
        End Function
        Public Shared Function GetSingleParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer) As Single
            If (theSqlDataReader.IsDBNull(index)) Then
                GetSingleParameter = 0
            Else
                GetSingleParameter = theSqlDataReader.GetFloat(index)
            End If
        End Function
        Public Shared Function getIntegerParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer, Optional ByVal tipoInt As Integer = 32) As Integer
            If (theSqlDataReader.IsDBNull(index)) Then
                getIntegerParameter = 0
            Else
                Select Case tipoInt
                    Case 16
                        getIntegerParameter = theSqlDataReader.GetInt16(index)
                    Case 32
                        getIntegerParameter = theSqlDataReader.GetInt32(index)
                    Case Else
                        getIntegerParameter = theSqlDataReader.GetInt32(index)
                End Select
            End If
        End Function
        Public Shared Function getBooleanParameter(ByVal theSqlDataReader As SqlDataReader, ByVal index As Integer) As Boolean
            If (theSqlDataReader.IsDBNull(index)) Then
                getBooleanParameter = False
            Else
                getBooleanParameter = theSqlDataReader.GetBoolean(index)
            End If
        End Function

    End Class

End Namespace
