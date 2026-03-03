Imports System.Drawing
Imports System.Windows.Forms

Namespace Presentation
    Public Class CalculatorForm
        Inherits Form

        Private numFee As NumericUpDown
        Private chkScholarship As CheckBox
        Private cmbInstallments As ComboBox
        Private lblResult As Label

        Public Sub New()
            InitializeComponent()
            ThemeBase.ApplyTheme(Me)
        End Sub

        Private Sub InitializeComponent()
            Dim lblTitle As New Label() With {.Text = "Course Fee & EMI Calculator", .Font = New Font("Segoe UI", 16), .Location = New Point(20, 20), .AutoSize = True}
            
            Dim lblFee As New Label() With {.Text = "Base Fee:", .Location = New Point(20, 70), .AutoSize = True}
            numFee = New NumericUpDown() With {.Location = New Point(150, 68), .Maximum = 1000000, .Value = 50000, .Width = 200}

            chkScholarship = New CheckBox() With {.Text = "Apply 10% Scholarship", .Location = New Point(150, 110), .AutoSize = True}

            Dim lblInst As New Label() With {.Text = "Installments:", .Location = New Point(20, 150), .AutoSize = True}
            cmbInstallments = New ComboBox() With {.Location = New Point(150, 148), .DropDownStyle = ComboBoxStyle.DropDownList, .Width = 200}
            cmbInstallments.Items.AddRange({"1 (Full)", "3 (Quarterly)", "6 (Half-Yearly)", "12 (Monthly)"})
            cmbInstallments.SelectedIndex = 0

            Dim btnCalc As New Button() With {.Text = "Calculate", .Location = New Point(150, 200), .Width = 200, .Height = 35}
            AddHandler btnCalc.Click, AddressOf BtnCalc_Click

            lblResult = New Label() With {.Text = "Result:", .Location = New Point(20, 250), .AutoSize = True, .Font = New Font("Segoe UI", 12, FontStyle.Bold)}

            Me.Controls.AddRange(New Control() {lblTitle, lblFee, numFee, chkScholarship, lblInst, cmbInstallments, btnCalc, lblResult})
        End Sub

        Private Sub BtnCalc_Click(sender As Object, e As EventArgs)
            Dim baseFee As Decimal = numFee.Value
            If chkScholarship.Checked Then baseFee *= 0.9D
            
            ' GST 18%
            Dim feeWithGST = baseFee * 1.18D
            
            Dim instMonths = 1
            If cmbInstallments.SelectedIndex = 1 Then instMonths = 3
            If cmbInstallments.SelectedIndex = 2 Then instMonths = 6
            If cmbInstallments.SelectedIndex = 3 Then instMonths = 12

            Dim emi = feeWithGST / instMonths
            lblResult.Text = $"Total Fee (Inc. GST): {feeWithGST:C} | EMI: {emi:C} / month"
        End Sub
    End Class
End Namespace
