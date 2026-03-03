Imports EduConnect.DAL
Imports System.Collections.Generic

Namespace BLL
    Public Class CourseService
        Private ReadOnly _repository As IRepository(Of Course)

        Public Sub New(repository As IRepository(Of Course))
            _repository = repository
        End Sub

        Public Function GetAllCourses() As IEnumerable(Of Course)
            Return _repository.GetAll()
        End Function

        Public Function GetCourseById(id As Integer) As Course
            Return _repository.GetById(id)
        End Function

        Public Sub AddCourse(course As Course)
            If String.IsNullOrWhiteSpace(course.Name) Then Throw New Exception("Course name cannot be empty.")
            If course.Fee < 0 Then Throw New Exception("Fee cannot be negative.")
            _repository.Add(course)
        End Sub

        Public Sub UpdateCourse(course As Course)
            If course Is Nothing OrElse course.Id <= 0 Then Throw New Exception("Invalid course for update.")
            If String.IsNullOrWhiteSpace(course.Name) Then Throw New Exception("Course name cannot be empty.")
            If course.Fee < 0 Then Throw New Exception("Fee cannot be negative.")
            _repository.Update(course)
        End Sub

        Public Sub DeleteCourse(id As Integer)
            _repository.Delete(id)
        End Sub
    End Class
End Namespace
