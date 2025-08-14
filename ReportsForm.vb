Public Class ReportsForm
    ' Add printing-related variables
    Private printDocument As New Printing.PrintDocument()
    Private printPreviewDialog As New PrintPreviewDialog()
    Private printDialog As New PrintDialog()

    Private Sub Guna2Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel1.Paint

    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub ReportsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Configure all textboxes for multi-line text with top-left alignment
        ConfigureTextBoxes()
        ' Configure scrollbar functionality
        ConfigureScrollbar()
        ' Configure printing
        ConfigurePrinting()
    End Sub

    Private Sub ConfigureTextBoxes()
        ' Configure Guna2TextBox1
        Guna2TextBox1.Multiline = True
        Guna2TextBox1.TextAlign = HorizontalAlignment.Left
        Guna2TextBox1.AcceptsReturn = True
        Guna2TextBox1.AcceptsTab = True
        Guna2TextBox1.ScrollBars = ScrollBars.Vertical
        Guna2TextBox1.WordWrap = True

        ' Configure Guna2TextBox2
        Guna2TextBox2.Multiline = True
        Guna2TextBox2.TextAlign = HorizontalAlignment.Left
        Guna2TextBox2.AcceptsReturn = True
        Guna2TextBox2.AcceptsTab = True
        Guna2TextBox2.ScrollBars = ScrollBars.Vertical
        Guna2TextBox2.WordWrap = True

        ' Configure Guna2TextBox3
        Guna2TextBox3.Multiline = True
        Guna2TextBox3.TextAlign = HorizontalAlignment.Left
        Guna2TextBox3.AcceptsReturn = True
        Guna2TextBox3.AcceptsTab = True
        Guna2TextBox3.ScrollBars = ScrollBars.Vertical
        Guna2TextBox3.WordWrap = True

        ' Configure Guna2TextBox4
        Guna2TextBox4.Multiline = True
        Guna2TextBox4.TextAlign = HorizontalAlignment.Left
        Guna2TextBox4.AcceptsReturn = True
        Guna2TextBox4.AcceptsTab = True
        Guna2TextBox4.ScrollBars = ScrollBars.Vertical
        Guna2TextBox4.WordWrap = True

        ' Configure Guna2TextBox5
        Guna2TextBox5.Multiline = True
        Guna2TextBox5.TextAlign = HorizontalAlignment.Left
        Guna2TextBox5.AcceptsReturn = True
        Guna2TextBox5.AcceptsTab = True
        Guna2TextBox5.ScrollBars = ScrollBars.Vertical
        Guna2TextBox5.WordWrap = True
    End Sub

    Private Sub ConfigureScrollbar()
        ' Configure the vertical scrollbar
        Guna2VScrollBar1.Minimum = 0
        Guna2VScrollBar1.Maximum = 100
        Guna2VScrollBar1.Value = 0
        Guna2VScrollBar1.SmallChange = 10
        Guna2VScrollBar1.LargeChange = 20

        ' Enable the scrollbar
        Guna2VScrollBar1.Enabled = True
        Guna2VScrollBar1.Visible = True
    End Sub

    Private Sub ConfigurePrinting()
        ' Configure print document
        printDocument.DocumentName = "Reports Form"
        AddHandler printDocument.PrintPage, AddressOf PrintDocument_PrintPage

        ' Configure print dialog for actual printing
        printDialog.Document = printDocument
        printDialog.AllowSomePages = True
        printDialog.AllowSelection = False
        printDialog.AllowPrintToFile = False ' Disable print to file to force actual printing
        printDialog.ShowHelp = True
        printDialog.UseEXDialog = True ' Use extended dialog for more printer options

        ' Configure print preview dialog
        printPreviewDialog.Document = printDocument
        printPreviewDialog.WindowState = FormWindowState.Maximized

        ' Try to get available printers
        Try
            Dim printers As String() = Printing.PrinterSettings.InstalledPrinters.Cast(Of String)().ToArray()
            If printers.Length > 0 Then
                ' Set default printer if available
                printDocument.PrinterSettings.PrinterName = printers(0)
            End If
        Catch ex As Exception
            ' Continue without setting default printer
        End Try
    End Sub

    Private Sub Guna2VScrollBar1_Scroll(sender As Object, e As ScrollEventArgs) Handles Guna2VScrollBar1.Scroll
        ' Handle scrollbar scrolling
        Dim scrollValue As Integer = Guna2VScrollBar1.Value

        ' Calculate the scroll offset based on the panel height and content
        Dim maxScroll As Integer = Math.Max(0, Guna2Panel1.Height - Me.ClientSize.Height)
        Dim scrollOffset As Integer = (scrollValue / 100.0) * maxScroll

        ' Apply the scroll offset to the panel
        Guna2Panel1.Top = -scrollOffset
    End Sub

    Private Sub Guna2ContainerControl1_Click(sender As Object, e As EventArgs)

    End Sub

    ' Add mouse wheel support for scrolling
    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
        MyBase.OnMouseWheel(e)

        ' Handle mouse wheel scrolling
        If e.Delta > 0 Then
            ' Scroll up
            If Guna2VScrollBar1.Value > Guna2VScrollBar1.Minimum Then
                Guna2VScrollBar1.Value = Math.Max(Guna2VScrollBar1.Minimum, Guna2VScrollBar1.Value - Guna2VScrollBar1.SmallChange)
            End If
        Else
            ' Scroll down
            If Guna2VScrollBar1.Value < Guna2VScrollBar1.Maximum Then
                Guna2VScrollBar1.Value = Math.Min(Guna2VScrollBar1.Maximum, Guna2VScrollBar1.Value + Guna2VScrollBar1.SmallChange)
            End If
        End If
    End Sub

    ' Add form resize handling to update scrollbar
    Private Sub ReportsForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ' Update scrollbar when form is resized
        UpdateScrollbarVisibility()
    End Sub

    Private Sub UpdateScrollbarVisibility()
        ' Show/hide scrollbar based on content height
        If Guna2Panel1.Height > Me.ClientSize.Height Then
            Guna2VScrollBar1.Visible = True
            Guna2VScrollBar1.Enabled = True
        Else
            Guna2VScrollBar1.Visible = False
            Guna2VScrollBar1.Enabled = False
            Guna2Panel1.Top = 0 ' Reset panel position
        End If
    End Sub

    ' Print button functionality - Now connects directly to computer print settings
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Try
            ' Configure the print dialog for direct printing
            printDialog.Document = printDocument
            printDialog.AllowSomePages = True
            printDialog.AllowSelection = False
            printDialog.AllowPrintToFile = False
            printDialog.ShowHelp = True
            printDialog.UseEXDialog = True

            ' Show the standard Windows print dialog
            If printDialog.ShowDialog() = DialogResult.OK Then
                ' User clicked OK in the print dialog, now actually print
                Try
                    printDocument.Print()
                    MessageBox.Show("Print job sent successfully!", "Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Error during printing: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        Catch ex As Exception
            MessageBox.Show("Error setting up print dialog: " & ex.Message, "Print Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Print Preview button functionality - Now on Guna2Button3
    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Try
            ' Check if there's any data to preview
            Dim hasData As Boolean = False
            If Not String.IsNullOrEmpty(Guna2TextBox1.Text.Trim()) OrElse
               Not String.IsNullOrEmpty(Guna2TextBox2.Text.Trim()) OrElse
               Not String.IsNullOrEmpty(Guna2TextBox3.Text.Trim()) OrElse
               Not String.IsNullOrEmpty(Guna2TextBox4.Text.Trim()) OrElse
               Not String.IsNullOrEmpty(Guna2TextBox5.Text.Trim()) Then
                hasData = True
            End If

            If Not hasData Then
                MessageBox.Show("No data to preview. Please enter some text in the textboxes first.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            ' Configure print preview dialog
            printPreviewDialog.Document = printDocument
            printPreviewDialog.WindowState = FormWindowState.Maximized
            printPreviewDialog.Text = "Print Preview - Review Your Data Layout Before Printing"

            ' Show the print preview dialog
            Dim previewResult As DialogResult = printPreviewDialog.ShowDialog()

            ' If user clicked Print button in the preview dialog, proceed to print
            If previewResult = DialogResult.OK Then
                ' Configure the print dialog properly
                printDialog.Document = printDocument
                printDialog.AllowSomePages = True
                printDialog.AllowSelection = False
                printDialog.AllowPrintToFile = False
                printDialog.ShowHelp = True
                printDialog.UseEXDialog = True

                ' Show the configured print dialog
                If printDialog.ShowDialog() = DialogResult.OK Then
                    ' User clicked OK in the print dialog, now actually print
                    Try
                        printDocument.Print()
                        MessageBox.Show("Print job sent successfully!", "Print Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MessageBox.Show("Error during printing: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End If

        Catch ex As Exception
            MessageBox.Show("Error setting up print preview: " & ex.Message, "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    ' Clear/Refresh button functionality
    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        ' Clear all textboxes
        Guna2TextBox1.Text = ""
        Guna2TextBox2.Text = ""
        Guna2TextBox3.Text = ""
        Guna2TextBox4.Text = ""
        Guna2TextBox5.Text = ""

        ' Reset scrollbar position
        Guna2VScrollBar1.Value = 0
        Guna2Panel1.Top = 0

        MessageBox.Show("All textboxes have been cleared!", "Clear Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' Text changed event for textbox2 (can be used for other textboxes if needed)
    Private Sub Guna2TextBox2_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox2.TextChanged

    End Sub

    ' PictureBox click event
    Private Sub Guna2PictureBox1_Click_1(sender As Object, e As EventArgs) Handles Guna2PictureBox1.Click

    End Sub

    ' Print document event handler
    Private Sub PrintDocument_PrintPage(sender As Object, e As Printing.PrintPageEventArgs)
        Try
            Dim graphics As Graphics = e.Graphics
            Dim pageBounds As Rectangle = e.PageBounds
            Dim margin As Integer = 40

            ' Print the PictureBox content directly to preserve the exact layout
            If Guna2PictureBox1.Image IsNot Nothing Then
                ' Calculate image size to fill the page with minimal margins
                Dim imageWidth As Integer = pageBounds.Width - (margin * 2)
                Dim imageHeight As Integer = pageBounds.Height - (margin * 2)

                ' Maintain aspect ratio while maximizing size
                Dim aspectRatio As Double = CDbl(Guna2PictureBox1.Image.Width) / CDbl(Guna2PictureBox1.Image.Height)
                Dim pageAspectRatio As Double = CDbl(imageWidth) / CDbl(imageHeight)

                If aspectRatio > pageAspectRatio Then
                    ' Image is wider than page, fit to width
                    imageHeight = CInt(imageWidth / aspectRatio)
                Else
                    ' Image is taller than page, fit to height
                    imageWidth = CInt(imageHeight * aspectRatio)
                End If

                ' Center the image horizontally
                Dim imageX As Integer = margin + (pageBounds.Width - (margin * 2) - imageWidth) \ 2
                Dim imageY As Integer = margin
                Dim imageRect As New Rectangle(imageX, imageY, imageWidth, imageHeight)

                ' Draw the PictureBox image to fill the page
                graphics.DrawImage(Guna2PictureBox1.Image, imageRect)

                ' Now overlay the textbox content on top of the form layout
                Dim textBoxes() As Guna.UI2.WinForms.Guna2TextBox = {Guna2TextBox1, Guna2TextBox2, Guna2TextBox3, Guna2TextBox4, Guna2TextBox5}

                ' Calculate scaling factors to map textbox positions from form to print
                Dim scaleX As Double = CDbl(imageWidth) / CDbl(Guna2PictureBox1.Width)
                Dim scaleY As Double = CDbl(imageHeight) / CDbl(Guna2PictureBox1.Height)

                ' Overlay textbox content at the correct positions
                For i As Integer = 0 To textBoxes.Length - 1
                    If Not String.IsNullOrEmpty(textBoxes(i).Text.Trim()) Then
                        ' Get the textbox position relative to the PictureBox
                        Dim textBoxX As Integer = textBoxes(i).Location.X - Guna2PictureBox1.Location.X
                        Dim textBoxY As Integer = textBoxes(i).Location.Y - Guna2PictureBox1.Location.Y

                        ' Scale the position to match the printed image
                        Dim printX As Integer = imageX + CInt(textBoxX * scaleX)
                        Dim printY As Integer = imageY + CInt(textBoxY * scaleY)

                        ' Scale the textbox size
                        Dim printWidth As Integer = CInt(textBoxes(i).Width * scaleX)
                        Dim printHeight As Integer = CInt(textBoxes(i).Height * scaleY)

                        ' Create a font that scales with the image
                        Dim scaledFont As New Font("Arial", CSng(10 * Math.Min(scaleX, scaleY)), FontStyle.Regular)

                        ' Print the textbox content at the correct position
                        Dim contentText As String = textBoxes(i).Text.Trim()
                        Dim textRect As New Rectangle(printX + 5, printY + 5, printWidth - 10, printHeight - 10)

                        ' Use word wrapping for the text
                        graphics.DrawString(contentText, scaledFont, Brushes.Black, textRect)
                    End If
                Next
            End If

        Catch ex As Exception
            MessageBox.Show("Error during printing: " & ex.Message, "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Helper method to print formatted text (handles bold formatting)
    Private Sub PrintFormattedText(graphics As Graphics, text As String, normalFont As Font, boldFont As Font, x As Integer, y As Integer, maxWidth As Integer)
        Dim currentX As Integer = x
        Dim currentY As Integer = y
        Dim lineHeight As Integer = CInt(graphics.MeasureString("A", normalFont).Height)
        Dim words() As String = text.Split(" "c)

        For Each word As String In words
            Dim currentFont As Font = normalFont
            Dim displayWord As String = word

            ' Check if word contains bold markers (**text**)
            If word.StartsWith("**") AndAlso word.EndsWith("**") Then
                currentFont = boldFont
                displayWord = word.Substring(2, word.Length - 4)
            End If

            Dim wordSize As SizeF = graphics.MeasureString(displayWord & " ", currentFont)

            ' Check if word fits on current line
            If currentX + wordSize.Width > x + maxWidth Then
                currentX = x
                currentY += lineHeight + 2
            End If

            ' Draw the word
            graphics.DrawString(displayWord & " ", currentFont, Brushes.Black, currentX, currentY)
            currentX += CInt(wordSize.Width)
        Next
    End Sub

    ' Helper method to calculate text height
    Private Function GetTextHeight(text As String, font As Font, maxWidth As Integer) As Integer
        Using g As Graphics = CreateGraphics()
            Dim size As SizeF = g.MeasureString(text, font, maxWidth)
            Return CInt(size.Height)
        End Using
    End Function

    ' Add keyboard shortcuts for text formatting
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        ' Check for Ctrl+B (bold)
        If keyData = (Keys.Control Or Keys.B) Then
            ApplyBoldToSelectedText()
            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    ' Method to apply bold formatting to selected text
    Private Sub ApplyBoldToSelectedText()
        Dim activeTextBox As Guna.UI2.WinForms.Guna2TextBox = Nothing

        ' Determine which textbox has focus
        If Guna2TextBox1.Focused Then
            activeTextBox = Guna2TextBox1
        ElseIf Guna2TextBox2.Focused Then
            activeTextBox = Guna2TextBox2
        ElseIf Guna2TextBox3.Focused Then
            activeTextBox = Guna2TextBox3
        ElseIf Guna2TextBox4.Focused Then
            activeTextBox = Guna2TextBox4
        ElseIf Guna2TextBox5.Focused Then
            activeTextBox = Guna2TextBox5
        End If

        If activeTextBox IsNot Nothing AndAlso activeTextBox.SelectionLength > 0 Then
            Dim selectedText As String = activeTextBox.SelectedText
            Dim boldText As String = "**" & selectedText & "**"

            ' Replace selected text with bold format
            Dim startPos As Integer = activeTextBox.SelectionStart
            activeTextBox.Text = activeTextBox.Text.Remove(startPos, activeTextBox.SelectionLength)
            activeTextBox.Text = activeTextBox.Text.Insert(startPos, boldText)

            ' Restore selection
            activeTextBox.SelectionStart = startPos
            activeTextBox.SelectionLength = boldText.Length
        End If
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged

    End Sub
End Class