﻿Imports System.Diagnostics
Imports System.Windows.Forms

Friend Class WindowLoggerForm

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.CreateHandle()

    End Sub
End Class

Public Class WindowLogger
    Private WithEvents MainForm As WindowLoggerForm
    Private Class ProcessEx
        Inherits Process

        Public Name As String
    End Class
    Private Processes As New Dictionary(Of String, ProcessEx)
    Private TmpProcess As ProcessEx
    Private CheckBoxLink As CheckBox
    Private LogShown As Boolean = False

    Private _Width As UInteger
    Friend Shared Event WidthChanged(ByVal NewValue As UInteger)
    ''' <summary>
    ''' The width of the log form
    ''' </summary>
    ''' <value>The width in pixels</value>
    ''' <returns>The width in pixels</returns>
    ''' <remarks></remarks>
    Public Property Width As UInteger
        Get
            Return _Width
        End Get
        Set(value As UInteger)
            _Width = value
            RaiseEvent WidthChanged(_Width)
        End Set
    End Property

    Private _Height As UInteger
    Friend Shared Event HeightChanged(ByVal NewValue As UInteger)
    ''' <summary>
    ''' The height of the log form
    ''' </summary>
    ''' <value>The height in pixels</value>
    ''' <returns>The height in pixels</returns>
    ''' <remarks></remarks>
    Public Property Height As UInteger
        Get
            Return _Height
        End Get
        Set(value As UInteger)
            _Height = value
            RaiseEvent HeightChanged(_Height)
        End Set
    End Property

    Private _X As UInteger
    Friend Shared Event XChanged(ByVal NewValue As UInteger)
    ''' <summary>
    ''' The distance from the left side of the screen
    ''' </summary>
    ''' <value>The distance in pixels</value>
    ''' <returns>The distance in pixels</returns>
    ''' <remarks></remarks>
    Public Property X As UInteger
        Get
            Return _X
        End Get
        Set(value As UInteger)
            _X = value
            RaiseEvent XChanged(_X)
        End Set
    End Property

    Private _Y As UInteger
    Friend Shared Event YChanged(ByVal NewValue As UInteger)
    ''' <summary>
    ''' The distance from the top of the screen
    ''' </summary>
    ''' <value>The distance in pixels</value>
    ''' <returns>The distance in pixels</returns>
    ''' <remarks></remarks>
    Public Property Y As UInteger
        Get
            Return _Y
        End Get
        Set(value As UInteger)
            _Y = value
            RaiseEvent YChanged(_Y)
        End Set
    End Property

    ''' <summary>
    ''' Constucts a new logger form. Multiple can be created at once.
    ''' </summary>
    ''' <param name="show">Whether to show to form straight away</param>
    ''' <remarks></remarks>
    Public Sub New(Optional ByVal show As Boolean = True)
        MainForm = New WindowLoggerForm()
        AddHandler WidthChanged, AddressOf Width_Changed
        AddHandler HeightChanged, AddressOf Height_Changed
        AddHandler XChanged, AddressOf X_Changed
        AddHandler YChanged, AddressOf Y_Changed
        If show Then Me_Show()
    End Sub

    Private Sub Me_Show()
        MainForm.Show()
        LogShown = True
        SetCheckBoxChecked()
    End Sub

    Private Sub Me_Hide()
        LogShown = False
        SetCheckBoxChecked()
        MainForm.Hide()
    End Sub

    Private Sub WindowLogger_Closing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MainForm.FormClosing
        e.Cancel = True
        Me_Hide()
    End Sub

    Private Sub SetCheckBoxChecked()
        If CheckBoxLink IsNot Nothing Then
            CheckBoxLink.Checked = LogShown
        End If
    End Sub

    Private Sub Width_Changed(ByVal NewValue As UInteger)
        MainForm.Width = NewValue
    End Sub

    Private Sub Height_Changed(ByVal NewValue As UInteger)
        MainForm.Height = NewValue
    End Sub

    Private Sub X_Changed(ByVal NewValue As UInteger)
        MainForm.Left = NewValue
    End Sub

    Private Sub Y_Changed(ByVal NewValue As UInteger)
        MainForm.Top = NewValue
    End Sub

    ''' <summary>
    ''' Starts the process specified and records any log to the window.
    ''' </summary>
    ''' <param name="uniqueName">A reference name, allows for checking and stopping the process prematurely</param>
    ''' <param name="proc">The actual file to execute</param>
    ''' <param name="args">Any command line aruments you want to give the process</param>
    ''' <returns>Whether the process was started</returns>
    ''' <remarks>If there was a problem running the process, then no error will occur, only a Boolean return will say if the process started successfully.</remarks>
    Public Function AddProcess(ByVal uniqueName As String, ByVal proc As String, Optional ByVal args As String = "") As Boolean
        TmpProcess = New ProcessEx()

        Dim procSplit As String() = proc.Split(".")
        If procSplit.Length > 1 Then
            Dim ext As String = procSplit(procSplit.Length - 1)
            If ext.ToLower() <> "exe" Then
                args = """" + proc + """" + args
                proc = GetAssociatedProgram(ext)
            End If
        End If

        With TmpProcess
            .Name = uniqueName
            .StartInfo.FileName = proc
            .StartInfo.Arguments = args

            .StartInfo.RedirectStandardOutput = True
            .StartInfo.RedirectStandardError = True
            .EnableRaisingEvents = True
            Application.DoEvents()
            .StartInfo.CreateNoWindow = True
            .StartInfo.UseShellExecute = False

            AddHandler .ErrorDataReceived, AddressOf proc_DataReceived
            AddHandler .OutputDataReceived, AddressOf proc_DataReceived
            AddHandler .Exited, AddressOf proc_Exited
            Try
                .Start()
            Catch ex As Exception
                Return False
            End Try
            .BeginErrorReadLine()
            .BeginOutputReadLine()
            '.WaitForExit()
        End With

        Processes.Add(uniqueName, TmpProcess)
        Return True
    End Function

    ''' <summary>
    ''' Checks whether the process is still running or not.
    ''' </summary>
    ''' <param name="uniqueName">The reference name use when calling 'AddProcess'</param>
    ''' <returns>Whether the process is still running or not.</returns>
    ''' <remarks></remarks>
    Public Function ProcessExists(ByVal uniqueName As String) As Boolean
        Return Processes.ContainsKey(uniqueName)
    End Function

    ''' <summary>
    ''' Stops and removes a running process.
    ''' </summary>
    ''' <param name="uniqueName">The process name</param>
    ''' <remarks>Will throw InvalidConstraintException if the name doesn't exist.</remarks>
    Public Sub RemoveProcess(ByVal uniqueName As String)
        If Processes.ContainsKey(uniqueName) Then
            TmpProcess = Processes.Item(uniqueName)
            TmpProcess.Close()
            Processes.Remove(uniqueName)
        Else
            Throw New InvalidConstraintException("A process by the name doesn't exist.")
        End If
    End Sub

    Private Delegate Sub UpdateRichTextBoxDelgSub(ByVal text As String)
    Private UpdateRichTextBoxDelg As UpdateRichTextBoxDelgSub = New UpdateRichTextBoxDelgSub(AddressOf Print)

    ''' <summary>
    ''' Will print a line to the log.
    ''' </summary>
    ''' <param name="text">The text to print (newline not needed)</param>
    ''' <remarks></remarks>
    Public Sub Print(ByVal text As String)
        MainForm.rtb_log.AppendText(text)
    End Sub

    Private Sub proc_DataReceived(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        If e.Data IsNot Nothing Then MainForm.Invoke(UpdateRichTextBoxDelg, e.Data + Environment.NewLine)
    End Sub

    Private Sub proc_Exited(ByVal sender As Object, ByVal e As EventArgs)
        Processes.Remove(sender.Name)
    End Sub

    ''' <summary>
    ''' Shows the logger form.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ShowLog()
        Me_Show()
    End Sub

    ''' <summary>
    ''' Hides the logger form.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub HideLog()
        Me_Hide()
    End Sub

    ''' <summary>
    ''' Sets the checkbox the will link to the form, so it will update the shown status and be updated.
    ''' </summary>
    ''' <param name="ckbx">The checkbox</param>
    ''' <remarks>This allows for quick and simple implementation of the logging window, as all that is needed is a checkbox, and the logging window will hide when it is unchecked, show when it is checked, and it will uncheck it when the window is closed manually.</remarks>
    Public Sub SetCheckBox(ByVal ckbx As CheckBox)
        CheckBoxLink = ckbx
        AddHandler CheckBoxLink.CheckedChanged, AddressOf ckbx_CheckedChangedHandle
        AddHandler CheckBoxLink.Disposed, AddressOf ckbx_DisposedHandle
        SetCheckBoxChecked()
    End Sub

    ''' <summary>
    ''' Unsets the checkbox currently linked to the form.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UnsetCheckBox()
        RemoveHandler CheckBoxLink.CheckedChanged, AddressOf ckbx_CheckedChangedHandle
        RemoveHandler CheckBoxLink.Disposed, AddressOf ckbx_DisposedHandle
        CheckBoxLink = Nothing
    End Sub

    Private Sub ckbx_CheckedChangedHandle(ByVal sender As Object, ByVal e As EventArgs)
        MainForm.Invoke(ckbx_CheckedChangedDelg, sender.Checked)
    End Sub

    Private Delegate Sub ckbx_CheckedChangedDelgSub(ByVal checked As Boolean)
    Private ckbx_CheckedChangedDelg As New ckbx_CheckedChangedDelgSub(AddressOf ckbx_CheckedChanged)
    Private Sub ckbx_CheckedChanged(ByVal checked As Boolean)
        If CheckBoxLink.Checked Then
            ShowLog()
        Else
            HideLog()
        End If
    End Sub

    Private Sub ckbx_DisposedHandle(ByVal sender As Object, ByVal e As EventArgs)
        MainForm.Invoke(ckbx_DisposedDelg)
    End Sub

    Private Delegate Sub ckbx_DisposedDelgSub()
    Private ckbx_DisposedDelg As New ckbx_DisposedDelgSub(AddressOf ckbx_Disposed)
    Private Sub ckbx_Disposed()
        UnsetCheckBox()
    End Sub
End Class