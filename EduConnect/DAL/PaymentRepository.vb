Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports EduConnect.Core
Imports EduConnect.BLL

Namespace DAL
    Public Class PaymentRepository
        Private ReadOnly connectionString As String

        Public Sub New()
            connectionString = AppSettings.Instance.ConnectionString
        End Sub

        Public Sub Add(payment As Payment)
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand("INSERT INTO Payments (EnrollmentId, Amount, PaidDate, Status) VALUES (@EnrollmentId, @Amount, @PaidDate, @Status)", conn)
                cmd.Parameters.AddWithValue("@EnrollmentId", payment.EnrollmentId)
                cmd.Parameters.AddWithValue("@Amount", payment.Amount)
                cmd.Parameters.AddWithValue("@PaidDate", If(payment.PaidDate.HasValue, CObj(payment.PaidDate.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@Status", If(String.IsNullOrEmpty(payment.Status), "Pending", payment.Status))
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub MarkPaid(id As Integer)
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand("UPDATE Payments SET Status = 'Paid', PaidDate = GETDATE() WHERE Id = @Id", conn)
                cmd.Parameters.AddWithValue("@Id", id)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Public Function GetByEnrollment(enrollmentId As Integer) As List(Of Payment)
            Dim list As New List(Of Payment)()
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT Id, EnrollmentId, Amount, PaidDate, Status FROM Payments WHERE EnrollmentId = @EnrollmentId", conn)
                    cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New Payment() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .EnrollmentId = Convert.ToInt32(reader("EnrollmentId")),
                                .Amount = Convert.ToDecimal(reader("Amount")),
                                .PaidDate = If(IsDBNull(reader("PaidDate")), Nothing, New DateTime?(Convert.ToDateTime(reader("PaidDate")))),
                                .Status = reader("Status").ToString()
                            })
                        End While
                    End Using
                End Using
            Catch
            End Try
            Return list
        End Function

        Public Function GetByStudent(studentId As Integer) As List(Of Payment)
            Dim list As New List(Of Payment)()
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT p.Id, p.EnrollmentId, p.Amount, p.PaidDate, p.Status FROM Payments p INNER JOIN Enrollments e ON p.EnrollmentId = e.Id WHERE e.StudentId = @StudentId ORDER BY p.Id DESC", conn)
                    cmd.Parameters.AddWithValue("@StudentId", studentId)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New Payment() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .EnrollmentId = Convert.ToInt32(reader("EnrollmentId")),
                                .Amount = Convert.ToDecimal(reader("Amount")),
                                .PaidDate = If(IsDBNull(reader("PaidDate")), Nothing, New DateTime?(Convert.ToDateTime(reader("PaidDate")))),
                                .Status = reader("Status").ToString()
                            })
                        End While
                    End Using
                End Using
            Catch
            End Try
            Return list
        End Function
    End Class
End Namespace
