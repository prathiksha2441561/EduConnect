Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.BLL
Imports EduConnect.Core

Namespace Presentation
    Public Class EnrollForm
        Inherits Form

        Private dgvAvailable As DataGridView
        Private dgvMyEnrollments As DataGridView
        Private _enrollmentService As New EnrollmentService()
        Private _courseService As New CourseService(New DAL.CourseRepository())

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
            LoadData()
        End Sub

        Private Function CurrentStudentId() As Integer
            Dim id = AppSettings.Instance.CurrentStudentId
            If id.HasValue Then Return id.Value
            Dim repo As New DAL.StudentRepository()
            Dim s = repo.GetFirst()
            Return If(s Is Nothing, 0, s.Id)
        End Function

        Private Sub InitializeComponent()
            Dim lblTitle As New Label() With {
                .Text = "Course Enrollment",
                .Font = New Font("Segoe UI", 20, FontStyle.Bold),
                .Location = New Point(32, 24),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }
            Dim lblSub As New Label() With {
                .Text = "Enroll in a course. Fee will be added to My Payments.",
                .Font = New Font("Segoe UI", 10),
                .Location = New Point(36, 64),
                .AutoSize = True,
                .ForeColor = ThemeBase.TextMuted
            }

            Dim lblAvail As New Label() With {.Text = "Available courses", .Location = New Point(32, 100), .AutoSize = True}
            dgvAvailable = New DataGridView() With {
                .Location = New Point(32, 124),
                .Size = New Size(500, 160),
                .AutoGenerateColumns = True,
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                .MultiSelect = False
            }
            ThemeBase.StyleControl(dgvAvailable)

            Dim btnEnroll As New Button() With {.Text = "Enroll selected", .Location = New Point(32, 292), .Size = New Size(140, 36)}
            AddHandler btnEnroll.Click, AddressOf BtnEnroll_Click

            Dim lblMy As New Label() With {.Text = "My enrollments", .Location = New Point(32, 340), .AutoSize = True}
            dgvMyEnrollments = New DataGridView() With {
                .Location = New Point(32, 364),
                .Size = New Size(500, 140),
                .AutoGenerateColumns = True
            }
            ThemeBase.StyleControl(dgvMyEnrollments)

            Me.Controls.AddRange(New Control() {lblTitle, lblSub, lblAvail, dgvAvailable, btnEnroll, lblMy, dgvMyEnrollments})
        End Sub

        Private Sub LoadData()
            Dim studentId = CurrentStudentId()
            If studentId <= 0 Then
                MessageBox.Show("Student profile not found. Please contact admin.", "Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            Try
                dgvAvailable.DataSource = _enrollmentService.GetAvailableCourses(studentId)
                dgvMyEnrollments.DataSource = _enrollmentService.GetMyEnrollments(studentId)
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub BtnEnroll_Click(sender As Object, e As EventArgs)
            If dgvAvailable.CurrentRow Is Nothing Then
                MessageBox.Show("Select a course to enroll.", "Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim studentId = CurrentStudentId()
            If studentId <= 0 Then Return
            Dim courseId = Convert.ToInt32(dgvAvailable.CurrentRow.Cells("Id").Value)
            Try
                _enrollmentService.Enroll(studentId, courseId)
                MessageBox.Show("Enrolled successfully. Pay the fee under My Payments.", "Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadData()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Enrollment", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace
