Imports System.Drawing
Imports System.Windows.Forms

Namespace Presentation
    Public Class MaterialsForm
        Inherits Form

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
        End Sub

        Private Sub InitializeComponent()
            Dim lblTitle As New Label() With {.Text = "Lecture Materials", .Font = New Font("Segoe UI", 24, FontStyle.Bold), .Location = New Point(40, 30), .AutoSize = True, .ForeColor = ThemeBase.Accent}
            Dim lblSub As New Label() With {.Text = "Download your encrypted lecture slides, notes, and assignments.", .Font = New Font("Segoe UI", 11), .Location = New Point(45, 80), .AutoSize = True, .ForeColor = ThemeBase.TextMuted}

            Dim listMats As New ListBox() With {
                .Location = New Point(40, 130),
                .Size = New Size(400, 200),
                .BackColor = ThemeBase.DarkBg,
                .ForeColor = Color.White,
                .Font = New Font("Segoe UI", 12),
                .BorderStyle = BorderStyle.FixedSingle
            }

            listMats.Items.AddRange({
                "📄 Module 1: Cloud Architecture.aes",
                "📄 Module 2: System Design.aes",
                "📄 Assignment 1: Distributed Systems.aes"
            })

            Dim btnDownload As New Button() With {.Text = "Download Selected", .Location = New Point(40, 350), .Size = New Size(200, 45)}

            Me.Controls.AddRange(New Control() {lblTitle, lblSub, listMats, btnDownload})
        End Sub
    End Class
End Namespace
