Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.BLL
Imports EduConnect.DAL

Namespace Presentation
    Public Class CourseMgmtForm
        Inherits Form

        Private dgvCourses As DataGridView
        Private courseService As CourseService
        Private bsCourses As BindingSource
        Private pnlEdit As Panel
        Private txtName As TextBox
        Private numFee As NumericUpDown
        Private btnSave As Button
        Private btnCancelEdit As Button
        Private editingCourseId As Integer = -1

        Public Sub New()
            courseService = New CourseService(New CourseRepository())
            bsCourses = New BindingSource()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
            LoadData()
        End Sub

        Private Sub InitializeComponent()
            Dim lblTitle As New Label() With {
                .Text = "Course Management",
                .Font = New Font("Segoe UI", 20, FontStyle.Bold),
                .Location = New Point(32, 24),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }
            Dim lblSub As New Label() With {
                .Text = "Add, edit, and remove courses. Fee in ₹.",
                .Font = New Font("Segoe UI", 10),
                .Location = New Point(36, 64),
                .AutoSize = True,
                .ForeColor = ThemeBase.TextMuted
            }

            dgvCourses = New DataGridView() With {
                .Location = New Point(32, 100),
                .Size = New Size(720, 300),
                .AutoGenerateColumns = True,
                .DataSource = bsCourses,
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                .MultiSelect = False
            }
            ThemeBase.StyleControl(dgvCourses)

            Dim btnRefresh As New Button() With {.Text = "Refresh", .Location = New Point(32, 412), .Size = New Size(100, 36)}
            Dim btnAdd As New Button() With {.Text = "Add course", .Location = New Point(142, 412), .Size = New Size(100, 36)}
            Dim btnEdit As New Button() With {.Text = "Edit", .Location = New Point(252, 412), .Size = New Size(100, 36)}
            Dim btnDelete As New Button() With {.Text = "Delete", .Location = New Point(362, 412), .Size = New Size(100, 36)}
            AddHandler btnRefresh.Click, Sub() LoadData()
            AddHandler btnAdd.Click, AddressOf BtnAdd_Click
            AddHandler btnEdit.Click, AddressOf BtnEdit_Click
            AddHandler btnDelete.Click, AddressOf BtnDelete_Click

            pnlEdit = New Panel() With {
                .Location = New Point(32, 456),
                .Size = New Size(720, 72),
                .BorderStyle = BorderStyle.FixedSingle,
                .BackColor = ThemeBase.CardBg,
                .Visible = False
            }
            Dim lblName As New Label() With {.Text = "Course name:", .Location = New Point(16, 18), .AutoSize = True}
            txtName = New TextBox() With {.Location = New Point(16, 38), .Width = 280}
            Dim lblFee As New Label() With {.Text = "Fee (₹):", .Location = New Point(320, 18), .AutoSize = True}
            numFee = New NumericUpDown() With {.Location = New Point(320, 38), .Width = 120, .Minimum = 0, .Maximum = 9999999, .Value = 50000}
            btnSave = New Button() With {.Text = "Save", .Location = New Point(460, 34), .Size = New Size(90, 32)}
            btnCancelEdit = New Button() With {.Text = "Cancel", .Location = New Point(560, 34), .Size = New Size(90, 32)}
            AddHandler btnSave.Click, AddressOf BtnSave_Click
            AddHandler btnCancelEdit.Click, Sub() pnlEdit.Visible = False
            pnlEdit.Controls.AddRange(New Control() {lblName, txtName, lblFee, numFee, btnSave, btnCancelEdit})

            Me.Controls.Add(pnlEdit)
            Me.Controls.AddRange(New Control() {lblTitle, lblSub, dgvCourses, btnRefresh, btnAdd, btnEdit, btnDelete})
        End Sub

        Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
            editingCourseId = -1
            txtName.Clear()
            numFee.Value = 50000
            pnlEdit.Visible = True
        End Sub

        Private Sub BtnEdit_Click(sender As Object, e As EventArgs)
            If dgvCourses.CurrentRow Is Nothing Then
                MessageBox.Show("Select a course to edit.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim id = Convert.ToInt32(dgvCourses.CurrentRow.Cells("Id").Value)
            Dim c = courseService.GetCourseById(id)
            If c Is Nothing Then
                MessageBox.Show("Course not found.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            editingCourseId = c.Id
            txtName.Text = c.Name
            numFee.Value = c.Fee
            pnlEdit.Visible = True
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As EventArgs)
            Try
                If editingCourseId < 0 Then
                    courseService.AddCourse(New Course() With {.Name = txtName.Text.Trim(), .Fee = numFee.Value})
                    MessageBox.Show("Course added.", "Course Management", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    courseService.UpdateCourse(New Course() With {.Id = editingCourseId, .Name = txtName.Text.Trim(), .Fee = numFee.Value})
                    MessageBox.Show("Course updated.", "Course Management", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                pnlEdit.Visible = False
                LoadData()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
            If dgvCourses.CurrentRow Is Nothing Then
                MessageBox.Show("Select a course to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            If MessageBox.Show("Delete this course? Students linked to it may be affected.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
            Try
                Dim id = Convert.ToInt32(dgvCourses.CurrentRow.Cells("Id").Value)
                courseService.DeleteCourse(id)
                MessageBox.Show("Course deleted.", "Course Management", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadData()
            Catch ex As Exception
                MessageBox.Show("Delete failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub LoadData()
            Try
                bsCourses.DataSource = courseService.GetAllCourses()
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Course Management", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace
