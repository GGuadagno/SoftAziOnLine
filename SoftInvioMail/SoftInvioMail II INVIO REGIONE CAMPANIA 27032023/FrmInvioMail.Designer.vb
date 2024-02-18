<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmInvioMail
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
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

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla nell'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmInvioMail))
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar
        Me.LblInfoPacchetto = New System.Windows.Forms.Label
        Me.LblInvio = New System.Windows.Forms.Label
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.btnChiudi = New System.Windows.Forms.Button
        Me.Button1 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(12, 47)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(543, 22)
        Me.ProgressBar1.TabIndex = 11
        '
        'LblInfoPacchetto
        '
        Me.LblInfoPacchetto.Location = New System.Drawing.Point(12, 72)
        Me.LblInfoPacchetto.Name = "LblInfoPacchetto"
        Me.LblInfoPacchetto.Size = New System.Drawing.Size(543, 23)
        Me.LblInfoPacchetto.TabIndex = 12
        Me.LblInfoPacchetto.Text = "Info pacchetto"
        Me.LblInfoPacchetto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblInvio
        '
        Me.LblInvio.Location = New System.Drawing.Point(9, 9)
        Me.LblInvio.Name = "LblInvio"
        Me.LblInvio.Size = New System.Drawing.Size(543, 23)
        Me.LblInvio.TabIndex = 13
        Me.LblInvio.Text = "Invio mail ..."
        Me.LblInvio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "Gestione invio E-mail"
        Me.NotifyIcon1.Visible = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 60000
        '
        'btnChiudi
        '
        Me.btnChiudi.Location = New System.Drawing.Point(480, 110)
        Me.btnChiudi.Name = "btnChiudi"
        Me.btnChiudi.Size = New System.Drawing.Size(75, 24)
        Me.btnChiudi.TabIndex = 14
        Me.btnChiudi.Text = "Chiudi"
        Me.btnChiudi.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 110)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(111, 24)
        Me.Button1.TabIndex = 15
        Me.Button1.Text = "Controllo Invio ora"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'FrmInvioMail
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(569, 146)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnChiudi)
        Me.Controls.Add(Me.LblInvio)
        Me.Controls.Add(Me.LblInfoPacchetto)
        Me.Controls.Add(Me.ProgressBar1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "FrmInvioMail"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Gestione invio E-mail"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents LblInfoPacchetto As System.Windows.Forms.Label
    Friend WithEvents LblInvio As System.Windows.Forms.Label
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents btnChiudi As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button

End Class
