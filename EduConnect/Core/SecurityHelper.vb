Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Namespace Core
    Public Class SecurityHelper
        Private ReadOnly Key As Byte()
        Private ReadOnly IV As Byte()

        Public Sub New()
            Using aes As Aes = Aes.Create()
                ' In production, store securely. Using static for demo.
                Key = Encoding.UTF8.GetBytes("A3F8E92B47C5D61A70B3E84F59C1D2E0")
                IV = Encoding.UTF8.GetBytes("9A3F8B7C5E2D104F")
            End Using
        End Sub

        Public Sub EncryptFile(inputFile As String, outputFile As String)
            Using aes As Aes = Aes.Create()
                aes.Key = Key
                aes.IV = IV
                Using fsIn As New FileStream(inputFile, FileMode.Open)
                    Using fsOut As New FileStream(outputFile, FileMode.Create)
                        Using cs As New CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write)
                            fsIn.CopyTo(cs)
                        End Using
                    End Using
                End Using
            End Using
        End Sub

        Public Sub DecryptFile(inputFile As String, outputFile As String)
            Using aes As Aes = Aes.Create()
                aes.Key = Key
                aes.IV = IV
                Using fsIn As New FileStream(inputFile, FileMode.Open)
                    Using fsOut As New FileStream(outputFile, FileMode.Create)
                        Using cs As New CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read)
                            cs.CopyTo(fsOut)
                        End Using
                    End Using
                End Using
            End Using
        End Sub
    End Class
End Namespace
