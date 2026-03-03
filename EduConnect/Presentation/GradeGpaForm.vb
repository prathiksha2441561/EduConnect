Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.BLL

Namespace Presentation
    Public Class GradeGpaForm
        Inherits Form

        Private cmbStudent As ComboBox
        Private cmbCourse As ComboBox
        Private numGrade As NumericUpDown
        Private txtSemester As TextBox
        Private dgvGrades As DataGridView
        Private lblGpa As Label
        Private _gradeService As New GradeService()
        Private _studentIds As New List(Of Integer)()
        Private _courseIds As New List(Of Integer)()

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
            LoadStudents()
            LoadCourses()
        End Sub

        Private Sub LoadStudents()
            cmbStudent.Items.Clear()
            _studentIds.Clear()
            Try
                Dim students = _gradeService.GetAllStudents()
                For Each s In students
                    cmbStudent.Items.Add(s.Name & " (ID: " & s.Id & ")")
                    _studentIds.Add(s.Id)
                Next
                If cmbStudent.Items.Count > 0 Then cmbStudent.SelectedIndex = 0
            Catch
            End Try
        End Sub

        Private Sub LoadCourses()
            cmbCourse.Items.Clear()
            _courseIds.Clear()
            Try
                Dim courses = _gradeService.GetAllCourses()
                For Each c In courses
                    cmbCourse.Items.Add(c.Name & " (ID: " & c.Id & ")")
                    _courseIds.Add(c.Id)
                Next
                If cmbCourse.Items.Count > 0 Then cmbCourse.SelectedIndex = 0
            Catch
            End Try
        End Sub

        Private Sub InitializeComponent()
            Dim lblTitle As New Label() With {
                .Text = "Grade & GPA Calculator",
                .Font = New Font("Segoe UI", 20, FontStyle.Bold),
                .Location = New Point(32, 24),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }
            Dim lblSub As New Label() With {
                .Text = "Add grades and calculate student GPA (0–10 and 4.0 scale).",
                .Font = New Font("Segoe UI", 10),
                .Location = New Point(36, 64),
                .AutoSize = True,
                .ForeColor = ThemeBase.TextMuted
            }

            Dim lblSt As New Label() With {.Text = "Student:", .Location = New Point(32, 108), .AutoSize = True}
            cmbStudent = New ComboBox() With {.Location = New Point(32, 132), .Width = 280, .DropDownStyle = ComboBoxStyle.DropDownList}
            Dim lblCo As New Label() With {.Text = "Course:", .Location = New Point(340, 108), .AutoSize = True}
            cmbCourse = New ComboBox() With {.Location = New Point(340, 132), .Width = 260, .DropDownStyle = ComboBoxStyle.DropDownList}
            Dim lblGr As New Label() With {.Text = "Grade (0–10):", .Location = New Point(32, 168), .AutoSize = True}
            numGrade = New NumericUpDown() With {.Location = New Point(32, 192), .Width = 80, .Minimum = 0, .Maximum = 10, .DecimalPlaces = 2, .Increment = 0.5D, .Value = 7}
            Dim lblSem As New Label() With {.Text = "Semester:", .Location = New Point(140, 168), .AutoSize = True}
            txtSemester = New TextBox() With {.Location = New Point(140, 192), .Width = 120}

            Dim btnAdd As New Button() With {.Text = "Add grade", .Location = New Point(280, 188), .Size = New Size(100, 28)}
            AddHandler btnAdd.Click, AddressOf BtnAdd_Click
            Dim btnCalc As New Button() With {.Text = "Calculate GPA", .Location = New Point(400, 188), .Size = New Size(120, 28)}
            AddHandler btnCalc.Click, AddressOf BtnCalc_Click
            AddHandler cmbStudent.SelectedIndexChanged, AddressOf cmbStudent_SelectedIndexChanged

            lblGpa = New Label() With {
                .Text = "GPA: —",
                .Font = New Font("Segoe UI", 14, FontStyle.Bold),
                .Location = New Point(32, 232),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }

            Dim lblList As New Label() With {.Text = "Grades for selected student", .Location = New Point(32, 272), .AutoSize = True}
            dgvGrades = New DataGridView() With {
                .Location = New Point(32, 296),
                .Size = New Size(640, 220),
                .AutoGenerateColumns = False,
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            }
            dgvGrades.Columns.Add("CourseId", "Course Id")
            dgvGrades.Columns.Add("GradePoint", "Grade (0–10)")
            dgvGrades.Columns.Add("Semester", "Semester")
            ThemeBase.StyleControl(dgvGrades)

            Me.Controls.AddRange(New Control() {lblTitle, lblSub, lblSt, cmbStudent, lblCo, cmbCourse, lblGr, numGrade, lblSem, txtSemester, btnAdd, btnCalc, lblGpa, lblList, dgvGrades})
        End Sub

        Private Function SelectedStudentId() As Integer?
            If cmbStudent.SelectedIndex < 0 OrElse cmbStudent.SelectedIndex >= _studentIds.Count Then Return Nothing
            Return _studentIds(cmbStudent.SelectedIndex)
        End Function

        Private Function SelectedCourseId() As Integer?
            If cmbCourse.SelectedIndex < 0 OrElse cmbCourse.SelectedIndex >= _courseIds.Count Then Return Nothing
            Return _courseIds(cmbCourse.SelectedIndex)
        End Function

        Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
            Dim studentId = SelectedStudentId()
            Dim courseId = SelectedCourseId()
            If Not studentId.HasValue OrElse Not courseId.HasValue Then
                MessageBox.Show("Select student and course.", "Grade", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Try
                _gradeService.AddGrade(New Grade() With {
                    .StudentId = studentId.Value,
                    .CourseId = courseId.Value,
                    .GradePoint = numGrade.Value,
                    .Semester = txtSemester.Text.Trim()
                })
                MessageBox.Show("Grade added.", "Grade", MessageBoxButtons.OK, MessageBoxIcon.Information)
                RefreshGradesGrid()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Grade", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub RefreshGradesGrid()
            Dim studentId = SelectedStudentId()
            If Not studentId.HasValue Then Return
            dgvGrades.Rows.Clear()
            Dim grades = _gradeService.GetGradesByStudent(studentId.Value)
            For Each g In grades
                dgvGrades.Rows.Add(g.CourseId, g.GradePoint.ToString("N2"), If(String.IsNullOrEmpty(g.Semester), "", g.Semester))
            Next
        End Sub

        Private Sub BtnCalc_Click(sender As Object, e As EventArgs)
            Dim studentId = SelectedStudentId()
            If Not studentId.HasValue Then
                MessageBox.Show("Select a student.", "GPA", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            RefreshGradesGrid()
            Dim gpa10 = _gradeService.CalculateGpa(studentId.Value)
            If Not gpa10.HasValue Then
                lblGpa.Text = "GPA: No grades yet"
                Return
            End If
            Dim gpa4 = GradeService.To4PointScale(gpa10.Value)
            lblGpa.Text = $"GPA: {gpa10.Value:N2} (10 scale)  |  {gpa4:N2} (4.0 scale)"
        End Sub

        Private Sub cmbStudent_SelectedIndexChanged(sender As Object, e As EventArgs)
            RefreshGradesGrid()
            Dim studentId = SelectedStudentId()
            If Not studentId.HasValue Then Return
            Dim gpa10 = _gradeService.CalculateGpa(studentId.Value)
            If gpa10.HasValue Then
                lblGpa.Text = $"GPA: {gpa10.Value:N2} (10)  |  {GradeService.To4PointScale(gpa10.Value):N2} (4.0)"
            Else
                lblGpa.Text = "GPA: —"
            End If
        End Sub
    End Class
End Namespace
