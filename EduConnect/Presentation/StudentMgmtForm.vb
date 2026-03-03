Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.BLL
Imports EduConnect.DAL

Namespace Presentation
    Public Class StudentMgmtForm
        Inherits Form

        Private dgvStudents As DataGridView
        Private studentService As StudentService
        Private bsStudents As BindingSource
        Private pnlEdit As Panel
        Private txtName As TextBox
        Private txtEmail As TextBox
        Private numCourseId As NumericUpDown
        Private btnSave As Button
        Private btnCancelEdit As Button
        Private editingStudentId As Integer = -1

        Public Sub New()
            studentService = New StudentService(New StudentRepository())
            bsStudents = New BindingSource()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
            LoadData()
        End Sub

        Private Sub InitializeComponent()
            Dim lblTitle As New Label() With {.Text = "Student Management Panel", .Font = New Font("Segoe UI", 16), .Location = New Point(20, 20), .AutoSize = True}

            dgvStudents = New DataGridView() With {
                .Location = New Point(20, 70),
                .Size = New Size(700, 280),
                .AutoGenerateColumns = True,
                .DataSource = bsStudents,
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                .MultiSelect = False
            }

            Dim btnRefresh As New Button() With {.Text = "Refresh", .Location = New Point(20, 360), .Size = New Size(100, 35)}
            AddHandler btnRefresh.Click, Sub() LoadData()

            Dim btnAdd As New Button() With {.Text = "Add New", .Location = New Point(130, 360), .Size = New Size(100, 35)}
            AddHandler btnAdd.Click, AddressOf BtnAdd_Click

            Dim btnEdit As New Button() With {.Text = "Edit", .Location = New Point(240, 360), .Size = New Size(100, 35)}
            AddHandler btnEdit.Click, AddressOf BtnEdit_Click

            Dim btnDelete As New Button() With {.Text = "Delete", .Location = New Point(350, 360), .Size = New Size(100, 35)}
            AddHandler btnDelete.Click, AddressOf BtnDelete_Click

            ' Add/Edit panel (hidden initially)
            pnlEdit = New Panel() With {.Location = New Point(20, 400), .Size = New Size(700, 80), .BorderStyle = BorderStyle.FixedSingle, .Visible = False}
            Dim lblName As New Label() With {.Text = "Name:", .Location = New Point(10, 12), .AutoSize = True}
            txtName = New TextBox() With {.Location = New Point(60, 10), .Width = 180}
            Dim lblEmail As New Label() With {.Text = "Email:", .Location = New Point(260, 12), .AutoSize = True}
            txtEmail = New TextBox() With {.Location = New Point(310, 10), .Width = 180}
            Dim lblCourse As New Label() With {.Text = "Course Id:", .Location = New Point(500, 12), .AutoSize = True}
            numCourseId = New NumericUpDown() With {.Location = New Point(570, 10), .Width = 80, .Minimum = 1, .Maximum = 999, .Value = 1}
            btnSave = New Button() With {.Text = "Save", .Location = New Point(10, 45), .Size = New Size(80, 28)}
            btnCancelEdit = New Button() With {.Text = "Cancel", .Location = New Point(100, 45), .Size = New Size(80, 28)}
            AddHandler btnSave.Click, AddressOf BtnSave_Click
            AddHandler btnCancelEdit.Click, AddressOf BtnCancelEdit_Click
            pnlEdit.Controls.AddRange(New Control() {lblName, txtName, lblEmail, txtEmail, lblCourse, numCourseId, btnSave, btnCancelEdit})

            Me.Controls.Add(pnlEdit)
            Me.Controls.AddRange(New Control() {lblTitle, dgvStudents, btnRefresh, btnAdd, btnEdit, btnDelete})
        End Sub

        Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
            editingStudentId = -1
            txtName.Clear()
            txtEmail.Clear()
            numCourseId.Value = 1
            pnlEdit.Visible = True
        End Sub

        Private Sub BtnEdit_Click(sender As Object, e As EventArgs)
            If dgvStudents.CurrentRow Is Nothing Then
                MessageBox.Show("Please select a student to edit.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim id As Integer = Convert.ToInt32(dgvStudents.CurrentRow.Cells("Id").Value)
            Dim student = studentService.GetStudentById(id)
            If student Is Nothing Then
                MessageBox.Show("Student not found or database unavailable.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            editingStudentId = student.Id
            txtName.Text = student.Name
            txtEmail.Text = student.Email
            numCourseId.Value = If(student.CourseId <= 0, 1, student.CourseId)
            pnlEdit.Visible = True
        End Sub

        Private Sub BtnSave_Click(sender As Object, e As EventArgs)
            Try
                If editingStudentId < 0 Then
                    Dim s As New Student() With {.Name = txtName.Text.Trim(), .Email = txtEmail.Text.Trim(), .CourseId = CInt(numCourseId.Value)}
                    studentService.AddStudent(s)
                    MessageBox.Show("Student added successfully.", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Dim s As New Student() With {.Id = editingStudentId, .Name = txtName.Text.Trim(), .Email = txtEmail.Text.Trim(), .CourseId = CInt(numCourseId.Value)}
                    studentService.UpdateStudent(s)
                    MessageBox.Show("Student updated successfully.", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
                pnlEdit.Visible = False
                LoadData()
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Validation / Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub BtnCancelEdit_Click(sender As Object, e As EventArgs)
            pnlEdit.Visible = False
        End Sub

        Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
            If dgvStudents.CurrentRow Is Nothing Then
                MessageBox.Show("Please select a student to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            If MessageBox.Show("Delete this student?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
            Try
                Dim id As Integer = Convert.ToInt32(dgvStudents.CurrentRow.Cells("Id").Value)
                studentService.DeleteStudent(id)
                MessageBox.Show("Student deleted.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadData()
            Catch ex As Exception
                MessageBox.Show("Delete failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub LoadData()
            Try
                bsStudents.DataSource = studentService.GetAllStudents()
            Catch ex As Exception
                MessageBox.Show("DB Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub
    End Class
End Namespace
