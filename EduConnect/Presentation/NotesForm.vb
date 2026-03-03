Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO

Namespace Presentation
    Public Class NotesForm
        Inherits Form

        Private rtbNotes As RichTextBox
        Private currentFilePath As String = Nothing

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
        End Sub

        Private Sub InitializeComponent()
            Dim menuStrip As New MenuStrip()

            ' File menu
            Dim mnuFile As New ToolStripMenuItem("&File")
            Dim mnuNew As New ToolStripMenuItem("&New", Nothing, AddressOf FileNew)
            Dim mnuOpen As New ToolStripMenuItem("&Open...", Nothing, AddressOf OpenFile)
            Dim mnuSave As New ToolStripMenuItem("&Save", Nothing, AddressOf SaveFile)
            Dim mnuSaveAs As New ToolStripMenuItem("Save &As...", Nothing, AddressOf SaveFileAs)
            mnuFile.DropDownItems.Add(mnuNew)
            mnuFile.DropDownItems.Add(mnuOpen)
            mnuFile.DropDownItems.Add(New ToolStripSeparator())
            mnuFile.DropDownItems.Add(mnuSave)
            mnuFile.DropDownItems.Add(mnuSaveAs)
            mnuFile.DropDownItems.Add(New ToolStripSeparator())
            mnuFile.DropDownItems.Add(New ToolStripMenuItem("E&xit", Nothing, Sub() Application.Exit()))

            ' Format menu
            Dim mnuFormat As New ToolStripMenuItem("F&ormat")
            mnuFormat.DropDownItems.Add(New ToolStripMenuItem("&Font...", Nothing, AddressOf FormatFont))
            mnuFormat.DropDownItems.Add(New ToolStripMenuItem("&Color...", Nothing, AddressOf FormatColor))

            menuStrip.Items.Add(mnuFile)
            menuStrip.Items.Add(mnuFormat)
            Me.MainMenuStrip = menuStrip

            rtbNotes = New RichTextBox() With {
                .Dock = DockStyle.Fill,
                .BackColor = ThemeBase.InputBg,
                .ForeColor = ThemeBase.TextColor,
                .BorderStyle = BorderStyle.None,
                .Font = New Font("Segoe UI", 11)
            }

            Me.Controls.Add(rtbNotes)
            Me.Controls.Add(menuStrip)
        End Sub

        Private Sub FileNew(sender As Object, e As EventArgs)
            If rtbNotes.Modified AndAlso MessageBox.Show("Discard changes?", "Notepad", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return
            rtbNotes.Clear()
            rtbNotes.Modified = False
            currentFilePath = Nothing
        End Sub

        Private Sub OpenFile(sender As Object, e As EventArgs)
            Using ofd As New OpenFileDialog() With {.Filter = "Rich Text|*.rtf|Text Files|*.txt|All Files|*.*"}
                If ofd.ShowDialog() = DialogResult.OK Then
                    Try
                        If ofd.FileName.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase) Then
                            rtbNotes.LoadFile(ofd.FileName)
                        Else
                            rtbNotes.Text = File.ReadAllText(ofd.FileName)
                        End If
                        currentFilePath = ofd.FileName
                        rtbNotes.Modified = False
                    Catch ex As Exception
                        MessageBox.Show("Could not open file: " & ex.Message, "Notepad", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End Using
        End Sub

        Private Sub SaveFile(sender As Object, e As EventArgs)
            If String.IsNullOrEmpty(currentFilePath) Then
                SaveFileAs(sender, e)
                Return
            End If
            Try
                If currentFilePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase) Then
                    rtbNotes.SaveFile(currentFilePath)
                Else
                    File.WriteAllText(currentFilePath, rtbNotes.Text)
                End If
                rtbNotes.Modified = False
                MessageBox.Show("Saved.", "Notepad", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Could not save: " & ex.Message, "Notepad", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        Private Sub SaveFileAs(sender As Object, e As EventArgs)
            Using sfd As New SaveFileDialog() With {.Filter = "Rich Text|*.rtf|Text Files|*.txt|All Files|*.*"}
                If sfd.ShowDialog() = DialogResult.OK Then
                    currentFilePath = sfd.FileName
                    Try
                        If currentFilePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase) Then
                            rtbNotes.SaveFile(currentFilePath)
                        Else
                            File.WriteAllText(currentFilePath, rtbNotes.Text)
                        End If
                        rtbNotes.Modified = False
                        MessageBox.Show("Saved.", "Notepad", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MessageBox.Show("Could not save: " & ex.Message, "Notepad", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End Using
        End Sub

        Private Sub FormatFont(sender As Object, e As EventArgs)
            Using fd As New FontDialog()
                If fd.ShowDialog() = DialogResult.OK Then rtbNotes.SelectionFont = fd.Font
            End Using
        End Sub

        Private Sub FormatColor(sender As Object, e As EventArgs)
            Using cd As New ColorDialog()
                If cd.ShowDialog() = DialogResult.OK Then rtbNotes.SelectionColor = cd.Color
            End Using
        End Sub
    End Class
End Namespace
