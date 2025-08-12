Imports MySql.Data.MySqlClient
Imports System.IO

Public Class ResidentForm
    ' Database connection string
    Private connectionString As String = "Server=127.0.0.1;Port=3306;Database=cap_101;Uid=root;Pwd=johnarbenanguring;SslMode=none;"

    ' Current resident data for update operations
    Private currentResidentData As Dictionary(Of String, String) = New Dictionary(Of String, String)

    ' Store the selected image path
    Private selectedImagePath As String = ""

    Private Sub ResidentForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load data into comboboxes
        LoadComboBoxData()

        ' Set default date to today
        Guna2DateTimePicker1.Value = DateTime.Now

        ' Generate and set the next available ID
        GenerateNextID()

        ' Make the ID textbox read-only
        Guna2TextBox2.ReadOnly = True
    End Sub

    Private Sub LoadComboBoxData()
        ' Load Sitio/Purok data
        Guna2ComboBox4.Items.Clear()
        Guna2ComboBox4.Items.AddRange({"Purok 1", "Purok 2", "Purok 3", "Purok 4", "Purok 5", "Purok 6", "Purok 7", "Purok 8"})

        ' Load Blood Type data
        Guna2ComboBox1.Items.Clear()
        Guna2ComboBox1.Items.AddRange({"A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"})

        ' Load Gender data
        Guna2ComboBox3.Items.Clear()
        Guna2ComboBox3.Items.AddRange({"Male", "Female"})

        ' Load Suffix data
        Guna2ComboBox2.Items.Clear()
        Guna2ComboBox2.Items.AddRange({"Jr.", "Sr.", "I", "II", "III", "IV", "V"})
    End Sub

    Private Sub GenerateNextID()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                ' Get the maximum ID from the database
                Dim maxIdQuery As String = "SELECT MAX(CAST(SUBSTRING(id, 1) AS UNSIGNED)) FROM tbl_resident"

                Using command As New MySqlCommand(maxIdQuery, connection)
                    Dim result As Object = command.ExecuteScalar()

                    Dim nextId As Integer = 1
                    If result IsNot Nothing AndAlso Not Convert.IsDBNull(result) Then
                        nextId = Convert.ToInt32(result) + 1
                    End If

                    ' Format the ID with leading zeros (4 digits)
                    Guna2TextBox2.Text = nextId.ToString("D4")
                End Using
            End Using

        Catch ex As Exception
            ' If there's an error, start with 0001
            Guna2TextBox2.Text = "0001"
        End Try
    End Sub

    Private Function GetFullName() As String
        ' Combine first, middle, and last name into full name
        Dim firstName As String = Guna2TextBox3.Text.Trim()
        Dim middleName As String = Guna2TextBox4.Text.Trim()
        Dim lastName As String = Guna2TextBox5.Text.Trim()

        Dim fullName As String = ""

        If Not String.IsNullOrWhiteSpace(firstName) Then
            fullName = firstName
        End If

        If Not String.IsNullOrWhiteSpace(middleName) Then
            If fullName.Length > 0 Then
                fullName &= " " & middleName
            Else
                fullName = middleName
            End If
        End If

        If Not String.IsNullOrWhiteSpace(lastName) Then
            If fullName.Length > 0 Then
                fullName &= " " & lastName
            Else
                fullName = lastName
            End If
        End If

        Return fullName
    End Function

    Private Sub ParseFullName(fullName As String)
        ' Split full name back into individual components
        Dim nameParts() As String = fullName.Split(" "c)

        If nameParts.Length >= 1 Then
            Guna2TextBox3.Text = nameParts(0) ' First name
        End If

        If nameParts.Length >= 2 Then
            Guna2TextBox4.Text = nameParts(1) ' Middle name
        End If

        If nameParts.Length >= 3 Then
            Guna2TextBox5.Text = nameParts(2) ' Last name
        End If
    End Sub

    Private Function ValidateInputs() As Boolean
        ' Check if all required fields are filled
        If String.IsNullOrWhiteSpace(Guna2TextBox3.Text) Then
            MessageBox.Show("Please enter First Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2TextBox3.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(Guna2TextBox4.Text) Then
            MessageBox.Show("Please enter Middle Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2TextBox4.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(Guna2TextBox5.Text) Then
            MessageBox.Show("Please enter Last Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2TextBox5.Focus()
            Return False
        End If

        If Guna2ComboBox4.SelectedIndex = -1 Then
            MessageBox.Show("Please select Sitio/Purok.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2ComboBox4.Focus()
            Return False
        End If

        If Guna2ComboBox1.SelectedIndex = -1 Then
            MessageBox.Show("Please select Blood Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2ComboBox1.Focus()
            Return False
        End If

        If Guna2ComboBox3.SelectedIndex = -1 Then
            MessageBox.Show("Please select Gender.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2ComboBox3.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(Guna2TextBox6.Text) Then
            MessageBox.Show("Please enter Birth Place.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2TextBox6.Focus()
            Return False
        End If

        If String.IsNullOrWhiteSpace(Guna2TextBox1.Text) Then
            MessageBox.Show("Please enter Contact Number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2TextBox1.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        ' SAVE button
        If Not ValidateInputs() Then
            Return
        End If

        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                ' Get the full name
                Dim fullName As String = GetFullName()

                ' Check if resident already exists (using full name)
                Dim checkQuery As String = "SELECT COUNT(*) FROM tbl_resident WHERE fullname = @fullname"

                Using checkCommand As New MySqlCommand(checkQuery, connection)
                    checkCommand.Parameters.AddWithValue("@fullname", fullName)

                    Dim existingCount As Integer = Convert.ToInt32(checkCommand.ExecuteScalar())

                    If existingCount > 0 Then
                        MessageBox.Show("A resident with this name already exists. Please use UPDATE instead.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using

                ' Insert new resident
                Dim insertQuery As String = "INSERT INTO tbl_resident (id, fullname, `sitio/purok`, contactnumber, bloodtype, suffix, gender, birthdate, birthplace, photo) VALUES (@id, @fullname, @sitio, @contactnumber, @bloodtype, @suffix, @gender, @birthdate, @birthplace, @photo)"

                Using command As New MySqlCommand(insertQuery, connection)
                    command.Parameters.AddWithValue("@id", Guna2TextBox2.Text.Trim())
                    command.Parameters.AddWithValue("@fullname", fullName)
                    command.Parameters.AddWithValue("@sitio", Guna2ComboBox4.Text)
                    command.Parameters.AddWithValue("@contactnumber", Guna2TextBox1.Text.Trim())
                    command.Parameters.AddWithValue("@bloodtype", Guna2ComboBox1.Text)
                    command.Parameters.AddWithValue("@suffix", If(Guna2ComboBox2.SelectedIndex >= 0, Guna2ComboBox2.Text, ""))
                    command.Parameters.AddWithValue("@gender", Guna2ComboBox3.Text)
                    command.Parameters.AddWithValue("@birthdate", Guna2DateTimePicker1.Value)
                    command.Parameters.AddWithValue("@birthplace", Guna2TextBox6.Text.Trim())

                    ' Add photo parameter
                    If Not String.IsNullOrEmpty(selectedImagePath) AndAlso File.Exists(selectedImagePath) Then
                        Dim imageBytes As Byte() = File.ReadAllBytes(selectedImagePath)
                        command.Parameters.AddWithValue("@photo", imageBytes)
                    Else
                        command.Parameters.AddWithValue("@photo", DBNull.Value)
                    End If

                    command.ExecuteNonQuery()
                End Using

                MessageBox.Show("Resident saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                ClearForm()
                ' Generate next ID for the next entry
                GenerateNextID()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error saving resident: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton4.Click
        ' CANCEL button - Hide ResidentForm and show Dashboard
        Try
            ' Hide the ResidentForm panel (Guna2GradientPanel1)
            Me.Guna2GradientPanel1.Visible = False

            ' Get the parent form (Dashboard)
            Dim dashboardForm As Dashboard = TryCast(Me.ParentForm, Dashboard)
            If dashboardForm IsNot Nothing Then
                ' Show the Dashboard panel (Guna2Panel2)
                dashboardForm.Guna2Panel2.Visible = True

                ' Clear the form before hiding
                ClearForm()
                currentResidentData.Clear()

                ' Generate next ID for future use
                GenerateNextID()
            End If

        Catch ex As Exception
            MessageBox.Show("Error returning to dashboard: " & ex.Message, "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ClearForm()
        ' Clear all form fields (except ID which will be regenerated)
        Guna2TextBox3.Clear()
        Guna2TextBox4.Clear()
        Guna2TextBox5.Clear()
        Guna2TextBox1.Clear()
        Guna2ComboBox4.SelectedIndex = -1
        Guna2ComboBox1.SelectedIndex = -1
        Guna2ComboBox2.SelectedIndex = -1
        Guna2ComboBox3.SelectedIndex = -1
        Guna2DateTimePicker1.Value = DateTime.Now
        Guna2TextBox6.Clear()

        ' Clear picture box if needed
        If Guna2PictureBox1.Image IsNot Nothing Then
            Guna2PictureBox1.Image = Nothing
        End If

        ' Clear selected image path
        selectedImagePath = ""

        ' Note: ID textbox is not cleared here as it will be regenerated when needed
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged
    End Sub

    Private Sub Guna2GradientPanel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2GradientPanel1.Paint
    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox1.Click
        ' Open file dialog to select an image
        Dim openFileDialog As New OpenFileDialog()

        ' Set filter for image files
        openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp; *.gif)|*.jpg; *.jpeg; *.png; *.bmp; *.gif|All Files (*.*)|*.*"
        openFileDialog.Title = "Select Resident Photo"

        ' Set initial directory to Pictures folder
        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)

        ' Show dialog and check if user selected a file
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                ' Load the selected image
                Dim selectedImage As Image = Image.FromFile(openFileDialog.FileName)

                ' Set the image to the picturebox
                Guna2PictureBox1.Image = selectedImage

                ' Set the SizeMode to StretchImage to fill all gaps in the picturebox
                Guna2PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage

                ' Store the file path for saving to database
                selectedImagePath = openFileDialog.FileName

            Catch ex As Exception
                MessageBox.Show("Error loading image: " & ex.Message, "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub Guna2TextBox2_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox2.TextChanged

    End Sub
End Class