Imports MySql.Data.MySqlClient
Imports System.IO

Public Class Form1
    ' Database connection string
    Private connectionString As String = "Server=127.0.0.1;Port=3306;Database=cap_101;Uid=root;Pwd=johnarbenanguring;SslMode=none;"
    
    ' File path for storing remembered credentials
    Private credentialsFile As String = Path.Combine(Application.StartupPath, "remembered_credentials.txt")

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load remembered credentials if they exist
        LoadRememberedCredentials()
        
        ' Set password textbox to use password character
        Guna2TextBox2.UseSystemPasswordChar = True
    End Sub

    Private Sub LoadRememberedCredentials()
        Try
            If File.Exists(credentialsFile) Then
                Dim lines = File.ReadAllLines(credentialsFile)
                If lines.Length >= 2 Then
                    Guna2TextBox1.Text = lines(0)
                    Guna2TextBox2.Text = lines(1)
                    Guna2ToggleSwitch1.Checked = True
                End If
            End If
        Catch ex As Exception
            ' Silently handle any errors when loading credentials
        End Try
    End Sub

    Private Sub SaveRememberedCredentials()
        Try
            If Guna2ToggleSwitch1.Checked Then
                ' Save username and password
                File.WriteAllLines(credentialsFile, {Guna2TextBox1.Text, Guna2TextBox2.Text})
            Else
                ' Remove saved credentials if toggle is off
                If File.Exists(credentialsFile) Then
                    File.Delete(credentialsFile)
                End If
            End If
        Catch ex As Exception
            ' Silently handle any errors when saving credentials
        End Try
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        ' Validate input
        If String.IsNullOrWhiteSpace(Guna2TextBox1.Text) Then
            MessageBox.Show("Please enter a username.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2TextBox1.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(Guna2TextBox2.Text) Then
            MessageBox.Show("Please enter a password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Guna2TextBox2.Focus()
            Return
        End If

        ' Attempt to login
        If ValidateLogin(Guna2TextBox1.Text.Trim(), Guna2TextBox2.Text) Then
            ' Save remembered credentials if toggle is on
            SaveRememberedCredentials()
            
            ' Show success message
            MessageBox.Show("Login successful! Welcome " & Guna2TextBox1.Text.Trim(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            
            ' Hide login form and show dashboard
            Me.Hide()
            Dashboard.Show()
        Else
            MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Guna2TextBox2.Clear()
            Guna2TextBox2.Focus()
        End If
    End Sub

    Private Function ValidateLogin(username As String, password As String) As Boolean
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                
                ' Query to check username and password in the log_in table
                Dim query As String = "SELECT COUNT(*) FROM log_in WHERE username = @username AND password = @password"
                
                Using command As New MySqlCommand(query, connection)
                    command.Parameters.AddWithValue("@username", username)
                    command.Parameters.AddWithValue("@password", password)
                    
                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Database connection error: " & ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Private Sub Guna2ToggleSwitch1_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch1.CheckedChanged
        ' If toggle is turned off, remove saved credentials
        If Not Guna2ToggleSwitch1.Checked Then
            Try
                If File.Exists(credentialsFile) Then
                    File.Delete(credentialsFile)
                End If
            Catch ex As Exception
                ' Silently handle any errors
            End Try
        End If
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        ' Forgot Password functionality
        MessageBox.Show("Please contact your system administrator to reset your password.", "Forgot Password", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Guna2TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Guna2TextBox1.KeyPress
        ' Allow Enter key to move to password field
        If e.KeyChar = ChrW(Keys.Enter) Then
            Guna2TextBox2.Focus()
            e.Handled = True
        End If
    End Sub

    Private Sub Guna2TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Guna2TextBox2.KeyPress
        ' Allow Enter key to trigger login
        If e.KeyChar = ChrW(Keys.Enter) Then
            Guna2GradientButton1.PerformClick()
            e.Handled = True
        End If
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged
        ' Clear any error styling when user starts typing
    End Sub

    Private Sub Guna2TextBox2_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox2.TextChanged
        ' Clear any error styling when user starts typing
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click, Label3.Click
        ' Handle label clicks if needed
    End Sub

    Private Sub Guna2PictureBox2_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox2.Click
        ' Handle picture box clicks if needed
    End Sub
End Class
