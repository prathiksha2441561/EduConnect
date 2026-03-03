Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports EduConnect.Core
Imports EduConnect.BLL

Namespace DAL
    Public Class CourseRepository
        Implements IRepository(Of Course)

        Private ReadOnly connectionString As String

        Public Sub New()
            connectionString = AppSettings.Instance.ConnectionString
        End Sub

        Public Sub Add(entity As Course) Implements IRepository(Of Course).Add
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand("INSERT INTO Courses (Name, Fee) VALUES (@Name, @Fee)", conn)
                cmd.Parameters.AddWithValue("@Name", entity.Name)
                cmd.Parameters.AddWithValue("@Fee", entity.Fee)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub Delete(id As Integer) Implements IRepository(Of Course).Delete
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand("DELETE FROM Courses WHERE Id = @Id", conn)
                cmd.Parameters.AddWithValue("@Id", id)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Public Function GetAll() As IEnumerable(Of Course) Implements IRepository(Of Course).GetAll
            Dim list As New List(Of Course)()
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT Id, Name, Fee FROM Courses", conn)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            list.Add(New Course() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .Name = reader("Name").ToString(),
                                .Fee = Convert.ToDecimal(reader("Fee"))
                            })
                        End While
                    End Using
                End Using
            Catch
                list.Add(New Course With {.Id = 1, .Name = "B.Tech Computer Science", .Fee = 150000D})
            End Try
            Return list
        End Function

        Public Function GetById(id As Integer) As Course Implements IRepository(Of Course).GetById
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT Id, Name, Fee FROM Courses WHERE Id = @Id", conn)
                    cmd.Parameters.AddWithValue("@Id", id)
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New Course() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .Name = reader("Name").ToString(),
                                .Fee = Convert.ToDecimal(reader("Fee"))
                            }
                        End If
                    End Using
                End Using
            Catch
            End Try
            Return Nothing
        End Function

        Public Sub Update(entity As Course) Implements IRepository(Of Course).Update
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand("UPDATE Courses SET Name=@Name, Fee=@Fee WHERE Id=@Id", conn)
                cmd.Parameters.AddWithValue("@Id", entity.Id)
                cmd.Parameters.AddWithValue("@Name", entity.Name)
                cmd.Parameters.AddWithValue("@Fee", entity.Fee)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Sub
    End Class
End Namespace
