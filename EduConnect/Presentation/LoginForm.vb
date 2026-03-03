Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.Core

Namespace Presentation
    Public Class LoginForm
        Inherits Form

        Private txtUser As TextBox
        Private txtPass As TextBox
        Private cmbRole As ComboBox
        Private btnLogin As Button
        Private WithEvents sessionManager As New SessionManager()

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
        End Sub

        Private Sub InitializeComponent()
            Me.Size = New Size(440, 480)
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.FormBorderStyle = FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.Text = "EduConnect - Sign in"
            Me.BackColor = ThemeBase.DarkBg

            Dim lblTitle As New Label() With {
                .Text = "EduConnect",
                .Font = New Font("Segoe UI", 26, FontStyle.Bold),
                .Location = New Point(80, 36),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }
            Dim lblSub As New Label() With {
                .Text = "Smart Learning Management System",
                .Font = New Font("Segoe UI", 10),
                .Location = New Point(72, 82),
                .AutoSize = True,
                .ForeColor = ThemeBase.TextMuted
            }

            Dim pnl As New Panel() With {
                .Location = New Point(40, 120),
                .Size = New Size(360, 280),
                .BackColor = ThemeBase.CardBg
            }

            Dim lblRole As New Label() With {.Text = "Role", .Location = New Point(24, 20), .AutoSize = True}
            cmbRole = New ComboBox() With {.Location = New Point(24, 44), .Width = 312, .DropDownStyle = ComboBoxStyle.DropDownList}
            cmbRole.Items.AddRange({"Admin", "Instructor", "Student"})
            cmbRole.SelectedIndex = 0

            Dim lblUser As New Label() With {.Text = "Username", .Location = New Point(24, 88), .AutoSize = True}
            txtUser = New TextBox() With {.Location = New Point(24, 112), .Width = 312}

            Dim lblPass As New Label() With {.Text = "Password", .Location = New Point(24, 156), .AutoSize = True}
            txtPass = New TextBox() With {.Location = New Point(24, 180), .Width = 312, .PasswordChar = "*"c}

            btnLogin = New Button() With {.Text = "Sign in", .Location = New Point(24, 228), .Width = 312, .Height = 40}
            AddHandler btnLogin.Click, AddressOf BtnLogin_Click

            Dim lnkRegister As New LinkLabel() With {
                .Text = "New user? Register here",
                .Location = New Point(24, 276),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent,
                .Cursor = Cursors.Hand
            }
            AddHandler lnkRegister.Click, AddressOf LnkRegister_Click

            pnl.Controls.AddRange(New Control() {lblRole, cmbRole, lblUser, txtUser, lblPass, txtPass, btnLogin, lnkRegister})
            pnl.Height = 300
            Me.Controls.Add(pnl)
            Me.Controls.Add(lblSub)
            Me.Controls.Add(lblTitle)
        End Sub

        Private Sub LnkRegister_Click(sender As Object, e As EventArgs)
            Using reg As New RegistrationForm(Me)
                reg.ShowDialog()
            End Using
        End Sub

        Private Sub BtnLogin_Click(sender As Object, e As EventArgs)
            sessionManager.Authenticate(txtUser.Text, txtPass.Text, cmbRole.SelectedItem.ToString())
        End Sub

        Private Sub OnLoginResult(username As String, success As Boolean, message As String) Handles sessionManager.OnLoginAttempt
            If success Then
                Dim main As New MainForm()
                main.Show()
                Me.Hide()
            Else
                MessageBox.Show(message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Sub
    End Class
End Namespace
