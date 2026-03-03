Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.Core

Namespace Presentation
    ''' <summary>App settings (Singleton) – view and edit from admin dashboard.</summary>
    Public Class AppSettingsForm
        Inherits Form

        Private txtConn As TextBox
        Private txtVersion As TextBox
        Private cmbTheme As ComboBox
        Private btnSave As Button

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
            LoadFromSingleton()
        End Sub

        Private Sub LoadFromSingleton()
            Dim app = AppSettings.Instance
            txtConn.Text = app.ConnectionString
            txtVersion.Text = app.AppVersion
            cmbTheme.SelectedItem = app.ThemePreference
        End Sub

        Private Sub InitializeComponent()
            Me.SuspendLayout()
            Dim lblTitle As New Label() With {
                .Text = "App Settings",
                .Font = New Font("Segoe UI", 20, FontStyle.Bold),
                .Location = New Point(32, 24),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }
            Dim lblSub As New Label() With {
                .Text = "Global application settings (Singleton). Changes apply for current session.",
                .Font = New Font("Segoe UI", 10),
                .Location = New Point(36, 68),
                .AutoSize = True,
                .ForeColor = ThemeBase.TextMuted
            }

            Dim pnl As New Panel() With {
                .Location = New Point(32, 110),
                .Size = New Size(560, 240),
                .BackColor = ThemeBase.CardBg
            }

            Dim lblConn As New Label() With {.Text = "Connection string:", .Location = New Point(20, 20), .AutoSize = True}
            txtConn = New TextBox() With {.Location = New Point(20, 44), .Size = New Size(520, 24), .Multiline = True, .Height = 44}
            Dim lblVer As New Label() With {.Text = "App version:", .Location = New Point(20, 100), .AutoSize = True}
            txtVersion = New TextBox() With {.Location = New Point(20, 124), .Width = 200}
            Dim lblTh As New Label() With {.Text = "Theme:", .Location = New Point(20, 160), .AutoSize = True}
            cmbTheme = New ComboBox() With {.Location = New Point(20, 184), .Width = 200, .DropDownStyle = ComboBoxStyle.DropDownList}
            cmbTheme.Items.AddRange({"Dark", "Light"})
            cmbTheme.SelectedIndex = 0

            btnSave = New Button() With {.Text = "Save settings", .Location = New Point(32, 360), .Size = New Size(140, 36)}
            AddHandler btnSave.Click, AddressOf BtnSave_Click

            pnl.Controls.AddRange(New Control() {lblConn, txtConn, lblVer, txtVersion, lblTh, cmbTheme})
            Me.Controls.Add(btnSave)
            Me.Controls.Add(lblTitle)
            Me.Controls.Add(lblSub)
            Me.Controls.Add(pnl)
            Me.ResumeLayout(False)
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As EventArgs)
            Dim app = AppSettings.Instance
            app.ConnectionString = txtConn.Text.Trim()
            app.AppVersion = txtVersion.Text.Trim()
            app.ThemePreference = If(cmbTheme.SelectedItem?.ToString(), "Dark")
            MessageBox.Show("Settings saved (current session).", "App Settings", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub
    End Class
End Namespace
