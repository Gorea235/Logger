Imports System.IO
Public Class MainLogger
    Private Class LoggerClass
        Public Stream As StreamWriter
        Public Prefix As String
        Public AddTime As Boolean
        Public AddDate As Boolean
    End Class
    Private currentLogs As Dictionary(Of Integer, LoggerClass)
    Private latestLog As Integer = 0

    ''' <summary>
    ''' Will create a new 'Logger' instance and return the ID
    ''' </summary>
    ''' <param name="LogLocation">The location of the log file, can an existing file, or it can create a new one. You can log on the same file at the same time using multiple loggers.</param>
    ''' <param name="LogPrefix">The prefix of each log (e.g. 'Log: ' followed by the rest of the log)</param>
    ''' <param name="AddTime">Whether to add the time after the prefix, and before the log</param>
    ''' <param name="AddDate">Whether to add the date after the prefix and time (time not needed), and before the log</param>
    ''' <returns>LogID as Integer</returns>
    ''' <remarks>The same file can have multiple loggers on it at once, allowing different prefixes etc.</remarks>
    Public Function NewLog(ByVal LogLocation As String, Optional ByVal LogPrefix As String = "", Optional ByVal AddTime As Boolean = True, Optional ByVal AddDate As Boolean = True)
        latestLog = latestLog + 1
        Dim NewLogger As New LoggerClass
        NewLogger.Stream = New StreamWriter(LogLocation)
        NewLogger.Prefix = LogPrefix
        NewLogger.AddTime = AddTime
        NewLogger.AddDate = AddDate
        currentLogs.Add(latestLog, NewLogger)
        Return latestLog
    End Function

    ''' <summary>
    ''' Will log a string into the log file using the logger ID and a string.
    ''' </summary>
    ''' <param name="LoggerID">The ID of the logger</param>
    ''' <param name="LogString">The string to log</param>
    ''' <returns>The full string that was logged</returns>
    ''' <remarks></remarks>
    Public Function Log(ByVal LoggerID As Integer, ByVal LogString As String)
        If Not currentLogs.ContainsKey(LoggerID) Then
            Throw New Exception("Logger " & LoggerID & " does not exist!")
        End If
        Dim CurrentLogger As LoggerClass = currentLogs(LoggerID)
        Dim StringToLog As String = CurrentLogger.Prefix & " "
        If CurrentLogger.AddDate Then
            StringToLog = StringToLog & "[" & Today.ToShortDateString & "] "
        End If
        If CurrentLogger.AddTime Then
            StringToLog = StringToLog & "[" & Today.ToShortTimeString & "] "
        End If
        StringToLog = StringToLog & LogString
        CurrentLogger.Stream.WriteLine(StringToLog)
        Return StringToLog
    End Function

    ''' <summary>
    ''' Will dispose of the logger
    ''' </summary>
    ''' <param name="LoggerID">The ID of the logger to dispose</param>
    ''' <remarks></remarks>
    Public Sub DisposeLogger(ByVal LoggerID As Integer)
        If Not currentLogs.ContainsKey(LoggerID) Then
            Throw New Exception("Logger " & LoggerID & " does not exist!")
        End If
        Dim CurrentLogger As LoggerClass = currentLogs(LoggerID)
        CurrentLogger.Stream.Dispose()
        currentLogs.Remove(LoggerID)
    End Sub
End Class
