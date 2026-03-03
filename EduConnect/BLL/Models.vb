Namespace BLL
    Public Class User
        Public Property Id As Integer
        Public Property Username As String
        Public Property PasswordHash As String
        Public Property Role As String
        Public Property Email As String
        Public Property DisplayName As String
        Public Property CreatedAt As DateTime
    End Class

    Public Class Student
        Public Property Id As Integer
        Public Property UserId As Integer?
        Public Property Name As String
        Public Property Email As String
        Public Property CourseId As Integer
    End Class

    Public Class Course
        Public Property Id As Integer
        Public Property Name As String
        Public Property Fee As Decimal
    End Class

    Public Class Enrollment
        Public Property Id As Integer
        Public Property StudentId As Integer
        Public Property CourseId As Integer
        Public Property EnrolledDate As DateTime
        Public Property Status As String
    End Class

    Public Class Payment
        Public Property Id As Integer
        Public Property EnrollmentId As Integer
        Public Property Amount As Decimal
        Public Property PaidDate As DateTime?
        Public Property Status As String
    End Class

    Public Class Grade
        Public Property Id As Integer
        Public Property StudentId As Integer
        Public Property CourseId As Integer
        Public Property GradePoint As Decimal
        Public Property Semester As String
    End Class
End Namespace
