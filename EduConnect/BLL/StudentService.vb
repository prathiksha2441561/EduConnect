Imports EduConnect.DAL
Imports System.Collections.Generic

Namespace BLL
    Public Class StudentService
        Private ReadOnly _repository As IRepository(Of Student)

        Public Sub New(repository As IRepository(Of Student))
            _repository = repository
        End Sub

        Public Function GetAllStudents() As IEnumerable(Of Student)
            Return _repository.GetAll()
        End Function

        Public Sub AddStudent(student As Student)
            If String.IsNullOrWhiteSpace(student.Name) Then Throw New Exception("Name cannot be empty.")
            If String.IsNullOrWhiteSpace(student.Email) Then Throw New Exception("Email cannot be empty.")
            _repository.Add(student)
        End Sub

        Public Sub UpdateStudent(student As Student)
            If student Is Nothing OrElse student.Id <= 0 Then Throw New Exception("Invalid student for update.")
            If String.IsNullOrWhiteSpace(student.Name) Then Throw New Exception("Name cannot be empty.")
            If String.IsNullOrWhiteSpace(student.Email) Then Throw New Exception("Email cannot be empty.")
            _repository.Update(student)
        End Sub

        Public Sub DeleteStudent(id As Integer)
            _repository.Delete(id)
        End Sub

        Public Function GetStudentById(id As Integer) As Student
            Return _repository.GetById(id)
        End Function
    End Class
End Namespace
