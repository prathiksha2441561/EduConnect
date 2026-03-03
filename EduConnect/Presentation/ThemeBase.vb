Imports System.Drawing
Imports System.Windows.Forms

Namespace Presentation
    Public Module ThemeBase
        ' Modern dark theme – deep blues and teals
        Public DarkBg As Color = Color.FromArgb(22, 27, 34)
        Public DarkPanel As Color = Color.FromArgb(33, 38, 45)
        Public CardBg As Color = Color.FromArgb(40, 46, 54)
        Public Accent As Color = Color.FromArgb(88, 166, 255)
        Public AccentSoft As Color = Color.FromArgb(56, 139, 253)
        Public TextColor As Color = Color.FromArgb(230, 237, 243)
        Public TextMuted As Color = Color.FromArgb(139, 148, 158)
        Public ButtonHover As Color = Color.FromArgb(56, 139, 253)
        Public InputBg As Color = Color.FromArgb(33, 38, 45)
        Public BorderColor As Color = Color.FromArgb(48, 54, 61)
        Public LogoutRed As Color = Color.FromArgb(207, 34, 46)
        Public SuccessGreen As Color = Color.FromArgb(46, 160, 67)

        Public Const SidebarWidth As Integer = 240

        Public Sub ApplyTheme(frm As Form)
            frm.BackColor = DarkBg
            frm.ForeColor = TextColor
            frm.Font = New Font("Segoe UI", 10)
            For Each c As Control In frm.Controls
                StyleControl(c)
            Next
        End Sub

        Public Sub StyleControl(c As Control)
            If TypeOf c Is Panel Then
                Dim pnl As Panel = DirectCast(c, Panel)
                If pnl.BackColor <> LogoutRed Then pnl.BackColor = DarkPanel
                pnl.ForeColor = TextColor
            ElseIf TypeOf c Is Button Then
                Dim btn As Button = DirectCast(c, Button)
                If btn.BackColor <> LogoutRed Then btn.BackColor = Accent
                btn.FlatStyle = FlatStyle.Flat
                btn.FlatAppearance.BorderSize = 0
                btn.FlatAppearance.MouseOverBackColor = ButtonHover
                btn.ForeColor = Color.White
                btn.Cursor = Cursors.Hand
                btn.Font = New Font("Segoe UI Semibold", 10)
            ElseIf TypeOf c Is TextBox Then
                Dim txt As TextBox = DirectCast(c, TextBox)
                txt.BackColor = InputBg
                txt.ForeColor = TextColor
                txt.BorderStyle = BorderStyle.FixedSingle
                txt.Font = New Font("Segoe UI", 11)
            ElseIf TypeOf c Is ComboBox Then
                Dim cmb As ComboBox = DirectCast(c, ComboBox)
                cmb.BackColor = InputBg
                cmb.ForeColor = TextColor
                cmb.FlatStyle = FlatStyle.Flat
            ElseIf TypeOf c Is Label Then
                Dim lbl As Label = DirectCast(c, Label)
                If lbl.ForeColor.ToArgb() <> Accent.ToArgb() Then lbl.ForeColor = TextColor
            ElseIf TypeOf c Is NumericUpDown Then
                Dim num As NumericUpDown = DirectCast(c, NumericUpDown)
                num.BackColor = InputBg
                num.ForeColor = TextColor
                num.BorderStyle = BorderStyle.FixedSingle
            ElseIf TypeOf c Is DataGridView Then
                StyleDataGridView(DirectCast(c, DataGridView))
            ElseIf TypeOf c Is ToolStrip Then
                Dim ts As ToolStrip = DirectCast(c, ToolStrip)
                ts.BackColor = DarkPanel
                ts.GripMargin = New Padding(0)
                ts.Renderer = New ThemeToolStripRenderer()
            ElseIf TypeOf c Is MenuStrip Then
                Dim ms As MenuStrip = DirectCast(c, MenuStrip)
                ms.BackColor = DarkPanel
                ms.ForeColor = TextColor
                ms.Renderer = New ThemeMenuStripRenderer()
            ElseIf TypeOf c Is RichTextBox Then
                Dim rtb As RichTextBox = DirectCast(c, RichTextBox)
                rtb.BackColor = InputBg
                rtb.ForeColor = TextColor
                rtb.BorderStyle = BorderStyle.FixedSingle
            ElseIf TypeOf c Is ListBox Then
                Dim lb As ListBox = DirectCast(c, ListBox)
                lb.BackColor = InputBg
                lb.ForeColor = TextColor
                lb.BorderStyle = BorderStyle.FixedSingle
            ElseIf TypeOf c Is CheckBox Then
                Dim chk As CheckBox = DirectCast(c, CheckBox)
                chk.ForeColor = TextColor
            End If
            For Each child As Control In c.Controls
                StyleControl(child)
            Next
        End Sub

        Private Sub StyleDataGridView(dgv As DataGridView)
            dgv.BackgroundColor = DarkBg
            dgv.BorderStyle = BorderStyle.None
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            dgv.DefaultCellStyle.BackColor = DarkPanel
            dgv.DefaultCellStyle.ForeColor = TextColor
            dgv.DefaultCellStyle.SelectionBackColor = Accent
            dgv.DefaultCellStyle.SelectionForeColor = Color.White
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 33, 40)
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextColor
            dgv.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            dgv.RowHeadersVisible = False
            dgv.EnableHeadersVisualStyles = False
            dgv.GridColor = BorderColor
        End Sub

        Private Class ThemeToolStripRenderer
            Inherits ToolStripProfessionalRenderer
            Public Sub New()
                MyBase.New(New ThemeColors())
            End Sub
        End Class

        Private Class ThemeMenuStripRenderer
            Inherits ToolStripProfessionalRenderer
            Public Sub New()
                MyBase.New(New ThemeColors())
            End Sub
        End Class

        Private Class ThemeColors
            Inherits ProfessionalColorTable
            Public Overrides ReadOnly Property MenuBorder As Color
                Get
                    Return BorderColor
                End Get
            End Property
            Public Overrides ReadOnly Property MenuItemSelected As Color
                Get
                    Return Accent
                End Get
            End Property
            Public Overrides ReadOnly Property MenuItemPressedGradientBegin As Color
                Get
                    Return DarkPanel
                End Get
            End Property
            Public Overrides ReadOnly Property MenuItemPressedGradientEnd As Color
                Get
                    Return DarkPanel
                End Get
            End Property
            Public Overrides ReadOnly Property ToolStripGradientBegin As Color
                Get
                    Return DarkPanel
                End Get
            End Property
            Public Overrides ReadOnly Property ToolStripGradientEnd As Color
                Get
                    Return DarkPanel
                End Get
            End Property
        End Class
    End Module
End Namespace
