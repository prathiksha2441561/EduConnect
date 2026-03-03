Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.BLL
Imports EduConnect.Core

Namespace Presentation
    Public Class MyPaymentsForm
        Inherits Form

        Private dgvPayments As DataGridView
        Private _paymentService As New PaymentService()
        Private _enrollmentService As New EnrollmentService()
        Private listPayments As New List(Of PaymentDisplay)()

        Private Class PaymentDisplay
            Public Property Id As Integer
            Public Property CourseName As String
            Public Property Amount As Decimal
            Public Property Status As String
            Public Property PaidDate As String
        End Class

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
                .Text = "My Payments",
                .Font = New Font("Segoe UI", 20, FontStyle.Bold),
                .Location = New Point(32, 24),
                .AutoSize = True,
                .ForeColor = ThemeBase.Accent
            }
            Dim lblSub As New Label() With {
                .Text = "View and pay your course fees.",
                .Font = New Font("Segoe UI", 10),
                .Location = New Point(36, 64),
                .AutoSize = True,
                .ForeColor = ThemeBase.TextMuted
            }

            dgvPayments = New DataGridView() With {
                .Location = New Point(32, 100),
                .Size = New Size(600, 280),
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                .MultiSelect = False
            }
            dgvPayments.Columns.Add("Id", "Payment Id")
            dgvPayments.Columns.Add("CourseName", "Course")
            dgvPayments.Columns.Add("Amount", "Amount (₹)")
            dgvPayments.Columns.Add("Status", "Status")
            dgvPayments.Columns.Add("PaidDate", "Paid on")
            ThemeBase.StyleControl(dgvPayments)

            Dim btnPay As New Button() With {.Text = "Mark as paid", .Location = New Point(32, 392), .Size = New Size(140, 36)}
            AddHandler btnPay.Click, AddressOf BtnPay_Click
            Dim btnRefresh As New Button() With {.Text = "Refresh", .Location = New Point(182, 392), .Size = New Size(100, 36)}
            AddHandler btnRefresh.Click, Sub() LoadData()

            Me.Controls.AddRange(New Control() {lblTitle, lblSub, dgvPayments, btnPay, btnRefresh})
        End Sub

        Private Sub LoadData()
            Dim studentId = CurrentStudentId()
            If studentId <= 0 Then Return
            listPayments.Clear()
            Try
                Dim payments = _paymentService.GetMyPayments(studentId)
                For Each p In payments
                    Dim enr = _paymentService.GetEnrollmentById(p.EnrollmentId)
                    Dim courseName = If(enr Is Nothing, "", _paymentService.GetCourseName(enr.CourseId))
                    listPayments.Add(New PaymentDisplay() With {
                        .Id = p.Id,
                        .CourseName = courseName,
                        .Amount = p.Amount,
                        .Status = p.Status,
                        .PaidDate = If(p.PaidDate.HasValue, p.PaidDate.Value.ToString("dd-MMM-yyyy"), "")
                    })
                Next
                dgvPayments.Rows.Clear()
                For Each d In listPayments
                    dgvPayments.Rows.Add(d.Id, d.CourseName, d.Amount.ToString("N2"), d.Status, d.PaidDate)
                Next
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message, "Payments", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub BtnPay_Click(sender As Object, e As EventArgs)
            If dgvPayments.CurrentRow Is Nothing Then
                MessageBox.Show("Select a payment.", "Payments", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim status = dgvPayments.CurrentRow.Cells("Status").Value?.ToString()
            If status = "Paid" Then
                MessageBox.Show("This payment is already marked paid.", "Payments", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim paymentId = Convert.ToInt32(dgvPayments.CurrentRow.Cells("Id").Value)
            _paymentService.MarkPaid(paymentId)
            MessageBox.Show("Payment recorded.", "Payments", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadData()
        End Sub
    End Class
End Namespace
