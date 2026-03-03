Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.Core

Namespace Presentation
    Public Class MainForm
        Inherits Form

        Private panelSidebar As Panel
        Private panelContent As Panel
        Private lblStatus As ToolStripStatusLabel
        Private lblUserInfo As Label

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
            UpdateUserInfo()
            lblStatus.Text = $"Logged in as {DisplayName()} ({AppSettings.Instance.UserRole})"

            If AppSettings.Instance.UserRole = "Admin" Then
                ShowForm(New AdminProfileForm())
            ElseIf AppSettings.Instance.UserRole = "Instructor" Then
                ShowForm(New StudentProfileForm())
            Else
                ShowForm(New StudentProfileForm())
            End If
        End Sub

        Private Function DisplayName() As String
            Dim name = AppSettings.Instance.ProfileDisplayName
            Return If(String.IsNullOrWhiteSpace(name), AppSettings.Instance.CurrentUser, name)
        End Function

        Private Sub UpdateUserInfo()
            If lblUserInfo IsNot Nothing Then lblUserInfo.Text = DisplayName() & vbCrLf & AppSettings.Instance.UserRole
            If lblStatus IsNot Nothing Then lblStatus.Text = $"Logged in as {DisplayName()} ({AppSettings.Instance.UserRole})"
        End Sub

        Public Sub RefreshUserDisplay()
            UpdateUserInfo()
        End Sub

        Private Sub InitializeComponent()
            Me.Size = New Size(1200, 750)
            Me.StartPosition = FormStartPosition.CenterScreen
            Me.Text = "EduConnect - Dashboard"
            Me.MinimumSize = New Size(900, 600)

            panelSidebar = New Panel() With {
                .Width = ThemeBase.SidebarWidth,
                .Dock = DockStyle.Left,
                .BackColor = ThemeBase.DarkPanel
            }

            Dim lblLogo As New Label() With {
                .Text = "EduConnect",
                .Font = New Font("Segoe UI", 18, FontStyle.Bold),
                .ForeColor = ThemeBase.Accent,
                .Location = New Point(20, 24),
                .AutoSize = True
            }

            lblUserInfo = New Label() With {
                .Text = DisplayName() & vbCrLf & AppSettings.Instance.UserRole,
                .Font = New Font("Segoe UI", 9),
                .ForeColor = ThemeBase.TextMuted,
                .Location = New Point(20, 64),
                .Size = New Size(200, 36),
                .BorderStyle = BorderStyle.FixedSingle,
                .BackColor = ThemeBase.CardBg,
                .TextAlign = ContentAlignment.MiddleLeft
            }

            Dim btnLogout As New Button() With {
                .Text = "Logout",
                .Dock = DockStyle.Bottom,
                .Height = 48,
                .BackColor = ThemeBase.LogoutRed,
                .FlatStyle = FlatStyle.Flat,
                .Font = New Font("Segoe UI Semibold", 10)
            }
            btnLogout.FlatAppearance.BorderSize = 0
            AddHandler btnLogout.Click, AddressOf Logout_Click

            Dim flow As New FlowLayoutPanel() With {
                .Location = New Point(12, 116),
                .Size = New Size(ThemeBase.SidebarWidth - 24, 400),
                .FlowDirection = FlowDirection.TopDown,
                .WrapContents = False,
                .AutoScroll = True,
                .BackColor = Color.Transparent
            }

            If AppSettings.Instance.UserRole = "Admin" Then
                BuildAdminMenu(flow)
            ElseIf AppSettings.Instance.UserRole = "Instructor" Then
                BuildInstructorMenu(flow)
            Else
                BuildStudentMenu(flow)
            End If

            panelSidebar.Controls.Add(flow)
            panelSidebar.Controls.Add(lblUserInfo)
            panelSidebar.Controls.Add(lblLogo)
            panelSidebar.Controls.Add(btnLogout)

            panelContent = New Panel() With {.Dock = DockStyle.Fill, .BackColor = ThemeBase.DarkBg}

            Dim statusStrip As New StatusStrip() With {.BackColor = ThemeBase.DarkPanel, .ForeColor = ThemeBase.TextMuted}
            lblStatus = New ToolStripStatusLabel() With {.Text = "Ready"}
            statusStrip.Items.Add(lblStatus)

            Me.Controls.Add(panelContent)
            Me.Controls.Add(panelSidebar)
            Me.Controls.Add(statusStrip)
        End Sub

        Private Function NavButton(text As String) As Button
            Dim btn As New Button() With {
                .Text = "  " & text,
                .Size = New Size(ThemeBase.SidebarWidth - 36, 44),
                .Margin = New Padding(6, 4, 6, 4),
                .TextAlign = ContentAlignment.MiddleLeft,
                .FlatStyle = FlatStyle.Flat
            }
            btn.FlatAppearance.BorderSize = 0
            Return btn
        End Function

        Private Sub BuildAdminMenu(flow As FlowLayoutPanel)
            Dim btnProfile = NavButton("Admin Profile")
            Dim btnCourses = NavButton("Course Management")
            Dim btnStudents = NavButton("Student Management")
            Dim btnGrades = NavButton("Grades & GPA")
            Dim btnSecurity = NavButton("Security Center")
            Dim btnSettings = NavButton("App Settings")
            AddHandler btnProfile.Click, Sub() ShowForm(New AdminProfileForm())
            AddHandler btnCourses.Click, Sub() ShowForm(New CourseMgmtForm())
            AddHandler btnStudents.Click, Sub() ShowForm(New StudentMgmtForm())
            AddHandler btnGrades.Click, Sub() ShowForm(New GradeGpaForm())
            AddHandler btnSecurity.Click, Sub() ShowForm(New SecurityForm())
            AddHandler btnSettings.Click, Sub() ShowForm(New AppSettingsForm())
            flow.Controls.AddRange(New Control() {btnProfile, btnCourses, btnStudents, btnGrades, btnSecurity, btnSettings})
        End Sub

        Private Sub BuildInstructorMenu(flow As FlowLayoutPanel)
            Dim btnProfile = NavButton("My Profile")
            Dim btnMaterials = NavButton("Study Materials")
            Dim btnWhiteboard = NavButton("Whiteboard")
            Dim btnNotes = NavButton("Notepad")
            Dim btnCalc = NavButton("Fee Simulator")
            AddHandler btnProfile.Click, Sub() ShowForm(New StudentProfileForm())
            AddHandler btnMaterials.Click, Sub() ShowForm(New MaterialsForm())
            AddHandler btnWhiteboard.Click, Sub() ShowForm(New WhiteboardForm())
            AddHandler btnNotes.Click, Sub() ShowForm(New NotesForm())
            AddHandler btnCalc.Click, Sub() ShowForm(New CalculatorForm())
            flow.Controls.AddRange(New Control() {btnProfile, btnMaterials, btnWhiteboard, btnNotes, btnCalc})
        End Sub

        Private Sub BuildStudentMenu(flow As FlowLayoutPanel)
            Dim btnProfile = NavButton("My Profile")
            Dim btnEnroll = NavButton("Enroll in Course")
            Dim btnPayments = NavButton("My Payments")
            Dim btnMaterials = NavButton("Study Materials")
            Dim btnNotes = NavButton("Notepad")
            Dim btnWhiteboard = NavButton("Whiteboard")
            AddHandler btnProfile.Click, Sub() ShowForm(New StudentProfileForm())
            AddHandler btnEnroll.Click, Sub() ShowForm(New EnrollForm())
            AddHandler btnPayments.Click, Sub() ShowForm(New MyPaymentsForm())
            AddHandler btnMaterials.Click, Sub() ShowForm(New MaterialsForm())
            AddHandler btnNotes.Click, Sub() ShowForm(New NotesForm())
            AddHandler btnWhiteboard.Click, Sub() ShowForm(New WhiteboardForm())
            flow.Controls.AddRange(New Control() {btnProfile, btnEnroll, btnPayments, btnMaterials, btnNotes, btnWhiteboard})
        End Sub

        Private Sub Logout_Click(sender As Object, e As EventArgs)
            AppSettings.Instance.CurrentUser = Nothing
            AppSettings.Instance.UserRole = Nothing
            AppSettings.Instance.ProfileDisplayName = Nothing
            AppSettings.Instance.ProfileEmail = Nothing
            AppSettings.Instance.CurrentStudentId = Nothing
            Dim login As New LoginForm()
            login.Show()
            Me.Hide()
        End Sub

        Private Sub ShowForm(frm As Form)
            panelContent.Controls.Clear()
            frm.TopLevel = False
            frm.FormBorderStyle = FormBorderStyle.None
            frm.Dock = DockStyle.Fill
            panelContent.Controls.Add(frm)
            frm.Show()
        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            MyBase.OnFormClosed(e)
            Application.Exit()
        End Sub
    End Class
End Namespace
