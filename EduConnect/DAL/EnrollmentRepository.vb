Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports EduConnect.Core
Imports EduConnect.BLL

Namespace DAL
    Public Class EnrollmentRepository
        Private ReadOnly connectionString As String

        Public Sub New()
            connectionString = AppSettings.Instance.ConnectionString
        End Sub

        Public Sub Add(enrollment As Enrollment)
            AddReturnId(enrollment)
        End Sub

        ''' <summary>Returns the new Enrollments.Id.</summary>
        Public Function AddReturnId(enrollment As Enrollment) As Integer
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand("INSERT INTO Enrollments (StudentId, CourseId, EnrolledDate, Status) VALUES (@StudentId, @CourseId, @EnrolledDate, @Status); SELECT CAST(SCOPE_IDENTITY() AS INT);", conn)
                cmd.Parameters.AddWithValue("@StudentId", enrollment.StudentId)
                cmd.Parameters.AddWithValue("@CourseId", enrollment.CourseId)
                cmd.Parameters.AddWithValue("@EnrolledDate", enrollment.EnrolledDate)
                cmd.Parameters.AddWithValue("@Status", If(String.IsNullOrEmpty(enrollment.Status), "Enrolled", enrollment.Status))
                conn.Open()
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Function

        Public Function GetByStudent(studentId As Integer) As List(Of Enrollment)
            Dim list As New List(Of Enrollment)()
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT Id, StudentId, CourseId, EnrolledDate, Status FROM Enrollments WHERE StudentId = @StudentId ORDER BY EnrolledDate DESC", conn)
                    cmd.Parameters.AddWithValue("@StudentId", studentId)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New Enrollment() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .StudentId = Convert.ToInt32(reader("StudentId")),
                                .CourseId = Convert.ToInt32(reader("CourseId")),
                                .EnrolledDate = Convert.ToDateTime(reader("EnrolledDate")),
                                .Status = reader("Status").ToString()
                            })
                        End While
                    End Using
                End Using
            Catch
            End Try
            Return list
        End Function

        Public Function GetById(id As Integer) As Enrollment
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT Id, StudentId, CourseId, EnrolledDate, Status FROM Enrollments WHERE Id = @Id", conn)
                    cmd.Parameters.AddWithValue("@Id", id)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New Enrollment() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .StudentId = Convert.ToInt32(reader("StudentId")),
                                .CourseId = Convert.ToInt32(reader("CourseId")),
                                .EnrolledDate = Convert.ToDateTime(reader("EnrolledDate")),
                                .Status = reader("Status").ToString()
                            }
                        End If
                    End Using
                End Using
            Catch
            End Try
            Return Nothing
        End Function

        Public Function Exists(studentId As Integer, courseId As Integer) As Boolean
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT 1 FROM Enrollments WHERE StudentId = @StudentId AND CourseId = @CourseId AND Status = 'Enrolled'", conn)
                    cmd.Parameters.AddWithValue("@StudentId", studentId)
                    cmd.Parameters.AddWithValue("@CourseId", courseId)
                    conn.Open()
                    Return cmd.ExecuteScalar() IsNot Nothing
                End Using
            Catch
            End Try
            Return False
        End Function
    End Class
End Namespace
