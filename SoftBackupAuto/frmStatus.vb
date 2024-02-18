Public Class frmStatus
    Inherits System.Windows.Forms.Form


#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lblStato As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmStatus))
        Me.lblStato = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblStato
        '
        Me.lblStato.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStato.Location = New System.Drawing.Point(8, 8)
        Me.lblStato.Name = "lblStato"
        Me.lblStato.Size = New System.Drawing.Size(504, 104)
        Me.lblStato.TabIndex = 0
        Me.lblStato.Text = "Invio e-mail in corso. Attendere ..."
        Me.lblStato.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmStatus
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(520, 117)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.lblStato})
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmStatus"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Stato"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Dim myTitolo As String = ""

    Public WriteOnly Property stMyTitolo() As String
        Set(ByVal Value As String)
            myTitolo = Value
        End Set
    End Property

    Public WriteOnly Property _Messaggio() As String
        Set(ByVal Value As String)
            Me.lblStato.Text = Value
        End Set
    End Property

    Private Sub frmStatus_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If myTitolo <> "" Then
            lblStato.Text = myTitolo
        End If
    End Sub
End Class
