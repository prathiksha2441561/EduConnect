Imports EduConnect.DAL
Imports System.Collections.Generic
Imports System.Linq

Namespace BLL
    Public Class EnrollmentService
        Private ReadOnly _enrollmentRepo As New EnrollmentRepository()
        Private ReadOnly _courseRepo As New CourseRepository()
        Private ReadOnly _paymentRepo As New PaymentRepository()

        Public Function GetMyEnrollments(studentId As Integer) As List(Of Enrollment)
            Return _enrollmentRepo.GetByStudent(studentId)
        End Function

        Public Function GetAvailableCourses(studentId As Integer) As List(Of Course)
            Dim all = _courseRepo.GetAll()
            Dim enrolled = _enrollmentRepo.GetByStudent(studentId)
            Dim enrolledIds As New HashSet(Of Integer)(enrolled.Where(Function(e) e.Status = "Enrolled").Select(Function(e) e.CourseId))
            Return all.Where(Function(c) Not enrolledIds.Contains(c.Id)).ToList()
        End Function

        Public Sub Enroll(studentId As Integer, courseId As Integer)
            If _enrollmentRepo.Exists(studentId, courseId) Then Throw New Exception("Already enrolled in this course.")
            Dim course = _courseRepo.GetById(courseId)
            If course Is Nothing Then Throw New Exception("Course not found.")
            Dim enr As New Enrollment() With {
                .StudentId = studentId,
                .CourseId = courseId,
                .EnrolledDate = DateTime.Now,
                .Status = "Enrolled"
            }
            Dim newId = _enrollmentRepo.AddReturnId(enr)
            Dim payment As New Payment() With {
                .EnrollmentId = newId,
                .Amount = course.Fee,
                .PaidDate = Nothing,
                .Status = "Pending"
            }
            _paymentRepo.Add(payment)
        End Sub
    End Class
End Namespace
