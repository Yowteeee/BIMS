Imports System.Windows.Forms
Imports LiveChartsCore
Imports LiveChartsCore.SkiaSharpView
Imports LiveChartsCore.SkiaSharpView.Painting
Imports LiveChartsCore.SkiaSharpView.WinForms
Imports SkiaSharp

Public Class dashboard

    Private Sub dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Create and configure the cartesian chart
        Dim cartesianChart1 As New LiveChartsCore.SkiaSharpView.WinForms.CartesianChart()
        cartesianChart1.Dock = DockStyle.Fill
        Me.Controls.Add(cartesianChart1)

        ' Create series
        Dim barSeries As New ColumnSeries(Of Double) With {
            .Name = "Population",
            .Values = New Double() {10, 20, 15, 30, 25, 18, 12, 22},
            .DataLabelsPaint = New SolidColorPaint(SKColors.Black),
            .DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top
        }

        ' Add series to chart
        cartesianChart1.Series = New ISeries() {barSeries}

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

    Private Sub Guna2Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel1.Paint

    End Sub

    Private Sub CartesianChart1_ChildChanged(sender As Object, e As Integration.ChildChangedEventArgs) Handles CartesianChart1.ChildChanged

    End Sub
End Class
