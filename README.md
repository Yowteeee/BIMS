# BIMS - Login Form Implementation

## Overview
This VB.NET application includes a complete login form with database authentication and "Remember Me" functionality.

## Features
- **Username/Password Authentication**: Validates credentials against MySQL database
- **Remember Me**: Saves and auto-fills login credentials
- **Database Integration**: Connects to MySQL database for user validation
- **Modern UI**: Built with Guna UI2 components for a beautiful interface

## Database Setup

### 1. MySQL Database Requirements
- **Server**: 127.0.0.1
- **Port**: 3306
- **Database**: cap_101
- **Username**: root
- **Password**: johnarbenanguring

### 2. Required Table Structure
Create the following table in your MySQL database:

```sql
CREATE TABLE log_in (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### 3. Sample Data
Insert test users into the database:

```sql
INSERT INTO log_in (username, password) VALUES 
('admin', 'admin123'),
('user1', 'password123'),
('test', 'test123');
```

## Package Dependencies

### Required NuGet Packages
1. **Guna.UI2.WinForms** (already included)
2. **MySql.Data** (version 8.3.0) - **NEEDS TO BE INSTALLED**

### Installing MySQL Connector
1. Open your project in Visual Studio
2. Right-click on your project in Solution Explorer
3. Select "Manage NuGet Packages"
4. Go to "Browse" tab
5. Search for "MySql.Data"
6. Install version 8.3.0

Alternatively, use Package Manager Console:
```
Install-Package MySql.Data -Version 8.3.0
```

## Form Controls

### Main Controls
- **Guna2TextBox1**: Username input field
- **Guna2TextBox2**: Password input field (masked)
- **Guna2ToggleSwitch1**: Remember Me toggle
- **Guna2GradientButton1**: Login button

### Additional Features
- **Enter Key Support**: Press Enter in username field to move to password, Enter in password to login
- **Auto-fill**: Remembers credentials when toggle is enabled
- **Error Handling**: Shows appropriate error messages for invalid inputs
- **Database Connection**: Validates credentials against MySQL database

## Usage

### Login Process
1. Enter username in the first textbox
2. Enter password in the second textbox
3. Optionally check "Remember Me" to save credentials
4. Click "LOGIN" button or press Enter
5. If credentials are valid, the dashboard will open

### Remember Me Functionality
- When enabled, credentials are saved to a local file
- On next application start, fields are auto-filled
- Toggle can be turned off to remove saved credentials

## Error Handling
- **Empty Fields**: Shows warning messages for missing username/password
- **Invalid Credentials**: Shows error message for wrong username/password
- **Database Errors**: Shows connection error messages
- **File Errors**: Silently handles credential file operations

## Security Notes
- Passwords are stored in plain text in the database (consider hashing for production)
- Credentials are saved locally in plain text (consider encryption for production)
- Database connection string is hardcoded (consider configuration file for production)

## Troubleshooting

### Common Issues
1. **Database Connection Failed**
   - Verify MySQL server is running
   - Check connection string parameters
   - Ensure database and table exist

2. **Package Not Found**
   - Install MySql.Data package via NuGet
   - Rebuild the solution

3. **Login Not Working**
   - Verify table structure matches requirements
   - Check if user exists in database
   - Ensure correct username/password combination

## File Structure
```
BIMS/
├── Form1.vb              # Main login form code
├── Form1.Designer.vb     # Form design
├── packages.config       # NuGet packages
└── remembered_credentials.txt  # Saved credentials (auto-generated)
```

## Development Notes
- Built with VB.NET and .NET Framework 4.7.2
- Uses Guna UI2 for modern interface components
- MySQL Connector for database operations
- File I/O for credential persistence