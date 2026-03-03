Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports EduConnect.Core
Imports EduConnect.BLL

Namespace DAL
    Public Class GradeRepository
        Private ReadOnly connectionString As String

        Public Sub New()
            connectionString = AppSettings.Instance.ConnectionString
        End Sub

        Public Sub Add(grade As Grade)
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand("INSERT INTO Grades (StudentId, CourseId, GradePoint, Semester) VALUES (@StudentId, @CourseId, @GradePoint, @Semester)", conn)
                cmd.Parameters.AddWithValue("@StudentId", grade.StudentId)
                cmd.Parameters.AddWithValue("@CourseId", grade.CourseId)
                cmd.Parameters.AddWithValue("@GradePoint", grade.GradePoint)
                cmd.Parameters.AddWithValue("@Semester", If(String.IsNullOrEmpty(grade.Semester), DBNull.Value, CObj(grade.Semester)))
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Public Function GetByStudent(studentId As Integer) As List(Of Grade)
            Dim list As New List(Of Grade)()
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT Id, StudentId, CourseId, GradePoint, Semester FROM Grades WHERE StudentId = @StudentId", conn)
                    cmd.Parameters.AddWithValue("@StudentId", studentId)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New Grade() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .StudentId = Convert.ToInt32(reader("StudentId")),
                                .CourseId = Convert.ToInt32(reader("CourseId")),
                                .GradePoint = Convert.ToDecimal(reader("GradePoint")),
                                .Semester = If(IsDBNull(reader("Semester")), Nothing, reader("Semester").ToString())
                            })
                        End While
                    End Using
                End Using
            Catch
            End Try
            Return list
        End Function

        Public Function GetAllGrades() As List(Of Grade)
            Dim list As New List(Of Grade)()
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT Id, StudentId, CourseId, GradePoint, Semester FROM Grades", conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New Grade() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .StudentId = Convert.ToInt32(reader("StudentId")),
                                .CourseId = Convert.ToInt32(reader("CourseId")),
                                .GradePoint = Convert.ToDecimal(reader("GradePoint")),
                                .Semester = If(IsDBNull(reader("Semester")), Nothing, reader("Semester").ToString())
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
