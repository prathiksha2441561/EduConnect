Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.Core
Imports EduConnect.BLL
Imports EduConnect.DAL

Namespace Presentation
    Public Class RegistrationForm
        Inherits Form

        Private txtUsername As TextBox
        Private txtPassword As TextBox
        Private txtConfirm As TextBox
        Private txtEmail As TextBox
        Private txtDisplayName As TextBox
        Private cmbRole As ComboBox
        Private loginFormRef As Form

        Public Sub New(loginForm As Form)
            loginFormRef = loginForm
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
        End Sub

        Private Sub InitializeComponent()
            Me.Size = New Size(460, 520)
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.FormBorderStyle = FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.Text = "EduConnect - Register"
            Me.BackColor = ThemeBase.DarkBg

            Dim lblTitle As New Label() With {
                .Text = "Create account",
                .Font = New Font("Segoe UI", 22, FontStyle.Bold),
                .Location = New Point(24, 20),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }
            Dim lblSub As New Label() With {
                .Text = "Register to access your dashboard.",
                .Font = New Font("Segoe UI", 10),
                .Location = New Point(28, 58),
                .AutoSize = True,
                .ForeColor = ThemeBase.TextMuted
            }

            Dim pnl As New Panel() With {
                .Location = New Point(24, 90),
                .Size = New Size(400, 340),
                .BackColor = ThemeBase.CardBg
            }

            Dim lblUser As New Label() With {.Text = "Username", .Location = New Point(20, 12), .AutoSize = True}
            txtUsername = New TextBox() With {.Location = New Point(20, 36), .Width = 360}
            Dim lblPass As New Label() With {.Text = "Password", .Location = New Point(20, 72), .AutoSize = True}
            txtPassword = New TextBox() With {.Location = New Point(20, 96), .Width = 360, .PasswordChar = "*"c}
            Dim lblConf As New Label() With {.Text = "Confirm password", .Location = New Point(20, 132), .AutoSize = True}
            txtConfirm = New TextBox() With {.Location = New Point(20, 156), .Width = 360, .PasswordChar = "*"c}
            Dim lblEmail As New Label() With {.Text = "Email", .Location = New Point(20, 192), .AutoSize = True}
            txtEmail = New TextBox() With {.Location = New Point(20, 216), .Width = 360}
            Dim lblDisp As New Label() With {.Text = "Display name", .Location = New Point(20, 252), .AutoSize = True}
            txtDisplayName = New TextBox() With {.Location = New Point(20, 276), .Width = 360}
            Dim lblRole As New Label() With {.Text = "Role", .Location = New Point(20, 312), .AutoSize = True}
            cmbRole = New ComboBox() With {.Location = New Point(20, 336), .Width = 360, .DropDownStyle = ComboBoxStyle.DropDownList}
            cmbRole.Items.AddRange({"Admin", "Instructor", "Student"})
            cmbRole.SelectedIndex = 2

            Dim btnRegister As New Button() With {.Text = "Register", .Location = New Point(24, 442), .Width = 400, .Height = 40}
            AddHandler btnRegister.Click, AddressOf BtnRegister_Click

            pnl.Controls.AddRange(New Control() {lblUser, txtUsername, lblPass, txtPassword, lblConf, txtConfirm, lblEmail, txtEmail, lblDisp, txtDisplayName, lblRole, cmbRole})
            Me.Controls.AddRange(New Control() {lblTitle, lblSub, pnl, btnRegister})
        End Sub

        Private Sub BtnRegister_Click(sender As Object, e As EventArgs)
            Dim username = txtUsername.Text.Trim()
            Dim password = txtPassword.Text
            Dim confirm = txtConfirm.Text
            Dim email = txtEmail.Text.Trim()
            Dim displayName = txtDisplayName.Text.Trim()
            Dim role = cmbRole.SelectedItem?.ToString()

            If String.IsNullOrEmpty(username) Then
                MessageBox.Show("Enter a username.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtUsername.Focus()
                Return
            End If
            If String.IsNullOrEmpty(password) Then
                MessageBox.Show("Enter a password.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtPassword.Focus()
                Return
            End If
            If password <> confirm Then
                MessageBox.Show("Passwords do not match.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtConfirm.Focus()
                Return
            End If
            If String.IsNullOrEmpty(email) Then
                MessageBox.Show("Enter your email.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtEmail.Focus()
                Return
            End If
            If String.IsNullOrEmpty(role) Then
                MessageBox.Show("Select a role.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim userRepo As New UserRepository()
            If userRepo.UsernameExists(username) Then
                MessageBox.Show("Username already exists. Choose another.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtUsername.Focus()
                Return
            End If

            Try
                Dim user As New User() With {
                    .Username = username,
                    .PasswordHash = SessionManager.HashPassword(password, "randomsalt123"),
                    .Role = role,
                    .Email = email,
                    .DisplayName = If(String.IsNullOrEmpty(displayName), username, displayName),
                    .CreatedAt = DateTime.Now
                }
                Dim userId = userRepo.Add(user)

                If String.Equals(role, "Student", StringComparison.OrdinalIgnoreCase) Then
                    Dim studentRepo As New StudentRepository()
                    Dim student As New Student() With {
                        .UserId = userId,
                        .Name = user.DisplayName,
                        .Email = user.Email,
                        .CourseId = 0
                    }
                    studentRepo.Add(student)
                End If

                ' Set session and redirect to dashboard
                AppSettings.Instance.CurrentUser = user.Username
                AppSettings.Instance.UserRole = user.Role
                AppSettings.Instance.ProfileDisplayName = user.DisplayName
                AppSettings.Instance.ProfileEmail = user.Email
                If String.Equals(role, "Student", StringComparison.OrdinalIgnoreCase) Then
                    Dim studentRepo2 As New StudentRepository()
                    Dim st = studentRepo2.GetByUserId(userId)
                    If st IsNot Nothing Then AppSettings.Instance.CurrentStudentId = st.Id
                Else
                    AppSettings.Instance.CurrentStudentId = Nothing
                End If

                Dim main As New MainForm()
                main.Show()
                If loginFormRef IsNot Nothing Then loginFormRef.Hide()
                Me.Close()
            Catch ex As Exception
                MessageBox.Show("Registration failed: " & ex.Message, "Register", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace
