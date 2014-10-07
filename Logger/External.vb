Friend Module External
    Public Function GetAssociatedProgram(ByVal FileExtension As _
       String) As String
        ' Returns the application associated with the specified
        ' FileExtension
        ' ie, path\denenv.exe for "VB" files
        Dim objExtReg As Microsoft.Win32.RegistryKey = _
             Microsoft.Win32.Registry.ClassesRoot
        Dim objAppReg As Microsoft.Win32.RegistryKey = _
            Microsoft.Win32.Registry.ClassesRoot
        Dim strExtValue As String
        Try
            ' Add trailing period if doesn't exist
            If FileExtension.Substring(0, 1) <> "." Then _
                FileExtension = "." & FileExtension
            ' Open registry areas containing launching app details
            objExtReg = objExtReg.OpenSubKey(FileExtension.Trim)
            strExtValue = objExtReg.GetValue("")
            objAppReg = objAppReg.OpenSubKey(strExtValue & _
                            "\shell\open\command")
            ' Parse out, tidy up and return result
            Dim SplitArray() As String
            SplitArray = Split(objAppReg.GetValue(Nothing), """")
            If SplitArray(0).Trim.Length > 0 Then
                Return SplitArray(0).Replace("%1", "")
            Else
                Return SplitArray(1).Replace("%1", "")
            End If
        Catch
            Return ""
        End Try
    End Function
End Module
