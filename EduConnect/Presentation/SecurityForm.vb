Imports System.Drawing
Imports System.Windows.Forms
Imports EduConnect.Core
Imports System.IO

Namespace Presentation
    Public Class SecurityForm
        Inherits Form

        Private secHelper As New SecurityHelper()

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
        End Sub

        Private Sub InitializeComponent()
            Dim lblTitle As New Label() With {.Text = "Assignment Encryption System", .Font = New Font("Segoe UI", 16), .Location = New Point(20, 20), .AutoSize = True}
            
            Dim btnEnc As New Button() With {.Text = "Upload & Encrypt File", .Location = New Point(20, 80), .Size = New Size(200, 40)}
            AddHandler btnEnc.Click, AddressOf EncryptAction

            Dim btnDec As New Button() With {.Text = "Decrypt & Download File", .Location = New Point(20, 140), .Size = New Size(200, 40)}
            AddHandler btnDec.Click, AddressOf DecryptAction

            Me.Controls.AddRange(New Control() {lblTitle, btnEnc, btnDec})
        End Sub

        Private Sub EncryptAction()
            Using ofd As New OpenFileDialog()
                If ofd.ShowDialog() = DialogResult.OK Then
                    Using sfd As New SaveFileDialog() With {.Filter = "Encrypted File|*.aes"}
                        If sfd.ShowDialog() = DialogResult.OK Then
                            secHelper.EncryptFile(ofd.FileName, sfd.FileName)
                            MessageBox.Show("File Encrypted via AES-256!")
                        End If
                    End Using
                End If
            End Using
        End Sub

        Private Sub DecryptAction()
            Using ofd As New OpenFileDialog() With {.Filter = "Encrypted File|*.aes"}
                If ofd.ShowDialog() = DialogResult.OK Then
                    Using sfd As New SaveFileDialog()
                        If sfd.ShowDialog() = DialogResult.OK Then
                            Try
                                secHelper.DecryptFile(ofd.FileName, sfd.FileName)
                                MessageBox.Show("File Decrypted Successfully!")
                            Catch ex As Exception
                                MessageBox.Show("Decryption Failed! Invalid Key/IV or File.")
                            End Try
                        End If
                    End Using
                End If
            End Using
        End Sub
    End Class
End Namespace
