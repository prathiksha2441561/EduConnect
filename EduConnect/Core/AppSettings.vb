Imports System.Threading

Namespace Core
    Public Class AppSettings
        Private Shared ReadOnly _instance As New Lazy(Of AppSettings)(Function() New AppSettings(), LazyThreadSafetyMode.ExecutionAndPublication)
        
        Public Shared ReadOnly Property Instance As AppSettings
            Get
                Return _instance.Value
            End Get
        End Property

        Public Property CurrentUser As String
        Public Property UserRole As String
        ''' <summary>Display name for profile (admin/student). Persisted per session.</summary>
        Public Property ProfileDisplayName As String
        ''' <summary>Email for profile (admin/student). Persisted per session.</summary>
        Public Property ProfileEmail As String
        Public Property ThemePreference As String = "Dark"
        Public Property ConnectionString As String = "Server=(localdb)\MSSQLLocalDB;Database=EduConnectDB;Trusted_Connection=True;"
        Public Property AppVersion As String = "1.0.0"
        ''' <summary>Currently logged-in student Id (for student role).</summary>
        Public Property CurrentStudentId As Integer?

        Private Sub New()
            ' Initialize defaults
        End Sub
    End Class
End Namespace
