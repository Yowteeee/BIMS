Imports MySql.Data.MySqlClient
Imports System.IO

Public Class ResidentForm
    ' Database connection string
    Private connectionString As String = "Server=127.0.0.1;Port=3306;Database=cap_101;Uid=root;Pwd=johnarbenanguring;SslMode=none;"

    ' Current resident data for update operations
    Private currentResidentData As Dictionary(Of String, String) = New Dictionary(Of String, String)

    ' Store the selected image path
    Private selectedImagePath As String = ""

    ' Flag to track if we're in update mode
    Private isUpdateMode As Boolean = False

    Private Sub ResidentForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load data into comboboxes
        LoadComboBoxData()

        ' Set default date to today
        Guna2DateTimePicker1.Value = DateTime.Now

        ' Generate and set the next available ID
        GenerateNextID()

        ' Make the ID textbox read-only
        Guna2TextBox2.ReadOnly = True

        ' Load data into DataGridView
        LoadDataGridView()

        ' Set up DataGridView event handlers
        AddHandler Guna2DataGridView1.CellClick, AddressOf DataGridView1_CellClick

        ' Set up search textbox event handler
        AddHandler Guna2TextBox7.KeyPress, AddressOf TextBox7_KeyPress
    End Sub

    Private Sub LoadComboBoxData()
        ' Load Sitio/Purok data
        Guna2ComboBox4.Items.Clear()
        Guna2ComboBox4.Items.AddRange({"Purok 1", "Purok 2", "Purok 3", "Purok 4", "Purok 5", "Purok 6", "Purok 7", "Purok 8"})

        ' Load Blood Type data
        Guna2ComboBox2.Items.Clear()
        Guna2ComboBox2.Items.AddRange({"A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"})

        ' Load Gender data
        Guna2ComboBox3.Items.Clear()
        Guna2ComboBox3.Items.AddRange({"Male", "Female"})

        ' Load Suffix data
        Guna2ComboBox1.Items.Clear()
        Guna2ComboBox1.Items.AddRange({"Jr.", "Sr.", "I", "II", "III", "IV", "V"})
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
        ' Combine first, middle, and last name into full name for display purposes
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

        ' Middle name is optional - no validation needed

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

        If Guna2ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("Please select Blood Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2ComboBox2.Focus()
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

                ' Check if resident already exists (using firstname, middlename, lastname combination)
                Dim checkQuery As String = "SELECT COUNT(*) FROM tbl_resident WHERE firstname = @firstname AND lastname = @lastname"
                If Not String.IsNullOrWhiteSpace(Guna2TextBox4.Text.Trim()) Then
                    checkQuery &= " AND (middlename = @middlename OR middlename IS NULL)"
                Else
                    checkQuery &= " AND (middlename IS NULL OR middlename = '')"
                End If

                Using checkCommand As New MySqlCommand(checkQuery, connection)
                    checkCommand.Parameters.AddWithValue("@firstname", Guna2TextBox3.Text.Trim())
                    checkCommand.Parameters.AddWithValue("@lastname", Guna2TextBox5.Text.Trim())
                    If Not String.IsNullOrWhiteSpace(Guna2TextBox4.Text.Trim()) Then
                        checkCommand.Parameters.AddWithValue("@middlename", Guna2TextBox4.Text.Trim())
                    End If

                    Dim existingCount As Integer = Convert.ToInt32(checkCommand.ExecuteScalar())

                    If existingCount > 0 Then
                        MessageBox.Show("A resident with this name already exists. Please use UPDATE instead.", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using

                ' Insert new resident
                Dim insertQuery As String = "INSERT INTO tbl_resident (id, firstname, middlename, lastname, `sitio/purok`, contactnumber, bloodtype, suffix, gender, birthdate, birthplace, photo) VALUES (@id, @firstname, @middlename, @lastname, @sitio, @contactnumber, @bloodtype, @suffix, @gender, @birthdate, @birthplace, @photo)"

                Using command As New MySqlCommand(insertQuery, connection)
                    command.Parameters.AddWithValue("@id", Guna2TextBox2.Text.Trim())
                    command.Parameters.AddWithValue("@firstname", Guna2TextBox3.Text.Trim())
                    command.Parameters.AddWithValue("@middlename", If(String.IsNullOrWhiteSpace(Guna2TextBox4.Text.Trim()), DBNull.Value, Guna2TextBox4.Text.Trim()))
                    command.Parameters.AddWithValue("@lastname", Guna2TextBox5.Text.Trim())
                    command.Parameters.AddWithValue("@sitio", Guna2ComboBox4.Text)
                    command.Parameters.AddWithValue("@contactnumber", Guna2TextBox1.Text.Trim())
                    command.Parameters.AddWithValue("@bloodtype", Guna2ComboBox2.Text)
                    command.Parameters.AddWithValue("@suffix", If(Guna2ComboBox1.SelectedIndex >= 0, Guna2ComboBox1.Text, DBNull.Value))
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

                ' Refresh DataGridView
                LoadDataGridView()

                ClearForm()
                ' Generate next ID for the next entry
                GenerateNextID()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error saving resident: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        ' UPDATE button
        If Not isUpdateMode Then
            MessageBox.Show("Please select a resident from the list to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If Not ValidateInputs() Then
            Return
        End If

        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                ' Update resident
                Dim updateQuery As String = "UPDATE tbl_resident SET firstname = @firstname, middlename = @middlename, lastname = @lastname, `sitio/purok` = @sitio, contactnumber = @contactnumber, bloodtype = @bloodtype, suffix = @suffix, gender = @gender, birthdate = @birthdate, birthplace = @birthplace, photo = @photo WHERE id = @id"

                Using command As New MySqlCommand(updateQuery, connection)
                    command.Parameters.AddWithValue("@id", currentResidentData("id"))
                    command.Parameters.AddWithValue("@firstname", Guna2TextBox3.Text.Trim())
                    command.Parameters.AddWithValue("@middlename", If(String.IsNullOrWhiteSpace(Guna2TextBox4.Text.Trim()), DBNull.Value, Guna2TextBox4.Text.Trim()))
                    command.Parameters.AddWithValue("@lastname", Guna2TextBox5.Text.Trim())
                    command.Parameters.AddWithValue("@sitio", Guna2ComboBox4.Text)
                    command.Parameters.AddWithValue("@contactnumber", Guna2TextBox1.Text.Trim())
                    command.Parameters.AddWithValue("@bloodtype", Guna2ComboBox2.Text)
                    command.Parameters.AddWithValue("@suffix", If(Guna2ComboBox1.SelectedIndex >= 0, Guna2ComboBox1.Text, DBNull.Value))
                    command.Parameters.AddWithValue("@gender", Guna2ComboBox3.Text)
                    command.Parameters.AddWithValue("@birthdate", Guna2DateTimePicker1.Value)
                    command.Parameters.AddWithValue("@birthplace", Guna2TextBox6.Text.Trim())

                    ' Add photo parameter
                    If Not String.IsNullOrEmpty(selectedImagePath) AndAlso File.Exists(selectedImagePath) Then
                        Dim imageBytes As Byte() = File.ReadAllBytes(selectedImagePath)
                        command.Parameters.AddWithValue("@photo", imageBytes)
                    ElseIf Guna2PictureBox1.Image IsNot Nothing Then
                        ' If there's an image in the picture box but no file path, convert it to bytes
                        Using ms As New MemoryStream()
                            Guna2PictureBox1.Image.Save(ms, Guna2PictureBox1.Image.RawFormat)
                            command.Parameters.AddWithValue("@photo", ms.ToArray())
                        End Using
                    Else
                        command.Parameters.AddWithValue("@photo", DBNull.Value)
                    End If

                    command.ExecuteNonQuery()
                End Using

                MessageBox.Show("Resident updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' Refresh DataGridView
                LoadDataGridView()

                ' Clear form and reset to add mode
                ClearForm()
                isUpdateMode = False
                currentResidentData.Clear()
                GenerateNextID()
            End Using

        Catch ex As Exception
            MessageBox.Show("Error updating resident: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton4.Click
        ' DELETE button
        If Not isUpdateMode Then
            MessageBox.Show("Please select a resident from the list to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Confirm deletion
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this resident?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            Try
                Using connection As New MySqlConnection(connectionString)
                    connection.Open()

                    ' Delete resident
                    Dim deleteQuery As String = "DELETE FROM tbl_resident WHERE id = @id"

                    Using command As New MySqlCommand(deleteQuery, connection)
                        command.Parameters.AddWithValue("@id", currentResidentData("id"))
                        command.ExecuteNonQuery()
                    End Using

                    MessageBox.Show("Resident deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' Refresh DataGridView
                    LoadDataGridView()

                    ' Clear form and reset to add mode
                    ClearForm()
                    isUpdateMode = False
                    currentResidentData.Clear()
                    GenerateNextID()
                End Using

            Catch ex As Exception
                MessageBox.Show("Error deleting resident: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
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

    Private Sub LoadDataGridView()
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                ' Query to get all resident data
                Dim query As String = "SELECT id, firstname, middlename, lastname, `sitio/purok`, contactnumber, bloodtype, suffix, gender, birthdate, birthplace, photo FROM tbl_resident ORDER BY id"

                Using adapter As New MySqlDataAdapter(query, connection)
                    Dim dataTable As New DataTable()
                    adapter.Fill(dataTable)

                    ' Set up DataGridView columns
                    Guna2DataGridView1.Columns.Clear()
                    Guna2DataGridView1.Columns.Add("id", "ID")
                    Guna2DataGridView1.Columns.Add("firstname", "First Name")
                    Guna2DataGridView1.Columns.Add("middlename", "Middle Name")
                    Guna2DataGridView1.Columns.Add("lastname", "Last Name")
                    Guna2DataGridView1.Columns.Add("sitio_purok", "Sitio/Purok")
                    Guna2DataGridView1.Columns.Add("contactnumber", "Contact Number")
                    Guna2DataGridView1.Columns.Add("bloodtype", "Blood Type")
                    Guna2DataGridView1.Columns.Add("suffix", "Suffix")
                    Guna2DataGridView1.Columns.Add("gender", "Gender")
                    Guna2DataGridView1.Columns.Add("birthdate", "Birth Date")
                    Guna2DataGridView1.Columns.Add("birthplace", "Birth Place")

                    ' Set column widths
                    Guna2DataGridView1.Columns("id").Width = 60
                    Guna2DataGridView1.Columns("firstname").Width = 120
                    Guna2DataGridView1.Columns("middlename").Width = 120
                    Guna2DataGridView1.Columns("lastname").Width = 120
                    Guna2DataGridView1.Columns("sitio_purok").Width = 100
                    Guna2DataGridView1.Columns("contactnumber").Width = 120
                    Guna2DataGridView1.Columns("bloodtype").Width = 80
                    Guna2DataGridView1.Columns("suffix").Width = 60
                    Guna2DataGridView1.Columns("gender").Width = 80
                    Guna2DataGridView1.Columns("birthdate").Width = 100
                    Guna2DataGridView1.Columns("birthplace").Width = 120

                    ' Add data to DataGridView
                    For Each row As DataRow In dataTable.Rows
                        Dim birthDate As DateTime
                        If DateTime.TryParse(row("birthdate").ToString(), birthDate) Then
                            Guna2DataGridView1.Rows.Add(
                                row("id").ToString(),
                                row("firstname").ToString(),
                                If(row("middlename") IsNot DBNull.Value, row("middlename").ToString(), ""),
                                row("lastname").ToString(),
                                row("sitio/purok").ToString(),
                                row("contactnumber").ToString(),
                                row("bloodtype").ToString(),
                                If(row("suffix") IsNot DBNull.Value, row("suffix").ToString(), ""),
                                row("gender").ToString(),
                                birthDate.ToString("MM/dd/yyyy"),
                                row("birthplace").ToString()
                            )
                        Else
                            Guna2DataGridView1.Rows.Add(
                                row("id").ToString(),
                                row("firstname").ToString(),
                                If(row("middlename") IsNot DBNull.Value, row("middlename").ToString(), ""),
                                row("lastname").ToString(),
                                row("sitio/purok").ToString(),
                                row("contactnumber").ToString(),
                                row("bloodtype").ToString(),
                                If(row("suffix") IsNot DBNull.Value, row("suffix").ToString(), ""),
                                row("gender").ToString(),
                                "",
                                row("birthplace").ToString()
                            )
                        End If
                    Next
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            ' Get the selected row data
            Dim selectedRow As DataGridViewRow = Guna2DataGridView1.Rows(e.RowIndex)

            ' Store current data for update operations
            currentResidentData.Clear()
            currentResidentData.Add("id", selectedRow.Cells("id").Value.ToString())
            currentResidentData.Add("firstname", selectedRow.Cells("firstname").Value.ToString())
            currentResidentData.Add("middlename", If(selectedRow.Cells("middlename").Value IsNot Nothing, selectedRow.Cells("middlename").Value.ToString(), ""))
            currentResidentData.Add("lastname", selectedRow.Cells("lastname").Value.ToString())
            currentResidentData.Add("sitio_purok", selectedRow.Cells("sitio_purok").Value.ToString())
            currentResidentData.Add("contactnumber", selectedRow.Cells("contactnumber").Value.ToString())
            currentResidentData.Add("bloodtype", selectedRow.Cells("bloodtype").Value.ToString())
            currentResidentData.Add("suffix", If(selectedRow.Cells("suffix").Value IsNot Nothing, selectedRow.Cells("suffix").Value.ToString(), ""))
            currentResidentData.Add("gender", selectedRow.Cells("gender").Value.ToString())
            currentResidentData.Add("birthdate", selectedRow.Cells("birthdate").Value.ToString())
            currentResidentData.Add("birthplace", selectedRow.Cells("birthplace").Value.ToString())

            ' Populate form fields with selected data
            Guna2TextBox2.Text = currentResidentData("id")
            Guna2TextBox3.Text = currentResidentData("firstname")
            Guna2TextBox4.Text = currentResidentData("middlename")
            Guna2TextBox5.Text = currentResidentData("lastname")
            Guna2ComboBox4.Text = currentResidentData("sitio_purok")
            Guna2TextBox1.Text = currentResidentData("contactnumber")
            Guna2ComboBox2.Text = currentResidentData("bloodtype")
            Guna2ComboBox1.Text = currentResidentData("suffix")
            Guna2ComboBox3.Text = currentResidentData("gender")

            ' Parse and set birth date
            Dim birthDate As DateTime
            If DateTime.TryParse(currentResidentData("birthdate"), birthDate) Then
                Guna2DateTimePicker1.Value = birthDate
            End If

            Guna2TextBox6.Text = currentResidentData("birthplace")

            ' Load photo from database
            LoadPhotoFromDatabase(currentResidentData("id"))

            ' Set update mode
            isUpdateMode = True
        End If
    End Sub

    Private Sub LoadPhotoFromDatabase(residentId As String)
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                Dim query As String = "SELECT photo FROM tbl_resident WHERE id = @id"
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@id", residentId)

                    Dim result As Object = command.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not Convert.IsDBNull(result) Then
                        Dim imageBytes As Byte() = DirectCast(result, Byte())
                        Using ms As New MemoryStream(imageBytes)
                            Guna2PictureBox1.Image = Image.FromStream(ms)
                            Guna2PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
                        End Using
                    Else
                        Guna2PictureBox1.Image = Nothing
                    End If
                End Using
            End Using

        Catch ex As Exception
            ' If there's an error loading the photo, just clear the picture box
            Guna2PictureBox1.Image = Nothing
        End Try
    End Sub

    Private Sub TextBox7_KeyPress(sender As Object, e As KeyPressEventArgs)
        ' Handle Enter key press in search textbox
        If e.KeyChar = ChrW(Keys.Enter) Then
            e.Handled = True
            PerformSearch()
        End If
    End Sub

    Private Sub PerformSearch()
        Dim searchTerm As String = Guna2TextBox7.Text.Trim()
        Dim searchBy As String = If(Guna2ComboBox5.SelectedItem IsNot Nothing, Guna2ComboBox5.SelectedItem.ToString(), "First Name")

        If String.IsNullOrEmpty(searchTerm) Then
            ' If search term is empty, load all data
            LoadDataGridView()
            Return
        End If

        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()

                ' Build search query based on selected search type
                Dim searchQuery As String = ""
                Select Case searchBy
                    Case "NameInDocument"
                        searchQuery = "SELECT id, firstname, middlename, lastname, `sitio/purok`, contactnumber, bloodtype, suffix, gender, birthdate, birthplace, photo FROM tbl_resident WHERE firstname LIKE @searchTerm ORDER BY id"
                    Case "Sitio/Purok"
                        searchQuery = "SELECT id, firstname, middlename, lastname, `sitio/purok`, contactnumber, bloodtype, suffix, gender, birthdate, birthplace, photo FROM tbl_resident WHERE `sitio/purok` LIKE @searchTerm ORDER BY id"
                    Case "Contact Number"
                        searchQuery = "SELECT id, firstname, middlename, lastname, `sitio/purok`, contactnumber, bloodtype, suffix, gender, birthdate, birthplace, photo FROM tbl_resident WHERE contactnumber LIKE @searchTerm ORDER BY id"
                    Case Else
                        searchQuery = "SELECT id, firstname, middlename, lastname, `sitio/purok`, contactnumber, bloodtype, suffix, gender, birthdate, birthplace, photo FROM tbl_resident WHERE firstname LIKE @searchTerm ORDER BY id"
                End Select

                Using adapter As New MySqlDataAdapter(searchQuery, connection)
                    adapter.SelectCommand.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")

                    Dim dataTable As New DataTable()
                    adapter.Fill(dataTable)

                    ' Clear existing data
                    Guna2DataGridView1.Rows.Clear()

                    ' Add search results to DataGridView
                    For Each row As DataRow In dataTable.Rows
                        Dim birthDate As DateTime
                        If DateTime.TryParse(row("birthdate").ToString(), birthDate) Then
                            Guna2DataGridView1.Rows.Add(
                                row("id").ToString(),
                                row("firstname").ToString(),
                                If(row("middlename") IsNot DBNull.Value, row("middlename").ToString(), ""),
                                row("lastname").ToString(),
                                row("sitio/purok").ToString(),
                                row("contactnumber").ToString(),
                                row("bloodtype").ToString(),
                                If(row("suffix") IsNot DBNull.Value, row("suffix").ToString(), ""),
                                row("gender").ToString(),
                                birthDate.ToString("MM/dd/yyyy"),
                                row("birthplace").ToString()
                            )
                        Else
                            Guna2DataGridView1.Rows.Add(
                                row("id").ToString(),
                                row("firstname").ToString(),
                                If(row("middlename") IsNot DBNull.Value, row("middlename").ToString(), ""),
                                row("lastname").ToString(),
                                row("sitio/purok").ToString(),
                                row("contactnumber").ToString(),
                                row("bloodtype").ToString(),
                                If(row("suffix") IsNot DBNull.Value, row("suffix").ToString(), ""),
                                row("gender").ToString(),
                                "",
                                row("birthplace").ToString()
                            )
                        End If
                    Next
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Error performing search: " & ex.Message, "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Guna2GradientButton5_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton5.Click
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
                isUpdateMode = False

                ' Generate next ID for future use
                GenerateNextID()
            End If

        Catch ex As Exception
            MessageBox.Show("Error returning to dashboard: " & ex.Message, "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        ' REFRESH button - Clear all form controls
        Try
            ClearForm()
            ' Clear search controls
            Guna2ComboBox5.SelectedIndex = -1
            Guna2TextBox7.Clear()
            ' Reset update mode
            isUpdateMode = False
            currentResidentData.Clear()
            GenerateNextID()
            ' Reload DataGridView to show all data
            LoadDataGridView()
            MessageBox.Show("Form refreshed successfully!", "Refresh Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error refreshing form: " & ex.Message, "Refresh Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Guna2TextBox3_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox3.TextChanged

    End Sub

    Private Sub Guna2TextBox4_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox4.TextChanged

    End Sub

    Private Sub Guna2TextBox5_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox5.TextChanged

    End Sub

    Private Sub Guna2ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox1.SelectedIndexChanged

    End Sub

    Private Sub Guna2ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox2.SelectedIndexChanged

    End Sub

    Private Sub Guna2ComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox5.SelectedIndexChanged

    End Sub
End Class