Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Windows.Forms
Imports System.IO

Namespace Presentation
    Public Class WhiteboardForm
        Inherits Form

        Private pnlCanvas As Panel
        Private canvasBitmap As Bitmap
        Private canvasGraphics As Graphics
        Private isDrawing As Boolean = False
        Private startPoint As Point
        Private lastPoint As Point
        Private currentPen As Pen
        Private currentColor As Color = Color.White
        Private brushSize As Integer = 3
        Private tool As DrawTool = DrawTool.Pen
        Private canvasWidth As Integer = 800
        Private canvasHeight As Integer = 500

        Private Enum DrawTool
            Pen
            Eraser
            Line
            Rectangle
            Ellipse
        End Enum

        Public Sub New()
            currentPen = New Pen(currentColor, brushSize)
            currentPen.StartCap = LineCap.Round
            currentPen.EndCap = LineCap.Round
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
        End Sub

        Private Sub InitCanvas()
            Dim w = If(pnlCanvas IsNot Nothing AndAlso pnlCanvas.Width > 0, pnlCanvas.Width, canvasWidth)
            Dim h = If(pnlCanvas IsNot Nothing AndAlso pnlCanvas.Height > 0, pnlCanvas.Height, canvasHeight)
            w = Math.Max(100, w)
            h = Math.Max(100, h)
            If canvasBitmap IsNot Nothing Then canvasBitmap.Dispose()
            If canvasGraphics IsNot Nothing Then canvasGraphics.Dispose()
            canvasBitmap = New Bitmap(w, h)
            canvasGraphics = Graphics.FromImage(canvasBitmap)
            canvasGraphics.SmoothingMode = SmoothingMode.AntiAlias
            canvasGraphics.Clear(ThemeBase.DarkBg)
            If pnlCanvas IsNot Nothing Then pnlCanvas.Invalidate()
        End Sub

        Private Sub InitializeComponent()
            Dim menuStrip As New MenuStrip()
            Dim mnuFile As New ToolStripMenuItem("&File")
            mnuFile.DropDownItems.Add(New ToolStripMenuItem("&New", Nothing, AddressOf FileNew))
            mnuFile.DropDownItems.Add(New ToolStripMenuItem("&Open...", Nothing, AddressOf FileOpen))
            mnuFile.DropDownItems.Add(New ToolStripMenuItem("&Save As...", Nothing, AddressOf FileSaveAs))
            mnuFile.DropDownItems.Add(New ToolStripSeparator())
            mnuFile.DropDownItems.Add(New ToolStripMenuItem("Clear canvas", Nothing, AddressOf ClearCanvas))
            menuStrip.Items.Add(mnuFile)
            Me.MainMenuStrip = menuStrip

            Dim toolStrip As New ToolStrip()
            Dim btnPen As New ToolStripButton("Pen")
            AddHandler btnPen.Click, Sub()
                tool = DrawTool.Pen
                currentPen = New Pen(currentColor, brushSize)
            End Sub
            Dim btnEraser As New ToolStripButton("Eraser")
            AddHandler btnEraser.Click, Sub() tool = DrawTool.Eraser
            Dim btnLine As New ToolStripButton("Line")
            AddHandler btnLine.Click, Sub() tool = DrawTool.Line
            Dim btnRect As New ToolStripButton("Rectangle")
            AddHandler btnRect.Click, Sub() tool = DrawTool.Rectangle
            Dim btnEllipse As New ToolStripButton("Ellipse")
            AddHandler btnEllipse.Click, Sub() tool = DrawTool.Ellipse
            Dim sep As New ToolStripSeparator()
            Dim lblSize As New ToolStripLabel("Size:")
            Dim cmbSize As New ToolStripComboBox() With {.Width = 60}
            cmbSize.Items.AddRange({"1", "2", "3", "5", "8", "12"})
            cmbSize.SelectedIndex = 1
            AddHandler cmbSize.SelectedIndexChanged, Sub()
                Integer.TryParse(cmbSize.SelectedItem?.ToString(), brushSize)
                If brushSize <= 0 Then brushSize = 3
                currentPen = New Pen(If(tool = DrawTool.Eraser, ThemeBase.DarkBg, currentColor), brushSize)
            End Sub
            Dim btnColor As New ToolStripButton("Color")
            AddHandler btnColor.Click, Sub()
                Using cd As New ColorDialog() With {.Color = currentColor}
                    If cd.ShowDialog() = DialogResult.OK Then
                        currentColor = cd.Color
                        If tool = DrawTool.Pen Then currentPen = New Pen(currentColor, brushSize)
                    End If
                End Using
            End Sub
            toolStrip.Items.AddRange(New ToolStripItem() {btnPen, btnEraser, btnLine, btnRect, btnEllipse, sep, lblSize, cmbSize, btnColor})

            pnlCanvas = New Panel() With {
                .Dock = DockStyle.Fill,
                .BackColor = ThemeBase.DarkBg,
                .Cursor = Cursors.Cross
            }
            AddHandler pnlCanvas.Paint, AddressOf Canvas_Paint
            AddHandler pnlCanvas.MouseDown, AddressOf Canvas_MouseDown
            AddHandler pnlCanvas.MouseMove, AddressOf Canvas_MouseMove
            AddHandler pnlCanvas.MouseUp, AddressOf Canvas_MouseUp
            AddHandler pnlCanvas.Resize, Sub()
                If pnlCanvas.Width <= 0 OrElse pnlCanvas.Height <= 0 Then Return
                If canvasBitmap Is Nothing Then
                    InitCanvas()
                    Return
                End If
                Dim bmp As New Bitmap(pnlCanvas.Width, pnlCanvas.Height)
                Using g = Graphics.FromImage(bmp)
                    g.Clear(ThemeBase.DarkBg)
                    g.DrawImage(canvasBitmap, 0, 0)
                End Using
                canvasBitmap.Dispose()
                If canvasGraphics IsNot Nothing Then canvasGraphics.Dispose()
                canvasBitmap = bmp
                canvasGraphics = Graphics.FromImage(canvasBitmap)
                canvasGraphics.SmoothingMode = SmoothingMode.AntiAlias
            End Sub

            Me.Controls.Add(pnlCanvas)
            Me.Controls.Add(toolStrip)
            Me.Controls.Add(menuStrip)
        End Sub

        Private Sub Canvas_Paint(sender As Object, e As PaintEventArgs)
            If canvasBitmap IsNot Nothing Then e.Graphics.DrawImage(canvasBitmap, 0, 0)
        End Sub

        Private Sub Canvas_MouseDown(sender As Object, e As MouseEventArgs)
            isDrawing = True
            startPoint = e.Location
            lastPoint = e.Location
            If canvasBitmap Is Nothing Then InitCanvas()
        End Sub

        Private Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs)
            If Not isDrawing OrElse canvasGraphics Is Nothing Then Return
            Select Case tool
                Case DrawTool.Pen
                    currentPen.Color = currentColor
                    currentPen.Width = brushSize
                    canvasGraphics.DrawLine(currentPen, lastPoint, e.Location)
                    lastPoint = e.Location
                Case DrawTool.Eraser
                    currentPen.Color = ThemeBase.DarkBg
                    currentPen.Width = Math.Max(brushSize * 4, 8)
                    canvasGraphics.DrawLine(currentPen, lastPoint, e.Location)
                    lastPoint = e.Location
                Case DrawTool.Line, DrawTool.Rectangle, DrawTool.Ellipse
                    ' Preview drawn on paint; final drawn on mouse up
            End Select
            pnlCanvas.Invalidate()
        End Sub

        Private Sub Canvas_MouseUp(sender As Object, e As MouseEventArgs)
            If Not isDrawing OrElse canvasGraphics Is Nothing Then
                isDrawing = False
                Return
            End If
            Select Case tool
                Case DrawTool.Line
                    currentPen.Color = currentColor
                    currentPen.Width = brushSize
                    canvasGraphics.DrawLine(currentPen, startPoint, e.Location)
                Case DrawTool.Rectangle
                    currentPen.Color = currentColor
                    currentPen.Width = brushSize
                    Dim r = Rectangle.FromLTRB(Math.Min(startPoint.X, e.X), Math.Min(startPoint.Y, e.Y), Math.Max(startPoint.X, e.X), Math.Max(startPoint.Y, e.Y))
                    canvasGraphics.DrawRectangle(currentPen, r)
                Case DrawTool.Ellipse
                    currentPen.Color = currentColor
                    currentPen.Width = brushSize
                    Dim r = Rectangle.FromLTRB(Math.Min(startPoint.X, e.X), Math.Min(startPoint.Y, e.Y), Math.Max(startPoint.X, e.X), Math.Max(startPoint.Y, e.Y))
                    canvasGraphics.DrawEllipse(currentPen, r)
            End Select
            isDrawing = False
            pnlCanvas.Invalidate()
        End Sub

        Private Sub ClearCanvas(sender As Object, e As EventArgs)
            If canvasGraphics Is Nothing Then Return
            canvasGraphics.Clear(ThemeBase.DarkBg)
            pnlCanvas.Invalidate()
        End Sub

        Private Sub FileNew(sender As Object, e As EventArgs)
            InitCanvas()
        End Sub

        Private Sub FileOpen(sender As Object, e As EventArgs)
            Using ofd As New OpenFileDialog() With {.Filter = "PNG|*.png|Bitmap|*.bmp|JPEG|*.jpg;*.jpeg|All|*.*"}
                If ofd.ShowDialog() <> DialogResult.OK Then Return
                Try
                    Using img = Image.FromFile(ofd.FileName)
                        If canvasBitmap IsNot Nothing Then canvasBitmap.Dispose()
                        canvasBitmap = New Bitmap(img)
                        If canvasGraphics IsNot Nothing Then canvasGraphics.Dispose()
                        canvasGraphics = Graphics.FromImage(canvasBitmap)
                        canvasGraphics.SmoothingMode = SmoothingMode.AntiAlias
                        pnlCanvas.Invalidate()
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Could not open image: " & ex.Message, "Whiteboard", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Sub

        Private Sub FileSaveAs(sender As Object, e As EventArgs)
            Using sfd As New SaveFileDialog() With {.Filter = "PNG|*.png|Bitmap|*.bmp|JPEG|*.jpg|All|*.*"}
                If sfd.ShowDialog() <> DialogResult.OK Then Return
                Try
                    If canvasBitmap Is Nothing Then Return
                    Dim fmt As ImageFormat = ImageFormat.Png
                    If sfd.FileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) Then fmt = ImageFormat.Bmp
                    If sfd.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) Then fmt = ImageFormat.Jpeg
                    canvasBitmap.Save(sfd.FileName, fmt)
                    MessageBox.Show("Saved.", "Whiteboard", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Could not save: " & ex.Message, "Whiteboard", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            If pnlCanvas IsNot Nothing AndAlso pnlCanvas.Width > 0 Then InitCanvas()
        End Sub
    End Class
End Namespace
