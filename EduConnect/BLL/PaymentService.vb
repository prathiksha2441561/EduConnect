Imports EduConnect.DAL
Imports System.Collections.Generic

Namespace BLL
    Public Class PaymentService
        Private ReadOnly _paymentRepo As New PaymentRepository()
        Private ReadOnly _enrollmentRepo As New EnrollmentRepository()
        Private ReadOnly _courseRepo As New CourseRepository()

        Public Function GetMyPayments(studentId As Integer) As List(Of Payment)
            Return _paymentRepo.GetByStudent(studentId)
        End Function

        Public Sub MarkPaid(paymentId As Integer)
            _paymentRepo.MarkPaid(paymentId)
        End Sub

        Public Function GetEnrollmentById(id As Integer) As Enrollment
            Return _enrollmentRepo.GetById(id)
        End Function

        Public Function GetCourseName(courseId As Integer) As String
            Dim c = _courseRepo.GetById(courseId)
            Return If(c Is Nothing, "Course #" & courseId, c.Name)
        End Function
    End Class
End Namespace
