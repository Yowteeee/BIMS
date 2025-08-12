Imports System.Windows.Forms
Imports LiveChartsCore
Imports LiveChartsCore.SkiaSharpView
Imports LiveChartsCore.SkiaSharpView.Painting
Imports LiveChartsCore.SkiaSharpView.WinForms
Imports SkiaSharp

Public Class Dashboard
    Private residentFormInstance As ResidentForm
    Private originalPanel2Content As Control() = {}

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Store original content of Guna2Panel2
        StoreOriginalPanel2Content()

        ' Create and configure the cartesian chart
        Dim cartesianChart1 As New LiveChartsCore.SkiaSharpView.WinForms.CartesianChart() With {
            .Dock = DockStyle.Fill
        }

        ' Add chart to Guna2Panel11 instead of the main form
        Guna2Panel11.Controls.Add(cartesianChart1)

        ' Create series
        Dim barSeries As New ColumnSeries(Of Double) With {
            .Name = "Population",
            .Values = {10, 20, 15, 30, 25, 18, 12, 22},
            .DataLabelsPaint = New SolidColorPaint(SKColors.Black),
            .DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top
        }

        ' Add series to chart
        cartesianChart1.Series = {barSeries}

        ' X Axis - Purok Labels
        cartesianChart1.XAxes = {
            New Axis With {
                .Labels = {"Purok 1", "Purok 2", "Purok 3", "Purok 4",
                           "Purok 5", "Purok 6", "Purok 7", "Purok 8"},
                .Name = "Purok"
            }
        }

        ' Y Axis
        cartesianChart1.YAxes = {
            New Axis With {
                .Name = "Residents"
            }
        }

        ' Optional: Show legend
        cartesianChart1.LegendPosition = LiveChartsCore.Measure.LegendPosition.Right
    End Sub

    Private Sub StoreOriginalPanel2Content()
        ' Store all controls from Guna2Panel2
        originalPanel2Content = New Control(Guna2Panel2.Controls.Count - 1) {}
        Guna2Panel2.Controls.CopyTo(originalPanel2Content, 0)
    End Sub

    Private Sub RestoreOriginalPanel2Content()
        ' Clear current content
        Guna2Panel2.Controls.Clear()

        ' Restore original content
        For Each control As Control In originalPanel2Content
            If control IsNot Nothing Then
                Guna2Panel2.Controls.Add(control)
            End If
        Next
    End Sub

    Private Sub Guna2Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel1.Paint
    End Sub

    Private Sub CartesianChart1_ChildChanged(sender As Object, e As Integration.ChildChangedEventArgs) Handles CartesianChart1.ChildChanged
    End Sub

    Private Sub Guna2Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel2.Paint

    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        ' Hide Guna2Panel2 (gradientpanel2) content
        Guna2Panel2.Visible = False

        ' Clear Guna2Panel2 content
        Guna2Panel2.Controls.Clear()

        ' Create a new ResidentForm instance
        residentFormInstance = New ResidentForm()
        residentFormInstance.TopLevel = False
        residentFormInstance.FormBorderStyle = FormBorderStyle.None
        residentFormInstance.Dock = DockStyle.Fill

        ' Add ResidentForm to Guna2Panel2
        Guna2Panel2.Controls.Add(residentFormInstance)
        residentFormInstance.Show()

        ' Make Guna2Panel2 visible again with the new content
        Guna2Panel2.Visible = True
    End Sub

    ' Method to show the dashboard again and hide resident form
    Private Sub ShowDashboard()
        ' Hide resident form
        If residentFormInstance IsNot Nothing Then
            Guna2Panel2.Controls.Remove(residentFormInstance)
            residentFormInstance.Dispose()
            residentFormInstance = Nothing
        End If

        ' Restore original content
        RestoreOriginalPanel2Content()
    End Sub

    ' Add button click handlers for other navigation buttons
    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        ' Dashboard button - show original dashboard content
        ShowDashboard()
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        ' Reports button - you can implement reports functionality here
        MessageBox.Show("Reports functionality will be implemented here.", "Reports", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        ' Backup and Restore button
        MessageBox.Show("Backup and Restore functionality will be implemented here.", "Backup & Restore", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click
        ' Cedula Tracker button
        MessageBox.Show("Cedula Tracker functionality will be implemented here.", "Cedula Tracker", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Guna2Button6_Click(sender As Object, e As EventArgs) Handles Guna2Button6.Click
        ' Staffs button
        MessageBox.Show("Staffs functionality will be implemented here.", "Staffs", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Guna2Button7_Click(sender As Object, e As EventArgs) Handles Guna2Button7.Click
        ' Log Out button
        Dim result = MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Me.Close()
            Form1.Show()
        End If
    End Sub
End Class