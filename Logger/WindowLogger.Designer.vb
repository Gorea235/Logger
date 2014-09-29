<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Friend Class WindowLoggerForm '''''''''''
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.rtb_log = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        'rtb_log
        '
        Me.rtb_log.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtb_log.Location = New System.Drawing.Point(0, 0)
        Me.rtb_log.Name = "rtb_log"
        Me.rtb_log.ReadOnly = True
        Me.rtb_log.Size = New System.Drawing.Size(413, 278)
        Me.rtb_log.TabIndex = 0
        Me.rtb_log.Text = ""
        '
        'WindowLoggerForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(413, 278)
        Me.Controls.Add(Me.rtb_log)
        Me.Name = "WindowLoggerForm"
        Me.Text = "Log"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rtb_log As System.Windows.Forms.RichTextBox
End Class
