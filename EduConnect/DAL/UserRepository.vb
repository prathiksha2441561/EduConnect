Imports System.Data.SqlClient
Imports EduConnect.Core
Imports EduConnect.BLL

Namespace DAL
    Public Class UserRepository
        Private ReadOnly connectionString As String

        Public Sub New()
            connectionString = AppSettings.Instance.ConnectionString
        End Sub

        Public Function GetByUsername(username As String) As User
            If String.IsNullOrWhiteSpace(username) Then Return Nothing
            Try
                Using conn As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand("SELECT Id, Username, PasswordHash, Role, Email, DisplayName, CreatedAt FROM Users WHERE Username = @Username", conn)
                    cmd.Parameters.AddWithValue("@Username", username.Trim())
                    conn.Open()
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Return New User() With {
                                .Id = Convert.ToInt32(reader("Id")),
                                .Username = reader("Username").ToString(),
                                .PasswordHash = reader("PasswordHash").ToString(),
                                .Role = reader("Role").ToString(),
                                .Email = reader("Email").ToString(),
                                .DisplayName = reader("DisplayName").ToString(),
                                .CreatedAt = Convert.ToDateTime(reader("CreatedAt"))
                            }
                        End If
                    End Using
                End Using
            Catch
            End Try
            Return Nothing
        End Function

        ''' <summary>Returns the new Users.Id.</summary>
        Public Function Add(user As User) As Integer
            Using conn As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand("INSERT INTO Users (Username, PasswordHash, Role, Email, DisplayName) VALUES (@Username, @PasswordHash, @Role, @Email, @DisplayName); SELECT CAST(SCOPE_IDENTITY() AS INT);", conn)
                cmd.Parameters.AddWithValue("@Username", user.Username.Trim())
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash)
                cmd.Parameters.AddWithValue("@Role", user.Role)
                cmd.Parameters.AddWithValue("@Email", user.Email.Trim())
                cmd.Parameters.AddWithValue("@DisplayName", If(String.IsNullOrWhiteSpace(user.DisplayName), user.Username, user.DisplayName))
                conn.Open()
                Return Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        End Function

        Public Function UsernameExists(username As String) As Boolean
            Return GetByUsername(username) IsNot Nothing
        End Function
    End Class
End Namespace
