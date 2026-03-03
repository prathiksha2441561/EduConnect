Imports System.Security.Cryptography
Imports System.Text
Imports EduConnect.DAL
Imports EduConnect.BLL

Namespace Core
    Public Class SessionManager
        Public Delegate Sub LoginAttemptEventHandler(username As String, success As Boolean, message As String)
        Public Event OnLoginAttempt As LoginAttemptEventHandler

        Private Const PasswordSalt As String = "randomsalt123"

        Public Function Authenticate(username As String, password As String, role As String) As Boolean
            Try
                ' 1. Try database (Users table) first
                Dim userRepo As New UserRepository()
                Dim user = userRepo.GetByUsername(username?.Trim())
                If user IsNot Nothing Then
                    Dim hashedInput = HashPassword(password, PasswordSalt)
                    If String.Equals(user.PasswordHash, hashedInput, StringComparison.Ordinal) AndAlso String.Equals(user.Role, role, StringComparison.OrdinalIgnoreCase) Then
                        SetSessionFromUser(user)
                        RaiseEvent OnLoginAttempt(user.Username, True, "Login successful.")
                        Return True
                    End If
                    RaiseEvent OnLoginAttempt(username, False, "Invalid password or role.")
                    Return False
                End If

                ' 2. Fallback: mock logins (when DB has no users or for demo)
                If role = "Admin" AndAlso username = "admin" AndAlso password = "password" Then
                    AppSettings.Instance.CurrentUser = username
                    AppSettings.Instance.UserRole = "Admin"
                    AppSettings.Instance.CurrentStudentId = Nothing
                    AppSettings.Instance.ProfileDisplayName = "Administrator"
                    RaiseEvent OnLoginAttempt(username, True, "Login successful.")
                    Return True
                End If
                If role = "Instructor" AndAlso username = "instructor" AndAlso password = "password" Then
                    AppSettings.Instance.CurrentUser = username
                    AppSettings.Instance.UserRole = "Instructor"
                    AppSettings.Instance.CurrentStudentId = Nothing
                    RaiseEvent OnLoginAttempt(username, True, "Login successful.")
                    Return True
                End If
                If role = "Student" AndAlso username = "student" AndAlso password = "password" Then
                    AppSettings.Instance.CurrentUser = username
                    AppSettings.Instance.UserRole = "Student"
                    Dim studentRepo As New StudentRepository()
                    Dim firstStudent = studentRepo.GetFirst()
                    If firstStudent IsNot Nothing Then AppSettings.Instance.CurrentStudentId = firstStudent.Id
                    RaiseEvent OnLoginAttempt(username, True, "Login successful.")
                    Return True
                End If

                RaiseEvent OnLoginAttempt(username, False, "Invalid credentials.")
                Return False
            Catch ex As Exception
                RaiseEvent OnLoginAttempt(username, False, ex.Message)
                Return False
            End Try
        End Function

        Private Sub SetSessionFromUser(user As User)
            AppSettings.Instance.CurrentUser = user.Username
            AppSettings.Instance.UserRole = user.Role
            AppSettings.Instance.ProfileDisplayName = user.DisplayName
            AppSettings.Instance.ProfileEmail = user.Email
            If String.Equals(user.Role, "Student", StringComparison.OrdinalIgnoreCase) Then
                Dim studentRepo As New StudentRepository()
                Dim student = studentRepo.GetByUserId(user.Id)
                AppSettings.Instance.CurrentStudentId = If(student IsNot Nothing, New Integer?(student.Id), Nothing)
            Else
                AppSettings.Instance.CurrentStudentId = Nothing
            End If
        End Sub

        Public Shared Function HashPassword(password As String, salt As String) As String
            Using sha256 As SHA256 = SHA256.Create()
                Dim bytes As Byte() = Encoding.UTF8.GetBytes(password & salt)
                Dim hash As Byte() = sha256.ComputeHash(bytes)
                Dim sb As New StringBuilder()
                For Each b As Byte In hash
                    sb.Append(b.ToString("x2"))
                Next
                Return sb.ToString()
            End Using
        End Function
    End Class
End Namespace
