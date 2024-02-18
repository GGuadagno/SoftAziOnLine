Option Explicit On 

Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Configuration.ConfigurationSettings

Public Class frmBKAuto
    Inherits System.Windows.Forms.Form

    Dim objDatabase As New clsDatabase()
    Dim objControlli As New clsControlli()
    Dim objBackup As New clsBackupDatabase()

    Dim WD As Integer = Weekday(Now, FirstDayOfWeek.Monday)
    Dim WDPrevisto As Integer = 0
    Dim BkGG As Integer = 0
    Dim Minuti As Integer = 0
    Const MinutiErr As Integer = 60

#Region " Codice generato da Progettazione Windows Form "

    Public Sub New()
        MyBase.New()

        'Chiamata richiesta da Progettazione Windows Form.
        InitializeComponent()

        'Aggiungere le eventuali istruzioni di inizializzazione dopo la chiamata a InitializeComponent()

    End Sub

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form.
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla nell'editor del codice.
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents StatusBar1 As System.Windows.Forms.StatusBar
    Friend WithEvents StatusBarPanel1 As System.Windows.Forms.StatusBarPanel
    Friend WithEvents StatusBarPanel3 As System.Windows.Forms.StatusBarPanel
    Friend WithEvents SqlConnTickets As System.Data.SqlClient.SqlConnection
    Friend WithEvents StatusBarPanel4 As System.Windows.Forms.StatusBarPanel
    Friend WithEvents btnChiudi As System.Windows.Forms.Button
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents txtBKEsito As System.Windows.Forms.TextBox
    Friend WithEvents txtBkOreMinStart As System.Windows.Forms.TextBox
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents btnEseguiBK As System.Windows.Forms.Button
    Friend WithEvents lblStato As System.Windows.Forms.Label
    Friend WithEvents lblNoBK As System.Windows.Forms.Label
    Friend WithEvents lblBKDataOraEsito As System.Windows.Forms.Label
    Friend WithEvents lblCicloOgni As System.Windows.Forms.Label
    Friend WithEvents btnReset As System.Windows.Forms.Button
    Friend WithEvents Timer2 As System.Windows.Forms.Timer
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBKAuto))
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.StatusBar1 = New System.Windows.Forms.StatusBar
        Me.StatusBarPanel1 = New System.Windows.Forms.StatusBarPanel
        Me.StatusBarPanel3 = New System.Windows.Forms.StatusBarPanel
        Me.StatusBarPanel4 = New System.Windows.Forms.StatusBarPanel
        Me.SqlConnTickets = New System.Data.SqlClient.SqlConnection
        Me.btnChiudi = New System.Windows.Forms.Button
        Me.GroupBox4 = New System.Windows.Forms.GroupBox
        Me.lblCicloOgni = New System.Windows.Forms.Label
        Me.lblBKDataOraEsito = New System.Windows.Forms.Label
        Me.lblNoBK = New System.Windows.Forms.Label
        Me.Label51 = New System.Windows.Forms.Label
        Me.txtBKEsito = New System.Windows.Forms.TextBox
        Me.txtBkOreMinStart = New System.Windows.Forms.TextBox
        Me.Label50 = New System.Windows.Forms.Label
        Me.Label48 = New System.Windows.Forms.Label
        Me.lblStato = New System.Windows.Forms.Label
        Me.btnEseguiBK = New System.Windows.Forms.Button
        Me.btnReset = New System.Windows.Forms.Button
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        CType(Me.StatusBarPanel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusBarPanel3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusBarPanel4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox4.SuspendLayout()
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Interval = 900000
        '
        'StatusBar1
        '
        Me.StatusBar1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusBar1.Location = New System.Drawing.Point(0, 259)
        Me.StatusBar1.Name = "StatusBar1"
        Me.StatusBar1.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.StatusBarPanel1, Me.StatusBarPanel3, Me.StatusBarPanel4})
        Me.StatusBar1.ShowPanels = True
        Me.StatusBar1.Size = New System.Drawing.Size(822, 22)
        Me.StatusBar1.TabIndex = 47
        Me.StatusBar1.Text = "StatusBar1"
        '
        'StatusBarPanel1
        '
        Me.StatusBarPanel1.Name = "StatusBarPanel1"
        Me.StatusBarPanel1.Text = "DataOra"
        Me.StatusBarPanel1.Width = 360
        '
        'StatusBarPanel3
        '
        Me.StatusBarPanel3.Alignment = System.Windows.Forms.HorizontalAlignment.Right
        Me.StatusBarPanel3.Name = "StatusBarPanel3"
        Me.StatusBarPanel3.Text = "Backup automatico previsto a partire dalle ore :"
        Me.StatusBarPanel3.Width = 360
        '
        'StatusBarPanel4
        '
        Me.StatusBarPanel4.Alignment = System.Windows.Forms.HorizontalAlignment.Center
        Me.StatusBarPanel4.Name = "StatusBarPanel4"
        Me.StatusBarPanel4.Text = "hh.mm"
        Me.StatusBarPanel4.Width = 90
        '
        'SqlConnTickets
        '
        Me.SqlConnTickets.ConnectionString = "data source=.\SQL2008R2;initial catalog=SoftTickets;integrated security=SSPI;pers" & _
            "ist security info=False;workstation id=GIUSEPPE;packet size=4096"
        Me.SqlConnTickets.FireInfoMessageEventOnUserErrors = False
        '
        'btnChiudi
        '
        Me.btnChiudi.Location = New System.Drawing.Point(685, 22)
        Me.btnChiudi.Name = "btnChiudi"
        Me.btnChiudi.Size = New System.Drawing.Size(120, 40)
        Me.btnChiudi.TabIndex = 48
        Me.btnChiudi.Text = "Arresta Backup CHIUDI"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.lblCicloOgni)
        Me.GroupBox4.Controls.Add(Me.lblBKDataOraEsito)
        Me.GroupBox4.Controls.Add(Me.lblNoBK)
        Me.GroupBox4.Controls.Add(Me.Label51)
        Me.GroupBox4.Controls.Add(Me.txtBKEsito)
        Me.GroupBox4.Controls.Add(Me.txtBkOreMinStart)
        Me.GroupBox4.Controls.Add(Me.Label50)
        Me.GroupBox4.Controls.Add(Me.Label48)
        Me.GroupBox4.Controls.Add(Me.lblStato)
        Me.GroupBox4.Enabled = False
        Me.GroupBox4.Location = New System.Drawing.Point(8, 8)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(808, 242)
        Me.GroupBox4.TabIndex = 49
        Me.GroupBox4.TabStop = False
        '
        'lblCicloOgni
        '
        Me.lblCicloOgni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblCicloOgni.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblCicloOgni.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblCicloOgni.Location = New System.Drawing.Point(8, 64)
        Me.lblCicloOgni.Name = "lblCicloOgni"
        Me.lblCicloOgni.Size = New System.Drawing.Size(232, 24)
        Me.lblCicloOgni.TabIndex = 71
        Me.lblCicloOgni.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBKDataOraEsito
        '
        Me.lblBKDataOraEsito.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblBKDataOraEsito.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblBKDataOraEsito.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblBKDataOraEsito.Location = New System.Drawing.Point(128, 88)
        Me.lblBKDataOraEsito.Name = "lblBKDataOraEsito"
        Me.lblBKDataOraEsito.Size = New System.Drawing.Size(112, 16)
        Me.lblBKDataOraEsito.TabIndex = 68
        Me.lblBKDataOraEsito.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblNoBK
        '
        Me.lblNoBK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblNoBK.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblNoBK.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblNoBK.Location = New System.Drawing.Point(240, 64)
        Me.lblNoBK.Name = "lblNoBK"
        Me.lblNoBK.Size = New System.Drawing.Size(560, 40)
        Me.lblNoBK.TabIndex = 67
        Me.lblNoBK.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label51
        '
        Me.Label51.AutoSize = True
        Me.Label51.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label51.Location = New System.Drawing.Point(8, 88)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(100, 13)
        Me.Label51.TabIndex = 65
        Me.Label51.Text = "Esito ultimo Backup"
        '
        'txtBKEsito
        '
        Me.txtBKEsito.BackColor = System.Drawing.SystemColors.InactiveBorder
        Me.txtBKEsito.Enabled = False
        Me.txtBKEsito.Location = New System.Drawing.Point(8, 104)
        Me.txtBKEsito.MaxLength = 2000
        Me.txtBKEsito.Multiline = True
        Me.txtBKEsito.Name = "txtBKEsito"
        Me.txtBKEsito.Size = New System.Drawing.Size(792, 64)
        Me.txtBKEsito.TabIndex = 64
        '
        'txtBkOreMinStart
        '
        Me.txtBkOreMinStart.Location = New System.Drawing.Point(311, 18)
        Me.txtBkOreMinStart.MaxLength = 5
        Me.txtBkOreMinStart.Name = "txtBkOreMinStart"
        Me.txtBkOreMinStart.Size = New System.Drawing.Size(64, 20)
        Me.txtBkOreMinStart.TabIndex = 63
        Me.txtBkOreMinStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label50
        '
        Me.Label50.AutoSize = True
        Me.Label50.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label50.Location = New System.Drawing.Point(215, 24)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(90, 13)
        Me.Label50.TabIndex = 62
        Me.Label50.Text = "a partire dalle Ore"
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label48.Location = New System.Drawing.Point(16, 24)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(193, 13)
        Me.Label48.TabIndex = 52
        Me.Label48.Text = "Esegui il Backup utomatico ogni  giorno"
        '
        'lblStato
        '
        Me.lblStato.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStato.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.lblStato.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.lblStato.Location = New System.Drawing.Point(8, 168)
        Me.lblStato.Name = "lblStato"
        Me.lblStato.Size = New System.Drawing.Size(792, 64)
        Me.lblStato.TabIndex = 66
        '
        'btnEseguiBK
        '
        Me.btnEseguiBK.Location = New System.Drawing.Point(555, 22)
        Me.btnEseguiBK.Name = "btnEseguiBK"
        Me.btnEseguiBK.Size = New System.Drawing.Size(120, 40)
        Me.btnEseguiBK.TabIndex = 66
        Me.btnEseguiBK.Text = "Esegui Backup adesso"
        '
        'btnReset
        '
        Me.btnReset.Location = New System.Drawing.Point(429, 22)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(120, 40)
        Me.btnReset.TabIndex = 68
        Me.btnReset.Text = "Reset Timer ed Errori"
        '
        'Timer2
        '
        Me.Timer2.Interval = 900000
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "SoftTickets [Backup automatico]"
        '
        'frmBKAuto
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(822, 281)
        Me.Controls.Add(Me.btnReset)
        Me.Controls.Add(Me.btnEseguiBK)
        Me.Controls.Add(Me.btnChiudi)
        Me.Controls.Add(Me.StatusBar1)
        Me.Controls.Add(Me.GroupBox4)
        Me.Cursor = System.Windows.Forms.Cursors.AppStarting
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmBKAuto"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SoftBKAuto  [Backup automatico]"
        CType(Me.StatusBarPanel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusBarPanel3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusBarPanel4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Dim LoadForm As Boolean = True : Dim ErrLoad As Boolean = False

    Private Sub frmBKAuto_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadForm = True : ErrLoad = False
        Try
            SWBKAuto = CBool(getConfigurationString("SWBKAuto"))
        Catch
            SWBKAuto = False
        End Try
        SWBkAttivo = SWBKAuto
        SWBKManuale = Not SWBKAuto
        SWGGEsito = 0
        Me.Cursor = Cursors.AppStarting

        CentraForm(Me, Owner)

        Me.StatusBarPanel1.Text = Format(Now, "dddd d MMMM yyyy") + " ore: " & Format(Now, "H:mm")

        '----- Controllo Config
        If funLeggiConfig() = False Then ErrLoad = True : Exit Sub
        If funBkPrevisto() = False Then ErrLoad = True : Exit Sub
        
        subImpostaTimer(Minuti)
        Timer2.Interval = 60000 '1minuto --- 
        btnReset.Enabled = Not SWBKManuale
        If SWBKManuale Then
            Me.Cursor = Cursors.Default
            Timer1.Stop()
            Timer2.Stop()
        Else
            Timer1.Start()
            Timer2.Start()
            Me.Cursor = Cursors.AppStarting
        End If
        LoadForm = False
    End Sub

    Private Function funLeggiConfig() As Boolean
        Minuti = Val(getConfigurationString("BKAutoMinuti"))
        'solo per bk su supporto removibile
        Dim PercorsoSupportoRemovibile As String = getConfigurationString("SupportoRemovibile")
        If PercorsoSupportoRemovibile.Trim = "" Then
            If SWBKManuale = False Then
                EsitoBKAuto = "ERRORE : Impossibile avviare [Backup automatico] perchè non è stato definito il supporto removibile. Si prega di configurare prima il file App.Config."
            Else
                MsgBox("Impossibile avviare [Backup automatico] perchè non è stato definito il supporto removibile." & vbCrLf + vbCrLf & "Si prega di configurare prima il file App.Config.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.Text)
            End If
            Return False
        ElseIf InStr(UCase(PercorsoSupportoRemovibile), "\\") > 0 Then
            If SWBKManuale = False Then
                EsitoBKAuto = "ERRORE : Impossibile avviare [Backup automatico] perchè non sono ammessi percorsi di rete. Si prega di configurare prima il file App.Config."
            Else
                MsgBox("Impossibile avviare [Backup automatico] perchè non sono ammessi percorsi di rete." & vbCrLf + vbCrLf & "Si prega di configurare prima il file App.Config.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.Text)
            End If
            Return False
        End If

        Me.txtBkOreMinStart.Text = getConfigurationString("BKOreMinStart")
        Me.StatusBarPanel4.Text = getConfigurationString("BKOreMinStart")
        '---------
        If Not objControlli.ControllaOre(Me.StatusBarPanel4.Text) Then
            If SWBKManuale = False Then
                EsitoBKAuto = "ERRORE : Impossibile avviare [Backup automatico]. Ore inizio Backup non definito. Si prega di configurare prima il file Config."
            Else
                MsgBox("Impossibile avviare [Backup automatico]. Ore inizio Backup non definito." & vbCrLf + vbCrLf & "Si prega di configurare prima il file Config.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.Text)
            End If
            Return False
        End If

        'giu040909 
        'giu240909 i Path devono puntare tutti su Locale senza percorsi di rete
        Dim SWPath As Boolean = True
        If getConfigurationString("PercorsoFilesBackup") = "" Then
            SWPath = False
        ElseIf InStr(UCase(getConfigurationString("PercorsoFilesBackup")), "\\") > 0 Then
            SWPath = False
        End If
        '-
        If getConfigurationString("PercorsoFlash_1") = "" Then
            ' ''SWPath = False
        ElseIf InStr(UCase(getConfigurationString("PercorsoFlash_1")), "\\") > 0 Then
            SWPath = False
        End If
        If getConfigurationString("PercorsoFlash_2") = "" Then
            ' ''SWPath = False
        ElseIf InStr(UCase(getConfigurationString("PercorsoFlash_2")), "\\") > 0 Then
            SWPath = False
        End If
        '-
        If getConfigurationString("PosizioneBackupFlash_1") = "" Then
            ' ''SWPath = False
        ElseIf InStr(UCase(getConfigurationString("PosizioneBackupFlash_1")), "\\") > 0 Then
            SWPath = False
        End If
        If getConfigurationString("PosizioneBackupFlash_2") = "" Then
            ' ''SWPath = False
        ElseIf InStr(UCase(getConfigurationString("PosizioneBackupFlash_2")), "\\") > 0 Then
            SWPath = False
        End If
        '-
        If SWPath = False Then
            If SWBKManuale = False Then
                EsitoBKAuto = "ERRORE : Impossibile avviare [Backup automatico]. Percorso DB/Flash1-2 non definiti oppure sono definiti con percorsi di rete. Si prega di configurare prima il file App.Config."
            Else
                MsgBox("Impossibile avviare [Backup automatico]. Percorso DB/Flash1-2 non definiti oppure sono definiti con percorsi di rete." & vbCrLf + vbCrLf & "Si prega di configurare prima il file App.Config.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.Text)
            End If
            Return False
        End If

        Return True
    End Function

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Me.StatusBarPanel1.Text = Format(Now, "dddd d MMMM yyyy") + " ore: " & Format(Now, "H:mm")
        lblBKDataOraEsito.Text = Format(Now, FormatoData) + " " & Format(Now, FormatoOra)
        If SWGGEsito = 1 Then
            Visualizzami()
            lblNoBK.Text = "ERRORE Backup precedente"
            AggChiaveOP(lblNoBK.Text)
            WDPrevisto = 0
            EsitoBKAuto = ""
            subImpostaTimer(MinutiErr)
            Exit Sub
        Else
            subImpostaTimer(Minuti)
        End If
        If SWBkAttivo = False Then
            Visualizzami()
            lblNoBK.Text = "Backup automatico sospeso"
            AggChiaveOP(lblNoBK.Text)
            WDPrevisto = 0
            EsitoBKAuto = ""
            Exit Sub
        Else
            subImpostaTimer(Minuti)
        End If

        SWGGEsito = 2
        EsitoBKAuto = "" : lblNoBK.Text = ""
        '--------------------------------------------------------
        If SWBkConfig = True Then 'giu310809 per far ricaricare le configurazioni a BKAuto
            SWBkConfig = False
            '----- Controllo Config
            If funLeggiConfig() = False Then
                Visualizzami()
                lblStato.Text = EsitoBKAuto
                lblNoBK.Text = "Backup non eseguito : ERRORE Config - Giorno previsto per il Backup"
                Dim str As String = ""
                If EsitoBKAuto.Length > 3 Then str = EsitoBKAuto.Substring(0, 3)
                If UCase(str) = "ERR" Then
                    EsitoBKAuto = EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
                Else
                    EsitoBKAuto = "ERRORE : " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
                End If
                lblStato.Text = EsitoBKAuto
                ScriviFileLog(EsitoBKAuto)
                AggChiaveOP(EsitoBKAuto)
                '-----
                subImpostaTimer(MinutiErr)
                Exit Sub
            Else
                subImpostaTimer(Minuti)
            End If
        End If
        '--------------------------------------------------------
        Dim BkOre As Integer = Mid(StatusBarPanel4.Text, 1, 2)
        Dim BkMin As Integer = Mid(StatusBarPanel4.Text, 4, 2)
        WD = Weekday(Now, FirstDayOfWeek.Monday)
        If funBkPrevisto() = False Then
            Visualizzami()
            lblNoBK.Text = "ERRORE Config - Giorno previsto per il Backup"
            ScriviFileLog(lblNoBK.Text)
            AggChiaveOP(lblNoBK.Text)
            WDPrevisto = 0 : SWGGEsito = 1
            EsitoBKAuto = ""
            subImpostaTimer(MinutiErr)
            Exit Sub
        Else
            subImpostaTimer(Minuti)
        End If
        '--------
        If WD <> WDPrevisto Then
            lblNoBK.Text = "Backup non eseguito : giorno non previsto"
            ScriviFileLog(lblNoBK.Text)
            AggChiaveOP(lblNoBK.Text)
            WDPrevisto = 0
            EsitoBKAuto = ""
            subImpostaTimer(MinutiErr)
            Exit Sub
        ElseIf BkGG = 0 Then
            subImpostaTimer(Minuti)
        Else
            lblNoBK.Text = "Backup non eseguito : Eseguito già un Backup in data odierna"
            ScriviFileLog(lblNoBK.Text)
            AggChiaveOP(lblNoBK.Text)
            WDPrevisto = 0
            EsitoBKAuto = ""
            subImpostaTimer(MinutiErr)
            Exit Sub
        End If
        If Now.Hour < BkOre Then
            lblNoBK.Text = "Backup non eseguito : ora non prevista"
            AggChiaveOP(lblNoBK.Text)
            WDPrevisto = 0
            EsitoBKAuto = ""
            subImpostaTimer(MinutiErr)
            Exit Sub
        ElseIf Now.Hour = BkOre Then
            If Now.Minute < BkMin Then
                lblNoBK.Text = "Backup non eseguito : minuti non previsti"
                AggChiaveOP(lblNoBK.Text)
                WDPrevisto = 0
                EsitoBKAuto = ""
                Exit Sub
            Else
                subImpostaTimer(Minuti)
            End If
        Else
            subImpostaTimer(Minuti)
        End If
        'ok procedo
        SWBKAuto = True
        Timer1.Stop()
        If getConfigurationString("SoftTickets") <> "" Then
            If BKASoftTickets("SoftTickets", "") = False Then
                GoTo TerminaBKA
            End If
        End If
        Call MainBKDB()
        
TerminaBKA:
        '@@@@@@@@@@@@@@@ OGNI ORA
        Timer1.Interval = 900000 '1
        Timer1.Start()
    End Sub
    'giu1300918
    Private Sub MainBKDB()
        '---------------------------------------------------------------
        'INIZIO BK COGE + AZI @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        Dim strConnDB As String = ""
        Dim strNomeDB As String = ""
        Dim strDitta As String = getConfigurationString("Ditta")
        If strDitta.Trim = "" Then
            ScriviFileLog("* * * ATTENZIONE SALVATAGGIO NON ESEGUITO * * * ERRORE Config: non è stato definito il codice Ditta")
            GoTo TerminaBKA
        End If
        Dim AnniBK As Integer = 0
        Try
            AnniBK = getConfigurationString("AnniBK")
        Catch ex As Exception
            AnniBK = 2 'DEFAULT
            ScriviFileLog("* * * ATTENZIONE * * * Config: non è stato definito il codice AnniBK, impostato a 2 anni")
        End Try
        strNomeDB = strDitta & "Install"
        strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
        Dim UltimoEser As String = ""
        Dim PrimoEser As String = ""
        Try
            UltimoEser = objBackup.GetUltimoEser(strConnDB, strDitta, AnniBK)
            If Not IsNumeric(UltimoEser.Trim) Then
                ScriviFileLog("* * * ATTENZIONE SALVATAGGIO NON ESEGUITO * * * ERRORE Lettura esercizi: " & UltimoEser)
                GoTo TerminaBKA
            End If
            PrimoEser = Format(CInt(UltimoEser.Trim) - AnniBK, "0000")
        Catch ex As Exception
            ScriviFileLog("* * * ATTENZIONE SALVATAGGIO NON ESEGUITO * * * ERRORE Lettura esercizi: " & ex.Message)
            GoTo TerminaBKA
        End Try
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@
        'Prima salvo i DB (NN) e poi nel ciclo gli ESERCIZI
        strNomeDB = strDitta & "Install"
        strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
        If BKASoftTickets(strNomeDB, strConnDB) = False Then
            GoTo TerminaBKA
        End If
        strNomeDB = strDitta & "Scadenze"
        strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
        If BKASoftTickets(strNomeDB, strConnDB) = False Then
            GoTo TerminaBKA
        End If
        strNomeDB = strDitta & "Opzioni"
        strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
        If BKASoftTickets(strNomeDB, strConnDB) = False Then
            GoTo TerminaBKA
        End If
        'OK procedo con il ciclo BK salvataggio Esercizi
        Dim NextEser As String = PrimoEser
        While NextEser <> UltimoEser 'fino all'anno precedente 
            strNomeDB = strDitta & NextEser & "CoGe"
            strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
            If BKASoftTickets(strNomeDB, strConnDB) = False Then
                GoTo TerminaBKA
            End If
            strNomeDB = strDitta & NextEser & "GestAzi"
            strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
            If BKASoftTickets(strNomeDB, strConnDB) = False Then
                GoTo TerminaBKA
            End If
            'aggiorno Data salvataggio
            strNomeDB = strDitta & "Install"
            strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
            If objBackup.UpdateDataBk(strNomeDB, strConnDB, strDitta & NextEser) = False Then
                GoTo TerminaBKA
            End If
            '-ok prossimo
            NextEser = Format(CInt(NextEser.Trim) + 1, "0000")
        End While
        'Eseguo l'anno corrente
        strNomeDB = strDitta & UltimoEser & "CoGe"
        strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
        If BKASoftTickets(strNomeDB, strConnDB) = False Then
            GoTo TerminaBKA
        End If
        strNomeDB = strDitta & UltimoEser & "GestAzi"
        strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
        If BKASoftTickets(strNomeDB, strConnDB) = False Then
            GoTo TerminaBKA
        End If
        'aggiorno Data salvataggio
        strNomeDB = strDitta & "Install"
        strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
        If objBackup.UpdateDataBk(strNomeDB, strConnDB, strDitta & UltimoEser) = False Then
            GoTo TerminaBKA
        End If
        'GIU120918 QUI INSERISCO LE OPERAZIONI DOPO IL SALVATAGGIO
        If getConfigurationString("MySQLTrasfDoc") = "1" Then
            ScriviFileLog("INIZIO trasferimento documenti per CRM Appuntamenti Ultimo Esercizio: " & strDitta & UltimoEser)
            strNomeDB = strDitta & UltimoEser & "GestAzi"
            strConnDB = getConfigurationString("SQLServer") & ";database=" & strNomeDB & ";user id=sa;password=" & Decripta() & ";Timeout=" & SqlTimeout()
            If objBackup.MySQLTrasfDoc(strConnDB) = False Then
                ScriviFileLog("* * * ERRORE * * * trasferimento documenti per CRM Appuntamenti Ultimo Esercizio: " & strDitta & UltimoEser)
            Else
                ScriviFileLog("OK TERMINATO con successo trasferimento documenti per CRM Appuntamenti Ultimo Esercizio: " & strDitta & UltimoEser)
            End If
        Else
            ScriviFileLog("* * * SOSPESO * * * servizio trasferimento documenti per CRM AppuntamentiUltimo Esercizio: " & strDitta & UltimoEser)
        End If

TerminaBKA:
        '- QUI AL TERMINE DI TUTTO COPIO IL LOG PER INVIO EMAIL
        Call SendLogBK()
    End Sub

    Private Function BKASoftTickets(ByVal TipoDatabase As String, ByVal ConnDatabase As String) As Boolean
        Me.Cursor = Cursors.WaitCursor
        Me.Enabled = False
        SWBkAttivo = False
        Timer1.Stop()

        If funEseguiBackup(TipoDatabase, ConnDatabase) = False Then
            SWGGEsito = 1
            Visualizzami()
        End If

        Timer1.Start()
        SWBkAttivo = True
        Me.Enabled = True
        Me.Cursor = Cursors.AppStarting

        Dim DataOraEsito As String = Format(Now, "dddd d MMMM yyyy") + " ore: " & Format(Now, "H:mm")
        If SWGGEsito = 1 Then
            Visualizzami()
            Dim str As String = ""
            If EsitoBKAuto.Length > 3 Then str = EsitoBKAuto.Substring(0, 3)
            If UCase(str) = "ERR" Then
                EsitoBKAuto = DataOraEsito & " - " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            Else
                EsitoBKAuto = DataOraEsito & " - " & "ERRORE : " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            End If
            subImpostaTimer(MinutiErr)
        Else
            EsitoBKAuto = DataOraEsito & " - " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            subImpostaTimer(Minuti)
        End If
        lblStato.Text = EsitoBKAuto
        '-----
        '----- Controllo Config
        Dim Ok As Boolean = True
        If funLeggiConfig() = False Then Ok = False
        If funBkPrevisto() = False Then Ok = False
        If Ok = False Then
            Visualizzami()
            SWGGEsito = 1
            lblStato.Text = EsitoBKAuto
            subImpostaTimer(MinutiErr)
        End If
        If SWGGEsito = 1 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub subImpostaTimer(ByVal Minuti As Integer)
        Timer1.Stop()
        Timer1.Interval = 900000 '15minuti --- 60000 '1minuto --- 
        If Minuti = 0 Then Minuti = 60
        Timer1.Interval = Minuti * 60000
        If SWBkAttivo = False And SWBKManuale = False Then
            lblCicloOgni.Text = "Backup automatico SOSPESO"
            AggChiaveOP(lblCicloOgni.Text)
            Timer1.Stop()
        ElseIf SWBKManuale Then
            lblCicloOgni.Text = "Backup MANUALE"
            AggChiaveOP(lblCicloOgni.Text)
            Timer1.Stop()
        Else
            lblCicloOgni.Text = "Controllo ogni : " & Minuti.ToString & " minuti"
            AggChiaveOP(lblCicloOgni.Text)
            Timer1.Start()
        End If
    End Sub

    Private Function funBkPrevisto() As Boolean
        WD = Weekday(Now, FirstDayOfWeek.Monday)
        BkGG = 0
        WDPrevisto = Weekday(Now, FirstDayOfWeek.Monday)
        Try
            If System.IO.File.Exists(getConfigurationString("PercorsoFilesBackup") + "\LogBKAuto" & Format(Now, "yyyyMMdd") & ".txt") = True Then
                BkGG = Weekday(Now, FirstDayOfWeek.Monday)
                WDPrevisto = Weekday(DateAdd(DateInterval.Day, 1, Now), FirstDayOfWeek.Monday)
            End If
        Catch
        End Try
        '-
        If SWBkAttivo = False And SWBKManuale = False Then
            Me.StatusBarPanel3.Text = "Backup automatico SOSPESO"
            AggChiaveOP(Me.StatusBarPanel3.Text)
        ElseIf SWBKManuale Then
            Me.StatusBarPanel3.Text = "Backup MANUALE"
            AggChiaveOP(Me.StatusBarPanel3.Text)
        Else
            Me.StatusBarPanel3.Text = "Bk Previsto per il giorno : " & getGiorno(WDPrevisto) & " alle ore :  "
            AggChiaveOP(Me.StatusBarPanel3.Text)
        End If

        Return True
    End Function

    Private Sub frmBKAuto_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        If ErrLoad = True Then
            SWBkAttivo = False
            End
            Exit Sub
        End If
        If LoadForm = True Or Not Me.Visible Then Exit Sub

        Me.Refresh()
    End Sub

    Private Sub btnChiudi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnChiudi.Click
        AggChiaveOP("PROGRAMMA CHIUSO")
        SWBkAttivo = False
        Me.NotifyIcon1.Dispose()
        Me.Dispose()
        ' ''Dim frmPassword As New frmPassword()
        ' ''frmPassword.Text = "Arresta Backup automatico"
        ' ''If frmPassword._Go(Owner) Then
        ' ''    frmPassword.Dispose()
        ' ''    frmPassword = Nothing
        ' ''    SWBkAttivo = False
        ' ''    Me.NotifyIcon1.Dispose()
        ' ''    Me.Dispose()
        ' ''Else
        ' ''    frmPassword.Dispose()
        ' ''    frmPassword = Nothing
        ' ''    MsgBox("Password errata.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, "Arresta Backup automatico")
        ' ''    SWBkAttivo = True
        ' ''End If

    End Sub

    Private Function funEseguiBackup(ByVal TipoDatabase As String, ByVal ConnDatabase As String) As Boolean

        Dim objDatabase As New clsDatabase()
        Dim objBackup As New clsBackupDatabase()

        Dim SupportoRemovibile As Boolean
        Dim PercorsoSupportoRemovibile As String = getConfigurationString("SupportoRemovibile")
        If InStr(UCase(PercorsoSupportoRemovibile), "\\") > 0 Then
            PercorsoSupportoRemovibile = ""
        End If
        If PercorsoSupportoRemovibile <> "" Then
            SupportoRemovibile = True
        Else
            SupportoRemovibile = False
        End If

        NomeDB = TipoDatabase

        If SWBKAuto = True Then
            Me.Cursor = Cursors.WaitCursor

            If SupportoRemovibile = False Then
                EsitoBKAuto = "ERRORE : Operazione di backup non è riuscita. Supporto removibile non definito"
                ScriviFileLog(EsitoBKAuto)
                AggChiaveOP(EsitoBKAuto)
                '--
                objDatabase = Nothing
                objBackup.Dispose()
                objBackup = Nothing

                Me.Cursor = Cursors.AppStarting
                Return False
            End If

            If objBackup.BackupDatabase(NomeDB, SupportoRemovibile, PercorsoSupportoRemovibile, ConnDatabase) Then
                objDatabase = Nothing
                objBackup.Dispose()
                objBackup = Nothing

                Me.Cursor = Cursors.AppStarting
                EsitoBKAuto = "OK : Operazione di backup di " & NomeDB & " completata con successo."
                ScriviFileLog(EsitoBKAuto)
                AggChiaveOP(EsitoBKAuto)
                Return True
            Else
                'QUI SE VOGLIO SALVARE IL CONTENUTO DI ESITOBKAUTO = CONTIENE L'ERRORE ULTIMO BLOCCANTE
                objDatabase = Nothing
                objBackup.Dispose()
                objBackup = Nothing

                Me.Cursor = Cursors.AppStarting
                'EsitoBKAuto = "ERRORE : Operazione di backup di " & NomeDB & " non è riuscita."
                ScriviFileLog("ERRORE : Operazione di backup di " & NomeDB & " non è riuscita.")
                AggChiaveOP(EsitoBKAuto)
                Return False
            End If

        ElseIf SWBKAuto = False Then
            If MsgBox("Confermi l'operazione di backup del database [" & TipoDatabase & "] ?", MsgBoxStyle.YesNo Or MsgBoxStyle.DefaultButton2 Or MsgBoxStyle.Question, Me.Text) = MsgBoxResult.Yes Then
                Me.Cursor = Cursors.WaitCursor

                If objBackup.BackupDatabase(NomeDB, SupportoRemovibile, PercorsoSupportoRemovibile, ConnDatabase) Then
                    MsgBox("Operazione di backup di " & NomeDB & " completata con successo.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.Text)
                    objDatabase = Nothing
                    objBackup.Dispose()
                    objBackup = Nothing

                    Me.Cursor = Cursors.AppStarting
                    ScriviFileLog("Operazione di backup di " & NomeDB & " completata con successo.")
                    AggChiaveOP("Operazione di backup di " & NomeDB & " completata con successo.")
                    Return True
                Else
                    MsgBox("Operazione di backup di " & NomeDB & " non è riuscita.", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, Me.Text)
                    objDatabase = Nothing
                    objBackup.Dispose()
                    objBackup = Nothing

                    Me.Cursor = Cursors.AppStarting
                    ScriviFileLog("Operazione di backup di " & NomeDB & " non è riuscita.")
                    AggChiaveOP("Operazione di backup di " & NomeDB & " non è riuscita.")
                    Return False
                End If

            Else
                Return False
            End If
        Else
            EsitoBKAuto = "Backup del database [" & TipoDatabase & "]"
            AggChiaveOP(EsitoBKAuto)
            Me.Cursor = Cursors.WaitCursor

            If objBackup.BackupDatabase(NomeDB, SupportoRemovibile, PercorsoSupportoRemovibile, ConnDatabase) Then
                EsitoBKAuto = "Operazione di backup di " & NomeDB & " completata con successo."
                objDatabase = Nothing
                objBackup.Dispose()
                objBackup = Nothing

                Me.Cursor = Cursors.AppStarting
                ScriviFileLog(EsitoBKAuto)
                AggChiaveOP(EsitoBKAuto)
                Return True
            Else
                EsitoBKAuto = "Operazione di backup di " & NomeDB & " non è riuscita."
                objDatabase = Nothing
                objBackup.Dispose()
                objBackup = Nothing

                Me.Cursor = Cursors.AppStarting
                ScriviFileLog(EsitoBKAuto)
                AggChiaveOP(EsitoBKAuto)
                Return False
            End If

        End If
    End Function

    Private Sub btnEseguiBK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEseguiBK.Click
        If PrevInstance(AppName) Then
            MsgBox("Risulta aperta una sessione di [Backup automatico].", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, AppName)
            Exit Sub
        ElseIf PrevInstance("SoftTickets") Then
            MsgBox("Risulta aperta una sessione di [SoftTickets Administrator].", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, AppName)
            Exit Sub
        ElseIf PrevInstance("SoftTicketsCassa") Then
            MsgBox("Risulta aperta una sessione di [SoftTickets Cassa].", MsgBoxStyle.OkOnly Or MsgBoxStyle.Information, AppName)
            Exit Sub
        End If

        If getConfigurationString("SoftTickets") <> "" Then
            If BKMSoftTickets("SoftTickets", "") = False Then
                Exit Sub
            End If
        End If
        Call MainBKDB()
    End Sub

    Private Function BKMSoftTickets(ByVal TipoDatabase As String, ByVal ConnDatabase As String) As Boolean
        '--------------------------------------
        SWGGEsito = 2
        EsitoBKAuto = "" : lblNoBK.Text = ""
        Me.Cursor = Cursors.WaitCursor
        Me.Enabled = False
        SWBkAttivo = False
        Timer1.Stop()

        If funEseguiBackup(TipoDatabase, ConnDatabase) = False Then
            SWGGEsito = 1
            EsitoBKAuto = "ERRORE : " + EsitoBKAuto
        Else
            SWGGEsito = 2
            EsitoBKAuto = "OK : " + EsitoBKAuto
        End If

        If SWBKManuale = False Then
            Timer1.Start()
            SWBkAttivo = True
        End If

        Me.Enabled = True
        Me.Cursor = Cursors.Default

        Dim DataOraEsito As String = Format(Now, "dddd d MMMM yyyy") + " ore: " & Format(Now, "H:mm")
        If SWGGEsito = 1 Then
            Dim str As String = ""
            If EsitoBKAuto.Length > 3 Then str = EsitoBKAuto.Substring(0, 3)
            If UCase(str) = "ERR" Then
                EsitoBKAuto = DataOraEsito & " - " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            Else
                EsitoBKAuto = DataOraEsito & " - " & "ERRORE : " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            End If
            subImpostaTimer(MinutiErr)
        Else
            EsitoBKAuto = DataOraEsito & " - " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            subImpostaTimer(Minuti)
        End If
        lblStato.Text = EsitoBKAuto
        '-----
        '----- Controllo Config
        Dim Ok As Boolean = True
        If funLeggiConfig() = False Then Ok = False
        If funBkPrevisto() = False Then Ok = False
        If Ok = False Then
            SWGGEsito = 1
            lblStato.Text = EsitoBKAuto
            subImpostaTimer(MinutiErr)
        End If
        If SWGGEsito = 1 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Sub btnReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReset.Click

        Me.StatusBarPanel1.Text = Format(Now, "dddd d MMMM yyyy") + " ore: " & Format(Now, "H:mm")
        lblBKDataOraEsito.Text = Format(Now, FormatoData) + " " & Format(Now, FormatoOra)

        SWGGEsito = 0
        lblNoBK.Text = "Reset eseguito. Attendere prossimo controllo per leggere le nuove impostazioni di Backup"
        '----
        If funLeggiConfig() = False Then
            lblStato.Text = EsitoBKAuto
            lblNoBK.Text = "Backup non eseguito : ERRORE Config - Giorno previsto per il Backup"
            Dim str As String = ""
            If EsitoBKAuto.Length > 3 Then str = EsitoBKAuto.Substring(0, 3)
            If UCase(str) = "ERR" Then
                EsitoBKAuto = EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            Else
                EsitoBKAuto = "ERRORE : " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            End If
            lblStato.Text = EsitoBKAuto
            '-----
            WDPrevisto = 0 : SWGGEsito = 1
            
            subImpostaTimer(MinutiErr)

        ElseIf funBkPrevisto() = False Then
            lblStato.Text = EsitoBKAuto
            lblNoBK.Text = "Backup non eseguito : ERRORE Config - Giorno previsto per il Backup"
            Dim str As String = ""
            If EsitoBKAuto.Length > 3 Then str = EsitoBKAuto.Substring(0, 3)
            If UCase(str) = "ERR" Then
                EsitoBKAuto = EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            Else
                EsitoBKAuto = "ERRORE : " & EsitoBKAuto.Trim & " " & StatusBarPanel3.Text.Trim & StatusBarPanel4.Text.Trim
            End If
            lblStato.Text = EsitoBKAuto
            '-----
            WDPrevisto = 0 : SWGGEsito = 1
          
            subImpostaTimer(MinutiErr)
        Else
            subImpostaTimer(Minuti)
        End If
    End Sub

    Private Sub Timer2_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        Me.StatusBarPanel1.Text = Format(Now, "dddd d MMMM yyyy") + " ore: " & Format(Now, "H:mm")
    End Sub

    Private Sub frmBKAuto_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        e.Cancel = True
    End Sub

    Private Sub frmBKAuto_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Timer2.Stop()
            Me.ShowInTaskbar = False
            NotifyIcon1.Visible = True
        Else
            Timer2.Start()
            Me.StatusBarPanel1.Text = Format(Now, "dddd d MMMM yyyy") + " ore: " & Format(Now, "H:mm")
            Me.Width = 828 : Me.Height = 310
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
        End If
    End Sub

    Private Sub NotifyIcon1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDown
        Visualizzami()
    End Sub

    Private Sub Visualizzami()
        If Me.WindowState = FormWindowState.Normal Then Exit Sub
        Me.WindowState = FormWindowState.Normal
        CentraForm(Me)
        Me.Refresh()
        btnChiudi.Focus()
    End Sub

    
End Class
