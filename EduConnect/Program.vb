Imports System.Windows.Forms
Imports EduConnect.Presentation

Module Program
    <STAThread()>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New LoginForm())
    End Sub
End Module
