Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.Core

Namespace Presentation
    Public Class StudentProfileForm
        Inherits Form

        Private txtDisplayName As TextBox
        Private txtEmail As TextBox
        Private lblUsername As Label
        Private lblCourse As Label

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
            LoadProfile()
        End Sub

        Private Sub LoadProfile()
            Dim app = AppSettings.Instance
            If String.IsNullOrWhiteSpace(app.ProfileDisplayName) Then app.ProfileDisplayName = app.CurrentUser
            txtDisplayName.Text = app.ProfileDisplayName
            txtEmail.Text = If(String.IsNullOrWhiteSpace(app.ProfileEmail), "", app.ProfileEmail)
            lblUsername.Text = "Username: " & app.CurrentUser
            lblCourse.Text = "Enrolled: B.Tech Computer Science"
        End Sub

        Private Sub InitializeComponent()
            Dim lblTitle As New Label() With {
                .Text = "My Profile",
                .Font = New Font("Segoe UI", 22, FontStyle.Bold),
                .Location = New Point(32, 24),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }
            Dim lblSub As New Label() With {
                .Text = "View and update your profile. Changes are saved for this session.",
                .Font = New Font("Segoe UI", 10),
                .Location = New Point(36, 64),
                .AutoSize = True,
                .ForeColor = ThemeBase.TextMuted
            }

            Dim pnl As New Panel() With {
                .Location = New Point(32, 108),
                .Size = New Size(480, 300),
                .BackColor = ThemeBase.CardBg
            }

            lblUsername = New Label() With {.Text = "Username:", .Location = New Point(24, 20), .AutoSize = True}
            lblCourse = New Label() With {.Text = "Enrolled:", .Location = New Point(24, 52), .AutoSize = True, .ForeColor = ThemeBase.TextMuted}
            Dim lblDisp As New Label() With {.Text = "Display name:", .Location = New Point(24, 96), .AutoSize = True}
            txtDisplayName = New TextBox() With {.Location = New Point(24, 120), .Width = 320}
            Dim lblEm As New Label() With {.Text = "Email:", .Location = New Point(24, 164), .AutoSize = True}
            txtEmail = New TextBox() With {.Location = New Point(24, 188), .Width = 320}

            Dim btnSave As New Button() With {.Text = "Save changes", .Location = New Point(24, 244), .Size = New Size(140, 36)}
            AddHandler btnSave.Click, AddressOf BtnSave_Click

            pnl.Controls.AddRange(New Control() {lblUsername, lblCourse, lblDisp, txtDisplayName, lblEm, txtEmail, btnSave})
            Me.Controls.AddRange(New Control() {lblTitle, lblSub, pnl})
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As EventArgs)
            AppSettings.Instance.ProfileDisplayName = txtDisplayName.Text.Trim()
            AppSettings.Instance.ProfileEmail = txtEmail.Text.Trim()
            If String.IsNullOrWhiteSpace(AppSettings.Instance.ProfileDisplayName) Then AppSettings.Instance.ProfileDisplayName = AppSettings.Instance.CurrentUser
            MessageBox.Show("Profile updated.", "My Profile", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Dim main = TryCast(Me.Parent?.Parent, MainForm)
            If main IsNot Nothing Then main.RefreshUserDisplay()
        End Sub
    End Class
End Namespace
