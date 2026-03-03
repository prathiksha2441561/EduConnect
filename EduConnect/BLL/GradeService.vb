Imports EduConnect.DAL
Imports System.Collections.Generic
Imports System.Linq

Namespace BLL
    Public Class GradeService
        Private ReadOnly _gradeRepo As New GradeRepository()
        Private ReadOnly _studentRepo As New StudentRepository()
        Private ReadOnly _courseRepo As New CourseRepository()

        Public Sub AddGrade(grade As Grade)
            If grade.GradePoint < 0 OrElse grade.GradePoint > 10 Then Throw New Exception("Grade point must be between 0 and 10.")
            _gradeRepo.Add(grade)
        End Sub

        Public Function GetGradesByStudent(studentId As Integer) As List(Of Grade)
            Return _gradeRepo.GetByStudent(studentId)
        End Function

        Public Function GetAllGrades() As List(Of Grade)
            Return _gradeRepo.GetAllGrades()
        End Function

        ''' <summary>Compute GPA as average of grade points (0-10 scale). Returns value 0-10.</summary>
        Public Function CalculateGpa(studentId As Integer) As Decimal?
            Dim grades = _gradeRepo.GetByStudent(studentId)
            If grades Is Nothing OrElse grades.Count = 0 Then Return Nothing
            Return grades.Average(Function(g) g.GradePoint)
        End Function

        ''' <summary>Convert 0-10 scale to 4.0 scale (e.g. 10 -> 4.0, 7.5 -> 3.0).</summary>
        Public Shared Function To4PointScale(grade10 As Decimal) As Decimal
            Return Math.Round(grade10 * 4D / 10D, 2)
        End Function

        Public Function GetStudent(id As Integer) As Student
            Return _studentRepo.GetById(id)
        End Function

        Public Function GetCourse(id As Integer) As Course
            Return _courseRepo.GetById(id)
        End Function

        Public Function GetAllStudents() As List(Of Student)
            Return _studentRepo.GetAll().ToList()
        End Function

        Public Function GetAllCourses() As List(Of Course)
            Return _courseRepo.GetAll().ToList()
        End Function
    End Class
End Namespace
